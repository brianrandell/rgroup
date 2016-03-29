namespace Rg.ApiTypes
{
    /// <summary>
    /// A request to add or remove a like.
    /// </summary>
    public class LikeRequest
    {
        /// <summary>
        /// One of "Like", "Frown", or "Hug".
        /// </summary>
        public string LikeKind { get; set; }

        /// <summary>
        /// True to like (or frown or hug), false to unlike (or unfrown or unhug).
        /// </summary>
        public bool Set { get; set; }
    }
}
