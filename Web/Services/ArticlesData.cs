using SyndicateLogic;
using SyndicateLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationWeb.Services
{
    public interface IArticlesData
    {
        IQueryable<Article> GetArticles();
        IQueryable<Article> GetArticlesByTicker(string ticker);
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

        public IQueryable<Article> GetArticlesByTicker(string ticker)
        {
            return _ctx.Articles.Where(a => a.Ticker == ticker);
        }
    }
}
