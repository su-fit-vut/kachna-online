// NotAnEventsManagerException.cs
// Author: David Chocholatý

using System;

namespace KachnaOnline.Business.Exceptions.Events
{
    /// <summary>
    /// Thrown when a regular user requests an operation that can only be done by an events manager.
    /// </summary>
    public class NotAnEventsManagerException : Exception
    {
        public NotAnEventsManagerException() : base("You must be an events manager to do that.")
        {
        }
    }
}
