using CMS.Utility;
using CMS.ViewModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Models;
using System.Web.Mvc;
using CMS.ViewModel.Model;
using CMS.DAO;
using CMS.SQL;
using CMS.SQL.TaskManagement;
using CMS.SQL.Examination;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Data;
using CMS.SQL.SupportData;

namespace CMS.Controllers
{
    public class TaskManagementController : Controller
    {
        // GET: TaskManagement
        JsonResultData ObjJsonData = new JsonResultData();
        ResultArgs ResultArg = new ResultArgs();
        public ActionResult Index()
        {
            return View();
        }

        #region Assignment
        public ActionResult Assignment()
        {
            stf_Personal_Info objStaffModel = new stf_Personal_Info();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objStaffModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Assignment", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Assignment", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult ListAssignment()
        {
            stf_Personal_Info objStaffModel = new stf_Personal_Info();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objStaffModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    List<cp_Course_Root_2017> listCourse = new List<cp_Course_Root_2017>();
                    listCourse.Add(new cp_Course_Root_2017() { COURSE_ROOT_ID = "0", COURSE_TITLE = "---Select---" });
                    objViewModel.CourseList = new SelectList(listCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "ListAssignment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "ListAssignment", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult GetCourseListByClassIdAndStaffId(string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            CourseWiseStaffList objStaffModel = new CourseWiseStaffList();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sOption = string.Empty;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.CLASS_ID = sClassId;
                    objStaffModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var CourseList = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(objStaffModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseList), sAcademicYear).DataSource.Data;
                    if (CourseList != null && CourseList.Count > 0)
                    {
                        sOption = "<option value='0'>--Select--</option>";
                        foreach (var item in CourseList)
                        {
                            sOption += "<option value='" + item.COURSE_ID + "'>" + item.COURSE_TITLE + "</option>";
                        }
                        objResult.sResult = sOption;
                    }
                }
                else
                {
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "GetCourseListByClassIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "GetCourseListByClassIdAndStaffId", ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindAssignmentListByCourseId(string sCourseId, string sClassId)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            AssignmentModel objModel = new AssignmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.COURSE_ID = sCourseId;
                    objModel.CLASS_ID = sClassId;
                    var FetchClass = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAssignmentList), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                    {
                        foreach (var item in FetchClass)
                        {
                            byte[] bytes = Convert.FromBase64String(item.DESCRIPTION);
                            item.DESCRIPTION = Encoding.UTF8.GetString(bytes);
                        }
                        objViewModel.liTaskManagementModel = FetchClass;
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "BindAssignmentListByCourseId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "BindAssignmentListByCourseId", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult SaveAssignment(string Assignment)
        {
            JsonResultData objResult = new JsonResultData();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            AssignmentModel objModel = new AssignmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (Assignment != null)
                    {
                        var Result = JsonConvert.DeserializeObject<AssignmentModel>(Assignment);
                        Result.DATE_FROM = DateTime.Today.ToString("yyyy-MM-dd");
                        Result.SUBMISSION_DATE = CommonMethods.ConvertdatetoyearFromat(Result.SUBMISSION_DATE).ToString();
                        Result.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        var SaveAssignment = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Result.ASSIGNMENT_ID)) ? TaskManagementSQLCommands.UpdateAssignment : TaskManagementSQLCommands.SaveAssignment), sAcademicYear);
                        if (SaveAssignment.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "SaveAssignment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "SaveAssignment", ex.Message);
                }

            }
            return Json(objResult);
        }
        public ActionResult EditAssignment(string id)
        {
            CourseWiseStaffList objStaffModel = new CourseWiseStaffList();
            AssignmentModel objAssignmentModel = new AssignmentModel();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objAssignmentModel.ASSIGNMENT_ID = id;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objStaffModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    objViewModel.liTaskManagementModel = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objAssignmentModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAssignmentById), sAcademicYear).DataSource.Data;
                    if (objViewModel.liTaskManagementModel != null && objViewModel.liTaskManagementModel.Count > 0)
                    {
                        objViewModel.Assignment.ASSIGNMENT_ID = objViewModel.liTaskManagementModel.FirstOrDefault().ASSIGNMENT_ID;
                        objViewModel.ClassId = objViewModel.liTaskManagementModel.FirstOrDefault().CLASS_ID;
                        objViewModel.CourseId = objViewModel.liTaskManagementModel.FirstOrDefault().COURSE_ID;
                        objViewModel.Assignment.ASSIGNMENT_TITLE = objViewModel.liTaskManagementModel.FirstOrDefault().ASSIGNMENT_TITLE;
                        objViewModel.Assignment.SUBMISSION_DATE = objViewModel.liTaskManagementModel.FirstOrDefault().SUBMISSION_DATE;
                        objViewModel.Assignment.IS_ACTIVE = objViewModel.liTaskManagementModel.FirstOrDefault().IS_ACTIVE;
                        objViewModel.Assignment.IS_FILE_UPLOAD = objViewModel.liTaskManagementModel.FirstOrDefault().IS_FILE_UPLOAD;
                        objStaffModel.CLASS_ID = objViewModel.ClassId;
                        var CourseList = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(objStaffModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseList), sAcademicYear).DataSource.Data;
                        if (CourseList != null && CourseList.Count > 0)
                            objViewModel.CourseList = new SelectList(CourseList, Common.CP_CLASS_COURSE_2017.COURSE_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                        objViewModel.Assignment.DESCRIPTION = objViewModel.liTaskManagementModel.FirstOrDefault().DESCRIPTION;
                        byte[] bytes = Convert.FromBase64String(objViewModel.Assignment.DESCRIPTION);
                        objViewModel.HiddenId = Encoding.UTF8.GetString(bytes);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Assignment", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Assignment", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult DeleteAssignment(string sAssignmentId)
        {
            JsonResultData objResult = new JsonResultData();
            AssignmentModel objModel = new AssignmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.ASSIGNMENT_ID = sAssignmentId;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var DeleteAssignment = CMSHandler.SaveOrUpdate(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteAssignment), sAcademicYear);
                    if (DeleteAssignment.Success)
                        objResult.Message = Common.Messages.RecordDeletedSuccessfully;
                    else
                        objResult.Message = Common.Messages.FailedToDeletedTryAgain;
                }
                else
                {
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "DeleteAssignment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "DeleteAssignment", ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AssignmentSubmittedList()
        {
            stf_Personal_Info objStaffModel = new stf_Personal_Info();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objStaffModel, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassList), sAcademicYear).DataSource.Data;
                    List<cp_Course_Root_2017> listCourse = new List<cp_Course_Root_2017>();
                    List<AssignmentModel> listAssignment = new List<AssignmentModel>();
                    listCourse.Add(new cp_Course_Root_2017() { COURSE_ROOT_ID = "0", COURSE_TITLE = "---Select---" });
                    listAssignment.Add(new AssignmentModel() { ASSIGNMENT_ID = "0", ASSIGNMENT_TITLE = "---Select---" });
                    objViewModel.CourseList = new SelectList(listCourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "AssignmentSubmittedList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "AssignmentSubmittedList", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult GetAssignmentListByCourseIdIdAndStaffId(string sClassId, string sCourseId)
        {
            JsonResultData objResult = new JsonResultData();
            CourseWiseStaffList objStaffModel = new CourseWiseStaffList();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sOption = string.Empty;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.CLASS_ID = sClassId;
                    objStaffModel.COURSE_ID = sCourseId;
                    objStaffModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var AssignmentList = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objStaffModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAssignmentByClassIdAndCourseIdAndStaffId), sAcademicYear).DataSource.Data;
                    if (AssignmentList != null && AssignmentList.Count > 0)
                    {
                        sOption = "<option value='0'>--Select--</option>";
                        foreach (var item in AssignmentList)
                        {
                            sOption += "<option value='" + item.ASSIGNMENT_ID + "'>" + item.ASSIGNMENT_TITLE + "</option>";
                        }
                        objResult.sResult = sOption;
                    }
                }
                else
                {
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "GetAssignmentListByCourseIdIdAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "GetAssignmentListByCourseIdIdAndStaffId", ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindAssignmentSubmittedListByAssignmentId(string sCourseId, string sClassId, string sAssignmentId)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            AssignmentModel objModel = new AssignmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.COURSE_ID = sCourseId;
                    objModel.ASSIGNMENT_ID = sAssignmentId;
                    objModel.CLASS_ID = sClassId;
                    var FetchSubmittedlist = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAssignmentSubmittedList), sAcademicYear).DataSource.Data;
                    if (FetchSubmittedlist != null && FetchSubmittedlist.Count > 0)
                    {
                        //foreach (var item in FetchSubmittedlist)
                        //{
                        //    byte[] bytes = Convert.FromBase64String(item.DESCRIPTION);
                        //    item.DESCRIPTION = Encoding.UTF8.GetString(bytes);
                        //}
                        objViewModel.liTaskManagementModel = FetchSubmittedlist;
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "BindAssignmentSubmittedListByAssignmentId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "BindAssignmentSubmittedListByAssignmentId", ex.Message);
                }

            }
            return View(objViewModel);
        }

        public ActionResult FetchStudentAssignementdetails(string sAssignmentId, string sStudentId)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            AssignmentModel objModel = new AssignmentModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objModel.ASSIGNMENT_ID = sAssignmentId;
                    objModel.STUDENT_ID = sStudentId;
                    var FetchAssignmentDetails = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchStudentAssignmentfile), sAcademicYear).DataSource.Data;
                    if (FetchAssignmentDetails != null && FetchAssignmentDetails.Count > 0)
                    {

                        objViewModel.liTaskManagementModel = FetchAssignmentDetails;
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "BindAssignmentSubmittedListByAssignmentId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "BindAssignmentSubmittedListByAssignmentId", ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Assignment Report
        public ActionResult TaskReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            TaskManagementViewModel ObjViewModel = new TaskManagementViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchEvent = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchLastSubmissionEventInfo), sAcademicYear).DataSource.Data;
                if (liFetchEvent != null && liFetchEvent.Count > 0)
                {
                    ObjViewModel.CalenderEvents = new SelectList(liFetchEvent, Common.CALENDER_EVENTS_2017.DAY_ID, Common.CALENDER_EVENTS_2017.EVENT_NAME);
                }
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "error");
            }
        }

        public JsonResult GetEventByDate()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //var liFetchEvent = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null,TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchEventByDate)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagement", "GetEventByDate", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagement", "GetEventByDate", ex.Message);
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

        #region Task Setting Component Wise Cia Block
        public ActionResult TaskSettingList()
        {
            TaskManagementViewModel ObjViewModel = new TaskManagementViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (liSemesterType != null && liSemesterType.Count > 0)
                    {
                        ObjViewModel.SemesterType = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }

                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYear = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.ACADEMIC_NAME);
                    }

                    var liCiaComponent = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent)).DataSource.Data;
                    if (liCiaComponent != null && liCiaComponent.Count > 0)
                    {
                        ObjViewModel.Component = new SelectList(liCiaComponent, Common.SUP_CIA_COMPONENTS.COMPONENT_ID, Common.SUP_CIA_COMPONENTS.COMPONENT);
                    }

                    ObjViewModel.liTaskSetting = (List<SUP_TASK_SETTING>)CMSHandler.FetchData<SUP_TASK_SETTING>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskSettingList), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "TaskSettingList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "TaskSettingList", ex.Message);
                    }
                }
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "error");
            }

        }

        public JsonResult FetchTaskSettingById(string sTaskSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sTaskSettingId))
                    {
                        var liTaskSetting = (List<SUP_TASK_SETTING>)CMSHandler.FetchData<SUP_TASK_SETTING>(new SUP_TASK_SETTING() { TASK_ID = sTaskSettingId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskSettingById)).DataSource.Data;
                        return Json(liTaskSetting, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "FetchTaskSettingById", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "FetchTaskSettingById", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SaveTaskSetting(string sTaskSetting)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<SUP_TASK_SETTING>(sTaskSetting);
                    if (!string.IsNullOrEmpty(Result.TASK_NAME))
                    {
                        Result.LAST_DATE = CommonMethods.ConvertdatetoyearFromat(Result.LAST_DATE);
                        if (!string.IsNullOrEmpty(Result.TASK_ID))
                        {
                            var UpdateTaskSetting = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateTaskSetting), sAcademicYear);
                            ObjJsonData.Message = (UpdateTaskSetting.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        }
                        else
                        {
                            var liExist = (List<SUP_TASK_SETTING>)CMSHandler.FetchData<SUP_TASK_SETTING>(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.IsTaskSettingExist), sAcademicYear).DataSource.Data;
                            if (liExist != null && liExist.Count > 0)
                            {
                                ObjJsonData.Message = "Task Already Exist ! ";
                            }
                            else
                            {
                                var InsertTaskSetting = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveTaskSetting), sAcademicYear);
                                ObjJsonData.Message = (InsertTaskSetting.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "SaveTaskSetting", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "SaveTaskSetting", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTaskSetting(string sTaskSettingId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sTaskSettingId))
                    {
                        var DeleteTaskSetting = CMSHandler.SaveOrUpdate(new SUP_TASK_SETTING() { TASK_ID = sTaskSettingId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteTaskSetting));
                        ObjJsonData.Message = (DeleteTaskSetting.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "DeletetaskSetting", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "DeletetaskSetting", ex.Message);
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

        #region FetchCiaUnUploadedStaff
        public ActionResult CiaUnUploadedStatus()
        {
            TaskManagementViewModel ObjViewModel = new TaskManagementViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCIA_Component = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCiaComponent), sAcademicYear).DataSource.Data;
                    if (liCIA_Component != null && liCIA_Component.Count > 0)
                    {
                        ObjViewModel.Component = new SelectList(liCIA_Component, Common.SUP_CIA_COMPONENTS.COMPONENT_ID, Common.SUP_CIA_COMPONENTS.COMPONENT);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "CiaUnUploadedStatus", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "CiaUnUploadedStatus", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult FetchCiaDefaulters(string sComponentId)
        {
            TaskManagementViewModel ObjViewModel = new TaskManagementViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = "2,4,6"; // (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSql = string.Empty;
                    if (!string.IsNullOrEmpty(sComponentId))
                    {
                        sSql = TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchCia_Status).Replace(Common.Delimiter.QUS+Common.CP_COURSE_ROOT_2017.SEMESTER_ID,sActiveSemester);
                        ObjViewModel.liComponent = (List<CIA_COMPONENT>)CMSHandler.FetchData<CIA_COMPONENT>(new CIA_COMPONENT() { COMPONENT_ID = sComponentId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchCiaComponent), sAcademicYear).DataSource.Data;
                        if (ObjViewModel.liComponent != null && ObjViewModel.liComponent.Count > 0)
                        {
                            foreach (var item in ObjViewModel.liComponent)
                            {
                                var listatus = (List<Cia_Status>)CMSHandler.FetchData<Cia_Status>(new CIA_COMPONENT() { COMPONENT_ID = item.CIA_GROUP_COMPONENT_ID }, sSql, sAcademicYear).DataSource.Data;
                                if (listatus != null && listatus.Count > 0)
                                {
                                    ObjViewModel.liCia_Status = (ObjViewModel.liCia_Status != null && ObjViewModel.liCia_Status.Count > 0) ? ObjViewModel.liCia_Status.Concat(listatus).ToList() : listatus;
                                }
                            }
                        }
                        //  ObjViewModel.liCia_Status = (List<Cia_Status>)CMSHandler.FetchData<Cia_Status>(new CIA_COMPONENT() { COMPONENT_ID=sComponentId},TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchCia_Status),sAcademicYear).DataSource.Data;

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "FetchCiaDefaulters", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "FetchCiaDefaulters", ex.Message);
                    }
                }
            }
            else
            {
                JsonResultData ObjJsonData = new JsonResultData();
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Block Cia By Admin
        // CIA Block List ...
        public ActionResult CiaBlockList()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new TaskManagementViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }

        // Fetch CIA List By DDL ...
        public ActionResult FetchCIAListByDDL(string sCIAId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new TaskManagementViewModel();
            //var liFetchCIABlockList = (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(null,).DataSource.Data;
            var liFetchCIABlockList = new List<CIA_BLOCK_LIST>();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {

                if (!string.IsNullOrEmpty(sCIAId))
                {

                    ObjViewModel.liCIABlockList = (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(new CIA_BLOCK_LIST() { ORDER_ID = sCIAId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchCIAListByOrderId), sAcademicYear).DataSource.Data;
                    ObjViewModel.ObjCIABlockList.ORDER_ID = sCIAId;
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Add CIA Block List ...
        public ActionResult AddCiaBlockList(string sCIABlockListId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new TaskManagementViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Class List ....
                    var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    // Course List ....
                    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourse), sAcademicYear).DataSource.Data;
                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        ObjViewModel.CourseList = new SelectList(liCourseList, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                    // Staff List ....
                    var liStaffList = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                    if (liStaffList != null && liStaffList.Count > 0)
                    {
                        ObjViewModel.StaffList = new SelectList(liStaffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                    // Programme List ....
                    var liProgramme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme), sAcademicYear).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                    {
                        ObjViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    }
                    // Progamme Level ....
                    var liProgrammeLevel = (List<SUP_PROGRAMME_LEVEL>)CMSHandler.FetchData<SUP_PROGRAMME_LEVEL>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeLevel), sAcademicYear).DataSource.Data;
                    if (liProgrammeLevel != null && liProgrammeLevel.Count > 0)
                    {
                        ObjViewModel.ProgrammeLevel = new SelectList(liProgrammeLevel, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL_ID, Common.SUP_PROGRAMME_LEVEL.PROGRAMME_LEVEL);
                    }
                    // Shift List ...
                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null,
                        SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.ShiftList = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    ObjViewModel.ObjCIABlockList.CIA_BLOCK_ID = sCIABlockListId;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "AddCiaBlockList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "AddCiaBlockList", ex.Message);
                    }
                }

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Fetch CIA Block List By ID ...
        public JsonResult FetchCIABlockListById(string sCIABlockListId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFetchCIaList = (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(new CIA_BLOCK_LIST() { CIA_BLOCK_ID = sCIABlockListId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchCiaBlockListById), sAcademicYear).DataSource.Data;
                    if (liFetchCIaList != null && liFetchCIaList.Count > 0)
                    {
                        return Json(liFetchCIaList, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "FetchCIABlockListById", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "FetchCIABlockListById", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Update Active ...
        public JsonResult ActiveCIABlockList(string sCIABlockListId, string sActive)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var UpdateActive = CMSHandler.SaveOrUpdate(new CIA_BLOCK_LIST() { CIA_BLOCK_ID = sCIABlockListId, IS_ACTIVE = sActive },
                   TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateActiveCIABlockList), sAcademicYear);
                    ObjJsonData.Message = (UpdateActive.Success) ? "Record Activated Successfully :)" : "Record Con't Activate ! Try Again :(";
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "ActiveCIABlockList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "ActiveCIABlockList", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Fetch Course By Class ...
        public JsonResult FetchCourseByClass(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new ClassCourse() { CLASS_ID = sClassId },
                    SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassId), sAcademicYear).DataSource.Data;
                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liCourseList)
                        {
                            ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "FetchCourseByClass", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "FetchCourseByClass", ex.Message);
                    }
                }

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Fetch Staff By ClassAndCourse ...
        public JsonResult FetchStaffByClassAndCourse(string sClassId, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liStaffList = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new ClassCourse() { CLASS_ID = sClassId, COURSE_ID = sCourseId },
                    SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffByClassIdAndCourseId), sAcademicYear).DataSource.Data;
                    if (liStaffList != null && liStaffList.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liStaffList)
                        {
                            ObjJsonData.sResult += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "FetchStaffByClassAndCourse", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "FetchStaffByClassAndCourse", ex.Message);
                    }
                }

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // On Submit CIA Block List ...
        public JsonResult SaveCIABlockList(string sSaveCIABlockList)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<CIA_BLOCK_LIST>(sSaveCIABlockList);
                    Result.DATE_FROM = CommonMethods.ConvertdatetoyearFromat(Result.DATE_FROM);
                    Result.DATE_TO = CommonMethods.ConvertdatetoyearFromat(Result.DATE_TO);


                    var liFetchCIABlockList = (List<CIA_BLOCK_LIST>)CMSHandler.FetchData<CIA_BLOCK_LIST>(Result,
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchCiaBlockList), sAcademicYear).DataSource.Data;
                    if (liFetchCIABlockList != null && liFetchCIABlockList.Count > 0)
                    {
                        // If Statement for Update .
                        if (!string.IsNullOrEmpty(Result.CIA_BLOCK_ID))
                        {
                            var UpdateCIABlockList = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateCiaBlockList), sAcademicYear);
                            ObjJsonData.Message = (UpdateCIABlockList.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        }
                        else
                        {
                            // Staff Exist ....
                            var IsSatffExist = liFetchCIABlockList.Where(x => x.S_STAFF_ID == Result.S_STAFF_ID && (!string.IsNullOrEmpty(Result.S_STAFF_ID))).ToList();
                            if (IsSatffExist != null && IsSatffExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Staff Already Exist Till :" + " " + IsSatffExist.FirstOrDefault().DATE_TO;
                            }
                            // Class Exist ....
                            var IsClassExist = liFetchCIABlockList.Where(x => x.S_CLASS_ID == Result.S_CLASS_ID && (!string.IsNullOrEmpty(Result.S_CLASS_ID))).ToList();
                            if (IsClassExist != null && IsClassExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Class Already Exist Till :" + " " + IsClassExist.FirstOrDefault().DATE_TO;
                            }

                            // Course Exist ....
                            var IsCourseExist = liFetchCIABlockList.Where(x => x.S_COURSE_ID == Result.S_COURSE_ID && (!string.IsNullOrEmpty(Result.S_COURSE_ID))).ToList();
                            if (IsCourseExist != null && IsCourseExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Course Already Exist Till :" + " " + IsCourseExist.FirstOrDefault().DATE_TO;
                            }
                            // Shift Exist ....
                            var IsShiftExist = liFetchCIABlockList.Where(x => x.SHIFT_ID == Result.SHIFT_ID && (!string.IsNullOrEmpty(Result.SHIFT_ID))).ToList();
                            if (IsShiftExist != null && IsShiftExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Shift Already Exist Till :" + " " + IsShiftExist.FirstOrDefault().DATE_TO;
                            }
                            // Programme Exist ....
                            var IsProgrammeExist = liFetchCIABlockList.Where(x => x.PROGRAMME_GROUP_ID == Result.PROGRAMME_GROUP_ID && (!string.IsNullOrEmpty(Result.PROGRAMME_GROUP_ID))).ToList();
                            if (IsProgrammeExist != null && IsProgrammeExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Programme Already Exist Till :" + " " + IsProgrammeExist.FirstOrDefault().DATE_TO;
                            }
                            // Programme Level Exist ....
                            var IsProgrammeLevelExist = liFetchCIABlockList.Where(x => x.PROGRAMME_LEVEL == Result.PROGRAMME_LEVEL && (!string.IsNullOrEmpty(Result.PROGRAMME_LEVEL))).ToList();
                            if (IsProgrammeLevelExist != null && IsProgrammeLevelExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Programme Level Already Exist Till :" + " " + IsProgrammeLevelExist.FirstOrDefault().DATE_TO;
                            }
                            // Class Course Staff .....
                            var IsClassCourseExist = liFetchCIABlockList.Where(x => x.CLASS_ID == Result.CLASS_ID && x.COURSE_ID == Result.COURSE_ID && x.STAFF_ID == Result.STAFF_ID && (!string.IsNullOrEmpty(Result.STAFF_ID))).ToList();
                            if (IsClassCourseExist != null && IsClassCourseExist.Count > 0)
                            {
                                ObjJsonData.Message = "This Class Course Already Exist Till :" + " " + IsClassCourseExist.FirstOrDefault().DATE_TO;
                            }
                            if (string.IsNullOrEmpty(ObjJsonData.Message))
                            {
                                var SaveCIABlockList = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveCiaBlockList), sAcademicYear);
                                ObjJsonData.Message = (SaveCIABlockList.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                        // End Update Else Part 
                    }
                    // End If (CIA Block List)
                    else
                    {
                        var SaveCIABlockList = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveCiaBlockList), sAcademicYear);
                        ObjJsonData.Message = (SaveCIABlockList.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "SaveCIABlockList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "SaveCIABlockList", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Delete CIA BlockList ...
        public JsonResult DeleteCIABlockList(string sCIABlockListId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DeleteCIABlockList = CMSHandler.SaveOrUpdate(new CIA_BLOCK_LIST() { CIA_BLOCK_ID = sCIABlockListId },
                   TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteCiaBlockList), sAcademicYear);
                    ObjJsonData.Message = (DeleteCIABlockList.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "DeleteCIABlockList", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "DeleteCIABlockList", ex.Message);
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

        #region Activity
        // Add Activity .
        public ActionResult AcademicActivity(string sActivityId)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    //var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    var FetchActivityLevel = (List<SUP_ACTIVITY_LEVEL>)CMSHandler.FetchData<SUP_ACTIVITY_LEVEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupActivityLevel), sAcademicYear).DataSource.Data;
                    var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                    //if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                    //    objViewModel.AcademicYear = new SelectList(FetchAcademicyear, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    if (FetchActivityLevel != null && FetchActivityLevel.Count > 0)
                        objViewModel.ActivityLevelList = new SelectList(FetchActivityLevel, Common.SUP_ACTIVITY_LEVEL.ACTIVITY_LEVEL_ID, Common.SUP_ACTIVITY_LEVEL.ACTIVITY_LEVEL);
                    if (FetchStaff != null && FetchStaff.Count > 0)
                        objViewModel.StaffList = new SelectList(FetchStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    objViewModel.sActivityId = sActivityId;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AcademicActivity", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AcademicActivity", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        // List Activity For Admin .
        public ActionResult ListAcademicActivity()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicyear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    var FetchActivityLevel = (List<SUP_ACTIVITY_LEVEL>)CMSHandler.FetchData<SUP_ACTIVITY_LEVEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupActivityLevel), sAcademicYear).DataSource.Data;
                    if (FetchActivityLevel != null && FetchActivityLevel.Count > 0)
                        objViewModel.ActivityLevelList = new SelectList(FetchActivityLevel, Common.SUP_ACTIVITY_LEVEL.ACTIVITY_LEVEL_ID, Common.SUP_ACTIVITY_LEVEL.ACTIVITY_LEVEL);
                    if (FetchAcademicyear != null && FetchAcademicyear.Count > 0)
                        objViewModel.AcademicYear = new SelectList(FetchAcademicyear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "ListAcademicActivity", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "ListAcademicActivity", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindActivity(string Academicyear, string ActivityLevel)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            var ObjModel = new AcademicActivityModel();
            var liActivityList = new List<AcademicActivityModel>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    ObjModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    ObjModel.ACTIVITY_LEVEL = ActivityLevel;
                    ObjModel.ACADEMIC_YEAR = Academicyear;
                    ObjModel.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(); ;
                    var liFetchStaff = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(ObjModel,
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchStaffIdByAcademicYearAndActivityLevel), sAcademicYear).DataSource.Data;
                    if (liFetchStaff != null && liFetchStaff.Count > 0)
                    {
                        string sSQL = string.Empty;
                        foreach (var item in liFetchStaff)
                        {
                            ObjModel.ACTIVITY_ID = item.ACTIVITY_ID;
                            sSQL = TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchActivityByActivityLevel).
                                Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, item.STAFF_ID);

                            var liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(ObjModel, sSQL, Academicyear).DataSource.Data;
                            if (liActivity != null && liActivity.Count > 0)
                            {
                                objViewModel.liActivityList = (objViewModel.liActivityList != null) ? objViewModel.liActivityList.Concat(liActivity).ToList() : liActivity;
                            }
                        }
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "BindActivity", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "BindActivity", ex.Message);
                }
            }
            return View(objViewModel);
        }

        /// <summary>
        /// 
        /// save file correction
        /// 
        /// </summary>
        /// <param name="sActivityId"></param>
        /// <returns></returns>
        public JsonResult SaveFileCorrectionByActivity(string sActivityId, string sFileCorrection)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sActivityId) && !string.IsNullOrEmpty(sFileCorrection))
                {
                    try
                    {
                        var saveFileCorrection = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { ACTIVITY_ID = sActivityId, FILE_CORRECTION = sFileCorrection },
                            TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionById), sAcademicYear);
                        ObjJsonData.Message = (saveFileCorrection.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.ErrorCode = (saveFileCorrection.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("TaskManagementController", "SaveFileCorrectionByActivity", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("TaskManagementController", "SaveFileCorrectionByActivity", ex.Message);
                        }
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchActivityFileCorrectionByActivityId(string sActivityId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(new AcademicActivityModel() { ACTIVITY_ID = sActivityId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAcademicActivityFileCorrectionById), sAcademicYear).DataSource.Data;
                    if (liActivity != null && liActivity.Count > 0)
                    {
                        return Json(liActivity, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagement", "FetchActivityFileCorrectionByActivityId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagement", "FetchActivityFileCorrectionByActivityId", ex.Message);
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
        public JsonResult UpdateFileCorrectionReturnStatus(string sActivityId, string sStatus)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sActivityId) && !string.IsNullOrEmpty(sStatus))
                {
                    try
                    {
                        var UpdateActivity = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { ACTIVITY_ID = sActivityId, FILE_CR_STATUS = sStatus },
                            TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionReturnStatusById), sAcademicYear);
                        ObjJsonData.ErrorCode = (UpdateActivity.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;


                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("TaskManagementController", "UpdateFileCorrectionReturnStatus", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("TaskManagementController", "UpdateFileCorrectionReturnStatus", ex.Message);
                        }
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveAcademicActivity(string sActivityId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var UpdateStatus = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { STATUS = Common.STATUS.Verified, ACTIVITY_ID = sActivityId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateStatus), sAcademicYear);
                    ObjJsonData.Message = (UpdateStatus.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagement", "ApproveAcademicActivity", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagement", "ApproveAcademicActivity", ex.Message);
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
        // Fetch Activity By ActivityId .
        public JsonResult FetchActivityByActivityId(string sActivityId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(new AcademicActivityModel() { ACTIVITY_ID = sActivityId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAcademicActivityById), sAcademicYear).DataSource.Data;
                    if (liActivity != null && liActivity.Count > 0)
                    {
                        return Json(liActivity, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagement", "FetchActivityByActivityId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagement", "FetchActivityByActivityId", ex.Message);
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
        // Save Activity .
        public JsonResult SaveAcademicActivity(string AcademicActivity)
        {
            JsonResultData objResult = new JsonResultData();
            AcademicActivityModel objModel = new AcademicActivityModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (AcademicActivity != null)
                    {
                        var Result = JsonConvert.DeserializeObject<AcademicActivityModel>(AcademicActivity);
                        Result.DATE_FROM = DateTime.Today.ToString("yyyy-MM-dd");
                        Result.SUBMISSION_DATE = CommonMethods.ConvertdatetoyearFromat(Result.SUBMISSION_DATE);
                        Result.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        var SaveAcademicActivity = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Result.ACTIVITY_ID)) ? TaskManagementSQLCommands.UpdateAcademicActivity : TaskManagementSQLCommands.SaveAcademicActivity), sAcademicYear, true);
                        objResult.Message = (SaveAcademicActivity.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        objResult.sResult = (!string.IsNullOrEmpty(Result.ACTIVITY_ID)) ? Result.ACTIVITY_ID : SaveAcademicActivity.RowUniqueId.ToString();
                        //    var sStaffIds = Result.STAFF_ID.Split(',');
                        //    if (sStaffIds!=null)
                        //    {
                        //        foreach (var item in sStaffIds)
                        //        {
                        //            Result.STAFF_ID = item;
                        //            var SaveAcademicActivity = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Result.ACTIVITY_ID)) ? TaskManagementSQLCommands.UpdateAcademicActivity : TaskManagementSQLCommands.SaveAcademicActivity), sAcademicYear);
                        //            objResult.Message = (SaveAcademicActivity.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        objResult.Message = "Please Select Staff To Continue !";
                        //    }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "SaveAcademicActivity", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "SaveAcademicActivity", ex.Message);
                }

            }
            return Json(objResult);
        }
        // List For User To Submit Files .
        public ActionResult GetActivityByUserId(string ActivityLevelId)
        {
            AcademicActivityModel objModel = new AcademicActivityModel();
            DashboardViewModel objViewModel = new DashboardViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    string sSQL = string.Empty;
                    objModel.ACTIVITY_LEVEL = ActivityLevelId;
                    sSQL = SQL.Dashboard.DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserIdAndActivityLevel).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                    objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(objModel, sSQL, sAcademicYear).DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "GetActivityByUserId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "GetActivityByUserId", ex.Message);
                }
            }
            return View(objViewModel);
        }
        // Save User Uploaded Files .
        public JsonResult SaveUploadedFiles()
        {
            JsonResultData ObjJsonData = new JsonResultData();
            AcademicActivityModel objModel = new AcademicActivityModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserName = (Session[Common.SESSION_VARIABLES.USERNAME] != null) ? Session[Common.SESSION_VARIABLES.USERNAME].ToString() : string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    string filePath = string.Empty;
                    string sImagePath = string.Empty;
                    string _sDirectorypath = string.Empty;
                    string sContent = string.Empty;
                    string sActivityId = string.Empty;
                    objModel.IS_SUBMITTED = Common.STATUS.Submitted;
                    objModel.STAFF_ID = sUserId;
                    sActivityId = Request.Form["ACTIVITY_ID"];
                    string sActivityTitle = Request.Form["ACTIVITY_TITLE"];
                    string Status = string.Empty;
                    //if (Status != "0" && !string.IsNullOrEmpty(Status))
                    //{
                    //    if (Status == Common.STATUS.Registered)
                    //        objModel.STATUS = (objModel.IS_SUBMITTED != "1") ? Common.STATUS.Registered : Common.STATUS.Submitted;
                    //    else
                    //        objModel.STATUS = Status;
                    //}
                    //else
                    // objModel.STATUS = (objModel.IS_SUBMITTED != "1") ? Common.STATUS.Registered : Common.STATUS.Submitted;

                    var i = 0;
                    if (Request.Files.Count > 0)
                    {
                        foreach (var item in Request.Files)
                        {
                            var File = Request.Files[i];
                            string fileName = File.FileName;
                            //var Img = ImgName.Split('.');
                            //string fileName = ImgName.Replace(Img[0], sUserName + "_" + sActivityId);
                            if (i == 0)
                            {
                                _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ACTIVITY_FILE_PATH + "\\" + sAcademicYear + "\\" + sActivityTitle;
                                if (!Directory.Exists(_sDirectorypath))
                                {
                                    Directory.CreateDirectory(_sDirectorypath);
                                }
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    filePath = _sDirectorypath + "\\" + fileName;
                                    sImagePath = "\\" + Common.URLPages.ACTIVITY_FILE_PATH + "\\" + sAcademicYear + "\\" + sActivityTitle + "\\" + fileName;
                                    File.SaveAs(filePath);
                                    objModel.FILE_PATH = sImagePath;
                                    var UpdateFilePath = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { FILE_PATH = sImagePath, ACTIVITY_ID = sActivityId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityFile), sAcademicYear);
                                    ObjJsonData.Message = (UpdateFilePath.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                            }
                            else
                            {
                                _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.ACTIVITY_IMAGE_PATH + "\\" + sAcademicYear + "\\" + sActivityTitle;
                                if (!Directory.Exists(_sDirectorypath))
                                {
                                    Directory.CreateDirectory(_sDirectorypath);
                                }
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    filePath = _sDirectorypath + "\\" + fileName;
                                    sImagePath = "\\" + Common.URLPages.ACTIVITY_IMAGE_PATH + "\\" + sAcademicYear + "\\" + sActivityTitle + "\\" + fileName;
                                    File.SaveAs(filePath);
                                    objModel.PHOTO_PATH = sImagePath;
                                    var UpdateImagePath = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { PHOTO_PATH = sImagePath, ACTIVITY_ID = sActivityId },
                                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityImage), sAcademicYear);
                                    ObjJsonData.Message = (UpdateImagePath.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                            }
                            i++;
                        }
                        if (!string.IsNullOrEmpty(ObjJsonData.Message))
                        {
                            var UpdateStatus = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { ACTIVITY_ID = sActivityId, STATUS = Common.STATUS.Submitted },
                                TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityStatus), sAcademicYear);
                            ObjJsonData.Message = (UpdateStatus.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }
                    }
                    else
                        ObjJsonData.Message = "Files Are Not Found ! :(";
                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
                    ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                    // return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagementController", "SaveUploadedFiles", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagementController", "SaveUploadedFiles", ex.Message);
                    //return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // List Activity Status For Staff .
        public ActionResult ListActivityStatusForStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new TaskManagementViewModel();
            var ObjModel = new AcademicActivityModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liActivityList = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(new AcademicActivityModel() { STAFF_ID = sUserId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchActivityStatusForStaff), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagementController", "ListActivityStatusForStaff", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagementController", "ListActivityStatusForStaff", ex.Message);
                    }
                }
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        public JsonResult SaveFileCorrectionReturnByActivity(string sActivityId, string sFileCorrectionReturn)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sActivityId) && !string.IsNullOrEmpty(sFileCorrectionReturn))
                {
                    try
                    {
                        var saveFileCorrectionReturn = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { ACTIVITY_ID = sActivityId, FILE_CORRECTION_RETURN = sFileCorrectionReturn },
                            TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionReturnById), sAcademicYear);
                        ObjJsonData.Message = (saveFileCorrectionReturn.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                        ObjJsonData.ErrorCode = (saveFileCorrectionReturn.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("TaskManagementController", "SaveFileCorrectionReturnByActivity", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("TaskManagementController", "SaveFileCorrectionReturnByActivity", ex.Message);
                        }
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchActivityFileCorrectionReturnByActivityId(string sActivityId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(new AcademicActivityModel() { ACTIVITY_ID = sActivityId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAcademicActivityFileCorrectionReturnById), sAcademicYear).DataSource.Data;
                    if (liActivity != null && liActivity.Count > 0)
                    {
                        return Json(liActivity, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TaskManagement", "FetchActivityFileCorrectionReturnByActivityId", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TaskManagement", "FetchActivityFileCorrectionReturnByActivityId", ex.Message);
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
        // Delete Activity .
        public JsonResult DeleteActvity(string sActivityId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sActivityId))
                {
                    try
                    {
                        var DeleteActivity = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { ACTIVITY_ID = sActivityId },
                            TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteAcademicActivity), sAcademicYear);
                        ObjJsonData.Message = (DeleteActivity.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("TaskManagementController", "DeleteActvity", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("TaskManagementController", "DeleteActvity", ex.Message);
                        }
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// update file correction status
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateFileCorrectionStatus(string sActivityId, string sStatus)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (!string.IsNullOrEmpty(sActivityId) && !string.IsNullOrEmpty(sStatus))
                {
                    try
                    {
                        var UpdateActivity = CMSHandler.SaveOrUpdate(new AcademicActivityModel() { ACTIVITY_ID = sActivityId, FILE_CORRECTION_STATUS = sStatus },
                            TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionStatusById), sAcademicYear);
                        ObjJsonData.ErrorCode = (UpdateActivity.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.ExpectationFailed;


                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("TaskManagementController", "UpdateFileCorrectionStatus", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("TaskManagementController", "UpdateFileCorrectionStatus", ex.Message);
                        }
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.ErrorMessage.BadRequest;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Notifications
        public ActionResult Notificaition()
        {
            stf_Personal_Info objStaffModel = new stf_Personal_Info();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByAcademicYear), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        objViewModel.ClassList = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }


                    var Programme = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme), sAcademicYear).DataSource.Data;
                    if (Programme != null && Programme.Count > 0)
                        objViewModel.ProgrammeList = new SelectList(Programme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);

                    // ProgrammeYear....
                    var ProgrammeYear = (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data;
                    if (ProgrammeYear != null && ProgrammeYear.Count > 0)
                    {
                        objViewModel.ProgrammeYear = new SelectList(ProgrammeYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }

                    // Programme Level ....
                    var liApplicationType = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationType)).DataSource.Data;
                    if (liApplicationType != null && liApplicationType.Count > 0)
                    {
                        objViewModel.ApplicationType = new SelectList(liApplicationType, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);
                    }

                    //Fetch staff list 
                    var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                    if (FetchStaff != null && FetchStaff.Count > 0)
                        objViewModel.StaffList = new SelectList(FetchStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);

                    //fetch department
                    var Department = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    objViewModel.DepartmentList = new SelectList(Department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);




                    var FetchNotificationLevel = (List<SUP_NOTIFICATION_LEVEL>)CMSHandler.FetchData<SUP_NOTIFICATION_LEVEL>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupNotificationLevel), sAcademicYear).DataSource.Data;
                    if (FetchNotificationLevel != null && FetchNotificationLevel.Count > 0)
                        objViewModel.NotificationLevelList = new SelectList(FetchNotificationLevel, Common.SUP_NOTIFICATION_LEVEL.NOTIFICATION_LEVEL_ID, Common.SUP_NOTIFICATION_LEVEL.NOTIFICATION_LEVEL);
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Assignment", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Assignment", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public JsonResult SaveNotification(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            AssignmentModel objModel = new AssignmentModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID] != null ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    if (sJsonData != null)
                    {
                        var Result = JsonConvert.DeserializeObject<List<NOTITFICATIONS>>(sJsonData);
                        int qCount = 0;
                        string sQuery = @"INSERT INTO notifications_" + sAcademicYear + "(NOTIFICATION_ROLE_TYPE_ID," +
                                   "ENTRY_ID," +
                                   "NOTIFICATION," +
                                   "USER_ID," +
                                   "CLASS_ID," +
                                   "PROGRAMME_GROUP_ID," +
                                   "PROGRAMME_YEAR," +
                                   "APP_TYPE_ID," +
                                   "NOTIFICATION_LEVEL_ID," +
                                   "NOTIFIED_DATE," +
                                   "ACADEMIC_YEAR," +
                                   "DEPARTMENT) VALUES";
                        foreach (var item in Result)
                        {
                            item.ENTRY_ID = sUserId;
                            item.USER_ID = !string.IsNullOrEmpty(item.USER_ID) ? item.USER_ID.ToString() : 0.ToString();
                            item.CLASS_ID = !string.IsNullOrEmpty(item.CLASS_ID) ? item.CLASS_ID.ToString() : 0.ToString();
                            item.PROGRAMME_GROUP_ID = !string.IsNullOrEmpty(item.PROGRAMME_GROUP_ID) ? item.PROGRAMME_GROUP_ID.ToString() : 0.ToString();
                            item.PROGRAMME_YEAR = !string.IsNullOrEmpty(item.PROGRAMME_YEAR) ? item.PROGRAMME_YEAR.ToString() : 0.ToString();
                            item.APP_TYPE_ID = !string.IsNullOrEmpty(item.APP_TYPE_ID) ? item.APP_TYPE_ID.ToString() : 0.ToString();
                            item.DEPARTMENT = !string.IsNullOrEmpty(item.DEPARTMENT) ? item.DEPARTMENT.ToString() : 0.ToString();

                            sQuery += "('" + item.NOTIFICATION_ROLE_TYPE_ID + "','" + item.ENTRY_ID + "','" + item.NOTIFICATION + "','" + item.USER_ID + "','" + item.CLASS_ID + "','" + item.PROGRAMME_GROUP_ID + "','" + item.PROGRAMME_YEAR + "'" +
                                ",'" + item.APP_TYPE_ID + "','" + item.NOTIFICATION_LEVEL_ID + "',now(),'" + sAcademicYear + "','" + item.DEPARTMENT + "'),";
                            qCount++;
                        }
                        if (qCount > 0)
                        {

                            sQuery = sQuery.Trim(',') + ";";
                            var SaveNotification = CMSHandler.SaveOrUpdate(null, sQuery, sAcademicYear);
                            if (SaveNotification.Success)
                            {
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                objResult.ErrorCode = Common.ErrorCode.Ok;
                            }

                            else
                            {
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                                objResult.ErrorCode = Common.ErrorCode.ExpectationFailed;
                            }

                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "SaveAssignment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "SaveAssignment", ex.Message);
                }

            }
            return Json(objResult);
        }



        #endregion

        #region Student Leave Request .

        // List Student Leave Request ...
        public ActionResult ListStudentLeaveRequest()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new TaskManagementViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liStuLeaveRequest = (List<STU_LEAVE_REQUEST>)CMSHandler.FetchData<STU_LEAVE_REQUEST>(new STU_LEAVE_REQUEST() { STUDENT_ID = sUserId },
                        TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.ListStudentLeaveRequest), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("ListStudentLeaveRequest", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("ListStudentLeaveRequest", "TaskManagement", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // Fetch Leave Request By Id ..
        public JsonResult FetchStudentLeaveRequestById(string sLeaveRequestId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liLeaveRequest = (List<STU_LEAVE_REQUEST>)CMSHandler.FetchData<STU_LEAVE_REQUEST>(new STU_LEAVE_REQUEST() { LEAVE_REQUEST_ID = sLeaveRequestId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchStudentLeaveRequestById), sAcademicYear).DataSource.Data;
                    if (liLeaveRequest != null && liLeaveRequest.Count > 0)
                    {
                        return Json(liLeaveRequest, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStudentLeaveRequestById", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStudentLeaveRequestById", "TaskManagement", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Add Student Leave Request ...
        public ActionResult AddStudentLeaveRequest(string sLeaveRequestId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new TaskManagementViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.ObjLeaveRequest.LEAVE_REQUEST_ID = sLeaveRequestId;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AddStudentLeaveRequest", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("AddStudentLeaveRequest", "TaskManagement", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // Save Student Leave Request ...
        public JsonResult SaveStudentLeaveRequest(string sSaveLeaveRequest)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveStudentLeaveRequest", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveStudentLeaveRequest", "TaskManagement", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Delete Student Leave Request ...
        public JsonResult DeleteStudentLeaveRequest(string sLeaveRequestId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DeleteStudentLeaveRequest", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DeleteStudentLeaveRequest", "TaskManagement", ex.Message);
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

        #region Image Correction List 
        // List Images By Class ..
        public ActionResult FetchImages()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new TaskManagementViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass), sAcademicYear).DataSource.Data;
                if (liClass != null && liClass.Count > 0)
                {
                    ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Bind Class Based Images .
        public ActionResult BindClassWiseImages(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new TaskManagementViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.liImageCorrection = (List<CORRECTION_IMAGES>)CMSHandler.FetchData<CORRECTION_IMAGES>(new SUP_CLASS() { CLASS_ID = sClassId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchImagesByClassId), sAcademicYear).DataSource.Data;
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion
        #region Event Registration
        //Event Registration Setting
        /// <summary>
        /// This is Action is used show Event registration settings
        /// </summary>
        /// <returns></returns>
        public ActionResult EventRegistratinSettings()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.Lieventregistrationsettings = (List<EVENT_REGISTRATION_SETTINGS>)CMSHandler.FetchData<EVENT_REGISTRATION_SETTINGS>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchEventRegistrationSetting), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EventRegistratinSettings", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EventRegistratinSettings", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchEventRegistrationById(string sEventsettingId)
        {
            var objModel = new EVENT_REGISTRATION_SETTINGS();

            if (!string.IsNullOrEmpty(sEventsettingId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.EVENT_SETTING_ID = sEventsettingId;
                    var Lieventsetting = (List<EVENT_REGISTRATION_SETTINGS>)CMSHandler.FetchData<EVENT_REGISTRATION_SETTINGS>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchEventRegistrationById), sAcademicYear).DataSource.Data;
                    if (Lieventsetting != null && Lieventsetting.Count > 0)
                    {
                        objModel.EVENT_SETTING_ID = Lieventsetting.FirstOrDefault().EVENT_SETTING_ID;
                        objModel.FROM_DATE = Lieventsetting.FirstOrDefault().FROM_DATE;
                        objModel.TO_DATE = Lieventsetting.FirstOrDefault().TO_DATE;
                        objModel.MAIN_EVENT_NAME = Lieventsetting.FirstOrDefault().MAIN_EVENT_NAME;
                        objModel.IS_ACTIVE = Lieventsetting.FirstOrDefault().IS_ACTIVE;

                    }
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchEventRegistrationById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateEventSetting(string sJsonData)
        {
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

                    var EventSetting = JsonConvert.DeserializeObject<EVENT_REGISTRATION_SETTINGS>(sJsonData);
                    if (EventSetting != null)
                    {

                        EventSetting.ACADEMIC_YEAR = sAcademicYear;
                        EventSetting.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(EventSetting.FROM_DATE);
                        EventSetting.TO_DATE = CommonMethods.ConvertdatetoyearFromat(EventSetting.TO_DATE);
                        var ResultArg = CMSHandler.SaveOrUpdate(EventSetting, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(EventSetting.EVENT_SETTING_ID) ? TaskManagementSQLCommands.UpdateEventSetting : TaskManagementSQLCommands.SaveeventSetting)));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateEventSetting", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEventSetting(string sEventsettingId)
        {
            if (!string.IsNullOrEmpty(sEventsettingId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new EVENT_REGISTRATION_SETTINGS() { EVENT_SETTING_ID = sEventsettingId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteEventSetting));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "DeleteEventSetting", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEventSettingIsActive(string sEventsettingId, string sValue)
        {
            if (!string.IsNullOrEmpty(sEventsettingId) && !string.IsNullOrEmpty(sValue))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new EVENT_REGISTRATION_SETTINGS() { EVENT_SETTING_ID = sEventsettingId, IS_ACTIVE = sValue }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateEventSettingIsActive));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "UpdateEventSettingIsActive", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        //Sup Event
        public ActionResult ListSupEvents()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.Lisubevents = (List<SUB_EVENTS>)CMSHandler.FetchData<SUB_EVENTS>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchSubevents), sAcademicYear).DataSource.Data;
                    var lieventsetting = (List<EVENT_REGISTRATION_SETTINGS>)CMSHandler.FetchData<EVENT_REGISTRATION_SETTINGS>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchEventSetting), sAcademicYear).DataSource.Data;
                    if (lieventsetting != null && lieventsetting.Count > 0)
                    {
                        objViewModel.EventSetting = new SelectList(lieventsetting, Common.EVENT_REGISTRATION_SETTINGS.EVENT_SETTING_ID, Common.EVENT_REGISTRATION_SETTINGS.MAIN_EVENT_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSupEvents", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSupEvents", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchSubEventsById(string sSubeventId)
        {
            var objModel = new SUB_EVENTS();

            if (!string.IsNullOrEmpty(sSubeventId))
            {
                try
                {
                    objModel.SUB_EVENT_ID = sSubeventId;
                    var Lieventsetting = (List<SUB_EVENTS>)CMSHandler.FetchData<SUB_EVENTS>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchSubeventsById)).DataSource.Data;
                    if (Lieventsetting != null && Lieventsetting.Count > 0)
                    {
                        objModel.SUB_EVENT_ID = Lieventsetting.FirstOrDefault().SUB_EVENT_ID;
                        objModel.ALLOTED_NO = Lieventsetting.FirstOrDefault().ALLOTED_NO;
                        objModel.SUB_EVENT_NAME = Lieventsetting.FirstOrDefault().SUB_EVENT_NAME;
                        objModel.MAIN_EVENT_ID = Lieventsetting.FirstOrDefault().MAIN_EVENT_ID;
                        objModel.MAIN_EVENT_NAME = Lieventsetting.FirstOrDefault().MAIN_EVENT_NAME;
                        objModel.IS_ACTIVE = Lieventsetting.FirstOrDefault().IS_ACTIVE;

                    }
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchSubEventsById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateSubEvents(string sJsonData)
        {
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

                    var Subevent = JsonConvert.DeserializeObject<SUB_EVENTS>(sJsonData);
                    if (Subevent != null)
                    {
                        var ResultArg = CMSHandler.SaveOrUpdate(Subevent, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Subevent.SUB_EVENT_ID) ? TaskManagementSQLCommands.UpdateSubEvents : TaskManagementSQLCommands.SaveSubEvents)));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateSubEvents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSubEvents(string sSubeventId)
        {
            if (!string.IsNullOrEmpty(sSubeventId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new SUB_EVENTS() { SUB_EVENT_ID = sSubeventId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteSubEvent));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "DeleteSubEvents", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        //Event Registration By Student
        public ActionResult StudentEventRegistration()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var lieventsetting = (List<EVENT_REGISTRATION_SETTINGS>)CMSHandler.FetchData<EVENT_REGISTRATION_SETTINGS>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchEventSetting), sAcademicYear).DataSource.Data;
                    if (lieventsetting != null && lieventsetting.Count > 0)
                    {
                        objViewModel.EventSetting = new SelectList(lieventsetting, Common.EVENT_REGISTRATION_SETTINGS.EVENT_SETTING_ID, Common.EVENT_REGISTRATION_SETTINGS.MAIN_EVENT_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("StudentEventRegistration", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("StudentEventRegistration", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public string LoadEvents(string sEvevntSettingId)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            ResultArgs resultArgs = new ResultArgs();
            string sResult = string.Empty;
            try
            {
                sResult = ValidateStudent(sEvevntSettingId);
                if (string.IsNullOrEmpty(sResult))
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        DataValue dv = new DataValue();
                        DataTable dt = new DataTable();
                        string sClassId = string.Empty;
                        string sShiftId = string.Empty;
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                        var liStudent = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                        if (liStudent != null && liStudent.Count > 0)
                        {
                            sClassId = liStudent.FirstOrDefault().CLASS_ID;
                            sShiftId = liStudent.FirstOrDefault().SHIFT_ID;
                        }
                        dv.Clear();
                        dv.Add(Common.EVENT_REGISTRATION_SETTINGS.EVENT_SETTING_ID, sEvevntSettingId, EnumCommand.DataType.String);
                        dv.Add(Common.STU_PERSONAL_INFO.SHIFT_ID, sShiftId, EnumCommand.DataType.String);
                        STU_EVENT_REGISTRATION objEvent = new STU_EVENT_REGISTRATION();
                        resultArgs = objViewModel.LoadEvents(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                        if (resultArgs.Success)
                        {
                            dt = resultArgs.DataSource.Table;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                sResult = string.Empty;

                                foreach (DataRow item in dt.Rows)
                                {

                                    var SumOfSubeventQuota = (List<STU_EVENT_REGISTRATION>)CMSHandler.FetchData<STU_EVENT_REGISTRATION>(new STU_EVENT_REGISTRATION() { SUB_EVENT_ID = item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID].ToString() }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchSumOfSubEventQuotaBySubEventId), sAcademicYear).DataSource.Data;
                                    if (SumOfSubeventQuota != null)
                                    {
                                        if (Convert.ToInt32(SumOfSubeventQuota.FirstOrDefault().ALLOTED_SEATS) > Convert.ToInt32(SumOfSubeventQuota.FirstOrDefault().SUMOFQUTOA))
                                        {
                                            sResult += "<div class='form-group'> <input type='radio'  name='events' id='" + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID].ToString() + "' value='" + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID] + "' > <label for='" + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID].ToString() + "'> " + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_NAME].ToString() + " [ <label class='label label-success'> Alloted-" + SumOfSubeventQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfSubeventQuota.FirstOrDefault().SUMOFQUTOA + "" + "</label> ]" + "</lable></div>";
                                        }
                                        else
                                        {
                                            sResult += "<div class='form-group'> <input type='radio' disabled   name='events' id='" + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID].ToString() + "' value='" + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID] + "' > <label for='" + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_ID].ToString() + "'> " + item[Common.EVENT_REGISTRATION_SETTINGS.SUB_EVENT_NAME].ToString() + " [ <label class='label label-success'> Alloted-" + SumOfSubeventQuota.FirstOrDefault().ALLOTED_SEATS + "</label> -" + "<label class='label label-warning'> Applied -" + SumOfSubeventQuota.FirstOrDefault().SUMOFQUTOA + "" + "</label>]- <label class='label label-danger'>Closed</label>" + "</lable></div>";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sResult = string.Empty;
                            }

                        }
                    }

                }

                else
                {
                    //sResult = "2";
                    sResult = @"<h5>Registered  Events : <i class='text-primary'> " + sResult + "</i></h5>";
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LoadEvents", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LoadEvents", "TaskManagement", ex.Message);
                }


            }
            return sResult;
        }
        public string ValidateStudent(string sEventsettingId)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            string sResult = string.Empty;

            string sCheckCourse = string.Empty;
            DataValue dv = new DataValue();
            ResultArgs resultArgs = new ResultArgs();

            try
            {
                dv.Clear();
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                dv.Add(Common.NME_STUDENT_REGISTRATION_2017.STUDENT_ID, Session[Common.SESSION_VARIABLES.USER_ID].ToString(), EnumCommand.DataType.String);
                dv.Add(Common.EVENT_REGISTRATION_SETTINGS.MAIN_EVENT_ID, sEventsettingId, EnumCommand.DataType.String);

                resultArgs = objViewModel.ValidateStudents(dv, (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "");
                sResult = resultArgs.StringResult;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ValidateStudent", "TaskManagement", "Err MSg : " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ValidateStudent", "TaskManagement", ex.Message);
                }
            }

            return sResult;

        }
        public JsonResult SaveEventRegistration(string sSubeventId, string sEventSettingId)
        {
            var objModel = new STU_EVENT_REGISTRATION();
            if (!string.IsNullOrEmpty(sSubeventId) && !string.IsNullOrEmpty(sEventSettingId))
            {
                try
                {
                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        objModel.SUB_EVENT_ID = sSubeventId;
                        objModel.MAIN_EVENT_ID = sEventSettingId;
                        objModel.ACADEMIC_YEAR = sAcademicYear;
                        objModel.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                        var ResultArg = CMSHandler.SaveOrUpdate(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveStudentEventRegistration));
                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveEventRegistration", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Meeting management
        /// <summary>
        /// This action is used to show list of meeting assigned
        /// </summary>
        /// <returns></returns>
        public ActionResult ListMeetingassigned()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.Limeetingassigned = (List<MEETING_ASSIGNED>)CMSHandler.FetchData<MEETING_ASSIGNED>(new MEETING_ASSIGNED() { ASSIGNED_BY = sUserId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchMeetingAssigned), sAcademicYear).DataSource.Data;
                    var Stafflist = (List<StaffList>)CMSHandler.FetchData<StaffList>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                    if (Stafflist != null && Stafflist.Count > 0)
                    {
                        objViewModel.StaffList = new SelectList(Stafflist, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListMeetingassigned", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListMeetingassigned", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchMeetingassigned(string meetingassignedid)
        {
            var objModel = new MEETING_ASSIGNED();
            string sOption = string.Empty;
            int count = 0;

            if (!string.IsNullOrEmpty(meetingassignedid))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.MEETING_ASSIGNED_ID = meetingassignedid;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.ASSIGNED_BY = sUserId;
                    var Limeetingassigned = (List<MEETING_ASSIGNED>)CMSHandler.FetchData<MEETING_ASSIGNED>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchMeetingAssignedById), sAcademicYear).DataSource.Data;
                    if (Limeetingassigned != null && Limeetingassigned.Count > 0)
                    {
                        objModel.MEETING_ASSIGNED_ID = Limeetingassigned.FirstOrDefault().MEETING_ASSIGNED_ID;
                        objModel.MEETING_NAME = Limeetingassigned.FirstOrDefault().MEETING_NAME;
                        objModel.TIME = Limeetingassigned.FirstOrDefault().TIME;
                        objModel.VENUE = Limeetingassigned.FirstOrDefault().VENUE;
                        objModel.DATE = Limeetingassigned.FirstOrDefault().DATE;

                    }
                    //Fetch Staff 
                    var Stafflist = (List<StaffList>)CMSHandler.FetchData<StaffList>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;

                    //Fetch Participants
                    var Liparticipants = (List<MEETING_PARTICIPANTS>)CMSHandler.FetchData<MEETING_PARTICIPANTS>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchMeetingParticipants), sAcademicYear).DataSource.Data;
                    if (Stafflist != null && Stafflist.Count > 0)
                    {
                        foreach (var staff in Stafflist)
                        {
                            foreach (var participant in Liparticipants)
                            {
                                if (staff.STAFF_ID == participant.PARTICIPANT_ID)
                                {
                                    sOption += "<option value=" + staff.STAFF_ID + " selected>" + staff.FIRST_NAME + "</option>";
                                    count++;
                                }
                            }
                            if (count <= 0)
                            {
                                sOption += "<option value=" + staff.STAFF_ID + ">" + staff.FIRST_NAME + "</option>";

                            }
                            count = 0;
                        }
                    }
                    objModel.PARTICIPANT_ID = sOption;
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchMeetingassigned", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateMeetingAssigned(string sJsonData)
        {
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Meetingassigned = JsonConvert.DeserializeObject<MEETING_ASSIGNED>(sJsonData);
                    if (Meetingassigned != null)
                    {
                        Meetingassigned.ASSIGNED_BY = sUserId;
                        Meetingassigned.ACADEMIC_YEAR = sAcademicYear;
                        var partipantsid = Meetingassigned.PARTICIPANT_ID.Split(',');
                        Meetingassigned.DATE = CommonMethods.ConvertdatetoyearFromat(Meetingassigned.DATE);
                        if (string.IsNullOrEmpty(Meetingassigned.MEETING_ASSIGNED_ID))
                        {

                            ResultArg = CMSHandler.SaveOrUpdate(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveMeetingAssigned), "", true);
                            Meetingassigned.MEETING_ID = ResultArg.RowUniqueId.ToString();
                            foreach (var participant in partipantsid)
                            {
                                Meetingassigned.PARTICIPANT_ID = participant;

                                if (ResultArg.Success)
                                {
                                    ResultArg = CMSHandler.SaveOrUpdate(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveMeetingParticipants));
                                }
                            }

                        }
                        else
                        {
                            ResultArg = CMSHandler.SaveOrUpdate(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateMeetingAssigned));
                            ResultArg = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            ResultArg = CMSHandler.SaveOrUpdate(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateMeetingassignedIsDeleted));
                            ResultArg = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                            //Meetingassigned.MEETING_ID = Meetingassigned.MEETING_ASSIGNED_ID;
                            foreach (var participant in partipantsid)
                            {
                                Meetingassigned.PARTICIPANT_ID = participant;
                                var CheckIsExist = (List<MEETING_ASSIGNED>)CMSHandler.FetchData<MEETING_ASSIGNED>(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.CheckIsParticipantExist), sAcademicYear).DataSource.Data;
                                if (CheckIsExist != null && CheckIsExist.Count > 0)
                                {
                                    Meetingassigned.MEETING_PARTICIPANT_ID = CheckIsExist.FirstOrDefault().MEETING_PARTICIPANT_ID;
                                    ResultArg = CMSHandler.SaveOrUpdate(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateIsDeleted));
                                }
                                else
                                {
                                    ResultArg = CMSHandler.SaveOrUpdate(Meetingassigned, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveMeetingParticipants));
                                }

                            }
                        }

                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateMeetingAssigned", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteMeetingAssigned(string meetingassignedid)
        {
            if (!string.IsNullOrEmpty(meetingassignedid))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new MEETING_ASSIGNED() { MEETING_ASSIGNED_ID = meetingassignedid }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteMeetingAssigned));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "DeleteMeetingAssigned", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        //Meeting Participant
        public ActionResult ListMeeting()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.Limeetingassigned = (List<MEETING_ASSIGNED>)CMSHandler.FetchData<MEETING_ASSIGNED>(new MEETING_ASSIGNED() { PARTICIPANT_ID = sUserId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchMeetingforParticipant), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListMeeting", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListMeeting", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion
        #region Task Management
        //Task List
        public ActionResult ListTasks()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.litasklist = (List<TASK_LIST>)CMSHandler.FetchData<TASK_LIST>(new TASK_LIST() { ASSIGNED_BY = sUserId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskLists), sAcademicYear).DataSource.Data;
                    var Stafflist = (List<StaffList>)CMSHandler.FetchData<StaffList>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                    if (Stafflist != null && Stafflist.Count > 0)
                    {
                        objViewModel.StaffList = new SelectList(Stafflist, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListTasks", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListTasks", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchTasklist(string taskid)
        {
            var objModel = new TASK_LIST();
            string sOption = string.Empty;
            int count = 0;

            if (!string.IsNullOrEmpty(taskid))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.TASK_ID = taskid;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.ASSIGNED_BY = sUserId;
                    var LiTasklist = (List<TASK_LIST>)CMSHandler.FetchData<TASK_LIST>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskListById), sAcademicYear).DataSource.Data;
                    if (LiTasklist != null && LiTasklist.Count > 0)
                    {
                        objModel.TASK_ID = LiTasklist.FirstOrDefault().TASK_ID;
                        objModel.TASK_NAME = LiTasklist.FirstOrDefault().TASK_NAME;
                        objModel.DESCRIPTION = LiTasklist.FirstOrDefault().DESCRIPTION;
                        objModel.SUB_DATE = LiTasklist.FirstOrDefault().SUB_DATE;

                    }
                    //Fetch Staff 
                    var Stafflist = (List<StaffList>)CMSHandler.FetchData<StaffList>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;

                    //Fetch Participants
                    var Litaskassigned = (List<TASK_ASSIGNED_LIST>)CMSHandler.FetchData<TASK_ASSIGNED_LIST>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskAssignedlist), sAcademicYear).DataSource.Data;
                    if (Stafflist != null && Stafflist.Count > 0)
                    {
                        foreach (var staff in Stafflist)
                        {
                            if (Litaskassigned != null && Litaskassigned.Count > 0)
                            {
                                foreach (var stf in Litaskassigned)
                                {
                                    if (staff.STAFF_ID == stf.STAFF_ID)
                                    {
                                        sOption += "<option value=" + staff.STAFF_ID + " selected>" + staff.FIRST_NAME + "</option>";
                                        count++;
                                    }
                                }
                            }
                            if (count <= 0)
                            {
                                sOption += "<option value=" + staff.STAFF_ID + ">" + staff.FIRST_NAME + "</option>";

                            }
                            count = 0;
                        }
                    }
                    objModel.STAFF_ID = sOption;
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchTasklist", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateTaskAssigned(string sJsonData)
        {
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Tasklist = JsonConvert.DeserializeObject<TASK_LIST>(sJsonData);
                    if (Tasklist != null)
                    {
                        Tasklist.ASSIGNED_BY = sUserId;
                        Tasklist.ACADEMIC_YEAR = sAcademicYear;
                        var staffid = Tasklist.STAFF_ID.Split(',');
                        Tasklist.SUB_DATE = CommonMethods.ConvertdatetoyearFromat(Tasklist.SUB_DATE);
                        if (string.IsNullOrEmpty(Tasklist.TASK_ID))
                        {

                            ResultArg = CMSHandler.SaveOrUpdate(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveTaskList), "", true);
                            Tasklist.TASK_ID = ResultArg.RowUniqueId.ToString();
                            foreach (var staff in staffid)
                            {
                                Tasklist.STAFF_ID = staff;

                                if (ResultArg.Success)
                                {
                                    ResultArg = CMSHandler.SaveOrUpdate(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveTaskAssigned));
                                }
                            }

                        }
                        else
                        {
                            ResultArg = CMSHandler.SaveOrUpdate(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateTaskList));
                            ResultArg = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                            ResultArg = CMSHandler.SaveOrUpdate(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateTaskAssignedIsDeleted));
                            ResultArg = CMSHandler.SaveOrUpdate(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SafeUpdateMode));
                            //Meetingassigned.MEETING_ID = Meetingassigned.MEETING_ASSIGNED_ID;
                            foreach (var staff in staffid)
                            {
                                Tasklist.STAFF_ID = staff;
                                var CheckIsExist = (List<TASK_ASSIGNED_LIST>)CMSHandler.FetchData<TASK_ASSIGNED_LIST>(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.CheckIsStaffExist), sAcademicYear).DataSource.Data;
                                if (CheckIsExist != null && CheckIsExist.Count > 0)
                                {
                                    Tasklist.TASK_ASSIGNED_ID = CheckIsExist.FirstOrDefault().TASK_ASSIGNED_ID;
                                    ResultArg = CMSHandler.SaveOrUpdate(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateIsDeletedInTaskAssigned));
                                }
                                else
                                {
                                    ResultArg = CMSHandler.SaveOrUpdate(Tasklist, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveTaskAssigned));
                                }

                            }
                        }

                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateTaskAssigned", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTaskList(string taskid)
        {
            if (!string.IsNullOrEmpty(taskid))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new TASK_LIST() { TASK_ID = taskid }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteTaskList));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "DeleteTaskList", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListTaskSubmitted()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.litasksubmissionlist = (List<TASK_SUBMISSION_LIST>)CMSHandler.FetchData<TASK_SUBMISSION_LIST>(new TASK_LIST() { ASSIGNED_BY = sUserId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskSubmittedList), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListTaskSubmitted", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListTaskSubmitted", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindSubmittedtaskdetails(string stasksubid)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();

            if (!string.IsNullOrEmpty(stasksubid))
            {
                try
                {
                    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                    {
                        string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                        objViewModel.litasksubmissionlist = (List<TASK_SUBMISSION_LIST>)CMSHandler.FetchData<TASK_SUBMISSION_LIST>(new TASK_SUBMISSION_LIST() { TASK_SUB_ID = stasksubid }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskSubmittedListById), sAcademicYear).DataSource.Data;

                    }
                    else
                        return RedirectToAction("ErrorMessage", "error");
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("BindSubmittedtaskdetails", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("BindSubmittedtaskdetails", "TaskManagement", ex.Message);
                    }
                }
            }
            return View(objViewModel);
        }
        //Task Submission
        public ActionResult ListSubmissionTask()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.litasklist = (List<TASK_LIST>)CMSHandler.FetchData<TASK_LIST>(new TASK_LIST() { STAFF_ID = sUserId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchSubmissionTask), sAcademicYear).DataSource.Data;


                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListSubmissionTask", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListSubmissionTask", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchTaskSubmission(string taskid)
        {
            var objModel = new TASK_LIST();
            string sOption = string.Empty;


            if (!string.IsNullOrEmpty(taskid))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.TASK_ID = taskid;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.STAFF_ID = sUserId;
                    var LiTasklist = (List<TASK_LIST>)CMSHandler.FetchData<TASK_LIST>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskSubmissionByTaskidandstaffId), sAcademicYear).DataSource.Data;
                    if (LiTasklist != null && LiTasklist.Count > 0)
                    {
                        objModel.TASK_ID = LiTasklist.FirstOrDefault().TASK_ID;
                        objModel.TASK_SUB_ID = LiTasklist.FirstOrDefault().TASK_SUB_ID;
                        objModel.TASK_NAME = LiTasklist.FirstOrDefault().TASK_NAME;
                        objModel.DESCRIPTION = LiTasklist.FirstOrDefault().DESCRIPTION;


                    }
                    objModel.STAFF_ID = sOption;
                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchTaskSubmission", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveSubmissionTask()
        {
            JsonResultData objResult = new JsonResultData();

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    string _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.FilePath.TASK_FILE;
                    if (!Directory.Exists(_sDirectorypath))
                    {
                        Directory.CreateDirectory(_sDirectorypath);
                    }

                    var File = Request.Files[0];

                    var Taskname = Request.Form["TASK_NAME"];
                    var Tasksubmission = Request.Form["sJsonData"];
                    string filePath = _sDirectorypath + "\\" + Session[Common.SESSION_VARIABLES.USER_CODE].ToString() + Taskname + "." + File.FileName.Split('.')[1];
                    File.SaveAs(filePath);
                    filePath = "\\" + Common.FilePath.TASK_FILE + "\\" + Session[Common.SESSION_VARIABLES.USER_CODE].ToString() + Taskname + "." + File.FileName.Split('.')[1];
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var Result = JsonConvert.DeserializeObject<TASK_LIST>(Tasksubmission);
                    Result.FILE_PATH = filePath;
                    Result.STAFF_ID = sUserId;
                    Result.SUB_DATE = DateTime.Today.ToString("yyyy-dd-MM");
                    // var FetchTaskSubmission = (List<TASK_SUBMISSION_LIST>)CMSHandler.FetchData<TASK_SUBMISSION_LIST>(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchTaskSubmissionByTaskIdandStaffId), sAcademicYear).DataSource.Data;
                    if (!string.IsNullOrEmpty(Result.TASK_SUB_ID))
                    {

                        var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateTaskSubmission), sAcademicYear);
                        if (SaveTaskSubmission.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveTaskSubmission), sAcademicYear);
                        if (SaveTaskSubmission.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "SaveSubmissionTask", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "SaveSubmissionTask", ex.Message);
                }

            }
            return Json(objResult);
        }

        #endregion
        #region Hall Resversation
        // Hall Details
        public ActionResult ListHalls()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.lihalls = (List<SUP_HALLS>)CMSHandler.FetchData<SUP_HALLS>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchHalls), sAcademicYear).DataSource.Data;
                    var Blocklist = (List<SUP_COLLEGE_BLOCK>)CMSHandler.FetchData<SUP_COLLEGE_BLOCK>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupCollgeBlock), sAcademicYear).DataSource.Data;
                    if (Blocklist != null && Blocklist.Count > 0)
                    {
                        objViewModel.BlockList = new SelectList(Blocklist, Common.SUP_COLLEGE_BLOCK.COLLEGE_BLOCK_ID, Common.SUP_COLLEGE_BLOCK.BLOCK_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ListHalls", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ListHalls", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchHallsById(string sHallId)
        {
            var objModel = new SUP_HALLS();
            string sOption = string.Empty;


            if (!string.IsNullOrEmpty(sHallId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.HALL_ID = sHallId;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    var Lisuphalls = (List<SUP_HALLS>)CMSHandler.FetchData<SUP_HALLS>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchHallsById), sAcademicYear).DataSource.Data;
                    if (Lisuphalls != null && Lisuphalls.Count > 0)
                    {
                        objModel.HALL_ID = Lisuphalls.FirstOrDefault().HALL_ID;
                        objModel.HALL_NAME = Lisuphalls.FirstOrDefault().HALL_NAME;
                        objModel.CAPACITY = Lisuphalls.FirstOrDefault().CAPACITY;
                        objModel.LOCATION = Lisuphalls.FirstOrDefault().LOCATION;
                        objModel.HALL_NO = Lisuphalls.FirstOrDefault().HALL_NO;
                        objModel.BLOCK_ID = Lisuphalls.FirstOrDefault().BLOCK_ID;
                        objModel.IMAGE = Lisuphalls.FirstOrDefault().IMAGE;
                        objModel.LOCATION = Lisuphalls.FirstOrDefault().LOCATION;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchHallsById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateHalls(string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    string _sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Common.URLPages.HALL_IMAGE;
                    if (!Directory.Exists(_sDirectorypath))
                    {
                        Directory.CreateDirectory(_sDirectorypath);
                    }
                    var halldetails = Request.Form["sJsonData"];
                    var Result = JsonConvert.DeserializeObject<SUP_HALLS>(halldetails);

                    var File = Request.Files[0];
                    if (File != null)
                    {
                        string filePath = _sDirectorypath + "\\" + Session[Common.SESSION_VARIABLES.USER_CODE].ToString() + File.FileName.Split('.')[0] + "." + File.FileName.Split('.')[1];
                        File.SaveAs(filePath);
                        filePath = "\\" + Common.URLPages.HALL_IMAGE + "\\" + Session[Common.SESSION_VARIABLES.USER_CODE].ToString() + File.FileName.Split('.')[0] + "." + File.FileName.Split('.')[1];
                        Result.IMAGE = filePath;
                    }
                    else
                        Result.IMAGE = string.Empty;


                    if (!string.IsNullOrEmpty(Result.HALL_ID))
                    {
                        if (!string.IsNullOrEmpty(Result.IMAGE))
                        {
                            var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateHalls), sAcademicYear);
                            if (SaveTaskSubmission.Success)
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                        else
                        {
                            var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateHallsWithNoFile), sAcademicYear);
                            if (SaveTaskSubmission.Success)
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                        }


                    }
                    else
                    {
                        var SaveTaskSubmission = CMSHandler.SaveOrUpdate(Result, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveHalls), sAcademicYear);
                        if (SaveTaskSubmission.Success)
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TaskManagement", "SaveSubmissionTask", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TaskManagement", "SaveSubmissionTask", ex.Message);
                }

            }
            return Json(objResult);
        }
        public JsonResult DeleteHalls(string sHallId)
        {
            if (!string.IsNullOrEmpty(sHallId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new SUP_HALLS() { HALL_ID = sHallId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteHalls));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "DeleteTaskList", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        //Hall Reservation by Staff
        public ActionResult HallReservation()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.lihallreservation = (List<HALL_RESERVATION>)CMSHandler.FetchData<HALL_RESERVATION>(new HALL_RESERVATION() { USER_ID = sUserId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchBookedHallsByUserId), sAcademicYear).DataSource.Data;
                    var Halllist = (List<SUP_HALLS>)CMSHandler.FetchData<SUP_HALLS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHalls), sAcademicYear).DataSource.Data;
                    if (Halllist != null && Halllist.Count > 0)
                    {
                        objViewModel.HallList = new SelectList(Halllist, Common.SUP_HALLS.HALL_ID, Common.SUP_HALLS.HALL_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HallReservation", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HallReservation", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchHallReservationById(string shallbookid)
        {
            var objModel = new HALL_RESERVATION();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(shallbookid))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.HALL_BOOK_ID = shallbookid;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Lihallreservation = (List<HALL_RESERVATION>)CMSHandler.FetchData<HALL_RESERVATION>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchHallReservationById), sAcademicYear).DataSource.Data;
                    if (Lihallreservation != null && Lihallreservation.Count > 0)
                    {
                        objModel.HALL_ID = Lihallreservation.FirstOrDefault().HALL_ID;

                        objModel.HALL_BOOK_ID = Lihallreservation.FirstOrDefault().HALL_BOOK_ID;
                        objModel.FROM_DATE = Lihallreservation.FirstOrDefault().FROM_DATE;
                        objModel.TO_DATE = Lihallreservation.FirstOrDefault().TO_DATE;

                        objModel.TO_TIME = Lihallreservation.FirstOrDefault().TO_TIME;
                        objModel.FROM_TIME = Lihallreservation.FirstOrDefault().FROM_TIME;
                        objModel.PURPOSE = Lihallreservation.FirstOrDefault().PURPOSE;
                        objModel.STATUS = Lihallreservation.FirstOrDefault().STATUS;


                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchHallsById", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateHallIsAvilable(string sJsonData)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    var Hallreservation = JsonConvert.DeserializeObject<HALL_RESERVATION>(sJsonData);
                    var LiHallbooked = (List<HALL_RESERVATION>)CMSHandler.FetchData<HALL_RESERVATION>(Hallreservation, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAlreadyBookedDetails)).DataSource.Data;
                    if (LiHallbooked != null && LiHallbooked.Count > 0)
                    {
                        foreach (var item in LiHallbooked)
                        {
                            sOption += "From Date:" + item.FROM_DATE + "To Date:" + item.TO_DATE + "From Time:" + item.FROM_TIME + "To Time:" + item.TO_TIME + " Book by " + @item.USER_NAME + "<br>";
                        }
                        return Json(sOption, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateTaskAssigned", ex.Message);
                    }

                }
            }
            return Json(sOption, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveorUpdateHallreservation(string sJsonData)
        {

            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {
                    string sTextMessage = string.Empty;
                    var liSendSMS = new List<MessageResult.sms>();
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Hallreservation = JsonConvert.DeserializeObject<HALL_RESERVATION>(sJsonData);
                    if (Hallreservation != null)
                    {
                        Hallreservation.USER_ID = sUserId;
                        Hallreservation.ACADEMIC_YEAR = sAcademicYear;

                        Hallreservation.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(Hallreservation.FROM_DATE);
                        Hallreservation.TO_DATE = CommonMethods.ConvertdatetoyearFromat(Hallreservation.TO_DATE);
                        if (Hallreservation.IS_AUTO_SCHEDULE == "1")
                        {
                            Hallreservation.STATUS = "1";
                            Hallreservation.APPROVAL_DATE = DateTime.Today.ToString("yyyy-dd-MM");
                        }


                        ResultArg = CMSHandler.SaveOrUpdate(Hallreservation, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Hallreservation.HALL_BOOK_ID) ? TaskManagementSQLCommands.UpdateHallReservation : TaskManagementSQLCommands.SaveHallReservation)));

                        if (ResultArg.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            if (Hallreservation.IS_AUTO_SMS_SEND == "1")
                            {
                                if (Hallreservation.IS_AUTO_SCHEDULE == "1")
                                    Hallreservation.STATUS_NAME = Common.HALL_RESERVATION_STATUS.APPROVED;
                                else
                                    Hallreservation.STATUS_NAME = Common.HALL_RESERVATION_STATUS.PENNDING;

                                var FetchUserInfo = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfo)).DataSource.Data;
                                if (FetchUserInfo != null && FetchUserInfo.Count > 0)
                                {

                                    var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.HallReservationSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicYear).DataSource.Data;
                                    sTextMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.FROM_DATE, Hallreservation.FROM_DATE).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.TO_DATE, Hallreservation.TO_DATE).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.FROM_TIME, Hallreservation.FROM_TIME).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.TO_TIME, Hallreservation.TO_TIME).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.HALL_NAME, Hallreservation.HALL_NAME).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.STATUS_NAME, Hallreservation.STATUS_NAME);
                                    var ObjControllerModel = new CommunicationController();
                                    string sCount = ObjControllerModel.ValidateCreditCount(sTextMessage.Length);
                                    ObjControllerModel.sCreditCount = sCount;
                                    ObjControllerModel.sSmsMode = Common.MessageType.HALL_RESERVATION;
                                    liSendSMS.Add(new MessageResult.sms() { to = FetchUserInfo.FirstOrDefault().MOBILE, message = sTextMessage, custom = "0", custom1 = "0", custom2 = "0", CreditCount = sCount });
                                    ObjControllerModel.liSMS = liSendSMS;
                                    ObjControllerModel.SendMessage();
                                }
                            }
                        }
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateTaskAssigned", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateHallReservationSetting()
        {
            var ObjModel = new HALL_RESERVATION_SETTING();
            try
            {
                var Reservationsetting = (List<HALL_RESERVATION_SETTING>)CMSHandler.FetchData<HALL_RESERVATION_SETTING>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchHallReservationSetting)).DataSource.Data;
                if (Reservationsetting != null && Reservationsetting.Count > 0)
                {
                    ObjModel.SETTING_ID = Reservationsetting.FirstOrDefault().SETTING_ID;

                    ObjModel.IS_AUTO_SCHEDULE = Reservationsetting.FirstOrDefault().IS_AUTO_SCHEDULE;
                    ObjModel.IS_AUTO_SMS_SEND = Reservationsetting.FirstOrDefault().IS_AUTO_SMS_SEND;

                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("ValidateHallReservationSetting", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("ValidateHallReservationSetting", "TaskManagement", ex.Message);
                }
            }
            return Json(ObjModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindHallReservationDetails(string sStartDate, string sEnddate)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            var objModel = new HALL_RESERVATION();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(sStartDate);
                    objModel.TO_DATE = CommonMethods.ConvertdatetoyearFromat(sEnddate);
                    objViewModel.lihallreservation = (List<HALL_RESERVATION>)CMSHandler.FetchData<HALL_RESERVATION>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchBookedHallByDate), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindHallReservationDetails", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindHallReservationDetails", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        //Hall Reservation(Approve/Reject) by Admin
        public ActionResult HallReservationMaintenance()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                    objViewModel.lihallreservation = (List<HALL_RESERVATION>)CMSHandler.FetchData<HALL_RESERVATION>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAllHallReservation), sAcademicYear).DataSource.Data;
                    var Halllist = (List<SUP_HALLS>)CMSHandler.FetchData<SUP_HALLS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupHalls), sAcademicYear).DataSource.Data;
                    if (Halllist != null && Halllist.Count > 0)
                    {
                        objViewModel.HallList = new SelectList(Halllist, Common.SUP_HALLS.HALL_ID, Common.SUP_HALLS.HALL_NAME);
                    }
                    var StatusList = (List<SUP_HALL_RESERVATION_STATUS>)CMSHandler.FetchData<SUP_HALL_RESERVATION_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHallreservationstatus), sAcademicYear).DataSource.Data;
                    if (StatusList != null && StatusList.Count > 0)
                    {
                        objViewModel.StatusList = new SelectList(StatusList, Common.SUP_HALL_RESERVATION_STATUS.STATUS_ID, Common.SUP_HALL_RESERVATION_STATUS.STATUS_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("HallReservationMaintenance", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("HallReservationMaintenance", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveorUPdateHallreservationStatus(string sHallbookid, string sStaus)
        {
            string sTextMessage = string.Empty;
            var liSendSMS = new List<MessageResult.sms>();
            var objModel = new HALL_RESERVATION();
            objModel.HALL_BOOK_ID = sHallbookid;
            objModel.STATUS = sStaus;
            if (!string.IsNullOrEmpty(sHallbookid) && !string.IsNullOrEmpty(sStaus))
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                try
                {
                    var Lihallreservation = (List<HALL_RESERVATION>)CMSHandler.FetchData<HALL_RESERVATION>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchHallReservationById), sAcademicYear).DataSource.Data;
                    var StatusList = (List<SUP_HALL_RESERVATION_STATUS>)CMSHandler.FetchData<SUP_HALL_RESERVATION_STATUS>(new SUP_HALL_RESERVATION_STATUS() { STATUS_ID = sStaus }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHallreservationstatusById), sAcademicYear).DataSource.Data;
                    if (StatusList != null && StatusList.Count > 0)
                    {
                        Lihallreservation.FirstOrDefault().STATUS_NAME = StatusList.FirstOrDefault().STATUS_NAME;
                    }
                    ResultArg = CMSHandler.SaveOrUpdate(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.UpdateHallreservationStatus), sAcademicYear);
                    if (ResultArg.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        if (Lihallreservation != null && Lihallreservation.Count > 0)
                        {
                            var FetchUserInfo = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = Lihallreservation.FirstOrDefault().USER_ID }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfo)).DataSource.Data;
                            if (FetchUserInfo != null && FetchUserInfo.Count > 0)
                            {

                                var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.HallReservationSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicYear).DataSource.Data;
                                sTextMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.FROM_DATE, Lihallreservation.FirstOrDefault().FROM_DATE).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.TO_DATE, Lihallreservation.FirstOrDefault().TO_DATE).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.FROM_TIME, Lihallreservation.FirstOrDefault().FROM_TIME).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.TO_TIME, Lihallreservation.FirstOrDefault().TO_TIME).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.HALL_NAME, Lihallreservation.FirstOrDefault().HALL_NAME).Replace(Common.Delimiter.AT + Common.HALL_RESERVATION.STATUS_NAME, Lihallreservation.FirstOrDefault().STATUS_NAME);
                                var ObjControllerModel = new CommunicationController();
                                string sCount = ObjControllerModel.ValidateCreditCount(sTextMessage.Length);
                                ObjControllerModel.sCreditCount = sCount;
                                ObjControllerModel.sSmsMode = Common.MessageType.HALL_RESERVATION;
                                liSendSMS.Add(new MessageResult.sms() { to = FetchUserInfo.FirstOrDefault().MOBILE, message = sTextMessage, custom = "0", custom1 = "0", custom2 = "0", CreditCount = sCount });
                                ObjControllerModel.liSMS = liSendSMS;
                                ObjControllerModel.SendMessage();
                            }
                        }

                    }
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUPdateHallreservationStatus", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Visitors Meeting and Appointment
        //Visitor Meeting Assignment 
        public ActionResult VisitorsMeetingAssignment()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    objViewModel.livisitorsmeetingassignment = (List<VISITORS_MEETING_ASSIGNMENT>)CMSHandler.FetchData<VISITORS_MEETING_ASSIGNMENT>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchVisitorsMeetingAssignment), sAcademicYear).DataSource.Data;
                    objViewModel.livisitorsmeetingappointment = (List<VISITORS_MEETING_APPOINTMENT>)CMSHandler.FetchData<VISITORS_MEETING_APPOINTMENT>(null, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchAllVisitorsMeetingAppointment), sAcademicYear).DataSource.Data;
                    var Stafflist = (List<StaffList>)CMSHandler.FetchData<StaffList>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffConcat)).DataSource.Data;
                    if (Stafflist != null && Stafflist.Count > 0)
                    {
                        objViewModel.StaffList = new SelectList(Stafflist, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.STAFF_NAME);
                    }
                    var VisitorCategory = (List<SUP_VISITORS_CATEGORY>)CMSHandler.FetchData<SUP_VISITORS_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchVisitorCategory), sAcademicYear).DataSource.Data;
                    if (VisitorCategory != null && VisitorCategory.Count > 0)
                    {
                        objViewModel.VisitorsCategory = new SelectList(VisitorCategory, Common.SUP_VISITORS_CATEGORY.VISITORS_CATEGORY_ID, Common.SUP_VISITORS_CATEGORY.VISITORS_CATEGORY_NAME);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("VisitorsMeetingAssignment", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("VisitorsMeetingAssignment", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchVisitorMeetingAssign(string sVisitormeetId)
        {
            var objModel = new VISITORS_MEETING_ASSIGNMENT();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sVisitormeetId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.VISITORS_MEET_ID = sVisitormeetId;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Livisitormeetingassign = (List<VISITORS_MEETING_ASSIGNMENT>)CMSHandler.FetchData<VISITORS_MEETING_ASSIGNMENT>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchVisitorMeetingAssignById), sAcademicYear).DataSource.Data;
                    if (Livisitormeetingassign != null && Livisitormeetingassign.Count > 0)
                    {
                        objModel.VISITORS_MEET_ID = Livisitormeetingassign.FirstOrDefault().VISITORS_MEET_ID;
                        objModel.VISITORS_NAME = Livisitormeetingassign.FirstOrDefault().VISITORS_NAME;
                        objModel.MOBILE = Livisitormeetingassign.FirstOrDefault().MOBILE;
                        objModel.VISITORS_CATEGORY = Livisitormeetingassign.FirstOrDefault().VISITORS_CATEGORY;

                        objModel.VISIT_DATE = Livisitormeetingassign.FirstOrDefault().VISIT_DATE;
                        objModel.STAFF_ID = Livisitormeetingassign.FirstOrDefault().STAFF_ID;
                        objModel.PURPOSE = Livisitormeetingassign.FirstOrDefault().PURPOSE;
                        objModel.IS_REQUIRED_APPOINMENT = Livisitormeetingassign.FirstOrDefault().IS_REQUIRED_APPOINMENT;
                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchVisitorMeetingAssign", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateVisitorsMeetingAssign(string sJsonData)
        {
            if (!string.IsNullOrEmpty(sJsonData))
            {
                try
                {

                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Visitormeetingassign = JsonConvert.DeserializeObject<VISITORS_MEETING_ASSIGNMENT>(sJsonData);
                    if (Visitormeetingassign != null)
                    {
                        Visitormeetingassign.ACADEMIC_YEAR = sAcademicYear;
                        Visitormeetingassign.VISIT_DATE = CommonMethods.ConvertdatetoyearFromat(Visitormeetingassign.VISIT_DATE);
                        ResultArg = CMSHandler.SaveOrUpdate(Visitormeetingassign, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Visitormeetingassign.VISITORS_MEET_ID) ? TaskManagementSQLCommands.UpdateVisitorMeetingAssign : TaskManagementSQLCommands.SaveVisitorMeetingAssign)));

                        if (ResultArg.Success)
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateVisitorsMeetingAssign", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteVisitorMeetingAssign(string sVisitormeetId)
        {
            if (!string.IsNullOrEmpty(sVisitormeetId))
            {
                try
                {

                    var ResultArg = CMSHandler.SaveOrUpdate(new VISITORS_MEETING_ASSIGNMENT() { VISITORS_MEET_ID = sVisitormeetId }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.DeleteVisitorMeetAssign));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "DeleteTaskList", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveVisitorCategory(string sVisitorCategory)
        {
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sVisitorCategory))
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    ResultArg = CMSHandler.SaveOrUpdate(new SUP_VISITORS_CATEGORY() { VISITORS_CATEGORY_NAME = sVisitorCategory }, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.SaveVisitorCategory));
                    if (ResultArg.Success)
                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;

                    var VisitorCategory = (List<SUP_VISITORS_CATEGORY>)CMSHandler.FetchData<SUP_VISITORS_CATEGORY>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchVisitorCategory), sAcademicYear).DataSource.Data;
                    if(VisitorCategory!=null && VisitorCategory.Count>0)
                    {
                        foreach(var item in VisitorCategory)
                        {
                            sOption += "<option value=" + item.VISITORS_CATEGORY_ID + ">" + @item.VISITORS_CATEGORY_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateVisitorsMeetingAssign", ex.Message);
                    }

                }
            }
            return Json(sOption, JsonRequestBehavior.AllowGet);
        }
        //Visitor Meeting Appointment
        public ActionResult VisitorsMeetingAppointment()
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
              
                    objViewModel.livisitorsmeetingappointment = (List<VISITORS_MEETING_APPOINTMENT>)CMSHandler.FetchData<VISITORS_MEETING_APPOINTMENT>(new VISITORS_MEETING_ASSIGNMENT() { STAFF_ID=sUserId}, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchVisitorsMeetingAppointment), sAcademicYear).DataSource.Data;
                    var StatusList = (List<SUP_HALL_RESERVATION_STATUS>)CMSHandler.FetchData<SUP_HALL_RESERVATION_STATUS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchVisitorsstatus), sAcademicYear).DataSource.Data;
                    if (StatusList != null && StatusList.Count > 0)
                    {
                        objViewModel.StatusList = new SelectList(StatusList, Common.SUP_HALL_RESERVATION_STATUS.STATUS_ID, Common.SUP_HALL_RESERVATION_STATUS.STATUS_NAME);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("VisitorsMeetingAppointment", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("VisitorsMeetingAppointment", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchVisitorMeetingappointment(string sVisitormeetId)
        {
            var objModel = new VISITORS_MEETING_APPOINTMENT();
            string sOption = string.Empty;
            if (!string.IsNullOrEmpty(sVisitormeetId))
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.VISITORS_MEET_ID = sVisitormeetId;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var livisitorappointment = (List<VISITORS_MEETING_APPOINTMENT>)CMSHandler.FetchData<VISITORS_MEETING_APPOINTMENT>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchVisitorMeetingAppointmentById), sAcademicYear).DataSource.Data;
                    if (livisitorappointment != null && livisitorappointment.Count > 0)
                    {
                        objModel.VISITORS_MEET_ID = livisitorappointment.FirstOrDefault().VISITORS_MEET_ID;
                        objModel.VISITORS_NAME = livisitorappointment.FirstOrDefault().VISITORS_NAME;                                        
                        objModel.APPOINTMENT_DATE = livisitorappointment.FirstOrDefault().APPOINTMENT_DATE;
                        objModel.APPOINTMENT_TIME = livisitorappointment.FirstOrDefault().APPOINTMENT_TIME;
                        objModel.REPLY = livisitorappointment.FirstOrDefault().REPLY;
                        objModel.STATUS = livisitorappointment.FirstOrDefault().STATUS;
                        objModel.MOBILE = livisitorappointment.FirstOrDefault().MOBILE;

                    }

                }
                catch (Exception ex)
                {

                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "FetchVisitorMeetingAssign", ex.Message);
                    }
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveorUpdateVisitorMeetingAppointment(string sJsonData)
        {
            if (!string.IsNullOrEmpty(sJsonData))
            {
                string sTextMessage = string.Empty;
                var liSendSMS = new List<MessageResult.sms>();
                try
                {

                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var Visitormeetingappointment = JsonConvert.DeserializeObject<VISITORS_MEETING_APPOINTMENT>(sJsonData);
                    if (Visitormeetingappointment != null)
                    {
                        Visitormeetingappointment.APPOINTMENT_DATE = CommonMethods.ConvertdatetoyearFromat(Visitormeetingappointment.APPOINTMENT_DATE);
                        ResultArg = CMSHandler.SaveOrUpdate(Visitormeetingappointment, TaskManagementSQL.GetTaskManagementSQL((!string.IsNullOrEmpty(Visitormeetingappointment.VISITORS_APPOINTMENT_ID) ? TaskManagementSQLCommands.UpdateVisitorMeetingAppointment : TaskManagementSQLCommands.SaveVisitorMeetingAppointment)));
                        if (ResultArg.Success)
                        {
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                            var MessageContent = (List<SMS_USERDEFINED_TEMPLATE>)CMSHandler.FetchData<SMS_USERDEFINED_TEMPLATE>(new SMS_USERDEFINED_TEMPLATE() { TEMPLATE_ID = Common.SMS_TEMPLATE.VisitorAppointmentSMS }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchsmsTemplateByTemplateId), sAcademicYear).DataSource.Data;

                            sTextMessage = MessageContent.FirstOrDefault().TEMPLATE_TEXT.Replace(Common.Delimiter.AT + Common.VISITORS_MEETING_APPOINTMENT.APPOINTMENT_DATE, Visitormeetingappointment.APPOINTMENT_DATE).Replace(Common.Delimiter.AT + Common.VISITORS_MEETING_APPOINTMENT.APPOINTMENT_TIME, Visitormeetingappointment.APPOINTMENT_TIME).Replace(Common.Delimiter.AT + Common.VISITORS_MEETING_APPOINTMENT.STATUS_NAME, Visitormeetingappointment.STATUS_NAME);
                            var ObjControllerModel = new CommunicationController();
                            string sCount = ObjControllerModel.ValidateCreditCount(sTextMessage.Length);
                            ObjControllerModel.sCreditCount = sCount;
                            ObjControllerModel.sSmsMode = Common.MessageType.HALL_RESERVATION;
                            liSendSMS.Add(new MessageResult.sms() { to = Visitormeetingappointment.MOBILE, message = sTextMessage, custom = "0", custom1 = "0", custom2 = "0", CreditCount = sCount });
                            ObjControllerModel.liSMS = liSendSMS;
                            ObjControllerModel.SendMessage();
                        }
                        else
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TaskManagement", "SaveorUpdateVisitorsMeetingAssign", ex.Message);
                    }

                }
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindVisitorMeetingandAppointment(string sStartDate,string sEndDate)
        {
            TaskManagementViewModel objViewModel = new TaskManagementViewModel();
            var objModel = new HALL_RESERVATION();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.FROM_DATE = CommonMethods.ConvertdatetoyearFromat(sStartDate);
                    objModel.TO_DATE = CommonMethods.ConvertdatetoyearFromat(sEndDate);
                    objViewModel.livisitorsmeetingappointment = (List<VISITORS_MEETING_APPOINTMENT>)CMSHandler.FetchData<VISITORS_MEETING_APPOINTMENT>(objModel, TaskManagementSQL.GetTaskManagementSQL(TaskManagementSQLCommands.FetchVisitormeetandappointmentByDate), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindVisitorMeetingandAppointment", "TaskManagement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindVisitorMeetingandAppointment", "TaskManagement", ex.Message);
                }
            }
            return View(objViewModel);
        }

        #endregion
    }
}