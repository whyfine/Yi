using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
   public class Channel
    {
        public long Id { get; set; }
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public long ViewTimes { get; set; }
        //public double AverageViewTimes { get; set; }
        public int ScrollCount { get; set; }
        //public int AverageScrollCount { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
