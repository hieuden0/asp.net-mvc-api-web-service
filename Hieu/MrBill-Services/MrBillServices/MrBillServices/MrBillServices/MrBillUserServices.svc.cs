using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MrBillServices.DTO;
using MrBillServices.Entities;
using MrBillServices.Utilities;
using MrBillServices.DAO;
//using OpenPop.Pop3;
//using OpenPop.Mime;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MrBillUserServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MrBillUserServices.svc or MrBillUserServices.svc.cs at the Solution Explorer and start debugging.
    public class MrBillUserServices : IMrBillUserServices
    {
        UserAccesser _userAccesser = new UserAccesser();
        ManagerAccesser _managerAccesser = new ManagerAccesser();

        public int CreateUser(UserDTO user)
        {
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var checkExistUser = MrbillDbEntities.UserProfiles.FirstOrDefault(u => u.UserName == user.Username);

                    if (checkExistUser == null)
                    {
                        MrbillDbEntities.UserProfiles.Add(MrBillUtility.ConvertToEntityUserProfile(user));
                        MrbillDbEntities.SaveChanges();
                        return 1;
                    }
                }
                return 0;
            }
            catch (Exception exception)
            {
                return 2;
            }

        }

        public string AddOrUpdateSetting(string token, string username, SettingDTO setting)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return "0";
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

        public SettingDTO GetSetingById(string token, string username, int userId)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                return MrBillUtility.ConvertToSettingDTO(_userAccesser.GetSettingForUser(userId));
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public int UpdateUser(string token, UserDTO user)
        {
            if (!MrBillUtility.CheckValidToken(token, user.Username)) return 0;
            try
            {
                UserProfile updateUserProfile = MrBillUtility.ConvertToEntityUserProfile(user);
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    MrbillDbEntities.UserProfiles.Attach(updateUserProfile);
                    MrbillDbEntities.Entry(updateUserProfile).State = EntityState.Modified;
                    MrbillDbEntities.SaveChanges();
                }

                return 1;
            }
            catch (Exception exception)
            {
                return 2;
            }

        }

        public UserDTO GetUserByUsername(string token, string username)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var userProfile = MrbillDbEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
                    if (userProfile != null)
                    {
                        //Get User Profile
                        UserDTO result = MrBillUtility.ConvertToUserDTO(userProfile);
                        result.UserSupplierInfoes = MrBillUtility.GetUserSupplierInfoes(result.UserId, MrbillDbEntities);
                        return result;
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                return new UserDTO() { FirstName = exception.Message };
            }

        }

        public int GetUserIDByUsername(string token, string username)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return 0;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var userProfile = MrbillDbEntities.UserProfiles.FirstOrDefault(up => up.UserName == username);
                    if (userProfile == null)
                        return 0;
                    return userProfile.UserID;
                }

            }
            catch (Exception exception)
            {
                return -1;
            }

        }

        public List<SupplierInfoDTO> GetAllSupplierInfoList(string token, string username, bool includeOtherSupplier)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    List<SupplierInfo> supp = MrbillDbEntities.SupplierInfoes.ToList();
                    var suppListDTO = new List<SupplierInfoDTO>();
                    foreach (var item in supp)
                    {
                        if (!includeOtherSupplier)
                        {
                            if (item.URL == "")
                            {
                                continue;
                            }
                        }
                        var suppDTO = MrBillUtility.ConverToSupplierInfoDTO(item);

                        suppListDTO.Add(suppDTO);
                    }
                    return suppListDTO;
                }
            }
            catch (Exception exception)
            {
                List<SupplierInfoDTO> errors = new List<SupplierInfoDTO>();
                errors.Add(new SupplierInfoDTO() { SupplierName = exception.Message });
                return errors;
            }
        }

        public bool IsSuperAdmin(int userId)
        {
            return _userAccesser.IsSuperAdmin(userId);
        }
        public bool IsAdmin(int userId)
        {
            return _userAccesser.IsAdmin(userId);
        }

        public List<Object> GetAllMessage()
        {
            return _managerAccesser.GetAllMessage();
        }
        public List<UserDTO> GetAllUser(string token, string username)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var listUserDTO = new List<UserDTO>();

                    foreach (var item in _managerAccesser.GettAllUser())
                    {
                        var userProfile = MrbillDbEntities.UserProfiles.FirstOrDefault(u => u.UserName == item.UserName);
                        UserDTO result = MrBillUtility.ConvertToUserDTO(userProfile);
                        result.UserSupplierInfoes = MrBillUtility.GetUserSupplierInfoes(result.UserId, MrbillDbEntities);
                        listUserDTO.Add(result);
                    }
                    return listUserDTO;
                }

                return null;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public List<UserSupplierInfoDTO> GetAllUserSupplierInfo(string token, string username)
        {
            if (!MrBillUtility.CheckValidToken(token, username)) return null;
            try
            {
                using (var MrbillDbEntities = new MrBillEntities())
                {
                    var listUserDTO = new List<UserSupplierInfoDTO>();

                    foreach (var item in _managerAccesser.GetAllUserSupplierInfo())
                    {
                        UserSupplierInfoDTO result = MrBillUtility.ConverToUserSupplierInfoDTO(item);
                        listUserDTO.Add(result);
                    }
                    return listUserDTO;
                }

                return null;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}
