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
    
    public partial class Project
    {
        public Project()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    
        public int ProjectID { get; set; }
        public string No { get; set; }
        public int UserID { get; set; }
    
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
