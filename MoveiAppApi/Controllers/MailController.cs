using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.Interfaces;

namespace MoveiAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [Authorize,HttpPost]
        public ActionResult SendMail([FromForm]SendMailData sendMail)
        {
            if(ModelState.IsValid)
            {
                var res=_mailService.SendMail(sendMail);
                if (res.Success)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
