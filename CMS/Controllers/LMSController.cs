using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CMS.DAO;
using CMS.SQL.Examination;
using CMS.SQL.LMS;
using CMS.SQL.SupportData;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace CMS.Controllers
{
    public class LMSController : Controller
    {
        ResultArgs resultArgs = new ResultArgs();
        JsonResultData objResultData = new JsonResultData();
        #region student
        // GET: LMS
        public ActionResult LMSHomepageforstudent()
        {
            LMSViewModel ObjviewModel = new LMSViewModel();
            string sAcyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sStudentId))
            {
                try
                {
                    GET_ACTIVE_SEMESTER_BATCH objActiveSemester = new GET_ACTIVE_SEMESTER_BATCH();
                    objActiveSemester.STUDENT_ID = sStudentId;
                    // Get All The Regular Papers By Student_Id ...
                    string sSQL5 = string.Empty;
                    sSQL5 = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration).Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sStudentId);
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objActiveSemester,
                       sSQL5, sAcyear).DataSource.Data;
                    string sSQL6 = string.Empty;
                    sSQL6 = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses).Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sStudentId);

                    var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objActiveSemester,
                      sSQL6, sAcyear).DataSource.Data;
                    if (liRegularCourses != null && liRegularCourses.Count > 0)
                    {

                        // var liTemp = liRegularCourses.Where(o => o.COURSE_STRUCTURE_ID == "579").ToList();
                        // Loop Optional Papers  To Remove Unnecessary Courses...
                        if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                        {
                            foreach (var item in liOptionalCourses)
                            {
                                var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID && o.STUDENT_ID == item.STUDENT_ID).ToList();
                                if (IsExist != null && IsExist.Count > 0)
                                {
                                    liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID && o.STUDENT_ID == IsExist.FirstOrDefault().STUDENT_ID);
                                    item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                    item.STUDENT_ID = IsExist.FirstOrDefault().STUDENT_ID;
                                    liRegularCourses.Add(item);
                                }
                                else
                                {
                                    liRegularCourses.Add(item);
                                }
                            }
                        }


                        ObjviewModel.liExamRegistrationList = liRegularCourses.OrderBy(o => o.COURSE_ORDER).ToList();

                        //Fetch Class Incharges
                        ObjviewModel.liClasscoursestaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(objActiveSemester,
                              LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasscourseStaffByClassId), sAcyear).DataSource.Data;

                        //Fetch Announcement
                        ObjviewModel.liAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { STUDENT_ID = sStudentId },
                              LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAnnouncement), sAcyear).DataSource.Data;
                        //Fetch Course Wise Attachments

                        ObjviewModel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(new LMS_COURSEWISE_ANNOUNCEMENT() { STUDENT_ID = sStudentId },
                             LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClassworkFiles), sAcyear).DataSource.Data;

                        //Fetch Quiz Setting

                        StudentController student = new StudentController();
                        var liClass = liRegularCourses.GroupBy(x => x.CLASS_ID).Select(x => new EXAM_REGISTRATION() { CLASS_ID = x.FirstOrDefault().CLASS_ID }).ToList();
                        string sClassIds = string.Join(",", liClass.Select(x => x.CLASS_ID).ToArray());

                        var liCourse = liRegularCourses.GroupBy(x => x.COURSE_ROOT_ID).Select(x => new EXAM_REGISTRATION() { COURSE_ID = x.FirstOrDefault().COURSE_ROOT_ID }).ToList();
                        string sCourseIds = string.Join(",", liCourse.Select(x => x.COURSE_ID).ToArray());

                        string sSQL = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchSettingForStudentQuizEntryByClassAndCourseIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                        var liFetchQuizSetting = (List<QUIZ_CLASS_COURSE_STAFF>)CMSHandler.FetchData<QUIZ_CLASS_COURSE_STAFF>(null, sSQL, sAcyear).DataSource.Data;
                        if (liFetchQuizSetting != null && liFetchQuizSetting.Count > 0)
                        {
                            foreach (var item in liFetchQuizSetting)
                            {
                                item.STATUS = student.ValidateQuizForSetWise(item.SETTING_ID, sStudentId, item.COURSE_ID, sAcyear);
                            }
                        }
                        ObjviewModel.liQuizSetting = liFetchQuizSetting;

                        //Fetch Assignment Setting
                        string sSQL1 = string.Empty;
                        sSQL1 = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchActivelmsassignmentsByStudentId);
                        sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                        sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                        sSQL1 = sSQL1.Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sStudentId);


                        var liActiveAssignment = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(null, sSQL1, sAcyear).DataSource.Data;
                        if (liActiveAssignment != null && liActiveAssignment.Count > 0)
                        {
                            ObjviewModel.liassignmentsetting = liActiveAssignment;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LMSController", "LMSHomepageforstudent", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");


                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjviewModel);
        }
        public ActionResult BindCourses()
        {
            LMSViewModel ObjviewModel = new LMSViewModel();
            string sAcyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sStudentId))
            {
                try
                {
                    GET_ACTIVE_SEMESTER_BATCH objActiveSemester = new GET_ACTIVE_SEMESTER_BATCH();
                    objActiveSemester.STUDENT_ID = sStudentId;
                    // Get All The Regular Papers By Student_Id ...
                    string sSQL5 = string.Empty;
                    sSQL5 = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration).Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sStudentId);
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objActiveSemester,
                       sSQL5, sAcyear).DataSource.Data;
                    string sSQL6 = string.Empty;
                    sSQL6 = ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses).Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sStudentId);

                    var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(objActiveSemester,
                      sSQL6, sAcyear).DataSource.Data;

                    // var liTemp = liRegularCourses.Where(o => o.COURSE_STRUCTURE_ID == "579").ToList();
                    // Loop Optional Papers  To Remove Unnecessary Courses...
                    if (liOptionalCourses != null && liOptionalCourses.Count > 0)
                    {
                        foreach (var item in liOptionalCourses)
                        {
                            var IsExist = liRegularCourses.Where(o => o.COURSE_ROOT_ID == item.COURSE_ROOT_ID && o.STUDENT_ID == item.STUDENT_ID).ToList();
                            if (IsExist != null && IsExist.Count > 0)
                            {
                                liRegularCourses.RemoveAll(o => o.COURSE_STRUCTURE_ID == IsExist.FirstOrDefault().COURSE_STRUCTURE_ID && o.STUDENT_ID == IsExist.FirstOrDefault().STUDENT_ID);
                                item.COURSE_STRUCTURE_ID = IsExist.FirstOrDefault().COURSE_STRUCTURE_ID;
                                item.STUDENT_ID = IsExist.FirstOrDefault().STUDENT_ID;
                                liRegularCourses.Add(item);
                            }
                            else
                            {
                                liRegularCourses.Add(item);
                            }
                        }
                    }
                    if (liRegularCourses != null && liRegularCourses.Count > 0)
                    {
                        ObjviewModel.liExamRegistrationList = liRegularCourses.OrderBy(o => o.COURSE_ORDER).ToList();
                    }

                    //Fetch Class Incharges
                    ObjviewModel.liClasscoursestaff = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(objActiveSemester,
                          LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasscourseStaffByClassId), sAcyear).DataSource.Data;

                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LMSController", "BindCourses", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");


                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjviewModel);
        }
        public ActionResult BindAnnouncements()
        {
            LMSViewModel ObjviewModel = new LMSViewModel();
            string sAcyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            if (!string.IsNullOrEmpty(sStudentId))
            {
                try
                {
                    //Fetch Announcement
                    ObjviewModel.liAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { STUDENT_ID = sStudentId },
                          LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAnnouncement), sAcyear).DataSource.Data;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LMSController", "BindAnnouncements", "Err MSg: " + ex.Message, "Query is empty!");
                    }
                    return RedirectToAction("ErrorMessage", "Error");


                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjviewModel);
        }
        public ActionResult BindClassworkFiles()
        {
            LMSViewModel ObjviewModel = new LMSViewModel();

            string sAcyear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null
                                ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()
                                : "";

            string sStudentId = Session[Common.SESSION_VARIABLES.USER_ID] != null
                                ? Session[Common.SESSION_VARIABLES.USER_ID].ToString()
                                : "";

            if (string.IsNullOrEmpty(sStudentId) || string.IsNullOrEmpty(sAcyear))
                return RedirectToAction("ErrorMessage", "Error");

            try
            {
                // ✅ 1. FETCH ALL CLASSWORK FILES (ENUM QUERY)
                var fetchResult = CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(
                    new { STUDENT_ID = sStudentId },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClassworkFiles),
                    sAcyear
                );

                List<LMS_COURSEWISE_ATTACHMENTS> allRecords =
                    fetchResult != null &&
                    fetchResult.DataSource != null &&
                    fetchResult.DataSource.Data != null
                    ? (List<LMS_COURSEWISE_ATTACHMENTS>)fetchResult.DataSource.Data
                    : new List<LMS_COURSEWISE_ATTACHMENTS>();

                // ✅ 2. SPLIT NOTES_ID = 4 AND OTHERS
                var notes4Records = allRecords.Where(x => x.NOTES_ID == "4").ToList();
                var otherRecords = allRecords.Where(x => x.NOTES_ID != "4").ToList();

                // ✅ 3. PROCESS NOTES_ID = 4 ONLY IF EXISTS
                if (notes4Records.Count > 0)
                {
                    List<LMS_COURSEWISE_ATTACHMENTS> processedList =
                        new List<LMS_COURSEWISE_ATTACHMENTS>();

                    for (int i = 0; i < notes4Records.Count; i++)
                    {
                        var item = notes4Records[i];

                        string courseId = item.COURSE_ID;
                        string staffId = item.STAFF_ID;
                        string classId = item.CLASS_ID;
                        string coursewiseatt = item.COURSEWISE_ATTACHMENT_ID;

                        // ✅ 1. FETCH SEMESTER START DATE
                        var semesterFetch = CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(
                            new { ACADEMIC_YEAR = sAcyear },
                            LMSSQL.GetLMSSQL(LMSSQLCommands.FetchActiveSemesterStart),
                            sAcyear
                        );

                        IEnumerable<LMS_COURSEWISE_ATTACHMENTS> semesterResult =
                            semesterFetch != null &&
                            semesterFetch.DataSource != null &&
                            semesterFetch.DataSource.Data != null
                            ? (IEnumerable<LMS_COURSEWISE_ATTACHMENTS>)semesterFetch.DataSource.Data
                            : new List<LMS_COURSEWISE_ATTACHMENTS>();

                        DateTime semesterStartDate = semesterResult.Count() > 0
                            ? Convert.ToDateTime(semesterResult.First().DATE_FROM)
                            : DateTime.Now;

                        string formattedDate = semesterStartDate.ToString("yyyy-MM-dd");

                        // ✅ 2. FETCH DAY ORDER COUNT
                        var dayOrderFetch = CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(
                            new
                            {
                                DATE_FROM = formattedDate,
                                COURSE_ID = courseId,
                                STAFF_ID = staffId,
                                CLASS_ID = classId,
                                AC_YEAR = sAcyear
                            },
                            LMSSQL.GetLMSSQL(LMSSQLCommands.FetchDayOrderCount),
                            sAcyear
                        );

                        IEnumerable<LMS_COURSEWISE_ATTACHMENTS> dayOrderResult =
                            dayOrderFetch != null &&
                            dayOrderFetch.DataSource != null &&
                            dayOrderFetch.DataSource.Data != null
                            ? (IEnumerable<LMS_COURSEWISE_ATTACHMENTS>)dayOrderFetch.DataSource.Data
                            : new List<LMS_COURSEWISE_ATTACHMENTS>();

                        int dayOrderCount = dayOrderResult.Count() > 0
                            ? Convert.ToInt32(dayOrderResult.First().HOUR)
                            : 0;

                        int nextHour = dayOrderCount + 1;

                        // ✅ 3. FETCH NEXT HOUR RECORD FOR THIS ITEM
                        if (dayOrderCount > 0)
                        {
                            var nextHourFetch = CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(
                                new
                                {
                                    COURSEWISE_ATTACHMENT_ID = coursewiseatt,
                                    HOURID = nextHour,
                                    STUDENT_ID = sStudentId,
                                    AC_YEAR = sAcyear,
                                },
                                LMSSQL.GetLMSSQL(LMSSQLCommands.FetchNextHourAttachment),
                                sAcyear
                            );

                            List<LMS_COURSEWISE_ATTACHMENTS> nextHourRecords =
                                nextHourFetch != null &&
                                nextHourFetch.DataSource != null &&
                                nextHourFetch.DataSource.Data != null
                                ? (List<LMS_COURSEWISE_ATTACHMENTS>)nextHourFetch.DataSource.Data
                                : new List<LMS_COURSEWISE_ATTACHMENTS>();

                            processedList.AddRange(nextHourRecords);
                        }
                    }

                    // ✅ REPLACE ORIGINAL NOTES4 WITH PROCESSED DATA
                    notes4Records = processedList;
                }

                // ✅ 7. FINAL MERGE
                var finalList = new List<LMS_COURSEWISE_ATTACHMENTS>();
                finalList.AddRange(otherRecords);
                finalList.AddRange(notes4Records);

                ObjviewModel.lilmscoursewiseatt = finalList;
                return View(ObjviewModel);
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError(
                        "LMSController",
                        "BindClassworkFiles",
                        "Err Msg: " + ex.Message,
                        "ENUM QUERY FAILED"
                    );
                }

                return RedirectToAction("ErrorMessage", "Error");
            }
        }


        public ActionResult BindClassTest()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LMSViewModel();
            StudentController student = new StudentController();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    // Get All The Regular Papers By Student_Id ...
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                            SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                          SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;

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

                    string sSQL = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchSettingForStudentQuizEntryByClassAndCourseIds);
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
                    ObjViewModel.liQuizSetting = liFetchQuizSetting;

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "BindClassTest", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindAssignment()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LMSViewModel();

            StudentController student = new StudentController();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    // Get All The Regular Papers By Student_Id ...
                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                            SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistration), sAcademicYear).DataSource.Data;
                    var liOptionalCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                          SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchOptionalCourses), sAcademicYear).DataSource.Data;
                    if (liRegularCourses != null && liRegularCourses.Count > 0)
                    {
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

                        string sSQL = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLMSActivelmsassignmentsByStudentId);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASSES_2017.CLASS_ID, sClassIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.CP_CLASS_COURSE_2017.COURSE_ID, sCourseIds);
                        sSQL = sSQL.Replace(Common.Delimiter.QUS + Common.STUDENT_INFO.STUDENT_ID, sUserId);

                        //Get all the active Assignment by student_id
                        var liActiveAssignment = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(null, sSQL, sAcademicYear).DataSource.Data;
                        if (liActiveAssignment != null && liActiveAssignment.Count > 0)
                        {
                            ObjViewModel.liassignmentsetting = liActiveAssignment;
                        }
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "BindAssignment", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public JsonResult SaveStuAssignment()
        {
            string sDirectorypath = string.Empty, sFilePath = string.Empty;
            var TotalPath = string.Empty;
            string sGrievanceType = string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
                    //from formdata deserializing the sJsonData
                    var result = JsonConvert.DeserializeObject<JSON_LMS_ASSIGNMENT>(Request.Form["JsonData"]);


                    if (sUserId != null)
                    {
                        if (result.listuassignment != null && result.listuassignment.Count > 0)
                        {
                            foreach (var item in result.listuassignment)
                            {
                                item.STUDENT_ID = sUserId;

                                resultArgs = CMSHandler.SaveOrUpdate(item, LMSSQL.GetLMSSQL(LMSSQLCommands.SavestuAssignment), sAcademicYear, true);
                                var sRid = resultArgs.RowUniqueId.ToString();

                                if (resultArgs.Success && Request.Files.Count > 0 && Request.Files[item.COURSE_ID] != null && sRid != null)
                                {
                                    item.STU_ASSIGNMENT_ID = resultArgs.RowUniqueId.ToString();
                                    //File savings
                                    if (Request.Files.Count > 0 && !string.IsNullOrEmpty(item.STU_ASSIGNMENT_ID))
                                    {
                                        sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.LMS_ASSIGNMENT_PATH;
                                        if (!Directory.Exists(sDirectorypath))
                                        {
                                            Directory.CreateDirectory(sDirectorypath);
                                        }
                                        var file = Request.Files[item.COURSE_ID];
                                        var fileName = file.FileName;
                                        int lastIndexDot = fileName.LastIndexOf('.');
                                        var nameFile = fileName.Substring(0, lastIndexDot);
                                        var extensionPart = fileName.Substring(lastIndexDot + 1);
                                        string sSavePath = Common.URLPages.LMS_ASSIGNMENT_PATH + nameFile + "." + extensionPart;
                                        sFilePath = sDirectorypath + "\\" + nameFile + "." + extensionPart;
                                        //var sFile = fileName.Split('.');
                                        //sFilePath = sDirectorypath + "\\" + sFile[0] + "." + sFile[1];
                                        //string sSavePath = Common.URLPages.LMS_ASSIGNMENT_PATH + sFile[0] + "." + sFile[1];
                                        file.SaveAs(sFilePath);
                                        item.FILE_PATH = sSavePath;
                                        var update = CMSHandler.SaveOrUpdate(new LMS_STUDENT_ASSIGNMENT_SUBMISSION() { FILE_PATH = sSavePath, STU_ASSIGNMENT_ID = sRid }, LMSSQL.GetLMSSQL(LMSSQLCommands.SavestuAssignmentAttachment), sAcademicYear);
                                        if (resultArgs.Success)
                                        {
                                            objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                                            objResultData.ErrorCode = Common.ErrorCode.Ok;
                                        }
                                        else
                                        {
                                            objResultData.Message = Common.Messages.FailedToSaveRecords;
                                            objResultData.ErrorCode = Common.ErrorCode.NotFound;
                                        }
                                    }
                                    else
                                    {
                                        objResultData.Message = Common.Messages.FailedToSaveRecords;
                                        objResultData.ErrorCode = Common.ErrorCode.NotFound;
                                    }

                                }
                                else
                                {
                                    objResultData.Message = Common.Messages.FailedToSaveRecords;
                                    objResultData.ErrorCode = Common.ErrorCode.NotFound;
                                }
                            }

                        }
                        else
                        {
                            objResultData.Message = Common.Messages.FailedToSaveRecords;
                        }

                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog objHandler = new ErrorLog())
                    {
                        objHandler.WriteError("LMS", "SaveCourseWiseAttachements", ex.Message);
                    }
                }
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Course Wise Attachments
        public ActionResult AddCourseWiseAttachments()
        {
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            LMSViewModel lmsviewmodel = new LMSViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                lmsviewmodel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(null, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchlmscoursewiseatt), sAcademicYear).DataSource.Data;

                var lilmsunit = (List<SUP_LMS_UNIT>)CMSHandler.FetchData<SUP_LMS_UNIT>(null, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchlmsunit)).DataSource.Data;
                if (lilmsunit != null && lilmsunit.Count > 0)
                {
                    lmsviewmodel.UNITLIST = new SelectList(lilmsunit, Common.SUP_LMS_UNIT.LMS_UNIT_ID, Common.SUP_LMS_UNIT.UNIT);
                }

                //var licourse = (List<cp_Course_Root_2017>)CMSHandler.FetchData<cp_Course_Root_2017>(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourse)).DataSource.Data;
                //if (licourse != null && licourse.Count > 0)
                //{
                //    lmsviewmodel.COURSELIST = new SelectList(licourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                //}

                var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                if (liclass != null && liclass.Count > 0)
                {
                    lmsviewmodel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }


                var lisupClassNotes = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsclassNotesListforddl), sAcademicYear).DataSource.Data;
                if (lisupClassNotes != null && lisupClassNotes.Count > 0)
                {
                    lmsviewmodel.LMSCLASSNOTES = new SelectList(lisupClassNotes, Common.CP_CLASSES_2017.NOTES_ID, Common.CP_CLASSES_2017.NOTES_TITLE);
                }

                var lisupquestionpaper = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsQuestionPaperListforddl), sAcademicYear).DataSource.Data;
                if (lisupquestionpaper != null && lisupquestionpaper.Count > 0)
                {
                    lmsviewmodel.LMSQUESTIONPAPER = new SelectList(lisupquestionpaper, Common.CP_CLASSES_2017.QUESTION_PAPER_ID, Common.CP_CLASSES_2017.PAPER_TITLE);
                }

                var liquesType = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsQuestionPaperTypeListforddl), sAcademicYear).DataSource.Data;
                if (liquesType != null && liquesType.Count > 0)
                {
                    lmsviewmodel.LMSQUESTIONPAPERTYPE = new SelectList(liquesType, Common.CP_CLASSES_2017.QUESTION_PAPER_TYPE_ID, Common.CP_CLASSES_2017.TYPE);
                }

            }
            return View(lmsviewmodel);
        }
        public JsonResult FetchCourseByClassid(string sClassid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { CLASS_ID = sClassid, STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassId), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    foreach (var item in licourse)
                    {
                        ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + " >" + item.COURSE_TITLE + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCourseByClassidlms(string sClassid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { CLASS_ID = sClassid, STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchCourseByClassIdLMS), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    foreach (var item in licourse)
                    {
                        ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + " >" + item.COURSE_TITLE + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Fetchassignmentsettingid(string sClassid, string sCourseid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var licourse = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new LMS_ASSIGNMENT_SETTING { CLASS_ID = sClassid, STAFF_ID = sUserId, COURSE_ID = sCourseid }, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchassignmentsettingbycourseid), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    foreach (var item in licourse)
                    {
                        ObjJsonData.sResult += "<option value=" + item.ASSIGNMENT_SETTING_ID + " >" + item.ASSIGNMENT_NAME + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LMSCourseWiseAttachments()
        {
            LMSViewModel lmsviewmodel = new LMSViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sSql = string.Empty;
                if (sRoleId == "4")
                {
                    sSql = LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchlmscoursewiseatt).Replace("-- R", "");
                    sSql = sSql.Replace("?STAFF_ID", sUserId);
                }
                else
                {
                    sSql = LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchlmscoursewiseatt);
                }
                lmsviewmodel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(null, sSql, sAcademicYear).DataSource.Data;


                var lilmsunit = (List<SUP_LMS_UNIT>)CMSHandler.FetchData<SUP_LMS_UNIT>(null, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchlmsunit)).DataSource.Data;
                if (lilmsunit != null && lilmsunit.Count > 0)
                {
                    lmsviewmodel.UNITLIST = new SelectList(lilmsunit, Common.SUP_LMS_UNIT.LMS_UNIT_ID, Common.SUP_LMS_UNIT.UNIT);
                }

                var licourse = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    lmsviewmodel.COURSELIST = new SelectList(licourse, Common.CP_CLASSES_2017.COURSE_ROOT_ID, Common.CP_CLASSES_2017.COURSE_CODE);
                }

                var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                if (liclass != null && liclass.Count > 0)
                {
                    lmsviewmodel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }


                var lisupClassNotes = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsclassNotesListforddl), sAcademicYear).DataSource.Data;
                if (lisupClassNotes != null && lisupClassNotes.Count > 0)
                {
                    lmsviewmodel.LMSCLASSNOTES = new SelectList(lisupClassNotes, Common.CP_CLASSES_2017.NOTES_ID, Common.CP_CLASSES_2017.NOTES_TITLE);
                }

                var lisupquestionpaper = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsQuestionPaperListforddl), sAcademicYear).DataSource.Data;
                if (lisupquestionpaper != null && lisupquestionpaper.Count > 0)
                {
                    lmsviewmodel.LMSQUESTIONPAPER = new SelectList(lisupquestionpaper, Common.CP_CLASSES_2017.QUESTION_PAPER_ID, Common.CP_CLASSES_2017.PAPER_TITLE);
                }

                var liquesType = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsQuestionPaperTypeListforddl), sAcademicYear).DataSource.Data;
                if (liquesType != null && liquesType.Count > 0)
                {
                    lmsviewmodel.LMSQUESTIONPAPERTYPE = new SelectList(liquesType, Common.CP_CLASSES_2017.QUESTION_PAPER_TYPE_ID, Common.CP_CLASSES_2017.TYPE);
                }
            }
            return View(lmsviewmodel);
        }

        //public JsonResult SaveCourseWiseAttachements()
        //{
        //    string sDirectorypath = string.Empty, sFilePath = string.Empty;
        //    var TotalPath = string.Empty;
        //    string sGrievanceType = string.Empty;
        //    if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
        //    {
        //        try
        //        {
        //            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
        //            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
        //            //from formdata deserializing the sJsonData
        //            var result = JsonConvert.DeserializeObject<LMS_COURSEWISE_ATTACHMENTS>(Request.Form["sJsonData"]);
        //            LMS_COURSEWISE_ATTACHMENTS objcoursewiseatt = new LMS_COURSEWISE_ATTACHMENTS();

        //            if (result != null && sUserId != null)
        //            {
        //                result.STAFF_ID = sUserId;
        //                result.ACADEMIC_YEAR = sAcademicYear;
        //                var HtmlContent = HttpUtility.UrlDecode(Request.Form["sHtml"]);
        //                result.DESCRIPTION = HtmlContent;
        //                resultArgs = CMSHandler.SaveOrUpdate(result, LMSSQL.GetLMSSQL(LMSSQLCommands.SaveCoursewiseatt), sAcademicYear, true);

        //                if (resultArgs.Success && Request.Files.Count > 0)
        //                {
        //                    result.COURSEWISE_ATTACHMENT_ID = resultArgs.RowUniqueId.ToString();
        //                    //File savings
        //                    if (Request.Files.Count > 0 && !string.IsNullOrEmpty(result.COURSEWISE_ATTACHMENT_ID))
        //                    {

        //                        sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.LMS_FILE_PATH;
        //                        if (!Directory.Exists(sDirectorypath))
        //                        {
        //                            Directory.CreateDirectory(sDirectorypath);
        //                        }

        //                        for (int i = 0; i < Request.Files.Count; i++)
        //                        {
        //                            var file = Request.Files[i];
        //                            var fileName = file.FileName;
        //                            var ext = Path.GetExtension(fileName).ToLower();
        //                            var sFile = fileName.Split('.');
        //                            int lastIndexDot = fileName.LastIndexOf('.');
        //                            var nameFile = fileName.Substring(0, lastIndexDot);
        //                            var extensionPart = fileName.Substring(lastIndexDot + 1);
        //                            string sSavePath = Common.URLPages.LMS_FILE_PATH + nameFile + "." + extensionPart;
        //                            sFilePath = sDirectorypath + "\\" + nameFile + "." + extensionPart;

        //                            //sFilePath = sDirectorypath + "\\" + sFile[0] + "." + sFile[1];
        //                            //string sSavePath = Common.URLPages.LMS_FILE_PATH + sFile[0] + "." + sFile[1];
        //                            file.SaveAs(sFilePath);
        //                            result.FILE_PATH = sSavePath;
        //                            var update = CMSHandler.SaveOrUpdate(result, LMSSQL.GetLMSSQL(LMSSQLCommands.Savecoursewisefiles), sAcademicYear, true);


        //                            if (ext == ".xlsx" || ext == ".xls")
        //                            {
        //                                // Set EPPlus License Context
        //                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //                                using (var package = new OfficeOpenXml.ExcelPackage(file.InputStream))
        //                                {
        //                                    ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();
        //                                    if (ws != null)
        //                                    {
        //                                        int rowCount = ws.Dimension.Rows;

        //                                        for (int row = 2; row <= rowCount; row++)
        //                                        {
        //                                            var excelRow = new LMS_EXCEL_UPLOAD_DETAILS();

        //                                            excelRow.COURSEWISE_ATTACHMENT_ID = result.COURSEWISE_ATTACHMENT_ID;
        //                                            excelRow.HOUR = Convert.ToString(ws.Cells[row, 2].Value);
        //                                            excelRow.UNIT = Convert.ToString(ws.Cells[row, 3].Value);
        //                                            excelRow.TOPIC = Convert.ToString(ws.Cells[row, 4].Value);
        //                                            excelRow.METHOD = Convert.ToString(ws.Cells[row, 5].Value);
        //                                            excelRow.RESOURCE = Convert.ToString(ws.Cells[row, 6].Value);
        //                                            excelRow.ASSESSMENT = Convert.ToString(ws.Cells[row, 7].Value);
        //                                            excelRow.RUBRIC = Convert.ToString(ws.Cells[row, 8].Value);
        //                                            excelRow.ANY_OTHER = Convert.ToString(ws.Cells[row, 9].Value);

        //                                            // INSERT INTO DB
        //                                            CMSHandler.SaveOrUpdate(
        //                                                excelRow,
        //                                                LMSSQL.GetLMSSQL(LMSSQLCommands.SaveExcelUploadforlmsCourseWiseAtt),
        //                                                sAcademicYear,
        //                                                true
        //                                            );
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }



        //                    }
        //                    else
        //                    {
        //                        objResultData.Message = Common.Messages.FailedToSaveRecords;
        //                    }
        //                    objResultData.Message = resultArgs.Success ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
        //                }
        //                else
        //                {
        //                    objResultData.Message = Common.Messages.FailedToSaveRecords;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            using (ErrorLog objHandler = new ErrorLog())
        //            {
        //                objHandler.WriteError("LMS", "SaveCourseWiseAttachements", ex.Message);
        //            }
        //        }
        //    }

        //    return Json(objResultData, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveCourseWiseAttachements()
        {
            string sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.LMS_FILE_PATH;

            if (!Directory.Exists(sDirectorypath))
                Directory.CreateDirectory(sDirectorypath);

            try
            {
                string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
                string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                var baseModel = JsonConvert.DeserializeObject<LMS_COURSEWISE_ATTACHMENTS>(
                    Request.Form["sJsonData"]
                );

                baseModel.STAFF_ID = sUserId;
                baseModel.ACADEMIC_YEAR = sAcademicYear;
                baseModel.DESCRIPTION = HttpUtility.UrlDecode(Request.Form["sHtml"]);

                // ✅ 1️⃣ HANDLE EXCEL → INSERT PER ROW
                HttpPostedFileBase excelFile = Request.Files["ExcelFile"];
                if (excelFile != null && excelFile.ContentLength > 0)
                {
                    SaveExcelAndInsertCourseWise(excelFile, baseModel, sAcademicYear, sDirectorypath);
                }

                // ✅ 2️⃣ HANDLE NORMAL FILES → INSERT PER FILE
                SaveNormalFilesWithCourseWiseInsert(Request.Files, baseModel, sAcademicYear, sDirectorypath);

                objResultData.Message = Common.Messages.RecordsSavedSuccessfully;
                objResultData.ErrorCode = Common.ErrorCode.Ok;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMS", "SaveCourseWiseAttachements", ex.Message);
                }

                objResultData.Message = Common.Messages.FailedToSaveRecords;
            }

            return Json(objResultData, JsonRequestBehavior.AllowGet);
        }


        private void SaveExcelAndInsertCourseWise(
    HttpPostedFileBase file,
    LMS_COURSEWISE_ATTACHMENTS baseModel,
    string sAcademicYear,
    string directoryPath)
        {
            string ext = Path.GetExtension(file.FileName).ToLower();
            string newFileName = Guid.NewGuid().ToString() + ext;
            string fullPath = Path.Combine(directoryPath, newFileName);
            file.SaveAs(fullPath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(fullPath)))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null) return;

                int rowCount = ws.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    LMS_COURSEWISE_ATTACHMENTS model = new LMS_COURSEWISE_ATTACHMENTS();

                    model.STAFF_ID = baseModel.STAFF_ID;
                    model.CLASS_ID = baseModel.CLASS_ID;
                    model.COURSE_ID = baseModel.COURSE_ID;
                    model.NOTES_ID = baseModel.NOTES_ID;
                    model.ACADEMIC_YEAR = baseModel.ACADEMIC_YEAR;
                    model.DESCRIPTION = baseModel.DESCRIPTION;
                    model.FILE_PATH = Common.URLPages.LMS_FILE_PATH + newFileName;

                    // ✅ ✅ ✅ INSERT COURSEWISE PER ROW
                    var resultArgs = CMSHandler.SaveOrUpdate(
                        model,
                        LMSSQL.GetLMSSQL(LMSSQLCommands.SaveCoursewiseatt),
                        sAcademicYear,
                        true
                    );

                    string coursewiseId = resultArgs.RowUniqueId.ToString();

                    // ✅ SAVE EXCEL ROW DETAILS
                    LMS_EXCEL_UPLOAD_DETAILS excelRow = new LMS_EXCEL_UPLOAD_DETAILS();

                    excelRow.COURSEWISE_ATTACHMENT_ID = coursewiseId;
                    excelRow.HOUR = Convert.ToString(ws.Cells[row, 2].Value);
                    excelRow.UNIT = Convert.ToString(ws.Cells[row, 3].Value);
                    excelRow.TOPIC = Convert.ToString(ws.Cells[row, 4].Value);
                    excelRow.METHOD = Convert.ToString(ws.Cells[row, 5].Value);
                    excelRow.RESOURCE = Convert.ToString(ws.Cells[row, 6].Value);
                    excelRow.ASSESSMENT = Convert.ToString(ws.Cells[row, 7].Value);
                    excelRow.RUBRIC = Convert.ToString(ws.Cells[row, 8].Value);
                    excelRow.ANY_OTHER = Convert.ToString(ws.Cells[row, 9].Value);

                    CMSHandler.SaveOrUpdate(
                        excelRow,
                        LMSSQL.GetLMSSQL(LMSSQLCommands.SaveExcelUploadforlmsCourseWiseAtt),
                        sAcademicYear,
                        true
                    );
                }
            }
        }


        private void SaveNormalFilesWithCourseWiseInsert(
    HttpFileCollectionBase files,
    LMS_COURSEWISE_ATTACHMENTS baseModel,
    string sAcademicYear,
    string directoryPath)
        {


            string sDirectorypath = string.Empty, sFilePath = string.Empty;

            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                try
                {
                    string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();

                    var result = JsonConvert.DeserializeObject<LMS_COURSEWISE_ATTACHMENTS>(Request.Form["sJsonData"]);

                    if (result != null && sUserId != null)
                    {
                        result.STAFF_ID = sUserId;
                        result.ACADEMIC_YEAR = sAcademicYear;

                        result.DESCRIPTION = HttpUtility.UrlDecode(Request.Form["sHtml"]);

                        var resultArgs = CMSHandler.SaveOrUpdate(
                            result,
                            LMSSQL.GetLMSSQL(LMSSQLCommands.SaveCoursewiseatt),
                            sAcademicYear,
                            true
                        );

                        if (resultArgs.Success)
                        {
                            result.COURSEWISE_ATTACHMENT_ID = resultArgs.RowUniqueId.ToString();

                            sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.LMS_FILE_PATH;

                            if (!Directory.Exists(sDirectorypath))
                            {
                                Directory.CreateDirectory(sDirectorypath);
                            }

                            
                            if (Request.Files.Count > 0)
                            {
                                for (int i = 0; i < Request.Files.Count; i++)
                                {
                                    var file = Request.Files[i];

                                    if (file == null || file.ContentLength == 0)
                                        continue;

                                    string ext = Path.GetExtension(file.FileName).ToLower();

                                    if (ext == ".xls" || ext == ".xlsx")
                                        continue;

                                    string uniqueName = Guid.NewGuid().ToString() + ext;
                                    sFilePath = Path.Combine(sDirectorypath, uniqueName);

                                    file.SaveAs(sFilePath);

                                    string sSavePath = Common.URLPages.LMS_FILE_PATH + uniqueName;

                                    LMS_COURSEWISE_ATTACHMENTS fileModel = new LMS_COURSEWISE_ATTACHMENTS
                                    {
                                        COURSEWISE_ATTACHMENT_ID = result.COURSEWISE_ATTACHMENT_ID,
                                        FILE_PATH = sSavePath,
                                        STAFF_ID = sUserId,
                                        ACADEMIC_YEAR = sAcademicYear
                                    };

                                    CMSHandler.SaveOrUpdate(
                                        fileModel,
                                        LMSSQL.GetLMSSQL(LMSSQLCommands.Savecoursewisefiles),
                                        sAcademicYear,
                                        true
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }


        }


        // Fetch Assignment Setting By Id ...
        public JsonResult FetchCourseWiseAttachmnet(string sAttachmentId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchAssignmentSetting = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(new LMS_COURSEWISE_ATTACHMENTS() { COURSEWISE_ATTACHMENT_ID = sAttachmentId },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchCourseWiseAttachmnetById), sAcademicYear).DataSource.Data;
                if (liFetchAssignmentSetting != null && liFetchAssignmentSetting.Count > 0)
                {
                    return Json(liFetchAssignmentSetting, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BingCourseWiseAttachmentsbyNotes(string CLASS_ID, string COURSE_ID, string NOTES_ID)
        {
            LMSViewModel ObjViewModel = new LMSViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {
                ObjViewModel.listattachmnets = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(new LMS_COURSEWISE_ATTACHMENTS() { STAFF_ID = Staffid, COURSE_ID = COURSE_ID, CLASS_ID = CLASS_ID, NOTES_ID = NOTES_ID },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.fechattachmentsByNotesid), sAcademicYear).DataSource.Data;
                //ObjViewModel.liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new stf_Personal_Info() { STAFF_ID = Staffid },SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchcourselistfromclasscoursestaff), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        public JsonResult DeleteCourseWiseAttachments(string sCWId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var Deletelms = CMSHandler.SaveOrUpdate(new LMS_COURSEWISE_ATTACHMENTS() { COURSEWISE_ATTACHMENT_ID = sCWId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.DeleteCoursewiseatt), null);
                    if (Deletelms.Success)
                    {
                        resultArgs = CMSHandler.SaveOrUpdate(null, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.UnSafeUpdateMode));
                        resultArgs = CMSHandler.SaveOrUpdate(new LMS_COURSEWISE_FILES() { COURSEWISE_ATTACHMENT_ID = sCWId, IS_DELETED = Common.DeleteFlag.Deleted }, SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.DeleteCoursewisefiles), null);
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
                        ObjLog.WriteError("ExaminationController", "DeleteExamTimeTable", ex.Message);
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

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateCourseWiseAttachments(string sSaveCourseWiseAttachment)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();


            try
            {
                var model = JsonConvert.DeserializeObject<LMS_COURSEWISE_ATTACHMENTS>(sSaveCourseWiseAttachment);

                model.STAFF_ID = sUserId;

                if (model.TOPIC != null)
                {
                    var resultArgs1 = CMSHandler.SaveOrUpdate(
                    model,
                    LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateCourseWiseAttachmentsExcelUpload),
                    sAcademicYear,
                    true
                );




                }
                else
                {
                    var resultArgs = CMSHandler.SaveOrUpdate(
                                        model,
                                        LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateCourseWiseAttachments),
                                        sAcademicYear,
                                        true
                                    );
                }


                //var resultArgs1 = CMSHandler.SaveOrUpdate(
                //    model,
                //    LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateCourseWiseAttachmentsExcelUpload),
                //    sAcademicYear,
                //    true
                //);


                // 2) File Upload
                if (Request.Files.Count > 0 && !string.IsNullOrEmpty(model.COURSEWISE_ATTACHMENT_ID))
                {
                    string folder = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.LMS_FILE_PATH;

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var file = Request.Files[0];
                    var fileName = Path.GetFileName(file.FileName);

                    string physicalPath = Path.Combine(folder, fileName);
                    string dbPath = Common.URLPages.LMS_FILE_PATH + fileName;

                    file.SaveAs(physicalPath);

                    model.FILE_PATH = dbPath;

                    CMSHandler.SaveOrUpdate(
                        model,
                        LMSSQL.GetLMSSQL(LMSSQLCommands.updateCourseWiseFile),
                        sAcademicYear,
                        true
                    );
                }

                ObjJsonData.Message = "Record Updated Successfully!";
            }
            catch (Exception ex)
            {
                ObjJsonData.Message = "Error: " + ex.Message;
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Overall & Classwise Announcement
        public ActionResult LMSAnnouncementList()
        {
            LMSViewModel ObjViewModel = new LMSViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            try
            {

                var liClassList = (List<CP_CLASSES>)CMSHandler.FetchData<CP_CLASSES>(new stf_Personal_Info() { STAFF_ID = Staffid },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                if (liClassList != null && liClassList.Count > 0)
                {
                    ObjViewModel.ClassList = new SelectList(liClassList, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                }

                //ObjViewModel.liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new stf_Personal_Info() { STAFF_ID = Staffid },SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchcourselistfromclasscoursestaff), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", ex.Message);
                }

            }
            return View(ObjViewModel);
        }
        public ActionResult BindLMSCourseWiseandOverallAnnouncement(string announcementType)
        {
            LMSViewModel ObjViewModel = new LMSViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string userRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            ViewBag.role = userRole;
            ViewBag.AType = announcementType;
            try
            {
                ObjViewModel.listAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { STAFF_ID = Staffid, ANNOUNCEMENT_TYPE = announcementType },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLMSAnnouncement), sAcademicYear).DataSource.Data;
                //ObjViewModel.liCourseRoot = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new stf_Personal_Info() { STAFF_ID = Staffid },SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.Fetchcourselistfromclasscoursestaff), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", ex.Message);
                }
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchcourselistByclassId(string sClassId)
        {
            var ObjJsonData = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {


                    if (sClassId != null)
                    {
                        var liCourseRootList = (List<CP_COURSE_ROOT>)CMSHandler.FetchData<CP_COURSE_ROOT>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = Staffid, CLASS_ID = sClassId },
                             SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchClasscoursestaffbyClassid), sAcademicYear).DataSource.Data;

                        if (liCourseRootList != null && liCourseRootList.Count > 0)
                        {
                            foreach (var item in liCourseRootList)
                            {
                                ObjJsonData.sResult += "<option value='" + item.COURSE_ROOT_ID + "'> " + item.COURSE_TITLE + " </option>";
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMSController", "FetchcourselistByclassId", ex.Message);
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
                ObjJsonData.ErrorCode = Common.ErrorCode.NotAcceptable;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveLMSannouncement(string sJson)
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            try
            {

                if (!string.IsNullOrEmpty(sJson))
                {
                    var Result = JsonConvert.DeserializeObject<LMS_COURSEWISE_ANNOUNCEMENT>(sJson);
                    Result.STAFF_ID = Staffid;

                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string path = Path.Combine(Server.MapPath("~/LMS_Files/Announcements/"), fileName);
                            // Ensure directory exists
                            string directory = Path.GetDirectoryName(path);
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                            file.SaveAs(path);
                            Result.FILE_PATH = "~/LMS_Files/Announcements/" + fileName;
                        }
                    }

                    if (!string.IsNullOrEmpty(Result.ANNOUNCEMENT_ID))
                    {
                        Result.ANNOUNCEMENT_ID = Result.ANNOUNCEMENT_ID.Trim();
                        // If no new file uploaded, keep existing file path (logic handled by SQL or by fetching first?)
                        // Actually, if Result.FILE_PATH is null here, the SQL Update will set it to NULL.
                        // We need to fetch the existing record to preserve FILE_PATH if not updating it.
                        // OR, we can change the SQL to only update FILE_PATH if it's not null.
                        // But simpler is to fetch existing one if FILE_PATH is currently null.
                        
                         if (string.IsNullOrEmpty(Result.FILE_PATH))
                        {
                             var existingAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { ANNOUNCEMENT_ID = Result.ANNOUNCEMENT_ID },
                                LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAnnouncementById), sAcademicYear).DataSource.Data;
                             if (existingAnnouncement != null && existingAnnouncement.Count > 0)
                             {
                                 Result.FILE_PATH = existingAnnouncement[0].FILE_PATH;
                             }
                        }

                        var Update = CMSHandler.SaveOrUpdate(Result, LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateLmsannouncement), null);
                        if (Update.Success)
                        {
                            objResult.ErrorCode = Common.ErrorCode.Ok;
                            objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;
                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.BadRequest;
                            objResult.Message = Common.Messages.FailedToUpdatedRecords;
                            return Json(objResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var Ssave = CMSHandler.SaveOrUpdate(Result, LMSSQL.GetLMSSQL(LMSSQLCommands.SaveLmsannouncements), null);

                        if (Ssave.Success)
                        {
                            objResult.ErrorCode = Common.ErrorCode.Ok;
                            objResult.Message = Common.Messages.RecordsSavedSuccessfully;

                        }
                        else
                        {
                            objResult.ErrorCode = Common.ErrorCode.BadRequest;
                            objResult.Message = Common.Messages.FailedToSaveRecords;
                            return Json(objResult, JsonRequestBehavior.AllowGet);
                        }
                    }




                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    objResult.Message = Common.ErrorMessage.InternalServerError;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }



            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "SaveLMSannouncements", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "SaveLMSannouncements", ex.Message);
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
        }


        public JsonResult DeleteLmsAnnouncement(string sCWId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var DeleteAssignmentSetting = CMSHandler.SaveOrUpdate(new LMS_COURSEWISE_ANNOUNCEMENT() { ANNOUNCEMENT_ID = sCWId },
                        LMSSQL.GetLMSSQL(LMSSQLCommands.DeleteLmsAnnouncementbyid), sAcademicYear);
                ObjJsonData.Message = (DeleteAssignmentSetting.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchAnnouncementById(string sAnnouncementId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { ANNOUNCEMENT_ID = sAnnouncementId },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAnnouncementById), sAcademicYear).DataSource.Data;
                if (liAnnouncement != null && liAnnouncement.Count > 0)
                {
                    return Json(liAnnouncement, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region List Assignment settings

        public ActionResult ListAssignmentSettings()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liassignmentsetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new CourseWiseStaffInfo() { STAFF_ID = sUserId },
                          LMSSQL.GetLMSSQL(LMSSQLCommands.ListAssignmentSetting), sAcademicYear).DataSource.Data;

                    var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                    if (liclass != null && liclass.Count > 0)
                    {
                        ObjViewModel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (licourse != null && licourse.Count > 0)
                    {
                        ObjViewModel.COURSELIST = new SelectList(licourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }

                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "ListAssignmentSettings", ex.Message);
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
        public JsonResult FetchCourseByClassidandStaffId(string sClassid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { CLASS_ID = sClassid, STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassIdandStaffId), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    foreach (var item in licourse)
                    {
                        ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + " >" + item.COURSE_TITLE + "</option>";
                    }
                }
                var listudent = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new cp_Classes_2017 { CLASS_ID = sClassid, STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStudentByClassId), sAcademicYear).DataSource.Data;
                if (listudent != null && listudent.Count > 0)
                {
                    foreach (var item in listudent)
                    {
                        ObjJsonData.sResultS += "<option value=" + item.STUDENT_ID + " >" + item.FIRST_NAME + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FetchClassByStudentId(string sClassid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { CLASS_ID = sClassid, STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByClassIdandStaffId), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    foreach (var item in licourse)
                    {
                        ObjJsonData.sResult += "<option value=" + item.COURSE_ROOT_ID + " >" + item.COURSE_TITLE + "</option>";
                    }
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListAssignmentclassgroup()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liassignmentsetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new CourseWiseStaffInfo() { STAFF_ID = sUserId },
                          LMSSQL.GetLMSSQL(LMSSQLCommands.ListAssignmentSetting), sAcademicYear).DataSource.Data;

                    var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                    if (liclass != null && liclass.Count > 0)
                    {
                        ObjViewModel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (licourse != null && licourse.Count > 0)
                    {
                        ObjViewModel.COURSELIST = new SelectList(licourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                    var lisetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new LMS_ASSIGNMENT_SETTING { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchassignmentsettingbystaffid), sAcademicYear).DataSource.Data;
                    if (lisetting != null && lisetting.Count > 0)
                    {
                        ObjViewModel.SETTING = new SelectList(lisetting, Common.LMS_ASSIGNMENT_SETTING.ASSIGNMENT_SETTING_ID, Common.LMS_ASSIGNMENT_SETTING.ASSIGNMENT_NAME);
                    }
                    var ligroup = (List<LMS_CLASS_GROUP>)CMSHandler.FetchData<LMS_CLASS_GROUP>(null, LMSSQL.GetLMSSQL(LMSSQLCommands.fetchclassgroup), sAcademicYear).DataSource.Data;
                    if (ligroup != null && ligroup.Count > 0)
                    {
                        ObjViewModel.CLASS_GROUP = new SelectList(ligroup, Common.LMS_ASSIGNMENT_SETTING.CLS_GROUP_ID, Common.LMS_ASSIGNMENT_SETTING.GROUP_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "ListAssignmentSettings", ex.Message);
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
        public ActionResult ListAssignmentclassgroupInfo()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liassignmentsetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new CourseWiseStaffInfo() { STAFF_ID = sUserId },
                          LMSSQL.GetLMSSQL(LMSSQLCommands.ListAssignmentSetting), sAcademicYear).DataSource.Data;

                    var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                    if (liclass != null && liclass.Count > 0)
                    {
                        ObjViewModel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (licourse != null && licourse.Count > 0)
                    {
                        ObjViewModel.COURSELIST = new SelectList(licourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                    var lisetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new LMS_ASSIGNMENT_SETTING { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchassignmentsettingbystaffid), sAcademicYear).DataSource.Data;
                    if (lisetting != null && lisetting.Count > 0)
                    {
                        ObjViewModel.SETTING = new SelectList(lisetting, Common.LMS_ASSIGNMENT_SETTING.ASSIGNMENT_SETTING_ID, Common.LMS_ASSIGNMENT_SETTING.ASSIGNMENT_NAME);
                    }
                    var ligroup = (List<LMS_CLASS_GROUP>)CMSHandler.FetchData<LMS_CLASS_GROUP>(null, LMSSQL.GetLMSSQL(LMSSQLCommands.fetchclassgroup), sAcademicYear).DataSource.Data;
                    if (ligroup != null && ligroup.Count > 0)
                    {
                        ObjViewModel.CLASS_GROUP = new SelectList(ligroup, Common.LMS_ASSIGNMENT_SETTING.CLS_GROUP_ID, Common.LMS_ASSIGNMENT_SETTING.GROUP_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "ListAssignmentSettings", ex.Message);
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
        public JsonResult SaveAssignmentstudentgroup(string Sstudentids, string sclass, string Scourse, string ssetting, string sgroup, string assinfo)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            LMS_ASSIGNMENT_SETTING objectmodel = new LMS_ASSIGNMENT_SETTING();
            LMS_ASSIGNMENT_SETTING objectmodel2 = new LMS_ASSIGNMENT_SETTING();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var studentList = JsonConvert.DeserializeObject<List<LMS_ASSIGNMENT_SETTING>>(Sstudentids);
                objectmodel.CLASS_ID = sclass;
                objectmodel.COURSE_ID = Scourse;
                objectmodel.SETTING_ID = ssetting;
                objectmodel.ASSIGNMENT_INFO = assinfo;
                objectmodel.GROUP_ID = sgroup;
                objectmodel.ACADEMIC_YEAR = sAcademicYear;
                foreach (var item in studentList)
                {
                    objectmodel.STUDENT_ID = item.STUDENT_ID;

                    if (!string.IsNullOrEmpty(item.LMS_GROUP_ID))
                    {
                        var UpdateQuiz = CMSHandler.SaveOrUpdate(objectmodel, LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateAssignmentSetting), sAcademicYear);
                        ObjJsonData.Message = (UpdateQuiz.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        var SaveQuiz = CMSHandler.SaveOrUpdate(objectmodel, LMSSQL.GetLMSSQL(LMSSQLCommands.Insertassignmentsettingstudent), sAcademicYear);
                        ObjJsonData.Message = (SaveQuiz.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }

            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateAssignmentGroupInfo(string sUpdateGroupinfo)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            LMS_ASSIGNMENT_SETTING objectmodel = new LMS_ASSIGNMENT_SETTING();
            LMS_ASSIGNMENT_SETTING objectmodel2 = new LMS_ASSIGNMENT_SETTING();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var studentList = JsonConvert.DeserializeObject<LMS_ASSIGNMENT_SETTING>(sUpdateGroupinfo);
                objectmodel.CLASS_ID = studentList.CLASS_ID;
                objectmodel.COURSE_ID = studentList.COURSE_ID;
                objectmodel.GROUP_ID = studentList.GROUP_ID;
                objectmodel.LMS_GROUP_ID = studentList.LMS_GROUP_ID;
                objectmodel.ACADEMIC_YEAR = sAcademicYear;
                if (!string.IsNullOrEmpty(objectmodel.LMS_GROUP_ID))
                {
                    var Updategroupinfo = CMSHandler.SaveOrUpdate(objectmodel, LMSSQL.GetLMSSQL(LMSSQLCommands.Updateassignmentsettingstudent), sAcademicYear);
                    ObjJsonData.Message = (Updategroupinfo.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BindstudentlistyClassandCourseId(string sClassid, string settingid, string sCourseid)
        {
            LMSViewModel ObjViewModel = new LMSViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string userRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;

            try
            {
                ObjViewModel.listudent = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { COURSE_ID = sCourseid, CLASS_ID = sClassid, SETTING_ID = settingid },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStudentByclassandcourseId), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", ex.Message);
                }
            }
            return View(ObjViewModel);
        }
        public ActionResult BindstudentlistyClassgroupinfo(string sClassid, string settingid, string sCourseid)
        {
            LMSViewModel ObjViewModel = new LMSViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string userRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;

            try
            {
                ObjViewModel.listudentgroupinfo = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new LMS_ASSIGNMENT_SETTING() { COURSE_ID = sCourseid, CLASS_ID = sClassid, SETTING_ID = settingid },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchassignmentstudentgroupinfo), sAcademicYear).DataSource.Data;
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", ex.Message);
                }
            }
            return View(ObjViewModel);
        }
        public JsonResult FetchAssignmentclassGroupInfo(string sgroupid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchAssignmentSetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new LMS_ASSIGNMENT_SETTING() { LMS_GROUP_ID = sgroupid },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAssignmentGroupstudentGroupByid), sAcademicYear).DataSource.Data;
                if (liFetchAssignmentSetting != null && liFetchAssignmentSetting.Count > 0)
                {
                    return Json(liFetchAssignmentSetting, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Fetch Assignment Setting By Id ...
        public JsonResult FetchAssignmentSetting(string sSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchAssignmentSetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new LMS_ASSIGNMENT_SETTING() { ASSIGNMENT_SETTING_ID = sSettingId },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAssignmentSettingById), sAcademicYear).DataSource.Data;
                if (liFetchAssignmentSetting != null && liFetchAssignmentSetting.Count > 0)
                {
                    return Json(liFetchAssignmentSetting, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Save And Update Assignment Setting ...
        public JsonResult SaveAssignmentSetting(string sSaveAssignmentSetting)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var Result = JsonConvert.DeserializeObject<LMS_ASSIGNMENT_SETTING>(sSaveAssignmentSetting);

                // Handle File Upload
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string sDirectorypath = AppDomain.CurrentDomain.BaseDirectory + Common.URLPages.LMS_ASSIGNMENT_PATH;
                        if (!Directory.Exists(sDirectorypath))
                        {
                            Directory.CreateDirectory(sDirectorypath);
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        var extension = Path.GetExtension(fileName);
                        var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        string uniqueName = nameWithoutExt + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        
                        string sSavePath = Common.URLPages.LMS_ASSIGNMENT_PATH + uniqueName;
                        string sPhysicalPath = Path.Combine(sDirectorypath, uniqueName);

                        file.SaveAs(sPhysicalPath);
                        Result.FILE_PATH = sSavePath;
                    }
                }

                Result.START_DATE = CommonMethods.ConvertdatetoyearFromat(Result.START_DATE);
                Result.END_DATE = CommonMethods.ConvertdatetoyearFromat(Result.END_DATE);
                Result.STAFF_ID = sUserId;

                var studentList = Result.STUDENT_ID.Split(',').ToList();

                foreach (var studentId in studentList)
                {
                    Result.STUDENT_ID = studentId.Trim();
                    if (!string.IsNullOrEmpty(Result.ASSIGNMENT_SETTING_ID))
                    {
                        var UpdateQuiz = CMSHandler.SaveOrUpdate(Result, LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateAssignmentSetting), sAcademicYear);
                        ObjJsonData.Message = (UpdateQuiz.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;
                    }
                    else
                    {
                        var SaveQuiz = CMSHandler.SaveOrUpdate(Result, LMSSQL.GetLMSSQL(LMSSQLCommands.SaveAssignmentSetting), sAcademicYear);
                        ObjJsonData.Message = (SaveQuiz.Success) ? Common.Messages.RecordsSavedSuccessfully : Common.Messages.FailedToSaveRecords;
                    }
                }


            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }




        public JsonResult UploadStudentWiseAssignmentExcel(HttpPostedFileBase excelFile, string classId, string courseId)
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                if (excelFile != null && excelFile.ContentLength > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        var ws = package.Workbook.Worksheets.FirstOrDefault();
                        if (ws != null)
                        {
                            int rowCount = ws.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var excelRow = new LMS_EXCEL_UPLOAD_ASSIGNMENT();

                                // Assign Class & Course IDs
                                excelRow.CLASS_ID = classId;
                                excelRow.COURSE_ID = courseId;
                                excelRow.STAFF_ID = sUserId;
                                excelRow.ACADEMIC_YEAR = sAcademicYear;

                                // Other columns
                                excelRow.ROLL_NO = Convert.ToString(ws.Cells[row, 2].Value);
                                excelRow.ASSIGNMENT_NAME = Convert.ToString(ws.Cells[row, 3].Value);
                                excelRow.DESCRIPTION = Convert.ToString(ws.Cells[row, 4].Value);
                                excelRow.TOTAL_MARK = Convert.ToString(ws.Cells[row, 5].Value);


                                var cellValue = ws.Cells[row, 6].Value;

                                DateTime dateValue;
                                if (DateTime.TryParse(Convert.ToString(cellValue), out dateValue))
                                {
                                    excelRow.START_DATE = dateValue.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    excelRow.START_DATE = "";
                                }

                                var cellValueS = ws.Cells[row, 7].Value;

                                DateTime dateValueS;
                                if (DateTime.TryParse(Convert.ToString(cellValueS), out dateValueS))
                                {
                                    excelRow.END_DATE = dateValueS.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    excelRow.END_DATE = "";
                                }


                                // INSERT/UPDATE into DB
                                CMSHandler.SaveOrUpdate(
                                    excelRow,
                                    LMSSQL.GetLMSSQL(LMSSQLCommands.SaveExcelUploadforlmsAssignmentsetting),
                                    sAcademicYear,
                                    true
                                );
                            }

                            return Json(new { success = true, message = "Excel uploaded successfully!" });
                        }
                    }
                }
            }
            return Json(new { success = false, message = "Please select a file." });
        }


        //assignment group info
        public JsonResult DeleteAssignmentgroupinfo(string sSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var DeleteAssignmentSetting = CMSHandler.SaveOrUpdate(new LMS_ASSIGNMENT_SETTING() { LMS_GROUP_ID = sSettingId },
                        LMSSQL.GetLMSSQL(LMSSQLCommands.DeleteAssignmentGroupInfo), sAcademicYear);
                ObjJsonData.Message = (DeleteAssignmentSetting.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        // Delete Assignment Setting ...
        public JsonResult DeleteAssignmentSetting(string sSettingId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var DeleteAssignmentSetting = CMSHandler.SaveOrUpdate(new LMS_ASSIGNMENT_SETTING() { ASSIGNMENT_SETTING_ID = sSettingId },
                        LMSSQL.GetLMSSQL(LMSSQLCommands.DeleteAssignmentSetting), sAcademicYear);
                ObjJsonData.Message = (DeleteAssignmentSetting.Success) ? Common.Messages.RecordDeletedSuccessfully : Common.Messages.FailedToDeletedTryAgain;
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Assignment Mark Entry
        public ActionResult ListAssignments()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                    if (liclass != null && liclass.Count > 0)
                    {
                        ObjViewModel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (licourse != null && licourse.Count > 0)
                    {
                        ObjViewModel.COURSELIST = new SelectList(licourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "ListAssignments", ex.Message);
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
        public ActionResult LMStAssignment()
        {
            return View();
        }
        public ActionResult BindAssignmentForStaff(string sClassid, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int total = 0;
                    int submited = 0;
                    List<LMS_ASSIGNMENT_SETTING> liassignment = new List<LMS_ASSIGNMENT_SETTING>();
                    var liassignmentsetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new CourseWiseStaffInfo() { COURSE_ID = sCourseId, CLASS_ID = sClassid, STAFF_ID = sUserId },
                        LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAssignmentSettingByClassandcourse), sAcademicYear).DataSource.Data;
                    var listudent = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new CP_CLASS_COURSE_STAFF() { CLASS_ID = sClassid, COURSE_ID = sCourseId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStudentsByClassandCourse), sAcademicYear).DataSource.Data;
                    var liassignmentsubmission = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new CP_CLASS_COURSE_STAFF() { CLASS_ID = sClassid, COURSE_ID = sCourseId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAssignmentSubmissionByClassandCourse), sAcademicYear).DataSource.Data;

                    if (liassignmentsetting != null && liassignmentsetting.Count > 0)
                    {
                        foreach (var setting in liassignmentsetting)
                        {
                            if (liassignmentsubmission != null && liassignmentsubmission.Count() > 0)
                            {
                                var submitedcount = liassignmentsubmission.Where(o => o.ASSIGNMENT_SETTING_ID == setting.ASSIGNMENT_SETTING_ID);
                                if (submitedcount != null && submitedcount.Count() > 0)
                                    submited = submitedcount.Count();
                            }
                            liassignment.Add(new LMS_ASSIGNMENT_SETTING()
                            {
                                CLASS_NAME = setting.CLASS_NAME,
                                ASSIGNMENT_NAME = setting.ASSIGNMENT_NAME,
                                COURSE_TITLE = setting.COURSE_TITLE,
                                COURSE_CODE = setting.COURSE_CODE,
                                TOTAL = listudent.Count().ToString(),
                                SUBMITED = submited.ToString(),
                                START_DATE = setting.START_DATE,
                                END_DATE = setting.END_DATE,
                                ASSIGNMENT_SETTING_ID = setting.ASSIGNMENT_SETTING_ID,
                                CLASS_ID = setting.CLASS_ID,
                                COURSE_ID = setting.COURSE_ID,
                            });
                        }
                        ObjViewModel.liassignmentsetting = liassignment;
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "BindAssignmentForStaff", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindStudentSubmissionForMarkEntry(string sSettingId, string sClassid, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    int total = 0;
                    int submited = 0;
                    var listudent = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new CP_CLASS_COURSE_STAFF() { CLASS_ID = sClassid, COURSE_ID = sCourseId, ASSIGNMENT_SETTING_ID = sSettingId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStudentsByClassCourseandSettingIdForSubmission), sAcademicYear).DataSource.Data;
                    var listuassignment = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new LMS_ASSIGNMENT_SETTING() { ASSIGNMENT_SETTING_ID = sSettingId, COURSE_ID = sCourseId, CLASS_ID = sClassid }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAssignmentSubmissionBySettingId), sAcademicYear).DataSource.Data;
                    List<LMS_STUDENT_ASSIGNMENT_SUBMISSION> liassignment = new List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>();

                    if (listudent != null && listudent.Count > 0)
                    {
                        if (listuassignment != null && listuassignment.Count > 0)
                        {
                            foreach (var stu in listudent)
                            {
                                var IsExist = listuassignment.Where(o => o.STUDENT_ID == stu.STUDENT_ID).ToList();

                                if (IsExist != null && IsExist.Count > 0)
                                {
                                    liassignment.Add(new LMS_STUDENT_ASSIGNMENT_SUBMISSION()
                                    {
                                        STUDENT_ID = stu.STUDENT_ID,
                                        NAME = stu.NAME,
                                        ROLL_NO = stu.ROLL_NO,
                                        COURSE_ID = stu.COURSE_ID,
                                        TOTAL_MARK = stu.TOTAL_MARK,
                                        SEC_MARK = stu.SEC_MARK,
                                        SUBMITED = "1",
                                        ASSIGNMENT_SETTING_ID = stu.ASSIGNMENT_SETTING_ID,
                                        FILE_PATH = IsExist.FirstOrDefault().FILE_PATH,
                                        ASSIGNMENT_NAME = IsExist.FirstOrDefault().ASSIGNMENT_NAME,
                                        COURSE_CODE = stu.COURSE_CODE,
                                        COURSE_TITLE = stu.COURSE_TITLE,
                                        STU_ASSIGNMENT_ID = stu.STU_ASSIGNMENT_ID,
                                    });
                                }
                                else
                                {
                                    liassignment.Add(new LMS_STUDENT_ASSIGNMENT_SUBMISSION()
                                    {
                                        STUDENT_ID = stu.STUDENT_ID,
                                        NAME = stu.NAME,
                                        ROLL_NO = stu.ROLL_NO,
                                        COURSE_ID = stu.COURSE_ID,
                                        TOTAL_MARK = stu.TOTAL_MARK,
                                        SEC_MARK = stu.SEC_MARK,
                                        SUBMITED = submited.ToString(),
                                        ASSIGNMENT_SETTING_ID = stu.ASSIGNMENT_SETTING_ID,
                                        ASSIGNMENT_NAME = stu.ASSIGNMENT_NAME,
                                        COURSE_CODE = stu.COURSE_CODE,
                                        COURSE_TITLE = stu.COURSE_TITLE,
                                        STU_ASSIGNMENT_ID = stu.STU_ASSIGNMENT_ID,
                                    });
                                }
                            }
                            ObjViewModel.listuassignment = liassignment;
                        }
                        else
                        {
                            foreach (var stu in listudent)
                            {
                                liassignment.Add(new LMS_STUDENT_ASSIGNMENT_SUBMISSION()
                                {
                                    STUDENT_ID = stu.STUDENT_ID,
                                    NAME = stu.NAME,
                                    ROLL_NO = stu.ROLL_NO,
                                    COURSE_ID = stu.COURSE_ID,
                                    TOTAL_MARK = stu.TOTAL_MARK,
                                    SEC_MARK = stu.SEC_MARK,
                                    SUBMITED = submited.ToString(),
                                    ASSIGNMENT_SETTING_ID = stu.ASSIGNMENT_SETTING_ID,
                                    ASSIGNMENT_NAME = stu.ASSIGNMENT_NAME,
                                    COURSE_CODE = stu.COURSE_CODE,
                                    COURSE_TITLE = stu.COURSE_TITLE,
                                    STU_ASSIGNMENT_ID = stu.STU_ASSIGNMENT_ID,
                                });
                            }
                            ObjViewModel.listuassignment = liassignment;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "BindStudentSubmissionForMarkEntry", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        // Fetch Student Assignment By Id ...
        public JsonResult FetchStuAssignmentSubmission(string sStusubid)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var liFetchstuAssignment = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new LMS_STUDENT_ASSIGNMENT_SUBMISSION() { STU_ASSIGNMENT_ID = sStusubid },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStuAssignmentSubmissionById), sAcademicYear).DataSource.Data;
                if (liFetchstuAssignment != null && liFetchstuAssignment.Count > 0)
                {
                    return Json(liFetchstuAssignment, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ObjJsonData.Message = Common.Messages.NoRecordsfound;
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }

        // Save And Update Mark for Assignment ...
        public JsonResult SaveAssignmentMark(string sJsonData)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var Result = JsonConvert.DeserializeObject<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(sJsonData);


                if (Result != null)
                {

                    var UpdateMark = CMSHandler.SaveOrUpdate(Result, LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateAssignmentMark), sAcademicYear);

                    ObjJsonData.Message = (UpdateMark.Success) ? Common.Messages.RecordsUpdatedSuccessfully : Common.Messages.FailedToUpdatedRecords;

                }
            }
            else
            {
                ObjJsonData.Message = Common.Messages.SessionIsExpiredPleaseLoginAgain;
            }
            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region List Grades for staff
        public ActionResult ListAssignmentGrades()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjJsonData = new JsonResultData();
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liclass = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClasslistfromclasscoursestaff), sAcademicYear).DataSource.Data;
                    if (liclass != null && liclass.Count > 0)
                    {
                        ObjViewModel.CLASSLIST = new SelectList(liclass, Common.CP_CLASSES_2017.CLASS_ID, Common.CP_CLASSES_2017.CLASS_NAME);
                    }
                    var licourse = (List<CP_CLASS_COURSE_STAFF>)CMSHandler.FetchData<CP_CLASS_COURSE_STAFF>(new cp_Classes_2017 { STAFF_ID = sUserId }, SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                    if (licourse != null && licourse.Count > 0)
                    {
                        ObjViewModel.COURSELIST = new SelectList(licourse, Common.CP_COURSE_ROOT_2017.COURSE_ROOT_ID, Common.CP_COURSE_ROOT_2017.COURSE_CODE);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "ListAssignmentGrades", ex.Message);
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
        public ActionResult BindAssignmentGradesForStaff(string sClassid, string sCourseId)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new LMSViewModel();
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    ObjViewModel.liassignmentsetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new CourseWiseStaffInfo() { COURSE_ID = sCourseId, CLASS_ID = sClassid, STAFF_ID = sUserId },
                        LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAssignmentSettingByClassandcourse), sAcademicYear).DataSource.Data;

                    ObjViewModel.listuassignment = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new CP_CLASS_COURSE_STAFF() { CLASS_ID = sClassid, COURSE_ID = sCourseId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStudentsByClassandCourseForSubmission), sAcademicYear).DataSource.Data;



                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "BindAssignmentForStaff", ex.Message);
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

        #region lms course wise attachment for admin
        public ActionResult ListLMSCourseWiseAttachment()
        {
            var ObjViewModel = new LMSViewModel();

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                           SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.ACDEMICYEAR = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        //ObjViewModel.sMessage = liStaff.FirstOrDefault().DEPARTMENT;
                        ObjViewModel.staffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ListLessonPlan", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindLMSCourseWiseAttachments(string sStaff_id, string Ac_year, string semtype)
        {
            LMSViewModel lmsviewmodel = new LMSViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sSql = string.Empty;

                //sSql = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchlmscoursewiseAttachment).Replace("?STAFF_ID", sStaff_id).Replace("?SEMESTER_TYPE_ID", semtype).Replace("?COURSE_ID", sCourseId);
                sSql = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchlmscoursewiseAttachment).Replace("?STAFF_ID", sStaff_id).Replace("?SEMESTER_TYPE_ID", semtype);

                lmsviewmodel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(null, sSql, Ac_year).DataSource.Data;



            }
            return View(lmsviewmodel);
        }
        #endregion

        #region lms course wise attachment for admin
        public ActionResult ListLMSCourseWiseAttachmentforHOD()
        {
            var ObjViewModel = new LMSViewModel();
            var department_id = string.Empty;
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<ACADEMIC_YEAR>)CMSHandler.FetchData<ACADEMIC_YEAR>(null,
                           SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAcademicYearList), sAcademicYear).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                    {
                        ObjViewModel.ACDEMICYEAR = new SelectList(liAcademicYear, Common.ACADEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACADEMIC_YEAR.AC_YEAR);
                    }
                    var UserDepartment = (List<cp_Department_2017>)CMSHandler.FetchData<cp_Department_2017>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentByStaffId).Replace("?STAFF_ID", sStaffId), sAcademicYear).DataSource.Data;
                    if (UserDepartment != null && UserDepartment.Count > 0)
                    {
                        department_id = UserDepartment.FirstOrDefault().DEPARTMENT;

                    }

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null,
                            SQL.SupportData.SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaffByDepartmentId).Replace("?DEPARTMENT", department_id), sAcademicYear).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                    {
                        //ObjViewModel.sMessage = liStaff.FirstOrDefault().DEPARTMENT;
                        ObjViewModel.staffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                    }
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("IQACController", "ListLessonPlan", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(ObjViewModel);
        }
        public ActionResult BindLMSCourseWiseAttachmentsHOD(string sStaff_id, string Ac_year, string semType)
        {
            LMSViewModel lmsviewmodel = new LMSViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sRoleId = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                string sSql = string.Empty;

                sSql = LMSSQL.GetLMSSQL(LMSSQLCommands.FetchlmscoursewiseAttachmentForHOD).Replace("?STAFF_ID", sStaff_id).Replace("?SEMESTER_TYPE_ID", semType);

                lmsviewmodel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(null, sSql, Ac_year).DataSource.Data;



            }
            return View(lmsviewmodel);
        }
        #endregion
        #region LMS Class Room
        public ActionResult LMSGoogleClassroom()
        {
            ExaminationViewModel ObjViewModel = new ExaminationViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string Staffid = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string userRole = (Session[Common.SESSION_VARIABLES.USER_ROLE_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ROLE_ID].ToString() : string.Empty;
            ViewBag.role = userRole;
            try
            {
                ObjViewModel.listAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { STAFF_ID = Staffid },
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchCourseForstaff), sAcademicYear).DataSource.Data ?? new List<LMS_COURSEWISE_ANNOUNCEMENT>();
                ObjViewModel.listDepartment = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { STAFF_ID = Staffid },
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartmentForstaff), sAcademicYear).DataSource.Data ?? new List<LMS_COURSEWISE_ANNOUNCEMENT>();
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", "Err MSg: " + ex.Message, "Query is empty!");
                    objHandler.WriteError("LMSController", "LMSAnnouncementList", ex.Message);
                }
            }
            return View(ObjViewModel);
        }

        public ActionResult CourseAnnouncements(string id ,string classId)
        {

            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            LMSViewModel lmsviewmodel = new LMSViewModel();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString();
            if (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
            {
                ViewBag.CourseId = id;
                ViewBag.classId = classId;

                lmsviewmodel.liAnnouncement = (List<LMS_COURSEWISE_ANNOUNCEMENT>)CMSHandler.FetchData<LMS_COURSEWISE_ANNOUNCEMENT>(new LMS_COURSEWISE_ANNOUNCEMENT() { COURSE_ID = id, CLASS_ID = classId },
                         LMSSQL.GetLMSSQL(LMSSQLCommands.FetchAnnouncementForClassroom), sAcademicYear).DataSource.Data;

                lmsviewmodel.lilmscoursewiseatt = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(new LMS_COURSEWISE_ATTACHMENTS { STAFF_ID = sUserId, COURSE_ID = id }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchlmscoursewiseattforClassroom), sAcademicYear).DataSource.Data;

                lmsviewmodel.liCoursesinfo = (List<LMS_COURSEWISE_ATTACHMENTS>)CMSHandler.FetchData<LMS_COURSEWISE_ATTACHMENTS>(new LMS_COURSEWISE_ATTACHMENTS { COURSE_ID = id }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchCourse), sAcademicYear).DataSource.Data;

                lmsviewmodel.liassignmentsetting = (List<LMS_ASSIGNMENT_SETTING>)CMSHandler.FetchData<LMS_ASSIGNMENT_SETTING>(new CourseWiseStaffInfo() { STAFF_ID = sUserId, COURSE_ID = id, CLASS_ID = classId },
                          LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchassignmentsetting), sAcademicYear).DataSource.Data;

                lmsviewmodel.limarksGrade = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(new CourseWiseStaffInfo() { COURSE_ID = id, STAFF_ID = sUserId },
                        LMSSQL.GetLMSSQL(LMSSQLCommands.FetchMarksAndGrade), sAcademicYear).DataSource.Data;


                lmsviewmodel.lizoomMeting = (List<LMS_ZOOM_MEETINGS>)CMSHandler.FetchData<LMS_ZOOM_MEETINGS>(new LMS_ZOOM_MEETINGS() { COURSE_ID = id },
                           LMSSQL.GetLMSSQL(LMSSQLCommands.FetchZoomMetingDetailsforstaff), sAcademicYear).DataSource.Data;

               
                    var listudentList = (List<STU_PERSONAL_INFO>)CMSHandler.FetchData<STU_PERSONAL_INFO>(new STU_PERSONAL_INFO() {  CLASS_ID = classId },
                                          LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStudentNameList), sAcademicYear).DataSource.Data;

                
                    lmsviewmodel.liStudentLists = new SelectList(listudentList, Common.STU_PERSONAL_INFO.STUDENT_ID, Common.STU_PERSONAL_INFO.FIRST_NAME);
                

                
                var lilmsunit = (List<SUP_LMS_UNIT>)CMSHandler.FetchData<SUP_LMS_UNIT>(null, LMSSQL.GetLMSSQL(LMSSQLCommands.Fetchlmsunit)).DataSource.Data;
                if (lilmsunit != null && lilmsunit.Count > 0)
                {
                    lmsviewmodel.UNITLIST = new SelectList(lilmsunit, Common.SUP_LMS_UNIT.LMS_UNIT_ID, Common.SUP_LMS_UNIT.UNIT);
                }

                var licourse = (List<cp_Classes_2017>)CMSHandler.FetchData<cp_Classes_2017>(new CP_CLASS_COURSE_STAFF() { STAFF_ID = sUserId }, LMSSQL.GetLMSSQL(LMSSQLCommands.FetchCourseByStaffId), sAcademicYear).DataSource.Data;
                if (licourse != null && licourse.Count > 0)
                {
                    lmsviewmodel.COURSELIST = new SelectList(licourse, Common.CP_CLASSES_2017.COURSE_ROOT_ID, Common.CP_CLASSES_2017.COURSE_CODE);
                }

                var lisupClassNotes = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsclassNotesListforddl), sAcademicYear).DataSource.Data;
                if (lisupClassNotes != null && lisupClassNotes.Count > 0)
                {
                    lmsviewmodel.LMSCLASSNOTES = new SelectList(lisupClassNotes, Common.CP_CLASSES_2017.NOTES_ID, Common.CP_CLASSES_2017.NOTES_TITLE);
                }

                var lisupquestionpaper = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsQuestionPaperListforddl), sAcademicYear).DataSource.Data;
                if (lisupquestionpaper != null && lisupquestionpaper.Count > 0)
                {
                    lmsviewmodel.LMSQUESTIONPAPER = new SelectList(lisupquestionpaper, Common.CP_CLASSES_2017.QUESTION_PAPER_ID, Common.CP_CLASSES_2017.PAPER_TITLE);
                }

                var liquesType = (List<SUP_LMS_CLASS_NOTES>)CMSHandler.FetchData<SUP_LMS_CLASS_NOTES>(new SUP_LMS_CLASS_NOTES(), LMSSQL.GetLMSSQL(LMSSQLCommands.FetchLmsQuestionPaperTypeListforddl), sAcademicYear).DataSource.Data;
                if (liquesType != null && liquesType.Count > 0)
                {
                    lmsviewmodel.LMSQUESTIONPAPERTYPE = new SelectList(liquesType, Common.CP_CLASSES_2017.QUESTION_PAPER_TYPE_ID, Common.CP_CLASSES_2017.TYPE);
                }


                if (lmsviewmodel.liCoursesinfo != null && lmsviewmodel.liCoursesinfo.Count > 0)
                {
                    ViewBag.CourseCode = lmsviewmodel.liCoursesinfo.FirstOrDefault().COURSE_CODE;
                    ViewBag.CourseTitle = lmsviewmodel.liCoursesinfo.FirstOrDefault().COURSE_TITLE;
                }
                else
                {
                    ViewBag.CourseCode = string.Empty;
                    ViewBag.CourseTitle = string.Empty;
                }

            }
            return View(lmsviewmodel);
        }


        public JsonResult SaveLMSannouncement()
        {
            JsonResultData objResult = new JsonResultData();
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? string.Empty;
            string staffId = Session[Common.SESSION_VARIABLES.USER_ID]?.ToString() ?? string.Empty;

            try
            {
                if (string.IsNullOrEmpty(sAcademicYear))
                {
                    objResult.ErrorCode = Common.ErrorCode.BadRequest;
                    objResult.Message = Common.ErrorMessage.InternalServerError;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

                var sJson = Request.Form["sJson"];
                var announcement = JsonConvert.DeserializeObject<LMS_COURSEWISE_ANNOUNCEMENT>(sJson);
                announcement.STAFF_ID = staffId;

                // 📁 Directory path setup
                string virtualPath = Common.URLPages.LMS_FILE_PATH;
                string lmsDirectoryPath = Server.MapPath(virtualPath);
                if (!Directory.Exists(lmsDirectoryPath))
                    Directory.CreateDirectory(lmsDirectoryPath);

                // 📎 Handle uploaded file if present
                if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
                {
                    var file = Request.Files[0];
                    string originalFileName = Path.GetFileName(file.FileName);
                    string ext = Path.GetExtension(originalFileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + ext;
                    string fullFilePath = Path.Combine(lmsDirectoryPath, uniqueFileName);
                    file.SaveAs(fullFilePath);

                    string relativePath = Path.Combine(virtualPath.Replace("~", ""), uniqueFileName).Replace("\\", "/");
                    announcement.FILE_PATH = relativePath;
                }
                else if (!string.IsNullOrEmpty(announcement.EXISTING_FILE_NAME))
                {
                    announcement.FILE_PATH = announcement.EXISTING_FILE_NAME;
                }

                bool isUpdate = !string.IsNullOrEmpty(announcement.ANNOUNCEMENT_ID);

                if (isUpdate)
                {
                    var updateResult = CMSHandler.SaveOrUpdate(
                        announcement,
                        LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateLmsannouncement),
                        sAcademicYear,
                        true
                    );

                    if (!updateResult.Success)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.Messages.FailedToUpdatedRecords;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }

                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = Common.Messages.RecordsUpdatedSuccessfully;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var saveResult = CMSHandler.SaveOrUpdate(
                        announcement,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.SaveLmsannouncement),
                        sAcademicYear,
                        true
                    );

                    if (!saveResult.Success)
                    {
                        objResult.ErrorCode = Common.ErrorCode.BadRequest;
                        objResult.Message = Common.Messages.FailedToSaveRecords;
                        return Json(objResult, JsonRequestBehavior.AllowGet);
                    }

                    announcement.ANNOUNCEMENT_ID = saveResult.RowUniqueId.ToString();

                    if (!string.IsNullOrEmpty(announcement.FILE_PATH))
                    {
                        CMSHandler.SaveOrUpdate(
                            announcement,
                            LMSSQL.GetLMSSQL(LMSSQLCommands.Saveannouncementfile),
                            sAcademicYear,
                            true
                        );
                    }

                    objResult.ErrorCode = Common.ErrorCode.Ok;
                    objResult.Message = Common.Messages.RecordsSavedSuccessfully;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objHandler = new ErrorLog())
                {
                    objHandler.WriteError("LMSController", "SaveLMSannouncement", ex.Message);
                }

                objResult.ErrorCode = Common.ErrorCode.InternalServerError;
                objResult.Message = "An unexpected error occurred while saving the announcement.";
                return Json(objResult, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion
        #region chatroom
        public ActionResult BindCourseChatRooms()
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            var ObjViewModel = new ExaminationViewModel();


            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {

                    var liRegularCourses = (List<EXAM_REGISTRATION>)CMSHandler.FetchData<EXAM_REGISTRATION>(new STU_CLASS() { STUDENT_ID = sUserId },
                            SQL.Examination.ExaminationSQL.GetExaminationSQL(ExaminationSQLCommands.FetchExamRegistrationForLms), sAcademicYear).DataSource.Data;


                    if (liRegularCourses != null && liRegularCourses.Count > 0)
                    {
                        liRegularCourses.RemoveAll(o => o.COURSE_ROOT_ID == "1001" || o.COURSE_ROOT_ID == "1002" || o.COURSE_ROOT_ID == "1188" || o.COURSE_ROOT_ID == "1189" || o.COURSE_ROOT_ID == "1198" || o.COURSE_ROOT_ID == "1464");
                    }

                    var unreadcount = CMSHandler.FetchData<CHAT_UNREAD_COUNT>(
                        new { STUDENT_ID = sUserId },
                        SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.FetchUnreadMessageCount)
                    );

                    var unreadCountsList = (List<CHAT_UNREAD_COUNT>)unreadcount.DataSource.Data;

                    if (liRegularCourses != null && unreadCountsList != null)
                    {
                        foreach (var course in liRegularCourses)
                        {

                            var unread = unreadCountsList.FirstOrDefault(u => u.COURSE_ID == course.COURSE_ROOT_ID);

                            if (unread != null)
                            {
                                course.UNREADCOUNT = unread.UNREADCOUNT;
                            }
                            else
                            {
                                course.UNREADCOUNT = "0";
                            }
                        }
                    }


                    ObjViewModel.liExamRegistrationListChatRooms = liRegularCourses;
                }
                catch (Exception ex)
                {
                    using (ErrorLog ObjLog = new ErrorLog())
                    {
                        ObjLog.WriteError("LMS", "BindCourseChatRooms", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }


            return View(ObjViewModel);
        }


        /*public ActionResult CourseChat(string courseId, string courseName, string courseCode)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserName = (Session[Common.SESSION_VARIABLES.USERNAME] != null)
                ? Session[Common.SESSION_VARIABLES.USERNAME].ToString() : string.Empty;
            string sUserPhoto = (Session[Common.SESSION_VARIABLES.PHOTO] != null)
                ? Session[Common.SESSION_VARIABLES.PHOTO].ToString() : string.Empty;

            if (string.IsNullOrEmpty(sUserId))
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            
            ViewBag.CourseId = courseId;
            ViewBag.CourseName = courseName;
            ViewBag.CourseCode = courseCode;
            ViewBag.UserId = sUserId;
            ViewBag.UserName = sUserName;
            ViewBag.UserPhoto = sUserPhoto;

            return View();
        }*/


        public ActionResult ChatRoom(string courseId, string courseName, string courseCode)
        {

            TempData["ChatCourseId"] = courseId;
            TempData["ChatCourseName"] = courseName;
            TempData["ChatCourseCode"] = courseCode;

            return RedirectToAction("CourseChat");
        }

        public ActionResult CourseChat()
        {

            if (TempData["ChatCourseId"] == null)
            {

                return RedirectToAction("BindCourseChatRooms");
            }


            string courseId = TempData["ChatCourseId"]?.ToString();
            string courseName = TempData["ChatCourseName"]?.ToString();
            string courseCode = TempData["ChatCourseCode"]?.ToString();




            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            string sUserName = (Session[Common.SESSION_VARIABLES.USERNAME] != null) ? Session[Common.SESSION_VARIABLES.USERNAME].ToString() : string.Empty;
            string sUserPhoto = (Session[Common.SESSION_VARIABLES.PHOTO] != null) ? Session[Common.SESSION_VARIABLES.PHOTO].ToString() : string.Empty;
            string sUserType = Session[Common.SESSION_VARIABLES.USER_TYPE]?.ToString();

            if (string.IsNullOrEmpty(sUserId) || string.IsNullOrEmpty(sAcademicYear))
            {
                return RedirectToAction("ErrorMessage", "Error");
            }

            string chatRoomId = "0";
            var historicalMessages = new List<CHAT_MESSAGE>();
            STUDENT_CHAT_CONTEXT studentContext = null;

            try
            {
                if (sUserType == "5")
                {
                    var result = CMSHandler.FetchData<STUDENT_CHAT_CONTEXT>(
                        new { STUDENT_ID = sUserId, ACADEMIC_YEAR = sAcademicYear },
                        SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.GetStudentContextForChat)
                    );

                    var contextData = result?.DataSource?.Data as List<STUDENT_CHAT_CONTEXT>;
                    studentContext = contextData?.FirstOrDefault();
                }
                else if (sUserType == "4")
                {
                    var result = CMSHandler.FetchData<STUDENT_CHAT_CONTEXT>(
                        new { STAFF_ID = sUserId, COURSEID = courseId, ACADEMIC_YEAR = sAcademicYear },
                        SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.FetchClassDetailsForChatContext)
                    );

                    var contextData = result?.DataSource?.Data as List<STUDENT_CHAT_CONTEXT>;
                    studentContext = contextData?.FirstOrDefault();
                }




                if (studentContext != null)
                {
                    var chatRoomData = CMSHandler.FetchData<CHAT_ROOM>(
                            new
                            {
                                CHATTYPEID = "1"
                            },
                            SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.GetChatRoomByDetails)
                        ).DataSource.Data;

                    // 2. Cast the result, THEN call FirstOrDefault()
                    var chatRoom = ((List<CHAT_ROOM>)chatRoomData).FirstOrDefault();

                    if (chatRoom != null && !string.IsNullOrEmpty(chatRoom.CHATROOMID))
                    {
                        chatRoomId = chatRoom.CHATROOMID;

                        historicalMessages = (List<CHAT_MESSAGE>)CMSHandler.FetchData<CHAT_MESSAGE>(
                            new { CHATROOMID = chatRoomId, COURSE_ID = courseId },
                            SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.GetChatMessagesByRoomId)
                        ).DataSource.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog ObjLog = new ErrorLog())
                {
                    ObjLog.WriteError("LMS", "CourseChat", ex.Message);
                }
            }

            ViewBag.ChatRoomId = chatRoomId;
            ViewBag.HistoricalMessages = historicalMessages;
            ViewBag.CourseId = courseId;
            ViewBag.CourseName = courseName;
            ViewBag.CourseCode = courseCode;
            ViewBag.UserId = sUserId;
            ViewBag.UserName = sUserName;
            ViewBag.UserPhoto = sUserPhoto;

            return View();
        }



        public ActionResult ListAllCouseChat()
        {

            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            string sStaffId = (Session[Common.SESSION_VARIABLES.USER_ID] != null) ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;
            var ObjViewModel = new ExaminationViewModel();

            if (!string.IsNullOrEmpty(sAcademicYear) && !string.IsNullOrEmpty(sStaffId))
            {
                try
                {

                    var liStaffCourses = (List<STAFF_COURSE_CHATINFO>)CMSHandler.FetchData<STAFF_COURSE_CHATINFO>(
                        new
                        {
                            STAFF_ID = sStaffId,
                            ACADEMIC_YEAR = sAcademicYear
                        },
                        SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStaffCoursesForChat),
                        sAcademicYear
                    ).DataSource.Data;


                    ObjViewModel.liStaffCourseList = liStaffCourses;
                }
                catch (Exception ex)
                {
                    using (ErrorLog objLog = new ErrorLog())
                    {
                        objLog.WriteError("LMS", "ListAllCouseChat", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }


            return View(ObjViewModel);
        }

     
        public JsonResult UpdateLastRead(int courseId)
        {

            if (Session[Common.SESSION_VARIABLES.USER_ID] == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }
            string sUserId = Session[Common.SESSION_VARIABLES.USER_ID].ToString();
            try
            {
                CMSHandler.SaveOrUpdate(
                    new
                    {
                        CHATROOMID = courseId,
                        USERID = sUserId
                    },
                    SQL.LMS.LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateUserChatStatus)
                );
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                using (ErrorLog objLog = new ErrorLog())
                {
                    objLog.WriteError("LMSController", "UpdateLastRead", ex.Message);
                }
                return Json(new { success = false, message = "An error occurred while updating the read status." });
            }
        }


        public JsonResult SaveAssignmentSettings(string jsonData, HttpPostedFileBase FileUpload)
        {
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null)
                ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;

            string sUserId = (Session[Common.SESSION_VARIABLES.USER_ID] != null)
                ? Session[Common.SESSION_VARIABLES.USER_ID].ToString() : string.Empty;

            var ObjJsonData = new JsonResultData();

            try
            {
                if (string.IsNullOrEmpty(sAcademicYear))
                {
                    ObjJsonData.Message = "Session expired. Please login again.";
                    return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
                }


                var Result = JsonConvert.DeserializeObject<LMS_ASSIGNMENT_SETTING>(jsonData);
                Result.STAFF_ID = sUserId;


                if (FileUpload != null && FileUpload.ContentLength > 0)
                {
                    // Folder: /Uploads/Assignments/
                    string folderVirtual = "/Uploads/Assignments/";
                    string folderPhysical = Server.MapPath(folderVirtual);

                    // Create folder if missing
                    if (!Directory.Exists(folderPhysical))
                        Directory.CreateDirectory(folderPhysical);

                    // Keep original filename (NO encryption)
                    string originalName = Path.GetFileName(FileUpload.FileName);
                    string fullPath = Path.Combine(folderPhysical, originalName);

                    // Save file directly
                    FileUpload.SaveAs(fullPath);

                    // Save file path to database
                    Result.FILE_PATH = folderVirtual + originalName;
                }
                else
                {
                    Result.FILE_PATH = Result.EXISTING_FILE_NAME;
                }
                //string[] studentIds = Result.STUDENT_ID.Split(',');

                //foreach (var stu in studentIds)
                //{

                    //string studentId = stu.Trim();
                    if (!string.IsNullOrEmpty(Result.ASSIGNMENT_SETTING_ID))
                    {
                        var update = CMSHandler.SaveOrUpdate(
                            Result,
                            LMSSQL.GetLMSSQL(LMSSQLCommands.UpdateAssignmentSettingS),
                            sAcademicYear
                        );

                        ObjJsonData.Message = update.Success
                            ? "Assignment updated successfully"
                            : "Failed to update record";
                    }
                    else
                    {
                        var save = CMSHandler.SaveOrUpdate(
                            Result,
                            LMSSQL.GetLMSSQL(LMSSQLCommands.SaveAssignmentSettingS),
                            sAcademicYear
                        );

                        ObjJsonData.Message = save.Success
                            ? "Assignment saved successfully"
                            : "Failed to save record";
                    }
                //}
            }
            catch (Exception ex)
            {
                ObjJsonData.Message = "Error: " + ex.Message;
            }

            return Json(ObjJsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FetchAssignmentSubmissions(string assignmentSettingId)
        {
            string sAcademicYear = Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]?.ToString() ?? string.Empty;
            var objJsonData = new JsonResultData();

            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                var submissions = (List<LMS_STUDENT_ASSIGNMENT_SUBMISSION>)CMSHandler.FetchData<LMS_STUDENT_ASSIGNMENT_SUBMISSION>(
                    new LMS_STUDENT_ASSIGNMENT_SUBMISSION() { ASSIGNMENT_SETTING_ID = assignmentSettingId },
                    LMSSQL.GetLMSSQL(LMSSQLCommands.FetchStudentAssignmentSubmissionBySettingId),
                    sAcademicYear
                ).DataSource.Data;

                if (submissions != null && submissions.Count > 0)
                {
                    return Json(submissions, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objJsonData.Message = "No submissions found.";
                    return Json(objJsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                objJsonData.Message = "Session expired. Please login again.";
                return Json(objJsonData, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion
    }
}