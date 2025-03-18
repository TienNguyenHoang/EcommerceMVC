using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class ProductListVM
    {
        public IEnumerable<ProductVM> Products { get; set; } = new List<ProductVM>();
        public IEnumerable<Category> Categories { get; set; }
        public PagingModel PagingInfo { get; set; } = new PagingModel();
    }
}
