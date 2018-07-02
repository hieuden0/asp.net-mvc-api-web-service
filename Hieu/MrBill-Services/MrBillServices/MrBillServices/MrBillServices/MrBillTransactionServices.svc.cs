using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using MrBillServices.DAO;
using MrBillServices.DTO;
using MrBillServices.Entities;
using MrBillServices.Utilities;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MrBillTransactionServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MrBillTransactionServices.svc or MrBillTransactionServices.svc.cs at the Solution Explorer and start debugging.
    public class MrBillTransactionServices : IMrBillTransactionServices
    {

        private readonly TransactionAccesser _transactionAccesser = new TransactionAccesser();
        private readonly UserAccesser _userAccesser = new UserAccesser();
        public List<TransactionDTO> GetTransactionsForUser(string token, string username, int userId)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                var transactions = _transactionAccesser.GetTransactionsByUserId(userId);
                return transactions.Select(MrBillUtility.ConvertToTransactionDTO).ToList();
                //return _transactionAccesser.GetTransactionDTOsByUserId(userId);
            }
            catch (Exception exception)
            {
                List<TransactionDTO> errors = new List<TransactionDTO>();
                errors.Add(new TransactionDTO() { BookingRef = exception.Message });
                return errors;
            }

        }

        public int SaveTransactionSuppliers(string token, string username, UserSupplierInfoDTO userSupplier)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return 0;
            try
            {
                UserSupplierInfo userSupplierInfo = _userAccesser.GetSupplierInfoById(userSupplier.UserId,
                    userSupplier.SupplierId);
                if (userSupplierInfo == null)
                {
                    _userAccesser.CreateUserSupplier(MrBillUtility.ConvertToEntityUserSupplierInfo(userSupplier));

                }
                else
                {
                    userSupplierInfo.Username = userSupplier.Username;
                    userSupplierInfo.Password = userSupplier.Password;
                    _userAccesser.EditUserSupplier(userSupplierInfo);
                }
                return 1;

            }
            catch (Exception exception)
            {
                return 2;
            }

        }

        public TransactionDTO GetTransactionById(string token, string username, int transactionId)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var transEntity = MrbillDbEntities.Transactions.FirstOrDefault(t => t.TransactionID == transactionId);
                    if (transEntity == null)
                        return null;
                    var transModel = MrBillUtility.ConvertToTransactionDTO(transEntity);
                    return transModel;
                }

            }
            catch (Exception exception)
            {
                return new TransactionDTO()
                {
                    BookingRef = exception.Message
                };
            }

        }

        public int CreateNewTransaction(string token, string username, TransactionDTO trans)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return 0;
            try
            {

                Transaction transEntity = MrBillUtility.ConvertToEntityTransaction(trans);
                _transactionAccesser.CreateTransaction(transEntity);
                return 1;

            }
            catch (Exception exception)
            {
                return 2;
            }

        }
        public string CreateNewTransactionEx(string token, string username, TransactionDTO trans)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "0";
            try
            {
                Transaction transEntity = MrBillUtility.ConvertToEntityTransaction(trans);
                _transactionAccesser.CreateTransaction(transEntity);
                return "1";

            }
            catch (Exception exception)
            {
                return exception.Message + "|||" + exception.StackTrace;
            }

        }
        public string UpdateTransaction(string token, string username, TransactionDTO trans, string groupName)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "0";
            try
            {
                Transaction updateTransaction = MrBillUtility.ConvertToEntityTransaction(trans);
                updateTransaction.TransactionID = trans.TransactionId;
                _transactionAccesser.UpdateTransaction(updateTransaction,groupName);
                return "1";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }
        //ConnectTrip
        public string UpdateTransactionWithMainTrip(string token, string username, TransactionDTO trans,int groupId, int mainTrip)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "0";
            try
            {
                Transaction updateTransaction = MrBillUtility.ConvertToEntityTransaction(trans);
                updateTransaction.TransactionID = trans.TransactionId;
                if (groupId == 0)
                {
                    updateTransaction.TransactionGroupID = null;
                }
                else
                {
                    updateTransaction.TransactionGroupID = groupId;
                }
                if (mainTrip == 0)
                {
                    updateTransaction.MainTrip = null;
                }
                else
                {
                    updateTransaction.MainTrip = mainTrip;
                    //updateTransaction.MainTripGroup = mainTrip;
                }                                
                _transactionAccesser.UpdateTransaction(updateTransaction,"");
                return "1";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        public string RemoveMainTrip(string token, string username, int mainTrip)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "0";
            try
            {
                _transactionAccesser.RemoveGroupTransaction(mainTrip);
                return "1";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        public string ChangeMainTrip(string token, string username, int mainTrip, int groupIdOfGroupTrans, int transIdOfGroupTrans)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "0";
            try
            {
                _transactionAccesser.ChangeGroupTransaction(mainTrip, groupIdOfGroupTrans, transIdOfGroupTrans);
                return "1";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }


        public TransactionCheckInfo[] GetUserTransactionsForCheck(string token, string username, long userID)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            List<TransactionCheckInfo> results = new List<TransactionCheckInfo>();
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var transactions = MrbillDbEntities.Transactions.Where(t => t.UserID == userID);
                    foreach (var item in transactions)
                    {
                        results.Add(new TransactionCheckInfo()
                        {
                            BookingReference = item.BookingRef,
                            SupplierName = item.SupplierInfo.Name,
                            TransactionID = item.TransactionID
                        });
                    }
                    return results.ToArray();
                }

            }
            catch
            {
                return null;
            }
        }

        public string CreateProject(string token, string username, string projectName, int userId)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "invalid token";
            using (var MrbillDbEntities = new MrBillEntities())
            {
                var project = MrbillDbEntities.Projects.FirstOrDefault(p => p.UserID == userId && p.No == projectName);
                if (project != null)
                    return "existed";
                try
                {
                    var projectEntity = new Project();
                    projectEntity.No = projectName;
                    projectEntity.UserID = userId;

                    MrbillDbEntities.Projects.Add(projectEntity);
                    MrbillDbEntities.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        public int GetProjectID(string token, string username, string projectName, int userId)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return -1;
            using (var MrbillDbEntities = new MrBillEntities())
            {
                var project = MrbillDbEntities.Projects.First(p => p.UserID == userId && p.No == projectName);
                if (project == null)
                    return -1;
                return project.ProjectID;

            }
        }

        public string CreateSupplier(string token, string username, string supplierName)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "invalid token";
            using (var MrbillDbEntities = new MrBillEntities())
            {
                var supplier = MrbillDbEntities.SupplierInfoes.FirstOrDefault(s => s.Name == supplierName);
                if (supplier != null)
                    return "existed";
                try
                {
                    var supplierEntity = new SupplierInfo();
                    supplierEntity.Name = supplierName;
                    supplierEntity.ResetPasswordUrl = "";
                    supplierEntity.SignUpUrl = "";
                    supplierEntity.URL = "";

                    MrbillDbEntities.SupplierInfoes.Add(supplierEntity);
                    MrbillDbEntities.SaveChanges();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        public int GetSupplierIDBySupplierName(string supplierName)
        {
            using (var MrbillDbEntities = new MrBillEntities())
            {
                var supplier = MrbillDbEntities.SupplierInfoes.FirstOrDefault(s => s.Name == supplierName);
                if (supplier == null)
                    return -1;
                return supplier.SupplierID;
            }
        }

        public string[] GetAllBookingRefByUserAndSupplier(string token, string username, int userId, int supplierId)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return new[] { "invalid token" };


            try
            {
                return _transactionAccesser.GetBookingRefByUserAndSupplier(userId, supplierId).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { ex.Message, ex.StackTrace };
            }

        }
    }
}
