using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using AjaxItemSelect.Models;

namespace AjaxItemSelect.Controllers
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





        // Attention - 01 - Manager methods that fetch artists, albums, and tracks

        // All artists
        public IEnumerable<ArtistBase> ArtistGetAll()
        {
            return mapper.Map<IEnumerable<ArtistBase>>(ds.Artists.OrderBy(a => a.Name));
        }

        // Get all albums, only for a specific artist
        public IEnumerable<AlbumBase> AlbumGetAllForArtist(int id)
        {
            var a = ds.Artists
                .Include("Albums")
                .SingleOrDefault(art => art.ArtistId == id);

            if (a == null)
            {
                return null;
            }
            else
            {
                return mapper.Map<IEnumerable<AlbumBase>>(a.Albums.OrderBy(alb => alb.Title));
            }
        }

        // Get all tracks, only for a specific album
        public IEnumerable<TrackBase> TrackGetAllForAlbum(int id)
        {
            var a = ds.Albums
                .Include("Tracks")
                .SingleOrDefault(alb => alb.AlbumId == id);

            if (a == null)
            {
                return null;
            }
            else
            {
                return mapper.Map<IEnumerable<TrackBase>>(a.Tracks.OrderBy(t => t.Name));
            }
        }

        // A simple method that does something with the user-submitted data
        public SelectedDataText DoSomething(UserSelectedData newItem)
        {
            var o = new SelectedDataText();

            // Artist
            var artist = ds.Artists.Find(newItem.ArtistId);
            o.ArtistName = (artist == null) ? "(not found)" : artist.Name;

            // Album
            var album = ds.Albums
                .Include("Tracks")
                .SingleOrDefault(a => a.AlbumId == newItem.AlbumId);
            o.AlbumTitle = (album == null) ? "(not found)" : album.Title;

            // Tracks
            if (album == null)
            {
                o.TrackNames.Add("(not found)");
            }
            else
            {
                // Get the track names for the selected tracks
                foreach (var item in album.Tracks)
                {
                    if (newItem.TrackIds.Contains(item.TrackId))
                    {
                        o.TrackNames.Add(item.Name);
                    }
                }
            }

            return o;
        }






    }
}