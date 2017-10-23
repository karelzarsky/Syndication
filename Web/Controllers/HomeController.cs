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
        private readonly IShingleData _shingleData;

        public HomeController(ILogData logData, ITipsData tipsData, IArticlesData articlesData, ICompanyData companyData, IFeedData FeedData, IShingleData ShingleData)
        {
            _logData = logData;
            _tipsData = tipsData;
            _articlesData = articlesData;
            _companyData = companyData;
            _feedData = FeedData;
            _shingleData = ShingleData;
        }

        public IActionResult Index()
        {
            var model = _logData.GetAll();
            return View(model);
        }

        public IActionResult Logs(byte level = 0, bool descending = true, int page = 1, int pageSize = 100 )
        {
            ViewData["level"] = level;
            return View(_logData.GetAll(level, descending, page, pageSize));
        }

        public IActionResult Tips()
        {
            var model = new TipsViewModel {Tips = _tipsData.GetTips()};
            return View(model);
        }

        public IActionResult ArticleList(string ticker = "", string sortOrder = "", string lang = "", int? page = 1)
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

        public IActionResult FeedList(int page = 1, int pageSize = 50, string lang = "", string newFeed = "", string active = "Active shown", string inactive = "Inactive shown")
        {
            ViewData["page"] = page;
            ViewData["pageSize"] = pageSize;
            ViewData["lang"] = lang;
            ViewData["active"] = active;
            ViewData["inactive"] = inactive;
            if (!string.IsNullOrEmpty(newFeed))
                _feedData.AddFeed(newFeed);
            return View(_feedData.GetFeeds(page: page, pageSize:pageSize, lang:lang, showActive:(active == "Active shown"), showInactive:(inactive=="Inactive shown")));
        }

        public IActionResult ArticleDetail(int ArticleID)
        {
            return View(_articlesData.GetDetail(ArticleID));
        }

        public IActionResult Shingles(byte kind = 0, bool descending = true, int page = 1, int pageSize = 100, string filter = "", byte tokens = 0, string lang = "")
        {
            ViewData["page"] = page;
            ViewData["pageSize"] = pageSize;
            ViewData["filter"] = filter;
            ViewData["kind"] = kind;
            ViewData["tokens"] = tokens;
            ViewData["lang"] = lang;
            return View(_shingleData.GetAll(kind, descending, page, pageSize, filter, tokens, lang));
        }

        public IActionResult ShingleDetail(int ShingleID)
        {
            return View(_shingleData.GetDetail(ShingleID));
        }
    }
}
