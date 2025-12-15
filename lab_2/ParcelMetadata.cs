using Repeat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    public class ParcelMetadata
    {
        [Key]
        public int Id { get; set; }
        public double Weight { get; set; }
        public int ParcelId { get; set; }
        public virtual Parcel Parcel { get; set; }
    }
}
