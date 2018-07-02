import React from 'react';
import { Link, UI, Mixins } from 'touchstonejs';

var $ = require('jquery');

var Language = require('../../../data/language');
var BillData = require('../../../api/billData');

var ConfirmPopup = React.createClass({

  propTypes: {
		onClick: React.PropTypes.func.isRequired,

	},
    
  getInitialState () {
		  return { 
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
				  iconName: 'ion-load-c',
				  iconType: 'default'
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

	confirm(){
		this.props.onClick();
		this.hide();
	},

  isShow(){
  	if(this.state.popup.visible){
  		return true;
  	}else{
  		return false;
  	}
  },
  render(){
     return(
       <UI.Popup visible={this.state.popup.visible} className="confirmPopup">
			   
				    <div><strong>{this.props.titleText}</strong></div>
				    <hr className="dark-blue" />
				    <div className="bill-detail-popup-footer" id = "confirmPopup">
				    			<div className="bill-detail-popup-footer-content text-align-left">
									<a className="bill-detail-popup-footer-text" onClick={this.hide} >{this.props.cancelText}</a>
								</div>
								<div className="bill-detail-popup-footer-content text-align-right">
									<a className="bill-detail-popup-footer-text" onClick={this.confirm} id="saveTransaction">{this.props.saveText}</a>
								</div>								
							</div>
		    </UI.Popup>
     )
   }
});



module.exports = ConfirmPopup;