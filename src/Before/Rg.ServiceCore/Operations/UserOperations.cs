using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    public static class UserOperations
    {
        public static string GetInformalPosessiveName(UserInfo user)
        {
            string name = user.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }

            string[] parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return "";
            }

            return parts[0] + "’s";
        }

        public static UserSettings GetUserSettings(UserInfo user)
        {
            return new UserSettings
            {
                EmailWhenMentioned = user.MentionNotificationSettings.HasFlag(NotifyOptions.Email),
                NotifyWhenMentioned = user.MentionNotificationSettings.HasFlag(NotifyOptions.PushNotification)
            };
        }

        public static string GetPosessiveName(UserInfo user)
        {
            string name = user.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }

            return name + "’s";
        }

        public static string GetAvatarUrl(UserInfo info)
        {
            if (!info.AvatarUserMediaId.HasValue)
            {
                return "/Images/DefaultAvatar.jpg";
            }
            return $"/api/userimages/{info.Avatar.UserMediaId}.{info.Avatar.Extension}";
        }

        public static string GetBannerUrl(UserInfo info)
        {
            if (!info.BannerUserMediaId.HasValue)
            {
                return "/Images/DefaultBanner.jpg";
            }
            return $"/api/userimages/{info.Banner.UserMediaId}.{info.Banner.Extension}";
        }

        public static async Task<HttpStatusCode> SetAvatarImage(
            ApplicationDbContext dbContext,
            UserInfo user,
            SetImage request)
        {
            UserMedia media = await dbContext.UserMedias
                .SingleOrDefaultAsync(um => um.UserMediaId == request.MediaId);
            if (media == null)
            {
                return HttpStatusCode.BadRequest;
            }

            if (!media.MediaAlbumId.HasValue)
            {

                if (user.AvatarsMediaAlbumId.HasValue)
                {
                    media.MediaAlbumId = user.AvatarsMediaAlbumId;
                }
                else
                {
                    var avatarsAlbum = new MediaAlbum
                    {
                        User = user,
                        Title = "Avatar Images",
                        Description = "Images used as my avatar"
                    };
                    user.AvatarsAlbum = avatarsAlbum;
                    media.MediaAlbum = avatarsAlbum;
                }
            }

            user.Avatar = media;
            await dbContext.SaveChangesAsync();

            return HttpStatusCode.OK;
        }

        public static async Task<HttpStatusCode> SetBannerImage(
            ApplicationDbContext dbContext,
            UserInfo user,
            SetImage request)
        {
            UserMedia media = await dbContext.UserMedias
                .SingleOrDefaultAsync(um => um.UserMediaId == request.MediaId);
            if (media == null)
            {
                return HttpStatusCode.BadRequest;
            }

            if (!media.MediaAlbumId.HasValue)
            {
                if (user.BannersMediaAlbumId.HasValue)
                {
                    media.MediaAlbumId = user.BannersMediaAlbumId;
                }
                else
                {
                    var bannersAlbum = new MediaAlbum
                    {
                        User = user,
                        Title = "Banner Images",
                        Description = "Images used as my banner"
                    };
                    user.BannersAlbum = bannersAlbum;
                    media.MediaAlbum = bannersAlbum;
                }
            }

            user.Banner = media;
            await dbContext.SaveChangesAsync();

            return HttpStatusCode.OK;
        }

        public static Task<bool> CheckNewUserAllowed(
            ApplicationDbContext dbContext,
            string email)
        {
            // This is where we'd be filtering based on invitation. As it is,
            // we let in anyone with a Microsoft account at the moment.
            return Task.FromResult(true);
        }

        public static async Task NotifyMentionsAsync(
            ApplicationDbContext dbContext,
            string mentionedIn,
            string mentioningUserId,
            UserText text)
        {
            await dbContext.Entry(text).Collection(t => t.MentionsUser).LoadAsync();

            if (text.MentionsUser != null && text.MentionsUser.Count > 0)
            {
                // TBD
            }
        }
    }
}
