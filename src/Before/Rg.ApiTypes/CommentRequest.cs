namespace Rg.ApiTypes
{
    /// <summary>
    /// A request to add a like.
    /// </summary>
    public class CommentRequest
    {
        /// <summary>
        /// The message for this comment. May include emoji.
        /// </summary>
        public string Message { get; set; }
    }
}
