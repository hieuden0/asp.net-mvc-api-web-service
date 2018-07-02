using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MrBillServices.DTO;
using MrBillServices.Entities;
using MrBillServices.Utilities;
using MrBillServices.State;

namespace MrBillServices.DAO
{
    public class UserAccesser
    {
        public bool CreateUserProfile(UserProfile user)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                var checkExistUser = mrbillDbEntities.UserProfiles.FirstOrDefault(u => u.UserName == user.UserName);

                if (checkExistUser == null)
                {
                    mrbillDbEntities.UserProfiles.Add(user);
                    mrbillDbEntities.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public UserProfile GetUserProfileByUsername(string username)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                var existUser = mrbillDbEntities.UserProfiles
                    .Include("CompanyInfo")
                    .FirstOrDefault(u => u.UserName == username);
                return existUser;
            }
        }

       

        public void CreateUserSupplier(UserSupplierInfo userSupplier)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                mrbillDbEntities.UserSupplierInfoes.Add(userSupplier);
                mrbillDbEntities.SaveChanges();
            }
        }
        public void EditUserSupplier(UserSupplierInfo userSupplier)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {

                mrbillDbEntities.UserSupplierInfoes.Attach(userSupplier);
                mrbillDbEntities.Entry(userSupplier).State = EntityState.Modified;
                mrbillDbEntities.SaveChanges();
            }
        }

        public UserSupplierInfo GetSupplierInfoById(int userId, int supplierId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {

                return
                    mrbillDbEntities.UserSupplierInfoes.FirstOrDefault(
                        us => us.UserID == userId && us.SupplierID == supplierId);
            }
        }



        public List<UserSupplierInfo> GetSupplierInfoByUserId(int userId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {

                return mrbillDbEntities.UserSupplierInfoes.Where(us => us.UserID == userId).ToList();
            }
        }
        public List<UserSupplierInfo> GetSupplierInfoBySupplierId(int supplierId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {

                return mrbillDbEntities.UserSupplierInfoes.Where(us => us.SupplierID == supplierId).ToList();
            }
        }

        public UserProfile GetUserProfileByUserId(long userId)
        {
            using (var mrbillDbEntities = new MrBillEntities())
            {
                var existUser = mrbillDbEntities.UserProfiles
                    .Include("CompanyInfo")
                    .FirstOrDefault(u => u.UserID == userId);
                return existUser;
            }
        }

        //Scraper
        public List<UserProfile> GetAllUserProfileBySupplier()
        {

            using (var mrbillDbEntities = new MrBillEntities())
            {
                return mrbillDbEntities.UserProfiles.Include("UserSupplierInfoes").Where(u => u.UserSupplierInfoes.Count > 0).ToList();

            }

        }

        public void AddSettingForUser(Setting setting)
        {
            using (var MrbillDbEntities = new MrBillEntities())
            {
                var result = MrbillDbEntities.Settings.FirstOrDefault(s => s.UserID == setting.UserID && s.SettingName == setting.SettingName);
                if (result == null)
                {
                    MrbillDbEntities.Settings.Add(setting);
                }
                else
                {
                    result.SettingValue = setting.SettingValue;
                }
                 MrbillDbEntities.SaveChanges();
            }
        }

        public Setting GetSettingForUser(int userId)
        {
            using (var MrbillDbEntities = new MrBillEntities())
            {
                var result = MrbillDbEntities.Settings.FirstOrDefault(s => s.UserID == userId && s.SettingName == "email_setting");
                return result;
            }
        }

        public bool IsSuperAdmin(int userId)
        {
            using (var MrbillDbEntities = new MrBillEntities())
            {
                return MrbillDbEntities.UserProfiles.Any(s => s.UserID == userId && s.UserRoleID == (int)MrbillState.MrbillUserRole.MrBillAdmin);
            }
        }

        public bool IsAdmin(int userId)
        {
            using (var MrbillDbEntities = new MrBillEntities())
            {
                return MrbillDbEntities.UserProfiles.Any(s => s.UserID == userId && s.UserRoleID != (int)MrbillState.MrbillUserRole.OrdinaryUser);
            }
        }
    }
}
