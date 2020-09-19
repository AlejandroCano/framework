using Newtonsoft.Json;
using Signum.Entities;
using Signum.Utilities;
using System;
using System.Linq;

namespace Signum.React.Json
{
    public static class JsonSerializerExtensions
    {
        public static object? DeserializeValue(this JsonSerializer serializer, JsonReader reader, Type valueType, object? oldValue)
        {
            if (oldValue != null)
            {
                var conv = serializer.Converters.FirstOrDefault(c => c.CanConvert(valueType));

                if (conv != null)
                    return conv.ReadJson(reader, valueType, oldValue, serializer);
            }

            if (valueType == typeof(string)) // string with valid iso datetime get converted otherwise
                return reader.Value;

            return serializer.Deserialize(reader, valueType);
        }

        public static void Assert(this JsonReader reader, JsonToken expected)
        {
            if (reader.TokenType != expected)
                throw new JsonSerializationException($"Expected '{expected}' but '{reader.TokenType}' found in '{reader.Path}'");
        }


        static readonly ThreadVariable<(PropertyRoute pr, ModifiableEntity? mod)?> currentPropertyRoute = Statics.ThreadVariable<(PropertyRoute pr, ModifiableEntity? mod)?>("jsonPropertyRoute");

        public static (PropertyRoute pr, ModifiableEntity? mod)? CurrentPropertyRouteAndEntity
        {
            get { return currentPropertyRoute.Value; }
        }

        public static IDisposable SetCurrentPropertyRouteAndEntity((PropertyRoute, ModifiableEntity?)? route)
        {
            var old = currentPropertyRoute.Value;

            currentPropertyRoute.Value = route;

            return new Disposable(() => { currentPropertyRoute.Value = old; });
        }

        static readonly ThreadVariable<bool> allowDirectMListChangesVariable = Statics.ThreadVariable<bool>("allowDirectMListChanges");

        public static bool AllowDirectMListChanges
        {
            get { return allowDirectMListChangesVariable.Value; }
        }

        public static IDisposable SetAllowDirectMListChanges(bool allowMListDirectChanges)
        {
            var old = allowDirectMListChangesVariable.Value;

            allowDirectMListChangesVariable.Value = allowMListDirectChanges;

            return new Disposable(() => { allowDirectMListChangesVariable.Value = old; });
        }
    }
}
