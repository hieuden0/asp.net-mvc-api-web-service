using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.State
{
    public class ScraperState
    {
        public enum EnumSuppliers
        {
            SAS = 1,
            HOTELS = 2,
            EASYPARK = 3,
            CABONLINE = 4,
            NORWEIGAN = 5
        }

        public enum EnumLogType
        {
            StartApp = 10,
            EndApp = 11,

            UnknowEx = 0,

            PassGetUsers = 1,
            PassGetSelectors = 2,
            PassGetSuppliers = 3,


            HaveNewBookings = 4,

            SavedNewTranPerSup = 5,
            SavedNewTranPerUser = 6,

          
           
           

            LoginAndGoToBookingEx = -1,
            GetListNewBookingsEx = -2,
            GetBookingsAndSaveThemEx = -3,
            SaveFileEx = -4
        }
    }
}
