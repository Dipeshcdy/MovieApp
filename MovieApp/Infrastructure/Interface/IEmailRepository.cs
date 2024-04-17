using MovieApp.Models;
using System.Net.Mail;

namespace MovieApp.Infrastructure.Interface
{
    public interface IEmailRepository
    {
        public bool sendEmail(Movie movie, string email);
        Task<bool> SendEmailAsync(string email, string subject, string message,AlternateView alternateView);
    }
}
