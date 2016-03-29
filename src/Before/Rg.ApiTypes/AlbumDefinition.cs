namespace Rg.ApiTypes
{
    /// <summary>
    /// Properties set when creating or modifying an album.
    /// </summary>
    public class AlbumDefinition
    {
        /// <summary>
        /// Plain text album title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Album description. May contain emoji.
        /// </summary>
        public string Description { get; set; }
    }
}
