namespace BusinessObject.BusinessObject.OrderModels.Request;

public class CreateOrderRequestDto
{
    public int CustomerId { get; set; }
    public List<CreateOrderVehicleRequestDto> OrderVehicles { get; set; } = new();
}
public class CreateOrderVehicleRequestDto
{
    public int VehicleId { get; set; }
    public int Quantity { get; set; }
}