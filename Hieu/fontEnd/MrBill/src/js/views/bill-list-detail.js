import Container from 'react-container';
import dialogs from 'cordova-dialogs';
import React from 'react';
import Tappable from 'react-tappable';
import Timers from 'react-timers';
import { Link, UI, Mixins } from 'touchstonejs';
import ReactDOM from 'react-dom';

import BurgerMenu from 'react-burger-menu';
import classNames from 'classnames';
import AlertBar from '../components/common/alertbar';
import LoadingPopup from '../components/common/loading-popup';

import MenuView from '../views/menu-view';

import MenuWrap from '../components/common/menu-wrap';
import ConfirmPopup from '../components/common/comfirmPopup';

const scrollable = Container.initScrollable();

var IScroll = require('iScroll/build/iscroll-probe');

var $ = require('jquery');

var _ = require('lodash');

var Dispatcher = require('../../dispatcher/appDispatcher');
var ActionTypes = require('../../constants/actionTypes');

var BillActions = require('../../actions/billActions');
var BillStore = require('../../stores/billStore');
var BillSql = require('../../api/billSql');
var BillApis = require('../../api/billApi');

var BillUti = require('../../actions/BillUtility');
var BillData = require('../../api/billData');

var Language = require('../../data/language');

var attachFastClick = require('fastclick');
attachFastClick(document.body);

var apiUrl = BillApis.getApiUrl();
//var apiUrl2 = BillApis.getApiUrl2();

var deviceType = (navigator.userAgent.match(/iPad/i))  == "iPad" ? "iPad" : (navigator.userAgent.match(/iPhone/i))  == "iPhone" ? "iPhone" : (navigator.userAgent.match(/Android/i)) == "Android" ? "Android" : (navigator.userAgent.match(/BlackBerry/i)) == "BlackBerry" ? "BlackBerry" : "null";

var MONTHS = Language.getMonths();

var myScroll,
	pullDownEl, pullDownOffset;

var scrollTop = 0;
var isEdit = "false";

var $stickies;
var current;

module.exports = React.createClass({
	mixins: [Mixins.Transitions],
	statics: {
		navigationBar: 'main',
		getNavigation () {
			return {
				title: 'Bill List Detail'
			}
		}
	},
	
	getInitialState () {		
		return {
				
		}
	},

	componentDidMount () {
		var currentSession = {
			username: '',
			page: 'tabs:bill-list-detail',
			language: ''
		};

		BillActions.updateCurrentSession(currentSession);

		window.localStorage.setItem("addmoreinfo", "false");
	},
	
	render () {
		return (
			<BillListDetail />
		);
	}
});

var BillListDetail = React.createClass({
	mixins: [Mixins.Transitions, Timers],

	backStep ()
	{
		var self = this;
		self.removeListViewStateLocalStorage();
		window.localStorage.setItem("scrollTop", scrollTop);
		self.transitionTo('tabs:bill-detail', { transition: 'show-from-right' });
	},

	 removeListViewStateLocalStorage(){
        window.localStorage.removeItem("isEdit");
        window.localStorage.removeItem("idExpand");
        window.localStorage.removeItem("scrollTop");
    },

	whenScrolling: function() {

			var top = scrollTop * -1;

	    	$stickies.each(function(i) {

		      	var $thisSticky = $(this);
		        var $stickyPosition = $thisSticky.data('originalPosition');

		      	if ($stickyPosition <= top) {

			        var $nextSticky = $stickies.eq(i + 1),
			          $nextStickyPosition = $nextSticky.data('originalPosition') - $thisSticky.data('originalHeight');

			        // $thisSticky.addClass("fixed");			

					$thisSticky.addClass("fixed").css({"top": (50 + top) + "px"});				

			        if ($nextSticky.length > 0 && ($thisSticky.offset().top * -1) >= $nextStickyPosition) {

			          	// $thisSticky.addClass("absolute").css("top", $nextStickyPosition);
			          	$thisSticky.addClass("absolute");
			        }

			    } else {

			        var $prevSticky = $stickies.eq(i - 1);

			        $thisSticky.removeClass("fixed");

			        if ($prevSticky.length > 0 && top <= $thisSticky.data('originalPosition') - $thisSticky.data('originalHeight')) {

			          	$prevSticky.removeClass("absolute");
			          	//$prevSticky.removeClass("absolute").removeAttr("style");
			        }
		      	}

	    	})
	  	},

	synchronize () {
		if(this.state.billState.localTransactions.length == 0)
		{
			return;
		}

		var self = this;
		var alertBar = self.refs['AlertBar'];
		this.showLoadingPopup();
		this.setTimeout(() => {
			var username = window.localStorage.getItem("username");		

			var query = "Traveller = '" + username + "'";
			BillSql.selectAllTransactions(query,
				function(queryOject, resultsOject){
					var index = 0;
					var total = resultsOject.rows.length;

					for (var i = 0; i < resultsOject.rows.length; i++){
					    var transaction = resultsOject.rows.item(i);

				    	var data = BillActions.synchronizeUserTransaction(transaction);

				    	if(data.isSuccess)
				    	{
				    		// delete local
				    		BillSql.deleteRecordTransactions(transaction, function() {
								Dispatcher.dispatch({
									actionType: ActionTypes.DELETE_TRANSACTION,
									transaction: transaction
								});

								index++;

								if(index == total)
								{
									self.reloadData(false);									
								}

							}, function(error){});	
				    	}
				    	else
				    	{
				    		alertBar.show('warning', data.message);

				    		index++;

							if(index == total)
							{
								self.reloadData(false);	
							}
				    	}
					}
				}, 
				function(error){});

				// reset current build
				var newTransaction = BillUti.NewTransaction();
				BillSql.updateRecordCurrentTransaction(newTransaction, function() {
					Dispatcher.dispatch({
						actionType: ActionTypes.CURRENT_TRANSACTION,
						currentTransaction: newTransaction
					});
				}, function(error){});	
			}, 1000);
	},

	formatDate (date) {
		if (date) {
			var year = date.getFullYear();
			var month = date.getMonth() + 1;
			var date = date.getDate();
			month = month < 10 ? ('0' + month) : month;
			date = date < 10 ? ('0' + date) : date;
			return year + '-' + month + '-' + date;
		}
	},

	showLoadingPopup () {
		this.setState({
			popup: {
				visible: true,
				loading: true,
				header: this.state.commonLanguage.popupLoading[this.state.currentLanguage],
				iconName: 'ion-load-c',
				iconType: 'default'
			}
		});
	},

	hideLoadingPopup () {
		$('.Popup-dialog').hide();
		this.setState({
			popup: {
				visible: false
			}
		});
	},

  

	getInitialState() {
		var billState = BillStore.getBillState();
		return {
		    currentMenu: 'slide',
		    side: 'right',
		    popup: {
				visible: false
			},
		    billState: billState,	
			currentLanguage: billState.currentLanguageCode,
			pageLanguage: Language.getApplicationLanguages('transactionList'),
			commonLanguage: Language.getApplicationLanguages('common'),
			menuLanguage: Language.getApplicationLanguages('menu'),
			currentTransactionGroup : [],
	    }	
	},

	componentWillMount() {
		
	},

	componentDidMount () {
		current = this;
		this.reloadData(true);
	},

	reloadData(isInit)
	{
		this.showLoadingPopup();
		var self = this;
		var alertBar = self.refs['AlertBar'];
		var username = window.localStorage.getItem("username");
		this.setTimeout(() => {
			var query = "Traveller = '" + username + "'";
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

					var url = apiUrl + "GetUserTrans";
					$.ajax({
						type : 'POST',
						url : url,
						timeout: 60000,
						//data : null,
						// contentType : "application/json;charset=utf-8",
						// dataType : 'json',
						contentType : "application/json",
						data: JSON.stringify({
							username:username
						}),
						async: true,
						success : function(data) {
							if(data)
							{
								var userTransations = data.GetUserTransResult;

								Dispatcher.dispatch({
									actionType: ActionTypes.USER_TRANSACTIONS,
									userTransations: userTransations
								});
							}

							self.hideLoadingPopup();
							self.showLoadingPopup();
						
							self.setTimeout(() => {
								if(isInit)
								{
									self.stickyHeaders.load($(".followMeBar"));	

									self.iscrollProbe.loadScroll();

									isEdit = window.localStorage.getItem("isEdit");
									if(isEdit=="true"){
										self.setTimeout(() => {
										scrollTop = parseInt(window.localStorage.getItem("scrollTop"));
										myScroll.scrollBy(0,scrollTop);
										scrollTop = 0;
										self.whenScrolling();
										window.localStorage.setItem("isEdit",false);
										myScroll.refresh();	
										});
									}	
								}
								else
								{
									self.setTimeout(() => {
										myScroll.scrollBy(0, (scrollTop * -1));
										scrollTop = 0;
										self.whenScrolling();

										$('#scroller').height('auto');

										var wrapperHeight = $('#wrapper').height();
								  		if(wrapperHeight >= ($('#scroller').height()))
								  		{
									  		$('#scroller').height(wrapperHeight + 1);
									  	}

										self.stickyHeadersReload.load($(".followMeBar"));	

										myScroll.refresh();	
									});
								}

								self.hideLoadingPopup();

							}, 100);
						},
						error : function(request, status) {	
							self.hideLoadingPopup();
							alertBar.show('warning', _error);
						},
					});
				}, 
				function(error){});

		}, 1000);
	},

	stickyHeadersReload: (function() {

	  	var load = function(stickies) {
		    if (typeof stickies === "object" && stickies instanceof $ && stickies.length > 0) {

		      	$stickies = stickies.each(function() {

		        	var $thisSticky = $(this);

		        	var top = 70; // header height

			        $thisSticky
			          .data('originalPosition', ($thisSticky.offset().top - top))
			          .data('originalHeight', $thisSticky.outerHeight())
			          .parent()
			          .height($thisSticky.outerHeight());
			    });
		    }
	  	};

	  	return {
	    	load: load
	  	}
	})(),

	stickyHeaders: (function() {

	  	var load = function(stickies) {
		    if (typeof stickies === "object" && stickies instanceof $ && stickies.length > 0) {

		      	$stickies = stickies.each(function() {

		        	var $thisSticky = $(this).wrap('<div class="followWrap" />');

		        	var top = 70; // header height

			        $thisSticky
			          .data('originalPosition', ($thisSticky.offset().top - top))
			          .data('originalHeight', $thisSticky.outerHeight())
			          .parent()
			          .height($thisSticky.outerHeight());
			    });
		    }
	  	};

	  	var _whenScrolling = function() {

	    	$stickies.each(function(i) {

		      	var $thisSticky = $(this);
		        var $stickyPosition = $thisSticky.data('originalPosition');

		      	if ($stickyPosition <= $('.bill-list-content-pull').scrollTop()) {

			        var $nextSticky = $stickies.eq(i + 1),
			          $nextStickyPosition = $nextSticky.data('originalPosition') - $thisSticky.data('originalHeight');

			        $thisSticky.addClass("fixed");

			        if ($nextSticky.length > 0 && $thisSticky.offset().top >= $nextStickyPosition) {

			          	// $thisSticky.addClass("absolute").css("top", $nextStickyPosition);
			          	$thisSticky.addClass("absolute");
			        }

			    } else {

			        var $prevSticky = $stickies.eq(i - 1);

			        $thisSticky.removeClass("fixed");

			        if ($prevSticky.length > 0 && $('.bill-list-content-pull').scrollTop() <= $thisSticky.data('originalPosition') - $thisSticky.data('originalHeight')) {

			          	$prevSticky.removeClass("absolute").removeAttr("style");
			        }
		      	}

	    	})
	  	};

	  	return {
	    	load: load
	  	}
	})(),

	iscrollProbe: (function() {
	  	var loadScroll = function() {
			scrollTop = 0;

	  		$('#scroller').height('auto');

			var wrapperHeight = $('#wrapper').height();
	  		if(wrapperHeight >= ($('#scroller').height()))
	  		{
		  		$('#scroller').height(wrapperHeight + 1);
		  	}

		    pullDownEl = document.getElementById('pullDown');
			pullDownOffset = pullDownEl.offsetHeight;

			var wrapper = document.getElementById('wrapper');

			myScroll = new IScroll(wrapper, {
				probeType: 3,
				mouseWheel: true,
	    		useTransition: false,
	    		//click: true,
    			tap: true,
    			preventDefaultException: { tagName: /^(INPUT|TEXTAREA|BUTTON|SELECT|DIV)$/ },
			});
		
			myScroll.on('scroll', scroll);
			myScroll.on('scrollEnd', scrollEnd);
	  	};

	  	var scroll = function() {
	  		scrollTop = this.y;

	    	if (this.y >= 70 && !pullDownEl.className.match('flip')) {
				pullDownEl.className = 'flip';
				pullDownEl.querySelector('.pullDownLabel').innerHTML = current.state.pageLanguage.reloadRelease[current.state.currentLanguage];
				//this.minScrollY = 70;

				$('#wrapper').css({'top' : '70px'});
			} else if (this.y < 70 && pullDownEl.className.match('flip')) {
				// pullDownEl.className = '';
				// pullDownEl.querySelector('.pullDownLabel').innerHTML = 'Pull down to refresh...';
				//this.minScrollY = -pullDownOffset;
			}
			else if (this.y < 70 && this.y >= 20 && pullDownEl.className.match('')) {
				// var top = this.y + 'px';
				// if(this.y < 25)
				// {
				// 	var top = '20px';
				// }
				// $('#wrapper').css({'top' : top});
				$('#wrapper').css({'top' : '20px'});
			} 
			
			_whenScrolling();
	  	};

	  	var scrollEnd = function() {
	  		scrollTop = this.y;

	    	if (pullDownEl.className.match('flip')) {
	    		$('#wrapper').css({'top' : '70px'});

				pullDownEl.className = 'loading';
				pullDownEl.querySelector('.pullDownLabel').innerHTML = current.state.pageLanguage.reloadLoading[current.state.currentLanguage];				
				pullDownAction();	// Execute custom function (ajax call?)
			}
	  	};

	  	var pullDownAction = function() {
			var username = window.localStorage.getItem("username");
	  		var query = "Traveller = '" +username + "'";
			var alertBar = current.refs['AlertBar'];  
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

					var url = apiUrl + "GetUserTrans";
					$.ajax({
						type : 'POST',
						url : url,
						timeout: 60000,
						//data : null,
						// contentType : "application/json;charset=utf-8",
						// dataType : 'json',
						contentType : "application/json",
						data: JSON.stringify({
							username: username
						}),
						async: true,
						success : function(data) {
							if(data)
							{
								var userTransations = data.GetUserTransResult;

								Dispatcher.dispatch({
									actionType: ActionTypes.USER_TRANSACTIONS,
									userTransations: userTransations
								});
							}

							current.showLoadingPopup();

							current.setTimeout(() => {
								pullDownEl.className = '';
								pullDownEl.querySelector('.pullDownLabel').innerHTML = current.state.pageLanguage.reloadPulldown[current.state.currentLanguage];
								//this.minScrollY = -pullDownOffset;
								$('#wrapper').css({'top' : '20px'});

								myScroll.scrollBy(0, (scrollTop * -1));
								scrollTop = 0;
								current.whenScrolling();

								$('#scroller').height('auto');

								var wrapperHeight = $('#wrapper').height();
						  		if(wrapperHeight >= ($('#scroller').height()))
						  		{
							  		$('#scroller').height(wrapperHeight + 1);
							  	}

							 	myScroll.refresh();		// Remember to refresh when contents are loaded (ie: on ajax completion)
		
								current.stickyHeadersReload.load($(".followMeBar"));
								current.hideLoadingPopup();
							}, 100);
						},
						error : function(request, status,_error) {	
							alertBar.show('warning', _error);
						},
					});
				}, 
				function(error){});
		};

		var _whenScrolling = function() {

			var top = scrollTop * -1;

	    	$stickies.each(function(i) {

		      	var $thisSticky = $(this);
		        var $stickyPosition = $thisSticky.data('originalPosition');

		      	if ($stickyPosition <= top) {

			        var $nextSticky = $stickies.eq(i + 1),
			          $nextStickyPosition = $nextSticky.data('originalPosition') - $thisSticky.data('originalHeight');

			        // $thisSticky.addClass("fixed");			

					$thisSticky.addClass("fixed").css({"top": (50 + top) + "px"});				

			        if ($nextSticky.length > 0 && ($thisSticky.offset().top * -1) >= $nextStickyPosition) {

			          	// $thisSticky.addClass("absolute").css("top", $nextStickyPosition);
			          	$thisSticky.addClass("absolute");
			        }

			    } else {

			        var $prevSticky = $stickies.eq(i - 1);

			        $thisSticky.removeClass("fixed");

			        if ($prevSticky.length > 0 && top <= $thisSticky.data('originalPosition') - $thisSticky.data('originalHeight')) {

			          	$prevSticky.removeClass("absolute");
			          	//$prevSticky.removeClass("absolute").removeAttr("style");
			        }
		      	}

	    	})
	  	};

	  	return {
	    	loadScroll: loadScroll
	  	}
	})(),

	openConfirm(transactionGroup){
		var self = this;
	 	if(this.state.billState.localTransactions.length > 0)
		{

			 var syncPopup = this.refs['SyncPopup'];	
			syncPopup.show();
		}else{
			var confirmPopup = this.refs['ConfirmPopup'];
			self.setState({currentTransactionGroup : transactionGroup});
			confirmPopup.show();
		}	
	},

	sendEmailReport(transactionMonthList){
		var self = this;
		//console.log(JSON.stringify(transactionMonthList));
		var loadingPopup = self.refs['LoadingPopup'];
		var alertBar = self.refs['AlertBar'];
		loadingPopup.show();

		self.setTimeout(() => {		
		var url = apiUrl + 'SendEmailReport'
		//var url = "http://localhost:1906/MrBillMobileServices.svc/" + 'SendEmailReport';
		//var url = "http://10.1.0.164/services/MrBillMobileServices.svc/" + 'SendEmailReport';
		//var url = "http://mrbill.mrorange.nu/services/MrBillMobileServices.svc/" + 'SendEmailReport';
		$.ajax({
			type : 'POST',
			url : url,
			timeout: 60000,
			contentType : "application/json",
			data: JSON.stringify({
				username : window.localStorage.getItem("username"),
				//emailList : ["hieuden0@gmail.com"],
				//emailList : ["bchieu@tma.com.vn"],
				emailList : [window.localStorage.getItem("username")],
				month : transactionMonthList[0].month,
				year : transactionMonthList[0].year,
				languageCode :  self.state.currentLanguage
			}),
			async: true,
			success : function(data) {
				if(data.SendEmailReportResult)
				{					
					loadingPopup.hide();
					var message = 'Email report has been sent successfully';
					alertBar.show('success', message);					
				}
				else
				{
					loadingPopup.hide();
					var message = 'Send email report has failed';
					alertBar.show('warning', message);
				}
				
			},
			error : function(request, status, _error) {	
				loadingPopup.hide();
				alertBar.show('warning', _error);
			},
		});

	}, 1000);
	},

  render() {
  		var userTransactions = this.state.billState.userTransactions;
  		var localTransactions = this.state.billState.localTransactions;
		var self = this;

  		var results = [];

		if ((userTransactions && userTransactions.length != 0) || (localTransactions && localTransactions.length != 0)) {		
			var notSynchronize =[];

			if(localTransactions && localTransactions.length > 0)
			{
				$('#wrapper').css({'bottom': '70px'});

				notSynchronize = BillActions._clone(localTransactions);

				for (var i = notSynchronize.length - 1; i >= 0; i--) {
					notSynchronize[i].isLocal = true;
				};
			}
			else
			{
				$('#wrapper').css({'bottom': '0px'})
			}

			var synchronize = [];
			var arrayMonthYears = [];

			if(userTransactions && userTransactions.length > 0)
			{
				//synchronize = BillActions._clone(userTransactions);	
				
				// for (var i = 0; i < userTransactions.length; i++) {
				// 	if (userTransactions[i].IsDisabled == null) {
				// 		synchronize.push(BillActions._clone(userTransactions[i]));
				// 	};
				// };		

				synchronize = _.filter(userTransactions, { 'IsDisabled': null });
				for(var i =0;i<localTransactions.length;i++){
					// var index = _.indexOf(synchronize,{'Id':localTransactions[i].Id});
					synchronize = _.remove(synchronize,function(n){
						return n.Id != localTransactions[i].Id;
					});
				}
				synchronize = _.differenceBy(synchronize,localTransactions,'Id');
			}

			var mergeArray = _.concat(notSynchronize, synchronize);

			var mergeArrayResult = [];

			if(mergeArray && mergeArray.length > 0)
			{			
				for (var i = 0; i < mergeArray.length; i++) {
					var date;

					if(mergeArray[i].isLocal)
					{
						date = BillUti.convertDate(mergeArray[i].Date1);
					}
					else
					{
						date = BillUti.convertDate(mergeArray[i].Date1);
						mergeArray[i].isLocal = false;
					}

					var month = date.getMonth() + 1;
					var year = date.getFullYear();
					mergeArray[i].year = year;
					mergeArray[i].month = month;
					mergeArray[i].day = date.getDate();
					mergeArray[i].dateData = date;

					arrayMonthYears.push({
						month: month,
						year: year,
					});
				};

				// order all items by date
				//var newMergeArray = _.sortBy(mergeArray, ['year', 'month', 'day']).reverse();;

				// get unique month/year
				var uniqueArray = _.uniqWith(arrayMonthYears, _.isEqual);			
				//var loopArray = _.sortBy(uniqueArray, ['year', 'month']).reverse();
				var loopArray = _.sortBy(uniqueArray, ['year', 'month']);

				// for each month/year (group) => add item to group
				for (var i = 0; i < loopArray.length; i++) {
					var month = loopArray[i].month;
					var year = loopArray[i].year;

					var transactionsInMonthAndYear = _.filter(mergeArray, { 'year': year, 'month': month });
					transactionsInMonthAndYear = _.sortBy(transactionsInMonthAndYear,['Date1']);		
					mergeArrayResult.push(<GroupTransactions key={'groupTransaction' + i} openConfirm = {self.openConfirm.bind(this,transactionsInMonthAndYear)} monthYear={loopArray[i]} bills={transactionsInMonthAndYear} self={self} />);
				};
			}

			results = (
				<div>
					{mergeArrayResult}
				</div>
		
			);
			
		} else {
			results = (
				<Container direction="column" align="center" justify="center" className="no-results">
					<div className="no-results__text">{this.state.pageLanguage.noTransaction[this.state.currentLanguage]}</div>
				</Container>
			)
		}

    return (
      <div id="outer-container" style={ { height: '100%' } }>
        <MenuView parent={this} page={BillData.pages.billListDetail}></MenuView>       
        <div className="bill-list-detail-background">
       <AlertBar ref="AlertBar" parent={this}></AlertBar>
				<div className="bill-header">
					<div className="bill-header-left">
						<button className="bill-header-left-button" onClick={this.backStep}>
							{this.state.pageLanguage.back[this.state.currentLanguage]}
						</button>
					</div>
					<div className="bill-list-header-title">
						<div>{this.state.pageLanguage.header[this.state.currentLanguage]}</div>
					</div>
					<div className="bill-header-right">
					</div>
				</div>

				<div id="wrapper" className="">
					<div id="scroller" className="bill-list-content-pull">
						<div id="pullDown">
							<span className="pullDownIcon"></span><span className="pullDownLabel">{this.state.pageLanguage.reloadPulldown[this.state.currentLanguage]}</span>
						</div>
						{results}	
					</div>
				</div>
														
				<div className={this.state.billState.localTransactions.length > 0 ? "bill-list-footer" : "list-hidden"}>
					<button onClick={this.synchronize} className={this.state.billState.localTransactions.length > 0 ? "bill-list-footer-button" : "bill-list-footer-button-disabled"}>
					  <img className="bill-list-footer-sync-image" src="img/sync.png" />

					  {this.state.pageLanguage.synchronize[this.state.currentLanguage]}
					</button>						
				</div>	
			</div>

			<UI.Popup visible={this.state.popup.visible}>
				<UI.PopupIcon name={this.state.popup.iconName} type={this.state.popup.iconType} spinning={this.state.popup.loading} />		
				<div><strong>{this.state.popup.header}</strong></div>
			</UI.Popup>
			<LoadingPopup ref="LoadingPopup" parent={this}></LoadingPopup>
			<ConfirmPopup ref="ConfirmPopup" parent={this}  confirm = "false" titleText = {this.state.pageLanguage.syncTransaction[this.state.currentLanguage]} cancelText = {this.state.pageLanguage.noButton[this.state.currentLanguage]} saveText = {this.state.pageLanguage.yesButton[this.state.currentLanguage]}  onClick={this.sendEmailReport.bind(this,this.state.currentTransactionGroup)}></ConfirmPopup>	
			<ConfirmPopup ref="SyncPopup" parent={this}  confirm = "false" titleText = {this.state.pageLanguage.notSyncTransaction[this.state.currentLanguage]} cancelText = {this.state.pageLanguage.noButton[this.state.currentLanguage]}  saveText = {this.state.pageLanguage.yesButton[this.state.currentLanguage]}  onClick={this.synchronize}></ConfirmPopup>	
      </div>
    );
  }
});

var GroupTransactions = React.createClass({
	mixins: [Timers],

	stickyHeaders: (function() {
	  	var $window = $(window);
	   	var $stickies;

	  	var load = function(stickies, self) {
		    if (typeof stickies === "object" && stickies instanceof $ && stickies.length > 0) {
		      	$stickies = stickies.each(function() {

		        	var $thisSticky = $(this);

		        	var top = 70; // header height

			        $thisSticky
			          .data('originalPosition', ($thisSticky.offset().top - top + (scrollTop * -1)))
			          .data('originalHeight', $thisSticky.outerHeight())
			          .parent()
			          .height($thisSticky.outerHeight());
			    });
		    }
	  	};

	  	return {
	    	load: load
	  	}
	})(),


	getInitialState() {
		var bills = this.props.bills;
		return {
		    expand: (this.checkExpand(bills)?true:false),
	    }	
	  },

  	changeState(bills)
  	{
		var newState = {};
		newState['expand'] = !this.state['expand'];

		//save localStage() when expandGroup
		var idExpand = bills[0].Id;	
		var	 oldIdExpand = window.localStorage.getItem("idExpand");
		var expand = [];
		if (oldIdExpand!=null && oldIdExpand != "") {
				expand = oldIdExpand.split(",");
			};
		if(newState['expand']==true){
			if(oldIdExpand!=null && oldIdExpand != ""){
				window.localStorage.setItem("idExpand",oldIdExpand + ","+idExpand);
			}else{
				window.localStorage.setItem("idExpand",idExpand);
			}			
			
		}else{
				for (var i = expand.length; i >= 0; i--) {
					if (expand[i]==idExpand) {
						expand.splice(i,1);
					};
				};
				window.localStorage.setItem("idExpand",expand.toString());			
		}


		this.setState(newState);
		var self = this.props.self;

		//self.showLoadingPopup();

		var currentElement = ReactDOM.findDOMNode(this);

		var elements = $(currentElement).nextAll().children().children('.followMeBar');

		this.setTimeout(() => {
			$('#scroller').height('auto');

			var wrapperHeight = $('#wrapper').height();
	  		if(wrapperHeight >= ($('#scroller').height()))
	  		{
		  		$('#scroller').height(wrapperHeight + 1);
		  	}

			this.stickyHeaders.load($(elements), self);

			myScroll.refresh();	

			//self.hideLoadingPopup();
		});
  	},

  	checkExpand (result)
	{
		var self = this;
		isEdit = window.localStorage.getItem("isEdit");
		if(isEdit=="true"){
			var	oldIdExpand = window.localStorage.getItem("idExpand");
			var expand = [];
			if (oldIdExpand!=null && oldIdExpand != "") {
				expand = oldIdExpand.split(",");
			};
			

			for (var i = 0; i < result.length; i++) {
				for (var j = 0; j < expand.length; j++) {
					if(result[i].Id==expand[j]){
					return true;
					}
				};							
			};
			return false;
		}	
		return false;	
	},

	// openConfirm(){
	// 	var self = this;
	// 	 var confirmPopup = this.refs['ConfirmPopup'];


	// 	 confirmPopup.show();
	// },

	// sendEmailReport(transactionMonthList){
	// 	var self = this;
	// 	console.log(JSON.stringify(transactionMonthList));
	// },

	render () {
		var bills = this.props.bills;
		var self = this.props.self;

		var monthYear = this.props.monthYear;
		var result = [];

		var localTransactions = _.filter(bills, { 'isLocal': true });
		var userTransactions = _.filter(bills, { 'isLocal': false });

		var notSynchronize = localTransactions
			.map((bill, i) => {
				return <BillInfoLocalTransactions key={'notSynchronize' + i} bill={bill} self={self} />
		});

		var synchronize = userTransactions
			.map((bill, i) => {
				return <BillInfoUserTransactions key={'synchronize' + i} bill={bill} self={self} />
			});

		result.push(notSynchronize);

		result.push(synchronize);

		return (
				<div className="group-transaction">
					<div className="group-transaction-header followMeBar">						
						<div className="group-transaction-header-text" onClick={this.changeState.bind(this,bills)}>
							{MONTHS[monthYear.month - 1][self.state.currentLanguage]} &nbsp; {monthYear.year}
						</div>
						<div className="group-transaction-header-printer" onClick={this.props.openConfirm.bind(null,this)} onTouch={this.props.openConfirm.bind(null,this)}>
						</div>

						<div className="group-transaction-header-action-text" onClick={this.changeState.bind(this,bills)}>
							<div className = "group-transaction-header-action-text-align">	{bills.length}</div>
						</div>
						<div className={(deviceType != "iPad" && deviceType != "iPhone") ? "group-transaction-header-action" : "group-transaction-header-action-ios"} onClick={this.changeState.bind(this,bills)}>
							{this.state.expand ? "-" : "+"}
						</div>
					</div>
					<div className={this.state.expand ? "group-transaction-content" : "group-transaction-content-hide" }>
						{result}
					</div>					
				</div>
		);
	}
});


var BillInfoUserTransactions = React.createClass({
	
	editTransaction(bill)
	{
		current.showLoadingPopup();
		var self = this.props.self;
		bill.action = BillData.actions.editFromUser;
		bill.Traveller = self.state.billState.currentTransaction.Traveller;
		var transaction = BillUti.convertTransaction(bill);
		this.saveEditScrollTop(bill.id);
		BillSql.updateRecordCurrentTransaction(transaction, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.CURRENT_TRANSACTION,
				currentTransaction: transaction
			});
			current.hideLoadingPopup();

			self.transitionTo('tabs:bill-detail', { transition: 'show-from-right' });

		}, function(error){});
	},

	saveEditScrollTop (id)
	{
		var self = this;
	
		//var elmnt = document.getElementById("scroller");
	  	window.localStorage.setItem("scrollTop", scrollTop);
	},
	
	render () {
		var bill = this.props.bill;

		var dateData = BillUti.formatDate(bill.dateData);

		var image = 'img/MRBILL.png';
		if(bill.Supplier==null) {}
		else if(bill.Supplier.toLowerCase().indexOf('sas') >= 0)
		{
			image = 'img/SAS.png';
		}
		else if(bill.Supplier.toLowerCase().indexOf('easypark') >= 0)
		{
			image = 'img/EASYPARK.png';
		}
		else if(bill.Supplier.toLowerCase().indexOf('hotels') >= 0)
		{
			image = 'img/HOTELS.png';
		}
		else if(bill.Supplier.toLowerCase().indexOf('cabonline') >= 0)
		{
			image = 'img/CABONLINE.png';
		}

		return (
				<div className="bill-info" onClick={this.editTransaction.bind(this, this.props.bill)}>
					<div className="bill-info-left">
						<div className="bill-info-content-image">
							<img src={image} className='bill-info-image' />
						</div>
					</div>

					<div className="bill-info-text">
						<div className="bill-info-title">{bill.Supplier},</div>
						<div className="bill-info-title bill-info-title-middle">{bill.Destination}</div>
						<div className="bill-info-subtitle">{dateData || ''}</div>
					</div>

					<div className="bill-info-right">
						<div className="bill-info-amount">{bill.Currency} {bill.Price}</div>
						<div className="bill-info-content-button">
							<img src="img/transaction-check.png" className="bill-info-button-check" />
						</div>
					</div>
				</div>
		);
	}
});


var BillInfoLocalTransactions = React.createClass({
	editTransaction(bill)
	{
		current.showLoadingPopup();
		var self = this.props.self;
		if(bill.action==null||bill.action==undefined||bill.action ==  BillData.actions.new){
		bill.action = BillData.actions.editFromLocal;
		}
		bill.Traveller = self.state.billState.currentTransaction.Traveller;
		var transaction = BillUti.convertTransaction(bill);
		this.saveEditScrollTop(bill.id);
		BillSql.updateRecordCurrentTransaction(transaction, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.CURRENT_TRANSACTION,
				currentTransaction: transaction
			});
			current.hideLoadingPopup();

			self.transitionTo('tabs:bill-detail', { transition: 'show-from-right' });

		}, function(error){});

		// get transaction by id
		// BillSql.selectTopTransactions(1, query,
		// 	function(queryOject, resultsOject){
		// 		var currentTransaction = null;
		// 		if(resultsOject.rows.length > 0)
		// 		{
		// 			currentTransaction = resultsOject.rows.item(0);

		// 			currentTransaction.action = 'edit';

		// 			BillSql.updateRecordCurrentTransaction(currentTransaction, function() {
		// 				Dispatcher.dispatch({
		// 					actionType: ActionTypes.CURRENT_TRANSACTION,
		// 					currentTransaction: currentTransaction
		// 				});

		// 				current.hideLoadingPopup();

		// 				self.transitionTo('tabs:bill-detail', { transition: 'show-from-right' });

		// 			}, function(error){});
		// 		}				
		// 	}, 
		// 	function(error){});
	},

	saveEditScrollTop (id)
	{
		var self = this;
	
		//var elmnt = document.getElementById("scroller");
	  	window.localStorage.setItem("scrollTop", scrollTop);
	},

	render () {
		var bill = this.props.bill;
		var image = 'img/MRBILL.png';
		if(bill.Supplier==null) {}
		else if(bill.Supplier.toLowerCase().indexOf('sas') >= 0)
		{
			image = 'img/SAS.png';
		}
		else if(bill.Supplier.toLowerCase().indexOf('easypark') >= 0)
		{
			image = 'img/EASYPARK.png';
		}
		else if(bill.Supplier.toLowerCase().indexOf('hotels') >= 0)
		{
			image = 'img/HOTELS.png';
		}
		else if(bill.Supplier.toLowerCase().indexOf('cabonline') >= 0)
		{
			image = 'img/CABONLINE.png';
		}
		
		var dateData = BillUti.formatDate(bill.dateData);
		return (
				<div className="bill-info" onClick={this.editTransaction.bind(this, bill)}>
					<div className="bill-info-left">
						<div className="bill-info-content-image">
							<img src={image} className='bill-info-image' />
						</div>
					</div>

					<div className="bill-info-text">
						<div className="bill-info-title">{bill.Supplier},</div>
						<div className="bill-info-title bill-info-title-middle">{bill.Destination}</div>
						<div className="bill-info-subtitle">{dateData}</div>
					</div>

					<div className="bill-info-right">
						<div className="bill-info-amount">{bill.Currency} {bill.Price}</div>
						<div className="bill-info-content-button">
							<img src="img/sync.png" className="bill-info-button-sync" />
						</div>
					</div>
				</div>
		);
	}
});