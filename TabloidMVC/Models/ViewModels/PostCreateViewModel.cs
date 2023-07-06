using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TabloidMVC.Models.ViewModels
{
    public class PostCreateViewModel
    {
        public Post Post { get; set; }
        public List<Category> CategoryOptions { get; set; }
        public List<Comment> Comments { get; set; }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [DisplayName("Header Image URL")]
        public string ImageLocation { get; set; }

        public DateTime CreateDateTime { get; set; }

        [DisplayName("Published")]
        [DataType(DataType.Date)]
        public DateTime? PublishDateTime { get; set; }

        public bool IsApproved { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [DisplayName("Author")]
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

    }
}
