using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationWeb.ViewModels
{
    public class ArticleDetail
    {
        public SyndicateLogic.Entities.Article ArticleEntity { get; set; }
        public string ColoredText { get; set; }
    }
}
