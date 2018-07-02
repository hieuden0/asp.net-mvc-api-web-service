var Dispatcher =  require('../dispatcher/appDispatcher');
var ActionTypes =  require('../constants/actionTypes');
var EventEmitter = require('events').EventEmitter;
import assign from 'object-assign';

var _ =  require('lodash');
var CHANGE_EVENT = 'change';

var formatDate = function (date) {
	if (date) {
		var year = date.getFullYear();
		var month = date.getMonth() + 1;
		var date = date.getDate();
		month = month < 10 ? ('0' + month) : month;
		date = date < 10 ? ('0' + date) : date;
		return year + '-' + month + '-' + date;
	}
};

var date = new Date();
var dateData = formatDate(date);

var image = 'null';

var bill = {
	userTransactions: [],
	localTransactions: [],
	loginSuccess: false,
	currentTransaction: {
		id: '0',
		username: '',
		image: image,
		date: dateData,
		supplier: "",
		place: "",
		amount: "",
		vat: "",
		currency: "SEK",
		information: "",
		action: "new"
	},
	currentLanguageCode: 'sv',
};

var BillStore = assign({}, EventEmitter.prototype, {
	addChangeListener: function(callback) {
		this.on(CHANGE_EVENT, callback);
	},

	removeChangeListener: function(callback) {
		this.removeListener(CHANGE_EVENT, callback);
	},

	emitChange: function() {
		this.emit(CHANGE_EVENT);
	},

	getBillById: function(id) {
		return _.find(bill.localTransactions, {id: id});
	},

	getBillState: function() {
		return bill;
	},
});

Dispatcher.register(function(action) {
	switch(action.actionType) {
		case ActionTypes.LOGIN:
			bill.loginSuccess = action.isSuccess;
			bill.currentTransaction.username = action.username;
			BillStore.emitChange();
			break;
		case ActionTypes.USER_TRANSACTIONS:
			bill.userTransactions = action.userTransations;
			BillStore.emitChange();
			break;
		case ActionTypes.LOCAL_TRANSACTIONS:
			bill.localTransactions = action.localTransations;
			BillStore.emitChange();
			break;
		case ActionTypes.CURRENT_TRANSACTION:
			if(action.currentTransaction != null)
			{
				bill.currentTransaction = action.currentTransaction;
			}
			BillStore.emitChange();
			break;

		case ActionTypes.INSERT_TRANSACTION:
			bill.localTransactions.push(action.transaction);
			BillStore.emitChange();
			break;	
		case ActionTypes.UPDATE_TRANSACTION:
			var existingBill = _.find(bill.localTransactions, {id: action.transaction.id});
			var existingBillIndex = _.indexOf(bill.localTransactions, existingBill);
			bill.localTransactions.splice(existingBillIndex, 1, action.transaction);
			BillStore.emitChange();
			break;
		case ActionTypes.DELETE_TRANSACTION:
			_.remove(bill.localTransactions, function(bill){
				return action.transaction.id === bill.id;				
			});
			BillStore.emitChange();
			break;	
		case ActionTypes.LANGUAGE:
			bill.currentLanguageCode = action.currentLanguageCode;
			BillStore.emitChange();
			break;	
		default:
	}
});

module.exports = BillStore;