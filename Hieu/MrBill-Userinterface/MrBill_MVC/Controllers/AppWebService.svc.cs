using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using MrBill_MVC.MrBillTransactionServices;
using MrBill_MVC.TransactionService;
using MrBill_MVC.UserService;
using MrBill_MVC.Utilities;
using WebMatrix.WebData;
using Transaction = MrBill_MVC.TransactionService.Transaction;
using UserAuthenticationToken = MrBill_MVC.UserService.AuthenticationToken;
using TransAuthenticationToken = MrBill_MVC.TransactionService.AuthenticationToken;

namespace MrBill_MVC.Controllers
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AppWebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AppWebService.svc or AppWebService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(MaxItemsInObjectGraph = int.MaxValue, IncludeExceptionDetailInFaults = true)]
    public class AppWebService : IAppWebService
    {

        private UserServicePortTypeClient UserServiceMrbill { get; set; }
        private TransactionServicePortTypeClient TransactionServiceMrbill { get; set; }

        private MrBillUserServices.MrBillUserServicesClient _MrbillUserService { get; set; }
        private MrBillTransactionServices.MrBillTransactionServicesClient _MrbillTransactionService { get; set; }

        public MrBillUserServices.MrBillUserServicesClient MrbillUserServiceClient
        {
            get
            {
                if (_MrbillUserService != null) return _MrbillUserService;

                _MrbillUserService = new MrBillUserServices.MrBillUserServicesClient();

                return _MrbillUserService;
            }
        }

        public MrBillTransactionServices.MrBillTransactionServicesClient MrbillTransactionServiceClient
        {
            get
            {
                if (_MrbillTransactionService != null) return _MrbillTransactionService;

                _MrbillTransactionService = new MrBillTransactionServices.MrBillTransactionServicesClient();

                return _MrbillTransactionService;
            }
        }

        public UserServicePortTypeClient UserServiceMrbillClient
        {
            get
            {
                if (UserServiceMrbill != null) return UserServiceMrbill;

                UserServiceMrbill = new UserServicePortTypeClient();
                ((BasicHttpBinding)UserServiceMrbill.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

                return UserServiceMrbill;
            }
        }

        public TransactionServicePortTypeClient TransactionServiceMrbillClient
        {
            get
            {
                if (TransactionServiceMrbill != null) return TransactionServiceMrbill;

                TransactionServiceMrbill = new TransactionServicePortTypeClient();
                ((BasicHttpBinding)TransactionServiceMrbill.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;

                return TransactionServiceMrbill;
            }
        }

        public UserAuthenticationToken GetUserTokenMrbill
        {
            get
            {
                return new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            }
        }

        public TransAuthenticationToken GetTransactionTokenMrbill
        {
            get
            {
                return new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            }
        }

        public Test DoWork()
        {
            var test = new Test
            {
                Name = "Martin",
                Number = "0707502979"
            };

            return test;
        }

        #region repose type
        public class ResponeBase
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
        #endregion

        public class Test
        {
            public string Name { get; set; }
            public string Number { get; set; }
        }

        public LocalUser Login(string username, string password)
        {
            try
            {
                if (WebSecurity.Login(username, password))
                {
                    var user = MrbillUserServiceClient.GetUserByUsername(MrBillUtility.GenerateAuthenticationToken(username), username);
                var localUser = new LocalUser
                {
                    Id = user.UserId,
                    UserName = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                return localUser;
                }
                return null;
            }
            catch (Exception exception)
            {
               //return new LocalUser() { FirstName = exception.Message };
                return null;
            }

        }

        public class LocalUser
        {
            public long? Id { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        //public List<TransactionObject> GetUserTrans(string username)
        //{
        //    var user = UserServiceMrbillClient.getUserByUsername(GetUserTokenMrbill, username);

        //    var userTransactions = TransactionServiceMrbillClient.getTransactionsForUser(GetTransactionTokenMrbill, user.id.ToString());

        //    var list = new List<TransactionObject>();

        //    foreach (var trans in userTransactions)
        //    {
        //        var obj = new TransactionObject
        //        {
        //            ExpensDate = trans.expenseDate.ToString(), //Sort on this date
        //            CostCenter = trans.travelerCostCenter,
        //            Supplier = trans.transactionSupplier,
        //            Reference = trans.bookingReference,
        //            Date1 = trans.date1.ToString(),
        //            Date2 = trans.date2.ToString(),
        //            Destination = trans.destination,
        //            Product = trans.product,
        //            Traveller = trans.travelerName,
        //            Price = trans.price,
        //            Currency = trans.currency,
        //            Vat = trans.vat1,
        //            CardHolder = trans.creditCardOwner,
        //            Reciept = trans.fileLocation,
        //            ExtraInfo = trans.description,
        //            IsDisabled = trans.vat3, //if null show transaction. else hide it!
        //            Id = trans.id,
        //        };

        //        list.Add(obj);
        //    }


        //    return list;
        //}

        public List<TransactionObject> GetUserTrans(string username)
        {
            try
            {
                var userID = MrbillUserServiceClient.GetUserIDByUsername(MrBillUtility.GenerateAuthenticationToken(username), username);

                var userTransactions = MrbillTransactionServiceClient.GetTransactionsForUser(MrBillUtility.GenerateAuthenticationToken(username), username, userID);

                var list = new List<TransactionObject>();

                foreach (var trans in userTransactions)
                {
                    var obj = new TransactionObject
                    {
                        ExpensDate = "", //Not found for PDB
                        CostCenter = "",//Change to ID, I think we need to do same with project
                        Supplier = trans.SupplierInfoes.SupplierName,
                        Reference = trans.BookingRef,
                        Date1 = trans.AddedDate.ToString(),//change to use add date
                        Date2 = trans.AddedDate.ToString(),
                        Destination = trans.Destination,
                        Product = trans.Product,
                        Traveller = trans.Traveller,
                        Price = (decimal)trans.Price,
                        Currency = trans.PriceCurrency,
                        Vat = (decimal)(trans.Vat1 ?? 0),
                        CardHolder = trans.CardHolder,
                        Reciept = trans.ReceiptLink,
                        ExtraInfo = trans.ExtraInfo,
                        IsDisabled = ((trans.Status == 1) ? (decimal?)null : trans.Status), //if null show transaction. else hide it!
                        Id = trans.TransactionId,
                    };

                    list.Add(obj);
                }


                return list;
            }
            catch (Exception exception)
            {
                List<TransactionObject> erros = new List<TransactionObject>();
                erros.Add(new TransactionObject() { ExtraInfo = exception.Message });
                return erros;
            }

        }
        public class TransactionObject
        {
            public string ExpensDate { get; set; }
            public string DateAdded { get; set; }
            public string Date1 { get; set; }
            public string Date2 { get; set; }
            public string Supplier { get; set; }
            public string Reference { get; set; }
            public string Destination { get; set; }
            public string Product { get; set; }
            public string Traveller { get; set; }
            public string PaymentMethod { get; set; }
            public string CardHolder { get; set; }
            public decimal? Price { get; set; }
            public decimal? Vat { get; set; }
            public string CostCenter { get; set; }
            public string Reciept { get; set; }
            public string Currency { get; set; }
            public string ExtraInfo { get; set; }
            public long? Id { get; set; }
            public decimal? IsDisabled { get; set; }

        }

        public string UploadFile(Stream fileContents)
        {
            if (WebOperationContext.Current == null)
            {
                return "Need header parameters";
            }
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            var headers = request.Headers;
            var username = headers["username"];
            var reference = headers["reference"];
            var tempDate = headers["date1"];
            var supplier = headers["supplier"];
            var location = headers["location"];
            var tempPrice = headers["price"];
            var tempVat = headers["vat"];
            var currency = headers["currency"];
            var extraInfo = headers["extraInfo"];

            decimal price = 0;
            double vat = 0;
            DateTime date1;
            try
            {
                price = decimal.Parse(tempPrice);
            }
            catch (Exception)
            {
                return "cant parse price. Needs to be a 'decimal' value";
            }
            try
            {
                vat = double.Parse(tempVat);
            }
            catch (Exception)
            {
                return "cant parse vat. Needs to be 'int' value";
            }

            try
            {
                date1 = DateTime.Parse(tempDate);
            }
            catch (Exception)
            {
                return "Date is wrong format. Should be ex. YYYY-MM-DD";
            }
            var userId = "";
            try
            {
                var userServiceClient = new UserServicePortTypeClient();
                var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
                ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
                var user = userServiceClient.getUserByUsername(auth, username);
                userId = user.id.ToString();
            }
            catch (Exception)
            {
                return "Could not find user " + username;
            }

            var date = Convert.ToDateTime(date1);
            var year = date.Year;
            var month = date.Month;
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            var transactionService = new TransactionServicePortTypeClient();
            var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
            var transaction = new Transaction();
            transaction.priceSpecified = true;
            transaction.date1Specified = true;
            transaction.version = 0;
            transaction.addedDate = DateTime.Now;
            transaction.transactionSupplier = supplier;
            transaction.price = price;
            transaction.destination = location;
            transaction.date1Specified = true;
            transaction.date2Specified = true;
            transaction.date1 = date1;
            transaction.date2 = date1;
            transaction.vat1Specified = true;
            transaction.vat1 = Convert.ToDecimal(Math.Round(vat, 0));
            transaction.bookingReference = reference;
            transaction.currency = currency;
            transaction.country = "SE";
            transaction.transactionOwner = "MrBill";
            transaction.userType = "REGULAR";
            transaction.fileLocation = ("/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + userId + "/" + reference + ".jpg");
            var transList = new List<Transaction> { transaction };
            try
            {
                var fileuploadDir = HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + userId);

                if (!Directory.Exists(fileuploadDir))
                {
                    Directory.CreateDirectory(fileuploadDir);

                }

                var buffer = new byte[1000000000];
                var fileSize = fileContents.Read(buffer, 0, 1000000000);
                var f = new FileStream(HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + supplier + "/" + userId + "/" + reference + ".jpg"), FileMode.OpenOrCreate);
                f.Write(buffer, 0, fileSize);
                f.Close();
                fileContents.Close();
            }

            catch (Exception)
            {

                return "error uploading file to server and no information uploaded";
            }
            transactionService.saveTransactions(transAuth, userId, transList.ToArray());
            return "File Uploaded successfully and information stored in DB";
        }

        #region new upload file function
        public ResponeBase UploadFileAndInfo(TransactionObject model)
        {
            try
            {
                DateTime date1;

                if (!model.Price.HasValue)
                {
                    throw new Exception("cant parse price. Needs to be a 'decimal' value");
                }


                if (!model.Vat.HasValue)
                {
                    throw new Exception("cant parse vat. Needs to be 'int' value");
                }

                date1 = DateTime.Parse(model.Date1);


                var user = MrbillUserServiceClient.GetUserByUsername(MrBillUtility.GenerateAuthenticationToken(model.Traveller), model.Traveller);



                var date = Convert.ToDateTime(date1);
                var year = date.Year;
                var month = date.Month;
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

                var transaction = new TransactionDTO();
                transaction.UserID = user.UserId;
                transaction.AddedDate = date1;
                transaction.BookingRef = model.Reference;
                transaction.PriceCurrency = model.Currency;
                transaction.Country = "SE";
                transaction.Destination = model.Destination;
                string fileName = MrBillUtility.RandomString(5) + "-" + MrBillUtility.RandomString(5);
                transaction.ReceiptLink = ("/uploads/reciept/" + year + "/" + monthName + "/" + model.Supplier + "/" +
                             user.UserId + "/" + fileName + ".jpg");
                transaction.Price = (float)(model.Price ?? 0);
                transaction.SupplierID = 0;
                //var suppliers = MrbillUserServiceClient.GetAllSupplierInfoList(MrBillUtility.GenerateAuthenticationToken(model.Traveller), model.Traveller, true);
                //foreach (var item in suppliers)
                //{
                //    if (item.SupplierName == model.Supplier) transaction.SupplierID = item.SupplierId;

                //}
                //if (transaction.SupplierID == 0)
                //{
                //    //check for other and set supplierID = other
                //    transaction.SupplierID = 6;
                //}
                var authToken = MrBillUtility.GenerateAuthenticationToken(model.Traveller);
                var supplierName = model.Supplier;
                var createSupplierStatus = MrbillTransactionServiceClient.CreateSupplier(authToken, model.Traveller, supplierName);
                var supplierId = MrbillTransactionServiceClient.GetSupplierIDBySupplierName(supplierName);
                transaction.SupplierID = supplierId;

                transaction.CategoryID = 1;
                transaction.PaymentID = 1;
                transaction.ExportStatus = 1;
                transaction.Status = 1;
                //transaction.transactionOwner = model.Traveller; not found
                //transaction.userType = "REGULAR";
                transaction.Vat1 = (float)(model.Vat ?? 0);



                var fileuploadDir =
                    HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" +
                                                       model.Supplier + "/" + user.UserId);

                if (!Directory.Exists(fileuploadDir))
                {
                    Directory.CreateDirectory(fileuploadDir);
                }

                var fileuploadpath =
                    HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" +
                                                       model.Supplier + "/" + user.UserId + "/" + fileName +
                                                       ".jpg");

                Byte[] bytes = Convert.FromBase64String(model.Reciept);
                File.WriteAllBytes(fileuploadpath, bytes);

                MrbillTransactionServiceClient.CreateNewTransaction(MrBillUtility.GenerateAuthenticationToken(model.Traveller),
                    model.Traveller, transaction);
                return new ResponeBase()
                {
                    Message = "File Uploaded successfully and information stored in DB",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ResponeBase()
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }
        //public ResponeBase UploadFileAndInfo(TransactionObject model)
        //{
        //    try
        //    {
        //        DateTime date1;

        //        if (!model.Price.HasValue)
        //        {
        //            throw new Exception("cant parse price. Needs to be a 'decimal' value");
        //        }


        //        if (!model.Vat.HasValue)
        //        {
        //            throw new Exception("cant parse vat. Needs to be 'int' value");
        //        }

        //        date1 = DateTime.Parse(model.Date1);


        //        var user = UserServiceMrbillClient.getUserByUsername(GetUserTokenMrbill, model.Traveller);



        //        var date = Convert.ToDateTime(date1);
        //        var year = date.Year;
        //        var month = date.Month;
        //        var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

        //        var transaction = new Transaction();

        //        transaction.addedDate = DateTime.Now;
        //        transaction.bookingReference = model.Reference;
        //        transaction.currency = model.Currency;
        //        transaction.country = "SE";
        //        transaction.date1 = date1;
        //        transaction.date1Specified = true;
        //        transaction.date2 = date1;
        //        transaction.date2Specified = true;
        //        transaction.destination = model.Destination;
        //        transaction.fileLocation = ("/uploads/reciept/" + year + "/" + monthName + "/" + model.Supplier + "/" +
        //                     user.id + "/" + model.Reference + ".jpg");
        //        transaction.price = model.Price;
        //        transaction.priceSpecified = true;
        //        transaction.transactionSupplier = model.Supplier;
        //        transaction.transactionOwner = model.Traveller;
        //        transaction.userType = "REGULAR";
        //        transaction.vat1 = model.Vat;
        //        transaction.vat1Specified = true;
        //        transaction.version = 0;

        //        var transList = new List<Transaction> { transaction };

        //        var fileuploadDir =
        //            HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" +
        //                                               model.Supplier + "/" + user.id);

        //        if (!Directory.Exists(fileuploadDir))
        //        {
        //            Directory.CreateDirectory(fileuploadDir);
        //        }

        //        var fileuploadpath =
        //            HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" +
        //                                               model.Supplier + "/" + user.id + "/" + model.Reference +
        //                                               ".jpg");

        //        Byte[] bytes = Convert.FromBase64String(model.Reciept);
        //        File.WriteAllBytes(fileuploadpath, bytes);

        //        TransactionServiceMrbillClient.saveTransactions(GetTransactionTokenMrbill, user.id.ToString(), transList.ToArray());


        //        return new ResponeBase()
        //        {
        //            Message = "File Uploaded successfully and information stored in DB",
        //            Success = true
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        return new ResponeBase()
        //        {
        //            Success = false,
        //            Message = e.Message
        //        };
        //    }
        //}

        //public ResponeBase UploadFileAndInfo(TransactionObject model)
        //{
        //    var response = new ResponeBase();
        //    response.Success = false;

        //    decimal price = 0;
        //    double vat = 0;
        //    DateTime date1;

        //    if (model.Price.HasValue)
        //    {
        //        price = model.Price.Value;
        //    }
        //    else
        //    {
        //        response.Message = "cant parse price. Needs to be a 'decimal' value";
        //        return response;
        //    }

        //    if (model.Vat.HasValue)
        //    {
        //        vat = (double)model.Vat.Value;
        //    }
        //    else
        //    {
        //        response.Message = "cant parse vat. Needs to be 'int' value";
        //        return response;
        //    }

        //    try
        //    {
        //        date1 = DateTime.Parse(model.Date1);

        //    }
        //    catch (Exception)
        //    {
        //        response.Message = "Date is wrong format. Should be ex. YYYY-MM-DD";
        //        return response;
        //    }

        //    var userId = "";
        //    var username = model.Traveller;
        //    try
        //    {
        //        var userServiceClient = new UserServicePortTypeClient();
        //        var auth = new UserAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
        //        ((BasicHttpBinding)userServiceClient.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
        //        var user = userServiceClient.getUserByUsername(auth, username);
        //        userId = user.id.ToString();
        //    }
        //    catch (Exception)
        //    {
        //        response.Message = "Could not find user " + username;
        //        return response;
        //    }

        //    var date = Convert.ToDateTime(date1);
        //    var year = date.Year;
        //    var month = date.Month;
        //    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

        //    var transactionService = new TransactionServicePortTypeClient();
        //    var transAuth = new TransAuthenticationToken { username = "MrBprofileWS", password = "Go4AHik3@n8!" };
        //    var transaction = new Transaction();
        //    transaction.priceSpecified = true;
        //    transaction.date1Specified = true;
        //    transaction.version = 0;
        //    transaction.addedDate = DateTime.Now;
        //    transaction.transactionSupplier = model.Supplier;
        //    transaction.price = price;
        //    transaction.destination = model.Destination;
        //    transaction.date1Specified = true;
        //    transaction.date2Specified = true;
        //    transaction.date1 = date1;
        //    transaction.date2 = date1;
        //    transaction.vat1Specified = true;
        //    transaction.vat1 = Convert.ToDecimal(Math.Round(vat, 0));
        //    transaction.bookingReference = model.Reference;
        //    transaction.currency = model.Currency;
        //    transaction.country = "SE";
        //    transaction.transactionOwner = "MrBill";
        //    transaction.userType = "REGULAR";
        //    transaction.fileLocation = ("/uploads/reciept/" + year + "/" + monthName + "/" + model.Supplier + "/" + userId + "/" + model.Reference + ".jpg");
        //    var transList = new List<Transaction> { transaction };
        //    try
        //    {
        //        var fileuploadDir = HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + model.Supplier + "/" + userId);

        //        if (!Directory.Exists(fileuploadDir))
        //        {
        //            Directory.CreateDirectory(fileuploadDir);
        //        }

        //        var fileuploadpath = HttpContext.Current.Server.MapPath("~/uploads/reciept/" + year + "/" + monthName + "/" + model.Supplier + "/" + userId + "/" + model.Reference + ".jpg");

        //        Byte[] bytes = Convert.FromBase64String(model.Reciept);
        //        File.WriteAllBytes(fileuploadpath, bytes);
        //    }
        //    catch (Exception)
        //    {
        //        response.Message = "error uploading file to server and no information uploaded";
        //        return response;
        //    }
        //    transactionService.saveTransactions(transAuth, userId, transList.ToArray());

        //    response.Message = "File Uploaded successfully and information stored in DB";
        //    response.Success = true;
        //    return response;
        //}

        #endregion

        #region editTransaction
        public ResponeBase EditTransaction(TransactionObject model)
        {
            try
            {
                if (model == null) throw new Exception("The transaction is null");

                if (!model.Price.HasValue) throw new Exception("Price cannot be null");

                if (!model.Vat.HasValue) throw new Exception("VAT cannot be null");


                var transaction = MrbillTransactionServiceClient.GetTransactionById(MrBillUtility.GenerateAuthenticationToken(model.Traveller), model.Traveller, (int)model.Id.Value);

                transaction.BookingRef = model.Reference;
                transaction.PriceCurrency = model.Currency;
                transaction.CardHolder = model.CardHolder;
                transaction.AddedDate = DateTime.Parse(model.Date1);
                if (!string.IsNullOrEmpty(model.Date2))
                {
                   
                }
                transaction.ExtraInfo = model.ExtraInfo;
                transaction.Destination = model.Destination;
                
                transaction.ReceiptLink = model.Reciept;
                transaction.TransactionId = (int)model.Id.Value;
                transaction.Price = (float)(model.Price ?? 0);
                transaction.Product = model.Product;
                //transaction.CostCenter = model.CostCenter;
                //var suppliers = MrbillUserServiceClient.GetAllSupplierInfoList(MrBillUtility.GenerateAuthenticationToken(model.Traveller), model.Traveller, true);
                //foreach (var item in suppliers)
                //{
                //    if (item.SupplierName == model.Supplier) transaction.SupplierID = item.SupplierId;

                //}
                //transaction.SupplierID = 1;
                transaction.Traveller = model.Traveller;
                var authToken = MrBillUtility.GenerateAuthenticationToken(model.Traveller);
                var supplierName = model.Supplier;
                var createSupplierStatus = MrbillTransactionServiceClient.CreateSupplier(authToken, model.Traveller, supplierName);
                var supplierId = MrbillTransactionServiceClient.GetSupplierIDBySupplierName(supplierName);
                transaction.SupplierID = supplierId;
                //if (transaction.SupplierID == 0)
                //{
                //    //check for other and set supplierID = other
                //    transaction.SupplierID = 6;
                //}
                transaction.Vat1 = (float)(model.Vat ?? 0);
                if (model.IsDisabled == null)
                {
                    transaction.Status = 1;
                }
                else
                {
                    transaction.Status = 0;
                }

                transaction.CategoryID = 1;
                transaction.PaymentID = 1;
                transaction.ExportStatus = 1;
                transaction.Status = 1;
                MrbillTransactionServiceClient.UpdateTransaction(MrBillUtility.GenerateAuthenticationToken(model.Traveller), model.Traveller, transaction,"");

                return new ResponeBase() { Success = true, Message = "edit sucessfull, Id:" + model.Id };
            }
            catch (Exception e)
            {
                return new ResponeBase()
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }
        //public ResponeBase EditTransaction(TransactionObject model)
        //{
        //    try
        //    {
        //        if (model == null) throw new Exception("The transaction is null");

        //        if (!model.Price.HasValue) throw new Exception("Price cannot be null");

        //        if (!model.Vat.HasValue) throw new Exception("VAT cannot be null");

        //        var user = UserServiceMrbillClient.getUserByUsername(GetUserTokenMrbill, model.Traveller);



        //        transaction.bookingReference = model.Reference;
        //        transaction.currency = model.Currency;
        //        transaction.creditCardOwner = model.CardHolder;
        //        transaction.date1 = DateTime.Parse(model.Date1);
        //        transaction.date1Specified = true;
        //        if (!string.IsNullOrEmpty(model.Date2))
        //        {
        //            transaction.date2 = DateTime.Parse(model.Date2);
        //            transaction.date2Specified = true;
        //        }
        //        transaction.description = model.ExtraInfo;
        //        transaction.destination = model.Destination;
        //        if (!string.IsNullOrEmpty(model.ExpensDate))
        //        {
        //            transaction.expenseDateSpecified = true;
        //            transaction.expenseDate = DateTime.Parse(model.ExpensDate);
        //        }
        //        transaction.fileLocation = model.Reciept;
        //        transaction.id = model.Id;
        //        transaction.idSpecified = true;
        //        transaction.price = model.Price;
        //        transaction.priceSpecified = true;
        //        transaction.product = model.Product;
        //        transaction.travelerCostCenter = model.CostCenter;
        //        transaction.travelerName = model.Traveller;
        //        transaction.transactionOwner = model.Traveller;
        //        transaction.transactionSupplier = model.Supplier;
        //        transaction.vat1 = model.Vat;
        //        transaction.vat1Specified = true;
        //        transaction.vat3 = model.IsDisabled;
        //        transaction.vat3Specified = true;

        //        var transList = new List<Transaction> { transaction };

        //        TransactionServiceMrbillClient.saveTransactions(GetTransactionTokenMrbill, user.id.ToString(), transList.ToArray());

        //        return new ResponeBase() { Success = true, Message = "edit sucessfull, Id:" + model.Id };
        //    }
        //    catch (Exception e)
        //    {
        //        return new ResponeBase()
        //        {
        //            Success = false,
        //            Message = e.Message
        //        };
        //    }
        //}
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public TransactionObject GetTransactionById(string username, string transactionId)
        {
            try
            {
                var trans = TransactionServiceMrbillClient.getTransaction(GetTransactionTokenMrbill, transactionId);
                return new TransactionObject()
                {
                    ExpensDate = trans.expenseDate.ToString(), //Sort on this date
                    CostCenter = trans.travelerCostCenter,
                    Supplier = trans.transactionSupplier,
                    Reference = trans.bookingReference,
                    Date1 = trans.date1.ToString(),
                    Date2 = trans.date2.ToString(),
                    Destination = trans.destination,
                    Product = trans.product,
                    Traveller = trans.travelerName,
                    Price = trans.price,
                    Currency = trans.currency,
                    Vat = trans.vat1,
                    CardHolder = trans.creditCardOwner,
                    Reciept = trans.fileLocation,
                    ExtraInfo = trans.description,
                    IsDisabled = trans.vat3, //if null show transaction. else hide it!
                    Id = trans.id,
                };

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}