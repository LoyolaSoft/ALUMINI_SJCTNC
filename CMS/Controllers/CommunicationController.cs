using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Data;
using CMS.SQL.Communication;
using System.Text;
using CMS.SQL.SupportData;
using System.Net.Mail;
using CMS.SQL.Student;
using System.Globalization;
using Microsoft.Owin.Security.Twitter.Messages;
using static CMS.Utility.MessageResult;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CMS.Controllers
{
    public class CommunicationController : Controller
    {
        JsonResultData ObjJsonData = new JsonResultData();
        ResultArgs resultArgs = new ResultArgs();
        string sTemplateId = string.Empty;
        string sTransactionId = string.Empty;
        string sJsonString = string.Empty;
        string sBaseUrl = string.Empty;
        string sFormat = string.Empty;
        string sMethod = string.Empty;
        string sAPI = string.Empty;
        string sAPIKey = string.Empty;
        int nLogcount = 0;
        string sUniCode = string.Empty;
        public string sTextMessage { get; set; }
        Int32 nSMSTimeOut = 0;
        int iReceivedCredits = 0;
        int iSMSLimit = 0;
        int iSMSSent = 0;
        int iUsedCredit = 0;
        int iTotalSendCreditCount = 0;
        int Count = 0;
        string sStartTime { get; set; }
        public string sCreditCount { get; set; }
        public string sSender { get; set; }
        public string sUser { get; set; }
        public string sAccusage { get; set; }
        string sResult = string.Empty;
        public string sEntityId = string.Empty;
        public string sSmsMode = string.Empty;
        public string acdemicYear = string.Empty;
        public string sClassIds { get; set; }
        public string sReceipientNo { get; set; }
        public string sResultMessage { get; set; }
        public int sSmsCount = 0;
        public List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
        public List<MessageResult.listsms> listbulkSMS = new List<MessageResult.listsms>();
        List<SMS_SETTING> liSMSSetting = new List<SMS_SETTING>();
        CommunicationViewModel objViewModel = new CommunicationViewModel();
        // GET: Communication
        //public ActionResult Announcement()
        //{
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("api_key", "A5a9d2d947c4687184ebfdc6ac34b8b87");
        //    parameters.Add("method", "sms");
        //    parameters.Add("message", "Test+sms");
        //    parameters.Add("to", "9994954540");
        //    parameters.Add("sender", "HCCTRY");
        //    var response = JsonConvert.DeserializeObject<ResponseFromSolutionInfi>(CommonMethods.HttpPostMethod(@"https://alerts.solutionsinfini.com/api/v4/?", parameters).ToString());
        //    if (response!=null)
        //    {

        //    }
        //    return View();
        //}

        #region SMS
        public ActionResult GeneralMessage()
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    var Programme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme), sAcademicYear).DataSource.Data;
                    var StaffCategoryList = (List<Sup_SubCategory>)CMSHandler.FetchData<Sup_SubCategory>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSubCategory), sAcademicYear).DataSource.Data;
                    var StaffType = (List<SUP_STAFF_TYPE>)CMSHandler.FetchData<SUP_STAFF_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStaffType), sAcademicYear).DataSource.Data;
                    var RoleForStaff = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                    if (Programme != null && Programme.Count > 0)
                        objViewModel.Programme = new SelectList(Programme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    if (StaffType != null && StaffType.Count > 0)
                        objViewModel.StaffType = new SelectList(StaffType, Common.SUP_STAFF_TYPE.STAFF_TYPE, Common.SUP_STAFF_TYPE.STAFF_TYPE_NAME);
                    if (StaffCategoryList != null && StaffCategoryList.Count > 0)
                        objViewModel.StaffCategoryList = new SelectList(StaffCategoryList, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "GeneralMessage", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "GeneralMessage", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public JsonResult GetClassByProgrammeGroupId(string sProgrammeGroupId)
        {
            JsonResultData objResult = new JsonResultData();
            string sSQL = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sSQL = SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByProgrammeGroupId).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.PROGRAMME_GROUP_ID, sProgrammeGroupId);
                    var FetchClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, sSQL).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                    {
                        foreach (var item in FetchClass)
                            objResult.sResult += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
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
                    objHandler.WriteError("Fee", "GetClassByDepartmentIds", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Fee", "GetClassByDepartmentIds", Ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        // To send Test Message
        public JsonResult TestMessage(string JsonTestMessage, string sApIMode, string UniCode)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<TestSMS>(JsonTestMessage);
                    if (!string.IsNullOrEmpty(Result.CONTENT) && !string.IsNullOrEmpty(Result.RECEIPIENT_NOS))
                    {
                        sSmsMode = Common.MessageType.TEST;
                        sTextMessage = Result.CONTENT;
                        string sCount = ValidateCreditCount(sTextMessage.Length);
                        sCreditCount = sCount;
                        sUniCode = UniCode;
                        liSMS.Add(new MessageResult.sms() { to = Result.RECEIPIENT_NOS, message = Result.CONTENT, custom = "0", custom1 = "0", custom2 = "0" });
                        listbulkSMS.Add(new MessageResult.listsms() { mobiles = Result.RECEIPIENT_NOS, sms = Result.CONTENT });
                        SendMessage();
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "TestMessage", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "TestMessage", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }

            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Validate SMS Credit
        public string ValidateCreditCount(int CreditCount)
        {
            string sCount = string.Empty;
            if (CreditCount <= 160)
            {
                sCount = "1";
            }
            else if (CreditCount <= 306)
            {
                sCount = "2";
            }
            else if (CreditCount <= 459)
            {
                sCount = "3";
            }
            else if (CreditCount <= 612)
            {
                sCount = "4";
            }
            else if (CreditCount <= 765)
            {
                sCount = "5";
            }
            else if (CreditCount <= 918)
            {
                sCount = "6";
            }
            else
            {
                sCount = "7";
            }
            return sCount;
        }
        // Choose Option For Send Message ...
        public void SendMessage()
        {
            try
            {
                var liVendordetails = DapperHandler.FetchAll<SMS_VENDOR_DETAILS>(CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchActiveSMSvendors)).ToList();
                if (liVendordetails != null && liVendordetails.Count > 0)
                {
                    if (liVendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.Solutioninfini)
                    {
                        SendBulkSMSFromSolutionInfi();
                    }
                    else if (liVendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.DoveSoft)
                    {
                        SendBulkSMSFromDovesoft();
                    }
                }
                else
                {
                    resultArgs.Success = false;
                    resultArgs.Message = "SMS Vendors not found ...!";
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendMessage", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendMessage", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }

        }
        public void SendMessageV1()
        {
            try
            {
                var liVendordetails = DapperHandler.FetchAll<SMS_VENDOR_DETAILS>(CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchActiveSMSvendors)).ToList();
                if (liVendordetails != null && liVendordetails.Count > 0)
                {
                    if (liVendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.Solutioninfini)
                    {
                        SendBulkSMSFromSolutionInfi();
                    }
                    else if (liVendordetails.FirstOrDefault().SMS_VENDOR_ID == Common.sms_vendor.DoveSoft)
                    {
                        //SendBulkSMSFromDovesoft();
                        SendBulkSMSFromDovesoftV1();
                    }
                }
                else
                {
                    resultArgs.Success = false;
                    resultArgs.Message = "SMS Vendors not found ...!";
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendMessage", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendMessage", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }

        }
        public void SendVoiceMessage()
        {
            try
            {
                // Getting Timeout Setting from AppSetting  ...
                nSMSTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["SMSTimeOut"]);
                // Getting Current Time ....
                sStartTime = DateTime.Now.ToString(Common.Formats.TimeFormat);
                // Checking Count Of Record ...
                sSmsCount = liSMS.Count;
                // Validate SMS ....
                ValidationToSendSMS();
                if (sResult == Common.ErrorCode.Ok)
                {
                    var ObjSmsSetting = new SMS_SETTING();
                    ObjSmsSetting.API_MODE = "2";
                    liSMSSetting = (List<SMS_SETTING>)CMSHandler.FetchData<SMS_SETTING>(ObjSmsSetting, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSolutionInfiApi)).DataSource.Data;
                    // SMS Setting Details ...
                    if (liSMSSetting != null && liSMSSetting.Count > 0)
                    {
                        sBaseUrl = liSMSSetting.FirstOrDefault().BASE_URL;
                        sFormat = liSMSSetting.FirstOrDefault().FORMAT;
                        sMethod = liSMSSetting.FirstOrDefault().METHOD;
                        sSender = liSMSSetting.FirstOrDefault().SENDER;
                        sAPIKey = liSMSSetting.FirstOrDefault().API_KEY;
                        var list = new List<MessageResult.sms>();
                        bool bFlag = true;
                        int ncount = 0;
                        int nLogcount = 0;
                        int nSMSCount = Convert.ToInt32(ConfigurationManager.AppSettings["SMSCount"]);
                        while (bFlag)
                        {
                            // Checking Record Count ...
                            if (ncount < liSMS.Count)
                            {
                                list = liSMS.Skip(ncount).Take(nSMSCount).ToList();
                                ncount = ncount + list.Count;
                            }
                            else
                            {
                                bFlag = false;
                                list.Clear();
                            }
                            // Checking Record Count To Send Meessages ....
                            if (list != null && list.Count > 0)
                            {
                                // Assign Values To Send Bulk SMS ....
                                var composeClass = new MessageResult.ComposeMessageList();
                                composeClass.unicode = 0;
                                composeClass.flash = 0;
                                composeClass.message = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : string.Empty;
                                composeClass.sender = sSender;

                                // Convert Records To Json , Encoding ....
                                var Numbers = string.Join(",", list.Select(o => o.to).ToArray());
                                sJsonString = new JavaScriptSerializer().Serialize(composeClass);
                                sAPI = string.Concat(Common.SolutionInfiSMSSettingFields.API_KEY.ToLower(), Common.Delimiter.EQUAL, sAPIKey, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.METHOD.ToLower(), Common.Delimiter.EQUAL, sMethod, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.FORMAT.ToLower(), Common.Delimiter.EQUAL, sFormat, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.MESSAGE.ToLower(), Common.Delimiter.EQUAL, sTextMessage, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.NUMBERS.ToLower(), Common.Delimiter.EQUAL, Numbers/*, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.JSON.ToLower(), Common.Delimiter.EQUAL, sJsonString*/);
                                //var data = Encoding.ASCII.GetBytes(sAPI);
                                var data = Encoding.ASCII.GetBytes(sAPI);

                                // Sending Message Via Web API .....                             
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sBaseUrl);
                                request.Method = "POST";
                                request.ContentType = "application/x-www-form-urlencoded";
                                request.ContentLength = data.Length;
                                using (var stream = request.GetRequestStream())
                                {
                                    stream.Write(data, 0, data.Length);
                                }
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                Stream s = (Stream)response.GetResponseStream();
                                StreamReader readStream = new StreamReader(s);
                                string dataString = readStream.ReadToEnd();
                                MessageResult.ResponseFromSolutionInfi responseClass = new MessageResult.ResponseFromSolutionInfi();
                                //  SaveSendSMSList(dataString);
                                // responseClass = JsonConvert.DeserializeObject<MessageResult.ResponseFromSolutionInfi>(dataString);
                                response.Close();
                                s.Close();
                                readStream.Close();
                                if (nLogcount == 0)
                                    //SaveSentItem(dataString);

                                    SaveSendSMSList(dataString);
                                nLogcount++;
                                list.Clear();
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.MethodNotAllowed;
                        ObjJsonData.Message = Common.Messages.PleaseConfigureMessageSetting;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendVoiceMessage", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendVoiceMessage", ex.Message);
                }
            }
        }
        private void SendSMSFromMvaayoo()
        {

        }
        private void SendBulkSMSFromSolutionInfi()
        {
            try
            {
                int ncount = 0;
                var list = new List<MessageResult.sms>();
                // Getting Timeout Setting from AppSetting  ...
                nSMSTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["SMSTimeOut"]);
                // Getting Current Time ....
                sStartTime = DateTime.Now.ToString(Common.Formats.TimeFormat);
                // Checking Count Of Record ...
                sSmsCount = liSMS.Count;
                // Validate SMS ....
                ValidationToSendSMS();
                if (sResult == Common.ErrorCode.Ok)
                {
                    var ObjSmsSetting = new SMS_SETTING();
                    ObjSmsSetting.API_MODE = "1";
                    liSMSSetting = (List<SMS_SETTING>)CMSHandler.FetchData<SMS_SETTING>(ObjSmsSetting, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSolutionInfiApi)).DataSource.Data;
                    // SMS Setting Details ...
                    if (liSMSSetting != null && liSMSSetting.Count > 0)
                    {
                        sAPIKey = liSMSSetting.FirstOrDefault().API_KEY;
                        sBaseUrl = liSMSSetting.FirstOrDefault().BASE_URL;
                        sFormat = liSMSSetting.FirstOrDefault().FORMAT;
                        sMethod = liSMSSetting.FirstOrDefault().METHOD;
                        sSender = liSMSSetting.FirstOrDefault().SENDER;
                        // sAPI = string.Concat(Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.METHOD, Common.Delimiter.EQUAL, sMethod, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.FORMAT, Common.Delimiter.EQUAL, sFormat, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.JSON, Common.Delimiter.EQUAL).ToLower();
                        //  sAPI = string.Concat(sBaseUrl, sAPI);
                        bool bFlag = true;
                        int nSMSCount = Convert.ToInt32(ConfigurationManager.AppSettings["SMSCount"]);
                        while (bFlag)
                        {
                            // Checking Record Count ...
                            if (ncount < liSMS.Count)
                            {
                                list = liSMS.Skip(ncount).Take(nSMSCount).ToList();
                                ncount = ncount + list.Count;
                            }
                            else
                            {
                                bFlag = false;
                                list.Clear();
                            }
                            // Checking Record Count To Send Meessages ....
                            if (list != null && list.Count > 0)
                            {
                                // Assign Values To Send Bulk SMS ....
                                var composeClass = new MessageResult.ComposeMessageList();
                                composeClass.unicode = 0;
                                composeClass.flash = 0;
                                composeClass.message = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : string.Empty;
                                composeClass.sender = sSender;
                                composeClass.sms = list;
                                // Convert Records To Json , Encoding ....
                                sJsonString = new JavaScriptSerializer().Serialize(composeClass);
                                // sJsonString = HttpUtility.UrlEncode(sJsonString);
                                // sAPI = string.Concat(sAPI, sJsonString);
                                sAPI = string.Concat(Common.SolutionInfiSMSSettingFields.API_KEY.ToLower(), Common.Delimiter.EQUAL, sAPIKey, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.METHOD.ToLower(), Common.Delimiter.EQUAL, sMethod, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.FORMAT.ToLower(), Common.Delimiter.EQUAL, sFormat, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.JSON.ToLower(), Common.Delimiter.EQUAL, sJsonString);
                                var data = Encoding.ASCII.GetBytes(sAPI);
                                // Sending Message Via Web API .....                             
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sBaseUrl);
                                request.Method = "POST";
                                request.ContentType = "application/x-www-form-urlencoded";
                                request.ContentLength = data.Length;
                                //request.KeepAlive = false;
                                // request.Timeout = nSMSTimeOut;
                                //  request.ReadWriteTimeout = nSMSTimeOut;
                                using (var stream = request.GetRequestStream())
                                {
                                    stream.Write(data, 0, data.Length);
                                }
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                Stream s = (Stream)response.GetResponseStream();
                                StreamReader readStream = new StreamReader(s);
                                string dataString = readStream.ReadToEnd();
                                MessageResult.ResponseFromSolutionInfi responseClass = new MessageResult.ResponseFromSolutionInfi();

                                responseClass = JsonConvert.DeserializeObject<MessageResult.ResponseFromSolutionInfi>(dataString);
                                response.Close();
                                s.Close();
                                readStream.Close();
                                SaveSendSMSList(dataString);
                                nLogcount++;
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.MethodNotAllowed;
                        ObjJsonData.Message = Common.Messages.PleaseConfigureMessageSetting;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendSMSFromSolutionInfi", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendSMSFromSolutionInfi", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }
        // to validate the sms to send
        private void ValidationToSendSMS()
        {
            var liBalance = (List<SMSBalence>)CMSHandler.FetchData<SMSBalence>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSBalance)).DataSource.Data;
            if (liBalance != null && liBalance.Count > 0)
            {
                iReceivedCredits = Convert.ToInt32(liBalance.FirstOrDefault().RECEIVED_CREDITS);
                iSMSLimit = Convert.ToInt32(liBalance.FirstOrDefault().SMS_LIMIT);
                iUsedCredit = Convert.ToInt32(liBalance.FirstOrDefault().USED_CREDITS);
                iSMSSent = (string.IsNullOrEmpty(sCreditCount)) ? (1 * Convert.ToInt16(sSmsCount)) : (Convert.ToInt16(sCreditCount) * Convert.ToInt16(sSmsCount));
                iTotalSendCreditCount = iSMSSent + iUsedCredit;
                if (iSMSLimit >= iTotalSendCreditCount && iSMSLimit != 0)
                    sResult = Common.ErrorCode.Ok;
                else
                    ObjJsonData.Message = Common.Messages.LowMessageLimit;
            }
            else
                ObjJsonData.Message = Common.Messages.PleaseConfigureMessageSetting;
        }
        // to save sms list
        private void SaveSendSMSList(string JsonResponse)
        {
            string sAcademicYear = string.Empty;
            AccountViewModel objAccount = new AccountViewModel();
            DataTable dtActiveYear = new DataTable();
            string sTempMessage = string.Empty;
            dtActiveYear = objAccount.FetchActiveYear().DataSource.Table;
            if (dtActiveYear != null && dtActiveYear.Rows.Count > 0)
            {
                sAcademicYear = dtActiveYear.Rows[0][Common.ACADEMIC_YEAR.AC_YEAR].ToString();
            }

            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    // Convert Form Json To Model ....
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var ObjSMS = serializer.Deserialize<MessageResult.ComposeResponcesms>(JsonResponse);

                    // Checking SMS Count ....
                    if (ObjSMS.data != null && ObjSMS.data.Count > 0)
                    {
                        ObjJsonData.Message = ObjSMS.message;
                        var ObjUpdateBls = new UpdateSMSBalance();
                        ObjUpdateBls.BALANCE_CREDITS = Convert.ToString(iSMSLimit - iTotalSendCreditCount);
                        ObjUpdateBls.USED_CREDITS = Convert.ToString(iSMSSent + iUsedCredit);
                        ObjUpdateBls.IS_ACTIVE = Common.IsActiveFalg.IsActive;
                        var resultArgs = CMSHandler.SaveOrUpdate(ObjUpdateBls, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.UpdateSMSBalance));
                        //ObjJsonData.Message = ObjSMS.message;
                        foreach (var item in ObjSMS.data)
                        {
                            sClassIds += item.customid1 + ",";
                            sReceipientNo += item.mobile + ",";
                        }
                        sClassIds = sClassIds.TrimEnd(',');
                        sReceipientNo = sReceipientNo.TrimEnd(',');
                        var ObjSaveRecord = new SENT_SMS_2017();
                        ObjSaveRecord.CLASS_IDS = sClassIds;
                        ObjSaveRecord.START_TIME = sStartTime;
                        ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                        if (sSmsMode == Common.MessageType.ATTENDANCE)
                        {
                            ObjSaveRecord.CONTENT = "Absentees List";
                            sCreditCount = "1";
                        }
                        else if (sSmsMode == Common.MessageType.CLASS_WISE_ANNOUNCEMENT)
                        {
                            ObjSaveRecord.CONTENT = "Classwise Announcement";
                        }
                        else if (sSmsMode == Common.MessageType.STAFF_ANNOUNCEMENT)
                        {
                            ObjSaveRecord.CONTENT = "Staff Announcement";
                        }
                        else if (sSmsMode == Common.MessageType.HOSTEL)
                        {
                            ObjSaveRecord.CONTENT = "Hostel Announcement";
                        }
                        else if (sSmsMode == Common.MessageType.COMMON_ANNOUNCEMENT)
                        {
                            ObjSaveRecord.CONTENT = "Common Announcement";
                        }
                        else if (sSmsMode == Common.MessageType.TEST)
                        {
                            ObjSaveRecord.CONTENT = "Test Message";
                        }
                        else
                        {
                            ObjSaveRecord.CONTENT = liSMS.FirstOrDefault().message;
                        }
                        ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                        ObjSaveRecord.STATUS = ObjSMS.data.FirstOrDefault().status;
                        ObjSaveRecord.TRANSACTION_ID = ObjSMS.data.FirstOrDefault().id;
                        ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                        ObjSaveRecord.SMS_MODE = sSmsMode;
                        ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                        ObjSaveRecord.ACADEMIC_YEAR = sAcademicYear;
                        var sresultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), sAcademicYear, true);
                        if (sresultArgs.Success)
                        {
                            string sSQL = string.Empty;
                            sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.RECEIPIENT_ID + ","
                                   + Common.SENT_SMS_LIST.CLASS_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                                   + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + ","
                                   + Common.SENT_SMS_LIST.CREDIT_COUNT + ","
                                   + Common.SENT_SMS_LIST.ACADEMIC_YEAR + ")VALUES";
                            sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                            string lstInsertedId = sresultArgs.RowUniqueId.ToString();
                            foreach (var item in ObjSMS.data)
                            {
                                if (liSMS != null && liSMS.Count > 0)
                                {
                                    var liSave = liSMS.Where(s => s.custom == item.customid).ToList();
                                    if (liSave != null && liSave.Count > 0)
                                    {
                                        foreach (var lstsave in liSave)
                                        {
                                            sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                            sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                            sSQL += @"('" + lstInsertedId + "','" + lstsave.custom + "','" + lstsave.custom1 + "','" + lstsave.message + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + item.id + "','" + lstsave.CreditCount + "','" + sAcademicYear + "'),";
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(item.customid))
                                            item.customid = "0";
                                        if (string.IsNullOrEmpty(sCreditCount))
                                            sCreditCount = "0";
                                        sTextMessage = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                        sSQL += @"('" + lstInsertedId + "','" + item.customid + "','" + item.customid1 + "','" + ObjSaveRecord.CONTENT + "','" + item.customid2 + "','" + item.mobile + "','" + item.id + "','" + sCreditCount + "','" + sAcademicYear + "'),";
                                    }
                                }
                                else
                                {
                                    ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                                }

                            }
                            sSQL = sSQL.TrimEnd(',');
                            using (MySQLHandler ObjHandler = new MySQLHandler())
                            {
                                var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                                if (resultArg.Success && resultArg.RowsAffected != 0)
                                {
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    sResultMessage = Common.ErrorCode.Ok;
                                }
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SaveSendSMSList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SaveSendSMSList", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }
        public ActionResult BindClassWiseInfoForIndividual(string sClassIds)
        {
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sClassIds))
                    {
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                        string sSql = string.Empty;
                        try
                        {
                            sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStudentInfoForIndividualSMS).Replace(Common.Delimiter.QUS + Common.STU_CLASS.CLASS_ID, sClassIds);
                            objViewModel.LIStudentDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSql, sAcademicYear).DataSource.Data;
                            return View(objViewModel);
                        }
                        catch (Exception ex)
                        {
                            using (ErrorLog objHandler = new ErrorLog())
                            {
                                objHandler.WriteError("CommunicationController", "BindClassWiseInfoForIndividual", "Err MSg: " + ex.Message, "Query is empty!");
                                objHandler.WriteError("CommunicationController", "BindClassWiseInfoForIndividual", ex.Message);
                            }
                            ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                            ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "BindClassWiseInfoForIndividual", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "BindClassWiseInfoForIndividual", ex.Message);
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }

        }
        public ActionResult BindDepartmentWiseInfoForIndividual(string sDepartmentIds)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sClassIds))
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                    string sSql = string.Empty;
                    try
                    {
                        sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStudentInfoForIndividualSMS).Replace(Common.Delimiter.QUS + Common.STU_CLASS.CLASS_ID, sClassIds);
                        objViewModel.LIStudentDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSql).DataSource.Data;
                        return View(objViewModel);
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("CommunicationController", "BindClassWiseInfoForIndividual", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("CommunicationController", "BindClassWiseInfoForIndividual", ex.Message);
                        }
                        ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                        ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult FetchClasswiseRecordsCount(string sClassIds)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                if (!string.IsNullOrEmpty(sClassIds))
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                    string sSql = string.Empty;
                    try
                    {
                        sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchClasswiseRecordsCount).Replace(Common.Delimiter.QUS + Common.STU_CLASS.CLASS_ID, sClassIds);
                        var LiStudentMoblileCount = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSql, sAcademicYear).DataSource.Data;
                        if (LiStudentMoblileCount != null && LiStudentMoblileCount.Count > 0)
                        {
                            ObjJsonData.sResult = LiStudentMoblileCount.SingleOrDefault().RECORDSCOUNT;
                            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("CommunicationController", "FetchClasswiseRecordsCount", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("CommunicationController", "FetchClasswiseRecordsCount", ex.Message);
                        }
                        ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                        ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindStaffInfoForIndividual(string sStaffCategoryId)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sSQL = string.Empty;
                    if (!string.IsNullOrEmpty(sStaffCategoryId))
                    {
                        sSQL = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffInfoForIndividualSMS).Replace(Common.Delimiter.QUS + Common.SUP_STAFF_TYPE.STAFF_TYPE, sStaffCategoryId);
                        objViewModel.LIStaffDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSQL).DataSource.Data;
                        return View(objViewModel);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "BindStaffInfoForIndividual", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "BindStaffInfoForIndividual", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult SendBulkMessage(string JsonIndividualMessage, string sSMSMode, string sContent, string sClassId, string ChStaff, string sCredit, string sRole)
        {
            string sSql = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                try
                {
                    sSmsMode = sSMSMode;
                    sCreditCount = sCredit;
                    if (string.IsNullOrEmpty(sSMSMode))
                    {
                        sSmsMode = "5";
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        var ObjSMS = serializer.Deserialize<MessageResult.ComposeMessageList>(JsonIndividualMessage);
                        liSMS = ObjSMS.sms;
                        SendMessage();
                    }
                    else if (sSMSMode.Equals("2"))
                    {
                        sSmsMode = Common.MessageType.CLASS_WISE_ANNOUNCEMENT;
                        //var ObjStudentByCls = new FetchClassWiseMobileNumber();
                        //ObjStudentByCls.CLASS_ID = sClassId;
                        sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchClasswiseMobileNumbers).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                        sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                        var liClsWiseNumber = (List<FetchClassWiseMobileNumber>)CMSHandler.FetchData<FetchClassWiseMobileNumber>(null, sSql, sAcademicYear).DataSource.Data;
                        if (!string.IsNullOrEmpty(ChStaff))
                        {
                            sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchClassStaffNumber).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                            sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                            var liStaff = (List<FetchClassWiseMobileNumber>)CMSHandler.FetchData<FetchClassWiseMobileNumber>(null, sSql).DataSource.Data;

                            if (liStaff != null && liStaff.Count > 0)
                            {
                                var liClsStaff = liStaff.Concat(liClsWiseNumber).ToList();
                                foreach (var item in liClsStaff)
                                {
                                    liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                                }
                                SendMessage();
                            }
                            else
                                ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                        }
                        else if (liClsWiseNumber != null && liClsWiseNumber.Count > 0)
                        {
                            foreach (var item in liClsWiseNumber)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;

                    }
                    else if (sSMSMode.Equals("3"))
                    {
                        sSmsMode = Common.MessageType.STAFF_ANNOUNCEMENT;
                        string sSQL = string.Empty;
                        sSQL = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffMobileNumbers).Replace(Common.Delimiter.QUS + Common.USER_ACCOUNT.CATEGORY, sRole);
                        var liStaff = (List<FetchClassStaffNumber>)CMSHandler.FetchData<FetchClassStaffNumber>(null, sSQL).DataSource.Data;
                        if (liStaff != null && liStaff.Count > 0)
                        {
                            foreach (var item in liStaff)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;

                    }
                    else if (sSMSMode.Equals("1"))
                    {
                        sSmsMode = Common.MessageType.COMMON_ANNOUNCEMENT;
                        var liAllMobileNumber = (List<FetchClassStaffNumber>)CMSHandler.FetchData<FetchClassStaffNumber>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffAndStudentMobileNumbers), sAcademicYear).DataSource.Data;
                        if (liAllMobileNumber != null && liAllMobileNumber.Count > 0)
                        {
                            foreach (var item in liAllMobileNumber)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                    }
                    else if (sSMSMode.Equals("12"))
                    {
                        sSmsMode = Common.MessageType.HOSTEL;
                        var liHostelstudentMobileNumber = (List<FetchClassStaffNumber>)CMSHandler.FetchData<FetchClassStaffNumber>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchHostelStudentMobileNumbers), sAcademicYear).DataSource.Data;
                        if (liHostelstudentMobileNumber != null && liHostelstudentMobileNumber.Count > 0)
                        {
                            foreach (var item in liHostelstudentMobileNumber)
                            {
                                liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "SendBulkMessage", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "SendBulkMessage", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStaffRecordsCount(string sRole)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sSQL = string.Empty;
                try
                {
                    sSQL = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffRecordsCount).Replace(Common.Delimiter.QUS + Common.USER_ACCOUNT.CATEGORY, sRole);
                    var LiStaffMoblileCount = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, sSQL).DataSource.Data;
                    if (LiStaffMoblileCount != null && LiStaffMoblileCount.Count > 0)
                    {
                        ObjJsonData.sResult = LiStaffMoblileCount.SingleOrDefault().RECORDSCOUNT;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "FetchStaffRecordsCount", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "FetchStaffRecordsCount", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }

            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStaffAndStudentRecordsCount()
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                try
                {
                    var BothCount = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffAndStudentRecordsCount), sAcademicYear).DataSource.Data;
                    if (BothCount != null && BothCount.Count > 0)
                    {
                        ObjJsonData.sResult = BothCount.SingleOrDefault().RECORDSCOUNT;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "FetchStaffAndStudentRecordsCount", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "FetchStaffAndStudentRecordsCount", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }

            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchHostelStudentRecordsCount()
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                try
                {

                    var HostelCount = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchHostelStudentRecordsCount), sAcademicYear).DataSource.Data;
                    if (HostelCount != null && HostelCount.Count > 0)
                    {
                        ObjJsonData.sResult = HostelCount.SingleOrDefault().RECORDSCOUNT;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "FetchStaffAndStudentRecordsCount", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "FetchStaffAndStudentRecordsCount", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }

            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #region   Sent SMS Report 
        public ActionResult SentMessageReport()
        {
            SENT_SMS_2017 liSentMessage = new SENT_SMS_2017();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var messsageType = DapperHandler.FetchAll<sup_message_type>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupMessageType)).ToList();
                    if (messsageType != null && messsageType.Count > 0)
                    {
                        objViewModel.messageType = new SelectList(messsageType, Common.SUP_MESSAGE_TYPE.MSG_TYPE_ID, Common.SUP_MESSAGE_TYPE.MESSAGE_TYPE_NAME);
                    }
                    //objViewModel.LISentMessage = (List<SENT_SMS_2017>)CMSHandler.FetchData<SENT_SMS_2017>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSentMessages), sAcademicYear).DataSource.Data;
                    objViewModel.LISentMessage = (List<SENT_SMS_2017>)CMSHandler.FetchData<SENT_SMS_2017>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSentMessagesByAcYear), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SentMessage", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SentMessage", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return View(objViewModel);
        }
        public ActionResult BindDateWiseSMSReport(string sMessageType, string sFromDate, string sToDate)
        {
            SENT_SMS_LIST_2017 objSentsmsList = new SENT_SMS_LIST_2017();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var ListSentMessages = new List<FetchSentItemIds>();
                sFromDate = CommonMethods.DateConvertionForPayU(sFromDate);
                sToDate = CommonMethods.DateConvertionForPayU(sToDate);
                objViewModel.LISentMessageList = (List<SENT_SMS_LIST_2017>)CMSHandler.FetchData<SENT_SMS_LIST_2017>(objSentsmsList, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSReportByModeAndDateWise).Replace("?SMS_MODE", sMessageType).Replace("?FROM_DATE", sFromDate).Replace("?TO_DATE", sToDate), sAcademicYear).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public ActionResult BindSMSBalance()
        {
            SENT_SMS_LIST_2017 objSentsmsList = new SENT_SMS_LIST_2017();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var ListSentMessages = new List<FetchSentItemIds>();

                objViewModel.LiSmsBalance = (List<SMSBalence>)CMSHandler.FetchData<SMSBalence>(objSentsmsList, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSCreditAndDebit), sAcademicYear).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        #endregion

        public ActionResult SentMessage()
        {
            SENT_SMS_2017 liSentMessage = new SENT_SMS_2017();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    //objViewModel.LISentMessage = (List<SENT_SMS_2017>)CMSHandler.FetchData<SENT_SMS_2017>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSentMessages), sAcademicYear).DataSource.Data;
                    objViewModel.LISentMessage = (List<SENT_SMS_2017>)CMSHandler.FetchData<SENT_SMS_2017>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSentMessagesByAcYear), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SentMessage", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SentMessage", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
            }
            return View(objViewModel);
        }
        public ActionResult SentMessageList(string SentSMSId)
        {
            SENT_SMS_LIST_2017 objSentsmsList = new SENT_SMS_LIST_2017();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var ListSentMessages = new List<FetchSentItemIds>();
                objSentsmsList.SENT_ITEM_ID = SentSMSId;
                ListSentMessages = (List<FetchSentItemIds>)CMSHandler.FetchData<FetchSentItemIds>(objSentsmsList, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchTransactionIdBySentId), sAcademicYear).DataSource.Data;
                if (ListSentMessages != null && ListSentMessages.Count > 0)
                    sTransactionId = ListSentMessages.FirstOrDefault().TRANSACTION_ID;
                GetRequestByTransactionId();
                objViewModel.LISentMessageList = (List<SENT_SMS_LIST_2017>)CMSHandler.FetchData<SENT_SMS_LIST_2017>(objSentsmsList, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSentItemForViewMode), sAcademicYear).DataSource.Data;
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        // Getting Request By TransactionId ...
        private void GetRequestByTransactionId()
        {
            try
            {
                var ObjSmsSetting = new SMS_SETTING();
                ObjSmsSetting.API_MODE = "1";
                liSMSSetting = (List<SMS_SETTING>)CMSHandler.FetchData<SMS_SETTING>(ObjSmsSetting, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSolutionInfiApi)).DataSource.Data;
                // SMS Setting Details ...
                if (liSMSSetting != null && liSMSSetting.Count > 0)
                {
                    foreach (var item in liSMSSetting)
                    {
                        sBaseUrl = item.BASE_URL;
                        sFormat = item.FORMAT;
                        sMethod = item.REQUEST_METHOD;
                        sSender = item.SENDER;
                        sAPI = string.Concat(Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.METHOD, Common.Delimiter.EQUAL, sMethod, Common.Delimiter.AMPRESAND, Common.SolutionInfiSMSSettingFields.ID, Common.Delimiter.EQUAL, sTransactionId).ToLower();
                        sAPI = string.Concat(sBaseUrl, sAPI);
                    }
                    string strURL = string.Format(sAPI);
                    WebRequest request = HttpWebRequest.Create(strURL);
                    request.ContentType = "application/json;";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)response.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    UpdateReplyStatus(dataString);
                    response.Close();
                    s.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "GetRequestByTransactionId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "GetRequestByTransactionId", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }
        // Update Delivery Report ...
        private ResultArgs UpdateReplyStatus(string JsonResponse)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var ObjSMS = serializer.Deserialize<MessageResult.ComposeResponcesms>(JsonResponse);
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                if (ObjSMS.data != null && ObjSMS.data.Count > 0)
                {
                    string sSQLscript = string.Empty;
                    sSQLscript = @"SET SQL_SAFE_UPDATES=0;";
                    if (ObjSMS.data != null && ObjSMS.data.Count > 0)
                    {
                        foreach (var item in ObjSMS.data)
                        {
                            sSQLscript += @" UPDATE SENT_SMS_LIST SET `COMPOSED_TIME`='" + item.senttime.Split(' ')[1] + "',`SENT_TIME`='" + item.dlrtime.Split(' ')[1] + "',`STATUS` ='" + item.status + "',`DELIVERED_DATE`='" + item.dlrtime.Split(' ')[0] + "',`SENT_DATE`='" + item.senttime.Split(' ')[0] + "'  WHERE TRANSACTION_ID ='" + item.id + "'; ";
                        }
                        sSQLscript = sSQLscript.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                    }
                    sSQLscript += @"SET SQL_SAFE_UPDATES=1;";

                    using (MySQLHandler ObjHandler = new MySQLHandler())
                    {
                        var resultArgs = ObjHandler.ExecuteAsScripts(sSQLscript);
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "UpdateReplyStatus", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "UpdateReplyStatus", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            return resultArgs;
        }
        public JsonResult Resend(string SentListItemId)
        {
            var ObjSentItem = new SENT_SMS_LIST_2017();
            ObjSentItem.SENT_SMS_LIST_ID = SentListItemId;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
            var liResend = (List<SENT_SMS_LIST_2017>)CMSHandler.FetchData<SENT_SMS_LIST_2017>(ObjSentItem, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSenderInfoForResend), sAcademicYear).DataSource.Data;
            if (liResend != null && liResend.Count > 0)
            {
                sSmsMode = Common.MessageType.RESENT;
                foreach (var item in liResend)
                {
                    int count = Convert.ToInt32(item.CONTENT.Length) / 153;
                    sCreditCount = Convert.ToString(count);
                    if (sCreditCount == "0")
                        sCreditCount = "1";
                    liSMS.Add(new MessageResult.sms() { to = item.MOBILE_NO, message = item.CONTENT, custom = item.RECEIPIENT_ID, custom1 = item.CLASS, custom2 = item.IS_STAFF });
                }
                SendMessage();

            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public string LibrarySmsSending(List<MessageResult.sms> listsms)
        {
            string Message = string.Empty;
            liSMS = listsms;
            SendMessage();
            return Message;
        }

        #region CommonsmsMethod 

        public void SendEmailAndTextMessage(string sContent, string sToNumber = "", string sToEmail = "", string sSubTitle = "")
        {
            if (!string.IsNullOrEmpty(sContent))
            {
                List<MessageResult.sms> liSMS = new List<MessageResult.sms>();
                //string sFromEmail = ConfigurationManager.AppSettings["AdmissionFromMail"].ToString();
                //string sPasswordEmail = ConfigurationManager.AppSettings["AdmissionPassword"].ToString();
                //string sEmailOTPContent = string.Empty;
                //string sEmailContent = string.Empty;
                if (!string.IsNullOrEmpty(sToNumber))
                {
                    //---------------SMS------------
                    CommunicationController communication = new CommunicationController();
                    int length = Convert.ToInt32(sContent.Length);

                    string sCount = CommonMethods.ValidateCreditCount(length);
                    if (sCount == "0")
                        sCount = "1";
                    communication.sCreditCount = sCount;
                    liSMS.Add(new MessageResult.sms() { to = sToNumber, message = sContent, custom = "0", custom1 = "0", custom2 = "0" });
                    communication.liSMS = liSMS;
                    communication.sSmsCount = 1;
                    communication.SendBulkSMSFromSolutionInfi();
                }
                //-----------end sms--------------
                //-----------Email----------------   
                if (!string.IsNullOrEmpty(sToEmail))
                {
                    //CommonMethods.SendMailFromGmailSMTP(sFromEmail, sPasswordEmail, sToEmail, sSubTitle, sContent);
                }
                //----------Ending Email---------
            }

        }
        #endregion


        #endregion

        #region SMS Excel Import
        public ActionResult SMSImportFromFile()
        {
            return View();
        }
        #endregion

        #region SMS List
        public ActionResult SentSMSList()
        {
            var ObjViewModel = new CommunicationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Dovesoft SMS

        public JsonResult SendBulkMessageInDovesoft(string JsonIndividualMessage, string sSMSMode, string sContent, string sClassId, string ChStaff, string sCredit, string sRole, string slanguageOption, string sStaffCategory)
        {
            string sSql = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                try
                {
                    sSmsMode = sSMSMode;
                    sCreditCount = sCredit;
                    sUniCode = slanguageOption;
                    if (string.IsNullOrEmpty(sSMSMode))
                    {
                        sSmsMode = "5";
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        var ObjSMS = serializer.Deserialize<MessageResult.ComposeMessageList>(JsonIndividualMessage);
                        listbulkSMS = ObjSMS.listSMS;
                        SendMessage();
                    }
                    else if (sSMSMode.Equals("2"))
                    {
                        sSmsMode = Common.MessageType.CLASS_WISE_ANNOUNCEMENT;
                        //var ObjStudentByCls = new FetchClassWiseMobileNumber();
                        //ObjStudentByCls.CLASS_ID = sClassId;
                        sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchClasswiseMobileNumbers).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                        sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                        var liClsWiseNumber = (List<FetchClassWiseMobileNumber>)CMSHandler.FetchData<FetchClassWiseMobileNumber>(null, sSql, sAcademicYear).DataSource.Data;
                        if (!string.IsNullOrEmpty(ChStaff))
                        {
                            sSql = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchClassStaffNumber).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                            sSql = sSql.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                            var liStaff = (List<FetchClassWiseMobileNumber>)CMSHandler.FetchData<FetchClassWiseMobileNumber>(null, sSql).DataSource.Data;

                            if (liStaff != null && liStaff.Count > 0)
                            {
                                var liClsStaff = liStaff.Concat(liClsWiseNumber).ToList();
                                foreach (var item in liClsStaff)
                                {
                                    //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                                    listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sContent });
                                }
                                SendMessage();
                            }
                            else
                                ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                        }
                        else if (liClsWiseNumber != null && liClsWiseNumber.Count > 0)
                        {
                            foreach (var item in liClsWiseNumber)
                            {
                                //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                                listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sContent });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;

                    }
                    else if (sSMSMode.Equals("3"))
                    {
                        sSmsMode = Common.MessageType.STAFF_ANNOUNCEMENT;
                        string sSQL = string.Empty;
                        sSQL = CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffMobileNumbers).Replace(Common.Delimiter.QUS + Common.USER_ACCOUNT.CATEGORY, sRole);
                        var liStaff = (List<FetchClassStaffNumber>)CMSHandler.FetchData<FetchClassStaffNumber>(null, sSQL).DataSource.Data;
                        if (liStaff != null && liStaff.Count > 0)
                        {
                            foreach (var item in liStaff)
                            {
                                //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                                listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sContent });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;

                    }
                    else if (sSMSMode.Equals("1"))
                    {
                        sSmsMode = Common.MessageType.COMMON_ANNOUNCEMENT;
                        var liAllMobileNumber = (List<FetchClassStaffNumber>)CMSHandler.FetchData<FetchClassStaffNumber>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchStaffAndStudentMobileNumbers), sAcademicYear).DataSource.Data;
                        if (liAllMobileNumber != null && liAllMobileNumber.Count > 0)
                        {
                            foreach (var item in liAllMobileNumber)
                            {
                                //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                                listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sContent });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                    }
                    else if (sSMSMode.Equals("12"))
                    {
                        sSmsMode = Common.MessageType.HOSTEL;
                        var liHostelstudentMobileNumber = (List<FetchClassStaffNumber>)CMSHandler.FetchData<FetchClassStaffNumber>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchHostelStudentMobileNumbers), sAcademicYear).DataSource.Data;
                        if (liHostelstudentMobileNumber != null && liHostelstudentMobileNumber.Count > 0)
                        {
                            foreach (var item in liHostelstudentMobileNumber)
                            {
                                //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sContent, custom = item.RECEIPIENT_ID, custom1 = item.CLASS_ID, custom2 = item.IS_STAFF, CreditCount = sCreditCount });
                                listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sContent });
                            }
                            SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "SendBulkMessageInDovesoft", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "SendBulkMessageInDovesoft", ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Author : yesudass
        /// Date : 25 April 2025
        /// title : sms balance updation 
        /// </summary>
        public void SendSMSFromDovesoft()
        {
            try
            {
                var liBalance = (List<SMSBalence>)CMSHandler.FetchData<SMSBalence>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSBalance)).DataSource.Data;
                if (liBalance != null && liBalance.Count > 0)
                {
                    iReceivedCredits = Convert.ToInt32(liBalance.FirstOrDefault().RECEIVED_CREDITS);
                    iSMSLimit = Convert.ToInt32(liBalance.FirstOrDefault().SMS_LIMIT);
                    iUsedCredit = Convert.ToInt32(liBalance.FirstOrDefault().USED_CREDITS);
                    iSMSSent = (string.IsNullOrEmpty(sCreditCount)) ? (1 * Convert.ToInt16(sSmsCount)) : (Convert.ToInt16(sCreditCount) * Convert.ToInt16(sSmsCount));
                    //iSMSSent = listbulkSMS.Count();
                    iTotalSendCreditCount = iSMSSent + iUsedCredit;
                    if (iSMSLimit >= iTotalSendCreditCount && iSMSLimit != 0)
                        sResult = Common.ErrorCode.Ok;
                    else
                        ObjJsonData.Message = Common.Messages.LowMessageLimit;
                    var ObjUpdateBls = new UpdateSMSBalance();
                    ObjUpdateBls.BALANCE_CREDITS = Convert.ToString(iSMSLimit - iTotalSendCreditCount);
                    ObjUpdateBls.USED_CREDITS = Convert.ToString(iSMSSent + iUsedCredit);
                    ObjUpdateBls.IS_ACTIVE = Common.IsActiveFalg.IsActive;
                    var resultArgs = CMSHandler.SaveOrUpdate(ObjUpdateBls, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.UpdateSMSBalance));
                }
                liSMSSetting = (List<SMS_SETTING>)CMSHandler.FetchData<SMS_SETTING>(new SMS_SETTING() { API_METHOD = Common.API_METHOD.SINGLE_SMS }, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchDovesoftSettings)).DataSource.Data;
                if (liSMSSetting != null && liSMSSetting.Count > 0)
                {
                    sAPIKey = liSMSSetting.FirstOrDefault().API_KEY;
                    sBaseUrl = liSMSSetting.FirstOrDefault().BASE_URL;
                    sFormat = liSMSSetting.FirstOrDefault().FORMAT;
                    sMethod = liSMSSetting.FirstOrDefault().METHOD;
                    sSender = liSMSSetting.FirstOrDefault().SENDER;
                    sUser = liSMSSetting.FirstOrDefault().USER;
                    sAccusage = liSMSSetting.FirstOrDefault().ACCUSAGE;
                    sEntityId = liSMSSetting.FirstOrDefault().ENTITY_ID;
                    string sbPostData = "user=" + sUser + "&key=" + sAPIKey + "&mobile=" + sReceipientNo + "&message=" + sTextMessage + "&senderid=" + sSender + "&accusage=" + sAccusage + "";
                    if (!string.IsNullOrEmpty(sEntityId))
                    {
                        sbPostData += "&entityid=" + sEntityId;
                    }
                    if (!string.IsNullOrEmpty(sUniCode))
                    {
                        sbPostData += "&unicode=1";
                    }
                    //Call Send SMS API
                    string sendSMSUri = sBaseUrl;
                    //Create HTTPWebrequest
                    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                    //Prepare and Add URL Encoded data
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] data = encoding.GetBytes(sbPostData.ToString());
                    //Specify post method
                    httpWReq.Method = sMethod;
                    httpWReq.ContentType = "application/x-www-form-urlencoded";
                    httpWReq.ContentLength = data.Length;
                    using (Stream stream = httpWReq.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    //Get the response
                    HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseString = reader.ReadToEnd();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var ObjSMS = serializer.Deserialize<MessageResult.Dovesoftcompose>(responseString);
                    if (ObjSMS.smslist.sms.status == "success")
                    {
                        sResultMessage = Common.ErrorCode.Ok;
                        ObjJsonData.Message = "Message Sent Successfully ...!";
                    }
                    //Close the response
                    reader.Close();
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendSMSFromDovesoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendSMSFromDovesoft", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }

        public void SendBulkSMSFromDovesoft()
        {
            bool bFlag = true;
            int ncount = 0;
            sSmsCount = listbulkSMS.Count;
            var list = new List<MessageResult.listsms>();
            try
            {


                var liBalance = (List<SMSBalence>)CMSHandler.FetchData<SMSBalence>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSBalance)).DataSource.Data;
                if (liBalance != null && liBalance.Count > 0)
                {
                    iReceivedCredits = Convert.ToInt32(liBalance.FirstOrDefault().RECEIVED_CREDITS);
                    iSMSLimit = Convert.ToInt32(liBalance.FirstOrDefault().SMS_LIMIT);
                    iUsedCredit = Convert.ToInt32(liBalance.FirstOrDefault().USED_CREDITS);
                    iSMSSent = (string.IsNullOrEmpty(sCreditCount)) ? (1 * Convert.ToInt16(sSmsCount)) : (Convert.ToInt16(sCreditCount) * Convert.ToInt16(sSmsCount));
                    //iSMSSent = listbulkSMS.Count();
                    iTotalSendCreditCount = iSMSSent + iUsedCredit;
                    if (iSMSLimit >= iTotalSendCreditCount && iSMSLimit != 0)
                        sResult = Common.ErrorCode.Ok;
                    else
                        ObjJsonData.Message = Common.Messages.LowMessageLimit;
                    var ObjUpdateBls = new UpdateSMSBalance();
                    ObjUpdateBls.BALANCE_CREDITS = Convert.ToString(iSMSLimit - iTotalSendCreditCount);
                    ObjUpdateBls.USED_CREDITS = Convert.ToString(iSMSSent + iUsedCredit);
                    ObjUpdateBls.IS_ACTIVE = Common.IsActiveFalg.IsActive;
                    var resultArgs = CMSHandler.SaveOrUpdate(ObjUpdateBls, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.UpdateSMSBalance));
                }

                liSMSSetting = (List<SMS_SETTING>)CMSHandler.FetchData<SMS_SETTING>(new SMS_SETTING() { API_METHOD = Common.API_METHOD.BULK_SMS }, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchDovesoftSettings)).DataSource.Data;
                if (liSMSSetting != null && liSMSSetting.Count > 0)
                {
                    sAPIKey = liSMSSetting.FirstOrDefault().API_KEY;
                    sBaseUrl = liSMSSetting.FirstOrDefault().BASE_URL;
                    sFormat = liSMSSetting.FirstOrDefault().FORMAT;
                    sMethod = liSMSSetting.FirstOrDefault().METHOD;
                    sSender = liSMSSetting.FirstOrDefault().SENDER;
                    sUser = liSMSSetting.FirstOrDefault().USER;
                    sAccusage = liSMSSetting.FirstOrDefault().ACCUSAGE;
                    sEntityId = liSMSSetting.FirstOrDefault().ENTITY_ID;
                    int nSMSCount = Convert.ToInt32(ConfigurationManager.AppSettings["SMSCount"]);
                    while (bFlag)
                    {
                        // Checking Record Count ...
                        if (ncount < listbulkSMS.Count)
                        {
                            list = listbulkSMS.Skip(ncount).Take(nSMSCount).ToList();
                            ncount = ncount + list.Count;
                        }
                        else
                        {
                            bFlag = false;
                            list.Clear();
                        }

                        // Checking Record Count To Send Meessages ....
                        if (list != null && list.Count > 0)
                        {
                            // Assign Values To Send Bulk SMS ....
                            var composeClass = new MessageResult.SMSRequestCompose();
                            composeClass.listsms = listbulkSMS;
                            composeClass.user = sUser;
                            composeClass.password = sAPIKey;
                            foreach (var item in composeClass.listsms)
                            {
                                item.senderid = sSender;
                                item.accountusagetypeid = sAccusage;
                                if (string.IsNullOrEmpty(item.entityid) && !string.IsNullOrEmpty(sEntityId))
                                {
                                    item.entityid = sEntityId;
                                }
                            }

                            // Convert Records To Json , Encoding ....
                            sJsonString = new JavaScriptSerializer().Serialize(composeClass);

                            //Call Send SMS API
                            string sendSMSUri = sBaseUrl;
                            //Create HTTPWebrequest
                            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                            //Prepare and Add URL Encoded data
                            UTF8Encoding encoding = new UTF8Encoding();
                            byte[] data = encoding.GetBytes(sJsonString);
                            //Specify post method
                            httpWReq.Method = sMethod;
                            httpWReq.ContentType = "application/json";
                            httpWReq.ContentLength = data.Length;
                            using (Stream stream = httpWReq.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }
                            //Get the response
                            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            string responseString = reader.ReadToEnd();
                            if (nLogcount == 0)
                                SaveSentItemFromDoveSoft(responseString);


                                SaveSendSMSListFromDoveSoft(responseString);
                                ObjJsonData.Message = listbulkSMS.Count.ToString() + " Messages Submitted successfully.";
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            sResultMessage = Common.ErrorCode.Ok;
                            nLogcount++;
                            //Close the response
                            reader.Close();
                            response.Close();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendBulkSMSFromDovesoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendBulkSMSFromDovesoft", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }
        public void SendBulkSMSFromDovesoftV1()
        {
            bool bFlag = true;
            int ncount = 0;
            sSmsCount = listbulkSMS.Count;
            int SMSSkipcount = 0;
            var list = new List<MessageResult.listsms>();
            string TotalresponseString = string.Empty;
            var lastInsertID = "";
            try
            {
                var listBulksmsList = listbulkSMS.GroupBy(o => o.class_id)
                                   .OrderBy(o => o.Key)
                                   .ToList();
                var liBalance = (List<SMSBalence>)CMSHandler.FetchData<SMSBalence>(null, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchSMSBalance)).DataSource.Data;
                if (liBalance != null && liBalance.Count > 0)
                {
                    iReceivedCredits = Convert.ToInt32(liBalance.FirstOrDefault().RECEIVED_CREDITS);
                    iSMSLimit = Convert.ToInt32(liBalance.FirstOrDefault().SMS_LIMIT);
                    iUsedCredit = Convert.ToInt32(liBalance.FirstOrDefault().USED_CREDITS);
                    iSMSSent = (string.IsNullOrEmpty(sCreditCount)) ? (1 * Convert.ToInt16(sSmsCount)) : (Convert.ToInt16(sCreditCount) * Convert.ToInt16(sSmsCount));
                    //iSMSSent = listbulkSMS.Count();
                    iTotalSendCreditCount = iSMSSent + iUsedCredit;
                    if (iSMSLimit >= iTotalSendCreditCount && iSMSLimit != 0)
                        sResult = Common.ErrorCode.Ok;
                    else
                        ObjJsonData.Message = Common.Messages.LowMessageLimit;
                    var ObjUpdateBls = new UpdateSMSBalance();
                    ObjUpdateBls.BALANCE_CREDITS = Convert.ToString(iSMSLimit - iTotalSendCreditCount);
                    ObjUpdateBls.USED_CREDITS = Convert.ToString(iSMSSent + iUsedCredit);
                    ObjUpdateBls.IS_ACTIVE = Common.IsActiveFalg.IsActive;
                    var resultArgs = CMSHandler.SaveOrUpdate(ObjUpdateBls, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.UpdateSMSBalance));
                }

                foreach (var liItem in listBulksmsList)
                {
                    sSmsCount = liItem.Count();
                    nLogcount = 0;

                    // Fetch SMS Settings Once for Each Class Group
                    liSMSSetting = CMSHandler.FetchData<SMS_SETTING>(new SMS_SETTING
                    {
                        API_METHOD = Common.API_METHOD.BULK_SMS
                    }, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchDovesoftSettings)).DataSource.Data as List<SMS_SETTING>;

                    if (liSMSSetting == null || liSMSSetting.Count == 0) continue;
                    sAPIKey = liSMSSetting.First().API_KEY;
                    sBaseUrl = liSMSSetting.First().BASE_URL;
                    sFormat = liSMSSetting.First().FORMAT;
                    sMethod = liSMSSetting.First().METHOD;
                    sSender = liSMSSetting.First().SENDER;
                    sUser = liSMSSetting.First().USER;
                    sAccusage = liSMSSetting.First().ACCUSAGE;
                    sEntityId = liSMSSetting.First().ENTITY_ID;

                    int nSMSCount = Convert.ToInt32(ConfigurationManager.AppSettings["SMSCount"]);

                    // Process SMS for the Current Class Group
                    var sortedClassItems = liItem.OrderBy(o => o.receipient_id).ToList();

                    foreach (var item in sortedClassItems)
                    {
                        list = sortedClassItems.Where(o => o.receipient_id == item.receipient_id).Take(1).ToList();

                        if (list == null || list.Count == 0)
                        {
                            bFlag = false;
                            break;
                        }

                        // Assign Values To Send Bulk SMS
                        var composeClass = new MessageResult.SMSRequestCompose
                        {
                            listsms = list,
                            user = sUser,
                            password = sAPIKey
                        };

                        var smsItem = composeClass.listsms.First();
                        smsItem.senderid = sSender;
                        smsItem.accountusagetypeid = sAccusage;
                        if (string.IsNullOrEmpty(smsItem.entityid) && !string.IsNullOrEmpty(sEntityId))
                        {
                            smsItem.entityid = sEntityId;
                        }

                        // Convert Records to JSON
                        sJsonString = new JavaScriptSerializer().Serialize(composeClass);
                        // Prepare and Send HTTP Request
                        try
                        {
                            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sBaseUrl);
                            byte[] data = new UTF8Encoding().GetBytes(sJsonString);

                            httpWReq.Method = sMethod;
                            httpWReq.ContentType = "application/json";
                            httpWReq.ContentLength = data.Length;

                            using (Stream stream = httpWReq.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }
                            using (HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse())
                            {
                                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                {
                                    string responseString = reader.ReadToEnd();
                                    if (nLogcount == 0)
                                    {
                                        lastInsertID = SaveSentItemFromDoveSoftV1(responseString, item.class_id);
                                    }
                                    SaveSendSMSListFromDoveSoftV1(responseString, lastInsertID, item.sms, item.class_id, item.receipient_id);

                                    ObjJsonData.Message = $"{listbulkSMS.Count} Messages Submitted successfully.";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                                    sResultMessage = Common.ErrorCode.Ok;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log Error (Handle Exception Appropriately)
                            Console.WriteLine($"Error Sending SMS: {ex.Message}");
                        }

                        nLogcount++;
                        SMSSkipcount++;
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommunicationController", "SendBulkSMSFromDovesoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommunicationController", "SendBulkSMSFromDovesoft", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }


        // save single SMS ...,
        private void SaveSentItemFromDoveSoft(string JsonResponse)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var ObjSMS = serializer.Deserialize<MessageResult.SMSResponse>(JsonResponse);

                // Checking SMS Count ....
                if (ObjSMS.smslist.sms != null && ObjSMS.smslist.sms.Count > 0)
                {
                    ObjJsonData.Message = " Messages Submitted successfully.";
                    //  ObjJsonData.Message = ObjSMS.message;
                    var ClassIds = liSMS.GroupBy(o => o.custom1).Select(x => new MessageResult.sms() { custom1 = x.FirstOrDefault().custom1 }).ToList();
                    sClassIds = string.Join(",", ClassIds.Select(o => o.custom1).ToArray());
                    sReceipientNo += string.Join(",", liSMS.Select(o => o.to).ToArray());
                    // sClassIds = sClassIds.TrimEnd(',');
                    // sReceipientNo = sReceipientNo.TrimEnd(',');
                    var ObjSaveRecord = new SentMessage();
                    ObjSaveRecord.CLASS_IDS = sClassIds;
                    ObjSaveRecord.START_TIME = sStartTime;
                    ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                    if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.ATTENDANCE)
                    {
                        ObjSaveRecord.CONTENT = "Absentees List";
                    }
                    else
                    {
                        //ObjSaveRecord.CONTENT = sTextMessage;
                        if (!string.IsNullOrEmpty(sTextMessage))
                        {
                            ObjSaveRecord.CONTENT = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                        }
                        if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.FEE_DUE_MESSAGE)
                        {
                            ObjSaveRecord.CONTENT = "Fee Due Message";
                        }
                    }
                    ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                    ObjSaveRecord.STATUS = ObjSMS.smslist.sms.FirstOrDefault().status;
                    ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                    ObjSaveRecord.SMS_MODE = sSmsMode;
                    ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                    ObjSaveRecord.ACADEMIC_YEAR_ID = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), "", true);
                }
                else
                {
                    var objsms = serializer.Deserialize<MessageResult.SMSSingleResponse>(JsonResponse);
                    //  ObjJsonData.Message = ObjSMS.message;
                    var ClassIds = liSMS.GroupBy(o => o.custom1).Select(x => new MessageResult.sms() { custom1 = x.FirstOrDefault().custom1 }).ToList();
                    sClassIds = string.Join(",", ClassIds.Select(o => o.custom1).ToArray());
                    sReceipientNo += string.Join(",", liSMS.Select(o => o.custom).ToArray());
                    // sClassIds = sClassIds.TrimEnd(',');
                    // sReceipientNo = sReceipientNo.TrimEnd(',');
                    var ObjSaveRecord = new SentMessage();
                    ObjSaveRecord.CLASS_IDS = sClassIds;
                    ObjSaveRecord.START_TIME = sStartTime;
                    ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                    if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.ATTENDANCE)
                    {
                        ObjSaveRecord.CONTENT = "Absentees List";
                    }
                    else
                    {
                        //ObjSaveRecord.CONTENT = sTextMessage;
                        if (!string.IsNullOrEmpty(sTextMessage))
                        {
                            ObjSaveRecord.CONTENT = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                        }
                        if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.FEE_DUE_MESSAGE)
                        {
                            ObjSaveRecord.CONTENT = "Fee Due Message";
                        }
                    }
                    ObjJsonData.Message = "Messages Submitted successfully.";
                    ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                    ObjSaveRecord.STATUS = objsms.smslist.sms.status;
                    ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                    ObjSaveRecord.SMS_MODE = sSmsMode;
                    ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                    ObjSaveRecord.ACADEMIC_YEAR_ID = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), "", true);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SMSController", "SaveSentItemFromDoveSoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SMSController", "SaveSentItemFromDoveSoft", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
        }
        public string SaveSentItemFromDoveSoftV1(string JsonResponse, string class_id)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var ObjSMS = serializer.Deserialize<MessageResult.SMSResponse>(JsonResponse);
                string sAcademicYear = (Session != null && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
    ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
    : " ";
                if (sAcademicYear == null || sAcademicYear == " ")
                {
                    sAcademicYear = acdemicYear;
                }

                //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : " ";

                // Checking SMS Count ....
                if (ObjSMS.smslist.sms != null && ObjSMS.smslist.sms.Count > 0)
                {
                    ObjJsonData.Message = " Messages Submitted successfully.";
                    //  ObjJsonData.Message = ObjSMS.message;
                    var ClassIds = liSMS.GroupBy(o => o.custom1).Select(x => new MessageResult.sms() { custom1 = x.FirstOrDefault().custom1 }).ToList();
                    //sClassIds = string.Join(",", ClassIds.Select(o => o.custom1).ToArray());
                    sReceipientNo += string.Join(",", liSMS.Select(o => o.to).ToArray());
                    // sClassIds = sClassIds.TrimEnd(',');
                    // sReceipientNo = sReceipientNo.TrimEnd(',');
                    var ObjSaveRecord = new SentMessage();
                    ObjSaveRecord.CLASS_IDS = class_id;
                    ObjSaveRecord.START_TIME = sStartTime;
                    ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                    if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.ATTENDANCE)
                    {
                        ObjSaveRecord.CONTENT = "Absentees List";
                    }
                    else
                    {
                        //ObjSaveRecord.CONTENT = sTextMessage;
                        if (!string.IsNullOrEmpty(sTextMessage))
                        {
                            ObjSaveRecord.CONTENT = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                        }
                        if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.FEE_DUE_MESSAGE)
                        {
                            ObjSaveRecord.CONTENT = "Fee Due Message";
                        }
                    }
                    ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                    ObjSaveRecord.STATUS = ObjSMS.smslist.sms.FirstOrDefault().status;
                    ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                    ObjSaveRecord.SMS_MODE = sSmsMode;
                    ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                    ObjSaveRecord.ACADEMIC_YEAR_ID = sAcademicYear;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), "", true);
                }
                else
                {
                    var objsms = serializer.Deserialize<MessageResult.SMSSingleResponse>(JsonResponse);
                    //  ObjJsonData.Message = ObjSMS.message;
                    var ClassIds = liSMS.GroupBy(o => o.custom1).Select(x => new MessageResult.sms() { custom1 = x.FirstOrDefault().custom1 }).ToList();
                    //sClassIds = string.Join(",", ClassIds.Select(o => o.custom1).ToArray());
                    sReceipientNo += string.Join(",", liSMS.Select(o => o.to).ToArray());
                    // sClassIds = sClassIds.TrimEnd(',');
                    // sReceipientNo = sReceipientNo.TrimEnd(',');
                    var ObjSaveRecord = new SentMessage();
                    ObjSaveRecord.CLASS_IDS = class_id;
                    ObjSaveRecord.START_TIME = sStartTime;
                    ObjSaveRecord.END_TIME = DateTime.Now.ToString(Common.Formats.TimeFormat);
                    if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.ATTENDANCE)
                    {
                        ObjSaveRecord.CONTENT = "Absentees List";
                    }
                    else
                    {
                        //ObjSaveRecord.CONTENT = sTextMessage;
                        if (!string.IsNullOrEmpty(sTextMessage))
                        {
                            ObjSaveRecord.CONTENT = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                        }
                        if (sSmsMode == Common.SUP_MESSAGE_TYPE_ID.FEE_DUE_MESSAGE)
                        {
                            ObjSaveRecord.CONTENT = "Fee Due Message";
                        }
                    }
                    ObjJsonData.Message = " Messages Submitted successfully.";
                    ObjSaveRecord.SMS_COUNT = Convert.ToString(sSmsCount);
                    ObjSaveRecord.STATUS = objsms.smslist.sms.status;

                    ObjSaveRecord.CREDIT_COUNT = sCreditCount;
                    ObjSaveRecord.SMS_MODE = sSmsMode;
                    ObjSaveRecord.RECEIPIENT_NOS = sReceipientNo;
                    ObjSaveRecord.ACADEMIC_YEAR_ID = sAcademicYear;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.SaveSentMessages), "", true);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SMSController", "SaveSentItemFromDoveSoft", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SMSController", "SaveSentItemFromDoveSoft", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
            }
            string lstInsertedId = resultArgs.RowUniqueId.ToString();

            return lstInsertedId;
        }
        static string FixJsonToArray(string jsonResponse)
        {
            // Fix concatenated objects into a valid JSON array
            string fixedJson = "[" + Regex.Replace(jsonResponse, "},\\s*\\{", "},{") + "]";
            return fixedJson;
        }
        private void SaveSendSMSListFromDoveSoftV1(string JsonResponse, string lastInsertID, string sms, string ClassId, string recipient)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonResponse = JsonResponse.TrimEnd(',');
                //JsonResponse = FixJsonToArray(JsonResponse);
                var ObjSMS = serializer.Deserialize<MessageResult.SMSResponse>(JsonResponse);

                // when I trying to access the Session object outside of a valid HTTP context so i changed here by yesudass

                string sAcademicYear = (Session != null && Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
 ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
 : " ";
                if (sAcademicYear == null || sAcademicYear == " ")
                {
                    sAcademicYear = acdemicYear;
                }
                //string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";

                var todayDate = DateTime.Now.ToString("yyyy-MM-dd");
                var CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                // Checking SMS Count ....
                if (ObjSMS.smslist.sms != null && ObjSMS.smslist.sms.Count > 0)
                {
                    var ObjSaveRecord = new SentMessage();
                    ObjJsonData.Message = "Messages Submitted successfully.";
                    string sSQL = string.Empty;
                    sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.RECEIPIENT_ID + ","
                           + Common.SENT_SMS_LIST.CLASS_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                           + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + "," + Common.SENT_SMS_LIST.STATUS + ","
                           + Common.SENT_SMS_LIST.CREDIT_COUNT + ")VALUES";
                    //string lstInsertedId = resultArgs.RowUniqueId.ToString();
                    string sTempMessage = string.Empty;
                    if (!string.IsNullOrEmpty(lastInsertID))
                    {
                        foreach (var item in ObjSMS.smslist.sms)
                        {
                            var customid = item.clientsmsid.ToString().Split(',');
                            var status = item.status.ToString();
                            Count++;

                            var liSave = liSMS.Where(s => s.custom1 == customid[0]).ToList();
                            if (liSave != null && liSave.Count > 0)
                            {
                                foreach (var lstsave in liSave)
                                {

                                    sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                    sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                    sSQL += @"('" + lastInsertID + "','" + lstsave.custom + "','" + lstsave.custom1 + "','" + sTempMessage + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + 0 + "','" + status + "','" + lstsave.CreditCount + "'),";
                                }
                            }
                            else
                            {
                                sTextMessage = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                sSQL += @"('" + lastInsertID + "','0','0','" + sTextMessage + "','0','" + item.mobileno + "','" + 0 + "','" + status + "','" + sCreditCount + "'),";
                            }
                        }
                        sSQL = sSQL.TrimEnd(',');

                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                            ObjJsonData.Message = Count + " Messages Submitted successfully.";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "Failed to send sms";
                    }
                }
                else
                {
                    var objsms = serializer.Deserialize<MessageResult.SMSSingleResponse>(JsonResponse);
                    var ObjSaveRecord = new SentMessage();
                    ObjJsonData.Message = " Messages Submitted successfully.";
                    string sSQL = string.Empty;
                    sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                           + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + "," + Common.SENT_SMS_LIST.STATUS + ","
                           + Common.SENT_SMS_LIST.CREDIT_COUNT + ","
                           + Common.SENT_SMS_LIST.SENT_DATE + ","
                           + Common.SENT_SMS_LIST.ACADEMIC_YEAR + ","
                           + Common.SENT_SMS_LIST.SENT_TIME + ","
                           + Common.SENT_SMS_LIST.TEMPLATE_ID + ","
                           + Common.SENT_SMS_LIST.CLASS_ID + ","
                           + Common.SENT_SMS_LIST.RECEIPIENT_ID + ")VALUES";
                    //string lstInsertedId = resultArgs.RowUniqueId.ToString();
                    string sTempMessage = string.Empty;
                    if (!string.IsNullOrEmpty(lastInsertID))
                    {

                        var customids = objsms.smslist.sms.clientsmsid.ToString().Split(',');
                        var status = objsms.smslist.sms.status.ToString();
                        Count++;
                        //if (liSMS != null && liSMS.Count > 0)
                        //{
                        var liSave = liSMS.Where(s => s.custom == customids[0]).ToList();
                        if (liSave != null && liSave.Count > 0)
                        {
                            foreach (var lstsave in liSave)
                            {
                                sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                sSQL += @"('" + lastInsertID + "','" + sTempMessage + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + 0 + "','" + status + "','" + lstsave.CreditCount + "','" + todayDate + "','" + sAcademicYear + "','" + CurrentTime + "','0','" + ClassId + "','" + recipient + "'),";
                            }
                        }
                        else
                        {
                            sTextMessage = sms;
                            sSQL += @"('" + lastInsertID + "','" + sTextMessage + "','0','" + objsms.smslist.sms.mobileno + "','" + 0 + "','" + status + "','1','" + todayDate + "','" + sAcademicYear + "','" + CurrentTime + "','0','" + ClassId + "','" + recipient + "'),";
                        }
                        //}
                        //else
                        //{
                        //    ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                        //}
                        sSQL = sSQL.TrimEnd(',');
                        sSQL = sSQL.Replace("'", "\"");
                        sSQL = sSQL.Replace("St.Joseph\"s", "St.Joseph's");
                        //resultArgs = CMSHandler.SaveOrUpdate(ObjSaveRecord, sSQL, "", true);

                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                            ObjJsonData.Message = Count + " Messages Submitted successfully.";
                        }
                    }
                    else
                    {

                        //var ErrorResponce = JsonConvert.DeserializeObject<MessageResult.ErrorResponceFromSolutionInfi>(JsonResponse);
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("SMSController", "SaveSentItem", "Err MSg: " + JsonResponse, "Query is empty!");
                            // objHandler.WriteError("SMSController", "SaveSentItem", ErrorResponce.message);
                        }
                        ObjJsonData.Message = "FailedToSendMessage";
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SMSController", "SaveSendSMSList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SMSController", "SaveSendSMSList", ex.Message);
                }
            }

        }
        // save Bulk List SMS ...,
        private void SaveSendSMSListFromDoveSoft(string JsonResponse)
        {
            try
            {
                // Convert Form Json To Model ....
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                var ObjSMS = serializer.Deserialize<MessageResult.SMSResponse>(JsonResponse);

                // Checking SMS Count ....
                if (ObjSMS.smslist.sms != null && ObjSMS.smslist.sms.Count > 0)
                {
                    var ObjSaveRecord = new SentMessage();
                    ObjJsonData.Message = " Messages Submitted successfully.";
                    string sSQL = string.Empty;
                    sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.RECEIPIENT_ID + ","
                           + Common.SENT_SMS_LIST.CLASS_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                           + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + "," + Common.SENT_SMS_LIST.STATUS + ","
                           + Common.SENT_SMS_LIST.CREDIT_COUNT + ")VALUES";
                    string lstInsertedId = resultArgs.RowUniqueId.ToString();
                    string sTempMessage = string.Empty;
                    if (!string.IsNullOrEmpty(lstInsertedId))
                    {
                        foreach (var item in ObjSMS.smslist.sms)
                        {
                            var customid = item.clientsmsid.ToString().Split(',');
                            var status = item.status.ToString();
                            Count++;
                            if (liSMS != null && liSMS.Count > 0)
                            {
                                var liSave = liSMS.Where(s => s.custom1 == customid[0]).ToList();
                                if (liSave != null && liSave.Count > 0)
                                {
                                    foreach (var lstsave in liSave)
                                    {

                                        sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                        sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                        sSQL += @"('" + lstInsertedId + "','" + lstsave.custom + "','" + lstsave.custom1 + "','" + sTempMessage + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + 0 + "','" + status + "','" + lstsave.CreditCount + "'),";
                                    }
                                }
                                else
                                {
                                    sTextMessage = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                    sSQL += @"('" + lstInsertedId + "','0','0','" + sTextMessage + "','0','" + item.mobileno + "','" + 0 + "','" + status + "','" + sCreditCount + "'),";
                                }
                            }
                            else
                            {
                                ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                            }

                        }
                        sSQL = sSQL.TrimEnd(',');
                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                            ObjJsonData.Message = Count + " Messages Submitted successfully.";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "Failed to send sms";
                    }
                }
                else
                {
                    var objsms = serializer.Deserialize<MessageResult.SMSSingleResponse>(JsonResponse);
                    var ObjSaveRecord = new SentMessage();
                    ObjJsonData.Message = " Messages Submitted successfully.";
                    string sSQL = string.Empty;
                    sSQL = @"INSERT INTO SENT_SMS_LIST (" + Common.SENT_SMS_LIST.SENT_ITEM_ID + "," + Common.SENT_SMS_LIST.RECEIPIENT_ID + ","
                           + Common.SENT_SMS_LIST.CLASS_ID + "," + Common.SENT_SMS_LIST.CONTENT + "," + Common.SENT_SMS_LIST.IS_STAFF + ","
                           + Common.SENT_SMS_LIST.MOBILE_NO + "," + Common.SENT_SMS_LIST.TRANSACTION_ID + "," + Common.SENT_SMS_LIST.STATUS + ","
                           + Common.SENT_SMS_LIST.CREDIT_COUNT + ")VALUES";
                    string lstInsertedId = resultArgs.RowUniqueId.ToString();
                    string sTempMessage = string.Empty;
                    if (!string.IsNullOrEmpty(lstInsertedId))
                    {
                        var customids = objsms.smslist.sms.clientsmsid.ToString().Split(',');
                        var status = objsms.smslist.sms.status.ToString();
                        Count++;
                        //if (liSMS != null && liSMS.Count > 0)
                        //{
                        var liSave = liSMS.Where(s => s.custom == customids[0]).ToList();
                        if (liSave != null && liSave.Count > 0)
                        {
                            foreach (var lstsave in liSave)
                            {

                                sTempMessage = (!string.IsNullOrEmpty(sTextMessage)) ? sTextMessage : lstsave.message;
                                sTempMessage = sTempMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                                sSQL += @"('" + lstInsertedId + "','" + lstsave.custom + "','" + lstsave.custom1 + "','" + sTempMessage + "','" + lstsave.custom2 + "','" + lstsave.to + "','" + 0 + "','" + status + "','" + lstsave.CreditCount + "'),";
                            }
                        }
                        else
                        {
                            sTextMessage = sTextMessage.Replace(Common.Delimiter.SINGLEQUOTE, "");
                            sSQL += @"('" + lstInsertedId + "','0','0','" + sTextMessage + "','0','" + objsms.smslist.sms.mobileno + "','" + 0 + "','" + status + "','" + sCreditCount + "'),";
                        }
                        //}
                        //else
                        //{
                        //    ObjJsonData.Message = Common.Messages.NoMobileNumbersAvailable;
                        //}


                        sSQL = sSQL.TrimEnd(',');
                        using (MySQLHandler ObjHandler = new MySQLHandler())
                        {
                            var resultArg = ObjHandler.ExecuteAsScripts(sSQL);
                            ObjJsonData.Message = Count + " Messages Submitted successfully.";
                        }
                    }
                    else
                    {

                        //var ErrorResponce = JsonConvert.DeserializeObject<MessageResult.ErrorResponceFromSolutionInfi>(JsonResponse);
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("SMSController", "SaveSentItem", "Err MSg: " + JsonResponse, "Query is empty!");
                            // objHandler.WriteError("SMSController", "SaveSentItem", ErrorResponce.message);
                        }
                        ObjJsonData.Message = "FailedToSendMessage";
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SMSController", "SaveSendSMSList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SMSController", "SaveSendSMSList", ex.Message);
                }
            }

        }
        #endregion

        #region Absentees SMS
        public ActionResult DailyAbsenteesSMS()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data;
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    {
                        ObjViewModel.ProgrammeMode = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    }
                    if (FetchApplicationType != null && FetchApplicationType.Count > 0)
                    {
                        ObjViewModel.ApplicationType = new SelectList(FetchApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    if (FetchShift != null && FetchShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (FetchHours != null && FetchHours.Count > 0)
                    {
                        ObjViewModel.Hours = new SelectList(FetchHours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("CommunicationController", "DailyAbsenteesSMS", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult BindProgrammesByShiftAndProgrammeMode(string sShift, string sProgrammeMode, string sApplicationType)
        {
            string sOption = string.Empty;
            string sSql = string.Empty;
            CP_PROGRAMME_GROUP objModel = new CP_PROGRAMME_GROUP();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.APPTYPE_ID = sApplicationType;
                    sSql = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammesByShiftAndProgrammeMode).Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.SHIFT, sShift).Replace(Common.Delimiter.QUS + Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_MODE, sProgrammeMode);
                    var Programme = (List<CP_PROGRAMME_2017>)CMS.DAO.CMSHandler.FetchData<CP_PROGRAMME_2017>(objModel, sSql).DataSource.Data;
                    if (Programme != null && Programme.Count > 0)
                    {
                        foreach (var item in Programme)
                        {
                            sOption += "<option value='" + item.PROGRAMME_GROUP_ID + "' >" + item.PROGRAMME_NAME + "</option>";
                        }
                    }
                }
                else
                    return Json(sOption);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindProgrammesByShiftAndProgrammeMode", "CommunicationController", "Err MSg: " + ex.Message, "Query is empty!");
                }
            }
            return Json(sOption);
        }

        public ActionResult BindAbsenteeslistByCurrentDate(string sProgrammeGroupIds, string sHoursIds)
        {
            AttendanceViewModel objAttendance = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAbsenteesListByCurrentDateAndProgrammeGroupIds).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_SPL_ATTENDANCE_TYPE.HOUR_FROM, sHoursIds);
                    objAttendance.liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("CommunicationController", "BindAbsenteeslistByCurrentDate", ex.Message);
                }
            }

            return View(objAttendance);
        }

        public JsonResult SendSMSAbsenteeslistByCurrentDate(string sProgrammeGroupIds, string sHoursIds)
        {
            AttendanceViewModel objAttendance = new AttendanceViewModel();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAbsenteesListByCurrentDateAndProgrammeGroupIds).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_SPL_ATTENDANCE_TYPE.HOUR_FROM, sHoursIds);
                    var liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    if (liAttendanceReport != null && liAttendanceReport.Count > 0)
                    {
                        foreach (var item in liAttendanceReport)
                        {
                            string sAbsentMessage = string.Empty;
                            string sTempId = string.Empty;
                            var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.DailyAbsenteesSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicYear).DataSource.Data;
                            if (MessageContent != null && MessageContent.Count > 0)
                            {
                                sAbsentMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, item.FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, item.CLASS_NAME).Replace(Common.Delimiter.AT + "ATTENDENCE_DATE", item.ATTENDENCE_DATE);
                                sTempId = MessageContent.FirstOrDefault().FIELDS_IDS;
                                string sCount = ValidateCreditCount(sAbsentMessage.Length);
                                sCreditCount = sCount;
                            }
                            else
                            {
                                ObjJsonData.Message = "No Absentees Template Found ! Please Meet ERP Admin !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                            }

                            sSmsMode = Common.MessageType.ATTENDANCE;
                            if (!string.IsNullOrEmpty(item.MOBILE))
                            {
                                //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sAbsentMessage, custom = item.STUDENT_ID, custom1 = item.CLASS_ID, CreditCount = sCount });
                                listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sAbsentMessage, tempid = sTempId });
                            }
                        }
                        SendMessage();
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.NoContent;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("CommunicationController", "SendSMSAbsenteeslistByCurrentDate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Absentees SMS For SJC
        public ActionResult DailyAbsenteesSMSForSJC()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liProgrammeGroup = DapperHandler.FetchAll<CP_PROGRAMME_GROUP>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).ToList();
                    if (liProgrammeGroup != null && liProgrammeGroup.Count > 0)
                    {
                        ObjViewModel.ProgrammeList = new SelectList(liProgrammeGroup, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_GROUP_ID, Common.SUP_APPTYPE_PROG_GROUPS.PROGRAMME_NAME);
                    }

                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                    {
                        ObjViewModel.Hours = new SelectList(FetchHours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("CommunicationController", "DailyAbsenteesSMS", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindAbsenteeslist(string sProgrammeGroupIds, string sHoursIds, string sClassId)
        {
            AttendanceViewModel objAttendance = new AttendanceViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    // Regular College Attendance ....,
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.BindRegularAbsenteesListByProgrammeHourAndClass).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_SPL_ATTENDANCE_TYPE.HOUR_FROM, sHoursIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                    objAttendance.liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;

                    if (objAttendance.liAttendanceReport == null)
                    {
                        // Online Attendance ....,
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.BindOnlineAbsenteesListByProgrammeHourAndClass).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_SPL_ATTENDANCE_TYPE.HOUR_FROM, sHoursIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                        objAttendance.liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("CommunicationController", "BindAbsenteeslistByCurrentDate", ex.Message);
                }
            }

            return View(objAttendance);
        }

        public JsonResult SendAbsenteesSMS(string sProgrammeGroupIds, string sHoursIds, string sClassId)
        {
            AttendanceViewModel objAttendance = new AttendanceViewModel();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.BindRegularAbsenteesListByProgrammeHourAndClass).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_SPL_ATTENDANCE_TYPE.HOUR_FROM, sHoursIds);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                    var liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;
                    if (liAttendanceReport == null)
                    {
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.BindOnlineAbsenteesListByProgrammeHourAndClass).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeGroupIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.SUP_SPL_ATTENDANCE_TYPE.HOUR_FROM, sHoursIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);
                        liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;
                    }


                    if (liAttendanceReport != null && liAttendanceReport.Count > 0)
                    {
                        foreach (var item in liAttendanceReport)
                        {
                            string sAbsentMessage = string.Empty;
                            string sTempId = string.Empty;
                            var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.DailyAbsenteesSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicYear).DataSource.Data;
                            if (MessageContent != null && MessageContent.Count > 0)
                            {
                                sAbsentMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, item.FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, item.CLASS_NAME).Replace(Common.Delimiter.AT + "ATTENDENCE_DATE", item.ATTENDENCE_DATE);
                                sTempId = MessageContent.FirstOrDefault().FIELDS_IDS;
                                string sCount = ValidateCreditCount(sAbsentMessage.Length);
                                sCreditCount = sCount;
                            }
                            else
                            {
                                ObjJsonData.Message = "No Absentees Template Found ! Please Meet ERP Admin !";
                                ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                            }

                            sSmsMode = Common.MessageType.ATTENDANCE;
                            if (!string.IsNullOrEmpty(item.MOBILE))
                            {
                                //liSMS.Add(new MessageResult.sms() { to = item.MOBILE, message = sAbsentMessage, custom = item.STUDENT_ID, custom1 = item.CLASS_ID, CreditCount = sCount });
                                listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE, sms = sAbsentMessage, tempid = sTempId });
                            }
                        }
                        SendMessage();
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.NoContent;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("CommunicationController", "SendSMSAbsenteeslistByCurrentDate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Mail
        public ActionResult GeneralMail()
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    var Programme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme), sAcademicYear).DataSource.Data;
                    var StaffCategoryList = (List<Sup_SubCategory>)CMSHandler.FetchData<Sup_SubCategory>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSubCategory), sAcademicYear).DataSource.Data;
                    var StaffType = (List<SUP_STAFF_TYPE>)CMSHandler.FetchData<SUP_STAFF_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupStaffType), sAcademicYear).DataSource.Data;
                    var RoleForStaff = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                    if (Programme != null && Programme.Count > 0)
                        objViewModel.Programme = new SelectList(Programme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    if (StaffType != null && StaffType.Count > 0)
                        objViewModel.StaffType = new SelectList(StaffType, Common.SUP_STAFF_TYPE.STAFF_TYPE, Common.SUP_STAFF_TYPE.STAFF_TYPE_NAME);
                    if (StaffCategoryList != null && StaffCategoryList.Count > 0)
                        objViewModel.StaffCategoryList = new SelectList(StaffCategoryList, Common.sup_staff_subcategory.STF_CATEGORY_ID, Common.sup_staff_subcategory.STF_CATEGORY);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("CommunicationController", "GeneralMail", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("CommunicationController", "GeneralMail", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }

        public JsonResult SendTestEmail(string sTestEmailId, string sEmailContent)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string Msresult = string.Empty; sEmailContent = "<b>this is your UserName : EX0156; Password : 9443426462 .xyz xyz xyz xyz xyz xyz xyz xyz xyz xyz . from st.joseph's college - cuddalore.:: this is your UserName : EX0156; Password : 9443426462 .xyz xyz xyz xyz xyz xyz xyz xyz xyz xyz . from st.joseph's college - cuddalore. :: this is your UserName : EX0156; Password : 9443426462 .xyz xyz xyz xyz xyz xyz xyz xyz xyz xyz . from st.joseph's college - cuddalore.</b>";
                    if (!string.IsNullOrEmpty(sTestEmailId) && !string.IsNullOrEmpty(sEmailContent))
                    {
                        //byte[] bytes = Convert.FromBase64String(sEmailContent);
                        //sEmailContent = System.Text.Encoding.UTF8.GetString(bytes);
                        string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID]).ToString() : string.Empty;
                        EMAIL_SETTING EmailSetting = new EMAIL_SETTING();
                        EmailSetting.ACADEMIC_YEAR_ID = sAcademicYearId;
                        var liEmailSetting = (List<EMAIL_SETTING>)CMSHandler.FetchData<EMAIL_SETTING>(EmailSetting, CommunicationSQL.GetCommunicationSQL(CommunicationSQLCommands.FetchEmailSetting)).DataSource.Data;
                        if (liEmailSetting != null && liEmailSetting.Count > 0)
                        {
                            var liCollegeInfo = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), null).ToList();

                            var CollegeName = liCollegeInfo.FirstOrDefault().COLLEGE_NAME;
                            var CollegeAdd = liCollegeInfo.FirstOrDefault().PLACE;
                            var CollegeLogo = liCollegeInfo.FirstOrDefault().COLLEGE_LOGO;
                            //MailMessage message = new MailMessage();
                            //MailMessage mail = new MailMessage();
                            //SmtpClient SmtpServer = new SmtpClient(liEmailSetting.FirstOrDefault().SMTP_HOST,587);
                            // mail.From = new MailAddress(liEmailSetting.FirstOrDefault().FROM_EMAIL_ID);
                            // mail.To.Add(sTestEmailId);
                            // mail.Subject = "Message From " + SchoolName + "";
                            //mail.IsBodyHtml = true;
                            sEmailContent = sEmailContent.Replace("\"", "'");
                            string htmlBody = string.Empty;
                            htmlBody = @"<div id=':1ae' class='ii gt '><div id=':16a' class='a3s aXjCH m16307dfad2bcba8e'>
                                        <table border='0' cellpadding='0' cellspacing='0' style='background-color:#f1f1f1;width:100%;height:100%;font-family:sans-serif;padding-top:36px;border-spacing:0'>
                                        <tbody>
                                        <tr>
                                        <td align='center' valign='middle'>
                                        <table border='0' cellpadding='0' cellspacing='0' style='background-color:#ffffff;line-height:18px;border-spacing:0;width:90%;max-width:680px;min-width:280px'>
                                        <tbody>
                                        <tr style='background-color:#42dbf4'>
                                        <td align='left' style='font-style:italic;font-size:14px;color:white;background-color:#42dbf4'>
                                        <span style='font-weight:bold;font-size:14px;color:black'>&nbsp;&nbsp;&nbsp;&nbsp; " + CollegeName + @"</span>
                                        <span style='font-style:italic;font-size:12px;color:black'>" + CollegeAdd + @"</span></td>
                                        <td align='right' style='padding:12px 16px 12px 0;min-width:96px;background-color:#42dbf4'><span>.</span>
                                        </td></tr>
                                        <tr>
                                        <td align='left' style='padding:16px;font-weight:500' colspan='2'>
                                        <span style='color:#212121;font-size:16px;line-height:22px'>
                                        <p style='text-decoration:none;color:#212121'> " + sEmailContent + @"</p>
                                        </span>
                                        <br>
                                        </td>
                                        </tr>
                                        <tr>
                                        <td colspan='3' style='text-align:center'>
                                        <hr style='width:90%;border:0;border-bottom:1px solid #ffffff'>
                                        </td>
                                        </tr>
  
                                        </tbody>
                                        </table>
                                        <br/><br/><br/><br/><br/>
                                        </td>
                                        </tr>
                                        <tr><td align='center'></td></tr>
                                        </tbody>
                                        </table>
                                      
                                        </div><br/><br/>";

                            //mail.Body = htmlBody;

                            //SmtpServer.Port = 587;
                            // SmtpServer.Credentials = new System.Net.NetworkCredential(liEmailSetting.FirstOrDefault().FROM_EMAIL_ID, liEmailSetting.FirstOrDefault().FROM_EMAIL_PASSWORD, "smtp.gmail.com");
                            //SmtpServer.EnableSsl = true;
                            //SmtpServer.Send(mail);
                            //SmtpServer.UseDefaultCredentials = false;

                            int sSMTP = 0;
                            string sDomain = string.Empty;
                            //bool bSSL = Convert.ToBoolean(ConfigurationManager.AppSettings[Common.AppSettingKeys.SSL].ToString());
                            sSMTP = (sSMTP == 0) ? Convert.ToInt16(ConfigurationManager.AppSettings[Common.AppSettingKeys.SMTPPORT].ToString()) : sSMTP;
                            sDomain = (string.IsNullOrEmpty(sDomain)) ? ConfigurationManager.AppSettings[Common.AppSettingKeys.Domain].ToString() : sDomain;
                            MailMessage message = new MailMessage(liEmailSetting.FirstOrDefault().FROM_EMAIL_ID, sTestEmailId);
                            message.Subject = "Message From " + CollegeName + "";
                            message.Body = htmlBody;
                            message.BodyEncoding = Encoding.UTF8;
                            message.IsBodyHtml = true;
                            SmtpClient client = new SmtpClient(sDomain, sSMTP);
                            System.Net.NetworkCredential basicCredential = new NetworkCredential(liEmailSetting.FirstOrDefault().FROM_EMAIL_ID, liEmailSetting.FirstOrDefault().FROM_EMAIL_PASSWORD);
                            client.EnableSsl = true;
                            client.UseDefaultCredentials = true;
                            client.Credentials = basicCredential;
                            client.Send(message);
                            ObjJsonData.Message = Common.ErrorCode.Accepted;

                        }
                        else
                        {
                            ObjJsonData.Message = "Please Configure E-mail Setting.!";
                            ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                        ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("EmailController", "SendTestEmail", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("EmailController", "SendTestEmail", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Absentees SMS For Common
        public ActionResult DailyAbsenteesSMSForReception()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var FetchProgrammeMode = (List<SupProgrammeMode>)CMSHandler.FetchData<SupProgrammeMode>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupProgrammeMode)).DataSource.Data;
                    var FetchApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    var classYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (FetchProgrammeMode != null && FetchProgrammeMode.Count > 0)
                    {
                        ObjViewModel.ProgrammeMode = new SelectList(FetchProgrammeMode, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE_ID, Common.SUP_PROGRAMME_MODE.PROGRAMME_MODE);
                    }
                    if (FetchApplicationType != null && FetchApplicationType.Count > 0)
                    {
                        ObjViewModel.ApplicationType = new SelectList(FetchApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }
                    if (FetchShift != null && FetchShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    if (classYear != null && classYear.Count > 0)
                    {
                        ObjViewModel.ClassYear = new SelectList(classYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("CommunicationController", "DailyAbsenteesSMS", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult GetProgramByShiftclasslevel(string sShiftId, string programmeLevel)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sOption = string.Empty;
            var liProgram = new List<CP_PROGRAMME_2017>();
            var ObjShift = new Sup_Shift();
            ObjShift.SHIFT_ID = sShiftId;
            liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(new CP_PROGRAMME_2017() { PROGRAMME_LEVEL = programmeLevel, SHIFT = sShiftId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupByShiftIdAndProgrammeLevel), sAcademicYear).DataSource.Data;
            if (liProgram != null && liProgram.Count > 0)
            {
                foreach (var item in liProgram)
                {
                    sOption += "<option value='" + item.PROGRAMME_ID + "'>" + item.PROGRAMME_NAME + "</option>";
                }
            }

            return Json(sOption);
        }
        public JsonResult BindDDLClassList(string sProgramId, string sShiftId, string sClassYear)
        {
            string sClass = string.Empty;
            string sAcademicYear = string.Empty;
            cp_Classes_2017 objModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    objModel.SHIFT = sShiftId;
                    var Class = (List<cp_Classes_2017>)CMS.DAO.CMSHandler.FetchData<cp_Classes_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByShiftAndProgrammeGroup).Replace("?PROGRAMME_GROUP_ID", sProgramId).Replace("?CLASS_YEAR", sClassYear), sAcademicYear).DataSource.Data;
                    foreach (var item in Class)
                    {
                        sClass += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
                    }
                }
                else
                    return Json(sClass);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ConductCertificate", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ConductCertificate", "StudentController", ex.Message);
                }
            }
            return Json(sClass);
        }
        public ActionResult BindStudentListforAbsentSMS(string sClassId, string sProgramName)
        {
            CommunicationViewModel objViewModel = new CommunicationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sProgramName))
            {
                try
                {
                    objViewModel.LIStudentDetails = (List<Student_Personal_Info>)CMS.DAO.CMSHandler.FetchData<Student_Personal_Info>(new cp_Classes_2017() { PROGRAMME_GROUP_ID = sProgramName }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentListForAbsentSMS).Replace("?CLASS_ID", sClassId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StudentController", "BindStudentForTcSerialNo", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("StudentController", "BindStudentForTcSerialNo", ex.Message);
                    }
                }
            }

            return View(objViewModel);
        }
        public JsonResult SendAbsenteesSMSReception(string StudentId)
        {
            AttendanceViewModel objAttendance = new AttendanceViewModel();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    if (StudentId != null)
                    {
                        StudentId = StudentId.TrimEnd(',');
                        StudentId = StudentId.Replace("undefined", "");
                        string sSQL = SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentMobileNoForAbsentSMS).Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, StudentId);
                        var studentDetails = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(null, sSQL, sAcademicYear).DataSource.Data;

                        if (studentDetails != null && studentDetails.Count > 0)
                        {
                            var AttendanceDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            string sCount = string.Empty;
                            //int sCount = studentDetails.Count;
                            foreach (var item in studentDetails)
                            {

                                string sAbsentMessage = string.Empty;
                                string sTempId = string.Empty;
                                var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.DailyAbsenteesSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSMSTemplateText), sAcademicYear).DataSource.Data;
                                if (MessageContent != null && MessageContent.Count > 0)
                                {
                                    sAbsentMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.STU_PERSONAL_INFO.FIRST_NAME, item.FIRST_NAME).Replace(Common.Delimiter.AT + Common.CP_CLASSES_2017.CLASS_NAME, item.CLASS_NAME).Replace(Common.Delimiter.AT + "ATTENDANCE_DATE", AttendanceDate);
                                    sTempId = MessageContent.FirstOrDefault().FIELDS_IDS;
                                    sCount = ValidateCreditCount(sAbsentMessage.Length);
                                    sCreditCount = Convert.ToString(sCount);
                                }
                                else
                                {
                                    ObjJsonData.Message = "No Absentees Template Found ! Please Meet ERP Admin !";
                                    ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                                }
                                sSmsMode = Common.MessageType.ATTENDANCE;
                                if (!string.IsNullOrEmpty(item.MOBILE_NO))
                                {
                                    liSMS.Add(new MessageResult.sms() { to = item.MOBILE_NO, message = sAbsentMessage, custom = item.STUDENT_ID, custom1 = item.CLASS_ID, CreditCount = Convert.ToString(sCount) });
                                    listbulkSMS.Add(new MessageResult.listsms() { mobiles = item.MOBILE_NO, sms = sAbsentMessage, class_id = item.CLASS_ID, receipient_id = item.STUDENT_ID, tempid = sTempId });
                                }
                                else
                                {
                                }
                            }
                            SendMessageV1();
                        }
                        else
                        {
                            ObjJsonData.Message = Common.ErrorMessage.NoContent;
                            ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.NoContent;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NoContent;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("CommunicationController", "SendSMSAbsenteeslistByCurrentDate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}