namespace NLIP.iShare.Models.Responses
{
    public class RequestResult
    {
        public RequestResult()
        {
            Success = true;
        }
        public RequestResult(string errorMessage)
        {
            Success = false;
            Errors = new[] { errorMessage };
        }

        public bool Success { get; set; }
        public string[] Errors { get; set; } = { };

        public virtual T As<T>() where T : RequestResult, new()
        {
            return new T
            {
                Success = Success,
                Errors = Errors
            };
        }
    }
}