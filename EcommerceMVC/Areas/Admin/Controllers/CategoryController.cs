using AutoMapper;
using EcommerceMVC.Areas.Admin.ViewModels;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("/Category/[action]")]
    public class CategoryController : Controller
    {
        private readonly EcommerceMvcContext _context;
        private readonly IMapper _mapper;
        public CategoryController(EcommerceMvcContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index([FromQuery] int p = 1)
        {
            var categories = _context.Categories.ToList();
            int totalCate = categories.Count();
            int countPage = (int)Math.Ceiling((double)totalCate / MySetting.ITEMS_PER_PAGE);
            var currentPage = p;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPage) currentPage = countPage;
            var qr = (from a in categories select a)
                .Skip((currentPage - 1) * MySetting.ITEMS_PER_PAGE).Take(MySetting.ITEMS_PER_PAGE);
            var categoryVMs = qr.Select(c=>_mapper.Map<CategoryVM>(c));

            var conditionList = new Dictionary<string, object>();
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

            var categoryListVM = new CategoryListVM()
            {
                Categories = categoryVMs,   
                PagingInfo = new PagingModel
                {
                    totalProduct = totalCate,
                    currentPage = currentPage,
                    countPage = countPage,
                    generateUrl = (int? p) => @Url.Action("Index", "ProductManagement", combineObj(p))
                }
            };
            return View(categoryListVM);
        }
        public IActionResult Update(int id)
        {

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return View("404");
            var categoryVM = _mapper.Map<CategoryVM>(category);
            return View(categoryVM);
        }
        public IActionResult Add()
        {
            return View();
        }
    }
}
