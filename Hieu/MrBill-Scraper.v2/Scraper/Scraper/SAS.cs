using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Scraper.MrBillServices;
using Scraper.MrBillTransactionServices;
using Scraper.State;

namespace Scraper
{

    // ReSharper disable once InconsistentNaming
    public class SAS : IScraper
    {
        public List<IWebElement> NewBookingList { get; set; }
        public List<string> BookingRefList { get; set; }
        public SelectorObject SelectorData { get; set; }
        public UserSupplierDetail UserSupplier { get; set; }
        public TransactionDTO ResultTransaction { get; set; }


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
                    if (BookingRefList.Contains(bookRef)) continue;

                    NewBookingList.Add(item);
                }
            }
            catch (Exception exception)
            {
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                mrBillServicesClient.WriteScraperLog((int)ScraperState.EnumLogType.GetListNewBookingsEx, UserSupplier.UserId, UserSupplier.SupplierId, exception.Message +"|||||"+ exception.StackTrace, DateTime.Now);
            }
        }
 
        public void GetBookingsAndSaveThem(IWebDriver driver)
        {
            foreach (var item in NewBookingList)
            {
                Thread.Sleep(1500);
                MrBillServicesClient mrBillServicesClient = new MrBillServicesClient();
                var provider = CultureInfo.CreateSpecificCulture("sv-SE");
                provider.DateTimeFormat.AbbreviatedDayNames = new[] { "sö", "må", "ti", "on", "to", "fr", "lö" };
                try
                {

                    #region get transaction data and save file
                    ResultTransaction = new TransactionDTO();
                    ResultTransaction.BookingRef = item.FindElement(By.CssSelector(SelectorData.BookingNumberCssSelector)).Text.Trim();
                    ResultTransaction.Destination = item.FindElement(By.CssSelector(SelectorData.BookingTripsCssSelector)).Text.Trim();
                    var url = driver.Url;

                    var link = item.FindElement(By.LinkText(SelectorData.ShowBookingItemLinkTextSelector)).GetAttribute("href");
                    IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
                    jscript.ExecuteScript("window.open()");
                    List<string> handles = driver.WindowHandles.ToList<string>();
                    driver.SwitchTo().Window(handles.Last());
                    driver.Navigate().GoToUrl(url);
                    jscript.ExecuteScript(link);

                    Thread.Sleep(6000);
                    ReadOnlyCollection<IWebElement> segments;
                    try
                    {
                        segments = driver.FindElements(By.CssSelector(SelectorData.SegmentItemsCssSelector));
                        if (segments.Count < 1)
                        {
                            jscript.ExecuteScript("window.close()");
                            driver.SwitchTo().Window(handles.First());

                            continue;
                        }
                    }

                    catch (Exception)
                    {
                        jscript.ExecuteScript("window.close()");
                        driver.SwitchTo().Window(handles.First());
                        continue;
                    }
                    var boundDep = segments[3];
                    var boundRet = segments[4];
                    var boundPas = segments[5];
                    string dateAir =
                        boundDep.FindElement(By.CssSelector(SelectorData.InboundDepartureDateCssSelector)).Text.Trim() +
                        " ";
                    var tmp = dateAir +
                        boundDep.FindElement(By.CssSelector(SelectorData.InboundDepartureTimeCssSelector)).Text.Split('-')[0].Trim();

                    ResultTransaction.AirDepTime1 = DateTime.Parse(tmp, provider);

                    tmp = dateAir +
                          boundDep.FindElements(By.CssSelector(SelectorData.InboundDepartureTimeCssSelector))
                              .Last()
                              .Text.Split('-')[1].Trim();
                    ResultTransaction.AirDepTime2 = DateTime.Parse(tmp, provider);
                    if (DateTime.Compare(ResultTransaction.AirDepTime2, ResultTransaction.AirDepTime1) < 0)
                    {
                       ResultTransaction.AirDepTime2=ResultTransaction.AirDepTime2.AddDays(1);
                    }
                    ResultTransaction.CityDep1 =
                        boundDep.FindElement(By.CssSelector(SelectorData.InboundDepartureCityCssSelector)).Text.Trim();
                    ResultTransaction.CityDep2 =
                       boundDep.FindElements(By.CssSelector(SelectorData.InboundDepartureCityCssSelector)).Last().Text.Trim();


                    dateAir =
                        boundRet.FindElement(By.CssSelector(SelectorData.InboundDepartureDateCssSelector)).Text.Trim() +
                        " ";
                    tmp = dateAir +
                        boundRet.FindElement(By.CssSelector(SelectorData.InboundDepartureTimeCssSelector)).Text.Split('-')[0].Trim();
                    ResultTransaction.AirRetTime1 = DateTime.Parse(tmp, provider);

                    tmp = dateAir +
                          boundRet.FindElements(By.CssSelector(SelectorData.InboundDepartureTimeCssSelector))
                              .Last()
                              .Text.Split('-')[1].Trim();
                    ResultTransaction.AirRetTime2 = DateTime.Parse(tmp, provider);
                    if (DateTime.Compare(ResultTransaction.AirRetTime2, ResultTransaction.AirRetTime1) < 0)
                    {
                        ResultTransaction.AirRetTime2 = ResultTransaction.AirRetTime2.AddDays(1);
                    }
                    ResultTransaction.CityRet1 =
                      boundRet.FindElement(By.CssSelector(SelectorData.InboundDepartureCityCssSelector)).Text.Trim();
                    ResultTransaction.CityRet2 =
                       boundRet.FindElements(By.CssSelector(SelectorData.InboundDepartureCityCssSelector)).Last().Text.Trim();



                    ResultTransaction.AddedDate = DateTime.Now;
                    ResultTransaction.Attendees = "SAS";
                    ResultTransaction.Country = "SE";

                    ResultTransaction.CategoryID = 1;

                    ResultTransaction.Description = "";


                    ResultTransaction.ExportStatus = 1;
                    ResultTransaction.PaymentID = 1;
                    ResultTransaction.Price = float.Parse(driver.FindElement(By.CssSelector(SelectorData.TotalPriceCssSelector)).Text.Trim());

                    ResultTransaction.Status = 1;
                    ResultTransaction.PriceCurrency = "SEK";
                    var travellers = boundPas.FindElements(By.CssSelector(SelectorData.TravellersCssSelector));
                    ResultTransaction.Traveller = "";
                    foreach (var traveller in travellers)
                    {
                        tmp = traveller.Text.Trim();
                        tmp = tmp.Substring(0, tmp.LastIndexOf(" ", StringComparison.Ordinal));
                        ResultTransaction.Traveller += tmp + ",";
                    }
                    if (ResultTransaction.Traveller.Length >= 100)
                    {

                        ResultTransaction.Traveller = ResultTransaction.Traveller.Substring(0,99);
                    }
                    else
                    {

                        ResultTransaction.Traveller = ResultTransaction.Traveller.Substring(0,
                            ResultTransaction.Traveller.Length - 1);
                    }
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

                    jscript.ExecuteScript("window.close()");
                    driver.SwitchTo().Window(handles.First());
                    #endregion

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
    }
}