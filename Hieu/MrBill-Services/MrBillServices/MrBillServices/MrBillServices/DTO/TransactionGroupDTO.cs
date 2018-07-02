using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class TransactionGroupDTO
    {
        [DataMember]
        public int TransactionGroupID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}