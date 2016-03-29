using System.Collections.Generic;

namespace Rg.ApiTypes
{
    /// <summary>
    /// Full information about an album
    /// </summary>
    public class AlbumDetail
    {
        /// <summary>
        /// The album's plain text title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The id of the user that owns this album.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// A description of the album. May contain emoji.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The contents of the album.
        /// </summary>
        public IList<AlbumItem> Items { get; set; }

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
