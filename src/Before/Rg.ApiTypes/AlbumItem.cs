using System.Collections.Generic;

namespace Rg.ApiTypes
{
    /// <summary>
    /// An item in an album.
    /// </summary>
    public class AlbumItem
    {
        /// <summary>
        /// Unique id of this album item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The id of the user that created this image.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The item's plain text title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A description of the item. May contain emoji.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Time when the item was created.
        /// </summary>
        public string CreatedTime { get; set; }

        /// <summary>
        /// Text describing how long ago the item was created (e.g. 20 mins, or 1 hour).
        /// </summary>
        public string CreatedTimeAgo { get; set; }

        /// <summary>
        /// The URL from which to fetch this item.
        /// </summary>
        public string MediaUrl { get; set; }

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
