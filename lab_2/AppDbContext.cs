using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Repeat
{
    public class AppDbContext : DbContext
    {
        // Набори даних (Таблиці)
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

            // ЗМІНА: Використовуємо SQLite
            optionsBuilder.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Наслідування (TPH - Table Per Hierarchy)
            modelBuilder.Entity<BaseMailItem>()
                .UseTphMappingStrategy();

            // 2. Fluent API: Обмеження
            modelBuilder.Entity<BaseMailItem>()
                .Property(m => m.SenderName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<BaseMailItem>()
                .Property(m => m.ReceiverName)
                .HasDefaultValue("Невідомий");

            // Check Constraint (SQL обмеження) - SQLite це підтримує
            modelBuilder.Entity<ParcelMetadata>()
                .ToTable(t => t.HasCheckConstraint("CK_Weight_Positive", "Weight > 0"));

            // 3. Ключі
            // Alternate Key
            modelBuilder.Entity<Parcel>()
                .HasAlternateKey(p => p.TrackingNumber);

            // Composite Key
            modelBuilder.Entity<MailLog>()
                .HasKey(l => new { l.MailItemId, l.LogDate });

            // 4. Зв'язки
            // Один-до-Багатьох
            modelBuilder.Entity<PostOfficeBranch>()
                .HasMany(b => b.MailItems)
                .WithOne(m => m.Branch)
                .HasForeignKey(m => m.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Один-до-Одного
            modelBuilder.Entity<Parcel>()
                .HasOne(p => p.Metadata)
                .WithOne(m => m.Parcel)
                .HasForeignKey<ParcelMetadata>(m => m.ParcelId);

            // 5. Seeding (Початкові дані)
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