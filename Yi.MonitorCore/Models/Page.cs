using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{
    public class Page : BaseModel
    {
        public long Id { get; set; }
        public int ChannelId { get; set; }
        public string ChannelTitle { get; set; }
        public long ContentId { get; set; }
        public string ContentTitle { get; set; }
        public short ScrollHeight { get; set; }
    }
}
