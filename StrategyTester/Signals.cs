using System;
using System.Data.SqlClient;
using System.Linq;
using SyndicateLogic;

namespace StrategyTester
{
    public class Signals
    {

        public static DirectionType Signal001(int ArticleID)
        {
            var ctx = new Db();
            if (ctx.ArticleScores.Any(x => x.articleID == ArticleID && x.score != null && x.score.Value > 0.5m)) return DirectionType.Buy;
            if (ctx.ArticleScores.Any(x => x.articleID == ArticleID && x.score != null && x.score.Value < -0.2m)) return DirectionType.Sell;
            return DirectionType.Nothing;
        }
    }

    public interface ISignal
    {
        int getID();
        DirectionType EvaluateArticle(int ArticleID, double buyLimit, double sellLimit);
    }

    public class Signal002 : ISignal
    {
        public DirectionType EvaluateArticle(int ArticleID, double buyLimit, double sellLimit)
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
            if (score == -1) return DirectionType.Nothing;
            if (score < sellLimit)
                return DirectionType.Sell;
            if (score > buyLimit)
                return DirectionType.Buy;
            return DirectionType.Nothing;
        }

        public int getID()
        {
            return 2;
        }
    }

    public class Signal003 : ISignal
    {
        public DirectionType EvaluateArticle(int ArticleID, double buyLimit, double sellLimit)
        {
            const string getActions =
@"DECLARE @avgmean float = (select avg(mean) from fact.shingleAction where interval = 30 and samples > 10)
SELECT avg((mean - @avgmean)/variance)
FROM fact.shingleAction sa
JOIN rss.shingleUse su on su.ShingleID = sa.shingleID
JOIN rss.articles a on a.ID = su.ArticleID
WHERE a.ID = @ArticleID and a.ticker is not null and sa.interval = 30 and mean <> 0
group by a.ID, a.Ticker, a.ScoreDownMin, a.ScoreUpMax
having count (su.ArticleID) > 10";
            var ctx = new Db();
            double score = ctx.Database.SqlQuery<double>(getActions,
                new SqlParameter("@ArticleID", ArticleID)).FirstOrDefault();
            if (score == -1) return DirectionType.Nothing;
            if (score < sellLimit)
                return DirectionType.Sell;
            if (score > buyLimit)
                return DirectionType.Buy;
            return DirectionType.Nothing;
        }

        public int getID()
        {
            return 3;
        }
    }

    public class Order
    {
        public DateTime Issued { get; set; }
        public int ArticleID { get; set; }
        public string Ticker { get; set; }
        public DirectionType Direction { get; set; }
        public double TakeProfit { get; set; }
        public double StopLoss { get; set; }
        public int Duration { get; set; }
        public string Reason { get; set; }
    }

    public static class Signal004
    {
        public static Order[] GetSignals004(int interval, double limit, double tp, double sl, DateTime from, DateTime till)
        {
            const string sqlcmd =
@"DECLARE @avgmean float = (select avg(mean) from fact.shingleAction where interval = @interval and samples > 10)
SELECT a.PublishedUTC Issued, a.ID ArticleID, a.Ticker Ticker, cast(sign(avg((mean - @avgmean)/variance)) as int) Direction,
@tp TakeProfit, @sl StopLoss, 30 Duration, concat(' Signal004 score:',Str(avg((mean - @avgmean)/variance), 8, 5)) Reason
FROM fact.shingleAction sa
JOIN rss.shingleUse su on su.ShingleID = sa.shingleID
JOIN rss.articles a on a.ID = su.ArticleID
WHERE a.ticker is not null and sa.interval = @interval and mean <> 0 and a.PublishedUTC >= @from and a.PublishedUTC <= @till
group by a.ID, a.PublishedUTC, a.Ticker
having count (su.ArticleID) > 10 and abs(avg((mean - @avgmean)/variance)) > @limit and abs( avg(mean - @avgmean))>0.01
order by a.PublishedUTC";
            using (var ctx = new Db())
            {
                return ctx.Database.SqlQuery<Order>(sqlcmd,
                    new SqlParameter("@interval", interval),
                    new SqlParameter("@limit", limit),
                    new SqlParameter("@tp", tp),
                    new SqlParameter("@sl", sl),
                    new SqlParameter("@from", from),
                    new SqlParameter("@till", till)).ToArray();
            }
        }
    }
}
