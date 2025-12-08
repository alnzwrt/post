using Repeat;
using System;
using System.Text;

namespace Repeat
{
    class Repeat
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Letter letter = new Letter()
            {
                Id = 258,
                SenderName = "Петро Г.",
                ReceiverName = "Роман Ш."
            };
            letter.PrintDetails();

            Parcel parcel = new Parcel()
            {
                Id = 678,
                SenderName = "Федір Я.",
                ReceiverName = "Ян Л."
            };
            parcel.PrintDetails();
            ITrackable admin = (ITrackable)parcel;
            Console.WriteLine($"Трекінг номер посилки: {admin.GetTrackingNumber()}");

            Console.WriteLine("---------");
            PostOffice postOffice = new PostOffice();
            NotificationService notifier = new NotificationService();
            postOffice.MailProcessed += notifier.Send;
            Parcel parcel_1 = new Parcel()
            {
                Id = 203,
                SenderName = "Ілля",
                ReceiverName = "Захар"
            };
            postOffice.ProcessMail(parcel_1);

            var mailContainer = new MailContainer<Parcel>();
            mailContainer.Add(parcel_1);
            mailContainer.ProcessAll();
        }
    }
}