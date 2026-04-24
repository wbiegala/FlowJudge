namespace FlowJudge.Common.Utils.Serialization
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        public static T FromJson<T>(this string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}
