using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Areas.Admin.ViewModels
{
    public class SupplierVM
    {
        public string SupplierId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
