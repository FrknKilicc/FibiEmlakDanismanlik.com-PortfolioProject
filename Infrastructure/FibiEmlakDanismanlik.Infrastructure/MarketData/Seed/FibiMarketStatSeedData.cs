using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Enums;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData.Seed
{
    /// <summary>
    /// Tahmini 2026 başlangıç emsal verisi. Resmi veri değildir; TCMB bülteni ve EVDS sağlayıcıları üzerine günceller.
    /// İstanbul ilçelerinde zemin sınıfı boştur, çünkü mahalle bazlı İBB deprem senaryosu kullanılır.
    /// Zemin tahmini yalnızca bilinen yüksek tehlikeli illerde verilir; düşük tehlike tahmini bilerek girilmez.
    /// </summary>
    public static class FibiMarketStatSeedData
    {
        public const string SeedPeriod = "2026-09";

        public static IReadOnlyList<MarketStatSeedRecord> All { get; } = new List<MarketStatSeedRecord>
        {
            // Türkiye
            Country(42000m, 175m, 22.3m, 115m, 1.00m, null, 50),

            // İller
            City(34, 72000m, 300m, 26.8m, 130m, 1.20m, null, 75),
            City(6, 38000m, 165m, 22.0m, 110m, 1.05m, null, 55),
            City(35, 56000m, 230m, 21.0m, 112m, 1.10m, SoilRiskClass.High, 60),
            City(16, 40000m, 165m, 22.1m, 110m, 1.05m, SoilRiskClass.High, 55),
            City(7, 55000m, 240m, 20.0m, 120m, 1.15m, null, 55),
            City(41, 40000m, 170m, 22.6m, 112m, 1.05m, SoilRiskClass.VeryHigh, 60),
            City(1, 28000m, 125m, 21.5m, 100m, 0.95m, null, 45),
            City(33, 32000m, 140m, 21.5m, 105m, 1.00m, null, 45),
            City(27, 26000m, 115m, 19.8m, 98m, 0.95m, null, 45),
            City(42, 27000m, 115m, 19.7m, 98m, 0.95m, null, 40),
            City(38, 28000m, 115m, 21.9m, 100m, 0.95m, null, 40),
            City(26, 33000m, 140m, 22.1m, 105m, 1.00m, null, 45),
            City(54, 32000m, 135m, 22.6m, 110m, 1.00m, SoilRiskClass.VeryHigh, 50),
            City(59, 36000m, 150m, 15.3m, 100m, 1.00m, SoilRiskClass.High, 50),
            City(48, 70000m, 270m, 19.3m, 120m, 1.20m, SoilRiskClass.High, 55),
            City(9, 38000m, 155m, 19.3m, 105m, 1.00m, SoilRiskClass.High, 45),
            City(20, 30000m, 125m, 19.3m, 100m, 0.95m, SoilRiskClass.High, 45),
            City(10, 34000m, 140m, 17.9m, 100m, 1.00m, SoilRiskClass.High, 45),
            City(45, 27000m, 115m, 17.9m, 95m, 0.95m, SoilRiskClass.High, 40),
            City(55, 30000m, 125m, 20.2m, 100m, 0.95m, null, 40),
            City(61, 32000m, 130m, 26.2m, 110m, 1.00m, null, 40),
            City(31, 24000m, 115m, 24.0m, 105m, 0.95m, SoilRiskClass.VeryHigh, 55),
            City(46, 22000m, 105m, 24.0m, 100m, 0.90m, SoilRiskClass.VeryHigh, 55),
            City(44, 21000m, 100m, 22.7m, 100m, 0.90m, SoilRiskClass.VeryHigh, 50),
            City(77, 42000m, 170m, 22.6m, 110m, 1.05m, SoilRiskClass.VeryHigh, 55),
            City(81, 28000m, 120m, 22.6m, 105m, 0.95m, SoilRiskClass.VeryHigh, 50),
            City(14, 28000m, 120m, 22.6m, 105m, 0.95m, SoilRiskClass.VeryHigh, 45),
            City(23, 21000m, 100m, 22.7m, 100m, 0.90m, SoilRiskClass.VeryHigh, 50),
            City(2, 18000m, 90m, 19.8m, 95m, 0.90m, SoilRiskClass.VeryHigh, 50),
            City(80, 18000m, 90m, 24.0m, 100m, 0.90m, SoilRiskClass.High, 45),
            City(17, 36000m, 150m, 17.9m, 100m, 1.00m, SoilRiskClass.High, 45),
            City(21, 20000m, 95m, 19.8m, 95m, 0.90m, null, 40),
            City(25, 22000m, 95m, 20.1m, 95m, 0.90m, null, 35),
            City(65, 20000m, 95m, 22.7m, 95m, 0.90m, SoilRiskClass.High, 45),

            // İstanbul İlçeleri
            IstanbulDistrict("ADALAR", 90000m, 280m, 1.15m, 40),
            IstanbulDistrict("ARNAVUTKÖY", 40000m, 170m, 1.25m, 55),
            IstanbulDistrict("ATAŞEHİR", 85000m, 360m, 1.20m, 70),
            IstanbulDistrict("AVCILAR", 48000m, 220m, 1.10m, 75),
            IstanbulDistrict("BAĞCILAR", 50000m, 230m, 1.05m, 80),
            IstanbulDistrict("BAHÇELİEVLER", 62000m, 270m, 1.10m, 80),
            IstanbulDistrict("BAKIRKÖY", 115000m, 420m, 1.20m, 85),
            IstanbulDistrict("BAŞAKŞEHİR", 58000m, 250m, 1.20m, 60),
            IstanbulDistrict("BAYRAMPAŞA", 58000m, 260m, 1.10m, 80),
            IstanbulDistrict("BEŞİKTAŞ", 140000m, 520m, 1.25m, 80),
            IstanbulDistrict("BEYKOZ", 95000m, 330m, 1.15m, 45),
            IstanbulDistrict("BEYLİKDÜZÜ", 50000m, 230m, 1.15m, 55),
            IstanbulDistrict("BEYOĞLU", 85000m, 380m, 1.20m, 70),
            IstanbulDistrict("BÜYÜKÇEKMECE", 55000m, 230m, 1.15m, 55),
            IstanbulDistrict("ÇATALCA", 40000m, 160m, 1.10m, 35),
            IstanbulDistrict("ÇEKMEKÖY", 65000m, 270m, 1.15m, 55),
            IstanbulDistrict("ESENLER", 45000m, 210m, 1.05m, 80),
            IstanbulDistrict("ESENYURT", 35000m, 180m, 1.05m, 65),
            IstanbulDistrict("EYÜPSULTAN", 70000m, 290m, 1.15m, 70),
            IstanbulDistrict("FATİH", 70000m, 320m, 1.10m, 85),
            IstanbulDistrict("GAZİOSMANPAŞA", 50000m, 230m, 1.05m, 80),
            IstanbulDistrict("GÜNGÖREN", 55000m, 250m, 1.05m, 80),
            IstanbulDistrict("KADIKÖY", 125000m, 480m, 1.25m, 90),
            IstanbulDistrict("KAĞITHANE", 70000m, 310m, 1.20m, 85),
            IstanbulDistrict("KARTAL", 70000m, 290m, 1.15m, 80),
            IstanbulDistrict("KÜÇÜKÇEKMECE", 52000m, 240m, 1.10m, 75),
            IstanbulDistrict("MALTEPE", 75000m, 310m, 1.15m, 85),
            IstanbulDistrict("PENDİK", 58000m, 250m, 1.15m, 70),
            IstanbulDistrict("SANCAKTEPE", 52000m, 230m, 1.15m, 60),
            IstanbulDistrict("SARIYER", 130000m, 470m, 1.20m, 65),
            IstanbulDistrict("SİLİVRİ", 42000m, 170m, 1.10m, 45),
            IstanbulDistrict("SULTANBEYLİ", 42000m, 200m, 1.05m, 65),
            IstanbulDistrict("SULTANGAZİ", 45000m, 210m, 1.05m, 70),
            IstanbulDistrict("ŞİLE", 55000m, 210m, 1.10m, 30),
            IstanbulDistrict("ŞİŞLİ", 100000m, 430m, 1.20m, 85),
            IstanbulDistrict("TUZLA", 58000m, 240m, 1.15m, 60),
            IstanbulDistrict("ÜMRANİYE", 70000m, 300m, 1.15m, 75),
            IstanbulDistrict("ÜSKÜDAR", 95000m, 380m, 1.20m, 80),
            IstanbulDistrict("ZEYTİNBURNU", 65000m, 290m, 1.10m, 85)
        };

        private static MarketStatSeedRecord Country(
            decimal sale,
            decimal rent,
            decimal g1,
            decimal g3,
            decimal dev,
            SoilRiskClass? soil,
            int appetite)
            => new(MarketStatScope.Country, null, null, sale, rent, g1, g3, dev, soil, appetite);

        private static MarketStatSeedRecord City(
            int plate,
            decimal sale,
            decimal rent,
            decimal g1,
            decimal g3,
            decimal dev,
            SoilRiskClass? soil,
            int appetite)
            => new(MarketStatScope.City, plate, null, sale, rent, g1, g3, dev, soil, appetite);

        private static MarketStatSeedRecord IstanbulDistrict(
            string name,
            decimal sale,
            decimal rent,
            decimal dev,
            int appetite)
            => new(MarketStatScope.District, 34, name, sale, rent, 26.8m, 130m, dev, null, appetite);
    }
}
