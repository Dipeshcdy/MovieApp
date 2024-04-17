using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;

namespace MovieApp.Infrastructure.Repository
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieRepository _movieRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingRepository(ApplicationDbContext context, IMovieRepository movieRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _movieRepository = movieRepository;
            _userManager = userManager;
        }

        public bool Delete(Rating rating)
        {
            _context.Ratings.Remove(rating);
            _context.SaveChanges();
            bool res = UpdateMovieRating(rating);
            return res;
        }

        public IEnumerable<Rating> GetAll()
        {
            List<Rating> result;
            result=_context.Ratings.ToList();
            foreach (var item in result)
            {
                item.User = GetIdentityUserById(item.UserId);
            }
            return result;
        }
        
        public bool Insert(Rating rating)
        {
            _context.Ratings.Add(rating);
            _context.SaveChanges();
            bool res=UpdateMovieRating(rating);
            return res;

        }

        public bool Update(Rating rating)
        {
            _context.Ratings.Update(rating);
            _context.SaveChanges();
            bool res=UpdateMovieRating(rating);
            return res;
        }
        public ApplicationUser GetIdentityUserById(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            return user;
        }
        private bool UpdateMovieRating(Rating rating)
        {
            Movie Movie = _movieRepository.GetAll().Where(x => x.Id == rating.MovieId).FirstOrDefault();
            var ratings = _context.Ratings.Where(x => x.MovieId == Movie.Id);
            if (ratings.Any())
            {
                float totalRating = 0;
                foreach (var item in ratings)
                {
                    totalRating += item.Value;
                }
                float averageRating = totalRating / ratings.Count();
                Movie.Rated = averageRating;
            }
            else
            {
                Movie.Rated = 0;
            }
            _movieRepository.Update(Movie);
            return true;
        }
    }
}
