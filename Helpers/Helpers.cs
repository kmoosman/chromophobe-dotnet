namespace Chromophobe.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class JsonHelper
    {
        public static string SerializeWithCamelCase<T>(T value)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(options);
            return JsonSerializer.Serialize(value, options);
        }
    }

}