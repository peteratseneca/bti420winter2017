using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using AddNew.Models;

namespace AddNew.Controllers
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

                // Attention 06 - Mapping from Customer to CustomerBase
                cfg.CreateMap<Models.Customer, Controllers.CustomerBase>();

                // Attention 07 - Add a mapping from CustomerAdd to Customer
                // Handles incoming data from the browser user
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

        // ############################################################
        // Customer
        // ############################################################

        // Attention 11 - Get all customers
        public IEnumerable<CustomerBase> CustomerGetAll()
        {
            // The ds object is the data store
            // It has a collection for each entity it manages

            return mapper.Map<IEnumerable<CustomerBase>>(ds.Customers);
        }

        // Attention 12 - Get one customer by its identifier
        public CustomerBase CustomerGetById(int id)
        {
            // Attempt to fetch the object
            var o = ds.Customers.Find(id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<CustomerBase>(o);
        }

        // Attention 13 - Add new customer
        public CustomerBase CustomerAdd(CustomerAdd newItem)
        {
            // Attempt to add the new item
            // Notice how we map the incoming data to the design model object
            var addedItem = ds.Customers.Add(mapper.Map<Customer>(newItem));
            ds.SaveChanges();

            // If successful, return the added item, mapped to a view model object
            return (addedItem == null) ? null : mapper.Map<CustomerBase>(addedItem);
        }






    }
}