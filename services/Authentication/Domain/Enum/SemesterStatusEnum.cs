using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Authentication.Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SemesterStatusEnum
    {
        [Description("Active")]
        Active,
        [Description("Disabled")]
        Disabled,
        [Description("Pending")]
        Pending
    }
}