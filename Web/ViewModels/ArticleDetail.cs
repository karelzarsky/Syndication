using System;
using System.Collections.Generic;

namespace SyndicationWeb.ViewModels
{
    public struct ColoredText
    {
        public string Color;
        public string Text;
    }

    public class ScoredPhrase : IComparable
    {
        public string Phrase;
        public string Color;
        public float Score;
        public int ShingleID;

        public int CompareTo(object obj)
        {
            return Phrase.CompareTo(((ScoredPhrase)obj).Phrase);
        }
    }

    public class ArticleDetail
    {
        public SyndicateLogic.Entities.Article ArticleEntity { get; set; }
        public string ColoredText { get; set; }
        public List<ColoredText> Colored { get; set; }
        public List<ScoredPhrase> ScoredPhrases { get; set; }
    }
}
