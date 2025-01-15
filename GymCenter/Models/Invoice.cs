using System;
using System.Collections.Generic;

namespace GymCenter.Models;

public partial class Invoice
{
    public decimal Invoiceid { get; set; }

    public byte[]? Pdfdata { get; set; }

    public DateTime? Createddate { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Paymentid { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
