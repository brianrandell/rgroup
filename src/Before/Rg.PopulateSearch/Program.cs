using Hyak.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Rg.ServiceCore.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Rg.PopulateSearch
{
    class Program
    {
        const string IndexName = "messages";

        static void Main(string[] args)
        {
            string searchServiceName = args[0];
            var credentials = new SearchCredentials(args[1]);
            var searchClient = new SearchServiceClient(searchServiceName, credentials);
            try
            {
                IndexDefinitionResponse getResponse = searchClient.Indexes.Get(IndexName);
                if (getResponse?.Index != null)
                {
                    Console.WriteLine("Deleting and recreating index " + IndexName);
                    searchClient.Indexes.Delete(IndexName);
                }
            }
            catch (CloudException)
            {
                // We expect this if the index does not yet exist.
            }

            IndexDefinitionResponse createIndexResponse = searchClient.Indexes.Create(new Index(
                IndexName,
                new[]
                {
                    new Field("ItemId", DataType.String) { IsKey = true },
                    new Field("Title", DataType.String) { IsSearchable = true },
                    new Field("Content", DataType.String) { IsSearchable = true },
                    new Field("CommentThreadId", DataType.Int32),
                    new Field("TimelineEntryId", DataType.Int32),
                    new Field("MediaAlbumId", DataType.Int32),
                    new Field("UserMediaId", DataType.Int32)
                }));

            Index index = createIndexResponse.Index;
            var indexClient = new SearchIndexClient(searchServiceName, IndexName, credentials);
            using (var dbContext = new ApplicationDbContext(args[2]))
            {
                IEnumerable<TimelineEntry> timelineEntries = dbContext.TimelineEntries
                    .Include(te => te.Message)
                    .Include(te => te.CommentThread.Comments.Select(c => c.Text));

                foreach (TimelineEntry entry in timelineEntries)
                {
                    var batchActions = new List<IndexAction<MessageIndexEntry>>();

                    batchActions.Add(new IndexAction<MessageIndexEntry>(
                        IndexActionType.Upload,
                        new MessageIndexEntry
                        {
                            ItemId = "timeline-" + entry.TimelineEntryId,
                            Content = entry.Message.Content,
                            TimelineEntryId = entry.TimelineEntryId
                        }));

                    if (entry.CommentThread != null)
                    {
                        foreach (Comment comment in entry.CommentThread.Comments)
                        {
                            batchActions.Add(new IndexAction<MessageIndexEntry>(
                                IndexActionType.Upload,
                                new MessageIndexEntry
                                {
                                    ItemId = "comment-" + comment.CommentId,
                                    Content = comment.Text.Content,
                                    TimelineEntryId = entry.TimelineEntryId,
                                    CommentThreadId = comment.CommentThreadId
                                }));
                        }
                    }
                    var batch = new IndexBatch<MessageIndexEntry>(batchActions);
                    DocumentIndexResponse indexDocumentsResponse = indexClient.Documents.Index(batch);
                }

                IEnumerable<MediaAlbum> albums = dbContext.MediaAlbums
                    .Include(a => a.CommentThread.Comments.Select(c => c.Text));

                foreach (MediaAlbum album in albums)
                {
                    var batchActions = new List<IndexAction<MessageIndexEntry>>();

                    batchActions.Add(new IndexAction<MessageIndexEntry>(
                        IndexActionType.Upload,
                        new MessageIndexEntry
                        {
                            ItemId = "album-" + album.MediaAlbumId,
                            Title = album.Title,
                            Content = album.Description,
                            MediaAlbumId = album.MediaAlbumId
                        }));

                    if (album.CommentThread != null)
                    {
                        foreach (Comment comment in album.CommentThread.Comments)
                        {
                            batchActions.Add(new IndexAction<MessageIndexEntry>(
                                IndexActionType.Upload,
                                new MessageIndexEntry
                                {
                                    ItemId = "comment-" + comment.CommentId,
                                    Content = comment.Text.Content,
                                    MediaAlbumId = album.MediaAlbumId,
                                    CommentThreadId = comment.CommentThreadId
                                }));
                        }
                    }
                    var batch = new IndexBatch<MessageIndexEntry>(batchActions);
                    DocumentIndexResponse indexDocumentsResponse = indexClient.Documents.Index(batch);
                }


                IEnumerable<UserMedia> medias = dbContext.UserMedias
                    .Include(m => m.Description)
                    .Include(m => m.CommentThread.Comments.Select(c => c.Text));

                foreach (UserMedia media in medias)
                {
                    var batchActions = new List<IndexAction<MessageIndexEntry>>();

                    batchActions.Add(new IndexAction<MessageIndexEntry>(
                        IndexActionType.Upload,
                        new MessageIndexEntry
                        {
                            ItemId = "media-" + media.UserMediaId,
                            Title = media.Title,
                            Content = media.Description?.Content,
                            UserMediaId = media.UserMediaId,
                            MediaAlbumId = media.MediaAlbumId
                        }));

                    if (media.CommentThread != null)
                    {
                        foreach (Comment comment in media.CommentThread.Comments)
                        {
                            batchActions.Add(new IndexAction<MessageIndexEntry>(
                                IndexActionType.Upload,
                                new MessageIndexEntry
                                {
                                    ItemId = "comment-" + comment.CommentId,
                                    Content = comment.Text.Content,
                                    UserMediaId = media.UserMediaId,
                                    MediaAlbumId = media.MediaAlbumId,
                                    CommentThreadId = comment.CommentThreadId
                                }));
                        }
                    }
                    var batch = new IndexBatch<MessageIndexEntry>(batchActions);
                    DocumentIndexResponse indexDocumentsResponse = indexClient.Documents.Index(batch);
                }
            }
        }
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
