using System.Threading;
using System.Diagnostics;

namespace SyndicationConsole
{
    internal class SyndicationConsole
    {
        private static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.BelowNormal;
            var s = new SyndicateService.Service();
            s.Start();
            while (true)
                System.Threading.Tasks.Task.Delay(20000);
        }
    }
}