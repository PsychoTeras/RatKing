using System;

namespace RK.Common.Classes.Users
{
    public class UserLogged
    {

#region Fields

        public long UserId;
        public DateTime LastLoginTime;

#endregion

#region Class methods

        public UserLogged()
        {
            LastLoginTime = DateTime.Now.ToUniversalTime();
        }

        public UserLogged(User user) : this()
        {
            if (user == null)
            {
                throw new NullReferenceException("User is null-reference");
            }
            UserId = user.Id;
        }

#endregion

    }
}
