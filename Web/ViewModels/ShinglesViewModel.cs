using System.Collections.Generic;
using SyndicateLogic.Entities;
using SyndicationWeb.Services;
using SyndicateLogic;

namespace SyndicationWeb.ViewModels
{
    public struct Kind
    {
        public byte Number;
        public string DisplayName;
        public int Records;
    }

    public class ShinglesViewModel
    {
        public PaginatedList<Shingle> Shingles { get; set; }
        public List<Kind> ShingleKinds { get; set; }
        public int TotalPages { get; set; }
    }
}
