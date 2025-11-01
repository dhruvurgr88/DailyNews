using System;
using System.Collections.Generic;

namespace DailyNewsDb.Modelss;

public partial class Like
{
    public int ArticleId { get; set; }

    public string UserEmail { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
   

    public virtual Article Article { get; set; } = null!;

    public virtual User UserEmailNavigation { get; set; } = null!;
}
