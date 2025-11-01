using System;
using System.Collections.Generic;

namespace DailyNewsDb.Modelss;

public partial class Comment
{
    public int CommentId { get; set; }

    public int ArticleId { get; set; }

    public string UserEmail { get; set; } = null!;

    public string CommentText { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public int? ParentCommentId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual User UserEmailNavigation { get; set; } = null!;
}
