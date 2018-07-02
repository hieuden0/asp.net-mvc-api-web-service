import Container from 'react-container';
import React from 'react';
import Timers from 'react-timers';
import { Link, UI, Mixins } from 'touchstonejs';

import Textbox from '../components/common/textbox';

import LoadingPopup from '../components/common/loading-popup';
import AlertBar from '../components/common/alertbar';
const scrollable = Container.initScrollable();

var $ = require('jquery');

var Dispatcher = require('../../dispatcher/appDispatcher');
var ActionTypes = require('../../constants/actionTypes');

var BillActions = require('../../actions/billActions');
var BillStore = require('../../stores/billStore');
var BillSql = require('../../api/billSql');
var BillApis = require('../../api/billApi');

var BillUti = require('../../actions/BillUtility');

var Language = require('../../data/language');

var attachFastClick = require('fastclick');
attachFastClick(document.body);

var apiUrl = BillApis.getApiUrl();

var LANGUAGE_CULTURE = Language.getLanguageCulture();

module.exports = React.createClass({
    mixins: [Mixins.Transitions, Timers],
    statics: {
        navigationBar: 'main',
        getNavigation () {
			return {
    			title: 'Login'
			}
		}
},
	
getInitialState () {
	var billState = BillStore.getBillState();

    return {
        model: {
            username: "",
            firstName: "",
            lastName: "",
            password: "",
            confirmPassword: "",
            Address: "",
            City : "",
            CompanyId : 1,
            Country : "SE",
            Phone : "(00) 000-000-000",
            PostCode : "",
            Status : 1,
            UserId : 0,
            UserRoleId : 3

        },
		billState: billState,
		currentLanguage: billState.currentLanguageCode,
		pageLanguage: Language.getApplicationLanguages('createUser')
    }
},

handleLabelSelectChange (key, event) {
	let newState = {};
	newState[key] = event.target.value;

	this.setState(newState);
},

backStep() {
	var self = this;
	self.transitionTo('tabs:login', { transition: 'show-from-right' });
},

componentDidMount () {

},

createUser: function () {
	var self = this;
	var loadingPopup = self.refs['LoadingPopup'];
	var alertBar = self.refs['AlertBar'];
	if(self.state.model.username == "" || self.state.model.password == "" || self.state.model.confirmPassword == "")
	{		
		var message = self.state.pageLanguage.warningLoginBlank[self.state.currentLanguage];
		alertBar.show('warning', message);
		return;
	}
	if(!validateEmail(self.state.model.username)){
		var message = self.state.pageLanguage.wrongEmail[self.state.currentLanguage];
		alertBar.show('warning', message);
		return;
	}else if(self.state.model.password != self.state.model.confirmPassword){
		var message = self.state.pageLanguage.passNotMath[self.state.currentLanguage];
		alertBar.show('warning', message);
		return;
	}
	

	loadingPopup.show();
	

	self.setTimeout(() => {		

		//var url = 'http://localhost:1906/MrBillMobileServices.svc/RegisterUser';
		var url = apiUrl + "RegisterUser";
		//var url = 'http://120.72.81.104/services/MrBillMobileServices.svc/RegisterUser'
		var  modelA = {
			UserName: self.state.model.username,
            FirstName: self.state.model.firstName,
            LastName: self.state.model.lastName,
            Password: self.state.model.password,          
            Address: "",
            City : "",
            CompanyId : 1,
            Country : "SE",
            Phone : "(00) 000-000-000",
            PostCode : "",
            Status : 1,
            UserId : 0,
            UserRoleId : 3
		}
		
		$.ajax({
			type : 'POST',
			url : url,
			timeout: 60000,
			contentType : "application/json",
			data: JSON.stringify({
				model: modelA,
			}),
			async: true,
			success : function(data) {
				if(data.RegisterUserResult.Success)
				{					
					Dispatcher.dispatch({
						actionType: ActionTypes.LOGIN,
						isSuccess: true,
						username: self.state.model.username
					});

					window.localStorage.setItem("username", self.state.model.username);

					var currentSession = {
						username: self.state.model.UserName,
						page: 'tabs:login',
						language: self.state.currentLanguage
					};

					//BillActions.updateCurrentSession(currentSession);
					BillSql.updateCurrentSession(currentSession, function() {
						Dispatcher.dispatch({
							actionType: ActionTypes.LANGUAGE,
							currentLanguageCode: self.state.currentLanguage
						});

						var query = "Traveller ='"	+ self.state.model.username + "'";
						
						BillSql.selectTopCurrentTransaction(1,query,function(queryOject,resultsOject){
							var currentTransaction = BillUti.NewTransaction();
							currentTransaction.Id = BillUti.getTime();
							currentTransaction.Traveller = self.state.model.username;
							BillSql.insertRecordCurrentTransaction(currentTransaction, function() {
								self.transitionTo('tabs:guide-step-1', { transition: 'show-from-left' });
								
							},function(error){});						
						},function(error){});				
					}, function(error){});	
					// var currentTransaction = BillUti.NewTransaction();
					// currentTransaction.Id = BillUti.getTime();
					// currentTransaction.Traveller = self.state.model.username;
					// BillSql.insertRecordCurrentTransaction(currentTransaction, function() {
					// 	self.transitionTo('tabs:guide-step-1', { transition: 'show-from-left' });						
					// },function(error){});

				}
				else
				{
					loadingPopup.hide();
					var message = data.RegisterUserResult.Message;
					alertBar.show('warning', message);
				}
				
			},
			error : function(request, status, _error) {	
				loadingPopup.hide();
				alertBar.show('warning', _error);
			},
		});

	}, 1000);

	function validateEmail(email) {
	    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	    return re.test(email);
	}
},

setUserState (event) {
    this.setState({dirty: true});
    var field = event.target.name;
    var value = event.target.value;
    this.state.model[field] = value;
    return this.setState({model: this.state.model});
},

render () {
    return (
		<div className="guide-background">
			<Container scrollable={scrollable}>
				<AlertBar ref="AlertBar" parent={this}></AlertBar>
				<div className="bill-header">
					<div className="bill-header-left">
						<button className="bill-header-left-button" onClick={this.backStep}>
							{this.state.pageLanguage.back[this.state.currentLanguage]}
						</button>
					</div>
					<div className="bill-header-title">
						<div>{this.state.pageLanguage.header[this.state.currentLanguage]}</div>
					</div>
					<div className="bill-header-right">
					</div>
				</div>

				<div className="login-form">
					<div className = "create-user-box">
						<Textbox name="username" placeholder={this.state.pageLanguage.emailTextbox[this.state.currentLanguage]} type="text" value={this.state.model.username} onChange={this.setUserState} />
					</div>
					<div className = "create-user-box">
						<Textbox name="firstName" placeholder={this.state.pageLanguage.firstName[this.state.currentLanguage]} type="text" value={this.state.model.firstName} onChange={this.setUserState} />
					</div>
					<div className = "create-user-box">
						<Textbox name="lastName" placeholder={this.state.pageLanguage.lastName[this.state.currentLanguage]} type="text" value={this.state.model.lastName} onChange={this.setUserState} />
					</div>
					<div className = "create-user-box">
						<Textbox name="password" placeholder={this.state.pageLanguage.password[this.state.currentLanguage]} type="password" value={this.state.model.password} onChange={this.setUserState} />
					</div>
					<div className = "create-user-box">
						<Textbox name="confirmPassword" placeholder={this.state.pageLanguage.confirmPassword[this.state.currentLanguage]} type="password" value={this.state.model.confirmPassword} onChange={this.setUserState} />		
					</div>
					<div className="login-button-group"> 
						<UI.Button type="primary" className="login-button" onTap={this.createUser}>
							{this.state.pageLanguage.register[this.state.currentLanguage]}
						</UI.Button>			
					</div>	

					<LoadingPopup ref="LoadingPopup" parent={this}></LoadingPopup>
				</div>
			</Container>
		</div>
	);
}

});

