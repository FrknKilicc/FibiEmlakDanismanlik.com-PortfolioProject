using System;
using System.Collections.Generic;
using System.Linq;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models.ListingSpecs;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class ListingSpecsBuilderTests
    {
        [Fact]
        public void FromSale_NullDto_ReturnsDefaultVm()
        {
            // Null satılık DTO verildiğinde varsayılan boş model dönmelidir
            var vm = ListingSpecsBuilder.FromSale(null!);

            Assert.NotNull(vm);
            Assert.Equal("sale", vm.UsageKey);
            Assert.Empty(vm.KeyFacts);
            Assert.Empty(vm.Groups);
            Assert.Empty(vm.AmenityGroups);
            Assert.Empty(vm.Insights);
        }

        [Fact]
        public void FromRental_NullDto_ReturnsDefaultVm()
        {
            // Null kiralık DTO verildiğinde varsayılan boş model dönmelidir
            var vm = ListingSpecsBuilder.FromRental(null!);

            Assert.NotNull(vm);
            Assert.Empty(vm.KeyFacts);
            Assert.Empty(vm.Groups);
            Assert.Empty(vm.AmenityGroups);
            Assert.Empty(vm.Insights);
        }

        [Fact]
        public void FromSale_HousingSale_BuildsExpectedKeyFactsGroupsAndInsights()
        {
            // Konut satılık ilanı için temel özellikler, gruplar ve analiz kartları üretilmelidir
            var dto = new ResultAllForSaleListinForPageDto
            {
                ListingId = 101,
                PropertyNo = 5001,
                PropertyName = "Kadıköy Moda 3+1 Daire",
                ListingType = "Konut",
                Price = 6000000m,
                GrossArea = 120,
                NetArea = 100,
                NumberOfRoom = "3+1",
                NumberOfBathRoom = 2,
                NumberOfFloors = 3,
                TotalNumberOfFloor = 5,
                BuildingAge = "5-10",
                Heating = "Doğalgaz Kombi",
                Facade = "Güney",
                TitleDeedStatus = "Kat Mülkiyeti",
                HomeLoan = true,
                Exchange = false,
                Dues = 1500m
            };

            var vm = ListingSpecsBuilder.FromSale(dto);

            Assert.Equal("sale", vm.UsageKey);
            Assert.Equal("housing", vm.CategoryKey);

            // KeyFacts kontrolü (m² Fiyatı: 6.000.000 / 120 = 50.000 ₺)
            Assert.Contains(vm.KeyFacts, f => f.Label == "Brüt / Net" && f.Value == "120 / 100 m²");
            Assert.Contains(vm.KeyFacts, f => f.Label == "Oda" && f.Value == "3+1");
            Assert.Contains(vm.KeyFacts, f => f.Label == "m² Fiyatı" && f.Value.Contains("50.000"));

            // Groups kontrolü
            Assert.Contains(vm.Groups, g => g.Title == "Genel Bilgiler");
            Assert.Contains(vm.Groups, g => g.Title == "Bina ve Yapı");
            Assert.Contains(vm.Groups, g => g.Title == "İç Mekan");
            Assert.Contains(vm.Groups, g => g.Title == "Satış ve Tapu");

            // Insights: m² birim fiyatı ve Deprem analizi kartı bulunmalı
            Assert.Equal(2, vm.Insights.Count);
            Assert.Contains(vm.Insights, i => i.Title == "m² Birim Fiyatı");
            Assert.Contains(vm.Insights, i => i.Title == "Deprem Ön Değerlendirmesi");
        }

        [Fact]
        public void FromSale_LandSale_MasksParcelDetailsInLockedGroup()
        {
            // Arsa ilanında ada/parsel/pafta numaraları güvenlik nedeniyle maskelenmeli ve ticari bilgi gizlenmeli
            var dto = new ResultAllForSaleListinForPageDto
            {
                ListingId = 202,
                ListingType = "Arsa",
                Area = 500,
                Price = 2500000m,
                ParcelNumber = "ADA-999",
                PlotNumber = "PARSEL-888",
                MapSheetNumber = "PAFTA-777",
                ZoningStatus = "İmarlı",
                FloorAreaRatio = 1.5,
                BaseAreaRatio = 0.3,
                TitleDeedStatus = "Müstakil Parsel"
            };

            var vm = ListingSpecsBuilder.FromSale(dto);

            Assert.Equal("land", vm.CategoryKey);

            var parcelGroup = vm.Groups.FirstOrDefault(g => g.Title == "Parsel Bilgileri");
            Assert.NotNull(parcelGroup);
            Assert.All(parcelGroup.Rows, row =>
            {
                Assert.Equal("locked", row.Tone);
                Assert.Equal("Danışmanımızdan talep edin", row.Value);
            });

            // Ham ada/parsel/pafta değerlerinin hiçbir grupta yer almadığını doğrula
            var allRowValues = vm.Groups.SelectMany(g => g.Rows).Select(r => r.Value).ToList();
            Assert.DoesNotContain("ADA-999", allRowValues);
            Assert.DoesNotContain("PARSEL-888", allRowValues);
            Assert.DoesNotContain("PAFTA-777", allRowValues);

            // Arsada deprem ön değerlendirmesi kartı bulunmamalıdır
            Assert.DoesNotContain(vm.Insights, i => i.Title == "Deprem Ön Değerlendirmesi");
        }

        [Fact]
        public void FromRental_HousingRental_CalculatesMovingCostAndRendersInsights()
        {
            // Kiralık konut ilanında taşınma maliyeti hesaplanmalı ve içgörü kartı oluşturulmalı
            var dto = new ResultAllForRentalListingForPageDto
            {
                ListingId = 303,
                ListingType = "Konut",
                Rent = 20000m,
                Deposit = 40000m,
                Dues = 1000m,
                GrossArea = 90,
                NetArea = 80,
                NumberOfRoom = "2+1",
                Furnished = true
            };

            var vm = ListingSpecsBuilder.FromRental(dto);

            Assert.Equal("rent", vm.UsageKey);
            Assert.Equal("housing", vm.CategoryKey);

            var kiraGroup = vm.Groups.FirstOrDefault(g => g.Title == "Kira Koşulları");
            Assert.NotNull(kiraGroup);

            // Taşınma maliyeti = Kira (20.000) + Depozito (40.000) + Aidat (1.000) = 61.000 ₺
            var movingCostRow = kiraGroup.Rows.FirstOrDefault(r => r.Label == "Taşınma Maliyeti");
            Assert.NotNull(movingCostRow);
            Assert.Contains("61.000", movingCostRow.Value);

            // Insights kontrolü
            var insight = vm.Insights.FirstOrDefault(i => i.Title == "Taşınma Maliyeti");
            Assert.NotNull(insight);
            Assert.Contains("61.000", insight.Text);
            Assert.Contains("(kira, depozito ve aidat)", insight.Text);
        }

        [Fact]
        public void FromSale_CommercialSale_BuildsCommercialSpecificGroups()
        {
            // Ticari satılık ilanı için ticari özel alanlar (bölüm sayısı, mutfak, wc) doğru modellenmeli
            var dto = new ResultAllForSaleListinForPageDto
            {
                ListingId = 404,
                ListingType = "Ticari",
                GrossArea = 250,
                Area = 200,
                NumberOfSection = 4,
                NumberOfKitchens = 1,
                NumberOfBathrooms = 2,
                Price = 10000000m,
                Transferable = true
            };

            var vm = ListingSpecsBuilder.FromSale(dto);

            Assert.Equal("commercial", vm.CategoryKey);

            var alanGroup = vm.Groups.FirstOrDefault(g => g.Title == "Alan ve Bölümler");
            Assert.NotNull(alanGroup);
            Assert.Contains(alanGroup.Rows, r => r.Label == "Bölüm Sayısı" && r.Value == "4");
            Assert.Contains(alanGroup.Rows, r => r.Label == "Mutfak" && r.Value == "Var" && r.Tone == "positive");
            Assert.Contains(alanGroup.Rows, r => r.Label == "WC" && r.Value == "2");

            var satisGroup = vm.Groups.FirstOrDefault(g => g.Title == "Satış ve Tapu");
            Assert.NotNull(satisGroup);
            Assert.Contains(satisGroup.Rows, r => r.Label == "Devren" && r.Value == "Devren" && r.Tone == "positive");
        }

        [Fact]
        public void FromSale_Amenities_GroupedAndSortedCorrectly()
        {
            // İlan donanımları belirlenen öncelik sırasına göre ("İç Özellikler" -> "Bina Özellikleri" -> "Manzara") gruplanmalı
            var dto = new ResultAllForSaleListinForPageDto
            {
                ListingId = 505,
                ListingType = "Konut",
                Amenities = new List<AmenityItemDto>
                {
                    new() { Group = "Manzara", Text = "Deniz Manzarası", SortOrder = 1 },
                    new() { Group = "İç Özellikler", Text = "Klima", SortOrder = 1 },
                    new() { Group = "Bina Özellikleri", Text = "Güvenlik", SortOrder = 1 },
                    new() { Group = "İç Özellikler", Text = "Ankastre Fırın", SortOrder = 2 }
                }
            };

            var vm = ListingSpecsBuilder.FromSale(dto);

            Assert.Equal(3, vm.AmenityGroups.Count);
            Assert.Equal("İç Özellikler", vm.AmenityGroups[0].Title);
            Assert.Equal("Bina Özellikleri", vm.AmenityGroups[1].Title);
            Assert.Equal("Manzara", vm.AmenityGroups[2].Title);

            Assert.Equal(2, vm.AmenityGroups[0].Items.Count);
            Assert.Equal("Klima", vm.AmenityGroups[0].Items[0]);
            Assert.Equal("Ankastre Fırın", vm.AmenityGroups[0].Items[1]);
        }
    }
}
