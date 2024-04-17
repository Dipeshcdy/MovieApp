using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Infrastructure.Interface;
using MovieApp.Infrastructure.Repository;
using MovieApp.Models;
using MovieAppApi.DTOs;

namespace MoveiAppApi.Services.classes
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IConfiguration _configuration;

        public MovieService(IMovieRepository movieRepository, IConfiguration configuration)
        {
            _movieRepository = movieRepository;
            _configuration = configuration;
        }

        public ResponseData GetAll()
        {
            var movies= _movieRepository.GetAll();
            var movieDataList = new List<dynamic>();
            foreach (var movie in movies)
            {
                var movieData = new
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    ImageUrl = UpdateImageUrl(movie.ImageUrl),
                    Rated=movie.Rated,
                    ReleaseDate = movie.ReleaseDate
                };
                movieDataList.Add(movieData);
            }
            ResponseData response = new ResponseData()
            { 
                Success = true,
                Data = movieDataList
            };
            return response;
        }

        public ResponseData GetById(int id,string userId)
        {
            ResponseData response = new ResponseData();
            var movie =_movieRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (movie == null)
            {
                response.Success = false;
            }
            else
            {
                bool HasRated = _movieRepository.HasUserRatedMovie(userId, movie.Id);
                var Data = new
                {
                    Id=movie.Id,
                    Title=movie.Title,
                    Description=movie.Description,
                    ImageUrl=UpdateImageUrl(movie.ImageUrl),
                    Rated=movie.Rated,
                    HasRated=HasRated,
                    ReleaseDate = movie.ReleaseDate

                };
                response.Success = true;
                response.Data = Data;
            }
            return response;
        }

        public ResponseMessage Create(MovieData movieData)
        {

            Movie movie = new Movie()
            {
                Title = movieData.Title,
                Description = movieData.Description,
                ReleaseDate=movieData.ReleaseDate
            };
            var file = movieData.file;
            string uploadFolder = Path.Combine("..", "MovieApp", "wwwroot","uploads");
            string fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
            string filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            movie.ImageUrl = fileName;
            bool res = _movieRepository.Insert(movie);
            ResponseMessage response = new ResponseMessage();
            if(res)
            {
                response.Success = true;
                response.Message = "Movie Created Successfully!";
            }
            else
            {
                response.Success= false;
                response.Message = "Something Went Wrong";
            }
            return response;
        }

        public ResponseMessage Update(MovieData movieData,int id)
        {
            Movie currentMovie= _movieRepository.GetAll().Where(x=>x.Id==id).FirstOrDefault();
            currentMovie.Title = movieData.Title;
            currentMovie.Description= movieData.Description;
            currentMovie.ReleaseDate= movieData.ReleaseDate;
            if (movieData.file != null)
            {
                string uploadFolder = Path.Combine("..", "MovieApp", "wwwroot", "uploads");
                string fileName = Guid.NewGuid().ToString() + "-" + movieData.file.FileName;
                string filePath = Path.Combine(uploadFolder, currentMovie.ImageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                filePath= Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    movieData.file.CopyTo(stream);
                }
                currentMovie.ImageUrl = fileName;
            }
            bool res = _movieRepository.Update(currentMovie);
            ResponseMessage response = new ResponseMessage();
            if (res)
            {
                response.Success = true;
                response.Message = "Movie Updated Successfully!";
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
            Movie currentMovie = _movieRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (currentMovie == null)
            {
                response.Success = false;
                response.Message = "Movie Doesnot Exist!";
                return response;
            }
            string uploadFolder = Path.Combine("..", "MovieApp", "wwwroot", "uploads");
            string filePath = Path.Combine(uploadFolder, currentMovie.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            bool res= _movieRepository.Delete(currentMovie);
            if (res)
            {
                response.Success = true;
                response.Message = "Movie Deleted Successfully!";
            }
            else
            {
                response.Success = false;
                response.Message = "Something Went Wrong";
            }
            return response;
        }
        private string UpdateImageUrl(string ImageUrl)
        {
            return _configuration.GetValue<string>("APP_URL") + "/MovieApp/uploads/" + ImageUrl;
        }

        public async Task<ResponseData> Search(string key)
        {
            key = key.ToLower();
            var movies = await _movieRepository.SearchMoviesAsync(key);
            var movieDataList = new List<dynamic>();
            foreach (var movie in movies)
            {
                var movieData = new
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    ImageUrl = UpdateImageUrl(movie.ImageUrl),
                    Rated = movie.Rated,
                    ReleaseDate=movie.ReleaseDate
                };
                movieDataList.Add(movieData);
            }
            ResponseData response = new ResponseData()
            {
                Success = true,
                Data = movieDataList
            };
            return response;
        }

    }
}
