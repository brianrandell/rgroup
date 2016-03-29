using Rg.ServiceCore.DbModel;
using Rg.ApiTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rg.ServiceCore.Operations
{
    public class SearchOperations
    {
        const string IndexName = "messages";

        public static Task<SearchResults> SearchAsync(
            ApplicationDbContext dbContext,
            string term)
        {
            // TBD
            return Task.FromResult(new SearchResults
            {
                AlbumMatches = new SearchResult[0],
                MediaMatches = new SearchResult[0],
                TimelineMatches = new SearchResult[0]
            });
        }

        public static Task IndexTimelineMessageAsync(
            TimelineEntry entry)
        {
            return IndexItemAsync(
                new MessageIndexEntry
                {
                    ItemId = "timeline-" + entry.TimelineEntryId,
                    Content = entry.Message.Content,
                    TimelineEntryId = entry.TimelineEntryId
                });
        }

        public static Task IndexAlbumAsync(
            MediaAlbum album)
        {
            return IndexItemAsync(
                new MessageIndexEntry
                {
                    ItemId = "album-" + album.MediaAlbumId,
                    Title = album.Title,
                    Content = album.Description,
                    MediaAlbumId = album.MediaAlbumId
                });
        }

        public static Task IndexCommentAsync(
            Comment comment,
            CommentItemIds ids)
        {
            return IndexItemAsync(
                new MessageIndexEntry
                {
                    ItemId = "comment-" + comment.CommentId,
                    Content = comment.Text.Content,
                    TimelineEntryId = ids.TimelineEntryId,
                    MediaAlbumId = ids.AlbumId,
                    UserMediaId = ids.MediaId,
                    CommentThreadId = comment.CommentThreadId
                });
        }

        // Can remove this warning once this method is implemented
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task IndexMediaAsync(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            IEnumerable<UserMedia> media)
        {
            List<UserMedia> mediaToIndex = media
                .Where(m => !string.IsNullOrWhiteSpace(m.Title) && !string.IsNullOrWhiteSpace(m.Description?.Content))
                .ToList();
            if (mediaToIndex.Count == 0)
            {
                return;
            }

            // TBD
        }

        private static Task IndexItemAsync(MessageIndexEntry entry)
        {
            // TBD
            return Task.CompletedTask;
        }

        public class MessageIndexEntry
        {
            public string ItemId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int? CommentThreadId { get; set; }
            public int? TimelineEntryId { get; set; }
            public int? MediaAlbumId { get; set; }
            public int? UserMediaId { get; set; }
        }
    }
}
