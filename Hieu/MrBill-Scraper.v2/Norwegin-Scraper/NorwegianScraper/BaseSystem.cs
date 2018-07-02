using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NorwegianScraper.MrBillService;
using NorwegianScraper.TransactionService;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AuthenticationToken = NorwegianScraper.MrBillService.AuthenticationToken;
using Identity = NorwegianScraper.MrBillService.Identity;
using Transaction = NorwegianScraper.TransactionService.Transaction;
using TransactionSupplier = NorwegianScraper.MrBillService.TransactionSupplier;

namespace NorwegianScraper
{
    public static class BaseSystem
    {
        public static IEnumerable<IWebElement> bookingList;
        static StringBuilder TextFile = new StringBuilder();
        public const string file = @"C:\Projects\bookingInfo.txt";

        private static NORWEIGAN _norweigan = new NORWEIGAN();


        public static IList<IScraper> ClassList = new List<IScraper>
        {
            // LÄGG TILL ALLA KALSSER SOM HAR INTERFACET
           
            _norweigan
			
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

            users = users.Where(u => u.transactionSuppliers.Length != 0);
            foreach (var user in users)
            {
                foreach (var transSup in user.transactionSuppliers)
                {
                    var driver = new ChromeDriver();
                    try
                    {
                        var selector = selectors.Single(s => s.Name.Trim().ToUpper() == transSup.name.Trim().ToUpper());
                        var classRun = ClassList.Single(c => c.GetType().Name.ToUpper() == transSup.name.ToUpper());

                        var userId = transSup.userIdentity;

                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                        driver.Navigate().GoToUrl(selector.HomeUrl);

                        LoginToSiteAndGoToMyBookings(driver, selector, transSup);
                        classRun.GetBookingsAndSaveThemToTextFile(driver, selector, transSup, userId);

                        driver.Quit();
                    }
                    catch (Exception exception)
                    {
                        LogTxt.WriteLineLog(exception.ToString(), 0);
                        driver.Quit();
                    }

                }
            }
            Console.ReadKey();
        }

        private static IEnumerable<MrBillDTO> UsersToScrape()
        {
            var userServiceClient = new MrBillServicePortTypeClient();
            var auth = new AuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

            //sets recieved data to max value
            ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var userList = new List<MrBillDTO>();
            var mrbilluser = userServiceClient.getMrBillUsers(auth).ToList();
            //mrbilluser= mrbilluser.Where(s => s.transactionSuppliers.Where(q => q.name == "NORWEIGAN" ).ToList().Count > 0).ToList();
            //foreach (var user in userServiceClient.getMrBillUsers(auth))
            //{

            //    userList.Add(user);
            //}
            #region for test

            string localUserId = ConfigurationManager.AppSettings["NORWEIGANMrBillUserId"];

            foreach (var user in mrbilluser)
            {
                //if (user.userId == 82433)
                //{
                //    var editsupplier = user.transactionSuppliers.Single(s => s.name == "NORWEIGAN");
                //    editsupplier.supplierUsername = "jens.jonasson@falco.se";
                //    editsupplier.supplierPassword = "falco123";
                //    userList.Add(user);
                //}
                if (user.userId.ToString() == localUserId)
                {
                    user.transactionSuppliers = new TransactionSupplier[1];
                    user.transactionSuppliers[0] = new TransactionSupplier()
                    {
                        supplierUsername = ConfigurationManager.AppSettings["NORWEIGANLoginUserName"],
                        supplierPassword = ConfigurationManager.AppSettings["NORWEIGANLoginUserPassword"],
                        name = "NORWEIGAN",
                        userIdentity = new Identity()
                        {
                            id = user.userId
                        }
                    };
                    //user.transactionSuppliers[0] = new TransactionSupplier()
                    //{
                    //    supplierUsername = "Helen-forsberg@hotmail.com",
                    //    supplierPassword = "Miranda08",
                    //    name = "NORWEIGAN",
                    //    userIdentity = new Identity()
                    //    {
                    //        id = user.userId
                    //    }
                    //};
                    userList.Add(user);
                }
            }
            #endregion
            return userList;
        }

        private static void LoginToSiteAndGoToMyBookings(IWebDriver driver, SelectorObject selector, TransactionSupplier user)
        {

            Thread.Sleep(2000);
            //try
            //{
            //    driver.FindElement(By.CssSelector("#frontpagePopup a.close-reveal-modal")).Click();
            //    Thread.Sleep(2000);
            //}
            //catch (Exception)
            //{
            //    Thread.Sleep(2000);
            //}
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
            var auth = new TransactionService.AuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            var userId = transSup.userIdentity.id.ToString();
            ((BasicHttpBinding)transactionServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
            var oldTrans = transactionServiceClient.getTransactionsForUser(auth, userId);

            var oldRefList = oldTrans.Select(trans => trans.bookingReference).ToList();

            var oldTransNameList = oldTrans.Select(trans => trans.transactionSupplier).ToList();
            if (oldRefList.Contains(booking.BookingRef) && oldTransNameList.Contains(booking.BookingAgent))
                return;
            var transList = new List<Transaction>();

            var transaction = new Transaction();

            var price = Regex.Replace(booking.TotalPrice, "[A-z]+.", string.Empty).Replace(".", ",").Replace(" ", "");
            var curency = "";
            if (booking.Currency != null)
            {
                curency = Regex.Replace(booking.Currency, @"\d+", string.Empty).Replace(".", "").Replace(" ", "");
            }

            transaction.transactionSupplier = transSup.name;
            transaction.bookingReference = booking.BookingRef;


            transaction.destinationCity = booking.OutboundArrivalCity;
            transaction.departureCity = booking.OutboundDepartureCity;

            if (booking.OutboundArrivalDate != null)
            {
                try
                {
                    transaction.date1 = Convert.ToDateTime(booking.OutboundDepartureDate + " " + booking.OutboundDepartureTime);
                    transaction.date2 = Convert.ToDateTime(booking.OutboundArrivalDate + " " + booking.OutboundArrivalTime);
                }


                catch (Exception)
                {


                    transaction.date1 = DateTime.Now;
                    transaction.date2 = DateTime.Now;
                }
            }
            else
            {
                transaction.date1 = Convert.ToDateTime(booking.OutboundDepartureDate + " " + booking.OutboundDepartureTime);
                transaction.date2 = Convert.ToDateTime(booking.OutboundArrivalDate + " " + booking.OutboundArrivalTime);

            }

            //if (transaction.date1 > DateTime.Now)
            //{
            //	return;
            //}

            transaction.date2Specified = true;
            transaction.date1Specified = true;
            transaction.fileLocation = booking.InfoOrReciept ?? "";
            if (!string.IsNullOrEmpty(price))
            {
                transaction.price = Convert.ToDecimal(price);
            }
            else
            {
                transaction.price = 0;
            }
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
                target = @"\\phuket.mrorange.local/InetPub/MrBill/uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".JPG";
                File.Copy(source, target, true);
            }
            catch (Exception)
            {
                var path = @"\\phuket.mrorange.local/InetPub/MrBill/uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/";
                DirectoryInfo di = Directory.CreateDirectory(path);

                target = @"\\phuket.mrorange.local/InetPub/MrBill/uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".JPG";

                File.Copy(source, target, true);
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

            var NORWEIGANselector = new SelectorObject
            {
                Name = ConfigurationManager.AppSettings["NORWEIGANNAME"],
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

