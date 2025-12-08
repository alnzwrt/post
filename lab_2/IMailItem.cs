using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
   public interface IMailItem
    {
        public string Name { get; }
        public int Id { get; }
        public  string SenderName { get; }
        public string? ReceiverName { get; }

        public void PrintDetails();
    }
    interface ITrackable
    {
        string GetTrackingNumber();
    }
}
