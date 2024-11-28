using System.ComponentModel.DataAnnotations;

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

        // UserId is no longer a foreign key to AspNetUsers, it’s just a regular field
        public string UserId { get; set; } // Stores the unique identifier for the user

        // No navigation property to IdentityUser anymore, as you're not using the AspNetUsers table
    }
}
