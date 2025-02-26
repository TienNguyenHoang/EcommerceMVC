using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceMVC.Data;
using AutoMapper;
using EcommerceMVC.ViewModels;
using EcommerceMVC.Helpers;
using System.Dynamic;
using static EcommerceMVC.Helpers.MySetting;

namespace EcommerceMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceMvcContext _context;
        public ProductController(EcommerceMvcContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? category, string? query, SortOption sort = MySetting.SortOption.Az, [FromQuery] int p = 1)
        {
            var products = _context.Products.AsQueryable();
            var conditionList = new Dictionary<string, object>();
            if (category.HasValue)
            {
                products = products.Where(p => p.CategoryId == category.Value);
                conditionList["category"] = category;
            }
            if (!string.IsNullOrEmpty(query))
            {
                products = products.Where(p => p.ProductName.Contains(query));
                conditionList["query"] = query;
            }
            products = sort switch
            {
                MySetting.SortOption.Az => products.OrderBy(p => p.ProductName),
                MySetting.SortOption.Za => products.OrderByDescending(p => p.ProductName),
                MySetting.SortOption.PriceAsc => products.OrderBy(p => p.Price),
                MySetting.SortOption.PriceDesc => products.OrderByDescending(p => p.Price),
                _ => products
            };
            int totalProduct = products.Count();
            int countPage = (int)Math.Ceiling((double)totalProduct / MySetting.ITEMS_PER_PAGE);
            var currentPage = p;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPage) currentPage = countPage;
            var qr = (from a in products select a)
                .Skip((currentPage - 1) * MySetting.ITEMS_PER_PAGE).Take(MySetting.ITEMS_PER_PAGE);

            var result = qr.Select(p => new HangHoaVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price ?? 0,
                Image = p.Image ?? "",
                Description = p.Description ?? "",
                CategoryName = p.Category.CategoryName
            });
            var combineObj = new Func<int?, object>(
                (p) =>
                {
                    var obj1 = new Dictionary<string, object>
                    {
                        { "p", p }
                    };
                    var obj2 = conditionList.ToDictionary(k => k.Key, v => v.Value);
                    var mergedObject = obj1.Concat(obj2).ToDictionary(k => k.Key, v => v.Value);
                    return mergedObject;
                }
            );

            var productListVM = new ProductListVM()
            {
                Products = result,
                PagingInfo = new PagingModel {
                    totalProduct = totalProduct,
                    currentPage = currentPage, 
                    countPage = countPage, 
                    generateUrl = (int? p) => @Url.Action("Index", "Product", combineObj(p))
                }
            };
            return View(productListVM);
        }

        public IActionResult Detail(int id)
        {
            var data = _context.Products
                .Include(p => p.Category)
                .SingleOrDefault(p => p.ProductId == id);
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                ProductId = data.ProductId,
                ProductName = data.ProductName,
                Price = data.Price ?? 0,
                Description = data.Description ?? string.Empty,
                Image = data.Image ?? string.Empty,
                CategoryName = data.Category.CategoryName
            };
            return View(result);
        }
    }
}
