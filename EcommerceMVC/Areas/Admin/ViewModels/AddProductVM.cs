using EcommerceMVC.Data;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class AddProductVM
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Supplier> Suppliers { get; set; } = new List<Supplier>();
    }
}
