using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace Wabbit
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new Db();
            foreach (var article in ctx.Articles)
            {

            }
        }
    }
}
