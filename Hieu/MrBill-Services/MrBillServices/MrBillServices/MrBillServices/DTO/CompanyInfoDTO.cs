using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class CompanyInfoDTO
    {
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string VATCode { get; set; }
    }
}