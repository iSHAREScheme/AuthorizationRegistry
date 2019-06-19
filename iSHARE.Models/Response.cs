using System.Collections.Generic;
using System.Linq;

namespace iSHARE.Models
{
    public sealed class Response
    {
        private Response()
        {
        }
        public ResponseStatus Status { get; private set; } = ResponseStatus.Success;

        public bool Success => Status == ResponseStatus.Success;

        public IReadOnlyCollection<string> Errors { get; private set; } = new string[] { };

        public static Response ForError(string error)
            => new Response
            {
                Status = ResponseStatus.InvalidOperation,
                Errors = new[] { error }
            };

        public static Response ForErrors(IEnumerable<string> errors)
            => new Response
            {
                Status = ResponseStatus.InvalidOperation,
                Errors = errors.ToArray()
            };

        public static Response ForSuccess() => new Response();
        public static Response ForNotAuthorized(string errorMessage = "Not Authorized") => new Response
        {
            Status = ResponseStatus.NotAuthorized,
            Errors = new[] { errorMessage }
        };

        public static Response ForNotFound(string errorMessage = "Not Found") => new Response
        {
            Status = ResponseStatus.NotFound,
            Errors = new[] { errorMessage }
        };
    }

    public sealed class Response<TModel>
    {

        private Response()
        {
        }

        public ResponseStatus Status { get; private set; } = ResponseStatus.Success;

        public bool Success => Status == ResponseStatus.Success;

        public IReadOnlyCollection<string> Errors { get; private set; } = new string[] { };
        public TModel Model { get; private set; }
        public static Response<TModel> ForSuccess() => new Response<TModel>();

        public static Response<TModel> ForSuccess(TModel model) => new Response<TModel> { Model = model };

        public static implicit operator Response<TModel>(Response response) => new Response<TModel>
        {
            Errors = response.Errors,
            Status = response.Status
        };

        public static implicit operator Response<TModel>(TModel model) => new Response<TModel>
        {
            Model = model,
            Status = ResponseStatus.Success
        };
    }

    public enum ResponseStatus
    {
        Success = 1,
        NotFound = 2,
        InvalidOperation = 3,
        NotAuthorized = 4
    }
}
