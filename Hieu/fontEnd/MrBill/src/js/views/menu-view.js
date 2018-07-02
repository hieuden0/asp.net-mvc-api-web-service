import React from 'react';

import BurgerMenu from 'react-burger-menu';
import MenuWrap from '../components/common/menu-wrap';

var $ = require('jquery');

var Language = require('../../data/language');
var BillData = require('../../api/billData');

var menuView = React.createClass({
    
    gotoCamera(event){
        event.preventDefault();
        if(this.props.page === BillData.pages.camera){
          $('div.bm-cross-button button').click();  
        }
        else{
            this.removeListViewStateLocalStorage();
            this.props.parent.transitionTo(BillData.getPage(BillData.pages.camera), { transition: 'show-from-bottom' });
        }
    },
    gotoOverView (event) {
  		event.preventDefault();
         if(this.props.page === BillData.pages.billListDetail){
          $('div.bm-cross-button button').click();  
        }
        else{
            this.removeListViewStateLocalStorage();
            this.props.parent.transitionTo(BillData.getPage(BillData.pages.billListDetail),{ transition: 'show-from-left' });
        } 
	},
    gotoLogin (event) {
  		event.preventDefault();
		if(this.props.page === BillData.pages.login){
          $('div.bm-cross-button button').click();  
        }
        else{
            this.removeListViewStateLocalStorage();
            this.props.parent.transitionTo(BillData.getPage(BillData.pages.login), { transition: 'show-from-bottom' });
        } 
	},
	gotoGuide (event) {
  		event.preventDefault();
		if(this.props.page === BillData.pages.guideStep1){
          $('div.bm-cross-button button').click();  
        }
        else{
            this.removeListViewStateLocalStorage();
            this.props.parent.transitionTo(BillData.getPage(BillData.pages.guideStep1), { transition: 'show-from-bottom' });
        }
	},
    gotoSetting (event) {
        event.preventDefault();
        if(this.props.page === BillData.pages.setting){
          $('div.bm-cross-button button').click();  
        }
        else{
            this.removeListViewStateLocalStorage();
            this.props.parent.transitionTo(BillData.getPage(BillData.pages.setting), { transition: 'show-from-bottom' });
        }
    },
    removeListViewStateLocalStorage(){
        window.localStorage.removeItem("isEdit");
        window.localStorage.removeItem("idExpand");
        window.localStorage.removeItem("scrollTop");
    },
    render(){
        var parent = this.props.parent;
        const Menu = BurgerMenu[parent.state.currentMenu];
        let items;
        var menuLanguage = Language.getApplicationLanguages('menu');
        items = [
            <a key="0" onClick={this.gotoCamera}><i className="fa fa-fw fa-star-o"></i><span>{menuLanguage.camera[parent.state.currentLanguage]}</span></a>,
            <a key="1" onClick={this.gotoOverView}><i className="fa fa-fw fa-bell-o"></i><span>{menuLanguage.transaction[parent.state.currentLanguage]}</span></a>,
            <a key="2" onClick={this.gotoLogin}><i className="fa fa-fw fa-envelope-o"></i><span>{menuLanguage.logout[parent.state.currentLanguage]}</span></a>,
            <a key="3" onClick={this.gotoGuide}><i className="fa fa-fw fa-comment-o"></i><span>{menuLanguage.guide[parent.state.currentLanguage]}</span></a>,
            <a key="4" onClick={this.gotoSetting}><i className="fa fa-fw fa-comment-o"></i><span>{menuLanguage.setting[parent.state.currentLanguage]}</span></a>
        ];
        
        if(parent.state.side === 'right'){
            return (
                <MenuWrap wait={ 20 } side={ parent.state.side }>
                    <Menu id={ parent.state.currentMenu } pageWrapId={ "page-wrap" } outerContainerId={ "outer-container" } right>
                        { items }
                    </Menu>
                </MenuWrap>
            );
        }
        return (
            <MenuWrap wait={ 20 }>
                <Menu id={ parent.state.currentMenu } pageWrapId={ "page-wrap" } outerContainerId={ "outer-container" }>
                    { items }
                </Menu>
            </MenuWrap>
        );
    }
});



module.exports = menuView;