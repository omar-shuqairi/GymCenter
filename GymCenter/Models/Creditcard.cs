using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Creditcard
{
    public decimal Cardid { get; set; }

    public string Cardnumber { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public decimal? Balance { get; set; }

    public DateTime? Expirydate { get; set; }
}
