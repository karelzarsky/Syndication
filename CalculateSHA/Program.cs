using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicateLogic;

namespace CalculateSHA
{
    class Program
    {
        static void Main(string[] args)
        {
            var _context = new Db();
            var count = 0;
            foreach (var article in _context.Articles)
            {
                if (article.Sha256Hash != null) continue;
                article.Sha256Hash = DataLayer.getHashSha256(article.RSS20);
                Console.WriteLine(count++ + " " + article.Sha256Hash[0]);
            }
            _context.SaveChanges();
        }
    }
}
