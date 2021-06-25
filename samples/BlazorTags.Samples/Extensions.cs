using Newtonsoft.Json;
using System.IO;

namespace BlazorTags.Samples
{
    public static class Extensions
    {
        public static string FormatJson(this string @this)
        {
            using var stringReader = new StringReader(@this);
            using var stringWriter = new StringWriter();
            
            var jsonReader = new JsonTextReader(stringReader);
            
            var jsonWriter = new JsonTextWriter(stringWriter) 
            { 
                Formatting = Formatting.Indented 
            };
            jsonWriter.WriteToken(jsonReader);

            return stringWriter.ToString();            
        }
    }
}
