using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Scraper.MrBillServices;
using Scraper.MrBillTransactionServices;
using Scraper.State;

namespace Scraper
{
    public class EASYPARK : IScraper
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

                    item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Click();

                    Thread.Sleep(2000);

                    ResultTransaction.AirDepTime1 =
                        DateTime.Parse(item.FindElement(By.CssSelector(SelectorData.OutboundDepartureDateCssSelector)).Text.Trim());
                    ResultTransaction.AirDepTime2 =
                       DateTime.Parse(item.FindElement(By.CssSelector(SelectorData.OutboundArrivalDateCssSelector)).Text.Trim());
                    ResultTransaction.AddedDate = DateTime.Now;
                    ResultTransaction.Attendees = "EASYPARK";
                    ResultTransaction.BookingRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).GetAttribute("href");
                    ResultTransaction.BookingRef = ResultTransaction.BookingRef.Replace("https://epic.easyparksystem.net/epic-ss/proxy/epicfile/parkings/", "").Replace("/pdf", "");
                    ResultTransaction.CityDep1 = ResultTransaction.Destination;
                    ResultTransaction.CityDep2 = ResultTransaction.Destination;
                    ResultTransaction.Country = "SE";

                    ResultTransaction.CategoryID = 3;
                    ResultTransaction.Description = "";
                    ResultTransaction.ExportStatus = 1;
                    ResultTransaction.PaymentID = 1;
                    ResultTransaction.Destination = item.FindElement(By.CssSelector(SelectorData.InboundDepartureCityCssSelector)).Text.Trim();
                    ResultTransaction.Price = float.Parse(item.FindElement(By.CssSelector(SelectorData.TotalPriceCssSelector)).Text.Trim());
                    
                    ResultTransaction.Status = 1;
                    ResultTransaction.PriceCurrency = "SEK";
                    ResultTransaction.Traveller = UserSupplier.UserName;
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
            return BaseSystem.LoginToSiteAndGoToMyBookings(driver, SelectorData, UserSupplier);
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
                string fileName = "Parking_" + ResultTransaction.BookingRef + ".PDF";
                string fileLocation = ConfigurationManager.AppSettings["FILELOCATION"] + "/" + ResultTransaction.ReceiptLink;

                if (!Directory.Exists(fileLocation))
                {
                    Directory.CreateDirectory(fileLocation);
                }
                var source = pathDownload + "/" + fileName;

                File.Copy(source, fileLocation + "/" + fileName , true);
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
                    var bookRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).GetAttribute("href");
                    bookRef = bookRef.Replace("https://epic.easyparksystem.net/epic-ss/proxy/epicfile/parkings/", "").Replace("/pdf", "");
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

