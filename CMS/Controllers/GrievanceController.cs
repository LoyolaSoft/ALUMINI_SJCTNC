using CMS.DAO;
using CMS.Utility;
using CMS.SQL.Grievance;
using CMS.Models;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using CMS.SQL.Examination;

namespace CMS.Controllers
{
    public class GrievanceController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData objResultData = new JsonResultData();
        // GET: Grievance
        public ActionResult Index()
        {
            return View();
        }

        #region PostComplaints
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 

        public ActionResult GrievanceTerms()
        {
            return View();
        }
        public ActionResult PostComplaint()
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                var GrievanceType = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceType)).DataSource.Data;
                if (GrievanceType != null && GrievanceType.Count > 0)
                {
                    grievanceViewModel.GRIEVANCE_TYPE_ID = new SelectList(GrievanceType, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                }
            }
            return View(grievanceViewModel);
        }

      /// <summary>
      /// Posting complaints by using formdata
      /// </summary>
      /// <returns></returns>
        public JsonResult PostComplaints()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty;
            var TotalPath = string.Empty;
            string sGrievanceType =string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    //from formdata deserializing the sJsonData
                    var result = JsonConvert.DeserializeObject<GRIEVANCEINFO>(Request.Form["sJsonData"]);
                    GRIEVANCE_REAMINDERINFO grievanceRemainderInfo = new GRIEVANCE_REAMINDERINFO();
                    if (result!= null && result.USER_ID!=null)
                    {
                        //Fetching the greivance Type
                        resultArgs = GrievanceDataMethod.FetchGrievanceType(result.GRIEVANCE_TYPE_ID);
                        DataTable dataTable = new DataTable();
                        dataTable = resultArgs.DataSource.Table;
                        result.ACADEMIC_YEAR = sAcademicYear;
                        //Generating Grievance Code by using greivance type and prefix
                        if (dataTable.Rows.Count > 0)
                        {
                            var Prefix = dataTable.Rows[0][Common.GRIEVANCE_TYPE.PREFIX_CODE].ToString();
                            sGrievanceType= dataTable.Rows[0][Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME].ToString();
                            result.GRIEVANCE_CODE = result.GRIEVANCE_TYPE_ID + GenerateGrievanceCode(Prefix) + result.USER_ID;
                        }
                        //Inserting grievanceinfo
                        result.STATUS = 0.ToString();
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.AddGrievance), sAcademicYear);
                        if(resultArgs.Success)
                        {
                            var grievance = (List<GRIEVANCEINFO>)CMSHandler.FetchData<GRIEVANCEINFO>(result, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoByCode)).DataSource.Data;
                           
                               //File savings
                                if (Request.Files.Count > 0 && grievance.Count > 0)
                                {
                                    sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.GRIEVANCEATTACHMENTPOST_FILE_PATH + "\\" + sGrievanceType;
                                    if (!Directory.Exists(sDirectorypath))
                                    {
                                        Directory.CreateDirectory(sDirectorypath);  
                                    }

                                    for (int i = 0; i < Request.Files.Count; i++)
                                    {
                                        var file = Request.Files[i];
                                        var fileName = file.FileName;
                                        var sFile = fileName.Split('.');
                                        sFilePath = sDirectorypath + "\\" + sFile[0] + "_" + grievance[0].GRIEVANCE_ID + "." + sFile[1];
                                        string sSavePath = "\\" + Common.URLPages.GRIEVANCEATTACHMENTPOST_FILE_PATH + "\\" + sGrievanceType + "\\" + sFile[0] + "_" + grievance[0].GRIEVANCE_ID + "." + sFile[1];
                                        file.SaveAs(sFilePath);
                                        if (i == 0 && Request.Files.Count == 0)
                                            TotalPath = sSavePath;
                                        else
                                           if (i == 0)
                                            TotalPath = sSavePath;
                                        else
                                            TotalPath +=","+sSavePath  ;
                                    }
                                    GRIEVANCE_ATTACHMENTINFO grievanceAttachmentInfo = new GRIEVANCE_ATTACHMENTINFO();
                                    grievanceAttachmentInfo.ATTACHMENT_PATH = TotalPath;
                                    grievanceAttachmentInfo.GRIEVANCE_ID = grievance[0].GRIEVANCE_ID;
                                    grievanceAttachmentInfo.IS_TYPE = 0.ToString();
                                    resultArgs = CMSHandler.SaveOrUpdate(grievanceAttachmentInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.AddGrievanceAttachment));

                                }
                                if (grievance.Count > 0)
                                {
                                    grievanceRemainderInfo.GRIEVANCE_ID = grievance[0].GRIEVANCE_ID;
                                    grievanceRemainderInfo.IS_READ = 0.ToString();
                                    grievanceRemainderInfo.IS_TYPE = 1.ToString();
                                    resultArgs = CMSHandler.SaveOrUpdate(grievanceRemainderInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.AddRemainder), sAcademicYear);
                                }
                        }
                        else
                        {
                            objResultData.Message = Common.Messages.FailedToSaveRecords;
                        }
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "PostComplaint", ex.Message);
                    }
                }
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ViewComplaints
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewComplaints()
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && Session[Common.SESSION_VARIABLES.USER_ID]!=null)
            { 
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                grievanceInfo.USER_ID = sUserId;
                var grievance = (List<GRIEVANCE_VIEWINFO>) CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfo), sAcademicYear).DataSource.Data;

                if (grievance != null && grievance.Count > 0)
                {
                    foreach (var info in grievance)
                    {
                        if (info.GRIEVANCE_CONTENT != "")
                        {
                            byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                            info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                        }
                        if (info.REDRESSAL_CONTENT != "")
                        {
                            byte[] bytesRedressal = Convert.FromBase64String(info.REDRESSAL_CONTENT);
                            info.REDRESSAL_CONTENT = Encoding.UTF8.GetString(bytesRedressal);
                        }

                    }
                    grievanceViewModel.liGrievanceViewInfo = grievance;
                }

   

                var GrievanceType = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceType)).DataSource.Data;
                grievanceViewModel.liGrievanceType = GrievanceType;
                if (GrievanceType != null && GrievanceType.Count > 0)
                {
                    grievanceViewModel.GRIEVANCE_TYPE_ID = new SelectList(GrievanceType, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                }
            }
            return View(grievanceViewModel);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sGrievanceId"></param>
        /// <returns></returns>
        public JsonResult ViewComplaintsByGrievanceId(string sGrievanceId)
        {
            var grievanceuserInfo = new List<GRIEVANCE_USERINFO>();
            if(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                grievanceInfo.GRIEVANCE_ID = sGrievanceId;
               var  grievance =(List<GRIEVANCE_USERINFO>)CMSHandler.FetchData<GRIEVANCE_USERINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoById), sAcademicYear).DataSource.Data;

                if(grievance != null && grievance.Count>0)
                {
                    foreach(var info in grievance)
                    {
                        if (info.GRIEVANCE_CONTENT != null)
                        {
                            byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                            info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                        }
                    }
                    grievanceuserInfo = grievance;
                }


            }
            return Json(grievanceuserInfo, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sGrievanceId"></param>
        /// <param name="sFlag"></param>
        /// <returns></returns>
        public JsonResult ViewAttachment(string sGrievanceId,string sFlag)
        {
            var grievanceAttachmentInfo = new List<GRIEVANCE_ATTACHMENTINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sGrievanceId != null && sFlag!=null)
            {

                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                grievanceInfo.USER_ID = sUserId;
                grievanceInfo.GRIEVANCE_ID = sGrievanceId;
                if(sFlag=="1")
                grievanceAttachmentInfo = (List<GRIEVANCE_ATTACHMENTINFO>)CMSHandler.FetchData<GRIEVANCE_ATTACHMENTINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchRedressalAttachment), sAcademicYear).DataSource.Data;
                else
                    grievanceAttachmentInfo = (List<GRIEVANCE_ATTACHMENTINFO>)CMSHandler.FetchData<GRIEVANCE_ATTACHMENTINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchAttachment), sAcademicYear).DataSource.Data;

                
            }
            return Json(grievanceAttachmentInfo, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sGrievanceType"></param>
        /// <returns></returns>
        public JsonResult ViewComplaintsByType(string sGrievanceType)
        {
            var result = new List<GRIEVANCE_VIEWINFO>();
            if(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null && Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                    grievanceInfo.USER_ID = sUserId;
                    grievanceInfo.GRIEVANCE_TYPE_ID = sGrievanceType;
                    if (grievanceInfo.GRIEVANCE_TYPE_ID != null)
                    {
                        var grievance = (List<GRIEVANCE_VIEWINFO>) CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoByType),sAcademicYear).DataSource.Data;


                        if (grievance != null && grievance.Count > 0)
                        {
                            foreach (var info in grievance)
                            {
                                if (info.GRIEVANCE_CONTENT != "")
                                {
                                    byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                                    info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                                }
                                if(info.REDRESSAL_CONTENT!="")
                                {
                                    byte[] bytesRedressal = Convert.FromBase64String(info.REDRESSAL_CONTENT);
                                    info.REDRESSAL_CONTENT = Encoding.UTF8.GetString(bytesRedressal);
                                }
                               
                            }
                            result = grievance;
                        }
                    }
                    if(result==null)
                    {

                        objResultData.Message = Common.Messages.NoRecordsfound;
                        return Json(objResultData, JsonRequestBehavior.AllowGet);
                    }


                }
                catch(Exception ex)
                {
                    using(ErrorLog objHandler=new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "FetchComplaintsByType", ex.Message);
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[UserRights("GRIEVANCE INCHARGE")]
        public ActionResult ViewPostedComplaints()
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                if(sUserId != null)
                {
                    GRIEVANCE_MEMBERINFO grievanceMemberInfo = new GRIEVANCE_MEMBERINFO();
                    grievanceMemberInfo.STAFF_ID = sUserId;
                    var grievance = (List<GRIEVANCE_USERINFO>)CMSHandler.FetchData<GRIEVANCE_USERINFO>(grievanceMemberInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceByStaff), sAcademicYear).DataSource.Data;

                   
                   grievanceViewModel.liGrievanceUserInfo = grievance;
                    
                }
                
            }
            return View(grievanceViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sComplaintId"></param>
        /// <returns></returns>
        public JsonResult CloseComplaintById(string sComplaintId)
        {
          
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sComplaintId!=null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                grievanceInfo.GRIEVANCE_ID = sComplaintId;
                grievanceInfo.STATUS = 2.ToString();
                resultArgs = CMSHandler.SaveOrUpdate(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.CloseComplaintById), sAcademicYear);
                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
            }
            return Json(objResultData,JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region NewComplaints
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult UserNewComplaints()
        {
            var liGrievanceViewInfo = new List<GRIEVANCE_VIEWINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                grievanceInfo.USER_ID = sUserId;
                liGrievanceViewInfo = (List<GRIEVANCE_VIEWINFO>)CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UserNewComplaints), sAcademicYear).DataSource.Data;
            }
            return Json(liGrievanceViewInfo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult UserNewComplaintsView()
        {
            var liGrievanceViewInfo = new List<GRIEVANCE_VIEWINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                    grievanceInfo.USER_ID = sUserId;
                    liGrievanceViewInfo = (List<GRIEVANCE_VIEWINFO>)CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UserNewComplaints), sAcademicYear).DataSource.Data;

                    if (liGrievanceViewInfo != null && liGrievanceViewInfo.Count > 0)
                    {
                        foreach (GRIEVANCE_VIEWINFO info in liGrievanceViewInfo)
                        {
                            GRIEVANCE_REAMINDERINFO grievanceRemainderInfo = new GRIEVANCE_REAMINDERINFO();
                            grievanceRemainderInfo.GRIEVANCE_ID = info.GRIEVANCE_ID;
                            grievanceRemainderInfo.IS_READ = 1.ToString();
                            resultArgs = CMSHandler.SaveOrUpdate(grievanceRemainderInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UpdateReaminderInfo), sAcademicYear);
                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "UserNewComplaintsView", ex.Message);
                    }
                }
            }
            return Json(liGrievanceViewInfo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult StaffNewComplaints()
        {
            var liGrievanceUsernfo = new List<GRIEVANCE_USERINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCE_MEMBERINFO grievanceMemberInfo = new GRIEVANCE_MEMBERINFO();
                grievanceMemberInfo.STAFF_ID = sUserId;
                liGrievanceUsernfo = (List<GRIEVANCE_USERINFO>)CMSHandler.FetchData<GRIEVANCE_USERINFO>(grievanceMemberInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.StaffNewComplaints), sAcademicYear).DataSource.Data;
            }
            return Json(liGrievanceUsernfo);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult StaffNewComplaintsView()
        {
            var liGrievanceUserInfo = new List<GRIEVANCE_USERINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    GRIEVANCE_MEMBERINFO grievanceMemberInfo = new GRIEVANCE_MEMBERINFO();
                    grievanceMemberInfo.STAFF_ID = sUserId;
                    liGrievanceUserInfo = (List<GRIEVANCE_USERINFO>)CMSHandler.FetchData<GRIEVANCE_USERINFO>(grievanceMemberInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.StaffNewComplaints), sAcademicYear).DataSource.Data;

                    if (liGrievanceUserInfo != null && liGrievanceUserInfo.Count > 0)
                    {

                        foreach (GRIEVANCE_USERINFO info in liGrievanceUserInfo)
                        {
                            GRIEVANCE_REAMINDERINFO grievanceRemainderInfo = new GRIEVANCE_REAMINDERINFO();
                            grievanceRemainderInfo.GRIEVANCE_ID = info.GRIEVANCE_ID;
                            grievanceRemainderInfo.IS_READ = 1.ToString();
                            resultArgs = CMSHandler.SaveOrUpdate(grievanceRemainderInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UpdateStaffRemainderInfo), sAcademicYear);
                        }

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "StaffNewComplaintsView", ex.Message);
                    }
                }
            }
            return Json(liGrievanceUserInfo);
        }
        #endregion

        #region ReplyComplaints
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sComplaintId"></param>
        /// <returns></returns>
        public ActionResult Reply(string sComplaintId)
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null)
            {

                if(sComplaintId!=null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                    grievanceInfo.GRIEVANCE_ID = sComplaintId;
                    var result = (List<GRIEVANCE_USERINFO>)CMSHandler.FetchData<GRIEVANCE_USERINFO>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceByGrievanceId), sAcademicYear).DataSource.Data; ;
                    if (result!=null && result.Count>0)
                    {
                        //   grievanceViewModel.liGrievanceUserInfo =(List<GRIEVANCE_USERINFO>) resultArgs.DataSource.Data;
                        foreach (var info in result)
                        {
                            if(info.GRIEVANCE_CONTENT!="")
                            {
                                byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                                info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                            }
                           
                        }
                        grievanceViewModel.liGrievanceUserInfo = result;
                        grievanceViewModel.grievance_userinfo = grievanceViewModel.liGrievanceUserInfo[0];
                        TempData["grievance"] = grievanceViewModel.grievance_userinfo;
                    }
                }
            }
            return RedirectToAction("ReplyComplaint");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public ActionResult ReplyComplaint()
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            grievanceViewModel.grievance_userinfo = TempData["grievance"] as GRIEVANCE_USERINFO;
            object value = TempData["grievance"];
            TempData.Keep("grievance");
            return View(grievanceViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult ReplyComplaints()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty;
            var TotalPath = string.Empty;
            string sGrievanceType = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<GRIEVANCEINFO>(Request.Form["sJsonData"]);
                 
                    GRIEVANCE_REAMINDERINFO grievanceRemainderInfo = new GRIEVANCE_REAMINDERINFO();
                    GrievanceSQLCommands grievanceSql;
                    if (result!=null && result.GRIEVANCE_ID!=null)
                    {
                        var redressalInfo = new GRIEVANCE_REDRESSALINFO();
                        redressalInfo.GRIEVANCE_ID = result.GRIEVANCE_ID;
                        redressalInfo.REDRESSAL_CONTENT = result.GRIEVANCE_CONTENT;
                        //
                        var grievanceRedressal=(List<GRIEVANCE_REDRESSALINFO>)CMSHandler.FetchData<GRIEVANCE_REDRESSALINFO>(redressalInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceRedressalByGrievanceId)).DataSource.Data;

                        if (grievanceRedressal!=null)
                        grievanceSql =GrievanceSQLCommands.UpdateRedressalInfo;
                        else
                        grievanceSql =GrievanceSQLCommands.AddGrievanceRedressalInfo;
                        //
                        resultArgs = CMSHandler.SaveOrUpdate(redressalInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(grievanceSql), sAcademicYear);
                        //
                        if(resultArgs.Success)
                        {
                            grievanceRemainderInfo.GRIEVANCE_ID = result.GRIEVANCE_ID;
                            grievanceRemainderInfo.IS_READ = 0.ToString();
                            grievanceRemainderInfo.IS_TYPE = 2.ToString();
                            resultArgs = CMSHandler.SaveOrUpdate(grievanceRemainderInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.AddRemainder), sAcademicYear);
                            result.STATUS = 1.ToString();
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UpdateGrievanceInfoStatus), sAcademicYear);
                            if (Request.Files.Count > 0)
                            {

                                sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.GRIEVANCEATTACHMENTREPLY_FILE_PATH + "\\" + result.GRIEVANCE_TYPE_ID;
                                if (!Directory.Exists(sDirectorypath))
                                {
                                    Directory.CreateDirectory(sDirectorypath);
                                }
                                for (int i = 0; i < Request.Files.Count; i++)
                                {
                                    var file = Request.Files[i];
                                    string sfileName = file.FileName;
                                    var sFile = sfileName.Split('.');
                                    sFilePath = sDirectorypath + "\\" + sFile[0] + "_" + result.GRIEVANCE_ID + "." + sFile[1];
                                    string sSavePath = "\\" + Common.URLPages.GRIEVANCEATTACHMENTREPLY_FILE_PATH + "\\" + result.GRIEVANCE_TYPE_ID + "\\" + sFile[0] + "_" + result.GRIEVANCE_ID + "." + sFile[1];
                                    file.SaveAs(sFilePath);
                                    if (i == 0 && Request.Files.Count == 0)
                                        TotalPath = sSavePath;
                                    else
                                         if (i == 0)
                                        TotalPath = sSavePath;
                                    else
                                        TotalPath += "," + sSavePath;

                                }
                                GRIEVANCE_ATTACHMENTINFO grievanceAttachmentInfo = new GRIEVANCE_ATTACHMENTINFO();
                                grievanceAttachmentInfo.GRIEVANCE_ID = result.GRIEVANCE_ID;
                                grievanceAttachmentInfo.ATTACHMENT_NAME = result.GRIEVANCE_TITLE;
                                grievanceAttachmentInfo.IS_TYPE = 1.ToString();
                                grievanceAttachmentInfo.ATTACHMENT_PATH = TotalPath;
                                resultArgs = CMSHandler.SaveOrUpdate(grievanceAttachmentInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.AddGrievanceAttachment));
                            }
                        }
                        else
                        {
                            objResultData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "PostComplaint", ex.Message);
                    }
                }
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region ReopenComplaints
        public ActionResult ReOpenComplaints()
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();

            if(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null)
            {
                try
                {
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass)).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        grievanceViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var GrievanceType = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceType)).DataSource.Data;
                    if (GrievanceType != null && GrievanceType.Count > 0)
                    {
                        grievanceViewModel.GRIEVANCE_TYPE_ID = new SelectList(GrievanceType, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                    }
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "ReOpenComplaints", ex.Message);
                    }
                }
            }


            return View(grievanceViewModel);
        }
      
        public JsonResult ReOpenComplaint(string sDate,string sGrievanceType, string sClass)
        {
            var liGrievanceUserInfo = new List<GRIEVANCE_USERINFO>();
            if(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    REOPEN_GREIVANCE reOpenGrievance = new REOPEN_GREIVANCE();
                    reOpenGrievance.STAFF_ID = sUserId;
                    reOpenGrievance.CLASS_ID = sClass;
                    reOpenGrievance.GRIEVANCE_TYPE_ID = sGrievanceType;
                    reOpenGrievance.SUB_DATE_TIME = sDate;
                    liGrievanceUserInfo = (List<GRIEVANCE_USERINFO>)CMSHandler.FetchData<GRIEVANCE_USERINFO>(reOpenGrievance, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceCloseInfoByStaff), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "ReOpenComplaints", ex.Message);
                    }
                }

            }
            return Json(liGrievanceUserInfo, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ReOpen(string sGrievanceId)
        {
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                grievanceInfo.STATUS = 1.ToString();
                grievanceInfo.GRIEVANCE_ID = sGrievanceId;
                resultArgs = CMSHandler.SaveOrUpdate(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UpdateGrievanceInfoStatus));
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Report
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GrievanceReport()
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass)).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    grievanceViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                var GrievanceType = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceType)).DataSource.Data;
                if (GrievanceType != null && GrievanceType.Count > 0)
                {
                    grievanceViewModel.GRIEVANCE_TYPE_ID = new SelectList(GrievanceType, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                }

                var liMonth = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender), sAcademicYear).DataSource.Data;
                if (liMonth != null && liMonth.Count > 0)
                {
                    grievanceViewModel.MonthList = new SelectList(liMonth, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                }
            }
            return View(grievanceViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="sGrievanceType"></param>
        /// <returns></returns>
        public ActionResult BindDayWiseGrievanceReport(string sDate,string sGrievanceType)
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCE_REPORT grievanceReport = new GRIEVANCE_REPORT();
                grievanceReport.STAFF_ID = sUserId;
                grievanceReport.SUB_DATE_TIME = sDate;
                grievanceReport.GRIEVANCE_TYPE_ID = sGrievanceType;

                string sSql = SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoByDayWise);
                var result =(List<GRIEVANCE_VIEWINFO>) CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceReport, sSql.Replace(Common.Delimiter.QUS + Common.GRIEVANCE_REPORT.GRIEVANCE_TYPE_ID, grievanceReport.GRIEVANCE_TYPE_ID), sAcademicYear).DataSource.Data;
                if(result!=null && result.Count>0)
                {
                    foreach (var info in result)
                    {
                        if (info.GRIEVANCE_CONTENT != "")
                        {
                            byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                            info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                        }
                        if(info.REDRESSAL_CONTENT!="")
                        {
                            byte[] bytesRedressal = Convert.FromBase64String(info.REDRESSAL_CONTENT);
                            info.REDRESSAL_CONTENT = Encoding.UTF8.GetString(bytesRedressal);
                        }
                    }
                    grievanceViewModel.liGrievanceViewInfo = result;
                }

       
                // Fetch College Details ....
                grievanceViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;


            }
            return View(grievanceViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="sEndDate"></param>
        /// <param name="sGrievanceType"></param>
        /// <returns></returns>
        public ActionResult BindGroupWiseGrievanceReport(string sDate, string sEndDate, string sGrievanceType)
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCE_REPORT grievanceReport = new GRIEVANCE_REPORT();
                grievanceReport.STAFF_ID = sUserId;
                grievanceReport.START_DATE = sDate;
                grievanceReport.END_DATE = sEndDate;
                grievanceReport.GRIEVANCE_TYPE_ID = sGrievanceType;

                string sSql = SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoByGroupWise);
                var result = (List<GRIEVANCE_VIEWINFO>)CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceReport, sSql.Replace(Common.Delimiter.QUS + Common.GRIEVANCE_REPORT.GRIEVANCE_TYPE_ID, grievanceReport.GRIEVANCE_TYPE_ID), sAcademicYear).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    foreach (var info in result)
                    {
                        if (info.GRIEVANCE_CONTENT != "")
                        {
                            byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                            info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                        }
                        if (info.REDRESSAL_CONTENT != "")
                        {
                            byte[] bytesRedressal = Convert.FromBase64String(info.REDRESSAL_CONTENT);
                            info.REDRESSAL_CONTENT = Encoding.UTF8.GetString(bytesRedressal);
                        }
                    }
                    grievanceViewModel.liGrievanceViewInfo = result;
                }
                // Fetch College Details ....
                grievanceViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;

            }
            return View(grievanceViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMonth"></param>
        /// <param name="sGrievanceType"></param>
        /// <returns></returns>
        public ActionResult BindMonthWiseGrievanceReport(string sMonth, string sGrievanceType)
        {
            GrievanceViewModel grievanceViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCE_REPORT grievanceReport = new GRIEVANCE_REPORT();
                grievanceReport.STAFF_ID = sUserId;
                grievanceReport.DAY_ID = sMonth;
                grievanceReport.GRIEVANCE_TYPE_ID = sGrievanceType;

                string sSql = SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceInfoByMonthWise);
                var result = (List<GRIEVANCE_VIEWINFO>)CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceReport, sSql.Replace(Common.Delimiter.QUS + Common.GRIEVANCE_REPORT.GRIEVANCE_TYPE_ID, grievanceReport.GRIEVANCE_TYPE_ID), sAcademicYear).DataSource.Data;

                if (result != null && result.Count > 0)
                {
                    foreach (var info in result)
                    {
                        if (info.GRIEVANCE_CONTENT != "")
                        {
                            byte[] bytes = Convert.FromBase64String(info.GRIEVANCE_CONTENT);
                            info.GRIEVANCE_CONTENT = Encoding.UTF8.GetString(bytes);
                        }
                        if (info.REDRESSAL_CONTENT != "")
                        {
                            byte[] bytesRedressal = Convert.FromBase64String(info.REDRESSAL_CONTENT);
                            info.REDRESSAL_CONTENT = Encoding.UTF8.GetString(bytesRedressal);
                        }
                    }
                    grievanceViewModel.liGrievanceViewInfo = result;
                }
                // Fetch College Details ....
                grievanceViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
            }
            return View(grievanceViewModel);
        }
        #endregion

        #region Remaind
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sGrievanceId"></param>
        /// <returns></returns>
        public JsonResult Remaind(string sGrievanceId)
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sGrievanceId != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                GRIEVANCE_REMAIND grievanceRemaind = new GRIEVANCE_REMAIND();
                grievanceRemaind.GRIEVANCE_ID = sGrievanceId;
                resultArgs = CMSHandler.SaveOrUpdate(grievanceRemaind, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.AddRemaind), sAcademicYear);
                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadRemaind()
        {
            var liGrievanceUserInfo = new List<GRIEVANCE_VIEWINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                GRIEVANCE_VIEWINFO grievanceViewInfo = new GRIEVANCE_VIEWINFO();
                grievanceViewInfo.STAFF_ID = sUserId;
                resultArgs = CMSHandler.FetchData<GRIEVANCE_VIEWINFO>(grievanceViewInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchRemaind), sAcademicYear);
                liGrievanceUserInfo = (List<GRIEVANCE_VIEWINFO>)resultArgs.DataSource.Data;
            }
            return Json(liGrievanceUserInfo, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sGrievanceId"></param>
        /// <returns></returns>
        public JsonResult UpdateRemaind(string sGrievanceId)
        {
            
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && sGrievanceId!=null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                GRIEVANCE_REMAIND grievanceRemind = new GRIEVANCE_REMAIND();
                grievanceRemind.GRIEVANCE_ID = sGrievanceId;
                grievanceRemind.IS_VIEWED = 1.ToString();
                resultArgs = CMSHandler.SaveOrUpdate(grievanceRemind, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UpdateRemaind), sAcademicYear);
                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
            }
            return Json(objResultData,JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region FetchTotalcomplaints

            public JsonResult FetchTotalComplaintsForUser()
            {
            var grievanceTotal = new List<GREIVANCE_TOTAL>();
            GRIEVANCEINFO grievanceInfo = new GRIEVANCEINFO();
                if(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]!=null)
                {
                try
                {
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    grievanceInfo.USER_ID = sUserId;
                    resultArgs = CMSHandler.FetchData<GREIVANCE_TOTAL>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchTotalComplaintsForUser), sAcademicYear);
                    if (resultArgs.Success)
                        grievanceTotal = (List<GREIVANCE_TOTAL>)resultArgs.DataSource.Data;
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "FetchTotalComplaintsForUser", ex.ToString());
                    }
                }
                      
            }
                return Json(grievanceTotal, JsonRequestBehavior.AllowGet);
            }
          public JsonResult FetchTotalComplaintsForStaff()
        {
            var  grievanceTotal = new List<GREIVANCE_TOTAL>();
            
            GRIEVANCE_MEMBERINFO grievanceInfo = new GRIEVANCE_MEMBERINFO();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    grievanceInfo.STAFF_ID = sUserId;
                    resultArgs = CMSHandler.FetchData<GREIVANCE_TOTAL>(grievanceInfo, SQL.Grievance.GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.StaffTotalComplaints), sAcademicYear);
                    if (resultArgs.Success)
                        grievanceTotal = (List<GREIVANCE_TOTAL>)resultArgs.DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Grievance", "FetchTotalComplaintsForStaff", ex.ToString());
                    }
                }

            }
            return Json(grievanceTotal, JsonRequestBehavior.AllowGet);
        }
        //public string GenerateGrievanceCode(string prefix)
        //{
        //    string sgrievanceCode = string.Empty;


        //    return sgrievanceCode;
        //}
        /// <summary>
        /// This action is used to show Grievancemembers List
        /// </summary>
        /// <returns>GrievanceViewModel</returns>
        public ActionResult GrievanceMemebersList()
        {
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {

                    objViewModel.liGrievancemembers = (List<GRIEVANCEMEMBERSINFO>)CMSHandler.FetchData<GRIEVANCEMEMBERSINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceMembersList)).DataSource.Data;
                    objViewModel.liGrievanceandredrassalinfo = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchTotalMembersandNewGrievanceCount)).DataSource.Data;
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "GrievanceMembersList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "GrievanceMembersList", ex.Message);
                    }
                }
            }
            else            
               return RedirectToAction("ErrorMessage", "Error");           
           return View(objViewModel);
        }
        public JsonResult Fetchstaffandgrievancetype()
        {
            GrievanceViewModel objviewModel = new GrievanceViewModel();
            string sStaff = string.Empty;
            string sGrievane = string.Empty;
            string sRole = string.Empty;
            try
            {
                
                var StaffList = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchStaffList)).DataSource.Data;
                var GrievanceList = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceTypeList)).DataSource.Data;
                var RoleList = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceRole)).DataSource.Data;
                if (StaffList != null)
                {
                    foreach (var item in StaffList)
                    {
                        sStaff += "<option roles='" + item.ROLES + "' value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                }
                if (GrievanceList != null)
                {
                    foreach (var item in GrievanceList)
                    {
                        sGrievane += "<option value='" + item.GRIEVANCE_TYPE_ID + "'>" + item.GRIEVANCE_TYPE_NAME + "</option>";
                    }
                }
                if (RoleList != null)
                {
                    foreach (var item in RoleList)
                    {
                        sRole += "<option value='" + item.ROLE_ID + "'>" + item.ROLE_NAME + "</option>";
                    }
                }
            }
            catch(Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GrievanceController", "Fetchstaffandgrievancetype", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GrievanceController", "Fetchstaffandgrievancetype", ex.Message);
                }
            }

            return Json(new { sStaff, sGrievane, sRole }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchGrievanceMemberById(string sGrievancememberid)
        {
            GRIEVANCEMEMBERSINFO GrievanceModel = new GRIEVANCEMEMBERSINFO();
            try
            {
                GrievanceModel.G_MEMBER_ID = sGrievancememberid;
                if (sGrievancememberid != null)
                {
                    var grievancememberlist = (List<GRIEVANCEMEMBERSINFO>)CMSHandler.FetchData<GRIEVANCEMEMBERSINFO>(GrievanceModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceMembersListById)).DataSource.Data;
                    GrievanceModel.G_MEMBER_ID = grievancememberlist.FirstOrDefault().G_MEMBER_ID;
                    GrievanceModel.GRIEVANCE_TYPE_ID = grievancememberlist.FirstOrDefault().GRIEVANCE_TYPE_ID;
                    GrievanceModel.FIRST_NAME = grievancememberlist.FirstOrDefault().FIRST_NAME;
                    GrievanceModel.STAFF_ID = grievancememberlist.FirstOrDefault().STAFF_ID;
                    GrievanceModel.ROLE = grievancememberlist.FirstOrDefault().ROLE;

                }
            }
            catch(Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GrievanceController", "FetchGrievanceMemberById", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GrievanceController", "FetchGrievanceMemberById", ex.Message);
                }
            }
            return Json(GrievanceModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteGrievanceMembers(string sGrievancememberid,string sUserid,string sRole)
        {
            GRIEVANCE_MEMEBERINFO GrievanceModel = new GRIEVANCE_MEMEBERINFO();
            USER_ACCOUNT_INFO UserAccountModel = new USER_ACCOUNT_INFO();
            try
            {
                string sSql = string.Empty;
                string UserAclist = string.Empty;
                if (sGrievancememberid != null)
                {
                    GrievanceModel.G_MEMBER_ID = sGrievancememberid;
                    UserAccountModel.USER_ID = sUserid;
                    UserAccountModel.ROLE = sRole;
                   
                    UserAclist = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchUserAcId).Replace(Common.Delimiter.QUS + Common.USER_ACCOUNT.ROLE, sRole);
                    var UserAccountlist = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(UserAccountModel, UserAclist).DataSource.Data;
                    if (UserAccountlist != null && UserAccountlist.Count>0)
                    {
                        UserAccountModel.USER_ACCOUNT_ID = UserAccountlist.FirstOrDefault().USER_ACCOUNT_ID;
                        sSql = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.DeleteUserAccountRole).Replace(Common.Delimiter.QUS + Common.USER_ACCOUNT.ROLE, sRole);

                        //var Result = CMSHandler.SaveOrUpdate(UserAccountModel, sSql);
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(GrievanceModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.DeleteGrievancemembers));
                        if (ResultArg.Success)
                            objResultData.Message = Common.Messages.RecordDeletedSuccessfully;
                        else
                            objResultData.Message = Common.Messages.FailedToDeletedTryAgain;
                    }
                    else
                        objResultData.Message = Common.Messages.FailedToDeletedTryAgain;
                }
                else
                    objResultData.Message = Common.Messages.FailedToDeletedTryAgain;

            }
            catch(Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GrievanceController", "DeleteGrievanceMembers", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GrievanceController", "DeleteGrievanceMembers", ex.Message);
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateGrievanceMembers(string memberid, string staffid, string members, string grievanceval,string role)
        {
            string sSql = string.Empty;
            string sUserInfo = string.Empty;
            string sUserAccInfo = string.Empty;
            USER_ACCOUNT_INFO UserModel = new USER_ACCOUNT_INFO();
            GRIEVANCE_MEMEBERINFO GrievanceModel = new GRIEVANCE_MEMEBERINFO();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {


                if (!string.IsNullOrEmpty(grievanceval))
                {
                    GrievanceModel.G_MEMBER_ID = memberid;
                    GrievanceModel.GRIEVANCE_TYPE_ID = grievanceval;
                    GrievanceModel.STAFF_ID = staffid;
                    GrievanceModel.ROLE = members;
                    UserModel.USER_ID = staffid;
                    UserModel.USER_TYPE = "4";
                    UserModel.ROLE_ID = members;
                    UserModel.ROLE = members;
                    string OldRole = "";

                    sUserAccInfo = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchUserAccountId).Replace(Common.Delimiter.QUS + Common.GRIEVANCEMEMBERSINFO.OLDROLE, OldRole);
                    var Useraccountinfo = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(UserModel, sUserAccInfo).DataSource.Data;
                    if (Useraccountinfo != null && Useraccountinfo.Count > 0)
                    {
                        sUserInfo = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchUserAccountForCheck).Replace(Common.Delimiter.QUS + Common.GRIEVANCEMEMBERSINFO.OLDROLE, OldRole);
                        var Useraccountinfocheck = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(UserModel, sUserInfo).DataSource.Data;

                        if (Useraccountinfocheck == null)
                        {
                            if (Useraccountinfo != null)
                            {
                                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(GrievanceModel, GrievanceSQL.GetGrievanceSQL((!string.IsNullOrEmpty(memberid)) ? GrievanceSQLCommands.UpdateGrievanceMembersList : GrievanceSQLCommands.SaveGrievancemembers));
                                //bool saveorupdate = (!string.IsNullOrEmpty(memberid) ? false : true);
                                //if (saveorupdate == true)
                                //{
                                    UserModel.USER_ACCOUNT_ID = Useraccountinfo.FirstOrDefault().USER_ACCOUNT_ID;
                                    sSql = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.UpdateUserAccountRole).Replace(Common.Delimiter.QUS + Common.GRIEVANCEMEMBERSINFO.OLDROLE, OldRole);
                                    var Result = CMSHandler.SaveOrUpdate(UserModel, sSql);
                                //}
                                if (ResultArg.Success)
                                {
                                    var Exist = (List<USER_ACCOUNT_INFO>)CMSHandler.FetchData<USER_ACCOUNT_INFO>(UserModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.CheckUserAccountByAcademicYearGrievanceRoleExist), sAcademicYear).DataSource.Data;
                                    if (Exist == null)
                                    {
                                        var result = CMSHandler.SaveOrUpdate(UserModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.InsertUserAccountByAcademicYearForGrievance), sAcademicYear);
                                    }
                                    objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                    objResultData.Message = Common.Messages.FailedToSaveRecords;
                            }
                            else
                            {
                                objResultData.Message = Common.Messages.AccountNotAvailable;
                            }
                        }
                        else
                        {
                            objResultData.Message = Common.Messages.LedgersAreMapped;
                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.UserAccountNotCreated;
                    }


                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Grievance", "SaveorUpdateGrievanceMembers", ex.ToString());
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This action is used to show List of Grievance
        /// </summary>
        /// <returns>GrievanceViewModel</returns>
        public ActionResult ListGrievance()
        {
            GrievanceViewModel objviewModel = new GrievanceViewModel();
            List<GRIEVANCEINFOANDREDRASSALINFO> formatlist = new List<GRIEVANCEINFOANDREDRASSALINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var GrievanceList = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceTypeList)).DataSource.Data;
                    objviewModel.GrievanceType = new SelectList(GrievanceList, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                    var list = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceandredrassal)).DataSource.Data;
                    foreach (var item in list)
                    {

                        byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                        string content = System.Text.Encoding.UTF8.GetString(bytes);
                        byte[] bytes1 = Convert.FromBase64String(item.REDRESSAL_CONTENT);
                        string redcontent = System.Text.Encoding.UTF8.GetString(bytes1);


                        formatlist.Add(new GRIEVANCEINFOANDREDRASSALINFO { GRIEVANCE_ID = item.GRIEVANCE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_CONTENT = content, SENDER_NAME = item.SENDER_NAME, STU_PHOTO = item.STU_PHOTO, REDRESSAL_CONTENT = redcontent, SUB_DATE_TIME = item.SUB_DATE_TIME, REPLY_DATE_TIME = item.REPLY_DATE_TIME, FIRST_NAME = item.FIRST_NAME, GRIEVANCE_SUBJECT = item.GRIEVANCE_SUBJECT, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME });
                    }
                    objviewModel.liGrievanceandredrassalinfo = formatlist;
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "ListGrievance", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "ListGrievance", ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objviewModel);
        }
        public JsonResult FetchAttachmentById(string sGrievanceid)
        {
            GRIEVANCEINFOANDREDRASSALINFO objModel = new GRIEVANCEINFOANDREDRASSALINFO();
            string sHtml = string.Empty;
            string SQL = string.Empty;
            try
            {
                objModel.GRIEVANCE_ID = sGrievanceid;

                SQL = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchAttachmentByGrievanceId).Replace(Common.Delimiter.QUS + Common.GRIEVANCEMEMBERSINFO.GRIEVANCE_ID, objModel.GRIEVANCE_ID);
                var Attachmentlist = (List<GRIEVANCE_ATTACHMENTINFO>)CMSHandler.FetchData<GRIEVANCE_ATTACHMENTINFO>(null, SQL).DataSource.Data;

               
                sHtml += "<div>";
                sHtml += "<ul style='list-style:none;padding:5px'>";
                if (Attachmentlist != null)
                {
                    int count = 1;
                    foreach (var item in Attachmentlist)
                    {
                        string[] path = item.ATTACHMENT_PATH.Split(',');
                        string[] filename = item.ATTACHMENT_NAME.Split(',');
                        var pathandfilename = path.Zip(filename, (p, f) => new { path = p, filename = f });
                        foreach (var item1 in pathandfilename)
                        {
                            sHtml += "<li><a class='filepath' style='text-decoration:none' href='#' path='" + item1.path + "'><h4 style='display:inline-block'>" + count++ + '.' + item1.filename + "</h4><i class='glyphicon glyphicon-file'></i>&nbsp;&nbsp;</a><a href='" + item1.path + "' target='- blank'><i class='glyphicon glyphicon-open open' title='Open'></i></a>&nbsp;&nbsp;<a href='" + item1.path + "' download><i class='glyphicon glyphicon-download download' title='Download'></i></a></li>";
                        }
                    }
                }
                else
                {
                    sHtml += "<li><h2>No Attachment</h2></li>";
                }
                sHtml += "</ul>";
                sHtml += "</div>";
            }
            catch(Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Grievance", "FetchAttachmentById", ex.ToString());
                }
            }                  
            return Json(sHtml, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Bindgrievancelist(string grievancetypeid)
        {
            GrievanceViewModel objviewModel = new GrievanceViewModel();
            List<GRIEVANCEINFOANDREDRASSALINFO> formatlist = new List<GRIEVANCEINFOANDREDRASSALINFO>();
            GRIEVANCEINFOANDREDRASSALINFO objModel = new GRIEVANCEINFOANDREDRASSALINFO();
                objModel.GRIEVANCE_TYPE_ID = grievancetypeid;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    if (grievancetypeid != null)
                    {

                        var list = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceandredrassalById)).DataSource.Data;
                        if (list != null)
                        {
                            foreach (var item in list)
                            {

                                byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                                string content = System.Text.Encoding.UTF8.GetString(bytes);
                                byte[] bytes1 = Convert.FromBase64String(item.REDRESSAL_CONTENT);
                                string redcontent = System.Text.Encoding.UTF8.GetString(bytes1);


                                formatlist.Add(new GRIEVANCEINFOANDREDRASSALINFO { GRIEVANCE_ID = item.GRIEVANCE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_CONTENT = content, SENDER_NAME = item.SENDER_NAME, STU_PHOTO = item.STU_PHOTO, REDRESSAL_CONTENT = redcontent, SUB_DATE_TIME = item.SUB_DATE_TIME, REPLY_DATE_TIME = item.REPLY_DATE_TIME, FIRST_NAME = item.FIRST_NAME, GRIEVANCE_SUBJECT = item.GRIEVANCE_SUBJECT, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME });
                            }
                            objviewModel.liGrievanceandredrassalinfo = formatlist;
                        }

                    }
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "Bindgrievancelist", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "Bindgrievancelist", ex.Message);
                    }
                }

              
            }
            else
                  return RedirectToAction("ErrorMessage", "Error");

                return View(objviewModel);
        }
        /// <summary>
        /// This actin is used to show Grievance report by date wise and grievance type wise
        /// </summary>
        /// <returns>GrievanceViewModel</returns>
        public ActionResult Report()
        {
            List<GRIEVANCEINFOANDREDRASSALINFO> formatlist = new List<GRIEVANCEINFOANDREDRASSALINFO>();
            GrievanceViewModel objviewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var GrievanceList = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceTypeList)).DataSource.Data;
                    objviewModel.GrievanceType = new SelectList(GrievanceList, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                    var list = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceandredrassal)).DataSource.Data;

                    foreach (var item in list)
                    {

                        byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                        string content = System.Text.Encoding.UTF8.GetString(bytes);
                        byte[] bytes1 = Convert.FromBase64String(item.REDRESSAL_CONTENT);
                        string redcontent = System.Text.Encoding.UTF8.GetString(bytes1);


                        formatlist.Add(new GRIEVANCEINFOANDREDRASSALINFO { GRIEVANCE_ID = item.GRIEVANCE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_CONTENT = content, SENDER_NAME = item.SENDER_NAME, STU_PHOTO = item.STU_PHOTO, REDRESSAL_CONTENT = redcontent, SUB_DATE_TIME = item.SUB_DATE_TIME, REPLY_DATE_TIME = item.REPLY_DATE_TIME, FIRST_NAME = item.FIRST_NAME, GRIEVANCE_SUBJECT = item.GRIEVANCE_SUBJECT, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME });
                    }
                    objviewModel.liGrievanceandredrassalinfo = formatlist;
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "Report", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "Report", ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
                return View(objviewModel);
        }
        public ActionResult BindgrievanceReport(string sGrievance)
        {
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            GRIEVANCEINFOANDREDRASSALINFO objModel = new GRIEVANCEINFOANDREDRASSALINFO();
            List<GRIEVANCEINFOANDREDRASSALINFO> formatlist = new List<GRIEVANCEINFOANDREDRASSALINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    if (sGrievance != null)
                    {
                        var Grievancedetails = JsonConvert.DeserializeObject<GRIEVANCEINFOANDREDRASSALINFO>(sGrievance);
                        string SQL = string.Empty;
                        objModel.GRIEVANCE_TYPE_ID = Grievancedetails.GRIEVANCE_TYPE_ID;
                        objModel.STATUS = Grievancedetails.STATUS;
                        SQL = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceandredrassalByDate).Replace(Common.Delimiter.QUS + Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, objModel.GRIEVANCE_TYPE_ID);
                        SQL = SQL.Replace(Common.Delimiter.QUS + "STATUS", objModel.STATUS);
                        objModel.FROM_DATE = CommonMethods.DateConvertionForPayU(Grievancedetails.FROM_DATE);
                        objModel.TO_DATE = CommonMethods.DateConvertionForPayU(Grievancedetails.TO_DATE);
                        if (Grievancedetails != null)
                        {
                            var list = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(objModel, SQL).DataSource.Data;
                            if (list != null)
                            {
                                foreach (var item in list)
                                {

                                    byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                                    string content = System.Text.Encoding.UTF8.GetString(bytes);
                                    byte[] bytes1 = Convert.FromBase64String(item.REDRESSAL_CONTENT);
                                    string redcontent = System.Text.Encoding.UTF8.GetString(bytes1);


                                    formatlist.Add(new GRIEVANCEINFOANDREDRASSALINFO { GRIEVANCE_ID = item.GRIEVANCE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_CONTENT = content, SENDER_NAME = item.SENDER_NAME, STU_PHOTO = item.STU_PHOTO, REDRESSAL_CONTENT = redcontent, SUB_DATE_TIME = item.SUB_DATE_TIME, REPLY_DATE_TIME = item.REPLY_DATE_TIME, FIRST_NAME = item.FIRST_NAME, GRIEVANCE_SUBJECT = item.GRIEVANCE_SUBJECT, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME });
                                }
                                objViewModel.liGrievanceandredrassalinfo = formatlist;
                            }
                            else
                            {
                                return View(objViewModel);
                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "BindgrievanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "BindgrievanceReport", ex.Message);
                    }
                }
            
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public ActionResult BindGrievanceBycategory(string sCategroy)
        {
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            GRIEVANCEINFOANDREDRASSALINFO objModel = new GRIEVANCEINFOANDREDRASSALINFO();
            List<GRIEVANCEINFOANDREDRASSALINFO> formatlist = new List<GRIEVANCEINFOANDREDRASSALINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    objModel.STATUS = sCategroy;
                    if (!string.IsNullOrEmpty(sCategroy))
                    {
                        
                            var list = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceBycategory)).DataSource.Data;
                            if (list != null && list.Count > 0)
                            {
                                foreach (var item in list)
                                {

                                    byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                                    string content = System.Text.Encoding.UTF8.GetString(bytes);
                                    byte[] bytes1 = Convert.FromBase64String(item.REDRESSAL_CONTENT);
                                    string redcontent = System.Text.Encoding.UTF8.GetString(bytes1);


                                    formatlist.Add(new GRIEVANCEINFOANDREDRASSALINFO { GRIEVANCE_ID = item.GRIEVANCE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_CONTENT = content, SENDER_NAME = item.SENDER_NAME, STU_PHOTO = item.STU_PHOTO, REDRESSAL_CONTENT = redcontent, SUB_DATE_TIME = item.SUB_DATE_TIME, REPLY_DATE_TIME = item.REPLY_DATE_TIME, FIRST_NAME = item.FIRST_NAME, GRIEVANCE_SUBJECT = item.GRIEVANCE_SUBJECT, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME });
                                }
                            }
                       
                        
                        
                        objViewModel.liGrievanceandredrassalinfo = formatlist;
                        return View(objViewModel);
                    }
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "BindGrievanceBycategory", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "BindGrievanceBycategory", ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");

            return RedirectToAction("Report", "Grievance");
        }
        /// <summary>
        /// This action is used to show statistical report of grievance
        /// </summary>
        /// <returns>GrievanceViewModel</returns>
        public ActionResult Statisticalreport()
        {
            int total = 0;
           
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            List<GRIEVANCEINFOANDREDRASSALINFO> liGrievanceinfo = new List<GRIEVANCEINFOANDREDRASSALINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var l = (List<GRIEVANCEINFOANDREDRASSALINFO>)CMSHandler.FetchData<GRIEVANCEINFOANDREDRASSALINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievancecount)).DataSource.Data;
                    if (l != null)
                    {
                        foreach (var item in l)
                        {
                            total += Convert.ToInt32(item.GRIEVANCE_COUNT);
                        }
                        foreach (var item1 in l)
                        {
                            int count = Convert.ToInt32(item1.GRIEVANCE_COUNT);

                            double percent;
                            double round;
                            string finalpercent;
                            if (count != 0)
                            { 
                                percent = (Math.Abs(100) * Math.Abs(count));
                                round= percent/ Math.Abs(total);
                                finalpercent = string.Format("{0:F2}", round);
                            }
                            else
                                finalpercent = "100";
                            liGrievanceinfo.Add(new GRIEVANCEINFOANDREDRASSALINFO { GRIEVANCE_TITLE = item1.GRIEVANCE_TITLE, GRIEVANCE_COUNT = finalpercent.ToString() });
                        }
                    }
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "Statisticalreport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "Statisticalreport", ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");          
            return View(liGrievanceinfo);
        }
        /// <summary>
        /// This action is used to assgin Grievance to members
        /// </summary>
        /// <returns>GrievanceViewModel</returns>
        public ActionResult AssignGrievancetoMembers()
        {
            List<GRIEVANCE_GRIEVANCEINFO> formatlist = new List<GRIEVANCE_GRIEVANCEINFO>();
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var GrievanceList = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceTypeList)).DataSource.Data;
                    objViewModel.GrievanceType = new SelectList(GrievanceList, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_ID, Common.GRIEVANCE_TYPE.GRIEVANCE_TYPE_NAME);
                    var StaffList = (List<GRIEVANCEMEMBERSINFO>)CMSHandler.FetchData<GRIEVANCEMEMBERSINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceMembers)).DataSource.Data;
                    objViewModel.StaffList = new SelectList(StaffList, Common.GRIEVANCEMEMBERSINFO.STAFF_ID, Common.GRIEVANCEMEMBERSINFO.FIRST_NAME);
                    var list = (List<GRIEVANCE_GRIEVANCEINFO>)CMSHandler.FetchData<GRIEVANCE_GRIEVANCEINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievance)).DataSource.Data;
                    foreach (var item in list)
                    {
                        byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                        string content = System.Text.Encoding.UTF8.GetString(bytes);
                        formatlist.Add(new GRIEVANCE_GRIEVANCEINFO { GRIEVANCE_ID = item.GRIEVANCE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME, GRIEVANCE_CONTENT = content, ROLL_NO = item.ROLL_NO, SENDER_NAME = item.SENDER_NAME, SUB_DATE_TIME = item.SUB_DATE_TIME, ASSIGN_TO = item.ASSIGN_TO });
                    }
                    objViewModel.liGrievanceinfolist = formatlist;

                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "AssignGrievancetoMembers", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "AssignGrievancetoMembers", ex.Message);
                    }
                }
                }
            
            else
                return RedirectToAction("ErrorMessage", "Error");
           return View(objViewModel);
        }
        public JsonResult FetchStaffBygrievancetype(string grievanceid)
        {
            string sOPtion = string.Empty;
            if (grievanceid != null)
            {
                GRIEVANCEMEMBERSINFO objModel = new GRIEVANCEMEMBERSINFO();
                objModel.GRIEVANCE_TYPE_ID = grievanceid;
                var StaffList = (List<GRIEVANCEMEMBERSINFO>)CMSHandler.FetchData<GRIEVANCEMEMBERSINFO>(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceMembersById)).DataSource.Data;
                if (StaffList != null)
                {
                    foreach (var item in StaffList)
                    {
                        sOPtion += "<option value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                }
            }
            return Json(sOPtion,JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindgrievanceBytype(string grievanceid)
        {
            GRIEVANCE_GRIEVANCEINFO objModel = new GRIEVANCE_GRIEVANCEINFO();
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            List<GRIEVANCE_GRIEVANCEINFO> formatlist = new List<GRIEVANCE_GRIEVANCEINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    objModel.GRIEVANCE_TYPE_ID = grievanceid;
                   
                    var liGrievanceinfolist = (List<GRIEVANCE_GRIEVANCEINFO>)CMSHandler.FetchData<GRIEVANCE_GRIEVANCEINFO>(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceBytype)).DataSource.Data;
                    foreach (var item in liGrievanceinfolist)
                    {
                        byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                        string content = System.Text.Encoding.UTF8.GetString(bytes);
                        formatlist.Add(new GRIEVANCE_GRIEVANCEINFO { GRIEVANCE_ID = item.GRIEVANCE_ID,  GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME, GRIEVANCE_CONTENT = content, SUB_DATE_TIME = item.SUB_DATE_TIME,GRIEVANCE_SUBJECT=item.GRIEVANCE_SUBJECT });
                    }
                    objViewModel.liGrievanceinfolist = formatlist;
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "BindgrievanceBytype", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "BindgrievanceBytype", ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public JsonResult AssignGrievance(string sGrievanceassign,string staffid)
        {
           
            if (sGrievanceassign!=null)
            {
                GRIEVANCE_GRIEVANCEINFO objModel = new GRIEVANCE_GRIEVANCEINFO();
                var Grievancedetails = JsonConvert.DeserializeObject<GRIEVANCE_GRIEVANCEINFO>(sGrievanceassign);
                objModel.ASSIGN_TO = staffid;                            
                string grievanceid = Grievancedetails.GRIEVANCE_ID;
                string SQL = string.Empty;
                SQL = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.SaveGrievanceAssign).Replace(Common.Delimiter.QUS + Common.GRIEVANCEMEMBERSINFO.GRIEVANCE_ID, grievanceid);
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, SQL);
                if (ResultArg.Success)
                {                   
                    objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                }
                else
                    objResultData.Message = Common.Messages.FailedToSaveRecords;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }      
        /// <summary>
        /// This Action is used to show Grievanceassigned List
        /// </summary>
        /// <returns></returns>
        public ActionResult AssignedGrievance()
        {
            GrievanceViewModel objViewModel = new GrievanceViewModel();
            List<GRIEVANCE_GRIEVANCEINFO> formatlist = new List<GRIEVANCE_GRIEVANCEINFO>();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var list = (List<GRIEVANCE_GRIEVANCEINFO>)CMSHandler.FetchData<GRIEVANCE_GRIEVANCEINFO>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchAssignedGrievance)).DataSource.Data;
                    foreach (var item in list)
                    {
                        byte[] bytes = Convert.FromBase64String(item.GRIEVANCE_CONTENT);
                        string content = System.Text.Encoding.UTF8.GetString(bytes);
                        formatlist.Add(new GRIEVANCE_GRIEVANCEINFO { GRIEVANCE_ID = item.GRIEVANCE_ID,GRIEVANCE_TYPE_ID=item.GRIEVANCE_TYPE_ID, GRIEVANCE_TITLE = item.GRIEVANCE_TITLE, GRIEVANCE_TYPE_NAME = item.GRIEVANCE_TYPE_NAME, GRIEVANCE_CONTENT = content, ROLL_NO = item.ROLL_NO, SENDER_NAME = item.SENDER_NAME, SUB_DATE_TIME = item.SUB_DATE_TIME, ASSIGN_TO = item.ASSIGN_TO, GRIEVANCE_SUBJECT = item.GRIEVANCE_SUBJECT });
                    }
                    objViewModel.liGrievanceinfolist = formatlist;
                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "AssignedGrievance", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "AssignedGrievance", ex.Message);
                    }
                }
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
           return View(objViewModel);
        }
        public JsonResult FetchAssignedGrievanceById(string sGrievanceid,string sGrievancetype)
        {
            
            GRIEVANCE_GRIEVANCEINFO objModel = new GRIEVANCE_GRIEVANCEINFO();
            objModel.GRIEVANCE_ID = sGrievanceid;
             var Grievancedetails= (List<GRIEVANCE_GRIEVANCEINFO>)CMSHandler.FetchData<GRIEVANCE_GRIEVANCEINFO>(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchAssignedGrievanceById)).DataSource.Data;
            objModel.GRIEVANCE_TYPE_NAME = Grievancedetails.FirstOrDefault().GRIEVANCE_TYPE_NAME;
            objModel.ASSIGN_TO = Grievancedetails.FirstOrDefault().ASSIGN_TO;
            objModel.GRIEVANCE_ID = Grievancedetails.FirstOrDefault().GRIEVANCE_ID;
            string sOPtion = string.Empty;
            if (sGrievanceid != null)
            {
                GRIEVANCEMEMBERSINFO objModel1 = new GRIEVANCEMEMBERSINFO();
                objModel1.GRIEVANCE_TYPE_ID = sGrievancetype;
                var StaffList = (List<GRIEVANCEMEMBERSINFO>)CMSHandler.FetchData<GRIEVANCEMEMBERSINFO>(objModel1, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceMembersById)).DataSource.Data;
                if (StaffList != null)
                {
                    foreach (var item in StaffList)
                    {
                        sOPtion += "<option value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
                    }
                }
            }
            return Json(new {objModel,sOPtion },JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAssignedGrievance(string sGrievanceid)
        {
            GRIEVANCE_GRIEVANCEINFO objModel = new GRIEVANCE_GRIEVANCEINFO();
            if(sGrievanceid!=null)
            {
                objModel.GRIEVANCE_ID = sGrievanceid;
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.DeleteAssignedGrievance));
                if (ResultArg.Success)
                    objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                else
                    objResultData.Message = Common.Messages.FailedToDeletedTryAgain;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteGrievance(string sGrievanceid)
        {

            GRIEVANCE_GRIEVANCEINFO objModel = new GRIEVANCE_GRIEVANCEINFO();
            if (sGrievanceid != null)
            {
                objModel.GRIEVANCE_ID = sGrievanceid;
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.DeleteGrievance));
                if (ResultArg.Success)
                    objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                else
                    objResultData.Message = Common.Messages.FailedToDeletedTryAgain;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GrievanceBaseStaff(string sGrievancetype)
        {
            GrievanceViewModel objVieModel = new GrievanceViewModel();
            string sOPtion = string.Empty;
            if (sGrievancetype!=null)
            {
                try
                {
                             
                    string sSql = GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceBasedStaff).Replace(Common.Delimiter.QUS + Common.GRIEVANCE_MEMBERINFO.GRIEVANCE_TYPE_ID, sGrievancetype);
                    var Grievancebasedstaff = (List<GRIEVANCEMEMBERSINFO>)CMSHandler.FetchData<GRIEVANCEMEMBERSINFO>(null, sSql).DataSource.Data;
                    var StaffList = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchStaffList)).DataSource.Data;
                    if (StaffList != null)
                    {
                        foreach (var totalstaff in StaffList)
                        {
                            foreach (var matchstaff in Grievancebasedstaff)
                            {
                                
                                sOPtion += "<option value='" + totalstaff.STAFF_ID + "' sSelected='selected'>" + totalstaff.FIRST_NAME + "</option>";
                            }
                            sOPtion += "<option value='" + totalstaff.STAFF_ID + "'>" + totalstaff.FIRST_NAME + "</option>";

                        }
                    }

                }
                catch(Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("GrievanceController", "GrievanceBaseStaff", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("GrievanceController", "GrievanceBaseStaff", ex.Message);
                    }
                }
            }
            return Json(sOPtion, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GrievanceType
        public ActionResult SupGrievanceType()
        {
            GrievanceViewModel objviewModel = new GrievanceViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                objviewModel.liGrievanceType= (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(null, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceTypeList)).DataSource.Data;

            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objviewModel);
        }
        public JsonResult GrievanceTypeById(string sGrievancetypeid)
        {
            GrievanceViewModel objviewModel = new GrievanceViewModel();
            GRIEVANCE_TYPE objModel = new GRIEVANCE_TYPE();
            if(sGrievancetypeid!=null)
            {
                objModel.GRIEVANCE_TYPE_ID = sGrievancetypeid;
                var Grievancetypelist = (List<GRIEVANCE_TYPE>)CMSHandler.FetchData<GRIEVANCE_TYPE>(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.FetchGrievanceTypeListById)).DataSource.Data;
                objModel.GRIEVANCE_TYPE_ID = Grievancetypelist.FirstOrDefault().GRIEVANCE_TYPE_ID;
                objModel.GRIEVANCE_TYPE_NAME = Grievancetypelist.FirstOrDefault().GRIEVANCE_TYPE_NAME;
                objModel.GRIEVANCE_TYPE_CODE = Grievancetypelist.FirstOrDefault().GRIEVANCE_TYPE_CODE;
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteGrievanceType(string sGrievancetypeid)
        {
            GRIEVANCE_TYPE objModel = new GRIEVANCE_TYPE();
            if (sGrievancetypeid != null)
            {
                objModel.GRIEVANCE_TYPE_ID = sGrievancetypeid;
                ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, GrievanceSQL.GetGrievanceSQL(GrievanceSQLCommands.DeleteGrievanceType));
                if (ResultArg.Success)
                    objResultData.Message = Common.Messages.RecordDeletedSuccessfully;
                else
                    objResultData.Message = Common.Messages.FailedToDeletedTryAgain;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public string GenerateGrievanceCode(string prefix)
        {

            string sgrievanceCode = string.Empty;
            sgrievanceCode = prefix;
            StringBuilder builder = new StringBuilder();
            byte[] hash;
            using (HMACSHA1 sha1 = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(prefix)))
            {
                hash = sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes("grievance"));
                Random random = new Random();
                for(int i=0;i<3;i++)
                {
                    builder.Append(random.Next(i,hash.Length));
                }
              
            }
            return builder.ToString(); 
        }
        public JsonResult SaveorUpdateGrievanceType(string sGrievancetype)
        {
            
            if(!string.IsNullOrEmpty(sGrievancetype))
            {
                var GrievanceTypedetails = JsonConvert.DeserializeObject<GRIEVANCE_TYPE>(sGrievancetype);
                if(GrievanceTypedetails != null)
                {
                    GrievanceTypedetails.PREFIX_CODE = GrievanceTypedetails.GRIEVANCE_TYPE_NAME.Substring(0, 2).ToUpper();
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(GrievanceTypedetails, GrievanceSQL.GetGrievanceSQL((!string.IsNullOrEmpty(GrievanceTypedetails.GRIEVANCE_TYPE_ID))?GrievanceSQLCommands.UpdateGrievanceType:GrievanceSQLCommands.SaveGrievanceType));
                    if (ResultArg.Success)
                        objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}