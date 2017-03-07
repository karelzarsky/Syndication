using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SyndicationWeb.Services
{
    public interface IArticlesData
    {
        IQueryable<Article> GetArticles();
        IQueryable<Article> GetArticlesByTicker(string ticker);
        IEnumerable<Article> GetArticles(string ticker, string lang, string sortOrder, int page = 1, int pageSize = 100);
        ArticleDetail GetDetail(int ArticleID);
    }

    public class ArticlesData : IArticlesData
    {
        private readonly Db _ctx;
        static readonly char[] delimiters = { '.', '?', '!', ',', ';', ' ' };

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
            var articleQuery = string.IsNullOrEmpty(ticker) ? _ctx.Articles : _ctx.Articles.Where(a => a.Ticker == ticker);

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

        public ArticleDetail GetDetail(int ArticleID)
        {
            var res = new ArticleDetail
            {
                ArticleEntity = _ctx.Articles.Find(ArticleID)
            };
            if (res.ArticleEntity == null) return null;
            var words = res.ArticleEntity.Text().Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Distinct();
            string text = res.ArticleEntity.Text();
            var scores = new double[text.Length];
            foreach (string word in words)
            {
                var sh = _ctx.Shingles.FirstOrDefault(s => s.text == word);
                if (sh == null) continue;
                var sa = _ctx.ShingleActions.FirstOrDefault(x => x.shingleID == sh.ID && x.interval == 15);
                if (sa == null) continue;
                if (sa.down == null || sa.up == null) continue;
                int wordIndex = text.IndexOf(word, StringComparison.Ordinal);
                for (int i = wordIndex; i < wordIndex + sh.text.Length; i++)
                    scores[i] += ((double)sa.up.Value + (double)sa.down.Value - 2) * 100;
            }

            res.colored = new List<coloredText>();
            for (int i = 0; i < text.Length; i++)
            {
                double blueValue = scores[i] * 4;
                if (blueValue < 0) blueValue = 0;
                if (blueValue > 15) blueValue = 15;
                string newColor = "#BB" + ((byte)blueValue).ToString("X");
                if (res.colored.Count == 0 || res.colored.Last().Color != newColor)
                {
                    res.colored.Add(new coloredText { Color = newColor, Text = text.Substring(i, 1) });
                }
                else
                {
                    res.colored[res.colored.Count - 1] = new coloredText
                    {
                        Color = newColor,
                        Text = res.colored[res.colored.Count - 1].Text + text.Substring(i, 1)
                    };
                }
            }
            return res;
        }
    }
}
