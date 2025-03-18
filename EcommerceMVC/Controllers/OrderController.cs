using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Services;
using EcommerceMVC.ViewModels;
using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static NuGet.Packaging.PackagingConstants;

namespace EcommerceMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly EcommerceMvcContext db;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public OrderController(EcommerceMvcContext context, UserManager<User> userManager, IMapper mapper)
        {
            db = context;
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = db.Orders.Include(o=>o.OrderDetails).Include(o=>o.Status).Where(order => order.UserId == customerId).ToList();
            orders.ToList().ForEach(order =>
            {
                order.OrderDetails = db.OrderDetails.Include(orderD => orderD.Product).Where(orderD => orderD.OrderId == order.OrderId).ToList();
            });
            var orderVMs = orders.Select(order => _mapper.Map<OrderVM>(order)).ToList();
            
            return View(orderVMs);
        }
        [HttpPost]
		public IActionResult Canceled(int id)
		{
            if (!ModelState.IsValid) return Redirect("/404");
            var order = db.Orders.Include(order=>order.Status).FirstOrDefault(order=>order.OrderId == id);
            if (order != null) 
            {
                order.StatusId = -1;
                db.SaveChanges();
                TempData["StatusMessage"] = "Đã hủy đơn hàng thành công!";
                return RedirectToAction(nameof(Index));
			}
			return Redirect("/404");
		}
        [HttpPost]
		public IActionResult OrderDetail(int id)
		{
			if (!ModelState.IsValid) return Redirect("/404");
			var order = db.Orders.Include(order => order.OrderDetails).Include(order => order.Status).FirstOrDefault(order => order.OrderId == id);
			if (order != null)
			{
				order.OrderDetails = db.OrderDetails.Include(orderD => orderD.Product).Where(orderD => orderD.OrderId == order.OrderId).ToList();
                var orderVM = _mapper.Map<OrderVM>(order);
				return View(orderVM);
			}
			return Redirect("/404");
		}
	}
}
