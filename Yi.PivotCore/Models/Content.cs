using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
    public class Content
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public long ViewTimes { get; set; }
        //public double AverageViewTimes { get; set; }
        public int ScrollCount { get; set; }
        //public int AverageScrollCount { get; set; }
        public int Sales { get; set; }
        public int CommentCount { get; set; }
        //public int CompositeIndex { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
