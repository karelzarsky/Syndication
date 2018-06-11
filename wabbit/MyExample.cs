using System;
using System.Collections.Generic;

namespace Wabbit
{
    public class MyExample
    {
        public double PriceChangePercent;
        public double PrevO, PrevH, PrevL, PrevC;
        public double AfterO, AfterH, AfterL, AfterC;
        public string Tag;
        public List<string> Shingles = new List<string>();
        public DateTime Date;
        public string Ticker;
        public int ArticleID;

        internal VWExample ToVW()
        {
            var PriceNamespace = new Namespace
            {
                Name = "P",
                Features = new List<Feature> {
                    new Feature { Name = "O", Value = PrevO},
                    new Feature { Name = "H", Value = PrevH},
                    new Feature { Name = "L", Value = PrevL},
                    new Feature { Name = "C", Value = PrevC},
                }
            };

            var ShinglesNamespace = new Namespace { Name = "S", Features = new List<Feature>()};

            foreach (var sh in Shingles)
                ShinglesNamespace.Features.Add(new Feature { Name = sh });

            var vw = new VWExample
            {
                Label = PriceChangePercent,
                Tag = ArticleID.ToString() + "X" + Math.Round(PriceChangePercent, 3),
                Namespaces = new List<Namespace>
                { PriceNamespace, ShinglesNamespace }
            };

            return vw;
        }
    }
}
