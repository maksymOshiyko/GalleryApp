using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalleryApplication.Models;

namespace GalleryApplication.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country> GetCountryByNameAsync(string country);
        Task<List<Country>> GetAllCountriesAsync();
    }
}