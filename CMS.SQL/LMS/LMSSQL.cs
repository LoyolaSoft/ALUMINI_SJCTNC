using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.LMS
{
    public static class LMSSQL
    {
        public static string GetLMSSQL(LMSSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case LMSSQLCommands.FetchStuAssignmentSubmissionById:
                    {
                        query = @"SELECT 
                                    LSAS.STU_ASSIGNMENT_ID,
                                    LSAS.STUDENT_ID,
                                    LSAS.COURSE_ID,
                                    LAS.TOTAL_MARK,
                                    LSAS.SEC_MARK,
                                    LSAS.ASSIGNMENT_SETTING_ID,
                                   LSAS.FILE_PATH
                                FROM
                                  LMS_ASSIGNMENT_SETTING AS LAS 
                                        INNER JOIN
                                    LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                WHERE
                                    LSAS.STU_ASSIGNMENT_ID =?STU_ASSIGNMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.UpdateAssignmentMark:
                    {
                        query = @"UPDATE LMS_STUDENT_ASSIGNMENT_SUBMISSION SET SEC_MARK=?SEC_MARK
                                  WHERE STU_ASSIGNMENT_ID=?STU_ASSIGNMENT_ID AND IS_DELETED!=1;";
                        break;
                    }

                case LMSSQLCommands.FetchAssignmentSubmissionBySettingId:
                    {
                        query = @"SELECT 
                                     LSAS.STU_ASSIGNMENT_ID,
                                     LSAS.STUDENT_ID,
                                     LSAS.COURSE_ID, 
                                     LSAS.TOTAL_MARK,
                                     LSAS.SEC_MARK, 
                                     LSAS.ASSIGNMENT_SETTING_ID, 
                                     LSAS.FILE_PATH,
                                     LSAS.ENTRY_DATE,LAS.ASSIGNMENT_NAME
                                    FROM
                                        LMS_ASSIGNMENT_SETTING AS LAS
                                            INNER JOIN
                                        LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS 
                                        ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                     WHERE LAS.ASSIGNMENT_SETTING_ID=?ASSIGNMENT_SETTING_ID 
                                     AND LAS.CLASS_ID=?CLASS_ID 
                                    AND LSAS.COURSE_ID=?COURSE_ID;";
                        break;
                    }
                case LMSSQLCommands.SavestuAssignmentAttachment:
                    {
                        query = @"UPDATE LMS_STUDENT_ASSIGNMENT_SUBMISSION SET FILE_PATH=?FILE_PATH WHERE STU_ASSIGNMENT_ID=?STU_ASSIGNMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.SavestuAssignment:
                    {
                        query = @"INSERT INTO LMS_STUDENT_ASSIGNMENT_SUBMISSION(STUDENT_ID,COURSE_ID,ASSIGNMENT_SETTING_ID)
                           VALUES(?STUDENT_ID,?COURSE_ID,?ASSIGNMENT_SETTING_ID);";
                        break;
                    }
                case LMSSQLCommands.FetchAssignmentSubmissionByClassandCourse:
                    {
                        query = @"SELECT 
                                      LSAS.STUDENT_ID,
                                      LSAS.COURSE_ID,
                                      LAS.ASSIGNMENT_SETTING_ID,
                                      LSAS.FILE_PATH
                                  FROM
                                      LMS_ASSIGNMENT_SETTING AS LAS
                                          INNER JOIN
                                      LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS 
                                      ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                   WHERE LSAS.COURSE_ID=?COURSE_ID AND LAS.CLASS_ID=?CLASS_ID;";


                        break;
                    }
                case LMSSQLCommands.FetchAssignmentSettingByClassandcourse:
                    {
                        query = @" SELECT 
                                S.ASSIGNMENT_SETTING_ID,
                                S.ASSIGNMENT_NAME,
                                S.STAFF_ID,
                                SP.FIRST_NAME,
                                SPI.FIRST_NAME,
                                S.COURSE_ID,
                                S.DESCRIPTION,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                CC.CLASS_NAME,
                                S.CLASS_ID,
                                S.TOTAL_MARK,
                                DATE_FORMAT(S.START_DATE, '%d/%m/%Y') AS START_DATE,
                                DATE_FORMAT(S.END_DATE, '%d/%m/%Y') AS END_DATE,
                                S.START_TIME,
                                S.END_TIME
                            FROM
                                LMS_ASSIGNMENT_SETTING AS S
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = S.CLASS_ID
                                INNER JOIN
                                stu_personal_info AS SPI ON SPI.STUDENT_ID=S.STUDENT_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = S.COURSE_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = S.STAFF_ID
                            WHERE
                                S.IS_DELETED != 1 AND S.CLASS_ID =?CLASS_ID AND S.STAFF_ID=?STAFF_ID
                                    AND S.COURSE_ID =?COURSE_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchStudentsByClassandCourse:
                    {
                        query = @"SELECT 
                                CLS.STUDENT_ID,
                                CONCAT(IF(SP.FIRST_NAME IS NULL,
                                            '',
                                            SP.FIRST_NAME),
                                        ' ',
                                        IF(SP.LAST_NAME IS NULL,
                                            '',
                                            SP.LAST_NAME)) NAME,
                                SP.REGISTER_NO,
                                SP.ROLL_NO,
                                COURSE_ROOT_ID,COURSE_TITLE,COURSE_CODE
                            FROM
                                STU_CLASS AS CLS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                    INNER JOIN
                                CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                    AND CC.COURSE_ID =?COURSE_ID
                                    AND CC.IS_DELETED != 1
                                    INNER JOIN
                                cp_course_root AS CCR ON CCR.COURSE_ROOT_ID = CC.COURSE_ID
                            WHERE
		                            CLS.CLASS_ID =?CLASS_ID
                                    AND CLS.ACADEMIC_YEAR =?AC_YEAR
                                    AND CLS.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1 
                            UNION SELECT 
                                CLS.STUDENT_ID,
                                CONCAT(IF(SP.FIRST_NAME IS NULL,
                                            '',
                                            SP.FIRST_NAME),
                                        ' ',
                                        IF(SP.LAST_NAME IS NULL,
                                            '',
                                            SP.LAST_NAME)) NAME,
                                SP.REGISTER_NO,
                                SP.ROLL_NO,
                                COURSE_ROOT_ID,COURSE_TITLE,COURSE_CODE
                            FROM
                                STU_COURSE_INFO AS CLS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                    INNER JOIN
                                cp_course_root AS CCR ON CCR.COURSE_ROOT_ID = CLS.COURSE_ID
                            WHERE
		                            CLS.S_CLASS_ID = ?CLASS_ID
                                    AND CLS.ACADEMIC_YEAR =?AC_YEAR
                                    AND COURSE_ID =?COURSE_ID
                                    AND CLS.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1;";
                        break;
                    }
                case LMSSQLCommands.FetchStudentsByClassandCourseForSubmission:
                    {
                        query = @"SELECT 
                                 CLS.STUDENT_ID,
                                 CONCAT(IF(SP.FIRST_NAME IS NULL,
                                             '',
                                             SP.FIRST_NAME),
                                         ' ',
                                         IF(SP.LAST_NAME IS NULL,
                                             '',
                                             SP.LAST_NAME)) NAME,
                                 SP.REGISTER_NO,
                                 SP.ROLL_NO,
                                 COURSE_ROOT_ID,
                                 COURSE_TITLE,
                                 COURSE_CODE,LAS.ASSIGNMENT_SETTING_ID,
                                 LSAS.FILE_PATH,LSAS.TOTAL_MARK,LSAS.SEC_MARK,
                                 LAS.ASSIGNMENT_NAME,LSAS.STU_ASSIGNMENT_ID
                             FROM
                                 STU_CLASS AS CLS
                                     INNER JOIN
                                 STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                     INNER JOIN
                                 CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                     AND CC.COURSE_ID =?COURSE_ID
                                     AND CC.IS_DELETED != 1
                                     INNER JOIN
                                 CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CC.COURSE_ID
                                     LEFT JOIN
                                 LMS_ASSIGNMENT_SETTING AS LAS ON LAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LAS.CLASS_ID = CC.CLASS_ID
                                     LEFT JOIN
                                 LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                     AND LSAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LSAS.STUDENT_ID = CLS.STUDENT_ID
                             WHERE
                                 CLS.CLASS_ID =?CLASS_ID
                                     AND CLS.ACADEMIC_YEAR =?AC_YEAR
                                     AND CLS.IS_DELETED != 1
                                     AND SP.IS_LEFT != 1 
                             UNION SELECT 
                                 CLS.STUDENT_ID,
                                 CONCAT(IF(SP.FIRST_NAME IS NULL,
                                             '',
                                             SP.FIRST_NAME),
                                         ' ',
                                         IF(SP.LAST_NAME IS NULL,
                                             '',
                                             SP.LAST_NAME)) NAME,
                                 SP.REGISTER_NO,
                                 SP.ROLL_NO,
                                 COURSE_ROOT_ID,
                                 COURSE_TITLE,
                                 COURSE_CODE,LAS.ASSIGNMENT_SETTING_ID,
                                 LSAS.FILE_PATH,LSAS.TOTAL_MARK,LSAS.SEC_MARK,
                                 LAS.ASSIGNMENT_NAME,LSAS.STU_ASSIGNMENT_ID
                             FROM
                                 STU_COURSE_INFO AS CLS
                                     INNER JOIN
                                 STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                     INNER JOIN
                                 CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CLS.COURSE_ID
                                     LEFT JOIN
                                 LMS_ASSIGNMENT_SETTING AS LAS ON LAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LAS.CLASS_ID = CLS.CLASS_ID
                                     LEFT JOIN
                                 LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID=LAS.ASSIGNMENT_SETTING_ID
                                     AND LSAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LSAS.STUDENT_ID = CLS.STUDENT_ID
                             WHERE
                                 CLS.S_CLASS_ID =?CLASS_ID
                                     AND CLS.ACADEMIC_YEAR =?AC_YEAR
                                     AND CLS.COURSE_ID =?COURSE_ID
                                     AND CLS.IS_DELETED != 1
                                     AND SP.IS_LEFT != 1;";
                        break;
                    }
                case LMSSQLCommands.FetchStudentsByClassCourseandSettingIdForSubmission:
                    {
                        query = @"SELECT 
                                 CLS.STUDENT_ID,
                                 CONCAT(IF(SP.FIRST_NAME IS NULL,
                                             '',
                                             SP.FIRST_NAME),
                                         ' ',
                                         IF(SP.LAST_NAME IS NULL,
                                             '',
                                             SP.LAST_NAME)) NAME,
                                 SP.REGISTER_NO,
                                 SP.ROLL_NO,
                                 COURSE_ROOT_ID,
                                 COURSE_TITLE,
                                 COURSE_CODE,LAS.ASSIGNMENT_SETTING_ID,
                                 LSAS.FILE_PATH,
                                 LAS.TOTAL_MARK,
                                 LSAS.SEC_MARK,
                                 LAS.ASSIGNMENT_NAME,
                                 LSAS.STU_ASSIGNMENT_ID
                             FROM
                                 STU_CLASS AS CLS
                                     INNER JOIN
                                 STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                     INNER JOIN
                                 CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                     AND CC.COURSE_ID =?COURSE_ID
                                     AND CC.IS_DELETED != 1
                                     INNER JOIN
                                 CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CC.COURSE_ID
                                     LEFT JOIN
                                 LMS_ASSIGNMENT_SETTING AS LAS ON LAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LAS.CLASS_ID = CC.CLASS_ID and LAS.ASSIGNMENT_SETTING_ID = ?ASSIGNMENT_SETTING_ID
                                     LEFT JOIN
                                 LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                     AND LSAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LSAS.STUDENT_ID = CLS.STUDENT_ID
                             WHERE
                                 CLS.CLASS_ID =?CLASS_ID
                                     AND CLS.ACADEMIC_YEAR =?AC_YEAR
                                     AND CLS.IS_DELETED != 1
                                     AND SP.IS_LEFT != 1 
                             UNION SELECT 
                                 CLS.STUDENT_ID,
                                 CONCAT(IF(SP.FIRST_NAME IS NULL,
                                             '',
                                             SP.FIRST_NAME),
                                         ' ',
                                         IF(SP.LAST_NAME IS NULL,
                                             '',
                                             SP.LAST_NAME)) NAME,
                                 SP.REGISTER_NO,
                                 SP.ROLL_NO,
                                 COURSE_ROOT_ID,
                                 COURSE_TITLE,
                                 COURSE_CODE,LAS.ASSIGNMENT_SETTING_ID,
                                 LSAS.FILE_PATH,LSAS.TOTAL_MARK,LSAS.SEC_MARK,
                                 LAS.ASSIGNMENT_NAME,LSAS.STU_ASSIGNMENT_ID
                             FROM
                                 STU_COURSE_INFO AS CLS
                                     INNER JOIN
                                 STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                     INNER JOIN
                                 CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CLS.COURSE_ID
                                     LEFT JOIN
                                 LMS_ASSIGNMENT_SETTING AS LAS ON LAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LAS.CLASS_ID = CLS.CLASS_ID and LAS.ASSIGNMENT_SETTING_ID = ?ASSIGNMENT_SETTING_ID
                                     LEFT JOIN
                                 LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID=LAS.ASSIGNMENT_SETTING_ID
                                     AND LSAS.COURSE_ID = CCR.COURSE_ROOT_ID
                                     AND LSAS.STUDENT_ID = CLS.STUDENT_ID
                             WHERE
                                 CLS.S_CLASS_ID =?CLASS_ID
                                     AND CLS.ACADEMIC_YEAR =?AC_YEAR
                                     AND CLS.COURSE_ID =?COURSE_ID
                                     AND CLS.IS_DELETED != 1
                                     AND SP.IS_LEFT != 1;";
                        break;
                    }
                case LMSSQLCommands.UpdateAssignmentSetting:
                    {
                        query = @"UPDATE LMS_ASSIGNMENT_SETTING 
                                    SET 
                                        ASSIGNMENT_NAME =?ASSIGNMENT_NAME,
                                        STAFF_ID =?STAFF_ID,
                                        COURSE_ID =?COURSE_ID,
                                        CLASS_ID =?CLASS_ID,
                                        TOTAL_MARK =?TOTAL_MARK,
                                        START_DATE =?START_DATE,
                                        END_DATE =?END_DATE,
                                        DESCRIPTION =?DESCRIPTION,
                                        IS_ACTIVE =?IS_ACTIVE,
                                        ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        STUDENT_ID =?STUDENT_ID
                                    WHERE
                                        ASSIGNMENT_SETTING_ID =?ASSIGNMENT_SETTING_ID;";
                        break;
                    }
                case LMSSQLCommands.SaveAssignmentSetting:
                    {
                        query = @"INSERT INTO LMS_ASSIGNMENT_SETTING
                                (ASSIGNMENT_NAME, 
                                STAFF_ID, 
                                COURSE_ID, 
                                CLASS_ID,
                                TOTAL_MARK, 
                                START_DATE, 
                                END_DATE,
                                START_TIME, 
                                END_TIME, 
                                DESCRIPTION,
                                STUDENT_ID,
                                IS_ACTIVE, 
                                ACADEMIC_YEAR)
                                 VALUES(?ASSIGNMENT_NAME, 
                                ?STAFF_ID, 
                                ?COURSE_ID, 
                                ?CLASS_ID,
                                ?TOTAL_MARK, 
                                ?START_DATE, 
                                ?END_DATE,
                                ?START_TIME, 
                                ?END_TIME, 
                                ?DESCRIPTION, 
                                ?STUDENT_ID, 
                                ?IS_ACTIVE, 
                                ?AC_YEAR);";
                        break;
                    }
                case LMSSQLCommands.FetchAssignmentSettingById:
                    {
                        query = @"SELECT 
                                       LMS.ASSIGNMENT_SETTING_ID,
                                       LMS.ASSIGNMENT_NAME,
                                       LMS.DESCRIPTION,
                                       LMS.STUDENT_ID,
                                       LMS.COURSE_ID,
                                       LMS.CLASS_ID,
                                       LMS.TOTAL_MARK,
                                       LMS.LINK_URL,
                                       LMS.FILE_PATH,
                                       DATE_FORMAT(LMS.START_DATE, '%d/%m/%Y') AS START_DATE,
                                       DATE_FORMAT(LMS.END_DATE, '%d/%m/%Y') AS END_DATE,
                                       LMS.START_TIME,
                                       LMS.END_TIME,
                                       LMS.ACADEMIC_YEAR,
                                       LMS.ENTRY_DATE,
                                       CCR.COURSE_TITLE,
                                       CC.CLASS_NAME,
                                       STF.FIRST_NAME
                                    FROM
                                        LMS_ASSIGNMENT_SETTING AS LMS
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = LMS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LMS.COURSE_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = LMS.STAFF_ID
                                    WHERE
                                        LMS.IS_DELETED != 1
                                        AND LMS.ASSIGNMENT_SETTING_ID = ?ASSIGNMENT_SETTING_ID
                                    GROUP BY LMS.ASSIGNMENT_SETTING_ID";
                        break;
                    }

                case LMSSQLCommands.ListAssignmentSetting:
                    {
                        query = @"SELECT 
                                 S.ASSIGNMENT_SETTING_ID,
                                S.ASSIGNMENT_NAME,
                                S.DESCRIPTION,
                                S.STAFF_ID,
                                SP.FIRST_NAME,
                                SPI.FIRST_NAME AS STUDENT_NAME,
                                S.COURSE_ID,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                CC.CLASS_NAME,
                                S.CLASS_ID,
                                S.TOTAL_MARK,
                                DATE_FORMAT(S.START_DATE, '%d/%m/%Y') AS START_DATE,
                                DATE_FORMAT(S.END_DATE, '%d/%m/%Y') AS END_DATE,
                                S.START_TIME,
                                S.END_TIME,
                                IF(DATE_FORMAT(S.END_DATE, '%d/%m/%Y') >= DATE_FORMAT(NOW(), '%d/%m/%Y'),
                                    1,
                                    0) AS IS_ACTIVE
                            FROM
                                LMS_ASSIGNMENT_SETTING AS S
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = S.CLASS_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = S.COURSE_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = S.STAFF_ID
                                    INNER JOIN 
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = S.STUDENT_ID  
                            WHERE
                                S.IS_DELETED != 1 AND S.STAFF_ID =?STAFF_ID AND S.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case LMSSQLCommands.DeleteAssignmentSetting:
                    {
                        query = @"UPDATE LMS_ASSIGNMENT_SETTING 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        ASSIGNMENT_SETTING_ID =?ASSIGNMENT_SETTING_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchClassworkFiles:
                    {
                        query = @"	SELECT 
									SP.FIRST_NAME,
									CCR.COURSE_CODE,
									CCR.COURSE_TITLE,
									A.COURSE_ID,
									A.COURSEWISE_ATTACHMENT_ID,
									A.STAFF_ID,
									A.UNIT,
									A.CLASS_ID,
									A.TITLE,
									LSC.NOTES_TITLE,
									LSC.NOTES_ID,
									A.SUBJECT,
									A.DESCRIPTION,
									DATE_FORMAT(ENTRY_DATE, '%d-%m-%Y') AS ENTRY_DATE,
									F.FILE_PATH,
									A.QUESTION_PAPER_ID,
									A.QUESTION_PAPER_TYPE_ID,
									PAPER_TITLE,
									ATT_DATE,
									TYPE,
									LCE.TOPIC AS EXCEL_TOPIC,
									LCE.UNIT AS EXCEL_UNIT
								FROM
								  LMS_COURSEWISE_ATTACHMENTS AS A 
									   INNER JOIN 
									STU_CLASS AS SC ON SC.CLASS_ID = A.CLASS_ID   
									   INNER JOIN
									CP_CLASSES AS CC ON SC.CLASS_ID = CC.CLASS_ID
										INNER JOIN
									STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = A.STAFF_ID
										INNER JOIN
									CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = A.COURSE_ID
										INNER JOIN
									SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CCR.SEMESTER_ID  
										INNER JOIN 
									SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = SS.SEMESTER_TYPE
										AND ST.IS_ACTIVE = 1
										LEFT JOIN
									SUP_LMS_UNIT AS U ON U.LMS_UNIT_ID = A.UNIT
										LEFT JOIN
									LMS_COURSEWISE_FILES AS F ON F.COURSEWISE_ATTACHMENT_ID = A.COURSEWISE_ATTACHMENT_ID
									   LEFT JOIN 
								   LMS_SUP_CLASS_NOTES AS LSC ON LSC.NOTES_ID = A.NOTES_ID
									  LEFT JOIN 
								   LMS_SUP_QUESTION_PAPER AS LSQ ON LSQ.QUESTION_PAPER_ID = A.QUESTION_PAPER_ID
									   LEFT JOIN 
								   LMS_SUP_QUESTION_PAPER_TYPE AS LSQT ON LSQT.QUESTION_PAPER_TYPE_ID = A.QUESTION_PAPER_TYPE_ID
									   LEFT JOIN 
									LMS_COURSE_ATTACH_EXCEL AS LCE ON LCE.COURSEWISE_ATTACHMENT_ID = A.COURSEWISE_ATTACHMENT_ID AND ATT_DATE = CURDATE()    
								WHERE
										SC.IS_DELETED != 1
										AND A.IS_DELETED != 1
										AND SC.ACADEMIC_YEAR =?AC_YEAR
										AND SC.STUDENT_ID =?STUDENT_ID
								;
;";
                        break;
                    }
                case LMSSQLCommands.FetchAnnouncement:
                    {
                        query = @"SELECT 
                                A.ANNOUNCEMENT_ID,
                                SP.FIRST_NAME,
                                CCR.COURSE_CODE,
                                A.STAFF_ID,
                                DATE_FORMAT(A.DATE, '%d/%m/%Y') AS `DATE`,
                                A.MESSAGE,
                                A.FILE_PATH
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_CLASSES AS CC ON SC.CLASS_ID = CC.CLASS_ID
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                LMS_COURSEWISE_ANNOUNCEMENT AS A ON A.COURSE_ID = CCS.COURSE_ID
                                    AND A.CLASS_ID = CCS.CLASS_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = A.STAFF_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                    AND ST.IS_ACTIVE = 1
                            WHERE
                                SC.IS_DELETED != 1
                                    AND CCS.IS_DELETED != 1
                                    AND A.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
                                    AND SC.STUDENT_ID =?STUDENT_ID
                            GROUP BY A.ANNOUNCEMENT_ID;";
                        break;
                    }

                case LMSSQLCommands.UpdateLmsannouncement:
                    {


                        query = @"UPDATE LMS_COURSEWISE_ANNOUNCEMENT 
                                  SET MESSAGE = ?MESSAGE,
                                      FILE_PATH = ?FILE_PATH
                                  WHERE ANNOUNCEMENT_ID = ?ANNOUNCEMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchAnnouncementById:
                    {
                        query = @"SELECT 
                                A.ANNOUNCEMENT_ID,
                                SP.FIRST_NAME,
                                CCR.COURSE_CODE,
                                A.STAFF_ID,
                                DATE_FORMAT(A.DATE, '%d/%m/%Y') AS `DATE`,
                                A.MESSAGE,
                                A.FILE_PATH
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                LMS_COURSEWISE_ANNOUNCEMENT AS A ON A.COURSE_ID = CCS.COURSE_ID
                                    AND A.CLASS_ID = CCS.CLASS_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = A.STAFF_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                    AND ST.IS_ACTIVE = 1
                            WHERE
                                    SC.IS_DELETED != 1
                                    AND CCS.IS_DELETED != 1
                                    AND A.ANNOUNCEMENT_ID=?ANNOUNCEMENT_ID
                            GROUP BY A.ANNOUNCEMENT_ID;";
                        break;
                    }
                //case LMSSQLCommands.DeleteLmsAnnouncementbyid:
                //    {
                //        query = @"UPDATE LMS_COURSEWISE_ANNOUNCEMENT SET IS_DELETED = 1 WHERE ANNOUNCEMENT_ID = ?ANNOUNCEMENT_ID;";
                //        break;
                //    }
                case LMSSQLCommands.Fetchlmscoursewiseatt:
                    {
                        query = @"SELECT 
                                  LCA.COURSEWISE_ATTACHMENT_ID,
                                  SPI.FIRST_NAME as STAFF_NAME,
                                  CC.CLASS_NAME ,
								  CCR.COURSE_TITLE,
                                  SLU.UNIT,
                                  LCA.TITLE,
                                  LCA.SUBJECT,
                                  LCA.DESCRIPTION,
                                  LCA.ACADEMIC_YEAR,LCF.FILE_PATH
                                  FROM
                                      LMS_COURSEWISE_ATTACHMENTS AS LCA
                                      LEFT JOIN
                                      STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID=LCA.STAFF_ID
                                      LEFT JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID=LCA.CLASS_ID
                                      LEFT JOIN
                                      SUP_LMS_UNIT AS SLU ON SLU.LMS_UNIT_ID=LCA.UNIT
                                      LEFT JOIN
                                      CP_COURSE_ROOT as CCR ON CCR.COURSE_ROOT_ID=LCA.COURSE_ID
                                      INNER JOIN 
                                      LMS_COURSEWISE_FILES AS LCF 
                                  ON LCF.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
                                          AND LCF.IS_DELETED != 1
                                  WHERE
                                      LCA.IS_DELETED != 1 AND LCA.ACADEMIC_YEAR=?AC_YEAR
                                    -- R AND LCA.STAFF_ID=?STAFF_ID ;";
                                       
                        break;
                    }
                case LMSSQLCommands.SaveCoursewiseatt:
                    {
                        query = @"INSERT INTO LMS_COURSEWISE_ATTACHMENTS
                                        (STAFF_ID, 
                                         CLASS_ID,
                                        COURSE_ID, 
                                        UNIT, TITLE, 
                                        SUBJECT, 
                                        DESCRIPTION, 
                                        ACADEMIC_YEAR,
                                        NOTES_ID,
                                        QUESTION_PAPER_TYPE_ID,
                                        QUESTION_PAPER_ID)
                                        VALUES
                                        (?STAFF_ID,
                                        ?CLASS_ID,
                                        ?COURSE_ID,
                                        ?UNIT,
                                        ?TITLE,
                                        ?SUBJECT,
                                        ?DESCRIPTION,
                                        ?ACADEMIC_YEAR,
                                        ?NOTES_ID,
                                        ?QUESTION_PAPER_TYPE_ID,
                                        ?QUESTION_PAPER_ID);";
                        break;
                    }
                case LMSSQLCommands.UpdateCoursewiseatt:
                    {
                        query = @"UPDATE 
                                   LMS_COURSEWISE_ATTACHMENTS
                                  SET 
                                   STAFF_ID=?STAFF_ID,
                                   CLASS_ID=?CLASS_ID,
                                   COURSE_ID=?COURSE_ID,
                                   UNIT=?UNIT,
                                   TITLE=?TITLE,
                                   SUBJECT=?SUBJECT,
                                   DESCRIPTION=?DESCRIPTION,
                                   ACADEMIC_YEAR=?ACADEMIC_YEAR
                                  WHERE 
                                   COURSEWISE_ATTACHMENT_ID=?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.DeleteCoursewiseatt:
                    {
                        query = @"UPDATE LMS_COURSEWISE_ATTACHMENTS
SET IS_DELETED=?IS_DELETED
WHERE COURSEWISE_ATTACHMENT_ID=?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.Savecoursewisefiles:
                    {
                        query = @"INSERT INTO LMS_COURSEWISE_FILES(COURSEWISE_ATTACHMENT_ID,FILE_PATH)
VALUES (?COURSEWISE_ATTACHMENT_ID,?FILE_PATH);";
                        break;
                    }

                case LMSSQLCommands.updateCourseWiseFile:
                    {
                        query = @"UPDATE LMS_COURSE_ATTACH_EXCEL
SET RESOURCE = ?FILE_PATH
WHERE COURSEWISE_ATTACHMENT_ID = ?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }

                case LMSSQLCommands.DeleteCoursewisefiles:
                    {
                        query = @"UPDATE LMS_COURSEWISE_FILES
                                SET IS_DELETED=?IS_DELETED
                                WHERE COURSEWISE_ATTACHMENT_ID=?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.Fetchlmsunit:
                    {
                        query = @"SELECT LMS_UNIT_ID, UNIT FROM SUP_LMS_UNIT WHERE IS_DELETED!= 1;";
                        break;
                    }
                case LMSSQLCommands.Insertassignmentsettingstudent:
                    {
                        query = @" INSERT INTO  LMS_STUDENT_GROUP_INFO  (STUDENT_ID,CLASS_ID,COURSE_ID,SETTING_ID,GROUP_ID,ACADEMIC_YEAR,DESCRIPTION) 
                                       VALUES(?STUDENT_ID,?CLASS_ID,?COURSE_ID,?SETTING_ID,?GROUP_ID,?ACADEMIC_YEAR,?DESCRIPTION,?ASSIGNMENT_INFO);";
                        break;
                    }
                case LMSSQLCommands.Updateassignmentsettingstudent:
                    {
                        query = @" UPDATE LMS_STUDENT_GROUP_INFO SET GROUP_ID =?GROUP_ID WHERE LMS_GROUP_ID =?LMS_GROUP_ID;";
                        break;
                    }
                case LMSSQLCommands.DeleteAssignmentGroupInfo:
                    {
                        query = @" UPDATE LMS_STUDENT_GROUP_INFO 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        LMS_GROUP_ID =LMS_GROUP_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchAssignmentGroupstudentGroupByid:
                    {
                        query = @" SELECT 
                                       LSA.STUDENT_ID,
                                       GROUP_ID,
                                       SP.FIRST_NAME,
                                       COURSE_TITLE,
                                       ASSIGNMENT_NAME,
                                       CLASS_NAME,
                                       LG.GROUP_NAME,
                                       LMS_GROUP_ID,
                                       LSA.CLASS_ID,
                                       LSA.SETTING_ID,
                                       LSA.COURSE_ID,
                                       LSA.DESCRIPTION AS ASSIGNMENT_INFO
                                   FROM
                                       LMS_STUDENT_GROUP_INFO AS LSA
                                           INNER JOIN
                                       STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = LSA.STUDENT_ID  
                                       AND SP.IS_DELETED !=1
                                       AND SP.IS_LEFT !=1
                                           INNER JOIN
                                       CP_CLASSES AS CC ON CC.CLASS_ID = LSA.CLASS_ID AND CC.IS_DELETED !=1
                                           INNER JOIN
                                       CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LSA.COURSE_ID AND CCR.IS_DELETED !=1
                                           INNER JOIN
                                   	LMS_CLASS_GROUP AS LG on LG.CLS_GROUP_ID =LSA.GROUP_ID
                                          INNER JOIN
                                   	LMS_ASSIGNMENT_SETTING AS LSS on LSS.ASSIGNMENT_SETTING_ID =LSA.SETTING_ID AND LSS.IS_DELETED !=1
                                       where
                                       LMS_GROUP_ID =?LMS_GROUP_ID AND LSA.IS_DELETED!=1;
                                   ";
                        break;
                    }
                case LMSSQLCommands.FetchlmscoursewiseAttachment:
                    {
                        query = @"SELECT 
                                        LCA.COURSEWISE_ATTACHMENT_ID,
                                        SPI.FIRST_NAME AS STAFF_NAME,
                                        CC.CLASS_NAME,
                                        CCR.COURSE_TITLE,
                                        SLU.UNIT,
                                        LCA.TITLE,
                                        LCA.SUBJECT,
                                        LCA.DESCRIPTION,
                                        LCA.ACADEMIC_YEAR,
                                        LCF.FILE_PATH
                                    FROM
                                        LMS_COURSEWISE_ATTACHMENTS AS LCA
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = LCA.STAFF_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = LCA.CLASS_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS ccs ON ccs.STAFF_ID = spi.STAFF_ID
                                            AND ccs.CLASS_ID = lca.CLASS_ID
                                            AND ccs.COURSE_ID = lca.COURSE_ID
                                            AND ccs.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_LMS_UNIT AS SLU ON SLU.LMS_UNIT_ID = LCA.UNIT
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LCA.COURSE_ID
                                            INNER JOIN
                                        LMS_COURSEWISE_FILES AS LCF ON LCF.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
                                            AND LCF.IS_DELETED != 1
                                    WHERE
                                        LCA.IS_DELETED != 1
                                            AND LCA.ACADEMIC_YEAR = ?AC_YEAR
                                            AND LCA.STAFF_ID IN (?STAFF_ID)
                                            -- AND LCA.COURSE_ID IN (?COURSE_ID)
                                            -- AND LCA.CLASS_ID IN (?CLASS_ID)
                                             AND CCS.SEMESTER_TYPE_ID= ?SEMESTER_TYPE_ID ORDER BY SPI.STAFF_ID,CCS.CLASS_ID,CCS.COURSE_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchlmscoursewiseAttachmentForHOD:
                    {
                        query = @"SELECT 
                                        LCA.COURSEWISE_ATTACHMENT_ID,
                                        SPI.FIRST_NAME AS STAFF_NAME,
                                        CC.CLASS_NAME,
                                        CCR.COURSE_TITLE,
                                        SLU.UNIT,
                                        LCA.TITLE,
                                        LCA.SUBJECT,
                                        LCA.DESCRIPTION,
                                        LCA.ACADEMIC_YEAR,
                                        LCF.FILE_PATH
                                    FROM
                                        LMS_COURSEWISE_ATTACHMENTS AS LCA
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = LCA.STAFF_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = LCA.CLASS_ID
                                            LEFT JOIN
                                        SUP_LMS_UNIT AS SLU ON SLU.LMS_UNIT_ID = LCA.UNIT
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LCA.COURSE_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS ccs ON ccs.STAFF_ID = spi.STAFF_ID
                                            AND ccs.CLASS_ID = lca.CLASS_ID
                                            AND ccs.COURSE_ID = lca.COURSE_ID
                                            AND ccs.IS_DELETED != 1
                                            INNER JOIN
                                        LMS_COURSEWISE_FILES AS LCF ON LCF.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
                                            AND LCF.IS_DELETED != 1
                                    WHERE
                                        LCA.IS_DELETED != 1
                                            AND LCA.ACADEMIC_YEAR = ?AC_YEAR
                                            AND LCA.STAFF_ID IN (?STAFF_ID) 
                                            AND CCS.SEMESTER_TYPE_ID= ?SEMESTER_TYPE_ID ORDER BY SPI.STAFF_ID,CCS.CLASS_ID,CCS.COURSE_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchClasscourseStaffByClassId:
                    {
                        query = @"SELECT 
                                CC.CLASS_NAME,
                                SP.FIRST_NAME,
                                SP.STAFF_CODE,
                                CCS.COURSE_ID,
                                CCS.STAFF_ID
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_CLASSES AS CC ON SC.CLASS_ID = CC.CLASS_ID
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                    AND ST.IS_ACTIVE = 1
                            WHERE
		                            SC.IS_DELETED != 1
                                    AND CCS.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
                                    AND SC.STUDENT_ID =?STUDENT_ID
                            GROUP BY CCS.CLASS_ID , CCS.STAFF_ID , CCS.COURSE_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchLMSAnnouncement:
                    {
                        query = @" SELECT 
                                        ANNOUNCEMENT_ID,
                                        MESSAGE,
                                        DATE_FORMAT(DATE, '%d/%m/%Y  %h:%i %p') AS DATE,
                                        CLASS_NAME
                                    FROM
                                        LMS_COURSEWISE_ANNOUNCEMENT AS lca
                                            left JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = lca.class_id
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND ANNOUNCEMENT_TYPE = ?ANNOUNCEMENT_TYPE and lca.IS_DELETED !=1 order by ANNOUNCEMENT_ID desc;";
                        break;
                    }
                case LMSSQLCommands.Fetchassignmentsettingbystaffid:
                    {
                        query = @" SELECT ASSIGNMENT_SETTING_ID, ASSIGNMENT_NAME
                                    FROM lms_assignment_setting
                                    WHERE  STAFF_ID = ?STAFF_ID AND IS_ACTIVE =1
                                      AND CURDATE() BETWEEN START_DATE AND END_DATE;";
                        break;
                    }
                case LMSSQLCommands.Fetchassignmentsettingbycourseid:
                    {
                        query = @" SELECT 
                                      ASSIGNMENT_SETTING_ID,
                                      ASSIGNMENT_NAME,
                                      CLASS_ID,
                                      COURSE_ID,
                                      TOTAL_MARK,
                                      DESCRIPTION
                                    FROM 
                                      lms_assignment_setting
                                    WHERE CLASS_ID = ?CLASS_ID
                                      AND COURSE_ID = ?COURSE_ID
                                      AND STAFF_ID = ?STAFF_ID
                                     AND IS_DELETED !=1
                                      AND CURDATE() BETWEEN START_DATE AND END_DATE;";
                        break;
                    }
                case LMSSQLCommands.fetchclassgroup:
                    {
                        query = @"SELECT CLS_GROUP_ID ,GROUP_NAME FROM LMS_CLASS_GROUP WHERE IS_DELETED!=1;";
                        break;
                    }
                case LMSSQLCommands.FetchClasslistfromclasscoursestaff:
                    {
                        query = @"SELECT 
                              CCS.CLASS_ID, CC.CLASS_NAME
                          FROM
                              CP_CLASS_COURSE_STAFF AS CCS
                                  LEFT JOIN
                              CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                  INNER JOIN
                              STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = CCS.STAFF_ID
                                  AND SPI.IS_DELETED != 1
                                  AND SPI.IS_LEFT != 1
                                  INNER JOIN
                              SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID 
                                  AND SST.IS_ACTIVE =1
                          WHERE
                              CCS.IS_DELETED != 1
                                  AND CCS.ACADEMIC_YEAR =?AC_YEAR
                                  AND CCS.STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchStudentByclassandcourseId:
                    {
                        query = @"SELECT 
                                      SC.STUDENT_ID,
                                      SP.FIRST_NAME,
                                      CC.CLASS_NAME,
                                      CCR.COURSE_TITLE,
                                      SC.CLASS_ID,
                                      SCI.COURSE_ID
                                  FROM
                                      STU_CLASS AS SC
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                          INNER JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                          INNER JOIN
                                      STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID AND SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                          INNER JOIN
                                      CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SCI.COURSE_ID
                                  WHERE
                                      SP.IS_DELETED != 1 AND SC.CLASS_ID = ?CLASS_ID
                                          AND SCI.COURSE_ID = ?COURSE_ID and SC.STUDENT_ID NOT IN( SELECT LSGI.STUDENT_ID 
                                         FROM LMS_STUDENT_GROUP_INFO AS LSGI
                                         WHERE LSGI.COURSE_ID = ?COURSE_ID
                                           AND LSGI.CLASS_ID = ?CLASS_ID
                                           AND LSGI.IS_DELETED !=1
                                           AND LSGI.SETTING_ID = ?SETTING_ID);";
                        break;
                    }
                case LMSSQLCommands.Fetchassignmentstudentgroupinfo:
                    {
                        query = @"SELECT 
                                      LSA.STUDENT_ID,
                                      SP.FIRST_NAME,
                                      LMS_GROUP_ID,
                                      COURSE_TITLE,
                                      CLASS_NAME,
                                      LG.GROUP_NAME,
                                      GROUP_ID,
                                      LSA.CLASS_ID,
                                      LSA.SETTING_ID,
                                      LSA.COURSE_ID,
                                      LSA.DESCRIPTION AS ASSIGNMENT_INFO
                                  FROM
                                      LMS_STUDENT_GROUP_INFO AS LSA
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = LSA.STUDENT_ID AND SP.IS_DELETED !=1
                                       AND SP.IS_LEFT !=1
                                          INNER JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = LSA.CLASS_ID
                                          INNER JOIN 
                                      CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LSA.COURSE_ID AND CCR.IS_DELETED !=1
                                          INNER JOIN
                                  	LMS_CLASS_GROUP AS LG on LG.CLS_GROUP_ID =LSA.GROUP_ID
                                      where
                                      LSA.CLASS_ID =?CLASS_ID AND LSA.IS_DELETED !=1 AND LSA.COURSE_ID =?COURSE_ID AND SETTING_ID =?SETTING_ID order by GROUP_ID;";
                        break;
                    }
                case LMSSQLCommands.SaveLmsannouncements:
                    {
                        query = @"INSERT INTO 
                                LMS_COURSEWISE_ANNOUNCEMENT
                                (
                                CLASS_ID,
                                COURSE_ID,
                                STAFF_ID,
                                MESSAGE,
                                ANNOUNCEMENT_TYPE,
                                FILE_PATH)
                                VALUES(
                                ?CLASS_ID,
                                ?COURSE_ID,
                                ?STAFF_ID,
                                ?MESSAGE,
                                ?ANNOUNCEMENT_TYPE,
                                ?FILE_PATH);";
                        break;
                    }
                case LMSSQLCommands.FetchCourseByClassIdLMS:
                    {
                        query = @"SELECT 
                                    CR.COURSE_ROOT_ID,
                                    CONCAT(IFNULL(UPPER(CR.COURSE_TITLE), ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, ''),
                                            ' - ',
                                            CR.SEMESTER_ID) AS 'COURSE_TITLE'
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CC
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                        INNER JOIN
									SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CC.SEMESTER_TYPE_ID
										AND ST.IS_ACTIVE=1
                                WHERE
                                    CC.CLASS_ID = ?CLASS_ID
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CC.STAFF_ID = ?STAFF_ID
                                        AND CC.IS_DELETED != 1
                                GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchActivelmsassignmentsByStudentId:
                    {
                        query = @"SELECT 
                                     LAS.ASSIGNMENT_SETTING_ID,LAS.ASSIGNMENT_NAME, 
                                     LAS.STAFF_ID,SPI.FIRST_NAME as STAFF_NAME,
                                     LAS.COURSE_ID,CCR.COURSE_TITLE,
                                     TRIM(TRAILING '.' FROM CCR.COURSE_CODE) AS COURSE_CODE,
                                     LAS.CLASS_ID,CC.CLASS_NAME,
                                     LAS.TOTAL_MARK,
                                     date_format(LAS.START_DATE,'%d/%m/%Y') as START_DATE,
                                     date_format(LAS.END_DATE,'%d/%m/%Y') as END_DATE,
                                     LAS.START_TIME,LAS.END_TIME, 
                                     LAS.ACADEMIC_YEAR,
                                    LAS.DESCRIPTION,
                                     LAS.FILE_PATH AS ATTACHMENTS,
                                     LSAS.SEC_MARK,LSAS.FILE_PATH,if(LSAS.FILE_PATH is null,'0','1') as STATUS
                                 FROM 
                              
									LMS_ASSIGNMENT_SETTING AS LAS
                                        LEFT JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = LAS.STAFF_ID
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LAS.COURSE_ID
                                        LEFT JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = LAS.CLASS_ID
                                        LEFT JOIN
                                    LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                WHERE 
                                    LAS.ACADEMIC_YEAR=?AC_YEAR
                                    AND LAS.IS_DELETED!=1
                                    AND LAS.CLASS_ID in (?CLASS_ID)
                                    AND FIND_IN_SET(?STUDENT_ID,LAS.STUDENT_ID)
                                    AND LAS.COURSE_ID in (?COURSE_ID)
                                    AND LAS.START_DATE <= CURDATE()
                                    AND LAS.END_DATE >= CURDATE();";
                        break;
                    }
                case LMSSQLCommands.FetchLMSActivelmsassignmentsByStudentId:
                    {
                        query = @" SELECT 
                                            LAS.ASSIGNMENT_SETTING_ID,
                                            LAS.ASSIGNMENT_NAME,
                                            LAS.STAFF_ID,
                                            SPI.FIRST_NAME AS STAFF_NAME,
                                            LAS.COURSE_ID,
                                            CCR.COURSE_TITLE,
                                            TRIM(TRAILING '.' FROM CCR.COURSE_CODE) AS COURSE_CODE,
                                            LAS.CLASS_ID,
                                            CC.CLASS_NAME,
                                            LAS.TOTAL_MARK,
                                            DATE_FORMAT(LAS.START_DATE, '%d/%m/%Y') AS START_DATE,
                                            DATE_FORMAT(LAS.END_DATE, '%d/%m/%Y') AS END_DATE,
                                            LAS.START_TIME,
                                            LAS.END_TIME,
                                            LAS.ACADEMIC_YEAR,
                                            LAS.DESCRIPTION,
                                            LAS.FILE_PATH AS ATTACHMENTS,
                                            LSAS.SEC_MARK,
                                            LSAS.FILE_PATH,
                                            IF(LSAS.FILE_PATH IS NULL, '0', '1') AS STATUS
                                        FROM
                                            stu_personal_info AS STPI
                                                INNER JOIN
                                            STU_CLASS AS SC ON SC.STUDENT_ID = STPI.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR AND SC.STUDENT_ID = ?STUDENT_ID
                                                INNER JOIN
                                            LMS_ASSIGNMENT_SETTING AS LAS ON LAS.CLASS_ID = SC.CLASS_ID
                                                LEFT JOIN
                                            STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = LAS.STAFF_ID
                                                LEFT JOIN
                                            CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LAS.COURSE_ID
                                                LEFT JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID = LAS.CLASS_ID
                                                LEFT JOIN
                                            LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSAS ON LSAS.ASSIGNMENT_SETTING_ID = LAS.ASSIGNMENT_SETTING_ID
                                                AND LSAS.STUDENT_ID =  SC.STUDENT_ID AND LSAS.IS_DELETED !=1
                                        WHERE
                                            LAS.ACADEMIC_YEAR = ?AC_YEAR
                                                AND LAS.IS_DELETED != 1
                                                AND FIND_IN_SET( ?STUDENT_ID,LAS.STUDENT_ID)
                                                AND LAS.CLASS_ID IN (?CLASS_ID)
                                                AND LAS.START_DATE <= CURDATE()
                                                AND LAS.END_DATE >= CURDATE();";
                        break;
                    }
                case LMSSQLCommands.FetchSettingForStudentQuizEntryByClassAndCourseIds:
                    {
                        query = @"SELECT 
                                        QS.SETTING_ID,
                                        QS.SETTING_NAME,
                                        QS.TIME AS TIME,
                                        C.CLASS_ID,
                                        C.CLASS_NAME,
                                        CCR.COURSE_ROOT_ID AS COURSE_ID,                                       
                                        TRIM(TRAILING '.' FROM CCR.COURSE_CODE) AS COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        SQT.QUIZ_TYPE_NAME
                                    FROM
                                        QUIZ_SETTING AS QS
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLSCRSSTF_ID = QS.QUIZ_STF_ID
		                                    AND CCS.IS_DELETED!=1
                                            AND CCS.ACADEMIC_YEAR=QS.ACADEMIC_YEAR
		                                    INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=CCS.CLASS_ID
		                                    AND C.IS_DELETED!=1
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=CCS.COURSE_ID
		                                    AND CCR.IS_DELETED!=1
		                                    INNER JOIN
	                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=CCS.STAFF_ID
		                                    AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                         LEFT JOIN
                                        SUP_QUIZ_TYPE AS SQT ON SQT.QUIZ_TYPE_ID = QS.QUIZ_TYPE_ID
                                    WHERE
                                            CCS.CLASS_ID IN (?CLASS_ID)
                                            AND CCS.COURSE_ID IN (?COURSE_ID)
                                            AND QS.ACADEMIC_YEAR=?AC_YEAR
                                            AND CURDATE() = QS.START_DATE 
                                            AND QS.IS_DELETED!=1;";
                        break;
                    }

                case LMSSQLCommands.FetchLmsclassNotesListforddl:
                    {
                        query = @"SELECT 
                                      NOTES_ID, NOTES_TITLE, IS_ACTIVE, IS_DELETED
                                  FROM
                                      LMS_SUP_CLASS_NOTES
                                  WHERE
                                      IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }

                case LMSSQLCommands.FetchLmsQuestionPaperListforddl:
                    {
                        query = @"SELECT 
                                      QUESTION_PAPER_ID, PAPER_TITLE, IS_ACTIVE, IS_DELETED
                                  FROM
                                      LMS_SUP_QUESTION_PAPER
                                  WHERE
                                      IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }

                case LMSSQLCommands.FetchLmsQuestionPaperTypeListforddl:
                    {
                        query = @"SELECT 
                                      QUESTION_PAPER_TYPE_ID, TYPE, IS_ACTIVE, IS_DELETED
                                  FROM
                                      LMS_SUP_QUESTION_PAPER_TYPE
                                  WHERE
                                      IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }

                case LMSSQLCommands.SaveExcelUploadforlmsCourseWiseAtt:
                    {
                        query = @"INSERT INTO LMS_COURSE_ATTACH_EXCEL (COURSEWISE_ATTACHMENT_ID,TOPIC,UNIT,HOUR,METHOD,RESOURCE,ASSESSMENT,RUBRIC,ANY_OTHER)
                                  VALUES (?COURSEWISE_ATTACHMENT_ID,?TOPIC,?UNIT,?HOUR,?METHOD,?RESOURCE,?ASSESSMENT,?RUBRIC,?ANY_OTHER);";
                        break;
                    }

                case LMSSQLCommands.SaveExcelUploadforlmsAssignmentsetting:
                    {
                        query = @"INSERT INTO LMS_ASSIGNMENT_SETTING (
                                       STUDENT_ID,
                                       CLASS_ID,
                                       COURSE_ID,
                                       STAFF_ID,
                                       ASSIGNMENT_NAME,
                                       DESCRIPTION,
                                       TOTAL_MARK,
                                       START_DATE,
                                       END_DATE,
                                       ENTRY_DATE,
                                       ACADEMIC_YEAR,
                                       IS_ACTIVE,
                                       IS_DELETED
                                   ) 
                                   SELECT 
                                       STUDENT_ID,
                                       ?CLASS_ID,
                                       ?COURSE_ID,
                                       ?STAFF_ID,
                                       ?ASSIGNMENT_NAME,
                                       ?DESCRIPTION,
                                       ?TOTAL_MARK,
                                       ?START_DATE,
                                       ?END_DATE,
                                       CURRENT_TIMESTAMP AS ENTRY_DATE,
                                       ?ACADEMIC_YEAR,
                                       1 AS IS_ACTIVE,  
                                       0 AS IS_DELETED  
                                   FROM STU_PERSONAL_INFO 
                                   WHERE ROLL_NO = ?ROLL_NO;";
                        break;
                    }

                case LMSSQLCommands.FetchCourseWiseAttachmnetById:
                    {
                        query = @"SELECT 
                                LCA.STAFF_ID,
                                LCA.CLASS_ID,
                                LCA.COURSEWISE_ATTACHMENT_ID,
                                CC.CLASS_NAME,
                                LCA.COURSE_ID,
                                CCR.COURSE_TITLE,
                                LCF.FILE_PATH,
                                LCA.UNIT,
                                LAE.UNIT as EXCEL_UNIT,
                                TITLE,
                                SUBJECT,
                                DESCRIPTION,
                                LCA.NOTES_ID,
                                LCN.NOTES_TITLE,
                                LCA.QUESTION_PAPER_ID,
                                LQP.PAPER_TITLE,
                                LCA.QUESTION_PAPER_TYPE_ID,
                                LPT.TYPE,
                                TOPIC,
                                HOUR,
                                METHOD,
                                RESOURCE,
                                ASSESSMENT,
                                RUBRIC,
                                ANY_OTHER
                            FROM
                                LMS_COURSEWISE_ATTACHMENTS AS LCA
                                    LEFT JOIN
                                 lms_coursewise_files  AS LCF ON LCF.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
                                 INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = LCA.CLASS_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LCA.COURSE_ID
                                    INNER JOIN
                                LMS_SUP_CLASS_NOTES AS LCN ON LCN.NOTES_ID = LCA.NOTES_ID
                                    LEFT JOIN
                                LMS_SUP_QUESTION_PAPER AS LQP ON LQP.QUESTION_PAPER_ID = LCA.QUESTION_PAPER_ID
                                    LEFT JOIN 
                                LMS_SUP_QUESTION_PAPER_TYPE AS LPT ON LPT.QUESTION_PAPER_TYPE_ID = LCA.QUESTION_PAPER_TYPE_ID 
                                    LEFT JOIN 
                                LMS_COURSE_ATTACH_EXCEL AS LAE ON LAE.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
                                WHERE LCA.COURSEWISE_ATTACHMENT_ID = ?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }


                case LMSSQLCommands.fechattachmentsByNotesid:
                    {
                        query = @"SELECT 
                                LCA.STAFF_ID,
                                LCA.CLASS_ID,
                                LCA.COURSEWISE_ATTACHMENT_ID,
                                CC.CLASS_NAME,
                                LCA.COURSE_ID,
                                CCR.COURSE_TITLE,
                                LCA.UNIT,
                                TITLE,
                                SUBJECT,
                                DESCRIPTION,
                                LCA.NOTES_ID,
                                LCF.FILE_PATH,
                                LCN.NOTES_TITLE,
                                LCA.QUESTION_PAPER_ID,
                                LQP.PAPER_TITLE,
                                LCA.QUESTION_PAPER_TYPE_ID,
                                LPT.TYPE,
                                TOPIC,
                                LAE.UNIT AS EXCEL_UNIT,
                                HOUR,
                                METHOD,
                                RESOURCE,
                                ASSESSMENT,
                                RUBRIC,
                                ANY_OTHER
                            FROM
                                LMS_COURSEWISE_ATTACHMENTS AS LCA
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = LCA.CLASS_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LCA.COURSE_ID
                                    INNER JOIN
                                LMS_SUP_CLASS_NOTES AS LCN ON LCN.NOTES_ID = LCA.NOTES_ID
                                    LEFT JOIN
                                LMS_SUP_QUESTION_PAPER AS LQP ON LQP.QUESTION_PAPER_ID = LCA.QUESTION_PAPER_ID
                                    LEFT JOIN 
                                LMS_SUP_QUESTION_PAPER_TYPE AS LPT ON LPT.QUESTION_PAPER_TYPE_ID = LCA.QUESTION_PAPER_TYPE_ID 
                                    LEFT JOIN 
                                LMS_COURSE_ATTACH_EXCEL AS LAE ON LAE.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
									LEFT JOIN 
                                LMS_COURSEWISE_FILES AS LCF ON LCF.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID    
                                WHERE LCA.COURSE_ID = ?COURSE_ID AND LCA.CLASS_ID = ?CLASS_ID AND LCA.STAFF_ID = ?STAFF_ID AND LCA.NOTES_ID = ?NOTES_ID;";
                        break;
                    }


                case LMSSQLCommands.UpdateCourseWiseAttachments:
                    {
                        query = @"UPDATE LMS_COURSEWISE_ATTACHMENTS 
                                  SET
                                      STAFF_ID = ?STAFF_ID,
                                      SUBJECT = ?SUBJECT,
                                      DESCRIPTION = ?DESCRIPTION,
                                      TITLE = ?TITLE,
                                      UNIT = ?UNIT,
                                      QUESTION_PAPER_TYPE_ID = ?QUESTION_PAPER_TYPE_ID,
                                      QUESTION_PAPER_ID = ?QUESTION_PAPER_ID,
                                      CLASS_ID = ?CLASS_ID,
                                      COURSE_ID = ?COURSE_ID,
                                      NOTES_ID = ?NOTES_ID
                                    WHERE 
                                      COURSEWISE_ATTACHMENT_ID = ?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }

                case LMSSQLCommands.UpdateCourseWiseAttachmentsExcelUpload:
                    {
                        query = @"UPDATE LMS_COURSE_ATTACH_EXCEL
                                   SET
                                       TOPIC = ?TOPIC,
                                       UNIT = ?EXCEL_UNIT,
                                       IS_ACTIVE = ?IS_ACTIVE,
                                       IS_DELETED = ?IS_DELETED,
                                       HOUR = ?HOUR,
                                       METHOD = ?METHOD,
                                       RESOURCE = ?RESOURCE,
                                       ASSESSMENT = ?ASSESSMENT,
                                       RUBRIC = ?RUBRIC,
                                       ANY_OTHER = ?ANY_OTHER
                                   WHERE
                                       COURSEWISE_ATTACHMENT_ID = ?COURSEWISE_ATTACHMENT_ID;";
                        break;
                    }

                case LMSSQLCommands.FetchCourseByStaffId:
                    {
                        query = @"SELECT 
	                                COURSE_ROOT_ID,
                                    UPPER(CONCAT(IFNULL(COURSE_TITLE,''),' - ',IFNULL(COURSE_CODE,''))) AS COURSE_CODE
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CCS
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE=1
                                WHERE
                                    CCS.STAFF_ID = ?STAFF_ID
                                        AND CCS.IS_DELETED != 1
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY COURSE_ROOT_ID;";
                        break;
                    }

                case LMSSQLCommands.FetchActiveSemesterStart:
                    {
                        query = @"SELECT DATE_FROM FROM ACADEMIC_SEMESTER WHERE ACADEMIC_YEAR = 2025 AND IS_ACTIVE = 1;";
                        break;
                    }

                case LMSSQLCommands.FetchDayOrderCount:
                    {
                        query = @"SELECT DAY_ORDER_DATE, COUNT(DAY_ORDER_ID) AS HOUR FROM CALENDER AS C INNER JOIN TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = C.DAY_ORDER WHERE DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE() AND COURSE_ID = ?COURSE_ID AND DAY_ORDER_ID IN ( SELECT DAY_ORDER AS DAY_ORDER_ID FROM CALENDER WHERE DAY_ORDER_DATE =CURDATE()) AND STAFF_ID =?STAFF_ID AND CLASS_ID =?CLASS_ID AND TT.ACADEMIC_YEAR = ?AC_YEAR AND TT.SEMESTER_ID IN ( SELECT SUP_SEMESTER_ID AS SEMESTER_ID FROM SUP_SEMESTER WHERE IS_ACTIVE = 1);";
                        break;
                    }

                case LMSSQLCommands.FetchNextHourAttachment:
                    {
                        query = @"SELECT 
									SP.FIRST_NAME,
									CCR.COURSE_CODE,
									CCR.COURSE_TITLE,
									A.COURSE_ID,
									A.COURSEWISE_ATTACHMENT_ID,
									A.STAFF_ID,
									A.UNIT,
									A.CLASS_ID,
									A.TITLE,
									LSC.NOTES_TITLE,
									LSC.NOTES_ID,
									A.SUBJECT,
									A.DESCRIPTION,
									DATE_FORMAT(ENTRY_DATE, '%d-%m-%Y') AS ENTRY_DATE,
									F.FILE_PATH,
									ATT_DATE,
									LCE.TOPIC AS EXCEL_TOPIC,
									LCE.UNIT AS EXCEL_UNIT,
									LCE.HOUR,
									LCE.METHOD,
									LCE.RESOURCE,
									LCE.ASSESSMENT,
									LCE.RUBRIC,
									LCE.ANY_OTHER
								FROM
								  LMS_COURSEWISE_ATTACHMENTS AS A 
									   INNER JOIN 
									STU_CLASS AS SC ON SC.CLASS_ID = A.CLASS_ID   
									   INNER JOIN
									CP_CLASSES AS CC ON SC.CLASS_ID = CC.CLASS_ID
										INNER JOIN
									STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = A.STAFF_ID
										INNER JOIN
									CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = A.COURSE_ID
										INNER JOIN
									SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CCR.SEMESTER_ID  
										INNER JOIN 
									SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = SS.SEMESTER_TYPE
										AND ST.IS_ACTIVE = 1
										LEFT JOIN
									SUP_LMS_UNIT AS U ON U.LMS_UNIT_ID = A.UNIT
										LEFT JOIN
									LMS_COURSEWISE_FILES AS F ON F.COURSEWISE_ATTACHMENT_ID = A.COURSEWISE_ATTACHMENT_ID
									   LEFT JOIN 
								   LMS_SUP_CLASS_NOTES AS LSC ON LSC.NOTES_ID = A.NOTES_ID
									  LEFT JOIN 
								   LMS_SUP_QUESTION_PAPER AS LSQ ON LSQ.QUESTION_PAPER_ID = A.QUESTION_PAPER_ID
									   LEFT JOIN 
								   LMS_SUP_QUESTION_PAPER_TYPE AS LSQT ON LSQT.QUESTION_PAPER_TYPE_ID = A.QUESTION_PAPER_TYPE_ID
									   LEFT JOIN 
									LMS_COURSE_ATTACH_EXCEL AS LCE ON LCE.COURSEWISE_ATTACHMENT_ID = A.COURSEWISE_ATTACHMENT_ID    
								WHERE
										SC.IS_DELETED != 1
										AND A.IS_DELETED != 1
										AND SC.ACADEMIC_YEAR =?AC_YEAR
										AND LCE.HOUR = ?HOURID AND  LCE.COURSEWISE_ATTACHMENT_ID = ?COURSEWISE_ATTACHMENT_ID AND SC.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchAnnouncementForClassroom:
                    {
                        query = @"   SELECT 
                            LMS.ANNOUNCEMENT_ID,
                            CC.CLASS_NAME,
                            LMS.MESSAGE,
                            SPI.FIRST_NAME,
                            LMS.FILE_PATH,
                            CR.COURSE_TITLE,
                            ANNOUNCEMENT_TYPE
                            FROM 
                            stu_class AS SC  
                            INNER JOIN
                            CP_CLASSES AS CC ON CC.CLASS_ID=SC.CLASS_ID
                            INNER JOIN
                            LMS_COURSEWISE_ANNOUNCEMENT AS LMS ON LMS.CLASS_ID=CC.CLASS_ID
                            INNER JOIN
                            STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID=LMS.STAFF_ID
                            INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=LMS.COURSE_ID
                            WHERE LMS.IS_DELETED!=1 
                            AND LMS.COURSE_ID=?COURSE_ID
                            AND LMS.CLASS_ID=?CLASS_ID
                            GROUP BY LMS.ANNOUNCEMENT_ID;";
                      
                        break;
                    }

                case LMSSQLCommands.FetchlmscoursewiseattforClassroom:
                    {
                        query = @"  SELECT 
                                  LCA.COURSEWISE_ATTACHMENT_ID,
                                  SPI.FIRST_NAME AS STAFF_NAME,
                                  CC.CLASS_NAME ,
								  CCR.COURSE_TITLE,
                                  SLU.UNIT,
                                  LCA.TITLE,
                                  LCA.SUBJECT,
                                  LCA.DESCRIPTION,
                                  LCA.ACADEMIC_YEAR,LCF.FILE_PATH,
                                  LSCN.NOTES_TITLE,
                                  QUTY.TYPE,
                                  QP.PAPER_TITLE
                                  
                                  FROM
                                      LMS_COURSEWISE_ATTACHMENTS AS LCA
                                      LEFT JOIN
                                      STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID=LCA.STAFF_ID
                                      LEFT JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID=LCA.CLASS_ID
                                      LEFT JOIN
                                      SUP_LMS_UNIT AS SLU ON SLU.LMS_UNIT_ID=LCA.UNIT
                                      LEFT JOIN
                                      CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=LCA.COURSE_ID
                                      LEFT JOIN
                                      LMS_SUP_CLASS_NOTES AS LSCN ON LSCN.NOTES_ID=LCA.NOTES_ID
                                      LEFT JOIN
                                      LMS_SUP_QUESTION_PAPER_TYPE AS QUTY ON QUTY.QUESTION_PAPER_TYPE_ID=LCA.QUESTION_PAPER_TYPE_ID
                                      LEFT JOIN
                                      LMS_SUP_QUESTION_PAPER AS QP ON QP.QUESTION_PAPER_ID=LCA.QUESTION_PAPER_ID
                                      LEFT  JOIN 
                                      LMS_COURSEWISE_FILES AS LCF 
                                  ON LCF.COURSEWISE_ATTACHMENT_ID = LCA.COURSEWISE_ATTACHMENT_ID
                                          AND LCF.IS_DELETED != 1
                                  WHERE
                                      LCA.STAFF_ID =?STAFF_ID AND LCA.COURSE_ID = ?COURSE_ID AND
                                      LCA.IS_DELETED != 1 ORDER BY COURSEWISE_ATTACHMENT_ID DESC;";
                        break;
                    }

                case LMSSQLCommands.FetchCourse:
                    {
                        query = @" SELECT COURSE_CODE,COURSE_TITLE FROM CP_COURSE_ROOT WHERE COURSE_ROOT_ID = ?COURSE_ID;";
                        break;
                    }

                case LMSSQLCommands.Fetchassignmentsetting:
                    {
                        query = @"  SELECT 
                                S.ASSIGNMENT_SETTING_ID,
                                S.ASSIGNMENT_NAME,
                                  GROUP_CONCAT(CONCAT(SPI.FIRST_NAME, ' - ', SPI.ROLL_NO) SEPARATOR ',') AS FIRST_NAME,
                               group_concat( SPI.STUDENT_ID) as STUDENT_ID,
                                S.DESCRIPTION,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                CC.CLASS_NAME,
                                S.TOTAL_MARK,
                                DATE_FORMAT(S.START_DATE, '%d/%m/%Y') AS START_DATE,
                                DATE_FORMAT(S.END_DATE, '%d/%m/%Y') AS END_DATE,
                                S.START_TIME,
                                S.END_TIME,
                                S.LINK_URL,
                                S.FILE_PATH,
                                IF(DATE_FORMAT(S.END_DATE, '%d/%m/%Y') >= DATE_FORMAT(NOW(), '%d/%m/%Y'),
                                    1,
                                    0) AS IS_ACTIVE
                            FROM
                                LMS_ASSIGNMENT_SETTING AS S
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = S.CLASS_ID
                                INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON find_in_set( SPI.STUDENT_ID, S.STUDENT_ID)
                                INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID=S.STUDENT_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = S.COURSE_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = S.STAFF_ID
                            WHERE
                                S.IS_DELETED != 1 AND S.STAFF_ID =?STAFF_ID AND S.COURSE_ID =?COURSE_ID AND S.CLASS_ID=?CLASS_ID AND SC.ACADEMIC_YEAR=?AC_YEAR group by ASSIGNMENT_SETTING_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchMarksAndGrade:
                    {
                        query = @"SELECT 
                                S.ASSIGNMENT_SETTING_ID,
                                S.ASSIGNMENT_NAME,
                                S.STAFF_ID,
                                SP.FIRST_NAME,
                                S.COURSE_ID,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                CC.CLASS_NAME,
                                S.CLASS_ID,
                                S.TOTAL_MARK,
                                DATE_FORMAT(S.START_DATE, '%D/%M/%Y') AS START_DATE,
                                DATE_FORMAT(S.END_DATE, '%D/%M/%Y') AS END_DATE,
                                S.START_TIME,
                                S.END_TIME,
                                LAS.SEC_MARK,
                                SPI.REGISTER_NO,
                                SPI.FIRST_NAME
                            FROM
                                LMS_ASSIGNMENT_SETTING AS S
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = S.CLASS_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = S.COURSE_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = S.STAFF_ID
                                    INNER JOIN 
                                LMS_STUDENT_ASSIGNMENT_SUBMISSION  AS LAS ON LAS.ASSIGNMENT_SETTING_ID = S.ASSIGNMENT_SETTING_ID 
                                    INNER JOIN 
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LAS.STUDENT_ID     
                            WHERE
                                S.IS_DELETED != 1  AND S.STAFF_ID=?STAFF_ID
                                    AND S.COURSE_ID =?COURSE_ID;";
                        break;
                    }
                case LMSSQLCommands.FetchZoomMetingDetailsforstaff:
                    {
                        query = @"SELECT 
                                    SP.FIRST_NAME,
                                    CCR.COURSE_TITLE,
                                    SP.FIRST_NAME,
                                    DATE_FORMAT(CREATED_DATE, '%d-%m-%Y') AS CREATED_DATE,
                                    ZM.JOIN_URL,
                                    ZM.STATUS,
                                    ZM.MEETING_UID,
                                    ZM.START_URL,
                                    ZM.MEETING_ID,
                                    ZM.MEETING_PASSWORD
                                FROM
                                    
                                    ZOOM_MEETING AS ZM
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ZM.COURSE_ID
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ZM.CREATED_BY
                                WHERE
		                              ZM.COURSE_ID =?COURSE_ID
                                ORDER BY ZM.CREATED_DATE;";
                        break;
                    }

                case LMSSQLCommands.Saveannouncementfile:
                    {
                        query = @"INSERT INTO LMS_ANNOUNCEMNET_FILES(ANNOUNCEMENT_ID,FILE_PATH)VALUES (?ANNOUNCEMENT_ID,?FILE_PATH);";
                        break;
                    }
                case LMSSQLCommands.FetchUnreadMessageCount:
                    {
                        query = @"SELECT 
                                cr.CHATROOMID,
                                cr.COURSEID AS COURSE_ID,
                                COUNT(cm.MessageId) AS UNREADCOUNT
                            FROM ChatMessages cm
                            INNER JOIN ChatRooms cr ON cm.ChatRoomId = cr.ChatRoomId
                            LEFT JOIN ChatRoomUserStatus us 
                                   ON us.ChatRoomId = cr.ChatRoomId 
                                  AND us.USERID = ?STUDENT_ID
                            WHERE (us.LastReadOn IS NULL OR cm.SentAt > us.LastReadOn)
                            GROUP BY cr.ChatRoomId;";
                        break;
                    }
                case LMSSQLCommands.GetStudentContextForChat:
                    {
                        query = @"SELECT
                                    SC.CLASS_ID AS CLASS_ID,
                                    CP.DEPARTMENT AS DEPARTMENTID,
                                    CP.CLASS_YEAR AS YEAR,
                                    ASM.SEMESTER AS SEMESTERID
                                FROM 
                                    STU_CLASS AS SC
                                    INNER JOIN CP_CLASSES AS CP 
                                        ON CP.CLASS_ID = SC.CLASS_ID 
                                        AND CP.IS_DELETED != 1
                                    INNER JOIN academic_semester AS ASM 
                                        ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND ASM.BATCH = SC.BATCH
                                        AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND ASM.IS_ACTIVE = 1 
                                        AND ASM.IS_DELETED = 0
                                WHERE 
                                    SC.STUDENT_ID = ?STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND SC.IS_DELETED != 1
                                ORDER BY 
                                    ASM.SEMESTER;";
                        break;
                    }
                case LMSSQLCommands.FetchClassDetailsForChatContext:
                    {
                        query = @"SELECT
                                lccs.CLASS_ID,
                                lc.DEPARTMENT AS DEPARTMENTID ,
                                lc.CLASS_YEAR AS YEAR,
                                lccs.SEMESTER_ID AS SEMESTERID
                            FROM 
                                lms_class_course_staff lccs
                            JOIN CP_CLASSES lc ON lccs.CLASS_ID = lc.CLASS_ID
                            JOIN CP_COURSE_ROOT lco ON lccs.COURSE_ID = lco.COURSE_ROOT_ID
                            WHERE 
                                lccs.STAFF_ID = ?STAFF_ID
                                AND lccs.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                AND lccs.COURSE_ID=?COURSEID
                                AND lccs.SEMESTER_ID IN (
                                    SELECT SUP_SEMESTER_ID 
                                    FROM sup_semester 
                                    WHERE IS_ACTIVE = 1 AND IS_DELETED = 0
                                );";
                        break;
                    }

                case LMSSQLCommands.GetChatRoomByDetails:
                    {
                        query = @"SELECT CHATROOMID,
                                         COURSEID
                                      FROM CHATROOMS 
                                      WHERE CHATTYPEID = ?CHATTYPEID 
                                        AND ISACTIVE=1
                                        AND ISDELETED=0;";
                        break;
                    }
                case LMSSQLCommands.GetChatMessagesByRoomId:
                    {
                        query = @"SELECT 
                                  MessageId AS MESSAGEID, 
                                  ChatRoomId AS CHATROOMID, 
                                  SenderId AS SENDERID, 
                                  SenderName AS SENDERNAME, 
                                  MessageText AS MESSAGETEXT, 
                                  SentAt AS SENTAT 
                              FROM CHATMESSAGES 
                              WHERE CHATROOMID = ?CHATROOMID  AND COURSEID = ?COURSE_ID 
                              ORDER BY SENTAT ASC;";
                        break;
                    }
                case LMSSQLCommands.FetchStaffCoursesForChat:
                    {
                        query = @"SELECT
                                lccs.COURSE_ID,
                                lc.DEPARTMENT,
                                lc.CLASS_YEAR AS YEAR,
                                lccs.SEMESTER_ID,
                                lccs.ACADEMIC_YEAR,
                                lccs.CLASS_ID,
                                lc.CLASS_NAME,
                                lco.COURSE_TITLE,
                                lco.COURSE_CODE,
                                lccs.STAFF_ID
                            FROM 
                                lms_class_course_staff lccs
                            JOIN CP_CLASSES lc ON lccs.CLASS_ID = lc.CLASS_ID
                            JOIN CP_COURSE_ROOT lco ON lccs.COURSE_ID = lco.COURSE_ROOT_ID
                            WHERE 
                                lccs.STAFF_ID = ?STAFF_ID
                                AND lccs.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                AND lccs.SEMESTER_ID IN (
                                    SELECT SUP_SEMESTER_ID 
                                    FROM sup_semester 
                                    WHERE IS_ACTIVE = 1 AND IS_DELETED = 0
                                );";
                        break;
                    }
                case LMSSQLCommands.UpdateUserChatStatus:
                    {
                        query = @"INSERT INTO ChatRoomUserStatus (ChatRoomId, UserId, LastReadOn)
                              VALUES (
                                  (SELECT ChatRoomId FROM ChatRooms WHERE CourseId = ?CHATROOMID),
                                  ?USERID,
                                  NOW()
                              )
                              ON DUPLICATE KEY UPDATE
                              LastReadOn = NOW();";
                        break;
                    }

                case LMSSQLCommands.FetchClassIdByCourseandStaff:
                    {
                        query = @"  SELECT 
                                    CP.CLASS_ID,
                                    CP.COURSE_ID,
                                    CP.ACADEMIC_YEAR,
                                    SST.SEMESTER_TYPE
                                FROM
                                    CP_CLASSES AS CC
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CP ON CP.CLASS_ID = CC.CLASS_ID
                                        INNER JOIN
                                    sup_semester_type AS SST ON SST.SEMESTER_TYPE_ID = CP.SEMESTER_TYPE_ID
                                WHERE
                                    CP.CLASS_ID = ?CLASS_ID 
                                    AND CP.COURSE_ID = ?COURSE_ID 
                                    AND SST.IS_ACTIVE=1
                                    AND CP.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CP.IS_DELETED!=1";
                        break;
                    }
                case LMSSQLCommands.FetchStudentNameList:
                    {
                            query = @"  SELECT 
                                        FIRST_NAME, 
                                        SPI.STUDENT_ID
                                    FROM
                                        stu_personal_info AS SPI
                                            INNER JOIN
                                        stu_class AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID
                                        AND SPI.IS_LEFT!=1 
                                        AND spi.IS_DELETED!=1
                                        AND SC.ACADEMIC_YEAR=?AC_YEAR";
                        break;
                    }


                case LMSSQLCommands.DeleteLmsAnnouncementbyid:
                    {
                        query = @"UPDATE LMS_COURSEWISE_ANNOUNCEMENT 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        ANNOUNCEMENT_ID =?ANNOUNCEMENT_ID;";
                        break;
                    }
                case LMSSQLCommands.UpdateAssignmentSettingS:
                    {
                        query = @"UPDATE LMS_ASSIGNMENT_SETTING 
                                    SET 
                                        ASSIGNMENT_NAME =?ASSIGNMENT_NAME,
                                        TOTAL_MARK =?TOTAL_MARK,
                                        DESCRIPTION =?DESCRIPTION,
                                        LINK_URL =?LINK_URL,
                                        FILE_PATH =?FILE_PATH,
                                        START_DATE =?START_DATE,
                                        END_DATE =?END_DATE,
                                        START_TIME =?START_TIME,
                                        END_TIME =?END_TIME,
                                        STUDENT_ID =?STUDENT_ID
                                    WHERE
                                        ASSIGNMENT_SETTING_ID =?ASSIGNMENT_SETTING_ID;";
                        break;
                    }
                case LMSSQLCommands.SaveAssignmentSettingS:
                    {
                        query = @"INSERT INTO LMS_ASSIGNMENT_SETTING
                             (
                                 ASSIGNMENT_NAME,
                                 STAFF_ID,
                                 STUDENT_ID,
                                 COURSE_ID,
                                 CLASS_ID,
                                 DESCRIPTION,
                                 TOTAL_MARK,
                                 START_DATE,
                                 END_DATE,
                                 START_TIME,
                                 FILE_PATH,
                                 LINK_URL,
                                 END_TIME,
                                 ACADEMIC_YEAR
                             )
                             VALUES( 
                                 ?ASSIGNMENT_NAME,
                                 ?STAFF_ID,
                                 ?STUDENT_ID,
                                 ?COURSE_ID,
                                 ?CLASS_ID,
                                 ?DESCRIPTION,
                                 ?TOTAL_MARK,
                                 ?START_DATE,
                                 ?END_DATE,
                                 ?START_TIME,
                                 ?FILE_PATH,
                                 ?LINK_URL,
                                 ?END_TIME,
                                 ?AC_YEAR);";
                        break;
                    }
                case LMSSQLCommands.FetchStudentAssignmentSubmissionBySettingId:
                    {
                        query = @"SELECT 
                        LSA.STU_ASSIGNMENT_ID,
                        LSA.STUDENT_ID,
                        SP.FIRST_NAME,
                        LSA.COURSE_ID,
                        LA.TOTAL_MARK,
                        LSA.SEC_MARK,
                        LSA.ASSIGNMENT_SETTING_ID,
                        LSA.FILE_PATH,
                        LSA.ENTRY_DATE
                    FROM
                        LMS_STUDENT_ASSIGNMENT_SUBMISSION AS LSA
                            INNER JOIN 
                        LMS_ASSIGNMENT_SETTING AS LA ON LA.ASSIGNMENT_SETTING_ID = LSA.ASSIGNMENT_SETTING_ID     
                            INNER JOIN
                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = LSA.STUDENT_ID WHERE LSA.ASSIGNMENT_SETTING_ID = ?ASSIGNMENT_SETTING_ID;";
                        break;
                    }


            }
            return query;
        }

    }
}
