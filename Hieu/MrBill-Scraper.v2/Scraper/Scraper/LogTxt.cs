using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    class LogTxt
    {
        public static void WriteLineLog(string logString, int logTitle)
        {
            DateTime dateNow = DateTime.Now;
            dateNow = dateNow.ToUniversalTime();
            try
            {
                string currentLocation = AppDomain.CurrentDomain.BaseDirectory;
                string logTitleText = "[Error]";
                if (logTitle == 1) logTitleText = "";
                if (logTitle == 2) logTitleText = "[Task]";
                using (
                    System.IO.StreamWriter file =
                        new System.IO.StreamWriter(currentLocation + @"\" + "log.txt", true))
                {
                    file.WriteLine("[" + dateNow.ToString() + "]"+logTitleText+": " + logString);
                }
                Console.Write("[" + dateNow.ToShortDateString() + "]" + logTitleText + ": get log");
            }
            catch (Exception e)
            {
                Console.Write("[Log error " + dateNow.ToShortDateString() + "]: " + e);

            }
        }
    }
}
