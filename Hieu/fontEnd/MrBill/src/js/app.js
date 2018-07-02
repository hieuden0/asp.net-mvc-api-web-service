import React from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import ReactDOM from 'react-dom';
import {
	Container,
	createApp,
	UI,
	View,
	ViewManager
} from 'touchstonejs';

import Timers from 'react-timers';

var $ = require('jquery');

var BillSql = require('../api/billSql');

var BillData = require('../api/billData');
// App Config
// ------------------------------

var self;
var App = React.createClass({
	mixins: [createApp()],

	componentDidMount () {
		// Hide the splash screen when the app is mounted
		if (navigator.splashscreen) {
			navigator.splashscreen.hide();
		}
		/*BillSql.deleteTransaction();
		BillSql.deleteCurrentTransaction();
		BillSql.dropTableSession();*/
		

	},

	render () {		
		let appWrapperClassName = 'app-wrapper device--' + (window.device || {}).platform

		return (
			<div className={appWrapperClassName}>
				<div className="device-silhouette">
					<ViewManager name="app" defaultView="main">
						<View name="main" component={MainViewController} />
					</ViewManager>
				</div>
			</div>
		);
	}
});

// Main Controller
// ------------------------------

var MainViewController = React.createClass({
	render () {
		return (
			<Container>
			<UI.NavigationBar name="main" />
				<ViewManager name="main" defaultView="tabs">
					<View name="tabs" component={TabViewController} />
				</ViewManager>
			</Container>
		);
	}
});

// Tab Controller
// ------------------------------

var lastSelectedTab = 'loading-screen';
var TabViewController = React.createClass({
	mixins: [Timers],

	getInitialState () {
		self = this;

		return {
			selectedTab: lastSelectedTab,
			popup: {
				visible: false
			},
		};
	},

	showLoadingPopup () {
		this.setState({
			popup: {
				visible: true,
				loading: true,
				header: 'Loading',
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

	onViewChange (nextView) {
		lastSelectedTab = nextView

		this.setState({
			selectedTab: nextView
		});
	},

	selectTab (value) {
		let viewProps;

		this.refs.vm.transitionTo(value, {
			transition: 'instant',
			viewProps: viewProps
		});

		this.setState({
			selectedTab: value
		})
	},

	render () {
		let selectedTab = this.state.selectedTab
		let selectedTabSpan = selectedTab

		if (selectedTab === 'lists' || selectedTab === 'list-simple' || selectedTab === 'list-complex' || selectedTab === 'list-details') {
			selectedTabSpan = 'lists';
		}

		if (selectedTab === 'transitions' || selectedTab === 'transitions-target') {
			selectedTabSpan = 'transitions';
		}

		return (
			<Container>
				<ViewManager ref="vm" name={BillData.viewManagerName} defaultView={selectedTab} onViewChange={this.onViewChange}>				
					<View name={BillData.pages.loginScreen} component={require('./views/loading-screen')} />
					<View name={BillData.pages.login} component={require('./views/login')} />	
					<View name={BillData.pages.guideStep1} component={require('./views/guide-step-1')} />	
					<View name={BillData.pages.guideStep2} component={require('./views/guide-step-2')} />	
					<View name={BillData.pages.guideStep3} component={require('./views/guide-step-3')} />
					<View name={BillData.pages.camera} component={require('./views/camera')} />
					<View name={BillData.pages.billDetail} component={require('./views/bill-detail')} />	
					<View name={BillData.pages.billListDetail} component={require('./views/bill-list-detail')} />	
					<View name={BillData.pages.createUser} component={require('./views/create-user')} />	
					<View name={BillData.pages.setting} component={require('./views/setting')} />	
				</ViewManager>

				<UI.Popup visible={this.state.popup.visible}>
					<UI.PopupIcon name={this.state.popup.iconName} type={this.state.popup.iconType} spinning={this.state.popup.loading} />		
					<div><strong>{this.state.popup.header}</strong></div>
				</UI.Popup>	
			</Container>
		);
	}
});

function startApp () {
	if (window.StatusBar) {
		window.StatusBar.styleDefault();
	}
	ReactDOM.render(<App />, document.getElementById('app'));
	universalLinks.subscribe('openNewsListPage', function(){console.log('Showing to user list of awesome news');};
}

function onPause () {
	var date = new Date();

	window.localStorage.setItem("pauseTime", date.getTime().toString());
}

function onResume () {
	self.showLoadingPopup();

	setTimeout(function(){
		BillSql.selectTop1CurrentSession(function(queryOject, resultsOject){
			var currentSession = null;
			if(resultsOject.rows.length > 0)
			{
				currentSession = resultsOject.rows.item(0);
			}
			
			if(currentSession != null)
			{
			 	if(currentSession.page != 'tabs:login' && currentSession.page != 'tabs:camera')
			 	{

 					var pauseTimeData = window.localStorage.getItem("pauseTime");
					var pauseTime = parseInt(pauseTimeData);

					var date = new Date();
					var resumeTime = date.getTime();

					var diffMinute = (resumeTime - pauseTime) / 1000 / 60;

			 		self.hideLoadingPopup();

			 		if(diffMinute >= 60)
			 		{
				 		self.selectTab('camera');
				 	}
				}
			}

			self.hideLoadingPopup();
		}, function(error){});
	}, 1000);
}

if (!window.cordova) {
	startApp();
} else {
	document.addEventListener('deviceready', startApp, false);

	document.addEventListener("pause", onPause, false);

	document.addEventListener("resume", onResume, false);
}