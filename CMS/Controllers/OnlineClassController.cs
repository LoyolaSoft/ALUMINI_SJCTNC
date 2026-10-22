using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using CMS.ViewModel.Model;
using Newtonsoft.Json;
using CMS.Models;
using CMS.SQL.OnlineClass;
using CMS.DAO;
using System.IO;
using CMS.SQL.SupportData;

namespace CMS.Controllers
{
    public class OnlineClassController : Controller
    {
        // GET: OnlineClass
        public ActionResult Index()
        {
            return View();
        }

        #region Online Class 
        public ActionResult OnlineClass()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new OnlineClassViewModel();
            var ObjModel = new USER_ACCOUNT();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    ObjModel.USER_ID = sUserId;
                    ObjViewModel.liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "6" }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours)).DataSource.Data;
                    if (sRole=="5")
                    {
                        ObjViewModel.liOnlineClass = (List<OnlineClassModel>)CMSHandler.FetchData<OnlineClassModel>(ObjModel, OnlineClassSQL.GetOnlineClassSQL(OnlineClassSQLCommads.FetchTimeTableForStudentOnlineClass), sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        ObjViewModel.liOnlineClass = (List<OnlineClassModel>)CMSHandler.FetchData<OnlineClassModel>(ObjModel, OnlineClassSQL.GetOnlineClassSQL(OnlineClassSQLCommads.FetchTimeTableForStaffOnlineClass), sAcademicYear).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("OnlineClassController", "OnlineClass", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Online Class Timetable Report
        public ActionResult OnlineTimeTableClassWiseList()
        {
            OnlineClassViewModel ObjViewModel = new OnlineClassViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        ObjViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("OnlineTimeTableClassWiseList", "OnlineClassController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("OnlineTimeTableClassWiseList", "OnlineClassController", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult OnlineTimeTableClassWiseListForHOD()
        {
            string sAdademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUser_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            OnlineClassViewModel ObjViewModel = new OnlineClassViewModel();
            var Objmodel = new STF_PERSONAL_INFO();
            if (!string.IsNullOrEmpty(sAdademicyear))
            {
                try
                {
                    Objmodel.STAFF_ID = sUser_id;
                    var Listaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(Objmodel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAdademicyear).DataSource.Data;
                    if (Listaff != null && Listaff.Count > 0)
                    {
                        Objmodel.STAFF_ID = Listaff.FirstOrDefault().STAFF_ID;
                        Objmodel.DEPARTMENT = Listaff.FirstOrDefault().DEPARTMENT;
                        string SQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.SHIFT, Listaff.FirstOrDefault().SHIFT);
                        var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(Objmodel, SQL, sAdademicyear).DataSource.Data;
                        if (liclass != null && liclass.Count > 0)
                        {
                            ObjViewModel.ClassList = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("OnlineTimeTableClassWiseListForHOD", "OnlineClassController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("OnlineTimeTableClassWiseListForHOD", "OnlineClassController", ex.Message);
                    }
                }
            }
            else
            {

            }
            return View(ObjViewModel);
        }

        public ActionResult BindOnlineClassTimeTable(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new OnlineClassViewModel();
            var ObjModel = new OnlineClassModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.CLASS_ID = sClassId;
                    ObjViewModel.liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours)).DataSource.Data;
                    ObjViewModel.liOnlineClass = (List<OnlineClassModel>)CMSHandler.FetchData<OnlineClassModel>(ObjModel, OnlineClassSQL.GetOnlineClassSQL(OnlineClassSQLCommads.FetchOnlineClassTimeTableReportClassWise), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog=new ErrorLog())
                    {
                        ObjLog.WriteError("OnlineClassController", "BindOnlineClassTimeTable", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return View(ObjViewModel);
        }
        #endregion
    }
}