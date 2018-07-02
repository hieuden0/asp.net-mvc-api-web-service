import React from 'react';

var Textbox = React.createClass({	
	propTypes: {
		name: React.PropTypes.string.isRequired,
		placeholder: React.PropTypes.string,
		value: React.PropTypes.string,
		type: React.PropTypes.string,
		onChange: React.PropTypes.func.isRequired,
	},

	render () {
		return (
			<div className="textbox-group">
				<div className="textbox-group-item">
					<div className="textbox-group-item-inner">
						<div className="textbox-group-item-content">
							<input className="textbox-group-item-content-field" 
							placeholder={this.props.placeholder}
							name={this.props.name}
							type={this.props.type}
							value={this.props.value} 
							onChange={this.props.onChange} />
						</div>
					</div>
				</div>
			</div>
		);
	}
});

module.exports = Textbox;