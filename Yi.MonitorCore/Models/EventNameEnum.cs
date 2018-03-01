using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.MonitorCore.Models
{
    public enum EventTypeEnum
    {
        OnLoad = 0,
        OnShow = 1,
        OnUnLoad = 2,
        OnPageScroll = 3,
        OnRegister = 101,
        OnLogin = 102,
        OnAddColl = 103,
        OnAddOrder = 104,
        OnPayOrder = 105,
        OnAddComment = 106,
    }
}
