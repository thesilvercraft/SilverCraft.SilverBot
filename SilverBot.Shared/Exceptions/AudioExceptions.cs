using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBot.Shared.Exceptions
{
    public class BotNotInVCException : Exception
    {
        public BotNotInVCException(string? message) : base(message)
        {
        }

        public BotNotInVCException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class UserNotInVCException : Exception
    {
        public UserNotInVCException(string? message) : base(message)
        {
        }

        public UserNotInVCException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class PlayerIsNullException : Exception
    {
        public PlayerIsNullException()
        {
        }

        public PlayerIsNullException(string? message) : base(message)
        {
        }

        public PlayerIsNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}