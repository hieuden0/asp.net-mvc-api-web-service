import Container from 'react-container';
import dialogs from 'cordova-dialogs';
import React from 'react';
import Tappable from 'react-tappable';
import Timers from 'react-timers';
import { Link, UI, Mixins } from 'touchstonejs';

import BurgerMenu from 'react-burger-menu';
import classNames from 'classnames';
import MenuView from '../views/menu-view';
import MenuWrap from '../components/common/menu-wrap';

var Dispatcher = require('../../dispatcher/appDispatcher');
var ActionTypes = require('../../constants/actionTypes');

var BillActions = require('../../actions/billActions');
var BillStore = require('../../stores/billStore');
var BillSql = require('../../api/billSql');

var BillUti = require('../../actions/BillUtility');
var BillData = require('../../api/billData');
var Language = require('../../data/language');

var attachFastClick = require('fastclick');
attachFastClick(document.body);



var $ = require('jquery');

const scrollable = Container.initScrollable();

module.exports = React.createClass({

	mixins: [Mixins.Transitions],
	statics: {
		navigationBar: 'main',
		getNavigation () {
			return {
				title: 'Camera'
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
			page: 'tabs:camera',
			language: ''
		};
		BillActions.updateCurrentSession(currentSession);
		
		window.localStorage.setItem("addmoreinfo", "false");
	},
	
	render () {
		return (
			<CameraPage />
		);
	}
});


var CameraPage = React.createClass({

	mixins: [Mixins.Transitions, Timers],

	changePictureFromGallery (event) {
			var current = this;
			 event.preventDefault();

			 this.hidePopup();

			  if (!navigator.camera) {
			      alert(this.state.pageLanguage.warningCameraNotSupport[this.state.currentLanguage], "Error");
			      return;
			  }

			var deviceType = (navigator.userAgent.match(/iPad/i))  == "iPad" ? "iPad" : (navigator.userAgent.match(/iPhone/i))  == "iPhone" ? "iPhone" : (navigator.userAgent.match(/Android/i)) == "Android" ? "Android" : (navigator.userAgent.match(/BlackBerry/i)) == "BlackBerry" ? "BlackBerry" : "null";

  			 if(deviceType == "Android")
  			 {
	 			 var width = $('.camera-content').width();
				  var height = $('.camera-content').height();

				  $('.camera-picture').height(height);
  			 }

			  var options =   {
		     					quality: 50,
			                    destinationType: Camera.DestinationType.DATA_URL,
			                    sourceType: 0,      // 0:Photo Library, 1=Camera, 2=Saved Album
			                    encodingType: 0,     // 0=JPG 1=PNG
			                    correctOrientation: true
			                  };

			  navigator.camera.getPicture(
			      function(imgData) {		          
			          var url = "data:image/jpeg;base64," + imgData;
			          //var url = imgData;
			          current.setState({image: url});
			          //dialogs.alert(url, function() {}, null);
			      },
			      function() {
			      	// dialogs.alert("Error taking picture", function() {}, null);
			      },
			      options);
			},

	changePicture (event) {
			var current = this;
			 event.preventDefault();

			this.hidePopup();

			  if (!navigator.camera) {
			      alert(this.state.pageLanguage.warningCameraNotSupport[this.state.currentLanguage], "Error");
			      return;
			  }

  			 var deviceType = (navigator.userAgent.match(/iPad/i))  == "iPad" ? "iPad" : (navigator.userAgent.match(/iPhone/i))  == "iPhone" ? "iPhone" : (navigator.userAgent.match(/Android/i)) == "Android" ? "Android" : (navigator.userAgent.match(/BlackBerry/i)) == "BlackBerry" ? "BlackBerry" : "null";

  			 if(deviceType == "Android")
  			 {
	 			 var width = $('.camera-content').width();
				  var height = $('.camera-content').height();

				  $('.camera-picture').height(height);
  			 }

			  var options =   {
			   					quality: 50,
			                    destinationType: Camera.DestinationType.DATA_URL,
			                    sourceType: 1,      // 0:Photo Library, 1=Camera, 2=Saved Album
			                    encodingType: 0,     // 0=JPG 1=PNG
			                    correctOrientation: true
			                  };

			  navigator.camera.getPicture(
			      function(imgData) {	
			      	var url = "data:image/jpeg;base64," + imgData;	          
			          //current.setState({image: url});

			          current.nextStepAutoRedirectToDetailPage(url);				
			      },
			      function() {
			      	// dialogs.alert("Error taking picture", function() {}, null);
			      },
			      options);
			},

	componentDidMount () {
		var deviceType = (navigator.userAgent.match(/iPad/i))  == "iPad" ? "iPad" : (navigator.userAgent.match(/iPhone/i))  == "iPhone" ? "iPhone" : (navigator.userAgent.match(/Android/i)) == "Android" ? "Android" : (navigator.userAgent.match(/BlackBerry/i)) == "BlackBerry" ? "BlackBerry" : "null";
	
		//  this.transitionTo('tabs:bill-detail');
		 
  			 if(deviceType == "Android")
  			 {
	 			 var width = $('.camera-content').width();
				  var height = $('.camera-content').height();

				  $('.camera-picture').height(height);
  			 }
	},

	nextStepAutoRedirectToDetailPage (image) {
		var self = this;

		self.showLoadingPopup();

		this.setTimeout(() => {
			var currentBill = BillActions._clone(this.state.billState.currentTransaction);
			if(currentBill.action==BillData.actions.editFromUser){
				currentBill = BillUti.NewTransaction();
			}
			currentBill.Reciept = image;

			currentBill.Traveller = window.localStorage.getItem("username");
			
			BillSql.updateRecordCurrentTransaction(BillUti.convertTransaction(currentBill), function() {
				Dispatcher.dispatch({
					actionType: ActionTypes.CURRENT_TRANSACTION,
					currentTransaction: currentBill
				});

				self.hideLoadingPopup();

				self.transitionTo('tabs:bill-detail', { transition: 'show-from-left' });

			}, function(error){});
		}, 1000);			
	},

	nextStep () {
		var self = this;
		
		if(self.state.image != "null")
		{
			self.showLoadingPopup();

			self.setTimeout(() => {
				var currentBill = BillActions._clone(self.state.billState.currentTransaction);
				if(currentBill.action==BillData.actions.editFromUser){
					currentBill = BillUti.NewTransaction();
				}
					currentBill.Reciept = self.state.image;

				currentBill.Traveller = window.localStorage.getItem("username");
				BillSql.updateRecordCurrentTransaction(BillUti.convertTransaction(currentBill), function() {
					Dispatcher.dispatch({
						actionType: ActionTypes.CURRENT_TRANSACTION,
						currentTransaction: currentBill
					});

					self.hideLoadingPopup();

					self.transitionTo('tabs:bill-detail', { transition: 'show-from-left' });

				}, function(error){});
			}, 1000);			
		}
		else
		{
			var message = self.state.pageLanguage.warningPhotoEmpty[self.state.currentLanguage];
			//dialogs.alert(message, function() {}, null);
			self.showAlertbar('warning', message);
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
	var image = 'null';

	var addmoreinfo = window.localStorage.getItem("addmoreinfo");	
	if(addmoreinfo && addmoreinfo == "true")
	{
		image = billState.currentTransaction.Reciept;
	}

    return {
      currentMenu: 'slide',
      side: 'right',
      //image: billState.currentTransaction.image,
      //image: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAF4AAABeBAMAAABY/L5dAAAAJFBMVEW+4f93qdS43fyVweeOu+KGtd18rde84P6u1Panz/Gdx+t/r9nblveaAAABdElEQVRYw+2WPUvEQBCGNX6cH41jLuG8KoJaWJnKwsYTDi2NXGFp7MTmQH+AgjZWZ2NhpVbaCYK/T1CRvby7MwMDcgd56ifJzmZ23p2oqan5Z+66Zdp71drRDn3zken0nH6JVQ880B9nCv2CHLZFvdFx/WZf8i9piH3Jz4f9WNDnqcKA9zeq/qliOfoFRQRknD+F/hbnL6L/zvnX6Lc4/xn9Nud30G9yfoF+wvkl+innkwerL6/HXq+8n/b/JfeDvd/kfpbPi+E82s87zhPDvLLPQ5y3hnluzgvMI0veyXk6fkSP3U9yOO+9cUVv+ubDS1C/IS8HAf2WAhx79TUKsuc7KWXYTwfYxzkxxNDX68RyVN34gveTjHm9/IFGIflJn52Dwly8l/0Vt9pS9lOn4mlSsAsxwdNiY44LvjlScQXLVxawqvOXIUV52lCuruCIlGSYQQhm04LWf4LtVG3ojNY/xEsDgleJWa1/8uNPav2l0fO/AAh2m6mav3yPAAAAAElFTkSuQmCC',
      image: image, //<===
      alertbar: {
			visible: false,
			type: '',
			text: ''
		},	
		billState: 	billState,
		popupcamera: {
				visible: false
		},
		popup: {
			visible: false
		},
		currentLanguage: billState.currentLanguageCode,
		pageLanguage: Language.getApplicationLanguages('cameraPage'),
		commonLanguage: Language.getApplicationLanguages('common')
    };
  },

  showAlertbar (type, text) {
	this.setState({
		alertbar: {
			visible: true,
			type: type,
			text: text
		}
	});

	this.setTimeout(() => {
		$('.Alertbar').hide();
		this.setState({
			alertbar: {
				visible: false
			}
		});
	}, 3000);
},

showPopup () {
	this.setState({
		popupcamera: {
			visible: true,
		}
	});
},

hidePopup () {
	$('.Popup-dialog').hide();
	this.setState({
		popupcamera: {
			visible: false
		}
	});
},

  render() {
    return (
      	<div id="outer-container" style={ { height: '100%' } }>
        
<MenuView parent={this} page={BillData.pages.camera}></MenuView>
	        <div className="guide-background">
	        	<UI.Alertbar type={this.state.alertbar.type || 'default'} visible={this.state.alertbar.visible} animated>{this.state.alertbar.text || ''}</UI.Alertbar>
				<div className="camera-header">
					<div className="camera-button-top-left">
						
					</div>
					<div className="header-central-image">
						<img src="img/MRBILL_NAVY.png" className="camera-header-image" />
					</div>
					<div className="camera-button-top-right">
						
					</div>
				</div>

				<div className="camera-content">
					<img src={this.state.image} className={this.state.image != "null" ? "camera-picture": "camera-picture list-hidden"}  />	
					<div className={this.state.image != "null" ? "camera-center-hint-hidden" : "camera-center-hint"}>{this.state.pageLanguage.textReceipt[this.state.currentLanguage]} <br /> {this.state.pageLanguage.textPhoto[this.state.currentLanguage]}</div>				
				</div>
						
				<div className="camera-footer">
					<div className="camera-button-bottom-left">
					</div>
					<div className="footer-central-image">
						<img src="img/CAMERA.png" className="camera-capture-button" onClick={this.showPopup} />
					</div>
					<div className="camera-button-bottom-right">
						<img src="img/SEND2.png" className={this.state.image != "null" ? "camera-next-button": "camera-next-button list-hidden"} onClick={this.nextStep} />
					</div>
				</div>
				<div className="camera-page">	
					<UI.Popup visible={this.state.popupcamera.visible}>
						<div className="camera-option camera-roll" onClick={this.changePictureFromGallery}><strong>{this.state.pageLanguage.popupGallery[this.state.currentLanguage]}</strong></div>
						<hr />
						<div className="camera-option camera-roll" onClick={this.changePicture}><strong>{this.state.pageLanguage.popupCamera[this.state.currentLanguage]}</strong></div>
						<hr />
						<div className="camera-option" onClick={this.hidePopup}><strong>{this.state.pageLanguage.popupCancel[this.state.currentLanguage]}</strong></div>
					</UI.Popup>
				</div>

				
			</div>			
	    </div>
    );
  }
});