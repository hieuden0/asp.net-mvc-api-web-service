using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MrBillServices.DTO
{
    [DataContract]
    public class ScraperLogDTO
    {
        [DataMember]
        public int SupplierId { get; set; }
        [DataMember]
        public string SupplierName { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string SignUpUrl { get; set; }
        [DataMember]
        public string ResetPasswordUrl { get; set; }
    }
}