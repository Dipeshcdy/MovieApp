using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public IdentityUser? User { get; set; }
        public Movie? Movie { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "The rating value must be between 1 and 5.")]
        public int Value { get; set; }
    }
}
