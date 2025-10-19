using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Authentication.Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SemesterTermEnum
    {
        [Description("Spring")]
        Spring,
        [Description("Summer")]
        Summer,
        [Description("Fall")]
        Fall,
    }
}