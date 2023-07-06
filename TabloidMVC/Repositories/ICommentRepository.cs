using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICommentRepository
    {
        void Add(Comment comment);
        List<Comment> GetCommentsByPostId(int postId);
        Comment GetCommentById(int id);
        void DeleteComment(int id);
        void UpdateComment(Comment comment);
    }
}
