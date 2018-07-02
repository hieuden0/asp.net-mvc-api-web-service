using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MrBillServices.DTO
{
    [DataContract]
    public class ContactInfoDTO
    {

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]

        public string Phone { get; set; }
        [DataMember]

        public string Address { get; set; }
        [DataMember]

        public string City { get; set; }
        [DataMember]

        public string PostCode { get; set; }
        [DataMember]
        public long? UserId { get; set; }


    }
}
