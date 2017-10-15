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
        ShingleViewModel GetAll(byte level = 0, bool descending = true, int page = 1, int pageSize = 100);
    }

    public class ShingleData : IShingleData
    {
        private Db _ctx;

        public ShingleData(Db ctx)
        {
            _ctx = ctx;
        }

        public ShingleViewModel GetAll(byte kind = 0, bool descending = true, int page = 1, int pageSize = 100)
        {
            var res = new ShingleViewModel
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
            ShingleList = descending ? ShingleList.OrderByDescending(l => l.ID) : ShingleList.OrderBy(l => l.ID);
            res.TotalPages = 1 + ShingleList.Count() / pageSize;
            res.Shingles = PaginatedList<Shingle>.Create(ShingleList, page, pageSize);
            return res;
        }
    }
}
