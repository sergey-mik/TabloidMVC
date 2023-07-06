using System.ComponentModel.DataAnnotations;

namespace TabloidMVC.Models.ViewModels
{
    public class CommentCreateViewModel
    {
        public int PostId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }
    }
}