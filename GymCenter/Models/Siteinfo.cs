using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymCenter.Models;

public partial class Siteinfo
{
    public decimal Id { get; set; }

    public string? Sitename { get; set; }

    public string? SiteImagePath { get; set; }

    public string? LogoImagePath { get; set; }

    public string? SharedImagePath { get; set; }

    [NotMapped]
    public virtual IFormFile? SiteImageFile { get; set; }

    [NotMapped]
    public virtual IFormFile? LogoImageFile { get; set; }

    [NotMapped]
    public virtual IFormFile? SharedImageFile { get; set; }
}
