using System.Collections.Generic;

namespace Rg.ApiTypes
{
    /// <summary>
    /// Information about a comment on a timeline entry, album, photo, etc.
    /// </summary>
    public class CommentDetails
    {
        /// <summary>
        /// Name of the user that posted this comment.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Id of the user that posted this comment.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// URL of the avatar image of the user that posted this comment.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// The message for this comment. May include emoji.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// When this comment was posted, in the form '12 minutes ago' or
        /// '1 day ago', or 'just now', etc.
        /// </summary>
        public string TimeAgo { get; set; }

        /// <summary>
        /// The URL to which to POST to like or unlike this comment.
        /// </summary>
        public string LikeUrl { get; set; }

        /// <summary>
        /// The existing likes for this comment, grouped by kind.
        /// </summary>
        public IList<LikeGroup> LikeGroups { get; set; }
    }
}
