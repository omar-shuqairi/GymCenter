using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Contactform
{
    public decimal Id { get; set; }

    public string? Guestname { get; set; }

    public string? Guestemail { get; set; }

    public string? Guestcomment { get; set; }
}
