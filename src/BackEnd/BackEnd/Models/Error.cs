using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public enum Error
    {
        UserDoesNotExist,
        UserAlreadyExists,
        PlugDoesNotExist,
        PlugNotConnected,
        IncorrectOldPassword,
        IncorrectPassword,
        UnauthorizedOwner,
        UnauthorizedUser
    }
}
