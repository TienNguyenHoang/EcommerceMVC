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
    [Route("/ProductManagement/[action]")]
    public class ProductManagementController : Controller
    {
        private readonly EcommerceMvcContext _context;
        private readonly IMapper _mapper;
        public ProductManagementController(EcommerceMvcContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index([FromQuery] int p = 1)
        {
            var products = _context.Products.Include(p=>p.Category).Include(p => p.Supplier).ToList();
            int totalProduct = products.Count();
            int countPage = (int)Math.Ceiling((double)totalProduct / MySetting.ITEMS_PER_PAGE);
            var currentPage = p;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPage) currentPage = countPage;
            var qr = (from a in products select a)
                .Skip((currentPage - 1) * MySetting.ITEMS_PER_PAGE).Take(MySetting.ITEMS_PER_PAGE);

            var productVMs = qr.Select(product => _mapper.Map<ProductVM>(product));
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

            var productListVM = new ProductListVM()
            {
                Products = productVMs,
                Categories = _context.Categories.ToList(),
                PagingInfo = new PagingModel
                {
                    totalProduct = totalProduct,
                    currentPage = currentPage,
                    countPage = countPage,
                    generateUrl = (int? p) => @Url.Action("Index", "ProductManagement", combineObj(p))
                }
            };
            return View(productListVM);
        }

        public IActionResult Update(int id)
        {
            
            var product = _context.Products.Include(c=>c.Category).Include(s => s.Supplier).FirstOrDefault(p=>p.ProductId == id);
            if (product == null) return View("404");
            var productVM = _mapper.Map<ProductVM>(product);
            var categories = _context.Categories.ToList();
            var suppliers = _context.Suppliers.ToList();
            var productDetailVM = new ProductDetailVM()
            {
                Product = productVM,
                Categories = categories,
                Suppliers = suppliers,
            };
            return View(productDetailVM);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var categories = _context.Categories.ToList();
            var suppliers = _context.Suppliers.ToList();
            var AddProdcutVM = new AddProductVM()
            {
                Categories = categories,
                Suppliers = suppliers
            };
            return View(AddProdcutVM);
        }
    }       
}
