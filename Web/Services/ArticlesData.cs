using SyndicateLogic;
using SyndicateLogic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SyndicationWeb.Services
{
    public interface IArticlesData
    {
        IQueryable<Article> GetArticles();
        IQueryable<Article> GetArticlesByTicker(string ticker);
        IEnumerable<Article> GetArticles(string ticker, string lang, string sortOrder, int page = 1, int pageSize = 100);
    }

    public class ArticlesData : IArticlesData
    {
        private Db _ctx;

        public ArticlesData(Db ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Article> GetArticles()
        {
            return _ctx.Articles;
        }

        public IEnumerable<Article> GetArticles(string ticker, string lang, string sortOrder, int page = 1, int pageSize = 100)
        {
            IQueryable<Article> articleQuery;

            if (string.IsNullOrEmpty(ticker))
            {
                articleQuery = _ctx.Articles;
            }
            else
            {
                articleQuery = _ctx.Articles.Where(a => a.Ticker == ticker);
            }

            if (!string.IsNullOrEmpty(lang))
            {
                articleQuery = articleQuery.Where(a => a.language == lang);
            }

            switch (sortOrder)
            {
                case "published": articleQuery = articleQuery.OrderBy(a => a.PublishedUTC); break;
                case "published_desc": articleQuery = articleQuery.OrderByDescending(a => a.PublishedUTC); break;
                case "received": articleQuery = articleQuery.OrderBy(a => a.ReceivedUTC); break;
                default: articleQuery = articleQuery.OrderByDescending(a => a.ReceivedUTC); break;
            }
            return PaginatedList<Article>.Create(articleQuery, page, pageSize);

        }

        public IQueryable<Article> GetArticlesByTicker(string ticker)
        {
            return _ctx.Articles.Where(a => a.Ticker == ticker);
        }
    }
}
