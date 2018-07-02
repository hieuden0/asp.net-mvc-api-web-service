using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class SupplierInfoDTO
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