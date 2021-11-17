// UserUnprivilegedException.cs
// Author: Ondřej Ondryáš

using System;

namespace KachnaOnline.Business.Exceptions
{
    public class UserUnprivilegedException : Exception
    {
        public int UserId { get; }
        public string MissingRoleName { get; }

        public UserUnprivilegedException(int userId, string missingRoleName)
            : base($"User {userId} is not a member of role {missingRoleName} required to perform this operation.")
        {
            this.UserId = userId;
            this.MissingRoleName = missingRoleName;
        }

        public UserUnprivilegedException(int userId, string missingRoleName, string targetAction)
            : base($"User {userId} is not a member of role {missingRoleName} required to {targetAction}.")
        {
            this.UserId = userId;
            this.MissingRoleName = missingRoleName;
        }
    }
}
