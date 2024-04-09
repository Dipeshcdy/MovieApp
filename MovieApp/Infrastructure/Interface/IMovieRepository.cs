using MovieApp.Models;

namespace MovieApp.Infrastructure.Interface
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAll();
        Task<IEnumerable<Movie>> SearchMoviesAsync(string Key);
        bool Insert(Movie movie);
        bool Update(Movie movie);
        bool Delete(Movie movie);
        bool HasUserRatedMovie(string UserId, int MovieId);
    }
}
