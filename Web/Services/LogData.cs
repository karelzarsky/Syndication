using SyndicateLogic;
using SyndicateLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationWeb.Services
{
    public interface ILogData
    {
        IEnumerable<Log> GetAll();
    }

    public class LogData : ILogData
    {
        private Db _ctx;

        public LogData(Db ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Log> GetAll()
        {
            return _ctx.Logs.OrderByDescending(l => l.Id).Take(100).ToArray();
        }
    }
}
