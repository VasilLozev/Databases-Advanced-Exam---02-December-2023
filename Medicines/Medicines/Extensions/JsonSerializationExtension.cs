using Newtonsoft.Json;
using System.Globalization;

namespace Medicines.Extensions
{
    public static class JsonSerializationExtension
    {
        public static string SerializeJson<T>(this T obj)
        {
            var jsonSerializer = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            string result = JsonConvert.SerializeObject(obj, jsonSerializer);

            return string.Format(CultureInfo.InvariantCulture, result);
        }

        public static T DeserializeFromJson<T>(this string jsonString)
        {
            var jsonSerializer = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            T result = JsonConvert.DeserializeObject<T>(jsonString, jsonSerializer);

            return result;  
        }
    }
}
