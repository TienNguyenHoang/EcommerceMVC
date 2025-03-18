using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static EcommerceMVC.Helpers.MySetting;
using EcommerceMVC.Services;
using EcommerceMVC.ExtendMethods;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly EcommerceMvcContext db; 
        private readonly PaypalClient _paypalClient;
        private readonly IVnPayService _vnPayService;
        private readonly UserManager<User> _userManager;

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public CartController(EcommerceMvcContext context, UserManager<User> userManager, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            db = context;
            _userManager = userManager;
            _paypalClient = paypalClient;
            _vnPayService = vnPayService;

		}
		[Authorize]
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
		[HttpGet]
        public IActionResult IncreaseQuantity(int id)
        {
            var giohang = Cart;
            var item = giohang.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
				item.Quantity += 1;
                HttpContext.Session.Set(MySetting.CART_KEY, giohang);
                return RedirectToAction("Index");
            }
            TempData["Message"] = $"Không tìm thấy hàng hóa {id}";
            return Redirect("/404");
        }
        [HttpGet]
        public IActionResult DecreaseQuantity(int id)
        {
            var giohang = Cart;
            var item = giohang.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
				if (item.Quantity > 1)
				{
                    item.Quantity -= 1;
                    HttpContext.Session.Set(MySetting.CART_KEY, giohang);
                }	
                    return RedirectToAction("Index");
            }
            TempData["Message"] = $"Không tìm thấy hàng hóa {id}";
            return Redirect("/404");
        }
        [HttpGet]
        public IActionResult HandlerUpdateQuantity(int id, int quantity)
        {
            var giohang = Cart;
            var item = giohang.FirstOrDefault(p => p.ProductId == id);
            var product = db.Products.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                if (quantity > 1)
                {
                    item.Quantity = quantity;
                    HttpContext.Session.Set(MySetting.CART_KEY, giohang);
                }
                return RedirectToAction("Index");
            }
            TempData["Message"] = $"Không tìm thấy hàng hóa {id}";
            return Redirect("/404");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = Cart;
            if (cart.Count == 0)
            {
                return RedirectToAction("Index","Product");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }
		[Authorize]
		[HttpPost]
		public IActionResult Checkout(CheckoutVM model, string payment = "COD")
		{
            if (ModelState.IsValid)
            {
                // Thanh toán VNPay
                if (payment == "Thanh toán VNPay")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = Cart.Sum(p => p.total),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.ReceiverName} {model.Phone}",
                        FullName = model.ReceiverName,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel, model));
                }
                // Thanh toán COD
                var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var order = new Order
                {
                    UserId = customerId,
                    ReceiverName = model.ReceiverName,
                    Address = model.Address,
                    Phone = model.Phone,
                    OrderDate = DateTime.Now,
                    PaymentMethod = "Thanh toán khi nhận hàng",
                    StatusId = 0,
                    Note = model.Note ?? ""
                };

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Add(order);
                        db.SaveChanges();

                        var cthds = new List<OrderDetail>();
                        foreach (var item in Cart)
                        {
                            cthds.Add(new OrderDetail
                            {
                                OrderId = order.OrderId,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                ProductId = item.ProductId
                            });
                        }
                        db.AddRange(cthds);
                        db.SaveChanges();

                        transaction.Commit();

                        HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                        TempData["StatusMessage"] = "Đơn hàng đã được khởi tạo!";
                        return RedirectToAction("Index","Order");
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
			ViewBag.PaypalClientId = _paypalClient.ClientId;
			return View(Cart);
		}
        [Authorize]
        [HttpPost("Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder([FromBody] CheckoutVM model, CancellationToken cancellationToken)
		{
            if (!ModelState.IsValid)
            {
				var errors = ModelState
					.Where(x => x.Value.Errors.Count > 0)
					.ToDictionary(
						kvp => kvp.Key,
						kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
					);

				return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
			}
			var tongTien = VNDConverter.VNDToUSD(Cart.Sum(p => p.total)).ToString();
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
		public async Task<IActionResult> CapturePaypalOrder(string orderID, [FromBody] CheckoutVM model, CancellationToken cancellationToken)
		{
			try
			{
				var response = await _paypalClient.CaptureOrder(orderID);
				// Lưu database 
				var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var order = new Order
				{
					UserId = customerId,
					ReceiverName = model.ReceiverName,
					Address = model.Address,
					Phone = model.Phone,
					OrderDate = DateTime.Now,
					PaymentMethod = "Thanh toán Paypal",
					StatusId = 0,
					Note = model.Note ?? ""
				};
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						db.Add(order);
						db.SaveChanges();

						var cthds = new List<OrderDetail>();
						foreach (var item in Cart)
						{
							cthds.Add(new OrderDetail
							{
								OrderId = order.OrderId,
								Quantity = item.Quantity,
								Price = item.Price,
								ProductId = item.ProductId
							});
						}
						db.AddRange(cthds);
						db.SaveChanges();
						transaction.Commit();
						HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
						TempData["StatusMessage"] = "Đơn hàng đã được khởi tạo!";
						return Ok(response);
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						var error = new { ex.GetBaseException().Message };
						return BadRequest(error);
					}
				}
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
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderInfo = response.OrderDescription.Split("|");
            var order = new Order
            {
                UserId = customerId,
                ReceiverName = orderInfo[0].Trim(),
                Address = orderInfo[1].Trim(),
                Phone = orderInfo[2].Trim(),
                OrderDate = DateTime.Now,
                PaymentMethod = "Thanh toán VNPAY",
                StatusId = 0,
                Note = orderInfo[3].Trim() ?? ""
            };
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Add(order);
                    db.SaveChanges();

                    var cthds = new List<OrderDetail>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new OrderDetail
                        {
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            ProductId = item.ProductId
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();
                    transaction.Commit();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                    TempData["StatusMessage"] = "Đơn hàng đã được khởi tạo!";
                    return RedirectToAction("Index","Order");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var error = new { ex.GetBaseException().Message };
                    return RedirectToAction(nameof(PaymentFail));
                }
            }
		}
	}
}
