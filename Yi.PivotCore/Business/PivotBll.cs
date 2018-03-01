using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.PivotCore.DataAccess;
using System.Data.Entity;

namespace Yi.PivotCore.Business
{
    public class PivotBll
    {
        public void Add<T>(params T[] m) where T : class,new()
        {
            using (var db = new PivotContext())
            {
                db.Insert<T>(m);
            }
        }

        public void GetOverview()
        {
            var today = DateTime.Now.Date;
            var lastweek = DateTime.Parse(DateTime.Now.AddDays(0 - DateTime.Now.DayOfWeek).ToString("yyyy/MM/hh"));
            using (var db = new PivotContext())
            {
                var d1 = db.Overviews.FirstOrDefault(v => v.PivotDate.Equals(today));

            }
        }
    }
}
