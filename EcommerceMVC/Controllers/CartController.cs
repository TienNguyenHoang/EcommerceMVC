using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static EcommerceMVC.Helpers.MySetting;
using EcommerceMVC.Services;
using EcommerceMVC.ExtendMethods;

namespace EcommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly EcommerceMvcContext db; 
        private readonly PaypalClient _paypalClient;
        private readonly IVnPayService _vnPayService;

		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public CartController(EcommerceMvcContext context, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            db = context;
            _paypalClient = paypalClient;
            _vnPayService = vnPayService;

		}
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = Cart;
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item == null) 
            {
                var product = db.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa {id}";
                    return Redirect("/404");
                }
                item = new CartItem()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price ?? 0,
                    Quantity = quantity,
                    Image = product.Image ?? string.Empty
                };
                cart.Add(item);
            } else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("index");
        }
        public IActionResult RemoveCart(int id)
        {
            var giohang = Cart;
            var item = giohang.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                giohang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, giohang);
                return RedirectToAction("Index");
            }
            TempData["Message"] = $"Không tìm thấy hàng hóa {id}";
            return Redirect("/404");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Checkout(int id)
        {
            var cart = Cart;
            if (cart.Count == 0)
            {
                return Redirect("/");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }
		[Authorize]
		[HttpPost]
		public IActionResult Checkout(CheckoutVM model, string payment = "COD")
		{
			//if (ModelState.IsValid)
			//{
			//	if (payment == "Thanh toán VNPay")
			//	{
			//		var vnPayModel = new VnPaymentRequestModel
			//		{
			//			Amount = Cart.Sum(p => p.total),
			//			CreatedDate = DateTime.Now,
			//			Description = $"{model.Name} {model.Phone}",
			//			FullName = model.Name,
			//			OrderId = new Random().Next(1000, 100000)
			//		};
			//		return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
			//	}
			//	var customerId = HttpContext.User.Claims.SingleOrDefault(p=>p.Type == MySetting.CLAIM_CUSTOMERID).Value;
   //             var kh = new User();
   //             if (model.SameCustomer)
   //             {
   //                 kh = db.User.SingleOrDefault(kh=>kh.Id==customerId);
   //             }
   //             var hoadon = new Order
   //             {
   //                 UserId = customerId,
   //                 ReceiverName = model.Name ?? kh.UserName,
   //                 Address = model.Address ?? kh.HomeAddress,
   //                 Phone = model.Phone ?? kh.PhoneNumber,
   //                 OrderDate = DateTime.Now,
   //                 PaymentMethod = "COD",
   //                 StatusId = 0,
   //                 Note = model.Note
   //             };

			//	db.Database.BeginTransaction();
			//	try
			//	{
			//		db.Database.CommitTransaction();
			//		db.Add(hoadon);
			//		db.SaveChanges();

			//		var cthds = new List<OrderDetail>();
			//		foreach (var item in Cart)
			//		{
			//			cthds.Add(new OrderDetail
			//			{
			//				OrderId = hoadon.OrderId,
   //                         Quantity = item.Quantity,
			//				Price = item.Price,
			//				ProductId = item.ProductId
			//			});
			//		}
			//		db.AddRange(cthds);
			//		db.SaveChanges();

			//		HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

			//		return View("Success");
			//	}
			//	catch
			//	{
			//		db.Database.RollbackTransaction();
			//	}
			//}
			return View(Cart);
		}
        [Authorize]
        [HttpPost("Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var tongTien = Cart.Sum(p => p.total).ToString();
            var donvi = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
			try
			{
				var response = await _paypalClient.CreateOrder(tongTien, donvi, maDonHangThamChieu);

				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}

		[Authorize]
		[HttpPost("/Cart/capture-paypal-order")]
		public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
		{
			try
			{
				var response = await _paypalClient.CaptureOrder(orderID);

				// Lưu database đơn hàng của mình

				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}
		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}
		[Authorize]
		public IActionResult PaymentFail()
		{
			return View();
		}

		[Authorize]
		public IActionResult PaymentCallBack()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);

			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}


			// Lưu đơn hàng vô database

			TempData["Message"] = $"Thanh toán VNPay thành công";
			return RedirectToAction("PaymentSuccess");
		}
	}
}
