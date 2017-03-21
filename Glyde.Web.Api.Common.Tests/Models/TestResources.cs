using System;
using Glyde.Web.Api.Resources;

namespace Glyde.Web.Api.Common.Tests.Models
{
    [Resource("test")]
    public class TestResource : Resource<Guid>
    {
        public string Value { get; set; }
    }

    public class TestByConventionResource : Resource<int>
    {
        public string Value { get; set; }
    }

    [Resource("testwithversion", Version = 3)]
    public class TestWithVersioningResource : Resource<int>
    {
        public string Value { get; set; }
    }

    namespace V2
    {
        public class TestByConventionResource : Resource<int>
        {
            public string Value { get; set; }
        }

        [Resource("test")]
        public class TestResource : Resource<int>
        {
            public string Value { get; set; }
        }

        [Resource("testwithversion-ns", Version = 3)]
        public class TestWithVersioningResource : Resource<int>
        {
            public string Value { get; set; }
        }
    }
}