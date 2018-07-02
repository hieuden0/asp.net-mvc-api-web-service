using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrBillServices.DTO.Mobile
{
    public class TransactionObject
    {
        public string ExpensDate { get; set; }
        public string DateAdded { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Supplier { get; set; }
        public string Reference { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string Traveller { get; set; }
        public string PaymentMethod { get; set; }
        public string CardHolder { get; set; }
        public decimal? Price { get; set; }
        public decimal? Vat { get; set; }
        public string CostCenter { get; set; }
        public string Reciept { get; set; }
        public string Currency { get; set; }
        public string ExtraInfo { get; set; }
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public decimal? IsDisabled { get; set; }
    }
}
