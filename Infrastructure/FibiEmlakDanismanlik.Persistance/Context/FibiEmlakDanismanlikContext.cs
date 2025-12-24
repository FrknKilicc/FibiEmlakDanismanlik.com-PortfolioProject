using FibiEmlakDanismanlik.Domain.Entities;
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
            public FibiEmlakDanismanlikContext CreateDbContext(string[] args)
            {
                
                var solutionRoot = FindSolutionRoot();
                var webApiAppSettings = Path.Combine(solutionRoot, "Presentation", "FibiEmlakDanismanlik.WebApi", "appsettings.json");

                if (!File.Exists(webApiAppSettings))
                    throw new FileNotFoundException($"WebApi appsettings.json bulunamadı: {webApiAppSettings}");

                var json = File.ReadAllText(webApiAppSettings);
                using var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;

                if (!root.TryGetProperty("ConnectionStrings", out var cs))
                    throw new InvalidOperationException("WebApi appsettings.json içinde 'ConnectionStrings' bulunamadı.");

                if (!cs.TryGetProperty("DefaultConnection", out var dc))
                    throw new InvalidOperationException("WebApi appsettings.json içinde 'ConnectionStrings:DefaultConnection' bulunamadı.");

                var connectionString = dc.GetString();

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("'DefaultConnection' boş.");

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
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<FrequentlyAskedQuestion> frequentlyAskedQuestions { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<ListingType>  listingTypes { get; set; }
        public DbSet<CustomerContact> customerContacts { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }


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


            base.OnModelCreating(modelBuilder);
        }



    }
}
