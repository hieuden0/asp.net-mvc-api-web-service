using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class UserSupplierdata
    {
        [DataMember]
        public int SupplierId { get; set; }
        [DataMember]
        public string SupplierPassword { get; set; }

        [DataMember]
        public string SupplierUsername { get; set; }
       
    }
}
