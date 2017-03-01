using System.Collections.Generic;
using SyndicateLogic.Entities;

namespace SyndicationWeb.ViewModels
{
    public class FeedViewModel
    {
        public Feed FeedEntity;
        public int articles;
        public int articlesWithTicker;
    }

    public class FeedListViewModel
    {
        public List<FeedViewModel> Feeds;
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        { get { return (PageIndex > 1); } }

        public bool HasNextPage
        { get { return (PageIndex < TotalPages); } }
    }
}
