using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Web.Controllers.Api
{
    [Authorize]
    public class InvitationsController : ApiControllerBase
    {
        [Route("api/Invitations")]
        public async Task<InvitationsIssued> GetAsync()
        {
            UserInfo user = await GetUserInfoAsync();
            if (!MasterEmails.Contains(user.Email))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            return await InvitationOperations.GetCurrentInvitations(DbContext);
        }

        private async Task<InvitationsIssued> GetCurentInvitations()
        {
            var inv = await DbContext.Invitations
                .Select(i =>
                new
                {
                    Email = i.Email,
                    Accepted = DbContext.UserInfos.Any(u => u.Email == i.Email)
                })
                .ToListAsync();
            return new InvitationsIssued
            {
                Outstanding = inv.Where(i => !i.Accepted).Select(i => i.Email).ToList(),
                Accepted = inv.Where(i => i.Accepted).Select(i => i.Email).ToList()
            };
        }

        [Route("api/Invitations")]
        public async Task<InvitationsIssued> PostAsync(InvitationRequest request)
        {
            UserInfo user = await GetUserInfoAsync();
            if (!MasterEmails.Contains(user.Email))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            return await InvitationOperations.InviteAsync(DbContext, request);
        }

        [Route("api/invitations/delete")]
        public async Task<InvitationsIssued> PostUninviteAsync(InvitationRequest request)
        {
            UserInfo user = await GetUserInfoAsync();
            if (!MasterEmails.Contains(user.Email))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            return await InvitationOperations.UninviteAsync(DbContext, request);
        }
    }
}
