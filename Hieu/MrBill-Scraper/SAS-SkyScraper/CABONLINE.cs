using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SAS_SkyScraper.MrBillService;

namespace SAS_SkyScraper
{
   public class CABONLINE : IScraper
    {

       public void GetBookingsAndSaveThemToTextFile(IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, Identity userId)
       {
          
           BaseSystem.bookingList = driver.FindElements(By.CssSelector(selector.BookingItemsCssSelector));
           foreach (var item in BaseSystem.bookingList)
           {
               var bookingRef = "";
			   this.GetDataFromBookingAndWriteToFile(item, driver, selector, transSup, bookingRef, userId);
           }
               
       }
       public void GetDataFromBookingAndWriteToFile(IWebElement item, IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, string bookingRef, Identity userId)
       {
           var booking = new BookingObject();
           Thread.Sleep(1000);
           item.Click();
           bookingRef = item.FindElement(By.CssSelector(selector.BookingNumberCssSelector)).Text;
           var destination = item.FindElement(By.CssSelector(selector.BookingTripsCssSelector)).Text;

           //var infoText = driver.FindElement(By.CssSelector(selector.EmailTextCssSelector)).Text;

           //GENERAL
           
           var tempPrice = item.FindElement(By.CssSelector(selector.TotalPriceCssSelector)).Text;
           var price = Regex.Replace(tempPrice, @"(\D+)", "");
           var currency = Regex.Replace(tempPrice, @"(\d+)", "");
           var creationDate = DateTime.Now.ToShortDateString();
           var creditCard = item.FindElement(By.CssSelector(selector.CreditCardCssSelector)).Text;
           var CcHolder = item.FindElement(By.CssSelector(selector.CardHolderFirstCssSelector)).Text;


           booking.BookingAgent = "Cabonline.se";
           booking.Category = "Taxi";
           booking.BookingRef = bookingRef;
           booking.Destination = destination;

           booking.OutboundDepartureDate = item.FindElement(By.CssSelector(selector.InboundDepartureDateCssSelector)).Text;

           booking.OutboundDepartureCity = item.FindElement(By.CssSelector(selector.InboundDepartureCityCssSelector)).Text;
           booking.OutboundArrivalCity = item.FindElement(By.CssSelector(selector.OutboundDepartureCityCssSelector)).Text;

           booking.Name = item.FindElement(By.CssSelector(selector.InfoCssSelector)).Text;

           booking.TotalPrice = price;
           booking.Currency = currency;
           booking.CreationDate = creationDate;
           booking.CreditCard = creditCard;
           booking.CardHolder = CcHolder;


	       var year = Convert.ToDateTime(booking.OutboundDepartureDate).Year.ToString();
		   var month = Convert.ToDateTime(booking.OutboundDepartureDate).Month;
		   var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
		   var fileLocation = year + "/" + monthName + "/" + "CABONLINE" + "/" + userId.id + "/";

		   var screen = new Size(750, 1000);


		   BaseSystem.TakeScreenShot(driver, booking, fileLocation, screen, "scroll(0, 90)");

		   var okey = BaseSystem.UploadFile(fileLocation, booking.BookingRef, year, monthName, "CABONLINE", userId);

		   if (okey)
		   {
			   booking.InfoOrReciept = "/uploads/reciept/" + fileLocation + booking.BookingRef + ".JPG";
		   }



           BaseSystem.WriteToFile(booking);
           BaseSystem.CheckDbAndSaveItem(booking, transSup);
           item.Click();
           Thread.Sleep(1000);
       }
    }
}
