using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class ProjectDTO
    {
        [DataMember]
        public int ProjectID { get; set; }
        [DataMember]
        public string ProjectNumber { get; set; }
        [DataMember]
        public int UserControlID { get; set; }
    }
}