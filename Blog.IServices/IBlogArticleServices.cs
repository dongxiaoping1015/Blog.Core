using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Core.IServices.Base;
using Blog.Core.Model.Models;

namespace Blog.Core.IServices
{
    public interface IBlogArticleServices : IBaseServices<BlogArticle>
    {
        Task<List<BlogArticle>> getBlogs();
        Task<BlogArticle> getBlogByID(int id);
    }
}
