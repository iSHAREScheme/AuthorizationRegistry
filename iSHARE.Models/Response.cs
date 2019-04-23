using System.Collections.Generic;
using System.Linq;

namespace iSHARE.Models
{
    public sealed class Response
    {
        private Response()
        {
        }

        public bool Success => Errors == null || !Errors.Any();

        public IReadOnlyCollection<string> Errors { get; private set; } = new string[] { };

        public static Response ForError(string error)
            => new Response { Errors = new[] { error } };

        public static Response ForErrors(IEnumerable<string> errors)
            => new Response { Errors = errors.ToArray() };

        public static Response ForSuccess() => new Response();
    }

    public sealed class Response<TModel>
    {

        private Response()
        {
        }

        public bool Success => Errors == null || !Errors.Any();

        public IReadOnlyCollection<string> Errors { get; private set; } = new string[] { };
        public TModel Model { get; private set; }

        public static Response<TModel> ForError(string error)
            => new Response<TModel> { Errors = new[] { error } };

        public static Response<TModel> ForErrors(IEnumerable<string> errors)
            => new Response<TModel> { Errors = errors.ToArray() };

        public static Response<TModel> ForSuccess()
            => new Response<TModel>();

        public static Response<TModel> ForSuccess(TModel model)
            => new Response<TModel> { Model = model };

        private const string Unauthorized = "Unauthorized";
        public static Response<TModel> ForUnauthorized() => ForError(Unauthorized);
        public bool IsAuthorized() => !Errors.Contains(Unauthorized);

        private const string NotFound = "NotFound";
        public static Response<TModel> ForNotFound() => ForError(NotFound);
        public bool IsFound() => !Errors.Contains(NotFound);
    }
}