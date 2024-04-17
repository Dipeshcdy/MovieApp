using Azure;
using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Infrastructure.Interface;
using MovieApp.Infrastructure.Repository;
using MovieApp.Models;

namespace MoveiAppApi.Services.classes
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public ResponseMessage Create(RatingData ratingData, string userId)
        {
            Rating rating=new Rating()
            {
                MovieId = ratingData.MovieId,
                UserId = userId,
                Value=ratingData.Value
            };
            ResponseMessage response = new ResponseMessage();
            bool res=_ratingRepository.Insert(rating);
            if (res)
            {
                response.Success = true;
                response.Message = "Rating Added Successfully!";
            }
            else
            {
                response.Success = false;
                response.Message = "Something Went Wrong";
            }
            return response;
        }
        public ResponseMessage Delete(int id)
        {
            ResponseMessage response = new ResponseMessage();
            Rating rating = _ratingRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if(rating == null)
            {
                response.Success = false;
                response.Message = "Rating Doesnot Exist!";
                return response;
            }
            bool res = _ratingRepository.Delete(rating);
            if (res)
            {
                response.Success = true;
                response.Message = "Rating Deleted Successfully!";
            }
            else
            {
                response.Success = false;
                response.Message = "Something Went Wrong";
            }
            return response;
        }
        public ResponseData GetRatingsByMovieId(int movieId)
        {
            var ratings = _ratingRepository.GetAll().Where(x => x.MovieId == movieId);
            var ratingDataList = new List<dynamic>();
            foreach (var rating in ratings)
            {
                var ratingData = new
                {
                    Id = rating.Id,
                    Value = rating.Value,
                    UserId = rating.User.Id,
                    UserName = rating.User.UserName,
                };
                ratingDataList.Add(ratingData);
            }
            ResponseData response = new ResponseData();
            response.Data = ratingDataList;
            response.Success = true;
            return response;
        }
    }
}
