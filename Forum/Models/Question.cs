using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Question
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        // Use ProfileId instead of IdentityUserId
        public int ProfileId { get; set; }
        
        [ForeignKey("ProfileId")]
        public Profile Profile { get; set; }  // Reference the Profile model
        
        public List<Answer>? Answers { get; set; }
    }
}
