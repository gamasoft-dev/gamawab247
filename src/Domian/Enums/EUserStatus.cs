using System.ComponentModel;

namespace Domain.Enums
{
    public enum EUserStatus
    {
        [Description("ACTIVE")]
        ACTIVE = 1,
        [Description("PENDING")]
        PENDING = 2,
        [Description("DISABLE")]
        DISABLE = 3,
    }
}
