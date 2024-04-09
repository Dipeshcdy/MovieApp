using MovieApp.Models;

namespace MovieApp.Infrastructure.Interface
{
    public interface IRatingRepository
    {
        IEnumerable<Rating> GetAll();
        bool Insert(Rating rating);
        bool Update(Rating rating);
        bool Delete(Rating rating);
    }
}
