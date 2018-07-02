import React from 'react';
import BurgerMenu from 'react-burger-menu';
import classNames from 'classnames';

var MenuWrap = React.createClass({

  getInitialState() {
    return { hidden : false };
  },

  componentWillReceiveProps(nextProps) {
    const sideChanged = this.props.children.props.right !== nextProps.children.props.right;

    if (sideChanged) {
      this.setState({ hidden : true });

      setTimeout(() => {
        this.show();
      }, this.props.wait);
    }
  },

  show() {
    this.setState({ hidden : false });
  },

  render() {
    let style;

    if (this.state.hidden) {
      style = { display: 'none' };
    }

    return (
      <div style={ style } className={ this.props.side }>
        { this.props.children }
      </div>
    );
  }
});

module.exports = MenuWrap;