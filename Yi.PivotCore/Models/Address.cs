using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
    public class Address
    {
        public long Id { get; set; }
        public string ParentName { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
