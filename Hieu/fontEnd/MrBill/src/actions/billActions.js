var Dispatcher = require('../dispatcher/appDispatcher');
var BillApi = require('../api/billApi');
var ActionTypes = require('../constants/actionTypes');
var BillSql = require('../api/billSql');
var BillData = require('../api/billData');
var BillActions = {
	_clone: function(item) {
		return JSON.parse(JSON.stringify(item));
	},

	login: function(email, password)
	{
		var user = BillApi.login(email, password);

		var isSuccess = false;
		if(user != null)
		{
			isSuccess = true;
		}
		
		Dispatcher.dispatch({
			actionType: ActionTypes.LOGIN,
			isSuccess: isSuccess,
			username: email
		});
	},

	getUserTransations: function(email)
	{
		var userTransations = BillApi.getUserTransations(email);

		Dispatcher.dispatch({
			actionType: ActionTypes.USER_TRANSACTIONS,
			userTransations: userTransations
		});
	},

	// CURRENT TRANSACTION
	saveCurrentTransaction: function(transaction)
	{
		transaction.id = '0';
		transaction.action = 'new';

		// delete before insert
		BillSql.deleteRecordCurrentTransaction(transaction, function(){
			// insert new record
			BillSql.insertRecordCurrentTransaction(transaction, function() {
				Dispatcher.dispatch({
					actionType: ActionTypes.CURRENT_TRANSACTION,
					currentTransaction: transaction
				});
			}, function(error){});
		}, function(error){});		
	},

	updateCurrentTransaction: function(transaction)
	{
		BillSql.updateRecordCurrentTransaction(transaction, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.CURRENT_TRANSACTION,
				currentTransaction: transaction
			});
		}, function(error){});			
	},

	getCurrentTransaction: function(userName)
	{
		var query = "username = '" + userName + "'";
		BillSql.selectTopCurrentTransaction(1, query,
			function(queryOject, resultsOject){
				var currentTransaction = null;
				if(resultsOject.rows.length > 0)
				{
					currentTransaction = resultsOject.rows.item(0);
				}				

				Dispatcher.dispatch({
					actionType: ActionTypes.CURRENT_TRANSACTION,
					currentTransaction: currentTransaction
				});
			}, 
			function(error){});
	},

	// TRANSACTIONS
	saveTransaction: function(transaction)
	{
		// insert new record
		BillSql.insertRecordTransactions(transaction, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.INSERT_TRANSACTION,
				transaction: transaction
			});
		}, function(error){});
	},

	updateTransaction: function(transaction)
	{
		BillSql.updateRecordTransactions(transaction, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.UPDATE_TRANSACTION,
				transaction: transaction
			});
		}, function(error){});			
	},

	getLocalTransations: function(username)
	{
		var query = "username = '" + username + "'";
		BillSql.selectAllTransactions(query,
			function(queryOject, resultsOject){
				var transactions = [];

				for (var i = 0; i < resultsOject.rows.length; i++){
				    transactions.push(resultsOject.rows.item(i));
				}

				Dispatcher.dispatch({
					actionType: ActionTypes.LOCAL_TRANSACTIONS,
					localTransations: transactions
				});
			}, 
			function(error){});
	},

	synchronizeUserTransaction: function(transaction)
	{
		if(transaction.action == BillData.actions.new ||transaction.action == BillData.actions.editFromLocal ){
			return BillApi.synchronizeAddNew(transaction);
		}
		return BillApi.synchronizeEdit(transaction);
	},

	saveCurrentSession: function(session)
	{
		// delete before insert
		BillSql.deleteCurrentSession(function(){
			// insert new record
			BillSql.insertCurrentSession(session, function() {
				
			}, function(error){});
		}, function(error){});		
	},

	updateCurrentSession: function(session)
	{
		BillSql.updateCurrentSession(session, function() {
			
		}, function(error){});			
	},

};

module.exports = BillActions;