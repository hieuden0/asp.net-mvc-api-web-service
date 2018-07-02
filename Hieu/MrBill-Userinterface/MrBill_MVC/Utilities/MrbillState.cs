using System;

namespace MrBill_MVC.Utilities
{
    public class MrbillState 
    {
        public enum MrbillUserRole
        {
            MrBillAdmin = 1,
            CompanyAdmin = 2,
            OrdinaryUser = 3
        }

        public enum TransSortType
        {
            ExpenseSort = 1,
            AllowanceSort = 2
        }

        public enum Supplier
        {
            SAS = 1,
            HOTELS = 2,
            EASYPARK = 3,
            CABONLINE = 4,
            NORWEIGAN =5
        }
    }
}