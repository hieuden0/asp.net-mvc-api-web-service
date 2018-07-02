module.exports = {
	viewManagerName:"tabs",
	pages:{
		loginScreen: "loading-screen",
		login: "login",
		guideStep1:"guide-step-1",
		guideStep2:"guide-step-2",
		guideStep3:"guide-step-3",
		camera:"camera",
		billDetail:"bill-detail", 
		billListDetail:"bill-list-detail",
		createUser:"create-user",
		setting:"setting",
	},
	actions:{
		new:'new',
		editFromUser:'edituser',
		editFromLocal:'editlocal'
	},
	getPage:function(name){
		return this.viewManagerName + ":" + name;
	},
	bills: 
	[
		{
			id: '1', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 1',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		},	
		{
			id: '2', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 2',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		},	
		{
			id: '3', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 3',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		},
		{
			id: '4', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 4',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		},
		{
			id: '5', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 5',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		},
		{
			id: '6', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 6',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		},
		{
			id: '7', 
			Reciept: 'img/ACCOUNT.png',
			Supplier: 'Taxiresa',
			Destination: 'Berlin 7',
		 	Date1: '2016-08-03',
		 	Price: '38,65',
		 	Currency: 'EUR',
		}
	]
};