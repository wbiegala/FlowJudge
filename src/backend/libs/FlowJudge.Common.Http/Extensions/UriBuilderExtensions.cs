using System.Reflection;
using System.Web;

namespace FlowJudge.Common.Http.Extensions
{
    public static class UriBuilderExtensions
    {
        public static void AddQueryParams<T>(this UriBuilder builder, T queryParams)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = property.GetValue(queryParams);
                if (value is not null)
                {
                    query[property.Name] = value.ToString();
                }
            }

            builder.Query = query.ToString();
        }

        public static Uri CombineUri(string baseUri, params string[] segments)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new ArgumentException("Base URI cannot be empty.", nameof(baseUri));

            var builder = new UriBuilder(baseUri);

            var basePath = builder.Path?.Trim('/') ?? string.Empty;

            var pathSegments = segments
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .Select(segment => segment.Trim('/'));

            builder.Path = string.Join('/', new[] { basePath }.Concat(pathSegments).Where(x => x.Length > 0));

            return builder.Uri;
        }
    }
}
