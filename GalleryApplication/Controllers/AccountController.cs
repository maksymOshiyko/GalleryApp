using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace GalleryApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork,
            SignInManager<AppUser> signInManager, IEmailService emailService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _emailService = emailService;
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
                if (await UserExists(model.UserName)) ModelState.AddModelError("UserName", "Login is taken");
                
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
                        await _userManager.AddToRoleAsync(user, "User");
                        
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            new {userId = user.Id, token},
                             HttpContext.Request.Scheme
                            );
                        
                        await _emailService.SendEmailAsync(model.Email, "Confirm your account",
                            $"Thank you for registering MyGallery account." +
                            $" In order to verify your account click here: <a href='{callbackUrl}'>link</a>");
                        
                        
                        return RedirectToAction("AccountConfirmation", "Account");
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
            ViewBag.LoginError = null;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            {
                ViewBag.LoginError = "Invalid input";
                return View();
            }
                
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.UserName == username.ToLower());
            ViewBag.LoginError = null;
            
            if (user == null)
            {
                ViewBag.LoginError = "Invalid credentials";
                return View();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                if (ViewBag.LoginError == null)
                {
                    ViewBag.LoginError = "Confirm your email firstly";
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, true, false);

            if (!result.Succeeded)
            { 
                ViewBag.LoginError = "Invalid credentials";
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
        
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);
            
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    IdentityResult result = 
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("PasswordSaving", "Home");
                    }
                    
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    
                }
                else
                {
                    return RedirectToAction("NotFoundResponse", "Error");
                }
            }
            return View(model);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
                return RedirectToAction("SuccessEmailConfirmation", "Account");
            else
                return View("Error");
        }

        [HttpGet]
        public IActionResult AccountConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SuccessEmailConfirmation()
        {
            return View();
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