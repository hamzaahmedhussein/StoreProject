namespace Application.DTOs
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; } = default!;
        public List<string> Errors { get; set; } = new List<string>();
    }

}
