using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.TaskManagement
{
    public class TaskManagementSQL
    {
        public static string GetTaskManagementSQL(TaskManagementSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case TaskManagementSQLCommands.FetchAssignmentList:
                    {
                        query = @"SELECT 
                                        ASS.ASSIGNMENT_ID,
                                        ASS.ASSIGNMENT_TITLE,
                                        ASS.DESCRIPTION,
                                        ASS.ACADEMIC_YEAR,
                                        DATE_FORMAT(ASS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(ASS.SUBMISSION_DATE, '%d/%m/%Y') AS SUBMISSION_DATE,
                                        IS_FILE_UPLOAD
                                    FROM
                                        ASSIGNMENT_?AC_YEAR AS ASS
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ASS.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ASS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ASS.COURSE_ID
                                    WHERE
                                        ASS.IS_DELETED != 1
                                            AND SP.IS_DELETED != 1
                                            AND CR.IS_DELETED != 1
                                            AND ASS.ACADEMIC_YEAR = ?AC_YEAR AND ASS.CLASS_ID=?CLASS_ID AND ASS.STAFF_ID=?STAFF_ID AND ASS.COURSE_ID=?COURSE_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveAssignment:
                    {
                        query = @"INSERT INTO ASSIGNMENT_?AC_YEAR
                                            (
                                            STAFF_ID,
                                            ACADEMIC_YEAR,
                                            CLASS_ID,
                                            COURSE_ID,
                                            ASSIGNMENT_TITLE,
                                            DATE_FROM,
                                            SUBMISSION_DATE,
                                            DESCRIPTION,
                                            IS_ACTIVE,
                                            IS_FILE_UPLOAD)
                                            VALUES
                                            (
                                            ?STAFF_ID,
                                            ?AC_YEAR,
                                            ?CLASS_ID,
                                            ?COURSE_ID,
                                            ?ASSIGNMENT_TITLE,
                                            ?DATE_FROM,
                                            ?SUBMISSION_DATE,
                                            ?DESCRIPTION,
                                            ?IS_ACTIVE,
                                            ?IS_FILE_UPLOAD);";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAssignmentById:
                    {
                        query = @"SELECT 
                                        ASSIGNMENT_ID,
                                        STAFF_ID,
                                        ACADEMIC_YEAR,
                                        CLASS_ID,
                                        COURSE_ID,
                                        ASSIGNMENT_TITLE,
                                        DATE_FROM,
                                        DATE_FORMAT(SUBMISSION_DATE, '%d/%m/%Y') AS SUBMISSION_DATE,
                                        DESCRIPTION,
                                        IS_ACTIVE,
                                        IS_FILE_UPLOAD
                                    FROM
                                        ASSIGNMENT_?AC_YEAR
                                    WHERE
                                        ASSIGNMENT_ID = ?ASSIGNMENT_ID AND IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAssignment:
                    {
                        query = @"UPDATE ASSIGNMENT_?AC_YEAR
                                            SET
                                            STAFF_ID=?STAFF_ID,
                                            ACADEMIC_YEAR=?AC_YEAR,
                                            CLASS_ID=?CLASS_ID,
                                            COURSE_ID=?COURSE_ID,
                                            ASSIGNMENT_TITLE=?ASSIGNMENT_TITLE,
                                            SUBMISSION_DATE=?SUBMISSION_DATE,
                                            IS_ACTIVE=?IS_ACTIVE,
                                            DESCRIPTION=?DESCRIPTION,
                                            IS_FILE_UPLOAD=?IS_FILE_UPLOAD
                                            WHERE ASSIGNMENT_ID=?ASSIGNMENT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteAssignment:
                    {
                        query = @"UPDATE ASSIGNMENT_?AC_YEAR
                                            SET
                                            IS_DELETED=1
                                            WHERE ASSIGNMENT_ID=?ASSIGNMENT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAssignmentByClassIdAndCourseIdAndStaffId:
                    {
                        query = @"SELECT 
                                    ASSIGNMENT_ID, ASSIGNMENT_TITLE
                                FROM
                                    ASSIGNMENT_?AC_YEAR
                                WHERE
                                    STAFF_ID = ?STAFF_ID AND CLASS_ID = ?CLASS_ID
                                        AND COURSE_ID = ?COURSE_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAssignmentSubmittedList:
                    {
                        query = @"SELECT 
                                SP.FIRST_NAME,
                                SP.ROLL_NO,
                                AGS.STUDENT_ID,
                                AGS.ASSIGNMENT_ID,
                                SP.CLASS_ID,
                                AGS.FILE_PATH,
                                date_format(AGS.SUBMITTED_DATE,'%Y-%m-%d') AS SUBMITTED_DATE
                            FROM
                                ASSIGNMENT_SUBMISSION_?AC_YEAR AS AGS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = AGS.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = AGS.CLASS_ID
                            WHERE
                                AGS.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchLastSubmissionEventInfo:
                    {
                        query = @"SELECT 
                                    EVENT_ID,
                                    DAY_ID,
                                    EVENT_START_DATE,
                                    EVENT_NAME
                                FROM
                                    CALENDAR_EVENTS_?AC_YEAR
                                WHERE
                                    EVENT_NAME LIKE '%LAST%';";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskSettingList:
                    {
                        query = @"SELECT 
                                    TS.TASK_ID,
                                    TS.TASK_NAME,
                                    DATE_FORMAT(TS.LAST_DATE, '%d/%m/%Y') AS 'LAST_DATE',
                                    TS.SEMESTER,
                                    TS.ACADEMIC_YEAR,
                                    CC.COMPONENT AS 'COMPONENT_ID'
                                FROM
                                    SUP_TASK_SETTING AS TS
                                        LEFT JOIN
                                    SUP_CIA_COMPONENTS AS CC ON CC.COMPONENT_ID = TS.COMPONENT_ID
                                WHERE
                                    TS.ACADEMIC_YEAR = ?AC_YEAR AND TS.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskSettingById:
                    {
                        query = @"SELECT 
                                    TS.TASK_ID,
                                    TS.TASK_NAME,
                                    DATE_FORMAT(TS.LAST_DATE, '%d/%m/%Y') AS 'LAST_DATE',
                                    UPPER(TS.SEMESTER) AS SEMESTER,
                                    TS.ACADEMIC_YEAR,
                                    TS.COMPONENT_ID
                                FROM
                                    SUP_TASK_SETTING AS TS
                                WHERE
                                    TS.TASK_ID = ?TASK_ID AND TS.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveTaskSetting:
                    {
                        query = @"INSERT INTO SUP_TASK_SETTING (
                                    TASK_NAME,
                                    LAST_DATE,
                                    SEMESTER,
                                    ACADEMIC_YEAR,
                                    COMPONENT_ID)
                                    VALUES
                                    (?TASK_NAME,
                                    ?LAST_DATE,
                                    ?SEMESTER,
                                    ?AC_YEAR,
                                    ?COMPONENT_ID);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateTaskSetting:
                    {
                        query = @"UPDATE SUP_TASK_SETTING
                                  SET
                                  TASK_NAME = ?TASK_NAME,
                                  LAST_DATE = ?LAST_DATE,
                                  SEMESTER = ?SEMESTER,
                                  ACADEMIC_YEAR = ?AC_YEAR,
                                  COMPONENT_ID = ?COMPONENT_ID
                                  WHERE TASK_ID = ?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteTaskSetting:
                    {
                        query = @"UPDATE SUP_TASK_SETTING SET IS_DELETED=1 WHERE TASK_ID = ?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.IsTaskSettingExist:
                    {
                        query = @"SELECT TASK_ID,
                                    TASK_NAME,
                                    LAST_DATE,
                                    SEMESTER,
                                    ACADEMIC_YEAR,
                                    COMPONENT_ID
                                FROM SUP_TASK_SETTING WHERE ACADEMIC_YEAR=?AC_YEAR 
                                AND IS_DELETED!=1 
                                AND SEMESTER=?SEMESTER
                                AND COMPONENT_ID=?COMPONENT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchCiaComponent:
                    {
                        query = @"SELECT 
                                        CIA_GROUP_COMPONENT_ID,
                                        CIA_GROUP_ID,
                                        SUP_CIA_COMPONENT_ID AS 'COMPONENT_ID',
                                        SCC.COMPONENT,
                                        MAX_MARK
                                    FROM
                                        CIA_GROUP_COMPONENT AS CG
		                                    INNER JOIN
	                                    SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID=CG.SUP_CIA_COMPONENT_ID AND SCC.IS_DELETED!=1
                                    WHERE
                                        SUP_CIA_COMPONENT_ID = ?COMPONENT_ID AND CG.IS_DELETED!=1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchStudentAssignmentfile:
                    {
                        query = @"SELECT 
                                SP.FIRST_NAME,
                                SP.ROLL_NO,
                                AGS.STUDENT_ID,
                                AGS.ASSIGNMENT_ID,
                                SP.CLASS_ID,
                                AGS.FILE_PATH
                            FROM
                                ASSIGNMENT_SUBMISSION_?AC_YEAR AS AGS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = AGS.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = AGS.CLASS_ID
                            WHERE
                                AGS.IS_DELETED != 1 
                                AND AGS.STUDENT_ID=?STUDENT_ID 
                                AND AGS.ASSIGNMENT_ID=?ASSIGNMENT_ID;";
                        break;
                    }

                case TaskManagementSQLCommands.FetchCia_Status:
                    {
                        query = @"SELECT 
                                        CCS.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS 'NAME',
                                        CCS.COURSE_ID,
                                        CONCAT(IFNULL(R.COURSE_TITLE,''),' ( ',IFNULL(R.COURSE_CODE,''),' )') AS 'COURSE',
                                        CCS.CLASS_ID,
                                        C.CLASS_NAME,
                                        CCS.SEMESTER_ID,
                                        SS.SEMESTER_NAME,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS1, '|', 1) AS COMPONENT_STATUS1,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS1, '|', -1) AS 'STATUS1',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS2, '|', 1) AS COMPONENT_STATUS2,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS2, '|', -1) AS 'STATUS2',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS3, '|', 1) AS COMPONENT_STATUS3,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS3, '|', -1) AS 'STATUS3',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS4, '|', 1) AS COMPONENT_STATUS4,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS4, '|', -1) AS 'STATUS4',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS5, '|', 1) AS COMPONENT_STATUS5,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS5, '|', -1) AS 'STATUS5',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS6, '|', 1) AS COMPONENT_STATUS6,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS6, '|', -1) AS 'STATUS6',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS7, '|', 1) AS COMPONENT_STATUS7,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS7, '|', -1) AS 'STATUS7',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS8, '|', 1) AS COMPONENT_STATUS8,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS8, '|', -1) AS 'STATUS8',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS9, '|', 1) AS COMPONENT_STATUS9,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS9, '|', -1) AS 'STATUS9',
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS10, '|', 1) AS COMPONENT_STATUS10,
                                        SUBSTRING_INDEX(S.COMPONENT_STATUS10, '|', -1) AS 'STATUS10'
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CCS
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = CCS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS R ON R.COURSE_ROOT_ID = CCS.COURSE_ID
                                            INNER JOIN
										SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
		                                    INNER JOIN
	                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID=CCS.SEMESTER_ID
		                                    LEFT JOIN
	                                    CIA_STATUS AS S ON S.STAFF_ID=CCS.STAFF_ID 
                                            AND S.CLASS_ID=CCS.CLASS_ID 
                                            AND S.COURSE_ID=CCS.COURSE_ID
                                            AND S.ACADEMIC_YEAR=CCS.ACADEMIC_YEAR
                                            AND SUBSTRING_INDEX(S.COMPONENT_STATUS1, '|', 1)=?COMPONENT_ID OR 
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS2, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS3, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS4, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS5, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS6, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS7, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS8, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS9, '|', 1)=?COMPONENT_ID OR
                                                SUBSTRING_INDEX(S.COMPONENT_STATUS10, '|', 1)=?COMPONENT_ID
                                    WHERE 
                                        CCS.ACADEMIC_YEAR=?AC_YEAR 
                                        AND CCS.IS_DELETED!=0
                                   ORDER BY R.SEMESTER_ID,R.PART,C.CLASS_YEAR;";
                        break;
                    }
                #region CIA Block List
                case TaskManagementSQLCommands.ListCIABlockItems:
                    {
                        query = @"SELECT 
                                    CIA_BLOCK_ID,
                                    STAFF_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    PROGRAMME_GROUP_ID,
                                    PROGRAMME_LEVEL,
                                    SHIFT_ID,
                                    S_STAFF_ID,
                                    S_CLASS_ID,
                                    S_COURSE_ID,
                                    DATE_FROM,
                                    DATE_TO,
                                    IS_ACTIVE
                                FROM
                                    CIA_BLOCK_LIST
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR 
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchCiaBlockListById:
                    {
                        query = @"SELECT 
                                    CIA_BLOCK_ID,
                                    STAFF_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    PROGRAMME_GROUP_ID,
                                    PROGRAMME_LEVEL,
                                    SHIFT_ID,
                                    S_STAFF_ID,
                                    S_CLASS_ID,
                                    S_COURSE_ID,
                                    ORDER_ID,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y') as DATE_FROM ,
                                    DATE_FORMAT(DATE_TO,'%d/%m/%Y') as DATE_TO ,
                                    IS_ACTIVE
                                FROM
                                    CIA_BLOCK_LIST
                                WHERE
                                   CIA_BLOCK_ID=?CIA_BLOCK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchCiaBlockList:
                    {
                        query = @"SELECT 
                                    CIA_BLOCK_ID,
                                    STAFF_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    PROGRAMME_GROUP_ID,
                                    PROGRAMME_LEVEL,
                                    SHIFT_ID,
                                    S_STAFF_ID,
                                    S_CLASS_ID,
                                    S_COURSE_ID,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                    IS_ACTIVE
                                FROM
                                    CIA_BLOCK_LIST
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR 
                                        AND IS_DELETED != 1 AND ?DATE_TO<= DATE_TO ;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveCiaBlockList:
                    {
                        query = @"INSERT INTO CIA_BLOCK_LIST
                                    (
                                    STAFF_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    PROGRAMME_GROUP_ID,
                                    PROGRAMME_LEVEL,
                                    SHIFT_ID,
                                    S_STAFF_ID,
                                    S_CLASS_ID,
                                    S_COURSE_ID,
                                    DATE_FROM,
                                    DATE_TO,
                                    ACADEMIC_YEAR,
                                    ORDER_ID,
                                    IS_ACTIVE)
                                    VALUES
                                    (
                                    ?STAFF_ID,
                                    ?CLASS_ID,
                                    ?COURSE_ID,
                                    ?PROGRAMME_GROUP_ID,
                                    ?PROGRAMME_LEVEL,
                                    ?SHIFT_ID,
                                    ?S_STAFF_ID,
                                    ?S_CLASS_ID,
                                    ?S_COURSE_ID,
                                    ?DATE_FROM,
                                    ?DATE_TO,
                                    ?AC_YEAR,
                                    ?ORDER_ID,
                                    ?IS_ACTIVE);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateCiaBlockList:
                    {
                        query = @"UPDATE CIA_BLOCK_LIST SET 
                                    STAFF_ID=?STAFF_ID,
                                    CLASS_ID=?CLASS_ID,
                                    COURSE_ID=?COURSE_ID,
                                    PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID,
                                    PROGRAMME_LEVEL=?PROGRAMME_LEVEL,
                                    SHIFT_ID=?SHIFT_ID,
                                    S_STAFF_ID=?S_STAFF_ID,
                                    S_CLASS_ID=?S_CLASS_ID,
                                    S_COURSE_ID=?S_COURSE_ID,
                                    DATE_FROM=?DATE_FROM,
                                    DATE_TO=?DATE_TO,
                                    IS_ACTIVE=?IS_ACTIVE WHERE CIA_BLOCK_ID=?CIA_BLOCK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateActiveCIABlockList:
                    {
                        query = @"UPDATE CIA_BLOCK_LIST SET IS_ACTIVE=?IS_ACTIVE WHERE CIA_BLOCK_ID=?CIA_BLOCK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteCiaBlockList:
                    {
                        query = @"UPDATE CIA_BLOCK_LIST SET IS_DELETED=1 WHERE CIA_BLOCK_ID=?CIA_BLOCK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchCIAListByOrderId:
                    {
                        query = @"SELECT 
                                    CIA_BLOCK_ID,
                                    CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS STAFF_ID,
                                    CLS.CLASS_NAME AS CLASS_ID,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) COURSE_ID,
                                    P.PROGRAMME_NAME AS PROGRAMME_GROUP_ID,
                                    SPL.PROGRAMME_LEVEL AS PROGRAMME_LEVEL,
                                    SS.SHIFT_NAME AS SHIFT_ID,
	                                CONCAT(IFNULL(SSP.FIRST_NAME,''),' ',IFNULL(SSP.LAST_NAME,'')) AS S_STAFF_ID,
                                    SCLS.CLASS_NAME AS S_CLASS_ID,
                                    CONCAT(IFNULL(SCR.COURSE_TITLE,''),' ',IFNULL(SCR.COURSE_CODE,''))  AS S_COURSE_ID,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM ,
                                    DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO ,
                                    CIA.IS_ACTIVE
                                FROM
                                    CIA_BLOCK_LIST AS CIA
		                                LEFT JOIN
	                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=CIA.STAFF_ID  AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
		                                LEFT JOIN
	                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=CIA.COURSE_ID  AND CR.IS_DELETED!=1
		                                LEFT JOIN
	                                CP_CLASSES AS CLS ON CLS.CLASS_ID=CIA.CLASS_ID AND CLS.IS_DELETED!=1
		                                LEFT JOIN
	                                SUP_SHIFT AS SS ON SS.SHIFT_ID=CIA.SHIFT_ID AND SS.IS_DELETED!=1
		                                LEFT JOIN
	                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=CIA.PROGRAMME_GROUP_ID AND PG.IS_DELETED!=1
		                                LEFT JOIN
	                                CP_PROGRAMME AS P ON P.PROGRAMME_ID=PG.PROGRAMME_ID  AND P.IS_DELETED!=1
		                                LEFT JOIN
	                                SUP_PROGRAMME_LEVEL AS SPL ON SPL.PROGRAMME_LEVEL_ID=CIA.PROGRAMME_LEVEL AND SPL.IS_DELETED!=1 
		                                LEFT JOIN
	                                STF_PERSONAL_INFO AS SSP ON SSP.STAFF_ID=CIA.S_STAFF_ID AND SSP.IS_DELETED!=1 AND SSP.IS_LEFT!=1
		                                LEFT JOIN
	                                CP_COURSE_ROOT AS SCR ON SCR.COURSE_ROOT_ID=CIA.S_COURSE_ID AND SCR.IS_DELETED!=1
		                                LEFT JOIN
	                                CP_CLASSES AS SCLS ON SCLS.CLASS_ID=CIA.S_CLASS_ID AND SCLS.IS_DELETED!=1
                                WHERE
	                                CIA.ACADEMIC_YEAR=?AC_YEAR AND CIA.IS_DELETED!=1 AND ORDER_ID=?ORDER_ID;";
                        break;
                    }
                #endregion

                #region AcademicActivity
                case TaskManagementSQLCommands.ListAcademicActivity:
                    {
                        query = @"SELECT 
                                    ACTIVITY_ID,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SAL.ACTIVITY_LEVEL,
                                    STAFF_ID,
                                    ACTIVITY_TITLE,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM ,
                                    DATE_FORMAT(SUBMISSION_DATE,'%d/%m/%Y') AS SUBMISSION_DATE,
                                    DESCRIPTION,
                                    IS_FILE_UPLOAD,
                                    SUBMITTED_DATE,
                                    FILE_PATH,
                                    PHOTO_PATH,
                                    SAS.ACTIVITY_STATUS_NAME AS 'STATUS',
                                    AA.IS_ACTIVE
                                FROM
                                    ACADEMIC_ACTIVITY AS AA
		                                INNER JOIN
	                                SUP_ACTIVITY_LEVEL AS SAL ON SAL.ACTIVITY_LEVEL_ID=AA.ACTIVITY_LEVEL
		                                INNER JOIN
	                                SUP_ACTIVITY_STATUS AS SAS ON SAS.ACTIVITY_STATUS_ID=AA.STATUS
                                WHERE
                                     AA.ACADEMIC_YEAR = ?AC_YEAR ENTRY_ID=?STAFF_ID
                                        AND AA.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAcademicActivityById:
                    {
                        query = @"SELECT 
                                    ACTIVITY_ID,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    ACTIVITY_LEVEL,
                                    STAFF_ID,
                                    ACTIVITY_TITLE,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM ,
                                    DATE_FORMAT(SUBMISSION_DATE,'%d/%m/%Y') AS SUBMISSION_DATE,
                                    DESCRIPTION,
                                    IS_FILE_UPLOAD,
                                    DATE_FORMAT(SUBMITTED_DATE,'%d/%m/%Y') AS SUBMITTED_DATE,
                                    FILE_PATH,
                                    PHOTO_PATH,
                                    STATUS,
                                    IS_ACTIVE
                                FROM
                                    ACADEMIC_ACTIVITY
                                WHERE
                                    ACTIVITY_ID = ?ACTIVITY_ID AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case TaskManagementSQLCommands.FetchAcademicActivityFileCorrectionById:
                    {
                        query = @"SELECT 
                                    ACTIVITY_ID,
                                    FILE_CORRECTION
                                FROM
                                    ACADEMIC_ACTIVITY
                                WHERE
                                    ACTIVITY_ID = ?ACTIVITY_ID AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case TaskManagementSQLCommands.FetchAcademicActivityFileCorrectionReturnById:
                    {
                        query = @"SELECT 
                                    ACTIVITY_ID,
                                    FILE_CORRECTION_RETURN
                                FROM
                                    ACADEMIC_ACTIVITY
                                WHERE
                                    ACTIVITY_ID = ?ACTIVITY_ID AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }


                case TaskManagementSQLCommands.SaveAcademicActivity:
                    {
                        query = @"INSERT INTO ACADEMIC_ACTIVITY(
                                        ENTRY_ID,
                                        ACADEMIC_YEAR,
                                        ACTIVITY_LEVEL,
                                        STAFF_ID,
                                        ACTIVITY_TITLE,
                                        DATE_FROM,
                                        SUBMISSION_DATE,
                                        DESCRIPTION,
                                        IS_FILE_UPLOAD,
                                        IS_ACTIVE)
                                        VALUES
                                        (
                                        ?ENTRY_ID,
                                        ?AC_YEAR,
                                        ?ACTIVITY_LEVEL,
                                        ?STAFF_ID,
                                        ?ACTIVITY_TITLE,
                                        ?DATE_FROM,
                                        ?SUBMISSION_DATE,
                                        ?DESCRIPTION,
                                        ?IS_FILE_UPLOAD,
                                        ?IS_ACTIVE);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivity:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY
                                    SET
                                    ENTRY_ID = ?ENTRY_ID,
                                    ACTIVITY_LEVEL = ?ACTIVITY_LEVEL,
                                    STAFF_ID = ?STAFF_ID,
                                    ACTIVITY_TITLE = ?ACTIVITY_TITLE,
                                    DATE_FROM = ?DATE_FROM,
                                    SUBMISSION_DATE = ?SUBMISSION_DATE,
                                    DESCRIPTION = ?DESCRIPTION,
                                    IS_FILE_UPLOAD = ?IS_FILE_UPLOAD,
                                    SUBMITTED_DATE = ?SUBMITTED_DATE,
                                    IS_ACTIVE = ?IS_ACTIVE
                                    WHERE ACTIVITY_ID = ?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteAcademicActivity:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET IS_DELETED=1 WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionById:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET FILE_CORRECTION=?FILE_CORRECTION,FILE_CORRECTION_STATUS=1 WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionReturnById:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET FILE_CORRECTION_RETURN=?FILE_CORRECTION_RETURN,FILE_CR_STATUS=1 WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionStatusById:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET FILE_CORRECTION_STATUS=?FILE_CORRECTION_STATUS WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityForFileCorrectionReturnStatusById:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET FILE_CR_STATUS=?FILE_CR_STATUS WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityFile:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET FILE_PATH=?FILE_PATH WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityImage:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET PHOTO_PATH=?PHOTO_PATH WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateAcademicActivityStatus:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET STATUS=?STATUS , SUBMITTED_DATE=CURDATE() WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateStatus:
                    {
                        query = @"UPDATE ACADEMIC_ACTIVITY SET STATUS=?STATUS  WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchStaffIdByAcademicYearAndActivityLevel:
                    {
                        query = @"SELECT 
                                    STAFF_ID,ACTIVITY_ID
                                FROM
                                    ACADEMIC_ACTIVITY
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR
                                        AND ACTIVITY_LEVEL = ?ACTIVITY_LEVEL
                                        AND IS_DELETED != 1 AND ENTRY_ID=?ENTRY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchActivityByActivityLevel:
                    {
                        query = @"SELECT 
                                    ACTIVITY_ID,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SAL.ACTIVITY_LEVEL,
                                    GROUP_CONCAT(SP.STAFF_ID) AS STAFF_ID,
                                    GROUP_CONCAT(CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) ) AS FIRST_NAME,
                                    ACTIVITY_TITLE,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM ,
                                    DATE_FORMAT(SUBMISSION_DATE,'%d/%m/%Y') AS SUBMISSION_DATE,
                                    DESCRIPTION,
                                    IS_FILE_UPLOAD,
                                    DATE_FORMAT(SUBMITTED_DATE,'%d/%m/%Y') AS SUBMITTED_DATE,
                                    FILE_PATH,
                                    PHOTO_PATH,
                                    SAS.ACTIVITY_STATUS_NAME,
                                    AA.STATUS,
                                    AA.IS_ACTIVE,
                                    AA.FILE_CORRECTION,
                                    AA.FILE_CORRECTION_STATUS,
                                    AA.FILE_CORRECTION_RETURN,
                                    AA.FILE_CR_STATUS
                                FROM
                                    ACADEMIC_ACTIVITY AS AA
		                                INNER JOIN
	                                SUP_ACTIVITY_LEVEL AS SAL ON SAL.ACTIVITY_LEVEL_ID=AA.ACTIVITY_LEVEL
		                                LEFT JOIN
	                                SUP_ACTIVITY_STATUS AS SAS ON SAS.ACTIVITY_STATUS_ID=AA.STATUS
		                                INNER JOIN
	                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID IN (?STAFF_ID) AND SP.IS_DELETED!=1 AND IS_LEFT!=1
                                WHERE
                                     AA.ACADEMIC_YEAR = ?AC_YEAR AND AA.ACTIVITY_LEVEL = ?ACTIVITY_LEVEL
                                        AND AA.IS_DELETED != 1 AND AA.ACTIVITY_ID=?ACTIVITY_ID AND ENTRY_ID=?ENTRY_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchActivityStatusForStaff:
                    {
                        query = @"SELECT 
                                        ACTIVITY_ID,
                                        ENTRY_ID,
                                        ACADEMIC_YEAR,
                                        SA.ACTIVITY_LEVEL,
                                        ACTIVITY_LEVEL_ID,
                                        SP.STAFF_ID,
                                        ACTIVITY_TITLE,
                                        DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(SUBMISSION_DATE, '%d/%m/%Y') AS SUBMISSION_DATE,
                                        DESCRIPTION,
                                        IS_FILE_UPLOAD,
                                        DATE_FORMAT(SUBMITTED_DATE, '%d/%m/%Y') AS SUBMITTED_DATE,
                                        FILE_PATH,
                                        PHOTO_PATH,
										SAS.ACTIVITY_STATUS_NAME,
                                        STATUS,
                                        AA.IS_ACTIVE,
                                        AA.FILE_CORRECTION,
                                        AA.FILE_CORRECTION_STATUS,
                                        AA.FILE_CORRECTION_RETURN,
										AA.FILE_CR_STATUS
                                    FROM
                                        ACADEMIC_ACTIVITY AS AA
                                            INNER JOIN
                                        SUP_ACTIVITY_LEVEL AS SA ON SA.ACTIVITY_LEVEL_ID=AA.ACTIVITY_LEVEL
											INNER JOIN
										STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ?STAFF_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
											LEFT JOIN
										SUP_ACTIVITY_STATUS AS SAS ON SAS.ACTIVITY_STATUS_ID=AA.STATUS
                                    WHERE
                                         AA.IS_DELETED != 1 AND FIND_IN_SET(?STAFF_ID,AA.STAFF_ID) AND ACADEMIC_YEAR=?AC_YEAR";
                        break;
                    }


                #endregion

                #region Student Leave Request 
                case TaskManagementSQLCommands.ListStudentLeaveRequest:
                    {
                        query = @"SELECT 
                                    LEAVE_REQUEST_ID,
                                    STUDENT_ID,
                                    SUBJECT,
                                    REASON,
                                    DATE_FORMAT(REQUEST_DATE,'%d/%m/%Y')as REQUEST_DATE,
                                    DATE_FORMAT(DATE_FROM,'%d/%m/%Y')as DATE_FROM,
                                    DATE_FORMAT(DATE_TO,'%d/%m/%Y')as DATE_TO,
                                    SLA.APPROVE_TO AS APPROVE_BY,
                                    DATE_FORMAT(APPROVED_DATE,'%d/%m/%Y') as APPROVED_DATE,
                                    CONCAT(IFNULL(SP.FIRST_NAME,''),'',IFNULL(SP.LAST_NAME,'')) AS 'NAME',
                                    SLR.STATUS,
                                    ACADEMIC_YEAR,
                                    SL.STATUS_NAME
                                FROM
                                    STU_LEAVE_REQUEST AS SLR
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = SLR.APPROVED_BY
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
	                                SUP_LEAVE_STATUS AS SL ON SL.STATUS_ID=SLR.STATUS
		                                INNER JOIN
	                                SUP_LEAVE_APPROVAL AS SLA ON SLA.APPROVAL_ID=SLR.APPROVE_BY
                                WHERE
                                    SLR.ACADEMIC_YEAR = ?AC_YEAR AND SLR.STUDENT_ID=?STUDENT_ID
                                        AND SLR.IS_DELETED != 1;";
                        break;
                    }

                case TaskManagementSQLCommands.FetchStudentLeaveRequestById:
                    {
                        query = @"SELECT 
                                    LEAVE_REQUEST_ID,
                                    STUDENT_ID,
                                    SUBJECT,
                                    REASON,
                                    date_format(DATE_FROM,'%d/%m/%Y') as DATE_FROM,
                                    date_format(DATE_TO,'%d/%m/%Y') as DATE_TO,
                                    APPROVE_BY,
                                    date_format(APPROVED_DATE,'%d/%m/%Y') as APPROVED_DATE,
                                    ACADEMIC_YEAR,
                                    STATUS
                                FROM
                                    STU_LEAVE_REQUEST
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR AND LEAVE_REQUEST_ID=?LEAVE_REQUEST_ID AND IS_DELETED != 1;";
                        break;
                    }

                case TaskManagementSQLCommands.SaveStudentLeaveRequest:
                    {
                        query = @"INSERT INTO STU_LEAVE_REQUEST
                                (
                                    STUDENT_ID,
                                    SUBJECT,
                                    REASON,
                                    DATE_FROM,
                                    DATE_TO,
                                    ACADEMIC_YEAR,
                                    STATUS
                                )
                                VALUES
                                (
                                    ?STUDENT_ID,
                                    ?SUBJECT,
                                    ?REASON,
                                    ?DATE_FROM,
                                    ?DATE_TO,
                                    ?AC_YEAR,
                                    ?STATUS
                                );";
                        break;
                    }

                case TaskManagementSQLCommands.UpdateStuLeaveRequest:
                    {
                        query = @"UPDATE stu_leave_request
                                   SET
                                    STUDENT_ID = ?STUDENT_ID,
                                    SUBJECT = ?SUBJECT,
                                    REASON = ?REASON,
                                    DATE_FROM = ?DATE_FROM,
                                    DATE_TO = ?DATE_TO
                                    WHERE LEAVE_REQUEST_ID = ?LEAVE_REQUEST_ID;";
                        break;
                    }

                case TaskManagementSQLCommands.IsStudentLeaveRequestExist:
                    {
                        query = @"SELECT 
                                    LEAVE_REQUEST_ID, 
                                    STUDENT_ID
                                FROM
                                    STU_LEAVE_REQUEST
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR AND STUDENT_ID = ?STUDENT_ID
                                        AND DATE_TO >= ?DATE_TO
                                        AND IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region Image
                case TaskManagementSQLCommands.FetchImagesByClassId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    C.CLASS_NAME,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    SP.ADMISSION_NO,
                                    DATE_FORMAT(sp.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    CONCAT('Shift - ', SS.SHIFT_NAME) AS SHIFT_NAME,
                                    P.PROGRAMME_NAME,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,''))) AS 'NAME',
                                     SP.PHOTO
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
		                                INNER JOIN
	                                CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
		                                INNER JOIN
	                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
		                                AND PG.IS_DELETED!=1
		                                INNER JOIN
	                                CP_PROGRAMME AS P ON P.PROGRAMME_ID=PG.PROGRAMME_ID
		                                AND P.IS_DELETED!=1
		                                INNER JOIN
	                                SUP_SHIFT AS SS ON SS.SHIFT_ID=PG.SHIFT
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1 AND SC.CLASS_ID=?CLASS_ID;";
                        break;
                    }
                #endregion

                #region Notification 
                case TaskManagementSQLCommands.SaveNotification:
                    {
                        query = @"INSERT INTO notifications_2019
                                    (NOTIFICATION_ROLE_TYPE_ID,
                                    ENTRY_ID,
                                    NOTIFICATION,
                                    USER_ID,
                                    CLASS_ID,
                                    PROGRAMME_GROUP_ID,
                                    PROGRAMME_YEAR,
                                    APP_TYPE_ID,
                                    NOTIFICATION_LEVEL_ID,
                                    NOTIFIED_DATE,
                                    ACADEMIC_YEAR,
                                    DEPARTMENT)
                                    VALUES
                                    (?NOTIFICATION_ROLE_TYPE_ID,
                                    ?ENTRY_ID,
                                    ?NOTIFICATION,
                                    ?USER_ID,
                                    ?CLASS_ID,
                                    ?PROGRAMME_GROUP_ID,
                                    ?PROGRAMME_YEAR,
                                    ?APP_TYPE_ID,
                                    ?NOTIFICATION_LEVEL_ID,
                                    now(),
                                    ?AC_YEAR,
                                    ?DEPARTMENT);";
                        break;
                    }

                #endregion
                #region Event Registration
                case TaskManagementSQLCommands.FetchEventRegistrationSetting:
                    {
                        query = @"SELECT 
                                EVENT_SETTING_ID,
                                MAIN_EVENT_NAME,
                                DATE_FORMAT(FROM_DATE,'%d/%m/%Y') AS FROM_DATE,
                                DATE_FORMAT(TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                ACADEMIC_YEAR,
                                IS_ACTIVE
                            FROM
                                EVENT_REGISTRATION_SETTINGS
                            WHERE
                                ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchEventRegistrationById:
                    {
                        query = @"SELECT 
                                EVENT_SETTING_ID,
                                MAIN_EVENT_NAME,
                                DATE_FORMAT(FROM_DATE,'%d/%m/%Y') AS FROM_DATE,
                                DATE_FORMAT(TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                ACADEMIC_YEAR,
                                IS_ACTIVE
                            FROM
                                EVENT_REGISTRATION_SETTINGS
                            WHERE
                                ACADEMIC_YEAR = ?AC_YEAR
                                    AND EVENT_SETTING_ID =?EVENT_SETTING_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateEventSetting:
                    {
                        query = @"UPDATE EVENT_REGISTRATION_SETTINGS 
                                SET 
                                    MAIN_EVENT_NAME =?MAIN_EVENT_NAME,
                                    FROM_DATE =?FROM_DATE,
                                    TO_DATE =?TO_DATE,
                                    ACADEMIC_YEAR =?ACADEMIC_YEAR
                                WHERE
                                    EVENT_SETTING_ID =?EVENT_SETTING_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveeventSetting:
                    {
                        query = @"INSERT INTO
                                 EVENT_REGISTRATION_SETTINGS(MAIN_EVENT_NAME, 
                                 FROM_DATE, TO_DATE, ACADEMIC_YEAR) 
                                 values(?MAIN_EVENT_NAME, ?FROM_DATE, 
                                 ?TO_DATE, ?ACADEMIC_YEAR);";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteEventSetting:
                    {
                        query = @"UPDATE EVENT_REGISTRATION_SETTINGS 
                            SET 
                              IS_DELETED=1
                            WHERE
                                EVENT_SETTING_ID =?EVENT_SETTING_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateEventSettingIsActive:
                    {
                        query = @"UPDATE EVENT_REGISTRATION_SETTINGS 
                                SET 
                                  IS_ACTIVE=?IS_ACTIVE
                                WHERE
                                    EVENT_SETTING_ID =?EVENT_SETTING_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchSubevents:
                    {
                        query = @"SELECT 
                                SUB_EVENT_ID,
                                SUB_EVENT_NAME,
                                MAIN_EVENT_ID,
                                ERS.MAIN_EVENT_NAME,
                                ALLOTED_NO
                            FROM
                                SUB_EVENTS AS SE
                                    INNER JOIN
                                EVENT_REGISTRATION_SETTINGS AS ERS ON ERS.EVENT_SETTING_ID = SE.MAIN_EVENT_ID
                            WHERE
                                SE.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveSubEvents:
                    {
                        query = @"INSERT INTO SUB_EVENTS( 
                                SUB_EVENT_NAME, MAIN_EVENT_ID,
                                ALLOTED_NO) VALUES(?SUB_EVENT_NAME, 
                                ?MAIN_EVENT_ID, ?ALLOTED_NO);";
                        break;
                    }
                case TaskManagementSQLCommands.FetchSubeventsById:
                    {
                        query = @"SELECT 
                                        SUB_EVENT_ID,
                                        SUB_EVENT_NAME,
                                        MAIN_EVENT_ID,
                                        ERS.MAIN_EVENT_NAME,
                                        ALLOTED_NO
                                    FROM
                                        SUB_EVENTS AS SE
                                            INNER JOIN
                                        EVENT_REGISTRATION_SETTINGS AS ERS ON ERS.EVENT_SETTING_ID = SE.MAIN_EVENT_ID
                                    WHERE
                                        SE.SUB_EVENT_ID = ?SUB_EVENT_ID
                                            AND SE.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateSubEvents:
                    {
                        query = @"UPDATE SUB_EVENTS 
                                    SET 
                                        SUB_EVENT_NAME =?SUB_EVENT_NAME,
                                        MAIN_EVENT_ID =?MAIN_EVENT_ID,
                                        ALLOTED_NO =?ALLOTED_NO
                                    WHERE
                                        SUB_EVENT_ID =?SUB_EVENT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchEventSetting:
                    {
                        query = @"SELECT 
                                    EVENT_SETTING_ID, MAIN_EVENT_NAME
                                FROM
                                    EVENT_REGISTRATION_SETTINGS
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR AND IS_ACTIVE != 0
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.LoadEvents:
                    {
                        query = @"SELECT 
                                    SE.SUB_EVENT_ID,
                                    SE.MAIN_EVENT_ID,
                                    SE.SUB_EVENT_NAME,
                                    ERS.MAIN_EVENT_NAME,
                                    SE.ALLOTED_NO
                                FROM
                                    SUB_EVENTS AS SE
                                        INNER JOIN
                                    EVENT_REGISTRATION_SETTINGS AS ERS ON ERS.EVENT_SETTING_ID = SE.MAIN_EVENT_ID
                                WHERE
                                    SE.MAIN_EVENT_ID = ?EVENT_SETTING_ID
                                        AND SE.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchSumOfSubEventQuotaBySubEventId:
                    {
                        query = @"SELECT 
                                    IFNULL(SE.ALLOTED_NO, '0') AS ALLOTED_SEATS,
                                    COUNT(SER.SUB_EVENT_ID) AS SUMOFQUTOA
                                FROM
                                    SUB_EVENTS AS SE
                                        LEFT JOIN
                                    STU_EVENT_REGISTRATION AS SER ON SER.SUB_EVENT_ID = SE.SUB_EVENT_ID
                                WHERE
                                    SE.SUB_EVENT_ID = ?SUB_EVENT_ID
                                        AND SE.IS_DELETED != 1;";
                        break;
                    }

                case TaskManagementSQLCommands.ValidateStudent:
                    {
                        query = @"SELECT 
                                GROUP_CONCAT(SE.SUB_EVENT_NAME) AS SUB_EVENT_NAME
                            FROM
                                STU_EVENT_REGISTRATION AS SER
                                    INNER JOIN
                                EVENT_REGISTRATION_SETTINGS AS ES ON SER.MAIN_EVENT_ID = ES.EVENT_SETTING_ID
                                    INNER JOIN
                                SUB_EVENTS AS SE ON SER.SUB_EVENT_ID = SE.SUB_EVENT_ID
                            WHERE
		                            SER.STUDENT_ID = ?STUDENT_ID
                                    AND SER.MAIN_EVENT_ID = ?MAIN_EVENT_ID
                                    AND SER.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SER.IS_DELETED != 1
                            GROUP BY SER.MAIN_EVENT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveStudentEventRegistration:
                    {
                        query = @"INSERT INTO 
                                STU_EVENT_REGISTRATION(MAIN_EVENT_ID,
                                 SUB_EVENT_ID, 
                                 STUDENT_ID, 
                                 ACADEMIC_YEAR) VALUES(?MAIN_EVENT_ID,
                                 ?SUB_EVENT_ID, ?STUDENT_ID,
                                 ?ACADEMIC_YEAR);";
                        break;
                    }
                case TaskManagementSQLCommands.FetchMeetingAssigned:
                    {
                        query = @"SELECT 
                                MEETING_ASSIGNED_ID,
                                MEETING_NAME,
                                DATE_FORMAT(DATE, '%Y/%m/%d') AS DATE,
                                VENUE,
                                TIME,
                                ASSIGNED_BY,
                                ACADEMIC_YEAR
                            FROM
                                MEETING_ASSIGNED
                            WHERE
                                ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1
                                    AND ASSIGNED_BY = ?ASSIGNED_BY;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchMeetingAssignedById:
                    {
                        query = @"SELECT 
                                    MEETING_ASSIGNED_ID,
                                    MEETING_NAME,
                                    DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE,
                                    VENUE,
                                    TIME,
                                    ASSIGNED_BY,
                                    ACADEMIC_YEAR
                                FROM
                                    MEETING_ASSIGNED
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1 
                                    AND MEETING_ASSIGNED_ID=?MEETING_ASSIGNED_ID
                                        AND ASSIGNED_BY = ?ASSIGNED_BY;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveMeetingAssigned:
                    {
                        query = @"INSERT INTO
                                 MEETING_ASSIGNED(MEETING_NAME, DATE, 
                                 VENUE, TIME, ASSIGNED_BY, 
                                 ACADEMIC_YEAR) 
                                 VALUES(?MEETING_NAME, ?DATE,
                                 ?VENUE, ?TIME, ?ASSIGNED_BY, 
                                 ?ACADEMIC_YEAR);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateMeetingAssigned:
                    {
                        query = @"UPDATE MEETING_ASSIGNED SET
                                  MEETING_NAME=?MEETING_NAME,
                                  DATE=?DATE, VENUE=?VENUE,
                                  TIME=?TIME, ASSIGNED_BY=?ASSIGNED_BY
                                  WHERE MEETING_ASSIGNED_ID=?MEETING_ASSIGNED_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveMeetingParticipants:
                    {
                        query = @"INSERT INTO MEETING_PARTICIPANTS(MEETING_ID,PARTICIPANT_ID) VALUES(?MEETING_ASSIGNED_ID,?PARTICIPANT_ID);";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteMeetingAssigned:
                    {
                        query = @"UPDATE  MEETING_ASSIGNED SET IS_DELETED=0 WHERE MEETING_ASSIGNED_ID=?MEETING_ASSIGNED_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateIsDeleted:
                    {
                        query = @"UPDATE MEETING_PARTICIPANTS SET IS_DELETED=0 WHERE MEETING_PARTICIPANT_ID=?MEETING_PARTICIPANT_ID;";
                        break;

                    }
                case TaskManagementSQLCommands.UpdateMeetingassignedIsDeleted:
                    {
                        query = @"UPDATE MEETING_PARTICIPANTS SET IS_DELETED=1 WHERE MEETING_ID=?MEETING_ASSIGNED_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchMeetingParticipants:
                    {
                        query = @"SELECT 
                                MEETING_ID, PARTICIPANT_ID
                            FROM
                                MEETING_PARTICIPANTS
                            WHERE
                                MEETING_ID = ?MEETING_ASSIGNED_ID AND IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.CheckIsParticipantExist:
                    {
                        query = @"SELECT 
                                    MEETING_PARTICIPANT_ID, MEETING_ID, PARTICIPANT_ID
                                FROM
                                    MEETING_PARTICIPANTS
                                WHERE
                                    MEETING_ID = ?MEETING_ASSIGNED_ID 
                                    AND PARTICIPANT_ID = ?PARTICIPANT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchMeetingforParticipant:
                    {
                        query = @"SELECT 
                                MA.MEETING_ASSIGNED_ID,
                                MA.MEETING_NAME,
                                DATE_FORMAT(MA.DATE, '%Y/%m/%d') AS DATE,
                                MA.VENUE,
                                MA.TIME,
                                SPI.FIRST_NAME AS ASSIGNED_BY
                            FROM
                                MEETING_ASSIGNED AS MA
                                    INNER JOIN
                                MEETING_PARTICIPANTS AS MP ON MA.MEETING_ASSIGNED_ID = MP.MEETING_ID
                                    INNER JOIN
                                stf_personal_info AS SPI ON SPI.STAFF_ID = MA.ASSIGNED_BY
                            WHERE
                                MA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND MP.PARTICIPANT_ID =?PARTICIPANT_ID
                                    AND MA.IS_DELETED != 1;";
                        break;
                    }
                #endregion
                #region Task management
                case TaskManagementSQLCommands.FetchTaskLists:
                    {
                        query = @"SELECT 
                                TASK_ID,
                                TASK_NAME,
                                DESCRIPTION,
                                DATE_FORMAT(TASK_DATE, '%Y/%m/%d') AS TASK_DATE,
                                DATE_FORMAT(SUB_DATE, '%Y/%m/%d') AS SUB_DATE,
                                ASSIGNED_BY,
                                ACADEMIC_YEAR
                            FROM
                                TASK_LIST
                            WHERE
                                IS_DELETED != 1 AND ACADEMIC_YEAR =?AC_YEAR
                                    AND ASSIGNED_BY =?ASSIGNED_BY;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskListById:
                    {
                        query = @"SELECT 
                                TASK_ID,
                                TASK_NAME,
                                DESCRIPTION,
                                DATE_FORMAT(TASK_DATE, '%Y/%m/%d') AS TASK_DATE,
                                DATE_FORMAT(SUB_DATE, '%d/%m/%Y') AS SUB_DATE,
                                ASSIGNED_BY,
                                ACADEMIC_YEAR
                            FROM
                                TASK_LIST
                            WHERE
                                IS_DELETED != 1 AND ACADEMIC_YEAR =?AC_YEAR
                                    AND TASK_ID =?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskAssignedlist:
                    {
                        query = @"SELECT 
                                    TASK_ASSIGNED_ID, TASK_ID, STAFF_ID
                                FROM
                                    TASK_ASSIGNED_LIST
                                WHERE
                                    IS_DELETED != 1 AND TASK_ID =?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveTaskList:
                    {
                        query = @"INSERT INTO TASK_LIST(TASK_NAME,
                                 DESCRIPTION,
                                 SUB_DATE, ASSIGNED_BY, 
                                 ACADEMIC_YEAR) 
                                VALUES(?TASK_NAME, ?DESCRIPTION,
                                 ?SUB_DATE, ?ASSIGNED_BY,
                                 ?ACADEMIC_YEAR);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateTaskList:
                    {
                        query = @"UPDATE TASK_LIST SET 
                                TASK_NAME=?TASK_NAME,
                                DESCRIPTION=?DESCRIPTION,
                                SUB_DATE=?SUB_DATE,
                                ACADEMIC_YEAR=?ACADEMIC_YEAR
                                 WHERE
                                 TASK_ID=?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveTaskAssigned:
                    {
                        query = @"INSERT INTO
                                TASK_ASSIGNED_LIST
                                (TASK_ID,STAFF_ID)
                                 VALUES(?TASK_ID,?STAFF_ID);";
                        break;

                    }
                case TaskManagementSQLCommands.UpdateTaskAssignedIsDeleted:
                    {
                        query = @"UPDATE TASK_ASSIGNED_LIST 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        TASK_ID =?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateIsDeletedInTaskAssigned:
                    {
                        query = @"UPDATE TASK_ASSIGNED_LIST 
                                    SET 
                                        IS_DELETED = 0
                                    WHERE
                                        TASK_ASSIGNED_ID =?TASK_ASSIGNED_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.CheckIsStaffExist:
                    {
                        query = @"SELECT 
                                TASK_ASSIGNED_ID, TASK_ID, STAFF_ID
                            FROM
                                TASK_ASSIGNED_LIST
                            WHERE
                                TASK_ID =?TASK_ID AND STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskSubmissionByTaskidandstaffId:
                    {
                        query = @"SELECT 
                                    TL.TASK_ID, TL.TASK_NAME, TSL.DESCRIPTION, TSL.TASK_SUB_ID
                                FROM
                                    TASK_LIST AS TL
                                        LEFT JOIN
                                    TASK_SUBMISSION_LIST AS TSL ON TL.TASK_ID = TSL.TASK_ID
                                        AND TSL.STAFF_ID =?STAFF_ID
                                WHERE
                                    TL.TASK_ID = ?TASK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchSubmissionTask:
                    {
                        query = @"SELECT 
                                TAL.TASK_ASSIGNED_ID,
                                TAL.TASK_ID,
                                TL.TASK_NAME,
                                TL.DESCRIPTION,
                                DATE_FORMAT(TL.SUB_DATE, '%Y/%m/%d') AS SUB_DATE,
                                TAL.STAFF_ID,
                                TST.TASK_SUB_ID
                            FROM
                                TASK_ASSIGNED_LIST AS TAL
                                    INNER JOIN
                                TASK_LIST AS TL ON TL.TASK_ID = TAL.TASK_ID
                                    LEFT JOIN
                                TASK_SUBMISSION_LIST AS TST ON TST.STAFF_ID = TAL.STAFF_ID
                                    AND TST.TASK_ID = TAL.TASK_ID
                            WHERE
                                TAL.STAFF_ID =?STAFF_ID AND TAL.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateTaskSubmission:
                    {
                        query = @"UPDATE TASK_SUBMISSION_LIST 
                            SET 
                                DESCRIPTION = ?DESCRIPTION,                              
                                FILE_PATH =?FILE_PATH
                            WHERE
                                TASK_SUB_ID = ?TASK_SUB_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveTaskSubmission:
                    {
                        query = @"INSERT INTO TASK_SUBMISSION_LIST
                                (TASK_ID, STAFF_ID, 
                                DESCRIPTION, FILE_PATH
                                ) 
                                VALUES(?TASK_ID, ?STAFF_ID, 
                                ?DESCRIPTION, ?FILE_PATH
                                );";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskSubmittedList:
                    {
                        query = @"SELECT 
                                TL.TASK_ID,
                                TL.TASK_NAME,
                                SP.FIRST_NAME,
                                DATE_FORMAT(TSL.SUB_DATE, '%Y/%m/%d') AS SUB_DATE,
                                SP.STAFF_CODE,
                                TSL.DESCRIPTION,
                                TSL.FILE_PATH
                            FROM
                                TASK_SUBMISSION_LIST AS TSL
                                    INNER JOIN
                                TASK_LIST AS TL ON TL.TASK_ID = TSL.TASK_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TSL.STAFF_ID
                            WHERE
                                TL.ASSIGNED_BY = ?ASSIGNED_BY
                                    AND TSL.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchTaskSubmittedListById:
                    {
                        query = @"SELECT 
                                TL.TASK_ID,
                                TL.TASK_NAME,
                                SP.FIRST_NAME,
                                DATE_FORMAT(TSL.SUB_DATE, '%Y/%m/%d') AS SUB_DATE,
                                SP.STAFF_CODE,
                                TSL.DESCRIPTION,
                                TSL.FILE_PATH
                            FROM
                                TASK_SUBMISSION_LIST AS TSL
                                    INNER JOIN
                                TASK_LIST AS TL ON TL.TASK_ID = TSL.TASK_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TSL.STAFF_ID
                            WHERE
                                TSL.TASK_SUB_ID = ?TASK_SUB_ID
                                    AND TSL.IS_DELETED != 1;";
                        break;
                    }

                #endregion
                #region Hallreservation
                case TaskManagementSQLCommands.FetchHalls:
                    {
                        query = @"SELECT 
                                SH.HALL_ID,
                                SH.HALL_NO,
                                SH.HALL_NAME,
                                SH.LOCATION,
                                SH.CAPACITY,
                                SH.IMAGE,
                                SH.STATUS,
                                SH.BLOCK_ID,
                                SCB.BLOCK_NAME
                            FROM
                                SUP_HALLS AS SH
                                    INNER JOIN
                                SUP_COLLEGE_BLOCK AS SCB ON SH.BLOCK_ID = SCB.COLLEGE_BLOCK_ID
                            WHERE
                                SH.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchHallsById:
                    {
                        query = @"SELECT 
                                    SH.HALL_ID,
                                    SH.HALL_NO,
                                    SH.HALL_NAME,
                                    SH.LOCATION,
                                    SH.CAPACITY,
                                    SH.IMAGE,
                                    SH.STATUS,
                                    SH.BLOCK_ID,
                                    SCB.BLOCK_NAME
                                FROM
                                    SUP_HALLS AS SH
                                        INNER JOIN
                                    SUP_COLLEGE_BLOCK AS SCB ON SH.BLOCK_ID = SCB.COLLEGE_BLOCK_ID
                                WHERE
                                    SH.HALL_ID = ?HALL_ID AND SH.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveHalls:
                    {
                        query = @"INSERT INTO SUP_HALLS(HALL_NO,
                                 HALL_NAME, 
                                 LOCATION, CAPACITY, IMAGE,
                                 BLOCK_ID) VALUES(?HALL_NO,
                                 ?HALL_NAME, ?LOCATION, 
                                 ?CAPACITY, ?IMAGE, 
                                 ?BLOCK_ID);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateHalls:
                    {
                        query = @"UPDATE SUP_HALLS 
                                SET 
                                    HALL_NO = ?HALL_NO,
                                    HALL_NAME =?HALL_NAME,
                                    LOCATION = ?LOCATION,
                                    CAPACITY = ?CAPACITY,
                                    IMAGE =?IMAGE,
                                    BLOCK_ID =?BLOCK_ID
                                WHERE
                                    HALL_ID = ?HALL_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateHallsWithNoFile:
                    {
                        query = @"UPDATE SUP_HALLS 
                                SET 
                                    HALL_NO = ?HALL_NO,
                                    HALL_NAME =?HALL_NAME,
                                    LOCATION = ?LOCATION,
                                    CAPACITY = ?CAPACITY,
                                    BLOCK_ID =?BLOCK_ID
                                WHERE
                                    HALL_ID = ?HALL_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteHalls:
                    {
                        query = @"UPDATE SUP_HALLS 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    HALL_ID =?HALL_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchHallReservationSetting:
                    {
                        query = @"SELECT 
                                    SETTING_ID,
                                    IS_AUTO_SCHEDULE,
                                    IS_AUTO_SMS_SEND
                                FROM
                                    HALL_RESERVATION_SETTING
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAlreadyBookedDetails:
                    {
                        query = @"SELECT 
                                HR.HALL_ID,
                                DATE_FORMAT(HR.FROM_DATE, '%Y-%m-%d') AS FROM_DATE,
                                DATE_FORMAT(HR.TO_DATE, '%Y-%m-%d') AS TO_DATE,
                                HR.FROM_TIME,
                                HR.TO_TIME,
                                SPI.FIRST_NAME AS USER_NAME
                            FROM
                                HALL_RESERVATION AS HR
                                    INNER JOIN
                                SUP_HALLS AS SH ON HR.HALL_ID = SH.HALL_ID
                                    INNER JOIN
                                stf_personal_info AS SPI ON SPI.STAFF_ID = HR.USER_ID
                            WHERE
                                
                                   (TIME_FORMAT(FROM_TIME, '%h:%i %p') BETWEEN TIME_FORMAT(?FROM_TIME, '%h:%i %p') AND TIME_FORMAT(?TO_TIME, '%h:%i %p')
                                    OR TIME_FORMAT(TO_TIME, '%h:%i %p') BETWEEN TIME_FORMAT(?FROM_TIME, '%h:%i %p') AND TIME_FORMAT(?TO_TIME, '%h:%i %p')) AND (FROM_DATE BETWEEN ?FROM_DATE AND ?TO_DATE
                                    OR TO_DATE BETWEEN ?FROM_DATE AND ?TO_DATE);";
                        break;
                    }
                case TaskManagementSQLCommands.FetchBookedHallsByUserId:
                    {
                        query = @"SELECT 
                                    HR.HALL_BOOK_ID,
                                    HR.HALL_ID,
                                    SH.HALL_NAME,
                                    DATE_FORMAT(HR.FROM_DATE, '%Y-%m-%d') AS FROM_DATE,
                                    DATE_FORMAT(HR.TO_DATE, '%Y-%m-%d') AS TO_DATE,
                                    HR.FROM_TIME,
                                    HR.TO_TIME,
                                    SPI.FIRST_NAME AS USER_NAME,
                                    DATE_FORMAT(HR.APPLY_DATE, '%Y-%m-%d') AS APPLY_DATE,
                                    DATE_FORMAT(HR.APPROVAL_DATE, '%Y-%m-%d') AS APPROVAL_DATE,
                                    HR.PURPOSE,
                                    CB.BLOCK_NAME,
                                    HR.STATUS,
                                    SS.STATUS_NAME
                                FROM
                                    HALL_RESERVATION AS HR
                                        INNER JOIN
                                    SUP_HALLS AS SH ON HR.HALL_ID = SH.HALL_ID
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = HR.USER_ID
                                        INNER JOIN
                                    SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = SH.BLOCK_ID
                                        LEFT JOIN
                                    SUP_HALL_RESERVATION_STATUS AS SS ON SS.STATUS_ID = HR.STATUS
                                WHERE
                                    HR.USER_ID = ?USER_ID
                                        AND HR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND HR.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchBookedHallByDate:
                    {
                        query = @"SELECT 
                                HR.HALL_BOOK_ID,
                                HR.HALL_ID,
                                SH.HALL_NAME,
                                DATE_FORMAT(HR.FROM_DATE, '%Y-%m-%d') AS FROM_DATE,
                                DATE_FORMAT(HR.TO_DATE, '%Y-%m-%d') AS TO_DATE,
                                HR.FROM_TIME,
                                HR.TO_TIME,
                                SPI.FIRST_NAME AS USER_NAME,
                                DATE_FORMAT(HR.APPLY_DATE, '%Y-%m-%d') AS APPLY_DATE,
                                DATE_FORMAT(HR.APPROVAL_DATE, '%Y-%m-%d') AS APPROVAL_DATE,
                                HR.PURPOSE,
                                CB.BLOCK_NAME,
                                SS.STATUS_NAME,
                                HR.STATUS
                            FROM
                                HALL_RESERVATION AS HR
                                    INNER JOIN
                                SUP_HALLS AS SH ON HR.HALL_ID = SH.HALL_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = HR.USER_ID
                                    INNER JOIN
                                SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = SH.BLOCK_ID
                                    LEFT JOIN
                                SUP_HALL_RESERVATION_STATUS AS SS ON SS.STATUS_ID = HR.STATUS
                            WHERE
                                HR.IS_DELETED != 1
                                    AND HR.FROM_DATE BETWEEN ?FROM_DATE AND ?TO_DATE
                                    OR HR.TO_DATE BETWEEN ?TO_DATE AND ?TO_DATE;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAllHallReservation:
                    {
                        query = @"SELECT 
                                HR.HALL_BOOK_ID,
                                HR.HALL_ID,
                                SH.HALL_NAME,
                                DATE_FORMAT(HR.FROM_DATE, '%Y-%m-%d') AS FROM_DATE,
                                DATE_FORMAT(HR.TO_DATE, '%Y-%m-%d') AS TO_DATE,
                                HR.FROM_TIME,
                                HR.TO_TIME,
                                SPI.FIRST_NAME AS USER_NAME,
                                DATE_FORMAT(HR.APPLY_DATE, '%Y-%m-%d') AS APPLY_DATE,
                                DATE_FORMAT(HR.APPROVAL_DATE, '%Y-%m-%d') AS APPROVAL_DATE,
                                HR.PURPOSE,
                                CB.BLOCK_NAME,
                                SS.STATUS_NAME,
                                HR.STATUS
                            FROM
                                HALL_RESERVATION AS HR
                                    INNER JOIN
                                SUP_HALLS AS SH ON HR.HALL_ID = SH.HALL_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = HR.USER_ID
                                    INNER JOIN
                                SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = SH.BLOCK_ID
                                    LEFT JOIN
                                SUP_HALL_RESERVATION_STATUS AS SS ON SS.STATUS_ID = HR.STATUS
                            WHERE
                                HR.IS_DELETED != 1
                            ORDER BY HR.FROM_DATE , HR.STATUS;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateHallreservationStatus:
                    {
                        query = @"UPDATE HALL_RESERVATION 
                                SET 
                                    STATUS = ?STATUS
                                WHERE
                                    HALL_BOOK_ID = ?HALL_BOOK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchHallReservationById:
                    {
                        query = @"SELECT 
                                HR.HALL_BOOK_ID,
                                HR.HALL_ID,
                                SH.HALL_NAME,
                                DATE_FORMAT(HR.FROM_DATE, '%Y-%m-%d') AS FROM_DATE,
                                DATE_FORMAT(HR.TO_DATE, '%Y-%m-%d') AS TO_DATE,
                                HR.FROM_TIME,
                                HR.TO_TIME,
                                SPI.FIRST_NAME AS USER_NAME,
                                DATE_FORMAT(HR.APPLY_DATE, '%Y-%m-%d') AS APPLY_DATE,
                                DATE_FORMAT(HR.APPROVAL_DATE, '%Y-%m-%d') AS APPROVAL_DATE,
                                HR.PURPOSE,
                                CB.BLOCK_NAME,
                                SS.STATUS_NAME,
                                HR.STATUS
                            FROM
                                HALL_RESERVATION AS HR
                                    INNER JOIN
                                SUP_HALLS AS SH ON HR.HALL_ID = SH.HALL_ID
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = HR.USER_ID
                                    INNER JOIN
                                SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = SH.BLOCK_ID
                                    LEFT JOIN
                                SUP_HALL_RESERVATION_STATUS AS SS ON SS.STATUS_ID = HR.STATUS
                            WHERE
                                HR.IS_DELETED != 1 AND HR.HALL_BOOK_ID=?HALL_BOOK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateHallReservation:
                    {
                        query = @"UPDATE HALL_RESERVATION 
                                    SET 
                                        HALL_ID = ?HALL_ID,
                                        FROM_DATE = ?FROM_DATE,
                                        TO_DATE = ?TO_DATE,
                                        FROM_TIME =?FROM_TIME,
                                        TO_TIME = ?TO_TIME,
                                        APPROVAL_DATE =?APPROVAL_DATE,
                                        PURPOSE =?PURPOSE
                                    WHERE
                                        HALL_BOOK_ID =?HALL_BOOK_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveHallReservation:
                    {
                        query = @"INSERT INTO 
                                HALL_RESERVATION(HALL_ID, FROM_DATE, 
                                TO_DATE, FROM_TIME, TO_TIME,
                                 APPROVAL_DATE, USER_ID,
                                 PURPOSE, ACADEMIC_YEAR,
                                 STATUS) 
                                VALUES(?HALL_ID, ?FROM_DATE, 
                                ?TO_DATE, ?FROM_TIME, ?TO_TIME,
                                 ?APPROVAL_DATE, ?USER_ID,
                                 ?PURPOSE, ?ACADEMIC_YEAR,
                                 ?STATUS);";
                        break;
                    }

                #endregion
                #region  Visitor Meeting and Appointment
                case TaskManagementSQLCommands.FetchVisitorsMeetingAssignment:
                    {
                        query = @"SELECT 
                        VMA.VISITORS_MEET_ID,
                        VMA.VISITORS_NAME,
                        VMA.MOBILE,
                        DATE_FORMAT(VMA.VISIT_DATE, '%Y/%m/%d') AS VISIT_DATE,
                        SC.VISITORS_CATEGORY_NAME,
                        VMA.PURPOSE,
                        VMA.STAFF_ID,
                        SP.FIRST_NAME,
                        VMA.IS_REQUIRED_APPOINMENT
                    FROM
                        VISITORS_MEETING_ASSIGNMENT AS VMA
                            INNER JOIN
                        SUP_VISITORS_CATEGORY AS SC ON VMA.VISITORS_CATEGORY = SC.VISITORS_CATEGORY_ID
                            INNER JOIN
                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = VMA.STAFF_ID
                    WHERE
                        VMA.IS_DELETED != 1;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchVisitorsMeetingAppointment:
                    {
                        query = @"SELECT 
                                VMA.VISITORS_MEET_ID,
                                VMA.VISITORS_NAME,
                                SC.VISITORS_CATEGORY_NAME,
                                DATE_FORMAT(VMA.VISIT_DATE, '%Y/%m/%d') AS VISIT_DATE,
                                VMA.MOBILE,
                                VMA.PURPOSE,
                                VA.VISITORS_APPOINTMENT_ID,
                                VA.REPLY,
                                VA.STATUS,
                                IF(STATUS = 1,
                                    'Approved',
                                    IF(STATUS = 2, 'Rejected', 'Pendding')) AS STATUS_NAME
                            FROM
                                VISITORS_MEETING_ASSIGNMENT AS VMA
                                    LEFT JOIN
                                VISITORS_MEETING_APPOINTMENT AS VA ON VMA.VISITORS_MEET_ID = VA.VISITORS_MEET_ID
                                    INNER JOIN
                                SUP_VISITORS_CATEGORY AS SC ON SC.VISITORS_CATEGORY_ID = VMA.VISITORS_CATEGORY
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = VMA.STAFF_ID
                            WHERE
                                VMA.IS_DELETED != 1
                                    AND VMA.STAFF_ID = ?STAFF_ID
                                    AND VMA.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchVisitorMeetingAssignById:
                    {
                        query = @"SELECT 
                                    VMA.VISITORS_MEET_ID,
                                    VMA.VISITORS_NAME,
                                    VMA.MOBILE,
                                    DATE_FORMAT(VMA.VISIT_DATE, '%d/%m/%Y') AS VISIT_DATE,
                                    VMA.VISITORS_CATEGORY,
                                    SC.VISITORS_CATEGORY_NAME,
                                    VMA.PURPOSE,
                                    VMA.STAFF_ID,
                                    SP.FIRST_NAME,
                                    VMA.IS_REQUIRED_APPOINMENT
                                FROM
                                    VISITORS_MEETING_ASSIGNMENT AS VMA
                                        INNER JOIN
                                    SUP_VISITORS_CATEGORY AS SC ON VMA.VISITORS_CATEGORY = SC.VISITORS_CATEGORY_ID
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = VMA.STAFF_ID
                                WHERE
                                    VMA.IS_DELETED != 1
                                        AND VMA.VISITORS_MEET_ID = ?VISITORS_MEET_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveVisitorMeetingAssign:
                    {
                        query = @"INSERT INTO 
                                VISITORS_MEETING_ASSIGNMENT(VISITORS_NAME, 
                                MOBILE, VISIT_DATE, VISITORS_CATEGORY,
                                 PURPOSE, STAFF_ID, IS_REQUIRED_APPOINMENT,
                                 ACADEMIC_YEAR) VALUES(?VISITORS_NAME, 
                                ?MOBILE, ?VISIT_DATE, ?VISITORS_CATEGORY,
                                 ?PURPOSE, ?STAFF_ID, ?IS_REQUIRED_APPOINMENT,
                                 ?ACADEMIC_YEAR);";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateVisitorMeetingAssign:
                    {
                        query = @"UPDATE VISITORS_MEETING_ASSIGNMENT 
                                SET 
                                    VISITORS_NAME =?VISITORS_NAME,
                                    MOBILE =?MOBILE,
                                    VISIT_DATE =?VISIT_DATE,
                                    VISITORS_CATEGORY =?VISITORS_CATEGORY,
                                    PURPOSE =?PURPOSE,
                                    STAFF_ID =?STAFF_ID,
                                    IS_REQUIRED_APPOINMENT =?IS_REQUIRED_APPOINMENT
                                WHERE
                                    VISITORS_MEET_ID =?VISITORS_MEET_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.DeleteVisitorMeetAssign:
                    {
                        query = @"UPDATE VISITORS_MEETING_ASSIGNMENT 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        VISITORS_MEET_ID = ?VISITORS_MEET_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveVisitorCategory:
                    {
                        query = @"INSERT INTO SUP_VISITORS_CATEGORY(VISITORS_CATEGORY_NAME) VALUES(?VISITORS_CATEGORY_NAME);";
                        break;
                    }
                case TaskManagementSQLCommands.FetchVisitorMeetingAppointmentById:
                    {
                        query = @"SELECT 
                                VMA.VISITORS_MEET_ID,
                                VMA.VISITORS_NAME,
                                VA.VISITORS_APPOINTMENT_ID,
                                DATE_FORMAT(VA.APPOINTMENT_DATE, '%d/%m/%Y') AS APPOINTMENT_DATE,
                                VA.APPOINTMENT_TIME,
                                VMA.MOBILE,
                                VA.REPLY,
                                VA.STATUS
                            FROM
                                VISITORS_MEETING_ASSIGNMENT AS VMA
                                    LEFT JOIN
                                VISITORS_MEETING_APPOINTMENT AS VA ON VMA.VISITORS_MEET_ID = VA.VISITORS_MEET_ID
                            WHERE
                                VMA.IS_DELETED != 1
                                    AND VMA.VISITORS_MEET_ID = ?VISITORS_MEET_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.UpdateVisitorMeetingAppointment:
                    {
                        query = @"UPDATE VISITORS_MEETING_APPOINTMENT 
                                    SET 
                                        APPOINTMENT_DATE =?APPOINTMENT_DATE,
                                        APPOINTMENT_TIME =?APPOINTMENT_TIME,
                                        REPLY = ?REPLY,
                                        STATUS =?STATUS
                                    WHERE
                                        VISITORS_APPOINTMENT_ID = ?VISITORS_APPOINTMENT_ID;";
                        break;
                    }
                case TaskManagementSQLCommands.SaveVisitorMeetingAppointment:
                    {
                        query = @"INSERT INTO 
                                VISITORS_MEETING_APPOINTMENT(VISITORS_MEET_ID,
                                 APPOINTMENT_DATE, 
                                APPOINTMENT_TIME, REPLY, STATUS) values(?VISITORS_MEET_ID,
                                 ?APPOINTMENT_DATE, 
                                ?APPOINTMENT_TIME, ?REPLY, ?STATUS);";
                        break;
                    }
                case TaskManagementSQLCommands.FetchAllVisitorsMeetingAppointment:
                    {
                        query = @"SELECT 
                                VMA.VISITORS_MEET_ID,
                                VMA.VISITORS_NAME,
                                VMA.MOBILE,
                                DATE_FORMAT(VA.APPOINTMENT_DATE,'%Y/%m/%d') AS APPOINTMENT_DATE,
                                VA.APPOINTMENT_TIME,
                                VA.REPLY,
                                VA.STATUS,
                                SS.STATUS_NAME
                            FROM
                                VISITORS_MEETING_ASSIGNMENT AS VMA
                                    INNER JOIN
                                VISITORS_MEETING_APPOINTMENT AS VA ON VMA.VISITORS_MEET_ID = VA.VISITORS_MEET_ID
                                    INNER JOIN
                                SUP_VISITORS_CATEGORY AS SC ON SC.VISITORS_CATEGORY_ID = VMA.VISITORS_CATEGORY
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = VMA.STAFF_ID
                                    INNER JOIN
                                sup_visitors_status AS SS ON SS.STATUS_ID = VA.STATUS
                            WHERE
                                VMA.IS_DELETED != 1
                                    AND VMA.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TaskManagementSQLCommands.FetchVisitormeetandappointmentByDate:
                    {
                        query = @"SELECT 
                                VMA.VISITORS_MEET_ID,
                                VMA.VISITORS_NAME,
                                SC.VISITORS_CATEGORY_NAME,
                                VMA.MOBILE,
                                DATE_FORMAT(VMA.VISIT_DATE, '%Y/%m/%d') AS VISIT_DATE,
                                DATE_FORMAT(VA.APPOINTMENT_DATE, '%Y/%m/%d') AS APPOINTMENT_DATE,
                                VA.APPOINTMENT_TIME,
                                VA.REPLY,
                                VA.STATUS,
                                SS.STATUS_NAME
                            FROM
                                VISITORS_MEETING_ASSIGNMENT AS VMA
                                    LEFT JOIN
                                VISITORS_MEETING_APPOINTMENT AS VA ON VMA.VISITORS_MEET_ID = VA.VISITORS_MEET_ID
                                    INNER JOIN
                                SUP_VISITORS_CATEGORY AS SC ON SC.VISITORS_CATEGORY_ID = VMA.VISITORS_CATEGORY
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = VMA.STAFF_ID
                                    LEFT JOIN
                                SUP_VISITORS_STATUS AS SS ON SS.STATUS_ID = VA.STATUS
                            WHERE
                                VMA.IS_DELETED != 1
                                    AND VMA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND VMA.VISIT_DATE BETWEEN ?FROM_DATE AND ?TO_DATE;";
                        break;
                    }
                    #endregion
            }
            return query;
        }
    }
}
