using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using System.ComponentModel.DataAnnotations;

namespace CustomStoreForCarBusiness.Models
{
    // Add your design model classes below

    // Follow these rules or conventions:

    // To ease other coding tasks, the name of the 
    //   integer identifier property should be "Id"
    // Collection properties (including navigation properties) 
    //   must be of type ICollection<T>
    // Valid data annotations are pretty much limited to [Required] and [StringLength(n)]
    // Required to-one navigation properties must include the [Required] attribute
    // Do NOT configure scalar properties (e.g. int, double) with the [Required] attribute
    // Initialize DateTime and collection properties in a default constructor

    // Attention 01 - Class that defines a role claim for the app
    // The Home controller Index() method calls the LoadData() method in the Manager
    // The LoadData() method defines all the allowable role claims for the app
    public class RoleClaim
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
    }

    // Attention 02 - Car business data model, Country design model class
    public class Country
    {
        public Country()
        {
            Manufacturers = new List<Manufacturer>();
        }

        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        public ICollection<Manufacturer> Manufacturers { get; set; }
    }

    // Attention 03 - Manufacturer design model class
    public class Manufacturer
    {
        public Manufacturer()
        {
            YearStarted = 1950;
            Vehicles = new List<Vehicle>();
        }

        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }
        public int YearStarted { get; set; }

        [Required]
        public Country Country { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }

    // Attention 04 - Vehicle design model class
    public class Vehicle
    {
        public Vehicle()
        {
            ModelYear = DateTime.Now.Year;
            MSRP = 20000;
        }

        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Model { get; set; }

        [Required, StringLength(100)]
        public string Trim { get; set; }

        public int ModelYear { get; set; }
        public int MSRP { get; set; }

        [Required]
        public Manufacturer Manufacturer { get; set; }
    }

}
