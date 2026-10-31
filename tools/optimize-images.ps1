<#
.SYNOPSIS
    Ana sayfa banner ve genel JPEG görsellerini en-boy oranını koruyarak yeniden boyutlandırır ve sıkıştırır.

.DESCRIPTION
    Belirtilen JPEG dosyalarını açar, varsa EXIF Orientation bilgisine göre yönlendirir,
    genişliği MaxWidth değerinden büyükse oranı koruyarak küçültür ve belirtilen kalite
    seviyesinde JPEG olarak yeniden kaydeder. Yeni dosya boyutu eskisinden küçükse yerinde
    günceller, değilse işlemi atlar.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools/optimize-images.ps1 -Path <dosya1>,<dosya2>

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools/optimize-images.ps1 -Path "Frontends/FibiEmlakDanismanlik.WebUI/wwwroot/Realshed/assets/images/banner/fibiBanner.jpg" -MaxWidth 1920 -Quality 80
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string[]]$Path,

    [int]$MaxWidth = 1920,

    [ValidateRange(1, 100)]
    [int]$Quality = 80,

    [switch]$WhatIf
)

Add-Type -AssemblyName System.Drawing

$culture = [System.Globalization.CultureInfo]::GetCultureInfo('tr-TR')

function Format-KB([long]$bytes) {
    $kb = [long][Math]::Round($bytes / 1KB)
    if ($bytes -gt 0 -and $kb -lt 1) {
        $kb = 1
    }
    return ($kb.ToString('N0', $culture) + ' KB')
}

$jpegCodec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() |
    Where-Object { $_.MimeType -eq 'image/jpeg' } |
    Select-Object -First 1

if ($null -eq $jpegCodec) {
    Write-Error "Sistemde JPEG codec (image/jpeg) bulunamadı."
    exit 1
}

$failedCount = 0

foreach ($p in $Path) {
    if ([string]::IsNullOrWhiteSpace($p)) {
        continue
    }

    if (-not (Test-Path -LiteralPath $p -PathType Leaf)) {
        Write-Warning "Dosya bulunamadı: $p"
        $failedCount++
        continue
    }

    $fileItem = Get-Item -LiteralPath $p
    $ext = $fileItem.Extension.ToLowerInvariant()
    if ($ext -ne '.jpg' -and $ext -ne '.jpeg') {
        Write-Warning "Desteklenmeyen dosya uzantısı ($ext): $($fileItem.FullName) (Yalnızca .jpg ve .jpeg desteklenir)"
        $failedCount++
        continue
    }

    $resolvedPath = $fileItem.FullName
    $dirName = $fileItem.DirectoryName
    $baseName = $fileItem.BaseName
    $tempPath = [System.IO.Path]::Combine($dirName, "$baseName.tmp.jpg")

    $origSize = $fileItem.Length
    $origWidth = 0
    $origHeight = 0
    $newWidth = 0
    $newHeight = 0

    $img = $null
    $destBitmap = $null
    $graphics = $null
    $imageAttr = $null
    $encoderParams = $null
    $processSuccess = $false

    try {
        if (Test-Path -LiteralPath $tempPath) {
            Remove-Item -LiteralPath $tempPath -Force -ErrorAction SilentlyContinue
        }

        # 1. Görseli aç ve EXIF Orientation varsa döndür
        $img = [System.Drawing.Image]::FromFile($resolvedPath)

        if ($img.PropertyIdList -contains 0x0112) {
            try {
                $prop = $img.GetPropertyItem(0x0112)
                $orientation = [System.BitConverter]::ToUInt16($prop.Value, 0)
                switch ($orientation) {
                    1 { break }
                    2 { $img.RotateFlip([System.Drawing.RotateFlipType]::RotateNoneFlipX); break }
                    3 { $img.RotateFlip([System.Drawing.RotateFlipType]::Rotate180FlipNone); break }
                    4 { $img.RotateFlip([System.Drawing.RotateFlipType]::Rotate180FlipX); break }
                    5 { $img.RotateFlip([System.Drawing.RotateFlipType]::Rotate90FlipX); break }
                    6 { $img.RotateFlip([System.Drawing.RotateFlipType]::Rotate90FlipNone); break }
                    7 { $img.RotateFlip([System.Drawing.RotateFlipType]::Rotate270FlipX); break }
                    8 { $img.RotateFlip([System.Drawing.RotateFlipType]::Rotate270FlipNone); break }
                }
                $img.RemovePropertyItem(0x0112)
            } catch {
                # EXIF property okuma/kaldırma hatası olursa devam et
            }
        }

        $origWidth = $img.Width
        $origHeight = $img.Height

        # 2. Yeni boyutu hesapla
        if ($origWidth -gt $MaxWidth) {
            $newWidth = $MaxWidth
            $newHeight = [int][Math]::Round($origHeight * ($MaxWidth / [double]$origWidth))
            if ($newHeight -lt 1) {
                $newHeight = 1
            }
        } else {
            $newWidth = $origWidth
            $newHeight = $origHeight
        }

        # 3. Yeni Bitmap üzerine Graphics ile çiz
        $destBitmap = New-Object System.Drawing.Bitmap($newWidth, $newHeight)
        $graphics = [System.Drawing.Graphics]::FromImage($destBitmap)
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality

        $imageAttr = New-Object System.Drawing.Imaging.ImageAttributes
        $imageAttr.SetWrapMode([System.Drawing.Drawing2D.WrapMode]::TileFlipXY)

        $destRect = New-Object System.Drawing.Rectangle(0, 0, $newWidth, $newHeight)
        $graphics.DrawImage(
            $img,
            $destRect,
            0,
            0,
            $origWidth,
            $origHeight,
            [System.Drawing.GraphicsUnit]::Pixel,
            $imageAttr
        )

        # 4. JPEG encoder ve Quality ile geçici dosyaya kaydet
        $encoderParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
        $encoderParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter(
            [System.Drawing.Imaging.Encoder]::Quality,
            [long]$Quality
        )

        $destBitmap.Save($tempPath, $jpegCodec, $encoderParams)
        $processSuccess = $true
    } catch {
        Write-Warning "Dosya işlenirken hata oluştu: $($fileItem.FullName) - $($_.Exception.Message)"
        $failedCount++
    } finally {
        # 5. Kaynak görsel ve Graphics/Bitmap nesnelerini Dispose et (dosya kilidi kalmasın)
        if ($null -ne $imageAttr) { $imageAttr.Dispose(); $imageAttr = $null }
        if ($null -ne $graphics) { $graphics.Dispose(); $graphics = $null }
        if ($null -ne $destBitmap) { $destBitmap.Dispose(); $destBitmap = $null }
        if ($null -ne $encoderParams) { $encoderParams.Dispose(); $encoderParams = $null }
        if ($null -ne $img) { $img.Dispose(); $img = $null }
    }

    if (-not $processSuccess) {
        if (Test-Path -LiteralPath $tempPath) {
            Remove-Item -LiteralPath $tempPath -Force -ErrorAction SilentlyContinue
        }
        continue
    }

    # 6. Boyut kontrolü ve dosya taşıma / temizlik
    $tempItem = Get-Item -LiteralPath $tempPath
    $tempSize = $tempItem.Length

    $origFormatted = Format-KB $origSize
    $newFormatted = Format-KB $tempSize

    if ($tempSize -lt $origSize) {
        if ($WhatIf) {
            Remove-Item -LiteralPath $tempPath -Force -ErrorAction SilentlyContinue
            Write-Host "$($fileItem.Name): ${origWidth}x${origHeight} $origFormatted -> ${newWidth}x${newHeight} $newFormatted"
        } else {
            Move-Item -LiteralPath $tempPath -Destination $resolvedPath -Force
            Write-Host "$($fileItem.Name): ${origWidth}x${origHeight} $origFormatted -> ${newWidth}x${newHeight} $newFormatted"
        }
    } else {
        Remove-Item -LiteralPath $tempPath -Force -ErrorAction SilentlyContinue
        Write-Host "$($fileItem.Name): atlandı"
    }
}

if ($failedCount -gt 0) {
    exit 1
}
