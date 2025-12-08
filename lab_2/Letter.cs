using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Неявна
namespace Repeat
{
    public class Letter : IMailItem
    {
        public string Name { get; set; } = "Лист";
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public void PrintDetails()
        {
            Console.WriteLine($"Лист: {Id}, Відправник: {SenderName}, Отримувач: {ReceiverName}");
        } 
    }
}
