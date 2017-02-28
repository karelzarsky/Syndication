using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var res = new FeedListViewModel();
            IQueryable<Feed> query = _ctx.Feeds;
            if (!string.IsNullOrEmpty(lang))
            {
                query = query.Where(a => a.Language == lang);
            }
            res.Feeds = new List<FeedViewModel>();
            foreach (var item in query.ToArray())
            {
                res.Feeds.Add(new FeedViewModel
                {
                    FeedEntity = item,
                    articles = _ctx.Articles.Where(a => a.FeedID == item.ID).Count(),
                    articlesWithTicker = _ctx.Articles.Where(a => a.FeedID == item.ID && a.Ticker != "").Count()
                });
            }
            return res;
        }
    }
}
