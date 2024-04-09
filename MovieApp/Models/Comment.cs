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
        
        public IdentityUser? User { get; set; }
        public Movie? Movie { get; set; }
        [Required]
        public String? Text{  get; set; }

    }
}
