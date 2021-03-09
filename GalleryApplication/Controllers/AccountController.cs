using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Data;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace GalleryApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork,
            SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var countries = (await _unitOfWork.CountryRepository.GetAllCountriesAsync());
            SelectList selectList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.Countries = selectList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await UserExists(model.UserName)) ModelState.AddModelError("UserName", "Login is taked");
                
                if(await EmailExists(model.Email)) ModelState.AddModelError("Email", "Email is taken");
                
                if(model.Country == "Choose country") ModelState.AddModelError("Country", "Choose country");
                
                var user = _mapper.Map<AppUser>(model);
                user.Country = await _unitOfWork.CountryRepository.GetCountryByNameAsync(model.Country);
                user.UserName = model.UserName.ToLower();

                if (ModelState.IsValid)
                {
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, "User");
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            var countries = (await _unitOfWork.CountryRepository.GetAllCountriesAsync()).Select(x => x.CountryName);
            SelectList selectList = new SelectList(countries);
            ViewBag.Countries = selectList;
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.LoginError = false;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.UserName == username.ToLower());
            ViewBag.LoginError = false;
            
            if (user == null)
            {
                ViewBag.LoginError = true;
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, true, false);

            if (!result.Succeeded)
            {
                _logger.LogInformation("1");
                ViewBag.LoginError = true;
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        private async Task<bool> EmailExists(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }
    }
}