using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    internal class LikeOperations
    {
        /// <summary>
        /// Groups <see cref="Like"/> entities into <see cref="LikeGroup"/>s.
        /// </summary>
        /// <param name="likeEntities">
        /// The like entries, with their <see cref="Like.User"/> populated.
        /// </param>
        /// <returns></returns>
        public static IList<LikeGroup> MakeLikeGroups(IEnumerable<Like> likeEntities)
        {
            return likeEntities.GroupBy(
                like => like.Kind,
                (key, likes) =>
                new
                {
                    Kind = key,
                    Group =
                    new LikeGroup
                    {
                        Kind = key.ToString(),
                        Users = likes
                        .Select(
                            like =>
                            new User
                            {
                                Id = like.UserId,
                                Name = like.User.Name
                            })
                        .ToList()
                    }
                })
            .OrderBy(g => g.Kind)
            .Select(g => g.Group)
            .ToList();
        }

        public static async Task<HttpStatusCode> AddOrRemoveLikeAsync(
            ApplicationDbContext dbContext,
            string userId,
            int likedId,
            Expression<Func<Like, int?>> entryIdProperty,
            LikeRequest like)
        {            
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                bool succeeded = false;
                try
                {
                    LikeKind kind;
                    switch (like.LikeKind.ToLowerInvariant())
                    {
                        case "like":
                            kind = LikeKind.Like;
                            break;

                        case "frown":
                            kind = LikeKind.Frown;
                            break;

                        case "hug":
                            kind = LikeKind.Hug;
                            break;

                        default:
                            return HttpStatusCode.BadRequest;
                    }

                    var match = (Expression<Func<Like, bool>>) Expression.Lambda(
                        Expression.Equal(
                            entryIdProperty.Body,
                            Expression.Convert(
                                Expression.Constant(likedId),
                                typeof(int?))),
                        entryIdProperty.Parameters[0]);

                    // Using First rather than Single because there are race conditions in which
                    // we can end up with multiple likes.
                    Like existingLikeEntity = await dbContext.Likes
                        .Where(le => le.Kind == kind)
                        .FirstOrDefaultAsync(match);

                    HttpStatusCode result;
                    if (like.Set)
                    {
                        if (existingLikeEntity != null)
                        {
                            // Ignore a relike
                            result = HttpStatusCode.OK;
                        }
                        var idProp = (PropertyInfo) ((MemberExpression) entryIdProperty.Body).Member;
                        result = await AddLikeAsync(dbContext, userId, idProp, likedId, kind);
                    }
                    else
                    {
                        if (existingLikeEntity != null)
                        {
                            dbContext.Likes.Remove(existingLikeEntity);
                            await dbContext.SaveChangesAsync();
                        } // Ignore a reunlike
                        result = HttpStatusCode.OK;
                    }
                    transaction.Commit();
                    succeeded = true;
                    return result;
                }
                finally
                {
                    if (!succeeded)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
        private static async Task<HttpStatusCode> AddLikeAsync(
            ApplicationDbContext dbContext,
            string userId,
            PropertyInfo likedIdProperty,
            int likedId,
            LikeKind kind)
        {
            var likeEntity = new Like
            {
                UserId = userId,
                Kind = kind
            };

            likedIdProperty.SetValue(likeEntity, likedId);


            dbContext.Likes.Add(likeEntity);

            await dbContext.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        private static async Task<HttpStatusCode> RemoveLikeAsync(
            ApplicationDbContext dbContext, string userId, int entryId, LikeKind kind)
        {
            TimelineEntry entryEntity = await dbContext.TimelineEntries
                .SingleOrDefaultAsync(te => te.TimelineEntryId == entryId);
            if (entryEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var likeEntity = new Like
            {
                UserId = userId,
                Kind = kind,
                UserTextId = entryEntity.MessageUserTextId
            };
            dbContext.Likes.Add(likeEntity);

            await dbContext.SaveChangesAsync();

            return HttpStatusCode.OK;
        }
    }
}
