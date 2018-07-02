using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class PaymentTypeDTO
    {
        [DataMember]
        public int PaymentId { get; set; }
        [DataMember]
        public string PaymentName { get; set; }
    }
}