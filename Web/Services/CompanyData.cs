using SyndicateLogic;
using SyndicateLogic.Entities;
using SyndicationWeb.ViewModels;
using System.Linq;

namespace SyndicationWeb.Services
{
    public interface ICompanyData
    {
        CompanyViewModel GetCompany(string Id);
        IQueryable<CompanyDetail> GetCompaniesByName(string name);
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

        public IQueryable<CompanyDetail> GetCompaniesByName(string name)
        {
            return _ctx.CompanyDetails.Where(c => c.name.Contains(name) || c.legal_name.Contains(name));
        }
    }
}
