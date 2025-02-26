using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
