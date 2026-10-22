using CMS.DAO;
using CMS.SQL.Alumnae;
using CMS.SQL.IQAC;
using CMS.SQL.LMS;
using CMS.SQL.StaffSQL;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.SQL.Website;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class WebsiteController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData ObjJsonData = new JsonResultData();
        string sResult = string.Empty;
        // GET: Website
        public ActionResult Index()
        {
            return View();
        }
        #region   Home Events 
        public ActionResult EventList()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                int nAcademicYear = Convert.ToInt32(sAcademicYear);
                nAcademicYear--;
                sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                var ObjModel = new WEB_HOME_EVENTS();
                ObjModel.ENTRY_ID = sStaffId;
                var lihomeEvents = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchWebHomeEvents), sAcademicYear).DataSource.Data;

                if (lihomeEvents != null && lihomeEvents.Count > 0)
                {
                    objViewModel.liWebsiteEvent = lihomeEvents;
                }
                // fetch project type
                var Department = (List<cp_Department_2017>)CMS.DAO.CMSHandler.FetchData<cp_Department_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(DAO.SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                if (Department != null && Department.Count > 0)
                {
                    objViewModel.sDepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                }

            }
            return View(objViewModel);
        }

        public JsonResult FetchEventById(string sIQACID)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objResultData.oResult = (List<IQAC_PROJECT>)CMS.DAO.CMSHandler.FetchData<IQAC_PROJECT>(new IQAC_PROJECT() { IQAC_PROJECT_ID = sIQACID }, SQL.IQAC.IQACSQL.GetIQACSQL(DAO.IQACEnumCommands.FetchIQACProjectsById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "FetchProjectById", ex.Message);
                    }
                    objResultData.Message = Common.ErrorMessage.InternalServerError;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveHomeEvents()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<IQAC_PROJECT>(sJsonData);
                    if (result != null)
                    {
                        result.STAFF_ID = sStaffId;
                        if (!string.IsNullOrEmpty(result.IQAC_PROJECT_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACProjectsById), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.SaveIQACProjects), sAcademicYear, true);
                            result.IQAC_PROJECT_ID = resultArgs.RowUniqueId.ToString();
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }


                        string simgPath = Request.Form["sPhoto"];
                        if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(simgPath))
                        {
                            string sFilePath = string.Empty;

                            //update file 
                            if (!string.IsNullOrEmpty(simgPath))
                            {
                                //for upload img path
                                var imgPath = simgPath.Split(',');
                                foreach (var item2 in imgPath)
                                {
                                    sFilePath += DecryptString(item2) + ",";
                                }
                            }
                            if (!string.IsNullOrEmpty(result.FILE_PATH))
                            {
                                //for add img 
                                int Count = 0;
                                var fmgPath = result.FILE_PATH.Split(',');
                                foreach (var item2 in fmgPath)
                                {
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId);
                                    sFilePath += sPath + ","; Count++;
                                }
                            }
                            if (!string.IsNullOrEmpty(sFilePath))
                            {
                                result.FILE_PATH = sFilePath.TrimEnd(',');
                            }
                            var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACProjectsFilePathById), sAcademicYear);
                            //    if (!string.IsNullOrEmpty(result.IQAC_PROJECT_ID) && Request.Files.Count > 0)
                            //    {
                            //    int nCount = 0;
                            //    string sFilePath = string.Empty;
                            //    foreach (var item2 in Request.Files)
                            //    {
                            //        string sPath = SaveFile(nCount, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId, Common.URLPages.IQAC_IMAGE_PATH + "Project" + "\\" + sStaffId);
                            //        sFilePath += sPath + ","; nCount++;
                            //    }
                            //    result.FILE_PATH = sFilePath.TrimEnd(',');
                            //    var UpdateSaveFile = CMSHandler.SaveOrUpdate(result, SQL.IQAC.IQACSQL.GetIQACSQL(IQACEnumCommands.UpdateIQACProjectsFilePathById), sAcademicYear);
                            //}
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "SaveProject", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEvents(string sIQACID)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new IQAC_PROJECT();
                    ObjModel.IQAC_PROJECT_ID = sIQACID;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, IQACSQL.GetIQACSQL(IQACEnumCommands.DeletedIQACProjectsById));
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "DeleteProject", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region home Achievement Entry Page 
        public ActionResult AchievementsList()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();
            var objResultData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {


                var ObjModel = new WEB_HOME_EVENTS();
                ObjModel.ENTRY_ID = sStaffId;
                var lihomeEvents = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchWebHomeAchievement), sAcademicYear).DataSource.Data;

                if (lihomeEvents != null && lihomeEvents.Count > 0)
                {
                    objViewModel.liWebsiteEvent = lihomeEvents;
                }
                // fetch project type
                var Department = (List<cp_Department_2017>)CMS.DAO.CMSHandler.FetchData<cp_Department_2017>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchsupDepartmentList), sAcademicYear).DataSource.Data;
                if (Department != null && Department.Count > 0)
                {
                    objViewModel.sDepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
                }
                var Achievement = (List<WEB_SUP_ACHIEVEMENT>)CMS.DAO.CMSHandler.FetchData<WEB_SUP_ACHIEVEMENT>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchsupAchievementList), sAcademicYear).DataSource.Data;
                if (Achievement != null && Achievement.Count > 0)
                {
                    objViewModel.sAchievement = new SelectList(Achievement, Common.SUP_ACHIEVEMENT.ACHIEVEMENT_ID, Common.SUP_ACHIEVEMENT.ACHIEVEMENT_NAME);
                }
                var sdglist = (List<WEB_SDGLIST>)CMS.DAO.CMSHandler.FetchData<WEB_SDGLIST>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchSDGList), sAcademicYear).DataSource.Data;
                if (sdglist != null && sdglist.Count > 0)
                {
                    objViewModel.sSdgList = new SelectList(sdglist, Common.SUP_SDGLIST.SDG_ID, Common.SUP_SDGLIST.SDG_NAME_LIST);
                }
                var staffList = (List<stf_Personal_Info>)CMS.DAO.CMSHandler.FetchData<stf_Personal_Info>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchStaffList), sAcademicYear).DataSource.Data;
                if (staffList != null && staffList.Count > 0)
                {
                    objViewModel.sStaffList = new SelectList(staffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                }

            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        //public PartialViewResult _AchievementsList()
        // {
        //     string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //     string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
        //     var objViewModel = new WebsiteViewModel();

        //     if (!string.IsNullOrEmpty(sAcademicYear))
        //     {
        //         int nAcademicYear = Convert.ToInt32(sAcademicYear);
        //         nAcademicYear--;
        //         sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

        //         var ObjModel = new WEB_HOME_EVENTS();
        //         ObjModel.ENTRY_ID = sStaffId;
        //         var lihomeEvents = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchWebHomeAchievement), sAcademicYear).DataSource.Data;

        //         if (lihomeEvents != null && lihomeEvents.Count > 0)
        //         {
        //             objViewModel.liWebsiteEvent = lihomeEvents;
        //         }
        //         // fetch project type
        //         var Department = (List<cp_Department_2017>)CMS.DAO.CMSHandler.FetchData<cp_Department_2017>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchsupDepartmentList), sAcademicYear).DataSource.Data;
        //         if (Department != null && Department.Count > 0)
        //         {
        //             objViewModel.sDepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
        //         }
        //         var Achievement = (List<WEB_SUP_ACHIEVEMENT>)CMS.DAO.CMSHandler.FetchData<WEB_SUP_ACHIEVEMENT>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchsupAchievementList), sAcademicYear).DataSource.Data;
        //         if (Achievement != null && Achievement.Count > 0)
        //         {
        //             objViewModel.sAchievement = new SelectList(Achievement, Common.SUP_ACHIEVEMENT.ACHIEVEMENT_ID, Common.SUP_ACHIEVEMENT.ACHIEVEMENT_NAME);
        //         }
        //         var sdglist = (List<WEB_SDGLIST>)CMS.DAO.CMSHandler.FetchData<WEB_SDGLIST>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchSDGList), sAcademicYear).DataSource.Data;
        //         if (sdglist != null && sdglist.Count > 0)
        //         {
        //             objViewModel.sSdgList = new SelectList(sdglist, Common.SUP_SDGLIST.SDG_ID, Common.SUP_SDGLIST.SDG_NAME_LIST);
        //         }

        //     }
        //     return PartialView(objViewModel);
        // }

        public JsonResult FetchAchievementsById(string sAchievement)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                var liAchievement = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(new WEB_HOME_EVENTS() { ACHIEVEMENT_ID = sAchievement }, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchHomeAchievementbyId), sAcademicYear).DataSource.Data;
                if (liAchievement != null && liAchievement.Count > 0)
                {

                    var HtmlContent = HttpUtility.UrlEncode(liAchievement.FirstOrDefault().DESCRIPTION);
                    return Json(liAchievement, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebsiteController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveHomeAchievements()
        //{
        //    ResultArgs resultArgs = new ResultArgs();
        //    var objResultData = new JsonResultData();
        //    string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
        //    string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
        //    string filePath = string.Empty;
        //    string sImagePath = string.Empty;
        //    string _sDirectorypath = string.Empty;
        //    try
        //    {
        //        var sJson = Request.Form["jsonData"];
        //        //string sJsonData = Request.Form["sJsonData"];
        //        var result = JsonConvert.DeserializeObject<WEB_HOME_EVENTS>(sJson);
        //        var HtmlContent = HttpUtility.UrlDecode(result.DESCRIPTION);

        //        if (result != null)
        //        {
        //            result.DATE = CommonMethods.DateConvertionForPayU(result.DATE);
        //            result.ACADEMIC_YEAR = sAcademicYear;
        //            result.SEMESTER_TYPE = sSemesterType;
        //            result.ENTRY_ID = sStaffId;
        //            result.DESCRIPTION = HtmlContent;

        //            if (!string.IsNullOrEmpty(result.ACHIEVEMENT_ID))
        //            {
        //                resultArgs = CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateWebHomeAchievementWithoutImage), sAcademicYear);
        //                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
        //                objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
        //            }
        //            else
        //            {
        //                resultArgs = CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.SaveWebAchievement), sAcademicYear, true);
        //                result.ACHIEVEMENT_ID = resultArgs.RowUniqueId.ToString();
        //                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
        //                objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
        //            }

        //            string sfilePath = Request.Form["sfiles"];
        //            if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(sfilePath))
        //            {
        //                string sFilePath = string.Empty;

        //                //update file 
        //                if (!string.IsNullOrEmpty(sfilePath))
        //                {
        //                    //for upload img path
        //                    var imgPath = sfilePath.Split(',');
        //                    foreach (var item2 in imgPath)
        //                    {
        //                        sFilePath += DecryptString(item2) + ",";
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(result.FILE_PATH))
        //                {
        //                    var fileCount = Convert.ToInt64(result.FILE_PATH);
        //                    //for add img 
        //                    int Count = Convert.ToInt32(fileCount);
        //                    var fmgPath = result.FILE_PATH.Split(',');
        //                    foreach (var item2 in fmgPath)
        //                    {
        //                        string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.ACHIEVEMENT_FILE_PATH + "\\" + sStaffId, Common.URLPages.ACHIEVEMENT_FILE_PATH + "\\" + sStaffId);
        //                        sFilePath += sPath + ","; Count++;
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(sFilePath))
        //                {
        //                    result.FILE_PATH = sFilePath.TrimEnd(',');
        //                }
        //                var FileUpdateQuery = CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateAchievementFilePathByID));
        //            }
        //            string simgPath = Request.Form["photo"];
        //            if (!string.IsNullOrEmpty(result.IMG_PATH) || !string.IsNullOrEmpty(simgPath))
        //            {
        //                string sImgPath = string.Empty;
        //                //update file 
        //                if (!string.IsNullOrEmpty(simgPath))
        //                {
        //                    //for upload img path
        //                    var imgPath = simgPath.Split(',');
        //                    foreach (var item2 in imgPath)
        //                    {
        //                        sImgPath += DecryptString(item2) + ",";
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(result.IMG_PATH))
        //                {
        //                    //for add img 
        //                    int Count = 0;
        //                    var fmgPath = result.IMG_PATH.Split(',');
        //                    foreach (var item2 in fmgPath)
        //                    {
        //                        string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.WEB_ACHIEVEMENT + "\\" + sStaffId, Common.URLPages.WEB_ACHIEVEMENT + "\\" + sStaffId);
        //                        sImgPath += sPath + ","; Count++;
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(sImgPath))
        //                {
        //                    //sImgPath = sImgPath.Replace(@"\\", @"\");

        //                    sImgPath = Regex.Replace(sImgPath, @"\\{2,}", @"\");
        //                    result.IMG_PATH = sImgPath.TrimEnd(',');
        //                }
        //                var FileUpdateQuery = CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateAchievementImgPathbyID));
        //            }

        //        }
        //    }


        //    catch (Exception ex)
        //    {
        //        using (ErrorLog ObjLog = new ErrorLog())
        //        {
        //            ObjLog.WriteError("WebsiteController", "SaveHomeAchievements", ex.Message);
        //        }

        //        objResultData.Message = ex.Message;
        //        objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //    }


        //    if (string.IsNullOrEmpty(objResultData.Message))
        //    {
        //        objResultData.Message = "No Changes Made";
        //        objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
        //    }
        //    return Json(objResultData, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveHomeAchievements()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                var sJson = Request.Form["jsonData"];
                var result = JsonConvert.DeserializeObject<WEB_HOME_EVENTS>(sJson);
                var HtmlContent = HttpUtility.UrlDecode(result.DESCRIPTION);

                if (result != null)
                {
                    // ... (Logic to save/update the main record)
                    result.DATE = CommonMethods.DateConvertionForPayU(result.DATE);
                    result.ACADEMIC_YEAR = sAcademicYear;
                    result.ENTRY_ID = sStaffId;
                    result.DESCRIPTION = HtmlContent;

                    if (!string.IsNullOrEmpty(result.ACHIEVEMENT_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateWebHomeAchievementWithoutImage), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.SaveWebAchievement), sAcademicYear, true);
                        result.ACHIEVEMENT_ID = resultArgs.RowUniqueId.ToString();
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }

                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    // --- CORRECT FILE HANDLING LOGIC ---

                    // 1. Get existing file paths from the form (for updates)
                    var existingImagePaths = Request.Form["photo"];
                    var existingFilePaths = Request.Form["sfiles"];

                    string finalImagePaths = string.Empty;
                    string finalFilePaths = string.Empty;

                    if (!string.IsNullOrEmpty(existingImagePaths))
                    {
                        finalImagePaths = string.Join(",", existingImagePaths.Split(',').Select(p => DecryptString(p))) + ",";
                    }
                    if (!string.IsNullOrEmpty(existingFilePaths))
                    {
                        finalFilePaths = string.Join(",", existingFilePaths.Split(',').Select(p => DecryptString(p))) + ",";
                    }

                    // 2. Process all newly uploaded files in a single loop
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase currentFile = Request.Files[i];
                        string extension = Path.GetExtension(currentFile.FileName).ToLower();

                        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                        if (imageExtensions.Contains(extension))
                        {
                            
                            string directory = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.WEB_ACHIEVEMENT + "\\" + sStaffId;
                            string saveDirectory = Path.Combine(Common.URLPages.WEB_ACHIEVEMENT, sStaffId);
                            finalImagePaths += SaveFile(currentFile, directory, saveDirectory) + ",";
                        }
                        else
                        {
                            // It's a document
                            string directory = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.ACHIEVEMENT_FILE_PATH + "\\" + sStaffId;

                            string saveDirectory = Path.Combine(Common.URLPages.ACHIEVEMENT_FILE_PATH, sStaffId);
                            finalFilePaths += SaveFile(currentFile, directory, saveDirectory) + ",";
                        }
                    }

                    // 3. Update the database with the final, combined file paths
                    if (!string.IsNullOrEmpty(finalImagePaths))
                    {
                        result.IMG_PATH = finalImagePaths.TrimEnd(',').Replace("\\","/");
                        result.IMG_PATH = result.IMG_PATH.Replace("//","/");
                        CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateAchievementImgPathbyID));
                    }
                    if (!string.IsNullOrEmpty(finalFilePaths))
                    {
                        result.FILE_PATH = finalFilePaths.TrimEnd(',');
                        CMSHandler.SaveOrUpdate(result, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateAchievementFilePathByID));
                    }
                }
            }
            catch (Exception ex)
            {
                // ... (Error handling)
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        // Ensure this helper method is also present and correct
        public string SaveFile(HttpPostedFileBase file, string sDirectory, string sSaveDirectory)
        {
            string sFilePath = string.Empty, sSavePath = string.Empty;

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    if (!Directory.Exists(sDirectory))
                    {
                        Directory.CreateDirectory(sDirectory);
                    }

                    string sFileName = Path.GetFileName(file.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(sFileName);

                    sFilePath = Path.Combine(sDirectory, uniqueFileName);
                    //sSavePath = Path.Combine(sSaveDirectory, uniqueFileName).Replace("\\", "/");
                    sSavePath = Path.Combine(sSaveDirectory, uniqueFileName).Replace("\\", "/");

                    file.SaveAs(sFilePath);
                }
                catch (Exception ex)
                {
                    // ... (Error logging)
                    return string.Empty;
                }
            }
            return sSavePath;
        }
        public JsonResult DeleteAchievements(string sAchieve)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new WEB_HOME_EVENTS();
                    ObjModel.ACHIEVEMENT_ID = sAchieve;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.DeleteWebAchievement));
                    if (DeleteQuery.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "DeleteProject", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Common method image save
        

        public string SaveFile(int Count, string sDirectory, string sSaveDirectory)
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty;
            var path = string.Empty;

            if (!string.IsNullOrEmpty(sDirectory))
            {
                try
                { 
                    sDirectorypath = sDirectory;
                    // AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ALUMNAE_PASSEDOUT_IMAGE_PATH + "\\" + sFolder;
                    if (!Directory.Exists(sDirectorypath))
                    {
                        Directory.CreateDirectory(sDirectorypath);
                    }
                    var file = Request.Files[Count];
                    sFileName = file.FileName;
                    string sPath = string.Empty;
                    var IPFile = sFileName.Split('.');
                    var lastIndex = sFileName.LastIndexOf(".");
                    if (lastIndex != -1)
                    {
                        string namePart = sFileName.Substring(0, lastIndex);
                        string ExtensionPart = sFileName.Substring(lastIndex + 1);
                        sPath = namePart + "." + ExtensionPart;
                        sFilePath = sDirectorypath + sPath;
                        sSavePath = sSaveDirectory + sPath;
                        file.SaveAs(sFilePath);

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("WebsiteController", "SaveFile", ex.Message);
                    }
                }
            }
            return sSavePath;
        }

        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }

        public JsonResult EncryptStringForImagePath(string inputString)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(inputString);
            string encrypted = Convert.ToBase64String(b);
            return Json(encrypted, JsonRequestBehavior.AllowGet);
        }

        private string SaveImage(string baseImage, string sExtension, string fileName)
        {
            string sImagePath = string.Empty;
            //this is to rename baseimgae64 formart.
            string sToReplaceBaseFormat = string.Format("data:image/{0};base64,", (sExtension == "jpg") ? "jpeg" : sExtension);
            string strBaseimage64 = string.Empty;
            string filePath = string.Empty;
            //actual replacement 
            strBaseimage64 = baseImage.Replace(sToReplaceBaseFormat, string.Empty);

            string _sDirectorypath = string.Empty;
            // bool _bSuccess = true;
            _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STUDENT_IMGAE_PATH;

            if (!Directory.Exists(_sDirectorypath))
            {
                Directory.CreateDirectory(_sDirectorypath);
            }
            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(strBaseimage64);
            if (!string.IsNullOrEmpty(fileName))
            {
                filePath = _sDirectorypath + fileName + "." + sExtension;
                sImagePath = "\\" + Common.URLPages.STUDENT_IMGAE_PATH + fileName + "." + sExtension;
            }
            //Save the Byte Array as Image File.and replace file if exists
            System.IO.File.WriteAllBytes(filePath, imageBytes);
            return sImagePath;
        }

        #endregion

        #region achievement approval Page 
        public ActionResult AchievementsListforApproval1()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            var Department = (List<cp_Department_2017>)CMS.DAO.CMSHandler.FetchData<cp_Department_2017>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchsupDepartmentList), sAcademicYear).DataSource.Data;
            if (Department != null && Department.Count > 0)
            {
                objViewModel.sDepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);
            }
            var Achievement = (List<WEB_SUP_ACHIEVEMENT>)CMS.DAO.CMSHandler.FetchData<WEB_SUP_ACHIEVEMENT>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchsupAchievementList), sAcademicYear).DataSource.Data;
            if (Achievement != null && Achievement.Count > 0)
            {
                objViewModel.sAchievement = new SelectList(Achievement, Common.SUP_ACHIEVEMENT.ACHIEVEMENT_ID, Common.SUP_ACHIEVEMENT.ACHIEVEMENT_NAME);
            }
            var sdglist = (List<WEB_SDGLIST>)CMS.DAO.CMSHandler.FetchData<WEB_SDGLIST>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchSDGList), sAcademicYear).DataSource.Data;
            if (sdglist != null && sdglist.Count > 0)
            {
                objViewModel.sSdgList = new SelectList(sdglist, Common.SUP_SDGLIST.SDG_ID, Common.SUP_SDGLIST.SDG_NAME_LIST);
            }
            var staffList = (List<stf_Personal_Info>)CMS.DAO.CMSHandler.FetchData<stf_Personal_Info>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchStaffList), sAcademicYear).DataSource.Data;
            if (staffList != null && staffList.Count > 0)
            {
                objViewModel.sStaffList = new SelectList(staffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
            }
            var ObjModel = new WEB_HOME_EVENTS();
            ObjModel.ENTRY_ID = sStaffId;
            var lihomeEvents = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchWebAchievementforUpdateApprovalStatus), sAcademicYear).DataSource.Data;

            if (lihomeEvents != null && lihomeEvents.Count > 0)
            {
                objViewModel.liWebsiteEvent = lihomeEvents;
            }
            return View(objViewModel);
        }

        public JsonResult AchievementStatusUpdate(string sAchieveId, string approveStatus)
        {

            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sSemesterType = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            var ObjModel = new WEB_HOME_EVENTS();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.ACHIEVEMENT_ID = sAchieveId;
                    ObjModel.APPROVE_LEVEL1 = approveStatus;
                    ObjModel.APPROVERID1 = sStaffId;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateWebAchievementApproveStatus1), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("WebisteController", "SaveProject", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        //aproval 2 
        public ActionResult AchievementsListforApproval2()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            var ObjModel = new WEB_HOME_EVENTS();
            ObjModel.ENTRY_ID = sStaffId;
            var lihomeEvents = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchApprovedachievementforSlevelApproval), sAcademicYear).DataSource.Data;

            if (lihomeEvents != null && lihomeEvents.Count > 0)
            {
                objViewModel.liWebsiteEvent = lihomeEvents;
            }
            return View(objViewModel);
        }

        public JsonResult AchievementStatusUpdate2(string sAchieveId, string approveStatus)
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            var ObjModel = new WEB_HOME_EVENTS();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.ACHIEVEMENT_ID = sAchieveId;
                    ObjModel.APPROVER_LEVEL2 = approveStatus;
                    ObjModel.APPROVERID2 = sStaffId;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateWebAchievementApproveStatus2), sAcademicYear);
                    if (resultArgs.Success)
                    {
                        var fetchCurrentUpdateRowe = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchHomeAchievementbyIDForInsertWebsite), sAcademicYear).DataSource.Data;
                        if (fetchCurrentUpdateRowe != null && fetchCurrentUpdateRowe.Count > 0)
                        {
                            ObjModel.ACHIEVEMNT_CATEGORY = fetchCurrentUpdateRowe.FirstOrDefault().ACHIEVEMNT_CATEGORY;
                            ObjModel.DATE = fetchCurrentUpdateRowe.FirstOrDefault().DATE;
                            ObjModel.TITLE = fetchCurrentUpdateRowe.FirstOrDefault().TITLE;
                            ObjModel.DESCRIPTION = fetchCurrentUpdateRowe.FirstOrDefault().DESCRIPTION;
                            ObjModel.DEPARTMENT = fetchCurrentUpdateRowe.FirstOrDefault().DEPARTMENT;
                            ObjModel.SDG_ID = fetchCurrentUpdateRowe.FirstOrDefault().SDG_ID;
                            ObjModel.LINK = fetchCurrentUpdateRowe.FirstOrDefault().LINK;
                            ObjModel.LINKTEXT = fetchCurrentUpdateRowe.FirstOrDefault().LINKTEXT;
                            ObjModel.ENTRY_ID = fetchCurrentUpdateRowe.FirstOrDefault().ENTRY_ID;
                            ObjModel.STAFF_ID = fetchCurrentUpdateRowe.FirstOrDefault().STAFF_ID;
                            ObjModel.FILE_PATH = "https://erp.sjctnc.edu.in/" + fetchCurrentUpdateRowe.FirstOrDefault().FILE_PATH;
                            var splitImgPath = fetchCurrentUpdateRowe.FirstOrDefault().IMG_PATH.Split(',');
                            foreach (var imgSplit in splitImgPath)
                            {
                                ObjModel.IMG_PATH += "https://erp.sjctnc.edu.in/" + imgSplit + ",";
                            }
                            ObjModel.IMG_PATH = ObjModel.IMG_PATH.TrimEnd(',');
                            resultArgs = CMSHandler.SaveOrUpdate(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.SavehomeAchievementinWebsiteDB), sAcademicYear);
                            objResultData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                            objResultData.ErrorCode = Common.ErrorCode.Ok;
                        }
                        objResultData.Message = Common.Messages.RecordsUpdatedSuccessfully;
                        objResultData.ErrorCode = Common.ErrorCode.Ok;

                    }
                    else
                    {
                        objResultData.Message = Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("WebisteController", "SaveProject", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region campus news guidlines 
        public ActionResult CampusNewsGuidLines()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                int nAcademicYear = Convert.ToInt32(sAcademicYear);
                nAcademicYear--;
                sAcademicYear = sAcademicYear + "," + nAcademicYear.ToString();

                var ObjModel = new WEB_HOME_EVENTS();
                ObjModel.ENTRY_ID = sStaffId;
                var lihomeEvents = (List<WEB_HOME_EVENTS>)CMSHandler.FetchData<WEB_HOME_EVENTS>(ObjModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchApprovedachievementforSlevelApproval), sAcademicYear).DataSource.Data;

                if (lihomeEvents != null && lihomeEvents.Count > 0)
                {
                    objViewModel.liWebsiteEvent = lihomeEvents;
                }


            }
            return View(objViewModel);
        }
        #endregion

        #region  Staff personal edit page for Staff
        /// <summary>
        /// staff profile edit page for staff user for show this into website  facutly profile page.
        /// </summary>
        /// <author>Yesudass</author>
        /// <date>2025-07-10</date>
        /// <returns></returns>
        /// 
        public ActionResult EditStaffProfileByStaff()
        {
            Session[Common.SESSION_VARIABLES.STAFF_ID] = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            StaffViewModel objStudentInfo = new StaffViewModel();
            return View(objStudentInfo);
        }
        public ActionResult BindStaffProfileByStaffID()
        {
            STF_PROFILE objAcademicInfo = new STF_PROFILE();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                // Religion ...
                var departmentList = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                if (departmentList != null && departmentList.Count > 0)
                {
                    objAcademicInfo.sDepartmentList = new SelectList(departmentList, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }
                var gender = (List<SupGender>)CMSHandler.FetchData<SupGender>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchGender)).DataSource.Data;
                if (gender != null && gender.Count > 0)
                {
                    objAcademicInfo.sGender = new SelectList(gender, Common.SUP_GENDER.GENDER_ID, Common.SUP_GENDER.GENDER_NAME);
                }
                var Designation = (List<Sup_Designation>)CMSHandler.FetchData<Sup_Designation>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDesignation)).DataSource.Data;
                if (Designation != null && Designation.Count > 0)
                {
                    objAcademicInfo.sDesignation = new SelectList(Designation, Common.SUP_DESIGNATION.DESIGNATIONID, Common.SUP_DESIGNATION.DESIGNATION);
                }
                var liRolesResponce = (List<SUP_STAFF_ROLES_RESPONSIBILE>)CMSHandler.FetchData<SUP_STAFF_ROLES_RESPONSIBILE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsupStaffRolesResponsibile)).DataSource.Data;
                if (liRolesResponce != null && liRolesResponce.Count > 0)
                {
                    objAcademicInfo.liRolesResponse = liRolesResponce;
                    objAcademicInfo.sRolesResponse = new SelectList(liRolesResponce, Common.SUP_STAFF_ROLES_RESPONSIBILE.STF_ROLE_RES_ID, Common.SUP_STAFF_ROLES_RESPONSIBILE.ROLES__RESPONSIBILITIES);
                }
                return View(objAcademicInfo);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjHandler = new ErrorLog())
                {
                    ObjHandler.WriteError("StudentController", "EditStuAcademicInfo", "Err MSg: " + ex.Message, "Query is empty!");
                    ObjHandler.WriteError("StudentController", "EditStuAcademicInfo", ex.Message);
                }
                return View(objAcademicInfo);
            }
        }
        public JsonResult UpdateStfProfile()
        {
            string sStudentID = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            var str = Request.Form["str"];
            string filePath = string.Empty;
            string profileID = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            try
            {
                var Result = JsonConvert.DeserializeObject<STF_PROFILE>(str);
                Result.LAST_UPDATE_DATE = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                if (!string.IsNullOrEmpty(Result.PROFILE_ID))
                {
                    resultArgs = CMSHandler.SaveOrUpdate(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffProfileByID));

                }
                else
                {
                    resultArgs = CMSHandler.SaveOrUpdate(Result, StaffSQL.GetStaffSQL(StaffSQLCommands.SaveStaffProfileByID), sAcademicYear, true);
                    Result.PROFILE_ID = resultArgs.RowUniqueId.ToString();
                }

                if (resultArgs.Success)
                {
                    using (StaffViewModel objViewModel = new StaffViewModel())
                    {
                        _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.STAFF_PROFILE_IMAGE;
                        if (!Directory.Exists(_sDirectorypath))
                        {
                            Directory.CreateDirectory(_sDirectorypath);
                        }
                        // Image Updation ....
                        var Rollno = Result.STAFF_CODE;
                        
                        if (Result.PHOTO == "REMOVE")
                        {
                            Result.PHOTO = "";
                            string sSql = StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffProfiletimage)
                                                  .Replace("?PROFILE_ID", Result.PROFILE_ID);
                            CMSHandler.SaveOrUpdate(Result, sSql, sAcademicYear, true);
                        }
                        else if (!string.IsNullOrEmpty(Result.PHOTO) && Result.PHOTO.StartsWith("data:image"))
                        {
                            // Handle Cropped Image (Base64)
                            string base64Data = Result.PHOTO.Split(',')[1];
                            byte[] imageBytes = Convert.FromBase64String(base64Data);
                            string imgName = Result.STAFF_CODE + ".jpg";
                            string imgPath = Path.Combine(_sDirectorypath, imgName);
                            System.IO.File.WriteAllBytes(imgPath, imageBytes);

                            Result.PHOTO = "\\" + Common.URLPages.STAFF_PROFILE_IMAGE + imgName;
                            string sSql = StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffProfiletimage)
                                                  .Replace("?PROFILE_ID", Result.PROFILE_ID);
                            CMSHandler.SaveOrUpdate(Result, sSql, sAcademicYear, true);
                        }
                        else
                        {
                            // Profile image upload (Legacy/Secondary) - Only if no cropped image/remove request
                            var profileImageFile = Request.Files["profileImage"];
                            if (profileImageFile != null && profileImageFile.ContentLength > 0)
                            {
                                var imgExt = Path.GetExtension(profileImageFile.FileName);
                                var imgName = Result.STAFF_CODE + imgExt;
                                var imgPath = Path.Combine(_sDirectorypath, imgName);
                                profileImageFile.SaveAs(imgPath);
                                Result.PHOTO = "\\" + Common.URLPages.STAFF_PROFILE_IMAGE + imgName;

                                string sSql = StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffProfiletimage)
                                                      .Replace("?PROFILE_ID", Result.PROFILE_ID);
                                CMSHandler.SaveOrUpdate(Result, sSql, sAcademicYear, true);
                            }
                        }
                        if (resultArgs.Success)
                        {

                            sResult = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        }
                        else
                        {
                            sResult = Common.ErrorCode.NotAcceptable;
                        }

                        string sDirectorypath = string.Empty,
                            sFilePath = string.Empty;
                        // Attachments upload (multi-file support)
                        var newFilePaths = new List<string>();
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var fileKey = Request.Files.AllKeys[i];
                            if (fileKey != null && fileKey.Equals("attachments", StringComparison.OrdinalIgnoreCase))
                            {
                                var file = Request.Files[i];
                                if (file != null && file.ContentLength > 0)
                                {
                                    var relativePath = Common.URLPages.STAFF_PROFILE_FILE.TrimStart('\\');
                                    var attachmentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                                    if (!Directory.Exists(attachmentDir))
                                        Directory.CreateDirectory(attachmentDir);

                                    var ext = Path.GetExtension(file.FileName);
                                    var nameFile = Path.GetFileNameWithoutExtension(file.FileName);
                                    var saveFileName = nameFile + "_" + Guid.NewGuid().ToString().Substring(0, 8) + ext;
                                    var savePath = Path.Combine(attachmentDir, saveFileName);
                                    file.SaveAs(savePath);

                                    newFilePaths.Add(Common.URLPages.STAFF_PROFILE_FILE + saveFileName);
                                }
                            }
                        }

                        if (newFilePaths.Count > 0)
                        {
                            var existingPaths = string.IsNullOrEmpty(Result.FILE_PATH) ? "" : Result.FILE_PATH;
                            var combinedPaths = string.IsNullOrEmpty(existingPaths) 
                                ? string.Join(",", newFilePaths) 
                                : existingPaths + "," + string.Join(",", newFilePaths);
                            
                            Result.FILE_PATH = combinedPaths;
                            CMSHandler.SaveOrUpdate(Result,
                                StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffProfileFiles),
                                sAcademicYear, true);
                        }

                        return Json(sResult);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("StudentController", "EditAcadamicInfo", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("StudentController", "EditAcadamicInfo", ex.Message);
                }
                sResult = Common.ErrorCode.NotAcceptable;
                return Json(sResult);
            }

            return Json(sResult);
        }
        public JsonResult FetchStaffPesonalInfobyId()
        {
            string sSQL = string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.STAFF_ID] != null) ? Session[Common.SESSION_VARIABLES.STAFF_ID].ToString() : string.Empty;
            if (string.IsNullOrEmpty(sStaffId))
                sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sStaffId))
            {
                var dtPersonalInfo = (List<STF_PROFILE>)CMSHandler.FetchData<STF_PROFILE>(new STF_PROFILE { STAFF_ID = sStaffId }, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStaffProfilebyStaffId)).DataSource.Data;
                if (dtPersonalInfo != null && dtPersonalInfo.Count > 0)
                {
                    STF_PROFILE objPersonal = new STF_PROFILE()
                    {
                        STAFF_NAME = dtPersonalInfo.FirstOrDefault().STAFF_NAME,
                        STAFF_CODE = dtPersonalInfo.FirstOrDefault().STAFF_CODE,
                        GENDER = dtPersonalInfo.FirstOrDefault().GENDER,
                        ADDRESS = dtPersonalInfo.FirstOrDefault().ADDRESS,
                        DATE_OF_JOIN = dtPersonalInfo.FirstOrDefault().DATE_OF_JOIN,
                        PROFILE_ID = dtPersonalInfo.FirstOrDefault().PROFILE_ID,
                        STAFF_ID = dtPersonalInfo.FirstOrDefault().STAFF_ID,
                        QUALIFICATION = dtPersonalInfo.FirstOrDefault().QUALIFICATION,
                        CONTACT_NO = dtPersonalInfo.FirstOrDefault().CONTACT_NO,
                        EMAIL_ID = dtPersonalInfo.FirstOrDefault().EMAIL_ID,
                        PHOTO = dtPersonalInfo.FirstOrDefault().PHOTO,
                        SPECIALIZATION = dtPersonalInfo.FirstOrDefault().SPECIALIZATION,
                        DESIGNATION = dtPersonalInfo.FirstOrDefault().DESIGNATION,
                        IS_EMAIL_SHOW = dtPersonalInfo.FirstOrDefault().IS_EMAIL_SHOW,
                        IS_CONTACT_NO_SHOW = dtPersonalInfo.FirstOrDefault().IS_CONTACT_NO_SHOW,
                        COLLEGE_EMAIL_ID = dtPersonalInfo.FirstOrDefault().COLLEGE_EMAIL_ID,

                        PHD_SUPERVISOR = dtPersonalInfo.FirstOrDefault().PHD_SUPERVISOR,
                        NEW_ACCEPTING_PHD = dtPersonalInfo.FirstOrDefault().NEW_ACCEPTING_PHD,
                        ORCID_ID = dtPersonalInfo.FirstOrDefault().ORCID_ID,
                        LINKEDIN_LINK = dtPersonalInfo.FirstOrDefault().LINKEDIN_LINK,
                        GOOGLE_SCHOLAR = dtPersonalInfo.FirstOrDefault().GOOGLE_SCHOLAR,
                        SCOPUS_ID = dtPersonalInfo.FirstOrDefault().SCOPUS_ID,
                        IRINS_ID = dtPersonalInfo.FirstOrDefault().IRINS_ID,
                        DEPARTMENT = dtPersonalInfo.FirstOrDefault().DEPARTMENT,
                        FILE_PATH = dtPersonalInfo.FirstOrDefault().FILE_PATH,
                        RESEARCHER_ID = dtPersonalInfo.FirstOrDefault().RESEARCHER_ID,
                        PHD_AWARDEES = dtPersonalInfo.FirstOrDefault().PHD_AWARDEES,
                        STF_ROLE_RES_ID = dtPersonalInfo.FirstOrDefault().STF_ROLE_RES_ID,
                        ROLES_RESPONSE = dtPersonalInfo.FirstOrDefault().ROLES_RESPONSE,

                    };
                    return Json(objPersonal);
                }
                else
                {
                    sResult = "No Records Available ...!";
                    return Json(sResult);
                }
            }
            else
            {
                sResult = "Session Has Expired Please Login And Try Again ...!";
                return Json(sResult);
            }
        }
        #endregion

        /// <summary>
        /// this page is used to show the staff profile approval page for admin user to approve the staff profile for website display.
        /// </summary>
        /// <author>Yesudass D</author>
        /// <date> 2025-07-21</date>
        /// <returns></returns>
        #region Staff Profile approval page 
        public ActionResult StaffProfileApproval()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new StaffViewModel();
            var departmentList = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCpDepartmentFaculty)).DataSource.Data;
            if (departmentList != null && departmentList.Count > 0)
            {
                objViewModel.DepartmentList = new SelectList(departmentList, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
            }
            objViewModel.liStfProfie = (List<STF_PROFILE>)CMSHandler.FetchData<STF_PROFILE>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchAllStfProfileForApproval), sAcademicYear).DataSource.Data;

            return View(objViewModel);
        }
        public ActionResult GetStaffProfileApprovalListByDeptID(string departmentId)
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                var objViewModel = new StaffViewModel();
                objViewModel.liStfProfie = (List<STF_PROFILE>)CMSHandler.FetchData<STF_PROFILE>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStfProfileForApprovalbyDeptID).Replace("?DEPARTMENT", departmentId), sAcademicYear).DataSource.Data;

                return View(objViewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjHandler = new ErrorLog())
                {
                    ObjHandler.WriteError("StudentController", "EditStuAcademicInfo", "Err MSg: " + ex.Message, "Query is empty!");
                    ObjHandler.WriteError("StudentController", "EditStuAcademicInfo", ex.Message);
                }
                return View(ex);
            }
        }
        public JsonResult StaffProfileApprovalStatusUpdate(string sProfileId, string approveStatus)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sSemesterType = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            var ObjModel = new STF_PROFILE();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.PROFILE_ID = sProfileId;
                    ObjModel.APPROVED_DATE = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    ObjModel.IS_SHOW_WEBSITE = approveStatus;
                    ObjModel.APPROVER_ID = sStaffId;
                    resultArgs = CMSHandler.SaveOrUpdate(ObjModel, StaffSQL.GetStaffSQL(StaffSQLCommands.UpdateStaffProfileApprovalStaff), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("WebisteController", "SaveProject", ex.Message);
                    }

                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Menu Click Report
        public ActionResult MenuClicksReport(string sFromDate, string sToDate)
        {
            var objViewModel = new WebsiteViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (string.IsNullOrEmpty(sFromDate))
            {
                sFromDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(sToDate))
            {
                sToDate = DateTime.Now.ToString("yyyy-MM-dd");
            }

            objViewModel.sFromDate = sFromDate;
            objViewModel.sToDate = sToDate;
            objViewModel.sClickDate = sFromDate == sToDate ? sFromDate : sFromDate + " to " + sToDate;

            try
            {
                // Fetch Date-wise report
                var objModel = new WEB_MENU_CLICKS_REPORT { FROM_DATE = sFromDate, TO_DATE = sToDate };
                var liDateWise = (List<WEB_MENU_CLICKS_REPORT>)CMSHandler.FetchData<WEB_MENU_CLICKS_REPORT>(objModel, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchMenuClicksReport), sAcademicYear).DataSource.Data;
                if (liDateWise != null)
                {
                    objViewModel.liMenuClicksReport = liDateWise;
                }

                // Fetch Menu-wise report
                var liMenuWise = (List<WEB_MENU_CLICKS_REPORT>)CMSHandler.FetchData<WEB_MENU_CLICKS_REPORT>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchMenuWiseTotalClicksReport), sAcademicYear).DataSource.Data;
                if (liMenuWise != null)
                {
                    objViewModel.liMenuWiseTotalClicksReport = liMenuWise;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("WebsiteController", "MenuClicksReport", ex.Message);
                }
            }

            return View(objViewModel);
        }
        #endregion



        #region Staff Role Responsibilities
        public ActionResult RoleResponsibilityList()
        {
            WebsiteViewModel viewmodal = new WebsiteViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                viewmodal.liRoleResponsibility = (List<ROLE_RESPONSIBILITY>)CMSHandler.FetchData<ROLE_RESPONSIBILITY>(null, WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchResponsibilities), sAcademicYear).DataSource.Data;

            }
            return View(viewmodal);
        }

        public JsonResult SaveRoleResponsibility(string category)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = JsonConvert.DeserializeObject<ROLE_RESPONSIBILITY>(category);

                    if (result.STF_ROLE_RES_ID != "" && result.STF_ROLE_RES_ID != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Website.WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.UpdateStaffRoleResponsibilities), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(result, SQL.Website.WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.AddStaffRoleResponsibilities), sAcademicYear);
                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Website", "SaveRoleResponsibility", ex.ToString());
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


        public JsonResult FetchStaffRoleResponsibilities(string STF_ROLE_RES_ID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && STF_ROLE_RES_ID != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var result = (List<ROLE_RESPONSIBILITY>)CMSHandler.FetchData<ROLE_RESPONSIBILITY>(new ROLE_RESPONSIBILITY() { STF_ROLE_RES_ID = STF_ROLE_RES_ID }, SQL.Website.WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.FetchRoleResponsibilityById), sAcademicYear).DataSource.Data;
                    if (result != null && result.Count > 0)
                        objResultData.oResult = result;
                    else
                        objResultData.Message = Common.Messages.NoRecordsfound;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Website", "FetchStaffRoleResponsibilities", ex.Message);
                    }
                }

            }
            else
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteStaffRoleResponsibilities(string STF_ROLE_RES_ID)
        {
            JsonResultData objResultData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null && STF_ROLE_RES_ID != "")
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();


                    resultArgs = CMSHandler.SaveOrUpdate(new ROLE_RESPONSIBILITY { STF_ROLE_RES_ID = STF_ROLE_RES_ID, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.Website.WebsiteSQL.GetWebsiteSQLSQL(WebsiteSQLCommands.DeleteRoleResponsibility), sAcademicYear);
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("Website", "DeleteStaffRoleResponsibilities", ex.Message);
                    }
                }

            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}