using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class TransactionCheckInfo
    {
        [DataMember]
        public long TransactionID { get; set; }
        [DataMember]
        public string BookingReference { get; set; }

        [DataMember]
        public string SupplierName { get; set; }


    }
}
