using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DailyNewsDb.Modelss;

public partial class Article
{
    public int ArticleId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ImageUrl { get; set; }

    public string Status { get; set; } = null!;

    public string AuthorEmail { get; set; } = null!;

    public bool IsPremium { get; set; }
    [JsonIgnore]

    public virtual User AuthorEmailNavigation { get; set; } = null!;
    [JsonIgnore]

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    [JsonIgnore]

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    [JsonIgnore]

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
