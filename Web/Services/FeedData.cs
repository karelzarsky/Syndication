using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;

namespace SyndicationWeb.Services
{
    public interface IFeedData
    {
        FeedListViewModel GetFeeds(int page, int pageSize, string lang, bool showActive, bool showInactive, string sortOrder = "");
        string AddFeed(string newFeed);
    }

    public class FeedData : IFeedData
    {
        private Db _ctx;

        public FeedData(Db ctx)
        {
            _ctx = ctx;
        }

        public string AddFeed(string newFeed)
        {
            if (_ctx.Feeds.Any(f => f.Url == newFeed))
                return "This feed is already in DB!";
            _ctx.Feeds.Add(new Feed { Url = newFeed, Active = true });
            _ctx.SaveChanges();
            RssLogic.UpdateServerConnection();
            return "Success !";
        }

        public FeedListViewModel GetFeeds(int page, int pageSize, string lang, bool showActive, bool showInactive, string sortOrder = "")
        {
            if (pageSize <= 0) pageSize = 50;
            var res = new FeedListViewModel();
            res.PageIndex = (page > 0) ? page : 1;
            IQueryable<Feed> query = _ctx.Feeds.Include("AffectedInstrument");
            query = query.Where(f => f.Language.Contains(lang) && ((f.Active && showActive) || (!f.Active && showInactive)));
            res.Feeds = new List<FeedViewModel>();
            res.TotalPages = query.Count() / pageSize + 1;
            foreach (var item in query.OrderBy(f => f.ID).Skip(pageSize * (page - 1)).Take(pageSize).ToArray())
            {
                res.Feeds.Add(new FeedViewModel
                {
                    FeedEntity = item,
                    articles = _ctx.Articles.Count(a => a.FeedID == item.ID),
                    articlesWithTicker = _ctx.Articles.Count(a => a.FeedID == item.ID && !string.IsNullOrEmpty(a.Ticker))
                });
            }
            return res;
        }
    }
}
