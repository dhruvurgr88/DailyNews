using DailyNewsDb.Modelss;
using DailyNewsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyNewsWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleService _articleService;
        private readonly UserService _userService;

        public ArticleController(ArticleService articleService, UserService userService)
        {
            _articleService = articleService;
            _userService = userService;
        }

        // Get all articles
        [HttpGet]
        public IActionResult GetAllArticles()
        {
            var articles = _articleService.GetAllArticles();
            return articles.Any() ? Ok(articles) : NotFound("No articles found.");
        }

        [HttpGet]
        public IActionResult GetAllArticlesByJournalist(string email)
        {
            var articles = _articleService.GetAllArticlesByJournalist(email);
            return articles.Any() ? Ok(articles) : NotFound("No articles found.");
        }

        // Get article by ID
        [HttpGet]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _articleService.GetArticle(id);
            return article == null ? NotFound() : Ok(article);
        }

        // Get comments for an article
        [HttpGet]
        public async Task<IActionResult> GetComments(int articleId)
        {
            var comments = await _articleService.GetCommentsByArticleAsync(articleId);
            return Ok(comments);
        }

        // Add comment to article
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int articleId, [FromBody] string text)
        {
            var userEmail = User.Identity?.Name;
            var comment = await _userService.AddCommentAsync(articleId, userEmail!, text);
            return Ok(comment);
        }

        // Like or Unlike an article
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeArticle(int articleId)
        {
            var userEmail = User.Identity?.Name;
            var success = await _userService.LikeArticleAsync(articleId, userEmail!);
            return success == 1 ? Ok("Liked") : Ok("Unliked");
        }

        // Get likes count
        [HttpGet]
        public async Task<IActionResult> GetLikesCount(int articleId)
        {
            var count = await _articleService.GetLikesCountAsync(articleId);
            return Ok(count);
        }

        // Rate article
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateArticle(int articleId, [FromBody] int rating)
        {
            var userEmail = User.Identity?.Name;
            var success = await _userService.RateArticleAsync(articleId, userEmail!, rating);
            return success ? Ok("Rated") : BadRequest("Failed to rate");
        }

        [HttpGet]
        public async Task<IActionResult> GetRating(int articleId)
        {
            var (average, count) = await _userService.GetArticleRatingAsync(articleId);
            return Ok(new { average, count });
        }

        [HttpGet]
        
        public async Task<IActionResult> GetArticlesByStatus(string status)
        {
            var articles = await _articleService.GetArticlesByStatusAsync(status);
            return Ok(articles);
        }

        // ✅ Approve article
        [HttpPost]
        
        public async Task<IActionResult> ApproveArticle(int articleId)
        {
            var success = await _articleService.UpdateArticleStatusAsync(articleId, "Approved");
            if (!success) return NotFound("Article not found or already approved");

            return Ok("Article approved successfully");
        }
       
        [HttpPut("RejectArticle")]
        public async Task<IActionResult> RejectArticle(int articleId)
        {
            Boolean status = _articleService.RejectArticle(articleId);
            if (!status) return BadRequest(status);
            return Ok(new { message = "Article rejected successfully" });
        }

    }
}
