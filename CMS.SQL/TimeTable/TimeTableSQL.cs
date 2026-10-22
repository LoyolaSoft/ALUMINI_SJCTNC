using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.TimeTable
{
   public  class TimeTableSQL
    {
        public static string GetTimeTableSQL(TimeTableSQLCommands sEnumCommand)
        {
            string query = "";


            switch (sEnumCommand)
            {
                case TimeTableSQLCommands.FetchPaparType:
                    {
                        query = @"SELECT 
                                    PAPER_TYPE_ID, PAPER_TYPE, IS_ACTIVE, IS_DELETED
                                FROM
                                    SUP_PAPER_TYPE
                                WHERE
                                    IS_ACTIVE = 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchAcademicBatches: {

                        query = @"SELECT 
                                    AC_BATCH_ID, BATCH_ID, AB.ACADEMIC_YEAR_ID, BATCH_YEAR
                                FROM
                                    ACADEMIC_BATCHES AS AB
                                        INNER JOIN
                                    ACADEMIC_YEAR AS AC ON AC.ACADEMIC_YEAR_ID = AB.ACADEMIC_YEAR_ID
                                WHERE
                                    AC.ACTIVE_YEAR = 1;";

                        break;
                    }
                case TimeTableSQLCommands.FetchTemplateOnlyStaticHours:
                    {

                        query = @"SELECT 
                                        DAY_ORDER_ID,HOUR_ID,PAPER_TYPE_ID,IS_STATIC
                                    FROM
                                        TT_TEMPLATE
                                    WHERE
                                        SETTING_ID =?SETTING_ID AND FACULTY_ID =?FACULTY_ID
                                            AND YEAR =?YEAR
                                            AND GRADUATIONTYPE =?GRADUATIONTYPE
                                            AND IS_ACTIVE =1
                                            AND IS_DELETED !=1
                                            AND IS_STATIC =?IS_STATIC";

                        break;
                    }
                case TimeTableSQLCommands.FetchSettingByAcademicYear:
                    {

                        query = @"SELECT 
                                    SETTING_ID,
                                    SETTING_NAME,
                                    ALLOW_STATIC_HOUR,
                                    NO_OF_HOURS,
                                    NO_OF_DAYS,
                                    BASIC_HOURS_FOR_HOD,
                                    BASIC_HOURS_FOR_STAFF,
                                    SEMESTER_TYPE,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED,
                                    TEMPLATE_ID
                                FROM
                                    TT_TEMPLATE_SETTING
                                WHERE
                                    IS_ACTIVE = 1 AND ACADEMIC_YEAR = ?ACADEMIC_YEAR;";

                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableSettings:
                    {

                        query = @"SELECT 
                                        SETTING_ID,
                                        SETTING_NAME,
                                        ALLOW_STATIC_HOUR,
                                        NO_OF_HOURS,
                                        NO_OF_DAYS,
                                        BASIC_HOURS_FOR_HOD,
                                        BASIC_HOURS_FOR_STAFF,
                                        SEMESTER_TYPE,
                                        ACADEMIC_YEAR,
                                        IS_ACTIVE,
                                        IS_DELETED,
                                        TEMPLATE_ID,
                                        GRADUATION_TYPE
                                    FROM
                                        TT_TEMPLATE_SETTING AS STG
                                    WHERE
                                        STG.IS_ACTIVE =?IS_ACTIVE
                                            AND STG.ACADEMIC_YEAR =?ACADEMIC_YEAR;";

                        break;
                    }
                case TimeTableSQLCommands.FetchDepartmentBySettingId:
                    {
                        query = @"SELECT 
                                                    DEPARTMENT_TEMPLATE_ID,
                                                    SETTING_ID,
                                                    DEPARTMENT_ID,
                                                    IS_ACTIVE,
                                                    IS_DELETED,
                                                    Academic_Year
                                                FROM
                                                    TT_DEPARTMENT_WISE_TEMPLATE AS DP
                                                WHERE
                                                    DP.SETTING_ID =?SETTING_ID AND DP.ACADEMIC_YEAR=?ACADEMIC_YEAR AND DP.IS_DELETED!=1;";

                        break;
                    }
                case TimeTableSQLCommands.FetchProgrammesByDepartmentAndGraduationType:
                    {

                        query = @"";

                        break;
                    }
                case TimeTableSQLCommands.FetchAcademicCurrilum:
                    {

                        query = @"";

                        break;
                    }


                case TimeTableSQLCommands.SaveTimeTable:
                    {

                        query = @"INSERT INTO TT_TIMETABLE
                                            (
                                            Course_Id,
                                            Day_Order_Id,
                                            Staff_Id,
                                            Class_Id,
                                            Hour_Id,
                                            Semester_Id,
                                            Hour_Type,
                                            ACADEMIC_YEAR)
                                            VALUES
                                            (
                                            ?COURSE_ID,
                                            ?DAY_ORDER_ID,
                                            ?STAFF_ID,
                                            ?CLASS_ID,
                                            ?HOUR_ID,
                                            ?SEMESTER_ID,
                                            ?HOURTYPE,
                                            ?AC_YEAR);";

                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableClassWiseListByClassId:
                    {
                       
                        query = @"SELECT 
                                    TT.Time_Table_Id,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    CONCAT(IFNULL(ST.FIRST_NAME, ''),' ',
                                                IFNULL(ST.LAST_NAME, ''),
                                                '(',
                                                ST.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                    TT.Day_Order_Id,
                                    TT.Hour_Id,
                                    SHT.hourType,
                                    TT.Semester_Id,
                                    SHT.hour_type_id,
                                    tt.Course_Id,
                                    tt.Staff_Id,
                                    tt.Class_Id
                                FROM
                                    TT_TIMETABLE AS TT
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID AND CR.IS_DELETED!=1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = TT.STAFF_ID AND ST.IS_DELETED!=1
                                        left JOIN
                                    SUP_HOUR_TYPE SHT ON SHT.HOUR_TYPE_ID = TT.HOUR_TYPE
										INNER JOIN
									CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
										AND CCS.COURSE_ID=TT.COURSE_ID
										AND CCS.STAFF_ID=TT.STAFF_ID
                                        AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                        AND CCS.IS_DELETED!=1
                                        INNER JOIN
									SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
										AND S.IS_ACTIVE=1
                                WHERE
                                    TT.CLASS_ID IN(?CLASS_ID) AND TT.IS_DELETED != 1
                                    AND CR.UGC_COURSE_TYPE!=48
                                    AND TT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchHoursByTimeTableHours:
                    {
                        query = @"SELECT 
                                        SH.HOUR_ID, SH.HOUR
                                    FROM
                                        SUP_HOURS AS SH
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.HOUR_ID = SH.HOUR_ID
                                    GROUP BY SH.HOUR_ID;";
                        break;
                    }
                case TimeTableSQLCommands.CheckIsRegularPaper:
                    {
                        query = @"SELECT 
                                        Time_Table_Id
                                    FROM
                                        tt_timetable_?AC_YEAR AS tt
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS cls ON cls.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        ACADEMIC_SEMESTER_?AC_YEAR AS ase ON ase.PROGRAMME = cls.PROGRAMME
                                            AND ase.IS_ACTIVE = 1
                                            AND ase.BATCH = (SELECT 
                                                sp.batch
                                            FROM
                                                STU_CLASS AS sc
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS sp ON sp.STUDENT_ID = sc.STUDENT_ID
                                            WHERE
                                                sc.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND sc.CLASS_ID = ?CLASS_ID
                                            GROUP BY sp.BATCH)
                                    WHERE
                                        tt.Hour_Id = ?HOUR_ID AND tt.Day_Order_Id = ?DAY_ORDER_ID
                                            AND tt.Class_Id = ?CLASS_ID
                                            AND tt.Semester_Id = ase.SEMESTER AND tt.Is_Deleted!=1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableById:
                    {
                        query= @"SELECT 
                                        tt.Time_Table_Id,
                                        tt.Course_Id,
                                        tt.Day_Order_Id,
                                        tt.Staff_Id,
                                        tt.Class_Id,
                                        cls.CLASS_CODE,
                                        tt.Hour_Id,
                                        tt.Semester_Id,
                                        tt.Hour_Type
                                    FROM
                                        tt_timetable AS tt
                                            INNER JOIN
                                        cp_classes AS cls ON cls.CLASS_ID = tt.Class_Id AND cls.IS_DELETED != 1
                                    WHERE
                                        tt.Time_Table_Id = ?TIME_TABLE_ID 
                                        AND tt.Is_Deleted != 1 AND tt.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchOnlyRegularCourseByClassId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                '(',
                                                IFNULL(CR.COURSE_CODE, ''),
                                                ')') AS COURSE_TITLE
                                    FROM
                                        ACADEMIC_CURRICULUM_?AC_YEAR AS AC
                                            INNER JOIN
                                        CP_COURSE_ROOT_?AC_YEAR AS CR ON CR.COURSE_ROOT_ID = AC.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.CLASS_ID = ?CLASS_ID
                                    WHERE
                                        CR.IS_DELETED != 1
                                            AND AC.PROGRAMME = CLS.PROGRAMME
                                            AND AC.BATCH = (SELECT 
                                                SP.BATCH
                                            FROM
                                                STU_CLASS AS SC
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            WHERE
                                                SC.CLASS_ID = ?CLASS_ID
                                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            GROUP BY SP.BATCH)
                                            AND AC.SEMESTER = (SELECT 
                                                SEMESTER
                                            FROM
                                                ACADEMIC_SEMESTER_?AC_YEAR
                                            WHERE
                                                BATCH = AC.BATCH
                                                    AND PROGRAMME = CLS.PROGRAMME
                                                    AND IS_ACTIVE = 1) AND AC.IS_DELETED!=1 AND CR.IS_COMPULSORY=1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchStaffByCourseIdAndClassId:
                    {
                        query = @"SELECT 
                                    ST.STAFF_ID,
                                    ST.STAFF_CODE,
                                    CONCAT(IFNULL(FIRST_NAME, ''),
                                            '',
                                            ' ',
                                            IFNULL(LAST_NAME, ''),
                                            '(',
                                            STAFF_CODE,
                                            ')') AS FIRST_NAME
                                FROM
                                    STF_PERSONAL_INFO AS ST
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = ST.STAFF_ID
		                                INNER JOIN
	                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=CCS.COURSE_ID
                                WHERE
                                    CCS.COURSE_ID = ?COURSE_ID AND CCS.IS_DELETED!=1
                                        AND CCS.CLASS_ID = ?CLASS_ID AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                        AND CCR.SEMESTER_ID IN (?SEMESTER_ID)
                                GROUP BY ST.STAFF_CODE;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateTimeTableInfo:
                    {
                        query = @"UPDATE tt_timetable
                                            SET
                                            Course_Id= ?COURSE_ID,
                                            Day_Order_Id= ?DAY_ORDER_ID,
                                            Staff_Id= ?STAFF_ID,
                                            Class_Id= ?CLASS_ID,
                                            Hour_Id= ?HOUR_ID,
                                            Semester_Id= ?SEMESTER_ID,
                                            Hour_Type= ?HOURTYPE
                                            WHERE Time_Table_Id= ?TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.IsTimeTableInfoExisting:
                    {
                        query = @"SELECT 
                                        tt.Time_Table_Id
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ?CLASS_ID
											INNER JOIN
										ACADEMIC_SEMESTER AS ASE ON ASE.PROGRAMME_GROUP_ID=CLS.PROGRAMME_GROUP_ID
                                        AND ASE.BATCH IN (SELECT 
                                                        SP.BATCH
                                                    FROM
                                                        STU_CLASS AS SC
                                                            INNER JOIN
                                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                    WHERE
                                                        SC.CLASS_ID = ?CLASS_ID
                                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                                    GROUP BY SP.BATCH)
                                                    AND ASE.IS_ACTIVE = 1
                                    WHERE
                                        tt.Hour_Id = ?HOUR_ID AND tt.Day_Order_Id = ?DAY_ORDER_ID
                                            AND tt.Class_Id = ?CLASS_ID
                                            AND tt.Is_Deleted != 1;";
                        break;
                    }
                case TimeTableSQLCommands.CheckIsStaffAlloted:
                    {
                        query = @"SELECT 
                                        tt.Time_Table_Id,tt.Course_Id,C.CLASS_NAME AS CLASS_CODE
                                    FROM
                                        TT_TIMETABLE AS TT 
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
										CP_CLASSES AS C ON C.CLASS_ID=TT.CLASS_ID
                                            INNER JOIN
	                                    CP_CLASSES AS CC ON CC.CLASS_ID=?CLASS_ID
		                                    AND CC.SHIFT=C.SHIFT
                                            INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
                                            AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.STAFF_ID=TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
                                    SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
                                             AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.HOUR_ID = ?HOUR_ID AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID 
                                            AND TT.STAFF_ID = ?STAFF_ID AND TT.ACADEMIC_YEAR=?AC_YEAR
                                            AND TT.IS_DELETED != 1 GROUP BY TT.COURSE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchOptinalHourByClassIdAndHourIdAndCourseId:
                    {
                        query = @"SELECT 
                                        TT.TIME_TABLE_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
											INNER JOIN
										CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
											AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.STAFF_ID=TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
										SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.CLASS_ID =?CLASS_ID AND TT.ACADEMIC_YEAR=?AC_YEAR
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCourseByClassId:
                    {
                        
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        CONCAT(CR.COURSE_TITLE, ' (', COURSE_CODE, ')') AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_ROOT AS CR
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND CCS.IS_DELETED != 1
                                            AND CCS.ACADEMIC_YEAR = CR.ACADEMIC_YEAR 
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        CCS.CLASS_ID = ?CLASS_ID
                                            AND CR.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchStaffByCourseId:
                    {
                        query = @"SELECT 
                                        ST.STAFF_ID,
                                        ST.STAFF_CODE,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                '(',
                                                STAFF_CODE,
                                                ')') AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO AS ST
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = ST.STAFF_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=CCS.COURSE_ID
                                    WHERE
                                        CCS.COURSE_ID = ?COURSE_ID 
                                        AND CCS.IS_DELETED!=1
                                        AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                        AND CCS.ACADEMIC_YEAR=?AC_YEAR 
                                        GROUP BY ST.STAFF_CODE;";
                        break;
                    }
                case TimeTableSQLCommands.FetchSelectedClassIdByCourseIdAndClassId:
                    {
                        query = @"SELECT 
                                        S_CLASS_ID
                                    FROM
                                        STU_COURSE_INFO
                                    WHERE
                                        CLASS_ID = ?CLASS_ID AND COURSE_ID = ?COURSE_ID
										AND ACADEMIC_YEAR=?ACADEMIC_YEAR
                                    GROUP BY S_CLASS_ID;";
                        break;
                    }
                case TimeTableSQLCommands.IsTimeTableInfoExistingForOptionalPaper:
                    {
                        query = @"SELECT 
                                        TT.TIME_TABLE_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                    WHERE
                                        TT.HOUR_ID = ?HOUR_ID AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.CLASS_ID = ?CLASS_ID
                                            AND TT.COURSE_ID = ?COURSE_ID
                                            AND TT.IS_DELETED != 1
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchHourType:
                    {
                        query = @"SELECT 
                                        TT.Paper_Type_Id
                                    FROM
                                        TT_TEMPLATE AS TT
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_LEVEL = TT.GRADUATIONTYPE
                                            AND CLS.CLASS_YEAR = TT.YEAR
                                            INNER JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = CLS.DEPARTMENT
                                            INNER JOIN
                                        CP_FACULTY AS CF ON CF.FACULTY_ID = DT.FACULTY
                                    WHERE
                                        CLS.CLASS_ID = ?CLASS_ID AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.FACULTY_ID = CF.FACULTY_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchAllTimeTableSettings:
                    {
                        query = @"SELECT 
                                        TTS.SETTING_ID,
                                        TTS.SETTING_NAME,
                                        TTS.NO_OF_DAYS,
                                        TTS.NO_OF_HOURS,
                                        TTS.BASIC_HOURS_FOR_HOD,
                                        TTS.BASIC_HOURS_FOR_STAFF,
                                        TTS.SEMESTER_TYPE,
                                        SS.SEMESTER_NAME,
                                        TTS.ACADEMIC_YEAR,
                                        TTS.ALLOW_STATIC_HOUR,
                                        TTS.GRADUATION_TYPE,
                                        SCL.CLASSLEVEL
                                    FROM
                                        TT_TEMPLATE_SETTING AS TTS
                                            INNER JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = TTS.SEMESTER_TYPE
                                            LEFT JOIN
                                        SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = TTS.GRADUATION_TYPE
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TTS.NO_OF_HOURS
                                    WHERE
                                        TTS.IS_DELETED != 1 AND TTS.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.SaveTimeTableSettings:
                    {
                        query = @"INSERT INTO TT_TEMPLATE_SETTING
                                            (
                                            SETTING_NAME,
                                            ALLOW_STATIC_HOUR,
                                            NO_OF_HOURS,
                                            NO_OF_DAYS,
                                            BASIC_HOURS_FOR_HOD,
                                            BASIC_HOURS_FOR_STAFF,
                                            IS_ACTIVE,
                                            SEMESTER_TYPE,
                                            ACADEMIC_YEAR,
                                            GRADUATION_TYPE)
                                            VALUES
                                            (
                                            ?SETTING_NAME,
                                            ?ALLOW_STATIC_HOUR,
                                            ?NO_OF_HOURS,
                                            ?NO_OF_DAYS,
                                            ?BASIC_HOURS_FOR_HOD,
                                            ?BASIC_HOURS_FOR_STAFF,
                                            1,
                                            ?SEMESTER_TYPE,
                                            ?ACADEMIC_YEAR,
                                            ?GRADUATION_TYPE);";
                        break;
                    }
                case TimeTableSQLCommands.IsTimeTableSettingExisting:
                    {
                        query = @"SELECT 
                                        SETTING_ID
                                    FROM
                                        TT_TEMPLATE_SETTING
                                    WHERE
                                        ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND GRADUATION_TYPE = ?GRADUATION_TYPE
                                            AND SEMESTER_TYPE = ?SEMESTER_TYPE
                                            AND SETTING_NAME = ?SETTING_NAME;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableSettingById:
                    {
                        query = @"SELECT 
                                        SETTING_NAME,
                                        ALLOW_STATIC_HOUR,
                                        NO_OF_HOURS,
                                        NO_OF_DAYS,
                                        BASIC_HOURS_FOR_HOD,
                                        BASIC_HOURS_FOR_STAFF,
                                        SEMESTER_TYPE,
                                        ACADEMIC_YEAR,
                                        GRADUATION_TYPE
                                    FROM
                                        TT_TEMPLATE_SETTING
                                    WHERE
                                        SETTING_ID = ?SETTING_ID;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateTimeTableSettings:
                    {
                        query = @"UPDATE TT_TEMPLATE_SETTING
                                        SET
                                        SETTING_NAME= ?SETTING_NAME,
                                        ALLOW_STATIC_HOUR= ?ALLOW_STATIC_HOUR,
                                        NO_OF_HOURS= ?NO_OF_HOURS,
                                        NO_OF_DAYS= ?NO_OF_DAYS,
                                        BASIC_HOURS_FOR_HOD= ?BASIC_HOURS_FOR_HOD,
                                        BASIC_HOURS_FOR_STAFF= ?BASIC_HOURS_FOR_STAFF,
                                        SEMESTER_TYPE= ?SEMESTER_TYPE,
                                        ACADEMIC_YEAR= ?ACADEMIC_YEAR,
                                        GRADUATION_TYPE= ?GRADUATION_TYPE
                                        WHERE SETTING_ID= ?SETTING_ID;";
                        break;
                    }
                case TimeTableSQLCommands.DeleteTimeTableSettings:
                    {
                        query = @"UPDATE TT_TEMPLATE_SETTING
                                        SET
                                        IS_DELETED=1
                                        WHERE SETTING_ID=?SETTING_ID";
                        break;
                    }
                case TimeTableSQLCommands.FetchDepartmentWiseTimetableTemplate:
                    {
                        query = @"SELECT 
                                        TDW.DEPARTMENT_TEMPLATE_ID,
                                        TTS.SETTING_NAME,
                                        DT.DEPARTMENT,
                                        TDW.IS_ACTIVE,
                                        TDW.Academic_Year
                                    FROM
                                        TT_DEPARTMENT_WISE_TEMPLATE AS TDW
                                            INNER JOIN
                                        TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TDW.SETTING_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = TDW.DEPARTMENT_ID
                                    WHERE
                                        TDW.IS_DELETED != 1 AND TDW.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.DeleteDepartmentWiseTimeTableTemplate:
                    {
                        query = @"UPDATE TT_DEPARTMENT_WISE_TEMPLATE
                                        SET
                                        IS_DELETED=1
                                        WHERE DEPARTMENT_TEMPLATE_ID=?DEPARTMENT_TEMPLATE_ID";
                        break;
                    }
                case TimeTableSQLCommands.IsDepartmentWiseTimeTableTemplateExisting:
                    {
                        query = @"SELECT 
                                        DEPARTMENT_TEMPLATE_ID
                                    FROM
                                        TT_DEPARTMENT_WISE_TEMPLATE
                                    WHERE
                                        DEPARTMENT_ID = ?DEPARTMENT_ID AND IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.SaveDepartmentWiseTimeTableTemplate:
                    {
                        query = @"INSERT INTO TT_DEPARTMENT_WISE_TEMPLATE
                                        (
                                        SETTING_ID,
                                        DEPARTMENT_ID,
                                        ACADEMIC_YEAR,
                                        IS_ACTIVE)
                                        VALUES
                                        (
                                        ?SETTING_ID,
                                        ?DEPARTMENT_ID,
                                        ?AC_YEAR,
                                        ?IS_ACTIVE);";
                        break;
                    }
                case TimeTableSQLCommands.FetchDepartmentWiseTimetableTemplateById:
                    {
                        query = @"SELECT 
                                        DEPARTMENT_TEMPLATE_ID,
                                        SETTING_ID,
                                        DEPARTMENT_ID,
                                        IS_ACTIVE,
                                        Academic_Year
                                    FROM
                                        TT_DEPARTMENT_WISE_TEMPLATE
                                    WHERE
                                        DEPARTMENT_TEMPLATE_ID = ?DEPARTMENT_TEMPLATE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateDepartmentWiseTimeTableTemplate:
                    {
                        query = @"UPDATE TT_DEPARTMENT_WISE_TEMPLATE
                                        SET
                                        SETTING_ID= ?SETTING_ID,
                                        DEPARTMENT_ID= ?DEPARTMENT_ID,
                                        IS_ACTIVE= ?IS_ACTIVE,
                                        ACADEMIC_YEAR= ?ACADEMIC_YEAR
                                        WHERE DEPARTMENT_TEMPLATE_ID= ?DEPARTMENT_TEMPLATE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableTemplate:
                    {
                        query = @"SELECT 
                                        TTT.TT_Temp_Id,
                                        SDO.DAY,
                                        SH.HOUR,
                                        SPT.PAPER_TYPE,
                                        CF.FACULTY,
                                        TTS.SETTING_NAME,
                                        SCY.CLASS_YEAR,
                                        SCL.CLASSLEVEL
                                    FROM
                                        TT_TEMPLATE AS TTT
                                            INNER JOIN
                                        SUP_DAY_ORDER AS SDO ON SDO.DAY_ORDER_ID = TTT.DAY_ORDER_ID
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TTT.HOUR_ID
                                            INNER JOIN
                                        SUP_PAPER_TYPE AS SPT ON SPT.PAPER_TYPE_ID = TTT.PAPER_TYPE_ID
                                            INNER JOIN
                                        CP_FACULTY_?AC_YEAR AS CF ON CF.FACULTY_ID = TTT.FACULTY_ID
                                            INNER JOIN
                                        TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TTT.SETTING_ID
                                            INNER JOIN
                                        SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = TTT.GRADUATIONTYPE
		                                    INNER JOIN
	                                    SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID=TTT.YEAR
                                    WHERE
                                        TTT.IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.IsTimeTableTemplateExisting:
                    {
                        query = @"SELECT 
                                        TT_Temp_Id
                                    FROM
                                        TT_TEMPLATE
                                    WHERE
                                        HOUR_ID = ?HOUR_ID AND DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND FACULTY_ID = ?FACULTY_ID
                                            AND SETTING_ID = ?SETTING_ID
                                            AND YEAR = ?YEAR
                                            AND GRADUATIONTYPE = ?GRADUATIONTYPE
                                            AND IS_DELETED!=1;";
                        break;
                    }
                case TimeTableSQLCommands.SaveTimeTableTemplate:
                    {
                        query = @"INSERT INTO TT_TEMPLATE
                                            (
                                            DAY_ORDER_ID,
                                            HOUR_ID,
                                            PAPER_TYPE_ID,
                                            IS_STATIC,
                                            FACULTY_ID,
                                            SETTING_ID,
                                            YEAR,
                                            GRADUATIONTYPE,
                                            ACADEMIC_YEAR)
                                            VALUES
                                            (
                                            ?DAY_ORDER_ID,
                                            ?HOUR_ID,
                                            ?PAPER_TYPE_ID,
                                            ?IS_STATIC,
                                            ?FACULTY_ID,
                                            ?SETTING_ID,
                                            ?YEAR,
                                            ?GRADUATIONTYPE,
                                            ?AC_YEAR);";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableTemplateById:
                    {
                        query = @"SELECT 
                                        TT_Temp_Id,
                                        Day_Order_Id,
                                        Hour_Id,
                                        Paper_Type_Id,
                                        Is_Static,
                                        Faculty_Id,
                                        Setting_Id,
                                        Year,
                                        GraduationType
                                    FROM
                                        TT_TEMPLATE
                                    WHERE
                                        TT_TEMP_ID = ?TT_TEMP_ID;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateTimeTableTemplate:
                    {
                        query = @"UPDATE TT_TEMPLATE
                                        SET
                                        DAY_ORDER_ID=?DAY_ORDER_ID,
                                        HOUR_ID=?HOUR_ID,
                                        PAPER_TYPE_ID=?PAPER_TYPE_ID,
                                        IS_STATIC=?IS_STATIC,
                                        FACULTY_ID=?FACULTY_ID,
                                        SETTING_ID=?SETTING_ID,
                                        YEAR=?YEAR,
                                        GRADUATIONTYPE=?GRADUATIONTYPE
                                        WHERE TT_TEMP_ID =?TT_TEMP_ID;";
                        break;
                    }
                case TimeTableSQLCommands.DeleteTimeTableTemplate:
                    {
                        query = @"UPDATE TT_TEMPLATE
                                        SET
                                        IS_DELETED=1
                                        WHERE TT_TEMP_ID=?TT_TEMP_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchIsStaticAndPaperTypeByClassId:
                    {
                        query = @"SELECT 
                                        TTT.IS_STATIC, TTT.Paper_Type_Id
                                    FROM
                                        CP_CLASSES AS CLS
                                            INNER JOIN
                                        SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = CLS.CLASS_LEVEL
                                            INNER JOIN
                                        SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID = CLS.CLASS_YEAR
                                            INNER JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = CLS.DEPARTMENT
                                            INNER JOIN
                                        CP_FACULTY AS CF ON CF.FACULTY_ID = DT.FACULTY
                                            INNER JOIN
                                        TT_TEMPLATE AS TTT ON TTT.GRADUATIONTYPE = CLS.CLASS_LEVEL
                                            AND TTT.YEAR = CLS.CLASS_YEAR AND TTT.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        TTT.DAY_ORDER_ID = ?DAY_ORDER_ID AND TTT.HOUR_ID = ?HOUR_ID
                                            AND CLS.CLASS_ID = ?CLASS_ID AND TTT.FACULTY_ID=CF.FACULTY_ID AND TTT.IS_DELETED!=1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableInfoForStaffByClassId:
                    {
                       
                        query = @"SELECT 
                                        TT.Time_Table_Id,
                                        TT.Course_Id,
                                        TT.Day_Order_Id,
                                        TT.Staff_Id,
                                        TT.Class_Id,
                                        TT.Hour_Id,
                                        SHT.hour_type_id,
                                        SHT.hourType,
                                        CR.Semester_Id,
                                        CLS.CLASS_CODE,
                                        CLS.PROGRAMME_GROUP_ID,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOUR_TYPE SHT ON SHT.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CC ON CC.COURSE_ID = TT.COURSE_ID
                                            AND CC.CLASS_ID = TT.CLASS_ID
                                            AND CC.STAFF_ID = TT.STAFF_ID
                                            AND CC.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CC.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CC.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID AND TT.CLASS_ID = ?CLASS_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1
                                    GROUP BY TT.TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTotalHoursAndDaysByClassId:
                    {
                        query = @"SELECT 
                                        TTS.NO_OF_DAYS, TTS.NO_OF_HOURS
                                    FROM
                                        CP_CLASSES AS CLS
                                            INNER JOIN
                                        TT_DEPARTMENT_WISE_TEMPLATE AS TTD ON TTD.DEPARTMENT_ID = CLS.DEPARTMENT
                                            INNER JOIN
                                        TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TTD.SETTING_ID
                                            AND TTD.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TTS.ACADEMIC_YEAR = ?AC_YEAR and TTS.IS_ACTIVE=1
                                    WHERE
                                        CLS.CLASS_ID = ?CLASS_ID
                                    GROUP BY CLS.CLASS_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchHoursByNoOfHours:
                    {
                        query = @"SELECT 
                                        HOUR_ID, HOUR
                                    FROM
                                        SUP_HOURS AS SH
                                    ORDER BY HOUR ASC
                                    LIMIT ?NO_OF_HOURS;";
                        break;
                    }
                case TimeTableSQLCommands.CheckIsRegularPaperByStaffId:
                    {
                        query = @"SELECT 
                                        Time_Table_Id
                                    FROM
                                        TT_TIMETABLE_?AC_YEAR AS TT
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        ACADEMIC_SEMESTER_?AC_YEAR AS ASE ON ASE.PROGRAMME = CLS.PROGRAMME
                                            AND ASE.IS_ACTIVE = 1
                                            AND ASE.BATCH = (SELECT 
                                                SP.BATCH
                                            FROM
                                                STU_CLASS AS SC
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            WHERE
                                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND SC.CLASS_ID = ?CLASS_ID
                                            GROUP BY SP.BATCH)
                                    WHERE
                                        TT.HOUR_ID = ?HOUR_ID AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.CLASS_ID = ?CLASS_ID AND TT.STAFF_ID=?STAFF_ID
                                            AND TT.SEMESTER_ID = ASE.SEMESTER AND TT.IS_DELETED!=1;";
                        break;
                    }
                case TimeTableSQLCommands.DeleteTimeTable:
                    {
                        query = @"UPDATE TT_TIMETABLE 
                                        SET 
                                            IS_DELETED = 1
                                        WHERE
                                            TIME_TABLE_ID = ?TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableInfoForStaffByClassIdOnlyOptionalPapers:
                    {
                        //query = @"SELECT 
                        //                TT.Time_Table_Id,
                        //                TT.Course_Id,
                        //                TT.Day_Order_Id,
                        //                TT.Staff_Id,
                        //                TT.Class_Id,
                        //                TT.Hour_Id,
                        //                TT.hour_Type AS hour_type_id,
                        //                CR.Semester_Id,
                        //                CLS.CLASS_CODE,
                        //                CLS.PROGRAMME,
                        //                CONCAT('(',
                        //                        IFNULL(CR.COURSE_CODE, ''),
                        //                        ') ',
                        //                        IFNULL(CR.COURSE_TITLE, '')) AS COURSE_CODE
                        //            FROM
                        //                TT_TIMETABLE AS TT
                        //                    INNER JOIN
                        //                CP_CLASS_COURSE_STAFF AS CC ON CC.COURSE_ID = TT.COURSE_ID
                        //                    AND CC.CLASS_ID = TT.CLASS_ID
                        //                    AND CC.STAFF_ID = TT.STAFF_ID
                        //                    INNER JOIN
                        //                CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CLS.COURSE_ID
                        //            WHERE
                        //                TT.STAFF_ID = ?STAFF_ID AND TT.CLASS_ID = ?CLASS_ID
                        //                    AND TT.IS_DELETED != 1
                        //                    AND CC.IS_DELETED != 1
                        //                    AND CR.IS_DELETED != 1;";
                        query = @"SELECT 
                                    TT.Time_Table_Id,
                                    TT.Course_Id,
                                    TT.Day_Order_Id,
                                    TT.Staff_Id,
                                    TT.Class_Id,
                                    TT.Hour_Id,
                                    TT.hour_Type AS hour_type_id,
                                    CR.Semester_Id,
                                    CLS.CLASS_CODE,
                                    CONCAT('(',
                                            IFNULL(CR.COURSE_CODE, ''),
                                            ') ',
                                            IFNULL(CR.COURSE_TITLE, '')) AS COURSE_CODE
                                FROM
                                    TT_TIMETABLE AS TT
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CC ON CC.STAFF_ID = TT.STAFF_ID
                                        AND CC.IS_DELETED != 1
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                        AND CLS.IS_DELETED != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        AND CR.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    TT.STAFF_ID = ?STAFF_ID AND TT.CLASS_ID = ?CLASS_ID
                                        AND TT.IS_DELETED != 1
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY TT.TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchDepartmentByTimeTableSettingId:
                    {
                        query = @"SELECT 
                                        DT.DEPARTMENT_ID,
                                        DT.DEPARTMENT,
                                        IF(DWT.DEPARTMENT_ID IS NOT NULL,
                                            'SELECTED',
                                            '') AS STATUS
                                    FROM
                                        CP_DEPARTMENT AS DT
                                            LEFT JOIN
                                        TT_DEPARTMENT_WISE_TEMPLATE AS DWT ON DWT.DEPARTMENT_ID = DT.DEPARTMENT_ID
                                            AND DWT.SETTING_ID = ?SETTING_ID AND DWT.IS_DELETED!=1
                                    WHERE
                                        DT.IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchHoursBySettingHours:
                    {
                        query = @"SELECT 
                                        HOUR_ID, HOUR
                                    FROM
                                        SUP_HOURS
                                    WHERE
                                        HOUR_ID <= ?NO_OF_HOURS AND IS_DELETED!=1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchDaysBySettingDays:
                    {
                        query = @"SELECT 
                                    DAY_ORDER_ID, DAY
                                FROM
                                    SUP_DAY_ORDER
                                WHERE
                                    DAY_ORDER_ID <= ?NO_OF_DAYS;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableTemplateBySettingIdAndYear:
                    {
                        query = @"SELECT 
                                        TTT.TT_Temp_Id,
                                        TTT.Day_Order_Id,
                                        TTT.Hour_Id,
                                        SPT.PAPER_TYPE,
                                        TTT.Paper_Type_Id,
                                        TTT.Is_Static,
                                        CF.FACULTY,
                                        TTS.SETTING_NAME,
                                        SCY.CLASS_YEAR,
                                        SCL.CLASSLEVEL
                                    FROM
                                        TT_TEMPLATE AS TTT
                                            INNER JOIN
                                        SUP_DAY_ORDER AS SDO ON SDO.DAY_ORDER_ID = TTT.DAY_ORDER_ID
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TTT.HOUR_ID
                                            INNER JOIN
                                        SUP_PAPER_TYPE AS SPT ON SPT.PAPER_TYPE_ID = TTT.PAPER_TYPE_ID
                                            INNER JOIN
                                        CP_FACULTY AS CF ON CF.FACULTY_ID = TTT.FACULTY_ID
                                            INNER JOIN
                                        TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TTT.SETTING_ID
                                            INNER JOIN
                                        SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = TTT.GRADUATIONTYPE
		                                    INNER JOIN
	                                    SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID=TTT.YEAR
                                    WHERE
                                        TTT.IS_DELETED != 1 AND TTT.SETTING_ID=?SETTING_ID AND TTT.YEAR=?YEAR AND TTT.GRADUATIONTYPE=?GRADUATIONTYPE AND TTT.FACULTY_ID=?FACULTY_ID AND TTT.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchSelectedClassByClassId:
                    {
                        query = @"SELECT 
                                        S_CLASS_ID
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
											INNER JOIN
										STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
											AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                            INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
											AND SEM.SEMESTER=CR.SEMESTER_ID
											AND SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                            AND SEM.IS_ACTIVE=1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR AND SC.CLASS_ID = ?CLASS_ID
                                            AND SC.CLASS_ID != SC.S_CLASS_ID
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.S_CLASS_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableInfoForEdit:
                    {
                        query = @"SELECT 
	                                    ttt.Paper_Type_Id,
                                        ttt.Hour_Id,
                                        ttt.Day_Order_Id,
                                        ttt.GraduationType,
                                        ttt.Faculty_Id,
                                        cls.Class_Id,
                                        ttt.IS_STATIC,
                                        ttt.Setting_Id
                                    FROM
                                        cp_classes AS cls
                                            INNER JOIN
                                        tt_department_wise_template AS dwt ON dwt.department_id = cls.DEPARTMENT
                                            INNER JOIN
                                        tt_template AS ttt ON ttt.Setting_Id = dwt.setting_id
                                            AND cls.CLASS_YEAR = ttt.Year
                                            AND cls.CLASS_LEVEL = ttt.GraduationType
                                    WHERE
                                        cls.CLASS_ID = ?CLASS_ID AND ttt.Is_Deleted != 1
                                            AND dwt.is_deleted != 1
                                            AND ttt.Hour_Id = ?HOUR_ID
                                            AND ttt.Day_Order_Id = ?DAY_ORDER_ID
                                            AND ttt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND dwt.Academic_Year = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableByClassIdAndHourIdAndDayId:
                    {
                        query = @"SELECT 
                                        TT.Time_Table_Id,
                                        CR.COURSE_CODE,
                                        ST.STAFF_CODE,
                                        TT.Day_Order_Id,
                                        TT.Hour_Id,
                                        SHT.hourType,
                                        TT.Semester_Id,
                                        SHT.hour_type_id,
                                        tt.Course_Id,
                                        tt.Staff_Id,
                                        tt.Class_Id
                                    FROM
                                        TT_TIMETABLE AS TT
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = TT.STAFF_ID
                                            INNER JOIN
                                        SUP_HOUR_TYPE SHT ON SHT.HOUR_TYPE_ID = TT.HOUR_TYPE
		                                    INNER JOIN
	                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID=TT.STAFF_ID
		                                    AND CCS.CLASS_ID=TT.CLASS_ID
                                            AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
	                                    SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
		                                    AND S.IS_ACTIVE=1
                                    WHERE
                                        TT.CLASS_ID IN (?CLASS_ID , ?S_CLASS_ID)
                                            AND TT.IS_DELETED != 1
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                    GROUP BY TT.COURSE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCourseByAcademicYearAndSemesterId:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    COURSE_CODE,
                                    COURSE_TITLE
                                FROM
                                    CP_COURSE_ROOT
                                WHERE ACADEMIC_YEAR=?ACADEMIC_YEAR AND IS_DELETED!=1 AND SEMESTER_ID IN (?SEMESTER_ID);";
                        break;
                    }
                case TimeTableSQLCommands.IsStaffExistForTimeTable:
                    {
                        //query = @"SELECT 
                        //            Time_Table_Id,
                        //            Course_Id,
                        //            Day_Order_Id,
                        //            Staff_Id,
                        //            Class_Id,
                        //            Hour_Id,
                        //            ACADEMIC_YEAR
                        //        FROM
                        //            tt_timetable
                        //        WHERE
                        //            Day_Order_Id = ?DAY_ORDER_ID AND Hour_Id = ?HOUR_ID
                        //                AND Staff_Id = ?STAFF_ID AND SEMESTER_ID=?SEMESTER_ID
                        //                AND ACADEMIC_YEAR = ?ACADEMIC_YEAR;";
                        query = @"SELECT 
                                    tt.Time_Table_Id,
                                    tt.Course_Id,
                                    tt.Day_Order_Id,
                                    tt.Staff_Id,
                                    tt.Class_Id,
                                    tt.Hour_Id,
                                    tt.ACADEMIC_YEAR,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, '')) AS 'COURSE_CODE',
                                    C.CLASS_NAME AS CLASS_CODE
                                FROM
                                    tt_timetable AS tt
                                        INNER JOIN
                                    cp_classes AS c ON c.class_id = tt.Class_Id
                                        AND c.IS_DELETED != 1
                                        LEFT JOIN
                                    cp_course_root AS cr ON cr.COURSE_ROOT_ID = tt.Course_Id
                                        AND cr.IS_DELETED != 1
                                        INNER JOIN
	                                 CP_CLASSES AS CC ON CC.CLASS_ID=?CLASS_ID
		                                AND CC.SHIFT=C.SHIFT
                                        INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
                                        AND CCS.COURSE_ID=TT.COURSE_ID
                                        AND CCS.STAFF_ID=TT.STAFF_ID
                                        AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                        AND CCS.IS_DELETED!=1
                                        INNER JOIN
                                    SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE=1
                                WHERE
                                    tt.Day_Order_Id = ?DAY_ORDER_ID AND tt.Hour_Id = ?HOUR_ID
                                        AND tt.Staff_Id = ?STAFF_ID
                                        
                                        AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                        AND tt.Is_Deleted != 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchHourByClassIdAndDayOrderId:
                    {
                        query= @"SELECT 
                                    H.HOUR_ID,
                                    H.HOUR
                                FROM
                                    TT_TIMETABLE AS TT
                                        INNER JOIN
                                    SUP_HOURS AS H ON H.HOUR_ID = TT.HOUR_ID
                                WHERE
                                    TT.CLASS_ID = ?CLASS_ID AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY TT.HOUR_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCoursePattenAvailableCourses:
                    {
                        query = @"SELECT 
                                    COURSE_PATTERN_ID,
                                    COURSE_STRUCTURE_ID,
                                    PROGRAMME_GROUP_ID,
                                    COURSE_ID,
                                    IS_OPTIONAL,
                                    ACADEMIC_YEAR
                                FROM
                                    CP_COURSE_PATTERN
                                WHERE
                                    COURSE_STRUCTURE_ID = ?COURSE_STRUCTURE_ID
                                        AND PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        AND ACADEMIC_YEAR = ?ACADEMIC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCourseByClassAndAcademicYear:
                    {
                        query = @"SELECT   
                                    COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            ' - ',
                                            IFNULL(COURSE_CODE, '')) AS 'COURSE_TITLE',
                                    SEMESTER_ID 
                                FROM
                                    CP_COURSE_PATTERN AS CCP
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                            ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR
                                        WHERE
                                            ACTIVE_YEAR = 1 AND IS_DELETED != 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SEM ON SEM.BATCH = AB.BATCH_ID
                                        AND SEM.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND SEM.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR 
                                        AND SEM.IS_DELETED != 1
                                WHERE
                                    CCP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCR.SEMESTER_ID = SEM.SEMESTER
                                        AND SEM.IS_ACTIVE = 1
                                        AND CCP.IS_DELETED != 1
                                        AND CCP.IS_DELETED != 1
                                        AND C.CLASS_ID = ?CLASS_ID GROUP BY CCP.COURSE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchStaffByClassId:
                    {
                        query = @"SELECT 
                                        ST.STAFF_ID,
                                        ST.STAFF_CODE,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                '(',
                                                STAFF_CODE,
                                                ')') AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO AS ST
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = ST.STAFF_ID AND CCS.IS_DELETED!=1
                                    WHERE
                                        CCS.CLASS_ID=?CLASS_ID AND CCS.ACADEMIC_YEAR=?AC_YEAR AND ST.IS_DELETED!=1 GROUP BY ST.STAFF_CODE;";
                        break;
                    }
                case TimeTableSQLCommands.FetchMultipleTimeTableByTimeTableId:
                    {
                        query = @"SELECT 
                                    TT.TIME_TABLE_ID as Time_Table_Id ,
                                    C.CLASS_NAME AS CLASS_CODE,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, '')) AS 'COURSE_CODE',
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                            IFNULL(SP.LAST_NAME, ''),
                                            '(',
                                            SP.STAFF_CODE,
                                            ')') AS STAFF_CODE,
                                    H.HOUR ,
                                    HT.HOURTYPE AS Hour_Type
                                    
                                FROM
                                    TT_TIMETABLE AS TT
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = TT.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_HOURS AS H ON H.HOUR_ID = TT.HOUR_ID
                                        INNER JOIN
                                    SUP_HOUR_TYPE AS HT ON HT.HOUR_TYPE_ID = TT.HOUR_TYPE
                                        AND HT.IS_DELETED != 1
                                WHERE
                                    TT.TIME_TABLE_ID IN (?TIME_TABLE_ID)
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.IsTimeTableExist:
                    {
                        query = @"SELECT 
                                    tt.Time_Table_Id,
                                    tt.Course_Id,
                                    tt.Day_Order_Id,
                                    tt.Staff_Id,
                                    tt.Class_Id,
                                    tt.Hour_Id,
                                    tt.ACADEMIC_YEAR,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, '')) AS 'COURSE_CODE',
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, ''),
                                            '(',
                                            SP.STAFF_CODE,
                                            ')') AS STAFF_CODE,
                                    C.CLASS_NAME AS CLASS_CODE
                                FROM
                                    tt_timetable AS tt
                                        INNER JOIN
                                    cp_classes AS c ON c.class_id = tt.Class_Id
                                        AND c.IS_DELETED != 1
                                        LEFT JOIN
                                    cp_course_root AS cr ON cr.COURSE_ROOT_ID = tt.Course_Id
                                        AND cr.IS_DELETED != 1
                                        INNER JOIN
                                    stf_personal_info AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                WHERE
                                    tt.Day_Order_Id = ?DAY_ORDER_ID AND tt.Hour_Id = ?HOUR_ID
                                        AND tt.Semester_Id IN(?SEMESTER_ID)
                                        AND tt.Staff_Id = ?STAFF_ID
                                        AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                        AND tt.Is_Deleted != 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchValueEducationPapersForTimeTable:
                    {
                        query = @"SELECT 
                                    TT.Time_Table_Id,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    CONCAT(IFNULL(ST.FIRST_NAME, ''),' ',
                                                IFNULL(ST.LAST_NAME, ''),
                                                '(',
                                                ST.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                    TT.Day_Order_Id,
                                    TT.Hour_Id,
                                    SHT.hourType,
                                    TT.Semester_Id,
                                    SHT.hour_type_id,
                                    tt.Course_Id,
                                    tt.Staff_Id,
                                    tt.Class_Id
                                FROM
                                    TT_TIMETABLE AS TT
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID AND CR.IS_DELETED!=1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = TT.STAFF_ID AND ST.IS_DELETED!=1
                                        INNER JOIN
                                    SUP_HOUR_TYPE SHT ON SHT.HOUR_TYPE_ID = TT.HOUR_TYPE
                                        INNER JOIN
                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = TT.SEMESTER_ID
                                        AND SS.IS_ACTIVE=1
                                WHERE
                                    TT.CLASS_ID = ?CLASS_ID AND TT.IS_DELETED != 1
                                        AND CR.UGC_COURSE_TYPE=48
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchValueEducationPaperByTTClassIdForDDL:
                    {
                        query = @"SELECT 
                                        TT.COURSE_ID,
                                        UPPER(CR.COURSE_TITLE) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID 
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID
                                         AND TT.IS_DELETED!=1
                                        AND CR.UGC_COURSE_TYPE = 48
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY COURSE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableInfoByDayIdAndHourId:
                    {
                        query = @"SELECT 
                                        TT.Time_Table_Id,
                                        TT.Class_Id,
                                        TT.Course_Id,
                                        TT.Day_Order_Id,
                                        TT.Hour_Id,
                                        TT.Hour_Type,
                                        TT.Staff_Id,
                                        CR.Semester_Id
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = TT.CLASS_ID
                                            INNER JOIN
										CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID=TT.STAFF_ID
											AND CCS.CLASS_ID=TT.CLASS_ID
											AND CCS.COURSE_ID=TT.COURSE_ID
											AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
											AND CCS.IS_DELETED!=1
											INNER JOIN
										SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND S.IS_ACTIVE=1
                                    WHERE
                                        TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.CLASS_ID = ?CLASS_ID
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.STAFF_ID = ?STAFF_ID
                                    GROUP BY TT.TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableByStaffBased:
                    {
                        query = @"SELECT 
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        TT.Day_Order_Id,
                                        TT.Hour_Id,
                                        TT.Semester_Id,
                                        CLS.CLASS_CODE,
                                        CLS.SHIFT
                                    FROM
                                         TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
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
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1 ORDER BY TT.COURSE_ID,DAY_ORDER_ID,HOUR_ID;
                                            select * from CP_CLASS_COURSE_STAFF where staff_id=1 and ACADEMIC_YEAR = 2021 and SEMESTER_TYPE_ID=2;
                                            select * from TT_ONLINE_TIMETABLE where STAFF_ID = 1 and ACADEMIC_YEAR = 2021 ;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableBySemesterTypeAndStaffIds:
                    {
                        query = @"SELECT 
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        TT.Day_Order_Id,
                                        TT.Hour_Id,
                                        TT.Semester_Id,
                                        CLS.CLASS_CODE,
                                        CLS.SHIFT,
                                        TT.Staff_Id,
                                        SP.STAFF_CODE,
                                        SP.DEPARTMENT,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        TT.ACADEMIC_YEAR = ?AC_YEAR AND TT.STAFF_ID IN (?STAFF_ID)
                                            AND TT.IS_DELETED != 1
                                    ORDER BY TT.COURSE_ID , DAY_ORDER_ID , HOUR_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTimeTableHoursCountByStaffBased:
                    {
                        query = @"SELECT 
    
                                         TT.Day_Order_Id,
                                         TT.Hour_Id
    
                                    FROM
                                        CP_COURSE_PATTERN AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        TT_TIMETABLE AS TT ON TT.COURSE_ID = CP.COURSE_ID
                                            AND TT.STAFF_ID =?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                            AND SEM.SEMESTER IN (SELECT 
                                                SEMESTER
                                            FROM
                                                ACADEMIC_SEMESTER
                                            WHERE
                                                ACADEMIC_YEAR = ?AC_YEAR AND IS_ACTIVE = 1)
                                            AND SEM.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE BETWEEN SEM.DATE_FROM AND SEM.DATE_TO
                                    WHERE
                                        SEM.IS_ACTIVE = 1
                                            AND CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CP.IS_DELETED != 1;";
                        break;
                    }


                #region Substitute Hour


                case TimeTableSQLCommands.IsSubstituteHourExist:
                    {
                        query = @"SELECT 
                                        ATT.SUBSTITUTE_ID,
                                        ATT.SEMESTER_TYPE_ID,
                                        ATT.DAY_ORDER_DATE,
                                        ATT.DAY_ORDER_ID,
                                        ATT.HOUR_ID,
                                        ATT.CLASS_ID,
                                        ATT.COURSE_ID,
                                        ATT.STAFF_ID,
                                        ATT.INCHARGE_ID
                                    FROM
                                        ATT_SUBSTITUTE AS ATT
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = ATT.CLASS_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = ?CLASS_ID
		                                    AND CC.SHIFT = C.SHIFT
                                    WHERE
                                        ATT.DAY_ORDER_DATE = ?DAY_ORDER_DATE
                                            AND ATT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND ATT.HOUR_ID = ?HOUR_ID
                                            AND ATT.INCHARGE_ID = ?INCHARGE_ID
                                            AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ATT.IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.SaveSubstituteHour:
                    {
                        query = @"INSERT INTO ATT_SUBSTITUTE
                                  (
	                                  SEMESTER_TYPE_ID,
	                                  DAY_ORDER_DATE,
	                                  DAY_ORDER_ID,
	                                  HOUR_ID,
	                                  CLASS_ID,
	                                  COURSE_ID,
	                                  STAFF_ID,
	                                  INCHARGE_ID,
	                                  ENTRY_ID,
	                                  ACADEMIC_YEAR
                                  )
                                  VALUES
                                  (
                                      ?SEMESTER_TYPE_ID,
	                                  ?DAY_ORDER_DATE,
	                                  ?DAY_ORDER_ID,
	                                  ?HOUR_ID,
	                                  ?CLASS_ID,
	                                  ?COURSE_ID,
	                                  ?STAFF_ID,
	                                  ?INCHARGE_ID,
	                                  ?ENTRY_ID,
	                                  ?AC_YEAR
                                  );";
                        break;
                    }
                case TimeTableSQLCommands.BindSubstituteHoursbyClassId:
                    {
                        query = @"SELECT 
                                    ATT.SUBSTITUTE_ID,
                                    ATT.DAY_ORDER_ID,
                                    ATT.HOUR_ID,
                                    ATT.COURSE_ID,
                                    ATT.STAFF_ID,
                                    ATT.CLASS_ID,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    '(Substitute Hour)' AS HOUR_TYPE,
                                    DATE_FORMAT(DAY_ORDER_DATE, '%d/%m/%Y') DAY_ORDER_DATE,
                                    CONCAT(IFNULL(ST.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(ST.LAST_NAME, ''),
                                            '(',
                                            ST.STAFF_CODE,
                                            ')') AS STAFF_CODE
                                FROM
                                    ATT_SUBSTITUTE AS ATT
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ATT.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = ATT.INCHARGE_ID
                                        AND ST.IS_DELETED != 1
                                WHERE
                                    ATT.CLASS_ID = ?CLASS_ID
                                        AND ATT.DAY_ORDER_DATE >= CURDATE()
                                        AND ATT.IS_DELETED != 1
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchSubstituteHourInfo:
                    {
                        query = @"SELECT 
                                        SUBSTITUTE_ID,
                                        H.HOUR,
                                        SD.DAY AS DAY_ORDER,
                                        C.CLASS_NAME,
                                        TT.COURSE_ID,  
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                '(',
                                                CR.COURSE_CODE,
                                                ')') AS COURSE_TITLE,
                                        DATE_FORMAT(DAY_ORDER_DATE, '%d/%m/%Y') DAY_ORDER_DATE,
                                        ST.STAFF_ID,
                                        CONCAT(IFNULL(ST.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(ST.LAST_NAME, ''),
                                                '(',
                                                ST.STAFF_CODE,
                                                ')') AS STAFF_CODE,
	                                    ATT.INCHARGE_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = TT.STAFF_ID
                                            AND ST.IS_DELETED != 1
                                            INNER JOIN
	                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID=TT.STAFF_ID
		                                    AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.CLASS_ID=TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
	                                    SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
		                                    AND S.IS_ACTIVE=1
                                            INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=TT.CLASS_ID
		                                    INNER JOIN
	                                    SUP_DAY_ORDER AS SD ON SD.DAY_ORDER_ID=TT.DAY_ORDER_ID
		                                    INNER JOIN
	                                    SUP_HOURS AS H ON H.HOUR_ID=TT.HOUR_ID
											LEFT JOIN
										ATT_SUBSTITUTE AS ATT ON ATT.DAY_ORDER_ID=TT.DAY_ORDER_ID
											AND ATT.HOUR_ID=TT.HOUR_ID
                                            AND ATT.CLASS_ID=TT.CLASS_ID
                                            AND ATT.COURSE_ID=TT.COURSE_ID
                                            AND ATT.STAFF_ID=TT.STAFF_ID
                                            AND ATT.DAY_ORDER_DATE >= CURDATE()
                                            AND ATT.IS_DELETED!=1
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID
                                            AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND TT.HOUR_ID = ?HOUR_ID
                                            AND TT.IS_DELETED != 1
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchDateByDayOrderId:
                    {
                        query = @"SELECT 
                                        DAY_ID,
                                        DATE_FORMAT(DAY_ORDER_DATE,'%d/%m/%Y') AS  DAY_ORDER_DATE
                                    FROM
                                        CALENDER
                                    WHERE
                                        DAY_ORDER = ?DAY_ORDER_ID
                                            AND DAY_ORDER_DATE >= CURDATE()AND IS_HOLIDAY != 1;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateSubstituteHour:
                    {
                        query = @"UPDATE ATT_SUBSTITUTE
                                  SET
	                                  DAY_ORDER_DATE = ?DAY_ORDER_DATE,
                                      STAFF_ID=?STAFF_ID,
	                                  INCHARGE_ID = ?INCHARGE_ID,
	                                  ENTRY_ID = ?ENTRY_ID
                                  WHERE 
	                                  SUBSTITUTE_ID = ?SUBSTITUTE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.DeleteSubstituteHour:
                    {
                        query = @"UPDATE ATT_SUBSTITUTE SET IS_DELETED=1 WHERE SUBSTITUTE_ID=?SUBSTITUTE_ID;";
                        break;
                    }

                case TimeTableSQLCommands.FetchSubstituteHourByStaffId:
                    {
                        query = @"SELECT 
                                    ATT.SUBSTITUTE_ID,
                                    ATT.DAY_ORDER_ID,
                                    ATT.HOUR_ID,
                                    ATT.COURSE_ID,
                                    ATT.STAFF_ID,
                                    ATT.CLASS_ID,
                                    C.SHIFT,
                                    C.CLASS_NAME,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    '(Substitute Hour)' AS HOUR_TYPE,
                                    DATE_FORMAT(DAY_ORDER_DATE, '%d/%m/%Y') DAY_ORDER_DATE,
                                    CONCAT(IFNULL(ST.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(ST.LAST_NAME, ''),
                                            '(',
                                            ST.STAFF_CODE,
                                            ')') AS STAFF_CODE
                                FROM
                                    ATT_SUBSTITUTE AS ATT
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ATT.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = ATT.INCHARGE_ID
                                        AND ST.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID=ATT.CLASS_ID
                                WHERE
                                    ATT.INCHARGE_ID = ?STAFF_ID
                                        AND ATT.DAY_ORDER_DATE >= CURDATE()
                                        AND ATT.IS_DELETED != 1
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }



                #endregion
                case TimeTableSQLCommands.FetchOverAllCoursePattern:
                    {
                        query = @"SELECT 
                                        CP.COURSE_PATTERN_ID,
                                        CP.COURSE_STRUCTURE_ID,
                                        CP.PROGRAMME_GROUP_ID,
                                        CP.COURSE_ID,
                                        CS.FACULTY_ID,
                                        CS.PROGRAMME_LEVEL,
                                        CS.PROGRAMME_YEAR,
                                        CS.SEMESTER_ID,
                                        CS.IS_OPTIONAL,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CR.HRS_PER_WEEK,
                                        CR.PAPER_TYPE,
                                        SPT.PAPER_TYPE AS PAPER_TYPE_NAME,
                                        CR.COURSE_TYPE,
                                        CR.SUBJECT_TYPE,
                                        SPT.IS_ACTIVE,
                                        ST.SUBJECT_TYPE AS SUBJECT_TYPE_NAME
                                    FROM
                                        CP_COURSE_PATTERN AS CP
                                            INNER JOIN
                                        CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                            INNER JOIN
                                        SUP_PAPER_TYPE AS SPT ON SPT.PAPER_TYPE_ID = CR.PAPER_TYPE
                                            INNER JOIN
                                        SUP_SUBJECT_TYPE AS ST ON ST.SUBJECT_TYPE_ID = CR.SUBJECT_TYPE
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.SEMESTER_ID IN (?SEMESTER)
                                           
                                            AND CP.IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchClassessByProgramme:
                    {
                        query = @"SELECT 
                                CLASS_ID,
                                CLASS_NAME,
                                CLASS_YEAR,
                                CLASS_LEVEL,
                                PROGRAMME_GROUP_ID,
                                SHIFT,
                                PROGRAMME_MODE,
                                CLASS_TYPE
                            FROM
                                CP_CLASSES
                            WHERE
                                PROGRAMME_GROUP_ID =?PROGRAMME_GROUP_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchProgammeTimetabletemplate:
                    {
                        query = @"SELECT 
                                TT.TT_Temp_Id,
                                TT.Day_Order_Id,
                                TT.Hour_Id,
                                TT.Paper_Type_Id,
                                SPT.PAPER_TYPE,
                                TT.Is_Static,
                                TT.Faculty_Id,
                                CF.FACULTY,
                                TT.Setting_Id,
                                TT.Year,
                                TT.GraduationType,
                                TT.ACADEMIC_YEAR,
                                TT.SHIFT
                            FROM
                                TT_TEMPLATE AS TT
                                    INNER JOIN
                                SUP_PAPER_TYPE AS SPT ON SPT.PAPER_TYPE_ID = TT.PAPER_TYPE_ID
                                    INNER JOIN
                                CP_FACULTY AS CF ON CF.FACULTY_ID = TT.FACULTY_ID
                            WHERE                       
                                     TT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchStaffHourAllotment:
                    {
                        query = @"SELECT 
                                CP.COURSE_PATTERN_ID,
                                CP.COURSE_STRUCTURE_ID,
                                CP.PROGRAMME_GROUP_ID,
                                CC.CLASS_ID,
                                CC.CLASS_NAME,
                                CP.COURSE_ID,
                                CCR.COURSE_CODE,
                                CCR.HRS_PER_WEEK,
                                CCR.CONS_COURSE_TITLE,
                                CCS.STAFF_ID,
                                CCR.PAPER_TYPE,
                                SPT.Paper_Type AS PAPER_TYPE_NAME,
                                CCR.UGC_COURSE_TYPE,
                                CUCT.UGC_COURSE_TYPE AS UGC_COURSE_TYPE_NAME,
                                CCR.SUBJECT_TYPE,
                                SST.SUBJECT_TYPE AS SUBJECT_TYPE_NAME,
                                CCR.SEMESTER_ID,
                                CCS.HRS_WEEK,
                                SP.STAFF_CODE
                            FROM
                                CP_COURSE_PATTERN AS CP
                                    INNER JOIN
                                CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                    AND CC.CLASS_YEAR = CS.PROGRAMME_YEAR
                                    AND CS.PROGRAMME_LEVEL = CC.CLASS_LEVEL
                                    LEFT JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.COURSE_ID = CP.COURSE_ID
                                    AND CCS.CLASS_ID = CC.CLASS_ID
                                    AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CCS.SEMESTER_ID IN (?SEMESTER)
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                    LEFT JOIN
                                sup_paper_type AS SPT ON SPT.Paper_Type_Id = CCR.PAPER_TYPE
                                    LEFT JOIN
                                sup_subject_type AS SST ON SST.SUBJECT_TYPE_ID = CCR.SUBJECT_TYPE
                                    LEFT JOIN
                                cp_ugc_course_type AS CUCT ON CUCT.UGC_COURSE_TYPE_ID = CCR.UGC_COURSE_TYPE
                                    LEFT JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                            WHERE
                                CP.ACADEMIC_YEAR =?AC_YEAR
                                    AND CP.IS_DELETED != 1
                                    AND CS.SEMESTER_ID IN (?SEMESTER);";
                        break;
                    }
                case TimeTableSQLCommands.FetchSupPaparType:
                    {
                        query = @"SELECT 
                            PAPER_TYPE_ID, PAPER_TYPE, IS_ACTIVE, IS_DELETED
                        FROM
                            SUP_PAPER_TYPE
                        WHERE
                            IS_ACTIVE = 1 AND IS_FIXED != 1
                        ORDER BY ORDER_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchOverAlltimetable:
                    {
                        query = @"SELECT 
                                TT.Time_Table_Id,
                                TT.Class_Id,
                                TT.Course_Id,
                                TT.Day_Order_Id,
                                TT.Hour_Id,
                                TT.Hour_Type,
                                TT.Staff_Id,
                                CR.Semester_Id
                            FROM
                                TT_TIMETABLE AS TT
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID = TT.CLASS_ID
                                    LEFT JOIN
                                CP_CLASS_COURSE_STAFF AS CCS 
		                            ON CCS.CLASS_ID = TT.CLASS_ID
                                    AND CCS.COURSE_ID = TT.COURSE_ID
                                    AND CCS.STAFF_ID=TT.STAFF_ID
                                    AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                    AND CCS.IS_DELETED != 1
                                    LEFT JOIN
                                SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                    AND S.IS_ACTIVE = 1
                            WHERE
                                TT.ACADEMIC_YEAR = ?AC_YEAR
                            GROUP BY TT.TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchPossibilityHoursForStaff:
                    {
                        query = @"SELECT 
                                    TBL.COURSE_ID,
                                    TBL.STAFF_ID,
                                    TBL.CLASS_ID,
                                    CCS.CLSCRSSTF_ID,
                                    CCR.SEMESTER_ID,
                                    TBL.Time_Table_Id,
                                    TT.TT_Temp_Id,
                                    TT.Hour_Id,
                                    TT.Day_Order_Id
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CCS
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
        
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                      
                                        INNER JOIN
                                    TT_TEMPLATE AS TT ON 
                                        TT.SHIFT = CC.SHIFT AND  TT.FACULTY_ID = ?FACULTY
                                        AND TT.YEAR = CC.CLASS_YEAR
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                        LEFT JOIN
                                    TT_TIMETABLE AS TBL ON TBL.HOUR_ID = TT.HOUR_ID
                                        AND TBL.STAFF_ID = CCS.STAFF_ID
                                        AND TBL.DAY_ORDER_ID = TT.DAY_ORDER_ID
                                        AND TBL.ACADEMIC_YEAR =?AC_YEAR
                                        AND TBL.SEMESTER_ID IN (?SEMESTER)
                                WHERE
		                                CCS.STAFF_ID =?STAFF_ID
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCS.SEMESTER_ID IN (?SEMESTER)
                                        AND TBL.TIME_TABLE_ID IS NULL
                                GROUP BY TT.HOUR_ID , TT.DAY_ORDER_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchoverallOptionclass:
                    {
                        query = @"SELECT 
                                CCR.COURSE_ROOT_ID,
                                CCR.COURSE_CODE,
                                CCR.COURSE_TITLE,
                                CC.CLASS_ID,
                                CC.CLASS_YEAR,
                                CC.SHIFT,
                                CC.DEPARTMENT,
                                CC.CLASS_NAME,
                                CC.CLASS_LEVEL,
                                CCR.PAPER_TYPE,
                                CCR.SEMESTER_ID,
                                SPT.PAPER_TYPE AS PAPER_TYPE_NAME,
                                DEP.FACULTY
                            FROM
                                CP_COURSE_PATTERN AS CP
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                    AND CCR.SEMESTER_ID IN (?SEMESTER)
                                    AND CCR.IS_DELETED != 1
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.COURSE_ID = CCR.COURSE_ROOT_ID
                                    AND CC.IS_DELETED != 1
                                    INNER JOIN
                                SUP_PAPER_TYPE AS SPT ON SPT.PAPER_TYPE_ID = CCR.PAPER_TYPE
                                    INNER JOIN
                                cp_department AS DEP ON DEP.DEPARTMENT_ID = CC.DEPARTMENT
                            WHERE
                                CP.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CP.IS_DELETED != 1
                            GROUP BY CP.COURSE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchNMEclasscoursestaff:
                    {
                        query = @"SELECT 
                                    CCS.CLSCRSSTF_ID,
                                    CCS.CLASS_ID,
                                    CCS.COURSE_ID,
                                    CCS.STAFF_ID,
                                    CCS.SEMESTER_ID,
                                    CCS.SHIFT,
                                    CCS.HRS_WEEK
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CCS
                                WHERE
                                    CCS.CLASS_ID = ?CLASS_ID
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCS.SEMESTER_ID IN (?SEMESTER);";
                        break;
                    }
                case TimeTableSQLCommands.FetchCheckIsTimetableExists:
                    {
                        query = @"SELECT 
                                TBL.Time_Table_Id,
                                TBL.Hour_Id,
                                TBL.Day_Order_Id
                            FROM
                                TT_TIMETABLE AS TBL
                            WHERE
                                TBL.HOUR_ID = ?HOUR AND TBL.DAY_ORDER_ID = ?DAY
                                    AND TBL.COURSE_ID = ?COURSE_ID
                                    AND TBL.CLASS_ID =?CLASS_ID
                                    AND TBL.ACADEMIC_YEAR =?AC_YEAR
                                    AND TBL.SEMESTER_ID IN (?SEMESTER);";
                        break;
                    }
                case TimeTableSQLCommands.FetchClasswisetemplatewithtimetable:
                    {
                        query = @"SELECT 
                                 TT.TT_Temp_Id,
                                 TT.Day_Order_Id,                      
                                 TT.Hour_Id,
                                 TT.Paper_Type_Id,
                                 TBL.Time_Table_Id,
                                 TBL.COURSE_ID,
                                 TBL.CLASS_ID,
                                 TBL.STAFF_ID,TT.Is_Static,TT.Faculty_Id
                            FROM
                                TT_TEMPLATE AS TT
                                    LEFT JOIN
                                TT_TIMETABLE AS TBL ON TBL.DAY_ORDER_ID = TT.DAY_ORDER_ID
                                    AND TBL.HOUR_ID = TT.HOUR_ID
                                    AND TBL.CLASS_ID =?CLASS_ID
                                    AND TBL.ACADEMIC_YEAR = ?AC_YEAR
                                    AND TBL.SEMESTER_ID IN (?SEMESTER)
                            WHERE
                                TT.ACADEMIC_YEAR =?AC_YEAR
                                    AND TT.FACULTY_ID = ?FACULTY
                                    AND TT.YEAR =?CLASS_YEAR
                                    AND TT.GRADUATIONTYPE =?GRADUATIONTYPE
                                    AND TT.SHIFT = ?SHIFT
                            GROUP BY TT.HOUR_ID , TT.DAY_ORDER_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchClassCourseStaff:
                    {
                        query = @"SELECT 
                                CP.COURSE_PATTERN_ID,
                                CP.COURSE_STRUCTURE_ID,
                                CP.PROGRAMME_GROUP_ID,
                                CC.CLASS_ID,
                                CC.CLASS_NAME,
                                CP.COURSE_ID,
                                CCR.COURSE_CODE,
                                CCR.HRS_PER_WEEK,
                                CCR.CONS_COURSE_TITLE,
                                CCS.STAFF_ID,
                                CCR.PAPER_TYPE,
                                SPT.Paper_Type AS PAPER_TYPE_NAME,
                                CCR.UGC_COURSE_TYPE,
                                CUCT.UGC_COURSE_TYPE AS UGC_COURSE_TYPE_NAME,
                                CCR.SUBJECT_TYPE,
                                SST.SUBJECT_TYPE AS SUBJECT_TYPE_NAME,
                                CCR.SEMESTER_ID,
                                CCS.HRS_WEEK,
                                SP.STAFF_CODE
                            FROM
                                CP_COURSE_PATTERN AS CP
                                    INNER JOIN
                                CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                    AND CC.CLASS_YEAR = CS.PROGRAMME_YEAR
                                    AND CS.PROGRAMME_LEVEL = CC.CLASS_LEVEL
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.COURSE_ID = CP.COURSE_ID
                                    AND CCS.CLASS_ID = CC.CLASS_ID
                                    AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CCS.SEMESTER_ID IN (?SEMESTER)
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                    LEFT JOIN
                                sup_paper_type AS SPT ON SPT.Paper_Type_Id = CCR.PAPER_TYPE
                                    LEFT JOIN
                                sup_subject_type AS SST ON SST.SUBJECT_TYPE_ID = CCR.SUBJECT_TYPE
                                    LEFT JOIN
                                CP_UGC_COURSE_TYPE AS CUCT ON CUCT.UGC_COURSE_TYPE_ID = CCR.UGC_COURSE_TYPE
                                    LEFT JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                            WHERE
                                CP.ACADEMIC_YEAR =?AC_YEAR
                                    AND CP.COURSE_ID = ?COURSE_ID
                                    AND CC.CLASS_ID =?CLASS_ID
                                    AND CP.IS_DELETED != 1
                                    AND CS.SEMESTER_ID IN (?SEMESTER);";
                        break;
                    }
                case TimeTableSQLCommands.FetchStaffFixedhours:
                    {
                        query = @"SELECT 
                                CLASS_ID, COURSE_ID, HOUR AS Hour_Id, DAY_ORDER AS Day_Order_Id, STAFF_ID
                            FROM
                                TT_FIXEDHOURS
                            WHERE
                                ACADEMIC_YEAR =?AC_YEAR
                                    AND SEMESTER_ID IN (?SEMESTER)
                                    AND STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchClasswisetemplate:
                    {
                        query = @"SELECT 
                                TT.TT_Temp_Id,
                                TT.Day_Order_Id,
                                TT.Hour_Id,
                                TT.Paper_Type_Id,
                                TBL.Time_Table_Id,
                                TBL.COURSE_ID,
                                TBL.CLASS_ID,
                                TBL.STAFF_ID,
                                TT.Is_Static,
                                TT.Faculty_Id
                            FROM
                                TT_TEMPLATE AS TT
                                    LEFT JOIN
                                TT_TIMETABLE AS TBL ON TBL.DAY_ORDER_ID = TT.DAY_ORDER_ID
                                    AND TBL.HOUR_ID = TT.HOUR_ID
                                    AND TBL.CLASS_ID = ?CLASS_ID
                                    AND TBL.ACADEMIC_YEAR = ?AC_YEAR
                                    AND TBL.SEMESTER_ID IN (?SEMESTER)
                            WHERE
                                TT.ACADEMIC_YEAR =?AC_YEAR
                                    AND TT.FACULTY_ID =?FACULTY_ID
                                    AND TT.YEAR = (SELECT 
                                        CLASS_YEAR
                                    FROM
                                        cp_classes
                                    WHERE
                                        CLASS_ID = ?CLASS_ID)
                                    AND TT.GRADUATIONTYPE = (SELECT 
                                        CLASS_LEVEL
                                    FROM
                                        cp_classes
                                    WHERE
                                        CLASS_ID = ?CLASS_ID)
                                    AND TT.SHIFT = (SELECT 
                                        SHIFT
                                    FROM
                                        cp_classes
                                    WHERE
                                        CLASS_ID = ?CLASS_ID)
                            GROUP BY TT.HOUR_ID , TT.DAY_ORDER_ID;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateTimetable:
                    {
                        query = @"UPDATE TT_TIMETABLE 
                                    SET 
                                        HOUR_ID=?HOUR_ID,DAY_ORDER_ID=?DAY_ORDER_ID
                                    WHERE
                                        TIME_TABLE_ID = ?TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchClasswisefixedhours:
                    {
                        query = @"SELECT 
                                CLASS_ID,
                                COURSE_ID,
                                HOUR AS Hour_Id,
                                DAY_ORDER AS Day_Order_Id,
                                STAFF_ID
                            FROM
                                tt_fixedhours
                            WHERE
                                ACADEMIC_YEAR =?AC_YEAR AND CLASS_ID =?CLASS_ID
                                    AND SEMESTER_ID IN (?SEMESTER);";
                        break;
                    }
                case TimeTableSQLCommands.FetchHourBasedStaffAvailabillty:
                    {
                        query = @"SELECT 
                                        STAFF_ID  as Staff_Id, 
                                        FIRST_NAME AS NAME, 
                                        STAFF_CODE, 
                                        DEPARTMENT
                                    FROM
                                        (SELECT 
                                            SP.STAFF_ID,
                                                SP.FIRST_NAME,
                                                SP.STAFF_CODE,
                                                TT.HOUR_ID,
                                                DD.DEPARTMENT
                                        FROM
                                            STF_PERSONAL_INFO AS SP
			                                    INNER JOIN 
		                                    CP_CLASS_COURSE_STAFF AS CCS ON SP.STAFF_ID = CCS.STAFF_ID
			                                    AND CCS.IS_DELETED != 1
			                                    INNER JOIN 
		                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
			                                    AND IS_ACTIVE = 1
			                                    INNER JOIN 
		                                    CP_DEPARTMENT AS DD ON DD.DEPARTMENT_ID = SP.DEPARTMENT
			                                    INNER JOIN 
		                                    CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
			                                    AND CC.SHIFT = ?SHIFT
			                                    AND CC.CLASS_LEVEL != 3
			                                    LEFT JOIN 
		                                    TT_TIMETABLE AS TT ON TT.STAFF_ID = SP.STAFF_ID
			                                    AND TT.CLASS_ID = CCS.CLASS_ID
			                                    AND TT.COURSE_ID = CCS.COURSE_ID
			                                    AND TT.HOUR_ID = ?HOUR_ID
			                                    AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID
			                                    AND TT.ACADEMIC_YEAR = CCS.ACADEMIC_YEAR
			                                    AND TT.IS_DELETED != 1
                                        WHERE
                                            SP.IS_DELETED != 1
                                                AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SP.NON_TEACHING_CATEGORY != 1
                                        GROUP BY SP.STAFF_ID
                                        ORDER BY SP.STAFF_ID) AS S
                                    WHERE
                                        S.HOUR_ID IS NULL
                                    ORDER BY DEPARTMENT";
                        break;
                    }
                #region Online TimeTable
                case TimeTableSQLCommands.FetchOnlineTimeTableClassWiseListByClassId:
                    {

                        query = @"SELECT 
                                    TT.Time_Table_Id,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    CONCAT(IFNULL(ST.FIRST_NAME, ''),' ',
                                                IFNULL(ST.LAST_NAME, ''),
                                                '(',
                                                ST.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                    TT.Day_Order_Id,
                                    TT.Hour_Id,
                                    SHT.hourType,
                                    TT.Semester_Id,
                                    SHT.hour_type_id,
                                    tt.Course_Id,
                                    tt.Staff_Id,
                                    tt.Class_Id
                                FROM
                                    TT_ONLINE_TIMETABLE AS TT
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID AND CR.IS_DELETED!=1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = TT.STAFF_ID AND ST.IS_DELETED!=1
                                        left JOIN
                                    SUP_HOUR_TYPE SHT ON SHT.HOUR_TYPE_ID = TT.HOUR_TYPE
										INNER JOIN
									CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
										AND CCS.COURSE_ID=TT.COURSE_ID
										AND CCS.STAFF_ID=TT.STAFF_ID
                                        AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                        AND CCS.IS_DELETED!=1
                                        INNER JOIN
									SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
										AND S.IS_ACTIVE=1
                                WHERE
                                    TT.CLASS_ID IN(?CLASS_ID) AND TT.IS_DELETED != 1
                                    AND CR.UGC_COURSE_TYPE!=48
                                    AND TT.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }

                case TimeTableSQLCommands.FetchMultipleOnlineTimeTableByTimeTableId:
                    {
                        query = @"SELECT 
                                    TT.TIME_TABLE_ID as Time_Table_Id ,
                                    C.CLASS_NAME AS CLASS_CODE,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, '')) AS 'COURSE_CODE',
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',
                                            IFNULL(SP.LAST_NAME, ''),
                                            '(',
                                            SP.STAFF_CODE,
                                            ')') AS STAFF_CODE,
                                    H.HOUR ,
                                    HT.HOURTYPE AS Hour_Type
                                    
                                FROM
                                    TT_ONLINE_TIMETABLE AS TT
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = TT.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_HOURS AS H ON H.HOUR_ID = TT.HOUR_ID
                                        INNER JOIN
                                    SUP_HOUR_TYPE AS HT ON HT.HOUR_TYPE_ID = TT.HOUR_TYPE
                                        AND HT.IS_DELETED != 1
                                WHERE
                                    TT.TIME_TABLE_ID IN (?TIME_TABLE_ID)
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED != 1;";
                        break;
                    }

                case TimeTableSQLCommands.FetchOnlineTimeTableById:
                    {
                        query = @"SELECT 
                                        tt.Time_Table_Id,
                                        tt.Course_Id,
                                        tt.Day_Order_Id,
                                        tt.Staff_Id,
                                        tt.Class_Id,
                                        cls.CLASS_CODE,
                                        tt.Hour_Id,
                                        tt.Semester_Id,
                                        tt.Hour_Type
                                    FROM
                                        tt_online_timetable AS tt
                                            INNER JOIN
                                        cp_classes AS cls ON cls.CLASS_ID = tt.Class_Id AND cls.IS_DELETED != 1
                                    WHERE
                                        tt.Time_Table_Id = ?TIME_TABLE_ID 
                                        AND tt.Is_Deleted != 1 AND tt.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.CheckIsStaffAllotedForOnlineTimeTable:
                    {
                        query = @"SELECT 
                                        tt.Time_Table_Id,tt.Course_Id,C.CLASS_NAME AS CLASS_CODE
                                    FROM
                                        TT_ONLINE_TIMETABLE AS TT 
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
										CP_CLASSES AS C ON C.CLASS_ID=TT.CLASS_ID
                                            INNER JOIN
	                                    CP_CLASSES AS CC ON CC.CLASS_ID=?CLASS_ID
		                                    AND CC.SHIFT=C.SHIFT
                                            INNER JOIN
                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
                                            AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.STAFF_ID=TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                            AND CCS.IS_DELETED!=1
                                            INNER JOIN
                                    SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
                                             AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.HOUR_ID = ?HOUR_ID AND TT.DAY_ORDER_ID = ?DAY_ORDER_ID 
                                            AND TT.STAFF_ID = ?STAFF_ID AND TT.ACADEMIC_YEAR=?AC_YEAR
                                            AND TT.IS_DELETED != 1 GROUP BY TT.COURSE_ID;";
                        break;
                    }

                case TimeTableSQLCommands.SaveOnlineTimeTable:
                    {

                        query = @"INSERT INTO TT_ONLINE_TIMETABLE
                                            (
                                            Course_Id,
                                            Day_Order_Id,
                                            Staff_Id,
                                            Class_Id,
                                            Hour_Id,
                                            Semester_Id,
                                            Hour_Type,
                                            ACADEMIC_YEAR)
                                            VALUES
                                            (
                                            ?COURSE_ID,
                                            ?DAY_ORDER_ID,
                                            ?STAFF_ID,
                                            ?CLASS_ID,
                                            ?HOUR_ID,
                                            ?SEMESTER_ID,
                                            ?HOURTYPE,
                                            ?AC_YEAR);";

                        break;
                    }
                case TimeTableSQLCommands.DeleteOnlineTimeTable:
                    {
                        query = @"UPDATE TT_ONLINE_TIMETABLE 
                                        SET 
                                            IS_DELETED = 1
                                        WHERE
                                            TIME_TABLE_ID = ?TIME_TABLE_ID;";
                        break;
                    }
                case TimeTableSQLCommands.UpdateOnlineTimeTable:
                    {
                        query = @"UPDATE tt_online_timetable
                                            SET
                                            Course_Id= ?COURSE_ID,
                                            Day_Order_Id= ?DAY_ORDER_ID,
                                            Staff_Id= ?STAFF_ID,
                                            Class_Id= ?CLASS_ID,
                                            Hour_Id= ?HOUR_ID,
                                            Semester_Id= ?SEMESTER_ID,
                                            Hour_Type= ?HOURTYPE
                                            WHERE Time_Table_Id= ?TIME_TABLE_ID;";
                        break;
                    }
                #endregion

                #region Pedagogic method 
                case TimeTableSQLCommands.FetchHourIdByUserIdandAcYear:
                    {
                        query = @"SELECT 
                                         sh.HOUR_ID, HOUR
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tt.Staff_Id = ?STAFF_ID
                                            AND tt.IS_DELETED !=1
                                            INNER JOIN
                                        sup_hours AS SH ON sh.Hour_Id = tt.Hour_Id
                                    WHERE
                                        DAY_ORDER_DATE = current_date();";
                        break;
                    }
                case TimeTableSQLCommands.FetchSupUnits:
                    {
                            query= @"SELECT 
                                        UNIT_ID, UNIT_NAME
                                    FROM
                                        sup_units
                                    WHERE
                                        IS_DELETED != 1";
                        break;
                    }
                case TimeTableSQLCommands.FetchSupUnitsForGlobal:
                    {
                        query = @"SELECT 
                                        UNIT_ID, UNIT_NAME
                                    FROM
                                        sup_units
                                    WHERE
                                        IS_DELETED != 1 order by UNIT_ID desc";
                        break;
                    }
                case TimeTableSQLCommands.FetchSupMethodology:
                    {
                            query= @"SELECT 
                                        METHODOLOGY_ID, METHODOLOGY_NAME
                                    FROM
                                        sup_methodology
                                    WHERE
                                        IS_DELETED != 1 and IS_ACTIVE=1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchSupMethodologyDesc:
                    {
                        query = @"SELECT 
                                        METHODOLOGY_ID, METHODOLOGY_NAME
                                    FROM
                                        sup_methodology
                                    WHERE
                                        IS_DELETED != 1 and IS_ACTIVE=1 order by METHODOLOGY_ID DESC;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCourseAndStaffInTimtablePM:
                    {
                        query = @"SELECT 
                                        sh.HOUR_ID,
                                        HOUR,
                                        ccr.COURSE_CODE,
                                        ccr.COURSE_TITLE,
                                        TT.COURSE_ID,
                                        cc.CLASS_NAME,
                                         ccs.SHIFT,
                                        CC.CLASS_ID,
                                        CLASS_CODE,
                                        DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE,
                                        tpm.PENDAGOGIC_ID,
                                        tpm.TOPIC_TITLE,
                                        tpm.SUP_TOPIC,
                                        tpm.UNIT_ID,
                                          group_concat(distinct(tpm.METHODOLOGY_ID)) AS METHODOLOGY_ID,
                                       group_concat(distinct( sm.METHODOLOGY_NAME)) AS METHODOLOGY_NAME,
                                        su.UNIT_NAME,
                                        spi.FIRST_NAME
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tt.STAFF_ID = ?STAFF_ID
                                            AND tt.IS_DELETED !=1
                                            INNER JOIN
                                        cp_class_course_staff AS ccs ON ccs.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            AND ccs.COURSE_ID = tt.Course_Id
                                            AND ccs.CLASS_ID = tt.Class_Id
                                            AND ccs.STAFF_ID = tt.Staff_Id
                                            AND ccs.SEMESTER_ID = tt.Semester_Id
                                            and ccs.IS_DELETED !=1
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.course_root_id = tt.course_id
                                            AND ccr.SEMESTER_ID = tt.Semester_Id
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.class_id
                                            LEFT JOIN
                                        tt_pendagogic_method_?AC_YEAR AS tpm ON tpm.HOUR_ID = tt.HOUR_ID
                                            AND tpm.date = c.DAY_ORDER_DATE
                                            AND tpm.CLASS_ID = tt.Class_Id
                                            AND tpm.IS_DELETED !=1
                                           AND tpm.COURSE_ID = tt.COURSE_ID
                                             and tpm.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            LEFT JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            LEFT JOIN
                                        sup_units AS su ON su.UNIT_ID = tpm.UNIT_ID
                                            INNER JOIN
                                        sup_hours AS SH ON sh.HOUR_ID = tt.HOUR_ID
                                            INNER JOIN
                                        sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                            AND ss.IS_ACTIVE = 1
                                            LEFT JOIN
                                        stf_personal_info AS spi ON spi.STAFF_ID = tpm.SUP_STAFF
                                    WHERE
                                        DAY_ORDER_DATE = '?DAY_ORDER_DATE'
                                    GROUP BY sh.HOUR_ID , tt.COURSE_ID , tt.CLASS_ID , DAY_ORDER_DATE order by ccs.SHIFT,HOUR;";
                        break;
                    }
                case TimeTableSQLCommands.SavePendagogicMethod:
                    {
                        query = @"insert into tt_pendagogic_method_?AC_YEAR( 
                                                                COURSE_ID,
                                                                 DATE, 
                                                                 STAFF_ID, 
                                                                 CLASS_ID,
                                                                 HOUR_ID,
                                                                 METHODOLOGY_ID,
                                                                 UNIT_ID,
                                                                 TOPIC_TITLE,
                                                                 SUP_TOPIC,
                                                                 ACADEMIC_YEAR, 
                                                                 SEMESTER_TYPE,
                                                                 ENTRY_ID,
                                                                 SUP_STAFF,
                                                                 IS_GLOBAL)
                                                            values(
                                                                 ?COURSE_ID,
                                                                 ?DATE, 
                                                                 ?STAFF_ID, 
                                                                 ?CLASS_ID,
                                                                 ?HOUR_ID,
                                                                 ?METHODOLOGY_ID,
                                                                 ?UNIT_ID,
                                                                 ?TOPIC_TITLE,
                                                                 ?SUP_TOPIC,
                                                                 ?ACADEMIC_YEAR, 
                                                                 ?SEMESTER_TYPE,
                                                                 ?ENTRY_ID,
                                                                 ?SUP_STAFF,
                                                                 ?IS_GLOBAL)";
                        break;
                    }
                case TimeTableSQLCommands.FetchProgrammebyDeptandShift:
                    {
                        query = @"SELECT 
                                    PG.PROGRAMME_GROUP_ID AS 'PROGRAMME_ID',
                                    CONCAT(P.PROGRAMME_NAME,
                                            ' ',
                                            PM.PROGRAMME_MODE,
                                            ' ( SHIFT ',
                                            S.SHIFT_NAME,
                                            ' )') AS 'PROGRAMME_NAME'
                                FROM
                                    CP_PROGRAMME_GROUP AS PG
                                        INNER JOIN
                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                        AND P.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_SHIFT AS S ON S.SHIFT_ID = PG.SHIFT
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                WHERE
                                    p.DEPARTMENT IN (?DEPARTMENT)
                                        AND PG.SHIFT IN (?SHIFT_ID)
                                        AND PG.IS_DELETED != 1
                                        AND pg.APPTYPE_ID IN (1,2);";
                        break;
                    }
                case TimeTableSQLCommands.FetchClassesByProgrammeId:
                    {
                        query = @"SELECT 
                                        CLASS_ID, CLASS_NAME, CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID)
                                            AND CLASS_TYPE = 2
                                            AND IS_DELETED != 1
                                    ORDER BY CLASS_LEVEL, CLASS_YEAR;";
                        break;
                    }
                case TimeTableSQLCommands.FetchAvailapleStaffList:
                    {
                        query = @"SELECT 
                                        spi.STAFF_ID,
                                        CONCAT(spi.FIRST_NAME,
                                                ' (',
                                                spi.STAFF_CODE,
                                                ')') AS FIRST_NAME
                                    FROM
                                        stf_personal_info AS SPI
                                    WHERE
                                        spi.IS_DELETED != 1 AND spi.IS_LEFT != 1
                                            AND NON_TEACHING_CATEGORY != 1
                                            AND STAFF_ID NOT IN (SELECT 
                                                TT.STAFF_ID
                                            FROM
                                                CALENDER AS C
                                                    INNER JOIN
                                                TT_TIMETABLE AS TT ON TT.DAY_ORDER_ID = C.DAY_ORDER
                                                    AND TT.IS_DELETED != 1
                                                    INNER JOIN
                                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                                    AND CCS.COURSE_ID = TT.COURSE_ID
                                                    AND CCS.CLASS_ID = TT.CLASS_ID
                                                    AND CCS.SEMESTER_ID = TT.SEMESTER_ID
                                                    AND CCS.STAFF_ID = TT.STAFF_ID
                                                    INNER JOIN
                                                SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                                    AND SST.IS_ACTIVE = 1
                                            WHERE
                                                tt.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND DAY_ORDER_DATE = '?DAY_ORDER_DATE'
                                                    AND tt.HOUR_ID = ?HOUR_ID
                                                    AND ccs.SHIFT = ?SHIFT
                                            GROUP BY tt.STAFF_ID);";
                        break;
                    }
                case TimeTableSQLCommands.FetchTPMOverAllReportforIndividualStaff:
                    {
                        query = @"SELECT 
                                        sh.HOUR_ID,
                                        HOUR,
                                        ccr.COURSE_CODE,
                                        ccr.COURSE_TITLE,
                                        TT.COURSE_ID,
                                        cc.CLASS_NAME,
                                        CC.CLASS_ID,
                                        CLASS_CODE,
                                        DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE,
                                        tpm.PENDAGOGIC_ID,
                                        tpm.TOPIC_TITLE,
                                        tpm.SUP_TOPIC,
                                        tpm.UNIT_ID,
                                          group_concat(distinct(tpm.METHODOLOGY_ID)) AS METHODOLOGY_ID,
                                       group_concat(distinct( sm.METHODOLOGY_NAME)) AS METHODOLOGY_NAME,
                                        su.UNIT_NAME,
                                        spi.FIRST_NAME
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tt.STAFF_ID = ?STAFF_ID
                                            AND tt.IS_DELETED !=1
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.course_root_id = tt.course_id
                                            AND ccr.SEMESTER_ID = tt.Semester_Id
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.class_id
                                            inner JOIN
                                        tt_pendagogic_method_?AC_YEAR AS tpm ON tpm.HOUR_ID = tt.HOUR_ID
                                            AND tpm.date = c.DAY_ORDER_DATE
                                            AND tpm.CLASS_ID = tt.Class_Id
                                            AND tpm.COURSE_ID = tt.COURSE_ID
                                             and tpm.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            AND tpm.IS_DELETED !=1
                                            LEFT JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            LEFT JOIN
                                        sup_units AS su ON su.UNIT_ID = tpm.UNIT_ID
                                            INNER JOIN
                                        sup_hours AS SH ON sh.HOUR_ID = tt.HOUR_ID
                                            INNER JOIN
                                        sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                            AND tt.SEMESTER_ID IN (?SEMESTER_ID)
                                            left JOIN
                                        stf_personal_info AS spi ON spi.STAFF_ID = tpm.SUP_STAFF
                                    WHERE
                                        DAY_ORDER_DATE between '?DATE' and '?TODATE' 
                                        
                                    GROUP BY sh.HOUR_ID , tt.COURSE_ID , tt.Class_Id ,tpm.STAFF_ID, DAY_ORDER_DATE order by substring(DAY_ORDER_DATE,1,2) desc,substring(DAY_ORDER_DATE,2,4)desc,substring(DAY_ORDER_DATE,4,7)desc ;";
                        break;
                    }
                    case TimeTableSQLCommands.FetchTotalHoursbyDayorderDateWise:
                    {
                        query = @"SELECT 
                                        COUNT(tt.HOUR_ID) AS TOTAL_HOUR,
                                        TT.COURSE_ID,

                                        DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tt.STAFF_ID = ?STAFF_ID
                                            AND tt.IS_DELETED !=1
                                            INNER JOIN
                                        sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                            AND tt.SEMESTER_ID IN (?SEMESTER_ID)
                                    WHERE
                                        DAY_ORDER_DATE BETWEEN '?DATE' AND '?TODATE'
                                            AND c.IS_HOLIDAY != 1
                                    GROUP BY tt.STAFF_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTotalEntryHourByEntryDate:
                    {
                        query = @"SELECT 
                                    COUNT(tt.HOUR_ID) AS ENTRY_HOUR,
                                    TT.COURSE_ID,
                                    tt.DAY_ORDER_ID,
                                    DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE
                                FROM
                                    calender AS c
                                        INNER JOIN
                                    tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                        AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                        AND STAFF_ID = ?STAFF_ID
                                        AND tt.IS_DELETED !=1
                                        INNER JOIN
                                    sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                        AND tt.SEMESTER_ID IN (?SEMESTER_ID)
                                        INNER JOIN
                                    tt_pendagogic_method_?AC_YEAR AS tpm ON tpm.Class_Id = tt.Class_Id
                                        AND tpm.academic_year = tt.ACADEMIC_YEAR
                                        AND tpm.hour_id = tt.Hour_Id
                                        AND TPM.DATE = c.DAY_ORDER_DATE
                                        AND tpm.COURSE_ID = tt.COURSE_ID
                                WHERE
                                    TPM.DATE BETWEEN '?DATE' AND '?TODATE'
                                        AND tpm.IS_DELETED !=1
                                        AND c.IS_HOLIDAY != 1 group by tpm.DATE,tpm.CLASS_ID,tpm.HOUR_ID ;";
                        break;
                    }
                case TimeTableSQLCommands.FetchTTClassbyStaffID:
                    {
                        query = @"SELECT 
                                        cc.CLASS_ID, cc.CLASS_NAME, cc.CLASS_CODE
                                    FROM
                                        tt_timetable AS tt
                                            INNER JOIN
                                        cp_class_course_staff AS cccs ON cccs.STAFF_ID = tt.staff_id
                                            AND tt.Course_Id = cccs.COURSE_ID
                                            AND tt.Class_Id = cccs.CLASS_ID
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.Class_Id
                                    WHERE
                                        tt.STAFF_ID = ?STAFF_ID
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                           and tt.IS_DELETED !=1
                                    GROUP BY cc.CLASS_ID, tt.STAFF_ID";
                        break;
                    }
                case TimeTableSQLCommands.FetchTTClassbyStaffIDAcYearSemWise:
                    {
                        query = @"SELECT 
                                        cc.CLASS_ID, cc.CLASS_NAME, cc.CLASS_CODE
                                    FROM
                                        tt_timetable AS tt
                                            INNER JOIN
                                        cp_class_course_staff AS cccs ON cccs.STAFF_ID = tt.staff_id
                                            AND tt.Course_Id = cccs.COURSE_ID
                                            AND tt.Class_Id = cccs.CLASS_ID
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.Class_Id
                                    WHERE
                                        tt.STAFF_ID = ?STAFF_ID 
                                        and tt.SEMESTER_ID in (?SEMESTER_ID)
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                           and tt.IS_DELETED !=1
                                    GROUP BY cc.CLASS_ID, tt.STAFF_ID";
                        break;
                    }
                case TimeTableSQLCommands.FetchCourseByClassIdInTT:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID, concat(CCR.COURSE_TITLE,'-', ccr.COURSE_CODE) as COURSE_TITLE
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCCS ON CCCS.STAFF_ID = TT.STAFF_ID
                                            AND TT.COURSE_ID = CCCS.COURSE_ID
                                            AND TT.CLASS_ID = CCCS.CLASS_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = TT.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = TT.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                    WHERE
                                            TT.STAFF_ID = ?STAFF_ID
                                            and tt.SEMESTER_ID in (?SEMESTER_ID) 
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.CLASS_ID IN (?CLASS_ID)
                                            AND TT.IS_DELETED != 1
                                    GROUP BY TT.COURSE_ID , TT.STAFF_ID , TT.CLASS_ID;";
                            break;
                    }
                case TimeTableSQLCommands.BindCITSReportCourseWiseIndividualReport:
                    {
                        query = @"SELECT 
                                        sh.HOUR_ID,
                                        HOUR,
                                        ccr.COURSE_CODE,
                                        ccr.COURSE_TITLE,
                                        TT.COURSE_ID,
                                        cc.CLASS_NAME,
                                        CC.CLASS_ID,
                                        CLASS_CODE,
                                        DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE,
                                        tpm.PENDAGOGIC_ID,
                                        tpm.TOPIC_TITLE,
                                        tpm.SUP_TOPIC,
                                        tpm.UNIT_ID,
                                          group_concat(distinct(tpm.METHODOLOGY_ID)) AS METHODOLOGY_ID,
                                       group_concat(distinct( sm.METHODOLOGY_NAME)) AS METHODOLOGY_NAME,
                                        su.UNIT_NAME,
                                        spi.FIRST_NAME,
                                        concat(spinew.FIRST_NAME,' (',spinew.STAFF_CODE,')') as STAFF_NAME
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND STAFF_ID = ?STAFF_ID
                                            AND tt.IS_DELETED !=1
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.course_root_id = tt.course_id
                                            AND ccr.SEMESTER_ID = tt.Semester_Id
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.class_id
                                            inner JOIN
                                        tt_pendagogic_method_?AC_YEAR AS tpm ON tpm.HOUR_ID = tt.HOUR_ID
                                            AND tpm.date = c.DAY_ORDER_DATE
                                            AND tpm.CLASS_ID = tt.Class_Id
                                            AND tpm.COURSE_ID = tt.COURSE_ID
                                            AND tpm.IS_DELETED !=1
                                             and tpm.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            LEFT JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            LEFT JOIN
                                        sup_units AS su ON su.UNIT_ID = tpm.UNIT_ID
                                            INNER JOIN
                                        sup_hours AS SH ON sh.HOUR_ID = tt.HOUR_ID
                                            INNER JOIN
                                        sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                            left JOIN
                                        stf_personal_info AS spi ON spi.STAFF_ID = tpm.SUP_STAFF
                                            INNER JOIN
                                        stf_personal_info AS spinew ON spinew.STAFF_ID = tt.Staff_Id
                                    WHERE
                                      
                                        tpm.COURSE_ID in(?COURSE_ID)
                                         and tt.SEMESTER_ID in (?SEMESTER_ID) 
                                        and tpm.CLASS_ID in (?CLASS_ID)
                                    GROUP BY sh.HOUR_ID , tt.COURSE_ID , tt.Class_Id ,tpm.STAFF_ID, DAY_ORDER_DATE order by substring(DAY_ORDER_DATE,1,2) desc,substring(DAY_ORDER_DATE,2,4)desc,substring(DAY_ORDER_DATE,4,7)desc ";
                        break;
                    }
                case TimeTableSQLCommands.FetchMethodologyCountByUserID:
                    {
                        query = @"SELECT 
                                        COUNT(tpm.METHODOLOGY_ID) AS METHODOLOGY_COUNT,
                                        tpm.METHODOLOGY_ID,
                                        sm.METHODOLOGY_NAME
                                    FROM
                                        tt_pendagogic_method_?AC_YEAR AS tpm
                                            INNER JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                    WHERE
                                         CLASS_ID in(?CLASS_ID) -- AND STAFF_ID IN (?STAFF_ID) 
                                            AND COURSE_ID in (?COURSE_ID)  and tpm.SEMESTER_TYPE in (?SEMESTER_TYPE)
                                            AND tpm.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tpm.IS_DELETED != 1
                                    GROUP BY tpm.METHODOLOGY_ID order by tpm.METHODOLOGY_ID; ";
                        break;
                    }
                case TimeTableSQLCommands.FetchSubjectWiseTpmReportForStaff:
                    {
                        query = @"SELECT 
                                        sh.HOUR_ID,
                                        HOUR,
                                        ccr.COURSE_CODE,
                                        ccr.COURSE_TITLE,
                                        TT.COURSE_ID,
                                        cc.CLASS_NAME,
                                        CC.CLASS_ID,
                                        CLASS_CODE,
                                        DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE,
                                        tpm.PENDAGOGIC_ID,
                                        tpm.TOPIC_TITLE,
                                        tpm.SUP_TOPIC,
                                        tpm.UNIT_ID,
                                         group_concat(distinct(tpm.METHODOLOGY_ID)) AS METHODOLOGY_ID,
                                       group_concat(distinct( sm.METHODOLOGY_NAME)) AS METHODOLOGY_NAME,
                                        su.UNIT_NAME,
                                        spi.FIRST_NAME,
                                        CONCAT(spinew.FIRST_NAME,
                                                ' (',
                                                spinew.STAFF_CODE,
                                                ')') AS STAFF_NAME,
                                        tpm.SUP_STAFF,
                                        tpm.IS_GLOBAL,
                                        spinew.STAFF_ID
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tt.STAFF_ID in( ?STAFF_ID)
                                            and tt.SEMESTER_ID in (?SEMESTER_ID) 
                                            AND tt.IS_DELETED != 1
                                            INNER JOIN
                                        cp_class_course_staff AS ccs ON ccs.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            AND ccs.COURSE_ID = tt.Course_Id
                                            AND ccs.CLASS_ID = tt.Class_Id
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.course_root_id = tt.course_id
                                            AND ccr.SEMESTER_ID = tt.Semester_Id
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.CLASS_ID
                                            INNER JOIN
                                        tt_pendagogic_method_?AC_YEAR AS tpm ON tpm.HOUR_ID = tt.HOUR_ID
                                            AND tpm.DATE = c.DAY_ORDER_DATE
                                            AND tpm.CLASS_ID = tt.CLASS_ID
                                            AND tpm.COURSE_ID = tt.COURSE_ID
                                            AND tpm.IS_DELETED != 1
                                            AND tpm.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            LEFT JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            LEFT JOIN
                                        sup_units AS su ON su.UNIT_ID = tpm.UNIT_ID
                                            INNER JOIN
                                        sup_hours AS SH ON sh.HOUR_ID = tt.HOUR_ID
                                            INNER JOIN
                                        sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                            
                                            LEFT JOIN
                                        stf_personal_info AS spi ON spi.STAFF_ID = tpm.SUP_STAFF
                                            INNER JOIN
                                        stf_personal_info AS spinew ON spinew.STAFF_ID = tt.Staff_Id
                                    WHERE
                                        ccr.IS_DELETED != 1
                                        group by TPM.COURSE_ID,tpm.CLASS_ID,tpm.UNIT_ID,tpm.HOUR_ID,tpm.date,tpm.STAFF_ID
                                    ORDER BY tt.Course_Id , tt.Class_Id;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCITSReportForHOD:
                    {
                        query = @"SELECT 
                                        sh.HOUR_ID,
                                        HOUR,
                                        ccr.COURSE_CODE,
                                        ccr.COURSE_TITLE,
                                        TT.COURSE_ID,
                                        cc.CLASS_NAME,
                                        CC.CLASS_ID,
                                        CLASS_CODE,
                                        DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE,
                                        tpm.PENDAGOGIC_ID,
                                        tpm.TOPIC_TITLE,
                                        tpm.SUP_TOPIC,
                                        tpm.UNIT_ID,
                                         group_concat(distinct(tpm.METHODOLOGY_ID)) AS METHODOLOGY_ID,
                                       group_concat(distinct( sm.METHODOLOGY_NAME)) AS METHODOLOGY_NAME,
                                        su.UNIT_NAME,
                                        spi.FIRST_NAME,
                                        CONCAT(spinew.FIRST_NAME,
                                                ' (',
                                                spinew.STAFF_CODE,
                                                ')') AS STAFF_NAME,
                                        tpm.SUP_STAFF,
                                        tpm.IS_GLOBAL,
                                        spinew.STAFF_ID
                                    FROM
                                        calender AS c
                                            INNER JOIN
                                        tt_timetable AS tt ON tt.Day_Order_Id = c.DAY_ORDER
                                            AND tt.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tt.STAFF_ID in( ?STAFF_ID)
                                            and tt.SEMESTER_ID in (?SEMESTER_ID) 
                                            AND tt.IS_DELETED != 1
                                            INNER JOIN
                                        cp_class_course_staff AS ccs ON ccs.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            AND ccs.COURSE_ID = tt.Course_Id
                                            AND ccs.CLASS_ID = tt.Class_Id
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.course_root_id = tt.course_id
                                            AND ccr.SEMESTER_ID = tt.Semester_Id
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = tt.CLASS_ID
                                            INNER JOIN
                                        tt_pendagogic_method_?AC_YEAR AS tpm ON tpm.HOUR_ID = tt.HOUR_ID
                                            AND tpm.DATE = c.DAY_ORDER_DATE
                                            AND tpm.STAFF_ID = tt.STAFF_ID
                                            AND tpm.IS_DELETED != 1
                                            AND tpm.ACADEMIC_YEAR = tt.ACADEMIC_YEAR
                                            LEFT JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            LEFT JOIN
                                        sup_units AS su ON su.UNIT_ID = tpm.UNIT_ID
                                            INNER JOIN
                                        sup_hours AS SH ON sh.HOUR_ID = tt.HOUR_ID
                                            INNER JOIN
                                        sup_semester AS ss ON ss.SUP_SEMESTER_ID = tt.SEMESTER_ID
                                            
                                            LEFT JOIN
                                        stf_personal_info AS spi ON spi.STAFF_ID = tpm.SUP_STAFF
                                            INNER JOIN
                                        stf_personal_info AS spinew ON spinew.STAFF_ID = tt.Staff_Id
                                    WHERE
                                        ccr.IS_DELETED != 1
                                        group by TPM.COURSE_ID,tpm.CLASS_ID,tpm.UNIT_ID,tpm.HOUR_ID,tpm.date,tpm.STAFF_ID
                                    ORDER BY tt.Course_Id , tt.Class_Id;";
                        break;
                    }
                case TimeTableSQLCommands.FetchMethTotalCountbyCourseWise:
                    {
                        query = @"SELECT 
                                        COUNT(tpm.METHODOLOGY_ID) AS METHODOLOGY_COUNT,
                                        tpm.METHODOLOGY_ID,
                                        sm.METHODOLOGY_NAME,
                                        tpm.COURSE_ID,
                                        tpm.CLASS_ID
                                    FROM
  
                                        tt_pendagogic_method_?AC_YEAR AS tpm 
                                            INNER JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            INNER JOIN
                                        sup_semester_type AS sst ON sst.SEMESTER_TYPE_ID = tpm.SEMESTER_TYPE
                                            AND sst.SEMESTER_TYPE_ID in (?SEMESTER_TYPE_ID)
                                    WHERE
                                        tpm.STAFF_ID IN (?STAFF_ID)
                                            AND tpm.ACADEMIC_YEAR = ?AC_YEAR

                                            AND tpm.IS_DELETED != 1
                                    GROUP BY tpm.METHODOLOGY_ID ,tpm.COURSE_ID
                                    ORDER BY tpm.COURSE_ID , tpm.METHODOLOGY_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCITSAdministratorEntryList:
                    {
                        query = @"SELECT 
                                    tpm.TOPIC_TITLE,
                                    tpm.SUP_TOPIC,
                                    tpm.METHODOLOGY_ID,
                                    cc.CLASS_NAME,
                                    cc.CLASS_CODE,
                                    CONCAT(spi.FIRST_NAME,
                                            ' (',
                                            spi.STAFF_CODE,
                                            ')') AS FIRST_NAME,
                                    group_concat(distinct( sm.METHODOLOGY_NAME)) AS METHODOLOGY_NAME,
                                    su.UNIT_NAME,
                                    sh.HOUR,
                                    DATE_FORMAT(tpm.DATE, '%d/%m/%Y') AS DATE
                                FROM
                                    tt_pendagogic_method_?AC_YEAR AS tpm
                                        INNER JOIN
                                    cp_classes AS cc ON cc.CLASS_ID = tpm.CLASS_ID
                                        AND cc.IS_DELETED != 1
                                        INNER JOIN
                                    stf_personal_info AS spi ON spi.STAFF_ID = tpm.ENTRY_ID
                                        LEFT JOIN
                                    sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                        LEFT JOIN
                                    sup_units AS su ON su.UNIT_ID = tpm.UNIT_ID
                                        INNER JOIN
                                    sup_hours AS sh ON sh.HOUR_ID = tpm.HOUR_ID
                                        INNER JOIN
                                    sup_semester_type AS sst ON sst.SEMESTER_TYPE_ID = tpm.SEMESTER_TYPE
                                        AND sst.SEMESTER_TYPE_ID in (?SEMESTER_TYPE_ID)
                                WHERE
                                    tpm.ACADEMIC_YEAR = ?AC_YEAR
                                        AND tpm.IS_DELETED != 1
                                        AND tpm.IS_GLOBAL = 1
                                GROUP BY tpm.DATE , tpm.CLASS_ID , tpm.HOUR_ID
                                ORDER BY SUBSTRING(tpm.DATE, 1, 2) DESC , SUBSTRING(tpm.DATE, 2, 4) DESC , SUBSTRING(tpm.DATE, 4, 7) DESC";
                        break;
                    }
                case TimeTableSQLCommands.FetchDepartmentWiseStaffList:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        CONCAT(FIRST_NAME, '(', STAFF_CODE, ')') AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        DEPARTMENT = ?DEPARTMENT and IS_DELETED !=1";
                        break;
                    }
                case TimeTableSQLCommands.FetchMethTotalCountbyCourseClassWise:
                    {
                        query = @"SELECT 
                                        COUNT(tpm.METHODOLOGY_ID) AS METHODOLOGY_COUNT,
                                        tpm.METHODOLOGY_ID,
                                        sm.METHODOLOGY_NAME,
                                        tpm.COURSE_ID,
                                        tpm.CLASS_ID
                                        
                                    FROM
  
                                        tt_pendagogic_method_?AC_YEAR AS tpm 
                                            INNER JOIN
                                        sup_methodology AS sm ON sm.METHODOLOGY_ID = tpm.METHODOLOGY_ID
                                            INNER JOIN
                                        sup_semester_type AS sst ON sst.SEMESTER_TYPE_id = tpm.SEMESTER_TYPE
                                            AND sst.SEMESTER_TYPE_ID in (?SEMESTER_TYPE_ID)
                                    WHERE
                                        tpm.STAFF_ID IN (?STAFF_ID)
                                            AND tpm.ACADEMIC_YEAR = ?AC_YEAR
                                            AND tpm.IS_DELETED != 1
                                    GROUP BY tpm.METHODOLOGY_ID ,tpm.COURSE_ID,tpm.CLASS_ID
                                    ORDER BY tpm.COURSE_ID , tpm.METHODOLOGY_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchMethodologyCountByStaffID:
                    {
                        query = @"SELECT 
                                        COUNT(DISTINCT (tpm.pendagogic_id)) AS METHODOLOGY_COUNT,
                                        TPM.METHODOLOGY_ID,
                                        SM.METHODOLOGY_NAME,
                                        TPM.COURSE_ID,
                                        TPM.CLASS_ID,
                                        TT.STAFF_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        TT_PENDAGOGIC_METHOD_?AC_YEAR AS TPM ON TT.CLASS_ID = TPM.CLASS_ID
                                            AND TT.COURSE_ID = TPM.COURSE_ID
                                            AND tt.STAFF_ID IN (?STAFF_ID)
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        SUP_METHODOLOGY AS SM ON SM.METHODOLOGY_ID = TPM.METHODOLOGY_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = TPM.SEMESTER_TYPE
                                            AND SST.SEMESTER_TYPE_ID IN (?SEMESTER_TYPE_ID)
                                    WHERE
                                        TPM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TPM.IS_DELETED != 1
                                    GROUP BY TPM.METHODOLOGY_ID , TPM.COURSE_ID , TPM.CLASS_ID
                                    ORDER BY TPM.COURSE_ID , TPM.METHODOLOGY_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchMethodologyCountByStaffIDforHODPDF:
                    {
                        query = @"SELECT 
                                        COUNT(DISTINCT (tpm.pendagogic_id)) AS METHODOLOGY_COUNT,
                                        TPM.METHODOLOGY_ID,
                                        SM.METHODOLOGY_NAME,
                                        TPM.COURSE_ID,
                                        TPM.CLASS_ID,
                                        TT.STAFF_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        TT_PENDAGOGIC_METHOD_?AC_YEAR AS TPM ON TT.CLASS_ID = TPM.CLASS_ID
                                            AND TT.COURSE_ID = TPM.COURSE_ID
                                            AND tpm.STAFF_ID IN (?STAFF_ID)
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        SUP_METHODOLOGY AS SM ON SM.METHODOLOGY_ID = TPM.METHODOLOGY_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = TPM.SEMESTER_TYPE
                                            AND SST.SEMESTER_TYPE_ID IN (?SEMESTER_TYPE_ID)
                                    WHERE
                                        TPM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TPM.IS_DELETED != 1
                                    GROUP BY TPM.METHODOLOGY_ID , TPM.COURSE_ID , TPM.CLASS_ID
                                    ORDER BY TPM.COURSE_ID , TPM.METHODOLOGY_ID;";
                        break;
                    }
                case TimeTableSQLCommands.FetchDayorderByDate:
                    {

                        query = @"SELECT 
                                            DAY_ORDER as DAY_ORDER_ID, DAY_ORDER_DATE
                                        FROM
                                            calender
                                        WHERE
                                            DAY_ORDER_DATE = '?DAY_ORDER_DATE'
                                                AND ACADEMIC_YEAR = ?AC_YEAR
                                                AND IS_DELETED != 1;";
                        break;
                    }
                case TimeTableSQLCommands.FetchCoursebyDayOrderandClassHour:
                    {

                        query = @"SELECT 
                                        COURSE_ID,CLASS_ID
                                    FROM
                                        tt_timetable
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1
                                            AND CLASS_ID = ?CLASS_ID
                                            AND DAY_ORDER_ID = ?DAY_ORDER_ID
                                            AND HOUR_ID = ?HOUR_ID
                                    GROUP BY Course_Id , Class_Id";
                        break;
                    }
                case TimeTableSQLCommands.CheckCITSAlreadyExistorNot:
                    {

                        query = @"SELECT 
                                    PENDAGOGIC_ID,
                                    COURSE_ID,
                                    STAFF_ID,
                                    CLASS_ID,
                                    HOUR_ID,
                                    SEMESTER_ID,
                                    SEMESTER_TYPE,
                                    DATE
                                FROM
                                    tt_pendagogic_method_?AC_YEAR
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR AND CLASS_ID = ?CLASS_ID
                                        AND DATE = '?DATE'
                                        AND HOUR_ID = ?HOUR_ID
                                        AND SEMESTER_TYPE = ?SEMESTER_TYPE
                                        AND IS_GLOBAL != 1;";
                        break;
                    }
                case TimeTableSQLCommands.IFExistDeleteCITS:
                    {

                        query = @"UPDATE tt_pendagogic_method_?AC_YEAR 
                                            SET 
                                                IS_DELETED = 1
                                            WHERE
                                                PENDAGOGIC_ID = ?PENDAGOGIC_ID;";
                        break;
                    }
                #endregion


                case TimeTableSQLCommands.FetchttStaffBasedSemtyeandAcyear:
                    {
                        query = @"SELECT 
                                        
                                        TT.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, ''),'- (',STAFF_CODE,')') AS FIRST_NAME
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        TT.ACADEMIC_YEAR = ?ACADEMIC_YEAR 
                                            AND TT.IS_DELETED != 1 GROUP BY TT.STAFF_ID
                                    ORDER BY SP.STAFF_ID ;";
                        break;
                    }
            }
            return query;
        }
    }
}
