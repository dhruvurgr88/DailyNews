using DailyNewsDb;
using DailyNewsDb.Modelss;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DailyNewsDb;
namespace DailyNewsServices
{
    public class UserService
    {
        private readonly DailyNewsDb.DailyNewsDbContext _context;
        private readonly IConfiguration _config;

        public UserService(DailyNewsDb.DailyNewsDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task<string?> RegisterUser(User dto, string password)
        {
            if (await _context.Users.AnyAsync(u => u.EmailId == dto.EmailId))
                return null;

            dto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Users.Add(dto);
            await _context.SaveChangesAsync();

            return GenerateJwtToken(dto); // return JWT on signup
        }
        public async Task<string?> GetRole(string email)
        {
            int roleId = _context.Users.Where(u=>u.EmailId == email).Select(u=>u.RoleId).FirstOrDefault();
            string role = _context.Roles.Where(r=>r.Id == roleId).Select(r=>r.Name).FirstOrDefault();
            return role;
        }

        public async Task<string?> LoginUser(string emailId, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == emailId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.EmailId),
                new Claim(JwtRegisteredClaimNames.Email, user.EmailId),
                new Claim("name", user.Name),
                new Claim("role", user.RoleId.ToString()),
                new Claim("isPremium",user.IsPremium ? "true" : "false"),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<int> LikeArticleAsync(int articleId, string userEmail)
        {
            var existing = await _context.Likes.FindAsync(articleId, userEmail);
            if (existing != null)
            {
                _context.Likes.Remove(existing);
                _context.SaveChanges();
                return -1;
            }
            

            _context.Likes.Add(new Like
            {
                ArticleId = articleId,
                UserEmail = userEmail,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return 1;
        }

        //public async Task<bool> UnlikeArticleAsync(int articleId, string userEmail)
        //{
        //    var like = await _context.Likes.FindAsync(articleId, userEmail);
        //    if (like == null) return false;

        //    _context.Likes.Remove(like);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<Comment> AddCommentAsync(int articleId, string userEmail, string commentText)
        {
            var comment = new Comment
            {
                ArticleId = articleId,
                UserEmail = userEmail,
                CommentText = commentText,
                CommentDate = DateTime.UtcNow
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(int articleId)
        {
            return await _context.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.CommentDate)
                .ToListAsync();
        }

        public async Task<bool> RateArticleAsync(int articleId, string userEmail, int rating)
        {
            var existing = await _context.Ratings.FindAsync(articleId, userEmail);
            if (existing != null)
            {
                existing.Rating1 = rating;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    ArticleId = articleId,
                    UserEmail = userEmail,
                    Rating1 = rating,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(double average, int count)> GetArticleRatingAsync(int articleId)
        {
            var ratings = await _context.Ratings.Where(r => r.ArticleId == articleId).ToListAsync();
            return ratings.Any()
                ? (ratings.Average(r => r.Rating1), ratings.Count)
                : (0, 0);
        }

        
    }
}
