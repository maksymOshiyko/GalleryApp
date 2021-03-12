using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
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

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        // [HttpGet]
        // [Route("[controller]/Update")]
        // public async Task<IActionResult> UpdateUser()
        // {
        //     
        // }
        
        // [HttpPost]
        // [Route("[controller]/Update")]
        // public async Task<IActionResult> UpdateUser(IFormFile file, )
        // {
        //     
        // }
    }
}