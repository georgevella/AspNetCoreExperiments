namespace Glyde.AspNetCore.Requests
{
    public interface IRequestContext
    {
        TData Get<TData>() where TData : class, IRequestData;
    }
}