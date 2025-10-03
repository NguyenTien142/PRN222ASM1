using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Model
{
    public partial class InventoryRequest
    {
        public int RequestId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public int RequestedBy { get; set; } // UserId of DealerManager who made the request
        public int RequestedQuantity { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public int? ProcessedBy { get; set; } // UserId of Admin who processed the request
        public string? AdminComment { get; set; }

        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual Dealer Dealer { get; set; } = null!;
        public virtual User RequestedByUser { get; set; } = null!;
        public virtual User? ProcessedByUser { get; set; }
    }
}