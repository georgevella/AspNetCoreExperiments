using System.Net.Http;

namespace Glyde.Web.Api.Client
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient(HttpClientSettings settings);
    }
}