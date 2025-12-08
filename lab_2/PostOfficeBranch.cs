using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class PostOfficeBranch
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public List<BaseMailItem> MailItems { get; set; } = new List<BaseMailItem>();
    }
}
