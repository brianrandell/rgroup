using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    public static class InvitationOperations
    {
        public static async Task<InvitationsIssued> GetCurrentInvitations(
            ApplicationDbContext dbContext)
        {
            var inv = await dbContext.Invitations
                .Select(i =>
                new
                {
                    Email = i.Email,
                    Accepted = dbContext.UserInfos.Any(u => u.Email == i.Email)
                })
                .ToListAsync();
            return new InvitationsIssued
            {
                Outstanding = inv.Where(i => !i.Accepted).Select(i => i.Email).ToList(),
                Accepted = inv.Where(i => i.Accepted).Select(i => i.Email).ToList()
            };
        }

        public static async Task<InvitationsIssued> InviteAsync(
            ApplicationDbContext dbContext,
            InvitationRequest request)
        {
            using (var tx = dbContext.Database.BeginTransaction())
            {
                bool succeeded = false;
                try
                {
                    InvitationsIssued current = await GetCurrentInvitations(dbContext);

                    if (!current.Accepted.Contains(request.Email) &&
                        !current.Outstanding.Contains(request.Email))
                    {
                        dbContext.Invitations.Add(new Invitation
                        {
                            Email = request.Email
                        });
                        await dbContext.SaveChangesAsync();
                        current.Outstanding.Add(request.Email);
                    }
                    tx.Commit();
                    succeeded = true;

                    return current;
                }
                finally
                {
                    if (!succeeded)
                    {
                        tx.Rollback();
                    }
                }
            }
        }

        public static async Task<InvitationsIssued> UninviteAsync(
            ApplicationDbContext dbContext,
            InvitationRequest request)
        {
            using (var tx = dbContext.Database.BeginTransaction())
            {
                bool succeeded = false;
                try
                {
                    Invitation existing = await dbContext.Invitations.SingleOrDefaultAsync(e => e.Email == request.Email);
                    if (existing != null)
                    {
                        dbContext.Invitations.Remove(existing);
                        await dbContext.SaveChangesAsync();
                    }

                    InvitationsIssued current = await GetCurrentInvitations(dbContext);
                    tx.Commit();
                    succeeded = true;

                    return current;
                }
                finally
                {
                    if (!succeeded)
                    {
                        tx.Rollback();
                    }
                }
            }
        }
    }
}
