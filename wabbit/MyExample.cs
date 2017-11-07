using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wabbit
{
    public class MyExample
    {
        public double PriceChangePercent;
        public double PrevO, PrevH, PrevL, PrevC;
        public double AfterO, AfterH, AfterL, AfterC;
        public string Tag;
        public List<string> Shingles;
        public DateTime Date;
        public string Ticker;
        public int ArticleID;
    }
}
