using CMS.DAO;

namespace CMS.SQL.OnlineClass
{
    public static class OnlineClassSQL
    {
        public static string GetOnlineClassSQL(OnlineClassSQLCommads sEnumCommand)
        {
            string sQuery = string.Empty;
            switch (sEnumCommand)
            {

                #region Online Class
                case OnlineClassSQLCommads.FetchTimeTableForStaffOnlineClass:
                    {
                        sQuery = @"SELECT 
                                        CLS.CLASS_CODE,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                '(',
                                                SP.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                        TT.HOUR_ID,
                                        SH.HOURTYPE,
                                        CLS.SHIFT,
                                        CLS.ONLINE_CLASS_LINK
                                    FROM
                                        TT_ONLINE_TIMETABLE AS TT
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE = CURDATE()
                                    WHERE
                                        TT.STAFF_ID = ?USER_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1
                                    ORDER BY CLS.SHIFT , TT.HOUR_ID;";
                        break;
                    }
                case OnlineClassSQLCommads.FetchTimeTableForStudentOnlineClass:
                    {
                        sQuery = @"SELECT 
                                        CLS.CLASS_CODE,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                '(',
                                                SP.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                        TT.HOUR_ID,
                                        SH.HOURTYPE,
                                        CLS.SHIFT,
                                        CLS.ONLINE_CLASS_LINK
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        TT_ONLINE_TIMETABLE AS TT ON TT.CLASS_ID=SC.CLASS_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
		                                    AND SP.IS_DELETED!=1
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE = CURDATE()
                                    WHERE
                                        SC.STUDENT_ID = ?USER_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            UNION
                                    SELECT 
                                        CLS.CLASS_CODE,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                '(',
                                                SP.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                        TT.HOUR_ID,
                                        SH.HOURTYPE,
                                        CLS.SHIFT,
                                        CLS.ONLINE_CLASS_LINK
                                    FROM
                                        stu_course_info AS SC
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.S_CLASS_ID
                                            INNER JOIN
                                        TT_ONLINE_TIMETABLE AS TT ON TT.CLASS_ID=SC.S_CLASS_ID
		                                    AND TT.COURSE_ID=SC.COURSE_ID
                                            AND TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND TT.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
		                                    AND SP.IS_DELETED!=1
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE = CURDATE()
                                    WHERE
                                        SC.STUDENT_ID = ?USER_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    ORDER BY SHIFT , HOUR_ID;";
                        break;
                    }
                #endregion

                #region Online Class Report
                case OnlineClassSQLCommads.FetchOnlineClassTimeTableReportClassWise:
                    {
                        sQuery = @"SELECT 
                                        CLS.CLASS_CODE,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                '(',
                                                SP.STAFF_CODE,
                                                ')') AS STAFF_CODE,
                                        TT.HOUR_ID,
                                        SH.HOURTYPE,
                                        CLS.SHIFT,
                                        CLS.ONLINE_CLASS_LINK
                                    FROM
                                        TT_ONLINE_TIMETABLE AS TT
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = TT.CLASS_ID
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                            LEFT JOIN
                                        SUP_HOUR_TYPE AS SH ON SH.HOUR_TYPE_ID = TT.HOUR_TYPE
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                            INNER JOIN
                                        CALENDER AS CAL ON CAL.DAY_ORDER = TT.DAY_ORDER_ID
                                            AND CAL.DAY_ORDER_DATE = CURDATE()
                                    WHERE
                                        TT.CLASS_ID = ?CLASS_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1
                                    ORDER BY CLS.SHIFT , TT.HOUR_ID;";
                        break;
                    }
                #endregion

                #region SupportData
                case OnlineClassSQLCommads.FetchSupHour:
                    {

                        break;
                    }
                #endregion
                default:
                    break;
            }
            return sQuery;
        }
    }
}
