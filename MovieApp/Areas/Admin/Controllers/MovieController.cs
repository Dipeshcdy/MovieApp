using Microsoft.AspNetCore.Mvc;
using MovieApp.Infrastructure.Interface;
using MovieApp.Infrastructure.Repository;
using MovieApp.Models;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IRatingRepository _ratingRepository;

        public MovieController(IMovieRepository movieRepository, ICommentRepository commentRepository, IRatingRepository ratingRepository)
        {
            _movieRepository = movieRepository;
            _commentRepository = commentRepository;
            _ratingRepository = ratingRepository;
        }

        private IActionResult HandleAuthorization()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Login", "Authentication", new { area = "User" });
            }

            if (!User.IsInRole("Admin"))
            {
                TempData["error"] = "Not Authorized";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            return null;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            var movies = _movieRepository.GetAll();
            return View(movies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, IFormFile? file)
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    movie.ImageUrl = fileName;
                    bool res = _movieRepository.Insert(movie);
                    if (res)
                    {
                        TempData["success"] = "Movie Created Successfully!";
                    }
                    else
                    {
                        TempData["error"] = "Opps! Something Went Wrong";
                    }
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Image is required!!";
            }
            return View(movie);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var movie = _movieRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile? file)
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, movie.ImageUrl);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    filePath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    movie.ImageUrl = fileName;

                }
                bool res = _movieRepository.Update(movie);
                if (res)
                {
                    TempData["success"] = "Movie Updated Successfully!";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Something Went Wrong!!";
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var movie = _movieRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (movie == null)
            {
                return NotFound();
            }
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            string filePath = Path.Combine(uploadFolder, movie.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            bool res = _movieRepository.Delete(movie);
            if (res)
            {
                TempData["success"] = "Movie Deleted Successfully!";
            }
            else
            {
                TempData["error"] = "Opps! Something went wrong!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult View(int? id)
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var movie = _movieRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (movie == null)
            {
                return NotFound();
            }
            var comments = _commentRepository.GetAll().Where(x => x.MovieId == movie.Id);
            var ratings = _ratingRepository.GetAll().Where(x => x.MovieId == movie.Id);
            movie.Comments = comments;
            movie.Ratings = ratings;
            return View(movie);
        }
    }
}
