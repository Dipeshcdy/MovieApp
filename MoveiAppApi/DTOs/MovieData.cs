using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MovieAppApi.DTOs
{
    public class MovieData
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateOnly ReleaseDate {  get; set; }
        public IFormFile? file { get; set; }
    }
}
