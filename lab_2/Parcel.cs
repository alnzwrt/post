using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class Parcel : IMailItem, ITrackable
    {
        //Неявна
        public string Name { get; set; } = "Посилка";
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string TrackingNumber { get; } = Guid.NewGuid().ToString().Substring(0, 8);

        public void PrintDetails()
        {
            Console.WriteLine($"Посилка: {Id}, Відправник: {SenderName}, Отримувач: {ReceiverName}");
        }

        //Явна
        string ITrackable.GetTrackingNumber()
        {
            return TrackingNumber;
        }
    }
}
