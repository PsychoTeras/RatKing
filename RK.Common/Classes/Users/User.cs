using RK.Common.Classes.Common;

namespace RK.Common.Classes.Users
{
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

#endregion

#region Class methods

        public override string ToString()
        {
            return string.Format("{0} ({1})", UserName, Email);
        }

#endregion

    }
}
