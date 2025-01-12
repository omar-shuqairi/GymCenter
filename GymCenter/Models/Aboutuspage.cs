using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymCenter.Models;

public partial class Aboutuspage
{
    public decimal Id { get; set; }

    public string? Title { get; set; }

    public string? Paragraph1 { get; set; }

    public string? Paragraph2 { get; set; }

    public string? Videourl { get; set; }

    public string? Backgroundvideoimg { get; set; }
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }
}
