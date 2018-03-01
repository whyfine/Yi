using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.PivotCore.Models
{
    public class Overview
    {
        public long Id { get; set; }
        public int PV { get; set; }
        public int UV { get; set; }
        public int RequestCount { get; set; }
        public int RegisterCount { get; set; }
        public int ChannelViews { get; set; }
        public int ContentViews { get; set; }
        public int ContentSales { get; set; }
        public int CommentCount { get; set; }
        public int OrderCount { get; set; }
        public int PayCount { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalPayAmount { get; set; }
        public DateTime PivotDate { get; set; }
    }
}
