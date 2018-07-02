using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Util;
using System.Web.WebPages;
using MrBill_MVC.Models;
using MrBill_MVC.TransactionService;
using MrBill_MVC.UserService;
using TransactionSupplier = MrBill_MVC.UserService.TransactionSupplier;
using UserAuthenticationToken = MrBill_MVC.UserService.AuthenticationToken;
using TransAuthenticationToken = MrBill_MVC.TransactionService.AuthenticationToken;
namespace MrBill_MVC.Controllers
{
	public class TravelController : Controller
	{
		// GET: Travel
		public ActionResult MyBookings()
		{
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			//userName = "StoraHovdingen";
			var userServiceClient = new UserServicePortTypeClient();
			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = new User();
			try
			{
				user = userServiceClient.getUserByUsername(auth, userName);
			}
			catch (Exception)
			{
				FormsAuthentication.SignOut();
				TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
				return RedirectToAction("Index", "Home");
			}

			var userId = user.id.ToString();

			var transactionService = new TransactionServicePortTypeClient();

			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

			var userTransactions = transactionService.getTransactionsForUser(transAuth, userId);

			var transList = userTransactions.ToList();

			string[] monthList = { "januari " + DateTime.Now.Year.ToString(), "februari " + DateTime.Now.Year.ToString(), "mars " + DateTime.Now.Year.ToString(), "april " + DateTime.Now.Year.ToString(), "maj " + DateTime.Now.Year.ToString(), "juni " + DateTime.Now.Year.ToString(), "juli " + DateTime.Now.Year.ToString(), "augusti " + DateTime.Now.Year.ToString(), "september " + DateTime.Now.Year.ToString(), "oktober " + DateTime.Now.Year.ToString(), "november " + DateTime.Now.Year.ToString(), "december " + DateTime.Now.Year.ToString() };

			ViewBag.monthList = monthList;

			if (transList.Count < 1)
			{
				ViewBag.noBook = "Inga bokningar hittade för ditt konto";
			}

			ViewBag.transList = transList;

			return View();
		}

		[HttpPost]
		public ActionResult MyBookings(UserModel model)
		{
			return View(model);


		}

		public ActionResult DeletedBookings()
		{
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			//userName = "StoraHovdingen";
			var userServiceClient = new UserServicePortTypeClient();
			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = new User();
			try
			{
				user = userServiceClient.getUserByUsername(auth, userName);
			}
			catch (Exception)
			{
				FormsAuthentication.SignOut();
				TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
				return RedirectToAction("Index", "Home");
			}

			var userId = user.id.ToString();

			var transactionService = new TransactionServicePortTypeClient();

			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			((BasicHttpBinding)transactionService.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

			var userTransactions = transactionService.getTransactionsForUser(transAuth, userId);

			var transList = userTransactions.ToList();

			string[] monthList = { "januari " + DateTime.Now.Year.ToString(), "februari " + DateTime.Now.Year.ToString(), "mars " + DateTime.Now.Year.ToString(), "april " + DateTime.Now.Year.ToString(), "maj " + DateTime.Now.Year.ToString(), "juni " + DateTime.Now.Year.ToString(), "juli " + DateTime.Now.Year.ToString(), "augusti " + DateTime.Now.Year.ToString(), "september " + DateTime.Now.Year.ToString(), "oktober " + DateTime.Now.Year.ToString(), "november " + DateTime.Now.Year.ToString(), "december " + DateTime.Now.Year.ToString() };

			ViewBag.monthList = monthList;

			if (transList.Count < 1)
			{
				ViewBag.noBook = "Inga bokningar hittade för ditt konto";
			}

			ViewBag.transList = transList;

			return View();
		}


		public ActionResult AddBooking(UserModel model)
		{
			return View(model);
		}



		[HttpPost]
		public ActionResult AddBooking(HttpPostedFileBase file)
		{

			var userService = new UserServicePortTypeClient();
			var userAuth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var transactionService = new TransactionServicePortTypeClient();
			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var transList = new List<TransactionService.Transaction>();

			var transaction = new TransactionService.Transaction();
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			var user = userService.getUserByUsername(userAuth, userName);

		
			var supplier = Request.Form["supplier"];


			var year = DateTime.Now.Year.ToString();
			var month = Request.Form["date1"].AsDateTime().Month;
			var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

			if (file != null && file.ContentLength > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				var path = Path.Combine(Server.MapPath("/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/"), fileName);

				string fileuploadDir = Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier);
				if (!System.IO.Directory.Exists(fileuploadDir))
				{
					System.IO.Directory.CreateDirectory(fileuploadDir);
				}

				file.SaveAs(path);

				transaction.fileLocation = "/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + fileName;

			}
			else
			{
				transaction.fileLocation = "";
			}

			var tempPrice = Request.Form["price"];
			var price = Regex.Replace(tempPrice, @"(\D+)", "");

			transaction.date2Specified = true;
			transaction.date1Specified = true;
			transaction.price = Convert.ToDecimal(price);
			transaction.priceSpecified = true;
			transaction.version = 0;
			transaction.travelerConfirmationEmail = "";
			transaction.travelerMembershipCard = "";
			transaction.creditCardNumber = Request.Form["payment"];
			transaction.destination = Request.Form["country"];
			transaction.addedDate = DateTime.Now;
			transaction.date1 = Request.Form["date1"].AsDateTime();
			transaction.date2 = Request.Form["date2"].AsDateTime();
			transaction.version = 1;
			transaction.country = "SE";
			transaction.transactionOwner = "MrBill";
			transaction.userType = "REGULAR";
			transaction.numberOfUnits = 1;
			transaction.bookingReference = Request.Form["reference"];
			transaction.currency = Request.Form["currency"];
			transaction.tripType = Request.Form["category"];
			transaction.product = Request.Form["product"];
			transaction.creditCardOwner = Request.Form["CcHolder"];
			transaction.travelerName = Request.Form["traveller"];
			transaction.transactionSupplier = supplier;
			transaction.travelerProjectNumber = Request.Form["projectNum"];


			transList.Add(transaction);

			transactionService.saveTransactions(transAuth, user.id.ToString(), transList.ToArray());

			RedirectToAction("MyBookings", "Travel");
			return View();
		}



		public ActionResult EditMyBookings(int id, UserModel model)
		{
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			//userName = "StoraHovdingen7";
			var userServiceClient = new UserServicePortTypeClient();

			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = new User();
			try
			{
				user = userServiceClient.getUserByUsername(auth, userName);
			}
			catch (Exception)
			{

				FormsAuthentication.SignOut();
				TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
				return RedirectToAction("Index", "Home");
			}

			var userId = user.id.ToString();

			var transactionService = new TransactionServicePortTypeClient();

			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var userTransaction = transactionService.getTransaction(transAuth, id.ToString());

			var transList = new List<TransactionService.Transaction>
            {
                userTransaction
            };

			ViewBag.transaction = transList;

			ViewBag.Url = Request.Headers["host"];

			return View(model);

		}

		[HttpPost]
		public ActionResult EditMyBookings(UserModel model, HttpPostedFileBase file)
		{


			var userService = new UserServicePortTypeClient();
			var userAuth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var transactionService = new TransactionServicePortTypeClient();
			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var transList = new List<TransactionService.Transaction>();

			var transaction = new TransactionService.Transaction();
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			var user = userService.getUserByUsername(userAuth, userName);

			var transId = Request.Form["id"];

			var editTransaction = transactionService.getTransaction(transAuth, transId);

			var supplier = Request.Form["supplier"];


			var year = DateTime.Now.Year.ToString();
			var month = Request.Form["date1"].AsDateTime().Month;
			var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

			if (file != null && file.ContentLength > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				var path = Path.Combine(Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/"), fileName);

				string fileuploadDir = Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier);
				if (!System.IO.Directory.Exists(fileuploadDir))
				{
					System.IO.Directory.CreateDirectory(fileuploadDir);
				}

				if (!string.IsNullOrEmpty(editTransaction.fileLocation))
				{
					try
					{
						System.IO.File.Delete(editTransaction.fileLocation);
					}
					catch (Exception)
					{

					}

				}
				file.SaveAs(path);

				editTransaction.fileLocation = "/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + fileName;

			}
			if (string.IsNullOrEmpty(editTransaction.fileLocation))
			{
				editTransaction.fileLocation = "";
			}
			
			var tempPrice = Request.Form["price"];
			var price = Regex.Replace(tempPrice, @"(\D+)", "");
			editTransaction.idSpecified = true;
			editTransaction.date2Specified = true;
			editTransaction.date1Specified = true;
			editTransaction.price = Convert.ToDecimal(price);
			editTransaction.priceSpecified = true;
			editTransaction.creditCardNumber = Request.Form["payment"];
			editTransaction.destination = Request.Form["country"];
			editTransaction.date1 = Request.Form["date1"].AsDateTime();
			editTransaction.date2 = Request.Form["date2"].AsDateTime();
			editTransaction.country = "SE";
			editTransaction.version = 1;
			editTransaction.transactionOwner = "MrBill";
			editTransaction.userType = "REGULAR";
			editTransaction.bookingReference = Request.Form["reference"];
			editTransaction.currency = Request.Form["currency"];
			editTransaction.tripType = Request.Form["category"];
			editTransaction.product = Request.Form["product"];
			editTransaction.creditCardOwner = Request.Form["CcHolder"];
			editTransaction.travelerName = Request.Form["traveller"];
			editTransaction.transactionSupplier = supplier;
			editTransaction.travelerProjectNumber = Request.Form["projectNum"];
			editTransaction.toBeDeleted = false;
			transList.Add(editTransaction);

			transactionService.saveTransactions(transAuth, user.id.ToString(), transList.ToArray());

			ViewBag.transaction = transList;
			ViewBag.Url = Request.Headers["host"];


			RedirectToAction("MyBookings", "Travel");
			return View(model);

		}


		public ActionResult ActivateBooking(int id, UserModel model)
		{
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			//userName = "StoraHovdingen7";
			var userServiceClient = new UserServicePortTypeClient();

			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = new User();
			try
			{
				user = userServiceClient.getUserByUsername(auth, userName);
			}
			catch (Exception)
			{

				FormsAuthentication.SignOut();
				TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
				return RedirectToAction("Index", "Home");
			}

			var userId = user.id.ToString();

			var transactionService = new TransactionServicePortTypeClient();

			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var userTransaction = transactionService.getTransaction(transAuth, id.ToString());

			var transList = new List<TransactionService.Transaction>
            {
                userTransaction
            };

			ViewBag.transaction = transList;

			return View(model);

		}

		[HttpPost]
		public ActionResult ActivateBooking(UserModel model)
		{
			var userService = new UserServicePortTypeClient();
			var userAuth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var transactionService = new TransactionServicePortTypeClient();
			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var transList = new List<TransactionService.Transaction>();

			var transaction = new TransactionService.Transaction();
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			var user = userService.getUserByUsername(userAuth, userName);

			var transId = Request.Form["id"];

			var editTransaction = transactionService.getTransaction(transAuth, transId);

			editTransaction.vat3 = null;

			transList.Add(editTransaction);

			transactionService.saveTransactions(transAuth, user.id.ToString(), transList.ToArray());

			ViewBag.transaction = transList;
			return RedirectToAction("DeletedBookings");

		}

		//REMOVE
		public ActionResult RemoveBooking(int id, UserModel model)
		{
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			//userName = "StoraHovdingen7";
			var userServiceClient = new UserServicePortTypeClient();

			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = new User();
			try
			{
				user = userServiceClient.getUserByUsername(auth, userName);
			}
			catch (Exception)
			{

				FormsAuthentication.SignOut();
				TempData["SignoutMessage"] = "Något gick fel, vänligen försök igen senare";
				return RedirectToAction("Index", "Home");
			}

			var userId = user.id.ToString();

			var transactionService = new TransactionServicePortTypeClient();

			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var userTransaction = transactionService.getTransaction(transAuth, id.ToString());

			var transList = new List<TransactionService.Transaction>
            {
                userTransaction
            };

			ViewBag.transaction = transList;

			return View(model);

		}

		[HttpPost]
		public ActionResult RemoveBooking(UserModel model)
		{
			var userService = new UserServicePortTypeClient();
			var userAuth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var transactionService = new TransactionServicePortTypeClient();
			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var transList = new List<TransactionService.Transaction>();

			var transaction = new TransactionService.Transaction();
			var membershipUser = Membership.GetUser();
			if (membershipUser == null) return RedirectToAction("Index", "Home");

			var userName = membershipUser.UserName;
			var user = userService.getUserByUsername(userAuth, userName);

			var transId = Request.Form["id"];

			var editTransaction = transactionService.getTransaction(transAuth, transId);


			editTransaction.vat3 = 0;

			transList.Add(editTransaction);

			transactionService.saveTransactions(transAuth, user.id.ToString(), transList.ToArray());

			ViewBag.transaction = transList;
			return RedirectToAction("MyBookings");

		}


		public ActionResult TravelAccounts()
		{
			var companyList = new List<SelectListItem>
            {
                new SelectListItem {Text = "VÄLJ", Value = "VÄLJ"},
                new SelectListItem {Text = "SAS", Value = "SAS"},
                new SelectListItem {Text = "HOTELS", Value = "HOTELS"},
                new SelectListItem {Text = "EASYPARK", Value = "EASYPARK"},
                new SelectListItem {Text = "CABONLINE", Value = "CABONLINE"},
                new SelectListItem {Text = "NORWEIGAN", Value = "NORWEIGAN"},

            };
			ViewBag.SupDropDown = companyList;

			return View();

		}

		[HttpPost]
		public ActionResult TravelAccounts(TravelModel model)
		{

			var companyList = new List<SelectListItem>
            {
                new SelectListItem {Text = "VÄLJ", Value = "VÄLJ"},
                new SelectListItem {Text = "SAS", Value = "SAS"},
                new SelectListItem {Text = "HOTELS", Value = "HOTELS"},
                new SelectListItem {Text = "EASYPARK", Value = "EASYPARK"},
                new SelectListItem {Text = "CABONLINE", Value = "CABONLINE"},
                new SelectListItem {Text = "NORWEIGAN", Value = "NORWEIGAN"},
            };

			var selectedItem = Request.Form["SupDropDown"];

			if (selectedItem == "VÄLJ")
			{
				return View(model);
			}

			ViewBag.SupDropDown = companyList;

			var userServiceClient = new UserServicePortTypeClient();

			var userAuth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var transactionService = new TransactionServicePortTypeClient();

			var userName = Membership.GetUser().UserName;

			var user = userServiceClient.getUserByUsername(userAuth, userName);

			var userId = user.id;

			var auth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var transSupList = new List<TransactionService.TransactionSupplier>();
			var transSup = new TransactionService.TransactionSupplier
			{
				name = Request.Form["SupDropDown"].ToString(CultureInfo.InvariantCulture),
				supplierUsername = model.UserName,
				supplierPassword = model.ConfirmPassword,
				version = 1,
				url = "Stored in Scraper"
			};

			transSupList.Add(transSup);
			transactionService.saveTransactionSuppliers(auth, userId.ToString(), transSupList.ToArray());

			ViewBag.Added = Request.Form["SupDropDown"] + " tillagt";
			return View(model);
		}

		public ActionResult Manage()
		{
			var userName = Membership.GetUser().UserName;
			var userServiceClient = new UserServicePortTypeClient();
			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = userServiceClient.getUserByUsername(auth, userName);

			var userSuppList = user.transactionSuppliers.ToList();


			string[] supplierNames = { "SAS", "HOTELS", "EASYPARK", "CABONLINE", "NORWEIGAN" };
			var suppBigList = new List<TransactionSupplier>();
			foreach (var sup in supplierNames)
			{
				var newTransSupp = new TransactionSupplier();
				newTransSupp.name = sup;
				newTransSupp.version = 1;

				string url = "";
				switch (sup)
				{
					case "SAS":
						url = "http://www.sas.se/misc/services/skapa-konto/";
						break;
					case "HOTELS":
						url = "https://ssl-sv.hotels.com/profile/signup.html";
						break;
					case "EASYPARK":
						url = "https://easypark.se/privat/prisinformation/";
						break;
					case "CABONLINE":
						url = "https://boka.cabonline.se/Signup";
						break;
					case "NORWEIGAN":
						url = "https://www.norwegian.com/ssl/se/kundeservice/mitt-norwegian/registrera-profil/";
						break;
				}
				newTransSupp.url = url;
				suppBigList.Add(newTransSupp);
			}


			var dict = suppBigList.ToDictionary(p => p.name);
			foreach (var supp in userSuppList)
			{
				dict[supp.name] = supp;
			}
			var merged = dict.Values.ToList();
			//var suppNameList = user.transactionSuppliers.Select(supp => new SelectListItem { Text = supp.name, Value = supp.name }).ToList();

			ViewBag.userSuppliers = merged;

			return View();
		}

		[HttpPost]
		public ActionResult Manage(TravelModel model)
		{
			var userName = Membership.GetUser().UserName;
			var userServiceClient = new UserServicePortTypeClient();
			var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
			var user = userServiceClient.getUserByUsername(auth, userName);
			var userId = user.id;

			var suppNameList = user.transactionSuppliers.Select(supp => new SelectListItem { Text = supp.name, Value = supp.name }).ToList();

			ViewBag.SupDropDown = suppNameList;

			var transactionServiceClient = new TransactionServicePortTypeClient();
			var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };

			var userTransList = user.transactionSuppliers;
			long? supId = new long();
			var transSupList = new List<TransactionService.TransactionSupplier>();
			var transSup = new TransactionService.TransactionSupplier();
			var userSuppList = user.transactionSuppliers.ToList();


			string[] supplierNames = { "SAS", "HOTELS", "EASYPARK", "CABONLINE", "NORWEIGAN" };
			var suppBigList = new List<TransactionSupplier>();
			foreach (var sup in supplierNames)
			{
				var newTransSupp = new TransactionSupplier();
				newTransSupp.name = sup;
				newTransSupp.version = 1;
				suppBigList.Add(newTransSupp);

			}


			var dict = suppBigList.ToDictionary(p => p.name);
			foreach (var supp in userSuppList)
			{
				dict[supp.name] = supp;
			}
			var merged = dict.Values.ToList();

			foreach (var supplier in merged)
			{
				if (Request.Form["CompanyName"] == supplier.name)
				{
					supId = supplier.id;

					if (string.IsNullOrEmpty(Request.Form["UserName"])) continue;
					transSup.name = Request.Form["CompanyName"];
					transSup.supplierUsername = Request.Form["UserName"];
					transSup.supplierPassword = Request.Form["ConfirmPassword"];
					transSup.id = supId;
					transSup.idSpecified = true;

					string url = "";
					switch (Request.Form["CompanyName"])
					{
						case "SAS":
							url = "http://www.sas.se/misc/services/skapa-konto/";
							break;
						case "HOTELS":
							url = "https://ssl-sv.hotels.com/profile/signup.html";
							break;
						case "EASYPARK":
							url = "https://easypark.se/privat/prisinformation/";
							break;
						case "CABONLINE":
							url = "https://boka.cabonline.se/Signup";
							break;
						case "NORWEIGAN":
							url = "https://www.norwegian.com/ssl/se/kundeservice/mitt-norwegian/registrera-profil/";
							break;
					}

					transSup.url = url;

					ViewBag.changed = transSup.name + " sparad";

				}

				else
				{
					if (string.IsNullOrEmpty(Request.Form["UserName"])) continue;
					transSup.name = Request.Form["CompanyName"];
					transSup.supplierUsername = Request.Form["UserName"];
					transSup.supplierPassword = Request.Form["ConfirmPassword"];

					string url = "";
					switch (Request.Form["CompanyName"])
					{
						case "SAS":
							url = "http://www.sas.se/misc/services/skapa-konto/";
							break;
						case "HOTELS":
							url = "https://ssl-sv.hotels.com/profile/signup.html";
							break;
						case "EASYPARK":
							url = "https://easypark.se/privat/prisinformation/";
							break;
						case "CABONLINE":
							url = "https://boka.cabonline.se/Signup";
							break;
						case "NORWEIGAN":
							url = "https://www.norwegian.com/ssl/se/kundeservice/mitt-norwegian/registrera-profil/";
							break;
					}
					transSup.url = url;
					ViewBag.changed = transSup.name + " sparad";
				}
			}





			transSupList.Add(transSup);
			transactionServiceClient.saveTransactionSuppliers(transAuth, userId.ToString(), transSupList.ToArray());




			ViewBag.userSuppliers = merged;

			return View(model);
		}

	}
}