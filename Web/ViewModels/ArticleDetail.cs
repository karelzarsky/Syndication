using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationWeb.ViewModels
{
    public struct coloredText
    {
        public string Color;
        public string Text;
    }

    public class ArticleDetail
    {
        public SyndicateLogic.Entities.Article ArticleEntity { get; set; }
        public string ColoredText { get; set; }
        public List<coloredText> colored { get; set; }
    }
}
