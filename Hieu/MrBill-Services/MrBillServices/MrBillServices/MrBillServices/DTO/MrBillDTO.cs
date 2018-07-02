using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class MrBillDTO
    {
        [DataMember]
        public List<UserSupplierdata> UserSupplier { get; set; }

        [DataMember]
        public int UserId { get; set; }
          [DataMember]
        public string UserName { get; set; }
    }
}
