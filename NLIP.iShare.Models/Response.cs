using System.Collections.Generic;
using System.Linq;

namespace NLIP.iShare.Models
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
    }
}