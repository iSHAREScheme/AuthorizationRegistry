namespace NLIP.iShare.Models.Responses
{
    public class RequestResultModel<T> : RequestResult
    {
        public RequestResultModel()
        {

        }
        public RequestResultModel(string error) : base(error)
        {

        }
        public T Model { get; set; }
        public virtual TResult As<TResult, TModel>(TModel model) where TResult : RequestResultModel<TModel>, new()
        {
            return new TResult
            {
                Errors = Errors,
                Model = model,
                Success = Success
            };
        }
    }
}
