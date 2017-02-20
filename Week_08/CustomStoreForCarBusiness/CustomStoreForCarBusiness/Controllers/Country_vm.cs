using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomStoreForCarBusiness.Controllers
{
    // Attention 11 - View model classes, Country entity
    public class CountryBase
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Country Name")]
        public string Name { get; set; }
    }

}