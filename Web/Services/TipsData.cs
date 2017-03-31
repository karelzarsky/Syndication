using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicationWeb.ViewModels;

namespace SyndicationWeb.Services
{
    public interface ITipsData
    {
        IEnumerable<TradeTip> GetTips();
    }

    public class TipsData : ITipsData
    {
        private Db _ctx;

        public TipsData(Db ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<TradeTip> GetTips()
        {
            string cmd =
@"SELECT a.PublishedUTC Published, a.ReceivedUTC Received, a.ID ArticleID, 100.00*AVG(sa.down + sa.up -2.00) Score, '' ScoreColor, a.Ticker Ticker, a.Title Title, a.URI_links Link
FROM fact.shingleAction sa
JOIN rss.shingleUse su on su.ShingleID = sa.shingleID
JOIN rss.articles a on a.ID = su.ArticleID
WHERE a.ticker is not null and sa.interval = 30 and sa.samples > 30 and mean <> 0 and a.ReceivedUTC >= GETDATE()-2
GROUP BY a.ID, a.Ticker, a.ReceivedUTC, a.PublishedUTC, a.URI_links, a.Title
HAVING AVG(sa.down + sa.up -2.00) > .04
ORDER BY a.ID DESC";

            var tips = _ctx.Database.SqlQuery<TradeTip>(cmd).ToArray();
            return tips;
        }
    }
}
