using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    public static class NotificationOperations
    {

        public static Task<bool> RegisterAsync(UserInfo user, NotificationRegistration registration)
        {
            Trace.TraceInformation("RegisterAsync");
            Trace.TraceInformation($"RegisterAsync: {user.UserInfoId} ({user.Name}), {registration.Platform}: {registration.Handle}");

            // TBD
            return Task.FromResult(true);
        }

        public static Task NotifyAsync(UserInfo user, string message)
        {
            // TBD
            return Task.FromResult(true);
        }
    }
}
