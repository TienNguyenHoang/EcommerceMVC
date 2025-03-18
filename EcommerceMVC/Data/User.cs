using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data;

public partial class User : IdentityUser
{

    public string? HomeAddress { get; set; }
    public DateTime? BirthDate { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}
