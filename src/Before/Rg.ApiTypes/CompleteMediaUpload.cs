namespace Rg.ApiTypes
{
    public class CompleteMediaUpload
    {
        /// <summary>
        /// Short plain text title describing this image. May be null.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of this image. May be null. May include emoji and
        /// @ references.
        /// </summary>
        public string Description { get; set; }
    }
}
