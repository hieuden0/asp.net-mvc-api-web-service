using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace MrBill_MVC.Models
{
    public class TravelModel
    {
        public IEnumerable<SelectListItem> CompanyName { get; set; }

        [Required]
        [Display(Name = "Anändarnamn")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Lösenord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

		[Required]
        [Display(Name = "Bekräfta lösenord")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }
		 
		[Display(Name = "Företag")]
	    public string Company { get; set; }

		[Display(Name = "Adress")]
		public string Adress { get; set; }

		[Display(Name = "Stad")]
		public string City { get; set; }

		[Display(Name = "Postnummer")]
		public string Zip { get; set; }

		[Display(Name = "Land")]
		public string Country { get; set; }

    }


}
