using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryInvoiceApp.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult Handle(int code)
        {
            ViewBag.StatusCode = code;

            switch (code)
            {
                case 404:
                    ViewBag.Title = "Page Not Found";

                    ViewBag.Message =
                        "The requested page does not exist.";
                    break;

                case 429:
                    ViewBag.Title = "Too Many Requests";

                    ViewBag.Message =
                        "Too many attempts. Please wait and try again.";
                    break;

                default:
                    ViewBag.Title = "Something Went Wrong";

                    ViewBag.Message =
                        "An unexpected error occurred.";
                    break;
            }

            return View();
        }
    }
}