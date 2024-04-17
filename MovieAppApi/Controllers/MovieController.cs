using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Infrastructure.Repository;

namespace MovieAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieRepository _movieRepository;

        public MovieController(MovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet("get")]
        public ActionResult Get()
        {
            var movies= _movieRepository.GetAll();
            return Ok(movies);
        }
    }
}
