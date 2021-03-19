using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GalleryApplication.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ChartsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChartsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet("GenderData")]
        public async Task<ActionResult> GenderData()
        {
            var users = await _unitOfWork.UserRepository.GetUsersAsync(new UserFilterParams());

            List<object> result = new List<object>();
            
            result.Add(new[] {"Gender", "Amount"});
            result.Add(new object[] {"Male", users.Count(x => x.Gender == "male")});
            result.Add(new object[] {"Female", users.Count(x => x.Gender == "female")});

            return new JsonResult(result);
        }
        
        [HttpGet("CountryData")]
        public async Task<ActionResult> CountryData()
        {
            var countries = await _unitOfWork.CountryRepository.GetAllCountriesAsync();

            List<object> result = new List<object>();
            
            result.Add(new[] {"Country", "Amount"});
            
            foreach (var country in countries)
            {
                result.Add(new object[] {country.CountryName, country.Users.Count});
            }

            return new JsonResult(result);
        }
    }
}