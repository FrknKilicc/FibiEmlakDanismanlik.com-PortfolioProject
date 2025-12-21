using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces
{
    public interface IBlogRepository
    {
        public Task<GetBlogDetailWithAuthorResult?> GetBlogDetailWithAuthorById(int blogId);
        public Task<List<GetBlogDetailWithAuthorResult?>> GetLast3BlogWithAuthor();
        public Task<List<GetBlogDetailWithAuthorResult?>> GetBlogListWithAuthor();
        public Task<List<BlogSuggestionResult>> SearchBlogSuggestion(string q , int take = 6);

    }
}
