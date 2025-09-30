namespace BusinessObject.BusinessObject.UserModels.Respond
{
    public class GetDetailUserRespond
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int DealerId { get; set; }
        public string DealerTypeName { get; set; } = string.Empty;
        public string DealerAddress { get; set; } = string.Empty;
    }
}