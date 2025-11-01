using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DailyNewsDb.Modelss;

public partial class User
{
    public string EmailId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Gender { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsPremium { get; set; }
    [JsonIgnore]

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    [JsonIgnore]

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    [JsonIgnore]

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    [JsonIgnore]

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    [JsonIgnore]

    public virtual Role Role { get; set; } = null!;
    [JsonIgnore]

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
