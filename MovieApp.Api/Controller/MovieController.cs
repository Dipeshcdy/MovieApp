using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetMovies()
        {
            var abc = "def";
            return Ok(abc);
        }
    }
}
