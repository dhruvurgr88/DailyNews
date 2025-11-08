using DailyNewsDb;
using DailyNewsDb.Modelss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DailyNewsServices
{
    public class ArticleService
    {
        private DailyNewsDb.DailyNewsDbContext _context;
        public ArticleService(DailyNewsDb.DailyNewsDbContext context)
        {
            _context = context;
        }
        public List<Article> GetAllArticles()
        {
            List<Article> list = new List<Article>();
            try
            {
                list = _context.Articles.ToList();
                return list;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<Article> GetAllArticlesByJournalist(string email)
        {
            List<Article> list = new List<Article>();
            try
            {
                list = _context.Articles.Where(a=>a.AuthorEmail==email).ToList();
                return list;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles
                                        
                                        .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null) return null;
            return article;
        }
        public async Task<List<Comment>> GetCommentsByArticleAsync(int articleId)
        {
            return await _context.Comments
                                 .Where(c => c.ArticleId == articleId)
                                 .OrderByDescending(c => c.CommentDate)
                                 .ToListAsync();
        }

        public async Task<int> GetLikesCountAsync(int articleId)
        {
            return await _context.Likes
                .Where(l => l.ArticleId == articleId)
                .CountAsync();
        }
        public async Task<List<Article>> GetArticlesByStatusAsync(string status)
        {
            return await _context.Articles
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateArticleStatusAsync(int articleId, string newStatus)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null) return false;

            article.Status = newStatus;
            article.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        public Boolean RejectArticle(int articleId)
        {

            Article article = _context.Articles.Find(articleId);
            if (article == null) return false;

            article.Status = "Rejected";
            _context.SaveChangesAsync();
            return true;
        }

        public async Task<Article> AddArticleAsync(Article article)
        {
            article.CreatedDate = DateTime.UtcNow;
            article.Status = "Pending"; // default status

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> RejectArticleAsync(int articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
                return false;

            article.Status = "Rejected";
            article.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
