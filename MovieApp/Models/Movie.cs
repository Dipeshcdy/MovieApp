using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        public float? Rated { get; set; }
        [Required]
        public DateOnly ReleaseDate { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<Comment> Comments { get; set; }
        [NotMapped]
        [ValidateNever]
        public Comment Comment { get; set; }
        [NotMapped]
        [ValidateNever]
        public IEnumerable<Rating> Ratings{ get; set; }
        [NotMapped]
        [ValidateNever]
        public Rating Rating{ get; set; }
    }
}
