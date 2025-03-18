using EcommerceMVC.Data;
using EcommerceMVC.Helpers;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class ProductDetailVM
    {
        public ProductVM Product { get; set; }
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Supplier> Suppliers { get; set; } = new List<Supplier>();
    }
}
