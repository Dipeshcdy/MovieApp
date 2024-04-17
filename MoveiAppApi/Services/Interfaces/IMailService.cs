using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;

namespace MoveiAppApi.Services.Interfaces
{
    public interface IMailService
    {
        public ResponseMessage SendMail(SendMailData sendMail);
    }
}
