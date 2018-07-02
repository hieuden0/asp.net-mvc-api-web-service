using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace MrBill_MVC.Models
{


    public class UserModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public class AddressInfo
        {
            public string Address;
            public string Address2;
            public string City;
            public string CoAddress;
            public string CoAddress2;
            public string Country;
            public string PostalCode;
        }
        public string Email { get; set; }

        public string Phone { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "Nytt lösenord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Bekräfta lösenord")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }
    public class LostPasswordModel
    {
        [Required(ErrorMessage = "Skriv in mail för att återställa lösenordet")]
        [Display(Name = "Email")]

        [EmailAddress(ErrorMessage = "Skriv in en giltig email")]
        public string Email { get; set; }
    }
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Name")]
        public string UserName { get; set; }
        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} måste minst innehålla {2} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
 
        [Required]
        [EmailAddress]
        [Display(Name = "Namn")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }

    public class BigRegisterModel
    {
        public UserDataModel UserDataModel { get; set; }
        public RegisterModel RegisterModel { get; set; }
    }

    public class UserDataModel
    {
        public string Name;
        public string Adress;
        public string City;
    }
    public class RegisterModel
    {
        [Required] 
        [Display(Name = "Förnamn")]
        public string FirstName;

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName;

        [Required]
        [Display(Name = "Efternamn")]
        public int UserRoleId;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string UserName { get; set; }

		[Required]
        [StringLength(100, ErrorMessage = "{0} måste minst innehålla {2} tecken", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösendord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenordet")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte")]
        public string ConfirmPassword { get; set; }

    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
