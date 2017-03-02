using System.Collections.Generic;
using SyndicateLogic.Entities;
using SyndicationWeb.Services;

namespace SyndicationWeb.ViewModels
{
    public struct LogLevel
    {
        public byte Number;
        public string DisplayName;
        public int Records;
    }

    public class LogViewModel
    {
        public PaginatedList<Log> Logs { get; set; }
        public List<LogLevel> LogLevels { get; set; }
        public int TotalPages { get; set; }
    }
}
