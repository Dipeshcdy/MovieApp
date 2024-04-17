using System.ComponentModel.DataAnnotations;

namespace MoveiAppApi.DTOs
{
    public class CommentData
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        public string Text{ get; set; }
    }
}
