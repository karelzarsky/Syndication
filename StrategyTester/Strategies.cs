using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace StrategyTester
{
    public class Strategies
    {
        public static Signal Strategy001(int ArticleID)
        {
            var ctx = new Db();
            if (ctx.ArticleScores.Any(x => x.articleID == ArticleID && x.score != null && x.score.Value > 0.5m)) return Signal.Buy;
            if (ctx.ArticleScores.Any(x => x.articleID == ArticleID && x.score != null && x.score.Value < -0.2m)) return Signal.Sell;
            return Signal.Nothing;
        }
    }
}
