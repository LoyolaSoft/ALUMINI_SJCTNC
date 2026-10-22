using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Attendance
{
    public static class AttendanceSQL
    {
        public static string GetAttendanceSQL(AttendanceSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {

                case AttendanceSQLCommands.FetchAbsenteesListByCurrentDateAndProgrammeGroupIds:
                    {

                        query = @"SELECT 
                                        date_format( ATTENDENCE_DATE,'%d-%m-%Y') as ATTENDENCE_DATE,
                                        GROUP_CONCAT(HOUR_FROM) AS HOUR_FROM,
                                        AB.STUDENT_ID,
                                        SP.FIRST_NAME,
                                        CL.CLASS_NAME,
                                        CONCAT(IFNULL(SP.FR_MOBILE,
                                                        IFNULL(SP.MO_MOBILE,
                                                                IFNULL(SP.STU_MOBILENO, '')))) AS MOBILE,
                                        IF(SP.GENDER = 2, 'DAUGHTER', 'SON') AS GENDER
                                    FROM
                                        ATT_ABSENTEE_LIST AS AB
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = AB.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_LEFT !=1 AND SP.IS_DELETED !=1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
	                                    CP_CLASSES AS CL ON CL.CLASS_ID=SC.CLASS_ID
                                    WHERE
                                        ATTENDENCE_DATE = DATE_FORMAT(NOW(),'%Y-%m-%d')
                                            AND AB.ATTENDANCE_TYPE_ID=2
                                            AND CP.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND AB.HOUR_FROM IN (?HOUR_FROM)
                                    GROUP BY AB.STUDENT_ID
                                    ORDER BY HOUR_FROM , AB.STUDENT_ID ASC; ";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentListForAttendanceByClassId:
                    {

                        query = @"SELECT 
                                SP.ROLL_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                ABS.ATTENDANCE_ID,
                                ABS.REASON_ID,
                                ABS.ATTENDANCE_TYPE_ID,
                                ABS.ENTRY_ID,
                                ABS.ENTRY_DATE,
                                ATT.ATTENDANCE_TYPE_ID
                            FROM
                                stu_class AS scls
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCLS.STUDENT_ID ON SP.IS_DELETED !=1 AND SP.IS_LEFT !=1
                                    LEFT JOIN
                                att_absentee_list_?AC_YEAR AS ABS ON ABS.ATTENDENCE_DATE = CURRENT_DATE()
                                    AND scls.STUDENT_ID = ABS.STUDENT_ID
                                    LEFT JOIN
                                sup_reason AS sr ON sr.REASON_ID = ABS.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = abs.ATTENDANCE_TYPE_ID
                            WHERE
                                scls.CLASS_ID = ?CLASS_ID
                                    AND scls.ACADEMIC_YEAR = ?AC_YEAR ;";
                        break;
                    }
                case AttendanceSQLCommands.SaveAbsenteeEntryByCourseStaff:
                    {
                        query = @"INSERT INTO ATT_ABSENTEE_LIST
                                        (
                                        ATTENDENCE_DATE,
                                        STAFF_ID,
                                        COURSE_ID,
                                        HOUR_FROM,
                                        HOUR_TO,
                                        STUDENT_ID,
                                        CLASS_ID,
                                        REASON_ID,
                                        ENTRY_ID,
                                        ENTRY_DATE,
                                        S_CLASS_ID,
                                        ATTENDANCE_TYPE_ID,
                                        ACADEMIC_YEAR
                                        )
                                        VALUES
                                        (?ATTENDENCE_DATE,
                                        ?STAFF_ID,
                                        ?COURSE_ID,
                                        ?HOUR_FROM,
                                        ?HOUR_TO,
                                        ?STUDENT_ID,
                                        ?CLASS_ID,
                                        ?REASON_ID,
                                        ?ENTRY_ID,
                                        CURDATE(),
                                        ?S_CLASS_ID,
                                        ?ATTENDANCE_TYPE_ID,
                                        ?AC_YEAR);";
                        return query;
                    }
                case AttendanceSQLCommands.UpdateAbsenteeEntryByCourseStaff:
                    {
                        query = @"UPDATE ATT_ABSENTEE_LIST
                                    SET
                                    ATTENDENCE_DATE = ?ATTENDENCE_DATE,
                                    STAFF_ID = ?STAFF_ID,
                                    COURSE_ID = ?COURSE_ID,
                                    HOUR_FROM = ?HOUR_FROM,
                                    HOUR_TO = ?HOUR_TO,
                                    STUDENT_ID = ?STUDENT_ID,
                                    CLASS_ID = ?CLASS_ID,
                                    REASON_ID = ?REASON_ID,
                                    ENTRY_ID = ?ENTRY_ID,
                                    ENTRY_DATE = CURDATE(),
                                    S_CLASS_ID = ?S_CLASS_ID,
                                    ATTENDANCE_TYPE_ID = ?ATTENDANCE_TYPE_ID
                                    WHERE ATTENDANCE_ID = ?ATTENDANCE_ID;";
                        return query;
                    }
                case AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffId:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                CR.COURSE_CODE,
                                TT.COURSE_ID,
                                CR.COURSE_TITLE,
                                CLS.CLASS_ID AS S_CLASS_ID,
                                CLS.CLASS_ID,
                                ABS.ATTENDANCE_ID,
                                ABS.REASON_ID,
                                ABS.ATTENDANCE_TYPE_ID,
                                ABS.ENTRY_ID,
                                ABS.ENTRY_DATE,
                                SR.REASON_ID,
                                ATT.ATTENDANCE_TYPE_ID,SP.STUDENT_ID,
                                C.CLASS_NAME
                            FROM
                                CALENDER AS CAL
                                    INNER JOIN
                                TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER 
                                    AND CAL.ACADEMIC_YEAR= ?AC_YEAR 
                                    AND TT.ACADEMIC_YEAR= ?AC_YEAR 
                                    AND TT.CLASS_ID IN (?CLASS_ID)
                                    AND TT.STAFF_ID = ?STAFF_ID
                                    AND TT.HOUR_ID = ?HOUR_ID
                                    AND TT.IS_DELETED!=1
                                    INNER JOIN
                                STU_CLASS AS CLS ON CLS.CLASS_ID IN (?CLASS_ID) AND CLS.ACADEMIC_YEAR= ?AC_YEAR AND CLS.IS_DELETED!=1
                                    INNER JOIN
								CP_CLASSES AS C ON C.CLASS_ID=CLS.CLASS_ID AND C.IS_DELETED!=1
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT !=1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID AND CR.IS_DELETED!=1
                                    LEFT JOIN
                                ATT_ABSENTEE_LIST AS ABS ON CAL.DAY_ORDER_DATE = ABS.ATTENDENCE_DATE AND ABS.ACADEMIC_YEAR=2018
                                    AND ABS.STUDENT_ID = CLS.STUDENT_ID AND ABS.HOUR_FROM = ?HOUR_ID
                                    LEFT JOIN
                                SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                            WHERE
                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                    AND CAL.IS_DELETED != 1
                                    AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                            GROUP BY SP.STUDENT_ID
                            ORDER BY SP.ROLL_NO , FIRST_NAME;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentListForAttendanceByOptionalCourseIdAndStaffId:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                CR.COURSE_CODE,
                                TT.COURSE_ID,
                                CR.COURSE_TITLE,
                                CLS.S_CLASS_ID,
                                CLS.CLASS_ID,
                                ABS.ATTENDANCE_ID,
                                ABS.REASON_ID,
                                ABS.ATTENDANCE_TYPE_ID,
                                ABS.ENTRY_ID,
                                ABS.ENTRY_DATE,
                                SR.REASON_ID,
                                ATT.ATTENDANCE_TYPE_ID,SP.STUDENT_ID,
                                C.CLASS_NAME
                            FROM
                                CALENDER AS CAL
                                    INNER JOIN
                                TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER AND CAL.ACADEMIC_YEAR=?AC_YEAR 
                                    AND TT.ACADEMIC_YEAR=?AC_YEAR
                                    AND TT.CLASS_ID IN (?CLASS_ID)
                                    AND TT.STAFF_ID = ?STAFF_ID
                                    AND TT.HOUR_ID = ?HOUR_ID AND TT.ACADEMIC_YEAR=?AC_YEAR
                                    AND TT.IS_DELETED!=1
                                    INNER JOIN
                                STU_COURSE_INFO AS CLS ON CLS.S_CLASS_ID IN (?CLASS_ID) 
                                    AND CLS.COURSE_ID=TT.COURSE_ID 
                                    AND CLS.ACADEMIC_YEAR=?AC_YEAR 
                                    AND CLS.IS_DELETED!=1
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID=CLS.STUDENT_ID 
                                    AND SC.ACADEMIC_YEAR=?AC_YEAR 
                                    AND SC.IS_DELETED!=1
                                    INNER JOIN
								CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID AND C.IS_DELETED!=1
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT !=1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CLS.COURSE_ID AND CR.IS_DELETED!=1
                                    LEFT JOIN
                                ATT_ABSENTEE_LIST AS ABS ON CAL.DAY_ORDER_DATE = ABS.ATTENDENCE_DATE
                                    AND ABS.STUDENT_ID = CLS.STUDENT_ID AND ABS.ACADEMIC_YEAR=?AC_YEAR
                                    AND ABS.HOUR_FROM = ?HOUR_ID
                                    LEFT JOIN
                                SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                            WHERE
                                DAY_ORDER_DATE =?ATTENDANCE_DATE
                                    AND CAL.IS_DELETED != 1
                                    AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                            GROUP BY CLS.STUDENT_ID
                            ORDER BY SP.ROLL_NO , FIRST_NAME;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentListForOverAllAttendance:
                    {

                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                CR.COURSE_CODE,
                                TT.COURSE_ID,
                                CR.COURSE_TITLE,
                                CLS.CLASS_ID AS S_CLASS_ID,
                                CLS.CLASS_ID,
                                ABS.ATTENDANCE_ID,
                                ABS.REASON_ID,
                                ABS.ATTENDANCE_TYPE_ID,
                                ABS.ENTRY_ID,
                                ABS.ENTRY_DATE,
                                SR.REASON_ID,
                                ATT.ATTENDANCE_TYPE_ID,SP.STUDENT_ID,
                                C.CLASS_NAME
                            FROM
                                CALENDER AS CAL
                                    INNER JOIN
                                TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER AND CAL.ACADEMIC_YEAR=?AC_YEAR AND TT.ACADEMIC_YEAR=?AC_YEAR                    
                                    AND TT.STAFF_ID = ?STAFF_ID
                                    AND TT.HOUR_ID = ?HOUR_ID
                                    AND TT.IS_DELETED!=1
                                    INNER JOIN
                                stu_class AS CLS ON CLS.CLASS_ID = tt.class_id AND CLS.ACADEMIC_YEAR=?AC_YEAR AND CLS.IS_DELETED!=1
                                     INNER JOIN
								CP_CLASSES AS C ON C.CLASS_ID=CLS.CLASS_ID  AND C.IS_DELETED!=1
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT !=1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                    LEFT JOIN
                                ATT_ABSENTEE_LIST AS ABS ON CAL.DAY_ORDER_DATE = ABS.ATTENDENCE_DATE
                                    AND ABS.STUDENT_ID = CLS.STUDENT_ID
                                    LEFT JOIN
                                SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                            WHERE
                                DAY_ORDER_DATE = ?ATTENDANCE_DATE   
                                    AND CAL.IS_DELETED != 1
                            GROUP BY SP.STUDENT_ID
                            ORDER BY SP.ROLL_NO , FIRST_NAME;";
                        break;
                    }
                case AttendanceSQLCommands.FetchOptionalCourseStudentListForOverAllAttendance:
                    {

                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        CLS.S_CLASS_ID,
                                        CLS.CLASS_ID,
                                        ABS.ATTENDANCE_ID,
                                        ABS.REASON_ID,
                                        ABS.ATTENDANCE_TYPE_ID,
                                        ABS.ENTRY_ID,
                                        ABS.ENTRY_DATE,
                                        SR.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,
                                        C.CLASS_NAME
                                    FROM
                                        CALENDER AS CAL
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER AND TT.STAFF_ID = ?STAFF_ID AND TT.ACADEMIC_YEAR=?AC_YEAR AND CAL.ACADEMIC_YEAR=?AC_YEAR
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            INNER JOIN
                                        STU_COURSE_INFO AS CLS ON CLS.S_CLASS_ID = TT.CLASS_ID AND CLS.ACADEMIC_YEAR=?AC_YEAR
                                            AND CLS.COURSE_ID = TT.COURSE_ID 
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID=CLS.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR AND SC.IS_DELETED!=1
                                             INNER JOIN
								        CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID AND C.IS_DELETED!=1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT !=1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID 
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ABS ON CAL.DAY_ORDER_DATE = ABS.ATTENDENCE_DATE
                                            AND ABS.STUDENT_ID = CLS.STUDENT_ID
                                            LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                                    WHERE
                                        DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                            AND CAL.IS_DELETED != 1
                                            AND TT.IS_DELETED != 1
                                            AND CLS.IS_DELETED != 1
                                            AND CR.IS_DELETED != 1
                                    GROUP BY SP.STUDENT_ID
                                    ORDER BY SP.ROLL_NO , FIRST_NAME;";
                        break;
                    }
                case AttendanceSQLCommands.FetchHoursByStaffIdAndDayOrder:
                    {
                        query = @"SELECT 
                                    HOUR_ID,COURSE_ID
                                FROM
                                    TT_TIMETABLE AS TT
                                WHERE
                                    STAFF_ID =?STAFF_ID AND DAY_ORDER_ID =(select DAY_ORDER from CALENDER where DAY_ORDER_DATE=CURDATE() AND ACADEMIC_YEAR=?AC_YEAR)  AND TT.ACADEMIC_YEAR=?AC_YEAR
                                       AND IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchAttendanceStatusByClassId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    ROUND((((SELECT 
                                                    (COUNT(DAY_ORDER_DATE) * (SELECT 
                                                                TTS.NO_OF_HOURS
                                                            FROM
                                                                TT_DEPARTMENT_WISE_TEMPLATE AS TTD
                                                                    INNER JOIN
                                                                TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TTD.SETTING_ID
                                                            WHERE
                                                                DEPARTMENT_ID = (SELECT 
                                                                        DEPARTMENT
                                                                    FROM
                                                                        CP_CLASSES
                                                                    WHERE
                                                                        CLASS_ID = ?CLASS_ID)
                                                                    AND TTD.ACADEMIC_YEAR = ?AC_YEAR
                                                                    AND TTD.IS_DELETED != 1)) AS TOTAL_HOURS
                                                FROM
                                                    CALENDER AS CAL
                                                WHERE
                                                    CAL.DAY_ORDER_DATE
                                                        AND CAL.ACADEMIC_YEAR = ?AC_YEAR BETWEEN (SELECT 
                                                            ASE.DATE_FROM
                                                        FROM
                                                            ACADEMIC_SEMESTER AS ASE
                                                        WHERE
                                                            ASE.IS_ACTIVE = 1
                                                                AND ASE.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                                AND ASE.BATCH = SP.BATCH
                                                                AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                                                                AND ASE.IS_DELETED != 1) AND CURDATE()
                                                        AND ((SELECT 
                                                            ASE.DATE_TO
                                                        FROM
                                                            ACADEMIC_SEMESTER AS ASE
                                                        WHERE
                                                            ASE.IS_ACTIVE = 1
                                                                AND ASE.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                                AND ASE.BATCH = SP.BATCH
                                                                AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                                                                AND ASE.IS_DELETED != 1)) <= CURDATE()
                                                        AND CAL.IS_HOLIDAY != 1) - (SELECT 
                                                    COUNT(ATT.ATTENDANCE_ID)
                                                FROM
                                                    ATT_ABSENTEE_LIST AS ATT
                                                WHERE
                                                    STUDENT_ID = SC.STUDENT_ID
                                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR)) / (SELECT 
                                                    (COUNT(DAY_ORDER_DATE) * (SELECT 
                                                                TTS.NO_OF_HOURS
                                                            FROM
                                                                TT_DEPARTMENT_WISE_TEMPLATE AS TTD
                                                                    INNER JOIN
                                                                TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TTD.SETTING_ID
                                                            WHERE
                                                                DEPARTMENT_ID = (SELECT 
                                                                        DEPARTMENT
                                                                    FROM
                                                                        CP_CLASSES
                                                                    WHERE
                                                                        CLASS_ID = ?CLASS_ID)
                                                                    AND TTD.ACADEMIC_YEAR = ?AC_YEAR
                                                                    AND TTD.IS_DELETED != 1)) AS TOTAL_HOURS
                                                FROM
                                                    CALENDER AS CAL
                                                WHERE
                                                    CAL.DAY_ORDER_DATE BETWEEN (SELECT 
                                                            ASE.DATE_FROM
                                                        FROM
                                                            ACADEMIC_SEMESTER AS ASE
                                                        WHERE
                                                            ASE.IS_ACTIVE = 1
                                                                AND ASE.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                                AND ASE.BATCH = SP.BATCH
                                                                AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                                                                AND ASE.IS_DELETED != 1) AND CURDATE()
                                                        AND ((SELECT 
                                                            ASE.DATE_TO
                                                        FROM
                                                            ACADEMIC_SEMESTER AS ASE
                                                        WHERE
                                                            ASE.IS_ACTIVE = 1
                                                                AND ASE.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                                AND ASE.BATCH = SP.BATCH
                                                                AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                                                                AND ASE.IS_DELETED != 1)) <= CURDATE()
                                                        AND CAL.IS_HOLIDAY != 1) * 100)) AS PERCENTAGE
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    SC.CLASS_ID = ?CLASS_ID AND SC.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                ORDER BY SP.STUDENT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchTotalHoursByClassIdAndCourseId:
                    {
                        query = @"SELECT 
                                        COUNT(TT.HOUR_ID) AS 'TOTAL_HOURS'
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE <=CURDATE()
                                            AND CAL.IS_HOLIDAY != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = TT.COURSE_ID
                                    WHERE
                                        TT.IS_DELETED != 1
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.CLASS_ID = ?CLASS_ID
                                            AND TT.COURSE_ID = ?COURSE_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND CCR.SEMESTER_ID IN (?SEMESTER_ID);";
                        //query = @"SELECT 
                        //                CCR.COURSE_ROOT_ID AS COURSE_ID,
                        //                (SELECT 
                        //                        COUNT(TT.COURSE_ID)
                        //                    FROM
                        //                        TT_TIMETABLE_?AC_YEAR AS TT
                        //                            INNER JOIN
                        //                        SUP_DAY_ORDER AS SDO ON SDO.DAY_ORDER_ID = TT.DAY_ORDER_ID
                        //                            INNER JOIN
                        //                        CALENDER_?AC_YEAR AS CAL ON CAL.DAY_ORDER = SDO.DAY_ORDER_ID
                        //                    WHERE
                        //                        TT.CLASS_ID = ?CLASS_ID AND TT.COURSE_ID = ?COURSE_ID
                        //                            AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                            AND ?DATE_TO <= CURDATE()
                        //                            AND TT.IS_DELETED != 1) AS TOTAL_HOURS
                        //            FROM
                        //                STU_CLASS AS SC
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                    AND SP.IS_LEFT != 1
                        //                    AND SP.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_CLASS_COURSE_?AC_YEAR AS CCC ON CCC.CLASS_ID = SC.CLASS_ID
                        //                    AND CCC.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT_?AC_YEAR AS CCR ON CCR.COURSE_ROOT_ID = CCC.COURSE_ID
                        //                    AND CCR.IS_DELETED != 1
                        //            WHERE
                        //                SC.CLASS_ID = ?CLASS_ID
                        //                    AND CCR.COURSE_ROOT_ID = ?COURSE_ID
                        //                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.IS_DELETED != 1
                        //            GROUP BY SC.CLASS_ID 
                        //            UNION SELECT 
                        //                CCR.COURSE_ROOT_ID AS COURSE_ID,
                        //                (SELECT 
                        //                        COUNT(TT.COURSE_ID)
                        //                    FROM
                        //                        TT_TIMETABLE_?AC_YEAR AS TT
                        //                            INNER JOIN
                        //                        SUP_DAY_ORDER AS SDO ON SDO.DAY_ORDER_ID = TT.DAY_ORDER_ID
                        //                            INNER JOIN
                        //                        CALENDER_?AC_YEAR AS CAL ON CAL.DAY_ORDER = SDO.DAY_ORDER_ID
                        //                    WHERE
                        //                        TT.CLASS_ID = ?CLASS_ID AND TT.COURSE_ID = ?COURSE_ID
                        //                            AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                            AND ?DATE_TO <= CURDATE()
                        //                            AND TT.IS_DELETED != 1) AS TOTAL_HOURS
                        //            FROM
                        //                STU_COURSE_INFO_?AC_YEAR AS SC
                        //                    INNER JOIN
                        //                STU_CLASS AS SCL ON SCL.STUDENT_ID = SC.STUDENT_ID
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCL.STUDENT_ID
                        //                    AND SP.IS_LEFT != 1
                        //                    AND SP.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT_?AC_YEAR AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
                        //                    AND CCR.IS_DELETED != 1
                        //            WHERE
                        //                SC.S_CLASS_ID = ?CLASS_ID
                        //                    AND CCR.COURSE_ROOT_ID = ?COURSE_ID
                        //                    AND SC.IS_DELETED != 1
                        //            GROUP BY SC.CLASS_ID;";

                        //query = @"SELECT 
                        //                CCP.COURSE_ID, COUNT(CCP.COURSE_ID) AS TOTAL_HOURS
                        //            FROM
                        //                CP_COURSE_PATTERN AS CCP
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                        //                    INNER JOIN
                        //                CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                        //                    INNER JOIN
                        //                ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER = CCR.SEMESTER_ID
                        //                    AND SEM.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                        //                    AND SEM.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                        //                    INNER JOIN
                        //                ACADEMIC_BATCHES AS B ON B.BATCH_YEAR = C.CLASS_YEAR
                        //                    AND B.ACADEMIC_YEAR_ID = (SELECT 
                        //                        ACADEMIC_YEAR_ID
                        //                    FROM
                        //                        ACADEMIC_YEAR AS AC
                        //                    WHERE
                        //                        AC.ACTIVE_YEAR = 1)
                        //                    AND SEM.BATCH = B.BATCH_ID
                        //                    INNER JOIN
                        //                TT_TIMETABLE AS TT ON TT.CLASS_ID = C.CLASS_ID
                        //                    AND TT.COURSE_ID = CCP.COURSE_ID
                        //                    AND TT.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                        //                    AND TT.SEMESTER_ID = SEM.SEMESTER
                        //                    AND TT.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                        //                    AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                    AND ?DATE_TO <= CURDATE()
                        //            WHERE
                        //                CCP.COURSE_ID = ?COURSE_ID AND C.CLASS_ID = ?CLASS_ID
                        //                    AND CCP.IS_DELETED != 1
                        //                    AND CCP.ACADEMIC_YEAR = ?AC_YEAR 
                        //                    HAVING COUNT(CCP.COURSE_ID)>=1
                        //            UNION SELECT 
                        //                TT.COURSE_ID, COUNT(TT.COURSE_ID) AS TOTAL_HOURS
                        //            FROM
                        //                TT_TIMETABLE AS TT
                        //                    INNER JOIN
                        //                CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                        //                    AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                    AND ?DATE_TO <= CURDATE()
                        //                    INNER JOIN
                        //                STU_COURSE_INFO AS SCI ON SCI.S_CLASS_ID = TT.CLASS_ID
                        //                    AND SCI.COURSE_ID = TT.COURSE_ID
                        //                    AND SCI.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                        //                    AND SCI.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = TT.COURSE_ID
                        //            WHERE
                        //                TT.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND CCR.SEMESTER_ID IN (?SEMESTER_ID)
                        //                    AND TT.CLASS_ID = ?CLASS_ID
                        //                    AND TT.COURSE_ID = ?COURSE_ID
                        //                    AND TT.IS_DELETED != 1 HAVING COUNT(TT.COURSE_ID)>=1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchTotalAbsentHoursByClassIdAndCourseId:
                    {
                        query = @"SELECT  
                                        COUNT(ATT.ATTENDANCE_ID) AS 'ABSENT_HOURS',
                                        ATT.STUDENT_ID
                                    FROM
                                        ATT_ABSENTEE_LIST AS ATT
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER_DATE = ATT.ATTENDENCE_DATE
                                            AND CAL.ACADEMIC_YEAR = ATT.ACADEMIC_YEAR
                                            AND CAL.IS_HOLIDAY != 1
                                            INNER JOIN
										CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=ATT.COURSE_ID
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER
                                            AND TT.CLASS_ID = ATT.S_CLASS_ID
                                            AND TT.STAFF_ID = ATT.STAFF_ID
                                            AND TT.COURSE_ID = ATT.COURSE_ID
                                            AND TT.ACADEMIC_YEAR = ATT.ACADEMIC_YEAR
                                            AND TT.HOUR_ID=ATT.HOUR_FROM
                                            AND TT.IS_DELETED != 1
                                    WHERE ATT.ACADEMIC_YEAR =?AC_YEAR
                                          AND ATT.IS_DELETED!=1 
                                          AND ATT.ATTENDANCE_TYPE_ID=2
                                          AND ATT.S_CLASS_ID=?CLASS_ID
                                          AND ATT.COURSE_ID=?COURSE_ID
                                          AND ATT.STAFF_ID=?STAFF_ID
                                          AND CCR.SEMESTER_ID IN (?SEMESTER_ID)
                                          GROUP BY ATT.STUDENT_ID;";
                        //query = @"SELECT 
                        //                SP.STUDENT_ID,
                        //                SP.REGISTER_NO,
                        //                CCR.COURSE_ROOT_ID AS COURSE_ID,
                        //                (SELECT 
                        //                                            COUNT(CAL.DAY_ORDER_DATE)
                        //                                        FROM
                        //                                            CALENDER_?AC_YEAR AS CAL
                        //                                                INNER JOIN
                        //                                            ATT_ABSENTEE_LIST_?AC_YEAR AS ATT ON ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                        //                                        WHERE
                        //                                            CAL.IS_HOLIDAY != 1
                        //                                                AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                                                AND ?DATE_TO <= CURDATE()
                        //                                                AND ATT.COURSE_ID = ?COURSE_ID
                        //                                                AND ATT.S_CLASS_ID = ?CLASS_ID
                        //                                                AND ATT.STUDENT_ID = SP.STUDENT_ID AND ATT.ATTENDANCE_TYPE_ID!=1) ABSENT_HOURS
                        //            FROM
                        //                STU_CLASS AS SC
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                    AND SP.IS_LEFT != 1
                        //                    AND SP.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_CLASS_COURSE_?AC_YEAR AS CCC ON CCC.CLASS_ID = SC.CLASS_ID
                        //                    AND CCC.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT_?AC_YEAR AS CCR ON CCR.COURSE_ROOT_ID = CCC.COURSE_ID
                        //                    AND CCR.IS_DELETED != 1
                        //            WHERE
                        //                SC.CLASS_ID = ?CLASS_ID
                        //                    AND CCR.COURSE_ROOT_ID = ?COURSE_ID
                        //                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.IS_DELETED != 1
                        //            UNION SELECT 
                        //                SP.STUDENT_ID,
                        //                SP.REGISTER_NO,
                        //                CCR.COURSE_ROOT_ID AS COURSE_ID,
                        //                (SELECT 
                        //                                            COUNT(CAL.DAY_ORDER_DATE)
                        //                                        FROM
                        //                                            CALENDER_?AC_YEAR AS CAL
                        //                                                INNER JOIN
                        //                                            ATT_ABSENTEE_LIST_?AC_YEAR AS ATT ON ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                        //                                        WHERE
                        //                                            CAL.IS_HOLIDAY != 1
                        //                                                AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                                                AND ?DATE_TO <= CURDATE()
                        //                                                AND ATT.COURSE_ID = ?COURSE_ID
                        //                                                AND ATT.S_CLASS_ID = ?CLASS_ID
                        //                                                AND ATT.STUDENT_ID = SP.STUDENT_ID AND ATT.ATTENDANCE_TYPE_ID!=1) AS ABSENT_HOURS
                        //            FROM
                        //                STU_COURSE_INFO_?AC_YEAR AS SC
                        //                    INNER JOIN
                        //                STU_CLASS AS SCL ON SCL.STUDENT_ID = SC.STUDENT_ID
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCL.STUDENT_ID
                        //                    AND SP.IS_LEFT != 1
                        //                    AND SP.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT_?AC_YEAR AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
                        //                    AND CCR.IS_DELETED != 1
                        //            WHERE
                        //                SC.S_CLASS_ID = ?CLASS_ID
                        //                    AND CCR.COURSE_ROOT_ID = ?COURSE_ID
                        //                    AND SC.IS_DELETED != 1
                        //            ORDER BY REGISTER_NO;";
                        //query = @"SELECT 
                        //                SP.STUDENT_ID,
                        //                SP.REGISTER_NO,
                        //                CCR.COURSE_ROOT_ID AS COURSE_ID,
                        //                COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_HOURS
                        //            FROM
                        //             STU_CLASS AS SC
                        //              INNER JOIN
                        //             STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
                        //              AND SP.IS_LEFT!=1 AND SP.IS_DELETED!=1
                        //                    INNER JOIN
                        //                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID 
                        //              INNER JOIN
                        //                CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID=C.PROGRAMME_GROUP_ID		
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                        //                    INNER JOIN
                        //                ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER = CCR.SEMESTER_ID
                        //                    AND SEM.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                        //                    AND SEM.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                        //                    AND SEM.SEMESTER=CCR.SEMESTER_ID
                        //                    INNER JOIN
                        //                ACADEMIC_BATCHES AS B ON B.BATCH_YEAR = C.CLASS_YEAR
                        //                    AND B.ACADEMIC_YEAR_ID = (SELECT 
                        //                        ACADEMIC_YEAR_ID
                        //                    FROM
                        //                        ACADEMIC_YEAR AS AC
                        //                    WHERE
                        //                        AC.ACTIVE_YEAR = 1)
                        //                    AND SEM.BATCH = B.BATCH_ID
                        //                    INNER JOIN
                        //                TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.CLASS_ID
                        //                    AND TT.COURSE_ID = CCP.COURSE_ID
                        //                    AND TT.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                        //                    AND TT.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                        //                    AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                    AND ?DATE_TO <= CURDATE()
                        //                    AND CAL.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                        //                    AND CAL.IS_DELETED!=1
                        //                    INNER JOIN
                        //             ATT_ABSENTEE_LIST AS ATT ON ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                        //              AND ATT.COURSE_ID = CCP.COURSE_ID
                        //                    AND ATT.S_CLASS_ID = SC.CLASS_ID
                        //                    AND ATT.STUDENT_ID = SC.STUDENT_ID 
                        //                    AND ATT.ATTENDANCE_TYPE_ID!=1
                        //                    AND ATT.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                        //                    AND ATT.HOUR_FROM=TT.HOUR_ID
                        //                    AND ATT.IS_DELETED!=1
                        //            WHERE
                        //                CCP.COURSE_ID = ?COURSE_ID AND SC.CLASS_ID = ?CLASS_ID
                        //              AND SEM.IS_ACTIVE=1
                        //                    AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.ACADEMIC_YEAR=?AC_YEAR
                        //                    AND SC.IS_DELETED!=1
                        //                    AND CCP.IS_DELETED != 1 
                        //              GROUP BY SC.STUDENT_ID
                        //              UNION
                        //           SELECT 
                        //                SP.STUDENT_ID,
                        //                SP.REGISTER_NO,
                        //                CCR.COURSE_ROOT_ID AS COURSE_ID,
                        //                COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_HOURS
                        //            FROM
                        //                STU_CLASS AS SC
                        //                    INNER JOIN
                        //                STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                        //                    AND SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                    AND SCI.IS_DELETED != 1
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                    AND SP.IS_LEFT != 1
                        //                    AND SP.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SCI.COURSE_ID
                        //                    INNER JOIN
                        //                TT_TIMETABLE AS TT ON TT.CLASS_ID = SCI.S_CLASS_ID
                        //                    AND TT.COURSE_ID = SCI.COURSE_ID
                        //                    AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                    AND TT.IS_DELETED != 1
                        //                    INNER JOIN
                        //                CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                        //                    AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND CURDATE()
                        //                    AND ?DATE_TO <= CURDATE()
                        //                    AND CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                    AND CAL.IS_DELETED != 1
                        //                    INNER JOIN
                        //                ATT_ABSENTEE_LIST AS ATT ON ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                        //                    AND ATT.COURSE_ID = CCR.COURSE_ROOT_ID
                        //                    AND ATT.S_CLASS_ID = SCI.S_CLASS_ID
                        //                    AND ATT.STUDENT_ID = SC.STUDENT_ID
                        //                    AND ATT.ATTENDANCE_TYPE_ID != 1
                        //                    AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                    AND ATT.HOUR_FROM = TT.HOUR_ID
                        //                    AND ATT.IS_DELETED != 1
                        //            WHERE
                        //                SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SCI.S_CLASS_ID = ?CLASS_ID
                        //                    AND SCI.COURSE_ID = ?COURSE_ID
                        //                    AND SC.IS_DELETED != 1
                        //                    AND CCR.SEMESTER_ID IN (?SEMESTER_ID)
                        //            GROUP BY SC.STUDENT_ID;";

                        break;
                    }
                case AttendanceSQLCommands.FetchdDayOrderByAttendanceDate:
                    {
                        query = @"SELECT 
                                        CAL.DAY_ORDER
                                    FROM
                                        CALENDER AS CAL
                                    WHERE
                                        CAL.DAY_ORDER_DATE = ?DAY_ORDER_DATE
                                            AND CAL.IS_DELETED != 1
                                            AND CAL.IS_HOLIDAY != 1
                                            AND CAL.DAY_ORDER != 0;";
                        break;
                    }
                case AttendanceSQLCommands.FetchNoOfHours:
                    {
                        query = @"SELECT 
                                        TS.NO_OF_DAYS
                                    FROM
                                        TT_DEPARTMENT_WISE_TEMPLATE AS DWT
                                            INNER JOIN
                                        CP_DEPARTMENT_?AC_YEAR AS DT ON DT.DEPARTMENT_ID = DWT.DEPARTMENT_ID
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.DEPARTMENT = DT.DEPARTMENT_ID
                                            INNER JOIN
                                        TT_TEMPLATE_SETTING AS TS ON TS.SETTING_ID = DWT.SETTING_ID
                                    WHERE
                                        CLS.CLASS_ID = ?S_CLASS_ID
                                            AND DWT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND DWT.IS_DELETED != 1
                                            AND TS.IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.IsAbsenteeEntryExisting:
                    {
                        query = @"SELECT 
                                        ATTENDANCE_ID
                                    FROM
                                        ATT_ABSENTEE_LIST_?AC_YEAR
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND CLASS_ID = ?CLASS_ID
                                            AND ATTENDENCE_DATE = ?ATTENDENCE_DATE
                                            AND HOUR_FROM = ?HOUR_FROM;";
                        break;
                    }
                case AttendanceSQLCommands.SaveAbsenteeEntryForSpecialAttendance:
                    {
                        query = @"INSERT INTO ATT_SPECIAL_ABSENTEE_LIST_?AC_YEAR
                                        (
                                        ATTENDENCE_DATE,
                                        HOUR_FROM,
                                        HOUR_TO,
                                        STUDENT_ID,
                                        CLASS_ID,
                                        REASON_ID,
                                        ENTRY_ID,
                                        ENTRY_DATE,
                                        S_CLASS_ID,
                                        ATTENDANCE_TYPE_ID,
                                        SPL_ATTENDANCE_TYPE_ID
                                        )
                                        VALUES
                                        (?ATTENDENCE_DATE,
                                        ?HOUR_FROM,
                                        ?HOUR_TO,
                                        ?STUDENT_ID,
                                        ?CLASS_ID,
                                        ?REASON_ID,
                                        ?ENTRY_ID,
                                        ?ENTRY_DATE,
                                        ?S_CLASS_ID,
                                        ?ATTENDANCE_TYPE_ID,
                                        ?SPL_ATTENDANCE_TYPE_ID);";
                        return query;
                    }
                case AttendanceSQLCommands.UpdateAbsenteeEntryForSpecialAttendance:
                    {
                        query = @"UPDATE ATT_SPECIAL_ABSENTEE_LIST_?AC_YEAR
                                    SET
                                    ATTENDENCE_DATE = ?ATTENDENCE_DATE,
                                    STAFF_ID = ?STAFF_ID,
                                    HOUR_FROM = ?HOUR_FROM,
                                    HOUR_TO = ?HOUR_TO,
                                    STUDENT_ID = ?STUDENT_ID,
                                    CLASS_ID = ?CLASS_ID,
                                    REASON_ID = ?REASON_ID,
                                    ENTRY_ID = ?ENTRY_ID,
                                    ENTRY_DATE = ?ENTRY_DATE,
                                    S_CLASS_ID = ?S_CLASS_ID,
                                    ATTENDANCE_TYPE_ID = ?ATTENDANCE_TYPE_ID,
                                    SPL_ATTENDANCE_TYPE_ID = ?SPL_ATTENDANCE_TYPE_ID
                                    WHERE ATTENDANCE_ID = ?ATTENDANCE_ID,;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentListForAttendanceByCourseIdAndStaffIdForSpecialAttendance:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                SH.HOUR_TYPE_ID,
                                TT.HOUR_TYPE,
                                CLS.CLASS_ID AS S_CLASS_ID,
                                CLS.CLASS_ID,
                                ABS.ATTENDANCE_ID,
                                ABS.REASON_ID,
                                ABS.ENTRY_ID,
                                ABS.ENTRY_DATE,
                                SR.REASON_ID,
                                ABS.ATTENDANCE_TYPE_ID,SP.STUDENT_ID,
                                C.CLASS_NAME
                            FROM
                                CALENDER AS CAL
                                    INNER JOIN
                                TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER AND CAL.ACADEMIC_YEAR=?AC_YEAR AND TT.ACADEMIC_YEAR=?AC_YEAR AND TT.SEMESTER_ID IN (?SEMESTER_ID)
                                    AND TT.STAFF_ID = ?STAFF_ID
                                    AND TT.HOUR_ID = ?HOUR_ID
                                    AND TT.IS_DELETED!=1
                                    INNER JOIN
                                STU_CLASS AS CLS ON CLS.CLASS_ID = TT.CLASS_ID  AND CLS.ACADEMIC_YEAR=?AC_YEAR AND CLS.IS_DELETED!=1
                                    INNER JOIN
								CP_CLASSES AS C ON C.CLASS_ID=CLS.CLASS_ID AND C.ACADEMIC_YEAR=?AC_YEAR AND C.IS_DELETED!=1X
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID AND SP.IS_DELETED !=1 AND SP.IS_LEFT !=1
                                    INNER JOIN
                                SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                    LEFT JOIN
                                ATT_ABSENTEE_LIST AS ABS ON CAL.DAY_ORDER_DATE = ABS.ATTENDENCE_DATE
                                    AND ABS.STUDENT_ID = CLS.STUDENT_ID AND ABS.HOUR_FROM = ?HOUR_ID
                                    LEFT JOIN
                                SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                            WHERE
                                DAY_ORDER_DATE =?ATTENDANCE_DATE
                                    AND CAL.IS_DELETED != 1
                            GROUP BY SP.STUDENT_ID
                            ORDER BY SP.ROLL_NO , FIRST_NAME;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentListForAttendanceByClassIdForSpecialAttendance:
                    {
                        //query = @"SELECT 
                        //        SP.STUDENT_ID,
                        //                SP.ROLL_NO,
                        //                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                        //                SCLS.CLASS_ID AS S_CLASS_ID,
                        //                ABS.ATTENDANCE_ID,
                        //                ABS.REASON_ID,
                        //                ABS.ATTENDANCE_TYPE_ID,
                        //                ABS.ENTRY_ID,
                        //                ABS.ENTRY_DATE,
                        //                SR.REASON_ID,
                        //                ATT.ATTENDANCE_TYPE_ID,
                        //                ABS.ATTENDANCE_TYPE_ID,
                        //                C.CLASS_NAME
                        //    FROM
                        //        STU_CLASS AS SCLS
                        //            INNER JOIN
                        //        CP_CLASSES AS C ON C.CLASS_ID=SCLS.CLASS_ID AND C.IS_DELETED!=1
                        //            INNER JOIN
                        //        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCLS.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                        //            LEFT JOIN
                        //        ATT_ABSENTEE_LIST AS ABS ON ABS.ATTENDENCE_DATE = ?ATTENDANCE_DATE AND ABS.ACADEMIC_YEAR=?AC_YEAR
                        //            AND SCLS.STUDENT_ID = ABS.STUDENT_ID AND ABS.IS_DELETED!=1
                        //            LEFT JOIN
                        //        SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                        //            LEFT JOIN
                        //        SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                        //    WHERE
                        //        SCLS.CLASS_ID = ?CLASS_ID
                        //            AND SCLS.ACADEMIC_YEAR = ?AC_YEAR GROUP BY SCLS.STUDENT_ID;";
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    SCLS.CLASS_ID AS S_CLASS_ID,
                                    ABS.ATTENDANCE_ID,
                                    ABS.REASON_ID,
                                    ABS.ATTENDANCE_TYPE_ID,
                                    ABS.ENTRY_ID,
                                    ABS.ENTRY_DATE,
                                    SR.REASON_ID,
                                    C.CLASS_NAME,
                                    CR.COURSE_ROOT_ID AS 'COURSE_ID',
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE
                                FROM
                                    STU_CLASS AS SCLS
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SCLS.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCLS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ABS ON ABS.ATTENDENCE_DATE = ?ATTENDANCE_DATE
                                        AND ABS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SCLS.STUDENT_ID = ABS.STUDENT_ID
                                        AND ABS.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                        LEFT JOIN
                                    SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
		                                INNER JOIN
	                                TT_TIMETABLE AS TT ON TT.CLASS_ID=C.CLASS_ID AND TT.HOUR_ID = ?HOUR_ID
                                         AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                         AND TT.IS_DELETED!=1
		                                INNER JOIN
	                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=TT.COURSE_ID AND CR.ACADEMIC_YEAR = ?AC_YEAR AND CR.IS_DELETED!=1
                                WHERE
                                    SCLS.CLASS_ID = ?CLASS_ID
                                        AND SCLS.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY SCLS.STUDENT_ID ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchClassesByProgrammeId:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES_?AC_YEAR CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.IS_DELETED != 1 AND CLS.PROGRAMME=?PROGRAMME;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentListByClassId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(UPPER(FIRST_NAME), ' - ', UPPER(ROLL_NO)) AS 'FIRST_NAME'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    SC.CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchAbsentListByStaffId:
                    {
                        query = @"SELECT 
                                    date_format(ATTENDENCE_DATE,'%d/%m/%Y')  as ATTENDENCE_DATE,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    CLS.CLASS_NAME,
                                    SP.REGISTER_NO,
                                    SP.ROLL_NO,
                                    date_format(ENTRY_DATE,'%d/%m/%Y')  as ENTRY_DATE,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    HOUR_FROM ,SAT.ATTENDANCE_TYPE AS REASON
                                FROM
                                    ATT_ABSENTEE_LIST AS ABS
                                        LEFT JOIN
                                    CP_COURSE_ROOT  AS CR ON CR.COURSE_ROOT_ID = ABS.COURSE_ID
                                        LEFT JOIN
                                    SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                                        LEFT JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = ABS.S_CLASS_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = ABS.STUDENT_ID
                                WHERE
                                    ABS.STAFF_ID = ?STAFF_ID 
                                    AND ABS.ACADEMIC_YEAR=?AC_YEAR 
                                    AND CR.SEMESTER_ID IN (?SEMESTER_ID) 
                                    AND ABS.ATTENDANCE_TYPE_ID!=1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchHoursByStaffIdAndDate:
                    {
                        query = @"SELECT 
                                        TT.HOUR_ID, HOUR,TT.COURSE_ID,CCR.SEMESTER_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TT.HOUR_ID
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=TT.COURSE_ID
		                                    INNER JOIN
	                                     CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID=TT.STAFF_ID
											AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.CLASS_ID=TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
										SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()
                                                    AND ACADEMIC_YEAR = ?AC_YEAR)
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchHoursByStaffIdAndDateAndClassId:
                    {
                        query = @"SELECT 
                                        TT.HOUR_ID, HOUR,TT.COURSE_ID,CCR.SEMESTER_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TT.HOUR_ID
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=TT.COURSE_ID
		                                    INNER JOIN
	                                     CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID=TT.STAFF_ID
											AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.CLASS_ID=TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
										SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()
                                                    AND ACADEMIC_YEAR = ?AC_YEAR)
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.CLASS_ID = ?CLASS_ID
                                            AND TT.IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchClassByStaffIdAndHour:
                    {

                        query = @"SELECT 
                                        TT.CLASS_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.IS_DELETED != 1
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR)
                                    GROUP BY TT.CLASS_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentForSpecialAttendance:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                SP.ADMISSION_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                ABS.COMMON_ATTENDANCE_ID AS ATTENDANCE_ID,
                                ABS.REASON_ID,
                                ABS.ATTENDANCE_TYPE_ID,
                                ABS.ENTRY_ID,
                                ABS.ENTRY_DATE,
                                -- ATT.ATTENDANCE_TYPE_ID,
                                SCLS.STUDENT_ID,
                                SCLS.CLASS_ID AS S_CLASS_ID,
                                SCLS.CLASS_ID,
                                C.CLASS_NAME
                            FROM
                                STU_CLASS AS SCLS
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID=SCLS.CLASS_ID AND SCLS.IS_DELETED!=1
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCLS.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                    LEFT JOIN
                                ATT_COMMON_ABSENTEE_LIST AS ABS ON ABS.ATTENDENCE_DATE = ?ATTENDANCE_DATE
                                    AND SCLS.STUDENT_ID = ABS.STUDENT_ID AND SCLS.IS_DELETED!=1
                                    LEFT JOIN
                                SUP_REASON AS SR ON SR.REASON_ID = ABS.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS ATT ON ATT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                            WHERE
                                SCLS.CLASS_ID = ?CLASS_ID
                                    AND SCLS.ACADEMIC_YEAR = ?AC_YEAR ORDER BY SCLS.CLASS_ID,SP.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentForTagMapping:
                    {
                        query = @"SELECT 
                                            SPI.STUDENT_ID,SPI.ROLL_NO, SPI.FIRST_NAME, CC.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                                INNER JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            LEFT JOIN
                                            STU_LEVEL_TAG_MAPPING_INFO AS TM ON TM.STUDENT_ID=SPI.STUDENT_ID

                                        WHERE
                                            sc.ACADEMIC_YEAR = ?AC_YEAR AND
                                                CC.CLASS_ID=?CLASS_ID 
                                                AND spi.IS_DELETED != 1
                                                AND spi.IS_LEFT != 1;";
                        break;
                    }
               
                case AttendanceSQLCommands.SaveCommonAbsenteeEntry:
                    {
                        query = @"INSERT INTO ATT_COMMON_ABSENTEE_LIST
                                        (
                                        ATTENDENCE_DATE,
                                        STAFF_ID,
                                        STUDENT_ID,
                                        CLASS_ID,
                                        REASON_ID,
                                        ENTRY_ID,
                                        ENTRY_DATE,
                                        S_CLASS_ID,
                                        ATTENDANCE_TYPE_ID,
                                        ACADEMIC_YEAR,
                                        SEMESTER_TYPE
                                        )
                                        VALUES
                                        (?ATTENDENCE_DATE,
                                        ?STAFF_ID,
                                        ?STUDENT_ID,
                                        ?CLASS_ID,
                                        ?REASON_ID,
                                        ?ENTRY_ID,
                                        CURDATE(),
                                        ?S_CLASS_ID,
                                        ?ATTENDANCE_TYPE_ID,
                                        ?AC_YEAR,
                                        ?SEMESTER_TYPE);";
                        return query;
                    }
                case AttendanceSQLCommands.UpdateCommonAbsenteeEntry:
                    {
                        query = @"UPDATE att_common_absentee_list
                                    SET
                                    ATTENDENCE_DATE = ?ATTENDENCE_DATE,
                                    STAFF_ID = ?STAFF_ID,
                                    STUDENT_ID = ?STUDENT_ID,
                                    CLASS_ID = ?CLASS_ID,
                                    REASON_ID = ?REASON_ID,
                                    ENTRY_ID = ?ENTRY_ID,
                                    ENTRY_DATE = ?ENTRY_DATE,
                                    S_CLASS_ID = ?S_CLASS_ID,
                                    ATTENDANCE_TYPE_ID = ?ATTENDANCE_TYPE_ID
                                    WHERE COMMON_ATTENDANCE_ID = ?ATTENDANCE_ID;";
                        return query;
                    }
                case AttendanceSQLCommands.FetchAttendanceReportByClassIdAndDate:
                    {
                        query = @"SELECT 
                                        DATE_FORMAT(ATTENDENCE_DATE, '%d/%m/%Y') AS ATTENDENCE_DATE,
                                        CR.COURSE_TITLE,
                                        CR.COURSE_CODE,
                                        CLS.CLASS_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        DATE_FORMAT(ENTRY_DATE, '%d/%m/%Y') AS ENTRY_DATE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        HOUR_FROM,
                                        SAT.ATTENDANCE_TYPE AS REASON
                                    FROM
                                        ATT_ABSENTEE_LIST AS ABS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ABS.COURSE_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = ABS.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ABS.S_CLASS_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                                    WHERE
                                        ABS.S_CLASS_ID = ?CLASS_ID AND ABS.HOUR_FROM IN (?HOUR_ID)
                                            AND ABS.ATTENDENCE_DATE =?ATTENDANCE_DATE   
                                UNION
                                      SELECT 
                                        DATE_FORMAT(ATTENDENCE_DATE, '%d/%m/%Y') AS ATTENDENCE_DATE,
                                        '' AS COURSE_TITLE,
                                        '' AS COURSE_CODE,
                                        CLS.CLASS_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        DATE_FORMAT(ENTRY_DATE, '%d/%m/%Y') AS ENTRY_DATE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        '' AS HOUR_FROM,
                                        SAT.ATTENDANCE_TYPE AS REASON
                                    FROM
                                        ATT_COMMON_ABSENTEE_LIST AS ABS
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = ABS.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ABS.CLASS_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ABS.ATTENDANCE_TYPE_ID
                                    WHERE
                                        ABS.S_CLASS_ID = ?CLASS_ID
                                            AND ABS.ATTENDENCE_DATE =?ATTENDANCE_DATE;";
                        break;
                    }

                #region Attendance Report

                case AttendanceSQLCommands.FetchHourBasedCompulsaryPapersAttendanceReport:
                    {
                        query = @"SELECT 
                                            STUDENT_ID,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                            SUM(IF(ATTENDANCE_TYPE_ID = '2', 1, 0)) AS ABSENT,
                                            SUM(IF(ATTENDANCE_TYPE_ID = '3', 1, 0)) AS OD,
                                            SUM(IF(ATTENDANCE_TYPE_ID = '4', 1, 0)) AS FREE_ATTENDANCE
                                        FROM
                                            (SELECT 
                                                SC.STUDENT_ID,
                                                    TT.COURSE_ID,
                                                    CR.COURSE_CODE,
                                                    CR.COURSE_TITLE,
                                                    TT.DAY_ORDER_ID,
                                                    TT.HOUR_ID,
                                                    ATT.ATTENDANCE_TYPE_ID,
                                                    ATTENDANCE_ID
                                            FROM
                                                STU_CLASS AS SC
			                                        INNER JOIN 
		                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
			                                        INNER JOIN 
		                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
			                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
				                                        ACADEMIC_YEAR_ID
			                                        FROM
				                                        ACADEMIC_YEAR
			                                        WHERE
				                                        ACTIVE_YEAR = 1)
			                                        INNER JOIN 
		                                        CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
			                                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        INNER JOIN 
		                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
			                                        INNER JOIN 
		                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
			                                        AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        AND SEM.BATCH = AB.BATCH_ID
			                                        AND SEM.SEMESTER = CR.SEMESTER_ID
			                                        AND SEM.IS_DELETED != 1
			                                        INNER JOIN 
		                                        TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
			                                        AND TT.CLASS_ID = SC.CLASS_ID
			                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        AND TT.IS_DELETED != 1
			                                        INNER JOIN
		                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
			                                        AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
			                                        LEFT JOIN 
		                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
			                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
			                                        AND ATT.HOUR_FROM = TT.HOUR_ID
                                                    AND ATT.S_CLASS_ID=SC.CLASS_ID
                                                    AND ATT.STAFF_ID=TT.STAFF_ID
			                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                            WHERE
                                                SC.STUDENT_ID = ?STUDENT_ID
                                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND CR.IS_COMPULSORY = 1
                                                    AND CCP.IS_DELETED != 1
                                                    AND SC.IS_DELETED != 1
                                                    AND SEM.IS_ACTIVE = 1
                                            GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                        GROUP BY COURSE_ID;";
                        break;
                    }

                case AttendanceSQLCommands.FetchHourBasedOptionalPapersAttendanceReport:
                    {

                        query = @"SELECT 
                                        STUDENT_ID,
                                        COURSE_ID,
                                        COURSE_CODE,
                                        COURSE_TITLE,
                                        COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                        SUM(IF(ATTENDANCE_TYPE_ID = '2', 1, 0)) AS ABSENT,
                                        SUM(IF(ATTENDANCE_TYPE_ID = '3', 1, 0)) AS OD,
                                        SUM(IF(ATTENDANCE_TYPE_ID = '4', 1, 0)) AS FREE_ATTENDANCE
                                    FROM
                                        (SELECT 
                                            CCP.STUDENT_ID,
                                                TT.COURSE_ID,
                                                CR.COURSE_CODE,
                                                CR.COURSE_TITLE,
                                                TT.DAY_ORDER_ID,
                                                TT.HOUR_ID,
                                                ATT.ATTENDANCE_TYPE_ID,
                                                ATTENDANCE_ID
                                        FROM
                                        STU_COURSE_INFO AS CCP 
		                                    INNER JOIN 
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
		                                    INNER JOIN 
                                        TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                            AND TT.CLASS_ID = CCP.S_CLASS_ID
                                            AND TT.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
		                                    INNER JOIN 
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
		                                    LEFT JOIN 
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = CCP.STUDENT_ID
                                            AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.S_CLASS_ID=CCP.S_CLASS_ID
                                            AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                            AND ATT.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                        WHERE
                                            CCP.STUDENT_ID = ?STUDENT_ID
                                                AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                                                AND CCP.IS_DELETED != 1
                                                AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                        GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                    GROUP BY COURSE_ID;";
                        break;
                    }

                case AttendanceSQLCommands.FetchAttendanceReportForCumulative:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        MONTH_NAME,
                                        ABSENT,
                                        OD,
                                        FREE_ATTENDANCE,
                                        DAYS,
                                        DAY_ID,
                                        SUM(DAYS - ABSENT) AS 'PRESENT'
                                    FROM
                                        (SELECT 
                                            SC.STUDENT_ID,
                                                CONCAT(MONTHNAME(CAL.DAY_ORDER_DATE), ' - ', SC.ACADEMIC_YEAR) AS 'MONTH_NAME',
                                                SUM(IF(ATT.ATTENDANCE_TYPE_ID = '2', '1', '0')) AS 'ABSENT',
                                                SUM(IF(ATT.ATTENDANCE_TYPE_ID = '3', '1', '0')) AS 'OD',
                                                SUM(IF(ATT.ATTENDANCE_TYPE_ID = '4', '1', '0')) AS 'FREE_ATTENDANCE',
                                                COUNT(CAL.DAY_ID) AS DAYS,
                                                CAL.DAY_ID
                                        FROM
                                            CALENDER AS CAL
                                        INNER JOIN STU_CLASS AS SC ON SC.ACADEMIC_YEAR = CAL.ACADEMIC_YEAR
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                        LEFT JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                            AND ATT.IS_DELETED != 1
                                        WHERE
                                            CAL.ACADEMIC_YEAR = ?AC_YEAR AND SC.STUDENT_ID = ?STUDENT_ID
                                                AND CAL.IS_DELETED != 1
                                                AND CAL.IS_HOLIDAY!=1 
                                                AND CAL.DAY_ORDER_DATE BETWEEN ?DATE AND CURDATE()
                                        GROUP BY MONTH_NAME
                                        ORDER BY CAL.DAY_ID) AS T
                                    GROUP BY T.MONTH_NAME
                                    ORDER BY T.DAY_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentInfoByStudentId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.ADMISSION_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        C.CLASS_NAME,
                                        P.PROGRAMME_NAME AS PROGRAMME_NAME,
                                        PM.PROGRAMME_MODE
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
										CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=C.PROGRAMME_GROUP_ID
											INNER JOIN
										CP_PROGRAMME AS P ON P.PROGRAMME_ID=PG.PROGRAMME_ID
											INNER JOIN
										SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID=PG.PROGRAMME_MODE
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchClassWiseAttendanceReport:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
									ACADEMIC_SEMESTER AS SEM ON SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR AND SEM.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
										AND SEM.BATCH=SC.BATCH
                                        AND SEM.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID AND CR.SEMESTER_ID = SEM.SEMESTER
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND TT.CLASS_ID =SC.CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR 
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID 
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?DATE 
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        LEFT JOIN
	                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1 AND SEM.IS_ACTIVE=1
                                        AND SC.CLASS_ID = ?CLASS_ID  AND CR.IS_COMPULSORY = 1
                                        GROUP BY SC.STUDENT_ID,CR.COURSE_ROOT_ID,TT.HOUR_ID
                                 ORDER BY SP.STUDENT_ID,CR.COURSE_ROOT_ID;";
                        break;
                    }

                case AttendanceSQLCommands.FetchClassWiseAttendanceReportForOptionalCourse:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                        INNER JOIN
                                    STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
									ACADEMIC_SEMESTER AS SEM ON SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR AND SEM.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
										AND SEM.BATCH=SC.BATCH
                                        AND SEM.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID AND CR.SEMESTER_ID = SEM.SEMESTER
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND TT.CLASS_ID =CCP.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR 
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID 
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?DATE 
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        LEFT JOIN
	                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1
                                        AND CCP.S_CLASS_ID = ?CLASS_ID  AND SEM.IS_ACTIVE=1
                                        GROUP BY SC.STUDENT_ID,CR.COURSE_ROOT_ID,TT.HOUR_ID
                                 ORDER BY SP.STUDENT_ID,CR.COURSE_ROOT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.IsCommonExistForClassWiseAttendence:
                    {
                        query = @"SELECT 
                                        STUDENT_ID
                                    FROM
                                        ATT_COMMON_ABSENTEE_LIST
                                    WHERE
                                        ATTENDENCE_DATE = ?DATE
                                            AND S_CLASS_ID = ?CLASS_ID AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case AttendanceSQLCommands.FetchClasswiseAttendanceReportForCommon:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    'COMMON ATTENDANCE' AS 'HOUR_ID',
                                    'COMMON ATTENDANCE' AS COURSE_CODE,
                                    IFNULL(SAT.ATTENDANCE_TYPE, 'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        LEFT JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = ?DATE
                                        AND ATT.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.CLASS_ID = ?CLASS_ID
                                        AND SC.IS_DELETED != 1
                                GROUP BY SC.STUDENT_ID 
                                UNION SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    'COMMON ATTENDANCE' AS 'HOUR_ID',
                                    'COMMON ATTENDANCE' AS COURSE_CODE,
                                    IFNULL(SAT.ATTENDANCE_TYPE, 'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                                        AND SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND SCI.IS_DELETED != 1
                                        LEFT JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = ?DATE
                                        AND ATT.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SCI.S_CLASS_ID = ?CLASS_ID
                                        AND SC.IS_DELETED != 1
                                GROUP BY SC.STUDENT_ID;";
                        break;
                    }

                case AttendanceSQLCommands.FetchClassByStaffId:
                    {
                        query = @"SELECT 
                                        CS.CLASS_ID, CLS.CLASS_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CS.COURSE_ID
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        CS.STAFF_ID = ?STAFF_ID
                                            AND CS.IS_DELETED!=1
                                            AND CS.ACADEMIC_YEAR=?AC_YEAR
                                    GROUP BY CS.CLASS_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCourseByClassAndStaffId:
                    {
                        query = @"SELECT 
                                        CS.COURSE_ID,
                                         CONCAT(IFNULL(UPPER(CR.COURSE_TITLE),''),' - ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_TITLE'
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CS.COURSE_ID 
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                      SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        CS.CLASS_ID = ?CLASS_ID
                                            AND CS.STAFF_ID = ?STAFF_ID
                                            AND CS.ACADEMIC_YEAR=?AC_YEAR
                                            AND CS.IS_DELETED!=1
                                    GROUP BY COURSE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStaffWiseAttendanceReport:
                    {
                        //               query = @"SELECT 
                        //                           SP.STUDENT_ID,
                        //                           SP.ROLL_NO,
                        //                           SP.REGISTER_NO,
                        //                           CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                                   ' ',
                        //                                   IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                        //                           TT.HOUR_ID,
                        //                           ATT.ATTENDANCE_TYPE_ID,
                        //                           UPPER(CR.COURSE_TITLE) AS COURSE_TITLE,
                        //                           CR.COURSE_CODE AS COURSE_CODE,
                        //                           IFNULL(SAT.ATTENDANCE_TYPE, 'PRESENT') AS ATTENDANCE_TYPE,
                        //                           CONCAT(IFNULL(SP.FR_MOBILE,
                        //                                           IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                        //                       FROM
                        //                           STU_CLASS AS SC
                        //                               INNER JOIN
                        //                           STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //	INNER JOIN
                        //CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                        //	AND CCP.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                        //	INNER JOIN
                        //ACADEMIC_SEMESTER AS SEM ON SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                        //	AND SEM.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                        //                               AND SEM.BATCH=SC.BATCH
                        //                               INNER JOIN
                        //                           TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.CLASS_ID AND TT.COURSE_ID=CCP.COURSE_ID
                        //                               AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                               INNER JOIN
                        //                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=CCP.COURSE_ID AND CR.IS_DELETED!=1
                        //                               INNER JOIN
                        //                           CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                        //                               AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                        //                               AND CAL.DAY_ORDER_DATE = ?DATE
                        //                               LEFT JOIN
                        //                           ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                        //                               AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                        //                               AND ATT.COURSE_ID = TT.COURSE_ID
                        //                               LEFT JOIN
                        //                           SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                        //                       WHERE
                        //                           SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                               AND SC.CLASS_ID = ?CLASS_ID
                        //                               AND CR.IS_COMPULSORY = 1
                        //                               AND SEM.IS_ACTIVE=1
                        //                               AND CR.COURSE_ROOT_ID = ?COURSE_ID 
                        //                       UNION SELECT 
                        //                           SP.STUDENT_ID,
                        //                           SP.ROLL_NO,
                        //                           SP.REGISTER_NO,
                        //                           CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                                   ' ',
                        //                                   IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                        //                           TT.HOUR_ID,
                        //                           ATT.ATTENDANCE_TYPE_ID,
                        //                           UPPER(CR.COURSE_TITLE) AS COURSE_TITLE,
                        //                                                           CR.COURSE_CODE AS COURSE_CODE,
                        //                           IFNULL(SAT.ATTENDANCE_TYPE, 'PRESENT') AS ATTENDANCE_TYPE,
                        //                           CONCAT(IFNULL(SP.FR_MOBILE,
                        //                                           IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                        //                       FROM
                        //                           STU_CLASS AS SC
                        //                               INNER JOIN
                        //                           STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                               INNER JOIN
                        //                           STU_COURSE_INFO AS SCI ON SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                               AND SCI.STUDENT_ID = SC.STUDENT_ID
                        //                               AND SCI.IS_DELETED != 1
                        //                               INNER JOIN
                        //                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=SCI.COURSE_ID AND CR.IS_DELETED!=1
                        //                               INNER JOIN
                        //                           TT_TIMETABLE AS TT ON TT.CLASS_ID = SCI.S_CLASS_ID
                        //                               AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                               AND TT.COURSE_ID = SCI.COURSE_ID
                        //                               INNER JOIN
                        //                           CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                        //                               AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                        //                               AND CAL.DAY_ORDER_DATE = ?DATE
                        //                               LEFT JOIN
                        //                           ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                        //                               AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                        //                               AND ATT.COURSE_ID = TT.COURSE_ID
                        //                               LEFT JOIN
                        //                           SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                        //                       WHERE
                        //                           SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                               AND SCI.S_CLASS_ID = ?CLASS_ID
                        //                               AND CR.COURSE_ROOT_ID = ?COURSE_ID ;";
                        query = @"SELECT 
                                    ATTENDANCE_ID,
                                    C.CLASS_NAME,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')))AS 'FIRST_NAME',
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    HOUR_FROM AS 'HOUR_ID',
                                    SP.STUDENT_ID,
                                    AT.ATTENDANCE_TYPE,
                                    CCR.COURSE_TITLE,
                                    CCR.COURSE_CODE,
                                    IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(SP.STU_MOBILENO,''))) AS 'MOBILE'
                                FROM
                                    ATT_ABSENTEE_LIST AS ATT
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER_DATE = ATT.ATTENDENCE_DATE
                                        AND CAL.ACADEMIC_YEAR = ATT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?DATE
                                        AND CAL.IS_HOLIDAY != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ATT.COURSE_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = ATT.STUDENT_ID
		                                INNER JOIN
	                                CP_CLASSES AS C ON C.CLASS_ID=ATT.CLASS_ID
		                                INNER JOIN
	                                SUP_ATTENDANCE_TYPE AS AT ON AT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = CAL.DAY_ORDER
                                        AND TT.CLASS_ID = ATT.S_CLASS_ID
                                        AND TT.STAFF_ID = ATT.STAFF_ID
                                        AND TT.COURSE_ID = ATT.COURSE_ID
                                        AND TT.ACADEMIC_YEAR = ATT.ACADEMIC_YEAR
                                        AND TT.HOUR_ID = ATT.HOUR_FROM
                                        AND TT.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                        AND CCS.COURSE_ID = TT.COURSE_ID
                                        AND CCS.CLASS_ID = TT.CLASS_ID
                                        AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CCS.IS_DELETED != 1
                                        INNER JOIN
                                    SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE = 1
                                WHERE
                                    ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ATT.IS_DELETED != 1
                                        AND ATT.S_CLASS_ID = ?CLASS_ID
                                        AND ATT.COURSE_ID = ?COURSE_ID
                                        AND ATT.STAFF_ID = ?STAFF_ID
                                ORDER BY SP.REGISTER_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchDayWiseAttendanceReportForRegularPapers:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    C.CLASS_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                        INNER JOIN
								 CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID 
                                        AND  C.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND CCP.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID 
                                        INNER JOIN 
                                    ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=C.PROGRAMME_GROUP_ID
                                        AND SEM.SEMESTER=CR.SEMESTER_ID
                                        AND SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                        AND SEM.IS_ACTIVE=1
                                        AND SEM.IS_DELETED!=1
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND TT.CLASS_ID =SC.CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR 
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID 
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?DATE
                                        INNER JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        LEFT JOIN
	                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                        
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1
                                       AND CR.IS_COMPULSORY = 1  
                                        GROUP BY SC.STUDENT_ID,CR.COURSE_ROOT_ID
                                 ORDER BY SP.STUDENT_ID,CR.COURSE_ROOT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchDayWiseAttendanceReportForOptionalPapers:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        C.CLASS_NAME,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                ' ',
                                                IFNULL(CR.COURSE_CODE, '')) AS 'COURSE_CODE',
                                        ATT.ATTENDANCE_TYPE_ID,
                                        TT.HOUR_ID,
                                        IFNULL(SAT.ATTENDANCE_TYPE, 'PRESENT') AS ATTENDANCE_TYPE,
                                        CONCAT(IFNULL(SP.FR_MOBILE,
                                                        IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
		                                    AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
                                            AND SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                            AND SEM.SEMESTER=CR.SEMESTER_ID
                                            AND SEM.IS_ACTIVE=1
                                            AND SEM.IS_DELETED!=1
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND TT.CLASS_ID = SC.S_CLASS_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE = ?DATE
                                            INNER JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                            AND ATT.CLASS_ID=TT.CLASS_ID
                                            AND ATT.COURSE_ID=TT.COURSE_ID
                                            AND ATT.STAFF_ID=TT.STAFF_ID
                                            AND ATT.HOUR_FROM =TT.HOUR_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1
                                    GROUP BY SC.STUDENT_ID , CR.COURSE_ROOT_ID
                                    ORDER BY SP.STUDENT_ID , CR.COURSE_ROOT_ID;";
                        break;
                    }

                case AttendanceSQLCommands.FetchStudentWiseAttendanceReportForRegularPapers:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    C.CLASS_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                        INNER JOIN
								 CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID 
                                        AND  C.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        AND CCP.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                 
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND TT.CLASS_ID =SC.CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR 
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID 
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?DATE
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        LEFT JOIN
	                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                        
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1
                                        AND SC.CLASS_ID = ?CLASS_ID AND SC.STUDENT_ID=?STUDENT_ID AND CR.IS_COMPULSORY = 1
                                        GROUP BY SC.STUDENT_ID,CR.COURSE_ROOT_ID,TT.HOUR_ID
                                 ORDER BY SP.STUDENT_ID,CR.COURSE_ROOT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentWiseAttendanceReportForOptionalPapers:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                     C.CLASS_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                        INNER JOIN
                                    STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID AND CCP.IS_DELETED!=1
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND TT.CLASS_ID =CCP.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR 
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID 
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?DATE 
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        LEFT JOIN
	                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                        INNER JOIN
								 CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID AND  C.IS_DELETED!=1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1
                                        AND CCP.S_CLASS_ID = ?CLASS_ID AND SC.STUDENT_ID=?STUDENT_ID 
                                        GROUP BY SC.STUDENT_ID,CR.COURSE_ROOT_ID,TT.HOUR_ID
                                 ORDER BY SP.STUDENT_ID,CR.COURSE_ROOT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchValueEducationCourseByStudentId:
                    {
                        query = @"SELECT 
                                    STUDENT_ID,
                                    COURSE_ID,
                                    COURSE_CODE,
                                    COURSE_TITLE,
                                    COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                    SUM(IF(ATTENDANCE_TYPE_ID = '2', 1, 0)) AS ABSENT,
                                    SUM(IF(ATTENDANCE_TYPE_ID = '3', 1, 0)) AS OD,
                                    SUM(IF(ATTENDANCE_TYPE_ID = '4', 1, 0)) AS FREE_ATTENDANCE
                                FROM
                                    (SELECT 
                                        SC.STUDENT_ID,
                                            TT.COURSE_ID,
                                            CR.COURSE_CODE,
                                            CR.COURSE_TITLE,
                                            TT.DAY_ORDER_ID,
                                            TT.HOUR_ID,
                                            ATT.ATTENDANCE_TYPE_ID,
                                            ATTENDANCE_ID
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID
                                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
    
                                    INNER JOIN TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = CCP.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND TT.IS_DELETED != 1 AND TT.HOUR_TYPE=7
                                    INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.DAY_ORDER_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
                                    LEFT JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND ATT.HOUR_FROM = TT.HOUR_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                    GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                GROUP BY COURSE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchvalueEducationCourseByClassId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                        ATT.ATTENDANCE_TYPE_ID,
                                        TT.HOUR_ID,
                                        IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                        CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID
                                        AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
    
                                    INNER JOIN TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = CCP.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED != 1 AND TT.HOUR_TYPE=7
                                    INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.DAY_ORDER_DATE = ?DATE 
                                    LEFT JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND ATT.HOUR_FROM = TT.HOUR_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        INNER JOIN
	                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                     LEFT JOIN
	                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        CCP.S_CLASS_ID = ?CLASS_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                    GROUP BY  CCP.STUDENT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchValueEducationCourseByClassAndCourseId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                INNER JOIN STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID
                                    AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                                INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
    
                                INNER JOIN TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                    AND TT.CLASS_ID = CCP.S_CLASS_ID
                                    AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                    AND TT.IS_DELETED != 1 AND TT.HOUR_TYPE=7
                                INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                    AND CAL.DAY_ORDER_DATE = ?DATE 
                                LEFT JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                    AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                    AND ATT.HOUR_FROM = TT.HOUR_ID
                                    AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                    INNER JOIN
	                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                 LEFT JOIN
	                            SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                WHERE
                                    CCP.S_CLASS_ID = ?CLASS_ID
                                    AND CCP.COURSE_ID=?COURSE_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCP.IS_DELETED != 1
                                        AND SC.IS_DELETED != 1
                                GROUP BY  CCP.STUDENT_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchValueEducationCourseByDate:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DOB,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE,''),' ',IFNULL(CR.COURSE_CODE,'')) AS 'COURSE_CODE',
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    IFNULL(SAT.ATTENDANCE_TYPE,'PRESENT') AS ATTENDANCE_TYPE,
                                    CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(STU_MOBILENO,'')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                INNER JOIN STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID
                                    AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                                INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
    
                                INNER JOIN TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                    AND TT.CLASS_ID = CCP.S_CLASS_ID
                                    AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                    AND TT.IS_DELETED != 1 AND TT.HOUR_TYPE=7
                                INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                    AND CAL.DAY_ORDER_DATE = ?DATE 
                                INNER JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                    AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                    AND ATT.HOUR_FROM = TT.HOUR_ID
                                    AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                    INNER JOIN
	                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                    LEFT JOIN
	                            SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID=ATT.ATTENDANCE_TYPE_ID
                                WHERE
                                     SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCP.IS_DELETED != 1
                                        AND SC.IS_DELETED != 1;";
                        break;
                    }
                #endregion
                case AttendanceSQLCommands.FetchClassAndCourseByRFID:
                    {
                        query = @"SELECT 
                                    T.CLASS_ID,
                                    C.CLASS_NAME,
                                    T.COURSE_ID,
                                    CONCAT(IFNULL(R.COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(R.COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE,
                                    T.STAFF_ID,
                                    T.HOUR_ID
                                FROM
                                    TT_TIMETABLE AS T
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = T.STAFF_ID 
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = T.CLASS_ID
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS R ON R.COURSE_ROOT_ID = T.COURSE_ID
                                WHERE
                                    T.STAFF_ID = (SELECT 
                                            STAFF_ID
                                        FROM
                                            STF_PERSONAL_INFO AS SF
                                        WHERE
                                            SF.RFID = ?RFID
                                                AND SF.IS_DELETED != 1) AND T.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()) AND T.IS_DELETED!=1;";
                        break;
                    }
                case AttendanceSQLCommands.IsAbsenteeEntryExistingForRFID:
                    {
                        query = @"SELECT 
                                        ATTENDANCE_ID,STUDENT_ID
                                    FROM
                                        ATT_ABSENTEE_LIST
                                    WHERE
                                         S_CLASS_ID = ?CLASS_ID
                                            AND COURSE_ID = ?COURSE_ID
                                            AND STAFF_ID = ?STAFF_ID
                                            AND HOUR_FROM = ?HOUR_FROM
                                            AND ENTRY_DATE = CURDATE()
                                            AND ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchHoursByStaffAndCourseIdAndClassIdAndDayOrderId:
                    {
                        query = @"SELECT 
                                        H.HOUR_ID, H.HOUR
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOURS AS H ON H.HOUR_ID = TT.HOUR_ID
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID AND TT.COURSE_ID = ?COURSE_ID
                                            AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR GROUP BY H.HOUR_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchAttendenceForDuplicateAttendence:
                    {
                        query = @"SELECT 
                                    ATT.ATTENDANCE_ID,
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                    C.CLASS_NAME,
                                    C.CLASS_ID AS S_CLASS_ID,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
	                                    INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON  TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR AND TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = SC.CLASS_ID
                                        AND TT.STAFF_ID = ?STAFF_ID
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.STAFF_ID = TT.STAFF_ID
                                        AND ATT.COURSE_ID = CCP.COURSE_ID
                                        AND ATT.HOUR_FROM=TT.HOUR_ID
                                        AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED != 1
                                        AND SC.CLASS_ID = ?CLASS_ID
                                        AND TT.COURSE_ID = ?COURSE_ID
                                        AND SC.IS_DELETED!=1
                                        AND CCP.IS_DELETED!=1
                                        AND TT.HOUR_ID = ?HOUR_ID
                                UNION SELECT 
                                    ATT.ATTENDANCE_ID,
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                    C.CLASS_NAME,
                                    SCI.S_CLASS_ID,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                                        INNER JOIN
                                    STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                                        AND SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = SCI.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.STAFF_ID = TT.STAFF_ID
                                        AND ATT.COURSE_ID = CCP.COURSE_ID
                                        AND ATT.HOUR_FROM=TT.HOUR_ID
                                        AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SCI.S_CLASS_ID = ?CLASS_ID
                                        AND SCI.COURSE_ID = ?COURSE_ID
                                        AND TT.STAFF_ID=?STAFF_ID
                                        AND TT.IS_DELETED != 1
                                        AND SC.IS_DELETED!=1
		                                AND CCP.IS_DELETED!=1
                                        AND SCI.IS_DELETED!=1
                                        AND TT.HOUR_ID = ?HOUR_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentForAttendanceByStaffIdAndHourId:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
	                                    SC.CLASS_ID,
                                        TT.CLASS_ID AS S_CLASS_ID,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID,'0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,   
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID,'0') AS ATT_TYPE_ID,                                   
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,                                       
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.CLASS_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
		                                    AND CR.SEMESTER_ID =?SEMESTER_ID
                                            AND CR.IS_COMPULSORY=1
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = TT.CLASS_ID
                                            AND ATT.COURSE_ID = TT.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM=TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE=?ATTENDANCE_DATE
                                            AND ATT.IS_DELETED!=1
                                            INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID 
		                                    AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
		                                    LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED!=1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID=?STAFF_ID
                                            AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) 
                                    UNION 
	                                    SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        SC.CLASS_ID,
                                        SC.S_CLASS_ID ,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID,'0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,   
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID,'0') AS ATT_TYPE_ID,
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,                                     
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.S_CLASS_ID
                                            AND TT.COURSE_ID = SC.COURSE_ID 
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                            AND CR.SEMESTER_ID =?SEMESTER_ID
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = SC.S_CLASS_ID
                                            AND ATT.COURSE_ID = SC.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE = ?ATTENDANCE_DATE
                                            AND ATT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                             AND SC.IS_DELETED!=1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) ORDER BY ROLL_NO;";
                        break;
                    }

                case AttendanceSQLCommands.LoadStudentsForAttendanceEntry:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
	                                    SC.CLASS_ID,
                                        TT.CLASS_ID AS S_CLASS_ID,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID,'0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,   
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID,'0') AS ATT_TYPE_ID,                                   
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,                                       
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.CLASS_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
		                                    AND CR.SEMESTER_ID =?SEMESTER_ID
                                            AND CR.IS_COMPULSORY=1
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = TT.CLASS_ID
                                            AND ATT.COURSE_ID = TT.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM=TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE=?ATTENDANCE_DATE
                                            AND ATT.IS_DELETED!=1
                                            INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                                            AND C.SHIFT=?SHIFT
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID 
		                                    AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
		                                    LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED!=1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID=?STAFF_ID
                                            AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) 
                                    UNION 
	                                    SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        SC.CLASS_ID,
                                        SC.S_CLASS_ID ,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID,'0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,   
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID,'0') AS ATT_TYPE_ID,
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,                                     
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.S_CLASS_ID
                                            AND TT.COURSE_ID = SC.COURSE_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                            AND CR.SEMESTER_ID =?SEMESTER_ID
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = SC.S_CLASS_ID
                                            AND ATT.COURSE_ID = SC.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE = ?ATTENDANCE_DATE
                                            AND ATT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.S_CLASS_ID
                                            AND C.SHIFT=?SHIFT
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                             AND SC.IS_DELETED!=1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) ORDER BY ROLL_NO;";
                        break;
                    }

                case AttendanceSQLCommands.LoadSubstituteStudentsForAttendanceEntry:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
	                                    SC.CLASS_ID,
                                        TT.CLASS_ID AS S_CLASS_ID,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID,'0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,   
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID,'0') AS ATT_TYPE_ID,                                   
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,                                       
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        ATT_SUBSTITUTE AS TT ON TT.CLASS_ID = SC.CLASS_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
		                                    AND CR.SEMESTER_ID =?SEMESTER_ID
                                            AND CR.IS_COMPULSORY=1
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = TT.CLASS_ID
                                            AND ATT.COURSE_ID = TT.COURSE_ID
                                            AND ATT.STAFF_ID = TT.INCHARGE_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM=TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE=?ATTENDANCE_DATE
                                            AND ATT.IS_DELETED!=1
                                            INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                                            AND C.SHIFT=?SHIFT
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID 
		                                    AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
		                                    LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED!=1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.INCHARGE_ID=?STAFF_ID
                                            AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) 
                                    UNION 
	                                    SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        SC.CLASS_ID,
                                        SC.S_CLASS_ID ,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID,'0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,   
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID,'0') AS ATT_TYPE_ID,
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,                                     
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        ATT_SUBSTITUTE AS TT ON TT.CLASS_ID = SC.S_CLASS_ID
                                            AND TT.COURSE_ID = SC.COURSE_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                            AND CR.SEMESTER_ID =?SEMESTER_ID
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = SC.S_CLASS_ID
                                            AND ATT.COURSE_ID = SC.COURSE_ID
                                            AND ATT.STAFF_ID = TT.INCHARGE_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE = ?ATTENDANCE_DATE
                                            AND ATT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.SHIFT=?SHIFT
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                             AND SC.IS_DELETED!=1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.INCHARGE_ID = ?STAFF_ID
                                            AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) ORDER BY ROLL_NO;";
                        break;
                    }

                case AttendanceSQLCommands.FetchValueEducationCourseAttendacneForCurrentSemesterByStudentId:
                    {
                        query = @"SELECT 
                                    STUDENT_ID,
                                    COURSE_ID,
                                    COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                    SUM(IF(ATTENDANCE_TYPE_ID = '2', 1, 0)) AS ABSENT,
                                    SUM(IF(ATTENDANCE_TYPE_ID = '3', 1, 0)) AS OD,
                                    SUM(IF(ATTENDANCE_TYPE_ID = '4', 1, 0)) AS FREE_ATTENDANCE
                                FROM
                                    (SELECT 
                                        SC.STUDENT_ID,
                                            TT.COURSE_ID,
                                            CR.COURSE_CODE,
                                            CR.COURSE_TITLE,
                                            TT.DAY_ORDER_ID,
                                            TT.HOUR_ID,
                                            ATT.ATTENDANCE_TYPE_ID,
                                            ATTENDANCE_ID
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN STU_COURSE_INFO AS CCP ON CCP.STUDENT_ID = SC.STUDENT_ID
                                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
    
                                     INNER JOIN
                                     ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                     AND S.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                     AND S.BATCH=SC.BATCH
                                     AND S.IS_DELETED!=1
                                     AND S.IS_ACTIVE=1

                                    INNER JOIN TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = CCP.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND TT.SEMESTER_ID=S.SEMESTER
                                        AND TT.IS_DELETED != 1 AND TT.HOUR_TYPE=7
                                        
                                    INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.DAY_ORDER_DATE BETWEEN S.DATE_FROM AND S.DATE_TO
                                    LEFT JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND ATT.HOUR_FROM = TT.HOUR_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                    WHERE
                                        SC.STUDENT_ID =?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                    GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                GROUP BY COURSE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchOptionalCourseAttendacneForCurrentSemesterByStudentId:
                    {
                        query = @"                                
                                    SELECT 
                                            STUDENT_ID,
                                            COURSE_ID,
                                            COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                            SUM(IF(ATTENDANCE_TYPE_ID = '2', 1, 0)) AS ABSENT,
                                            SUM(IF(ATTENDANCE_TYPE_ID = '3', 1, 0)) AS OD,
                                            SUM(IF(ATTENDANCE_TYPE_ID = '4', 1, 0)) AS FREE_ATTENDANCE
                                        FROM
                                            (SELECT 
                                                CCP.STUDENT_ID,
                                                    TT.COURSE_ID,
                                                    CR.COURSE_CODE,
                                                    CR.COURSE_TITLE,
                                                    TT.DAY_ORDER_ID,
                                                    TT.HOUR_ID,
                                                    ATT.ATTENDANCE_TYPE_ID,
                                                    ATTENDANCE_ID
                                            FROM
                                            STU_COURSE_INFO AS CCP 
                                        
		                                        INNER JOIN 
                                                STU_CLASS AS SC ON SC.STUDENT_ID=CCP.STUDENT_ID
                                                AND SC.IS_DELETED!=1
                                                AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            
                                                INNER JOIN
                                                ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                                AND S.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                                AND S.IS_DELETED!=1
                                                AND S.BATCH=SC.BATCH
                                                AND S.IS_ACTIVE=1
                                            
                                               INNER JOIN
                                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                        
		                                        INNER JOIN 
                                            TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                                AND TT.CLASS_ID = CCP.S_CLASS_ID
                                                AND TT.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                                AND TT.IS_DELETED != 1
                                                 AND TT.SEMESTER_ID=S.SEMESTER
		                                        INNER JOIN 
                                            CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                                AND CAL.DAY_ORDER_DATE BETWEEN S.DATE_FROM AND S.DATE_TO
		                                        INNER JOIN 
                                            ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                                AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                                AND ATT.HOUR_FROM = TT.HOUR_ID
                                                AND ATT.S_CLASS_ID=CCP.S_CLASS_ID
                                                AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                                AND ATT.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                                INNER JOIN
                                            CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                                AND CCS.COURSE_ID = TT.COURSE_ID
                                                AND CCS.CLASS_ID = TT.CLASS_ID
                                                AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                                AND CCS.IS_DELETED != 1
                                                INNER JOIN
                                            SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                                AND ST.IS_ACTIVE = 1
                                            WHERE
                                                CCP.STUDENT_ID =?STUDENT_ID
                                                    AND CCP.ACADEMIC_YEAR =?AC_YEAR
                                                    AND CCP.IS_DELETED != 1
                                            GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                        GROUP BY COURSE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchRegularCourseAttendacneForCurrentSemesterByStudentId:
                    {
                        query = @"

                                            SELECT 
                                            STUDENT_ID,
                                            COURSE_ID,
                                          
                                            COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                            SUM(IF(ATTENDANCE_TYPE_ID = '2', 1, 0)) AS ABSENT,
                                            SUM(IF(ATTENDANCE_TYPE_ID = '3', 1, 0)) AS OD,
                                            SUM(IF(ATTENDANCE_TYPE_ID = '4', 1, 0)) AS FREE_ATTENDANCE
                                        FROM
                                            (SELECT 
                                                SC.STUDENT_ID,
                                                    TT.COURSE_ID,
                                                    CR.COURSE_CODE,
                                                    CR.COURSE_TITLE,
                                                    TT.DAY_ORDER_ID,
                                                    TT.HOUR_ID,
                                                    ATT.ATTENDANCE_TYPE_ID,
                                                    ATTENDANCE_ID
                                            FROM
                                                STU_CLASS AS SC
			                                        INNER JOIN 
		                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
			                                        INNER JOIN 
		                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
			                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
				                                        ACADEMIC_YEAR_ID
			                                        FROM
				                                        ACADEMIC_YEAR
			                                        WHERE
				                                        ACTIVE_YEAR = 1)
			                                        INNER JOIN 
		                                        CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
			                                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        INNER JOIN 
		                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
			                                        INNER JOIN 
		                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
			                                        AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        AND SEM.BATCH = AB.BATCH_ID
			                                        AND SEM.SEMESTER = CR.SEMESTER_ID
                                                    AND SEM.IS_ACTIVE=1
			                                        AND SEM.IS_DELETED != 1
			                                        INNER JOIN 
		                                        TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
			                                        AND TT.CLASS_ID = SC.CLASS_ID
			                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        AND TT.IS_DELETED != 1
			                                        INNER JOIN
		                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
			                                        AND CAL.DAY_ORDER_DATE BETWEEN SEM.DATE_FROM AND SEM.DATE_TO
			                                        INNER JOIN 
		                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
			                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
			                                        AND ATT.HOUR_FROM = TT.HOUR_ID
                                                    AND ATT.S_CLASS_ID=SC.CLASS_ID
                                                    AND ATT.STAFF_ID=TT.STAFF_ID
			                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                            WHERE
                                                SC.STUDENT_ID =?STUDENT_ID
                                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
                                                    AND CR.IS_COMPULSORY = 1
                                                    AND CCP.IS_DELETED != 1
                                                    AND SC.IS_DELETED != 1
                                                    AND SEM.IS_ACTIVE = 1
                                            GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                        GROUP BY COURSE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStaffBasedStudentForAttendanceByStaffIdAndHourId:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                CR.COURSE_CODE,
                                TT.COURSE_ID,
                                CR.COURSE_TITLE,
                                SC.CLASS_ID,
                                SC.S_CLASS_ID,
                                TT.STAFF_ID,
                                IFNULL(ATT.ATTENDANCE_ID, '0') AS ATT_ID,
                                ATT.ATTENDANCE_ID,
                                ATT.REASON_ID,
                                ATT.ATTENDANCE_TYPE_ID,
                                IFNULL(ATT.ATTENDANCE_TYPE_ID, '0') AS ATT_TYPE_ID,
                                ATT.ENTRY_ID,
                                ATT.ENTRY_DATE,
                                SR.REASON_ID,
                                SP.STUDENT_ID,
                                C.CLASS_NAME
                            FROM
                                STU_COURSE_INFO AS SC
                                    INNER JOIN
                                TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.S_CLASS_ID
                                    AND TT.COURSE_ID = SC.COURSE_ID
                                    AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    AND TT.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                    AND CR.SEMESTER_ID = ?SEMESTER_ID
                                    LEFT JOIN
                                ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                    AND ATT.S_CLASS_ID = SC.S_CLASS_ID
                                    AND ATT.COURSE_ID = SC.COURSE_ID
                                    AND ATT.STAFF_ID = TT.STAFF_ID
                                    AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    AND ATT.HOUR_FROM = TT.HOUR_ID
                                    AND ATT.ATTENDENCE_DATE =?ATTENDANCE_DATE
                                    AND ATT.IS_DELETED != 1
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    LEFT JOIN
                                SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                    LEFT JOIN
                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                            WHERE
		                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.STAFF_ID = ?STAFF_ID
                                    AND SC.IS_DELETED != 1
                                    AND TT.HOUR_ID = ?HOUR_ID
                                    AND TT.STAFF_ID = ?STAFF_ID
                                    AND TT.COURSE_ID = ?COURSE_ID
                                    AND TT.DAY_ORDER_ID = (SELECT 
                                        DAY_ORDER
                                    FROM
                                        CALENDER
                                    WHERE
                                        DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                            AND ACADEMIC_YEAR = ?AC_YEAR)
                            ORDER BY ROLL_NO;";
                        break;
                    }
                #region MonthWise Attendance Report

                #region MonthWise Attendance Report For Student
                case AttendanceSQLCommands.FetchAbsentHoursCountWithDate:
                    {
                        query = @"SELECT 
                                        CAL.DAY_ID,
                                        DATE_FORMAT(CAL.DAY_ORDER_DATE,'%d/%m/%Y') DAY_ORDER_DATE,
                                        CAL.DAY_ORDER,
                                        CAL.DAY_ORDER_MONTH,
                                        ?STUDENT_ID AS STUDENT_ID,
                                        TT.COURSE_ID,
                                        COUNT(TT.DAY_ORDER_ID) AS HOURS_COUNT
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID 
                                            AND TT.COURSE_ID IN (?COURSE_ID)
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CAL.DAY_ORDER_MONTH LIKE '%?DAY_ORDER_MONTH%'
                                            AND TT.IS_DELETED != 1
                                    GROUP BY TT.COURSE_ID,CAL.DAY_ORDER_DATE,TT.STAFF_ID
                                    UNION
                                    SELECT 
                                        CAL.DAY_ID,
                                        DATE_FORMAT(CAL.DAY_ORDER_DATE,'%d/%m/%Y') DAY_ORDER_DATE,
                                        CAL.DAY_ORDER,
                                        CAL.DAY_ORDER_MONTH,
                                        ?STUDENT_ID AS STUDENT_ID,
                                        TT.COURSE_ID,
                                        COUNT(TT.DAY_ORDER_ID) AS HOURS_COUNT
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.CLASS_ID = S_CLASS_ID
		                                    AND TT.COURSE_ID=SC.COURSE_ID
                                            AND TT.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER=TT.DAY_ORDER_ID AND CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.COURSE_ID IN (?COURSE_ID)
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND TT.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                            AND CAL.DAY_ORDER_MONTH LIKE '%?DAY_ORDER_MONTH%'
                                    GROUP BY SC.COURSE_ID,CAL.DAY_ORDER_DATE,TT.STAFF_ID;";
                        break;
                    }

                case AttendanceSQLCommands.FetchStudentAbsentCountWithDate:
                    {
                        query = @"SELECT 
                                    ATTENDANCE_ID,
                                    DATE_FORMAT(ATT.ATTENDENCE_DATE,'%d/%m/%Y') ATTENDENCE_DATE,                                    
                                    ATT.STAFF_ID,
                                    ATT.COURSE_ID,
                                    HOUR_FROM,
                                    HOUR_TO,
                                    STUDENT_ID,
                                    ATT.CLASS_ID,
                                    ENTRY_ID,
                                    ENTRY_DATE,
                                    ATT.S_CLASS_ID,
                                    COUNT(STUDENT_ID) AS ABSENT_COUNT
                                FROM
                                    ATT_ABSENTEE_LIST AS ATT
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER_DATE = ATT.ATTENDENCE_DATE
                                        AND CAL.ACADEMIC_YEAR = ATT.ACADEMIC_YEAR
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND CAL.DAY_ORDER_MONTH LIKE '%?DAY_ORDER_MONTH%'
                                        AND ATTENDANCE_TYPE_ID = 2
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY ATTENDENCE_DATE,COURSE_ID
                                ORDER BY ATTENDANCE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchTotalDaysByMonthFromCalender:
                    {
                        query = @"SELECT 
                                    DAY_ID,
                                    DATE_FORMAT(DAY_ORDER_DATE,'%d/%m/%Y') DAY_ORDER_DATE,
                                    IS_HOLIDAY,
                                    IF(DAY_ORDER IS NULL,'',IF(DAY_ORDER=0,'',DAY_ORDER)) AS DAY_ORDER,
                                    DAY_ORDER_MONTH,
                                    IF(DAY_ORDER_DATE>CURDATE(),'0','1') AS CUR_DATE_VALIDATE,
                                    UPPER(DAYNAME(DAY_ORDER_DATE)) AS DAY_NAME
                                FROM
                                    CALENDER
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR
                                        AND DAY_ORDER_MONTH LIKE '%?DAY_ORDER_MONTH%' ORDER BY DAY_ID;";
                        break;
                    }
                #endregion

                #region MonthWise Attendance Report For Course Staff

                case AttendanceSQLCommands.FetchTotalHoursByCourseIdWithDate:
                    {
                        query = @"SELECT 
                                        CAL.DAY_ID,
                                        DATE_FORMAT(CAL.DAY_ORDER_DATE,'%d/%m/%Y') DAY_ORDER_DATE,
                                        CAL.DAY_ORDER,
                                        CAL.DAY_ORDER_MONTH,
                                        TT.COURSE_ID,
                                        COUNT(TT.DAY_ORDER_ID) AS HOURS_COUNT
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID
											AND TT.COURSE_ID=?COURSE_ID
                                            AND TT.STAFF_ID=?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CAL.DAY_ORDER_MONTH LIKE '%?DAY_ORDER_MONTH%'
                                            AND TT.IS_DELETED != 1
                                    GROUP BY TT.COURSE_ID,CAL.DAY_ORDER_DATE
									ORDER BY DAY_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentAbsentCountByCourseIdWithDate:
                    {
                        query = @"SELECT 
                                    ATTENDANCE_ID,
                                    DATE_FORMAT(ATT.ATTENDENCE_DATE,'%d/%m/%Y') ATTENDENCE_DATE,                                    
                                    ATT.STAFF_ID,
                                    ATT.COURSE_ID,
                                    HOUR_FROM,
                                    HOUR_TO,
                                    STUDENT_ID,
                                    ATT.CLASS_ID,
                                    ENTRY_ID,
                                    ENTRY_DATE,
                                    ATT.S_CLASS_ID,
                                    COUNT(STUDENT_ID) AS ABSENT_COUNT
                                FROM
                                    ATT_ABSENTEE_LIST AS ATT
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER_DATE = ATT.ATTENDENCE_DATE
                                        AND CAL.ACADEMIC_YEAR = ATT.ACADEMIC_YEAR
                                WHERE
                                    ATT.S_CLASS_ID=?CLASS_ID
                                       AND ATT.COURSE_ID = ?COURSE_ID
                                        AND ATT.STAFF_ID=?STAFF_ID
                                        AND CAL.DAY_ORDER_MONTH LIKE '%?DAY_ORDER_MONTH%'
                                        AND ATTENDANCE_TYPE_ID = 2
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY ATTENDENCE_DATE,STUDENT_ID
                                ORDER BY ATTENDANCE_ID;";
                        break;
                    }
                #endregion
                #endregion

                #region Att Absentees Proccessed
                case AttendanceSQLCommands.IsExistAbsenteesProccessed:
                    {
                        query = @"SELECT 
                                        ATTENDANCE_ID
                                    FROM
                                        ATT_ABSENTEES_PROCCESSED
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND COURSE_ID = ?COURSE_ID
                                            AND HOUR_FROM = ?HOUR_FROM
                                            AND CLASS_ID = ?CLASS_ID
                                            AND ATTENDENCE_DATE = ?ATTENDENCE_DATE
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND S_CLASS_ID = ?S_CLASS_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.SaveAttAbsenteesProccessed:
                    {
                        query = @"INSERT INTO ATT_ABSENTEES_PROCCESSED 
                                  ( 
                                     ATTENDENCE_DATE, 
                                     STAFF_ID, 
                                     COURSE_ID, 
                                     HOUR_FROM, 
                                     HOUR_TO, 
                                     STATUS, 
                                     CLASS_ID, 
                                     ENTRY_ID, 
                                     ENTRY_DATE, 
                                     S_CLASS_ID, 
                                     ACADEMIC_YEAR
                                ) VALUES
                                ( 
                                     ?ATTENDENCE_DATE, 
                                     ?STAFF_ID, 
                                     ?COURSE_ID, 
                                     ?HOUR_FROM, 
                                     ?HOUR_TO, 
                                     ?STATUS, 
                                     ?CLASS_ID, 
                                     ?ENTRY_ID, 
                                     CURDATE(),
                                     ?S_CLASS_ID, 
                                     ?AC_YEAR
                                );";
                        break;
                    }
                #endregion
                #region Attendance Proccessed Report For Principal
                case AttendanceSQLCommands.FetchAttAbsenteesProccessedReportForPrincipal:
                    {
                        query = @"SELECT 
                                        CONCAT(IFNULL(CR.COURSE_TITLE,''),' ( ',IFNULL(CR.COURSE_CODE,''),' ) ') AS COURSE_TITLE,
                                        C.CLASS_ID,
                                        C.CLASS_NAME,
                                        TT.STAFF_ID,
                                        SP.STAFF_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        D.DEPARTMENT,
                                        TT.HOUR_ID,
                                        H.HOUR,
                                        ATT.ATTENDANCE_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
		                                    AND CR.IS_DELETED!=1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = TT.CLASS_ID
                                            AND C.SHIFT=?SHIFT
		                                    AND C.IS_DELETED!=1
                                            INNER JOIN
                                         SUP_HOURS AS H ON H.HOUR_ID=TT.HOUR_ID
											INNER JOIN
										CP_CLASS_COURSE_STAFF AS CC ON CC.CLASS_ID=TT.CLASS_ID
											AND CC.STAFF_ID=TT.STAFF_ID
                                            AND CC.COURSE_ID=TT.COURSE_ID
											AND CC.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CC.IS_DELETED!=1
                                            INNER JOIN
										SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CC.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=TT.STAFF_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                            LEFT JOIN   
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=SP.DEPARTMENT
                                            LEFT JOIN
                                        ATT_ABSENTEES_PROCCESSED AS ATT ON ATT.S_CLASS_ID = TT.CLASS_ID
                                            AND ATT.COURSE_ID = TT.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE=CAL.DAY_ORDER_DATE
                                            AND ATT.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                    WHERE
                                        TT.IS_DELETED != 1
                                            AND CAL.DAY_ORDER_DATE = ?ATTENDENCE_DATE AND TT.ACADEMIC_YEAR=?AC_YEAR
                                        ORDER BY SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                #endregion

                #region Att Fetch Attendance Subtitute
                case AttendanceSQLCommands.FetchSubtituteHoursToRemove:
                    {
                        query = @"SELECT 
	                                    DAY_ORDER_ID,
                                        HOUR_ID,
                                        STAFF_ID,
                                        INCHARGE_ID
                                    FROM
                                        ATT_SUBSTITUTE AS ATT
                                            INNER JOIN
										CP_CLASSES AS C ON C.CLASS_ID=ATT.CLASS_ID
											AND C.SHIFT=?SHIFT
                                    WHERE
                                        DAY_ORDER_DATE = CURDATE()
                                            AND STAFF_ID = ?STAFF_ID
                                            AND ATT.IS_DELETED != 1;";
                        break;
                    }
                case AttendanceSQLCommands.FetchSubtituteHoursToAdd:
                    {
                        query = @"SELECT 
	                                    ATT.DAY_ORDER_ID,
                                        ATT.HOUR_ID,
                                        CONCAT(H.HOUR,' (Substitute Hour)') AS HOUR,
                                        ATT.COURSE_ID,
                                        CCR.SEMESTER_ID,
                                        ATT.STAFF_ID,
                                        ATT.INCHARGE_ID
                                    FROM
                                        ATT_SUBSTITUTE AS ATT
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=ATT.COURSE_ID
		                                    INNER JOIN
	                                    CP_CLASSES AS CC ON CC.CLASS_ID=ATT.CLASS_ID
		                                    AND CC.SHIFT=?SHIFT
                                            INNER JOIN
	                                    SUP_HOURS AS H ON H.HOUR_ID=ATT.HOUR_ID
                                    WHERE
                                        ATT.DAY_ORDER_DATE = CURDATE()
                                            AND ATT.INCHARGE_ID = ?STAFF_ID
                                            AND ATT.IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region Online Attendance
                case AttendanceSQLCommands.FetchStudentsForOnlineAttendance:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        SC.CLASS_ID,
                                        TT.CLASS_ID AS S_CLASS_ID,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID, '0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID, '0') AS ATT_TYPE_ID,
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        TT_ONLINE_TIMETABLE AS TT ON TT.CLASS_ID = SC.CLASS_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            AND CR.IS_COMPULSORY = 1
                                            LEFT JOIN
                                        ATT_ONLINE_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = TT.CLASS_ID
                                            AND ATT.COURSE_ID = TT.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE = CURDATE()
                                            AND ATT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.SHIFT=?SHIFT
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()
                                                    AND ACADEMIC_YEAR = ?AC_YEAR) 
                                    UNION SELECT 
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CR.COURSE_CODE,
                                        TT.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        SC.CLASS_ID,
                                        SC.S_CLASS_ID,
                                        TT.STAFF_ID,
                                        IFNULL(ATT.ATTENDANCE_ID, '0') AS ATT_ID,
                                        ATT.ATTENDANCE_ID,
                                        ATT.REASON_ID,
                                        ATT.ATTENDANCE_TYPE_ID,
                                        IFNULL(ATT.ATTENDANCE_TYPE_ID, '0') AS ATT_TYPE_ID,
                                        ATT.ENTRY_ID,
                                        ATT.ENTRY_DATE,
                                        SR.REASON_ID,
                                        SP.STUDENT_ID,
                                        C.CLASS_NAME
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        TT_ONLINE_TIMETABLE AS TT ON TT.CLASS_ID = SC.S_CLASS_ID
                                            AND TT.COURSE_ID = SC.COURSE_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                            LEFT JOIN
                                        ATT_ONLINE_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.S_CLASS_ID = SC.S_CLASS_ID
                                            AND ATT.COURSE_ID = SC.COURSE_ID
                                            AND ATT.STAFF_ID = TT.STAFF_ID
                                            AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.ATTENDENCE_DATE = CURDATE()
                                            AND ATT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            LEFT JOIN
                                        SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                            LEFT JOIN
                                        SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()
                                                    AND ACADEMIC_YEAR = ?AC_YEAR)
                                    ORDER BY ROLL_NO;";
                        break;
                    }

                case AttendanceSQLCommands.SaveOnlineAbsentees:
                    {
                        query = @"INSERT INTO ATT_ONLINE_ABSENTEE_LIST
                                    (
	                                    ATTENDENCE_DATE,
	                                    STAFF_ID,
	                                    COURSE_ID,
	                                    HOUR_FROM,
	                                    HOUR_TO,
	                                    STUDENT_ID,
	                                    STATUS,
	                                    CLASS_ID,
	                                    REASON_ID,
	                                    ENTRY_ID,
	                                    ENTRY_DATE,
	                                    S_CLASS_ID,
	                                    ATTENDANCE_TYPE_ID,
	                                    IS_PROCESSED,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
	                                    ?ATTENDENCE_DATE,
                                        ?STAFF_ID,
                                        ?COURSE_ID,
                                        ?HOUR_FROM,
                                        ?HOUR_TO,
                                        ?STUDENT_ID,
                                        ?STATUS,
                                        ?CLASS_ID,
                                        ?REASON_ID,
                                        ?ENTRY_ID,
                                        ?ENTRY_DATE,
                                        ?S_CLASS_ID,
                                        ?ATTENDANCE_TYPE_ID,
                                        '1',
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case AttendanceSQLCommands.UpdateOnlineAbsentees:
                    {
                        query = @"UPDATE ATT_ONLINE_ABSENTEE_LIST SET ATTENDANCE_TYPE_ID = ?ATTENDANCE_TYPE_ID WHERE ATTENDANCE_ID = ?ATTENDANCE_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchOnlineHourByClassIdAndCourseIdAndDate:
                    {
                        query = @"SELECT 
                                        H.HOUR_ID, H.HOUR
                                    FROM
                                        TT_ONLINE_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOURS AS H ON H.HOUR_ID = TT.HOUR_ID
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID
                                            AND TT.COURSE_ID = ?COURSE_ID
                                            AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED!=1
                                    GROUP BY H.HOUR_ID;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentsForDateWiseOnlineAttendance:
                    {
                        query = @"SELECT 
                                    ATT.ATTENDANCE_ID,
                                    SP.STUDENT_ID,
                                    UPPER(SP.ROLL_NO) AS ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                    C.CLASS_NAME,
                                    C.CLASS_ID AS S_CLASS_ID,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
	                                    INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
                                    TT_ONLINE_TIMETABLE AS TT ON  TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR AND TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = SC.CLASS_ID
                                        AND TT.STAFF_ID = ?STAFF_ID
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                        LEFT JOIN
                                    ATT_ONLINE_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.STAFF_ID = TT.STAFF_ID
                                        AND ATT.COURSE_ID = CCP.COURSE_ID
                                        AND ATT.HOUR_FROM=TT.HOUR_ID
                                        AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED != 1
                                        AND SC.CLASS_ID = ?CLASS_ID
                                        AND TT.COURSE_ID = ?COURSE_ID
                                        AND SC.IS_DELETED!=1
                                        AND CCP.IS_DELETED!=1
                                        AND TT.HOUR_ID = ?HOUR_ID
                                UNION SELECT 
                                    ATT.ATTENDANCE_ID,
                                    SP.STUDENT_ID,
                                    UPPER(SP.ROLL_NO) AS ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                    C.CLASS_NAME,
                                    SCI.S_CLASS_ID,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                                        INNER JOIN
                                    STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                                        AND SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
                                    TT_ONLINE_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = SCI.S_CLASS_ID
                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                        LEFT JOIN
                                    ATT_ONLINE_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.STAFF_ID = TT.STAFF_ID
                                        AND ATT.COURSE_ID = CCP.COURSE_ID
                                        AND ATT.HOUR_FROM=TT.HOUR_ID
                                        AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SCI.S_CLASS_ID = ?CLASS_ID
                                        AND SCI.COURSE_ID = ?COURSE_ID
                                        AND TT.STAFF_ID=?STAFF_ID
                                        AND TT.IS_DELETED != 1
                                        AND SC.IS_DELETED!=1
		                                AND CCP.IS_DELETED!=1
                                        AND SCI.IS_DELETED!=1
                                        AND TT.HOUR_ID = ?HOUR_ID;";
                        break;
                    }
                #endregion

                #region Attendance Report Sem Wise
                case AttendanceSQLCommands.FetchAttendancepercentagereport:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        ROUND(COUNT(ATT.ATTENDENCE_DATE) / 5) AS ABSENT_COUNT,
                                        T.DAYS_COUNT
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.SHIFT = ?SHIFT
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                #endregion

                #region Daily Absentees SMS For SJC
                case AttendanceSQLCommands.BindRegularAbsenteesListByProgrammeHourAndClass:
                    {
                        query = @"SELECT 
                                        DATE_FORMAT( ATTENDENCE_DATE,'%d-%m-%Y') as ATTENDENCE_DATE,
                                        GROUP_CONCAT(HOUR_FROM) AS HOUR_FROM,
                                        AB.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.FIRST_NAME,
                                        CL.CLASS_NAME,
                                        CONCAT(IFNULL(SP.FR_MOBILE,
                                                        IFNULL(SP.MO_MOBILE,
                                                                IFNULL(SP.STU_MOBILENO, '')))) AS MOBILE,
                                        IF(SP.GENDER = 2, 'DAUGHTER', 'SON') AS GENDER
                                    FROM
                                        ATT_ABSENTEE_LIST AS AB
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = AB.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID IN (?CLASS_ID)
                                            AND SC.IS_DELETED!=1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
											AND SP.IS_DELETED!=1
                                            AND SP.IS_LEFT!=1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
	                                    CP_CLASSES AS CL ON CL.CLASS_ID=SC.CLASS_ID
                                    WHERE
                                        ATTENDENCE_DATE = DATE_FORMAT(NOW(),'%Y-%m-%d')
                                            AND AB.ATTENDANCE_TYPE_ID=2
                                            AND CP.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND AB.HOUR_FROM IN (?HOUR_FROM)
                                    GROUP BY AB.STUDENT_ID
                                    ORDER BY HOUR_FROM , AB.STUDENT_ID ASC;";
                        break;
                    }

                case AttendanceSQLCommands.BindOnlineAbsenteesListByProgrammeHourAndClass:
                    {
                        query = @"SELECT 
                                        DATE_FORMAT( ATTENDENCE_DATE,'%d-%m-%Y') as ATTENDENCE_DATE,
                                        GROUP_CONCAT(HOUR_FROM) AS HOUR_FROM,
                                        AB.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.FIRST_NAME,
                                        CL.CLASS_NAME,
                                        CONCAT(IFNULL(SP.FR_MOBILE,
                                                        IFNULL(SP.MO_MOBILE,
                                                                IFNULL(SP.STU_MOBILENO, '')))) AS MOBILE,
                                        IF(SP.GENDER = 2, 'DAUGHTER', 'SON') AS GENDER
                                    FROM
                                        ATT_ONLINE_ABSENTEE_LIST AS AB
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = AB.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID IN (?CLASS_ID)
                                            AND SC.IS_DELETED!=1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
											AND SP.IS_DELETED!=1
                                            AND SP.IS_LEFT!=1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
	                                    CP_CLASSES AS CL ON CL.CLASS_ID=SC.CLASS_ID
                                    WHERE
                                        ATTENDENCE_DATE = DATE_FORMAT(NOW(),'%Y-%m-%d')
                                            AND AB.ATTENDANCE_TYPE_ID=2
                                            AND CP.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND AB.HOUR_FROM IN (?HOUR_FROM)
                                    GROUP BY AB.STUDENT_ID
                                    ORDER BY HOUR_FROM , AB.STUDENT_ID ASC;";
                        break;
                    }
                #endregion

                #region Individual Student Report
                case AttendanceSQLCommands.BindIndividualStudentAttendancePercentage:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.FIRST_NAME,
                                        DATE_FORMAT(CAL.DAY_ORDER_DATE,'%d/%m/%Y') AS 'START_DATE',
                                        MONTHNAME(CAL.DAY_ORDER_DATE) AS 'MONTH',
                                        GROUP_CONCAT(ATT.HOUR_FROM) AS 'HOUR'
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                            AND CAL.IS_HOLIDAY != 1
                                            INNER JOIN
                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SP.ROLL_NO = ?ROLL_NO
                                    GROUP BY CAL.DAY_ORDER_DATE
                                    ORDER BY CAL.DAY_ID , SP.ROLL_NO;";
                        break;
                    }
                #endregion

                #region attendance percentage report common
                case AttendanceSQLCommands.FetchHostelStudentAbsentReport:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(T.START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(T.END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME,
                                        T.HOSTEL_NAME,
                                        T.ROOM_NO,
                                        T.BLOCK_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME,
                                                H.HOSTEL_NAME,
                                                HS.ROOM_NO,
                                                B.BLOCK_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SC.STUDENT_ID
                                            AND HS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND HS.IS_DELETED != 1
                                            AND HS.IS_LEFT != 1
                                        LEFT JOIN SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        LEFT JOIN SUP_BLOCK AS B ON B.BLOCK_ID = HS.BLOCK_ID
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                            AND CAL.IS_HOLIDAY != 1
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID 
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                                ?HOSTEL_CONDITION
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchHostelStudentDailyReport:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(T.START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(T.END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME,
                                        T.HOSTEL_NAME,
                                        T.ROOM_NO,
                                        T.BLOCK_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME,
                                                H.HOSTEL_NAME,
                                                HS.ROOM_NO,
                                                B.BLOCK_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN HOSTEL_STUDENT_INFO AS HS ON HS.STUDENT_ID = SC.STUDENT_ID
                                            AND HS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND HS.IS_DELETED != 1
                                            AND HS.IS_LEFT != 1
                                        LEFT JOIN SUP_HOSTEL AS H ON H.HOSTEL_ID = HS.HOSTEL_ID
                                        LEFT JOIN SUP_BLOCK AS B ON B.BLOCK_ID = HS.BLOCK_ID
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.IS_HOLIDAY != 1
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID 
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                                ?HOSTEL_CONDITION
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                    INNER JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.SEMESTER_TYPE = ?SEMESTER_TYPE
                                            AND ATT.IS_DELETED != 1
                                            AND ATT.student_id IN (SELECT 
                                                abl.STUDENT_ID
                                            FROM
                                                ATT_COMMON_ABSENTEE_LIST AS abl
                                            WHERE
                                                abl.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND abl.ATTENDANCE_TYPE_ID = 2
                                                    AND abl.IS_DELETED != 1 
                                                    AND abl.SEMESTER_TYPE = ?SEMESTER_TYPE 
                                                    AND abl.ATTENDENCE_DATE = '?START_DATE')
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchStudentAbsentDates:
                    {
                        query = @"SELECT DATE_FORMAT(ATTENDENCE_DATE, '%d/%m/%Y') AS ATTENDENCE_DATE
                                    FROM ATT_COMMON_ABSENTEE_LIST
                                    WHERE STUDENT_ID = ?STUDENT_ID
                                      AND ACADEMIC_YEAR = ?AC_YEAR
                                      AND SEMESTER_TYPE = ?SEMESTER_TYPE
                                      AND ATTENDANCE_TYPE_ID = 2
                                      AND IS_DELETED != 1
                                      AND ATTENDENCE_DATE BETWEEN ?START_DATE AND ?END_DATE
                                    ORDER BY ATTENDENCE_DATE;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAttendancepercentagereport:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.SHIFT in (?SHIFT)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAttendancepercentagereportWithMark:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                    AND SP.IS_DELETED != 1
                                                    AND SP.IS_LEFT != 1
                                                INNER JOIN 
                                            CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                    AND CAL.DAY_ORDER_DATE BETWEEN '?START_DATE' AND '?END_DATE'
                                                 -- AND CAL.DAY_TYPE=2
                                                    AND CAL.IS_HOLIDAY != 1
                                                 -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                                INNER JOIN 
                                            CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID 
                                                    AND C.SHIFT in (?SHIFT) 
                                                    AND C.CLASS_LEVEL IN (?CLASS_LEVEL)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAbsetneeListDateWise:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.SHIFT in (?SHIFT)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID  having ABSENT_COUNT !=0
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAttendancedDailyReport:
                    {

                        query = @"SELECT 
                                    T.STUDENT_ID,
                                    T.ROLL_NO,
                                    UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                    T.FR_MOBILE AS MOBILE,
                                    T.DEPARTMENT,
                                    T.CLASS_YEAR,
                                    T.CLASS_LEVEL,
                                    T.CLASS_ID,
                                    DATE_FORMAT(START_DATE, '%d/%m/%Y') START_DATE,
                                    DATE_FORMAT(END_DATE, '%d/%m/%Y') END_DATE,
                                    COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                    T.DAYS_COUNT,
                                    T.CLASS_NAME
                                FROM
                                    (SELECT 
                                        SP.STUDENT_ID,
                                            SP.ROLL_NO,
                                            SP.FIRST_NAME,
                                            SP.FR_MOBILE,
                                            C.DEPARTMENT,
                                            C.CLASS_LEVEL,
                                            SC.CLASS_ID,
                                            C.CLASS_YEAR,
                                            MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                            MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                            COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                            C.CLASS_NAME
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                    INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        -- AND CAL.DAY_ORDER_DATE BETWEEN '2024-06-19' AND '2024-07-29'
                                        AND CAL.IS_HOLIDAY != 1
                                    INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.SHIFT  in(?SHIFT) 
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.STUDENT_ID
                                    ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                        INNER JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ATT.ATTENDANCE_TYPE_ID = 2
                                        and ATT.SEMESTER_TYPE =?SEMESTER_TYPE
                                        AND ATT.IS_DELETED != 1
                                        AND att.student_id IN (SELECT 
                                            abl.STUDENT_ID
                                        FROM
                                            ATT_COMMON_ABSENTEE_LIST AS abl
                                        WHERE
                                            abl.ACADEMIC_YEAR = ?AC_YEAR
                                                AND abl.ATTENDANCE_TYPE_ID = 2
                                                AND abl.IS_DELETED != 1 
                                                and abl.SEMESTER_TYPE = ?SEMESTER_TYPE 
                                                and abl.ATTENDENCE_DATE = '?START_DATE')
                                        AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                GROUP BY T.STUDENT_ID
                                ORDER BY T.DEPARTMENT , T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAbsenteeListRangeWise:
                    {

                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME,
                                        ATT.ATTENDENCE_DATE
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                                
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                           --  AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.SHIFT in(?SHIFT)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            inner JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.SEMESTER_TYPE = ?SEMESTER_TYPE
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                          
                                    GROUP BY T.STUDENT_ID  having ABSENT_COUNT BETWEEN ?VALUE1 AND ?VALUE2
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAsbsenteesListbyClassWise:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.CLASS_ID in(?CLASS_ID)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAsbsenteesListbyStudentId:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE, '%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE, '%d/%m/%Y') END_DATE,
                                        1 as ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME,
                                        DATE_FORMAT(ATT.ATTENDENCE_DATE, '%d/%m/%Y') ATTENDENCE_DATE
                                        
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                            AND CAL.IS_HOLIDAY != 1
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND sC.STUDENT_ID IN (?STUDENT_ID)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            INNER JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    ORDER BY T.DEPARTMENT , T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchNotAbsenteeListClassWise:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                           -- AND CAL.DAY_ORDER_DATE BETWEEN '2024-06-19' AND '2024-08-03'
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND SC.CLASS_ID IN(?CLASS_ID)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.SEMESTER_TYPE IN(?SEMESTER_TYPE)
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID  having ABSENT_COUNT = 0
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                #endregion

                #region attendance percentage report For HOD

                case AttendanceSQLCommands.FetchCommonAttendancepercentagereportForHod:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.SHIFT = ?SHIFT  AND C.DEPARTMENT =?DEPARTMENT
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAbsetneeListDateWiseForHOD:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ?START_DATE AND ?END_DATE
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.SHIFT = ?SHIFT and c.DEPARTMENT =?DEPARTMENT
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID  having ABSENT_COUNT !=0
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case AttendanceSQLCommands.FetchCommonAttendancedDailyReportForHOD:
                    {

                        query = @"SELECT 
                                    T.STUDENT_ID,
                                    T.ROLL_NO,
                                    UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                    T.FR_MOBILE AS MOBILE,
                                    T.DEPARTMENT,
                                    T.CLASS_YEAR,
                                    T.CLASS_LEVEL,
                                    T.CLASS_ID,
                                    DATE_FORMAT(START_DATE, '%d/%m/%Y') START_DATE,
                                    DATE_FORMAT(END_DATE, '%d/%m/%Y') END_DATE,
                                    COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                    T.DAYS_COUNT,
                                    T.CLASS_NAME
                                FROM
                                    (SELECT 
                                        SP.STUDENT_ID,
                                            SP.ROLL_NO,
                                            SP.FIRST_NAME,
                                            SP.FR_MOBILE,
                                            C.DEPARTMENT,
                                            C.CLASS_LEVEL,
                                            SC.CLASS_ID,
                                            C.CLASS_YEAR,
                                            MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                            MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                            COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                            C.CLASS_NAME
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                    INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        -- AND CAL.DAY_ORDER_DATE BETWEEN '2024-06-19' AND '2024-07-29'
                                        AND CAL.IS_HOLIDAY != 1
                                    INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.SHIFT  =?SHIFT and c.DEPARTMENT = ?DEPARTMENT
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.STUDENT_ID
                                    ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                        INNER JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ATT.ATTENDANCE_TYPE_ID = 2
                                        and ATT.SEMESTER_TYPE =?SEMESTER_TYPE
                                        AND ATT.IS_DELETED != 1
                                        AND att.student_id IN (SELECT 
                                            abl.STUDENT_ID
                                        FROM
                                            ATT_COMMON_ABSENTEE_LIST AS abl
                                        WHERE
                                            abl.ACADEMIC_YEAR = ?AC_YEAR
                                                AND abl.ATTENDANCE_TYPE_ID = 2
                                                AND abl.IS_DELETED != 1 
                                                and abl.SEMESTER_TYPE = ?SEMESTER_TYPE 
                                                and abl.ATTENDENCE_DATE = '?START_DATE')
                                        AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                GROUP BY T.STUDENT_ID
                                ORDER BY T.DEPARTMENT , T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }

                case AttendanceSQLCommands.FetchNotAbsenteeListClassWiseForHOD:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE,'%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE,'%d/%m/%Y') END_DATE,
                                        COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                           -- AND CAL.DAY_ORDER_DATE BETWEEN '2024-06-19' AND '2024-08-03'
                                          --  AND CAL.DAY_TYPE=2
                                            AND CAL.IS_HOLIDAY != 1
                                           -- AND CAL.DAY_ORDER !='' OR CAL.DAY_ORDER!=0
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND SC.CLASS_ID IN(?CLASS_ID)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.STUDENT_ID
                                        ORDER BY C.DEPARTMENT,C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            LEFT JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.SEMESTER_TYPE IN(?SEMESTER_TYPE)
                                            AND ATT.IS_DELETED != 1
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    GROUP BY T.STUDENT_ID  having ABSENT_COUNT = 0
                                    ORDER BY T.DEPARTMENT,T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }

                #endregion

                case AttendanceSQLCommands.FetchCommonAsbsenteesListbyStudentIdforStudent:
                    {
                        query = @"SELECT 
                                        T.STUDENT_ID,
                                        T.ROLL_NO,
                                        UPPER(T.FIRST_NAME) AS FIRST_NAME,
                                        T.FR_MOBILE AS MOBILE,
                                        T.DEPARTMENT,
                                        T.CLASS_YEAR,
                                        T.CLASS_LEVEL,
                                        T.CLASS_ID,
                                        DATE_FORMAT(START_DATE, '%d/%m/%Y') START_DATE,
                                        DATE_FORMAT(END_DATE, '%d/%m/%Y') END_DATE,
                                        1 AS ABSENT_COUNT,
                                        T.DAYS_COUNT,
                                        T.CLASS_NAME,
                                        DATE_FORMAT(ATT.ATTENDENCE_DATE, '%d/%m/%Y') ATTENDENCE_DATE
                                    FROM
                                        (SELECT 
                                            SP.STUDENT_ID,
                                                SP.ROLL_NO,
                                                SP.FIRST_NAME,
                                                SP.FR_MOBILE,
                                                C.DEPARTMENT,
                                                C.CLASS_LEVEL,
                                                SC.CLASS_ID,
                                                C.CLASS_YEAR,
                                                MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                                MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                                COUNT(CAL.DAY_ORDER_DATE) AS DAYS_COUNT,
                                                C.CLASS_NAME,
                                                ASM.SEMESTER AS SEMESTER_ID,
                                                SS.SEMESTER_TYPE
                                        FROM
                                            STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                        INNER JOIN academic_semester AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ASM.BATCH = SC.BATCH
                                            AND ASM.IS_ACTIVE = 1
                                        INNER JOIN sup_semester AS SS ON SS.SUP_SEMESTER_ID = ASM.SEMESTER
                                        INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CAL.DAY_ORDER_DATE BETWEEN ASM.DATE_FROM AND  ?ENDDATE
                                            AND ASM.IS_ACTIVE = 1
                                            AND CAL.IS_HOLIDAY != 1
                                        INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND sC.STUDENT_ID IN  (?STUDENT_ID)
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                            INNER JOIN
                                        ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                            AND ATT.IS_DELETED != 1
                                            AND (ATT.SEMESTER_TYPE = T.SEMESTER_TYPE)
                                            AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                    ORDER BY T.DEPARTMENT , T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }

                case AttendanceSQLCommands.FetchSemesterEndDate:
                    {
                        query = @"SELECT 
                                            SP.STUDENT_ID, DATE_FROM,  date_format(DATE_TO,'%d/%m/%Y') as DATE_TO
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                AND SP.IS_DELETED != 1
                                                AND SP.IS_LEFT != 1
                                                INNER JOIN
                                            ACADEMIC_SEMESTER AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                                AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                AND ASM.BATCH = SC.BATCH
                                                AND ASM.IS_ACTIVE = 1
                                        WHERE
                                            SP.STUDENT_ID = ?STUDENT_ID
                                             AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case AttendanceSQLCommands.FetchSemesterEndDatebyProgramme:
                    {
                        query = @"SELECT 
                                          DATE_FROM,  date_format(DATE_TO,'%d/%m/%Y') as DATE_TO
                                        FROM
                                           
                                            ACADEMIC_SEMESTER AS ASM 
                                                
                                        WHERE
                                            ASM.PROGRAMME_GROUP_ID in(?PROGRAMME_GROUP_ID) and  ASM.IS_ACTIVE = 1
                                             AND ASM.ACADEMIC_YEAR = ?AC_YEAR group by PROGRAMME_GROUP_ID";
                        break;
                    }
                case AttendanceSQLCommands.LoadStudentsForDateWiseAttendanceEntry:
                    {
                        query = @"SELECT 
                                    ATT.ATTENDANCE_ID,
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                    C.CLASS_NAME,
                                    C.CLASS_ID AS S_CLASS_ID,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                    ATT.ATTENDANCE_TYPE_ID,
                                    TT.HOUR_ID,
                                    CONCAT(IFNULL(SP.FR_MOBILE,
                                                    IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
	                                    INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SC.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        INNER JOIN
                                    TT_TIMETABLE AS TT ON  TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR AND TT.COURSE_ID = CCP.COURSE_ID
                                        AND TT.CLASS_ID = SC.CLASS_ID
                                        AND TT.STAFF_ID = ?STAFF_ID
                                        INNER JOIN
                                    CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                        AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                        LEFT JOIN
                                    ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                        AND ATT.STAFF_ID = TT.STAFF_ID
                                        AND ATT.COURSE_ID = CCP.COURSE_ID
                                        AND ATT.HOUR_FROM=TT.HOUR_ID
                                        AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED != 1
                                        AND SC.CLASS_ID = ?CLASS_ID
                                        AND TT.COURSE_ID = ?COURSE_ID
                                        AND SC.IS_DELETED!=1
                                        AND CCP.IS_DELETED!=1
                                        AND TT.HOUR_ID = ?HOUR_ID
                                UNION SELECT 
                                            ATT.ATTENDANCE_ID,
                                            SP.STUDENT_ID,
                                            SP.ROLL_NO,
                                            SP.REGISTER_NO,
                                            CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                    ' ',
                                                    IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                            C.CLASS_NAME,
                                            SC.S_CLASS_ID,
                                            DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DOB,
                                            ATT.ATTENDANCE_TYPE_ID,
                                            TT.HOUR_ID,
                                            CONCAT(IFNULL(SP.FR_MOBILE,
                                                            IFNULL(SP.MO_MOBILE, IFNULL(STU_MOBILENO, '')))) AS 'MOBILE'
                                        FROM
                                            STU_COURSE_INFO AS SC
                                                INNER JOIN
                                            TT_TIMETABLE AS TT ON TT.CLASS_ID = SC.S_CLASS_ID
                                                AND TT.COURSE_ID = SC.COURSE_ID
                                                AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                AND TT.IS_DELETED != 1
                                                INNER JOIN
                                            CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                                    AND CAL.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                                    AND CAL.DAY_ORDER_DATE = ?ATTENDANCE_DATE
                                                INNER JOIN
                                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                                LEFT JOIN
                                            ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                                AND ATT.S_CLASS_ID = SC.S_CLASS_ID
                                                AND ATT.COURSE_ID = SC.COURSE_ID
                                                AND ATT.STAFF_ID = TT.STAFF_ID
                                                AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                AND ATT.HOUR_FROM = TT.HOUR_ID
                                                AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                                AND ATT.IS_DELETED != 1
                                                INNER JOIN
                                            CP_CLASSES AS C ON C.CLASS_ID = SC.S_CLASS_ID
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                AND SP.IS_DELETED != 1
                                                AND SP.IS_LEFT != 1
                                                LEFT JOIN
                                            SUP_REASON AS SR ON SR.REASON_ID = ATT.REASON_ID
                                                LEFT JOIN
                                            SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = ATT.ATTENDANCE_TYPE_ID
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                                AND TT.HOUR_ID = ?HOUR_ID
                                                AND TT.STAFF_ID = ?STAFF_ID
                                                AND SC.S_CLASS_ID = ?CLASS_ID
                                                AND TT.COURSE_ID = ?COURSE_ID
                                        ORDER BY ROLL_NO;";
                        break;
                    }


                #region Attendance

                case AttendanceSQLCommands.FetchAttendance:
                    {
                        query = @"SELECT 
                                      ATTENDANCE_TYPE_ID,
                                      ATTENDANCE_TYPE,
                                      IS_ACTIVE,
                                      IS_DELETED
                                      
                                  FROM
                                      sup_attendance_type
                                  WHERE
                                     IS_DELETED !=1;";
                        break;
                    }


                            case AttendanceSQLCommands.UpdateAttendance:
                                {
                                    query = @"UPDATE 
                                                sup_attendance_type
                                                SET 
                                                ATTENDANCE_TYPE=?ATTENDANCE_TYPE,
                                                IS_ACTIVE=?IS_ACTIVE

                                                WHERE ATTENDANCE_TYPE_ID=?ATTENDANCE_TYPE_ID
                                                AND IS_DELETED!=1;";
                                    break;
                               }



                            case AttendanceSQLCommands.AddAttendance:
                                {
                                    query = @"INSERT INTO sup_attendance_type
                                                (
                                                 ATTENDANCE_TYPE,
                                                 IS_ACTIVE)
                                                 
                                                VALUES
                                                (
                                                ?ATTENDANCE_TYPE,
                                                ?IS_ACTIVE);";
                                                 

                                    break;
                                }


                            case AttendanceSQLCommands.FetchAttendancbyId:
                                {
                                    query = @"SELECT
                                            ATTENDANCE_TYPE_ID,
                                            ATTENDANCE_TYPE,
                                            IS_ACTIVE,
                                            IS_DELETED
                                            

                                           FROM 
                                            sup_attendance_type

                                            WHERE ATTENDANCE_TYPE_ID=?ATTENDANCE_TYPE_ID
                                            AND IS_DELETED !=1;";
                                    break;

                                }


                            case AttendanceSQLCommands.DeleteAttendancById:
                                {
                                    query = @"UPDATE sup_attendance_type 
                                            SET 
                                            IS_DELETED=?IS_DELETED
                                            WHERE 
                                           ATTENDANCE_TYPE_ID=?ATTENDANCE_TYPE_ID";
                                    break;
                                }
                #endregion

                case AttendanceSQLCommands.InsertStudentTag:
                    {
                        query = @"  INSERT INTO STU_LEVEL_TAG_MAPPING_INFO
                                        (
                                        TAG_ID,
										STUDENT_ID,
										ACADEMIC_YEAR,
										SEMESTER_TYPE,
										CREATED_BY,
                                        REMARK
                                        )
                                        VALUES
                                        (
                                       ?TAG_ID,
                                       ?STUDENT_ID,
                                       ?ACADEMIC_YEAR,
                                       ?SEMESTER_TYPE,
                                       ?CREATED_BY,
									   ?REMARK
                                        );";
                        return query;
                    }




                case AttendanceSQLCommands.UpdateStudentTag:
                    {
                        query = @"  
                                    UPDATE STU_LEVEL_TAG_MAPPING_INFO
                                    SET
                                    TAG_ID=?TAG_ID
                                    WHERE STU_TAG_ID = ?STU_TAG_ID;";
                        return query;
                    }


                case AttendanceSQLCommands.FetchClassWiseMentorStudentList:
                    {
                        query = @"  SELECT 
                                                SPI.STUDENT_ID,
                                                SPI.ROLL_NO,
                                                SPI.FIRST_NAME AS STUDENT_NAME,
                                                STF.FIRST_NAME AS STAFF_NAME,
                                                STF.MOBILE_NO,
                                                CC.CLASS_NAME,
                                                CD.DEPARTMENT
                                            FROM
                                                STU_CLASS SC
                                                    INNER JOIN
                                                STU_PERSONAL_INFO SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                                    INNER JOIN
                                                CP_CLASSES CC ON CC.CLASS_ID = SC.CLASS_ID
                                                    INNER JOIN
                                                MENTORING_STUDENT_LIST MSL ON MSL.STUDENT_ID = SC.STUDENT_ID
                                                    LEFT JOIN
                                                STF_PERSONAL_INFO STF ON STF.STAFF_ID = MSL.STAFF_ID
                                                    LEFT JOIN
                                                CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                            WHERE
                                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND CC.CLASS_ID = ?CLASS_ID
                                                    AND SPI.IS_DELETED != 1
                                                    AND SPI.IS_LEFT != 1
                                                    order by ROLL_NO;";
                        break;
                    }

            }
            return query;
        }
    }
}
