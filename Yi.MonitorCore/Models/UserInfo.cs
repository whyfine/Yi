using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{
    public class UserInfo : BaseModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public bool? Result { get; set; }
        public string RequestId { get; set; }
    }
}
