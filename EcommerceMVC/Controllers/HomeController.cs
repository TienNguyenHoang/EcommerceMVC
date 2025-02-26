using EcommerceMVC.Data;
using EcommerceMVC.Models;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EcommerceMvcContext _context;

        public HomeController(ILogger<HomeController> logger, EcommerceMvcContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Take(6);
            var result = products.Select(p => new HangHoaVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price ?? 0,
                Image = p.Image ?? "",
                Description = p.Description ?? "",
                CategoryName = p.Category.CategoryName
            });
            return View(result);
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
