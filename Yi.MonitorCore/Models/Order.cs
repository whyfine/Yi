using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{

    public class OrderMain : BaseModel
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public decimal OrderValue { get; set; }
        public decimal PayValue { get; set; }
        public short PayWay { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public bool? Result { get; set; }
        public string RequestId { get; set; }
        [NotMapped]
        public List<OrderDetail> Details { get; set; }
    }

    public class OrderDetail : BaseModel
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public long ContentId { get; set; }
        public int Num { get; set; }
        public bool? Result { get; set; }
        public string RequestId { get; set; }
    }
}
