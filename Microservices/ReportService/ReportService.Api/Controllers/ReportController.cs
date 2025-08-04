using Microsoft.AspNetCore.Mvc;

namespace ReportService.Api.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
