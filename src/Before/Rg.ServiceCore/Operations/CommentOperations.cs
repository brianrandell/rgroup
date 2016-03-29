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
    public class CommentOperations
    {
        public static IList<CommentDetails> GetComments(CommentThread commentThread)
        {
            if (commentThread == null)
            {
                return new CommentDetails[0];
            }

            return commentThread.Comments
                .OrderBy(c => c.Text.CreatedUtc)
                .Select(
                    c =>
                    new CommentDetails
                    {
                        UserName = c.User.Name,
                        UserId = c.UserId,
                        AvatarUrl = UserOperations.GetAvatarUrl(c.User),
                        Message = c.Text.Content,
                        TimeAgo = TimeOperations.GetTimeAgo(c.Text.CreatedUtc),
                        LikeUrl = $"/api/Comments/{c.CommentId}/Like",
                        LikeGroups = LikeOperations.MakeLikeGroups(c.Text.Likes)
                    })
                .ToList();
        }

        public static async Task<HttpStatusCode> AddCommentAsync<T>(
            ApplicationDbContext dbContext,
            string userId,
            CommentItemIds ids,
            Expression<Func<T, CommentThread>> commentThreadProperty,
            T commentTargetEntity,
            CommentRequest comment)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                bool succeeded = false;
                try
                {
                    var text = await TextOperations.CreateTextAsync(
                        dbContext, comment.Message);
                    dbContext.UserTexts.Add(text);

                    var commentThreadPropInfo = (PropertyInfo) ((MemberExpression) commentThreadProperty.Body).Member;
                    CommentThread commentThread = commentThreadPropInfo.GetValue(commentTargetEntity) as CommentThread;
                    if (commentThread == null)
                    {
                        commentThread = new CommentThread
                        {
                            Comments = new List<Comment>()
                        };
                        dbContext.CommentThreads.Add(commentThread);
                        commentThreadPropInfo.SetValue(commentTargetEntity, commentThread);
                    }

                    var commentEntity = new Comment
                    {
                        Text = text,
                        Thread = commentThread,
                        UserId = userId
                    };
                    dbContext.Comments.Add(commentEntity);
                    commentThread.Comments.Add(commentEntity);

                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    succeeded = true;

                    await UserOperations.NotifyMentionsAsync(
                        dbContext, "Comment", userId, text);
                    await SearchOperations.IndexCommentAsync(commentEntity, ids);

                    return HttpStatusCode.OK;
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

        public static async Task<HttpStatusCode> AddOrRemoveCommentLikeAsync(
            ApplicationDbContext dbContext,
            string userId,
            int commentId,
            LikeRequest like)
        {
            Comment commentEntity = await dbContext.Comments
                .SingleOrDefaultAsync(c => c.CommentId == commentId);
            if (commentEntity == null)
            {
                // The entry id is part of the URL, so return a 404.
                return HttpStatusCode.NotFound;
            }

            return await LikeOperations.AddOrRemoveLikeAsync(
                dbContext,
                userId,
                commentEntity.UserTextId,
                l => l.UserTextId,
                like);
        }
    }
}
