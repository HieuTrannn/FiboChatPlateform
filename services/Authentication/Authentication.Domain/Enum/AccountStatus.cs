using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Domain.Enum
{
    public enum AccountStatus
    {
        Pending = 0,        // User registered but not yet verified (e.g., email not confirmed)
        Active = 1,         // User is active and can use the system
        Banned = 2,         // User is permanently banned from the system
        Deleted = 3,       // User account is deleted (soft delete)
    }
}
