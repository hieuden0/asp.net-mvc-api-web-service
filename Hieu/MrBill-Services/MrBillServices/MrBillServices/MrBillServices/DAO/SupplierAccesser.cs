using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrBillServices.Entities;

namespace MrBillServices.DAO
{
   public class SupplierAccesser
    {
       public SupplierInfo GetSupplierInfoByName(string name)
       {
           using (var mrbillDbEntities = new MrBillEntities())
           {
               name = name.Trim().ToUpper();
               SupplierInfo results = mrbillDbEntities.SupplierInfoes.FirstOrDefault(s => s.Name == name);

               return results;
           }
       }
       public int CreateSupplierInfoByName(string name, string url)
       {
           using (var mrbillDbEntities = new MrBillEntities())
           {
               name = name.Trim().ToUpper();
               SupplierInfo results = mrbillDbEntities.SupplierInfoes.FirstOrDefault(s => s.Name == name);
               if (results == null)
               {
                   results= new SupplierInfo();
                   results.Name = name;
                   results.URL = "";
                   mrbillDbEntities.SupplierInfoes.Add(results);
                   mrbillDbEntities.SaveChanges();
                   return 1;
               }
               return 0;
           }
       }
       public int CreateSupplierInfo(SupplierInfo supplierInfo)
       {
           using (var mrbillDbEntities = new MrBillEntities())
           {
               SupplierInfo results = mrbillDbEntities.SupplierInfoes.FirstOrDefault(s => s.Name == supplierInfo.Name);
               if (results == null)
               {
                   mrbillDbEntities.SupplierInfoes.Add(supplierInfo);
                   mrbillDbEntities.SaveChanges();
                   return 1;
               }
               return 0;
           }
       }

       public List<SupplierInfo> GetSupplierInfosByListId(int[] ids)
       {
           using (var mrbillDbEntities = new MrBillEntities())
           {
               return mrbillDbEntities.SupplierInfoes.Where(s => ids.Contains(s.SupplierID)).ToList();
           }
       }
    }
}
