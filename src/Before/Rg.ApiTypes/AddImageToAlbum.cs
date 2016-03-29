namespace Rg.ApiTypes
{
    /// <summary>
    /// Requests that an (already uploaded) image be added to an album.
    /// </summary>
    public class AddImageToAlbum
    {
        /// <summary>
        /// The plain text title to give this image.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description to give this image. May contain emoji.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The unique id of the uploaded image (as reported by the service
        /// when the uploade completed) to add.
        /// </summary>
        public int MediaId { get; set; }
    }
}
