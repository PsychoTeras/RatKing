using System;
using RK.Common.Common;

namespace RK.Common.Classes.Users
{
    [Serializable]
    public class User : DbObject
    {

#region Public fields

        public string UserName;
        public string Password;
        public string Email;

#endregion

#region Ctor

        private User() { }

        public User(string userName, string password, string email = "")
        {
            SetNewId();
            UserName = userName ?? string.Empty;
            Password = password ?? string.Empty;
            Email = email ?? string.Empty;
        }

        public static User Create()
        {
            return new User(string.Empty, string.Empty, string.Empty);
        }

#endregion

#region Class methods

        public override string ToString()
        {
            return string.Format("{0} ({1})", UserName, Email);
        }

#endregion

    }
}
