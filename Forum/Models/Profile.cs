using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class Profile
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string UserId { get; set; } // This will store the foreign key to the AspNetUsers table

        // Navigation property for the related IdentityUser
        public virtual IdentityUser User { get; set; } // This allows you to access the related user
    }
}
