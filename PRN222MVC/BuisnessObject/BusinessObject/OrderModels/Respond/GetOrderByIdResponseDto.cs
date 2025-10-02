namespace BusinessObject.BusinessObject.OrderModels.Respond;

public class GetOrderByIdResponseDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public List<GetOrderVehicleResponseDto> OrderVehicles { get; set; } = new();
}
public class GetOrderVehicleResponseDto
{

    public int VehicleId { get; set; }
    public string VehicleName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}