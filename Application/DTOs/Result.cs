namespace Application.DTOs
{
    public class Result
    {
        public bool Succeeded { get; private set; }
        public string Message { get; private set; }

        public static Result Success() => new Result { Succeeded = true };
        public static Result Failure(string message) => new Result { Succeeded = false, Message = message };
    }

}
