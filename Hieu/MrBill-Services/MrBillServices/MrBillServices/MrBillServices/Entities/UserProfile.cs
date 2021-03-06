//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MrBillServices.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.CostCenters = new HashSet<CostCenter>();
            this.Emails = new HashSet<Email>();
            this.Employees = new HashSet<Employee>();
            this.Projects = new HashSet<Project>();
            this.Settings = new HashSet<Setting>();
            this.UserSupplierInfoes = new HashSet<UserSupplierInfo>();
            this.webpages_Roles = new HashSet<webpages_Roles>();
            this.Transactions = new HashSet<Transaction>();
            this.Transactions1 = new HashSet<Transaction>();
        }
    
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public int CompanyID { get; set; }
        public int UserRoleID { get; set; }
        public string Country { get; set; }
    
        public virtual CompanyInfo CompanyInfo { get; set; }
        public virtual ICollection<CostCenter> CostCenters { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Setting> Settings { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual ICollection<UserSupplierInfo> UserSupplierInfoes { get; set; }
        public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Transaction> Transactions1 { get; set; }
    }
}
