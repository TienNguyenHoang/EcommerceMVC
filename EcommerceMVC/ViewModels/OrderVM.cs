using EcommerceMVC.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.ViewModels
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        public string? ReceiverName { get; set; }

        public string Address { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public int StatusId { get; set; }

        public string? Note { get; set; }

        public string? Phone { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual OrderStatus Status { get; set; } = null!;
    }
}
