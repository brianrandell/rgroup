using System;

namespace Rg.ServiceCore.DbModel
{
    [Flags]
    public enum NotifyOptions
    {
        Email = 1,
        PushNotification = 2
    }
}
