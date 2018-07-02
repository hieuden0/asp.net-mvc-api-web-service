using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MrBillServices.DAO;
using MrBillServices.DTO;
using MrBillServices.Entities;
using MrBillServices.Utilities;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MrBillServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MrBillServices.svc or MrBillServices.svc.cs at the Solution Explorer and start debugging.
    public class MrBillServices : IMrBillServices
    {
        private readonly UserAccesser _userAccesser = new UserAccesser();
        private readonly TransactionAccesser _transactionAccesser = new TransactionAccesser();

        private readonly SupplierAccesser _supplierAccesser = new SupplierAccesser();
        private readonly MrBillUtility _util = new MrBillUtility();
        private readonly LogAccesser _logAccesser = new LogAccesser();
        public MrBillDTO[] GetMrBillUsers(string token, string username)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
               
                    List<UserProfile> listProfiles = _userAccesser.GetAllUserProfileBySupplier();
                    List<MrBillDTO> resultList = new List<MrBillDTO>();

                    foreach (var userProf in listProfiles)
                    {
                        MrBillDTO mrBillresult = new MrBillDTO();
                        mrBillresult.UserId = userProf.UserID;
                        mrBillresult.UserName = userProf.UserName;
                        mrBillresult.UserSupplier = new List<UserSupplierdata>();
                        foreach (var usersup in userProf.UserSupplierInfoes)
                        {
                            UserSupplierdata usersupItem = new UserSupplierdata();
                            usersupItem.SupplierId = usersup.SupplierID;
                            usersupItem.SupplierUsername = usersup.Username;
                            usersupItem.SupplierPassword = usersup.Password;
                             mrBillresult.UserSupplier.Add(usersupItem);
                        }
                        resultList.Add(mrBillresult);
                    }
                    return resultList.ToArray();
                

            }
            catch (Exception exception)
            {
                List<MrBillDTO> resultList = new List<MrBillDTO>();
                resultList.Add(new MrBillDTO() {UserName = exception.Message +"|||" + exception.StackTrace});
                return resultList.ToArray();
            }
        }

        public SupplierInfoDTO[] GetSupplierByListId(string token, string username, int[] ids)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {

                List<SupplierInfo> resuilts = _supplierAccesser.GetSupplierInfosByListId(ids);
                return resuilts.Select(MrBillUtility.ConverToSupplierInfoDTO).ToArray();


            }
            catch (Exception exception)
            {
                List<SupplierInfoDTO> resultList = new List<SupplierInfoDTO>();
                resultList.Add(new SupplierInfoDTO() { SupplierName = exception.Message,Url = exception.StackTrace });
                return resultList.ToArray();
            }
        }

        public ContactInfoDTO[] GetMrBillUsersWithoutSuppliers(string token, string username)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    UserProfile[] listProfiles = MrbillDbEntities.UserProfiles.ToArray();
                    List<ContactInfoDTO> resultList = new List<ContactInfoDTO>();
                    foreach (var userProf in listProfiles)
                    {
                        resultList.Add(MrBillUtility.GetContactByUserPF(userProf));
                    }
                    return resultList.ToArray();
                }

            }
            catch (Exception exception)
            {
                List<ContactInfoDTO> resultList = new List<ContactInfoDTO>();
                resultList.Add(new ContactInfoDTO() { FirstName = exception.Message });
                return resultList.ToArray();
            }


        }

        public int testConnection()
        {
            
            return 1000;
        }

        #region create Pdf

        public bool SendEmailReport(string username, List<string> emailList, int month, int year, int SortType)
        {
            return _util.SendEmailReport(username, emailList, month, year, "sv", SortType);
        }

        public string pdfReview(string username, int month, int year, int SortType)
        {
            var result = _util.pdfReview(username, month, year, SortType);
            return result;
        }

        public byte[] DownloadReport(string username, List<string> emailList, int month, int year, int SortType)
        {
            return _util.DownloadReport(username, emailList, month, year, SortType );
        }

        public string WriteScraperLog(int? type, int? userId, int? supplierId, string message, DateTime time)
        {
            try
            {
                ScraperLog newLog = new ScraperLog()
                {
                    Type = type,
                    UserID = userId,
                    SupplierID = supplierId,
                    Message = message,
                    LogTime = time
                };
                _logAccesser.WriteScraperLog(newLog);
                return "";
            }
            catch (Exception exception)
            {
                return exception.Message + "|||" + exception.StackTrace  ;
            }

        }

        #endregion
    }
}
