using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using CMS.ViewModel.Model;
using System.Data;
using CMS.Models;
using CMS.DAO;
using Newtonsoft.Json;
using CMS.SQL.Examination;
using CMS.SQL.OBE;
using CMS.SQL.Academics;
using System.Threading.Tasks;
using CMS.SQL.SupportData;
using System.Globalization;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using CMS.SQL.Student;
using Rotativa;
using System.Configuration;
using System.Web.Security;
using LinqToExcel;
using System.ComponentModel;
using OfficeOpenXml;
using CMS.SQL.Fee;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Mail;
using Remotion.FunctionalProgramming;
namespace CMS.Controllers
{
    public class OBEController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData objJsonData = new JsonResultData();

        #region OBE Question Paper pattern

        // OBEQuestionPaperpattern
        public ActionResult OBEQuestionPaperPattern()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        //var liExam = liExamSetting.OrderByDescending(X => X.EXAM_SETTING_ID);
                        ObjViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var FechProgrammeType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    if (FechProgrammeType != null && FechProgrammeType.Count > 0)
                        ObjViewModel.ApplicationType = new SelectList(FechProgrammeType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    var FetchDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                    if (FetchDepartment != null && FetchDepartment.Count > 0)
                        ObjViewModel.DepartmentList = new SelectList(FetchDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBEQuestionPaperPattern", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindOBEQuestionPaperPattern(string sExamSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSettingId }, OBESQL.GetOBESQL(OBESQLCommands.BindOBEQuestionPaperPatternByExamSetting), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindOBEQuestionPaperPattern", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchOBEQuestionPaperPatternbyId(string sOBEQuestionPaperPatternId)
        {
            List<OBE_QUESTIONPAPER_PATTERN> LiOBEQuestionPaperPattern = new List<OBE_QUESTIONPAPER_PATTERN>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            if (!string.IsNullOrEmpty(sOBEQuestionPaperPatternId))
            {
                try
                {
                    LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { OBE_QUESTIONPAPERPATTERN_ID = sOBEQuestionPaperPatternId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionPaperPatternByPatternId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "FetchOBEQuestionPaperPatternbyId", ex.Message);
                    }
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(LiOBEQuestionPaperPattern, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOBEQuestionPaperPattern(string sJsonData)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<OBE_QUESTIONPAPER_PATTERN>(sJsonData);
                    if (string.IsNullOrEmpty(Result.OBE_QUESTIONPAPERPATTERN_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(Result, OBESQL.GetOBESQL(OBESQLCommands.InsertOBEQuestionPaperPattern), sAcademicYear);
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(Result, OBESQL.GetOBESQL(OBESQLCommands.UpdateOBEQuestionPaperPattern), sAcademicYear);
                    }
                    ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "SaveOBEQuestionPaperPattern", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "SaveOBEQuestionPaperPattern", "Err.Msg:" + ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteOBEQuestionPaperPattern(string sOBEQuestionPaperPatternId)
        {
            if (!string.IsNullOrEmpty(sOBEQuestionPaperPatternId))
            {
                try
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new OBE_QUESTIONPAPER_PATTERN() { OBE_QUESTIONPAPERPATTERN_ID = sOBEQuestionPaperPatternId }, OBESQL.GetOBESQL(OBESQLCommands.DeleteOBEQuestionPaperPattern));
                    objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "DeleteOBEQuestionPaperPattern", ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  OBE Question Paper pattern Section

        public ActionResult OBEQuestionPaperPatternSection()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var OBEQuestionLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionLevel), sAcademicYear).DataSource.Data;
                    if (OBEQuestionLevel != null && OBEQuestionLevel.Count > 0)
                    {
                        ObjViewModel.OBEQuestionLevel = new SelectList(OBEQuestionLevel, Common.OBE_QUESTION_LEVEL.OBE_QUESTIONLEVEL_ID, Common.OBE_QUESTION_LEVEL.OBE_QUESTIONLEVEL);
                    }
                    var Section = (List<EXAM_SECTION>)CMSHandler.FetchData<EXAM_SECTION>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchExamSections), sAcademicYear).DataSource.Data;
                    if (Section != null && Section.Count > 0)
                    {
                        ObjViewModel.Section = new SelectList(Section, Common.SUP_SECTION.SECTION_ID, Common.SUP_SECTION.SECTION_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBEQuestionPaperPatternSection", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindQuestionPaperPatternSection(string sOBEQuestionPaperPatternId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { OBE_QUESTIONPAPERPATTERN_ID = sOBEQuestionPaperPatternId }, OBESQL.GetOBESQL(OBESQLCommands.BindOBEQuestionPaperPatternSectionByPatternId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindQuestionPaperPatternSection", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public string FetchOBEQuestionPaperPattern(string sExamSettingId)
        {
            string sOption = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var OBEPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSettingId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionpaperPatternByExamsetting), sAcademicYear).DataSource.Data;
            if (OBEPattern != null && OBEPattern.Count > 0)
            {
                foreach (var item in OBEPattern)
                {
                    sOption += "<option value=" + item.OBE_QUESTIONPAPERPATTERN_ID + "  >" + item.OBE_QUESTIONPAPERPATTERN + "</option>";
                }
            }

            return sOption;
        }
        public JsonResult FetchOBEQuestionPaperPatternSectionbyId(string sOBEQuestionPaperPatternSectionId)
        {
            List<OBE_QUESTIONPAPER_PATTERN> LiOBEQuestionPaperPattern = new List<OBE_QUESTIONPAPER_PATTERN>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();
            if (!string.IsNullOrEmpty(sOBEQuestionPaperPatternSectionId))
            {
                try
                {
                    LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { OBE_QUESTIONPAPERPATTERNSECTION_ID = sOBEQuestionPaperPatternSectionId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionPaperPatternSectionByPatternSectionId), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "FetchOBEQuestionPaperPattern", ex.Message);
                    }
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(LiOBEQuestionPaperPattern, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOBEQuestionPaperPatternSection(string sJsonData)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<OBE_QUESTIONPAPER_PATTERN>(sJsonData);
                    if (string.IsNullOrEmpty(Result.OBE_QUESTIONPAPERPATTERNSECTION_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(Result, OBESQL.GetOBESQL(OBESQLCommands.InsertOBEQuestionPatternSection), sAcademicYear);
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(Result, OBESQL.GetOBESQL(OBESQLCommands.UpdateOBEQuestionPatternSection), sAcademicYear);
                    }
                    ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "SaveOBEQuestionPaperPattern", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "SaveOBEQuestionPaperPattern", "Err.Msg:" + ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteOBEQuestionPaperPatternSection(string sOBEQuesPaperPatternSectionId)
        {
            if (!string.IsNullOrEmpty(sOBEQuesPaperPatternSectionId))
            {
                try
                {
                    resultArgs = CMSHandler.SaveOrUpdate(new OBE_QUESTIONPAPER_PATTERN() { OBE_QUESTIONPAPERPATTERNSECTION_ID = sOBEQuesPaperPatternSectionId }, OBESQL.GetOBESQL(OBESQLCommands.DeleteOBEQuestionPaperPatternSection));
                    objJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "DeleteOBEQuestionPaperPattern", ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OBE Question Paper Structure
        public ActionResult OBEQuestionPaperPatternStrucuture()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var OBEQuestionLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionLevel), sAcademicYear).DataSource.Data;
                    if (OBEQuestionLevel != null && OBEQuestionLevel.Count > 0)
                    {
                        ObjViewModel.OBEQuestionLevel = new SelectList(OBEQuestionLevel, Common.OBE_QUESTION_LEVEL.OBE_QUESTIONLEVEL_ID, Common.OBE_QUESTION_LEVEL.OBE_QUESTIONLEVEL);
                    }
                    var Section = (List<EXAM_SECTION>)CMSHandler.FetchData<EXAM_SECTION>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchExamSections), sAcademicYear).DataSource.Data;
                    if (Section != null && Section.Count > 0)
                    {
                        ObjViewModel.Section = new SelectList(Section, Common.SUP_SECTION.SECTION_ID, Common.SUP_SECTION.SECTION_NAME);
                    }
                    var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                    if (OBECOComponent != null && OBECOComponent.Count > 0)
                    {
                        ObjViewModel.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBEQuestionPaperPatternStrucuture", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindQuestionPaperPatternStructure(string sOBEQuestionPaperPatternId, string sOBECOComponent)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { OBE_QUESTIONPAPERPATTERN_ID = sOBEQuestionPaperPatternId, QUESTION_LEVEL_ID = sOBECOComponent }, OBESQL.GetOBESQL(OBESQLCommands.BindOBEQuestionPaperPatternStuctureByPatternId), sAcademicYear).DataSource.Data;
                    ObjViewModel.LiOBEUnit = (List<SUP_OBE_UNIT>)CMSHandler.FetchData<SUP_OBE_UNIT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupOBEUnit)).DataSource.Data;
                    ObjViewModel.LiQuesionLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeQuestionLevelrbtLevel)).DataSource.Data;
                    ObjViewModel.LiOBECOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeCOLevel), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindQuestionPaperPatternStructure", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveOrUpdateObeQuestionPaperStructure(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            LI_JSON_OBE objJson = serializer.Deserialize<LI_JSON_OBE>(sJsonData);
            try
            {
                if (objJson.liobeQusPaperStucture != null && objJson.liobeQusPaperStucture.Count > 0)
                {
                    foreach (var item in objJson.liobeQusPaperStucture)
                    {
                        if (item.OBE_QUESTIONPAPERSTRUCTURE_ID == "")
                        {
                            //Insert
                            resultArgs = CMSHandler.SaveOrUpdate(item, OBESQL.GetOBESQL(OBESQLCommands.InsertOBEQuestionPaperStructure));
                            ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }
                        else
                        {
                            //UPDATE
                            resultArgs = CMSHandler.SaveOrUpdate(item, OBESQL.GetOBESQL(OBESQLCommands.UPDATEOBEQuestionPaperStructure));
                            ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Examination", "SaveOrUpdate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region OBE Course Pattern Mapping

        public ActionResult OBECoursePatternMapping()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcYear != null && liAcYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var SemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (SemesterType != null && SemesterType.Count > 0)
                    {
                        ObjViewModel.Semester = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                    if (OBECOComponent != null && OBECOComponent.Count > 0)
                    {
                        ObjViewModel.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBEQuestionPaperPatternStrucuture", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindObeCoursePatternMapping(string sExamSetting, string sAcYear, string sSemester, string sCOComponentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sSql = OBESQL.GetOBESQL(OBESQLCommands.BindOBECoursePatternByExamSettingId).Replace("?SEMESTER_ID", sSemester);
                    ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting, OBE_COMPONENT_ID = sCOComponentId }, sSql, sAcYear).DataSource.Data;
                    ObjViewModel.LiOBEPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionpaperPatternByExamsetting)).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindQuestionPaperPatternStructure", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult ExamSettingByAcademicYear(string sACYear)
        {
            string sOption = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {

                var ResultMonth = DapperHandler.FetchAll<EXAM_SETTING>(OBESQL.GetOBESQL(OBESQLCommands.FetchAcademicYearByExamSetting), new { @ACADEMIC_YEAR = sACYear }, null);
                if (ResultMonth != null && ResultMonth.Count > 0)
                {
                    foreach (var item in ResultMonth)
                    {
                        sOption += "<option value=" + item.EXAM_SETTING_ID + ">" + item.EXAM_NAME + "</option>";
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Examination", "FetchProgrammeByTypeandShift", ex.Message);
                }
            }
            return Json(sOption, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateObeCoursePatternMapping(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            LI_JSON_OBE objJson = serializer.Deserialize<LI_JSON_OBE>(sJsonData);
            try
            {
                if (objJson.OBEPattern != null && objJson.OBEPattern.Count > 0)
                {
                    foreach (var item in objJson.OBEPattern)
                    {
                        if (item.OBE_COURSEPATTERN_ID == "")
                        {
                            //Insert
                            resultArgs = CMSHandler.SaveOrUpdate(item, OBESQL.GetOBESQL(OBESQLCommands.InsertOBECoursePattern));
                            ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }
                        else
                        {
                            //UPDATE
                            resultArgs = CMSHandler.SaveOrUpdate(item, OBESQL.GetOBESQL(OBESQLCommands.UPDATEOBECoursePattern));
                            ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                            ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Examination", "SaveOrUpdate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddOrEditObeCoursePattern()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcYear != null && liAcYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var SemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (SemesterType != null && SemesterType.Count > 0)
                    {
                        ObjViewModel.Semester = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                    if (OBECOComponent != null && OBECOComponent.Count > 0)
                    {
                        ObjViewModel.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                    }
                    var FechProgrammeType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    if (FechProgrammeType != null && FechProgrammeType.Count > 0)
                        ObjViewModel.ApplicationType = new SelectList(FechProgrammeType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "AddOrEditObeCoursePattern", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindEditObeCoursePatternMapping(string sExamSetting, string sAcYear, string sSemester, string sCOComponentId, string sApptype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sSql = OBESQL.GetOBESQL(OBESQLCommands.BindEditOBECoursePatternByExamSettingId).Replace("?SEMESTER_ID", sSemester).Replace(Common.Delimiter.QUS + Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, sApptype);
                    ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting, OBE_COMPONENT_ID = sCOComponentId }, sSql, sAcYear).DataSource.Data;
                    ObjViewModel.LiOBEPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionpaperPatternByExamsetting)).DataSource.Data;

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchInternalAndExternalStaff)).DataSource.Data;
                    if (liStaff != null)
                    {
                        ObjViewModel.StaffList = new SelectList(liStaff.Select(s => new { STAFF_ID = s.STAFF_ID + ":" + s.ROLES, STAFF_NAME = s.STAFF_NAME }), "STAFF_ID", "STAFF_NAME");
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindQuestionPaperPatternStructure", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        #endregion

        #region OBE Student Mark Entry
        public async Task<ActionResult> OBEStudentMarkEntry()
        {

            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        List<ClassList> listClass = new List<ClassList>();
                        DataTable dtClassList = new DataTable();
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        objExamination.dv.Clear();
                        objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                        //dtCourseList = objExamination.FetchCourseList().DataSource.Table;

                        dtClassList = await Task.Run(() => objExamination.FetchClassList((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);



                        if (dtClassList != null && dtClassList.Rows.Count > 0)
                        {
                            listClass = await Task.Run(() => (from DataRow dr in dtClassList.Rows
                                                              select new ClassList()
                                                              {
                                                                  CLASS_ID = dr[Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID].ToString(),
                                                                  CLASS_NAME = dr[Common.CP_CLASSES_2017.CLASS_NAME].ToString()
                                                              }).ToList());
                            objExamination.ClassList = new SelectList(listClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                        }
                        else
                        {
                            //Need to add select set here 
                        }
                        var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                        if (liExamSetting != null && liExamSetting.Count > 0)
                        {
                            objExamination.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                        }
                        var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                        if (OBECOComponent != null && OBECOComponent.Count > 0)
                        {
                            objExamination.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                        }
                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "OBEStudentMarkEntry", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "OBEStudentMarkEntry", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public ActionResult BindOBEQuestionMarkEntry(string sExamSettingId, string sCourseId, string sClassId, string sCOComponentId, string sExamType, string sAcademicYear)
        {
            OBEViewModel ObjViewModel = new OBEViewModel();
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sCourseId) && !string.IsNullOrEmpty(sExamSettingId))
            {
                try
                {
                    if (sExamType == "1")
                    {
                        var ciaVerify = DapperHandler.FetchAll<CP_COURSE_PATTERN>(OBESQL.GetOBESQL(OBESQLCommands.IsCiaVerifiedByCourseId), new CP_COURSE_PATTERN { COURSE_ID = sCourseId }, sAcademicYear);
                        if (ciaVerify != null && ciaVerify.Count > 0)
                        {
                            ObjViewModel.liSemestermark = DapperHandler.FetchAll<SEMESTER_MARKS>(OBESQL.GetOBESQL(OBESQLCommands.FetchObeExternalVerify), new { @STAFF_ID = sUserId, @COURSE_ID = sCourseId, @EXAM_SETTING_ID = sExamSettingId, @CLASS_ID = sClassId }, sAcademicYear);
                            var sPatternId = DapperHandler.FetchAll<OBE_QUESTIONPAPER_PATTERN>(OBESQL.GetOBESQL(OBESQLCommands.FetchPatternIdByExamSettingandComponentId), new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSettingId, OBE_COMPONENT_ID = sCOComponentId, COURSE_ID = sCourseId }, sAcademicYear);
                            ObjViewModel.LiOBEQuestionMarkEntry = (List<OBE_QUESTIONPAPER_MARKENTRY>)CMSHandler.FetchData<OBE_QUESTIONPAPER_MARKENTRY>(new OBE_QUESTIONPAPER_MARKENTRY { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamSettingId, CLASS_ID = sClassId, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionMarkEntry), sAcademicYear).DataSource.Data;
                            ObjViewModel.liCIA_Marks_Info = (List<CIA_MARKS_INFO>)CMSHandler.FetchData<CIA_MARKS_INFO>(new CIA_MARKS_INFO { COURSE_ID = sCourseId, CLASS_ID = sClassId, EXAM_SETTING_ID = sExamSettingId, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchStudentForOBEMarkEntry), sAcademicYear).DataSource.Data;
                            if (sPatternId != null)
                            {
                                ObjViewModel.LiOBEQuestionPaperStructure = (List<OBE_QUESTIONPAPER_STRUCTURE>)CMSHandler.FetchData<OBE_QUESTIONPAPER_STRUCTURE>(new OBE_QUESTIONPAPER_STRUCTURE { EXAM_SETTING_ID = sExamSettingId, OBE_QUESTIONPAPERPATTERN_ID = sPatternId.FirstOrDefault().PATTERN_ID, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEMarkEntryStructureByComponent), sAcademicYear).DataSource.Data;

                            }
                            ObjViewModel.sCoComponent = sCOComponentId;
                            ObjViewModel.LiOBECOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeCOLevel), sAcademicYear).DataSource.Data;
                            return View(ObjViewModel);
                        }
                        else
                        {
                            return Json("Permission declined by Controller of Examination!!!", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ObjViewModel.liSemestermark = DapperHandler.FetchAll<SEMESTER_MARKS>(OBESQL.GetOBESQL(OBESQLCommands.FetchObeExternalVerify), new { @STAFF_ID = sUserId, @COURSE_ID = sCourseId, @EXAM_SETTING_ID = sExamSettingId, @CLASS_ID = sClassId }, sAcademicYear);
                        var sPatternId = DapperHandler.FetchAll<OBE_QUESTIONPAPER_PATTERN>(OBESQL.GetOBESQL(OBESQLCommands.FetchPatternIdByExamSettingandComponentId), new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSettingId, OBE_COMPONENT_ID = sCOComponentId, COURSE_ID = sCourseId }, sAcademicYear);
                        ObjViewModel.LiOBEQuestionMarkEntry = (List<OBE_QUESTIONPAPER_MARKENTRY>)CMSHandler.FetchData<OBE_QUESTIONPAPER_MARKENTRY>(new OBE_QUESTIONPAPER_MARKENTRY { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamSettingId, CLASS_ID = sClassId, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionMarkEntry), sAcademicYear).DataSource.Data;
                        ObjViewModel.liCIA_Marks_Info = (List<CIA_MARKS_INFO>)CMSHandler.FetchData<CIA_MARKS_INFO>(new CIA_MARKS_INFO { COURSE_ID = sCourseId, CLASS_ID = sClassId, EXAM_SETTING_ID = sExamSettingId, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchStudentForOBEMarkEntry), sAcademicYear).DataSource.Data;
                        ObjViewModel.LiOBEQuestionPaperStructure = (List<OBE_QUESTIONPAPER_STRUCTURE>)CMSHandler.FetchData<OBE_QUESTIONPAPER_STRUCTURE>(new OBE_QUESTIONPAPER_STRUCTURE { EXAM_SETTING_ID = sExamSettingId, OBE_QUESTIONPAPERPATTERN_ID = sPatternId.FirstOrDefault().PATTERN_ID, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEMarkEntryStructureByComponent), sAcademicYear).DataSource.Data;
                        ObjViewModel.sCoComponent = sCOComponentId;
                        ObjViewModel.LiOBECOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeCOLevel), sAcademicYear).DataSource.Data;
                        var Dummyverification = DapperHandler.FetchAll<EXAM_TIMETABLE>(OBESQL.GetOBESQL(OBESQLCommands.FetchexamDummyCourseByCourseIdandExamSettingId), new { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamSettingId }).ToList();
                        if (Dummyverification != null && Dummyverification.Count > 0)
                        {
                            ObjViewModel.Correct_Answer = 1;
                        }
                        else
                        {
                            ObjViewModel.Correct_Answer = 0;
                        }
                        return View(ObjViewModel);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "BindOBEQuestionMarkEntry", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "BindOBEQuestionMarkEntry", "Err.Msg:" + ex.Message);
                        return View(ObjViewModel);
                    }
                }
            }
            else
            {
                return Json("Session has Expired..!!!", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult SaveOBEQuestionMarkEntry(string sJsonData, string SJsonAbsentee)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sResult = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sResult = SaveOBEMarkEntry(sJsonData, SJsonAbsentee);
                    ObjJsonData.Message = sResult;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "SaveOBEQuestionMarkEntry", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "SaveOBEQuestionMarkEntry", "Err.Msg:" + ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        private string SaveOBEMarkEntry(string sJsonData, string SJsonAbsentee)
        {
            string sResult = string.Empty;
            bool insertFlag = false;
            bool updateFlag = false;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sJsonData))
            {
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                string sInsertQuery = string.Empty;
                string sInsertQueryCom = string.Empty;
                string sUpdateQuery = string.Empty;
                ResultArgs resultArgs = new ResultArgs();
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                LI_JSON_OBE objJson = serializer.Deserialize<LI_JSON_OBE>(sJsonData);
                LI_JSON_OBE objJsonAbsentee = serializer.Deserialize<LI_JSON_OBE>(SJsonAbsentee);
                if (objJsonAbsentee.OBEAbsentee != null && objJsonAbsentee.OBEAbsentee.Count > 0)
                {
                    sInsertQuery = @"INSERT INTO OBE_ABSENTEE 
                                    (STUDENT_ID,
                                    COURSE_ID,
                                    IS_ABSENT,
                                    OBE_COMPONENT_ID,
                                    EXAM_SETTING_ID,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR)VALUES";
                    sInsertQueryCom = sInsertQuery;
                    foreach (var item in objJsonAbsentee.OBEAbsentee)
                    {
                        if (string.IsNullOrEmpty(item.OBE_ABSENT_ID))
                        {
                            sInsertQuery += "('" + item.STUDENT_ID.Trim() + "','" + item.COURSE_ID.Trim() + "','1','" + item.OBE_COMPONENT_ID.Trim() + "','" + item.EXAM_SETTING_ID.Trim() + "','" + sUserId + "','" + sAcademicYear + "'),";
                            insertFlag = true;
                        }
                        else
                        {
                            sUpdateQuery += @"UPDATE OBE_ABSENTEE SET IS_ABSENT='0' WHERE OBE_ABSENT_ID='" + item.OBE_ABSENT_ID.Trim() + "';";
                            updateFlag = true;
                        }
                    }
                    if (insertFlag)
                    {
                        sInsertQuery = sInsertQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                        sInsertQuery = sInsertQuery.TrimEnd(',');
                        sInsertQuery = sInsertQuery + ";";
                    }
                    using (OBEViewModel objExamination = new OBEViewModel())
                    {
                        using (MySQLHandler objHandler = new MySQLHandler())
                        {
                            if (updateFlag)
                            {
                                sUpdateQuery = sUpdateQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                                resultArgs = objHandler.ExecuteAsScripts(sUpdateQuery);
                            }
                            if (insertFlag)
                            {
                                resultArgs = objHandler.ExecuteAsScripts(sInsertQuery);
                            }
                        }
                        sResult = resultArgs.Success ? "Marks have been updated successfully...!" : Common.Messages.FailedToSaveRecords;
                    }
                }
                if (objJson.OBEMarkEntry != null && objJson.OBEMarkEntry.Count > 0)
                {
                    sInsertQuery = @"INSERT INTO OBE_MARKENTRY 
                                    (OBE_QUESTIONMARKSTRUCTURE_ID,
                                    STUDENT_ID,
                                    COURSE_ID,
                                    TOTAL_MARK,
                                    SECURED_MARK,
                                    QUESTION_NUMBER,
                                    OBE_COLEVEL_ID,
                                    OBE_COMPONENT_ID,
                                    EXAM_SETTING_ID,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    EXAM_TYPE)VALUES";
                    sInsertQueryCom = sInsertQuery;
                    foreach (var item in objJson.OBEMarkEntry)
                    {
                        if (string.IsNullOrEmpty(item.OBE_MARKENTRY_ID))
                        {
                            sInsertQuery += "('" + item.OBE_QUESTIONMARKSTRUCTURE_ID.Trim() + "','" + item.STUDENT_ID.Trim() + "','" + item.COURSE_ID.Trim() + "','" + item.TOTAL_MARK.Trim() + "','" + item.SECURED_MARK.Trim() + "','" + item.QUESTION_NUMBER.Trim() + "','" + item.OBE_COLEVEL_ID.Trim() + "','" + item.OBE_COMPONENT_ID.Trim() + "','" + item.EXAM_SETTING_ID.Trim() + "','" + sUserId + "','" + sAcademicYear + "'," + item.EXAM_TYPE + "),";
                            insertFlag = true;
                        }
                        else
                        {
                            if (item.SECURED_MARK.Trim() == "")
                            {
                                sUpdateQuery += @"UPDATE OBE_MARKENTRY SET IS_DELETED=1 WHERE OBE_MARKENTRY_ID='" + item.OBE_MARKENTRY_ID.Trim() + "';";
                            }
                            else
                            {
                                sUpdateQuery += @"UPDATE OBE_MARKENTRY SET SECURED_MARK='" + item.SECURED_MARK.Trim() + "',OBE_COLEVEL_ID='" + item.OBE_COLEVEL_ID + "', ENTRY_ID='" + sUserId + "' WHERE OBE_MARKENTRY_ID='" + item.OBE_MARKENTRY_ID.Trim() + "';";
                            }

                            updateFlag = true;
                        }
                    }
                    if (insertFlag)
                    {
                        sInsertQuery = sInsertQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                        sInsertQuery = sInsertQuery.TrimEnd(',');
                        sInsertQuery = sInsertQuery + ";";
                    }
                    using (ExaminationViewModel objExamination = new ExaminationViewModel())
                    {
                        using (MySQLHandler objHandler = new MySQLHandler())
                        {
                            if (updateFlag)
                            {
                                sUpdateQuery = sUpdateQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                                resultArgs = objHandler.ExecuteAsScripts(sUpdateQuery);
                            }
                            if (insertFlag)
                            {
                                resultArgs = objHandler.ExecuteAsScripts(sInsertQuery);
                            }
                        }
                        sResult = resultArgs.Success ? "Marks have been updated successfully...!" : Common.Messages.FailedToSaveRecords;
                    }
                }
            }
            else
            {
                sResult = "Re-enter The Marks";
            }
            return sResult;
        }

        public JsonResult FetchCourseComponentddl(string sCourseId)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                var liCourseInfo = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_COURSE_ROOT() { COURSE_ROOT_ID = sCourseId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseBasicInformationByCourseId)).DataSource.Data;

                if (liCourseInfo != null && liCourseInfo.Count > 0)
                {
                    var CourseComponent = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(new CIA_COMPONENT() { COURSE_TYPE_ID = liCourseInfo.FirstOrDefault().COURSE_TYPE }, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseCIAComponentByCoursetype)).DataSource.Data;
                    if (CourseComponent != null && CourseComponent.Count > 0)
                    {
                        foreach (var item in CourseComponent)
                        {
                            ObjJsonData.sResult += "<option value='" + item.CIA_GROUP_COMPONENT_ID + "' CourseGroupMax ='" + item.MAX_MARK + " 'CourseSemesterId ='" + liCourseInfo.FirstOrDefault().SEMESTER_ID + "' >" + item.COMPONENT + "</option>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("ExaminationController", "FetchCourseComponentddl", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public async Task<string> GetCourseListByClassIdAndStaffId(string sClassId)
        {
            string sOption = string.Empty;
            DataTable dtCourseList = new DataTable();
            DataTable dtClassList = new DataTable();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        //objExamination.dv.Clear();
                        //objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                        //objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.CLASS_ID, sClassId, EnumCommand.DataType.String);
                        //dtCourseList = await Task.Run(() => objExamination.FetchCourseList((Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "").DataSource.Table);
                        var liCourseList = await Task.Run(() => (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CourseWiseStaffInfo() { CLASS_ID = sClassId, STAFF_ID = sUserId },
                            OBESQL.GetOBESQL(OBESQLCommands.FetchCourseList), sAcademicYear).DataSource.Data);
                        if (liCourseList != null && liCourseList.Count > 0)
                        {
                            var liCiaBlockList = await Task.Run(() => (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(null,
                                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCIABlockList), sAcademicYear).DataSource.Data);

                            foreach (var item in liCourseList)
                            {
                                if (liCiaBlockList != null && liCiaBlockList.Count > 0)
                                {
                                    // CourseWise Cia Block List ...
                                    var liCourseWise = liCiaBlockList.Where(x => x.S_COURSE_ID == item.COURSE_ID).ToList();
                                    if (liCourseWise != null && liCourseWise.Count > 0)
                                    {
                                        item.CIA_DISABLE = "disabled";
                                    }
                                    else
                                    {
                                        // Staff And Class And Course Wise Cia Block List ...
                                        var liStaffAndClassWise = liCiaBlockList.Where(x => x.STAFF_ID == sUserId && x.CLASS_ID == sClassId && x.COURSE_ID == item.COURSE_ID).ToList();
                                        if (liStaffAndClassWise != null && liStaffAndClassWise.Count > 0)
                                        {
                                            item.CIA_DISABLE = "disabled";
                                        }
                                    }
                                } // End If Cia Block List ....

                                if (!string.IsNullOrEmpty(item.CIA_DISABLE))
                                    sOption += "<option value=" + item.COURSE_ID + " " + item.CIA_DISABLE + " data-icon='fa fa-ban' class='text-danger'>" + item.COURSE_TITLE + "</option>";
                                else
                                    sOption += "<option value=" + item.COURSE_ID + " " + item.CIA_DISABLE + " >" + item.COURSE_TITLE + " </option>";

                            }// End Loop ....
                        }// End If Course List ....
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "GetCourseListByClassIdAndStaffId", ex.Message);
                        }

                    }
                }
            }
            else
            {
                sOption = "204";

            }


            return sOption;
        }
        public JsonResult SavePushCIAMarkFromComponent(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                string sResult = string.Empty;
                bool insertFlag = false;
                bool updateFlag = false;
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sJsonData))
                {
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    string sInsertQuery = string.Empty;
                    string sInsertQueryCom = string.Empty;
                    string sUpdateQuery = string.Empty;
                    string SsAcademicYear = string.Empty;
                    ResultArgs resultArgs = new ResultArgs();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    JSON_CIA_MARKS objJson = serializer.Deserialize<JSON_CIA_MARKS>(sJsonData);
                    if (objJson.SAVE_CIA_MARKS != null && objJson.SAVE_CIA_MARKS.Count > 0)
                    {

                        sInsertQuery = @"INSERT INTO CIA_GROUP_MARKS_?AC_YEAR (STUDENT_ID, COURSE_ID, COURSE_GROUP_ID, COURSE_GROUP_MARK, SEMESTER_ID, ENTRY_ID) VALUES";
                        sInsertQueryCom = sInsertQuery;
                        foreach (var item in objJson.SAVE_CIA_MARKS)
                        {
                            SsAcademicYear = item.AC_YEAR;
                            var FetchCIAMarks = (List<CIA_MARKS_INFO>)CMSHandler.FetchData<CIA_MARKS_INFO>(item, OBESQL.GetOBESQL(OBESQLCommands.FetchCIAMarkExistByStudentForCIAPush), SsAcademicYear).DataSource.Data;
                            if (FetchCIAMarks != null && FetchCIAMarks.Count > 0)
                            {
                                if (string.IsNullOrEmpty(FetchCIAMarks.FirstOrDefault().CIA_GROUP_MARK_ID))
                                {
                                    sInsertQuery += "('" + item.STUDENT_ID.Trim() + "','" + item.COURSE_ID.Trim() + "','" + item.COURSE_GROUP_ID.Trim() + "','" + item.COURSE_GROUP_MARK.Trim() + "','" + item.SEMESTER_ID.Trim() + "','" + sUserId + "'),";
                                    insertFlag = true;
                                }
                                else
                                {
                                    sUpdateQuery += @"UPDATE CIA_GROUP_MARKS_?AC_YEAR SET COURSE_GROUP_MARK='" + item.COURSE_GROUP_MARK.Trim() + "', ENTRY_ID='" + sUserId + "' WHERE CIA_GROUP_MARK_ID='" + FetchCIAMarks.FirstOrDefault().CIA_GROUP_MARK_ID.Trim() + "';";
                                    updateFlag = true;
                                }
                            }
                            else
                            {
                                sInsertQuery += "('" + item.STUDENT_ID.Trim() + "','" + item.COURSE_ID.Trim() + "','" + item.COURSE_GROUP_ID.Trim() + "','" + item.COURSE_GROUP_MARK.Trim() + "','" + item.SEMESTER_ID.Trim() + "','" + sUserId + "'),";
                                insertFlag = true;
                            }

                        }
                        using (ExaminationViewModel objExamination = new ExaminationViewModel())
                        {
                            if (updateFlag)
                            {
                                sUpdateQuery = sUpdateQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, SsAcademicYear);
                                resultArgs = objExamination.ExcuteCIA_Marks(sUpdateQuery);

                            }
                            if (insertFlag)
                            {
                                sInsertQuery = sInsertQuery.TrimEnd(',');
                                sInsertQuery = sInsertQuery + ";";
                                sInsertQuery = sInsertQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, SsAcademicYear);
                                resultArgs = objExamination.ExcuteCIA_Marks(sInsertQuery);
                            }
                            ObjJsonData.Message = resultArgs.Success ? "Marks have Pushed Successfully...!!!" : Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("ExaminationController", "SavePushCIAMarkFromComponent", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OBE Student Mark Entry For Coe

        public async Task<ActionResult> OBEStudentMarkEntryForCoe()
        {

            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        List<ClassList> listClass = new List<ClassList>();
                        DataTable dtClassList = new DataTable();
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        objExamination.dv.Clear();
                        objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                        //dtCourseList = objExamination.FetchCourseList().DataSource.Table;

                        var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasswithCode)).DataSource.Data;
                        if (liClass != null && liClass.Count > 0)
                        {
                            objExamination.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_CODE);
                        }
                        var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                        if (liExamSetting != null && liExamSetting.Count > 0)
                        {
                            objExamination.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                        }
                        var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                        if (OBECOComponent != null && OBECOComponent.Count > 0)
                        {
                            objExamination.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                        }
                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "OBEStudentMarkEntryForCoe", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "OBEStudentMarkEntryForCoe", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }

        }
        public JsonResult FetchCourseByClassIdAndSemTypeId(string sClassId, string sSemesterTypeId, string sAcYear)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                sAcademicYear = (!string.IsNullOrEmpty(sAcYear)) ? sAcYear : sAcademicYear;
                try
                {
                    string sSemesterId = string.Empty;
                    if (sSemesterTypeId == "1")
                    {
                        sSemesterId = "1,3,5";
                    }
                    else if (sSemesterTypeId == "2")
                    {
                        sSemesterId = "2,4,6";
                    }
                    else if (sSemesterTypeId == "3")
                    {
                        sSemesterId = "1";
                    }
                    var ObjModel = new CourseWiseStaffList();
                    ObjModel.CLASS_ID = sClassId;
                    ObjModel.SEMESTER_TYPE_ID = sSemesterTypeId;
                    ObjModel.SEMESTER_ID = sSemesterId;

                    ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                    var liOptionalCourse = (List<CourseInfo>)CMSHandler.FetchData<CourseInfo>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByOptionalClassId), sAcademicYear).DataSource.Data;
                    if (liOptionalCourse != null && liOptionalCourse.Count > 0)
                    {
                        foreach (var item in liOptionalCourse)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'>( " + item.COURSE_CODE + " ) - " + item.COURSE_TITLE + "</option>";
                        }
                        objJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }

                    string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassIdAndSemesterId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sSemesterId);
                    var liCourseInfo = (List<CourseInfo>)CMSHandler.FetchData<CourseInfo>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                    if (liCourseInfo != null && liCourseInfo.Count > 0)
                    {
                        foreach (var item in liCourseInfo)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'>( " + item.COURSE_CODE + " ) - " + item.COURSE_TITLE + "</option>";
                        }
                        objJsonData.ErrorCode = Common.ErrorCode.Ok;
                    }

                    if (liOptionalCourse == null && liCourseInfo == null)
                    {
                        ObjJsonData.Message = Common.ErrorMessage.NotFound;
                        objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "FetchCourseByClassIdAndSemTypeId", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    objJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveObeExternalMark(string sJsonData)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                string sResult = string.Empty;
                bool insertFlag = false;
                bool updateFlag = false;
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sJsonData))
                {
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    string sInsertQuery = string.Empty;
                    string sInsertQueryCom = string.Empty;
                    string sUpdateQuery = string.Empty;
                    string SsAcademicYear = string.Empty;
                    string sSql = string.Empty;
                    ResultArgs resultArgs = new ResultArgs();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    JSON_CIA_MARKS objJson = serializer.Deserialize<JSON_CIA_MARKS>(sJsonData);
                    if (objJson.SAVE_CIA_MARKS != null && objJson.SAVE_CIA_MARKS.Count > 0)
                    {
                        var LiCourseDetails = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseBasicInformationByCourseId), new CP_COURSE_ROOT { COURSE_ROOT_ID = objJson.SAVE_CIA_MARKS.FirstOrDefault().COURSE_ID });
                        decimal nConvertedMark = Convert.ToInt32(LiCourseDetails.FirstOrDefault().EXTERNAL);
                        sSql = @"INSERT INTO OBE_EXAM_MARK_ENTERED (STAFF_ID,CLASS_ID, COURSE_ID, SEMESTER, EXAM_SETTING_ID, ENTRY_ID, ACADEMIC_YEAR) VALUES";
                        if (sSql != "")
                        {
                            sSql += "(" + sUserId + "," + objJson.SAVE_CIA_MARKS.FirstOrDefault().CLASS_ID + "," + objJson.SAVE_CIA_MARKS.FirstOrDefault().COURSE_ID + ",'" + LiCourseDetails.FirstOrDefault().SEMESTER_ID + "'," + objJson.SAVE_CIA_MARKS.FirstOrDefault().EXAM_SETTING_ID + "," + sUserId + "," + objJson.SAVE_CIA_MARKS.FirstOrDefault().AC_YEAR + ");";
                            using (ExaminationViewModel Obj = new ExaminationViewModel())
                            {
                                resultArgs = Obj.ExcuteCIA_Marks(sSql);
                            }
                        }

                        sInsertQuery = @"INSERT INTO EXTERNAL_MARK (STUDENT_ID, COURSE_ID, EXTERNAL_MARK, ENTRY_MARK, SEMESTER, EXAM_SETTING_ID, ENTRY_ID, ACADEMIC_YEAR) VALUES";
                        sInsertQueryCom = sInsertQuery;
                        foreach (var item in objJson.SAVE_CIA_MARKS)
                        {
                            string sExternalMark = string.Empty;
                            if (item.COURSE_GROUP_MARK == "AA")
                            {
                                sExternalMark = "AA";
                            }
                            else
                            {
                                decimal nExternalMark = Convert.ToDecimal(item.COURSE_GROUP_MARK);
                                nExternalMark = Math.Round((nExternalMark / 100) * nConvertedMark, 2);

                                int roundedResult = (int)Math.Round(nExternalMark, MidpointRounding.AwayFromZero);
                                sExternalMark = roundedResult.ToString();
                            }
                            SsAcademicYear = item.AC_YEAR;
                            var FetchExternalMarks = (List<SEMESTER_MARKS>)CMSHandler.FetchData<SEMESTER_MARKS>(item, OBESQL.GetOBESQL(OBESQLCommands.FetchExternalMarkExistByStudent), SsAcademicYear).DataSource.Data;
                            item.SEMESTER_ID = LiCourseDetails.FirstOrDefault().SEMESTER_ID;
                            if (FetchExternalMarks != null && FetchExternalMarks.Count > 0)
                            {
                                if (string.IsNullOrEmpty(FetchExternalMarks.FirstOrDefault().EXTERNAL_MARK_ID))
                                {
                                    sInsertQuery += "('" + item.STUDENT_ID.Trim() + "','" + item.COURSE_ID.Trim() + "','" + sExternalMark + "','" + item.COURSE_GROUP_MARK.Trim() + "','" + item.SEMESTER_ID.Trim() + "'," + item.EXAM_SETTING_ID.Trim() + ",'" + sUserId + "','" + item.AC_YEAR.Trim() + "'),";
                                    insertFlag = true;
                                }
                                else
                                {
                                    sUpdateQuery += @"UPDATE EXTERNAL_MARK SET EXTERNAL_MARK='" + sExternalMark + "', ENTRY_ID='" + sUserId + "',ENTRY_MARK='" + item.COURSE_GROUP_MARK.Trim() + "' WHERE EXTERNAL_MARK_ID='" + FetchExternalMarks.FirstOrDefault().EXTERNAL_MARK_ID.Trim() + "';";
                                    updateFlag = true;
                                }
                            }
                            else
                            {
                                sInsertQuery += "('" + item.STUDENT_ID.Trim() + "','" + item.COURSE_ID.Trim() + "','" + sExternalMark + "','" + item.COURSE_GROUP_MARK.Trim() + "','" + item.SEMESTER_ID.Trim() + "'," + item.EXAM_SETTING_ID.Trim() + ",'" + sUserId + "','" + item.AC_YEAR.Trim() + "'),";
                                insertFlag = true;
                            }

                        }
                        using (ExaminationViewModel objExamination = new ExaminationViewModel())
                        {
                            if (updateFlag)
                            {
                                sUpdateQuery = sUpdateQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, SsAcademicYear);
                                resultArgs = objExamination.ExcuteCIA_Marks(sUpdateQuery);

                            }
                            if (insertFlag)
                            {
                                sInsertQuery = sInsertQuery.TrimEnd(',');
                                sInsertQuery = sInsertQuery + ";";
                                sInsertQuery = sInsertQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, SsAcademicYear);
                                resultArgs = objExamination.ExcuteCIA_Marks(sInsertQuery);
                            }
                            ObjJsonData.Message = resultArgs.Success ? "Marks have Pushed Successfully...!!!" : Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("ExaminationController", "SavePushCIAMarkFromComponent", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePushOBEAttainments(string sAcyear, string sExamSettingId, string sCourseId, string sClassId, string sCOComponentId)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                string sResult = string.Empty;
                bool insertFlag = false;
                bool updateFlag = false;

                int threscount = 0;
                string CO_ATTAINMENT_ID = "";


                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sExamSettingId))
                {
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    string sInsertQuery = string.Empty;
                    string sInsertQueryCom = string.Empty;
                    string sUpdateQuery = string.Empty;
                    string SsAcademicYear = string.Empty;
                    ResultArgs resultArgs = new ResultArgs();



                    var liOBEMarkEntry = (List<OBE_QUESTIONPAPER_MARKENTRY>)CMSHandler.FetchData<OBE_QUESTIONPAPER_MARKENTRY>(new OBE_QUESTIONPAPER_MARKENTRY { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamSettingId, CLASS_ID = sClassId, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionPaperMarkEntryById), sAcyear).DataSource.Data;

                    //var liCoursepaperpattern = liOBEMarkEntry.GroupBy(x=>x.);

                    if (liOBEMarkEntry != null && liOBEMarkEntry.Count > 0)
                    {
                        sInsertQuery = @"INSERT INTO OBE_CO_ATTAINMENTS (CLASS_ID, COURSE_ID, EXAM_SETTING_ID, COMPONENT_ID, CO_LEVEL, RBT_LEVEL, NO_OF_STUDENT, THRESSHOLD, ATTAINMENT, POINTSCALE,QUS_NO) VALUES";

                        sInsertQueryCom = sInsertQuery;

                        var grp = liOBEMarkEntry.GroupBy(o => o.QUS_NO).ToList();
                        if (grp != null && grp.Count > 0)
                        {
                            foreach (var Course in grp)
                            {
                                var grpstu = liOBEMarkEntry.Where(o => o.QUESTION_NUMBER == Course.FirstOrDefault().QUS_NO).ToList();
                                int totalstu = grpstu.Count();
                                if (grpstu != null && grpstu.Count > 0)
                                {
                                    threscount = 0;
                                    foreach (var stu in grpstu)
                                    {
                                        decimal secmark = Convert.ToDecimal(stu.SECURED_MARK);
                                        decimal maxmark = 0;
                                        //if (stu.TOTAL_MARK == "2")
                                        //{
                                        //    maxmark = 1;
                                        //}
                                        //else if (stu.TOTAL_MARK == "7")
                                        //{
                                        //    maxmark = 3;
                                        //}
                                        //else if (stu.TOTAL_MARK == "15")
                                        //{
                                        //    maxmark = 7;
                                        //}
                                        //else
                                        //{
                                        //    maxmark = Convert.ToDecimal(stu.TOTAL_MARK) * Convert.ToDecimal(0.5);
                                        //}
                                        if (stu.PROGRAMME_LEVEL_ID == "1")
                                        {
                                            maxmark = Convert.ToDecimal(stu.TOTAL_MARK) * Convert.ToDecimal(0.4);
                                        }
                                        else
                                        {
                                            maxmark = Convert.ToDecimal(stu.TOTAL_MARK) * Convert.ToDecimal(0.5);
                                        }


                                        if (Convert.ToDecimal(secmark) >= Convert.ToDecimal(maxmark))
                                        {
                                            threscount++;
                                        }

                                    }
                                    //attain calculation
                                    double attain = Convert.ToDouble(threscount) / Convert.ToDouble(totalstu) * Convert.ToDouble(100);
                                    attain = Math.Round(attain, 2);

                                    //pointscale calculation
                                    double poinscale = Convert.ToDouble(attain) * Convert.ToDouble(0.03);
                                    poinscale = Math.Round(poinscale, 2);


                                    var IsExist = (List<OBE_CO_ATTAINMENTS>)CMSHandler.FetchData<OBE_CO_ATTAINMENTS>(new OBE_CO_ATTAINMENTS() { CLASS_ID = Course.FirstOrDefault().CLASS_ID, COURSE_ID = Course.FirstOrDefault().COURSE_ID, EXAM_SETTING_ID = Course.FirstOrDefault().EXAM_SETTING_ID, COMPONENT_ID = Course.FirstOrDefault().OBE_COMPONENT_ID }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECoAttainmentsById), null).DataSource.Data;
                                    if (IsExist != null && IsExist.Count > 0)
                                    {
                                        string ids = string.Join(",", IsExist.Select(x => x.CO_ATTAINMENT_ID).ToArray());
                                        sUpdateQuery = @"DELETE FROM OBE_CO_ATTAINMENTS WHERE CO_ATTAINMENT_ID IN (" + ids + ");";
                                        updateFlag = true;
                                    }
                                    if (string.IsNullOrEmpty(CO_ATTAINMENT_ID))
                                    {

                                        sInsertQuery += "('" + Course.FirstOrDefault().CLASS_ID + "','" + Course.FirstOrDefault().COURSE_ID + "','" + Course.FirstOrDefault().EXAM_SETTING_ID + "','" + Course.FirstOrDefault().OBE_COMPONENT_ID + "','"
                                           + Course.FirstOrDefault().OBE_COLEVEL_ID + "','" + Course.FirstOrDefault().OBE_RBTLEVEL_ID + "','"
                                           + totalstu + "','" + threscount + "','"
                                           + attain + "','" + poinscale + "','" + Course.FirstOrDefault().QUS_NO + "'),";
                                        insertFlag = true;
                                    }
                                }
                                else
                                {
                                    sInsertQuery += "('" + Course.FirstOrDefault().CLASS_ID + "','" + Course.FirstOrDefault().COURSE_ID + "','" + Course.FirstOrDefault().EXAM_SETTING_ID + "','" + Course.FirstOrDefault().OBE_COMPONENT_ID + "','"
                                           + Course.FirstOrDefault().OBE_COLEVEL_ID + "','" + Course.FirstOrDefault().OBE_RBTLEVEL_ID + "','" + totalstu + "','0','0','0','" + Course.FirstOrDefault().QUS_NO + "'),";
                                    insertFlag = true;
                                }

                            }
                        }
                        else
                        {

                        }

                        if (insertFlag)
                        {
                            sInsertQuery = sInsertQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                            sInsertQuery = sInsertQuery.TrimEnd(',');
                            sInsertQuery = sInsertQuery + ";";
                        }
                        using (ExaminationViewModel objExamination = new ExaminationViewModel())
                        {
                            using (MySQLHandler objHandler = new MySQLHandler())
                            {
                                if (updateFlag)
                                {
                                    sUpdateQuery = sUpdateQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                                    resultArgs = objHandler.ExecuteAsScripts(sUpdateQuery);
                                }
                                if (insertFlag)
                                {
                                    resultArgs = objHandler.ExecuteAsScripts(sInsertQuery);
                                }
                            }
                            ObjJsonData.sResult = resultArgs.Success ? "Attainments Updated successfully...!" : Common.Messages.FailedToSaveRecords;
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("ExaminationController", "SavePushCIAMarkFromComponent", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region enable and disable final Submit

        public ActionResult enableanddisablefinalSubmit()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "enableanddisablefinalSubmit", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "enableanddisablefinalSubmit", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public ActionResult BindFinalSubmitReportForCoe(string sACYear, string sExamSetting)
        {
            OBEViewModel ObjViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sExamSetting))
            {
                try
                {
                    ObjViewModel.liClasscoursestaff = DapperHandler.FetchAll<CP_CLASS_COURSE_STAFF>(OBESQL.GetOBESQL(OBESQLCommands.FetchObeFinalSubmitReportByExamSettingId), new { @EXAM_SETTING_ID = sExamSetting }, sACYear);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "BindFinalSubmitReportForCoe", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "BindFinalSubmitReportForCoe", "Err.Msg:" + ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        public JsonResult UpdateDeletedObeFinalSubmit(string sObeEntryId, string sActive)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            object objResult = new object();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    objResult = DapperHandler.SaveOrUpdate(OBESQL.GetOBESQL(OBESQLCommands.UpdateDeletedFlogForCoe), new { OBE_EXAM_MARK_ENTERED_ID = sObeEntryId, IS_DELETED = sActive }, sAcademicYear);

                    if (objResult != null && Convert.ToInt32(objResult) > 0)
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }

                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "UpdateActive", ex.Message);
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

        #region  OBE Attainment Report
        //OBE Attainment Report
        public ActionResult OBEAttainmentsReport()
        {
            string sAcademicYear = string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            OBEViewModel objExamination = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<ClassList> listClass = new List<ClassList>();
                    DataTable dtClassList = new DataTable();

                    objExamination.dv.Clear();
                    objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                    //dtCourseList = objExamination.FetchCourseList().DataSource.Table;

                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasswithCode)).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        objExamination.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_CODE);
                    }
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        objExamination.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                    if (OBECOComponent != null && OBECOComponent.Count > 0)
                    {
                        objExamination.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                    }
                    var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcYear != null && liAcYear.Count > 0)
                    {
                        objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ExaminationController", "OBEAttainmentsReport", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ExaminationController", "OBEAttainmentsReport", ex.Message);
                    }

                }
            }
            return View(objExamination);
        }
        public ActionResult BindOBEAttainmentsReport(string sAcyear, string sExamSettingId, string sCourseId, string sClassId, string sCOComponentId, string sSemester)
        {
            var ObjViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcyear) && !string.IsNullOrEmpty(sExamSettingId) && !string.IsNullOrEmpty(sCourseId) && !string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sCOComponentId))
            {
                try
                {
                    // fetch LiOBE Marks

                    string sSQL = OBESQL.GetOBESQL(OBESQLCommands.FetchObeMarksListBycourseId)
                        .Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.COURSE_ID, sCourseId)
                        .Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);

                    ObjViewModel.liobemarks = (List<OBE_QUESTIONPAPER_STRUCTURE>)CMSHandler.FetchData<OBE_QUESTIONPAPER_STRUCTURE>(new OBE_QUESTIONPAPER_STRUCTURE() { EXAM_SETTING_ID = sExamSettingId, OBE_COMPONENT_ID = sCOComponentId }, sSQL, sAcyear).DataSource.Data;

                    //Fetch Li CO Attainments
                    string sSQL1 = OBESQL.GetOBESQL(OBESQLCommands.FetchOBECoAttainmentsList)
                       .Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.COURSE_ID, sCourseId)
                       .Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassId);

                    ObjViewModel.LiCOAttainments = (List<OBE_CO_ATTAINMENTS>)CMSHandler.FetchData<OBE_CO_ATTAINMENTS>(new OBE_CO_ATTAINMENTS() { EXAM_SETTING_ID = sExamSettingId, COMPONENT_ID = sCOComponentId }, sSQL1, sAcyear).DataSource.Data;


                    // Fetch College Details ....
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null,
                        OBESQL.GetOBESQL(OBESQLCommands.FetchCollegeDetails), sAcademicYear).DataSource.Data;
                    ObjViewModel.LiOBECOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeCOLevel), sAcyear).DataSource.Data;
                    ObjViewModel.LiOBERBTLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeRBTLevel), sAcyear).DataSource.Data;

                    var sPatternId = DapperHandler.FetchAll<OBE_QUESTIONPAPER_PATTERN>(OBESQL.GetOBESQL(OBESQLCommands.FetchPatternIdByExamSettingandComponentId), new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSettingId, OBE_COMPONENT_ID = sCOComponentId, COURSE_ID = sCourseId }, sAcyear);
                    if (sPatternId != null && sPatternId.Count > 0)
                    {
                        ObjViewModel.LiOBEQuestionPaperStructure = (List<OBE_QUESTIONPAPER_STRUCTURE>)CMSHandler.FetchData<OBE_QUESTIONPAPER_STRUCTURE>(new OBE_QUESTIONPAPER_STRUCTURE { EXAM_SETTING_ID = sExamSettingId, OBE_QUESTIONPAPERPATTERN_ID = sPatternId.FirstOrDefault().PATTERN_ID, OBE_COMPONENT_ID = sCOComponentId, COURSE_ID = sCourseId },
                            OBESQL.GetOBESQL(OBESQLCommands.FetchOBEMarkEntryStructureByComponents), sAcyear).DataSource.Data;
                    }
                    ObjViewModel.sAcyear = sAcyear;
                    ObjViewModel.sSemester = sSemester;
                }

                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindOBEAttainmentsReport", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        #endregion

        #region OBE CLO Attainment
        public ActionResult OBECLOAttainment()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                    if (OBECOComponent != null && OBECOComponent.Count > 0)
                    {
                        ObjViewModel.OBECOComponent = new SelectList(OBECOComponent, Common.OBE_QUESTION_LEVEL.OBE_COMPONENT_ID, Common.OBE_QUESTION_LEVEL.OBE_COMPONENT);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBECLOAttainment", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult OBECLOAttainmentCalcluation(string sExamId, string sComponentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            string sSQL = string.Empty;
            bool insertFlag = false;
            if (!string.IsNullOrEmpty(sExamId) && !string.IsNullOrEmpty(sComponentId))
            {
                try
                {
                    var listInternal = new List<OBE_CLO_ATTAINMENT>();
                    var listExternal = new List<OBE_CLO_ATTAINMENT>();
                    //Fetch CO Level
                    var liCOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeCOLevel), sAcademicYear).DataSource.Data;
                    //Fetch Courses By Exam,Component
                    sSQL = OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOAttainmentsByExamandComponent).Replace(Common.Delimiter.QUS + Common.OBE_CO_ATTAINMENT_COMPONENT.COMPONENT_ID, sComponentId);
                    var liCoursewiseAttainment = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_COATTAINMENTS() { EXAM_SETTING_ID = sExamId }, sSQL, sAcademicYear).DataSource.Data;
                    if (liCoursewiseAttainment != null && liCoursewiseAttainment.Count() > 0)
                    {
                        var licourses = liCoursewiseAttainment.GroupBy(o => o.COURSE_ID).ToList();
                        if (licourses != null && licourses.Count > 0)
                        {
                            foreach (var course in licourses)
                            {
                                string sInsertQuery = @"INSERT INTO OBE_CLO_ATTAINMENT(EXAM_SETTING_ID,
                                                         COURSE_ID,
                                                         CO_LEVEL, 
                                                         EXTERNAL_SEC, 
                                                         EXTERNAL, 
                                                         INTERNAL_SEC, 
                                                         INTERNAL,
                                                         TOTAL_ATTAINMENT, 
                                                         ACADEMIC_YEAR) VALUES";

                                //Internal
                                var liInternal = liCoursewiseAttainment.Where(o => o.COURSE_ID == course.FirstOrDefault().COURSE_ID && o.EXAM_TYPE == Common.EXAM_TYPE.INTERNAL).ToList();
                                if (liInternal != null && liInternal.Count() > 0)
                                {
                                    var groupedco = liInternal.GroupBy(o => o.CO_LEVEL).ToList();
                                    if (groupedco != null && groupedco.Count > 0)
                                    {
                                        foreach (var coitem in groupedco)
                                        {
                                            var licolevel = liInternal.Where(o => o.CO_LEVEL == coitem.FirstOrDefault().CO_LEVEL).ToList();
                                            if (licolevel != null && licolevel.Count() > 0)
                                            {
                                                decimal sumofco = licolevel.Sum(o => Convert.ToDecimal(o.POINTSCALE));
                                                decimal avergeofco = sumofco / licolevel.Count();

                                                listInternal.Add(new OBE_CLO_ATTAINMENT { CO_LEVEL = coitem.FirstOrDefault().CO_LEVEL, AVG = avergeofco.ToString() });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    objJsonData.ErrorCode = Common.ErrorCode.NoContent;
                                    ObjJsonData.Message = "Internal component value no found ...!";
                                }
                                //External
                                var liExternal = liCoursewiseAttainment.Where(o => o.COURSE_ID == course.FirstOrDefault().COURSE_ID && o.EXAM_TYPE == Common.EXAM_TYPE.EXTERNAL).ToList();
                                if (liExternal != null && liExternal.Count() > 0)
                                {
                                    var groupedco = liExternal.GroupBy(o => o.CO_LEVEL).ToList();
                                    if (groupedco != null && groupedco.Count > 0)
                                    {
                                        foreach (var coitem in groupedco)
                                        {
                                            var licolevel = liExternal.Where(o => o.CO_LEVEL == coitem.FirstOrDefault().CO_LEVEL && Convert.ToInt32(o.NO_OF_STUDENT) != 0);
                                            if (licolevel != null && licolevel.Count() > 0)
                                            {
                                                decimal sumofco = licolevel.Sum(o => Convert.ToDecimal(o.POINTSCALE));
                                                decimal avergeofco = sumofco / licolevel.Count();

                                                listExternal.Add(new OBE_CLO_ATTAINMENT { CO_LEVEL = coitem.FirstOrDefault().CO_LEVEL, AVG = avergeofco.ToString() });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    objJsonData.ErrorCode = Common.ErrorCode.NoContent;
                                    ObjJsonData.Message = "External component value no found ...!";
                                }

                                if (liCOLevel != null && liCOLevel.Count() > 0)
                                {
                                    foreach (var coitem in liCOLevel)
                                    {
                                        if (listExternal != null && listExternal.Count() > 0 && listInternal != null && listInternal.Count() > 0)
                                        {
                                            var liexternal = listExternal.Where(o => o.CO_LEVEL == coitem.OBE_COLEVEL_ID).ToList();
                                            var liinternal = listInternal.Where(o => o.CO_LEVEL == coitem.OBE_COLEVEL_ID).ToList();
                                            if (liexternal != null && liexternal.Count > 0 && liinternal != null && liinternal.Count > 0)
                                            {
                                                decimal externalpercent = Convert.ToDecimal(liexternal.FirstOrDefault().AVG) / 100 * 80;
                                                decimal internalpercent = Convert.ToDecimal(liinternal.FirstOrDefault().AVG) / 100 * 20;
                                                decimal totalattainment = externalpercent + internalpercent;

                                                sInsertQuery += "('" + sExamId + "','" + course.FirstOrDefault().COURSE_ID + "','"
                                      + coitem.OBE_COLEVEL_ID + "','" + liexternal.FirstOrDefault().AVG + "','"
                                      + externalpercent + "','" + liinternal.FirstOrDefault().AVG + "','" + internalpercent + "','" + totalattainment + "','" + sAcademicYear + "'),";
                                                insertFlag = true;
                                            }

                                        }
                                        if (listInternal != null && listInternal.Count() > 0 && listExternal == null || listExternal.Count == 0)
                                        {
                                            var liinternal = listInternal.Where(o => o.CO_LEVEL == coitem.OBE_COLEVEL_ID).ToList();
                                            if (liinternal != null && liinternal.Count > 0)
                                            {


                                                decimal internalpercent = Convert.ToDecimal(liinternal.FirstOrDefault().AVG) / 100 * 100;
                                                decimal totalattainment = internalpercent;

                                                sInsertQuery += "('" + sExamId + "','" + course.FirstOrDefault().COURSE_ID + "','"
                                      + coitem.OBE_COLEVEL_ID + "','0','0','" + liinternal.FirstOrDefault().AVG + "','" + internalpercent + "','" + totalattainment + "','" + sAcademicYear + "'),";
                                                insertFlag = true;
                                            }
                                        }
                                        if (listInternal == null || listInternal.Count == 0 && listExternal != null && listExternal.Count > 0)
                                        {
                                            var liexternal = listExternal.Where(o => o.CO_LEVEL == coitem.OBE_COLEVEL_ID).ToList();
                                            if (liexternal != null && liexternal.Count > 0)
                                            {

                                                decimal externalpercent = Convert.ToDecimal(liexternal.FirstOrDefault().AVG) / 100 * 80;
                                                decimal totalattainment = externalpercent;

                                                sInsertQuery += "('" + sExamId + "','" + course.FirstOrDefault().COURSE_ID + "','"
                                      + coitem.OBE_COLEVEL_ID + "','" + liexternal.FirstOrDefault().AVG + "','"
                                      + externalpercent + "','0','0','" + totalattainment + "','" + sAcademicYear + "'),";
                                                insertFlag = true;
                                            }
                                        }
                                    }

                                }

                                using (MySQLHandler objHandler = new MySQLHandler())
                                {

                                    if (insertFlag)
                                    {
                                        //Delete Exist
                                        sSQL = "DELETE FROM OBE_CLO_ATTAINMENT WHERE COURSE_ID='" + course.FirstOrDefault().COURSE_ID + "' AND OBE_CLO_ATTAINMENT_ID!=0;";
                                        ResultArgs resultArgs1 = objHandler.ExecuteAsScripts(sSQL);
                                        sInsertQuery = sInsertQuery.TrimEnd(',');
                                        sInsertQuery = sInsertQuery + ";";
                                        resultArgs = objHandler.ExecuteAsScripts(sInsertQuery);
                                        if (resultArgs.Success)
                                        {
                                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                        }
                                    }
                                }



                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBECLOAttainmentCalcluation", ex.Message);
                    }
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OBE CLO Attainment Report

        public ActionResult OBECLOAttainmentReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var liSemester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester), sAcademicYear).DataSource.Data;
                    if (liSemester != null && liSemester.Count > 0)
                    {
                        ObjViewModel.Semester = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBECLOAttainment", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public string FetchCourseBySemester(string sSemster)
        {
            string sOption = string.Empty;
            ClassList ObjClass = new ClassList();
            List<CourseList> liCourse = new List<CourseList>();
            ObjClass.ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            ObjClass.SEMESTER = sSemster;

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            liCourse = (List<CourseList>)CMSHandler.FetchData<CourseList>(ObjClass, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseBySemester), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
            if (liCourse != null && liCourse.Count > 0)
            {
                foreach (var item in liCourse)
                {
                    sOption += "<option value=" + item.COURSE_ROOT_ID + "  >" + item.COURSE_TITLE + "</option>";
                }
            }

            return sOption;
        }
        public ActionResult BindOBECLOAttainmentReport(string sExamId, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //Fetch CLO Attainment
                    ObjViewModel.liCLOCoursewiseAttainment = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_COATTAINMENTS() { EXAM_SETTING_ID = sExamId, COURSE_ID = sCourseId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseWiseCLOAttainment), sAcademicYear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBECLOAttainment", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        #endregion

        #region CLO PLO Attainment	

        public ActionResult OBECLOPLOAttainment()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                try
                {
                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }
                    var liSemester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester), sAcademicYear).DataSource.Data;
                    if (liSemester != null && liSemester.Count > 0)
                    {
                        ObjViewModel.Semester = new SelectList(liSemester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBECLOAttainment", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult OBECLOPLOAttainmentCalcluation(string sExamId, string sCourseId, string sPotype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            string sSQL = string.Empty;
            string sUpdateQuery = string.Empty;
            string sInsertQuery = string.Empty;
            bool insertFlag = false;
            bool updateFlag = false;
            if (!string.IsNullOrEmpty(sExamId) && !string.IsNullOrEmpty(sCourseId))
            {
                try
                {
                    //Fetch CO Level
                    var liCOLevel = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_CLO_ATTAINMENT() { EXAM_SETTING_ID = sExamId, COURSE_ID = sCourseId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCoLevelByClassId), sAcademicYear).DataSource.Data;

                    //Fetch PO Level
                    var liPOLevel = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_CLO_ATTAINMENT() { EXAM_SETTING_ID = sExamId, COURSE_ID = sCourseId, PO_TYPE = sPotype }, OBESQL.GetOBESQL(OBESQLCommands.FetchPoLevelByClassId), sAcademicYear).DataSource.Data;
                    if (liCOLevel != null && liCOLevel.Count() > 0 && liPOLevel != null && liPOLevel.Count() > 0)
                    {
                        var grpPolevel = liPOLevel.GroupBy(o => o.OBE_POLEVEL_ID).ToList();
                        if (grpPolevel != null && grpPolevel.Count > 0)
                        {
                            sInsertQuery = @"INSERT INTO OBE_CO_PO_ATTAINMENTS (OBE_COLEVEL_ID, OBE_POLEVEL_ID, OBE_CO_PO_MARK, EXAM_SETTING_ID, COURSE_ID, PO_TYPE) VALUES";
                            foreach (var polevel in grpPolevel)
                            {
                                var grpcolevel = liPOLevel.GroupBy(o => o.OBE_COLEVEL_ID).ToList();

                                if (grpcolevel != null && grpcolevel.Count > 0)
                                {
                                    foreach (var colevel in grpcolevel)
                                    {

                                        var Pocomark = liPOLevel.Where(o => o.OBE_COLEVEL_ID == colevel.FirstOrDefault().OBE_COLEVEL_ID && o.OBE_POLEVEL_ID == polevel.FirstOrDefault().OBE_POLEVEL_ID).ToList();
                                        var Coattainment = liCOLevel.Where(o => o.CO_LEVEL == colevel.FirstOrDefault().OBE_COLEVEL_ID).ToList();

                                        if (Pocomark != null && Pocomark.Count > 0 && Coattainment != null && Coattainment.Count > 0)
                                        {
                                            decimal totalattain = Convert.ToDecimal(Coattainment.FirstOrDefault().TOTAL_ATTAINMENT);
                                            decimal pomark = Convert.ToDecimal(Pocomark.FirstOrDefault().OBE_CO_PO_MARK);
                                            decimal total = Convert.ToDecimal(totalattain) * Convert.ToDecimal(pomark) / Convert.ToInt16(3);

                                            var IsExist = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_CLO_ATTAINMENT() { OBE_COLEVEL_ID = colevel.FirstOrDefault().OBE_COLEVEL_ID, EXAM_SETTING_ID = sExamId, PO_TYPE = sPotype, COURSE_ID = sCourseId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCoPoAttainmentsById), null).DataSource.Data;
                                            if (IsExist != null && IsExist.Count > 0)
                                            {
                                                sUpdateQuery += @"UPDATE OBE_CO_PO_ATTAINMENTS SET OBE_COLEVEL_ID='" + colevel.FirstOrDefault().OBE_COLEVEL_ID + "' , OBE_POLEVEL_ID='" + Pocomark.FirstOrDefault().OBE_POLEVEL_ID + "' , OBE_CO_PO_MARK='" + total + "' , EXAM_SETTING_ID='" + sExamId + "' , COURSE_ID='" + sCourseId + "',PO_TYPE='" + sPotype + "' WHERE OBE_CO_PO_ATTAINMENT_ID='" + IsExist.FirstOrDefault().OBE_CO_PO_ATTAINMENT_ID + "';";
                                                updateFlag = true;
                                            }
                                            else
                                            {
                                                sInsertQuery += "('" + colevel.FirstOrDefault().OBE_COLEVEL_ID + "','" + Pocomark.FirstOrDefault().OBE_POLEVEL_ID + "','" + total + "','" + sExamId + "','" + sCourseId + "','" + sPotype + "'),";
                                                insertFlag = true;
                                            }
                                        }
                                        else
                                        {
                                            objJsonData.ErrorCode = Common.ErrorCode.NoContent;
                                            ObjJsonData.Message = "PO Level value no found ...!";
                                        }
                                    }
                                }
                                else
                                {
                                    objJsonData.ErrorCode = Common.ErrorCode.NoContent;
                                    ObjJsonData.Message = "CO Level value no found ...!";
                                }



                            }
                        }
                    }

                    using (ExaminationViewModel objExamination = new ExaminationViewModel())
                    {
                        using (MySQLHandler objHandler = new MySQLHandler())
                        {
                            if (updateFlag)
                            {
                                sUpdateQuery = sUpdateQuery.Replace(Common.Delimiter.QUS + Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
                                resultArgs = objHandler.ExecuteAsScripts(sUpdateQuery);
                            }
                            if (insertFlag)
                            {
                                sInsertQuery = sInsertQuery.TrimEnd(',');
                                sInsertQuery = sInsertQuery + ";";
                                resultArgs = objHandler.ExecuteAsScripts(sInsertQuery);
                            }
                        }
                        ObjJsonData.sResult = resultArgs.Success ? "CO PO Attainments Updated successfully...!" : Common.Messages.FailedToSaveRecords;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "OBECLOPLOAttainmentCalcluation", ex.Message);
                    }
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindCLOPLOAttainment(string sExamId, string sCourseId, string sPotype)
        {
            OBEViewModel ObjViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sExamId) && !string.IsNullOrEmpty(sCourseId))
            {
                try
                {
                    ObjViewModel.liCLOCoursewiseAttainment = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_CLO_ATTAINMENT { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamId, PO_TYPE = sPotype }, OBESQL.GetOBESQL(OBESQLCommands.FetchCoPoAttainmentsByIdandcourseId), null).DataSource.Data;
                    if (ObjViewModel.liCLOCoursewiseAttainment == null)
                    {
                        OBECLOPLOAttainmentCalcluation(sExamId, sCourseId, sPotype);
                        ObjViewModel.liCLOCoursewiseAttainment = (List<OBE_CLO_ATTAINMENT>)CMSHandler.FetchData<OBE_CLO_ATTAINMENT>(new OBE_CLO_ATTAINMENT { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamId, PO_TYPE = sPotype }, OBESQL.GetOBESQL(OBESQLCommands.FetchCoPoAttainmentsByIdandcourseId), null).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "BindCLOPLOAttainment", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "BindCLOPLOAttainment", "Err.Msg:" + ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        #endregion

        #region Obe Calculation  for COE
        public async Task<ActionResult> OBECalculationForCoe()
        {

            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        List<ClassList> listClass = new List<ClassList>();
                        DataTable dtClassList = new DataTable();
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        objExamination.dv.Clear();
                        objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                        //dtCourseList = objExamination.FetchCourseList().DataSource.Table;

                        var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchclassForRegulr)).DataSource.Data;
                        if (liClass != null && liClass.Count > 0)
                        {
                            objExamination.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                        var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                        if (liExamSetting != null && liExamSetting.Count > 0)
                        {
                            objExamination.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                        }
                        var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                        if (OBECOComponent != null && OBECOComponent.Count > 0)
                        {
                            objExamination.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                        }
                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                        var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                        if (liProgramme != null && liProgramme.Count > 0)
                        {
                            objExamination.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                        }
                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }

        }
        public ActionResult BindOBECalculationForCoe(string sExamSettingId, string sCourseId, string sClassId, string sExamType, string sACYear, string sReportType, string sStudentId, string sProgramme)
        {
            OBEViewModel ObjViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sExamSettingId))
            {
                try
                {
                    if (sReportType == "1")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForKLevel).Replace("-- S", "");
                        sSql = sSql.Replace("-- C", "");
                        var listudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(OBESQL.GetOBESQL(OBESQLCommands.FetchStudentDetailsByStudentId), new { STUDENT_ID = sStudentId }, sACYear);
                        var licourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseDetailsByCourseId), new { COURSE_ID = sCourseId }, sACYear);
                        ObjViewModel.UDF1 = "Student Name : " + listudent.FirstOrDefault().FIRST_NAME + " Course Code : " + licourse.FirstOrDefault().COURSE_CODE + " - " + licourse.FirstOrDefault().COURSE_TITLE + "";
                    }
                    else if (sReportType == "2")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForKLevel).Replace("-- S", "");
                        var listudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(OBESQL.GetOBESQL(OBESQLCommands.FetchStudentDetailsByStudentId), new { STUDENT_ID = sStudentId }, sACYear);
                        ObjViewModel.UDF1 = "Student Name : " + listudent.FirstOrDefault().FIRST_NAME + " ";
                    }
                    else if (sReportType == "3")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForKLevel).Replace("-- C", "");
                        var licourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseDetailsByCourseId), new { COURSE_ID = sCourseId }, sACYear);
                        ObjViewModel.UDF1 = "Course Code : " + licourse.FirstOrDefault().COURSE_CODE + " - " + licourse.FirstOrDefault().COURSE_TITLE + "";
                    }
                    else if (sReportType == "4")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForKLevel).Replace("-- C", "");
                        sSql = sSql.Replace("-- G", "");
                        var liClass = DapperHandler.FetchAll<CP_CLASSES>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShiftAndProgrameGroupByClassId), new { CLASS_ID = sClassId }, sACYear);
                        var licourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseDetailsByCourseId), new { COURSE_ID = sCourseId }, sACYear);
                        ObjViewModel.UDF1 = "Class Name : " + liClass.FirstOrDefault().CLASS_CODE + " Course Code : " + licourse.FirstOrDefault().COURSE_CODE + " - " + licourse.FirstOrDefault().COURSE_TITLE + "";
                    }
                    else if (sReportType == "5")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForKLevel).Replace("-- G", "");
                        var liClass = DapperHandler.FetchAll<CP_CLASSES>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShiftAndProgrameGroupByClassId), new { CLASS_ID = sClassId }, sACYear);
                        ObjViewModel.UDF1 = "Class Name : " + liClass.FirstOrDefault().CLASS_CODE + "";
                    }
                    else if (sReportType == "6")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForKLevel).Replace("-- P", "");
                        var liProgramme = DapperHandler.FetchAll<cp_Program_2017>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByProgrammeGroupId), new { PROGRAMME_GROUP_ID = sProgramme }, sACYear);
                        ObjViewModel.UDF1 = "Programme Name : " + liProgramme.FirstOrDefault().PROGRAMME_NAME + "";
                    }
                    ObjViewModel.LiOBEQuestionMarkEntry = DapperHandler.FetchAll<OBE_QUESTIONPAPER_MARKENTRY>(sSql, new { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamSettingId, EXAM_TYPE = sExamType, STUDENT_ID = sStudentId, CLASS_ID = sClassId }, sACYear);
                    ViewBag.likLevel = ObjViewModel.LiOBEQuestionMarkEntry.Select(X => new OBE_QUESTIONPAPER_MARKENTRY { OBE_QUESTIONLEVEL_ID = X.OBE_QUESTIONLEVEL_ID, OBE_RBT_LEVEL = X.OBE_RBT_LEVEL, PERCENTAGE = X.PERCENTAGE, QUESTION_LEVEL = X.QUESTION_LEVEL });

                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "BindFinalSubmitReportForCoe", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "BindFinalSubmitReportForCoe", "Err.Msg:" + ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchCourseDetailsByStudentIdandExamSettingId(string sStudentId, string sExamSettingId, string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourseList = DapperHandler.FetchAll<EXAM_REGISTRATION>(OBESQL.GetOBESQL(OBESQLCommands.FetchExamRegisteredListByStudentId), new { STUDENT_ID = sStudentId, EXAM_SETTING_ID = sExamSettingId }, sAcYear);
                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        foreach (var item in liCourseList)
                        {
                            ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_CODE + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "FetchStudentByClassId", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchStudentByClassId(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFetchStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new ClassInfo() { CLASS_ID = sClassId },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByClassId), sAcademicYear).DataSource.Data;
                    if (liFetchStudent != null && liFetchStudent.Count > 0)
                    {
                        foreach (var item in liFetchStudent)
                        {
                            ObjJsonData.sResult += "<option value=" + item.STUDENT_ID + ">" + item.FIRST_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "FetchStudentByClassId", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Co Level Calculation For Coe

        public async Task<ActionResult> OBECoLevelCalculationForCoe()
        {

            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        List<ClassList> listClass = new List<ClassList>();
                        DataTable dtClassList = new DataTable();
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        objExamination.dv.Clear();
                        objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                        //dtCourseList = objExamination.FetchCourseList().DataSource.Table;

                        var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchclassForRegulr)).DataSource.Data;
                        if (liClass != null && liClass.Count > 0)
                        {
                            objExamination.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                        var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                        if (liExamSetting != null && liExamSetting.Count > 0)
                        {
                            objExamination.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                        }
                        var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                        if (OBECOComponent != null && OBECOComponent.Count > 0)
                        {
                            objExamination.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                        }
                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                        var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                        if (liProgramme != null && liProgramme.Count > 0)
                        {
                            objExamination.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                        }
                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }

        }
        public ActionResult BindOBECoLevelCalculationForCoe(string sExamSettingId, string sCourseId, string sClassId, string sExamType, string sACYear, string sReportType, string sStudentId, string sProgramme)
        {
            OBEViewModel ObjViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sExamSettingId))
            {
                try
                {
                    if (sReportType == "1")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForCOLevel).Replace("-- S", "");
                        sSql = sSql.Replace("-- C", "");
                        var listudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(OBESQL.GetOBESQL(OBESQLCommands.FetchStudentDetailsByStudentId), new { STUDENT_ID = sStudentId }, sACYear);
                        var licourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseDetailsByCourseId), new { COURSE_ID = sCourseId }, sACYear);
                        ObjViewModel.UDF1 = "Student Name : " + listudent.FirstOrDefault().FIRST_NAME + " Course Code : " + licourse.FirstOrDefault().COURSE_CODE + " - " + licourse.FirstOrDefault().COURSE_TITLE + "";
                    }
                    else if (sReportType == "2")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForCOLevel).Replace("-- S", "");
                        var listudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(OBESQL.GetOBESQL(OBESQLCommands.FetchStudentDetailsByStudentId), new { STUDENT_ID = sStudentId }, sACYear);
                        ObjViewModel.UDF1 = "Student Name : " + listudent.FirstOrDefault().FIRST_NAME + " ";
                    }
                    else if (sReportType == "3")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForCOLevel).Replace("-- C", "");
                        var licourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseDetailsByCourseId), new { COURSE_ID = sCourseId }, sACYear);
                        ObjViewModel.UDF1 = "Course Code : " + licourse.FirstOrDefault().COURSE_CODE + " - " + licourse.FirstOrDefault().COURSE_TITLE + "";
                    }
                    else if (sReportType == "4")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForCOLevel).Replace("-- C", "");
                        sSql = sSql.Replace("-- G", "");
                        var liClass = DapperHandler.FetchAll<CP_CLASSES>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShiftAndProgrameGroupByClassId), new { CLASS_ID = sClassId }, sACYear);
                        var licourse = DapperHandler.FetchAll<CP_COURSE_ROOT>(OBESQL.GetOBESQL(OBESQLCommands.FetchCourseDetailsByCourseId), new { COURSE_ID = sCourseId }, sACYear);
                        ObjViewModel.UDF1 = "Class Name : " + liClass.FirstOrDefault().CLASS_CODE + " Course Code : " + licourse.FirstOrDefault().COURSE_CODE + " - " + licourse.FirstOrDefault().COURSE_TITLE + "";
                    }
                    else if (sReportType == "5")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForCOLevel).Replace("-- G", "");
                        var liClass = DapperHandler.FetchAll<CP_CLASSES>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShiftAndProgrameGroupByClassId), new { CLASS_ID = sClassId }, sACYear);
                        ObjViewModel.UDF1 = "Class Name : " + liClass.FirstOrDefault().CLASS_CODE + "";
                    }
                    else if (sReportType == "6")
                    {
                        sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchObeCalvulationForCOLevel).Replace("-- P", "");
                        var liProgramme = DapperHandler.FetchAll<cp_Program_2017>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeByProgrammeGroupId), new { PROGRAMME_GROUP_ID = sProgramme }, sACYear);
                        ObjViewModel.UDF1 = "Programme Name : " + liProgramme.FirstOrDefault().PROGRAMME_NAME + "";
                    }
                    ObjViewModel.LiOBEQuestionMarkEntry = DapperHandler.FetchAll<OBE_QUESTIONPAPER_MARKENTRY>(sSql, new { COURSE_ID = sCourseId, EXAM_SETTING_ID = sExamSettingId, EXAM_TYPE = sExamType, STUDENT_ID = sStudentId, CLASS_ID = sClassId }, sACYear);
                    ViewBag.likLevel = ObjViewModel.LiOBEQuestionMarkEntry.Select(X => new OBE_QUESTIONPAPER_MARKENTRY { OBE_QUESTIONLEVEL_ID = X.OBE_QUESTIONLEVEL_ID, OBE_COLEVEL = X.OBE_COLEVEL, PERCENTAGE = X.PERCENTAGE, QUESTION_LEVEL = X.QUESTION_LEVEL });

                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "BindFinalSubmitReportForCoe", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "BindFinalSubmitReportForCoe", "Err.Msg:" + ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Po and Pso Mapping for Staff
        public ActionResult PoandPsoMapping()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        List<ClassList> listClass = new List<ClassList>();
                        DataTable dtClassList = new DataTable();
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        objExamination.dv.Clear();
                        objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);

                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                        var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                        if (Semester != null && Semester.Count > 0)
                        {
                            objExamination.SemesterList = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                        }

                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public ActionResult BindOBEPoandPso(string sSemesterId, string sACYear, string sCourseId, string sMapping)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            OBEViewModel ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.LiOBECOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeCOLevel), sAcademicYear).DataSource.Data;
                    ObjViewModel.LiOBEPOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobePOLevel), sAcademicYear).DataSource.Data;
                    ObjViewModel.LiOBEPSOLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobePSOLevel), sAcademicYear).DataSource.Data;
                    ObjViewModel.liObeCoPoMapping = (List<OBE_CO_PO_MATRIX>)CMSHandler.FetchData<OBE_CO_PO_MATRIX>(new OBE_CO_PO_MATRIX() { COURSE_ID = sCourseId, PO_TYPE = sMapping },
                        OBESQL.GetOBESQL(OBESQLCommands.Fetchobecopomatrix), sACYear).DataSource.Data;
                    ObjViewModel.UDF1 = sMapping;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "BindOBEPoandPso", ex.Message);
                    }

                }
            }
            else
            {

            }
            return View(ObjViewModel);
        }
        public JsonResult SaveBindOBECOPOMatrix(string sJsonData)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    LI_JSON_OBE objJson = serializer.Deserialize<LI_JSON_OBE>(sJsonData);
                    if (objJson.OBECOPOMatrix != null && objJson.OBECOPOMatrix.Count > 0)
                    {

                        foreach (var vitem in objJson.OBECOPOMatrix)
                        {
                            vitem.ENTRY_ID = sUserId;
                            if (!string.IsNullOrEmpty(vitem.OBE_MAPPING_ID))
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(vitem, OBESQL.GetOBESQL(OBESQLCommands.UpdateOBECOPOMatrix), sAcademicYear);
                            }
                            else
                            {
                                resultArgs = CMSHandler.SaveOrUpdate(vitem, OBESQL.GetOBESQL(OBESQLCommands.InsertOBECOPOMatrix), sAcademicYear);
                            }
                        }

                        ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "SaveBindOBECOPOMatrix", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "SaveBindOBECOPOMatrix", "Err.Msg:" + ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerifyOBECOPOMatrix(string sCourseId, string sMapping)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var checkcourse = DapperHandler.FetchAll<OBE_CO_PO_MATRIX>(OBESQL.GetOBESQL(OBESQLCommands.Fetchobecopomatrix), new { COURSE_ID = sCourseId, PO_TYPE = sMapping }, sAcademicYear);
                    if (checkcourse.Count > 0 && checkcourse != null)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(new OBE_CO_PO_MATRIX { COURSE_ID = sCourseId, PO_TYPE = sMapping, VERIFY_ID = sUserId }, OBESQL.GetOBESQL(OBESQLCommands.UpdateOBEVerification), sAcademicYear);
                        ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "VerifyOBECOPOMatrix", "Err.Msg:" + ex.Message, "Qury is Empty !");
                        objErrorLog.WriteError("ExaminationController", "VerifyOBECOPOMatrix", "Err.Msg:" + ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCourseByAcademicYearandSemester(string sSemesterId, string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = DapperHandler.FetchAll<CP_CLASSES>(OBESQL.GetOBESQL(OBESQLCommands.Fetchdepartmentbystaff), new { STAFF_ID = sUserid }, sAcYear);
                    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new STU_CLASS() { SEMESTER = sSemesterId, DEPARTMENT = lidepartment.FirstOrDefault().DEPARTMENT },
                        OBESQL.GetOBESQL(OBESQLCommands.FetchCourseBySemesterIdandAcademicyearForObe), sAcYear).DataSource.Data;
                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liCourseList)
                        {
                            ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + " cms-sSemesterId=" + item.SEMESTER_ID + "> " + item.COURSE_TITLE + " ( " + item.COURSE_CODE + " ) " + " </option>";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "FetchCourseByAcademicYearandSemester", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Po and Pso Mapping Report For Coe
        public ActionResult PoandPsoMappingReportForCoe()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                using (OBEViewModel objExamination = new OBEViewModel())
                {
                    try
                    {
                        List<ClassList> listClass = new List<ClassList>();
                        DataTable dtClassList = new DataTable();
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        objExamination.dv.Clear();
                        objExamination.dv.Add(Common.CP_CLASS_COURSE_STAFF_2017.STAFF_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);

                        var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                        if (liAcYear != null && liAcYear.Count > 0)
                        {
                            objExamination.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }
                        var FetchDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment)).DataSource.Data;
                        if (FetchDepartment != null && FetchDepartment.Count > 0)
                            objExamination.DepartmentList = new SelectList(FetchDepartment, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);

                    }
                    catch (Exception ex)
                    {

                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("ExaminationController", "OBECalculationForCoe", ex.Message);
                        }
                    }

                    return View(objExamination);
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public ActionResult BindOBEPoandPsoMappingReport(string sSemesterType, string sACYear, string sMapping, string sDepartment)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            OBEViewModel ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSql = OBESQL.GetOBESQL(OBESQLCommands.FetchobecopomatrixEntryStatus).Replace("?SEMESTER_ID", sSemesterType == "1" ? "1,3,5" : "2,4,6");
                    ObjViewModel.liObeCoPoMapping = (List<OBE_CO_PO_MATRIX>)CMSHandler.FetchData<OBE_CO_PO_MATRIX>(new OBE_CO_PO_MATRIX() { SEMESTER_ID = sSemesterType, PO_TYPE = sMapping, DEPARTMENT_ID = sDepartment },
                        sSql, sACYear).DataSource.Data;
                    ObjViewModel.UDF1 = sMapping;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Examination", "BindOBEPoandPso", ex.Message);
                    }

                }
            }
            else
            {

            }
            return View(ObjViewModel);
        }



        #endregion

        #region Blooms Taxonomy Master

        public ActionResult BloomsTaxonomyMaster()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            try
            {
                string createTableSql = @"CREATE TABLE IF NOT EXISTS blooms_taxonomy_master (
                    bloom_id INT AUTO_INCREMENT PRIMARY KEY,
                    k_level VARCHAR(50), 
                    cognitive_process VARCHAR(255),
                    description TEXT,
                    action_verbs TEXT,
                    sample_question_stems TEXT,
                    display_order INT,
                    is_active TINYINT(1) DEFAULT 1,
                    created_by VARCHAR(50),
                    created_date DATETIME,
                    updated_by VARCHAR(50),
                    updated_date DATETIME
                );";
                using (CMS.DAO.MySQLHandler objHandler = new CMS.DAO.MySQLHandler())
                {
                    objHandler.ExecuteCommand(createTableSql, false);
                }
            }
            catch (Exception) { }

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var OBEQuestionLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeQuestionLevelrbtLevel), sAcademicYear).DataSource.Data;
                    if (OBEQuestionLevel != null && OBEQuestionLevel.Count > 0)
                    {
                        ObjViewModel.OBEQuestionLevel = new SelectList(OBEQuestionLevel, Common.OBE_QUESTION_LEVEL.OBE_RBT_LEVEL_ID, Common.OBE_QUESTION_LEVEL.OBE_RBT_LEVEL);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BloomsTaxonomyMaster", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindBloomsTaxonomyMaster()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.LiBloomsTaxonomyMaster = (List<BLOOMS_TAXONOMY_MASTER>)CMSHandler.FetchData<BLOOMS_TAXONOMY_MASTER>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchBloomsTaxonomyMaster), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindBloomsTaxonomyMaster", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchBloomsTaxonomyMasterById(string bloom_id)
        {
            List<BLOOMS_TAXONOMY_MASTER> LiBloomsTaxonomyMaster = new List<BLOOMS_TAXONOMY_MASTER>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var objJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(bloom_id))
            {
                try
                {
                    LiBloomsTaxonomyMaster = (List<BLOOMS_TAXONOMY_MASTER>)CMSHandler.FetchData<BLOOMS_TAXONOMY_MASTER>(new BLOOMS_TAXONOMY_MASTER { bloom_id = bloom_id }, OBESQL.GetOBESQL(OBESQLCommands.FetchBloomsTaxonomyMasterById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "FetchBloomsTaxonomyMasterById", ex.Message);
                    }
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(objJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(LiBloomsTaxonomyMaster, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBloomsTaxonomyMaster(string sJsonData)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<BLOOMS_TAXONOMY_MASTER>(sJsonData);
                    Result.created_by = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0";
                    Result.updated_by = Result.created_by;
                    Result.is_active = "1";

                    if (string.IsNullOrEmpty(Result.bloom_id) || Result.bloom_id == "0")
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(Result, OBESQL.GetOBESQL(OBESQLCommands.InsertBloomsTaxonomyMaster), sAcademicYear);
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(Result, OBESQL.GetOBESQL(OBESQLCommands.UpdateBloomsTaxonomyMaster), sAcademicYear);
                    }
                    ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "SaveBloomsTaxonomyMaster", "Err.Msg:" + ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBloomsTaxonomyMaster(string bloom_id)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            ResultArgs resultArgs = new ResultArgs();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0";
                    resultArgs = CMSHandler.SaveOrUpdate(new BLOOMS_TAXONOMY_MASTER { bloom_id = bloom_id, updated_by = sUserId }, OBESQL.GetOBESQL(OBESQLCommands.DeleteBloomsTaxonomyMaster), sAcademicYear);
                    ObjJsonData.ErrorCode = (resultArgs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = (resultArgs.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("ExaminationController", "DeleteBloomsTaxonomyMaster", "Err.Msg:" + ex.Message);
                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Exam Question Entry

        public ActionResult ExamQuestionEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ExternalUser = "-" + sUserId;
                    // Fetch Exam settings
                    var examSettings = CMSHandler.FetchData<EXAM_SETTING>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchExamSettingForQuestionEntry), sAcademicYear).DataSource?.Data as List<EXAM_SETTING>;
                    if (examSettings != null) ViewBag.ExamSettings = new SelectList(examSettings, "EXAM_SETTING_ID", "EXAM_NAME");

                    // Fetch Courses
                    if (sUserRole != Common.SupRole.EXAM_EXAMINAR)
                    {
                        var courses = CMSHandler.FetchData<CourseList>(new STF_PERSONAL_INFO { STAFF_ID = sUserId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseForQuestionEntrybyStaffID).Replace("?EXAM_SETTING_ID", examSettings.FirstOrDefault().EXAM_SETTING_ID), sAcademicYear).DataSource?.Data as List<CourseList>;
                        if (courses != null) ViewBag.Courses = new SelectList(courses, "COURSE_ID", "COURSE_CODE");
                    }
                    else
                    {
                        var courses = CMSHandler.FetchData<CourseList>(new STF_PERSONAL_INFO { STAFF_ID = sUserId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseForQuestionEntrybyStaffID).Replace("?EXAM_SETTING_ID", examSettings.FirstOrDefault().EXAM_SETTING_ID), sAcademicYear).DataSource?.Data as List<CourseList>;
                        if (courses != null) ViewBag.Courses = new SelectList(courses, "COURSE_ID", "COURSE_CODE");
                    }

                    // Fetch Units
                    var units = CMSHandler.FetchData<SUP_OBE_UNIT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchUnitForQuestionEntry), sAcademicYear).DataSource?.Data as List<SUP_OBE_UNIT>;
                    if (units != null) ViewBag.Units = new SelectList(units, "OBE_UNIT_ID", "OBE_UNIT");

                    // Fetch Question Levels mapping
                    var levels = CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionLevelsForEntry), sAcademicYear).DataSource?.Data as List<OBE_QUESTION_LEVEL>;
                    if (levels != null) ViewBag.Levels = new SelectList(levels, "QUESTION_LEVEL_ID", "QUESTION_LEVEL");

                    // Fetch K Levels for Sidebar
                    var blooms = (List<BLOOMS_TAXONOMY_MASTER>)CMSHandler.FetchData<BLOOMS_TAXONOMY_MASTER>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchBloomsTaxonomyMaster), sAcademicYear).DataSource?.Data;
                    if (blooms != null) ViewBag.BloomsTaxonomy = blooms;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("OBEController", "ExamQuestionEntry", "Err.Msg:" + ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindExamQuestionEntryStructure(string courseId, string examSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var patternResult = CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { COURSE_ID = courseId, EXAM_SETTING_ID = examSettingId }, OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionPatternFromCoursePattern), sAcademicYear).DataSource?.Data as List<OBE_QUESTIONPAPER_PATTERN>;
                    if (patternResult != null && patternResult.Count > 0)
                    {
                        string sOBEQuestionPaperPatternId = patternResult.FirstOrDefault().OBE_QUESTIONPAPERPATTERN_ID;

                        ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { OBE_QUESTIONPAPERPATTERN_ID = sOBEQuestionPaperPatternId, QUESTION_LEVEL_ID = "0" }, OBESQL.GetOBESQL(OBESQLCommands.BindOBEQuestionPaperPatternStuctureByPatternId), sAcademicYear).DataSource.Data;
                        ObjViewModel.LiOBEUnit = (List<SUP_OBE_UNIT>)CMSHandler.FetchData<SUP_OBE_UNIT>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupOBEUnit)).DataSource.Data;
                        ObjViewModel.LiQuesionLevel = (List<OBE_QUESTION_LEVEL>)CMSHandler.FetchData<OBE_QUESTION_LEVEL>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchobeQuestionLevelrbtLevel)).DataSource.Data;

                        var questionsData = CMSHandler.FetchData<OBE_QUESTIONS>(new OBE_QUESTIONS { COURSE_ID = courseId, EXAM_SETTING_ID = examSettingId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionsByCourseAndExam), sAcademicYear).DataSource?.Data as List<OBE_QUESTIONS>;
                        if (questionsData != null)
                        {
                            ObjViewModel.LiOBEQuestions = questionsData;
                        }
                        else
                        {
                            ObjViewModel.LiOBEQuestions = new List<OBE_QUESTIONS>();
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("OBEController", "BindExamQuestionEntryStructure", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        [HttpPost]
        public JsonResult GetFinalizedCourses(string examSettingId)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

                if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
                {
                    var result = CMSHandler.FetchData<OBE_CLAIM_ENTRY>(new OBE_CLAIM_ENTRY { EXAM_SETTING_ID = examSettingId, STAFF_ID = sUserId }, OBESQL.GetOBESQL(OBESQLCommands.FetchFinalizedCoursesByStaff), sAcademicYear);
                    return Json(new { success = true, data = result?.DataSource?.Data ?? new List<OBE_CLAIM_ENTRY>() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Session expired." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "GetFinalizedCourses", ex.Message);
                }
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveCourseFinalStatus(string courseId, string examSettingId)
        {
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? string.Empty;
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId))
                {
                    var data = new { COURSE_ID = courseId, EXAM_SETTING_ID = examSettingId, STAFF_ID = sUserId, ENTRY_ID = sUserId };
                    var resultArgs = CMSHandler.SaveOrUpdate(data, OBESQL.GetOBESQL(OBESQLCommands.InsertCourseFinalStatus), sAcademicYear);
                    return Json(new { success = resultArgs.Success, message = resultArgs.Message });
                }
                return Json(new { success = false, message = "Session expired." });
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "SaveCourseFinalStatus", ex.Message);
                }
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetClaimEntry(string courseId, string examSettingId)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                var result = CMSHandler.FetchData<OBE_CLAIM_ENTRY>(new OBE_CLAIM_ENTRY { COURSE_ID = courseId, EXAM_SETTING_ID = examSettingId, STAFF_ID = sUserId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEClaimEntryByCourseAndStaff), sAcademicYear);

                if (result.Success && result.DataSource.Data != null)
                {
                    var liClaim = (List<OBE_CLAIM_ENTRY>)result.DataSource.Data;
                    if (liClaim.Count > 0)
                    {
                        return Json(new { success = true, data = liClaim[0] }, JsonRequestBehavior.AllowGet);
                    }
                }
                
                // If no claim found, fetch basic course details for auto-fill
                var courseResult = CMSHandler.FetchData<OBE_CLAIM_ENTRY>(new { COURSE_ROOT_ID = courseId }, OBESQL.GetOBESQL(OBESQLCommands.FetchCourseBasicInformationByCourseId));
                if (courseResult.Success && courseResult.DataSource.Data != null)
                {
                    var liCourse = (List<OBE_CLAIM_ENTRY>)courseResult.DataSource.Data;
                    if (liCourse.Count > 0)
                    {
                        var defaultData = liCourse[0];
                        return Json(new { success = false, data = defaultData, message = "No existing claim found. Basic info pre-filled." }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false, message = "No existing claim found." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "GetClaimEntry", ex.Message);
                }
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveClaimEntry(string sJsonData)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                var claim = JsonConvert.DeserializeObject<OBE_CLAIM_ENTRY>(sJsonData);
                if (claim != null)
                {
                    claim.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                    claim.ACADEMIC_YEAR = sAcademicYear;
                    
                    // Map string status to database integers
                    // 1 = Draft, 2 = Submitted
                    claim.SUBMISSION_STATUS = (claim.SUBMISSION_STATUS == "SUBMITTED") ? "2" : "1";

                    if (string.IsNullOrEmpty(claim.CLAIM_ID))
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(claim, OBESQL.GetOBESQL(OBESQLCommands.InsertOBEClaimEntry), "");
                    }
                    else
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(claim, OBESQL.GetOBESQL(OBESQLCommands.UpdateOBEClaimEntry), "");
                    }

                    return Json(new { success = resultArgs.Success, message = resultArgs.Success ? "Claim saved successfully." : "Failed to save claim.", claimId = claim.CLAIM_ID }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Invalid data received." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "SaveClaimEntry", ex.Message);
                }
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OBEClaimReport()
        {
            var ObjViewModel = new OBEViewModel();
            try
            {
                ObjViewModel.LiAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchAcademicYearInfo)).DataSource.Data;
                ObjViewModel.LiExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchExamSettingForQuestionEntry)).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "OBEClaimReport", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        [HttpPost]
        public ActionResult BindOBEClaimReport(string examSettingId, string academicYear)
        {
            var ObjViewModel = new OBEViewModel();
            try
            {
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString() ?? string.Empty;
                string sRole = Session[Common.SESSION_VARIABLES.USER_ROLE_ID]?.ToString() ?? string.Empty;

                string query = OBESQL.GetOBESQL(OBESQLCommands.FetchOBEClaimReport);
                
                // If it's a staff member (Role 4 is usually Faculty/Staff), filter by STAFF_ID
                // Admin/COE (Role 1/2) can see everything
                if (sRole == "4")
                {
                    query = query.Replace("-- S", "");
                }
                
                if (!string.IsNullOrEmpty(examSettingId) && examSettingId != "0")
                {
                    query = query.Replace("-- P", "");
                }

                ObjViewModel.LiOBEClaimEntry = (List<OBE_CLAIM_ENTRY>)CMSHandler.FetchData<OBE_CLAIM_ENTRY>(new OBE_CLAIM_ENTRY { EXAM_SETTING_ID = examSettingId, STAFF_ID = sUserId }, query, academicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "BindOBEClaimReport", ex.Message);
                }
            }
            return PartialView(ObjViewModel);
        }
        public ActionResult PrintClaimForm(string claimId)
        {
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                var result = CMSHandler.FetchData<OBE_CLAIM_ENTRY>(new OBE_CLAIM_ENTRY { CLAIM_ID = claimId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEClaimEntryById), sAcademicYear);

                if (result.Success && result.DataSource.Data != null)
                {
                    var liClaim = (List<OBE_CLAIM_ENTRY>)result.DataSource.Data;
                    if (liClaim.Count > 0)
                    {
                        var claim = liClaim[0];
                        return new Rotativa.ViewAsPdf("_ClaimFormPDF", claim)
                        {
                            FileName = "ClaimForm_" + claim.COURSE_CODE + ".pdf",
                            PageSize = Rotativa.Options.Size.A4,
                            PageOrientation = Rotativa.Options.Orientation.Portrait,
                            PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
                        };
                    }
                }
                return Content("Claim not found.");
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "PrintClaimForm", ex.Message);
                }
                return Content("Error: " + ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetQuestionsForPreview(string courseId, string examSettingId)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                var questions = CMSHandler.FetchData<OBE_QUESTIONS>(
                    new OBE_QUESTIONS { COURSE_ID = courseId, EXAM_ID = examSettingId, EXAM_SETTING_ID = examSettingId },
                    OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionsByCourseAndExam),
                    sAcademicYear).DataSource?.Data as List<OBE_QUESTIONS>;

                if (questions != null && questions.Count > 0)
                {
                    var result = questions.Select(q => new
                    {
                        QUESTION_ID       = q.QUESTION_ID ?? "",
                        QUESTION          = q.QUESTION ?? "",
                        FORMULA_EQUATION  = q.FORMULA_EQUATION ?? "",
                        OBE_UNIT_ID       = q.OBE_UNIT ?? "",
                        QUESTION_LEVEL_ID = q.BRIEF_CAPTION ?? "",
                        SUBMISSION_STATUS = q.SUBMISSION_STATUS ?? ""
                    }).ToList();

                    ObjJsonData.ErrorCode = "1";
                    ObjJsonData.oResult   = result;
                    ObjJsonData.Message   = "Success";
                }
                else
                {
                    ObjJsonData.ErrorCode = "0";
                    ObjJsonData.Message   = "No questions found.";
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog()) { ObjLog.WriteError("OBEController", "GetQuestionsForPreview", ex.Message); }
                ObjJsonData.ErrorCode = "-1";
                ObjJsonData.Message   = ex.Message;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PreviewQuestionPaper(string courseId, string examSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var patternResult = CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { COURSE_ID = courseId, EXAM_SETTING_ID = examSettingId }, OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionPatternFromCoursePattern), sAcademicYear).DataSource?.Data as List<OBE_QUESTIONPAPER_PATTERN>;
                    if (patternResult != null && patternResult.Count > 0)
                    {
                        string sOBEQuestionPaperPatternId = patternResult.FirstOrDefault().OBE_QUESTIONPAPERPATTERN_ID;
                        ObjViewModel.LiOBEQuestionPaperPattern = CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { OBE_QUESTIONPAPERPATTERN_ID = sOBEQuestionPaperPatternId, QUESTION_LEVEL_ID = "0" }, OBESQL.GetOBESQL(OBESQLCommands.BindOBEQuestionPaperPatternStuctureByPatternId), sAcademicYear).DataSource.Data as List<OBE_QUESTIONPAPER_PATTERN>;
                        var questionsData = CMSHandler.FetchData<OBE_QUESTIONS>(new OBE_QUESTIONS { COURSE_ID = courseId, EXAM_ID = examSettingId, EXAM_SETTING_ID = examSettingId }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionsByCourseAndExam), sAcademicYear).DataSource?.Data as List<OBE_QUESTIONS>;
                        ObjViewModel.LiOBEQuestions = questionsData ?? new List<OBE_QUESTIONS>();
                    }
                    var liCollegeDetails = CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource?.Data as List<CollegeDetails>;
                    ViewBag.CollegeDetails = liCollegeDetails?.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog()) { ObjLog.WriteError("OBEController", "PreviewQuestionPaper", ex.Message); }
                }
            }
            return PartialView("_PreviewQuestionPaper", ObjViewModel);
        }

        [HttpPost]
        public JsonResult FetchQuestionPattern(string courseId, string examSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var data = CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new { COURSE_ID = courseId, EXAM_SETTING_ID = examSettingId }, OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionPatternFromCoursePattern), sAcademicYear).DataSource?.Data;
                    ObjJsonData.oResult = data;
                    ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog()) { objErrorLog.WriteError("OBEController", "FetchQuestionPattern", "Err.Msg:" + ex.Message); }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStaffCourseSyllabusList(string examSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sUserId) && !string.IsNullOrEmpty(examSettingId))
            {
                try
                {
                    var data = CMSHandler.FetchData<AcademicSyllabusModel>(new { STAFF_ID = sUserId, EXAM_SETTING_ID = examSettingId, AC_YEAR = sAcademicYear }, OBESQL.GetOBESQL(OBESQLCommands.FetchStaffCoursesWithSyllabus), sAcademicYear).DataSource?.Data;
                    ObjJsonData.oResult = data;
                    ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog()) { objErrorLog.WriteError("OBEController", "GetStaffCourseSyllabusList", "Err.Msg:" + ex.Message); }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveQuestionEntry(string sJsonData, bool IsFinalSubmit)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "0";
                    var objQus = Newtonsoft.Json.JsonConvert.DeserializeObject<OBE_QUESTIONS>(sJsonData);
                    objQus.ENTRY_ID = sUserId;

                    if (IsFinalSubmit)
                    {
                        if (string.IsNullOrEmpty(objQus.QUESTION_ID) || objQus.QUESTION_ID == "0")
                        {
                            objQus.SUBMISSION_STATUS = "FINAL_SUBMITTED";
                            var rs = CMSHandler.SaveOrUpdate(objQus, OBESQL.GetOBESQL(OBESQLCommands.InsertExamQuestionEntry), sAcademicYear, true);
                            if (rs.Success)
                            {
                                objQus.QUESTION_ID = Convert.ToString(rs.RowUniqueId);
                                CMSHandler.SaveOrUpdate(objQus, OBESQL.GetOBESQL(OBESQLCommands.FinalSubmitExamQuestionEntry), sAcademicYear);
                            }
                        }
                        else
                        {
                            var rs = CMSHandler.SaveOrUpdate(objQus, OBESQL.GetOBESQL(OBESQLCommands.UpdateExamQuestionEntry), sAcademicYear);
                            CMSHandler.SaveOrUpdate(objQus, OBESQL.GetOBESQL(OBESQLCommands.FinalSubmitExamQuestionEntry), sAcademicYear);
                        }
                        ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                        ObjJsonData.Message = "Final submission completed successfully. Edit locked.";
                    }
                    else
                    {
                        objQus.SUBMISSION_STATUS = "DRAFT";
                        ResultArgs rs = new ResultArgs();
                        if (string.IsNullOrEmpty(objQus.QUESTION_ID) || objQus.QUESTION_ID == "0")
                        {
                            rs = CMSHandler.SaveOrUpdate(objQus, OBESQL.GetOBESQL(OBESQLCommands.InsertExamQuestionEntry), sAcademicYear, true);
                            if (rs.Success) objQus.QUESTION_ID = Convert.ToString(rs.RowUniqueId);
                        }
                        else
                        {
                            rs = CMSHandler.SaveOrUpdate(objQus, OBESQL.GetOBESQL(OBESQLCommands.UpdateExamQuestionEntry), sAcademicYear);
                        }
                        ObjJsonData.ErrorCode = (rs.Success) ? Common.ErrorCode.Accepted : Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = (rs.Success) ? "Draft saved successfully!" : "Failed to save draft.";
                        ObjJsonData.sResult = objQus.QUESTION_ID; // Return ID for subsequent updates
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog()) { objErrorLog.WriteError("OBEController", "SaveQuestionEntry", "Err.Msg:" + ex.Message); }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FetchQuestionDetailsForPreview(string questionId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var data = CMSHandler.FetchData<OBE_QUESTIONS>(new OBE_QUESTIONS { QUESTION_ID = questionId }, OBESQL.GetOBESQL(OBESQLCommands.FetchExamQuestionEntryForPreview), sAcademicYear).DataSource?.Data;
                    ObjJsonData.oResult = data;
                    ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog()) { objErrorLog.WriteError("OBEController", "FetchQuestionDetailsForPreview", "Err.Msg:" + ex.Message); }
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region obe course pattern with staff mapping 
        public ActionResult OBECoursePatternMappingWithStaffAsign()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcYear != null && liAcYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var SemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (SemesterType != null && SemesterType.Count > 0)
                    {
                        ObjViewModel.Semester = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var OBECOComponent = (List<OBE_CO_ATTAINMENT_COMPONENT>)CMSHandler.FetchData<OBE_CO_ATTAINMENT_COMPONENT>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchOBECOComponent), sAcademicYear).DataSource.Data;
                    if (OBECOComponent != null && OBECOComponent.Count > 0)
                    {
                        ObjViewModel.OBECOComponent = new SelectList(OBECOComponent, "OBE_COMPONENT_ID", "OBE_COMPONENT");
                    }
                    var FechProgrammeType = (List<SUP_APPLICTION_TYPE>)CMSHandler.FetchData<SUP_APPLICTION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType), sAcademicYear).DataSource.Data;
                    if (FechProgrammeType != null && FechProgrammeType.Count > 0)
                        ObjViewModel.ApplicationType = new SelectList(FechProgrammeType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "AddOrEditObeCoursePattern", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindOBECoursePatternMappingWithStaffAsign(string sExamSetting, string sAcYear, string sSemester, string sCOComponentId, string sApptype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sSql = OBESQL.GetOBESQL(OBESQLCommands.BindEditOBECoursePatternByExamSettingId).Replace("?SEMESTER_ID", sSemester).Replace(Common.Delimiter.QUS + Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, sApptype);
                    ObjViewModel.LiOBEQuestionPaperPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting, OBE_COMPONENT_ID = sCOComponentId }, sSql, sAcYear).DataSource.Data;
                    ObjViewModel.LiOBEPattern = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting }, OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionpaperPatternByExamsetting)).DataSource.Data;

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchInternalAndExternalStaff)).DataSource.Data;
                    if (liStaff != null)
                    {
                        ObjViewModel.StaffList = new SelectList(liStaff.Select(s => new { STAFF_ID = s.STAFF_ID + ":" + s.ROLES, STAFF_NAME = s.STAFF_NAME }), "STAFF_ID", "STAFF_NAME");
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("ExaminationController", "BindQuestionPaperPatternStructure", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindStaffWiseCourseList(string sExamSetting, string sAcYear, string sSemester, string sCOComponentId, string sApptype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            List<OBE_QUESTIONPAPER_PATTERN> liStaffWise = new List<OBE_QUESTIONPAPER_PATTERN>();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liStaffWise = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.BindStaffWiseCourseList), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("OBEController", "BindStaffWiseCourseList", ex.Message);
                    }
                }
            }
            return PartialView(liStaffWise);
        }

        [HttpPost]
        public JsonResult SendEmailToStaff(List<string> staffIds, string sExamSetting, string sAcYear, string sCOComponentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var objJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liStaffDetails = (List<OBE_QUESTIONPAPER_PATTERN>)CMSHandler.FetchData<OBE_QUESTIONPAPER_PATTERN>(new OBE_QUESTIONPAPER_PATTERN { EXAM_SETTING_ID = sExamSetting, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.BindStaffWiseCourseList), sAcYear).DataSource.Data;
                    var SelectedStaff = liStaffDetails.Where(x => staffIds.Contains(x.STAFF_ID)).ToList();

                    foreach (var staff in SelectedStaff)
                    {
                        string toEmail = "";
                        STF_PERSONAL_INFO staffInfo = null;
                        List<STF_PERSONAL_INFO> staffInfoList = new List<STF_PERSONAL_INFO>();
                        if (staff.STAFF_ID.Contains("-"))
                        {
                            string sActualId = staff.STAFF_ID.Replace("-", "");
                            staffInfoList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO { STAFF_ID = sActualId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchExternalStaffListById)).DataSource.Data;
                            staffInfo = staffInfoList.FirstOrDefault();
                            if (staffInfo != null)
                            {
                                toEmail = !string.IsNullOrEmpty(staffInfo.GSUITE_MAIL_ID) ? staffInfo.GSUITE_MAIL_ID : staffInfo.EMAIL;
                            }
                        }
                        else
                        {
                            string sActualId = staff.STAFF_ID.Replace("-", "");

                            staffInfoList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new STF_PERSONAL_INFO { STAFF_ID = sActualId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffListById)).DataSource.Data;
                            staffInfo = staffInfoList.FirstOrDefault();
                            if (staffInfo != null)
                            {
                                toEmail = !string.IsNullOrEmpty(staffInfo.GSUITE_MAIL_ID) ? staffInfo.GSUITE_MAIL_ID : staffInfo.EMAIL;
                            }
                        }


                        if (staffInfo != null)
                        {
                            toEmail = !string.IsNullOrEmpty(staffInfo.GSUITE_MAIL_ID) ? staffInfo.GSUITE_MAIL_ID : staffInfo.EMAIL;
                        }
                        if (!string.IsNullOrEmpty(toEmail))
                        {
                            string content = "Dear " + staff.STAFF_NAME + ",<br/><br/>The following courses have been mapped to you for OBE Course Pattern Mapping:<br/><br/>" + staff.COURSE_LIST + "<br/><br/>Regards,<br/>Admin";
                            string fromEmail = "erpsupport@sjctnc.edu.in";

                            string fromPassword = "fcpe zfje ktsl qlpg"; // or App Password should not change this 

                            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(fromEmail, toEmail);
                            message.Subject = "OBE Course Pattern Mapping Notification";
                            message.Body = content;
                            message.IsBodyHtml = true;

                            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                            {
                                client.EnableSsl = true;
                                client.UseDefaultCredentials = false;
                                client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                                client.Send(message);
                            }

                            // Update individual staff status across all their course mappings
                            CMSHandler.SaveOrUpdate(new OBE_QUESTIONPAPER_PATTERN { STAFF_ID = staff.STAFF_ID, EXAM_SETTING_ID = sExamSetting, AC_YEAR = sAcYear, OBE_COMPONENT_ID = sCOComponentId }, OBESQL.GetOBESQL(OBESQLCommands.UpdateStaffEmailStatus));
                        }
                    }

                    // Remove bulk update as we now do it individually for reliability with aggregate strings

                    objJsonData.ErrorCode = Common.ErrorCode.Accepted;
                    objJsonData.Message = "Emails sent successfully!";
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("OBEController", "SendEmailToStaff", ex.Message);
                    }
                    objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    objJsonData.Message = "Internal Server Error while sending emails.";
                }
            }
            else
            {
                objJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                objJsonData.Message = "Session Expired";
            }
            return Json(objJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOrUpdateObeCoursePatternMappingWithStaffAssign(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            LI_JSON_OBE objJson = serializer.Deserialize<LI_JSON_OBE>(sJsonData);
            try
            {
                if (objJson.OBEPattern != null && objJson.OBEPattern.Count > 0)
                {
                    foreach (var item in objJson.OBEPattern)
                    {
                        // 1. Delete existing assignments for this specific course mapping criteria
                        CMSHandler.SaveOrUpdate(new OBE_QUESTIONPAPER_PATTERN
                        {
                            COURSE_ID = item.COURSE_ID,
                            EXAM_SETTING_ID = item.EXAM_SETTING_ID,
                            OBE_COMPONENT_ID = item.OBE_COMPONENT_ID,
                            ACADEMIC_YEAR = sAcademicYear
                        }, OBESQL.GetOBESQL(OBESQLCommands.DeleteOBECoursePattern));

                        // 2. If staff are assigned, insert as individual records
                        if (!string.IsNullOrEmpty(item.STAFF_ID) && !string.IsNullOrEmpty(item.ROLE))
                        {
                            string[] staffIds = item.STAFF_ID.Split(',');
                            string[] roles = item.ROLE.Split(',');

                            for (int i = 0; i < staffIds.Length; i++)
                            {
                                string sId = staffIds[i];
                                string sRole = (i < roles.Length) ? roles[i] : "";

                                // External examiner identification as negative ID
                                if (sRole == "39")
                                {
                                    if (!sId.StartsWith("-")) sId = "-" + sId;
                                }

                                var singleMapping = new OBE_QUESTIONPAPER_PATTERN
                                {
                                    COURSE_ID = item.COURSE_ID,
                                    PATTERN_ID = item.PATTERN_ID,
                                    ACADEMIC_YEAR = sAcademicYear,
                                    EXAM_SETTING_ID = item.EXAM_SETTING_ID,
                                    OBE_COMPONENT_ID = item.OBE_COMPONENT_ID,
                                    STAFF_ID = sId,
                                    ROLE = sRole
                                };

                                var result = CMSHandler.SaveOrUpdate(singleMapping, OBESQL.GetOBESQL(OBESQLCommands.InsertOBECoursePattern));
                                if (result.Success)
                                {
                                    ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                                    ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                            }
                        }
                        else
                        {
                            // If no staff assigned, the delete above already handled clearing the course.
                            ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("Examination", "SaveOrUpdate", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Exam Question Entry Report

        public ActionResult ExamQuestionEntryReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Academic Year list
                    var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcYear != null && liAcYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    // Fetch Exam Settings
                    var examSettings = CMSHandler.FetchData<EXAM_SETTING>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchExamSettingForQuestionEntry), sAcademicYear).DataSource?.Data as List<EXAM_SETTING>;
                    if (examSettings != null && examSettings.Count > 0)
                    {
                        ObjViewModel.ExamSetting = new SelectList(examSettings, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
                    }

                    // Fetch Class list
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasswithCode)).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_CODE);
                    }

                    // Fetch Semester list
                    var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    if (Semester != null && Semester.Count > 0)
                    {
                        ObjViewModel.SemesterList = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objErrorLog = new ErrorLog())
                    {
                        objErrorLog.WriteError("OBEController", "ExamQuestionEntryReport", "Err.Msg:" + ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindExamQuestionReport(string sAcYear, string sSemester, string sStatus)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new OBEViewModel();
            if (!string.IsNullOrEmpty(sAcYear) && !string.IsNullOrEmpty(sStatus))
            {
                try
                {
                    var courseData = DapperHandler.FetchAll<OBEViewModel.ObeCourseEntryStatus>(
                        OBESQL.GetOBESQL(OBESQLCommands.FetchCourseEntryStatusReport),
                        new { AC_YEAR = sAcYear },
                        sAcYear);

                    if (courseData != null)
                    {
                        // Filter by semester(s) if selected — sSemester is comma-separated IDs
                        var filtered = courseData;
                        if (!string.IsNullOrEmpty(sSemester))
                        {
                            var semesterIds = sSemester.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                            if (semesterIds.Count > 0)
                            {
                                filtered = courseData.Where(x => semesterIds.Contains(x.SEMESTER_ID)).ToList();
                            }
                        }

                        // Filter by entry status
                        if (sStatus == "Entered")
                        {
                            ObjViewModel.LiCourseEntryStatus = filtered.Where(x => x.ENTRY_STATUS == "Entered").ToList();
                        }
                        else if (sStatus == "Not Entered")
                        {
                            ObjViewModel.LiCourseEntryStatus = filtered.Where(x => x.ENTRY_STATUS == "Not Entered").ToList();
                        }
                        else
                        {
                            ObjViewModel.LiCourseEntryStatus = filtered;
                        }
                    }
                    else
                    {
                        ObjViewModel.LiCourseEntryStatus = new List<OBEViewModel.ObeCourseEntryStatus>();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("OBEController", "BindExamQuestionReport", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchCourseByClassForReport(string sClassId, string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sClassId))
            {
                try
                {
                    if (string.IsNullOrEmpty(sAcYear)) sAcYear = sAcademicYear;
                    var liCourses = DapperHandler.FetchAll<CourseList>(
                        OBESQL.GetOBESQL(OBESQLCommands.FetchCourseByClassForReport),
                        new { CLASS_ID = sClassId, AC_YEAR = sAcYear },
                        sAcYear);
                    if (liCourses != null && liCourses.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''>-- Select Course --</option>";
                        foreach (var item in liCourses)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ID + "'>" + item.COURSE_TITLE + "</option>";
                        }
                    }
                    else
                    {
                        ObjJsonData.sResult = "<option value=''>-- No Courses Found --</option>";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("OBEController", "FetchCourseByClassForReport", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.sResult = "<option value=''>-- Select Course --</option>";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuestionEntryStatusReport()
        {
            var ObjViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            // Fetch Academic Years
            var liAcYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
            if (liAcYear != null && liAcYear.Count > 0)
            {
                ObjViewModel.AcademicYearList = new SelectList(liAcYear, Common.ACADEMIC_YEAR.AC_YEAR, Common.ACADEMIC_YEAR.AC_YEAR, sAcademicYear);
            }

            // Fetch Exam Settings for the default academic year
            var examSettings = CMSHandler.FetchData<EXAM_SETTING>(null, OBESQL.GetOBESQL(OBESQLCommands.FetchExamSettingForQuestionEntry), sAcademicYear).DataSource?.Data as List<EXAM_SETTING>;
            if (examSettings != null && examSettings.Count > 0)
            {
                ObjViewModel.ExamSetting = new SelectList(examSettings, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);
            }

            return View(ObjViewModel);
        }

        [HttpGet]
        public JsonResult GetQuestionEntryStatusSummary(int examSettingId)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                var liquestion = (List<OBE_QUESTION_ENTRY>)CMSHandler.FetchData<OBE_QUESTION_ENTRY>(new OBE_QUESTION_ENTRY { EXAM_SETTING_ID = examSettingId.ToString() }, OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionEntryStatusSummary)).DataSource.Data;
               
                var summaryList = new List<object>();
                if (liquestion != null && liquestion.Count > 0)
                {
                    foreach (var liItem in liquestion)
                    {
                        summaryList.Add(new
                        {
                            COURSE_ID      = liItem.COURSE_ID?.ToString() ?? "",
                            COURSE_TITLE   = liItem.COURSE_TITLE?.ToString() ?? "",
                            STAFF_NAME     = liItem.STAFF_NAME?.ToString() ?? "",
                            ENTERED_COUNT  = liItem.ENTERED_COUNT?.ToString() ?? "0"
                        });
                    }
                }

                ObjJsonData.ErrorCode = "1";  // 1 = success
                ObjJsonData.oResult = summaryList;
                ObjJsonData.Message = "Success";
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("OBEController", "GetQuestionEntryStatusSummary", ex.Message);
                }
                ObjJsonData.ErrorCode = "0";  // 0 = error
                ObjJsonData.Message = "Error fetching report data: " + ex.Message;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}