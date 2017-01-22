using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using System.ComponentModel.DataAnnotations;

namespace DataAnnotations.Controllers
{
    // Attention 06 - View model classes for the VEHICLE (car, truck) entity

    // There are three classes in this source code file...

    // VehicleAdd and VehicleBase go together, as you have recently learned
    // They have some nice data annotations, for both INPUT and OUTPUT tasks

    // VehicleAddPlain has NO data annotations, which is a bad idea

    // Attention 07 - VehicleAdd view model class, with some nice rich data annotations
    // Notice the following...
    // Required, StringLength
    // Display, DataType
    // Range

    public class VehicleAdd
    {
        public VehicleAdd()
        {
            DateAvailable = DateTime.Now;
            ModelYear = DateAvailable.Year;
        }

        [Required]
        [StringLength(100)]
        [Display(Name = "Manufacturer name")]
        public string Manufacturer { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Vehicle model name")]
        public string ModelName { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Trim level")]
        public string TrimLevel { get; set; }

        [Range(1885,2285)]
        [Display(Name ="Model year")]
        public int ModelYear { get; set; }

        [Required]
        [Display(Name = "Public availability date")]
        [DataType(DataType.Date)]
        public DateTime DateAvailable { get; set; }

        [Range(1,50)]
        [Display(Name = "Number of seats")]
        public int Seats { get; set; }

        [Range(1.0, 50.0)]
        [Display(Name = "Fuel consumption, L/100km, city")]
        public double ConsumptionCity { get; set; }

        [Range(1.0, 50.0)]
        [Display(Name = "Fuel consumption, L/100km, highway")]
        public double ConsumptionHighway { get; set; }
    }

    public class VehicleBase : VehicleAdd
    {
        public VehicleBase() { }

        public int Id { get; set; }
    }

    public class VehicleAddPlain
    {
        public VehicleAddPlain()
        {
            DateAvailable = DateTime.Now;
            ModelYear = DateAvailable.Year;
        }

        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string TrimLevel { get; set; }
        public int ModelYear { get; set; }
        public DateTime DateAvailable { get; set; }
        public int Seats { get; set; }
        public double ConsumptionCity { get; set; }
        public double ConsumptionHighway { get; set; }

    }
}
