using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTaskJob;
using Yi.MonitorCore.Business;
using Yi.PivotCore.Models;
using Yi.PivotCore.Business;

namespace Yi.PivotorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")); // DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
            var endTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd"));//DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));

            var monitorBll = new MonitorBll();
            var overview = monitorBll.GetPivotRelated(startTime, endTime);

            var pivotBll = new PivotBll();
            pivotBll.Add<Related>(overview.ToArray());


            Console.ReadKey();

            //PivotJob job = new PivotJob(new TaskJobConfig(60000));
            //job.StartJob();
            //Console.ReadKey();
            //job.StopJob(true);
        }
    }

    public class PivotJob : TaskJob
    {
        public PivotJob(TaskJobConfig config)
            : base(config)
        {

        }
        protected override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
