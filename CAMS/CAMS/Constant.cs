using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAMS
{
    public class Constant
    {
        public enum ActivityMode : byte { User,Class, Off ,On };
        public enum NotificationFrequency : byte { None,Daily, Weekly, Monthly };
        public enum NotificationType {Disconnected,NotUsed };


    }
}