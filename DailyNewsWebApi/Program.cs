using DailyNewsServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DailyNewsDb.Modelss;

namespace DailyNewsWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Database connection
            builder.Services.AddDbContext<DailyNewsDb.DailyNewsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DailyNewsDb")));

            // ✅ CORS setup (ALLOW BOTH localhost + GitHub Pages + Render)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://localhost:4200",                     // local Angular dev
                            "https://dhruvurgr88.github.io",            // GitHub Pages site
                            "https://dailynewsapp-i821.onrender.com"    // Render API (self-origin)
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            // ✅ Services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ArticleService>();
            builder.Services.AddScoped<DailyNewsDb.DailyNewsDbContext>();

            // ✅ JWT Authentication setup
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            var app = builder.Build();

            // ✅ Use authentication and CORS before controllers
            app.UseAuthentication();
            app.UseCors("AllowFrontend");

            // ✅ Development-only Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
