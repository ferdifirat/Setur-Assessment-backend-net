using Microsoft.AspNetCore.Mvc;

namespace ContactService.Api.Properties
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
