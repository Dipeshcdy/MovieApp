using System.ComponentModel.DataAnnotations;

namespace MovieApp.DTOs
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public bool RememberMe {  get; set; }
        [Required]
        public string? Password { get; set; }

    }
}
