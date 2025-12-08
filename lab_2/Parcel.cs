using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class Parcel : BaseMailItem, ITrackable
    {
        public Parcel() { Name = "Посилка"; }
        public string TrackingNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);

        public ParcelMetadata? Metadata { get; set; }

        public override void PrintDetails() =>
            Console.WriteLine($"Посилка: {Id}, Трек: {TrackingNumber}");

        string ITrackable.GetTrackingNumber() => TrackingNumber;
    }
}
