using Newtonsoft.Json.Linq;
using System;

namespace Xamla.Graph
{
    public struct SerializationFunctions
    {
        public Func<object, JToken> Serialize;
        public Func<JToken, object> Deserialize;
    }
}
