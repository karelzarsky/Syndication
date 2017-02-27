using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyndicationWeb.Services
{
    public interface ILogData
    {
        LogViewModel GetAll(byte level = 0, bool descending = true, int page = 1, int pageSize = 100);
    }

    public class LogData : ILogData
    {
        private Db _ctx;

        public LogData(Db ctx)
        {
            _ctx = ctx;
        }

        public LogViewModel GetAll(byte level = 0, bool descending = true, int page = 1, int pageSize = 100)
        {
            var res = new LogViewModel();
            res.TotalRecords = _ctx.Logs.Count();
            res.LogLevels = new List<ViewModels.LogLevel>();
            Array values = Enum.GetValues(typeof(SyndicateLogic.LogLevel));
            res.LogLevels.Add(new ViewModels.LogLevel()
            { Number = 0, DisplayName = "All" });
            foreach (var val in values)
            {
                res.LogLevels.Add(new ViewModels.LogLevel()
                { Number = (byte)val, DisplayName = Enum.GetName(typeof(SyndicateLogic.LogLevel), val) });
            }
            IQueryable<Log> LogList = _ctx.Logs;
            if (level != 0)
                LogList = LogList.Where(l => l.Severity == level);
            if (descending)
                LogList = LogList.OrderByDescending(l => l.Id);
            else
                LogList = LogList.OrderBy(l => l.Id);
            res.TotalPages = 1 + LogList.Count() / pageSize;
            res.Logs = PaginatedList<Log>.Create(LogList, page, pageSize);
            return res;
        }
    }
}
