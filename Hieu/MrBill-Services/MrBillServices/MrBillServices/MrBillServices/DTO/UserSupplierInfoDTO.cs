using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class UserSupplierInfoDTO
    {
        [DataMember]
        public int SupplierId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }

        //public UserModel User { get; set; }
        [DataMember]
        public SupplierInfoDTO SupplierInfo { get; set; }
        //[DataMember]
        //public UserDTO UserProfile { get; set; }
    }
}