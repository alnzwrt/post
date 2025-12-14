using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Repeat
{
    public class AppDbContext : DbContext
    {
        public DbSet<BaseMailItem> MailItems { get; set; }
        public DbSet<Letter> Letters { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<PostOfficeBranch> Branches { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ParcelMetadata> ParcelMetadatas { get; set; }
        public DbSet<MailLog> MailLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();

            optionsBuilder.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseMailItem>()
                .UseTphMappingStrategy();

            modelBuilder.Entity<BaseMailItem>()
                .Property(m => m.SenderName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<BaseMailItem>()
                .Property(m => m.ReceiverName)
                .HasDefaultValue("Невідомий");

            modelBuilder.Entity<ParcelMetadata>()
                .ToTable(t => t.HasCheckConstraint("CK_Weight_Positive", "Weight > 0"));

            modelBuilder.Entity<Parcel>()
                .HasIndex(p => p.TrackingNumber)
                .IsUnique();

            modelBuilder.Entity<MailLog>()
                .HasKey(l => new { l.MailItemId, l.LogDate });

            modelBuilder.Entity<PostOfficeBranch>()
                .HasMany(b => b.MailItems)
                .WithOne(m => m.Branch)
                .HasForeignKey(m => m.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Parcel>()
                .HasOne(p => p.Metadata)
                .WithOne(m => m.Parcel)
                .HasForeignKey<ParcelMetadata>(m => m.ParcelId);

            modelBuilder.Entity<PostOfficeBranch>().HasData(
                new PostOfficeBranch { Id = 1, Address = "Київ, вул. Хрещатик, 1" }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Label = "Терміново" },
                new Tag { Id = 2, Label = "Крихке" }
            );

            modelBuilder.Entity<Letter>().HasData(
                new Letter { Id = 1, Name = "Лист", SenderName = "Петро", ReceiverName = "Іван", BranchId = 1 }
            );

            modelBuilder.Entity<Parcel>().HasData(
                new Parcel { Id = 2, Name = "Посилка", SenderName = "Ольга", ReceiverName = "Марія", TrackingNumber = "TRACK123", BranchId = 1 }
            );
        }
    }
}