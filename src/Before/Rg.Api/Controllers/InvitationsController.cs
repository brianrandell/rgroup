using Rg.ServiceCore.DbModel;
using Rg.ServiceCore.Operations;
using Rg.ApiTypes;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Provides access to invitations.
    /// </summary>
    public class InvitationsController : ApiControllerBase
    {
        /// <summary>
        /// Gets the issued invitations.
        /// </summary>
        /// <returns>Lists of the outstanding and accepted invitations.</returns>
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

        /// <summary>
        /// Adds a new invitation.
        /// </summary>
        /// <param name="request">Who to invite.</param>
        /// <returns>Lists of the outstanding and accepted invitations.</returns>
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

        /// <summary>
        /// Cancel an invitation.
        /// </summary>
        /// <param name="request">Who to uninvite.</param>
        /// <returns></returns>
        /// <returns>Lists of the outstanding and accepted invitations.</returns>
        public async Task<InvitationsIssued> PostUninviteAsync(InvitationRequest request)
        {
            UserInfo user = await GetUserInfoAsync();
            if (!MasterEmails.Contains(user.Email))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            return await InvitationOperations.UninviteAsync(DbContext, request);
        }

        private async Task<InvitationsIssued> GetCurrentInvitations()
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
    }
}
