using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Profile
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }

    
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}

}
