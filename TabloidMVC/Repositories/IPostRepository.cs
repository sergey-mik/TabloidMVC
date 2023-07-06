using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        List<Post> GetAllPublishedPosts();
        Post GetPublishedPostById(int id);
        Post GetUserPostById(int id, int userProfileId);
        List<Post> GetAllPostsByCurrentUser(int userId);
        void DeletePost(int id);
        void EditPost(Post post);

        int GetWordCount(Post post);
        int GetEstimatedReadingTime(Post post);

        void ToggleApproval(int id);
    }
}