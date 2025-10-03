namespace BusinessObject.BusinessObject.VehicleModels.Respond
{
    public class AddVehicleWithInventoryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string? Error { get; set; }
    }
}