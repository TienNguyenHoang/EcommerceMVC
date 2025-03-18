using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
    }
}
