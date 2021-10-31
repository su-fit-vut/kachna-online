// NotAuthenticatedException.cs
// Author: František Nečas

using System;

namespace KachnaOnline.Business.Exceptions
{
    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException() : base("User was not authenticated.")
        {
        }
    }
}
