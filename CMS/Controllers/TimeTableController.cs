using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.DAO;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using CMS.SQL.TimeTable;
using CMS.SQL.SupportData;
using CMS.Utility;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CMS.SQL.Examination;
using CMS.SQL.Dashboard;
using System.Web.Http.Results;
using CMS.SQL.Student;
using System.Windows.Diagnostics;
using Rotativa;
using System.IO;
using PdfSharp.Pdf;
using Microsoft.Office.Interop.Excel;
using PdfSharp.Drawing;
using PdfSharp;
using static System.Web.Razor.Parser.SyntaxConstants;



namespace CMS.Controllers
{
    public class TimeTableController : Controller
    {
        // GET: TimeTable
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData ObjJsonData = new JsonResultData();
        public ActionResult Index()
        {
            GenerateTimeTableForStaticHours();
            return View();
        }
        #region Methods
        private void GenerateTimeTableForStaticHours()
        {
            //Declaration
            string sAcademicYear = string.Empty;
            sAcademicYear = (string.IsNullOrEmpty(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()) ? string.Empty : Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString());
            var settings = new List<TimeTableSetting>();
            TimeTableSetting objSetting = new TimeTableSetting();
            var departmentWiseTemplate = new TT_DEPARTMENT_WISE_TEMPLATE();
            var departmentWiseTemplates = new List<TT_DEPARTMENT_WISE_TEMPLATE>();
            var programme = new CP_PROGRAMME_2017();
            var programmes = new List<CP_PROGRAMME_2017>();
            var academicCurriculum = new ACADEMIC_CURRICULUM_2017();
            var academicCurriculums = new List<ACADEMIC_CURRICULUM_2017>();
            var academicBatch = new AcademicBatches();
            var academicBatches = new List<AcademicBatches>();
            var timeTableTemplate = new TimeTableTemplate();
            var timeTableTemplates = new List<TimeTableTemplate>();
            var objClass = new CP_CLASSES();
            var Classes = new List<CP_CLASSES>();

            var paperTypes = (List<SupPaperType>)CMSHandler.FetchData<SupPaperType>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchPaparType)).DataSource.Data;
            academicBatches = (List<AcademicBatches>)CMSHandler.FetchData<AcademicBatches>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchAcademicBatches)).DataSource.Data;

            objSetting.ACADEMIC_YEAR = sAcademicYear;

            settings = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objSetting, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableSettings)).DataSource.Data;
            if (settings != null && settings.Count() > 0)
            {
                departmentWiseTemplate.Academic_Year = sAcademicYear;

                foreach (var setting in settings)
                {
                    departmentWiseTemplate.SETTING_ID = setting.SETTING_ID;
                    departmentWiseTemplates = (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(departmentWiseTemplate, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDepartmentBySettingId)).DataSource.Data;
                    if (departmentWiseTemplates != null && departmentWiseTemplates.Count() > 0)
                    {
                        foreach (var template in departmentWiseTemplates)
                        {
                            programme.DEPARTMENT = template.DEPARTMENT_ID;
                            programme.PROGRAMME_LEVEL = setting.GRADUATION_TYPE;
                            programmes = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(programme, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchProgrammesByDepartmentAndGraduationType), sAcademicYear).DataSource.Data;
                            foreach (var prog in programmes)
                            {
                                foreach (var batch in academicBatches)
                                {
                                    academicCurriculum.BATCH = batch.BATCH_ID;
                                    academicCurriculum.PROGRAMME = prog.PROGRAMME_ID;
                                    academicCurriculums = (List<ACADEMIC_CURRICULUM_2017>)CMSHandler.FetchData<ACADEMIC_CURRICULUM_2017>(academicCurriculum, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchAcademicCurrilum), sAcademicYear).DataSource.Data;
                                    timeTableTemplate.Setting_Id = setting.SETTING_ID;
                                    timeTableTemplate.Year = batch.BATCH_YEAR;
                                    timeTableTemplates = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(timeTableTemplate, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTemplateOnlyStaticHours), sAcademicYear).DataSource.Data;

                                    objClass.PROGRAMME = programme.PROGRAMME_ID;
                                    Classes = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objClass, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClassesByProgrammeId), sAcademicYear).DataSource.Data;

                                    if (Classes != null && Classes.Count() > 0)
                                    {
                                        if (academicCurriculums != null && academicCurriculums.Count() > 0)
                                        {
                                            if (timeTableTemplates != null && timeTableTemplates.Count() > 0)
                                            {
                                                foreach (var cls in Classes)
                                                {
                                                    foreach (var paperType in paperTypes)
                                                    {
                                                        var temp = (List<TimeTableTemplate>)timeTableTemplates.Where(o => o.Paper_Type_Id.Equals(paperType.PAPER_TYPE_ID));
                                                        if (temp != null && temp.Count() > 0)
                                                        {

                                                            var objTimeTable = new TimeTable();

                                                            var objCourses = academicCurriculums.Where(o => o.PAPER_TYPE == paperType.PAPER_TYPE_ID);
                                                            foreach (var tt in temp)
                                                            {
                                                                if (objCourses != null && objCourses.Count() > 0)
                                                                {
                                                                    foreach (var course in objCourses)
                                                                    {
                                                                        objTimeTable.Class_Id = cls.CLASS_ID;
                                                                        objTimeTable.Course_Id = course.COURSE_ID;
                                                                        objTimeTable.Day_Order_Id = tt.Day_Order_Id;
                                                                        objTimeTable.Hour_Id = tt.Hour_Id;
                                                                        objTimeTable.Is_Active = tt.Is_Active;
                                                                        objTimeTable.Semester_Id = course.SEMESTER;
                                                                        CMSHandler.SaveOrUpdate(objTimeTable, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }


                                }
                            }

                        }
                    }
                }

            }



            //     var staticTamplate = CMSHandler.FetchData<TimeTableTemplate>(timeTableTemplate, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTemplateOnlyStaticHours)).DataSource.Data;

        }

        #endregion

        #region Time Table Crude UpdateClassWiseTimeTable
        public ActionResult TimeTableClassWiseList()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableClassWiseList", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableClassWiseList", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult CombinedTimeTableEntry()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("CombinedTimeTableEntry", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("CombinedTimeTableEntry", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public JsonResult UpdateCombinedTimeTable(string sCourseId, string sClassId, string sTimeTableId, string sStaffId, string sDayOrderId, string sHourId, string sHourType, string sSemesterId, string sForceSave)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            TimeTable objModel = new TimeTable();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                    
                    if (!string.IsNullOrEmpty(sStaffId))
                    {
                        objModel.Course_Id = sCourseId;
                        objModel.COURSE_ROOT_ID = sCourseId;
                        objModel.Class_Id = sClassId;
                        objModel.Staff_Id = sStaffId;
                        objModel.Time_Table_Id = sTimeTableId;
                        objModel.Day_Order_Id = sDayOrderId;
                        objModel.Hour_Id = sHourId;
                        objModel.hourType = sHourType;
                        objModel.Semester_Id = sSemesterId;
                        objModel.ACADEMIC_YEAR = sAcademicYear;

                        var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                        if (liCourseRoot != null && liCourseRoot.Count > 0)
                        {
                            objModel.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                            
                            bool checkOccupation = (sForceSave != "true");
                            bool isOccupied = false;
                            string occupiedMessage = "";

                            if (checkOccupation)
                            {
                                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsStaffExistForTimeTable)
                                    .Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                                var IsStaffExist = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                                if (IsStaffExist != null && IsStaffExist.Count > 0)
                                {
                                    isOccupied = true;
                                    occupiedMessage = "This Staff occupied this hour ! \n For Class = " + IsStaffExist.FirstOrDefault().CLASS_CODE + " \n  Course = " + IsStaffExist.FirstOrDefault().COURSE_CODE + " !\nDo you want to allot anyway?";
                                }
                            }

                            if (isOccupied)
                            {
                                ObjJsonData.ErrorCode = "OCCUPIED";
                                ObjJsonData.Message = occupiedMessage;
                            }
                            else
                            {
                                ResultArgs ResultArgs;
                                if (string.IsNullOrEmpty(sTimeTableId))
                                {
                                    ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                                }
                                else
                                {
                                    ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimeTableInfo), sAcademicYear);
                                }
                                ObjJsonData.ErrorCode = ResultArgs.Success ? "200" : "500";
                                ObjJsonData.Message = ResultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                        else
                        {
                            ObjJsonData.ErrorCode = "500";
                            ObjJsonData.Message = "Course details not found.";
                        }
                    }
                    else
                    {
                        ObjJsonData.ErrorCode = "500";
                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimeTableController", "UpdateCombinedTimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimeTableController", "UpdateCombinedTimeTable", ex.Message);
                    }
                    ObjJsonData.ErrorCode = "500";
                    ObjJsonData.Message = "Internal Server Error: " + ex.Message;
                }
            }
            else
            {
                ObjJsonData.ErrorCode = "401";
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindTimeTableInfo(string sClassId)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                        ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                    TimeTable objModel = new TimeTable();
                    var BindStudentCourseInfo = new List<TimeTable>();
                    objModel.Class_Id = sClassId;
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

                    string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableClassWiseListByClassId);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Semester_Id.ToUpper(), sActiveSemester);
                    BindStudentCourseInfo = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                    objViewModel.lstTimeTable = BindStudentCourseInfo;

                    string ssql = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchValueEducationPapersForTimeTable);
                    var liFetchEthics = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, ssql, sAcademicYear).DataSource.Data;
                    if (liFetchEthics != null && liFetchEthics.Count > 0)
                    {
                        objViewModel.lstTimeTable = (objViewModel.lstTimeTable != null) ? objViewModel.lstTimeTable.Concat(liFetchEthics).ToList() : liFetchEthics;
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindTimeTableInfo", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindTimeTableInfo", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult EditClassWiseTimeTable(string sTimetable)
        {
            string sOption = string.Empty;
            JsonResultData objResult = new JsonResultData();
            TimeTable objModel = new TimeTable();
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                string sHourType = string.Empty;
                string sCourse = string.Empty;
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var Result = JsonConvert.DeserializeObject<TimeTable>(sTimetable);
                    if (!string.IsNullOrEmpty(Result.Count) && Result.Count != "undefined" && Result.Count != "0")
                    {
                        // If More Then One Record Available ....
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMultipleTimeTableByTimeTableId).
                            Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Time_Table_Id.ToUpper(), Result.Time_Table_Id);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        var liTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, sSQL, sAcademicYear).DataSource.Data;
                        if (liTimeTable != null && liTimeTable.Count > 0)
                        {
                            objViewModel.lstTimeTable = liTimeTable;
                            return View(objViewModel);
                        }
                    }
                    else
                    {
                        // Only One Record Available ...


                        var FetchTimeTableInfo = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableInfoForEdit), sAcademicYear).DataSource.Data;
                        if (FetchTimeTableInfo != null && FetchTimeTableInfo.Count > 0)
                        {
                            objModel.IS_STATIC = FetchTimeTableInfo.FirstOrDefault().IS_STATIC;
                            objModel.PAPER_TYPE = FetchTimeTableInfo.FirstOrDefault().Paper_Type_Id;
                        }

                        // HourType ....
                        var HourType = (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null,
                               SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourType)).DataSource.Data;
                        if (HourType != null && HourType.Count > 0)
                        {
                            foreach (var DataItem in HourType)
                                sHourType += "<option value='" + DataItem.hour_type_id + "'>" + DataItem.hourType + "</option>";
                        }

                        if (objModel.IS_STATIC == "0" || string.IsNullOrEmpty(objModel.IS_STATIC))
                        {
                            // Course  By Class Id.....
                            var liCourse = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new CP_CLASSES() { CLASS_ID = Result.Class_Id },
                                   TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCourseByClassAndAcademicYear), sAcademicYear).DataSource.Data;
                            if (liCourse != null && liCourse.Count > 0)
                            {
                                foreach (var DataItem in liCourse)
                                    sCourse += "<option value=" + DataItem.COURSE_ROOT_ID + " >" + DataItem.COURSE_TITLE + "</option>";
                            }
                        }
                        else
                        {
                            // Course By Paper Type .....
                            var liCourse = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(objModel,
                                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByPaperType), sAcademicYear).DataSource.Data;
                            if (liCourse != null && liCourse.Count > 0)
                            {
                                foreach (var DataItem in liCourse)
                                    sCourse += "<option value=" + DataItem.COURSE_ROOT_ID + " >" + DataItem.COURSE_TITLE + "</option>";
                            }
                        }



                        // Staff List .....
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffByClassId);
                        var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchStaff != null && FetchStaff.Count > 0)
                        {
                            foreach (var item in FetchStaff)
                                sOption += "<option value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
                        }

                        objModel.hourType = sHourType;
                        objModel.sCourseId = sCourse;
                        objModel.sStaffId = sOption;
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
                    objHandler.WriteError("EditClassWiseTimeTable", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditClassWiseTimeTable", "TimeTableController", ex.Message);
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public string GetCourseAndStaff(string sClassId)
        {
            string sCourse = string.Empty;
            string sHourType = string.Empty;
            string sSemester = string.Empty;
            string sStaff = string.Empty;
            string sPaperType = string.Empty;
            string sCourseList = string.Empty;
            string sTimeTableCourse = string.Empty;
            string JsonData = string.Empty;
            TimeTable objModel = new TimeTable();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    objModel.Class_Id = sClassId;
                    if (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                    {
                        objModel.Staff_Id = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                        string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                        var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                        string sSQL = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseList);
                        var FetchCourse = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(objModel, sSQL, sAcademicYear).DataSource.Data;
                        var HourType = (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourType)).DataSource.Data;
                        var PaperType = (List<SupPaperType>)CMSHandler.FetchData<SupPaperType>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSupPaperType)).DataSource.Data;
                        //var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;

                        if (FetchStaff != null && FetchStaff.Count > 0)
                        {
                            foreach (var item in FetchStaff)
                                sStaff += "<option value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
                        }
                        if (FetchCourse != null && FetchCourse.Count > 0)
                        {
                            foreach (var list in FetchCourse)
                            {
                                sCourse += "<option value='" + list.COURSE_ID + "'>" + list.COURSE_TITLE + "</option>";
                                sSemester = list.SEMESTER_ID;
                            }
                        }
                        if (HourType != null && HourType.Count > 0)
                        {
                            foreach (var DataItem in HourType)
                                sHourType += "<option value='" + DataItem.hour_type_id + "'>" + DataItem.hourType + "</option>";
                        }
                        if (PaperType != null && PaperType.Count > 0)
                        {
                            foreach (var DataItem in PaperType)
                                sPaperType += "<option value='" + DataItem.PAPER_TYPE_ID + "'>" + DataItem.PAPER_TYPE + "</option>";
                        }

                        // Fetch Course List ....
                        var liFetchCourseList = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(objModel,
                            ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCourseByClassId), sAcademicYear).DataSource.Data;
                        if (liFetchCourseList != null && liFetchCourseList.Count > 0)
                        {
                            foreach (var item in liFetchCourseList)
                            {
                                sCourseList += "<option value='" + item.COURSE_ROOT_ID + "'>" + item.COURSE_TITLE + "</option>";
                                sSemester = item.SEMESTER_ID;
                            }
                        }

                        var liFetchValueEducation = (List<CourseWiseStaffInfo>)CMSHandler.FetchData<CourseWiseStaffInfo>(objModel,
                                TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchValueEducationPaperByTTClassIdForDDL), sAcademicYear).DataSource.Data;
                        if (liFetchValueEducation != null && liFetchValueEducation.Count > 0)
                        {
                            foreach (var item in liFetchValueEducation)
                            {
                                sCourseList += "<option value='" + item.COURSE_ID + "'>" + item.COURSE_TITLE + "</option>";
                            }
                        }

                        // Check Is Nme Class ...
                        //var IsNMEClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel,SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.CheckIsNMEClass)).DataSource.Data;
                        //if (IsNMEClass!=null && IsNMEClass.Count>0)
                        //{
                        //    sTimeTableCourse = string.Empty;
                        //    foreach (var DataItem in IsNMEClass)
                        //        sTimeTableCourse += "<option value='" + DataItem.COURSE_ID + "'>" + DataItem.COURSE + "</option>";
                        //}
                        //else
                        //{
                        //    sTimeTableCourse = string.Empty;
                        //    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel,SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassId),sAcademicYear).DataSource.Data;
                        //    if (liCourseList != null && liCourseList.Count>0)
                        //    {
                        //        foreach (var DataItem in liCourseList)
                        //            sTimeTableCourse += "<option value='" + DataItem.COURSE_ROOT_ID + "'>" + DataItem.COURSE_TITLE + "</option>";
                        //    }
                        //    var liValueEducation = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel,SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchValueEducation),sAcademicYear).DataSource.Data;
                        //    if (liValueEducation!=null && liValueEducation.Count>0)
                        //    {
                        //        foreach (var DataItem in liValueEducation)
                        //        {
                        //            var IsExist = liCourseList.Where(x => x.COURSE_ROOT_ID == DataItem.COURSE_ROOT_ID).ToList();
                        //            if (IsExist!=null && IsExist.Count>0)
                        //            {
                        //                // Already Data Binded  In Before Execution ...
                        //            }
                        //            else
                        //            {
                        //                sTimeTableCourse += "<option value='" + DataItem.COURSE_ROOT_ID + "'>" + DataItem.COURSE_TITLE + "</option>";
                        //            }

                        //        }

                        //    }
                        //}
                        var objJsonData = new TimeTable() { COURSE_CODE = sCourseList, Course_Id = sCourse, Staff_Id = sStaff, hourType = sHourType, Semester_Id = sSemester, PAPER_TYPE = sPaperType, COURSE_ROOT_ID = sTimeTableCourse };
                        JsonData = JsonConvert.SerializeObject(objJsonData);
                    }
                }
                else
                {
                    JsonData = Common.ErrorCode.RequestTimeout + '\n';
                    JsonData += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("GetCourseAndStaff", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetCourseAndStaff", "TimeTableController", ex.Message);
                }
            }
            return JsonData;
        }

        // Fetch 2 Way TimeTable ...
        public JsonResult FetchTimeTableById(string sTimeTableId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    TimeTable ObjModal = new TimeTable();
                    var liTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(new TimeTable() { Time_Table_Id = sTimeTableId }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableById), sAcademicYear).DataSource.Data;
                    if (liTimeTable != null && liTimeTable.Count > 0)
                    {
                        return Json(liTimeTable, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchTimeTableById", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchTimeTableById", "TimeTableController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Fetch & Load DropDown For MultiPle ...
        public JsonResult FetchAllDropDownForTimeTable(string sClassId)
        {
            string sCourseList = string.Empty;
            string sTimeTableCourse = string.Empty;
            string sHour = string.Empty;
            string sHourType = string.Empty;
            string sDayOrderId = string.Empty;
            string sStaffList = string.Empty;
            TimeTable objModel = new TimeTable();
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                objModel.Class_Id = sClassId;

                // Hour Type ...
                var HourType = (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourType)).DataSource.Data;
                if (HourType != null && HourType.Count > 0)
                {
                    foreach (var DataItem in HourType)
                        sHourType += "<option value='" + DataItem.hour_type_id + "'>" + DataItem.hourType + "</option>";
                }

                // Hour List ...
                var Hour = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data;
                if (Hour != null && Hour.Count > 0)
                {
                    foreach (var DataItem in Hour)
                        sHour += "<option value='" + DataItem.HOUR_ID + "'>" + DataItem.HOUR + "</option>";
                }

                // DayOrder List ...
                var DayOrder = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrders)).DataSource.Data;
                if (DayOrder != null && DayOrder.Count > 0)
                {
                    foreach (var DataItem in DayOrder)
                        sDayOrderId += "<option value='" + DataItem.DAY_ORDER_ID + "'>" + DataItem.DAY + "</option>";
                }

                // Staff List ...
                //var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                //if (FetchStaff!=null && FetchStaff.Count>0)
                //{
                //    foreach (var DataItem in FetchStaff)
                //        sStaffList += "<option value='" + DataItem.STAFF_ID + "'>" + DataItem.FIRST_NAME + "</option>";
                //}

                // Check Is Nme Class ...
                var IsNMEClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.CheckIsNMEClass)).DataSource.Data;
                if (IsNMEClass != null && IsNMEClass.Count > 0)
                {
                    sTimeTableCourse = string.Empty;
                    foreach (var DataItem in IsNMEClass)
                        sTimeTableCourse += "<option value='" + DataItem.COURSE_ID + "'>" + DataItem.COURSE + "</option>";
                }
                else
                {
                    // Normal Course List ...
                    sTimeTableCourse = string.Empty;
                    var liCourseList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCourseByClassAndAcademicYear), sAcademicYear).DataSource.Data;
                    if (liCourseList != null && liCourseList.Count > 0)
                    {
                        foreach (var DataItem in liCourseList)
                            sTimeTableCourse += "<option value='" + DataItem.COURSE_ROOT_ID + "'>" + DataItem.COURSE_TITLE + "</option>";
                    }
                    var liValueEducation = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchValueEducation), sAcademicYear).DataSource.Data;
                    if (liValueEducation != null && liValueEducation.Count > 0)
                    {
                        foreach (var DataItem in liValueEducation)
                        {
                            var IsExist = liCourseList.Where(x => x.COURSE_ROOT_ID == DataItem.COURSE_ROOT_ID).ToList();
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                // Already Binded Data In Before Execution ...
                            }
                            else
                            {
                                sTimeTableCourse += "<option value='" + DataItem.COURSE_ROOT_ID + "'>" + DataItem.COURSE_TITLE + "</option>";
                            }
                        }
                    }
                }
                string JsonData = string.Empty;
                var objJson = new TimeTable() { hourType = sHourType, Day_Order_Id = sDayOrderId, Hour_Id = sHour, COURSE_ROOT_ID = sTimeTableCourse, Staff_Id = sStaffList };
                JsonData = JsonConvert.SerializeObject(objJson);
                return Json(JsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Save TimeTable 2 Way ...
        public JsonResult SaveTimeTableMultiple(string sTimeTable)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<TimeTable>(sTimeTable);
                    Result.COURSE_ROOT_ID = Result.Course_Id;
                    var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(Result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                    if (liCourseRoot != null && liCourseRoot.Count > 0)
                    {
                        Result.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAlloted);
                        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        {
                            ObjJsonData.Message = "This Staff Already Alloted To " + FetchAllotedStaff.FirstOrDefault().CLASS_CODE;
                        }
                        else
                        {
                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                            ObjJsonData.Message = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveTimeTableMultiple", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("SaveTimeTableMultiple", "TimeTableController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTemplateByClassId(string sClassId)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    var HourType = (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourType)).DataSource.Data;
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
                    objHandler.WriteError("GetTemplateByClassId", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("GetTemplateByClassId", "TimeTableController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }

        public string GetStaffByCourseId(string sCourseId, string sClassId)
        {
            string sOption = string.Empty;
            string JsonData = string.Empty;
            TimeTable objModel = new TimeTable();
            objModel.Course_Id = sCourseId;
            objModel.Class_Id = sClassId;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
            //var CheckIsOptionalPaper = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.CheckIsOptionalPaper), sAcademicYear).DataSource.Data;
            //if (CheckIsOptionalPaper != null && CheckIsOptionalPaper.Count > 0)
            //{
            //    string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffByCourseId).
            //        Replace(Common.Delimiter.QUS+Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
            //    var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(objModel, sSQL, sAcademicYear).DataSource.Data;
            //    if (FetchStaff != null && FetchStaff.Count > 0)
            //    {
            //        foreach (var item in FetchStaff)
            //            sOption += "<option value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
            //    }
            //}
            //else 
            //{

            //}

            string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffByCourseIdAndClassId).
                        Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
            var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(objModel, sSQL, sAcademicYear).DataSource.Data;
            if (FetchStaff != null && FetchStaff.Count > 0)
            {
                foreach (var item in FetchStaff)
                    sOption += "<option value=" + item.STAFF_ID + ">" + item.FIRST_NAME + "</option>";
            }
            //var objJsonData = new TimeTable() { Staff_Id = sOption };
            //JsonData = JsonConvert.SerializeObject(objJsonData);
            return sOption;
        }
        public string GetCourseByClassId(string sClassId, string sDayOrderId)
        {
            string sOption = string.Empty;
            string sSemester = string.Empty;
            string sHourId = string.Empty;
            string JsonData = string.Empty;
            TimeTable objModel = new TimeTable();
            objModel.Class_Id = sClassId;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            // var FetchActiveSemester = (List<SUP_SEMESTER>)CMSHandler.FetchData<SUP_SEMESTER>(null,SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
            string ActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            objModel.Semester_Id = ActiveSemester;
            objModel.Day_Order_Id = sDayOrderId;
            string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCourseByClassId).Replace(Common.Delimiter.QUS + Common.CP_COURSE_STRUCTURE.SEMESTER_ID, ActiveSemester);
            var FetchCourse = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(objModel, sSQL, sAcademicYear).DataSource.Data;
            if (FetchCourse != null && FetchCourse.Count > 0)
            {
                foreach (var list in FetchCourse)
                    sOption += "<option value='" + list.COURSE_ROOT_ID + "'>" + list.COURSE_TITLE + "</option>";
            }

            // Fetch Semester By Class Id ....
            var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(objModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterByClassId), sAcademicYear).DataSource.Data;
            if (Semester != null && Semester.Count > 0)
            {
                foreach (var item in Semester)
                {
                    sSemester += "<option value=" + item.SUP_SEMESTER_ID + "> " + item.SEMESTER_NAME + " </option>";
                }
            }

            // Hour By Class Id ...
            var Hour = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursByTimeTableHours), sAcademicYear).DataSource.Data;
            if (Hour != null && Hour.Count > 0)
            {
                var HourByClassId = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourByClassIdAndDayOrderId), sAcademicYear).DataSource.Data;
                if (HourByClassId != null && HourByClassId.Count > 0)
                {
                    foreach (var item in Hour)
                    {
                        var Temp = HourByClassId.Where(o => o.HOUR_ID == item.HOUR_ID).ToList();
                        if (Temp.Count == 0)
                        {
                            sHourId += "<option value=" + item.HOUR_ID + ">" + item.HOUR + "</option>";
                        }
                    }
                }
                else
                {
                    foreach (var item in Hour)
                    {
                        sHourId += "<option value=" + item.HOUR_ID + ">" + item.HOUR + "</option>";
                    }
                }
            }
            if (string.IsNullOrEmpty(sHourId))
            {
                sHourId = "<option value='0' disabled>All Hours Are Alloted </option>";
            }

            var objJsonData = new TimeTable() { Course_Id = sOption, Semester_Id = sSemester, Hour_Id = sHourId };
            JsonData = JsonConvert.SerializeObject(objJsonData);
            return JsonData;
        }
        public string UpdateClassWiseTimeTable(string sCourseId, string sClassId, string sTimeTableId, string sStaffId, string sDayOrderId, string sHourId, string sHourType, string sSemesterId)
        {
            string sOption = string.Empty;
            TimeTable objModel = new TimeTable();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                    if (!string.IsNullOrEmpty(sStaffId))
                    {
                        objModel.Course_Id = sCourseId;
                        objModel.COURSE_ROOT_ID = sCourseId;
                        objModel.Class_Id = sClassId;
                        objModel.Staff_Id = sStaffId;
                        objModel.Time_Table_Id = sTimeTableId;
                        objModel.Day_Order_Id = sDayOrderId;
                        objModel.Hour_Id = sHourId;
                        objModel.hourType = sHourType;
                        objModel.Semester_Id = sSemesterId;
                        objModel.ACADEMIC_YEAR = sAcademicYear;
                        var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                        if (liCourseRoot != null && liCourseRoot.Count > 0)
                        {
                            string sPaperType = liCourseRoot.FirstOrDefault().PAPER_TYPE;
                            objModel.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                            //if (!string.IsNullOrEmpty(sPaperType) && sPaperType != "3")
                            //{
                            //    ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL((string.IsNullOrEmpty(sTimeTableId)? TimeTableSQLCommands.SaveTimeTable : TimeTableSQLCommands.UpdateTimeTableInfo)), sAcademicYear);
                            //    sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            //}
                            //else
                            //{

                            //}
                            if (string.IsNullOrEmpty(sTimeTableId))
                            {
                                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsStaffExistForTimeTable).
                                       Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                                var IsStaffExist = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                                if (IsStaffExist == null)
                                {
                                    ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                                    sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    sOption = "This Staff occupied this hour ! \n For Class = " + IsStaffExist.FirstOrDefault().CLASS_CODE + " \n  Course = " + IsStaffExist.FirstOrDefault().COURSE_CODE + " !";
                                }
                            }
                            else
                            {
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimeTableInfo), sAcademicYear);
                                sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                        // var FetchIsStaticHourAndPaperType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchIsStaticAndPaperTypeByClassId), sAcademicYear).DataSource.Data;
                    }
                    else
                    {
                        sOption = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimeTableController", "UpdateClassWiseTimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimeTableController", "UpdateClassWiseTimeTable", ex.Message);
                    }
                }
            }
            else
            {
                sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return sOption;
        }
        public async Task<ActionResult> ClassWiseTimeTableInfo()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            var Class = new List<cp_Classes_2017>();
            var Course = new List<cp_Course_Root_2017>();
            var Staff = new List<stf_Personal_Info>();
            var HourType = new List<SUP_HOUR_TYPE>();
            var Hour = new List<SUP_HOURS>();
            var Semester = new List<SupSemester>();
            var DayOrder = new List<SUP_DAY_ORDER>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            Class = await Task.Run(() => (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data);
            if (Class != null && Class.Count > 0)
            {
                objViewModel.ClassList = new SelectList(Class, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
            }
            Course = await Task.Run(() => (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOnlyRegularCoursesList), sAcademicYear).DataSource.Data);
            if (Course != null && Course.Count > 0)
            {
                objViewModel.CourseList = new SelectList(Course, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_TITLE);
            }

            Staff = await Task.Run(() => (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data);
            if (Staff != null && Staff.Count > 0)
            {
                objViewModel.StaffList = new SelectList(Staff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
            }

            HourType = await Task.Run(() => (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourType)).DataSource.Data);
            if (HourType != null && HourType.Count > 0)
            {
                objViewModel.HourType = new SelectList(HourType, Common.SUP_HOUR_TYPE.HOUR_TYPE_ID, Common.SUP_HOUR_TYPE.HOURTYPE);
            }
            //Hour = await Task.Run(() => (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data);
            Hour = await Task.Run(() => (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursByTimeTableHours), sAcademicYear).DataSource.Data);
            if (Hour != null && Hour.Count > 0)
            {
                objViewModel.HourList = new SelectList(Hour, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
            }

            Semester = await Task.Run(() => (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data);
            if (Semester != null && Semester.Count > 0)
            {
                objViewModel.SemesterList = new SelectList(Semester, Common.SUP_SEMESTER.SUP_SEMESTER_ID, Common.SUP_SEMESTER.SEMESTER_NAME);
            }
            DayOrder = await Task.Run(() => (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrders)).DataSource.Data);
            if (DayOrder != null && DayOrder.Count > 0)
            {
                objViewModel.DayOrderList = new SelectList(DayOrder, Common.SUP_DAY_ORDERS.DAY_ORDER_ID, Common.SUP_DAY_ORDERS.DAY);
            }

            return View(objViewModel);
        }

        // Fetch Semester By Class Id ...
        public JsonResult FetchSemsterByClassId(string sClassId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            if (!string.IsNullOrEmpty(sClassId))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        var Semester = (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data;
                        if (Semester != null && Semester.Count > 0)
                        {
                            foreach (var item in Semester)
                            {
                                ObjJsonData.sResult += "<option value=" + item.SUP_SEMESTER_ID + "> " + item.SEMESTER_NAME + " </option>";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("FetchSemsterByClassId", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("FetchSemsterByClassId", "TimeTableController", ex.Message);
                        }
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
        public string SaveClassWiseTimeTable(string TimeTableInfo)
        {
            string sOption = string.Empty;
            if (TimeTableInfo != null)
            {
                var Result = JsonConvert.DeserializeObject<TimeTable>(TimeTableInfo);
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(Result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                if (liCourseRoot != null && liCourseRoot.Count > 0)
                {
                    string sPaperType = liCourseRoot.FirstOrDefault().PAPER_TYPE;
                    Result.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                    // Allow staff Dupllicate Entry For Optional Course ... 
                    if (!string.IsNullOrEmpty(sPaperType) && sPaperType != "3")
                    {
                        ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAlloted).
                                     Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        {
                            sOption = "This Staff Already Alloted To " + FetchAllotedStaff.FirstOrDefault().CLASS_CODE;
                        }
                        else
                        {
                            Result.Class_Id = Result.CLASS_CODE;
                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                            if (ResultArgs.Success)
                            {
                                sOption = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                sOption = Common.Messages.FailedToSaveRecords;
                            }
                        }
                        //var FetchSelectedClassId = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSelectedClassIdByCourseIdAndClassId), sAcademicYear).DataSource.Data;
                        //if (FetchSelectedClassId != null && FetchSelectedClassId.Count > 0)
                        //{
                        //    Result.CLASS_CODE = Result.Class_Id;
                        //    Result.Class_Id = FetchSelectedClassId.FirstOrDefault().S_CLASS_ID;
                        //    var FetchExistingTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableInfoExistingForOptionalPaper), sAcademicYear).DataSource.Data;
                        //    if (FetchExistingTimeTable != null && FetchExistingTimeTable.Count > 0)
                        //    {
                        //        sOption = "This Hour Is Already Alloted...!!!";
                        //    }
                        //    else
                        //    {
                        //        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAlloted).
                        //             Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        //        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        //        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        //        {
                        //            sOption = "This Staff Already Alloted To " + FetchAllotedStaff.FirstOrDefault().CLASS_CODE;
                        //        }
                        //        else
                        //        {
                        //            Result.Class_Id = Result.CLASS_CODE;
                        //            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //            if (ResultArgs.Success)
                        //            {
                        //                sOption = Common.Messages.RecordsSavedSuccessfully;
                        //            }
                        //            else
                        //            {
                        //                sOption = Common.Messages.FailedToSaveRecords;
                        //            }
                        //        }
                        //    }

                        //}
                        //else
                        //{
                        //    var FetchExistingTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableInfoExisting), sAcademicYear).DataSource.Data;
                        //    if (FetchExistingTimeTable != null && FetchExistingTimeTable.Count > 0)
                        //    {
                        //        sOption = "This Hour Is Already Alloted...!!!";
                        //    }
                        //    else
                        //    {
                        //        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAlloted), sAcademicYear).DataSource.Data;
                        //        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        //        {
                        //            sOption = "This Staff Already Alloted To Another Class";
                        //        }
                        //        else
                        //        {
                        //            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //            if (ResultArgs.Success)
                        //            {
                        //                sOption = Common.Messages.RecordsSavedSuccessfully;
                        //            }
                        //            else
                        //            {
                        //                sOption = Common.Messages.FailedToSaveRecords;
                        //            }

                        //        }
                        //    }
                        //}
                    }
                }

            }
            return sOption;
        }
        #endregion

        #region Time Table Setting
        public async Task<ActionResult> TimeTableSettingList()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var TimeTableSetting = new TimeTableSetting();
                    var ListTimeTableSettings = await Task.Run(() => (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchAllTimeTableSettings), sAcademicYear).DataSource.Data);
                    objViewModel.lstTimeTableSetting = ListTimeTableSettings;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableSettingList", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableSettingList", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public async Task<ActionResult> TimeTableSetting()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            var Semester = new List<SupSemester>();
            var Hours = new List<SUP_HOURS>();
            var Days = new List<SUP_DAY_ORDER>();
            var GraduationType = new List<SupClassLevel>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    Semester = await Task.Run(() => (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data);
                    Days = await Task.Run(() => (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrders)).DataSource.Data);
                    Hours = await Task.Run(() => (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data);
                    GraduationType = await Task.Run(() => (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data);
                    var SemesterType = await Task.Run(() => (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data);
                    objViewModel.SemesterList = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    objViewModel.HourList = new SelectList(Hours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    objViewModel.DayOrderList = new SelectList(Days, Common.SUP_DAY_ORDERS.DAY_ORDER_ID, Common.SUP_DAY_ORDERS.DAY);
                    objViewModel.GraduationType = new SelectList(GraduationType, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableSetting", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableSetting", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public string SaveTimeTableSetting(string TimeTableSetting)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    if (TimeTableSetting != null)
                    {
                        var Result = JsonConvert.DeserializeObject<TimeTableSetting>(TimeTableSetting);
                        var FetchExistingSetting = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableSettingExisting), sAcademicYear).DataSource.Data;
                        if (FetchExistingSetting != null)
                            sOption = "This Setting Is Existing Already...!";
                        else
                        {
                            ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTableSettings), sAcademicYear);
                            if (ResultArg.Success)
                                sOption = Common.Messages.RecordsSavedSuccessfully;
                            else
                                sOption = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout + '\n';
                    sOption += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveTimeTableSetting", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveTimeTableSetting", "TimeTableController", ex.Message);
                }
            }
            return sOption;
        }
        public async Task<ActionResult> EditTimeTableSetting(string id)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            var Semester = new List<SupSemester>();
            var Hours = new List<SUP_HOURS>();
            var Days = new List<SUP_DAY_ORDER>();
            var GraduationType = new List<SupClassLevel>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    Session[Common.SESSION_VARIABLES.SETTING_ID] = id;
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    Semester = await Task.Run(() => (List<SupSemester>)CMSHandler.FetchData<SupSemester>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemester)).DataSource.Data);
                    Days = await Task.Run(() => (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrders)).DataSource.Data);
                    Hours = await Task.Run(() => (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data);
                    GraduationType = await Task.Run(() => (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data);
                    var SemesterType = await Task.Run(() => (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType)).DataSource.Data);
                    objViewModel.SemesterList = new SelectList(SemesterType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);
                    objViewModel.HourList = new SelectList(Hours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    objViewModel.DayOrderList = new SelectList(Days, Common.SUP_DAY_ORDERS.DAY_ORDER_ID, Common.SUP_DAY_ORDERS.DAY);
                    objViewModel.GraduationType = new SelectList(GraduationType, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditTimeTableSetting", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditTimeTableSetting", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchTimeTableSettingById(string sSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            TimeTableSetting objModel = new TimeTableSetting();
            objModel.SETTING_ID = sSettingId;
            var FetchTimeTableSetting = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableSettingById), sAcademicYear).DataSource.Data;
            return Json(FetchTimeTableSetting);
        }
        public string UpdateTimeTableSetting(string TimeTableSetting)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    if (TimeTableSetting != null)
                    {
                        var Result = JsonConvert.DeserializeObject<TimeTableSetting>(TimeTableSetting);
                        Result.SETTING_ID = (Session[Common.SESSION_VARIABLES.SETTING_ID] != null) ? Session[Common.SESSION_VARIABLES.SETTING_ID].ToString() : "";
                        var FetchExistingSetting = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableSettingExisting), sAcademicYear).DataSource.Data;
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimeTableSettings), sAcademicYear);
                        if (ResultArg.Success)
                            sOption = Common.Messages.RecordsSavedSuccessfully;
                        else
                            sOption = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout + '\n';
                    sOption += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateTimeTableSetting", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateTimeTableSetting", "TimeTableController", ex.Message);
                }
            }
            return sOption;
        }
        public string DeleteTimeTableSetting(string sSettingId)
        {
            string sOption = string.Empty;
            TimeTableSetting objModel = new TimeTableSetting();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    objModel.SETTING_ID = sSettingId;
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteTimeTableSettings), sAcademicYear);
                    if (ResultArg.Success)
                        sOption = Common.Messages.RecordDeletedSuccessfully;
                    else
                        sOption = Common.Messages.FailedToDeletedTryAgain;
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout + '\n';
                    sOption += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteTimeTableSetting", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteTimeTableSetting", "TimeTableController", ex.Message);
                }
            }
            return sOption;
        }
        #endregion

        #region  Department Wise TimeTable Template
        public async Task<ActionResult> TimeTableDepartmentWiseTemplateList()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    //var TimeTableSetting = new TT_DEPARTMENT_WISE_TEMPLATE();
                    var ListDeparmentWiseTemplate = await Task.Run(() => (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDepartmentWiseTimetableTemplate), sAcademicYear).DataSource.Data);
                    objViewModel.lstTimeTableDepartmentWiseTemplate = ListDeparmentWiseTemplate;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableDepartmentWiseTemplateList", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableDepartmentWiseTemplateList", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchDepartmentByTimeTableSettingId(string sSettingId)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    if (!string.IsNullOrEmpty(sSettingId))
                    {
                        string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                        var FetchDepartment = (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(new TT_DEPARTMENT_WISE_TEMPLATE() { SETTING_ID = sSettingId }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDepartmentByTimeTableSettingId), sAcademicYear).DataSource.Data;
                        if (FetchDepartment != null && FetchDepartment.Count > 0)
                        {
                            foreach (var item in FetchDepartment)
                                objResult.sResult += "<option value='" + item.DEPARTMENT_ID + "' " + item.STATUS + ">" + item.DEPARTMENT + "</option>";
                        }
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
                    objHandler.WriteError("FetchDepartmentByTimeTableSettingId", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("FetchDepartmentByTimeTableSettingId", "TimeTableController", ex.Message);
                }

            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public string DeleteDepartmentWiseTimeTableTemplate(string sDepatmentWiseTemplateId)
        {
            string sOption = string.Empty;
            TT_DEPARTMENT_WISE_TEMPLATE objModel = new TT_DEPARTMENT_WISE_TEMPLATE();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    objModel.DEPARTMENT_TEMPLATE_ID = sDepatmentWiseTemplateId;
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteDepartmentWiseTimeTableTemplate), sAcademicYear);
                    if (ResultArg.Success)
                        sOption = Common.Messages.RecordDeletedSuccessfully;
                    else
                        sOption = Common.Messages.FailedToDeletedTryAgain;
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout + '\n';
                    sOption += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DeleteDepartmentWiseTimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DeleteDepartmentWiseTimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return sOption;
        }
        public async Task<ActionResult> DepartmentWiseTimeTableTemplate()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            var Setting = new List<TimeTableSetting>();
            var Department = new List<SupDepartment>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    Setting = await Task.Run(() => (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTimeTableSetting), sAcademicYear).DataSource.Data);
                    Department = await Task.Run(() => (List<SupDepartment>)CMSHandler.FetchData<SupDepartment>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data);
                    objViewModel.SettingList = new SelectList(Setting, Common.TT_TEMPLATE_SETTING.SETTING_ID, Common.TT_TEMPLATE_SETTING.SETTING_NAME);
                    objViewModel.Departmentlist = new SelectList(Department, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("DepartmentWiseTimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("DepartmentWiseTimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveDepartmentWiseTimeTableTemplate(string DepartmentWiseTimeTableTemplate)
        {
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var TempList = new List<TT_DEPARTMENT_WISE_TEMPLATE>();
                    if (DepartmentWiseTimeTableTemplate != null)
                    {
                        var Result = JsonConvert.DeserializeObject<TT_DEPARTMENT_WISE_TEMPLATE>(DepartmentWiseTimeTableTemplate);
                        string[] sArray = Result.DEPARTMENT_ID.Split(',');
                        List<string> List = sArray.ToList();
                        var FetchDepartment = (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(new TT_DEPARTMENT_WISE_TEMPLATE() { SETTING_ID = Result.SETTING_ID, Academic_Year = sAcademicYear }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDepartmentBySettingId), sAcademicYear).DataSource.Data;
                        if (FetchDepartment != null && FetchDepartment.Count > 0)
                        {
                            foreach (var item in FetchDepartment)
                            {
                                int i = 0;
                                foreach (string x in sArray)
                                {
                                    i++;
                                    if (item.DEPARTMENT_ID.Contains(x))
                                    {
                                        TempList.Add(FetchDepartment.SingleOrDefault(S => S.DEPARTMENT_ID == x));
                                        var index = Array.FindIndex(sArray, row => row.Contains(x));
                                        List.Remove(x);
                                    }
                                }
                            }
                            foreach (var dataitem in TempList)
                            {
                                if (dataitem != null)
                                    FetchDepartment.Remove(FetchDepartment.SingleOrDefault(s => s.DEPARTMENT_ID == dataitem.DEPARTMENT_ID));
                            }
                            foreach (var item in FetchDepartment)
                            {
                                var o = CMSHandler.SaveOrUpdate(item, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteDepartmentWiseTimeTableTemplate));
                            }
                        }
                        foreach (var dataitem in List)
                        {

                        }
                        var FetchExistingDepartmentWiseTimeTableTemplate = (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsDepartmentWiseTimeTableTemplateExisting), sAcademicYear).DataSource.Data;
                        if (FetchExistingDepartmentWiseTimeTableTemplate != null)
                            objResult.Message = "This Department Template Is Existing Already...!";
                        else
                        {
                            ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveDepartmentWiseTimeTableTemplate), sAcademicYear);
                            if (ResultArg.Success)
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            else
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                else
                {
                    objResult.Message = Common.ErrorCode.RequestTimeout + '\n';
                    objResult.Message += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("SaveDepartmentWiseTimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("SaveDepartmentWiseTimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return Json(objResult, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> EditDepartmentWiseTimeTableTemplate(string id)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    Session[Common.SESSION_VARIABLES.DEPARTMENT_TEMPLATE_ID] = id;
                    var Setting = new List<TimeTableSetting>();
                    var Department = new List<SupDepartment>();
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    Setting = await Task.Run(() => (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTimeTableSetting), sAcademicYear).DataSource.Data);
                    Department = await Task.Run(() => (List<SupDepartment>)CMSHandler.FetchData<SupDepartment>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data);
                    objViewModel.SettingList = new SelectList(Setting, Common.TT_TEMPLATE_SETTING.SETTING_ID, Common.TT_TEMPLATE_SETTING.SETTING_NAME);
                    objViewModel.Departmentlist = new SelectList(Department, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("EditDepartmentWiseTimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditDepartmentWiseTimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult FetchDepartmentmentWiseTimeTableTemplateById(string sDepartmentWiseTimeTableTemplateId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            TT_DEPARTMENT_WISE_TEMPLATE objModel = new TT_DEPARTMENT_WISE_TEMPLATE();
            objModel.DEPARTMENT_TEMPLATE_ID = sDepartmentWiseTimeTableTemplateId;
            var FetchTimeTableSetting = (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDepartmentWiseTimetableTemplateById), sAcademicYear).DataSource.Data;
            return Json(FetchTimeTableSetting);
        }
        public string UpdateDepartmentWiseTimeTableTemplate(string DepartmentWiseTimeTableTemplate)
        {
            string sOption = string.Empty;
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    if (DepartmentWiseTimeTableTemplate != null)
                    {
                        var Result = JsonConvert.DeserializeObject<TT_DEPARTMENT_WISE_TEMPLATE>(DepartmentWiseTimeTableTemplate);
                        Result.DEPARTMENT_TEMPLATE_ID = (Session[Common.SESSION_VARIABLES.DEPARTMENT_TEMPLATE_ID] != null) ? Session[Common.SESSION_VARIABLES.DEPARTMENT_TEMPLATE_ID].ToString() : "";
                        //var FetchExistingDepartmentWiseTimeTableTemplate = (List<TT_DEPARTMENT_WISE_TEMPLATE>)CMSHandler.FetchData<TT_DEPARTMENT_WISE_TEMPLATE>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsDepartmentWiseTimeTableTemplateExisting), sAcademicYear).DataSource.Data;
                        //if (FetchExistingDepartmentWiseTimeTableTemplate != null)
                        //    sOption = "This Department Template Is Existing Already...!";
                        //else
                        //{

                        //}
                        ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateDepartmentWiseTimeTableTemplate), sAcademicYear);
                        if (ResultArg.Success)
                            sOption = Common.Messages.RecordsSavedSuccessfully;
                        else
                            sOption = Common.Messages.FailedToSaveRecords;
                    }
                }
                else
                {
                    sOption = Common.ErrorCode.RequestTimeout + '\n';
                    sOption += Common.ErrorMessage.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("UpdateDepartmentWiseTimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("UpdateDepartmentWiseTimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return sOption;
        }
        #endregion

        #region TimeTable Template
        public async Task<ActionResult> TimeTableTemplateList()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var Setting = await Task.Run(() => (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTimeTableSetting), sAcademicYear).DataSource.Data);
                    if (Setting != null && Setting.Count > 0)
                        objViewModel.SettingList = new SelectList(Setting, Common.TT_TEMPLATE_SETTING.SETTING_ID, Common.TT_TEMPLATE_SETTING.SETTING_NAME);
                    var Faculty = await Task.Run(() => (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty), sAcademicYear).DataSource.Data);
                    if (Faculty != null && Faculty.Count > 0)
                        objViewModel.FacultyLIst = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                    var GraduationType = await Task.Run(() => (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel), sAcademicYear).DataSource.Data);
                    if (GraduationType != null && GraduationType.Count > 0)
                        objViewModel.GraduationType = new SelectList(GraduationType, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    var ClassYear = await Task.Run(() => (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear), sAcademicYear).DataSource.Data);
                    if (ClassYear != null && ClassYear.Count > 0)
                        objViewModel.ClassYearList = new SelectList(ClassYear, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                    //var TimeTableSetting = new TT_DEPARTMENT_WISE_TEMPLATE();
                    var ListTimeTableTemplate = await Task.Run(() => (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableTemplate), sAcademicYear).DataSource.Data);
                    objViewModel.lstTimeTableTemplate = ListTimeTableTemplate;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableTemplateList", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableTemplateList", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindTimeTableTemplate(string sSettingId, string sFacultyId, string sGraduationType, string sClassYear)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            TimeTableTemplate objModel = new TimeTableTemplate();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchSetting = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(new TimeTableSetting() { SETTING_ID = sSettingId }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableSettingById), sAcademicYear).DataSource.Data;
                    objModel.Setting_Id = sSettingId;
                    objModel.Faculty_Id = sFacultyId;
                    objModel.GraduationType = sGraduationType;
                    objModel.Year = sClassYear;
                    var FecthTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableTemplateBySettingIdAndYear), sAcademicYear).DataSource.Data;
                    if (FecthTimeTableTemplate != null && FecthTimeTableTemplate.Count > 0)
                        objViewModel.lstTimeTableTemplate = FecthTimeTableTemplate;
                    if (FetchSetting != null && FetchSetting.Count > 0)
                    {
                        var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchSetting.FirstOrDefault().NO_OF_HOURS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                        var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchSetting.FirstOrDefault().NO_OF_DAYS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                        if (FetchHours != null && FetchHours.Count > 0)
                            objViewModel.lstHours = FetchHours;
                        if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                            objViewModel.lstDayOrders = FetchDayOrders;
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindTimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindTimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public async Task<ActionResult> TimeTableTemplate()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            var Setting = new List<TimeTableSetting>();
            var Hours = new List<SUP_HOURS>();
            var Days = new List<SUP_DAY_ORDER>();
            var GraduationType = new List<SupClassLevel>();
            var Faculty = new List<SupFaculty>();
            var PaperType = new List<SupPaperType>();
            var Year = new List<SubClassYear>();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    Days = await Task.Run(() => (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrders)).DataSource.Data);
                    Hours = await Task.Run(() => (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data);
                    GraduationType = await Task.Run(() => (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data);
                    Setting = await Task.Run(() => (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTimeTableSetting)).DataSource.Data);
                    Faculty = await Task.Run(() => (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty), sAcademicYear).DataSource.Data);
                    PaperType = await Task.Run(() => (List<SupPaperType>)CMSHandler.FetchData<SupPaperType>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllPaperType)).DataSource.Data);
                    Year = await Task.Run(() => (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data);
                    objViewModel.SettingList = new SelectList(Setting, Common.TT_TEMPLATE_SETTING.SETTING_ID, Common.TT_TEMPLATE_SETTING.SETTING_NAME);
                    objViewModel.HourList = new SelectList(Hours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    objViewModel.DayOrderList = new SelectList(Days, Common.SUP_DAY_ORDERS.DAY_ORDER_ID, Common.SUP_DAY_ORDERS.DAY);
                    objViewModel.GraduationType = new SelectList(GraduationType, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
                    objViewModel.FacultyLIst = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
                    objViewModel.PaperTypeList = new SelectList(PaperType, Common.SUP_PAPER_TYPE.PAPER_TYPE_ID, Common.SUP_PAPER_TYPE.PAPER_TYPE);
                    objViewModel.ClassYearList = new SelectList(Year, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableTemplate", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableTemplate", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult SaveOrUpdateTimeTableTemplate(string sTemplate)
        {
            string sOption = string.Empty;
            JsonResultData objResult = new JsonResultData();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<TimeTableTemplate>(sTemplate);
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    if (CMSHandler.SaveOrUpdate(Result, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(string.IsNullOrEmpty(Result.TT_Temp_Id) ? TimeTableSQLCommands.SaveTimeTableTemplate : TimeTableSQLCommands.UpdateTimeTableTemplate), sAcademicYear).Success)
                    {
                        objResult.ErrorCode = Common.ErrorMessage.Ok;
                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimeTableController", "SaveOrUpdateTimeTableTemplate", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimeTableController", "SaveOrUpdateTimeTableTemplate", ex.Message);
                    }
                }
            }
            else
                objResult.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            return Json(objResult);
        }
        public string SaveTimeTableTemplate(string TimeTableTemplate)
        {
            string sOption = string.Empty;
            if (TimeTableTemplate != null)
            {
                var Result = JsonConvert.DeserializeObject<TimeTableTemplate>(TimeTableTemplate);
                var FecthTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableTemplateExisting)).DataSource.Data;
                if (FecthTimeTableTemplate != null)
                {
                    sOption = "This TimeTable Template Is Existing Already...!";
                }
                else
                {
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTableTemplate));
                    if (ResultArg.Success)
                    {
                        sOption = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        sOption = Common.Messages.FailedToSaveRecords;
                    }
                }
            }
            return sOption;
        }
        public string DeleteTimeTableTemplate(string sTemplateId)
        {
            string sOption = string.Empty;
            TimeTableTemplate objModel = new TimeTableTemplate();
            objModel.TT_Temp_Id = sTemplateId;
            ResultArgs ResultArg = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteTimeTableTemplate));
            if (ResultArg.Success)
            {
                sOption = Common.Messages.RecordDeletedSuccessfully;
            }
            else
            {
                sOption = Common.Messages.FailedToDeletedTryAgain;
            }
            return sOption;
        }
        public async Task<ActionResult> EditTimeTableTemplate(string id)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            Session[Common.SESSION_VARIABLES.TT_TEMP_ID] = id;
            var Setting = new List<TimeTableSetting>();
            var Hours = new List<SUP_HOURS>();
            var Days = new List<SUP_DAY_ORDER>();
            var GraduationType = new List<SupClassLevel>();
            var Faculty = new List<SupFaculty>();
            var PaperType = new List<SupPaperType>();
            var Year = new List<SubClassYear>();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            Days = await Task.Run(() => (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDayOrders)).DataSource.Data);
            Hours = await Task.Run(() => (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHours)).DataSource.Data);
            GraduationType = await Task.Run(() => (List<SupClassLevel>)CMSHandler.FetchData<SupClassLevel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassLevel)).DataSource.Data);
            Setting = await Task.Run(() => (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTimeTableSetting)).DataSource.Data);
            Faculty = await Task.Run(() => (List<SupFaculty>)CMSHandler.FetchData<SupFaculty>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchFaculty), sAcademicYear).DataSource.Data);
            PaperType = await Task.Run(() => (List<SupPaperType>)CMSHandler.FetchData<SupPaperType>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllPaperType)).DataSource.Data);
            Year = await Task.Run(() => (List<SubClassYear>)CMSHandler.FetchData<SubClassYear>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassYear)).DataSource.Data);
            objViewModel.SettingList = new SelectList(Setting, Common.TT_TEMPLATE_SETTING.SETTING_ID, Common.TT_TEMPLATE_SETTING.SETTING_NAME);
            objViewModel.HourList = new SelectList(Hours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
            objViewModel.DayOrderList = new SelectList(Days, Common.SUP_DAY_ORDERS.DAY_ORDER_ID, Common.SUP_DAY_ORDERS.DAY);
            objViewModel.GraduationType = new SelectList(GraduationType, Common.SUP_CLASS_LEVEL.CLASSLEVELID, Common.SUP_CLASS_LEVEL.CLASSLEVEL);
            objViewModel.FacultyLIst = new SelectList(Faculty, Common.CP_FACULTY_2017.FACULTY_ID, Common.CP_FACULTY_2017.FACULTY);
            objViewModel.PaperTypeList = new SelectList(PaperType, Common.SUP_PAPER_TYPE.PAPER_TYPE_ID, Common.SUP_PAPER_TYPE.PAPER_TYPE);
            objViewModel.ClassYearList = new SelectList(Year, Common.SUP_CLASS_YEAR.CLASS_YEAR_ID, Common.SUP_CLASS_YEAR.CLASS_YEAR);
            return View(objViewModel);
        }
        public JsonResult FetchTimeTableTemplateById(string sTemplateId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
            TimeTableTemplate objModel = new TimeTableTemplate();
            objModel.TT_Temp_Id = sTemplateId;
            var FetchTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableTemplateById), sAcademicYear).DataSource.Data;
            return Json(FetchTimeTableTemplate);
        }
        public string UpdateTimeTableTemplate(string TimeTableTemplate)
        {
            string sOption = string.Empty;
            if (TimeTableTemplate != null)
            {
                var Result = JsonConvert.DeserializeObject<TimeTableTemplate>(TimeTableTemplate);
                Result.TT_Temp_Id = Session[Common.SESSION_VARIABLES.TT_TEMP_ID].ToString();
                var FecthTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableTemplateExisting)).DataSource.Data;
                if (FecthTimeTableTemplate != null)
                {
                    sOption = "This TimeTable Template Is Existing Already...!";
                }
                else
                {
                    ResultArgs ResultArg = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimeTableTemplate));
                    if (ResultArg.Success)
                    {
                        sOption = Common.Messages.RecordsSavedSuccessfully;
                    }
                    else
                    {
                        sOption = Common.Messages.FailedToSaveRecords;
                    }
                }
            }
            return sOption;
        }
        #endregion

        #region TimeTable Registration For Staff
        public ActionResult TimeTableEntryForStaff()
        {
            stf_Personal_Info objStaffModel = new stf_Personal_Info();
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    //var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(objStaffModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassesByStaffId), sAcademicYear).DataSource.Data;
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
                    objHandler.WriteError("TimeTableEntryForStaff", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableEntryForStaff", "TimeTable", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindTimeTableForStaffByClassId(string sClassId)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            stf_Personal_Info objStaffModel = new stf_Personal_Info();
            TimeTableSetting objSettingModel = new TimeTableSetting();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    objStaffModel.STAFF_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    string sActiveSemester = Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString();
                    objStaffModel.CLASS_ID = sClassId;
                    var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStaffModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalHoursAndDaysByClassId), sAcademicYear).DataSource.Data;
                    if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                    {
                        var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                        var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                        if (FetchHours != null && FetchHours.Count > 0)
                            objViewModel.lstHours = FetchHours;
                        if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                            objViewModel.lstDayOrders = FetchDayOrders;
                    }
                    // Fetch Time Table Info By Staff ....
                    string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableInfoForStaffByClassId).Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Semester_Id.ToUpper(), sActiveSemester);
                    objViewModel.lstTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStaffModel, sSQL, sAcademicYear).DataSource.Data;
                    //if (objViewModel.lstTimeTable == null)
                    //   objViewModel.lstTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStaffModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableInfoForStaffByClassIdOnlyOptionalPapers), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindTimeTableForStaff", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("BindTimeTableForStaff", "TimeTable", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public JsonResult EditTimeTableForStaff(string sClassId, string sHourId, string sDayId, string sHourType, string sSemesterId, string sCourseId, string sStaffId)
        {
            string sOption = string.Empty;
            JsonResultData ObjJsonData = new JsonResultData();
            TimeTable objModel = new TimeTable();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                objModel.Class_Id = sClassId;
                objModel.Hour_Id = sHourId;
                objModel.hourType = sHourType;
                objModel.Day_Order_Id = sDayId;
                objModel.Course_Id = sCourseId;
                objModel.Staff_Id = sStaffId;
                objModel.Semester_Id = sSemesterId;
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                var CheckIsRegularType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsRegularPaperByStaffId), sAcademicYear).DataSource.Data;
                if (CheckIsRegularType != null && CheckIsRegularType.Count > 0)
                {
                    objModel.Time_Table_Id = CheckIsRegularType.SingleOrDefault().Time_Table_Id;
                    var FetchRegularPaperTimeTableById = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableById), sAcademicYear).DataSource.Data;

                    //objModel.Time_Table_Id = FetchRegularPaperTimeTableById.FirstOrDefault().Time_Table_Id;
                    objModel.CLASS_CODE = FetchRegularPaperTimeTableById.FirstOrDefault().CLASS_CODE;
                    //objModel.Course_Id = FetchRegularPaperTimeTableById.FirstOrDefault().Course_Id;
                    //objModel.Staff_Id = FetchRegularPaperTimeTableById.FirstOrDefault().Staff_Id;
                }
            }
            else
            {
                ObjJsonData.Message = Common.ErrorMessage.RequestTimeout;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchTimeTableInfoByDayIdAndHourId(string sDayOrderId, string sHourId, string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjModel = new TT_TIMETABLE_2017();
            var ObjTimeTable = new TimeTable();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                ObjModel.HOUR_ID = sHourId;
                ObjModel.DAY_ORDER_ID = sDayOrderId;
                ObjModel.STAFF_ID = sUserId;
                ObjModel.CLASS_ID = sClassId;
                var liFetchTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(ObjModel,
                      TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableInfoByDayIdAndHourId), sAcademicYear).DataSource.Data;
                if (liFetchTimeTable != null && liFetchTimeTable.Count > 0)
                {
                    foreach (var item in liFetchTimeTable)
                    {
                        ObjTimeTable.Time_Table_Id = item.Time_Table_Id;
                        ObjTimeTable.Class_Id = item.Class_Id;
                        ObjTimeTable.Course_Id = item.Course_Id;
                        ObjTimeTable.Day_Order_Id = item.Day_Order_Id;
                        ObjTimeTable.Hour_Id = item.Hour_Id;
                        ObjTimeTable.Hour_Type = item.Hour_Type;
                        ObjTimeTable.Staff_Id = item.Staff_Id;
                        ObjTimeTable.Semester_Id = item.Semester_Id;
                    }
                }

                string sSQL = string.Empty;
                sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffIdAndClassId);
                // Fetch Course By Staff Id ....
                var liCourse = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                if (liCourse != null && liCourse.Count > 0)
                {
                    foreach (var item in liCourse)
                    {
                        ObjTimeTable.COURSE_CODE += "<option value=" + item.COURSE_ROOT_ID + ">" + item.COURSE_TITLE + "</option>";
                    }
                }

                var liHourType = (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourType)).DataSource.Data;
                if (liHourType != null && liHourType.Count > 0)
                {
                    foreach (var item in liHourType)
                    {
                        ObjTimeTable.hourType += "<option value=" + item.hour_type_id + ">" + item.hourType + "</option>";
                    }
                }
                return Json(ObjTimeTable, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTimeTable(string sTimeTableId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            TimeTable objModel = new TimeTable();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                try
                {
                    objModel.Time_Table_Id = sTimeTableId;
                    var DeleteTimeTable = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteTimeTable), sAcademicYear);
                    if (DeleteTimeTable.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimeTableController", "DeleteTimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimeTableController", "DeleteTimeTable", ex.Message);
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
        public JsonResult SaveTimeTableStaffEntry(string TimeTableInfo)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sOption = string.Empty;
            if (TimeTableInfo != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        var Result = JsonConvert.DeserializeObject<TimeTable>(TimeTableInfo);
                        Result.Staff_Id = sUserId;
                        var liFetchSemester = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(Result,
                                SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterByCourseId), sAcademicYear).DataSource.Data;
                        if (liFetchSemester != null && liFetchSemester.Count > 0)
                        {
                            Result.Semester_Id = liFetchSemester.FirstOrDefault().SEMESTER_ID;
                        }
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsTimeTableExist).
                               Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        var IsStaffExist = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (IsStaffExist != null && IsStaffExist.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(Result.Time_Table_Id))
                            {
                                Result.Time_Table_Id = IsStaffExist.FirstOrDefault().Time_Table_Id;
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimeTableInfo), sAcademicYear);
                                ObjJsonData.Message = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                            else
                            {
                                ObjJsonData.Message = "This Hour Already Alloted !  For Class = " + IsStaffExist.FirstOrDefault().CLASS_CODE + "  \n Course =" + IsStaffExist.FirstOrDefault().COURSE_CODE + " \n Staff = " + IsStaffExist.FirstOrDefault().STAFF_CODE;
                            }
                        }
                        else
                        {
                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                            ObjJsonData.Message = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }

                        //if (Result.Course_Id != "0")
                        //{
                        //    var CheckIsOptionalPaper = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchOptionalPaperByCourseId), sAcademicYear).DataSource.Data;
                        //    var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAlloted), sAcademicYear).DataSource.Data;
                        //    if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 1)
                        //    {
                        //        ObjJsonData.Message = "There Are Duplicate Data, Please Meet Admin...!!!";
                        //        return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                        //    }
                        //    if (CheckIsOptionalPaper != null && CheckIsOptionalPaper.Count > 0)
                        //    {

                        //        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        //        {
                        //            foreach (var item in FetchAllotedStaff)
                        //            {

                        //                if (CheckIsOptionalPaper.FirstOrDefault().Course_Id == item.Course_Id)
                        //                {
                        //                    ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //                    if (ResultArgs.Success)
                        //                    {
                        //                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        //                    }
                        //                    else
                        //                    {
                        //                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    ObjJsonData.Message = "This Staff Already Alloted To Another Class...!!!";
                        //                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            var FetchIsStaticHourAndPaperType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchIsStaticAndPaperTypeByClassId), sAcademicYear).DataSource.Data;
                        //            if (FetchIsStaticHourAndPaperType != null && FetchIsStaticHourAndPaperType.Count > 0)
                        //            {
                        //                if (FetchIsStaticHourAndPaperType.FirstOrDefault().IS_STATIC == Common.IsActiveFalg.IsNotActive)
                        //                {
                        //                    ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //                    if (ResultArgs.Success)
                        //                    {
                        //                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        //                    }
                        //                    else
                        //                    {
                        //                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    var FetchPaperType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPaperType), sAcademicYear).DataSource.Data;
                        //                    var FetchHourType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourType), sAcademicYear).DataSource.Data;
                        //                    if (FetchPaperType != null && FetchPaperType.Count > 0 && FetchHourType != null && FetchHourType.Count > 0)
                        //                    {
                        //                        Result.PAPER_TYPE = FetchPaperType.SingleOrDefault().PAPER_TYPE;
                        //                        Result.Paper_Type_Id = FetchHourType.SingleOrDefault().Paper_Type_Id;
                        //                        if (Result.PAPER_TYPE == Result.Paper_Type_Id)
                        //                        {
                        //                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //                            if (ResultArgs.Success)
                        //                            {
                        //                                ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        //                            }
                        //                            else
                        //                            {
                        //                                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            ObjJsonData.Message = "This Course is not eligible for this hour...!!!";
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        ObjJsonData.Message = "No paper type and hour type for this course...!!!";
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                ObjJsonData.Message = "Please check the template and No paper type and hour type for this course...!!!";
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        //        {
                        //            ObjJsonData.Message = "This Staff Already Alloted To Another Class...!!!";
                        //        }
                        //        else
                        //        {
                        //            var FetchIsStaticHourAndPaperType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchIsStaticAndPaperTypeByClassId), sAcademicYear).DataSource.Data;
                        //            if (FetchIsStaticHourAndPaperType != null && FetchIsStaticHourAndPaperType.Count > 0)
                        //            {
                        //                if (FetchIsStaticHourAndPaperType.FirstOrDefault().IS_STATIC == Common.IsActiveFalg.IsNotActive)
                        //                {
                        //                    ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //                    if (ResultArgs.Success)
                        //                    {
                        //                        ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        //                    }
                        //                    else
                        //                    {
                        //                        ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    var FetchPaperType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchPaperType), sAcademicYear).DataSource.Data;
                        //                    var FetchHourType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourType), sAcademicYear).DataSource.Data;
                        //                    if (FetchPaperType != null && FetchPaperType.Count > 0 && FetchHourType != null && FetchHourType.Count > 0)
                        //                    {
                        //                        Result.PAPER_TYPE = FetchPaperType.SingleOrDefault().PAPER_TYPE;
                        //                        Result.Paper_Type_Id = FetchHourType.SingleOrDefault().Paper_Type_Id;
                        //                        if (Result.PAPER_TYPE == Result.Paper_Type_Id)
                        //                        {
                        //                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //                            if (ResultArgs.Success)
                        //                            {
                        //                                ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        //                            }
                        //                            else
                        //                            {
                        //                                ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            ObjJsonData.Message = "This Course is not eligible for this hour...!!!";
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        ObjJsonData.Message = "No paper type and hour type for this course...!!!";
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                ObjJsonData.Message = "Please check the template and No paper type and hour type for this course...!!!";
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    var FetchIsStaticHourAndPaperType = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchIsStaticAndPaperTypeByClassId), sAcademicYear).DataSource.Data;
                        //    if (FetchIsStaticHourAndPaperType.FirstOrDefault().IS_STATIC == "0")
                        //    {
                        //        ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicYear);
                        //        if (ResultArgs.Success)
                        //        {
                        //            ObjJsonData.Message = Common.Messages.RecordsSavedSuccessfully;
                        //        }
                        //        else
                        //        {
                        //            ObjJsonData.Message = Common.Messages.FailedToSaveRecords;
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog objHandler = new ErrorLog())
                        {
                            objHandler.WriteError("TimeTableController", "SaveTimeTableStaffEntry", "Err MSg: " + ex.Message, "Query is empty!");
                            objHandler.WriteError("TimeTableController", "SaveTimeTableStaffEntry", ex.Message);
                        }
                        ObjJsonData.Message = Common.ErrorMessage.InternalServerError;
                        ObjJsonData.ErrorCode = Common.ErrorCode.InternalServerError;
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.RequestTimeout;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StaffOverallTimetable()
        {
            string sUserId = string.Empty;
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            stf_Personal_Info objStfModel = new stf_Personal_Info();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                objStfModel.STAFF_ID = sUserId;
                var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicyear).DataSource.Data;
                if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicyear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicyear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        objViewModel.lstHours = FetchHours;
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                        objViewModel.lstDayOrders = FetchDayOrders;
                }
                objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicyear).DataSource.Data;

                objViewModel.liSubstituteList = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubstituteHourByStaffId), sAcademicyear).DataSource.Data;
            }
            else
                return RedirectToAction("Session Expired Login again..!", "error");

            return View(objViewModel);
        }
        #endregion

        #region Timetable Report(paper based,Staff based,Hour based)
        public ActionResult TimetableReportStaffBased()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                //var StaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                //objViewModel.StaffList = new SelectList(StaffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);

                var academicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList)).DataSource.Data;
                objViewModel.AcademicYear = new SelectList(academicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

            }
            else
                return RedirectToAction("ErrorMessage", "Error");
            return View(objViewModel);
        }
        public JsonResult FetchStaffByAcYearandSemtype(string sSemesterTypeId, string sAcYear)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcYear))
            {
                try
                {
                    var StaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(new TimeTable() { ACADEMIC_YEAR = sAcYear, SEMESTER_TYPE_ID = sSemesterTypeId }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchttStaffBasedSemtyeandAcyear)).DataSource.Data;
                    {
                        if (StaffList != null)
                        {
                            foreach (var item in StaffList)
                            {
                                ObjJsonData.sResult += "<option value='" + item.STAFF_ID + "' >" + item.FIRST_NAME + "</option>";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjHandler = new ErrorLog())
                    {
                        ObjHandler.WriteError("TimeTableController", "FetchSubStaffforTpm", "Err MSg: " + ex.Message, "Query is empty!");
                        ObjHandler.WriteError("TimeTableController", "FetchSubStaffforTpm", ex.Message);
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
        public ActionResult BindTimetableReport(string sStaffId)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                stf_Personal_Info objStfModel = new stf_Personal_Info();
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                objStfModel.STAFF_ID = sStaffId;

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

                var liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableByStaffBased), sAcademicYear).DataSource.Data;
                var liCourse = liTimetable; // (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableByStaffBased), sAcademicYear).DataSource.Data;
                                            //objViewModel.lstTimeTable =(List<TimeTable>)liTimetable.Zip(liCourse, (t, c) => new { Time = t, Course = c });
                                            //objViewModel.lstTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableHoursCountByStaffBased), sAcademicYear).DataSource.Data;
                objViewModel.lstTimeTable = liTimetable;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TimeTable", "BindTimetableReport", ex.Message);
                }
            }

            return View(objViewModel);
        }
        public ActionResult TimeTableReportForIncharge()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STF_PERSONAL_INFO();
                    ObjModel.STAFF_ID = sStaffId;
                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffUserDepartmentId), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjViewModel.StaffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTable", "BindTimetableReport", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult TimeTableReportForStudent()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjViewModel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new STU_PERSONAL_INFO();
                    ObjModel.STUDENT_ID = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    var liStudentInfo = (List<Student_Personal_Info>)CMSHandler.FetchData<Student_Personal_Info>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentInfoByUserId), sAcademicYear).DataSource.Data;
                    if (liStudentInfo != null && liStudentInfo.Count > 0)
                    {
                        ObjViewModel.sMessage = liStudentInfo.FirstOrDefault().CLASS_ID;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "TimeTableReportForStudent", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult TimeTableHoursCountForSJC()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            // string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            TimeTableViewModel Objttmodel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Listf = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchTeachingStaffOnly), sAcademicYear).DataSource.Data;
                    if (Listf != null && Listf.Count > 0)
                    {
                        Objttmodel.Listaff = Listf;
                    }
                    var Lihourscount = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimetableHourscount), sAcademicYear).DataSource.Data;
                    if (Lihourscount != null && Lihourscount.Count > 0)
                    {
                        Objttmodel.lstTimeTable = Lihourscount;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "TimeTableHoursCountForSJC", ex.Message);
                    }
                }
            }
            return View(Objttmodel);
        }
        public ActionResult TimeTableClassWiseReport()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTableClassWiseReport", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTableClassWiseReport", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }

        public ActionResult TimetableReportForHOD()
        {
            string sAdademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUser_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            TimeTableViewModel objttmodel = new TimeTableViewModel();
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
                            objttmodel.ClassList = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "Timetablereportforhod", ex.Message);
                    }
                }
            }
            return View(objttmodel);
        }

        public ActionResult BindTimetableReportForAllStaff(string sSemesterTypeId, string sStaffId)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                CourseWiseStaffList objStfModel = new CourseWiseStaffList();
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objStfModel.STAFF_ID = sStaffId;
                objStfModel.SEMESTER_TYPE_ID = sSemesterTypeId;
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
                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableBySemesterTypeAndStaffIds).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sStaffId);
                var liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, sSQL, sAcademicYear).DataSource.Data;
                var liCourse = liTimetable;
                // (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableByStaffBased), sAcademicYear).DataSource.Data;
                //objViewModel.lstTimeTable =(List<TimeTable>)liTimetable.Zip(liCourse, (t, c) => new { Time = t, Course = c });
                //objViewModel.lstTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableHoursCountByStaffBased), sAcademicYear).DataSource.Data;
                objViewModel.lstTimeTable = liTimetable;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TimeTable", "BindTimetableReport", ex.Message);
                }
            }

            return View(objViewModel);
        }
        public ActionResult BindTimetableReportForAllStaffBasedAcYear(string sSemesterTypeId, string sStaffId, string sAcYear)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                CourseWiseStaffList objStfModel = new CourseWiseStaffList();
                //string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                objStfModel.STAFF_ID = sStaffId;
                objStfModel.SEMESTER_TYPE_ID = sSemesterTypeId;
                var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcYear).DataSource.Data;
                if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcYear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcYear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        objViewModel.lstHours = FetchHours;
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                        objViewModel.lstDayOrders = FetchDayOrders;
                }
                else
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcYear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = "6" }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcYear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        objViewModel.lstHours = FetchHours;
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                        objViewModel.lstDayOrders = FetchDayOrders;
                }
                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableBySemesterTypeAndStaffIds).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.STAFF_ID, sStaffId);
                var liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, sSQL, sAcYear).DataSource.Data;
                var liCourse = liTimetable; // (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableByStaffBased), sAcademicYear).DataSource.Data;
                                            //objViewModel.lstTimeTable =(List<TimeTable>)liTimetable.Zip(liCourse, (t, c) => new { Time = t, Course = c });
                                            //objViewModel.lstTimeTableTemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTimeTableHoursCountByStaffBased), sAcademicYear).DataSource.Data;
                objViewModel.lstTimeTable = liTimetable;
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TimeTable", "BindTimetableReport", ex.Message);
                }
            }

            return View(objViewModel);
        }
        #endregion

        #region TimeTable Entry For SJC
        public ActionResult TimeTableEntryForSJC()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            //string sDepartment = (Session[Common.SESSION_VARIABLES.DEPARTMENT_ID] != null) ? Session[Common.SESSION_VARIABLES.DEPARTMENT_ID].ToString() : string.Empty;
            //string sShift = (Session[Common.SESSION_VARIABLES.SHIFT_ID] != null) ? Session[Common.SESSION_VARIABLES.SHIFT_ID].ToString() : string.Empty;
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            var ObjModel = new STF_PERSONAL_INFO();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sUserId;

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjModel.STAFF_ID = liStaff.FirstOrDefault().STAFF_ID;
                        ObjModel.DEPARTMENT = liStaff.FirstOrDefault().DEPARTMENT;

                        string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.SHIFT, liStaff.FirstOrDefault().SHIFT);
                        var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchClass != null && FetchClass.Count > 0)
                        {
                            ObjViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "TimeTableEntryForSJC", ex.Message);
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

        #region Add Substitute Hour
        public ActionResult ListSubstituteHour()
        {
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liClassList = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (liClassList != null && liClassList.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }

                    var liStafffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                    if (liStafffList != null && liStafffList.Count > 0)
                    {
                        ObjViewModel.StaffList = new SelectList(liStafffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("AttendanceController", "ListSubstituteHour", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }

        public ActionResult BindSubstituteHourByClassId(string sClassId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ATT_SUBSTITUTE ObjModel = new ATT_SUBSTITUTE();
                    var liSubstituteHour = new List<ATT_SUBSTITUTE>();
                    ObjModel.CLASS_ID = sClassId;
                    var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(ObjModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalHoursAndDaysByClassId), sAcademicYear).DataSource.Data;
                    if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                    {
                        var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicYear).DataSource.Data;
                        var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicYear).DataSource.Data;
                        if (FetchHours != null && FetchHours.Count > 0)
                            ObjViewModel.lstHours = FetchHours;
                        if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                            ObjViewModel.lstDayOrders = FetchDayOrders;
                    }

                    string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.BindSubstituteHoursbyClassId);
                    liSubstituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                    ObjViewModel.liSubstituteHour = liSubstituteHour;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "BindSubstituteHourByClassId", ex.Message);
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

        public JsonResult FetchSubstituteHurinfo(string sJson)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = JsonConvert.DeserializeObject<ATT_SUBSTITUTE>(sJson);
                    var liSubstituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(ObjModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubstituteHourInfo), sAcademicYear).DataSource.Data;
                    if (liSubstituteHour != null && liSubstituteHour.Count > 0)
                    {
                        ObjViewModel.liSubstituteHour = liSubstituteHour;
                    }

                    var liDayOrderDate = (List<CALENDER_2017>)CMSHandler.FetchData<CALENDER_2017>(ObjModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDateByDayOrderId), sAcademicYear).DataSource.Data;
                    if (liDayOrderDate != null && liDayOrderDate.Count > 0)
                    {
                        ObjViewModel.liCalender = liDayOrderDate;
                    }
                    return Json(ObjViewModel, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "FetchSubstituteHurinfo", ex.Message);
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

        public JsonResult SaveSubstituteHour(string sSubstitute)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<ATT_SUBSTITUTE>(sSubstitute);
                    Result.COURSE_ROOT_ID = Result.COURSE_ID;
                    var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(Result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                    if (liCourseRoot != null && liCourseRoot.Count > 0)
                    {
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAlloted);
                        // Check Is Staff Time Table ...
                        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        {
                            ObjJsonData.Message = "This Staff Already Alloted To " + FetchAllotedStaff.FirstOrDefault().CLASS_CODE;
                        }
                        else
                        {
                            Result.DAY_ORDER_DATE = CommonMethods.ConvertdatetoyearFromat(Result.DAY_ORDER_DATE);
                            // Check Is Staff Alloted For Substitute Hour ...
                            var liSubstituteHour = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IsSubstituteHourExist), sAcademicYear).DataSource.Data;
                            if (liSubstituteHour != null && liSubstituteHour.Count > 0)
                            {
                                ObjJsonData.Message = "This Staff Already Alloted For Substitute Hour !";
                            }
                            else
                            {
                                Result.SEMESTER_TYPE_ID = "0";
                                Result.STAFF_ID = Result.STAFF_CODE;
                                Result.ENTRY_ID = sUserId;
                                sSQL = string.Empty;
                                sSQL = (!string.IsNullOrEmpty(Result.SUBSTITUTE_ID)) ? TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateSubstituteHour) : TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveSubstituteHour);
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, sSQL, sAcademicYear);
                                ObjJsonData.Message = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "SaveSubstituteHour", ex.Message);
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

            if (string.IsNullOrEmpty(ObjJsonData.Message))
            {
                ObjJsonData.Message = "No Changes Made !";
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSubstituteHour(string sSubstituteId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var ObjModel = new ATT_SUBSTITUTE();
                    ObjModel.SUBSTITUTE_ID = sSubstituteId;
                    var DeleteSubstituteHour = CMSHandler.SaveOrUpdate(ObjModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteSubstituteHour), sAcademicYear);
                    ObjJsonData.Message = (DeleteSubstituteHour.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
                    ObjJsonData.ErrorCode = (DeleteSubstituteHour.Success) ? Common.ErrorCode.Ok : Common.ErrorCode.NotAcceptable;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "DeleteSubstituteHour", ex.Message);
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
        #endregion

        #region Timetable Generation
        public string TimetableGeneration()
        {
            string SQL = string.Empty;
            string sOPtionClass = string.Empty;
            string Dayorder = string.Empty;
            string Hour = string.Empty;
            int stfhr_week = 0;
            int coursehr_week = 0;
            int tmp = 0;
            Random rdm = new Random();
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            List<CP_COURSE_PATTERN> LicoursePattern = new List<CP_COURSE_PATTERN>();
            List<TimeTableTemplate> LiTimetabletemplate = new List<TimeTableTemplate>();
            List<CP_CLASS_COURSE_STAFF> ClassCoursesStaff = new List<CP_CLASS_COURSE_STAFF>();
            List<CP_CLASS_COURSE_STAFF> CourseWisestaff = new List<CP_CLASS_COURSE_STAFF>();
            List<CP_CLASS_COURSE_STAFF> NMECourseWisestaff = new List<CP_CLASS_COURSE_STAFF>();
            List<TimeTableTemplate> linmetemplates = new List<TimeTableTemplate>();
            List<TimeTableTemplate> matchtemplate = new List<TimeTableTemplate>();
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    //Fetch Course Pattern
                    LicoursePattern = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOverAllCoursePattern), sAcademicyear).DataSource.Data;
                    //Fetch Paper type
                    var Listpapertype = (List<SupPaperType>)CMSHandler.FetchData<SupPaperType>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSupPaparType)).DataSource.Data;
                    //Fetch Timetable
                    var Lioveralltimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOverAlltimetable), sAcademicyear).DataSource.Data;
                    //Fetch  Classwise Course with staff
                    SQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffHourAllotment).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    ClassCoursesStaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(null, SQL, sAcademicyear).DataSource.Data;

                    //Fetch Overall Template 
                    LiTimetabletemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchProgammeTimetabletemplate), sAcademicyear).DataSource.Data;

                    //Fetch Optional Class
                    sOPtionClass = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchoverallOptionclass).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    var LiOptionalClass = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(null, sOPtionClass, sAcademicyear).DataSource.Data;

                    if (Listpapertype != null && Listpapertype.Count > 0)
                    {
                        foreach (var papertype in Listpapertype)
                        {

                            //Group course pattern by paper type
                            var papertypecoursepattern = LicoursePattern.Where(o => o.PAPER_TYPE == papertype.PAPER_TYPE_ID).ToList();

                            //Group by Programme
                            var Listprogrammegrouped = LicoursePattern.GroupBy(o => o.PROGRAMME_GROUP_ID).ToList();

                            if (papertype.PAPER_TYPE_ID == Common.PAPER_TYPE.NME)
                            {
                                var LiNMEClassecoursee = LiOptionalClass.Where(o => o.PAPER_TYPE == papertype.PAPER_TYPE_ID);
                                foreach (var nmecls in LiNMEClassecoursee)
                                {
                                    //Template by Class year and shift and papertype
                                    var Litimetbltemplate = LiTimetabletemplate.Where(o => o.Year == nmecls.CLASS_YEAR && o.SHIFT == nmecls.SHIFT && o.Paper_Type_Id == papertype.PAPER_TYPE_ID && o.Faculty_Id == nmecls.FACULTY && o.GraduationType == nmecls.CLASS_LEVEL).ToList();

                                    //Fetch nmeclass classcourse staff
                                    SQL = string.Empty;
                                    SQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchNMEclasscoursestaff).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                                    var Linmeclasscoursestaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new CP_CLASSES() { CLASS_ID = nmecls.CLASS_ID }, SQL, sAcademicyear).DataSource.Data;

                                    if (Litimetbltemplate.Count > 0 && Litimetbltemplate != null && Linmeclasscoursestaff != null && Linmeclasscoursestaff.Count > 0)
                                    {
                                        foreach (var staff in Linmeclasscoursestaff)
                                        {
                                            while (stfhr_week < Convert.ToInt32(staff.HRS_WEEK))
                                            {
                                                NMECourseWisestaff.Add(new CP_CLASS_COURSE_STAFF() { STAFF_ID = staff.STAFF_ID, STAFF_CODE = staff.STAFF_CODE, COURSE_ID = staff.COURSE_ID, CLASS_ID = staff.CLASS_ID, CLSCRSSTF_ID = staff.CLSCRSSTF_ID });
                                                stfhr_week++;
                                            }
                                            stfhr_week = 0;
                                        }
                                        linmetemplates = Litimetbltemplate;
                                        while (NMECourseWisestaff.Count != 0)
                                        {
                                            if (NMECourseWisestaff.Count != 0 && linmetemplates.Count != 0)
                                            {

                                                foreach (var nmetemplate in linmetemplates)
                                                {

                                                    foreach (var staff in NMECourseWisestaff)
                                                    {
                                                        var Staffpossibilityhours = HourPossibility(staff.STAFF_ID, staff.PAPER_TYPE, staff.COURSE_ID, nmecls.FACULTY);
                                                        if (Staffpossibilityhours != null && Staffpossibilityhours.Count > 0)
                                                        {
                                                            var Listallotable = Staffpossibilityhours.Where(o => o.Day_Order_Id == nmetemplate.Day_Order_Id && o.Hour_Id == nmetemplate.Hour_Id).ToList();
                                                            if (Listallotable != null && Listallotable.Count > 0)
                                                            {

                                                                //Insert course with staff
                                                                var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Staff_Id = staff.STAFF_ID, Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = Listallotable.FirstOrDefault().Hour_Id, Day_Order_Id = Listallotable.FirstOrDefault().Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);

                                                            }
                                                            else
                                                            {
                                                                //Insert course
                                                                var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = nmetemplate.Hour_Id, Day_Order_Id = nmetemplate.Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Insert course
                                                            var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = nmetemplate.Hour_Id, Day_Order_Id = nmetemplate.Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                        }
                                                        linmetemplates.RemoveAll(x => x.TT_Temp_Id == nmetemplate.TT_Temp_Id);
                                                        NMECourseWisestaff.RemoveAt(0);
                                                        break;
                                                    }
                                                    break;


                                                }
                                            }
                                            else if (NMECourseWisestaff.Count != 0 && linmetemplates.Count == 0)
                                            {
                                                linmetemplates = LiTimetabletemplate.Where(o => o.Year == nmecls.CLASS_YEAR && o.SHIFT == nmecls.SHIFT && o.Paper_Type_Id == papertype.PAPER_TYPE_ID && o.Faculty_Id == nmecls.FACULTY && o.GraduationType == nmecls.CLASS_LEVEL).ToList();
                                                if (linmetemplates != null && linmetemplates.Count > 0)
                                                {
                                                    while (NMECourseWisestaff.Count != 0)
                                                    {
                                                        if (linmetemplates == null || linmetemplates.Count <= 0)
                                                        {
                                                            linmetemplates = LiTimetabletemplate.Where(o => o.Year == nmecls.CLASS_YEAR && o.SHIFT == nmecls.SHIFT && o.Paper_Type_Id == papertype.PAPER_TYPE_ID && o.Faculty_Id == nmecls.FACULTY && o.GraduationType == nmecls.CLASS_LEVEL).ToList();
                                                        }
                                                        foreach (var nmetemplate in linmetemplates)
                                                        {
                                                            foreach (var staff in NMECourseWisestaff)
                                                            {
                                                                var Staffpossibilityhours = HourPossibility(staff.STAFF_ID, staff.PAPER_TYPE, staff.COURSE_ID, nmecls.FACULTY);
                                                                if (Staffpossibilityhours != null && Staffpossibilityhours.Count > 0)
                                                                {
                                                                    var Listallotable = Staffpossibilityhours.Where(o => o.Day_Order_Id == nmetemplate.Day_Order_Id && o.Hour_Id == nmetemplate.Hour_Id).ToList();
                                                                    if (Listallotable != null && Listallotable.Count > 0)
                                                                    {
                                                                        //Insert course with staff
                                                                        var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Staff_Id = staff.STAFF_ID, Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = Listallotable.FirstOrDefault().Hour_Id, Day_Order_Id = Listallotable.FirstOrDefault().Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);

                                                                    }
                                                                    else
                                                                    {
                                                                        //Insert course
                                                                        var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = nmetemplate.Hour_Id, Day_Order_Id = nmetemplate.Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    //Insert course
                                                                    var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = nmetemplate.Hour_Id, Day_Order_Id = nmetemplate.Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                }
                                                                NMECourseWisestaff.RemoveAt(0);
                                                                linmetemplates.RemoveAll(x => x.TT_Temp_Id == nmetemplate.TT_Temp_Id);
                                                                break;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (Litimetbltemplate != null && Litimetbltemplate.Count > 0)
                                        {
                                            foreach (var nmetemplate in Litimetbltemplate)
                                            {
                                                //Insert course
                                                var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = nmecls.COURSE_ROOT_ID, Hour_Id = nmetemplate.Hour_Id, Day_Order_Id = nmetemplate.Day_Order_Id, Class_Id = nmecls.CLASS_ID, Semester_Id = nmecls.SEMESTER_ID, ACADEMIC_YEAR = nmecls.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                            }
                                        }
                                    }


                                }
                                continue;
                            }


                            foreach (var programme in Listprogrammegrouped)
                            {
                                //Fetch Programme Classess
                                var LiClassess = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES() { PROGRAMME_GROUP_ID = programme.FirstOrDefault().PROGRAMME_GROUP_ID }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClassessByProgramme), sAcademicyear).DataSource.Data;
                                var RegularClassess = LiClassess.Where(x => x.CLASS_TYPE == Common.CLASS_TYPE.R_Regular);

                                foreach (var cls in RegularClassess)
                                {

                                    var Liclasswisecourse = ClassCoursesStaff.Where(o => o.CLASS_ID == cls.CLASS_ID && o.PAPER_TYPE == papertype.PAPER_TYPE_ID).ToList();

                                    //Template by Class year and shift and papertype
                                    var Litimetbltemplate = LiTimetabletemplate.Where(o => o.Year == cls.CLASS_YEAR && o.SHIFT == cls.SHIFT && o.Paper_Type_Id == papertype.PAPER_TYPE_ID && o.Faculty_Id == programme.FirstOrDefault().FACULTY_ID && o.GraduationType == cls.CLASS_LEVEL).ToList();

                                    foreach (var course in Liclasswisecourse)
                                    {
                                        if (papertype.PAPER_TYPE_ID == Common.PAPER_TYPE.Language || papertype.PAPER_TYPE_ID == Common.PAPER_TYPE.English || papertype.PAPER_TYPE_ID == Common.PAPER_TYPE.Value_Education)
                                        {
                                            //Template by Class year and shift and papertype
                                            Litimetbltemplate = LiTimetabletemplate.Where(o => o.Year == cls.CLASS_YEAR && o.SHIFT == cls.SHIFT && o.Paper_Type_Id == papertype.PAPER_TYPE_ID && o.Faculty_Id == programme.FirstOrDefault().FACULTY_ID && o.GraduationType == cls.CLASS_LEVEL).ToList();
                                        }
                                        var Dayordergroup = Litimetbltemplate.GroupBy(o => o.Day_Order_Id).ToList();
                                        if (Dayordergroup != null && Dayordergroup.Count > 0)
                                        {
                                            if (!string.IsNullOrEmpty(course.STAFF_ID))
                                            {
                                                while (coursehr_week < Convert.ToInt32(course.HRS_PER_WEEK))
                                                {

                                                    if (Litimetbltemplate != null && Litimetbltemplate.Count > 0)
                                                    {
                                                        foreach (var day in Dayordergroup)
                                                        {
                                                            if (coursehr_week < Convert.ToInt32(course.HRS_PER_WEEK))
                                                            {
                                                                var templatelist = Litimetbltemplate.Where(o => o.Day_Order_Id == day.FirstOrDefault().Day_Order_Id).ToList();
                                                                if (templatelist != null && templatelist.Count > 0)
                                                                {

                                                                    var Staffpossibilityhours = HourPossibility(course.STAFF_ID, course.PAPER_TYPE, course.COURSE_ID, programme.FirstOrDefault().FACULTY_ID);
                                                                    foreach (var item in templatelist)
                                                                    {
                                                                        var list = Staffpossibilityhours.Where(o => o.Hour_Id == item.Hour_Id && o.Day_Order_Id == item.Day_Order_Id).ToList();
                                                                        if (list != null && list.Count > 0)
                                                                        {
                                                                            matchtemplate = (List<TimeTableTemplate>)matchtemplate.Concat(list);
                                                                        }
                                                                        else
                                                                        {
                                                                            matchtemplate = list;
                                                                        }
                                                                    }
                                                                    var temp = matchtemplate != null && matchtemplate.Count > 0 ? matchtemplate[rdm.Next(matchtemplate.Count)] : templatelist[rdm.Next(templatelist.Count)];

                                                                    if (Staffpossibilityhours != null && Staffpossibilityhours.Count > 0)
                                                                    {
                                                                        var Listallotable = Staffpossibilityhours.Where(o => o.Day_Order_Id == day.FirstOrDefault().Day_Order_Id && o.Hour_Id == temp.Hour_Id).ToList();

                                                                        if (stfhr_week < Convert.ToInt32(course.HRS_WEEK))
                                                                        {
                                                                            if (Listallotable != null && Listallotable.Count() > 0)
                                                                            {

                                                                                //Insert course with staff
                                                                                var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Staff_Id = course.STAFF_ID, Course_Id = course.COURSE_ID, Hour_Id = Listallotable.FirstOrDefault().Hour_Id, Day_Order_Id = Listallotable.FirstOrDefault().Day_Order_Id, Class_Id = course.CLASS_ID, Semester_Id = course.SEMESTER_ID, ACADEMIC_YEAR = course.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                                Litimetbltemplate.RemoveAll(o => o.TT_Temp_Id == temp.TT_Temp_Id);
                                                                                stfhr_week++;
                                                                                coursehr_week++;
                                                                            }
                                                                            else if (tmp >= Convert.ToInt32(Litimetbltemplate.Count) && stfhr_week < Convert.ToInt32(course.HRS_WEEK))
                                                                            {
                                                                                var IsExist = TemplatePossibility(temp.Hour_Id, day.FirstOrDefault().Day_Order_Id, cls.CLASS_ID, course.COURSE_ID, course.STAFF_ID);
                                                                                if (IsExist == null)
                                                                                {
                                                                                    //Insert course
                                                                                    var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = course.COURSE_ID, Hour_Id = temp.Hour_Id, Day_Order_Id = temp.Day_Order_Id, Class_Id = course.CLASS_ID, Semester_Id = course.SEMESTER_ID, ACADEMIC_YEAR = course.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                                }
                                                                                Litimetbltemplate.RemoveAll(o => o.TT_Temp_Id == temp.TT_Temp_Id);
                                                                                stfhr_week++;
                                                                                coursehr_week++;
                                                                            }

                                                                        }
                                                                        else
                                                                        {
                                                                            var IsExist = TemplatePossibility(temp.Hour_Id, day.FirstOrDefault().Day_Order_Id, cls.CLASS_ID, course.COURSE_ID, course.STAFF_ID);
                                                                            if (IsExist == null)
                                                                            {
                                                                                //Insert course
                                                                                var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = course.COURSE_ID, Hour_Id = temp.Hour_Id, Day_Order_Id = temp.Day_Order_Id, Class_Id = course.CLASS_ID, Semester_Id = course.SEMESTER_ID, ACADEMIC_YEAR = course.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                            }
                                                                            Litimetbltemplate.RemoveAll(o => o.TT_Temp_Id == temp.TT_Temp_Id);
                                                                            coursehr_week++;
                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        var IsExist = TemplatePossibility(temp.Hour_Id, day.FirstOrDefault().Day_Order_Id, cls.CLASS_ID, course.COURSE_ID, course.STAFF_ID);
                                                                        if (IsExist == null)
                                                                        {
                                                                            //Insert course
                                                                            var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = course.COURSE_ID, Hour_Id = temp.Hour_Id, Day_Order_Id = temp.Day_Order_Id, Class_Id = course.CLASS_ID, Semester_Id = course.SEMESTER_ID, ACADEMIC_YEAR = course.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                        }
                                                                        Litimetbltemplate.RemoveAll(o => o.TT_Temp_Id == temp.TT_Temp_Id);
                                                                        coursehr_week++;
                                                                    }


                                                                    tmp++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                stfhr_week = 0;
                                                coursehr_week = 0;
                                            }
                                            else
                                            {
                                                while (coursehr_week < Convert.ToInt32(course.HRS_PER_WEEK))
                                                {
                                                    if (Litimetbltemplate != null && Litimetbltemplate.Count > 0)
                                                    {
                                                        foreach (var day in Dayordergroup)
                                                        {
                                                            var templatelist = Litimetbltemplate.Where(o => o.Day_Order_Id == day.FirstOrDefault().Day_Order_Id).ToList();
                                                            foreach (var temp in templatelist)
                                                            {

                                                                var IsExist = TemplatePossibility(temp.Hour_Id, day.FirstOrDefault().Day_Order_Id, cls.CLASS_ID, course.COURSE_ID, course.STAFF_ID);
                                                                if (IsExist == null)
                                                                {
                                                                    //insert only course
                                                                    var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Course_Id = course.COURSE_ID, Hour_Id = temp.Hour_Id, Day_Order_Id = temp.Day_Order_Id, Class_Id = course.CLASS_ID, Semester_Id = course.SEMESTER_ID, ACADEMIC_YEAR = course.ACADEMIC_YEAR }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveTimeTable), sAcademicyear);
                                                                }
                                                                Litimetbltemplate.RemoveAll(o => o.TT_Temp_Id == temp.TT_Temp_Id);
                                                                continue;

                                                            }
                                                            coursehr_week++;

                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                coursehr_week = 0;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimetableGeneration", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimetableGeneration", "TimeTable", ex.Message);
                    }
                }
            }
            else
            {

            }
            return SQL;
        }
        public List<TimeTableTemplate> HourPossibility(string sStaffId, string sPaptertype, string sCourseId, string sFaculty)
        {
            string sSQL = string.Empty;
            List<TimeTableTemplate> ListPossibilityhours = new List<TimeTableTemplate>();
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear) && !string.IsNullOrEmpty(sActiveSemesterId))
            {
                try
                {
                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchPossibilityHoursForStaff).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    ListPossibilityhours = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sStaffId, FACULTY = sFaculty }, sSQL, sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HourPossibility", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HourPossibility", "TimeTable", ex.Message);
                    }
                }
            }
            else
            {

            }
            return ListPossibilityhours;
        }
        public List<TimeTable> TemplatePossibility(string sHourid, string sDayorderId, string sClassId, string sCourseId, string sStaffId)
        {
            string sSQL = string.Empty;
            List<TimeTable> IsExist = new List<TimeTable>();
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear) && !string.IsNullOrEmpty(sActiveSemesterId))
            {
                try
                {
                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCheckIsTimetableExists).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    IsExist = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(new CP_CLASS_COURSE_STAFF() { COURSE_ID = sCourseId, CLASS_ID = sClassId, HOUR = sHourid, DAY = sDayorderId }, sSQL, sAcademicyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HourPossibility", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HourPossibility", "TimeTable", ex.Message);
                    }
                }
            }
            else
            {

            }
            return IsExist;
        }
        #endregion

        #region Timetable Generation Methods
        public string TimetableMethod()
        {
            string SQL = string.Empty;
            string sOPtionClass = string.Empty;
            string Dayorder = string.Empty;
            string Hour = string.Empty;
            int coursehrs = 0;
            int count = 0;
            Random rdm = new Random();
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            List<CP_COURSE_PATTERN> LicoursePattern = new List<CP_COURSE_PATTERN>();
            List<TimeTableTemplate> LiTimetabletemplate = new List<TimeTableTemplate>();
            List<CP_CLASS_COURSE_STAFF> ClassCoursesStaff = new List<CP_CLASS_COURSE_STAFF>();
            List<CP_CLASS_COURSE_STAFF> CourseWisestaff = new List<CP_CLASS_COURSE_STAFF>();
            List<CP_CLASS_COURSE_STAFF> NMECourseWisestaff = new List<CP_CLASS_COURSE_STAFF>();
            List<TimeTableTemplate> linmetemplates = new List<TimeTableTemplate>();
            List<TimeTableTemplate> matchtemplate = new List<TimeTableTemplate>();
            List<TimeTable> Litimetable = new List<TimeTable>();
            List<TimeTableTemplate> Listaticresult = new List<TimeTableTemplate>();
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    SQL = string.Empty;
                    SQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOverAllCoursePattern).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    //Fetch Course Pattern
                    LicoursePattern = (List<CP_COURSE_PATTERN>)CMSHandler.FetchData<CP_COURSE_PATTERN>(null, SQL, sAcademicyear).DataSource.Data;
                    //Fetch Paper type
                    var Listpapertype = (List<SupPaperType>)CMSHandler.FetchData<SupPaperType>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSupPaparType)).DataSource.Data;
                    //Fetch Timetable
                    var Lioveralltimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOverAlltimetable), sAcademicyear).DataSource.Data;
                    //Fetch  Classwise Course with staff
                    SQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffHourAllotment).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    ClassCoursesStaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(null, SQL, sAcademicyear).DataSource.Data;

                    //Fetch Overall Template 
                    LiTimetabletemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchProgammeTimetabletemplate), sAcademicyear).DataSource.Data;

                    //Group by Programme
                    var Listprogrammegrouped = LicoursePattern.GroupBy(o => o.PROGRAMME_GROUP_ID).ToList();
                    int pCount = 0;
                    foreach (var programme in Listprogrammegrouped)
                    {
                        string programmeid = programme.FirstOrDefault().PROGRAMME_GROUP_ID;

                        pCount++;
                        //Fetch Programme Classess
                        var LiClassess = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES() { PROGRAMME_GROUP_ID = programme.FirstOrDefault().PROGRAMME_GROUP_ID }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClassessByProgramme), sAcademicyear).DataSource.Data;
                        var RegularClassess = LiClassess.Where(x => x.CLASS_TYPE == Common.CLASS_TYPE.R_Regular);
                        var groupedclass = RegularClassess.GroupBy(o => o.CLASS_YEAR);
                        string lFaculty = string.Empty;

                        foreach (var classyear in groupedclass)
                        {
                            var classess = RegularClassess.Where(o => o.CLASS_YEAR == classyear.FirstOrDefault().CLASS_YEAR);
                            foreach (var cls in classess)
                            {


                                var classwisecourses = LicoursePattern.Where(o => o.PROGRAMME_YEAR == cls.CLASS_YEAR && o.PROGRAMME_LEVEL == cls.CLASS_LEVEL && o.PROGRAMME_GROUP_ID == programme.FirstOrDefault().PROGRAMME_GROUP_ID && o.IS_ACTIVE != "0").ToList();
                                var liFaculty = LicoursePattern.Where(x => x.PROGRAMME_GROUP_ID == programme.FirstOrDefault().PROGRAMME_GROUP_ID && x.PROGRAMME_YEAR.Equals(cls.CLASS_YEAR) && x.PROGRAMME_LEVEL == cls.CLASS_LEVEL).ToList();
                                if (liFaculty != null && liFaculty.Count > 0)
                                    lFaculty = liFaculty.FirstOrDefault().FACULTY_ID;
                                else
                                    continue;

                                var groupedcoursestructure = classwisecourses.GroupBy(o => o.COURSE_STRUCTURE_ID).ToList();
                                SQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClasswisetemplatewithtimetable).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                                //Class template by class id and faculty id 
                                var classwisetemplatewithtimetbl = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(new TimeTableTemplate() { CLASS_YEAR = cls.CLASS_YEAR, CLASS_ID = cls.CLASS_ID, FACULTY = lFaculty, GraduationType = cls.CLASS_LEVEL, SHIFT = cls.SHIFT }, SQL, sAcademicyear).DataSource.Data;
                                if (classwisetemplatewithtimetbl != null)
                                {
                                    if (groupedcoursestructure != null && groupedcoursestructure.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (var coursestructure in groupedcoursestructure)
                                            {
                                                var coureses = classwisecourses.Where(o => o.COURSE_STRUCTURE_ID == coursestructure.FirstOrDefault().COURSE_STRUCTURE_ID).ToList();

                                                foreach (var item in coureses)
                                                {
                                                    var temp = classwisetemplatewithtimetbl.Where(o => o.COURSE_ID.Equals(item.COURSE_ID)).ToList();
                                                    //var i = classwisetemplatewithtimetbl.Where(o => o.COURSE_ID == "807");
                                                    if (temp != null && temp.Count > 0)
                                                    {
                                                        count++;
                                                    }

                                                }
                                                if (count > 0)
                                                {
                                                    count = 0;
                                                    continue;
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        int j = 0;
                                                        int hrscount = 0;
                                                        List<TimeTable> insertlist = new List<TimeTable>();
                                                        Litimetable.Clear();
                                                        //while (j < Convert.ToInt32(coureses.FirstOrDefault().HRS_PER_WEEK))
                                                        //{
                                                        var Listatictemplate = classwisetemplatewithtimetbl.Where(o => o.Paper_Type_Id == coureses.FirstOrDefault().PAPER_TYPE && o.Is_Static == "1").ToList();
                                                        if (Listatictemplate != null && Listatictemplate.Count > 0)
                                                        {
                                                            string sSQL = string.Empty;
                                                            sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClassCourseStaff).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);


                                                            foreach (var course in coureses)
                                                            {
                                                                var coursewisestaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new CP_CLASS_COURSE_STAFF() { COURSE_ID = course.COURSE_ID, CLASS_ID = cls.CLASS_ID }, sSQL, sAcademicyear).DataSource.Data;
                                                                if (coursewisestaff != null && coursewisestaff.Count > 0)
                                                                {
                                                                    int totalhrs = coursewisestaff.Sum(o => Convert.ToInt32(o.HRS_WEEK));

                                                                    int startpos = 0;
                                                                    foreach (var staff in coursewisestaff)
                                                                    {
                                                                        var list = Listatictemplate.Skip(startpos).Take(Convert.ToInt32(!string.IsNullOrEmpty(staff.HRS_WEEK) ? staff.HRS_WEEK : "0"));
                                                                        if (list != null)
                                                                        {
                                                                            foreach (var item in list)
                                                                            {
                                                                                Litimetable.Add(new TimeTable { Staff_Id = staff.STAFF_ID, Course_Id = course.COURSE_ID, Day_Order_Id = item.Day_Order_Id, Hour_Id = item.Hour_Id, Class_Id = cls.CLASS_ID, Semester_Id = course.SEMESTER_ID });
                                                                                //Litimetable = Litimetable.Concat(Litimetable).ToList();
                                                                                startpos++;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            startpos = 0;
                                                                        }

                                                                        if (startpos == Listatictemplate.Count)
                                                                        {
                                                                            startpos = 0;
                                                                        }

                                                                    }
                                                                    Savetimetable(Litimetable);


                                                                    Litimetable.Clear();

                                                                }
                                                                else
                                                                {

                                                                    if (Listatictemplate != null && Listatictemplate.Count > 0)
                                                                    {
                                                                        var temp = Listatictemplate;
                                                                        while (coursehrs < Convert.ToInt32(course.HRS_PER_WEEK))
                                                                        {
                                                                            if (temp != null && temp.Count > 0)
                                                                            {
                                                                                foreach (var item in temp)
                                                                                {

                                                                                    Litimetable.Add(new TimeTable { Course_Id = course.COURSE_ID, Day_Order_Id = item.Day_Order_Id, Hour_Id = item.Hour_Id, Class_Id = cls.CLASS_ID, Semester_Id = course.SEMESTER_ID });
                                                                                    temp.RemoveAll(o => o.TT_Temp_Id == item.TT_Temp_Id);
                                                                                    coursehrs++;
                                                                                    break;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                break;
                                                                            }
                                                                        }
                                                                        Savetimetable(Litimetable);
                                                                        Litimetable.Clear();
                                                                        coursehrs = 0;
                                                                    }
                                                                    else
                                                                    {
                                                                        //Maintain no space in tt_template
                                                                    }
                                                                }
                                                            }


                                                        }
                                                        else
                                                        {
                                                            //Non Static course excution
                                                            //var Linonstatictemplate = classwisetemplatewithtimetbl.Where(o => o.Is_Static != "1" && string.IsNullOrEmpty(o.Time_Table_Id)).ToList();
                                                            var tmp = FetchClasswisetemplatewithtimetable(cls.CLASS_ID, lFaculty);
                                                            if (tmp != null && tmp.Count > 0)
                                                            {
                                                                var Linonstatictemplate = tmp.Where(o => o.Is_Static != "1" && string.IsNullOrEmpty(o.Time_Table_Id)).ToList();
                                                                if (Linonstatictemplate != null && Linonstatictemplate.Count > 0)
                                                                {

                                                                    string sSQL = string.Empty;
                                                                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClassCourseStaff).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);

                                                                    foreach (var course in coureses)
                                                                    {

                                                                        var coursewisestaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new CP_CLASS_COURSE_STAFF() { COURSE_ID = course.COURSE_ID, CLASS_ID = cls.CLASS_ID }, sSQL, sAcademicyear).DataSource.Data;
                                                                        if (coursewisestaff != null && coursewisestaff.Count > 0)
                                                                        {
                                                                            int totalhrs = coursewisestaff.Sum(o => Convert.ToInt32(o.HRS_WEEK));


                                                                            try
                                                                            {
                                                                                foreach (var staff in coursewisestaff)
                                                                                {
                                                                                    //int liscount = 0;
                                                                                    var StaffPosssiblityHours = StaffHourPossibility(staff.STAFF_ID, lFaculty);
                                                                                    var result = Linonstatictemplate.Where(x => (StaffPosssiblityHours.Any(o => o.Hour_Id == x.Hour_Id && o.Day_Order_Id == x.Day_Order_Id))).ToList();

                                                                                    var daywisegroup = result.GroupBy(x => x.Day_Order_Id).ToList();
                                                                                    //while (liscount < result.Count)
                                                                                    //{
                                                                                    //    foreach (var item in daywisegroup)
                                                                                    //    {
                                                                                    //        Listaticresult.Add(new TimeTableTemplate() { STAFF_ID = item.FirstOrDefault().STAFF_ID, COURSE_ID = item.FirstOrDefault().COURSE_ID, Hour_Id = item.FirstOrDefault().Hour_Id, Day_Order_Id = item.FirstOrDefault().Day_Order_Id, CLASS_ID = item.FirstOrDefault().CLASS_ID, SEMESTER_ID = item.FirstOrDefault().SEMESTER_ID });
                                                                                    //        liscount++;
                                                                                    //    }
                                                                                    //}
                                                                                    for (int k = 0; k < Convert.ToInt32(staff.HRS_WEEK); k++)
                                                                                    {

                                                                                        //if (Listaticresult != null && Listaticresult.Count > 0)
                                                                                        //{
                                                                                        //    foreach (var item in Listaticresult)
                                                                                        //    {


                                                                                        //        insertlist.Add(new TimeTable() { Staff_Id = staff.STAFF_ID, Course_Id = course.COURSE_ID, Day_Order_Id = item.Day_Order_Id, Hour_Id = item.Hour_Id, Class_Id = cls.CLASS_ID, Semester_Id = course.SEMESTER_ID });
                                                                                        //        Listaticresult.RemoveAll(o => o.TT_Temp_Id == result.FirstOrDefault().TT_Temp_Id);
                                                                                        //        Savetimetable(insertlist);
                                                                                        //        Litimetable = Litimetable.Concat(insertlist).ToList();
                                                                                        //        insertlist.Clear();

                                                                                        //        continue;
                                                                                        //    }
                                                                                        //}
                                                                                        //else
                                                                                        //{

                                                                                        //    var Liresult = GetAllotedHours(StaffHourPossibility(staff.STAFF_ID, lFaculty), FetchClasswisetemplatewithtimetable(cls.CLASS_ID, lFaculty).Where(o => o.Is_Static != "1").ToList(), staff.STAFF_ID, staff.HRS_WEEK, course.COURSE_ID, cls.CLASS_ID, course.SEMESTER_ID, lFaculty);
                                                                                        //    if (Liresult != null && Liresult.Count > 0)
                                                                                        //    {
                                                                                        //        Savetimetable(Liresult);
                                                                                        //        Litimetable.Concat(Liresult);
                                                                                        //    }

                                                                                        //}

                                                                                        if (result != null && result.Count > 0)
                                                                                        {
                                                                                            var randomvalue = result[rdm.Next(result.Count)];
                                                                                            insertlist.Add(new TimeTable() { Staff_Id = staff.STAFF_ID, Course_Id = course.COURSE_ID, Day_Order_Id = randomvalue.Day_Order_Id, Hour_Id = randomvalue.Hour_Id, Class_Id = cls.CLASS_ID, Semester_Id = course.SEMESTER_ID });
                                                                                            result.RemoveAll(o => o.TT_Temp_Id == randomvalue.TT_Temp_Id);
                                                                                            Savetimetable(insertlist);
                                                                                            Litimetable = Litimetable.Concat(insertlist).ToList();
                                                                                            insertlist.Clear();

                                                                                        }

                                                                                        else
                                                                                        {

                                                                                            var Liresult = GetAllotedHours(StaffHourPossibility(staff.STAFF_ID, lFaculty), FetchClasswisetemplatewithtimetable(cls.CLASS_ID, lFaculty).Where(o => o.Is_Static != "1").ToList(), staff.STAFF_ID, staff.HRS_WEEK, course.COURSE_ID, cls.CLASS_ID, course.SEMESTER_ID, lFaculty);
                                                                                            if (Liresult != null && Liresult.Count > 0)
                                                                                            {
                                                                                                Savetimetable(Liresult);
                                                                                                Litimetable.Concat(Liresult);
                                                                                            }

                                                                                        }


                                                                                    }
                                                                                }
                                                                            }
                                                                            catch (Exception ex)
                                                                            {


                                                                            }
                                                                        }
                                                                        try
                                                                        {
                                                                            if (Litimetable == null || Litimetable.Count <= 0)
                                                                            {
                                                                                var temp = FetchClasswisetemplatewithtimetable(cls.CLASS_ID, lFaculty);
                                                                                var liavilabletmp = temp.Where(o => o.Is_Static != "1" && string.IsNullOrEmpty(o.Time_Table_Id)).ToList();
                                                                                if (liavilabletmp != null && liavilabletmp.Count > 0)
                                                                                {
                                                                                    while (coursehrs < Convert.ToInt32(course.HRS_PER_WEEK))
                                                                                    {
                                                                                        if (liavilabletmp != null && liavilabletmp.Count > 0)
                                                                                        {
                                                                                            foreach (var item in liavilabletmp)
                                                                                            {
                                                                                                var randomvalue = liavilabletmp[rdm.Next(liavilabletmp.Count)];
                                                                                                Litimetable.Add(new TimeTable { Course_Id = course.COURSE_ID, Day_Order_Id = randomvalue.Day_Order_Id, Hour_Id = randomvalue.Hour_Id, Class_Id = cls.CLASS_ID, Semester_Id = course.SEMESTER_ID });
                                                                                                liavilabletmp.RemoveAll(o => o.TT_Temp_Id == item.TT_Temp_Id);
                                                                                                coursehrs++;
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                    coursehrs = 0;
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Maintain no space in tt_template
                                                                                }
                                                                                Savetimetable(Litimetable);
                                                                                Litimetable.Clear();

                                                                            }
                                                                            else
                                                                            {
                                                                                Litimetable.Clear();
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {


                                                                        }

                                                                    }

                                                                }
                                                            }

                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {


                                                    }


                                                    //}

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {


                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimetableMethod", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimetableMethod", "TimeTable", ex.Message);
                    }
                }
            }


            return SQL;
        }
        public bool Savetimetable(List<TimeTable> timetable)
        {
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            ResultArgs Result = new ResultArgs();
            string sSQL = string.Empty;
            try
            {
                if (timetable != null && timetable.Count > 0)
                {
                    sSQL = @"INSERT INTO TT_TIMETABLE
                                            (
                                            Course_Id,
                                            Day_Order_Id,
                                            Staff_Id,
                                            Class_Id,
                                            Hour_Id,
                                            Semester_Id,                     
                                            ACADEMIC_YEAR) Values";
                    foreach (var item in timetable)
                    {
                        item.Staff_Id = !string.IsNullOrEmpty(item.Staff_Id) ? item.Staff_Id : "0";
                        sSQL += "(" + item.Course_Id + "," + item.Day_Order_Id + "," + item.Staff_Id + "," + item.Class_Id + "," + item.Hour_Id + "," + item.Semester_Id + ",'" + sAcademicyear + "'),";

                    }
                    sSQL = sSQL.TrimEnd(',') + ";";

                    Result = CMSHandler.SaveOrUpdate(null, sSQL);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("Savetimetable", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("Savetimetable", "TimeTable", ex.Message);
                }
            }
            return Result.Success;
        }
        public List<TimeTableTemplate> StaffHourPossibility(string sStaffId, string sFaculty)
        {
            string sSQL = string.Empty;
            List<TimeTableTemplate> ListPossibilityhours = new List<TimeTableTemplate>();
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear) && !string.IsNullOrEmpty(sActiveSemesterId))
            {
                try
                {
                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchPossibilityHoursForStaff).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    ListPossibilityhours = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sStaffId, FACULTY = sFaculty }, sSQL, sAcademicyear).DataSource.Data;
                    sSQL = string.Empty;
                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffFixedhours).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    var LiStafffixedhours = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sStaffId }, sSQL, sAcademicyear).DataSource.Data;
                    if (LiStafffixedhours != null && LiStafffixedhours.Count > 0)
                    {
                        ListPossibilityhours.RemoveAll(o => (LiStafffixedhours.Any(x => x.CLASS_ID == o.CLASS_ID && x.COURSE_ID == o.COURSE_ID && x.STAFF_ID == o.STAFF_ID)));
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HourPossibility", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HourPossibility", "TimeTable", ex.Message);
                    }
                }
            }
            else
            {

            }
            return ListPossibilityhours;
        }
        public List<TimeTableTemplate> FetchClasswisetemplatewithtimetable(string sClassId, string sFaculty)
        {
            List<TimeTableTemplate> Liclasswisetemplate = new List<TimeTableTemplate>();
            string sSQL = string.Empty;
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sClassId))
            {
                try
                {
                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClasswisetemplate).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    Liclasswisetemplate = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(new TimeTableTemplate() { CLASS_ID = sClassId, Faculty_Id = sFaculty }, sSQL, sAcademicyear).DataSource.Data;

                    sSQL = string.Empty;
                    sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClasswisefixedhours).Replace(Common.Delimiter.QUS + Common.SEMESTER, sActiveSemesterId);
                    var liclasswisefixedhours = (List<TimeTableTemplate>)CMSHandler.FetchData<TimeTableTemplate>(new TimeTableTemplate() { CLASS_ID = sClassId }, sSQL, sAcademicyear).DataSource.Data;
                    if (liclasswisefixedhours != null && liclasswisefixedhours.Count > 0)
                    {
                        Liclasswisetemplate.RemoveAll(o => (liclasswisefixedhours.Any(x => x.CLASS_ID == o.CLASS_ID && x.COURSE_ID == o.COURSE_ID && x.STAFF_ID == o.STAFF_ID)));

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchClasswisetemplatewithtimetable", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchClasswisetemplatewithtimetable", "TimeTable", ex.Message);
                    }
                }
            }
            return Liclasswisetemplate;
        }
        public List<TimeTable> GetAllotedHours(List<TimeTableTemplate> sStaffHourPossible, List<TimeTableTemplate> template, string sStaffId, string sHour, string sCourseId, string sClassId, string sSemId, string sFaculty)
        {
            string sSQL = string.Empty;
            Random rdm = new Random();
            List<TimeTable> Litimetable = new List<TimeTable>();
            List<TimeTableTemplate> List = new List<TimeTableTemplate>();
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sActiveSemesterId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicyear) && !string.IsNullOrEmpty(sActiveSemesterId))
            {
                try
                {
                    if (sStaffHourPossible != null && sStaffHourPossible.Count > 0 && template != null && template.Count > 0)
                    {
                        var listaffpossible = sStaffHourPossible.Where(o => string.IsNullOrEmpty(o.Time_Table_Id)).ToList();
                        var liavilabletemplate = template.Where(o => string.IsNullOrEmpty(o.Time_Table_Id)).ToList();
                        var matchedtemplate = liavilabletemplate.Where(x => (listaffpossible.Any(o => o.Hour_Id == x.Hour_Id && o.Day_Order_Id == x.Day_Order_Id))).ToList();
                        // Need to change this logic it is wrong
                        //foreach (var item in liavilabletemplate)
                        //{
                        //    var staffAlloated = sStaffHourPossible.Where(x => x.Day_Order_Id == item.Day_Order_Id && x.Hour_Id == item.Hour_Id).ToList();
                        //    if (staffAlloated == null || staffAlloated.Count<=0)
                        //        continue;
                        //    var Staffallotedclstemplate = FetchClasswisetemplatewithtimetable(sClassId);

                        //    var templist = Staffallotedclstemplate.Where(o => (sStaffHourPossible.Any(x => x.Hour_Id == o.Hour_Id && o.Day_Order_Id == x.Day_Order_Id))).ToList();
                        //    if (templist != null && templist.Count > 0)
                        //    {
                        //        foreach (var tem in templist)
                        //        {
                        //            var result = CMSHandler.SaveOrUpdate(new TimeTable() { Hour_Id = tem.Hour_Id, Day_Order_Id = tem.Day_Order_Id, Course_Id = staffAlloated.FirstOrDefault().COURSE_ID, Staff_Id =sStaffId, Time_Table_Id = staffAlloated.FirstOrDefault().Time_Table_Id, Semester_Id = staffAlloated.FirstOrDefault().SEMESTER_ID }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimetable));

                        //            if (result.Success)
                        //            {
                        //                // assign staffalloted day,hours
                        //                Litimetable.Add(new TimeTable() { Staff_Id = sStaffId, Hour_Id = item.Hour_Id, Day_Order_Id = item.Day_Order_Id, Course_Id = sCourseId, Class_Id = sClassId, Semester_Id = sSemId });
                        //                return Litimetable;
                        //            }
                        //        }
                        //    }
                        //}
                        if (Litimetable == null || Litimetable.Count <= 0)
                        {
                            if (listaffpossible != null && listaffpossible.Count > 0)
                            {
                                foreach (var item in sStaffHourPossible)
                                {
                                    var staffalloted = template.Where(o => o.Hour_Id == item.Hour_Id && o.Day_Order_Id == item.Day_Order_Id).ToList();
                                    if (staffalloted == null || staffalloted.Count <= 0)
                                        continue;
                                    var staffallotedclstemplate = FetchClasswisetemplatewithtimetable(staffalloted.FirstOrDefault().CLASS_ID, sFaculty).Where(x => x.Is_Static != "1" && string.IsNullOrEmpty(x.Time_Table_Id)).ToList();
                                    var staffallotedpossibility = StaffHourPossibility(staffalloted.FirstOrDefault().STAFF_ID, staffalloted.FirstOrDefault().Faculty_Id);
                                    if (staffallotedpossibility != null && staffallotedpossibility.Count > 0)
                                    {
                                        var templist = staffallotedclstemplate.Where(o => (staffallotedpossibility.Any(x => x.Hour_Id == o.Hour_Id && o.Day_Order_Id == x.Day_Order_Id))).ToList();

                                        if (templist != null && templist.Count > 0)
                                        {
                                            foreach (var temp in templist)
                                            {
                                                var result = CMSHandler.SaveOrUpdate(new TimeTable() { Hour_Id = temp.Hour_Id, Day_Order_Id = temp.Day_Order_Id, Course_Id = staffalloted.FirstOrDefault().COURSE_ID, Staff_Id = staffalloted.FirstOrDefault().STAFF_ID, Time_Table_Id = staffalloted.FirstOrDefault().Time_Table_Id, Semester_Id = staffalloted.FirstOrDefault().SEMESTER_ID, Class_Id = staffalloted.FirstOrDefault().CLASS_ID }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimetable));

                                                if (result.Success)
                                                {
                                                    Litimetable.Add(new TimeTable() { Staff_Id = sStaffId, Hour_Id = item.Hour_Id, Day_Order_Id = item.Day_Order_Id, Course_Id = sCourseId, Class_Id = sClassId, Semester_Id = sSemId });
                                                    return Litimetable;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("HourPossibility", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("HourPossibility", "TimeTable", ex.Message);
                    }
                }
            }
            else
            {

            }
            return Litimetable;
        }
        public string TimetableSwappingMethod()
        {
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string faculty = string.Empty;
            string sSQL = string.Empty;
            try
            {
                //Fetch Timetable
                var Lioveralltimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOverAlltimetable), sAcademicyear).DataSource.Data;

                var Staffwisetimetablegrouped = Lioveralltimetable.Where(x => x.Staff_Id != "0").GroupBy(x => x.Staff_Id).ToList();
                foreach (var item in Staffwisetimetablegrouped)
                {
                    var Litimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOverAlltimetable), sAcademicyear).DataSource.Data;
                    var Staffwisetimetable = Litimetable.Where(o => o.Staff_Id == item.FirstOrDefault().Staff_Id);
                    var Daywisegrouped = Staffwisetimetable.Where(o => o.Staff_Id == item.FirstOrDefault().Staff_Id).GroupBy(x => x.Day_Order_Id).ToList();

                    foreach (var timetable in Daywisegrouped)
                    {
                        var dayorderwise = Staffwisetimetable.Where(o => o.Day_Order_Id == timetable.FirstOrDefault().Day_Order_Id).OrderBy(o => o.Hour_Id);
                        int current = 0;
                        int previous = 0;
                        int count = 0;
                        foreach (var day in dayorderwise)
                        {
                            current = Convert.ToInt32(day.Hour_Id);

                            if (current - previous == 1)
                            {
                                count++;

                            }
                            if (count > 2)
                            {
                                var clswisetemplatewithtimetable = FetchClasswisetemplatewithtimetable(day.Class_Id, faculty);

                                if (clswisetemplatewithtimetable != null && clswisetemplatewithtimetable.Count > 0)
                                {
                                    var finalclswisetemplate = clswisetemplatewithtimetable.Where(o => o.Is_Static != "1" && o.STAFF_ID == item.FirstOrDefault().Staff_Id).ToList();
                                    if (finalclswisetemplate != null && finalclswisetemplate.Count > 0)
                                    {
                                        finalclswisetemplate.RemoveAll(o => o.Day_Order_Id == day.Day_Order_Id);
                                        var Listaffpossibility = StaffHourPossibility(item.FirstOrDefault().Staff_Id, "1");
                                        var matchedtemplate = finalclswisetemplate.Where(o => (Listaffpossibility.Any(x => x.Hour_Id == o.Hour_Id && o.Day_Order_Id == x.Day_Order_Id))).ToList();
                                        if (matchedtemplate != null && matchedtemplate.Count > 0)
                                        {
                                            foreach (var swaptemplate in matchedtemplate)
                                            {
                                                //update
                                                var Result = CMSHandler.SaveOrUpdate(new TimeTable() { Hour_Id = swaptemplate.Hour_Id, Day_Order_Id = swaptemplate.Day_Order_Id, Time_Table_Id = day.Time_Table_Id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimetable));
                                                if (Result.Success)
                                                {
                                                    var result = CMSHandler.SaveOrUpdate(new TimeTable() { Hour_Id = day.Hour_Id, Day_Order_Id = day.Day_Order_Id, Time_Table_Id = swaptemplate.Time_Table_Id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateTimetable));
                                                }
                                            }
                                        }
                                    }

                                }


                                count = 0;

                            }
                            previous = Convert.ToInt32(day.Hour_Id);

                        }



                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimetableSwappingMethod", "TimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("TimetableSwappingMethod", "TimeTable", ex.Message);
                }

            }
            return sSQL;

        }
        #endregion

        #region Staff Availability 
        public ActionResult HourBasesStaffAvailabillity()
        {
            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = "5" }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicyear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = "6" }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicyear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                    {
                        ObjViewModel.HourList = new SelectList(FetchHours, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    }
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                    {
                        ObjViewModel.DayOrderList = new SelectList(FetchDayOrders, Common.SUP_DAY_ORDERS.DAY_ORDER_ID, Common.SUP_DAY_ORDERS.DAY);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("TimeTableController", "HourBasesStaffAvailabillity", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        public ActionResult BindStaffAvailabillity(string dayorder, string hour, string Shift)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            TimeTableViewModel Objmodel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Obj = new TimeTable();
                    Obj.Hour_Id = hour;
                    Obj.Day_Order_Id = dayorder;
                    Obj.SHIFT = Shift;
                    var Listaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Obj, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourBasedStaffAvailabillty), sAcademicYear).DataSource.Data;
                    if (Listaff != null && Listaff.Count > 0)
                    {
                        Objmodel.lstTimeTable = Listaff;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("TimeTableController", "BindStaffAvailabillity", ex.Message);
                    }
                }
            }
            return View(Objmodel);
        }
        #endregion

        #region ClassTimeTable and StaffTimeTable for Pricipal
        public ActionResult TimeTableViewForPrincipal()
        {

            string sAcademicyear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                try
                {
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicyear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                    {
                        ObjViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var StaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicyear).DataSource.Data;
                    if (StaffList != null && StaffList.Count > 0)
                    {
                        ObjViewModel.StaffList = new SelectList(StaffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog Objlog = new ErrorLog())
                    {
                        Objlog.WriteError("TimeTableController", "TimeTableViewForPrincipal", ex.Message);
                    }
                }
            }
            return View(ObjViewModel);
        }
        public ActionResult BindStaffWiseTimeTable(string sStaffId)
        {
            string sAcademicyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            stf_Personal_Info objStfModel = new stf_Personal_Info();
            if (!string.IsNullOrEmpty(sAcademicyear))
            {
                objStfModel.STAFF_ID = sStaffId;
                var FetchHourAndDays = (List<TimeTableSetting>)CMSHandler.FetchData<TimeTableSetting>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId), sAcademicyear).DataSource.Data;
                if (FetchHourAndDays != null && FetchHourAndDays.Count > 0)
                {
                    var FetchHours = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new TimeTableSetting() { NO_OF_HOURS = FetchHourAndDays.FirstOrDefault().NO_OF_HOURS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursBySettingHours), sAcademicyear).DataSource.Data;
                    var FetchDayOrders = (List<SUP_DAY_ORDER>)CMSHandler.FetchData<SUP_DAY_ORDER>(new TimeTableSetting() { NO_OF_DAYS = FetchHourAndDays.FirstOrDefault().NO_OF_DAYS }, SQL.TimeTable.TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDaysBySettingDays), sAcademicyear).DataSource.Data;
                    if (FetchHours != null && FetchHours.Count > 0)
                        objViewModel.lstHours = FetchHours;
                    if (FetchDayOrders != null && FetchDayOrders.Count > 0)
                        objViewModel.lstDayOrders = FetchDayOrders;
                }
                objViewModel.liTimetable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objStfModel, DashboardSQL.GetDashboardSQL(DashboardSQLCommands.FetchTimetableForStaff), sAcademicyear).DataSource.Data;

                objViewModel.liSubstituteList = (List<ATT_SUBSTITUTE>)CMSHandler.FetchData<ATT_SUBSTITUTE>(objStfModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubstituteHourByStaffId), sAcademicyear).DataSource.Data;
            }
            else
                return RedirectToAction("Session Expired Login again..!", "error");

            return View(objViewModel);
        }
        #endregion

        #region Online TimeTable Entry
        public ActionResult OnlineTimeTableEntry()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            TimeTableViewModel ObjViewModel = new TimeTableViewModel();
            var ObjModel = new STF_PERSONAL_INFO();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjModel.STAFF_ID = sUserId;

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjModel.STAFF_ID = liStaff.FirstOrDefault().STAFF_ID;
                        ObjModel.DEPARTMENT = liStaff.FirstOrDefault().DEPARTMENT;

                        string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.SHIFT, liStaff.FirstOrDefault().SHIFT);
                        var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchClass != null && FetchClass.Count > 0)
                        {
                            ObjViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("TimeTableController", "TimeTableEntryForSJC", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindOnlineTimeTable(string sClassId)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                    TimeTable objModel = new TimeTable();
                    var BindStudentCourseInfo = new List<TimeTable>();
                    objModel.Class_Id = sClassId;
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

                    string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOnlineTimeTableClassWiseListByClassId);
                    sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Semester_Id.ToUpper(), sActiveSemester);
                    BindStudentCourseInfo = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                    objViewModel.lstTimeTable = BindStudentCourseInfo;

                    string ssql = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchValueEducationPapersForTimeTable);
                    var liFetchEthics = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, ssql, sAcademicYear).DataSource.Data;
                    if (liFetchEthics != null && liFetchEthics.Count > 0)
                    {
                        objViewModel.lstTimeTable = (objViewModel.lstTimeTable != null) ? objViewModel.lstTimeTable.Concat(liFetchEthics).ToList() : liFetchEthics;
                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("BindOnlineTimeTable", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult EditOnlineTimeTable(string sTimetable)
        {
            string sOption = string.Empty;
            JsonResultData objResult = new JsonResultData();
            TimeTable objModel = new TimeTable();
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                string sHourType = string.Empty;
                string sCourse = string.Empty;
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    var Result = JsonConvert.DeserializeObject<TimeTable>(sTimetable);
                    if (!string.IsNullOrEmpty(Result.Count) && Result.Count != "undefined" && Result.Count != "0")
                    {
                        // If More Then One Record Available ....
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMultipleOnlineTimeTableByTimeTableId).
                            Replace(Common.Delimiter.QUS + Common.TT_TIMETABLE.Time_Table_Id.ToUpper(), Result.Time_Table_Id);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        var liTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(null, sSQL, sAcademicYear).DataSource.Data;
                        if (liTimeTable != null && liTimeTable.Count > 0)
                        {
                            objViewModel.lstTimeTable = liTimeTable;
                            return View(objViewModel);
                        }
                    }
                    else
                    {
                        // Only One Record Available ...

                        // HourType ....
                        var HourType = (List<SUP_HOUR_TYPE>)CMSHandler.FetchData<SUP_HOUR_TYPE>(null,
                               SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchHourType)).DataSource.Data;
                        if (HourType != null && HourType.Count > 0)
                        {
                            foreach (var DataItem in HourType)
                                sHourType += "<option value='" + DataItem.hour_type_id + "'>" + DataItem.hourType + "</option>";
                        }

                        var liCourse = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new CP_CLASSES() { CLASS_ID = Result.Class_Id },
                                   TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCourseByClassAndAcademicYear), sAcademicYear).DataSource.Data;
                        if (liCourse != null && liCourse.Count > 0)
                        {
                            foreach (var DataItem in liCourse)
                                sCourse += "<option value=" + DataItem.COURSE_ROOT_ID + " >" + DataItem.COURSE_TITLE + "</option>";
                        }

                        // Staff List .....
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchStaffByClassId);
                        var FetchStaff = (List<stf_Personal_Info>)CMSHandler.FetchData<stf_Personal_Info>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchStaff != null && FetchStaff.Count > 0)
                        {
                            foreach (var item in FetchStaff)
                                sOption += "<option value='" + item.STAFF_ID + "'>" + item.FIRST_NAME + "</option>";
                        }

                        objModel.hourType = sHourType;
                        objModel.sCourseId = sCourse;
                        objModel.sStaffId = sOption;
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
                    objHandler.WriteError("EditOnlineTimeTable", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("EditOnlineTimeTable", "TimeTableController", ex.Message);
                }
            }
            return Json(objModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchOnlineTimeTableById(string sTimeTableId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    TimeTable ObjModal = new TimeTable();
                    var liTimeTable = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(new TimeTable() { Time_Table_Id = sTimeTableId }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchOnlineTimeTableById), sAcademicYear).DataSource.Data;
                    if (liTimeTable != null && liTimeTable.Count > 0)
                    {
                        return Json(liTimeTable, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("FetchOnlineTimeTableById", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("FetchOnlineTimeTableById", "TimeTableController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOnlineTimeTableMultiple(string sTimeTable)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Result = JsonConvert.DeserializeObject<TimeTable>(sTimeTable);
                    Result.COURSE_ROOT_ID = Result.Course_Id;
                    var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(Result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                    if (liCourseRoot != null && liCourseRoot.Count > 0)
                    {
                        Result.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAllotedForOnlineTimeTable);
                        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        {
                            ObjJsonData.Message = "This Staff Already Alloted To " + FetchAllotedStaff.FirstOrDefault().CLASS_CODE;
                        }
                        else
                        {
                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveOnlineTimeTable), sAcademicYear);
                            ObjJsonData.Message = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("SaveOnlineTimeTableMultiple", "TimeTableController", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public string SaveClassWiseOnlineTimeTable(string TimeTableInfo)
        {
            string sOption = string.Empty;
            if (TimeTableInfo != null)
            {
                var Result = JsonConvert.DeserializeObject<TimeTable>(TimeTableInfo);
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(Result, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                if (liCourseRoot != null && liCourseRoot.Count > 0)
                {
                    string sPaperType = liCourseRoot.FirstOrDefault().PAPER_TYPE;
                    Result.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                    // Allow staff Dupllicate Entry For Optional Course ... 
                    if (!string.IsNullOrEmpty(sPaperType) && sPaperType != "3")
                    {
                        ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveOnlineTimeTable), sAcademicYear);
                        sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                    else
                    {
                        string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAllotedForOnlineTimeTable).
                                     Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                        var FetchAllotedStaff = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(Result, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchAllotedStaff != null && FetchAllotedStaff.Count > 0)
                        {
                            sOption = "This Staff Already Alloted To " + FetchAllotedStaff.FirstOrDefault().CLASS_CODE;
                        }
                        else
                        {
                            Result.Class_Id = Result.CLASS_CODE;
                            ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(Result, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveOnlineTimeTable), sAcademicYear);
                            if (ResultArgs.Success)
                            {
                                sOption = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                sOption = Common.Messages.FailedToSaveRecords;
                            }
                        }

                    }
                }

            }
            return sOption;
        }
        public JsonResult DeleteOnlineTimeTable(string sTimeTableId)
        {
            JsonResultData ObjJsonData = new JsonResultData();
            TimeTable objModel = new TimeTable();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]).ToString() : string.Empty;
                try
                {
                    objModel.Time_Table_Id = sTimeTableId;
                    var DeleteTimeTable = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.DeleteOnlineTimeTable), sAcademicYear);
                    if (DeleteTimeTable.Success)
                    {
                        ObjJsonData.Message = Common.Messages.RecordDeletedSuccessfully;
                        ObjJsonData.ErrorCode = Common.ErrorCode.Accepted;
                    }
                    else
                    {
                        ObjJsonData.Message = Common.Messages.FailedToDeletedTryAgain;
                        ObjJsonData.ErrorCode = Common.ErrorCode.ExpectationFailed;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimeTableController", "DeleteOnlineTimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimeTableController", "DeleteOnlineTimeTable", ex.Message);
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
        public string UpdateOnlineTimeTable(string sCourseId, string sClassId, string sTimeTableId, string sStaffId, string sDayOrderId, string sHourId, string sHourType, string sSemesterId)
        {
            string sOption = string.Empty;
            TimeTable objModel = new TimeTable();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string sActiveSemester = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STAFF].ToString() : "";
                    if (!string.IsNullOrEmpty(sStaffId))
                    {
                        objModel.Course_Id = sCourseId;
                        objModel.COURSE_ROOT_ID = sCourseId;
                        objModel.Class_Id = sClassId;
                        objModel.Staff_Id = sStaffId;
                        objModel.Time_Table_Id = sTimeTableId;
                        objModel.Day_Order_Id = sDayOrderId;
                        objModel.Hour_Id = sHourId;
                        objModel.hourType = sHourType;
                        objModel.Semester_Id = sSemesterId;
                        objModel.ACADEMIC_YEAR = sAcademicYear;
                        var liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(objModel, SQL.Academics.AcademicsSQL.GetAcademicSQL(AcademicSQLCommands.FetchCourseRootInfoById)).DataSource.Data;
                        if (liCourseRoot != null && liCourseRoot.Count > 0)
                        {
                            string sPaperType = liCourseRoot.FirstOrDefault().PAPER_TYPE;
                            objModel.Semester_Id = liCourseRoot.FirstOrDefault().SEMESTER_ID;
                            if (string.IsNullOrEmpty(sTimeTableId))
                            {
                                string sSQL = TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckIsStaffAllotedForOnlineTimeTable).
                                       Replace(Common.Delimiter.QUS + Common.CP_COURSE_ROOT_2017.SEMESTER_ID, sActiveSemester);
                                var IsStaffExist = (List<TimeTable>)CMSHandler.FetchData<TimeTable>(objModel, sSQL, sAcademicYear).DataSource.Data;
                                if (IsStaffExist == null)
                                {
                                    ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SaveOnlineTimeTable), sAcademicYear);
                                    sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                                }
                                else
                                {
                                    sOption = "This Staff occupied this hour ! \n For Class = " + IsStaffExist.FirstOrDefault().CLASS_CODE + " \n  Course = " + IsStaffExist.FirstOrDefault().COURSE_CODE + " !";
                                }
                            }
                            else
                            {
                                ResultArgs ResultArgs = CMSHandler.SaveOrUpdate(objModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.UpdateOnlineTimeTable), sAcademicYear);
                                sOption = (ResultArgs.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                    else
                    {
                        sOption = Common.Messages.FailedToSaveRecords;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("TimeTableController", "UpdateOnlineTimeTable", "Err MSg: " + ex.Message, "Query is empty!");
                        objHandler.WriteError("TimeTableController", "UpdateOnlineTimeTable", ex.Message);
                    }
                }
            }
            else
            {
                sOption = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return sOption;
        }

        public ActionResult OnlineTimeTableClassWiseList()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasses), sAcademicYear).DataSource.Data;
                    if (FetchClass != null && FetchClass.Count > 0)
                        objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("OnlineTimeTableClassWiseList", "TimeTableController", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("OnlineTimeTableClassWiseList", "TimeTableController", ex.Message);
                }
            }
            return View(objViewModel);
        }
        #endregion

        #region CITS Faculty Entry
        public ActionResult PedagogicMethodInputSystem()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    var hourId = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourIdByUserIdandAcYear), sAcademicYear).DataSource.Data;
                    if (hourId != null && hourId.Count > 0)
                    {
                        objViewModel.HourList = new SelectList(hourId, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);
                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindPedagogicMethodInputSystem(string sdate)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sdate);
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCourseAndStaffInTimtablePM).Replace("?STAFF_ID", user_id).Replace("?DAY_ORDER_DATE", Date), sAcademicYear).DataSource.Data;

                    //var hourId = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourIdByUserIdandAcYear), sAcademicYear).DataSource.Data;
                    //if (hourId != null && hourId.Count > 0)
                    //{

                    //    objViewModel.HourList = new SelectList(hourId, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);

                    //}
                    var Units = (List<SUP_UNITS>)CMSHandler.FetchData<SUP_UNITS>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSupUnits), sAcademicYear).DataSource.Data;
                    if (Units != null && Units.Count > 0)
                    {

                        objViewModel.UnitList = new SelectList(Units, Common.SUP_UNITS.UNIT_ID, Common.SUP_UNITS.UNIT_NAME);

                    }
                    var Metholodolagy = (List<SUP_METHODOLOGY>)CMSHandler.FetchData<SUP_METHODOLOGY>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSupMethodology), sAcademicYear).DataSource.Data;
                    if (Metholodolagy != null && Metholodolagy.Count > 0)
                    {

                        objViewModel.MethodologyList = new SelectList(Metholodolagy, Common.SUP_METHODOLOGY.METHODOLOGY_ID, Common.SUP_METHODOLOGY.METHODOLOGY_NAME);

                    }
                    //var AvailabStaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchAvailapleStaffList).Replace("?DAY_ORDER_DATE", Date), sAcademicYear).DataSource.Data;
                    //if (AvailabStaffList != null && AvailabStaffList.Count > 0)
                    //{
                    //    objViewModel.StaffList = new SelectList(AvailabStaffList, Common.GRIEVANCEMEMBERSINFO.STAFF_ID, Common.GRIEVANCEMEMBERSINFO.FIRST_NAME);
                    //}

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchSubStaffforTpm(string shourID, string sdate, string sshift)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(shourID))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        string Date = CommonMethods.ConvertdatetoyearFromat(sdate);

                        var AvailabStaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchAvailapleStaffList).Replace("?DAY_ORDER_DATE", Date).Replace("?HOUR_ID", shourID).Replace("?SHIFT", sshift), sAcademicYear).DataSource.Data;
                        if (AvailabStaffList != null && AvailabStaffList.Count > 0)
                        {
                            ObjJsonData.sResult += "<option value='0'> ---- Select --- </option>";

                            foreach (var item in AvailabStaffList)
                            {
                                ObjJsonData.sResult += "<option value='" + item.STAFF_ID + "' >" + item.FIRST_NAME + "</option>";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjHandler = new ErrorLog())
                        {
                            ObjHandler.WriteError("TimeTableController", "FetchSubStaffforTpm", "Err MSg: " + ex.Message, "Query is empty!");
                            ObjHandler.WriteError("TimeTableController", "FetchSubStaffforTpm", ex.Message);
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
        public JsonResult SavePedagogicMethod(string JsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
            string SemType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : "";
            JsonResultData objResult = new JsonResultData();
            string sSQL = string.Empty;
            try
            {

                var objson = JsonConvert.DeserializeObject<JSONCOURSEPATTERN>(JsonData);
                if (objson.JSONPendagogicMethod != null && objson.JSONPendagogicMethod.Any())
                {
                    foreach (var item in objson.JSONPendagogicMethod)
                    {
                        item.DATE = CommonMethods.ConvertdatetoyearFromat(item.DATE);
                        item.STAFF_ID = user_id;
                        item.ENTRY_ID = user_id;
                        item.ACADEMIC_YEAR = sAcademicYear;
                        item.SEMESTER_TYPE = SemType;
                        item.IS_GLOBAL = "0";

                        string[] methodologyIds = item.METHODOLOGY_ID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (methodologyIds.Length > 0 && methodologyIds.All(id => id != "0"))
                        {
                            foreach (var methodology in methodologyIds)
                            {
                                item.METHODOLOGY_ID = methodology;
                                var SaveCourseQuota = CMSHandler.SaveOrUpdate(item, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SavePendagogicMethod), sAcademicYear);
                                if (SaveCourseQuota.Success)
                                {
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
                                }
                            }
                        }
                        else
                        {
                            var SaveCourseQuota = CMSHandler.SaveOrUpdate(item, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SavePendagogicMethod), sAcademicYear);
                            if (SaveCourseQuota.Success)
                            {
                                objResult.ErrorCode = Common.ErrorCode.Ok;
                                objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                            }
                            else
                            {
                                objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                objResult.Message = Common.Messages.FailedToSaveRecords;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("TimeTable", "SavePedagogicMethod", ex.Message);
                }
            }

            return Json(objResult);
        }
        #endregion

        #region  CITS Administrator Entry
        public ActionResult GlobalPedagogicMethod()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    var hourId = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHoursByTimeTableHours), sAcademicYear).DataSource.Data;
                    if (hourId != null && hourId.Count > 0)
                    {

                        objViewModel.HourList = new SelectList(hourId, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);

                    }
                    var department = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment), sAcademicYear).DataSource.Data;
                    if (department != null && department.Count > 0)
                    {

                        objViewModel.Departmentlist = new SelectList(department, Common.SUP_DEPARTMENT.DEPARTMENT_ID, Common.SUP_DEPARTMENT.DEPARTMENT);

                    }
                    var shift = (List<Sup_Shift>)CMSHandler.FetchData<Sup_Shift>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchShift), sAcademicYear).DataSource.Data;
                    if (shift != null && shift.Count > 0)
                    {

                        objViewModel.ShiftList = new SelectList(shift, Common.SUP_SHIFT.SHIFT_ID, Common.SUP_SHIFT.SHIFT_NAME);

                    }
                    var Units = (List<SUP_UNITS>)CMSHandler.FetchData<SUP_UNITS>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSupUnitsForGlobal), sAcademicYear).DataSource.Data;
                    if (Units != null && Units.Count > 0)
                    {

                        objViewModel.UnitList = new SelectList(Units, Common.SUP_UNITS.UNIT_ID, Common.SUP_UNITS.UNIT_NAME);

                    }
                    var Metholodolagy = (List<SUP_METHODOLOGY>)CMSHandler.FetchData<SUP_METHODOLOGY>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSupMethodologyDesc), sAcademicYear).DataSource.Data;
                    if (Metholodolagy != null && Metholodolagy.Count > 0)
                    {

                        objViewModel.MethodologyList = new SelectList(Metholodolagy, Common.SUP_METHODOLOGY.METHODOLOGY_ID, Common.SUP_METHODOLOGY.METHODOLOGY_NAME);

                    }

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "GlobalPedagogicMethod", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "GlobalPedagogicMethod", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchProgrmmebyShiftandDept(string ShiftID, string sDepartment)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(ShiftID))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        var liProgram = (List<CP_PROGRAMME_2017>)CMSHandler.FetchData<CP_PROGRAMME_2017>(new CP_PROGRAMME_2017(), TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchProgrammebyDeptandShift).Replace(Common.Delimiter.QUS + Common.CP_DEPARTMENT_2017.DEPARTMENT, sDepartment).Replace(Common.Delimiter.QUS + Common.SUP_SHIFT.SHIFT_ID, ShiftID)).DataSource.Data;
                        if (liProgram != null && liProgram.Count > 0)
                        {
                            foreach (var item in liProgram)
                            {
                                ObjJsonData.sResult += "<option value='" + item.PROGRAMME_ID + "' >" + item.PROGRAMME_NAME + "</option>";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjHandler = new ErrorLog())
                        {
                            ObjHandler.WriteError("TimeTableController", "FetchProgrmmebyShiftandDept", "Err MSg: " + ex.Message, "Query is empty!");
                            ObjHandler.WriteError("TimeTableController", "FetchProgrmmebyShiftandDept", ex.Message);
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


        // Fetch Course By Department ...
        public JsonResult FetchClassByProgrammeID(string sProgrammeId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sProgrammeId))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        var liClass = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new CP_CLASSES(), TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchClassesByProgrammeId).Replace(Common.Delimiter.QUS + Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, sProgrammeId), sAcademicYear).DataSource.Data;
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
                            ObjHandler.WriteError("TimeTableController", "FetchClassByProgrammeID", "Err MSg: " + ex.Message, "Query is empty!");
                            ObjHandler.WriteError("TimeTableController", "FetchClassByProgrammeID", ex.Message);
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
        public JsonResult SaveGlobalPedagogicMethod(string sdate, string sClassId, string SHourId, string sUnit, string sTopic, string sSubTopic, string sMethodology)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
            string SemType = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_TYPE].ToString() : "";
            string SemId = (Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STUDENT] != null) ? Session[Common.SESSION_VARIABLES.ACTIVE_SEMESTER_FOR_STUDENT].ToString() : "";
            JsonResultData objResult = new JsonResultData();
            var model = new PendagogicMethod();
            string sSQL = string.Empty;
            try
            {
                sdate = CommonMethods.ConvertdatetoyearFromat(sdate);
                string[] classIds = sClassId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] hourIds = SHourId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] MethodologyIds = sMethodology.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var classId in classIds)
                {

                    foreach (var hourId in hourIds)
                    {
                        var dayOrder = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(new PendagogicMethod(), TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDayorderByDate).Replace("?DAY_ORDER_DATE", sdate), sAcademicYear).DataSource.Data;

                        var ISExist = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(new PendagogicMethod(), TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.CheckCITSAlreadyExistorNot).Replace("?DATE", sdate).Replace("?CLASS_ID", classId).Replace("?HOUR_ID", hourId).Replace("?SEMESTER_TYPE", SemType), sAcademicYear).DataSource.Data;
                        if (ISExist != null)
                        {
                            foreach (var item in ISExist)
                            {
                                var pendagogicID = item.PENDAGOGIC_ID;
                                var DeleteCITS = CMSHandler.SaveOrUpdate(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.IFExistDeleteCITS).Replace("?PENDAGOGIC_ID", pendagogicID), sAcademicYear);
                            }
                        }
                        if (dayOrder != null)
                        {
                            var dayorderID = dayOrder.FirstOrDefault().DAY_ORDER_ID;
                            var CourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(new PendagogicMethod(), TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCoursebyDayOrderandClassHour).Replace("?CLASS_ID", classId).Replace("?HOUR_ID", hourId).Replace("?DAY_ORDER_ID", dayorderID), sAcademicYear).DataSource.Data;
                            if (CourseList != null)
                            {
                                foreach (var courseId in CourseList)
                                {
                                    foreach (var methodologyId in MethodologyIds)
                                    {
                                        model.COURSE_ID = courseId.COURSE_ID;
                                        model.CLASS_ID = classId;
                                        model.DATE = sdate;
                                        model.TOPIC_TITLE = sTopic;
                                        model.SUP_TOPIC = sSubTopic;
                                        model.UNIT_ID = sUnit;
                                        model.HOUR_ID = hourId;
                                        model.METHODOLOGY_ID = methodologyId;
                                        model.ACADEMIC_YEAR = sAcademicYear;
                                        //model.STAFF_ID = user_id;
                                        model.SEMESTER_TYPE = SemType;
                                        model.ENTRY_ID = user_id;
                                        model.IS_GLOBAL = "1";
                                        var saveTeachingPlan = CMSHandler.SaveOrUpdate(model, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SavePendagogicMethod), sAcademicYear);

                                        if (saveTeachingPlan.Success)
                                        {
                                            objResult.ErrorCode = Common.ErrorCode.Ok;
                                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                        }
                                        else
                                        {
                                            objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                            objResult.Message = Common.Messages.FailedToSaveRecords;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var methodologyId in MethodologyIds)
                                {
                                    model.CLASS_ID = classId;
                                    model.DATE = sdate;
                                    model.TOPIC_TITLE = sTopic;
                                    model.SUP_TOPIC = sSubTopic;
                                    model.UNIT_ID = sUnit;
                                    model.HOUR_ID = hourId;
                                    model.METHODOLOGY_ID = methodologyId;
                                    model.ACADEMIC_YEAR = sAcademicYear;
                                    //model.STAFF_ID = user_id;
                                    model.SEMESTER_TYPE = SemType;
                                    model.ENTRY_ID = user_id;
                                    model.IS_GLOBAL = "1";
                                    var saveTeachingPlan = CMSHandler.SaveOrUpdate(model, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SavePendagogicMethod), sAcademicYear);

                                    if (saveTeachingPlan.Success)
                                    {
                                        objResult.ErrorCode = Common.ErrorCode.Ok;
                                        objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                    }
                                    else
                                    {
                                        objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                        objResult.Message = Common.Messages.FailedToSaveRecords;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var methodologyId in MethodologyIds)
                            {
                                model.CLASS_ID = classId;
                                model.DATE = sdate;
                                model.TOPIC_TITLE = sTopic;
                                model.SUP_TOPIC = sSubTopic;
                                model.UNIT_ID = sUnit;
                                model.HOUR_ID = hourId;
                                model.METHODOLOGY_ID = methodologyId;
                                model.ACADEMIC_YEAR = sAcademicYear;
                                //model.STAFF_ID = user_id;
                                model.SEMESTER_TYPE = SemType;
                                model.ENTRY_ID = user_id;
                                model.IS_GLOBAL = "1";
                                var saveTeachingPlan = CMSHandler.SaveOrUpdate(model, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.SavePendagogicMethod), sAcademicYear);

                                if (saveTeachingPlan.Success)
                                {
                                    objResult.ErrorCode = Common.ErrorCode.Ok;
                                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                                }
                                else
                                {
                                    objResult.ErrorCode = Common.ErrorCode.NotAcceptable;
                                    objResult.Message = Common.Messages.FailedToSaveRecords;
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
                    ObjLog.WriteError("TimeTable", "SaveGlobalPedagogicMethod", ex.Message);
                }
            }

            return Json(objResult);
        }
        #endregion

        #region CITS Report generation  for staff
        public ActionResult TpmReportforIndividualStaff()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    var hourId = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourIdByUserIdandAcYear), sAcademicYear).DataSource.Data;
                    if (hourId != null && hourId.Count > 0)
                    {

                        objViewModel.HourList = new SelectList(hourId, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);

                    }
                    //var ClassList = (List<ClassInfo>)CMSHandler.FetchData<ClassInfo>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTTClassbyStaffID), sAcademicYear).DataSource.Data;
                    //if (ClassList != null && ClassList.Count > 0)
                    //{

                    //    objViewModel.ClassList = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                    //}
                    var SemType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                    if (SemType != null && SemType.Count > 0)
                    {

                        objViewModel.SemesterTypeList = new SelectList(SemType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);

                    }
                    var Acdemic_year = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (Acdemic_year != null && Acdemic_year.Count > 0)
                    {

                        objViewModel.AcademicYear = new SelectList(Acdemic_year, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "PedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindCITSReportinDateWise(string sdate, string edate, string sAcyear, string sSemType)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string Date = CommonMethods.ConvertdatetoyearFromat(sdate);
                    string ToDate = CommonMethods.ConvertdatetoyearFromat(edate);
                    string semester = string.Empty;
                    if (sSemType == "1")
                    {
                        semester = "1,3,5";
                    }
                    else if (sSemType == "2")
                    {
                        semester = "2,4,6";
                    }
                    else
                    {
                        semester = "0";
                    }
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTPMOverAllReportforIndividualStaff).Replace("?STAFF_ID", user_id).Replace("?SEMESTER_ID", semester).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcyear).DataSource.Data;
                    objViewModel.liTotalHours = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalHoursbyDayorderDateWise).Replace("?SEMESTER_ID", semester).Replace("?STAFF_ID", user_id).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcyear).DataSource.Data;

                    objViewModel.liTotalEntry = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalEntryHourByEntryDate).Replace("?SEMESTER_ID", semester).Replace("?STAFF_ID", user_id).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcyear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindPedagogicMethodInputSystem", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public JsonResult FetchttClassByAcYearandSem(string sAcYear, string SemType)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

            if (!string.IsNullOrEmpty(sAcYear))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        string semester = string.Empty;
                        if (SemType == "1")
                        {
                            semester = "1,3,5";
                        }
                        else if (SemType == "2")
                        {
                            semester = "2,4,6";
                        }
                        else
                        {
                            semester = "0";
                        }

                        var ClassList = (List<ClassInfo>)CMSHandler.FetchData<ClassInfo>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTTClassbyStaffIDAcYearSemWise).Replace("?SEMESTER_ID", semester), sAcYear).DataSource.Data;

                        if (ClassList != null && ClassList.Count > 0)
                        {
                            foreach (var item in ClassList)
                            {
                                ObjJsonData.sResult += "<option value='" + item.CLASS_ID + "' >" + item.CLASS_NAME + "</option>";
                            }

                        }
                        else
                        {
                            ObjJsonData.sResult += "<option value='0' >--- Select ---</option>";
                        }

                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjHandler = new ErrorLog())
                        {
                            ObjHandler.WriteError("TimeTableController", "FetchClassByProgrammeID", "Err MSg: " + ex.Message, "Query is empty!");
                            ObjHandler.WriteError("TimeTableController", "FetchClassByProgrammeID", ex.Message);
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
        public JsonResult FetchttCourseByClassID(string sClassID, string sAcYear, string semType)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

            if (!string.IsNullOrEmpty(sClassID))
            {
                if (!string.IsNullOrEmpty(sAcademicYear))
                {
                    try
                    {
                        string semester = string.Empty;
                        if (semType == "1")
                        {
                            semester = "1,3,5";
                        }
                        else if (semType == "2")
                        {
                            semester = "2,4,6";
                        }
                        else
                        {
                            semester = "0";
                        }
                        var liClass = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(new CP_CLASSES(), TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCourseByClassIdInTT).Replace("?SEMESTER_ID", semester).Replace("?CLASS_ID", sClassID).Replace("?STAFF_ID", user_id), sAcYear).DataSource.Data;
                        ObjJsonData.sResult += "<option value='' > ---- Select --- </option>";

                        if (liClass != null && liClass.Count > 0)
                        {
                            foreach (var item in liClass)
                            {
                                ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "' >" + item.COURSE_TITLE + "</option>";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (ErrorLog ObjHandler = new ErrorLog())
                        {
                            ObjHandler.WriteError("TimeTableController", "FetchClassByProgrammeID", "Err MSg: " + ex.Message, "Query is empty!");
                            ObjHandler.WriteError("TimeTableController", "FetchClassByProgrammeID", ex.Message);
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
        public ActionResult BindCITSReportinCourseWise(string sClass, string sCourse, string SacYear, string semType)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string semester = string.Empty;
                    if (semType == "1")
                    {
                        semester = "1,3,5";
                    }
                    else if (semType == "2")
                    {
                        semester = "2,4,6";
                    }
                    else
                    {
                        semester = "0";
                    }
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.BindCITSReportCourseWiseIndividualReport).Replace("?SEMESTER_ID", semester).Replace("?STAFF_ID", user_id).Replace("?CLASS_ID", sClass).Replace("?COURSE_ID", sCourse), SacYear).DataSource.Data;
                    objViewModel.liTotalMethodology = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMethodologyCountByUserID).Replace("?SEMESTER_TYPE", semType).Replace("?STAFF_ID", user_id).Replace("?CLASS_ID", sClass).Replace("?COURSE_ID", sCourse), SacYear).DataSource.Data;

                    //objViewModel.liTotalEntry = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalEntryHourByEntryDate).Replace("?STAFF_ID", user_id).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindCITSReportinCourseWise", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindCITSReportinCourseWise", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindCITSReportinSubjectWiseforStraff(string sAcYear, string semType)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string semester = string.Empty;
                    if (semType == "1")
                    {
                        semester = "1,3,5";
                    }
                    else if (semType == "2")
                    {
                        semester = "2,4,6";
                    }
                    else
                    {
                        semester = "0";
                    }
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubjectWiseTpmReportForStaff).Replace("?SEMESTER_ID", semester).Replace("?STAFF_ID", user_id), sAcYear).DataSource.Data;
                    objViewModel.liTotalMethodology = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMethTotalCountbyCourseWise).Replace("?SEMESTER_TYPE_ID", semType).Replace("?STAFF_ID", user_id), sAcYear).DataSource.Data;

                    //objViewModel.liTotalEntry = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalEntryHourByEntryDate).Replace("?STAFF_ID", user_id).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcademicYear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindCITSReportinCourseWise", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindCITSReportinCourseWise", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        #endregion

        #region CITS Report Generation For HOD
        public ActionResult CITSReportForAdministor()
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            var ObjModel = new STF_PERSONAL_INFO();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    ObjModel.STAFF_ID = user_id;

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffInfoById), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        ObjModel.STAFF_ID = liStaff.FirstOrDefault().STAFF_ID;
                        ObjModel.DEPARTMENT = liStaff.FirstOrDefault().DEPARTMENT;

                        string sSQL = SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClassByDepartmenIdShifId).Replace(Common.Delimiter.QUS + Common.STF_PERSONAL_INFO.SHIFT, liStaff.FirstOrDefault().SHIFT);
                        var FetchClass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(ObjModel, sSQL, sAcademicYear).DataSource.Data;
                        if (FetchClass != null && FetchClass.Count > 0)
                        {
                            objViewModel.ClassList = new SelectList(FetchClass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                        }
                        //var fetchStaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchDepartmentWiseStaffList), sAcademicYear).DataSource.Data;

                        var fetchStaffList = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(ObjModel, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                        if (fetchStaffList != null && fetchStaffList.Count > 0)
                        {
                            objViewModel.StaffList = new SelectList(fetchStaffList, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);

                        }
                    }
                    var hourId = (List<SUP_HOURS>)CMSHandler.FetchData<SUP_HOURS>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchHourIdByUserIdandAcYear), sAcademicYear).DataSource.Data;
                    if (hourId != null && hourId.Count > 0)
                    {

                        objViewModel.HourList = new SelectList(hourId, Common.SUP_HOURS.HOUR_ID, Common.SUP_HOURS.HOUR);

                    }
                    var ClassList = (List<ClassInfo>)CMSHandler.FetchData<ClassInfo>(new StaffList() { STAFF_ID = user_id }, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTTClassbyStaffID), sAcademicYear).DataSource.Data;
                    if (ClassList != null && ClassList.Count > 0)
                    {

                        objViewModel.ClassList = new SelectList(ClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);

                    }
                    var SemType = (List<SUP_SEMESTER_TYPE>)CMSHandler.FetchData<SUP_SEMESTER_TYPE>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchSemesterType), sAcademicYear).DataSource.Data;
                    if (SemType != null && SemType.Count > 0)
                    {

                        objViewModel.SemesterTypeList = new SelectList(SemType, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE_ID, Common.SUP_SEMESTER_TYPE.SEMESTER_TYPE);

                    }
                    var Acdemic_year = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (Acdemic_year != null && Acdemic_year.Count > 0)
                    {

                        objViewModel.AcademicYear = new SelectList(Acdemic_year, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);

                    }
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "CITSReportForAdministor", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "CITSReportForAdministor", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindCITSAdministratorEntryList(string sAcyear, string SemType)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {

                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";

                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCITSAdministratorEntryList).Replace("?SEMESTER_TYPE_ID", SemType), sAcyear).DataSource.Data;

                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindCITSAdministratorEntryList", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindCITSAdministratorEntryList", Ex.Message);
                }

            }
            return View(objViewModel);
        }
        public ActionResult BindCITSReportStaffWiseforHOD(string staffId, string sAcYear, string SemType)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string semester = string.Empty;
                    if (SemType == "1")
                    {
                        semester = "1,3,5";
                    }
                    else if (SemType == "2")
                    {
                        semester = "2,4,6";
                    }
                    else
                    {
                        semester = "0";
                    }
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";

                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchSubjectWiseTpmReportForStaff).Replace("?SEMESTER_ID", semester).Replace("?STAFF_ID", staffId), sAcYear).DataSource.Data;
                    objViewModel.liTotalMethodology = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMethodologyCountByStaffID).Replace("?SEMESTER_TYPE_ID", SemType).Replace("?STAFF_ID", staffId), sAcYear).DataSource.Data;

                    //objViewModel.liTotalEntry = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalEntryHourByEntryDate).Replace("?STAFF_ID", user_id).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcademicYear).DataSource.Data;
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindCITSReportStaffWiseforHOD", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindCITSReportStaffWiseforHOD", Ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindCITSReportStaffWiseforPdf(string staffId, string sAcYear, string SemType)
        {
            TimeTableViewModel objViewModel = new TimeTableViewModel();
            JsonResultData objResult = new JsonResultData();
            try
            {
                if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                {
                    string semester = string.Empty;
                    if (SemType == "1")
                    {
                        semester = "1,3,5";
                    }
                    else if (SemType == "2")
                    {
                        semester = "2,4,6";
                    }
                    else
                    {
                        semester = "0";
                    }
                    string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : "";
                    string user_id = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : "";
                    objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;
                    objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCITSReportForHOD).Replace("?SEMESTER_ID", semester).Replace("?STAFF_ID", staffId), sAcYear).DataSource.Data;
                    objViewModel.liTotalMethodology = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMethodologyCountByStaffIDforHODPDF).Replace("?SEMESTER_TYPE_ID", SemType).Replace("?STAFF_ID", staffId), sAcYear).DataSource.Data;

                    //objViewModel.liTotalEntry = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(null, TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchTotalEntryHourByEntryDate).Replace("?STAFF_ID", user_id).Replace("?DATE", Date).Replace("?TODATE", ToDate), sAcademicYear).DataSource.Data;
                    //BindCITSReportStaffWiseforPdfPrint( staffId,  sAcYear, SemType);
                }
                else
                    return RedirectToAction("ErrorMessage", "Error");
            }
            catch (Exception Ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindCITSReportStaffWiseforHOD", "Err MSg: " + Ex.Message, "Query is empty!");
                    objHandler.WriteError("TimeTable", "BindCITSReportStaffWiseforHOD", Ex.Message);
                }
            }
            return View(objViewModel);
        }
        public ActionResult BindCITSReportStaffWiseforPdfPrint(string staffId, string sAcYear, string SemType)
        {
            try
            {
                TimeTableViewModel objViewModel = new TimeTableViewModel();
                // Validate input parameters
                if (string.IsNullOrEmpty(staffId) || string.IsNullOrEmpty(sAcYear) || string.IsNullOrEmpty(SemType))
                {
                    return Content("Invalid input parameters.");
                }
                // Process semester and fetch data
                string semester = SemType == "1" ? "1,3,5" : SemType == "2" ? "2,4,6" : "0";
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString();
                string user_id = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString();
                objViewModel.liCollegeDetails = (List<CollegeDetails>)CMSHandler.FetchData<CollegeDetails>(null, SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchCollegeDetails)).DataSource.Data;

                objViewModel.liStaffCourseList = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(
                    null,
                    TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchCITSReportForHOD)
                        .Replace("?SEMESTER_ID", semester)
                        .Replace("?STAFF_ID", staffId),
                    sAcYear
                ).DataSource.Data;
                objViewModel.liTotalMethodology = (List<PendagogicMethod>)CMSHandler.FetchData<PendagogicMethod>(
                    null,
                    TimeTableSQL.GetTimeTableSQL(TimeTableSQLCommands.FetchMethodologyCountByStaffIDforHODPDF)
                        .Replace("?SEMESTER_TYPE_ID", SemType)
                        .Replace("?STAFF_ID", staffId),
                    sAcYear
                ).DataSource.Data;

                // Validate data availability
                if (objViewModel.liStaffCourseList == null || !objViewModel.liStaffCourseList.Any())
                {
                    return Content("No records found for this staff.");
                }
                string semesterTypeName = SemType == "1" ? "Odd" : SemType == "2" ? "Even" : "0";

                // Define output path for each staff's report
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string outputDirectory = Path.Combine(desktopPath, "CITS_FACULTY_REPORT", sAcYear + "_" + semesterTypeName);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Create PDF for each staff
                foreach (var GstaffGroup in objViewModel.liStaffCourseList.GroupBy(o => o.STAFF_ID).ToList())
                {
                    string currentStaffId = GstaffGroup.Key;

                    // Create a new view model for the current staff
                    var filteredViewModel = new TimeTableViewModel
                    {
                        liStaffCourseList = objViewModel.liStaffCourseList.Where(x => x.STAFF_ID == currentStaffId).ToList(),
                        liTotalMethodology = objViewModel.liTotalMethodology.Where(x => x.STAFF_ID == currentStaffId).ToList(),
                        liCollegeDetails = objViewModel.liCollegeDetails
                    };

                    // Generate the PDF using Rotativa
                    var staffPdf = new Rotativa.ViewAsPdf("BindCITSReportStaffWiseforPdf", filteredViewModel)
                    {
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = "--print-media-type --no-stop-slow-scripts --javascript-delay 5000"
                    };

                    // Save the PDF to the specified directory
                    string pdfFilename = Path.Combine(outputDirectory, $"{GstaffGroup.FirstOrDefault().STAFF_NAME}_{sAcYear}_{semesterTypeName}_Report.pdf");
                    byte[] pdfBytes = staffPdf.BuildFile(ControllerContext);
                    System.IO.File.WriteAllBytes(pdfFilename, pdfBytes);
                }

                return Content("PDFs successfully saved.");
            }
            catch (Exception ex)
            {
                // Log the error
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("TimeTable", "BindCITSReportStaffWiseforPdfPrint", "Error: " + ex.Message);
                }

                // Return an error message to the client
                return Content("An error occurred while generating the PDFs.");
            }
        }
        public JsonResult SaveChartImage(string imageData, string imgType, string staffId, string classId, string course_ID)
        {
            try
            {
                // Decode the Base64 string
                byte[] imageBytes = Convert.FromBase64String(imageData.Split(',')[1]);

                // Define the file path
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = "desktopPath";
                if (imgType == "1")
                {
                    folderPath = Path.Combine(desktopPath, "CITS_FACULTY_REPORT", "Charts", staffId, "Unit");

                }
                else
                {
                    folderPath = Path.Combine(desktopPath, "CITS_FACULTY_REPORT", "Charts", staffId, "Methodology");

                }
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string filePath = Path.Combine(folderPath, $"{classId}_{course_ID}_Chart.png");

                // Save the image
                System.IO.File.WriteAllBytes(filePath, imageBytes);

                return Json(new { success = true, filePath = filePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion


    }
}