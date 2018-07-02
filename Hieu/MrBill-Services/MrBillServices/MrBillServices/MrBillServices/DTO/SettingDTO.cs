using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
namespace MrBillServices.DTO
{
    [DataContract]
    public class SettingDTO
    {
         [DataMember]
        public int ID { get; set; }
         [DataMember]
        public int UserID { get; set; }
         [DataMember]
        public string SettingName { get; set; }
         [DataMember]
        public string SettingValue { get; set; }
    }
}