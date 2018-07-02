import React from 'react';

var TextboxPopup = React.createClass({	
	propTypes: {
		name: React.PropTypes.string.isRequired,
		placeholder: React.PropTypes.string,
		value: React.PropTypes.string,
		type: React.PropTypes.string,
		onChange: React.PropTypes.func.isRequired,
		label: React.PropTypes.string.isRequired
	},

	render () {
		return (
			<div>
				<label className="textbox-label dark-blue" htmlFor={this.props.name}>{this.props.label}</label>
				<div className="textbox-group">
					<div className="textbox-group-item">
						<div className="textbox-label-group-item-inner">
							<div className="textbox-group-item-content">
								<input className="textbox-group-item-content-field textbox-label-group-item-content-field text-box-popup" 
								placeholder={this.props.placeholder}
								name={this.props.name}
								type={this.props.type}
								value={this.props.value}
								onChange={this.props.onChange} />
							</div>
						</div>
					</div>
				</div>	
			</div>
		);
	}
});

module.exports = TextboxPopup;