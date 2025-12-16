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
        private static readonly object _monitorLock = new object();

        private static readonly Mutex _mutex = new Mutex(false, "MyUniqueMutexName");

        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private static int _globalCounter = 0;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using (var context = new AppDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Console.WriteLine("Базу даних підготовлено.\n");
            }

            Console.WriteLine("Класичні потоки (Monitor + Mutex) ===");
            RunClassicThreads();

            _globalCounter = 0;
            Console.WriteLine("\n----------------------------------------\n");

            Console.WriteLine("TPL та Async (SemaphoreSlim) ===");
            await RunTplAsync();

            _mutex.Dispose();
            _semaphoreSlim.Dispose();
            Console.WriteLine("\nРоботу завершено.");
        }

        #region Classic Threads (Monitor & Mutex)

        static void RunClassicThreads()
        {
            List<Thread> threads = new List<Thread>();
            int numberOfThreads = 5;

            for (int i = 0; i < numberOfThreads; i++)
            {
                Thread t = new Thread(WriteDataThreadWorker);
                threads.Add(t);
                t.Start();
            }

            foreach (var t in threads) t.Join();

            ReadData();
        }

        static void WriteDataThreadWorker()
        {
            string uniqueName = "";
            bool monitorLockTaken = false;

            try
            {
                Monitor.Enter(_monitorLock, ref monitorLockTaken);

                _globalCounter++;
                uniqueName = $"Letter_Thread_{_globalCounter}";
            }
            finally
            {
                if (monitorLockTaken) Monitor.Exit(_monitorLock);
            }

            using (var context = new AppDbContext())
            {
                var letter = new Letter
                {
                    Name = uniqueName,
                    SenderName = $"Sender_{Thread.CurrentThread.ManagedThreadId}",
                    BranchId = 1
                };

                _mutex.WaitOne();
                try
                {
                    Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Mutex захоплено. Зберігаю {uniqueName}...");
                    context.Letters.Add(letter);
                    context.SaveChanges();
                    Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Збережено.");
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
        }

        #endregion

        #region TPL (SemaphoreSlim)

        static async Task RunTplAsync()
        {
            List<Task> tasks = new List<Task>();
            int numberOfTasks = 5;

            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(WriteDataTaskWorkerAsync());
            }

            await Task.WhenAll(tasks);

            ReadData();
        }

        static async Task WriteDataTaskWorkerAsync()
        {
            string uniqueName = "";

            await _semaphoreSlim.WaitAsync();
            try
            {
                _globalCounter++;
                uniqueName = $"Letter_Task_{_globalCounter}";

                await Task.Delay(10);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            Console.WriteLine($"[Task {Task.CurrentId}] Отримав ім'я: {uniqueName}. Починаю запис...");

            using (var context = new AppDbContext())
            {
                var letter = new Letter
                {
                    Name = uniqueName,
                    SenderName = $"Sender_Task_{Task.CurrentId}",
                    BranchId = 1
                };

                await context.Letters.AddAsync(letter);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        static void ReadData()
        {
            using (var context = new AppDbContext())
            {
                var count = context.Letters.Count();
                Console.WriteLine($"-> У базі зараз {count} записів.");
            }
        }
    }
}