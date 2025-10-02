namespace BusinessObject.BusinessObject.OrderModels.Respond;

public class GetOrdersResponseDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
}