using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // GET: CommentController
        public ActionResult Index(int postId)
        {
            var comments = _commentRepository.GetCommentsByPostId(postId)
                .OrderByDescending(c => c.CreateDateTime)
                .ToList();
            ViewBag.PostId = postId;
            return View(comments);
        }

        // GET: CommentController/Details/5
        public ActionResult Details(int id)
        {
            // Retrieve the comment from the database using its id
            var comment = _commentRepository.GetCommentById(id);

            // Pass the comment to the Details view
            return View(comment);
        }

        // GET: CommentController/Create
        public ActionResult Create(int postId)
        {
            CommentCreateViewModel viewModel = new CommentCreateViewModel();
            viewModel.PostId = postId;
            return View(viewModel);
        }

        // POST: CommentController/Create
        [HttpPost]
        public ActionResult Create(CommentCreateViewModel viewModel)
        {
            // Get the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new Comment object
            Comment newComment = new Comment();
            newComment.Subject = viewModel.Subject;
            newComment.Content = viewModel.Content;
            newComment.PostId = viewModel.PostId;
            newComment.CreateDateTime = DateTime.Now;
            newComment.UserProfileId = int.Parse(userId);

            // Add the new comment to the database
            _commentRepository.Add(newComment);

            // Redirect back to the post details page
            return RedirectToAction("Details", "Post", new { id = viewModel.PostId });
        }

        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            // Retrieve the comment from the database using its id
            var comment = _commentRepository.GetCommentById(id);

            // Create a new CommentCreateViewModel object and populate its properties with the data from the comment
            var viewModel = new CommentCreateViewModel();
            viewModel.PostId = comment.PostId;
            viewModel.Subject = comment.Subject;
            viewModel.Content = comment.Content;

            // Pass the viewModel to the Edit view
            return View(viewModel);
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CommentCreateViewModel viewModel)
        {
            try
            {
                // Retrieve the comment from the database using its id
                var comment = _commentRepository.GetCommentById(id);

                // Update the comment properties with the new values from the viewModel object
                comment.Subject = viewModel.Subject;
                comment.Content = viewModel.Content;

                // Save the changes to the database
                _commentRepository.UpdateComment(comment);

                // Redirect to the Details page for the updated comment
                return RedirectToAction("Details", new { id = comment.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            var comment = _commentRepository.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var comment = _commentRepository.GetCommentById(id);
                if (comment == null)
                {
                    return NotFound();
                }

                _commentRepository.DeleteComment(id);
                return RedirectToAction("Index", new { postId = comment.PostId });
            }
            catch
            {
                return View();
            }
        }
    }
}
