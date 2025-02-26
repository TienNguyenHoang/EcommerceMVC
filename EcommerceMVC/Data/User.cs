using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Data;

public partial class User : IdentityUser
{
    [Column(TypeName = "nvarchar")]
    [StringLength(400)]
    public string? HomeAddress { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}
