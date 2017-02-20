using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomStoreForCarBusiness.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LoadDataController : Controller
    {
        // Reference to the manager object
        Manager m = new Manager();

        // Attention 16 - Must run methods in this LoadData controller before using the app

        // GET: LoadData
        public ActionResult Index()
        {
            return View();
        }

        // GET: LoadData/Country
        public ActionResult Country()
        {
            if (m.LoadDataCountry())
            {
                ViewBag.Result = "Country data was loaded";
            }
            else
            {
                ViewBag.Result = "(done)";
            }
            return View("result");
        }

        // GET: LoadData/Manufacturer
        public ActionResult Manufacturer()
        {
            ViewBag.Result = m.LoadDataManufacturer()
                ? "Manufacturer data was loaded"
                : "(done)";

            return View("result");
        }

        // GET: LoadData/Vehicle
        public ActionResult Vehicle()
        {
            ViewBag.Result = m.LoadDataVehicle()
                ? "Vehicle data was loaded"
                : "(done)";

            return View("result");
        }

        // GET: LoadData/Remove
        public ActionResult Remove()
        {
            ViewBag.Result = m.RemoveData()
                ? "Data has been removed"
                : "(done)";

            return View("result");
        }

        // GET: LoadData/RemoveDatabase
        public ActionResult RemoveDatabase()
        {
            ViewBag.Result = m.RemoveDatabase()
                ? "Database has been removed"
                : "(done)";

            return View("result");
        }

    }
}