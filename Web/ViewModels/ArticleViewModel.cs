using System;

namespace SyndicationWeb.ViewModels
{
    public class ArticleViewModel
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime Published { get; set; }
        public DateTime Received { get; set; }
        public string URL { get; set; }
        public string Ticker { get; set; }
        public string Language { get; set; }
        public decimal ScoreMin { get; set; }
        public decimal ScoreMax { get; set; }
        public decimal Score
        {
            get { return 100 * (ScoreMin + ScoreMax - 2); }
        }
    }
}
