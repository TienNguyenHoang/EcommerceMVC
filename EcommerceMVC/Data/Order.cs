using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Data;

[Table("Order")]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [StringLength(50)]
    public string? ReceiverName { get; set; }

    [StringLength(60)]
    public string Address { get; set; } = null!;

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    public int StatusId { get; set; }

    [StringLength(50)]
    public string? Note { get; set; }

    [StringLength(10)]
    public string? Phone { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("StatusId")]
    [InverseProperty("Orders")]
    public virtual OrderStatus Status { get; set; } = null!;
}
