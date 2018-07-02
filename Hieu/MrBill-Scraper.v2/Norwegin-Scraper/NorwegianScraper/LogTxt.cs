using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorwegianScraper
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

                using (
                    System.IO.StreamWriter file =
                        new System.IO.StreamWriter(currentLocation + @"\" + dateNow.ToShortDateString() + ".txt", true))
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
