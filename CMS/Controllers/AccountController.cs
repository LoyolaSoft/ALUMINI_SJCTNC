using CMS.DAO;
using CMS.SQL.Account;
using CMS.SQL.StaffSQL;
using CMS.SQL.SupportData;
using CMS.SQL.UserAccount;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CMS.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public async Task<ActionResult> Index()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
            }

            var ObjViewModel = new AccountViewModel();
            ObjViewModel.liCollegeDetails = await Task.Run(() =>
            (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data);

            ObjViewModel.liCalender = await Task.Run(() => (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchDayOrderInfoByCurrentDate)).DataSource.Data);
            return View(ObjViewModel);
        }
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.howsmyssl.com/a/check");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        // GET: Account/Details/5

        public ActionResult SignOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account");
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        #region login
        // POST: Account/Create
        [HttpPost]
        //      [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ViewModel.ViewModel.AccountViewModel collection, string Latitude, string Longitude, string UserAgent, string IPAddress, string Place)
        {
            try
            {
                CommunicationController communicationCon = new CommunicationController();

                //if (string.IsNullOrWhiteSpace(Latitude) || string.IsNullOrWhiteSpace(Longitude))
                //{

                //    return Json(new
                //    {
                //        success = false,
                //        ErrorMessage = "Location access is required to log in.",
                //        redirectUrl = Url.Action("Index", "Account")
                //    });
                //}

                using (AccountViewModel objAccount = new AccountViewModel())
                {
                    ResultArgs resultArgs = new ResultArgs();
                    DataValue dv = new DataValue();
                    string sUserRoles = string.Empty;
                    string sUserRoleId = string.Empty;
                    dv.Clear();
                    dv.Add(Common.USER_ACCOUNT.USERNAME, collection.objLoginViewModel.Username, EnumCommand.DataType.String);
                    string sPassword = CommonMethods.GetSha256FromString(collection.objLoginViewModel.Password);
                    dv.Add(Common.USER_ACCOUNT.PASSWORD, sPassword, EnumCommand.DataType.String);
                    string sResult = string.Empty;
                    string sOTPContent = string.Empty;
                    resultArgs = await Task.Run(() => objAccount.FetchUserCredential(dv));

                    if (resultArgs.Success)
                    {
                        if (resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {
                            // Check for OTP
                            string isOtp = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.IS_OTP].ToString();
                            if (isOtp == "1")
                            {
                                string sMobileNo = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.MOBILE].ToString(); // Assume MOBILE column exists
                                //string sEmail = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.EMAIL].ToString();
                                string sStudentId = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID].ToString();
                                string name = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.NAME].ToString();

                                // Send OTP logic (reusing or adapting SendSMS/SendMail)
                                // We'll use the existing SendSMS logic structure but internally calling it or refactoring.
                                // For now, we will call 'SendSMS' method or similar logic directly if possible, or just generate and store.
                                // NOTE: The SendSMS method in this controller returns an ActionResult, which is not ideal for internal call.
                                // We will implement a helper or call the existing one if it was refactored. 
                                // For this implementation, let's assume we send it and store it.



                                // Send SMS/Email
                                // Note: You might need to adjust this part to actually send the message using your existing infrastructure.
                                // The existing SendSMS method in this file is an Action, so we can't easily call it directly and get the result without overhead.
                                // Ideally, extract the sending logic to a helper.
                                // For now, I'll simulate setting it up.

                                // Example:
                                //CommunicationController .SendSMS(sMobileNo, "Your OTP is " + sOTP); 



                                string sOTP = CommonMethods.GenerateOTP();

                                Session["OTP"] = sOTP;
                                Session["TempUser"] = resultArgs;
                                communicationCon.sSmsMode = Common.MessageType.OTP;
                                    communicationCon.sTextMessage = "Login OTP";
                                    sOTPContent = "Dear " + name + ", Your Verification code is " + sOTP + " For ERP Login. St.Joseph's College of Arts & Science (Autonomous) - Cuddalore.";
                                    //liSMS.Add(new MessageResult.sms() { to = sMobileNo, message = "Use " + sOTP + " as ERP account password reset code ", custom = sStudentId, custom1 = "0", custom2 = "0", CreditCount = "1" });

                                    communicationCon.liSMS.Add(new MessageResult.sms() { to = sMobileNo, message = sOTPContent, custom = sStudentId, custom1 = null, custom2 = "0", CreditCount = Convert.ToString(1) });
                                    communicationCon.listbulkSMS.Add(new MessageResult.listsms() { mobiles = sMobileNo, sms = sOTPContent, class_id = null, receipient_id = sStudentId });

                                communicationCon.SendMessageV1();
                                return Json(new
                                {
                                    success = true,
                                    isOtpRequired = true,
                                    ErrorMessage = "OTP Sent to your registered mobile No. if anything issue please condact erp admin.",
                                    // redirectUrl = Url.Action("Index", "Account") // Stay on page
                                });
                            }

                            await SetUserSession(objAccount, resultArgs, Latitude, Longitude, UserAgent, IPAddress, Place, collection);
                              sUserRoles = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString() : string.Empty;
                           sUserRoles = sUserRoles.Replace(" ", string.Empty);
                            return Json(new
                            {
                                success = true,
                                ErrorMessage = "Success",
                                redirectUrl = Url.Action("DashBoard", "DashBoard")
                                //redirectUrl = Url.Action(sUserRoles, "DashBoard")
                                });
                        }
                        else
                        {
                            LogLoginAttempt(collection.objLoginViewModel.Username, "FAILED", Latitude, Longitude, UserAgent, IPAddress, Place);

                            return Json(new
                            {
                                success = false,
                                ErrorMessage = Common.Messages.InvalidUserCredentials,
                                redirectUrl = Url.Action("Index", "Account")
                            });

                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            ErrorMessage = Common.Messages.InvalidUserCredentials,
                            redirectUrl = Url.Action("Index", "Account")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AccountController", "Login", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Login", ex.Message);
                }
                return Json(new
                {
                    success = false,
                    ErrorMessage = ex.Message,
                    redirectUrl = Url.Action("Index", "Account")
                });
            }

        //return RedirectToAction("Index");
        }

        private async Task SetUserSession(AccountViewModel objAccount, ResultArgs resultArgs, string Latitude, string Longitude, string UserAgent, string IPAddress, string Place, AccountViewModel collection)
        {
            DataValue dv = new DataValue();
            string sUserRoles = string.Empty;
            string sUserRoleId = string.Empty;

            // Fetch College Details ,...
            var liCollegeDetails = await Task.Run(() => (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data);
            Session[Common.SESSION_VARIABLES.COLLEGE_DETAILS] = liCollegeDetails;

            FormsAuthentication.SetAuthCookie(resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID].ToString(), false);
            Session[Common.SESSION_VARIABLES.COLLEGE_INFO] = objAccount.FetchCollegeInfo().DataSource.Table;
            //new AuthorizeAttribute().Roles = "SuperAdmin";
            var activeSemesterType = await Task.Run(() => (List<SUP_SEMESTER_TYPE>)DAO.CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.GetActiveSemesterType)).DataSource.Data);
            if (activeSemesterType != null && activeSemesterType.Count() > 0)
            {
                Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] = activeSemesterType.FirstOrDefault().SEMESTER_TYPE_ID;
            }
            DataTable dtActiveYear = new DataTable();
            dtActiveYear = await Task.Run(() => objAccount.FetchActiveYear().DataSource.Table);
            Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
            Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_NAME] = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.ACADEMIC_NAME].ToString();

            // need to store semester id 
            Session[Common.SESSION_VARIABLES.USER_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID];
            Session[Common.SESSION_VARIABLES.USERNAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.NAME];
            Session[Common.SESSION_VARIABLES.USER_CODE] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USERNAME];
            Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_ROLES_RIGHTS.ROLE_NAME];
            Session[Common.SESSION_VARIABLES.USER_ROLE_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.ROLE];
            Session[Common.SESSION_VARIABLES.USER_TYPE] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_TYPE_ID];
            Session[Common.SESSION_VARIABLES.PHOTO] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.PHOTO];
            Session[Common.SESSION_VARIABLES.CONTROLLER_NAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.CONTROLLER_NAME];
            Session[Common.SESSION_VARIABLES.ACTION_NAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.ACTION_NAME];
            sUserRoles = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString() : string.Empty;
            sUserRoles = sUserRoles.Replace(" ", string.Empty);

            var currenrDayOrder = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null,
                SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchDayOrderInfoByCurrentDate), dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString()).DataSource.Data;
            var activeSemesterForStaff = (List<SemesterRootInfo>)CMSHandler.FetchData<SemesterRootInfo>(null,
                SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveSemesterForStaffByAcademicYear), dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString()).DataSource.Data;
            if (activeSemesterForStaff != null)
                Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] = activeSemesterForStaff.FirstOrDefault().SEMESTER;

            if (currenrDayOrder != null)
            {
                Session[Common.SESSION_VARIABLES.DAY_ORDER] = (currenrDayOrder.FirstOrDefault().DAY_ORDER != null) ? currenrDayOrder.FirstOrDefault().DAY_ORDER : string.Empty;
                Session[Common.SESSION_VARIABLES.DAY_ORDER_DATE] = currenrDayOrder.FirstOrDefault().DAY_ORDER_DATE;
                Session[Common.SESSION_VARIABLES.IS_HOLIDAY] = currenrDayOrder.FirstOrDefault().IS_HOLIDAY;
            }
            else
            {
                Session[Common.SESSION_VARIABLES.DAY_ORDER] = "0";
                Session[Common.SESSION_VARIABLES.DAY_ORDER_DATE] = "";
                Session[Common.SESSION_VARIABLES.IS_HOLIDAY] = "0";
            }

            if (!string.IsNullOrEmpty(sUserRoles))
            {
                sUserRoleId = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.ROLE].ToString();
                dv.Clear();
                dv.Add(Common.USER_ROLES_RIGHTS.ROLE_ID, sUserRoleId, EnumCommand.DataType.String);
                dv.Add(Common.USER_ROLES_RIGHTS.USER_ID, resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_ID].ToString(), EnumCommand.DataType.String);
                dv.Add(Common.USER_ROLES_RIGHTS.USER_TYPE, resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USER_TYPE_ID].ToString(), EnumCommand.DataType.String);
                dv.Add(Common.USER_ROLES_RIGHTS.ACADEMIC_YEAR, dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString(), EnumCommand.DataType.String);
                resultArgs = await Task.Run(() => objAccount.FetchMenuItemsByRole(dv));
                Session[Common.SESSION_VARIABLES.MENU_ITEMS] = resultArgs.DataSource.Table;
            }
            using (LoginLog objHandler = new LoginLog())
            {
                objHandler.WriteUerDetails((Session[Common.SESSION_VARIABLES.USERNAME] != null) ? Session[Common.SESSION_VARIABLES.USERNAME].ToString() : "", collection.objLoginViewModel.Username);
            }
            if (sUserRoles.ToUpper().Equals("STUDENT"))
            {
                StuPersonalInfo studentPersonal = new StuPersonalInfo();
                studentPersonal.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                var activeSemesterForStudent = await Task.Run(() => (List<SemesterRootInfo>)CMS.DAO.CMSHandler.FetchData<SemesterRootInfo>(studentPersonal, CMS.SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchActiveSemesterForStudentByAcademicYear), dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString()).DataSource.Data);
                if (activeSemesterForStudent != null)
                    Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STUDENT] = activeSemesterForStudent.FirstOrDefault().SEMESTER;
            }
            LogLoginAttempt(collection.objLoginViewModel.Username, "SUCCESS", Latitude, Longitude, UserAgent ?? Request.UserAgent, IPAddress ?? Request.UserHostAddress, Place);

        }

        public async Task<ActionResult> VerifyLoginOTP(string sOTP)
        {
            try
            {
                if (Session["OTP"] != null && Session["OTP"].ToString() == sOTP)
                {
                    if (Session["TempUser"] != null)
                    {
                        var resultArgs = (ResultArgs)Session["TempUser"];
                        AccountViewModel collection = new AccountViewModel(); // Dummy or retrieve if stored
                        collection.objLoginViewModel = new LoginViewModel(); // Dummy
                        // You probably want to store the username too if LogLoginAttempt needs it, 
                        // or retrieve it from resultArgs
                        string username = resultArgs.DataSource.Table.Rows[0][Common.USER_ACCOUNT.USERNAME].ToString();
                        collection.objLoginViewModel.Username = username;

                        using (AccountViewModel objAccount = new AccountViewModel())
                        {
                            await SetUserSession(objAccount, resultArgs, "", "", "", "", "", collection); // Pass empty string for lat/long if not available or store them in session too
                        }

                        string sUserRoles = (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString() : string.Empty;
                        sUserRoles = sUserRoles.Replace(" ", string.Empty);

                        Session["OTP"] = null;
                        Session["TempUser"] = null;

                        return Json(new
                        {
                            success = true,
                            ErrorMessage = "Success",
                            redirectUrl = Url.Action(sUserRoles, "DashBoard")
                        });
                    }
                }
                
                return Json(new
                {
                    success = false,
                    ErrorMessage = "Invalid OTP",
                });
            }
             catch (Exception ex)
            {
                 using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AccountController", "VerifyLoginOTP", "Err MSg: " + ex.Message);
                }
                return Json(new
                {
                    success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// Store Login Histroy IP,location
        /// made by [Yesudass D], Date: [18:06:2025]
        /// </summary>
        /// <author>[Yesudass]</author>
        /// <param name="username"></param>
        /// <param name="status"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="agent"></param>
        /// <param name="ip"></param>
        /// <param name="Place"></param>

        public void LogLoginAttempt(string username, string status, string lat, string lng, string agent, string ip, string Place)
        {
            try
            {
                
                var FetchActiveYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, UserAccountSQL.GetSupportDataSQL(UserAccountSQLCommands.FetchActiveYear)).DataSource.Data;
                if (FetchActiveYear != null && FetchActiveYear.Count > 0)
                {
                    string sAcademicYear = FetchActiveYear.FirstOrDefault().AC_YEAR;
                    IP_ADDRESS_LOG log = new IP_ADDRESS_LOG
                    {
                        IP_ADDRESS = ip ?? Request.UserHostAddress,
                        USERNAME = username,
                        STATUS = status,
                        USER_AGENT = agent ?? Request.UserAgent,
                        LATITUDE = lat,
                        LONGITUDE = lng,
                        PLACE = Place,
                        LOGTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    ResultArgs logResult = CMSHandler.SaveOrUpdate(log, AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserLoginDetection), sAcademicYear);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AccountController", "Login", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Login", ex.Message);
                }
                TempData["ErrorMessage"] = ex.Message;
            }
        }
        public string GetPlaceFromCoordinates(string lat, string lng)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0");
                    string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lng}&zoom=10";
                    var json = client.DownloadString(url);
                    dynamic result = JsonConvert.DeserializeObject(json);
                    return result.display_name;
                }
            }
            catch
            {
                return "Unknown";
            }
        }
        #endregion

        #region Change Password
        public ActionResult ChangePassword()
        {
            return View();
        }
        public JsonResult EditPassword(string ChangePassword)
        {
            string sResult = string.Empty;
            var ChangePasswords = new ChangePassword();
            var Result = JsonConvert.DeserializeObject<ChangePassword>(ChangePassword);
            Result.USERNAME = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
            Result.CONFIRM_PASSWORD = CommonMethods.GetSha256FromString(Result.CONFIRM_PASSWORD);
            var FetchPassword = (List<ChangePassword>)DAO.CMSHandler.FetchData<ChangePassword>(Result, AccountSQL.GetAccountSQL(DAO.AccountSQLCommands.FetchPassword)).DataSource.Data;
            var UserPassword = FetchPassword.Select(x => x.PASSWORD).SingleOrDefault();
            if (UserPassword == (CommonMethods.GetSha256FromString(Result.PASSWORD)))
            {
                ResultArgs resultArgs = new ResultArgs();
                resultArgs = DAO.CMSHandler.SaveOrUpdate(Result, AccountSQL.GetAccountSQL(DAO.AccountSQLCommands.UpdatePassword));
                if (resultArgs.Success)
                {
                    sResult = "Password Changed Successfully...!";

                    // Log the password change
                    try
                    {
                        var userAccountInfo = new USER_ACCOUNT_INFO();
                        userAccountInfo.USERNAME = Result.USERNAME;
                        if (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null)
                            userAccountInfo.ROLE = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                        if (!string.IsNullOrEmpty(userAccountInfo.ROLE))
                        {
                            USER_CHANGE_PASSWORD log = new USER_CHANGE_PASSWORD();
                            log.USERNAME = Result.USERNAME;
                            log.PASSWORD = CommonMethods.GetSha256FromString(Result.PASSWORD);

                            if (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null)
                                log.ROLE = Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString();
                            else if (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null)
                                log.ROLE = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                            if (Session[Common.SESSION_VARIABLES.USERNAME] != null)
                                log.NAME = Session[Common.SESSION_VARIABLES.USERNAME].ToString();

                            log.LAST_LOGIN = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

                            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                                log.USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                            if (Session[Common.SESSION_VARIABLES.USER_TYPE] != null)
                                log.USER_TYPE = Session[Common.SESSION_VARIABLES.USER_TYPE].ToString();

                            log.DATE = DateTime.Now.ToString("yyyy/MM/dd");
                            log.TIME = DateTime.Now.ToString("HH:mm:ss");
                           
                            CMSHandler.SaveOrUpdate(log, AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserChangePasswordLog));

                        }
                    }

                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("AccountController", "EditPassword-Log", "Err MSg: " + ex.Message);
                        }
                    }

                }
                else
                {
                    sResult = "Failed to Update Password...!";
                }
            }
            return Json(sResult);
        }

        public JsonResult SaveChangePassword(string ChangePassword)
        {
            string sResult = string.Empty;
            try
            {
                var ChangePasswords = new ChangePassword();
                var Result = JsonConvert.DeserializeObject<ChangePassword>(ChangePassword);
                //Result.USERNAME = Session[Common.SESSION_VARIABLES.USER_CODE].ToString();
                Result.CONFIRM_PASSWORD = CommonMethods.GetSha256FromString(Result.CONFIRM_PASSWORD);
                ResultArgs resultArgs = new ResultArgs();
                resultArgs = DAO.CMSHandler.SaveOrUpdate(Result, AccountSQL.GetAccountSQL(DAO.AccountSQLCommands.UpdatePassword));
                if (resultArgs.Success)
                {
                    sResult = "Password Changed Successfully...!";

                    // Log the password change
                    try
                    {
                        var userAccountInfo = new USER_ACCOUNT_INFO();
                        userAccountInfo.USERNAME = Result.USERNAME;

                        if (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null)
                            userAccountInfo.ROLE = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";

                        if (!string.IsNullOrEmpty(userAccountInfo.ROLE))
                        {
                            USER_CHANGE_PASSWORD log = new USER_CHANGE_PASSWORD();
                            log.USERNAME = Result.USERNAME;
                            log.PASSWORD = CommonMethods.GetSha256FromString(Result.PASSWORD);

                            if (Session[Common.SESSION_VARIABLES.USER_ROLE_NAME] != null)
                                log.ROLE = Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString();
                            else if (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null)
                                log.ROLE = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                            if (Session[Common.SESSION_VARIABLES.USERNAME] != null)
                                log.NAME = Session[Common.SESSION_VARIABLES.USERNAME].ToString();

                            log.LAST_LOGIN = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

                            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                                log.USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                            if (Session[Common.SESSION_VARIABLES.USER_TYPE] != null)
                                log.USER_TYPE = Session[Common.SESSION_VARIABLES.USER_TYPE].ToString();

                            log.DATE = DateTime.Now.ToString("yyyyMMdd");
                            log.TIME = DateTime.Now.ToString("HHmmss");

                            CMSHandler.SaveOrUpdate(log, AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserChangePasswordLog));
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("AccountController", "SaveChangePassword-Log", "Err MSg: " + ex.Message);
                        }
                    }

                }
                else
                {

                    sResult = "Failed to Update Password...!";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AccountController", "SaveChangePassword", ex.Message);
                }
            }
            if (string.IsNullOrEmpty(sResult))
            {
                sResult = "No Changes Made !";
            }
            return Json(sResult);
        }
        #endregion

        #region Forgot Password
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public JsonResult SendMail(string sMobileNo, string sMailId, string sStudentId)
        {
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sMailId))
            {
                try
                {
                    string sOTP = CommonMethods.GetRandomPasswordNumber();
                    bool sResult = CommonMethods.SendMailFromGmailSMTP("", "", sMailId, "Verification Code Form ERP Team", sOTP);
                    if (sResult)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        ObjJsonData.Message = "verification Code Sent To Your Mail";
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = "Sorry ! Failed To Send Verification Code  .!";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "SendMail" +
                            "", ex.Message);
                    }
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateForgotPassword(string sUsername, string sDOB)
        {
            var sJsonResultData = new JsonResultData();
            sDOB = CommonMethods.ConvertdatetoyearFromat(sDOB);
            if (string.IsNullOrEmpty(sDOB))
            {
                sJsonResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                sJsonResultData.Message = "Invalid Date";
                return Json(sJsonResultData, JsonRequestBehavior.AllowGet);
            }
            //Session[Common.SESSION_VARIABLES.USER_CODE] = sUsername;
            var liValidate = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(new ForgotPassword() { USERNAME = sUsername, DATE_OF_BIRTH = sDOB }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserCredentialsForgetPassword)).DataSource.Data;
            if (liValidate != null && liValidate.Count > 0)
                return Json(liValidate, JsonRequestBehavior.AllowGet);
            else
                sJsonResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            return Json(sJsonResultData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendSMS(string sMobileNo, string sMailId, string sStudentId)
        {
            var ObjJsonData = new JsonResultData();
            string sOTP = CommonMethods.GetRandomPasswordNumber();
            if (!string.IsNullOrEmpty(sOTP))
            {
                // Session[Common.SESSION_VARIABLES.OTP] = sOTP;
                var liSMS = new List<MessageResult.sms>();
                liSMS.Add(new MessageResult.sms() { to = sMobileNo, message = "Use " + sOTP + " as ERP account password reset code ", custom = sStudentId, custom1 = "0", custom2 = "0", CreditCount = "1" });
                //SendMessage();
                //return RedirectToAction("SendMessage", "Communication");
                var ObjControllerModel = new CommunicationController();
                var livendordetails = (List<SMS_VENDOR_DETAILS>)CMSHandler.FetchData<SMS_VENDOR_DETAILS>(null, SQL.Communication.CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchActiveSMSvendors)).DataSource.Data;
                if (livendordetails != null && livendordetails.Count > 0)
                {
                    if (livendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.Solutioninfini)
                    {
                        string sContent = "Use " + sOTP + " as ERP account password reset code";
                        ObjControllerModel.sSmsMode = Common.MessageType.OTP;
                        int length = Convert.ToInt32(sContent.Length);

                        string sCount = CommonMethods.ValidateCreditCount(length);
                        if (sCount == "0")
                            sCount = "1";
                        ObjControllerModel.sCreditCount = sCount;

                        ObjControllerModel.liSMS = liSMS;
                        ObjControllerModel.sSmsCount = 1;
                        ObjControllerModel.SendMessage();
                    }

                    if (livendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.DoveSoft)
                    {
                        ObjControllerModel.sTextMessage = "Use " + sOTP + " as ERP account password reset code";
                        ObjControllerModel.sReceipientNo = sMobileNo;
                        ObjControllerModel.SendSMSFromDovesoft();
                    }
                }

                if (ObjControllerModel.sResultMessage == "200")
                {
                    ObjJsonData.Message = "verification Code Sent To Your Mobile Number";
                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    ObjJsonData.sResult = sOTP;
                }
                else
                {
                    ObjJsonData.Message = "Sorry ! Failed To Sent Messages .!";
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Account Creation
        public ActionResult ListUserAccount()
        {
            AccountViewModel objViewModel = new AccountViewModel();
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var FetchRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                    objViewModel.SupRoleList = new SelectList(FetchRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListUserAccount", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("ListUserAccount", "AccountController", Ex.Message);
                    }

                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(objViewModel);
        }
        public ActionResult BindUseAccountInfo(string RoleId)
        {
            AccountViewModel objViewModel = new AccountViewModel();
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    objViewModel.ListUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(new USER_ACCOUNT_INFO() { ROLE = RoleId }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserAccountByRoleNew), sAcademicYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("BindUseAccountInfo", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("BindUseAccountInfo", "AccountController", Ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public ActionResult BindUseAccountInfobyuserName(string Username, string roleId)
        {
            AccountViewModel objViewModel = new AccountViewModel();
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    objViewModel.ListUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(new USER_ACCOUNT_INFO() { ROLE = roleId }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserAccountByUserNameForUpdate).Replace("?USERNAME", Username), sAcademicYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("BindUseAccountInfo", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("BindUseAccountInfo", "AccountController", Ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }

        public JsonResult FetchUsersAndUserType()
        {
            JsonResultData objResult = new JsonResultData();
            string sRole = string.Empty;
            string sUserType = string.Empty;
            string sUsers = string.Empty;
            string JsonData = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole)).DataSource.Data;
                    var FetchUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserType), sAcademicyear).DataSource.Data;
                    var FetchUsers = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUsers), sAcademicyear).DataSource.Data;
                    if (FetchRole != null && FetchRole.Count > 0)
                    {
                        foreach (var item in FetchRole)
                            sRole += "<option value='" + item.ROLE_ID + "'>" + item.ROLE_NAME + "</option>";
                    }
                    if (FetchUserType != null && FetchUserType.Count > 0)
                    {
                        foreach (var item in FetchUserType)
                            sUserType += "<option value='" + item.USER_TYPE_ID + "'>" + item.USER_TYPE_NAME + "</option>";
                    }
                    if (FetchUsers != null && FetchUsers.Count > 0)
                    {
                        foreach (var item in FetchUsers)
                            sUsers += "<option value='" + item.STUDENT_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchUsersAndUserType", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchUsersAndUserType", "AccountController", Ex.Message);
                }
            }
            var objJsonData = new USER_ACCOUNT_INFO() { ROLE = sRole, USER_TYPE = sUserType, USER_ID = sUsers };
            JsonData = JsonConvert.SerializeObject(objJsonData);
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveUserAccount(string sUserAccount)
        {
            JsonResultData objResult = new JsonResultData();
            SUP_BANK objModel = new SUP_BANK();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (!string.IsNullOrEmpty(sUserAccount))
                    {
                        var AccountDetails = JsonConvert.DeserializeObject<USER_ACCOUNT_INFO>(sUserAccount);
                        if (AccountDetails != null)
                        {
                            AccountDetails.PASSWORD = CommonMethods.GetSha256FromString(AccountDetails.PASSWORD);
                            //var FetchExistingUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(AccountDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchExistingUserAccount), sAcademicyear).DataSource.Data;
                            if (!string.IsNullOrEmpty(AccountDetails.USER_ACCOUNT_ID) && AccountDetails.USER_ACCOUNT_ID != "undefined")
                            {

                                var UpdateUserAccount = CMSHandler.SaveOrUpdate(AccountDetails, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.UpdateUserAccountDetails));
                                if (UpdateUserAccount.Success && UpdateUserAccount.RowsAffected != 0)
                                {
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    objResult.Message = Common.Messages.FailedToUpdatedRecords;
                                }
                            }
                            else
                            {

                                var FetchStaffUserName = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(AccountDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchStaffNameByUserIdAndUserType), sAcademicyear).DataSource.Data;
                                var FetchUserNameExist = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(AccountDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.CheckUsernameExistOrNot), sAcademicyear).DataSource.Data;
                                if (FetchUserNameExist == null)
                                {
                                    var FetchUserID = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(AccountDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.CheckValidUsername), sAcademicyear).DataSource.Data;
                                    if (FetchUserID != null && FetchUserID.Count > 0)
                                    {
                                        AccountDetails.USER_ID = FetchUserID.FirstOrDefault().USER_ID;

                                        var result = CMSHandler.SaveOrUpdate(AccountDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserAccount));
                                        if (result.Success && result.RowsAffected != 0)
                                        {
                                            var Rresult = CMSHandler.SaveOrUpdate(AccountDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserRolesByAcYear), sAcademicyear);

                                            objResult.ErrorCode = Common.ErrorCode.Ok;
                                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                        }
                                    }
                                    else
                                    {
                                        objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                        objResult.Message = Common.Messages.AccountNotAvailable;
                                    }
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    objResult.Message = Common.Messages.RecordsExistAlready;
                                }
                            }
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                            objResult.Message = "Something went wrong . Please meet ERP Admin !";
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objResult.Message = "Something went wrong . Please meet ERP Admin !";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveUserAccount", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveUserAccount", "AccountController", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchUserAccount(string sUserAccountId)
        {
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sUserAccountId))
            {
                var liUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(new USER_ACCOUNT() { USER_ACCOUNT_ID = sUserAccountId }, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserAccountById)).DataSource.Data;
                if (liUserAccount != null && liUserAccount.Count > 0)
                {
                    return Json(liUserAccount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = "Something went wrong . please meet ERP Admin !";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUserAccount(string sUserAccountId)
        {
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sUserAccountId))
            {
                var DeleteUserAccount = CMSHandler.SaveOrUpdate(new USER_ACCOUNT() { USER_ACCOUNT_ID = sUserAccountId }, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.DeleteUserAccount));
                if (DeleteUserAccount.Success && DeleteUserAccount.RowsAffected != 0)
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = "Something went wrong . please meet ERP Admin !";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  User roles by AcdemicYear
        public ActionResult LiUserRolesByAcYear()
        {
            AccountViewModel objViewModel = new AccountViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var liacyear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), null).DataSource.Data;
                    objViewModel.AcademicYearList = new SelectList(liacyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    var FetchRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), null).DataSource.Data;
                    objViewModel.SupRoleList = new SelectList(FetchRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                    //var Fetchuser = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUsers), sAcademicYear).DataSource.Data;
                    var Fetchuser = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(null, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUsersfromuseraccount), sAcademicYear).DataSource.Data;
                    if (Fetchuser != null)
                    {
                        objViewModel.userList = new SelectList(Fetchuser, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                    }
                    var FetchUserType = (List<SUP_USER_TYPE>)CMSHandler.FetchData<SUP_USER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupUserType), null).DataSource.Data;
                    objViewModel.SupUsertypeList = new SelectList(FetchUserType, Common.SUP_USER_TYPE.USER_TYPE_ID, Common.SUP_USER_TYPE.USER_TYPE_NAME);


                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LiUserRolesRight", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("LiUserRolesRight", "AccountController", Ex.Message);
                    }

                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(objViewModel);
        }
        public string BindUserbyrole(string sRoleId)
        {
            string sOption = string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchUsers = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(new USER_ACCOUNT { ROLE = sRoleId }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUsersByRole), sAcademicyear).DataSource.Data;
                    if (FetchUsers != null && FetchUsers.Count > 0)
                    {
                        foreach (var item in FetchUsers)
                            sOption += "<option value='" + item.STUDENT_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout;
                    sOption = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindUserbyrole", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("BindUserbyrole", "AccountController", Ex.Message);
                }
            }
            return sOption;
        }
        public string BindUsersbyrole(string sRoleId)
        {
            string sOption = string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchUsers = (List<STUDENT_PERSONAL_INFO>)CMSHandler.FetchData<STUDENT_PERSONAL_INFO>(null, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUsersfromuseraccount), null).DataSource.Data;
                    if (FetchUsers != null && FetchUsers.Count > 0)
                    {
                        foreach (var item in FetchUsers)
                            sOption += "<option value='" + item.STUDENT_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout;
                    sOption = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindUserbyrole", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("BindUserbyrole", "AccountController", Ex.Message);
                }
            }
            return sOption;
        }
        public ActionResult BindUserRolesFromUSerRolebyACYear(string sUserid, string sAcyear, string sRoleid)
        {
            AccountViewModel objViewModel = new AccountViewModel();
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = sAcyear;
            // string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    //objViewModel.ListUserRolesByYear = (List<USER_ROLES_BY_ACADEMIC_YEAR>)CMSHandler.FetchData<USER_ROLES_BY_ACADEMIC_YEAR>(new USER_ROLES_BY_ACADEMIC_YEAR() { USER_ID = sUserid, ROLE = sRoleid }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserAccountByUserid), sAcademicYear).DataSource.Data;
                    objViewModel.ListUserRolesByYear = (List<USER_ROLES_BY_ACADEMIC_YEAR>)CMSHandler.FetchData<USER_ROLES_BY_ACADEMIC_YEAR>(new USER_ROLES_BY_ACADEMIC_YEAR() { USERNAME = sUserid, ROLE = sRoleid }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserAccountByUserName), sAcademicYear).DataSource.Data;
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("BindUserRolesbyyear", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("BindUserRolesbyyear", "AccountController", Ex.Message);
                    }

                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return View(objViewModel);
        }
        public JsonResult SaveUserRole(string sJson)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    // string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (!string.IsNullOrEmpty(sJson))
                    {
                        var UserRoleDetails = JsonConvert.DeserializeObject<USER_ROLES_BY_ACADEMIC_YEAR>(sJson);
                        string sAcademicyear = UserRoleDetails.ACADEMIC_YEAR;
                        if (UserRoleDetails != null)
                        {

                            var FetchExistingUserAccount = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(UserRoleDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchExistingUserRolesbyUsername), sAcademicyear).DataSource.Data;
                            if (FetchExistingUserAccount == null)
                            {
                                if (!string.IsNullOrEmpty(UserRoleDetails.USER_ROLE_ID))
                                {
                                    var UpdateUserRoles = CMSHandler.SaveOrUpdate(UserRoleDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.UpdateUserRoles), sAcademicyear);
                                    if (UpdateUserRoles.Success && UpdateUserRoles.RowsAffected != 0)
                                    {
                                        objResult.ErrorCode = Common.ErrorCode.Ok;
                                        objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;
                                    }
                                    else
                                    {
                                        objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                        objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                                    }
                                }
                                else
                                {
                                    var username = UserRoleDetails.USERNAME;
                                    var FetchExistUsername = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(UserRoleDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserIdbyUserName), sAcademicyear).DataSource.Data;
                                    if (FetchExistUsername != null)
                                    {
                                        UserRoleDetails.USER_ID = FetchExistUsername.FirstOrDefault().USER_ID;
                                        var result = CMSHandler.SaveOrUpdate(UserRoleDetails, AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserRolesByAcYear), sAcademicyear);
                                        if (result.Success && result.RowsAffected != 0)
                                        {
                                            objResult.ErrorCode = Common.ErrorCode.Ok;
                                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                        }
                                        else
                                        {
                                            objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                            objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                objResult.Message = "This User & Role Already Exist!";
                            }

                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                            objResult.Message = "Something went wrong . Please meet ERP Admin !";
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                        objResult.Message = "Something went wrong . Please meet ERP Admin !";
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveUserRole", "AccountController", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveUserRole", "AccountController", Ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchUserRole(string sUserRoleId)
        {
            var ObjJsonData = new JsonResultData();
            string sRole = string.Empty;
            string sUserType = string.Empty;
            string sUsers = string.Empty;

            if (!string.IsNullOrEmpty(sUserRoleId))
            {
                var liUserrole = (List<USER_ROLES_BY_ACADEMIC_YEAR>)CMSHandler.FetchData<USER_ROLES_BY_ACADEMIC_YEAR>(new USER_ROLES_BY_ACADEMIC_YEAR() { USER_ROLE_ID = sUserRoleId }, AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserRolesById)).DataSource.Data;
                if (liUserrole != null && liUserrole.Count > 0)
                {
                    return Json(liUserrole, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = "Something went wrong . please meet ERP Admin !";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteUserRole(string sUserRoleId)
        {
            var ObjJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sUserRoleId))
            {
                var DeleteUserAccount = CMSHandler.SaveOrUpdate(new USER_ROLES_BY_ACADEMIC_YEAR() { USER_ROLE_ID = sUserRoleId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Account.AccountSQL.GetAccountSQL(AccountSQLCommands.DeleteUserRoles));
                if (DeleteUserAccount.Success && DeleteUserAccount.RowsAffected != 0)
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = "Something went wrong . please meet ERP Admin !";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BulkMoveUserRoles(string sFromYear, string sToYear)
        {
            try { System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\debug.txt", "Hit! From: " + sFromYear + ", To: " + sToYear); } catch {}
            var objResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null)
            {
                objResult.ErrorCode = Common.ErrorCode.ServiceUnavailable;
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(objResult);
            }

            if (string.IsNullOrEmpty(sFromYear) || string.IsNullOrEmpty(sToYear))
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = "From and To Academic Years are required.";
                return Json(objResult);
            }

            if (sFromYear == sToYear)
            {
                objResult.ErrorCode = Common.ErrorCode.BadRequest;
                objResult.Message = "From and To Academic Years cannot be the same.";
                return Json(objResult);
            }

            try
            {
                // 1. Fetch all roles from the source year
                var fromRoles = (List<USER_ROLES_BY_ACADEMIC_YEAR>)CMSHandler.FetchData<USER_ROLES_BY_ACADEMIC_YEAR>(
                    new USER_ROLES_BY_ACADEMIC_YEAR() { ACADEMIC_YEAR = sFromYear }, 
                    AccountSQL.GetAccountSQL(AccountSQLCommands.FetchUserRolesByAcademicYear), 
                    sFromYear).DataSource.Data;

                if (fromRoles == null || fromRoles.Count == 0)
                {
                    objResult.ErrorCode = Common.ErrorCode.NotFound;
                    objResult.Message = "No user roles found in the source Academic Year " + sFromYear;
                    return Json(objResult);
                }

                int movedCount = 0;
                int skippedCount = 0;

                foreach (var item in fromRoles)
                {
                    // 2. Check if this record already exists in the target year
                    var checkObj = new USER_ROLES_BY_ACADEMIC_YEAR()
                    {
                        USER_ID = item.USER_ID,
                        ROLE_ID = item.ROLE_ID,
                        USER_TYPE = item.USER_TYPE
                    };

                    var existList = (List<USER_ROLES_BY_ACADEMIC_YEAR>)CMSHandler.FetchData<USER_ROLES_BY_ACADEMIC_YEAR>(
                        checkObj, 
                        AccountSQL.GetAccountSQL(AccountSQLCommands.FetchExistingUserRoles), 
                        sToYear).DataSource.Data;

                    if (existList == null || existList.Count == 0)
                    {
                        // 3. Save role to target year if it doesn't exist
                        var result = CMSHandler.SaveOrUpdate(
                            checkObj, 
                            AccountSQL.GetAccountSQL(AccountSQLCommands.SaveUserRolesByAcYear), 
                            sToYear);

                        if (result.Success)
                        {
                            movedCount++;
                        }
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                objResult.ErrorCode = Common.ErrorCode.Ok;
                objResult.Message = string.Format("Bulk move completed. {0} roles moved, {1} duplicate roles skipped.", movedCount, skippedCount);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BulkMoveUserRoles", "AccountController", "Err MSg: " + ex.Message);
                }
                objResult.ErrorCode = Common.ErrorCode.InternalServerError;
                objResult.Message = "Error occurred during bulk move: " + ex.Message;
            }

            return Json(objResult);
        }
        #endregion

        #region Menu Configuration
        public ActionResult ListMenuConfiguration()
        {
            SupportDataViewModel objViewModel = new SupportDataViewModel();
            string sAcademiYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    // Fetch Modules
                    objViewModel.liModules = (List<MODULES>)CMSHandler.FetchData<MODULES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchModules), sAcademiYear).DataSource.Data;
                    if (objViewModel.liModules != null && objViewModel.liModules.Count > 0)
                    {
                        objViewModel.ModuleList = new SelectList(objViewModel.liModules, Common.MODULES.MODULES_ID, Common.MODULES.MODULE_NAME);
                    }

                    // Fetch Menu Items
                    objViewModel.liMenuItems = (List<MENU_ITEMS>)CMSHandler.FetchData<MENU_ITEMS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMenuItems), sAcademiYear).DataSource.Data;
                    if (objViewModel.liMenuItems != null && objViewModel.liMenuItems.Count > 0)
                    {
                        objViewModel.MenuItemList = new SelectList(objViewModel.liMenuItems, Common.MENU_ITEMS.MENU_ID, Common.MENU_ITEMS.MENU_NAME);
                    }

                    // Fetch User Roles & Rights
                    objViewModel.liUserRolesRights = (List<USER_ROLES_RIGHTS>)CMSHandler.FetchData<USER_ROLES_RIGHTS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesRights), sAcademiYear).DataSource.Data;

                    // Fetch Roles
                    var liSupRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademiYear).DataSource.Data;
                    if (liSupRole != null && liSupRole.Count > 0)
                    {
                        objViewModel.RoleList = new SelectList(liSupRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                    }
                }
                catch (Exception Ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AccountController", "ListMenuConfiguration", "Err MSg: " + Ex.Message, "Query is empty!");
                        objHandler.WriteError("AccountController", "ListMenuConfiguration", Ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        public JsonResult FetchModuleById(string sModuleId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sModuleId))
                {
                    objResultData.oResult = (List<MODULES>)CMSHandler.FetchData<MODULES>(new MODULES() { MODULES_ID = sModuleId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchModuleById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateModule(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<MODULES>(sJsonData);
                    if (result != null)
                    {
                        if (result.MODULES_ID != null && result.MODULES_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateModuleById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveModule), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "SaveorUpdateModule", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult DeleteModuleById(string sModuleId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sModuleId))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(new MODULES() { MODULES_ID = sModuleId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteModuleById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "DeleteModuleById", ex.Message);
                    }
                }
            }
            return Json(objResultData);
        }

        public JsonResult FetchMenuItemById(string sMenuId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sMenuId))
                {
                    objResultData.oResult = (List<MENU_ITEMS>)CMSHandler.FetchData<MENU_ITEMS>(new MENU_ITEMS() { MENU_ID = sMenuId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMenuItemById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateMenuItem(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<MENU_ITEMS>(sJsonData);
                    if (result != null)
                    {
                        if (result.MENU_ID != null && result.MENU_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateMenuItemById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveMenuItem), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "SaveorUpdateMenuItem", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult DeleteMenuItemById(string sMenuId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sMenuId))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(new MENU_ITEMS() { MENU_ID = sMenuId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteMenuItemById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "DeleteMenuItemById", ex.Message);
                    }
                }
            }
            return Json(objResultData);
        }

        public JsonResult FetchUserRolesRightsById(string sUserRightsId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sUserRightsId))
                {
                    objResultData.oResult = (List<USER_ROLES_RIGHTS>)CMSHandler.FetchData<USER_ROLES_RIGHTS>(new USER_ROLES_RIGHTS() { USER_RIGHTS_ID = sUserRightsId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesRightsById), sAcademicYear).DataSource.Data;
                }
            }
            return Json(objResultData);
        }

        public JsonResult SaveorUpdateUserRolesRights(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<USER_ROLES_RIGHTS>(sJsonData);
                    if (result != null)
                    {
                        if (result.USER_RIGHTS_ID != null && result.USER_RIGHTS_ID != "")
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UpdateUserRolesRightsById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            var isExist = (List<USER_ROLES_RIGHTS>)CMSHandler.FetchData<USER_ROLES_RIGHTS>(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchUserRolesRightsByMenuItemIdandRoleId), sAcademicYear).DataSource.Data;
                            if (isExist != null && isExist.Count > 0)
                            {
                                objResultData.Message = Common.Messages.RecordsExistAlready;
                                objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                                return Json(objResultData);
                            }

                            resultArgs = CMSHandler.SaveOrUpdate(result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveUserRolesRights), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "SaveorUpdateUserRolesRights", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData);
        }

        public JsonResult DeleteUserRolesRightsById(string sUserRightsId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sUserRightsId))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(new USER_ROLES_RIGHTS() { USER_RIGHTS_ID = sUserRightsId, IS_DELETED = Common.DeleteFlag.Deleted }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.DeleteUserRolesRightsById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "DeleteUserRolesRightsById", ex.Message);
                    }
                }
            }
            return Json(objResultData);
        }

        public JsonResult ActivateorDeactivateUserRolesRightsById(string sUserRightsId, string sMode)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.CUR_ACADEMIC_YEAR].ToString() : (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty);
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sUserRightsId))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(new USER_ROLES_RIGHTS() { USER_RIGHTS_ID = sUserRightsId, IS_ACTIVE = sMode }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.ActivateorDeactivateUserRolesRightsById), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AccountController", "ActivateorDeactivateUserRolesRightsById", ex.Message);
                    }
                }
            }
            return Json(objResultData);
        }

        public ActionResult ListSemesterConfiguration()
        {
            return RedirectToAction("ListSemesterConfiguration", "SupportData");
        }
        #endregion
    }
}
