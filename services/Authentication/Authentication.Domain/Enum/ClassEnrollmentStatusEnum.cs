using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Authentication.Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClassEnrollmentStatusEnum
    {
        [Description("Active")]
        Active,
        [Description("Disabled")]
        Disabled,
    }
}