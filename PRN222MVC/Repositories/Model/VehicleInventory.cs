using System;
using System.Collections.Generic;

namespace Repositories.Model;

public partial class VehicleInventory
{
    public int VehicleId { get; set; }

    public int InventoryId { get; set; }

    public int Quantity { get; set; }

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
