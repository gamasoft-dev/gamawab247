using System.ComponentModel;

namespace Domain.Enums
{
    public enum ERole
    {
        [Description("USER")]
        USER = 1,
        [Description("ADMIN")]
        ADMIN = 2,
        [Description("SUPERADMIN")]
        SUPERADMIN = 3
    }
}
