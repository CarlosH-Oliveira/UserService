namespace UserService.Models.UserMainService
{
    public class UpdateEmailResult
    {
        public bool Error { get; set; } = false;
        public int StatusCode { get; set; } = 0;
        public string Message { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
    }
}
