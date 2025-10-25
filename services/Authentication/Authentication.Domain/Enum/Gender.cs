using System.Text.Json.Serialization;

namespace Authentication.Domain.Enum
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
