var version = '1.1.0';

var BillUtility = {
	getVersion: function(){
		return	version;
	},


	jsonObjClone: function(item) {
		return JSON.parse(JSON.stringify(item));
	},
	formatDate:function(date){
		if (date) {
			var year = date.getFullYear();
			var month = date.getMonth() + 1;
			var date = date.getDate();
			month = month < 10 ? ('0' + month) : month;
			date = date < 10 ? ('0' + date) : date;
			return year + '-' + month + '-' + date;
		}
	},
	gotoCamera(self,event,$){
		event.preventDefault();
		if($ != undefined && $ != null){
			$('div.bm-cross-button button').click();
		}
		else{
			self.transitionTo('tabs:camera', { transition: 'show-from-bottom' });
		}
	},
	getTime(){
		var d = new Date();
		return d.getTime();
	},
	convertDate(date)
	{
		var newdate = new Date(date);
		if(newdate!=null&&newdate!=undefined  && !isNaN(newdate.getTime())  ) return newdate;
		var dateTimeArr = date.split(" ");
		var dateArr = dateTimeArr[0].split("-");
		var year = parseInt(dateArr[0]);
		var month = parseInt(dateArr[1]) - 1;
		var day = parseInt(dateArr[2]);

		return new Date(year, month, day);
	},
	NewTransaction(){
		return {
			Id:'',
			ExpensDate:'',
			Reference:'',	
			Product:'',
			PaymentMethod:'',
			CardHolder:'',
			IsDisabled:'null',
			Reciept:'null',
			CostCenter:'',
			Traveller:'',
			Date1:new Date(),
			Date2:new Date(),
			DateAdded:new Date(),
			Supplier:'',
			Destination:'',
			Price:'',
			Vat:'',
			Currency:'',
			ExtraInfo:'',
			Description:'',
			action:'new'
		}
	},
	convertTransaction(transaction){
		return {
			Id:transaction.Id,
			ExpensDate:transaction.ExpensDate,
			Reference:transaction.Reference,	
			Product:transaction.Product,
			PaymentMethod:transaction.PaymentMethod,
			CardHolder:transaction.CardHolder,
			IsDisabled:transaction.IsDisabled,
			Reciept:transaction.Reciept,
			CostCenter:transaction.CostCenter,
			Traveller:transaction.Traveller,
			Date1:transaction.Date1,
			Date2:transaction.Date2,
			DateAdded:transaction.DateAdded,
			Supplier:transaction.Supplier,
			Destination:transaction.Destination,
			Price:transaction.Price,
			Vat:transaction.Vat,
			Currency:transaction.Currency,
			ExtraInfo:transaction.ExtraInfo,
			Description:transaction.Description,
			action:transaction.action
		}
	}
}

module.exports = BillUtility;