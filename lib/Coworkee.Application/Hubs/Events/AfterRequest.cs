namespace lib.Coworkee.Application.Hubs.Events
{
    public class AfterRequest<TRequest, TResponse> : BeforeRequest<TRequest>
    {
        public AfterRequest()
        {}

        public TResponse Response { get;}

        public AfterRequest(TRequest request, TResponse response) : base(request)
        {
            Response = response;
        }
    }
}