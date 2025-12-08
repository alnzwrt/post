using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class NotificationService
    {
        public void Send(IMailItem item)
        {
            Console.WriteLine("\nОтримано сповіщення про завершення обробки!");
            Console.WriteLine($"{item.Name} з #{item.Id}, готово до наступного етапу.");
        }
    }
}
