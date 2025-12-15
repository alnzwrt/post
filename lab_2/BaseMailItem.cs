using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public abstract class BaseMailItem : IMailItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string SenderName { get; set; }

        public string? ReceiverName { get; set; }

        public int? BranchId { get; set; }
        public virtual PostOfficeBranch? Branch { get; set; }

        public virtual List<Tag> Tags { get; set; } = new List<Tag>();

        public abstract void PrintDetails();
    }
}