using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using SAS_SkyScraper.MrBillService;

namespace SAS_SkyScraper
{
    public class HOTELS : IScraper
    {

        public void GetBookingsAndSaveThemToTextFile(IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, Identity userId)
        {

            driver.FindElement(By.CssSelector(selector.EndedBookings)).Click();
            Thread.Sleep(2000);
            BaseSystem.bookingList = driver.FindElements(By.CssSelector(selector.BookingItemsCssSelector));

            foreach (var item in BaseSystem.bookingList)
            {
                var bookingref = "";
                this.GetDataFromBookingAndWriteToFile(item, driver, selector, transSup, bookingref, userId);
   
            }
        }
        public void GetDataFromBookingAndWriteToFile(IWebElement item, IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, string bookingRef, Identity userId)
        {
            BookingObject booking = new BookingObject();
            Thread.Sleep(1000);
           // driver.FindElement(By.CssSelector(selector.EndedBookings)).Click();
            bookingRef = item.FindElement(By.CssSelector(selector.BookingNumberCssSelector)).Text;
            var destination = item.FindElement(By.CssSelector(selector.BookingTripsCssSelector)).Text;
            var url = driver.Url;

            var link = item.FindElement(By.LinkText(selector.ShowBookingItemLinkTextSelector)).GetAttribute("href");
            IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
            jscript.ExecuteScript("window.open()");
            List<string> handles = driver.WindowHandles.ToList<string>();
            driver.SwitchTo().Window(handles.Last());

            driver.Navigate().GoToUrl(link);
          
            Thread.Sleep(5000);
            


            var arrival = driver.FindElement(By.CssSelector(selector.OutboundArrivalDateCssSelector)).Text;
            var departure = driver.FindElement(By.CssSelector(selector.InboundDepartureDateCssSelector)).Text;

            var arrivalCity = driver.FindElement(By.CssSelector(selector.InboundDepartureCityCssSelector)).Text;
            var email = selector.LoginUserName;
            //GENERAL
            var totalPrice = "";
            var currency = "";
            try
            {
               totalPrice = driver.FindElement(By.CssSelector(selector.TotalPriceCssSelector)).Text;
               currency = driver.FindElement(By.CssSelector(selector.TotalPriceCssSelector)).Text;
            }
            catch (Exception)
            {
                totalPrice = driver.FindElement(By.CssSelector(selector.CurrencyCssSelector)).Text; ;
                currency = driver.FindElement(By.CssSelector(selector.CurrencyCssSelector)).Text;
            }
            var regCurr = Regex.Replace(currency, @"[\d-]", string.Empty);
            var hotelName = driver.FindElement(By.CssSelector(selector.Name)).Text;
             
            var creationDate = DateTime.Now.ToShortDateString();
            var membershipNo = driver.FindElement(By.CssSelector(selector.MembershipNoCssSelector)).Text;
            var creditCard = driver.FindElement(By.CssSelector(selector.CreditCardCssSelector)).Text;
            var creditCardNumber = driver.FindElement(By.CssSelector(selector.CreditcardNumber)).Text;
            booking.BookingRef = bookingRef;
            booking.Destination = destination;
            var cardFirst = driver.FindElement(By.CssSelector(selector.CardHolderFirstCssSelector)).Text;

            var cardLast = driver.FindElement(By.CssSelector(selector.CardHolderLastCssSelector)).Text;

            var cardHolder = cardFirst + " " + cardLast;
            var travellers = driver.FindElement(By.CssSelector(selector.TravellersCssSelector)).Text;

            booking.Name = hotelName;
            booking.BookingAgent = "Hotels.com";
            booking.Category = "Hotell";

            booking.TotalPrice = totalPrice.Replace("€", "").Replace("SEK", "").Replace(" ", "").Replace(".", ",");
            booking.Currency = regCurr;

            booking.InboundArrivalDate = departure;
            booking.OutboundDepartureDate = arrival;
            
            booking.CreationDate = creationDate;
            booking.MembershipNo = membershipNo;
            
            booking.Email = email;
            booking.CreditCard = creditCard +" "+ creditCardNumber;
            
            booking.OutboundDepartureCity = arrivalCity;
            booking.CardHolder = cardHolder;
            booking.Travellers = travellers;



			var year = Convert.ToDateTime(booking.InboundArrivalDate).Year.ToString();
			var month = Convert.ToDateTime(booking.InboundArrivalDate).Month;
			var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
			var fileLocation = year + "/" + monthName + "/" + "HOTELS" + "/" + userId.id + "/";
			var screen = new Size(900, 1000);

			BaseSystem.TakeScreenShot(driver, booking, fileLocation, screen, "scroll(0, 500)");

			var okey = BaseSystem.UploadFile(fileLocation, booking.BookingRef, year, monthName, "HOTELS", userId);

			if (okey)
			{
				booking.InfoOrReciept = "/uploads/reciept/" + fileLocation + booking.BookingRef + ".JPG";
			}

            //BaseSystem.WriteToFile(booking);
            BaseSystem.CheckDbAndSaveItem(booking, transSup);
            jscript.ExecuteScript("window.close()");
            driver.SwitchTo().Window(handles.First());
            Thread.Sleep(2000);
        }
    }
}
