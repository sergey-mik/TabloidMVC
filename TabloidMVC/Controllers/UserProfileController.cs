using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TabloidMVC.Repositories;
using TabloidMVC.Models;
using System.Security.Claims;
using System;
using TabloidMVC.Models.ViewModels;

namespace TabloidMVC.Controllers
{
    public class UserProfileController : Controller
	{
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserTypeRepository _userTypeRepository;

        public UserProfileController(
            IUserProfileRepository userProfileRepository,
            IUserTypeRepository userTypeRepository)
        {
            _userProfileRepository = userProfileRepository;
            _userTypeRepository = userTypeRepository;
        }

        public ActionResult Index()
        {
            List<UserProfile> userProfiles = _userProfileRepository.GetUserProfiles();

            int userId = GetCurrentUserProfileId();

            UserProfile userProfile = _userProfileRepository.GetByUserId(userId);

            if (userProfile.UserType.Name != "Admin")
            {
                return NotFound();
            }
            return View(userProfiles);
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        public IActionResult Details(int id)
        {
            var user = _userProfileRepository.GetByUserId(id);
            return View(user);
        }

        public ActionResult Deactivate(int id)
        {
            UserProfile profile = _userProfileRepository.GetByUserId(id);
            if (profile.UserTypeId == 1 || profile.UserTypeId == 2)
            {
                return View(profile);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deactivate(int id, UserProfile profile)
        {
            try
            {
                    _userProfileRepository.DeactivateProfile(profile);
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(profile);
            }
        }

        public ActionResult Activate(int id)
        {
            UserProfile profile = _userProfileRepository.GetByUserId(id);
            if (profile.UserTypeId == 3 || profile.UserTypeId == 4)
            {
                return View(profile);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int id, UserProfile profile)
        {
            try
            {
                _userProfileRepository.ActivateProfile(profile);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(profile);
            }
        }

        public ActionResult InactiveIndex()
        {
            List<UserProfile> inactiveUserProfiles = _userProfileRepository.GetDeactivatedUserProfiles();
            return View(inactiveUserProfiles);
        }

        public ActionResult Edit(int id)
        {
            UserProfile profile = _userProfileRepository.GetByUserId(id);

            if (profile == null)
            {
                return NotFound();
            }

            List<UserType> userTypes = _userTypeRepository.GetAllTypes();

            UserProfileEditViewModel vm = new UserProfileEditViewModel()
            {
                UserProfile = profile,
                UserTypes = userTypes
            };

            return View(vm);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserProfile UserProfile)
        {
            try
            {
                _userProfileRepository.UpdateProfile(UserProfile);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return RedirectToAction("Index");
            }
        }
    }
}

