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
    [Route("/Supplier/[action]")]
    public class SupplierController : Controller
    {
        private readonly EcommerceMvcContext _context;
        private readonly IMapper _mapper;
        public SupplierController(EcommerceMvcContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index([FromQuery] int p = 1)
        {
            var suppliers = _context.Suppliers.ToList();
            int totalSupplier = suppliers.Count();
            int countPage = (int)Math.Ceiling((double)totalSupplier / MySetting.ITEMS_PER_PAGE);
            var currentPage = p;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPage) currentPage = countPage;
            var qr = (from a in suppliers select a)
                .Skip((currentPage - 1) * MySetting.ITEMS_PER_PAGE).Take(MySetting.ITEMS_PER_PAGE);
            var supplierVMs = qr.Select(c => _mapper.Map<SupplierVM>(c));

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

            var supplierListVM = new SupplierListVM()
            {
                Suppliers = supplierVMs,
                PagingInfo = new PagingModel
                {
                    totalProduct = totalSupplier,
                    currentPage = currentPage,
                    countPage = countPage,
                    generateUrl = (int? p) => @Url.Action("Index", "ProductManagement", combineObj(p))
                }
            };
            return View(supplierListVM);
        }
        public IActionResult Update(string id)
        {

            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null) return View("404");
            var supplierVM = _mapper.Map<SupplierVM>(supplier);
            return View(supplierVM);
        }
        public IActionResult Add()
        {
            return View();
        }
    }
}
