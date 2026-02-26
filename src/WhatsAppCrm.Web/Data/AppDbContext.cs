using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WhatsAppCrm.Web.Entities;

namespace WhatsAppCrm.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignMessage> CampaignMessages => Set<CampaignMessage>();
    public DbSet<Template> Templates => Set<Template>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.Phone).IsUnique();
        });

        modelBuilder.Entity<Conversation>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasOne(c => c.Contact)
                .WithMany(c => c.Conversations)
                .HasForeignKey(c => c.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Message>(e =>
        {
            e.HasKey(m => m.Id);
            e.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Pipeline>(e =>
        {
            e.HasKey(p => p.Id);
        });

        modelBuilder.Entity<Stage>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.Pipeline)
                .WithMany(p => p.Stages)
                .HasForeignKey(s => s.PipelineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Deal>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasOne(d => d.Contact)
                .WithMany(c => c.Deals)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.Stage)
                .WithMany(s => s.Deals)
                .HasForeignKey(d => d.StageId);
        });

        modelBuilder.Entity<Campaign>(e =>
        {
            e.HasKey(c => c.Id);
        });

        modelBuilder.Entity<CampaignMessage>(e =>
        {
            e.HasKey(cm => cm.Id);
            e.HasOne(cm => cm.Campaign)
                .WithMany(c => c.Messages)
                .HasForeignKey(cm => cm.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(cm => cm.Contact)
                .WithMany(c => c.CampaignMessages)
                .HasForeignKey(cm => cm.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Template>(e =>
        {
            e.HasKey(t => t.Id);
        });

        // ============================================================
        // Match Prisma schema naming conventions:
        // - Table names: PascalCase singular (Contact, not Contacts)
        //   EF Core defaults to DbSet name (Contacts), Prisma uses model name (Contact)
        // - Column names: camelCase (contactId, lastMessageAt)
        //   EF Core defaults to PascalCase (ContactId, LastMessageAt)
        // PostgreSQL is case-sensitive with quoted identifiers.
        // ============================================================
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Table name = entity class name (singular PascalCase)
            entity.SetTableName(entity.ClrType.Name);

            // Column names = camelCase
            foreach (var property in entity.GetProperties())
            {
                var name = property.Name;
                property.SetColumnName(char.ToLowerInvariant(name[0]) + name[1..]);
            }
        }
    }
}
