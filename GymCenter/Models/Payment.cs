using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Payment
{
    public decimal Paymentid { get; set; }

    public decimal? Amountpaid { get; set; }

    public DateTime? Paymentdate { get; set; }

    public string? Invoicepath { get; set; }

    public decimal? Userid { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual User? User { get; set; }
}
