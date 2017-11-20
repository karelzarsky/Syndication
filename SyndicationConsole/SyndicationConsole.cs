using System;
using System.Linq;
using System.Threading;
using SyndicateLogic;
using SyndicateLogic.Entities;
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
                Thread.Sleep(20000);
        }
    }
}