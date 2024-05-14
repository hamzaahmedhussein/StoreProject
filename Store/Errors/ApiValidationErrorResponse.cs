namespace API.Errors
{
    public class ApiValidationErrorResponse : ApiRespose
    {
        public ApiValidationErrorResponse() : base(400)
        {
        }
        public IEnumerable<string> Errors { get; set; }
    }
}
