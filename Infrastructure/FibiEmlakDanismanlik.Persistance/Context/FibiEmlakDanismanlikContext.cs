using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace FibiEmlakDanismanlik.Persistence.Context
{
    public class FibiEmlakDanismanlikContext : DbContext
    {
        public FibiEmlakDanismanlikContext(DbContextOptions<FibiEmlakDanismanlikContext> options)
        : base(options)
        {
        }
        public class FibiEmlakDanismanlikContextFactory
        : IDesignTimeDbContextFactory<FibiEmlakDanismanlikContext>
        {
            private const string WebApiUserSecretsId = "c14f0041-5e6d-4166-a771-03807aefed81";

            public FibiEmlakDanismanlikContext CreateDbContext(string[] args)
            {
                // 1. Ortam değişkeni ConnectionStrings__DefaultConnection
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

                // 2. WebApi'nin UserSecretsId'si ile User Secrets
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    var secretsConfig = new ConfigurationBuilder()
                        .AddUserSecrets(WebApiUserSecretsId)
                        .Build();
                    connectionString = secretsConfig.GetConnectionString("DefaultConnection");
                }

                // 3. appsettings.json
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    var solutionRoot = FindSolutionRoot();
                    var webApiAppSettings = Path.Combine(solutionRoot, "Presentation", "FibiEmlakDanismanlik.WebApi", "appsettings.json");
                    if (File.Exists(webApiAppSettings))
                    {
                        var jsonConfig = new ConfigurationBuilder()
                            .AddJsonFile(webApiAppSettings, optional: true)
                            .Build();
                        connectionString = jsonConfig.GetConnectionString("DefaultConnection");
                    }
                }

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Bağlantı dizesi bulunamadı: user-secrets veya ConnectionStrings__DefaultConnection ayarlayın.");

                var optionsBuilder = new DbContextOptionsBuilder<FibiEmlakDanismanlikContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new FibiEmlakDanismanlikContext(optionsBuilder.Options);
            }

            private static string FindSolutionRoot()
            {
                var dir = new DirectoryInfo(Directory.GetCurrentDirectory());


                while (dir != null && dir.GetFiles("*.sln").Length == 0)
                    dir = dir.Parent;

                if (dir == null)
                    throw new DirectoryNotFoundException("");

                return dir.FullName;
            }
        }

        public DbSet<AmenityDefinition> AmenityDefinitions { get; set; }
        public DbSet<ForSaleHousingListingAmenity> ForSaleHousingListingAmenities { get; set; }
        public DbSet<ForSaleLandListingAmenity> ForSaleLandListingAmenities { get; set; }
        public DbSet<ForSaleCommercialListingAmenity> ForSaleCommercialListingAmenities { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ForSaleCommercialPropertyListing> forSaleCommercialPropertyListings { get; set; }
        public DbSet<ForSaleHousingListing> forSaleHousingPropertyListings { get; set; }
        public DbSet<ForSaleLandListing> forSaleLandListings { get; set; }
        public DbSet<RentalCommercialPropertyListing> rentalCommercialPropertyListings { get; set; }
        public DbSet<RentalHousingListing> rentalHousingListings { get; set; }
        public DbSet<RentalLandListing> rentalLandListings { get; set; }
        public DbSet<HousingCategory> HousingCategories { get; set; }
        public DbSet<LandCategory> LandCategories { get; set; }
        public DbSet<MainBanner> MainBanners { get; set; }
        public DbSet<FrequentlyAskedQuestion> frequentlyAskedQuestions { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<ListingType> listingTypes { get; set; }
        public DbSet<CustomerContact> customerContacts { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<RentalHousingListingAmenities> RentalHousingListingAmenities { get; set; }
        public DbSet<RentalLandListingAmenities> RentalLandListingAmenities { get; set; }
        public DbSet<RentalCommercialListingAmenities> RentalCommercialListingAmenities { get; set; }
        public DbSet<ListingImageSelection> ListingImageSelections { get; set; }
        public DbSet<NearbyCategory> NearbyCategories { get; set; }
        public DbSet<ListingNearbyPlace> ListingNearbyPlaces { get; set; }
        public DbSet<ListingGeoPoint> ListingGeoPoints { get; set; }
        public DbSet<Testimonials> Testimonials { get; set; }
        public DbSet<OurPartners> OurPartners { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<CityGalleryImage> CityGalleryImages { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<HomeSectionImage> HomeSectionImages { get; set; }
        public DbSet<ExternalMarketStat> ExternalMarketStats { get; set; }
        public DbSet<NeighborhoodSeismicScenarioStat> NeighborhoodSeismicScenarioStats { get; set; }
        public DbSet<MarketDataSyncLog> MarketDataSyncLogs { get; set; }
        public DbSet<BankLoanRate> BankLoanRates { get; set; }
        public DbSet<FibiServiceLead> FibiServiceLeads { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //İlişkisel lokasyon / şehir silinince ona bağlı il ilçe de silinmemeli restirict bu işi yapacak
            modelBuilder.Entity<City>().HasMany(c => c.Districts).WithOne(d => d.City).HasForeignKey(d => d.CityId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<District>()
    .HasMany(d => d.Neighborhoods)
    .WithOne(n => n.District)
    .HasForeignKey(n => n.DistrictId)
    .OnDelete(DeleteBehavior.Restrict);

            //performans için db index
            modelBuilder.Entity<City>()
    .HasIndex(c => c.Name);

            modelBuilder.Entity<District>()
                .HasIndex(d => new { d.CityId, d.Name });

            modelBuilder.Entity<Neighborhood>()
                .HasIndex(n => new { n.DistrictId, n.Name });


            modelBuilder.HasSequence<int>("Seq_ForSaleCommercialPropertyListings_Table")
                .StartsAt(500000)
                .IncrementsBy(1);

            modelBuilder.HasSequence<int>("Seq_ForSaleHousingPropertyListings_Table")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.HasSequence<int>("Seq_ForSaleLandListings_Table")
                .StartsAt(200000)
                .IncrementsBy(1);

            modelBuilder.HasSequence<int>("Seq_RentalCommercialPropertyListings_Table")
                .StartsAt(1000000)
                .IncrementsBy(1);

            modelBuilder.HasSequence<int>("Seq_RentalHousingListings_Table")
                .StartsAt(100000)
                .IncrementsBy(1);

            modelBuilder.HasSequence<int>("Seq_RentalLandListings_Table")
                .StartsAt(300000)
                .IncrementsBy(1);

            modelBuilder.Entity<ForSaleCommercialPropertyListing>()
                .Property(f => f.PropertyNo)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table");

            modelBuilder.Entity<ForSaleHousingListing>()
                .Property(f => f.PropertyNo)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table");

            modelBuilder.Entity<ForSaleLandListing>()
                .Property(f => f.PropertyNo)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_ForSaleLandListings_Table");

            modelBuilder.Entity<RentalCommercialPropertyListing>()
                .Property(f => f.PropertyNo)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table");

            modelBuilder.Entity<RentalHousingListing>()
                .Property(f => f.PropertyNo)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_RentalHousingListings_Table");

            modelBuilder.Entity<RentalLandListing>()
                .Property(f => f.PropertyNo)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_RentalLandListings_Table");
            modelBuilder.Entity<ListingImageSelection>()
    .HasIndex(x => new { x.ListingId, x.SectionKey, x.ImageNo })
    .IsUnique();

            // Tag
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(80);
            });

            // BlogTag 
            modelBuilder.Entity<BlogTag>(entity =>
            {
                entity.HasKey(x => new { x.BlogId, x.TagId });

                entity.HasOne(x => x.Blog)
                      .WithMany(x => x.BlogTags)
                      .HasForeignKey(x => x.BlogId);

                entity.HasOne(x => x.Tag)
                      .WithMany(x => x.BlogTags)
                      .HasForeignKey(x => x.TagId);
            });

            modelBuilder.Entity<SocialMedia>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(x => x.Url)
                      .IsRequired();

                entity.Property(x => x.Icon)
                      .HasMaxLength(50);

                entity.Property(x => x.Order)
                      .IsRequired();

                entity.Property(x => x.IsActive)
                      .HasDefaultValue(true);

                entity.Property(x => x.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");
            });

            //citygalleryimage

            modelBuilder.Entity<CityGalleryImage>(builder =>
            {
                builder.HasKey(x => x.CityGalleryImageId);

                builder.Property(x => x.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                builder.Property(x => x.Title)
                    .HasMaxLength(150)
                    .IsRequired(false);

                builder.Property(x => x.Description)
                    .HasMaxLength(300)
                    .IsRequired(false);

                builder.Property(x => x.DisplayOrder)
                    .HasDefaultValue(0);

                builder.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                builder.HasOne(x => x.City)
                    .WithMany(x => x.CityGalleryImages)
                    .HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //BlogCat
            modelBuilder.Entity<BlogCategory>(entity =>
            {
                entity.HasKey(x => x.BlogCategoryId);

                entity.Property(x => x.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(x => x.CategoryName)
                      .IsUnique();
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasOne(x => x.BlogCategory)
                      .WithMany(x => x.Blogs)
                      .HasForeignKey(x => x.BlogCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ListingGeoPoint>()
    .HasIndex(x => new { x.ListingId, x.ListingTypeId })
    .IsUnique();

            // AmenityDefinition
            modelBuilder.Entity<AmenityDefinition>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
                entity.Property(x => x.GroupName).HasMaxLength(80);

                entity.HasIndex(x => x.Name);
            });

            // Join forsalehousing
            modelBuilder.Entity<ForSaleHousingListingAmenity>(entity =>
            {
                entity.HasKey(x => new { x.ForSaleHousingListId, x.AmenityId });

                entity.HasOne(x => x.forSaleHousingListing)
                      .WithMany()
                      .HasForeignKey(x => x.ForSaleHousingListId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.AmenityDefinition)
                      .WithMany()
                      .HasForeignKey(x => x.AmenityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.AmenityId, x.ForSaleHousingListId });
            });

            // Join forsaleLand
            modelBuilder.Entity<ForSaleLandListingAmenity>(entity =>
            {
                entity.HasKey(x => new { x.ForSaleLandListId, x.AmenityId });

                entity.HasOne(x => x.ForSaleLandListing)
                      .WithMany()
                      .HasForeignKey(x => x.ForSaleLandListId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.AmenityDefinition)
                      .WithMany()
                      .HasForeignKey(x => x.AmenityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.AmenityId, x.ForSaleLandListId });
            });

            // Join forsaleCommercial
            modelBuilder.Entity<ForSaleCommercialListingAmenity>(entity =>
            {
                entity.HasKey(x => new { x.ForSaleCommercialListingId, x.AmenityId });

                entity.HasOne(x => x.ForSaleCommercialProperty)
                      .WithMany()
                      .HasForeignKey(x => x.ForSaleCommercialListingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.AmenityDefinition)
                      .WithMany()
                      .HasForeignKey(x => x.AmenityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.AmenityId, x.ForSaleCommercialListingId });
            });
            // NearbyCategory
            modelBuilder.Entity<NearbyCategory>(entity =>
            {
                entity.Property(x => x.Key).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
                entity.Property(x => x.IconCss).HasMaxLength(80);

                entity.HasIndex(x => x.Key).IsUnique();
                entity.HasIndex(x => new { x.IsActive, x.SortOrder });
            });

            // ListingNearbyPlace
            modelBuilder.Entity<ListingNearbyPlace>(entity =>
            {
                entity.Property(x => x.PlaceName).IsRequired().HasMaxLength(120);

                entity.Property(x => x.DistanceKm).HasColumnType("decimal(6,2)");

                entity.HasOne(x => x.NearbyCategory)
                      .WithMany(c => c.Places)
                      .HasForeignKey(x => x.NearbyCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.ListingId, x.ListingTypeId, x.IsActive });
                entity.HasIndex(x => new { x.NearbyCategoryId, x.IsActive });

                entity.HasIndex(x => new { x.ListingId, x.ListingTypeId, x.NearbyCategoryId, x.PlaceName }).IsUnique();
            });

            // Agent: e-posta benzersiz (login e-postayla yapılır)
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(x => x.Mail).IsRequired().HasMaxLength(256);
                entity.HasIndex(x => x.Mail).IsUnique();
            });

            // HomeSectionImage: Ana sayfa bölümlerine ait görseller (bölüm anahtarı + slot)
            modelBuilder.Entity<HomeSectionImage>(entity =>
            {
                entity.Property(x => x.SectionKey).IsRequired().HasMaxLength(HomeSectionImageConstants.SectionKeyMaxLength);
                entity.Property(x => x.ImageUrl).IsRequired().HasMaxLength(HomeSectionImageConstants.ImageUrlMaxLength);
                entity.Property(x => x.AltText).HasMaxLength(HomeSectionImageConstants.AltTextMaxLength);

                entity.HasIndex(x => new { x.SectionKey, x.Slot }).IsUnique();

                entity.HasData(
                    new HomeSectionImage
                    {
                        HomeSectionImageId = 1,
                        SectionKey = HomeSectionImageConstants.SectionPopularPlaces,
                        Slot = 1,
                        ImageUrl = "/Realshed/assets/images/resource/popular-place-1.jpg",
                        AltText = "Fibi Emlak Danışmanlık merkez ofis",
                        IsActive = true
                    },
                    new HomeSectionImage
                    {
                        HomeSectionImageId = 2,
                        SectionKey = HomeSectionImageConstants.SectionPopularPlaces,
                        Slot = 2,
                        ImageUrl = "/Realshed/assets/images/resource/popular-place-2.jpg",
                        AltText = "Fibi Emlak Danışmanlık ofis içi",
                        IsActive = true
                    },
                    new HomeSectionImage
                    {
                        HomeSectionImageId = 3,
                        SectionKey = HomeSectionImageConstants.SectionTestimonials,
                        Slot = 1,
                        ImageUrl = "/Realshed/assets/images/background/testimonial-home.jpg",
                        AltText = "Yeni evinin önünde mutlu aile",
                        IsActive = true
                    }
                );
            });

            // ExternalMarketStat: Kademeli kapsamlı dış piyasa emsal verileri
            modelBuilder.Entity<ExternalMarketStat>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_ExternalMarketStats_ScopeLocation",
                    "([Scope] = 1 AND [CityId] IS NULL AND [DistrictId] IS NULL AND [NeighborhoodId] IS NULL) OR " +
                    "([Scope] = 2 AND [CityId] IS NOT NULL AND [DistrictId] IS NULL AND [NeighborhoodId] IS NULL) OR " +
                    "([Scope] = 3 AND [CityId] IS NOT NULL AND [DistrictId] IS NOT NULL AND [NeighborhoodId] IS NULL) OR " +
                    "([Scope] = 4 AND [CityId] IS NOT NULL AND [DistrictId] IS NOT NULL AND [NeighborhoodId] IS NOT NULL)"));

                entity.HasOne(x => x.City)
                      .WithMany()
                      .HasForeignKey(x => x.CityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.District)
                      .WithMany()
                      .HasForeignKey(x => x.DistrictId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Neighborhood)
                      .WithMany()
                      .HasForeignKey(x => x.NeighborhoodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.Scope).IsUnique().HasFilter("[Scope] = 1");
                entity.HasIndex(x => x.CityId).IsUnique().HasFilter("[Scope] = 2");
                entity.HasIndex(x => x.DistrictId).IsUnique().HasFilter("[Scope] = 3");
                entity.HasIndex(x => x.NeighborhoodId).IsUnique().HasFilter("[Scope] = 4");

                entity.Property(x => x.BaseSalePricePerSqm).HasPrecision(18, 2);
                entity.Property(x => x.BaseRentPricePerSqm).HasPrecision(18, 2);

                entity.Property(x => x.AgencyMarginMultiplier)
                      .HasPrecision(5, 3)
                      .HasDefaultValue(FibiServiceConstants.DefaultAgencyMarginMultiplier);

                entity.Property(x => x.BandSpreadPercentage)
                      .HasPrecision(5, 2)
                      .HasDefaultValue(FibiServiceConstants.DefaultBandSpreadPercentage);

                entity.Property(x => x.Last1YearGrowthRate).HasPrecision(7, 2);
                entity.Property(x => x.Last3YearsCompoundGrowthRate).HasPrecision(7, 2);
                entity.Property(x => x.RegionalDevelopmentFactor).HasPrecision(4, 2);

                entity.Property(x => x.HistoricalQuarterlyPricesJson).IsRequired();

                entity.Property(x => x.DataSource)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.DataSourceMaxLength);

                entity.Property(x => x.LastAppliedIndexPeriod)
                      .HasMaxLength(FibiServiceConstants.IndexPeriodMaxLength);
            });

            // NeighborhoodSeismicScenarioStat: İBB deprem senaryosu mahalle verisi
            modelBuilder.Entity<NeighborhoodSeismicScenarioStat>(entity =>
            {
                entity.HasOne(x => x.Neighborhood)
                      .WithMany()
                      .HasForeignKey(x => x.NeighborhoodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.NeighborhoodId).IsUnique();

                entity.Property(x => x.DataSource)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.DataSourceMaxLength);
            });

            // MarketDataSyncLog: Dış kaynak senkronizasyon kayıtları
            modelBuilder.Entity<MarketDataSyncLog>(entity =>
            {
                entity.Property(x => x.ProviderKey)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.ProviderKeyMaxLength);

                entity.Property(x => x.Message)
                      .HasMaxLength(FibiServiceConstants.SyncMessageMaxLength);

                entity.HasIndex(x => new { x.ProviderKey, x.StartedAt });
            });

            // BankLoanRate: Banka kredi faiz oranları ve seed data
            modelBuilder.Entity<BankLoanRate>(entity =>
            {
                entity.Property(x => x.BankName)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.BankNameMaxLength);

                entity.Property(x => x.LogoUrl)
                      .HasMaxLength(FibiServiceConstants.LogoUrlMaxLength);

                entity.Property(x => x.ProviderKey)
                      .HasMaxLength(FibiServiceConstants.ProviderKeyMaxLength);

                entity.HasIndex(x => x.ProviderKey)
                      .IsUnique()
                      .HasFilter("[ProviderKey] IS NOT NULL");

                entity.Property(x => x.SourceUrl)
                      .HasMaxLength(FibiServiceConstants.SourceUrlMaxLength);

                entity.Property(x => x.DataSource)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.DataSourceMaxLength);

                entity.Property(x => x.HousingLoanMonthlyRate).HasPrecision(9, 4);
                entity.Property(x => x.CommercialLoanMonthlyRate).HasPrecision(9, 4);
                entity.Property(x => x.AllocationFeeRate).HasPrecision(9, 4);

                entity.HasData(
                    new BankLoanRate { BankLoanRateId = 1, BankName = "Ziraat Bankası", LogoUrl = null, ProviderKey = "ziraat", SourceUrl = null, DataSource = FibiServiceConstants.EstimatedSeedDataSource, HousingLoanMonthlyRate = 2.49m, CommercialLoanMonthlyRate = 2.89m, MaxTermMonths = 120, AllocationFeeRate = 0.50m, IsActive = true, UpdatedDate = new DateTime(2026, 10, 1) },
                    new BankLoanRate { BankLoanRateId = 2, BankName = "Halkbank", LogoUrl = null, ProviderKey = "halkbank", SourceUrl = null, DataSource = FibiServiceConstants.EstimatedSeedDataSource, HousingLoanMonthlyRate = 2.52m, CommercialLoanMonthlyRate = 2.95m, MaxTermMonths = 120, AllocationFeeRate = 0.50m, IsActive = true, UpdatedDate = new DateTime(2026, 10, 1) },
                    new BankLoanRate { BankLoanRateId = 3, BankName = "VakıfBank", LogoUrl = null, ProviderKey = "vakifbank", SourceUrl = null, DataSource = FibiServiceConstants.EstimatedSeedDataSource, HousingLoanMonthlyRate = 2.55m, CommercialLoanMonthlyRate = 2.99m, MaxTermMonths = 120, AllocationFeeRate = 0.50m, IsActive = true, UpdatedDate = new DateTime(2026, 10, 1) },
                    new BankLoanRate { BankLoanRateId = 4, BankName = "Türkiye İş Bankası", LogoUrl = null, ProviderKey = "isbank", SourceUrl = null, DataSource = FibiServiceConstants.EstimatedSeedDataSource, HousingLoanMonthlyRate = 2.79m, CommercialLoanMonthlyRate = 3.19m, MaxTermMonths = 120, AllocationFeeRate = 0.50m, IsActive = true, UpdatedDate = new DateTime(2026, 10, 1) },
                    new BankLoanRate { BankLoanRateId = 5, BankName = "Garanti BBVA", LogoUrl = null, ProviderKey = "garanti", SourceUrl = null, DataSource = FibiServiceConstants.EstimatedSeedDataSource, HousingLoanMonthlyRate = 2.85m, CommercialLoanMonthlyRate = 3.25m, MaxTermMonths = 120, AllocationFeeRate = 0.50m, IsActive = true, UpdatedDate = new DateTime(2026, 10, 1) }
                );
            });

            // FibiServiceLead: Danışmanlık talepleri
            modelBuilder.Entity<FibiServiceLead>(entity =>
            {
                entity.Property(x => x.FullName)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.FullNameMaxLength);

                entity.Property(x => x.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(FibiServiceConstants.PhoneNumberMaxLength);

                entity.Property(x => x.Email)
                      .HasMaxLength(FibiServiceConstants.EmailMaxLength);

                entity.Property(x => x.CustomerNote)
                      .HasMaxLength(FibiServiceConstants.CustomerNoteMaxLength);

                entity.HasOne(x => x.Neighborhood)
                      .WithMany()
                      .HasForeignKey(x => x.NeighborhoodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.IsContacted, x.CreatedDate });
            });

            // İlanların il/ilçe ilişkileri ve indeksleri
            ConfigureListingLocation<ForSaleHousingListing>(modelBuilder);
            ConfigureListingLocation<ForSaleLandListing>(modelBuilder);
            ConfigureListingLocation<ForSaleCommercialPropertyListing>(modelBuilder);
            ConfigureListingLocation<RentalHousingListing>(modelBuilder);
            ConfigureListingLocation<RentalLandListing>(modelBuilder);
            ConfigureListingLocation<RentalCommercialPropertyListing>(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureListingLocation<TListing>(ModelBuilder modelBuilder) where TListing : class, ILocatedListing
        {
            modelBuilder.Entity<TListing>(entity =>
            {
                entity.HasOne<City>().WithMany().HasForeignKey(nameof(ILocatedListing.CityId)).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<District>().WithMany().HasForeignKey(nameof(ILocatedListing.DistrictId)).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(nameof(ILocatedListing.CityId));
                entity.HasIndex(nameof(ILocatedListing.DistrictId));
            });
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ResolveListingLocations();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            await ResolveListingLocationsAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ResolveListingLocations()
        {
            var entries = ChangeTracker.Entries<ILocatedListing>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            if (entries.Count == 0)
                return;

            var cityList = Cities.AsNoTracking()
                .Select(c => new { c.CityId, c.Name })
                .ToList();

            var cityMap = BuildNormalizedMap(cityList.Select(c => (c.CityId, (string?)c.Name)));
            var neededCityIds = GetResolvedCityIds(entries, cityMap);

            var districtCache = new Dictionary<int, Dictionary<string, int>>();
            foreach (var cityId in neededCityIds)
            {
                var districts = Districts.AsNoTracking()
                    .Where(d => d.CityId == cityId)
                    .Select(d => new { d.DistrictId, d.Name })
                    .ToList();

                districtCache[cityId] = BuildNormalizedMap(districts.Select(d => (d.DistrictId, (string?)d.Name)));
            }

            ApplyLocations(entries, cityMap, districtCache);
        }

        private async Task ResolveListingLocationsAsync(CancellationToken cancellationToken)
        {
            var entries = ChangeTracker.Entries<ILocatedListing>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            if (entries.Count == 0)
                return;

            var cityList = await Cities.AsNoTracking()
                .Select(c => new { c.CityId, c.Name })
                .ToListAsync(cancellationToken);

            var cityMap = BuildNormalizedMap(cityList.Select(c => (c.CityId, (string?)c.Name)));
            var neededCityIds = GetResolvedCityIds(entries, cityMap);

            var districtCache = new Dictionary<int, Dictionary<string, int>>();
            foreach (var cityId in neededCityIds)
            {
                var districts = await Districts.AsNoTracking()
                    .Where(d => d.CityId == cityId)
                    .Select(d => new { d.DistrictId, d.Name })
                    .ToListAsync(cancellationToken);

                districtCache[cityId] = BuildNormalizedMap(districts.Select(d => (d.DistrictId, (string?)d.Name)));
            }

            ApplyLocations(entries, cityMap, districtCache);
        }

        private static Dictionary<string, int> BuildNormalizedMap(IEnumerable<(int Id, string? Name)> items)
        {
            var map = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var item in items)
            {
                var key = LocationNameNormalizer.Normalize(item.Name);
                if (!string.IsNullOrEmpty(key))
                {
                    map.TryAdd(key, item.Id);
                }
            }
            return map;
        }

        private static HashSet<int> GetResolvedCityIds(List<ILocatedListing> entries, Dictionary<string, int> cityMap)
        {
            var neededCityIds = new HashSet<int>();
            foreach (var entry in entries)
            {
                var normCity = LocationNameNormalizer.Normalize(entry.City);
                if (!string.IsNullOrEmpty(normCity) && cityMap.TryGetValue(normCity, out var cityId))
                {
                    neededCityIds.Add(cityId);
                }
            }
            return neededCityIds;
        }

        private static void ApplyLocations(
            List<ILocatedListing> entries,
            Dictionary<string, int> cityMap,
            Dictionary<int, Dictionary<string, int>> districtCache)
        {
            foreach (var entity in entries)
            {
                int? cityId = null;
                var normCity = LocationNameNormalizer.Normalize(entity.City);
                if (!string.IsNullOrEmpty(normCity) && cityMap.TryGetValue(normCity, out var foundCityId))
                {
                    cityId = foundCityId;
                }
                entity.CityId = cityId;

                if (cityId.HasValue && districtCache.TryGetValue(cityId.Value, out var districtMap))
                {
                    var normDistrict = LocationNameNormalizer.Normalize(entity.District);
                    if (!string.IsNullOrEmpty(normDistrict) && districtMap.TryGetValue(normDistrict, out var foundDistrictId))
                    {
                        entity.DistrictId = foundDistrictId;
                    }
                    else
                    {
                        entity.DistrictId = null;
                    }
                }
                else
                {
                    entity.DistrictId = null;
                }
            }
        }
    }
}
