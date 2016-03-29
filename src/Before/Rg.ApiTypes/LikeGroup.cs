using System.Collections.Generic;

namespace Rg.ApiTypes
{
    /// <summary>
    /// The users who 'liked' an item in a particular way.
    /// </summary>
    public class LikeGroup
    {
        /// <summary>
        /// The 'like' kind ('Like', 'Hug', or 'Frown').
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// The users who 'liked' the item in the way specified in <see cref="Kind"/>.
        /// </summary>
        public IList<User> Users { get; set; }
    }
}
