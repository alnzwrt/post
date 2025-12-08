using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public delegate void MailProcessedHandler(IMailItem item);
    class PostOffice
    {
       public event MailProcessedHandler MailProcessed;

        public void ProcessMail(IMailItem item)
        {
            Console.WriteLine($"Почато обробку {item.Name}");
            item.PrintDetails();
            if (item is Parcel parcel)
            {
                ITrackable trackableItem = parcel;
                Console.WriteLine($"Трекінг номер: {trackableItem.GetTrackingNumber()}");

                OnMailProcessed(item);
            }
            else if (item is Letter)
            {
                Console.WriteLine("Лист відсортовано.");
            }
        }
        protected virtual void OnMailProcessed(IMailItem item)
        {
            MailProcessed?.Invoke(item);
        }
    }
}
