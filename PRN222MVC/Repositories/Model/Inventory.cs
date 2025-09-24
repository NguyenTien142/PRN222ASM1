using System;
using System.Collections.Generic;

namespace Repositories.Model;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int DealerId { get; set; }

    public virtual Dealer Dealer { get; set; } = null!;

    public virtual ICollection<VehicleInventory> VehicleInventories { get; set; } = new List<VehicleInventory>();
}
