using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using AssocOneToMany.Models;
using System.IO;
using Excel;

namespace AssocOneToMany.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

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

                // Attention 01 - Object mapper definitions

                cfg.CreateMap<Models.Team, Controllers.TeamBase>();

                // Attention 02 - Will use the mapping below to map the associated Player objects
                cfg.CreateMap<Models.Team, Controllers.TeamWithPlayers>();

                cfg.CreateMap<Models.Player, Controllers.PlayerBase>();

                // Attention 03 - Will use the mapping above to map the associated Team object
                cfg.CreateMap<Models.Player, Controllers.PlayerWithTeamInfo>();

                // Attention 04 - Will use the mapping above to map a property from the associated Team object
                cfg.CreateMap<Models.Player, Controllers.PlayerWithTeamName>();
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
        // Team

        public IEnumerable<TeamBase> TeamGetAll()
        {
            return mapper.Map<IEnumerable<TeamBase>>(ds.Teams.OrderBy(t => t.Name));
        }

        public TeamBase TeamGetById(int id)
        {
            // Attempt to fetch the object
            var o = ds.Teams.Find(id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<TeamBase>(o);
        }

        public TeamWithPlayers TeamGetByIdWithPlayers(int id)
        {
            // Attempt to fetch the object
            // Attention 16 - Fetch team, notice that we "include" the Players collection
            // When we use Include(), we cannot use Find(), we must use SingleOrDefault()
            var o = ds.Teams
                .Include("Players")
                .SingleOrDefault(t => t.Id == id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<TeamWithPlayers>(o);
        }

        // ############################################################
        // Player

        public IEnumerable<PlayerBase> PlayerGetAll()
        {
            return mapper.Map<IEnumerable<PlayerBase>>(ds.Players.OrderBy(p => p.PlayerName));
        }

        public IEnumerable<PlayerWithTeamInfo> PlayerGetAllWithTeamInfo()
        {
            // Attention 17 - Fetch players, notice that we "include" the team object
            var c = ds.Players
                .Include("Team")
                .OrderBy(p => p.PlayerName);

            // The mapper returns the whole team object with the results
            return mapper.Map<IEnumerable<PlayerWithTeamInfo>>(c);
        }

        public IEnumerable<PlayerWithTeamName> PlayerGetAllWithTeamName()
        {
            // Attention 18 - Fetch players, notice (again) that we "include" the team object
            var c = ds.Players
                .Include("Team")
                .OrderBy(p => p.PlayerName);

            // The mapper returns only the team name with the results
            return mapper.Map<IEnumerable<PlayerWithTeamName>>(c);
        }

        public PlayerBase PlayerGetById(int id)
        {
            // Attempt to fetch the object
            var o = ds.Players.Find(id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<PlayerBase>(o);
        }

        // ############################################################
        // Load data (one-time task)

        public void LoadData()
        {
            // If there's data, then exit
            if (ds.Teams.Count() > 0) { return; }

            // Load the teams first

            /*
            ds.Teams.Add(new Team
            {
                City = "Glendale, AZ",
                CodeName = "ARI",
                Conference = "NFC",
                Division = "West",
                Name = "Arizona Cardinals",
                Stadium = "University of Phoenix Stadium",
                YearFounded = 1920
            });
            */

            ds.Teams.Add(new Team
            {
                City = "Foxborough, MA",
                CodeName = "NE",
                Conference = "AFC",
                Division = "East",
                Name = "New England Patriots",
                Stadium = "Gillette Stadium",
                YearFounded = 1960
            });

            ds.Teams.Add(new Team
            {
                City = "Pittsburgh, PA",
                CodeName = "PIT",
                Conference = "AFC",
                Division = "North",
                Name = "Pittsburgh Steelers",
                Stadium = "Heinz Field",
                YearFounded = 1933
            });

            ds.Teams.Add(new Team
            {
                City = "Atlanta, GA",
                CodeName = "ATL",
                Conference = "NFC",
                Division = "South",
                Name = "Atlanta Falcons",
                Stadium = "Mercedez-Benz Stadium",
                YearFounded = 1965
            });

            ds.Teams.Add(new Team
            {
                City = "Green Bay, WI",
                CodeName = "GB",
                Conference = "NFC",
                Division = "North",
                Name = "Green Bay Packers",
                Stadium = "Lambeau Field",
                YearFounded = 1919
            });

            /*
            ds.Teams.Add(new Team
            {
                City = "Charlotte, NC",
                CodeName = "CAR",
                Conference = "NFC",
                Division = "South",
                Name = "Carolina Panthers",
                Stadium = "Bank of America Stadium",
                YearFounded = 1995
            });

            ds.Teams.Add(new Team
            {
                City = "Denver, CO",
                CodeName = "DEN",
                Conference = "AFC",
                Division = "West",
                Name = "Denver Broncos",
                Stadium = "Sports Authority Field at Mile High",
                YearFounded = 1960
            });
            */

            ds.SaveChanges();

            // Now, load the players

            // This uses a nice little library from Dietmar Schoder
            // http://www.codeproject.com/Tips/801032/Csharp-How-To-Read-xlsx-Excel-File-With-Lines-of 

            // File system path to the data file (in this project's App_Data folder)
            string path = HttpContext.Current.Server.MapPath("~/App_Data/NFLPlayoffTeams.xlsx");

            // Get or open the workbook
            var wb = Workbook.Worksheets(path);

            // Go through all the worksheets in the workbook
            for (int i = 0; i < wb.Count(); i++)
            {
                // Get a reference to the current worksheet
                var ws = wb.ElementAt(i);

                // Current team
                Team currentTeam = null;

                // Worksheets can't be referenced by worksheet (tab) name
                // Therefore, we'll have to go by index
                //if (i == 0) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "DEN"); }
                if (i == 0) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "PIT"); }
                //if (i == 1) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "CAR"); }
                if (i == 1) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "ATL"); }
                if (i == 2) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "NE"); }
                //if (i == 3) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "ARI"); }
                if (i == 3) { currentTeam = ds.Teams.SingleOrDefault(t => t.CodeName == "GB"); }

                // Now, go through the list of players
                // Start at index 1, ignore the header row
                for (int j = 1; j < ws.Rows.Count(); j++)
                {
                    // Get a reference to the cell collection
                    // This just makes the syntax that follows easier to work with
                    var c = ws.Rows[j].Cells;

                    // Add a new player
                    ds.Players.Add(new Player
                    {
                        BirthDate = DateTime.Parse(c[6].Text),
                        College = c[8].Text,
                        Height = c[4].Text,
                        PlayerName = c[1].Text,
                        Position = c[2].Text,
                        UniformNumber = (int)c[0].Amount,
                        Weight = (int)c[5].Amount,
                        YearsExperience = (int)c[7].Amount,
                        Team = currentTeam
                    });
                }

                // After each team's players are processed, save the changes
                ds.SaveChanges();
            }
            // Done
        }

        public bool RemoveData()
        {
            try
            {
                foreach (var p in ds.Players)
                {
                    ds.Entry(p).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                foreach (var t in ds.Teams)
                {
                    ds.Entry(t).State = System.Data.Entity.EntityState.Deleted;
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
}