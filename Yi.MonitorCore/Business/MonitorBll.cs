using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.MonitorCore.Models;
using Yi.MonitorCore.DataAccess;
using System.Data.Entity;
using Yi.PivotCore.Models;

namespace Yi.MonitorCore.Business
{
    public class MonitorBll
    {
        public void Add<T>(T m) where T : class,new()
        {
            Task.Run(() =>
            {
                using (var db = new MonitorContext())
                {
                    db.Insert<T>(m);
                }
            });
        }

        public void AddOrder(OrderMain order)
        {
            Task.Run(() =>
            {
                using (var db = new MonitorContext())
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Insert<OrderMain>(order);
                            foreach (var item in order.Details)
                            {
                                item.CookieId = order.CookieId;
                                item.UserId = order.UserId;
                                item.PageId = order.PageId;
                                item.EventType = order.EventType;
                                item.MonitorTimestamp = order.MonitorTimestamp;
                                item.OrderId = order.OrderId;
                                item.Result = order.Result;
                                item.RequestId = order.RequestId;
                                db.Insert<OrderDetail>(item);
                            }
                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            });
        }

        #region pivot
        public Overview GetPivotOverview(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            using (var db = new MonitorContext())
            {
                var today = db.Pages.Where(v => v.EventType.Equals((short)EventTypeEnum.OnShow) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp);
                var pv = today.GroupBy(v => v.CookieId).Count();
                var uv = today.GroupBy(v => v.UserId).Count();
                var requestCount = db.Comments.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).Count() + db.OrderMains.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).Count() + db.Pages.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).Count() + db.UserColls.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).Count() + db.UserInfos.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).Count();
                var registerCount = db.UserInfos.Where(v => v.EventType.Equals((short)EventTypeEnum.OnRegister) && v.Result.HasValue && v.Result.Value).Count();
                var channelViews = db.Pages.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.EventType.Equals((short)EventTypeEnum.OnShow) && v.ContentId.Equals(0)).Count();
                var contentViews = db.Pages.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.EventType.Equals((short)EventTypeEnum.OnShow) && !v.ContentId.Equals(0)).Count();
                var contentSales = db.OrderDetails.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.Result.HasValue && v.Result.Value).Sum(v => (int?)v.Num);
                var commentCount = db.Comments.Where(v => v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.Result.HasValue && v.Result.Value).Count();
                var orderCount = db.OrderMains.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.Result.HasValue && v.Result.Value).Count();
                var payCount = db.OrderMains.Where(v => v.EventType.Equals((short)EventTypeEnum.OnPayOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.Result.HasValue && v.Result.Value).Count();
                var totalOrderAmount = db.OrderMains.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.Result.HasValue && v.Result.Value).Sum(v => (decimal?)v.OrderValue);
                var totalPayAmount = db.OrderMains.Where(v => v.EventType.Equals((short)EventTypeEnum.OnPayOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp && v.Result.HasValue && v.Result.Value).Sum(v => (decimal?)v.PayValue);
                var overview = new Overview() { PV = pv, UV = uv, RequestCount = requestCount, RegisterCount = registerCount, ChannelViews = channelViews, ContentViews = contentViews, ContentSales = contentSales.HasValue ? contentSales.Value : 0, CommentCount = commentCount, OrderCount = orderCount, PayCount = payCount, TotalOrderAmount = totalOrderAmount.HasValue ? totalOrderAmount.Value : 0, TotalPayAmount = totalPayAmount.HasValue ? totalPayAmount.Value : 0, PivotDate = startTime };

                return overview;
            }
        }

        public List<Member> GetPivotMember(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            var eventArr = new short[] { (short)EventTypeEnum.OnShow, (short)EventTypeEnum.OnUnLoad };
            using (var db = new MonitorContext())
            {
                var user = db.Pages
                    .Where(v => eventArr.Contains(v.EventType) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp)
                    .GroupBy(v => new { v.UserId, v.PageId })
                    .Select(v => new { UserId = v.Key.UserId, ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1 })
                    .GroupBy(v => v.UserId)
                    .Select(v => new
                    {
                        UserId = v.Key,
                        UserName = db.UserInfos.Where(u => u.UserId.Equals(v.Key) && u.UserName != null).OrderByDescending(u => u.MonitorTimestamp).FirstOrDefault().UserName,
                        ViewTimes = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                        ViewCount = v.Sum(k => (int?)k.ViewCount),
                        CommentCount = db.Comments.Count(k => k.UserId.Equals(v.Key) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp && k.EventType.Equals((short)EventTypeEnum.OnAddComment) && k.Result.HasValue && k.Result.Value),
                        OrderCount = db.OrderMains.Count(k => k.UserId.Equals(v.Key) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp && k.EventType.Equals((short)EventTypeEnum.OnAddOrder) && k.Result.HasValue && k.Result.Value),
                        PayCount = db.OrderMains.Count(k => k.UserId.Equals(v.Key) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp && k.EventType.Equals((short)EventTypeEnum.OnPayOrder) && k.Result.HasValue && k.Result.Value),
                        TotalOrderAmount = db.OrderMains.Where(k => k.UserId.Equals(v.Key) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp && k.EventType.Equals((short)EventTypeEnum.OnAddOrder) && k.Result.HasValue && k.Result.Value).Sum(k => (decimal?)k.OrderValue),
                        TotalPayAmount = db.OrderMains.Where(k => k.UserId.Equals(v.Key) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp && k.EventType.Equals((short)EventTypeEnum.OnPayOrder) && k.Result.HasValue && k.Result.Value).Sum(k => (decimal?)k.PayValue)
                    }).Select(v => new Member() { UserId = v.UserId, UserName = v.UserName, ViewCount = v.ViewCount.HasValue ? v.ViewCount.Value : 0, ViewTimes = v.ViewTimes.HasValue ? v.ViewTimes.Value : 0, CommentCount = v.CommentCount, OrderCount = v.OrderCount, PayCount = v.PayCount, TotalOrderAmount = v.TotalOrderAmount.HasValue ? v.TotalOrderAmount.Value : 0m, TotalPayAmount = v.TotalPayAmount.HasValue ? v.TotalPayAmount.Value : 0m, PivotDate = startTime }).ToList();
                return user;
            }
        }

        public List<Address> GetPivoAddress(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            using (var db = new MonitorContext())
            {
                var today = db.OrderMains
                    .Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddOrder) && !v.Result.HasValue && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp);
                var address = today
                    .GroupBy(v => v.Province)
                    .Where(v => !string.IsNullOrEmpty(v.Key))
                    .Select(v => new { ParentName = "", Name = v.Key, Count = v.Count() })
                    .Union(today
                        .GroupBy(v => new { v.Province, v.City })
                        .Where(v => !string.IsNullOrEmpty(v.Key.City))
                        .Select(v => new { ParentName = v.Key.Province, Name = v.Key.City, Count = v.Count() }))
                    .Union(today
                        .GroupBy(v => new { v.Province, v.City, v.Area })
                        .Where(v => !string.IsNullOrEmpty(v.Key.Area))
                        .Select(v => new { ParentName = v.Key.Province + "|" + v.Key.City, Name = v.Key.Area, Count = v.Count() })).Select(v => new Address() { Name = v.Name, ParentName = v.ParentName, Count = v.Count, PivotDate = startTime }).ToList();
                return address;
            }
        }

        public List<Channel> GetPivotChannel(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            var eventArr = new short[] { (short)EventTypeEnum.OnShow, (short)EventTypeEnum.OnUnLoad };
            using (var db = new MonitorContext())
            {
                var channel = db.Pages
                   .Where(v => eventArr.Contains(v.EventType) && v.ContentId.Equals(0) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp)
                   .GroupBy(v => new { v.ChannelId, v.PageId })
                   .Select(v => new { ChannelId = v.Key.ChannelId, ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1 })
                   .GroupBy(v => v.ChannelId)
                   .Select(v => new
                   {
                       ChannelId = v.Key,
                       Title = db.Pages.Where(p => p.ChannelId.Equals(v.Key) && p.ChannelTitle != null).OrderByDescending(p => p.MonitorTimestamp).FirstOrDefault().ChannelTitle,
                       ViewTimes = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                       ViewCount = v.Sum(k => (int?)k.ViewCount),
                       ScrollCount = db.Pages.Count(k => k.ChannelId.Equals(v.Key) && k.EventType.Equals((short)EventTypeEnum.OnPageScroll) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp)
                   }).Select(v => new Channel() { ChannelId = v.ChannelId, Title = v.Title, ViewCount = v.ViewCount.HasValue ? v.ViewCount.Value : 0, ViewTimes = v.ViewTimes.HasValue ? v.ViewTimes.Value : 0, ScrollCount = v.ScrollCount, PivotDate = startTime }).ToList();
                return channel;
            }
        }

        public List<Content> GetPivotContent(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            var eventArr = new short[] { (short)EventTypeEnum.OnShow, (short)EventTypeEnum.OnUnLoad };
            using (var db = new MonitorContext())
            {
                var content = db.Pages
                    .Where(v => eventArr.Contains(v.EventType) && !v.ContentId.Equals(0) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp)
                    .GroupBy(v => new { v.ContentId, v.PageId })
                    .Select(v => new { ContentId = v.Key.ContentId, ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1 })
                    .GroupBy(v => v.ContentId)
                    .Select(v => new
                    {
                        ContentId = v.Key,
                        Title = db.Pages.Where(p => p.ContentId.Equals(v.Key) && p.ContentTitle != null).OrderByDescending(p => p.MonitorTimestamp).FirstOrDefault().ContentTitle,
                        ViewTimes = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                        ViewCount = v.Sum(k => (int?)k.ViewCount),
                        ScrollCount = db.Pages.Count(k => k.ContentId.Equals(v.Key) && k.EventType.Equals((short)EventTypeEnum.OnPageScroll) && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp),
                        Sales = db.OrderDetails.Where(k => k.ContentId.Equals(v.Key) && k.Result.HasValue && k.Result.Value && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp).Sum(k => (int?)k.Num),
                        CommentCount = db.Comments.Count(k => k.ContentId.Equals(v.Key) && k.Result.HasValue && k.Result.Value && k.MonitorTimestamp > startTimestamp && k.MonitorTimestamp < endTimestamp)
                    }).Select(v => new Content() { ContentId = v.ContentId, Title = v.Title, ViewCount = v.ViewCount.HasValue ? v.ViewCount.Value : 0, ViewTimes = v.ViewTimes.HasValue ? v.ViewTimes.Value : 0, ScrollCount = v.ScrollCount, Sales = v.Sales.HasValue ? v.Sales.Value : 0, CommentCount = v.CommentCount, PivotDate = startTime }).ToList();
                return content;
            }
        }

        public List<ApiInfo> GetPivotApiInfo(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            var eventArr = new short[] { (short)EventTypeEnum.OnShow, (short)EventTypeEnum.OnLoad };
            using (var db = new MonitorContext())
            {
                var api = db.Pages
                    .Where(v => eventArr.Contains(v.EventType) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp)
                    .GroupBy(v => new { v.ChannelId, v.ContentId, v.PageId })
                    .Select(v => new { ChannelId = v.Key.ChannelId, ContentId = v.Key.ChannelId, ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1 })
                    .GroupBy(v => new { v.ChannelId, v.ContentId })
                    .Select(v => new
                    {
                        v.Key.ChannelId,
                        ChannelTitle = db.Pages
                        .Where(p => p.ChannelId.Equals(v.Key.ChannelId) && p.ChannelTitle != null).OrderByDescending(p => p.MonitorTimestamp).FirstOrDefault().ChannelTitle,
                        v.Key.ContentId,
                        ContentTitle = db.Pages.Where(p => p.ContentId.Equals(v.Key.ContentId) && p.ContentTitle != null).OrderByDescending(p => p.MonitorTimestamp).FirstOrDefault().ContentTitle,
                        RequestCount = v.Sum(k => (int?)k.ViewCount),
                        RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                        MinRequestTime = v.Min(k => (long?)k.ViewTime),
                        MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                        SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                    }).Select(v => new ApiInfo() { Type = 0, Title = v.ChannelTitle + (v.ContentId.Equals(0) || v.ContentTitle == null ? "" : "_" + v.ContentTitle), RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).ToList();
                var api_login = db.UserInfos.Where(v => v.EventType.Equals((short)EventTypeEnum.OnLogin) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.PageId).Select(v => new { ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1, Type = 1 }).GroupBy(v => v.Type).Select(v => new
                     {
                         v.Key,
                         RequestCount = v.Sum(k => (int?)k.ViewCount),
                         RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                         MinRequestTime = v.Min(k => (long?)k.ViewTime),
                         MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                         SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                     }).Select(v => new ApiInfo() { Type = (short)v.Key, Title = "用户登陆", RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).FirstOrDefault();
                var api_register = db.UserInfos.Where(v => v.EventType.Equals((short)EventTypeEnum.OnRegister) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.PageId).Select(v => new { ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1, Type = 1 }).GroupBy(v => v.Type).Select(v => new
                {
                    v.Key,
                    RequestCount = v.Sum(k => (int?)k.ViewCount),
                    RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                    MinRequestTime = v.Min(k => (long?)k.ViewTime),
                    MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                    SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                }).Select(v => new ApiInfo() { Type = (short)v.Key, Title = "用户注册", RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).FirstOrDefault();
                var api_coll = db.UserColls.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddColl) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.PageId).Select(v => new { ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1, Type = 1 }).GroupBy(v => v.Type).Select(v => new
                {
                    v.Key,
                    RequestCount = v.Sum(k => (int?)k.ViewCount),
                    RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                    MinRequestTime = v.Min(k => (long?)k.ViewTime),
                    MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                    SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                }).Select(v => new ApiInfo() { Type = (short)v.Key, Title = "用户收藏", RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).FirstOrDefault();
                var api_comment = db.UserColls.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddComment) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.PageId).Select(v => new { ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1, Type = 1 }).GroupBy(v => v.Type).Select(v => new
                {
                    v.Key,
                    RequestCount = v.Sum(k => (int?)k.ViewCount),
                    RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                    MinRequestTime = v.Min(k => (long?)k.ViewTime),
                    MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                    SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                }).Select(v => new ApiInfo() { Type = (short)v.Key, Title = "用户评论", RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).FirstOrDefault();
                var api_order = db.UserColls.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.PageId).Select(v => new { ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1, Type = 1 }).GroupBy(v => v.Type).Select(v => new
                {
                    v.Key,
                    RequestCount = v.Sum(k => (int?)k.ViewCount),
                    RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                    MinRequestTime = v.Min(k => (long?)k.ViewTime),
                    MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                    SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                }).Select(v => new ApiInfo() { Type = (short)v.Key, Title = "用户下单", RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).FirstOrDefault();
                var api_pay = db.UserColls.Where(v => v.EventType.Equals((short)EventTypeEnum.OnAddOrder) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.PageId).Select(v => new { ViewTime = v.Max(k => k.MonitorTimestamp) - v.Min(k => k.MonitorTimestamp), ViewCount = v.Count() - 1, Type = 1 }).GroupBy(v => v.Type).Select(v => new
                {
                    v.Key,
                    RequestCount = v.Sum(k => (int?)k.ViewCount),
                    RequestTimeSum = v.Sum(k => (long?)(k.ViewTime > 600000 ? 0 : k.ViewTime)),
                    MinRequestTime = v.Min(k => (long?)k.ViewTime),
                    MaxRequestTime = v.Max(k => (long?)k.ViewTime),
                    SpeedPercentage = v.Count(k => k.ViewTime < 1000)
                }).Select(v => new ApiInfo() { Type = (short)v.Key, Title = "用户支付", RequestCount = v.RequestCount.HasValue ? v.RequestCount.Value : 0, RequestTimeSum = v.RequestTimeSum.HasValue ? v.RequestTimeSum.Value : 0, MinRequestTime = v.MinRequestTime.HasValue ? v.MinRequestTime.Value : 0, MaxRequestTime = v.MaxRequestTime.HasValue ? v.MaxRequestTime.Value : 0, SpeedPercentage = v.SpeedPercentage, PivotDate = startTime }).FirstOrDefault();
                if (api_login != null)
                    api.Add(api_login);
                if (api_register != null)
                    api.Add(api_register);
                if (api_comment != null)
                    api.Add(api_comment);
                if (api_order != null)
                    api.Add(api_order);
                if (api_pay != null)
                    api.Add(api_pay);

                return api;
            }
        }

        public List<Related> GetPivotRelated(DateTime startTime, DateTime endTime)
        {
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var startTimestamp = (startTime.Ticks - time.Ticks) / 10000;
            var endTimestamp = (endTime.Ticks - time.Ticks) / 10000;
            using (var db = new MonitorContext())
            {
                var today_colls = db.UserColls
                  .Where(v => !v.Result.HasValue && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp);
                var today_orders = db.OrderDetails
                   .Where(v => !v.Result.HasValue && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp);
                var coll_related = today_colls.GroupBy(v => v.ContentId).Select(v => v.Key).Join(today_colls, a => a, b => b.ContentId, (a, b) => new { ContentId = a, b.UserId }).Join(today_colls, a => a.UserId, b => b.UserId, (a, b) => new { a.ContentId, a.UserId, RelevanceId = b.ContentId }).Where(v => !v.ContentId.Equals(v.RelevanceId)).GroupBy(v => new { v.ContentId, v.RelevanceId, v.UserId }).GroupBy(v => new { v.Key.ContentId, v.Key.RelevanceId }).Select(v => new Related() { ContentId = v.Key.ContentId, RelevanceId = v.Key.RelevanceId, CollRelevance = v.Count() }).ToList();
                var order_related = today_orders.GroupBy(v => v.ContentId).Select(v => v.Key).Join(today_orders, a => a, b => b.ContentId, (a, b) => new { ContentId = a, b.UserId }).Join(today_colls, a => a.UserId, b => b.UserId, (a, b) => new { a.ContentId, a.UserId, RelevanceId = b.ContentId }).Where(v => !v.ContentId.Equals(v.RelevanceId)).GroupBy(v => new { v.ContentId, v.RelevanceId, v.UserId }).GroupBy(v => new { v.Key.ContentId, v.Key.RelevanceId }).Select(v => new Related() { ContentId = v.Key.ContentId, RelevanceId = v.Key.RelevanceId, CollRelevance = v.Count() }).ToList();
                var today_pages = db.Pages.Where(v => v.EventType.Equals((short)EventTypeEnum.OnShow) && v.MonitorTimestamp > startTimestamp && v.MonitorTimestamp < endTimestamp).GroupBy(v => v.ContentId).Select(v => new Related() { ContentId = v.Key, ContentTitle = v.OrderByDescending(k => k.MonitorTimestamp).FirstOrDefault().ContentTitle, RelevanceId = 0, PivotDate = startTime }).ToList();
                today_pages.ForEach(v =>
                {
                    today_pages.ForEach(k =>
                    {
                        var m_coll = coll_related.FirstOrDefault(f => f.ContentId.Equals(v.ContentId) && f.RelevanceId.Equals(k.RelevanceId));
                        var m_order = order_related.FirstOrDefault(f => f.ContentId.Equals(v.ContentId) && f.RelevanceId.Equals(k.RelevanceId));
                        v.RelevanceId = k.ContentId;
                        v.TitleRelevance = (int)(GetLikePercentage(v.ContentTitle, k.ContentTitle) * 100);
                        v.CollRelevance = m_coll == null ? 0 : m_coll.CollRelevance;
                        v.OrderRelevance = m_order == null ? 0 : m_order.OrderRelevance;
                    });
                });
                return today_pages;
            }
        }

        private double GetLikePercentage(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;
            var c1 = s1.ToArray().Distinct().ToList();
            var c2 = s2.ToArray().Distinct().ToList();
            var r1 = c1.Intersect(c2);
            c2.AddRange(c1);
            return Math.Round(r1.Count() / 1.0 / c2.Distinct().Count(), 2);
        }
        #endregion
    }
}
