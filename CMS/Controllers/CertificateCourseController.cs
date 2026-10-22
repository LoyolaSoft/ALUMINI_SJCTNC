using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.SQL.CertificateCourses;
using CMS.DAO;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CMS.Models;

namespace CMS.Controllers
{
    public class CertificateCourseController : Controller
    {
        // GET: CertificateCourse
        public ActionResult Index()
        {
            return View();
        }

        #region Certificate Course Root
        // List Certificate Course Root ...
        [UserRights("ADMIN")]
        public ActionResult ListCertificateCourses()
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    //fetch setting
                    var liCertificateSetting = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(null, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseSetting), sAcademicYear).DataSource.Data;
                    if (liCertificateSetting != null && liCertificateSetting.Count > 0)
                    {
                        ObjViewModel.SettingList = new SelectList(liCertificateSetting, Common.CERTIFICATE_COURSE_SETTING.SETTING_ID, Common.CERTIFICATE_COURSE_SETTING.SETTING_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCertificateCoursesBySettingId(string sSettingId)
        {
            CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //fetch certificate courses by setting id

                objViewModel.liCourseRoot = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                           new CERTIFICATE_COURSE_ROOT() { SETTING_ID = sSettingId },
                           CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListCertificateCourseBySetting),
                           sAcademicYear).DataSource.Data;
                //var liSemster = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                //if (liSemster != null && liSemster.Count > 0)
                //{
                //    objViewModel.Semester = new SelectList(liSemster, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                //}
            }
            return View(objViewModel);
        }
        // Fetch Certificate Courses By Id ....
        public JsonResult FetchCertificateCourseById(string sCertificateCourseId)
        {
            var liCourseRoot = new List<CERTIFICATE_COURSE_ROOT>();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liCourseRoot = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                        new CERTIFICATE_COURSE_ROOT() { COURSE_ROOT_ID = sCertificateCourseId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchCertificateCourseById", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchCertificateCourseById", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(liCourseRoot, JsonRequestBehavior.AllowGet);
        }
        // Save Certificate Courses ...
        public JsonResult SaveCertificateCourses(string sCertificateCourses)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<CERTIFICATE_COURSE_ROOT>(sCertificateCourses);

                    if (!string.IsNullOrEmpty(Result.COURSE_ROOT_ID))
                    {

                        resultArgs = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateCertificateCourses), sAcademicYear);
                        ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    else
                    {
                        //check if data existed already or  not
                        var liExist = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                            Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsCertificateCourseExist), sAcademicYear).DataSource.Data;
                        if (liExist != null && liExist.Count > 0)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsExistAlready;
                            ObjJsonData.ErrorCode = Common.ErrorCode.AlreadyReported;
                            return Json(ObjJsonData);
                        }



                        resultArgs = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.SaveCertificateCourses), sAcademicYear);
                        ObjJsonData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        ObjJsonData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveCertificateCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveCertificateCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Active DeActive
        public JsonResult ActivateCertificateCourseRoot(string sCourseId, string sIsActive)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ActiveAndDeActive = CMSHandler.SaveOrUpdate(new CERTIFICATE_COURSE_ROOT() { COURSE_ROOT_ID = sCourseId, IS_ACTIVE = sIsActive }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ActivateDeactivateCertificateCourses), sAcademicYear);
                    ObjJsonData.Message = (ActiveAndDeActive.Success) ? "Activation Done Successfully !" : "Failed To Activate !";
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ActivateCertificateClassCourse", "ActivateCertificateCourseRoot", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ActivateCertificateClassCourse", "ActivateCertificateCourseRoot", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        // Delete Certificate Courses By Id ....
        public JsonResult DeleteCertificateCourses(string sCertificateCourseId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new CERTIFICATE_COURSE_ROOT() { COURSE_ROOT_ID = sCertificateCourseId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.DeleteCertificateCourses), sAcademicYear);
                    ObjJsonData.Message = DeleteCourseRoot.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteCertificateCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteCertificateCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Certificate Class Course
        // List Certificate Course Root ....
        [UserRights("ADMIN")]
        public ActionResult ListCertificateClassCourses()
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //ObjViewModel.liClassCourse = (List<CERTIFICATE_CLASS_COURSE>)CMSHandler.FetchData<CERTIFICATE_CLASS_COURSE>(null, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListCertificateClassCourses), sAcademicYear).DataSource.Data;

                    ////fetch class
                    //var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    //if (ClassList != null && ClassList.Count > 0)
                    //{
                    //    ObjViewModel.Class = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    //}


                    //fetch setting
                    var liCertificateSetting = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(null, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseSetting), sAcademicYear).DataSource.Data;
                    if (liCertificateSetting != null && liCertificateSetting.Count > 0)
                    {
                        ObjViewModel.SettingList = new SelectList(liCertificateSetting, Common.CERTIFICATE_COURSE_SETTING.SETTING_ID, Common.CERTIFICATE_COURSE_SETTING.SETTING_NAME);
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateClassCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateClassCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCertificateClassCoursesBySettingId(string sSettingId)
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liClassCourse = (List<CERTIFICATE_CLASS_COURSE>)CMSHandler.FetchData<CERTIFICATE_CLASS_COURSE>(new CERTIFICATE_CLASS_COURSE() { SETTING_ID = sSettingId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListCertificateClassCourses), sAcademicYear).DataSource.Data;

                    //fetch class
                    var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        ObjViewModel.Class = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateClassCourses", "BindCertificateClassCourseBySetting", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateClassCourses", "BindCertificateClassCourseBySetting", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // Fetch Certificate Courses By Id .... 
        public string FetchCertificateCourseBySetting(string sSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            var liClassCourse = new List<CERTIFICATE_COURSE_ROOT>();
            string sOption = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liClassCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                        new CERTIFICATE_COURSE_ROOT() { SETTING_ID = sSettingId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseBySettingId), sAcademicYear).DataSource.Data;
                    if (liClassCourse != null && liClassCourse.Count > 0)
                    {
                        foreach (var item in liClassCourse)
                        {
                            sOption += "<option value=" + item.COURSE_ROOT_ID + "   cms-balance-quota=" + item.BALANCE_QUOTA + ">" + item.COURSE_TITLE + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", ex.Message);
                    }
                }
            }

            return sOption;
        }

        public JsonResult FetchCertificateClassCourseById(string sCertificateClassCourseId)
        {
            var liCourseRoot = new List<CERTIFICATE_CLASS_COURSE>();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liCourseRoot = (List<CERTIFICATE_CLASS_COURSE>)CMSHandler.FetchData<CERTIFICATE_CLASS_COURSE>(
                        new CERTIFICATE_CLASS_COURSE() { CLASS_COURSE_ID = sCertificateClassCourseId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateClassCourseById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchCertificateClassCourseById", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return Json(liCourseRoot, JsonRequestBehavior.AllowGet);
        }
        // Fetch Certificate Courses By Id ....
        public JsonResult ActivateCertificateClassCourse(string sCertificateClassCourseId, string sIsActive)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ActiveAndDeActive = CMSHandler.SaveOrUpdate(new CERTIFICATE_CLASS_COURSE() { CLASS_COURSE_ID = sCertificateClassCourseId, IS_ACTIVE = sIsActive }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ActivateCertificateClassCourse), sAcademicYear);
                    ObjJsonData.Message = (ActiveAndDeActive.Success) ? "Activation Done Successfully !" : "Failed To Activate !";
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ActivateCertificateClassCourse", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ActivateCertificateClassCourse", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Save Certificate Courses ...
        public JsonResult SaveCertificateClassCourses(string SCertificateClassCourses)
        {
            JsonResultData objResultData = new JsonResultData();
            CERTIFICATE_CLASS_COURSE ObjModel = new CERTIFICATE_CLASS_COURSE();
            MySQLHandler objhandler = new MySQLHandler();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<CERTIFICATE_CLASS_COURSE>(SCertificateClassCourses);
                    //ObjModel.SETTINGS_ID = Result.SETTINGS_ID;
                    //ObjModel.IS_ACTIVE = "1";
                    ////objhandler.ExecuteAsScripts("SET SQL_SAFE_UPDATES = 0;");
                    ////var sresultArgs = CMSHandler.SaveOrUpdate(ObjModel, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateIsDeletedBySettingId),sAcademicYear);
                    ////objhandler.ExecuteAsScripts("SET SQL_SAFE_UPDATES = 1;");
                    //var Class_id = (List<CERTIFICATE_COURSE_SETTINGS>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTINGS>(ObjModel, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseSettingById), sAcademicYear).DataSource.Data;
                    //ObjModel.CLASS_ID = Class_id.FirstOrDefault().CLASS_ID;
                    //var CourseId = Result.COURSE_ID.Split(',').ToList();


                    if (!string.IsNullOrEmpty(Result.CLASS_COURSE_ID))
                    {
                        //update
                        var InsertCourseRoot = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateCertificateClassCoursess), sAcademicYear);
                        objResultData.Message = InsertCourseRoot.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = InsertCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                    else
                    {

                        //ObjModel.COURSE_ID = item;
                        var liExist = (List<CERTIFICATE_CLASS_COURSE>)CMSHandler.FetchData<CERTIFICATE_CLASS_COURSE>(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsCertificateClassCourseExist), sAcademicYear).DataSource.Data;
                        if (liExist != null && liExist.Count > 0)
                        {
                            objResultData.Message = Common.Messages.RecordsExistAlready;
                            objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                            return Json(objResultData, JsonRequestBehavior.AllowGet);

                        }

                        var InsertCourseRoot = CMSHandler.SaveOrUpdate(Result, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.SaveCertificateClassCourses), sAcademicYear);
                        objResultData.Message = InsertCourseRoot.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResultData.ErrorCode = InsertCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveCertificateClassCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveCertificateClassCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        // Delete Certificate Courses By Id ....
        public JsonResult DeleteCertificateClassCourses(string sCertificateClassCourseId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new CERTIFICATE_CLASS_COURSE() { CLASS_COURSE_ID = sCertificateClassCourseId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.DeleteCertificateClassCourses), sAcademicYear);
                    ObjJsonData.Message = DeleteCourseRoot.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteCertificateClassCourses", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteCertificateClassCourses", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult BindCoursesBySetting(string sSettingId)
        {
            string sOption = string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            CERTIFICATE_COURSE_SETTINGS objModel = new CERTIFICATE_COURSE_SETTINGS();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.SETTINGS_ID = sSettingId;
                    var FetchClasss = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(objModel, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseBySetting), sAcademicYear).DataSource.Data;
                    if (FetchClasss != null && FetchClasss.Count > 0)
                    {
                        foreach (var item in FetchClasss)
                        {
                            if (!string.IsNullOrEmpty(item.CLASS_NAME))
                            {
                                ObjJsonData.oResult = item.CLASS_NAME;
                            }
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' " + item.SELECTED + " >" + item.COURSE_TITLE + "</option>";
                        }
                    }
                }
                else
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindClassListByProgrammeGroupId", "StudentController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindClassListByProgrammeGroupId", "StudentController", ex.Message);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Certificate Student Registration
        // List Certificate Registration ...
        // [UserRights("ADMIN")]
        public ActionResult ListCertificateCourseStudent()
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                { //fetch setting
                    var liCertificateSetting = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(null, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseSetting), sAcademicYear).DataSource.Data;
                    if (liCertificateSetting != null && liCertificateSetting.Count > 0)
                    {
                        ObjViewModel.SettingList = new SelectList(liCertificateSetting, Common.CERTIFICATE_COURSE_SETTING.SETTING_ID, Common.CERTIFICATE_COURSE_SETTING.SETTING_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListCertificateCourseStudent", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListCertificateCourseStudent", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCertificateCourseStudentList(string sSettingId)
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    // Fetch Student Registration By Setting Id And AcYear ...
                    ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new CERTIFICATE_COURSE_SETTINGS() { SETTING_ID = sSettingId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ListCertificateRegisteredStudent), sAcademicYear).DataSource.Data;

                    // Fetch Classes ....
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.Class = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                    //// Fetch Semester .....
                    //var liSemster = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    //if (liSemster != null && liSemster.Count > 0)
                    //{
                    //    ObjViewModel.Semester = new SelectList(liSemster, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    //}

                    // Fetch Students For DDL ....
                    var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByAcademicYear), sAcademicYear).DataSource.Data;
                    if (liStudent != null && liStudent.Count > 0)
                    {
                        ObjViewModel.Student = new SelectList(liStudent, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                    }

                    // Fetch Courses ....
                    //var FetchCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(null, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCourseDDL), sAcademicYear).DataSource.Data;
                    //if (FetchCourse != null && FetchCourse.Count > 0)
                    //{
                    //    ObjViewModel.Course = new SelectList(FetchCourse, Common.CERTIFICATE_COURSE_ROOT.COURSE_ROOT_ID, Common.CERTIFICATE_COURSE_ROOT.COURSE_TITLE);
                    //}


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("BindCertificateCourseStudentList", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("BindCertificateCourseStudentList", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);

        }
        public JsonResult FetchCertificateCoursesByClassId(string sClassId, string sSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sClassId) && !string.IsNullOrEmpty(sSettingId))
            {
                try
                {
                    var liCourseRoot = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                                    new SUP_CLASS() { CLASS_ID = sClassId, SETTING_ID = sSettingId },
                                CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.LoadCertificateCourses), sAcademicYear).DataSource.Data;
                    if (liCourseRoot != null && liCourseRoot.Count > 0)
                    {
                        foreach (var item in liCourseRoot)
                        {
                            if (item.BALANCE_QUOTA == "0" || int.Parse(item.BALANCE_QUOTA) < 0)
                            {
                                ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' disabled> " + item.COURSE_TITLE + " </option>";
                            }
                            else
                            {
                                ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "'> " + item.COURSE_TITLE + " </option>";
                            }

                        }
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchCertificateCoursesByClassId", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchCertificateCoursesByClassId", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Fetch Class & Course ...
        public JsonResult FetchStudentByClassId(string sClassId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new SUP_CLASS() { CLASS_ID = sClassId }, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchStudentInfoByClassId), sAcademicYear).DataSource.Data;
                    if (liStudent != null && liStudent.Count > 0)
                    {
                        foreach (var item in liStudent)
                        {
                            ObjJsonData.sResult += "<option value='" + item.STUDENT_ID + "'> " + item.FIRST_NAME + " (" + item.ROLL_NO + ") </option>";
                        }
                    }

                    var FetchCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(new SUP_CLASS() { CLASS_ID = sClassId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.LoadCertificateCourses), sAcademicYear).DataSource.Data;
                    if (FetchCourse != null && FetchCourse.Count > 0)
                    {
                        foreach (var item in FetchCourse)
                        {
                            ObjJsonData.ErrorCode += "<option value='" + item.COURSE_ROOT_ID + "'>" + item.COURSE_TITLE + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStudentByClassId", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStudentByClassId", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Student Certificate Course Regisration ...
        [UserRights("STUDENT")]
        public ActionResult CertificateCourseRegistration()
        {
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //Fetch Setting Id....
                string sSettingId = string.Empty;
                var liSetting = (List<CERTIFICATE_COURSE_SETTINGS>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTINGS>(null,
                    SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveCertificateSettingIdByActiveYear), sAcademicYear).DataSource.Data;
                if (liSetting != null && liSetting.Count > 0)
                {
                    sSettingId = liSetting.FirstOrDefault().SETTING_ID;
                }
                // Fetch Registered Course....
                var liCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(new CERTIFICATE_STUDENT_REGISTRATION() { STUDENT_ID = sUserId, SETTING_ID = sSettingId },
                   CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchRegisteredCourse), sAcademicYear).DataSource.Data;
                if (liCourse != null && liCourse.Count > 0)
                {
                    ObjViewModel.sMessage = "You have chosen Certificate Course : " + liCourse.FirstOrDefault().COURSE_TITLE;
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // Fetch Student Registration ...
        public JsonResult FetchStudentRegistration(string sRegistrationId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            var liRegistration = new List<CERTIFICATE_STUDENT_REGISTRATION>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    liRegistration = (List<CERTIFICATE_STUDENT_REGISTRATION>)CMSHandler.FetchData<CERTIFICATE_STUDENT_REGISTRATION>(
                        new CERTIFICATE_STUDENT_REGISTRATION() { REGISTRATION_ID = sRegistrationId },
                        CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchRegistrationById), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStudentRegistration", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStudentRegistration", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(liRegistration, JsonRequestBehavior.AllowGet);
        }
        // Save Certificate Registration ...
        public JsonResult SaveStudentRegisration(string sCourseId, string sClassId, string sSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            var ObjModel = new CERTIFICATE_STUDENT_REGISTRATION();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.CLASS_ID = sClassId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.STUDENT_ID = sUserId;
                    ObjModel.SETTING_ID = sSettingId;
                    var liExist = (List<CERTIFICATE_STUDENT_REGISTRATION>)CMSHandler.FetchData<CERTIFICATE_STUDENT_REGISTRATION>(ObjModel, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsRegistrationExist), sAcademicYear).DataSource.Data;
                    if (liExist != null && liExist.Count > 0)
                    {
                        ObjJsonData.Message = "5";
                    }
                    else
                    {
                        //and check if the course already existed in other setting registration
                        var liCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(new CERTIFICATE_COURSE_ROOT() { COURSE_ROOT_ID = ObjModel.COURSE_ID }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseById), sAcademicYear).DataSource.Data;
                        if (liCourse != null && liCourse.Count > 0)
                        {
                            var liExist1 = (List<CERTIFICATE_STUDENT_REGISTRATION>)CMSHandler.FetchData<CERTIFICATE_STUDENT_REGISTRATION>(new CERTIFICATE_COURSE_ROOT() { COURSE_CODE = liCourse.FirstOrDefault().COURSE_CODE, STUDENT_ID = ObjModel.STUDENT_ID }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsRegistrationExistinSameCourse), sAcademicYear).DataSource.Data;
                            if (liExist1 != null && liExist1.Count > 0)
                            {
                                ObjJsonData.Message = "3";
                            }
                            else
                            {
                                var SaveRegistration = CMSHandler.SaveOrUpdate(ObjModel, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.SaveStudentRegistration), sAcademicYear);
                                ObjJsonData.Message = (SaveRegistration.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                if (SaveRegistration.Success)
                                {
                                    ObjJsonData.Message = "1";
                                }
                                else
                                {
                                    ObjJsonData.Message = "0";
                                }
                            }
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveStudentRegisration", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveStudentRegisration", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = "4";
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Delete Registration ...
        public JsonResult DeleteRegistration(string sRegistrationId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCourseRoot = CMSHandler.SaveOrUpdate(new CERTIFICATE_STUDENT_REGISTRATION() { REGISTRATION_ID = sRegistrationId }, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.DeleteRegistration), sAcademicYear);
                    ObjJsonData.Message = (DeleteCourseRoot.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = DeleteCourseRoot.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteRegistration", "CertificateCoursesController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteRegistration", "CertificateCoursesController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public string ValidateStudent(string sSettingId)
        {
            string sResult = string.Empty;
            CertificateCoursesViewModel ObjViewModel = new CertificateCoursesViewModel();
            CERTIFICATE_STUDENT_REGISTRATION ObjModel = new CERTIFICATE_STUDENT_REGISTRATION();
            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                ObjModel.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                ObjModel.SETTING_ID = sSettingId;
                var ValidateStudent = (List<CERTIFICATE_STUDENT_REGISTRATION>)CMSHandler.FetchData<CERTIFICATE_STUDENT_REGISTRATION>(ObjModel,
                    CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.ValidateStudent), sAcademicYear).DataSource.Data;
                sResult = ValidateStudent.SingleOrDefault().REGISTRATION_ID.ToString();
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ValidateStudent", "NMEController", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ValidateStudent", "NMEController", ex.Message);
                }
            }
            return sResult;

        }
        public JsonResult LoadCertificateCourses()
        {
            ResultArgs resultArgs = new ResultArgs();
            CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
            JsonResultData objResultData = new JsonResultData();
            string Result = string.Empty;
            string sResult = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sSettingId = string.Empty;
                    var liSetting = (List<CERTIFICATE_COURSE_SETTINGS>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTINGS>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveCertificateSettingIdByActiveYear), sAcademicYear).DataSource.Data;
                    if (liSetting != null && liSetting.Count > 0)
                    {
                        sSettingId = liSetting.FirstOrDefault().SETTING_ID;
                    }
                    // Fetch Registered Course ....
                    var liCourse = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(new CERTIFICATE_STUDENT_REGISTRATION() { STUDENT_ID = sUserId, SETTING_ID = sSettingId },
                       CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchRegisteredCourse), sAcademicYear).DataSource.Data;
                    if (liCourse != null && liCourse.Count > 0)
                    {
                        objResultData.Message = "You have chosen Certificate Course : " + liCourse.FirstOrDefault().COURSE_TITLE;
                        objResultData.ErrorCode = Common.ErrorCode.AlreadyReported;
                        return Json(objResultData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string sClassId = string.Empty;
                        var liStuClass = (List<STU_CLASS>)CMSHandler.FetchData<STU_CLASS>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByStudentId), sAcademicYear).DataSource.Data;
                        if (liStuClass != null && liStuClass.Count > 0)
                        {
                            sClassId = liStuClass.FirstOrDefault().CLASS_ID;
                            // Fetch Certificate Available Courses ...
                            var liCourseRoot = (List<CERTIFICATE_COURSE_ROOT>)CMSHandler.FetchData<CERTIFICATE_COURSE_ROOT>(
                                    new SUP_CLASS() { CLASS_ID = sClassId, SETTING_ID = sSettingId },
                                CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.LoadCertificateCourses), sAcademicYear).DataSource.Data;
                            if (liCourseRoot != null && liCourseRoot.Count > 0)
                            {
                                string sDisabled = string.Empty;
                                foreach (var item in liCourseRoot)
                                {
                                    sDisabled = string.Empty;
                                    if (item.BALANCE_QUOTA == "0")
                                    {
                                        sDisabled = "disabled";
                                    }
                                    Result += "<div class='form-group'> <input type='radio' cms-CourseTitle='" + item.COURSE_TITLE + "' cms-semester='" + item.SEMESTER_ID + "' cms-classid='" + sClassId + "' cms-courseid='" + item.COURSE_ROOT_ID + "'name='CerticateCourse' id='" + item.COURSE_ROOT_ID + "' value='" + sSettingId + "' " + sDisabled + "> <label for='" + item.COURSE_ROOT_ID + "' id=" + item.COURSE_ROOT_ID + "> " + item.COURSE_TITLE + "</lable></div>";
                                }
                                if (!string.IsNullOrEmpty(Result))
                                {
                                    objResultData.sResult = Result;
                                    objResultData.ErrorCode = Common.ErrorCode.Ok;
                                }

                            }
                            else
                            {
                                objResultData.Message = Common.Messages.NoRecordsfound;
                                objResultData.ErrorCode = Common.ErrorCode.NoContent;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LoadNMECourses", "NMEController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("LoadNMECourses", "NMEController", ex.Message);
                    }

                }
            }
            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Certificate Course Setting
        [UserRights("ADMIN")]
        public ActionResult CertificateSettingsList()
        {
            CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                var NMESettings = new NMESettings();
                objViewModel.liCertificateCourseSetiing = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(null, CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseSettingList), sAcademicYear).DataSource.Data;

                //fetch semester
                var SemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                if (SemesterType != null && SemesterType.Count > 0)
                {
                    objViewModel.SemesterType = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);

                }


                return View(objViewModel);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        public ActionResult CertificateCourseSetting()
        {
            CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
            //var Class = new List<cp_Classes_2017>();
            var Semester = new List<SupSemester>();
            var Shift = new List<Sup_Shift>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.SemesterType = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CertificateCourseSetting", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CertificateCourseSetting", "CertificateCourse", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public string GetClassIdByShiftId(string sShiftId)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
                    SaveNMESettings objModel = new SaveNMESettings();
                    objModel.SHIFT = sShiftId;
                    var Class = new List<cp_Classes_2017>();
                    Class = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objModel, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOrdinaryClassesByShiftId), sAcademicYear).DataSource.Data;
                    if (Class != null && Class.Count > 0)
                    {
                        foreach (var List in Class)
                            sOption += "<option value='" + List.CLASS_ID + "' >" + List.CLASS_NAME + "</option>";
                    }
                }
                else
                    sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetClassIdByShiftId", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetClassIdByShiftId", "CertificateCourse", ex.Message);
                }
            }
            return sOption;
        }
        public JsonResult SaveCertificateCourseSetting(string sJsonData)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            ResultArgs resultArgs = new ResultArgs();
            JsonResultData objResultData = new JsonResultData();
            var sResult = string.Empty;
            try
            {
                if (sJsonData != null)
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        var Result = JsonConvert.DeserializeObject<CERTIFICATE_COURSE_SETTING>(sJsonData);
                        Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                        Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                        //var FetchExistingSetting = (List<CERTIFICATE_COURSE_SETTINGS>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTINGS>(Result, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsCertificateCourseSettingExist), sAcademicYear).DataSource.Data;
                        //if (FetchExistingSetting != null)
                        //{
                        //    sResult = "This Setting Is Existing Already...!";
                        //}

                        if (!string.IsNullOrEmpty(Result.SETTING_ID))
                        {
                            //update 
                            resultArgs = CMSHandler.SaveOrUpdate(Result, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateCertificateCourseSetting), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                        else
                        {
                            //insert 
                            resultArgs = CMSHandler.SaveOrUpdate(Result, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.SaveCertificateCourseSetting), sAcademicYear);
                            objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                        }
                    }
                    else
                    {
                        objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                        objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
                    }

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveCertificateCourseSetting", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveCertificateCourseSetting", "CertificateCourse", ex.Message);
                }
            }
            return Json(objResultData);
        }
        public ActionResult EditCertificateCourseSettings(int id)
        {
            CertificateCoursesViewModel objViewModel = new CertificateCoursesViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    Session[Common.SESSION_VARIABLES.SETTINGS_ID] = id;
                    var Class = new List<cp_Classes_2017>();
                    var Semester = new List<SupSemester>();
                    var Shift = new List<Sup_Shift>();
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    Class = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                    Shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (Class != null && Class.Count > 0)
                        objViewModel.Class = new SelectList(Class, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    if (Semester != null && Semester.Count > 0)
                        objViewModel.SemesterType = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
                    if (Shift != null && Shift.Count > 0)
                        objViewModel.ShiftList = new SelectList(Shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditNMESettings", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditNMESettings", "CertificateCourse", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchCertificateCourseSettingsById(string sSettingsId)
        {
            JsonResultData sResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    CERTIFICATE_COURSE_SETTING objModel = new CERTIFICATE_COURSE_SETTING();
                    objModel.SETTING_ID = sSettingsId;
                    sResult.oResult = (List<CERTIFICATE_COURSE_SETTING>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTING>(objModel, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.FetchCertificateCourseSettingById), sAcademicYear).DataSource.Data;
                }
                else
                {
                    sResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchCertificateCourseSettingsById", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchCertificateCourseSettingsById", "CertificateCourse", ex.Message);

                }
            }
            return Json(sResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ActiveRegistration(string sSettingsId, string sMode)
        {
            string Result = string.Empty;
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sSettingsId) && !string.IsNullOrEmpty(sMode))
                    {
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        CERTIFICATE_COURSE_SETTING objModel = new CERTIFICATE_COURSE_SETTING();
                        objModel.SETTING_ID = sSettingsId;
                        objModel.IS_ACTIVE = sMode;
                        resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.CertificateCourseSettingRegistrationClose), sAcademicYear);

                        objResultData.Message = resultArgs.Success ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;

                    }
                    else
                    {
                        objResultData.Message = "Failed to close Registration !!!";
                        objResultData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }

                }
                else
                {
                    objResultData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResultData.ErrorCode = Common.ErrorCode.ServiceUnavailable;
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateRegistration", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateRegistration", "CertificateCourse", ex.Message);
                }
            }
            return Json(objResultData);
        }
        public ActionResult UpdateCertificateCourseSettings(string jsonCertificateCourseSetting)
        {
            var sResult = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (jsonCertificateCourseSetting != null)
                    {
                        var Result = JsonConvert.DeserializeObject<CERTIFICATE_COURSE_SETTINGS>(jsonCertificateCourseSetting);
                        Result.SETTINGS_ID = (Session[Common.SESSION_VARIABLES.SETTINGS_ID] != null) ? Session[Common.SESSION_VARIABLES.SETTINGS_ID].ToString() : "";
                        Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                        Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.UpdateCertificateCourseSettings), sAcademicYear);
                        if (ResultArg.Success)
                            sResult = Common.Messages.RecordsSavedSuccessfully;
                        else
                            sResult = Common.Messages.FailedToSaveRecords;
                        //var FetchExistingSetting = (List<CERTIFICATE_COURSE_SETTINGS>)CMSHandler.FetchData<CERTIFICATE_COURSE_SETTINGS>(Result, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.IsCertificateCourseSettingExist), sAcademicYear).DataSource.Data;
                        //if (FetchExistingSetting != null)
                        //  sResult = "This Setting Is Existing Already...!";
                        //else
                        //{

                        // }
                    }
                    return Json(sResult);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateRegistration", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateRegistration", "CertificateCourse", ex.Message);
                    return Json(sResult);
                }
            }

        }
        public JsonResult DeleteCertificateCourseSettings(string sSettingId)
        {
            JsonResultData objResultData = new JsonResultData();
            ResultArgs resultArgs = new ResultArgs();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() != null)
            {
                try
                {
                    CERTIFICATE_COURSE_SETTING objModel = new CERTIFICATE_COURSE_SETTING();
                    objModel.SETTING_ID = sSettingId;
                    resultArgs = CMSHandler.SaveOrUpdate(objModel, SQL.CertificateCourses.CertificateCourseSQL.GetCertificateCourseSQL(CertificateCourse.DeleteCertificateCourseSetting));
                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    objResultData.ErrorCode = resultArgs.Success ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteCertificateCourseSettings", "CertificateCourse", "Err MSg : " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteCertificateCourseSettings", "CertificateCourse", ex.Message);

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
        #endregion
    }
}