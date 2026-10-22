using CMS.DAO;
using CMS.Models;
using CMS.SQL.Dashboard;
using CMS.SQL.Examination;
using CMS.SQL.Fee;
using CMS.SQL.LMS;
using CMS.SQL.StaffSQL;
using CMS.SQL.SupportData;
using CMS.SQL.TimeTable;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;


namespace CMS.Controllers
{

    public class DashBoardController : Controller
    {
        // GET: DashBoard
        JsonResultData ObjsonData = new JsonResultData();
        ResultArgs Resultargs = new ResultArgs();
        string sResult = string.Empty;
        private object sNoticeBoardId;

        public ActionResult DashboardIndex()
        {
            return View();
        }
        [UserRights("SUPER ADMIN")]
        public ActionResult SuperAdmin()
        {
            return View();
        }
        [UserRights("ADMIN")]
        public ActionResult Admin()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new DashboardViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Fetch Student Today Birth Day Celebraties With Image ,..
                    ObjViewModel.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

                    // Fetch Staff Birthday Day Celebraties With Image ,..
                    ObjViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;

                    ResultArgs resultArgs = new ResultArgs();
                    resultArgs = ObjViewModel.GetOverallFeedbackCountByStudent();
                    ViewBag.FeedbackTotal = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    resultArgs = ObjViewModel.NoOfStudents(sAcademicYear);
                    ViewBag.TotalStudents = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    ViewBag.FeedbackPercentage = ((ViewBag.FeedbackTotal / ViewBag.TotalStudents) * 100);
                    resultArgs = ObjViewModel.NoOfStaff();
                    ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);

                    // Fetch Student Count By Programme Type ,..
                    ObjViewModel.liDashBoardModel = (List<DashBoardModel>)CMSHandler.FetchData<DashBoardModel>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchProgrammeTypeWiseStudentCount), sAcademicYear).DataSource.Data;

                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        ObjViewModel.lstHours = FetchHours;

                    ObjViewModel.liSubstituteList = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchSubstituteHoursByCurDate), sAcademicYear).DataSource.Data;
                    // Fetch Year Wise Student Count ,..
                    //var liYearWiseStudent = (List<DashBoardModel>)CMSHandler.FetchData<DashBoardModel>(null,
                    //     DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchClassYearStudentCount)).DataSource.Data;

                    //Fetch calender events 
                    ObjViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                    // ObjViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("DashBoardController", "Admin", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [UserRights("VICEPRINCIPAL")]
        public ActionResult VicePrincipal()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();  // academic year need to handle paul
                                                                                                     //   sAcademicYear = "2018";
                                                                                                     //  Session[Common.SESSION_VARIABLES.SEMESTER_ID] = "1,3,5";
                        dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                        //  resultArgs = objDashboard.FetchStaffInfoById(dv);
                        if (resultArgs.Success && resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {

                        }
                        objStfModel.STAFF_ID = sUserId;
                        sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                        objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                        var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                        if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;


                        //Fetch calender events 
                        objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                        resultArgs = objDashboard.NoOfStaff();
                        ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);

                        var liAssignment = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoOfRemainingAssignments), sAcademicYear).DataSource.Data;
                        ViewBag.TotalAssignment = liAssignment != null && liAssignment.Count > 0 ? Convert.ToDouble(liAssignment.FirstOrDefault().NO_OF_ASSIGNMENTS) : 0;


                        // Fetch Student Today Birth Day Celebraties With Image ,..
                        objViewModel.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

                        // Fetch Staff Birthday Day Celebraties With Image ,..
                        objViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
                        objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "HeadOftheDepartment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "HeadOftheDepartment", ex.Message);
                }

            }

            return View();
        }
        [UserRights("DEAN")]
        public ActionResult Dean()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();  // academic year need to handle paul
                                                                                                     //   sAcademicYear = "2018";
                                                                                                     //  Session[Common.SESSION_VARIABLES.SEMESTER_ID] = "1,3,5";
                        dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                        //  resultArgs = objDashboard.FetchStaffInfoById(dv);
                        if (resultArgs.Success && resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {

                        }
                        objStfModel.STAFF_ID = sUserId;
                        sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                        objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                        var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                        if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;


                        //Fetch calender events 
                        objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                        resultArgs = objDashboard.NoOfStaff();
                        ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);


                        //fetch Notification
                        //objViewModel.liNotification = (List<NOTITFICATIONS>)CMSHandler.FetchData<NOTITFICATIONS>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNotificationInfoforStaff), sAcademicYear).DataSource.Data;

                        //fetch no of assignments
                        var liAssignment = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoOfRemainingAssignments), sAcademicYear).DataSource.Data;
                        ViewBag.TotalAssignment = liAssignment != null && liAssignment.Count > 0 ? Convert.ToDouble(liAssignment.FirstOrDefault().NO_OF_ASSIGNMENTS) : 0;
                        // Fetch Student Today Birth Day Celebraties With Image ,..
                        objViewModel.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

                        // Fetch Staff Birthday Day Celebraties With Image ,..
                        objViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
                        objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;

                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "HeadOftheDepartment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "HeadOftheDepartment", ex.Message);
                }

            }

            return View();
        }
        [UserRights("HEAD OF THE DEPARTMENT")]
        public ActionResult HeadOftheDepartment()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();  // academic year need to handle paul
                                                                                                     //   sAcademicYear = "2018";
                                                                                                     //  Session[Common.SESSION_VARIABLES.SEMESTER_ID] = "1,3,5";
                        dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                        //  resultArgs = objDashboard.FetchStaffInfoById(dv);
                        if (resultArgs.Success && resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {

                        }
                        objStfModel.STAFF_ID = sUserId;
                        sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                        objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                        var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                        if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;



                        //Fetch calender events 
                        objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;

                        //fetch Notification
                        //objViewModel.liNotification = (List<NOTITFICATIONS>)CMSHandler.FetchData<NOTITFICATIONS>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNotificationInfoforStaff), sAcademicYear).DataSource.Data;

                        //fetch  no of staff
                        resultArgs = objDashboard.NoOfStaff();
                        ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);

                        //fetch no of assignments
                        var liAssignment = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoOfRemainingAssignments), sAcademicYear).DataSource.Data;
                        ViewBag.TotalAssignment = liAssignment != null && liAssignment.Count > 0 ? Convert.ToDouble(liAssignment.FirstOrDefault().NO_OF_ASSIGNMENTS) : 0;



                        // Fetch Staff Birthday Day Celebraties With Image ,..
                        objViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
                        objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "HeadOftheDepartment", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "HeadOftheDepartment", ex.Message);
                }

            }

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult MessAdmin()
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sSQL = string.Empty;
            string sFEECollectionSql = string.Empty;
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                //Fetch Fee Collection 
                sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FEE_MODE, Common.FrequencyType.MessFee);
                var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.MessFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes), sAcademicYear).DataSource.Data;
                if (FetchFrequency != null && FetchFrequency.Count > 0)
                {
                    sFEECollectionSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDateWiseFeeCollection).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID);
                    var FeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, sFEECollectionSql, sAcademicYear).DataSource.Data;
                    if (FeeCollection != null && FeeCollection.Count > 0)
                    {
                        ViewBag.Feecollection = FeeCollection;
                    }
                }
                // Fetch Student Count

                var HostelStudentCount = (List<HOSTEL_STUDENT_INFO>)CMSHandler.FetchData<HOSTEL_STUDENT_INFO>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.NoOfHostelStudent), sAcademicYear).DataSource.Data;
                if (HostelStudentCount != null && HostelStudentCount.Count > 0)
                {
                    ViewBag.HostelStudent = HostelStudentCount.FirstOrDefault().STUDENT_ID;
                }
                objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "COE", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "COE", ex.Message);
                }
            }
            return View(objViewModel);
        }
        [UserRights("PRINCIPAL")]
        public ActionResult Principal()
        {
            string sSQL = string.Empty;
            int totalstudent = 0;
            int passedstudent = 0;
            var listudentmark = new List<OBE_SCORESHEET>();
            string sFEECollectionSql = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            DashboardViewModel objDashboard = new DashboardViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ResultArgs resultArgs = new ResultArgs();
                    resultArgs = objDashboard.GetOverallFeedbackCountByStudent();
                    ViewBag.FeedbackTotal = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    resultArgs = objDashboard.NoOfStudents(sAcademicYear);
                    ViewBag.TotalStudents = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    ViewBag.FeedbackPercentage = ((ViewBag.FeedbackTotal / ViewBag.TotalStudents) * 100);
                    resultArgs = objDashboard.NoOfStaff();
                    ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);
                    //objDashboard.liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList)).DataSource.Data;
                    //objDashboard.liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList)).DataSource.Data;

                    // Fetch Student Today Birth Day Celebraties With Image ,..
                    objDashboard.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

                    // Fetch Staff Birthday Day Celebraties With Image ,..
                    objDashboard.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;

                    //objDashboard.liCommonAttendance = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchCommonAbsentees), sAcademicYear).DataSource.Data;
                    objDashboard.liHourbasedAttendance = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchHourbasedAbsentees), sAcademicYear).DataSource.Data;
                    objDashboard.liFeeTransaction = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchDaywiseFeeTransaction)).DataSource.Data;
                    //Fetch Absentees list
                    objDashboard.liCommonAttendance = (List<ATT_ABSENTEE_LIST_2017>)CMSHandler.FetchData<ATT_ABSENTEE_LIST_2017>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAbsenteesList), sAcademicYear).DataSource.Data;

                    //Fetch calender events 
                    objDashboard.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;

                    //Fetch Fee Collection 
                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FEE_MODE, Common.FrequencyType.MessFee + Common.Delimiter.COMMA + Common.FrequencyType.SemesterFee + Common.Delimiter.COMMA + Common.FrequencyType.ExamFee + Common.Delimiter.COMMA + Common.FrequencyType.HostelFee);
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(null, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        sFEECollectionSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDateWiseFeeCollection).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID);
                        var FeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, sFEECollectionSql, sAcademicYear).DataSource.Data;
                        if (FeeCollection != null && FeeCollection.Count > 0)
                        {
                            ViewBag.Feecollection = FeeCollection;
                        }
                    }
                    //Exam Chart Report
                    var Studentmarks = (List<SEMESTER_MARKS>)CMSHandler.FetchData<SEMESTER_MARKS>(new SEMESTER_MARKS() { ACADEMIC_YEAR = sAcademicYear }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchStudentsMarks), sAcademicYear).DataSource.Data;
                    if (Studentmarks != null && Studentmarks.Count > 0)
                    {
                        var groupedsemster = Studentmarks.GroupBy(o => o.SEMESTER).ToList();
                        foreach (var item in groupedsemster)
                        {
                            var semstermarks = Studentmarks.Where(o => o.SEMESTER == item.FirstOrDefault().SEMESTER).ToList();
                            var totalstu = semstermarks.GroupBy(o => o.STUDENT_ID).ToList();
                            totalstudent = totalstu.Count();
                            var Failstudent = semstermarks.Where(o => o.RESULT == "F").ToList();
                            foreach (var mark in Failstudent)
                            {
                                semstermarks.RemoveAll(o => o.STUDENT_ID == mark.STUDENT_ID);
                            }
                            passedstudent = semstermarks.GroupBy(o => o.STUDENT_ID).Count();
                            listudentmark.Add(new OBE_SCORESHEET() { TOTAL_REGISTER = totalstudent.ToString(), PASSED = passedstudent.ToString(), SEMSTER = item.FirstOrDefault().SEMESTER, SEMSTER_NAME = item.FirstOrDefault().SEMESTER_NAME });

                        }
                        ViewBag.lichartdata = listudentmark.OrderBy(o => o.SEMSTER);

                    }
                    //College Days
                    var licollegeday = (List<OBE_SCORESHEET>)CMSHandler.FetchData<OBE_SCORESHEET>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchCollegedays), sAcademicYear).DataSource.Data;
                    if (licollegeday != null && licollegeday.Count > 0)
                    {
                        ViewBag.licollegeday = licollegeday;
                    }
                    //Fee Collection Statistical Report
                    sSQL = string.Empty;
                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FEE_MODE, Common.FrequencyType.MessFee + Common.Delimiter.COMMA + Common.FrequencyType.SemesterFee + Common.Delimiter.COMMA + Common.FrequencyType.ExamFee + Common.Delimiter.COMMA + Common.FrequencyType.HostelFee);
                    var FetchFeeFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(null, sSQL, sAcademicYear).DataSource.Data;
                    if (FetchFeeFrequency != null && FetchFeeFrequency.Count > 0)
                    {
                        sFEECollectionSql = string.Empty;
                        sFEECollectionSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStatusticalFeeCollection).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID);
                        var StatusticalFeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, sFEECollectionSql, sAcademicYear).DataSource.Data;
                        if (StatusticalFeeCollection != null && StatusticalFeeCollection.Count > 0)
                        {
                            ViewBag.Statusticalfeecollection = StatusticalFeeCollection;
                        }
                    }
                    objDashboard.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                    //Lack of Attendance List
                    //var Liststudent = new List<ListStudents>();
                    //double nAverageHrs = 0;

                    //var listudent = (List<ListStudents>)CMSHandler.FetchData<ListStudents>(null, StaffSQL.GetStaffSQL(StaffSQLCommands.FetchStudentList), sAcademicYear).DataSource.Data;
                    //if (listudent != null && listudent.Count > 0)
                    //{

                    //    foreach (var item in listudent)
                    //    {

                    //        // Fetch absent Hours ...
                    //        var liFetchAbsentHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                    //                    ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalAbsentHours), sAcademicYear).DataSource.Data;


                    //        // Get All The Regular Papers By Student_Id ...
                    //        var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                    //            ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                    //        var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = item.STUDENT_ID },
                    //              ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;


                    //        // Loop Optional Papers  To Remove Unnecessary Courses...
                    //        if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                    //        {
                    //            foreach (var oplcourse in liOptionalCourses)
                    //            {
                    //                var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == oplcourse.COURSE_ROOT_ID).ToList();
                    //                if (IsExist != null && IsExist.Count > 0)
                    //                {
                    //                    liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID);
                    //                    oplcourse.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                    //                    liRegularCourses.Add(oplcourse);
                    //                }
                    //            }
                    //        }
                    //        sSQL = string.Empty;
                    //        string sCourseIds = string.Join(",", liRegularCourses.Select(x => x.COURSE_ROOT_ID).ToArray());
                    //        sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchTotalHours).Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                    //        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STU_CLASS.STUDENT_ID, item.STUDENT_ID);
                    //        // Fetch Total Hours ...
                    //        var liFetchTotalHours = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(null, sSQL, sAcademicYear).DataSource.Data;

                    //        // Attendace Calculation ...
                    //        if (liFetchTotalHours != null && liFetchTotalHours.Count > 0 && liFetchAbsentHours != null && liFetchAbsentHours.Count > 0)
                    //        {
                    //            int nTotal = Convert.ToInt32(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                    //            double nTotalHrs = Convert.ToDouble(liFetchTotalHours.FirstOrDefault().TOTAL_HRS);
                    //            double nAbsentHrs = Convert.ToDouble(liFetchAbsentHours.FirstOrDefault().TOTAL_HRS);
                    //            if (nTotal == 0)
                    //            {
                    //                nAverageHrs = 0;
                    //            }
                    //            else
                    //            {
                    //                double percenthrs = nTotalHrs - nAbsentHrs;
                    //                nAverageHrs = (percenthrs / (double)nTotalHrs) * 100;
                    //            }

                    //            if (Math.Round(nAverageHrs) < Convert.ToInt32(ConfigurationManager.AppSettings["LACKOFPERCENTAGE"]))
                    //            {
                    //                Liststudent.Add(new ListStudents() { STUDENT_ID = item.STUDENT_ID, FIRST_NAME = item.FIRST_NAME, CLASS_NAME = item.CLASS_NAME, ROLL_NO = item.ROLL_NO, ATTENDANCE_PERCENTAGE = Math.Round(nAverageHrs).ToString() });
                    //            }
                    //        }
                    //    }
                    //    objDashboard.Listudent = Liststudent;




                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DashBoardController", "Principal", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DashBoardController", "Principal", ex.Message);
                    }
                }
                return View(objDashboard);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
        }

        // Fetch Fee Transactions For Dashboard ...
        public JsonResult FetchFeeTransactions()
        {
            Morris_Chart ObjJsonData = new Morris_Chart();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int i = 0;
                    string[] Color = { "#5AC8FA", "#8686ce", "#bd4c44", "007aff", "#003366", "#ffd39b", "#c6d8f1", "#f09f61", "#cd9c99", "#00b3b3" };
                    string sMonthwise = string.Empty;
                    var liMonthWiseFeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchMonthwiseFeeTransaction)).DataSource.Data;
                    if (liMonthWiseFeeCollection != null && liMonthWiseFeeCollection.Count > 0)
                    {
                        sMonthwise = "[";
                        string sMonth = "[";
                        foreach (var item in liMonthWiseFeeCollection)
                        {
                            sMonthwise += "{x:'" + item.DAY_ORDER_DATE + "' , y: 60},";
                            sMonth += "'" + item.DAY_ORDER_MONTH + "'" + ",";
                        }
                        ObjJsonData.sMonthData = sMonthwise.TrimEnd(',') + "]".Trim();
                        ObjJsonData.sMonthLabel = sMonth.TrimEnd(',') + "]".Trim();
                    }
                    i = 0;
                    var liDaywiseFeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchTotalFeeTransaction)).DataSource.Data;
                    if (liDaywiseFeeCollection != null && liDaywiseFeeCollection.Count > 0)
                    {
                        string sDaywise = "[";
                        string sDayColor = "[";
                        foreach (var item in liDaywiseFeeCollection)
                        {
                            sDaywise += "{label:'" + item.FREQUENCY_TYPE + "',value:" + item.COLLECTED + "},";
                            sDayColor += "'" + Color[i] + "'" + ",";
                            i++;
                        }
                        ObjJsonData.sFeeData = sDaywise.TrimEnd(',') + "]".Trim();
                        ObjJsonData.sFeeColor = sDayColor.TrimEnd(',') + "]".Trim();
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DashBoardController", "FetchFeeTransactions", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DashBoardController", "FetchFeeTransactions", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        [UserRights("STAFF")]
        public ActionResult Staff()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();  // academic year need to handle paul
                                                                                                     //   sAcademicYear = "2018";
                                                                                                     //  Session[Common.SESSION_VARIABLES.SEMESTER_ID] = "1,3,5";
                        dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                        //  resultArgs = objDashboard.FetchStaffInfoById(dv);
                        if (resultArgs.Success && resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {

                        }
                        objStfModel.STAFF_ID = sUserId;
                        sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                        objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                        var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                        if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        else
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = "6" }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        resultArgs = objViewModel.NoOfStaff();
                        ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);
                        objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;
                        objViewModel.liOnlineTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchOnlineTimetableForStaff), sAcademicYear).DataSource.Data;

                        objViewModel.liSubstituteList = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubstituteHourByStaffId), sAcademicYear).DataSource.Data;

                        //Fetch calender events 
                        objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;

                        //fetch Notification
                        //objViewModel.liNotification = (List<NOTITFICATIONS>)CMSHandler.FetchData<NOTITFICATIONS>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNotificationInfoforStaff), sAcademicYear).DataSource.Data;

                        //fetch  no of staff
                        //resultArgs = objDashboard.NoOfStaff();
                        //ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);

                        //fetch no of assignments
                        //var liAssignment = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoOfRemainingAssignments), sAcademicYear).DataSource.Data;
                        //ViewBag.TotalAssignment = liAssignment != null && liAssignment.Count > 0 ? Convert.ToDouble(liAssignment.FirstOrDefault().NO_OF_ASSIGNMENTS) : 0;

                        // Fetch Staff Birthday Day Celebraties With Image ,..
                        objViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
                        //objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "Staff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Staff", ex.Message);
                }

            }

            return View();
        }
        [UserRights("STUDENT")]
        public async Task<ActionResult> Student()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            try
            {
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sAcademicYear = string.Empty;
                string sDayOrder = string.Empty;
                AssignmentModel objAssignmentModel = new AssignmentModel();
                SUP_BATCH_DURATION objbatchduartion = new SUP_BATCH_DURATION();
                List<SEMESTER_MARKS> liSemestermarks = new List<SEMESTER_MARKS>();
                List<SEMESTER_MARKS> lifinalSemstermark = new List<SEMESTER_MARKS>();
                List<SEMESTER_MARKS> lisem = new List<SEMESTER_MARKS>();
                List<SEMESTER_MARKS> lipart = new List<SEMESTER_MARKS>();

                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        await Task.Run(() =>
                        {
                            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                            //sDayOrder = Session[Common.SESSION_VARIABLES.DAY_ORDER].ToString();
                            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                            objAssignmentModel.STUDENT_ID = sUserId;
                            //sAcademicYear = "2017";
                            dv.Add(Common.STUDENT_INFO.ACADEMIC_YEAR, sAcademicYear, EnumCommand.DataType.String);
                            dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                            resultArgs = objDashboard.FetchStudentInfoById(dv, sAcademicYear);
                            if (resultArgs.Success)
                            {
                                if (resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                                {
                                    Session[Common.SESSION_VARIABLES.USER_INFO] = resultArgs.DataSource.Table;
                                    Session[Common.SESSION_VARIABLES.CLASS_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_ID];
                                    Session[Common.SESSION_VARIABLES.DEPARTMENT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.DEPT_ID];
                                    Session[Common.SESSION_VARIABLES.SHIFT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.SHIFT_ID];
                                    Session[Common.SESSION_VARIABLES.CLASS_NAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_NAME];
                                    Session[Common.SESSION_VARIABLES.PHOTO] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.PHOTO];
                                    Session[Common.SESSION_VARIABLES.ADMITTED_BATCH] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.ADMITTED_BATCH];
                                    Session[Common.SESSION_VARIABLES.CLASS_LEVEL] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_LEVEL];
                                }
                            }
                            objbatchduartion.BATCH_ID = (Session[Common.SESSION_VARIABLES.ADMITTED_BATCH] != null) ? Session[Common.SESSION_VARIABLES.ADMITTED_BATCH].ToString() : string.Empty;
                            objbatchduartion.APPLICATION_TYPE_ID = (Session[Common.SESSION_VARIABLES.CLASS_LEVEL] != null) ? Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString() : string.Empty;
                            string sApplicationtype = (Session[Common.SESSION_VARIABLES.CLASS_LEVEL] != null) ? Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString() : string.Empty;
                            var BatchExpiredYear = (List<SUP_BATCH_DURATION>)CMSHandler.FetchData<SUP_BATCH_DURATION>(objbatchduartion, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchBatchExpiredYear)).DataSource.Data;
                            if (BatchExpiredYear != null && BatchExpiredYear.Count > 0)
                            {

                                for (int i = Convert.ToInt16(BatchExpiredYear.FirstOrDefault().BEGIN_YEAR); i < Convert.ToInt16(BatchExpiredYear.FirstOrDefault().EXPIRED_YEAR); i++)
                                {
                                    var Studentmarks = (List<SEMESTER_MARKS>)CMSHandler.FetchData<SEMESTER_MARKS>(new SEMESTER_MARKS() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear }, DashboardSQL.GetDashboardSQL(sApplicationtype.Equals("1") ? DashboardSQLCommands.FetchUgStudentMarks : DashboardSQLCommands.FetchStudentMarks), i.ToString()).DataSource.Data;
                                    if (Studentmarks != null && Studentmarks.Count > 0)
                                    {
                                        liSemestermarks = liSemestermarks.Concat(Studentmarks).ToList();
                                    }
                                }
                                if (liSemestermarks != null && liSemestermarks.Count > 0)
                                {
                                    var lifinalSemestermarks = liSemestermarks.GroupBy(c => new
                                    {
                                        c.SEMESTER,
                                        c.PART

                                    })
                                       .Select(t => new
                                       {
                                           STUDENT_ID = t.FirstOrDefault().STUDENT_ID,
                                           NAME = t.FirstOrDefault().NAME,
                                           M_PASS = t.FirstOrDefault().SEMESTER_NAME,
                                           PART_NAME = t.FirstOrDefault().PART_NAME,
                                           PART = t.FirstOrDefault().PART,
                                           SEMESTER_NAME = t.FirstOrDefault().SEMESTER_NAME,
                                           SEMESTER = t.FirstOrDefault().SEMESTER,
                                           MAXIMUM = t.Sum(ta => Convert.ToUInt32(ta.MAXIMUM)),
                                           START_DATE = t.FirstOrDefault().START_DATE,

                                           TOTAL = t.Sum(ta => Convert.ToUInt32(ta.TOTAL)),
                                       }).ToList();
                                    if (lifinalSemestermarks != null)
                                    {
                                        foreach (var item in lifinalSemestermarks)
                                        {
                                            double percentage = ((double)Convert.ToInt32(item.TOTAL) / (double)Convert.ToInt32(item.MAXIMUM)) * 100;
                                            lifinalSemstermark.Add(new SEMESTER_MARKS() { STUDENT_ID = item.STUDENT_ID, M_PASS = item.SEMESTER_NAME, TOTAL = (Math.Round(percentage)).ToString(), START_DATE = item.START_DATE, SEMESTER = item.SEMESTER, PART_NAME = item.PART_NAME, PART = item.PART });

                                        }
                                        objViewModel.liSemstermark = lifinalSemstermark.OrderBy(o => o.SEMESTER).ToList();

                                        ViewBag.liSemstermark = lifinalSemstermark.OrderBy(o => o.SEMESTER);

                                        var lisemster = lifinalSemestermarks.GroupBy(o => o.SEMESTER).ToList();
                                        if (lisemster != null && lisemster.Count > 0)
                                        {
                                            foreach (var sem in lisemster)
                                            {
                                                lisem.Add(new SEMESTER_MARKS() { SEMESTER = sem.FirstOrDefault().SEMESTER, SEMESTER_NAME = sem.FirstOrDefault().SEMESTER_NAME });
                                            }
                                        }
                                        ViewBag.lisem = lisem.OrderBy(o => o.SEMESTER);
                                        var parts = lifinalSemestermarks.GroupBy(o => o.PART).ToList();
                                        if (parts != null && parts.Count > 0)
                                        {
                                            foreach (var part in parts)
                                            {
                                                lipart.Add(new SEMESTER_MARKS() { PART = part.FirstOrDefault().PART, PART_NAME = part.FirstOrDefault().PART_NAME });
                                            }
                                        }
                                        ViewBag.lipart = lipart.OrderBy(o => o.PART);

                                    }
                                }
                            }
                            //Time Table
                            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                            TimeTable objModel = new TimeTable();
                            var BindStudentCourseInfo = new List<TimeTable>();
                            var OptionCourseInfo = new List<TimeTable>();
                            var FetchStudentClsId = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByStudentId), sAcademicYear).DataSource.Data;
                            if (FetchStudentClsId != null && FetchStudentClsId.Count > 0)
                            {
                                objModel.Class_Id = FetchStudentClsId.FirstOrDefault().CLASS_ID;
                            }

                            var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalHoursAndDaysByClassId), sAcademicYear).DataSource.Data;
                            if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                            {
                                var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                                var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                                if (FetchHours != null && FetchHours.Count > 0)
                                    objViewModel.lstHours = FetchHours;
                                if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                    objViewModel.lstDayOrders = FetchDayOrders;
                            }
                            else
                            {
                                var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                                var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = "6" }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                                if (FetchHours != null && FetchHours.Count > 0)
                                    objViewModel.lstHours = FetchHours;
                                if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                    objViewModel.lstDayOrders = FetchDayOrders;
                            }

                            string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableClassWiseListByClassId);
                            sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Semester_Id.ToUpper(), sActiveSemester);
                            BindStudentCourseInfo = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                            objViewModel.liTimetable = BindStudentCourseInfo;

                            objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;

                            //Fetchoptioncourse time table
                            //var Fetchoptionalcourse = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;
                            //if (Fetchoptionalcourse != null && Fetchoptionalcourse.Count > 0)
                            //{
                            //    objModel.Class_Id = string.Join(",", Fetchoptionalcourse.Select(o => o.CLASS_ID));
                            //    string sOption = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableClassWiseListByClassId);
                            //    sOption = sSQL.Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Semester_Id.ToUpper(), sActiveSemester);
                            //    sOption = sOption.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, objModel.Class_Id);
                            //    OptionCourseInfo = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, sOption, sAcademicYear).DataSource.Data;
                            //    if (OptionCourseInfo != null && OptionCourseInfo.Count > 0)
                            //    {
                            //        objViewModel.liTimetable = (objViewModel.liTimetable != null) ? objViewModel.liTimetable.Concat(OptionCourseInfo).ToList() : OptionCourseInfo;
                            //    }
                            //}
                            //string ssql = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchValueEducationPapersForTimeTable);
                            //var liFetchEthics = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, ssql, sAcademicYear).DataSource.Data;
                            //if (liFetchEthics != null && liFetchEthics.Count > 0)
                            //{
                            //    objViewModel.liTimetable = (objViewModel.liTimetable != null) ? objViewModel.liTimetable.Concat(liFetchEthics).ToList() : liFetchEthics;
                            //}
                            //objAssignmentModel.CLASS_ID = Session[Common.SESSION_VARIABLES.CLASS_ID].ToString();
                            //objAssignmentModel.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                            // objViewModel.liNMERegisteredStudent = (List<NMERegisteredStudent>)CMSHandler.FetchData<NMERegisteredStudent>(objAssignmentModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRegisteredNMECourse), sAcademicYear).DataSource.Data;
                            //objViewModel.liCertificateRegisteredStudent = (List<CERTIFICATE_STUDENT_REGISTRATION>)CMSHandler.FetchData<CERTIFICATE_STUDENT_REGISTRATION>(objAssignmentModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRegisteredCertificateCourse), sAcademicYear).DataSource.Data;
                            //objViewModel.liAssignment = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objAssignmentModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAssignmentInfo), sAcademicYear).DataSource.Data;
                            //if (objViewModel.liAssignment != null && objViewModel.liAssignment.Count > 0)
                            //{
                            //    foreach (var item in objViewModel.liAssignment)
                            //    {
                            //        byte[] bytes = Convert.FromBase64String(item.DESCRIPTION);
                            //        item.DESCRIPTION = Encoding.UTF8.GetString(bytes);
                            //    }
                            //}
                            //objViewModel.liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(objAssignmentModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffInfoByStudentId), sAcademicYear).DataSource.Data;


                            //fetch Notification
                            //objViewModel.liNotification = (List<NOTITFICATIONS>)CMSHandler.FetchData<NOTITFICATIONS>(objAssignmentModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNotificationInfoforStudent), sAcademicYear).DataSource.Data;


                            //chart
                            //For Semester Mark
                            //List<SEM_DETAILS_FOR_CHAT> liDataPoints = new List<SEM_DETAILS_FOR_CHAT>();
                            //objViewModel.liSemforChat = (List<SEM_DETAILS_FOR_CHAT>)CMSHandler.FetchData<SEM_DETAILS_FOR_CHAT>(new FEE_STUDENT_ACCOUNT() { STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString()}, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemMarkForChat), sAcademicYear).DataSource.Data;

                            //if (objViewModel.liSemforChat != null && objViewModel.liSemforChat.Count > 0)
                            //{
                            //    foreach (var item in objViewModel.liSemforChat)
                            //    {
                            //        liDataPoints.Add(new SEM_DETAILS_FOR_CHAT() { y = Convert.ToDouble(item.Y), label = item.LABEL});
                            //    }
                            //}
                            //ViewBag.DataPoints = JsonConvert.SerializeObject(liDataPoints);

                            //for attendance percentage

                            //// Fetch Value Education Papers .....
                            //var liValueEducation = (List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() {STUDENT_ID = sUserId },
                            //    SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchValueEducationCourseAttendacneForCurrentSemesterByStudentId), sAcademicYear).DataSource.Data;
                            //// Fetch Optional Papers ....
                            //var liAttendanceReportForOptional =(List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() {STUDENT_ID = sUserId},
                            //  SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchOptionalCourseAttendacneForCurrentSemesterByStudentId), sAcademicYear).DataSource.Data;
                            //// Fetch Regular Papers ....
                            //var liAttendanceReport =(List<ATTENDANCE_REPORT>)CMSHandler.FetchData<ATTENDANCE_REPORT>(new ATTENDANCE_REPORT() { STUDENT_ID = sUserId},
                            // SQL.Attendance.AttendanceSQL.GetAttendanceSQL(AttendanceSQLCommands.FetchRegularCourseAttendacneForCurrentSemesterByStudentId), sAcademicYear).DataSource.Data;

                            //if (TotalHoursByRegularCourse!=null && TotalHoursByRegularCourse.Count>0)
                            //{
                            //    //SUP_SEMESTER_ID
                            //    TotalHours = (from s in TotalHoursByRegularCourse
                            //                  join ss in TotalHoursByOptionalCourse on s.SUP_SEMESTER_ID equals ss.SUP_SEMESTER_ID
                            //                  select new SEM_DETAILS_FOR_CHAT
                            //                  {
                            //                      LABEL = s.LABEL,
                            //                      SUP_SEMESTER_ID = s.SUP_SEMESTER_ID,
                            //                      TOTAL_WORKING_HOURS = s.TOTAL_WORKING_HOURS,
                            //                      TOTAL_OPTIONAL_WORKING_HOURS = ss.TOTAL_OPTIONAL_WORKING_HOURS,
                            //                      TOTAL_HOURS = Convert.ToInt16(s.TOTAL_WORKING_HOURS) - Convert.ToInt16(ss.TOTAL_OPTIONAL_WORKING_HOURS)
                            //                  }).ToList();

                            //    if (TotalAbsentHours!=null && TotalAbsentHours.Count>0)
                            //    {
                            //        TotalHours1 = (from s in TotalHours
                            //                       join ss in TotalAbsentHours on s.SUP_SEMESTER_ID equals ss.SUP_SEMESTER_ID
                            //                       select new SEM_DETAILS_FOR_CHAT
                            //                       {
                            //                           y = s.TOTAL_HOURS - Convert.ToInt16(ss.ABSENT_HOURS),
                            //                           label = s.LABEL
                            //                       }).ToList();

                            //        foreach (var item in TotalHours1)
                            //        {
                            //            liDataPoints1.Add(new SEM_DETAILS_FOR_CHAT() { y = item.y, label = item.LABEL });
                            //        }

                            //        if (liDataPoints1 != null && liDataPoints1.Count > 0)
                            //            ViewBag.DataPoints1 = JsonConvert.SerializeObject(liDataPoints1);
                            //    }
                            //}

                            //// Concat Regular Papers ...
                            //if (liAttendanceReport != null && liAttendanceReport.Count > 0)
                            //{
                            //    objViewModel.liAttendanceReport = (objViewModel.liAttendanceReport == null) ? liAttendanceReport : objViewModel.liAttendanceReport.Concat(liAttendanceReport).ToList();

                            //    // Concat Value Education Papers ...
                            //    if (liValueEducation != null && liValueEducation.Count > 0)
                            //    {
                            //        objViewModel.liAttendanceReport = (objViewModel.liAttendanceReport == null) ? liValueEducation : objViewModel.liAttendanceReport.Concat(liValueEducation).ToList();
                            //    }
                            //}

                            //// Concat Optional Papers ...
                            //if (liAttendanceReportForOptional != null && liAttendanceReportForOptional.Count > 0)
                            //{
                            //    objViewModel.liAttendanceReport = (objViewModel.liAttendanceReport == null) ? liAttendanceReportForOptional : objViewModel.liAttendanceReport.Concat(liAttendanceReportForOptional).ToList();
                            //}

                            //var PresentHrs = objViewModel.liAttendanceReport!=null && objViewModel.liAttendanceReport.Count>0 ? Convert.ToDouble(objViewModel.liAttendanceReport.Where(x=>x.COURSE_ID!=null && x.COURSE_ID!="").Sum(x => Convert.ToDouble(x.TOTAL_HRS))) - Convert.ToDouble(objViewModel.liAttendanceReport.Where(x=>x.COURSE_ID!="" && x.COURSE_ID!=null).Sum(x => Convert.ToDouble(x.ABSENT))):0;
                            //var TotalHrs = objViewModel.liAttendanceReport != null && objViewModel.liAttendanceReport.Count > 0 ? objViewModel.liAttendanceReport.Where(x=>x.COURSE_ID!="" && x.COURSE_ID!=null).Sum(x => Convert.ToDouble(x.TOTAL_HRS)):0;
                            //var  TotalPercentage = PresentHrs>0 && TotalHrs>0 ? Math.Round((PresentHrs / TotalHrs) * 100):0;


                            //ViewBag.AttendancePercentage = TotalPercentage >0 ? Convert.ToDouble(TotalPercentage) : 0;


                            //fetch library book approved status
                            //objViewModel.liLibrarybookApprovedAnounce =(List<LIBRARY_BOOK_APPROVED_ANNOUNCE>) CMSHandler.FetchData<LIBRARY_BOOK_APPROVED_ANNOUNCE>(new STU_PERSONAL_INFO() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLibraryBookApprovedUserByStudentId), sAcademicYear).DataSource.Data;


                            //Fetch calender events 
                            //objViewModel .liCalenderEvents= (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null,SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                            StudentController student = new StudentController();
                            var liArearList = new List<SEMESTER_RESULT>();
                            //Get All The Regular Papers By Student_Id...
                            var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objAssignmentModel,
                              SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                            var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objAssignmentModel,
                                SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;
                            if (liRegularCourses != null && liRegularCourses.Count > 0)
                            {
                                var liTemp = liRegularCourses.Where(o => o.COURSE_STRUCTURE_ID == "579").ToList();
                                //Loop Optional Papers To Remove Unnecessary Courses...
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
                                        else
                                        {
                                            liRegularCourses.Add(item);
                                        }
                                    }
                                }
                                var liClass = liRegularCourses.GroupBy(x => x.CLASS_ID).Select(x => new EXAM_REGISTRATION() { CLASS_ID = x.FirstOrDefault().CLASS_ID }).ToList();
                                string sClassIds = string.Join(",", liClass.Select(x => x.CLASS_ID).ToArray());

                                var liCourse = liRegularCourses.GroupBy(x => x.COURSE_ROOT_ID).Select(x => new EXAM_REGISTRATION() { COURSE_ID = x.FirstOrDefault().COURSE_ROOT_ID }).ToList();
                                string sCourseIds = string.Join(",", liCourse.Select(x => x.COURSE_ID).ToArray());

                                sSQL = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchSettingForStudentQuizEntryByClassAndCourseIds);
                                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                                var liFetchQuizSetting = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(null, sSQL, sAcademicYear).DataSource.Data;
                                if (liFetchQuizSetting != null && liFetchQuizSetting.Count > 0)
                                {
                                    foreach (var item in liFetchQuizSetting)
                                    {
                                        item.STATUS = student.ValidateQuizForSetWise(item.SETTING_ID, sUserId, item.COURSE_ID, sAcademicYear);
                                    }
                                }
                                objViewModel.liQuizSetting = liFetchQuizSetting;


                                string sSQL1 = string.Empty;
                                sSQL1 = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchActivelmsassignmentsByStudentId);
                                sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                                sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                                sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sUserId);


                                var liActiveAssignment = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(null, sSQL1, sAcademicYear).DataSource.Data;
                                if (liActiveAssignment != null && liActiveAssignment.Count > 0)
                                {
                                    objViewModel.liassignmentsetting = liActiveAssignment;
                                }
                                objViewModel.liCourseDetails = liRegularCourses;
                                objViewModel.liExamRegistrationList = liRegularCourses.OrderBy(o => o.COURSE_ORDER).ToList();
                            }



                            //Fetch Class Incharges
                            objViewModel.liClasscoursestaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(null,
                                  LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasscourseStaffByClassId), sAcademicYear).DataSource.Data;

                            //Fetch Announcement
                            objViewModel.liAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { STUDENT_ID = sUserId },
                                  LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAnnouncement), sAcademicYear).DataSource.Data;
                            //Fetch Course Wise Attachments

                            objViewModel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(new LMS_COURSEWISE_ANNOUNCEMENT() { STUDENT_ID = sUserId },
                                 LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClassworkFiles), sAcademicYear).DataSource.Data;




                        });

                    }
                    else
                        return RedirectToAction("ErrorMessage", "error");

                }

            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "Student", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Student", ex.Message);
                }
            }

            return View(objViewModel);
        }
        [UserRights("OFFICEASSISTANT")]






        public ActionResult OfficeAssistant()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sSQL = string.Empty;
            string sFEECollectionSql = string.Empty;
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    //fetch  no of staff

                    resultArgs = objDashboard.GetOverallFeedbackCountByStudent();
                    ViewBag.FeedbackTotal = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    resultArgs = objDashboard.NoOfStudents(sAcademicYear);
                    ViewBag.TotalStudents = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    ViewBag.FeedbackPercentage = ((ViewBag.FeedbackTotal / ViewBag.TotalStudents) * 100);
                    resultArgs = objDashboard.NoOfStaff();
                    ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);
                    //Fetch Fee Collection 
                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FEE_MODE, Common.FrequencyType.SemesterFee);
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.SemesterFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        sFEECollectionSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchSettlementWiseFeeCollection).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID);
                        var FeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, sFEECollectionSql, sAcademicYear).DataSource.Data;
                        if (FeeCollection != null && FeeCollection.Count > 0)
                        {
                            ViewBag.Feecollection = FeeCollection;
                        }
                    }
                    //Fetch calender events 
                    objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                    // Fetch Student Today Birth Day Celebraties With Image ,..
                    objViewModel.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

                    // Fetch Staff Birthday Day Celebraties With Image ,..
                    objViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
                    objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                }


            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "OfficeAssistant", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "OfficeAssistant", ex.Message);
                }
            }
            return View(objViewModel);
        }
        [UserRights("EXAMINCHARGE")]

        public ActionResult EXAMINCHARGE()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sSQL = string.Empty;
            string sFEECollectionSql = string.Empty;
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    //fetch  no of staff
                    resultArgs = objDashboard.NoOfStaff();
                    ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);
                    //Fetch Fee Collection 
                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FEE_MODE, Common.FrequencyType.ExamFee);
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.ExamFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        sFEECollectionSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDateWiseFeeCollection).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID);
                        var FeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, sFEECollectionSql, sAcademicYear).DataSource.Data;
                        if (FeeCollection != null && FeeCollection.Count > 0)
                        {
                            ViewBag.Feecollection = FeeCollection;
                        }
                    }
                }
                //Fetch calender events 
                objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
            }

            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "OfficeAssistant", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "OfficeAssistant", ex.Message);
                }
            }
            return View(objViewModel);
        }
        [UserRights("ASSISTANTEXAMINCHARGE")]

        public ActionResult ASSISTANTEXAMINCHARGE()
        {
            return View();
        }
        [UserRights("COE")]
        public ActionResult COE()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            DashboardViewModel objViewModel = new DashboardViewModel();
            int totalstudent = 0;
            int passedstudent = 0;
            var listudentmark = new List<OBE_SCORESHEET>();
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();


                resultArgs = objViewModel.GetOverallFeedbackCountByStudent();
                ViewBag.FeedbackTotal = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));

                ViewBag.FeedbackPercentage = ((ViewBag.FeedbackTotal / ViewBag.TotalStudents) * 100);
                resultArgs = objViewModel.NoOfStaff();
                ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);

                //Fetch calender events 
                objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                //Exam Chart Report
                var Studentmarks = (List<SEMESTER_MARKS>)CMSHandler.FetchData<SEMESTER_MARKS>(new SEMESTER_MARKS() { ACADEMIC_YEAR = sAcademicYear }, ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchStudentsMarks), sAcademicYear).DataSource.Data;
                if (Studentmarks != null && Studentmarks.Count > 0)
                {
                    var groupedsemster = Studentmarks.GroupBy(o => o.SEMESTER).ToList();
                    foreach (var item in groupedsemster)
                    {
                        var semstermarks = Studentmarks.Where(o => o.SEMESTER == item.FirstOrDefault().SEMESTER).ToList();
                        var totalstu = semstermarks.GroupBy(o => o.STUDENT_ID).ToList();
                        totalstudent = totalstu.Count();
                        var Failstudent = semstermarks.Where(o => o.RESULT == "F").ToList();
                        foreach (var mark in Failstudent)
                        {
                            semstermarks.RemoveAll(o => o.STUDENT_ID == mark.STUDENT_ID);
                        }
                        passedstudent = semstermarks.GroupBy(o => o.STUDENT_ID).Count();
                        listudentmark.Add(new OBE_SCORESHEET() { TOTAL_REGISTER = totalstudent.ToString(), PASSED = passedstudent.ToString(), SEMSTER = item.FirstOrDefault().SEMESTER, SEMSTER_NAME = item.FirstOrDefault().SEMESTER_NAME });

                    }
                    ViewBag.lichartdata = listudentmark.OrderBy(o => o.SEMSTER);
                    objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;

                }
            }
            catch (Exception ex)
            {

                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "COE", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "COE", ex.Message);
                }
            }
            return View(objViewModel);
        }

        [UserRights("EXTERNALUSERS")]
        public ActionResult EXTERNALUSERS()
        {
            var ObjViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {
                ObjViewModel.liStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(new STF_PERSONAL_INFO()
                { STAFF_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchHODsNameAndMobileNoForExternalUser), sAcademicYear).DataSource.Data;

                ObjViewModel.liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new STF_PERSONAL_INFO()
                { STAFF_ID = sUserId }, SQL.Feedback.FeedbackSQL.GetFeedbackSQL(FeedbackSQLCommands.FetchMappedCourseListByExternalStaffId), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("DashBoardController", "EXTERNALUSERS", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        [UserRights("HRDADMIN")]
        public ActionResult HRDADMIN()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        //sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();   academic year need to handle paul
                        //   sAcademicYear = "2018";
                        //  Session[Common.SESSION_VARIABLES.SEMESTER_ID] = "1,3,5";
                        dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                        //  resultArgs = objDashboard.FetchStaffInfoById(dv);
                        if (resultArgs.Success && resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {

                        }
                        objStfModel.STAFF_ID = sUserId;
                        sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                        objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                        var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                        if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;
                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "Staff", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Staff", ex.Message);
                }

            }

            return View();
        }

        [UserRights("ALUMNAE")]
        public ActionResult ALUMNAE()
        {
            var sAcYear = ConfigurationManager.AppSettings["AcYearForAlumini"];
            string sRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            //if (sRole == Common.SupRole.ALUMINI)
            //{
            //    Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] = sAcYear;
            //}
            return View();
        }

        [UserRights("SECRETARY")]
        public ActionResult Secretary()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();  // academic year need to handle paul
                                                                                                     //   sAcademicYear = "2018";
                                                                                                     //  Session[Common.SESSION_VARIABLES.SEMESTER_ID] = "1,3,5";
                        dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                        //  resultArgs = objDashboard.FetchStaffInfoById(dv);
                        if (resultArgs.Success && resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                        {

                        }
                        objStfModel.STAFF_ID = sUserId;
                        sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                        objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                        var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                        if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                            if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                                objViewModel.lstDayOrders = FetchDayOrders;
                        }
                        else
                        {
                            var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                            if (FetchHours != null && FetchHours.Count > 0)
                                objViewModel.lstHours = FetchHours;
                        }
                        objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;

                        objViewModel.liSubstituteList = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchSubstituteHoursByCurDate), sAcademicYear).DataSource.Data;

                        //Fetch calender events 
                        objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;

                        //fetch  no of staff
                        resultArgs = objDashboard.NoOfStaff();
                        ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);

                        //fetch no of assignments
                        var liAssignment = (List<AssignmentModel>)CMSHandler.FetchData<AssignmentModel>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoOfRemainingAssignments), sAcademicYear).DataSource.Data;
                        ViewBag.TotalAssignment = liAssignment != null && liAssignment.Count > 0 ? Convert.ToDouble(liAssignment.FirstOrDefault().NO_OF_ASSIGNMENTS) : 0;
                        objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "SECRETARY", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "SECRETARY", ex.Message);
                }

            }
            return View(objViewModel);
        }

        public ActionResult EVALUATOR()
        {
            return View();
        }
        [UserRights("LIBRARY ADMIN")]
        public ActionResult LibraryAdmin()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {

                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "LibraryAdmin", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "LibraryAdmin", ex.Message);
                }

            }

            return View();
        }

        [UserRights("LIBRARY INCHARGE")]
        public ActionResult LibraryIncharge()
        {
            try
            {
                DashboardViewModel objViewModel = new DashboardViewModel();
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {

                        return View(objViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "LibraryIncharge", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "LibraryIncharge", ex.Message);
                }

            }

            return View();
        }

        [UserRights("GateFeeIncharge")]
        public ActionResult GateFeeIncharge()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            return View(ObjViewModel);
        }

        [UserRights("PARENT")]
        public ActionResult PARENT()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                DataValue dv = new DataValue();
                string sUserId = string.Empty;
                string sSQL = string.Empty;
                string sAcademicYear = string.Empty;
                string sStudent_id = string.Empty;
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    // need to use this block for dashbord tasks

                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                        dv.Clear();
                        dv.Add(Common.STU_PARENT_INFO.PARENT_ID, sUserId, EnumCommand.DataType.String);
                        resultArgs = objDashboard.FetchStudentInfoByParentId(dv, sAcademicYear);
                        if (resultArgs.Success)
                        {
                            if (resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                            {
                                Session[Common.SESSION_VARIABLES.STUDENT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.STUDENT_ID];
                                sStudent_id = Session[Common.SESSION_VARIABLES.STUDENT_ID].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "Parent", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "Parent", ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult AppDownloaded()
        {
            return View();
        }
        public JsonResult SaveAppDownload(string sValue)
        {
            if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
            {
                OTHERS_AAROGYA_SETUAPP_DOWNLOAD objModel = new OTHERS_AAROGYA_SETUAPP_DOWNLOAD();
                try
                {

                    objModel.USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    objModel.ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();
                    objModel.ACADEMIC_YEAR = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objModel.STAUS = sValue;

                    Resultargs = CMSHandler.SaveOrUpdate(objModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.SaveAppdownload));
                    if (Resultargs.Success)
                        ObjsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                    else
                        ObjsonData.Message = Common.Messages.FailedToSaveRecords;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DashBoardController", "SaveAppDownload", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DashBoardController", "SaveAppDownload", ex.Message);
                    }

                }
            }
            else
            {
                ObjsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }

            return Json(ObjsonData, JsonRequestBehavior.AllowGet);
        }


        [UserRights("PLACEMENTOFFICER")]
        public ActionResult PLACEMENTOFFICER()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sSQL = string.Empty;
            string sFEECollectionSql = string.Empty;
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                ResultArgs resultArgs = new ResultArgs();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                using (DashboardViewModel objDashboard = new DashboardViewModel())
                {
                    //fetch  no of staff

                    resultArgs = objDashboard.GetOverallFeedbackCountByStudent();
                    ViewBag.FeedbackTotal = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    resultArgs = objDashboard.NoOfStudents(sAcademicYear);
                    ViewBag.TotalStudents = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
                    ViewBag.FeedbackPercentage = ((ViewBag.FeedbackTotal / ViewBag.TotalStudents) * 100);
                    resultArgs = objDashboard.NoOfStaff();
                    ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);
                    //Fetch Fee Collection 
                    sSQL = SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FEE_MODE, Common.FrequencyType.SemesterFee);
                    var FetchFrequency = (List<FEE_FREQUENCY_FEE_MODE>)CMSHandler.FetchData<FEE_FREQUENCY_FEE_MODE>(new FEE_FREQUENCY_FEE_MODE() { FEE_MODE = Common.FrequencyType.SemesterFee }, SQL.Fee.FeeSQL.GetFeeSQL(FeeSQLCommands.FetchFrequencyIdByFeeModes), sAcademicYear).DataSource.Data;
                    if (FetchFrequency != null && FetchFrequency.Count > 0)
                    {
                        sFEECollectionSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchSettlementWiseFeeCollection).Replace(Common.Delimiter.QUS + Common.SUP_FEE_FREQUENCY.FREQUENCY_ID, FetchFrequency.FirstOrDefault().FREQUENCY_ID);
                        var FeeCollection = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, sFEECollectionSql, sAcademicYear).DataSource.Data;
                        if (FeeCollection != null && FeeCollection.Count > 0)
                        {
                            ViewBag.Feecollection = FeeCollection;
                        }
                    }
                    //Fetch calender events 
                    objViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
                    // Fetch Student Today Birth Day Celebraties With Image ,..
                    objViewModel.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

                    // Fetch Staff Birthday Day Celebraties With Image ,..
                    objViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
                    objViewModel.liappdownload = (List<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>)CMSHandler.FetchData<OTHERS_AAROGYA_SETUAPP_DOWNLOAD>(new OTHERS_AAROGYA_SETUAPP_DOWNLOAD() { USER_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString(), ROLE_ID = Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAppdownloadlist), sAcademicYear).DataSource.Data;
                }


            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "OfficeAssistant", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("AccountController", "OfficeAssistant", ex.Message);
                }
            }
            return View(objViewModel);




        }
        public PartialViewResult BindNoticeBoardList(string sClassYear, string sAppType, string sShift)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = DapperHandler.FetchAll<NoticeBoard>(
                    DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfo),
                    new { CLASS_YEAR = sClassYear, APPTYPE = sAppType, SHIFT = sShift },
                    sAcademicYear
                );
                ObjViewModel.liNoticeBoard = result;
            }
            return PartialView(ObjViewModel);
        }
        public PartialViewResult GetNoticeBoardResponseLog(string sNoticeBoardId)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = DapperHandler.FetchAll<NoticeBoard>(
                    DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardLogByNoticeId),
                    new { NOTICEBOARD_ID = sNoticeBoardId , ROLE_ID =Common.SupRole.STUDENT},
                    sAcademicYear
                );
                ObjViewModel.liNoticeBoardLog = result;
            }
            return PartialView(ObjViewModel);
        }


        public ActionResult NoticeBoardReportForAdmin(string sNoticeBoardId)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch Roles for Dropdown
                var FetchRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                ObjViewModel.SupRoleList = new SelectList(FetchRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                // Fetch Notice Titles (All Notices) for Dropdown
                var result = DapperHandler.FetchAll<NoticeBoard>(
                    DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfo),
                    new { CLASS_YEAR = "", APPTYPE = "", SHIFT = "" },
                    sAcademicYear
                );
                ObjViewModel.Title = new SelectList(result, "NOTICEBOARD_ID", "NOTICEBOARD");

                // Fetch Specific Notice Board Details (for initial display)
                if (!string.IsNullOrEmpty(sNoticeBoardId))
                {
                    var liDashboard = DapperHandler.FetchAll<NoticeBoard>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfoById), new { NOTICEBOARD_ID = sNoticeBoardId }, sAcademicYear);
                    if (liDashboard != null && liDashboard.Count > 0)
                    {
                        ObjViewModel.liNoticeBoard = liDashboard;
                    }
                }
            }
            return View(ObjViewModel);
        }



        //public ActionResult NoticeBoardReportForAdmin(string sNoticeBoardId)
        //{
        //    DashboardViewModel ObjViewModel = new DashboardViewModel();

        //    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null)
        //        ? string.Empty
        //        : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

        //    if (!string.IsNullOrEmpty(sAcademicYear))
        //    {
        //        // Roles
        //        var FetchRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(
        //            null,
        //            SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole),
        //            sAcademicYear
        //        ).DataSource.Data;

        //        ObjViewModel.SupRoleList =
        //            new SelectList(FetchRole, "ROLE_ID", "ROLE_NAME");

        //        // Titles
        //        var result = DapperHandler.FetchAll<NoticeBoard>(
        //            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfo),
        //            new { CLASS_YEAR = "", APPTYPE = "", SHIFT = "" },
        //            sAcademicYear
        //        );

        //        ObjViewModel.Title =
        //            new SelectList(result, "NOTICEBOARD_ID", "NOTICEBOARD");

        //        // Load notice card
        //        if (!string.IsNullOrEmpty(sNoticeBoardId))
        //        {
        //            var liDashboard = DapperHandler.FetchAll<NoticeBoard>(
        //                DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfoById),
        //                new { NOTICEBOARD_ID = sNoticeBoardId },
        //                sAcademicYear
        //            );

        //            if (liDashboard != null && liDashboard.Count > 0)
        //            {
        //                ObjViewModel.liNoticeBoard = liDashboard;
        //            }
        //        }
        //    }

        //    return View(ObjViewModel);
        //}



        public PartialViewResult BindNoticeBoardReportLog(string sNoticeBoardId, string sRoleId)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = DapperHandler.FetchAll<NoticeBoard>(
                   DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardReportByNoticeId),
                   new { NOTICEBOARD_ID = sNoticeBoardId, ROLE_ID = sRoleId },
                   sAcademicYear
               );
                ObjViewModel.liNoticeBoardLog = result;
            }

            return PartialView(ObjViewModel);
        }

        public JsonResult GetNoticeBoardTitleByRole(string sRoleId)
        {

            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var result = DapperHandler.FetchAll<NoticeBoard>(
                    DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfo),
                    new { CLASS_YEAR = "", APPTYPE = "", SHIFT = "" },
                    sAcademicYear
                );
                
                if (result != null && result.Count > 0 && !string.IsNullOrEmpty(sRoleId))
                {
                    // Filter notices that target the selected role
                    var filteredResult = result.Where(x => x.ROLE_ID != null && x.ROLE_ID.Contains(sRoleId)).ToList();
                    return Json(filteredResult, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new List<NoticeBoard>(), JsonRequestBehavior.AllowGet);
        }

        #region Notice Board for admin
        public ActionResult NoticeBoard()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            //string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var FetchRole = (List<SUP_ROLE>)CMSHandler.FetchData<SUP_ROLE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupRole), sAcademicYear).DataSource.Data;
                ObjViewModel.RoleId = new SelectList(FetchRole, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);

                var FetchApptype = (List<SUP_APPLICATION_TYPE>)CMSHandler.FetchData<SUP_APPLICATION_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchApplicationTypeNew), sAcademicYear).DataSource.Data;
                ObjViewModel.AppType = new SelectList(FetchApptype, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE_ID, Common.SUP_APPLICATION_TYPE.APPLICATION_TYPE);

                var ClassYear = (List<SUP_CLASS_YEAR>)CMSHandler.FetchData<SUP_CLASS_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYearNew), sAcademicYear).DataSource.Data;
                ObjViewModel.ClassYear = new SelectList(ClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);

                var FetchShift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                ObjViewModel.Shift = new SelectList(FetchShift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);
            }
            return View(ObjViewModel);
        }

        public JsonResult FetchNoticeBoard(string sNoticeBoard_id)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            var objModel = new NoticeBoard();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                var liDashboard = DapperHandler.FetchAll<NoticeBoard>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardInfoById), new { NOTICEBOARD_ID = sNoticeBoard_id }, sAcademicYear);

                if (liDashboard != null && liDashboard.Count > 0)
                {
                    objModel.NOTICEBOARD_ID = liDashboard.FirstOrDefault().NOTICEBOARD_ID;
                    objModel.NOTICEBOARD = liDashboard.FirstOrDefault().NOTICEBOARD;
                    objModel.FILE_NAME = liDashboard.FirstOrDefault().FILE_NAME;
                    objModel.ROLE_ID = liDashboard.FirstOrDefault().ROLE_ID;
                    objModel.APPTYPE = liDashboard.FirstOrDefault().APPTYPE;
                    objModel.CLASS_YEAR = liDashboard.FirstOrDefault().CLASS_YEAR;
                    objModel.LINK_UPLOAD = liDashboard.FirstOrDefault().LINK_UPLOAD;
                    objModel.IS_ACTIVE = liDashboard.FirstOrDefault().IS_ACTIVE;
                    objModel.IS_DELETED = liDashboard.FirstOrDefault().IS_DELETED;
                    // New Fields
                    objModel.DESCRIPTION = liDashboard.FirstOrDefault().DESCRIPTION;
                    objModel.FILE_TYPE = liDashboard.FirstOrDefault().FILE_TYPE;
                    objModel.DUE_DATE = liDashboard.FirstOrDefault().DUE_DATE;
                    objModel.SHIFT = liDashboard.FirstOrDefault().SHIFT;
                    objModel.IS_COMMENT_ENABLE = liDashboard.FirstOrDefault().IS_COMMENT_ENABLE;
                    objModel.IS_FILE_UPLOAD_ENABLE = liDashboard.FirstOrDefault().IS_FILE_UPLOAD_ENABLE;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "FetchNoticeBoard", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "FetchNoticeBoard", ex.Message);
                }
            }
            return Json(objModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateNoticeBoard(string sJsonvalue, HttpPostedFileBase FILE_NAME)
        {
            string sSemesterType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

            var ObjJsonData = new JsonResultData();
            try
            {
                var Result = JsonConvert.DeserializeObject<NoticeBoard>(sJsonvalue);
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    if (Result != null)
                    {
                        // If file is uploaded
                        if (FILE_NAME != null && FILE_NAME.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(FILE_NAME.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/Content/assets"), fileName);
                            FILE_NAME.SaveAs(filePath);

                            // Save file name/path to the model (optional)
                            Result.FILE_NAME = "/Content/assets/" + fileName;
                        }

                        if (string.IsNullOrEmpty(Result.APPTYPE)) Result.APPTYPE = null;
                        if (string.IsNullOrEmpty(Result.CLASS_YEAR)) Result.CLASS_YEAR = null;
                        if (string.IsNullOrEmpty(Result.SHIFT)) Result.SHIFT = null;

                        if (string.IsNullOrEmpty(Result.SHIFT)) Result.SHIFT = null;

                        Result.ACADEMIC_YEAR = sAcademicYear;
                        Result.SEMESTER_TYPE = sSemesterType;

                        Result.DUE_DATE = CommonMethods.DateConvertionForPayU(Result.DUE_DATE);
                        object objResult;
                        if (!string.IsNullOrEmpty(Result.NOTICEBOARD_ID))
                        {
                            objResult = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.UpdateNoticeBoardInfoById), Result, null);
                        }
                        else
                        {
                            objResult = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.SaveNoticeBoardInfoById), Result, null);
                        }

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
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "UpdateNoticeBoard", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "UpdateNoticeBoard", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = ex.Message;
            }

            return Json(ObjJsonData);
        }

        public JsonResult DeleteNoticeBoard(string sNoticeBoard_id, string sIsdelete)
        {
            NoticeBoard objmodel = new NoticeBoard();
            var ObjJsonData = new JsonResultData();
            try
            {
                var Student = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.DeleteNoticeBoardInfoById), new { NOTICEBOARD_ID = sNoticeBoard_id, IS_DELETED = sIsdelete }, null);
                if (Student != null && Convert.ToInt32(Student) > 0)
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
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "DeleteNoticeBoard", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "DeleteNoticeBoard", ex.Message);
                }
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = ex.Message;
            }
            return Json(ObjJsonData);
        }
        #endregion


        #region SemesterMarkReportByChatForStudent
        public PartialViewResult SemesterMarkReportByChatForStudent()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            SUP_BATCH_DURATION objbatchduartion = new SUP_BATCH_DURATION();
            List<SEMESTER_MARKS> liSemestermarks = new List<SEMESTER_MARKS>();
            List<SEMESTER_MARKS> lifinalSemstermark = new List<SEMESTER_MARKS>();
            List<SEMESTER_MARKS> lisem = new List<SEMESTER_MARKS>();
            List<SEMESTER_MARKS> lipart = new List<SEMESTER_MARKS>();
            ResultArgs resultArgs = new ResultArgs();
            DataValue dv = new DataValue();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                dv.Add(Common.STUDENT_INFO.ACADEMIC_YEAR, sAcademicYear, EnumCommand.DataType.String);
                dv.Add(Common.STUDENT_INFO.STUDENT_ID, sUserId, EnumCommand.DataType.String);
                resultArgs = objViewModel.FetchStudentInfoById(dv, sAcademicYear);
                if (resultArgs.Success)
                {
                    if (resultArgs.DataSource.Table != null && resultArgs.DataSource.Table.Rows.Count > 0)
                    {
                        Session[Common.SESSION_VARIABLES.USER_INFO] = resultArgs.DataSource.Table;
                        Session[Common.SESSION_VARIABLES.CLASS_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_ID];
                        Session[Common.SESSION_VARIABLES.DEPARTMENT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.DEPT_ID];
                        Session[Common.SESSION_VARIABLES.SHIFT_ID] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.SHIFT_ID];
                        Session[Common.SESSION_VARIABLES.CLASS_NAME] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_NAME];
                        Session[Common.SESSION_VARIABLES.PHOTO] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.PHOTO];
                        Session[Common.SESSION_VARIABLES.ADMITTED_BATCH] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.ADMITTED_BATCH];
                        Session[Common.SESSION_VARIABLES.CLASS_LEVEL] = resultArgs.DataSource.Table.Rows[0][Common.USER_INFO.CLASS_LEVEL];
                    }
                }
                objbatchduartion.BATCH_ID = (Session[Common.SESSION_VARIABLES.ADMITTED_BATCH] != null) ? Session[Common.SESSION_VARIABLES.ADMITTED_BATCH].ToString() : string.Empty;
                objbatchduartion.APPLICATION_TYPE_ID = (Session[Common.SESSION_VARIABLES.CLASS_LEVEL] != null) ? Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString() : string.Empty;
                string sApplicationtype = (Session[Common.SESSION_VARIABLES.CLASS_LEVEL] != null) ? Session[Common.SESSION_VARIABLES.CLASS_LEVEL].ToString() : string.Empty;
                var BatchExpiredYear = (List<SUP_BATCH_DURATION>)CMSHandler.FetchData<SUP_BATCH_DURATION>(objbatchduartion, SQL.Student.StudentSQL.GetStudentSQL(StudentSQLCommads.FetchBatchExpiredYear)).DataSource.Data;
                if (BatchExpiredYear != null && BatchExpiredYear.Count > 0)
                {

                    for (int i = Convert.ToInt16(BatchExpiredYear.FirstOrDefault().BEGIN_YEAR); i < Convert.ToInt16(BatchExpiredYear.FirstOrDefault().EXPIRED_YEAR); i++)
                    {
                        var Studentmarks = (List<SEMESTER_MARKS>)CMSHandler.FetchData<SEMESTER_MARKS>(new SEMESTER_MARKS() { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear }, DashboardSQL.GetDashboardSQL(sApplicationtype.Equals("1") ? DashboardSQLCommands.FetchUgStudentMarks : DashboardSQLCommands.FetchStudentMarks), i.ToString()).DataSource.Data;
                        if (Studentmarks != null && Studentmarks.Count > 0)
                        {
                            liSemestermarks = liSemestermarks.Concat(Studentmarks).ToList();
                        }
                    }
                    if (liSemestermarks != null && liSemestermarks.Count > 0)
                    {
                        var lifinalSemestermarks = liSemestermarks.GroupBy(c => new
                        {
                            c.SEMESTER,
                            c.PART

                        })
                           .Select(t => new
                           {
                               STUDENT_ID = t.FirstOrDefault().STUDENT_ID,
                               NAME = t.FirstOrDefault().NAME,
                               M_PASS = t.FirstOrDefault().SEMESTER_NAME,
                               PART_NAME = t.FirstOrDefault().PART_NAME,
                               PART = t.FirstOrDefault().PART,
                               SEMESTER_NAME = t.FirstOrDefault().SEMESTER_NAME,
                               SEMESTER = t.FirstOrDefault().SEMESTER,
                               MAXIMUM = t.Sum(ta => Convert.ToUInt32(ta.MAXIMUM)),
                               START_DATE = t.FirstOrDefault().START_DATE,

                               TOTAL = t.Sum(ta => Convert.ToUInt32(ta.TOTAL)),
                           }).ToList();
                        if (lifinalSemestermarks != null)
                        {
                            foreach (var item in lifinalSemestermarks)
                            {
                                double percentage = ((double)Convert.ToInt32(item.TOTAL) / (double)Convert.ToInt32(item.MAXIMUM)) * 100;
                                lifinalSemstermark.Add(new SEMESTER_MARKS() { STUDENT_ID = item.STUDENT_ID, M_PASS = item.SEMESTER_NAME, TOTAL = (Math.Round(percentage)).ToString(), START_DATE = item.START_DATE, SEMESTER = item.SEMESTER, PART_NAME = item.PART_NAME, PART = item.PART });

                            }
                            objViewModel.liSemstermark = lifinalSemstermark.OrderBy(o => o.SEMESTER).ToList();

                            ViewBag.liSemstermark = lifinalSemstermark.OrderBy(o => o.SEMESTER);

                            var lisemster = lifinalSemestermarks.GroupBy(o => o.SEMESTER).ToList();
                            if (lisemster != null && lisemster.Count > 0)
                            {
                                foreach (var sem in lisemster)
                                {
                                    lisem.Add(new SEMESTER_MARKS() { SEMESTER = sem.FirstOrDefault().SEMESTER, SEMESTER_NAME = sem.FirstOrDefault().SEMESTER_NAME });
                                }
                            }
                            ViewBag.lisem = lisem.OrderBy(o => o.SEMESTER);
                            var parts = lifinalSemestermarks.GroupBy(o => o.PART).ToList();
                            if (parts != null && parts.Count > 0)
                            {
                                foreach (var part in parts)
                                {
                                    lipart.Add(new SEMESTER_MARKS() { PART = part.FirstOrDefault().PART, PART_NAME = part.FirstOrDefault().PART_NAME });
                                }
                            }
                            ViewBag.lipart = lipart.OrderBy(o => o.PART);

                        }
                    }
                }
            }
            return PartialView(objViewModel);
        }
        #endregion

        #region CollegeTopperByExamSettong
        public PartialViewResult CollegeTopperByExamSettong()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamSettingByPublished)).DataSource.Data;
                if (ExamSetting != null && ExamSetting.Count > 0)
                {
                    var listudentList = DapperHandler.FetchAll<STU_PERSONAL_INFO>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchCollegeTopperByExamSetting), null, ExamSetting.FirstOrDefault().ACADEMIC_YEAR);
                    ObjViewModel.liStudent = listudentList;
                }
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region PassPercentageForCoe
        public PartialViewResult PassPercentageForCoe()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            int totalfailcount = 0;
            int totalpasscount = 0;
            List<SEMESTER_RESULT> liPassPercentage = new List<SEMESTER_RESULT>();
            List<SEMESTER_RESULT> lisem = new List<SEMESTER_RESULT>();
            List<SEMESTER_RESULT> liclasses = new List<SEMESTER_RESULT>();
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamSettingByPublished)).DataSource.Data;
                if (ExamSetting != null && ExamSetting.Count > 0)
                {
                    var listudentList = DapperHandler.FetchAll<SEMESTER_RESULT>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchPassPercentageByExamSetting), null, ExamSetting.FirstOrDefault().ACADEMIC_YEAR).ToList();
                    var liClassYear = listudentList.GroupBy(o => o.CLASS_YEAR).ToList().OrderBy(o => o.FirstOrDefault().CLASS_YEAR).ToList();
                    var liSemester = listudentList.GroupBy(o => o.SEMESTER_ID).ToList().OrderBy(o => o.FirstOrDefault().SEMESTER_ID).ToList();
                    if (liClassYear != null && liClassYear.Count > 0)
                    {
                        foreach (var liclass in liClassYear)
                        {
                            foreach (var sem in liSemester)
                            {
                                var StuList = listudentList.Where(x => x.CLASS_YEAR == liclass.FirstOrDefault().CLASS_YEAR && x.SEMESTER_ID == sem.FirstOrDefault().SEMESTER_ID).ToList();
                                if (StuList.Count > 0 && StuList != null)
                                {
                                    var Fail = StuList.Where(x => x.RESULT.Contains('F') || x.RESULT.Contains('A')).ToList();
                                    totalfailcount += Convert.ToInt32(Fail.Count());
                                    var passed = (Convert.ToInt32(StuList.Count)) - (Convert.ToInt32(Fail.Count));
                                    totalpasscount += Convert.ToInt32(passed);
                                    double percentage = (Convert.ToDouble(passed) / Convert.ToDouble(StuList.Count)) * 100;
                                    liPassPercentage.Add(new SEMESTER_RESULT { CLASS_YEAR = liclass.FirstOrDefault().CLASS_YEAR, SEMESTER_ID = sem.FirstOrDefault().SEMESTER_ID, TOTAL = (Math.Round(percentage)).ToString() });
                                }
                            }
                        }
                    }


                    if (liSemester != null && liSemester.Count > 0)
                    {
                        foreach (var sem in liSemester)
                        {
                            lisem.Add(new SEMESTER_RESULT() { SEMESTER_ID = sem.FirstOrDefault().SEMESTER_ID, SEMESTER_NAME = sem.FirstOrDefault().SEMESTER_NAME });
                        }
                    }
                    if (liClassYear != null && liClassYear.Count > 0)
                    {
                        foreach (var sem in liClassYear)
                        {
                            liclasses.Add(new SEMESTER_RESULT() { CLASS_YEAR = sem.FirstOrDefault().CLASS_YEAR });
                        }
                    }

                    ViewBag.liclass = liclasses;
                    ViewBag.lisem = lisem;
                    ViewBag.lisemester = liPassPercentage;

                    ObjViewModel.liSemesterChart = listudentList;
                }
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region FeedBackTopRatingStaff
        public PartialViewResult FeedBackTopRatingStaff()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            int sAcademicyear = 0;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            sAcademicyear = Convert.ToInt32(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]);
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var FeedbackList = DapperHandler.FetchAll<FBFEEDBACK_SETTINGS>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchFeedbackSettingByAcademicYear), null, sAcademicYear);
                if (FeedbackList != null && FeedbackList.Count > 0)
                {

                }
                else
                {
                    sAcademicyear = sAcademicyear - 1;
                    sAcademicYear = sAcademicyear.ToString();
                }

                var staffFeedback = DapperHandler.FetchAll<FBEVALUATION>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchFeedbackStaffReportByAcademicYear), null, sAcademicYear);
                ObjViewModel.liFeedBack = staffFeedback;
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region TimeTableForStudent
        public PartialViewResult TimeTableForStudent()
        {
            DashboardViewModel ObjviewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                TimeTable objModel = new TimeTable();
                var BindStudentCourseInfo = new List<TimeTable>();
                var OptionCourseInfo = new List<TimeTable>();
                var FetchStudentClsId = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new STU_CLASS() { STUDENT_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByStudentId), sAcademicYear).DataSource.Data;
                if (FetchStudentClsId != null && FetchStudentClsId.Count > 0)
                {
                    objModel.Class_Id = FetchStudentClsId.FirstOrDefault().CLASS_ID;
                }

                var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalHoursAndDaysByClassId), sAcademicYear).DataSource.Data;
                if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        ObjviewModel.lstHours = FetchHours;
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                        ObjviewModel.lstDayOrders = FetchDayOrders;
                }

                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableClassWiseListByClassId);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Semester_Id.ToUpper(), sActiveSemester);
                BindStudentCourseInfo = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                ObjviewModel.liTimetable = BindStudentCourseInfo;
            }
            return PartialView(ObjviewModel);
        }
        #endregion

        #region TimeTableForStaff
        public PartialViewResult TimeTableForStaff()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            stf_Personal_Info objStfModel = new stf_Personal_Info();
            ResultArgs resultArgs = new ResultArgs();
            DataValue dv = new DataValue();
            string sUserId = string.Empty;
            string sSQL = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                objStfModel.STAFF_ID = sUserId;
                sSQL = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAcademicActivityByUserId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sUserId);
                objViewModel.liActivity = (List<AcademicActivityModel>)CMSHandler.FetchData<AcademicActivityModel>(null, sSQL, sAcademicYear).DataSource.Data;
                var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicYear).DataSource.Data;
                if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        objViewModel.lstHours = FetchHours;
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                        objViewModel.lstDayOrders = FetchDayOrders;
                }
                objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicYear).DataSource.Data;

                objViewModel.liSubstituteList = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubstituteHourByStaffId), sAcademicYear).DataSource.Data;
            }
            return PartialView(objViewModel);
        }
        #endregion

        #region DayOrder
        public PartialViewResult DayOrder()
        {
            return PartialView();
        }
        #endregion

        #region CurrentTime
        public PartialViewResult CurrentTime()
        {
            return PartialView();
        }
        #endregion

        #region OptionalCourseCountForStudent
        public PartialViewResult OptionalCourseCountForStudent()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sSQL = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                objViewModel.liCourseDetails = DapperHandler.FetchAll<EXAM_REGISTRATION>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchOptionalCourselistForStudent), new { STUDENT_ID = sUserId }, sAcademicYear);
            }
            return PartialView(objViewModel);
        }
        #endregion

        #region AbsentCountForStudent
        public PartialViewResult AbsentCountForStudent()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sSQL = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                objViewModel.liAttendanceReport = DapperHandler.FetchAll<ATTENDANCE_REPORT>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchAbsenteeCountForStudent), new { STUDENT_ID = sUserId }, sAcademicYear);
            }
            return PartialView(objViewModel);
        }
        #endregion

        #region YearWiseExaminationChart

        public PartialViewResult YearWiseExaminationChart()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            int totalfailcount = 0;
            int totalpasscount = 0;
            List<SEMESTER_RESULT> liPassPercentage = new List<SEMESTER_RESULT>();
            List<SEMESTER_RESULT> liPass = new List<SEMESTER_RESULT>();
            List<SEMESTER_RESULT> liFail = new List<SEMESTER_RESULT>();
            List<SEMESTER_RESULT> lisem = new List<SEMESTER_RESULT>();
            List<SEMESTER_RESULT> liclasses = new List<SEMESTER_RESULT>();
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var ExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamSettingByPublished)).DataSource.Data;
                if (ExamSetting != null && ExamSetting.Count > 0)
                {
                    var listudentList = DapperHandler.FetchAll<SEMESTER_RESULT>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchPassPercentageByExamSetting), null, ExamSetting.FirstOrDefault().ACADEMIC_YEAR).ToList();
                    var liClassYear = listudentList.GroupBy(o => o.CLASS_YEAR).ToList().OrderBy(o => o.FirstOrDefault().CLASS_YEAR).ToList();
                    var liSemester = listudentList.GroupBy(o => o.SEMESTER_ID).ToList().OrderBy(o => o.FirstOrDefault().SEMESTER_ID).ToList();
                    if (liClassYear.Count > 0 && liClassYear != null)
                    {
                        foreach (var liclass in liClassYear)
                        {
                            foreach (var sem in liSemester)
                            {
                                var StuList = listudentList.Where(x => x.CLASS_YEAR == liclass.FirstOrDefault().CLASS_YEAR && x.SEMESTER_ID == sem.FirstOrDefault().SEMESTER_ID).ToList();
                                if (StuList.Count > 0 && StuList != null)
                                {
                                    var Fail = StuList.Where(x => x.RESULT.Contains('F') || x.RESULT.Contains('A')).ToList();
                                    totalfailcount += Convert.ToInt32(Fail.Count());
                                    var passed = (Convert.ToInt32(StuList.Count)) - (Convert.ToInt32(Fail.Count));
                                    totalpasscount += Convert.ToInt32(passed);
                                    double percentage = (Convert.ToDouble(passed) / Convert.ToDouble(StuList.Count)) * 100;
                                    liPassPercentage.Add(new SEMESTER_RESULT { CLASS_YEAR = liclass.FirstOrDefault().CLASS_YEAR, SEMESTER_ID = sem.FirstOrDefault().SEMESTER_ID, TOTAL = StuList.Count.ToString(), PASSED = passed.ToString(), FAILED = Convert.ToString(Fail.Count) });
                                    liPass.Add(new SEMESTER_RESULT { CLASS_YEAR = liclass.FirstOrDefault().CLASS_YEAR, SEMESTER_ID = sem.FirstOrDefault().SEMESTER_ID, TOTAL = passed.ToString() });
                                    liFail.Add(new SEMESTER_RESULT { CLASS_YEAR = liclass.FirstOrDefault().CLASS_YEAR, SEMESTER_ID = sem.FirstOrDefault().SEMESTER_ID, TOTAL = Fail.ToString() });
                                }
                            }
                        }
                    }
                    if (liSemester != null && liSemester.Count > 0)
                    {
                        foreach (var sem in liSemester)
                        {
                            lisem.Add(new SEMESTER_RESULT() { SEMESTER_ID = sem.FirstOrDefault().SEMESTER_ID, SEMESTER_NAME = sem.FirstOrDefault().SEMESTER_NAME });
                        }
                    }
                    if (liClassYear != null && liClassYear.Count > 0)
                    {
                        foreach (var sem in liClassYear)
                        {
                            liclasses.Add(new SEMESTER_RESULT() { CLASS_YEAR = sem.FirstOrDefault().CLASS_YEAR });
                        }
                    }

                    ViewBag.liclass = liclasses;
                    ViewBag.lisem = lisem;
                    ViewBag.lisemester = liPassPercentage;


                    ObjViewModel.liSemesterChart = listudentList;
                }
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region Event
        public PartialViewResult Event()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.liCalenderEvents = (List<CALENDER_EVENTS>)CMSHandler.FetchData<CALENDER_EVENTS>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate), sAcademicYear).DataSource.Data;
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region BirthDayListForStudent
        public PartialViewResult BirthDayListForStudent()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.liStudentBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                        DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentTodayBirthDayList), sAcademicYear).DataSource.Data;

            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region BirthDayListForStaff
        public PartialViewResult BirthDayListForStaff()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch Staff Birthday Day Celebraties With Image ,..
                ObjViewModel.liStaffBirthDayList = (List<BIRTHDAY_CELEBRITIES>)CMSHandler.FetchData<BIRTHDAY_CELEBRITIES>(null,
                    DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStaffTodayBirthDayList), sAcademicYear).DataSource.Data;
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region UpcomingMeetings
        public PartialViewResult UpcomingMeetings()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

            ObjViewModel.UserId = sUserId;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                // Fetch Upcoming Meetings
                ObjViewModel.liUpcomingMeetings = (List<MeetingCreation>)CMSHandler.FetchData<MeetingCreation>(null,
                    DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchUpcomingMeetings), sAcademicYear).DataSource.Data;
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region Calender
        public PartialViewResult Calender()
        {
            return PartialView();
        }
        #endregion

        #region TotalStudentCount
        public PartialViewResult TotalStudentCount()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ResultArgs resultArgs = new ResultArgs();
                resultArgs = ObjViewModel.NoOfStudents(sAcademicYear);
                ViewBag.TotalStudents = (string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult));
            }

            return PartialView();
        }
        #endregion

        #region TotalStaffCount
        public PartialViewResult TotalStaffCount()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ResultArgs resultArgs = new ResultArgs();
                resultArgs = ObjViewModel.NoOfStaff();
                ViewBag.TotalStaff = string.IsNullOrEmpty(resultArgs.StringResult) ? 0 : Convert.ToDouble(resultArgs.StringResult);
            }

            return PartialView();
        }
        #endregion

        #region DashBoard
        public ActionResult DashBoard()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = string.Empty;
            sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSql = string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var sRole = DapperHandler.FetchAll<USER_ROLES_RIGHTS>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRoleByUserId), new { @USER_ID = sUserId }, sAcademicYear);
                sSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRoleBasedDashboardMenu).Replace(Common.Delimiter.QUS + Common.SUP_ROLE.ROLE_ID, sRole.FirstOrDefault().ROLE_ID);
                ObjViewModel.liDashBoardModel = DapperHandler.FetchAll<DashBoardModel>(sSql);
                if (sRole != null && sRole.Count > 0)
                {
                    if (sRole.FirstOrDefault().ROLE_ID == "5")
                    {
                        ObjViewModel.liStudent = DapperHandler.FetchAll<STU_PERSONAL_INFO>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentDetailsbyUserId), new { STUDENT_ID = sUserId }, sAcademicYear);
                        if (ObjViewModel.liStudent != null && ObjViewModel.liStudent.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(ObjViewModel.liStudent.FirstOrDefault().STU_IIC_ID))
                            {
                                ObjViewModel.liDashBoardModel.FirstOrDefault().ROLE_ID = "0";
                            }
                        }

                    }
                }

            }
            else
            {
                return RedirectToAction("ErrorMessage", "error");
            }
            return View(ObjViewModel);
        }
        #endregion

        #region DashboardMenu
        public ActionResult DashboardMenu()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var sup_role = DapperHandler.FetchAll<SUP_ROLE>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRole));
                if (sup_role.Count > 0 && sup_role != null)
                {
                    ObjViewModel.lirole = new SelectList(sup_role, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                }
                var Limenu = DapperHandler.FetchAll<DashBoardModel>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDashboardMenuList));
                if (Limenu.Count > 0 && Limenu != null)
                {
                    ObjViewModel.liMenu = new SelectList(Limenu, Common.SUP_ROLE.DASHBOARD_ID, Common.SUP_ROLE.SHORT_NAME);
                }
            }
            return View(ObjViewModel);
        }
        #endregion

        #region BindDashBoardMenuList
        public ActionResult BindDashBoardMenuList()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjViewModel.liDashBoardModel = DapperHandler.FetchAll<DashBoardModel>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDashboardMenuList));
            }
            return View(ObjViewModel);
        }
        #endregion

        #region FetchDashBoardMenu
        public JsonResult FetchDashBoardMenu(string sDashBoard_id)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            var objModel = new DashBoardModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            try
            {
                var liDashboard = DapperHandler.FetchAll<DashBoardModel>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.BindDashBoardByDashBoardId), new { @DASHBOARD_ID = sDashBoard_id }, sAcademicYear);

                objModel.DASHBOARD_ID = liDashboard.FirstOrDefault().DASHBOARD_ID;
                objModel.SHORT_NAME = liDashboard.FirstOrDefault().SHORT_NAME;
                objModel.ACTION_NAME = liDashboard.FirstOrDefault().ACTION_NAME;
                objModel.CONTROLLER_NAME = liDashboard.FirstOrDefault().CONTROLLER_NAME;
                objModel.IS_ORDER = liDashboard.FirstOrDefault().IS_ORDER;
                objModel.IS_GROUP = liDashboard.FirstOrDefault().IS_GROUP;

            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("StudentController", "FetchStudentForTC", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("StudentController", "FetchStudentForTC", ex.Message);
                }
                sResult = "Fail To Fetch Record(s) ...!";

            }
            return Json(objModel);
        }
        #endregion

        #region UpdateDashboardMenu
        public JsonResult UpdateDashboardMenu(string sJsonvalue)
        {

            DashBoardModel objModel = new DashBoardModel();
            object objResult = new object();
            var ObjJsonData = new JsonResultData();
            var Result = JsonConvert.DeserializeObject<DashBoardModel>(sJsonvalue);
            // string sAcademicYear = string.Empty;
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string SQL = string.Empty;

            if (Result != null)
            {
                if (!string.IsNullOrEmpty(Result.DASHBOARD_ID))
                {
                    objResult = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.UpdateDashBoardMenu), Result, null);

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
                else
                {

                    objResult = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.InsertDashBoardMenu), Result, null);
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
            }

            return Json(ObjJsonData);
        }
        #endregion

        #region DeleteDashboardMenu

        public JsonResult DeleteDashboardMenu(string sDashBoard_id)
        {
            DashBoardModel objmodel = new DashBoardModel();
            var ObjJsonData = new JsonResultData();
            try
            {
                var Student = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.DeleteDashBoardmenu), new { @DASHBOARD_ID = sDashBoard_id }, null);
                if (Student != null && Convert.ToInt32(Student) > 0)
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
            catch (Exception)
            {

                throw;
            }
            return Json(objmodel);
        }
        #endregion

        #region RoleBasedDashboard
        public ActionResult RoleBasedDashboard()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var sup_role = DapperHandler.FetchAll<SUP_ROLE>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRole));
                if (sup_role.Count > 0 && sup_role != null)
                {
                    ObjViewModel.lirole = new SelectList(sup_role, Common.SUP_ROLE.ROLE_ID, Common.SUP_ROLE.ROLE_NAME);
                }
                var Limenu = DapperHandler.FetchAll<DashBoardModel>(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDashboardMenuList));
                if (Limenu.Count > 0 && Limenu != null)
                {
                    ObjViewModel.liMenu = new SelectList(Limenu, Common.SUP_ROLE.DASHBOARD_ID, Common.SUP_ROLE.SHORT_NAME);
                }
            }
            return View(ObjViewModel);
        }


        public ActionResult BindRoleBasedDashBoardMenu(string sRoleId)
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                string sSql = string.Empty;
                sSql = DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchRoleBasedDashboardMenu).Replace(Common.Delimiter.QUS + Common.SUP_ROLE.ROLE_ID, sRoleId);
                ObjViewModel.liDashBoardModel = DapperHandler.FetchAll<DashBoardModel>(sSql);
            }
            return View(ObjViewModel);
        }


        public JsonResult InsertRoleBasedDashBoardMenu(string sJsonvalue)
        {

            DashBoardModel objModel = new DashBoardModel();
            object objResult = new object();
            var ObjJsonData = new JsonResultData();
            var Result = JsonConvert.DeserializeObject<DashboardViewModel.JsonDashBoard>(sJsonvalue);
            // string sAcademicYear = string.Empty;
            var sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sSql = string.Empty;

            if (Result != null)
            {
                sSql = "INSERT INTO ROLEBASED_DASHBOARD_MENU (ROLE_ID, DASHBOARD_ID, IS_ACTIVE) VALUES";
                foreach (var item in Result.JsonMenuList)
                {
                    sSql += "(" + item.sroleId + "," + item.sMenuId + ",1),";
                }
                sSql = sSql.Remove(sSql.Length - 1, 1);
                sSql += ";";
                objResult = DapperHandler.SaveOrUpdate(sSql);
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

            return Json(ObjJsonData);
        }

        public JsonResult ActiveDeActiveDashBoardMenu(string sUserRole_id, string sActive)
        {
            DashBoardModel objmodel = new DashBoardModel();
            var ObjJsonData = new JsonResultData();
            try
            {
                var Student = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.ActiveDeActiveDashBoardRole), new { @USER_ROLE_ID = sUserRole_id, @IS_ACTIVE = sActive }, null);
                if (Student != null && Convert.ToInt32(Student) > 0)
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
            catch (Exception)
            {

                throw;
            }
            return Json(objmodel);
        }
        #endregion

        #region DateWiseFeeCollectionSettlement
        public PartialViewResult DateWiseFeeCollectionSettlement()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            List<FEE_TRANSACTION> SettleDate = new List<FEE_TRANSACTION>();
            List<FEE_TRANSACTION> Frequecy = new List<FEE_TRANSACTION>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            try
            {
                var DateWiseFeeReport = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchDateFrequencyBasedSettlementAmount), sAcademicYear).DataSource.Data;
                if (DateWiseFeeReport != null && DateWiseFeeReport.Count > 0)
                {
                    ObjViewModel.liFeeTransaction = DateWiseFeeReport;
                    var Groupdate = DateWiseFeeReport.GroupBy(s => s.SETTLEMENT_DATE);
                    foreach (var item in Groupdate)
                    {
                        SettleDate.Add(new FEE_TRANSACTION() { SETTLEMENT_DATE = item.FirstOrDefault().SETTLEMENT_DATE });
                    }
                    var FrequencyList = DateWiseFeeReport.GroupBy(s => s.FREQUENCY);
                    foreach (var item in FrequencyList)
                    {
                        Frequecy.Add(new FEE_TRANSACTION() { FREQUENCY = item.FirstOrDefault().FREQUENCY, FREQUENCY_NAME = item.FirstOrDefault().FREQUENCY_NAME });
                    }
                    ViewBag.SettleDate = SettleDate.OrderBy(o => o.SETTLEMENT_DATE);
                    ViewBag.Frequecy = Frequecy.OrderBy(o => o.FREQUENCY);
                    ViewBag.liFeeTransaction = DateWiseFeeReport.OrderBy(s => s.FREQUENCY);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", ex.Message);
                }
            }
            return PartialView(ObjViewModel);
        }
        #endregion

        #region TotalFreQuencyCollectionSettlement
        public PartialViewResult TotalFreQuencyCollectionSettlement()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            List<FEE_TRANSACTION> Frequecy = new List<FEE_TRANSACTION>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DateWiseFeeReport = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchFrequencyBasedTotalSettlement), sAcademicYear).DataSource.Data;
                    if (DateWiseFeeReport != null && DateWiseFeeReport.Count > 0)
                    {
                        ObjViewModel.liFeeTransaction = DateWiseFeeReport;
                        var FrequencyList = DateWiseFeeReport.GroupBy(s => s.FREQUENCY);
                        foreach (var item in FrequencyList)
                        {
                            Frequecy.Add(new FEE_TRANSACTION() { FREQUENCY = item.FirstOrDefault().FREQUENCY, FREQUENCY_NAME = item.FirstOrDefault().FREQUENCY_NAME });
                        }
                        ViewBag.Frequecy = Frequecy.OrderBy(o => o.FREQUENCY);
                        ViewBag.Transaction = DateWiseFeeReport.OrderBy(s => s.FREQUENCY);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", ex.Message);
                    }
                }
            }
            else
            {

            }

            return PartialView();
        }
        #endregion

        #region FreQuencyNotPaidList
        public PartialViewResult FreQuencyNotPaidList()
        {
            DashboardViewModel ObjViewModel = new DashboardViewModel();
            List<FEE_TRANSACTION> Frequecy = new List<FEE_TRANSACTION>();
            List<FEE_TRANSACTION> LiNotPaid = new List<FEE_TRANSACTION>();
            List<FEE_TRANSACTION> LiPaid = new List<FEE_TRANSACTION>();
            int Pcount = 0;
            int Fcount = 0;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var DateWiseFeeReport = (List<FEE_TRANSACTION>)CMSHandler.FetchData<FEE_TRANSACTION>(null, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNotPaidListByFrequency), sAcademicYear).DataSource.Data;
                    if (DateWiseFeeReport != null && DateWiseFeeReport.Count > 0)
                    {
                        var FrequencyList = DateWiseFeeReport.GroupBy(s => s.FREQUENCY_ID);
                        foreach (var item in FrequencyList)
                        {
                            Frequecy.Add(new FEE_TRANSACTION() { FREQUENCY_ID = item.FirstOrDefault().FREQUENCY_ID, FREQUENCY_NAME = item.FirstOrDefault().FREQUENCY_NAME });
                            var Students = DateWiseFeeReport.Where(s => s.FREQUENCY_ID == item.FirstOrDefault().FREQUENCY_ID).ToList();
                            if (Students != null && Students.Count > 0)
                            {
                                Fcount = 0;
                                Pcount = 0;
                                Fcount = Students.Where(s => s.STATUS == "0").Count();
                                Pcount = Students.Where(s => s.STATUS == "1").Count();
                                LiPaid.Add(new FEE_TRANSACTION() { FREQUENCY_ID = item.FirstOrDefault().FREQUENCY_ID, PCOUNT = Pcount.ToString() });
                                LiNotPaid.Add(new FEE_TRANSACTION() { FREQUENCY_ID = item.FirstOrDefault().FREQUENCY_ID, FCOUNT = Fcount.ToString() });
                            }
                        }
                        ViewBag.Frequecy = Frequecy.OrderBy(o => o.FREQUENCY_ID);
                        ViewBag.LiPaid = LiPaid.OrderBy(s => s.FREQUENCY_ID);
                        ViewBag.LiNotPaid = LiNotPaid.OrderBy(s => s.FREQUENCY_ID);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", ex.Message);
                    }
                }
            }
            else
            {

            }

            return PartialView();
        }
        #endregion


        public PartialViewResult LMSstudent()
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sUserId = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sDayOrder = string.Empty;
            AssignmentModel objAssignmentModel = new AssignmentModel();
            StudentController student = new StudentController();
            var liArearList = new List<SEMESTER_RESULT>();
            //Get All The Regular Papers By Student_Id...
            var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objAssignmentModel,
              SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
            var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objAssignmentModel,
                SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;
            if (liRegularCourses != null && liRegularCourses.Count > 0)
            {
                var liTemp = liRegularCourses.Where(o => o.COURSE_STRUCTURE_ID == "579").ToList();
                //Loop Optional Papers To Remove Unnecessary Courses...
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
                        else
                        {
                            liRegularCourses.Add(item);
                        }
                    }
                }
                var liClass = liRegularCourses.GroupBy(x => x.CLASS_ID).Select(x => new EXAM_REGISTRATION() { CLASS_ID = x.FirstOrDefault().CLASS_ID }).ToList();
                string sClassIds = string.Join(",", liClass.Select(x => x.CLASS_ID).ToArray());

                var liCourse = liRegularCourses.GroupBy(x => x.COURSE_ROOT_ID).Select(x => new EXAM_REGISTRATION() { COURSE_ID = x.FirstOrDefault().COURSE_ROOT_ID }).ToList();
                string sCourseIds = string.Join(",", liCourse.Select(x => x.COURSE_ID).ToArray());
                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableClassWiseListByClassId);
                sSQL = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchSettingForStudentQuizEntryByClassAndCourseIds);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                var liFetchQuizSetting = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(null, sSQL, sAcademicYear).DataSource.Data;
                if (liFetchQuizSetting != null && liFetchQuizSetting.Count > 0)
                {
                    foreach (var item in liFetchQuizSetting)
                    {
                        item.STATUS = student.ValidateQuizForSetWise(item.SETTING_ID, sUserId, item.COURSE_ID, sAcademicYear);
                    }
                }
                objViewModel.liQuizSetting = liFetchQuizSetting;


                string sSQL1 = string.Empty;
                sSQL1 = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchActivelmsassignmentsByStudentId);
                sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sUserId);


                var liActiveAssignment = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(null, sSQL1, sAcademicYear).DataSource.Data;
                if (liActiveAssignment != null && liActiveAssignment.Count > 0)
                {
                    objViewModel.liassignmentsetting = liActiveAssignment;
                }
                objViewModel.liCourseDetails = liRegularCourses;
                objViewModel.liExamRegistrationList = liRegularCourses.OrderBy(o => o.COURSE_ORDER).ToList();


            }

            return PartialView(objViewModel);
        }






        public PartialViewResult NoticeBoardForAdmin(string sRoleId = "")

        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string userId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string RoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;

            try
            {
                string sUserRoleId = string.Empty;
                if (!string.IsNullOrEmpty(sRoleId))
                {
                    sUserRoleId = sRoleId;
                }
                else
                {
                    sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
                }

                if (!string.IsNullOrEmpty(sUserRoleId))
                {
                    DataValue dv = new DataValue();
                    if (RoleId == Common.SupRole.STUDENT)
                    {
                        var liStudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() { STUDENT_ID = userId }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchStudentacademicDetails), sAcademicYear).DataSource.Data;
                        if(liStudent !=null && liStudent.Count > 0)
                        {
                            var classYear = liStudent.FirstOrDefault().CLASS_YEAR;
                            var classLevel = liStudent.FirstOrDefault().CLASS_LEVEL;
                            var shift = liStudent.FirstOrDefault().SHIFT;
                            objViewModel.liNoticeBoard = (List<NoticeBoard>)CMSHandler.FetchData<NoticeBoard>(new NoticeBoard() {SHIFT= shift,CLASS_YEAR= classYear,APPTYPE= classLevel, ROLE_ID = sUserRoleId, USER_ID = userId }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardByUserIdforStudent), sAcademicYear).DataSource.Data;

                        }
                    }
                    else
                    {
                        objViewModel.liNoticeBoard = (List<NoticeBoard>)CMSHandler.FetchData<NoticeBoard>(new NoticeBoard() { ROLE_ID = sUserRoleId, USER_ID = userId }, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchNoticeBoardByUserId), sAcademicYear).DataSource.Data;

                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", ex.Message);
                }
            }
            return PartialView(objViewModel);
        }

        public PartialViewResult FeesLink(string sRoleId = "")
        {
            DashboardViewModel objViewModel = new DashboardViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            try
            {
                string sUserRoleId = string.Empty;
                if (!string.IsNullOrEmpty(sRoleId))
                {
                    sUserRoleId = sRoleId;
                }
                else
                {
                    sUserRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
                }

                if (!string.IsNullOrEmpty(sUserRoleId))
                {
                    DataValue dv = new DataValue();
                    objViewModel.liNoticeBoard = (List<NoticeBoard>)CMSHandler.FetchData<NoticeBoard>(new NoticeBoard() { ROLE_ID = sUserRoleId }, DashboardSQL.GetNoticeBoardForAdminSQL(), sAcademicYear).DataSource.Data;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DashBoardController", "DateWiseFeeCollectionSettlement", ex.Message);
                }
            }
            return PartialView(objViewModel);
        }


        [HttpPost]
        //public JsonResult LogNoticeBoardView(int noticeId)
        //{
        //    try
        //    {
        //        if (Session == null)
        //            return Json(new { success = false, message = "Session is null" });

        //        string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] == null) ? "Unknown" : Session[Common.SESSION_VARIABLES.USER_ID].ToString();
        //        string sUserName = (Session[Common.SESSION_VARIABLES.USER_NAME] == null) ? "Unknown" : Session[Common.SESSION_VARIABLES.USER_NAME].ToString();
        //        string sRoleName = (Session[Common.SESSION_VARIABLES.ROLE_NAME] == null) ? "Unknown" : Session[Common.SESSION_VARIABLES.ROLE_NAME].ToString();

        //        // Save view tracking to database
        //        DapperHandler.Execute(
        //            DashboardSQL.GetDashboardSQL(DashboardSQLCommands.SaveNoticeViewTracking),
        //            new { NOTICEBOARD_ID = noticeId, USER_ID = sUserId },
        //            (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
        //        );

        //        // Log the view in the ErrorLog as a simple way to "notify" admin for now
        //        using (ErrorLog objlog = new ErrorLog())
        //        {
        //            objlog.WriteError("NoticeBoardTracking", "LogNoticeBoardView", "NoticeID: " + noticeId, 
        //                $"Notice viewed by {sUserName} ({sRoleName}) - UserID: {sUserId}");
        //        }

        //        return Json(new { success = true, message = "View logged successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error: " + ex.Message });
        //    }
        //}

        public JsonResult SaveNoticeBoardResponse(string sJsonValue, HttpPostedFileBase FILE_PATH)
        {
            var ObjJsonData = new JsonResultData();
            try
            {
                var Result = JsonConvert.DeserializeObject<dynamic>(sJsonValue);
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sRoleName = (Session[Common.SESSION_VARIABLES.ROLE_NAME] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.ROLE_NAME].ToString();
                string sRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] == null) ? string.Empty : Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString();

                string filePath = null;
                if (FILE_PATH != null && FILE_PATH.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(FILE_PATH.FileName);
                    string fullPath = Path.Combine(Server.MapPath("~/Content/assets/notice_uploads"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Content/assets/notice_uploads")))
                        Directory.CreateDirectory(Server.MapPath("~/Content/assets/notice_uploads"));

                    FILE_PATH.SaveAs(fullPath);
                    filePath = "/Content/assets/notice_uploads/" + fileName;
                }

                var param = new
                {
                    NOTICEBOARD_ID = Result.NOTICEBOARD_ID,
                    USER_ID = sUserId,
                    USER_ROLE = sRoleId,
                    ACADEMIC_YEAR = sAcademicYear,
                    SEMESTER_TYPE = "", // Can be fetched if needed
                    FILE_PATH = filePath,
                    COMMENTS = Result.COMMENTS,
                    IS_ACCEPTED = Result.IS_ACCEPTED ?? 0,
                    IS_CANCELED = Result.IS_CANCELED ?? 0,
                    CREATED_BY = sUserId
                };

                var objResult = DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.SaveNoticeBoardLog), param, sAcademicYear);

                if (objResult != null && Convert.ToInt32(objResult) > 0)
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                    ObjJsonData.Message = "Response saved successfully!";
                }
                else
                {
                    ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
                    ObjJsonData.Message = "Failed to save response.";
                }
            }
            catch (Exception ex)
            {
                ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                ObjJsonData.Message = ex.Message;
            }
            return Json(ObjJsonData);
        }

        [HttpPost]
        public JsonResult SaveNoticeViewTracking(string noticeId)
        {
            try
            {
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();

                var param = new
                {
                    NOTICEBOARD_ID = noticeId,
                    USER_ID = sUserId,
                    ACADEMIC_YEAR = sAcademicYear
                };

                DapperHandler.SaveOrUpdate(DashboardSQL.GetDashboardSQL(DashboardSQLCommands.SaveNoticeViewTracking), param, sAcademicYear);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
