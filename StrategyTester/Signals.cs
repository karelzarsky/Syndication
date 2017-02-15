using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;
using System.Data.SqlClient;

namespace StrategyTester
{
    public interface ISignal
    {
        int getID();
        Signal EvaluateArticle(int ArticleID, double buyLimit, double sellLimit);
    }

    public class Signal002 : ISignal
    {
        public Signal EvaluateArticle(int ArticleID, double buyLimit, double sellLimit)
        {
            const string getActions = @"SELECT 100* (avg(mean)-1) mean
FROM fact.shingleAction sa
JOIN rss.shingleUse su on su.ShingleID = sa.shingleID
JOIN rss.articles a on a.ID = su.ArticleID
WHERE a.ID = @ArticleID and a.ticker is not null and sa.interval = 10 and mean <> 0 and a.ReceivedUTC < DATEADD(day, -50, getdate())
group by a.ID, a.Ticker, a.ScoreDownMin, a.ScoreUpMax
having count (su.ArticleID) > 20";
            var ctx = new Db();
            double score = ctx.Database.SqlQuery<double>(getActions,
                new SqlParameter("@ArticleID", ArticleID)).FirstOrDefault();
            if (score == -1) return Signal.Nothing;
            if (score < sellLimit)
                return Signal.Sell;
            if (score > buyLimit)
                return Signal.Buy;
            return Signal.Nothing;
        }

        public int getID()
        {
            return 2;
        }
    }

    public class Signals
    {

        public static Signal Signal001(int ArticleID)
        {
            var ctx = new Db();
            if (ctx.ArticleScores.Any(x => x.articleID == ArticleID && x.score != null && x.score.Value > 0.5m)) return Signal.Buy;
            if (ctx.ArticleScores.Any(x => x.articleID == ArticleID && x.score != null && x.score.Value < -0.2m)) return Signal.Sell;
            return Signal.Nothing;
        }
    }
}
