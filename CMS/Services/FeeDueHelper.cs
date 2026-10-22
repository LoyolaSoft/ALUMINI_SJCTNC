using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CMS.DAO;
using CMS.Utility;
using CMS.ViewModel.Model;

namespace CMS.Services
{
    public class FeeDueHelper
    {
        /// <summary>
        /// Calculates itemized fee dues for a student (College Term 1 & 2, Previous/Arrear Year Term dues, Hostel dues, and Bus dues).
        /// Matches the calculation logic from /Fee/FeeStudentDueDetails.
        /// </summary>
        public static StudentFeeDueSummary GetStudentFeeDueSummary(string studentIdOrRollNo, string academicYear)
        {
            var summary = new StudentFeeDueSummary
            {
                StudentId = studentIdOrRollNo,
                AcademicYear = academicYear,
                DueItems = new List<ItemizedDueDetail>()
            };

            if (string.IsNullOrWhiteSpace(studentIdOrRollNo))
            {
                return summary;
            }

            try
            {
                // 1. Resolve student info
                string studentSql = @"
                    SELECT 
                        SPI.STUDENT_ID,
                        SPI.ROLL_NO,
                        SPI.REGISTER_NO,
                        CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, '')) AS FULL_NAME
                    FROM STU_PERSONAL_INFO SPI
                    WHERE (SPI.STUDENT_ID = @STU_ID OR SPI.ROLL_NO = @STU_ID OR SPI.REGISTER_NO = @STU_ID)
                      AND SPI.IS_DELETED <> 1
                    LIMIT 1;";

                var students = DapperHandler.FetchAll<dynamic>(studentSql, new { STU_ID = studentIdOrRollNo.Trim() });
                var student = students?.FirstOrDefault();
                if (student == null)
                {
                    return summary;
                }

                string studentId = Convert.ToString(student.STUDENT_ID);
                summary.StudentId = studentId;
                summary.RollNo = Convert.ToString(student.ROLL_NO);
                summary.StudentName = Convert.ToString(student.FULL_NAME);

                // 2. Fetch current academic year fee dues from STU_FEE_DUE_INFO
                string curDueSql = @"
                    SELECT 
                        CUR.STU_DUE_ID,
                        CUR.ACADEMIC_YEAR,
                        IFNULL(CUR.TERMFEE1, 0) AS TERMFEE1,
                        IFNULL(CUR.TERMFEE1PAID, 0) AS TERMFEE1PAID,
                        GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0), 0) AS TERMFEE1DUE,
                        IFNULL(CUR.TERMFEE2, 0) AS TERMFEE2,
                        IFNULL(CUR.TERMFEE2PAID, 0) AS TERMFEE2PAID,
                        GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0), 0) AS TERMFEE2DUE,
                        IFNULL(CUR.HOSTELFEE, 0) AS HOSTELFEE,
                        IFNULL(CUR.HOSTELFEEPAID, 0) AS HOSTELFEEPAID,
                        GREATEST(IFNULL(CUR.HOSTELFEE, 0) - IFNULL(CUR.HOSTELFEEPAID, 0), 0) AS HOSTELFEEDUE,
                        IFNULL(CUR.BUSFEE, 0) AS BUSFEE,
                        IFNULL(CUR.BUSFEEPAID, 0) AS BUSFEEPAID,
                        GREATEST(IFNULL(CUR.BUSFEE, 0) - IFNULL(CUR.BUSFEEPAID, 0), 0) AS BUSFEEDUE
                    FROM STU_FEE_DUE_INFO CUR
                    WHERE CUR.STUDENT_ID = @STUDENT_ID
                      AND CUR.ACADEMIC_YEAR = @AC_YEAR
                      AND CUR.IS_DELETED <> 1
                    LIMIT 1;";

                var curDueList = DapperHandler.FetchAll<dynamic>(curDueSql, new { STUDENT_ID = studentId, AC_YEAR = academicYear });
                var curDue = curDueList?.FirstOrDefault();

                decimal curTerm1Due = 0;
                decimal curTerm2Due = 0;
                decimal curHostelDueFromDueInfo = 0;
                decimal curBusDueFromDueInfo = 0;

                if (curDue != null)
                {
                    decimal.TryParse(Convert.ToString(curDue.TERMFEE1DUE), out curTerm1Due);
                    decimal.TryParse(Convert.ToString(curDue.TERMFEE2DUE), out curTerm2Due);
                    decimal.TryParse(Convert.ToString(curDue.HOSTELFEEDUE), out curHostelDueFromDueInfo);
                    decimal.TryParse(Convert.ToString(curDue.BUSFEEDUE), out curBusDueFromDueInfo);

                    summary.Term1Due = curTerm1Due;
                    summary.Term2Due = curTerm2Due;

                    if (curTerm1Due > 0)
                    {
                        decimal t1Total = 0, t1Paid = 0;
                        decimal.TryParse(Convert.ToString(curDue.TERMFEE1), out t1Total);
                        decimal.TryParse(Convert.ToString(curDue.TERMFEE1PAID), out t1Paid);

                        summary.DueItems.Add(new ItemizedDueDetail
                        {
                            FeeCategory = "College Fee",
                            Particulars = "College Term 1 Fee (" + academicYear + ")",
                            AllocatedAmount = t1Total,
                            PaidAmount = t1Paid,
                            OutstandingAmount = curTerm1Due,
                            Status = "Unpaid"
                        });
                    }

                    if (curTerm2Due > 0)
                    {
                        decimal t2Total = 0, t2Paid = 0;
                        decimal.TryParse(Convert.ToString(curDue.TERMFEE2), out t2Total);
                        decimal.TryParse(Convert.ToString(curDue.TERMFEE2PAID), out t2Paid);

                        summary.DueItems.Add(new ItemizedDueDetail
                        {
                            FeeCategory = "College Fee",
                            Particulars = "College Term 2 Fee (" + academicYear + ")",
                            AllocatedAmount = t2Total,
                            PaidAmount = t2Paid,
                            OutstandingAmount = curTerm2Due,
                            Status = "Unpaid"
                        });
                    }
                }

                // 3. Fetch Arrear / Previous Academic Year term fee dues
                string arrearSql = @"
                    SELECT 
                        SFD.ACADEMIC_YEAR,
                        IFNULL(AY.AC_YEAR, SFD.ACADEMIC_YEAR) AS AC_YEAR_DISPLAY,
                        IFNULL(SFD.TERMFEE1, 0) AS TERMFEE1,
                        IFNULL(SFD.TERMFEE1PAID, 0) AS TERMFEE1PAID,
                        GREATEST(IFNULL(SFD.TERMFEE1, 0) - IFNULL(SFD.TERMFEE1PAID, 0), 0) AS TERMFEE1DUE,
                        IFNULL(SFD.TERMFEE2, 0) AS TERMFEE2,
                        IFNULL(SFD.TERMFEE2PAID, 0) AS TERMFEE2PAID,
                        GREATEST(IFNULL(SFD.TERMFEE2, 0) - IFNULL(SFD.TERMFEE2PAID, 0), 0) AS TERMFEE2DUE
                    FROM STU_FEE_DUE_INFO SFD
                    LEFT JOIN ACADEMIC_YEAR AY ON AY.ACADEMIC_YEAR_ID = SFD.ACADEMIC_YEAR
                    WHERE SFD.STUDENT_ID = @STUDENT_ID
                      AND SFD.ACADEMIC_YEAR < @AC_YEAR
                      AND SFD.IS_DELETED <> 1
                    ORDER BY SFD.ACADEMIC_YEAR ASC;";

                var arrearList = DapperHandler.FetchAll<dynamic>(arrearSql, new { STUDENT_ID = studentId, AC_YEAR = academicYear });
                decimal totalArrearDue = 0;
                if (arrearList != null)
                {
                    foreach (var item in arrearList)
                    {
                        decimal arrT1Due = 0, arrT2Due = 0;
                        decimal.TryParse(Convert.ToString(item.TERMFEE1DUE), out arrT1Due);
                        decimal.TryParse(Convert.ToString(item.TERMFEE2DUE), out arrT2Due);
                        string acDisplay = Convert.ToString(item.AC_YEAR_DISPLAY);

                        if (arrT1Due > 0)
                        {
                            decimal tot = 0, pd = 0;
                            decimal.TryParse(Convert.ToString(item.TERMFEE1), out tot);
                            decimal.TryParse(Convert.ToString(item.TERMFEE1PAID), out pd);
                            totalArrearDue += arrT1Due;

                            summary.DueItems.Add(new ItemizedDueDetail
                            {
                                FeeCategory = "Old / Arrear Fee",
                                Particulars = "Previous Fee - Term 1 (" + acDisplay + ")",
                                AllocatedAmount = tot,
                                PaidAmount = pd,
                                OutstandingAmount = arrT1Due,
                                Status = "Unpaid"
                            });
                        }

                        if (arrT2Due > 0)
                        {
                            decimal tot = 0, pd = 0;
                            decimal.TryParse(Convert.ToString(item.TERMFEE2), out tot);
                            decimal.TryParse(Convert.ToString(item.TERMFEE2PAID), out pd);
                            totalArrearDue += arrT2Due;

                            summary.DueItems.Add(new ItemizedDueDetail
                            {
                                FeeCategory = "Old / Arrear Fee",
                                Particulars = "Previous Fee - Term 2 (" + acDisplay + ")",
                                AllocatedAmount = tot,
                                PaidAmount = pd,
                                OutstandingAmount = arrT2Due,
                                Status = "Unpaid"
                            });
                        }
                    }
                }
                summary.OldFeeDue = totalArrearDue;

                // 4. Fetch Hostel Fee Dues from FEE_STUDENT_ACCOUNT (FEE_MODE = '4')
                string hostelSql = @"
                    SELECT 
                        SUM(IFNULL(FSA.CREDIT, 0)) AS HOSTEL_FEE,
                        SUM(IFNULL(FSA.DEBIT, 0)) AS HOSTEL_FEE_PAID,
                        GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS HOSTEL_DUE
                    FROM FEE_STUDENT_ACCOUNT FSA
                    INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID AND FFF.FEE_MODE = '4'
                    WHERE FSA.STUDENT_ID = @STUDENT_ID
                      AND FSA.IS_DELETED <> 1;";

                var hostelList = DapperHandler.FetchAll<dynamic>(hostelSql, new { STUDENT_ID = studentId });
                decimal hostelAccountDue = 0;
                decimal hostelCredit = 0;
                decimal hostelDebit = 0;

                if (hostelList != null && hostelList.Count > 0)
                {
                    decimal.TryParse(Convert.ToString(hostelList[0].HOSTEL_DUE), out hostelAccountDue);
                    decimal.TryParse(Convert.ToString(hostelList[0].HOSTEL_FEE), out hostelCredit);
                    decimal.TryParse(Convert.ToString(hostelList[0].HOSTEL_FEE_PAID), out hostelDebit);
                }

                decimal maxHostelDue = Math.Max(hostelAccountDue, curHostelDueFromDueInfo);
                summary.HostelFeeDue = maxHostelDue;
                if (maxHostelDue > 0)
                {
                    summary.DueItems.Add(new ItemizedDueDetail
                    {
                        FeeCategory = "Hostel Fee",
                        Particulars = "Outstanding Hostel & Mess Fee",
                        AllocatedAmount = (hostelCredit > 0 ? hostelCredit : maxHostelDue),
                        PaidAmount = hostelDebit,
                        OutstandingAmount = maxHostelDue,
                        Status = "Unpaid"
                    });
                }

                // 5. Fetch Bus Fee Dues from FEE_STUDENT_ACCOUNT (FEE_MODE = '8')
                string busSql = @"
                    SELECT 
                        SUM(IFNULL(FSA.CREDIT, 0)) AS BUS_FEE,
                        SUM(IFNULL(FSA.DEBIT, 0)) AS BUS_FEE_PAID,
                        GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS BUS_DUE
                    FROM FEE_STUDENT_ACCOUNT FSA
                    INNER JOIN FEE_FREQUENCY_FEE_MODE FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID AND FFF.FEE_MODE = '8'
                    WHERE FSA.STUDENT_ID = @STUDENT_ID
                      AND FSA.IS_DELETED <> 1;";

                var busList = DapperHandler.FetchAll<dynamic>(busSql, new { STUDENT_ID = studentId });
                decimal busAccountDue = 0;
                decimal busCredit = 0;
                decimal busDebit = 0;

                if (busList != null && busList.Count > 0)
                {
                    decimal.TryParse(Convert.ToString(busList[0].BUS_DUE), out busAccountDue);
                    decimal.TryParse(Convert.ToString(busList[0].BUS_FEE), out busCredit);
                    decimal.TryParse(Convert.ToString(busList[0].BUS_FEE_PAID), out busDebit);
                }

                decimal maxBusDue = Math.Max(busAccountDue, curBusDueFromDueInfo);
                summary.BusFeeDue = maxBusDue;
                if (maxBusDue > 0)
                {
                    summary.DueItems.Add(new ItemizedDueDetail
                    {
                        FeeCategory = "Bus Fee",
                        Particulars = "Outstanding Bus Fee",
                        AllocatedAmount = (busCredit > 0 ? busCredit : maxBusDue),
                        PaidAmount = busDebit,
                        OutstandingAmount = maxBusDue,
                        Status = "Unpaid"
                    });
                }
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "GetStudentFeeDueSummary", ex.Message);
                }
            }

            return summary;
        }

        /// <summary>
        /// Checks if the student has an active, non-deleted permission to pay exam fees.
        /// Matches student_id or roll_no, exam_setting (or 'ALL'), and academic_year (or 'ALL').
        /// </summary>
        public static bool HasActiveExamFeePermission(string studentIdOrRollNo, string examSetting, string academicYear)
        {
            if (string.IsNullOrWhiteSpace(studentIdOrRollNo))
            {
                return false;
            }

            try
            {
                string sql = @"
                    SELECT COUNT(*)
                    FROM exam_fee_permission EFP
                    LEFT JOIN STU_PERSONAL_INFO SPI ON SPI.STUDENT_ID = EFP.student_id
                    WHERE (EFP.student_id = @STU_ID OR SPI.ROLL_NO = @STU_ID OR SPI.REGISTER_NO = @STU_ID)
                      AND EFP.is_active = 1
                      AND EFP.is_deleted = 0
                      AND (EFP.exam_setting = @EXAM_SETTING OR EFP.exam_setting = 'ALL' OR @EXAM_SETTING IS NULL OR @EXAM_SETTING = '')
                      AND (EFP.academic_year = @AC_YEAR OR EFP.academic_year = 'ALL' OR @AC_YEAR IS NULL OR @AC_YEAR = '');";

                var countList = DapperHandler.FetchAll<int>(sql, new
                {
                    STU_ID = studentIdOrRollNo.Trim(),
                    EXAM_SETTING = string.IsNullOrEmpty(examSetting) ? "ALL" : examSetting,
                    AC_YEAR = string.IsNullOrEmpty(academicYear) ? "ALL" : academicYear
                });

                return (countList != null && countList.FirstOrDefault() > 0);
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "HasActiveExamFeePermission", ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Fetches list of exam fee permissions for administrative management.
        /// </summary>
        public static List<EXAM_FEE_PERMISSION> GetPermissions(string academicYear = null, string examSetting = null, string searchTerm = null)
        {
            var list = new List<EXAM_FEE_PERMISSION>();
            try
            {
                string sql = @"
                    SELECT 
                        EFP.exam_fee_id,
                        EFP.student_id,
                        EFP.exam_setting,
                        EFP.academic_year,
                        EFP.remarks,
                        EFP.is_active,
                        EFP.created_by,
                        EFP.created_date,
                        EFP.is_deleted,
                        SPI.ROLL_NO AS roll_no,
                        SPI.REGISTER_NO AS register_no,
                        CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, '')) AS student_name,
                        (SELECT CC.CLASS_NAME 
                         FROM STU_CLASS SC 
                         INNER JOIN CP_CLASSES CC ON CC.CLASS_ID = SC.CLASS_ID 
                         WHERE SC.STUDENT_ID = SPI.STUDENT_ID AND SC.IS_DELETED <> 1 
                         ORDER BY SC.ACADEMIC_YEAR DESC, SC.STU_CLASS_ID DESC LIMIT 1) AS class_name,
                        IFNULL(ES.EXAM_NAME, EFP.exam_setting) AS exam_name
                    FROM exam_fee_permission EFP
                    LEFT JOIN STU_PERSONAL_INFO SPI ON (SPI.STUDENT_ID = EFP.student_id OR SPI.ROLL_NO = EFP.student_id) AND SPI.IS_DELETED <> 1
                    LEFT JOIN EXAM_SETTING ES ON (CAST(ES.EXAM_SETTING_ID AS CHAR) = EFP.exam_setting)
                    WHERE EFP.is_deleted = 0
                      AND (@AC_YEAR IS NULL OR @AC_YEAR = '' OR EFP.academic_year = @AC_YEAR OR EFP.academic_year = 'ALL')
                      AND (@EXAM_SETTING IS NULL OR @EXAM_SETTING = '' OR EFP.exam_setting = @EXAM_SETTING OR EFP.exam_setting = 'ALL')
                      AND (@SEARCH IS NULL OR @SEARCH = '' 
                           OR SPI.ROLL_NO LIKE CONCAT('%', @SEARCH, '%') 
                           OR SPI.REGISTER_NO LIKE CONCAT('%', @SEARCH, '%') 
                           OR SPI.FIRST_NAME LIKE CONCAT('%', @SEARCH, '%')
                           OR EFP.student_id LIKE CONCAT('%', @SEARCH, '%'))
                    ORDER BY EFP.created_date DESC;";

                list = DapperHandler.FetchAll<EXAM_FEE_PERMISSION>(sql, new
                {
                    AC_YEAR = string.IsNullOrEmpty(academicYear) ? null : academicYear,
                    EXAM_SETTING = string.IsNullOrEmpty(examSetting) ? null : examSetting,
                    SEARCH = string.IsNullOrEmpty(searchTerm) ? null : searchTerm
                }) ?? new List<EXAM_FEE_PERMISSION>();
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "GetPermissions", ex.Message);
                }
            }

            return list;
        }

        /// <summary>
        /// Fetches a single permission by ID.
        /// </summary>
        public static EXAM_FEE_PERMISSION GetPermissionById(int examFeeId)
        {
            try
            {
                string sql = @"
                    SELECT 
                        EFP.exam_fee_id,
                        EFP.student_id,
                        EFP.exam_setting,
                        EFP.academic_year,
                        EFP.remarks,
                        EFP.is_active,
                        EFP.created_by,
                        EFP.created_date,
                        EFP.is_deleted,
                        SPI.ROLL_NO AS roll_no,
                        SPI.REGISTER_NO AS register_no,
                        CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, '')) AS student_name,
                        (SELECT CC.CLASS_NAME 
                         FROM STU_CLASS SC 
                         INNER JOIN CP_CLASSES CC ON CC.CLASS_ID = SC.CLASS_ID 
                         WHERE SC.STUDENT_ID = SPI.STUDENT_ID AND SC.IS_DELETED <> 1 
                         ORDER BY SC.ACADEMIC_YEAR DESC, SC.STU_CLASS_ID DESC LIMIT 1) AS class_name,
                        IFNULL(ES.EXAM_NAME, EFP.exam_setting) AS exam_name
                    FROM exam_fee_permission EFP
                    LEFT JOIN STU_PERSONAL_INFO SPI ON (SPI.STUDENT_ID = EFP.student_id OR SPI.ROLL_NO = EFP.student_id) AND SPI.IS_DELETED <> 1
                    LEFT JOIN EXAM_SETTING ES ON (CAST(ES.EXAM_SETTING_ID AS CHAR) = EFP.exam_setting)
                    WHERE EFP.exam_fee_id = @ID AND EFP.is_deleted = 0
                    LIMIT 1;";

                var result = DapperHandler.FetchAll<EXAM_FEE_PERMISSION>(sql, new { ID = examFeeId });
                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "GetPermissionById", ex.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Inserts a new permission record into exam_fee_permission.
        /// </summary>
        public static bool SavePermission(EXAM_FEE_PERMISSION model, string createdBy)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.student_id))
            {
                return false;
            }

            try
            {
                // Normalize student_id: if user entered roll no, resolve to STUDENT_ID
                string stuSql = @"
                    SELECT STUDENT_ID 
                    FROM STU_PERSONAL_INFO 
                    WHERE (STUDENT_ID = @TERM OR ROLL_NO = @TERM OR REGISTER_NO = @TERM) 
                      AND IS_DELETED <> 1 
                    LIMIT 1;";
                var stuResult = DapperHandler.FetchAll<dynamic>(stuSql, new { TERM = model.student_id.Trim() });
                string resolvedStudentId = (stuResult != null && stuResult.Count > 0)
                    ? Convert.ToString(stuResult[0].STUDENT_ID)
                    : model.student_id.Trim();

                string insertSql = @"
                    INSERT INTO exam_fee_permission 
                        (student_id, exam_setting, academic_year, remarks, is_active, created_by, created_date, is_deleted)
                    VALUES 
                        (@student_id, @exam_setting, @academic_year, @remarks, @is_active, @created_by, NOW(), 0);";

                var affected = DapperHandler.SaveOrUpdate(insertSql, new
                {
                    student_id = resolvedStudentId,
                    exam_setting = string.IsNullOrWhiteSpace(model.exam_setting) ? "ALL" : model.exam_setting.Trim(),
                    academic_year = string.IsNullOrWhiteSpace(model.academic_year) ? "ALL" : model.academic_year.Trim(),
                    remarks = model.remarks ?? "",
                    is_active = model.is_active,
                    created_by = createdBy ?? "System"
                });

                return Convert.ToInt32(affected) > 0;
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "SavePermission", ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Updates an existing permission record.
        /// </summary>
        public static bool UpdatePermission(EXAM_FEE_PERMISSION model)
        {
            if (model == null || model.exam_fee_id <= 0)
            {
                return false;
            }

            try
            {
                string updateSql = @"
                    UPDATE exam_fee_permission
                    SET 
                        exam_setting = @exam_setting,
                        academic_year = @academic_year,
                        remarks = @remarks,
                        is_active = @is_active
                    WHERE exam_fee_id = @exam_fee_id
                      AND is_deleted = 0;";

                var affected = DapperHandler.SaveOrUpdate(updateSql, new
                {
                    exam_fee_id = model.exam_fee_id,
                    exam_setting = string.IsNullOrWhiteSpace(model.exam_setting) ? "ALL" : model.exam_setting.Trim(),
                    academic_year = string.IsNullOrWhiteSpace(model.academic_year) ? "ALL" : model.academic_year.Trim(),
                    remarks = model.remarks ?? "",
                    is_active = model.is_active
                });

                return Convert.ToInt32(affected) > 0;
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "UpdatePermission", ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Toggles is_active status of a permission.
        /// </summary>
        public static bool TogglePermissionStatus(int examFeeId, bool isActive)
        {
            try
            {
                string sql = "UPDATE exam_fee_permission SET is_active = @IS_ACTIVE WHERE exam_fee_id = @ID AND is_deleted = 0;";
                var affected = DapperHandler.SaveOrUpdate(sql, new { IS_ACTIVE = isActive ? 1 : 0, ID = examFeeId });
                return Convert.ToInt32(affected) > 0;
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "TogglePermissionStatus", ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Soft deletes a permission (is_deleted = 1).
        /// </summary>
        public static bool DeletePermission(int examFeeId)
        {
            try
            {
                string sql = "UPDATE exam_fee_permission SET is_deleted = 1 WHERE exam_fee_id = @ID;";
                var affected = DapperHandler.SaveOrUpdate(sql, new { ID = examFeeId });
                return Convert.ToInt32(affected) > 0;
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "DeletePermission", ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Searches active students for autocomplete in Permission management UI.
        /// </summary>
        public static List<STUDENT_SEARCH_RESULT> SearchStudents(string term, string academicYear)
        {
            var list = new List<STUDENT_SEARCH_RESULT>();
            if (string.IsNullOrWhiteSpace(term))
            {
                return list;
            }

            try
            {
                string sql = @"
                    SELECT 
                        SPI.STUDENT_ID,
                        SPI.ROLL_NO,
                        SPI.REGISTER_NO,
                        CONCAT(IFNULL(SPI.FIRST_NAME, ''), ' ', IFNULL(SPI.LAST_NAME, '')) AS FULL_NAME,
                        (SELECT CC.CLASS_NAME 
                         FROM STU_CLASS SC 
                         INNER JOIN CP_CLASSES CC ON CC.CLASS_ID = SC.CLASS_ID 
                         WHERE SC.STUDENT_ID = SPI.STUDENT_ID AND SC.IS_DELETED <> 1 AND (@AC_YEAR IS NULL OR SC.ACADEMIC_YEAR = @AC_YEAR)
                         ORDER BY SC.ACADEMIC_YEAR DESC, SC.STU_CLASS_ID DESC LIMIT 1) AS CLASS_NAME
                    FROM STU_PERSONAL_INFO SPI
                    WHERE SPI.IS_DELETED <> 1
                      AND (SPI.ROLL_NO LIKE CONCAT('%', @TERM, '%') 
                           OR SPI.REGISTER_NO LIKE CONCAT('%', @TERM, '%') 
                           OR SPI.FIRST_NAME LIKE CONCAT('%', @TERM, '%')
                           OR SPI.STUDENT_ID = @EXACT)
                    LIMIT 15;";

                list = DapperHandler.FetchAll<STUDENT_SEARCH_RESULT>(sql, new
                {
                    TERM = term.Trim(),
                    EXACT = term.Trim(),
                    AC_YEAR = string.IsNullOrWhiteSpace(academicYear) ? null : academicYear.Trim()
                }) ?? new List<STUDENT_SEARCH_RESULT>();
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "SearchStudents", ex.Message);
                }
            }

            return list;
        }

        /// <summary>
        /// Fetches active exam settings filtered by academic year.
        /// If academicYear is null, empty, or 'ALL', returns all active exam settings.
        /// </summary>
        public static List<EXAM_SETTING> GetExamSettingsByAcademicYear(string academicYear)
        {
            var list = new List<EXAM_SETTING>();
            try
            {
                string sql = @"
                    SELECT 
                        EXAM_SETTING_ID, 
                        UPPER(EXAM_NAME) AS EXAM_NAME, 
                        ACADEMIC_YEAR 
                    FROM EXAM_SETTING 
                    WHERE IS_DELETED <> 1 
                      AND (@AC_YEAR IS NULL OR @AC_YEAR = '' OR @AC_YEAR = 'ALL' OR ACADEMIC_YEAR = @AC_YEAR)
                    ORDER BY EXAM_SETTING_ID DESC;";

                list = DapperHandler.FetchAll<EXAM_SETTING>(sql, new
                {
                    AC_YEAR = string.IsNullOrWhiteSpace(academicYear) ? null : academicYear.Trim()
                }) ?? new List<EXAM_SETTING>();
            }
            catch (Exception ex)
            {
                using (ErrorLog log = new ErrorLog())
                {
                    log.WriteError("FeeDueHelper", "GetExamSettingsByAcademicYear", ex.Message);
                }
            }

            return list;
        }
    }
}
