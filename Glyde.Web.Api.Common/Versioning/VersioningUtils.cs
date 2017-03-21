using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Glyde.Web.Api.Versioning
{
    public static class VersioningUtils
    {
        private static readonly Regex VersionInNamespaceRegex = new Regex("(?:V|v)(\\d{1,3})", RegexOptions.Compiled);


        public static int DetermineVersionFromNamespace(this TypeInfo typeInfo)
        {
            // get versioning scheme from namespace
            var lastNamespacePart = typeInfo.Namespace.Split('.').Last();

            if (!VersionInNamespaceRegex.IsMatch(lastNamespacePart))
                return 1;
            var match = VersionInNamespaceRegex.Match(lastNamespacePart);

            return int.Parse(match.Groups[1].Value);
        }
    }
}