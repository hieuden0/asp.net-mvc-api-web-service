using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class UserIdentity
    {
        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public string Username { get; set; }
    }
}