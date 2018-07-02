using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SAS_SkyScraper.MrBillService;

namespace SAS_SkyScraper
{
   public interface IScraper
    {
	   void GetDataFromBookingAndWriteToFile(IWebElement item, IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, string bookingRef, Identity userId);
       void GetBookingsAndSaveThemToTextFile(IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, Identity userId);
    }
}
