using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace ShingelAnalyze
{
    class ShingleAnalyze
    {
        static void Main(string[] args)
        {
            var ctx = new Db();
            var s = FindNewShingle(ctx);

        }

        private static Shingle FindNewShingle(Db ctx)
        {

            return null;
        }
    }
}
