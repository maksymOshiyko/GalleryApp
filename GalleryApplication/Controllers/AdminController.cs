using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GalleryApplication.Controllers
{
    [Authorize(Roles = "moderator")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

       
        public async Task<IActionResult> Index()
        {
            var posts = await _unitOfWork.PostRepository.GetPostsWithComplaints();
            return View(posts);
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet("/Admin/EditRoles/{username}")]
        public async Task<IActionResult> EditRoles(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            
            var userRoles = await _userManager.GetRolesAsync(user);

            EditRolesViewModel model = new EditRolesViewModel
            {
                Admin = userRoles.Contains("admin"),
                Moderator = userRoles.Contains("moderator")
            };
            
            return View(model);
        }
        
        [HttpPost("/Admin/EditRoles/{username}")]
        public async Task<IActionResult> EditRoles(EditRolesViewModel model, [FromRoute]string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) RedirectToAction("NotFoundResponse", "Error");
            
            var selectedRoles = new List<string>();

            if (model.Admin) selectedRoles.Add("admin");
            
            if (model.Moderator) selectedRoles.Add("moderator");
            
            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            
            if (!result.Succeeded) return RedirectToAction("ServerError", "Error",
                new {msg = "Something went wrong."});

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            
            if (!result.Succeeded) return RedirectToAction("ServerError", "Error",
                new {msg = "Something went wrong."});
            
            return RedirectToAction("Index", "Admin");
        }
    }
}