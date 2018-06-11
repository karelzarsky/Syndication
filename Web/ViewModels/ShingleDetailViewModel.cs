using SyndicateLogic.Entities;
using System.Collections.Generic;

namespace SyndicationWeb.ViewModels
{
    public class ShingleDetailViewModel
    {
        public SyndicateLogic.Entities.Shingle ShingleEntity { get; set; }
        public List<Article> Articles {get; set;}
    }
}
