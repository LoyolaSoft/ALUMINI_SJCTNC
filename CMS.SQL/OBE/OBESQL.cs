using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.OBE
{
    public static class OBESQL
    {
 
        public static string GetOBESQL(OBESQLCommands sEnumCommand)
        {
            string query = "";


            switch (sEnumCommand)
            {
                //OBE
                case OBESQLCommands.BindOBEQuestionPaperPatternByExamSetting:
                    {
                        query = @"SELECT 
                                OBE_QUESTIONPAPERPATTERN_ID,
                                OBE_QUESTIONPAPERPATTERN,
                                TOTAL_HOUR,
                                TOTAL_QUESTION,
                                TOTAL_MARK
                            FROM
                                OBE_QUESTIONPAPER_PATTERN
                            WHERE
                                EXAM_SETTING_ID = ?EXAM_SETTING_ID AND IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchOBEQuestionPaperPatternByPatternId:
                    {
                        query = @"SELECT 
                                OBE_QUESTIONPAPERPATTERN_ID,
                                OBE_QUESTIONPAPERPATTERN,
                                EXAM_SETTING_ID,
                                TOTAL_HOUR,
                                TOTAL_QUESTION,
                                TOTAL_MARK,
                                DEPARTMENT,
                                APPLICATION_TYPE_ID
                            FROM
                                OBE_QUESTIONPAPER_PATTERN
                            WHERE
                                OBE_QUESTIONPAPERPATTERN_ID = ?OBE_QUESTIONPAPERPATTERN_ID;";
                        break;
                    }
                case OBESQLCommands.InsertOBEQuestionPaperPattern:
                    {
                        query = @"INSERT INTO OBE_QUESTIONPAPER_PATTERN (OBE_QUESTIONPAPERPATTERN, EXAM_SETTING_ID, TOTAL_HOUR, TOTAL_QUESTION, TOTAL_MARK,DEPARTMENT,APPLICATION_TYPE_ID) VALUES
                                (?OBE_QUESTIONPAPERPATTERN, ?EXAM_SETTING_ID, ?TOTAL_HOUR, ?TOTAL_QUESTION, ?TOTAL_MARK,?DEPARTMENT,?APPLICATION_TYPE_ID);";
                        break;
                    }
                case OBESQLCommands.UpdateOBEQuestionPaperPattern:
                    {
                        query = @"UPDATE OBE_QUESTIONPAPER_PATTERN SET    
                                OBE_QUESTIONPAPERPATTERN = ?OBE_QUESTIONPAPERPATTERN,   
                                TOTAL_HOUR = ?TOTAL_HOUR,
                                TOTAL_QUESTION =?TOTAL_QUESTION,
                                TOTAL_MARK = ?TOTAL_MARK,
                                DEPARTMENT=?DEPARTMENT,
                                APPLICATION_TYPE_ID=?APPLICATION_TYPE_ID
                            WHERE
                                OBE_QUESTIONPAPERPATTERN_ID = ?OBE_QUESTIONPAPERPATTERN_ID;";
                        break;
                    }
                case OBESQLCommands.DeleteOBEQuestionPaperPattern:
                    {
                        query = @"UPDATE OBE_QUESTIONPAPER_PATTERN SET IS_DELETED=1 WHERE OBE_QUESTIONPAPERPATTERN_ID=?OBE_QUESTIONPAPERPATTERN_ID;";
                        break;
                    }
                case OBESQLCommands.FetchOBEQuestionLevel:
                    {
                        query = @"SELECT 
                                    OBE_QUESTIONLEVEL_ID, OBE_QUESTIONLEVEL
                                FROM
                                    SUP_OBE_QUESTIONLEVEL
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.BindOBEQuestionPaperPatternSectionByPatternId:
                    {
                        query = @"SELECT 
                                PS.OBE_QUESTIONPAPERPATTERNSECTION_ID,
                                PS.OBE_QUESTIONPAPERPATTERN_ID,
                                PS.OBE_QUESTIONLEVEL_ID,
                                QL.OBE_QUESTIONLEVEL,
                                PS.SECTION_ID,
                                S.SECTION_NAME,
                                PS.TOTAL_QUESTION,
                                PS.TO_BE_ATTEND,
                                PS.EITHER_OR,
                                PS.MARK_FOR_EACH_QUESTION,
                                PS.MARK_FOR_EACH_SECTION,
                                PS.SECTION_DECRIPTION,
                                P.EXAM_SETTING_ID
                            FROM
                                OBE_QUESTIONPAPER_PATTERN_SECTION AS PS
                                    INNER JOIN
                                OBE_QUESTIONPAPER_PATTERN AS P ON P.OBE_QUESTIONPAPERPATTERN_ID = PS.OBE_QUESTIONPAPERPATTERN_ID
                                    INNER JOIN
                                SUP_OBE_QUESTIONLEVEL AS QL ON QL.OBE_QUESTIONLEVEL_ID = PS.OBE_QUESTIONLEVEL_ID
                                    LEFT JOIN
                                EXAM_SECTION AS S ON S.SECTION_ID = PS.SECTION_ID
                            WHERE
                                PS.IS_DELETED != 1
                                    AND PS.OBE_QUESTIONPAPERPATTERN_ID = ?OBE_QUESTIONPAPERPATTERN_ID
                            ORDER BY SECTION_ID , OBE_QUESTIONLEVEL_ID;";
                        break;
                    }
                case OBESQLCommands.BindOBEQuestionPaperPatternStuctureByPatternId:
                    {
                        query = @"SELECT 
                                PS.OBE_QUESTIONPAPERPATTERNSECTION_ID,
                                PS.OBE_QUESTIONPAPERPATTERN_ID,
                                QL.OBE_QUESTIONLEVEL,
                                PS.SECTION_ID,
                                S.SECTION_NAME,
                                PS.TOTAL_QUESTION,
                                PS.TO_BE_ATTEND,
                                PS.EITHER_OR,
                                PS.MARK_FOR_EACH_QUESTION,
                                PS.MARK_FOR_EACH_SECTION,
                                PS.SECTION_DECRIPTION,
                                P.EXAM_SETTING_ID,
                                IFNULL(OBE_QUESTIONPAPERSTRUCTURE_ID,'') AS OBE_QUESTIONPAPERSTRUCTURE_ID,
                                IFNULL(QUS.QUS_NO,0) AS QUS_NO,
                                QUS.OBE_COLEVEL_ID,
                                QUS.OBE_QUESTIONLEVEL_ID,
                                QUS.OBE_UNIT_ID,
                                IFNULL(QUESTION_LEVEL_ID,?QUESTION_LEVEL_ID) AS QUESTION_LEVEL_ID
                            FROM
                                OBE_QUESTIONPAPER_PATTERN_SECTION AS PS
                                    INNER JOIN
                                OBE_QUESTIONPAPER_PATTERN AS P ON P.OBE_QUESTIONPAPERPATTERN_ID = PS.OBE_QUESTIONPAPERPATTERN_ID
                                    INNER JOIN
                                SUP_OBE_QUESTIONLEVEL AS QL ON QL.OBE_QUESTIONLEVEL_ID = PS.OBE_QUESTIONLEVEL_ID
                                    LEFT JOIN
                                EXAM_SECTION AS S ON S.SECTION_ID = PS.SECTION_ID
									LEFT JOIN
								OBE_QUESTIONPAPER_STRUCTURE AS QUS ON QUS.OBE_QUESTIONPAPERPATTERN_ID = PS.OBE_QUESTIONPAPERPATTERN_ID
                                    AND QUS.OBE_QUESTIONPAPERPATTERNSECTION_ID=PS.OBE_QUESTIONPAPERPATTERNSECTION_ID
                                    AND QUS.EXAM_SETTING_ID=P.EXAM_SETTING_ID
                                    AND QUS.QUESTION_LEVEL_ID=?QUESTION_LEVEL_ID
                                    AND QUS.IS_DELETED!=1
                            WHERE
                                PS.IS_DELETED != 1
                                    AND PS.OBE_QUESTIONPAPERPATTERN_ID = ?OBE_QUESTIONPAPERPATTERN_ID
                                    
                            ORDER BY SECTION_ID , OBE_QUESTIONLEVEL_ID;";
                        break;
                    }
                case OBESQLCommands.DeleteOBEQuestionPaperPatternSection:
                    {
                        query = @"UPDATE OBE_QUESTIONPAPER_PATTERN_SECTION SET IS_DELETED=1 WHERE OBE_QUESTIONPAPERPATTERNSECTION_ID=?OBE_QUESTIONPAPERPATTERNSECTION_ID;";
                        break;
                    }
                case OBESQLCommands.FetchOBEQuestionPaperPatternSectionByPatternSectionId:
                    {
                        query = @"SELECT 
                                OBE_QUESTIONPAPERPATTERNSECTION_ID,
                                OBE_QUESTIONPAPERPATTERN_ID,
                                OBE_QUESTIONLEVEL_ID,
                                SECTION_ID,
                                TOTAL_QUESTION,
                                TO_BE_ATTEND,
                                EITHER_OR,
                                MARK_FOR_EACH_QUESTION AS MARK_FOR_EACH_QUESTION,
                                MARK_FOR_EACH_SECTION,
                                SECTION_DECRIPTION
                            FROM
                                OBE_QUESTIONPAPER_PATTERN_SECTION
                            WHERE
                                IS_DELETED != 1
                                    AND OBE_QUESTIONPAPERPATTERNSECTION_ID = ?OBE_QUESTIONPAPERPATTERNSECTION_ID;";
                        break;
                    }
                case OBESQLCommands.InsertOBEQuestionPatternSection:
                    {
                        query = @"INSERT INTO OBE_QUESTIONPAPER_PATTERN_SECTION (OBE_QUESTIONPAPERPATTERNSECTION_ID, OBE_QUESTIONPAPERPATTERN_ID, OBE_QUESTIONLEVEL_ID, SECTION_ID, TOTAL_QUESTION, TO_BE_ATTEND,EITHER_OR, MARK_FOR_EACH_QUESTION, MARK_FOR_EACH_SECTION, SECTION_DECRIPTION)
                                VALUES (?OBE_QUESTIONPAPERPATTERNSECTION_ID, ?OBE_QUESTIONPAPERPATTERN_ID, ?OBE_QUESTIONLEVEL_ID, ?SECTION_ID, ?TOTAL_QUESTION, ?TO_BE_ATTEND,?EITHER_OR, ?MARK_FOR_EACH_QUESTION, ?MARK_FOR_EACH_SECTION, ?SECTION_DECRIPTION);";
                        break;
                    }
                case OBESQLCommands.UpdateOBEQuestionPatternSection:
                    {
                        query = @"UPDATE OBE_QUESTIONPAPER_PATTERN_SECTION SET
                                OBE_QUESTIONLEVEL_ID=?OBE_QUESTIONLEVEL_ID,
                                SECTION_ID=?SECTION_ID,
                                TOTAL_QUESTION=?TOTAL_QUESTION,
                                TO_BE_ATTEND=?TO_BE_ATTEND,
                                EITHER_OR=?EITHER_OR,
                                MARK_FOR_EACH_QUESTION=?MARK_FOR_EACH_QUESTION,
                                MARK_FOR_EACH_SECTION=?MARK_FOR_EACH_SECTION,
                                SECTION_DECRIPTION=?SECTION_DECRIPTION
                             WHERE OBE_QUESTIONPAPERPATTERNSECTION_ID=?OBE_QUESTIONPAPERPATTERNSECTION_ID;";
                        break;
                    }
                case OBESQLCommands.FetchOBEQuestionpaperPatternByExamsetting:
                    {
                        query = @"SELECT 
                                    OBE_QUESTIONPAPERPATTERN_ID, OBE_QUESTIONPAPERPATTERN,TOTAL_HOUR,TOTAL_QUESTION,TOTAL_MARK
                                FROM
                                    OBE_QUESTIONPAPER_PATTERN
                                WHERE
                                    EXAM_SETTING_ID = ?EXAM_SETTING_ID AND IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchExamSections:
                    {
                        query = @"SELECT 
                            SECTION_ID, SECTION_NAME
                        FROM
                            EXAM_SECTION
                        WHERE
                            IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchOBECOComponent:
                    {
                        query = @"SELECT 
                                    OBE_COMPONENT_ID, OBE_COMPONENT, MAX_OBE_COMPONENT
                                FROM
                                    OBE_CO_ATTAINMENT_COMPONENT
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchobeQuestionLevelrbtLevel:
                    {
                        query = @"SELECT 
                                    QL.QUESTION_LEVEL_ID,
                                    QL.QUESTION_LEVEL,
                                    QL.OBE_RBT_LEVEL_ID,
                                    RL.OBE_RBT_LEVEL
                                FROM
                                    OBE_QUESTION_LEVEL AS QL
                                        INNER JOIN
                                    SUP_OBE_RBT_LEVEL AS RL ON RL.OBE_RBT_LEVEL_ID = QL.OBE_RBT_LEVEL_ID
                                        AND RL.IS_DELETED != 1
                                WHERE
                                    QL.IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchobeCOLevel:
                    {
                        query = @"SELECT 
                                    OBE_COLEVEL_ID, OBE_COLEVEL, OBE_QUESTIONLEVEL_ID
                                FROM
                                    SUP_OBE_CO_LEVEL
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchobePOLevel:
                    {
                        query = @"SELECT 
                                    OBE_POLEVEL_ID, OBE_POLEVEL
                                FROM
                                    SUP_OBE_PO_LEVEL
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchobePSOLevel:
                    {
                        query = @"SELECT 
                                    OBE_PSOLEVEL_ID, OBE_PSOLEVEL
                                FROM
                                    SUP_OBE_PSO_LEVEL
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.InsertOBEQuestionPaperStructure:
                    {
                        query = @"INSERT INTO OBE_QUESTIONPAPER_STRUCTURE (OBE_QUESTIONPAPERPATTERNSECTION_ID,
                                      OBE_QUESTIONPAPERPATTERN_ID,
                                      QUS_NO,
                                      QUESTION_NUMBER,
                                      OBE_COLEVEL_ID,
                                      OBE_QUESTIONLEVEL_ID,
                                      OBE_UNIT_ID,
                                      EXAM_SETTING_ID,
                                      QUESTION_LEVEL_ID,
                                      OBE_COMPONENT_ID) VALUES
                                      (?OBE_QUESTIONPAPERPATTERNSECTION_ID,
                                      ?OBE_QUESTIONPAPERPATTERN_ID,
                                      ?QUS_NO,
                                      ?QUESTION_NUMBER,
                                      ?OBE_COLEVEL_ID,
                                      ?OBE_QUESTIONLEVEL_ID,
                                      ?OBE_UNIT_ID,
                                      ?EXAM_SETTING_ID,
                                      ?QUESTION_LEVEL_ID,?OBE_COMPONENT_ID);";
                        break;
                    }
                case OBESQLCommands.UPDATEOBEQuestionPaperStructure:
                    {
                        query = @"UPDATE OBE_QUESTIONPAPER_STRUCTURE SET 
                                      OBE_QUESTIONPAPERPATTERNSECTION_ID=?OBE_QUESTIONPAPERPATTERNSECTION_ID,
                                      OBE_QUESTIONPAPERPATTERN_ID=?OBE_QUESTIONPAPERPATTERN_ID,
                                      QUS_NO=?QUS_NO,
                                      QUESTION_NUMBER=?QUESTION_NUMBER,
                                      OBE_COLEVEL_ID=?OBE_COLEVEL_ID,
                                      OBE_QUESTIONLEVEL_ID=?OBE_QUESTIONLEVEL_ID,
                                      OBE_UNIT_ID=?OBE_UNIT_ID,
                                      EXAM_SETTING_ID=?EXAM_SETTING_ID,
                                      QUESTION_LEVEL_ID=?QUESTION_LEVEL_ID,
                                      OBE_COMPONENT_ID=?OBE_COMPONENT_ID
                                 WHERE OBE_QUESTIONPAPERSTRUCTURE_ID=?OBE_QUESTIONPAPERSTRUCTURE_ID;";
                        break;
                    }
                case OBESQLCommands.BindOBECoursePatternByExamSettingId:
                    {
                        query = @"SELECT 
                                      COURSE_ROOT_ID,
                                      COURSE_CODE,
                                      COURSE_TITLE,
                                      SEMESTER_ID,
                                      DD.DEPARTMENT,
                                      IFNULL(OBE_COURSEPATTERN_ID,'') AS OBE_COURSEPATTERN_ID,
                                      IFNULL(PATTERN_ID,0) AS PATTERN_ID
                                  FROM
                                      CP_COURSE_PATTERN AS CP
                                          INNER JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                          AND CR.PART IN (1 , 2, 3)
                                          AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                          INNER JOIN
                                      CP_DEPARTMENT AS DD ON DD.DEPARTMENT_ID = CR.DEPARTMENT
                                  		  LEFT JOIN
                                  	  OBE_COURSE_PATTERN AS P ON P.COURSE_ID=CP.COURSE_ID
                                          AND P.EXAM_SETTING_ID=?EXAM_SETTING_ID
                                         AND P.IS_DELETED!=1
                                         AND P.OBE_COMPONENT_ID=?OBE_COMPONENT_ID
                                  WHERE
                                      CP.IS_DELETED != 1
                                          AND CP.ACADEMIC_YEAR = ?AC_YEAR
                                  GROUP BY CR.COURSE_ROOT_ID
                                  ORDER BY CR.DEPARTMENT;";
                        break;
                    }
                case OBESQLCommands.BindEditOBECoursePatternByExamSettingId:
                    {
                        query = @"SELECT 
                                      COURSE_ROOT_ID,
                                      COURSE_CODE,
                                      COURSE_TITLE,
                                      SEMESTER_ID,
                                      DD.DEPARTMENT,
                                      IFNULL(OBE_COURSEPATTERN_ID, '') AS OBE_COURSEPATTERN_ID,
                                      QP.OBE_QUESTIONPAPERPATTERN_ID AS PATTERN_ID,
                                      IFNULL(P.STAFF_ID, '') AS STAFF_ID,
                                      IFNULL(P.ROLE, '') AS ROLE
                                  FROM
                                      CP_COURSE_PATTERN AS CP
                                          INNER JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                          AND CR.PART IN (1 , 2, 3)
                                          AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                          AND CR.PROGRAMME_LEVEL_ID IN (?PROGRAMME_LEVEL_ID)
                                          INNER JOIN
                                      CP_DEPARTMENT AS DD ON DD.DEPARTMENT_ID = CR.DEPARTMENT
                                          INNER JOIN
                                      OBE_QUESTIONPAPER_PATTERN AS QP ON QP.APPLICATION_TYPE_ID = CR.PROGRAMME_LEVEL_ID
                                          AND QP.EXAM_SETTING_ID =?EXAM_SETTING_ID
                                          AND  FIND_IN_SET(CR.DEPARTMENT,QP.DEPARTMENT)
                                          LEFT JOIN
                                      OBE_COURSE_PATTERN AS P ON P.COURSE_ID = CP.COURSE_ID
                                          AND P.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                          AND P.IS_DELETED != 1
                                          AND P.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                  WHERE
                                      CP.IS_DELETED != 1
                                          AND CP.ACADEMIC_YEAR = ?AC_YEAR
                                  GROUP BY CR.COURSE_ROOT_ID
                                  ORDER BY CR.DEPARTMENT;";
                        break;
                    }
                case OBESQLCommands.InsertOBECoursePattern:
                    {
                        query = @"INSERT INTO OBE_COURSE_PATTERN (COURSE_ID,
                                    PATTERN_ID,
                                    ACADEMIC_YEAR,
                                    EXAM_SETTING_ID,
                                    OBE_COMPONENT_ID,
                                    STAFF_ID,
                                    ROLE) VALUES
                                    (?COURSE_ID,
                                    ?PATTERN_ID,
                                    ?ACADEMIC_YEAR,
                                    ?EXAM_SETTING_ID,
                                    ?OBE_COMPONENT_ID,
                                    ?STAFF_ID,
                                    ?ROLE)";
                        break;
                    }
                case OBESQLCommands.UPDATEOBECoursePattern:
                    {
                        query = @"UPDATE OBE_COURSE_PATTERN SET 
                                    COURSE_ID=?COURSE_ID,
                                    PATTERN_ID=?PATTERN_ID,
                                    ACADEMIC_YEAR=?ACADEMIC_YEAR,
                                    EXAM_SETTING_ID=?EXAM_SETTING_ID,
                                    OBE_COMPONENT_ID=?OBE_COMPONENT_ID,
                                    STAFF_ID=?STAFF_ID,
                                    ROLE=?ROLE
                                 WHERE OBE_COURSEPATTERN_ID=?OBE_COURSEPATTERN_ID;";
                        break;
                    }
                case OBESQLCommands.FetchOBEQuestionMarkEntry:
                    {
                        query = @"SELECT 
                                CL.STUDENT_ID,
                                M.OBE_MARKENTRY_ID,
                                M.OBE_QUESTIONMARKSTRUCTURE_ID,
                                M.QUESTION_NUMBER,
                                M.TOTAL_MARK,
                                M.SECURED_MARK,
                                M.COURSE_ID,
                                M.EXAM_SETTING_ID,
                                M.OBE_COLEVEL_ID,
                                M.OBE_COMPONENT_ID,
                                M.ENTRY_ID
                            FROM
                                STU_CLASS AS CL
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CL.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    LEFT JOIN
                                OBE_MARKENTRY AS M ON M.STUDENT_ID = CL.STUDENT_ID
                                    AND M.COURSE_ID = ?COURSE_ID
                                    AND M.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                    AND M.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                    AND M.IS_DELETED!=1
                            WHERE
                                CL.CLASS_ID = ?CLASS_ID
                                    AND CL.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CL.IS_DELETED != 1 
                            UNION SELECT 
                                CLS.STUDENT_ID,
                                M.OBE_MARKENTRY_ID,
                                M.OBE_QUESTIONMARKSTRUCTURE_ID,
                                M.QUESTION_NUMBER,
                                M.TOTAL_MARK,
                                M.SECURED_MARK,
                                M.COURSE_ID,
                                M.EXAM_SETTING_ID,
                                M.OBE_COLEVEL_ID,
                                M.OBE_COMPONENT_ID,
                                M.ENTRY_ID
                            FROM
                                STU_COURSE_INFO AS CLS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    LEFT JOIN
                                OBE_MARKENTRY AS M ON M.STUDENT_ID = CLS.STUDENT_ID
                                    AND M.COURSE_ID = ?COURSE_ID
                                    AND M.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                    AND M.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                    AND M.IS_DELETED!=1
                            WHERE
                                CLS.S_CLASS_ID = ?CLASS_ID
                                    AND CLS.IS_DELETED != 1
                                    AND CLS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case OBESQLCommands.FetchStudentForOBEMarkEntry:
                    {
                        query = @"SELECT 
                                    CL.STUDENT_ID, SP.REGISTER_NO, SP.ROLL_NO,CONCAT(LPAD(EPP1.PACKET_REFERENCE_NO, 4, 0),
            ' - ',
            LPAD(IFNULL(EPP1.DUMMY_NO, 0), 4, 0)) AS DUMMY_NO,CONCAT(SP.FIRST_NAME,'  (',DATE_FORMAT(DATE_OF_BIRTH,'%d-%m-%Y'),')') AS FIRST_NAME,IFNULL(A.IS_ABSENT,0) AS IS_ABSENT,OBE_ABSENT_ID
                                FROM
                                    STU_CLASS AS CL
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CL.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        LEFT JOIN
                                    EXAM_PACKET_PROCESS AS EPP1 ON EPP1.STUDENT_ID=CL.STUDENT_ID
										AND EPP1.COURSE_ID=?COURSE_ID
                                        AND EPP1.EXAM_SETTING_ID=?EXAM_SETTING_ID
                                        AND EPP1.IS_DELETED!=1
                                        LEFT JOIN
									OBE_ABSENTEE AS A ON A.STUDENT_ID=CL.STUDENT_ID
                                        AND A.COURSE_ID=?COURSE_ID
                                        AND A.EXAM_SETTING_ID=?EXAM_SETTING_ID
                                        AND A.OBE_COMPONENT_ID=?OBE_COMPONENT_ID
                                        AND A.IS_DELETED!=1
                                WHERE
                                    CL.CLASS_ID = ?CLASS_ID
                                        AND CL.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CL.IS_DELETED != 1 
                                UNION SELECT 
                                    CLS.STUDENT_ID, SP.REGISTER_NO, SP.ROLL_NO,CONCAT(LPAD(EPP.PACKET_REFERENCE_NO, 4, 0),
            ' - ',
            LPAD(IFNULL(EPP.DUMMY_NO, 0), 4, 0)) AS DUMMY_NO,CONCAT(SP.FIRST_NAME,'  (',DATE_FORMAT(DATE_OF_BIRTH,'%d-%m-%Y'),')') AS FIRST_NAME,IFNULL(A.IS_ABSENT,0) AS IS_ABSENT,OBE_ABSENT_ID
                                FROM
                                    STU_COURSE_INFO AS CLS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        LEFT JOIN
                                    EXAM_PACKET_PROCESS AS EPP ON EPP.STUDENT_ID=CLS.STUDENT_ID
										AND EPP.COURSE_ID=?COURSE_ID
                                        AND EPP.EXAM_SETTING_ID=?EXAM_SETTING_ID
                                        AND EPP.IS_DELETED!=1
                                        LEFT JOIN
									OBE_ABSENTEE AS A ON A.STUDENT_ID=CLS.STUDENT_ID
                                        AND A.COURSE_ID=CLS.COURSE_ID
                                        AND A.EXAM_SETTING_ID=?EXAM_SETTING_ID
                                        AND A.OBE_COMPONENT_ID=?OBE_COMPONENT_ID
                                        AND A.IS_DELETED!=1
                                WHERE
                                    CLS.S_CLASS_ID = ?CLASS_ID
                                        AND CLS.COURSE_ID = ?COURSE_ID
                                        AND CLS.IS_DELETED != 1
                                        AND CLS.ACADEMIC_YEAR = ?AC_YEAR
                                ORDER BY REGISTER_NO;";
                        break;
                    }
                case OBESQLCommands.FetchOBEMarkEntryStructureByComponent:
                    {
                        query = @"SELECT 
                                MS.OBE_QUESTIONPAPERSTRUCTURE_ID,
                                MS.OBE_QUESTIONPAPERPATTERN_ID,
                                PS.MARK_FOR_EACH_QUESTION AS MARK_MAX,
                                MS.QUESTION_NUMBER,
                                MS.OBE_QUESTIONLEVEL_ID,
                                QL.OBE_QUESTIONLEVEL,
                                MS.QUESTION_LEVEL_ID,
                                RB.OBE_RBT_LEVEL,
                                MS.OBE_COLEVEL_ID,
                                CO.OBE_COLEVEL,
                                MS.OBE_COMPONENT_ID,
                                MS.EXAM_SETTING_ID,
                                PS.SECTION_ID,
                                PS.TO_BE_ATTEND
                            FROM
                                OBE_QUESTIONPAPER_STRUCTURE AS MS
                                    INNER JOIN
                                OBE_QUESTIONPAPER_PATTERN_SECTION AS PS ON PS.OBE_QUESTIONPAPERPATTERNSECTION_ID=MS.OBE_QUESTIONPAPERPATTERNSECTION_ID
                                AND PS.IS_DELETED!=1
                                    LEFT JOIN
                                SUP_OBE_RBT_LEVEL AS RB ON RB.OBE_RBT_LEVEL_ID = MS.OBE_QUESTIONLEVEL_ID
                                    LEFT JOIN
                                SUP_OBE_CO_LEVEL AS CO ON CO.OBE_COLEVEL_ID = MS.OBE_COLEVEL_ID
                                    LEFT JOIN
                                SUP_OBE_QUESTIONLEVEL AS QL ON QL.OBE_QUESTIONLEVEL_ID = MS.QUESTION_LEVEL_ID
                            WHERE
                                MS.IS_DELETED != 1
                                    AND EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                    AND MS.OBE_QUESTIONPAPERPATTERN_ID = ?OBE_QUESTIONPAPERPATTERN_ID
                                    AND OBE_COMPONENT_ID = ?OBE_COMPONENT_ID;";
                        break;
                    }
                case OBESQLCommands.FetchPatternIdByExamSettingandComponentId:
                    {
                        query = @"SELECT 
                                         PATTERN_ID
                                     FROM
                                         OBE_COURSE_PATTERN
                                     WHERE
                                         EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                             AND OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                             AND COURSE_ID = ?COURSE_ID
                                     GROUP BY COURSE_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCourseBasicInformationByCourseId:
                    {
                        query = @"SELECT 
                                CR.COURSE_ROOT_ID,
                                CR.PROGRAMME_LEVEL_ID,
                                CR.DEPARTMENT AS DEPARTMENT_ID,
                                D.DEPARTMENT AS PROGRAMME,
                                CR.PART,
                                CR.COURSE_TYPE,
                                CR.CREDITS,
                                CR.OPTION_NAME,
                                CR.COURSE_TITLE,
                                CR.COURSE_CODE,
                                CR.SEMESTER_ID,
                                CR.IS_COMPULSORY,
                                CR.UGC_COURSE_TYPE,
                                CR.INTERNAL,
                                CR.EXTERNAL,
                                CR.TOTAL,
                                CR.EXAM_MARK,
                                CR.PAPER_TYPE,
                                CR.CONS_COURSE_TITLE,
                                CR.SUBJECT_TYPE
                            FROM
                                CP_COURSE_ROOT AS CR
                                LEFT JOIN CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CR.DEPARTMENT
                            WHERE
                                CR.COURSE_ROOT_ID = ?COURSE_ROOT_ID;";
                        break;
                    }
                case OBESQLCommands.FetchAcademicYearByExamSetting:
                    {
                        query = @"SELECT 
                                EXAM_SETTING_ID, 
                                EXAM_NAME, 
                                ACADEMIC_YEAR,
                                M_PASS
                            FROM
                                EXAM_SETTING
                            WHERE
                                ACADEMIC_YEAR = ?ACADEMIC_YEAR 
                                AND IS_DELETED != 1;";

                        break;
                    }
                case OBESQLCommands.FetchClassList:
                    {
                        query = @"SELECT 
                                        CS.CLASS_ID,
                                        CLS.CLASS_NAME,
                                        CLS.PROGRAMME_GROUP_ID,
                                        CLS.CLASS_LEVEL,
                                        CLS.SHIFT,
                                        CLS.PROGRAMME_MODE,
                                        CLS.DEPARTMENT
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CS.COURSE_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        CS.STAFF_ID = ?STAFF_ID AND CS.IS_DELETED != 1
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CS.CLASS_ID;";

                        break;
                    }
                case OBESQLCommands.IsCiaVerifiedByCourseId:
                    {
                        query = @"SELECT 
                                      COURSE_PATTERN_ID,COURSE_ID,CP.PROGRAMME_GROUP_ID,CC.IS_ACTIVE
                                  FROM
                                      CP_COURSE_PATTERN AS CP
                                          INNER JOIN
                                      CIA_SETTING AS CC ON CC.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                  		AND CC.IS_ACTIVE = 1
                                          AND CC.IS_DELETED != 1
                                  WHERE
                                      ACADEMIC_YEAR = ?AC_YEAR
                                          AND COURSE_ID = ?COURSE_ID
                                            AND DATE_FROM<=CURDATE() AND CURDATE()<= DATE_TO;";
                        break;
                    }
                case OBESQLCommands.FetchexamDummyCourseByCourseIdandExamSettingId:
                    {
                        query = @"SELECT DUMMY_ID,COURSE_ID,EXAM_SETTING_ID FROM EXAM_DUMMY_COURSE WHERE IS_DELETED!=1 AND EXAM_SETTING_ID=?EXAM_SETTING_ID AND COURSE_ID=?COURSE_ID";
                        break;
                    }
                case OBESQLCommands.FetchCIAMarkExistByStudentForCIAPush:
                    {
                        query = "SELECT CIA_GROUP_MARK_ID, STUDENT_ID, COURSE_ID, COURSE_GROUP_ID, COURSE_GROUP_MARK, SEMESTER_ID FROM CIA_GROUP_MARKS_?AC_YEAR WHERE STUDENT_ID = ?STUDENT_ID AND COURSE_ID= ?COURSE_ID AND COURSE_GROUP_ID=?COURSE_GROUP_ID;";
                        break;
                    }
                case OBESQLCommands.FetchObeFinalSubmitReportByExamSettingId:
                    {
                        query = @"SELECT 
                                      OBE_EXAM_MARK_ENTERED_ID,
                                      STAFF_CODE,
                                      FIRST_NAME,
                                      COURSE_CODE,
                                      COURSE_TITLE,
                                      CLASS_NAME,
                                      OB.IS_DELETED
                                  FROM
                                      OBE_EXAM_MARK_ENTERED AS OB
                                          INNER JOIN
                                      STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = OB.STAFF_ID
                                          INNER JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OB.COURSE_ID
                                          INNER JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = OB.CLASS_ID
                                  WHERE
                                      EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                  ORDER BY OB.STAFF_ID,OB.COURSE_ID;";
                        break;
                    }
                case OBESQLCommands.UpdateDeletedFlogForCoe:
                    {
                        query = @"UPDATE OBE_EXAM_MARK_ENTERED SET IS_DELETED=?IS_DELETED WHERE OBE_EXAM_MARK_ENTERED_ID=?OBE_EXAM_MARK_ENTERED_ID;";
                        break;
                    }
                case OBESQLCommands.Fetchobecopomatrix:
                    {
                        query = @"SELECT 
                                      OBE_MAPPING_ID,
                                      OBE_COLEVEL_ID,
                                      OBE_POLEVEL_ID,
                                      COURSE_ID,
                                      OBE_CO_PO_MARK,
                                      ENTRY_ID,
                                      PO_TYPE,
                                      ACADEMIC_YEAR,
                                      IS_DELETED
                                  FROM
                                      OBE_CO_PO_MATRIX
                                  WHERE
                                      COURSE_ID = ?COURSE_ID AND PO_TYPE = ?PO_TYPE;";
                        break;
                    }
                case OBESQLCommands.Fetchdepartmentbystaff:
                    {
                        query = @"SELECT DEPARTMENT FROM STF_PERSONAL_INFO WHERE STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case OBESQLCommands.InsertOBECOPOMatrix:
                    {
                        query = @"INSERT INTO OBE_CO_PO_MATRIX ( OBE_COLEVEL_ID, OBE_POLEVEL_ID, OBE_CO_PO_MARK, COURSE_ID,PO_TYPE,ENTRY_ID) VALUES 
                                (?OBE_COLEVEL_ID, ?OBE_POLEVEL_ID, ?OBE_CO_PO_MARK, ?COURSE_ID,?PO_TYPE,?ENTRY_ID);";
                        break;
                    }
                case OBESQLCommands.UpdateOBECOPOMatrix:
                    {
                        query = @"UPDATE OBE_CO_PO_MATRIX SET OBE_CO_PO_MARK=?OBE_CO_PO_MARK WHERE OBE_MAPPING_ID=?OBE_MAPPING_ID;";
                        break;
                    }
                case OBESQLCommands.InsertOBECOPSOMatrix:
                    {
                        query = @"INSERT INTO OBE_CO_PSO_MATRIX ( OBE_COLEVEL_ID, OBE_PSOLEVEL_ID, OBE_CO_PSO_MARK, COURSE_ID,PO_TYPE) VALUES 
                                (?OBE_COLEVEL_ID, ?OBE_POLEVEL_ID, ?OBE_CO_PO_MARK, ?COURSE_ID,?PO_TYPE);";
                        break;
                    }
                case OBESQLCommands.UpdateOBECOPSOMatrix:
                    {
                        query = @"UPDATE OBE_CO_PSO_MATRIX SET OBE_CO_PO_MARK=?OBE_CO_PO_MARK WHERE OBE_MAPPING_ID=?OBE_MAPPING_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCoLevelByClassId:
                    {
                        query = @"SELECT 
                              EXAM_SETTING_ID,
                              COURSE_ID,
                              CO_LEVEL,
                              EXTERNAL_SEC,
                              EXTERNAL,
                              INTERNAL_SEC,
                              INTERNAL,
                              TOTAL_ATTAINMENT,
                              ACADEMIC_YEAR
                          FROM
                              OBE_CLO_ATTAINMENT
                          WHERE
                              EXAM_SETTING_ID = ?EXAM_SETTING_ID
                          AND COURSE_ID = ?COURSE_ID 
                          AND IS_DELETED!=1;";
                        break;
                    }
                case OBESQLCommands.FetchCourseWiseCLOAttainment:
                    {
                        query = @"SELECT 
                                A.EXAM_SETTING_ID,
                                A.COURSE_ID,
                                A.CO_LEVEL,
                                C.OBE_COLEVEL,
                                A.EXTERNAL_SEC,
                                A.EXTERNAL,
                                A.INTERNAL_SEC,
                                A.INTERNAL,
                                A.TOTAL_ATTAINMENT,
                                A.ACADEMIC_YEAR,
                                CCR.COURSE_CODE,
                                CCR.COURSE_TITLE
                            FROM
                                OBE_CLO_ATTAINMENT AS A
                                    INNER JOIN
                                SUP_OBE_CO_LEVEL AS C ON C.OBE_COLEVEL_ID = A.CO_LEVEL
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = A.COURSE_ID
                            WHERE
		                            A.EXAM_SETTING_ID =?EXAM_SETTING_ID
                                    AND A.COURSE_ID =?COURSE_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCourseBySemester:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(COURSE_CODE, '-', COURSE_TITLE) AS COURSE_TITLE
                                FROM
                                    CP_COURSE_ROOT
                                WHERE
                                    SEMESTER_ID=?SEMESTER
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchOBECOAttainmentsByExamandComponent:
                    {
                        query = @"SELECT 
                        COA.CLASS_ID,
                        COA.COURSE_ID,
                        COA.EXAM_SETTING_ID,
                        COA.COMPONENT_ID,
                        COA.CO_LEVEL,
                        COA.RBT_LEVEL,
                        COA.NO_OF_STUDENT,
                        COA.THRESSHOLD,
                        COA.ATTAINMENT,
                        COA.POINTSCALE,
                        SC.EXAM_TYPE
                    FROM
                        OBE_CO_ATTAINMENTS AS COA
                            INNER JOIN
                        OBE_CO_ATTAINMENT_COMPONENT AS SC ON SC.OBE_COMPONENT_ID = COA.COMPONENT_ID
                    WHERE
                        COA.IS_DELETED != 1
                            AND COA.EXAM_SETTING_ID =?EXAM_SETTING_ID
                            AND COA.COMPONENT_ID IN (?COMPONENT_ID);";
                        break;
                    }
                case OBESQLCommands.FetchOBECoAttainmentsList:
                    {
                        query = @"SELECT 
                                      OCA.CO_ATTAINMENT_ID,
                                      OCA.CLASS_ID,CC.CLASS_NAME,
                                      OCA.COURSE_ID,CCR.COURSE_TITLE,CCR.COURSE_CODE,
                                      OCA.EXAM_SETTING_ID,CP.PROGRAMME_NAME,
                                      OCA.COMPONENT_ID,
                                      OCA.CO_LEVEL,CO.OBE_COLEVEL,
                                      OCA.RBT_LEVEL,RBT.OBE_RBT_LEVEL,
                                      OCA.QUS_NO,
                                      OCA.NO_OF_STUDENT,
                                      OCA.THRESSHOLD,
                                      OCA.ATTAINMENT,
                                      OCA.POINTSCALE,PS.MARK_FOR_EACH_QUESTION  as MAX_MARK
                                  FROM
                                    OBE_QUESTIONPAPER_STRUCTURE AS MS
                                        INNER JOIN
                                    OBE_QUESTIONPAPER_PATTERN_SECTION AS PS ON  PS.OBE_QUESTIONPAPERPATTERNSECTION_ID = MS.OBE_QUESTIONPAPERPATTERNSECTION_ID
                                        AND PS.IS_DELETED != 1
                                        INNER JOIN
                                    OBE_COURSE_PATTERN AS OCP ON OCP.EXAM_SETTING_ID = MS.EXAM_SETTING_ID
                                        AND OCP.COURSE_ID = ?COURSE_ID
                                        AND OCP.OBE_COMPONENT_ID = MS.OBE_COMPONENT_ID
                                        AND OCP.PATTERN_ID = PS.OBE_QUESTIONPAPERPATTERN_ID
                                        LEFT JOIN
                                    OBE_CO_ATTAINMENTS AS OCA ON OCA.QUS_NO = MS.QUS_NO
                                        AND OCA.EXAM_SETTING_ID = MS.EXAM_SETTING_ID
                                        AND OCA.CLASS_ID = ?CLASS_ID
                                        AND OCA.COURSE_ID = ?COURSE_ID
                                        AND OCA.COMPONENT_ID = ?COMPONENT_ID
                                        AND OCA.IS_DELETED!=1
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = OCA.COURSE_ID
                                        LEFT JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = OCA.CLASS_ID
                                        LEFT JOIN
                                    SUP_OBE_CO_LEVEL AS CO ON CO.OBE_COLEVEL_ID = OCA.CO_LEVEL
                                        LEFT JOIN
                                    SUP_OBE_RBT_LEVEL AS RBT ON RBT.OBE_RBT_LEVEL_ID = OCA.RBT_LEVEL
                                        LEFT JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                        LEFT JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                WHERE
                                    PS.IS_DELETED != 1 AND MS.IS_DELETED != 1
                                        AND MS.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                        AND MS.OBE_COMPONENT_ID = ?COMPONENT_ID
                                        ORDER BY CO.OBE_COLEVEL,OCA.QUS_NO;";
                        break;
                    }

                case OBESQLCommands.FetchObeMarksListBycourseId:
                    {
                        query = @"SELECT 
                              OME.OBE_MARKENTRY_ID,
                              OME.OBE_QUESTIONMARKSTRUCTURE_ID,
                              OME.STUDENT_ID,SPI.REGISTER_NO,SPI.ROLL_NO,SPI.FIRST_NAME,
                              OME.COURSE_ID,
                              OME.TOTAL_MARK,
                              OME.SECURED_MARK,
                              OME.QUESTION_NUMBER,
                              OCPP.OBE_QUESTIONLEVEL_ID as OBE_RBTLEVEL_ID,
                              OCPP.OBE_COLEVEL_ID as OBE_COLEVEL_ID,
                              OME.OBE_COMPONENT_ID,cac.OBE_COMPONENT,
                              OME.EXAM_SETTING_ID,
                              OME.EXAM_TYPE,
                              OME.ENTRY_ID,
                              OME.ACADEMIC_YEAR
                          FROM
                              OBE_MARKENTRY AS OME
                              INNER JOIN
                              EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID=OME.EXAM_SETTING_ID
                              INNER JOIN
                              CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=OME.COURSE_ID
                              INNER JOIN
                              OBE_CO_ATTAINMENT_COMPONENT AS CAC ON CAC.OBE_COMPONENT_ID=OME.OBE_COMPONENT_ID
                              INNER JOIN
                              OBE_QUESTIONPAPER_STRUCTURE AS OCPP ON OCPP.EXAM_SETTING_ID=OME.EXAM_SETTING_ID
                              AND OCPP.OBE_QUESTIONPAPERSTRUCTURE_ID=OME.OBE_QUESTIONMARKSTRUCTURE_ID
                              INNER JOIN
                              STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=OME.STUDENT_ID
                              INNER JOIN
                              STU_CLASS AS SC ON SC.STUDENT_ID=SPI.STUDENT_ID
                              AND SC.ACADEMIC_YEAR=?AC_YEAR
                              AND SC.CLASS_ID=?CLASS_ID
                          WHERE
                              OME.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                  AND OME.COURSE_ID = ?COURSE_ID 
                                  AND OME.OBE_COMPONENT_ID=?OBE_COMPONENT_ID
                                  AND OME.IS_DELETED!=1;";
                        break;
                    }
                case OBESQLCommands.FetchOBECoAttainmentsById:
                    {
                        query = @"SELECT CO_ATTAINMENT_ID FROM OBE_CO_ATTAINMENTS  WHERE CLASS_ID=?CLASS_ID AND COURSE_ID=?COURSE_ID AND EXAM_SETTING_ID=?EXAM_SETTING_ID AND COMPONENT_ID=?COMPONENT_ID AND IS_DELETED!=1;";
                        break;
                    }
                case OBESQLCommands.FetchobeCoursepaperPatternById:
                    {
                        query = @"SELECT 
                                      OBE_COURSEPAPERPATTERN_ID,
                                      EXAM_SETTING_ID,
                                      COURSE_ID,
                                      QUS_NO,
                                      OBE_COMPONENT_ID,
                                      OBE_COLEVEL_ID,
                                      OBE_RBTLEVEL_ID,
                                      ACADEMIC_YEAR,CLASS_ID
                                  FROM
                                      OBE_COURSE_PAPER_PATTERN
                                  WHERE
                                      IS_DELETED != 1 AND EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                          AND COURSE_ID = ?COURSE_ID
                                          AND OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                          AND CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case OBESQLCommands.FetchQuestionPaperMarkEntryById:
                    {
                        query = @"SELECT 
                                    OME.OBE_MARKENTRY_ID,
                                    OME.OBE_QUESTIONMARKSTRUCTURE_ID,
                                    OME.STUDENT_ID,
                                    SPI.REGISTER_NO,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    OME.COURSE_ID,
                                    OME.TOTAL_MARK,
                                    OME.SECURED_MARK,
                                    OME.QUESTION_NUMBER,
                                    OCPP.OBE_QUESTIONLEVEL_ID AS OBE_RBTLEVEL_ID,
                                    OCPP.OBE_COLEVEL_ID,
                                    OME.OBE_COMPONENT_ID,
                                    CAC.OBE_COMPONENT,
                                    OME.EXAM_SETTING_ID,
                                    OME.EXAM_TYPE,
                                    OME.ENTRY_ID,
                                    OME.ACADEMIC_YEAR,
                                    OCPP.QUS_NO,
                                    SC.CLASS_ID,
                                    CCR.PROGRAMME_LEVEL_ID
                                FROM
                                    OBE_MARKENTRY AS OME
                                        INNER JOIN
                                    EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = OME.EXAM_SETTING_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = OME.COURSE_ID
                                        INNER JOIN
                                    OBE_CO_ATTAINMENT_COMPONENT AS CAC ON CAC.OBE_COMPONENT_ID = OME.OBE_COMPONENT_ID
                                        INNER JOIN
                                    OBE_QUESTIONPAPER_STRUCTURE AS OCPP ON OCPP.EXAM_SETTING_ID = OME.EXAM_SETTING_ID
                                        AND OCPP.OBE_QUESTIONPAPERSTRUCTURE_ID = OME.OBE_QUESTIONMARKSTRUCTURE_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = OME.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ES.ACADEMIC_YEAR
                                WHERE
                                    OME.EXAM_SETTING_ID =?EXAM_SETTING_ID
                                    AND OME.COURSE_ID =?COURSE_ID 
                                    AND SC.CLASS_ID =?CLASS_ID
                                        AND OME.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                        AND OME.IS_DELETED != 1 
                                UNION SELECT 
                                    OME.OBE_MARKENTRY_ID,
                                    OME.OBE_QUESTIONMARKSTRUCTURE_ID,
                                    OME.STUDENT_ID,
                                    SPI.REGISTER_NO,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    OME.COURSE_ID,
                                    OME.TOTAL_MARK,
                                    OME.SECURED_MARK,
                                    OME.QUESTION_NUMBER,
                                    OCPP.OBE_QUESTIONLEVEL_ID AS OBE_RBTLEVEL_ID,
                                    OCPP.OBE_COLEVEL_ID,
                                    OME.OBE_COMPONENT_ID,
                                    CAC.OBE_COMPONENT,
                                    OME.EXAM_SETTING_ID,
                                    OME.EXAM_TYPE,
                                    OME.ENTRY_ID,
                                    OME.ACADEMIC_YEAR,
                                    OCPP.QUS_NO,
                                    SC.CLASS_ID,
                                    CCR.PROGRAMME_LEVEL_ID
                                FROM
                                    OBE_MARKENTRY AS OME
                                        INNER JOIN
                                    EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = OME.EXAM_SETTING_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = OME.COURSE_ID
                                        INNER JOIN
                                    OBE_CO_ATTAINMENT_COMPONENT AS CAC ON CAC.OBE_COMPONENT_ID = OME.OBE_COMPONENT_ID
                                        INNER JOIN
                                    OBE_QUESTIONPAPER_STRUCTURE AS OCPP ON OCPP.EXAM_SETTING_ID = OME.EXAM_SETTING_ID
                                        AND OCPP.OBE_QUESTIONPAPERSTRUCTURE_ID = OME.OBE_QUESTIONMARKSTRUCTURE_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = OME.STUDENT_ID
                                        INNER JOIN
                                    STU_COURSE_INFO AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ES.ACADEMIC_YEAR and sc.COURSE_ID= OME.COURSE_ID
                                WHERE
                                    OME.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                        AND OME.COURSE_ID = ?COURSE_ID
                                        AND SC.S_CLASS_ID = ?CLASS_ID
                                        AND OME.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                        AND OME.IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchobeRBTLevel:
                    {
                        query = @"SELECT 
                                      OBE_RBT_LEVEL_ID, OBE_RBT_LEVEL
                                  FROM
                                      SUP_OBE_RBT_LEVEL
                                  WHERE
                                      IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchOBEMarkEntryStructureByComponents:
                    {

                        query = @"SELECT 
                                MS.OBE_QUESTIONPAPERSTRUCTURE_ID,
                                MS.OBE_QUESTIONPAPERPATTERN_ID,
                                ROUND(PS.MARK_FOR_EACH_QUESTION,0) AS MARK_MAX,
                                MS.QUESTION_NUMBER,MS.QUS_NO,
                                MS.OBE_QUESTIONLEVEL_ID AS OBE_RBTLEVEL_ID,
                                QL.OBE_QUESTIONLEVEL,
                                MS.QUESTION_LEVEL_ID,
                                RB.OBE_RBT_LEVEL,
                                MS.OBE_COLEVEL_ID,
                                CO.OBE_COLEVEL,
                                MS.OBE_COMPONENT_ID,
                                MS.EXAM_SETTING_ID,
                                PS.SECTION_ID,
                                PS.TO_BE_ATTEND,
                                PS.EITHER_OR,PS.MARK_FOR_EACH_QUESTION
                            FROM
                                OBE_QUESTIONPAPER_STRUCTURE AS MS
                                    INNER JOIN
                                OBE_QUESTIONPAPER_PATTERN_SECTION AS PS ON PS.OBE_QUESTIONPAPERPATTERNSECTION_ID=MS.OBE_QUESTIONPAPERPATTERNSECTION_ID
                                AND PS.IS_DELETED!=1
                                    LEFT JOIN
                                SUP_OBE_RBT_LEVEL AS RB ON RB.OBE_RBT_LEVEL_ID = MS.OBE_QUESTIONLEVEL_ID
                                    LEFT JOIN
                                SUP_OBE_CO_LEVEL AS CO ON CO.OBE_COLEVEL_ID = MS.OBE_COLEVEL_ID
                                    LEFT JOIN
                                SUP_OBE_QUESTIONLEVEL AS QL ON QL.OBE_QUESTIONLEVEL_ID = MS.QUESTION_LEVEL_ID
                            WHERE
                                MS.IS_DELETED != 1  
                                    AND MS.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                    AND MS.OBE_QUESTIONPAPERPATTERN_ID = ?OBE_QUESTIONPAPERPATTERN_ID
                                    AND MS.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID;";
                        break;
                    }
                case OBESQLCommands.FetchPoLevelByClassId:
                    {
                        query = @"SELECT 
                                      OBE_MAPPING_ID,
                                      OBE_COLEVEL_ID,
                                      OBE_POLEVEL_ID,
                                      OBE_CO_PO_MARK,
                                      COURSE_ID
                                  FROM
                                      OBE_CO_PO_MATRIX
                                  WHERE
                                      COURSE_ID = ?COURSE_ID AND PO_TYPE=?PO_TYPE
                                  AND IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchCoPoAttainmentsById:
                    {
                        query = @"SELECT OBE_CO_PO_ATTAINMENT_ID FROM OBE_CO_PO_ATTAINMENTS WHERE IS_DELETED!=1 AND EXAM_SETTING_ID=?EXAM_SETTING_ID AND OBE_COLEVEL_ID=?OBE_COLEVEL_ID AND COURSE_ID=?COURSE_ID AND PO_TYPE=?PO_TYPE; ";
                        break;
                    }
                case OBESQLCommands.FetchCoPoAttainmentsByIdandcourseId:
                    {
                        query = @"SELECT 
                                      COPO.OBE_CO_PO_ATTAINMENT_ID,
                                      COPO.OBE_COLEVEL_ID,CO.OBE_COLEVEL,
                                      COPO.OBE_POLEVEL_ID,PO.OBE_POLEVEL,PSO.OBE_PSOLEVEL,
                                      COPO.OBE_CO_PO_MARK,
                                      COPO.EXAM_SETTING_ID,ES.EXAM_NAME,ES.ACADEMIC_YEAR,
                                      COURSE_ID,CCR.COURSE_CODE,CCR.COURSE_TITLE,
                                      COPO.IS_DELETED,
                                      COPO.PO_TYPE
                                  FROM
                                      OBE_CO_PO_ATTAINMENTS AS COPO
                                      INNER JOIN
                                      EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID=COPO.EXAM_SETTING_ID
                                      INNER JOIN
                                      CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=COPO.COURSE_ID
                                      INNER JOIN
                                      SUP_OBE_PO_LEVEL AS PO ON PO.OBE_POLEVEL_ID=COPO.OBE_POLEVEL_ID
                                      INNER JOIN
                                      SUP_OBE_CO_LEVEL AS CO ON CO.OBE_COLEVEL_ID=COPO.OBE_COLEVEL_ID
                                      INNER JOIN
                                      SUP_OBE_PSO_LEVEL AS PSO ON PSO.OBE_PSOLEVEL_ID=COPO.OBE_POLEVEL_ID
                                  WHERE
                                      COPO.IS_DELETED != 1 AND COPO.COURSE_ID = ?COURSE_ID AND COPO.PO_TYPE = ?PO_TYPE
                                          AND COPO.EXAM_SETTING_ID = ?EXAM_SETTING_ID;";
                        break;
                    }
                case OBESQLCommands.FetchObeExternalVerify:
                    {
                        query = @"SELECT 
                                    COURSE_ID,
                                    SEMESTER,
                                    EXAM_SETTING_ID,
                                    ACADEMIC_YEAR
                                 FROM 
                                    OBE_EXAM_MARK_ENTERED 
                                WHERE
                                    STAFF_ID=?STAFF_ID 
                                    AND COURSE_ID=?COURSE_ID 
                                    AND EXAM_SETTING_ID=?EXAM_SETTING_ID 
                                    AND IS_DELETED!=1 
                                    AND CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCourseCIAComponentByCoursetype:
                    {
                        query = @"SELECT 
                                GC.CIA_GROUP_COMPONENT_ID,
                                GC.MAX_MARK,
                                SCC.COMPONENT,
                                CG.COURSE_TYPE_ID,
                                CG.GROUP_MARK,
                                GC.CIA_GROUP_ID,
                                GC.ORDER_ID
                            FROM
                                CIA_GROUP_COMPONENT AS GC
                                    INNER JOIN
                                CP_COURSE_TYPE_GROUP CG ON CG.GROUP_ID = GC.CIA_GROUP_ID
                                    INNER JOIN
                                SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                            WHERE
                                CG.COURSE_TYPE_ID =?COURSE_TYPE_ID
                            ORDER BY GC.ORDER_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCollegeDetails:
                    {
                        query = @"SELECT 
                                    UPPER(COLLEGE_CODE) AS 'COLLEGE_CODE',
                                    UPPER(COLLEGE_NAME) AS 'COLLEGE_NAME',
                                    CONCAT(UPPER(ADDRESS_ONE),'-',PINCODE) AS 'COLLEGE_ADDRESS',
                                    UNIVERSITY AS 'UNIVERSITY',
                                    GRADE AS 'GRADE',
                                    CONCAT(PLACE,' - ',PINCODE) AS 'PLACE',
                                    COLLEGE_LOGO,
                                    LOGIN_IMG_PATH,
                                    DISTRICT
                                FROM
                                    COLLEGE_DETAILS;";
                        break;
                    }
                case OBESQLCommands.FetchOnlyExternalMark:
                    {
                        query = @"SELECT 
                                    EM.EXTERNAL_MARK_ID,
                                    EM.EXTERNAL_MARK,
                                    '--' AS INTERNAL_MARK,
                                    EM.SEMESTER,
                                    EM.ACADEMIC_YEAR,
                                    EM.STUDENT_ID,
                                    EM.COURSE_ID,
                                    EM.EXAM_SETTING_ID,
                                    SM.SEMESTER_MARK_ID
                                FROM
                                    EXTERNAL_MARK AS EM
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = EM.COURSE_ID
                                        AND CR.PROGRAMME_LEVEL_ID = ?APPTYPE_ID
                                        LEFT JOIN
                                    SEMESTER_MARKS_2023 AS SM ON SM.STUDENT_ID = EM.STUDENT_ID
                                        AND SM.COURSE_ID = EM.COURSE_ID
                                        AND EM.EXAM_SETTING_ID = SM.EXAM_SETTING_ID
                                WHERE
                                    EM.EXAM_SETTING_ID = 54
                                HAVING SEMESTER_MARK_ID IS NULL
                                ;
";
                        break;
                    }
                case OBESQLCommands.FetchobecopomatrixEntryStatus:
                    {
                        query = @"SELECT 
                                        OM.COURSE_ID,
                                        COURSE_CODE,
                                        COURSE_TITLE,
                                        IF(T.COURSE_ID IS NULL, 0, 1) AS ENTRY_ID,
                                        IFNULL(IS_VERIFY, 0) AS IS_VERIFY
                                    FROM
                                        OBE_MARKENTRY AS OM
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OM.COURSE_ID
                                            AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                            AND CR.DEPARTMENT = ?DEPARTMENT_ID
                                            LEFT JOIN
                                        (SELECT 
                                            COURSE_ID, IS_VERIFY
                                        FROM
                                            OBE_CO_PO_MATRIX AS CP
                                        WHERE
                                            CP.PO_TYPE = ?PO_TYPE AND CP.IS_DELETED != 1
                                        GROUP BY COURSE_ID) AS T ON T.COURSE_ID = OM.COURSE_ID
                                    WHERE
                                        OM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND OM.IS_DELETED != 1
                                    GROUP BY OM.COURSE_ID
                                    ORDER BY CR.PROGRAMME_LEVEL_ID , CR.SEMESTER_ID , CR.COURSE_ORDER;";
                        break;
                    }
                case OBESQLCommands.FetchObeCalvulationForCOLevel:
                    {
                        query = @"SELECT 
                                        OQS.OBE_QUESTIONLEVEL_ID,
                                        OBE_RBT_LEVEL,
                                        QUESTION_LEVEL,
                                        OM.OBE_COLEVEL_ID, 
                                        SOC.OBE_COLEVEL,
                                        SUM(TOTAL_MARK) AS TOTAL_MARK,
                                        SUM(SECURED_MARK) AS SECURED_MARK,
                                        ROUND((SUM(SECURED_MARK) / SUM(TOTAL_MARK)) * 100,
                                                0) AS PERCENTAGE
                                    FROM
                                        OBE_MARKENTRY AS OM
											INNER JOIN
										SUP_OBE_CO_LEVEL AS SOC ON SOC.OBE_COLEVEL_ID=OM.OBE_COLEVEL_ID
                                            INNER JOIN
                                        OBE_QUESTIONPAPER_STRUCTURE AS OQS ON OQS.OBE_QUESTIONPAPERSTRUCTURE_ID = OM.OBE_QUESTIONMARKSTRUCTURE_ID
                                            INNER JOIN
                                        OBE_QUESTION_LEVEL AS QL ON OQS.OBE_QUESTIONLEVEL_ID = QL.QUESTION_LEVEL_ID
                                            INNER JOIN
                                        SUP_OBE_RBT_LEVEL AS RL ON RL.OBE_RBT_LEVEL_ID = QL.OBE_RBT_LEVEL_ID
                                            AND RL.IS_DELETED != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = OM.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        OM.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                            AND EXAM_TYPE = ?EXAM_TYPE
                                           -- P AND CP.PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID
                                           -- G AND SC.CLASS_ID = ?CLASS_ID
                                           -- S AND OM.STUDENT_ID = ?STUDENT_ID
                                           -- C AND OM.COURSE_ID = ?COURSE_ID
                                            AND OM.IS_DELETED != 1
                                    GROUP BY OBE_QUESTIONLEVEL_ID;";
                        break;
                    }
                case OBESQLCommands.FetchObeCalvulationForKLevel:
                    {
                        query = @"SELECT 
                                        OBE_QUESTIONLEVEL_ID,
                                        OBE_RBT_LEVEL,
                                        QUESTION_LEVEL,
                                        SUM(TOTAL_MARK) AS TOTAL_MARK,
                                        SUM(SECURED_MARK) AS SECURED_MARK,
                                        ROUND((SUM(SECURED_MARK) / SUM(TOTAL_MARK)) * 100,
                                                0) AS PERCENTAGE
                                    FROM
                                        OBE_MARKENTRY AS OM
                                            INNER JOIN
                                        OBE_QUESTIONPAPER_STRUCTURE AS OQS ON OQS.OBE_QUESTIONPAPERSTRUCTURE_ID = OM.OBE_QUESTIONMARKSTRUCTURE_ID
                                            INNER JOIN
                                        OBE_QUESTION_LEVEL AS QL ON OQS.OBE_QUESTIONLEVEL_ID = QL.QUESTION_LEVEL_ID
                                            INNER JOIN
                                        SUP_OBE_RBT_LEVEL AS RL ON RL.OBE_RBT_LEVEL_ID = QL.OBE_RBT_LEVEL_ID
                                            AND RL.IS_DELETED != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = OM.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        OM.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                            AND EXAM_TYPE = ?EXAM_TYPE
                                           -- P AND CP.PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID
                                           -- G AND SC.CLASS_ID = ?CLASS_ID
                                           -- S AND OM.STUDENT_ID = ?STUDENT_ID
                                           -- C AND OM.COURSE_ID = ?COURSE_ID
                                            AND OM.IS_DELETED != 1
                                    GROUP BY OBE_QUESTIONLEVEL_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCourseList:
                    {
                        query = @"SELECT 
                                        CS.COURSE_ID, UPPER(CR.COURSE_TITLE) AS 'COURSE_TITLE'
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CS.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        CS.CLASS_ID = ?CLASS_ID AND CS.STAFF_ID = ?STAFF_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.IS_DELETED != 1
                                    GROUP BY COURSE_ID;";

                        break;
                    }
                case OBESQLCommands.FetchStudentDetailsByStudentId:

                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO, 
                                    CONCAT(UPPER(IFNULL(SP.FIRST_NAME, '')),
                                            ' ',
                                            UPPER(IFNULL(SP.LAST_NAME, ''))) AS FIRST_NAME,
                                    SP.REGISTER_NO,
                                    SP.PHOTO
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1 AND SP.IS_LEFT!=1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CC.DEPARTMENT
                                WHERE
                                    SC.STUDENT_ID IN (?STUDENT_ID)
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY SP.STUDENT_ID";
                        break;
                    }
                case OBESQLCommands.FetchCourseDetailsByCourseId:
                    {
                        query = @"SELECT COURSE_CODE,COURSE_TITLE,'R' AS STATUS FROM CP_COURSE_ROOT WHERE COURSE_ROOT_ID IN (?COURSE_ID);";
                        break;
                    }
                case OBESQLCommands.FetchExamRegisteredListByStudentId:
                    {
                        query = @"SELECT 
                                        ER.REGISTRATION_ID,
                                        ER.STUDENT_ID,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        ER.STATUS
                                    FROM
                                        EXAM_REGISTRATION_?AC_YEAR AS ER
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ER.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                    WHERE
                                        ER.IS_DELETED != 1
                                            AND ER.STUDENT_ID = ?STUDENT_ID
                                            AND ER.EXAM_SETTING_ID = ?EXAM_SETTING_ID;";
                        break;
                    }
                case OBESQLCommands.UpdateOBEVerification:
                    {
                        query = @"UPDATE OBE_CO_PO_MATRIX SET IS_VERIFY=1,VERIFY_ID=?VERIFY_ID WHERE COURSE_ID=?COURSE_ID AND PO_TYPE=?PO_TYPE AND OBE_MAPPING_ID!=0;";
                        break;
                    }
                case OBESQLCommands.FetchCourseBySemesterIdandAcademicyearForObe:
                    {
                        query = @"SELECT 
                                CCR.COURSE_ROOT_ID,
                                CCR.COURSE_CODE,
                                CONCAT( CCR.COURSE_CODE,'-',CCR.COURSE_TITLE) AS COURSE_TITLE,
                                CCR.SEMESTER_ID,
                                CCR.UGC_COURSE_TYPE
                            FROM
                                CP_COURSE_PATTERN AS CCP
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                    AND CCR.SEMESTER_ID IN (?SEMESTER)
                                    AND CCR.PART IN (1,2,3)
                                    AND CCR.DEPARTMENT IN (?DEPARTMENT)
                            WHERE
                                CCP.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CCP.IS_DELETED != 1
                            GROUP BY CCR.COURSE_ROOT_ID
                            ORDER BY CCR.PART;";
                        break;
                    }
                case OBESQLCommands.FetchBloomsTaxonomyMaster:
                    {
                        query = @"SELECT bloom_id, k_level, cognitive_process, description, action_verbs, sample_question_stems, display_order, is_active FROM blooms_taxonomy_master WHERE is_active=1 ORDER BY display_order;";
                        break;
                    }
                case OBESQLCommands.FetchBloomsTaxonomyMasterById:
                    {
                        query = @"SELECT bloom_id, k_level, cognitive_process, description, action_verbs, sample_question_stems, display_order, is_active FROM blooms_taxonomy_master WHERE bloom_id=?BLOOM_ID AND is_active=1;";
                        break;
                    }
                case OBESQLCommands.InsertBloomsTaxonomyMaster:
                    {
                        query = @"INSERT INTO blooms_taxonomy_master (k_level, cognitive_process, description, action_verbs, sample_question_stems, display_order, is_active, created_by, created_date) VALUES (?K_LEVEL, ?COGNITIVE_PROCESS, ?DESCRIPTION, ?ACTION_VERBS, ?SAMPLE_QUESTION_STEMS, ?DISPLAY_ORDER, ?IS_ACTIVE, ?CREATED_BY, NOW());";
                        break;
                    }
                case OBESQLCommands.UpdateBloomsTaxonomyMaster:
                    {
                        query = @"UPDATE blooms_taxonomy_master SET k_level=?K_LEVEL, cognitive_process=?COGNITIVE_PROCESS, description=?DESCRIPTION, action_verbs=?ACTION_VERBS, sample_question_stems=?SAMPLE_QUESTION_STEMS, display_order=?DISPLAY_ORDER, is_active=?IS_ACTIVE, updated_by=?UPDATED_BY, updated_date=NOW() WHERE bloom_id=?BLOOM_ID;";
                        break;
                    }
                case OBESQLCommands.DeleteBloomsTaxonomyMaster:
                    {
                        query = @"UPDATE blooms_taxonomy_master SET is_active=0, updated_by=?UPDATED_BY, updated_date=NOW() WHERE bloom_id=?BLOOM_ID;";
                        break;
                    }
                case OBESQLCommands.FetchExamSettingForQuestionEntry:
                    {
                        query = @"SELECT EXAM_SETTING_ID, EXAM_NAME FROM EXAM_SETTING WHERE IS_ACTIVE=1 AND IS_DELETED!=1 ORDER BY EXAM_NAME;";
                        break;
                    }
                case OBESQLCommands.FetchCourseForQuestionEntry:
                    {
                        query = @"SELECT 
                                        OCP.COURSE_ID, COURSE_CODE, COURSE_TITLE
                                    FROM
                                        OBE_COURSE_PATTERN AS OCP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OCP.COURSE_ID 
                                    WHERE
                                        OCP.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                            AND OCP.IS_DELETED != 1 group by COURSE_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCourseForQuestionEntrybyStaffID:
                    {
                        query = @"SELECT 
                                        OCP.COURSE_ID, COURSE_CODE, COURSE_TITLE
                                    FROM
                                        OBE_COURSE_PATTERN AS OCP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OCP.COURSE_ID AND OCP.STAFF_ID = ?STAFF_ID
                                    WHERE
                                        OCP.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                            AND OCP.IS_DELETED != 1 group by COURSE_ID;";
                        break;
                    } 
                case OBESQLCommands.FetchCourseForQuestionEntryForExternalUser:
                    {
                        query = @"
                                  SELECT 
                                      OCP.COURSE_ID, COURSE_CODE, COURSE_TITLE
                                  FROM
                                      OBE_COURSE_PATTERN AS OCP
                                          INNER JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OCP.COURSE_ID
                                       INNER JOIN EXAM_EXAMINER_ORDER AS EEO ON EEO.COURSE_ID=OCP.COURSE_ID
                                  WHERE
                                      OCP.EXAM_SETTING_ID = ?EXAM_SETTING_ID AND EEO.STAFF_ID=?STAFF_ID
                                          AND OCP.IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchQuestionPatternFromCoursePattern:
                    {
                        query = @"SELECT CP.PATTERN_ID AS OBE_QUESTIONPAPERPATTERN_ID, P.OBE_QUESTIONPAPERPATTERN 
                                  FROM OBE_COURSE_PATTERN CP 
                                  INNER JOIN OBE_QUESTIONPAPER_PATTERN P ON CP.PATTERN_ID = P.OBE_QUESTIONPAPERPATTERN_ID
                                  WHERE CP.COURSE_ID=?COURSE_ID AND CP.EXAM_SETTING_ID=?EXAM_SETTING_ID AND CP.IS_DELETED!=1 AND P.IS_DELETED!=1;";
                        break;
                    }
                case OBESQLCommands.FetchUnitForQuestionEntry:
                    {
                        query = @"SELECT OBE_UNIT_ID, OBE_UNIT FROM SUP_OBE_UNIT WHERE IS_DELETED!=1 ORDER BY OBE_UNIT_ID;";
                        break;
                    }
                case OBESQLCommands.FetchQuestionLevelsForEntry:
                    {
                        query = @"SELECT OBE_QUESTIONLEVEL_ID as QUESTION_LEVEL_ID, OBE_QUESTIONLEVEL as QUESTION_LEVEL FROM SUP_OBE_QUESTIONLEVEL WHERE IS_DELETED!=1 ORDER BY QUESTION_LEVEL_ID;";
                        break;
                    }
                case OBESQLCommands.InsertExamQuestionEntry:
                    {
                        query = @"INSERT INTO OBE_QUESTIONS (QUESTION, FORMULA_EQUATION, COURSE_ID, EXAM_ID, QUESTION_LEVEL_ID, OBE_UNIT_ID, SUBMISSION_STATUS, CREATED_BY, CREATED_DATE, IS_ACTIVE, IS_DELETED) 
                                  VALUES (?QUESTION, ?FORMULA_EQUATION, ?COURSE_ID, ?EXAM_SETTING_ID, ?QUESTION_LEVEL_ID, ?OBE_UNIT_ID, ?SUBMISSION_STATUS, ?ENTRY_ID, NOW(), 1, 0);";
                        break;
                    }
                case OBESQLCommands.UpdateExamQuestionEntry:
                    {
                        query = @"UPDATE OBE_QUESTIONS 
                                  SET QUESTION=?QUESTION, FORMULA_EQUATION=?FORMULA_EQUATION, QUESTION_LEVEL_ID=?QUESTION_LEVEL_ID, OBE_UNIT_ID=?OBE_UNIT_ID, SUBMISSION_STATUS=?SUBMISSION_STATUS, UPDATED_BY=?ENTRY_ID, UPDATED_DATE=NOW() 
                                  WHERE QUESTION_ID=?QUESTION_ID AND SUBMISSION_STATUS='DRAFT';";
                        break;
                    }
                case OBESQLCommands.FinalSubmitExamQuestionEntry:
                    {
                        query = @"UPDATE OBE_QUESTIONS 
                                  SET SUBMISSION_STATUS='FINAL_SUBMITTED', FINAL_SUBMITTED_DATE=NOW(), UPDATED_BY=?ENTRY_ID, UPDATED_DATE=NOW() 
                                  WHERE QUESTION_ID=?QUESTION_ID;";
                        break;
                    }
                case OBESQLCommands.FetchExamQuestionEntryForPreview:
                    {
                        query = @"SELECT Q.QUESTION_ID, Q.QUESTION, Q.FORMULA_EQUATION, Q.SUBMISSION_STATUS, Q.FINAL_SUBMITTED_DATE, 
                                         U.OBE_UNIT AS OBE_UNIT, L.QUESTION_LEVEL AS OBE_QUESTIONLEVEL
                                  FROM OBE_QUESTIONS Q
                                  LEFT JOIN SUP_OBE_UNIT U ON Q.OBE_UNIT_ID = U.OBE_UNIT_ID
                                  LEFT JOIN SUP_OBE_QUESTIONLEVEL L ON Q.QUESTION_LEVEL_ID = L.QUESTION_LEVEL_ID
                                  WHERE Q.QUESTION_ID=?QUESTION_ID AND Q.IS_DELETED=0;";
                        break;
                    }
                case OBESQLCommands.FetchOBEQuestionsByCourseAndExam:
                    {
                        query = @"SELECT 
                                      oq.QUESTION_ID,
                                      oq.QUESTION,
                                      oq.FORMULA_EQUATION,
                                      sou.OBE_UNIT,
                                      oql.BRIEF_CAPTION,
                                      oq.SUBMISSION_STATUS
                                  FROM
                                      OBE_QUESTIONS AS oq
                                          INNER JOIN
                                      sup_obe_unit AS sou ON sou.OBE_UNIT_ID = oq.OBE_UNIT_ID
                                          INNER JOIN
                                      obe_question_level AS oql ON oq.QUESTION_LEVEL_ID = oql.QUESTION_LEVEL_ID
                                  WHERE
                                      COURSE_ID = ?COURSE_ID AND EXAM_ID = ?EXAM_SETTING_ID
                                          AND oq.IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchInternalAndExternalStaff:
                    {
                            query = @"SELECT 
                                            STAFF_ID,
                                            CONCAT(FIRST_NAME,
                                                    ' (',
                                                    STAFF_CODE,
                                                    ')',
                                                    ' - Internal') AS STAFF_NAME,
                                            '4' AS ROLES
                                        FROM
                                            STF_PERSONAL_INFO
                                        WHERE
                                            IS_DELETED != 1 AND IS_LEFT != 1 
                                        UNION SELECT 
                                            EXAMINAR_ID AS STAFF_ID,
                                            CONCAT(EXAMINAR_NAME,
                                                    ' (',
                                                    EXAMINAR_CODE,
                                                    ')',
                                                    ' - External') AS STAFF_NAME,
                                            '39' AS ROLES
                                        FROM
                                            EXAM_EXAMINARS_INFO
                                        WHERE
                                            IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.BindStaffWiseCourseList:
                    {
                        query = @"SELECT 
                                    U.STAFF_ID,
                                    U.STAFF_NAME,
                                    U.STAFF_CODE,
                                    U.EMAIL,
                                    IFNULL(D.DEPARTMENT, '') AS DEPARTMENT,
                                    COUNT(OCP.COURSE_ID) AS COURSE_COUNT,
                                    MAX(IFNULL(OCP.IS_MAIL_SENT, '0')) AS IS_MAIL_SENT,
                                    GROUP_CONCAT(CONCAT(CR.COURSE_CODE, ' - ', CR.COURSE_TITLE)
                                        SEPARATOR '<br/>') AS COURSE_LIST
                                FROM
                                    (SELECT 
                                        SPI.STAFF_ID,
                                            CONCAT(SPI.FIRST_NAME, ' - Internal') AS STAFF_NAME,
                                            SPI.STAFF_CODE,
                                            SPI.EMAIL,
                                            SPI.DEPARTMENT
                                    FROM
                                        STF_PERSONAL_INFO SPI
                                    WHERE
                                        SPI.IS_DELETED != 1 UNION ALL SELECT 
                                        - EX.EXAMINAR_ID AS STAFF_ID,
                                            CONCAT(EX.EXAMINAR_NAME, ' - External') AS STAFF_NAME,
                                            EX.EXAMINAR_CODE AS STAFF_CODE,
                                            EX.EMAIL_ID AS EMAIL,
                                            0 AS DEPARTMENT
                                    FROM
                                        EXAM_EXAMINARS_INFO EX
                                    WHERE
                                        EX.IS_DELETED != 1) U
                                        INNER JOIN
                                    OBE_COURSE_PATTERN OCP ON OCP.STAFF_ID = CAST(U.STAFF_ID AS CHAR)
                                        INNER JOIN
                                    CP_COURSE_ROOT CR ON CR.COURSE_ROOT_ID = OCP.COURSE_ID
                                        LEFT JOIN
                                    CP_DEPARTMENT D ON D.DEPARTMENT_ID = U.DEPARTMENT
                                WHERE
                                    OCP.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                        AND OCP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND OCP.OBE_COMPONENT_ID = ?OBE_COMPONENT_ID
                                        AND OCP.IS_DELETED != 1
                                GROUP BY U.STAFF_ID;";
                        break;
                    }
                case OBESQLCommands.DeleteOBECoursePattern:
                    {
                        query = @"DELETE FROM OBE_COURSE_PATTERN 
                                  WHERE COURSE_ID = ?COURSE_ID 
                                  AND EXAM_SETTING_ID = ?EXAM_SETTING_ID 
                                  AND OBE_COMPONENT_ID = ?OBE_COMPONENT_ID 
                                  AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case OBESQLCommands.UpdateStaffEmailStatus:
                    {
                        query = @"UPDATE OBE_COURSE_PATTERN 
                                  SET IS_MAIL_SENT = 1 
                                  WHERE FIND_IN_SET(?STAFF_ID, STAFF_ID) 
                                  AND EXAM_SETTING_ID = ?EXAM_SETTING_ID 
                                  AND OBE_COMPONENT_ID = ?OBE_COMPONENT_ID 
                                  AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case OBESQLCommands.FetchStaffCoursesWithSyllabus:
                    {
                        query = @"SELECT 
                                    OCP.COURSE_ID, CR.COURSE_CODE, CR.COURSE_TITLE, ASY.FILE_PATH
                                FROM
                                    OBE_COURSE_PATTERN AS OCP
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OCP.COURSE_ID
                                        LEFT JOIN
                                    ACADEMIC_SYLLABUS AS ASY ON ASY.COURSE_ID = OCP.COURSE_ID AND ASY.IS_DELETED != 1 AND ASY.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    OCP.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                        AND OCP.STAFF_ID = ?STAFF_ID
                                        AND OCP.IS_DELETED != 1 
                                GROUP BY OCP.COURSE_ID;";
                        break;
                    }
                case OBESQLCommands.FetchOBEClaimEntryByCourseAndStaff:
                    {
                        query = @"SELECT * FROM OBE_CLAIM_ENTRY WHERE COURSE_ID = ?COURSE_ID AND EXAM_SETTING_ID = ?EXAM_SETTING_ID AND IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.FetchOBEClaimEntryById:
                    {
                        query = @"SELECT * FROM OBE_CLAIM_ENTRY WHERE CLAIM_ID = ?CLAIM_ID AND IS_DELETED != 1;";
                        break;
                    }
                case OBESQLCommands.InsertOBEClaimEntry:
                    {
                        query = @"INSERT INTO OBE_CLAIM_ENTRY (EXAM_SETTING_ID, COURSE_ID, STAFF_ID, REMUNERATION_TYPE, NO_OF_QUESTION, QUESTION_AMOUNT, SCHEME_AMOUNT, POSTAL_CHARGES, TOTAL_CLAIM, RUPEES_IN_WORDS, STAFF_NAME, MOBILE_NO, DESIGNATION, OFFICIAL_ADDRESS, AC_NO, BANK, BRANCH, IFS_CODE, SUBMISSION_STATUS, ACADEMIC_YEAR) 
                                VALUES (?EXAM_SETTING_ID, ?COURSE_ID, ?STAFF_ID, ?REMUNERATION_TYPE, ?NO_OF_QUESTION, ?QUESTION_AMOUNT, ?SCHEME_AMOUNT, ?POSTAL_CHARGES, ?TOTAL_CLAIM, ?RUPEES_IN_WORDS, ?STAFF_NAME, ?MOBILE_NO, ?DESIGNATION, ?OFFICIAL_ADDRESS, ?AC_NO, ?BANK, ?BRANCH, ?IFS_CODE, ?SUBMISSION_STATUS, ?ACADEMIC_YEAR);";
                        break;
                    }
                case OBESQLCommands.UpdateOBEClaimEntry:
                    {
                        query = @"UPDATE OBE_CLAIM_ENTRY SET 
                                    REMUNERATION_TYPE=?REMUNERATION_TYPE, 
                                    NO_OF_QUESTION=?NO_OF_QUESTION, 
                                    QUESTION_AMOUNT=?QUESTION_AMOUNT, 
                                    SCHEME_AMOUNT=?SCHEME_AMOUNT, 
                                    POSTAL_CHARGES=?POSTAL_CHARGES, 
                                    TOTAL_CLAIM=?TOTAL_CLAIM, 
                                    RUPEES_IN_WORDS=?RUPEES_IN_WORDS, 
                                    STAFF_NAME=?STAFF_NAME, 
                                    MOBILE_NO=?MOBILE_NO, 
                                    DESIGNATION=?DESIGNATION, 
                                    OFFICIAL_ADDRESS=?OFFICIAL_ADDRESS, 
                                    AC_NO=?AC_NO, 
                                    BANK=?BANK, 
                                    BRANCH=?BRANCH, 
                                    IFS_CODE=?IFS_CODE, 
                                    SUBMISSION_STATUS=?SUBMISSION_STATUS 
                                WHERE CLAIM_ID=?CLAIM_ID;";
                        break;
                    }
                case OBESQLCommands.FetchFinalizedCoursesByStaff:
                    {
                        query = @"SELECT DISTINCT 
                                        OCP.COURSE_ID, 
                                        CR.COURSE_CODE, 
                                        CR.COURSE_TITLE
                                    FROM 
                                        OBE_COURSE_PATTERN AS OCP
                                    INNER JOIN 
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = OCP.COURSE_ID
                                    INNER JOIN 
                                        OBE_QUESTIONS AS Q ON Q.COURSE_ID = OCP.COURSE_ID AND Q.EXAM_ID = OCP.EXAM_SETTING_ID
                                    WHERE 
                                        OCP.EXAM_SETTING_ID = ?EXAM_SETTING_ID 
                                        AND (OCP.STAFF_ID = ?STAFF_ID OR FIND_IN_SET(?STAFF_ID, OCP.STAFF_ID))
                                        AND IFNULL(OCP.IS_DELETED, 0) != 1
                                        AND IFNULL(Q.IS_DELETED, 0) != 1;";
                        break;
                    }
                case OBESQLCommands.InsertCourseFinalStatus:
                    {
                        query = @"INSERT INTO OBE_COURSE_FINAL_STATUS (COURSE_ID, EXAM_SETTING_ID, STAFF_ID, STATUS, CREATED_DATE, CREATED_BY) 
                                  VALUES (?COURSE_ID, ?EXAM_SETTING_ID, ?STAFF_ID, 'FINAL_SUBMITTED', NOW(), ?ENTRY_ID)
                                  ON DUPLICATE KEY UPDATE STATUS='FINAL_SUBMITTED', CREATED_DATE=NOW(), CREATED_BY=?ENTRY_ID;";
                        break;
                    }
                case OBESQLCommands.FetchCourseByClassForReport:
                    {
                        query = @"SELECT 
                                    CCS.COURSE_ID,
                                    CR.COURSE_CODE,
                                    CONCAT(CR.COURSE_CODE, ' - ', CR.COURSE_TITLE) AS COURSE_TITLE
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CCS
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                WHERE
                                    CCS.CLASS_ID = ?CLASS_ID
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCS.IS_DELETED != 1
                                GROUP BY CCS.COURSE_ID
                                ORDER BY CR.COURSE_CODE;";
                        break;
                    }

                case OBESQLCommands.FetchCourseEntryStatusReport:
                    {
                        query = @"SELECT 
                                    DISTINCT CCS.COURSE_ID, 
                                    CR.COURSE_CODE, 
                                    CR.COURSE_TITLE,
                                    CR.SEMESTER_ID,
                                    SS.SEMESTER_NAME,
                                    CASE 
                                        WHEN Q.COURSE_ID IS NOT NULL THEN 'Entered'
                                        ELSE 'Not Entered'
                                    END AS ENTRY_STATUS,
                                    Q.EXAM_ID AS EXAM_SETTING_ID
                                FROM CP_CLASS_COURSE_STAFF CCS
                                INNER JOIN CP_COURSE_ROOT CR ON CCS.COURSE_ID = CR.COURSE_ROOT_ID
                                LEFT JOIN sup_semester AS SS ON SS.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                LEFT JOIN (
                                    SELECT COURSE_ID, MAX(EXAM_ID) as EXAM_ID
                                    FROM OBE_QUESTIONS 
                                    GROUP BY COURSE_ID
                                ) Q ON Q.COURSE_ID = CCS.COURSE_ID
                                WHERE CCS.ACADEMIC_YEAR = ?AC_YEAR
                                  AND CCS.IS_DELETED != 1
                                ORDER BY CR.COURSE_CODE;";
                        break;
                    }
                case OBESQLCommands.FetchExamQuestionEntryReport:
                    {
                        query = @"SELECT 
                                    Q.QUESTION_ID,
                                    Q.QUESTION,
                                    Q.FORMULA_EQUATION,
                                    Q.SUBMISSION_STATUS,
                                    Q.FINAL_SUBMITTED_DATE,
                                    Q.COURSE_ID,
                                    Q.EXAM_ID AS EXAM_SETTING_ID,
                                    U.OBE_UNIT AS OBE_UNIT,
                                    U.OBE_UNIT_ID,
                                    L.QUESTION_LEVEL AS OBE_QUESTIONLEVEL,
                                    L.OBE_QUESTIONLEVEL_ID AS QUESTION_LEVEL_ID,
                                    CR.COURSE_CODE,
                                    CR.COURSE_TITLE,
                                    ES.EXAM_NAME,
                                    CONCAT(SPI.FIRST_NAME, ' ', IFNULL(SPI.LAST_NAME,'')) AS STAFF_NAME
                                FROM OBE_QUESTIONS Q
                                LEFT JOIN SUP_OBE_UNIT U ON Q.OBE_UNIT_ID = U.OBE_UNIT_ID
                                LEFT JOIN SUP_OBE_QUESTIONLEVEL L ON Q.QUESTION_LEVEL_ID = L.OBE_QUESTIONLEVEL_ID
                                LEFT JOIN CP_COURSE_ROOT CR ON CR.COURSE_ROOT_ID = Q.COURSE_ID
                                LEFT JOIN EXAM_SETTING ES ON ES.EXAM_SETTING_ID = Q.EXAM_ID
                                LEFT JOIN STF_PERSONAL_INFO SPI ON SPI.STAFF_ID = Q.CREATED_BY
                                WHERE Q.COURSE_ID = ?COURSE_ID
                                    AND Q.IS_DELETED = 0
                                ORDER BY Q.EXAM_ID, Q.QUESTION_LEVEL_ID, Q.QUESTION_ID;";
                        break;
                    }
                case OBESQLCommands.FetchQuestionEntryStatusSummary:
                    {
                        query = @"SELECT 
                                    OQ.COURSE_ID,
                                    SPI.FIRST_NAME AS STAFF_NAME,
                                    CC.COURSE_TITLE,
                                    COUNT(*) AS ENTERED_COUNT
                                FROM OBE_QUESTIONS AS OQ
                                INNER JOIN STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = OQ.CREATED_BY
                                INNER JOIN CP_COURSE_ROOT AS CC ON CC.COURSE_ROOT_ID = OQ.COURSE_ID
                                WHERE OQ.EXAM_ID = ?EXAM_SETTING_ID AND OQ.IS_DELETED = 0
                                GROUP BY 
                                    OQ.COURSE_ID,
                                    SPI.FIRST_NAME,
                                    CC.COURSE_TITLE;";
                        break;
                    }
                case OBESQLCommands.FetchOBEClaimReport:
                    {
                        query = @"SELECT 
                                    OCE.*, 
                                    ES.EXAM_NAME,
                                    CR.COURSE_CODE,
                                    CR.COURSE_TITLE,
                                    CASE 
                                        WHEN OCE.SUBMISSION_STATUS = '1' THEN 'Draft'
                                        WHEN OCE.SUBMISSION_STATUS = '2' THEN 'Submitted'
                                        ELSE 'Unknown'
                                    END AS STATUS_TEXT
                                FROM OBE_CLAIM_ENTRY OCE
                                INNER JOIN EXAM_SETTING ES ON ES.EXAM_SETTING_ID = OCE.EXAM_SETTING_ID
                                INNER JOIN CP_COURSE_ROOT CR ON CR.COURSE_ROOT_ID = OCE.COURSE_ID
                                WHERE OCE.IS_DELETED != 1 
                                  AND OCE.ACADEMIC_YEAR = ?AC_YEAR
                                  -- P AND OCE.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                  -- S AND OCE.STAFF_ID = ?STAFF_ID
                                ORDER BY OCE.CREATED_DATE DESC;";
                        break;
                    }
            }
            
            return query;
        }
    }
}
