using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{
    public class UserColl : BaseModel
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string ContentTitle { get; set; }
        public bool? Result { get; set; }
        public string RequestId { get; set; }
    }
}
