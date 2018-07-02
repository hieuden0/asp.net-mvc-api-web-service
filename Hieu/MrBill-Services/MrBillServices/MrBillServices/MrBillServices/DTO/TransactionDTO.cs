using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class TransactionDTO
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public string BookingRef { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string Traveller { get; set; }
        [DataMember]
        public DateTime? HtlTime1 { get; set; }
        [DataMember]
        public DateTime? HtlTime2 { get; set; }
        [DataMember]
        public DateTime AirDepTime1 { get; set; }
        [DataMember]
        public DateTime AirDepTime2 { get; set; }
        [DataMember]
        public DateTime AirRetTime1 { get; set; }
        [DataMember]
        public DateTime AirRetTime2 { get; set; }
        [DataMember]
        public DateTime CarTime1 { get; set; }
        [DataMember]
        public DateTime CarTime2 { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string CityDep1 { get; set; }
        [DataMember]
        public string CityDep2 { get; set; }
        [DataMember]
        public string CityRet1 { get; set; }
        [DataMember]
        public string CityRet2 { get; set; }
        [DataMember]
        public string Product { get; set; }
        [DataMember]
        public float Price { get; set; }
        [DataMember]
        public float PriceUserCurrency { get; set; }
        [DataMember]
        public float Total { get; set; }
        [DataMember]
        public float? Vat1 { get; set; }
        [DataMember]
        public float? Vat2 { get; set; }
        [DataMember]
        public float? Vat3 { get; set; }
        [DataMember]
        public int? Units { get; set; }
        [DataMember]
        public string PriceCurrency { get; set; }
        [DataMember]
        public string CardNumber { get; set; }
        [DataMember]
        public string CardHolder { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public DateTime BookingDate { get; set; }
        [DataMember]
        public DateTime AddedDate { get; set; }
        [DataMember]
        public DateTime RemoveDate { get; set; }
        [DataMember]
        public string ExtraInfo { get; set; }
        [DataMember]
        public string Attendees { get; set; }
        [DataMember]
        public DateTime ApproveDate { get; set; }
        [DataMember]
        public int ExportStatus { get; set; }
        [DataMember]
        public DateTime? UnlockedDate { get; set; }
        [DataMember]
        public int UnlockedBy { get; set; }
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public int PaymentID { get; set; }
        [DataMember]
        public int? TransactionGroupID { get; set; }
        [DataMember]
        public int SupplierID { get; set; }
        [DataMember]
        public int? CostCenter { get; set; }
        [DataMember]
        public int? EmployeeID { get; set; }
        [DataMember]
        public int? ProjectNO { get; set; }

        [DataMember]
        public SupplierInfoDTO SupplierInfoes { get; set; }
        [DataMember]
        public PaymentTypeDTO PaymentType { get; set; }
        [DataMember]
        public string ReceiptLink { get; set; }
        [DataMember]
        public ProjectDTO Project { get; set; }
        [DataMember]
        public TransactionGroupDTO TransactionGroup { get; set; }
        [DataMember]
        public int? MainTrip { get; set; }
        [DataMember]
        public int? MainTripGroup { get; set; }


    }
}