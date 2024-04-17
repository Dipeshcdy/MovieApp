using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;

namespace MoveiAppApi.Services.Interfaces
{
    public interface IRatingService
    {
        public ResponseMessage Create(RatingData rating,string userId);
        public ResponseMessage Delete(int id);
        public ResponseData GetRatingsByMovieId(int movieId);

    }
}
