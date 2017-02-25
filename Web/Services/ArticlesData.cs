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
        IEnumerable<Article> GetArticles();
    }

    public class ArticlesData : IArticlesData
    {
        private Db _ctx;

        public ArticlesData(Db ctx)
        {
            _ctx = ctx;
        }
        public IEnumerable<Article> GetArticles()
        {
            return _ctx.Articles.OrderByDescending(a => a.ID).Take(100).ToArray();
        }
    }
}
