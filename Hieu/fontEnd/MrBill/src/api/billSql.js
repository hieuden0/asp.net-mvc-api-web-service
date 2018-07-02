
var BillSql = {
	//=======================================Function for data base===========================================
	//Database name: MrBill 		
	//Database version: 1.0
	//Database displayName: MrBill Mobile
	//Database size: 20000000
	//Table: TRANSACTIONS
	//fields:
	//reference unique
	//username
	//date1
	//supplier
	//locationInfo
	//price
	//vat
	//currency
	//extrainfo
	//imagefile

	//create a connect to database
	ConnectDatabase() {
	    return window.openDatabase("MrBill", "1.0", "MrBill Mobile", 51380224); // 49 MB
	},


	//function check if database,table exist do nothing, if not create database and table
	//param: success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(queryOject,error))
	createTableCurrentTransactionIfNotExists(success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    var queryCommand = function(qr){
	    		var queryText ='CREATE TABLE IF NOT EXISTS CURRENT_TRANSACTION(Id, ExpensDate, Reference, Product, PaymentMethod, CardHolder, IsDisabled, Reciept, CostCenter, Traveller, Date1, Date2, DateAdded, Supplier, Destination, Price, Vat, Currency, ExtraInfo, action)';
		        qr.executeSql(queryText);
			};
		db.transaction(queryCommand, error, success); 
	},

	//function insert data to table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	insertRecordCurrentTransaction(record, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			//var queryText = "INSERT INTO CURRENT_TRANSACTION VALUES ('" + record.Id + "','" + record.ExpensDate + "','" + record.Reference + "','" + record.Product + "','" + record.PaymentMethod  + "','" + record.CardHolder + "','"  + record.IsDisabled + "','"  + record.Reciept + "','"  + record.CostCenter + "','"  + record.Traveller + "','"  + record.Date1 + "','"  + record.Date2 + "','"  + record.DateAdded + "','"  + record.Supplier + "','"  + record.Destination + "','"  + record.Price + "','" + record.Vat + "','" + record.Currency + "','" + record.Description +"','" + record.action + "')";
			var queryText = "INSERT INTO CURRENT_TRANSACTION VALUES ('" + record.Id + "','" + record.ExpensDate + "','" + record.Reference + "','" + record.Product + "','" + record.PaymentMethod  + "','" + record.CardHolder + "','"  + record.IsDisabled + "','"  + record.Reciept + "','"  + record.CostCenter + "','"  + record.Traveller + "','"  + record.Date1 + "','"  + record.Date2 + "','"  + record.DateAdded + "','"  + record.Supplier + "','"  + record.Destination + "','"  + record.Price + "','" + record.Vat + "','" + record.Currency + "','" + record.ExtraInfo +"','" + record.action + "')";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);

	},

	//function update data in table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	updateRecordCurrentTransaction(record, success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			
			var queryText = 'UPDATE CURRENT_TRANSACTION SET';
			for(var key in record){
				if(key == "ExtraInfo"){
					queryText += " " + key + "='" + record['Description'] + "',"
				}else if(key == "Description"){
				}
				else if(key !=='Traveller'){
					queryText += " " + key + "='" + record[key] + "',"
				}
			}

			if(queryText.lastIndexOf(",") === queryText.length-1) {
				queryText=queryText.substr(0 ,queryText.length - 1);
			}
			queryText += " WHERE Traveller = '" + record.Traveller + "'";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function delete data from table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	deleteRecordCurrentTransaction(record, success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DELETE FROM CURRENT_TRANSACTION WHERE';

			queryText += " Traveller = '" + record.username + "'";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function select top or select limit
	//param: top(int)
	//       wherestring(string, part of where segment in sqlQuery,after "where" word)
	//		 success(a function if select success, format: function(queryOject,resultsOject))
	//       error(a function if this function gets any error, format:function(error))
	selectTopCurrentTransaction(top, whereString, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    if(top === undefined || top === 0 || top === '0' || top === null) 
	    	return me.selectAll(whereString, success, error);

		var queryText = 'SELECT * FROM CURRENT_TRANSACTION ';
			if(whereString !== null && whereString !== undefined && whereString !== ''){
				queryText += ' WHERE ' + whereString;
			}

			queryText += ' LIMIT ' + top;

	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }
	    db.transaction(queryCommand, error);
	},

	//function select * 
	//param: wherestring(string, part of where segment in sqlQuery,after "where" word)
	//		 success(a function if select success, format: function(queryOject,resultsOject))
	//       error(a function if this function gets any error, format:function(error))
	selectAllCurrentTransaction(whereString, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryText = 'SELECT * FROM CURRENT_TRANSACTION';
		if(whereString !== null && whereString !== undefined && whereString !== ''){
			queryText += ' WHERE ' + whereString;
		}
	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }

	    db.transaction(queryCommand, error);
	},

	//=========================================================================================================
	// TRANSACTIONS
	//=========================================================================================================
	//function check if database,table exist do nothing, if not create database and table
	//param: success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(queryOject,error))
	createTableTransactionsIfNotExists(success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    var queryCommand = function(qr){
	    		var queryText ='CREATE TABLE IF NOT EXISTS TRANSACTIONS(Id, ExpensDate, Reference, Product, PaymentMethod, CardHolder, IsDisabled, Reciept, CostCenter, Traveller, Date1, Date2, DateAdded, Supplier, Destination, Price, Vat, Currency, ExtraInfo, action)';
		        qr.executeSql(queryText);
			};
		db.transaction(queryCommand, error, success); 
	},

	//function insert data to table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	insertRecordTransactions(record, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = "INSERT INTO TRANSACTIONS VALUES ('" + record.Id + "','" + record.ExpensDate + "','" + record.Reference + "','" + record.Product + "','" + record.PaymentMethod  + "','" + record.CardHolder + "','"  + record.IsDisabled + "','"  + record.Reciept + "','"  + record.CostCenter + "','"  + record.Traveller + "','"  + record.Date1 + "','"  + record.Date2 + "','"  + record.DateAdded + "','"  + record.Supplier + "','"  + record.Destination + "','"  + record.Price + "','" + record.Vat + "','" + record.Currency + "','" + record.ExtraInfo + "','"+record.action + "')";
			//var queryText = "INSERT INTO TRANSACTIONS VALUES ('" + record.Id + "','" + record.ExpensDate + "','" + record.Reference + "','" + record.Product + "','" + record.PaymentMethod  + "','" + record.CardHolder + "','"  + record.IsDisabled + "','"  + record.Reciept + "','"  + record.CostCenter + "','"  + record.Traveller + "','"  + record.Date1 + "','"  + record.Date2 + "','"  + record.DateAdded + "','"  + record.Supplier + "','"  + record.Destination + "','"  + record.Price + "','" + record.Vat + "','" + record.Currency + "','" + record.ExtraInfo + "','"+record.action + "')";
			qr.executeSql(queryText);
		};
		db.transaction(queryCommand, error, success);

	},

	//function update data in table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	updateRecordTransactions(record, success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'UPDATE TRANSACTIONS SET';
			for(var key in record){
				if(key == "Description"){
				}else if(key !== 'Id'){
					queryText += " " + key + "='" + record[key] + "',"
				}
			}

			if(queryText.lastIndexOf(",") === queryText.length-1) {
				queryText=queryText.substr(0 ,queryText.length - 1);
			}
			queryText += " WHERE Id = '" + record.Id +"'";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function delete data from table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	deleteRecordTransactions(record, success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DELETE FROM TRANSACTIONS ';

			queryText += " WHERE Id = '" + record.Id + "'";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function select top or select limit
	//param: top(int)
	//       wherestring(string, part of where segment in sqlQuery,after "where" word)
	//		 success(a function if select success, format: function(queryOject,resultsOject))
	//       error(a function if this function gets any error, format:function(error))
	selectTopTransactions(top, whereString, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    if(top === undefined || top === 0 || top === '0' || top === null) 
	    	return me.selectAll(whereString, success, error);

		var queryText = 'SELECT * FROM TRANSACTIONS ';
			if(whereString !== null && whereString !== undefined && whereString !== ''){
				queryText += ' WHERE ' + whereString;
			}

			queryText += ' LIMIT ' + top;

	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }
	    db.transaction(queryCommand, error);
	},

	//function select * 
	//param: wherestring(string, part of where segment in sqlQuery,after "where" word)
	//		 success(a function if select success, format: function(queryOject,resultsOject))
	//       error(a function if this function gets any error, format:function(error))
	selectAllTransactions(whereString, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryText = 'SELECT * FROM TRANSACTIONS';
		if(whereString !== null && whereString !== undefined && whereString !== ''){
			queryText += ' WHERE ' + whereString;
		}
	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }

	    db.transaction(queryCommand, error);
	},
	//=========================================================================================================

	createTableCurrentSessionIfNotExists(success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    var queryCommand = function(qr){
	    		var queryText ='CREATE TABLE IF NOT EXISTS CURRENT_SESSION(username, page, language)';
		        qr.executeSql(queryText);
			};
		db.transaction(queryCommand, error, success); 
	},

	//function insert data to table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	insertCurrentSession(record, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = "INSERT INTO CURRENT_SESSION VALUES ('" + record.username + "','" + record.page + "','" + record.language + "')";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);

	},

	//function update data in table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	updateCurrentSession(record, success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'UPDATE CURRENT_SESSION SET';
			for(var key in record){
				if(record[key] != ''){
					queryText += " " + key + "='" + record[key] + "',"
				}
			}

			if(queryText.lastIndexOf(",") === queryText.length-1) {
				queryText=queryText.substr(0 ,queryText.length - 1);
			}

			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function delete data from table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	deleteCurrentSession(success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DELETE FROM CURRENT_SESSION';
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function select top or select limit
	//param: top(int)
	//       wherestring(string, part of where segment in sqlQuery,after "where" word)
	//		 success(a function if select success, format: function(queryOject,resultsOject))
	//       error(a function if this function gets any error, format:function(error))
	selectTop1CurrentSession(success, error){
	    var me = this;
	    var db = me.ConnectDatabase();

		var queryText = 'SELECT * FROM CURRENT_SESSION LIMIT 1';

	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }
	    db.transaction(queryCommand, error);
	},

	//=========================================================================================================
	// USER SETTING
	//=========================================================================================================
	//function check if database,table exist do nothing, if not create database and table
	//param: success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(queryOject,error))
	createTableSettingIfNotExists(success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    var queryCommand = function(qr){
	    		var queryText ='CREATE TABLE IF NOT EXISTS SETTING(ID_setting INTEGER PRIMARY KEY AUTOINCREMENT, user_id, setting_name, setting_value)';
		        qr.executeSql(queryText);
			};
		db.transaction(queryCommand, error, success); 
	},

	//function insert data to table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	insertRecordSetting(record, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = "INSERT INTO SETTING(user_id, setting_name, setting_value) VALUES ('" + record.user_id + "','" + record.setting_name + "','" + record.setting_value + "')";
			qr.executeSql(queryText);
		};
		db.transaction(queryCommand, error, success);

	},


	//function delete data from table 
	//param: record(JavaScript Ojbect, format : field:value )
	//		 success(a function if select success, format: function())
	//       error(a function if this function gets any error, format:function(error))
	deleteRecordSetting(record, success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DELETE FROM SETTING ';

			queryText += " WHERE user_id = '" + record.user_id + "'";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	//function select * 
	//param: wherestring(string, part of where segment in sqlQuery,after "where" word)
	//		 success(a function if select success, format: function(queryOject,resultsOject))
	//       error(a function if this function gets any error, format:function(error))
	selectAllSettingByUserID(whereString, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryText = 'SELECT * FROM SETTING';
		if(whereString !== null && whereString !== undefined && whereString !== ''){
			//queryText += ' WHERE ' + whereString;
			queryText += " WHERE user_id = '" + record.user_id + "'";
		}
	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }

	    db.transaction(queryCommand, error);
	},
	//=========================================================================================================


	//=========================================================================================================
	createTableHistoryIfNotExists(success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
	    var queryCommand = function(qr){
	    		var queryText ='CREATE TABLE IF NOT EXISTS HISTORY(username)';
		        qr.executeSql(queryText);
			};
		db.transaction(queryCommand, error, success); 
	},

	insertRecordHistory(record, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = "INSERT INTO HISTORY VALUES ('" + record.username + "')";
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);

	},

	selectTop1History(whereString, success, error){
	    var me = this;
	    var db = me.ConnectDatabase();

		var queryText = 'SELECT * FROM HISTORY ';
			if(whereString !== null && whereString !== undefined && whereString !== ''){
				queryText += ' WHERE ' + whereString;
			}

			queryText += ' LIMIT 1';

	    var queryCommand = function(tx) {
	        tx.executeSql(queryText, [], success, error);
	    }
	    db.transaction(queryCommand, error);
	},


	deleteTransaction(success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DROP TABLE IF EXISTS TRANSACTIONS';
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},
	deleteHistory(success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DROP TABLE IF EXISTS HISTORY';
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	deleteCurrentTransaction(success, error){
	  	var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DROP TABLE IF EXISTS CURRENT_TRANSACTION';
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	},

	dropTableSession(success, error)
	{
		var me = this;
	    var db = me.ConnectDatabase();
		var queryCommand = function(qr){
			var queryText = 'DROP TABLE IF EXISTS CURRENT_SESSION';
			qr.executeSql(queryText);
		};

		db.transaction(queryCommand, error, success);
	}
};

module.exports = BillSql;