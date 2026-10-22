using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CMS.DAO;
using CMS.Utility;
using CMS.ViewModel.Model;
using MySql.Data.MySqlClient;
using OfficeOpenXml;

namespace CMS.Services
{
    public class AttendanceMarkHelper
    {
        /// <summary>
        /// Reads Excel stream and upserts attendance marks into att_attendance_mark_upload.
        /// </summary>
        public static bool UploadAttendanceMarks(Stream fileStream, string academicYear, string semesterType, string userId,
            out int totalCount, out int successCount, out List<string> notFoundRollNos, out string errorMessage)
        {
            totalCount = 0;
            successCount = 0;
            notFoundRollNos = new List<string>();
            errorMessage = string.Empty;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(fileStream))
                {
                    var ws = package.Workbook.Worksheets.FirstOrDefault();
                    if (ws == null || ws.Dimension == null || ws.Dimension.Rows < 2)
                    {
                        errorMessage = "The uploaded Excel sheet contains no data rows.";
                        return false;
                    }

                    int rollCol = -1;
                    int markCol = -1;
                    int rowCount = ws.Dimension.Rows;
                    int colCount = ws.Dimension.Columns;

                    for (int col = 1; col <= colCount; col++)
                    {
                        object val = ws.Cells[1, col].Value;
                        string header = (val != null) ? val.ToString().Trim().ToUpper() : string.Empty;
                        if (header == "ROLL_NO" || header == "ROLL NO" || header == "ROLLNO" || header == "REG_NO" || header == "REGNO" || header == "REG.NO")
                        {
                            rollCol = col;
                        }
                        else if (header == "MARK" || header == "MARKS" || header == "ATTENDANCE_MARK" || header == "ATTENDANCE MARK" || header == "ATT_MARK")
                        {
                            markCol = col;
                        }
                    }

                    if (rollCol == -1) rollCol = 1;
                    if (markCol == -1) markCol = 2;

                    var rawList = new List<Tuple<string, decimal>>();
                    for (int r = 2; r <= rowCount; r++)
                    {
                        object rVal = ws.Cells[r, rollCol].Value;
                        object mVal = ws.Cells[r, markCol].Value;
                        string rollNo = (rVal != null) ? rVal.ToString().Trim() : string.Empty;
                        string markStr = (mVal != null) ? mVal.ToString().Trim() : string.Empty;

                        if (string.IsNullOrEmpty(rollNo)) continue;

                        decimal mark = 0;
                        if (!decimal.TryParse(markStr, out mark))
                        {
                            mark = 0;
                        }

                        rawList.Add(Tuple.Create(rollNo, mark));
                    }

                    if (rawList.Count == 0)
                    {
                        errorMessage = "No student roll number entries found in the Excel file.";
                        return false;
                    }

                    totalCount = rawList.Count;

                    // Group by RollNo to take the latest entry if duplicates exist in sheet
                    var distinctEntries = rawList
                        .GroupBy(x => x.Item1.ToUpper())
                        .ToDictionary(g => g.Key, g => g.Last().Item2);

                    var allRolls = distinctEntries.Keys.ToList();

                    // Batch lookup student IDs from stu_personal_info
                    var studentMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    // Process in chunks of 500 to avoid huge IN clauses
                    for (int i = 0; i < allRolls.Count; i += 500)
                    {
                        var chunk = allRolls.Skip(i).Take(500).ToList();
                        var paramNames = new List<string>();
                        for (int idx = 0; idx < chunk.Count; idx++)
                        {
                            paramNames.Add("@r" + idx);
                        }
                        string inClause = string.Join(",", paramNames.ToArray());
                        string sql = string.Format("SELECT STUDENT_ID, ROLL_NO FROM STU_PERSONAL_INFO WHERE UPPER(ROLL_NO) IN ({0}) AND IS_DELETED <> 1;", inClause);

                        using (var conn = new MySqlConnection(MySQLHandler.CurrentConnectionString))
                        {
                            conn.Open();
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                for (int idx = 0; idx < chunk.Count; idx++)
                                {
                                    cmd.Parameters.AddWithValue("@r" + idx, chunk[idx]);
                                }

                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string sId = Convert.ToString(reader["STUDENT_ID"]);
                                        string rNo = Convert.ToString(reader["ROLL_NO"]);
                                        if (!string.IsNullOrEmpty(rNo) && !studentMap.ContainsKey(rNo))
                                        {
                                            studentMap[rNo] = sId;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Check which roll numbers were not found
                    foreach (var roll in allRolls)
                    {
                        if (!studentMap.ContainsKey(roll))
                        {
                            notFoundRollNos.Add(roll);
                        }
                    }

                    // Upsert valid students into att_attendance_mark_upload
                    using (var conn = new MySqlConnection(MySQLHandler.CurrentConnectionString))
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction())
                        {
                            try
                            {
                                string upsertSql = @"
                                    INSERT INTO att_attendance_mark_upload 
                                        (STUDENT_ID, ROLL_NO, ATTENDANCE_MARK, ACADEMIC_YEAR, SEMESTER_TYPE, IS_DELETED, CREATED_BY, CREATED_DATE)
                                    VALUES 
                                        (@STUDENT_ID, @ROLL_NO, @ATTENDANCE_MARK, @ACADEMIC_YEAR, @SEMESTER_TYPE, 0, @USER_ID, NOW())
                                    ON DUPLICATE KEY UPDATE 
                                        ATTENDANCE_MARK = VALUES(ATTENDANCE_MARK),
                                        IS_DELETED = 0,
                                        UPDATED_BY = @USER_ID,
                                        UPDATED_DATE = NOW();";

                                string studentId = string.Empty;
                                foreach (var kvp in distinctEntries)
                                {
                                    if (studentMap.TryGetValue(kvp.Key, out studentId))
                                    {
                                        using (var cmd = conn.CreateCommand())
                                        {
                                            cmd.Transaction = trans;
                                            cmd.CommandText = upsertSql;
                                            cmd.Parameters.AddWithValue("@STUDENT_ID", studentId);
                                            cmd.Parameters.AddWithValue("@ROLL_NO", kvp.Key);
                                            cmd.Parameters.AddWithValue("@ATTENDANCE_MARK", kvp.Value);
                                            cmd.Parameters.AddWithValue("@ACADEMIC_YEAR", academicYear.Trim());
                                            cmd.Parameters.AddWithValue("@SEMESTER_TYPE", semesterType.Trim());
                                            cmd.Parameters.AddWithValue("@USER_ID", userId ?? "");
                                            cmd.ExecuteNonQuery();
                                        }
                                        successCount++;
                                    }
                                }

                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                throw ex;
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("AttendanceMarkHelper", "UploadAttendanceMarks", ex.Message);
                }
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Fetches uploaded attendance marks from att_attendance_mark_upload.
        /// </summary>
        public static List<ATT_ATTENDANCE_MARK_UPLOAD_ITEM> FetchUploadedMarks(string academicYear, string semesterType)
        {
            var list = new List<ATT_ATTENDANCE_MARK_UPLOAD_ITEM>();
            try
            {
                string sql = @"
                    SELECT 
                        u.ATT_MARK_UPLOAD_ID,
                        u.STUDENT_ID,
                        u.ROLL_NO,
                        u.ATTENDANCE_MARK,
                        u.ACADEMIC_YEAR,
                        u.SEMESTER_TYPE,
                        DATE_FORMAT(u.CREATED_DATE, '%d/%m/%Y %h:%i %p') AS CREATED_DATE_DISPLAY,
                        CONCAT(IFNULL(sp.FIRST_NAME,''), ' ', IFNULL(sp.LAST_NAME,'')) AS STUDENT_NAME,
                        IFNULL(c.CLASS_NAME, '') AS CLASS_NAME
                    FROM att_attendance_mark_upload u
                    INNER JOIN stu_personal_info sp ON sp.STUDENT_ID = u.STUDENT_ID AND sp.IS_DELETED != 1
                    LEFT JOIN stu_class sc ON sc.STUDENT_ID = u.STUDENT_ID AND sc.ACADEMIC_YEAR = u.ACADEMIC_YEAR
                    LEFT JOIN cp_classes c ON c.CLASS_ID = sc.CLASS_ID
                    WHERE u.IS_DELETED != 1
                      AND (@AC_YEAR IS NULL OR @AC_YEAR = '' OR u.ACADEMIC_YEAR = @AC_YEAR)
                      AND (@SEM_TYPE IS NULL OR @SEM_TYPE = '' OR u.SEMESTER_TYPE = @SEM_TYPE)
                    ORDER BY u.ATT_MARK_UPLOAD_ID DESC;";

                var result = DapperHandler.FetchAll<ATT_ATTENDANCE_MARK_UPLOAD_ITEM>(sql, new
                {
                    AC_YEAR = string.IsNullOrWhiteSpace(academicYear) ? null : academicYear.Trim(),
                    SEM_TYPE = string.IsNullOrWhiteSpace(semesterType) ? null : semesterType.Trim()
                });
                if (result != null)
                {
                    list = result;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("AttendanceMarkHelper", "FetchUploadedMarks", ex.Message);
                }
            }
            return list;
        }

        /// <summary>
        /// Fetches active exam settings for dropdown.
        /// </summary>
        public static List<EXAM_SETTING_DROPDOWN_ITEM> GetExamSettingsByAcYear(string academicYear)
        {
            var list = new List<EXAM_SETTING_DROPDOWN_ITEM>();
            try
            {
                string sql = @"
                    SELECT 
                        EXAM_SETTING_ID,
                        EXAM_NAME,
                        ACADEMIC_YEAR,
                        SEMESTER_TYPE,
                        IS_ACTIVE
                    FROM EXAM_SETTING
                    WHERE IS_DELETED <> 1
                      AND (@AC_YEAR IS NULL OR @AC_YEAR = '' OR @AC_YEAR = 'ALL' OR ACADEMIC_YEAR = @AC_YEAR)
                    ORDER BY EXAM_SETTING_ID DESC;";

                var result = DapperHandler.FetchAll<EXAM_SETTING_DROPDOWN_ITEM>(sql, new
                {
                    AC_YEAR = string.IsNullOrWhiteSpace(academicYear) ? null : academicYear.Trim()
                });
                if (result != null)
                {
                    list = result;
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("AttendanceMarkHelper", "GetExamSettingsByAcYear", ex.Message);
                }
            }
            return list;
        }

        /// <summary>
        /// Pushes uploaded attendance marks to cia_group_marks_{year} for all registered courses
        /// in that exam setting that have an Attendance component (UPPER(COMPONENT) = 'ATTENDANCE').
        /// If a CIA record exists, UPDATES it; if not, INSERTS it.
        /// </summary>
        public static bool PushAttendanceMarksToCIA(string academicYear, int examSettingId, string userId,
            out int updatedCount, out int insertedCount, out string errorMessage)
        {
            updatedCount = 0;
            insertedCount = 0;
            errorMessage = string.Empty;

            try
            {
                // 1. Fetch Exam Setting details to verify target academic year & semester type
                string settingSql = @"
                    SELECT EXAM_SETTING_ID, EXAM_NAME, ACADEMIC_YEAR, SEMESTER_TYPE 
                    FROM EXAM_SETTING 
                    WHERE EXAM_SETTING_ID = @SETTING_ID AND IS_DELETED <> 1 LIMIT 1;";

                var settings = DapperHandler.FetchAll<dynamic>(settingSql, new { SETTING_ID = examSettingId });
                dynamic setting = (settings != null && settings.Count > 0) ? settings.FirstOrDefault() : null;
                if (setting == null)
                {
                    errorMessage = "Exam Setting with ID " + examSettingId + " not found.";
                    return false;
                }

                string targetYear = Convert.ToString(setting.ACADEMIC_YEAR);
                if (string.IsNullOrWhiteSpace(targetYear))
                {
                    targetYear = academicYear;
                }
                targetYear = (targetYear ?? "").Trim();
                if (targetYear.Length > 4 && targetYear.Contains("-"))
                {
                    targetYear = targetYear.Split('-')[0].Trim();
                }

                string updateSql = string.Format(@"
                    UPDATE cia_group_marks_{0} AS c
                    INNER JOIN exam_registration_{0} AS er ON er.STUDENT_ID = c.STUDENT_ID 
                        AND er.COURSE_ID = c.COURSE_ID 
                        AND er.EXAM_SETTING_ID = @EXAM_SETTING_ID 
                        AND er.IS_DELETED != 1
                    INNER JOIN exam_setting AS es ON es.EXAM_SETTING_ID = er.EXAM_SETTING_ID
                    INNER JOIN cp_course_root AS cr ON cr.COURSE_ROOT_ID = er.COURSE_ID AND cr.IS_DELETED != 1
                    INNER JOIN cp_course_type_group AS ctg ON ctg.COURSE_TYPE_ID = cr.COURSE_TYPE
                    INNER JOIN cia_group_component AS cgc ON cgc.CIA_GROUP_ID = ctg.GROUP_ID 
                        AND cgc.CIA_GROUP_COMPONENT_ID = c.COURSE_GROUP_ID 
                        AND cgc.IS_DELETED != 1
                    INNER JOIN sup_cia_components AS scc ON scc.COMPONENT_ID = cgc.SUP_CIA_COMPONENT_ID 
                        AND UPPER(TRIM(scc.COMPONENT)) = 'ATTENDANCE'
                    INNER JOIN att_attendance_mark_upload AS att ON att.STUDENT_ID = er.STUDENT_ID 
                        AND (att.ACADEMIC_YEAR = @AC_YEAR OR att.ACADEMIC_YEAR = @TARGET_YEAR)
                        AND (
                            att.SEMESTER_TYPE = CAST(es.SEMESTER_TYPE AS CHAR)
                            OR (es.SEMESTER_TYPE = 1 AND UPPER(att.SEMESTER_TYPE) IN ('ODD', '1'))
                            OR (es.SEMESTER_TYPE = 2 AND UPPER(att.SEMESTER_TYPE) IN ('EVEN', '2'))
                            OR es.SEMESTER_TYPE = 0
                        )
                        AND att.IS_DELETED != 1
                    SET c.COURSE_GROUP_MARK = LEAST(att.ATTENDANCE_MARK, cgc.MAX_MARK),
                        c.ENTRY_ID = @ENTRY_ID
                    WHERE c.COURSE_GROUP_ID = cgc.CIA_GROUP_COMPONENT_ID;", targetYear);

                string insertSql = string.Format(@"
                    INSERT INTO cia_group_marks_{0} (STUDENT_ID, COURSE_ID, COURSE_GROUP_ID, COURSE_GROUP_MARK, SEMESTER_ID, ENTRY_ID)
                    SELECT 
                        er.STUDENT_ID,
                        er.COURSE_ID,
                        cgc.CIA_GROUP_COMPONENT_ID,
                        LEAST(att.ATTENDANCE_MARK, cgc.MAX_MARK),
                        cr.SEMESTER_ID,
                        @ENTRY_ID
                    FROM exam_registration_{0} AS er
                    INNER JOIN exam_setting AS es ON es.EXAM_SETTING_ID = er.EXAM_SETTING_ID
                    INNER JOIN cp_course_root AS cr ON cr.COURSE_ROOT_ID = er.COURSE_ID AND cr.IS_DELETED != 1
                    INNER JOIN cp_course_type_group AS ctg ON ctg.COURSE_TYPE_ID = cr.COURSE_TYPE
                    INNER JOIN cia_group_component AS cgc ON cgc.CIA_GROUP_ID = ctg.GROUP_ID AND cgc.IS_DELETED != 1
                    INNER JOIN sup_cia_components AS scc ON scc.COMPONENT_ID = cgc.SUP_CIA_COMPONENT_ID 
                        AND UPPER(TRIM(scc.COMPONENT)) = 'ATTENDANCE'
                    INNER JOIN att_attendance_mark_upload AS att ON att.STUDENT_ID = er.STUDENT_ID 
                        AND (att.ACADEMIC_YEAR = @AC_YEAR OR att.ACADEMIC_YEAR = @TARGET_YEAR)
                        AND (
                            att.SEMESTER_TYPE = CAST(es.SEMESTER_TYPE AS CHAR)
                            OR (es.SEMESTER_TYPE = 1 AND UPPER(att.SEMESTER_TYPE) IN ('ODD', '1'))
                            OR (es.SEMESTER_TYPE = 2 AND UPPER(att.SEMESTER_TYPE) IN ('EVEN', '2'))
                            OR es.SEMESTER_TYPE = 0
                        )
                        AND att.IS_DELETED != 1
                    LEFT JOIN cia_group_marks_{0} AS c ON c.STUDENT_ID = er.STUDENT_ID 
                        AND c.COURSE_ID = er.COURSE_ID 
                        AND c.COURSE_GROUP_ID = cgc.CIA_GROUP_COMPONENT_ID
                    WHERE er.EXAM_SETTING_ID = @EXAM_SETTING_ID 
                      AND er.IS_DELETED != 1
                      AND c.CIA_GROUP_MARK_ID IS NULL;", targetYear);

                using (var conn = new MySqlConnection(MySQLHandler.CurrentConnectionString))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Run UPDATE
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.Transaction = trans;
                                cmd.CommandText = updateSql;
                                cmd.CommandTimeout = 300;
                                cmd.Parameters.AddWithValue("@EXAM_SETTING_ID", examSettingId);
                                cmd.Parameters.AddWithValue("@AC_YEAR", academicYear);
                                cmd.Parameters.AddWithValue("@TARGET_YEAR", targetYear);
                                cmd.Parameters.AddWithValue("@ENTRY_ID", userId ?? "0");
                                updatedCount = cmd.ExecuteNonQuery();
                            }

                            // 2. Run INSERT
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.Transaction = trans;
                                cmd.CommandText = insertSql;
                                cmd.CommandTimeout = 300;
                                cmd.Parameters.AddWithValue("@EXAM_SETTING_ID", examSettingId);
                                cmd.Parameters.AddWithValue("@AC_YEAR", academicYear);
                                cmd.Parameters.AddWithValue("@TARGET_YEAR", targetYear);
                                cmd.Parameters.AddWithValue("@ENTRY_ID", userId ?? "0");
                                insertedCount = cmd.ExecuteNonQuery();
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("AttendanceMarkHelper", "PushAttendanceMarksToCIA", ex.Message);
                }
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Generates a sample Excel template for Attendance Mark upload.
        /// </summary>
        public static byte[] GenerateTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Attendance_Marks");

                // Headers
                ws.Cells[1, 1].Value = "ROLL_NO";
                ws.Cells[1, 2].Value = "MARK";

                // Sample rows
                ws.Cells[2, 1].Value = "21UCA01";
                ws.Cells[2, 2].Value = 5;

                ws.Cells[3, 1].Value = "21UCA02";
                ws.Cells[3, 2].Value = 4;

                ws.Cells[4, 1].Value = "21UCA03";
                ws.Cells[4, 2].Value = 3;

                // Styling
                using (var range = ws.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                }

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 15;

                return package.GetAsByteArray();
            }
        }
    }
}
