import React from 'react';
import Timers from 'react-timers';
import { Mixins } from 'touchstonejs';

var Dispatcher = require('../../dispatcher/appDispatcher');
var ActionTypes = require('../../constants/actionTypes');

var $ = require('jquery');

var BillActions = require('../../actions/billActions');
var BillSql = require('../../api/billSql');

module.exports = React.createClass({
	mixins: [Mixins.Transitions, Timers],
	statics: {
		navigationBar: 'main',
		getNavigation () {
			return {
				title: 'Loading Screen'
			}
		}
	},

	componentWillMount(){
	},

	componentDidMount () {
		var self = this;
		window.localStorage.setItem("addmoreinfo", "false");
		self.checkLastedVersion();




		// BillSql.deleteTransaction();
		// BillSql.deleteCurrentTransaction();
		// BillSql.dropTableSession();
		// BillSql.deleteHistory();
		BillSql.createTableCurrentSessionIfNotExists(function(){	
			BillSql.createTableCurrentTransactionIfNotExists(function(){	
				BillSql.createTableTransactionsIfNotExists(function(){	
					BillSql.createTableHistoryIfNotExists(function(){	
						var page = 'tabs:login';
						var language = 'sv';
						// select


						BillSql.selectTop1CurrentSession(function(queryOject, resultsOject){
							var currentSession = null;
							if(resultsOject.rows.length > 0)
							{
								currentSession = resultsOject.rows.item(0);
							}
							
							if(currentSession != null)
							{
								page = currentSession.page;

								language = currentSession.language;						

								Dispatcher.dispatch({
									actionType: ActionTypes.LANGUAGE,
									currentLanguageCode: language
								});

								// BillActions.getCurrentTransaction(currentSession.username);
							 	// BillActions.getLocalTransations(currentSession.username);

							 	if(page != 'tabs:login' && currentSession.username != '')
							 	{
							 		window.localStorage.setItem("username", currentSession.username);

							 		// Get current transaction
								 	var query = "Traveller = '" + currentSession.username + "'";
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
											query = "traveller = '" + currentSession.username + "'";
											// Get Local transactions
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

													self.transitionTo(page, { transition: 'show-from-bottom' });
												}, 
												function(error){});												
										}, 
										function(error){});
								}
								else
								{
									self.transitionTo(page, { transition: 'show-from-bottom' });
								}
							}
							else
							{
								currentSession = {
									username: '',
									page: 'tabs:login',
									language: language
								};

								//BillActions.saveCurrentSession(currentSession);
								// delete before insert
								BillSql.deleteCurrentSession(function(){
									// insert new record
									BillSql.insertCurrentSession(currentSession, function() {
										self.transitionTo(page, { transition: 'show-from-bottom' });
									}, function(error){});
								}, function(error){});		
							}

						}, function(error){});

					}, 
					function(queryOject, error){});
				}, 
				function(queryOject, error){});
			}, 
			function(queryOject, error){});
		}, 
		function(queryOject, error){});
	},

	checkLastedVersion(){
		//JSON.stringify(window.plugins.checkVersion);
		console.log('version');
	},

	render () {
		return (
			<div className="loading-background">
				<div className="loading-div">
					<img src="img/MRBILL_NAVY.png" className="loading-image" />
				</div>
			</div>
		);
	}
});

