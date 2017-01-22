using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using System.ComponentModel.DataAnnotations;

namespace DataAnnotations.Controllers
{
    // Attention 01 - View model classes for the ACCOUNT entity

    // There are three classes in this source code file...
    
    // AccountAdd and AccountBase go together, as you have recently learned
    // They have some nice data annotations, for both INPUT and OUTPUT tasks

    // AccountAddPlain has NO data annotations, which is a bad idea

    // Attention 02 - AccountAdd view model class, with some nice rich data annotations
    // Notice the following...
    // Required, StringLength
    // Display, DataType
    // RegularExpression
    // Compare

    public class AccountAdd
    {
        public AccountAdd()
        {
            DateOfBirth = DateTime.Now.AddYears(-25);
        }

        // Attention 03 - Coding style recommendation:
        // Separate properties that have data annotations with a blank line
        // This makes the code more readable

        [Required, StringLength(100, MinimumLength = 2)]
        [Display(Name = "First (given) name(s)")]
        public string FirstName { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        [Display(Name ="Last (family) name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Web site address")]
        [DataType(DataType.Url)]
        public string Website { get; set; }

        [Required, StringLength(1000)]
        [Display(Name = "Tell us a little about yourself")]
        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Password")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&'])[^ ]{8,}", ErrorMessage ="Password must be 8+ characters, have 1+ digits, 1+ upper-case characters, 1+ lower-case characters, and 1+ special characters ( ! @ # $ % ^ &)")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&'])[^ ]{8,}", ErrorMessage = "Password must be 8+ characters, have 1+ digits, 1+ upper-case characters, 1+ lower-case characters, and 1+ special characters ( ! @ # $ % ^ &)")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordCompare { get; set; }
    }

    public class AccountBase : AccountAdd
    {
        public AccountBase() { }

        public int Id { get; set; }
    }

    public class AccountAddPlain
    {
        public AccountAddPlain()
        {
            DateOfBirth = DateTime.Now.AddYears(-25);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string AboutMe { get; set; }
        public string Password { get; set; }
        public string PasswordCompare { get; set; }
    }
}
