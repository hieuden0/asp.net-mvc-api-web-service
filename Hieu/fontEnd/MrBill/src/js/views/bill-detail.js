import Container from 'react-container';
import dialogs from 'cordova-dialogs';
import React from 'react';
import Tappable from 'react-tappable';
import Timers from 'react-timers';
import { Link, UI, Mixins } from 'touchstonejs';

import BurgerMenu from 'react-burger-menu';
import classNames from 'classnames';

import MenuWrap from '../components/common/menu-wrap';
import TextboxLabel from '../components/common/textbox-label';
import TextArea from '../components/common/text-area';
import TextboxPopup from '../components/common/textbox-popup';
import MenuView from '../views/menu-view';
import LoadingPopup from '../components/common/loading-popup';
import AlertBar from '../components/common/alertbar';

// import ConfirmPopup from '../components/common/confirm';

var Dispatcher = require('../../dispatcher/appDispatcher');
var ActionTypes = require('../../constants/actionTypes');

var BillActions = require('../../actions/billActions');
var BillStore = require('../../stores/billStore');
var BillSql = require('../../api/billSql');

var BillData = require('../../api/billData');
var BillUti = require('../../actions/BillUtility');

var Language = require('../../data/language');

var $ = require('jquery');

var attachFastClick = require('fastclick');
attachFastClick(document.body);

const scrollable = Container.initScrollable();

var deviceType = (navigator.userAgent.match(/iPad/i))  == "iPad" ? "iPad" : (navigator.userAgent.match(/iPhone/i))  == "iPhone" ? "iPhone" : (navigator.userAgent.match(/Android/i)) == "Android" ? "Android" : (navigator.userAgent.match(/BlackBerry/i)) == "BlackBerry" ? "BlackBerry" : "null";


module.exports = React.createClass({

	mixins: [Mixins.Transitions],
	statics: {
		navigationBar: 'main',
		getNavigation () {
			return {
				title: 'Bill Detail'
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
			page: 'tabs:bill-detail',
			language: ''
		};

		BillActions.updateCurrentSession(currentSession);

		window.localStorage.setItem("addmoreinfo", "false");
	},
	
	render () {
		return (
			<BillDetail />
		);
	}
});


var BillDetail = React.createClass({
	mixins: [Mixins.Transitions, Timers],

	backStep ()
	{
		//this.resetBillDetailExtraInfo();

		var self = this;
		
		if(self.state.currentBill.action == BillData.actions.editFromUser){
			window.localStorage.setItem("isEdit",true);
			self.transitionTo(BillData.getPage(BillData.pages.billListDetail), { transition: 'show-from-right' });
		}
		else{
			self.transitionTo(BillData.getPage(BillData.pages.camera), { transition: 'show-from-right' });
		}
	},

	
	resetBillDetailExtraInfo () {
	    //window.localStorage.removeItem("currentBill");
	},

	getBillDetailExtraInfo () {
		var currentBill = BillActions._clone(this.state.currentBill);
		currentBill.date = BillUti.formatDate(this.state.date);

		return currentBill;
	},
	updateNewCurrentAndTransition(){
		var self = this;
		var loadingPopup = self.refs['LoadingPopup'];
		var newCurrent = BillUti.NewTransaction();
		newCurrent.Traveller =  window.localStorage.getItem("username");
		BillSql.updateRecordCurrentTransaction(newCurrent, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.CURRENT_TRANSACTION,
				currentTransaction: newCurrent
			});
			loadingPopup.hide();
			self.transitionTo('tabs:bill-list-detail', { transition: 'show-from-left' });
		}, function(error){});
	},
	localEditMode(){
		if(this.state.currentBill.action == BillData.actions.editFromUser) return false;
		return true;
	},
	showConfirmSave(){
		if(!this.localEditMode()){
		this.showPopupDetail("Confirm");
		}
		else{
			this.saveBillDetail();
		}
	},
	saveBillDetail () {
		var self = this;
		var loadingPopup = self.refs['LoadingPopup'];
		var alertBar = self.refs['AlertBar'];
// 
		if(
			this.state.currentBill.Supplier == '' || 
		  	this.state.currentBill.Destination == ''||
			this.state.currentBill.Price == '' ||
			this.state.currentBill.Currency == ''||
			(this.state.currentBill.action != BillData.actions.editFromUser&&this.state.currentBill.Reciept == 'null'))
		{
			var message = self.state.pageLanguage.warningEmptyField[this.state.currentLanguage];
			alertBar.show('warning', message);
			return;
		}

		loadingPopup.show();
		var username = window.localStorage.getItem("username");
		var query = "username = '" + username + "'";

		var currentBill = BillActions._clone(this.getBillDetailExtraInfo());
		currentBill.Date1 = BillUti.formatDate(self.state.date);
		currentBill.Traveller = username;
		currentBill = BillUti.convertTransaction(currentBill);
		// save transaction
		if(currentBill.action == 'new')
		{
			currentBill.Id = BillUti.getTime();
			// insert new record
			BillSql.insertRecordTransactions(currentBill, function() {
				self.updateNewCurrentAndTransition();
			}, function(error){});
		}
		else
		{
			query = "Id = '"+currentBill.Id+"'";
			BillSql.selectTopTransactions(1,query,function(queryOject,resultsOject){
				window.localStorage.setItem("isEdit",true);
				if(resultsOject.rows.length>0){
					BillSql.updateRecordTransactions(currentBill,function(){
							self.updateNewCurrentAndTransition();		
					},function(error){});
				}
				else{
					BillSql.insertRecordTransactions(currentBill,function(){
							self.updateNewCurrentAndTransition();
					},function(){});
				}
			},function(error){});
		}
	},

	addMoreInformation () {
		var self = this;

		var currentBill = BillActions._clone(this.getBillDetailExtraInfo());
		currentBill.Traveller = window.localStorage.getItem("username");
		BillSql.updateRecordCurrentTransaction(BillUti.convertTransaction(currentBill), function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.CURRENT_TRANSACTION,
				currentTransaction: currentBill
			});

			window.localStorage.setItem("addmoreinfo", "true");

			self.transitionTo('tabs:camera', { transition: 'show-from-left' });

		}, function(error){});	
	},


	changeValue (event) {
		var field = event.target.name;
		var value = event.target.value;
		if(field == 'Currency')
		{
			if(value.length > 3)
			{
				return;
			}
		}

		this.setState({dirty: true});

		this.state.currentBill[field] = value;
		return this.setState({currentBill: this.state.currentBill});
	},


	showDatePicker () {
		this.setState({datePicker: true});
	},

	handleDatePickerChange (d) {
		$('.Popup-dialog').hide();
		this.setState({datePicker: false, date: d});

		this.setTimeout(() => {
			this.setState({
				popupDetail: {
					visible: true,
					popupSupplier: true
				}
			});
			
			this.setFocusTextBox('Supplier');
		}, 500);
	},
	
  

  getInitialState() {
  	var billState = BillStore.getBillState();
  	var currentBill = BillActions._clone(billState.currentTransaction);
  	var currentBillPopup = BillActions._clone(billState.currentTransaction);

	var date = new Date();

	if(currentBill.action != 'new')
	{
		date = new Date(BillUti.convertDate(currentBill.Date1));
	}

    return {
      currentMenu: 'slide',
      side: 'right',
      //date: new Date(parseInt(dateArray[0]), month, parseInt(dateArray[2])),
      date: date,
      currentBill: currentBill,
      currentBillPopup: currentBillPopup,
  	
		popupDetail: {
			visible: false,
			popupSupplier: false,
			popupDestination: false,
			popupPrice: false,
			popupVat: false,
			popupCurrency: false,
			popupExtraInfo: false,
			popupConfirm:false
		},
		currentLanguage: billState.currentLanguageCode,
		pageLanguage: Language.getApplicationLanguages('transaction'),
		commonLanguage: Language.getApplicationLanguages('common'),
		menuLanguage: Language.getApplicationLanguages('menu')
    };
  },

	onChangeTextbox()
	{
	},

	changeValuePopup (event) {
		var field = event.target.name;
		var value = event.target.value;
		if(field == 'Currency')
		{
			if(value.length > 3)
			{
				return;
			}
		}

		this.setState({dirty: true});

		this.state.currentBillPopup[field] = value;
		return this.setState({currentBillPopup: this.state.currentBillPopup});
	},

	setFocusTextBox(field)
	{
		this.setTimeout(() => {
			var element = $('input.text-box-popup[name="' + field + '"]');

		    element.focus();

		    element.val(element.val());
		}, 200);
	},

	showPopupDetail (field) {
		if(field == 'Confirm')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupConfirm: true
				}
			});

			
		}
		else if(field == 'Supplier')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupSupplier: true
				}
			});

			this.setFocusTextBox(field);
		}
		else if(field == 'Destination')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupDestination: true
				}
			});

			this.setFocusTextBox(field);
		}
		else if(field == 'Price')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupPrice: true
				}
			});

			this.setFocusTextBox(field);
		}
		else if(field == 'Vat')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupVat: true
				}
			});

			this.setFocusTextBox(field);
		}
		else if(field == 'Currency')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupCurrency: true
				}
			});

			this.setFocusTextBox(field);
		}
		else if(field == 'ExtraInfo')
		{
			this.setState({
				popupDetail: {
					visible: true,
					popupExtraInfo: true
				}
			});

			this.setFocusTextBox(field);
		}
	},

	hidePopupDetail (field, action) {	
		if(deviceType != "iPad" && deviceType != "iPhone")
		{
			if(field == 'Price' || field == 'Vat')
			{
				$('input.input-focus-number').focus();
			}
			else
			{
 				$('input.input-focus').focus();
 			}
	 	}

		if(field == 'Supplier')
		{
			if(action == "Next")
			{	
				this.setState({
					popupDetail: {
						visible: true,
						popupSupplier: false
					}
				});

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});

				this.showPopupDetail("Destination");
			}
			else if(action == "OK")
			{	
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupSupplier: false
					}
				});

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});
			}
			else
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupSupplier: false
					}
				});

				var value = this.state.currentBill[field];

				this.setState({dirty: true});

				this.state.currentBillPopup[field] = value;
				this.setState({currentBillPopup: this.state.currentBillPopup});
			}
		}
		else if(field == 'Destination')
		{
			if(action == "Next")
			{
				this.setState({
					popupDetail: {
						visible: true,
						popupDestination: false
					}
				});	

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});

				this.showPopupDetail("Price");
			}
			else if(action == "OK")
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupDestination: false
					}
				});	

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});
			}
			else
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupDestination: false
					}
				});	

				var value = this.state.currentBill[field];

				this.setState({dirty: true});

				this.state.currentBillPopup[field] = value;
				this.setState({currentBillPopup: this.state.currentBillPopup});
			}				
		}
		else if(field == 'Confirm')
		{
			if(action == "OK")
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupPrice: false
					}
				});

				this.saveBillDetail();
			}
			else
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupPrice: false
					}
				});
			}	
		}
		else if(field == 'Price')
		{
			if(action == "Next")
			{
				this.setState({
					popupDetail: {
						visible: true,
						popupPrice: false
					}
				});

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});

				this.showPopupDetail("Vat");
			}
			else if(action == "OK")
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupPrice: false
					}
				});

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});
			}
			else
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupPrice: false
					}
				});

				var value = this.state.currentBill[field];

				this.setState({dirty: true});

				this.state.currentBillPopup[field] = value;
				this.setState({currentBillPopup: this.state.currentBillPopup});
			}	
		}
		else if(field == 'Vat')
		{
			if(action == "Next")
			{
				this.setState({
					popupDetail: {
						visible: true,
						popupVat: false
					}
				});

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});

				this.showPopupDetail("Currency");
			}
			else if(action == "OK")
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupVat: false
					}
				});

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});
			}
			else
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupVat: false
					}
				});

				var value = this.state.currentBill[field];

				this.setState({dirty: true});

				this.state.currentBillPopup[field] = value;
				this.setState({currentBillPopup: this.state.currentBillPopup});
			}	
		}
		else if(field == 'Currency')
		{			
			if(action == "Next")
			{
				this.setState({
					popupDetail: {
						visible: true,
						popupCurrency: false
					}
				});	

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});

				this.showPopupDetail("ExtraInfo");
			}
			else if(action == "OK")
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupCurrency: false
					}
				});	

				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});
			}
			else
			{
				$('.Popup-dialog').hide();
				this.setState({
					popupDetail: {
						visible: false,
						popupCurrency: false
					}
				});	

				var value = this.state.currentBill[field];

				this.setState({dirty: true});

				this.state.currentBillPopup[field] = value;
				this.setState({currentBillPopup: this.state.currentBillPopup});
			}				
		}
		else if(field == 'ExtraInfo')
		{	
			$('.Popup-dialog').hide();
			this.setState({
				popupDetail: {
					visible: false,
					popupExtraInfo: false
				}
			});	

			if(action == "OK")
			{
				var value = this.state.currentBillPopup[field];

				this.setState({dirty: true});

				this.state.currentBill[field] = value;
				this.setState({currentBill: this.state.currentBill});
			}
			else
			{
				var value = this.state.currentBill[field];

				this.setState({dirty: true});

				this.state.currentBillPopup[field] = value;
				this.setState({currentBillPopup: this.state.currentBillPopup});
			}
		}		
	},

	focusTextbox (event) {
		var field = event.target.name;
		this.showPopupDetail(field);
	},
	getPlug(){
		if(this.localEditMode())
		{
			return(<div className="bill-content-top-left-plus" onClick={this.addMoreInformation}>
								+
								</div>)
		}
	},
	getFooterLeft(){
		if(this.localEditMode())
		{
			return (
				<div className="bill-footer-left">
						<button className="bill-footer-button"  onClick={this.addMoreInformation}>
							{this.state.pageLanguage.addMore[this.state.currentLanguage]}
						</button>
					</div>
			);
		}		
	},

  render() {
    return (
      <div id="outer-container" style={ { height: '100%' } }>
        <MenuView parent={this} page={BillData.pages.billDetail}></MenuView>

        <div className="guide-background">
        <AlertBar ref="AlertBar" parent={this}></AlertBar>
		
				<div className="bill-header">
					<div className="bill-header-left">
						<button className="bill-header-left-button" onClick={this.backStep}>
							{!this.localEditMode()?this.state.pageLanguage.cancel[this.state.currentLanguage]:this.state.pageLanguage.back[this.state.currentLanguage]}
						</button>
					</div>
					<div className="bill-header-title">
						<div>{this.state.pageLanguage.header[this.state.currentLanguage]}</div>
					</div>
					<div className="bill-header-right">
					</div>
				</div>

				<div className="bill-content">
					<div className="bill-content-top">
						<div className="bill-content-top-left">
							<div className="bill-content-top-left-content-image">
								<img src={this.state.currentBill.action !=BillData.actions.editFromUser? this.state.currentBill.Reciept: ""} className={this.state.currentBill.Reciept != "null" ? "bill-content-top-left-image": "bill-content-top-left-image list-hidden"} />
								{this.getPlug()}
							</div>	
						</div>
						<div className="bill-content-top-right">
							<div className="div-content-input">
								<div>
									<label className="textbox-label" htmlFor="date">{this.state.pageLanguage.date[this.state.currentLanguage]}</label>
									<div className="date-picker-group">
										<Tappable onTap={this.showDatePicker}>
										<UI.LabelValue value={BillUti.formatDate(this.state.date)} placeholder="" />
										</Tappable>
									</div>	
								</div>
							</div>
							<div className="div-content-input">
								<TextboxLabel name="Supplier" placeholder="" type="text" value={this.state.currentBill.Supplier} label={this.state.pageLanguage.Supplier[this.state.currentLanguage]} onChange={this.onChangeTextbox} onClick={this.focusTextbox} />
							</div>
							<div className="div-content-input">
								<TextboxLabel name="Destination" placeholder="" type="text" value={this.state.currentBill.Destination} label={this.state.pageLanguage.place[this.state.currentLanguage]} onChange={this.onChangeTextbox} onClick={this.focusTextbox} />
							</div>
						</div>
					</div>

					<div className="bill-content-central">
						<div className="div-content-input bill-content-central-left">
							<TextboxLabel name="Price" placeholder="" type="number" value={this.state.currentBill.Price} label={this.state.pageLanguage.Price[this.state.currentLanguage]} onChange={this.onChangeTextbox} onClick={this.focusTextbox} />
						</div>
						<div className="div-content-input bill-content-central-middle">
							<TextboxLabel name="Vat" placeholder="" type="number" value={this.state.currentBill.Vat} label={this.state.pageLanguage.Vat[this.state.currentLanguage]} onChange={this.onChangeTextbox} onClick={this.focusTextbox} />
						</div>
						<div className="div-content-input bill-content-central-right">
							<TextboxLabel name="Currency" placeholder="" type="text" value={this.state.currentBill.Currency} label={this.state.pageLanguage.Currency[this.state.currentLanguage]} onChange={this.onChangeTextbox} onClick={this.focusTextbox} />
						</div>
					</div>

					<div className="bill-content-bottom">									
						<TextArea name="ExtraInfo" placeholder="" type="text" value={this.state.currentBill.ExtraInfo} label={this.state.pageLanguage.ExtraInfo[this.state.currentLanguage]} onChange={this.onChangeTextbox} onClick={this.focusTextbox} />
					</div>
				</div>
						
				<div className="bill-footer">
					{this.getFooterLeft()}
					<div className="bill-footer-right">
						<button className="bill-footer-button bill-footer-right-button" onClick={this.showConfirmSave}>
							{this.state.pageLanguage.save[this.state.currentLanguage]}
						</button>
					</div>
		
				</div>	

				<UI.DatePickerPopup visible={this.state.datePicker} date={this.state.date} onChange={this.handleDatePickerChange}/>
	      		<LoadingPopup ref="LoadingPopup" parent={this}></LoadingPopup>

				<div className="bill-detail-page">
					<UI.Popup visible={this.state.popupDetail.visible}>
						
						<div className={this.state.popupDetail.popupConfirm ? "" : "list-hidden"}>
							<div>{this.state.pageLanguage.confirmMessage[this.state.currentLanguage]}</div>
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Confirm", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Confirm", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
							</div>
						</div>
						
						<div className={this.state.popupDetail.popupSupplier ? "" : "list-hidden"}>
							<TextboxPopup name="Supplier" placeholder="" type="text" value={this.state.currentBillPopup.Supplier} label={this.state.pageLanguage.Supplier[this.state.currentLanguage]} onChange={this.changeValuePopup} />
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Supplier", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-center">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Supplier", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Supplier", "Next")}>{this.state.pageLanguage.next[this.state.currentLanguage]}</a>
								</div>
							</div>
						</div>

						<div className={this.state.popupDetail.popupDestination ? "" : "list-hidden"}>
							<TextboxPopup name="Destination" placeholder="" type="text" value={this.state.currentBillPopup.Destination} label={this.state.pageLanguage.place[this.state.currentLanguage]} onChange={this.changeValuePopup} />
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Destination", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-center">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Destination", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Destination", "Next")}>{this.state.pageLanguage.next[this.state.currentLanguage]}</a>
								</div>
							</div>
						</div>

						<div className={this.state.popupDetail.popupPrice ? "" : "list-hidden"}>
							<TextboxPopup name="Price" placeholder="" type="number" value={this.state.currentBillPopup.Price} label={this.state.pageLanguage.Price[this.state.currentLanguage]} onChange={this.changeValuePopup} />
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Price", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-center">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Price", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Price", "Next")}>{this.state.pageLanguage.next[this.state.currentLanguage]}</a>
								</div>
							</div>
						</div>

						<div className={this.state.popupDetail.popupVat ? "" : "list-hidden"}>
							<TextboxPopup name="Vat" placeholder="" type="number" value={this.state.currentBillPopup.Vat} label={this.state.pageLanguage.Vat[this.state.currentLanguage]} onChange={this.changeValuePopup} />
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Vat", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-center">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Vat", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Vat", "Next")}>{this.state.pageLanguage.next[this.state.currentLanguage]}</a>
								</div>
							</div>
						</div>

						<div className={this.state.popupDetail.popupCurrency ? "" : "list-hidden"}>
							<TextboxPopup name="Currency" placeholder="" type="text" value={this.state.currentBillPopup.Currency} label={this.state.pageLanguage.Currency[this.state.currentLanguage]} onChange={this.changeValuePopup} />
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Currency", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-center">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Currency", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "Currency", "Next")}>{this.state.pageLanguage.next[this.state.currentLanguage]}</a>
								</div>
							</div>
						</div>

						<div className={this.state.popupDetail.popupExtraInfo ? "" : "list-hidden"}>
							<TextboxPopup name="ExtraInfo" placeholder="" type="text" value={this.state.currentBillPopup.ExtraInfo} label={this.state.pageLanguage.ExtraInfo[this.state.currentLanguage]} onChange={this.changeValuePopup} />
							
							<hr className="dark-blue" />

							<div className="bill-detail-popup-footer">
								<div className="bill-detail-popup-footer-content">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "ExtraInfo", "Cancel")}>{this.state.pageLanguage.cancel[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-center">
									<a className="bill-detail-popup-footer-text" onClick={this.hidePopupDetail.bind(this, "ExtraInfo", "OK")}>{this.state.pageLanguage.save[this.state.currentLanguage]}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<span className="bill-detail-popup-footer-text-disabled">{this.state.pageLanguage.next[this.state.currentLanguage]}</span>
								</div>
							</div>
						</div>

						<div className="div-opacity">
							<input type="text" className="input-focus" />
							<input type="number" className="input-focus-number" />
						</div>

					</UI.Popup>
				</div>
			</div>			
      </div>
    );
  }
});