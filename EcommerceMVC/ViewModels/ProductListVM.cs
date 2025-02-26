using EcommerceMVC.Helpers;

namespace EcommerceMVC.ViewModels
{
    public class ProductListVM
    {
        public IEnumerable<HangHoaVM> Products { get; set; } = new List<HangHoaVM>();
        public PagingModel PagingInfo { get; set; } = new PagingModel();
    }
}
