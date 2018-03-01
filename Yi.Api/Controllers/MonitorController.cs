using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yi.MonitorCore.Models;
using Yi.MonitorCore.Business;

namespace Yi.Api.Controllers
{
    public class MonitorController : ApiController
    {
        private static MonitorBll bll = new MonitorBll();
        [HttpPost]
        public IHttpActionResult Add(dynamic monitor)
        {
            if (monitor == null)
                return Ok(new { status = 0, msg = "data not allow" });
            try
            {
                BaseModel b = JsonConvert.DeserializeObject<BaseModel>(monitor.ToString());
                if (b != null)
                {
                    switch ((EventTypeEnum)b.EventType)
                    {
                        case EventTypeEnum.OnLoad:
                        case EventTypeEnum.OnShow:
                        case EventTypeEnum.OnUnLoad:
                        case EventTypeEnum.OnPageScroll:
                            var page = JsonConvert.DeserializeObject<Page>(monitor.ToString());
                            bll.Add<Page>(page);
                            return Ok(new { status = 1, msg = "ok" });
                        case EventTypeEnum.OnLogin:
                        case EventTypeEnum.OnRegister:
                            var user = JsonConvert.DeserializeObject<UserInfo>(monitor.ToString());
                            bll.Add<UserInfo>(user);
                            return Ok(new { status = 1, msg = "ok" });
                        case EventTypeEnum.OnAddColl:
                            var coll = JsonConvert.DeserializeObject<UserColl>(monitor.ToString());
                            bll.Add<UserColl>(coll);
                            return Ok(new { status = 1, msg = "ok" });
                        case EventTypeEnum.OnAddOrder:
                            var order = JsonConvert.DeserializeObject<OrderMain>(monitor.ToString());
                            bll.AddOrder(order);
                            return Ok(new { status = 1, msg = "ok" });
                        case EventTypeEnum.OnPayOrder:
                            var pay = JsonConvert.DeserializeObject<OrderMain>(monitor.ToString());
                            bll.Add<OrderMain>(pay);
                            return Ok(new { status = 1, msg = "ok" });
                        case EventTypeEnum.OnAddComment:
                            var comment = JsonConvert.DeserializeObject<Comment>(monitor.ToString());
                            bll.Add<Comment>(comment);
                            return Ok(new { status = 1, msg = "ok" });
                        default:
                            return Ok(new { status = 0, msg = "EventType not allow" });
                    }
                }
            }
            catch
            {
            }
            return Ok(new { status = 0, msg = "data not allow" });
        }
    }
}
