using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoveiAppApi.Services.Interfaces;
using MovieAppApi.DTOs;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Identity;
using MoveiAppApi.DTOs;
using MovieApp.Infrastructure.Repository;

namespace MoveiAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MovieController(IMovieService movieService, UserManager<ApplicationUser> userManager)
        {
            _movieService = movieService;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult GetMovies()
        {
            var res = _movieService.GetAll();
            return Ok(res);
        }
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult GetMovieById(int id)
        {
            var UserId = _userManager.GetUserId(User);
            var res = _movieService.GetById(id, UserId);
            if(!res.Success)
            {
                return NotFound();
            }
            return Ok(res);
        }
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public ActionResult CreateMovie([FromForm] MovieData movieData)
        {
            if(ModelState.IsValid)
            {
                if(movieData.file !=null)
                {
                    var res=_movieService.Create(movieData);
                    if(res.Success)
                    {
                        return Ok(res);
                    }
                    else
                    {
                        return BadRequest(res);
                    }
                }
                ModelState.AddModelError("file", "Please select a file to upload.");
                return BadRequest(ModelState);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public ActionResult UpdateMovie([FromForm] MovieData movieData,int id)
        {
            if (ModelState.IsValid)
            {
                var res = _movieService.Update(movieData,id);
                if(res.Success)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy ="RequireAdminRole")]
        public ActionResult DeleteMovie(int id)
        {
            if(id == 0)
            {
                ModelState.AddModelError("id", "Id is Required");
                return BadRequest(ModelState);
            }
            var res= _movieService.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }

        [HttpGet("search/{key}")]
        [AllowAnonymous]
        public ActionResult SearchMovie(string key)
        {
            if(key== null)
            {
                ModelState.AddModelError("key", "key is Required");
                return BadRequest(ModelState);
            }
            var res=_movieService.Search(key).Result;
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
