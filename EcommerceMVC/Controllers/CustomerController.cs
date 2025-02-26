using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly EcommerceMvcContext db;
        private readonly IMapper _mapper;

        public CustomerController(EcommerceMvcContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

		[HttpPost]
		public IActionResult SignIn(RegisterVM model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var khachHang = _mapper.Map<User>(model);
					db.Add(khachHang);
					db.SaveChanges();
					return RedirectToAction("Index", "HangHoa");
				}
				catch (Exception ex)
				{
					var mess = $"{ex.Message} shh";
				}
			}
			return View();
		}
        [HttpGet]
        public IActionResult LogIn(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
		[HttpPost]
		public async Task<IActionResult> LogIn(LoginVM model, string? ReturnUrl)
		{
			//ViewBag.ReturnUrl = ReturnUrl;
   //         if (ModelState.IsValid)
   //         {
   //             var khachHang = db.User.FirstOrDefault(kh => kh.Id == model.UserName);
   //             if (khachHang == null)
   //             {
   //                 ModelState.AddModelError("Lỗi", "Sai thông tin đăng nhập");
   //             } else
   //             {
   //                 var claims = new List<Claim> {
   //                             new Claim(ClaimTypes.Email, khachHang.Email),
   //                             new Claim(ClaimTypes.Name, khachHang.UserName),
   //                             new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.Id),
   //                             new Claim(ClaimTypes.Role, "Customer")
   //                         };
   //                 var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
   //                 var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

   //                 await HttpContext.SignInAsync(claimsPrincipal);
   //                 if (Url.IsLocalUrl(ReturnUrl))
   //                 {
   //                     return Redirect(ReturnUrl);
   //                 }
   //                 else
   //                 {
   //                     return Redirect("/");
   //                 }
   //             }
   //         }    
			return View();
		}
		[Authorize]
		public IActionResult Profile()
		{
			return View();
		}
		[Authorize]
		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}
	}
}
