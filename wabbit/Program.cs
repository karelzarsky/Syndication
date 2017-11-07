using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;
using SyndicateLogic.Entities;

namespace Wabbit
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new Db())
                foreach (var article in ctx.Articles.Where(a => a.Ticker != null && a.language == "en"))
                {
                    var myExa = new MyExample
                    {
                        Date = article.PublishedUTC,
                        ArticleID = article.ID,
                        Ticker = article.Ticker
                    };
                    FillPreviousPrices(myExa);
                    FillAfterPrices(myExa);
                    FillShingles(myExa);
                }
        }

        private static void FillShingles(MyExample myExa)
        {
            using (var ctx = new Db())
            {
                myExa.Shingles.AddRange(ctx.Shingles
                    .Where(s => ctx.ShingleUses
                    .Any(su => s.kind == ShingleKind.interesting && su.ArticleID == myExa.ArticleID && su.ShingleID == s.ID))
                    .Select(x => x.text));
            }
        }

        private static void FillAfterPrices(MyExample myExa)
        {
            throw new NotImplementedException();
        }

        private static void FillPreviousPrices(MyExample myExa)
        {
            throw new NotImplementedException();
        }
    }
}
