using System.Diagnostics;
using SyndicateLogic;

namespace Analysis
{
    class Analysis
    {
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;

            var ctx = new Db();
            ctx.Database.CommandTimeout = 120;
            var shingleArray = ShingleLogic.GetNextShingles(ctx);

            while (shingleArray.Length>0)
            {
                foreach (int ShingleID in shingleArray)
                {
                    ShingleLogic.AnalyzeShingle(ShingleID);
                }
                shingleArray = ShingleLogic.GetNextShingles(ctx);
            }
        }
    }
}
