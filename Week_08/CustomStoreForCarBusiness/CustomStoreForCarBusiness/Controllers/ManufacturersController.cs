using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomStoreForCarBusiness.Controllers
{
    public class ManufacturersController : Controller
    {
        // Reference to the data manager
        private Manager m = new Manager();

        // Attention 22 - Manufacturers controller, limited functionality (get all, get one, add vehicle)

        // GET: Manufacturers
        public ActionResult Index()
        {
            return View(m.ManufacturerGetAllWithDetail());
        }

        // GET: Manufacturers/Details/5
        public ActionResult Details(int? id)
        {
            // Attempt to get the matching object
            var o = m.ManufacturerGetByIdWithDetail(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Pass the object to the view
                return View(o);
            }
        }

        // GET: Manufacturers/5/AddVehicle
        // Attention 23 - Used "attribute routing" for a custom URL segment (resource)
        // Allow a Manager to use this action/method
        [Authorize(Roles = "Manager")]
        [Route("manufacturers/{id}/addvehicle")]
        public ActionResult AddVehicle(int? id)
        {
            // Attempt to get the associated object
            var a = m.ManufacturerGetById(id.GetValueOrDefault());

            if (a == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Add vehicle for a known manufacturer
                // We send the manufacturer identifier to the form
                // There, it is hidden... <input type=hidden
                // We also pass on the name, so that the browser user
                // knows which manufacturer they're working with

                // Create and configure a form object
                var o = new VehicleAddForm();
                o.ManufacturerId = a.Id;
                o.ManufacturerName = a.Name;

                return View(o);
            }
        }

        // POST: Manufacturers/5/AddVehicle
        // Attention 24 - Used "attribute routing" for a custom URL segment (resource)
        // Allow a Manager to use this action/method
        [Authorize(Roles = "Manager")]
        [Route("manufacturers/{id}/addvehicle")]
        [HttpPost]
        public ActionResult AddVehicle(VehicleAdd newItem)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }

            // Process the input
            var addedItem = m.VehicleAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                // Attention 25 - Must redirect to the Vehicles controller
                return RedirectToAction("details", "vehicles", new { id = addedItem.Id });
            }
        }


    }
}
