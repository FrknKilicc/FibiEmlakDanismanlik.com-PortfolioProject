using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using SkiaSharp;

namespace FibiEmlakDanismanlik.Infrastructure.Imaging
{
    /// <summary>
    /// SkiaSharp kütüphanesini kullanarak görsel doğrulama, EXIF yönlendirme ve WebP formatında
    /// farklı çözünürlüklerde yeniden kodlama işlemlerini gerçekleştiren servis.
    /// </summary>
    public class SkiaImageProcessor : IImageProcessor
    {
        public async Task<ProcessedImage> ProcessAsync(Stream input, CancellationToken ct)
        {
            if (input == null)
            {
                throw new ImageValidationException("Görsel dosyası akışı boş olamaz.");
            }

            if (input.CanSeek && input.Length > ListingImageConstants.MaxUploadBytes)
            {
                throw new ImageValidationException("Dosya en fazla 15 MB olabilir.");
            }

            using var memoryStream = new MemoryStream();
            await input.CopyToAsync(memoryStream, ct);

            if (memoryStream.Length == 0)
            {
                throw new ImageValidationException("Görsel dosyası boş olamaz.");
            }

            if (memoryStream.Length > ListingImageConstants.MaxUploadBytes)
            {
                throw new ImageValidationException("Dosya en fazla 15 MB olabilir.");
            }

            byte[] buffer = memoryStream.ToArray();

            ValidateMagicBytes(buffer);

            using var skData = SKData.CreateCopy(buffer);
            using var codec = SKCodec.Create(skData);
            if (codec == null)
            {
                throw new ImageValidationException("Görsel dosyası çözümlenemedi veya bozuk.");
            }

            if (codec.Info.Width <= 0 || codec.Info.Height <= 0 ||
                codec.Info.Width > ListingImageConstants.MaxPixelDimension ||
                codec.Info.Height > ListingImageConstants.MaxPixelDimension)
            {
                throw new ImageValidationException($"Görsel boyutları en fazla {ListingImageConstants.MaxPixelDimension} piksel olabilir.");
            }

            var origin = codec.EncodedOrigin;
            var decodedBitmap = SKBitmap.Decode(codec);
            if (decodedBitmap == null)
            {
                throw new ImageValidationException("Görsel pikselleri çözümlenemedi.");
            }

            using var orientedBitmap = OrientBitmap(decodedBitmap, origin);

            if (orientedBitmap.Width <= 0 || orientedBitmap.Height <= 0 ||
                orientedBitmap.Width > ListingImageConstants.MaxPixelDimension ||
                orientedBitmap.Height > ListingImageConstants.MaxPixelDimension)
            {
                throw new ImageValidationException($"Görsel boyutları en fazla {ListingImageConstants.MaxPixelDimension} piksel olabilir.");
            }

            // En büyük çıktının boyutlarını hesapla (büyütme yapılmaz)
            int largeWidth = orientedBitmap.Width;
            int largeHeight = orientedBitmap.Height;
            if (orientedBitmap.Width > ListingImageConstants.LargeWidth)
            {
                largeWidth = ListingImageConstants.LargeWidth;
                largeHeight = (int)Math.Round((double)orientedBitmap.Height * ListingImageConstants.LargeWidth / orientedBitmap.Width);
                if (largeHeight < 1) largeHeight = 1;
            }

            byte[] largeBytes = EncodeWebp(orientedBitmap, ListingImageConstants.LargeWidth);
            byte[] mediumBytes = EncodeWebp(orientedBitmap, ListingImageConstants.MediumWidth);
            byte[] thumbBytes = EncodeWebp(orientedBitmap, ListingImageConstants.ThumbWidth);

            return new ProcessedImage
            {
                Width = largeWidth,
                Height = largeHeight,
                Large = largeBytes,
                Medium = mediumBytes,
                Thumb = thumbBytes
            };
        }

        private static void ValidateMagicBytes(byte[] bytes)
        {
            if (bytes.Length < 12)
            {
                throw new ImageValidationException("Yalnızca JPEG, PNG veya WebP yükleyebilirsiniz.");
            }

            // JPEG: FF D8 FF
            bool isJpeg = bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            bool isPng = bytes.Length >= 8 &&
                         bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
                         bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;

            // WebP: RIFF (bytes 0-3) + 4 bytes + WEBP (bytes 8-11)
            bool isWebp = bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                          bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50;

            if (!isJpeg && !isPng && !isWebp)
            {
                throw new ImageValidationException("Yalnızca JPEG, PNG veya WebP yükleyebilirsiniz.");
            }
        }

        private static SKBitmap OrientBitmap(SKBitmap source, SKEncodedOrigin origin)
        {
            if (origin == SKEncodedOrigin.TopLeft || origin == SKEncodedOrigin.Default)
            {
                return source;
            }

            bool swapDimensions = origin switch
            {
                SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop or
                SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom => true,
                _ => false
            };

            int targetWidth = swapDimensions ? source.Height : source.Width;
            int targetHeight = swapDimensions ? source.Width : source.Height;

            var alphaType = source.AlphaType == SKAlphaType.Opaque ? SKAlphaType.Opaque : SKAlphaType.Premul;
            var targetInfo = new SKImageInfo(targetWidth, targetHeight, SKColorType.Rgba8888, alphaType);
            var orientedBitmap = new SKBitmap(targetInfo);

            using (var canvas = new SKCanvas(orientedBitmap))
            {
                switch (origin)
                {
                    case SKEncodedOrigin.TopRight: // Yatay aynalama
                        canvas.Translate(targetWidth, 0);
                        canvas.Scale(-1, 1);
                        break;
                    case SKEncodedOrigin.BottomRight: // 180 derece döndürme
                        canvas.Translate(targetWidth, targetHeight);
                        canvas.RotateDegrees(180);
                        break;
                    case SKEncodedOrigin.BottomLeft: // Dikey aynalama
                        canvas.Translate(0, targetHeight);
                        canvas.Scale(1, -1);
                        break;
                    case SKEncodedOrigin.LeftTop: // Transpose
                        canvas.RotateDegrees(90);
                        canvas.Scale(1, -1);
                        break;
                    case SKEncodedOrigin.RightTop: // 90 derece saat yönünde döndürme
                        canvas.Translate(targetWidth, 0);
                        canvas.RotateDegrees(90);
                        break;
                    case SKEncodedOrigin.RightBottom: // Transverse
                        canvas.Translate(targetWidth, 0);
                        canvas.Scale(-1, 1);
                        canvas.Translate(0, targetHeight);
                        canvas.RotateDegrees(270);
                        break;
                    case SKEncodedOrigin.LeftBottom: // 270 derece saat yönünde döndürme
                        canvas.Translate(0, targetHeight);
                        canvas.RotateDegrees(270);
                        break;
                }

                using var sourceImage = SKImage.FromBitmap(source);
                canvas.DrawImage(sourceImage, 0, 0);
                canvas.Flush();
            }

            source.Dispose();
            return orientedBitmap;
        }

        private static byte[] EncodeWebp(SKBitmap source, int maxWidth)
        {
            int targetWidth = source.Width;
            int targetHeight = source.Height;

            // Görsel hedef genişlikten küçükse büyütme YAPMA (orijinal genişlikte kodla)
            if (source.Width > maxWidth)
            {
                targetWidth = maxWidth;
                targetHeight = (int)Math.Round((double)source.Height * maxWidth / source.Width);
                if (targetHeight < 1) targetHeight = 1;
            }

            // Boyut değişmiyorsa doğrudan orijinal piksellerden WebP kodla
            if (targetWidth == source.Width && targetHeight == source.Height)
            {
                using var fullImage = SKImage.FromBitmap(source);
                using var encodedFull = fullImage.Encode(SKEncodedImageFormat.Webp, ListingImageConstants.WebpQuality);
                if (encodedFull == null)
                {
                    throw new ImageValidationException("Görsel WebP formatında kodlanamadı.");
                }
                return encodedFull.ToArray();
            }

            var alphaType = source.AlphaType == SKAlphaType.Opaque ? SKAlphaType.Opaque : SKAlphaType.Premul;
            var targetInfo = new SKImageInfo(targetWidth, targetHeight, SKColorType.Rgba8888, alphaType);
            using var resizedBitmap = new SKBitmap(targetInfo);

            using (var canvas = new SKCanvas(resizedBitmap))
            using (var paint = new SKPaint { IsAntialias = true })
            {
                var destRect = new SKRect(0, 0, targetWidth, targetHeight);
                using var sourceImage = SKImage.FromBitmap(source);
                var sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);
                canvas.DrawImage(sourceImage, destRect, sampling, paint);
                canvas.Flush();
            }

            using var finalImage = SKImage.FromBitmap(resizedBitmap);
            using var encodedData = finalImage.Encode(SKEncodedImageFormat.Webp, ListingImageConstants.WebpQuality);
            if (encodedData == null)
            {
                throw new ImageValidationException("Görsel WebP formatında kodlanamadı.");
            }

            return encodedData.ToArray();
        }
    }
}
