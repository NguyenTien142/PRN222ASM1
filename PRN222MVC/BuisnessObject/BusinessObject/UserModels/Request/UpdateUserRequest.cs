namespace BusinessObject.BusinessObject.UserModels.Request
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string DealerTypeName { get; set; } = string.Empty;
        public string DealerAddress { get; set; } = string.Empty;
    }
}