using System;
using System.Collections.Generic;

namespace Repositories.Model;

public partial class Dealer
{
    public int DealerId { get; set; }

    public int DealerTypeId { get; set; }

    public string Address { get; set; } = null!;

    public virtual DealerType DealerType { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
