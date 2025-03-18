using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data;

public partial class Supplier
{
    public string SupplierId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
