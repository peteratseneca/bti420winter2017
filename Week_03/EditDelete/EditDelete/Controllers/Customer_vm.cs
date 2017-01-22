using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EditDelete.Controllers
{
    // Attention 01 - Create the view models that we will need
    // CustomerAdd
    // Then, CustomerBase inherits from CustomerAdd
    // We also need a model for the HTML Form
    // And finally, a model for the data that's entered by the browser user

    public class CustomerAdd
    {
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

    // Notice the inheritance pattern here - it works in this situation
    public class CustomerBase : CustomerAdd
    {
        public CustomerBase() { }

        // Reminder... must use the [Key] data annotation here
        [Key]
        public int CustomerId { get; set; }
    }

    // Attention 02 - View model for the "customer edit" HTML Form

    public class CustomerEditContactInfoForm
    {
        public CustomerEditContactInfoForm() { }

        [Key]
        public int CustomerId { get; set; }

        // In the view, we will display this info
        [Required]
        [StringLength(40)]
        public string FirstName { get; set; }

        // In the view, we will display this info
        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        // In the view, we will display this info
        [StringLength(80)]
        public string Company { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [StringLength(24)]
        public string Fax { get; set; }

        [Required]
        [StringLength(60)]
        public string Email { get; set; }
    }

    // Attention 03 - View model for the data that's entered by the browser user

        // It may be possible to use an inheritance with these kinds of view model class pairs
    // Carefully consider whether it makes sense by inspecting the properties and considering the use cases
    // Don't always force the issue - if it's easy and it makes sense, do it

    public class CustomerEditContactInfo
    {
        public CustomerEditContactInfo() { }

        [Key]
        public int CustomerId { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [StringLength(24)]
        public string Fax { get; set; }

        [Required]
        [StringLength(60)]
        public string Email { get; set; }
    }

}

