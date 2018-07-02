using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using MrBill_MVC.Controllers;
namespace MrBill_MVC.Controllers
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAppWebService" in both code and config file together.

	[ServiceContract(Namespace = "AppWebService")]
	public interface IAppWebService
	{

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
		[OperationContract]
		AppWebService.LocalUser Login(string username, string password);

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]   
		[OperationContract]
		List<AppWebService.TransactionObject> GetUserTrans(string username);

		[OperationContract] //UriTemplate = 
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
		string UploadFile(Stream fileContents);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AppWebService.ResponeBase UploadFileAndInfo(AppWebService.TransactionObject model);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AppWebService.ResponeBase EditTransaction(AppWebService.TransactionObject model);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AppWebService.TransactionObject GetTransactionById(string username, string transactionId);

	}
	


}
