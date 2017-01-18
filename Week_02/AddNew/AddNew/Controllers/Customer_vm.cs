using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// added...
using System.ComponentModel.DataAnnotations;

namespace AddNew.Controllers
{
    // Attention 01 - Revised view models for Customer, "Add" and "Base
    // Based on the Customer class in the Models folder

    // First, define the "Add" view model, which has most of the fields, except for the store-generated identifier
    // Then, define the "Base" view model, which inherits from "Add"

    public class CustomerAdd
    {
        // Constructor, for setting reasonable initial values
        public CustomerAdd() { }

        [Required]
        [StringLength(40)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(80)]
        public string Company { get; set; }

        [StringLength(70)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(40)]
        public string State { get; set; }

        [StringLength(40)]
        public string Country { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [StringLength(24)]
        public string Fax { get; set; }

        [Required]
        [StringLength(60)]
        public string Email { get; set; }
    }

    // Attention 02 - Notice that "Base" inherits from "Add"
    public class CustomerBase : CustomerAdd
    {
        // Constructor, for setting reasonable initial values
        public CustomerBase() { }

        // Notice that we must identify the key/identifier with a [Key] data annotation
        [Key]
        public int CustomerId { get; set; }
    }
}