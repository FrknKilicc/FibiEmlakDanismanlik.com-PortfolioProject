using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces.ContentImageInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ContentImageRepositories
{
    /// <summary>
    /// İçerik görsellerinin kullanıldığı veritabanı sütunlarını tarayan repository uygulaması.
    /// </summary>
    public class ContentImageUsageRepository : IContentImageUsageRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public ContentImageUsageRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetReferenceTextsAsync()
        {
            var results = new List<string>();

            // Blogs.BlogImgUrl
            var blogImgs = await _context.Blogs.AsNoTracking()
                .Where(b => b.BlogImgUrl != null && b.BlogImgUrl != "")
                .Select(b => b.BlogImgUrl!)
                .ToListAsync();
            results.AddRange(blogImgs);

            // Blogs.BlogDescription
            var blogDescs = await _context.Blogs.AsNoTracking()
                .Where(b => b.BlogDescription != null && b.BlogDescription != "")
                .Select(b => b.BlogDescription!)
                .ToListAsync();
            results.AddRange(blogDescs);

            // AboutUs.AboutUsImgUrl
            var aboutImgs = await _context.AboutUs.AsNoTracking()
                .Where(a => a.AboutUsImgUrl != null && a.AboutUsImgUrl != "")
                .Select(a => a.AboutUsImgUrl!)
                .ToListAsync();
            results.AddRange(aboutImgs);

            // AboutUs.AboutUsDesc
            var aboutDescs = await _context.AboutUs.AsNoTracking()
                .Where(a => a.AboutUsDesc != null && a.AboutUsDesc != "")
                .Select(a => a.AboutUsDesc!)
                .ToListAsync();
            results.AddRange(aboutDescs);

            // Authors.AuthorImgUrl
            var authorImgs = await _context.Authors.AsNoTracking()
                .Where(a => a.AuthorImgUrl != null && a.AuthorImgUrl != "")
                .Select(a => a.AuthorImgUrl!)
                .ToListAsync();
            results.AddRange(authorImgs);

            // MainBanners.MainBannerUrl
            var bannerUrls = await _context.MainBanners.AsNoTracking()
                .Where(m => m.MainBannerUrl != null && m.MainBannerUrl != "")
                .Select(m => m.MainBannerUrl!)
                .ToListAsync();
            results.AddRange(bannerUrls);

            // Agents.AgentImgUrl
            var agentImgs = await _context.Agents.AsNoTracking()
                .Where(a => a.AgentImgUrl != null && a.AgentImgUrl != "")
                .Select(a => a.AgentImgUrl!)
                .ToListAsync();
            results.AddRange(agentImgs);

            // Testimonials.LogoUrl
            var testimonialLogos = await _context.Testimonials.AsNoTracking()
                .Where(t => t.LogoUrl != null && t.LogoUrl != "")
                .Select(t => t.LogoUrl!)
                .ToListAsync();
            results.AddRange(testimonialLogos);

            // OurPartners.PartnerLogoUrl
            var partnerLogos = await _context.OurPartners.AsNoTracking()
                .Where(p => p.PartnerLogoUrl != null && p.PartnerLogoUrl != "")
                .Select(p => p.PartnerLogoUrl!)
                .ToListAsync();
            results.AddRange(partnerLogos);

            // HomeSectionImages.ImageUrl
            var homeSectionImages = await _context.HomeSectionImages.AsNoTracking()
                .Where(h => h.ImageUrl != null && h.ImageUrl != "")
                .Select(h => h.ImageUrl!)
                .ToListAsync();
            results.AddRange(homeSectionImages);

            // CityGalleryImages.ImageUrl
            var cityGalleryImages = await _context.CityGalleryImages.AsNoTracking()
                .Where(c => c.ImageUrl != null && c.ImageUrl != "")
                .Select(c => c.ImageUrl!)
                .ToListAsync();
            results.AddRange(cityGalleryImages);

            // BankLoanRates.LogoUrl
            var bankLogos = await _context.BankLoanRates.AsNoTracking()
                .Where(b => b.LogoUrl != null && b.LogoUrl != "")
                .Select(b => b.LogoUrl!)
                .ToListAsync();
            results.AddRange(bankLogos);

            // services.ServiceImgUrl
            var serviceImgs = await _context.services.AsNoTracking()
                .Where(s => s.ServiceImgUrl != null && s.ServiceImgUrl != "")
                .Select(s => s.ServiceImgUrl!)
                .ToListAsync();
            results.AddRange(serviceImgs);

            return results;
        }
    }
}
