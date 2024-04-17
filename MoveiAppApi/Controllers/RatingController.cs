using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.classes;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Models;
using System.Security.Claims;

namespace MoveiAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly UserManager<ApplicationUser> _userManager;
      
        public RatingController(IRatingService ratingService, UserManager<ApplicationUser> userManager)
        {
            _ratingService = ratingService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateRating([FromForm] RatingData rating)
        {
            if(ModelState.IsValid)
            {
                var UserId = _userManager.GetUserId(User);
                var res= _ratingService.Create(rating,UserId);
                return Ok(res);
            }
            return BadRequest(ModelState);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteRating(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Id is Required");
                return BadRequest(ModelState);
            }
            var res = _ratingService.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [Authorize]
        [HttpGet("movie/{movieId}")]
        public ActionResult GetRatingsByMovieId(int movieId)
        {
            if (movieId == 0)
            {
                ModelState.AddModelError("movieId", "MovieId is Required");
                return BadRequest(ModelState);
            }
            var res = _ratingService.GetRatingsByMovieId(movieId);
            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }
    }
}
