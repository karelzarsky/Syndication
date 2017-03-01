using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SyndicateLogic.Entities;
using SyndicationWeb.Services;
using SyndicationWeb.ViewModels;

namespace SyndicationWeb.Controllers
{
    public class HomeController : Controller
    {
        private ILogData _logData;
        private ITipsData _tipsData;
        private IArticlesData _articlesData;
        private ICompanyData _companyData;

        public HomeController(ILogData logData, ITipsData tipsData, IArticlesData articlesData, ICompanyData companyData)
        {
            _logData = logData;
            _tipsData = tipsData;
            _articlesData = articlesData;
            _companyData = companyData;
        }

        public IActionResult Index()
        {
            var model = _logData.GetAll();
            return View(model);
        }

        public IActionResult Logs(byte level = 0, bool descending = true, int page = 1, int pageSize = 100 )
        {
            return View(_logData.GetAll(level, descending, page, pageSize));
        }

        public IActionResult Tips()
        {
            var model = new TipsViewModel();
            model.Tips = _tipsData.GetTips();
            return View(model);
        }

        public IActionResult Articles(string ticker = "", string sortOrder = "", string lang = "", int? page = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PublishedSortParm"] = string.IsNullOrEmpty(sortOrder) ? "published_desc" : "";
            ViewData["ReceivedSortParm"] = sortOrder == "received" ? "received_desc" : "received";
            ViewData["ticker"] = ticker;
            ViewData["lang"] = lang;

            return View(_articlesData.GetArticles(ticker, lang, sortOrder, page ?? 1));
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Company(string ticker)
        {
            var model = _companyData.GetCompany(ticker);
            if (model == null || model.Detail == null)
                return NotFound();
            return View(model);
        }

        public IActionResult Companies(int page = 1, string nameFilter = "")
        {
            ViewData["nameFilter"] = nameFilter;
            return View(PaginatedList <CompanyDetail>.Create(_companyData.GetCompaniesByName(nameFilter).OrderBy(c => c.ticker), page, 100));
        }
    }
}
