using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace GalleryApplication.Data
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Country> GetCountryByNameAsync(string country)
        {
            return await _context.Countries.SingleOrDefaultAsync(x => x.CountryName == country);
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.ToListAsync();
        }
    }
}