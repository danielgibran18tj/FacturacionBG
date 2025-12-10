namespace API.Middleware
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ErrorDetail>? Errors { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ErrorDetail
    {
        public string? Field { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
