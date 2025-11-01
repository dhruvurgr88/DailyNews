using System;
using System.Collections.Generic;

namespace DailyNewsDb.Modelss;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public string UserEmailId { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string PlanType { get; set; } = null!;

    public virtual User UserEmail { get; set; } = null!;
}
