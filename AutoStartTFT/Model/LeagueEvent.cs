using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoStartTFT.Model
{
    public class LeagueEvent
    {
        /// <summary>
        /// The event's data.
        /// </summary>
        [JsonPropertyName("data")]
        public JsonDocument Data { get; set; }

        /// <summary>
        /// The event's type.
        /// </summary>
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// The event's uri.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        public override string ToString()
        {
            using var stream = new MemoryStream();
            string dataJson = null;
            if (Data != null)
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                {
                    Indented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                Data.WriteTo(writer);
                writer.Flush();
                dataJson = Encoding.UTF8.GetString(stream.ToArray());
            }
            return $"EventType:{EventType} Uri:{Uri} Data:{dataJson}";
        }
    }
}
