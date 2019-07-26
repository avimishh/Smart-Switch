using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEnd.Models
{
    public class UsernameAlreadyInUseException : Exception
    {
        public UsernameAlreadyInUseException()
        {
        }

        public UsernameAlreadyInUseException(string message)
            : base(message)
        {
        }

        public UsernameAlreadyInUseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}