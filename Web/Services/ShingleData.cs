using System;
using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;

namespace SyndicationWeb.Services
{
    public interface IShingleData
    {
        ShinglesViewModel GetAll(byte level = 0, bool descending = true, int page = 1, int pageSize = 100, string filter = null, byte tokens = 0, string lang = "");
        ShingleDetailViewModel GetDetail(int shingleID);
    }

    public class ShingleData : IShingleData
    {
        private Db _ctx;

        public ShingleData(Db ctx)
        {
            _ctx = ctx;
        }

        public ShinglesViewModel GetAll(byte kind = 0, bool descending = true, int page = 1, int pageSize = 100, string filter = null, byte tokens = 0, string lang = "")
        {
            var res = new ShinglesViewModel
            {
                ShingleKinds = new List<Kind>()
            };
            Array values = Enum.GetValues(typeof(ShingleKind));
            res.ShingleKinds.Add(new Kind { Number = 0, DisplayName = "All", Records = _ctx.Shingles.Count() });
            foreach (var val in values)
            {
                int records = _ctx.Shingles.Count(l => l.kind == (ShingleKind) val);
                if (records > 0)
                    res.ShingleKinds.Add(new Kind
                    {
                        Number = (byte)val,
                        DisplayName = Enum.GetName(typeof(ShingleKind), val),
                        Records = records
                    });
            }
            IQueryable<Shingle> ShingleList = _ctx.Shingles;
            if (kind != 0)
                ShingleList = ShingleList.Where(l => (byte) l.kind == kind);
            if (!string.IsNullOrWhiteSpace(filter))
                ShingleList = ShingleList.Where(l => l.text.Contains(filter));
            if (!string.IsNullOrWhiteSpace(lang))
                ShingleList = ShingleList.Where(l => l.language == lang);
            if (tokens > 0)
                ShingleList = ShingleList.Where(l => l.tokens == tokens);
            ShingleList = descending ? ShingleList.OrderByDescending(l => l.ID) : ShingleList.OrderBy(l => l.ID);
            res.TotalPages = 1 + ShingleList.Count() / pageSize;
            res.Shingles = PaginatedList<Shingle>.Create(ShingleList, page, pageSize);
            return res;
        }

        public ShingleDetailViewModel GetDetail(int shingleID)
        {
            ShingleDetailViewModel res = new ShingleDetailViewModel();
            res.ShingleEntity = _ctx.Shingles.FirstOrDefault(s => s.ID == shingleID);
            if (res.ShingleEntity == null)
                res.ShingleEntity = new Shingle();
            res.Articles =
                (from a in _ctx.Articles
                 join su in _ctx.ShingleUses on a.ID equals su.ArticleID
                 where su.ShingleID == shingleID
                 select a
                ).Take(10).ToList();
            //foreach (var a in res.Articles)
            //{
            //    a.Summary = Regex.Replace(a.Summary, res.ShingleEntity.text, "<B>" + res.ShingleEntity.text + "</B>", RegexOptions.IgnoreCase);
            //}
            return res;
        }
    }
}
