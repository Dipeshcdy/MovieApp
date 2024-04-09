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

        public RatingRepository(ApplicationDbContext context, IMovieRepository movieRepository)
        {
            _context = context;
            _movieRepository = movieRepository;
        }

        public bool Delete(Rating rating)
        {
            _context.Ratings.Remove(rating);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Rating> GetAll()
        {
            
            return _context.Ratings.Include(x=>x.User).ToList();
        }
        
        public bool Insert(Rating rating)
        {
            _context.Ratings.Add(rating);
            _context.SaveChanges();
            Movie Movie = _movieRepository.GetAll().Where(x => x.Id == rating.MovieId).FirstOrDefault();
            var ratings=_context.Ratings.Where(x=>x.MovieId==Movie.Id);
            if(ratings.Any())
            {
                float totalRating = 0;
                foreach (var item in ratings)
                {
                    totalRating += item.Value;
                }
                float averageRating = totalRating/ratings.Count();
                Movie.Rated = averageRating;
            }
            else
            {
                Movie.Rated=rating.Value;
            }
            _movieRepository.Update(Movie);
            return true;
        }

        public bool Update(Rating rating)
        {
            _context.Ratings.Update(rating);
            _context.SaveChanges();
            return true;
        }
    }
}
