using System.ComponentModel.DataAnnotations.Schema;

namespace GymCenter.Models
{
    public class JoinAdminUserTables
    {
        public decimal Userid { get; set; }

        public string? Fname { get; set; }

        public string? Lname { get; set; }

        public string? Email { get; set; }
        public string? Username { get; set; }

        public string? Passwordd { get; set; }

        public string? ImagePath { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
    }
}
