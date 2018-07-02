import React from 'react';
import { Link, UI, Mixins } from 'touchstonejs';

var $ = require('jquery');

var alertBar = React.createClass({
    
  getInitialState () {
		  return { 
        visible: false,
				type: '',
				text: ''
		  }
	},
  show (type, text) {
	var self = this.props.parent;
	this.setState({
			visible: true,
			type: type,
			text: text
	});
	
	self.setTimeout(() => {
		$('.Alertbar').hide();

		this.setState({
				visible: false
		});
	}, 3000);
},
render(){
     return(
     		<UI.Alertbar type={this.state.type || 'default'} visible={this.state.visible} animated>{this.state.text || ''}</UI.Alertbar>
     )
   }
});



module.exports = alertBar;