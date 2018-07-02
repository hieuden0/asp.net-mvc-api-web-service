using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MrBillServices.DTO;
//using OpenPop.Mime;
using MrBillServices.Entities;

namespace MrBillServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMrBillUserServices" in both code and config file together.
    [ServiceContract]
    public interface IMrBillUserServices
    {
        [OperationContract]
        int CreateUser(UserDTO user);

        [OperationContract]
        int UpdateUser(string token, UserDTO user);

        [OperationContract]
        UserDTO GetUserByUsername(string token, string username);

        [OperationContract]
        int GetUserIDByUsername(string token, string username);

        [OperationContract]
        List<SupplierInfoDTO> GetAllSupplierInfoList(string token, string username, bool includeOtherSupplier);

        [OperationContract]
        string AddOrUpdateSetting(string token, string username, SettingDTO settingDto);

        [OperationContract]
        SettingDTO GetSetingById(string token, string username, int userId);

        [OperationContract]
        bool IsSuperAdmin(int userId);

        [OperationContract]
        bool IsAdmin(int userId);

        [OperationContract]
        List<Object> GetAllMessage();

        [OperationContract]
        List<UserDTO> GetAllUser(string token, string username);

        [OperationContract]
        List<UserSupplierInfoDTO> GetAllUserSupplierInfo(string token, string username);
    }
}
