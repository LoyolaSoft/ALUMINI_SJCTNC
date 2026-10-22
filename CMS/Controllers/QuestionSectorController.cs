using CMS.DAO;
using CMS.SQL.SupportData;
using CMS.SQL.OBE;
using CMS.SQL.QuestionSector;
using CMS.Utility;
using CMS.ViewModel.Model;
using CMS.ViewModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Linq;

namespace CMS.Controllers
{
    public class QuestionSectorController : Controller
    {
        // GET: QuestionSector/QuestionUpload
        public ActionResult QuestionUpload()
        {
            try
            {
                using (MySQLHandler objHandler = new MySQLHandler())
                {
                    ViewBag.AcademicYear = objHandler.FetchData(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveAcademicYearList));

                    ViewBag.Department = objHandler.FetchData(null,
                        SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchDepartment));

                    ViewBag.BloomsLevels = objHandler.FetchData(null,
                        OBESQL.GetOBESQL(OBESQLCommands.FetchOBEQuestionLevel));
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("QuestionSectorController", "QuestionUpload", "", ex.Message);
                }
            }
            return View();
        }

        // POST: Save Question Paper
        [HttpPost]
        public JsonResult SaveQuestionPaper(string jsonData)
        {
            ResultArgs result = new ResultArgs();
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    result.Success = false;
                    result.Message = "No data received.";
                    return Json(result);
                }

                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var model = serializer.Deserialize<JSON_QUESTION_PAPER>(jsonData);

                if (model == null || model.PaperHeader == null)
                {
                    result.Success = false;
                    result.Message = "Invalid data.";
                    return Json(result);
                }

                string userId = Convert.ToString(Session[Common.SESSION_VARIABLES.USER_ID]);
                model.PaperHeader.CREATED_BY = userId;
                model.PaperHeader.UPDATED_BY = userId;

                bool isNew = string.IsNullOrEmpty(model.PaperHeader.PAPER_ID)
                             || model.PaperHeader.PAPER_ID == "0";

                string paperSql = isNew
                    ? QuestionSectorSQL.GetQuestionSectorSQL(QuestionSectorSQLCommands.InsertQuestionPaper)
                    : QuestionSectorSQL.GetQuestionSectorSQL(QuestionSectorSQLCommands.UpdateQuestionPaperHeader);

                result = CMSHandler.SaveOrUpdate(model.PaperHeader, paperSql);

                if (result.Success)
                {
                    string paperId = isNew
                        ? Convert.ToString(result.RowUniqueId)
                        : model.PaperHeader.PAPER_ID;

                    if (!isNew)
                    {
                        CMSHandler.SaveOrUpdate(
                            new QS_QUESTION_PAPER { PAPER_ID = paperId },
                            QuestionSectorSQL.GetQuestionSectorSQL(
                                QuestionSectorSQLCommands.DeleteQuestionDetailsByPaperId));
                    }

                    if (model.Questions != null)
                    {
                        foreach (var q in model.Questions)
                        {
                            if (string.IsNullOrWhiteSpace(q.QUESTION_TEXT)) continue;
                            q.PAPER_ID = paperId;
                            CMSHandler.SaveOrUpdate(q,
                                QuestionSectorSQL.GetQuestionSectorSQL(
                                    QuestionSectorSQLCommands.InsertQuestionDetails));
                        }
                    }

                    result.Success = true;
                    result.Message = "Question Paper saved successfully!";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error: " + ex.Message;
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("QuestionSectorController", "SaveQuestionPaper", "", ex.Message);
                }
            }
            return Json(result);
        }

        // AJAX: Exam Settings by Academic Year — returns full row for auto-fill
        public JsonResult GetExamSettings(string academicYearId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySQLHandler objHandler = new MySQLHandler())
                {
                    DataValue dv = new DataValue();
                    dv.Add("ACADEMIC_YEAR_ID", academicYearId, EnumCommand.DataType.Varchar);
                    dt = objHandler.FetchData(dv,
                        "SELECT EXAM_SETTING_ID, EXAM_NAME, EXAM_TYPE, SEMESTER_TYPE, " +
                        "       START_DATE, END_DATE, M_PASS " +
                        "FROM   EXAM_SETTING " +
                        "WHERE  ACADEMIC_YEAR_ID = ?ACADEMIC_YEAR_ID AND IS_DELETED != 1 " +
                        "ORDER BY EXAM_NAME");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                { objlog.WriteError("QuestionSectorController", "GetExamSettings", "", ex.Message); }
            }
            return Json(ToList(dt), JsonRequestBehavior.AllowGet);

        }

        // AJAX: Courses by Department + Semester
        public JsonResult GetCourses(string departmentId, string semesterId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySQLHandler objHandler = new MySQLHandler())
                {
                    DataValue dv = new DataValue();
                    dv.Add("DEPT", departmentId, EnumCommand.DataType.Varchar);
                    dv.Add("SEM",  semesterId,   EnumCommand.DataType.Varchar);
                    dt = objHandler.FetchData(dv,
                        "SELECT COURSE_ROOT_ID, COURSE_CODE, COURSE_TITLE, " +
                        "CONCAT(COURSE_CODE,' - ',COURSE_TITLE) AS COURSE_DISPLAY " +
                        "FROM CP_COURSE_ROOT " +
                        "WHERE DEPARTMENT = ?DEPT AND SEMESTER_ID = ?SEM AND IS_DELETED != 1 " +
                        "ORDER BY COURSE_CODE");
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                { objlog.WriteError("QuestionSectorController", "GetCourses", "", ex.Message); }
            }
            return Json(ToList(dt), JsonRequestBehavior.AllowGet);
        }

        #region Exam Staff Mapping
        
        // GET: QuestionSector/ExamStaffMapping
        public ActionResult ExamStaffMapping()
        {
            OBEViewModel objViewModel = new OBEViewModel();
            string sAcademicYear = (Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR] != null) ? Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(sAcademicYear))
            {
                try
                {
                    var liAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveAcademicYearList)).DataSource.Data;
                    if (liAcademicYear != null && liAcademicYear.Count > 0)
                        objViewModel.AcademicYearList = new SelectList(liAcademicYear, Common.ACDEMIC_YEAR.ACADEMIC_YEAR_ID, Common.ACDEMIC_YEAR.AC_YEAR);

                    var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchAllExamSetting), sAcademicYear).DataSource.Data;
                    if (liExamSetting != null && liExamSetting.Count > 0)
                        objViewModel.ExamSetting = new SelectList(liExamSetting, Common.EXAM_SETTING.EXAM_SETTING_ID, Common.EXAM_SETTING.EXAM_NAME);

                    var liProgramme = (List<ProgrammeInfo>)CMSHandler.FetchData<ProgrammeInfo>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchProgrammeGroupProgramme)).DataSource.Data;
                    if (liProgramme != null && liProgramme.Count > 0)
                        objViewModel.ProgrammeList = new SelectList(liProgramme, Common.CP_PROGRAMME_2017.PROGRAMME_GROUP_ID, Common.CP_PROGRAMME_2017.PROGRAMME_NAME);

                    var liStaff = (List<STF_PERSONAL_INFO>)CMSHandler.FetchData<STF_PERSONAL_INFO>(null, SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchStaff)).DataSource.Data;
                    if (liStaff != null && liStaff.Count > 0)
                        objViewModel.StaffList = new SelectList(liStaff, Common.STF_PERSONAL_INFO.STAFF_ID, Common.STF_PERSONAL_INFO.FIRST_NAME);
                }
                catch (Exception ex)
                {
                    using (ErrorLog objlog = new ErrorLog())
                    {
                        objlog.WriteError("QuestionSectorController", "ExamStaffMapping", "", ex.Message);
                    }
                }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Error");
            }
            return View(objViewModel);
        }

        // AJAX: Get Exam Settings by Academic Year
        [HttpPost]
        public JsonResult GetExamSettingByAcademicYear(string acYearId)
        {
            try
            {
                if (string.IsNullOrEmpty(acYearId)) return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);

                // Fetch the AC_YEAR string for the given ID
                var liAcademicYear = (List<AcademicYearModel>)CMSHandler.FetchData<AcademicYearModel>(new AcademicYearModel { ACADEMIC_YEAR_ID = acYearId }, 
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchActiveAcademicYearList)).DataSource.Data;
                
                var liExamSetting = (List<EXAM_SETTING>)CMSHandler.FetchData<EXAM_SETTING>(null, 
                    SupportDataSQL.GetSupportDataSQL(SupportDataSQLCommands.FetchExamSetting), acYearId).DataSource.Data;

                var result = new List<SelectListItem>();
                if (liExamSetting != null && liExamSetting.Count > 0)
                {
                    result = liExamSetting.Select(x => new SelectListItem
                    {
                        Value = x.EXAM_SETTING_ID,
                        Text = x.EXAM_NAME
                    }).ToList();
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                { objlog.WriteError("QuestionSectorController", "GetExamSettingByAcademicYear", "", ex.Message); }
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // AJAX: Get Courses by Multiple Majors
        [HttpPost]
        public JsonResult GetCoursesByMajors(string majorIds, string examSettingId)
        {
            DataTable dt = new DataTable();
            try
            {
                if (string.IsNullOrEmpty(majorIds)) return Json(new List<object>(), JsonRequestBehavior.AllowGet);

                string sAcyear = Convert.ToString(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]);
                
                using (MySQLHandler objHandler = new MySQLHandler())
                {
                    // Basic SQL to fetch courses for selected majors
                    // We also join with existing mapping if any
                    string sql = @"SELECT cr.COURSE_ROOT_ID, cr.COURSE_CODE, cr.COURSE_TITLE, p.PROGRAMME_NAME as MAJOR_NAME,
                                         m.STAFF_ID, m.IS_LOCK
                                  FROM   CP_COURSE_ROOT cr
                                  JOIN   CP_PROGRAMME_2017 p ON cr.PROGRAMME_ID = p.PROGRAMME_ID
                                  LEFT JOIN QS_EXAM_STAFF_MAPPING m ON cr.COURSE_ROOT_ID = m.COURSE_ID 
                                         AND m.EXAM_SETTING_ID = '" + examSettingId + @"'
                                  WHERE  cr.PROGRAMME_ID IN (" + majorIds + @") 
                                  AND    cr.IS_DELETED != 1
                                  ORDER BY p.PROGRAMME_NAME, cr.COURSE_CODE";
                    
                    dt = objHandler.FetchData(null, sql);
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog objlog = new ErrorLog())
                { objlog.WriteError("QuestionSectorController", "GetCoursesByMajors", "", ex.Message); }
            }
            return Json(ToList(dt), JsonRequestBehavior.AllowGet);
        }

        // POST: Save Staff Mapping
        [HttpPost]
        public JsonResult SaveStaffMapping(List<MappingData> mappingList)
        {
            ResultArgs result = new ResultArgs();
            try
            {
                if (mappingList == null || mappingList.Count == 0)
                {
                    result.Success = false;
                    result.Message = "No data to save.";
                    return Json(result);
                }

                string userId = Convert.ToString(Session[Common.SESSION_VARIABLES.USER_ID]);
                string acYear = Convert.ToString(Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR]);

                foreach (var item in mappingList)
                {
                    // Check if mapping exists
                    string checkSql = "SELECT MAPPING_ID FROM QS_EXAM_STAFF_MAPPING WHERE EXAM_SETTING_ID = '" + item.EXAM_SETTING_ID + "' AND COURSE_ID = '" + item.COURSE_ID + "'";
                    
                    using (MySQLHandler objHandler = new MySQLHandler())
                    {
                        var dtCheck = objHandler.FetchData(null, checkSql);
                        if (dtCheck != null && dtCheck.Rows.Count > 0)
                        {
                            // Update
                            string updateSql = @"UPDATE QS_EXAM_STAFF_MAPPING 
                                                SET    STAFF_ID = '" + item.STAFF_ID + @"', MODIFIED_BY = '" + userId + @"', MODIFIED_DATE = CURRENT_TIMESTAMP 
                                                WHERE  EXAM_SETTING_ID = '" + item.EXAM_SETTING_ID + @"' AND COURSE_ID = '" + item.COURSE_ID + @"' AND IS_LOCK != 1";
                            objHandler.ExecuteAsScripts(updateSql);
                        }
                        else
                        {
                            // Insert
                            string insertSql = @"INSERT INTO QS_EXAM_STAFF_MAPPING (EXAM_SETTING_ID, AC_YEAR, STAFF_ID, COURSE_ID, CREATED_BY, MODIFIED_BY)
                                                VALUES ('" + item.EXAM_SETTING_ID + "', '" + acYear + "', '" + item.STAFF_ID + "', '" + item.COURSE_ID + "', '" + userId + "', '" + userId + "')";
                            objHandler.ExecuteAsScripts(insertSql);
                        }
                    }
                }

                result.Success = true;
                result.Message = "Staff mapping saved successfully!";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error: " + ex.Message;
                using (ErrorLog objlog = new ErrorLog())
                {
                    objlog.WriteError("QuestionSectorController", "SaveStaffMapping", "", ex.Message);
                }
            }
            return Json(result);
        }

        public class MappingData
        {
            public string EXAM_SETTING_ID { get; set; }
            public string COURSE_ID { get; set; }
            public string STAFF_ID { get; set; }
        }

        #endregion

        private List<Dictionary<string, object>> ToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            if (dt == null) return list;
            foreach (DataRow dr in dt.Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                    row[col.ColumnName] = dr[col];
                list.Add(row);
            }
            return list;
        }
    }
}
