import React from 'react';

var TextboxLabel = React.createClass({	
	propTypes: {
		name: React.PropTypes.string.isRequired,
		placeholder: React.PropTypes.string,
		value: React.PropTypes.string,
		type: React.PropTypes.string,
		label: React.PropTypes.string.isRequired,
		onChange: React.PropTypes.func.isRequired,
		onClick: React.PropTypes.func.isRequired
	},

	render () {
		return (
			<div>
				<label className="textbox-label" htmlFor={this.props.name}>{this.props.label}</label>
				<div className="textbox-group">
					<div className="textbox-group-item">
						<div className="textbox-label-group-item-inner">
							<div className="textbox-group-item-content" onClick={this.props.onClick} name={this.props.name}>
								<input className="textbox-group-item-content-field textbox-label-group-item-content-field webkit-color" 
								placeholder={this.props.placeholder}
								name={this.props.name}
								type={this.props.type}
								value={this.props.value}
								onChange={this.props.onChange} disabled />
							</div>
						</div>
					</div>
				</div>	
			</div>
		);
	}
});

module.exports = TextboxLabel;