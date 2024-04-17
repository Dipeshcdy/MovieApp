using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;

namespace MoveiAppApi.Services.classes
{
    public class MailService : IMailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IMovieRepository _movieRepository;

        public MailService(IEmailRepository emailRepository, IMovieRepository movieRepository)
        {
            _emailRepository = emailRepository;
            _movieRepository = movieRepository;
        }

        public ResponseMessage SendMail(SendMailData sendMail)
        {
            ResponseMessage response = new ResponseMessage();
            var movie=_movieRepository.GetAll().Where(x=>x.Id==sendMail.MovieId).FirstOrDefault();
            if(movie==null)
            {
                response.Success = false;
                response.Message = "Movie Doesnot exits";
            }
            else
            {
                var res=_emailRepository.sendEmail(movie, sendMail.Email);
                if(res)
                {
                    response.Success = true;
                    response.Message = "Mail Sent Successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Something went Wrong!!";
                }
            }
            return response;
        }
    }
}
