using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditDelete.Controllers
{
    public class CustomersController : Controller
    {
        // Attention 11 - Reference to a manager object
        private Manager m = new Manager();

        // GET: Customers
        public ActionResult Index()
        {
            // Fetch the collection
            var c = m.CustomerGetAll();

            // Pass the collection to the view
            return View(c);
        }

        // GET: Customers/Details/5
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
        public ActionResult Create()
        {
            // Optionally, can create and send an object to the view
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        public ActionResult Create(CustomerAdd newItem)
        {
            // Notice the pattern for handling incoming data...
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

        // GET: Customers/Edit/5
        // Attention 12 - Edit customer info, "send the HTML Form to the browser user"
        public ActionResult Edit(int? id)
        {
            // Attempt to fetch the matching object
            var o = m.CustomerGetById(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Create and configure an "edit form"

                // Notice that o is a CustomerBase object
                // We must map it to a CustomerEditContactInfoForm object
                // Notice that the AutoMapper maps were defined in the Manager class
                var editForm = m.mapper.Map<CustomerEditContactInfoForm>(o);

                return View(editForm);
            }
        }

        // POST: Customers/Edit/5
        // Attention 13 - Edit customer info, "handle the data that was sent by the browser user"
        [HttpPost]
        public ActionResult Edit(int? id, CustomerEditContactInfo newItem)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                // Our "version 1" approach is to display the "edit form" again
                return RedirectToAction("edit", new { id = newItem.CustomerId });
            }

            if (id.GetValueOrDefault() != newItem.CustomerId)
            {
                // This appears to be data tampering, so redirect the user away
                return RedirectToAction("index");
            }

            // Attempt to do the update
            var editedItem = m.CustomerEditContactInfo(newItem);

            if (editedItem == null)
            {
                // There was a problem updating the object
                // Our "version 1" approach is to display the "edit form" again
                return RedirectToAction("edit", new { id = newItem.CustomerId });
            }
            else
            {
                // Show the details view, which will have the updated data
                return RedirectToAction("details", new { id = newItem.CustomerId });
            }
        }

        // GET: Customers/Delete/5
        // Attention 14 - Delete customer, show the confirmation HTML Form
        public ActionResult Delete(int? id)
        {
            var itemToDelete = m.CustomerGetById(id.GetValueOrDefault());

            if (itemToDelete == null)
            {
                // Don't leak info about the delete attempt
                // Simply redirect
                return RedirectToAction("index");
            }
            else
            {
                return View(itemToDelete);
            }
        }

        // POST: Customers/Delete/5
        // Attention 15 - Delete customer, handle the user intent
        [HttpPost]
        public ActionResult Delete(int? id, FormCollection collection)
        {
            var result = m.CustomerDelete(id.GetValueOrDefault());

            // "result" will be true or false
            // We probably won't do much with the result, because 
            // we don't want to leak info about the delete attempt

            // In the end, we should just redirect to the list view
            return RedirectToAction("index");
        }

    }
}
