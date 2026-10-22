using CMS.DAO;
using System;

namespace CMS.SQL.QuestionSector
{
    public enum QuestionSectorSQLCommands
    {
        InsertQuestionPaper,
        UpdateQuestionPaperHeader,
        InsertQuestionDetails,
        DeleteQuestionDetailsByPaperId,
        FetchQuestionPaperById,
        FetchQuestionDetailsByPaperId
    }

    public static class QuestionSectorSQL
    {
        public static string GetQuestionSectorSQL(QuestionSectorSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case QuestionSectorSQLCommands.InsertQuestionPaper:
                    query = @"INSERT INTO QS_QUESTION_PAPER
                                (ACADEMIC_YEAR_ID, SEMESTER_ID, COURSE_ID, EXAM_SETTING_ID, TOTAL_MARK, DURATION, CREATED_BY, UPDATED_BY)
                              VALUES
                                (?ACADEMIC_YEAR_ID, ?SEMESTER_ID, ?COURSE_ID, ?EXAM_SETTING_ID, ?TOTAL_MARK, ?DURATION, ?CREATED_BY, ?UPDATED_BY);";
                    break;

                case QuestionSectorSQLCommands.UpdateQuestionPaperHeader:
                    query = @"UPDATE QS_QUESTION_PAPER SET
                                ACADEMIC_YEAR_ID = ?ACADEMIC_YEAR_ID,
                                SEMESTER_ID      = ?SEMESTER_ID,
                                COURSE_ID        = ?COURSE_ID,
                                EXAM_SETTING_ID  = ?EXAM_SETTING_ID,
                                TOTAL_MARK       = ?TOTAL_MARK,
                                DURATION         = ?DURATION,
                                UPDATED_BY       = ?UPDATED_BY
                              WHERE PAPER_ID = ?PAPER_ID;";
                    break;

                case QuestionSectorSQLCommands.InsertQuestionDetails:
                    query = @"INSERT INTO QS_QUESTION_DETAILS
                                (PAPER_ID, SECTION, QUESTION_NO, SUB_PART, QUESTION_TEXT, BLOOMS_LEVEL_ID, MARKS)
                              VALUES
                                (?PAPER_ID, ?SECTION, ?QUESTION_NO, ?SUB_PART, ?QUESTION_TEXT, ?BLOOMS_LEVEL_ID, ?MARKS);";
                    break;

                case QuestionSectorSQLCommands.DeleteQuestionDetailsByPaperId:
                    query = "DELETE FROM QS_QUESTION_DETAILS WHERE PAPER_ID = ?PAPER_ID;";
                    break;

                case QuestionSectorSQLCommands.FetchQuestionPaperById:
                    query = @"SELECT QP.*, CR.COURSE_TITLE, CR.COURSE_CODE, ES.EXAM_NAME
                              FROM QS_QUESTION_PAPER QP
                              LEFT JOIN CP_COURSE_ROOT  CR ON CR.COURSE_ROOT_ID    = QP.COURSE_ID
                              LEFT JOIN EXAM_SETTING    ES ON ES.EXAM_SETTING_ID  = QP.EXAM_SETTING_ID
                              WHERE QP.PAPER_ID = ?PAPER_ID;";
                    break;

                case QuestionSectorSQLCommands.FetchQuestionDetailsByPaperId:
                    query = @"SELECT QD.*, QL.OBE_QUESTIONLEVEL
                              FROM QS_QUESTION_DETAILS QD
                              LEFT JOIN SUP_OBE_QUESTIONLEVEL QL ON QL.OBE_QUESTIONLEVEL_ID = QD.BLOOMS_LEVEL_ID
                              WHERE QD.PAPER_ID = ?PAPER_ID
                              ORDER BY QD.SECTION, CAST(QD.QUESTION_NO AS UNSIGNED), QD.SUB_PART;";
                    break;
            }
            return query;
        }
    }
}
