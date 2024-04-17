using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;

namespace MoveiAppApi.Services.Interfaces
{
    public interface ICommentService
    {
        public ResponseMessage Create(CommentData rating, string userId);
        public ResponseMessage Delete(int id);
        public ResponseData GetCommentsByMovieId(int movieId);
    }
}
