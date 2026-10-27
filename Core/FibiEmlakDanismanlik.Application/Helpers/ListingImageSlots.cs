using FibiEmlakDanismanlik.Application.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>
    /// Sonuç/DTO nesnelerindeki PropImgUrl1..30 alanlarını verilen fotoğraf listesiyle doldurur
    /// (eski kolon kuralı: 1. slot görsel yoksa "", diğer boş slotlar null).
    /// </summary>
    public static class ListingImageSlots
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo?[]> SlotPropertiesCache = new();

        /// <summary>
        /// Hedef nesnedeki PropImgUrl1..PropImgUrl30 alanlarına fotoğraf adreslerini yazar.
        /// </summary>
        /// <param name="target">Fotoğraf adreslerinin atanacağı nesne.</param>
        /// <param name="urls">Fotoğraf adresleri listesi.</param>
        public static void Apply(object target, IReadOnlyList<string>? urls)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var properties = SlotPropertiesCache.GetOrAdd(target.GetType(), static type =>
            {
                var array = new PropertyInfo?[ListingImageConstants.MaxImagesPerListing];
                for (int i = 1; i <= ListingImageConstants.MaxImagesPerListing; i++)
                {
                    var prop = type.GetProperty($"PropImgUrl{i}", BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
                    {
                        array[i - 1] = prop;
                    }
                }
                return array;
            });

            int count = urls?.Count ?? 0;
            for (int i = 1; i <= ListingImageConstants.MaxImagesPerListing; i++)
            {
                var prop = properties[i - 1];
                if (prop == null)
                {
                    continue;
                }

                string? value;
                if (i - 1 < count)
                {
                    value = urls![i - 1];
                }
                else if (i == 1)
                {
                    value = string.Empty;
                }
                else
                {
                    value = null;
                }

                prop.SetValue(target, value);
            }
        }

        public static List<string> ToList(IReadOnlyList<string>? urls) => urls?.ToList() ?? new List<string>();
    }
}
