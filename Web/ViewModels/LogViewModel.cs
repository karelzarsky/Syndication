using SyndicateLogic.Entities;
using System;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public byte Severity { get; set; }
        public string Message { get; set; }
        public IEnumerable<LogViewModel> Restaurants { get; set; }
    }
}
