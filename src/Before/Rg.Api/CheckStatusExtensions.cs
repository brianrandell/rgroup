using System.Net;
using System.Web.Http;

namespace Rg.Api
{
    internal static class CheckStatusExtensions 
    {
        public static bool IsNotSuccessful(this HttpStatusCode code)
        {
            int i = (int) code;
            return !(i >= 200 && i <= 299);
        }

        public static void ThrowHttpResponseExceptionIfNotSuccessful(this HttpStatusCode code)
        {
            if (code.IsNotSuccessful())
            {
                throw new HttpResponseException(code);
            }
        }
    }
}
