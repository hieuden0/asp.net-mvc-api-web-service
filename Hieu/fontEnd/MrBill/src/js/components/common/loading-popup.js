import React from 'react';
import { Link, UI, Mixins } from 'touchstonejs';

var $ = require('jquery');

var Language = require('../../../data/language');
var BillData = require('../../../api/billData');

var loadingPopup = React.createClass({
    
  getInitialState () {
		  return { 
        headerText: Language.getApplicationLanguages('loadingPopup'),
        popup:{
          visible: false
        }
		  }
	},
  show() {
		this.setState({
        popup:{
          visible: true,
				  loading: true,
				  header: this.state.headerText.popupLoading[this.props.parent.state.currentLanguage],
				  iconName: 'ion-load-c',
				  iconType: 'default'
        }
		});
	},
  update(){
    this.setState({
		  popup: {
			  visible: true,
			  loading: false,
			  header: this.state.headerText.popupDone[this.props.parent.state.currentLanguage],
			  iconName: 'ion-ios-checkmark',
			  iconType: 'success'
		  }
	  });
  },
  hide () {
		$('.Popup-dialog').hide();
		this.setState({
			popup: {
				visible: false
			}
		});
	},
  render(){
     return(
       <UI.Popup visible={this.state.popup.visible}>
			    <UI.PopupIcon name={this.state.popup.iconName} type={this.state.popup.iconType} spinning={this.state.popup.loading} />		
				    <div><strong>{this.state.popup.header}</strong></div>
		    </UI.Popup>
     )
   }
});



module.exports = loadingPopup;