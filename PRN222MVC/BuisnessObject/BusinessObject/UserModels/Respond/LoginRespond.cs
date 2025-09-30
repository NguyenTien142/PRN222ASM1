namespace BusinessObject.BusinessObject.UserModels.Respond
{
    public class LoginRespond
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public GetUserRespond? User { get; set; }
    }
}