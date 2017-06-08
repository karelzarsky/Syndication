using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using System;

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
                articleQuery = articleQuery.Where(a => a.language == lang);
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
                ArticleEntity = _ctx.Articles.Find(ArticleID),
                ScoredPhrases = new List<ScoredPhrase>()
            };
            if (res.ArticleEntity == null) return null;
            var phrases = ShingleLogic.FindPhrases(res.ArticleEntity);
            string text = res.ArticleEntity.Text();
            var scores = new double[text.Length];
            foreach (string phrase in phrases)
            {
                var sh = _ctx.Shingles.FirstOrDefault(s => s.text == phrase);
                if (sh == null) continue;
                var sa = _ctx.ShingleActions.FirstOrDefault(x => x.shingleID == sh.ID && x.interval == 15);
                if (sa == null) continue;
                if (sa.down == null || sa.up == null) continue;
                float score = (sa.up.Value + sa.down.Value - 2) * 100;
                res.ScoredPhrases.Add(new ScoredPhrase { Score = score, Phrase = phrase, Color = GetColor(score) });
                int wordIndex = text.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (wordIndex >= 0)
                    for (int i = wordIndex; i < wordIndex + sh.text.Length; i++)
                        scores[i] += score;
            }

            res.Colored = new List<ColoredText>();
            for (int i = 0; i < text.Length; i++)
            {
                string newColor = GetColor(scores[i]);
                if (res.Colored.Count == 0 || res.Colored.Last().Color != newColor)
                {
                    res.Colored.Add(new ColoredText { Color = newColor, Text = text.Substring(i, 1) });
                }
                else
                {
                    res.Colored[res.Colored.Count - 1] = new ColoredText
                    {
                        Color = newColor,
                        Text = res.Colored[res.Colored.Count - 1].Text + text.Substring(i, 1)
                    };
                }
            }
            return res;
        }

        private static string GetColor(double score)
        {
            if (score == 0.0) // black #000
                return "#000";
            else if (score > 0) // blue #0FF
            {
                double blueValue = (score * 4);
                if (blueValue > 15) blueValue = 15;
                return "#0" + ((byte)blueValue).ToString("X") + ((byte)blueValue).ToString("X");
            }
            // red #F00
            double redValue = -score * 8;
            if (redValue > 15) redValue = 15;
            return "#" + ((byte)redValue).ToString("X") + "00";
        }
    }
}
