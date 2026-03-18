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
    }
}
