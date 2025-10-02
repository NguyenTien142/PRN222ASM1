namespace BusinessObject.BusinessObject.InventoryModels.Request
{
    public class ProcessInventoryRequestModel
    {
        public int RequestId { get; set; }
        public bool IsApproved { get; set; }
        public string? AdminComment { get; set; }
    }
}