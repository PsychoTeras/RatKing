using RK.Common.Common;
using RK.Common.Proto.Packets;

namespace RK.Common.Classes.Users
{
    public sealed class LoggedUser
    {

#region User fields

        public string UserName;

        public ShortSize ScreenRes;

#endregion

#region Ctor

        public LoggedUser(User user)
        {
            UserName = user.UserName;
        }

#endregion

#region Class methods

        public void AppendUserEnter(PUserEnter pUserEnter)
        {
            ScreenRes = pUserEnter.ScreenRes;
        }

#endregion

    }
}
