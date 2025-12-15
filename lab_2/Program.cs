using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Repeat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using (var context = new AppDbContext())
            {
                bool created = context.Database.EnsureCreated();
                if (created) Console.WriteLine("Базу даних SQLite успішно створено!");
                else Console.WriteLine("База даних вже існує.");
            }

            using (var context = new AppDbContext())
            {
                if (!context.ParcelMetadatas.Any())
                {
                    var parcel = context.Parcels.FirstOrDefault();
                    if (parcel != null)
                    {
                        context.ParcelMetadatas.Add(new ParcelMetadata { ParcelId = parcel.Id, Weight = 5.5 });
                        context.SaveChanges();
                    }
                }

                LinqOperations(context);
                LoadingStrategies(context);
                NoTracking(context);
                StoredProcedures(context);
            }
        }
        
        //LINQ операції 
        public static void LinqOperations(AppDbContext context)
        {
            Console.WriteLine("LINQ операції: ");

            var senders = context.MailItems.Select(m => m.SenderName);
            var receivers = context.MailItems.Select(m => m.ReceiverName);
            var allNames = senders.Union(receivers).ToList();
            Console.WriteLine($"Всього унікальних імен (Union): {allNames.Count}");

            var uniqueSenders = context.MailItems.Select(m => m.SenderName).Distinct().ToList();
            Console.WriteLine($"Унікальних відправників: {string.Join(", ", uniqueSenders)}");

            var pureSenders = senders.Except(receivers).ToList();
            Console.WriteLine($"Тільки відправляли (Except): {string.Join(", ", pureSenders)}");

            var activeUsers = senders.Intersect(receivers).ToList();
            Console.WriteLine($"Активні користувачі (Intersect): {string.Join(", ", activeUsers)}");

            var queryJoin = context.MailItems.Join(context.Branches, mail => mail.BranchId, branch => branch.Id, (mail, branch) => new { MailName = mail.Name, BranchAddress = branch.Address });

            foreach (var item in queryJoin)
            {
                Console.WriteLine($"Join: {item.MailName} у відділенні {item.BranchAddress}");
            }

            var groupedBySender = context.MailItems.GroupBy(m => m.SenderName).Select(g => new { Sender = g.Key, Count = g.Count() }).ToList();

            foreach (var g in groupedBySender)
            {
                Console.WriteLine($"Group By: {g.Sender} відправ(ив/ла) {g.Count} посилок.");
            }

            if (context.ParcelMetadatas.Any())
            {
                var maxWeight = context.ParcelMetadatas.Max(pm => pm.Weight);
                var avgWeight = context.ParcelMetadatas.Average(pm => pm.Weight);
                var totalWeight = context.ParcelMetadatas.Sum(pm => pm.Weight);

                Console.WriteLine($"Агрегація: Макс вага: {maxWeight}, Середня: {avgWeight}, Загальна: {totalWeight}");
            }
        }

        //Стратегії завантаження
        public static void LoadingStrategies(AppDbContext context)
        {
            Console.WriteLine("\nСтратегії завантаження: ");

            var parcelsEager = context.Parcels
                .Include(p => p.Branch)
                .Include(p => p.Metadata)
                .ToList();

            Console.WriteLine("Eager: Завантажено посилок з відділеннями: " + parcelsEager.Count);
            if (parcelsEager.Any() && parcelsEager[0].Branch != null)
            {
                Console.WriteLine($"Eager details: {parcelsEager[0].Name}, Branch: {parcelsEager[0].Branch.Address}");
            }

            var letter = context.Letters.FirstOrDefault();
            if (letter != null)
            {
                context.Entry(letter).Reference(l => l.Branch).Load();

                context.Entry(letter).Collection(l => l.Tags).Load();

                Console.WriteLine($"Explicit: Лист {letter.Name} дозавантажив відділення: {letter.Branch?.Address}");
            }

            Console.WriteLine("\nLazy Loading: ");

            context.ChangeTracker.Clear();
            

            var lazyItem = context.MailItems.FirstOrDefault(m => m.BranchId != null);

            if (lazyItem != null)
            {
                Console.WriteLine($"Lazy: {lazyItem.Name}, Відділення: {lazyItem.Branch?.Address}");
            }
        }
        
        //Ті що не відслідковуются 
        public static void NoTracking(AppDbContext context)
        {
            Console.WriteLine("\nДані що не відслідковуються: ");

            context.ChangeTracker.Clear();

            var item = context.MailItems.AsNoTracking().FirstOrDefault();

            if (item != null)
            {
                Console.WriteLine($"Original Name: {item.Name}");

                item.Name = "Не відсліковуєтся";

                context.SaveChanges();

                context.MailItems.Update(item);
                //або context.Entry(item).State = EntityState.Modified;

                context.SaveChanges();
                Console.WriteLine("Зміни збережено після явного Update().");
            }
        }

        //Збереженні процедури
        public static void StoredProcedures(AppDbContext context)
        {
            Console.WriteLine("\nЗбереженні процедури: ");

            var senderName = "Петро";
            var itemsFromSql = context.MailItems.FromSqlInterpolated($"SELECT * FROM MailItems WHERE SenderName = {senderName}").ToList();

            Console.WriteLine($"Raw SQL (FromSql): Знайдено {itemsFromSql.Count} елементів від {senderName}");

            int rowsAffected = context.Database.ExecuteSqlRaw("UPDATE ParcelMetadatas SET Weight = Weight + 1");

            Console.WriteLine($"Raw SQL (ExecuteSql): Оновлено вагу для {rowsAffected} записів.");
        }

    }
}