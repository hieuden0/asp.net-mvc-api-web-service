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
        user: {
            email: "",
            password: ""
        },
		billState: billState,
		currentLanguage: billState.currentLanguageCode,
		pageLanguage: Language.getApplicationLanguages('loginPage')
    }
},

handleLabelSelectChange (key, event) {
	let newState = {};
	newState[key] = event.target.value;

	this.setState(newState);
},

componentDidMount () {
	var currentSession = {
		username: '',
		page: 'tabs:login',
		language: ''
	};

	BillActions.updateCurrentSession(currentSession);
	window.localStorage.setItem("addmoreinfo", "false");
},

login: function () {
	var self = this;
	var loadingPopup = self.refs['LoadingPopup'];
	var alertBar = self.refs['AlertBar'];
	if(self.state.user.email == "" || self.state.user.password == "")
	{
		var message = self.state.pageLanguage.warningLoginBlank[self.state.currentLanguage];
		alertBar.show('warning', message);
		return;
	}

	loadingPopup.show();
	

	self.setTimeout(() => {
		//BillActions.login(this.state.user.email, this.state.user.password);
		
		var url = apiUrl + "Login";
		//var url = "http://120.72.81.104/services/MrBillMobileServices.svc/Login"
		$.ajax({
			type : 'POST',
			url : url,
			timeout: 60000,
			contentType : "application/json",
			data: JSON.stringify({
				username:self.state.user.email,
				password:self.state.user.password
			}),
			async: true,
			success : function(data) {
				if(data)
				{
					if(data.LoginResult != null && data.LoginResult.Id > 0)
					{

						Dispatcher.dispatch({
							actionType: ActionTypes.LOGIN,
							isSuccess: true,
							username: self.state.user.email
						});

						window.localStorage.setItem("username", self.state.user.email);
						window.localStorage.setItem("userId", data.LoginResult.Id);
						var currentSession = {
							username: self.state.user.email,
							page: 'tabs:login',
							language: self.state.currentLanguage
						};

						//BillActions.updateCurrentSession(currentSession);
						BillSql.updateCurrentSession(currentSession, function() {
							Dispatcher.dispatch({
								actionType: ActionTypes.LANGUAGE,
								currentLanguageCode: self.state.currentLanguage
							});

							var query = "Traveller ='"	+ self.state.user.email + "'";
							
							BillSql.selectTopCurrentTransaction(1,query,function(queryOject,resultsOject){
								if(resultsOject.rows.length>0){
									Dispatcher.dispatch({
										actionType: ActionTypes.CURRENT_TRANSACTION,
										currentTransaction: resultsOject.rows[0]
									});
									self.transitionTo('tabs:camera', { transition: 'show-from-left' });
								}
								else{
									var currentTransaction = BillUti.NewTransaction();
									currentTransaction.Id = BillUti.getTime();
									currentTransaction.Traveller = self.state.user.email;
									BillSql.insertRecordCurrentTransaction(currentTransaction, function() {
										self.transitionTo('tabs:guide-step-1', { transition: 'show-from-left' });
										
									},function(error){});
								}
							},function(error){});				
						}, function(error){});	
					}
					else
					{
						loadingPopup.hide();

						var message = self.state.pageLanguage.warningLoginFail[self.state.currentLanguage];
						alertBar.show('warning', message);
					}
				}
			},
			error : function(request, status, _error) {	
				loadingPopup.hide();
				alertBar.show('warning', _error);
			},
		});

	}, 1000);
},

setUserState (event) {
    this.setState({dirty: true});
    var field = event.target.name;
    var value = event.target.value;
    this.state.user[field] = value;
    return this.setState({user: this.state.user});
},

createUser(){
	var self = this;
	var currentSession = {
		username: self.state.user.email,
		page: 'tabs:login',
		language: self.state.currentLanguage
	};
		BillSql.updateCurrentSession(currentSession, function() {
			Dispatcher.dispatch({
				actionType: ActionTypes.LANGUAGE,
				currentLanguageCode: self.state.currentLanguage
			});
			self.transitionTo('tabs:create-user', { transition: 'show-from-left' });
		}, function(error){});	
},

render () {
    return (
		<div className="login-background">
			<Container scrollable={scrollable}>
				<AlertBar ref="AlertBar" parent={this}></AlertBar>
				<div className="login-div">
					<img src="img/MRBILL_NAVY.png" className="loading-image" />
				</div>

				<div className="login-form">
					<Textbox name="email" placeholder={this.state.pageLanguage.emailTextbox[this.state.currentLanguage]} type="text" value={this.state.user.email} onChange={this.setUserState} />

					<Textbox name="password" placeholder={this.state.pageLanguage.passwordTextbox[this.state.currentLanguage]} type="password" value={this.state.user.password} onChange={this.setUserState} />		

					<div className="login-button-group"> 
						<UI.Button type="primary" className="login-button" onTap={this.login}>
							{this.state.pageLanguage.loginButton[this.state.currentLanguage]}
						</UI.Button>


						<UI.Button type="primary" className="login-button" onTap={this.createUser}>	
							{this.state.pageLanguage.createAccountButton[this.state.currentLanguage]}
						</UI.Button>

						<div className="div-password-recovery">
							<a href="http://mrbill.mrorange.nu/Account/LostPassword" className="password-recovery">{this.state.pageLanguage.forgotPasswordButton[this.state.currentLanguage]}</a>
						</div>	

						<div className="login-language">
							<div className="login-language-label">{this.state.pageLanguage.laguage[this.state.currentLanguage]}</div>
							<div className="login-language-dropdown-group">
								<UI.LabelSelect label="Language" value={this.state.currentLanguage} onChange={this.handleLabelSelectChange.bind(this, 'currentLanguage')} options={LANGUAGE_CULTURE} />
							</div>	
						</div>				
					</div>	

					<LoadingPopup ref="LoadingPopup" parent={this}></LoadingPopup>
				</div>
			</Container>
		</div>
	);
}

});

//a href="http://mrbill.mrorange.nu/Account/Login" className="login-button">{this.state.pageLanguage.createAccountButton[this.state.currentLanguage]}</a>