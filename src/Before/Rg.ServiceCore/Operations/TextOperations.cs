using Rg.ServiceCore.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    public static class TextOperations
    {
        private static Regex _emailRefRe = new Regex(
            @"\B@([A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6})@\B",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static async Task<UserText> CreateTextAsync(
            ApplicationDbContext dbContext,
            string content)
        {
            var entity = new UserText
            {
                Content = content,
                CreatedUtc = DateTime.UtcNow
            };

            MatchCollection matches = _emailRefRe.Matches(content);
            if (matches.Count > 0)
            {
                entity.MentionsUser = new List<UserInfo>();

                // We typically have few enough users that it's easiest just to get
                // all of them.
                IDictionary<string, UserInfo> allUsers = (await dbContext.UserInfos
                    .ToListAsync())
                    .ToDictionary(u => u.Email);

                foreach (string match in matches.Cast<Match>().Select(m => m.Groups[1].Value).Distinct())
                {
                    UserInfo mentionedUser;
                    if (allUsers.TryGetValue(match, out mentionedUser))
                    {
                        entity.MentionsUser.Add(mentionedUser);
                    }
                }
            }

            return entity;
        }
    }
}
