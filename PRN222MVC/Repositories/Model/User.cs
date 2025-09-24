using System;
using System.Collections.Generic;

namespace Repositories.Model;

public partial class User
{
    public int UserId { get; set; }

    public int DealerId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual Dealer Dealer { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
