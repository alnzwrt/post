using Repeat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class Tag
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public List<BaseMailItem> MailItems { get; set; } = new List<BaseMailItem>();
    }
}
