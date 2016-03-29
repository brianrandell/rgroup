using System.Collections.Generic;

namespace Rg.ApiTypes
{
    /// <summary>
    /// Information for an entry in the timeline.
    /// </summary>
    public class TimelineEntryDetails
    {
        /// <summary>
        /// Unique id of the user that posted this entry.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Name of the user that posted this entry.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// URL of the avatar image of the user that posted this entry.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// The message for this entry. May include emoji.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Date and time message in ISO 8601 (2015-09-16T12:58:43Z)
        /// format.
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// A list of URLs of images posted with this entry.
        /// </summary>
        public IList<string> MediaUrls { get; set; }

        /// <summary>
        /// The URL to which to POST to like or unlike this entry.
        /// </summary>
        public string LikeUrl { get; set; }

        /// <summary>
        /// The existing likes for this entry, grouped by kind.
        /// </summary>
        public IList<LikeGroup> LikeGroups { get; set; }

        /// <summary>
        /// The URL to which to POST to a comment for this entry.
        /// </summary>
        public string CommentUrl { get; set; }

        /// <summary>
        /// Comments made on this timeline item.
        /// </summary>
        public IList<CommentDetails> Comments { get; set; }
    }
}
