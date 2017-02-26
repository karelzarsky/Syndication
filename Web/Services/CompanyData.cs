using SyndicateLogic;
using SyndicationWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationWeb.Services
{
    public interface ICompanyData
    {
        CompanyViewModel GetCompany(string Id);
    }

    public class CompanyData : ICompanyData
    {
        private Db _ctx;

        public CompanyData(Db ctx)
        {
            _ctx = ctx;
        }

        public CompanyViewModel GetCompany(string Id)
        {
            return new CompanyViewModel
            {
                Detail = _ctx.CompanyDetails.FirstOrDefault(c => c.ticker == Id)
            };
        }
    }
}
