using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;
using System.Security.Claims;

namespace MovieApp.Areas.User.Controllers
{
    [Area("User")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public MovieController(IMovieRepository movieRepository, ICommentRepository commentRepository, IRatingRepository ratingRepository, UserManager<ApplicationUser> userManager)
        {
            _movieRepository = movieRepository;
            _commentRepository = commentRepository;
            _ratingRepository = ratingRepository;
            _userManager = userManager;
        }

        private IActionResult HandleAuthentication()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return null; // Indicates no authorization issue
        }
        [Route("movies")]

        public IActionResult Index()
        {
            var movies = _movieRepository.GetAll();
            return View(movies);
        }

        [HttpGet]
        [Route("movies/{id}")]
        public IActionResult Details(int? id)
        {
            IActionResult authResult = HandleAuthentication();
            if (authResult != null)
            {
                return authResult;
            }
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Movie movie= GetById(id);
            var UserId = _userManager.GetUserId(User);
            bool HasRated=_movieRepository.HasUserRatedMovie(UserId, movie.Id);
            ViewBag.HasRated = HasRated;
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comment(Comment comment)
        {
            IActionResult authResult = HandleAuthentication();
            if (authResult != null)
            {
                return authResult;
            }
            if (ModelState.IsValid)
            {
                var UserId = _userManager.GetUserId(User);
                comment.UserId = UserId;
                bool res=_commentRepository.Insert(comment);
                if (res)
                {
                    TempData["success"] = "Comment Added Successfully!";
                }
                else
                {
                    TempData["error"] = "Opps! SomeThing Went Wrong!!";
                }
            }
            else
            {
                TempData["error"] = "Comment is Required!";
            }
            return RedirectToAction("Details", new {id=comment.MovieId});
        }

        public IActionResult AddRating(Rating rating)
        {
            IActionResult authResult = HandleAuthentication();
            if (authResult != null)
            {
                return authResult;
            }
            if(ModelState.IsValid)
            {
                var UserId = _userManager.GetUserId(User);
                rating.UserId = UserId;
                bool res=_ratingRepository.Insert(rating);
                if (res)
                {
                    TempData["success"] = "Rating Added Successfully!";
                }
                else
                {
                    TempData["error"] = "Opps! SomeThing Went Wrong!!";
                }
            }
            else
            {
                TempData["error"] = "Rating is Required!";
            }
            return RedirectToAction("Details", new { id = rating.MovieId });
        }
        [Route("Search")]

        public async Task<IActionResult> Search(string key)
        {
            if(key==null)
            {
                TempData["error"] = "Search Key is Required";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            key = key.ToLower();
            var movies = await _movieRepository.SearchMoviesAsync(key);
            ViewBag.Key=key;
            return View("Index",movies);
        }
        
        private Movie GetById(int? id)
        {
            
            var movie = _movieRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (movie == null)
            {
                return null;
            }
            var comments = _commentRepository.GetAll();
            var ratings = _ratingRepository.GetAll().Where(x => x.MovieId == movie.Id);
           
            movie.Comments = comments;
            movie.Ratings = ratings;
            return movie;
        }
        
    }
}
