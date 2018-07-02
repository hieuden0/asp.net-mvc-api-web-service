using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MrBillServices.DTO;
using MrBillServices.DTO.Mobile;
using MrBillServices.Entities;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMrBillMobileServices" in both code and config file together.
    [ServiceContract]
    public interface IMrBillMobileServices
    {
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        LocalUser Login(string username, string password);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        ServiceResponeBase RegisterUser(UserProfile model);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        bool SendEmailReport(string username, List<string> emailList, int month, int year,string languageCode);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        List<TransactionObject> GetUserTrans(string username);

        //[OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ServiceResponeBase UploadTransacitonAndFile(TransactionObject model);

        //[OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ServiceResponeBase EditTransaction(TransactionObject model);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string AddOrUpdateSetting(string username, SettingDTO setting);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        SettingDTO GetSetingById(string username, int userId);

        //[OperationContract]
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //AppWebService.TransactionObject GetTransactionById(string username, string transactionId);


    }
}
