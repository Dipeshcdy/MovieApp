using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Infrastructure.Interface;
using MovieApp.Models;

namespace MovieApp.Infrastructure.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly bool _useStoreProcedure;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentRepository(ApplicationDbContext context, IConfiguration configuration,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _useStoreProcedure = _configuration.GetValue<bool>("UseStoreProcedure");
        }

        public bool Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Comment> GetAll()
        {
           if(_useStoreProcedure)
            {
                var result=_context.Comments.FromSqlRaw("EXEC GetComents").ToList();
                return result;
            }
           else
            {
                var result= _context.Comments.ToList();
                return result;
            }
        }

        public bool Insert(Comment comment)
        {
            if (_useStoreProcedure)
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC InsertComment @UserId,@MovieId,@Text",
                    new SqlParameter("@UserId",comment.UserId),
                    new SqlParameter("@MovieId", comment.MovieId),
                    new SqlParameter("@Text", comment.Text)
                    );
                return result == 1;
            }
            else
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();
                return true;
            }
        }

        public bool Update(Comment comment)
        {
            if (_useStoreProcedure)
            {
                var result = _context.Database.ExecuteSqlRaw("EXEC UpdateComment @Id,@Text",
                    new SqlParameter("@UserId", comment.Id),
                    new SqlParameter("@Text", comment.Text)
                    );
                return result == 1;
            }
            else
            {
                _context.Comments.Update(comment);
                _context.SaveChanges();
                return true;
            }
        }
    }
}
