using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{
    public class BaseModel
    {
        public string CookieId { get; set; }
        public string UserId { get; set; }
        public string PageId { get; set; }
        public short EventType { get; set; }
        public long MonitorTimestamp { get; set; }
    }
}
