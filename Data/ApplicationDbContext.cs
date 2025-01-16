using DopamineDetoxAPI.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }

        public DbSet<ContentTypeEntity> ContentTypes { get; set; }
        public DbSet<TopicEntity> Topics { get; set; }
        public DbSet<SubTopicEntity> SubTopics { get; set; }
        public DbSet<ChannelEntity> Channels { get; set; }
        public DbSet<HistorySearchResultEntity> HistorySearchResults { get; set; }
        public DbSet<SearchResultEntity> SearchResults { get; set; }
        public DbSet<NoteEntity> Notes { get; set; }
        public DbSet<TopSearchResultEntity> TopSearchResults { get; set; }
        public DbSet<SearchResultReportEntity> SearchResultReports { get; set; }
        public DbSet<WeeklySearchResultReportEntity> WeeklySearchResultReports { get; set; }
        public DbSet<WebTrace> WebTraces { get; set; }
        public DbSet<DefaultTopicEntity> DefaultTopics { get; set; }
        public DbSet<EmailTemplateEntity> EmailTemplates { get; set; }
        public DbSet<QuoteEntity> Quotes { get; set; }
        public DbSet<LearnMoreDetailsEntity> LearnMoreDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ConfigureUserRelationships(modelBuilder);
            //ConfigureTopicRelationships(modelBuilder);
            // User-Topic relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Topics)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TopicEntity>()
                .HasOne(t => t.User)
                .WithMany(u => u.Topics)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User-SubTopic relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.SubTopics)
                .WithOne(st => st.User)
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubTopicEntity>()
                .HasOne(st => st.User)
                .WithMany(u => u.SubTopics)
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Topic-SubTopic relationship
            modelBuilder.Entity<TopicEntity>()
                .HasMany(t => t.SubTopics)
                .WithOne(st => st.Topic)
                .HasForeignKey(st => st.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubTopicEntity>()
                .HasOne(st => st.Topic)
                .WithMany(t => t.SubTopics)
                .HasForeignKey(st => st.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            // User-Channel relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(c => c.Channels)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContentTypeEntity>()
                .HasMany(ct => ct.Channels)
                .WithOne(c => c.ContentType)
                .HasForeignKey(c => c.ContentTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChannelEntity>()
                .HasOne(c => c.User)
                .WithMany(c => c.Channels)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChannelEntity>()
                .HasOne(c => c.ContentType)
                .WithMany(ct => ct.Channels)
                .HasForeignKey(c => c.ContentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User-Note relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(n => n.Notes)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NoteEntity>()
                .HasOne(n => n.User)
                .WithMany(n => n.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User-TopSearchResult relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(t => t.TopSearchResults)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TopSearchResultEntity>()
                .HasOne(t => t.User)
                .WithMany(t => t.TopSearchResults)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
