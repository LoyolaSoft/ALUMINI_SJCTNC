using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CMS.DAO;
using CMS.Utility;
using Dapper;

namespace CMS.SQL.OBE
{
    /// <summary>
    /// DAO class for OBE Question Entry operations
    /// Handles database operations for question setter to add questions based on question pattern
    /// </summary>
    public class OBEQuestionEntryDAO
    {
        private readonly string _connectionString;

        public OBEQuestionEntryDAO()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CMS"].ConnectionString;
        }

        /// <summary>
        /// Fetch all active exam settings for question entry
        /// </summary>
        public DataTable FetchExamSettingsForQuestionEntry()
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchExamSettingForQuestionEntry);
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Fetch all active courses for question entry
        /// </summary>
        public DataTable FetchCoursesForQuestionEntry()
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchCourseForQuestionEntry);
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Get question pattern based on course and exam setting
        /// </summary>
        public DataTable FetchQuestionPatternFromCoursePattern(int courseId, int examSettingId)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionPatternFromCoursePattern);
                query = query.Replace("?COURSE_ID", courseId.ToString());
                query = query.Replace("?EXAM_SETTING_ID", examSettingId.ToString());
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Fetch all OBE units for unit selection in question entry
        /// </summary>
        public DataTable FetchUnitsForQuestionEntry()
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchUnitForQuestionEntry);
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Fetch all question levels (K1 to K6) for question entry
        /// </summary>
        public DataTable FetchQuestionLevelsForEntry()
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchQuestionLevelsForEntry);
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Fetch Bloom's Taxonomy Master for display in left panel
        /// </summary>
        public DataTable FetchBloomsTaxonomyMaster()
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchBloomsTaxonomyMaster);
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Fetch single Bloom's Taxonomy record by ID for modal popup
        /// </summary>
        public DataTable FetchBloomsTaxonomyById(int bloomId)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchBloomsTaxonomyMasterById);
                query = query.Replace("?BLOOM_ID", bloomId.ToString());
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert new question with draft status
        /// </summary>
        public ResultArgs InsertExamQuestion(int courseId, int examSettingId, int questionLevelId, int unitId, 
            string questionText, string formulaEquation, int createdBy)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.InsertExamQuestionEntry);
                query = query.Replace("?QUESTION", $"'{MySQLHandler.EscapeString(questionText)}'");
                query = query.Replace("?FORMULA_EQUATION", string.IsNullOrEmpty(formulaEquation) ? "NULL" : $"'{MySQLHandler.EscapeString(formulaEquation)}'");
                query = query.Replace("?COURSE_ID", courseId.ToString());
                query = query.Replace("?EXAM_SETTING_ID", examSettingId.ToString());
                query = query.Replace("?QUESTION_LEVEL_ID", questionLevelId.ToString());
                query = query.Replace("?OBE_UNIT_ID", unitId.ToString());
                query = query.Replace("?SUBMISSION_STATUS", "'DRAFT'");
                query = query.Replace("?ENTRY_ID", createdBy.ToString());

                var result = MySQLHandler.ExecuteQuery(query, _connectionString);
                
                if (result.IsSuccessful)
                {
                    return new ResultArgs { IsSuccessful = true, Message = "Question saved as draft successfully." };
                }
                else
                {
                    return new ResultArgs { IsSuccessful = false, Message = result.Message ?? "Failed to save question." };
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                return new ResultArgs { IsSuccessful = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Update existing question while in draft status
        /// </summary>
        public ResultArgs UpdateExamQuestion(int questionId, int questionLevelId, int unitId, 
            string questionText, string formulaEquation, int updatedBy)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.UpdateExamQuestionEntry);
                query = query.Replace("?QUESTION", $"'{MySQLHandler.EscapeString(questionText)}'");
                query = query.Replace("?FORMULA_EQUATION", string.IsNullOrEmpty(formulaEquation) ? "NULL" : $"'{MySQLHandler.EscapeString(formulaEquation)}'");
                query = query.Replace("?QUESTION_LEVEL_ID", questionLevelId.ToString());
                query = query.Replace("?OBE_UNIT_ID", unitId.ToString());
                query = query.Replace("?SUBMISSION_STATUS", "'DRAFT'");
                query = query.Replace("?ENTRY_ID", updatedBy.ToString());
                query = query.Replace("?QUESTION_ID", questionId.ToString());

                var result = MySQLHandler.ExecuteQuery(query, _connectionString);
                
                if (result.IsSuccessful)
                {
                    return new ResultArgs { IsSuccessful = true, Message = "Question updated successfully." };
                }
                else
                {
                    return new ResultArgs { IsSuccessful = false, Message = result.Message ?? "Failed to update question." };
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                return new ResultArgs { IsSuccessful = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Final submit a question - locks it from further editing
        /// </summary>
        public ResultArgs FinalSubmitExamQuestion(int questionId, int submittedBy)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FinalSubmitExamQuestionEntry);
                query = query.Replace("?ENTRY_ID", submittedBy.ToString());
                query = query.Replace("?QUESTION_ID", questionId.ToString());

                var result = MySQLHandler.ExecuteQuery(query, _connectionString);
                
                if (result.IsSuccessful)
                {
                    return new ResultArgs { IsSuccessful = true, Message = "Question submitted successfully. No further editing is allowed." };
                }
                else
                {
                    return new ResultArgs { IsSuccessful = false, Message = result.Message ?? "Failed to submit question." };
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                return new ResultArgs { IsSuccessful = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Fetch question for preview after draft save
        /// </summary>
        public DataTable FetchExamQuestionForPreview(int questionId)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchExamQuestionEntryForPreview);
                query = query.Replace("?QUESTION_ID", questionId.ToString());
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Log validation attempts (e.g., copy-paste attempts)
        /// </summary>
        public void LogValidationAttempt(int questionId, string validationType, int userId, string message)
        {
            try
            {
                string query = string.Format(
                    @"INSERT INTO obe_question_validation_log 
                      (OBE_QUESTIONPAPERSTRUCTURE_ID, VALIDATION_TYPE, USER_ID, LOG_MESSAGE, LOG_DATE) 
                      VALUES ({0}, '{1}', {2}, '{3}', NOW());",
                    questionId,
                    validationType,
                    userId,
                    MySQLHandler.EscapeString(message)
                );

                MySQLHandler.ExecuteQuery(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// Save draft version history
        /// </summary>
        public void SaveDraftHistory(int questionId, string questionText, string formulaEquation, 
            int questionLevelId, int unitId, decimal marks)
        {
            try
            {
                string query = string.Format(
                    @"INSERT INTO obe_question_draft_history 
                      (OBE_QUESTIONPAPERSTRUCTURE_ID, QUESTION_TEXT, QUESTION_ANSWER, BLOOM_TAXONOMY_LEVEL, OBE_UNIT_ID, MARKS_FOR_QUESTION, SAVED_DATE, SEQUENCE_NUMBER) 
                      VALUES ({0}, '{1}', '{2}', '{3}', {4}, {5}, NOW(), 
                              (SELECT COALESCE(MAX(SEQUENCE_NUMBER), 0) + 1 FROM obe_question_draft_history WHERE OBE_QUESTIONPAPERSTRUCTURE_ID = {0}))",
                    questionId,
                    MySQLHandler.EscapeString(questionText),
                    MySQLHandler.EscapeString(formulaEquation ?? string.Empty),
                    $"K{questionLevelId}",
                    unitId,
                    marks
                );

                MySQLHandler.ExecuteQuery(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// Check if question is already submitted finally
        /// </summary>
        public bool IsQuestionFinallnySubmitted(int questionId)
        {
            try
            {
                string query = string.Format(
                    @"SELECT COUNT(*) as cnt FROM obe_questions 
                      WHERE QUESTION_ID = {0} AND SUBMISSION_STATUS = 'FINAL_SUBMITTED'",
                    questionId
                );

                var dt = MySQLHandler.FetchData(query, _connectionString);
                return dt != null && dt.Rows.Count > 0 && int.Parse(dt.Rows[0]["cnt"].ToString()) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                return false;
            }
        }

        /// <summary>
        /// Fetch existing claim entry for a course and exam setting
        /// </summary>
        public DataTable FetchClaimEntry(int courseId, int examSettingId)
        {
            try
            {
                var query = OBESQL.GetOBESQL(OBESQLCommands.FetchOBEClaimEntryByCourseAndStaff);
                query = query.Replace("?COURSE_ID", courseId.ToString());
                query = query.Replace("?EXAM_SETTING_ID", examSettingId.ToString());
                return MySQLHandler.FetchData(query, _connectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert or Update claim entry
        /// </summary>
        public ResultArgs SaveClaimEntry(OBE_CLAIM_ENTRY claim)
        {
            try
            {
                string query = "";
                if (string.IsNullOrEmpty(claim.CLAIM_ID))
                {
                    claim.CLAIM_ID = Guid.NewGuid().ToString();
                    query = OBESQL.GetOBESQL(OBESQLCommands.InsertOBEClaimEntry);
                    query = query.Replace("?CLAIM_ID", $"'{claim.CLAIM_ID}'");
                    query = query.Replace("?EXAM_SETTING_ID", claim.EXAM_SETTING_ID ?? "NULL");
                    query = query.Replace("?COURSE_ID", claim.COURSE_ID ?? "NULL");
                    query = query.Replace("?STAFF_ID", claim.STAFF_ID ?? "NULL");
                    query = query.Replace("?ACADEMIC_YEAR", claim.ACADEMIC_YEAR != null ? $"'{MySQLHandler.EscapeString(claim.ACADEMIC_YEAR)}'" : "NULL");
                }
                else
                {
                    query = OBESQL.GetOBESQL(OBESQLCommands.UpdateOBEClaimEntry);
                    query = query.Replace("?CLAIM_ID", $"'{claim.CLAIM_ID}'");
                }

                query = query.Replace("?PROGRAMME", claim.PROGRAMME != null ? $"'{MySQLHandler.EscapeString(claim.PROGRAMME)}'" : "NULL");
                query = query.Replace("?COURSE_CODE", claim.COURSE_CODE != null ? $"'{MySQLHandler.EscapeString(claim.COURSE_CODE)}'" : "NULL");
                query = query.Replace("?COURSE_TITLE", claim.COURSE_TITLE != null ? $"'{MySQLHandler.EscapeString(claim.COURSE_TITLE)}'" : "NULL");
                query = query.Replace("?REMUNERATION_TYPE", claim.REMUNERATION_TYPE != null ? $"'{MySQLHandler.EscapeString(claim.REMUNERATION_TYPE)}'" : "NULL");
                query = query.Replace("?NO_OF_QUESTION", claim.NO_OF_QUESTION.ToString());
                query = query.Replace("?QUESTION_AMOUNT", claim.QUESTION_AMOUNT.ToString());
                query = query.Replace("?SCHEME_AMOUNT", claim.SCHEME_AMOUNT.ToString());
                query = query.Replace("?POSTAL_CHARGES", claim.POSTAL_CHARGES.ToString());
                query = query.Replace("?TOTAL_CLAIM", claim.TOTAL_CLAIM.ToString());
                query = query.Replace("?RUPEES_IN_WORDS", claim.RUPEES_IN_WORDS != null ? $"'{MySQLHandler.EscapeString(claim.RUPEES_IN_WORDS)}'" : "NULL");
                query = query.Replace("?STAFF_NAME", claim.STAFF_NAME != null ? $"'{MySQLHandler.EscapeString(claim.STAFF_NAME)}'" : "NULL");
                query = query.Replace("?MOBILE_NO", claim.MOBILE_NO != null ? $"'{MySQLHandler.EscapeString(claim.MOBILE_NO)}'" : "NULL");
                query = query.Replace("?DESIGNATION", claim.DESIGNATION != null ? $"'{MySQLHandler.EscapeString(claim.DESIGNATION)}'" : "NULL");
                query = query.Replace("?OFFICIAL_ADDRESS", claim.OFFICIAL_ADDRESS != null ? $"'{MySQLHandler.EscapeString(claim.OFFICIAL_ADDRESS)}'" : "NULL");
                query = query.Replace("?AC_NO", claim.AC_NO != null ? $"'{MySQLHandler.EscapeString(claim.AC_NO)}'" : "NULL");
                query = query.Replace("?BANK", claim.BANK != null ? $"'{MySQLHandler.EscapeString(claim.BANK)}'" : "NULL");
                query = query.Replace("?BRANCH", claim.BRANCH != null ? $"'{MySQLHandler.EscapeString(claim.BRANCH)}'" : "NULL");
                query = query.Replace("?IFS_CODE", claim.IFS_CODE != null ? $"'{MySQLHandler.EscapeString(claim.IFS_CODE)}'" : "NULL");
                query = query.Replace("?SUBMISSION_STATUS", claim.SUBMISSION_STATUS != null ? $"'{MySQLHandler.EscapeString(claim.SUBMISSION_STATUS)}'" : "'DRAFT'");

                var result = MySQLHandler.ExecuteQuery(query, _connectionString);
                return new ResultArgs { IsSuccessful = result.IsSuccessful, Message = result.Message, Tag = claim.CLAIM_ID };
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorLog(ex);
                return new ResultArgs { IsSuccessful = false, Message = ex.Message };
            }
        }
    }
}
