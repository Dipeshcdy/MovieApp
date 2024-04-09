using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;
using System.Data;

namespace MovieApp.Infrastructure.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly bool _useStoreProcedure;
        public MovieRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _useStoreProcedure = _configuration.GetValue<bool>("UseStoreProcedure");
        }

        public bool Delete(Movie movie)
        {
            if (_useStoreProcedure)
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC DeleteMovie @MovieId",
                    new SqlParameter("@MovieId",movie.Id)
                    );
              
                return result == 1;
            }
            else
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return true;
            }
        }

        public IEnumerable<Movie> GetAll()
        {

            if(_useStoreProcedure)
            {
                return _context.Movies.FromSqlRaw("EXEC GetMovies").ToList();
            }
            else
            {
                return _context.Movies.ToList();
            }
        }

        public bool HasUserRatedMovie(string userId, int movieId)
        {
            if (_useStoreProcedure)
            {
                var result = _context.Ratings
                .FromSqlRaw("EXEC CheckUserMovieRating @UserId, @MovieId",
                             new SqlParameter("@UserId", userId),
                             new SqlParameter("@MovieId", movieId))
                .AsEnumerable()
                .FirstOrDefault();
                
                return result !=null;
            }
            else
            {
                return _context.Ratings.Any(r => r.UserId == userId && r.MovieId == movieId);
            }
        }

        public bool Insert(Movie movie)
        {
            if(_useStoreProcedure)
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC InsertMovie @Title, @Description, @ImageUrl",
                    new SqlParameter("@Title", movie.Title),
                    new SqlParameter("@Description", movie.Description),
                    new SqlParameter("@ImageUrl", movie.ImageUrl)
                    );
                if (result != 1)
                {
                    return false;
                }
               /* _command.CommandText = "InsertMovie";
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.Clear();
                _command.Parameters.Add(new SqlParameter("@Title", movie.Title));
                _command.Parameters.Add(new SqlParameter("@Description", movie.Description));
                _command.Parameters.Add(new SqlParameter("@ImageUrl", movie.ImageUrl));
                var result =_command.ExecuteNonQuery();
                if (result != 1)
                {
                    return false;
                }*/
            }
            else
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string Key)
        {
            if(_useStoreProcedure)
            {
                var Searchkey = new SqlParameter("@SearchKey", Key);
                return await _context.Movies.FromSqlRaw("EXEC SearchMovies @SearchKey", Searchkey).ToListAsync();
            }
            else
            {
                return await _context.Movies.Where(m => m.Title.Contains(Key)).ToListAsync();
            }
        }

        public bool Update(Movie movie)
        {
            if(_useStoreProcedure)
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC UpdateMovie @Id,@Title, @Description, @ImageUrl,@Rated",
                    new SqlParameter("@Id",movie.Id),
                    new SqlParameter("@Title", movie.Title),
                    new SqlParameter("@Description", movie.Description),
                    new SqlParameter("@ImageUrl", movie.ImageUrl),
                    new SqlParameter("@Rated", movie.Rated)
                    );
                if (result != 1)
                {
                    return false;
                }
            }
            else
            {
                _context.Movies.Update(movie);
                _context.SaveChanges();
            }
            return true;
        }
    }
}
