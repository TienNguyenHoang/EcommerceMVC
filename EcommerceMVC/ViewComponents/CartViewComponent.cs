﻿using EcommerceMVC.ExtendMethods;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            return View("CartPanel", new CartModel()
            {
                Quantity = cart.Sum(p=>p.Quantity),
                Total = cart.Sum(p=>p.total)
            });
        }
    }
}
