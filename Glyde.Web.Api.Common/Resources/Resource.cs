namespace Glyde.Web.Api.Resources
{
    public abstract class Resource<TResourceId> : IResource
    {
        TResourceId Id { get; set; }
    }
}