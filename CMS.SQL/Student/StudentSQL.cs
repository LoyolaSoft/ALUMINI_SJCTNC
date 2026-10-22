using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;

namespace CMS.SQL.Student
{
    public static class StudentSQL
    {
        public static string GetStudentSQL(StudentSQLCommads sEnumCommand)

        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case StudentSQLCommads.IfStudentExitsByMultiTable:
                    {
                        query = @"SELECT STUDENT_ID FROM ?TABLE_NAME WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.IsStudentExistingForTC:
                    {

                        query = @"SELECT COUNT(STUDENT_ID)>0 AS STATUS FROM  STU_TRANSFER_CERTIFICATE;";
                        break;
                    }
                case StudentSQLCommads.FetchAlliedCourseByStudentId:
                    {

                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CT.COURSE_CODE,
                                    CT.COURSE_TITLE,
                                    SC.CLASS_ID,
                                    CT.IS_ALLIED_SUBJECT
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    SEMESTER_MARKS_?AC_YEAR AS SM ON SM.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CT ON CT.COURSE_ROOT_ID = SM.COURSE_ID AND CT.IS_DELETED!=1  AND SM.IS_ARREAR != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_DELETED!=1
                                WHERE
                                    SC.STUDENT_ID IN (?STUDENT_ID) 
                                        AND SC.ACADEMIC_YEAR=?AC_YEAR
                                        AND CT.IS_ALLIED_SUBJECT = 1 
                                        AND SC.IS_DELETED!=1
                                ORDER BY SP.ROLL_NO,SEMESTER_ID,COURSE_CODE;";
                        break;
                    }
                case StudentSQLCommads.FetchAcademicYearByBatchAndStudentId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
	                                    Y.ACADEMIC_YEAR_ID,
	                                    Y.AC_YEAR,
                                        SB.BATCH,
	                                    S.SEMESTER, 
	                                    S.IS_ACTIVE
                                    FROM
                                        STU_PERSONAL_INFO AS SP
		                                    INNER JOIN
	                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                            INNER JOIN
                                        ACADEMIC_YEAR AS Y ON Y.ACADEMIC_YEAR_ID = AB.ACADEMIC_YEAR_ID
                                         --  AND Y.ACTIVE_YEAR = 1
                                            INNER JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = AB.BATCH_ID
		                                    INNER JOIN
	                                    ACADEMIC_SEMESTER AS S ON  S.PROGRAMME_GROUP_ID=PG.PROGRAMME_GROUP_ID 
		                                    AND S.BATCH=AB.BATCH_ID
		                                    AND S.ACADEMIC_YEAR=?AC_YEAR
                                            AND S.IS_DELETED!=1 
                                    WHERE  SP.STUDENT_ID IN (?STUDENT_ID)  AND S.IS_ACTIVE=1 GROUP BY SB.BATCH;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentListByStaffId:
                    {
                        query = @"SELECT 
                                STUDENT_ID
                            FROM
                                CP_CLASS_COURSE_STAFF_?AC_YEAR C
                                    INNER JOIN
                                STU_PERSONAL_INFO AS P ON P.CLASS_ID = C.CLASS_ID
                            WHERE
                                C.STAFF_ID = ?STAFF_ID AND P.IS_DELETED !=1 AND P.IS_LEFT !=1
                            GROUP BY P.STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.AcadamicIfExits:
                    {
                        query = @"SELECT ROLL_NO,STUDENT_ID FROM STU_PERSONAL_INFO WHERE ROLL_NO=?ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.DeleteCalender:
                    {
                        query = @"UPDATE CALENDER SET
                                  IS_DELETED=?IS_DELETED
                                  WHERE CALENDER_ID=?CALENDER_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateDayOrder:
                    {
                        query = @"UPDATE CALENDER SET DAY_ORDER=?DAY_ORDER , IS_HOLIDAY=?IS_HOLIDAY,EVENT=?EVENT WHERE DAY_ID=?DAY_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateCalender:
                    {
                        query = @"UPDATE CALENDAR_EVENTS_?AC_YEAR SET 
                                  EVENT_ID=?EVENT_ID,
                                  EVENT_NAME=?EVENT_NAME,
                                  EVENT_START_DATE=?EVENT_START_DATE,
                                  EVENT_END_DATE=?EVENT_END_DATE
                                WHERE EVENT_ID=?EVENT_ID;";
                        break;
                    }
                case StudentSQLCommads.InsertCalender:
                    {
                        query = @"INSERT INTO CALENDAR_EVENTS_?AC_YEAR(
                                DAY_ID,EVENT_START_DATE,EVENT_TYPE,
                                EVENT_NAME,EVENT_DESCRIPTION,EVENT_DEPARTMENT,
                                EVENT_END_DATE,STAFF_CATEGORY,ACCESS_VISIBILITY,
                                VIEW_MANAGE,USER_ID,RESPONSIBLE_STAFF,RESPONSIBLE_STUDENT,
                                EVENT_REPORT,EVENT_PARTICIPANTS,EVENT_STATUS,EVENT_BUDGET,
                                AMOUNT_SPENT,HOLIDAY_TYPE)
                                VALUES
                                (
                                   ?DAY_ID,?EVENT_START_DATE,?EVENT_TYPE,
                                   ?EVENT_NAME,?EVENT_DESCRIPTION,?EVENT_DEPARTMENT,
                                   ?EVENT_END_DATE,?STAFF_CATEGORY,?ACCESS_VISIBILITY,
                                   ?VIEW_MANAGE,?USER_ID,?RESPONSIBLE_STAFF,?RESPONSIBLE_STUDENT,
                                   ?EVENT_REPORT,?EVENT_PARTICIPANTS,?EVENT_STATUS,?EVENT_BUDGET,
                                   ?AMOUNT_SPENT,?HOLIDAY_TYPE
                                )";
                        break;
                    }
                case StudentSQLCommads.FetchCalenderEvent:
                    {
                        query = @"SELECT EVENT_ID,
                                CE.DAY_ID,
                                DATE_FORMAT(CE.EVENT_START_DATE,'%Y')  AS 'SYEAR', 
	                            DATE_FORMAT(CE.EVENT_START_DATE,'%m') AS 'SMONTH' , 
	                            CONCAT('-',DATE_FORMAT(CE.EVENT_START_DATE,'%d'))  AS 'SDAY',
                                CE.EVENT_TYPE,
                                CE.EVENT_NAME,
                                CE.EVENT_DESCRIPTION,
                                CE.EVENT_DEPARTMENT,
	                            DATE_FORMAT(CE.EVENT_END_DATE,'%Y')  AS 'EYEAR', 
	                            DATE_FORMAT(CE.EVENT_END_DATE,'%m') AS 'EMONTH' , 
	                            CONCAT('-',DATE_FORMAT(CE.EVENT_END_DATE,'%d'))  AS 'EDAY',
                                CE.USER_ID,
                                CE.RESPONSIBLE_STAFF,
                                CE.RESPONSIBLE_STUDENT,
                                CE.EVENT_REPORT,
                                CE.EVENT_PARTICIPANTS,
                                CE.EVENT_STATUS,
                                CE.EVENT_BUDGET,
                                CE.AMOUNT_SPENT,
                                CE.HOLIDAY_TYPE,
                                CONCAT('ORANGE')AS COLOR
                            FROM CALENDAR_EVENTS_?AC_YEAR CE
                             WHERE CE.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchCalender:
                    {
                        //            query = @"SELECT EVENT_ID,
                        //                  CE.DAY_ID,
                        //                    DATE_FORMAT(CE.EVENT_START_DATE,'%Y')  AS 'SYEAR', 
                        //                    DATE_FORMAT(CE.EVENT_START_DATE,'%m') AS 'SMONTH' , 
                        //                    CONCAT('-',DATE_FORMAT(CE.EVENT_START_DATE,'%d'))  AS 'SDAY',
                        //                    DATE_FORMAT(CE.EVENT_END_DATE,'%Y')  AS 'EYEAR', 
                        //                    DATE_FORMAT(CE.EVENT_END_DATE,'%m') AS 'EMONTH' , 
                        //                    CONCAT('-',DATE_FORMAT(CE.EVENT_END_DATE,'%d'))  AS 'EDAY',
                        //                    CE.RESPONSIBLE_STUDENT,
                        //                    CE.EVENT_NAME AS 'DAY_ORDER',
                        //                  '' DAY_ORDER_ID,
                        //                     '' IS_HOLIDAY,
                        //                    CONCAT('ORANGE') AS 'COLOR'
                        //                FROM CALENDAR_EVENTS_?AC_YEAR CE
                        //                WHERE CE.IS_DELETED!=1 UNION SELECT 
                        //                     '' AS EVENT_ID,
                        //                    C.DAY_ID AS DAY_ID,
                        //                    DATE_FORMAT(C.DAY_ORDER_DATE,'%Y')  AS 'SYEAR', 
                        //                    DATE_FORMAT(C.DAY_ORDER_DATE,'%m') AS 'SMONTH' , 
                        //                    CONCAT('-',DATE_FORMAT(C.DAY_ORDER_DATE,'%d'))  AS 'SDAY',
                        //                    DATE_FORMAT(C.DAY_ORDER_END_DATE,'%Y')  AS 'EYEAR', 
                        //                    DATE_FORMAT(C.DAY_ORDER_END_DATE,'%m') AS 'EMONTH' , 
                        //                    CONCAT('-',DATE_FORMAT(C.DAY_ORDER_END_DATE,'%d'))  AS 'EDAY',
                        //                    '' AS RESPONSIBLE_STUDENT,
                        //                     if(C.DAY_ORDER IS NULL || C.DAY_ORDER='' || C.DAY_ORDER='0','',CONCAT('DAY ORDER  :  ',C.DAY_ORDER))as 'DAY_ORDER',
                        //C.DAY_ORDER AS DAY_ORDER_ID,
                        //                        C.IS_HOLIDAY,
                        //                    CONCAT('GRAY') AS 'COLOR'
                        //                FROM CALENDER C  WHERE C.ACADEMIC_YEAR=?AC_YEAR;";
                        query = @"SELECT 
                                        CE.DAY_ID AS EVENT_ID,
                                        CE.DAY_ID,
                                        DATE_FORMAT(CE.DAY_ORDER_DATE, '%Y') AS 'SYEAR',
                                        DATE_FORMAT(CE.DAY_ORDER_DATE, '%m') AS 'SMONTH',
                                        CONCAT('-',
                                                DATE_FORMAT(CE.DAY_ORDER_DATE, '%d')) AS 'SDAY',
                                        DATE_FORMAT(CE.DAY_ORDER_END_DATE, '%Y') AS 'EYEAR',
                                        DATE_FORMAT(CE.DAY_ORDER_END_DATE, '%m') AS 'EMONTH',
                                        CONCAT('-',
                                                DATE_FORMAT(CE.DAY_ORDER_END_DATE, '%d')) AS 'EDAY',
                                        '' AS RESPONSIBLE_STUDENT,
                                        CE.EVENT AS 'DAY_ORDER',
                                        '' DAY_ORDER_ID,
                                        '' IS_HOLIDAY,
                                        CONCAT('ORANGE') AS 'COLOR'
                                    FROM
                                        CALENDER AS CE
                                    WHERE
                                        CE.ACADEMIC_YEAR = ?AC_YEAR 
                                    UNION SELECT 
                                        '' AS EVENT_ID,
                                        CAL.DAY_ID AS DAY_ID,
                                        DATE_FORMAT(CAL.DAY_ORDER_DATE, '%Y') AS 'SYEAR',
                                        DATE_FORMAT(CAL.DAY_ORDER_DATE, '%m') AS 'SMONTH',
                                        CONCAT('-',
                                                DATE_FORMAT(CAL.DAY_ORDER_DATE, '%d')) AS 'SDAY',
                                        DATE_FORMAT(CAL.DAY_ORDER_END_DATE, '%Y') AS 'EYEAR',
                                        DATE_FORMAT(CAL.DAY_ORDER_END_DATE, '%m') AS 'EMONTH',
                                        CONCAT('-',
                                                DATE_FORMAT(CAL.DAY_ORDER_END_DATE, '%d')) AS 'EDAY',
                                        '' AS RESPONSIBLE_STUDENT,
                                        IF(CAL.DAY_ORDER IS NULL
                                                || CAL.DAY_ORDER = ''
                                                || CAL.DAY_ORDER = '0',
                                            '',
                                            CONCAT('DAY ORDER  :  ', CAL.DAY_ORDER)) AS 'DAY_ORDER',
                                        CAL.DAY_ORDER AS DAY_ORDER_ID,
                                        CAL.IS_HOLIDAY,
                                        CONCAT('GRAY') AS 'COLOR'
                                    FROM
                                        CALENDER AS CAL
                                    WHERE
                                        CAL.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchDayIdByDayOrderDate:
                    {
                        query = @"SELECT 
                                    DAY_ID
                                FROM
                                    CALENDER
                                WHERE
                                    DAY_ORDER_DATE = ?DAY_ORDER_DATE
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchBonafideCertificate:
                    {
                        query = @"SELECT 
                                    BONAFIDE_ID, 
                                    UPPER(ROLL_NO) AS 'ROLL_NO', 
                                    UPPER(NAME)AS 'NAME', 
                                    UPPER(FATHER_NAME)AS 'FATHER_NAME', 
                                    UPPER(CLASS)AS 'CLASS', 
                                    UPPER(DURING_YEAR)AS 'DURING_YEAR',
	                                DATE_FORMAT(CURRENT_DATE(),'%d/%m/%Y')AS 'DATE'
                                FROM
                                    STU_BONAFIDE_CERTIFICATE WHERE BONAFIDE_ID=?BONAFIDE_ID;";
                        break;
                    }
                case StudentSQLCommads.InsertBonafideCertificate:
                    {
                        query = @"INSERT INTO STU_BONAFIDE_CERTIFICATE(
                                    ROLL_NO, 
                                    NAME, 
                                    FATHER_NAME, 
                                    CLASS, 
                                    DURING_YEAR,ACADEMIC_YEAR)
                                    VALUES(?ROLL_NO,?NAME,?FATHER_NAME,?CLASS,?DURING_YEAR,?AC_YEAR);";
                        break;
                    }
                case StudentSQLCommads.UpdateBonafideCertificate:
                    {
                        query = @"UPDATE STU_BONAFIDE_CERTIFICATE SET
	                                ROLL_NO =?ROLL_NO,
	                                NAME =?NAME,
	                                FATHER_NAME =?FATHER_NAME,
	                                CLASS =?CLASS,
	                                DURING_YEAR =?DURING_YEAR
                                WHERE BONAFIDE_ID =?BONAFIDE_ID;";
                        break;
                    }
                case StudentSQLCommads.CheckBonafideIsExits:
                    {
                        query = @"SELECT 
                                    BONAFIDE_ID
                                FROM
                                    STU_BONAFIDE_CERTIFICATE
                                WHERE
                                    ROLL_NO = ?ROLL_NO AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentBatchByStudentId:
                    {
                        query = @"SELECT 
                                    SB.BATCH
                                FROM
                                    SUP_BATCHES AS SB
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.BATCH = SB.BATCH_ID
                                WHERE
                                    STUDENT_ID IN(?STUDENT_ID);";
                        break;
                    }
                case StudentSQLCommads.FetchCourseCertificate:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                            ' ',
                                            IFNULL(UPPER(SP.LAST_NAME), '')) AS 'NAME',
                                    CONCAT(IFNULL((CASE
                                                        WHEN CLS.CLASS_YEAR = 1 THEN 'FIRST YEAR'
                                                        WHEN CLS.CLASS_YEAR = 2 THEN 'SECOND YEAR'
                                                        WHEN CLS.CLASS_YEAR = 3 THEN 'THIRD YEAR'
                                                        WHEN CLS.CLASS_YEAR = 4 THEN 'FOURTH YEAR'
                                                        WHEN CLS.CLASS_YEAR = 5 THEN 'FIVETH YEAR'
                                                    END),
                                                    ''),
                                            ' ',
                                            IFNULL(UPPER(CP.PROGRAMME_NAME), '')) AS CLASS,
	                                CONCAT(IFNULL(UPPER(SB.BATCH), ''),
                                            '-',
                                            ' ',
                                            IFNULL(AY.AC_YEAR+1, '')) AS 'Year',
                                    UPPER(DT.DEPARTMENT) AS 'PART1',
                                    CONCAT('ENGLISH') AS 'PART2',
                                    CDT.DEPARTMENT AS 'MAIN',
                                    DATE_FORMAT(CURRENT_DATE(), '%d/%m/%Y') AS 'DATE'
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR AND SP.IS_DELETED!=1 AND SP.IS_LEFT !=1
                                        INNER JOIN
                                    SEMESTER_MARKS_?BATCH AS SEM ON SEM.STUDENT_ID = SC.STUDENT_ID -- and SEM.ACADEMIC_YEAR=?BATCH
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SEM.COURSE_ID AND CR.IS_DELETED!=1
                                        INNER JOIN
                                    CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = CR.DEPARTMENT AND DT.IS_DELETED!=1
                                        INNER JOIN
                                    SUP_BATCHES AS SB ON SB.BATCH_ID = SP.BATCH
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID AND CLS.IS_DELETED!=1
										INNER JOIN
									CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID AND PG.IS_DELETED!=1
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID AND CP.IS_DELETED!=1
                                        INNER JOIN
                                    CP_DEPARTMENT AS CDT ON CDT.DEPARTMENT_ID = CLS.DEPARTMENT AND CDT.IS_DELETED!=1
                                        INNER JOIN
                                    ACADEMIC_YEAR AS AY ON AY.ACTIVE_YEAR = 1 AND AY.IS_DELETED!=1
                                WHERE
                                    SC.STUDENT_ID IN (?STUDENT_ID)
                                        AND CR.PAPER_TYPE = 1 AND SC.IS_DELETED!=1
                                GROUP BY SC.STUDENT_ID
                                ORDER BY SC.STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchConductCertificate:
                    {
                        query = @"SELECT 
                                   SC.STUDENT_ID,
                               SP.TC_SERIAL_NO,
                                   SP.ROLL_NO,
                               CONCAT(IFNULL(UPPER(SP.FIRST_NAME),''),' ',IFNULL(UPPER(SP.LAST_NAME),'')) AS 'FIRST_NAME',
                                   UPPER(CP.PROGRAMME_NAME)AS 'PROGRAMME_NAME',
                                   CONCAT('JUNE - ',LEFT(TC.ACADEMIC_YEARS , 4)) AS 'START_YEAR',
                               CONCAT('APRIL - ',RIGHT(TC.ACADEMIC_YEARS , 4)) AS 'END_YEAR',
                                   C.CONDUCT_NAME AS 'CONDUCT',
                                   DATE_FORMAT(TC.TC_GIVEN_DATE,'%d/%m/%Y')AS 'DATE',
                                   (SELECT PLACE FROM COLLEGE_DETAILS)AS 'PLACE',
                                    '~/Content/hcc_img/sign/Principal-E-Sign.png' AS PRINCIPAL_SIGN 
                               FROM
									STU_CLASS AS SC INNER JOIN 
                                   STU_PERSONAL_INFO SP ON SP.STUDENT_ID=SC.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                   INNER JOIN CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                   INNER JOIN CP_PROGRAMME AS CP ON CP.PROGRAMME_ID=PG.PROGRAMME_ID
                                   LEFT JOIN  SUP_CONDUCT AS C ON C.CONDUCT_ID=SP.CONDUCT
                                   LEFT JOIN STU_TRANSFER_CERTIFICATE AS TC ON TC.STUDENT_ID=SP.STUDENT_ID
                                   WHERE SC.STUDENT_ID IN(?STUDENT_ID) AND SC.ACADEMIC_YEAR=?AC_YEAR AND SP.IS_DELETED!=1 AND SC.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.BindStudentsGroupById:
                    {
                        //               query = @"SELECT 
                        //                           SP.ROLL_NO,
                        //                           SP.ADMISSION_NO,
                        //                           TC.CERTIFICATE_ID,
                        //                           TC.STUDENT_ID,
                        //                           UPPER(TC.FIRST_NAME) AS 'FIRST_NAME',
                        //                           UPPER(TC.GENDER) AS 'GENDER',
                        //                           CONCAT(UPPER(TC.NATIONALITY),
                        //                                   ' - ',
                        //                                   UPPER(TC.RELIGION)) AS 'NATIONALITY',
                        //                           CONCAT(UPPER(TC.CASTE),
                        //                                   ' - ',
                        //                                   UPPER(TC.COMMUNITY)) AS 'CASTE',
                        //                           CONCAT(DATE_FORMAT(TC.DATE_OF_BIRTH, '%d/%m/%Y'),
                        //                                   ' ( ',
                        //                                   UPPER(TC.DATETOWORDS),
                        //                                   ' ) ') AS 'DATE_OF_BIRTH',
                        //                           CONCAT(DATE_FORMAT(TC.ADMISSION_DATE, '%d/%m/%Y'),
                        //                                   ' - ',
                        //                                   UPPER(TC.ADMITTED_CLASS)) AS 'ADMITTED_DATE',
                        //                           CONCAT('1. ',' ',UPPER(TC.IDENTIFICATION_MARK1))  AS 'IDENTIFICATION_MARK1',
                        //                           CONCAT('2. ',' ',UPPER(TC.IDENTIFICATION_MARK2)) AS 'IDENTIFICATION_MARK2',
                        //                           UPPER(TC.MAIN_COURSE) AS 'MAIN_COURSE',
                        //                           UPPER(TC.ALLIED) AS 'ALLIED',
                        //                           UPPER(O.OPTION_NAME) AS 'FEE_PAID',
                        //                           UPPER(SO.OPTION_NAME) AS 'SCHOLAR_SHIP',
                        //                           DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                        //                           UPPER(TC.REASON_FOR_LEAVING) AS 'REASON_FOR_LEAVING',
                        //                           UPPER(TC.LEAVING_CLASS) AS 'LEAVING_CLASS',
                        //                           UPPER(SC.CONDUCT_NAME) AS 'CONDUCT',
                        //                           DATE_FORMAT(TC.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                        //                           DATE_FORMAT(TC.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE',
                        //                           IF(TC.FATHER_NAME IS NULL
                        //                                   || TC.FATHER_NAME = '',
                        //                               IF(TC.GUARDIAN_NAME IS NULL
                        //                                       || TC.GUARDIAN_NAME = '',
                        //                                   '----',
                        //                                   UPPER(TC.GUARDIAN_NAME)),
                        //                               UPPER(TC.FATHER_NAME)) AS 'FATHER_NAME',
                        //                           UPPER(TC.ACADEMIC_YEARS) AS 'ACADEMIC_YEARS',
                        //                           UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                        //                           UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE',
                        //                           CONTACT_NO,
                        //                           UPPER(ADDRESS) AS ADDRESS,
                        //                           UPPER(MEDIUM_OF_INSTRUCTION) AS MEDIUM_OF_INSTRUCTION,
                        //                           UPPER(STUDIED_FROM) AS STUDIED_FROM,
                        //                           '~/Content/hcc_img/sign/Principal-E-Sign.png' AS PRINCIPAL_SIGN 
                        //                       FROM
                        //                           STU_TRANSFER_CERTIFICATE TC
                        //                               INNER JOIN
                        //STU_CLASS AS S ON S.STUDENT_ID=TC.STUDENT_ID AND S.ACADEMIC_YEAR=?AC_YEAR
                        //	INNER JOIN
                        //                           STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = S.STUDENT_ID
                        //                               LEFT JOIN
                        //                           SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                        //                           LEFT JOIN 
                        //                           SUP_OPTION AS SO ON SO.OPTION_ID=TC.SCHOLAR_SHIP
                        //                            LEFT JOIN 
                        //                           SUP_CONDUCT AS SC ON SC.CONDUCT_ID=TC.CONDUCT

                        //                       WHERE
                        //                           TC.STUDENT_ID IN (?STUDENT_ID)";
                        //               break;
                        query = @"SELECT 
                                SP.ROLL_NO,
                                TC.ADMISSION_NO,
                                TC.SERIAL_NO,
                                TC.UMIS_NO,
                                TC.CERTIFICATE_ID,
                                TC.STUDENT_ID,
                                UPPER(TC.FIRST_NAME) AS 'FIRST_NAME',
                                UPPER(TC.GENDER) AS 'GENDER',
                                CONCAT(UPPER(TC.NATIONALITY),
                                        ' - ',
                                        UPPER(TC.RELIGION)) AS 'NATIONALITY',
		                            'REFER COMMUNITY CERTIFICATE' AS 'CASTE',
                                CONCAT(DATE_FORMAT(TC.DATE_OF_BIRTH, '%d/%m/%Y'),
                                        ' ( ',
                                        UPPER(TC.DATETOWORDS),
                                        ' ) ') AS 'DATE_OF_BIRTH',
                                CONCAT(DATE_FORMAT(TC.ADMISSION_DATE, '%d/%m/%Y'),
                                        ' - ',
                                        UPPER(TC.ADMITTED_CLASS)) AS 'ADMITTED_DATE',
                                CONCAT('1. ',
                                        ' ',
                                        UPPER(TC.IDENTIFICATION_MARK1)) AS 'IDENTIFICATION_MARK1',
                                CONCAT('2. ',
                                        ' ',
                                        UPPER(TC.IDENTIFICATION_MARK2)) AS 'IDENTIFICATION_MARK2',
                                UPPER(TC.MAIN_COURSE) AS 'MAIN_COURSE',
                                'REFER MARK STATEMENT' AS 'ALLIED',
                                UPPER(O.OPTION_NAME) AS 'FEE_PAID',
                                UPPER(SO.OPTION_NAME) AS 'SCHOLAR_SHIP',
                                DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                                UPPER(TC.REASON_FOR_LEAVING) AS 'REASON_FOR_LEAVING',
                                UPPER(TC.LEAVING_CLASS) AS 'LEAVING_CLASS',
                                UPPER(SC.CONDUCT_NAME) AS 'CONDUCT',
                                DATE_FORMAT(TC.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                                DATE_FORMAT(TC.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE',
                                IF(TC.FATHER_NAME IS NULL
                                        || TC.FATHER_NAME = '',
                                    IF(TC.GUARDIAN_NAME IS NULL
                                            || TC.GUARDIAN_NAME = '',
                                        '----',
                                        UPPER(TC.GUARDIAN_NAME)),
                                    UPPER(TC.FATHER_NAME)) AS 'FATHER_NAME',
                                UPPER(TC.ACADEMIC_YEARS) AS 'ACADEMIC_YEARS',
                                UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                                UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE',
                                CONTACT_NO,
                                UPPER(ADDRESS) AS ADDRESS,
                                UPPER(MEDIUM_OF_INSTRUCTION) AS MEDIUM_OF_INSTRUCTION,
                                UPPER(STUDIED_FROM) AS STUDIED_FROM,
                                '~/Content/hcc_img/sign/Principal-E-Sign.png' AS PRINCIPAL_SIGN
                            FROM
                                STU_TRANSFER_CERTIFICATE TC
                                    INNER JOIN
                                STU_CLASS AS S ON S.STUDENT_ID = TC.STUDENT_ID
                                    AND S.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = S.STUDENT_ID
                                    LEFT JOIN
                                SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                                    LEFT JOIN
                                SUP_OPTION AS SO ON SO.OPTION_ID = TC.SCHOLAR_SHIP
                                    LEFT JOIN
                                SUP_CONDUCT AS SC ON SC.CONDUCT_ID = TC.CONDUCT
                            WHERE
                                TC.STUDENT_ID IN (?STUDENT_ID);";
                        break;
                    }
                case StudentSQLCommads.BindStudentDetailsforTcPrint:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        UPPER(IFNULL(TC.ADMISSION_NO, SP.ROLL_NO)) AS ADMISSION_NO,
                                        TC.SERIAL_NO,
                                        (IFNULL(SP.UMIS_NO, TC.UMIS_NO)) AS UMIS_NO,
                                        TC.CERTIFICATE_ID,
                                        IFNULL(SP.STUDENT_ID, TC.STUDENT_ID) AS STUDENT_ID,
                                        UPPER(IFNULL(TC.FIRST_NAME,
                                                        CONCAT(SP.FIRST_NAME,
                                                                ' ',
                                                                IFNULL(SP.LAST_NAME, '')))) AS 'FIRST_NAME',
                                        UPPER(IFNULL(TC.GENDER, sg.GENDER_NAME)) AS 'GENDER',
                                        CONCAT(UPPER(IFNULL(IFNULL(TC.NATIONALITY,
                                                                        IFNULL(sn.NATIONALITY_NAME, 'INDIAN')),
                                                                ' ---- ')),
                                                ' , ',
                                                UPPER(IFNULL(IFNULL(TC.RELIGION, sr.RELIGION),
                                                                ' ---- '))) AS 'NATIONALITY',
                                        'Refer The Community Certificate' AS CASTE,
                                        DATE_FORMAT(IFNULL(TC.DATE_OF_BIRTH, sp.DATE_OF_BIRTH),
                                                '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                        CONCAT(DATE_FORMAT(IFNULL(TC.ADMISSION_DATE, sp.ADMISSION_DATE),
                                                        '%d/%m/%Y')) AS 'ADMITTED_DATE',
                                        DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                                        (IFNULL(TC.REASON_FOR_LEAVING, 'Course Completed')) AS 'REASON_FOR_LEAVING',
                                        IFNULL(TC.LEAVING_CLASS,
                                                IF(CC.SHIFT = 1,
                                                    REPLACE(CC.CLASS_NAME, ' DAY', ''),
                                                    REPLACE(CC.CLASS_NAME, ' EVE', ''))) AS 'LEAVING_CLASS',
                                        UPPER(IFNULL(SC.CONDUCT_NAME,'GOOD')) AS 'CONDUCT',
                                        IFNULL(IFNULL(IFNULL(TC.FATHER_NAME, sp.FATHER_NAME),
                                                        IFNULL(TC.GUARDIAN_NAME, sp.MOTHER_NAME)),
                                                '-----') AS FATHER_NAME,
                                        (IFNULL(TC.ACADEMIC_YEARS,
                                                CONCAT(CASE
                                                            WHEN CC.CLASS_YEAR = 1 THEN 'One Year'
                                                            WHEN CLASS_YEAR = 2 THEN 'Two Years'
                                                            WHEN CLASS_YEAR = 3 THEN 'Three Years'
                                                            WHEN CLASS_YEAR = 4 THEN 'Four Years'
                                                            ELSE 'UNKNOWN'
                                                        END,
                                                        ' (',
                                                        SB.BATCH,
                                                        '-',
                                                        SB.BATCH + CC.CLASS_YEAR,
                                                        ')'))) AS 'ACADEMIC_YEARS',
                                        UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                                        UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE',
                                        CONTACT_NO,
                                        UPPER(ADDRESS) AS ADDRESS,
                                        (IFNULL(TC.MEDIUM_OF_INSTRUCTION, 'English')) AS MEDIUM_OF_INSTRUCTION,
                                        'Refer The Marksheets' AS QUALIFIED_PROMOTION,
                                        '~/Content/hcc_img/sign/Principal-E-Sign.png' AS PRINCIPAL_SIGN
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS S ON S.STUDENT_ID = SP.STUDENT_ID
                                            AND S.ACADEMIC_YEAR = ?AC_YEAR
                                            LEFT JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = S.BATCH
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = S.CLASS_ID
                                            LEFT JOIN
                                        STU_TRANSFER_CERTIFICATE AS TC ON TC.STUDENT_ID = SP.STUDENT_ID
                                            LEFT JOIN
                                        SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                                            LEFT JOIN
                                        SUP_OPTION AS SO ON SO.OPTION_ID = TC.SCHOLAR_SHIP
                                            LEFT JOIN
                                        SUP_CONDUCT AS SC ON SC.CONDUCT_ID = TC.CONDUCT
                                            LEFT JOIN
                                        sup_gender AS sg ON sg.GENDER_ID = sp.gender
                                            LEFT JOIN
                                        sup_nationality AS sn ON sn.NATIONALITY_ID = sp.NATIONALITY
                                            LEFT JOIN
                                        sup_religion AS sr ON sr.RELIGIONID = sp.RELIGION
                                            LEFT JOIN
                                        sup_community AS scm ON scm.COMMUNITYID = sp.COMMUNITY
                                    WHERE
                                        sp.STUDENT_ID IN (?STUDENT_ID);";
                        break;
                    }
                case StudentSQLCommads.UpdatePersonalFromTC:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
                                 FIRST_NAME=?FIRST_NAME,
                                -- STUDENT_ID=?STUDENT_ID, 
                                 FATHER_NAME=?FATHER_NAME, 
                                 GUARDIAN_NAME=?GUARDIAN_NAME, 
                                 NATIONALITY=?NATIONALITY, 
                                 RELIGION=?RELIGION, 
                                 CASTE=?CASTE, 
                                 COMMUNITY=?COMMUNITY,
                                 GENDER=?GENDER, 
                                 DATE_OF_BIRTH=?DATE_OF_BIRTH, 
                                 IDENTIFICATION_MARK1=?IDENTIFICATION_MARK1,
                                 IDENTIFICATION_MARK2=?IDENTIFICATION_MARK2,
                                 ADMISSION_DATE=?ADMISSION_DATE,
                                -- ADMITTED_CLASS=?ADMITTED_CLASS,
                                 LEAVING_CLASS=?LEAVING_CLASS, 
                                 LEAVIN_GDATE=?LEAVIN_GDATE, 
                                 TC_APPLIED_DATE=?TC_APPLIED_DATE, 
                                 TC_GIVEN_DATE=?TC_GIVEN_DATE,
                                 CONDUCT=?CONDUCT
                                 WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindStudentDetailsForTC:
                    {
                        //query = @"SELECT 
                        //    SP.PROGRAM_ID,
                        //    SP.CLASS_ID,
                        //    SP.STUDENT_ID,
                        //    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //            ' ',
                        //            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                        //    UPPER(SP.FATHER_NAME) AS 'FATHER_NAME',
                        //    UPPER(SP.GUARDIAN_NAME) AS 'GUARDIAN_NAME',
                        //    SP.NATIONALITY,
                        //    SP.RELIGION,
                        //    SP.COMMUNITY,
                        //    SP.CASTE,
                        //    SP.GENDER,
                        //    CONCAT(IFNULL(sp.STU_MOBILENO,
                        //                    IFNULL(sp.FR_MOBILE, IFNULL(sp.MO_MOBILE, '')))) AS CONTACT_NO,
                        //    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                        //    SP.IDENTIFICATION_MARK1,
                        //    SP.IDENTIFICATION_MARK2,
                        //    DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS 'ADMISSION_DATE',
                        //    CONCAT(IF(C.CLASS_YEAR = '1',
                        //                'I',
                        //                IF(C.CLASS_YEAR = '2',
                        //                    'II',
                        //                    IF(C.CLASS_YEAR = '3',
                        //                        'III',
                        //                        IF(C.CLASS_YEAR = '4',
                        //                            'IV',
                        //                            C.CLASS_YEAR)))),
                        //            ' ',
                        //            IFNULL(P.PROGRAMME_DESCRIPTION, '')) AS CLASS_NAME,
                        //    D.DEPARTMENT,
                        //    CONCAT(IFNULL(AD.C_DOOR_DETAIL, ''),
                        //            ',',
                        //            IFNULL(AD.C_STREET, ''),
                        //            '(ST),',
                        //            IFNULL(AD.C_POST_PLACE_TOWN, ''),
                        //            '(PO),',
                        //            IFNULL(AD.C_TALUK, ''),
                        //            '(TK),',
                        //            IFNULL(AD.C_DISTRICT, ''),
                        //            '(DT) - ',
                        //            IFNULL(AD.C_PINCODE, '')) AS ADDRESS,
                        //    DATE_FORMAT(SP.LEAVIN_GDATE, '%d/%m/%Y') AS 'LEAVIN_GDATE',
                        //    DATE_FORMAT(SP.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                        //    DATE_FORMAT(SP.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE'
                        //FROM
                        //    STU_CLASS SC
                        //        INNER JOIN
                        //    STU_PERSONAL_INFO SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //        INNER JOIN
                        //    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                        //        INNER JOIN
                        //    CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                        //        INNER JOIN
                        //    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                        //        INNER JOIN
                        //    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = C.DEPARTMENT
                        //        LEFT JOIN
                        //    stu_address AS AD ON AD.STUDENT_ID = SP.STUDENT_ID
                        //WHERE
                        //    SC.STUDENT_ID = ?STUDENT_ID
                        //        AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        //query = @"SELECT 
                        //    SP.PROGRAM_ID,
                        //    SP.CLASS_ID,
                        //    SP.STUDENT_ID,
                        //    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //            ' ',
                        //            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                        //    UPPER(SP.FATHER_NAME) AS 'FATHER_NAME',
                        //    UPPER(SP.GUARDIAN_NAME) AS 'GUARDIAN_NAME',
                        //    SP.NATIONALITY,
                        //    SP.RELIGION,
                        //    SP.COMMUNITY,
                        //    SP.CASTE,
                        //    SP.GENDER,
                        //    CONCAT(IFNULL(sp.STU_MOBILENO,
                        //                    IFNULL(sp.FR_MOBILE, IFNULL(sp.MO_MOBILE, '')))) AS CONTACT_NO,
                        //    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                        //    SP.IDENTIFICATION_MARK1,
                        //    SP.IDENTIFICATION_MARK2,
                        //    DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS 'ADMISSION_DATE',
                        //    CONCAT(IF(C.CLASS_YEAR = '1',
                        //                'I',
                        //                IF(C.CLASS_YEAR = '2',
                        //                    'II',
                        //                    IF(C.CLASS_YEAR = '3',
                        //                        'III',
                        //                        IF(C.CLASS_YEAR = '4',
                        //                            'IV',
                        //                            C.CLASS_YEAR)))),
                        //            ' ',
                        //            IFNULL(P.PROGRAMME_DESCRIPTION, '')) AS CLASS_NAME,
                        //    D.DEPARTMENT,
                        //    AD.ADDRESS,
                        //    DATE_FORMAT(SP.LEAVIN_GDATE, '%d/%m/%Y') AS 'LEAVIN_GDATE',
                        //    DATE_FORMAT(SP.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                        //    DATE_FORMAT(SP.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE'
                        //FROM
                        //    STU_CLASS SC
                        //        INNER JOIN
                        //    STU_PERSONAL_INFO SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //        INNER JOIN
                        //    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                        //        INNER JOIN
                        //    CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                        //        INNER JOIN
                        //    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                        //        INNER JOIN
                        //    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = C.DEPARTMENT
                        //        LEFT JOIN
                        //    stu_address AS AD ON AD.STUDENT_ID = SP.STUDENT_ID
                        //WHERE
                        //    SC.STUDENT_ID = ?STUDENT_ID
                        //        AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        query = @"SELECT 
                                        sc.PROGRAMME_GROUP_ID,
                                        SP.CLASS_ID,
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                        UPPER(SP.FATHER_NAME) AS 'FATHER_NAME',
                                        UPPER(SP.MOTHER_NAME) AS 'MOTHER_NAME',
                                        sn.NATIONALITY_NAME AS NATIONALITY,
                                        sr.RELIGION AS RELIGION,
                                        scm.COMMUNITY AS COMMUNITY,
                                        SP.ROLL_NO,
                                        'ENGLISH' AS MEDIUM_STUDY,
                                        SP.UMIS_NO,
                                        SP.CASTE,
                                        sg.GENDER_NAME AS GENDER,
                                        CONCAT(IFNULL(sp.STU_MOBILENO,
                                                        IFNULL(sp.FR_MOBILE, IFNULL(sp.MO_MOBILE, '')))) AS CONTACT_NO,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                        SP.IDENTIFICATION_MARK1,
                                        SP.IDENTIFICATION_MARK2,
                                        DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS 'ADMISSION_DATE',
                                        C.CLASS_NAME AS CLASS_NAME,
                                        D.DEPARTMENT,
                                        DATE_FORMAT(SP.LEAVIN_GDATE, '%d/%m/%Y') AS 'LEAVIN_GDATE',
                                        DATE_FORMAT(SP.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                                        DATE_FORMAT(SP.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE',
                                        'Course Completed' AS 'REASON_FOR_LEAVING',
                                        AC.BATCH AS BEGINYEAR,
                                        'English' AS MEDIUM_OF_INSTRUCTION,
                                        'Refer The Marksheets' AS QUALIFIED_PROMOTION,
                                        C.CLASS_YEAR
                                    FROM
                                        STU_CLASS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            inner JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = C.DEPARTMENT
                                            LEFT JOIN
                                        STU_ADDRESS AS AD ON AD.STUDENT_ID = SP.STUDENT_ID
                                            LEFT JOIN
                                        sup_batches AS AC ON AC.BATCH_ID = SC.BATCH
                                            LEFT JOIN
                                        sup_gender AS sg ON sg.GENDER_ID = SP.GENDER
                                            LEFT JOIN
                                        sup_nationality AS sn ON sn.NATIONALITY_ID = sp.NATIONALITY
                                            LEFT JOIN
                                        sup_religion AS sr ON sr.RELIGIONID = sp.RELIGION
                                            LEFT JOIN
                                        sup_community AS scm ON scm.COMMUNITYID = sp.COMMUNITY
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;

                    }
                case StudentSQLCommads.StudentIfExits:
                    {
                        query = @"SELECT 
                                    STUDENT_ID, FIRST_NAME
                                FROM
                                    STU_TRANSFER_CERTIFICATE_?AC_YEAR WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchPriviousQualification:
                    {
                        query = @"SELECT 
                                    STUDENT_ID,
	                                LAST_SCHOOL_OR_COLLEGE,
                                    LAST_STUDIED_PLACE,
                                    LAST_STUDIED_CLASS,
                                    EXAM_PASSED
                                FROM STU_PERSONAL_INFO WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeByID:
                    {
                        query = @"SELECT 
                                    PROGRAMME_ID,
                                    PROGRAMME_NAME
                                FROM
                                    CP_PROGRAMME WHERE DEPARTMENT=?DEPARTMENT;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeByShiftId:
                    {
                        query = @"SELECT 
                                CP.PROGRAMME_ID,
                                CP.PROGRAMME_NAME
                            FROM
                               CP_CLASSES_?AC_YEAR CC
                                    INNER JOIN
                                CP_PROGRAMME_?AC_YEAR AS CP ON CP.PROGRAMME_ID = CC.PROGRAMME
                            WHERE
                                CC.SHIFT =?SHIFT_ID
                            GROUP BY CP.PROGRAMME_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchClassesByID:
                    {
                        query = @"SELECT 
                                    CLASS_ID,
                                    CLASS_NAME
                                FROM
                                    CP_CLASSES_?AC_YEAR
                                WHERE
                                    SHIFT = ?SHIFT;";
                        break;
                    }
                case StudentSQLCommads.FetchClass:
                    {
                        query = @"SELECT 
                                       CLASS_ID,CLASS_NAME
                                    FROM
                                        CP_CLASSES_?AC_YEAR WHERE PROGRAMME=?PROGRAMME AND SHIFT=?SHIFT;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentsForTC:
                    {
                        query = @"SELECT 
                                       SP.STUDENT_ID,
                                       CONCAT(UPPER(FIRST_NAME),' - ',UPPER(ROLL_NO)) AS 'FIRST_NAME',
                                       SC.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SP INNER JOIN  STU_CLASS AS SC ON SC.STUDENT_ID=SP.STUDENT_ID
                                        WHERE PROGRAM_ID=?PROGRAM_ID AND SC.CLASS_ID=?CLASS_ID ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchConduct:
                    {
                        query = @"SELECT 
                                   CONDUCT_ID, CONDUCT_NAME
                                FROM
                                    SUP_CONDUCT;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentList:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        ROLL_NO,
                                        REGISTER_NO,
                                        ADMISSION_NO,
                                        FIRST_NAME,
                                        D.DEPARTMENT,
                                        CC.CLASS_NAME,
                                        S.SHIFT_NAME,
                                        IF(SPI.IS_LEFT = 0, 'NO', 'YES') AS IS_LEFT,
                                        SPI.PHOTO
                                    FROM
                                        STU_PERSONAL_INFO SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            AND CC.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CC.DEPARTMENT
                                            AND D.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_SHIFT AS S ON S.SHIFT_ID = CC.SHIFT
                                    WHERE
                                        CC.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                    ORDER BY SPI.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchAcadamicInfo:
                    {
                        query = @"SELECT 
                                        SP.FIRST_NAME,
                                        SP.LAST_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        SP.UNIVERSITY_ROLLNO,
                                        DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        SP.ADMISSION_NO,
                                        SP.ADMITTED_CLASS,
                                        DATE_FORMAT(SP.STUDENT_REGISTERDED_DATE, '%d/%m/%Y') AS STUDENT_REGISTERDED_DATE,
                                        SP.LAST_SCHOOL_OR_COLLEGE,
                                        SP.LAST_STUDIED_PLACE,
                                        SP.LAST_STUDIED_CLASS,
                                        EXAM_PASSED,
                                        SP.REMARKS,
                                        SP.TC_SERIAL_NO,
                                        SC.CLASS_ID,
                                        PG.PROGRAMME_ID AS 'PROGRAM_ID',
                                        SP.BATCH,
                                        SP.CURRENT_BATCH,
                                        SP.SHIFT_ID,
                                        SP.STU_EMAILID,
                                        SP.IS_LEFT,
                                        SP.DEPT_ID,
                                        SP.LEFT_REASON,
                                        SP.PHOTO,
                                        PG.APPTYPE_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1 
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }

                case StudentSQLCommads.FetchParentsDetails:
                    {
                        query = @"SELECT
                                                FATHER_NAME,
                                                DATE_FORMAT(F_DATE_OF_BIRTH,'%d/%m/%Y') AS F_DATE_OF_BIRTH,
                                                MOTHER_NAME, 
                                                DATE_FORMAT(M_DATE_OF_BIRTH,'%d/%m/%Y') AS M_DATE_OF_BIRTH, 
                                                FR_BUSINESS_PHONE,
                                                FR_MOBILE,
                                                FR_PASSPORT_number, 
                                                MO_BUSINESS_PHONE,
                                                MO_MOBILE,
                                                MO_PASSPORT_number, 
                                                FR_INCOME,
                                                MO_INCOME,
                                                DATE_FORMAT(FR_DATE_OF_EXPIRY,'%d/%m/%Y') AS FR_DATE_OF_EXPIRY, 
                                                DATE_FORMAT(MO_DATE_OF_EXPIRY,'%d/%m/%Y') AS MO_DATE_OF_EXPIRY,
                                                FR_EMAIL,
                                                MO_EMAIL,
                                                MO_OCCUPATION,
                                                FR_OCCUPATION,
                                                FATHER_EDUCATION, 
                                                MOTHER_EDUCATION,
                                                FR_NATIONALITY,
                                                MO_NATIONALITY
                                            FROM STU_PERSONAL_INFO WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentActivity:
                    {
                        query = @"SELECT 
                                                ACTIVITY_ID,
                                                STUDENT_ID,
                                                POST_HELD,
                                                INITIATIVE_SHOWN,
                                                PARTICIPATION,
                                                DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                                DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                                EXTRA_ACTIVITY,
                                                POSITION_OBTAINED,
                                                ACTIVITY,
                                                ACTIVITY_IMAGE1,
                                                ACTIVITY_IMAGE2,
                                                ACTIVITY_IMAGE3
                                            FROM
                                                STU_ACTIVITIES WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case StudentSQLCommads.BindActivity:
                    {
                        query = @"SELECT 
                                   SA.ACTIVITY_ID,
                                   STUDENT_ID,
                                   POST_HELD,
                                   INITIATIVE_SHOWN,
                                   PARTICIPATION,
                                   DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                   DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                   EXTRA_ACTIVITY,
                                   POSITION_OBTAINED,
                                   A.ACTIVITY_NAME AS 'ACTIVITY',
                                   ACTIVITY_IMAGE1,
                                   ACTIVITY_IMAGE2,
                                   ACTIVITY_IMAGE3
                               FROM
                                   STU_ACTIVITIES SA 
                                   INNER JOIN sup_activity AS A ON A.ACTIVITY_ID=SA.ACTIVITY WHERE SA.IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteActivity:
                    {
                        query = @"UPDATE STU_ACTIVITIES SET IS_DELETED=1 WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchPersonl:
                    {
                        query = @"SELECT 
                                        FIRST_NAME,
                                        LAST_NAME,
                                        CASTE,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        BATCH,
                                        ANNUAL_INCOME,
                                        RESIDENCE_NO,
                                        PASSPORT_Number,
                                        VISA_SPONSOR,
                                        VISA_Number,
                                        IDENTIFICATION_MARK1,
                                        IDENTIFICATION_MARK2,
                                        HOME_PHONE,
                                        AADHAR_NUMBER,
                                        STU_MOBILENO,
                                        FR_MOBILE,
                                        PLACE_OF_BIRTH,
                                        MARITAL_STATUS,
                                        GENDER,
                                        COMMUNITY,
                                        MOTHER_TONGUE,
                                        BLOOD_GROUP,
                                        RELIGION,
                                        SPECIALLY_ABLED,
                                        FIRST_LANGUAGE,
                                        SECOND_LANGUAGE,
                                        RESIDENCY_TYPE,
                                        NATIONALITY,
                                        DISTRICT,
                                        SPECIALLY_ABLED,
                                        TAMIL_NAME,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        FR_OCCUPATION,
                                        MO_OCCUPATION,
                                        FR_INCOME,
                                        MO_INCOME,
                                        STATE_ID,
                                        CITY,
                                        PINCODE,
                                        ADDRESS_LINE1,
                                        HSS_NO,
                                        EX_SERVICE_MAN,
                                        HSS_GROUP_CODE,
                                        EMIS_NO,
                                        STUDY_BOARD,
                                        HSS_TOTAL,
                                        LAST_SCHOOL_OR_COLLEGE,
                                        MEDIUM_STUDY,
                                        EXTRA_CURRRICULAR,
                                        SPORTS,
                                        FIRSTGRADUATE,
                                        PART_1_LANGUAGE,
                                        LAST_STUDIED_UNIVERSITY,
                                        YEAR_OF_PASSING,
                                        PERCENTAGE,
                                        ALLIED_2,
                                        ALLIED_1,
                                        UMIS_NO,
                                        ENROLLMENT_NO,
                                        MAIN_SUBJECT,
                                        ACCOUNT_NO,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        IFSC_CODE
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchAddress:
                    {
                        query = @"SELECT 
                                              ADDRESS_ID,
                                              STUDENT_ID,
                                              C_DOOR_DETAIL,
                                              C_STREET,
                                              C_VILLAGE_AREA,
                                              C_POST_PLACE_TOWN,
                                              C_TALUK,C_CITY,
                                              C_DISTRICT,
                                              C_PINCODE,
                                              C_COUNTRY,
                                              P_DOOR_DETAIL,
                                              P_STREET,
                                              P_VILLAGE_AREA,
                                              P_POST_PLACE_TOWN,
                                              P_TALUK,
                                              P_CITY,
                                              P_DISTRICT,
                                              P_PINCODE,
                                              P_COUNTRY
                                            FROM STU_ADDRESS WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCertificate:
                    {
                        query = @"SELECT 
                                 CERTIFICATE_ID,
                                 STUDENT_ID,
                                 CERTIFICATE_NO,
                                 DATE_FORMAT(RECEIVED_DATE,'%d/%m/%Y') AS RECEIVED_DATE,
                                 DATE_FORMAT(GIVEN_DATE,'%d/%m/%Y') AS GIVEN_DATE, 
                                 SA.ACHIEVEMENT_NAME,
                                 PURPOSE,
                                 REGISTER_NUMBER,
                                 CERTIFICATE_NAME
                                 FROM STU_CERTIFICATE SC 
                                 INNER JOIN SUP_ACHIEVEMENT AS SA ON SA.ACHIEVEMENT_ID=SC.ARCHIVE 
                                 WHERE SC.IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindCertificate:
                    {
                        query = @"SELECT 
                                              CERTIFICATE_ID,
                                              STUDENT_ID,
                                              CERTIFICATE_NO,
                                              DATE_FORMAT(RECEIVED_DATE,'%d/%m/%Y') AS RECEIVED_DATE,
                                              DATE_FORMAT(GIVEN_DATE,'%d/%m/%Y') AS GIVEN_DATE, 
                                              ARCHIVE,
                                              PURPOSE,
                                              REGISTER_NUMBER,
                                              CERTIFICATE_NAME
                                        FROM STU_CERTIFICATE WHERE CERTIFICATE_ID=?CERTIFICATE_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteCertificate:
                    {
                        query = @"UPDATE STU_CERTIFICATE SET IS_DELETED=1 WHERE CERTIFICATE_ID=?CERTIFICATE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchAccountInfo:
                    {
                        query = @"SELECT 
                                                   ACCOUNT_NO,
                                                   IFSC_CODE,
                                                   MICR_CODE
                                                FROM STU_PERSONAL_INFO WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCounselling:
                    {
                        query = @"SELECT 
                                     SC_ID,
                                     DATE_FORMAT(DOC,'%d/%m/%Y') AS DOC, 
                                     DURATION,
                                     STUDENT_ID,
                                     REMARKS,
                                     COMMENTS,
                                     SB.BATCH,
                                     SC.IS_DELETED
                                 FROM
                                     STU_COUNSELLING SC 
                                     INNER JOIN SUP_BATCHES AS SB ON SB.BATCH_ID=SC.BATCH
                                     WHERE SC.IS_DELETED!=1 AND SC.STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindCounselling:
                    {
                        query = @"SELECT 
                                                SC_ID,
                                                DATE_FORMAT(DOC,'%d/%m/%Y') AS DOC, 
                                                DURATION,
                                                STUDENT_ID,
                                                REMARKS,
                                                COMMENTS,
                                                BATCH,
                                                IS_DELETED
                                            FROM
                                                STU_COUNSELLING WHERE SC_ID=?SC_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCourses:
                    {
                        query = @"SELECT 
                                                COURSE_REGISTRATION_ID,
                                                PROGRAM_ID,
                                                CLASS_ID,
                                                STUDENT_ID,
                                                PART,
                                                MAIN_SUBJECT,
                                                ALLIED1,
                                                ALLIED2,
                                                ALLIED3,
                                                ALLIED4,
                                                ELECTIVE1,
                                                ELECTIVE2,
                                                ELECTIVE3,
                                                ELECTIVE4,
                                                SPECIAL_SUBJECT
                                            FROM
                                                STU_COURSES WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchIncident:
                    {
                        query = @"SELECT 
                                     INCIDENT_ID,
                                     STUDENT_ID,
                                     DATE_FORMAT(DATE_OF_INCIDENT,'%d/%m/%Y') AS DATE_OF_INCIDENT,
                                     TIME_OF_INCIDENT,
                                     PLACE_OF_INCIDENT,
                                     FIRST_AID_DONE,
                                     SO.OPTION_NAME,
                                     DATE_FORMAT(DATE_INFORMED,'%d/%m/%Y') AS DATE_INFORMED,
                                     INCIDENT_DETAILS
                                 FROM
                                     STU_INCIDENT SI INNER JOIN SUP_OPTION AS SO ON SO.OPTION_ID=SI.INFORMED_TO_PARENTS WHERE SI.IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchSibling:
                    {
                        query = @"SELECT 
                                     SIBLING_ID,
                                     STUDENT_ID,
                                     NAME,
                                     AGE,
                                     INSTITUTE_NAME,
                                     P.PROGRAMME_NAME,
                                     S.IS_DELETED,
                                     QUALIFICATION,
                                     EMPLOYED,
                                     DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                     G.GENDER_NAME,
                                     O.OCCUPATION_NAME,
                                     MONTHLY_INCOME,
                                     PROGNAME,
                                     IS_SAME_COLLEGE
                                 FROM STU_SIBLING  S
                                 INNER JOIN CP_PROGRAMME_?AC_YEAR AS P ON P.PROGRAMME_ID=S.PROGRAM 
                                 INNER JOIN SUP_GENDER AS G ON G.GENDER_ID=S.GENDER
                                 INNER JOIN SUP_OCCUPATION AS O ON O.OCCUPATION_ID=S.OCCUPATION
                                 WHERE S.IS_DELETED!=1 AND S.STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindSibling:
                    {
                        query = @"SELECT 
                                                    SIBLING_ID,
                                                    STUDENT_ID,
                                                    NAME,
                                                    AGE,
                                                    INSTITUTE_NAME,
                                                    PROGRAM,
                                                    IS_DELETED,
                                                    QUALIFICATION,
                                                    EMPLOYED,
                                                    DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                                    GENDER,
                                                    OCCUPATION,
                                                    MONTHLY_INCOME,
                                                    PROGNAME,
                                                    IS_SAME_COLLEGE
                                                FROM STU_SIBLING WHERE SIBLING_ID=?SIBLING_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteSibling:
                    {
                        query = @"UPDATE STU_SIBLING  SET IS_DELETED=?IS_DELETED WHERE SIBLING_ID=?SIBLING_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchTalents:
                    {
                        query = @"SELECT TALENT_ID,
                                      STUDENT_ID,
                                      DATE_FORMAT(DATE,'%d/%m/%Y') AS DATE,
                                      L.ACTIVITY_LEVEL,
                                      A.ACTIVITY_TYPE,
                                      TALENT_DESCRIPTION,
                                      STATUS,
                                      G.OVERALL_GRADE,
                                      REMARKS,
                                      T.IS_DELETED
                                  FROM STU_TALENTS T
                                  INNER JOIN SUP_OVERALL_GRADE AS G ON G.OVERALL_GRADE_ID=T.GRADE
                                  INNER JOIN SUP_ACTIVITY_TYPE AS A ON A.ACTIVITY_TYPE_ID=T.TALENT_ACTIVITY_TYPE
                                  INNER JOIN SUP_ACTIVITY_LEVEL AS L ON L.ACTIVITY_LEVEL_ID=T.TALENT_AREA
                                  WHERE T.IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindTalents:
                    {
                        query = @"SELECT TALENT_ID,
                                                    STUDENT_ID,
                                                    DATE_FORMAT(DATE,'%d/%m/%Y') AS DATE,
                                                    TALENT_AREA,
                                                    TALENT_ACTIVITY_TYPE,
                                                    TALENT_DESCRIPTION,
                                                    STATUS,
                                                    GRADE,
                                                    REMARKS,
                                                    IS_DELETED
                                                FROM STU_TALENTS WHERE TALENT_ID=?TALENT_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteTalents:
                    {
                        query = @"UPDATE STU_TALENTS  SET IS_DELETED=?IS_DELETED WHERE TALENT_ID=?TALENT_ID ;";
                        break;
                    }
                case StudentSQLCommads.FetchClgHistory:
                    {
                        query = @"SELECT 
                                                    SCHOOL_ID, 
                                                    STUDENT_ID, 
                                                    AGE, 
                                                    SCHOOL_NAME, 
                                                    GRADE, 
                                                    IS_DELETED, 
                                                    DATE_FORMAT(ENTRY_DATE,'%d/%m/%Y') AS ENTRY_DATE, 
                                                    DATE_FORMAT(EXIT_DATE,'%d/%m/%Y') AS EXIT_DATE, 
                                                    ACADEMIC_LEVELS, 
                                                    CITY, 
                                                    COUNTRY, 
                                                    OFFICIAL_WEBSITE, 
                                                    CURRICULUM, 
                                                    REASON_FOR_WITHDRAWAL, 
                                                    DATE_FORMAT(LAST_ATTENDANCE_DATE,'%d/%m/%Y') AS LAST_ATTENDANCE_DATE 
                                                FROM
                                                    STU_COLLEGE_HISTORY;";
                        break;
                    }
                case StudentSQLCommads.FetchStuIssueEtc:
                    {
                        query = @"SELECT 
                                                    TC_ID, 
                                                    STUDENT_ID, 
                                                    TC_PRODUCED_NUMBER, 
                                                    DATE_FORMAT(TC_PRODUCE_DATE,'%d/%m/%Y') AS TC_PRODUCE_DATE, 
                                                    TC_ISSUED_NUMBER, 
                                                    DATE_FORMAT(TC_ISSUED_DATE,'%d/%m/%Y') AS TC_ISSUED_DATE, 
                                                    IS_DELETED, 
                                                    IS_ISSUED_TC, 
                                                    IS_DUPLICATED, 
                                                    SERIAL_NO, 
                                                    FLAG
                                                FROM
                                                    STU_ISSU_ETC WHERE IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindIssueEtc:
                    {
                        query = @"SELECT 
                                                    TC_ID, 
                                                    STUDENT_ID, 
                                                    TC_PRODUCED_NUMBER, 
                                                    DATE_FORMAT(TC_PRODUCE_DATE,'%d/%m/%Y') AS TC_PRODUCE_DATE, 
                                                    TC_ISSUED_NUMBER, 
                                                    DATE_FORMAT(TC_ISSUED_DATE,'%d/%m/%Y') AS TC_ISSUED_DATE, 
                                                    IS_DELETED, 
                                                    IS_ISSUED_TC, 
                                                    IS_DUPLICATED, 
                                                    SERIAL_NO, 
                                                    FLAG
                                                FROM
                                                    STU_ISSU_ETC WHERE TC_ID=?TC_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteIssueEtc:
                    {
                        query = @"UPDATE STU_ISSU_ETC  SET IS_DELETED=?IS_DELETED WHERE TC_ID=?TC_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStuAchievements:
                    {
                        query = @"SELECT 
                                                    ACHIEVE_ID, 
                                                    STUDENT_ID, 
                                                    ACHIEVEMENTS, 
                                                    IS_DELETED, 
                                                    CLASS_ID, 
                                                    DATE_FORMAT(DATE,'%d/%m/%Y')AS DATE, 
                                                    IMAGE_PATH, 
                                                    ACTIVITY
                                                FROM
                                                    STU_ACHIEVEMENTS WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchMedical:
                    {
                        query = @"SELECT 
	                                                MEDICAL_ID,
                                                    STUDENT_ID,
                                                    ALLERGIES,
                                                    DOCTOR_NOTE,
                                                    DATE_FORMAT(MEDICAL_DATE,'%d/%m/%Y')AS MEDICAL_DATE,
                                                    MEDICAL_CONDITION,
                                                    IS_DELETED,
                                                    VACCINATION
                                                FROM STU_MEDICAL WHERE IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindMedical:
                    {
                        query = @"SELECT 
	                                                MEDICAL_ID,
                                                    STUDENT_ID,
                                                    ALLERGIES,
                                                    DOCTOR_NOTE,
                                                    DATE_FORMAT(MEDICAL_DATE,'%d/%m/%Y')AS MEDICAL_DATE,
                                                    MEDICAL_CONDITION,
                                                    IS_DELETED,
                                                    VACCINATION
                                                FROM STU_MEDICAL WHERE MEDICAL_ID=?MEDICAL_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteMedical:
                    {
                        query = @"UPDATE STU_MEDICAL  SET IS_DELETED=?IS_DELETED WHERE MEDICAL_ID=?MEDICAL_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchLeaveLetter:
                    {
                        query = @"SELECT 
	                                                LEAVE_LETTER_ID,
                                                    STUDENT_ID,
                                                    LEAVE_LETTER_TITLE,
                                                    LEAVE_LETTER_FORMAT,
                                                    IS_ACTIVE,
                                                    IS_DELETED,
                                                    LETTER_FOR
                                                FROM STU_LEAVE_LETTER WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.InsertAcadamicInfo:
                    {
                        query = @"INSERT INTO STU_PERSONAL_INFO(
		                              REGISTER_NO,
                                     ROLL_NO,
                                     -- UNIVERSITY_ROLLNO,
                                     ADMISSION_DATE,
                                     ADMISSION_NO,
                                     ADMITTED_CLASS,
                                     -- STUDENT_REGISTERDED_DATE,
                                     REMARKS,
                                     -- TC_SERIAL_NO,
                                     CLASS_ID,
                                     PROGRAM_ID,
                                     BATCH,
                                     CURRENT_BATCH,
                                     DEPT_ID,
                                     SHIFT_ID,
                                     STU_EMAILID,
                                     FIRST_NAME,
                                     LAST_NAME,
                                     PHOTO,
                                     RFID,
                                     BAR_CODE,
                                     ENTRY_ID
                                    )
                                     VALUES
		                            (
		                              ?REGISTER_NO,
                                     ?ROLL_NO,
                                     -- ?UNIVERSITY_ROLLNO,
                                     ?ADMISSION_DATE,
                                     ?ADMISSION_NO,
                                     ?ADMITTED_CLASS,
                                     -- ?STUDENT_REGISTERDED_DATE,
                                     ?REMARKS,
                                     -- ?TC_SERIAL_NO,
                                     ?CLASS_ID,
                                     ?PROGRAM_ID,
                                     ?BATCH,
                                     ?CURRENT_BATCH,
                                     ?DEPT_ID,
                                     ?SHIFT_ID,
                                     ?STU_EMAILID,
                                     ?FIRST_NAME,
                                     ?LAST_NAME,
                                     ?PHOTO,
                                     ?ROLL_NO,
                                     ?ROLL_NO,
                                     ?ENTRY_ID
                                   );";
                        break;
                    }
                case StudentSQLCommads.InsertStudentAddressDetails:
                    {
                        query = @"INSERT INTO STU_ADDRESS(
                                                STUDENT_ID,C_DOOR_DETAIL,C_STREET,C_VILLAGE_AREA,C_POST_PLACE_TOWN,C_TALUK,C_CITY,C_DISTRICT,C_PINCODE,C_COUNTRY,
                                                P_DOOR_DETAIL,P_STREET,P_VILLAGE_AREA,P_POST_PLACE_TOWN,P_TALUK,P_CITY,P_DISTRICT,P_PINCODE,P_COUNTRY)
                                                VALUES
                                                (
                                                ?STUDENT_ID,?C_DOOR_DETAIL,?C_STREET,?C_VILLAGE_AREA,?C_POST_PLACE_TOWN,?C_TALUK,?C_CITY,?C_DISTRICT,?C_PINCODE,?C_COUNTRY,
                                                ?P_DOOR_DETAIL,?P_STREET,?P_VILLAGE_AREA,?P_POST_PLACE_TOWN,?P_TALUK,?P_CITY,?P_DISTRICT,?P_PINCODE,?P_COUNTRY
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertActivity:
                    {
                        query = @"INSERT INTO STU_ACTIVITIES(
                                                STUDENT_ID,
                                                INITIATIVE_SHOWN,
                                                PARTICIPATION,
                                                DATE_FROM,
                                                DATE_TO,
                                                EXTRA_ACTIVITY,
                                                ACTIVITY)
                                                VALUES
                                                (
                                                   ?STUDENT_ID,?INITIATIVE_SHOWN,?PARTICIPATION,?DATE_FROM,?DATE_TO,?EXTRA_ACTIVITY,?ACTIVITY
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertCdertificate:
                    {
                        query = @"INSERT INTO STU_CERTIFICATE(
                                                STUDENT_ID,
                                                CERTIFICATE_NO,
                                                RECEIVED_DATE,
                                                GIVEN_DATE,
                                                ARCHIVE,
                                                PURPOSE,
                                                REGISTER_NUMBER,
                                                CERTIFICATE_NAME)
                                                VALUES
                                                (
                                                   ?STUDENT_ID,?CERTIFICATE_NO,?RECEIVED_DATE,?GIVEN_DATE,?ARCHIVE,?PURPOSE,?REGISTER_NUMBER,?CERTIFICATE_NAME
                                                );";
                        break;
                    }
                ////case StudentSQLCommads.InsertParentsInfo:
                ////    { 
                ////        query = @"UPDATE STU_PERSONAL_INFO 
                ////                                SET
                ////                                    FATHER_NAME=?FATHER_NAME,
                ////                                    F_DATE_OF_BIRTH=?F_DATE_OF_BIRTH,
                ////                                    FR_OCCUPATION=?FR_OCCUPATION,
                ////                                    FATHER_EDUCATION=?FATHER_EDUCATION,
                ////                                    FR_NATIONALITY=?FR_NATIONALITY,
                ////                                    FR_BUSINESS_PHONE=?FR_BUSINESS_PHONE,
                ////                                    FR_MOBILE=?FR_MOBILE,
                ////                                    FR_INCOME=?FR_INCOME,
                ////                                    FR_DATE_OF_EXPIRY=?FR_DATE_OF_EXPIRY,
                ////                                    FR_EMAIL=?FR_EMAIL,
                ////                                    MOTHER_NAME=?MOTHER_NAME,
                ////                                    M_DATE_OF_BIRTH=?M_DATE_OF_BIRTH,
                ////                                    MO_OCCUPATION=?MO_OCCUPATION,
                ////                                    MOTHER_EDUCATION=?MOTHER_EDUCATION,
                ////                                    MO_NATIONALITY=?MO_NATIONALITY,
                ////                                    MO_BUSINESS_PHONE=?MO_BUSINESS_PHONE,
                ////                                    MO_MOBILE=?MO_MOBILE,
                ////                                    MO_INCOME=?MO_INCOME,
                ////                                    MO_DATE_OF_EXPIRY=?MO_DATE_OF_EXPIRY,
                ////                                    MO_EMAIL=?MO_EMAIL
                ////                                WHERE STUDENT_ID=?STUDENT_ID;";
                ////        break;
                ////    }
                case StudentSQLCommads.InsertPersonalInfo:
                    {
                        query = @"INSERT INTO STU_PERSONAL_INFO 
                                    (
                                    FIRST_NAME,LAST_NAME,CASTE,DATE_OF_BIRTH,ANNUAL_INCOME
                                    ,RESIDENCE_NO,IDENTIFICATION_MARK1,IDENTIFICATION_MARK2
                                    ,HOME_PHONE,AADHAR_NUMBER,STU_MOBILENO,PLACE_OF_BIRTH
                                    ,MARITAL_STATUS,GENDER,COMMUNITY,MOTHER_TONGUE
                                    ,BLOOD_GROUP,RELIGION,NATIONALITY,FIRST_LANGUAGE
                                    ,SECOND_LANGUAGE,SPECIALLY_ABLED,RESIDENCY_TYPE
                                    ) VALUES
                                    (
                                    ?FIRST_NAME,?LAST_NAME,?CASTE,?DATE_OF_BIRTH,?ANNUAL_INCOME
                                    ,?RESIDENCE_NO,?IDENTIFICATION_MARK1,?IDENTIFICATION_MARK2
                                    ,?HOME_PHONE,?AADHAR_NUMBER,?STU_MOBILENO,?PLACE_OF_BIRTH
                                    ,?MARITAL_STATUS,?GENDER,?COMMUNITY,?MOTHER_TONGUE
                                    ,?BLOOD_GROUP,?RELIGION,?NATIONALITY,?FIRST_LANGUAGE
                                    ,?SECOND_LANGUAGE,?SPECIALLY_ABLED,?RESIDENCY_TYPE
                                    );";
                        break;
                    }
                case StudentSQLCommads.InsertAccountInfo:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET 
	                                            ACCOUNT_NO=?ACCOUNT_NO,
                                                IFSC_CODE=?IFSC_CODE,
                                                MICR_CODE=?MICR_CODE
                                            WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.InsertCounseling:
                    {
                        query = @"INSERT INTO STU_COUNSELLING (
                                                DOC,
                                                DURATION,
                                                STUDENT_ID,
                                                REMARKS,
                                                COMMENTS,
                                                BATCH )
                                                VALUES 
                                                (
                                                   ?DOC,?DURATION,?STUDENT_ID,?REMARKS,?COMMENTS,?BATCH
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertCourses:
                    {
                        query = @"INSERT INTO STU_COURSES
                                                    (PROGRAM_ID,
                                                    STUDENT_ID,
                                                    CLASS_ID,
                                                    PART,
                                                    MAIN_SUBJECT,
                                                    ALLIED1,
                                                    ALLIED2,
                                                    ALLIED3,
                                                    ALLIED4,
                                                    ELECTIVE1,
                                                    ELECTIVE2,
                                                    ELECTIVE3,
                                                    ELECTIVE4,
                                                    SPECIAL_SUBJECT)
                                                 VALUES
                                                    (
                                                        ?PROGRAM_ID,?STUDENT_ID,?CLASS_ID,?PART,?MAIN_SUBJECT,?ALLIED1,?ALLIED2,?ALLIED3,?ALLIED4,
                                                        ?ELECTIVE1,?ELECTIVE2,?ELECTIVE3,?ELECTIVE4,?SPECIAL_SUBJECT
                                                    );";
                        break;
                    }
                case StudentSQLCommads.InsertIncident:
                    {
                        query = @"INSERT INTO STU_INCIDENT(
                                                STUDENT_ID,
                                                DATE_OF_INCIDENT,
                                                TIME_OF_INCIDENT,
                                                PLACE_OF_INCIDENT,
                                                FIRST_AID_DONE,
                                                INFORMED_TO_PARENTS,
                                                DATE_INFORMED,
                                                INCIDENT_DETAILS)
                                                VALUES
                                                (
                                                    ?STUDENT_ID,?DATE_OF_INCIDENT,?TIME_OF_INCIDENT,?PLACE_OF_INCIDENT,?FIRST_AID_DONE,?INFORMED_TO_PARENTS,?DATE_INFORMED,?INCIDENT_DETAILS
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertSibling:
                    {
                        query = @"INSERT INTO STU_SIBLING(
                                                STUDENT_ID,NAME,AGE,
                                                INSTITUTE_NAME,PROGRAM,
                                                QUALIFICATION,EMPLOYED,
                                                DATE_OF_BIRTH,GENDER,
                                                OCCUPATION,MONTHLY_INCOME,
                                                PROGNAME,IS_SAME_COLLEGE)
                                                VALUES
                                                (
                                                   ?STUDENT_ID,?NAME,?AGE,
                                                   ?INSTITUTE_NAME,?PROGRAM,
                                                   ?QUALIFICATION,?EMPLOYED,
                                                   ?DATE_OF_BIRTH,?GENDER,
                                                   ?OCCUPATION,?MONTHLY_INCOME,
                                                   ?PROGNAME,?IS_SAME_COLLEGE
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertTalents:
                    {
                        query = @"INSERT INTO STU_TALENTS(
                                                STUDENT_ID,DATE,TALENT_AREA,
                                                TALENT_ACTIVITY_TYPE,TALENT_DESCRIPTION,
                                                STATUS,GRADE,REMARKS)
                                                VALUES
                                                (
                                                   ?STUDENT_ID,?DATE,?TALENT_AREA,
                                                   ?TALENT_ACTIVITY_TYPE,?TALENT_DESCRIPTION,
                                                   ?STATUS,?GRADE,?REMARKS
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertClgHistory:
                    {
                        query = @"INSERT INTO STU_COLLEGE_HISTORY( 
                                                    STUDENT_ID, AGE, SCHOOL_NAME, 
                                                    GRADE, ENTRY_DATE, EXIT_DATE, 
                                                    ACADEMIC_LEVELS, CITY, COUNTRY, 
                                                    OFFICIAL_WEBSITE, CURRICULUM, 
                                                    REASON_FOR_WITHDRAWAL, LAST_ATTENDANCE_DATE)
                                                VALUES
                                                (
                                                    ?STUDENT_ID,?AGE,?SCHOOL_NAME,
                                                    ?GRADE,?ENTRY_DATE,?EXIT_DATE,
                                                    ?ACADEMIC_LEVELS,?CITY,?COUNTRY,
                                                    ?OFFICIAL_WEBSITE,?CURRICULUM,
                                                    ?REASON_FOR_WITHDRAWAL,?LAST_ATTENDANCE_DATE
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertStuIssuEtc:
                    {
                        query = @"INSERT INTO STU_ISSU_ETC( 
                                                    STUDENT_ID, 
                                                    TC_PRODUCED_NUMBER, 
                                                    TC_PRODUCE_DATE, 
                                                    TC_ISSUED_NUMBER, 
                                                    TC_ISSUED_DATE)
                                                VALUE 
                                                (
                                                   ?STUDENT_ID,?TC_PRODUCED_NUMBER,?TC_PRODUCE_DATE,?TC_ISSUED_NUMBER,?TC_ISSUED_DATE
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertStuAchievements:
                    {
                        query = @"INSERT INTO STU_ACHIEVEMENTS( 
	                                                STUDENT_ID, 
                                                    ACHIEVEMENTS,  
                                                    CLASS_ID, 
	                                                DATE, 
                                                    IMAGE_PATH, 
                                                    ACTIVITY)
                                                VALUES
                                                (
                                                   ?STUDENT_ID,?ACHIEVEMENTS,?CLASS_ID,?DATE,?IMAGE_PATH,?ACTIVITY
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertMedical:
                    {
                        query = @"INSERT INTO STU_MEDICAL(
                                                STUDENT_ID,ALLERGIES,
                                                DOCTOR_NOTE,MEDICAL_DATE,
                                                MEDICAL_CONDITION,VACCINATION)
                                                VALUES
                                                (
                                                    ?STUDENT_ID,?ALLERGIES,
                                                    ?DOCTOR_NOTE,?MEDICAL_DATE,
                                                    ?MEDICAL_CONDITION,?VACCINATION
                                                );";
                        break;
                    }
                case StudentSQLCommads.InsertLeaveLetter:
                    {
                        query = @"INSERT INTO STU_LEAVE_LETTER(
                                                        STUDENT_ID,
                                                        LEAVE_LETTER_TITLE,
                                                        LEAVE_LETTER_FORMAT,
                                                        LETTER_FOR)
                                                    VALUES
                                                    (
                                                        ?STUDENT_ID,?LEAVE_LETTER_TITLE,?LEAVE_LETTER_FORMAT,?LETTER_FOR 
                                                    );";
                        break;
                    }
                case StudentSQLCommads.InsertTransferCertificate:
                    {
                        query = @"INSERT INTO STU_TRANSFER_CERTIFICATE(
                                                                    STUDENT_ID,
                                                                    SERIAL_NO,
                                                                    ADMISSION_NO,
                                                                    UMIS_NO,
                                                                    FIRST_NAME,
                                                                    FATHER_NAME,
                                                                    GUARDIAN_NAME,
                                                                    NATIONALITY,
                                                                    RELIGION,
                                                                    CASTE,
                                                                    COMMUNITY,
                                                                    DATE_OF_BIRTH,
                                                                    GENDER,
                                                                    LEAVING_CLASS,
                                                                    ADMISSION_DATE,
                                                                    ACADEMIC_YEARS,
                                                                    LEAVING_DATE,
                                                                    REASON_FOR_LEAVING, 
                                                                    CONDUCT,
                                                                    MEDIUM_OF_INSTRUCTION,
                                                                    QUALIFIED_PROMOTION
                                                               )VALUES(
                                                                    ?STUDENT_ID,
                                                                    ?SERIAL_NO,
                                                                    ?ADMISSION_NO,
                                                                    ?UMIS_NO,
                                                                    ?FIRST_NAME,
                                                                    ?FATHER_NAME,
                                                                    ?GUARDIAN_NAME,
                                                                    ?NATIONALITY,
                                                                    ?RELIGION,
                                                                    ?CASTE,
                                                                    ?COMMUNITY,
                                                                    ?DATE_OF_BIRTH,
                                                                    ?GENDER,
                                                                    ?LEAVING_CLASS,
                                                                    ?ADMISSION_DATE,
                                                                    ?ACADEMIC_YEARS,
                                                                    ?LEAVING_DATE,
                                                                    ?REASON_FOR_LEAVING, 
                                                                    ?CONDUCT,
                                                                    ?MEDIUM_OF_INSTRUCTION,
                                                                    ?QUALIFIED_PROMOTION);";
                        break;
                    }

                case StudentSQLCommads.UpdateTransferCertificate:
                    {
                        query = @"UPDATE STU_TRANSFER_CERTIFICATE
                                    SET
                                    STUDENT_ID =?STUDENT_ID,
                                    UMIS_NO =?UMIS_NO,
                                    ADMISSION_NO =?ADMISSION_NO,
                                    SERIAL_NO =?SERIAL_NO,
                                    FIRST_NAME =?FIRST_NAME,
                                    GENDER =?GENDER,
                                    RELIGION =?RELIGION,
                                    NATIONALITY =?NATIONALITY,
                                    CASTE =?CASTE,
                                    COMMUNITY =?COMMUNITY,
                                    DATE_OF_BIRTH =?DATE_OF_BIRTH,
                                    ADMISSION_DATE =?ADMISSION_DATE,
                                    LEAVING_DATE =?LEAVING_DATE,
                                    LEAVING_CLASS =?LEAVING_CLASS,
                                    CONDUCT =?CONDUCT,
                                    FATHER_NAME =?FATHER_NAME,
                                    GUARDIAN_NAME =?GUARDIAN_NAME,
                                    ACADEMIC_YEARS =?ACADEMIC_YEARS,
                                    QUALIFIED_PROMOTION =?QUALIFIED_PROMOTION,
                                    MEDIUM_OF_INSTRUCTION =?MEDIUM_OF_INSTRUCTION,
                                    REASON_FOR_LEAVING =?REASON_FOR_LEAVING
                                WHERE 
                                    CERTIFICATE_ID =?CERTIFICATE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentsForEdit:
                    {
                        query = @"SELECT 
                                        TC.CERTIFICATE_ID,
                                        TC.SERIAL_NO,
                                        IFNULL(TC.ADMISSION_NO, SP.ROLL_NO) AS ADMISSION_NO,
                                        sp.STUDENT_ID,
                                        IFNULL(TC.FIRST_NAME, SP.FIRST_NAME) AS FIRST_NAME,
                                        IFNULL(TC.LAST_NAME, SP.LAST_NAME) AS LAST_NAME,
                                        IFNULL(TC.GENDER, SG.GENDER_NAME) AS GENDER,
                                        IFNULL(TC.RELIGION, SR.RELIGION) AS RELIGION,
                                        IFNULL(TC.NATIONALITY, SN.NATIONALITY_NAME) AS NATIONALITY,
                                        IFNULL(TC.CASTE, SP.CASTE) AS CASTE,
                                        IFNULL(TC.COMMUNITY, SCM.COMMUNITY) AS COMMUNITY,
                                        DATE_FORMAT(IFNULL(TC.DATE_OF_BIRTH, SP.DATE_OF_BIRTH),
                                                '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                        DATE_FORMAT(IFNULL(tc.ADMISSION_DATE, sp.ADMISSION_DATE),
                                                '%d/%m/%Y') AS ADMISSION_DATE,
                                        DATE_FORMAT(tc.LEAVING_DATE, '%d/%m/%Y') AS LEAVING_DATE,
                                       IFNULL(TC.LEAVING_CLASS,
                                                IF(CC.SHIFT = 1,
                                                    REPLACE(CC.CLASS_NAME, ' DAY', ''),
                                                    REPLACE(CC.CLASS_NAME, ' EVE', ''))) AS 'LEAVING_CLASS',
                                        tc.CONDUCT,
                                        DATE_FORMAT(tc.TC_APPLIED_DATE, '%d/%m/%Y') AS TC_APPLIED_DATE,
                                        DATE_FORMAT(tc.TC_GIVEN_DATE, '%d/%m/%Y') AS TC_GIVEN_DATE,
                                        IFNULL(TC.FATHER_NAME, SP.FATHER_NAME) AS FATHER_NAME,
                                        IFNULL(TC.GUARDIAN_NAME, SP.MOTHER_NAME) AS GUARDIAN_NAME,
                                       -- TC.ACADEMIC_YEARS,
                                        (IFNULL(TC.ACADEMIC_YEARS, CONCAT(CASE
                                                            WHEN CC.CLASS_YEAR = 1 THEN 'One Year'
                                                            WHEN CLASS_YEAR = 2 THEN 'Two Years'
                                                            WHEN CLASS_YEAR = 3 THEN 'Three Years'
                                                            WHEN CLASS_YEAR = 4 THEN 'Four Years'
                                                            ELSE 'UNKNOWN'
                                                        END,
                                                        ' (',
                                                        SB.BATCH,
                                                        '-',
                                                        SB.BATCH + CC.CLASS_YEAR,
                                                        ')'))) AS 'ACADEMIC_YEARS',
                                        TC.CLASSES_STUDEIED,
                                        TC.FIRST_LANGUAGE,
                                        TC.DATETOWORDS,
                                         UPPER(IFNULL(TC.CONDUCT,'1')) AS 'CONDUCT',
                                        TC.ADDRESS,
                                         (IFNULL(TC.REASON_FOR_LEAVING, 'Course Completed')) AS 'REASON_FOR_LEAVING',
                                        IFNULL(TC.UMIS_NO, SP.UMIS_NO) AS UMIS_NO,
                                     
										(IFNULL(TC.MEDIUM_OF_INSTRUCTION, 'English')) AS MEDIUM_OF_INSTRUCTION,
                                        'Refer The Marksheets' AS QUALIFIED_PROMOTION,
                                        TC.STUDIED_FROM
                                    FROM
                                       
                                        STU_PERSONAL_INFO AS SP 
                                            LEFT JOIN
                                         STU_TRANSFER_CERTIFICATE AS TC ON SP.STUDENT_ID = TC.STUDENT_ID
                                            LEFT JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            LEFT JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            LEFT JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = SC.BATCH
                                        LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SP.GENDER
                                            LEFT JOIN
                                        SUP_NATIONALITY AS SN ON SN.NATIONALITY_ID = SP.NATIONALITY
                                            LEFT JOIN
                                        SUP_RELIGION AS SR ON SR.RELIGIONID = SP.RELIGION
                                            LEFT JOIN
                                        SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SP.COMMUNITY
                                    WHERE
                                        SP.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateAcadamicInfo:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
                                    ROLL_NO=?ROLL_NO,
                                    FIRST_NAME=?FIRST_NAME,
                                    LAST_NAME=?LAST_NAME,
                                    ADMISSION_DATE=?ADMISSION_DATE,
                                    ADMISSION_NO=?ADMISSION_NO,
                                    REGISTER_NO = ?REGISTER_NO,
                                    ADMITTED_CLASS=?ADMITTED_CLASS,
                                    REMARKS=?REMARKS,
                                    CLASS_ID=?CLASS_ID,
                                    PROGRAM_ID=?PROGRAM_ID,
                                    BATCH=?BATCH,
                                    CURRENT_BATCH = ?CURRENT_BATCH,
                                    DEPT_ID=?DEPT_ID,
                                    SHIFT_ID=?SHIFT_ID,
                                    STU_EMAILID=?STU_EMAILID,
                                    RFID=?RFID,
                                    BAR_CODE=?BAR_CODE,
                                    IS_LEFT=?IS_LEFT
                                   
                                WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdatePersonalInfo:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
		                                            FIRST_NAME=?FIRST_NAME,
                                                    LAST_NAME=?LAST_NAME,
                                                    NAME_IN_NATIVE=?NAME_IN_NATIVE,
                                                    CASTE=?CASTE,
		                                            DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                                    ANNUAL_INCOME=?ANNUAL_INCOME,
                                                    RESIDENCE_NO=?RESIDENCE_NO,
                                                    IDENTIFICATION_MARK1=?IDENTIFICATION_MARK1,
                                                    IDENTIFICATION_MARK2=?IDENTIFICATION_MARK2,
                                                    HOME_PHONE=?HOME_PHONE,
                                                    AADHAR_NUMBER=?AADHAR_NUMBER,
                                                    STU_MOBILENO=?STU_MOBILENO,
                                                    PLACE_OF_BIRTH=?PLACE_OF_BIRTH,
                                                    MARITAL_STATUS=?MARITAL_STATUS,
                                                    GENDER=?GENDER,
                                                    COMMUNITY=?COMMUNITY,
                                                    MOTHER_TONGUE=?MOTHER_TONGUE,
                                                    BLOOD_GROUP=?BLOOD_GROUP,
		                                            RELIGION=?RELIGION,
                                                    SECOND_LANGUAGE=?SECOND_LANGUAGE,
                                                    FIRST_LANGUAGE=?FIRST_LANGUAGE,
                                                    RESIDENCY_TYPE=?RESIDENCY_TYPE,
                                                    SPECIALLY_ABLED=?SPECIALLY_ABLED,
                                                    NATIONALITY=?NATIONALITY,
                                                    ACCOUNT_NO=?ACCOUNT_NO,
                                                    BANK_NAME=?BANK_NAME,
                                                    BRANCH_NAME=?BRANCH_NAME,
                                                    IFSC_CODE=?IFSC_CODE
                                                 WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateParentsInfo:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO 
                                                SET
                                                    FATHER_NAME=?FATHER_NAME,
                                                    F_DATE_OF_BIRTH=?F_DATE_OF_BIRTH,
                                                    FR_OCCUPATION=?FR_OCCUPATION,
                                                    FATHER_EDUCATION=?FATHER_EDUCATION,
                                                    FR_NATIONALITY=?FR_NATIONALITY,
                                                    FR_BUSINESS_PHONE=?FR_BUSINESS_PHONE,
                                                    FR_MOBILE=?FR_MOBILE,
                                                    FR_INCOME=?FR_INCOME,
                                                    FR_DATE_OF_EXPIRY=?FR_DATE_OF_EXPIRY,
                                                    FR_EMAIL=?FR_EMAIL,
                                                    MOTHER_NAME=?MOTHER_NAME,
                                                    M_DATE_OF_BIRTH=?M_DATE_OF_BIRTH,
                                                    MO_OCCUPATION=?MO_OCCUPATION,
                                                    MOTHER_EDUCATION=?MOTHER_EDUCATION,
                                                    MO_NATIONALITY=?MO_NATIONALITY,
                                                    MO_BUSINESS_PHONE=?MO_BUSINESS_PHONE,
                                                    MO_MOBILE=?MO_MOBILE,
                                                    MO_INCOME=?MO_INCOME,
                                                    MO_DATE_OF_EXPIRY=?MO_DATE_OF_EXPIRY,
                                                    MO_EMAIL=?MO_EMAIL
                                                WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateAddress:
                    {
                        query = @"Update STU_ADDRESS SET
                                               C_DOOR_DETAIL=?C_DOOR_DETAIL,
                                               C_STREET=?C_STREET,
                                               C_VILLAGE_AREA=?C_VILLAGE_AREA,
                                               C_POST_PLACE_TOWN=?C_POST_PLACE_TOWN,
                                               C_TALUK=?C_TALUK,
                                               C_CITY=?C_CITY,
                                               C_DISTRICT=?C_DISTRICT,
                                               C_PINCODE=?C_PINCODE,
                                               C_COUNTRY=?C_COUNTRY,
                                               P_DOOR_DETAIL=?P_DOOR_DETAIL,
                                               P_STREET=?P_STREET,
                                               P_VILLAGE_AREA=?P_VILLAGE_AREA,
                                               P_POST_PLACE_TOWN=?P_POST_PLACE_TOWN,
                                               P_TALUK=?P_TALUK,
                                               P_CITY=?P_CITY,
                                               P_DISTRICT=?P_DISTRICT,
                                               P_PINCODE=?P_PINCODE,
                                               P_COUNTRY=?P_COUNTRY
                                            WHERE ADDRESS_ID=?ADDRESS_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateActivity:
                    {
                        query = @"UPDATE STU_ACTIVITIES SET
                                                  POST_HELD=?POST_HELD,
                                                  INITIATIVE_SHOWN=?INITIATIVE_SHOWN,
                                                  PARTICIPATION=?PARTICIPATION,
                                                  DATE_FROM=?DATE_FROM,
                                                  DATE_TO=?DATE_TO,
                                                  EXTRA_ACTIVITY=?EXTRA_ACTIVITY,
                                                  POSITION_OBTAINED=?POSITION_OBTAINED,
                                                  ACTIVITY=?ACTIVITY
                                                WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateCertificate:
                    {
                        query = @"UPDATE STU_CERTIFICATE SET
                                                    CERTIFICATE_NO=?CERTIFICATE_NO,
                                                    RECEIVED_DATE=?RECEIVED_DATE,
                                                    GIVEN_DATE=?GIVEN_DATE,
                                                    ARCHIVE=?ARCHIVE,
                                                    PURPOSE=?PURPOSE,
                                                    REGISTER_NUMBER=?REGISTER_NUMBER,
                                                    CERTIFICATE_NAME=?CERTIFICATE_NAME
                                                WHERE CERTIFICATE_ID=?CERTIFICATE_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateAccountInfo:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET 
	                                            ACCOUNT_NO=?ACCOUNT_NO,
                                                IFSC_CODE=?IFSC_CODE,
                                                MICR_CODE=?MICR_CODE
                                            WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateCounseling:
                    {
                        query = @"UPDATE STU_COUNSELLING
                                                SET
                                                DOC =?DOC,
                                                DURATION =?DURATION,
                                                REMARKS =?REMARKS,
                                                COMMENTS =?COMMENTS,
                                                BATCH =?BATCH,
                                                STUDENT_ID=?STUDENT_ID
                                                WHERE SC_ID =?SC_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteCounselling:
                    {
                        query = @"UPDATE STU_COUNSELLING  SET IS_DELETED=?IS_DELETED WHERE SC_ID=?SC_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateCourses:
                    {
                        query = @"UPDATE STU_COURSES SET
                                                    PROGRAM_ID=?PROGRAM_ID,
                                                    CLASS_ID=?CLASS_ID,
                                                    PART=?PART,
                                                    MAIN_SUBJECT=?MAIN_SUBJECT,
                                                    ALLIED1=?ALLIED1,
                                                    ALLIED2=?ALLIED2,
                                                    ALLIED3=?ALLIED3,
                                                    ALLIED4=?ALLIED4,
                                                    ELECTIVE1=?ELECTIVE1,
                                                    ELECTIVE2=?ELECTIVE2,
                                                    ELECTIVE3=?ELECTIVE3,
                                                    ELECTIVE4=?ELECTIVE4,
                                                    SPECIAL_SUBJECT=SPECIAL_SUBJECT
                                                WHERE COURSE_REGISTRATION_ID=?COURSE_REGISTRATION_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateIncident:
                    {
                        query = @"UPDATE STU_INCIDENT SET
                                                    STUDENT_ID=?STUDENT_ID,
                                                    DATE_OF_INCIDENT=?DATE_OF_INCIDENT,
                                                    TIME_OF_INCIDENT=?TIME_OF_INCIDENT,
                                                    PLACE_OF_INCIDENT=?PLACE_OF_INCIDENT,
                                                    FIRST_AID_DONE=?FIRST_AID_DONE,
                                                    INFORMED_TO_PARENTS=?INFORMED_TO_PARENTS,
                                                    DATE_INFORMED=?DATE_INFORMED,
                                                    INCIDENT_DETAILS=?INCIDENT_DETAILS
                                                WHERE INCIDENT_ID=?INCIDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.BindIncident:
                    {
                        query = @"SELECT 
                                                INCIDENT_ID,
                                                STUDENT_ID,
                                                DATE_FORMAT(DATE_OF_INCIDENT,'%d/%m/%Y') AS DATE_OF_INCIDENT,
                                                TIME_OF_INCIDENT,
                                                PLACE_OF_INCIDENT,
                                                FIRST_AID_DONE,
                                                INFORMED_TO_PARENTS,
                                                DATE_FORMAT(DATE_INFORMED,'%d/%m/%Y') AS DATE_INFORMED,
                                                INCIDENT_DETAILS
                                            FROM
                                                STU_INCIDENT WHERE INCIDENT_ID=?INCIDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteIncident:
                    {
                        query = @"UPDATE STU_INCIDENT SET IS_DELETED=?IS_DELETED WHERE INCIDENT_ID=?INCIDENT_ID; ";
                        break;
                    }
                case StudentSQLCommads.UpdateSibling:
                    {
                        query = @"UPDATE STU_SIBLING SET
                                                STUDENT_ID=?STUDENT_ID,
                                                NAME=?NAME,
                                                AGE=?AGE,
                                                INSTITUTE_NAME=?INSTITUTE_NAME,
                                                PROGRAM=?PROGRAM,
                                                QUALIFICATION=?QUALIFICATION,
                                                EMPLOYED=?EMPLOYED,
                                                DATE_OF_BIRTH=?DATE_OF_BIRTH,
                                                GENDER=?GENDER,
                                                OCCUPATION=?OCCUPATION,
                                                MONTHLY_INCOME=?MONTHLY_INCOME,
                                                PROGNAME=?PROGNAME,
                                                IS_SAME_COLLEGE=?IS_SAME_COLLEGE WHERE SIBLING_ID=?SIBLING_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateTalents:
                    {
                        query = @"UPDATE STU_TALENTS
                                                SET
                                                STUDENT_ID =?STUDENT_ID,
                                                DATE =?DATE,
                                                TALENT_AREA =?TALENT_AREA,
                                                TALENT_ACTIVITY_TYPE =?TALENT_ACTIVITY_TYPE,
                                                TALENT_DESCRIPTION =?TALENT_DESCRIPTION,
                                                STATUS =?STATUS,
                                                GRADE =?GRADE,
                                                REMARKS =?REMARKS
                                                WHERE TALENT_ID =?TALENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateClgHistory:
                    {
                        query = @"UPDATE STU_COLLEGE_HISTORY SET
                                                    AGE=?AGE, 
                                                    SCHOOL_NAME=?SCHOOL_NAME, 
                                                    GRADE=?GRADE, 
                                                    ENTRY_DATE=?ENTRY_DATE, 
                                                    EXIT_DATE=?EXIT_DATE, 
                                                    ACADEMIC_LEVELS=?ACADEMIC_LEVELS, 
                                                    CITY=?CITY, 
                                                    COUNTRY=?COUNTRY, 
                                                    OFFICIAL_WEBSITE=?OFFICIAL_WEBSITE, 
                                                    CURRICULUM=?CURRICULUM, 
                                                    REASON_FOR_WITHDRAWAL=?REASON_FOR_WITHDRAWAL, 
                                                    LAST_ATTENDANCE_DATE=?LAST_ATTENDANCE_DATE
                                                WHERE SCHOOL_ID=?SCHOOL_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuissueEtc:
                    {
                        query = @"UPDATE STU_ISSU_ETC SET
                                                    STUDENT_ID=?STUDENT_ID, 
                                                    TC_PRODUCED_NUMBER=?TC_PRODUCED_NUMBER, 
                                                    TC_PRODUCE_DATE=?TC_PRODUCE_DATE, 
                                                    TC_ISSUED_NUMBER=?TC_ISSUED_NUMBER, 
                                                    TC_ISSUED_DATE=?TC_ISSUED_DATE
                                                WHERE TC_ID=?TC_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuAchievements:
                    {
                        query = @"UPDATE STU_ACHIEVEMENTS SET
	                                                STUDENT_ID=?STUDENT_ID, 
                                                    ACHIEVEMENTS=?ACHIEVEMENTS,  
                                                    CLASS_ID=?CLASS_ID, 
	                                                DATE=?DATE, 
                                                    IMAGE_PATH=?IMAGE_PATH, 
                                                    ACTIVITY=?ACTIVITY
                                                WHERE ACHIEVE_ID=?ACHIEVE_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateMedical:
                    {
                        query = @"UPDATE STU_MEDICAL SET
	                                                STUDENT_ID=?STUDENT_ID,
	                                                ALLERGIES=?ALLERGIES,
	                                                DOCTOR_NOTE=?DOCTOR_NOTE,
	                                                MEDICAL_DATE=?MEDICAL_DATE,
	                                                MEDICAL_CONDITION=?MEDICAL_CONDITION,
	                                                VACCINATION=?VACCINATION
                                                WHERE MEDICAL_ID=?MEDICAL_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateLeaveLetter:
                    {
                        query = @"UPDATE STU_LEAVE_LETTER SET
                                                    STUDENT_ID=?STUDENT_ID,
                                                    LEAVE_LETTER_TITLE=?LEAVE_LETTER_TITLE,
                                                    LEAVE_LETTER_FORMAT=?LEAVE_LETTER_FORMAT,
                                                    LETTER_FOR=?LETTER_FOR
                                                WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdatePriviousQualification:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
	                                LAST_SCHOOL_OR_COLLEGE=?LAST_SCHOOL_OR_COLLEGE,
                                    LAST_STUDIED_PLACE=?LAST_STUDIED_PLACE,
                                    LAST_STUDIED_CLASS=?LAST_STUDIED_CLASS,
                                    EXAM_PASSED=?EXAM_PASSED
                                WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteStudent:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET IS_DELETED=?IS_DELETED ,REMARKS=?REMARKS WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentByClassId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        DT.DEPARTMENT,
                                        SP.ROLL_NO,
                                        SP.FIRST_NAME,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y')AS 'DATE_OF_BIRTH'
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = SP.DEPT_ID WHERE SC.CLASS_ID=?CLASS_ID AND SP.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchStuclassByStudentIdAndAcademicYear:
                    {
                        query = @"SELECT 
                                        STU_CLASS_ID
                                    FROM
                                        STU_CLASS
                                    WHERE
                                        ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND STUDENT_ID = ?STUDENT_ID AND IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuclass:
                    {
                        query = @"UPDATE STU_CLASS
                                       SET
                                        STUDENT_ID= ?STUDENT_ID,
                                        ACADEMIC_YEAR= ?ACADEMIC_YEAR,
                                        CLASS_ID= ?CLASS_ID,
                                        PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID,
                                        BATCH=?BATCH
                                        WHERE STU_CLASS_ID= ?STU_CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.SaveStuClass:
                    {
                        query = @"INSERT INTO STU_CLASS(
	                                STUDENT_ID,
                                    ACADEMIC_YEAR,
                                    CLASS_ID,
                                    PROGRAMME_GROUP_ID,
                                    BATCH
                                  )VALUES(
                                    ?STUDENT_ID,
                                    ?ACADEMIC_YEAR,
                                    ?CLASS_ID,
                                    ?PROGRAMME_GROUP_ID,
                                    ?BATCH
                                );";
                        break;
                    }
                case StudentSQLCommads.FetchClassesByProgrammeId:
                    {
                        query = @"SELECT 
                                        CLASS_ID, CLASS_CODE
                                    FROM
                                        CP_CLASSES_?AC_YEAR
                                    WHERE
                                        SHIFT = ?SHIFT AND PROGRAMME = ?PROGRAMME;";
                        break;
                    }
                case StudentSQLCommads.FetchStuClassByClassId:
                    {
                        query = @"SELECT 
                                    STU_CLASS_ID,
                                    SC.STUDENT_ID,
                                    SC.ACADEMIC_YEAR,
                                    SC.CLASS_ID,
                                    PROGRAMME_GROUP_ID,
                                    ROLL_NO,
                                    SP.BATCH
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                WHERE
                                        SC.CLASS_ID = ?CLASS_ID 
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR 
                                        and SP.IS_DELETED !=1
                                        AND SP.IS_LEFT !=1
                                        AND SC.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchStuCourseInfoByStudentIdAndAcademicYear:
                    {
                        query = @"SELECT 
                                        SC.COURSE_INFO_ID
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS AR ON AR.SEMESTER = SC.SEMESTER
                                            AND AR.IS_ACTIVE = 1  AND AR.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                    GROUP BY SC.COURSE_INFO_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentDepartment:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET DEPT_ID=?DEPARTMENT WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuCourseInfo:
                    {
                        query = @"UPDATE STU_COURSE_INFO
                                        SET 
                                            COURSE_ID = ?COURSE_ID,
                                            S_CLASS_ID=?S_CLASS_ID,
                                            STAFF_ID=?STAFF_ID
                                        WHERE
                                            COURSE_INFO_ID = ?COURSE_INFO_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCurrentProgrammeByStudentId:
                    {
                        query = @"SELECT 
                                        CLS.PROGRAMME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchTransferedProgrammeByStudentId:
                    {
                        query = @"SELECT 
                                        PROGRAMME
                                    FROM
                                        CP_CLASSES_?AC_YEAR
                                    WHERE
                                        CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentCIAMarks:
                    {
                        query = @"SELECT 
                                        STUDENT_ID
                                    FROM
                                        CIA_GROUP_MARKS_?AC_YEAR
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchShiftByClassId:
                    {
                        query = @"SELECT 
                                        SHIFT
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentPersonalInfoForStudentTransfer:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO
                                        SET
                                        PROGRAM_ID = ?PROGRAMME,
                                        SHIFT_ID = ?SHIFT
                                        WHERE STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuCourseInfoForStudentTransferOnlyLanguagePaper:
                    {
                        query = @"UPDATE STU_COURSE_INFO
                                        SET
                                        CLASS_ID = ?CLASS_ID,
                                        S_CLASS_ID = ?CLASS_ID
                                        WHERE 
                                        STUDENT_ID=?STUDENT_ID AND CLASS_ID=S_CLASS_ID AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchStuCourseInfobyClassId:
                    {
                        query = @"SELECT 
                                    SCI.COURSE_INFO_ID,
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.FIRST_NAME,
                                    SCI.COURSE_ID,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(CR.COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE,
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(STF.LAST_NAME,'')) AS STAFF_NAME
                                FROM
                                    STU_COURSE_INFO AS SCI
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SCI.STUDENT_ID
                                        AND SP.IS_DELETED!=1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SCI.COURSE_ID 
                                        AND CR.IS_DELETED!=1                                      
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS ASE ON ASE.PROGRAMME_GROUP_ID=SP.PROGRAM_ID 
                                        AND ASE.ACADEMIC_YEAR = SCI.ACADEMIC_YEAR
                                        AND ASE.IS_DELETED!=1
                                         LEFT JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SCI.STAFF_ID
                                        AND STF.IS_DELETED!=1
                                WHERE
                                    SCI.S_CLASS_ID = ?CLASS_ID
                                        AND ASE.IS_ACTIVE = 1
                                        AND ASE.SEMESTER = CR.SEMESTER_ID
                                        AND SCI.IS_DELETED != 1
                                        AND SCI.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY SCI.STUDENT_ID , SCI.COURSE_ID
                                ORDER BY SCI.S_CLASS_ID,SCI.COURSE_ID ,SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchCourseListForStuCourseInfo:
                    {
                        query = @"SELECT 
                            COURSE_ROOT_ID,
                            CONCAT(IFNULL(COURSE_CODE, ''),
                                    '(',
                                    IFNULL(COURSE_TITLE, ''),
                                    ')') AS COURSE_TITLE
                        FROM
                            CP_COURSE_ROOT
                        WHERE
                            IS_COMPULSORY != 1
                                AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.BindStudentPersonalInfoByClassId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.FIRST_NAME,
                                        SP.ROLL_NO
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID  AND SC.ACADEMIC_YEAR=?AC_YEAR  AND SC.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.BindSelectedStudentsByCourseId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.FIRST_NAME,
                                        SP.ROLL_NO
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                    WHERE
                                        SC.S_CLASS_ID = ?S_CLASS_ID 
                                        AND SC.COURSE_ID=?COURSE_ID 
                                        AND SC.CLASS_ID=?CLASS_ID 
                                        AND SC.IS_DELETED!=1 
                                        AND SP.IS_DELETED!=1 
                                        AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchSelectedClassIdByCourseIdAndShiftId:
                    {
                        query = @"SELECT 
                                        CLASS_ID
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        SHIFT = ?SHIFT AND PROGRAMME_MODE = ?PROGRAMME_MODE AND COURSE_ID = ?COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentCourseByStudentId:
                    {
                        query = @"SELECT 
                                        SC.COURSE_ID, SC.COURSE_INFO_ID
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                            AND CR.IS_NME_SUBJECT = 1
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS ASE ON ASE.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            AND ASE.SEMESTER = CR.SEMESTER_ID
                                            AND ASE.IS_ACTIVE = 1
                                            AND ASE.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY SC.COURSE_ID ;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentCourseByCourseIdAndStudentId:
                    {
                        query = @"SELECT 
                                        S_CLASS_ID
                                    FROM
                                        STU_COURSE_INFO
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND CLASS_ID = ?CLASS_ID
                                            AND COURSE_ID = ?COURSE_ID AND IS_DELETED!=1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentCIAMarksByStudentIdAndCourseId:
                    {
                        query = @"SELECT 
                                        STUDENT_ID
                                    FROM
                                        CIA_GROUP_MARKS_?AC_YEAR
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND COURSE_ID=?COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeAndBatchByClassId:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_ID AS  PROGRAMME_GROUP_ID,
                                        SP.BATCH
                                    FROM
                                        CP_CLASSES AS CLS
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.CLASS_ID = CLS.CLASS_ID
                                            INNER JOIN
										CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
											INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        CLS.CLASS_ID = ?CLASS_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY SC.CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCourseByProgrammeAndBatch:
                    {


                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                '(',
                                                IFNULL(CR.COURSE_CODE, ''),
                                                ')') AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_PATTERN AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
		                                    INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
		                                    INNER JOIN
	                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR=CLS.CLASS_YEAR	
		                                    AND AB.ACADEMIC_YEAR_ID =(SELECT ACADEMIC_YEAR_ID FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1)
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS ASE ON ASE.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
		                                    AND ASE.SEMESTER=CR.SEMESTER_ID
		                                    AND ASE.BATCH=AB.BATCH_ID
                                            AND ASE.IS_DELETED!=1
                                            AND ASE.ACADEMIC_YEAR=CP.ACADEMIC_YEAR
                                    WHERE
                                        CLS.CLASS_ID = ?CLASS_ID
                                            AND CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ASE.IS_ACTIVE = 1
                                    GROUP BY CP.COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchClassCourseByCourseIdAndClassId:
                    {
                        query = @"SELECT 
                                        CLASS_COURSE_ID
                                    FROM
                                        CP_CLASS_COURSE_?AC_YEAR AS CC
                                    WHERE
                                        COURSE_ID = ?COURSE_ID AND CLASS_ID = ?CLASS_ID
                                            AND IS_DELETED != 1";
                        break;
                    }
                case StudentSQLCommads.CheckIsNMECourse:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        IS_NME_SUBJECT = 1
                                            AND COURSE_ROOT_ID = ?COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchLanguagePaperByStudentId:
                    {
                        query = @"SELECT 
                                        SC.COURSE_ID, SC.COURSE_INFO_ID
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
	                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
		                                    AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS ASE ON ASE.PROGRAMME_GROUP_ID=SP.PROGRAM_ID AND ASE.SEMESTER = CR.SEMESTER_ID
                                            AND ASE.IS_ACTIVE = 1
                                            AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.CLASS_ID = SC.S_CLASS_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.COURSE_ID = ?COURSE_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY SC.COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuCourseInfoOnlyLanguagePaper:
                    {
                        query = @"UPDATE STU_COURSE_INFO
                                        SET 
                                            COURSE_ID = ?COURSE_ID
                                        WHERE
                                            COURSE_INFO_ID = ?COURSE_INFO_ID;";
                        break;
                    }
                case StudentSQLCommads.SaveStuCourseInfo:
                    {
                        query = @"INSERT INTO STU_COURSE_INFO
                                  (
                                      COURSE_ID,
                                      CLASS_ID,
                                      STUDENT_ID,
                                      STAFF_ID,
                                      S_CLASS_ID,
                                      ACADEMIC_YEAR,
                                      SEMESTER
                                  )
                                  VALUES
                                  (
                                      ?COURSE_ID,
                                      ?CLASS_ID,
                                      ?STUDENT_ID,
                                      ?STAFF_ID,
                                      ?S_CLASS_ID,
                                      ?AC_YEAR,
                                      ?SEMESTER
                                  );";
                        break;
                    }
                case StudentSQLCommads.DeleteStuCourseInfo:
                    {
                        query = @"UPDATE STU_COURSE_INFO SET IS_DELETED=1 WHERE COURSE_INFO_ID=?COURSE_INFO_ID";
                        break;
                    }
                case StudentSQLCommads.CheckClassCourse:
                    {
                        query = @"SELECT 
                                        COURSE_ID
                                    FROM
                                        CP_CLASS_COURSE
                                    WHERE
                                        CLASS_ID = ?CLASS_ID
                                            AND COURSE_ID = ?COURSE_ID
                                            AND IS_DELETED!=1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.SaveAssignmentSubmission:
                    {
                        query = @"INSERT INTO ASSIGNMENT_SUBMISSION_?AC_YEAR
                                            (
                                            STUDENT_ID,
                                            CLASS_ID,
                                            ASSIGNMENT_ID,
                                            SUBMITTED_DATE,
                                            FILE_PATH)
                                            VALUES
                                            (
                                            ?STUDENT_ID,
                                            ?CLASS_ID,
                                            ?ASSIGNMENT_ID,
                                            ?SUBMITTED_DATE,
                                            ?FILE_PATH);";
                        break;
                    }
                case StudentSQLCommads.FetchAssignmentSubmissionByAssignmentIdAndStudentId:
                    {
                        query = @"SELECT 
                                        ASS_SUBMISSION_ID
                                    FROM
                                        ASSIGNMENT_SUBMISSION_?AC_YEAR
                                    WHERE
                                        ASSIGNMENT_ID = ?ASSIGNMENT_ID AND STUDENT_ID = ?STUDENT_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.UpdateAssignmentSubmission:
                    {
                        query = @"UPDATE ASSIGNMENT_SUBMISSION_?AC_YEAR
                                        SET
                                        STUDENT_ID= ?STUDENT_ID,
                                        CLASS_ID= ?CLASS_ID,
                                        ASSIGNMENT_ID= ?ASSIGNMENT_ID,
                                        SUBMITTED_DATE= ?SUBMITTED_DATE,
                                        FILE_PATH= ?FILE_PATH
                                        WHERE ASS_SUBMISSION_ID= ?ASS_SUBMISSION_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentInfo:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        ROLL_NO,
                                        REGISTER_NO,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeGroupByShiftId:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_GROUP_ID AS PROGRAMME_ID,
                                        CONCAT(IFNULL(CP.PROGRAMME_NAME, ''),
                                                ' (',
                                                IFNULL(CONCAT(SPM.PROGRAMME_MODE), ''),
                                                ')',
                                                '-',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = PG.SHIFT
                                    WHERE
                                        PG.SHIFT = ?SHIFT_ID
                                    GROUP BY PG.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeGroupByShiftIdAndProgrammeLevel:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_GROUP_ID AS PROGRAMME_ID,
                                        CONCAT(IFNULL(CP.PROGRAMME_NAME, ''),
                                                ' (',
                                                IFNULL(CONCAT(SPM.PROGRAMME_MODE), ''),
                                                ')',
                                                '-',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = PG.SHIFT
                                    WHERE
                                        PG.SHIFT = ?SHIFT and APPTYPE_ID=?PROGRAMME_LEVEL
                                    GROUP BY PG.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeGroupByShiftIdMAndProgrammeLevelM:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_GROUP_ID AS PROGRAMME_ID,
                                        CONCAT(IFNULL(CP.PROGRAMME_NAME, ''),
                                                ' (',
                                                IFNULL(CONCAT(SPM.PROGRAMME_MODE), ''),
                                                ')',
                                                '-',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = PG.SHIFT
                                    WHERE
                                        PG.SHIFT in(?SHIFT) and APPTYPE_ID in(?PROGRAMME_LEVEL)
                                    GROUP BY PG.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchClassByShiftAndProgrammeGroupId:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                           --  AND ACADEMIC_YEAR <= ?AC_YEAR
                                            AND SHIFT = ?SHIFT
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchClassByShiftAndProgrammeGroupIdandClassYear:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID in (?PROGRAMME_GROUP_ID)
                                           AND CLASS_YEAR = ?CLASS_YEAR
                                            AND SHIFT = ?SHIFT
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchClassByShiftAndProgrammeGroupIdandClassYearM:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID in (?PROGRAMME_GROUP_ID)
                                           AND CLASS_YEAR in (?CLASS_YEAR)
                                            AND SHIFT = ?SHIFT
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchClassByShiftMAndProgrammeGroupIdandClassYearM:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID in (?PROGRAMME_GROUP_ID)
                                           AND CLASS_YEAR in(?CLASS_YEAR)
                                            AND SHIFT in(?SHIFT)
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchClassListByClassYarandShift:
                    {
                        query = @"         
                                        SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        CLASS_LEVEL in (?CLASS_LEVEL)  AND CLASS_TYPE!=3
                                           AND CLASS_YEAR in(?CLASS_YEAR)
                                            AND SHIFT in(?SHIFT)
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentsForCourseCertificate:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(UPPER(FIRST_NAME), ' - ', UPPER(ROLL_NO)) AS 'FIRST_NAME',
                                    SC.CLASS_ID
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID AND CLS.PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID
                                WHERE
                                    SC.CLASS_ID in (?CLASS_ID)
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentsForTcPrint:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(UPPER(FIRST_NAME), ' - ', UPPER(ROLL_NO)) AS 'FIRST_NAME',
                                    SC.CLASS_ID
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                WHERE
                                        SP.IS_LEFT != 1
                                        AND SP.IS_DELETED != 1
                                        AND SC.IS_DELETED != 1
                                        AND SC.CLASS_ID in (?CLASS_ID)
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.SaveStuClassWithProgrammeGroupId:
                    {
                        query = @"INSERT INTO STU_CLASS
                                            (
                                            STUDENT_ID,
                                            ACADEMIC_YEAR,
                                            CLASS_ID,
                                            PROGRAMME_GROUP_ID,
                                            BATCH
                                            )
                                            VALUES(
                                            ?STUDENT_ID,
                                            ?ACADEMIC_YEAR,
                                            ?CLASS_ID,
                                            ?PROGRAMME_GROUP_ID,?BATCH);";
                        break;
                    }
                case StudentSQLCommads.FetchStuclassInfoByStudentIdAndAcademicYear:
                    {
                        query = @"SELECT 
                                        STU_CLASS_ID, SC.STUDENT_ID, SC.ACADEMIC_YEAR, SC.CLASS_ID,PROGRAMME_GROUP_ID,ROLL_NO, SP.BATCH
                                    FROM
                                        STU_CLASS AS SC LEFT JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
                                    WHERE
                                        SP.STUDENT_ID = ?STUDENT_ID AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND SC.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchClassByShiftId:
                    {
                        query = @"SELECT 
                                    CLASS_ID,
                                    CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                            '  ',
                                            ' (SHIFT ',
                                            IFNULL(ST.SHIFT_NAME, ''),
                                            ')') AS CLASS_NAME
                                FROM
                                    CP_CLASSES AS CLS
                                        LEFT JOIN
                                    SUP_SHIFT AS ST ON ST.SHIFT_ID = CLS.SHIFT
                                WHERE
                                    SHIFT = ?SHIFT AND PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                     AND ACADEMIC_YEAR<=?AC_YEAR   AND CLS.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchProgramGroupByDepartment:
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
                                    P.DEPARTMENT = ?DEPARTMENT AND PG.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.InsertOldTransferCertificate:
                    {
                        query = @"INSERT INTO STU_OLD_TRANSFER_CERTIFICATE(
                                STUDENT_ID,SERIAL_NO,ADMISSION_NO,FIRST_NAME,GENDER,
                                RELIGION,NATIONALITY,CASTE,COMMUNITY,
                                DATE_OF_BIRTH,ADMISSION_DATE,ADMITTED_CLASS,
                                IDENTIFICATION_MARK1,IDENTIFICATION_MARK2,MAIN_COURSE,ALLIED,
                                FEE_PAID,SCHOLAR_SHIP,LEAVING_DATE,
                                LEAVING_CLASS,CONDUCT,
                                TC_APPLIED_DATE,TC_GIVEN_DATE,FATHER_NAME,
                                GUARDIAN_NAME,ACADEMIC_YEARS,CLASSES_STUDEIED,FIRST_LANGUAGE,DATETOWORDS,ACADEMIC_YEAR)VALUES
                                (
                                ?STUDENT_ID,?SERIAL_NO,?ADMISSION_NO,?FIRST_NAME,?GENDER,
                                ?RELIGION,?NATIONALITY,?CASTE,?COMMUNITY,
                                ?DATE_OF_BIRTH,?ADMISSION_DATE,?ADMITTED_CLASS,
                                ?IDENTIFICATION_MARK1,?IDENTIFICATION_MARK2,?MAIN_COURSE,?ALLIED,
                                ?FEE_PAID,?SCHOLAR_SHIP,?LEAVING_DATE,
                                ?LEAVING_CLASS,?CONDUCT,
                                ?TC_APPLIED_DATE,?TC_GIVEN_DATE,?FATHER_NAME,
                                ?GUARDIAN_NAME,?ACADEMIC_YEARS,?CLASSES_STUDEIED,?FIRST_LANGUAGE,?DATETOWORDS,?AC_YEAR);";
                        break;
                    }
                case StudentSQLCommads.UpdateOldTransferCertificate:
                    {
                        query = @"UPDATE STU_OLD_TRANSFER_CERTIFICATE
                                    SET
                                    STUDENT_ID =?STUDENT_ID,
                                    ADMISSION_NO=?ADMISSION_NO,
                                    FIRST_NAME =?FIRST_NAME,
                                    GENDER =?GENDER,
                                    RELIGION =?RELIGION,
                                    NATIONALITY =?NATIONALITY,
                                    CASTE =?CASTE,
                                    COMMUNITY =?COMMUNITY,
                                    DATE_OF_BIRTH =?DATE_OF_BIRTH,
                                    ADMISSION_DATE =?ADMISSION_DATE,
                                    ADMITTED_CLASS =?ADMITTED_CLASS,
                                    IDENTIFICATION_MARK1 =?IDENTIFICATION_MARK1,
                                    IDENTIFICATION_MARK2 =?IDENTIFICATION_MARK2,
                                    MAIN_COURSE =?MAIN_COURSE,
                                    ALLIED =?ALLIED,
                                    FEE_PAID =?FEE_PAID,
                                    SCHOLAR_SHIP =?SCHOLAR_SHIP,
                                    LEAVING_DATE =?LEAVING_DATE,
                                    LEAVING_CLASS =?LEAVING_CLASS,
                                    CONDUCT =?CONDUCT,
                                    TC_APPLIED_DATE =?TC_APPLIED_DATE,
                                    TC_GIVEN_DATE =?TC_GIVEN_DATE,
                                    FATHER_NAME =?FATHER_NAME,
                                    GUARDIAN_NAME =?GUARDIAN_NAME,
                                    ACADEMIC_YEARS =?ACADEMIC_YEARS,
                                    CLASSES_STUDEIED =?CLASSES_STUDEIED,
                                    FIRST_LANGUAGE =?FIRST_LANGUAGE,
                                    DATETOWORDS =?DATETOWORDS
                                    WHERE CERTIFICATE_ID =?CERTIFICATE_ID;";
                        break;
                    }
                case StudentSQLCommads.IsStudentExistingForOldTC:
                    {
                        query = @"SELECT ADMISSION_NO,CERTIFICATE_ID FROM  STU_OLD_TRANSFER_CERTIFICATE WHERE ADMISSION_NO=?ADMISSION_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchAdmissionNoForOldTc:
                    {
                        query = @"SELECT 
                                        ADMISSION_NO,
                                        CONCAT(IFNULL(UPPER(ADMISSION_NO), ''),
                                                ' - ',
                                                IFNULL(UPPER(FIRST_NAME), '')) AS 'FIRST_NAME'
                                    FROM
                                        STU_OLD_TRANSFER_CERTIFICATE
                                    GROUP BY ADMISSION_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchOldTransferCertificate:
                    {
                        query = @"SELECT 
                                    TC.ADMISSION_NO AS 'ROLL_NO',
                                    TC.CERTIFICATE_ID,
                                    TC.STUDENT_ID,
                                    UPPER(TC.FIRST_NAME) AS 'FIRST_NAME',
                                    UPPER(TC.GENDER) AS 'GENDER',
                                    CONCAT(UPPER(TC.NATIONALITY),
                                            ' - ',
                                            UPPER(TC.RELIGION)) AS 'NATIONALITY',
                                    CONCAT(UPPER(TC.CASTE),
                                            ' - ',
                                            UPPER(TC.COMMUNITY)) AS 'CASTE',
                                    CONCAT(DATE_FORMAT(TC.DATE_OF_BIRTH, '%d/%m/%Y'),
                                            ' ( ',
                                            UPPER(TC.DATETOWORDS),
                                            ' ) ') AS 'DATE_OF_BIRTH',
                                    CONCAT(DATE_FORMAT(TC.ADMISSION_DATE, '%d/%m/%Y'),
                                            ' - ',
                                            UPPER(TC.ADMITTED_CLASS)) AS 'ADMITTED_DATE',
                                    CONCAT('1. ',
                                            ' ',
                                            UPPER(TC.IDENTIFICATION_MARK1)) AS 'IDENTIFICATION_MARK1',
                                    CONCAT('2. ',
                                            ' ',
                                            UPPER(TC.IDENTIFICATION_MARK2)) AS 'IDENTIFICATION_MARK2',
                                    UPPER(TC.MAIN_COURSE) AS 'MAIN_COURSE',
                                    UPPER(TC.ALLIED) AS 'ALLIED',
                                    UPPER(O.OPTION_NAME) AS 'FEE_PAID',
                                    UPPER(SO.OPTION_NAME) AS 'SCHOLAR_SHIP',
                                    DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                                    UPPER(TC.REASON_FOR_LEAVING) AS 'REASON_FOR_LEAVING',
                                    UPPER(TC.LEAVING_CLASS) AS 'LEAVING_CLASS',
                                    UPPER(SC.CONDUCT_NAME) AS 'CONDUCT',
                                    DATE_FORMAT(TC.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                                    DATE_FORMAT(TC.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE',
                                    IF(TC.FATHER_NAME IS NULL
                                            || TC.FATHER_NAME = '',
                                        IF(TC.GUARDIAN_NAME IS NULL
                                                || TC.GUARDIAN_NAME = '',
                                            '----',
                                            UPPER(TC.GUARDIAN_NAME)),
                                        UPPER(TC.FATHER_NAME)) AS 'FATHER_NAME',
                                    UPPER(TC.ACADEMIC_YEARS) AS 'ACADEMIC_YEARS',
                                    UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                                    UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE'
                                FROM
                                    STU_OLD_TRANSFER_CERTIFICATE AS TC
                                        LEFT JOIN
                                    SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                                        LEFT JOIN
                                    SUP_OPTION AS SO ON SO.OPTION_ID = TC.SCHOLAR_SHIP
                                        LEFT JOIN
                                    SUP_CONDUCT AS SC ON SC.CONDUCT_ID = TC.CONDUCT
                                WHERE
                                    ADMISSION_NO = ?ADMISSION_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentInfoByClassId:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.FIRST_NAME,
                                    C.CLASS_NAME AS 'CLASS_ID',
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                    SP.CASTE,
                                    SP.NAME_IN_NATIVE
                                    
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        AND SC.CLASS_ID = ?CLASS_ID ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentByClassIdandProgrammeGroupIdforddl:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, ''),
                                        ' - ',
                                        SP.ROLL_NO) AS FIRST_NAME                               
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    AND CLS.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                            WHERE
                                SC.CLASS_ID = ?CLASS_ID
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                            ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchIndividualSemesterMarks:
                    {
                        query = @"SELECT 
                                SM.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                CR.COURSE_CODE,
                                CONCAT(IF(CT.UGC_COURSE_TYPE IS NULL,'',CONCAT(CT.UGC_COURSE_TYPE,': ')),                                        
                                        CR.COURSE_TITLE) COURSE_TITLE,
                                SM.SEMESTER,
                                SEM.SEMESTER_NAME,
                                IF(SM.INTERNAL_MARK = - 1,'A',SM.INTERNAL_MARK) AS INTERNAL_MARK,
                                IF(SM.EXTERNAL_MARK = - 1,'A',SM.EXTERNAL_MARK) AS EXTERNAL_MARK,
                                IF(SM.TOTAL = - 1,'A',SM.TOTAL) AS TOTAL,
                                SM.GRADE,
                                SM.GRADE_POINT,                               
                                PR.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                CR.CREDITS AS CREDIT,
                                SM.M_PASS,
                                ES.M_EXAM,
                                ES.EXAM_SETTING_ID,
                                CLS.CLASS_LEVEL,
                                SM.RESULT,
                                PG.APPTYPE_ID AS APPLICATION_TYPE_ID,
                                CR.UGC_COURSE_TYPE,
CR.INTERNAL,
CR.EXTERNAL,
                                DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') AS START_DATE,
                                SM.IS_ARREAR
                                
                            FROM
                                SEMESTER_MARKS_?AC_YEAR AS SM
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SM.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    INNER JOIN
                                CP_UGC_COURSE_TYPE AS CT ON CT.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    AND CT.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN 
                               EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID=SM.EXAM_SETTING_ID
                            WHERE                                
                                     SM.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                    AND SC.STUDENT_ID IN (?STUDENT_ID)
                            ORDER BY REGISTER_NO,CR.PART,CT.COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchIndividualSemesterMarksForSupplementary:
                    {
                        query = @"SELECT 
                                SM.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                CR.COURSE_CODE,
                                CONCAT(IF(CT.UGC_COURSE_TYPE IS NULL,'',CONCAT(CT.UGC_COURSE_TYPE,': ')),                                        
                                        CR.COURSE_TITLE) COURSE_TITLE,
                                SM.SEMESTER,
                                SEM.SEMESTER_NAME,
                                IF(SM.INTERNAL_MARK = - 1,'A',SM.INTERNAL_MARK) AS INTERNAL_MARK,
                                IF(SM.EXTERNAL_MARK = - 1,'A',SM.EXTERNAL_MARK) AS EXTERNAL_MARK,
                                IF(SM.TOTAL = - 1,'A',SM.TOTAL) AS TOTAL,
                                SM.GRADE,
                                SM.GRADE_POINT,                               
                                PR.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                CR.CREDITS AS CREDIT,
                                SM.M_PASS,
                                ES.M_EXAM,
                                CLS.CLASS_LEVEL,
                                SM.RESULT,
                                PG.APPTYPE_ID AS APPLICATION_TYPE_ID,
                                CR.UGC_COURSE_TYPE,
                                DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') AS START_DATE
                                
                            FROM
                                SEMESTER_MARKS_?AC_YEAR AS SM
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SM.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    INNER JOIN
                                CP_UGC_COURSE_TYPE AS CT ON CT.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    AND CT.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN 
                               EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID=SM.EXAM_SETTING_ID
                            WHERE                                
                                     SM.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                    AND SC.STUDENT_ID IN (?STUDENT_ID)
                            ORDER BY REGISTER_NO,CR.PART,CT.COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchOverAllSemesterMarks:
                    {
                        query = @"SELECT 
                                SM.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                CR.COURSE_CODE,
                                CONCAT(IF(CT.UGC_COURSE_TYPE IS NULL,'',CONCAT(CT.UGC_COURSE_TYPE,': ')),                                        
                                        CR.COURSE_TITLE) COURSE_TITLE,
                                SEM.SEMESTER_NAME,
                                SM.INTERNAL_MARK,
                                SM.EXTERNAL_MARK,
                                SM.GRADE,
                                SM.GRADE_POINT,
                                SM.TOTAL,
                                PR.PROGRAMME_NAME,
                                CR.CREDITS AS CREDIT,
                                SM.M_PASS,
                                CLS.CLASS_LEVEL,
                                SM.RESULT,
                                CR.UGC_COURSE_TYPE,
                                DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') AS START_DATE
                            FROM
                                SEMESTER_MARKS_?AC_YEAR AS SM
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SM.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    INNER JOIN
                                CP_UGC_COURSE_TYPE AS CT ON CT.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    AND CT.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                                 INNER JOIN exam_setting AS ES ON ES.exam_setting_id=SM.EXAM_SETTING_ID
                            WHERE
                                -- SM.ACADEMIC_YEAR <= ?ACADEMIC_YEAR
                                     SC.STUDENT_ID IN (?STUDENT_ID)
                            ORDER BY REGISTER_NO,CR.PART,CT.COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchOverAllSemesterMarksForSupplementary:
                    {
                        query = @"SELECT 
                                SM.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                CR.COURSE_CODE,
                                CONCAT(IF(CT.UGC_COURSE_TYPE IS NULL,'',CONCAT(CT.UGC_COURSE_TYPE,': ')),                                        
                                        CR.COURSE_TITLE) COURSE_TITLE,
                                SEM.SEMESTER_NAME,
                                SM.INTERNAL_MARK,
                                SM.EXTERNAL_MARK,
                                SM.GRADE,
                                SM.GRADE_POINT,
                                SM.TOTAL,
                                PR.PROGRAMME_NAME,
                                CR.CREDITS AS CREDIT,
                                SM.M_PASS,
                                CLS.CLASS_LEVEL,
                                SM.RESULT,
                                CR.UGC_COURSE_TYPE,
                                DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') AS START_DATE
                            FROM
                                SEMESTER_MARKS_?AC_YEAR AS SM
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SM.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    INNER JOIN
                                CP_UGC_COURSE_TYPE AS CT ON CT.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    AND CT.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                                 INNER JOIN exam_setting AS ES ON ES.exam_setting_id=SM.EXAM_SETTING_ID
                            WHERE                              
                                     SC.STUDENT_ID IN (?STUDENT_ID)
                            ORDER BY REGISTER_NO,CR.PART,CT.COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentByClassIdforddl:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                               UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(SP.LAST_NAME, ''),
                                    ' - ',
                                    SP.ROLL_NO)) AS FIRST_NAME
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO SP ON SC.STUDENT_ID = SP.STUDENT_ID AND SP.IS_DELETED!=1 AND IS_LEFT!=1
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    AND CLS.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                            WHERE
                                SC.CLASS_ID = ?CLASS_ID
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED!=1
                            ORDER BY SP.FIRST_NAME,ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchCIAMarkByStudentandCourse:
                    {
                        query = @"SELECT 
                                CLS.STUDENT_ID,
                                CIA_GROUP_MARK_ID,
                                CGM.COURSE_GROUP_ID,
                                CGM.COURSE_ID,
                                IF(COURSE_GROUP_MARK = '-1',
                                    'A',
                                    COURSE_GROUP_MARK) AS COURSE_GROUP_MARK,
                                REMARK,
                                GC.MAX_MARK,
                                P.PART,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                P.PART_ID,
                                UGC.COURSE_ORDER
                            FROM
                                STU_CLASS AS CLS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    AND SEM.ACADEMIC_YEAR = CLS.ACADEMIC_YEAR
                                    AND SEM.IS_ACTIVE = 1
                                    AND SEM.IS_DELETED != 1
                                    INNER JOIN
                                CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                    AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CC.IS_DELETED != 1
                                    AND CLS.CLASS_ID = ?CLASS_ID
                                    AND CLS.STUDENT_ID = ?STUDENT_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    AND CR.SEMESTER_ID = SEM.SEMESTER
                                    AND CR.IS_COMPULSORY = 1
                                    INNER JOIN
                                CP_COURSE_TYPE_GROUP AS CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    AND CG.IS_DELETED != 1
                                    INNER JOIN
                                CIA_GROUP_COMPONENT AS GC ON GC.CIA_GROUP_ID = CG.GROUP_ID
                                    INNER JOIN
                                SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                                    AND SCC.COMPONENT_ID IN (?COMPONENT_ID)
                                    LEFT JOIN
                                CIA_GROUP_MARKS_?AC_YEAR AS CGM ON CLS.STUDENT_ID = CGM.STUDENT_ID
                                    AND CGM.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                    AND CGM.COURSE_ID = CR.COURSE_ROOT_ID
                                    LEFT JOIN
                                STU_COURSE_REMARK AS RE ON RE.STUDENT_ID = CLS.STUDENT_ID
                                    AND RE.COURSE_ID = CR.COURSE_ROOT_ID
                                    AND RE.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                    LEFT JOIN
                                CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART 
                            UNION SELECT 
                                CLS.STUDENT_ID,
                                CIA_GROUP_MARK_ID,
                                CGM.COURSE_GROUP_ID,
                                CGM.COURSE_ID,
                                IF(COURSE_GROUP_MARK = '-1',
                                    'A',
                                    COURSE_GROUP_MARK) AS COURSE_GROUP_MARK,
                                REMARK,
                                GC.MAX_MARK,
                                P.PART,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                P.PART_ID,
                                UGC.COURSE_ORDER
                            FROM
                                STU_COURSE_INFO AS CLS
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CP ON CP.CLASS_ID = CLS.S_CLASS_ID
                                    INNER JOIN
                                ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    AND SEM.ACADEMIC_YEAR = CLS.ACADEMIC_YEAR
                                    AND SEM.IS_ACTIVE = 1
                                    AND SEM.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CLS.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    AND CLS.CLASS_ID = ?CLASS_ID
                                    AND CLS.STUDENT_ID = ?STUDENT_ID
                                    AND CLS.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CR.SEMESTER_ID = SEM.SEMESTER
                                    AND CR.IS_COMPULSORY != 1
                                    INNER JOIN
                                CP_COURSE_TYPE_GROUP AS CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    AND CG.IS_DELETED != 1
                                    INNER JOIN
                                CIA_GROUP_COMPONENT AS GC ON GC.CIA_GROUP_ID = CG.GROUP_ID
                                    INNER JOIN
                                SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                                    AND SCC.COMPONENT_ID IN (?COMPONENT_ID)
                                    LEFT JOIN
                                CIA_GROUP_MARKS_?AC_YEAR AS CGM ON CLS.STUDENT_ID = CGM.STUDENT_ID
                                    AND CGM.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                    AND CGM.COURSE_ID = CR.COURSE_ROOT_ID
                                    LEFT JOIN
                                STU_COURSE_REMARK AS RE ON RE.STUDENT_ID = CLS.STUDENT_ID
                                    AND RE.COURSE_ID = CR.COURSE_ROOT_ID
                                    AND RE.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                    LEFT JOIN
                                CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                            WHERE
                                CLS.IS_DELETED != 1
                                AND CP.IS_DELETED != 1
                            ORDER BY PART_ID , COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchCoursesByStudent:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID, CC.COURSE_ID, CR.COURSE_TITLE, P.PART
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
										INNER JOIN
									ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
										AND SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                        AND SEM.IS_DELETED!=1
                                        AND SEM.IS_ACTIVE=1
                                        INNER JOIN
                                    CP_CLASS_COURSE AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                        LEFT JOIN
                                    SUP_PART AS P ON CR.PART = P.PART_ID
                                WHERE
                                    SC.STUDENT_ID = ?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CR.SEMESTER_ID = SEM.SEMESTER
                                UNION SELECT 
                                    CI.STUDENT_ID, CI.COURSE_ID, CR.COURSE_TITLE, P.PART
                                FROM
                                    STU_COURSE_INFO AS CI
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CI.STUDENT_ID
										INNER JOIN
									ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
										AND SEM.ACADEMIC_YEAR=CI.ACADEMIC_YEAR
                                        AND SEM.IS_DELETED!=1
                                        AND SEM.IS_ACTIVE=1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CI.COURSE_ID
                                        LEFT JOIN
                                    SUP_PART AS P ON CR.PART = P.PART_ID
                                WHERE
                                    CI.STUDENT_ID = ?STUDENT_ID
                                        AND CI.ACADEMIC_YEAR = ?AC_YEAR       
                                        AND CR.SEMESTER_ID = SEM.SEMESTER;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentDetailsForCIAIndidualReport:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                P.PROGRAMME_NAME,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                DP.DEPARTMENT,
                                CONCAT(IFNULL(CL.CLASS_NAME, ''),
                                        ' -',
                                        ' (SHIFT ',
                                        IFNULL(S.SHIFT_NAME, ''),
                                        ')') AS CLASS_NAME
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_LEFT != 1
                                    AND SP.IS_DELETED != 1
                                    LEFT JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CL.IS_DELETED != 1
                                    LEFT JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                    LEFT JOIN
                                CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                    AND P.IS_DELETED != 1
                                    LEFT JOIN
                                CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = CL.DEPARTMENT
                                    LEFT JOIN
                                SUP_SHIFT AS S ON S.SHIFT_ID = CL.SHIFT
                            WHERE
                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED != 1
                                    AND SC.STUDENT_ID IN (?STUDENT_ID);";
                        break;
                    }
                case StudentSQLCommads.FetchAcademicYearByStudent:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                SC.BATCH,
                                SC.ACADEMIC_YEAR,
                                SC.CLASS_ID,
                                SC.PROGRAMME_GROUP_ID
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                            WHERE
                                SC.STUDENT_ID = ?STUDENT_ID
                                    AND SC.IS_DELETED != 1 ORDER BY SC.ACADEMIC_YEAR DESC;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentAllCoursesByStudentIdAndAcademicYear:
                    {
                        query = @"SELECT 
		                        CCP.COURSE_ID,
		                        CR.COURSE_TITLE,
		                        CCP.ACADEMIC_YEAR,
		                        SEM.TOTAL,
		                        CR.SEMESTER_ID,
		                        SEM.RESULT,
		                        CR.IS_COMPULSORY
	                        FROM
		                        CP_COURSE_STRUCTURE AS CS
			                        INNER JOIN
		                        CP_COURSE_PATTERN AS CCP ON CCP.ACADEMIC_YEAR = CS.ACADEMIC_YEAR
			                        AND CCP.COURSE_STRUCTURE_ID = CS.COURSE_STRUCTURE_ID
                                    AND CCP.IS_DELETED!=1
                                    AND CS.IS_DELETED!=1
			                        INNER JOIN
		                        STU_CLASS AS SC ON SC.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
			                        AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    AND SC.IS_DELETED!=1
			                        INNER JOIN
		                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
			                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
			                        AND SC.STUDENT_ID = ?STUDENT_ID
                                    AND CR.IS_DELETED!=1
			                        LEFT JOIN
		                        SEMESTER_MARKS AS SEM ON SEM.COURSE_ID = CR.COURSE_ROOT_ID
			                        AND SEM.STUDENT_ID = SC.STUDENT_ID
			                        AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuPersonalImages:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET PHOTO=?PHOTO WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidateSemesterMarks:
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
                                CONCAT(IF(UGC.UGC_COURSE_TYPE IS NULL,'',CONCAT(UGC.UGC_COURSE_TYPE,': ')),                                        
                                        CR.CONS_COURSE_TITLE) COURSE_TITLE,
                                UGC.COURSE_ORDER,
                                CR.SEMESTER_ID AS SEMESTER,
                                SS.SEMESTER_NAME,
                                SEM.INTERNAL_MARK,
                                SEM.EXTERNAL_MARK,
                                SEM.GRADE,
                                SEM.GRADE_POINT,
                                SEM.TOTAL,
                                IFNULL(CR.TOTAL, 100) AS MAXIMUM,
                                CP.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                CPG.PROGRAMME_GROUP_ID,
                                CPG.APPTYPE_ID as APPLICATION_TYPE_ID,
                                CR.CREDITS AS CREDIT,
                                SEM.M_PASS,
                                SEM.RESULT,
                                CR.UGC_COURSE_TYPE,
                                ES.M_EXAM,
                               DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') as START_DATE
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                SEMESTER_MARKS_?AC_YEAR AS SEM ON SEM.STUDENT_ID = SP.STUDENT_ID
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
                                    AND ES.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                            WHERE
                                SP.STUDENT_ID IN (?STUDENT_ID)
                                    AND UPPER(SEM.RESULT) != 'F'
                                    AND UPPER(SEM.RESULT) != 'A'
                            ORDER BY CR.PART , UGC.COURSE_ORDER , CR.SEMESTER_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentsForCIAMarkAndAttendance:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                SP.REGISTER_NO,
                                SP.ROLL_NO,
                                CL.CLASS_NAME
                            FROM
                                stu_class AS SC
                                    INNER JOIN
                                stu_personal_info AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                AND SP.IS_DELETED!=2 AND SP.IS_LEFT	!=1
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID AND CL.IS_DELETED!=1
                            WHERE
                                SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.CLASS_ID = ?CLASS_ID
                            ORDER BY SP.FIRST_NAME,SP.ROLL_NO;";
                        break;

                    }
                case StudentSQLCommads.FetchStudentAllCIAMark:
                    {
                        query = @"SELECT 
                            CM.CIA_GROUP_MARK_ID,
                            SC.STUDENT_ID,
                            CR.COURSE_ROOT_ID,
                            GC.MAX_MARK,
                            GC.ORDER_ID,
                            GC.CIA_GROUP_ID,
                            COURSE_GROUP_ID,
                            CR.COURSE_TITLE,
                            GC.SUP_CIA_COMPONENT_ID AS 'COMPONENT_ID',
                            SCC.COMPONENT,
                            IF(CM.COURSE_GROUP_MARK = 0,
                                '',
                                IF(CM.COURSE_GROUP_MARK = '-1',
                                    'A',
                                    CM.COURSE_GROUP_MARK)) AS COURSE_GROUP_MARK,
                            CM.SEMESTER_ID,
                            P.PART_ID
                        FROM
                            STU_CLASS AS SC
								INNER JOIN
							STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
								AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                INNER JOIN
							ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
								AND SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
								AND SEM.IS_ACTIVE=1
                                AND SEM.IS_DELETED!=1
                                INNER JOIN
                            CP_CLASS_COURSE AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                AND CC.CLASS_ID = 1
                                AND CC.IS_DELETED != 1
                                INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                AND CR.IS_DELETED != 1
                                AND CR.SEMESTER_ID = SEM.SEMESTER
                                INNER JOIN
                            CP_COURSE_TYPE_GROUP AS CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                AND CG.IS_DELETED != 1
                                INNER JOIN
                            CIA_GROUP_COMPONENT AS GC ON GC.CIA_GROUP_ID = CG.GROUP_ID
                                INNER JOIN
                            SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                                LEFT JOIN
                            CIA_GROUP_MARKS_?AC_YEAR AS CM ON CM.STUDENT_ID = SC.STUDENT_ID
                                AND CM.COURSE_ID = CR.COURSE_ROOT_ID
                                AND CM.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                LEFT JOIN
                            SUP_PART AS P ON P.PART_ID = CR.PART
                        WHERE
                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                AND SC.IS_DELETED != 1
                                AND SC.STUDENT_ID = ?STUDENT_ID 
                        UNION SELECT 
                            CM.CIA_GROUP_MARK_ID,
                            SC.STUDENT_ID,
                            CR.COURSE_ROOT_ID,
                            GC.MAX_MARK,
                            GC.ORDER_ID,
                            GC.CIA_GROUP_ID,
                            COURSE_GROUP_ID,
                            CR.COURSE_TITLE,
                            GC.SUP_CIA_COMPONENT_ID AS 'COMPONENT_ID',
                            SCC.COMPONENT,
                            IF(CM.COURSE_GROUP_MARK = 0,
                                '',
                                IF(CM.COURSE_GROUP_MARK = '-1',
                                    'A',
                                    CM.COURSE_GROUP_MARK)) AS COURSE_GROUP_MARK,
                            CR.SEMESTER_ID,
                            P.PART_ID
                        FROM
                            STU_COURSE_INFO AS SC
								INNER JOIN
							STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
								AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                INNER JOIN
							ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
								AND SEM.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
								AND SEM.IS_ACTIVE=1
                                AND SEM.IS_DELETED!=1
                                INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SC.COURSE_ID
                                AND SC.STUDENT_ID = ?STUDENT_ID
                                AND SC.CLASS_ID = 1
                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                AND CR.SEMESTER_ID = SEM.SEMESTER
                                INNER JOIN
                            CP_COURSE_TYPE_GROUP AS CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                AND CG.IS_DELETED != 1
                                INNER JOIN
                            CIA_GROUP_COMPONENT AS GC ON GC.CIA_GROUP_ID = CG.GROUP_ID
                                INNER JOIN
                            SUP_CIA_COMPONENTS AS SCC ON SCC.COMPONENT_ID = GC.SUP_CIA_COMPONENT_ID
                                LEFT JOIN
                            CIA_GROUP_MARKS_?AC_YEAR AS CM ON CM.STUDENT_ID = SC.STUDENT_ID
                                AND CM.COURSE_ID = CR.COURSE_ROOT_ID
                                AND CM.COURSE_GROUP_ID = GC.CIA_GROUP_COMPONENT_ID
                                LEFT JOIN
                            SUP_PART AS P ON P.PART_ID = CR.PART
                        ORDER BY STUDENT_ID , PART_ID , COURSE_ROOT_ID , ORDER_ID;";
                        break;

                    }
                case StudentSQLCommads.FetchAcademicYearForSemesterMark:
                    {
                        query = @"SELECT 
                                 AC.AC_YEAR,AC.ACADEMIC_YEAR_ID
                            FROM
                                STU_PERSONAL_INFO AS S
                                    INNER JOIN
                                ACADEMIC_BATCHES AS B ON B.BATCH_ID = S.BATCH
                                    INNER JOIN
                                ACADEMIC_YEAR AS AC ON AC.ACADEMIC_YEAR_ID = B.ACADEMIC_YEAR_ID
                            WHERE
                                S.STUDENT_ID = ?STUDENT_ID AND AC_YEAR<=?ACADEMIC_YEAR ORDER BY AC_YEAR ASC;";
                        break;
                    }
                case StudentSQLCommads.FetchBatchExpiredYear:
                    {
                        query = @"SELECT 
                                BD.BATCH_ID,
                                APPLICATION_TYPE_ID,
                                DURATION,
                                BEGIN_YEAR,
                                EXPIRED_YEAR
                            FROM
                                SUP_BATCH_DURATION AS BD
                                    INNER JOIN
                                SUP_BATCHES AS B ON B.BATCH_ID = BD.BATCH_ID
                            WHERE
                                b.BATCH_ID = ?BATCH_ID
                                    AND APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentByProgrammeGroupBatch:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, ''),
                                        ' - ',
                                        SP.REGISTER_NO) AS FIRST_NAME
                            FROM
                                STU_PERSONAL_INFO AS SP
                            WHERE
                                PROGRAM_ID = ?PROGRAMME_GROUP_ID AND BATCH =?BATCH
                            ORDER BY REGISTER_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchOptionalCoursebyShiftIdAndClassId:
                    {

                        query = @"SELECT 
                                    CCR.COURSE_ROOT_ID,
                                    CONCAT(IFNULL(CCR.COURSE_CODE,''),' - ',IFNULL(CCR.COURSE_TITLE,''),' (SHIFT ',
					                  
                                     IF(CLS.CLASS_ID IS NOT NULL ,IFNULL(SHH.SHIFT_NAME,''),IFNULL(SH.SHIFT_NAME,'')),')') AS COURSE_TITLE,    
                                    IFNULL(CLS.CLASS_ID,C.CLASS_ID) AS CLASS_ID,
                                    CCR.SEMESTER_ID,
                                    CCR.IS_NME_SUBJECT                                    
                                FROM
                                    CP_COURSE_PATTERN AS CCP
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                        AND CCR.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                            ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR
                                        WHERE
                                            ACTIVE_YEAR = 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                        AND SEM.SEMESTER = CCR.SEMESTER_ID
                                        AND SEM.IS_DELETED != 1
                                        AND SEM.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                        AND SEM.BATCH = AB.BATCH_ID
                                        LEFT JOIN
                                    CP_CLASSES AS CLS ON CLS.COURSE_ID = CCP.COURSE_ID
                                        AND CLS.SHIFT = C.SHIFT
                                        AND CLS.IS_DELETED!=1
                                        AND IF(CLS.PROGRAMME_MODE IS NULL ,CLS.PROGRAMME_MODE IS NULL, CLS.PROGRAMME_MODE=C.PROGRAMME_MODE)                                         
                                        INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID=C.SHIFT
                                        AND SH.IS_DELETED!=1                                        
                                        LEFT JOIN
										SUP_SHIFT AS SHH ON SHH.SHIFT_ID=CLS.SHIFT
                                        AND SHH.IS_DELETED!=1                                        
                                WHERE
                                    CCP.ACADEMIC_YEAR =?AC_YEAR
                                        AND C.CLASS_ID =?CLASS_ID
	                                    AND SEM.IS_ACTIVE=1
                                        AND CCR.IS_COMPULSORY!=1
                                        AND CCP.IS_DELETED !=1 GROUP BY CCP.COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.IsStuCourseInfoExist:
                    {
                        query = @"SELECT 
                                        COURSE_INFO_ID
                                    FROM
                                        STU_COURSE_INFO
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR
                                            AND STUDENT_ID = ?STUDENT_ID
                                            AND COURSE_ID = ?COURSE_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.BindSelectedStudentsBysClassId:
                    {
                        query = @"SELECT 
                                    '0' AS COURSE_INFO_ID,
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                                FROM
                                    STU_COURSE_INFO AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    SC.CLASS_ID = ?CLASS_ID
		                                AND SC.COURSE_ID = ?COURSE_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.S_CLASS_ID = ?S_CLASS_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentInformationForStaffEdit:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                FATHER_NAME,
                                FR_OCCUPATION,
                                MOTHER_NAME,
                                MO_OCCUPATION,
                                PHOTO,
                                FR_MOBILE,
                                MO_MOBILE,
                                FIRST_NAME,
                                LAST_NAME,
                                STU_MOBILENO,
                                STU_EMAILID,
                                AADHAR_NUMBER,
                                C_DOOR_DETAIL,
                                C_STREET,
                                C_VILLAGE_AREA,
                                C_POST_PLACE_TOWN,
                                C_TALUK,
                                C_CITY,
                                C_DISTRICT,
                                C_PINCODE,
                                C_PHONENO,
                                C_COUNTRY,
                                P_DOOR_DETAIL,
                                P_STREET,
                                P_VILLAGE_AREA,
                                P_POST_PLACE_TOWN,
                                P_TALUK,
                                P_CITY,
                                P_DISTRICT,
                                P_PINCODE,
                                P_PHONENO,
                                P_COUNTRY
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    LEFT JOIN
                                STU_ADDRESS AS AD ON AD.STUDENT_ID = SP.STUDENT_ID
                                    AND AD.IS_DELETED != 1
                            WHERE
                                SP.STUDENT_ID=?STUDENT_ID AND
                                SP.IS_DELETED != 1 AND SP.IS_LEFT != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentReportByFields:
                    {
                        query = @"SELECT 
                                FIELD_ID, FIELD, FIELD_ALIAS, FIELD_TABLE, FEILD_ORDER
                            FROM
                                STU_REPORT_FIELDS
                            WHERE
                                IS_DELETED != 1
                                    AND FIELD_ID IN (?FIELD_ID)
                            ORDER BY FEILD_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentReportByForOffice:
                    {
                        query = @"SELECT
                                      ?FIELD
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                            AND CL.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND PG.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = CP.DEPARTMENT
                                            AND DP.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_BLOOD_GROUP AS BL ON BL.BLOOD_GROUP_ID = SP.BLOOD_GROUP
                                            AND BL.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_RELIGION AS RG ON RG.RELIGIONID = SP.RELIGION
                                            AND RG.ISDELETED != 1
                                            LEFT JOIN
                                        SUP_COMMUNITY AS COM ON COM.COMMUNITYID = SP.COMMUNITY
                                            AND COM.ISDELETED != 1
                                            LEFT JOIN
                                        SUP_GENDER AS GEN ON GEN.GENDER_ID = SP.GENDER
                                            AND GEN.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_CLASS_LEVEL AS CY ON CY.CLASSLEVELID = CL.CLASS_LEVEL
                                            AND CY.ISDELETED != 1
                                            LEFT JOIN
                                        SUP_NATIONALITY AS NA ON NA.NATIONALITY_ID = SP.NATIONALITY
                                            AND NA.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_OCCUPATION AS FO ON FO.OCCUPATION_ID = SP.FR_OCCUPATION
                                            AND FO.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_OCCUPATION AS MO ON MO.OCCUPATION_ID = SP.FR_OCCUPATION
                                            AND MO.IS_DELETED != 1
                                            LEFT JOIN
                                        SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                            AND PM.IS_DELETED != 1
	                                    -- left JOIN STU_ADDRESS AS AD ON AD.STUDENT_ID=SP.STUDENT_ID AND AD.IS_DELETED!=1   
                                      --  LEFT JOIN sup_country AS CC ON CC.COUNTRY_ID=AD.P_COUNTRY_ID
	
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchCasteReportByProgrammeForProgramme:
                    {
                        query = @"
                                SELECT 
                                COUNT(SPI.COMMUNITY)AS TOTAL,
                                CONCAT(CPP.PROGRAMME_NAME,' ',SPM.PROGRAMME_MODE,' ','(SHIFT',' ',SS.SHIFT_NAME,')') AS PROGRAMME_NAME,
                                 SUM(IF(SPI.COMMUNITY = 1, 1, 0)) AS BC,
                                 SUM(IF(SPI.COMMUNITY = 2, 1, 0)) AS BCM,
                                 SUM(IF(SPI.COMMUNITY = 3, 1, 0)) AS DNC,
                                 SUM(IF(SPI.COMMUNITY = 4, 1, 0)) AS FC,
                                 SUM(IF(SPI.COMMUNITY = 5, 1, 0)) AS MBC,
                                 SUM(IF(SPI.COMMUNITY = 6, 1, 0)) AS OBC,
                                 SUM(IF(SPI.COMMUNITY = 7, 1, 0)) AS OC,
                                 SUM(IF(SPI.COMMUNITY = 8, 1, 0)) AS SC,
                                 SUM(IF(SPI.COMMUNITY = 9, 1, 0)) AS SCA,
                                 SUM(IF(SPI.COMMUNITY = 10, 1, 0)) AS ST
                                 
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                    INNER JOIN
                                SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                            WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                    AND CPG.PROGRAMME_MODE IN (?PROGRAMME_MODE)
                                    AND SC.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                    AND SC.IS_DELETED != 1
                                    AND CPP.PROGRAMME_ID IN(?PROGRAMME_ID)
                                    AND SPI.COMMUNITY IN(?COMMUNITY)
                                    GROUP BY SC.PROGRAMME_GROUP_ID;";
                        break;
                    }

                case StudentSQLCommads.FetchCasteReportByProgrammeForStudents:
                    {
                        query = @"  SELECT 
                                    CONCAT(CPP.PROGRAMME_NAME,
                                            ' ',
                                            SPM.PROGRAMME_MODE,
                                            ' ',
                                            '(SHIFT',
                                            ' ',
                                            SS.SHIFT_NAME,
                                            ')') AS PROGRAMME_NAME,
                                    CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                            IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                    SCC.COMMUNITY
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                                        INNER JOIN
                                    SUP_COMMUNITY AS SCC ON SCC.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                        AND CPG.PROGRAMME_MODE IN (?PROGRAMME_MODE)
                                        AND SC.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        AND SC.IS_DELETED != 1
                                        AND  CPG.PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID)
                                        AND SPI.COMMUNITY IN(?COMMUNITY)
                                        ORDER BY CPG.PROGRAMME_GROUP_ID;";
                        break;
                    }

                case StudentSQLCommads.FetchCasteReportByProgrammeForCommunity:
                    {
                        query = @"SELECT 
                                    SCC.COMMUNITY,
                                    SUM(IF(SPI.COMMUNITY != 0
                                            OR SPI.COMMUNITY != NULL,
                                        1,
                                        0)) AS COUNT
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                                        INNER JOIN
                                    SUP_COMMUNITY AS SCC ON SCC.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                        AND CPG.PROGRAMME_MODE IN (?PROGRAMME_MODE)
                                        AND SC.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        AND SC.IS_DELETED != 1
                                        AND CPG.PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID)
                                        AND SPI.COMMUNITY IN(?COMMUNITY)
                                        GROUP BY SPI.COMMUNITY;";
                        break;

                    }
                case StudentSQLCommads.FetchProgrammeByProgrammeModeandShift:
                    {
                        query = @"SELECT 
                                        CP.PROGRAMME_GROUP_ID AS PROGRAMME_GROUP_ID,
                                        CPP.PROGRAMME_NAME AS PROGRAMME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                                    WHERE
                                        CPG.SHIFT IN(?SHIFT)
                                            AND CPG.PROGRAMME_MODE IN(?PROGRAMME_MODE)
                                            AND SC.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCasteByProgramme:
                    {
                        query = @"SELECT 
                                    SPI.COMMUNITY AS COMMUNITY_ID,
                                    SCM.COMMUNITY
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                                        INNER JOIN 
                                        SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID=SPI.COMMUNITY
        
                                WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                        AND CPG.PROGRAMME_MODE IN(?PROGRAMME_MODE)
                                        AND SC.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        AND CPP.PROGRAMME_ID IN(?PROGRAMME_ID)
                                        AND SC.IS_DELETED != 1
                                GROUP BY SPI.COMMUNITY;";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeByProgrammeGroupId:
                    {
                        query = @"                      
		                    	SELECT 
                                CPG.PROGRAMME_GROUP_ID,
                                 CONCAT(CPP.PROGRAMME_NAME,' ',SPM.PROGRAMME_MODE,' ','(SHIFT',' ',SS.SHIFT_NAME,')') AS PROGRAMME_NAME
                               
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                    INNER JOIN
                                SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                            WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                    AND CPG.PROGRAMME_MODE IN (?PROGRAMME_MODE)
                                    AND SC.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                    AND SC.IS_DELETED != 1
                                    AND CPG.PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID)
                                    group by SC.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCasteByCasteId:
                    {
                        query = @"SELECT 
                                    CPG.PROGRAMME_GROUP_ID,
                                    SPI.COMMUNITY AS COMMUNITY,
                                    SCC.COMMUNITY AS COMMUNITY_NAME,
                                    COUNT(SPI.COMMUNITY) AS COUNT
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                                        INNER JOIN
                                    SUP_COMMUNITY AS SCC ON SCC.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                        AND CPG.PROGRAMME_MODE IN (?PROGRAMME_MODE)
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SPI.COMMUNITY IN (?COMMUNITY)
                                        AND SC.IS_DELETED != 1
                                GROUP BY SCC.COMMUNITYID;";
                        break;
                    }

                case StudentSQLCommads.FetchCasteByProgrammeGroup:
                    {
                        query = @"SELECT 
                                    CPG.PROGRAMME_GROUP_ID,
                                    SPI.COMMUNITY AS COMMUNITY,
                                    SCC.COMMUNITY AS COMMUNITY_NAME,
                                    COUNT(SPI.COMMUNITY) AS COUNT
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CP ON SC.CLASS_ID = CP.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CP.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CPP ON CPG.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON CPG.SHIFT = SS.SHIFT_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON CPG.PROGRAMME_MODE = SPM.PROGRAMME_MODE_ID
                                        INNER JOIN
                                    SUP_COMMUNITY AS SCC ON SCC.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    CPG.SHIFT IN (?SHIFT)
                                        AND CPG.PROGRAMME_MODE IN (?PROGRAMME_MODE)
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SPI.COMMUNITY IN (?COMMUNITY)
                                        AND SC.IS_DELETED != 1
                                GROUP BY CPG.PROGRAMME_GROUP_ID , SCC.COMMUNITYID
                                ORDER BY CPG.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchClassYearStudentReport:
                    {
                        query = @"SELECT 
                                            SP.ADMISSION_NO,
                                            SP.REGISTER_NO,
                                            CONCAT(SP.FIRST_NAME, '', SP.LAST_NAME) AS FIRST_NAME,
                                            SC.CLASS_ID,
                                            CC.CLASS_NAME,
                                            REL.RELIGION,
                                            COM.COMMUNITY,
                                            PM.PROGRAMME_MODE
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            CP_CLASSES AS CC ON SC.CLASS_ID = CC.CLASS_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CC.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                                INNER JOIN
                                            SUP_COMMUNITY AS COM ON COM.COMMUNITYID = SP.COMMUNITY
                                                INNER JOIN
                                            SUP_RELIGION AS REL ON REL.RELIGIONID = SP.RELIGION
                                                INNER JOIN
                                            SUP_GENDER AS G ON G.GENDER_ID = SP.GENDER
                                                INNER JOIN
                                            SUP_SHIFT AS S ON S.SHIFT_ID = SP.SHIFT_ID
                                                INNER JOIN
                                            SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = CC.PROGRAMME_MODE
                                        WHERE
		                                        CPG.PROGRAMME_GROUP_ID =?PROGRAM_ID
                                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        ORDER BY CC.CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchAllCourseFromCoursePatternByStudentId:
                    {
                        query = @"SELECT 
                                            CCP.COURSE_ID,
                                            CCP.COURSE_STRUCTURE_ID,
                                            UPPER(CCR.COURSE_CODE) AS COURSE_CODE,
                                            UPPER(CCR.COURSE_TITLE) AS COURSE_TITLE,
                                            CCR.SEMESTER_ID,
                                            SSEM.SEMESTER_NAME,
                                            CCR.CREDITS
    
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                                INNER JOIN
                                            CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                                AND CCP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                AND CCP.IS_DELETED != 1
                                                INNER JOIN
                                            CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                                AND CCR.SEMESTER_ID IN (?SEMESTER_ID) INNER JOIN SUP_SEMESTER AS SSEM ON SSEM.SUP_SEMESTER_ID=CCR.SEMESTER_ID
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.STUDENT_ID = ?STUDENT_ID
                                                AND SC.IS_DELETED != 1
                                        ORDER BY CCR.SEMESTER_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchSemesteridsByStudentId:
                    {
                        query = @"SELECT 
	                                    
                                        IF(SEM.SEMESTER = 2,
                                            '1,2',
                                            IF(SEM.SEMESTER = 3,
                                                '1,2,3',
                                                IF(SEM.SEMESTER = 4,
                                                    '1,2,3,4',
                                                    IF(SEM.SEMESTER = 5,
                                                        '1,2,3,4,5',
                                                        IF(SEM.SEMESTER = 6,
                                                            '1,2,3,4,5,6',
                                                            SEM.SEMESTER))))) AS SEMESTER,
                                        B.BEGIN_YEAR,
                                        B.EXPIRED_YEAR,
                                        B.BATCH_ID AS BATCH
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
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        SUP_BATCH_DURATION AS B ON B.BATCH_ID = AB.BATCH_ID
                                            AND B.APPLICATION_TYPE_ID = PG.APPTYPE_ID 
                                    WHERE
                                        SC.ACADEMIC_YEAR =?AC_YEAR
                                            AND SEM.IS_ACTIVE = 1
                                            AND SC.STUDENT_ID =?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchOptionCourseByStudentId:
                    {
                        query = @"SELECT 
                                        SC.COURSE_ID,
                                        UPPER(CCR.COURSE_CODE) AS COURSE_CODE,
                                        UPPER(CCR.COURSE_TITLE) AS COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        SSEM.SEMESTER_NAME,
                                        CCR.CREDITS
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
                                          --  AND CCR.SEMESTER_ID IN (1 , 2, 3, 4, 5, 6)
                                            INNER JOIN
                                        SUP_SEMESTER AS SSEM ON SSEM.SUP_SEMESTER_ID = SC.SEMESTER
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                    ORDER BY CCR.SEMESTER_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentimage:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET PHOTO=?PHOTO WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                #region Admission Register 
                case StudentSQLCommads.FetchAdmissionRegisterByStudentIdAndAcYear:
                    {
                        query = @"SELECT 
                                    REGISTER_ID, 
                                    SERIAL_NO, 
                                    ADMISSION_NO, 
                                    STUDENT_ID, 
                                    FIRST_NAME,
                                    LAST_NAME, 
                                    GENDER, 
                                    RELIGION, 
                                    NATIONALITY, 
                                    CASTE, 
                                    COMMUNITY, 
                                    DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y')AS 'DATE_OF_BIRTH', 
									DATE_FORMAT(ADMISSION_DATE,'%d/%m/%Y')AS ADMISSION_DATE, 
                                    ADMITTED_CLASS, 
                                    IDENTIFICATION_MARK1, 
                                    IDENTIFICATION_MARK2, 
                                    MAIN_COURSE, ALLIED, 
                                    FEE_PAID, 
                                    SCHOLAR_SHIP, 
                                    DATE_FORMAT(LEAVING_DATE,'%d/%m/%Y')AS LEAVING_DATE, 
                                    REASON_FOR_LEAVING, 
                                    LEAVING_CLASS, CONDUCT, 
                                    DATE_FORMAT(TC_APPLIED_DATE,'%d/%m/%Y')AS TC_APPLIED_DATE, 
                                    DATE_FORMAT(TC_GIVEN_DATE,'%d/%m/%Y')AS TC_GIVEN_DATE, 
                                    FATHER_NAME, 
                                    GUARDIAN_NAME, 
                                    ACADEMIC_YEARS, 
                                    CLASSES_STUDEIED, 
                                    FIRST_LANGUAGE, 
                                    DATETOWORDS,
                                    MEDIUM_OF_INSTRUCTION,
                                    STUDIED_FROM,
                                    CONTACT_NO,
                                    ADDRESS
                                FROM
                                    STU_ADMISSION_REGISTER WHERE STUDENT_ID=?STUDENT_ID AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }

                case StudentSQLCommads.UpdateAdmissionRegister:
                    {
                        query = @"UPDATE STU_TRANSFER_CERTIFICATE
                                    SET
                                    FIRST_NAME =?FIRST_NAME ,
                                    LAST_NAME =?LAST_NAME ,
                                    GENDER =?GENDER ,
                                    RELIGION =?RELIGION ,
                                    NATIONALITY =?NATIONALITY ,
                                    CASTE =?CASTE ,
                                    COMMUNITY =?COMMUNITY ,
                                    DATE_OF_BIRTH =?DATE_OF_BIRTH ,
                                    ADMISSION_DATE =?ADMISSION_DATE ,
                                    ADMITTED_CLASS =?ADMITTED_CLASS ,
                                    IDENTIFICATION_MARK1 =?IDENTIFICATION_MARK1 ,
                                    IDENTIFICATION_MARK2 =?IDENTIFICATION_MARK2 ,
                                    MAIN_COURSE =?MAIN_COURSE ,
                                    ALLIED =?ALLIED ,
                                    FEE_PAID =?FEE_PAID ,
                                    SCHOLAR_SHIP =?SCHOLAR_SHIP ,
                                    LEAVING_DATE =?LEAVING_DATE ,
                                    REASON_FOR_LEAVING =?REASON_FOR_LEAVING ,
                                    LEAVING_CLASS =?LEAVING_CLASS ,
                                    CONDUCT =?CONDUCT ,
                                    ACADEMIC_YEAR =?ACADEMIC_YEAR ,
                                    TC_APPLIED_DATE =?TC_APPLIED_DATE ,
                                    TC_GIVEN_DATE =?TC_GIVEN_DATE ,
                                    FATHER_NAME =?FATHER_NAME ,
                                    GUARDIAN_NAME =?GUARDIAN_NAME ,
                                    ACADEMIC_YEARS =?ACADEMIC_YEARS ,
                                    CLASSES_STUDEIED =?CLASSES_STUDEIED ,
                                    FIRST_LANGUAGE =?FIRST_LANGUAGE ,
                                    DATETOWORDS =?DATETOWORDS ,
                                    MEDIUM_OF_INSTRUCTION =?MEDIUM_OF_INSTRUCTION ,
                                    STUDIED_FROM =?STUDIED_FROM ,
                                    CONTACT_NO =?CONTACT_NO ,
                                    ADDRESS =?ADDRESS 
                                    WHERE CERTIFICATE_ID =?CERTIFICATE_ID;";
                        break;
                    }
                case StudentSQLCommads.IsExistAdmissionRegister:
                    {
                        query = @"SELECT 
                                        REGISTER_ID, 
                                        STUDENT_ID, 
                                        FIRST_NAME, 
                                        LAST_NAME
                                    FROM
                                        STU_ADMISSION_REGISTER
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.InsertAdmissionRegister:
                    {
                        query = @"INSERT INTO STU_TRANSFER_CERTIFICATE
                                (
                                STUDENT_ID,SERIAL_NO,ADMISSION_NO,FIRST_NAME,GENDER,
                                RELIGION,NATIONALITY,CASTE,COMMUNITY,
                                DATE_OF_BIRTH,ADMISSION_DATE,ADMITTED_CLASS,
                                IDENTIFICATION_MARK1,IDENTIFICATION_MARK2,MAIN_COURSE,ALLIED,
                                FEE_PAID,SCHOLAR_SHIP,LEAVING_DATE,
                                LEAVING_CLASS,CONDUCT,
                                TC_APPLIED_DATE,TC_GIVEN_DATE,FATHER_NAME,
                                GUARDIAN_NAME,ACADEMIC_YEARS,CLASSES_STUDEIED,FIRST_LANGUAGE,DATETOWORDS,ACADEMIC_YEAR,
                                CONTACT_NO,ADDRESS,MEDIUM_OF_INSTRUCTION,STUDIED_FROM)VALUES
                                (
                                ?STUDENT_ID,?SERIAL_NO,?ADMISSION_NO,?FIRST_NAME,?GENDER,
                                ?RELIGION,?NATIONALITY,?CASTE,?COMMUNITY,
                                ?DATE_OF_BIRTH,?ADMISSION_DATE,?ADMITTED_CLASS,
                                ?IDENTIFICATION_MARK1,?IDENTIFICATION_MARK2,?MAIN_COURSE,?ALLIED,
                                ?FEE_PAID,?SCHOLAR_SHIP,?LEAVING_DATE,
                                ?LEAVING_CLASS,?CONDUCT,
                                ?TC_APPLIED_DATE,?TC_GIVEN_DATE,?FATHER_NAME,
                                ?GUARDIAN_NAME,?ACADEMIC_YEARS,?CLASSES_STUDEIED,?FIRST_LANGUAGE,?DATETOWORDS,?ACADEMIC_YEAR,
                                ?CONTACT_NO,?ADDRESS,?MEDIUM_OF_INSTRUCTION,?STUDIED_FROM);";
                        break;
                    }
                case StudentSQLCommads.PrintAdmissionRegisterInfo:
                    {
                        query = @"SELECT 
                                        AR.REGISTER_ID,
                                        SP.ROLL_NO,
                                        SP.ADMISSION_NO,
                                        AR.STUDENT_ID,
                                        CONCAT(IFNULL(AR.FIRST_NAME,''),' ',ifnull(AR.LAST_NAME,'')) AS FIRST_NAME,
                                        G.GENDER_NAME  AS GENDER,
                                        AR.RELIGION,
                                        AR.NATIONALITY,
                                        AR.CASTE,
                                        AR.COMMUNITY,
                                        DATE_FORMAT(AR.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(AR.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        AR.ADMITTED_CLASS,
                                        AR.IDENTIFICATION_MARK1,
                                        AR.IDENTIFICATION_MARK2,
                                        AR.MAIN_COURSE,
                                        ALLIED,
                                        IF(AR.FEE_PAID = 1, 'YES', 'NO') AS FEE_PAID,
                                        IF(AR.SCHOLAR_SHIP = 1, 'YES', 'NO') AS SCHOLAR_SHIP,
                                        DATE_FORMAT(AR.LEAVING_DATE, '%d/%m/%Y') AS LEAVING_DATE,
                                        AR.REASON_FOR_LEAVING,
                                        AR.LEAVING_CLASS,
                                        IF(AR.CONDUCT = 1, 'YES', 'NO') AS CONDUCT,
                                        DATE_FORMAT(AR.TC_APPLIED_DATE, '%d/%m/%Y') AS TC_APPLIED_DATE,
                                        DATE_FORMAT(AR.TC_GIVEN_DATE, '%d/%m/%Y') AS TC_GIVEN_DATE,
                                        AR.FATHER_NAME,
                                        AR.GUARDIAN_NAME,
                                        AR.ACADEMIC_YEARS,
                                        AR.CLASSES_STUDEIED,
                                        AR.FIRST_LANGUAGE,
                                        AR.DATETOWORDS,
                                        AR.MEDIUM_OF_INSTRUCTION,
                                        AR.STUDIED_FROM,
                                        AR.CONTACT_NO,
                                        AR.ADDRESS
                                    FROM
                                        STU_ADMISSION_REGISTER AS AR
		                                    INNER JOIN
	                                    SUP_GENDER AS G ON G.GENDER_ID=AR.GENDER
		                                    INNER JOIN
	                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=AR.STUDENT_ID
		                                    AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                    WHERE
                                        AR.STUDENT_ID IN (?STUDENT_ID)
                                            AND AR.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                #endregion
                case StudentSQLCommads.FetchUGgradeByMark:
                    {
                        query = @"SELECT 
                                GRADE, GRADE_POINT
                            FROM
                                SUP_GRADE_POINTS
                            WHERE
                                MIN_MARK <= ?MIN_MARK AND MAX_MARK >= ?MIN_MARK
                                    AND PROGRAME_LEVEL = 1;";
                        break;
                    }
                case StudentSQLCommads.FetchConvocationApplicantInfoByStudentId:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                PG.PROGRAMME_GROUP_ID,
                                CP.PROGRAMME_NAME,
                                SP.FIRST_NAME,
                                SP.LAST_NAME,
                                date_format(DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                SP.GENDER,
                                GE.GENDER_NAME,
                                SP.FATHER_NAME,
                                SP.PHOTO,
                                SP.COMMUNITY,
                                COM.COMMUNITY as COMMUNITY_NAME,
                                SP.CASTE,
                                SP.ROLL_NO,
                                STU_MOBILENO AS STUDENT_MOBILE_NO,
                                SP.RELIGION,
                                RL.RELIGION AS RELIGION_NAME,
                                CC.COUNTRY_NAME,
                                ADDRESS_ID,
                                C_DOOR_DETAIL AS P_DOOR_DEATAIL,
                                C_STREET AS P_STREET,
                                C_VILLAGE_AREA AS P_VILLAGE_AREA,
                                C_POST_PLACE_TOWN AS P_POST_PLACE_TOWN,
                                C_TALUK AS P_TALUK,
                                C_CITY AS P_CITY,
                                C_DISTRICT AS P_DISTRICT,
                                C_PINCODE AS P_PINCODE,
                                C_PHONENO,
                                C_COUNTRY AS P_CONUNTRY
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    LEFT JOIN
                                STU_ADDRESS AS AD ON AD.STUDENT_ID = SP.STUDENT_ID
                                    LEFT JOIN
                                SUP_RELIGION AS RL ON RL.RELIGIONID = SP.RELIGION
                                    LEFT JOIN
                                SUP_COMMUNITY AS COM ON COM.COMMUNITYID = SP.COMMUNITY
                                    LEFT JOIN
                                SUP_GENDER AS GE ON GE.GENDER_ID = SP.GENDER
                                    LEFT JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    LEFT JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                    LEFT JOIN
                                SUP_COUNTRY AS CC ON CC.COUNTRY_ID = AD.C_COUNTRY
                            WHERE
                                SP.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.CheckConvocationApplicationIsExistByStudentId:
                    {
                        query = @"";
                        break;
                    }
                case StudentSQLCommads.InsertConvocationApplication:
                    {
                        query = @"INSERT INTO `convocation_application_register`
                                (
                                `STUDENT_ID`,
                                `FIRST_NAME`,
                                `TAMIL_NAME`,
                                `DATE_OF_BIRTH`,
                                `GENDER`,
                                `RELIGION`,
                                `COMMUNITY`,
                                `CASTE`,
                                `MOTHER_NAME`,
                                `PHOTO`,
                                `PROGRAMME_GROUP_ID`,
                                `P_DOOR_DEATAIL`,
                                `P_STREET`,
                                `P_VILLAGE_AREA`,
                                `P_POST_PLACE_TOWN`,
                                `P_TALUK`,
                                `P_CITY`,
                                `P_DISTRICT`,
                                `P_PINCODE`,
                                `P_STATE`,
                                `P_CONUNTRY`,
                                `BATCH`,
                                `FATHER_NAME`,
                                `STUDENT_MOBILE_NO`,
                                `PARENT_MOBILE`,
                                `EMAIL_ID`,
                                `AADHAR_NO`,
                                `YEAR_OF_JOIN`,
                                `YEAR_OF_PASSING`,
                                `IS_DISABLED`,
                                `ACADEMIC_YEAR`,
                                `TAMIL_NAME1`
                                )
                                VALUES
                                (
                                ?STUDENT_ID,
                                ?FIRST_NAME,
                                ?TAMIL_NAME,
                                ?DATE_OF_BIRTH,
                                ?GENDER,
                                ?RELIGION,
                                ?COMMUNITY,
                                ?CASTE,
                                ?MOTHER_NAME,
                                ?PHOTO,
                                ?PROGRAMME_GROUP_ID,
                                ?P_DOOR_DEATAIL,
                                ?P_STREET,
                                ?P_VILLAGE_AREA,
                                ?P_POST_PLACE_TOWN,
                                ?P_TALUK,
                                ?P_CITY,
                                ?P_DISTRICT,
                                ?P_PINCODE,
                                ?P_STATE,
                                ?P_CONUNTRY,
                                ?BATCH,
                                ?FATHER_NAME,
                                ?STUDENT_MOBILE_NO,
                                ?PARENT_MOBILE,
                                ?EMAIL_ID,
                                ?AADHAR_NO,
                                ?YEAR_OF_JOIN,
                                ?YEAR_OF_PASSING,
                                ?IS_DISABLED,                                
                                ?AC_YEAR,?TAMIL_NAME1);";
                        break;
                    }
                case StudentSQLCommads.UpdateConvocationApplication:
                    {
                        query = @"UPDATE `CONVOCATION_APPLICATION_REGISTER`
                                    SET
                                    `FIRST_NAME` =?FIRST_NAME,
                                    `TAMIL_NAME` =?TAMIL_NAME,
                                    `DATE_OF_BIRTH` =?DATE_OF_BIRTH,
                                    `GENDER` =?GENDER,
                                    `RELIGION` =?RELIGION,
                                    `COMMUNITY` =?COMMUNITY,
                                    `CASTE` =?CASTE,
                                    `MOTHER_NAME` =?MOTHER_NAME,
                                    `PHOTO` =?PHOTO,
                                    `PROGRAMME_GROUP_ID` =?PROGRAMME_GROUP_ID,
                                    `P_DOOR_DEATAIL` =?P_DOOR_DEATAIL,
                                    `P_STREET` =?P_STREET,
                                    `P_VILLAGE_AREA` =?P_VILLAGE_AREA,
                                    `P_POST_PLACE_TOWN` =?P_POST_PLACE_TOWN,
                                    `P_TALUK` =?P_TALUK,
                                    `P_CITY` =?P_CITY,
                                    `P_DISTRICT` =?P_DISTRICT,
                                    `P_PINCODE` =?P_PINCODE,
                                    `P_STATE` =?P_STATE,
                                    `P_CONUNTRY` =?P_CONUNTRY,
                                    `BATCH` =?BATCH,
                                    `FATHER_NAME` =?FATHER_NAME,
                                    `STUDENT_MOBILE_NO` =?STUDENT_MOBILE_NO,
                                    `PARENT_MOBILE` =?PARENT_MOBILE,
                                    `EMAIL_ID` =?EMAIL_ID,
                                    `AADHAR_NO` =?AADHAR_NO,
                                    `YEAR_OF_JOIN` =?YEAR_OF_JOIN,
                                    `YEAR_OF_PASSING` =?YEAR_OF_PASSING,
                                    `IS_DISABLED` =?IS_DISABLED,
                                    `TAMIL_NAME1` =?TAMIL_NAME1
                                    WHERE CONVOC_APP_REGISTER_ID=?CONVOC_APP_REGISTER_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuAddressByConvocationDetails:
                    {
                        query = @"UPDATE STU_ADDRESS 
                                SET                              
                                C_DOOR_DETAIL=?P_DOOR_DEATAIL,
                                C_STREET=?P_STREET,
                                C_VILLAGE_AREA=?P_VILLAGE_AREA,
                                C_POST_PLACE_TOWN=?P_POST_PLACE_TOWN,
                                C_TALUK=?P_TALUK,
                                C_CITY=?P_CITY,
                                C_DISTRICT=?P_DISTRICT,
                                C_PINCODE=?P_PINCODE,
                                C_PHONENO=?PARENT_MOBILE,
                                C_COUNTRY=?P_CONUNTRY
                                WHERE IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuPersonalByConvocationDetails:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO 
                                    SET 
                                    MOTHER_NAME=?MOTHER_NAME,
                                    AADHAR_NUMBER=?AADHAR_NO,
                                    STU_EMAILID=?EMAIL_ID,
                                    FR_MOBILE=?PARENT_MOBILE,
                                    STU_MOBILENO=?STUDENT_MOBILE_NO
                                    WHERE IS_DELETED!=1 AND STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchConvocationApplicationRegisteredByStudentId:
                    {
                        query = @"SELECT 
                                CONVOC_APP_REGISTER_ID,
                                CC.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.FIRST_NAME,
                                TAMIL_NAME,
                                TAMIL_NAME1,
                                DATE_FORMAT(CC.DATE_OF_BIRTH,'%d/%m/%Y')  as DATE_OF_BIRTH,
                                CC.GENDER,
                                CC.RELIGION,
                                CC.COMMUNITY,
                                CC.CASTE,
                                CC.MOTHER_NAME,
                                SP.PHOTO,
                                PROGRAMME_GROUP_ID,
                                P_DOOR_DEATAIL,
                                P_STREET,
                                P_VILLAGE_AREA,
                                P_POST_PLACE_TOWN,
                                P_TALUK,
                                P_CITY,
                                P_DISTRICT,
                                P_PINCODE,
                                P_STATE,
                                SS.STATE_NAME,
                                P_CONUNTRY,
                                SC.COUNTRY_NAME,
                                CC.BATCH,
                                CC.FATHER_NAME,
                                STUDENT_MOBILE_NO,
                                PARENT_MOBILE,
                                EMAIL_ID,
                                AADHAR_NO,
                                YEAR_OF_JOIN,
                                YEAR_OF_PASSING,
                                IS_DISABLED,
                                REGISTER_DATE
                            FROM
                                CONVOCATION_APPLICATION_REGISTER AS CC
                                    INNER JOIN
                                SUP_GENDER AS GE ON GE.GENDER_ID = CC.GENDER
                                    INNER JOIN
                                SUP_COMMUNITY AS COM ON COM.COMMUNITYID = CC.COMMUNITY
                                    INNER JOIN
                                SUP_STATE SS ON SS.STATE_ID = CC.P_STATE
                                    INNER JOIN
                                SUP_COUNTRY AS SC ON SC.COUNTRY_ID = CC.P_CONUNTRY
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CC.STUDENT_ID
                            WHERE
                                CC.STUDENT_ID = ?STUDENT_ID
                                    AND CC.IS_DELETED != 1
                                    AND CC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.ListConvocationApplicationRegistered:
                    {
                        query = @"SELECT 
                                    CONVOC_APP_REGISTER_ID,
                                    STUDENT_ID,
                                    FIRST_NAME,
                                    TAMIL_NAME,
                                    DATE_OF_BIRTH,
                                    CC.GENDER,
                                    CC.RELIGION,
                                    CC.COMMUNITY,
                                    CASTE,
                                    MOTHER_NAME,
                                    PHOTO,
                                    PROGRAMME_GROUP_ID,
                                    P_DOOR_DEATAIL,
                                    P_STREET,
                                    P_VILLAGE_AREA,
                                    P_POST_PLACE_TOWN,
                                    P_TALUK,
                                    P_CITY,
                                    P_DISTRICT,
                                    P_PINCODE,
                                    P_STATE,
                                    SS.STATE_NAME,
                                    P_CONUNTRY,
                                    SC.COUNTRY_NAME,
                                    BATCH,
                                    FATHER_NAME,
                                    STUDENT_MOBILE_NO,
                                    PARENT_MOBILE,
                                    EMAIL_ID,
                                    AADHAR_NO,
                                    YEAR_OF_JOIN,
                                    YEAR_OF_PASSING,
                                    IS_DISABLED,
                                    REGISTER_DATE,
                                    ACADEMIC_YEAR
                                FROM
                                    CONVOCATION_APPLICATION_REGISTER AS CC
                                        INNER JOIN
                                    SUP_GENDER AS GE ON GE.GENDER_ID = CC.GENDER
                                        INNER JOIN
                                    SUP_COMMUNITY AS COM ON COM.COMMUNITYID = CC.COMMUNITY
                                        INNER JOIN
                                    SUP_STATE SS ON SS.STATE_ID = CC.P_STATE
                                        INNER JOIN
                                    SUP_COUNTRY AS SC ON SC.COUNTRY_ID = CC.P_CONUNTRY
                                WHERE
                                         CC.IS_DELETED != 1
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentAdmittedByCurrentBatch:
                    {
                        query = @"SELECT 
                                STUDENT_ID, CURRENT_BATCH AS BATCH, AC_YEAR as ACADEMIC_YEAR
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                ACADEMIC_YEAR AS AY ON AY.ACADEMIC_YEAR_ID = CURRENT_BATCH
                            WHERE
                                STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.Convocationnadreport:
                    {
                        query = @"SELECT 
                                '027' AS CENTER_NO,
                                SP.REGISTER_NO,
                                CAR.FIRST_NAME,
                                CAR.TAMIL_NAME1,
                                CP.PROGRAMME_NAME,
                                SG.GENDER_NAME,
                                DATE_FORMAT(CAR.DATE_OF_BIRTH, '%Y/%m/%d') AS DATE_OF_BIRTH,
                                CAR.CASTE,
                                SCC.COMMUNITY,
                                CAR.FATHER_NAME,
                                '' AS DOA,
                                CAR.STUDENT_MOBILE_NO,
                                SR.RELIGION,
                                '' AS IS_DISABLED,
                                CAR.EMAIL_ID,
                                CAR.YEAR_OF_PASSING,
                                CAR.AADHAR_NO,
                                FT.COLLECTED AS AMOUNT
                            FROM
                                CONVOCATION_APPLICATION_REGISTER AS CAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CAR.STUDENT_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    LEFT JOIN
                                FEE_TRANSACTION AS FT ON FT.STUDENT_ID = CAR.STUDENT_ID
                                    AND FT.FREQUENCY = 17
                                    INNER JOIN
                                sup_gender AS SG ON SG.GENDER_ID = CAR.GENDER
                                    INNER JOIN
                                sup_community AS SCC ON SCC.COMMUNITYID = CAR.COMMUNITY
                                    INNER JOIN
                                sup_religion AS SR ON SR.RELIGIONID = CAR.RELIGION
                                    INNER JOIN
                                sup_nationality AS SN ON SN.NATIONALITY_ID = CAR.P_CONUNTRY
                            WHERE
                                CAR.IS_DELETED != 1
                            GROUP BY CAR.STUDENT_ID;";
                        break;
                    }


                #region Student Registration Card
                case StudentSQLCommads.FetchCurrentSemRegularPaperByStudentId:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    CS.COURSE_STRUCTURE_ID,
                                    CCR.COURSE_ROOT_ID,
                                    CCR.COURSE_CODE,
                                    CCR.CREDITS,
                                    UPPER(CCR.COURSE_TITLE) AS 'COURSE_TITLE',
                                    CCR.IS_COMPULSORY,
                                    SC.CLASS_ID,
                                    CCR.SEMESTER_ID,
                                    CCR.SUBJECT_TYPE AS 'SUBJECT_TYPE_ID',
                                    SS.SUBJECT_TYPE,
                                    CP.ACADEMIC_YEAR AS 'AC_YEAR',
                                    SP.PART,
								    UPPER(STF.STAFF_CODE) AS STAFF_CODE,
                                       CONCAT(IFNULL(STU.FIRST_NAME,''),' ',IFNULL(STU.LAST_NAME,'')) AS FIRST_NAME,
                                    STU.ROLL_NO,
                                    C.CLASS_NAME,
									P.PROGRAMME_NAME
                                FROM
                                    CP_COURSE_STRUCTURE AS CS
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CP ON CP.COURSE_STRUCTURE_ID = CS.COURSE_STRUCTURE_ID
                                        AND CP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                        AND CCR.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                        AND PG.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                        AND P.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                        AND C.IS_DELETED != 1 AND C.CLASS_YEAR=1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.CLASS_ID = C.CLASS_ID
                                        AND SC.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                        AND SC.IS_DELETED != 1
                                        
                                        INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON STU.STUDENT_ID=SC.STUDENT_ID
                                        AND STU.IS_DELETED!=1
                                        AND STU.IS_LEFT!=1
                                        
                                        
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                            ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR
                                        WHERE
                                            ACTIVE_YEAR = 1 AND IS_DELETED != 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS S ON S.SEMESTER = CCR.SEMESTER_ID
                                        AND S.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                        AND S.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND S.BATCH = AB.BATCH_ID
                                        AND S.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_SUBJECT_TYPE AS SS ON SS.SUBJECT_TYPE_ID = CCR.SUBJECT_TYPE
                                        AND SS.IS_DELETED != 1
                                        LEFT JOIN
                                        CP_CLASS_COURSE_STAFF AS CSS ON CSS.COURSE_ID=CCR.COURSE_ROOT_ID
                                        AND CSS.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                        AND CSS.SEMESTER_ID=S.SEMESTER
                                        AND CSS.IS_DELETED!=1
                                         AND CSS.IS_PRIMARY_STAFF=1
                                        AND CSS.CLASS_ID=SC.CLASS_ID
                                         
                                         
                                        LEFT JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=CSS.STAFF_ID
                                        AND STF.IS_DELETED!=1


                                        LEFT JOIN
                                        SUP_PART AS SP ON SP.PART_ID=CCR.PART 
                                        AND SP.IS_DELETED!=1
                                WHERE
                                    CP.ACADEMIC_YEAR =?AC_YEAR
                                        AND S.IS_ACTIVE = 1
                                        AND SC.STUDENT_ID =?STUDENT_ID
                                        AND CS.ACADEMIC_YEAR =?AC_YEAR
                                        AND CS.IS_DELETED != 1
                                GROUP BY CCR.COURSE_CODE;";
                        break;
                    }
                case StudentSQLCommads.FetchCurrentSemOptionalPaperByStudentId:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    CCR.COURSE_ROOT_ID,
                                    CCR.COURSE_CODE,
                                     CCR.CREDITS,
                                    UPPER(CCR.COURSE_TITLE) AS 'COURSE_TITLE',
                                    CCR.IS_COMPULSORY,
                                    SC.S_CLASS_ID AS CLASS_ID,
                                    CCR.SEMESTER_ID,
                                    CCR.SUBJECT_TYPE AS 'SUBJECT_TYPE_ID',
                                    SS.SUBJECT_TYPE,
                                    SC.ACADEMIC_YEAR AS 'AC_YEAR',
                                    SP.PART,
                                     UPPER(STF.STAFF_CODE) AS STAFF_CODE
                                FROM
                                    STU_COURSE_INFO AS SC
                                        INNER JOIN
                                    STU_CLASS AS S ON S.STUDENT_ID = SC.STUDENT_ID
                                        AND S.IS_DELETED != 1
                                        AND S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
										INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID AND C.IS_DELETED != 1
										INNER JOIN
									ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
									AND AB.ACADEMIC_YEAR_ID = (SELECT 
										ACADEMIC_YEAR_ID
									FROM
										ACADEMIC_YEAR
									WHERE
										ACTIVE_YEAR = 1 AND IS_DELETED != 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER = CCR.SEMESTER_ID
                                        AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND SEM.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND SEM.BATCH = AB.BATCH_ID
                                        LEFT JOIN
                                    SUP_SUBJECT_TYPE AS SS ON SS.SUBJECT_TYPE_ID = CCR.SUBJECT_TYPE
                                        AND SS.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_CLASS_COURSE_STAFF AS CSS ON CSS.COURSE_ID=CCR.COURSE_ROOT_ID
                                        AND CSS.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                        AND CSS.SEMESTER_ID=SEM.SEMESTER
                                        AND CSS.IS_DELETED!=1
                                        AND CSS.IS_PRIMARY_STAFF=1
                                        AND CSS.CLASS_ID=S.CLASS_ID
                                        
                                        LEFT JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=CSS.STAFF_ID
                                        AND STF.IS_DELETED!=1
                                        

                                        LEFT JOIN
                                        SUP_PART AS SP ON SP.PART_ID=CCR.PART 
                                        AND SP.IS_DELETED!=1
                                        
                                WHERE
                                    SC.STUDENT_ID =?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SEM.IS_ACTIVE = 1
                                        AND CCR.IS_DELETED != 1
                                        AND SC.IS_DELETED != 1;";
                        break;

                    }
                case StudentSQLCommads.FetchClassTeacherByStudentId:
                    {
                        query = @" SELECT 
                                    STU.STUDENT_ID,
                                    UPPER(STF.STAFF_CODE) AS STAFF_CODE
    
                                FROM
                                    STU_PERSONAL_INFO AS STU
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        INNER JOIN
                                    CLASS_INCHARGE AS CI ON CI.CLASS_ID = SC.CLASS_ID
                                        AND CI.IS_DELETED != 1
                                        AND CI.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
        
                                        INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID=CI.STAFF_ID
                                        AND STF.IS_DELETED!=1
                                WHERE
                                    STU.STUDENT_ID =?STUDENT_ID
                                        AND STU.IS_DELETED != 1
                                        AND STU.IS_LEFT != 1;";
                        break;

                    }
                case StudentSQLCommads.FetchHodByStudentId:
                    {
                        query = @" SELECT 
                                        STU.STUDENT_ID,
                                        UPPER(STF.STAFF_CODE) AS STAFF_CODE,
                                        STF.STAFF_ID,
                                        STF.FIRST_NAME
    
    
                                    FROM
                                        STU_PERSONAL_INFO AS STU
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
       
		                                    INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID=SC.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED!=1
        
                                            INNER JOIN 
                                            CP_PROGRAMME AS CP ON CP.PROGRAMME_ID=CPP.PROGRAMME_ID
                                            AND CP.IS_DELETED!=1
        
                                            INNER JOIN
                                            CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID=CP.DEPARTMENT
                                            AND CD.IS_DELETED!=1
        
                                            INNER JOIN
                                            STF_PERSONAL_INFO AS STF ON STF.DEPARTMENT=CD.DEPARTMENT_ID
                                            AND STF.IS_DELETED!=1
        
                                            INNER JOIN
        
                                             USER_ROLES_BY_ACADEMIC_YEAR AS URA ON URA.USER_ID=STF.STAFF_ID
                                             AND URA.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                             AND URA.USER_TYPE=4
                                             AND URA.IS_DELETED!=1
                                             AND URA.ROLE_ID=7
       
                                    WHERE
                                        STU.STUDENT_ID =?STUDENT_ID
                                            AND STU.IS_DELETED != 1
                                            AND STU.IS_LEFT != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchCurrentDate:
                    {
                        query = @"SELECT DATE_FORMAT(DAY_ORDER_DATE,'%Y-%m-%d') AS DAY_ORDER_DATE FROM CALENDER WHERE DATE_FORMAT(curdate(),'%Y-%m-%d')=DAY_ORDER_DATE;";
                        break;
                    }
                #endregion


                case StudentSQLCommads.FetchStudentFeeDetailsByFrequencyId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.FIRST_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        SC.CLASS_ID,
                                        FS.FREQUENCY_ID,
                                        FS.HEAD,
                                        FS.CREDIT AS 'DEBIT',
                                        FS.FEE_MAIN_HEAD_ID,
                                        COUNT(FS.HEAD) AS 'COUNT'
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        FEE_STUDENT_ACCOUNT AS FS ON FS.STUDENT_ID = SC.STUDENT_ID
                                            AND FS.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.STUDENT_ID , FS.HEAD
                                    HAVING COUNT(FS.HEAD) = 1;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentByDepartmentId:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,SC.CLASS_ID,CC.DEPARTMENT
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                            WHERE
                                CC.DEPARTMENT =?DEPARTMENT
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchAssignmentCourses:
                    {
                        query = @"SELECT 
                            AG.COURSE_ID AS COURSE_ROOT_ID, CCR.COURSE_TITLE
                        FROM
                            STU_CLASS AS SC
                                INNER JOIN
                            ASSIGNMENT_2019 AS AG ON SC.CLASS_ID = SC.CLASS_ID
                                INNER JOIN
                            CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = AG.COURSE_ID
                        WHERE
                            SC.STUDENT_ID =?STUDENT_ID
                                AND SC.ACADEMIC_YEAR = ?AC_YEAR GROUP BY AG.COURSE_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchAssignmentByCourse:
                    {
                        query = @"SELECT 
                                AG.ASSIGNMENT_ID, AG.ASSIGNMENT_TITLE
                            FROM
                                ASSIGNMENT_2019 AS AG
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = AG.COURSE_ID
                            WHERE
                                AG.COURSE_ID = ?COURSE_ID
                                    AND AG.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentAppliedList:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STUDENT_ID,
                                SLA.LEAVE_TYPE_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                SLA.DESCRIPTION,
                                SLA.STATUS AS STATUS_ID,
                                IF(SLA.STATUS = 0,
                                'Pending',
                                IF(SLA.STATUS = 1,
                                    'Approved',
                                    'Rejected')) AS STATUS
                            FROM
                                STU_LEAVE_APPLY AS SLA
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SLA.STUDENT_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS LT ON LT.LEAVE_TYPE_ID = SLA.LEAVE_TYPE_ID
                            WHERE
                                SLA.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SLA.STUDENT_ID =?STUDENT_ID
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchLeavetype:
                    {
                        query = @"SELECT 
                                LEAVE_TYPE_ID, LEAVE_TYPE
                            FROM
                                SUP_LEAVE_TYPE
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchLeaveAppliedById:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STUDENT_ID,
                                SLA.LEAVE_TYPE_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                 SLA.STATUS AS STATUS_ID,
                                SLA.DESCRIPTION, IF(SLA.STATUS = 0,
                                'Pending',
                                IF(SLA.STATUS = 1,
                                    'Approved',
                                    'Rejected')) AS STATUS
                            FROM
                                STU_LEAVE_APPLY AS SLA
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SLA.STUDENT_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS LT ON LT.LEAVE_TYPE_ID = SLA.LEAVE_TYPE_ID
                            WHERE
                                
                                     SLA.LEAVE_APPLY_ID =?LEAVE_APPLY_ID
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.SaveStudentleaveapply:
                    {
                        query = @"INSERT INTO 
                                STU_LEAVE_APPLY(STUDENT_ID, 
                                LEAVE_TYPE_ID,
                                 FROM_DATE, TO_DATE, DESCRIPTION, 
                                 ACADEMIC_YEAR)
                                 VALUES(?STUDENT_ID, 
                                ?LEAVE_TYPE_ID,
                                 ?FROM_DATE, ?TO_DATE, ?DESCRIPTION, 
                                 ?ACADEMIC_YEAR);";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentleaveapply:
                    {
                        query = @"UPDATE STU_LEAVE_APPLY
                                 SET LEAVE_TYPE_ID=?LEAVE_TYPE_ID,
                                 FROM_DATE=?FROM_DATE,
                                 TO_DATE=?TO_DATE,
                                 DESCRIPTION=?DESCRIPTION 
                                 WHERE LEAVE_APPLY_ID=?LEAVE_APPLY_ID;";
                        break;
                    }
                case StudentSQLCommads.DeleteAppliedLeave:
                    {
                        query = @"UPDATE STU_LEAVE_APPLY SET IS_DELETED=1 WHERE LEAVE_APPLY_ID=?LEAVE_APPLY_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchLeaveAppliedStudent:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.FIRST_NAME,
                                SP.PHOTO,
                                CC.CLASS_NAME,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STUDENT_ID,
                                SLA.LEAVE_TYPE_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE, '%Y/%m/%d') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE, '%Y/%m/%d') AS TO_DATE,
                                SLA.DESCRIPTION,
                                SLA.STATUS AS STATUS_ID,
                                IF(SLA.STATUS = 0,
                                    'Pending',
                                    IF(SLA.STATUS = 1,
                                        'Approved',
                                        'Rejected')) AS STATUS
                            FROM
                                STU_LEAVE_APPLY AS SLA
                                    INNER JOIN
                                stu_class AS SC ON SC.STUDENT_ID = SLA.STUDENT_ID
                                    INNER JOIN
                                cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS LT ON LT.LEAVE_TYPE_ID = SLA.LEAVE_TYPE_ID
                            WHERE
		                            SLA.ACADEMIC_YEAR =?AC_YEAR
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchLeaveAppliedDetails:
                    {
                        query = @"SELECT 
                                SP.ROLL_NO,
                                SP.FIRST_NAME,
                                SP.PHOTO,
                                CC.CLASS_NAME,
                                SLA.LEAVE_APPLY_ID,
                                SLA.STUDENT_ID,
                                SLA.LEAVE_TYPE_ID,
                                LT.LEAVE_TYPE,
                                DATE_FORMAT(SLA.FROM_DATE, '%Y/%m/%d') AS FROM_DATE,
                                DATE_FORMAT(SLA.TO_DATE, '%Y/%m/%d') AS TO_DATE,
                                SLA.DESCRIPTION,
                                 SLA.STATUS AS STATUS_ID,
                                IF(SLA.STATUS = 0,
                                    'Pending',
                                    IF(SLA.STATUS = 1,
                                        'Approved',
                                        'Rejected')) AS STATUS
                            FROM
                                STU_LEAVE_APPLY AS SLA
                                    INNER JOIN
                                stu_class AS SC ON SC.STUDENT_ID = SLA.STUDENT_ID
                                    INNER JOIN
                                cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                SUP_LEAVE_TYPE AS LT ON LT.LEAVE_TYPE_ID = SLA.LEAVE_TYPE_ID
                            WHERE
		                            SLA.ACADEMIC_YEAR =?AC_YEAR
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SLA.LEAVE_APPLY_ID =?LEAVE_APPLY_ID
                                    AND SLA.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchLeaveStatus:
                    {
                        query = @"SELECT 
                                LEAVE_STATUS_ID, LEAVE_STATUS
                            FROM
                                SUP_LEAVE_STATUS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.UpdateLeaveStatus:
                    {
                        query = @"UPDATE STU_LEAVE_APPLY 
                                    SET 
                                        STATUS =?STATUS
                                    WHERE
                                        LEAVE_APPLY_ID =?LEAVE_APPLY_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentMarks:
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
                                CONCAT(CR.COURSE_CODE, ':', CR.COURSE_TITLE) COURSE_TITLE,
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
                                SEM.GRADE,
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
                                    ST.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND ST.STUDENT_ID =?STUDENT_ID
                            ORDER BY SP.REGISTER_NO;";
                        break;
                    }
                case StudentSQLCommads.SaveCertificateapply:
                    {
                        query = @"INSERT INTO 
                                STU_CERTIFICATE_APPLY(STUDENT_ID, 
                                HEAD_ID, FEE_MAINHEAD_ID
                                ,ACADEMIC_YEAR) VALUES(?STUDENT_ID, 
                                ?HEAD_ID, ?FEE_MAINHEAD_ID
                                ,?ACADEMIC_YEAR);";
                        break;
                    }
                case StudentSQLCommads.FetchCertificatefeestructure:
                    {
                        query = @"SELECT 
                                    FS.FREQUENCY AS FREQUENCY_ID,
                                    FS.HEAD,
                                    FS.AMOUNT AS CREDIT,
                                    FS.ACADEMIC_YEAR,
                                    FS.FEE_MAIN_HEAD_ID
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_MAIN_HEADS AS FM ON FM.FEE_MAIN_HEAD_ID =?FEE_MAIN_HEAD_ID
                                        AND FM.IS_DELETED != 1
                                        INNER JOIN
                                    FEE_STRUCTURE AS FS ON FS.PROGRAMME = CPP.PROGRAMME_GROUP_ID
                                        AND FS.FREQUENCY = FM.FREQUENCY_ID
                                        AND FS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND FS.FEE_MAIN_HEAD_ID = FM.FEE_MAIN_HEAD_ID
                                        AND FS.IS_DELETED != 1
                                WHERE
                                    SC.STUDENT_ID =?STUDENT_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.SaveFeeStudentAc:
                    {
                        query = @"INSERT INTO FEE_STUDENT_ACCOUNT
                                  (
	                                  STUDENT_ID,
	                                  ACADEMIC_YEAR,
	                                  FREQUENCY_ID,
	                                  HEAD,
	                                  CREDIT,
	                                  FEE_MAIN_HEAD_ID
                                  )
                                  VALUES
                                  (
	                                  ?STUDENT_ID,
	                                  ?AC_YEAR,
	                                  ?FREQUENCY_ID,
	                                  ?HEAD,
	                                  ?CREDIT,
	                                  ?FEE_MAIN_HEAD_ID
                                  );";
                        break;
                    }
                case StudentSQLCommads.FetchCertificateForDDL:
                    {
                        query = @"SELECT 
                                FMH.FEE_MAIN_HEAD_ID, FMH.HEAD_ID, SH.HEAD
                            FROM
                                FEE_MAIN_HEADS AS FMH
                                    INNER JOIN
                                FEE_FREQUENCY_FEE_MODE AS FFM ON FFM.FEE_FREQUENCY_FEE_MODE_ID = FMH.FREQUENCY_ID
                                    INNER JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                            WHERE
                                FFM.FEE_MODE = ?FEE_MODE
                                    AND FFM.ACADEMIC_YEAR =?AC_YEAR
                                    AND FMH.PROGRAMME_MODE =?PROGRAMME_MODE
                                    AND FMH.SHIFT = ?SHIFT AND FMH.IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchCalendarmonths:
                    {
                        query = @"SELECT 
                                    DAY_ID, DAY_ORDER_DATE, DAY_ORDER_MONTH
                                FROM
                                    CALENDER AS CAL
                                WHERE
                                    CAL.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CAL.DAY_ORDER_DATE BETWEEN (SELECT 
                                            DATE_FROM
                                        FROM
                                            ACADEMIC_SEMESTER
                                        WHERE
                                            PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID AND BATCH =?BATCH_ID
                                                AND ACADEMIC_YEAR = ?AC_YEAR
                                                AND IS_DELETED != 1
                                                AND IS_ACTIVE = 1) AND (SELECT 
                                            DATE_TO
                                        FROM
                                            ACADEMIC_SEMESTER
                                        WHERE
                                            PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID AND BATCH =?BATCH_ID
                                                AND ACADEMIC_YEAR = ?AC_YEAR
                                                AND IS_DELETED != 1
                                                AND IS_ACTIVE = 1) AND IS_DELETED!=1
                                GROUP BY DAY_ORDER_MONTH
                                ORDER BY DAY_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCalendar:
                    {
                        query = @"SELECT 
                                DAY_ID,
                                date_format(DAY_ORDER_DATE,'%Y-%m-%d') AS DAY_ORDER_DATE,
                                DAY_ORDER,
                                SDO.DAY,
                                DAY_ORDER_MONTH,
                                IS_HOLIDAY,
                                DAYNAME(DAY_ORDER_DATE) AS DAY_NAME,
                                IF(DATE(DAY_ORDER_DATE) < DATE(NOW()),0,1) AS DAY_TYPE
                            FROM
                                CALENDER AS CAL
                                    LEFT JOIN
                                sup_day_order AS SDO ON CAL.DAY_ORDER = SDO.DAY_ORDER_ID
                            WHERE
                                CAL.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CAL.DAY_ORDER_DATE BETWEEN (SELECT 
                                        DATE_FROM
                                    FROM
                                        ACADEMIC_SEMESTER
                                    WHERE
                                        PROGRAMME_GROUP_ID =?PROGRAMME_GROUP_ID AND BATCH = ?BATCH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1
                                            AND IS_ACTIVE = 1) AND (SELECT 
                                        DATE_TO
                                    FROM
                                        ACADEMIC_SEMESTER
                                    WHERE
                                        PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID AND BATCH = ?BATCH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1
                                            AND IS_ACTIVE = 1)
                                    AND CAL.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchDaywiseStudentAttendance:
                    {
                        query = @"SELECT 
                                AL.HOUR_FROM, AL.HOUR_TO, SAT.ATTENDANCE_TYPE,date_format(ATTENDENCE_DATE,'%d-%m-%Y') AS ATTENDENCE_DATE
                            FROM
                                ATT_ABSENTEE_LIST AS AL
                                    INNER JOIN
                                SUP_ATTENDANCE_TYPE AS SAT ON SAT.ATTENDANCE_TYPE_ID = AL.ATTENDANCE_TYPE_ID
                            WHERE
                                AL.ATTENDENCE_DATE = ?ATTENDENCE_DATE
                                    AND AL.STUDENT_ID = ?STUDENT_ID
                                    AND AL.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchAllAdmissionRegisterByProgrammeMode:
                    {
                        query = @"SELECT 
                                    
                                    SP.ROLL_NO,
                                    SP.ADMISSION_NO,
                                    TC.CERTIFICATE_ID,
                                    TC.STUDENT_ID,
                                    SP.PROGRAM_ID,
                                    UPPER(TC.FIRST_NAME) AS 'FIRST_NAME',
                                    UPPER(TC.GENDER) AS 'GENDER',
                                    UPPER(TC.NATIONALITY) AS 'NATIONALITY',
                                    UPPER(TC.RELIGION) AS 'RELIGION',
                                    UPPER(SP.CASTE) AS 'CASTE',
                                    UPPER(TC.COMMUNITY) AS 'COMMUNITY',
                                    DATE_FORMAT(TC.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                    UPPER(TC.DATETOWORDS) AS 'DATETOWORDS',
                                    DATE_FORMAT(TC.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                    UPPER(TC.ADMITTED_CLASS) AS 'ADMITTED_CLASS',
                                    CONCAT('1. ',
                                            ' ',
                                            UPPER(TC.IDENTIFICATION_MARK1)) AS 'IDENTIFICATION_MARK1',
                                    CONCAT('2. ',
                                            ' ',
                                            UPPER(TC.IDENTIFICATION_MARK2)) AS 'IDENTIFICATION_MARK2',
                                    UPPER(TC.MAIN_COURSE) AS 'MAIN_COURSE',
                                    UPPER(TC.ALLIED) AS 'ALLIED',
                                    UPPER(O.OPTION_NAME) AS 'FEE_PAID',
                                    UPPER(SO.OPTION_NAME) AS 'SCHOLAR_SHIP',
                                    DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                                    UPPER(TC.REASON_FOR_LEAVING) AS 'REASON_FOR_LEAVING',
                                    UPPER(TC.LEAVING_CLASS) AS 'LEAVING_CLASS',
                                    UPPER(SC.CONDUCT_NAME) AS 'CONDUCT',
                                    DATE_FORMAT(TC.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                                    DATE_FORMAT(TC.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE',
                                    IF(TC.FATHER_NAME IS NULL
                                            || TC.FATHER_NAME = '',
                                        IF(TC.GUARDIAN_NAME IS NULL
                                                || TC.GUARDIAN_NAME = '',
                                            '----',
                                            UPPER(TC.GUARDIAN_NAME)),
                                        UPPER(TC.FATHER_NAME)) AS 'FATHER_NAME',
                                      UPPER(OC.OCCUPATION_NAME) AS FR_OCCUPATION,
                                    SP.ANNUAL_INCOME,
                                    UPPER(TC.ACADEMIC_YEARS) AS 'ACADEMIC_YEARS',
                                    UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                                    UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE',
                                    CONTACT_NO,
                                    UPPER(TC.ADDRESS) AS ADDRESS,
                                    UPPER(MEDIUM_OF_INSTRUCTION) AS MEDIUM_OF_INSTRUCTION,
                                    UPPER(STUDIED_FROM) AS STUDIED_FROM,
                                    '~/Content/hcc_img/sign/Principal-E-Sign.png' AS PRINCIPAL_SIGN
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_TRANSFER_CERTIFICATE AS TC ON TC.STUDENT_ID = SP.STUDENT_ID
                                        LEFT JOIN
                                    SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                                        LEFT JOIN
                                    SUP_OPTION AS SO ON SO.OPTION_ID = TC.SCHOLAR_SHIP
                                        LEFT JOIN
                                    SUP_CONDUCT AS SC ON SC.CONDUCT_ID = TC.CONDUCT
                                         LEFT JOIN
                                    SUP_OCCUPATION AS OC ON OC.OCCUPATION_ID = SP.FR_OCCUPATION
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID 
                                WHERE
                                    PG.APPTYPE_ID IN (?APPTYPE_ID)	
                                        AND SP.BATCH=?BATCH
                                        AND PG.PROGRAMME_MODE = ?PROGRAMME_MODE 
                                        AND PG.SHIFT = ?SHIFT
                                        AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchOldTransferCertificateForEdit:
                    {
                        query = @"SELECT 
                                TC.ADMISSION_NO AS 'ROLL_NO',
                                TC.CERTIFICATE_ID,
                                TC.STUDENT_ID,
                                UPPER(TC.FIRST_NAME) AS 'FIRST_NAME',
                                UPPER(TC.GENDER) AS 'GENDER',
                                UPPER(TC.NATIONALITY) AS 'NATIONALITY',
                                UPPER(TC.RELIGION) AS 'RELIGION',
                                UPPER(TC.CASTE) AS CASTE,
                               UPPER(TC.COMMUNITY) AS 'COMMUNITY',
                               DATE_FORMAT(TC.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                               UPPER(TC.DATETOWORDS) AS DATETOWORDS,
                                CONCAT(DATE_FORMAT(TC.ADMISSION_DATE, '%d/%m/%Y'),
                                        ' - ',
                                        UPPER(TC.ADMITTED_CLASS)) AS 'ADMITTED_DATE',
                                CONCAT('1. ',
                                        ' ',
                                        UPPER(TC.IDENTIFICATION_MARK1)) AS 'IDENTIFICATION_MARK1',
                                CONCAT('2. ',
                                        ' ',
                                        UPPER(TC.IDENTIFICATION_MARK2)) AS 'IDENTIFICATION_MARK2',
                                UPPER(TC.MAIN_COURSE) AS 'MAIN_COURSE',
                                UPPER(TC.ALLIED) AS 'ALLIED',
	                            TC.FEE_PAID,
	                            TC.SCHOLAR_SHIP,
                                DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                                UPPER(TC.REASON_FOR_LEAVING) AS 'REASON_FOR_LEAVING',
                                UPPER(TC.LEAVING_CLASS) AS 'LEAVING_CLASS',
                                TC.CONDUCT,
                                DATE_FORMAT(TC.TC_APPLIED_DATE, '%d/%m/%Y') AS 'TC_APPLIED_DATE',
                                DATE_FORMAT(TC.TC_GIVEN_DATE, '%d/%m/%Y') AS 'TC_GIVEN_DATE',
                                IF(TC.FATHER_NAME IS NULL
                                        || TC.FATHER_NAME = '',
                                    IF(TC.GUARDIAN_NAME IS NULL
                                            || TC.GUARDIAN_NAME = '',
                                        '----',
                                        UPPER(TC.GUARDIAN_NAME)),
                                    UPPER(TC.FATHER_NAME)) AS 'FATHER_NAME',
                                UPPER(TC.ACADEMIC_YEARS) AS 'ACADEMIC_YEARS',
                                UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                                UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE',
                                DATE_FORMAT(TC.ADMISSION_DATE, '%d/%m/%Y') AS 'ADMISSION_DATE',
	                            TC.ADMITTED_CLASS
                            FROM
                                STU_OLD_TRANSFER_CERTIFICATE AS TC
                                    LEFT JOIN
                                SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                                    LEFT JOIN
                                SUP_OPTION AS SO ON SO.OPTION_ID = TC.SCHOLAR_SHIP
                                    LEFT JOIN
                                SUP_CONDUCT AS SC ON SC.CONDUCT_ID = TC.CONDUCT
                            WHERE
                                ADMISSION_NO = ?ADMISSION_NO;";
                        break;
                    }
                case StudentSQLCommads.IsAdmissionnoExist:
                    {
                        query = @"SELECT 
                                CERTIFICATE_ID
                            FROM
                                STU_OLD_TRANSFER_CERTIFICATE
                            WHERE
                                ADMISSION_NO = ?ADMISSION_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentListForMandatoryFields:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID, SP.ROLL_NO,SP.FIRST_NAME  AS STUDENT_NAME,CL.CLASS_NAME AS CLASS
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        AND PG.APPTYPE_ID = 1
                                        AND PG.PROGRAMME_MODE = 1
                                        AND PG.SHIFT = 1
                                        INNER JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = 2019
                                        AND SC.IS_DELETED != 1 ORDER BY PG.PROGRAMME_GROUP_ID,CL.CLASS_YEAR, SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentMandatoryFields:
                    {
                        query = @"SELECT 
	                            SP.STUDENT_ID,
                                SP.ADMISSION_NO,
                                DATE_FORMAT(SP.ADMISSION_DATE,'%d/%m/%Y') AS ADMISSION_DATE,
                                SP.ADMITTED_CLASS,
                                SP.DEPT_ID,
                                SP.PROGRAM_ID,
                                SP.GENDER,
                                DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                SP.BLOOD_GROUP,
                                SP.RELIGION,
                                SP.NATIONALITY,
                                SP.FATHER_NAME,
                                SP.GUARDIAN_NAME,
                                SP.FATHER_NAME,
                                SP.MOTHER_NAME,
                                SP.ANNUAL_INCOME,
                                SP.PHOTO,
                                SP.FR_MOBILE,
                                SP.FIRST_NAME,
                                SP.FIRST_LANGUAGE,
                                SP.COMMUNITY,
                                SP.CASTE,
                                SP.REGISTER_NO,
                                SP.ROLL_NO,
                                SP.STU_MOBILENO,
                                SP.STU_EMAILID,
                                SP.BATCH,
                                SP.CURRENT_BATCH,
                                SP.ACCOUNT_NO,
                                SP.AADHAR_NUMBER,
                                SP.RFID,
                                SP.BAR_CODE,
                                SP.BIOD_NUMBER,
                                SP.ADDRESS
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    AND PG.APPTYPE_ID = 1
                                    AND PG.PROGRAMME_MODE = 1
                                    AND PG.SHIFT = 1
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                            WHERE
                                SC.ACADEMIC_YEAR = 2019
                                    AND SC.IS_DELETED != 1
                            ORDER BY PG.PROGRAMME_GROUP_ID , CL.CLASS_YEAR , SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchCertificateApplied:
                    {
                        query = @"SELECT 
                                    A.APPLY_ID,
                                    SPI.ROLL_NO,
                                    H.HEAD,
                                    DATE_FORMAT(A.APPLY_DATE, '%d/%m/%Y') AS APPLY_DATE,
                                    FS.DEBIT,
                                    DATE_FORMAT(A.APPROVED_DATE, '%d/%m/%Y') AS APPROVED_DATE,
                                    A.STATUS,
                                    IFNULL(S.STATUS_NAME, 'Pending') AS STATUS_NAME
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    STU_CERTIFICATE_APPLY AS A ON A.STUDENT_ID = SC.STUDENT_ID
                                        AND A.IS_DELETED != 1
                                        AND A.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_HEAD AS H ON H.HEAD_ID = A.HEAD_ID
                                        AND H.IS_DELETED != 1
                                        LEFT JOIN
                                    FEE_STUDENT_ACCOUNT AS FS ON FS.STUDENT_ID = SC.STUDENT_ID
                                        AND FS.IS_DELETED != 1
                                        AND FS.FEE_MAIN_HEAD_ID = A.FEE_MAINHEAD_ID
                                        AND FS.HEAD = A.HEAD_ID
                                        AND FS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        LEFT JOIN
                                    SUP_STATUS AS S ON S.STATUS_ID = A.STATUS
                                WHERE
                                    SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.STUDENT_ID =?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCertificateAppliedById:
                    {
                        query = @"SELECT 
                                    APPLY_ID,
                                    STUDENT_ID,
                                    HEAD_ID,
                                    FEE_MAINHEAD_ID,
                                    DATE_FORMAT(APPLY_DATE, '%d/%m/%Y') AS APPLY_DATE,
                                    STATUS,
                                   DATE_FORMAT(APPROVED_DATE, '%d/%m/%Y') AS APPROVED_DATE,
                                    APPROVER
                                FROM
                                    STU_CERTIFICATE_APPLY
                                WHERE
                                    APPLY_ID =?APPLY_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCertificateByStudentHead:
                    {
                        query = @"SELECT
                                    APPLY_ID,
                                    STUDENT_ID,
                                    HEAD_ID,
                                    FEE_MAINHEAD_ID
                                    FROM 
                                    STU_CERTIFICATE_APPLY  
                                    WHERE 
                                    STUDENT_ID=?STUDENT_ID 
                                    AND FEE_MAINHEAD_ID=?FEE_MAINHEAD_ID
                                    AND ACADEMIC_YEAR=?AC_YEAR 
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchCertificateRequestedList:
                    {
                        query = @"SELECT 
                                SP.FIRST_NAME,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CC.CLASS_ID,
                                CC.CLASS_NAME,
                                C.APPLY_ID,
                                C.STUDENT_ID,
                                C.FEE_MAINHEAD_ID,
                                C.STATUS,
                                IFNULL(S.STATUS_NAME, 'Pending') AS STATUS_NAME,
                                DATE_FORMAT(C.APPLY_DATE, '%Y/%m/%d') AS APPLY_DATE,
                                DATE_FORMAT(C.APPROVED_DATE, '%Y/%m/%d') AS APPROVED_DATE,
                                SH.HEAD
                            FROM
                                STU_CERTIFICATE_APPLY AS C
                                    INNER JOIN
                                stu_class AS SC ON SC.STUDENT_ID = C.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN
                                cp_classes AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = C.HEAD_ID
                                    AND SH.IS_DELETED != 1
                                    LEFT JOIN
                                SUP_STATUS AS S ON S.STATUS_ID = C.STATUS
                            WHERE
                                C.IS_DELETED != 1
                                    AND C.ACADEMIC_YEAR =?AC_YEAR
                                    AND SC.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID);";
                        break;
                    }
                case StudentSQLCommads.UpdateCertificateRequestedStatus:
                    {
                        query = @"UPDATE
                                STU_CERTIFICATE_APPLY 
                                SET 
                                STATUS=?STATUS,
                                APPROVED_DATE=?APPROVED_DATE,
                                APPROVER=?APPROVER
                                WHERE 
                                APPLY_ID=?APPLY_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentInfoByStudentId:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(SPI.FIRST_NAME,''),' ',IFNULL(SPI.LAST_NAME,'')) AS FIRST_NAME,
                                    SPI.ROLL_NO,
                                    SPI.STUDENT_ID,
                                    CC.CLASS_NAME,
                                    CP.PROGRAMME_NAME,
                                    IFNULL(SPI.FATHER_NAME,'') AS FATHER_NAME,
                                    A.ACADEMIC_NAME,
                                    B.BEGIN_YEAR,
                                    B.BEGIN_YEAR + IF(CP.DURATION_UNIT IS NULL,0,CP.DURATION_UNIT) AS END_YEAR
    
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        AND CP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        AND CC.IS_DELETED != 1
        
                                        INNER JOIN 
                                        ACADEMIC_YEAR AS A ON A.AC_YEAR=SC.ACADEMIC_YEAR 
                                        AND A.IS_DELETED!=1
        
                                        INNER JOIN
        
                                        SUP_BATCH_DURATION AS B ON B.BATCH_ID=SC.BATCH
                                        AND B.APPLICATION_TYPE_ID=CPP.APPTYPE_ID
                                        AND B.IS_DELETED!=1
	
                                WHERE
                                    SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.STUDENT_ID =?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStuPersonalInfoForAdmissionCorrection:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.FIRST_NAME,
                                    SPI.ADMISSION_NO,
                                    SPI.REGISTER_NO,
                                    DATE_FORMAT(SPI.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                    SPI.YEAR_OF_JOIN,
                                    SPI.MAJOR,
                                    SPI.ALLIED_1,
                                    SPI.ALLIED_2,
                                    SPI.PART_1_LANGUAGE,
                                    SPI.MEDIUM_OF_INSTRUCTION,
                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                    SPI.RELIGION,
                                    SPI.COMMUNITY,
                                    SPI.CASTE,
                                    SPI.NATIONALITY,
                                    SPI.GUARDIAN_NAME,
                                    SPI.FR_OCCUPATION,
                                    SPI.ANNUAL_INCOME,
                                    SD.P_DOOR_DETAIL AS C_DOOR_DETAIL,
                                    SD.P_STREET AS C_STREET,
                                    SD.P_TALUK AS C_TALUK,
                                    SD.P_CITY AS C_CITY,
                                    SD.P_DISTRICT AS C_DISTRICT,
                                    SD.P_PINCODE AS C_PINCODE,
                                    SPI.STU_MOBILENO,
                                    SPI.LAST_SCHOOL_OR_COLLEGE,
                                    SPI.LAST_STUDIED_PLACE,
                                    SPI.TC_SERIAL_NO,
                                    SPI.SPECIALLY_ABLED,
                                    SPI.EX_SERVICE_MAN,
                                    SPI.IS_ANDAMAN_ORIGIN,
                                    SPI.IS_SPORTS,
                                    SPI.TOTAL_MARK,
                                    SPI.TOTAL_MARK_OBTAINED,
                                    SPI.CUT_OF_MARK,
                                    SPI.IDENTIFICATION_MARK1,
                                    SPI.IDENTIFICATION_MARK2,
                                    SPI.REMARKS,
                                    SPI.YEAR_OF_PASSING,
                                    SPI.MONTH_OF_PASSING,
                                    SPI.MARK_SHEET_NO,
                                    SPI.QUALIFICATION,
                                    SPI.BLOOD_GROUP,
                                    SPI.AADHAR_NUMBER,
                                    SPI.Q_REGISTER_NO
                                    
                                    
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        LEFT JOIN
                                    STU_ADDRESS AS SD ON SD.STUDENT_ID = SPI.STUDENT_ID
                                        AND SD.IS_DELETED != 1
                                WHERE
                                    SPI.STUDENT_ID =?STUDENT_ID
                                        AND SPI.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuPersonalInfoForAdmissionCorrection:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET FIRST_NAME=?FIRST_NAME,
						 ADMISSION_NO=?ADMISSION_NO,
						 REGISTER_NO=?REGISTER_NO,
						 ADMISSION_DATE=?ADMISSION_DATE,
                         YEAR_OF_JOIN=?YEAR_OF_JOIN,
                         MAJOR=?MAJOR,
                         ALLIED_1=?ALLIED_1,
                         ALLIED_2=?ALLIED_2,
                         PART_1_LANGUAGE=?PART_1_LANGUAGE,
                         MEDIUM_OF_INSTRUCTION=?MEDIUM_OF_INSTRUCTION,
                         DATE_OF_BIRTH=?DATE_OF_BIRTH,
                         RELIGION=?RELIGION,
                         COMMUNITY=?COMMUNITY,
                         CASTE=?CASTE,
                         NATIONALITY=?NATIONALITY,
                         GUARDIAN_NAME=?GUARDIAN_NAME,
                         FR_OCCUPATION=?FR_OCCUPATION,
                         ANNUAL_INCOME=?ANNUAL_INCOME,
                         STU_MOBILENO=?STU_MOBILENO,
                         LAST_SCHOOL_OR_COLLEGE=?LAST_SCHOOL_OR_COLLEGE,
                         LAST_STUDIED_PLACE=?LAST_STUDIED_PLACE,
                         TC_SERIAL_NO=?TC_SERIAL_NO,
                         SPECIALLY_ABLED=?SPECIALLY_ABLED,
                         EX_SERVICE_MAN=?EX_SERVICE_MAN,
                         IS_ANDAMAN_ORIGIN=?IS_ANDAMAN_ORIGIN,
                         IS_SPORTS=?IS_SPORTS,
                         TOTAL_MARK=?TOTAL_MARK,
                         TOTAL_MARK_OBTAINED=?TOTAL_MARK_OBTAINED,
                         CUT_OF_MARK=?CUT_OF_MARK,
                         IDENTIFICATION_MARK1=?IDENTIFICATION_MARK1,
                         IDENTIFICATION_MARK2=?IDENTIFICATION_MARK2,
                         REMARKS=?REMARKS,
                         YEAR_OF_PASSING=?YEAR_OF_PASSING,
                         MARK_SHEET_NO=?MARK_SHEET_NO,
                         MONTH_OF_PASSING=?MONTH_OF_PASSING,
                         Q_REGISTER_NO=?Q_REGISTER_NO,
                         AADHAR_NUMBER=?AADHAR_NUMBER,
                         BLOOD_GROUP=?BLOOD_GROUP
                         
                         WHERE 
                         STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuAddressForAdmissionCorrection:
                    {
                        query = @"UPDATE STU_ADDRESS SET P_DOOR_DETAIL=?P_DOOR_DETAIL,P_STREET=?P_STREET,P_TALUK=?P_TALUK,P_CITY=?P_CITY,P_DISTRICT=?P_DISTRICT,P_PINCODE=?P_PINCODE WHERE ADDRESS_ID=?ADDRESS_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchMajorWiseAlliedCourses:
                    {
                        query = @"SELECT MAJOR_ID, MAJOR, ALLIED1, ALLIED2, ACADEMIC_YEAR FROM MAJOR_WISE_ALLIED_COURSES WHERE IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.FetchMajorWiseAlliedCoursesById:
                    {
                        query = @"SELECT MAJOR_ID, MAJOR, ALLIED1, ALLIED2, ACADEMIC_YEAR FROM MAJOR_WISE_ALLIED_COURSES WHERE IS_DELETED!=1 AND MAJOR_ID=?MAJOR_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidateSemesterMarksforauxclg:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                SP.PHOTO,
                                SP.BAR_CODE,
                                DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                P.DESCRIPTION,
                                CR.COURSE_CODE,
                                CONCAT(IF(UGC.UGC_COURSE_TYPE IS NULL,'',CONCAT(UGC.UGC_COURSE_TYPE,': ')),                                        
                                        CR.CONS_COURSE_TITLE) COURSE_TITLE,
                                CR.COURSE_ORDER,
                                CR.SEMESTER_ID AS SEMESTER,
                                SS.SEMESTER_NAME,
                                SEM.INTERNAL_MARK,
                                SEM.EXTERNAL_MARK,
                                SEM.GRADE,
                                SEM.GRADE_POINT,
                                SEM.TOTAL,
                                IFNULL(CR.TOTAL, 100) AS MAXIMUM,
                                CP.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                CPG.PROGRAMME_GROUP_ID,
                                CPG.APPTYPE_ID as APPLICATION_TYPE_ID,
                                CR.CREDITS AS CREDIT,
                                SEM.M_PASS,
                                SEM.RESULT,
                                CR.UGC_COURSE_TYPE,
                                ES.M_EXAM,
                               DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') as START_DATE
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                SEMESTER_MARKS_?AC_YEAR AS SEM ON SEM.STUDENT_ID = SP.STUDENT_ID
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
                                    AND ES.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                            WHERE
                                SP.STUDENT_ID IN (?STUDENT_ID)
                                    AND UPPER(SEM.RESULT) != 'F'
                                    AND UPPER(SEM.RESULT) != 'A'
                            ORDER BY CR.PART , UGC.COURSE_ORDER , CR.SEMESTER_ID";
                        break;
                    }
                case StudentSQLCommads.FetchIndividualSemesterMarksforauxclg:
                    {
                        query = @"SELECT 
                            SM.STUDENT_ID,
                            SP.ROLL_NO,
                            SP.PHOTO,
                            SP.REGISTER_NO,
                            DATE_FORMAT(DATE_OF_BIRTH,'%d/%m/%Y') as DATE_OF_BIRTH,
                            SP.BAR_CODE,
                            CONCAT(IFNULL(FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(LAST_NAME, '')) AS NAME,
                            CR.PART,
                            P.PART AS PART_NAME,
                            CR.COURSE_CODE,
                            CONCAT(IF(CT.UGC_COURSE_TYPE IS NULL,
                                        '',
                                        CONCAT(CT.UGC_COURSE_TYPE, ': ')),
                                    CR.COURSE_TITLE) COURSE_TITLE,
                            SM.SEMESTER,
                            SEM.SEMESTER_NAME,
                            IF(SM.INTERNAL_MARK = - 1,
                                'A',
                                SM.INTERNAL_MARK) AS INTERNAL_MARK,
                            IF(SM.EXTERNAL_MARK = - 1,
                                'A',
                                SM.EXTERNAL_MARK) AS EXTERNAL_MARK,
                            IF(SM.TOTAL = - 1, 'A', SM.TOTAL) AS TOTAL,
                            SM.GRADE,
                            SM.GRADE_POINT,
                            PR.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                            CR.CREDITS AS CREDIT,
                            SM.M_PASS,
                            ES.M_EXAM,
                            ES.EXAM_SETTING_ID,
                            CLS.CLASS_LEVEL,
                            SM.RESULT,
                            PG.APPTYPE_ID AS APPLICATION_TYPE_ID,
                            CR.UGC_COURSE_TYPE,
                            CR.INTERNAL,
                            CR.EXTERNAL,
                            DATE_FORMAT(ES.START_DATE, '%d/%m/%Y') AS START_DATE,
                            SM.IS_ARREAR,
                            CR.COURSE_ORDER,
                             SD.DEGREE
                        FROM
                            SEMESTER_MARKS_?AC_YEAR AS SM
                                INNER JOIN
                            STU_CLASS AS SC ON SC.STUDENT_ID = SM.STUDENT_ID
                                AND SC.IS_DELETED != 1
                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                INNER JOIN
                            STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                AND SP.IS_DELETED != 1
                                INNER JOIN
                            CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                AND CR.IS_DELETED != 1
                                LEFT JOIN
                            CP_UGC_COURSE_TYPE AS CT ON CT.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                AND CT.IS_DELETED != 1
                                INNER JOIN
                            SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                LEFT JOIN
                            SUP_PART AS P ON P.PART_ID = CR.PART
                                INNER JOIN
                            CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                INNER JOIN
                            CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                INNER JOIN
                            CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                            LEFT JOIN 
                                   SUP_DEGREE AS SD ON SD.DEGREE_ID=PR.DEGREE
                                INNER JOIN
                            EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = SM.EXAM_SETTING_ID
                        WHERE
                            SM.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                AND SC.STUDENT_ID IN (?STUDENT_ID)
                        ORDER BY REGISTER_NO ,CR.COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchOverAllSemesterMarksforauxclg:
                    {
                        query = @"SELECT 
                                SM.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                SP.PHOTO,
                                DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') as DATE_OF_BIRTH,
                                CONCAT(IFNULL(FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(LAST_NAME, '')) AS NAME,
                                CR.PART,
                                P.PART AS PART_NAME,
                                CR.COURSE_CODE,
                                CONCAT(IF(CT.UGC_COURSE_TYPE IS NULL,'',CONCAT(CT.UGC_COURSE_TYPE,': ')),                                        
                                        CR.COURSE_TITLE) COURSE_TITLE,
                                SEM.SEMESTER_NAME,
                                SM.INTERNAL_MARK,
                                SM.EXTERNAL_MARK,
                                SM.GRADE,
                                SM.GRADE_POINT,
                                SM.TOTAL,
                                PR.PROGRAMME_NAME,
                                CR.CREDITS AS CREDIT,
                                SM.M_PASS,
                                CLS.CLASS_LEVEL,
                                SM.RESULT,
                                CR.UGC_COURSE_TYPE,
                                DATE_FORMAT(ES.START_DATE,'%d/%m/%Y') AS START_DATE
                            FROM
                                SEMESTER_MARKS_?AC_YEAR AS SM
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SM.STUDENT_ID
                                    AND SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                    AND CR.IS_DELETED != 1
                                    LEFT JOIN
                                CP_UGC_COURSE_TYPE AS CT ON CT.UGC_COURSE_TYPE_ID = CR.UGC_COURSE_TYPE
                                    AND CT.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CR.PART
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                                 INNER JOIN exam_setting AS ES ON ES.exam_setting_id=SM.EXAM_SETTING_ID
                            WHERE
                                
                                     SC.STUDENT_ID IN (?STUDENT_ID)
                            ORDER BY REGISTER_NO,CR.COURSE_ORDER;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentByBacthandcertificatetype:
                    {
                        query = @"SELECT 
                                    SFN.FOLIO_ID,
                                    SC.STUDENT_ID,
                                    SP.FIRST_NAME,
                                    SP.REGISTER_NO,
                                    date_format(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    SFN.FOLIO_NUMBER
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    STU_FOLIO_NUMBER AS SFN ON SFN.STUDENT_ID = SP.STUDENT_ID
                                        AND SFN.CERTIFICATE_TYPE = ?CERTIFICATE_ID
                                        AND SFN.BATCH_ID = ?BATCH
                                WHERE
                                    SC.BATCH = ?BATCH AND SC.IS_DELETED != 1 GROUP BY SC.STUDENT_ID
                                ORDER BY SFN.FOLIO_NUMBER;";
                        break;
                    }
                case StudentSQLCommads.SaveStudentFolionumber:
                    {
                        query = @"INSERT INTO 
                                STU_FOLIO_NUMBER(STUDENT_ID,
                                CERTIFICATE_TYPE,
                                BATCH_ID,FOLIO_NUMBER,
                                APPLICATION_TYPE)
                                VALUES(?STUDENT_ID,
                                ?CERTIFICATE_TYPE,
                                ?BATCH_ID,?FOLIO_NUMBER,
                                ?APPLICATION_TYPE);";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentFolioNumber:
                    {
                        query = @"UPDATE STU_FOLIO_NUMBER SET FOLIO_NUMBER=?FOLIO_NUMBER WHERE FOLIO_ID=?FOLIO_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentFolioNumber:
                    {
                        query = @"SELECT 
                                STUDENT_ID,FOLIO_NUMBER
                            FROM
                                STU_FOLIO_NUMBER
                            WHERE
                                STUDENT_ID IN (?STUDENT_ID)
                                    AND CERTIFICATE_TYPE = ?CERTIFICATE_TYPE
                                    AND BATCH_ID = ?BATCH_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchAgeByDateOfBirth:
                    {
                        query = @"SELECT DATE_FORMAT(FROM_DAYS(DATEDIFF(now(),DATE_FORMAT(?DATE_OF_BIRTH, '%Y-%m-%d'))), '%Y')+0 AS AGE;";
                        break;
                    }
                case StudentSQLCommads.FetchAdmissionCorrectionReport:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.FIRST_NAME,
                                    SPI.ADMISSION_NO,
                                    SPI.REGISTER_NO,
                                    DATE_FORMAT(SPI.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                    SPI.YEAR_OF_JOIN,
                                    SPI.MAJOR,
                                    SPI.ALLIED_1,
                                    SPI.ALLIED_2,
                                    SPI.PART_1_LANGUAGE,
                                    SPI.MEDIUM_OF_INSTRUCTION,
                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                    R.RELIGION,
                                    C.COMMUNITY,
                                    SPI.CASTE,
                                    SPI.NATIONALITY,
                                    SPI.FATHER_NAME,
                                    SPI.GUARDIAN_NAME,
                                    SPI.FR_OCCUPATION,
                                    SPI.ANNUAL_INCOME,
                                    IFNULL(SD.P_DOOR_DETAIL, SD.C_DOOR_DETAIL) AS C_DOOR_DETAIL,
                                    IFNULL(SD.P_STREET, SD.C_STREET) AS C_STREET,
                                    IFNULL(SD.P_TALUK, SD.C_TALUK) AS C_TALUK,
                                    IFNULL(SD.P_CITY, SD.C_CITY) AS C_CITY,
                                    IFNULL(SD.P_DISTRICT, SD.C_DISTRICT) AS C_DISTRICT,
                                    IFNULL(SD.P_PINCODE, SD.C_PINCODE) AS C_PINCODE,
                                    SPI.STU_MOBILENO,
                                    SPI.LAST_SCHOOL_OR_COLLEGE,
                                    SPI.LAST_STUDIED_PLACE,
                                    SPI.TC_SERIAL_NO,
                                    SPI.SPECIALLY_ABLED,
                                    SPI.EX_SERVICE_MAN,
                                    SPI.IS_ANDAMAN_ORIGIN,
                                    SPI.IS_SPORTS,
                                    SPI.TOTAL_MARK,
                                    SPI.TOTAL_MARK_OBTAINED,
                                    SPI.CUT_OF_MARK,
                                    SPI.IDENTIFICATION_MARK1,
                                    SPI.IDENTIFICATION_MARK2,
                                    SPI.REMARKS,
                                    SPI.YEAR_OF_PASSING,
                                    SPI.MONTH_OF_PASSING,
                                    SPI.MARK_SHEET_NO,
                                    SPI.QUALIFICATION,
                                    SPI.BLOOD_GROUP,
                                    SPI.AADHAR_NUMBER,
                                    SPI.Q_REGISTER_NO
                                    
                                    
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        LEFT JOIN
                                    STU_ADDRESS AS SD ON SD.STUDENT_ID = SPI.STUDENT_ID
                                        AND SD.IS_DELETED != 1
                                        
                                        LEFT JOIN
                                        
                                        SUP_COMMUNITY AS C ON C.COMMUNITYID=SPI.COMMUNITY
                                        
                                        LEFT JOIN
                                        
                                        SUP_RELIGION AS R ON R.RELIGIONID=SPI.RELIGION
									
                                WHERE
                                    SPI.STUDENT_ID =?STUDENT_ID
                                        AND SPI.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchStuAddressByStudentId:
                    {
                        query = @"SELECT ADDRESS_ID,STUDENT_ID FROM stu_address WHERE STUDENT_ID=?STUDENT_ID AND IS_DELETED!=1;";
                        break;
                    }
                case StudentSQLCommads.SaveStuPersonalinfo:
                    {
                        query = @"INSERT INTO STU_PERSONAL_INFO(
									ROLL_NO,
                                    ADMISSION_DATE,
                                    ADMISSION_NO,
                                    ADMITTED_CLASS,
                                    CLASS_ID,
                                    PROGRAM_ID,
                                    BATCH,
                                    CURRENT_BATCH,
                                    DEPT_ID,
                                    SHIFT_ID,
                                    STU_EMAILID,
                                    FIRST_NAME,
                                    LAST_NAME,
                                    PHOTO,
                                    CASTE,
									DATE_OF_BIRTH,
									ANNUAL_INCOME,
									HOME_PHONE,
									AADHAR_NUMBER,
									STU_MOBILENO,
									PLACE_OF_BIRTH,
									GENDER,
									COMMUNITY,
									BLOOD_GROUP,
									RELIGION,
									SPECIALLY_ABLED,
									NATIONALITY,
									TAMIL_NAME,
                                    FATHER_NAME,
                                    MOTHER_NAME,
                                    FR_OCCUPATION,
                                    MO_OCCUPATION,
                                    FR_INCOME,
                                    MO_INCOME,
                                    STATE_ID,
                                    CITY,
                                    PINCODE,
                                    ADDRESS_LINE1,
                                    HSS_NO,
                                    EX_SERVICE_MAN,
                                    HSS_GROUP_CODE,
                                    EMIS_NO,
                                    STUDY_BOARD,
                                    HSS_TOTAL,
                                    LAST_SCHOOL_OR_COLLEGE,
                                    MEDIUM_STUDY,
                                    EXTRA_CURRRICULAR,
                                    SPORTS,
                                    FIRSTGRADUATE,
                                    RFID,
                                    BAR_CODE,
                                    BIOD_NUMBER,
                                    PART_1_LANGUAGE,
                                    LAST_STUDIED_UNIVERSITY,
                                    YEAR_OF_PASSING,
                                    PERCENTAGE,
                                    ALLIED_2,
                                    ALLIED_1,
                                    UMIS_NO,
                                    FR_MOBILE,
                                    ENROLLMENT_NO,
                                    ENTRY_ID,
                                    ENTRY_DATE,
                                    ENTRY_ROLE,
                                    MAIN_SUBJECT,
                                    IFSC_CODE,
                                    ACCOUNT_NO,
                                    BANK_NAME,
                                    BRANCH_NAME
                                        
                                    )
                                     VALUES
		                            (
									
									?ROLL_NO,
									?ADMISSION_DATE,
									?ADMISSION_NO,
									?ADMITTED_CLASS,
									?CLASS_ID,
									?PROGRAM_ID,
									?BATCH,
									?CURRENT_BATCH,
									?DEPT_ID,
									?SHIFT_ID,
									?STU_EMAILID,
									?FIRST_NAME,
									?LAST_NAME,
									?PHOTO,
									?CASTE,
									?DATE_OF_BIRTH,
									?ANNUAL_INCOME,
									?HOME_PHONE,
									?AADHAR_NUMBER,
									?STU_MOBILENO,
									?PLACE_OF_BIRTH,
									?GENDER,
									?COMMUNITY,
									?BLOOD_GROUP,
									?RELIGION,
									?SPECIALLY_ABLED,
									?NATIONALITY,
									?TAMIL_NAME,
                                    ?FATHER_NAME,
                                    ?MOTHER_NAME,
                                    ?FR_OCCUPATION,
                                    ?MO_OCCUPATION,
                                    ?FR_INCOME,
                                    ?MO_INCOME,
                                    ?STATE_ID,
                                    ?CITY,
                                    ?PINCODE,
                                    ?ADDRESS_LINE1,
                                    ?HSS_NO,
                                    ?EX_SERVICE_MAN,
                                    ?HSS_GROUP_CODE,
                                    ?EMIS_NO,
                                    ?STUDY_BOARD,
                                    ?HSS_TOTAL,
                                    ?LAST_SCHOOL_OR_COLLEGE,
                                    ?MEDIUM_STUDY,
                                    ?EXTRA_CURRRICULAR,
                                    ?SPORTS,
                                    ?FIRSTGRADUATE,
                                    ?RFID,
                                    ?BAR_CODE,
                                    ?BIOD_NUMBER,
                                    ?PART_1_LANGUAGE,
                                    ?LAST_STUDIED_UNIVERSITY,
                                    ?YEAR_OF_PASSING,
                                    ?PERCENTAGE,
                                    ?ALLIED_2,
                                    ?ALLIED_1,
                                    ?UMIS_NO,
                                    ?FR_MOBILE,
                                    ?ENROLLMENT_NO,
                                    ?ENTRY_ID,
                                    ?ENTRY_DATE,
                                    ?ENTRY_ROLE,
                                    ?MAIN_SUBJECT,
                                    ?IFSC_CODE,
                                    ?ACCOUNT_NO,
                                    ?BANK_NAME,
                                    ?BRANCH_NAME
                                   );";
                        break;
                    }
                case StudentSQLCommads.UpdateStupersonalInfoDetails:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
                                    ROLL_NO=?ROLL_NO,
                                    FIRST_NAME=?FIRST_NAME,
                                    LAST_NAME=?LAST_NAME,
                                    ADMISSION_DATE=?ADMISSION_DATE,
                                    ADMISSION_NO=?ADMISSION_NO,
                                    REGISTER_NO = ?REGISTER_NO,
                                    ADMITTED_CLASS=?ADMITTED_CLASS,
                                    REMARKS=?REMARKS,
                                    CLASS_ID=?CLASS_ID,
                                    PROGRAM_ID=?PROGRAM_ID,
                                    BATCH=?BATCH,
                                    --  CURRENT_BATCH = ?CURRENT_BATCH,
                                    DEPT_ID=?DEPT_ID,
                                    SHIFT_ID=?SHIFT_ID,
                                    STU_EMAILID=?STU_EMAILID,
                                    IS_LEFT=?IS_LEFT,
                                    CASTE=?CASTE,
									DATE_OF_BIRTH=?DATE_OF_BIRTH,
									-- ANNUAL_INCOME=?ANNUAL_INCOME,
									-- RESIDENCE_NO=?RESIDENCE_NO,
									-- IDENTIFICATION_MARK1=?IDENTIFICATION_MARK1,
									-- IDENTIFICATION_MARK2=?IDENTIFICATION_MARK2,
									-- HOME_PHONE=?HOME_PHONE,
									AADHAR_NUMBER=?AADHAR_NUMBER,
									 STU_MOBILENO=?STU_MOBILENO,
									 FR_MOBILE=?FR_MOBILE,
									 ENROLLMENT_NO=?ENROLLMENT_NO,
									-- PLACE_OF_BIRTH=?PLACE_OF_BIRTH,
									-- MARITAL_STATUS=?MARITAL_STATUS,
									GENDER=?GENDER,
									COMMUNITY=?COMMUNITY,
									-- MOTHER_TONGUE=?MOTHER_TONGUE,
									BLOOD_GROUP=?BLOOD_GROUP,
									RELIGION=?RELIGION,
									-- SECOND_LANGUAGE=?SECOND_LANGUAGE,
									-- FIRST_LANGUAGE=?FIRST_LANGUAGE,
									-- RESIDENCY_TYPE=?RESIDENCY_TYPE,
									SPECIALLY_ABLED=?SPECIALLY_ABLED,
									NATIONALITY=?NATIONALITY,
									TAMIL_NAME='?TAMIL_NAME',
									FATHER_NAME=?FATHER_NAME,
									MOTHER_NAME=?MOTHER_NAME,
									FR_OCCUPATION=?FR_OCCUPATION,
									MO_OCCUPATION=?MO_OCCUPATION,
									FR_INCOME=?FR_INCOME,
									MO_INCOME=?MO_INCOME,
									STATE_ID=?STATE_ID,
									CITY=?CITY,
									PINCODE=?PINCODE,
									ADDRESS_LINE1=?ADDRESS_LINE1,
									HSS_NO=?HSS_NO,
									EX_SERVICE_MAN=?EX_SERVICE_MAN,
									HSS_GROUP_CODE=?HSS_GROUP_CODE,
									EMIS_NO=?EMIS_NO,
									STUDY_BOARD=?STUDY_BOARD,
									HSS_TOTAL=?HSS_TOTAL,
									LAST_SCHOOL_OR_COLLEGE=?LAST_SCHOOL_OR_COLLEGE,
									MEDIUM_STUDY=?MEDIUM_STUDY,
									EXTRA_CURRRICULAR=?EXTRA_CURRRICULAR,
									SPORTS=?SPORTS,
									FIRSTGRADUATE=?FIRSTGRADUATE,
									PART_1_LANGUAGE=?PART_1_LANGUAGE,
									LAST_STUDIED_UNIVERSITY=?LAST_STUDIED_UNIVERSITY,
									YEAR_OF_PASSING=?YEAR_OF_PASSING,
	                                ALLIED_1=?ALLIED_1,
									ALLIED_2=?ALLIED_2,
									PERCENTAGE=?PERCENTAGE,
									MAIN_SUBJECT=?MAIN_SUBJECT,
									APPLICATION_TYPE=?APPLICATION_TYPE,
									UMIS_NO=?UMIS_NO,
									NAME_IN_NATIVE=?NAME_IN_NATIVE,
									ENTRY_ID=?ENTRY_ID,
									ENTRY_DATE=?ENTRY_DATE,
									RFID=?RFID,
									BIOD_NUMBER=?BIOD_NUMBER,
									BAR_CODE=?BAR_CODE,
									LEFT_REASON=?LEFT_REASON,
									ENTRY_ROLE=?ENTRY_ROLE,
									FIRSTGRADUATE=?FIRSTGRADUATE
                                   
                                WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentStrengthDetails:
                    {
                        query = @"SELECT 
                                        sc.CLASS_ID,
                                        sc.PROGRAMME_GROUP_ID,
                                        sc.ACADEMIC_YEAR,
                                        CONCAT(cp.PROGRAMME_NAME,
                                                ' ',
                                                IF(cpg.SHIFT = 1, ' DAY ', ' EVE '),
                                                '  ',
                                                IFNULL(scs.section, '')) AS PROGRAMME_NAME,
                                        cpg.SHIFT,
                                        ccl.CLASSLEVEL,
                                        ccl.CLASSLEVELID,
                                        COUNT(*) AS STUDENT_COUNT,
                                        CP.PROGRAMME_ID,
                                        cc.CLASS_YEAR,
                                        CPG.PROGRAMME_MODE
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            AND CC.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_CLASS_LEVEL AS CCL ON CCL.CLASSLEVELID = CC.CLASS_LEVEL
                                            LEFT JOIN
                                        SUP_CLASS_SECTION AS SCS ON SCS.SECTION_ID = CC.SECTION
                                    WHERE
                                        sc.IS_DELETED != 1
                                            AND ccl.CLASSLEVELID IN (1 , 2)
                                            AND sc.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CLASS_ID , CLASSLEVELID , CLASS_YEAR , PROGRAMME_ID , PROGRAMME_MODE
                                    ORDER BY SHIFT , CLASSLEVELID , PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentStrengthDetailsGenderWise:
                    {
                        query = @"SELECT 
                                        sc.CLASS_ID,
                                        sc.PROGRAMME_GROUP_ID,
                                        sc.ACADEMIC_YEAR,
                                        CONCAT(cp.PROGRAMME_NAME,
                                                ' ',
                                                IF(cpg.SHIFT = 1, ' DAY ', ' EVE '),
                                                '  ',
                                                IFNULL(scs.section, '')) AS PROGRAMME_NAME,
                                        cpg.SHIFT,
                                        ccl.CLASSLEVEL,
                                        ccl.CLASSLEVELID,
                                        COUNT(*) AS STUDENT_COUNT,
                                        CP.PROGRAMME_ID,
                                        cc.CLASS_YEAR,
                                        SPI.GENDER,
                                        CPG.PROGRAMME_MODE
                                    FROM
                                        stu_class AS sc
                                            INNER JOIN
                                        stu_personal_info AS spi ON spi.STUDENT_ID = sc.STUDENT_ID
                                            AND spi.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            INNER JOIN
                                        cp_classes AS cc ON cc.class_id = sc.CLASS_ID
                                            AND cc.PROGRAMME_GROUP_ID = sc.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        cp_programme_group AS cpg ON cpg.PROGRAMME_GROUP_ID = cc.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        cp_programme AS cp ON cp.PROGRAMME_ID = cpg.PROGRAMME_ID
                                            INNER JOIN
                                        sup_class_level AS ccl ON ccl.CLASSLEVELID = cc.CLASS_LEVEL
                                            LEFT JOIN
                                        sup_class_section AS scs ON scs.SECTION_ID = cc.SECTION
                                    WHERE
                                        sc.IS_DELETED != 1
                                            AND ccl.CLASSLEVELID IN (1 , 2)
                                            AND sc.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CLASS_ID , CLASSLEVELID , CLASS_YEAR , PROGRAMME_ID , PROGRAMME_MODE,GENDER
                                    ORDER BY SHIFT , CLASSLEVELID , PROGRAMME_GROUP_ID;";
                        break;
                    }

                case StudentSQLCommads.UpdateStupersonalInfoDetailsForStudent:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
                                    STU_EMAILID=?STU_EMAILID,
                                    CASTE=?CASTE,
									AADHAR_NUMBER=?AADHAR_NUMBER,
									STU_MOBILENO=?STU_MOBILENO,
									GENDER=?GENDER,
									COMMUNITY=?COMMUNITY,
									BLOOD_GROUP=?BLOOD_GROUP,
									RELIGION=?RELIGION,
									SPECIALLY_ABLED=?SPECIALLY_ABLED,
									NATIONALITY=?NATIONALITY,
									TAMIL_NAME='?TAMIL_NAME',
									NAME_IN_NATIVE='?NAME_IN_NATIVE',
									FATHER_NAME=?FATHER_NAME,
									MOTHER_NAME=?MOTHER_NAME,
									FR_OCCUPATION=?FR_OCCUPATION,
									MO_OCCUPATION=?MO_OCCUPATION,
									FR_INCOME=?FR_INCOME,
									MO_INCOME=?MO_INCOME,
									STATE_ID=?STATE_ID,
									CITY=?CITY,
									PINCODE=?PINCODE,
									ADDRESS_LINE1=?ADDRESS_LINE1,
									HSS_NO=?HSS_NO,
									EX_SERVICE_MAN=?EX_SERVICE_MAN,
									HSS_GROUP_CODE=?HSS_GROUP_CODE,
									EMIS_NO=?EMIS_NO,
									STUDY_BOARD=?STUDY_BOARD,
									HSS_TOTAL=?HSS_TOTAL,
									LAST_SCHOOL_OR_COLLEGE=?LAST_SCHOOL_OR_COLLEGE,
									MEDIUM_STUDY=?MEDIUM_STUDY,
									EXTRA_CURRRICULAR=?EXTRA_CURRRICULAR,
									SPORTS=?SPORTS,
									FIRSTGRADUATE=?FIRSTGRADUATE,
									PART_1_LANGUAGE=?PART_1_LANGUAGE,
									LAST_STUDIED_UNIVERSITY=?LAST_STUDIED_UNIVERSITY,
									YEAR_OF_PASSING=?YEAR_OF_PASSING,
	                                ALLIED_1=?ALLIED_1,
									ALLIED_2=?ALLIED_2,
									PERCENTAGE=?PERCENTAGE,
									MAIN_SUBJECT=?MAIN_SUBJECT,
									UMIS_NO=?UMIS_NO,
									EMIS_NO=?EMIS_NO,
									IFSC_CODE=?IFSC_CODE,
									BRANCH_NAME=?BRANCH_NAME,
									BANK_NAME=?BANK_NAME,
									ACCOUNT_NO=?BANK_AC_NO,
									IDENTIFICATION_MARK1=?IDENTIFICATION_MARK1,
									IDENTIFICATION_MARK2=?IDENTIFICATION_MARK2,
									ENROLLMENT_NO=?ENROLLMENT_NO,
									RESIDENCY_TYPE=?RESIDENCY_TYPE,
									ENTRY_ID=?ENTRY_ID,
									ENTRY_DATE=?ENTRY_DATE,
									ENTRY_ROLE=?ENTRY_ROLE,
									FIRSTGRADUATE=?FIRSTGRADUATE
                                   
                                WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCommunityReportbyAcYearClassYear:
                    {
                        query = @"SELECT 
                                        scm.COMMUNITY,
                                        scm.COMMUNITYID,
                                        COUNT(*) AS COUNT,
                                        cp.PROGRAMME_NAME,
                                        cp.PROGRAMME_ID,
                                        ss.SHIFT_ID,
                                        ss.SHIFT_NAME,
                                        sr.RELIGION,
                                        sr.RELIGIONID,
                                        sg.GENDER_NAME,
                                        sg.GENDER_ID,
                                        scl.CLASSLEVEL,
                                        scl.CLASSLEVELID,
                                        scm.COMMUNITYORDER,
                                        cc.CLASS_YEAR
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SPI.COMMUNITY
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                            INNER JOIN
                                        SUP_RELIGION AS SR ON SR.RELIGIONID = SPI.RELIGION
                                            INNER JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
		                                    INNER JOIN 
                                        SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = CC.CLASS_LEVEL
                                    WHERE
                                        spi.IS_DELETED != 1 AND spi.IS_LEFT != 1
                                            AND sc.ACADEMIC_YEAR = ?AC_YEAR
                                            AND cc.CLASS_YEAR IN (?CLASS_YEAR)
                                            AND CLASS_LEVEL IN (?CLASS_LEVEL)
                                    GROUP BY spi.COMMUNITY, RELIGION, cp.PROGRAMME_ID, SHIFT_ID,GENDER_ID
                                    ORDER BY COMMUNITYID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentDetailsForExport:
                    {
                        query = @"SELECT 
                                        ROLL_NO,
                                        FIRST_NAME,
                                        UMIS_NO,
                                        ENROLLMENT_NO,
                                        SPI.FR_MOBILE,
                                        CD.DEPARTMENT,
                                        CC.CLASS_NAME,
                                        SPL.CLASSLEVEL as CLASS_LEVEL,
                                        cp.PROGRAMME_NAME,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        ADMISSION_NO,
                                        sb.BATCH,
                                        AADHAR_NUMBER,
                                        FR_MOBILE,
                                        STU_MOBILENO,
                                        SG.GENDER_NAME AS GENDER,
                                        SR.RELIGION,
                                        sm.COMMUNITY,
                                        CASTE,
                                        SBG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        ssa.SPECIALLY_ABLED_NAME AS SPECIALLY_ABLED,
                                        DISTRICT,
                                        TAMIL_NAME,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        SPI.FR_OCCUPATION,
                                        SPI.MO_OCCUPATION,
                                        FR_INCOME,
                                        MO_INCOME,
                                        ANNUAL_INCOME,
                                        IDENTIFICATION_MARK1,
                                        IDENTIFICATION_MARK2,
                                        ss.STATE_NAME,
                                        CITY,
                                        PINCODE,
                                        ADDRESS_LINE1,
                                        HSS_NO,
                                        sem.EX_SERVICE_MAN AS EX_SERVICE_MAN,
                                        HSS_GROUP_CODE,
                                        EMIS_NO,
                                        seb.EDUCATION_BOARD,
                                        HSS_TOTAL,
                                        LAST_SCHOOL_OR_COLLEGE,
                                        sms.MEDIUM_NAME,
                                        CURUCULAR_NAME AS EXTRA_CURRRICULAR,
                                        SPORTS,
                                        IF(FIRSTGRADUATE = 0, 'NO', 'YES') AS FIRSTGRADUATE,
                                        PART_1_LANGUAGE,
                                        UNIVERSITY AS LAST_STUDIED_UNIVERSITY,
                                        YEAR_OF_PASSING,
                                        PERCENTAGE,
                                        ALLIED_1,
                                        ALLIED_2,
                                        MAIN_SUBJECT,
                                        ACCOUNT_NO,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        GSUITE_MAIL_ID,
                                        STU_EMAILID,
                                        IFSC_CODE,
                                        NATIONALITY_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CP.DEPARTMENT
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        SUP_CLASS_LEVEL AS SPL ON SPL.CLASSLEVELID = CC.CLASS_LEVEL
                                            LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                            LEFT JOIN
                                        SUP_RELIGION AS SR ON SR.RELIGIONID = SPI.RELIGION
                                            LEFT JOIN
                                        SUP_COMMUNITY AS SM ON SM.COMMUNITYID = SPI.COMMUNITY
                                            LEFT JOIN
                                        SUP_STATE AS SS ON SS.STATE_ID = SPI.STATE_ID
                                            LEFT JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = SPI.BATCH
                                            LEFT JOIN
                                        SUP_MEDIUMSTUDY AS SMS ON SMS.MEDIUM_ID = STUDY_BOARD
                                            LEFT JOIN
                                        SUP_EDUCATION_BOARD AS SEB ON SEB.EDUCATION_BOARD_ID = SPI.STUDY_BOARD
                                            LEFT JOIN
                                        SUP_BLOOD_GROUP AS SBG ON SBG.BLOOD_GROUP_ID = SPI.BLOOD_GROUP
                                            LEFT JOIN
                                        SUP_EX_SERVICE_MAN AS SEM ON SEM.EX_SERVICE_ID = SPI.EX_SERVICE_MAN
                                            LEFT JOIN
                                        SUP_EXTRA_CURICULAR_ACTIVITIES AS SECA ON SECA.CURUCULAR_ID = SPI.EXTRA_CURRRICULAR
                                            LEFT JOIN
                                        SUP_UNIVERSITY AS SU ON SU.UNIVERSITY_ID = SPI.LAST_STUDIED_UNIVERSITY
                                            LEFT JOIN
                                        SUP_SPECIALLYABLED AS SSA ON SSA.SPECIALLY_ABLED_ID = SPI.SPECIALLY_ABLED
                                            LEFT JOIN
                                        SUP_NATIONALITY AS SN ON SN.NATIONALITY_ID = SPI.NATIONALITY
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND sc.IS_DELETED != 1 and sc.CLASS_ID in (?CLASS_ID)
                                     ORDER BY SPL.CLASSLEVEL ,CC.CLASS_YEAR,CC.CLASS_NAME,SPI.ROLL_NO,SPI.FIRST_NAME;";
                        break;
                    }
                case StudentSQLCommads.FetchStuPersonlForStudent:
                    {
                        query = @"SELECT 
                                        FIRST_NAME,
                                        LAST_NAME,
                                        CASTE,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        DATE_FORMAT(ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        BATCH,
                                        ANNUAL_INCOME,
                                        RESIDENCE_NO,
                                        PASSPORT_Number,
                                        VISA_SPONSOR,
                                        VISA_Number,
                                        IDENTIFICATION_MARK1,
                                        IDENTIFICATION_MARK2,
                                        HOME_PHONE,
                                        AADHAR_NUMBER,
                                        STU_MOBILENO,
                                        FR_MOBILE,
                                        PLACE_OF_BIRTH,
                                        MARITAL_STATUS,
                                        GENDER,
                                        COMMUNITY,
                                        MOTHER_TONGUE,
                                        BLOOD_GROUP,
                                        RELIGION,
                                        SPECIALLY_ABLED,
                                        FIRST_LANGUAGE,
                                        SECOND_LANGUAGE,
                                        RESIDENCY_TYPE,
                                        NATIONALITY,
                                        DISTRICT,
                                        SPECIALLY_ABLED,
                                        TAMIL_NAME,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        FR_OCCUPATION,
                                        MO_OCCUPATION,
                                        FR_INCOME,
                                        MO_INCOME,
                                        STATE_ID,
                                        CITY,
                                        PINCODE,
                                        ADDRESS_LINE1,
                                        HSS_NO,
                                        EX_SERVICE_MAN,
                                        HSS_GROUP_CODE,
                                        EMIS_NO,
                                        STUDY_BOARD,
                                        HSS_TOTAL,
                                        LAST_SCHOOL_OR_COLLEGE,
                                        MEDIUM_STUDY,
                                        EXTRA_CURRRICULAR,
                                        SPORTS,
                                        FIRSTGRADUATE,
                                        PART_1_LANGUAGE,
                                        LAST_STUDIED_UNIVERSITY,
                                        YEAR_OF_PASSING,
                                        PERCENTAGE,
                                        ALLIED_2,
                                        ALLIED_1,
                                        UMIS_NO,
                                        ENROLLMENT_NO,
                                        MAIN_SUBJECT,
                                        IFSC_CODE,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        ACCOUNT_NO,
                                        RESIDENCY_TYPE,
                                        IDENTIFICATION_MARK1,
                                        IDENTIFICATION_MARK2
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }

                #region list approved phd supervisors

                case StudentSQLCommads.FetchApprovedPhdSupervisors:
                    {
                        query = @"SELECT 
                                        SUPERVISOR_ID,
                                        CONCAT(IFNULL(LAST_NAME, ''),
                                                '',
                                                IFNULL(FIRST_NAME, '')) AS STAFF_NAME,
                                        PS.STAFF_ID,
                                        PHD_DEPARTMENT,
                                        sd.DEPARTMENT,
                                        POSITION,
                                        REFERENCE_ID,
                                        APPROVED_STRENGTH,
                                        PS.EMAIL,
                                        CONTACT_NO,
                                        IMG_PATH,
                                        FILE_PATH,
                                        COMPLETED,
                                        IN_PROGRESS,
                                        VACANCIES_AVAILABLE,
                                        ps.IS_ACTIVE
                                    FROM
                                        PHD_APPROVED_SUPERVISORS_INFO AS PS
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = PS.STAFF_ID
                                            INNER JOIN
                                        sup_phd_department AS sd ON sd.DEPARTMENT_ID = ps.PHD_DEPARTMENT
                                    WHERE
                                        PS.IS_DELETED != 1";
                        break;
                    }
                case StudentSQLCommads.SavePhdSupervisors:
                    {
                        query = @"INSERT INTO PHD_APPROVED_SUPERVISORS_INFO(
                                                                        STAFF_ID,
                                                                        ENTRY_ID,
                                                                        PHD_DEPARTMENT,
                                                                        POSITION,
                                                                        REFERENCE_ID,
                                                                        APPROVED_STRENGTH,
                                                                        EMAIL,
                                                                        CONTACT_NO,
                                                                        IMG_PATH,
                                                                        COMPLETED,
                                                                        IN_PROGRESS,
                                                                        VACANCIES_AVAILABLE
                                                                   )VALUES(
                                                                        ?STAFF_ID,
                                                                        ?ENTRY_ID,
                                                                        ?PHD_DEPARTMENT,
                                                                        ?POSITION,
                                                                        ?REFERENCE_ID,
                                                                        ?APPROVED_STRENGTH,
                                                                        ?EMAIL,
                                                                        ?CONTACT_NO,
                                                                        ?IMG_PATH,
                                                                        ?COMPLETED,
                                                                        ?IN_PROGRESS,
                                                                        ?VACANCIES_AVAILABLE
                                                                        );";
                        break;
                    }
                case StudentSQLCommads.UpdatePhdSupervisorsExceptImgPath:
                    {
                        query = @"UPDATE PHD_APPROVED_SUPERVISORS_INFO 
                                                                   SET
                                                                     STAFF_ID=?STAFF_ID,
                                                                     PHD_DEPARTMENT=?PHD_DEPARTMENT,
                                                                     POSITION=?POSITION,
                                                                     REFERENCE_ID=?REFERENCE_ID,
                                                                     APPROVED_STRENGTH=?APPROVED_STRENGTH,
                                                                     EMAIL=?EMAIL,
                                                                     COMPLETED=?COMPLETED,
                                                                     CONTACT_NO=?CONTACT_NO,
                                                                     IN_PROGRESS=?IN_PROGRESS,
                                                                     VACANCIES_AVAILABLE=?VACANCIES_AVAILABLE 
                                                                   WHERE
                                                                     SUPERVISOR_ID=?SUPERVISOR_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdatePhdSupervisorsimgPath:
                    {
                        query = @" UPDATE PHD_APPROVED_SUPERVISORS_INFO 
                                                                       SET
                                                                         IMG_PATH=?IMG_PATH
                                                                       WHERE SUPERVISOR_ID=?SUPERVISOR_ID";
                        break;
                    }
                case StudentSQLCommads.DeletephdSupervisorsList:
                    {
                        query = @" UPDATE PHD_APPROVED_SUPERVISORS_INFO 
                                                                       SET
                                                                         IS_DELETED=?IS_DELETED
                                                                       WHERE 
                                                                         SUPERVISOR_ID=?SUPERVISOR_ID";
                        break;
                    }
                case StudentSQLCommads.FetchApprovedPhdSupervisorsByID:
                    {
                        query = @"SELECT 
                                        SUPERVISOR_ID,
                                        PS.STAFF_ID,
                                        PHD_DEPARTMENT,
                                        POSITION,
                                        REFERENCE_ID,
                                        APPROVED_STRENGTH,
                                        PS.EMAIL,
                                        CONTACT_NO,
                                        IMG_PATH,
                                        FILE_PATH,
                                        COMPLETED,
                                        IN_PROGRESS,
                                        VACANCIES_AVAILABLE,
                                        ps.IS_ACTIVE
                                    FROM
                                        PHD_APPROVED_SUPERVISORS_INFO AS PS
                                           
                                    WHERE
                                        PS.IS_DELETED != 1 and SUPERVISOR_ID=?SUPERVISOR_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentsForSerialNo:
                    {
                        query = @"SELECT 
                                STC.CERTIFICATE_ID,
                                SP.STUDENT_ID,
                                UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                UPPER(ROLL_NO) AS ROLL_NO,
                                STC.SERIAL_NO
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                   
                                    LEFT JOIN
                                STU_TRANSFER_CERTIFICATE AS STC ON STC.STUDENT_ID = SC.STUDENT_ID
                            WHERE
                                SC.CLASS_ID in (?CLASS_ID)
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                            ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.SaveTcSerialNumber:
                    {
                        query = @"INSERT INTO STU_TRANSFER_CERTIFICATE(
                                                                    STUDENT_ID,
                                                                    SERIAL_NO,
                                                                    ACADEMIC_YEAR,
                                                                    LEAVING_DATE
                                                                  )VALUES(
                                                                    ?STUDENT_ID,
                                                                    ?SERIAL_NO,
                                                                    ?ACADEMIC_YEAR,
                                                                    ?LEAVING_DATE);";
                        break;
                    }
                case StudentSQLCommads.UpdateTcSerialNumber:
                    {
                        query = @"UPDATE STU_TRANSFER_CERTIFICATE 
                                  SET 
                                    SERIAL_NO = ?SERIAL_NO,
                                    LEAVING_DATE = ?LEAVING_DATE
                                  WHERE
                                    CERTIFICATE_ID = ?CERTIFICATE_ID;";
                        break;
                    }
                #endregion

                #region Left Student Details
                case StudentSQLCommads.FetchLeftStudentList:
                    {
                        query = @"SELECT 
                         SC.STUDENT_ID,
                         SP.ROLL_NO,
                         SP.ADMISSION_NO,
                         P.PROGRAMME_NAME,
                         CC.CLASS_NAME,
                         CP.PROGRAMME_GROUP_ID,
                         SP.IS_DELETED,
                         CC.CLASS_ID,
                         FIRST_NAME,
                         DATE_FORMAT(DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH
                     FROM
                         STU_CLASS AS SC
                             INNER JOIN
                         STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                             AND SP.IS_LEFT = 1
                             INNER JOIN
                         CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                             INNER JOIN
                         CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                             INNER JOIN
                         CP_PROGRAMME AS P ON P.PROGRAMME_ID = CP.PROGRAMME_ID
                     WHERE
                         SC.ACADEMIC_YEAR in (?AC_YEAR) AND SP.IS_DELETED !=1
                     ORDER BY PROGRAMME_GROUP_ID, CLASS_YEAR, ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStuRollNoByID:
                    {
                        query = @"SELECT STUDENT_ID,ROLL_NO FROM STU_PERSONAL_INFO WHERE STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.CheckRollNoExist:
                    {
                        query = @" SELECT 
                                        STUDENT_ID, ROLL_NO, FIRST_NAME,BATCH
                                    FROM
                                        STU_PERSONAL_INFO AS SPI 
                                    WHERE
                                        ROLL_NO = ?ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.CheckNewRollNoExistorNot:
                    {
                        query = @"SELECT 
                                        ROLL_NO,STUDENT_ID
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        ROLL_NO = ?ROLL_NO
                                            AND STUDENT_ID != ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchClassbyClassId:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        PROGRAMME_GROUP_ID
                                    FROM
                                        cp_classes
                                    WHERE
                                        CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchUserAccountDetails:
                    {
                        query = @"SELECT 
                                        USERNAME, USER_ID,USER_ACCOUNT_ID
                                    FROM
                                        user_account
                                    WHERE
                                        USER_ID = ?USER_ID AND ROLE = 5
                                            AND is_deleted != 1;";
                        break;
                    }
                case StudentSQLCommads.UpdateUserAccount:
                    {
                        query = @"UPDATE USER_ACCOUNT SET  
                                                USERNAME=?USERNAME,
                                                PASSWORD=?PASSWORD,
                                                USER_ID=?USER_ID
	                                        WHERE 
                                                USER_ACCOUNT_ID=?USER_ACCOUNT_ID";
                        break;
                    }
                case StudentSQLCommads.SaveUserAccount:
                    {
                        query = @"INSERT INTO USER_ACCOUNT(
                                    USERNAME,
                                    PASSWORD,
                                    NAME,
                                    USER_ID,
                                    ROLE,
                                    USER_TYPE)
                                VALUES 
	                                (?USERNAME,
                                    ?PASSWORD,
                                    ?NAME,
                                    ?USER_ID,
                                    ?ROLE,
                                    ?USER_TYPE);";
                        break;
                    }
                case StudentSQLCommads.SaveUserRoleByAcademicYear:
                    {
                        query = @"INSERT INTO user_roles_by_academic_year(
								 USER_ID,
								 ROLE_ID,
								 USER_TYPE, 
								 ACADEMIC_YEAR, IS_ACTIVE)
							VALUES(
                                 ?USER_ID,
								 ?ROLE_ID,
								 ?USER_TYPE, 
								 ?ACADEMIC_YEAR,1
                                 );";
                        break;
                    }
                case StudentSQLCommads.FetchAcademicSemester:
                    {
                        query = @"SELECT 
                                        ACC_SEMESTER_ID,
                                        BATCH,
                                        PROGRAMME_GROUP_ID
                                    FROM
                                        ACADEMIC_SEMESTER
                                    WHERE
                                            BATCH = ?BATCH 
                                        AND PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        AND SEMESTER = ?SEMESTER
                                        AND ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.SaveAcademicSemester:
                    {
                        query = @"INSERT INTO ACADEMIC_SEMESTER (
                                                                BATCH,
                                                                PROGRAMME_GROUP_ID,
                                                                SEMESTER,
                                                                ACADEMIC_YEAR,
                                                                IS_ACTIVE
                                                                ) VALUES(
                                                                ?BATCH,
                                                                ?PROGRAMME_GROUP_ID,
                                                                ?SEMESTER,
                                                                ?ACADEMIC_YEAR,
                                                                1
                                                                );";
                        break;
                    }
                case StudentSQLCommads.FetchStuClass:
                    {
                        query = @"SELECT 
                                    STU_CLASS_ID,
                                    STUDENT_ID
                                FROM
                                    STU_CLASS
                                WHERE
                                    ACADEMIC_YEAR = ?ACADEMIC_YEAR 
                                    AND STUDENT_ID = ?STUDENT_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case StudentSQLCommads.UpdateStuPersonalInfoForLeftStudent:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO 
                                                        SET 
                                                            IS_DELETED = 0,
                                                            IS_LEFT = 0,
                                                            IS_REJOIN = 1,
                                                            REMARKS = ?REMARKS,
                                                            ENTRY_ID = ?ENTRY_ID,
                                                            ROLL_NO = ?ROLL_NO,
                                                            BIOD_NUMBER = ?BIOD_NUMBER,
                                                            BAR_CODE = ?BAR_CODE,
                                                            RFID = ?RFID
                                                        WHERE
                                                            STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.SaveRejoinStudentDetails:
                    {
                        query = @"INSERT INTO STU_REJOIN (
                                                    STUDENT_ID,
                                                    ACADEMIC_YEAR,
                                                    SEMESTER,
                                                    ROLL_NO,
                                                    OLD_ROLL_NO,
                                                    PROGRAMME_GROUP_ID,
                                                    PROGRAMME_MODE,
                                                    ENTRY_ID,
                                                    REMARKS
                                               )VALUES (
                                                    ?STUDENT_ID,
                                                    ?ACADEMIC_YEAR,
                                                    ?SEMESTER,
                                                    ?ROLL_NO,
                                                    ?OLD_ROLL_NO,
                                                    ?PROGRAMME_GROUP_ID,
                                                    ?PROGRAMME_MODE,
                                                    ?ENTRY_ID,
                                                    ?REMARKS
                                                    );";
                        break;
                    }
                case StudentSQLCommads.FetchRejoinedStudentDetails:
                    {
                        query = @"SELECT 
                                    SRE.STUDENT_ID,
                                    FIRST_NAME,
                                    SRE.OLD_ROLL_NO,
                                    SPI.ROLL_NO,
                                    CC.CLASS_NAME,
                                    CP.PROGRAMME_NAME,
                                    SS.SEMESTER_NAME,
                                    DATE_FORMAT(SRE.ENTRY_DATE, '%d/%m/%Y %H:%m:%s %p') AS ENTRY_DATE
                                FROM
                                    STU_REJOIN AS SRE
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SRE.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SRE.STUDENT_ID
                                        AND SRE.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        LEFT JOIN
                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SRE.SEMESTER
                                WHERE
                                    SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                        AND SRE.ACADEMIC_YEAR in (?AC_YEAR)
                                ORDER BY SRE.SEMESTER , CP.PROGRAMME_ID , SRE.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchSpeciallyAbledStudentbyAcYearWise:
                    {
                        query = @"SELECT 
                                        spi.STUDENT_ID,
                                        FIRST_NAME,
                                        SPI.ROLL_NO,
                                        DATE_FORMAT(spi.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        FR_MOBILE,
                                        CC.CLASS_NAME,
                                        CP.PROGRAMME_NAME,
                                        ss.SPECIALLY_ABLED_NAME,
                                        SPECIALLY_ABLED
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = spi.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SPECIALLYABLED AS ss ON ss.SPECIALLY_ABLED_ID = spi.SPECIALLY_ABLED
                                    WHERE
                                       SPI.IS_DELETED != 1 
                                            AND SPI.IS_LEFT != 1 
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR  =?AC_YEAR 
                                            and sc.CLASS_ID in (?CLASS_ID) 
                                            AND spi.SPECIALLY_ABLED = 1
                                    ORDER BY CP.PROGRAMME_ID , spi.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchExServiceManStudentReportbyAcYearWise:
                    {
                        query = @"SELECT 
                                        spi.STUDENT_ID,
                                        FIRST_NAME,
                                        SPI.ROLL_NO,
                                        DATE_FORMAT(spi.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        FR_MOBILE,
                                        CC.CLASS_NAME,
                                        CP.PROGRAMME_NAME,
                                        sem.EX_SERVICE_MAN,
                                        EX_SERVICE_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = spi.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_EX_SERVICE_MAN AS SEM ON SEM.EX_SERVICE_ID = SPI.EX_SERVICE_MAN
                                    WHERE
                                            SPI.IS_DELETED != 1 
                                            AND SPI.IS_LEFT != 1 
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR  =?AC_YEAR 
                                            and sc.CLASS_ID in (?CLASS_ID) 
                                            AND spi.EX_SERVICE_MAN = 2
                                    ORDER BY CP.PROGRAMME_ID , spi.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentDetailReportbyAcYearWise:
                    {
                        query = @"SELECT
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        SPI.REGISTER_NO,
                                        DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        SPI.NAME_IN_NATIVE,
                                        CC.CLASS_NAME,
                                        CP.PROGRAMME_NAME
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    WHERE
                                            SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR  =?AC_YEAR
                                            and sc.CLASS_ID in (?CLASS_ID)
                                    ORDER BY CP.PROGRAMME_ID , SPI.ROLL_NO;";
                        break;
                    }
                #endregion

                /// <summary>
                /// Author : yesudass
                /// Date: 2025-04-15 
                /// Title: exam hallticket date wise order  
                /// </summary>
                /// <returns></returns>
                case StudentSQLCommads.FetchExamHallTicketByForStudent:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        EXAM_NAME,
                                        STI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        SPI.REGISTER_NO,
                                        SPI.UMIS_NO,
                                        CC.CLASS_NAME,
                                        SPI.PHOTO,
                                        IFNULL(DATE_FORMAT(STI.DATE_OF_BIRTH, '%d/%m/%Y'),
                                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y')) AS DATE_OF_BIRTH,
                                        CCR.COURSE_TITLE,
                                        CCR.COURSE_CODE,
                                       --  STI.ELIGIBLE_TYPE,
                                        IF(CIA_MALPRACTICE_ID IS NOT NULL ,2 ,STI.ELIGIBLE_TYPE) AS ELIGIBLE_TYPE,
                                        DATE_FORMAT(sti.DATE_OF_EXAM,'%d/%m/%Y') as DATE_OF_EXAM,
                                        STI.SESSION_NAME,
                                        STI.HALL_NAME,
                                        DATE_FORMAT(HALL_TICKET_PUBLICATION_DATE,'%d/%m/%Y') as HALL_TICKET_PUBLICATION_DATE
                                    FROM
                                        EXAM_SETTING AS ES
                                            INNER JOIN
                                        STU_HALL_TICKET_INFO AS STI ON STI.EXAM_SETTING_ID = ES.EXAM_SETTING_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = STI.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = STI.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = STI.COURSE_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = STI.SEMESTER_TYPE AND SS.IS_ACTIVE =1
                                            LEFT JOIN
                                        CIA_MALPRACTICE AS CM ON CM.STUDENT_ID = SPI.STUDENT_ID
                                            AND CM.COURSE_ID = STI.COURSE_ID AND CM.IS_ACTIVE=1 AND CM.IS_DELETED !=1
                                           -- INNER JOIN
                                      --  EXAM_REGISTRATION_?AC_YEAR AS ER ON ER.STUDENT_ID = STI.STUDENT_ID
                                        --    AND ER.EXAM_SETTING_ID = STI.EXAM_SETTING_ID
                                         --   AND ER.IS_VIRIFIED = 1
                                    WHERE
                                        SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                            AND STI.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND STI.IS_DELETED != 1
                                            AND ES.IS_DELETED != 1
                                            AND ES.IS_ACTIVE = 1 AND HALL_TICKET_PUBLICATION_DATE <=CURDATE()
                                    GROUP BY sti.STUDENT_ID , STI.COURSE_ID ORDER BY STI.DATE_OF_EXAM;";
                        break;
                    }
                case StudentSQLCommads.FetchExamHallTicketByForArrearSeep:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        EXAM_NAME,
                                        STI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        SPI.REGISTER_NO,
                                        SPI.UMIS_NO,
                                        SPI.PHOTO,
                                        IFNULL(DATE_FORMAT(STI.DATE_OF_BIRTH, '%d/%m/%Y'),
                                                DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y')) AS DATE_OF_BIRTH,
                                        CCR.COURSE_TITLE,
                                        CCR.COURSE_CODE,
                                        DATE_FORMAT(sti.DATE_OF_EXAM, '%d/%m/%Y') AS DATE_OF_EXAM,
                                        STI.SESSION_NAME,
                                        STI.HALL_NAME,
                                        CP.PROGRAMME_NAME AS CLASS_NAME,
                                        DATE_FORMAT(HALL_TICKET_PUBLICATION_DATE, '%d/%m/%Y') AS HALL_TICKET_PUBLICATION_DATE
                                    FROM
                                        EXAM_SETTING AS ES
                                            INNER JOIN
                                        STU_HALL_TICKET_INFO AS STI ON STI.EXAM_SETTING_ID = ES.EXAM_SETTING_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = STI.STUDENT_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = STI.COURSE_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = STI.SEMESTER_TYPE
                                            AND SS.IS_ACTIVE = 1
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SPI.PROGRAM_ID
                                            LEFT JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    WHERE
                                        SPI.IS_DELETED != 1
                                            AND STI.STUDENT_ID = ?STUDENT_ID
                                            AND STI.ACADEMIC_YEAR = ?AC_YEAR
                                            AND STI.IS_DELETED != 1
                                            AND ES.IS_DELETED != 1
                                            AND ES.IS_ACTIVE = 1
                                            AND HALL_TICKET_PUBLICATION_DATE <=CURDATE()
                                    GROUP BY sti.STUDENT_ID , STI.COURSE_ID
                                    ORDER BY STI.DATE_OF_EXAM;";
                        break;
                    }
                case StudentSQLCommads.FetchExamHallTicketByForAdmin:
                    {
                        query = @"SELECT 
                                        spi.STUDENT_ID,
                                        EXAM_NAME,
                                        sti.FIRST_NAME,
                                        spi.ROLL_NO,
                                        spi.REGISTER_NO,
                                        spi.UMIS_NO,
                                        cc.CLASS_NAME,
                                        spi.PHOTO,
                                        IFNULL(DATE_FORMAT(STI.DATE_OF_BIRTH, '%d/%m/%Y'),
                                                    DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y')) AS DATE_OF_BIRTH,
                                        ccr.COURSE_TITLE,
                                        ccr.COURSE_CODE,
                                        sti.ELIGIBLE_TYPE,
                                        DATE_FORMAT(sti.DATE_OF_EXAM,'%d/%m/%Y') as DATE_OF_EXAM,
                                        sti.SESSION_NAME,
                                        sti.HALL_NAME,
                                        DATE_FORMAT(HALL_TICKET_PUBLICATION_DATE,'%d/%m/%Y') as HALL_TICKET_PUBLICATION_DATE
                                    FROM
                                        EXAM_SETTING AS ES
                                            INNER JOIN
                                        STU_HALL_TICKET_INFO AS STI ON STI.EXAM_SETTING_ID = ES.EXAM_SETTING_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = STI.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = STI.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = STI.COURSE_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = STI.SEMESTER_TYPE AND SS.IS_ACTIVE =1
                                    WHERE
                                        SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                            AND SPI.ROLL_NO = '?ROLL_NO'
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND STI.IS_DELETED != 1
                                            AND ES.IS_DELETED != 1
                                            AND ES.IS_ACTIVE = 1 AND HALL_TICKET_PUBLICATION_DATE <=CURDATE()
                                    GROUP BY STI.STUDENT_ID , STI.COURSE_ID ORDER BY STI.DATE_OF_EXAM;";
                        break;
                    }
                #region Community Report Class Year and Section Wise

                case StudentSQLCommads.FetchCommunityReportClassYearWise:
                    {
                        query = @"SELECT 
                                        scm.COMMUNITY,
                                        scm.COMMUNITYID,
                                        COUNT(*) AS COUNT,
                                        cp.PROGRAMME_ID,
                                        ss.SHIFT_ID,
                                        ss.SHIFT_NAME,
                                        scl.CLASSLEVEL,
                                        CONCAT(cp.PROGRAMME_NAME,
                                                ' ',
                                                IF(scs.section IS NULL,
                                                    '',
                                                    CONCAT('Sec - ', scs.section))) AS PROGRAMME_NAME,
                                        scl.CLASSLEVELID,
                                        scm.COMMUNITYORDER,
                                        cc.CLASS_YEAR,
                                        cc.CLASS_ID,
                                        cc.CLASS_YEAR
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SPI.COMMUNITY
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                            LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                            INNER JOIN
                                        SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = CC.CLASS_LEVEL
                                            LEFT JOIN
                                        SUP_CLASS_SECTION AS SCS ON SCS.SECTION_ID = CC.SECTION
                                    WHERE
                                        spi.IS_DELETED != 1 AND spi.IS_LEFT != 1
                                            AND sc.ACADEMIC_YEAR = ?AC_YEAR
                                            and cc.SHIFT  in (?SHIFT)
                                            AND CLASS_LEVEL IN (?CLASS_LEVEL)
                                    GROUP BY spi.COMMUNITY , cp.PROGRAMME_ID , SHIFT_ID , cc.CLASS_ID , CLASSLEVELID , CLASS_YEAR , cpg.PROGRAMME_MODE
                                    ORDER BY COMMUNITYID, CC.CLASS_YEAR,CLASS_ID,SHIFT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStuAttendanceReportParentMeet:
                    {
                        query = @"SELECT 
                                    T.STUDENT_ID,
                                    T.CLASS_ID,
                                    DATE_FORMAT(START_DATE, '%d/%m/%Y') START_DATE,
                                    DATE_FORMAT(END_DATE, '%d/%m/%Y') END_DATE,
                                    COUNT(ATT.ATTENDENCE_DATE) AS ABSENT_COUNT,
                                    T.DAYS_COUNT
                                FROM
                                    (SELECT 
                                        SP.STUDENT_ID,
                                            SC.CLASS_ID,
                                            MIN(CAL.DAY_ORDER_DATE) AS START_DATE,
                                            MAX(CAL.DAY_ORDER_DATE) AS END_DATE,
                                            COUNT(distinct CAL.DAY_ORDER_DATE) AS DAYS_COUNT
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                    INNER JOIN
                                    academic_semester AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    INNER JOIN 
                                    CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE BETWEEN ASM.DATE_FROM AND ?ENDDATE
                                        AND ASM.IS_ACTIVE = 1
                                        AND CAL.IS_HOLIDAY != 1
                                    INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.CLASS_ID IN (?CLASS_ID)
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.STUDENT_ID
                                    ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                        LEFT JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ATT.ATTENDANCE_TYPE_ID = 2
                                        AND ATT.IS_DELETED != 1
                                        AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                GROUP BY T.STUDENT_ID
                                ORDER BY  T.CLASS_ID,T.STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentFeeDueDetailsForMeet:
                    {
                        query = @"SELECT 
                                            SPI.STUDENT_ID,
                                            SPI.FIRST_NAME,
                                            SPI.ROLL_NO,
                                            SFD.STU_DUE_ID,
                                            CC.CLASS_NAME,
                                            CC.CLASS_ID,
                                            SFD.TERMFEE1,
                                            SFD.TERMFEE1PAID,
                                            SUM(CASE
                                                WHEN SFD.TERMFEE1 IS NULL THEN 0
                                                ELSE IFNULL(SFD.TERMFEE1, 0) - IFNULL(SFD.TERMFEE1PAID, 0)
                                            END + CASE
                                                WHEN SFD.TERMFEE2 IS NULL THEN 0
                                                ELSE IFNULL(SFD.TERMFEE2, 0) - IFNULL(SFD.TERMFEE2PAID, 0)
                                            END) AS TERMFEE1DUE,
                                            IFNULL(EF.EXAMFEEDUE, 0) AS EXAMFEEDUE,
                                            SFD.TERMFEE2,
                                            SFD.TERMFEE2PAID,
                                            IFNULL(EF.EXAMFEE, 0) AS EXAMFEE1,
                                            IFNULL(EF.EXAMFEEPAID, 0) AS EXAMFEE1PAID,
                                            IFNULL(EF.EXAMFEEDUE, 0) AS EXAMFEE1DUE,
                                            IFNULL(BFSA.B_CREDIT, 0) AS BUSFEE,
                                            IFNULL(BFSA.B_DEBIT, 0) AS BUSFEEPAID,
                                            GREATEST(IFNULL(BFSA.B_CREDIT, 0) - IFNULL(BFSA.B_DEBIT, 0),
                                                    0) AS BUSFEEDUE,
                                            IFNULL(HFSA.CREDIT, 0) AS HOSTELFEE,
                                            IFNULL(HFSA.DEBIT, 0) AS HOSTELFEEPAID,
                                            GREATEST(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0),
                                                    0) AS HOSTELFEEDUE,
                                            IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                                            (IFNULL(ARR.ARREAR_DUE, 0) + SUM(CASE
                                                WHEN SFD.TERMFEE1 IS NULL THEN 0
                                                ELSE GREATEST(IFNULL(SFD.TERMFEE1, 0) - IFNULL(SFD.TERMFEE1PAID, 0),
                                                        0)
                                            END + CASE
                                                WHEN SFD.TERMFEE2 IS NULL THEN 0
                                                ELSE GREATEST(IFNULL(SFD.TERMFEE2, 0) - IFNULL(SFD.TERMFEE2PAID, 0),
                                                        0)
                                            END) + IFNULL(EF.EXAMFEEDUE, 0)
                                            + GREATEST(IFNULL(BFSA.B_CREDIT, 0) - IFNULL(BFSA.B_DEBIT, 0), 0)
                                            + GREATEST(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0), 0)) AS TOTAL_DUE_AMOUNT,
                                            SFD.ENTRY_ID,
                                            SFD.ENTRY_DATE
                                        FROM
                                            STU_PERSONAL_INFO AS SPI
                                                LEFT JOIN
                                            STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                                AND sfd.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SFD.IS_DELETED <> 1
                                                INNER JOIN
                                            STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                                INNER JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                                LEFT JOIN
                                            (SELECT 
                                                FSA.STUDENT_ID,
                                                SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE,
                                                SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEEPAID,
                                                CAST(GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS SIGNED) AS EXAMFEEDUE
                                            FROM FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                            INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN ACADEMIC_YEAR AS AY ON (AY.AC_YEAR = SFF.ACADEMIC_YEAR OR AY.ACADEMIC_YEAR_ID = SFF.ACADEMIC_YEAR) AND AY.ACTIVE_YEAR = 1 AND AY.IS_DELETED <> 1
                                            INNER JOIN SUP_SEMESTER_TYPE AS SST ON (SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE OR SST.SEMESTER_TYPE = SFF.SEMESTER_TYPE) AND SST.IS_ACTIVE = 1 AND SST.IS_DELETED <> 1
                                            WHERE
                                                SFT.FREQUENCY_TYPE_ID = 1
                                                AND FSA.IS_DELETED <> 1
                                                AND SFF.IS_DELETED <> 1
                                            GROUP BY FSA.STUDENT_ID) AS EF ON EF.STUDENT_ID = SPI.STUDENT_ID
                                                LEFT JOIN
                                            (SELECT 
                                                FSA.STUDENT_ID,
                                                    FSA.ACADEMIC_YEAR,
                                                    SUM(IFNULL(FSA.CREDIT, 0)) AS CREDIT,
                                                    SUM(IFNULL(FSA.DEBIT, 0)) AS DEBIT
                                            FROM
                                                FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                            INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            WHERE
                                                SFT.FREQUENCY_TYPE_ID = 4
                                                    AND FSA.IS_DELETED <> 1
                                            GROUP BY FSA.STUDENT_ID , FSA.ACADEMIC_YEAR) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                                                LEFT JOIN
                                            (SELECT 
                                                FSA.STUDENT_ID,
                                                    FSA.ACADEMIC_YEAR,
                                                    SUM(IFNULL(FSA.CREDIT, 0)) AS B_CREDIT,
                                                    SUM(IFNULL(FSA.DEBIT, 0)) AS B_DEBIT
                                            FROM
                                                FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                            INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            WHERE
                                                SFT.FREQUENCY_TYPE_ID = 8
                                                    AND FSA.IS_DELETED <> 1
                                            GROUP BY FSA.STUDENT_ID , FSA.ACADEMIC_YEAR) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                                LEFT JOIN
                                            (SELECT 
                                                STUDENT_ID,
                                                    SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                                            FROM
                                                STU_FEE_DUE_INFO
                                            WHERE
                                                ACADEMIC_YEAR < ?AC_YEAR AND IS_DELETED <> 1
                                            GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SPI.STUDENT_ID
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.CLASS_ID IN (?CLASS_ID)
                                                AND SC.IS_DELETED <> 1
                                                AND SPI.IS_DELETED <> 1
                                                AND SPI.IS_LEFT <> 1
                                        GROUP BY SC.STUDENT_ID , SC.ACADEMIC_YEAR
                                        ORDER BY SPI.ROLL_NO , SC.CLASS_ID;";
                        break;
                    }
                //case StudentSQLCommads.FetchStudentFeeDueDetailsForMeet:
                //{
                //    query = @"SELECT 
                //                sfd.STUDENT_ID,
                //                spi.FIRST_NAME,
                //                spi.ROLL_NO,
                //                STU_DUE_ID,
                //                cc.CLASS_NAME,
                //                cc.CLASS_ID,
                //                sfd.TERMFEE1,
                //                TERMFEE1PAID,
                //                SUM(CASE
                //                    WHEN sfd.TERMFEE1 IS NULL THEN 0
                //                    ELSE IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0)
                //                END + CASE
                //                    WHEN sfd.TERMFEE2 IS NULL THEN 0
                //                    ELSE IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0)
                //                END) AS TERMFEE1DUE,
                //                SUM(CASE
                //                    WHEN sfd.EXAMFEE1 IS NULL THEN 0
                //                    ELSE IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0)
                //                END + CASE
                //                    WHEN sfd.EXAMFEE2 IS NULL THEN 0
                //                    ELSE IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0)
                //                END) AS EXAMFEEDUE,
                //                sfd.TERMFEE2,
                //                sfd.TERMFEE2PAID,
                //                sfd.EXAMFEE1,
                //                EXAMFEE1PAID,
                //                IF(EXAMFEE1 IS NULL,
                //                    EXAMFEE1,
                //                    SUM(IFNULL(EXAMFEE1, 0) - IFNULL(EXAMFEE1PAID, 0))) AS EXAMFEE1DUE,
                //                B_CREDIT AS BUSFEE,
                //                B_DEBIT AS BUSFEEPAID,
                //                IF(B_CREDIT IS NULL,
                //                    B_CREDIT,
                //                    SUM(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))) AS BUSFEEDUE,
                //                IF(HFSA.CREDIT IS NULL,
                //                    HFSA.CREDIT,
                //                    SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                //                IF(HFSA.DEBIT IS NULL,
                //                    HFSA.DEBIT,
                //                    SUM(IFNULL(HFSA.DEBIT, '0'))) HOSTELFEEPAID,
                //                IF(HFSA.CREDIT IS NULL,
                //                    NULL,
                //                    SUM(IFNULL(HFSA.CREDIT, '0')) - SUM(IFNULL(HFSA.DEBIT, '0'))) AS HOSTELFEEDUE,
                //                IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                //                (IFNULL(ARR.ARREAR_DUE, 0) + SUM(CASE
                //                    WHEN sfd.TERMFEE1 IS NULL THEN 0
                //                    ELSE IF(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0) < 0,
                //                        0,
                //                        IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0))
                //                END + CASE
                //                    WHEN sfd.TERMFEE2 IS NULL THEN 0
                //                    ELSE IF(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0)<0,
                //                        0,
                //                        IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0))
                //                END + CASE
                //                    WHEN sfd.EXAMFEE1 IS NULL THEN 0
                //                    ELSE IF(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0)<0,
                //                        0,
                //                        IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0))
                //                END + CASE
                //                    WHEN sfd.EXAMFEE2 IS NULL THEN 0
                //                    ELSE IF(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0)<0,
                //                        0,
                //                        IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0))
                //                END + CASE
                //                    WHEN B_CREDIT IS NULL THEN 0
                //                    ELSE IF(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0)<0,
                //                        0,
                //                        IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))
                //                END + CASE
                //                    WHEN HFSA.CREDIT IS NULL THEN 0
                //                    ELSE IF(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0)<0,
                //                        0,
                //                        IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0))
                //                END)) AS TOTAL_DUE_AMOUNT,
                //                SFD.ENTRY_ID,
                //                SFD.ENTRY_DATE
                //            FROM
                //                STU_PERSONAL_INFO AS SPI
                //                    INNER JOIN
                //                STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                //                    INNER JOIN
                //                STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                //                    AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                //                    INNER JOIN
                //                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                //                    INNER JOIN
                //                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                //                    LEFT JOIN
                //                (SELECT 
                //                    STUDENT_ID,
                //                        SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                //                        SUM(IFNULL(DEBIT, '0')) AS DEBIT
                //                FROM
                //                    HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                //                WHERE
                //                    hf.IS_DELETED != 1
                //                GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                //                left join (SELECT 
                //                    STUDENT_ID,
                //                        SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                //                        SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                //                FROM
                //                    BUS_FEE_STUDENT_ACCOUNT AS Bf
                //                WHERE
                //                    Bf.IS_DELETED != 1
                //                GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                //                    LEFT JOIN
                //                (SELECT 
                //                    STUDENT_ID,
                //                        SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                //                FROM
                //                    STU_FEE_DUE_INFO
                //                WHERE
                //                    ACADEMIC_YEAR < ?AC_YEAR AND IS_DELETED <> 1
                //                GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SPI.STUDENT_ID
                //            WHERE
                //                SFD.ACADEMIC_YEAR = ?AC_YEAR AND SC.CLASS_ID IN (?CLASS_ID)
                //                    AND SC.IS_DELETED != 1
                //                    AND SFD.IS_DELETED != 1
                //                    AND SPI.IS_DELETED != 1
                //                    AND SPI.IS_LEFT != 1
                //            GROUP BY SFD.STUDENT_ID, sfd.ACADEMIC_YEAR
                //            ORDER BY ROLL_NO , SC.CLASS_ID;";
                //    break;
                //}
                #endregion

                case StudentSQLCommads.FetchAttendancePercentageForNoDue:
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
                                    COUNT(COMMON_ATTENDANCE_ID) AS ABSENT_COUNT,
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
                                    INNER JOIN academic_semester AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND ASM.BATCH = SC.BATCH
                                        AND ASM.IS_ACTIVE = 1
                                    INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE BETWEEN ASM.DATE_FROM AND ?ENDDATE
                                        AND ASM.IS_ACTIVE = 1
                                        AND CAL.IS_HOLIDAY != 1
                                    INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND sC.STUDENT_ID IN (?STUDENT_ID)
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                        LEFT JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ATT.ATTENDANCE_TYPE_ID = 2
                                        AND ATT.IS_DELETED != 1
                                        AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                GROUP BY STUDENT_ID
                                ORDER BY T.DEPARTMENT , T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchBusStudentList:
                    {
                        query = @"SELECT 
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        CC.CLASS_NAME,
                                        BSA.BUS_ROUTE,
                                        BSA.STUDENT_ID,
                                        CC.SHIFT,
                                        SS.SHIFT_NAME,
                                        CC.CLASS_ID,
                                        BSA.BUS_NO
                                    FROM
                                        BUS_FEE_STUDENT_ACCOUNT AS BSA
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = BSA.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = BSA.ACADEMIC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            LEFT JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CC.SHIFT
                                    WHERE
                                        BSA.TERM = ?TERM AND CC.SHIFT IN (?SHIFT)
                                            AND BSA.ACADEMIC_YEAR = ?AC_YEAR
                                            AND BSA.ROLE =5
                                            AND BSA.IS_DELETED != 1 
                                    UNION SELECT 
                                        SPI.FIRST_NAME,
                                        SPI.STAFF_CODE,
                                        CD.DEPARTMENT,
                                        BSA.BUS_ROUTE,
                                        SPI.STAFF_ID,
                                        SPI.SHIFT,
                                        SS.SHIFT_NAME,
                                        CD.DEPARTMENT_ID,
                                        BSA.BUS_NO
                                    FROM
                                        BUS_FEE_STUDENT_ACCOUNT AS BSA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = BSA.STUDENT_ID
                                            LEFT JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SPI.DEPARTMENT
                                        LEFT JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = SPI.SHIFT
                                    WHERE
                                        BSA.TERM = ?TERM AND BSA.ROLE=4
                                            AND BSA.ACADEMIC_YEAR = ?AC_YEAR
                                            AND BSA.IS_DELETED != 1
                                    ORDER BY BUS_ROUTE , STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentDetailsForPrint:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    SPI.FATHER_NAME,
                                    date_format(SPI.DATE_OF_BIRTH,'%d/%m/%Y') as DATE_OF_BIRTH,
                                    SPI.UMIS_NO,
                                    SR.RELIGION,
                                    SCM.COMMUNITY,
                                    SC.CLASS_ID,
                                    CC.CLASS_NAME
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        LEFT JOIN
                                    SUP_RELIGION AS SR ON SR.RELIGIONID = SPI.RELIGION
                                        LEFT JOIN
                                    SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    SC.CLASS_ID IN(?CLASS_ID) AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR ORDER BY SPI.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchNoDueDefaultersForValidation:
                    {
                        query = @"SELECT 
                                        SND.STUDENT_ID,
                                        SND.REASON,
                                        SPI.ROLL_NO,
                                        SPI.FIRST_NAME
                                    FROM
                                        STU_NODUE_DEFAULTERS AS SND
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SND.STUDENT_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SND.SEMESTER_TYPE
                                    WHERE
                                        SST.IS_ACTIVE = 1
                                            AND SND.IS_DELETED != 1
                                            AND SND.IS_ACTIVE = 1
                                            AND SND.STUDENT_ID = ?STUDENT_ID
                                            AND SND.ACADEMIC_YEAR = ?AC_YEAR ORDER BY SPI.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchCommonAttendancePercentage:
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
                                    COUNT(COMMON_ATTENDANCE_ID) AS ABSENT_COUNT,
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
                                    INNER JOIN academic_semester AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND ASM.BATCH = SC.BATCH
                                        AND ASM.IS_ACTIVE = 1
                                    INNER JOIN CALENDER AS CAL ON CAL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND CAL.DAY_ORDER_DATE BETWEEN ASM.DATE_FROM AND ?ENDDATE
                                        AND ASM.IS_ACTIVE = 1
                                        AND CAL.IS_HOLIDAY != 1
                                    INNER JOIN CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND sC.STUDENT_ID IN (?STUDENT_ID)
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    ORDER BY C.DEPARTMENT , C.CLASS_LEVEL , C.CLASS_YEAR , SP.ROLL_NO) AS T
                                        LEFT JOIN
                                    ATT_COMMON_ABSENTEE_LIST AS ATT ON ATT.STUDENT_ID = T.STUDENT_ID
                                        AND ATT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND ATT.ATTENDANCE_TYPE_ID = 2
                                        AND ATT.IS_DELETED != 1
                                        AND ATTENDENCE_DATE BETWEEN T.START_DATE AND T.END_DATE
                                GROUP BY STUDENT_ID
                                ORDER BY T.DEPARTMENT , T.CLASS_LEVEL , T.CLASS_YEAR , T.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FtechRegularCourseForLactATTHallTicket:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    EXAM_NAME,
                                    STI.FIRST_NAME,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    SPI.UMIS_NO,
                                    CC.CLASS_NAME,
                                    SPI.PHOTO,
                                    IFNULL(DATE_FORMAT(STI.DATE_OF_BIRTH, '%d/%m/%Y'),
                                            DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y')) AS DATE_OF_BIRTH,
                                    CCR.COURSE_TITLE,
                                    CCR.COURSE_CODE,
                                    STI.ELIGIBLE_TYPE,
                                    DATE_FORMAT(sti.DATE_OF_EXAM, '%d/%m/%Y') AS DATE_OF_EXAM,
                                    STI.SESSION_NAME,
                                    STI.HALL_NAME,
                                    DATE_FORMAT(HALL_TICKET_PUBLICATION_DATE, '%d/%m/%Y') AS HALL_TICKET_PUBLICATION_DATE
                                FROM
                                    EXAM_SETTING AS ES
                                        INNER JOIN
                                    STU_HALL_TICKET_INFO AS STI ON STI.EXAM_SETTING_ID = ES.EXAM_SETTING_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = STI.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = STI.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = STI.COURSE_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = STI.SEMESTER_TYPE
                                        AND SS.IS_ACTIVE = 1
                                        INNER JOIN
                                    EXAM_REGISTRATION_?AC_YEAR AS ER ON ER.STUDENT_ID = STI.STUDENT_ID
                                        AND ER.EXAM_SETTING_ID = STI.EXAM_SETTING_ID
                                        AND STI.COURSE_ID = ER.COURSE_ID
                                        AND STATUS = 'A'
                                WHERE
                                    SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                        AND STI.STUDENT_ID = ?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND STI.IS_DELETED != 1
                                        AND ES.IS_DELETED != 1
                                        AND ES.IS_ACTIVE = 1
                                        AND HALL_TICKET_PUBLICATION_DATE <= CURDATE()
                                GROUP BY sti.STUDENT_ID , sti.COURSE_ID ORDER BY STI.DATE_OF_EXAM;";
                        break;
                    }



                case StudentSQLCommads.FetchStudentsbyBatchid:
                    {
                        query = @"SELECT 
                                    SPI.REGISTER_NO,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    SPI.ABC_ID,
                                    SPI.EMIS_NO,
                                    SPI.UMIS_NO,
                                    SPI.AADHAR_NUMBER,
                                    SPI.BATCH
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.IS_DELETED !=1
                                WHERE
                                    SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.CLASS_ID in (?CLASS_ID)
                                ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentUMISNo:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO 
                                                SET
                                                    UMIS_NO=?UMIS_NO,
                                                    EMIS_NO=?EMIS_NO,
                                                    ABC_ID=?ABC_ID
                                                WHERE ROLL_NO=?ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchCollegeDetail:
                    {
                        query = @"SELECT 
                                    UPPER(COLLEGE_CODE) AS 'COLLEGE_CODE',
                                    UPPER(COLLEGE_NAME) AS 'COLLEGE_NAME',
                                    CONCAT(ADDRESS_ONE,' - ',PINCODE) AS 'COLLEGE_ADDRESS',
                                    UNIVERSITY AS 'UNIVERSITY',
                                    GRADE AS 'GRADE',
                                    CONCAT(PLACE,' - ',PINCODE) AS 'PLACE',
                                    COLLEGE_LOGO,
                                    LOGIN_IMG_PATH
                                FROM
                                    COLLEGE_DETAILS;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentInfoForCourseComCertificate:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SP.GENDER,
                                    SP.FIRST_NAME,
                                    CLASS_NAME,
                                    SP.ROLL_NO,
                                    CP.PROGRAMME_NAME,
                                    SC.ACADEMIC_YEAR,
                                    CONCAT(SB.BATCH, '-', SB.BATCH + CC.CLASS_YEAR) AS BATCH,
                                    DATE_FORMAT(STC.LEAVING_DATE,'%d/%m/%Y') as LEAVING_DATE,
                                    DATE_FORMAT(IFNULL(STC.ADMISSION_DATE,SP.ADMISSION_DATE),'%d/%m/%Y') AS ADMISSION_DATE
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_LEFT != 1
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPP.PROGRAMME_ID
                                        LEFT JOIN
                                    SUP_BATCHES AS SB ON SB.BATCH_ID = SC.BATCH
                                        LEFT JOIN
                                    STU_TRANSFER_CERTIFICATE AS STC ON STC.STUDENT_ID = SP.STUDENT_ID 
                                        AND STC.IS_DELETED !=1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.STUDENT_ID IN (?STUDENT_ID) ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.BindStudentDetailsforDisTcPrint:
                    {
                        query = @"SELECT 
                                        SP.ROLL_NO,
                                        UPPER(IFNULL(TC.ADMISSION_NO, SP.ROLL_NO)) AS ADMISSION_NO,
                                        TC.SERIAL_NO,
                                        (IFNULL(SP.UMIS_NO, TC.UMIS_NO)) AS UMIS_NO,
                                        TC.CERTIFICATE_ID,
                                        IFNULL(SP.STUDENT_ID, TC.STUDENT_ID) AS STUDENT_ID,
                                        UPPER(IFNULL(TC.FIRST_NAME,
                                                        CONCAT(SP.FIRST_NAME,
                                                                ' ',
                                                                IFNULL(SP.LAST_NAME, '')))) AS 'FIRST_NAME',
                                        UPPER(IFNULL(TC.GENDER, sg.GENDER_NAME)) AS 'GENDER',
                                        CONCAT(UPPER(IFNULL(IFNULL(TC.NATIONALITY,
                                                                        IFNULL(sn.NATIONALITY_NAME, 'INDIAN')),
                                                                ' ---- ')),
                                                ' , ',
                                                UPPER(IFNULL(IFNULL(TC.RELIGION, sr.RELIGION),
                                                                ' ---- '))) AS 'NATIONALITY',
                                        'Refer The Community Certificate' AS CASTE,
                                        DATE_FORMAT(IFNULL(TC.DATE_OF_BIRTH, sp.DATE_OF_BIRTH),
                                                '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                        CONCAT(DATE_FORMAT(IFNULL(TC.ADMISSION_DATE, sp.ADMISSION_DATE),
                                                        '%d/%m/%Y')) AS 'ADMITTED_DATE',
                                        DATE_FORMAT(TC.LEAVING_DATE, '%d/%m/%Y') AS 'LEAVING_DATE',
                                        (IFNULL(TC.REASON_FOR_LEAVING, 'Discontinue')) AS 'REASON_FOR_LEAVING',
                                        IFNULL(TC.LEAVING_CLASS,
                                                IF(CC.SHIFT = 1,
                                                    REPLACE(CC.CLASS_NAME, ' DAY', ''),
                                                    REPLACE(CC.CLASS_NAME, ' EVE', ''))) AS 'LEAVING_CLASS',
                                        UPPER(IFNULL(SC.CONDUCT_NAME,'GOOD')) AS 'CONDUCT',
                                        IFNULL(IFNULL(IFNULL(TC.FATHER_NAME, sp.FATHER_NAME),
                                                        IFNULL(TC.GUARDIAN_NAME, sp.MOTHER_NAME)),
                                                '-----') AS FATHER_NAME,
                                        (IFNULL(TC.ACADEMIC_YEARS,
                                                CONCAT(CASE
                                                            WHEN CC.CLASS_YEAR = 1 THEN 'One Year'
                                                            WHEN CLASS_YEAR = 2 THEN 'Two Years'
                                                            WHEN CLASS_YEAR = 3 THEN 'Three Years'
                                                            WHEN CLASS_YEAR = 4 THEN 'Four Years'
                                                            ELSE 'UNKNOWN'
                                                        END,
                                                        ' (',
                                                        SB.BATCH,
                                                        '-',
                                                        SB.BATCH + CC.CLASS_YEAR,
                                                        ')'))) AS 'ACADEMIC_YEARS',
                                        UPPER(TC.CLASSES_STUDEIED) AS 'CLASSES_STUDEIED',
                                        UPPER(TC.FIRST_LANGUAGE) AS 'FIRST_LANGUAGE',
                                        CONTACT_NO,
                                        UPPER(ADDRESS) AS ADDRESS,
                                        (IFNULL(TC.MEDIUM_OF_INSTRUCTION, 'English')) AS MEDIUM_OF_INSTRUCTION,
                                        'Refer The Marksheets' AS QUALIFIED_PROMOTION,
                                        '~/Content/hcc_img/sign/Principal-E-Sign.png' AS PRINCIPAL_SIGN
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS S ON S.STUDENT_ID = SP.STUDENT_ID
                                            AND S.ACADEMIC_YEAR = ?AC_YEAR
                                            LEFT JOIN
                                        sup_batches AS sb ON sb.BATCH_ID = s.BATCH
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = S.CLASS_ID
                                            LEFT JOIN
                                        STU_TRANSFER_CERTIFICATE AS TC ON TC.STUDENT_ID = SP.STUDENT_ID
                                            LEFT JOIN
                                        SUP_OPTION AS O ON O.OPTION_ID = TC.FEE_PAID
                                            LEFT JOIN
                                        SUP_OPTION AS SO ON SO.OPTION_ID = TC.SCHOLAR_SHIP
                                            LEFT JOIN
                                        SUP_CONDUCT AS SC ON SC.CONDUCT_ID = TC.CONDUCT
                                            LEFT JOIN
                                        sup_gender AS sg ON sg.GENDER_ID = sp.gender
                                            LEFT JOIN
                                        sup_nationality AS sn ON sn.NATIONALITY_ID = sp.NATIONALITY
                                            LEFT JOIN
                                        sup_religion AS sr ON sr.RELIGIONID = sp.RELIGION
                                            LEFT JOIN
                                        sup_community AS scm ON scm.COMMUNITYID = sp.COMMUNITY
                                    WHERE
                                        sp.STUDENT_ID IN (?STUDENT_ID);";
                        break;
                    }
                case StudentSQLCommads.FetchStudentListForTcPrint:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(UPPER(FIRST_NAME), ' - ', UPPER(ROLL_NO)) AS 'FIRST_NAME',
                                    SC.CLASS_ID
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                WHERE
                                         SC.CLASS_ID in (?CLASS_ID)
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.UpdateStuPersonalInfoAddLeft:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO 
                                            SET 
                                                IS_LEFT = 1,
                                                ENTRY_DATE = ?ENTRY_DATE
                                            WHERE
                                                STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.DeletedStuClassbyStuClassID:
                    {
                        query = @"UPDATE STU_CLASS 
                                            SET 
                                                IS_DELETED = ?IS_DELETED
                                            WHERE
                                                STU_CLASS_ID = ?STU_CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.DeletedUserAccountbyUserID:
                    {
                        query = @"UPDATE USER_ACCOUNT 
                                                    SET 
                                                        IS_DELETED = ?IS_DELETED
                                                    WHERE
                                                        USER_ID = ?USER_ID AND USER_TYPE = ?USER_TYPE;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentDueForTCDistriputionSign:
                    {
                        query = @"SELECT 
                                    sfd.STUDENT_ID,
                                    spi.FIRST_NAME,
                                    spi.ROLL_NO,
                                    spi.UMIS_NO,
                                    STU_DUE_ID,
                                    cc.CLASS_NAME,
                                    cc.CLASS_ID,
                                    sfd.TERMFEE1,
                                    TERMFEE1PAID,
                                    SUM(CASE
                                        WHEN sfd.TERMFEE1 IS NULL THEN 0
                                        ELSE IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0)
                                    END + CASE
                                        WHEN sfd.TERMFEE2 IS NULL THEN 0
                                        ELSE IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0)
                                    END) AS TERMFEE1DUE,
                                    SUM(CASE
                                        WHEN sfd.EXAMFEE1 IS NULL THEN 0
                                        ELSE IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0)
                                    END + CASE
                                        WHEN sfd.EXAMFEE2 IS NULL THEN 0
                                        ELSE IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0)
                                    END) AS EXAMFEEDUE,
                                    sfd.TERMFEE2,
                                    sfd.TERMFEE2PAID,
                                    sfd.EXAMFEE1,
                                    EXAMFEE1PAID,
                                    IF(EXAMFEE1 IS NULL,
                                        EXAMFEE1,
                                        SUM(IFNULL(EXAMFEE1, 0) - IFNULL(EXAMFEE1PAID, 0))) AS EXAMFEE1DUE,
                                    B_CREDIT AS BUSFEE,
                                    B_DEBIT AS BUSFEEPAID,
                                    IF(B_CREDIT IS NULL,
                                        B_CREDIT,
                                        SUM(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))) AS BUSFEEDUE,
                                    IF(HFSA.CREDIT IS NULL,
                                        HFSA.CREDIT,
                                        SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                                    IF(HFSA.DEBIT IS NULL,
                                        HFSA.DEBIT,
                                        SUM(IFNULL(HFSA.DEBIT, '0'))) HOSTELFEEPAID,
                                    IF(HFSA.CREDIT IS NULL,
                                        NULL,
                                        SUM(IFNULL(HFSA.CREDIT, '0')) - SUM(IFNULL(HFSA.DEBIT, '0'))) AS HOSTELFEEDUE,
                                    SUM(CASE
                                        WHEN sfd.TERMFEE1 IS NULL THEN 0
                                        ELSE IF(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0) < 0,
                                            0,
                                            IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0))
                                    END + CASE
                                        WHEN sfd.TERMFEE2 IS NULL THEN 0
                                        ELSE IF(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0) < 0,
                                            0,
                                            IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0))
                                    END + CASE
                                        WHEN B_CREDIT IS NULL THEN 0
                                        ELSE IF(IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0) < 0,
                                            0,
                                            IFNULL(B_CREDIT, 0) - IFNULL(B_DEBIT, 0))
                                    END + CASE
                                        WHEN HFSA.CREDIT IS NULL THEN 0
                                        ELSE IF(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0) < 0,
                                            0,
                                            IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0))
                                    END) AS TOTAL_DUE_AMOUNT,
                                    SFD.ENTRY_ID,
                                    SFD.ENTRY_DATE,
                                    snd.REASON
                                FROM
                                    STU_PERSONAL_INFO AS SPI
                                        INNER JOIN
                                    STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        LEFT JOIN
                                    STU_NODUE_DEFAULTERS AS SND ON SND.STUDENT_ID = SC.STUDENT_ID
                                        AND SND.IS_DELETED != 1
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                                            SUM(IFNULL(DEBIT, '0')) AS DEBIT
                                    FROM
                                        HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                                    WHERE
                                        hf.IS_DELETED != 1
                                            AND hf.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                                            SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                                    FROM
                                        BUS_FEE_STUDENT_ACCOUNT AS Bf
                                    WHERE
                                        Bf.IS_DELETED != 1
                                            AND Bf.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                WHERE
                                    SFD.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        AND SC.CLASS_ID IN (?CLASS_ID)
                                        AND SFD.IS_DELETED != 1
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                                ORDER BY ROLL_NO , SC.CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentListbyAcademicYearWise:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    ROLL_NO,
                                    REGISTER_NO,
                                    ADMISSION_NO,
                                    FIRST_NAME,
                                    D.DEPARTMENT,
                                    CC.CLASS_NAME,
                                    S.SHIFT_NAME,
                                    IF(SPI.IS_LEFT = 0, 'NO', 'YES') AS IS_LEFT,SPI.PHOTO
                                FROM
                                    STU_PERSONAL_INFO SPI
                                        LEFT JOIN
                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SPI.DEPT_ID
                                        AND D.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_SHIFT AS S ON S.SHIFT_ID = SPI.SHIFT_ID
                                        LEFT JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        AND CC.IS_DELETED != 1
                                WHERE                 
                                CC.IS_DELETED != 1 and SPI.IS_DELETED !=1
                                ORDER BY SPI.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchNoDueDefaultersbyAcYear:
                    {
                        query = @"SELECT 
                                    SND.STU_NODUE_DEFALTER_ID,
                                    SND.REASON,
                                    SND.IS_ACTIVE,
                                    SPI.FIRST_NAME,
                                    SPI.ROLL_NO,
                                    CC.CLASS_NAME,
                                    DATE_FORMAT(SND.ENTRY_DATE, '%d/%m/%Y') AS ENTRY_DATE
                                FROM
                                    STU_NODUE_DEFAULTERS AS SND
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SND.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SND.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SND.SEMESTER_TYPE
                                WHERE
                                    SND.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SND.SEMESTER_TYPE = ?SEMESTER_TYPE
                                ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.DeleteNodueDefaultersList:
                    {
                        query = @"UPDATE STU_NODUE_DEFAULTERS 
                                                        SET 
                                                            IS_DELETED = ?IS_DELETED
                                                        WHERE
                                                            STU_NODUE_DEFALTER_ID = ?STU_NODUE_DEFALTER_ID;";

                        break;
                    }
                case StudentSQLCommads.ActiveOrDeactiveNodueDefaultersList:
                    {
                        query = @"UPDATE STU_NODUE_DEFAULTERS 
                                                        SET 
                                                            IS_ACTIVE = ?IS_ACTIVE
                                                        WHERE
                                                            STU_NODUE_DEFALTER_ID = ?STU_NODUE_DEFALTER_ID;";

                        break;
                    }
                case StudentSQLCommads.FetchNodueDefaulterByID:
                    {
                        query = @"SELECT 
                                    SND.STU_NODUE_DEFALTER_ID,
                                    SND.STUDENT_ID,
                                    SND.REASON,
                                    SND.SEMESTER_TYPE,
                                    SND.ACADEMIC_YEAR,
                                    SPI.ROLL_NO,
                                    SND.IS_ACTIVE
 
                                FROM
                                    STU_NODUE_DEFAULTERS AS SND
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SND.STUDENT_ID = SPI.STUDENT_ID
                                WHERE
                                    STU_NODUE_DEFALTER_ID = ?STU_NODUE_DEFALTER_ID;";

                        break;
                    }
                case StudentSQLCommads.CheckNDefaultersAlreadyExistOrNot:
                    {
                        query = @"SELECT 
                                    STU_NODUE_DEFALTER_ID,
                                    STUDENT_ID,
                                    REASON,
                                    SEMESTER_TYPE,
                                    ACADEMIC_YEAR
    
                                FROM
                                    STU_NODUE_DEFAULTERS
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID AND ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SEMESTER_TYPE = ?SEMESTER_TYPE;";

                        break;
                    }
                case StudentSQLCommads.saveNodeuDefaulters:
                    {
                        query = @"INSERT INTO STU_NODUE_DEFAULTERS (
                                                            STUDENT_ID,
                                                            REASON,
                                                            SEMESTER_TYPE,
                                                            ACADEMIC_YEAR,
                                                            IS_ACTIVE
                                                        ) VALUES(
                                                            ?STUDENT_ID,
                                                            ?REASON,
                                                            ?SEMESTER_TYPE,
                                                            ?ACADEMIC_YEAR,
                                                            ?IS_ACTIVE);";

                        break;
                    }
                case StudentSQLCommads.UpdateNodeuDefaulters:
                    {
                        query = @"UPDATE STU_NODUE_DEFAULTERS SET 
                                                            STUDENT_ID =?STUDENT_ID,
                                                            REASON =?REASON,
                                                            SEMESTER_TYPE =?SEMESTER_TYPE,
                                                            ACADEMIC_YEAR=?ACADEMIC_YEAR,
                                                            IS_ACTIVE=?IS_ACTIVE 
                                                        WHERE 
                                                            STU_NODUE_DEFALTER_ID=?STU_NODUE_DEFALTER_ID";

                        break;
                    }
                case StudentSQLCommads.FetchProgrammeByApprogramme:
                    {
                        query = @"SELECT 
                        AP.PROGRAMME_GROUP_ID,
                        P.PROGRAMME_ID,
                        AP.SHIFT,
                        AP.PROGRAMME_MODE,
                        CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                ' (',
                                IFNULL(CONCAT(PM.PROGRAMME_MODE), ''),
                                ')',
                                '-',
                                IFNULL(CONCAT(' SHIFT - ', SS.SHIFT_NAME), '')) AS PROGRAMME_NAME
                    FROM
                       CP_PROGRAMME_GROUP AS AP
                            INNER JOIN
                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = AP.PROGRAMME_ID
                            AND P.IS_DELETED != 1
                            INNER JOIN
                        SUP_SHIFT SS ON SS.SHIFT_ID = AP.SHIFT
                            AND SS.IS_DELETED != 1
                            INNER JOIN
                        SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = AP.PROGRAMME_MODE
                    WHERE
                        AP.IS_DELETED != 1;";
                        break;
                    }

                case StudentSQLCommads.BindStudentListForRollNoCreation:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                        UPPER(SP.ADMISSION_NO) AS ADMISSION_NO,
                                        UPPER(ROLL_NO) AS ROLL_NO,
                                        CLS.CLASS_NAME,
                                        CPG.PROGRAMME_GROUP_CODE
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    WHERE
                                            SC.CLASS_ID IN (?CLASS_ID)
                                            AND SP.IS_LEFT !=1 
                                            AND SP.IS_DELETED !=1 
                                            AND SC.IS_DELETED !=1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    ORDER BY SC.CLASS_ID,SP.GENDER, SP.FIRST_NAME , ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.UpdateRollNoByStudentId:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO 
                                                        SET
                                                             ROLL_NO=?ROLL_NO,
                                                             BAR_CODE=?ROLL_NO,
                                                             BIOD_NUMBER=?ROLL_NO,
                                                             RFID=?ROLL_NO
                                                        WHERE 
                                                            STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStuDetailsbyStudentId:
                    {
                        query = @" SELECT 
                                        FIRST_NAME,
                                        ROLL_NO, 
                                        STU_MOBILENO,
                                        STUDENT_ID
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdatePersonalFromTCEntry:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET
                                 FIRST_NAME=?FIRST_NAME,
                                -- STUDENT_ID=?STUDENT_ID, 
                                 FATHER_NAME=?FATHER_NAME, 
                                 GUARDIAN_NAME=?GUARDIAN_NAME, 
                                 NATIONALITY=?NATIONALITY, 
                                 RELIGION=?RELIGION, 
                                --  CASTE=?CASTE, 
                                --  COMMUNITY=?COMMUNITY,
                                 GENDER=?GENDER, 
                                 DATE_OF_BIRTH=?DATE_OF_BIRTH
                                -- IDENTIFICATION_MARK1=?IDENTIFICATION_MARK1,
                                 -- IDENTIFICATION_MARK2=?IDENTIFICATION_MARK2,
                                 -- ADMISSION_DATE=?ADMISSION_DATE,
                                -- ADMITTED_CLASS=?ADMITTED_CLASS,
                                 -- LEAVING_CLASS=?LEAVING_CLASS, 
                                -- LEAVIN_GDATE=?LEAVIN_GDATE, 
                                -- TC_APPLIED_DATE=?TC_APPLIED_DATE, 
                               --  TC_GIVEN_DATE=?TC_GIVEN_DATE,
                                -- CONDUCT=?CONDUCT
                                 WHERE STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchCommunityReportbyAcYearClassYearandClassLevel:
                    {
                        query = @"SELECT 
                                    scm.COMMUNITY,
                                    scm.COMMUNITYID,
                                    COUNT(*) AS COUNT,
                                    cp.PROGRAMME_NAME,
                                    cp.PROGRAMME_ID,
                                    sr.RELIGION,
                                    sr.RELIGIONID,
                                    sg.GENDER_NAME,
                                    sg.GENDER_ID,
                                    scl.CLASSLEVEL,
                                    scl.CLASSLEVELID,
                                    scm.COMMUNITYORDER,
                                    scy.CLASS_YEAR
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SPI.COMMUNITY
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                        INNER JOIN
                                    SUP_RELIGION AS SR ON SR.RELIGIONID = SPI.RELIGION
                                        INNER JOIN
                                    SUP_GENDER AS SG ON SG.GENDER_ID = SPI.GENDER
                                        INNER JOIN
                                    SUP_CLASS_LEVEL AS SCL ON SCL.CLASSLEVELID = CC.CLASS_LEVEL
                                        INNER JOIN
                                    SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID = CC.CLASS_YEAR
                                WHERE
                                    spi.IS_DELETED != 1 AND spi.IS_LEFT != 1
                                        AND sc.ACADEMIC_YEAR = ?AC_YEAR
                                        AND cc.CLASS_YEAR IN (?CLASS_YEAR)
                                        AND CLASS_LEVEL IN (?CLASS_LEVEL)
                                GROUP BY spi.COMMUNITY , RELIGION , cp.PROGRAMME_ID , GENDER_ID,CC.CLASS_YEAR
                                ORDER BY COMMUNITYID;";
                        break;
                    }
                case StudentSQLCommads.FetchClassInchargeByAcYear:
                    {
                        query = @"SELECT 
                                        sc.CLASS_ID, CLASS_NAME
                                    FROM
                                        CP_CLASSES AS SC
                                            INNER JOIN
                                        CLASS_INCHARGE AS CI ON CI.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                       SC.IS_DELETED <> 1
                                            AND CI.IS_DELETED <> 1
                                            AND STAFF_ID = ?STAFF_ID
                                            AND CI.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case StudentSQLCommads.FetchClassWiseStudentForClassIncharge:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                        ' ',
                                                        IFNULL(SP.LAST_NAME, ''))) AS 'FIRST_NAME',
                                        SP.ADMISSION_NO,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                        IFNULL(SP.STU_MOBILENO,
                                                IFNULL(SP.FR_MOBILE, MO_MOBILE)) AS 'MOBILE',
                                        UPPER(C.CLASS_NAME) AS 'CLASS_NAME'
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CLASS_INCHARGE AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_LEFT != 1
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = CCS.STAFF_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID = ?CLASS_ID
                                            AND SC.IS_DELETED != 1
                                            AND CCS.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentDetailsbyStudentId:
                    {
                        query = @"SELECT 
                                        SP.FIRST_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        SP.ADMISSION_NO,
                                        SP.LAST_SCHOOL_OR_COLLEGE,
                                        SP.LAST_STUDIED_PLACE,
                                        CC.CLASS_NAME,
                                        CP.PROGRAMME_NAME AS 'PROGRAMME_NAME',
                                        SP.STU_EMAILID,
                                        SP.PHOTO,
                                        PG.APPTYPE_ID,
                                        CASTE,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        SB.BATCH,
                                        ANNUAL_INCOME,
                                        AADHAR_NUMBER,
                                        STU_MOBILENO,
                                        FR_MOBILE,
                                        SG.GENDER_NAME AS GENDER,
                                        SCM.COMMUNITY,
                                        SMT.MOTHER_TONGUE_NAME AS MOTHER_TONGUE,
                                        SBG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        SR.RELIGION,
                                        SS.SPECIALLY_ABLED_NAME AS SPECIALLY_ABLED,
                                        SN.NATIONALITY_NAME AS NATIONALITY,
                                        DISTRICT,
                                        TAMIL_NAME,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        FR_OCCUPATION,
                                        MO_OCCUPATION,
                                        FR_INCOME,
                                        MO_INCOME,
                                        SAT.STATE_NAME AS STATE_NAME,
                                        CITY,
                                        PINCODE,
                                        ADDRESS_LINE1,
                                        HSS_NO,
                                        SESM.EX_SERVICE_MAN,
                                        HSS_GROUP_CODE,
                                        EMIS_NO,
                                        SEB.EDUCATION_BOARD AS STUDY_BOARD,
                                        HSS_TOTAL,
                                        LAST_SCHOOL_OR_COLLEGE,
                                        MEDIUM_NAME AS MEDIUM_STUDY,
                                        CURUCULAR_NAME AS EXTRA_CURRRICULAR,
                                        SPORTS,
                                        IF(FIRSTGRADUATE=1,'YES','NO') AS FIRSTGRADUATE,
                                        PART_1_LANGUAGE,
                                        LAST_STUDIED_UNIVERSITY,
                                        YEAR_OF_PASSING,
                                        PERCENTAGE,
                                        ALLIED_2,
                                        ALLIED_1,
                                        UMIS_NO,
                                        ENROLLMENT_NO,
                                        MAIN_SUBJECT,
                                        IFSC_CODE,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        ACCOUNT_NO,
                                        RESIDENCY_TYPE_NAME AS RESIDENCY_TYPE,
                                        IDENTIFICATION_MARK1,
                                        IDENTIFICATION_MARK2
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            LEFT JOIN
                                        SUP_RELIGION AS SR ON SR.RELIGIONID = SP.RELIGION
                                            LEFT JOIN
                                        SUP_BLOOD_GROUP AS SBG ON SBG.BLOOD_GROUP_ID = SP.BLOOD_GROUP
                                            LEFT JOIN
                                        SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SP.COMMUNITY
                                            LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SP.GENDER
                                            LEFT JOIN
                                        SUP_SPECIALLYABLED AS SS ON SS.SPECIALLY_ABLED_ID = SP.SPECIALLY_ABLED
                                            LEFT JOIN
                                        SUP_NATIONALITY AS SN ON SN.NATIONALITY_ID = SP.NATIONALITY
                                            LEFT JOIN
                                        SUP_MOTHER_TONGUE AS SMT ON SMT.MOTHER_TONGUE_ID = SP.MOTHER_TONGUE
                                            LEFT JOIN
                                        SUP_EDUCATION_BOARD AS SEB ON SEB.EDUCATION_BOARD_ID = SP.STUDY_BOARD
                                            LEFT JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = SC.BATCH
                                            LEFT JOIN
                                        SUP_EX_SERVICE_MAN AS SESM ON SESM.EX_SERVICE_ID = SP.EX_SERVICE_MAN
                                            LEFT JOIN
                                        SUP_EXTRA_CURICULAR_ACTIVITIES AS SEC ON SEC.CURUCULAR_ID = SP.EXTRA_CURRRICULAR
                                            LEFT JOIN
                                        SUP_RESIDENCY_TYPE AS SRT ON SRT.RESIDENCY_TYPE_ID = SP.RESIDENCY_TYPE
                                            LEFT JOIN
                                        SUP_STATE AS SAT ON SAT.STATE_ID = SP.STATE_ID
                                            LEFT JOIN
                                        SUP_MEDIUMSTUDY AS SMS ON SMS.MEDIUM_ID = SP.MEDIUM_STUDY
                                    WHERE
                                        SC.STUDENT_ID =?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR  = (select Max(ACADEMIC_YEAR) from stu_class where  STUDENT_ID =?STUDENT_ID and is_deleted !=1)
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }

                case StudentSQLCommads.FetchStudentDetailsbyStaffId:
                    {
                        query = @" SELECT 
                                        SPI.USER_CODE,
                                        SPI.FIRST_NAME,
                                        SPI.STAFF_CODE,
                                        SPI.ACADEMIC_YEAR,
                                        SPI.MOBILE_NO,
                                        SPI.ALTER_MOBILE_NO,
                                        SPI.LAST_NAME,
                                        SG.GENDER_NAME,
                                        SPI.KNOWN_AS,
                                        SPI.PLACE_OF_BIRTH,
                                        SPI.IDENTIFICATION_MARK1,
                                        SPI.IDENTIFICATION_MARK2,
                                        SPI.HEIGHT,
                                        SC.COMMUNITY,

                                        SPI.CASTE,
                                        SN.NATIONALITY_NAME,
                                        SMS.MARITAL_STATUS_NAME,
                                        SCA.CATEGORY_NAME,
                                        SSS.STATUS,
                                        SPI.MAIN_SUBJECT,
                                        SPI.ANNUATION_DATE,
                                        SPI.LEAVING_DATE,
                                        SPI.LEAVING_REMARKS,
                                        SPI.USER_FIELD1,
                                        SPI.USER_FIELD2,
                                        SPI.SESSIONS,
                                        
										DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
										DATE_FORMAT(DATE_OF_JOIN, '%d/%m/%Y') AS DATE_OF_JOIN,

                                        SBG.BLOOD_GROUP_NAME,
                                        SR.RELIGION,
                                        SMT.MOTHER_TONGUE_NAME,
                                        SPS.PHYSICAL_STATUS_NAME,
                                        SD.DESIGNATION,
                                        SDE.DEPARTMENT,
										DATE_FORMAT(RETIREMENT_DATE, '%d/%m/%Y') AS RETIREMENT_DATE,
                                        LEAVING_REASON,
                                        PHOTO,
                                        SERVICE_REGNO,
                                        KNOWLEDGE_IN_COMPUTER,
                                        CERTIFICATE1,
                                        CERTIFICATE2,
                                        CERTIFICATE3,
                                        QUALIFICATION_DOC1,
                                        QUALIFICATION_DOC2,
                                        QUALIFICATION_DOC3,
                                        QUALIFICATION,
                                        NON_TEACHING_CATEGORY,
                                        VISA_NUMBER,
                                        STAFF_REGISTERED_DATE,
                                        STAFF_ORDER,
                                        SRO.ROLE_NAME,
                                        PROLE_ID,
                                        PREFIX,
                                        SS.SHIFT_NAME,
                                        ATT_SHIFT,
                                        RFID,
                                        BAR_CODE,
                                        SUT.USER_TYPE_NAME,
                                        GSUITE_MAIL_ID,
                                        G_PASSWORD,
										DATE_FORMAT(DATE_OF_EPF, '%d/%m/%Y') AS DATE_OF_EPF,
                                        GROSS_SALARY,
                                        PP_SALARY,
                                        OTHERS_SALARY,
                                        PACKAGE,
                                        SALARY_BASIC,
                                        IFSC_CODE,
                                        BRANCH_NAME,
                                        BANK_NAME,
                                        BANK_AC_NO,
                                        EMAIL,
                                        PG_EXPERIENCE,
                                        UG_EXPERIENCE,
                                        SSLC_COMPLETION,
                                        HSC_COMPLETION,
                                        UG_COMPLETION,
                                        PG_COMPLETION,
                                        ADDRESS_PERMANENT,
                                        ADDRESS_PRESENT,
                                        BIOD_NUMBER,
                                        OLD_STAFF_CODE
                                    FROM
                                        stf_personal_info AS SPI
                                            LEFT JOIN
                                        sup_gender AS SG ON SPI.GENDER = SG.GENDER_ID
                                            LEFT JOIN
                                        sup_community AS SC ON SC.COMMUNITYID = SPI.COMMUNITY
                                            LEFT JOIN
                                        sup_nationality AS SN ON SN.NATIONALITY_ID = SPI.NATIONALITY
                                            LEFT JOIN
                                        sup_marrital_status AS SMS ON SMS.MARITAL_STAUS_ID = SPI.MARITAL_STATUS
                                            LEFT JOIN
                                        sup_category AS SCA ON SCA.CATEGORY_ID = SPI.CATEGORY
                                            LEFT JOIN
                                        sup_staff_status AS SSS ON SSS.STATUS_ID = SPI.STAFF_STATUS
                                            LEFT JOIN
                                        sup_blood_group AS SBG ON SBG.BLOOD_GROUP_ID = SPI.BLOOD_GROUP
                                            LEFT JOIN
                                        sup_religion AS SR ON SR.RELIGIONID = SPI.RELIGION
                                            LEFT JOIN
                                        sup_mother_tongue AS SMT ON SMT.MOTHER_TONGUE_ID = SPI.MOTHER_TONGUE
                                            LEFT JOIN
                                        sup_physical_status AS SPS ON SPS.PHYSICAL_STATUS_ID = SPI.PHYSICAL_STATUS
                                            LEFT JOIN
                                        sup_designation AS SD ON SD.DESIGNATIONID = SPI.DESIGNATION
                                            LEFT JOIN
                                        CP_department AS SDE ON SDE.DEPARTMENT_ID = SPI.DEPARTMENT
                                            LEFT JOIN
                                        sup_shift AS SS ON find_in_set(SPI.SHIFT,SS.SHIFT_ID )
                                            LEFT JOIN
                                        sup_role AS SRO ON SRO.ROLE_ID = SPI.ROLE
                                            LEFT JOIN
                                        sup_user_type AS SUT ON SUT.USER_TYPE_ID = SPI.USER_TYPE WHERE SPI.STAFF_ID=?STAFF_ID";


                        break;
                    }
                case StudentSQLCommads.Fetchstatsselected:
                    {
                        query = @"SELECT 
                                      ?SELECTED_NAME as SELECTED_NAME,
                                      CC.CLASS_NAME,
                                      CC.CLASS_YEAR,
                                      SR.RELIGIONID,
                                      CASE 
                                          WHEN ?EMPTY_FIELD = 0 AND (TRIM(?SELECTED_FIELD) IS NULL OR TRIM(?SELECTED_FIELD) = '') THEN 'No Data'
                                          ELSE ?SELECTED_FIELD
                                      END AS SELECTED_FIELD,
                                      COUNT(SC.STUDENT_ID) AS STUDENT_COUNT
                                  FROM
                                      STU_CLASS AS SC
                                          INNER JOIN
                                      STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID AND SP.IS_LEFT IN(0,?IS_LEFT) AND SP.IS_DELETED!=1
                                          LEFT JOIN
                                      STU_ADDRESS AS SA ON SP.STUDENT_ID = SA.STUDENT_ID
                                          LEFT JOIN
                                      CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                          LEFT JOIN
                                      CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                          LEFT JOIN
                                      SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                          LEFT JOIN
                                      SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                          LEFT JOIN
                                      SUP_RELIGION AS SR ON SR.RELIGIONID = SP.RELIGION
                                          LEFT JOIN
                                      SUP_GENDER AS SG ON SG.GENDER_ID = SP.GENDER
                                          LEFT JOIN
                                      CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID AND CC.CLASS_YEAR IN (?CLASS_YEAR)
                                          LEFT JOIN
                                      SUP_MOTHER_TONGUE AS SM ON SM.MOTHER_TONGUE_ID = SP.MOTHER_TONGUE
                                          LEFT JOIN
                                      SUP_OCCUPATION AS SO ON SO.OCCUPATION_ID = SP.FR_OCCUPATION
                                          LEFT JOIN
                                      SUP_BLOOD_GROUP AS BG ON BG.BLOOD_GROUP_ID = SP.BLOOD_GROUP
                                          LEFT JOIN
                                      SUP_COMMUNITY AS C ON C.COMMUNITYID = SP.COMMUNITY
                                          LEFT JOIN
                                      SUP_NATIONALITY AS N ON N.NATIONALITY_ID = SP.NATIONALITY
                                          LEFT JOIN
                                      CP_CLASSES AS AC ON AC.CLASS_ID = SP.ADMITTED_CLASS
                                          LEFT JOIN
                                      SUP_CLASS_YEAR AS CY ON CY.CLASS_YEAR_ID = AC.CLASS_YEAR
                                          LEFT JOIN 
                                      SUP_COUNTRY AS SCC ON SCC.COUNTRY_ID=SA.C_COUNTRY
                                          LEFT JOIN
                                      SUP_EDUCATION_BOARD AS EB ON EB.EDUCATION_BOARD_ID=SP.EDUCATION_BOARD_ID
                                  WHERE
                                      SC.ACADEMIC_YEAR = ?AC_YEAR
                                      AND SC.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                      AND CC.CLASS_LEVEL = ?CLASS_LEVEL
                                      AND (
                                          ?EMPTY_FIELD = 0 
                                          OR (?EMPTY_FIELD = 1 AND ?SELECTED_FIELD IS NOT NULL AND TRIM(?SELECTED_FIELD) != '')
                                      )
                                  GROUP BY 
                                      CC.CLASS_NAME,
                                      CASE 
                                          WHEN ?EMPTY_FIELD = 0 AND (TRIM(?SELECTED_FIELD) IS NULL OR TRIM(?SELECTED_FIELD) = '') THEN 'No Data'
                                          ELSE ?SELECTED_FIELD
                                      END
                                  ORDER BY CC.CLASS_ID, SP.REGISTER_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentIdbyRollNo:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        ROLL_NO
                                    FROM
                                        stu_personal_info
                                    WHERE
                                        ROLL_NO = ?ROLL_NO AND IS_DELETED !=1;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentAcYearbyStudentId:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        ROLL_NO,
                                        MAX(sc.ACADEMIC_YEAR) AS ACADEMIC_YEAR
                                    FROM
                                        STU_PERSONAL_INFO AS SPI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                    WHERE
                                        SPI.STUDENT_ID = ?STUDENT_ID AND SC.IS_DELETED !=1;";
                        break;
                    }
                case StudentSQLCommads.FetchLeftStudentListByAcYear:
                    {
                        query = @"SELECT 
                                     SC.STUDENT_ID,
                                     SP.ROLL_NO,
                                     SP.ADMISSION_NO,
                                     P.PROGRAMME_NAME,
                                     CC.CLASS_NAME,
                                     CP.PROGRAMME_GROUP_ID,
                                     SP.IS_DELETED,
                                     CC.CLASS_ID,
                                     FIRST_NAME,
                                     DATE_FORMAT(DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH,
                                     REMARKS,
                                     DATE_FORMAT(ENTRY_DATE, '%d-%m-%Y') AS ENTRY_DATE
                                 FROM
                                     STU_CLASS AS SC
                                         INNER JOIN
                                     STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                         AND SP.IS_LEFT = 1
                                         INNER JOIN
                                     CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                         INNER JOIN
                                     CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                         INNER JOIN
                                     CP_PROGRAMME AS P ON P.PROGRAMME_ID = CP.PROGRAMME_ID
                                 WHERE
                                     SC.ACADEMIC_YEAR in (?AC_YEAR) AND SP.IS_DELETED !=1
                                 ORDER BY PROGRAMME_GROUP_ID, CLASS_YEAR, ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchOverAllStudentCumulativeStrengthReport:
                    {
                        query = @"SELECT 
                                    sc.CLASS_ID,
                                    sc.PROGRAMME_GROUP_ID,
                                    sc.ACADEMIC_YEAR,
                                    CONCAT(cp.PROGRAMME_NAME,
                                            ' ',
                                            IF(cpg.SHIFT = 1, ' DAY ', ' EVE '),
                                            '  ',
                                            IFNULL(scs.section, '')) AS PROGRAMME_NAME,
                                    cpg.SHIFT,
                                    ccl.CLASSLEVEL,
                                    cd.DEPARTMENT,
                                    ccl.CLASSLEVELID,
                                    COUNT(*) AS STUDENT_COUNT,
                                    CP.PROGRAMME_ID,
                                    cc.CLASS_YEAR,
                                    CPG.PROGRAMME_MODE,
                                    SRE.RELIGION,
                                    SGE.GENDER_NAME,
                                    SCM.COMMUNITY
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED <> 1
                                        AND SPI.IS_LEFT <> 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        AND CC.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                        cp_department as cd on CD.DEPARTMENT_ID = CC.DEPARTMENT
                                        inner join 
                                    SUP_CLASS_LEVEL AS CCL ON CCL.CLASSLEVELID = CC.CLASS_LEVEL
                                        LEFT JOIN
                                    SUP_CLASS_SECTION AS SCS ON SCS.SECTION_ID = CC.SECTION
                                        LEFT JOIN
                                    SUP_RELIGION AS SRE ON SRE.RELIGIONID = SPI.RELIGION
                                        LEFT JOIN
                                    SUP_GENDER AS SGE ON SGE.GENDER_ID = SPI.GENDER
                                        LEFT JOIN
                                    SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    sc.IS_DELETED <> 1
                                        AND ccl.CLASSLEVELID IN (?CLASS_LEVEL)
                                        AND sc.ACADEMIC_YEAR = ?AC_YEAR
                               GROUP BY sc.CLASS_ID, sc.PROGRAMME_GROUP_ID, cc.CLASS_YEAR, cpg.SHIFT, cp.PROGRAMME_NAME, ccl.CLASSLEVELID, sc.ACADEMIC_YEAR,RELIGION, GENDER_NAME, COMMUNITY
                                ORDER BY SHIFT , CLASSLEVELID , PROGRAMME_GROUP_ID;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidateAdmissionDetailsReport:
                    {
                        query = @"SELECT 
                                    cpg.PROGRAMME_GROUP_ID,
                                    cp.PROGRAMME_ID,
                                    cp.PROGRAMME_CODE,
                                    cp.PROGRAMME_NAME,
                                    cpg.SHIFT,
                                    ccl.CLASSLEVEL,
                                    ccl.CLASSLEVELID,
                                    cp.MEDIUM_OF_INSTRACTION,
                                    COALESCE(m.NO_OF_SEATS, m_alt.NO_OF_SEATS, 0) AS SANCTIONED_INTAKE,
                                    IFNULL(spi.GENDER, 0) AS GENDER_ID,
                                    IFNULL(scm.COMMUNITY, 'Others') AS COMMUNITY_NAME,
                                    IFNULL(scm.COMMUNITYID, 0) AS COMMUNITY_ID,
                                    IFNULL(spi.SPECIALLY_ABLED, 0) AS SPECIALLY_ABLED,
                                    COUNT(spi.STUDENT_ID) AS STUDENT_COUNT
                                FROM CP_CLASSES cc
                                INNER JOIN CP_PROGRAMME_GROUP cpg ON cpg.PROGRAMME_GROUP_ID = cc.PROGRAMME_GROUP_ID
                                INNER JOIN CP_PROGRAMME cp ON cp.PROGRAMME_ID = cpg.PROGRAMME_ID
                                INNER JOIN SUP_CLASS_LEVEL ccl ON ccl.CLASSLEVELID = cc.CLASS_LEVEL
                                LEFT JOIN ADM_MAXIMUM_IN_TAKES m ON m.PROGRAMME_ID = cpg.PROGRAMME_GROUP_ID 
                                                               AND m.ACADEMIC_YEAR = ?AC_YEAR 
                                                               AND m.IS_DELETED = 0
                                LEFT JOIN ADM_MAXIMUM_IN_TAKES m_alt ON m_alt.PROGRAMME_ID = cp.PROGRAMME_ID 
                                                                   AND (m_alt.SHIFT = cpg.SHIFT OR m_alt.SHIFT = 0)
                                                                   AND m_alt.ACADEMIC_YEAR = ?AC_YEAR 
                                                                   AND m_alt.IS_DELETED = 0
                                LEFT JOIN STU_CLASS sc ON sc.CLASS_ID = cc.CLASS_ID 
                                                      AND sc.ACADEMIC_YEAR = ?AC_YEAR 
                                                      AND sc.IS_DELETED != 1
                                LEFT JOIN STU_PERSONAL_INFO spi ON spi.STUDENT_ID = sc.STUDENT_ID 
                                                               AND spi.IS_DELETED != 1 
                                                               AND spi.IS_LEFT != 1
                                LEFT JOIN SUP_COMMUNITY scm ON scm.COMMUNITYID = spi.COMMUNITY
                                WHERE cc.IS_DELETED != 1
                                  AND cc.CLASS_YEAR IN (?CLASS_YEAR)
                                  AND ccl.CLASSLEVELID IN (?CLASS_LEVEL)
                                  AND cpg.SHIFT IN (?SHIFT)
                                GROUP BY cpg.PROGRAMME_GROUP_ID, ccl.CLASSLEVELID, spi.GENDER, scm.COMMUNITY, scm.COMMUNITYID, spi.SPECIALLY_ABLED
                                ORDER BY ccl.CLASSLEVELID, cp.PROGRAMME_NAME, cpg.SHIFT;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentFeeDueDetailsForMeetWithoutNegative:
                    {
                        query = @"SELECT 
                                            SPI.STUDENT_ID,
                                            SPI.FIRST_NAME,
                                            SPI.ROLL_NO,
                                            SFD.STU_DUE_ID,
                                            CC.CLASS_NAME,
                                            CC.CLASS_ID,

                                            SFD.TERMFEE1,
                                            SFD.TERMFEE1PAID,

                                            GREATEST(
                                                IFNULL(SFD.TERMFEE1, 0) 
                                                - IFNULL(SFD.TERMFEE1PAID, 0),
                                                0
                                            )
                                            +
                                            GREATEST(
                                                IFNULL(SFD.TERMFEE2, 0) 
                                                - IFNULL(SFD.TERMFEE2PAID, 0),
                                                0
                                            ) AS TERMFEE1DUE,

                                            IFNULL(EF.EXAMFEEDUE, 0) AS EXAMFEEDUE,

                                            SFD.TERMFEE2,
                                            SFD.TERMFEE2PAID,

                                            IFNULL(EF.EXAMFEE, 0) AS EXAMFEE1,
                                            IFNULL(EF.EXAMFEEPAID, 0) AS EXAMFEE1PAID,

                                            IFNULL(EF.EXAMFEEDUE, 0) AS EXAMFEE1DUE,

                                            0 AS EXAMFEE2,
                                            0 AS EXAMFEE2PAID,

                                            0 AS EXAMFEE2DUE,

                                            IFNULL(BFSA.B_CREDIT, 0) AS BUSFEE,

                                            IFNULL(BFSA.B_DEBIT, 0) AS BUSFEEPAID,

                                            GREATEST(
                                                IFNULL(BFSA.B_CREDIT, 0)
                                                - IFNULL(BFSA.B_DEBIT, 0),
                                                0
                                            ) AS BUSFEEDUE,
                                            IFNULL(HFSA.CREDIT, 0) AS HOSTELFEE,

                                            IFNULL(HFSA.DEBIT, 0) AS HOSTELFEEPAID,

                                            GREATEST(
                                                IFNULL(HFSA.CREDIT, 0)
                                                - IFNULL(HFSA.DEBIT, 0),
                                                0
                                            ) AS HOSTELFEEDUE,

                                            IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                                            (
                                                IFNULL(ARR.ARREAR_DUE, 0)

                                                + GREATEST(
                                                    IFNULL(SFD.TERMFEE1, 0)
                                                    - IFNULL(SFD.TERMFEE1PAID, 0),
                                                    0
                                                )

                                                + GREATEST(
                                                    IFNULL(SFD.TERMFEE2, 0)
                                                    - IFNULL(SFD.TERMFEE2PAID, 0),
                                                    0
                                                )

                                                + IFNULL(EF.EXAMFEEDUE, 0)

                                                + GREATEST(
                                                    IFNULL(BFSA.B_CREDIT, 0)
                                                    - IFNULL(BFSA.B_DEBIT, 0),
                                                    0
                                                )

                                                + GREATEST(
                                                    IFNULL(HFSA.CREDIT, 0)
                                                    - IFNULL(HFSA.DEBIT, 0),
                                                    0
                                                )

                                            ) AS TOTAL_DUE_AMOUNT,

                                            SFD.ENTRY_ID,
                                            SFD.ENTRY_DATE

                                        FROM STU_PERSONAL_INFO AS SPI

                                        LEFT JOIN STU_FEE_DUE_INFO AS SFD 
                                            ON SPI.STUDENT_ID = SFD.STUDENT_ID AND SFD.ACADEMIC_YEAR = ?AC_YEAR  AND SFD.IS_DELETED <> 1

                                        INNER JOIN STU_CLASS AS SC 
                                            ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR

                                        INNER JOIN CP_CLASSES AS CC 
                                            ON CC.CLASS_ID = SC.CLASS_ID

                                        INNER JOIN CP_PROGRAMME_GROUP AS CPG 
                                            ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID

                                        LEFT JOIN
                                        (
                                            SELECT 
                                                FSA.STUDENT_ID,
                                                SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE,
                                                SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEEPAID,
                                                CAST(GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS SIGNED) AS EXAMFEEDUE
                                            FROM FEE_STUDENT_ACCOUNT AS FSA
                                            INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                            INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN ACADEMIC_YEAR AS AY ON (AY.AC_YEAR = SFF.ACADEMIC_YEAR OR AY.ACADEMIC_YEAR_ID = SFF.ACADEMIC_YEAR) AND AY.ACTIVE_YEAR = 1 AND AY.IS_DELETED <> 1
                                            INNER JOIN SUP_SEMESTER_TYPE AS SST ON (SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE OR SST.SEMESTER_TYPE = SFF.SEMESTER_TYPE) AND SST.IS_ACTIVE = 1 AND SST.IS_DELETED <> 1
                                            WHERE
                                                SFT.FREQUENCY_TYPE_ID = 1
                                                AND FSA.IS_DELETED <> 1
                                                AND SFF.IS_DELETED <> 1
                                            GROUP BY FSA.STUDENT_ID
                                        ) AS EF ON EF.STUDENT_ID = SPI.STUDENT_ID

                                        LEFT JOIN
                                        (
                                            SELECT 
                                                FSA.STUDENT_ID,
                                                FSA.ACADEMIC_YEAR,

                                                SUM(IFNULL(FSA.CREDIT, 0)) AS CREDIT,
                                                SUM(IFNULL(FSA.DEBIT, 0)) AS DEBIT

                                            FROM FEE_STUDENT_ACCOUNT AS FSA

                                            INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF
                                                ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                            INNER JOIN SUP_FEE_FREQUENCY AS SFF
                                                ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                            INNER JOIN SUP_FREQUENCY_TYPE AS SFT
                                                ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                            WHERE
                                                SFT.FREQUENCY_TYPE_ID = 4
                                                AND FSA.IS_DELETED <> 1

                                            GROUP BY
                                                FSA.STUDENT_ID,
                                                FSA.ACADEMIC_YEAR

                                        ) AS HFSA
                                            ON HFSA.STUDENT_ID = SPI.STUDENT_ID

                                        LEFT JOIN
                                        (
                                            SELECT 
                                                FSA.STUDENT_ID,
                                                FSA.ACADEMIC_YEAR,

                                                SUM(IFNULL(FSA.CREDIT, 0)) AS B_CREDIT,
                                                SUM(IFNULL(FSA.DEBIT, 0)) AS B_DEBIT

                                            FROM FEE_STUDENT_ACCOUNT AS FSA

                                            INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF
                                                ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                            INNER JOIN SUP_FEE_FREQUENCY AS SFF
                                                ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                            INNER JOIN SUP_FREQUENCY_TYPE AS SFT
                                                ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                            WHERE
                                                SFT.FREQUENCY_TYPE_ID = 8
                                                AND FSA.IS_DELETED <> 1

                                            GROUP BY
                                                FSA.STUDENT_ID,
                                                FSA.ACADEMIC_YEAR

                                        ) AS BFSA
                                            ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                        LEFT JOIN
                                        (
                                            SELECT 
                                                STUDENT_ID,

                                                SUM(
                                                    GREATEST(
                                                        IFNULL(TERMFEE1, 0)
                                                        - IFNULL(TERMFEE1PAID, 0),
                                                        0
                                                    )
                                                    +
                                                    GREATEST(
                                                        IFNULL(TERMFEE2, 0)
                                                        - IFNULL(TERMFEE2PAID, 0),
                                                        0
                                                    )
                                                ) AS ARREAR_DUE

                                            FROM STU_FEE_DUE_INFO

                                            WHERE
                                                ACADEMIC_YEAR < ?AC_YEAR
                                                AND IS_DELETED <> 1

                                            GROUP BY STUDENT_ID

                                        ) ARR 
                                            ON ARR.STUDENT_ID = SPI.STUDENT_ID
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID IN (?CLASS_ID)
                                            AND SC.IS_DELETED <> 1
                                            AND SPI.IS_DELETED <> 1
                                            AND SPI.IS_LEFT <> 1

                                        GROUP BY
                                            SC.STUDENT_ID,
                                            SC.ACADEMIC_YEAR

                                        ORDER BY
                                            SC.PROGRAMME_GROUP_ID,
                                            CC.CLASS_YEAR,
                                            SC.CLASS_ID,
                                            SPI.ROLL_NO;";
                        break;
                    }
                //case StudentSQLCommads.FetchStudentFeeDueDetailsForMeetWithoutNegative:
                //    {
                //        query = @"SELECT 
                //                            sfd.STUDENT_ID,
                //                            spi.FIRST_NAME,
                //                            spi.ROLL_NO,
                //                            STU_DUE_ID,
                //                            cc.CLASS_NAME,
                //                            cc.CLASS_ID,
                //                            sfd.TERMFEE1,
                //                            TERMFEE1PAID,
                //                            GREATEST(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0),
                //                                    0) + GREATEST(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0),
                //                                    0) AS TERMFEE1DUE,
                //                            GREATEST(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0),
                //                                    0) + GREATEST(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0),
                //                                    0) AS EXAMFEEDUE,
                //                            sfd.TERMFEE2,
                //                            sfd.TERMFEE2PAID,
                //                            sfd.EXAMFEE1,
                //                            EXAMFEE1PAID,
                //                            GREATEST(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0),
                //                                    0) AS EXAMFEE1DUE,
                //                            sfd.EXAMFEE2,
                //                            EXAMFEE2PAID,
                //                            GREATEST(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0),
                //                                    0) AS EXAMFEE2DUE,
                //                            B_CREDIT AS BUSFEE,
                //                            B_DEBIT AS BUSFEEPAID,
                //                            GREATEST(IFNULL(BFSA.B_CREDIT, 0) - IFNULL(BFSA.B_DEBIT, 0),
                //                                    0) AS BUSFEEDUE,
                //                            IF(HFSA.CREDIT IS NULL,
                //                                HFSA.CREDIT,
                //                                SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                //                            GREATEST(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0),
                //                                    0) AS HOSTELFEEDUE,
                //                            IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                //                            (IFNULL(ARR.ARREAR_DUE, 0) + GREATEST(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0),
                //                                    0) + GREATEST(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0),
                //                                    0) + GREATEST(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0),
                //                                    0) + GREATEST(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0),
                //                                    0) + GREATEST(IFNULL(BFSA.B_CREDIT, 0) - IFNULL(BFSA.B_DEBIT, 0),
                //                                    0) + GREATEST(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0),
                //                                    0)) AS TOTAL_DUE_AMOUNT,
                //                            SFD.ENTRY_ID,
                //                            SFD.ENTRY_DATE
                //                        FROM
                //                            STU_PERSONAL_INFO AS SPI
                //                                INNER JOIN
                //                            STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                //                                INNER JOIN
                //                            STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                //                                AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                //                                INNER JOIN
                //                            CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                //                                INNER JOIN
                //                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                //                                LEFT JOIN
                //                            (SELECT 
                //                                STUDENT_ID,
                //                                    SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                //                                    SUM(IFNULL(DEBIT, '0')) AS DEBIT
                //                            FROM
                //                                HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                //                            WHERE
                //                                hf.IS_DELETED <> 1
                //                            GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                //                                LEFT JOIN
                //                            (SELECT 
                //                                STUDENT_ID,
                //                                    SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                //                                    SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                //                            FROM
                //                                BUS_FEE_STUDENT_ACCOUNT AS Bf
                //                            WHERE
                //                                Bf.IS_DELETED <> 1
                //                            GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                //                                LEFT JOIN
                //                            (SELECT 
                //                                STUDENT_ID,
                //                                    SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                //                            FROM
                //                                STU_FEE_DUE_INFO
                //                            WHERE
                //                                ACADEMIC_YEAR < ?AC_YEAR AND IS_DELETED <> 1
                //                            GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SPI.STUDENT_ID
                //                        WHERE
                //                            SFD.ACADEMIC_YEAR = ?AC_YEAR
                //                            AND SC.CLASS_ID IN (?CLASS_ID)
                //                                AND SC.IS_DELETED <> 1
                //                                AND SFD.IS_DELETED <> 1
                //                                AND SPI.IS_DELETED <> 1
                //                                AND SPI.IS_LEFT <> 1
                //                        GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                //                        ORDER BY SC.PROGRAMME_GROUP_ID,CC.CLASS_YEAR, SC.CLASS_ID,ROLL_NO;";
                //        break;
                //    }
                case StudentSQLCommads.FetchExamHallTicketByArrearOnlyForAdmin:
                    {
                        query = @"SELECT 
                                        spi.STUDENT_ID,
                                        EXAM_NAME,
                                        sti.FIRST_NAME,
                                        spi.ROLL_NO,
                                        spi.REGISTER_NO,
                                        spi.UMIS_NO,
                                        cc.CLASS_NAME,
                                        spi.PHOTO,
                                        IFNULL(DATE_FORMAT(STI.DATE_OF_BIRTH, '%d/%m/%Y'),
                                                DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y')) AS DATE_OF_BIRTH,
                                        ccr.COURSE_TITLE,
                                        ccr.COURSE_CODE,
                                        sti.ELIGIBLE_TYPE,
                                        DATE_FORMAT(sti.DATE_OF_EXAM, '%d/%m/%Y') AS DATE_OF_EXAM,
                                        sti.SESSION_NAME,
                                        sti.HALL_NAME,
                                        DATE_FORMAT(HALL_TICKET_PUBLICATION_DATE, '%d/%m/%Y') AS HALL_TICKET_PUBLICATION_DATE
                                    FROM
                                        EXAM_SETTING AS ES
                                            INNER JOIN
                                        STU_HALL_TICKET_INFO AS STI ON STI.EXAM_SETTING_ID = ES.EXAM_SETTING_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = STI.STUDENT_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = STI.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = STI.COURSE_ID
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = STI.SEMESTER_TYPE
                                            AND SS.IS_ACTIVE = 1
                                            INNER JOIN
                                        EXAM_REGISTRATION_?AC_YEAR AS ER ON ER.STUDENT_ID = STI.STUDENT_ID
                                            AND ER.EXAM_SETTING_ID = STI.EXAM_SETTING_ID
                                            AND STI.COURSE_ID = ER.COURSE_ID
                                            AND ER.STATUS = 'A'  and ER.IS_DELETED!=1
                                    WHERE
                                        SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                            AND SPI.ROLL_NO = '?ROLL_NO'
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND STI.IS_DELETED != 1
                                            AND ES.IS_DELETED != 1
                                            AND ES.IS_ACTIVE = 1
                                            AND HALL_TICKET_PUBLICATION_DATE <= CURDATE()
                                    GROUP BY STI.STUDENT_ID , STI.COURSE_ID ORDER BY STI.DATE_OF_EXAM;";
                        break;
                    }
                case StudentSQLCommads.PaidUnpaidAttendanceList:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID,
                                        SPI.FIRST_NAME,
                                        SPI.ROLL_NO,
                                        SFD.STU_DUE_ID,
                                        CC.CLASS_NAME,
                                        CC.CLASS_ID,

                                        SFD.TERMFEE1,
                                        SFD.TERMFEE1PAID,

                                        GREATEST(
                                            IFNULL(SFD.TERMFEE1, 0) - IFNULL(SFD.TERMFEE1PAID, 0),
                                            0
                                        )
                                        +
                                        GREATEST(
                                            IFNULL(SFD.TERMFEE2, 0) - IFNULL(SFD.TERMFEE2PAID, 0),
                                            0
                                        ) AS TERMFEE1DUE,

                                        IFNULL(EF.EXAMFEEDUE, 0) AS EXAMFEEDUE,

                                        SFD.TERMFEE2,
                                        SFD.TERMFEE2PAID,

                                        IFNULL(EF.EXAMFEE, 0) AS EXAMFEE1,
                                        IFNULL(EF.EXAMFEEPAID, 0) AS EXAMFEE1PAID,

                                        IFNULL(EF.EXAMFEEDUE, 0) AS EXAMFEE1DUE,

                                        0 AS EXAMFEE2,
                                        0 AS EXAMFEE2PAID,

                                        0 AS EXAMFEE2DUE,

                                        IFNULL(BFSA.B_CREDIT, 0) AS BUSFEE,

                                        IFNULL(BFSA.B_DEBIT, 0) AS BUSFEEPAID,

                                        GREATEST(
                                            IFNULL(BFSA.B_CREDIT, 0)
                                            - IFNULL(BFSA.B_DEBIT, 0),
                                            0
                                        ) AS BUSFEEDUE,

                                        IFNULL(HFSA.CREDIT, 0) AS HOSTELFEE,

                                        IFNULL(HFSA.DEBIT, 0) AS HOSTELFEEPAID,

                                        GREATEST(
                                            IFNULL(HFSA.CREDIT, 0)
                                            - IFNULL(HFSA.DEBIT, 0),
                                            0
                                        ) AS HOSTELFEEDUE,

                                        IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                                        (
                                            IFNULL(ARR.ARREAR_DUE, 0)

                                            + GREATEST(
                                                IFNULL(SFD.TERMFEE1, 0)
                                                - IFNULL(SFD.TERMFEE1PAID, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(SFD.TERMFEE2, 0)
                                                - IFNULL(SFD.TERMFEE2PAID, 0),
                                                0
                                            )

                                            + IFNULL(EF.EXAMFEEDUE, 0)

                                            + GREATEST(
                                                IFNULL(BFSA.B_CREDIT, 0)
                                                - IFNULL(BFSA.B_DEBIT, 0),
                                                0
                                            )

                                            + GREATEST(
                                                IFNULL(HFSA.CREDIT, 0)
                                                - IFNULL(HFSA.DEBIT, 0),
                                                0
                                            )

                                        ) AS TOTAL_DUE_AMOUNT,

                                        SFD.ENTRY_ID,
                                        SFD.ENTRY_DATE

                                    FROM STU_PERSONAL_INFO AS SPI

                                    LEFT JOIN STU_FEE_DUE_INFO AS SFD
                                        ON SPI.STUDENT_ID = SFD.STUDENT_ID AND SFD.ACADEMIC_YEAR = ?AC_YEAR AND SFD.IS_DELETED <> 1
                                    INNER JOIN STU_CLASS AS SC
                                        ON SC.STUDENT_ID = SPI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR

                                    INNER JOIN CP_CLASSES AS CC
                                        ON CC.CLASS_ID = SC.CLASS_ID

                                    INNER JOIN CP_PROGRAMME_GROUP AS CPG
                                        ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            SUM(IFNULL(FSA.CREDIT, 0)) AS EXAMFEE,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS EXAMFEEPAID,
                                            CAST(GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS SIGNED) AS EXAMFEEDUE
                                        FROM FEE_STUDENT_ACCOUNT AS FSA
                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                        INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                        INNER JOIN ACADEMIC_YEAR AS AY ON (AY.AC_YEAR = SFF.ACADEMIC_YEAR OR AY.ACADEMIC_YEAR_ID = SFF.ACADEMIC_YEAR) AND AY.ACTIVE_YEAR = 1 AND AY.IS_DELETED <> 1
                                        INNER JOIN SUP_SEMESTER_TYPE AS SST ON (SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE OR SST.SEMESTER_TYPE = SFF.SEMESTER_TYPE) AND SST.IS_ACTIVE = 1 AND SST.IS_DELETED <> 1
                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 1
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                        GROUP BY FSA.STUDENT_ID
                                    ) AS EF ON EF.STUDENT_ID = SPI.STUDENT_ID
                                    LEFT JOIN
                                    (
                                        SELECT
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS CREDIT,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS DEBIT

                                        FROM FEE_STUDENT_ACCOUNT AS FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY AS SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 4
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR

                                    ) AS HFSA
                                        ON HFSA.STUDENT_ID = SPI.STUDENT_ID

                                    LEFT JOIN
                                    (
                                        SELECT
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS B_CREDIT,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS B_DEBIT

                                        FROM FEE_STUDENT_ACCOUNT AS FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY AS SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 8
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR

                                    ) AS BFSA
                                        ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                                    LEFT JOIN
                                    (
                                        SELECT
                                            STUDENT_ID,

                                            SUM(
                                                GREATEST(
                                                    IFNULL(TERMFEE1, 0)
                                                    - IFNULL(TERMFEE1PAID, 0),
                                                    0
                                                )
                                                +
                                                GREATEST(
                                                    IFNULL(TERMFEE2, 0)
                                                    - IFNULL(TERMFEE2PAID, 0),
                                                    0
                                                )
                                            ) AS ARREAR_DUE

                                        FROM STU_FEE_DUE_INFO

                                        WHERE
                                            ACADEMIC_YEAR < ?AC_YEAR
                                            AND IS_DELETED <> 1

                                        GROUP BY STUDENT_ID

                                    ) ARR
                                        ON ARR.STUDENT_ID = SPI.STUDENT_ID


                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.CLASS_ID IN (?CLASS_ID)
                                        AND SC.IS_DELETED <> 1
                                        AND SPI.IS_DELETED <> 1
                                        AND SPI.IS_LEFT <> 1

                                    GROUP BY
                                        SC.STUDENT_ID,
                                        SC.ACADEMIC_YEAR
                                        -- p having TOTAL_DUE_AMOUNT=0
                                        -- u having TOTAL_DUE_AMOUNT!=0
                                    ORDER BY
                                        CPG.PROGRAMME_GROUP_ID,
                                        SC.CLASS_ID,
                                        SPI.ROLL_NO;";
                        break;
                    }
                //    case StudentSQLCommads.FetchExamHallTicketByArrearOnlyForAdmin:
                //    {
                //        query = @"SELECT 
                //                        spi.STUDENT_ID,
                //                        EXAM_NAME,
                //                        sti.FIRST_NAME,
                //                        spi.ROLL_NO,
                //                        spi.REGISTER_NO,
                //                        spi.UMIS_NO,
                //                        cc.CLASS_NAME,
                //                        spi.PHOTO,
                //                        IFNULL(DATE_FORMAT(STI.DATE_OF_BIRTH, '%d/%m/%Y'),
                //                                DATE_FORMAT(SPI.DATE_OF_BIRTH, '%d/%m/%Y')) AS DATE_OF_BIRTH,
                //                        ccr.COURSE_TITLE,
                //                        ccr.COURSE_CODE,
                //                        sti.ELIGIBLE_TYPE,
                //                        DATE_FORMAT(sti.DATE_OF_EXAM, '%d/%m/%Y') AS DATE_OF_EXAM,
                //                        sti.SESSION_NAME,
                //                        sti.HALL_NAME,
                //                        DATE_FORMAT(HALL_TICKET_PUBLICATION_DATE, '%d/%m/%Y') AS HALL_TICKET_PUBLICATION_DATE
                //                    FROM
                //                        EXAM_SETTING AS ES
                //                            INNER JOIN
                //                        STU_HALL_TICKET_INFO AS STI ON STI.EXAM_SETTING_ID = ES.EXAM_SETTING_ID
                //                            INNER JOIN
                //                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = STI.STUDENT_ID
                //                            INNER JOIN
                //                        STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                //                            AND SC.ACADEMIC_YEAR = STI.ACADEMIC_YEAR
                //                            INNER JOIN
                //                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                //                            INNER JOIN
                //                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                //                            INNER JOIN
                //                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                //                            INNER JOIN
                //                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = STI.COURSE_ID
                //                            INNER JOIN
                //                        SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = STI.SEMESTER_TYPE
                //                            AND SS.IS_ACTIVE = 1
                //                            INNER JOIN
                //                        EXAM_REGISTRATION_?AC_YEAR AS ER ON ER.STUDENT_ID = STI.STUDENT_ID
                //                            AND ER.EXAM_SETTING_ID = STI.EXAM_SETTING_ID
                //                            AND STI.COURSE_ID = ER.COURSE_ID
                //                            AND ER.STATUS = 'A'  and ER.IS_DELETED!=1
                //                    WHERE
                //                        SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                //                            AND SPI.ROLL_NO = '?ROLL_NO'
                //                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                //                            AND STI.IS_DELETED != 1
                //                            AND ES.IS_DELETED != 1
                //                            AND ES.IS_ACTIVE = 1
                //                            AND HALL_TICKET_PUBLICATION_DATE <= CURDATE()
                //                    GROUP BY STI.STUDENT_ID , STI.COURSE_ID ORDER BY STI.DATE_OF_EXAM;";
                //        break;
                //    }
                //case StudentSQLCommads.PaidUnpaidAttendanceList:
                //    {
                //        query = @"SELECT 
                //                            sfd.STUDENT_ID,
                //                             spi.FIRST_NAME,
                //                             spi.ROLL_NO,
                //                             STU_DUE_ID,
                //                             cc.CLASS_NAME,
                //                             cc.CLASS_ID,
                //                             sfd.TERMFEE1,
                //                             TERMFEE1PAID,
                //                             GREATEST(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0),
                //                                     0) + GREATEST(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0),
                //                                     0) AS TERMFEE1DUE,
                //                             GREATEST(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0),
                //                                     0) + GREATEST(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0),
                //                                     0) AS EXAMFEEDUE,
                //                             sfd.TERMFEE2,
                //                             sfd.TERMFEE2PAID,
                //                             sfd.EXAMFEE1,
                //                             EXAMFEE1PAID,
                //                             GREATEST(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0),
                //                                     0) AS EXAMFEE1DUE,
                //                             sfd.EXAMFEE2,
                //                             EXAMFEE2PAID,
                //                             GREATEST(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0),
                //                                     0) AS EXAMFEE2DUE,
                //                             B_CREDIT AS BUSFEE,
                //                             B_DEBIT AS BUSFEEPAID,
                //                             GREATEST(IFNULL(BFSA.B_CREDIT, 0) - IFNULL(BFSA.B_DEBIT, 0),
                //                                     0) AS BUSFEEDUE,
                //                             IF(HFSA.CREDIT IS NULL,
                //                                 HFSA.CREDIT,
                //                                 SUM(IFNULL(HFSA.CREDIT, '0'))) AS HOSTELFEE,
                //                             GREATEST(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0),
                //                                     0) AS HOSTELFEEDUE,
                //                             IFNULL(ARR.ARREAR_DUE, 0) AS PREVIOUS_YEAR_TERM_DUE,
                //                             (IFNULL(ARR.ARREAR_DUE, 0) + GREATEST(IFNULL(sfd.TERMFEE1, 0) - IFNULL(sfd.TERMFEE1PAID, 0),
                //                                     0) + GREATEST(IFNULL(sfd.TERMFEE2, 0) - IFNULL(sfd.TERMFEE2PAID, 0),
                //                                     0) + GREATEST(IFNULL(sfd.EXAMFEE1, 0) - IFNULL(sfd.EXAMFEE1PAID, 0),
                //                                     0) + GREATEST(IFNULL(sfd.EXAMFEE2, 0) - IFNULL(sfd.EXAMFEE2PAID, 0),
                //                                     0) + GREATEST(IFNULL(BFSA.B_CREDIT, 0) - IFNULL(BFSA.B_DEBIT, 0),
                //                                     0) + GREATEST(IFNULL(HFSA.CREDIT, 0) - IFNULL(HFSA.DEBIT, 0),
                //                                     0)) AS TOTAL_DUE_AMOUNT,
                //                             SFD.ENTRY_ID,
                //                             SFD.ENTRY_DATE
                //                         FROM
                //                             STU_PERSONAL_INFO AS SPI
                //                                 INNER JOIN
                //                             STU_FEE_DUE_INFO AS SFD ON SPI.STUDENT_ID = SFD.STUDENT_ID
                //                                 INNER JOIN
                //                             STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                //                                 AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                //                                 INNER JOIN
                //                             CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                //                                 INNER JOIN
                //                             CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                //                                 LEFT JOIN
                //                             (SELECT 
                //                                 STUDENT_ID,
                //                                     SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                //                                     SUM(IFNULL(DEBIT, '0')) AS DEBIT
                //                             FROM
                //                                 HOSTEL_FEE_STUDENT_ACCOUNT AS hf
                //                             WHERE
                //                                 hf.IS_DELETED <> 1
                //                             GROUP BY STUDENT_ID) AS HFSA ON HFSA.STUDENT_ID = SPI.STUDENT_ID
                //                                 LEFT JOIN
                //                             (SELECT 
                //                                 STUDENT_ID,
                //                                     SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                //                                     SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                //                             FROM
                //                                 BUS_FEE_STUDENT_ACCOUNT AS Bf
                //                             WHERE
                //                                 Bf.IS_DELETED <> 1
                //                             GROUP BY STUDENT_ID) AS BFSA ON BFSA.STUDENT_ID = SPI.STUDENT_ID
                //                                 LEFT JOIN
                //                             (SELECT 
                //                                 STUDENT_ID,
                //                                     SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                //                             FROM
                //                                 STU_FEE_DUE_INFO
                //                             WHERE
                //                                 ACADEMIC_YEAR < ?ACADEMIC_YEAR AND IS_DELETED <> 1
                //                             GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SPI.STUDENT_ID
                //                         WHERE
                //                             SFD.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                //                              AND SC.CLASS_ID IN (?CLASS_ID)
                //                                 AND SC.IS_DELETED <> 1
                //                                 AND SFD.IS_DELETED <> 1
                //                                 AND SPI.IS_DELETED <> 1
                //                                 AND SPI.IS_LEFT <> 1

                //                         GROUP BY SFD.STUDENT_ID , sfd.ACADEMIC_YEAR
                //                          -- p having TOTAL_DUE_AMOUNT=0
                //                           -- u having TOTAL_DUE_AMOUNT!=0
                //                         ORDER BY CPG.PROGRAMME_GROUP_ID,SC.CLASS_ID, ROLL_NO;";
                //        break;
                //    }



                case StudentSQLCommads.FetchMentorStudentList:
                    {
                        query = @"SELECT 
                                                MENTORINT_ID,
                                                SPI.STUDENT_ID,
                                                CC.CLASS_ID,
                                                STAFF_ID,
                                                SB.BATCH,
                                                MSL.IS_ACTIVE,
                                                MSL.IS_DELETED,
                                                CREATED_BY,
                                                CREATED_DATE,
                                                MODIFIED_BY,
                                                MODIFIED_DATE
                                                from
                                                stu_class as sc 
                                                inner join
                                                mentoring_student_list as MSL ON SC.STUDENT_ID=MSL.STUDENT_ID
                                                INNER JOIN
                                                CP_CLASSES AS CC ON CC.CLASS_ID=SC.CLASS_ID
                                                INNER JOIN
                                                sup_batches AS SB ON SB.BATCH_ID=MSL.MENTORINT_ID
                                                INNER JOIN
                                                stu_personal_info AS SPI ON SPI.STUDENT_ID=SC.STUDENT_ID
                                                  WHERE
                                        SC.STUDENT_ID =?STUDENT_ID
                                        AND SC.IS_DELETED != 1;";

                        break;
                    }
                case StudentSQLCommads.FetchMentorStudentReportList:
                    {
                        query = @"SELECT 
                                        MENTORINT_ID,
                                        FIRST_NAME,
                                        MOBILE_NO,
                                        sd.DESIGNATION,
                                        cd.DEPARTMENT,
                                        MSL.IS_ACTIVE,
                                        MSL.IS_DELETED,
                                        CONCAT(IFNULL(SMB.BLOCK_NAME, ''),
                                                    ' - ',
                                                    IFNULL(SMR.ROOM_NO, '')) AS ROOM_NO
                                    FROM
                                        MENTORING_STUDENT_LIST AS MSL
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = MSL.STAFF_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SPI.DEPARTMENT
                                            INNER JOIN
                                        SUP_DESIGNATION AS SD ON SD.DESIGNATIONID = SPI.DESIGNATION
                                            LEFT JOIN
                                        MENTOR_ROOM_ALLOTMENT AS SMA ON SMA.STAFF_ID = MSL.STAFF_ID
                                            LEFT JOIN
                                        SUP_MENTOR_ROOM AS SMR ON SMR.ROOM_ID = SMA.ROOM_ID
                                            LEFT JOIN
                                        SUP_MENTOR_BLOCK AS SMB ON SMB.BLOCK_ID = SMR.BLOCK_ID
                                    WHERE
                                        MSL.IS_DELETED != 1  AND SMA.IS_DELETED !=1
                                            AND STUDENT_ID = ?STUDENT_ID";




                        break;
                    }
                case StudentSQLCommads.FetchMentorStudentForStaff:
                    {
                        query = @"  SELECT 
                                            MENTORINT_ID,
                                            SPI.ROLL_NO,
                                            SPI.STUDENT_ID,
                                            SPI.PHOTO,
                                            SPI.FIRST_NAME,
                                            CC.CLASS_NAME,
                                            CD.DEPARTMENT,
                                            SPI.STU_MOBILENO,
                                            SPI.FR_MOBILE,
                                            MSI.IS_ACTIVE,
                                            MSI.IS_DELETED,
                                         CONCAT(IFNULL(SMB.BLOCK_NAME, ''),
                                                    ' - ',
                                                    IFNULL(SMR.ROOM_NO, '')) AS ROOM_NO
                                        FROM
                                            MENTORING_STUDENT_LIST AS MSI
                                                INNER JOIN
                                            STU_CLASS AS SC ON SC.STUDENT_ID = MSI.STUDENT_ID
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = MSI.STUDENT_ID
                                                INNER JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                                INNER JOIN
                                            CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                                LEFT JOIN
                                            MENTOR_ROOM_ALLOTMENT AS SMA ON SMA.STAFF_ID = MSI.STAFF_ID
                                                LEFT JOIN
                                            SUP_MENTOR_ROOM AS SMR ON SMR.ROOM_ID = SMA.ROOM_ID
                                                LEFT JOIN
                                            SUP_MENTOR_BLOCK AS SMB ON SMB.BLOCK_ID = SMR.BLOCK_ID
                                        WHERE
                                            MSI.IS_DELETED != 1 AND SMA.IS_DELETED !=1
                                                AND MSI.STAFF_ID = ?STAFF_ID
                                                AND SC.ACADEMIC_YEAR = ?AC_YEAR;";




                        break;
                    }

                case StudentSQLCommads.FetchStudentInfos:
                    {
                        query = @"SELECT 
                                        SP.FIRST_NAME,
                                        SP.LAST_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        SP.UNIVERSITY_ROLLNO,
                                        DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        SP.ADMISSION_NO,
                                        SP.ADMITTED_CLASS,
                                        DATE_FORMAT(SP.STUDENT_REGISTERDED_DATE, '%d/%m/%Y') AS STUDENT_REGISTERDED_DATE,
                                        SP.LAST_SCHOOL_OR_COLLEGE,
                                        SP.LAST_STUDIED_PLACE,
                                        SP.LAST_STUDIED_CLASS,
                                        EXAM_PASSED,
                                        SP.REMARKS,
                                        SP.ABC_ID,
                                        SP.TC_SERIAL_NO,
                                        SC.CLASS_ID,
                                        PG.PROGRAMME_ID AS 'PROGRAM_ID',
                                        SP.BATCH,
                                        SP.CURRENT_BATCH,
                                        SP.SHIFT_ID,
                                        SP.STU_EMAILID,
                                        SP.IS_LEFT,
                                        SP.DEPT_ID,
                                        SP.PHOTO,
                                        SP.FATHER_NAME,
                                        DATE_FORMAT(F_DATE_OF_BIRTH,'%d/%m/%Y') AS F_DATE_OF_BIRTH,
                                        SP.MOTHER_NAME, 
                                        DATE_FORMAT(M_DATE_OF_BIRTH,'%d/%m/%Y') AS M_DATE_OF_BIRTH, 
                                        SP.FR_BUSINESS_PHONE,
                                        SP.FR_MOBILE,
                                        SP.FR_PASSPORT_number, 
                                        SP.MO_BUSINESS_PHONE,
                                        SP.MO_MOBILE,
                                        SP.MO_PASSPORT_number, 
                                        SP.FR_INCOME,
                                        SP.MO_INCOME,
                                        DATE_FORMAT(FR_DATE_OF_EXPIRY,'%d/%m/%Y') AS FR_DATE_OF_EXPIRY, 
                                        DATE_FORMAT(MO_DATE_OF_EXPIRY,'%d/%m/%Y') AS MO_DATE_OF_EXPIRY,
                                        SP.FR_EMAIL,
                                        SP.MO_EMAIL,
                                        SP.MO_OCCUPATION,
                                        SP.FR_OCCUPATION,
                                        SP.FATHER_EDUCATION, 
                                        SP.MOTHER_EDUCATION,
                                        SP.FR_NATIONALITY,
                                        SP.MO_NATIONALITY
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.Getstudentacademicdetailsforalumnae:
                    {
                        query = @"SELECT 
                                            SP.REGISTER_NO,
                                            SP.PHOTO,
                                            SP.ROLL_NO,
                                            DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                            UPPER(TRIM(SP.FIRST_NAME)) AS FIRST_NAME,
                                            UPPER(TRIM(SP.FATHER_NAME)) AS FATHER_NAME,
                                            UPPER(TRIM(SP.MOTHER_NAME)) AS MOTHER_NAME,
                                            CP.PROGRAMME_DESCRIPTION AS PROGRAMME_NAME,
                                            CONCAT(BEGIN_YEAR, '-', ACTUAL_EXPIRED_YEAR) AS DURATION,
                                            CD.DEPARTMENT,
                                            BEGIN_YEAR,
                                            ACTUAL_EXPIRED_YEAR as EXPIRED_YEAR,
                                            CEEE.APPROVAL,
                                            CEEE.AP_REMARKS, 
                                            CEEE.FB_REMARKS,
                                            CEEE.PROFILE_REMARKS,
                                            SP.ADDRESS_LINE1 AS ADDRESS
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                                INNER JOIN
                                            CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                                INNER JOIN
                                            CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CP.DEPARTMENT
                                                INNER JOIN
                                            CP_CLASSES AS CPC ON CPC.CLASS_ID = SC.CLASS_ID
                                                INNER JOIN
                                            SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID = CPC.CLASS_YEAR
                                                INNER JOIN
                                            ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                        		INNER JOIN
                                        	SUP_BATCH_DURATION AS SB ON SB.BATCH_ID=SC.BATCH
                                            AND SB.APPLICATION_TYPE_ID=CPG.APPTYPE_ID
                                            INNER JOIN convocation_elegibility AS CEEE ON SP.STUDENT_ID = CEEE.STUDENT_ID AND CEEE.IS_DELETED !=1
                                        WHERE
                                            SP.STUDENT_ID =?STUDENT_ID
                                                AND SC.IS_DELETED != 1
                                                AND SC.ACADEMIC_YEAR = (
                                                    SELECT MAX(ACADEMIC_YEAR)
                                                    FROM STU_CLASS
                                                    WHERE STUDENT_ID =?STUDENT_ID
                                                        AND IS_DELETED != 1
                                                )
                                        ORDER BY PROGRAMME_NAME;";
                        break;
                    }
                case StudentSQLCommads.GetMBAstudentacademicdetailsforalumnae:
                    {
                        query = @"SELECT 
                                        SP.REGISTER_NO,
                                        SP.PHOTO,
                                        SP.ROLL_NO,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                        UPPER(TRIM(SP.FIRST_NAME)) AS FIRST_NAME,
                                        UPPER(TRIM(SP.FATHER_NAME)) AS FATHER_NAME,
                                        UPPER(TRIM(SP.MOTHER_NAME)) AS MOTHER_NAME,
                                        CONCAT(BEGIN_YEAR, '-', ACTUAL_EXPIRED_YEAR) AS DURATION,
                                        BEGIN_YEAR,
                                        ACTUAL_EXPIRED_YEAR AS EXPIRED_YEAR,
                                        CEEE.APPROVAL,
                                        CEEE.AP_REMARKS,
                                        CEEE.FB_REMARKS,
                                        CEEE.PROFILE_REMARKS,
                                        SP.ADDRESS_LINE1 AS ADDRESS
                                    FROM
  
                                        STU_PERSONAL_INFO AS SP
   
                                            INNER JOIN
                                        SUP_BATCH_DURATION AS SB ON SB.BATCH_ID = SP.BATCH
                                            INNER JOIN
                                        CONVOCATION_ELEGIBILITY AS CEEE ON SP.STUDENT_ID = CEEE.STUDENT_ID
                                            AND CEEE.IS_DELETED != 1
                                    WHERE
                                        SP.STUDENT_ID = ?STUDENT_ID AND SP.IS_DELETED !=1 GROUP BY CEEE.STUDENT_ID
                                    ORDER BY ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchCIAStudentMarkList:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    SPI.FATHER_NAME,
                                    date_format(SPI.DATE_OF_BIRTH,'%d/%m/%Y') as DATE_OF_BIRTH,
                                    SPI.UMIS_NO,
                                    SR.RELIGION,
                                    SCM.COMMUNITY,
                                    SC.CLASS_ID,
                                    SCY.CLASS_YEAR,
                                    concat(SC.ACADEMIC_YEAR,'-',SC.ACADEMIC_YEAR+1) as ACADEMIC_YEAR,
                                    CC.CLASS_NAME
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    SUP_CLASS_YEAR AS SCY ON SCY.CLASS_YEAR_ID = CC.CLASS_YEAR
                                        INNER JOIN 
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        LEFT JOIN
                                    SUP_RELIGION AS SR ON SR.RELIGIONID = SPI.RELIGION
                                        LEFT JOIN
                                    SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SPI.COMMUNITY
                                WHERE
                                    SC.CLASS_ID IN(?CLASS_ID) AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR ORDER BY CPG.PROGRAMME_ID,ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidatedPersonalInfo:
                    {
                        query = @"SELECT 
                                        SP.FIRST_NAME,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        DATE_FORMAT(SP.ADMISSION_DATE, '%d/%m/%Y') AS ADMISSION_DATE,
                                        SP.ADMISSION_NO,
                                        SP.LAST_SCHOOL_OR_COLLEGE,
                                        SP.LAST_STUDIED_PLACE,
                                        CC.CLASS_NAME,
                                        CP.PROGRAMME_NAME AS 'PROGRAMME_NAME',
                                        SP.STU_EMAILID,
                                        SP.PHOTO,
                                        PG.APPTYPE_ID,
                                        SP.CASTE,
                                        DATE_FORMAT(DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        SB.BATCH,
                                        ANNUAL_INCOME,
                                        AADHAR_NUMBER,
                                        STU_MOBILENO,
                                        FR_MOBILE,
                                        SG.GENDER_NAME AS GENDER,
                                        SCM.COMMUNITY,
                                        SMT.MOTHER_TONGUE_NAME AS MOTHER_TONGUE,
                                        SBG.BLOOD_GROUP_NAME AS BLOOD_GROUP,
                                        SR.RELIGION,
                                        SS.SPECIALLY_ABLED_NAME AS SPECIALLY_ABLED,
                                        SN.NATIONALITY_NAME AS NATIONALITY,
                                        DISTRICT,
                                        TAMIL_NAME,
                                        FATHER_NAME,
                                        MOTHER_NAME,
                                        FR_OCCUPATION,
                                        MO_OCCUPATION,
                                        FR_INCOME,
                                        MO_INCOME,
                                        SAT.STATE_NAME AS STATE_NAME,
                                        CITY,
                                        PINCODE,
                                        ADDRESS_LINE1,
                                        HSS_NO,
                                        SESM.EX_SERVICE_MAN,
                                        HSS_GROUP_CODE,
                                        EMIS_NO,
                                        SEB.EDUCATION_BOARD AS STUDY_BOARD,
                                        HSS_TOTAL,
                                        LAST_SCHOOL_OR_COLLEGE,
                                        MEDIUM_NAME AS MEDIUM_STUDY,
                                        CURUCULAR_NAME AS EXTRA_CURRRICULAR,
                                        SPORTS,
                                        IF(FIRSTGRADUATE=1,'YES','NO') AS FIRSTGRADUATE,
                                        PART_1_LANGUAGE,
                                        LAST_STUDIED_UNIVERSITY,
                                        YEAR_OF_PASSING,
                                        PERCENTAGE,
                                        ALLIED_2,
                                        ALLIED_1,
                                        UMIS_NO,
                                        ENROLLMENT_NO,
                                        MAIN_SUBJECT,
                                        IFSC_CODE,
                                        BANK_NAME,
                                        BRANCH_NAME,
                                        ACCOUNT_NO,
                                        RESIDENCY_TYPE_NAME AS RESIDENCY_TYPE,
                                        IDENTIFICATION_MARK1,
                                        IDENTIFICATION_MARK2,
                                        CC.CLASS_CODE,
                                        CD.DEPARTMENT,
                                        SF.SHIFT_NAME,
                                        SP.STUDENT_ID,
                                        SC.ACADEMIC_YEAR,
                                        SP.IS_LEFT,
                                        SP.IS_REJOIN
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                            LEFT JOIN
                                        SUP_SHIFT AS SF ON SF.SHIFT_ID = CC.SHIFT
                                            LEFT JOIN
                                        SUP_RELIGION AS SR ON SR.RELIGIONID = SP.RELIGION
                                            LEFT JOIN
                                        SUP_BLOOD_GROUP AS SBG ON SBG.BLOOD_GROUP_ID = SP.BLOOD_GROUP
                                            LEFT JOIN
                                        SUP_COMMUNITY AS SCM ON SCM.COMMUNITYID = SP.COMMUNITY
                                            LEFT JOIN
                                        SUP_GENDER AS SG ON SG.GENDER_ID = SP.GENDER
                                            LEFT JOIN
                                        SUP_SPECIALLYABLED AS SS ON SS.SPECIALLY_ABLED_ID = SP.SPECIALLY_ABLED
                                            LEFT JOIN
                                        SUP_NATIONALITY AS SN ON SN.NATIONALITY_ID = SP.NATIONALITY
                                            LEFT JOIN
                                        SUP_MOTHER_TONGUE AS SMT ON SMT.MOTHER_TONGUE_ID = SP.MOTHER_TONGUE
                                            LEFT JOIN
                                        SUP_EDUCATION_BOARD AS SEB ON SEB.EDUCATION_BOARD_ID = SP.STUDY_BOARD
                                            LEFT JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = SC.BATCH
                                            LEFT JOIN
                                        SUP_EX_SERVICE_MAN AS SESM ON SESM.EX_SERVICE_ID = SP.EX_SERVICE_MAN
                                            LEFT JOIN
                                        SUP_EXTRA_CURICULAR_ACTIVITIES AS SEC ON SEC.CURUCULAR_ID = SP.EXTRA_CURRRICULAR
                                            LEFT JOIN
                                        SUP_RESIDENCY_TYPE AS SRT ON SRT.RESIDENCY_TYPE_ID = SP.RESIDENCY_TYPE
                                            LEFT JOIN
                                        SUP_STATE AS SAT ON SAT.STATE_ID = SP.STATE_ID
                                            LEFT JOIN
                                        SUP_MEDIUMSTUDY AS SMS ON SMS.MEDIUM_ID = SP.MEDIUM_STUDY
                                    WHERE
                                        SP.ROLL_NO = ?ROLL_NO
                                            AND SC.ACADEMIC_YEAR = (select Max(ACADEMIC_YEAR) from stu_class where STUDENT_ID = SP.STUDENT_ID and is_deleted != 1)
                                            AND SP.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidatedAcademicMarks:
                    {
                        query = @"SELECT 
                                    SM.INTERNAL_MARK,
                                    SM.EXTERNAL_MARK,
                                    SM.GRADE,
                                    SM.TOTAL,
                                    SM.RESULT,
                                    CR.COURSE_CODE,
                                    CR.COURSE_TITLE,
                                    SEM.SEMESTER_NAME
                                FROM
                                    SEMESTER_MARKS_?AC_YEAR AS SM
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SM.STUDENT_ID = SP.STUDENT_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                        INNER JOIN
                                    SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SM.SEMESTER
                                WHERE
                                    SP.ROLL_NO = ?ROLL_NO
                                    AND SM.IS_DELETED != 1
                                ORDER BY SEM.SEMESTER_NAME, CR.COURSE_CODE;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidatedLateEntryInfo:
                    {
                        query = @"SELECT 
                                    DATE_FORMAT(LE.ENTRY_DATE, '%d/%m/%Y') AS ENTRY_DATE,
                                    DATE_FORMAT(LE.ENTRY_DATE, '%h:%i:%s %p') AS ENTRY_TIME,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    CC.CLASS_NAME,
                                    SPI.STU_MOBILENO,
                                    SPI.FR_MOBILE
                                FROM
                                    STU_LATE_ENTRY AS LE
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = LE.STUDENT_ID
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID 
                                        AND LE.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                WHERE
                                    SPI.ROLL_NO = ?ROLL_NO
                                    AND LE.IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidatedAbsentReport:
                    {
                        query = @"SELECT 
                                    DATE_FORMAT(SAD.ATTENDENCE_DATE, '%d/%m/%Y') AS ATTENDENCE_DATE,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    CC.CLASS_NAME,
                                    IFNULL(SPI.STU_MOBILENO, SPI.FR_MOBILE) AS MOBILE
                                FROM
                                    ATT_COMMON_ABSENTEE_LIST AS SAD
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SAD.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID 
                                        AND SAD.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND ASM.BATCH = SC.BATCH
                                        AND ASM.SEMESTER = ?SEMESTER_ID
                                        INNER JOIN
                                    SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = ASM.SEMESTER
                                WHERE
                                    SPI.ROLL_NO = ?ROLL_NO
                                    AND SAD.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR
                                    AND SAD.SEMESTER_TYPE = SEM.SEMESTER_TYPE
                                    AND SAD.ATTENDANCE_TYPE_ID = 2
                                    AND SAD.IS_DELETED != 1
                                    AND SAD.ATTENDENCE_DATE BETWEEN ASM.DATE_FROM AND ASM.DATE_TO
                                    AND (?ACADEMIC_YEAR = '' OR ?ACADEMIC_YEAR IS NULL OR ASM.ACADEMIC_YEAR = ?ACADEMIC_YEAR)
                                GROUP BY SAD.COMMON_ATTENDANCE_ID, SAD.ATTENDENCE_DATE, SPI.ROLL_NO, SPI.FIRST_NAME, CC.CLASS_NAME, SPI.STU_MOBILENO, SPI.FR_MOBILE
                                ORDER BY SAD.ATTENDENCE_DATE DESC;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidatedFees:
                    {
                        query = @"SELECT 
                                        SC.ACADEMIC_YEAR,
                                        CC.CLASS_NAME,
                                        SUM(IFNULL(TERMFEE1, 0))
                                            - SUM(IFNULL(TERMFEE1PAID, 0))
                                            + SUM(IFNULL(TERMFEE2, 0))
                                            - SUM(IFNULL(TERMFEE2PAID, 0)) AS COLLEGE_FEE,

                                        GREATEST(
                                            IFNULL(HFS.CREDIT, 0)
                                            - IFNULL(HFS.DEBIT, 0),
                                            0
                                        ) AS HOSTELFEEDUE,

                                        GREATEST(
                                            IFNULL(BFS.B_CREDIT, 0)
                                            - IFNULL(BFS.B_DEBIT, 0),
                                            0
                                        ) AS BUSFEEDUE,

                                        SUM(IFNULL(EXAMFEE1, 0))
                                            - SUM(IFNULL(EXAMFEE1PAID, 0))
                                            + SUM(IFNULL(EXAMFEE2, 0))
                                            - SUM(IFNULL(EXAMFEE2PAID, 0)) AS EXAMFEEDUE

                                    FROM STU_CLASS AS SC

                                    INNER JOIN STU_PERSONAL_INFO AS SP
                                        ON SP.STUDENT_ID = SC.STUDENT_ID

                                    INNER JOIN CP_CLASSES AS CC
                                        ON CC.CLASS_ID = SC.CLASS_ID

                                    LEFT JOIN STU_FEE_DUE_INFO AS SFD
                                        ON SFD.STUDENT_ID = SP.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = SFD.ACADEMIC_YEAR
                                        AND SFD.IS_DELETED <> 1
                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS CREDIT,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS DEBIT

                                        FROM FEE_STUDENT_ACCOUNT AS FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY AS SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 4
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR

                                    ) AS HFS
                                        ON HFS.STUDENT_ID = SC.STUDENT_ID
                                        AND HFS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR

                                    LEFT JOIN
                                    (
                                        SELECT 
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR,

                                            SUM(IFNULL(FSA.CREDIT, 0)) AS B_CREDIT,
                                            SUM(IFNULL(FSA.DEBIT, 0)) AS B_DEBIT

                                        FROM FEE_STUDENT_ACCOUNT AS FSA

                                        INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF
                                            ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID

                                        INNER JOIN SUP_FEE_FREQUENCY AS SFF
                                            ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID

                                        INNER JOIN SUP_FREQUENCY_TYPE AS SFT
                                            ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE

                                        WHERE
                                            SFT.FREQUENCY_TYPE_ID = 8
                                            AND FSA.IS_DELETED <> 1

                                        GROUP BY
                                            FSA.STUDENT_ID,
                                            FSA.ACADEMIC_YEAR

                                    ) AS BFS
                                        ON BFS.STUDENT_ID = SC.STUDENT_ID
                                        AND BFS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR

                                    WHERE
                                        SP.IS_DELETED <> 1
                                        AND SC.IS_DELETED <> 1
                                        AND SP.ROLL_NO = ?ROLL_NO

                                    GROUP BY
                                        SC.ACADEMIC_YEAR,
                                        CC.CLASS_NAME;";
                        break;
                    }
                case StudentSQLCommads.FetchConsolidatedSemesterAttendance:
                    {
                        query = @"SELECT 
                                    ASM.ACADEMIC_YEAR,
                                    SEM.SEMESTER_NAME,
                                    ASM.SEMESTER AS SEMESTER_ID,
                                    DATE_FORMAT(ASM.DATE_FROM, '%d/%m/%Y') AS DATE_FROM, 
                                    DATE_FORMAT(ASM.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    (SELECT COUNT(DISTINCT CAL.DAY_ORDER_DATE) 
                                     FROM CALENDER AS CAL 
                                     WHERE CAL.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR 
                                       AND CAL.DAY_ORDER_DATE BETWEEN ASM.DATE_FROM AND LEAST(ASM.DATE_TO, CURDATE()) 
                                       AND CAL.IS_HOLIDAY != 1) AS DAYS_COUNT,
                                    (SELECT COUNT(SAD.COMMON_ATTENDANCE_ID)
                                     FROM ATT_COMMON_ABSENTEE_LIST AS SAD
                                     WHERE SAD.STUDENT_ID = SPI.STUDENT_ID
                                       AND SAD.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR
                                       AND SAD.SEMESTER_TYPE = SEM.SEMESTER_TYPE
                                       AND SAD.ATTENDANCE_TYPE_ID = 2
                                       AND SAD.IS_DELETED != 1
                                       AND SAD.ATTENDENCE_DATE BETWEEN ASM.DATE_FROM AND ASM.DATE_TO) AS ABSENT_COUNT
                                FROM 
                                    ACADEMIC_SEMESTER AS ASM
                                    INNER JOIN STU_CLASS AS SC ON SC.PROGRAMME_GROUP_ID = ASM.PROGRAMME_GROUP_ID 
                                       AND SC.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR 
                                       AND SC.BATCH = ASM.BATCH
                                       AND SC.IS_DELETED != 1
                                    INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                       AND SPI.IS_DELETED != 1
                                    INNER JOIN SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = ASM.SEMESTER
                                WHERE 
                                    SPI.ROLL_NO = ?ROLL_NO
                                GROUP BY ASM.ACADEMIC_YEAR, ASM.SEMESTER, SEM.SEMESTER_NAME, ASM.DATE_FROM, ASM.DATE_TO, SPI.STUDENT_ID, SEM.SEMESTER_TYPE
                                ORDER BY ASM.ACADEMIC_YEAR, ASM.SEMESTER;";
                        break;
                    }
                case StudentSQLCommads.FetchRejoinedHistoryByRollNo:
                    {
                        query = @"SELECT 
                                    SRE.OLD_ROLL_NO,
                                    SRE.ROLL_NO,
                                    SRE.ACADEMIC_YEAR,
                                    SS.SEMESTER_NAME,
                                    DATE_FORMAT(SRE.ENTRY_DATE, '%d/%m/%Y') AS ENTRY_DATE,
                                    SRE.REMARKS
                                FROM
                                    STU_REJOIN AS SRE
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SRE.STUDENT_ID
                                        LEFT JOIN
                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SRE.SEMESTER
                                WHERE
                                    SPI.ROLL_NO = ?ROLL_NO OR SRE.OLD_ROLL_NO = ?ROLL_NO
                                ORDER BY SRE.ENTRY_DATE DESC;";
                        break;
                    }
                case StudentSQLCommads.FetchStudentOptionalCoursesByClassId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SC.CLASS_ID,
                                        SCI.S_CLASS_ID,
                                        SP.FIRST_NAME,
                                        SCI.COURSE_ID,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CR.COURSE_ORDER,
                                        CL.CLASS_LEVEL,
                                        ASM.SEMESTER AS SEMESTER_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                            AND SP.IS_LEFT != 1
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                            AND CL.CLASS_TYPE = 2
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ASM.BATCH = SC.BATCH
                                            AND ASM.IS_ACTIVE = 1
                                            LEFT JOIN
                                        STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                                            AND SCI.CLASS_ID = SC.CLASS_ID
                                            AND SCI.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SCI.IS_DELETED != 1
                                            AND SCI.SEMESTER = ASM.SEMESTER
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SCI.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID = ?CLASS_ID
                                            AND SC.IS_DELETED != 1 ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.DeleteStuOptCourse:
                    {
                        query = @"UPDATE STU_COURSE_INFO
                                 SET IS_DELETED=?IS_DELETED 
                                 WHERE 
                                 COURSE_INFO_ID!=0 
                                 AND STUDENT_ID=?STUDENT_ID
                                 AND COURSE_ID=?COURSE_ID
                                 AND S_CLASS_ID=?S_CLASS_ID
                                 AND ACADEMIC_YEAR=?AC_YEAR 
                                 AND IS_DELETED!=1; ";
                        break;
                    }

                case StudentSQLCommads.FetchNotExistStudentByClassIdAndCourseId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SC.CLASS_ID,
                                        SCI.S_CLASS_ID,
                                        SP.FIRST_NAME,
                                        SCI.COURSE_ID,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CR.COURSE_ORDER,
                                        CL.CLASS_LEVEL,
                                        ASM.SEMESTER AS SEMESTER_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SC.STUDENT_ID = SP.STUDENT_ID
                                            AND SP.IS_LEFT != 1
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                            AND CL.CLASS_TYPE = 2
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS ASM ON ASM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            AND ASM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND ASM.BATCH = SC.BATCH
                                            AND ASM.IS_ACTIVE = 1
                                            LEFT JOIN
                                        STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                                            AND SCI.CLASS_ID = SC.CLASS_ID
                                            AND SCI.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SCI.COURSE_ID = ?COURSE_ID
                                            AND SCI.S_CLASS_ID = ?S_CLASS_ID
                                            AND SCI.IS_DELETED != 1
                                            AND SCI.SEMESTER = ASM.SEMESTER
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SCI.COURSE_ID
                                            AND CR.IS_DELETED != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID = ?CLASS_ID
                                            AND SCI.COURSE_INFO_ID IS NULL
                                            AND SC.IS_DELETED != 1 ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.FetchStaffByOptionalCoursesAndClassId:
                    {
                        query = @"SELECT 
                                    CLASS_ID,COURSE_ID,STAFF_ID
                                FROM
                                    CP_CLASS_COURSE_STAFF
                                WHERE
                                    ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1
                                        AND COURSE_ID = ?COURSE_ID
                                        AND CLASS_ID = ?S_CLASS_ID;";
                        break;
                    }
                case StudentSQLCommads.InsertStuCourseInfo:
                    {
                        query = @"INSERT INTO STU_COURSE_INFO
                                    (COURSE_ID,
                                     CLASS_ID,
                                     STUDENT_ID,
                                     IS_ACTIVE,
                                     S_CLASS_ID,
                                     ACADEMIC_YEAR,
                                     SEMESTER,
                                     STAFF_ID)
                                     VALUES
                                     (?COURSE_ID,
                                     ?CLASS_ID, 
                                     ?STUDENT_ID,
                                     1,
                                     ?S_CLASS_ID,
                                     ?AC_YEAR,
                                     ?SEMESTER, 
                                     ?STAFF_ID);";

                        break;
                    }
                case StudentSQLCommads.FetchStuOptionalCourseFromStuCourseToCheck:
                    {
                        query = @"SELECT 
                                    COURSE_ID,
                                    CLASS_ID,
                                    STUDENT_ID,
                                    S_CLASS_ID,
                                    ACADEMIC_YEAR,
                                    SEMESTER
                                FROM
                                    STU_COURSE_INFO
                                WHERE
                                    COURSE_ID = ?COURSE_ID AND S_CLASS_ID = ?S_CLASS_ID
                                        AND CLASS_ID =?CLASS_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND SEMESTER = ?SEMESTER
                                        AND IS_DELETED != 1
                                        AND STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.CreateProgrammeTransferLogTableIfNotExists:
                    {
                        query = @"CREATE TABLE IF NOT EXISTS STU_PROGRAMME_TRANSFER_LOG (
                                    TRANSFER_LOG_ID INT AUTO_INCREMENT PRIMARY KEY,
                                    STUDENT_ID INT NOT NULL,
                                    CURRENT_ROLL_NO VARCHAR(50),
                                    NEW_ROLL_NO VARCHAR(50),
                                    CURRENT_PROGRAMME_GROUP_ID INT,
                                    CURRENT_CLASS_ID INT,
                                    TRANSFER_PROGRAMME_GROUP_ID INT,
                                    TRANSFER_CLASS_ID INT,
                                    ACADEMIC_YEAR VARCHAR(50),
                                    REMARKS TEXT,
                                    TRANSFERRED_BY VARCHAR(50),
                                    TRANSFER_DATE DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    IS_DELETED TINYINT DEFAULT 0
                                );";
                        break;
                    }
                case StudentSQLCommads.FetchStudentListForProgrammeTransfer:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.REGISTER_NO,
                                    SPI.ADMISSION_NO,
                                    SPI.FIRST_NAME,
                                    CC.CLASS_ID,
                                    CC.CLASS_NAME,
                                    CC.PROGRAMME_GROUP_ID,
                                    CONCAT(IFNULL(P.PROGRAMME_NAME, ''), ' (SHIFT - ', IFNULL(S.SHIFT_NAME, ''), ') ', IFNULL(SPM.PROGRAMME_MODE, '')) AS PROGRAMME_NAME,
                                    S.SHIFT_NAME,
                                    IF(SPI.IS_LEFT = 0, 'NO', 'YES') AS IS_LEFT,
                                    SPI.PHOTO
                                FROM STU_PERSONAL_INFO SPI
                                INNER JOIN STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED != 1
                                INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                LEFT JOIN CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                LEFT JOIN CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID AND P.IS_DELETED != 1
                                LEFT JOIN SUP_SHIFT AS S ON S.SHIFT_ID = CC.SHIFT
                                LEFT JOIN SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                WHERE SPI.IS_DELETED != 1
                                ORDER BY CC.CLASS_ID, SPI.ROLL_NO;";
                        break;
                    }
                case StudentSQLCommads.InsertProgrammeTransferLog:
                    {
                        query = @"INSERT INTO STU_PROGRAMME_TRANSFER_LOG (
                                    STUDENT_ID,
                                    CURRENT_ROLL_NO,
                                    NEW_ROLL_NO,
                                    CURRENT_PROGRAMME_GROUP_ID,
                                    CURRENT_CLASS_ID,
                                    TRANSFER_PROGRAMME_GROUP_ID,
                                    TRANSFER_CLASS_ID,
                                    ACADEMIC_YEAR,
                                    REMARKS,
                                    TRANSFERRED_BY,
                                    TRANSFER_DATE
                                ) VALUES (
                                    ?STUDENT_ID,
                                    ?CURRENT_ROLL_NO,
                                    ?NEW_ROLL_NO,
                                    ?CURRENT_PROGRAMME_GROUP_ID,
                                    ?CURRENT_CLASS_ID,
                                    ?TRANSFER_PROGRAMME_GROUP_ID,
                                    ?TRANSFER_CLASS_ID,
                                    ?ACADEMIC_YEAR,
                                    ?REMARKS,
                                    ?TRANSFERRED_BY,
                                    NOW()
                                );";
                        break;
                    }
                case StudentSQLCommads.FetchProgrammeTransferLogs:
                    {
                        query = @"SELECT 
                                    TL.TRANSFER_LOG_ID,
                                    TL.STUDENT_ID,
                                    IFNULL(SPI.FIRST_NAME, '') AS STUDENT_NAME,
                                    TL.CURRENT_ROLL_NO,
                                    TL.NEW_ROLL_NO,
                                    TL.CURRENT_PROGRAMME_GROUP_ID,
                                    CONCAT(IFNULL(CP_CUR.PROGRAMME_NAME, ''),
                                           IF(SH_CUR.SHIFT_NAME IS NOT NULL, CONCAT(' (SHIFT - ', SH_CUR.SHIFT_NAME, ')'), ''),
                                           IF(SPM_CUR.PROGRAMME_MODE IS NOT NULL, CONCAT(' ', SPM_CUR.PROGRAMME_MODE), '')) AS CURRENT_PROGRAMME_NAME,
                                    TL.CURRENT_CLASS_ID,
                                    IFNULL(CC_CUR.CLASS_NAME, '') AS CURRENT_CLASS_NAME,
                                    TL.TRANSFER_PROGRAMME_GROUP_ID,
                                    CONCAT(IFNULL(CP_TRN.PROGRAMME_NAME, ''),
                                           IF(SH_TRN.SHIFT_NAME IS NOT NULL, CONCAT(' (SHIFT - ', SH_TRN.SHIFT_NAME, ')'), ''),
                                           IF(SPM_TRN.PROGRAMME_MODE IS NOT NULL, CONCAT(' ', SPM_TRN.PROGRAMME_MODE), '')) AS TRANSFER_PROGRAMME_NAME,
                                    TL.TRANSFER_CLASS_ID,
                                    IFNULL(CC_TRN.CLASS_NAME, '') AS TRANSFER_CLASS_NAME,
                                    TL.ACADEMIC_YEAR,
                                    TL.REMARKS,
                                    TL.TRANSFERRED_BY,
                                    DATE_FORMAT(TL.TRANSFER_DATE, '%d/%m/%Y %h:%i %p') AS TRANSFER_DATE
                                FROM STU_PROGRAMME_TRANSFER_LOG TL
                                LEFT JOIN STU_PERSONAL_INFO SPI ON SPI.STUDENT_ID = TL.STUDENT_ID
                                LEFT JOIN CP_CLASSES CC_CUR ON CC_CUR.CLASS_ID = TL.CURRENT_CLASS_ID
                                LEFT JOIN CP_PROGRAMME_GROUP CPG_CUR ON CPG_CUR.PROGRAMME_GROUP_ID = TL.CURRENT_PROGRAMME_GROUP_ID
                                LEFT JOIN CP_PROGRAMME CP_CUR ON CP_CUR.PROGRAMME_ID = CPG_CUR.PROGRAMME_ID
                                LEFT JOIN SUP_SHIFT SH_CUR ON SH_CUR.SHIFT_ID = CPG_CUR.SHIFT
                                LEFT JOIN SUP_PROGRAMME_MODE SPM_CUR ON SPM_CUR.PROGRAMME_MODE_ID = CPG_CUR.PROGRAMME_MODE
                                LEFT JOIN CP_CLASSES CC_TRN ON CC_TRN.CLASS_ID = TL.TRANSFER_CLASS_ID
                                LEFT JOIN CP_PROGRAMME_GROUP CPG_TRN ON CPG_TRN.PROGRAMME_GROUP_ID = TL.TRANSFER_PROGRAMME_GROUP_ID
                                LEFT JOIN CP_PROGRAMME CP_TRN ON CP_TRN.PROGRAMME_ID = CPG_TRN.PROGRAMME_ID
                                LEFT JOIN SUP_SHIFT SH_TRN ON SH_TRN.SHIFT_ID = CPG_TRN.SHIFT
                                LEFT JOIN SUP_PROGRAMME_MODE SPM_TRN ON SPM_TRN.PROGRAMME_MODE_ID = CPG_TRN.PROGRAMME_MODE
                                WHERE TL.IS_DELETED != 1
                                  AND (?AC_YEAR = '' OR ?AC_YEAR = 'ALL' OR TL.ACADEMIC_YEAR = ?AC_YEAR)
                                ORDER BY TL.TRANSFER_LOG_ID DESC;";
                        break;
                    }
                case StudentSQLCommads.CheckStudentRollNoExists:
                    {
                        query = @"SELECT 
                                    SPI.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    CC.CLASS_ID,
                                    CC.CLASS_NAME,
                                    PG.PROGRAMME_GROUP_ID,
                                    CONCAT(IFNULL(P.PROGRAMME_NAME, ''), ' (SHIFT - ', IFNULL(S.SHIFT_NAME, ''), ')') AS PROGRAMME_NAME
                                FROM STU_PERSONAL_INFO SPI
                                LEFT JOIN STU_CLASS AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID AND SC.IS_DELETED != 1
                                LEFT JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID AND CC.IS_DELETED != 1
                                LEFT JOIN CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                LEFT JOIN CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID AND P.IS_DELETED != 1
                                LEFT JOIN SUP_SHIFT AS S ON S.SHIFT_ID = CC.SHIFT
                                WHERE SPI.ROLL_NO = ?ROLL_NO AND SPI.IS_DELETED != 1
                                LIMIT 1;";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentPersonalInfoForProgrammeTransfer:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO
                                SET 
                                    ROLL_NO = ?ROLL_NO,
                                    PROGRAM_ID = ?PROGRAM_ID,
                                    CLASS_ID = ?CLASS_ID,
                                    DEPT_ID = ?DEPT_ID,
                                    SHIFT_ID = ?SHIFT_ID
                                WHERE STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case StudentSQLCommads.UpdateStudentClassForProgrammeTransfer:
                    {
                        query = @"UPDATE STU_CLASS
                                SET 
                                    CLASS_ID = ?CLASS_ID,
                                    PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                WHERE STUDENT_ID = ?STUDENT_ID 
                                  AND ACADEMIC_YEAR = ?AC_YEAR 
                                  AND IS_DELETED != 1;";
                        break;
                    }
                case StudentSQLCommads.UpdateUserAccountRollNo:
                    {
                        query = @"UPDATE USER_ACCOUNT
                                SET USERNAME = ?USERNAME
                                WHERE USER_ID = ?USER_ID AND ROLE = 5 AND IS_DELETED != 1;";
                        break;
                    }

            }
            return query;
        }
    }
}

