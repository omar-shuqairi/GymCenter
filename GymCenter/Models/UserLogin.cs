using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class UserLogin
{
    public decimal Loginid { get; set; }

    public string? Username { get; set; }

    public string? Passwordd { get; set; }

    public decimal? Roleid { get; set; }

    public decimal? Userid { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
