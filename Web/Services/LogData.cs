using SyndicateLogic;
using SyndicateLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Services
{
    public class LogData
    {
        private Db _ctx;

        public LogData(Db ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Log> GetAll()
        {
            return _ctx.Logs.ToArray();
        }
    }
}
