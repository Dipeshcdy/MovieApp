using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ValidateNever]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        [Required]
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie? Movie { get; set; }
        [Required]
        public String? Text{  get; set; }

    }
}
