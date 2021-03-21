using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IMapper = AutoMapper.IMapper;

namespace GalleryApplication.Controllers
{
    [Authorize(Roles = "moderator")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IExcelService excelService, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _excelService = excelService;
            _mapper = mapper;
        }

       
        public async Task<IActionResult> Index()
        {
            var posts = await _unitOfWork.PostRepository.GetPostsWithComplaints();
            return View(posts);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UsersControl()
        {
            var users = (await _unitOfWork.UserRepository.GetUsersAsync(new UserFilterParams()))
                .Where(x => x.UserName != User.Identity.Name);
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string username)
        {
            await _unitOfWork.UserRepository.DeleteUser(
                await _unitOfWork.UserRepository.GetUserByUsernameAsync(username));
            await _unitOfWork.Complete();
            return RedirectToAction("UsersControl", "Admin");
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

        [HttpPost]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            var usersData = await _excelService.ImportUsers(file);
            ViewBag.Succeeded = true;
            
            if (!usersData.Any())
            {
                ViewBag.Succeeded = false;
                return View();
            }

            foreach (var userData in usersData)
            {
                var user = _mapper.Map<AppUser>(userData);
                user.Country = await _unitOfWork.CountryRepository.GetCountryByNameAsync(userData.Country);
                user.UserName = userData.UserName.ToLower();

                var result = await _userManager.CreateAsync(user, userData.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    user.EmailConfirmed = true;
                }
            }

            await _unitOfWork.Complete();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExportUsersInfo()
        {
            var stream = await _excelService.ExportUsersInfo();
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"users_info_{DateTime.Now.ToString("g")}.xlsx");
        }
    }
}