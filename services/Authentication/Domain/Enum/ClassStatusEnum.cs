using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Authentication.Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClassStatusEnum
    {
        [Description("Active")]
        Active,
        [Description("Pending")]
        Pending,
        [Description("Disabled")]
        Disabled,
    }
}