using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using CustomStoreForCarBusiness.Models;
using System.Security.Claims;

namespace CustomStoreForCarBusiness.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper components
        MapperConfiguration config;
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Employee, EmployeeBase>();

                // Object mapper definitions

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();

                // Attention 14 - AutoMapper definitions

                cfg.CreateMap<Models.Country, Controllers.CountryBase>();

                cfg.CreateMap<Models.Manufacturer, Controllers.ManufacturerBase>();
                cfg.CreateMap<Models.Manufacturer, Controllers.ManufacturerWithDetail>();

                cfg.CreateMap<Models.Vehicle, Controllers.VehicleBase>();
                cfg.CreateMap<Models.Vehicle, Controllers.VehicleWithDetail>();
                cfg.CreateMap<Controllers.VehicleAdd, Models.Vehicle>();
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

        // ############################################################
        // RoleClaim

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
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

        // Attention 26 - Better version of "get all", because it fetches associated data
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

        // Attention 27 - Better version of "get one", because it fetches associated data
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





        // Add some programmatically-generated objects to the data store
        // Can write one method, or many methods - your decision
        // The important idea is that you check for existing data first
        // Call this method from a controller action/method

        // Attention 17 - Load Country data
        public bool LoadDataCountry()
        {
            // Return if there's existing data
            if (ds.Countries.Count() > 0) { return false; }

            // Otherwise...
            // Create and add objects
            ds.Countries.Add(new Country { Name = "Germany" });
            ds.Countries.Add(new Country { Name = "South Korea" });
            ds.Countries.Add(new Country { Name = "Japan" });
            ds.Countries.Add(new Country { Name = "United States of America" });

            // Save changes
            ds.SaveChanges();

            return true;
        }

        // Attention 18 - Load Manufacturer data
        public bool LoadDataManufacturer()
        {
            // Return if there's existing data
            if (ds.Manufacturers.Count() > 0) { return false; }

            // Otherwise...
            // Create and add objects

            // Germany...
            // Fetch the country object, because we need it
            var germany = ds.Countries.SingleOrDefault(c => c.Name == "Germany");
            if (germany == null) { return false; }
            // Continue...
            ds.Manufacturers.Add(new Manufacturer { Country = germany, Name = "BMW AG", YearStarted = 1916 });
            ds.Manufacturers.Add(new Manufacturer { Country = germany, Name = "Daimler AG", YearStarted = 1926 });
            ds.Manufacturers.Add(new Manufacturer { Country = germany, Name = "Volkswagen AG", YearStarted = 1937 });
            ds.SaveChanges();

            // South Korea...
            var korea = ds.Countries.SingleOrDefault(c => c.Name == "South Korea");
            if (korea == null) { return false; }
            ds.Manufacturers.Add(new Manufacturer { Country = korea, Name = "Hyundai Motor Company", YearStarted = 1968 });
            ds.Manufacturers.Add(new Manufacturer { Country = korea, Name = "Kia Motors Company", YearStarted = 1944 });
            ds.SaveChanges();

            // Japan...
            var japan = ds.Countries.SingleOrDefault(c => c.Name == "Japan");
            if (japan == null) { return false; }
            ds.Manufacturers.Add(new Manufacturer { Country = japan, Name = "Honda Motor Co. Ltd.", YearStarted = 1946 });
            ds.Manufacturers.Add(new Manufacturer { Country = japan, Name = "Mazda Motor Corporation", YearStarted = 1920 });
            ds.Manufacturers.Add(new Manufacturer { Country = japan, Name = "Toyota Motor Company", YearStarted = 1937 });
            ds.SaveChanges();

            // United States of America
            var usa = ds.Countries.SingleOrDefault(c => c.Name == "United States of America");
            if (usa == null) { return false; }
            ds.Manufacturers.Add(new Manufacturer { Country = usa, Name = "Chrysler", YearStarted = 1925 });
            ds.Manufacturers.Add(new Manufacturer { Country = usa, Name = "Ford Motor Company", YearStarted = 1903 });
            ds.Manufacturers.Add(new Manufacturer { Country = usa, Name = "General Motors", YearStarted = 1908 });
            ds.SaveChanges();

            return true;
        }

        // Attention 18 - Load Vehicle data
        public bool LoadDataVehicle()
        {
            // Return if there's existing data
            if (ds.Vehicles.Count() > 0) { return false; }

            // Otherwise...
            // Create and add objects

            // Honda...
            var honda = ds.Manufacturers.SingleOrDefault(m => m.Name == "Honda Motor Co. Ltd.");
            if (honda == null) { return false; }
            ds.Vehicles.Add(new Vehicle { Manufacturer = honda, Model = "Accord", Trim = "Sedan LX", ModelYear = 2017, MSRP = 24150 });
            ds.Vehicles.Add(new Vehicle { Manufacturer = honda, Model = "Civic", Trim = "Coupe Si", ModelYear = 2017, MSRP = 26850 });
            ds.Vehicles.Add(new Vehicle { Manufacturer = honda, Model = "CR-V", Trim = "EX", ModelYear = 2017, MSRP = 32190 });

            // Save changes
            ds.SaveChanges();

            return true;
        }

        public bool LoadData()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Monitor the progress
            bool done = false;

            // ############################################################
            // Role claims

            if (ds.RoleClaims.Count() == 0)
            {
                // Add role claims here

                ds.RoleClaims.Add(new RoleClaim { Name = "VicePresident" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Manager" });
                ds.RoleClaims.Add(new RoleClaim { Name = "SalesRep" });
                ds.RoleClaims.Add(new RoleClaim { Name = "ClericalSupport" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Mechanic" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Admin" });

                ds.SaveChanges();
                done = true;
            }

            return done;
        }

        // Attention 88 - Example method that will remove data from the store
        public bool RemoveData()
        {
            try
            {
                foreach (var e in ds.RoleClaims)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Attention 89 - Example method that will remove the database
        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    // New "RequestUser" class for the authenticated user
    // Includes many convenient members to make it easier to render user account info
    // Study the properties and methods, and think about how you could use it

    // How to use...

    // In the Manager class, declare a new property named User
    //public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value
    //User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }
        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }
        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }
        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

}