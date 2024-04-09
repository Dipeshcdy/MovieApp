using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.DTOs
{
    public class RegisterUser
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }


    }
}
