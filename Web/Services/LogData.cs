using System;
using System.Collections.Generic;
using System.Linq;
using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using LogLevel = SyndicationWeb.ViewModels.LogLevel;

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
            var res = new LogViewModel
            {
                LogLevels = new List<LogLevel>()
            };
            Array values = Enum.GetValues(typeof(SyndicateLogic.LogLevel));
            res.LogLevels.Add(new LogLevel { Number = 0, DisplayName = "All", Records = _ctx.Logs.Count() });
            foreach (var val in values)
            {
                int records = _ctx.Logs.Count(l => l.Severity == (byte) val);
                if (records > 0)
                    res.LogLevels.Add(new LogLevel
                    {
                        Number = (byte)val,
                        DisplayName = Enum.GetName(typeof(SyndicateLogic.LogLevel), val),
                        Records = records
                    });
            }
            IQueryable<Log> LogList = _ctx.Logs;
            if (level != 0)
                LogList = LogList.Where(l => l.Severity == level);
            LogList = descending ? LogList.OrderByDescending(l => l.Id) : LogList.OrderBy(l => l.Id);
            res.TotalPages = 1 + LogList.Count() / pageSize;
            res.Logs = PaginatedList<Log>.Create(LogList, page, pageSize);
            return res;
        }
    }
}
