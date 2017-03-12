using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using ManageAccounts.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace ManageAccounts.Controllers
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

        // User manager property...

        // Backing field for the property
        private ApplicationUserManager _userManager;

        // Getter and setter
        public ApplicationUserManager UserManager
        {
            get
            {
                // Null coalescing operator
                // https://msdn.microsoft.com/en-us/library/ms173224.aspx
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
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

                cfg.CreateMap<Models.ApplicationUser, Controllers.ApplicationUserBase>();
                cfg.CreateMap<Controllers.RequestUser, Controllers.ApplicationUserDetail>();
                cfg.CreateMap<Controllers.ApplicationUserDetail, Controllers.ApplicationUserEditForm>();
                cfg.CreateMap<Controllers.ApplicationUserEdit, Controllers.ApplicationUserDetail>();

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

        // ############################################################
        // User account management methods
        // ############################################################

        // Get All Users
        public IEnumerable<ApplicationUserBase> UsersGetAll()
        {
            // Fetch all users        
            var allUsers = UserManager.Users.ToList();

            if (allUsers == null)
            {
                return null;
            }

            var userList = new List<ApplicationUserBase>();
            foreach (var user in allUsers)
            {
                // Map the values all users to view model
                var appUser = mapper.Map<ApplicationUserBase>(user);
                var userClaims = user.Claims.Where
                     (c => c.ClaimType == ClaimTypes.Role).Select(roles => roles.ClaimValue).ToArray();

                // Add Role Claims
                appUser.Roles = userClaims;
                userList.Add(appUser);
            }
            return userList;
        }

        // Find a user based on a search string - partial
        public IEnumerable<ApplicationUserBase> FindUsers(string findString)
        {
            // Fetch all users
            var allUsers = UserManager.Users.ToList();
              
            // Copy the matching users to memory
            var matchingUsers = allUsers.Where(e => e.UserName.Contains(findString) || e.Email.Contains(findString));

            if (matchingUsers == null)
            {
                return null;
            }

            // Map the users to the view model
            var userList = new List<ApplicationUserBase>();
            foreach (var user in matchingUsers)
            {
                var appUser = mapper.Map<ApplicationUserBase>(user);
                var userClaims = user.Claims.Where
                     (c => c.ClaimType == ClaimTypes.Role).Select(roles => roles.ClaimValue).ToArray();

                appUser.Roles = userClaims;
                userList.Add(appUser);
            }

            return mapper.Map<IEnumerable<ApplicationUserBase>>(userList);
        }


        // Get User by Id
        public ApplicationUserDetail GetUserById(string id)
        {
            // Fetch the User by Id
            var user = UserManager.FindById(id);

            if (user == null)
            {
                return null;
            }

            // Initialize UserAccount
            var userIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie) as ClaimsIdentity;
            var claimsPrincipal = new ClaimsPrincipal(userIdentity);

            var userAccount = new RequestUser(claimsPrincipal);

            // Map user details
            var details = mapper.Map<ApplicationUserDetail>(userAccount);
            details.UserName = user.UserName;
            details.Email = user.UserName;
            details.Roles = userAccount.RoleClaims;

            return details;
        }


        // Delete User
        public void DeleteUser(string id)
        {
            var user = UserManager.FindById(id);

            // Initialize UserAccount
            var userIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie) as ClaimsIdentity;
            var claimsPrincipal = new ClaimsPrincipal(userIdentity);

            var userAccount = new RequestUser(claimsPrincipal);

            // Get all claims
            var claims = claimsPrincipal.Claims;
            // Set a flag for successful remove
            var check = true;
            // Remove all claims from user
            foreach (var claim in claims)
            {
                var r = UserManager.RemoveClaimAsync(user.Id, new Claim(claim.Type, claim.Value)).Result;
                if (!r.Succeeded) { check = false; }
            }

            // Finally remove the user
            if (check)
            {
                var result = UserManager.DeleteAsync(user).Result;
            }
        }

        // Edit User Claims - For Now Only Roles
        public ApplicationUserDetail ApplicationUserEdit(ApplicationUserEdit newItem)
        {
            var result = new IdentityResult();

            // Attempt to fetch the object
            var o = UserManager.FindById(newItem.Id);

            if (o == null)
            {
                return null;
            }

            var userIdentity = UserManager.CreateIdentity(o, DefaultAuthenticationTypes.ApplicationCookie) as ClaimsIdentity;
            var claimsPrincipal = new ClaimsPrincipal(userIdentity);
            var userAccount = new RequestUser(claimsPrincipal);

            // Remove all roles
            foreach (var role in userAccount.RoleClaims)
            {
                result = UserManager.RemoveClaimAsync(o.Id, new Claim(ClaimTypes.Role, role)).Result;
            }

            // If successful removal, Add Roles
            if (result.Succeeded)
            {
                foreach (var newRole in newItem.Roles)
                {
                    result = UserManager.AddClaimAsync(o.Id, new Claim(ClaimTypes.Role, newRole)).Result;
                }
                if (result.Succeeded)
                {
                    return mapper.Map<ApplicationUserDetail>(newItem);
                }
            }
            return null;
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