using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AddNew.Controllers
{
    public class CustomersController : Controller
    {
        // Attention 16 - Reference to a manager object
        private Manager m = new Manager();

        // GET: Customers
        // Attention 17 - Get all customers
        public ActionResult Index()
        {
            // Fetch the collection
            var c = m.CustomerGetAll();

            // Pass the collection to the view
            return View(c);
        }

        // GET: Customers/Details/5
        // Attention 18 - Get one customer by its identifier
        public ActionResult Details(int? id)
        {
            // Attempt to get the matching object
            var o = m.CustomerGetById(id.GetValueOrDefault());

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

        // GET: Customers/Create
        // Attention 19 - The "add new" pattern always requires two methods
        // One handles "get" (show me the HTML Form)
        // The other handles the user input ("post")
        public ActionResult Create()
        {
            // Optionally, can create and send an object to the view
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        public ActionResult Create(CustomerAdd newItem)
        {
            // Attention 20 - Notice the pattern for handling incoming data...
            // First, ensure that the incoming data is valid
            // Next, attempt to process the incoming data (add customer etc.)
            // Finally, if successful, redirect to another view
            // This is known as the PRG pattern - Post, Redirect, Get

            // Validate the input
            if (!ModelState.IsValid)
            {
                // Uh oh, problem with the data, show the form again, with the data
                return View(newItem);
            }

            // Process the input
            var addedItem = m.CustomerAdd(newItem);

            if (addedItem == null)
            {
                // Uh oh, some problem adding, show the empty form again
                return View(newItem);
            }
            else
            {
                return RedirectToAction("details", new { id = addedItem.CustomerId });
            }
        }
    }
}
