using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repeat
{
    class Program
    {
        private static readonly object _threadLocker = new object();

        private static readonly object _taskLocker = new object();

        private static int _globalCounter = 0;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using (var context = new AppDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Console.WriteLine("Базу даних перестворено для тесту.\n");
            }

            Console.WriteLine("Класичні потоки (Thread)");
            RunClassicThreads();

            _globalCounter = 0;
            Console.WriteLine("\n----------------------------------------\n");

            Console.WriteLine("TPL та Async (Task)");
            await RunTplAsync();

            Console.WriteLine("\nРоботу завершено.");
        }

        #region Classic Threads (Class Thread)

        static void RunClassicThreads()
        {
            List<Thread> threads = new List<Thread>();
            int numberOfThreads = 5;

            Console.WriteLine("[Thread] Початок генерації даних...");

            for (int i = 0; i < numberOfThreads; i++)
            {
                Thread t = new Thread(WriteDataThreadWorker);
                threads.Add(t);
                t.Start();
            }

            foreach (var t in threads) t.Join();

            Console.WriteLine("[Thread] Генерацію завершено. Початок читання...");

            threads.Clear();
            for (int i = 0; i < 2; i++)
            {
                Thread t = new Thread(ReadDataThreadWorker);
                threads.Add(t);
                t.Start();
            }

            foreach (var t in threads) t.Join();
        }

        static void WriteDataThreadWorker()
        {
            string uniqueName;

            lock (_threadLocker)
            {
                _globalCounter++;
                uniqueName = $"Letter_Thread_{_globalCounter}";
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} згенерував: {uniqueName}");
            }

            using (var context = new AppDbContext())
            {
                var letter = new Letter
                {
                    Name = uniqueName,
                    SenderName = $"Sender_{Thread.CurrentThread.ManagedThreadId}",
                    ReceiverName = "Receiver_Thread",
                    BranchId = 1
                };

                context.Letters.Add(letter);
                context.SaveChanges(); 
            }
        }

        static void ReadDataThreadWorker()
        {
            using (var context = new AppDbContext())
            {
                // Читаємо дані (синхронно)
                var items = context.Letters.AsNoTracking().ToList();
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} прочитав {items.Count} записів.");
            }
        }

        #endregion

        #region TPL (Task + Async)

        static async Task RunTplAsync()
        {
            List<Task> tasks = new List<Task>();
            int numberOfTasks = 5;

            Console.WriteLine("[Task] Початок асинхронної генерації даних...");

            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(() => WriteDataTaskWorkerAsync()));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("[Task] Генерацію завершено. Початок асинхронного читання...");

            tasks.Clear();
            for (int i = 0; i < 2; i++)
            {
                tasks.Add(Task.Run(() => ReadDataTaskWorkerAsync()));
            }

            await Task.WhenAll(tasks);
        }

        static async Task WriteDataTaskWorkerAsync()
        {
            string uniqueName;

            lock (_taskLocker)
            {
                _globalCounter++;
                uniqueName = $"Letter_Task_{_globalCounter}";
            }

            Console.WriteLine($"Task {Task.CurrentId} підготував: {uniqueName}");

            using (var context = new AppDbContext())
            {
                var letter = new Letter
                {
                    Name = uniqueName,
                    SenderName = $"Sender_Task_{Task.CurrentId}",
                    ReceiverName = "Receiver_Task",
                    BranchId = 1
                };

                await context.Letters.AddAsync(letter);
                await context.SaveChangesAsync();
            }
        }

        static async Task ReadDataTaskWorkerAsync()
        {
            using (var context = new AppDbContext())
            {
                var items = await context.Letters.AsNoTracking().ToListAsync();
                Console.WriteLine($"Task {Task.CurrentId} асинхронно прочитав {items.Count} записів.");

                if (items.Any())
                {
                    var last = items.Last();
                    Console.WriteLine($"   -> Останній запис (Task {Task.CurrentId}): {last.Name}");
                }
            }
        }

        #endregion
    }
}