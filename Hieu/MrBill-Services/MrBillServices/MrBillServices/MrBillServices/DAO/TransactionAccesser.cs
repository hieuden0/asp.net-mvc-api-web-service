using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MrBillServices.DTO;
using MrBillServices.Entities;
using MrBillServices.Utilities;

namespace MrBillServices.DAO
{
    public class TransactionAccesser
    {
        public List<Transaction> GetTransactionsByUserId(int userId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                var transactionList = mrbillDbEntities.Transactions
                    .Include("PaymentType")
                    .Include("SupplierInfo")
                    .Include("Project")
                    .Include("TransactionGroup")
                    //.Where(t => t.UserID == userId).OrderBy(t => t.TransactionGroupID == null).ThenBy(t => t.TransactionGroupID).ThenBy(t => t.MainTrip).ThenBy(t => t.AddedDate).ToList();
                    .Where(t => t.UserID == userId).ToList();
                return transactionList;
            }
        }

        //public List<Transaction> GetTransactionsByUserId(int userId)
        //{
        //    using (var mrbillDbEntities = new MrBillEntities())
        //    {
        //        List<Transaction> listResult = new List<Transaction>();
        //        var transactionList = mrbillDbEntities.Transactions
        //            .Include("PaymentType")
        //            .Include("SupplierInfo")
        //            .Include("Project")
        //            .Include("TransactionGroup")                   
        //            .Where(t => t.UserID == userId && t.TransactionGroupID != null && t.MainTrip == null).OrderBy(t=>t.AddedDate).ToList();
                
        //        foreach (var item in transactionList)
        //        {
        //            listResult.Add(item);
        //            listResult.AddRange(mrbillDbEntities.Transactions
        //            .Include("PaymentType")
        //            .Include("SupplierInfo")
        //            .Include("Project")
        //            .Include("TransactionGroup")
        //            .Where(t => t.UserID == userId && t.TransactionGroupID != null && t.MainTrip == item.TransactionID).OrderBy(t => t.TransactionGroup.Name).OrderBy(t => t.AddedDate).ToList());
        //        }
        //        listResult.AddRange(mrbillDbEntities.Transactions
        //            .Include("PaymentType")
        //            .Include("SupplierInfo")
        //            .Include("Project")
        //            .Include("TransactionGroup")
        //            .Where(t => t.UserID == userId && t.TransactionGroupID == null && t.MainTrip == null).OrderBy(t => t.AddedDate).ToList());
        //        return listResult;
        //    }
        //}


        public List<Transaction> GetTransactionsByUsername(string username)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                var result = mrbillDbEntities.Transactions
                    .Include("PaymentType")
                    .Include("SupplierInfo")
                    .Include("Project")
                    .Include("TransactionGroup")
                    .Where(t => t.UserProfile.UserName == username).ToList();
                return result;
            }          
        }

        public Transaction GetTransactionsByTransactionId(int transactionId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                var result = mrbillDbEntities.Transactions
                    .FirstOrDefault(t => t.TransactionID == transactionId);
                return result;
            }
        }

        public bool UpdateTransaction(Transaction trans, string groupName)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                if (!string.IsNullOrEmpty(groupName) && trans.MainTrip == null)
                {
                    TransactionGroup transactionGroup = new TransactionGroup();
                    CreateTransactionGroupInfoByName(groupName);
                    transactionGroup = GetTransactionGroupByName(groupName);
                    trans.TransactionGroupID = transactionGroup.TransactionGroupID;
                    //trans.MainTripGroup = trans.TransactionID;
                }


                mrbillDbEntities.Transactions.Attach(trans);
                mrbillDbEntities.Entry(trans).State = EntityState.Modified;
                mrbillDbEntities.SaveChanges();
                return true;
            }
        }

        public bool RemoveGroupTransaction(int mainTrip)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                if (mainTrip != 0)
                {
                    List<Transaction> transactionList = new List<Transaction>();
                    transactionList = GetListTransactionByMainTrip(mainTrip);
                    foreach (var item in transactionList)
                    {
                        item.TransactionGroupID = null;
                        item.MainTrip = null;
                        //item.MainTripGroup = null;
                        mrbillDbEntities.Transactions.Attach(item);
                        mrbillDbEntities.Entry(item).State = EntityState.Modified;
                    }                    
                }                
                mrbillDbEntities.SaveChanges();
                return true;
            }
        }

        public bool ChangeGroupTransaction(int mainTrip, int grouIdOfGroupTrans, int transIdOfGroupTrans)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                if (mainTrip != 0)
                {
                    List<Transaction> transactionList = new List<Transaction>();
                    transactionList = GetListTransactionByMainTrip(mainTrip);
                    foreach (var item in transactionList)
                    {
                        item.TransactionGroupID = grouIdOfGroupTrans;
                        item.MainTrip = transIdOfGroupTrans;
                        //item.MainTripGroup = transIdOfGroupTrans;
                        mrbillDbEntities.Transactions.Attach(item);
                        mrbillDbEntities.Entry(item).State = EntityState.Modified;
                    }
                }
                mrbillDbEntities.SaveChanges();
                return true;
            }
        }

        public bool CreateTransaction(Transaction trans)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                mrbillDbEntities.Transactions.Add(trans);
                mrbillDbEntities.SaveChanges();
            }
            return true;
        }

        public List<string> GetBookingRefByUserAndSupplier(int userId, int supplierId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                return mrbillDbEntities.Transactions.Where(t => t.SupplierID == supplierId && t.UserID == userId).Select(t => t.BookingRef).ToList();
            }
        }

        public TransactionGroup GetTransactionGroupByName(string name)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                TransactionGroup results = mrbillDbEntities.TransactionGroups.FirstOrDefault(s => s.Name == name);

                return results;
            }
        }

        public List<Transaction> GetListTransactionByGroupId(int groupId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                List<Transaction> results = mrbillDbEntities.Transactions.Where(s => s.TransactionGroupID == groupId).ToList();

                return results;
            }
        }

        public List<Transaction> GetListTransactionByMainTrip(int mainTrip)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                List<Transaction> results = mrbillDbEntities.Transactions.Where(s => s.MainTrip == mainTrip || s.TransactionID == mainTrip).ToList();

                return results;
            }
        }

        public int CreateTransactionGroupInfoByName(string name)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                name = name.Trim();
                TransactionGroup results = mrbillDbEntities.TransactionGroups.FirstOrDefault(s => s.Name == name);
                if (results == null)
                {
                    results = new TransactionGroup();
                    results.Name = name;
                    mrbillDbEntities.TransactionGroups.Add(results);
                    mrbillDbEntities.SaveChanges();
                    return 1;
                }
                return 0;
            }
        }
    }
}
