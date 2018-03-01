using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{
    public class Comment : BaseModel
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public short Level { get; set; }
        public string Content { get; set; }
        public bool HasImg { get; set; }
        public bool? Result { get; set; }
        public string RequestId { get; set; }
    }
}
