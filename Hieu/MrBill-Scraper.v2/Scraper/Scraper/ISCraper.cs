using System.Collections.Generic;
using OpenQA.Selenium;
using Scraper.MrBillTransactionServices;

namespace Scraper
{
   public interface IScraper
   {
       List<IWebElement> NewBookingList { get; set; }
       List<string> BookingRefList { get; set; }
       SelectorObject SelectorData { get; set; }
       UserSupplierDetail UserSupplier { get; set; }
       TransactionDTO ResultTransaction { get; set; }
       void GetBookingsAndSaveThem(IWebDriver driver);
       bool LoginAndGoToBooking(IWebDriver driver);

       void GetListNewBookings(IWebDriver driver);
       
    }
}
