using System;

namespace Glyde.Web.Api.Client
{
    internal class HttpClientKey : IEquatable<HttpClientKey>
    {
        public Uri BaseUri { get; }

        public Uri ProxyUri { get; }

        public bool UseProxy { get; }

        public HttpClientKey(Uri baseUri, bool useProxy = false, Uri proxyUri = null)
        {
            BaseUri = baseUri;
            ProxyUri = proxyUri;
            UseProxy = useProxy;
        }

        public bool Equals(HttpClientKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(BaseUri, other.BaseUri) && Equals(ProxyUri, other.ProxyUri) && UseProxy == other.UseProxy;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HttpClientKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BaseUri.GetHashCode();
                hashCode = (hashCode * 397) ^ (ProxyUri != null ? ProxyUri.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ UseProxy.GetHashCode();
                return hashCode;
            }
        }
    }
}