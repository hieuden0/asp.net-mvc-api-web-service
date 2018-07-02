//This file is mocking a web API by hitting hard coded data.
var bills = require('./billData').bills;
var _ = require('lodash');
var $ = require('jquery');
var BillUti = require('../actions/BillUtility');
// var hostUrl = "http://120.72.81.104/";
var hostUrl = "http://mrbill.mrorange.nu/";
 //var apiUrl = "http://120.72.81.104/controllers/appWebService.svc/";
 //var apiUrl2 = "http://120.72.81.104/services/MrBillMobileServices.svc/"
 //var apiUrl = "http://120.72.81.104/services/MrBillMobileServices.svc/";
//var apiUrl = "http://localhost:1906/MrBillMobileServices.svc/";//
var apiUrl = "http://mrbill.mrorange.nu/services/MrBillMobileServices.svc/";

var BillApi = {
	getApiUrl: function()
	{
		return apiUrl;
	},

	// getApiUrl2: function()
	// {
	// 	return apiUrl2;
	// },

	getHostUrl: function()
	{
		return hostUrl;
	},

	// NEW LOGIC
	login: function(email, pass)
	{
		var returnData = null;
		// var url = apiUrl + "Login?u=" + email + "&p=" + password;
		var url = apiUrl + "Login"
		$.ajax({
			type : 'POST',
			url : url,
			//timeout: 60000,
			// contentType : "application/json;charset=utf-8",
			contentType : "application/json",
			// dataType : 'json',
			data: JSON.stringify({
				username:email,
				password:pass
			}),
			async: false,
			success : function(data) {
				if(data)
				{
					returnData = data.LoginResult;
				}
			},
			error : function(request, status, error) {	
			},
		});

		return returnData;
	},

	getUserTransations: function(email)
	{
		var returnData = [];
		// var url = apiUrl + "GetTransactions?u=" + email;
		var url = apiUrl + "GetUserTrans"
		$.ajax({
			type : 'POST',
			url : url,
			//timeout: 60000,
			//data : null,
			// contentType : "application/json;charset=utf-8",
			contentType : "application/json",
			data: JSON.stringify({
				username:email
			}),
			dataType : 'json',
			async: false,
			success : function(data) {
				if(data)
				{
					returnData = data.GetUserTransResult;
				}
			},
			error : function(request, status, error) {	
			},
		});

		return returnData;
	},
	insertUserTransaction:function(transaction) {
		var url = apiUrl + "UploadFileAndInfo";
		var isSuccess = false;
		var message = "";
		transaction.reciept = transaction.reciept.split(',')[1];

		if(transaction.vat == ''||transaction.vat== undefined)
		{
			transaction.vat = 0;
		}
		
		$.ajax({
		    url: url,
		    type: 'POST',
		    //timeout: 60000,
		    contentType: 'application/json',
		    data: JSON.stringify({model: transaction}),
		    async: false,
		    success : function(data) {
				isSuccess = data.UploadFileAndInfoResult.Success;
				message = data.UploadFileAndInfoResult.Message;
			},
			error : function(request, status, error) {	
				message = error;
			},
		});

		return {
			isSuccess: isSuccess,
			message: message
		};
	},
	synchronizeAddNew: function(transaction)
	{
		var isSuccess = false;
		var message = "";
		//var url = apiUrl + "UploadFileAndInfo";
		var url = apiUrl + "UploadTransacitonAndFile";
		var base64 = transaction.Reciept.split(',')[1];
		var userId = window.localStorage.getItem("userId");
	

		var newTransaction = {
			//thieu user id
			UserId: userId,
			Date1: transaction.Date1,
			Date2: "",
			Supplier: transaction.Supplier,
			Reference: transaction.Reference,
			Destination: transaction.Destination,
			Product: "",
			Traveller: transaction.Traveller,
			PaymentMethod: "",
			CardHolder: "",
			Price:transaction.Price,
			Vat: transaction.Vat,
			CostCenter: "",
			Reciept: base64,
			ExtraInfo : transaction.ExtraInfo,
			Currency: transaction.Currency
		};
if(newTransaction.Vat==""||newTransaction.Vat==null||newTransaction.Vat=="null"||newTransaction.Vat==undefined)
		newTransaction.Vat = 0;
		$.ajax({
		    url: url,
		    type: 'POST',
		    //timeout: 60000,
		    contentType: 'application/json',
		    data: JSON.stringify({model: newTransaction}),
		    async: false,
		    success : function(data) {
				isSuccess = data.UploadTransacitonAndFileResult.Success;
				message = data.UploadTransacitonAndFileResult.Message;
			},
			error : function(request, status, error) {	
				message = error;
			},
		});

		return {
			isSuccess: isSuccess,
			message: message
		};
	},
	synchronizeEdit: function(transaction)
	{
		var isSuccess = false;
		var message = "";
		var url = apiUrl + "EditTransaction";
		var editTransaction = {
			ExpensDate : transaction.ExpensDate, 
            CostCenter : transaction.CostCenter,
            Supplier : transaction.Supplier,
            Reference : transaction.Reference,
            Date1 : BillUti.formatDate(BillUti.convertDate(transaction.Date1)),
            Date2 : transaction.Date2,
          Destination : transaction.Destination,
          Product : transaction.Product,
          Traveller : transaction.Traveller,
          Price : transaction.Price,
        Currency : transaction.Currency,
        Vat : transaction.Vat,
        CardHolder : transaction.CardHolder,
        Reciept : transaction.Reciept,
        ExtraInfo : transaction.ExtraInfo,
        Id : transaction.Id
		};
		if(editTransaction.Vat==""||editTransaction.Vat==null||editTransaction.Vat=="null"||editTransaction.Vat==undefined)
		editTransaction.Vat = 0;
		
		$.ajax({
		    url: url,
		    type: 'POST',
		    //timeout: 60000,
		    contentType: 'application/json',
		    data: JSON.stringify({model: editTransaction}),
		    async: false,
		    success : function(data) {
				isSuccess = data.EditTransactionResult.Success;
				message = data.EditTransactionResult.Message;
			},
			error : function(request, status, error) {	
				message = error;
			},
		});

		return {
			isSuccess: isSuccess,
			message: message
		};
	}
};

module.exports = BillApi;