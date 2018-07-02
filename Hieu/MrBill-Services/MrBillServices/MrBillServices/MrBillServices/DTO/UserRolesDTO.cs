using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class UserRolesDTO
    {
        [DataMember]
        public int RoleId { get; set; }
        [DataMember]
        public string RoleName { get; set; }
    }
}