using System.ComponentModel.DataAnnotations.Schema;

namespace GymCenter.Models
{
    public class JoinMemberUserTables
    {
        public decimal Userid { get; set; }

        public string? Fname { get; set; }

        public string? Lname { get; set; }

        public string? Email { get; set; }

        public string? ImagePath { get; set; }

        public decimal Memberid { get; set; }

        public DateTime? SubscriptionStart { get; set; }

        public DateTime? SubscriptionEnd { get; set; }

        public decimal? Planid { get; set; }

        //public virtual Workoutplan? Plan { get; set; }

        //public virtual User? User { get; set; }
        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public string? Username { get; set; }

        public string? Passwordd { get; set; }
        //public string ? Workoutplanname { get; set; }
    }
}
