using EcommerceMVC.Helpers;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class SupplierListVM
    {
        public IEnumerable<SupplierVM> Suppliers = new List<SupplierVM>();
        public PagingModel PagingInfo { get; set; } = new PagingModel();
    }
}
