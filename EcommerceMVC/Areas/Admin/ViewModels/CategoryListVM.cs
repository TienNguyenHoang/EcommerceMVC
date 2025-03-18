using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class CategoryListVM
    {
        public IEnumerable<CategoryVM> Categories = new List<CategoryVM>();
        public PagingModel PagingInfo { get; set; } = new PagingModel();
    }
}
