using FibiEmlakDanismanlik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Context
{
    public class FibiEmlakDanismanlikContext:DbContext
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
        public DbSet<ForSaleHousingListing> forSaleHousingPropertyListings { get;set; }
        public DbSet<ForSaleLandListing> forSaleLandListings { get; set;}
        public DbSet<RentalCommercialPropertyListing> rentalCommercialPropertyListings { get; set; }
        public DbSet<RentalHousingListing> rentalHousingListings { get; set; }
        public DbSet<RentalLandListing> rentalLandListings { get; set; }
        public DbSet<HousingCategory> HousingCategories { get; set; }
        public DbSet<LandCategory> LandCategories { get; set; }
        public DbSet<MainBanner> MainBanners { get; set; }
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<FrequentlyAskedQuestion> frequentlyAskedQuestions { get; set; }
        public DbSet<Service> services { get; set; }

    }
}
