using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Answer
    {
        public int Id { get; set; }
        
        public string Content { get; set; }
        
        public int? ProfileId { get; set; }  // Reference Profile instead of IdentityUser
        
        [ForeignKey("ProfileId")]
        public Profile? Profile { get; set; }  // Reference the Profile model instead of IdentityUser
        
        public int? QuestionId { get; set; }
        
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
