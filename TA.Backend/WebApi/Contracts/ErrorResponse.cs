namespace WebApi.Contracts
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Errors { get; } = new();
        public DateTime Timestamp { get; set; }
    }
}
