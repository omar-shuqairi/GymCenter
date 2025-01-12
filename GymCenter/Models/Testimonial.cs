using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Testimonial
{
    public decimal Testimonialid { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public decimal? Userid { get; set; }

    public virtual User? User { get; set; }
}
