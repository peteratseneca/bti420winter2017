using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using AssocAddEdit.Models;

namespace AssocAddEdit.Controllers
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
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Employee, EmployeeBase>();

                // Attention 03 - Object mapper definitions

                cfg.CreateMap<Models.Country, Controllers.CountryBase>();

                cfg.CreateMap<Models.Manufacturer, Controllers.ManufacturerBase>();
                cfg.CreateMap<Models.Manufacturer, Controllers.ManufacturerWithDetail>();

                cfg.CreateMap<Models.Vehicle, Controllers.VehicleBase>();
                cfg.CreateMap<Models.Vehicle, Controllers.VehicleWithDetail>();
                cfg.CreateMap<Controllers.VehicleAdd, Models.Vehicle>();

                // Attention 04 - Notice the definitions that help build the HTML Forms
                cfg.CreateMap<Controllers.VehicleBase, Controllers.VehicleEditForm>();
                cfg.CreateMap<Controllers.VehicleWithDetail, Controllers.VehicleEditForm>();
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
        // Country

        // Attention 07 - Get all country objects
        // Country objects were created with code, and then the code was deleted

        public IEnumerable<CountryBase> CountryGetAll()
        {
            return mapper.Map<IEnumerable<CountryBase>>(ds.Countries.OrderBy(c => c.Name));
        }

        // ############################################################
        // Manufacturer

        public IEnumerable<ManufacturerBase> ManufacturerGetAll()
        {
            return mapper.Map<IEnumerable<ManufacturerBase>>(ds.Manufacturers.OrderBy(m => m.Name));
        }

        public IEnumerable<ManufacturerWithDetail> ManufacturerGetAllWithDetail()
        {
            var c = ds.Manufacturers.Include("Country");

            return mapper.Map<IEnumerable<ManufacturerWithDetail>>(c.OrderBy(m => m.Name));
        }

        public ManufacturerBase ManufacturerGetById(int id)
        {
            // Attempt to fetch the object
            var o = ds.Manufacturers.Find(id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<ManufacturerBase>(o);
        }

        public ManufacturerWithDetail ManufacturerGetByIdWithDetail(int id)
        {
            var o = ds.Manufacturers.Include("Country").Include("Vehicles")
                .SingleOrDefault(m => m.Id == id);

            return mapper.Map<ManufacturerWithDetail>(o);
        }

        // ############################################################
        // Vehicle

        public IEnumerable<VehicleBase> VehicleGetAll()
        {
            return mapper.Map<IEnumerable<VehicleBase>>(ds.Vehicles.OrderBy(v => v.Model).ThenBy(v => v.Trim));
        }

        public IEnumerable<VehicleWithDetail> VehicleGetAllWithDetail()
        {
            var c = ds.Vehicles.Include("Manufacturer.Country");

            return mapper.Map<IEnumerable<VehicleWithDetail>>(c.OrderBy(v => v.Model).ThenBy(v => v.Trim));
        }

        public VehicleBase VehicleGetById(int id)
        {
            // Attempt to fetch the object
            var o = ds.Vehicles.Find(id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<VehicleBase>(o);
        }

        public VehicleWithDetail VehicleGetByIdWithDetail(int id)
        {
            var o = ds.Vehicles.Include("Manufacturer.Country")
                .SingleOrDefault(v => v.Id == id);

            return mapper.Map<VehicleWithDetail>(o);
        }

        public VehicleWithDetail VehicleAdd(VehicleAdd newItem)
        {
            // This method is called from the Vehicles controller...
            // ...AND the Manufacturers controller

            // When adding an object with a required to-one association,
            // MUST fetch the associated object first

            // Attempt to find the associated object
            var a = ds.Manufacturers.Find(newItem.ManufacturerId);

            if (a == null)
            {
                return null;
            }
            else
            {
                // Attempt to add the new item
                var addedItem = ds.Vehicles.Add(mapper.Map<Vehicle>(newItem));
                // Set the associated item property
                addedItem.Manufacturer = a;
                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<VehicleWithDetail>(addedItem);
            }
        }

        public VehicleWithDetail VehicleEditMSRP(VehicleEdit newItem)
        {
            // Attempt to fetch the object

            // When editing an object with a required to-one association,
            // MUST fetch its associated object
            var o = ds.Vehicles.Include("Manufacturer")
                .SingleOrDefault(v => v.Id == newItem.Id);

            if (o == null)
            {
                // Problem - item was not found, so return
                return null;
            }
            else
            {
                // Update the object with the incoming values
                ds.Entry(o).CurrentValues.SetValues(newItem);
                ds.SaveChanges();

                // Prepare and return the object
                return mapper.Map<VehicleWithDetail>(o);
            }
        }

        public bool VehicleDelete(int id)
        {
            // Attempt to fetch the object to be deleted
            var itemToDelete = ds.Vehicles.Find(id);

            if (itemToDelete == null)
            {
                return false;
            }
            else
            {
                // Remove the object
                ds.Vehicles.Remove(itemToDelete);
                ds.SaveChanges();

                return true;
            }
        }

    }
}