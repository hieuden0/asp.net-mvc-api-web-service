using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using Scraper.MrBillServices;
using Scraper.MrBillTransactionServices;
using Scraper.State;

namespace Scraper
{
    class NORWEIGAN : IScraper
    {
        public List<IWebElement> NewBookingList { get; set; }
        public List<string> BookingRefList { get; set; }
        public SelectorObject SelectorData { get; set; }
        public UserSupplierDetail UserSupplier { get; set; }
        public TransactionDTO ResultTransaction { get; set; }

        public void GetBookingsAndSaveThem(IWebDriver driver)
        {

            Thread.Sleep(1500);
            MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
            foreach (var item in NewBookingList)
            {

                try
                {

                    ResultTransaction = new TransactionDTO();
                    ResultTransaction.BookingRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Text.Trim();
                    
                    var url = driver.Url;
                    var link = item.FindElement(By.CssSelector(SelectorData.ShowBookingItemLinkTextSelector)).GetAttribute("href");
                    IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
                    jscript.ExecuteScript("window.open()");
                    List<string> handles = driver.WindowHandles.ToList<string>();
                    driver.SwitchTo().Window(handles.Last());
                    driver.Navigate().GoToUrl(link);

                    driver.FindElement(By.CssSelector(SelectorData.InfoOrReciept)).Click();
                    Thread.Sleep(2000);
                    var items = driver.FindElements(By.CssSelector(SelectorData.MembershipNoCssSelector));
                    string tmp =
                        items[0].FindElement(By.CssSelector(SelectorData.OutboundDepartureDateCssSelector)).Text.Trim() + " ";
                   
                    ResultTransaction.AirDepTime1 =
                     DateTime.Parse(tmp + items[0].FindElement(By.CssSelector(SelectorData.OutboundDepartureTimeCssSelector)).Text.Trim(), CultureInfo.CreateSpecificCulture("sv-SE"));
                    ResultTransaction.AirDepTime2 =
                    DateTime.Parse(tmp + items[0].FindElement(By.CssSelector(SelectorData.OutboundArrivalTimeCssSelector)).Text.Replace("Ankomst", "").Trim(), CultureInfo.CreateSpecificCulture("sv-SE"));
                    ResultTransaction.CityDep1 =
                        items[0].FindElement(By.CssSelector(SelectorData.OutboundDepartureCityCssSelector)).Text.Trim();
                    ResultTransaction.CityDep2 =
                        items[0].FindElement(By.CssSelector(SelectorData.OutboundArrivalCityCssSelector)).Text.Trim();
                    if (items.Count > 1)
                    {
                        tmp =
                        items[1].FindElement(By.CssSelector(SelectorData.OutboundDepartureDateCssSelector)).Text.Trim() + " ";
                        ResultTransaction.AirRetTime1 =
                     DateTime.Parse(tmp + items[1].FindElement(By.CssSelector(SelectorData.OutboundDepartureTimeCssSelector)).Text.Trim(), CultureInfo.CreateSpecificCulture("sv-SE"));
                        ResultTransaction.AirRetTime2 =
                        DateTime.Parse(tmp + items[1].FindElement(By.CssSelector(SelectorData.OutboundArrivalTimeCssSelector)).Text.Replace("Ankomst", "").Trim(), CultureInfo.CreateSpecificCulture("sv-SE"));
                        ResultTransaction.CityRet1 =
                            items[1].FindElement(By.CssSelector(SelectorData.OutboundDepartureCityCssSelector)).Text.Trim();
                        ResultTransaction.CityRet2 =
                            items[1].FindElement(By.CssSelector(SelectorData.OutboundArrivalCityCssSelector)).Text.Trim();
                    }

                    ResultTransaction.AddedDate = DateTime.Now;
                    ResultTransaction.Attendees = "NORWEIGAN";
                   ResultTransaction.Country = "SE";
                    
                    ResultTransaction.CategoryID = 3;
                    ResultTransaction.Description = "";
                    ResultTransaction.ExportStatus = 1;
                    ResultTransaction.PaymentID = 1;
                    tmp = driver.FindElement(By.CssSelector(SelectorData.BookingTripsCssSelector)).Text;

                    ResultTransaction.Destination = Regex.Split(tmp, @"från (.*?)[.]")[1];
                    ResultTransaction.Price = 0;
                    ResultTransaction.Status = 1;
                    ResultTransaction.PriceCurrency = "SEK";
                    items = driver.FindElements(By.CssSelector(SelectorData.TravellersCssSelector));
                    ResultTransaction.Traveller = "";
                    foreach (var traveller in items)
                    {
                        ResultTransaction.Traveller += traveller.Text.Trim() + ",";
                    }
                    ResultTransaction.Traveller = ResultTransaction.Traveller.Substring(0, ResultTransaction.Traveller.Length - 1);
                    ResultTransaction.SupplierID = UserSupplier.SupplierId;
                    ResultTransaction.Units = 1;
                    ResultTransaction.UserID = UserSupplier.UserId;
                    ResultTransaction.Vat1 = 0;
                    if(!SaveFile()) continue;

                    MrBillTransactionServicesClient transactionServices = new MrBillTransactionServicesClient();
                    string resuilt = transactionServices.CreateNewTransactionEx(MrBillAuthEncode.MrBillEncode.GetAuthenticationToken(DateTime.Now.Ticks.ToString(), "Mrbill"), "Mrbill", ResultTransaction);
                    if (resuilt != "1")
                    {
                        mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.GetBookingsAndSaveThemEx, UserSupplier.UserId, UserSupplier.SupplierId, resuilt, DateTime.Now);

                    } 
                    jscript.ExecuteScript("window.close()");
                    driver.SwitchTo().Window(handles.First());
                }
                catch (Exception exception)
                {
                    mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.GetBookingsAndSaveThemEx, UserSupplier.UserId, UserSupplier.SupplierId, exception.Message + "|||" + exception.StackTrace, DateTime.Now);
                   
                    var jscript = driver as IJavaScriptExecutor;
                    List<string> handles = driver.WindowHandles.ToList<string>();
                    jscript.ExecuteScript("window.close()");
                    driver.SwitchTo().Window(handles.First());
                }
            }
        }

        public bool LoginAndGoToBooking(IWebDriver driver)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl(SelectorData.HomeUrl);
                (driver as IJavaScriptExecutor).ExecuteScript("scroll(0, 500)");
                driver.FindElement(By.CssSelector(SelectorData.LoginUserNameCssSelector)).SendKeys(UserSupplier.SupplierUsername);
                driver.FindElement(By.CssSelector(SelectorData.LoginUserPasswordCssSelector)).SendKeys(UserSupplier.SupplierPassword);
                driver.FindElement(By.CssSelector(SelectorData.LoginButtonCssSelector)).Click();
                Thread.Sleep(2000);
                driver.Navigate().GoToUrl(SelectorData.MyBookingsUrl);

                if (String.IsNullOrEmpty(SelectorData.EndedBookings)) return true;
                Thread.Sleep(1500);
                return true;
            }
            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.LoginAndGoToBookingEx, UserSupplier.UserId, UserSupplier.SupplierId, exception.Message + "|||||" + exception.StackTrace, DateTime.Now);
                return false;
            }
        }
        private bool SaveFile()
        {
            try
            {
                ResultTransaction.ReceiptLink = "/uploads/reciept/"
                                                + ResultTransaction.AirDepTime1.Year + "/"
                                                +
                                                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(
                                                    ResultTransaction.AirDepTime1.Month) + "/"
                                                + UserSupplier.SupplierName + "/"
                                                + UserSupplier.UserId;
                string pathDownload = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string fileName = "Resekvitto - " + ResultTransaction.BookingRef + ".PDF";
                string fileLocation = ConfigurationManager.AppSettings["FILELOCATION"] + "/" + ResultTransaction.ReceiptLink;

                if (!Directory.Exists(fileLocation))
                {
                    Directory.CreateDirectory(fileLocation);
                }
                var source = pathDownload + "/" + fileName;

                File.Copy(source, fileLocation + "/" + fileName, true);
                ResultTransaction.ReceiptLink += "/" + fileName;
                File.Delete(source);
                return true;
            }

            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.SaveFileEx, UserSupplier.UserId, UserSupplier.SupplierId, exception.Message + "|||||" + exception.StackTrace, DateTime.Now);
                return false;
            }

        }
        public void GetListNewBookings(IWebDriver driver)
        {
            try
            {
                NewBookingList = new List<IWebElement>();
                IEnumerable<IWebElement> BookingList = driver.FindElements(By.CssSelector(SelectorData.BookingItemsCssSelector));
               
                foreach (var item in BookingList)
                {
                    //var bookRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).GetAttribute("href");
                    var bookRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Text.Trim();
                    if (BookingRefList.Contains(bookRef)) continue;

                    NewBookingList.Add(item);

                }
            }
            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.GetListNewBookingsEx, UserSupplier.UserId, UserSupplier.SupplierId, exception.Message + "|||||" + exception.StackTrace, DateTime.Now);
         
            }
        }
    }
}
