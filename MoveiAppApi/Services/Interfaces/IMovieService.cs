using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;
using MovieApp.Models;
using MovieAppApi.DTOs;

namespace MoveiAppApi.Services.Interfaces
{
    public interface IMovieService
    {
        public ResponseData GetAll();
        public Task<ResponseData> Search(string key);
        public ResponseData GetById(int id,string userId);
        public ResponseMessage Create(MovieData movieData);
        public ResponseMessage Update(MovieData movieData,int id);
        public ResponseMessage Delete(int id);




    }
}
