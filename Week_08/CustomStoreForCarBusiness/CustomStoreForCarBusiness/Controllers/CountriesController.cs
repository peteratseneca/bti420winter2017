using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomStoreForCarBusiness.Controllers
{
    public class CountriesController : Controller
    {
        // Reference to the data manager
        private Manager m = new Manager();

        // Attention 21 - Countries controller, not much happening here

        // GET: Countries
        public ActionResult Index()
        {
            return View(m.CountryGetAll());
        }

    }
}
