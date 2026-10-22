using CMS.DAO;

namespace CMS.SQL.Dashboard
{
    public static class DashboardSQL
    {
        public static string GetNoticeBoardForAdminSQL()
        {
            return @"SELECT 
                        NOTICEBOARD_ID,
                        NOTICEBOARD,
                        IFNULL(FILE_NAME, '') AS FILE_NAME,
                        ROLE_ID,
                        APPTYPE,
                        CLASS_YEAR,
						ROLE_ID,
                        IFNULL(LINK_UPLOAD, '') AS LINK_UPLOAD,
                        IS_ACTIVE,
                        IS_DELETED,
                        IFNULL(DESCRIPTION, '') AS DESCRIPTION,
                        IFNULL(FILE_TYPE, '') AS FILE_TYPE,
                        IFNULL(DUE_DATE, '') AS DUE_DATE,
                        IFNULL(SHIFT, '') AS SHIFT,
                        IS_COMMENT_ENABLE,
                        IS_FILE_UPLOAD_ENABLE,
                        (SELECT COUNT(*) FROM db_noticeboard_view_log WHERE NOTICEBOARD_ID = DB_NOTICEBOARD.NOTICEBOARD_ID) AS VIEW_COUNT,
                        (SELECT COUNT(*) FROM db_noticeboard_log WHERE NOTICEBOARD_ID = DB_NOTICEBOARD.NOTICEBOARD_ID) AS RESPONSE_COUNT,
                        (SELECT GROUP_CONCAT(ROLE_NAME) FROM SUP_ROLE WHERE FIND_IN_SET(ROLE_ID, DB_NOTICEBOARD.ROLE_ID)) AS ROLE_NAME
                    FROM 
                        DB_NOTICEBOARD 
                    WHERE 
                        IS_ACTIVE = 1 
                        AND IS_DELETED != 1 and DUE_DATE>=curdate()
                        AND (FIND_IN_SET(?ROLE_ID, ROLE_ID) > 0 OR ?ROLE_ID IS NULL)
                    ORDER BY NOTICEBOARD_ID DESC;";
        }

        public static string GetDashboardSQL(DashboardSQLCommands sEnumCommand)
        {
            string query = "";

            switch (sEnumCommand)
            {
                case DashboardSQLCommands.FetchStudentInfoByUserId:
                    {
                        //need to remove academic year which is hardcode here...

                        query = @"SELECT 
                                    ADMISSION_NO,
                                    ADMITTED_CLASS,
                                    DEPT_ID,
                                    SHIFT_ID,
                                    FIRST_NAME,
                                    CLS.CLASS_NAME,
                                    SCLS.STUDENT_ID,
                                    SP.PHOTO,
                                    SCLS.CLASS_ID,
                                    SCLS.PROGRAMME_GROUP_ID,
                                    SP.ROLL_NO,
                                    CP.PROGRAMME_NAME,
                                    B.BATCH,
                                    SP.BATCH AS ADMITTED_BATCH,
                                    CLS.CLASS_LEVEL,
                                    SP.STU_EMAILID,
                                    SP.STU_MOBILENO
                                FROM
                                    STU_PERSONAL_INFO SP
                                        INNER JOIN
                                    STU_CLASS SCLS ON SCLS.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES CLS ON CLS.CLASS_ID = SCLS.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SCLS.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        AND CP.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_BATCHES AS B ON B.BATCH_ID = SCLS.BATCH
                                        AND B.IS_DELETED != 1
                                WHERE
                                    SP.STUDENT_ID =?STUDENT_ID
                                        AND SCLS.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case DashboardSQLCommands.FetchStudentacademicDetails:
                    {
                        //need to remove academic year which is hardcode here...

                        query = @"SELECT 
                                        SPI.STUDENT_ID, CC.CLASS_YEAR,CC.SHIFT,CC.CLASS_LEVEL
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SPI.IS_DELETED != 1 AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SPI.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchNoticeBoardByUserIdforStudent:
                    {
                        query = @"SELECT 
                                    dn.NOTICEBOARD_ID,
                                    dn.NOTICEBOARD,
                                    IFNULL(FILE_NAME, '') AS FILE_NAME,
                                    dn.ROLE_ID,
                                    dnl.NOTICEBOARD_LOG_ID,
                                    APPTYPE,
                                    CLASS_YEAR,
                                    IFNULL(LINK_UPLOAD, '') AS LINK_UPLOAD,
                                    dn.IS_ACTIVE,
                                    IFNULL(DESCRIPTION, '') AS DESCRIPTION,
                                    IFNULL(FILE_TYPE, '') AS FILE_TYPE,
                                    IFNULL(DUE_DATE, '') AS DUE_DATE,
                                    IFNULL(SHIFT, '') AS SHIFT,
                                    IS_COMMENT_ENABLE,
                                    IS_ACCEPTED,
                                    IS_CANCELED,
                                    IS_FILE_UPLOAD_ENABLE
                                FROM
                                    DB_NOTICEBOARD AS DN
                                        LEFT JOIN
                                    DB_NOTICEBOARD_LOG AS DNL ON DNL.NOTICEBOARD_ID = DN.NOTICEBOARD_ID
                                        AND DNL.USER_ID = ?USER_ID
                                        AND DN.ROLE_ID = ?ROLE_ID
                                        AND DNL.USER_ROLE = DN.ROLE_ID
                                WHERE
                                    dn.IS_ACTIVE = 1 AND dn.IS_DELETED != 1 
                                    and (FIND_IN_SET(?SHIFT, SHIFT) > 0 OR ?SHIFT IS NULL)
                                   AND (FIND_IN_SET(?CLASS_YEAR, CLASS_YEAR) > 0 OR ?CLASS_YEAR IS NULL)
                                  AND (FIND_IN_SET(?APPTYPE, APPTYPE) > 0 OR ?APPTYPE IS NULL)
                                        AND DUE_DATE >= CURDATE()
                                        AND (FIND_IN_SET(?ROLE_ID, ROLE_ID) > 0 OR ?ROLE_ID IS NULL)
                                ORDER BY NOTICEBOARD_ID DESC;";
                        break;
                    }

                case DashboardSQLCommands.FetchNoticeBoardReportByNoticeId:
                    {
                        query = @"SELECT 
                                    DNL.NOTICEBOARD_LOG_ID,
                                    DNL.NOTICEBOARD_ID,
                                    DNL.USER_ID,
                                    DNL.IS_VIEWED,
                                    DNL.COMMENTS,
                                    IFNULL(DNL.FILE_PATH, '') AS FILE_PATH,
                                    DATE_FORMAT(DNL.CREATED_DATE, '%d/%m/%Y %h:%i %p') AS CREATED_DATE,
                                    COALESCE(SP.FIRST_NAME, STF.FIRST_NAME, DNL.USER_ID) AS FIRST_NAME,
                                    COALESCE(SP.ROLL_NO, STF.STAFF_CODE, '') AS USER_CODE
                                FROM 
                                    DB_NOTICEBOARD_LOG AS DNL
                                LEFT JOIN 
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = DNL.USER_ID
                                LEFT JOIN 
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = DNL.USER_ID
                                WHERE 
                                    DNL.NOTICEBOARD_ID = ?NOTICEBOARD_ID
                                    AND (DNL.USER_ROLE = ?ROLE_ID OR ?ROLE_ID IS NULL)
                                ORDER BY DNL.CREATED_DATE DESC;";
                        break;
                    }

                case DashboardSQLCommands.FetchNoticeBoardByUserId:
                    {
                        query = @"SELECT 
                                    dn.NOTICEBOARD_ID,
                                    dn.NOTICEBOARD,
                                    IFNULL(FILE_NAME, '') AS FILE_NAME,
                                    dn.ROLE_ID,
                                    dnl.NOTICEBOARD_LOG_ID,
                                    APPTYPE,
                                    CLASS_YEAR,
                                    IFNULL(LINK_UPLOAD, '') AS LINK_UPLOAD,
                                    dn.IS_ACTIVE,
                                    IFNULL(DESCRIPTION, '') AS DESCRIPTION,
                                    IFNULL(FILE_TYPE, '') AS FILE_TYPE,
                                    IFNULL(DUE_DATE, '') AS DUE_DATE,
                                    IFNULL(SHIFT, '') AS SHIFT,
                                    IS_COMMENT_ENABLE,
                                    IS_ACCEPTED,
                                    IS_CANCELED,
                                    IS_FILE_UPLOAD_ENABLE
                                FROM
                                    DB_NOTICEBOARD AS DN
                                        LEFT JOIN
                                    DB_NOTICEBOARD_LOG AS DNL ON DNL.NOTICEBOARD_ID = DN.NOTICEBOARD_ID
                                        AND DNL.USER_ID = ?USER_ID
                                        AND DN.ROLE_ID = ?ROLE_ID
                                        AND DNL.USER_ROLE = DN.ROLE_ID
                                WHERE
                                    dn.IS_ACTIVE = 1 AND dn.IS_DELETED != 1
                                        AND DUE_DATE >= CURDATE()
                                        AND (FIND_IN_SET(?ROLE_ID, ROLE_ID) > 0 OR ?ROLE_ID IS NULL)
                                ORDER BY NOTICEBOARD_ID DESC;";
                        break;
                    }

                

                case  DashboardSQLCommands.FetchStaffInfoByUserId:{                      

                        break;
                    }

                case DashboardSQLCommands.GetFeedbackCountByStudent:
                    {
                        query = @"SELECT  COUNT(distinct ASSESSOR) AS NoOfStudentsFeedbackPosted FROM fbevaluation?AC_YEAR";
                        break;
                    }

                case DashboardSQLCommands.GetTotalNoOfStudents:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS STUDENT_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED!=1
		                                    AND SP.IS_LEFT!=1
                                            INNER JOIN
                                        cp_classes AS cc ON cc.CLASS_ID = sc.CLASS_ID
                                            AND cc.CLASS_LEVEL IN (1 , 2)
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case DashboardSQLCommands.FetchAssignmentInfo:
                    {
                        query = @"SELECT
                                    ASS.ASSIGNMENT_ID, 
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            '(',
                                            IFNULL(SP.STAFF_CODE, ''),
                                            ')') AS STAFF_NAME,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(CR.COURSE_CODE, ''),
                                            ')') AS COURSE_CODE,
                                    DATE_FORMAT(ASS.SUBMISSION_DATE, '%d/%m/%Y') AS SUBMISSION_DATE,
                                       DATE_FORMAT(ASS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    ASS.DESCRIPTION,
                                    UA.PHOTO,
                                    ASS.STAFF_ID,
                                    ASS.COURSE_ID,
                                    ASS.ASSIGNMENT_TITLE,
                                    ASS.IS_FILE_UPLOAD,
                                    ASS.CLASS_ID
                                FROM
                                    ASSIGNMENT_2019 AS ASS
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = ASS.CLASS_ID
                                        AND CLS.IS_DELETED != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.CLASS_ID = CLS.CLASS_ID
                                        AND SC.STUDENT_ID =?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ASS.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ASS.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASS_COURSE AS CC ON CC.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND CC.CLASS_ID = ASS.CLASS_ID
                                        AND CC.IS_DELETED != 1
                                        INNER JOIN
                                    USER_ACCOUNT AS UA ON UA.USERNAME = SP.STAFF_CODE
                                WHERE
                                    SC.CLASS_ID =?CLASS_ID
                                   AND ASS.IS_DELETED!=1
                                   AND DATE_FORMAT(ASS.SUBMISSION_DATE,'%Y-%m-%d')>=DATE_FORMAT(CURDATE(),'%Y-%m-%d')  GROUP BY ASS.ASSIGNMENT_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchStaffInfoByStudentId:
                    {
                        query = @"SELECT
                                    ASS.STAFF_ID,
                                    UA.PHOTO,
                                    ASS.COURSE_ID
                                FROM
                                    ASSIGNMENT_?AC_YEAR AS ASS
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = ASS.CLASS_ID
                                        AND CLS.IS_DELETED != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.CLASS_ID = CLS.CLASS_ID
                                        AND SC.STUDENT_ID = ?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ASS.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ASS.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASS_COURSE AS CC ON CC.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND CC.CLASS_ID = ASS.CLASS_ID
                                        AND CC.IS_DELETED != 1
                                        INNER JOIN
                                    USER_ACCOUNT AS UA ON UA.USERNAME = SP.STAFF_CODE
                                WHERE
                                    SC.CLASS_ID = ?CLASS_ID GROUP BY ASS.COURSE_ID,ASS.STAFF_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchRegisteredNMECourse:
                    {
                        query = @"SELECT 
                                    COURSE_TITLE
                                FROM
                                    NME_STUDENT_REGISTRATION AS NS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = NS.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                        AND AB.BATCH_YEAR = CL.CLASS_YEAR
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SM ON SM.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                        AND SM.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SM.BATCH = AB.BATCH_ID
                                        AND SM.IS_ACTIVE = 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NS.COURSE_ID
                                        AND CR.SEMESTER_ID = SM.SEMESTER
                                WHERE
                                    NS.STUDENT_ID = ?STUDENT_ID
                                        AND NS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NS.IS_DELETED != 1;";
                        break;
                    }
                case DashboardSQLCommands.FetchRegisteredCertificateCourse:
                    {
                        query = @"SELECT 
                                    COURSE_TITLE
                                FROM
                                    CERTIFICATE_STUDENT_REGISTRATION AS NS
                                        INNER JOIN
                                    CERTIFICATE_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NS.COURSE_ID
                                        INNER JOIN
                                    CERTIFICATE_COURSE_SETTING AS ST ON ST.SETTING_ID = CR.SETTING_ID
                                        AND ST.IS_ACTIVE = 1
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND NS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NS.IS_DELETED != 1;";
                        break;
                    }
                case DashboardSQLCommands.GetTotalNoOfStaff:
                    {
                        query = @"SELECT COUNT(STAFF_ID) AS STAFF_ID FROM STF_PERSONAL_INFO WHERE IS_DELETED!=1 AND IS_LEFT!=1;";
                        break;
                    }
                case DashboardSQLCommands.FetchStaffTodayBirthDayList:
                    {
                        query = @"SELECT 
                                    UPPER(CONCAT(IFNULL(ST.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(ST.LAST_NAME, ''))) AS 'FIRST_NAME',
                                    ST.STAFF_CODE,
                                    UPPER(DP.DEPARTMENT) AS DEPARTMENT,
                                    CAT.STF_CATEGORY AS CATEGORY,
                                    SA.CCELL_NO AS MOBILE,
                                    ST.PHOTO
                                FROM
                                    STF_PERSONAL_INFO AS ST
                                        LEFT JOIN
                                    CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = ST.DEPARTMENT
                                        LEFT JOIN
                                    SUP_STAFF_CATEGORY AS CAT ON CAT.STF_CATEGORY_ID = ST.CATEGORY
                                        LEFT JOIN
                                    STF_ADDRESS AS SA ON SA.STAFFNO = ST.STAFF_ID
                                        AND DP.IS_DELETED != 1
                                WHERE
                                    ST.IS_DELETED != 1 AND ST.IS_LEFT != 1
                                        AND DATE_FORMAT(ST.DATE_OF_BIRTH, '%d-%m') = DATE_FORMAT(CURRENT_DATE(), '%d-%m');";
                        break;
                    }
                case DashboardSQLCommands.FetchStudentTodayBirthDayList:
                    {
                        //    query = @"SELECT 
                        //    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //            ' ',
                        //            IFNULL(SP.LAST_NAME, ''),
                        //            ' - ',
                        //            SP.ROLL_NO) AS 'FIRST_NAME',
                        //    C.CLASS_NAME,
                        //    IFNULL(SP.FR_MOBILE,
                        //            IFNULL(SP.MO_MOBILE, SP.STU_MOBILENO)) AS 'MOBILE',
                        //    SP.PHOTO
                        //FROM
                        //    STU_CLASS AS SC
                        //        INNER JOIN
                        //    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //        AND SP.IS_DELETED != 1
                        //        AND SP.IS_LEFT != 1
                        //        INNER JOIN
                        //    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                        //        AND C.IS_DELETED != 1
                        //WHERE
                        //    SC.ACADEMIC_YEAR = 2018
                        //        AND SC.IS_DELETED != 1
                        //        AND DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m') = DATE_FORMAT(CURRENT_DATE(), '%d-%m');";
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,''))) AS FIRST_NAME,
                                        SP.DATE_OF_BIRTH,
                                        CONCAT(IFNULL(SP.FR_MOBILE,IFNULL(SP.MO_MOBILE,IFNULL(SP.STU_MOBILENO,'')))) AS MOBILE,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SP.PHOTO,
                                        PG.PROGRAMME_GROUP_ID,
                                        SC.CLASS_ID,
                                        UPPER(CONCAT(IFNULL(C.CLASS_NAME,''),'  ',IFNULL(PM.PROGRAMME_MODE,''),' ( SHIFT - ',IFNULL(SS.SHIFT_NAME,''),' ) ')) AS CLASS_NAME,
                                        UPPER(CONCAT(IFNULL(P.PROGRAMME_NAME,''))) AS PROGRAMME_NAME,
                                        DATE_FORMAT(CURDATE(), '%d/%m/%Y') AS TODAY_DATE
                                    FROM
	                                    STU_CLASS AS SC 
		                                    INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
		                                    INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
		                                    INNER JOIN
	                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
		                                    INNER JOIN
	                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID=PG.PROGRAMME_ID
		                                    INNER JOIN
	                                    SUP_SHIFT AS SS ON SS.SHIFT_ID=PG.SHIFT
		                                    INNER JOIN
	                                    SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID=PG.PROGRAMME_MODE
                                    WHERE
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d-%m') = DATE_FORMAT(CURDATE(), '%d-%m')
	                                    AND SP.IS_DELETED!=1
                                        AND SP.IS_LEFT!=1
                                        AND SC.IS_DELETED!=1
                                        AND SC.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case DashboardSQLCommands.FetchHourbasedAbsentees:
                    {
                        query= @"SELECT 
                        COUNT(AT.STUDENT_ID) as STUDENT_ID,
                        SS.SHIFT_NAME AS SHIFT,
                        AT.HOUR_FROM AS HOUR
                    FROM
                        ATT_ABSENTEE_LIST AS AT
                            INNER JOIN
                        CP_CLASSES AS CC ON CC.CLASS_ID = AT.CLASS_ID
                            INNER JOIN
                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CC.SHIFT
                    WHERE
                        ATTENDENCE_DATE = CURRENT_DATE()
                    GROUP BY AT.HOUR_FROM , CC.SHIFT;";
                        break;
                        //query = @"SELECT 
                        //        COUNT(AB.STUDENT_ID) AS STUDENT_ID,
                        //       SHIFT_NAME AS SHIFT,
                        //        TT.HOUR_ID AS HOUR
                        //    FROM
                        //        ATT_ABSENTEE_LIST AS AB
                        //            INNER JOIN
                        //        TT_TIMETABLE AS TT ON TT.HOUR_ID = AB.HOUR_FROM
                        //            AND AB.ATTENDENCE_DATE = CURRENT_DATE()
                        //            AND TT.DAY_ORDER_ID = (SELECT 
                        //                DAY_ORDER
                        //            FROM
                        //                CALENDER
                        //            WHERE
                        //                ACADEMIC_YEAR=?AC_YEAR AND DAY_ORDER_DATE = CURRENT_DATE())
                        //            INNER JOIN
                        //        CP_CLASSES AS CL ON TT.CLASS_ID = CL.CLASS_ID
                        //            INNER JOIN
                        //        sup_shift AS SH ON CL.SHIFT = SH.SHIFT_ID
                        //    WHERE
                        //        AB.ATTENDANCE_TYPE_ID = 2
                        //    GROUP BY CL.SHIFT , TT.HOUR_ID;";
                        //break;
                    }
                case DashboardSQLCommands.FetchCommonAbsentees:
                    {
                        query = @"SELECT 
                                         COUNT(AB.STUDENT_ID) AS STUDENT_ID,SHIFT_NAME  AS SHIFT
                                    FROM
                                        ATT_COMMON_ABSENTEE_LIST AS AB
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = AB.CLASS_ID
                                        INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CL.SHIFT
                                    WHERE
                                        ATTENDENCE_DATE = CURRENT_DATE()
                                            AND ATTENDANCE_TYPE_ID = 2 GROUP BY CL.SHIFT;";
                        break;
                    }
                case DashboardSQLCommands.FetchTimetableForStaff:
                    {
                        query = @"SELECT 
                                        TT.Time_Table_Id,
                                        CR.COURSE_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                '(',
                                                SP.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                        TT.Day_Order_Id,
                                        TT.Hour_Id,
                                        SH.hourType,
                                        TT.Semester_Id,
                                        SH.hour_type_id,
                                        TT.Course_Id,
                                        TT.Staff_Id,
                                        TT.Class_Id,
                                        CLS.CLASS_CODE,
                                        CR.COURSE_TITLE,
                                        CLS.SHIFT
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID=TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
											INNER JOIN
										cp_class_course_staff AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
											AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.STAFF_ID=TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
											INNER JOIN
										SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1 ";
                        break;
                    }
                case DashboardSQLCommands.FetchOnlineTimetableForStaff:
                    {
                        query = @"SELECT 
                                        TT.Time_Table_Id,
                                        CR.COURSE_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                '(',
                                                SP.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                        TT.Day_Order_Id,
                                        TT.Hour_Id,
                                        SH.hourType,
                                        TT.Semester_Id,
                                        SH.hour_type_id,
                                        TT.Course_Id,
                                        TT.Staff_Id,
                                        TT.Class_Id,
                                        CLS.CLASS_CODE,
                                        CR.COURSE_TITLE,
                                        CLS.SHIFT
                                    FROM
                                        TT_ONLINE_TIMETABLE AS TT
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID=TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
											INNER JOIN
										cp_class_course_staff AS CCS ON CCS.CLASS_ID=TT.CLASS_ID
											AND CCS.COURSE_ID=TT.COURSE_ID
                                            AND CCS.STAFF_ID=TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR=TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED!=1
											INNER JOIN
										SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR AND TT.IS_DELETED!=1 ";
                        break;
                    }
                case DashboardSQLCommands.FetchTotalHoursAndDaysByStaffId:
                    {
                        query = @"SELECT 
                                        MAX(TTS.NO_OF_DAYS) AS NO_OF_DAYS,
                                        MAX(TTS.NO_OF_HOURS) AS NO_OF_HOURS
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            INNER JOIN
                                        TT_DEPARTMENT_WISE_TEMPLATE AS TTD ON TTD.DEPARTMENT_ID = CLS.DEPARTMENT
                                            INNER JOIN
                                        TT_TEMPLATE_SETTING AS TTS ON TTS.SETTING_ID = TTD.SETTING_ID
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
                                        TT.STAFF_ID =?STAFF_ID
                                            AND TT.ACADEMIC_YEAR =?AC_YEAR
                                            AND TT.IS_DELETED!=1
                                    GROUP BY TTS.NO_OF_DAYS , TTS.NO_OF_HOURS;";
                        break;
                    }
                case DashboardSQLCommands.FetchAcademicActivityByUserId:
                    {
                        query = @"SELECT 
                                        ACTIVITY_ID,
                                        ENTRY_ID,
                                        ACADEMIC_YEAR,
                                        SA.ACTIVITY_LEVEL,
                                        ACTIVITY_LEVEL_ID,
                                        STAFF_ID,
                                        ACTIVITY_TITLE,
                                        DATE_FROM,
                                        SUBMISSION_DATE,
                                        DESCRIPTION,
                                        IS_FILE_UPLOAD,
                                        SUBMITTED_DATE,
                                        FILE_PATH,
                                        PHOTO_PATH,
                                        STATUS,
                                        AA.IS_ACTIVE
                                    FROM
                                        ACADEMIC_ACTIVITY AS AA
                                            INNER JOIN
                                        SUP_ACTIVITY_LEVEL AS SA ON SA.ACTIVITY_LEVEL_ID=AA.ACTIVITY_LEVEL
                                    WHERE
                                        STAFF_ID IN (?STAFF_ID) AND AA.IS_DELETED != 1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case DashboardSQLCommands.FetchAcademicActivityByUserIdAndActivityLevel:
                    {
                        query = @"SELECT 
                                    ACTIVITY_ID,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    SA.ACTIVITY_LEVEL,
                                    ACTIVITY_LEVEL_ID,
                                    AA.STAFF_ID,
                                    ACTIVITY_TITLE,
                                    DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    SUBMISSION_DATE,
                                    DESCRIPTION,
                                    IS_FILE_UPLOAD,
                                    DATE_FORMAT(SUBMISSION_DATE, '%d/%m/%Y') AS SUBMISSION_DATE,
                                    FILE_PATH,
                                    PHOTO_PATH,
                                    STATUS,
                                    AA.IS_ACTIVE
                                FROM
                                    ACADEMIC_ACTIVITY AS AA
                                        INNER JOIN
                                    SUP_ACTIVITY_LEVEL AS SA ON SA.ACTIVITY_LEVEL_ID = AA.ACTIVITY_LEVEL
                                WHERE
                                   AA.STATUS != 4 
                                        AND AA.ACTIVITY_LEVEL = ?ACTIVITY_LEVEL 
                                        AND FIND_IN_SET(?STAFF_ID,AA.STAFF_ID)
                                        AND AA.IS_DELETED != 1
                                        AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }

                case DashboardSQLCommands.FetchNotificationInfoforStudent:
                    {
                        query = @"SELECT 
                                        N.NOTIFICATION,
                                        N.NOTIFICATION_ID,
                                        DATE_FORMAT(N.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID = SC.STUDENT_ID
                                            AND STU.IS_DELETED != 1
                                            AND STU.IS_LEFT != 1
                                            INNER JOIN
                                        NOTIFICATIONS_?AC_YEAR AS N ON N.USER_ID = STU.STUDENT_ID
                                            AND N.IS_DELETED != 1
                                            AND N.NOTIFICATION_ROLE_TYPE_ID = 1
                                            AND N.NOTIFICATION_LEVEL_ID = 1
                                            AND DATE_FORMAT(N.NOTIFIED_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                                    WHERE
                                        SC.STUDENT_ID =?STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                    GROUP BY N.NOTIFICATION_ID 
                                    UNION SELECT 
                                        N1.NOTIFICATION,
                                        N1.NOTIFICATION_ID,
                                        DATE_FORMAT(N1.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
                                    FROM
                                        STU_CLASS AS SC1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU1 ON STU1.STUDENT_ID = SC1.STUDENT_ID
                                            AND STU1.IS_DELETED != 1
                                            AND STU1.IS_LEFT != 1
                                            INNER JOIN
                                        NOTIFICATIONS_?AC_YEAR AS N1 ON N1.CLASS_ID = SC1.CLASS_ID
                                            AND N1.IS_DELETED != 1
                                            AND N1.NOTIFICATION_ROLE_TYPE_ID = 1
                                            AND N1.NOTIFICATION_LEVEL_ID = 2
                                            AND DATE_FORMAT(N1.NOTIFIED_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                                    WHERE
                                        SC1.STUDENT_ID =?STUDENT_ID
                                            AND SC1.IS_DELETED != 1
                                            AND SC1.ACADEMIC_YEAR =?AC_YEAR
                                    GROUP BY N1.NOTIFICATION_ID 
                                    UNION SELECT 
                                        N2.NOTIFICATION,
                                        N2.NOTIFICATION_ID,
                                        DATE_FORMAT(N2.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
                                    FROM
                                        STU_CLASS AS SC2
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU2 ON STU2.STUDENT_ID = SC2.STUDENT_ID
                                            AND STU2.IS_DELETED != 1
                                            AND STU2.IS_LEFT != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SC2.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED != 1
                                            INNER JOIN
                                         CP_CLASSES AS C ON C.CLASS_ID=SC2.CLASS_ID
                                         AND C.IS_DELETED!=1
                                            INNER JOIN
                                        NOTIFICATIONS_?AC_YEAR AS N2 ON N2.PROGRAMME_GROUP_ID = CPP.PROGRAMME_GROUP_ID
                                            AND N2.PROGRAMME_YEAR=C.CLASS_YEAR
                                            AND N2.IS_DELETED != 1
                                            AND N2.NOTIFICATION_ROLE_TYPE_ID = 1
                                            AND N2.NOTIFICATION_LEVEL_ID = 3
                                            AND DATE_FORMAT(N2.NOTIFIED_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                                    WHERE
                                        SC2.STUDENT_ID =?STUDENT_ID
                                            AND SC2.IS_DELETED != 1
                                            AND SC2.ACADEMIC_YEAR =?AC_YEAR
                                    GROUP BY N2.NOTIFICATION_ID 
                                    UNION SELECT 
                                        N3.NOTIFICATION,
                                        N3.NOTIFICATION_ID,
                                        DATE_FORMAT(N3.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
                                    FROM
                                        STU_CLASS AS SC3
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU3 ON STU3.STUDENT_ID = SC3.STUDENT_ID
                                            AND STU3.IS_DELETED != 1
                                            AND STU3.IS_LEFT != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPP1 ON CPP1.PROGRAMME_GROUP_ID = SC3.PROGRAMME_GROUP_ID
                                            AND CPP1.IS_DELETED != 1
                                            INNER JOIN
                                        NOTIFICATIONS_?AC_YEAR AS N3 ON N3.APP_TYPE_ID = CPP1.APPTYPE_ID
                                            AND N3.IS_DELETED != 1
                                            AND N3.NOTIFICATION_ROLE_TYPE_ID = 1
                                            AND N3.NOTIFICATION_LEVEL_ID = 5
                                            AND DATE_FORMAT(N3.NOTIFIED_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                                    WHERE
                                        SC3.STUDENT_ID =?STUDENT_ID
                                            AND SC3.IS_DELETED != 1
                                            AND SC3.ACADEMIC_YEAR =?AC_YEAR
                                    GROUP BY N3.NOTIFICATION_ID 
                                   
                                    UNION SELECT 
                                        N6.NOTIFICATION,
                                        N6.NOTIFICATION_ID,
                                        DATE_FORMAT(N6.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
                                    FROM
                                        STU_CLASS AS SC6
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU6 ON STU6.STUDENT_ID = SC6.STUDENT_ID
                                            AND STU6.IS_DELETED != 1
                                            AND STU6.IS_LEFT != 1
                                            INNER JOIN
                                        NOTIFICATIONS_?AC_YEAR AS N6 ON N6.NOTIFICATION_ROLE_TYPE_ID != 2
                                            AND N6.IS_DELETED != 1
                                            AND N6.NOTIFICATION_LEVEL_ID = 4
                                            AND DATE_FORMAT(N6.NOTIFIED_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                                    WHERE
                                        SC6.STUDENT_ID =?STUDENT_ID
                                            AND SC6.IS_DELETED != 1
                                            AND SC6.ACADEMIC_YEAR =?AC_YEAR
                                    GROUP BY N6.NOTIFICATION_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchNotificationInfoforStaff:
                    {
                        query = @"SELECT 
    
                                            N.NOTIFICATION,
                                            N.NOTIFICATION_ID,
                                             DATE_FORMAT(N.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
   
                                        FROM
	                                           STF_PERSONAL_INFO AS STF
                                                INNER JOIN
                                            NOTIFICATIONS_?AC_YEAR AS N ON N.USER_ID = STF.STAFF_ID
                                                AND N.IS_DELETED != 1
                                                AND N.NOTIFICATION_ROLE_TYPE_ID = 2
                                                AND N.NOTIFICATION_LEVEL_ID = 1
		                                        AND DATE_FORMAT(N.NOTIFIED_DATE,'%Y-%m-%d')>=DATE_FORMAT(CURDATE(),'%Y-%m-%d')
        
                                        WHERE
                                            STF.STAFF_ID =?STAFF_ID
                                                AND STF.IS_DELETED != 1
                                                AND STF.IS_LEFT!=1 GROUP BY N.NOTIFICATION_ID
        
                                                UNION
                                                SELECT 
    
                                            N1.NOTIFICATION,
                                            N1.NOTIFICATION_ID,
                                             DATE_FORMAT(N1.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
   
                                        FROM
                                            STF_PERSONAL_INFO AS STF1
                                                INNER JOIN
        
                                             NOTIFICATIONS_?AC_YEAR AS N1 ON N1.DEPARTMENT = STF1.DEPARTMENT
                                                AND N1.IS_DELETED != 1
                                                AND N1.NOTIFICATION_ROLE_TYPE_ID = 2
                                                AND N1.NOTIFICATION_LEVEL_ID = 3
		                                        AND DATE_FORMAT(N1.NOTIFIED_DATE,'%Y-%m-%d')>=DATE_FORMAT(CURDATE(),'%Y-%m-%d')
	
                                        WHERE
                                            STF1.STAFF_ID =?STAFF_ID
                                                AND STF1.IS_DELETED != 1
                                                AND STF1.IS_LEFT!=1  GROUP BY N1.NOTIFICATION_ID
                                                UNION

                                          SELECT 
    
                                            N2.NOTIFICATION,
                                            N2.NOTIFICATION_ID,
                                             DATE_FORMAT(N2.NOTIFIED_DATE, '%Y-%m-%d') AS NOTIFIED_DATE
   
                                        FROM
                                            STF_PERSONAL_INFO AS STF2
                                                INNER JOIN

                                            NOTIFICATIONS_?AC_YEAR AS N2 ON  N2.NOTIFICATION_LEVEL_ID = 4
                                                AND  N2.IS_DELETED != 1
                                                AND N2.NOTIFICATION_ROLE_TYPE_ID != 1
		                                        AND DATE_FORMAT(N2.NOTIFIED_DATE,'%Y-%m-%d')>=DATE_FORMAT(CURDATE(),'%Y-%m-%d')
          
      
                                        WHERE
                                           STF2.STAFF_ID =?STAFF_ID
                                                AND STF2.IS_DELETED != 1
                                                AND STF2.IS_LEFT!=1  GROUP BY N2.NOTIFICATION_ID;";
                        break;
                    }

                case DashboardSQLCommands.FetchNoOfRemainingAssignments:
                    {
                        query = @"SELECT 
                                COUNT(A.STAFF_ID) AS NO_OF_ASSIGNMENTS
                            FROM
                                ASSIGNMENT_?AC_YEAR AS A
                                    INNER JOIN
                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = A.STAFF_ID
                                    AND STF.IS_DELETED != 1
                                    AND STF.IS_LEFT != 1
                            WHERE
                                A.STAFF_ID =?STAFF_ID
                                    AND A.ACADEMIC_YEAR =?AC_YEAR
                                    AND A.IS_DELETED != 1
                                    AND DATE_FORMAT(A.SUBMISSION_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d');";
                        break;
                    }
                case DashboardSQLCommands.FetchUgStudentMarks:
                    {
                        query = @"SELECT 
                            SP.STUDENT_ID,
                            SP.ROLL_NO,
                            SP.REGISTER_NO,
                            CONCAT(IFNULL(FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(LAST_NAME, '')) AS NAME,
                            CR.PART,
                            P.PART AS PART_NAME,
                            CR.COURSE_CODE,
                            CONCAT(UGC.UGC_COURSE_TYPE,
                                    ': ',
                                    CR.COURSE_TITLE) COURSE_TITLE,
                            UGC.COURSE_ORDER,
                            SEM.SEMESTER,
                            SS.SEMESTER_NAME,
                            SEM.INTERNAL_MARK,
                            SEM.EXTERNAL_MARK,
                            SEM.COURSE_ID,
                            SEM.TOTAL,
                            IFNULL(CR.TOTAL, 100) AS MAXIMUM,
                            CR.CREDITS AS CREDIT,
                            SEM.GRADE_POINT,
                            SEM.M_PASS,
                            SEM.RESULT,
                            ES.M_EXAM,
                            DATE_FORMAT(ES.START_DATE, '%d/%m/%Y') AS START_DATE,
                            CR.UGC_COURSE_TYPE,
                            SEM.EXAM_SETTING_ID
                        FROM
                            STU_CLASS AS ST
                                INNER JOIN
                            STU_PERSONAL_INFO AS SP ON ST.STUDENT_ID = SP.STUDENT_ID
                                AND (SP.IS_DELETED != 1 AND IS_LEFT != 1)
                                INNER JOIN
                            CP_CLASSES AS CC ON CC.CLASS_ID = ST.CLASS_ID
                                INNER JOIN
                            SEMESTER_MARKS_?AC_YEAR AS SEM ON SEM.STUDENT_ID = ST.STUDENT_ID
                                INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SEM.COURSE_ID 
                                LEFT JOIN
                            CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                LEFT JOIN
                            CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                LEFT JOIN
                            SUP_PART AS P ON P.PART_ID = CR.PART
                                LEFT JOIN
                            SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                INNER JOIN
                            EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = SEM.EXAM_SETTING_ID
        
                        WHERE 
                            ST.ACADEMIC_YEAR =?ACADEMIC_YEAR AND ST.STUDENT_ID=?STUDENT_ID AND SEM.RESULT='P'
                        ORDER BY SP.REGISTER_NO;";
                        break;
                    }
                case DashboardSQLCommands.FetchStudentMarks:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                CR.COURSE_CODE,
                                CONCAT(UGC.UGC_COURSE_TYPE,
                                        ': ',
                                        CR.COURSE_TITLE) COURSE_TITLE,
                                UGC.COURSE_ORDER,
                                SEM.SEMESTER,
                                SS.SEMESTER_NAME,
                                SEM.INTERNAL_MARK,
                                SEM.EXTERNAL_MARK,
                                SEM.COURSE_ID,
                                SEM.TOTAL,
                                IFNULL(CR.TOTAL, 100) AS MAXIMUM,
                                CR.CREDITS AS CREDIT,
                                SEM.GRADE_POINT,
                                SEM.M_PASS,
                                SEM.RESULT,
                                ES.M_EXAM,
                                DATE_FORMAT(ES.START_DATE, '%d/%m/%Y') AS START_DATE,
                                CR.UGC_COURSE_TYPE,
                                SEM.EXAM_SETTING_ID
                            FROM
                                STU_CLASS AS ST
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON ST.STUDENT_ID = SP.STUDENT_ID
                                    AND (SP.IS_DELETED != 1 AND IS_LEFT != 1)
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = ST.CLASS_ID
                                    INNER JOIN
                                SEMESTER_MARKS_?AC_YEAR AS SEM ON SEM.STUDENT_ID = ST.STUDENT_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SEM.COURSE_ID
                                    LEFT JOIN
                                CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    LEFT JOIN
                                CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                                    LEFT JOIN
                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                    INNER JOIN
                                EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = SEM.EXAM_SETTING_ID
        
                            WHERE
                                ST.ACADEMIC_YEAR =?ACADEMIC_YEAR AND ST.STUDENT_ID=?STUDENT_ID AND SEM.RESULT='P'
                            ORDER BY SP.REGISTER_NO;";
                        break;
                    }

                case DashboardSQLCommands.FetchDateWiseFeeCollection:
                    {
                        query = @"SELECT 
                                SUM(COLLECTED) AS COLLECTED,
                                DATE_FORMAT(PAYMENT_DATE, '%b-%d') AS PAYMENT_DATE
                            FROM
                                FEE_TRANSACTION
                            WHERE
                                FREQUENCY IN(?FREQUENCY_ID)
                                    AND PAYMENT_DATE BETWEEN DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-%d'),
                                    INTERVAL 30 DAY) AND DATE_FORMAT(NOW(), '%Y-%m-%d')
                            GROUP BY PAYMENT_DATE;";
                        break;
                    }
                case DashboardSQLCommands.FetchAbsenteesList:
                    {
                        query = @"SELECT 
                            ATT.STUDENT_ID, SP.FIRST_NAME, CC.CLASS_NAME, SP.ROLL_NO
                        FROM
                            ATT_ABSENTEE_LIST AS ATT
                                INNER JOIN
                            STU_CLASS AS SC ON SC.STUDENT_ID = ATT.STUDENT_ID
                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                INNER JOIN
                            stu_personal_info AS SP ON ATT.STUDENT_ID = SP.STUDENT_ID
                                INNER JOIN
                            cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID
                        WHERE
                            ATTENDENCE_DATE = DATE_FORMAT(CURDATE(), '%Y-%m-%d');";
                        break;
                    }
                #region Admin And Principal DashBoard
                case DashboardSQLCommands.FetchProgrammeTypeWiseStudentCount:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS STUDENT_COUNT, SAT.APPLICATION_TYPE
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND PG.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_APPLICATION_TYPE AS SAT ON SAT.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SAT.APPLICATION_TYPE_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchClassYearStudentCount:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS STUDENT_COUNT,
                                        CY.CLASS_YEAR
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
	                                    CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
		                                    INNER JOIN
	                                    SUP_CLASS_YEAR AS CY ON CY.CLASS_YEAR_ID=C.CLASS_YEAR
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1 GROUP BY CY.CLASS_YEAR_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchprogrammeWiseStudentCount:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS STUDENT_COUNT,
                                        UPPER(CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                        '  ',
                                                        IFNULL(PM.PROGRAMME_MODE, ''),
                                                        ' ( SHIFT - ',
                                                        IFNULL(SS.SHIFT_NAME, ''),
                                                        ' ) ')) AS PROGRAMME_NAME,
                                        CY.CLASS_YEAR
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        SUP_CLASS_YEAR AS CY ON CY.CLASS_YEAR_ID = C.CLASS_YEAR
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY PG.PROGRAMME_GROUP_ID ;";
                        break;
                    }
                case DashboardSQLCommands.FetchProgrammeAndClassWiseStudentCount:
                    {
                        query = @"SELECT 
                                    COUNT(SC.STUDENT_ID) AS STUDENT_COUNT,
                                    UPPER(CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                    '  ',
                                                    IFNULL(PM.PROGRAMME_MODE, ''),
                                                    ' ( SHIFT - ',
                                                    IFNULL(SS.SHIFT_NAME, ''),
                                                    ' ) ')) AS PROGRAMME_NAME,
                                    CY.CLASS_YEAR
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    SUP_CLASS_YEAR AS CY ON CY.CLASS_YEAR_ID = C.CLASS_YEAR
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                GROUP BY PG.PROGRAMME_GROUP_ID , CY.CLASS_YEAR_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchDepartmentWiseStaffCount:
                    {
                        query = @"SELECT 
                                    COUNT(SP.STAFF_ID) AS STAFF_COUNT,
                                    IFNULL(D.DEPARTMENT, 'OFFICE') AS DEPARTMENT
                                FROM
                                    STF_PERSONAL_INFO AS SP
                                        LEFT JOIN
                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SP.DEPARTMENT
                                WHERE
                                    SP.IS_DELETED != 1 AND SP.IS_LEFT != 1
                                GROUP BY D.DEPARTMENT;";
                        break;
                    }
                case DashboardSQLCommands.FetchTotalAbsentiesCountByHourWiseForReularPaper:
                    {
                        query = @"SELECT 
                                            COUNT(STUDENT_ID) AS TOTAL_ABSENT,
                                        --    COUNT(COURSE_ID) AS 'TOTAL_HRS',
                                            HOUR_ID
                                         --   IF(ATTENDANCE_TYPE_ID = '2', 1, 0) AS ABSENT
                                           
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
												STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=SC.STUDENT_ID
                                                AND SPI.IS_DELETED!=1
                                                AND SPI.IS_LEFT!=1
                                                    INNER JOIN 
		                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                                AND C.IS_DELETED!=1
			                                        INNER JOIN 
		                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
			                                        AND AB.ACADEMIC_YEAR_ID =(SELECT 
				                                        ACADEMIC_YEAR_ID
			                                        FROM
				                                        ACADEMIC_YEAR
			                                        WHERE
				                                        ACTIVE_YEAR = 1)
			                                        INNER JOIN 
		                                        CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
			                                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                    AND CCP.IS_DELETED!=1
			                                        INNER JOIN 
		                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                                    AND CR.IS_DELETED!=1
			                                        INNER JOIN 
		                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
			                                        AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        AND SEM.BATCH = AB.BATCH_ID
			                                        AND SEM.SEMESTER = CR.SEMESTER_ID
			                                        AND SEM.IS_DELETED != 1
													AND SEM.IS_ACTIVE = 1
			                                        INNER JOIN 
		                                        TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
			                                        AND TT.CLASS_ID = SC.CLASS_ID
			                                        AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
			                                        AND TT.IS_DELETED != 1
			                                        INNER JOIN
		                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
			                                        AND CAL.DAY_ORDER_DATE=DATE_FORMAT('2019-09-17', '%Y-%m-%d')
                                                    AND CAL.IS_DELETED!=1
			                                        INNER JOIN 
		                                        ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
			                                        AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
			                                        AND ATT.HOUR_FROM = TT.HOUR_ID
                                                    AND ATT.S_CLASS_ID=SC.CLASS_ID
                                                    AND ATT.STAFF_ID=TT.STAFF_ID
			                                        AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                                    AND ATT.ATTENDANCE_TYPE_ID=2
                                            WHERE
                                               
                                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                                     AND CR.IS_COMPULSORY = 1
                                                    AND CCP.IS_DELETED != 1
                                                    AND SC.IS_DELETED != 1
                                            GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                        GROUP BY HOUR_ID ORDER BY HOUR_ID;";
                        break;
                    }
                case DashboardSQLCommands.FetchTotalAbsentiesCountByHourWiseForOptionalPaper:
                    {
                        query = @"SELECT 
                                        COUNT(STUDENT_ID) AS TOTAL_ABSENT, HOUR_ID
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
                                        INNER JOIN STU_CLASS AS SC ON SC.STUDENT_ID = CCP.STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                        INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                        INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            AND CC.IS_DELETED != 1
                                        INNER JOIN ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = CC.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                                ACADEMIC_YEAR_ID
                                            FROM
                                                ACADEMIC_YEAR
                                            WHERE
                                                ACTIVE_YEAR = 1 AND IS_DELETED != 1)
                                        INNER JOIN ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND S.BATCH = AB.BATCH_ID
                                            AND S.IS_ACTIVE = 1
                                        INNER JOIN TT_TIMETABLE AS TT ON TT.COURSE_ID = CCP.COURSE_ID
                                            AND TT.CLASS_ID = CCP.S_CLASS_ID
                                            AND TT.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                        INNER JOIN CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE = DATE_FORMAT('2019-09-17', '%Y-%m-%d')
                                            AND CAL.IS_DELETED != 1
                                        INNER JOIN ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = CCP.STUDENT_ID
                                            AND ATT.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND ATT.HOUR_FROM = TT.HOUR_ID
                                            AND ATT.S_CLASS_ID = CCP.S_CLASS_ID
                                            AND ATT.ATTENDENCE_DATE = CAL.DAY_ORDER_DATE
                                            AND ATT.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                            AND ATT.ATTENDANCE_TYPE_ID = 2
                                        INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                        INNER JOIN SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                        WHERE
                                            CCP.ACADEMIC_YEAR =?AC_YEAR
                                                AND CCP.IS_DELETED != 1
                                                AND CCP.SEMESTER = S.SEMESTER
                                        GROUP BY CAL.DAY_ORDER_DATE , CCP.COURSE_ID , TT.DAY_ORDER_ID , TT.HOUR_ID) AS T
                                    GROUP BY HOUR_ID;";
                        break;
                    }
                #endregion

                #region COE DashBoard

                #endregion

                #region Secrectory DashBoard
                case DashboardSQLCommands.FetchSubstituteHoursByCurDate:
                    {
                        query = @"SELECT 
                                        DAY_ORDER_ID,
                                        HOUR_ID,
                                        'Substitute Hour' AS HOUR_TYPE,
                                        concat('( ',DATE_FORMAT(ATT.DAY_ORDER_DATE,'%d/%m/%Y'),' )') AS DAY_ORDER_DATE,
                                        CC.SHIFT,
                                        CC.CLASS_CODE,
                                        STAFF_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS STAFF_NAME
                                    FROM
                                        ATT_SUBSTITUTE AS ATT
		                                    INNER JOIN
	                                    CP_CLASSES AS CC ON CC.CLASS_ID=ATT.CLASS_ID
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=ATT.COURSE_ID
		                                    INNER JOIN
	                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=ATT.INCHARGE_ID
                                    WHERE
                                        ATT.DAY_ORDER_DATE = CURDATE()
                                            AND ATT.IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region Parent
                case DashboardSQLCommands.FetchStudentIdbyParentId:
                    {
                        query = @"SELECT 
                                      U.USER_ID,
                                      SP.PARENT_ID,
                                      U.NAME,
                                      GROUP_CONCAT(SC.STUDENT_ID) AS STUDENT_ID
                                  FROM
                                      USER_ACCOUNT AS U
                                          INNER JOIN
                                      STU_PARENT_INFO AS SP ON SP.PARENT_ID = U.USER_ID
                                          INNER JOIN
                                      STU_PARENT_STUDENT_MAPPING AS PS ON PS.PARENT_ID = SP.PARENT_ID
										  INNER JOIN
                                          STU_CLASS AS SC ON SC.STUDENT_ID=PS.STUDENT_ID
                                          AND SC.ACADEMIC_YEAR=2019
                                  WHERE
                                      U.IS_DELETED != 1 AND U.USER_TYPE = 17
                                          AND U.USER_ID = ?PARENT_ID
                                   GROUP BY PS.PARENT_ID
                                  ;";
                        break;
                    }
                #endregion

                case DashboardSQLCommands.NoOfHostelStudent:
                    {
                        query = @"SELECT 
                            COUNT(HS.STUDENT_ID) AS STUDENT_ID
                        FROM
                            HOSTEL_STUDENT_INFO AS HS
                        WHERE
                            HS.ACADEMIC_YEAR = ?AC_YEAR
                                AND HS.IS_DELETED != 1
                                AND IS_LEFT != 1;";
                        break;
                    }
                case DashboardSQLCommands.FetchCollegedays:
                    {
                        query = @"SELECT 
                                DATEDIFF(DATE_FORMAT(DATE_TO, '%Y/%m/%d'),
                                        DATE_FORMAT(DATE_FROM, '%Y/%m/%d')) AS TOTAL_REGISTER,
                                DATEDIFF(DATE_FORMAT(DATE_TO, '%Y/%m/%d'),
                                        DATE_FORMAT(CURDATE(), '%Y/%m/%d')) AS PASSED
                            FROM
                                ACADEMIC_YEAR
                            WHERE
                                ACTIVE_YEAR = 1;";
                        break;
                    }
                case DashboardSQLCommands.FetchStatusticalFeeCollection:
                    {
                        query = @"SELECT 
                                SUM(FT.COLLECTED) AS COLLECTED,
                                CONCAT(SFT.frequency_type) AS FREQUENCY_NAME
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFM ON FFM.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                    INNER JOIN
                                SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFM.FEE_MODE
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFM.FREQUENCY_ID
                            WHERE
                                FT.FREQUENCY IN (?FREQUENCY_ID)
                                    AND FFM.ACADEMIC_YEAR =?AC_YEAR
                            GROUP BY FT.FREQUENCY;";
                        break;
                    }
                case DashboardSQLCommands.FetchSettlementWiseFeeCollection:
                    {
                        query = @"SELECT 
                                    SUM(FC.PAID_AMOUNT) AS COLLECTED,
                                    FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%Y/%m/%d') AS PAYMENT_DATE
                                FROM
                                    FEE_TRANSACTION AS FT
                                        INNER JOIN
                                    fee_collection AS FC ON FT.TRANSACTION_ID = FC.TRANSACTION_ID
                                        AND FC.SETTLEMENT_DATE IS NOT NULL
                                WHERE
                                    FT.FREQUENCY IN (?FREQUENCY_ID)
                                        AND FT.PAYMENT_DATE BETWEEN DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-%d'),
                                        INTERVAL 30 DAY) AND DATE_FORMAT(NOW(), '%Y-%m-%d')
                                GROUP BY FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%Y/%m/%d');";
                        break;
                    }
                case DashboardSQLCommands.FetchAppdownloadlist:
                    {
                        query = @"SELECT 
                                USER_ID,ROLE_ID
                            FROM
                                OTHERS_AAROGYA_SETUAPP_DOWNLOAD
                            WHERE
                                USER_ID = ?USER_ID AND ROLE_ID = ?ROLE_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case DashboardSQLCommands.SaveAppdownload:
                    {
                        query = @"INSERT INTO 
                                OTHERS_AAROGYA_SETUAPP_DOWNLOAD(USER_ID, ROLE_ID, STAUS, ACADEMIC_YEAR)
                                 VALUES(?USER_ID, ?ROLE_ID, ?STAUS, ?ACADEMIC_YEAR);";
                        break;
                    }
                case DashboardSQLCommands.FetchNoticeBoardInfo:
                    {
                        query = @"SELECT 
                                        NOTICEBOARD_ID,
                                        NOTICEBOARD,
                                        FILE_NAME,
                                        ROLE_ID,
                                        (SELECT GROUP_CONCAT(ROLE_NAME) FROM SUP_ROLE WHERE FIND_IN_SET(ROLE_ID, DB_NOTICEBOARD.ROLE_ID)) AS ROLE_NAME,
                                        APPTYPE,
                                        (SELECT GROUP_CONCAT(APPLICATION_TYPE) FROM SUP_APPLICATION_TYPE WHERE FIND_IN_SET(APPLICATION_TYPE_ID, DB_NOTICEBOARD.APPTYPE)) AS APPTYPE_NAME,
                                        CLASS_YEAR,
                                        (SELECT GROUP_CONCAT(CLASS_YEAR) FROM SUP_CLASS_YEAR WHERE FIND_IN_SET(CLASS_YEAR_ID, DB_NOTICEBOARD.CLASS_YEAR)) AS CLASS_YEAR_NAME,
                                        LINK_UPLOAD,
                                        IS_ACTIVE,
                                        IS_DELETED,
                                        DESCRIPTION,
                                        FILE_TYPE,
                                        DUE_DATE,
                                        SHIFT,
                                        (SELECT GROUP_CONCAT(SHIFT_NAME) FROM SUP_SHIFT WHERE FIND_IN_SET(SHIFT_ID, DB_NOTICEBOARD.SHIFT)) AS SHIFT_NAME,
                                        IS_COMMENT_ENABLE,
                                        IS_FILE_UPLOAD_ENABLE,
                                        (SELECT COUNT(*) FROM db_noticeboard_view_log WHERE NOTICEBOARD_ID = DB_NOTICEBOARD.NOTICEBOARD_ID) AS VIEW_COUNT,
                                        (SELECT COUNT(*) FROM db_noticeboard_log WHERE NOTICEBOARD_ID = DB_NOTICEBOARD.NOTICEBOARD_ID) AS RESPONSE_COUNT
                                    FROM
                                        DB_NOTICEBOARD 
                                    WHERE 
                                         IS_DELETED != 1
                                        AND (FIND_IN_SET(?CLASS_YEAR, CLASS_YEAR) > 0 OR ?CLASS_YEAR IS NULL OR ?CLASS_YEAR = '')
                                        AND (FIND_IN_SET(?APPTYPE, APPTYPE) > 0 OR ?APPTYPE IS NULL OR ?APPTYPE = '')
                                        AND (FIND_IN_SET(?SHIFT, SHIFT) > 0 OR ?SHIFT IS NULL OR ?SHIFT = '');";
                        break;
                    }

                case DashboardSQLCommands.FetchNoticeBoardInfoById:
                    {
                        query = @"SELECT 
                                        NOTICEBOARD_ID,
                                        NOTICEBOARD,
                                        FILE_NAME,
                                        ROLE_ID,
                                        APPTYPE,
                                        CLASS_YEAR,
                                        LINK_UPLOAD,
                                        IS_ACTIVE,
                                        IS_DELETED,
                                        DESCRIPTION,
                                        FILE_TYPE,
                                        DATE_FORMAT(DUE_DATE, '%d/%m/%Y') AS DUE_DATE,
                                        SHIFT,
                                        IS_COMMENT_ENABLE,
                                        IS_FILE_UPLOAD_ENABLE
                                    FROM
                                        DB_NOTICEBOARD WHERE IS_ACTIVE =1 AND IS_DELETED !=1 AND NOTICEBOARD_ID=?NOTICEBOARD_ID;";
                        break;
                    }

                case DashboardSQLCommands.UpdateNoticeBoardInfoById:
                    {
                        query = @"UPDATE DB_NOTICEBOARD
                                  SET
                                  NOTICEBOARD = ?NOTICEBOARD,
                                  FILE_NAME = ?FILE_NAME,
                                  ROLE_ID = ?ROLE_ID,
                                  APPTYPE = ?APPTYPE,
                                  CLASS_YEAR = ?CLASS_YEAR,
                                  LINK_UPLOAD = ?LINK_UPLOAD,
                                  IS_ACTIVE = ?IS_ACTIVE,
                                  DESCRIPTION = ?DESCRIPTION,
                                  FILE_TYPE = ?FILE_TYPE,
                                  DUE_DATE = ?DUE_DATE,
                                  SHIFT = ?SHIFT,
                                  IS_COMMENT_ENABLE = ?IS_COMMENT_ENABLE,
                                  IS_FILE_UPLOAD_ENABLE = ?IS_FILE_UPLOAD_ENABLE
                                  ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                  SEMESTER_TYPE = ?SEMESTER_TYPE
                                  WHERE NOTICEBOARD_ID = ?NOTICEBOARD_ID;";
                        break;
                    }

                case DashboardSQLCommands.SaveNoticeBoardInfoById:
                    {
                        query = @"INSERT INTO DB_NOTICEBOARD
                                             (
                                             NOTICEBOARD,
                                             FILE_NAME,
                                             ROLE_ID,
                                             APPTYPE,
                                             CLASS_YEAR,
                                             LINK_UPLOAD,
                                             IS_ACTIVE,
                                             DESCRIPTION,
                                             FILE_TYPE,
                                             DUE_DATE,
                                             SHIFT,
                                             IS_COMMENT_ENABLE,
                                             ACADEMIC_YEAR,
                                             SEMESTER_TYPE,
                                             IS_FILE_UPLOAD_ENABLE)
                                             VALUES
                                             (
                                             ?NOTICEBOARD,
                                             ?FILE_NAME,
                                             ?ROLE_ID,
                                             ?APPTYPE,
                                             ?CLASS_YEAR,
                                             ?LINK_UPLOAD,
                                             ?IS_ACTIVE,
                                             ?DESCRIPTION,
                                             ?FILE_TYPE,
                                             ?DUE_DATE,
                                             ?SHIFT,
                                             ?IS_COMMENT_ENABLE,
                                             ?ACADEMIC_YEAR,
                                             ?SEMESTER_TYPE,
                                             ?IS_FILE_UPLOAD_ENABLE);";
                        break;
                    }

                case DashboardSQLCommands.DeleteNoticeBoardInfoById:
                    {
                        query = @"UPDATE DB_NOTICEBOARD
                                  SET
                                  IS_DELETED = ?IS_DELETED
                                  WHERE NOTICEBOARD_ID = ?NOTICEBOARD_ID;";
                        break;
                    }

                case DashboardSQLCommands.SaveNoticeBoardLog:
                    {
                        query = @"INSERT INTO db_noticeboard_log
                                    (NOTICEBOARD_ID, USER_ID, USER_ROLE, ACADEMIC_YEAR, SEMESTER_TYPE, FILE_PATH, COMMENTS, IS_ACCEPTED, IS_CANCELED, CREATED_BY)
                                    VALUES (?NOTICEBOARD_ID, ?USER_ID, ?USER_ROLE, ?ACADEMIC_YEAR, ?SEMESTER_TYPE, ?FILE_PATH, ?COMMENTS, ?IS_ACCEPTED, ?IS_CANCELED, ?CREATED_BY);";
                        break;
                    }

                //case DashboardSQLCommands.SaveNoticeBoardInfoById:
                //    {
                //        query = @"INSERT INTO DB_NOTICEBOARD (
                //                    NOTICEBOARD, 
                //                    FILE_NAME, 
                //                    ROLE_ID, 
                //                    APPTYPE, 
                //                    CLASS_YEAR, 
                //                    LINK_UPLOAD, 
                //                    IS_ACTIVE, 
                //                    IS_DELETED, 
                //                    DESCRIPTION, 
                //                    FILE_TYPE, 
                //                    DUE_DATE, 
                //                    SHIFT, 
                //                    IS_COMMENT_ENABLE, 
                //                    IS_FILE_UPLOAD_ENABLE,
                //                    ACADEMIC_YEAR,
                //                    SEMESTER_TYPE,
                //                    CREATED_DATE
                //                ) VALUES (
                //                    ?NOTICEBOARD, 
                //                    ?FILE_NAME, 
                //                    ?ROLE_ID, 
                //                    ?APPTYPE, 
                //                    ?CLASS_YEAR, 
                //                    ?LINK_UPLOAD, 
                //                    ?IS_ACTIVE, 
                //                    0, 
                //                    ?DESCRIPTION, 
                //                    ?FILE_TYPE, 
                //                    ?DUE_DATE, 
                //                    ?SHIFT, 
                //                    ?IS_COMMENT_ENABLE, 
                //                    ?IS_FILE_UPLOAD_ENABLE,
                //                    ?ACADEMIC_YEAR,
                //                    ?SEMESTER_TYPE,
                //                    NOW()
                //                );";
                //        break;
                //    }

                //case DashboardSQLCommands.UpdateNoticeBoardInfoById:
                //    {
                //        query = @"UPDATE DB_NOTICEBOARD SET 
                //                    NOTICEBOARD = ?NOTICEBOARD,
                //                    FILE_NAME = ?FILE_NAME,
                //                    ROLE_ID = ?ROLE_ID,
                //                    APPTYPE = ?APPTYPE,
                //                    CLASS_YEAR = ?CLASS_YEAR,
                //                    LINK_UPLOAD = ?LINK_UPLOAD,
                //                    IS_ACTIVE = ?IS_ACTIVE,
                //                    DESCRIPTION = ?DESCRIPTION,
                //                    FILE_TYPE = ?FILE_TYPE,
                //                    DUE_DATE = ?DUE_DATE,
                //                    SHIFT = ?SHIFT,
                //                    IS_COMMENT_ENABLE = ?IS_COMMENT_ENABLE,
                //                    IS_FILE_UPLOAD_ENABLE = ?IS_FILE_UPLOAD_ENABLE,
                //                    ACADEMIC_YEAR = ?ACADEMIC_YEAR,
                //                    SEMESTER_TYPE = ?SEMESTER_TYPE
                //                WHERE 
                //                    NOTICEBOARD_ID = ?NOTICEBOARD_ID;";
                //        break;
                //    }

                case DashboardSQLCommands.FetchNoticeBoardLogByNoticeId:
                    {
                        query = @"SELECT 
                                        L.NOTICEBOARD_LOG_ID,
                                        L.NOTICEBOARD_ID,
                                        L.USER_ID,
                                        L.USER_ROLE,
                                        L.COMMENTS,
                                        L.FILE_PATH,
                                        L.IS_ACCEPTED,
                                        L.IS_CANCELED,
                                        (SP.FIRST_NAME) AS FIRST_NAME,
                                        ROLE_NAME
                                    FROM
                                        DB_NOTICEBOARD_LOG L
                                            INNER JOIN
                                        STU_PERSONAL_INFO SP ON SP.STUDENT_ID = L.USER_ID
                                            INNER JOIN
                                        SUP_ROLE AS SR ON SR.ROLE_ID = USER_ROLE
                                            AND L.USER_ROLE = ?ROLE_ID
                                    WHERE
                                        (L.NOTICEBOARD_ID = ?NOTICEBOARD_ID OR ?NOTICEBOARD_ID IS NULL OR ?NOTICEBOARD_ID = '')
                                            AND L.IS_DELETED = 0 
                                     union SELECT 
                                        L.NOTICEBOARD_LOG_ID,
                                        L.NOTICEBOARD_ID,
                                        L.USER_ID,
                                        L.USER_ROLE,
                                        L.COMMENTS,
                                        L.FILE_PATH,
                                        L.IS_ACCEPTED,
                                        L.IS_CANCELED,
                                        SF.FIRST_NAME AS FIRST_NAME,
                                        ROLE_NAME
                                    FROM
                                        DB_NOTICEBOARD_LOG L
                                            INNER JOIN
                                        STF_PERSONAL_INFO SF ON SF.STAFF_ID = L.USER_ID
                                            AND L.USER_ROLE !=?ROLE_ID
                                            INNER JOIN
                                        SUP_ROLE AS SR ON SR.ROLE_ID = USER_ROLE
                                    WHERE
                                        (L.NOTICEBOARD_ID = ?NOTICEBOARD_ID OR ?NOTICEBOARD_ID IS NULL OR ?NOTICEBOARD_ID = '')
                                            AND L.IS_DELETED = 0;";
                        break;
                    }

                case DashboardSQLCommands.SaveNoticeViewTracking:
                    {
                        query = @"INSERT INTO db_noticeboard_view_log (NOTICEBOARD_ID, USER_ID, VIEW_DATE, ACADEMIC_YEAR)
                                  VALUES (?NOTICEBOARD_ID, ?USER_ID, NOW(), ?ACADEMIC_YEAR);";
                        break;
                    }
                case DashboardSQLCommands.FetchCollegeTopperByExamSetting:
                    {
                        query = @"SELECT 
                                      SC.STUDENT_ID,
                                      CC.CLASS_NAME,
                                      CP.PROGRAMME_GROUP_ID,
                                      CC.CLASS_ID,
                                      CONCAT(P.PROGRAMME_NAME,
                                              ' - ',
                                              PM.PROGRAMME_MODE,
                                              '(',
                                              S.SHIFT_NAME,
                                              ')') AS PROGRAMME_NAME,
                                      REGISTER_NO,
                                      FIRST_NAME,
                                      ROLL_NO,
                                      ADMISSION_NO,
                                      PHOTO,
                                      SUM(TOTAL) AS TOTAL,
                                      COUNT(SC.STUDENT_ID) AS UDF1
                                  FROM
                                      EXAM_SETTING AS EE
                                          INNER JOIN
                                      SEMESTER_MARKS_?AC_YEAR AS SS ON SS.EXAM_SETTING_ID = EE.EXAM_SETTING_ID
                                          INNER JOIN
                                      STU_CLASS AS SC ON SC.STUDENT_ID = SS.STUDENT_ID
                                          AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                          INNER JOIN
                                      CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                          INNER JOIN
                                      CP_PROGRAMME AS P ON P.PROGRAMME_ID = CP.PROGRAMME_ID
                                          INNER JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                          INNER JOIN
                                      SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = CP.PROGRAMME_MODE
                                          INNER JOIN
                                      SUP_SHIFT AS S ON S.SHIFT_ID = CP.SHIFT
                                  WHERE
                                      EE.IS_PUBLISHED = 1
                                  GROUP BY SC.STUDENT_ID
                                  ORDER BY CC.CLASS_ID;";
                        break;
                    }

                case DashboardSQLCommands.FetchPassPercentageByExamSetting:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.REGISTER_NO,
                                SP.FIRST_NAME,
                                DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                CCR.COURSE_CODE,
                                CCR.COURSE_TITLE,
                                SR.M_PASS,
                                CPG.PROGRAMME_GROUP_ID,
                                CONCAT(CP.PROGRAMME_NAME, '-', S.SHIFT_NAME) AS SHIFT,
                                SB.BEGIN_YEAR,
                                SB.EXPIRED_YEAR,
                                SS.SEMESTER_NAME,
                                GROUP_CONCAT(SR.RESULT) AS RESULT,
                                CC.CLASS_ID,
                                CC.CLASS_NAME,
                                CC.CLASS_YEAR,
                                ES.EXAM_NAME,CCR.SEMESTER_ID
                            FROM
                                SEMESTER_MARKS_?AC_YEAR AS SR
                                    INNER JOIN
                                EXAM_REGISTRATION_?AC_YEAR AS ER ON ER.STUDENT_ID = SR.STUDENT_ID
                                    AND ER.COURSE_ID = SR.COURSE_ID
                                    AND ER.EXAM_SETTING_ID = SR.EXAM_SETTING_ID
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SR.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SR.COURSE_ID
                                    INNER JOIN
                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SR.SEMESTER
                                    INNER JOIN
                                SUP_BATCH_DURATION AS SB ON SB.BATCH_ID = SC.BATCH
                                    AND SB.APPLICATION_TYPE_ID = CPG.APPTYPE_ID
                                    INNER JOIN
                                SUP_SHIFT AS S ON S.SHIFT_ID = CPG.SHIFT
                                    INNER JOIN 
                                EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID=SR.EXAM_SETTING_ID
                            WHERE
                                    ES.IS_PUBLISHED=1
                                    AND ER.STATUS!='RA' AND ER.STATUS!='A'  AND ER.STATUS!='AAA'
                                    GROUP BY SC.STUDENT_ID ORDER BY CC.CLASS_YEAR;";
                        break;
                    }

                case DashboardSQLCommands.FetchFeedbackSettingByAcademicYear:
                    {
                        query = @"select FEEDBACK_ID,FEEDBACK_NAME from fbfeedback_settings where ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }

                case DashboardSQLCommands.FetchFeedbackStaffReportByAcademicYear:
                    {
                        query = @"SELECT 
                                         STAFF_CODE,
                                         UPPER(FIRST_NAME) AS FIRST_NAME,
                                         DD.DEPARTMENT,
                                         V.ASSESSEE,
                                         FIRST_NAME,
                                         COUNT(V.ASSESSEE) AS STUDENT_COUNT,
                                         COUNT(if(V.FEEDBACK_ID=8,IF(v.ANSWER=1,1,NULL),IF(v.ANSWER=6,1,NULL))) AS ANSWER,
                                         OBJECTIVES,V.FEEDBACK_ID,ROUND(COUNT(if(V.FEEDBACK_ID=8,IF(v.ANSWER=1,1,NULL),IF(v.ANSWER=6,1,NULL)))/COUNT(V.ASSESSEE) * 100,2) AS PERCENTAGE,FEEDBACK_NAME
                                     FROM
                                         fbevaluation?AC_YEAR AS V
                                             INNER JOIN
                                         FBFEEDBACK_QUESTIONS AS Q ON Q.QUESTION_ID = V.QUESTION
                                             INNER JOIN
                                             STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=V.ASSESSEE
                                             INNER JOIN
                                             CP_DEPARTMENT AS DD ON DD.DEPARTMENT_ID=SP.DEPARTMENT
                                              INNER JOIN
                                            FBOBJECTIVES AS FO ON FO.OBJECTIVE_ID=ANSWER
                                            INNER JOIN
                                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=V.COURSE_ID
                                            INNER JOIN
                                            fbfeedback_settings AS F ON F.FEEDBACK_ID=V.FEEDBACK_ID
                                            AND F.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                            fbfeedback_role_settings AS FR ON FR.FEEDBACK_ID=F.FEEDBACK_ID
                                            AND  FR.SUP_FEEDBACK_ROLE_ID = 4
                                     GROUP BY V.FEEDBACK_ID,V.ASSESSEE
                                     ORDER BY V.FEEDBACK_ID,PERCENTAGE DESC;";
                        break;
                    }
                case DashboardSQLCommands.FetchOptionalCourselistForStudent:
                    {
                        query = @"SELECT 
                                      SEMESTER_NAME, SEMESTER_ID, COURSE_CODE, COURSE_TITLE
                                  FROM
                                      stu_course_info AS SC
                                          INNER JOIN
                                      cp_course_root AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                          INNER JOIN
                                      sup_semester AS SS ON SS.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                  WHERE
                                      SC.ACADEMIC_YEAR = ?AC_YEAR
                                          AND SC.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }


                case DashboardSQLCommands.FetchAbsenteeCountForStudent:
                    {
                        query = @"SELECT 
                                      SC.STUDENT_ID,FIRST_NAME,ADMISSION_NO,COURSE_CODE,date_format(ATTENDENCE_DATE,'%d-%m-%Y') as ATTENDENCE_DATE
                                  FROM
                                      STU_CLASS AS SC
                                          INNER JOIN
                                      ATT_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = SC.STUDENT_ID
                                          AND ATT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                          AND ATT.ATTENDANCE_TYPE_ID != 1
                                          INNER JOIN
                                      ACADEMIC_SEMESTER AS AC ON AC.BATCH = SC.BATCH
                                          AND AC.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                          AND AC.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                          AND AC.IS_ACTIVE=1
                                          INNER JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ATT.COURSE_ID
                                          AND CR.SEMESTER_ID = AC.SEMESTER
                                          INNER JOIN
                                          STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
                                  WHERE
                                      SC.ACADEMIC_YEAR = ?AC_YEAR AND SC.STUDENT_ID=?STUDENT_ID
                                      GROUP BY ATTENDENCE_DATE;";
                        break;
                    }
                case DashboardSQLCommands.FetchUpcomingMeetings:
                    {
                        query = @"SELECT 
                                    MEETING_ID,
                                    MEETING_NAME,
                                    DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE,
                                    STAFF_CATEGORY,
                                    ATTENDEES,
                                    REPORT_FILE,
                                    MEETING_HOST,
                                    RESPONSIBLE_PERSON,
                                    START_TIME,
                                    END_TIME,
                                    ARRIVAL_BEFORE,
                                    AGENDA,
                                    FILE_UPLOAD,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    stf_meeting
                                WHERE
                                        STR_TO_DATE(CONCAT(DATE, ' ', END_TIME),
                                        '%Y-%m-%d %h:%i %p') >= NOW()
                                        AND IS_ACTIVE != 0
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case DashboardSQLCommands.FetchRoleByUserId:
                    {
                        query = @"SELECT 
                                  USER_ID,
                                  USER_TYPE,
                                  ACADEMIC_YEAR,
                                  GROUP_CONCAT(ROLE_ID) AS ROLE_ID
                              FROM
                                  USER_ROLES_BY_ACADEMIC_YEAR
                              WHERE
                                  USER_ID = ?USER_ID AND IS_DELETED != 1
                                      AND ACADEMIC_YEAR = ?AC_YEAR
                              GROUP BY USER_ID,USER_TYPE, ACADEMIC_YEAR;";
                        break;
                    }

                case DashboardSQLCommands.FetchRoleBasedDashboardMenu:
                    {
                        query = @"SELECT 
                                        USER_ROLE_ID,
                                        SR.ROLE_NAME,
                                        SHORT_NAME,
                                        RD.IS_ACTIVE,
                                        DD.ACTION_NAME,
                                        DD.CONTROLLER_NAME
                                    FROM
                                        ROLEBASED_DASHBOARD_MENU AS RD
                                            INNER JOIN
                                        DASHBOARD_MENU_ITEM AS DD ON DD.DASHBOARD_ID = RD.DASHBOARD_ID
                                            INNER JOIN
                                        SUP_ROLE AS SR ON SR.ROLE_ID = RD.ROLE_ID
                                    WHERE
                                        RD.ROLE_ID IN (?ROLE_ID) AND RD.IS_DELETED != 1
                                            AND DD.IS_DELETED != 1
                                            AND RD.IS_ACTIVE =1
                                    ORDER BY RD.ORDER_BY;
";
                        break;
                    }
                case DashboardSQLCommands.FetchStudentDetailsbyUserId:
                    {
                        query = @"SELECT 
                                      SC.STUDENT_ID,
                                      FIRST_NAME,
                                      IF(REGISTER_NO IS NULL,
                                          ADMISSION_NO,
                                          REGISTER_NO) AS REGISTER_NO,
                                      CLASS_NAME,
                                      H.STU_IIC_ID
                                  FROM
                                      STU_CLASS AS SC
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                          AND SP.IS_DELETED != 1
                                          AND SP.IS_LEFT != 1
                                          INNER JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                          LEFT JOIN
                                      IIC_STUDENT_INFO AS H ON H.STUDENT_ID = SC.STUDENT_ID
                                          AND H.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                  WHERE
                                  	SC.STUDENT_ID=?STUDENT_ID AND 
                                      SC.ACADEMIC_YEAR = ?AC_YEAR
                                          AND SC.IS_DELETED != 1";
                        break;
                    }
                case DashboardSQLCommands.FetchDashboardMenuList:
                    {
                        query = @"SELECT 
                                       DASHBOARD_ID,
                                       SHORT_NAME,
                                       ACTION_NAME,
                                       CONTROLLER_NAME,
                                       IS_GROUP,
                                       IS_ORDER,
                                       IS_ACTIVE,
                                       IS_DELETED
                                   FROM
                                       DASHBOARD_MENU_ITEM WHERE IS_DELETED!=1;";
                        break;
                    }
                case DashboardSQLCommands.BindDashBoardByDashBoardId:
                    {
                        query = @"SELECT 
                                       DASHBOARD_ID,
                                       SHORT_NAME,
                                       ACTION_NAME,
                                       CONTROLLER_NAME,
                                       IS_GROUP,
                                       IS_ORDER,
                                       IS_ACTIVE,
                                       IS_DELETED
                                   FROM
                                       DASHBOARD_MENU_ITEM WHERE DASHBOARD_ID=?DASHBOARD_ID;";
                        break;
                    }
                case DashboardSQLCommands.UpdateDashBoardMenu:
                    {
                        query = @"UPDATE DASHBOARD_MENU_ITEM SET 
                                       SHORT_NAME=?SHORT_NAME,
                                       ACTION_NAME=?ACTION_NAME,
                                       CONTROLLER_NAME=?CONTROLLER_NAME,
                                       IS_GROUP=?IS_GROUP,
                                       IS_ORDER=?IS_ORDER
                                    WHERE DASHBOARD_ID=?DASHBOARD_ID";
                        break;
                    }
                case DashboardSQLCommands.InsertDashBoardMenu:
                    {
                        query = @"INSERT DASHBOARD_MENU_ITEM (SHORT_NAME,
                                       ACTION_NAME,
                                       CONTROLLER_NAME,
                                       IS_GROUP,
                                       IS_ORDER) VALUES
                                        (?SHORT_NAME,
                                       ?ACTION_NAME,
                                       ?CONTROLLER_NAME,
                                       ?IS_GROUP,
                                       ?IS_ORDER);";
                        break;
                    }
                case DashboardSQLCommands.DeleteDashBoardmenu:
                    {
                        query = @"UPDATE DASHBOARD_MENU_ITEM SET 
                                       IS_DELETED=1
                                    WHERE DASHBOARD_ID=?DASHBOARD_ID";
                        break;
                    }
                case DashboardSQLCommands.FetchRole:
                    {
                        query = @"SELECT 
                                      ROLE_ID,
                                      ROLE_NAME,
                                      CONTROLLER_NAME,
                                      ACTION_NAME,
                                      IS_ACTIVE,
                                      IS_DELETED
                                  FROM
                                      SUP_ROLE
                                  WHERE
                                      IS_ACTIVE = 1;";
                        break;
                    }

                case DashboardSQLCommands.ActiveDeActiveDashBoardRole:
                    {
                        query = @"UPDATE ROLEBASED_DASHBOARD_MENU SET IS_ACTIVE=?IS_ACTIVE WHERE USER_ROLE_ID=?USER_ROLE_ID;";
                        break;
                    }

                case DashboardSQLCommands.FetchDateFrequencyBasedSettlementAmount:
                    {
                        query = @" SELECT 
                                FT.FREQUENCY,
                                CONCAT(FF.FREQUENCY_NAME,
                                        '( ',
                                        T.FREQUENCY_TYPE,
                                        ' )') AS FREQUENCY_NAME,
                                SUM(FT.COLLECTED) AS COLLECTED,
                                FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%d/%m/%Y') AS SETTLEMENT_DATE
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                fee_collection AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FM ON FM.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                    INNER JOIN
                                SUP_FREQUENCY_TYPE AS T ON T.FREQUENCY_TYPE_ID = FM.FEE_MODE
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FM.FREQUENCY_ID
                            WHERE
                                FT.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FT.IS_DELETED != 1
                                    AND FT.PAYMENT_DATE BETWEEN DATE_SUB(DATE_FORMAT(NOW(), '%Y-%m-%d'),
                                    INTERVAL 30 DAY) AND DATE_FORMAT(NOW(), '%Y-%m-%d')
                                    AND FC.SETTLEMENT_DATE IS NOT NULL
                            GROUP BY FT.PAYMENT_DATE , FT.FREQUENCY
                            ORDER BY FT.PAYMENT_DATE , FT.FREQUENCY;";
                        break;
                    }
                case DashboardSQLCommands.FetchFrequencyBasedTotalSettlement:
                    {
                        query = @"SELECT 
                                FT.FREQUENCY,
                                CONCAT(FF.FREQUENCY_NAME,
                                        '( ',
                                        T.FREQUENCY_TYPE,
                                        ' )') AS FREQUENCY_NAME,
                                SUM(FT.COLLECTED) AS COLLECTED
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                fee_collection AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FM ON FM.FEE_FREQUENCY_FEE_MODE_ID = FT.FREQUENCY
                                    INNER JOIN
                                SUP_FREQUENCY_TYPE AS T ON T.FREQUENCY_TYPE_ID = FM.FEE_MODE
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FM.FREQUENCY_ID
                            WHERE
                                FT.ACADEMIC_YEAR = ?AC_YEAR
                                    AND FT.IS_DELETED != 1
                                    AND FC.SETTLEMENT_DATE IS NOT NULL
                            GROUP BY FT.FREQUENCY
                            ORDER BY FT.FREQUENCY;";
                        break;
                    }
                case DashboardSQLCommands.FetchNotPaidListByFrequency:
                    {
                        query = @"SELECT 
                                STUDENT_ID,
                                IF(SUM((IFNULL(FS.CREDIT, 0) - IFNULL(FS.DEBIT, 0))) = 0,
                                    '1',
                                    '0') AS STATUS,
                                FS.FREQUENCY_ID,
                                FREQUENCY_NAME
                            FROM
                                fee_student_account AS FS
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FM ON FM.FEE_FREQUENCY_FEE_MODE_ID = FS.FREQUENCY_ID
                                    INNER JOIN
                                SUP_FREQUENCY_TYPE AS T ON T.FREQUENCY_TYPE_ID = FM.FEE_MODE
                                    INNER JOIN
                                SUP_FEE_FREQUENCY AS FF ON FF.FREQUENCY_ID = FM.FREQUENCY_ID
                            WHERE
                                FS.ACADEMIC_YEAR = 2022
                            GROUP BY FS.FREQUENCY_ID , FS.STUDENT_ID;";
                        break;
                    }
            }
            return query;
        }
    }


}
