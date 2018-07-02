using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.ServiceModel.Activation;
using System.Web;
using MrBillServices.DAO;
using MrBillServices.DTO;
using MrBillServices.DTO.Mobile;
using MrBillServices.Entities;
using MrBillServices.State;
using MrBillServices.Utilities;
using WebMatrix.WebData;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MrBillMobileServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MrBillMobileServices.svc or MrBillMobileServices.svc.cs at the Solution Explorer and start debugging.

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MrBillMobileServices : IMrBillMobileServices
    {

        private readonly UserAccesser _userAccesser = new UserAccesser();
        private readonly TransactionAccesser _transactionAccesser = new TransactionAccesser();

        private readonly SupplierAccesser _supplierAccesser = new SupplierAccesser();
        private readonly MrBillUtility _util = new MrBillUtility();
        #region user functions
        public ServiceResponeBase RegisterUser(UserProfile model)
        {
            ServiceResponeBase responeResult = new ServiceResponeBase() { Success = false };
            try
            {

                model.UserRoleID = (int)MrbillState.MrbillUserRole.OrdinaryUser;
                model.Status = 5;
                model.CompanyID = int.Parse(ConfigurationManager.AppSettings["NonCampanyID"]);

                if (_userAccesser.CreateUserProfile(model))
                {
                    WebSecurity.CreateAccount(model.UserName, model.Password);
                    responeResult.Success = true;
                }
                else
                {
                    responeResult.errorCode = -1;
                    responeResult.Message = "Already username";
                }
            }
            catch (Exception exception)
            {
                responeResult.errorCode = -2;
                responeResult.Message = exception.Message;

            }

            return responeResult;
        }
        public LocalUser Login(string username, string password)
        {
            LocalUser localUser = new LocalUser() { Id = -2 };
            try
            {
                if (WebSecurity.Login(username, password))
                {
                    var userProfile = _userAccesser.GetUserProfileByUsername(username);
                    localUser.Id = userProfile.UserID;
                    localUser.UserName = userProfile.UserName;
                    localUser.FirstName = userProfile.FirstName;
                    localUser.LastName = userProfile.LastName;
                }
                else
                {
                    localUser.Id = -1;
                    localUser.FirstName = "Wrong password or username";
                }
                return localUser;

            }
            catch (Exception exception)
            {
                localUser.FirstName = exception.Message;
                return localUser;

            }
        }
        #endregion

        #region transaction functions
        public List<TransactionObject> GetUserTrans(string username)
        {
            try
            {
                List<Transaction> userTransactions = _transactionAccesser.GetTransactionsByUsername(username);

                var list = new List<TransactionObject>();

                foreach (var trans in userTransactions)
                {
                    var obj = new TransactionObject
                    {
                        ExpensDate = "", //Not found for PDB
                        CostCenter = "",//Change to ID, I think we need to do same with project
                        Supplier = trans.SupplierInfo.Name,
                        Reference = trans.BookingRef,
                        Date1 = trans.AddedDate.ToString(),//change to use add date
                        Date2 = trans.AddedDate.ToString(),
                        Destination = trans.Destination,
                        Product = trans.Product,
                        Traveller = trans.Traveller,
                        Price = (decimal)(trans.Price ?? 0),
                        Currency = trans.PriceCurrency,
                        Vat = (decimal)(trans.Vat1 ?? 0),
                        CardHolder = trans.CardHolder,
                        Reciept = trans.ReceiptLink,
                        ExtraInfo = trans.ExtraInfo,
                        Description = trans.Description,                 
                        IsDisabled = ((trans.Status == 1) ? (decimal?)null : trans.Status), //if null show transaction. else hide it!
                        Id = trans.TransactionID,
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

        public ServiceResponeBase UploadTransacitonAndFile(TransactionObject model)
        {
            ServiceResponeBase responeResult = new ServiceResponeBase() { Success = false };
            try
            {
              
                if (!model.Price.HasValue)
                {
                    throw new Exception("cant parse price. Needs to be a 'decimal' value");
                }


                if (!model.Vat.HasValue)
                {
                    throw new Exception("cant parse vat. Needs to be 'int' value");
                }

                UserProfile user = null;
                if(model.UserId.HasValue)
                    user = _userAccesser.GetUserProfileByUserId(model.UserId.Value);
                if (user == null) throw new Exception("UserID not found");

                DateTime date = DateTime.Parse(model.Date1);
                var year = date.Year;
                var month = date.Month;
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

                Transaction transaction = new Transaction();
                transaction.UserID = user.UserID;
                transaction.AddedDate = date;
                transaction.BookingRef = model.Reference;
                transaction.PriceCurrency = model.Currency;
                transaction.ExtraInfo = model.ExtraInfo;
                //transaction.Description = model.ExtraInfo;
                transaction.Country = "SE";
                transaction.Destination = model.Destination;
                 _supplierAccesser.CreateSupplierInfoByName(model.Supplier,"");
                SupplierInfo supplierInfo = _supplierAccesser.GetSupplierInfoByName(model.Supplier);
                transaction.SupplierID = supplierInfo.SupplierID;

                string rootLocation = ConfigurationManager.AppSettings["docPath"];
                string fileLocation = MrBillUtility.UploadRecieptLocaltion(date, user.UserID, supplierInfo.Name);

                string fileName = MrBillUtility.RandomString(5) + "-" + MrBillUtility.RandomString(5);
                transaction.ReceiptLink = "/" + fileLocation + "/" + fileName + ".jpg";
                transaction.Price = (float)(model.Price ?? 0);
               

                transaction.CategoryID = 1;
                transaction.PaymentID = 1;
                transaction.ExportStatus = 1;
                transaction.Status = 1;
                transaction.Vat1 = (float)(model.Vat ?? 0);



                if (!Directory.Exists(rootLocation + "/" +  fileLocation))
                {
                    Directory.CreateDirectory(rootLocation + "/" + fileLocation);
                }


                Byte[] bytes = Convert.FromBase64String(model.Reciept);
                File.WriteAllBytes(rootLocation + "/" + fileLocation + "/" + fileName + ".jpg", bytes);

                _transactionAccesser.CreateTransaction(transaction);
                responeResult.Success = true;
                responeResult.Message = "File Uploaded successfully and information stored in DB";
            }
            catch (Exception exception)
            {
                responeResult.Message = exception.Message;
               
            }
             return responeResult;
        }

        public ServiceResponeBase EditTransaction(TransactionObject model)
        {
            ServiceResponeBase responeResult = new ServiceResponeBase() { Success = false };
            try
            {
                if (model == null) throw new Exception("The transaction is null");

                if (!model.Price.HasValue) throw new Exception("Price cannot be null");

                if (!model.Vat.HasValue) throw new Exception("VAT cannot be null");


                var transaction = _transactionAccesser.GetTransactionsByTransactionId( (int)model.Id.Value);

                transaction.PriceCurrency = model.Currency;
                transaction.AddedDate = DateTime.Parse(model.Date1);

                transaction.ExtraInfo = model.ExtraInfo;
                //transaction.Description = model.ExtraInfo;
                transaction.Destination = model.Destination;

                transaction.Price = (float)(model.Price ?? 0);
                SupplierInfo supplierInfo = _supplierAccesser.GetSupplierInfoByName(model.Supplier);
                if (supplierInfo == null)
                {
                    _supplierAccesser.CreateSupplierInfoByName(model.Supplier,"");
                    supplierInfo = _supplierAccesser.GetSupplierInfoByName(model.Supplier);
                }
                transaction.SupplierID = supplierInfo.SupplierID;

                transaction.Vat1 = (float)(model.Vat ?? 0);
                _transactionAccesser.UpdateTransaction(transaction,"");
                responeResult.Success = true;
                responeResult.Message = "edit sucessfull, Id:" + transaction.TransactionID;
            }
            catch (Exception exception)
            {
                responeResult.Message = exception.Message;
            }
            return responeResult;
        }
        #endregion

        #region create Pdf

        public bool SendEmailReport(string username, List<string> emailList, int month, int year, string languageCode)
        {
            return _util.SendEmailReport(username, emailList, month, year, languageCode, 1);
        }

        #endregion

        #region Setting

        public string AddOrUpdateSetting( string username, SettingDTO setting)
        {
            try
            {
                _userAccesser.AddSettingForUser(MrBillUtility.ConvertToEntitySetting(setting));

                return "1";
            }
            catch (Exception exception)
            {
                return exception.Message + "|||" + exception.StackTrace;
            }
        }

        public SettingDTO GetSetingById( string username, int userId)
        {
            try
            {
                return MrBillUtility.ConvertToSettingDTO(_userAccesser.GetSettingForUser(userId));
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion
    }
}
