using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GalleryApplication.Data
{
    public class Seed
    {
        public static async Task SeedCountries(DataContext dataContext)
        {
            if (await dataContext.Countries.AnyAsync()) return;

            var countries = new List<Country>
            {
                {new Country() {CountryName = "Russia"}},
                {new Country() {CountryName = "Ukraine"}},
                {new Country() {CountryName = "France"}},
                {new Country() {CountryName = "Spain"}},
                {new Country() {CountryName = "Sweden"}},
                {new Country() {CountryName = "Norway"}},
                {new Country() {CountryName = "Germany"}},
                {new Country() {CountryName = "Finland"}},
                {new Country() {CountryName = "Poland"}},
                {new Country() {CountryName = "Italy"}},
                {new Country() {CountryName = "Great Britain"}},
                {new Country() {CountryName = "Romania"}},
                {new Country() {CountryName = "Belarus"}},
                {new Country() {CountryName = "Greece"}},
                {new Country() {CountryName = "Bulgaria"}},
                {new Country() {CountryName = "Iceland"}},
                {new Country() {CountryName = "Hungary"}},
                {new Country() {CountryName = "Portugal"}},
                {new Country() {CountryName = "USA"}},
                {new Country() {CountryName = "Canada"}},
                {new Country() {CountryName = "Australia"}},
                {new Country() {CountryName = "Lebanon"}}
            };
            
            await dataContext.Countries.AddRangeAsync(countries);
            await dataContext.SaveChangesAsync();
        }

        public static async Task SeedRoles(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager,
            IUnitOfWork unitOfWork)
        {
            string adminEmail = "admin@gmail.com";
            string moderatorEmail = "moderator@gmail.com";
            
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("user"));
            }
            if (await roleManager.FindByNameAsync("moderator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("moderator"));
            }
            if (await userManager.FindByNameAsync("admin") == null)
            {
                string password = "password";
                DateTime dateOfBirth = DateTime.Now;
                Country country = await unitOfWork.CountryRepository.GetCountryByNameAsync("Ukraine");
                AppUser admin = new AppUser()
                {
                    Email = adminEmail, UserName = "admin", DateOfBirth = dateOfBirth,
                    Country = country, FirstName = "Maksym", LastName = "Oshyiko", Gender = "male",
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    await userManager.AddToRoleAsync(admin, "moderator");
                    await userManager.AddToRoleAsync(admin, "user");
                }
            }
            if (await userManager.FindByNameAsync("moderator") == null)
            {
                string password = "password";
                DateTime dateOfBirth = DateTime.Now;
                Country country = await unitOfWork.CountryRepository.GetCountryByNameAsync("Ukraine");
                AppUser moderator = new AppUser()
                {
                    Email = moderatorEmail, UserName = "moderator", DateOfBirth = dateOfBirth,
                    Country = country, FirstName = "Moder", LastName = "Moderator", Gender = "male",
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(moderator, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(moderator, "moderator");
                    await userManager.AddToRoleAsync(moderator, "user");
                }
            }
        }

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager,
            DataContext dataContext, IUnitOfWork unitOfWork)
        {
            if (await userManager.Users.CountAsync() > 2) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null) return;
            
            foreach (var user in users)
            {
                user.Country = await unitOfWork.CountryRepository.GetCountryByNameAsync(user.Country.CountryName);
                user.EmailConfirmed = true;
                IdentityResult result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "user");
                } 
            }
            
            await dataContext.SaveChangesAsync();
        }
    }
}
















