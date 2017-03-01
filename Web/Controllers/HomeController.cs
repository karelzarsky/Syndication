using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SyndicateLogic.Entities;
using SyndicationWeb.Services;
using SyndicationWeb.ViewModels;

namespace SyndicationWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogData _logData;
        private readonly ITipsData _tipsData;
        private readonly IArticlesData _articlesData;
        private readonly ICompanyData _companyData;
        private readonly IFeedData _feedData;

        public HomeController(ILogData logData, ITipsData tipsData, IArticlesData articlesData, ICompanyData companyData, IFeedData FeedData)
        {
            _logData = logData;
            _tipsData = tipsData;
            _articlesData = articlesData;
            _companyData = companyData;
             _feedData = FeedData;
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
            var model = new TipsViewModel {Tips = _tipsData.GetTips()};
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

        public IActionResult FeedList(int page = 1, int pageSize = 100)
        {
            ViewData["page"] = page;
            ViewData["pageSize"] = pageSize;
            return View(_feedData.GetFeeds(page: page, pageSize:pageSize));
        }
    }
}
