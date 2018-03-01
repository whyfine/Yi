using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
    public class ApiInfo
    {
        public long Id { get; set; }
        public short Type { get; set; }
        public string Title { get; set; }
        public int RequestCount { get; set; }
        public long RequestTimeSum { get; set; }
        public long MinRequestTime { get; set; }
        public long MaxRequestTime { get; set; }
        public int SpeedPercentage { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
