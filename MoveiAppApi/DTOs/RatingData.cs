using System.ComponentModel.DataAnnotations;

namespace MoveiAppApi.DTOs
{
    public class RatingData
    {

        [Required]
        public int MovieId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "The rating value must be between 1 and 5.")]
        public int Value { get; set; }
    }
}
