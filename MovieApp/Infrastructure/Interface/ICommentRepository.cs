using MovieApp.Models;

namespace MovieApp.Infrastructure.Interface
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> GetAll();
        bool Insert(Comment comment);
        bool Update(Comment comment);
        bool Delete(Comment comment);
    }
}
