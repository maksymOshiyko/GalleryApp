using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using GalleryApplication.Services;
using GalleryApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GalleryApplication.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string country, string lastName,
            string firstName, string gender, int page = 1)
        {
            
            var countries = await _unitOfWork.CountryRepository.GetAllCountriesAsync();
            countries.Insert(0, new Country{ CountryName = "All countries" });
            SelectList selectList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.Countries = selectList;
            
            ViewBag.CurrentUsername = User.Identity.Name;

            var userFilterParams = new UserFilterParams
            {
                Country = string.IsNullOrEmpty(country) ? "All countries" : country,
                Gender = string.IsNullOrEmpty(gender) ? "Male or female" : gender,
                CurrentUsername = User.Identity.Name,
                FirstName = firstName,
                LastName = lastName
            };
            var users = await _unitOfWork.UserRepository.GetUsersAsync(userFilterParams);
            
            int pageSize = 10;
            var count = users.Count();
            var items = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            PaginatedUsersViewModel viewModel = new PaginatedUsersViewModel()
            {
                PageViewModel = pageViewModel,
                Users = items,
                Country = country,
                Gender = gender,
                CurrentUsername = User.Identity.Name,
                FirstName = firstName,
                LastName = lastName
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("Users/Detail/{username}")]
        public async Task<IActionResult> Detail([FromRoute] string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            if (user == null) return RedirectToAction("NotFoundResponse", "Error");

            // var item = user.FollowedByUsers.Select(f => f.SourceUser.UserName).ToList();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> FollowUser(string username)
        {
            var currentUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);
            var followedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            
            if (followedUser == null) return RedirectToAction("NotFoundResponse", "Error");

            await _unitOfWork.UserRepository.FollowUser(currentUser, followedUser);

            await _unitOfWork.Complete();

            return RedirectToAction("Detail", "Users", new {username});
        }

        [HttpPost]
        public async Task<IActionResult> UnfollowUser(string username)
        {
            var currentUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);
            var followedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            
            if (followedUser == null) return RedirectToAction("NotFoundResponse", "Error");
            
            await _unitOfWork.UserRepository.UnfollowUser(currentUser, followedUser);

            await _unitOfWork.Complete();
            
            return RedirectToAction("Detail", "Users", new {username});
        }

        [HttpGet]
        [Route("[controller]/Update")]
        public async Task<IActionResult> UpdateUser()
        {
            var currentUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);
            
            var countries = (await _unitOfWork.CountryRepository.GetAllCountriesAsync()).Select(x => x.CountryName);
            SelectList selectList = new SelectList(countries);
            ViewBag.Countries = selectList;
            
            var model = new UpdateUserViewModel()
            {
                Country = currentUser.Country.CountryName,
                Gender = currentUser.Gender,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                DateOfBirth = currentUser.DateOfBirth.ToShortDateString()
            };
            return View(model);
        }

        [HttpPost]
        [Route("[controller]/Update")]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.Country == "Choose country") ModelState.AddModelError("Country", "Choose country");

                if (ModelState.IsValid)
                {
                    var currentUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.Identity.Name);

                    currentUser.Gender = currentUser.Gender != model.Gender ? model.Gender : currentUser.Gender;
                    currentUser.FirstName = currentUser.FirstName != model.FirstName 
                        ? model.FirstName : currentUser.FirstName;
                    currentUser.LastName = currentUser.LastName != model.LastName 
                        ? model.LastName : currentUser.LastName;
                    currentUser.Country = currentUser.Country.CountryName != model.Country
                        ? await _unitOfWork.CountryRepository.GetCountryByNameAsync(model.Country)
                        : currentUser.Country;
                    if (model.DateOfBirth != null)
                    {
                        currentUser.DateOfBirth = currentUser.DateOfBirth != Convert.ToDateTime(model.DateOfBirth)
                            ? Convert.ToDateTime(model.DateOfBirth)
                            : currentUser.DateOfBirth;
                    }

                    if (model.Image != null)
                    {
                        await _photoService.DeletePhotoAsync(currentUser.MainPhotoPublicId);
                        var result = await _photoService.AddPhotoAsync(model.Image);
                        if (result.Error != null)
                        {
                            ModelState.AddModelError("Image", "Something went wrong");
                            return View(model);
                        }
                        currentUser.MainPhotoUrl = result.SecureUrl.AbsoluteUri;
                        currentUser.MainPhotoPublicId = result.PublicId;
                    }

                    await _unitOfWork.Complete();

                    return RedirectToAction("Detail", "Users",
                        new {username = currentUser.UserName});
                }
            }

            var countries = (await _unitOfWork.CountryRepository.GetAllCountriesAsync()).Select(x => x.CountryName);
            SelectList selectList = new SelectList(countries);
            ViewBag.Countries = selectList;
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Charts()
        {
            return View();
        }
    }
}