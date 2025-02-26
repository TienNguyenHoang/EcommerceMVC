using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly EcommerceMvcContext db;

        public MenuLoaiViewComponent(EcommerceMvcContext context)
        {
            db = context;
        }
        public IViewComponentResult Invoke()
        {
            var data = db.Categories.Select(lo => new MenuLoaiVM
            {
                CategoryId = lo.CategoryId,CategoryName = lo.CategoryName, Quantity = lo.Products.Count
            }).OrderBy(p=>p.CategoryName);
            return View(data);
        }
    }
}
