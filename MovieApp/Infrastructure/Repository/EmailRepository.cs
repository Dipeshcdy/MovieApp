using Microsoft.AspNetCore.Hosting;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;
using MovieApp.Models.Email;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace MovieApp.Infrastructure.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailRepository(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public bool sendEmail(Movie movie, string email)
        {
            string htmlBody = "<h2 style='font-size:40px;font-weight:bold;'>Movie Details</h2>" +
                             "<p><strong>Movie Name:</strong> " + movie.Title + "</p>" +
                             "<p><strong>Description:</strong> " + movie.Description + "</p>" +
                             "<p><strong>Rating:</strong> " + (movie.Rated ?? 0) + "</p>" +
                             "<p><strong>Release Date:</strong> " + (movie.ReleaseDate) + "</p>" +
                             "<img style='border-radius:20px; width:90%; object-fit:cover; object-position:center;' src='cid:movieImage'>";
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            string imagePath;
            if(_webHostEnvironment.WebRootPath==null)
            {
                imagePath = Path.Combine("..", "MovieApp", "wwwroot", "uploads",movie.ImageUrl);
            }
            else
            {
                imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", movie.ImageUrl);
            }
            LinkedResource imageResource = new LinkedResource(imagePath, MediaTypeNames.Image.Jpeg);
            imageResource.ContentId = "movieImage";
            htmlView.LinkedResources.Add(imageResource);

           /* // Read the image file as bytes
            byte[] imageBytes = File.ReadAllBytes(imagePath);

            var ImageStream= new MemoryStream(imageBytes);

            // Create LinkedResource with the image bytes and specify its Content-ID
           
            imageResource.ContentId = "movieImage";
*/
            string subject = "Movie Details of " + movie.Title;
            var result=SendEmailAsync(email, subject, htmlBody,htmlView);
            return true;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message,AlternateView alternateView)
        {
            bool status = false;
            try
            {
                Email emailService = new Email()
                {
                    SecurityKey=_configuration.GetValue<string>("AppSettings:SecretKey"),
                    From=_configuration.GetValue<string>("AppSettings:EmailSettings:From"),
                    SmtpServer=_configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer"),
                    Port = _configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnableSSL= _configuration.GetValue<bool>("AppSettings:EmailSettings:EnableSSL")

                };
                MailMessage mailMessage = new MailMessage()
                {
                    From=new MailAddress(emailService.From),
                    Subject=subject,
                };
                mailMessage.To.Add(email);
                mailMessage.AlternateViews.Add(alternateView);


                SmtpClient smtpClient = new SmtpClient(emailService.SmtpServer)
                {
                    Port=emailService.Port,
                    Credentials=new NetworkCredential(emailService.From,emailService.SecurityKey),
                    EnableSsl = emailService.EnableSSL
                };
                await smtpClient.SendMailAsync(mailMessage);
                status=true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
    }
}
