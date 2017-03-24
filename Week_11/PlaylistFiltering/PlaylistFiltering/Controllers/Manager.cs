using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using PlaylistFiltering.Models;

namespace PlaylistFiltering.Controllers
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

                cfg.CreateMap<Models.Playlist, Controllers.PlaylistBase>();
                cfg.CreateMap<Models.Playlist, Controllers.PlaylistWithDetails>();

                cfg.CreateMap<Models.Track, Controllers.TrackBase>();
                cfg.CreateMap<Models.Track, Controllers.TrackWithGenre>();

                cfg.CreateMap<Models.Genre, Controllers.GenreBase>();

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
        // Playlist

        public IEnumerable<PlaylistBase> PlaylistGetAll()
        {
            // Playlist get all

            var o = ds.Playlists
                .Include("Tracks")
                .OrderBy(p => p.Name);

            return mapper.Map<IEnumerable<PlaylistBase>>(o);
        }

        public PlaylistWithDetails PlaylistGetByIdWithDetails(int id)
        {
            // Playlist get one

            // Attempt to fetch the object
            var o = ds.Playlists
                .Include("Tracks")
                .SingleOrDefault(p => p.PlaylistId == id);

            return (o == null) ? null : mapper.Map<PlaylistWithDetails>(o);
        }

        public PlaylistWithDetails PlaylistEditTracks(PlaylistEditTracks newItem)
        {
            // Playlist edit existing

            // Attempt to fetch the object
            var o = ds.Playlists
                .Include("Tracks")
                .SingleOrDefault(p => p.PlaylistId == newItem.PlaylistId);

            if (o == null)
            {
                return null;
            }
            else
            {
                // Update the object with the incoming values

                // First, clear out the existing collection
                o.Tracks.Clear();

                // Then, go through the incoming items
                // For each one, add to the fetched object's collection
                foreach (var item in newItem.TrackIds)
                {
                    var a = ds.Tracks.Find(item);
                    o.Tracks.Add(a);
                }

                ds.SaveChanges();

                return mapper.Map<PlaylistWithDetails>(o);
            }
        }

        // ############################################################
        // Track

        public IEnumerable<TrackBase> TrackGetAll()
        {
            // Track get all

            return mapper.Map<IEnumerable<TrackBase>>(ds.Tracks.OrderBy(t => t.Name));
        }

        // New method, get all tracks, include the genre info
        public IEnumerable<TrackWithGenre> TrackWithGenreGetAll()
        {
            var c = ds.Tracks.Include("Genre");

            return mapper.Map<IEnumerable<TrackWithGenre>>(c.OrderBy(t => t.Name));
        }

        public IEnumerable<TrackBase> TrackGetAllByPlaylistId(int id)
        {
            // Track get one

            // Attempt to fetch playlist
            var o = ds.Playlists.Include("Tracks").SingleOrDefault(p => p.PlaylistId == id);

            if (o == null)
            {
                return null;
            }
            else
            {
                return mapper.Map<IEnumerable<TrackBase>>(o.Tracks.OrderBy(t => t.Name));
            }
        }


        // Experimental
        // Not used in this assignment
        public IEnumerable<TrackBase> TrackGetAllByGenreId(int id)
        {
            // Attempt to fetch genre
            var o = ds.Genres.Include("Tracks").SingleOrDefault(g => g.GenreId == id);

            if (o == null)
            {
                return null;
            }
            else
            {
                return mapper.Map<IEnumerable<TrackBase>>(o.Tracks.OrderBy(t => t.Name));
            }
        }

        public IEnumerable<TrackBase> TrackGetAllByGenreIds(IEnumerable<int> ids)
        {
            // Create a collectio to hold the results
            var c = new List<TrackBase>();

            // Go through the incoming collection
            // Ensure they're unique, by using the Distinct() method

            foreach (var g in ids.Distinct())
            {
                c.Union(this.TrackGetAllByGenreId(g));
            }

            return c.OrderBy(t => t.Name);
        }


        // ############################################################
        // Genre

        // Not used in this assignment
        public IEnumerable<GenreBase> GenreGetAll()
        {
            return mapper.Map<IEnumerable<GenreBase>>(ds.Genres.OrderBy(g => g.Name));
        }






    }
}