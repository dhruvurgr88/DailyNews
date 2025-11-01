using DailyNewsDb.Modelss;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DailyNewsDb;

public partial class DailyNewsDbContext : DbContext
{
    public DailyNewsDbContext()
    {
    }

    public DailyNewsDbContext(DbContextOptions<DailyNewsDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Build configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Read connection string
            var connectionString = config.GetConnectionString("DailyNewsDB");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270E85584BCD7");

            entity.Property(e => e.AuthorEmail).HasMaxLength(200);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AuthorEmailNavigation).WithMany(p => p.Articles)
                .HasForeignKey(d => d.AuthorEmail)
                .HasConstraintName("FK_Articles_Users");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA8677F2FB");

            entity.Property(e => e.CommentDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CommentText).HasMaxLength(1000);
            entity.Property(e => e.UserEmail).HasMaxLength(200);

            entity.HasOne(d => d.Article).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK_Comments_Articles");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comments_Parent");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.UserEmail }).HasName("PK__Likes__1CE448378CB060A9");

            entity.Property(e => e.UserEmail).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Article).WithMany(p => p.Likes)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK_Likes_Articles");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Users");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.UserEmail }).HasName("PK__Ratings__1CE44837355E8867");

            entity.Property(e => e.UserEmail).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Rating1).HasColumnName("Rating");

            entity.HasOne(d => d.Article).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK_Ratings_Articles");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Users");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.UserEmail }).HasName("PK__Reports__1CE448374678FE2E");

            entity.Property(e => e.UserEmail).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(d => d.Article).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK_Reports_Articles");

            entity.HasOne(d => d.UserEmailNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reports_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07B441A77E");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F65DBA8A3F").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK__Users__7ED91ACF3FD10113");

            entity.Property(e => e.EmailId).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
