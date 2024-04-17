using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.classes;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Models;

namespace MoveiAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentController(ICommentService commentService, UserManager<ApplicationUser> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateComment([FromForm] CommentData comment)
        {
            if (ModelState.IsValid)
            {
                var UserId = _userManager.GetUserId(User);
                var res = _commentService.Create(comment, UserId);
                return Ok(res);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteComment(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Id is Required");
                return BadRequest(ModelState);
            }
            var res = _commentService.Delete(id);
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
        public ActionResult GetCommentsByMovieId(int movieId)
        {
            if (movieId == 0)
            {
                ModelState.AddModelError("movieId", "MovieId is Required");
                return BadRequest(ModelState);
            }
            var res = _commentService.GetCommentsByMovieId(movieId);
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
