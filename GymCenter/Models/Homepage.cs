using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymCenter.Models;

public partial class Homepage
{
    public decimal Id { get; set; }

    public string? ImagePath { get; set; }

    public string? Title1 { get; set; }

    public string? Title2 { get; set; }

    public string? Titlebtn { get; set; }
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }
}
