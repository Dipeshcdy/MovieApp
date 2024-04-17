using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;
using System.Drawing.Printing;
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
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _configuration;

        public MovieController(IMovieRepository movieRepository, ICommentRepository commentRepository, IRatingRepository ratingRepository, UserManager<ApplicationUser> userManager, IEmailRepository emailRepository, IConfiguration configuration)
        {
            _movieRepository = movieRepository;
            _commentRepository = commentRepository;
            _ratingRepository = ratingRepository;
            _userManager = userManager;
            _emailRepository = emailRepository;
            _configuration = configuration;
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
        [HttpGet]

        public IActionResult Index(int page = 1)
        {
            int pageSize = 2;
            int count = _movieRepository.Count().Result;
            var movies = _movieRepository.GetAll();
            var pageRecords=movies.Skip((page-1)*pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;
            ViewBag.MaxPage= (count / pageSize) + (count % pageSize != 0 ? 1 : 0);
            return View(pageRecords);
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

        [HttpGet]
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
        [HttpGet]

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
            var comments = _commentRepository.GetAll().Where(x=>x.MovieId==movie.Id);
            var ratings = _ratingRepository.GetAll().Where(x => x.MovieId == movie.Id);
            
            movie.Comments = comments;
            movie.Ratings = ratings;
            return movie;
        }

        [Route("SendEmail")]
        [HttpPost]
        public IActionResult sendEmail(int movieId,string email)
        {
            IActionResult authResult = HandleAuthentication();
            if (authResult != null)
            {
                return authResult;
            }
            Movie movie = GetById(movieId);
            _emailRepository.sendEmail(movie,email);
            return RedirectToAction("Index");
        }

        

    }
}
