using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MrBillServices.DTO;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMrBillTransactionServices" in both code and config file together.
    [ServiceContract]
    public interface IMrBillTransactionServices
    {
        [OperationContract]
        TransactionCheckInfo[] GetUserTransactionsForCheck(string token, string username, long userID);

        [OperationContract]
        List<TransactionDTO> GetTransactionsForUser(string token, string username, int userId);

        [OperationContract]
        int SaveTransactionSuppliers(string token, string username, UserSupplierInfoDTO userSupplier);

        [OperationContract]
        TransactionDTO GetTransactionById(string token, string username, int transactionId);

        [OperationContract]
        int CreateNewTransaction(string token, string username, TransactionDTO trans);

        [OperationContract]
        string CreateNewTransactionEx(string token, string username, TransactionDTO trans);
        [OperationContract]
        string UpdateTransaction(string token, string username, TransactionDTO trans, string groupName);

        [OperationContract]
        string UpdateTransactionWithMainTrip(string token, string username, TransactionDTO trans,int groupId, int mainTrip);

        [OperationContract]
        string RemoveMainTrip(string token, string username, int mainTrip);

        [OperationContract]
        string ChangeMainTrip(string token, string username, int mainTrip,int groupIdOfGroupTrans, int transIdOfGroupTrans);

        [OperationContract]
        string CreateProject(string token, string username, string projectName, int userId);
        [OperationContract]
        int GetProjectID(string token, string username, string projectName, int userId);
        [OperationContract]
        string CreateSupplier(string token, string username, string supplierName);
        [OperationContract]
        int GetSupplierIDBySupplierName(string supplierName);

        [OperationContract]
        string[] GetAllBookingRefByUserAndSupplier(string token, string username, int userId, int supplierId);
    }
}
