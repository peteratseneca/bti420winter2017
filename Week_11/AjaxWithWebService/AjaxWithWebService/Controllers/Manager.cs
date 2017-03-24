using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using AjaxWithWebService.Models;

namespace AjaxWithWebService.Controllers
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

                cfg.CreateMap<Models.Artist, Controllers.ArtistBase>();
                cfg.CreateMap<Models.Album, Controllers.AlbumBase>();
                cfg.CreateMap<Models.Track, Controllers.TrackBase>();

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





        // Attention - 1 - Manager methods that fetch and return collections
        // Also, study the view model classes

        public IEnumerable<ArtistBase> ArtistGetAll()
        {
            return mapper.Map<IEnumerable<ArtistBase>>(ds.Artists.OrderBy(a => a.Name));
        }

        public IEnumerable<AlbumBase> AlbumGetAll()
        {
            var c = ds.Albums
                .Include("Artist")
                .OrderBy(a => a.Title);

            return mapper.Map<IEnumerable<AlbumBase>>(c);
        }

        public IEnumerable<TrackBase> TrackGetAll()
        {
            var c = ds.Tracks
                .Include("Album.Artist")
                .Include("MediaType")
                .Include("Genre");

            return mapper.Map<IEnumerable<TrackBase>>(c.OrderBy(t => t.Name));
        }






    }
}