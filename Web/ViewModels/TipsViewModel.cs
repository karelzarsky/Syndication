using System;
using System.Collections.Generic;

namespace SyndicationWeb.ViewModels
{
    public class TradeTip
    {
        public DateTime Published { get; set; }
        public DateTime Received { get; set; }
        public int ArticleID { get; set; }
        public decimal Score { get; set; }
        public string ScoreColor { get; set; }
        public string Ticker { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public class TipsViewModel
    {
        public IEnumerable<TradeTip> Tips { get; set; }
    }
}
