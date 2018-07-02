using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using MrBillServices.Entities;

namespace MrBillServices.DTO
{
    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string PostCode { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public int UserRoleId { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public CompanyInfoDTO CompanyInfo { get; set; }

        [DataMember]
        public UserRolesDTO UserRoles { get; set; }

        [DataMember]
        public List<UserSupplierInfoDTO> UserSupplierInfoes { get; set; }
    }
}