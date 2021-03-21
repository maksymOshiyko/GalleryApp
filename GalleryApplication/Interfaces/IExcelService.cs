using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GalleryApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalleryApplication.Interfaces
{
    public interface IExcelService
    {
        Task<List<ExcelUserData>> ImportUsers(IFormFile fileExcel);
        Task<MemoryStream> ExportUsersInfo();
    }
}