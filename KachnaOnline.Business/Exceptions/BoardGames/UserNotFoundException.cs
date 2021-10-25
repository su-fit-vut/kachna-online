// UserNotFoundException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions.BoardGames
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("The specified user does not exist")
        {
        }
    }
}
