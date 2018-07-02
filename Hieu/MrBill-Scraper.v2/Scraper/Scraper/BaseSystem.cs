using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Scraper.MrBillServices;
using Scraper.MrBillTransactionServices;
using Scraper.State;
using SupplierInfoDTO = Scraper.MrBillServices.SupplierInfoDTO;

namespace Scraper
{
    public static class BaseSystem
    {


        static void Main(string[] args)
        {
            var culture = CultureInfo.CreateSpecificCulture("sv-SE");
            var uiCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();

            try
            {
                var selectors = SetupAndGetSelectors();

                var users = UsersToScrape();
                
                var suppliers = GetAllSupplier();

                foreach (var user in users)
                {
                    //foreach (var userSup in user.UserSupplier)
                    foreach (var userSup in user.UserSupplier)
                    //foreach (var userSup in user.UserSupplier)
                    {

                        var driver = new ChromeDriver();
                        try
                        {
                            if (suppliers.SingleOrDefault(s => s.SupplierId == userSup.SupplierId) == null) continue;

                            IScraper processor = SupplierScraperFactory(userSup.SupplierId);
                            processor.UserSupplier = new UserSupplierDetail()
                            {
                                SupplierId = userSup.SupplierId,
                                SupplierPassword = userSup.SupplierPassword,
                                SupplierUsername = userSup.SupplierUsername,
                                SupplierName = suppliers.Single(s => s.SupplierId == userSup.SupplierId).SupplierName,
                                UserId = user.UserId,
                                UserName = user.UserName

                            };
                            processor.SelectorData = GetSelector(selectors, processor.UserSupplier.SupplierId);
                            processor.BookingRefList = GetAllBookingRefByUserAndSup(processor.UserSupplier.UserId, processor.UserSupplier.SupplierId);

                            if (!processor.LoginAndGoToBooking(driver)) { driver.Quit(); continue; }
                            processor.GetListNewBookings(driver);
                            processor.GetBookingsAndSaveThem(driver);
                            driver.Quit();
                        }
                        catch (Exception exception)
                        {
                            try
                            {
                                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.UnknowEx,user.UserId , userSup.SupplierId, exception.Message + "|||"+exception.StackTrace, DateTime.Now);
                            }
                            catch (Exception serviceException)
                            {
                                LogTxt.WriteLineLog(exception.Message+"|||"+exception.StackTrace+"|||"+serviceException.Message+"|||"+serviceException.StackTrace, 0);
                            }
                            driver.Quit();
                        }

                    }
                }
                //Console.ReadKey();
            }
            catch (Exception exception)
            {
                try
                {
                    mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.UnknowEx, null, null, exception.Message + "|||" + exception.StackTrace, DateTime.Now);
                }
                catch (Exception serviceException)
                {
                    LogTxt.WriteLineLog(exception.Message + "|||" + exception.StackTrace + "|||" + serviceException.Message + "|||" + serviceException.StackTrace, 0);
                }
            }
           
        }

        private static List<MrBillDTO> UsersToScrape()
        {
            var mrbillServices = new MrBillServicesClient();

            var allUserData = mrbillServices.GetMrBillUsers(MrBillAuthEncode.MrBillEncode.GetAuthenticationToken(DateTime.Now.Ticks.ToString(), "Mrbill"), "Mrbill");
            //allUserData = allUserData.Where(u => u.UserId == 96).ToArray();
            return allUserData.ToList();
        }

        private static List<SupplierInfoDTO> GetAllSupplier()
        {
            var mrbillServices = new MrBillServicesClient();
            List<int> ids = new List<int>();
            ids.Add((int)ScraperState.EnumSuppliers.SAS);
            ids.Add((int)ScraperState.EnumSuppliers.HOTELS);
            ids.Add((int)ScraperState.EnumSuppliers.CABONLINE);
            ids.Add((int)ScraperState.EnumSuppliers.EASYPARK);
            ids.Add((int)ScraperState.EnumSuppliers.NORWEIGAN);
            var allSupplier = mrbillServices.GetSupplierByListId(MrBillAuthEncode.MrBillEncode.GetAuthenticationToken(DateTime.Now.Ticks.ToString(), "Mrbill"), "Mrbill", ids.ToArray());

            return allSupplier.ToList();
        }
        private static IScraper SupplierScraperFactory(int supplierId)
        {
            if (supplierId == (int)ScraperState.EnumSuppliers.SAS)
            {
                return new SAS();
            }
            if (supplierId == (int)ScraperState.EnumSuppliers.CABONLINE)
            {
                return new CABONLINE();
            }
            if (supplierId == (int)ScraperState.EnumSuppliers.EASYPARK)
            {
                return new EASYPARK();
            }
            if (supplierId == (int)ScraperState.EnumSuppliers.HOTELS)
            {
                return new HOTELS();
            }
            if (supplierId == (int)ScraperState.EnumSuppliers.NORWEIGAN)
            {
                return new NORWEIGAN();
            }
            return null;


        }
        public static bool LoginToSiteAndGoToMyBookings(IWebDriver driver, SelectorObject selectorData, UserSupplierDetail userSupplier)
        {

            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl(selectorData.HomeUrl);
                Thread.Sleep(1500);
                driver.FindElement(By.CssSelector(selectorData.LoginLinkCssPath)).Click();
                driver.FindElement(By.CssSelector(selectorData.LoginUserNameCssSelector)).SendKeys(userSupplier.SupplierUsername);
                driver.FindElement(By.CssSelector(selectorData.LoginUserPasswordCssSelector)).SendKeys(userSupplier.SupplierPassword);
                driver.FindElement(By.CssSelector(selectorData.LoginButtonCssSelector)).Click();
                Thread.Sleep(1500);
                driver.Navigate().GoToUrl(selectorData.MyBookingsUrl);

                if (String.IsNullOrEmpty(selectorData.EndedBookings)) return true;
                Thread.Sleep(1500);
                return true;
            }
            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.LoginAndGoToBookingEx, userSupplier.UserId, userSupplier.SupplierId, exception.Message + "|||||" + exception.StackTrace, DateTime.Now);
                return false;
            }
        }
        private static List<string> GetAllBookingRefByUserAndSup(int userId, int supplierId)
        {
            var mrbillTransactionServices = new MrBillTransactionServicesClient();

            var resuilts = mrbillTransactionServices.GetAllBookingRefByUserAndSupplier(MrBillAuthEncode.MrBillEncode.GetAuthenticationToken(DateTime.Now.Ticks.ToString(), "Mrbill"), "Mrbill", userId, supplierId);

            return resuilts.ToList();
        }

        private static SelectorObject GetSelector(List<SelectorObject> selectors, int supplier)
        {
            if (supplier == (int)ScraperState.EnumSuppliers.SAS)
            {
                return selectors[0];
            }
            if (supplier == (int)ScraperState.EnumSuppliers.CABONLINE)
            {
                return selectors[3];
            }
            if (supplier == (int)ScraperState.EnumSuppliers.EASYPARK)
            {
                return selectors[2];
            }
            if (supplier == (int)ScraperState.EnumSuppliers.HOTELS)
            {
                return selectors[1];
            }
            if (supplier == (int)ScraperState.EnumSuppliers.NORWEIGAN)
            {
                return selectors[4];
            }
            return null;


        }


        public static bool TakeScreenShot(IWebDriver driver,UserSupplierDetail userSupplier, string fileName, string fileLocation, Size screenSize, string scroll)
        {
            try
            {
                var js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.body.style.zoom='90%'");
                js.ExecuteScript(scroll);
                driver.Manage().Window.Size = screenSize;
                var screenshotDriver = driver as ITakesScreenshot;
                var screenshot = screenshotDriver.GetScreenshot();
                fileLocation = ConfigurationManager.AppSettings["FILELOCATION"] + fileLocation;
                //screenshot.SaveAsFile(fileLocation + booking.BookingRef + ".png", ImageFormat.Png);
                if (!Directory.Exists(fileLocation))
                {
                    Directory.CreateDirectory(fileLocation);
                }

                screenshot.SaveAsFile(fileLocation + "/" + fileName + ".JPG", ImageFormat.Jpeg);

                js.ExecuteScript("document.body.style.zoom='100%'");
                return true;
            }
            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.SaveFileEx, userSupplier.UserId, userSupplier.SupplierId, exception.Message + "|||||" + exception.StackTrace, DateTime.Now);
                return false;
            }

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
                TravellersCssSelector = ConfigurationManager.AppSettings["SASTravellersCssSelector"]
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

