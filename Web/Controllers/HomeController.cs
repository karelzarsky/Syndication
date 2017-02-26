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

        public IActionResult Articles(string id = "")
        {
            if (string.IsNullOrEmpty(id))
                return View(_articlesData.GetArticles());
            else
                return View(_articlesData.GetArticlesByTicker(id));
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

        [HttpPost]
        public IActionResult CompanyByTicker(string ticker)
        {
            var model = _companyData.GetCompany(ticker);
            if (model == null || model.Detail == null)
                return NotFound();
            else
                return View("Company", model);
        }
    }
}
