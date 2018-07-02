using System.Runtime.Serialization;

namespace MrBillServices.DTO
{
    [DataContract]
    public class TransactionData
    {
        [DataMember]
        public long TransactionID { get; set; }
        [DataMember]
        public string BookingReference { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string Traveller { get; set; }
        [DataMember]
        public string HtlTime1 { get; set; }
        [DataMember]
        public string HtlTime2 { get; set; }
        [DataMember]
        public string AirDepTime1 { get; set; }
        [DataMember]
        public string AirDepTime2 { get; set; }
        [DataMember]
        public string AirRetTime1 { get; set; }
        [DataMember]
        public string AirRetTime2 { get; set; }
        [DataMember]
        public string CarTime1 { get; set; }
        [DataMember]
        public string CarTime2 { get; set; }
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
        public double? Price { get; set; }
        [DataMember]
        public double? PriceUserCurrency { get; set; }
        [DataMember]
        public double? Total { get; set; }
        [DataMember]
        public double? Vat1 { get; set; }
        [DataMember]
        public double? Vat2 { get; set; }
        [DataMember]
        public double? Vat3 { get; set; }
        [DataMember]
        public int?  Units { get; set; }
        [DataMember]
        public double? PriceCurrency { get; set; }
        public string CardNumber { get; set; }
        [DataMember]
        public string CardHolder { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string BookingDate { get; set; }
        [DataMember]
        public string AddedDate { get; set; }
        public string RemoveDate { get; set; }
        [DataMember]
        public string ExtraInfo { get; set; }
        [DataMember]
        public string Attendees { get; set; }
        [DataMember]
        public string ApproveDate { get; set; }
        [DataMember]
        public int ExportStatus { get; set; }
        public string UnlockedDate { get; set; }
        [DataMember]
        public int UnlockedByID { get; set; }
        [DataMember]
        public string UnlockedBy { get; set; }
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        public int UserID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public int PaymentID { get; set; }
        [DataMember]
        public string PaymentName { get; set; }
        [DataMember]
        public int TransactionGroupID { get; set; }
        public string TransactionGroupName { get; set; }
        [DataMember]
        public int SupplierID { get; set; }
        [DataMember]
        public string SupplierName { get; set; }
        [DataMember]
        public int CostCenterID { get; set; }
        [DataMember]
        public string CostCenterName { get; set; }
        [DataMember]
        public int EmployeeID { get; set; }
        [DataMember]
        public string EmployeeIden { get; set; }
        [DataMember]
        public int ProjectID { get; set; }
        [DataMember]
        public string ProjectNO { get; set; }
    }
}
