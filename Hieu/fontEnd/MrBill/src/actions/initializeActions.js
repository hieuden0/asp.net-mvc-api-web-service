var Dispatcher = require('../dispatcher/appDispatcher');
var ActionTypes = require('../constants/actionTypes');
var BillApi = require('../api/billApi');

var InitializeActions = {
	initApp: function() {
		Dispatcher.dispatch({
			actionType: ActionTypes.INITIALIZE,
			initialData: {
				bills: BillApi.getAllBills()
			}
		});
	}
};

module.exports = InitializeActions;