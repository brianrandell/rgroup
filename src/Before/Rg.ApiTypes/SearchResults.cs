using System.Collections.Generic;

namespace Rg.ApiTypes
{
    public class SearchResults
    {
        /// <summary>
        /// Timeline items where the text or one of the comments matched the search.
        /// </summary>
        public IList<SearchResult> TimelineMatches { get; set; }

        /// <summary>
        /// Albums where the title, description, or one of the comments matched the search.
        /// </summary>
        public IList<SearchResult> AlbumMatches { get; set; }

        /// <summary>
        /// Media items where the title, description, or one of the comments matched the search.
        /// </summary>
        public IList<SearchResult> MediaMatches { get; set; }
    }
}
