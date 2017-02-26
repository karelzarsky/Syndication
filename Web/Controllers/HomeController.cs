using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var model = new LogViewModel();
            model.Logs = _logData.GetAll();
            return View(model);
        }

        public IActionResult Logs()
        {
            var model = new LogViewModel();
            model.Logs = _logData.GetAll();
            return View(model);
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

            IQueryable<Article> articleQuery;

            if (string.IsNullOrEmpty(ticker))
            {
                articleQuery = _articlesData.GetArticles();
            }
            else
            {
                articleQuery = _articlesData.GetArticlesByTicker(ticker);
            }

            if (!string.IsNullOrEmpty(lang))
            {
                articleQuery = articleQuery.Where(a => a.language == lang);
                ViewData["lang"] = lang;
            }

            switch (sortOrder)
            {
                case "published": articleQuery = articleQuery.OrderBy(a => a.PublishedUTC); break;
                case "published_desc": articleQuery = articleQuery.OrderByDescending(a => a.PublishedUTC); break;
                case "received": articleQuery = articleQuery.OrderBy(a => a.ReceivedUTC); break;
                default: articleQuery = articleQuery.OrderByDescending(a => a.ReceivedUTC); break;
            }
            return View(PaginatedList<Article>.Create(articleQuery, page ?? 1, 100));
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Company(string Id)
        {
            var model = _companyData.GetCompany(Id);
            if (model == null || model.Detail == null)
                return NotFound();
            else
                return View(model);
        }
    }
}
