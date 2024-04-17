using System.ComponentModel.DataAnnotations;

namespace MoveiAppApi.DTOs
{
    public class SendMailData
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public int MovieId {  get; set; }
    }
}
