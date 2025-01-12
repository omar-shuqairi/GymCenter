using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Member
{
    public decimal Memberid { get; set; }

    public DateTime? SubscriptionStart { get; set; }

    public DateTime? SubscriptionEnd { get; set; }

    public decimal? Planid { get; set; }

    public decimal? Userid { get; set; }

    public virtual Workoutplan? Plan { get; set; }

    public virtual User? User { get; set; }
}
