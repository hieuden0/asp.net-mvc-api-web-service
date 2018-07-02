using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Drawing.Imaging;

using iTextSharp.text;
using iTextSharp.text.pdf;
using MrBillServices.DAO;
using MrBillServices.DTO;
using MrBillServices.Entities;
using MrBillServices.State;


namespace MrBillServices.Utilities
{
    class MrBillUtility
    {
        private readonly UserAccesser _userAccesser = new UserAccesser();
        private readonly TransactionAccesser _transactionAccesser = new TransactionAccesser();
        //private readonly LogAccesser _logAccesser = new LogAccesser();
        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden


        public static string RandomString(int size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, size)
              .Select(s => s[random.Next(s.Length)]).ToArray());

        }
        public static string UploadRecieptLocaltion(DateTime date, int userId, string supplierName)
        {
            return "uploads/reciept/" + date.Year + "/" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month) + "/" +
                                                       supplierName + "/" + userId;

        }
        public static bool CheckValidToken(string token, string keyCode)
        {
            try
            {
                string result = MrBillAuthDecode.MrBillDecode.MrBillAuthDecode(token, keyCode);
                DateTime dateResult = new DateTime(long.Parse(result));
                if (dateResult.CompareTo(DateTime.UtcNow) > 0) return true;
                return false;
            }
            catch (Exception exception)
            {
                return false;
            }

        }
        public static bool ValidToken(string token)
        {
            return true;
        }

        public static Transaction ConvertToEntityTransaction(TransactionDTO trans)
        {

            Transaction result = new Transaction();

            #region Transaction

            result.TransactionID = trans.TransactionId;
            result.AddedDate = trans.AddedDate;
            if (trans.AirDepTime1.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.AirDepTime1 = null;
            else
                result.AirDepTime1 = trans.AirDepTime1;

            if (trans.AirDepTime2.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.AirDepTime2 = null;
            else
                result.AirDepTime2 = trans.AirDepTime2;

            if (trans.AirRetTime1.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.AirRetTime1 = null;
            else
                result.AirRetTime1 = trans.AirRetTime1;

            if (trans.AirRetTime2.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.AirRetTime2 = null;
            else
                result.AirRetTime2 = trans.AirRetTime2;

            if (trans.ApproveDate.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.ApproveDate = null;
            else
                result.ApproveDate = trans.ApproveDate;

            result.Attendees = trans.Attendees;

            if (trans.BookingDate.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.BookingDate = null;
            else
                result.BookingDate = trans.BookingDate;

            result.BookingRef = trans.BookingRef;
            result.CardHolder = trans.CardHolder;
            result.CardNumber = trans.CardNumber;

            if (trans.CarTime1.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.CarTime1 = null;
            else
                result.CarTime1 = trans.CarTime1;

            if (trans.CarTime2.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.CarTime2 = null;
            else
                result.CarTime2 = trans.CarTime2;

            result.CategoryID = trans.CategoryID;
            result.CityDep1 = trans.CityDep1;
            result.CityDep2 = trans.CityDep2;
            result.CityRet1 = trans.CityRet1;
            result.CityRet2 = trans.CityRet2;
            result.CostCenter = trans.CostCenter;
            result.Country = trans.Country;
            result.Description = trans.Description;
            result.Destination = trans.Destination;
            result.EmployeeID = trans.EmployeeID;
            result.ExportStatus = trans.ExportStatus;
            result.ExtraInfo = trans.ExtraInfo;
            result.HtlTime1 = trans.HtlTime1 == DateTime.MinValue ? null : trans.HtlTime1;
            result.HtlTime2 = trans.HtlTime2 == DateTime.MinValue ? null : trans.HtlTime2;
            result.PaymentID = trans.PaymentID;
            result.Price = trans.Price;
            result.PriceCurrency = trans.PriceCurrency;
            result.PriceUserCurrency = trans.PriceUserCurrency;
            result.Product = trans.Product;
            result.ProjectNO = trans.ProjectNO;

            if (trans.RemoveDate.ToString("yyyy-MM-dd") == DateTime.MinValue.ToString("yyyy-MM-dd"))
                result.RemoveDate = null;
            else
                result.RemoveDate = trans.RemoveDate;

            result.Status = trans.Status;
            result.SupplierID = trans.SupplierID;
            result.Total = trans.Total;
            result.TransactionGroupID = trans.TransactionGroupID;
            result.Traveller = trans.Traveller;
            result.Units = trans.Units;
            result.UnlockedBy = trans.UnlockedBy;
            result.UnlockedDate = trans.UnlockedDate == DateTime.MinValue ? null : trans.UnlockedDate;
            result.UserID = trans.UserID;
            result.Vat1 = trans.Vat1;
            result.Vat2 = trans.Vat2;
            result.Vat3 = trans.Vat3;
            result.ReceiptLink = trans.ReceiptLink;
            result.MainTrip = trans.MainTrip;
            #endregion

            return result;
        }

        public static UserSupplierInfo ConvertToEntityUserSupplierInfo(UserSupplierInfoDTO usersup)
        {
            UserSupplierInfo result = new UserSupplierInfo();
            result.Password = usersup.Password;
            result.SupplierID = usersup.SupplierId;
            result.UserID = usersup.UserId;
            result.Username = usersup.Username;
            return result;
        }

        public static SupplierInfoDTO ConverToSupplierInfoDTO(SupplierInfo supplierEntity)
        {
            SupplierInfoDTO result = new SupplierInfoDTO();
            result.ResetPasswordUrl = supplierEntity.ResetPasswordUrl;
            result.SignUpUrl = supplierEntity.SignUpUrl;
            result.SupplierId = supplierEntity.SupplierID;
            result.SupplierName = supplierEntity.Name;
            result.Url = supplierEntity.URL;
            return result;
        }

        public static UserSupplierInfoDTO ConverToUserSupplierInfoDTO(UserSupplierInfo userSupplierEntity)
        {
            UserSupplierInfoDTO result = new UserSupplierInfoDTO();
            result.SupplierId = userSupplierEntity.SupplierID;
            result.UserId = userSupplierEntity.UserID;
            result.Username = userSupplierEntity.Username;
            result.Password = userSupplierEntity.Password;
            return result;
        }
        public static UserDTO ConvertToUserDTO(UserProfile userProfile)
        {
            UserDTO result = new UserDTO();
            result.Address = userProfile.Address;
            result.City = userProfile.City;
            result.CompanyId = userProfile.CompanyID;
            result.FirstName = userProfile.FirstName;
            result.LastName = userProfile.LastName;
            result.Password = userProfile.Password;
            result.Phone = userProfile.Phone;
            result.PostCode = userProfile.PostCode;
            result.Status = userProfile.Status;
            result.UserId = userProfile.UserID;
            result.Username = userProfile.UserName;
            result.UserRoleId = userProfile.UserRoleID;
            result.Country = userProfile.Country;
            //Get User Role
            var userRole = userProfile.UserRole;
            var userRoleModel = new UserRolesDTO();
            userRoleModel.RoleId = userRole.UserRoleID;
            userRoleModel.RoleName = userRole.Name;

            result.UserRoles = userRoleModel;

            //Get Company Info
            var com = userProfile.CompanyInfo;
            var company = new CompanyInfoDTO();
            company.Address = com.Adress;
            company.CompanyId = com.CompanyID;
            company.CompanyName = com.Name;
            company.Country = com.Country;
            company.Email = com.Email;
            company.Phone = com.Phone;
            company.VATCode = com.VatCode;

            result.CompanyInfo = company;
            return result;
        }

        public static UserProfile ConvertToEntityUserProfile(UserDTO userDto)
        {
            UserProfile userProfile = new UserProfile();
            userProfile.UserID = userDto.UserId;
            userProfile.Address = userDto.Address == null ? "" : userDto.Address;
            userProfile.City = userDto.City == null ? "" : userDto.City;
            userProfile.CompanyID = userDto.CompanyId;
            userProfile.UserName = userDto.Username;
            userProfile.Password = userDto.Password;
            userProfile.FirstName = userDto.FirstName == null ? "" : userDto.FirstName;
            userProfile.LastName = userDto.LastName == null ? "" : userDto.LastName;
            userProfile.PostCode = userDto.PostCode == null ? "" : userDto.PostCode;
            userProfile.Phone = userDto.Phone == null ? "" : userDto.Phone;
            userProfile.Status = userDto.Status;
            userProfile.UserRoleID = userDto.UserRoleId;
            userProfile.Country = userDto.Country;
            return userProfile;
        }

        public static Setting ConvertToEntitySetting(SettingDTO settingDto)
        {
            Setting setting = new Setting();
            setting.ID = settingDto.ID;
            setting.UserID = settingDto.UserID;
            setting.SettingName = settingDto.SettingName;
            setting.SettingValue = settingDto.SettingValue;
            return setting;
        }

        public static SettingDTO ConvertToSettingDTO(Setting setting)
        {
            SettingDTO settingDto = new SettingDTO();
            settingDto.ID = setting.ID;
            settingDto.UserID = setting.UserID;
            settingDto.SettingName = setting.SettingName;
            settingDto.SettingValue = setting.SettingValue;
            return settingDto;
        }

        public static TransactionDTO ConvertToTransactionDTO(Transaction trans)
        {
            TransactionDTO result = new TransactionDTO();
            #region Get Transaction

            result.TransactionId = trans.TransactionID;
            result.AddedDate = trans.AddedDate;
            result.AirDepTime1 = trans.AirDepTime1 ?? DateTime.MinValue;
            result.AirDepTime2 = trans.AirDepTime2 ?? DateTime.MinValue;
            result.AirRetTime1 = trans.AirRetTime1 ?? DateTime.MinValue;
            result.AirRetTime2 = trans.AirRetTime2 ?? DateTime.MinValue;
            result.ApproveDate = trans.ApproveDate ?? DateTime.MinValue;
            result.Attendees = trans.Attendees;
            result.BookingDate = trans.BookingDate ?? DateTime.MinValue;
            result.BookingRef = trans.BookingRef;
            result.CardHolder = trans.CardHolder;
            result.CardNumber = trans.CardNumber;
            result.CarTime1 = trans.CarTime1 ?? DateTime.MinValue;
            result.CarTime2 = trans.CarTime2 ?? DateTime.MinValue;
            result.CategoryID = trans.CategoryID;
            result.CityDep1 = trans.CityDep1;
            result.CityDep2 = trans.CityDep2;
            result.CityRet1 = trans.CityRet1;
            result.CityRet2 = trans.CityRet2;
            result.CostCenter = trans.CostCenter;
            result.Country = trans.Country;
            result.Description = trans.Description;
            result.Destination = trans.Destination;
            result.EmployeeID = trans.EmployeeID;
            result.ExportStatus = trans.ExportStatus;
            result.ExtraInfo = trans.ExtraInfo;
            result.HtlTime1 = trans.HtlTime1;
            result.HtlTime2 = trans.HtlTime2;
            result.PaymentID = trans.PaymentID;
            result.Price = trans.Price ?? 0;
            result.PriceCurrency = trans.PriceCurrency;
            result.PriceUserCurrency = trans.PriceUserCurrency ?? 0;
            result.Product = trans.Product;
            result.ProjectNO = trans.ProjectNO;
            result.RemoveDate = trans.RemoveDate ?? DateTime.MinValue;
            result.Status = trans.Status;
            result.SupplierID = trans.SupplierID;
            result.Total = trans.Total ?? 0;
            result.TransactionGroupID = trans.TransactionGroupID;
            result.TransactionId = trans.TransactionID;
            result.Traveller = trans.Traveller;
            result.Units = trans.Units;
            result.UnlockedBy = trans.UnlockedBy ?? 0;
            result.UnlockedDate = trans.UnlockedDate;
            result.UserID = trans.UserID;
            result.Vat1 = trans.Vat1;
            result.Vat2 = trans.Vat2;
            result.Vat3 = trans.Vat3;
            result.ReceiptLink = trans.ReceiptLink;
            result.MainTrip = trans.MainTrip;
            //result.MainTripGroup = trans.MainTripGroup;
            #endregion
            #region Get Payment
            result.PaymentID = trans.PaymentID;
            var paymentItem = trans.PaymentType;
            var payment = new PaymentTypeDTO()
            {
                PaymentId = paymentItem.PaymentID,
                PaymentName = paymentItem.Name
            };
            result.PaymentType = payment;
            #endregion
            #region Get Supplier Infoes
            var supplierItem = trans.SupplierInfo;
            var supplierInfoes = new SupplierInfoDTO()
            {
                ResetPasswordUrl = supplierItem.ResetPasswordUrl,
                SignUpUrl = supplierItem.SignUpUrl,
                SupplierId = supplierItem.SupplierID,
                SupplierName = supplierItem.Name,
                Url = supplierItem.URL
            };
            result.SupplierInfoes = supplierInfoes;
            #endregion
            #region Get Project
            var project = trans.Project;
            if (project != null)
            {
                var projectDTO = new ProjectDTO()
                {
                    ProjectID = project.ProjectID,
                    ProjectNumber = project.No,
                    UserControlID = project.UserID
                };
                result.Project = projectDTO;
            }

            #endregion

            #region Get TransactionGroup
            var group = trans.TransactionGroup;
            if (group != null)
            {
                var groupDTO = new TransactionGroupDTO()
                {
                    TransactionGroupID = group.TransactionGroupID,
                    Name = group.Name
                };
                result.TransactionGroup = groupDTO;
            }

            #endregion

            return result;
        }

        public static ContactInfoDTO GetContactByUserPF(UserProfile userProf)
        {
            return new ContactInfoDTO()
            {
                FirstName = userProf.FirstName,
                LastName = userProf.LastName,
                Phone = userProf.Phone,
                UserId = userProf.UserID,
                UserName = userProf.UserName,
                Address = userProf.Address,
                City = userProf.City,
                PostCode = userProf.PostCode
            };
        }

        public static List<UserSupplierInfoDTO> GetUserSupplierInfoes(long userID, MrBillEntities MrbillDbEntities)
        {
            var userSupplierInfo = MrbillDbEntities.UserSupplierInfoes.Where(us => us.UserID == userID).ToList();
            var userSupplierInfoList = new List<UserSupplierInfoDTO>();
            foreach (var item in userSupplierInfo)
            {
                var userSupplierInfoModel = new UserSupplierInfoDTO();
                userSupplierInfoModel.Password = item.Password;
                userSupplierInfoModel.SupplierId = item.SupplierID;
                userSupplierInfoModel.UserId = item.UserID;
                userSupplierInfoModel.Username = item.Username;

                //var userModel = new UserDTO();
                //var userProfile = item.UserProfile;
                //userModel.Address = userProfile.Address;
                //userModel.City = userProfile.City;
                //userModel.CompanyId = userProfile.CompanyID;
                //userModel.FirstName = userProfile.FirstName;
                //userModel.LastName = userProfile.LastName;
                //userModel.Password = userProfile.Password;
                //userModel.Phone = userProfile.Phone;
                //userModel.PostCode = userProfile.PostCode;
                //userModel.Status = userProfile.Status;
                //userModel.UserId = userProfile.UserID;
                //userModel.Username = userProfile.UserName;
                //userModel.UserRoleId = userProfile.UserRoleID;

                //userSupplierInfoModel.UserProfile = userModel;

                var supplierInfoModel = new SupplierInfoDTO();
                var supplierInfo = item.SupplierInfo;
                supplierInfoModel.Url = supplierInfo.URL;
                supplierInfoModel.SupplierName = supplierInfo.Name;
                supplierInfoModel.SupplierId = supplierInfo.SupplierID;
                supplierInfoModel.SignUpUrl = supplierInfo.SignUpUrl;
                supplierInfoModel.ResetPasswordUrl = supplierInfo.ResetPasswordUrl;

                userSupplierInfoModel.SupplierInfo = supplierInfoModel;

                userSupplierInfoList.Add(userSupplierInfoModel);
            }
            return userSupplierInfoList;
        }

        #region create Pdf


        public bool SendEmailReport(string username, List<string> emailList, int month, int year, string languageCode, int SortType)
        {
            var user = _userAccesser.GetUserProfileByUsername(username);
            var trans = _transactionAccesser.GetTransactionsByUsername(username);
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "Pdf\\" + user.UserName + " - " + "Transaction " + month + "-" + year + ".pdf";
            string _mailSetting = "file";
            var setting = _userAccesser.GetSettingForUser(user.UserID);
            if (setting != null)
            {
                _mailSetting = setting.SettingValue;
            }

            try
            {
                if (SortType == (int)MrbillState.TransSortType.ExpenseSort)
                {
                    CreateTransFile(user, trans, month, year, filePath, (int)MrbillState.TransSortType.ExpenseSort);
                }
                else
                {
                    CreateTransFile(user, trans, month, year, filePath, (int)MrbillState.TransSortType.AllowanceSort);
                }

                if (languageCode.Equals("en"))
                {
                    long size = new FileInfo(filePath).Length / 1024 / 1024;
                    if (_mailSetting.Equals("link"))
                    {
                        SendMailWithDownloadLink(user, emailList, month, year, filePath, languageCode);
                    }
                    else if (_mailSetting.Equals("file"))
                    {
                        if (size < 10)
                        {
                            SendMail(user, emailList, month, year, filePath, "sv");
                        }
                        else
                        {
                            SendMailWithDownloadLink(user, emailList, month, year, filePath, "sv");
                        }
                    }
                }
                else
                {
                    //filePath = "E:\\Project\\Mrbill-Hieu\\MrBill-Userinterface\\MrBill_MVC\\Pdf\\test@mrbill.se - Transaction 7-2016.pdf";
                    long size = new FileInfo(filePath).Length / 1024 / 1024;
                    if (_mailSetting.Equals("link"))
                    {
                        SendMailWithDownloadLink(user, emailList, month, year, filePath, languageCode);
                    }
                    else if (_mailSetting.Equals("file"))
                    {
                        if (size < 10)
                        {
                            SendMail(user, emailList, month, year, filePath, "sv");
                        }
                        else
                        {
                            SendMailWithDownloadLink(user, emailList, month, year, filePath, "sv");
                        }
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string pdfReview(string username, int month, int year, int SortType)
        {
            var user = _userAccesser.GetUserProfileByUsername(username);
            var trans = _transactionAccesser.GetTransactionsByUsername(username);
            var fileName = user.UserName + " - " + "Transaction " + month + "-" + year + ".pdf";
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "Pdf\\" + user.UserName + " - " + "Transaction " + month + "-" + year + ".pdf";

            try
            {
                if (SortType == (int)MrbillState.TransSortType.ExpenseSort)
                {
                    CreateTransFile(user, trans, month, year, filePath, (int)MrbillState.TransSortType.ExpenseSort);
                }
                else
                {
                    CreateTransFile(user, trans, month, year, filePath, (int)MrbillState.TransSortType.AllowanceSort);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public byte[] DownloadReport(string username, List<string> emailList, int month, int year, int SortType)
        {
            byte[] result;
            var user = _userAccesser.GetUserProfileByUsername(username);
            var trans = _transactionAccesser.GetTransactionsByUsername(username);
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "Pdf\\" + user.UserName + " - " + "Transaction " + month + "-" + year + ".pdf";


            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Pdf"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Pdf");
            }
            try
            {
                if (SortType == (int)MrbillState.TransSortType.ExpenseSort)
                {
                    CreateTransFile(user, trans, month, year, filePath, (int)MrbillState.TransSortType.ExpenseSort);
                }
                else
                {
                    CreateTransFile(user, trans, month, year, filePath, (int)MrbillState.TransSortType.AllowanceSort);
                }
                result = FileToByteArray(filePath);
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        private void CreateTransFile(UserProfile user, List<Transaction> trans, int month, int year, string filePath, int SortType)
        {
            var doc = new Document();

            //var doc = new Document(new RectangleReadOnly(842, 595));
            //var filePath = AppDomain.CurrentDomain.BaseDirectory + "Pdf/" + user.UserID + " - " + "Transaction " + month + "-" + year + ".pdf";

            //if (File.Exists(filePath)) return;
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var currencyFormat = "N";
                var writer = PdfWriter.GetInstance(doc, fs);

                try
                {
                    doc.Open();

                    int imgHeight = 640;
                    int imgWidth = 480;
                    float totalPrice = 0;
                    float totalVat = 0;

                    // Upper details

                    doc.Add(new Paragraph("Utskrivet: " + DateTime.Now) { Alignment = Element.ALIGN_RIGHT });
                    doc.Add(new Paragraph("Namn: " + user.FirstName + " " + user.LastName));
                    doc.Add(new Paragraph("Företag: " + (user.CompanyInfo == null ? "" : user.CompanyInfo.Name)));
                    doc.Add(new Paragraph("Adress: " + user.Address));
                    doc.Add(new Paragraph("Stad: " + user.City));
                    doc.Add(new Paragraph("PostNr: " + user.PostCode));
                    doc.Add(new Paragraph("Land: " + user.Country));
                    doc.Add(new Chunk("\n"));
                    doc.Add(new Paragraph(GetSweMonths(month) + " " + year, new Font(Font.COURIER, 16, Font.BOLD)));
                    doc.Add(new Chunk("\n"));

                    // End upper details

                    var fontTable = new Font(Font.COURIER, 7, Font.BOLD);
                    var fontHeader = new Font(Font.COURIER, 7, Font.BOLD);
                    var fontGroup = new Font(Font.COURIER, 8, Font.BOLD);
                    var fontTotalGroup = new Font(Font.COURIER, 8, Font.BOLDITALIC, Color.BLACK);
                    var fontTotal = new Font(Font.COURIER, 7, Font.BOLD, Color.BLUE);
                    var count = 0;

                    var items = new List<Transaction>();
                    if (SortType == (int)MrbillState.TransSortType.ExpenseSort)
                    {
                        items = trans.Where(dto => dto.AddedDate.Month == month && dto.AddedDate.Year == year && dto.Vat3 == null).OrderBy(dto => dto.AddedDate).ToList(); //.OrderBy(dto => dto.AddedDate)
                    }
                    else
                    {
                        items = trans.Where(dto => dto.AirDepTime1 != null && ((DateTime)dto.AirDepTime1).Month == month && ((DateTime)dto.AirDepTime1).Year == year && dto.Vat3 == null).OrderBy(dto => dto.AddedDate).ToList(); //.OrderBy(dto => dto.AddedDate)
                    }


                    // cell blank

                    PdfPCell cellblank = new PdfPCell(new Phrase("", fontGroup));
                    cellblank.Colspan = 11;
                    cellblank.BorderColor = Color.WHITE;
                    // cell blank

                    PdfPCell cellLine = new PdfPCell(new Phrase("", fontGroup));
                    cellLine.Colspan = 11;
                    cellLine.BorderColorBottom = Color.BLACK;
                    cellLine.BackgroundColor = Color.BLACK;

                    // The transactions table
                    var table = new PdfPTable(11) { WidthPercentage = 100 };
                    float[] colWidthPercentages = new[] { 3f, 10f, 10f, 10f, 10f, 12f, 8f, 6f, 12f, 10f, 6f };
                    table.SetWidths(colWidthPercentages);

                    var backgroundColor = Color.WHITE;

                    table.AddCell(CreateTableCell("Nr", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Datum", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Leverantör", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Ort", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Produkt", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Kvittobelopp", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("SEK", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Moms", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Kostnadsställe", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Tillägsinfo", fontHeader, backgroundColor));
                    table.AddCell(CreateTableCell("Kvitto", fontHeader, backgroundColor));

                    items.ForEach(dto =>
                    {
                        count++;
                        if (dto.MainTrip == null && dto.TransactionGroupID != null)
                        {
                            float totalGroupPrice = 0;
                            float totalGroupVat = 0;
                            if (count > 1)
                            {
                                table.AddCell(cellblank);
                            }
                            //if is maintrip
                            PdfPCell cellGroup = new PdfPCell(new Phrase(dto.TransactionGroup.Name, fontGroup));
                            cellGroup.Colspan = 11;
                            cellGroup.BorderColor = Color.WHITE;
                            table.AddCell(cellGroup);
                            ItemTracsaction(table, backgroundColor, count, dto, fontTable, fontGroup, currencyFormat);
                            if (dto.PriceUserCurrency == null || dto.PriceUserCurrency == 0)
                            {
                                totalPrice += dto.Price.Value;
                                totalGroupPrice += dto.Price.Value;
                            }
                            else
                            {
                                totalPrice += dto.PriceUserCurrency.Value;
                                totalGroupPrice += dto.PriceUserCurrency.Value;
                            }
                            if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
                            {
                                totalVat += dto.Vat1.Value;
                                totalGroupVat += dto.Vat1.Value;
                            }
                            var transTripItem = new List<Transaction>();
                            transTripItem = trans.Where(t => t.MainTrip == dto.TransactionID).OrderBy(t => t.AddedDate).ToList();



                            for (int i = 0; i < transTripItem.Count; i++)
                            {
                                count++;
                                ItemTracsaction(table, backgroundColor, count, transTripItem[i], fontTable, fontGroup, currencyFormat);
                                if (transTripItem[i].PriceUserCurrency == null || transTripItem[i].PriceUserCurrency == 0)
                                {
                                    totalPrice += transTripItem[i].Price.Value;
                                    totalGroupPrice += transTripItem[i].Price.Value;
                                }
                                else
                                {
                                    totalPrice += transTripItem[i].PriceUserCurrency.Value;
                                    totalGroupPrice += transTripItem[i].PriceUserCurrency.Value;
                                }
                                if (string.IsNullOrEmpty(transTripItem[i].PriceCurrency) || transTripItem[i].PriceCurrency.ToUpperInvariant().Equals("SEK"))
                                {
                                    totalVat += transTripItem[i].Vat1.Value;
                                    totalGroupVat += transTripItem[i].Vat1.Value;
                                }
                            }
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("Total", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell(Convert.ToDecimal(totalGroupPrice).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell(Convert.ToDecimal(totalGroupVat).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                            table.AddCell(cellblank);
                        }
                        else if (dto.MainTrip != null && dto.TransactionGroupID != null)
                        {
                            count--;
                        }
                        else
                        {
                            ItemTracsaction(table, backgroundColor, count, dto, fontTable, fontGroup, currencyFormat);
                            if (dto.PriceUserCurrency == null || dto.PriceUserCurrency == 0)
                            {
                                totalPrice += dto.Price.Value;
                            }
                            else
                            {
                                totalPrice += dto.PriceUserCurrency.Value;
                            }
                            if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
                            {
                                totalVat += dto.Vat1.Value;
                            }
                        }

                    });

                    table.AddCell(cellblank);
                    table.AddCell(cellLine);


                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("Total", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell(Convert.ToDecimal(totalPrice).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell(Convert.ToDecimal(totalVat).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));
                    table.AddCell(CreateTableCell("", fontTotal, Color.WHITE));

                    doc.Add(table);


                    doc.Add(new Chunk("\n"));

                    // End the transactions table


                    // ********* if have group *******************//
                    var itemGroups = new List<Transaction>();
                    if (SortType == (int)MrbillState.TransSortType.ExpenseSort)
                    {
                        itemGroups = trans.Where(dto => dto.AddedDate.Month == month && dto.AddedDate.Year == year && dto.Vat3 == null && dto.TransactionGroupID != null && dto.MainTrip == null).OrderBy(dto => dto.AddedDate).ToList(); //.OrderBy(dto => dto.AddedDate)
                    }
                    else
                    {
                        itemGroups = trans.Where(dto => dto.AirDepTime1 != null && ((DateTime)dto.AirDepTime1).Month == month && ((DateTime)dto.AirDepTime1).Year == year && dto.Vat3 == null && dto.TransactionGroupID != null && dto.MainTrip == null).OrderBy(dto => dto.AddedDate).ToList(); //.OrderBy(dto => dto.AddedDate)
                    }

                    GroupTransaction(doc, table, backgroundColor, count, itemGroups, trans, fontHeader, fontTable, fontTotal, currencyFormat, cellblank, cellLine, imgWidth, imgHeight, colWidthPercentages);

                   

                    // Transaction details
                    count = 0;
                    var itemNormal = new List<Transaction>();
                    if (SortType == (int)MrbillState.TransSortType.ExpenseSort)
                    {
                        itemNormal = trans.Where(dto => dto.AddedDate.Month == month && dto.AddedDate.Year == year && dto.Vat3 == null && dto.TransactionGroupID == null && dto.MainTrip == null).OrderBy(dto => dto.AddedDate).ToList(); //.OrderBy(dto => dto.AddedDate)
                    }
                    else
                    {
                        itemNormal = trans.Where(dto => dto.AirDepTime1 != null && ((DateTime)dto.AirDepTime1).Month == month && ((DateTime)dto.AirDepTime1).Year == year && dto.Vat3 == null && dto.TransactionGroupID == null && dto.MainTrip == null).OrderBy(dto => dto.AddedDate).ToList(); //.OrderBy(dto => dto.AddedDate)
                    }
                    if (itemGroups.Count > 0)
                    {
                        GroupNormalTransaction(doc, table, backgroundColor, count, itemNormal, trans, fontHeader, fontTable, fontTotal, currencyFormat, cellblank, cellLine, imgWidth, imgHeight, colWidthPercentages);
                    }
                    else
                    {
                        ReceiptTransaction(table, doc, count, itemNormal, fontTable, imgWidth, imgHeight);
                    }                   
                    // End transaction details
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    doc.Close();
                    writer.Close();
                    fs.Close();
                }
            }

        }

        private void ItemTracsaction(PdfPTable table, Color backgroundColor, int count, Transaction dto, Font fontTable, Font fontGroup, string currencyFormat)
        {
            var isEven = count % 2 == 0;

            backgroundColor = Color.WHITE;
            if (!isEven)
            {
                backgroundColor = new Color(230, 230, 230);
            }
            //if is maintrip


            table.AddCell(CreateTableCell(count.ToString(), fontTable, backgroundColor));
            if (dto.AirDepTime1 != null)
            {
                table.AddCell(CreateTableCell(dto.AirDepTime1.Value.ToString("yyy-dd-MM"), fontTable, backgroundColor));
            }
            else
            {
                table.AddCell(CreateTableCell("", fontTable, backgroundColor));
            }
            table.AddCell(CreateTableCell(dto.SupplierInfo.Name, fontTable, backgroundColor));
            table.AddCell(CreateTableCell(dto.Destination, fontTable, backgroundColor));
            table.AddCell(CreateTableCell((dto.Product ?? ""), fontTable, backgroundColor));

            //price
            if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
            {
                table.AddCell(CreateTableCell("", fontTable, backgroundColor));
            }
            else
            {
                table.AddCell(CreateTableCell((Convert.ToDecimal(dto.Price) + " " + dto.PriceCurrency.ToUpperInvariant()), fontTable, backgroundColor));
            }

            //SEK-price
            if (dto.PriceUserCurrency == 0 || string.IsNullOrEmpty(dto.PriceUserCurrency.ToString()))
            {
                table.AddCell(CreateTableCell(Convert.ToDecimal(dto.Price).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTable, backgroundColor));
            }
            else
            {
                table.AddCell(CreateTableCell(Convert.ToDecimal(dto.PriceUserCurrency).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTable, backgroundColor));
            }

            if (dto.Vat1 == null)
            {
                table.AddCell(CreateTableCell(("0.00 " + dto.PriceCurrency.ToUpperInvariant()), fontTable, backgroundColor));
            }
            else
            {
                if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
                {
                    table.AddCell(CreateTableCell(Convert.ToDecimal(dto.Vat1.Value).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTable, backgroundColor));
                }
                else
                {
                    table.AddCell(CreateTableCell("", fontTable, backgroundColor));
                }
            }

            table.AddCell(CreateTableCell((dto.Project != null ? dto.Project.No.ToString() : ""), fontTable, backgroundColor));
            table.AddCell(CreateTableCell(dto.Description, fontTable, backgroundColor));

            if (string.IsNullOrEmpty(dto.ReceiptLink))
            {
                table.AddCell(CreateTableCell("Nej", fontTable, backgroundColor));
            }
            else
            {
                table.AddCell(CreateTableCell("Ja", fontTable, backgroundColor));
            }

        }
        private void GroupTransaction(Document doc, PdfPTable table, Color backgroundColor, int count, List<Transaction> items, List<Transaction> trans, Font fontHeader, Font fontTable, Font fontTotalGroup, string currencyFormat, PdfPCell cellblank, PdfPCell cellLine, int imgWidth, int imgHeight, float[] colWidthPercentages)
        {

            var fontGroup = new Font(Font.COURIER, 15, Font.BOLD);

            items.ForEach(dto =>
            {
                count = 0;
                doc.NewPage();
                doc.NewPage();
                count++;
                table = new PdfPTable(11) { WidthPercentage = 100 };
                table.SetWidths(colWidthPercentages);
                float totalGroupPrice = 0;
                float totalGroupVat = 0;
                //if is maintrip
                PdfPCell cellGroup = new PdfPCell(new Phrase(dto.TransactionGroup.Name, fontGroup));
                cellGroup.Colspan = 11;
                cellGroup.BorderColor = Color.WHITE;
                table.AddCell(cellGroup);
                table.AddCell(CreateTableCell("Nr", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Datum", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Leverantör", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Ort", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Produkt", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Kvittobelopp", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("SEK", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Moms", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Kostnadsställe", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Tillägsinfo", fontHeader, backgroundColor));
                table.AddCell(CreateTableCell("Kvitto", fontHeader, backgroundColor));

                ItemTracsaction(table, backgroundColor, count, dto, fontTable, fontGroup, currencyFormat);
                if (dto.PriceUserCurrency == null || dto.PriceUserCurrency == 0)
                {
                    totalGroupPrice += dto.Price.Value;
                }
                else
                {
                    totalGroupPrice += dto.PriceUserCurrency.Value;
                }
                if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
                {
                    totalGroupVat += dto.Vat1.Value;
                }
                var transTripItem = new List<Transaction>();
                transTripItem = trans.Where(t => t.MainTrip == dto.TransactionID).OrderBy(t => t.AddedDate).ToList();



                for (int i = 0; i < transTripItem.Count; i++)
                {
                    count++;
                    ItemTracsaction(table, backgroundColor, count, transTripItem[i], fontTable, fontGroup, currencyFormat);
                    if (transTripItem[i].PriceUserCurrency == null || transTripItem[i].PriceUserCurrency == 0)
                    {
                        totalGroupPrice += transTripItem[i].Price.Value;
                    }
                    else
                    {
                        totalGroupPrice += transTripItem[i].PriceUserCurrency.Value;
                    }
                    if (string.IsNullOrEmpty(transTripItem[i].PriceCurrency) || transTripItem[i].PriceCurrency.ToUpperInvariant().Equals("SEK"))
                    {
                        totalGroupVat += transTripItem[i].Vat1.Value;
                    }
                }
                table.AddCell(cellLine);
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("Total", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell(Convert.ToDecimal(totalGroupPrice).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell(Convert.ToDecimal(totalGroupVat).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
                table.AddCell(cellblank);
                doc.Add(table);
                List<Transaction> maintripItem = new List<Transaction>() { dto };
                ReceiptTransaction(table, doc, count, maintripItem, fontTable, imgWidth, imgHeight);
                ReceiptTransaction(table, doc, 99, transTripItem, fontTable, imgWidth, imgHeight);

            });

        }
        private void GroupNormalTransaction(Document doc, PdfPTable table, Color backgroundColor, int count, List<Transaction> items, List<Transaction> trans, Font fontHeader, Font fontTable, Font fontTotalGroup, string currencyFormat, PdfPCell cellblank, PdfPCell cellLine, int imgWidth, int imgHeight, float[] colWidthPercentages)
        {
            doc.NewPage();
            var fontGroup = new Font(Font.COURIER, 15, Font.BOLD);
            float totalGroupPrice = 0;
            float totalGroupVat = 0;
            table = new PdfPTable(11) { WidthPercentage = 100 };
            table.SetWidths(colWidthPercentages);
            PdfPCell cellGroup = new PdfPCell(new Phrase("Normal", fontGroup));
            cellGroup.Colspan = 11;
            cellGroup.BorderColor = Color.WHITE;
            table.AddCell(cellGroup);
            table.AddCell(CreateTableCell("Nr", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Datum", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Leverantör", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Ort", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Produkt", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Kvittobelopp", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("SEK", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Moms", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Kostnadsställe", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Tillägsinfo", fontHeader, backgroundColor));
            table.AddCell(CreateTableCell("Kvitto", fontHeader, backgroundColor));

            count = 0;
            items.ForEach(dto =>
            {                
                count++;                                                       
                ItemTracsaction(table, backgroundColor, count, dto, fontTable, fontGroup, currencyFormat);
                if (dto.PriceUserCurrency == null || dto.PriceUserCurrency == 0)
                {
                    totalGroupPrice += dto.Price.Value;
                }
                else
                {
                    totalGroupPrice += dto.PriceUserCurrency.Value;
                }
                if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
                {
                    totalGroupVat += dto.Vat1.Value;
                }                                
            });
            table.AddCell(cellLine);
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("Total", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell(Convert.ToDecimal(totalGroupPrice).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell(Convert.ToDecimal(totalGroupVat).ToString(currencyFormat, CultureInfo.GetCultureInfo("sv")), fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(CreateTableCell("", fontTotalGroup, Color.WHITE));
            table.AddCell(cellblank);
            doc.Add(table);
            ReceiptTransaction(table, doc, count, items, fontTable, imgWidth, imgHeight);
        }

        private void ReceiptTransaction(PdfPTable table, Document doc, int count, List<Transaction> items, Font fontTable, int imgWidth, int imgHeight)
        {
            if (count != 99)
            {
                count = 0;
            }
            else {
                count = 1;
            }            
            items.ForEach(dto =>
            {
                doc.NewPage();
                doc.NewPage();

                count++;
                doc.Add(new Paragraph("KVITTO " + count, new Font(Font.COURIER, 16, Font.BOLD)));

                table = new PdfPTable(7) { WidthPercentage = 100, SpacingBefore = 20 };

                table.AddCell(CreateTableCell("Datum", fontTable, Color.LIGHT_GRAY));
                table.AddCell(CreateTableCell("Leverantör", fontTable, Color.LIGHT_GRAY));
                table.AddCell(CreateTableCell("Ort", fontTable, Color.LIGHT_GRAY));
                table.AddCell(CreateTableCell("Produkt", fontTable, Color.LIGHT_GRAY));
                table.AddCell(CreateTableCell("Kvittobelopp", fontTable, Color.LIGHT_GRAY));
                table.AddCell(CreateTableCell("Moms", fontTable, Color.LIGHT_GRAY));
                table.AddCell(CreateTableCell("Betalsätt", fontTable, Color.LIGHT_GRAY));

                table.AddCell(CreateTableCell(dto.AddedDate.ToShortDateString(), fontTable, Color.WHITE));
                table.AddCell(CreateTableCell(dto.SupplierInfo.Name, fontTable, Color.WHITE));
                table.AddCell(CreateTableCell(dto.Destination, fontTable, Color.WHITE));
                table.AddCell(CreateTableCell(dto.Product, fontTable, Color.WHITE));

                var tempPriceCurrency = dto.PriceCurrency;
                if (string.IsNullOrEmpty(dto.PriceCurrency))
                {
                    tempPriceCurrency = "SEK";
                }
                table.AddCell(CreateTableCell(Convert.ToDecimal(dto.Price).ToString("#,0.00") + " " + tempPriceCurrency.ToUpperInvariant(), fontTable, Color.WHITE));

                if (dto.Vat1 != null)
                {
                    if (string.IsNullOrEmpty(dto.PriceCurrency) || dto.PriceCurrency.ToUpperInvariant().Equals("SEK"))
                    {
                        table.AddCell(CreateTableCell(Convert.ToDecimal(dto.Vat1).ToString("#,0.00"), fontTable, Color.WHITE));
                    }
                    else
                    {
                        table.AddCell(CreateTableCell("", fontTable, Color.WHITE));
                    }
                }
                else
                {
                    table.AddCell(CreateTableCell("0,00", fontTable, Color.WHITE));
                }
                table.AddCell(CreateTableCell(dto.CardNumber, fontTable, Color.WHITE));

                doc.Add(table);

                var repFilePath = ConfigurationManager.AppSettings["docPath"] + dto.ReceiptLink;

                if (dto.ReceiptLink.ToLower().Contains(".pdf"))
                {
                    if (!File.Exists(repFilePath)) return;

                    var pdfDoc = PdfiumViewer.PdfDocument.Load(repFilePath);
                    for (var i = 0; i < pdfDoc.PageCount; i++)
                    {
                        var image = pdfDoc.Render(i, imgWidth, imgHeight, true);
                        var tempPath = AppDomain.CurrentDomain.BaseDirectory + "Pdf/temp/" + "temp.png";
                        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Pdf/temp"))
                        {
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Pdf/temp");
                        }
                        image.Save(tempPath, ImageFormat.Png);
                        var img = Image.GetInstance(tempPath);
                        img.ScaleAbsolute(imgWidth, imgHeight);
                        img.Alignment = Element.ALIGN_CENTER;
                        img.WidthPercentage = 50;
                        doc.Add(img);
                    }
                }
                else
                {
                    if (!File.Exists(repFilePath)) return;

                    //using (var image = System.Drawing.Image.FromFile(repFilePath))
                    //using (var newImage = ScaleImage(image, imgWidth, imgHeight))
                    var image = new FileInfo(repFilePath);
                    using (var newImage = ResizePhoto(image, imgWidth, imgHeight))
                    {
                        var img = Image.GetInstance(newImage, Color.WHITE);
                        img.ScaleAbsolute(imgWidth, imgHeight);
                        img.Alignment = Element.ALIGN_CENTER;

                        doc.Add(img);
                    }
                }
            });
        }

        private PdfPCell CreateTableCell(string content, Font font, Color backgroundColor)
        {
            var cell = new PdfPCell(new Phrase(content, font))
            {
                Border = Rectangle.NO_BORDER,
                BackgroundColor = backgroundColor
            };

            return cell;
        }

        private string GetSweMonths(int month)
        {
            switch (month)
            {
                case 1:
                    return "Januari";
                case 2:
                    return "Februari";
                case 3:
                    return "Mars";
                case 4:
                    return "April";
                case 5:
                    return "Maj";
                case 6:
                    return "Juni";
                case 7:
                    return "Juli";
                case 8:
                    return "Augusti";
                case 9:
                    return "September";
                case 10:
                    return "Oktober";
                case 11:
                    return "November";
                case 12:
                    return "December";
            }

            return "";
        }

        private string GetSweMonthsEmail(int month)
        {
            switch (month)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "Maj";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Okt";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
            }

            return "";
        }

        private void SendMail(UserProfile user, List<string> emaiList, int month, int year, string filePath, string languageCode)
        {
            var client = new SmtpClient();
            client.Port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = ConfigurationManager.AppSettings["smtpServer"];
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpId"], ConfigurationManager.AppSettings["smtpPassword"]);

            if (languageCode.Equals("en"))
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]),
                    // todo: change to user email address
                    To = { string.Join(",", emaiList) },
                    Subject = "Report Mrbill " + GetSweMonthsEmail(month) + " " + year,
                    Body = "Hi " + user.UserName + ", the attached file is your transaction report of " + GetSweMonths(month) + " " + year
                };
                using (var attachment = new Attachment(filePath))
                {
                    mail.Attachments.Add(attachment);

                    client.Send(mail);
                }
            }
            else if (languageCode.Equals("sv"))
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]),
                    // todo: change to user email address
                    To = { string.Join(",", emaiList) },
                    Subject = "Sammanställning MrBill " + GetSweMonthsEmail(month) + " " + year,
                    Body = "Hi " + user.UserName + ", the attached file is your transaction report of " + GetSweMonths(month) + " " + year
                };
                using (var attachment = new Attachment(filePath))
                {
                    mail.Attachments.Add(attachment);

                    client.Send(mail);
                }
            }

            //var filePath = AppDomain.CurrentDomain.BaseDirectory + "Pdf/" + user.UserID + " - " + "Transaction " + month + "-" + year + ".pdf";


        }

        private void SendMailWithDownloadLink(UserProfile user, List<string> emaiList, int month, int year, string filePath, string languageCode)
        {
            var client = new SmtpClient();
            client.Port = Convert.ToInt16(ConfigurationManager.AppSettings["smtpPort"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = ConfigurationManager.AppSettings["smtpServer"];
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpId"], ConfigurationManager.AppSettings["smtpPassword"]);

            string downloadLink = "<a href='" + ConfigurationManager.AppSettings["server"] + "/Print/DownloadReport?username=" + user.UserName + "&month=" + month + "&year=" + year + "'>Download Link</a>";

            if (languageCode.Equals("en"))
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]),
                    // todo: change to user email address
                    To = { string.Join(",", emaiList) },
                    Subject = "Report Mrbill " + GetSweMonthsEmail(month) + " " + year,
                    Body = "Hi " + user.UserName + ", this is your transaction report of " + GetSweMonths(month) + ": " + downloadLink,
                    IsBodyHtml = true
                };
                client.Send(mail);
            }
            else if (languageCode.Equals("sv"))
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]),
                    // todo: change to user email address
                    To = { string.Join(",", emaiList) },
                    Subject = "Sammanställning MrBill " + GetSweMonthsEmail(month) + " " + year,
                    Body = "Hi " + user.UserName + ", this file is your transaction report of " + GetSweMonths(month) + ": " + downloadLink,
                    IsBodyHtml = true
                };
                client.Send(mail);

            }

            //var filePath = AppDomain.CurrentDomain.BaseDirectory + "Pdf/" + user.UserID + " - " + "Transaction " + month + "-" + year + ".pdf";


        }

        private byte[] FileToByteArray(string fileName)
        {
            byte[] buffer;

            try
            {
                var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var binaryReader = new BinaryReader(fileStream);
                var totalBytes = new FileInfo(fileName).Length;
                buffer = binaryReader.ReadBytes((int)totalBytes);
                fileStream.Close();
                fileStream.Dispose();
                binaryReader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return buffer;
        }

        #endregion

        #region resize Image

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new System.Drawing.Bitmap(newWidth, newHeight);

            using (var graphics = System.Drawing.Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }


            return newImage;
        }
        #endregion

        private System.Drawing.Image ResizePhoto(FileInfo sourceImage, int desiredWidth, int desiredHeight)
        {
            //throw error if bouning box is to small
            if (desiredWidth < 4 || desiredHeight < 4)
                throw new InvalidOperationException("Bounding Box of Resize Photo must be larger than 4X4 pixels.");
            var original = System.Drawing.Bitmap.FromFile(sourceImage.FullName);

            //store image widths in variable for easier use
            var oW = (decimal)original.Width;
            var oH = (decimal)original.Height;
            var dW = (decimal)desiredWidth;
            var dH = (decimal)desiredHeight;

            //check if image already fits
            if (oW < dW && oH < dH)
                return original; //image fits in bounding box, keep size (center with css) If we made it biger it would stretch the image resulting in loss of quality.

            //check for double squares
            if (oW == oH && dW == dH)
            {
                //image and bounding box are square, no need to calculate aspects, just downsize it with the bounding box
                System.Drawing.Bitmap square = new System.Drawing.Bitmap(original, (int)dW, (int)dH);
                original.Dispose();
                return square;
            }

            //check original image is square
            if (oW == oH)
            {
                //image is square, bounding box isn't.  Get smallest side of bounding box and resize to a square of that center the image vertically and horizonatally with Css there will be space on one side.
                int smallSide = (int)Math.Min(dW, dH);
                System.Drawing.Bitmap square = new System.Drawing.Bitmap(original, smallSide, smallSide);
                original.Dispose();
                return square;
            }

            //not dealing with squares, figure out resizing within aspect ratios            
            if (oW > dW && oH > dH) //image is wider and taller than bounding box
            {
                var r = Math.Min(dW, dH) / Math.Min(oW, oH); //two demensions so figure out which bounding box demension is the smallest and which original image demension is the smallest, already know original image is larger than bounding box
                var nH = oW * r; //will downscale the original image by an aspect ratio to fit in the bounding box at the maximum size within aspect ratio.
                var nW = oW * r;
                var resized = new System.Drawing.Bitmap(original, (int)nW, (int)nH);
                original.Dispose();
                return resized;
            }
            else
            {
                if (oW > dW) //image is wider than bounding box
                {
                    var r = dW / oW; //one demension (width) so calculate the aspect ratio between the bounding box width and original image width
                    var nW = oW * r; //downscale image by r to fit in the bounding box...
                    var nH = oW * r;
                    var resized = new System.Drawing.Bitmap(original, (int)nW, (int)nH);
                    original.Dispose();
                    return resized;
                }
                else
                {
                    //original image is taller than bounding box
                    var r = dH / oH;
                    var nH = oH * r;
                    var nW = oW * r;
                    var resized = new System.Drawing.Bitmap(original, (int)nW, (int)nH);
                    original.Dispose();
                    return resized;
                }
            }
        }


    }
}
