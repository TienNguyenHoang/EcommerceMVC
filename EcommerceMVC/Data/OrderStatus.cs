using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Data;

[Table("OrderStatus")]
public partial class OrderStatus
{
    [Key]
    public int OrderStatusId { get; set; }

    [StringLength(50)]
    public string OrderStatusName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
