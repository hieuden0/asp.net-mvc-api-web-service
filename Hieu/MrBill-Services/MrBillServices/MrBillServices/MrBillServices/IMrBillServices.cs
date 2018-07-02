using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MrBillServices.DTO;
using MrBillServices.Entities;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMrBillServices" in both code and config file together.
    [ServiceContract]
    public interface IMrBillServices
    {
        [OperationContract]
        MrBillDTO[] GetMrBillUsers(string token, string username);

        [OperationContract]
        SupplierInfoDTO[] GetSupplierByListId(string token, string username,int[] ids);
        [OperationContract]
        ContactInfoDTO[] GetMrBillUsersWithoutSuppliers(string token, string username);

        [OperationContract]
        int testConnection();

        [OperationContract]
        bool SendEmailReport(string username, List<string> emailList, int month, int year, int SortType);

        [OperationContract]
        string pdfReview(string username, int month, int year, int SortType);

        [OperationContract]
        byte[] DownloadReport(string username, List<string> emailList, int month, int year, int SortType);

        [OperationContract]
        string WriteScraperLog(int? type, int? userId, int? supplierId, string message, DateTime time);
    }
}
