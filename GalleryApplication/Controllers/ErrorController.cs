using Microsoft.AspNetCore.Mvc;

namespace GalleryApplication.Controllers
{
    public class ErrorController : Controller
    {
        // GET
        public IActionResult Index(string error)
        {
            return View(error);
        }
    }
}