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
    public class HOTELS : IScraper
    {


        public List<IWebElement> NewBookingList { get; set; }
        public List<string> BookingRefList { get; set; }
        public SelectorObject SelectorData { get; set; }
        public UserSupplierDetail UserSupplier { get; set; }
        public TransactionDTO ResultTransaction { get; set; }
        public void GetBookingsAndSaveThem(IWebDriver driver)
        {

            MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
            foreach (var item in NewBookingList)
            {

                try
                {
                    ResultTransaction = new TransactionDTO();

                    ResultTransaction.BookingRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Text.Trim();
                    ResultTransaction.BookingRef = ResultTransaction.BookingRef.Split(' ')[1];
                    ResultTransaction.Destination = item.FindElement(By.CssSelector(SelectorData.BookingTripsCssSelector)).Text;
                    var url = driver.Url;

                    var link = item.FindElement(By.CssSelector(SelectorData.ShowBookingItemLinkTextSelector)).GetAttribute("href");
                    IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
                    jscript.ExecuteScript("window.open()");
                    List<string> handles = driver.WindowHandles.ToList<string>();
                    driver.SwitchTo().Window(handles.Last());
                    driver.Navigate().GoToUrl(link);


                    ResultTransaction.AirDepTime1 =
                       DateTime.Parse(driver.FindElement(By.CssSelector(SelectorData.OutboundArrivalDateCssSelector)).Text.Trim(), CultureInfo.CreateSpecificCulture("sv-SE"));
                    ResultTransaction.AirDepTime2 =
                       DateTime.Parse(driver.FindElement(By.CssSelector(SelectorData.OutboundDepartureDateCssSelector)).Text.Trim(), CultureInfo.CreateSpecificCulture("sv-SE"));
                    ResultTransaction.AddedDate = DateTime.Now;
                    ResultTransaction.Attendees = "HOTELS";
                    ResultTransaction.CityDep1 = ResultTransaction.Destination;
                    ResultTransaction.CityDep2 = ResultTransaction.Destination;
                    ResultTransaction.Country = "SE";

                    ResultTransaction.CategoryID = 1;
                    try
                    {
                        ResultTransaction.Description = driver.FindElement(By.CssSelector(SelectorData.OutboundDepartureTimeCssSelector)).Text.Trim();
                        ResultTransaction.Description += ", " + driver.FindElement(By.CssSelector(SelectorData.OutboundDepartureCityCssSelector)).Text.Trim();
                    }
                    catch 
                    {
                        ResultTransaction.Description = "";
                    }
                   
                    ResultTransaction.ExportStatus = 1;
                    ResultTransaction.PaymentID = 1;
                   
                    //ResultTransaction.Price = float.Parse(driver.FindElement(By.CssSelector(SelectorData.TotalPriceCssSelector)).Text.Replace("SEK", "").Replace("€","").Replace(" ", ""));
                     ResultTransaction.Price = float.Parse(driver.FindElement(By.CssSelector(SelectorData.TotalPriceCssSelector))
                            .GetAttribute("textContent").Replace("SEK", "").Replace("€", "").Replace((char)160, ' ').Trim().Replace(" ", ""));
                     ResultTransaction.Status = 1;
                    ResultTransaction.PriceCurrency = "SEK";
                    ResultTransaction.Traveller = driver.FindElement(By.CssSelector(SelectorData.MembershipNoCssSelector)).Text.Trim();
                    ResultTransaction.Traveller += ", " + driver  .FindElement(By.CssSelector(SelectorData.TravellersCssSelector)).Text.Trim();
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
                    if (
                        !BaseSystem.TakeScreenShot(driver, UserSupplier, ResultTransaction.BookingRef,
                            ResultTransaction.ReceiptLink, new Size(900, 1000), "scroll(0, 500)"))
                    {
                        continue;
                    }
                    ;
                    ResultTransaction.ReceiptLink += "/" + ResultTransaction.BookingRef + ".JPG";
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
            return BaseSystem.LoginToSiteAndGoToMyBookings(driver, SelectorData, UserSupplier);
        }

        public void GetListNewBookings(IWebDriver driver)
        {
            try
            {
                NewBookingList = new List<IWebElement>();
                IEnumerable<IWebElement> BookingList = driver.FindElements(By.CssSelector(SelectorData.BookingItemsCssSelector));
               
                foreach (var item in BookingList)
                {
                    var bookRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Text.Trim();
                    bookRef = bookRef.Split(' ')[1];
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
