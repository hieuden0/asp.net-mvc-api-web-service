using System;

namespace MrBillServices.State
{
    [Serializable]
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
    }
}