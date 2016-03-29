using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Rg.ServiceCore.DbModel
{

    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<UserText> UserTexts { get; set; }
        public virtual DbSet<UserMedia> UserMedias { get; set; }
        public virtual DbSet<UserMediaData> UserMediaDatas { get; set; }
        public virtual DbSet<MediaAlbum> MediaAlbums { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<TimelineEntry> TimelineEntries { get; set; }
        public virtual DbSet<TimelineEntryMedia> TimelineEntryMedia { get; set; }
        public virtual DbSet<CommentThread> CommentThreads { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Invitation> Invitations { get; set; }
        public virtual DbSet<PushTriggerRegistration> PushTriggerRegistrations { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>()
                .HasMany(user => user.MentionedIn)
                .WithMany(userText => userText.MentionsUser)
                .Map(m =>
                {
                    m.ToTable("UserTextMention");
                    m.MapLeftKey("UserInfoId");
                    m.MapRightKey("UserTextId");
                });

            modelBuilder.Entity<UserMedia>()
              .HasRequired(um => um.Data)
              .WithRequiredPrincipal(umd => umd.UserMedia);


            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
