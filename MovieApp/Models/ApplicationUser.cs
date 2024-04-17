using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class ApplicationUser:IdentityUser
    {

        [ValidateNever]
        public IEnumerable<Comment> Comments { get; set; }
    }
}
