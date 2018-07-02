using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using NorwegianScraper.MrBillService;

namespace NorwegianScraper
{
    class NORWEIGAN : IScraper
    {
        public void GetBookingsAndSaveThemToTextFile(IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, Identity userId)
        {
            Thread.Sleep(2000);
            BaseSystem.bookingList = driver.FindElements(By.CssSelector(selector.BookingItemsCssSelector));

            foreach (var item in BaseSystem.bookingList)
            {

                OpenWindowAndGoToBooking(driver, item);

                var bookingref = "";
                try
                {
                    GetDataFromBookingAndWriteToFile(item, driver, selector, transSup, bookingref, userId);
                }
                catch (Exception e)
                {
                    LogTxt.WriteLineLog(e.ToString(), 0);
                }
                CloseWindow(driver);
            }
        }

        private void CloseWindow(IWebDriver driver)
        {
            List<string> handles = driver.WindowHandles.ToList<string>();
            var javaScriptExecutor = driver as IJavaScriptExecutor;
            if (javaScriptExecutor != null) javaScriptExecutor.ExecuteScript("window.close()");
            driver.SwitchTo().Window(handles.First());
        }

        private void OpenWindowAndGoToBooking(IWebDriver driver, IWebElement item)
        {
            var link = item.GetAttribute("href");
            IJavaScriptExecutor jscript = driver as IJavaScriptExecutor;
            jscript.ExecuteScript("window.open()");
            List<string> handles = driver.WindowHandles.ToList<string>();
            driver.SwitchTo().Window(handles.Last());
            driver.Navigate().GoToUrl(link);


        }

        public void GetDataFromBookingAndWriteToFile(IWebElement item, IWebDriver driver, SelectorObject selector, TransactionSupplier transSup, string bookingRef, Identity userId)
        {
            BookingObject booking = new BookingObject();
            Thread.Sleep(1000);
            string bookingDetail = driver.FindElement(By.CssSelector(selector.BookingNumberCssSelector)).Text;

            string[] detailSubstrings = Regex.Split(bookingDetail, @"from (.*?)[.]");

            var destination = detailSubstrings[1];
            bookingRef = Regex.Split(detailSubstrings[2], @": ")[1];

            booking.OutboundDepartureDate = driver.FindElement(By.CssSelector(selector.OutboundDepartureDateCssSelector)).Text;
            booking.OutboundDepartureCity = driver.FindElement(By.CssSelector(selector.OutboundDepartureCityCssSelector)).Text;
            booking.OutboundDepartureTime = driver.FindElement(By.CssSelector(selector.OutboundDepartureTimeCssSelector)).Text;
            //try
            //{
            booking.OutboundArrivalDate = driver.FindElement(By.CssSelector(selector.OutboundArrivalDateCssSelector)).Text;
            booking.OutboundArrivalCity = driver.FindElement(By.CssSelector(selector.OutboundArrivalCityCssSelector)).Text;
            var time = driver.FindElement(By.CssSelector(selector.OutboundArrivalTimeCssSelector)).Text;

            booking.OutboundArrivalTime = Regex.Split(time, @"Arrival ")[1];
            //}
            //catch
            //{
            //    booking.InboundArrivalDate = "";
            //    booking.InboundArrivalCity = "";
            //    booking.InboundArrivalTime = "";
            //}


            //GENERAL


            booking.BookingRef = bookingRef;
            booking.Destination = destination;

            driver.FindElement(By.CssSelector(selector.InfoOrReciept)).Click();

            booking.Name = "";
            booking.BookingAgent = "Norweigan.se";
            booking.Category = "Norweigan";
            booking.Currency = "";
            booking.TotalPrice = "";

            booking.CreationDate = DateTime.Now.ToShortDateString();
            // booking.MembershipNo = membershipNo;

            booking.Email = transSup.supplierUsername;
            // booking.CreditCard = creditCard + " " + creditCardNumber;
            booking.Travellers = driver.FindElement(By.CssSelector(selector.TravellersCssSelector)).Text;

            Thread.Sleep(2500);
            var year = Convert.ToDateTime(booking.OutboundDepartureDate).Year.ToString();
            var month = Convert.ToDateTime(booking.OutboundDepartureDate).Month;
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var fileLocation = year + "/" + monthName + "/" + "NORWEIGAN" + "/" + userId.id + "/";

            booking.InfoOrReciept = "/uploads/reciept/" + fileLocation + "Resekvitto - " + bookingRef + ".PDF";

            SaveFile(year, monthName, "NORWEIGAN", userId, bookingRef);


            //BaseSystem.WriteToFile(booking);

            BaseSystem.CheckDbAndSaveItem(booking, transSup);
            Thread.Sleep(2500);
        }

        private static void SaveFile(string year, string month, string supplier, Identity userId, string bookingNumber)
        {
            var serverFileLocation = ConfigurationManager.AppSettings["FILELOCATION"];
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads");
            var fileName = "Resekvitto - " + bookingNumber;

            var target = serverFileLocation + @"uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/" + fileName + ".PDF";
            var source = pathDownload + "/" + fileName + ".PDF";

            try
            {
                File.Copy(source, target, true);
            }

            catch (Exception)
            {
                var path = serverFileLocation + @"uploads/reciept/" + year + "/" + month + "/" + supplier + "/" + userId.id + "/";
                Directory.CreateDirectory(path);
                File.Copy(source, target, true);
            }

            File.Delete(source);
        }


    }
}
