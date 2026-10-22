
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.SQL.SupportData;
using CMS.DAO;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using System.Web.Script.Serialization;
using CMS.Models;
using System.Threading.Tasks;
using CMS.SQL.TimeTable;
using System.Globalization;


namespace CMS.Controllers
{
    public class AttendanceController : Controller
    {
        string sSQL = string.Empty;
        // GET: Attendance
        //    [UserRights("STAFF")]
        JsonResultData ObjJsonData = new JsonResultData();
        public ActionResult Index()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    stf_Personal_Info staff = new stf_Personal_Info();
                    AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();

                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objAttendanceViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    // Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] = "1,3,5";
                    //staff.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    //sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassesByStaffIdAndActiveSemester);
                    //objAttendanceViewModel.Classes = (List<ClassModel>)CMSHandler.FetchData<ClassModel>(staff, sSQL, sAcademicYear).DataSource.Data;
                    //sSQL = string.Empty;
                    //sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHoursByStaffIdAndActiveSemester);
                    //objAttendanceViewModel.hourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(staff, sSQL, sAcademicYear).DataSource.Data;
                    //if (objAttendanceViewModel.hourList != null && objAttendanceViewModel.hourList.Count > 0)
                    //    objAttendanceViewModel.Hours = new SelectList(objAttendanceViewModel.hourList, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    return View(objAttendanceViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "Index", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public JsonResult FetchHoursByShift(string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<SUP_HOURS> liHours = new List<SUP_HOURS>();
                    List<SUP_HOURS> liAddHours = new List<SUP_HOURS>();
                    stf_Personal_Info staff = new stf_Personal_Info();
                    staff.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    staff.SHIFT = sShiftId;
                    sSQL = string.Empty;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHoursByStaffIdAndActiveSemester);
                    liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(staff, sSQL, sAcademicYear).DataSource.Data;
                    if (liHours != null && liHours.Count > 0)
                    {
                        // Remove Hours By Subtitute Hour ...,
                        var liRemoveHourBySubtituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(staff, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSubtituteHoursToRemove), sAcademicYear).DataSource.Data;
                        if (liRemoveHourBySubtituteHour != null && liRemoveHourBySubtituteHour.Count > 0)
                        {
                            foreach (var item in liRemoveHourBySubtituteHour)
                            {
                                liHours.RemoveAll(x => x.HOUR_ID == item.HOUR_ID);
                            }
                        }

                        // Add Subtitute Hours ...,
                        var liAddSubtituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(staff, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSubtituteHoursToAdd), sAcademicYear).DataSource.Data;
                        if (liHours != null && liHours.Count > 0)
                        {
                            if (liAddSubtituteHour != null && liAddSubtituteHour.Count > 0)
                            {
                                foreach (var item in liAddSubtituteHour)
                                {
                                    liHours.Add(new SUP_HOURS() { HOUR_ID = item.HOUR_ID, HOUR = item.HOUR, COURSE_ID = item.COURSE_ID, SEMESTER_ID = item.SEMESTER_ID, STAFF_ID = item.STAFF_ID });
                                }
                            }
                        }
                        else
                        {
                            if (liAddSubtituteHour != null && liAddSubtituteHour.Count > 0)
                            {
                                foreach (var item in liAddSubtituteHour)
                                {
                                    liAddHours.Add(new SUP_HOURS() { HOUR_ID = item.HOUR_ID, HOUR = item.HOUR, COURSE_ID = item.COURSE_ID, SEMESTER_ID = item.SEMESTER_ID, STAFF_ID = item.STAFF_ID });
                                }
                                liHours = liAddHours;
                            }
                        }

                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                        foreach (var item in liHours)
                        {
                            ObjJsonData.sResult += "<option value='" + item.HOUR_ID + "' cms-CourseId='" + item.COURSE_ID + "' cms-sStaffId='" + item.STAFF_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'> " + item.HOUR + " </option>";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;


                        // Add Subtitute Hour  ....
                        var liAddSubtituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(staff, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSubtituteHoursToAdd), sAcademicYear).DataSource.Data;
                        if (liAddSubtituteHour != null && liAddSubtituteHour.Count > 0)
                        {
                            foreach (var item in liAddSubtituteHour)
                            {
                                liAddHours.Add(new SUP_HOURS() { HOUR_ID = item.HOUR_ID, HOUR = item.HOUR, COURSE_ID = item.COURSE_ID, SEMESTER_ID = item.SEMESTER_ID, STAFF_ID = item.STAFF_ID });
                            }
                            liHours = liAddHours;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                            foreach (var item in liHours)
                            {
                                ObjJsonData.sResult += "<option value='" + item.HOUR_ID + "' cms-sStaffId='" + item.STAFF_ID + "' cms-CourseId='" + item.COURSE_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'> " + item.HOUR + " </option>";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "FetchHoursByShift", ex.Message);
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
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OverAllAttendance()
        {
            string sAcademicYear = string.Empty;
            AttendanceViewModel attendanceViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    attendanceViewModel.shifts = (List<Shift>)CMSHandler.FetchData<Shift>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    attendanceViewModel.Departments = (List<Department>)CMSHandler.FetchData<Department>(null,
                      SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentbyClasses), sAcademicYear).DataSource.Data;
                    // attendanceViewModel.staffList = (List<STAFF_PERSONAL>)CMSHandler.FetchData<STAFF_PERSONAL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                    // attendanceViewModel.hourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data;
                    //attendanceViewModel.SplAttendanceType = (List<SUP_SPL_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_SPL_ATTENDANCE_TYPE>(null, 
                    //    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSplAttendanceType)).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("OverAllAttendance", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("OverAllAttendance", "AttendanceController", ex.Message);
                }
            }
            return View(attendanceViewModel);
        }
        public JsonResult FetchClassByshiftandDepartmentID(string shift, string sdepart, string classYear, string classLevel)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(shift))
            {
                try
                {
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES(), SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifIdMuti).Replace("?DEPARTMENT", sdepart).Replace("?SHIFT", shift).Replace("?CLASS_YEAR", classYear).Replace("?CLASS_LEVEL", classLevel), sAcademicYear).DataSource.Data;

                    if (liClass != null && liClass.Count > 0)
                    {
                        foreach (var item in liClass)
                        {
                            ObjJsonData.sResult += "<option value='" + item.CLASS_ID + "' >" + item.CLASS_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "FetchClassByshiftandDepartmentID", "Err MSg: " + ex.Message, "Query is empty!");
                        ObjHandler.WriteError("AttendanceController", "FetchClassByshiftandDepartmentID", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }


            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.BadRequest;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchClassByshiftandDepartmentIDAttEntry(string shift, string sdepart)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(shift))
            {
                try
                {
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES(), SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifIdMutiAttEntry).Replace("?DEPARTMENT", sdepart).Replace("?SHIFT", shift), sAcademicYear).DataSource.Data;

                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value='0' > --- Select --- </option>";
                        foreach (var item in liClass)
                        {
                            ObjJsonData.sResult += "<option value='" + item.CLASS_ID + "' >" + item.CLASS_NAME + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "FetchClassByshiftandDepartmentID", "Err MSg: " + ex.Message, "Query is empty!");
                        ObjHandler.WriteError("AttendanceController", "FetchClassByshiftandDepartmentID", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                }


            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.BadRequest;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadStudentsForOverAll(string objInput)
        {
            var obj = new JavaScriptSerializer().Deserialize<OVER_ALL_ATTENDANCE>(objInput);
            AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
            TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();
            CALENDER_2017 Calender = new CALENDER_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objAttendanceViewModel.sDayOrder = (Session[Common.SESSION_VARIABLES.DAY_ORDER] != null) ? Session[Common.SESSION_VARIABLES.DAY_ORDER].ToString() : string.Empty;
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    string sClassId = string.Empty;
                    // Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] = "1,3,5";
                    timeTable.STAFF_ID = obj.StaffId;
                    timeTable.CLASS_ID = obj.ClassId;
                    timeTable.HOUR_ID = obj.HourId;
                    timeTable.SEMESTER_ID = obj.semesterId;
                    timeTable.COURSE_ID = obj.courseId;
                    timeTable.ATTENDANCE_DATE = CommonMethods.ConvertdatetoyearFromat(obj.AttendanceDate);
                    //timeTable.ATTENDANCE_DATE = DateTime.Today.ToString("yyyy-MM-dd");
                    if (obj.Flag.Equals("1"))
                    {
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentForAttendanceByStaffIdAndHourId);
                        objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;

                        // FETCH CLASS ID BY STAFF ID AND HOUR 

                        //sSQL = string.Empty;
                        //sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassByStaffIdAndHour).
                        //    Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //var FetchClass = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //if (FetchClass != null && FetchClass.Count > 0)
                        //{
                        //    // timeTable.CLASS_ID = FetchClass.FirstOrDefault().CLASS_ID.ToString();
                        //    sClassId = string.Join(",",FetchClass.Select(x=>x.CLASS_ID).ToArray());
                        //    //foreach (var item in FetchClass)
                        //    //{
                        //    //    sClassId += item.CLASS_ID + ",";
                        //    //}
                        //    //timeTable.CLASS_ID = sClassId.Remove(sClassId.Length - 1);
                        //}
                        //// FETCH STUDENTS FOR OPTIONAL COURSE
                        //sSQL = string.Empty;
                        //sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByOptionalCourseIdAndStaffId).
                        //    Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, timeTable.CLASS_ID);
                        //objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //if (objAttendanceViewModel.attendanceEntry == null)
                        //{
                        //    // FETCH STUDENTS FOR REGULAR COURSE
                        //    sSQL = string.Empty;
                        //    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffId).
                        //        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, timeTable.CLASS_ID);
                        //    objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //}
                        //if (objAttendanceViewModel.attendanceEntry == null)
                        //{
                        //    // FETCH STUDENTS FOR SPECIAL HOURS
                        //    sSQL = string.Empty;
                        //    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffIdForSpecialAttendance).
                        //        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);                            
                        //    objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //}
                        //// FETCH ATTENDANCE FOR OPTIONAL COURSE
                        ////objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchOptionalCourseStudentListForOverAllAttendance), sAcademicYear).DataSource.Data;
                        ////if (objAttendanceViewModel.attendanceEntry == null)
                        //// FETCH ATTENDANCE FOR SPECIAL COURSE
                        ////objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffIdForSpecialAttendance), sAcademicYear).DataSource.Data;
                        //if (objAttendanceViewModel.attendanceEntry == null)
                        //    // FETCH ATTENDANCE FOR REGULAR COURSE
                        //    objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, 
                        //        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForOverAllAttendance), sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable,
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentForSpecialAttendance), sAcademicYear).DataSource.Data;
                        if (objAttendanceViewModel.attendanceEntry == null)
                        {
                            Calender.DAY_ORDER_DATE = timeTable.ATTENDANCE_DATE;
                            var FetchDayOrder = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(Calender,
                                SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchdDayOrderByAttendanceDate), sAcademicYear).DataSource.Data;
                            if (FetchDayOrder != null && FetchDayOrder.Count > 0)
                            {
                                timeTable.DAY_ORDER_ID = FetchDayOrder.FirstOrDefault().DAY_ORDER;
                                var liAttEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable,
                                    SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByClassIdForSpecialAttendance), sAcademicYear).DataSource.Data;
                                if (liAttEntry != null && liAttEntry.Count > 0)
                                {
                                    objAttendanceViewModel.attendanceEntry = liAttEntry;
                                }
                            }
                        }

                    }
                    objAttendanceViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes)).DataSource.Data;
                    objAttendanceViewModel.reasons = (List<SUP_REASON>)CMSHandler.FetchData<SUP_REASON>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForAttendance)).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Attendance", "LoadStudentsForOverAll", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Attendance", "LoadStudentsForOverAll", Ex.Message);
                }

            }
            return PartialView("LoadStudentsForOverAll", objAttendanceViewModel);
        }
        public JsonResult FetchHoursByStaffIdAndDate(string sAttendanceDate, string sStaffId, string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            TT_TIMETABLE_2017 objModel = new TT_TIMETABLE_2017();
            List<SUP_HOURS> liHours = new List<SUP_HOURS>();

            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    objModel.ATTENDANCE_DATE = CommonMethods.ConvertdatetoyearFromat(sAttendanceDate);
                    objModel.STAFF_ID = sStaffId;
                    objModel.CLASS_ID = sClassId;
                    if (string.IsNullOrEmpty(sClassId))
                    {
                        liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(objModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHoursByStaffIdAndDate), sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(objModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHoursByStaffIdAndDateAndClassId), sAcademicYear).DataSource.Data;
                    }

                    if (liHours != null && liHours.Count > 0)
                    {
                        foreach (var item in liHours)
                            objResult.sResult += "<option value='" + item.HOUR_ID + "' cms-sCourseId='" + item.COURSE_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'>" + item.HOUR + "</option>";
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchHoursByStaffIdAndDate", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchHoursByStaffIdAndDate", "AttendanceController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchDayOrderByDate(string sAttendanceDate)
        {
            JsonResultData objResult = new JsonResultData();
            CALENDER_2017 Calender = new CALENDER_2017();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                Calender.DAY_ORDER_DATE = CommonMethods.ConvertdatetoyearFromat(sAttendanceDate);
                var FetchDayOrder = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(Calender,
                    SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchdDayOrderByAttendanceDate), sAcademicYear).DataSource.Data;
                if (FetchDayOrder != null && FetchDayOrder.Count > 0)
                    objResult.sResult = FetchDayOrder.FirstOrDefault().DAY_ORDER;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        // Fetch Staff By Class ...
        public JsonResult FetchStaffByClass(string sClassId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new STU_CLASS() { CLASS_ID = sClassId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffByClassId), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        foreach (var item in liStaff)
                        {
                            ObjJsonData.sResult += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
                        }
                    }
                    else
                    {
                        ObjJsonData.sResult += "<option value=0> No Records Available </option>";

                    }
                    ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchStaffByClass", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchStaffByClass", "AttendanceController", ex.Message);
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
        public JsonResult AttendanceEntry(string sJsonString, string IsAllPrasent, string sFlag, string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TimeTableSetting TimeTable = new TimeTableSetting();
            int SuccessCount = 0;
            int FailedCount = 0;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sJsonString))
                    {
                        var absentees = new ABSENTEE_LIST();
                        if (sFlag == "true")
                        {
                            absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonString);
                        }
                        else
                        {
                            absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonData);
                        }
                        DateTime date = DateTime.Today;
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                        if (absentees.ATT_ABSENTEE_LIST != null && absentees.ATT_ABSENTEE_LIST.Count > 0 && sFlag == "true")
                        {
                            foreach (var item in absentees.ATT_ABSENTEE_LIST)
                            {
                                if (!string.IsNullOrEmpty(item.ATTENDENCE_DATE))
                                {
                                    item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                                }
                                else
                                {
                                    item.ATTENDENCE_DATE = date.ToString("yyyy/MM/dd");
                                }
                                item.ENTRY_ID = sUserId;

                                if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveAbsenteeEntryByCourseStaff : AttendanceSQLCommands.UpdateAbsenteeEntryByCourseStaff), sAcademicYear).Success)
                                    SuccessCount++;
                                else
                                    FailedCount++;
                            }
                        }


                        // Save Staff Attendance Entry Done ...
                        if (SuccessCount != 0 || IsAllPrasent == "1")
                        {
                            var ObjModel = new ATT_ABSENTEE_LIST_2017();
                            ObjModel.ATTENDENCE_DATE = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().ATTENDENCE_DATE;
                            if (string.IsNullOrEmpty(ObjModel.ATTENDENCE_DATE))
                            {
                                ObjModel.ATTENDENCE_DATE = date.ToString("yyyy/MM/dd");
                            }
                            ObjModel.STAFF_ID = sUserId;
                            ObjModel.HOUR_FROM = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().HOUR_FROM;
                            ObjModel.HOUR_TO = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().HOUR_TO;
                            ObjModel.COURSE_ID = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().COURSE_ID;
                            ObjModel.STATUS = IsAllPrasent;
                            ObjModel.CLASS_ID = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().CLASS_ID;
                            ObjModel.ENTRY_ID = sUserId;
                            ObjModel.S_CLASS_ID = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().S_CLASS_ID;

                            var liCheckAttAbsentteesProccressed = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(ObjModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.IsExistAbsenteesProccessed), sAcademicYear).DataSource.Data;
                            if (liCheckAttAbsentteesProccressed != null && liCheckAttAbsentteesProccressed.Count > 0)
                            {
                                // Do Nothing , All Ready Record Inserted ,...
                            }
                            else
                            {
                                var SaveAttAbsentteesProccessed = CMSHandler.SaveOrUpdate(ObjModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.SaveAttAbsenteesProccessed), sAcademicYear);
                                if (SaveAttAbsentteesProccessed.Success)
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }

                        if (SuccessCount != 0)
                        {
                            objResult.Message = "(" + SuccessCount + ") ";
                            objResult.Message += Common.Messages.RecordsSavedSuccessfully + "\n";
                        }

                        if (FailedCount != 0)
                        {
                            objResult.Message += "(" + FailedCount + ")" + Common.Messages.FailedToSaveRecords;
                        }
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
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
                    objHandler.WriteError("AttendanceEntry", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AttendanceEntry", "AttendanceController", ex.Message);
                }
            }

            if (string.IsNullOrEmpty(objResult.Message))
            {
                objResult.Message = "No Changes Made !";
                objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CommonAttendanceEntry(string sJsonString, string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TimeTableSetting TimeTable = new TimeTableSetting();
            int SuccessCount = 0;
            int FailedCount = 0;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sJsonString))
                    {
                        var absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonString);
                        string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        foreach (var item in absentees.ATT_ABSENTEE_LIST)
                        {
                            if (item.ATTENDANCE_TYPE_ID == "1" && item.ATTENDANCE_ID != "")
                            {
                                item.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                                item.CLASS_ID = sClassId;
                                item.SEMESTER_TYPE = sSemesterType;
                                if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveCommonAbsenteeEntry : AttendanceSQLCommands.UpdateCommonAbsenteeEntry), sAcademicYear).Success)
                                {
                                    SuccessCount++;
                                    objResult.sResult = Convert.ToString(SuccessCount);
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    FailedCount++;
                                    objResult.sResult = Convert.ToString(FailedCount);
                                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            else if (item.ATTENDANCE_TYPE_ID != "1")
                            {
                                item.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                                item.CLASS_ID = sClassId;
                                item.SEMESTER_TYPE = sSemesterType;
                                if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveCommonAbsenteeEntry : AttendanceSQLCommands.UpdateCommonAbsenteeEntry), sAcademicYear).Success)
                                {
                                    SuccessCount++;
                                    objResult.sResult = Convert.ToString(SuccessCount);
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    FailedCount++;
                                    objResult.sResult = Convert.ToString(FailedCount);
                                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommonAttendanceEntry", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommonAttendanceEntry", "AttendanceController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        //[UserRights("STAFF")] 
        public ActionResult AbsenteesList()
        {
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sActiveSemester = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString();
                string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAbsentListByStaffId).
                    Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                var absenteesList = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(new TT_TIMETABLE_2017()
                { STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString() }, sSQL, sAcademicYear).DataSource.Data;
                return View(absenteesList);
            }
            else
                return RedirectToAction("ErrorMessage", "Error");
        }
        public ActionResult LoadStudentsForAttendance(string sClassId, string sStaffId, string sHourId, string sAttendanceDate, string sCourseId, string sSemesterId)
        {
            JsonResultData objResult = new JsonResultData();
            AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sHourId))
                    {
                        TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sSClassId = string.Empty;
                        string sStafId = (!string.IsNullOrEmpty(sStaffId)) ? sStaffId : Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                        timeTable.STAFF_ID = sStafId;
                        timeTable.CLASS_ID = sClassId;
                        timeTable.COURSE_ID = sCourseId;
                        timeTable.HOUR_ID = sHourId;
                        timeTable.SEMESTER_ID = sSemesterId;
                        timeTable.ATTENDANCE_DATE = DateTime.Today.ToString("yyyy-MM-dd");

                        // Fetch Students For Attendance ...
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentForAttendanceByStaffIdAndHourId);
                        var liAttendance = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        objAttendanceViewModel.attendanceEntry = liAttendance.OrderBy(x => x.ROLL_NO).ToList();
                        // FETCH CLASS ID BY STAFF ID AND HOUR 
                        //sSQL = string.Empty;
                        //sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassByStaffIdAndHour).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //var FetchClass = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //if (FetchClass != null && FetchClass.Count > 0)
                        //{
                        //    foreach (var item in FetchClass)
                        //    {
                        //        sSClassId += item.CLASS_ID + ",";
                        //    }
                        //    timeTable.CLASS_ID = sSClassId.Remove(sSClassId.Length - 1);
                        //}
                        //// FETCH STUDENTS FOR OPTIONAL COURSE
                        //sSQL = string.Empty;
                        //sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByOptionalCourseIdAndStaffId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, timeTable.CLASS_ID);
                        //objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //if (objAttendanceViewModel.attendanceEntry == null)
                        //{
                        //    // FETCH STUDENTS FOR REGULAR COURSE
                        //    sSQL = string.Empty;
                        //    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, timeTable.CLASS_ID);
                        //    objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //}
                        //if (objAttendanceViewModel.attendanceEntry == null)
                        //{
                        //    // FETCH STUDENTS FOR SPECIAL HOURS
                        //    sSQL = string.Empty;
                        //    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffIdForSpecialAttendance).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //    objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        //}
                        objAttendanceViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes)).DataSource.Data;
                        objAttendanceViewModel.reasons = (List<SUP_REASON>)CMSHandler.FetchData<SUP_REASON>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForAttendance)).DataSource.Data;
                        return PartialView("LoadStudentsForAttendance", objAttendanceViewModel);
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LoadStudentsForAttendance", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LoadStudentsForAttendance", "AttendanceController", ex.Message);
                    return PartialView("LoadStudentsForAttendance", objAttendanceViewModel);
                }
            }
        }
        public JsonResult FetchHoursByStaffId(string sClassId, string sAttendanceDate)
        {
            JsonResultData objResult = new JsonResultData();
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                {
                    List<TT_TIMETABLE_2017> timeTables = new List<TT_TIMETABLE_2017>();
                    TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();
                    string sStaffId, sDayOrderId = string.Empty;
                    sStaffId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    sDayOrderId = Session[Common.SESSION_VARIABLES.DAY_ORDER].ToString();
                    timeTable.DAY_ORDER_ID = sDayOrderId;
                    timeTable.STAFF_ID = sStaffId;
                    timeTable.CLASS_ID = sClassId;
                    timeTable.ATTENDANCE_DATE = CommonMethods.ConvertdatetoyearFromat(sAttendanceDate);
                    timeTables = (List<TT_TIMETABLE_2017>)CMSHandler.FetchData<TT_TIMETABLE_2017>(timeTable, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHoursByStaffIdAndDayOrder), Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()).DataSource.Data;
                    if (timeTables != null)
                    {
                        foreach (var item in timeTables)
                        {
                            sOption += "<option value='" + item.HOUR_ID + "'>" + item.HOUR_ID + "</option>";
                        }
                        objResult.ErrorCode = Common.ErrorCode.NotFound;
                        objResult.Message = Common.Messages.NoRecordsfound;
                        objResult.sResult = sOption;
                    }
                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = Common.Messages.Success;
                    objResult.sResult = sOption;

                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                    objResult.sResult = sOption;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("FetchHoursByStaffId", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchHoursByStaffId", "AttendanceController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        //Staff Based Attendance Entry
        public ActionResult StaffBasedAttendanceEntry()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {

                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    stf_Personal_Info staff = new stf_Personal_Info();
                    AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();

                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objAttendanceViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }

                    return View(objAttendanceViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("StaffBasedAttendanceEntry", "Index", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public JsonResult StaffBasedFetchHoursByShift(string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    List<SUP_HOURS> liHours = new List<SUP_HOURS>();
                    List<SUP_HOURS> liAddHours = new List<SUP_HOURS>();
                    stf_Personal_Info staff = new stf_Personal_Info();
                    staff.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    staff.SHIFT = sShiftId;
                    staff.PAPER_TYPE = Common.PAPER_TYPE.Value_Education;
                    sSQL = string.Empty;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchValueeducationHoursByStaffIdAndActiveSemester);
                    liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(staff, sSQL, sAcademicYear).DataSource.Data;
                    if (liHours != null && liHours.Count > 0)
                    {
                        // Remove Hours By Subtitute Hour ...,
                        var liRemoveHourBySubtituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(staff, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSubtituteHoursToRemove), sAcademicYear).DataSource.Data;
                        if (liRemoveHourBySubtituteHour != null && liRemoveHourBySubtituteHour.Count > 0)
                        {
                            foreach (var item in liRemoveHourBySubtituteHour)
                            {
                                liHours.RemoveAll(x => x.HOUR_ID == item.HOUR_ID);
                            }
                        }

                        // Add Subtitute Hours ...,
                        var liAddSubtituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(staff, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSubtituteHoursToAdd), sAcademicYear).DataSource.Data;
                        if (liHours != null && liHours.Count > 0)
                        {
                            if (liAddSubtituteHour != null && liAddSubtituteHour.Count > 0)
                            {
                                foreach (var item in liAddSubtituteHour)
                                {
                                    liHours.Add(new SUP_HOURS() { HOUR_ID = item.HOUR_ID, HOUR = item.HOUR, COURSE_ID = item.COURSE_ID, SEMESTER_ID = item.SEMESTER_ID, STAFF_ID = item.STAFF_ID });
                                }
                            }
                        }
                        else
                        {
                            if (liAddSubtituteHour != null && liAddSubtituteHour.Count > 0)
                            {
                                foreach (var item in liAddSubtituteHour)
                                {
                                    liAddHours.Add(new SUP_HOURS() { HOUR_ID = item.HOUR_ID, HOUR = item.HOUR, COURSE_ID = item.COURSE_ID, SEMESTER_ID = item.SEMESTER_ID, STAFF_ID = item.STAFF_ID });
                                }
                                liHours = liAddHours;
                            }
                        }

                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                        foreach (var item in liHours)
                        {
                            ObjJsonData.sResult += "<option value='" + item.HOUR_ID + "' cms-CourseId='" + item.COURSE_ID + "' cms-sStaffId='" + item.STAFF_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'> " + item.HOUR + " </option>";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;


                        // Add Subtitute Hour  ....
                        var liAddSubtituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(staff, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSubtituteHoursToAdd), sAcademicYear).DataSource.Data;
                        if (liAddSubtituteHour != null && liAddSubtituteHour.Count > 0)
                        {
                            foreach (var item in liAddSubtituteHour)
                            {
                                liAddHours.Add(new SUP_HOURS() { HOUR_ID = item.HOUR_ID, HOUR = item.HOUR, COURSE_ID = item.COURSE_ID, SEMESTER_ID = item.SEMESTER_ID, STAFF_ID = item.STAFF_ID });
                            }
                            liHours = liAddHours;
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                            foreach (var item in liHours)
                            {
                                ObjJsonData.sResult += "<option value='" + item.HOUR_ID + "' cms-sStaffId='" + item.STAFF_ID + "' cms-CourseId='" + item.COURSE_ID + "' cms-sSemesterId='" + item.SEMESTER_ID + "'> " + item.HOUR + " </option>";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "FetchHoursByShift", ex.Message);
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
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StaffBasedLoadStudentsForAttendance(string sClassId, string sStaffId, string sHourId, string sAttendanceDate, string sCourseId, string sSemesterId)
        {
            JsonResultData objResult = new JsonResultData();
            AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sHourId))
                    {
                        TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sSClassId = string.Empty;
                        string sStafId = (!string.IsNullOrEmpty(sStaffId)) ? sStaffId : Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                        timeTable.STAFF_ID = sStafId;
                        timeTable.CLASS_ID = sClassId;
                        timeTable.COURSE_ID = sCourseId;
                        timeTable.HOUR_ID = sHourId;
                        timeTable.SEMESTER_ID = sSemesterId;
                        timeTable.ATTENDANCE_DATE = DateTime.Today.ToString("yyyy-MM-dd");

                        // Fetch Students For Attendance ...
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStaffBasedStudentForAttendanceByStaffIdAndHourId);
                        objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;


                        objAttendanceViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes)).DataSource.Data;
                        objAttendanceViewModel.reasons = (List<SUP_REASON>)CMSHandler.FetchData<SUP_REASON>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForAttendance)).DataSource.Data;
                        return PartialView("LoadStudentsForAttendance", objAttendanceViewModel);
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LoadStudentsForAttendance", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LoadStudentsForAttendance", "AttendanceController", ex.Message);
                    return PartialView("LoadStudentsForAttendance", objAttendanceViewModel);
                }
            }
        }

        #region Attendance Entry For SJC
        public ActionResult AttendanceEntryForSJC()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    stf_Personal_Info staff = new stf_Personal_Info();
                    AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();

                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        objAttendanceViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    return View(objAttendanceViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "AttendanceEntryForSJC", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public ActionResult LoadStudentsForAttendanceEntry(string sClassId, string sStaffId, string sHourId, string sAttendanceDate, string sCourseId, string sSemesterId, string sShiftId)
        {
            JsonResultData objResult = new JsonResultData();
            AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sHourId))
                    {
                        TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sSClassId = string.Empty;
                        string sStafId = (!string.IsNullOrEmpty(sStaffId)) ? sStaffId : Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                        timeTable.STAFF_ID = sStafId;
                        timeTable.CLASS_ID = sClassId;
                        timeTable.COURSE_ID = sCourseId;
                        timeTable.HOUR_ID = sHourId;
                        timeTable.SEMESTER_ID = sSemesterId;
                        timeTable.SHIFT = sShiftId;
                        timeTable.ATTENDANCE_DATE = DateTime.Today.ToString("yyyy-MM-dd");

                        // Fetch Students For Attendance ...
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.LoadStudentsForAttendanceEntry);
                        var attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                        if (attendanceEntry != null && attendanceEntry.Count > 0)
                        {
                            objAttendanceViewModel.attendanceEntry = attendanceEntry.OrderBy(x => x.ROLL_NO).ToList();
                        }

                        objAttendanceViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes)).DataSource.Data;
                        objAttendanceViewModel.reasons = (List<SUP_REASON>)CMSHandler.FetchData<SUP_REASON>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForAttendance)).DataSource.Data;
                        return View(objAttendanceViewModel);
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LoadStudentsForAttendanceEntry", "AttendanceController", ex.Message);
                    return View(objAttendanceViewModel);
                }
            }
        }
        #endregion

        #region Attendance Admin View
        public ActionResult StudentAttendanceViewForAdmin()
        {
            AttendanceViewModel objViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchProgramme = (List<cp_Program_2017>)CMSHandler.FetchData<cp_Program_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgram), sAcademicYear).DataSource.Data;
                    if (FetchProgramme != null && FetchProgramme.Count > 0)
                        objViewModel.ProgrammeList = new SelectList(FetchProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);
                    List<cp_Classes_2017> listClass = new List<cp_Classes_2017>();
                    listClass.Add(new cp_Classes_2017() { CLASS_ID = "0", CLASS_NAME = "---Select---" });
                    objViewModel.ClassList = new SelectList(listClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");

            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Attendance", "StudentAttendanceViewForAdmin", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Attendance", "StudentAttendanceViewForAdmin", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult GetStudentListByClassId(string sProgrammeId)
        {
            JsonResultData objResult = new JsonResultData();
            cp_Classes_2017 objClassModel = new cp_Classes_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sOption = string.Empty;
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objClassModel.PROGRAMME = sProgrammeId;
                    var ClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objClassModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassesByProgrammeId), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {
                        sOption = "<option value='0'>--Select--</option>";
                        foreach (var item in ClassList)
                        {
                            sOption += "<option value='" + item.CLASS_ID + "'>" + item.CLASS_NAME + "</option>";
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
                    objHandler.WriteError("Attendance", "GetStudentListByClassId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Attendance", "GetStudentListByClassId", ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindAttendanceStatusByClassIdAndProgrammeId(string sProgrammeId, string sClassId)
        {
            AttendanceViewModel objViewModel = new AttendanceViewModel();
            Student_Personal_Info objModel = new Student_Personal_Info();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.CLASS_ID = sClassId;
                    var FetchAttendanceStatus = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(objModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendanceStatusByClassId), sAcademicYear).DataSource.Data;
                    if (FetchAttendanceStatus != null && FetchAttendanceStatus.Count > 0)
                        objViewModel.liAttendanceStaus = FetchAttendanceStatus;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Attendance", "BindAttendanceStatusByClassIdAndProgrammeId", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Attendance", "BindAttendanceStatusByClassIdAndProgrammeId", ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region Hour Wise Attendance Report
        public ActionResult HourWiseAttendanceReport()
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    stf_Personal_Info staff = new stf_Personal_Info();
                    AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
                    Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] = "1,3,5";
                    staff.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    objAttendanceViewModel.Classes = (List<ClassModel>)CMSHandler.FetchData<ClassModel>(staff, sSQL, sAcademicYear).DataSource.Data;
                    if (objAttendanceViewModel.Classes != null && objAttendanceViewModel.Classes.Count > 0)
                        objAttendanceViewModel.ClassList = new SelectList(objAttendanceViewModel.Classes, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    sSQL = string.Empty;
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours).Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    objAttendanceViewModel.hourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(staff, sSQL, sAcademicYear).DataSource.Data;
                    if (objAttendanceViewModel.hourList != null && objAttendanceViewModel.hourList.Count > 0)
                        objAttendanceViewModel.Hours = new SelectList(objAttendanceViewModel.hourList, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    return View(objAttendanceViewModel);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "HourWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public ActionResult LoadStudentsForAttendanceReport(string sClassId, string sHourId, string sAttendanceDate)
        {
            JsonResultData objResult = new JsonResultData();
            AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sHourId))
                    {
                        TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        timeTable.CLASS_ID = sClassId;
                        timeTable.ATTENDANCE_DATE = CommonMethods.ConvertdatetoyearFromat(sAttendanceDate);
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendanceReportByClassIdAndDate).Replace(Common.Delimiter.QUS + Common.SUP_HOURS.HOUR_ID, sHourId);
                        objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LoadStudentsForAttendanceReport", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LoadStudentsForAttendanceReport", "AttendanceController", ex.Message);
                    return View(objAttendanceViewModel);
                }
            }
            return View(objAttendanceViewModel);
        }
        #endregion

        #region Attendence Report By Date For Student
        // HourBasedAttendance Report For Student....
        public ActionResult HourBasedAttendanceReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                return View();
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }
        // Load Hour Based Attendance Report ...
        public async Task<ActionResult> LoadHourBasedAttendanceReport(string sDateFrom, string sDateTo)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    string sFromDate = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    string sToDate = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    // Fetch Regular Papers ....
                    string sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHourBasedCompulsaryPapersAttendanceReport);
                    ObjViewModel.liAttendanceReport = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(
                        new ATTENDANCE_REPORT() { DATE_FROM = sFromDate, DATE_TO = sToDate, STUDENT_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data);

                    // Fetch Optional Papers ....
                    sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHourBasedOptionalPapersAttendanceReport);
                    var liOptionalPapers = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(
                        new ATTENDANCE_REPORT() { DATE_FROM = sFromDate, DATE_TO = sToDate, STUDENT_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data;
                    if (liOptionalPapers != null && liOptionalPapers.Count > 0)
                    {
                        ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport != null) ? ObjViewModel.liAttendanceReport.Concat(liOptionalPapers).ToList() : liOptionalPapers;
                    }

                    // Fetch Student Info ....
                    ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new ATTENDANCE_REPORT() { STUDENT_ID = sUserId },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "LoadHourBasedAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        // Hour Based Attendance For Parent ...
        public ActionResult HourBasedAttendanceReportForParent()
        {
            var ObjViewModel = new AttendanceViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.STUDENT_ID] != null) ? Session[Common.SESSION_VARIABLES.STUDENT_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                return View(ObjViewModel);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }

        public async Task<ActionResult> LoadHourBasedAttendanceReportForParent(string sDateFrom, string sDateTo)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.STUDENT_ID] != null) ? Session[Common.SESSION_VARIABLES.STUDENT_ID].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    string sFromDate = CommonMethods.ConvertdatetoyearFromat(sDateFrom);
                    string sToDate = CommonMethods.ConvertdatetoyearFromat(sDateTo);
                    // Fetch Regular Papers ....
                    string sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHourBasedCompulsaryPapersAttendanceReport);
                    ObjViewModel.liAttendanceReport = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(
                         new ATTENDANCE_REPORT() { DATE_FROM = sFromDate, DATE_TO = sToDate, STUDENT_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data);

                    // Fetch Optional Papers ....
                    sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHourBasedOptionalPapersAttendanceReport);
                    var liOptionalPapers = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(
                        new ATTENDANCE_REPORT() { DATE_FROM = sFromDate, DATE_TO = sToDate, STUDENT_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data;
                    if (liOptionalPapers != null && liOptionalPapers.Count > 0)
                    {
                        ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport != null) ? ObjViewModel.liAttendanceReport.Concat(liOptionalPapers).ToList() : liOptionalPapers;
                    }

                    // Fetch Student Info ....
                    ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new ATTENDANCE_REPORT() { STUDENT_ID = sUserId },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;


                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "LoadHourBasedAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region ClassWise Attendance Report
        // Class Wise Attendance Report ...
        public ActionResult ClassWiseAttendanceReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass)).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "ClassWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Load Student For ClassWise Attendance Report ...
        public async Task<ActionResult> LoadClassWiseStudentReport(string sClassId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    // Check If Common Attendance Exist ...
                    var liCommonExist = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { CLASS_ID = sClassId, DATE = sDate },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.IsCommonExistForClassWiseAttendence), sAcademicYear).DataSource.Data;
                    if (liCommonExist != null && liCommonExist.Count > 0)
                    {
                        // Fetch Common Attendance ....
                        var liCommonAttendance = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { CLASS_ID = sClassId, DATE = sDate },
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClasswiseAttendanceReportForCommon), sAcademicYear).DataSource.Data;
                        if (liCommonAttendance != null && liCommonAttendance.Count > 0)
                        {
                            ObjViewModel.liAttendanceReportForOptionalPapers = liCommonAttendance.OrderBy(x => x.ROLL_NO).ToList();
                        }
                    }
                    else
                    {
                        // Fetch optional Courses ...
                        var liOptionalCourseList = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { CLASS_ID = sClassId, DATE = sDate },
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassWiseAttendanceReportForOptionalCourse), sAcademicYear).DataSource.Data);

                        // Fetch Regular Course Student By Class Id ....
                        ObjViewModel.liAttendanceReport = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = sDate, CLASS_ID = sClassId },
                           SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassWiseAttendanceReport), sAcademicYear).DataSource.Data);

                        // Fetch Value Education Course By Class Id ...
                        var liValueEducation = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = sDate, CLASS_ID = sClassId },
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchvalueEducationCourseByClassId), sAcademicYear).DataSource.Data);

                        if (ObjViewModel.liAttendanceReport != null && ObjViewModel.liAttendanceReport.Count > 0 && liOptionalCourseList != null && liOptionalCourseList.Count > 0)
                        {
                            ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liOptionalCourseList : ObjViewModel.liAttendanceReport.Concat(liOptionalCourseList).OrderBy(x => x.HOUR_ID).ToList();
                            if (liValueEducation != null && liValueEducation.Count > 0)
                            {
                                ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liValueEducation : ObjViewModel.liAttendanceReport.Concat(liValueEducation).OrderBy(x => x.HOUR_ID).ToList();
                            }
                            ObjViewModel.sDate = sDate;
                        }
                        else
                        {
                            ObjViewModel.liAttendanceReportForOptionalPapers = liOptionalCourseList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "LoadClassWiseStudentReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region ClassWise Attendance Report For Course Staff
        // Staffwise Attendace Report ....
        public ActionResult ClassWiseAttendanceReportForStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            STAFFPROFILEVIEW objModel = new STAFFPROFILEVIEW();
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Classes By UserId ...
                    objModel.STAFF_ID = sUserId;
                    var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassByStaffId), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "StaffWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Fetch Course By ClassId & StaffId ....
        public JsonResult FetchCourseByClassAndStaffId(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            JsonResultData ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFetchCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CourseWiseStaffList() { STAFF_ID = sUserId, CLASS_ID = sClassId },
                   SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCourseByClassAndStaffId), sAcademicYear).DataSource.Data;
                    if (liFetchCourse != null && liFetchCourse.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liFetchCourse)
                        {
                            ObjJsonData.sResult += "<option value='" + item.COURSE_ID + "'> " + item.COURSE_TITLE + " </option>";
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "FetchCourseByClassAndStaffId", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Load StaffWise Attendance Report ...
        public ActionResult LoadStaffWiseAttendanceReport(string sClassId, string sCourseId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStaffWiseAttendanceReport).
                        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    // Fetch Attendance Report By Class & Course ...
                    ObjViewModel.liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { CLASS_ID = sClassId, COURSE_ID = sCourseId, DATE = sDate, STAFF_ID = sUserId },
                                  sSQL, sAcademicYear).DataSource.Data;

                    ObjViewModel.UserName = (Session[Common.SESSION_VARIABLES.USERNAME] != null) ? Session[Common.SESSION_VARIABLES.USERNAME].ToString() : string.Empty;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "StaffWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region DayWise Attendance Report ...
        public ActionResult DayWiseAttendanceReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "DayWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View();
        }

        // Bind Student Info By ClassId & StudentId & Date ...
        public ActionResult BindStuentsForDayWiseAttendanceReport(string sDate)
        {
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sDate);

                    // Fetch Regular Papers ......
                    string sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchDayWiseAttendanceReportForRegularPapers).
                        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    ObjViewModel.liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date },
                        sSQL, sAcademicYear).DataSource.Data;

                    //Fetch Optional Papers .....
                    sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchDayWiseAttendanceReportForOptionalPapers).
                        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    var liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date },
                          sSQL, sAcademicYear).DataSource.Data;
                    if (liAttendanceReport != null && liAttendanceReport.Count > 0)
                    {
                        ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liAttendanceReport : ObjViewModel.liAttendanceReport.Concat(liAttendanceReport).OrderBy(x => x.HOUR_ID).ToList();
                    }

                    // Fetch Value Education Papers ....
                    var liValueEducation = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date },
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchValueEducationCourseByStudentId), sAcademicYear).DataSource.Data;
                    if (liValueEducation != null && liValueEducation.Count > 0)
                    {
                        ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liValueEducation : ObjViewModel.liAttendanceReport.Concat(liValueEducation).ToList();
                    }
                    ObjViewModel.sDate = sDate;

                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "BindStuentsForDayWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Student Based Attendance Report
        // Student Based Attendance Report ....
        public ActionResult StudentWiseAttendanceReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "DayWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Fetch Student By ClassId ...
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

        // Bind Student Info By ClassId & StudentId & Date ...
        public async Task<ActionResult> BindStuentsForAttendanceReport(string sClassId, string sStudentId, string sDate)
        {
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.sDate = sDate;
                    string Date = CommonMethods.ConvertdatetoyearFromat(sDate);
                    var liAttendanceReport = new List<ATTENDANCE_REPORT>();
                    var liAttendanceReportForOptional = new List<ATTENDANCE_REPORT>();
                    var liCommonAttendance = new List<ATTENDANCE_REPORT>();
                    var liValueEducation = new List<ATTENDANCE_REPORT>();
                    var sStudentIds = sStudentId.Split(',');
                    int count = 0;
                    foreach (var item in sStudentIds)
                    {
                        // Fetch Value Education Papers .....
                        liValueEducation = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date, STUDENT_ID = item, CLASS_ID = sClassId },
                           SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchValueEducationCourseByStudentId), sAcademicYear).DataSource.Data);
                        // Fetch Optional Papers ....
                        liAttendanceReportForOptional = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date, STUDENT_ID = item, CLASS_ID = sClassId },
                          SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHourBasedOptionalPapersAttendanceReport), sAcademicYear).DataSource.Data);
                        // Fetch Regular Papers ....
                        liAttendanceReport = await Task.Run(() => (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date, STUDENT_ID = item, CLASS_ID = sClassId },
                         SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHourBasedCompulsaryPapersAttendanceReport), sAcademicYear).DataSource.Data);

                        // Fetch Common Attendance ....
                        liCommonAttendance = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { DATE = Date, STUDENT_ID = item },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendanceReportForCumulative), sAcademicYear).DataSource.Data;

                        // Fetch Student Info .... 
                        var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new ATTENDANCE_REPORT() { STUDENT_ID = item },
                           SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentInfoByStudentId), sAcademicYear).DataSource.Data;
                        // Concat Student Info ....
                        if (liStudent != null && liStudent.Count > 0)
                        {
                            ObjViewModel.liStudent = (ObjViewModel.liStudent == null) ? liStudent : ObjViewModel.liStudent.Concat(liStudent).ToList();
                        }

                        // Concat Regular Papers ...
                        if (liAttendanceReport != null && liAttendanceReport.Count > 0)
                        {
                            ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liAttendanceReport : ObjViewModel.liAttendanceReport.Concat(liAttendanceReport).ToList();

                            // Concat Value Education Papers ...
                            if (liValueEducation != null && liValueEducation.Count > 0)
                            {
                                ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liValueEducation : ObjViewModel.liAttendanceReport.Concat(liValueEducation).ToList();
                            }
                        }

                        // Concat Optional Papers ...
                        if (liAttendanceReportForOptional != null && liAttendanceReportForOptional.Count > 0)
                        {
                            ObjViewModel.liAttendanceReport = (ObjViewModel.liAttendanceReport == null) ? liAttendanceReportForOptional : ObjViewModel.liAttendanceReport.Concat(liAttendanceReportForOptional).ToList();
                        }

                        // Concat Common Attendance ....
                        if (liCommonAttendance != null && liCommonAttendance.Count > 0)
                        {
                            ObjViewModel.liCommonAttendance = (ObjViewModel.liCommonAttendance == null) ? liCommonAttendance : ObjViewModel.liCommonAttendance.Concat(liCommonAttendance).ToList();
                        }
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "BindStuentsForDayWiseAttendanceReport", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region RFID ENTRIES
        public ActionResult AttendanceEntryByInput()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                //try
                //{

                //}
                //catch (Exception ex)
                //{
                //    using (ErrorLog objHandler = new ErrorLog())
                //    {
                //        objHandler.WriteError("AttendanceEntryByInput", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                //        objHandler.WriteError("AttendanceEntryByInput", "AttendanceController", ex.Message);
                //    }
                //}
                return View();
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

        }

        // Fetch Class & Course For Staff ...
        public JsonResult FetchClassAndCourse(string sRFID)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            StaffViewModel ObjViewModel = new StaffViewModel();
            string sAcaedemicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcaedemicYear))
            {
                try
                {
                    sRFID = sRFID.TrimEnd(',');
                    var liClassCourse = (List<ClassCourse>)CMSHandler.FetchData<ClassCourse>(new ClassCourse() { RFID = sRFID }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassAndCourseByRFID), sAcaedemicYear).DataSource.Data;
                    if (liClassCourse != null && liClassCourse.Count > 0)
                    {
                        foreach (var item in liClassCourse)
                        {
                            ObjJsonData.sResult += "<div class='form-group'> <input type='radio' cms-StaffId='" + item.STAFF_ID + "' cms-Hour='" + item.HOUR_ID + "'  cms-classid='" + item.CLASS_ID + "' cms-courseid='" + item.COURSE_ID + "'name='RFID' id='" + item.COURSE_ID + "' val='1' > <label for='" + item.COURSE_ID + "'> " + item.COURSE_TITLE + " (" + item.CLASS_NAME + ")</lable></div>";
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchClassAndCourseForStaff", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchClassAndCourseForStaff", "AttendanceController", ex.Message);
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

        // Remove Prasent Student  By RFID ...
        public ActionResult RemovePrasentStudents(string ClassId, string sCourseId, string sRFID)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            StaffViewModel ObjViewModel = new StaffViewModel();
            string sAcaedemicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcaedemicYear))
            {
                try
                {
                    string sJsonData = string.Empty;
                    var sRFIDS = sRFID.Split(',').ToList();
                    var liAbsentees = new List<ListStudents>();
                    var liStudentList = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(new ListStudents() { CLASS_ID = ClassId, COURSE_ID = sCourseId }, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchStudentByCourseIdAndClassId), sAcaedemicYear).DataSource.Data;
                    if (liStudentList != null && liStudentList.Count > 0)
                    {
                        liAbsentees = liStudentList;
                        // Remove Prasent Students ...
                        foreach (var item in sRFIDS)
                        {
                            if (item != "" && item != null)
                                liAbsentees.RemoveAll(x => x.RFID == item);

                        }
                        // Absentees List ...
                        if (liAbsentees != null && liAbsentees.Count > 0)
                        {
                            ObjViewModel.liStudentList = liAbsentees;
                            return View(ObjViewModel);
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("RemovePrasentStudents", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("RemovePrasentStudents", "AttendanceController", ex.Message);
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

        // Save Absentee Student ...
        public JsonResult SaveAbsenteeStudents(string sAbsenteeStudent, string sClassId, string sCourseId, string sHourId, string sStaffId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcaedemicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcaedemicYear))
            {
                try
                {
                    int nSuccessCount = 0;
                    int nUpdateCount = 0;
                    int nFailedCount = 0;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ABSENTEE_LIST objJson = serializer.Deserialize<ABSENTEE_LIST>(sAbsenteeStudent);
                    if (objJson.liSaveStudentAbsentees != null && objJson.liSaveStudentAbsentees.Count > 0)
                    {
                        var IsRecordExist = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(
                            new ATT_ABSENTEE_LIST_2017() { CLASS_ID = sClassId, COURSE_ID = sCourseId, HOUR_FROM = sHourId, STAFF_ID = sStaffId },
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.IsAbsenteeEntryExistingForRFID), sAcaedemicYear).DataSource.Data;
                        //if (liStudentList!=null && liStudentList.Count>0)
                        //{
                        //    string sAttendanceTypeId = string.Empty;

                        //    foreach (var item in objJson.liSaveStudentAbsentees)
                        //    {
                        //        item.ENTRY_ID = sUserId;
                        //        var IsExist = liStudentList.Where(x => x.STUDENT_ID == item.STUDENT_ID).ToList();
                        //        if (IsExist!=null && IsExist.Count>0)
                        //        {
                        //            sAttendanceTypeId = string.Empty;
                        //            sAttendanceTypeId = IsExist.FirstOrDefault().ATTENDANCE_TYPE_ID;
                        //            if (sAttendanceTypeId!="2")
                        //            {
                        //                item.ATTENDANCE_ID = "1";
                        //                var UpdateAbsentees = CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.UpdateAbsenteeEntryByCourseStaff), sAcaedemicYear);
                        //                if (UpdateAbsentees.Success)
                        //                    nUpdateCount++;
                        //                else
                        //                    nFailedCount++;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var SaveAbsentees = CMSHandler.SaveOrUpdate(item,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.SaveAbsenteeEntryByCourseStaff),sAcaedemicYear);
                        //            if (SaveAbsentees.Success)
                        //                nSuccessCount++;
                        //            else
                        //                nFailedCount++;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        if (IsRecordExist != null && IsRecordExist.Count > 0)
                        {
                            foreach (var item in IsRecordExist)
                            {
                                objJson.liSaveStudentAbsentees.RemoveAll(x => x.STUDENT_ID == item.STUDENT_ID);
                            }
                        }
                        if (objJson.liSaveStudentAbsentees != null && objJson.liSaveStudentAbsentees.Count > 0)
                        {
                            foreach (var item in objJson.liSaveStudentAbsentees)
                            {
                                item.ENTRY_ID = sUserId;
                                item.ATTENDENCE_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                                //var IsRecordExists = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.IsAbsenteeEntryExistingForRFID), sAcaedemicYear).DataSource.Data;
                                var SaveAbsentees = CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.SaveAbsenteeEntryByCourseStaff), sAcaedemicYear);
                                if (SaveAbsentees.Success)
                                    nSuccessCount++;
                                else
                                    nFailedCount++;
                            }
                        }
                        else
                        {
                            ObjJsonData.Message = "Sorry... Attendance is entered already !!";
                        }
                        if (nSuccessCount != 0)
                            ObjJsonData.Message = nSuccessCount + " - " + Common.Messages.RecordsSavedSuccessfully + "\n";
                        if (nUpdateCount != 0)
                            ObjJsonData.Message += nUpdateCount + " - " + Common.Messages.RecordsUpdatedSuccessfully + "\n";
                        if (nFailedCount != 0)
                            ObjJsonData.Message += nFailedCount + " - " + Common.Messages.FailedToSaveRecords + "\n";
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveAbsenteeStudents", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveAbsenteeStudents", "AttendanceController", ex.Message);
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

        #region Duplicate Attendance Entry By Course Staff

        // Duplicate Attendance By Course Staff .
        public ActionResult AttendancewEntryByCourseStaff()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liClassList = (List<SUP_CLASS>)CMSHandler.FetchData<SUP_CLASS>(new CourseWiseStaffList() { STAFF_ID = sUserId },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassByStaffId), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "AttendancewEntryByCourseStaff", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        // Fetch Course By ClassId ..
        public JsonResult FetchCourseByClassId(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new ClassCourse() { CLASS_ID = sClassId, STAFF_ID = sUserId },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCourseByClassAndStaffId), sAcademicYear).DataSource.Data;
                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                        foreach (var item in liCourseList)
                        {
                            ObjJsonData.sResult += "<option value=" + item.COURSE_ID + "> " + item.COURSE_TITLE + " </option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "FetchCourseByClassId", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Fetch Hour By ClassId , Course Id And DayOrderId.
        public JsonResult FetchHourByClassAndCourse(string sClassId, string sCourseId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sDate);
                    string sDayOrderId = string.Empty;
                    //Fetch Day Order By Date ..
                    var liDayOrder = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(new CALENDER_2017() { DAY_ORDER_DATE = Date },
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrderInfoByDate), sAcademicYear).DataSource.Data;
                    if (liDayOrder != null && liDayOrder.Count > 0)
                    {
                        sDayOrderId = liDayOrder.FirstOrDefault().DAY_ORDER;

                        // Fetch TimeTable Info .
                        var liHourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TT_TIMETABLE_2017() { CLASS_ID = sClassId, COURSE_ID = sCourseId, STAFF_ID = sUserId, DAY_ORDER_ID = sDayOrderId },
                             SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHoursByStaffAndCourseIdAndClassIdAndDayOrderId), sAcademicYear).DataSource.Data;
                        if (liHourList != null && liHourList.Count > 0)
                        {
                            ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                            foreach (var item in liHourList)
                            {
                                ObjJsonData.sResult += "<option value=" + item.HOUR_ID + "> " + item.HOUR + " </option>";
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.Message = "No Day Order Found !";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "FetchHourByClassAndCourse", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Bind Students For Attendance .
        public ActionResult BindStudentsForAttendance(string sClassId, string sCourseId, string sHourId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sDate);

                    // Fetch Student List
                    //ObjViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(new TT_TIMETABLE_2017() { CLASS_ID = sClassId, COURSE_ID = sCourseId, STAFF_ID = sUserId, HOUR_ID = sHourId, ATTENDANCE_DATE = Date },
                    //    SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendenceForDuplicateAttendence), sAcademicYear).DataSource.Data; 
                    ObjViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(new TT_TIMETABLE_2017() { CLASS_ID = sClassId, COURSE_ID = sCourseId, STAFF_ID = sUserId, HOUR_ID = sHourId, ATTENDANCE_DATE = Date },
                       SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.LoadStudentsForDateWiseAttendanceEntry), sAcademicYear).DataSource.Data;

                    // Fetch DropDown List For Attendance Type .
                    ObjViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "BindStudentsForAttendance", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        // Save Duplicate Attendance .
        public JsonResult SaveDuplicateAttendance(string sJsonString)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonString);
                    int SuccessCount = 0;
                    int FailedCount = 0;
                    foreach (var item in absentees.ATT_ABSENTEE_LIST)
                    {
                        if (item.ATTENDANCE_TYPE_ID == "1" && string.IsNullOrEmpty(item.ATTENDANCE_ID))
                        {
                            continue;
                        }
                        item.STAFF_ID = sUserId;
                        item.ENTRY_ID = sUserId;
                        if (!string.IsNullOrEmpty(item.ATTENDENCE_DATE))
                        {
                            item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                        }
                        else
                        {
                            DateTime date = DateTime.Today;
                            item.ATTENDENCE_DATE = date.ToString("yyyy/MM/dd");
                        }

                        // Save Or Update .
                        if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveAbsenteeEntryByCourseStaff : AttendanceSQLCommands.UpdateAbsenteeEntryByCourseStaff), sAcademicYear).Success)
                        {
                            SuccessCount++;
                            ObjJsonData.sResult = Convert.ToString(SuccessCount);
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            FailedCount++;
                            ObjJsonData.sResult = Convert.ToString(FailedCount);
                            ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "SaveDuplicateAttendance", "Err MSg: " + ex.Message, "Query is empty!");
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

        #region Calender Wise Attendance Report 
        // Over All Attendance Report ...
        public ActionResult OverAllMonthWiseAttendanceReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Month DDL ...
                    var liMonth = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null,
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender), sAcademicYear).DataSource.Data;
                    if (liMonth != null && liMonth.Count > 0)
                    {
                        ObjViewModel.MonthList = new SelectList(liMonth, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }

                    // Fetch Class DDL ...
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null,
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("Attendance", "OverAllMonthWiseAttendanceReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("Error", "ErrorMessage");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindOverAllAttendanceReport(string sMonth, string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "BindOverAllAttendanceReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        // Month Wise Report For Course Staff ...
        public ActionResult MonthCourseWiseAttendanceReport()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Month DDL ...
                    var liMonth = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender), sAcademicYear).DataSource.Data;
                    if (liMonth != null && liMonth.Count > 0)
                    {
                        ObjViewModel.MonthList = new SelectList(liMonth, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }

                    // Fetch Class DDL ...
                    string sSQL = SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchClassByStaffId).
                        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CourseWiseStaffInfo() { STAFF_ID = sUserId }, sSQL, sAcademicYear).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "MonthCourseWiseAttendanceReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("Error", "ErrorMessage");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindMonthCourseWiseAttendaneReport(string sClassId, string sCourseId, string sMonth)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjModel = new Stu_Course_Info();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.CLASS_ID = sClassId;
                    ObjModel.COURSE_ID = sCourseId;
                    ObjModel.DAY_ORDER_MONTH = sMonth;
                    ObjModel.STAFF_ID = sUserId;
                    // Hour Count With Date ...
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchTotalHoursByCourseIdWithDate).
                            Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                    ObjViewModel.liHours = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel,
                              sSQL, sAcademicYear).DataSource.Data;

                    // Student Absentees With Date ...
                    sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentAbsentCountByCourseIdWithDate).
                        Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                    ObjViewModel.liAbsenteesList = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(ObjModel,
                          sSQL, sAcademicYear).DataSource.Data;

                    // Fetch Student By Class Id ...
                    ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(ObjModel,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByClassIdAndAcYear), sAcademicYear).DataSource.Data;

                    // Fetch Calender Days ....
                    sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchTotalDaysByMonthFromCalender).
                        Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                    ObjViewModel.liCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel,
                        sSQL, sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "BindMonthCourseWiseAttendaneReport", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        // Month Wise Attendance For Student ...
        public ActionResult MonthCourseWiseAttendanceForStudent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sStudentId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonth = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalender), sAcademicYear).DataSource.Data;
                    if (liMonth != null && liMonth.Count > 0)
                    {
                        ObjViewModel.MonthList = new SelectList(liMonth, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                    //// Fetch Regular Papers .
                    //var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                    //        SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;

                    //// Fetch Optional Papers .
                    //var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sStudentId },
                    //             SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                    //// Loop Optional Papers  To Remove Unnecessary Courses...
                    //if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                    //{
                    //    foreach (var item in liOptionalCourses)
                    //    {
                    //        var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID).ToList();
                    //        if (IsExist != null && IsExist.Count > 0)
                    //        {
                    //            liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                    //            item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                    //            liRegularCourses.Add(item);
                    //        }
                    //    }
                    //}
                    //if (liRegularCourses!=null && liRegularCourses.Count>0)
                    //{
                    //    ObjViewModel.CourseList = new SelectList(liRegularCourses,Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID,Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
                    //}
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "MonthCourseWiseAttendanceForStudent", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("Error", "ErrorMessage");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindMonthCourseWiseAttendanceForStudent(string sMonth)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new Stu_Course_Info();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STUDENT_ID = sUserId;
                    //ObjModel.COURSE_ID = sCourseId;
                    ObjModel.DAY_ORDER_MONTH = sMonth;
                    var liClass = (List<STU_CLASS>)CMSHandler.FetchData<STU_CLASS>(new STU_CLASS() { STUDENT_ID = sUserId },
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByStudentId), sAcademicYear).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjModel.CLASS_ID = liClass.FirstOrDefault().CLASS_ID;
                        // Fetch Absent Count ...
                        string sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentAbsentCountWithDate).
                            Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);

                        ObjViewModel.liAbsenteesList = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(ObjModel,
                              sSQL, sAcademicYear).DataSource.Data;

                        // Fetch Calender Days ....
                        sSQL = string.Empty;
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchTotalDaysByMonthFromCalender).
                            Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                        ObjViewModel.liCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel,
                            sSQL, sAcademicYear).DataSource.Data;

                        ObjViewModel.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(ObjModel,
                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoForReport), sAcademicYear).DataSource.Data;

                        // Fetch Regular Papers .
                        var liRegularCourses = (List<EXAM_REGISTRATION>)CMS.DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                                SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;

                        // Fetch Optional Papers .
                        var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                                     SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                        // Loop Optional Papers  To Remove Unnecessary Courses...
                        if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                        {
                            foreach (var item in liOptionalCourses)
                            {
                                var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID).ToList();
                                if (IsExist != null && IsExist.Count > 0)
                                {
                                    liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                    item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                    liRegularCourses.Add(item);
                                }
                            }
                        }
                        ObjViewModel.liStudentCourseList = liRegularCourses.OrderBy(x => x.PART).ToList();
                        string sCourseIds = string.Empty;
                        if (ObjViewModel.liStudentCourseList != null && ObjViewModel.liStudentCourseList.Count > 0)
                        {
                            sCourseIds = string.Join(",", ObjViewModel.liStudentCourseList.Select(x => x.COURSE_ROOT_ID).ToArray());
                        }

                        // Fetch Hour Count ...
                        sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAbsentHoursCountWithDate).
                            Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                        // Replace CourseIds ...
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                        ObjViewModel.liHours = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "BindMonthCourseWiseAttendanceForStudent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        // Month Wise Attendance For Parent ...
        public ActionResult MonthCourseWiseAttendanceForParent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sStudentId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liMonth = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchMonthFromCalenderByCurrentYear), sAcademicYear).DataSource.Data;
                    if (liMonth != null && liMonth.Count > 0)
                    {
                        ObjViewModel.MonthList = new SelectList(liMonth, Common.CALENDER_2017.DAY_ID, Common.CALENDER_2017.DAY_ORDER_MONTH);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "MonthCourseWiseAttendanceForParent", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("Error", "ErrorMessage");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindMonthCourseWiseAttendanceForParent(string sMonth)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string StudentId = (Session[Common.SESSION_VARIABLES.STUDENT_ID] != null) ? Session[Common.SESSION_VARIABLES.STUDENT_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsonData = new JsonResultData();
            var ObjModel = new Stu_Course_Info();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = string.Empty;
                    // Fetch Calender Days ....
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchTotalDaysByMonthFromCalender).
                        Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                    ObjViewModel.liCalender = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel,
                        sSQL, sAcademicYear).DataSource.Data;
                    var sStudentId = StudentId.Split(',');
                    foreach (var item in sStudentId)
                    {
                        ObjModel.STUDENT_ID = item;
                        //ObjModel.COURSE_ID = sCourseId;
                        ObjModel.DAY_ORDER_MONTH = sMonth;
                        var liClass = (List<STU_CLASS>)CMSHandler.FetchData<STU_CLASS>(new STU_CLASS() { STUDENT_ID = item },
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByStudentId), sAcademicYear).DataSource.Data;
                        if (liClass != null && liClass.Count > 0)
                        {
                            ObjModel.CLASS_ID = liClass.FirstOrDefault().CLASS_ID;
                            // Fetch Absent Count ...
                            sSQL = string.Empty;
                            sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentAbsentCountWithDate).
                                Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);

                            var liAbsenteesList = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;

                            var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(ObjModel,
                                 SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoForReport), sAcademicYear).DataSource.Data;

                            if (liStudent != null && liStudent.Count > 0)
                            {
                                ObjViewModel.liStudent = (ObjViewModel.liStudent != null && ObjViewModel.liStudent.Count > 0) ? ObjViewModel.liStudent.Concat(liStudent).ToList() : liStudent;
                            }
                            // Fetch Regular Papers .
                            var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item },
                                    SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;

                            // Fetch Optional Papers .
                            var liOptionalCourses = (List<EXAM_REGISTRATION>)DAO.CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item },
                                         SQL.Examination.ExaminationSQL.GetExaminationSQL(DAO.ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

                            // Loop Optional Papers  To Remove Unnecessary Courses...
                            if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                            {
                                foreach (var Courseitem in liOptionalCourses)
                                {
                                    var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == Courseitem.COURSE_ROOT_ID).ToList();
                                    if (IsExist != null && IsExist.Count > 0)
                                    {
                                        liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                                        Courseitem.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                        liRegularCourses.Add(Courseitem);
                                    }
                                }
                            }

                            if (liRegularCourses != null && liRegularCourses.Count > 0)
                            {
                                string sCourseIds = string.Empty;
                                sCourseIds = string.Join(",", liRegularCourses.Select(x => x.COURSE_ROOT_ID).ToArray());
                                // Fetch Hour Count ...
                                sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAbsentHoursCountWithDate).
                                    Replace(Common.Delimiter.QUS + Common.CALENDER_2017.DAY_ORDER_MONTH, sMonth);
                                // Replace CourseIds ...
                                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                                var liHours = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                                ObjViewModel.liHours = (ObjViewModel.liHours != null && ObjViewModel.liHours.Count > 0) ? ObjViewModel.liHours.Concat(liHours).ToList() : liHours;

                                ObjViewModel.liStudentCourseList = (ObjViewModel.liStudentCourseList != null && ObjViewModel.liStudentCourseList.Count > 0) ? ObjViewModel.liStudentCourseList.Concat(liRegularCourses.OrderBy(x => x.PART).ToList()).ToList() : liRegularCourses.OrderBy(x => x.PART).ToList();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("AttendanceController", "BindMonthCourseWiseAttendanceForParent", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Attendace Submisssion Report Principal View
        public ActionResult AttendanceSubmissionForPrincipal()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Attendance", "AttendanceSubmissionForPrincipal", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindAttendanceSubmissionForPrincipal(string sDate, string sShift)//,string sStaffId
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttAbsenteesProccessedReportForPrincipal)
                        .Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                    sDate = CommonMethods.ConvertdatetoyearFromat(sDate);
                    ObjViewModel.liAttendanceReport = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT()
                    { ATTENDENCE_DATE = sDate, SHIFT = sShift }, sSQL, sAcademicYear).DataSource.Data; //, STAFF_ID=sStaffId
                    ObjViewModel.hourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("Attendance", "AttendanceSubmissionForPrincipal", ex.Message);
                    }
                    ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region Class Wise Attendance Entry
        public ActionResult ClassWiseAttendanceEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (liClass != null && liClass.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                    var liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours), sAcademicYear).DataSource.Data;
                    if (liHours != null && liHours.Count > 0)
                    {
                        ObjViewModel.Hours = new SelectList(liHours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "ClassWiseAttendanceEntry", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindClassWiseAttendance(string sClassId, string sHourId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "ClassWiseAttendanceEntry", ex.Message);
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

        #region Online Attendance Entry
        public ActionResult OnlineAttendanceEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;

                    var liShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "OnlineAttendanceEntry", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchHourByShiftIdForOnlineAttendance(string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourByShiftIdForOnlineAttendance);
                    var liHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TT_TIMETABLE_2017() { STAFF_ID = sStaffId, SHIFT = sShiftId }, sSQL, sAcademicYear).DataSource.Data;
                    if (liHours != null && liHours.Count > 0)
                    {
                        ObjJsonData.sResult = "<option value='0'> -- Select -- </option>";
                        ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                        foreach (var item in liHours)
                        {
                            ObjJsonData.sResult += "<option value='" + item.HOUR_ID + "'>" + item.HOUR + "</option>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "FetchHourByShiftIdForOnlineAttendance", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadStudentsForOnlineAttendance(string sHourId, string sShiftId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var timeTable = new TT_TIMETABLE_2017();
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    timeTable.STAFF_ID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                    timeTable.HOUR_ID = sHourId;
                    timeTable.SHIFT = sShiftId;
                    // Fetch Students For Attendance ...
                    sSQL = string.Empty;
                    sSQL = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentsForOnlineAttendance);
                    ObjViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable, sSQL, sAcademicYear).DataSource.Data;
                    if (ObjViewModel.attendanceEntry != null && ObjViewModel.attendanceEntry.Count > 0)
                    {
                        ObjViewModel.attendanceEntry = ObjViewModel.attendanceEntry.OrderBy(x => x.ROLL_NO).ToList();
                    }

                    ObjViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes)).DataSource.Data;
                    ObjViewModel.reasons = (List<SUP_REASON>)CMSHandler.FetchData<SUP_REASON>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForAttendance)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "LoadStudentsForOnlineAttendance", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveOnlineAttendance(string sJsonString, string IsAllPrasent, string sFlag, string sJsonData)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TimeTableSetting TimeTable = new TimeTableSetting();
            int SuccessCount = 0;
            int FailedCount = 0;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sJsonString))
                    {
                        var absentees = new ABSENTEE_LIST();
                        if (sFlag == "true")
                        {
                            absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonString);
                        }
                        else
                        {
                            absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonData);
                        }
                        DateTime date = DateTime.Today;
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
                        if (absentees.ATT_ABSENTEE_LIST != null && absentees.ATT_ABSENTEE_LIST.Count > 0 && sFlag == "true")
                        {
                            foreach (var item in absentees.ATT_ABSENTEE_LIST)
                            {
                                if (!string.IsNullOrEmpty(item.ATTENDENCE_DATE))
                                {
                                    item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                                }
                                else
                                {
                                    item.ATTENDENCE_DATE = date.ToString("yyyy/MM/dd");
                                }
                                item.ENTRY_ID = sUserId;

                                if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveOnlineAbsentees : AttendanceSQLCommands.UpdateOnlineAbsentees), sAcademicYear).Success)
                                    SuccessCount++;
                                else
                                    FailedCount++;
                            }
                        }


                        // Save Staff Attendance Entry Done ...
                        if (SuccessCount != 0 || IsAllPrasent == "1")
                        {
                            var ObjModel = new ATT_ABSENTEE_LIST_2017();
                            ObjModel.ATTENDENCE_DATE = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().ATTENDENCE_DATE;
                            if (string.IsNullOrEmpty(ObjModel.ATTENDENCE_DATE))
                            {
                                ObjModel.ATTENDENCE_DATE = date.ToString("yyyy/MM/dd");
                            }
                            ObjModel.STAFF_ID = sUserId;
                            ObjModel.HOUR_FROM = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().HOUR_FROM;
                            ObjModel.HOUR_TO = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().HOUR_TO;
                            ObjModel.COURSE_ID = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().COURSE_ID;
                            ObjModel.STATUS = IsAllPrasent;
                            ObjModel.CLASS_ID = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().CLASS_ID;
                            ObjModel.ENTRY_ID = sUserId;
                            ObjModel.S_CLASS_ID = absentees.ATT_ABSENTEE_LIST.FirstOrDefault().S_CLASS_ID;

                            var liCheckAttAbsentteesProccressed = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(ObjModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.IsExistAbsenteesProccessed), sAcademicYear).DataSource.Data;
                            if (liCheckAttAbsentteesProccressed != null && liCheckAttAbsentteesProccressed.Count > 0)
                            {
                                // Do Nothing , All Ready Record Inserted ,...
                            }
                            else
                            {
                                var SaveAttAbsentteesProccessed = CMSHandler.SaveOrUpdate(ObjModel, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.SaveAttAbsenteesProccessed), sAcademicYear);
                                if (SaveAttAbsentteesProccessed.Success)
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                else
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }

                        if (SuccessCount != 0)
                        {
                            objResult.Message = "(" + SuccessCount + ") ";
                            objResult.Message += Common.Messages.RecordsSavedSuccessfully + "\n";
                        }

                        if (FailedCount != 0)
                        {
                            objResult.Message += "(" + FailedCount + ")" + Common.Messages.FailedToSaveRecords;
                        }
                        objResult.ErrorCode = Common.ErrorCode.Ok;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
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
                    objHandler.WriteError("SaveOnlineAttendance", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveOnlineAttendance", "AttendanceController", ex.Message);
                }
            }

            if (string.IsNullOrEmpty(objResult.Message))
            {
                objResult.Message = "No Changes Made !";
                objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Duplicate Online Attendance Entry
        public ActionResult DateWiseAttendanceEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liClassList = (List<SUP_CLASS>)CMSHandler.FetchData<SUP_CLASS>(new CourseWiseStaffList() { STAFF_ID = sUserId },
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchClassByStaffId), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "DateWiseAttendanceEntry", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchOnlineHourByClassIdAndCourseIdAndDate(string sClassId, string sCourseId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sDate);
                    string sDayOrderId = string.Empty;
                    //Fetch Day Order By Date ..
                    var liDayOrder = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(new CALENDER_2017() { DAY_ORDER_DATE = Date },
                         SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrderInfoByDate), sAcademicYear).DataSource.Data;
                    if (liDayOrder != null && liDayOrder.Count > 0)
                    {
                        sDayOrderId = liDayOrder.FirstOrDefault().DAY_ORDER;

                        // Fetch TimeTable Info .
                        var liHourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TT_TIMETABLE_2017() { CLASS_ID = sClassId, COURSE_ID = sCourseId, STAFF_ID = sUserId, DAY_ORDER_ID = sDayOrderId },
                             SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchOnlineHourByClassIdAndCourseIdAndDate), sAcademicYear).DataSource.Data;
                        if (liHourList != null && liHourList.Count > 0)
                        {
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.sResult = "<option value=''> -- Select -- </option>";
                            foreach (var item in liHourList)
                            {
                                ObjJsonData.sResult += "<option value=" + item.HOUR_ID + "> " + item.HOUR + " </option>";
                            }
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                        ObjJsonData.Message = "No Day Order Found !";
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "FetchHourByClassIdAndCourseIdAndDate", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindStudentsForOnlineDateWiseAttendance(string sClassId, string sCourseId, string sHourId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sDate);

                    // Fetch Student List
                    var liAttendance = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(new TT_TIMETABLE_2017() { CLASS_ID = sClassId, COURSE_ID = sCourseId, STAFF_ID = sUserId, HOUR_ID = sHourId, ATTENDANCE_DATE = Date },
                         SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentsForDateWiseOnlineAttendance), sAcademicYear).DataSource.Data;
                    if (liAttendance != null && liAttendance.Count > 0)
                    {
                        ObjViewModel.attendanceEntry = liAttendance.OrderBy(x => x.ROLL_NO).ToList();
                    }

                    // Fetch DropDown List For Attendance Type .
                    ObjViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null,
                            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "BindStudentsForOnlineDateWiseAttendance", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveDateWiseOnlineAttendance(string sJsonString)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonString);
                    int SuccessCount = 0;
                    int FailedCount = 0;
                    foreach (var item in absentees.ATT_ABSENTEE_LIST)
                    {
                        if (item.ATTENDANCE_TYPE_ID == "1" && string.IsNullOrEmpty(item.ATTENDANCE_ID))
                        {
                            continue;
                        }
                        item.STAFF_ID = sUserId;
                        item.ENTRY_ID = sUserId;
                        if (!string.IsNullOrEmpty(item.ATTENDENCE_DATE))
                        {
                            item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                        }
                        else
                        {
                            DateTime date = DateTime.Today;
                            item.ATTENDENCE_DATE = date.ToString("yyyy/MM/dd");
                        }

                        // Save Or Update .
                        if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveOnlineAbsentees : AttendanceSQLCommands.UpdateOnlineAbsentees), sAcademicYear).Success)
                        {
                            SuccessCount++;
                            ObjJsonData.sResult = Convert.ToString(SuccessCount);
                            ObjJsonData.ErrorCode = Common.ErrorCode.Ok;
                            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        }
                        else
                        {
                            FailedCount++;
                            ObjJsonData.sResult = Convert.ToString(FailedCount);
                            ObjJsonData.ErrorCode = Common.ErrorCode.BadRequest;
                            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("AttendanceController", "SaveDuplicateAttendance", "Err MSg: " + ex.Message, "Query is empty!");
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

        #region Attendance Report For SJC
        public ActionResult AttendancePercentage()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = DapperHandler.FetchAll<ACADEMIC_YEAR>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).ToList();
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liSemType = DapperHandler.FetchAll<SUP_SEMESTER_TYPE>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).ToList();
                    if (liSemType != null && liSemType.Count > 0)
                    {
                        ObjViewModel.SemesterType = new SelectList(liSemType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }

                    var liShift = DapperHandler.FetchAll<Sup_Shift>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).ToList();
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttedanceController", "AttendancePercentage", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindAttendancePercentage(string sAcYear, string sShiftId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = DapperHandler.FetchAll<ATT_ATTENDANCE_REPORT>(SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchAttendancepercentagereport), new { SHIFT = sShiftId, START_DATE = sStartDate, END_DATE = sEndDate }, sAcYear).ToList();
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }


        // Individual Student Report ...,
        public ActionResult InvidualStudentAttendancePercentage()
        {
            return View();
        }

        public ActionResult BindInvidualStudentAttendancePercentage(string sRollNo, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liCollegeDetails = DapperHandler.FetchAll<CollegeDetails>(SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).ToList();
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = DapperHandler.FetchAll<ATT_ATTENDANCE_REPORT>(SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.BindIndividualStudentAttendancePercentage), new { ROLL_NO = sRollNo, START_DATE = sStartDate, END_DATE = sEndDate }, sAcademicYear).ToList();
                }
                catch (Exception Ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindInvidualStudentAttendancePercentage", Ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        #endregion

        #region    common attendance report for Office
        public ActionResult AttendancePercentageCommon()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var department = DapperHandler.FetchAll<Department>(
                  SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentbyClasses)).ToList();
                    if (department != null && department.Count > 0)
                    {
                        ObjViewModel.DepartmentList = new SelectList(department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);


                    }
                    var liAcademicYear = DapperHandler.FetchAll<ACADEMIC_YEAR>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).ToList();
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liSemType = DapperHandler.FetchAll<SUP_SEMESTER_TYPE>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).ToList();
                    if (liSemType != null && liSemType.Count > 0)
                    {
                        ObjViewModel.SemesterType = new SelectList(liSemType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }

                    var liShift = DapperHandler.FetchAll<Sup_Shift>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).ToList();
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    //var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchclass), sAcademicYear).DataSource.Data;
                    //if (liClassList != null && liClassList.Count > 0)
                    //{
                    //    ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    //}
                    var LiSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                    if (LiSemesterType != null && LiSemesterType.Count > 0)
                    {
                        ObjViewModel.SemesterType = new SelectList(LiSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                    var ClassLevel = DapperHandler.FetchAll<SupClassLevel>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).ToList();
                    if (ClassLevel != null && ClassLevel.Count > 0)
                    {
                        ObjViewModel.ClassLevel = new SelectList(ClassLevel, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    }
                    var ClassYear = DapperHandler.FetchAll<SubClassYear>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).ToList();
                    if (ClassYear != null && ClassYear.Count > 0)
                    {
                        ObjViewModel.ClassYear = new SelectList(ClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    }
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttedanceController", "AttendancePercentage", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult LackAttendancePercentageCommon(string sAcYear, string sShiftId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAttendancepercentagereport).Replace("?SHIFT", sShiftId),  sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }

        public ActionResult BindAttendancePercentageCommon(string sAcYear, string sShiftId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAttendancepercentagereport).Replace("?SHIFT", sShiftId),  sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAbsenteesListDateWise(string sAcYear, string sShiftId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAbsetneeListDateWise).Replace("?SHIFT", sShiftId),  sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAbsenteesListDayWise(string sShiftId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ViewBag.Date = sDate;
                    sDate = CommonMethods.DateConvertionForPayU(sDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAttendancedDailyReport).Replace("?SEMESTER_TYPE", sSemesterType).Replace("?SHIFT", sShiftId).Replace("?START_DATE", sDate), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindCommonAbsenteesListDayWise", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAbsenteesListRangeWise(string sShiftId, string Range)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    string[] range = Range.Split(',');
                    string value1 = range[0];
                    string value2 = range[1];
                    // sDate = CommonMethods.DateConvertionForPayU(sDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAbsenteeListRangeWise).Replace("?SEMESTER_TYPE", sSemesterType).Replace("?SHIFT", sShiftId).Replace("?VALUE1", value1).Replace("?VALUE2", value2), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindCommonAbsenteesListDayWise", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAttendancePercentageWithMark(string sAcYear, string sShiftId, string sStartDate, string sEndDate, string programmeLevel)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAttendancepercentagereportWithMark).Replace("?SHIFT", sShiftId).Replace("?START_DATE", sStartDate).Replace("?END_DATE", sEndDate).Replace("?CLASS_LEVEL", programmeLevel), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAttendanceReportbyClassWise(string sAcYear, string classId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAsbsenteesListbyClassWise).Replace("?CLASS_ID", classId),  sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonIndividualAttendanceReport(string sAcYear, string sStartDate, string sEndDate, string sStudent)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { START_DATE = sStartDate, END_DATE = sEndDate },SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAsbsenteesListbyStudentId).Replace("?STUDENT_ID", sStudent), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchStudentByShiftId(string sShiftID, string sAc_year)
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sAcademicYearId = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR_ID].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liFetchStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new ClassInfo(),
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentListbyShiftId).Replace("?SHIFT", sShiftID), sAc_year).DataSource.Data;

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
        public ActionResult BindCommonNotAbsenteesListClassWise(string sAcYear, string classId, string semestertype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    //sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    //sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchNotAbsenteeListClassWise).Replace("?CLASS_ID", classId).Replace("?SEMESTER_TYPE", semestertype), sAcYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        
        #endregion


        #region    Common Attendance Report for HOD
        public ActionResult AttendancePercentageReportForHOD()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;


                        //      var department = DapperHandler.FetchAll<Department>(
                        //SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentbyClasses)).ToList();
                        //      if (department != null && department.Count > 0)
                        //      {
                        //          ObjViewModel.DepartmentList = new SelectList(department, Common.CP_DEPARTMENT_2017.DEPARTMENT_ID, Common.CP_DEPARTMENT_2017.DEPARTMENT);


                        //      }
                        var liAcademicYear = DapperHandler.FetchAll<ACADEMIC_YEAR>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).ToList();
                        if (liAcademicYear != null && liAcademicYear.Count > 0)
                        {
                            ObjViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                        }

                        var liSemType = DapperHandler.FetchAll<SUP_SEMESTER_TYPE>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).ToList();
                        if (liSemType != null && liSemType.Count > 0)
                        {
                            ObjViewModel.SemesterType = new SelectList(liSemType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                        }

                        var liShift = DapperHandler.FetchAll<Sup_Shift>(SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).ToList();
                        if (liShift != null && liShift.Count > 0)
                        {
                            ObjViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                        }

                        var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartment).Replace("?DEPARTMENT", departmnet), sAcademicYear).DataSource.Data;
                        if (liClassList != null && liClassList.Count > 0)
                        {
                            ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                        var LiSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                        if (LiSemesterType != null && LiSemesterType.Count > 0)
                        {
                            ObjViewModel.SemesterType = new SelectList(LiSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                        }
                    }
                    ObjViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttedanceController", "AttendancePercentage", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAbsenteesListDayWiseForHOD(string sShiftId, string sDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;
                        ViewBag.Date = sDate;
                        sDate = CommonMethods.DateConvertionForPayU(sDate);


                        ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAttendancedDailyReportForHOD).Replace("?SEMESTER_TYPE", sSemesterType).Replace("?SHIFT", sShiftId).Replace("?START_DATE", sDate).Replace("?DEPARTMENT", departmnet), sAcademicYear).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindCommonAbsenteesListDayWise", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult LackAttendancePercentageReportHOD(string sAcYear, string sShiftId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserID = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserID }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;
                        sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                        sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);

                        ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { DEPARTMENT = departmnet, SHIFT = sShiftId, START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAttendancepercentagereportForHod),  sAcYear).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonNotAbsenteesListClassWiseForHOD(string sAcYear, string classId, string semestertype)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;
                        //sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                        //sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);

                        ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchNotAbsenteeListClassWiseForHOD).Replace("?CLASS_ID", classId).Replace("?SEMESTER_TYPE", semestertype), sAcYear).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public ActionResult BindCommonAbsenteesListDateWiseForHOD(string sAcYear, string sShiftId, string sStartDate, string sEndDate)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (lidepartment != null)
                    {
                        var departmnet = lidepartment.FirstOrDefault().DEPARTMENT;
                        sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                        sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                        ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(new { DEPARTMENT = departmnet, SHIFT = sShiftId, START_DATE = sStartDate, END_DATE = sEndDate }, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAbsetneeListDateWiseForHOD), sAcYear).DataSource.Data;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindAttendancePercentage", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchClassByshiftID(string shift)
        {
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(shift))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        var lidepartment = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new stf_Personal_Info() { STAFF_ID = sUserId }, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                        if (lidepartment != null)
                        {
                            var sdepart = lidepartment.FirstOrDefault().DEPARTMENT;
                            var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES(), SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId).Replace("?DEPARTMENT", sdepart).Replace("?SHIFT", shift), sAcademicYear).DataSource.Data;

                            if (liClass != null && liClass.Count > 0)
                            {
                                foreach (var item in liClass)
                                {
                                    ObjJsonData.sResult += "<option value='" + item.CLASS_ID + "' >" + item.CLASS_NAME + "</option>";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjHandler = new ErrorLog())
                        {
                            ObjHandler.WriteError("AttendanceController", "FetchClassByshiftandDepartmentID", "Err MSg: " + ex.Message, "Query is empty!");
                            ObjHandler.WriteError("AttendanceController", "FetchClassByshiftandDepartmentID", ex.Message);
                        }
                        ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                    }
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.BadRequest;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  attendance report for student 
        public ActionResult CommonAbsenteeReportforStudent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string suser_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            //string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            try
            {
                var fetchAcademicsem = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchSemesterEndDate).Replace("?STUDENT_ID", suser_id), sAcademicYear).DataSource.Data;
                if (fetchAcademicsem != null)
                {
                    var endDateString = fetchAcademicsem.FirstOrDefault().DATE_TO;

                    var currentDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(endDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var currentDateParsed = DateTime.ParseExact(currentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (endDate > currentDateParsed)
                    {
                        ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAsbsenteesListbyStudentIdforStudent)
                                .Replace("?STUDENT_ID", suser_id).Replace("?ENDDATE", "curdate()"),sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(null,
                                SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchCommonAsbsenteesListbyStudentIdforStudent)
                                    .Replace("?STUDENT_ID", suser_id).Replace("?ENDDATE", "ASM.DATE_TO"), sAcademicYear ).DataSource.Data;
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AttendanceController", "CommonAbsenteeReportforStudent", "Student Attendance Report", ex.Message);
                }
                ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }

            return View(ObjViewModel);
        }
        #endregion

        #region Common Attendance Entry  for Office 
        public ActionResult OverAllAttendanceEntryForOffice()
        {
            string sAcademicYear = string.Empty;
            AttendanceViewModel attendanceViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    attendanceViewModel.shifts = (List<Shift>)CMSHandler.FetchData<Shift>(null,
                       SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    attendanceViewModel.Departments = (List<Department>)CMSHandler.FetchData<Department>(null,
                      SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentbyClasses), sAcademicYear).DataSource.Data;
                    // attendanceViewModel.staffList = (List<STAFF_PERSONAL>)CMSHandler.FetchData<STAFF_PERSONAL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                    // attendanceViewModel.hourList = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data;
                    //attendanceViewModel.SplAttendanceType = (List<SUP_SPL_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_SPL_ATTENDANCE_TYPE>(null, 
                    //    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupSplAttendanceType)).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("OverAllAttendance", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("OverAllAttendance", "AttendanceController", ex.Message);
                }
            }
            return View(attendanceViewModel);
        }

        public ActionResult LoadStudentsForOverAllAttendanceEntry(string objInput)
        {
            var obj = new JavaScriptSerializer().Deserialize<OVER_ALL_ATTENDANCE>(objInput);
            AttendanceViewModel objAttendanceViewModel = new AttendanceViewModel();
            TT_TIMETABLE_2017 timeTable = new TT_TIMETABLE_2017();
            CALENDER_2017 Calender = new CALENDER_2017();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objAttendanceViewModel.sDayOrder = (Session[Common.SESSION_VARIABLES.DAY_ORDER] != null) ? Session[Common.SESSION_VARIABLES.DAY_ORDER].ToString() : string.Empty;
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
                    string sClassId = string.Empty;
                    // Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] = "1,3,5";
                    timeTable.STAFF_ID = obj.StaffId;
                    timeTable.CLASS_ID = obj.ClassId;
                    timeTable.HOUR_ID = obj.HourId;
                    timeTable.SEMESTER_ID = obj.semesterId;
                    timeTable.COURSE_ID = obj.courseId;
                    timeTable.ATTENDANCE_DATE = CommonMethods.ConvertdatetoyearFromat(obj.AttendanceDate);
                    //timeTable.ATTENDANCE_DATE = DateTime.Today.ToString("yyyy-MM-dd");

                    objAttendanceViewModel.attendanceEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable,
                        SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentForSpecialAttendance), sAcademicYear).DataSource.Data;
                    if (objAttendanceViewModel.attendanceEntry == null)
                    {
                        Calender.DAY_ORDER_DATE = timeTable.ATTENDANCE_DATE;
                        var FetchDayOrder = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(Calender,
                            SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchdDayOrderByAttendanceDate), sAcademicYear).DataSource.Data;
                        if (FetchDayOrder != null && FetchDayOrder.Count > 0)
                        {
                            timeTable.DAY_ORDER_ID = FetchDayOrder.FirstOrDefault().DAY_ORDER;
                            var liAttEntry = (List<ATTENDANCE_ENTRY_MODEL>)CMSHandler.FetchData<ATTENDANCE_ENTRY_MODEL>(timeTable,
                                SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentListForAttendanceByClassIdForSpecialAttendance), sAcademicYear).DataSource.Data;
                            if (liAttEntry != null && liAttEntry.Count > 0)
                            {
                                objAttendanceViewModel.attendanceEntry = liAttEntry;
                            }
                        }
                    }

                    objAttendanceViewModel.attendanceTypes = (List<SUP_ATTENDANCE_TYPE>)CMSHandler.FetchData<SUP_ATTENDANCE_TYPE>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAttendanceTypes)).DataSource.Data;
                    objAttendanceViewModel.reasons = (List<SUP_REASON>)CMSHandler.FetchData<SUP_REASON>(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchReasonForAttendance)).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Attendance", "LoadStudentsForOverAll", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("Attendance", "LoadStudentsForOverAll", Ex.Message);
                }

            }
            return PartialView("LoadStudentsForOverAllAttendanceEntry", objAttendanceViewModel);
        }
        public JsonResult CommonAttendanceEntryForOffice(string sJsonString, string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TimeTableSetting TimeTable = new TimeTableSetting();
            int SuccessCount = 0;
            int FailedCount = 0;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sJsonString))
                    {
                        var absentees = serializer.Deserialize<ABSENTEE_LIST>(sJsonString);
                        string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : string.Empty;

                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                        foreach (var item in absentees.ATT_ABSENTEE_LIST)
                        {
                            if (item.ATTENDANCE_TYPE_ID == "1" && item.ATTENDANCE_ID != "")
                            {
                                item.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                                item.CLASS_ID = sClassId;
                                item.SEMESTER_TYPE = sSemesterType;
                                if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveCommonAbsenteeEntry : AttendanceSQLCommands.UpdateCommonAbsenteeEntry), sAcademicYear).Success)
                                {
                                    SuccessCount++;
                                    objResult.sResult = Convert.ToString(SuccessCount);
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    FailedCount++;
                                    objResult.sResult = Convert.ToString(FailedCount);
                                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                            else if (item.ATTENDANCE_TYPE_ID != "1")
                            {
                                item.ENTRY_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                                item.ATTENDENCE_DATE = CommonMethods.ConvertdatetoyearFromat(item.ATTENDENCE_DATE);
                                item.CLASS_ID = sClassId;
                                item.SEMESTER_TYPE = sSemesterType;
                                if (CMSHandler.SaveOrUpdate(item, SQL.Attendance.AttendanceSQL.GetAttendanceSQL(string.IsNullOrEmpty(item.ATTENDANCE_ID) ? AttendanceSQLCommands.SaveCommonAbsenteeEntry : AttendanceSQLCommands.UpdateCommonAbsenteeEntry), sAcademicYear).Success)
                                {
                                    SuccessCount++;
                                    objResult.sResult = Convert.ToString(SuccessCount);
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    FailedCount++;
                                    objResult.sResult = Convert.ToString(FailedCount);
                                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.ErrorMessage.BadRequest;
                    }
                }
                else
                {
                    objResult.ErrorCode = Common.ErrorCode.RequestTimeout;
                    objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CommonAttendanceEntry", "AttendanceController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CommonAttendanceEntry", "AttendanceController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HostelStudentAbsentReport()
        {
            AttendanceViewModel ObjViewModel = new AttendanceViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                    if (FetchAcademicYear != null && FetchAcademicYear.Count > 0)
                    {
                        ObjViewModel.AcademicYearList = new SelectList(FetchAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var liShift = (List<Sup_Shift>)CMS.DAO.CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift)).DataSource.Data;
                    if (liShift != null && liShift.Count > 0)
                    {
                        ObjViewModel.Shift = new SelectList(liShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
                    }
                    var liHostel = (List<SUP_HOSTEL>)CMS.DAO.CMSHandler.FetchData<SUP_HOSTEL>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHostelDDL)).DataSource.Data;
                    if (liHostel != null && liHostel.Count > 0)
                    {
                        ObjViewModel.HostelList = new SelectList(liHostel, Common.SUP_HOSTEL.HOSTEL_ID, Common.SUP_HOSTEL.HOSTEL_NAME);
                    }
                    var liSemesterType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                    if (liSemesterType != null && liSemesterType.Count > 0)
                    {
                        ObjViewModel.SemesterType = new SelectList(liSemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("AttendanceController", "HostelStudentAbsentReport", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult BindHostelStudentAbsentReport(string sAcYear, string sShiftId, string sStartDate, string sEndDate, string sHostelId, string sSemesterType, string sMode)
        {
            var ObjViewModel = new AttendanceViewModel();
            var ObjJsoneData = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    ViewBag.Mode = sMode;

                    if (sMode == "overall")
                    {
                        var liDuration = (List<SEMESTER_DURATION>)CMSHandler.FetchData<SEMESTER_DURATION>(null, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchSemesterDuration)).DataSource.Data;
                        var liSemesterTypeObj = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data;
                        var selectedSemTypeObj = liSemesterTypeObj.FirstOrDefault(x => x.SEMESTER_TYPE_ID == sSemesterType);
                        string semTypeName = selectedSemTypeObj != null ? selectedSemTypeObj.SEMESTER_TYPE : "";

                        var activeDuration = liDuration.FirstOrDefault(x => 
                            x.ACADEMIC_YEAR == sAcYear && 
                            x.SEMESTER_TYPE.Equals(semTypeName, StringComparison.OrdinalIgnoreCase) && 
                            x.IS_ACTIVE == "1"
                        );

                        if (activeDuration != null)
                        {
                            sStartDate = activeDuration.FROM_DATE;
                            sEndDate = activeDuration.TO_DATE;
                        }
                        else
                        {
                            ObjJsoneData.ErrorCode = Common.ErrorCode.NotFound;
                            ObjJsoneData.Message = "No active semester duration found for this Academic Year and Semester Type.";
                            return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    ViewBag.StartDate = sStartDate;
                    ViewBag.EndDate = sEndDate;

                    string formattedStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                    string formattedEndDate = sMode == "daywise" ? formattedStartDate : CommonMethods.DateConvertionForPayU(sEndDate);

                    string query = "";
                    if (sMode == "daywise")
                    {
                        query = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHostelStudentDailyReport)
                                   .Replace("?SHIFT", sShiftId)
                                   .Replace("?SEMESTER_TYPE", sSemesterType)
                                   .Replace("?START_DATE", formattedStartDate);
                    }
                    else
                    {
                        query = SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchHostelStudentAbsentReport)
                                   .Replace("?SHIFT", sShiftId)
                                   .Replace("?SEMESTER_TYPE", sSemesterType);
                    }

                    if (!string.IsNullOrEmpty(sHostelId) && sHostelId != "0")
                    {
                        query = query.Replace("?HOSTEL_CONDITION", "AND HS.HOSTEL_ID = '" + sHostelId + "'");
                    }
                    else
                    {
                        query = query.Replace("?HOSTEL_CONDITION", "");
                    }

                    ObjViewModel.liAttAttendanceReport = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(
                        new { START_DATE = formattedStartDate, END_DATE = formattedEndDate },
                        query,
                        sAcYear
                    ).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "BindHostelStudentAbsentReport", ex.Message);
                    }
                    ObjJsoneData.ErrorCode = Common.ErrorCode.InternalServerError;
                    ObjJsoneData.Message = Common.ErrorMessage.InternalServerError;
                    return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsoneData.ErrorCode = Common.ErrorCode.NotAcceptable;
                ObjJsoneData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                return Json(ObjJsoneData, JsonRequestBehavior.AllowGet);
            }
            return PartialView("BindHostelStudentAbsentReport", ObjViewModel);
        }

        [HttpPost]
        public ActionResult FetchStudentAbsentDates(string sStudentId, string sAcYear, string sSemesterType, string sStartDate, string sEndDate)
        {
            try
            {
                sStartDate = CommonMethods.DateConvertionForPayU(sStartDate);
                sEndDate = CommonMethods.DateConvertionForPayU(sEndDate);
                var list = (List<ATT_ATTENDANCE_REPORT>)CMSHandler.FetchData<ATT_ATTENDANCE_REPORT>(
                    new { START_DATE = sStartDate, END_DATE = sEndDate },
                    SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchStudentAbsentDates)
                        .Replace("?STUDENT_ID", sStudentId)
                        .Replace("?SEMESTER_TYPE", sSemesterType),
                    sAcYear
                ).DataSource.Data;
                return Json(list);
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("AttendanceController", "FetchStudentAbsentDates", ex.Message);
                }
                return Json(new List<ATT_ATTENDANCE_REPORT>());
            }
        }
        #endregion
    }
}