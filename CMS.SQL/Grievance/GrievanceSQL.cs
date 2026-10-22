using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Grievance
{
    public static class GrievanceSQL
    {
        public static string GetGrievanceSQL(GrievanceSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case GrievanceSQLCommands.FetchGrievanceType:
                    {
                        query = @"SELECT 
                                    GRIEVANCE_TYPE_ID,
                                    GRIEVANCE_TYPE_NAME,
                                    PREFIX_CODE,
                                    GRIEVANCE_TYPE_CODE,
                                    HAS_SUB,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    GRIEVANCE_TYPE
                                WHERE
                                    IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.AddGrievance:
                    {
                        query = @"INSERT INTO GRIEVANCE_GRIEVANCEINFO(USER_ID, GRIEVANCE_CODE, GRIEVANCE_TYPE_ID, GRIEVANCE_TITLE, GRIEVANCE_SUBJECT, GRIEVANCE_CONTENT, SUB_DATE_TIME, ACADEMIC_YEAR, STATUS)
                                 VALUES(?USER_ID,?GRIEVANCE_CODE,?GRIEVANCE_TYPE_ID,?GRIEVANCE_TITLE,?GRIEVANCE_SUBJECT,?GRIEVANCE_CONTENT,NOW(),?ACADEMIC_YEAR,?STATUS); ";
                        break;
                    }
                case GrievanceSQLCommands.AddGrievanceAttachment:
                    {
                        query = @"INSERT INTO grievance_attachmentinfo(GRIEVANCE_ID, ATTACHMENT_PATH, ATTACHMENT_NAME,IS_TYPE)
                                  VALUES(?GRIEVANCE_ID,?ATTACHMENT_PATH,?ATTACHMENT_NAME,?IS_TYPE);";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceByType:
                    {
                        query = @"SELECT 
                                    PREFIX_CODE,GRIEVANCE_TYPE_NAME
                                FROM
                                    GRIEVANCE_TYPE
                                WHERE
                                    GRIEVANCE_TYPE_ID =?GRIEVANCE_TYPE_ID
                                        AND IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceInfoByCode:
                    {
                        query = @"SELECT 
                                    GRIEVANCE_ID
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO
                                WHERE
                                    GRIEVANCE_CODE =?GRIEVANCE_CODE
                                        AND IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceInfo:
                    {
                        query = @"SELECT 
                                    GI.GRIEVANCE_ID,
                                    GI.USER_ID,
                                    CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                    GI.GRIEVANCE_CODE,
                                    GT.GRIEVANCE_TYPE_ID,
                                    GT.GRIEVANCE_TYPE_NAME,
                                    GI.GRIEVANCE_SUBJECT,
                                    GI.GRIEVANCE_CONTENT,
                                    IF(GI.ASSIGN_TO != 0,
                                        IF(GI.ASSIGN_TO IS NOT NULL,
                                            CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                    '',
                                                    IFNULL(SP.LAST_NAME, '')),
                                            ''),
                                        '') AS STAFF_NAME,
                                    IF(GRII.IS_READ = 1,
                                        GR.REDRESSAL_CONTENT,
                                        '') AS REDRESSAL_CONTENT,
                                    GI.SUB_DATE_TIME,
                                    GR.REP_DATE_TIME,
                                    GI.ACADEMIC_YEAR,
                                    GI.STATUS,
                                    GRI.IS_READ,
                                    GRD.IS_VIEWED
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                        AND GT.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON GI.USER_ID = SPI.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    GRIEVANCE_MEMBERINFO AS GM ON GM.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                        AND GM.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON GM.STAFF_ID = SP.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        LEFT JOIN
                                    GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID = GR.GRIEVANCE_ID
                                        AND GR.IS_DELETED != 1
                                        INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 1
                                        AND GRI.IS_DELETED != 1
                                        LEFT JOIN
                                    GRIEVANCE_REMINDERINFO AS GRII ON GRII.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRII.IS_TYPE = 2
                                        AND GRII.IS_READ != 0
                                        AND GRI.IS_DELETED != 1
                                        LEFT JOIN
                                    GRIEVANCE_REMINDER AS GRD ON GI.GRIEVANCE_ID = GRD.GRIEVANCE_ID
                                        AND GRD.IS_DELETED != 1
                                        AND GRD.IS_VIEWED != 1
                                WHERE
                                    GI.USER_ID =?USER_ID AND GI.STATUS != 2
                                        AND GI.ACADEMIC_YEAR =?AC_YEAR
                                        AND GI.IS_DELETED != 1
                                        GROUP BY  GI.GRIEVANCE_ID ORDER BY GI.GRIEVANCE_ID;";
                        break;
                    }
                
                case GrievanceSQLCommands.FetchGrievanceInfoById:
                    {
                        query = @"
                                    SELECT 
                                    GI.GRIEVANCE_ID,
                                   
                                   CONCAT(IFNULL(SPI.FIRST_NAME, ''),
		                                 ' ',
		                                  IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                   SPI.ROLL_NO,
                                   SPI.PHOTO,
                                    CC.CLASS_NAME,
                                    GI.GRIEVANCE_CODE,
                                    GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                    GI.GRIEVANCE_SUBJECT,
                                    GI.GRIEVANCE_CONTENT,
                                    GA.ATTACHMENT_PATH,
                                    GI.SUB_DATE_TIME,
                                    GI.ACADEMIC_YEAR,
                                    GI.STATUS
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                        AND GT.IS_DELETED != 1
                                        LEFT JOIN
                                    GRIEVANCE_ATTACHMENTINFO AS GA ON GI.GRIEVANCE_ID = GA.GRIEVANCE_ID
                                        AND GA.IS_DELETED != 1
                                        AND IS_TYPE = 0
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = GI.USER_ID
                                    AND SPI.IS_LEFT!=1
                                        INNER JOIN
                                       STU_CLASS AS SC ON SC.STUDENT_ID=GI.USER_ID AND SC.ACADEMIC_YEAR=GI.ACADEMIC_YEAR
                                       AND SC.IS_DELETED!=1
                                        INNER JOIN
                                     CP_DEPARTMENT AS CD ON SPI.DEPT_ID = CD.DEPARTMENT_ID
                                        AND CD.IS_DELETED != 1
                                        INNER JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                       AND CC.IS_DELETED != 1
                                            INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 1
                                        AND GRI.IS_READ != 0
                                        AND GRI.IS_DELETED != 1
                                       WHERE
                                       GI.GRIEVANCE_ID =?GRIEVANCE_ID
                                       AND GI.ACADEMIC_YEAR=?AC_YEAR
                                       AND GI.STATUS != 2
                                       AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceInfoByType:
                    {
                        query = @"SELECT 
                                GI.GRIEVANCE_ID,
                                GI.USER_ID,
                                CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                GI.GRIEVANCE_CODE,
                                GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                GI.GRIEVANCE_SUBJECT,
                                GI.GRIEVANCE_CONTENT,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        '',
                                        IFNULL(SP.LAST_NAME, '')) AS STAFF_NAME,
                                IF(GRII.IS_READ = 1,
                                    GR.REDRESSAL_CONTENT,
                                    '') AS REDRESSAL_CONTENT,
                                GI.SUB_DATE_TIME,
                                GR.REP_DATE_TIME,
                                GI.ACADEMIC_YEAR,
                                GI.STATUS
                            FROM
                                GRIEVANCE_GRIEVANCEINFO AS GI
                                    INNER JOIN
                                GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                    AND GT.IS_DELETED != 1
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=GI.USER_ID 
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT!=1
                                    INNER JOIN
                                GRIEVANCE_MEMBERINFO AS GM ON GM.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                AND GM.STAFF_ID=GI.ASSIGN_TO
                                    AND GM.IS_DELETED != 1
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON GM.STAFF_ID = SP.STAFF_ID
                                    AND SP.IS_DELETED != 1
                                    LEFT JOIN
                                GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID = GR.GRIEVANCE_ID
                                    AND GR.IS_DELETED != 1
                                    LEFT JOIN
                                GRIEVANCE_REMINDERINFO AS GRII ON GRII.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                    AND GRII.IS_TYPE = 2
                                    AND GRII.IS_READ != 0
                            WHERE
                                GI.USER_ID =?USER_ID AND GI.STATUS != 2
                                AND GI.ACADEMIC_YEAR=?AC_YEAR
                                AND GI.GRIEVANCE_TYPE_ID=?GRIEVANCE_TYPE_ID
                                    AND GI.IS_DELETED != 1
                                    ORDER BY GI.GRIEVANCE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceByStaff:
                    {
                        query = @"                               
                                
                                    SELECT 
                                    GI.GRIEVANCE_ID,
                                    GI.USER_ID,
                                    CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(CP.CLASS_NAME, ''),
                                            '(',
                                            IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                            ')') AS CLASS_NAME,
                                    GI.GRIEVANCE_CODE,
                                    GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                    GI.GRIEVANCE_SUBJECT,
                                    GI.SUB_DATE_TIME,
                                    GI.ACADEMIC_YEAR,
                                    GI.STATUS
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                        AND GT.IS_DELETED != 1
                                        INNER JOIN
                                    GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                    AND MI.STAFF_ID=GI.ASSIGN_TO
                                    AND MI.IS_DELETED!=1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=GI.USER_ID 
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT!=1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = GI.USER_ID
                                        AND SC.ACADEMIC_YEAR = GI.ACADEMIC_YEAR
                                        AND SC.IS_DELETED!=1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                        AND CP.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_SHIFT AS SH ON SH.SHIFT_ID = CP.SHIFT
                                        INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 1
                                        AND GRI.IS_READ != 0
                                        AND GRI.IS_DELETED != 1
                                WHERE
                                    MI.STAFF_ID =?STAFF_ID AND GI.STATUS != 2
                                    AND GI.ACADEMIC_YEAR=?AC_YEAR
                                        AND GI.IS_DELETED != 1
                                        GROUP BY GI.GRIEVANCE_ID ORDER BY GI.GRIEVANCE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceCloseInfoByStaff:
                    {
                        query = @"SELECT 
                                        GI.GRIEVANCE_ID,
                                        GI.USER_ID,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(CP.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME,
                                         GI.GRIEVANCE_CODE,
                                        GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                        GI.GRIEVANCE_SUBJECT,
                                        GI.GRIEVANCE_CONTENT,
                                        GI.SUB_DATE_TIME,
                                        GI.ACADEMIC_YEAR,
                                        GI.STATUS
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                            AND GT.IS_DELETED != 1
                                            INNER JOIN
                                        GRIEVANCE_MEMBERINFO AS MI ON GI.GRIEVANCE_TYPE_ID = MI.GRIEVANCE_TYPE_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON GI.USER_ID = SPI.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = GI.USER_ID
                                            AND SC.ACADEMIC_YEAR = GI.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CP.SHIFT
                                    WHERE
                                        MI.STAFF_ID =?STAFF_ID AND GI.STATUS = 2
                                            AND SC.CLASS_ID =?CLASS_ID
                                            AND GI.GRIEVANCE_TYPE_ID=?GRIEVANCE_TYPE_ID
                                            AND DATE_FORMAT(GI.SUB_DATE_TIME,'%d/%m/%Y')=?SUB_DATE_TIME
                                            AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceInfoByDayWise:
                    {
                        query = @"
                                        SELECT 
                                        GI.GRIEVANCE_ID,
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(CP.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME,
                                         GI.GRIEVANCE_CODE,
                                        GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                        GI.GRIEVANCE_SUBJECT,
                                        GI.GRIEVANCE_CONTENT,
                                        GI.SUB_DATE_TIME,
                                        GR.REDRESSAL_CONTENT,
                                        GR.REP_DATE_TIME,
                                        GI.ACADEMIC_YEAR
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                            AND GT.IS_DELETED != 1
                                            INNER JOIN
                                        GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                        AND MI.STAFF_ID=GI.ASSIGN_TO
                                        AND MI.IS_DELETED!=1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=GI.USER_ID 
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT!=1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = GI.USER_ID
                                            AND SC.ACADEMIC_YEAR = GI.ACADEMIC_YEAR
                                            AND SC.IS_DELETED!=1
                                            INNER JOIN
                                        CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CP.SHIFT
                                        LEFT JOIN 
                                        GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID=GR.GRIEVANCE_ID
                                    WHERE
                                        MI.STAFF_ID =?STAFF_ID AND GI.STATUS !=2
                                        AND GI.ACADEMIC_YEAR=?AC_YEAR
                                          AND DATE_FORMAT(GI.SUB_DATE_TIME,'%d/%m/%Y')=?SUB_DATE_TIME
                                            AND GI.GRIEVANCE_TYPE_ID IN(?GRIEVANCE_TYPE_ID)
                                            AND GI.IS_DELETED != 1 
                                            ORDER BY GI.GRIEVANCE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceInfoByGroupWise:
                    {
                        query = @"SELECT 
                                        GI.GRIEVANCE_ID,
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(IFNULL(CP.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME,
                                         GI.GRIEVANCE_CODE,
                                        GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                        GI.GRIEVANCE_SUBJECT,
                                        GI.GRIEVANCE_CONTENT,
                                        GI.SUB_DATE_TIME,
                                        GR.REDRESSAL_CONTENT,
                                        GR.REP_DATE_TIME,
                                        GI.ACADEMIC_YEAR
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                            AND GT.IS_DELETED != 1
                                            INNER JOIN
                                        GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                        AND MI.STAFF_ID=GI.ASSIGN_TO
                                        AND MI.IS_DELETED!=1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=GI.USER_ID 
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT!=1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = GI.USER_ID
                                            AND SC.ACADEMIC_YEAR = GI.ACADEMIC_YEAR
                                            AND SC.IS_DELETED!=1
                                            INNER JOIN
                                        CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CP.SHIFT
                                        LEFT JOIN 
                                        GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID=GR.GRIEVANCE_ID
                                    WHERE
                                        MI.STAFF_ID =?STAFF_ID AND GI.STATUS !=2
                                        AND GI.ACADEMIC_YEAR=?AC_YEAR
                                          AND (DATE_FORMAT(GI.SUB_DATE_TIME,'%d/%m/%Y') BETWEEN ?START_DATE AND ?END_DATE)
                                            AND GI.GRIEVANCE_TYPE_ID IN(?GRIEVANCE_TYPE_ID)
                                            AND GI.IS_DELETED != 1
                                            ORDER BY GI.GRIEVANCE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceInfoByMonthWise:
                    {
                        query = @"
                               
                                    SELECT 
                                    GI.GRIEVANCE_ID,
                                    SPI.ROLL_NO,
                                    CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(CP.CLASS_NAME, ''),
                                            '(',
                                            IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                            ')') AS CLASS_NAME,
                                    GI.GRIEVANCE_CODE,
                                    GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                    GI.GRIEVANCE_SUBJECT,
                                    GI.GRIEVANCE_CONTENT,
                                    GI.SUB_DATE_TIME,
                                    GR.REDRESSAL_CONTENT,
                                    GR.REP_DATE_TIME,
                                    GI.ACADEMIC_YEAR
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                        AND GT.IS_DELETED != 1
                                        INNER JOIN
                                    GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                    AND MI.STAFF_ID=GI.ASSIGN_TO
                                    AND MI.IS_DELETED!=1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=GI.USER_ID  
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT!=1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = GI.USER_ID
                                        AND SC.ACADEMIC_YEAR = GI.ACADEMIC_YEAR
                                        AND SC.IS_DELETED!=1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                        AND CP.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_SHIFT AS SH ON SH.SHIFT_ID = CP.SHIFT
                                        INNER JOIN
                                    CALENDER AS CAL ON GI.ACADEMIC_YEAR = CAL.ACADEMIC_YEAR
                                    AND CAL.DAY_ID=?DAY_ID
                                        AND CAL.IS_DELETED != 1
                                        LEFT JOIN
                                    GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID = GR.GRIEVANCE_ID
                                WHERE
                                    MI.STAFF_ID =?STAFF_ID AND GI.STATUS != 2
                                    AND GI.ACADEMIC_YEAR=?AC_YEAR
                                        AND DATE_FORMAT(GI.SUB_DATE_TIME, '%m') = DATE_FORMAT(CAL.DAY_ORDER_DATE, '%m')
                                        AND GI.GRIEVANCE_TYPE_ID IN (?GRIEVANCE_TYPE_ID)
                                        AND GI.IS_DELETED != 1
                                        ORDER BY GI.GRIEVANCE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceByGrievanceId:
                    {
                        query = @"
                                SELECT 
                                GI.GRIEVANCE_ID,
                                GI.USER_ID,
                                SPI.ROLL_NO,
                                SPI.PHOTO,
                                CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                CONCAT(IFNULL(CP.CLASS_NAME, ''),
                                        '(',
                                        IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                        ')') AS CLASS_NAME,
                                GI.GRIEVANCE_CODE,
                                GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TYPE_ID,
                                GI.GRIEVANCE_SUBJECT,
                                GI.GRIEVANCE_CONTENT,
                                GI.SUB_DATE_TIME,
                                GI.ACADEMIC_YEAR,
                                GI.STATUS
                            FROM
                                GRIEVANCE_GRIEVANCEINFO AS GI
                                    INNER JOIN
                                GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                    AND GT.IS_DELETED != 1
                                    INNER JOIN
                                GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                    AND GRI.IS_TYPE = 1
                                    AND GRI.IS_READ != 0
                                    AND GRI.IS_DELETED != 1
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = GI.USER_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT!=1
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = GI.USER_ID
                                    AND SC.ACADEMIC_YEAR = GI.ACADEMIC_YEAR
                                    AND SC.IS_DELETED!=1
                                    INNER JOIN
                                CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                    AND CP.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SHIFT AS SH ON SH.SHIFT_ID = CP.SHIFT
                                    INNER JOIN
                                GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                AND MI.STAFF_ID=GI.ASSIGN_TO
                                    AND MI.IS_DELETED != 1
                            WHERE
                                GI.GRIEVANCE_ID =?GRIEVANCE_ID AND GI.STATUS != 2
                                AND GI.ACADEMIC_YEAR=?AC_YEAR
                                    AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.AddGrievanceRedressalInfo:
                    {
                        query = @"INSERT INTO GRIEVANCE_REDRESSALINFO(GRIEVANCE_ID, REDRESSAL_CONTENT, REP_DATE_TIME)
                                  VALUES(?GRIEVANCE_ID,?REDRESSAL_CONTENT,NOW());";
                        break;
                    }
                case GrievanceSQLCommands.UpdateGrievanceInfoStatus:
                    {
                        query = @"UPDATE GRIEVANCE_GRIEVANCEINFO 
                                  SET 
                                        STATUS =?STATUS
                                    WHERE
                                        GRIEVANCE_ID =?GRIEVANCE_ID
                                            AND IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceRedressalByGrievanceId:
                    {
                        query = @"SELECT 
                                    REDRESSAL_ID,
                                    GRIEVANCE_ID,
                                    REDRESSAL_CONTENT,
                                    REP_DATE_TIME,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    GRIEVANCE_REDRESSALINFO
                                WHERE
                                    GRIEVANCE_ID =?GRIEVANCE_ID AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.UpdateRedressalInfo:
                    {
                        query = @"UPDATE GRIEVANCE_REDRESSALINFO 
                                SET 
                                    GRIEVANCE_ID =?GRIEVANCE_ID,
                                    REDRESSAL_CONTENT =?REDRESSAL_CONTENT,
                                    REP_DATE_TIME = NOW()
                                WHERE
                                    REDRESSAL_ID =?REDRESSAL_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchRedressalInfo:
                    {
                        query = @"SELECT 
                                GI.GRIEVANCE_ID,
                                GR.REDRESSAL_ID,
                                GR.REDRESSAL_CONTENT,
                                GR._DATE_TIME,
                                GR.IS_ACTIVE,
                                GR.IS_DELETED,
                                GA.ATTACHMENT_PATH,
                                GA.ATTACHMENT_ID,
                                UA.NAME
                            FROM
                                GRIEVANCE_GRIEVANCEINFO AS GI
                                    RIGHT JOIN
                                GRIEVANCE_REDRESSALINFO AS GR ON GR.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                    LEFT JOIN
                                GRIEVANCE_ATTACHMENTINFO AS GA ON GR.GRIEVANCE_ID = GA.GRIEVANCE_ID
                                    AND GA.IS_TYPE = 1
                                    INNER JOIN
                                USER_ACCOUNT AS UA ON UA.USER_ID = GI.USER_ID
                            WHERE
                                GI.USER_ID =?USER_ID AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchAttachment:
                    {
                        query = @"SELECT 
                                    GA.ATTACHMENT_ID,
                                    GA.GRIEVANCE_ID,
                                    GA.ATTACHMENT_PATH,
                                    GA.ATTACHMENT_NAME,
                                    GA.IS_ACTIVE,
                                    GA.IS_DELETED,
                                    GA.IS_TYPE
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO  AS GI
                                        INNER JOIN
                                   GRIEVANCE_ATTACHMENTINFO  AS GA ON GI.GRIEVANCE_ID = GA.GRIEVANCE_ID
                                        AND GA.IS_TYPE = 0 AND GA.IS_DELETED!=1
                                WHERE
                                    GI.USER_ID =?USER_ID AND GI.GRIEVANCE_ID=?GRIEVANCE_ID 
                                  AND GI.IS_DELETED != 1 
                                  AND GI.STATUS!=2";
                        break;
                    }
                case GrievanceSQLCommands.FetchRedressalAttachment:
                    {
                        query = @"
                                    SELECT 
                                    GA.ATTACHMENT_ID,
                                    GA.GRIEVANCE_ID,
                                    GA.ATTACHMENT_PATH,
                                    GA.ATTACHMENT_NAME,
                                    GA.IS_ACTIVE,
                                    GA.IS_DELETED,
                                    GA.IS_TYPE
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_ATTACHMENTINFO AS GA ON GA.GRIEVANCE_ID=GI.GRIEVANCE_ID 
                                        AND GA.IS_TYPE = 1
                                        AND GA.IS_DELETED != 1
                                        INNER JOIN
                                    GRIEVANCE_REDRESSALINFO AS GR ON  GR.GRIEVANCE_ID=GI.GRIEVANCE_ID 
                                        INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 2
                                        AND GRI.IS_READ != 0
                                        AND GRI.IS_DELETED != 1
                                WHERE
                                    GI.USER_ID =?USER_ID
                                        AND GI.GRIEVANCE_ID =?GRIEVANCE_ID
                                        AND GI.STATUS = 1
                                        AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.UserNewComplaints:
                    {
                        query = @"
                                    SELECT 
                                    GI.GRIEVANCE_ID
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 2
                                        AND GRI.IS_READ != 1
                                        AND GRI.IS_DELETED != 1
                                        LEFT JOIN
                                    GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID = GR.GRIEVANCE_ID
                                WHERE
                                    GI.USER_ID =?USER_ID AND GI.STATUS != 2
                                        AND GI.ACADEMIC_YEAR=?AC_YEAR
                                        AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.AddRemainder:
                    {
                        query = @"INSERT INTO grievance_reminderinfo(GRIEVANCE_ID, DATE,IS_READ,IS_TYPE)
                                  VALUES(?GRIEVANCE_ID,NOW(),?IS_READ,?IS_TYPE);";
                        break;
                    }
                case GrievanceSQLCommands.UpdateReaminderInfo:
                    {
                        query = @"UPDATE GRIEVANCE_REMINDERINFO 
                                SET 
                                    IS_READ =?IS_READ
                                WHERE
                                    GRIEVANCE_ID =?GRIEVANCE_ID AND IS_TYPE=2 AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.UpdateStaffRemainderInfo:
                    {
                        query = @"UPDATE GRIEVANCE_REMINDERINFO 
                                SET 
                                    IS_READ =?IS_READ
                                WHERE
                                    GRIEVANCE_ID =?GRIEVANCE_ID AND IS_TYPE=1 AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.StaffNewComplaints:
                    {
                        query = @"                                        
                                SELECT 
                                    GI.GRIEVANCE_ID
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                    AND MI.STAFF_ID=GI.ASSIGN_TO
                                    AND MI.IS_DELETED!=1
                                        INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 1
                                        AND GRI.IS_READ != 1
                                        AND GRI.IS_DELETED != 1
                                WHERE
                                    MI.STAFF_ID =?STAFF_ID AND GI.STATUS != 2
                                    AND GI.ACADEMIC_YEAR=?AC_YEAR
                                        AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.CloseComplaintById:
                    {
                        query = @"UPDATE GRIEVANCE_GRIEVANCEINFO 
                                    SET 
                                        STATUS =?STATUS
                                    WHERE
                                        GRIEVANCE_ID =?GRIEVANCE_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.AddRemaind:
                    {
                        query = @" INSERT INTO GRIEVANCE_REMINDER(GRIEVANCE_ID, DATE)
                                   VALUES(?GRIEVANCE_ID,NOW());";
                        break;
                    }
                case GrievanceSQLCommands.FetchRemaind:
                    {
                        query = @" 
                                
                                     SELECT 
                                    GI.GRIEVANCE_ID,
                                    CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                    GI.GRIEVANCE_SUBJECT,
                                    GD.DATE AS SUB_DATE_TIME
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                        AND GT.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=GI.USER_ID 
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT!=1
                                        INNER JOIN
                                    GRIEVANCE_MEMBERINFO AS GM ON GM.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                    AND GM.STAFF_ID=GI.ASSIGN_TO
                                        AND GM.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON GM.STAFF_ID = SP.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    GRIEVANCE_REMINDERINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        AND GRI.IS_TYPE = 1
                                        AND GRI.IS_READ != 0
                                        INNER JOIN
                                    GRIEVANCE_REMINDER AS GD ON GI.GRIEVANCE_ID = GD.GRIEVANCE_ID
                                        AND GD.IS_DELETED != 1
                                        AND GD.IS_VIEWED != 1
                                WHERE
                                    GM.STAFF_ID =?STAFF_ID AND GI.STATUS != 2
                                    AND GI.ACADEMIC_YEAR=?AC_YEAR
                                        AND GI.IS_DELETED != 1
                                        ORDER BY GI.GRIEVANCE_ID; ";
                        break;
                    }
                case GrievanceSQLCommands.UpdateRemaind:
                    {
                        query = @"UPDATE GRIEVANCE_REMINDER 
                                    SET 
                                        IS_VIEWED =?IS_VIEWED
                                    WHERE
                                        GRIEVANCE_ID =?GRIEVANCE_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchTotalComplaintsForUser:
                    {
                        query = @"
                                   
                                SELECT 
                                    COUNT(GRIEVANCE_ID) AS TOTAL_COMPLAINT,
                                    SUM(IF(STATUS = 1, 1, 0)) AS REPLIED_COMPLAINT,
                                    SUM(IF(STATUS = 0, 1, 0)) AS PENDING_COMPLAINT
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO
                                WHERE
                                    IS_DELETED != 1 AND STATUS != 2
                                     AND ACADEMIC_YEAR=?AC_YEAR
                                        AND USER_ID = ?USER_ID;";
                        break;
                    }
                case GrievanceSQLCommands.StaffTotalComplaints:
                    {
                        query = @"       
                                    SELECT 
                                    COUNT(GI.GRIEVANCE_ID) AS TOTAL_COMPLAINT,
                                    SUM(IF(STATUS=1,1,0)) AS REPLIED_COMPLAINT,
                                    SUM(IF(STATUS=0,1,0)) AS PENDING_COMPLAINT
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GI.GRIEVANCE_TYPE_ID = GT.GRIEVANCE_TYPE_ID
                                        AND GT.IS_DELETED != 1
                                          INNER JOIN
                                    GRIEVANCE_MEMBERINFO AS MI ON MI.GRIEVANCE_TYPE_ID  = GI.GRIEVANCE_TYPE_ID 
                                    AND MI.STAFF_ID=GI.ASSIGN_TO	
                                        AND MI.IS_DELETED != 1
        
                                WHERE
                                    MI.STAFF_ID =?STAFF_ID 
                                    AND GI.ACADEMIC_YEAR=?AC_YEAR
                                    AND GI.IS_DELETED!=1
                                    AND GI.STATUS != 2;";
                        break;
                    }              
                case GrievanceSQLCommands.FetchStaffList:
                    {
                        query = @"SELECT 
                                SP.STAFF_ID,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        '(',
                                        IFNULL(STAFF_CODE, ''),
                                        ')') AS FIRST_NAME,
                                U.ROLE AS ROLES
                            FROM
                                STF_PERSONAL_INFO AS SP
                                    INNER JOIN
                                user_account AS U ON U.USER_ID = SP.STAFF_ID AND USER_TYPE=4;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceTypeList:
                    {
                        query = @"SELECT 
                                        GRIEVANCE_TYPE_ID, GRIEVANCE_TYPE_NAME,GRIEVANCE_TYPE_CODE
                                    FROM
                                        GRIEVANCE_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceTypeListById:
                    {
                        query = @"SELECT 
                                    GRIEVANCE_TYPE_ID, GRIEVANCE_TYPE_NAME, GRIEVANCE_TYPE_CODE
                                FROM
                                    GRIEVANCE_TYPE
                                WHERE
                                    GRIEVANCE_TYPE_ID = ?GRIEVANCE_TYPE_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.SaveGrievanceType:
                    {
                        query = @"INSERT INTO GRIEVANCE_TYPE(GRIEVANCE_TYPE_NAME,PREFIX_CODE,GRIEVANCE_TYPE_CODE) VALUES(?GRIEVANCE_TYPE_NAME,?PREFIX_CODE,CONCAT('GRI',?PREFIX_CODE,(LAST_INSERT_ID()+1)));";
                        break;
                    }
                case GrievanceSQLCommands.DeleteGrievanceType:
                    {
                        query = @"UPDATE GRIEVANCE_TYPE SET IS_DELETED=1 WHERE GRIEVANCE_TYPE_ID=?GRIEVANCE_TYPE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.UpdateGrievanceType:
                    {
                        query = @"UPDATE GRIEVANCE_TYPE SET GRIEVANCE_TYPE_NAME=?GRIEVANCE_TYPE_NAME WHERE GRIEVANCE_TYPE_ID=?GRIEVANCE_TYPE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.SaveGrievancemembers:
                    {
                        query = @"INSERT INTO GRIEVANCE_MEMBERINFO(GRIEVANCE_TYPE_ID,STAFF_ID,ROLE) VALUES(?GRIEVANCE_TYPE_ID,?STAFF_ID,?ROLE);";
                        break;
                    }
                case GrievanceSQLCommands.DeleteGrievancemembers:
                    {
                        query = @"UPDATE GRIEVANCE_MEMBERINFO 
                                                                SET 
                                                            IS_DELETED = 1
                                                        WHERE
                                                            G_MEMBER_ID =?G_MEMBER_ID;";
                        break;
                    }
                case GrievanceSQLCommands.UpdateUserAccountRole:
                    {
                        query = @"UPDATE USER_ACCOUNT 
                                SET 
                                    ROLE = CONCAT(?OLDROLE ,',',?ROLE)
                                WHERE
                                    USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                    }
                case GrievanceSQLCommands.DeleteUserAccountRole:
                    {
                        query = @"UPDATE USER_ACCOUNT 
                                    SET 
                                        ROLE =?ROLE
                                    WHERE
                                        USER_ACCOUNT_ID=?USER_ACCOUNT_ID;";
                        break;
                   }              
                case GrievanceSQLCommands.FetchUserAccountId:
                    {
                        query = @"SELECT USER_ACCOUNT_ID FROM USER_ACCOUNT  WHERE USER_ID=?USER_ID AND FIND_IN_SET('4',ROLE);";
                        break;
                    }
                case GrievanceSQLCommands.FetchUserAcId:
                    {
                        query = @"SELECT USER_ACCOUNT_ID FROM USER_ACCOUNT  WHERE USER_ID=?USER_ID AND FIND_IN_SET(?ROLE,ROLE)";
                        break;
                    }
                case GrievanceSQLCommands.FetchUserAccountForCheck:
                    {
                        query = @"SELECT USER_ACCOUNT_ID,USER_ID FROM USER_ACCOUNT WHERE USER_ID=?USER_ID AND ROLE=CONCAT(?OLDROLE,?ROLE);";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceandredrassal:
                    {
                        query = @" SELECT 
                                            CONCAT(IFNULL(SPI.FIRST_NAME,IFNULL(SPI.LAST_NAME,''))) AS SENDER_NAME,
                                            SPI.ROLL_NO,
                                            SPI.PHOTO AS STU_PHOTO,
                                            GI.GRIEVANCE_ID,
                                            GT.GRIEVANCE_TYPE_NAME,
                                            GI.GRIEVANCE_SUBJECT,
                                            GI.GRIEVANCE_CONTENT,
                                            DATE_FORMAT(GI.SUB_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS SUB_DATE_TIME,
                                            SP.PHOTO AS STAFF_PHOTO,
                                            SP.FIRST_NAME,
                                            GRI.REDRESSAL_CONTENT,
                                            DATE_FORMAT(GRI.REP_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS REPLY_DATE_TIME
                                        FROM
                                            GRIEVANCE_GRIEVANCEINFO AS GI
                                                LEFT JOIN
                                            GRIEVANCE_REDRESSALINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                                INNER JOIN
                                            GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = GI.USER_ID
                                                LEFT JOIN
                                            STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GI.ASSIGN_TO
                                                AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceandredrassalById:
                    {
                        query = @"SELECT 
                                        CONCAT(SPI.FIRST_NAME,ifnull(SPI.LAST_NAME,'')) AS SENDER_NAME,
                                        SPI.ROLL_NO,
                                        SPI.PHOTO AS STU_PHOTO,
                                        GI.GRIEVANCE_ID,
                                        GT.GRIEVANCE_TYPE_NAME,
                                        GI.GRIEVANCE_SUBJECT,
                                        GI.GRIEVANCE_CONTENT,
                                        DATE_FORMAT(GI.SUB_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS SUB_DATE_TIME,
                                        SP.FIRST_NAME,
                                        SP.PHOTO AS STAFF_PHOTO,
                                        GRI.REDRESSAL_CONTENT,
                                        DATE_FORMAT(GRI.REP_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS REPLY_DATE_TIME
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            LEFT JOIN
                                        GRIEVANCE_REDRESSALINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                       INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = GI.USER_ID
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GI.ASSIGN_TO
                                    WHERE
                                        GI.GRIEVANCE_TYPE_ID = ?GRIEVANCE_TYPE_ID AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceandredrassalByDate:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(SPI.FIRST_NAME,
                                                    IFNULL(SPI.LAST_NAME, ''))) AS SENDER_NAME,
                                    SPI.ROLL_NO,
                                    SPI.PHOTO AS STU_PHOTO,
                                    GI.GRIEVANCE_ID,
                                    GT.GRIEVANCE_TYPE_NAME,
                                    GI.GRIEVANCE_SUBJECT,
                                    GI.GRIEVANCE_CONTENT,
                                    DATE_FORMAT(GI.SUB_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS SUB_DATE_TIME,
                                    SP.FIRST_NAME,
                                    SP.PHOTO AS STAFF_PHOTO,
                                    GRI.REDRESSAL_CONTENT,
                                    DATE_FORMAT(GRI.REP_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS REPLY_DATE_TIME
                                FROM
                                    GRIEVANCE_GRIEVANCEINFO AS GI
                                        LEFT JOIN
                                    GRIEVANCE_REDRESSALINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = GI.USER_ID
                                        LEFT JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GI.ASSIGN_TO
                                WHERE
                                    GI.GRIEVANCE_TYPE_ID IN (?GRIEVANCE_TYPE_ID)
                                        AND DATE_FORMAT(GI.SUB_DATE_TIME,'%Y-%m-%d')  BETWEEN ?FROM_DATE AND ?TO_DATE
                                        AND GI.STATUS IN (?STATUS)
                                        AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievancecount:
                    {
                        query = @"SELECT 
                                        GT.GRIEVANCE_TYPE_NAME AS GRIEVANCE_TITLE,
                                        GI.GRIEVANCE_TYPE_ID,
                                        COUNT(1) AS GRIEVANCE_COUNT
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                    GROUP BY GRIEVANCE_TYPE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceMembersList:
                    {
                        query = @"SELECT 
                                    GMI.G_MEMBER_ID,
                                    GT.GRIEVANCE_TYPE_NAME,
                                    GMI.STAFF_ID,
                                    SP.FIRST_NAME,
                                    SR.ROLE_NAME,
                                    SP.ROLES
                                FROM
                                    GRIEVANCE_MEMBERINFO AS GMI
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GMI.STAFF_ID
                                        INNER JOIN
                                    SUP_ROLE AS SR ON SR.ROLE_ID = GMI.ROLE
                                        INNER JOIN
                                    GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GMI.GRIEVANCE_TYPE_ID AND GMI.IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceMembersListById:
                    {
                        query = @"SELECT 
                            GMI.G_MEMBER_ID,
                            GMI.GRIEVANCE_TYPE_ID,
                            GT.GRIEVANCE_TYPE_NAME,
                            GMI.STAFF_ID,
                            SP.FIRST_NAME,
                            GMI.ROLE,
                            SR.ROLE_NAME
                        FROM
                            GRIEVANCE_MEMBERINFO AS GMI
                                INNER JOIN
                            STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GMI.STAFF_ID
                                INNER JOIN
                            SUP_ROLE AS SR ON SR.ROLE_ID = GMI.ROLE
                                INNER JOIN
                            GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GMI.GRIEVANCE_TYPE_ID AND GMI.G_MEMBER_ID=?G_MEMBER_ID AND GMI.IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.UpdateGrievanceMembersList:
                    {
                        query = @"UPDATE GRIEVANCE_MEMBERINFO SET GRIEVANCE_TYPE_ID=?GRIEVANCE_TYPE_ID,ROLE=?ROLE WHERE G_MEMBER_ID=?G_MEMBER_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceMembers:
                    {
                        query = @"SELECT 
                                    GMI.STAFF_ID,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            '(',
                                            IFNULL(SP.STAFF_CODE, ''),
                                            ')') AS FIRST_NAME
                                FROM
                                    GRIEVANCE_MEMBERINFO AS GMI
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON GMI.STAFF_ID = SP.STAFF_ID
                                WHERE
                                    GMI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceMembersById:
                    {
                        query = @"SELECT 
                                        GMI.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '(',
                                                IFNULL(SP.STAFF_CODE, ''),
                                                ')') AS FIRST_NAME
                                    FROM
                                        GRIEVANCE_MEMBERINFO AS GMI
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON GMI.STAFF_ID = SP.STAFF_ID
                                    WHERE
                                        GMI.GRIEVANCE_TYPE_ID = ?GRIEVANCE_TYPE_ID
                                            AND GMI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievance:
                    {
                        query = @"SELECT 
                                                GI.GRIEVANCE_ID,
                                                CONCAT(IFNULL(SP.FIRST_NAME, IFNULL(SP.LAST_NAME, ''))) AS SENDER_NAME,
                                                GT.GRIEVANCE_TYPE_NAME,
                                                GI.GRIEVANCE_SUBJECT,
                                                GI.GRIEVANCE_CONTENT,
                                                DATE_FORMAT(GI.SUB_DATE_TIME, '%D/%M/%Y %H:%I:%S') AS SUB_DATE_TIME
                                            FROM
                                                GRIEVANCE_GRIEVANCEINFO AS GI
                                                    INNER JOIN
                                                GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                                    INNER JOIN
                                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = GI.USER_ID
                                            WHERE
                                                GI.GRIEVANCE_ID NOT IN (SELECT 
                                                        GR.GRIEVANCE_ID
                                                    FROM
                                                        GRIEVANCE_REDRESSALINFO AS GR)
                                                    AND GI.ASSIGN_TO = 0
                                                    AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceBytype:
                    {
                        query = @"SELECT 
                                            GI.GRIEVANCE_ID,
                                            GT.GRIEVANCE_TYPE_NAME,
                                            GI.GRIEVANCE_SUBJECT,
                                            GI.GRIEVANCE_CONTENT,
                                            DATE_FORMAT(GI.SUB_DATE_TIME, '%D/%M/%Y %H:%I:%S') AS SUB_DATE_TIME
                                        FROM
                                            GRIEVANCE_GRIEVANCEINFO AS GI
                                                INNER JOIN
                                            GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                        WHERE
                                            GI.GRIEVANCE_ID NOT IN (SELECT 
                                                    GR.GRIEVANCE_ID
                                                FROM
                                                    GRIEVANCE_REDRESSALINFO AS GR)
                                                AND GI.GRIEVANCE_TYPE_ID = ?GRIEVANCE_TYPE_ID
                                                AND GI.ASSIGN_TO = 0
                                                AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.SaveGrievanceAssign:
                    {
                        query = @"UPDATE GRIEVANCE_GRIEVANCEINFO SET ASSIGN_TO=?ASSIGN_TO WHERE GRIEVANCE_ID IN(?GRIEVANCE_ID)";
                        break;
                    }
                case GrievanceSQLCommands.DeleteAssignedGrievance:
                    {
                        query = @"UPDATE GRIEVANCE_GRIEVANCEINFO SET ASSIGN_TO=0 WHERE GRIEVANCE_ID IN(?GRIEVANCE_ID);";
                        break;
                    }
                case GrievanceSQLCommands.DeleteGrievance:
                    {
                        query = @"UPDATE GRIEVANCE_GRIEVANCEINFO SET IS_DELETED=1 WHERE GRIEVANCE_ID=?GRIEVANCE_ID;";
                        break;
                    }
                case GrievanceSQLCommands.FetchAssignedGrievance:
                    {
                        query = @"SELECT 
                                        GI.GRIEVANCE_ID,
                                        ST.ROLL_NO,
                                        CONCAT(IFNULL(ST.FIRST_NAME, IFNULL(ST.LAST_NAME, ''))) AS SENDER_NAME,
                                        GI.GRIEVANCE_TYPE_ID,
                                        GT.GRIEVANCE_TYPE_NAME,
                                        GI.GRIEVANCE_SUBJECT,
                                        GI.GRIEVANCE_CONTENT,
                                        DATE_FORMAT(GI.SUB_DATE_TIME, '%D/%M/%Y %H:%I:%S') AS SUB_DATE_TIME,
                                        SP.FIRST_NAME AS ASSIGN_TO
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GI.ASSIGN_TO
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS ST ON ST.STUDENT_ID = GI.USER_ID
                                    WHERE
                                        GI.GRIEVANCE_ID NOT IN (SELECT 
                                                GR.GRIEVANCE_ID
                                            FROM
                                                GRIEVANCE_REDRESSALINFO AS GR)
                                            AND GI.ASSIGN_TO != 0
                                            AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchAssignedGrievanceById:
                    {
                        query = @"SELECT 
                                        GI.GRIEVANCE_ID,
                                         GI.GRIEVANCE_TYPE_ID,
                                        GT.GRIEVANCE_TYPE_NAME,
                                        GI.GRIEVANCE_SUBJECT,
                                        GI.GRIEVANCE_CONTENT,
                                        DATE_FORMAT(GI.SUB_DATE_TIME,'%d/%m/%Y %H:%i:%s') AS SUB_DATE_TIME,
                                        GI.ASSIGN_TO
                                    FROM
                                        GRIEVANCE_GRIEVANCEINFO AS GI
                                            INNER JOIN
                                        GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                            INNER JOIN
                                        GRIEVANCE_REDRESSALINFO AS GR ON GI.GRIEVANCE_ID != GR.GRIEVANCE_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GI.ASSIGN_TO
                                            AND GI.GRIEVANCE_ID=?GRIEVANCE_ID AND GI.IS_DELETED!=1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchAttachmentByGrievanceId:
                    {
                        query = @"SELECT 
                                    GAI.ATTACHMENT_PATH, GAI.ATTACHMENT_NAME
                                FROM
                                    GRIEVANCE_ATTACHMENTINFO AS GAI
                                        INNER JOIN
                                    GRIEVANCE_GRIEVANCEINFO AS GI ON GI.GRIEVANCE_ID =GAI.GRIEVANCE_ID 
                                    AND GAI.GRIEVANCE_ID=?GRIEVANCE_ID
	                                AND GAI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchTotalMembersandNewGrievanceCount:
                    {
                       query = @"SELECT 
                                    (SELECT 
                                            COUNT(*)
                                        FROM
                                            GRIEVANCE_MEMBERINFO
                                        WHERE
                                            IS_DELETED != 1) AS TOTAL_MEMBERS,
                                    (SELECT 
                                            COUNT(*)
                                        FROM
                                            GRIEVANCE_GRIEVANCEINFO
                                        WHERE
                                            ASSIGN_TO = 0 AND IS_DELETED != 1) AS UN_ASSIGNED;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceBycategory:
                    {
                        query = @"SELECT 
                                            CONCAT(SPI.FIRST_NAME, '', SPI.LAST_NAME) AS SENDER_NAME,
                                            SPI.ROLL_NO,
                                            GI.GRIEVANCE_ID,
                                            CONCAT(IFNULL(ST.FIRST_NAME,IFNULL(ST.LAST_NAME,''))) AS SENDER_NAME,
                                            GT.GRIEVANCE_TYPE_NAME,
                                            GI.GRIEVANCE_SUBJECT,
                                            GI.GRIEVANCE_CONTENT,
                                            DATE_FORMAT(GI.SUB_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS SUB_DATE_TIME,
                                            SP.FIRST_NAME,
                                            GRI.REDRESSAL_CONTENT,
                                            DATE_FORMAT(GRI.REP_DATE_TIME, '%d/%m/%Y %H:%i:%s') AS REPLY_DATE_TIME
                                        FROM
                                            GRIEVANCE_GRIEVANCEINFO AS GI
                                                LEFT JOIN
                                            GRIEVANCE_REDRESSALINFO AS GRI ON GRI.GRIEVANCE_ID = GI.GRIEVANCE_ID
                                                INNER JOIN
                                            GRIEVANCE_TYPE AS GT ON GT.GRIEVANCE_TYPE_ID = GI.GRIEVANCE_TYPE_ID
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = GI.USER_ID
                                                LEFT JOIN
                                            STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = GI.ASSIGN_TO
                                            INNER JOIN  STU_PERSONAL_INFO AS ST ON ST.STUDENT_ID=GI.USER_ID 
                                        WHERE
                                            GI.STATUS = ?STATUS AND GI.IS_DELETED != 1;";
                        break;
                    }
                case GrievanceSQLCommands.FetchGrievanceRole:
                    {
                        query = @"SELECT ROLE_ID,ROLE_NAME FROM SUP_ROLE WHERE ROLE_NAME LIKE '%GRI%';";
                        break;
                    }

                case GrievanceSQLCommands.InsertUserAccountByAcademicYearForGrievance:
                    {
                        query= @"INSERT INTO USER_ROLES_BY_ACADEMIC_YEAR (USER_ID, ROLE_ID, USER_TYPE, ACADEMIC_YEAR) VALUES (?USER_ID, ?ROLE_ID, ?USER_TYPE, ?AC_YEAR);";
                        break;
                    }
                case GrievanceSQLCommands.CheckUserAccountByAcademicYearGrievanceRoleExist:
                    {
                        query = @"SELECT 
                                    USER_ID
                                FROM
                                    USER_ROLES_BY_ACADEMIC_YEAR
                                WHERE
                                    USER_ID = ?USER_ID AND ROLE_ID = 16
                                        AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }

            }
            return query;
        }
    }
}
