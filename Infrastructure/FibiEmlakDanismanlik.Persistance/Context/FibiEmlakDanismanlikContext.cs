using FibiEmlakDanismanlik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Context
{
    public class FibiEmlakDanismanlikContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=FURKAN\\FURKANKILIC;initial Catalog=fibiemlak.com;integrated Security=true;TrustServerCertificate=true;");
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            base.OnModelCreating(modelBuilder);
        }



    }
}
