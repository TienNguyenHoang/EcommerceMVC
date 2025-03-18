using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public double? Price { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public string SupplierId { get; set; } = null!;

    public bool Status { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
