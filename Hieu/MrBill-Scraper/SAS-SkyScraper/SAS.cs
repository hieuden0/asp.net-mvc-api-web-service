using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using OpenQA.Selenium;
using SAS_SkyScraper.MrBillService;

namespace SAS_SkyScraper
{

    // ReSharper disable once InconsistentNaming
    public class SAS : IScraper
    {
        public void GetBookingsAndSaveThemToTextFile(IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, Identity userId)
        {
            BaseSystem.bookingList = driver.FindElements(By.CssSelector(selector.BookingItemsCssSelector));
            foreach (var item in BaseSystem.bookingList)
            {
                Thread.Sleep(1500);
				try
				{
					var bookingRef = item.FindElement(By.CssSelector(selector.BookingNumberCssSelector)).Text;
					this.GetDataFromBookingAndWriteToFile(item, driver, selector, transSup, bookingRef, userId);
				}
				catch (Exception)
				{
					var jscript = driver as IJavaScriptExecutor;
					List<string> handles = driver.WindowHandles.ToList<string>();
					jscript.ExecuteScript("window.close()");
					driver.SwitchTo().Window(handles.First());
				}
            }
                
        }


        public void GetDataFromBookingAndWriteToFile(IWebElement item, IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, string bookingRef, Identity userId)
        {

            BookingObject booking = new BookingObject();
            var destination = item.FindElement(By.CssSelector(selector.BookingTripsCssSelector)).Text;
            var url = driver.Url;

            var link = item.FindElement(By.LinkText(selector.ShowBookingItemLinkTextSelector)).GetAttribute("href");
            IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
            jscript.ExecuteScript("window.open()");
            List<string> handles = driver.WindowHandles.ToList<string>();
            driver.SwitchTo().Window(handles.Last());
            driver.Navigate().GoToUrl(url);
            jscript.ExecuteScript(link);

            Thread.Sleep(5000);
            ReadOnlyCollection<IWebElement> segments;
            try
            {
                segments = driver.FindElements(By.CssSelector(selector.SegmentItemsCssSelector));
                if (segments.Count < 1)
                    return;
            }

            catch (Exception)
            {
                return;
            }

            var outboundDepartureDate = "";
            var tempTime1 = "";
            var tempOutCity = "";
            var outboundArrivalDate = "";

            var inboundDepartureDate = "";
            var tempTime2 = "";
            var tempInCity = "";
            var inboundArrivalDate = "";

            //OUTBOUND
            try
            {
                foreach (var segment in segments.Skip(4).Take(1))
                {
                    outboundDepartureDate =
                        segment.FindElement(By.CssSelector(selector.OutboundDepartureDateCssSelector)).Text;
                    tempTime1 = segment.FindElement(By.CssSelector(selector.OutboundDepartureTimeCssSelector)).Text;
                    tempOutCity = segment.FindElement(By.CssSelector(selector.OutboundDepartureCityCssSelector)).Text;
                    outboundArrivalDate =
                        segment.FindElement(By.CssSelector(selector.OutboundArrivalDateCssSelector)).Text;
                }
            }
            catch (Exception)
            {

                foreach (var segment in segments.Skip(5).Take(1))
                {
                    outboundDepartureDate =
                        segment.FindElement(By.CssSelector(selector.OutboundDepartureDateCssSelector)).Text;
                    tempTime1 = segment.FindElement(By.CssSelector(selector.OutboundDepartureTimeCssSelector)).Text;
                    tempOutCity = segment.FindElement(By.CssSelector(selector.OutboundDepartureCityCssSelector)).Text;
                    outboundArrivalDate =
                        segment.FindElement(By.CssSelector(selector.OutboundArrivalDateCssSelector)).Text;
                }
            }


            var outboundDepartureTime = "";
            var outboundArrivalTime = "";
            var outboundCity1 = "";
            var outboundCity2 = "";

            var tempOutboundDepartureTime = tempTime1.Split('-');
            outboundDepartureTime = tempOutboundDepartureTime[0].Replace("-", "");
            outboundArrivalTime = tempOutboundDepartureTime[1].Replace("-", "");

            var outCities = tempOutCity.Split('-');
            outboundCity1 = outCities[0];
            outboundCity2 = outCities[1];

            //INBOUND
            try
            {
                foreach (var segment in segments.Skip(5).Take(1))
                {
                    inboundDepartureDate =
                        segment.FindElement(By.CssSelector(selector.InboundDepartureDateCssSelector)).Text;
                    tempTime2 = segment.FindElement(By.CssSelector(selector.InboundDepartureTimeCssSelector)).Text;
                    tempInCity = segment.FindElement(By.CssSelector(selector.InboundDepartureCityCssSelector)).Text;
                    inboundArrivalDate =
                        segment.FindElement(By.CssSelector(selector.InboundArrivalDateCssSelector)).Text;
                }
            }
            catch (Exception)
            {
                foreach (var segment in segments.Skip(6).Take(1))
                {
                    inboundDepartureDate =
                        segment.FindElement(By.CssSelector(selector.InboundDepartureDateCssSelector)).Text;
                    tempTime2 = segment.FindElement(By.CssSelector(selector.InboundDepartureTimeCssSelector)).Text;
                    tempInCity = segment.FindElement(By.CssSelector(selector.InboundDepartureCityCssSelector)).Text;
                    inboundArrivalDate =
                        segment.FindElement(By.CssSelector(selector.InboundArrivalDateCssSelector)).Text;
                }
            }

            var inboundDepartureTime = "";
            var inboundArrivalTime = "";

            var inCity1 = "";
            var inCity2 = "";
            var email = "";

            var tempInboundDepartureTime = tempTime2.Split('-');
            inboundDepartureTime = tempInboundDepartureTime[0].Replace("-", "");
            inboundArrivalTime = tempInboundDepartureTime[1].Replace("-", "");

            var inCities = tempInCity.Split('-');
            inCity1 = inCities[0];
            inCity2 = inCities[1];

            var infoText = driver.FindElement(By.CssSelector(selector.EmailTextCssSelector)).Text;
            var tempEmail = infoText.Split(' ');
            email = tempEmail[3];
            var totalPrice = "";
            //GENERAL
            try
            {
                totalPrice = driver.FindElement(By.CssSelector(selector.TotalPriceCssSelector)).Text;
            }
            catch (Exception)
            {
                totalPrice = driver.FindElement(By.CssSelector("span.data.price span.number")).Text;
            }

            var currency = "";
            try
            {
                currency = driver.FindElement(By.CssSelector(selector.CurrencyCssSelector)).Text;
            }
            catch (Exception)
            {
                currency = driver.FindElement(By.CssSelector("span.data.price span.iso")).Text;
            }
            var creationDate = driver.FindElement(By.CssSelector(selector.CreationDateCssSelector)).Text;
            var membershipNo = "";
            try
            {
                membershipNo = driver.FindElement(By.CssSelector(selector.MembershipNoCssSelector)).Text;
            }

            catch (Exception)
            {
                membershipNo = driver.FindElement(By.CssSelector("span.data.price span.number")).Text;
            }
            var creditCard = driver.FindElement(By.CssSelector(selector.CreditCardCssSelector)).Text;


            //var cardFirst = driver.FindElement(By.CssSelector(selector.CardHolderFirstCssSelector)).Text;

            //var cardLast = driver.FindElement(By.CssSelector(selector.CardHolderLastCssSelector)).Text;

            //var cardHolder = cardFirst + " " + cardLast;
            //var travellers = driver.FindElement(By.CssSelector(selector.TravellersCssSelector)).Text;


            booking.BookingRef = bookingRef;
            booking.Destination = destination;
            booking.OutboundDepartureDate = outboundDepartureDate;
            booking.OutboundDepartureTime = outboundDepartureTime;
            booking.OutboundDepartureCity = outboundCity1;
            booking.OutboundArrivalDate = outboundArrivalDate;
            booking.OutboundArrivalTime = outboundArrivalTime;
            booking.OutboundArrivalCity = outboundCity2;

            booking.InboundDepartureDate = inboundDepartureDate;
            booking.InboundDepartureTime = inboundDepartureTime;
            booking.InboundDepartureCity = inCity1;
            booking.InboundArrivalDate = inboundArrivalDate;
            booking.InboundArrivalTime = inboundArrivalTime;
            booking.InboundArrivalCity = inCity2;

            booking.TotalPrice = totalPrice;
            booking.Currency = currency;
            booking.CreationDate = creationDate;
            booking.MembershipNo = membershipNo;
            booking.Email = email;
            booking.CreditCard = creditCard;
            booking.BookingAgent = "SAS.se";
            booking.Category = "Flyg";
            booking.Name = "Flygbiljett";
	        try
	        {
				booking.VAT = driver.FindElement(By.CssSelector(selector.VAT)).Text;
	        }
	        catch (Exception)
	        {
		        booking.VAT = "0";
	        }


	        var year = DateTime.Now.Year.ToString();
	        var tempMonth = booking.OutboundArrivalDate.Substring(3, booking.OutboundArrivalDate.Length - 3);
			var month = Convert.ToDateTime(tempMonth).Month;
			var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
	        var fileLocation = year + "/" + monthName + "/" + "SAS" + "/" + userId.id + "/";

			var screen = new Size(750, 1000);

			
			BaseSystem.TakeScreenShot(driver, booking, fileLocation, screen, "scroll(0, 90)");
	       
			var okey = BaseSystem.UploadFile(fileLocation, booking.BookingRef, year, monthName, "SAS", userId);

	        if (okey)
	        {
				booking.InfoOrReciept = "/uploads/reciept/" + fileLocation + booking.BookingRef + ".JPG";
	        }
            //booking.CardHolder = cardHolder;
            //booking.Travellers = travellers;

			//BaseSystem.WriteToFile(booking);

            BaseSystem.CheckDbAndSaveItem(booking, transSup);
            jscript.ExecuteScript("window.close()");
            driver.SwitchTo().Window(handles.First());
            Thread.Sleep(5000);
        }

       
    }
}