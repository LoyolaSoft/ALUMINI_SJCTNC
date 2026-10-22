using CMS.DAO;
using CMS.SQL.IQAC;
using CMS.SQL.Placement;
using CMS.SQL.Student;
using CMS.SQL.SupportData;
using CMS.SQL.Website;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class PlaceMentController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData ObjJsonData = new JsonResultData();
        // GET: Website
        public ActionResult Index()
        {
            return View();
        }


        #region Placement Details Entry Page 
        public ActionResult StuPlacementEntryPage()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new STU_PLACEMENT_NEWS();

                var liStuPlacement = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchstuPlacementNews), sAcademicYear).DataSource.Data;

                if (liStuPlacement != null && liStuPlacement.Count > 0)
                {
                    objViewModel.liPlacementnews = liStuPlacement;
                }
                // fetch project type

                var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicYear != null)
                {
                    objViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        public ActionResult BindAcYearWisePlacementList(string sAcYear)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcYear))
            {
                var ObjModel = new STU_PLACEMENT_NEWS();

                var liStuPlacement = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchstuPlacementNews), sAcYear).DataSource.Data;

                if (liStuPlacement != null && liStuPlacement.Count > 0)
                {
                    objViewModel.liPlacementnews = liStuPlacement;
                }
                // fetch project type

                var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicYear != null)
                {
                    objViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        public JsonResult FetchstuPlacementNewsById(string sPlacementId)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                objResultData.oResult = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(new STU_PLACEMENT_NEWS() { PLACEMENT_ID = sPlacementId }, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchstuPlacementNewsByID), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("PlacementController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePalcementNews()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {

                    var sJson = Request.Form["jsonData"];
                    //string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<STU_PLACEMENT_NEWS>(sJson);
                    if (result != null)
                    {
                        result.FROM_DATE = CommonMethods.DateConvertionForPayU(result.FROM_DATE);
                        result.TO_DATE = CommonMethods.DateConvertionForPayU(result.TO_DATE);
                        result.ACADEMIC_YEAR = sAcademicYear;
                        result.SEMESTER_TYPE = sSemesterType;
                        result.ENTRY_ID = sStaffId;

                        if (!string.IsNullOrEmpty(result.PLACEMENT_ID))
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.UpdateStuplacementNewsWithoutImage), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            resultArgs = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.SaveStuplacementNews), sAcademicYear, true);
                            result.PLACEMENT_ID = resultArgs.RowUniqueId.ToString();
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }

                        string sfilePath = Request.Form["sfiles"];
                        if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(sfilePath))
                        {
                            string sFilePath = string.Empty;

                            //update file 
                            if (!string.IsNullOrEmpty(sfilePath))
                            {
                                //for upload img path
                                var imgPath = sfilePath.Split(',');
                                foreach (var item2 in imgPath)
                                {
                                    sFilePath += DecryptString(item2) + ",";
                                }
                            }
                            if (!string.IsNullOrEmpty(result.FILE_PATH))
                            {
                                //var fileCount = Convert.ToInt64(result.FILE_PATH) - 1;
                                ////for add img 
                                //int Count = Convert.ToInt32(fileCount);
                                int Count = 0;
                                var fmgPath = result.FILE_PATH.Split(',');
                                foreach (var item2 in fmgPath)
                                {
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.PlacementNews_FILE_PATH, Common.URLPages.PlacementNews_FILE_PATH);
                                    sFilePath += sPath + ","; Count++;
                                }
                            }
                            if (!string.IsNullOrEmpty(sFilePath))
                            {
                                result.FILE_PATH = sFilePath.TrimEnd(',');
                            }
                            var FileUpdateQuery = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.UpdatePlacementFilePathByID));
                        }
                        string simgPath = Request.Form["photo"];
                        if (!string.IsNullOrEmpty(result.IMG_PATH) || !string.IsNullOrEmpty(simgPath))
                        {
                            string sImgPath = string.Empty;

                            //update file 
                            if (!string.IsNullOrEmpty(simgPath))
                            {
                                //for upload img path
                                var imgPath = simgPath.Split(',');
                                foreach (var item2 in imgPath)
                                {
                                    sImgPath += DecryptString(item2) + ",";
                                }
                            }
                            if (!string.IsNullOrEmpty(result.IMG_PATH))
                            {
                                //for add img 
                                int Count = 0;
                                var fmgPath = result.IMG_PATH.Split(',');
                                foreach (var item2 in fmgPath)
                                {
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.PlacementNews_IMG_PATH, Common.URLPages.PlacementNews_IMG_PATH);
                                    sImgPath += sPath + ","; Count++;
                                }
                            }
                            if (!string.IsNullOrEmpty(sImgPath))
                            {
                                //sImgPath = sImgPath.Replace(@"\\", @"\");

                                sImgPath = Regex.Replace(sImgPath, @"\\{2,}", @"\");
                                result.IMG_PATH = sImgPath.TrimEnd(',');
                            }
                            var FileUpdateQuery = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.UpdatePlacementImgPathbyID));
                        }

                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("PlacementController", "SavePlacementNews", ex.Message);
                }

                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }


            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteStuPlacementNews(string sPlacement)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_PLACEMENT_NEWS();
                    ObjModel.PLACEMENT_ID = sPlacement;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.DeleteStudentPlacementNews));
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
                        ObjLog.WriteError("PlacementController", "deleted", ex.Message);
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

        #region Placment news for student
        public ActionResult StuPlacementviewPageForStudent()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string user_id = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string Role_id = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var ObjModel = new STU_PLACEMENT_NEWS();
                    if (Role_id == Common.SupRole.STUDENT)
                    {
                        var liStuinfo = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = user_id, ACADEMIC_YEAR = sAcademicYear }, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchStuInfoForValidate), sAcademicYear).DataSource.Data;
                        if (liStuinfo != null)
                        {
                            var class_year = liStuinfo.FirstOrDefault().CLASS_YEAR;
                            var class_level = liStuinfo.FirstOrDefault().CLASS_LEVEL;
                            Session[Common.SESSION_VARIABLES.CLASS_LEVEL] = class_level;
                            Session[Common.SESSION_VARIABLES.CLASS_YEAR] = class_year;

                            if (class_level == Common.SupClassLevel.UG && class_year == "3")
                            {
                                var liStuPlacement = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchActiveInterviewList).Replace("-- UG", ""), sAcademicYear).DataSource.Data;

                                if (liStuPlacement != null && liStuPlacement.Count > 0)
                                {
                                    objViewModel.liPlacementnews = liStuPlacement;
                                }
                            }
                            else if (class_level == Common.SupClassLevel.PG && class_year == "2")
                            {
                                var liStuPlacement = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchActiveInterviewList).Replace("-- PG", ""), sAcademicYear).DataSource.Data;

                                if (liStuPlacement != null && liStuPlacement.Count > 0)
                                {
                                    objViewModel.liPlacementnews = liStuPlacement;
                                }
                            }

                        }
                    }
                    else
                    {
                        var liStuPlacement = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchActiveInterviewList), sAcademicYear).DataSource.Data;

                        if (liStuPlacement != null && liStuPlacement.Count > 0)
                        {
                            objViewModel.liPlacementnews = liStuPlacement;
                        }
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
                // fetch project type
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("PlacementController", "deleted", ex.Message);
                }
                ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }


            return View(objViewModel);
        }
        public ActionResult BindplacmentDetailsCategoryWise(string CategoryStatus)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string UserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string ClassLevel = Session[Common.SESSION_VARIABLES.CLASS_LEVEL] != null ? Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString() : string.Empty;
            string ClassYear = Session[Common.SESSION_VARIABLES.CLASS_YEAR] != null ? Session[Common.SESSION_VARIABLES.CLASS_YEAR].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();
            string Role_id = Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new STU_PLACEMENT_NEWS();
                switch (CategoryStatus)
                {
                    case "1":
                        {
                            if (Role_id == Common.SupRole.STUDENT)
                            {
                                if (ClassLevel == Common.SupClassLevel.UG && ClassYear == "3")
                                {
                                    objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchActiveInterviewList).Replace("-- UG", ""), sAcademicYear).DataSource.Data;

                                }
                                else if (ClassLevel == Common.SupClassLevel.PG && ClassYear == "2")
                                {
                                    objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchActiveInterviewList).Replace("-- PG", ""), sAcademicYear).DataSource.Data;

                                }
                            }
                            else
                            {
                                objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchActiveInterviewList), sAcademicYear).DataSource.Data;

                            }
                            break;
                        }
                    case "2":
                        {
                            if (Role_id == Common.SupRole.STUDENT)
                            {
                                if (ClassLevel == Common.SupClassLevel.UG && ClassYear == "3")
                                {
                                    objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchCompletedInterviewList).Replace("-- UG", ""), sAcademicYear).DataSource.Data;

                                }
                                else if (ClassLevel == Common.SupClassLevel.PG && ClassYear == "2")
                                {
                                    objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchCompletedInterviewList).Replace("-- PG", ""), sAcademicYear).DataSource.Data;

                                }
                            }
                            else
                            {
                                objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchCompletedInterviewList), sAcademicYear).DataSource.Data;

                            }
                            break;
                        }
                    case "3":
                        {
                            if (Role_id == Common.SupRole.STUDENT)
                            {
                                if (ClassLevel == Common.SupClassLevel.UG && ClassYear == "3")
                                {
                                    objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchOtherAdList).Replace("-- UG", ""), sAcademicYear).DataSource.Data;

                                }
                                else if (ClassLevel == Common.SupClassLevel.PG && ClassYear == "2")
                                {
                                    objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchOtherAdList).Replace("-- PG", ""), sAcademicYear).DataSource.Data;

                                }
                            }
                            else
                            {
                                objViewModel.liPlacementnews = (List<STU_PLACEMENT_NEWS>)CMSHandler.FetchData<STU_PLACEMENT_NEWS>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchOtherAdList), sAcademicYear).DataSource.Data;

                            }
                            break;
                        }
                    default:
                        break;

                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        #endregion

        #region Placement Details Entry Page 
        public ActionResult ListPlacedStudentYearWise()
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ObjModel = new PLACED_STUDENT_LIST();

                var liStuPlacement = (List<PLACED_STUDENT_LIST>)CMSHandler.FetchData<PLACED_STUDENT_LIST>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchInterviewPlacedStudentList).Replace("?YEAR_OF_PLACEMENT", sAcademicYear), sAcademicYear).DataSource.Data;

                if (liStuPlacement != null && liStuPlacement.Count > 0)
                {
                    objViewModel.liInterviewPlacedList = liStuPlacement;
                }
                // fetch project type

                var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicYear != null)
                {
                    objViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        public ActionResult BindAcYearWiseInterViewPlacedStuList(string sAcYear)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var objViewModel = new WebsiteViewModel();

            if (!string.IsNullOrEmpty(sAcYear))
            {
                var ObjModel = new STU_PLACEMENT_NEWS();

                var liStuPlacement = (List<PLACED_STUDENT_LIST>)CMSHandler.FetchData<PLACED_STUDENT_LIST>(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchInterviewPlacedStudentList).Replace("?YEAR_OF_PLACEMENT", sAcYear), sAcYear).DataSource.Data;

                if (liStuPlacement != null && liStuPlacement.Count > 0)
                {
                    objViewModel.liInterviewPlacedList = liStuPlacement;
                }
                // fetch project type
                var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                if (liAcademicYear != null)
                {
                    objViewModel.sAcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(objViewModel);
        }
        public JsonResult FetchInterViewPlacedInfoById(string sPlacementId)
        {
            JsonResultData objResultData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {
                objResultData.oResult = (List<PLACED_STUDENT_LIST>)CMSHandler.FetchData<PLACED_STUDENT_LIST>(new PLACED_STUDENT_LIST() { PLACED_ID = sPlacementId }, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchInterviewPlacedStudentListById), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("PlacementController", "FetchAchievementsById", ex.Message);
                }
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckRollNoExistorNot(string rollNo)
        {
            JsonResultData objResultData = new JsonResultData();
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                var checkRollNo = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { ROLL_NO = rollNo }, SQL.Placement.PlacementSQL.GetPlacementSQL(PlacementSQLCommands.CheckRollNoExistOrNot), sAcademicYear).DataSource.Data;
                if (checkRollNo != null)
                {
                    ViewBag.studentID = checkRollNo.FirstOrDefault().STUDENT_ID;
                    objResultData.Message = "";
                    objResultData.ErrorCode = Common.ErrorCode.Ok;
                }
                else
                {
                    objResultData.Message = "Invalid Roll Number Please Check ";
                    objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            catch (Exception ex)
            {
                objResultData.Message = Common.ErrorMessage.InternalServerError;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData);
        }
        public JsonResult SaveInterviewPlacedStudents()
        {
            ResultArgs resultArgs = new ResultArgs();
            var objResultData = new JsonResultData();
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string filePath = string.Empty;
            string sImagePath = string.Empty;
            string _sDirectorypath = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var sJson = Request.Form["jsonData"];
                    //string sJsonData = Request.Form["sJsonData"];
                    var result = JsonConvert.DeserializeObject<PLACED_STUDENT_LIST>(sJson);
                    if (result != null)
                    {
                        result.ENTRY_ID = sStaffId;
                        var checkRollNo = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { ROLL_NO = result.ROLL_NO }, SQL.Placement.PlacementSQL.GetPlacementSQL(PlacementSQLCommands.CheckRollNoExistOrNot), sAcademicYear).DataSource.Data;
                        if (checkRollNo == null)
                        {

                            objResultData.Message = " Invalid Roll Number Please Check ";
                            objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
                            return Json(objResultData, JsonRequestBehavior.DenyGet);
                        }
                        else
                        {
                            result.STUDENT_ID = checkRollNo.FirstOrDefault().STUDENT_ID;

                            if (!string.IsNullOrEmpty(result.PLACED_ID))
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.UpdatePlacedStudentDetailsExceptFileImage), sAcademicYear);
                                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                                objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.SavePlacedStudentDetails), sAcademicYear, true);
                                result.PLACED_ID = resultArgs.RowUniqueId.ToString();
                                objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                            }
                            string sfilePath = Request.Form["sfiles"];
                            if (!string.IsNullOrEmpty(result.FILE_PATH) || !string.IsNullOrEmpty(sfilePath))
                            {
                                string sFilePath = string.Empty;

                                //update file 
                                if (!string.IsNullOrEmpty(sfilePath))
                                {
                                    //for upload img path
                                    var imgPath = sfilePath.Split(',');
                                    foreach (var item2 in imgPath)
                                    {
                                        sFilePath += DecryptString(item2) + ",";
                                    }
                                }
                                if (!string.IsNullOrEmpty(result.FILE_PATH))
                                {
                                    //var fileCount = Convert.ToInt64(result.FILE_PATH) - 1;
                                    ////for add img 
                                    //int Count = Convert.ToInt32(fileCount);
                                    int Count = 0;
                                    var fmgPath = result.FILE_PATH.Split(',');
                                    foreach (var item2 in fmgPath)
                                    {
                                        string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.InterViewPlaced_FILE_PATH, Common.URLPages.InterViewPlaced_FILE_PATH);
                                        sFilePath += sPath + ","; Count++;
                                    }
                                }
                                if (!string.IsNullOrEmpty(sFilePath))
                                {
                                    result.FILE_PATH = sFilePath.TrimEnd(',');
                                }
                                var FileUpdateQuery = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.UpdatePlacedStudentDetailsFilePathByID));
                            }

                            // Image Updation ....
                            var Rollno = result.ROLL_NO;
                            var File = Request.Files["inputimg"];
                            if (File != null)
                            {

                                string ImgName = File.FileName;
                                var Img = ImgName.Split('.');
                                string fileName = ImgName.Replace(Img[0], result.ROLL_NO);

                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    int Count = 0;
                                    filePath = _sDirectorypath + fileName;
                                    sImagePath = "\\" + Common.URLPages.InterViewPlaced_IMG_PATH + fileName;
                                    //File.SaveAs(filePath);
                                    string sPath = SaveFile(Count, AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.PlacementNews_IMG_PATH, Common.URLPages.PlacementNews_IMG_PATH);

                                    //File.SaveAs(filePath);
                                    result.IMG_PATH = sPath;

                                }
                                var FileUpdateQuery = CMSHandler.SaveOrUpdate(result, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.UpdatePlacedStudentDetailsImgPathbyID));


                            }

                        }

                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("PlacementController", "SavePlacementNews", ex.Message);
                }

                objResultData.Message = ex.Message;
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }


            if (string.IsNullOrEmpty(objResultData.Message))
            {
                objResultData.Message = "No Changes Made";
                objResultData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInterViewPlacedStudentd(string sPlacement)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new PLACED_STUDENT_LIST();
                    ObjModel.PLACED_ID = sPlacement;
                    ObjModel.IS_DELETED = Common.DeleteFlag.Deleted;
                    var DeleteQuery = CMSHandler.SaveOrUpdate(ObjModel, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.DeleteInterViewPlacedStudentNews));
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
                        ObjLog.WriteError("PlacementController", "deleted", ex.Message);
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
        public ActionResult ExportInterviewPlacedStudentDetails(string sAcYear)
        {

            var csv = new StringBuilder();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var liStuPlacement = new List<PLACED_STUDENT_LIST>();
            if (sAcYear != null && sAcYear !="-- Select --")
            {

                liStuPlacement = (List<PLACED_STUDENT_LIST>)CMSHandler.FetchData<PLACED_STUDENT_LIST>(null, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchInterviewPlacedStudentList).Replace("?YEAR_OF_PLACEMENT", sAcYear), sAcYear).DataSource.Data;

            }
            else
            {
                liStuPlacement = (List<PLACED_STUDENT_LIST>)CMSHandler.FetchData<PLACED_STUDENT_LIST>(null, PlacementSQL.GetPlacementSQL(PlacementSQLCommands.FetchInterviewPlacedStudentList).Replace("?YEAR_OF_PLACEMENT", sAcademicYear), sAcademicYear).DataSource.Data;

            }
            if (liStuPlacement != null)
            {
                // Add headers
                // CSV header based on the provided database fields
                csv.AppendLine("S.NO,FIRST_NAME,ROLL_NO,PROGRAMME_NAME,CLASS_LEVEL,YEAR_OF_PLACEMENT,COMPANY_NAME,POSITION_OFFERED,SALARY_PACKAGE,REMARKS,FILE_PATH,IMG_PATH");

                // Add data rows
                int Count = 0;
                foreach (var student in liStuPlacement)
                {
                    Count++;
                    if (student.FILE_PATH != null && student.FILE_PATH != "")
                    {
                        var splitedFile = student.FILE_PATH.Split(',');
                        student.FILE_PATH = "";
                        foreach (var file in splitedFile)
                        {
                            file.TrimEnd(',');
                            var files = "https://erp.sjctnc.edu.in/" + file;
                            student.FILE_PATH += files + " , ";
                        }
                    }
                    if (student.IMG_PATH != null && student.IMG_PATH != "")
                    {
                        student.IMG_PATH = "https://erp.sjctnc.edu.in/" + student.IMG_PATH;
                    }
                    csv.AppendLine($"\"{Count}\",\"{student.FIRST_NAME}\",\"{student.ROLL_NO}\",\"{student.PROGRAMME_NAME}\",\"{student.CLASS_LEVEL}\",\"{student.YEAR_OF_PLACEMENT}\",\"{student.COMPANY_NAME}\",\"{student.POSITION_OFFERED}\",\"{student.SALARY_PACKAGE}\",\"{student.REMARKS}\",\"{student.FILE_PATH}\",\"{student.IMG_PATH}\"");
                }
                byte[] byteOrderMarkUtf8 = new byte[] { 0xEF, 0xBB, 0xBF };
                byte[] csvBytes = Encoding.UTF8.GetBytes(csv.ToString());
                byte[] csvWithBom = byteOrderMarkUtf8.Concat(csvBytes).ToArray();
            }
            return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "InterViewPlacedStudentList.csv");
        }
        #endregion

        #region Common method 's
        public string SaveFile(int Count, string sDirectory, string sSaveDirectory)
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty, sFileName = string.Empty, sSavePath = string.Empty;
            var path = string.Empty;
            var sPath = string.Empty;

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
                    var IPFile = sFileName.Split('.');
                    var lastIndex = sFileName.IndexOf(".");
                    if (lastIndex !=-1)
                    {
                        var namePart = sFileName.Substring(0, lastIndex);
                        var extensionPart = sFileName.Substring(lastIndex+1);
                        sPath = namePart + "." + extensionPart;
                        sFilePath = sDirectorypath + sPath;
                        sSavePath = sSaveDirectory + sPath;
                        file.SaveAs(sFilePath);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("PlacementController", "SaveFile", ex.Message);
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
    }
}