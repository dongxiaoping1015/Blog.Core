using System;
using System.Threading.Tasks;
using Blog.Core.IRepository;
using Blog.Core.Model.Models;
using Blog.Core.Repository.Base;

namespace Blog.Core.Repository
{
    public class BlogArticleRepository : BaseRepository<BlogArticle>, IBlogArticleRepository
    {
        public async Task<BlogArticle> GetBlogByID(int id)
        {
            return base.Db.Queryable<BlogArticle>().InSingle(id);
        }
    }
}
