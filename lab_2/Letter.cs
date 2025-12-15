using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class Letter : BaseMailItem
    {
        public Letter() { Name = "Лист"; }
        public override void PrintDetails() =>
            Console.WriteLine($"Лист: {Id}, Відправник: {SenderName}");
    }
}
