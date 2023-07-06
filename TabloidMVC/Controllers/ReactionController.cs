using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TabloidMVC.Repositories;
using TabloidMVC.Models;
using System.Collections.Generic;

namespace TabloidMVC.Controllers
{
    public class ReactionController : Controller
    {
        private readonly IReactionRepository _reactionRepo;
        public ReactionController(IReactionRepository reactionRepo)
        {
            _reactionRepo = reactionRepo;
        }
        // GET: ReactionController
        public ActionResult Index()
        {
            List<Reaction> reactions = _reactionRepo.GetAll();
            return View(reactions);
        }

        // GET: ReactionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReactionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReactionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reaction r)
        {
            try
            {
                _reactionRepo.CreateReaction(r);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(r);
            }
        }

        // GET: ReactionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReactionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReactionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReactionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
