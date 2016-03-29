using System.Collections.Generic;

namespace Rg.ApiTypes
{
    /// <summary>
    /// Album properties and a subset of images.
    /// </summary>
    public class AlbumSummary
    {
        /// <summary>
        /// This album's unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The id of the user that owns this album.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The album's plain text title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The album's description. May contain emoji.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A textual description of how long ago this albums was
        /// last modified.
        /// </summary>
        public string LastModifiedText { get; set; }

        /// <summary>
        /// Contains up to 4 URLs of images from the album
        /// </summary>
        public IList<string> SampleMediaUrls { get; set; }

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
