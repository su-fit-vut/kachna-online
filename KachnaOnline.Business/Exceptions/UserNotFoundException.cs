using System;

namespace KachnaOnline.Business.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public int UserId { get; }

        public UserNotFoundException(int userId) : base($"No user with ID {userId} exists.")
        {
            this.UserId = userId;
        }

        public UserNotFoundException(string message) : base(message)
        {
        }
    }
}
