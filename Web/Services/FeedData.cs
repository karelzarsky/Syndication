using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;

namespace SyndicationWeb.Services
{
    public interface IFeedData
    {
        FeedListViewModel GetFeeds(int page = 1, int pageSize = 100, string sortOrder = "", string lang = "");
    }

    public class FeedData : IFeedData
    {
        private Db _ctx;

        public FeedData(Db ctx)
        {
            _ctx = ctx;
        }

        public FeedListViewModel GetFeeds(int page = 1, int pageSize = 100, string sortOrder = "", string lang = "")
        {
            if (pageSize <= 0) pageSize = 100;
            var res = new FeedListViewModel();
            res.PageIndex = (page > 0) ? page : 1;
            IQueryable<Feed> query = _ctx.Feeds.Include("AffectedInstrument");
            if (!string.IsNullOrEmpty(lang))
            {
                query = query.Where(a => a.Language == lang);
            }
            res.Feeds = new List<FeedViewModel>();
            res.TotalPages = query.Count() / pageSize + 1;
            foreach (var item in query.OrderBy(f => f.ID).Skip(pageSize * page).Take(pageSize).ToArray())
            {
                res.Feeds.Add(new FeedViewModel
                {
                    FeedEntity = item,
                    articles = _ctx.Articles.Count(a => a.FeedID == item.ID),
                    articlesWithTicker = _ctx.Articles.Count(a => a.FeedID == item.ID && a.Ticker != "")
                });
            }
            return res;
        }
    }
}
