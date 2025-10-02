namespace BusinessObject.BusinessObject.InventoryModels.Respond
{
    public class GetInventoryRequestRespond
    {
        public int RequestId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = null!;
        public string VehicleColor { get; set; } = null!;
        public int DealerId { get; set; }
        public string DealerAddress { get; set; } = null!;
        public string DealerType { get; set; } = null!;
        public int RequestedQuantity { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = null!; // Pending, Approved, Denied
        public string RequesterUsername { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? AdminComment { get; set; }
        public string? ProcessedByAdmin { get; set; }
    }
}