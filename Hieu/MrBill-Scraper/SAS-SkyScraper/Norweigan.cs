using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using SAS_SkyScraper.MrBillService;

namespace SAS_SkyScraper
{
	class NORWEIGAN : IScraper
	{
		public void GetBookingsAndSaveThemToTextFile(IWebDriver driver, SelectorObject selector, TransactionSupplier transSup,
		Identity userId)
		{
			var currentYear = DateTime.Now.Year;

			Thread.Sleep(2000);
			driver.Navigate().GoToUrl("https://www.norwegian.com/ssl/se/kundeservice/mitt-norwegian/mina-reservationer/?year=" + currentYear);

			BaseSystem.bookingList = driver.FindElements(By.CssSelector(selector.BookingItemsCssSelector));

			foreach (var item in BaseSystem.bookingList)
			{
				OpenWindowAndGoToBooking(driver, item);

				var bookingref = "";
				this.GetDataFromBookingAndWriteToFile(item, driver, selector, transSup, bookingref, userId);
			}
		}

		private void OpenWindowAndGoToBooking(IWebDriver driver, IWebElement item)
		{
			var link = item.FindElement(By.CssSelector(".pagetools_MyTrips a")).GetAttribute("href");
			IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
			jscript.ExecuteScript("window.open()");
			List<string> handles = driver.WindowHandles.ToList<string>();
			driver.SwitchTo().Window(handles.Last());
			driver.Navigate().GoToUrl(link);
			driver.FindElement(By.CssSelector(".pagetooltext")).Click();

		}

		public void GetDataFromBookingAndWriteToFile(IWebElement item, IWebDriver driver, SelectorObject selector,
			TransactionSupplier transSup, string bookingRef, Identity userId)
		{
			BookingObject booking = new BookingObject();
			Thread.Sleep(1000);
			// driver.FindElement(By.CssSelector(selector.EndedBookings)).Click();
			var recieptUrl = driver.FindElement(By.CssSelector(selector.InfoOrReciept));
			//recieptUrl.Click();

			bookingRef = driver.FindElement(By.CssSelector(selector.BookingNumberCssSelector)).Text;
			var destination = driver.FindElement(By.CssSelector(selector.OutboundDepartureCityCssSelector)).Text;

			var arrival = driver.FindElement(By.CssSelector(selector.OutboundDepartureDateCssSelector)).Text.Replace(".", "") + " " + DateTime.Now.Year;
			var departure = driver.FindElement(By.CssSelector(selector.InboundDepartureDateCssSelector)).Text.Replace(".", "") + " " + DateTime.Now.Year;

			var email = selector.LoginUserName;
			//GENERAL
			var totalPrice = driver.FindElement(By.CssSelector(selector.TotalPriceCssSelector)).Text;


			var Name = "";
			if (selector.Name != null)
			{
				Name = driver.FindElement(By.CssSelector(selector.Name)).Text;
			}

			var creationDate = DateTime.Now.ToShortDateString();
			//   var membershipNo = driver.FindElement(By.CssSelector(selector.MembershipNoCssSelector)).Text;
			//  var creditCard = driver.FindElement(By.CssSelector(selector.CreditCardCssSelector)).Text;
			//  var creditCardNumber = driver.FindElement(By.CssSelector(selector.CreditcardNumber)).Text;
			booking.BookingRef = bookingRef;
			booking.Destination = destination;

			var tempTravellers = driver.FindElements(By.CssSelector(selector.TravellersCssSelector));

			var travellers = "";
			foreach (var traveller in tempTravellers)
			{
				travellers += " " + traveller.Text;
			}
			 
			var currency = Regex.Replace(totalPrice, @"\d+", string.Empty);
			booking.Name = Name;
			booking.BookingAgent = "Norweigan.se";
			booking.Category = "Norweigan";
			booking.Currency = currency;
			booking.TotalPrice = totalPrice.Replace(".", ",");

			booking.InboundArrivalDate = departure;
			booking.OutboundDepartureDate = arrival;
			

			booking.CreationDate = creationDate;
			// booking.MembershipNo = membershipNo;

			booking.Email = email;
			// booking.CreditCard = creditCard + " " + creditCardNumber;
			booking.Travellers = travellers;

			driver.FindElement(By.CssSelector("div.reservationToolBar > table > tbody > tr:nth-child(2) > td:nth-child(3) > div > a > span")).Click();
			Thread.Sleep(2500);
			var year = Convert.ToDateTime(arrival).Year.ToString();
			var month = Convert.ToDateTime(arrival).Month;
			var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
			var fileLocation = year + "/" + monthName + "/" + "NORWEIGAN" + "/" + userId.id + "/";

			booking.InfoOrReciept = "/uploads/reciept/" + fileLocation + "Resekvitto - " + bookingRef + ".PDF";

			SaveFile(year, monthName, "NORWEIGAN", userId, bookingRef);
				
		
			//BaseSystem.WriteToFile(booking);
			IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
			List<string> handles = driver.WindowHandles.ToList<string>();
			BaseSystem.CheckDbAndSaveItem(booking, transSup);
			jscript.ExecuteScript("window.close()");
			driver.SwitchTo().Window(handles.First());
			Thread.Sleep(2500);
		}

		private static void SaveFile(string year, string month, string supplier, Identity userId, string bookingNumber)
		{

			string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			string pathDownload = Path.Combine(pathUser, "Downloads");
			var fileName = "Resekvitto - " + bookingNumber;

			var target = @"\\phuket.mrorange.local/InetPub/MrBill/uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".PDF";
			var source = pathDownload + "/" + fileName + ".PDF";

			try
			{
				File.Copy(source, target, true);
			}

			catch (Exception)
			{
				var path = @"\\phuket.mrorange.local/InetPub/MrBill/uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/";
				Directory.CreateDirectory(path);
				File.Copy(source, target, true);
			}

			File.Delete(source);
		}

		
	}
}
