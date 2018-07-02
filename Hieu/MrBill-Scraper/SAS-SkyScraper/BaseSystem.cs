using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using SAS_SkyScraper.MrBillService;
using SAS_SkyScraper.TransactionService;
using AuthenticationToken = SAS_SkyScraper.TransactionService.AuthenticationToken;
using Identity = SAS_SkyScraper.MrBillService.Identity;
using TransactionStatus = SAS_SkyScraper.TransactionService.TransactionStatus;
using TransactionSupplier = SAS_SkyScraper.MrBillService.TransactionSupplier;

namespace SAS_SkyScraper
{
	public static class BaseSystem
	{
		public static IEnumerable<IWebElement> bookingList;
		static StringBuilder TextFile = new StringBuilder();
		public const string file = @"C:\Projects\bookingInfo.txt";
		private static SAS _sas = new SAS();
		private static HOTELS _hotels = new HOTELS();
		private static EASYPARK _easypark = new EASYPARK();
		private static CABONLINE _cabonline = new CABONLINE();
		private static NORWEIGAN _norweigan = new NORWEIGAN();


		public static IList<IScraper> ClassList = new List<IScraper>
        {
            // LÄGG TILL ALLA KALSSER SOM HAR INTERFACET
            	
			 _sas,
			_hotels,
			_easypark,
		    _cabonline,
			_norweigan,
		};

		static void Main(string[] args)
		{
			//UploadFile("", "", "", "", "");
			//  COMMENTED AWAY "SAVE TO FILE" OBJECTS  \\

			////Create csv file or make it empty 
			//if (File.Exists(file))
			//    File.WriteAllText(file, String.Empty);
			//else
			//    File.Create(file);
			//SaveFIrstLineToTextFile();

			//  COMMENTED AWAY "SAVE TO FILE" OBJECTS  \\

			//SET CULTUREINFO

			var culture = CultureInfo.CreateSpecificCulture("sv-SE");
			var uiCulture = CultureInfo.CreateSpecificCulture("en-US");
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = uiCulture;
			Console.WriteLine("Using " + culture + " cultureInfo and " + uiCulture + " as UiCulture");
			int index = 0;

			index++;
			var selectors = SetupAndGetSelectors();
			var users = UsersToScrape();
			//Creting a foreachLoop and loop through 2 Enumerables at the same time.
			using (var e1 = ClassList.GetEnumerator())
			using (var e2 = selectors.GetEnumerator())
			{
				while (e1.MoveNext() && e2.MoveNext())
				{
					var Class = e1.Current;
					var selector = e2.Current;

					foreach (var transSup in from user in users.Where(user => user.transactionSuppliers.Length != 0) from transSup in user.transactionSuppliers where String.Equals(transSup.name, Class.GetType().Name, StringComparison.CurrentCultureIgnoreCase) select transSup)
					{
						var userId = transSup.userIdentity;
						//var driver = new PhantomJSDriver();
						var driver = new ChromeDriver();
						//driver.Manage().Window.Maximize();
						
						driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
						driver.Navigate().GoToUrl(selector.HomeUrl);
						try
						{
							LoginToSiteAndGoToMyBookings(driver, selector, transSup);
							Class.GetBookingsAndSaveThemToTextFile(driver, selector, transSup, userId);
						}
						catch (Exception exception)
						{
                            Console.WriteLine(exception);
							driver.Quit();
						}
						driver.Quit();

					}

				}
			}
            //Console.ReadKey();
		}

		private static IEnumerable<MrBillDTO> UsersToScrape()
		{
			var userServiceClient = new MrBillServicePortTypeClient();
			var auth = new MrBillService.AuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			//sets recieved data to max value
			((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
			var userList = new List<MrBillDTO>();
            foreach (var user in userServiceClient.getMrBillUsers(auth))
            {
                userList.Add(user);
            }
            //#region for test
            //var mrbilluser = userServiceClient.getMrBillUsers(auth).ToList();
            //foreach (var user in mrbilluser)
            //{
            //    if (user.userId == 82433)
            //    {
            //        var editsupplier = user.transactionSuppliers.Single(s => s.name == "NORWEIGAN");
            //        editsupplier.supplierUsername = "jens.jonasson@falco.se";
            //        editsupplier.supplierPassword = "falco123";
            //        userList.Add(user);
            //    }
            //    //if (user.userId == 82454)
            //    //{
            //    //    user.transactionSuppliers = new TransactionSupplier[1];
            //    //    user.transactionSuppliers[0] = new TransactionSupplier()
            //    //    {
            //    //        supplierUsername = "jens.jonasson@falco.se",
            //    //        supplierPassword = "falco123",
            //    //        name = "NORWEIGAN",
            //    //        userIdentity = new SAS_SkyScraper.MrBillService.Identity()
            //    //        {
            //    //            id = user.userId
            //    //        }
            //    //    };
            //    //    userList.Add(user);
            //    //}
            //}
            //#endregion
			return userList;
		}

		private static void LoginToSiteAndGoToMyBookings(IWebDriver driver, SelectorObject selector, TransactionSupplier user)
		{

			Thread.Sleep(2000);
			try
			{
				driver.FindElement(By.CssSelector("#frontpagePopup a.close-reveal-modal")).Click();
				Thread.Sleep(2000);
			}
			catch (Exception)
			{
				Thread.Sleep(2000);
			}
			driver.FindElement(By.CssSelector(selector.LoginLinkCssPath)).Click();
			driver.FindElement(By.CssSelector(selector.LoginUserNameCssSelector)).SendKeys(user.supplierUsername);
			driver.FindElement(By.CssSelector(selector.LoginUserPasswordCssSelector)).SendKeys(user.supplierPassword);
			driver.FindElement(By.CssSelector(selector.LoginButtonCssSelector)).Click();
			Thread.Sleep(3000);
			driver.Navigate().GoToUrl(selector.MyBookingsUrl);

			if (String.IsNullOrEmpty(selector.EndedBookings)) return;
			Thread.Sleep(4000);
		}

		public static void WriteToFile(BookingObject booking)
		{
			var newLine = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}{21}", booking.BookingRef, booking.Destination, booking.OutboundDepartureDate, booking.OutboundDepartureTime, booking.OutboundDepartureCity, booking.OutboundArrivalDate, booking.OutboundArrivalTime, booking.OutboundArrivalCity, booking.InboundDepartureDate, booking.InboundDepartureTime, booking.InboundDepartureCity, booking.InboundArrivalDate, booking.InboundArrivalTime, booking.InboundArrivalCity, booking.TotalPrice, booking.Currency, booking.CreationDate, booking.MembershipNo, booking.Email, booking.CreditCard, booking.InfoOrReciept, Environment.NewLine);

			TextFile.Append(newLine);
			File.WriteAllText(file, TextFile.ToString());
		}

		public static void CheckDbAndSaveItem(BookingObject booking, TransactionSupplier transSup)
		{
			var transactionServiceClient = new TransactionServicePortTypeClient();
			var auth = new AuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var userId = transSup.userIdentity.id.ToString();
			((BasicHttpBinding)transactionServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
			var oldTrans = transactionServiceClient.getTransactionsForUser(auth, userId);

			var oldRefList = oldTrans.Select(trans => trans.bookingReference).ToList();

			var oldTransNameList = oldTrans.Select(trans => trans.transactionSupplier).ToList();

			var transList = new List<TransactionService.Transaction>();

			var transaction = new TransactionService.Transaction();

			var price = Regex.Replace(booking.TotalPrice, "[A-z]+.", string.Empty).Replace(".", ",").Replace(" ", "");
			var curency = "";
			if (booking.Currency != null)
			{
				curency = Regex.Replace(booking.Currency, @"\d+", string.Empty).Replace(".", "").Replace(" ", "");
			}

			transaction.transactionSupplier = transSup.name;
			transaction.bookingReference = booking.BookingRef;

			if (booking.OutboundArrivalCity != null)
			{
				transaction.destinationCity = booking.OutboundArrivalCity;
			}
			var depDate = "";
			var arrDate = "";
			if (booking.InboundArrivalDate != null)
			{
				depDate = booking.OutboundDepartureDate.Substring(3);
				arrDate = booking.InboundArrivalDate.Substring(3);
				try
				{
					transaction.date1 = Convert.ToDateTime(depDate);
					transaction.date2 = Convert.ToDateTime(arrDate);
				}


				catch (Exception)
				{
					depDate = booking.OutboundDepartureDate;
					arrDate = booking.InboundArrivalDate;

					transaction.date1 = Convert.ToDateTime(depDate);
					transaction.date2 = Convert.ToDateTime(arrDate);
				}
			}
			else
			{
				transaction.date1 = Convert.ToDateTime(booking.OutboundDepartureDate);
				transaction.date2 = Convert.ToDateTime(booking.OutboundDepartureDate);
			}

			//if (transaction.date1 > DateTime.Now)
			//{
			//	return;
			//}

			transaction.date2Specified = true;
			transaction.date1Specified = true;
			transaction.fileLocation = booking.InfoOrReciept ?? "";
			transaction.price = Convert.ToDecimal(price);
			transaction.priceSpecified = true;
			transaction.travelerConfirmationEmail = booking.Email;
			transaction.travelerMembershipCard = booking.MembershipNo;
			transaction.creditCardNumber = booking.CreditCard;
			transaction.destination = booking.Destination;
			transaction.currency = curency;
			transaction.addedDate = DateTime.Now;
			transaction.expenseDate = DateTime.Now;
			transaction.version = 1;
			transaction.country = "SE";
			transaction.transactionOwner = "MrBill";
			transaction.userType = "REGULAR";
			transaction.numberOfUnits = 1;
			transaction.tripType = booking.Category;
			transaction.product = booking.Name;
			transaction.creditCardOwner = booking.CardHolder;
			transaction.travelerName = booking.Travellers;
			transaction.transactionSupplier = booking.BookingAgent;
			transaction.vat3 = null;
			transList.Add(transaction);

			if (oldRefList.Contains(booking.BookingRef) && oldTransNameList.Contains(booking.BookingAgent))
				return;

			transactionServiceClient.saveTransactions(auth, userId, transList.ToArray());
		}

		private static void SaveFIrstLineToTextFile()
		{
			var newLine = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}{21}", "BookingRef", "Destination", "OutboundDepartureDate", "OutboundDepartureTime", "OutboundDepartureCity", "OutboundArrivalDate", "OutboundArrivalTime", "OutboundArrivalCity", "InboundDepartureDate", "InboundDepartureTime", "InboundDepartureCity", "InboundArrivalDate", "InboundArrivalTime", "InboundArrivalCity", "TotalPrice", "Currency", "CreationDate", "MembershipNo", "Email", "CreditCard", "Recepit or INFO", Environment.NewLine);

			TextFile.Append(newLine);
			File.WriteAllText(file, TextFile.ToString());
		}


		public static bool UploadFile(string fileLocation, string fileName, string year, string month, string supplier, Identity userId)
		{

			var source = "C:/" + fileLocation + fileName + ".JPG";
			var target = "";
            
			try
			{
                //Console.WriteLine("upfile 1");
                target = ConfigurationManager.AppSettings["UPLOADFILEFOLDER"] + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".JPG";
				File.Copy(source, target, true);
			}
			catch (Exception)
			{
                //Console.WriteLine("upfile 2");
                var path = ConfigurationManager.AppSettings["UPLOADFILEFOLDER"] + year + "/" + month + "/" + supplier + "/" + userId.id + "/";
				DirectoryInfo di = Directory.CreateDirectory(path);

                target = ConfigurationManager.AppSettings["UPLOADFILEFOLDER"] + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".JPG";

				File.Copy(source, target, true);
                //Console.WriteLine("upfile 3");
			}
			

			
			File.Delete(source);
			return true;
		}



		public static void TakeScreenShot(IWebDriver driver, BookingObject booking, string fileLocation, Size screenSize, string scroll)
		{
			var js = (IJavaScriptExecutor)driver;
			js.ExecuteScript("document.body.style.zoom='90%'");
			js.ExecuteScript(scroll);
			driver.Manage().Window.Size = screenSize;
			var screenshotDriver = driver as ITakesScreenshot;
			var screenshot = screenshotDriver.GetScreenshot();

			//screenshot.SaveAsFile(fileLocation + booking.BookingRef + ".png", ImageFormat.Png);
			try
			{
				screenshot.SaveAsFile("C:/" + fileLocation + booking.BookingRef + ".JPG", ImageFormat.Jpeg);

			}
			catch (Exception)
			{
				var path = "C:/" + fileLocation;
				DirectoryInfo di = Directory.CreateDirectory(path);
				screenshot.SaveAsFile("C:/" + fileLocation + booking.BookingRef + ".JPG", ImageFormat.Jpeg);
			}


			js.ExecuteScript("document.body.style.zoom='100%'");
		}


		public static List<SelectorObject> SetupAndGetSelectors()
		{
			var SelectorList = new List<SelectorObject>();

            var SASselector = new SelectorObject
            {
                HomeUrl = ConfigurationManager.AppSettings["SASHomeUrl"],
                LoginLinkCssPath = ConfigurationManager.AppSettings["SASloginLinkCssPath"],
                LoginUserNameCssSelector = ConfigurationManager.AppSettings["SASLoginUserNameCssSelector"],
                LoginUserName = ConfigurationManager.AppSettings["SASLoginUserName"],
                LoginUserPasswordCssSelector = ConfigurationManager.AppSettings["SASLoginUserPasswordCssSelector"],
                LoginUserPassword = ConfigurationManager.AppSettings["SASLoginUserPassword"],
                LoginButtonCssSelector = ConfigurationManager.AppSettings["SASLoginButtonCssSelector"],
                MyBookingsUrl = ConfigurationManager.AppSettings["SASMyBookingsUrl"],
                BookingItemsCssSelector = ConfigurationManager.AppSettings["SASBookingItemsCssSelector"],
                BookingNumberCssSelector = ConfigurationManager.AppSettings["SASBookingNumberCssSelector"],
                BookingTripsCssSelector = ConfigurationManager.AppSettings["SASBookingTripsCssSelector"],
                ShowBookingItemLinkTextSelector = ConfigurationManager.AppSettings["SASShowBookingItemLinkTextSelector"],
                SegmentItemsCssSelector = ConfigurationManager.AppSettings["SASSegmentItemsCssSelector"],
                OutboundDepartureDateCssSelector = ConfigurationManager.AppSettings["SASOutboundDepartureDateCssSelector"],
                OutboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["SASOutboundDepartureTimeCssSelector"],
                OutboundDepartureCityCssSelector = ConfigurationManager.AppSettings["SASOutboundDepartureCityCssSelector"],
                OutboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["SASOutboundArrivalTimeCssSelector"],
                OutboundArrivalDateCssSelector = ConfigurationManager.AppSettings["SASOutboundArrivalDateCssSelector"],
                OutboundArrivalCityCssSelector = ConfigurationManager.AppSettings["SASOutboundArrivalCityCssSelector"],
                InboundDepartureDateCssSelector = ConfigurationManager.AppSettings["SASInboundDepartureDateCssSelector"],
                InboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["SASInboundDepartureTimeCssSelector"],
                InboundDepartureCityCssSelector = ConfigurationManager.AppSettings["SASInboundDepartureCityCssSelector"],
                InboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["SASInboundArrivalTimeCssSelector"],
                InboundArrivalDateCssSelector = ConfigurationManager.AppSettings["SASInboundArrivalDateCssSelector"],
                InboundArrivalCityCssSelector = ConfigurationManager.AppSettings["SASInboundArrivalCityCssSelector"],
                TotalPriceCssSelector = ConfigurationManager.AppSettings["SASTotalPriceCssSelector"],
                CurrencyCssSelector = ConfigurationManager.AppSettings["SASCurrencyCssSelector"],
                CreationDateCssSelector = ConfigurationManager.AppSettings["SASCreationDateCssSelector"],
                MembershipNoCssSelector = ConfigurationManager.AppSettings["SASMembershipNoCssSelector"],
                EmailTextCssSelector = ConfigurationManager.AppSettings["SASEmailTextCssSelector"],
                CreditCardCssSelector = ConfigurationManager.AppSettings["SASCreditCardCssSelector"],
                CardHolderFirstCssSelector = ConfigurationManager.AppSettings["SASCardHolder"],
                InfoOrReciept = ConfigurationManager.AppSettings["SASRecieptCssLink"],
                VAT = ConfigurationManager.AppSettings["SASVAT"],
            };

            SelectorList.Add(SASselector);

            var HOTELSselector = new SelectorObject
            {
                HomeUrl = ConfigurationManager.AppSettings["HOTELSHomeUrl"],
                LoginLinkCssPath = ConfigurationManager.AppSettings["HOTELSloginLinkCssPath"],
                LoginUserNameCssSelector = ConfigurationManager.AppSettings["HOTELSLoginUserNameCssSelector"],
                LoginUserName = ConfigurationManager.AppSettings["HOTELSLoginUserName"],
                LoginUserPasswordCssSelector = ConfigurationManager.AppSettings["HOTELSLoginUserPasswordCssSelector"],
                LoginUserPassword = ConfigurationManager.AppSettings["HOTELSLoginUserPassword"],
                LoginButtonCssSelector = ConfigurationManager.AppSettings["HOTELSLoginButtonCssSelector"],
                EndedBookings = ConfigurationManager.AppSettings["HOTELSEndedBookingCssSelector"],
                MyBookingsUrl = ConfigurationManager.AppSettings["HOTELSMyBookingsUrl"],
                BookingItemsCssSelector = ConfigurationManager.AppSettings["HOTELSBookingItemsCssSelector"],
                BookingNumberCssSelector = ConfigurationManager.AppSettings["HOTELSBookingNumberCssSelector"],
                BookingTripsCssSelector = ConfigurationManager.AppSettings["HOTELSBookingTripsCssSelector"],
                ShowBookingItemLinkTextSelector = ConfigurationManager.AppSettings["HOTELSShowBookingItemLinkTextSelector"],
                SegmentItemsCssSelector = ConfigurationManager.AppSettings["HOTELSSegmentItemsCssSelector"],
                OutboundDepartureDateCssSelector = ConfigurationManager.AppSettings["HOTELSOutboundDepartureDateCssSelector"],
                OutboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["HOTELSOutboundDepartureTimeCssSelector"],
                OutboundDepartureCityCssSelector = ConfigurationManager.AppSettings["HOTELSOutboundDepartureCityCssSelector"],
                OutboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["HOTELSOutboundArrivalTimeCssSelector"],
                OutboundArrivalDateCssSelector = ConfigurationManager.AppSettings["HOTELSOutboundArrivalDateCssSelector"],
                OutboundArrivalCityCssSelector = ConfigurationManager.AppSettings["HOTELSOutboundArrivalCityCssSelector"],
                InboundDepartureDateCssSelector = ConfigurationManager.AppSettings["HOTELSInboundDepartureDateCssSelector"],
                InboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["HOTELSInboundDepartureTimeCssSelector"],
                InboundDepartureCityCssSelector = ConfigurationManager.AppSettings["HOTELSInboundDepartureCityCssSelector"],
                InboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["HOTELSInboundArrivalTimeCssSelector"],
                InboundArrivalDateCssSelector = ConfigurationManager.AppSettings["HOTELSInboundArrivalDateCssSelector"],
                InboundArrivalCityCssSelector = ConfigurationManager.AppSettings["HOTELSInboundArrivalCityCssSelector"],
                TotalPriceCssSelector = ConfigurationManager.AppSettings["HOTELSTotalPriceCssSelector"],
                CurrencyCssSelector = ConfigurationManager.AppSettings["HOTELSCurrencyCssSelector"],
                CreationDateCssSelector = ConfigurationManager.AppSettings["HOTELSCreationDateCssSelector"],
                MembershipNoCssSelector = ConfigurationManager.AppSettings["HOTELSMembershipNoCssSelector"],
                EmailTextCssSelector = ConfigurationManager.AppSettings["HOTELSEmailTextCssSelector"],
                CreditCardCssSelector = ConfigurationManager.AppSettings["HOTELSCreditCardCssSelector"],
                Name = ConfigurationManager.AppSettings["HOTELSName"],
                CreditcardNumber = ConfigurationManager.AppSettings["HOTELSCreditCardNumberCssSelector"],
                CardHolderFirstCssSelector = ConfigurationManager.AppSettings["HOTELSCardHolderFirstNameCssSelector"],
                CardHolderLastCssSelector = ConfigurationManager.AppSettings["HOTELSCardHolderLastNameCssSelector"],
                TravellersCssSelector = ConfigurationManager.AppSettings["HOTELSTravellersCssSelector"],
                InfoOrReciept = ConfigurationManager.AppSettings["HOTELSRecieptCssLink"],
                VAT = ConfigurationManager.AppSettings["HOTELSVAT"],
            };

            SelectorList.Add(HOTELSselector);


            var EASYPARKselector = new SelectorObject
            {
                HomeUrl = ConfigurationManager.AppSettings["EASYPARKHomeUrl"],
                LoginLinkCssPath = ConfigurationManager.AppSettings["EASYPARKloginLinkCssPath"],
                LoginUserNameCssSelector = ConfigurationManager.AppSettings["EASYPARKLoginUserNameCssSelector"],
                LoginUserName = ConfigurationManager.AppSettings["EASYPARKLoginUserName"],
                LoginUserPasswordCssSelector = ConfigurationManager.AppSettings["EASYPARKLoginUserPasswordCssSelector"],
                LoginUserPassword = ConfigurationManager.AppSettings["EASYPARKLoginUserPassword"],
                LoginButtonCssSelector = ConfigurationManager.AppSettings["EASYPARKLoginButtonCssSelector"],
                MyBookingsUrl = ConfigurationManager.AppSettings["EASYPARKMyBookingsUrl"],
                BookingItemsCssSelector = ConfigurationManager.AppSettings["EASYPARKBookingItemsCssSelector"],
                BookingNumberCssSelector = ConfigurationManager.AppSettings["EASYPARKBookingNumberCssSelector"],
                BookingTripsCssSelector = ConfigurationManager.AppSettings["EASYPARKBookingTripsCssSelector"],
                ShowBookingItemLinkTextSelector = ConfigurationManager.AppSettings["EASYPARKShowBookingItemLinkTextSelector"],
                SegmentItemsCssSelector = ConfigurationManager.AppSettings["EASYPARKSegmentItemsCssSelector"],
                OutboundDepartureDateCssSelector = ConfigurationManager.AppSettings["EASYPARKOutboundDepartureDateCssSelector"],
                OutboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["EASYPARKOutboundDepartureTimeCssSelector"],
                OutboundDepartureCityCssSelector = ConfigurationManager.AppSettings["EASYPARKOutboundDepartureCityCssSelector"],
                OutboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["EASYPARKOutboundArrivalTimeCssSelector"],
                OutboundArrivalDateCssSelector = ConfigurationManager.AppSettings["EASYPARKOutboundArrivalDateCssSelector"],
                OutboundArrivalCityCssSelector = ConfigurationManager.AppSettings["EASYPARKOutboundArrivalCityCssSelector"],
                InboundDepartureDateCssSelector = ConfigurationManager.AppSettings["EASYPARKInboundDepartureDateCssSelector"],
                InboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["EASYPARKInboundDepartureTimeCssSelector"],
                InboundDepartureCityCssSelector = ConfigurationManager.AppSettings["EASYPARKInboundDepartureCityCssSelector"],
                InboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["EASYPARKInboundArrivalTimeCssSelector"],
                InboundArrivalDateCssSelector = ConfigurationManager.AppSettings["EASYPARKInboundArrivalDateCssSelector"],
                InboundArrivalCityCssSelector = ConfigurationManager.AppSettings["EASYPARKInboundArrivalCityCssSelector"],
                TotalPriceCssSelector = ConfigurationManager.AppSettings["EASYPARKTotalPriceCssSelector"],
                CurrencyCssSelector = ConfigurationManager.AppSettings["EASYPARKCurrencyCssSelector"],
                CreationDateCssSelector = ConfigurationManager.AppSettings["EASYPARKCreationDateCssSelector"],
                MembershipNoCssSelector = ConfigurationManager.AppSettings["EASYPARKMembershipNoCssSelector"],
                EmailTextCssSelector = ConfigurationManager.AppSettings["EASYPARKEmailTextCssSelector"],
                CreditCardCssSelector = ConfigurationManager.AppSettings["EASYPARKCreditCardCssSelector"],
                InfoOrReciept = ConfigurationManager.AppSettings["EASYPARKRecieptCssLink"],
                CardHolderFirstCssSelector = ConfigurationManager.AppSettings["EASYPARKCardHolder"],
                VAT = ConfigurationManager.AppSettings["EASYPARKVAT"],
                Name = ConfigurationManager.AppSettings["EASYPARKRegNumber"],

            };

            SelectorList.Add(EASYPARKselector);




			var CABONLINEselector = new SelectorObject
			{
				HomeUrl = ConfigurationManager.AppSettings["CABONLINEHomeUrl"],
				LoginLinkCssPath = ConfigurationManager.AppSettings["CABONLINEloginLinkCssPath"],
				LoginUserNameCssSelector = ConfigurationManager.AppSettings["CABONLINELoginUserNameCssSelector"],
				LoginUserName = ConfigurationManager.AppSettings["CABONLINELoginUserName"],
				LoginUserPasswordCssSelector = ConfigurationManager.AppSettings["CABONLINELoginUserPasswordCssSelector"],
				LoginUserPassword = ConfigurationManager.AppSettings["CABONLINELoginUserPassword"],
				LoginButtonCssSelector = ConfigurationManager.AppSettings["CABONLINELoginButtonCssSelector"],
				MyBookingsUrl = ConfigurationManager.AppSettings["CABONLINEMyBookingsUrl"],
				BookingItemsCssSelector = ConfigurationManager.AppSettings["CABONLINEBookingItemsCssSelector"],
				BookingNumberCssSelector = ConfigurationManager.AppSettings["CABONLINEBookingNumberCssSelector"],
				BookingTripsCssSelector = ConfigurationManager.AppSettings["CABONLINEBookingTripsCssSelector"],
				ShowBookingItemLinkTextSelector = ConfigurationManager.AppSettings["CABONLINEShowBookingItemLinkTextSelector"],
				SegmentItemsCssSelector = ConfigurationManager.AppSettings["CABONLINESegmentItemsCssSelector"],
				OutboundDepartureDateCssSelector = ConfigurationManager.AppSettings["CABONLINEOutboundDepartureDateCssSelector"],
				OutboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["CABONLINEOutboundDepartureTimeCssSelector"],
				OutboundDepartureCityCssSelector = ConfigurationManager.AppSettings["CABONLINEOutboundDepartureCityCssSelector"],
				OutboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["CABONLINEOutboundArrivalTimeCssSelector"],
				OutboundArrivalDateCssSelector = ConfigurationManager.AppSettings["CABONLINEOutboundArrivalDateCssSelector"],
				OutboundArrivalCityCssSelector = ConfigurationManager.AppSettings["CABONLINEOutboundArrivalCityCssSelector"],
				InboundDepartureDateCssSelector = ConfigurationManager.AppSettings["CABONLINEInboundDepartureDateCssSelector"],
				InboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["CABONLINEInboundDepartureTimeCssSelector"],
				InboundDepartureCityCssSelector = ConfigurationManager.AppSettings["CABONLINEInboundDepartureCityCssSelector"],
				InboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["CABONLINEInboundArrivalTimeCssSelector"],
				InboundArrivalDateCssSelector = ConfigurationManager.AppSettings["CABONLINEInboundArrivalDateCssSelector"],
				InboundArrivalCityCssSelector = ConfigurationManager.AppSettings["CABONLINEInboundArrivalCityCssSelector"],
				TotalPriceCssSelector = ConfigurationManager.AppSettings["CABONLINETotalPriceCssSelector"],
				CurrencyCssSelector = ConfigurationManager.AppSettings["CABONLINECurrencyCssSelector"],
				CreationDateCssSelector = ConfigurationManager.AppSettings["CABONLINECreationDateCssSelector"],
				MembershipNoCssSelector = ConfigurationManager.AppSettings["CABONLINEMembershipNoCssSelector"],
				EmailTextCssSelector = ConfigurationManager.AppSettings["CABONLINEEmailTextCssSelector"],
				CreditCardCssSelector = ConfigurationManager.AppSettings["CABONLINECreditCardCssSelector"],
				CardHolderFirstCssSelector = ConfigurationManager.AppSettings["CABONLINECardHolder"],
				InfoCssSelector = ConfigurationManager.AppSettings["CABONLINEInfoCssSelector"],
				InfoOrReciept = ConfigurationManager.AppSettings["CABONLINERecieptCssLink"],
				VAT = ConfigurationManager.AppSettings["CABONLINEVAT"],
			};

			SelectorList.Add(CABONLINEselector);

            var NORWEIGANselector = new SelectorObject
            {
                HomeUrl = ConfigurationManager.AppSettings["NORWEIGANHomeUrl"],
                LoginLinkCssPath = ConfigurationManager.AppSettings["NORWEIGANloginLinkCssPath"],
                LoginUserNameCssSelector = ConfigurationManager.AppSettings["NORWEIGANLoginUserNameCssSelector"],
                LoginUserName = ConfigurationManager.AppSettings["NORWEIGANLoginUserName"],
                LoginUserPasswordCssSelector = ConfigurationManager.AppSettings["NORWEIGANLoginUserPasswordCssSelector"],
                LoginUserPassword = ConfigurationManager.AppSettings["NORWEIGANLoginUserPassword"],
                LoginButtonCssSelector = ConfigurationManager.AppSettings["NORWEIGANLoginButtonCssSelector"],
                MyBookingsUrl = ConfigurationManager.AppSettings["NORWEIGANMyBookingsUrl"],
                BookingItemsCssSelector = ConfigurationManager.AppSettings["NORWEIGANBookingItemsCssSelector"],
                BookingNumberCssSelector = ConfigurationManager.AppSettings["NORWEIGANBookingNumberCssSelector"],
                BookingTripsCssSelector = ConfigurationManager.AppSettings["NORWEIGANBookingTripsCssSelector"],
                ShowBookingItemLinkTextSelector = ConfigurationManager.AppSettings["NORWEIGANShowBookingItemLinkTextSelector"],
                SegmentItemsCssSelector = ConfigurationManager.AppSettings["NORWEIGANSegmentItemsCssSelector"],
                OutboundDepartureDateCssSelector = ConfigurationManager.AppSettings["NORWEIGANOutboundDepartureDateCssSelector"],
                OutboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["NORWEIGANOutboundDepartureTimeCssSelector"],
                OutboundDepartureCityCssSelector = ConfigurationManager.AppSettings["NORWEIGANOutboundDepartureCityCssSelector"],
                OutboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["NORWEIGANOutboundArrivalTimeCssSelector"],
                OutboundArrivalDateCssSelector = ConfigurationManager.AppSettings["NORWEIGANOutboundArrivalDateCssSelector"],
                OutboundArrivalCityCssSelector = ConfigurationManager.AppSettings["NORWEIGANOutboundArrivalCityCssSelector"],
                InboundDepartureDateCssSelector = ConfigurationManager.AppSettings["NORWEIGANInboundDepartureDateCssSelector"],
                InboundDepartureTimeCssSelector = ConfigurationManager.AppSettings["NORWEIGANInboundDepartureTimeCssSelector"],
                InboundDepartureCityCssSelector = ConfigurationManager.AppSettings["NORWEIGANInboundDepartureCityCssSelector"],
                InboundArrivalTimeCssSelector = ConfigurationManager.AppSettings["NORWEIGANInboundArrivalTimeCssSelector"],
                InboundArrivalDateCssSelector = ConfigurationManager.AppSettings["NORWEIGANInboundArrivalDateCssSelector"],
                InboundArrivalCityCssSelector = ConfigurationManager.AppSettings["NORWEIGANInboundArrivalCityCssSelector"],
                TotalPriceCssSelector = ConfigurationManager.AppSettings["NORWEIGANTotalPriceCssSelector"],
                CurrencyCssSelector = ConfigurationManager.AppSettings["NORWEIGANCurrencyCssSelector"],
                CreationDateCssSelector = ConfigurationManager.AppSettings["NORWEIGANCreationDateCssSelector"],
                MembershipNoCssSelector = ConfigurationManager.AppSettings["NORWEIGANMembershipNoCssSelector"],
                EmailTextCssSelector = ConfigurationManager.AppSettings["NORWEIGANEmailTextCssSelector"],
                CreditCardCssSelector = ConfigurationManager.AppSettings["NORWEIGANCreditCardCssSelector"],
                InfoOrReciept = ConfigurationManager.AppSettings["NORWEIGANRecieptCssLink"],
                CardHolderFirstCssSelector = ConfigurationManager.AppSettings["NORWEIGANCardHolder"],
                VAT = ConfigurationManager.AppSettings["NORWEIGANVAT"],
                TravellersCssSelector = ConfigurationManager.AppSettings["NORWEIGANTravellersCssSelector"]
            };

            SelectorList.Add(NORWEIGANselector);


			return SelectorList;

		}
	}
}

