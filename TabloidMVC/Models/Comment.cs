using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace TabloidMVC.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Subject")]
        public string Subject { get; set; }

        [Required]
        [DisplayName("Content")]
        public string Content { get; set; }

        [Required]
        [DisplayName("Post")]
        public int PostId { get; set; }

        [DisplayName("Date Created")]
        public DateTime CreateDateTime { get; set; }

        public int UserProfileId { get; set; }

        public UserProfile UserProfile { get; set; }
    }

}

