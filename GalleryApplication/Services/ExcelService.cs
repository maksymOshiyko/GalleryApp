using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GalleryApplication.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ExcelService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        
        public async Task<List<ExcelUserData>> ImportUsers(IFormFile fileExcel)
        {
            List<ExcelUserData> users = new List<ExcelUserData>();
            try
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            var worksheet = workBook.Worksheet(1);
                            var rows = worksheet.RangeUsed().RowsUsed();

                            foreach (var row in rows)
                            {
                                ExcelUserData user = new ExcelUserData()
                                {
                                    Email = row.Cell(1).Value.ToString(),
                                    UserName = row.Cell(2).Value.ToString(),
                                    Password = row.Cell(3).Value.ToString(),
                                    FirstName = row.Cell(4).Value.ToString(),
                                    LastName = row.Cell(5).Value.ToString(),
                                    Country = row.Cell(6).Value.ToString(),
                                    Gender = row.Cell(7).Value.ToString(),
                                    DateOfBirth = row.Cell(8).Value.ToString()
                                };
                                bool isValid = await ValidateUserDataAsync(user);
                                if (isValid)
                                {
                                    users.Add(user);
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new List<ExcelUserData>();
            }

            return users;
        }

        private async Task<bool> ValidateUserDataAsync(ExcelUserData userData)
        {
            if (string.IsNullOrEmpty(userData.Email) || string.IsNullOrEmpty(userData.UserName) ||
                string.IsNullOrEmpty(userData.Password) || string.IsNullOrEmpty(userData.FirstName) ||
                string.IsNullOrEmpty(userData.LastName) || string.IsNullOrEmpty(userData.Country) ||
                string.IsNullOrEmpty(userData.Gender) || string.IsNullOrEmpty(userData.DateOfBirth))
                return false;
            
            if (!IsValidEmail(userData.Email)) return false;

            if (await _userManager.Users.AnyAsync(x => x.UserName == userData.UserName.ToLower()) || 
                await _userManager.Users.AnyAsync(x => x.Email == userData.Email)) return false;

            if (!(userData.UserName.Length >= 4 && userData.UserName.Length <= 25)) return false;
            
            if (!(userData.Password.Length >= 6 && userData.Password.Length <= 20)) return false;

            if (await _unitOfWork.CountryRepository.GetCountryByNameAsync(userData.Country) == null) return false;

            if (userData.Gender != "male" && userData.Gender != "female") return false;

            try
            { 
                Convert.ToDateTime(userData.DateOfBirth);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        
        private bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        public async Task<MemoryStream> ExportUsersInfo()
        {
            var users = await _unitOfWork.UserRepository.GetUsersForExport();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");

            worksheet.Cell("A" + 1).Value = "Username";
            worksheet.Cell("B" + 1).Value = "Likes";
            worksheet.Cell("C" + 1).Value = "Follows";
            worksheet.Cell("D" + 1).Value = "Following";

            int i = 2;
            foreach (var user in users)
            {
                worksheet.Cell("A" + i).Value = user.UserName;
                worksheet.Cell("B" + i).Value = user.Likes.Count();
                worksheet.Cell("C" + i).Value = user.FollowedByUsers.Count();
                worksheet.Cell("D" + i).Value = user.FollowedUsers.Count();
                i++;
            }
            
            worksheet.Columns().AdjustToContents();

            await using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream;
            }
        }
    }
}