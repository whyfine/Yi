using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
    public class Member
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int ViewCount { get; set; }
        public long ViewTimes { get; set; }
        //public double AverageViewTimes { get; set; }
        public int CommentCount { get; set; }
        public int OrderCount { get; set; }
        public int PayCount { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalPayAmount { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
