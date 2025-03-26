using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FluxGuard.Core.Utilities
{
    internal class ConvertJsonElement
    {
        // Convert JsonElement to appropriate type
        public static dynamic Convert(dynamic element)
        {
            if (element is JsonElement jsonElement)
            {
                return jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => element // Return as is for unsupported types
                };
            }
            return element;
        }
    }
}
