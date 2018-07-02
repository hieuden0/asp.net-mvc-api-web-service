using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MrBillServices.DTO
{
    [DataContract]
    public class ScraperLogResultDTO
    {
        [DataMember]
        public bool SaveSucessfull { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Track { get; set; }
    }
}