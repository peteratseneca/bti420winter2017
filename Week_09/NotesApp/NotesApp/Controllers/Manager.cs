using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using NotesApp.Models;
using System.Security.Claims;

namespace NotesApp.Controllers
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

                cfg.CreateMap<Models.Note, Controllers.NoteBase>();
                cfg.CreateMap<Controllers.NoteAdd, Models.Note>();
                cfg.CreateMap<Controllers.NoteBase, Controllers.NoteEditForm>();
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
        // Note

        // Attention - 2 - Most of the logic is in this Manager class
        // The methods will pay attention to the current user information

        public IEnumerable<NoteBase> NoteGetAll()
        {
            // Attention - 3 - Get all, for the authenticated user only
            var c = ds.Notes.Where(n => n.Owner == User.Name);

            return mapper.Map<IEnumerable<NoteBase>>(c.OrderByDescending(n => n.DateCreated));
        }

        public NoteBase NoteGetById(int id)
        {
            // Attention - 4 - Get one, for the authenticated user only
            var o = ds.Notes.SingleOrDefault
                (n => n.Id == id && n.Owner == User.Name);

            return (o == null) ? null : mapper.Map<NoteBase>(o);
        }

        public IEnumerable<NoteBase> NoteGetAllByTitle(string text)
        {
            // Search for partial match in title property, case-insensitive
            // Future
            throw new NotImplementedException();
        }

        public IEnumerable<NoteBase> NoteGetAllByContent(string text)
        {
            // Search for partial match in content property, case-insensitive
            // Future
            throw new NotImplementedException();
        }

        public NoteBase NoteAdd(NoteAdd newItem)
        {
            // Attention - 5 - Add new is NOT restricted
            var addedItem = ds.Notes.Add(mapper.Map<Note>(newItem));
            // Assign the owner
            addedItem.Owner = User.Name;

            ds.SaveChanges();

            return (addedItem == null) ? null : mapper.Map<NoteBase>(addedItem);
        }

        public NoteBase NoteEdit(NoteEdit newItem)
        {
            // Attention - 6 - Edit existing, for the authenticated user only

            // Attempt to fetch the object
            // Can either do a two-condition fetch, or test it in the "if" statement
            // In this method, we'll do it in the following statement
            var o = ds.Notes.SingleOrDefault
                (n => n.Id == newItem.Id && n.Owner == User.Name);

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
                return mapper.Map<NoteBase>(o);
            }
        }

        public bool NoteDelete(int id)
        {
            // Attention - 7 - Delete item, for the authenticated user only

            // Attempt to fetch the object to be deleted
            // Can either do a two-condition fetch, or test it in the "if" statement
            // In this method, we'll do it in the "if" statement
            var itemToDelete = ds.Notes.Find(id);

            if (itemToDelete == null || itemToDelete.Owner != User.Name)
            {
                return false;
            }
            else
            {
                // Remove the object
                ds.Notes.Remove(itemToDelete);
                ds.SaveChanges();

                return true;
            }
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






        // Add some programmatically-generated objects to the data store
        // Can write one method, or many methods - your decision
        // The important idea is that you check for existing data first
        // Call this method from a controller action/method

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

                //ds.SaveChanges();
                //done = true;
            }

            return done;
        }

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