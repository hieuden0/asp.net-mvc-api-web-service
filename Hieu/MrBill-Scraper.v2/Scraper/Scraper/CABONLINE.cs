using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using Scraper.MrBillServices;
using Scraper.MrBillTransactionServices;
using Scraper.State;

namespace Scraper
{
   public class CABONLINE : IScraper
    {

       
        public List<IWebElement> NewBookingList { get; set; }
        public List<string> BookingRefList { get; set; }
        public SelectorObject SelectorData { get; set; }
        public UserSupplierDetail UserSupplier { get; set; }
        public TransactionDTO ResultTransaction { get; set; }
        public void GetBookingsAndSaveThem(IWebDriver driver)
        {

            Thread.Sleep(1500);
            foreach (var item in NewBookingList)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                try
                {

                    ResultTransaction = new TransactionDTO();
                    item.Click();
                    Thread.Sleep(500);
                    ResultTransaction.BookingRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Text.Trim();
                    if (BookingRefList.Contains(ResultTransaction.BookingRef))
                    {
                        continue;
                    }
                    ResultTransaction.Destination = item.FindElement(By.CssSelector(SelectorData.BookingTripsCssSelector)).Text;
                    ResultTransaction.AirDepTime1 =
                        DateTime.Parse(item.FindElement(By.CssSelector(SelectorData.OutboundDepartureDateCssSelector)).Text.Trim());
                    ResultTransaction.AirDepTime2 =
                       DateTime.Parse(item.FindElement(By.CssSelector(SelectorData.OutboundArrivalDateCssSelector)).Text.Trim());
                    ResultTransaction.AddedDate = DateTime.Now;
                    ResultTransaction.Attendees = "CABONLINE";
                    ResultTransaction.CityDep1 = item.FindElement(By.CssSelector(SelectorData.OutboundDepartureCityCssSelector)).Text.Trim();
                    ResultTransaction.CityDep2 = ResultTransaction.Destination;
                    ResultTransaction.Country = "SE";

                    ResultTransaction.CategoryID = 3;
                    ResultTransaction.Description = "";
                    ResultTransaction.ExportStatus = 1;
                    ResultTransaction.PaymentID = 1;
                   ResultTransaction.Price = float.Parse(item.FindElement(By.CssSelector(SelectorData.TotalPriceCssSelector)).Text.Replace("SEK","").Trim());

                    ResultTransaction.Status = 1;
                    ResultTransaction.PriceCurrency = "SEK";
                    ResultTransaction.Traveller = UserSupplier.UserName;
                    ResultTransaction.SupplierID = UserSupplier.SupplierId;
                    ResultTransaction.Units = 1;
                    ResultTransaction.UserID = UserSupplier.UserId;
                    ResultTransaction.Vat1 = 0;
                    ResultTransaction.ReceiptLink = "/uploads/reciept/"
                                              + ResultTransaction.AirDepTime1.Year + "/"
                                              +
                                              CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(
                                                  ResultTransaction.AirDepTime1.Month) + "/"
                                              + UserSupplier.SupplierName + "/"
                                              + UserSupplier.UserId;
                    if (!BaseSystem.TakeScreenShot(driver,UserSupplier, ResultTransaction.BookingRef, ResultTransaction.ReceiptLink,
                        new Size(750, 1000), "scroll(0, 90)"))
                    {
                        continue;
                        
                    }
                    ResultTransaction.ReceiptLink += "/" + ResultTransaction.BookingRef + ".JPG";

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

       

        public void GetListNewBookings(IWebDriver driver)
        {

            try
            {
                NewBookingList = new List<IWebElement>();
                NewBookingList = driver.FindElements(By.CssSelector(SelectorData.BookingItemsCssSelector)).ToList(); 
            }
            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.GetListNewBookingsEx, UserSupplier.UserId, UserSupplier.SupplierId, exception.Message + "|||||" + exception.StackTrace, DateTime.Now);
            }
        }
    }
}
