using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Course.Domain.Enums
{
    public class StaticEnums
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SemesterStatus
        {
            [Description("Active")]
            Active = 1, // active by admin
            [Description("Disabled")]
            Disabled = 0, // disabled by admin
            [Description("Pending")]
            Pending = 2, // waiting for admin to confirm
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SemesterTerm
        {
            [Description("Spring")]
            Spring = 1,
            [Description("Summer")]
            Summer = 2,
            [Description("Fall")]
            Fall = 3
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClassStatus
        {
            [Description("Active")]
            Active = 1, // active by lecturer
            [Description("Pending")]
            Pending = 2, // waiting for lecturer to confirm
            [Description("Disabled")]
            Disabled = 0, // disabled by admin
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TopicStatus
        {
            [Description("Active")]
            Active = 1, // active by admin
            [Description("Pending")]
            Pending = 2, // waiting for admin to confirm
            [Description("Disabled")]
            Disabled = 0, // disabled by admin
        }
    }
}