using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using LINQIntroduction.Models;

namespace LINQIntroduction.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private DataContext ds = new DataContext();

        // AutoMapper components
        MapperConfiguration config;
        public IMapper mapper;

        public Manager()
        {
            // If necessary, add more constructor code here...

            // Configure the AutoMapper components
            config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Employee, EmployeeBase>();

                cfg.CreateMap<Models.Customer, Controllers.CustomerBase>();
                cfg.CreateMap<Controllers.CustomerAdd, Models.Customer>();

            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }

        // Add methods below
        // Controllers will call these methods
        // Ensure that the methods accept and deliver ONLY view model objects and collections
        // The collection return type is almost always IEnumerable<T>

        // Suggested naming convention: Entity + task/action
        // For example:
        // ProductGetAll()
        // ProductGetById()
        // ProductAdd()
        // ProductEdit()
        // ProductDelete()

        // Get all customers
        public IEnumerable<CustomerBase> CustomerGetAll()
        {
            // The ds object is the data store
            // It has a collection for each entity it manages

            return mapper.Map<IEnumerable<CustomerBase>>(ds.Customers);
        }

        // Attention 04 - Most of the interesting code is in this Manager class
        // We introduce common LINQ methods for querying, filtering, and sorting

        // Get one customer by its identifier
        public CustomerBase CustomerGetById(int id)
        {
            // Original statement, using the Find() method
            //var o = ds.Customers.Find(id);

            // Attention 06 - Replacement statement, using the SingleOrDefault method
            var o = ds.Customers.SingleOrDefault(c => c.CustomerId == id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<CustomerBase>(o);
        }

        // Attention 07 - Get customers, filter by all or part of a country name
        public IEnumerable<CustomerBase> CustomerGetAllByCountry(string item)
        {
            // Don't trust incoming data - clean it before using it
            // The following could be done as a separate statement, as shown,
            // or inline with the query - it's your choice
            item = item.Trim().ToLower();

            // Filtered fetch, using the Where method
            // Also uses the Contains method, useful for strings
            // Do a case-insensitive comparison
            var c = ds.Customers.Where(cn => cn.Country.ToLower().Contains(item));

            return mapper.Map<IEnumerable<CustomerBase>>(c);
        }

        // Attention 08 - Get customers, filter by all or part of a person or company name
        public IEnumerable<CustomerBase> CustomerGetAllByName(string item)
        {
            // Be sure to sanitize (trim) the incoming string item before comparing
            // Do a case-insensitive comparison
            item = item.Trim().ToLower();

            // We're going to do three separate searches - first name, last name, company name
            // And then combine the search results

            var first = ds.Customers.Where(fn => fn.FirstName.ToLower().Contains(item));

            var last = ds.Customers.Where(ln => ln.LastName.ToLower().Contains(item));

            var comp = ds.Customers.Where(cn => cn.Company.ToLower().Contains(item));

            // The Union method helps join collections together
            // Combine the results, eliminating duplicates
            var results = first.Union(last).Union(comp);

            // Return sorted
            return mapper.Map<IEnumerable<CustomerBase>>
                (results.OrderBy(ln => ln.LastName).ThenBy(fn => fn.FirstName));
        }

        

    }
}