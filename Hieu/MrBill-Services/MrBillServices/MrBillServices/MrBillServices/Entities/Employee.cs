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
    
    public partial class Employee
    {
        public Employee()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    
        public int EmployeeID { get; set; }
        public string EmployeeIdentity { get; set; }
        public int UserID { get; set; }
    
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}