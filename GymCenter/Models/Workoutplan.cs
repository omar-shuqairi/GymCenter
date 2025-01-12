using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Workoutplan
{
    public decimal Planid { get; set; }

    public string Planname { get; set; } = null!;

    public decimal Durationinmonths { get; set; }

    public string? ExerciseRoutines { get; set; }

    public string? Goals { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
