using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrBillServices.Entities;

namespace MrBillServices.DAO
{
    public class LogAccesser
    {
        public void WriteScraperLog(ScraperLog log)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                mrbillDbEntities.ScraperLogs.Add(log);
                mrbillDbEntities.SaveChanges();
            }
        }
		
    }
}
