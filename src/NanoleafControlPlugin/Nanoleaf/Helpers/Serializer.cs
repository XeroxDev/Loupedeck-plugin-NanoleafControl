namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Helpers
{
    using System;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal static class Serializer
    {
        public static String Serialize<T>(T o)
        {
            var attr = o.GetType().GetCustomAttribute(typeof(JsonObjectAttribute)) as JsonObjectAttribute;

            var jv = JToken.FromObject(o);

            return new JObject(new JProperty(attr.Title, jv)).ToString();
        }
    }
}