using MoveiAppApi.ApiResponses;
using MoveiAppApi.DTOs;
using MoveiAppApi.Services.Interfaces;
using MovieApp.Infrastructure.Interface;
using MovieApp.Infrastructure.Repository;
using MovieApp.Models;

namespace MoveiAppApi.Services.classes
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public ResponseMessage Create(CommentData commentData, string userId)
        {
            Comment comment= new Comment()
            {
                MovieId = commentData.MovieId,
                UserId = userId,
                Text = commentData.Text
            };
            ResponseMessage response = new ResponseMessage();
            bool res = _commentRepository.Insert(comment);
            if (res)
            {
                response.Success = true;
                response.Message = "Comment Added Successfully!";
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
            Comment comment= _commentRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            if (comment == null)
            {
                response.Success = false;
                response.Message = "Comment Doesnot Exist!";
                return response;
            }
            bool res = _commentRepository.Delete(comment);
            if (res)
            {
                response.Success = true;
                response.Message = "Comment Deleted Successfully!";
            }
            else
            {
                response.Success = false;
                response.Message = "Something Went Wrong";
            }
            return response;
        }
        public ResponseData GetCommentsByMovieId(int movieId)
        {
            var comments=_commentRepository.GetAll().Where(x=>x.MovieId== movieId);
            var commentDataList = new List<dynamic>();
            foreach (var comment in comments)
            {
                var commentData = new
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    UserId = comment.User.Id,
                    UserName = comment.User.UserName,
                };
                commentDataList.Add(commentData);
            }
            ResponseData response = new ResponseData();
            response.Data = commentDataList;
            response.Success = true;
            return response;
        }

    }
}
