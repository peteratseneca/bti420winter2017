using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssocAddEdit.Controllers
{
    // Attention 05 - Study the All_vm class diagram and its "readme" text, for more info/explanations

    // Attention 06 - Country view model class
    public class CountryBase
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Country Name")]
        public string Name { get; set; }
    }

}