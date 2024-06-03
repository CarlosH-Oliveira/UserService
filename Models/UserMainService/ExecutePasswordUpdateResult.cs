namespace UserService.Models.UserMainService
{
    public class ExecutePasswordUpdateResult
    {
        public bool Error { get; set; } = false;
        public int StatusCode { get; set; } = 0;
        public string Message { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
    }
}
