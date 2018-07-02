import Container from 'react-container';
import dialogs from 'cordova-dialogs';
import React from 'react';
import Tappable from 'react-tappable';
import { Link, UI, Mixins } from 'touchstonejs';

import Textbox from '../components/common/textbox';

const scrollable = Container.initScrollable();

var BillActions = require('../../actions/billActions');
var BillStore = require('../../stores/billStore');

var Language = require('../../data/language');

var attachFastClick = require('fastclick');
attachFastClick(document.body);

module.exports = React.createClass({
	mixins: [Mixins.Transitions],
	statics: {
		navigationBar: 'main',
		getNavigation () {
			return {
				title: 'Step 3'
			}
		}
	},

	componentDidMount () {
		var currentSession = {
			username: '',
			page: 'tabs:guide-step-3',
			language: ''
		};

		BillActions.updateCurrentSession(currentSession);

		window.localStorage.setItem("addmoreinfo", "false");
	},
	
	getInitialState () {
		return {
			currentLanguage: BillStore.getBillState().currentLanguageCode,
			pageLanguage: Language.getApplicationLanguages('guideStep3Page'),
			commonLanguage: Language.getApplicationLanguages('common')
		}
	},

	skipStep () {
		var self = this;
		self.transitionTo('tabs:camera', { transition: 'show-from-left' });
	},
	
	nextStep () {
		var self = this;
		self.transitionTo('tabs:camera', { transition: 'show-from-left' });
	},
	
	render () {
		return (
			<div className="guide-background">
				<div className="guide-header">
					<img src="img/MRBILL_NT.png" className="guide-header-image" />
				</div>

				<div className="guide-body">
					<div className="content-image-capture">
						<div className="image-capture">
							<img src="img/sync.png" className="guide-body-image-step-1" />
						</div>	
					</div>

					<div className="guide-body-content">
						<div className="guide-body-content-header">
							{this.state.pageLanguage.header[this.state.currentLanguage]}
						</div>
						<div className="guide-body-content-text">
							{this.state.pageLanguage.text[this.state.currentLanguage]}
						</div>
					</div>
				</div>

				

				<div className="guide-footer">
					<div className="div-bottom-button">
					<div className="span-inactive"></div>
					<div className="span-inactive central-button"></div>
					<div className="span-active"></div>
					</div>

					<div className="div-bottom-navigate">
						<div>
						<a className="guid-bottom-text footer-skip" onClick={this.skipStep}>{this.state.commonLanguage.skipButton[this.state.currentLanguage]}</a>
						<a className="guid-bottom-text footer-next" onClick={this.nextStep}>{this.state.commonLanguage.nextButton[this.state.currentLanguage]}</a>
						</div>
					</div>
				</div>								
			</div>			
		);
	}
});
