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
                var newLetter = new Letter
                {
                    SenderName = "Андрій",
                    ReceiverName = "Олена"
                };
                context.Letters.Add(newLetter);
                context.SaveChanges();
                Console.WriteLine($"\nДодано лист, ID: {newLetter.Id}");

                var parcels = context.Parcels.Include(p => p.Branch).ToList();
                Console.WriteLine($"\nЗнайдено посилок: {parcels.Count}");
                foreach (var p in parcels)
                {
                    Console.Write($"- {p.TrackingNumber} ");
                    if (p.Branch != null) Console.Write($"(Відділення: {p.Branch.Address})");
                    Console.WriteLine();
                }
            }
        }
    }
}