using GamingAssistant.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamingAssistant.Infrastructure.Persistence
{
    public class SamiraDbContext : DbContext
    {
        public SamiraDbContext(
            DbContextOptions<SamiraDbContext> options)
            : base(options)
        {
        }

        public DbSet<ConversationMessageEntity> ConversationMessages => Set<ConversationMessageEntity>();
        public DbSet<ConversationEntity> Conversations => Set<ConversationEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConversationMessageEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Content)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();
            });

            modelBuilder.Entity<ConversationEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.HasMany(x => x.Messages)
                    .WithOne(x => x.Conversation)
                    .HasForeignKey(x => x.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}