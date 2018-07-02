using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SAS_SkyScraper.UserService;
using Identity = SAS_SkyScraper.MrBillService.Identity;
using TransactionSupplier = SAS_SkyScraper.MrBillService.TransactionSupplier;

namespace SAS_SkyScraper
{
	public class EASYPARK : IScraper
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

			Thread.Sleep(1000);
			var destination = item.FindElement(By.CssSelector(selector.InboundDepartureCityCssSelector)).Text;


			var email = "";
			var totalPrice = item.FindElement(By.CssSelector(selector.TotalPriceCssSelector)).Text.Replace(".", ",");
			var currency = "SEK";
			var creationDate = item.FindElement(By.CssSelector(selector.CreationDateCssSelector)).Text;
			//	item.FindElement(By.CssSelector(selector.InfoOrReciept)).Click();
			item.FindElement(By.CssSelector("td.tableActions.text-center > a")).Click();
			Thread.Sleep(3000);
			var link = item.FindElement(By.CssSelector("td.tableActions.text-center > a")).GetAttribute("href");
			
			var bookingNumber = link.Replace("https://epic.easyparksystem.net/epic-ss/proxy/epicfile/parkings/", "").Replace("/pdf", "");
			var product = item.FindElement(By.CssSelector(selector.Name)).Text;
			bookingRef = bookingNumber;
			var booking = new BookingObject();
			booking.BookingAgent = "Easypark.se";
			booking.Category = "Easypark";
			booking.Name = product;
			booking.InboundDepartureDate = creationDate;
			booking.OutboundDepartureDate = creationDate;
			booking.BookingRef = bookingRef;
			booking.Destination = destination;
			booking.TotalPrice = totalPrice;
			booking.Currency = currency;
			booking.CreationDate = creationDate;
			booking.Email = email;
			

			try
			{
				Convert.ToDouble(booking.TotalPrice);


				var year = Convert.ToDateTime(creationDate).Year.ToString();
				var month = Convert.ToDateTime(creationDate).Month;
				var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
				var fileLocation = year + "/" + monthName + "/" + "EASYPARK" + "/" + userId.id + "/";

				booking.InfoOrReciept = "/uploads/reciept/" + fileLocation + "Parking_" + bookingNumber +".PDF";
				
				var fileName = booking.BookingRef.Replace(" ", "").Replace(":", "");
				SaveFile(year, monthName, "EASYPARK", userId, bookingNumber);
				

				BaseSystem.CheckDbAndSaveItem(booking, transSup);
			}
			catch (Exception)
			{


			}

		}

		private static void SaveFile( string year, string month, string supplier, Identity userId, string bookingNumber)
		{

			string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			string pathDownload = Path.Combine(pathUser, "Downloads");
			var fileName = "Parking_" + bookingNumber;

			var target =  @"\\phuket.mrorange.local/InetPub/MrBill/uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".PDF";
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

