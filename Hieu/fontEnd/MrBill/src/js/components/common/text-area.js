import React from 'react';

var TextArea = React.createClass({	
	propTypes: {
		name: React.PropTypes.string.isRequired,
		value: React.PropTypes.string,
		onChange: React.PropTypes.func.isRequired,
		label: React.PropTypes.string.isRequired,
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
								<textarea rows="5" className="textbox-group-item-content-field webkit-color" 
									name={this.props.name}
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

module.exports = TextArea;