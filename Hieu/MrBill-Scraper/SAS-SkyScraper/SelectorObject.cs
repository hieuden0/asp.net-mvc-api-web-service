using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAS_SkyScraper
{
    public class SelectorObject
    {
        public string HomeUrl;
        public string LoginLinkCssPath;
        public string LoginUserNameCssSelector;
        public string LoginUserName;
        public string LoginUserPasswordCssSelector;
        public string LoginUserPassword;
        public string LoginButtonCssSelector;
        public string MyBookingsUrl;
        public string EndedBookings;

        public string BookingItemsCssSelector;
        public string BookingNumberCssSelector;
        public string BookingTripsCssSelector;
        public string ShowBookingItemLinkTextSelector;
        public string SegmentItemsCssSelector;

        public string OutboundDepartureDateCssSelector;
        public string OutboundDepartureTimeCssSelector;
        public string OutboundDepartureCityCssSelector;
        public string OutboundArrivalTimeCssSelector;
        public string OutboundArrivalDateCssSelector;
        public string OutboundArrivalCityCssSelector;

        public string InboundDepartureDateCssSelector;
        public string InboundDepartureTimeCssSelector;
        public string InboundDepartureCityCssSelector;
        public string InboundArrivalTimeCssSelector;
        public string InboundArrivalDateCssSelector;
        public string InboundArrivalCityCssSelector;

        public string TotalPriceCssSelector;
        public string CurrencyCssSelector;
        public string CreationDateCssSelector;
        public string MembershipNoCssSelector;
        public string EmailTextCssSelector;
        public string CreditCardCssSelector;
        public string CreditcardNumber;
        public string CardHolderFirstCssSelector;
        public string CardHolderLastCssSelector;

        public string TravellersCssSelector;
        public string VAT;
        public string CategorieCssSerlector;
        public string InfoOrReciept;
        public string Name { get; set; }
	    public string InfoCssSelector;

    }
}
