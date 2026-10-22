using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.NME
{
    public static class NMESQL
    {
        public static string GetNMESQL(NMESQLCommands sEnumCommand)
        {
            string query = "";


            switch (sEnumCommand)
            {
                case NMESQLCommands.CheckNMEAvailability:
                    {
                        query = @"SELECT 
                                    (NC.QUOTA > (SELECT 
                                            COUNT(*) AS CC
                                        FROM
                                            NME_STUDENT_REGISTRATION AS SR
                                        WHERE
                                            SR.CLASS_ID = ?CLASS_ID AND SR.COURSE_ID = ?COURSE_ID AND SR.ACADEMIC_YEAR=?AC_YEAR
                                                AND SR.SETTINGS_ID = ?SETTINGS_ID)) AS COURSEAVAILABILITY
                                FROM
                                  NME_CLASS_COURSE_QUOTA AS NC
                                WHERE
                                    NC.CLASS_ID = ?CLASS_ID AND NC.COURSE_ID =?COURSE_ID AND NC.IS_DELETED!=1
                                        AND NC.SETTINGS_ID =?SETTINGS_ID;";
                        break;
                    }
                case NMESQLCommands.LoadNMECourses:
                    {
                        //query = @"SELECT 
                        //            NC.CLASS_ID,
                        //            NC.COURSE_ID,
                        //            NC.SETTINGS_ID,
                        //            CR.COURSE_TITLE,
                        //            CR.SEMESTER_ID,
                        //            cls.CLASS_ID as NME_CLASS
                        //        FROM
                        //            nme_class_course_quota AS NC
                        //                INNER JOIN
                        //            cp_course_root AS CR ON CR.COURSE_ROOT_ID = NC.COURSE_ID
                        //                INNER JOIN
                        //            cp_classes AS cls ON cls.COURSE_ID = NC.COURSE_ID
                        //                AND cls.SHIFT = ?SHIFT_ID
                        //        WHERE
                        //            NC.SETTINGS_ID = (SELECT 
                        //                    SETTINGS_ID
                        //                FROM
                        //                    nme_settings ST
                        //                WHERE
                        //                    ST.CLASS_ID = ?CLASS_ID AND ST.ACADEMIC_YEAR=?AC_YEAR AND ST.IS_DELETED!=1)
                        //                AND NC.QUOTA > (SELECT 
                        //                    COUNT(*) AS CC
                        //                FROM
                        //                    nme_student_registration AS SR
                        //                WHERE
                        //                    SR.CLASS_ID = NC.CLASS_ID AND SR.ACADEMIC_YEAR=?AC_YEAR
                        //                        AND SR.COURSE_ID = NC.COURSE_ID);";


                        query = @"SELECT 
                                        NC.CLASS_ID,
                                        NC.COURSE_ID,
                                        NC.SETTINGS_ID,
                                        CR.COURSE_TITLE,
                                        CR.COURSE_CODE,
                                        CR.SEMESTER_ID,
                                        CLS.CLASS_ID AS NME_CLASS
                                    FROM
                                        NME_CLASS_COURSE_QUOTA AS NC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NC.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.COURSE_ID = NC.COURSE_ID
                                            AND CLS.SHIFT = ?SHIFT_ID
                                            INNER JOIN
                                     ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=CR.SEMESTER_ID
                                      AND SEM.ACADEMIC_YEAR=NC.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                    WHERE SEM.IS_ACTIVE=1 AND  NC.ACADEMIC_YEAR=?AC_YEAR AND NC.IS_DELETED!=1 AND
                                        NC.SETTINGS_ID = (SELECT 
                                                SETTINGS_ID
                                            FROM
                                                NME_SETTINGS ST
                                            WHERE
                                                ST.CLASS_ID = ?CLASS_ID
                                                    AND ST.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND ST.IS_DELETED != 1 AND IS_ALLOWED=1)
                                             GROUP BY CR.COURSE_ROOT_ID	;";
                        //query = @"SELECT 
                        //                NC.CLASS_ID,
                        //                NC.COURSE_ID,
                        //                NC.SETTINGS_ID,
                        //                CR.COURSE_TITLE,
                        //                CR.COURSE_CODE,
                        //                CR.SEMESTER_ID,
                        //                CLS.CLASS_ID AS NME_CLASS
                        //            FROM
                        //                NME_CLASS_COURSE_QUOTA AS NC
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NC.COURSE_ID
                        //                    INNER JOIN
                        //                CP_CLASSES AS CLS ON CLS.CLASS_ID = NC.CLASS_ID
                        //                    AND CLS.SHIFT = ?SHIFT_ID
                        //                    INNER JOIN
	                       //             ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=CR.SEMESTER_ID
		                      //              AND SEM.ACADEMIC_YEAR=NC.ACADEMIC_YEAR
                        //                    AND SEM.IS_DELETED!=1
                        //            WHERE SEM.IS_ACTIVE=1 AND  NC.ACADEMIC_YEAR=?AC_YEAR AND NC.IS_DELETED!=1 AND
                        //                NC.SETTINGS_ID = (SELECT 
                        //                        SETTINGS_ID
                        //                    FROM
                        //                        NME_SETTINGS ST
                        //                    WHERE
                        //                        ST.CLASS_ID = ?CLASS_ID
                        //                            AND ST.ACADEMIC_YEAR = ?AC_YEAR
                        //                            AND ST.IS_DELETED != 1 AND IS_ALLOWED=1)
                        //                     GROUP BY CR.COURSE_ROOT_ID	;";
                        break;
                    }
                case NMESQLCommands.RegisterNMECourse:
                    {
                        query = @"INSERT INTO NME_STUDENT_REGISTRATION
                                    (
                                        STUDENT_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                        TIME_REGISTERED,
                                        SETTINGS_ID,
                                        SEMESTER,
                                        S_CLASS_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STUDENT_ID,
                                        ?CLASS_ID,
                                        ?COURSE_ID,
                                        CURDATE(),
                                        ?SETTINGS_ID,
                                        ?SEMESTER,
                                        ?S_CLASS_ID,
                                        ?AC_YEAR
                                   );";
                        break;
                    }
                case NMESQLCommands.ValidateStudent:
                    {
                        //query = @"SELECT 
                        //            COUNT(*) > 0 AS 'Status'
                        //        FROM
                        //            NME_STUDENT_REGISTRATION
                        //        WHERE
                        //            STUDENT_ID = ?STUDENT_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";

                        query = @"SELECT 
                                    -- COUNT(*) > 0 AS 'Status'
        concat( CCR.COURSE_TITLE, ' - ' ,CCR.COURSE_CODE) COURSE_TITLE
                                FROM
                                    NME_STUDENT_REGISTRATION AS NME
										INNER JOIN
									CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = NME.COURSE_ID
                                     INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = NME.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR 
										INNER JOIN
									ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=CCR.SEMESTER_ID
										AND SEM.ACADEMIC_YEAR=NME.ACADEMIC_YEAR
                                        AND SEM.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND SEM.IS_DELETED!=1
                                WHERE
                                    NME.STUDENT_ID = ?STUDENT_ID 
                                    AND SEM.IS_ACTIVE=1 
                                    AND NME.ACADEMIC_YEAR=?AC_YEAR 
                                    AND NME.IS_DELETED!=1;";
                        break;
                    }

                case NMESQLCommands.GetNMEClassListIDs:
                    {

                        query = @"SELECT group_concat(distinct COURSE_ID) FROM nme_class_course_?AC_YEAR";
                        break;
                    }
                case NMESQLCommands.FetchNMEClassList:
                    {

                        query = @"SELECT 
                                        CLASS_ID, CLASS_NAME
                                    FROM
                                        cp_classes_?AC_YEAR CP
                                    WHERE
                                        CP.COURSE_ID IN (?COURSE_ID)";
                        break;
                    }
                case NMESQLCommands.FetchNMEStudentList:
                    {
                        query = @"SELECT 
                                    SP.ADMISSION_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IF(SP.FIRST_NAME IS NULL,
                                                '',
                                                SP.FIRST_NAME),
                                            ' ',
                                            IF(SP.LAST_NAME IS NULL,
                                                '',
                                                SP.LAST_NAME)) AS FIRST_NAME,
                                    CR.COURSE_TITLE,
                                    CLS1.CLASS_NAME 
                                FROM
                                    NME_STUDENT_REGISTRATION_?AC_YEAR AS NSR
                                        INNER JOIN
                                    CP_CLASSES_?AC_YEAR AS CLS ON CLS.CLASS_ID = NSR.CLASS_ID
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = NSR.STUDENT_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT_?AC_YEAR AS CR ON CR.COURSE_ROOT_ID = NSR.COURSE_ID
                                    INNER JOIN cp_classes_?AC_YEAR AS CLS1 ON CLS1.CLASS_ID=NSR.CLASS_ID
                                WHERE
                                    NSR.CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMESettingsList:
                    {
                        query = @"SELECT 
                                        SS.SETTINGS_ID,
                                        SS.SETTINGS_NAME,
                                        CLS.CLASS_CODE,
                                        DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                        SS.ACADEMIC_YEAR,
                                        SEM.SEMESTER_NAME,
                                        SS.IS_ALLOWED
                                    FROM
                                        NME_SETTINGS AS SS
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SS.CLASS_ID
                                            LEFT JOIN
                                        SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SS.SEMESTER WHERE SS.IS_DELETED!=1;";
                        break;
                    }
                case NMESQLCommands.IsNMESettingExisting:
                    {

                        query = @"SELECT 
                                        SS.SETTINGS_ID
                                    FROM
                                        NME_SETTINGS AS SS
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SS.CLASS_ID
                                            LEFT JOIN
                                        SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SS.SEMESTER WHERE SS.SEMESTER=?SEMESTER AND SS.CLASS_ID=?CLASS_ID AND SS.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.SaveNMESettings:
                    {

                        query = @"INSERT INTO NME_SETTINGS
                                                (
                                                SETTINGS_NAME,
                                                CLASS_ID,
                                                DATE_FROM,
                                                DATE_TO,
                                                ACADEMIC_YEAR,
                                                IS_ALLOWED,
                                                SEMESTER)
                                                VALUES
                                                (
                                                ?SETTINGS_NAME,
                                                ?CLASS_ID,
                                                ?DATE_FROM,
                                                ?DATE_TO,
                                                ?AC_YEAR,
                                                ?IS_ALLOWED,
                                                ?SEMESTER);";
                        break;
                    }
                case NMESQLCommands.FetchNMESettingsById:
                    {
                        query = @"SELECT 
                                        NS.SETTINGS_ID,
                                        NS.SETTINGS_NAME,
                                        NS.CLASS_ID,
                                        DATE_FORMAT(NS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(NS.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                        NS.ACADEMIC_YEAR,
                                        NS.SEMESTER,
                                        CL.SHIFT
                                    FROM
                                        NME_SETTINGS AS NS INNER JOIN CP_CLASSES AS CL ON CL.CLASS_ID=NS.CLASS_ID WHERE NS.SETTINGS_ID=?SETTINGS_ID;";


                        break;
                    }
                case NMESQLCommands.UpdateNMESettings:
                    {
                        query = @"UPDATE NME_SETTINGS
                                                    SET
                                                    SETTINGS_NAME=?SETTINGS_NAME,
                                                    CLASS_ID=?CLASS_ID,
                                                    DATE_FROM=?DATE_FROM,
                                                    DATE_TO=?DATE_TO,
                                                    ACADEMIC_YEAR=?ACADEMIC_YEAR,
                                                    SEMESTER=?SEMESTER
                                                    WHERE SETTINGS_ID=?SETTINGS_ID;";


                        break;
                    }
                case NMESQLCommands.DeleteNMESettings:
                    {

                        query = @"UPDATE NME_SETTINGS
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE SETTINGS_ID=?SETTINGS_ID;";
                        break;
                    }
                case NMESQLCommands.NMESettingsRegistrationClose:
                    {

                        query = @"UPDATE NME_SETTINGS
                                                    SET
                                                    IS_ALLOWED=0
                                                    WHERE SETTINGS_ID=?SETTINGS_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMEClassCourseList:
                    {

                        //query = @"SELECT 
                        //                CC.NME_CLASS_COURSE_ID,
                        //                RT.COURSE_CODE,
                        //                RT.COURSE_TITLE,
                        //                ST.SETTINGS_NAME,
                        //                CL.CLASS_CODE,
                        //                CQ.QUOTA
                        //            FROM
                        //                NME_CLASS_COURSE AS CC
                        //                    LEFT JOIN
                        //                CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CC.COURSE_ID
                        //                    LEFT JOIN
                        //                NME_SETTINGS AS ST ON ST.SETTINGS_ID = CC.SETTINGS_ID
                        //                    LEFT JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = CC.CLASS_ID
                        //                    LEFT JOIN
                        //                NME_CLASS_COURSE_QUOTA AS CQ ON CQ.CLASS_ID = CC.CLASS_ID
                        //                    AND CQ.COURSE_ID = CC.COURSE_ID
                        //                    AND CQ.SETTINGS_ID = CC.SETTINGS_ID
                        //            WHERE
                        //                CC.IS_DELETED!=1 AND CC.ACADEMIC_YEAR=?AC_YEAR;";

                        query = @"SELECT 
                                        CC.NME_CLASS_COURSE_ID,
                                        RT.COURSE_CODE,
                                        RT.COURSE_TITLE,
                                        ST.SETTINGS_NAME,
                                        CL.CLASS_CODE,
                                        CQ.QUOTA
                                    FROM
                                        NME_CLASS_COURSE AS CC
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CC.COURSE_ID
                                            LEFT JOIN
                                        NME_SETTINGS AS ST ON ST.SETTINGS_ID = CC.SETTINGS_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = CC.CLASS_ID
                                            LEFT JOIN
                                        NME_CLASS_COURSE_QUOTA AS CQ ON CQ.CLASS_ID = CC.CLASS_ID
                                            AND CQ.COURSE_ID = CC.COURSE_ID
                                            AND CQ.SETTINGS_ID = CC.SETTINGS_ID
                                            AND CQ.IS_DELETED!=1
                                            LEFT JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=RT.SEMESTER_ID
											AND SEM.ACADEMIC_YEAR=CC.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                    WHERE
                                        CC.IS_DELETED!=1 AND SEM.IS_ACTIVE=1 AND CC.ACADEMIC_YEAR=?AC_YEAR  GROUP BY CC.NME_CLASS_COURSE_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMECourseAllotment:
                    {
                        //query = @"SELECT 
                        //                CA.ALLOTMENT_ID,SH.SHIFT_NAME,RT.COURSE_CODE,RT.COURSE_TITLE,CA.ALLOTED_SEATS,CA.ACADEMIC_YEAR
                        //            FROM
                        //               NME_COURSE_ALLOTMENT CA
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CA.COURSE_ID
                        //                    AND RT.IS_DELETED != 1
                        //                    INNER JOIN
                        //                SUP_SHIFT AS SH ON SH.SHIFT_ID = CA.SHIFT_ID
                        //                    INNER JOIN
                        //                NME_COURSE_REGISTRATION AS CR ON CR.COURSE_ID = RT.COURSE_ROOT_ID
                        //                    AND CR.IS_DELETED != 1
                        //            WHERE
                        //                CA.IS_DELETED !=1 AND RT.IS_DELETED!=1 AND CA.ACADEMIC_YEAR=?AC_YEAR;";

                        query = @"SELECT 
                                        CA.ALLOTMENT_ID,
                                        SH.SHIFT_NAME,
                                        RT.COURSE_CODE,
                                        RT.COURSE_TITLE,
                                        CA.ALLOTED_SEATS,
                                        CA.ACADEMIC_YEAR
                                    FROM
                                       NME_COURSE_ALLOTMENT CA
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CA.COURSE_ID
                                            AND RT.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CA.SHIFT_ID
                                            INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=RT.SEMESTER_ID
											AND SEM.ACADEMIC_YEAR=CA.ACADEMIC_YEAR
											AND SEM.IS_DELETED!=1
                                    WHERE
                                        CA.IS_DELETED !=1 AND SEM.IS_ACTIVE=1  AND RT.IS_DELETED!=1 
                                        AND CA.ACADEMIC_YEAR=?AC_YEAR GROUP BY CA.ALLOTMENT_ID;";
                        break;
                    }
                case NMESQLCommands.IsNMECourseAllotmentExisting:
                    {

                        query = @"SELECT 
                                        CA.ALLOTMENT_ID
                                    FROM
                                        NME_COURSE_ALLOTMENT CA
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CA.COURSE_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CA.SHIFT_ID
                                    WHERE
                                        CA.IS_DELETED !=1 AND CA.COURSE_ID=?COURSE_ID AND CA.SHIFT_ID=?SHIFT_ID AND CA.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.SaveNMECourseAllotment:
                    {

                        query = @"INSERT INTO NME_COURSE_ALLOTMENT
                                            (
                                            COURSE_ID,
                                            SHIFT_ID,
                                            ALLOTED_SEATS,
                                            ACADEMIC_YEAR)
                                            VALUES
                                            (
                                            ?COURSE_ID,
                                            ?SHIFT_ID,
                                            ?ALLOTED_SEATS,
                                            ?ACADEMIC_YEAR);";
                        break;
                    }
                case NMESQLCommands.FetchNMECourseAllotmentById:
                    {
                        query = @"SELECT 
                                        ALLOTMENT_ID,
                                        COURSE_ID,
                                        SHIFT_ID,
                                        ALLOTED_SEATS,
                                        ACADEMIC_YEAR
                                    FROM
                                        NME_COURSE_ALLOTMENT
                                    WHERE
                                        ALLOTMENT_ID = ?ALLOTMENT_ID;";
                        break;
                    }
                case NMESQLCommands.UpdateNMECourseAllotment:
                    {
                        query = @"UPDATE NME_COURSE_ALLOTMENT
                                            SET
                                            ALLOTMENT_ID=?ALLOTMENT_ID,
                                            COURSE_ID=?COURSE_ID,
                                            SHIFT_ID=?SHIFT_ID,
                                            ALLOTED_SEATS=?ALLOTED_SEATS,
                                            ACADEMIC_YEAR=?ACADEMIC_YEAR
                                            WHERE ALLOTMENT_ID=?ALLOTMENT_ID;";
                        break;
                    }
                case NMESQLCommands.DeleteNMECourseAllotment:
                    {

                        query = @"UPDATE NME_COURSE_ALLOTMENT
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE ALLOTMENT_ID=?ALLOTMENT_ID;";
                        break;
                    }
                case NMESQLCommands.GetSelectedNMECourseId:
                    {
                        //query = @"SELECT 
                        //                CC.NME_CLASS_COURSE_ID,CL.CLASS_ID,CL.CLASS_CODE,ST.SETTINGS_NAME,CC.COURSE_ID,CONCAT(IFNULL(RT.COURSE_TITLE, ''),
                        //                    '(',
                        //                    IFNULL(RT.COURSE_CODE, ''),
                        //                    ')') AS COURSE_CODE,IF(CC.NME_CLASS_COURSE_ID IS NULL,
                        //                                                    '',
                        //                                                    'SELECTED') AS SELECTED
                        //            FROM
                        //                NME_CLASS_COURSE AS CC
                        //                    LEFT JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = CC.CLASS_ID
                        //                    LEFT JOIN
                        //                NME_SETTINGS AS ST ON ST.SETTINGS_ID = CC.SETTINGS_ID
                        //              LEFT JOIN
                        //             CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID=CC.COURSE_ID
                        //            WHERE
                        //                CC.SETTINGS_ID = ?SETTINGS_ID AND CC.ACADEMIC_YEAR=?AC_YEAR AND CC.IS_DELETED!=1 AND ST.IS_DELETED!=1 AND RT.IS_DELETED!=1;";


                        query = @"SELECT 
                                    CC.NME_CLASS_COURSE_ID,
                                    CL.CLASS_ID,
                                    CL.CLASS_CODE,
                                    ST.SETTINGS_NAME,
                                    CC.COURSE_ID,
                                    CONCAT(IFNULL(RT.COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(RT.COURSE_CODE, ''),
                                            ')') AS COURSE_CODE,
                                    IF(CC.NME_CLASS_COURSE_ID IS NULL,
                                        '',
                                        'SELECTED') AS SELECTED
                                FROM
                                    NME_CLASS_COURSE AS CC
                                        LEFT JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = CC.CLASS_ID
                                        LEFT JOIN
                                    NME_SETTINGS AS ST ON ST.SETTINGS_ID = CC.SETTINGS_ID
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CC.COURSE_ID
		                                INNER JOIN
	                                ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=RT.SEMESTER_ID
		                                AND SEM.ACADEMIC_YEAR=CC.ACADEMIC_YEAR
                                        AND SEM.IS_DELETED!=1
                                WHERE
                                    CC.SETTINGS_ID = ?SETTINGS_ID
	                                AND SEM.IS_ACTIVE=1
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CC.IS_DELETED != 1
                                        AND ST.IS_DELETED != 1
                                        AND RT.IS_DELETED != 1 group by RT.COURSE_ROOT_ID;";
                        break;
                    }
                case NMESQLCommands.GetSelectedNMEClassId:
                    {
                        query = @"SELECT 
                                        CC.NME_CLASS_COURSE_ID,CL.CLASS_ID,CL.CLASS_CODE,ST.SETTINGS_NAME,CC.COURSE_ID,CONCAT(IFNULL(RT.COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(RT.COURSE_CODE, ''),
                                            ')') AS COURSE_CODE,IF(CC.NME_CLASS_COURSE_ID IS NULL,
                                                                            '',
                                                                            'SELECTED') AS SELECTED
                                    FROM
                                        NME_CLASS_COURSE_?AC_YEAR AS CC
                                            LEFT JOIN
                                        CP_CLASSES_?AC_YEAR AS CL ON CL.CLASS_ID = CC.CLASS_ID
                                            LEFT JOIN
                                        NME_SETTINGS AS ST ON ST.SETTINGS_ID = CC.SETTINGS_ID
		                                    LEFT JOIN
	                                    CP_COURSE_ROOT_?AC_YEAR AS RT ON RT.COURSE_ROOT_ID=CC.COURSE_ID
                                    WHERE
                                        CC.SETTINGS_ID = ?SETTINGS_ID GROUP BY CL.CLASS_ID;";
                        break;
                    }
                case NMESQLCommands.IsNMEClassCourseExisting:
                    {
                        query = @"SELECT 
                                        NME_CLASS_COURSE_ID
                                    FROM
                                        NME_CLASS_COURSE
                                    WHERE
                                        COURSE_ID = ?COURSE_ID AND SETTINGS_ID = ?SETTINGS_ID
                                            AND CLASS_ID = ?CLASS_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case NMESQLCommands.SaveNMEClassCourse:
                    {

                        query = @"INSERT INTO NME_CLASS_COURSE
                                            (
                                            COURSE_ID,
                                            SETTINGS_ID,
                                            ACADEMIC_YEAR,
                                            CLASS_ID)
                                            VALUES
                                            (
                                            ?COURSE_ID,
                                            ?SETTINGS_ID,
                                            ?AC_YEAR,
                                            ?CLASS_ID);";
                        break;
                    }
                case NMESQLCommands.UpdateNMEClassCourse:
                    {

                        query = @"UPDATE NME_CLASS_COURSE
                                            SET
                                            COURSE_ID=?COURSE_ID,
                                            SETTINGS_ID=?SETTINGS_ID,
                                            CLASS_ID=?CLASS_ID
                                            WHERE NME_CLASS_COURSE_ID = ?NME_CLASS_COURSE_ID;";
                        break;
                    }
                case NMESQLCommands.DeleteNMEClassCourse:
                    {

                        query = @"UPDATE NME_CLASS_COURSE
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE NME_CLASS_COURSE_ID=?NME_CLASS_COURSE_ID;";
                        break;
                    }
                case NMESQLCommands.DeleteNMEClassCourseQuotaFromClassCourse:
                    {

                        query = @"UPDATE NME_CLASS_COURSE_QUOTA
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE NME_CLASS_COURSE_QUOTA_ID=?NME_CLASS_COURSE_QUOTA_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMEClassCourseById:
                    {

                        query = @"SELECT 
                                        COURSE_ID,
                                        SETTINGS_ID,
                                        CLASS_ID
                                    FROM
                                        NME_CLASS_COURSE
                                    WHERE
                                        NME_CLASS_COURSE_ID = ?NME_CLASS_COURSE_ID AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.FetchClassCourseQuotaByClassIdAndCourseIdAndSettingId:
                    {

                        query = @"SELECT 
                                        NME_CLASS_COURSE_QUOTA_ID
                                    FROM
                                        NME_CLASS_COURSE_QUOTA
                                    WHERE
                                        CLASS_ID = ?CLASS_ID AND COURSE_ID = ?COURSE_ID
                                            AND SETTINGS_ID = ?SETTINGS_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case NMESQLCommands.SaveNMEClassCourseQuota:
                    {

                        query = @"INSERT INTO NME_CLASS_COURSE_QUOTA
                                            (
                                            COURSE_ID,
                                            SETTINGS_ID,
                                            CLASS_ID,
                                            QUOTA,
                                            ACADEMIC_YEAR)
                                            VALUES
                                            (
                                            ?COURSE_ID,
                                            ?SETTINGS_ID,
                                            ?CLASS_ID,
                                            ?QUOTA,
                                            ?AC_YEAR);";
                        break;
                    }
                case NMESQLCommands.BindNMEClassCourseQuota:
                    {
                        //query = @"SELECT 
                        //        COURSE_ROOT_ID,
                        //        COURSE_CODE,
                        //        COURSE_TITLE,
                        //        (SELECT 
                        //                ALLOTED_SEATS
                        //            FROM
                        //                NME_COURSE_ALLOTMENT AS NA
                        //                    INNER JOIN
                        //                SUP_SHIFT AS SS ON SS.SHIFT_ID = NA.SHIFT_ID
                        //                    INNER JOIN
                        //                CP_CLASSES AS CLS ON CLS.SHIFT = SS.SHIFT_ID
                        //            WHERE
                        //                NA.COURSE_ID = CT.COURSE_ROOT_ID
                        //                    AND SS.SHIFT_ID = CLS.SHIFT
                        //                    AND CLS.CLASS_ID = ?CLASS_ID AND NA.ACADEMIC_YEAR=?AC_YEAR AND NA.IS_DELETED!=1) AS TOTALQUOTA,
                        //        (SELECT 
                        //                SUM(QUOTA)
                        //            FROM
                        //                NME_CLASS_COURSE_QUOTA AS CQ
                        //                    INNER JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = CQ.CLASS_ID
                        //            WHERE
                        //                CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //                    AND CL.CLASS_ID = ?CLASS_ID AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1) AS CLASSQUOTA,
                        //        (SELECT 
                        //                SUM(QUOTA)
                        //            FROM
                        //                NME_CLASS_COURSE_QUOTA AS CQ
                        //                    INNER JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = CQ.CLASS_ID
                        //                    INNER JOIN
                        //                SUP_SHIFT AS SS ON SS.SHIFT_ID = CL.SHIFT
                        //            WHERE
                        //                CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //                 -- AND SS.SHIFT_ID = 2 
                        //                    AND CL.CLASS_ID=?CLASS_ID
                        //                    AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1) AS SUMOFQUOTA,
                        //        (SELECT 
                        //                SUM(QUOTA)
                        //            FROM
                        //                NME_CLASS_COURSE_QUOTA AS CQ
                        //                    INNER JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = CQ.CLASS_ID
                        //                    INNER JOIN
                        //                SUP_SHIFT AS SS ON SS.SHIFT_ID = CL.SHIFT
                        //            WHERE
                        //                CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //                    AND SS.SHIFT_ID = CLS.SHIFT AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1)-   (SELECT 
                        //                SUM(QUOTA)
                        //            FROM
                        //                NME_CLASS_COURSE_QUOTA AS CQ
                        //                    INNER JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = CQ.CLASS_ID
                        //            WHERE
                        //                CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //                    AND CL.CLASS_ID = ?CLASS_ID AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1) AS REMAININGQUOTA
                        //    FROM
                        //        CP_COURSE_ROOT AS CT
                        //            LEFT JOIN
                        //        NME_CLASS_COURSE_QUOTA AS NQ ON NQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //            LEFT JOIN
                        //        CP_CLASSES AS CLS ON CLS.CLASS_ID = NQ.CLASS_ID
                        //    WHERE
                        //        CT.COURSE_ROOT_ID IN (?COURSE_ROOT_ID)
                        //    GROUP BY COURSE_ROOT_ID;";
                        //              query = @"SELECT 
                        //                      COURSE_ROOT_ID,
                        //                      COURSE_CODE,
                        //                      COURSE_TITLE,
                        //                      (SELECT 
                        //                              ALLOTED_SEATS
                        //                          FROM
                        //                              NME_COURSE_ALLOTMENT AS NA
                        //                                  INNER JOIN
                        //                              CP_CLASSES AS CLS ON CLS.SHIFT = NA.SHIFT_ID
                        //                          WHERE
                        //                              NA.COURSE_ID = CT.COURSE_ROOT_ID
                        //                                  AND CLS.CLASS_ID = ?CLASS_ID AND NA.ACADEMIC_YEAR=?AC_YEAR AND NA.IS_DELETED!=1) AS TOTALQUOTA,
                        //                      (SELECT 
                        //                              SUM(QUOTA)
                        //                          FROM
                        //                              NME_CLASS_COURSE_QUOTA AS CQ
                        //                          WHERE
                        //                              CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //                                  AND CQ.CLASS_ID = ?CLASS_ID AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1) AS CLASSQUOTA,
                        //                      (SELECT 
                        //                              SUM(QUOTA)
                        //                          FROM
                        //                              NME_CLASS_COURSE_QUOTA AS CQ
                        //                                  INNER JOIN
                        //                              CP_CLASSES AS CL ON CL.CLASS_ID = CQ.CLASS_ID
                        //                          WHERE
                        //                              CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        // AND CL.SHIFT=?SHIFT	AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1) AS SUMOFQUOTA,
                        //                      (SELECT 
                        //                              ALLOTED_SEATS
                        //                          FROM
                        //                              NME_COURSE_ALLOTMENT AS NA
                        //                                  INNER JOIN
                        //                              CP_CLASSES AS CLS ON CLS.SHIFT = NA.SHIFT_ID
                        //                          WHERE
                        //                              NA.COURSE_ID = CT.COURSE_ROOT_ID
                        //                                  AND CLS.CLASS_ID = ?CLASS_ID AND NA.ACADEMIC_YEAR=?AC_YEAR AND NA.IS_DELETED!=1)- (SELECT 
                        //                              SUM(QUOTA)
                        //                          FROM
                        //                              NME_CLASS_COURSE_QUOTA AS CQ
                        //                                  INNER JOIN
                        //                              CP_CLASSES AS CL ON CL.CLASS_ID = CQ.CLASS_ID
                        //                          WHERE
                        //                              CQ.COURSE_ID = CT.COURSE_ROOT_ID
                        // AND CL.SHIFT=1	AND CQ.ACADEMIC_YEAR=?AC_YEAR AND CQ.IS_DELETED!=1) AS REMAININGQUOTA,
                        //                        (SELECT 
                        //                              COUNT(STUDENT_ID)
                        //                          FROM
                        //                              NME_STUDENT_REGISTRATION 
                        //                          WHERE
                        //COURSE_ID =CT.COURSE_ROOT_ID AND CLASS_ID=?CLASS_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1) AS STUDENT_COUNT
                        //                  FROM
                        //                      CP_COURSE_ROOT AS CT
                        //                          LEFT JOIN
                        //                      NME_CLASS_COURSE_QUOTA AS NQ ON NQ.COURSE_ID = CT.COURSE_ROOT_ID
                        //                          LEFT JOIN
                        //                      CP_CLASSES AS CLS ON CLS.CLASS_ID = NQ.CLASS_ID
                        //                  WHERE
                        //                      CT.COURSE_ROOT_ID IN (?COURSE_ROOT_ID)
                        //                  GROUP BY COURSE_ROOT_ID;";

                        query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        COURSE_CODE,
                                        COURSE_TITLE,
                                        NME.ALLOTED_SEATS,
                                        IFNULL(CC.QUOTA,0) AS QUOTA,
                                        (SELECT 
                                                COUNT(STUDENT_ID)
                                            FROM
                                                NME_STUDENT_REGISTRATION 
                                            WHERE
			                                    COURSE_ID =CCR.COURSE_ROOT_ID AND CLASS_ID=CC.CLASS_ID 
                                                AND ACADEMIC_YEAR=NME.ACADEMIC_YEAR 
                                                AND IS_DELETED!=1) AS STUDENT_COUNT
                                    FROM
                                        NME_COURSE_ALLOTMENT AS NME
                                            LEFT JOIN
                                        NME_CLASS_COURSE_QUOTA AS CC ON CC.COURSE_ID = NME.COURSE_ID
                                            AND CC.ACADEMIC_YEAR=NME.ACADEMIC_YEAR  AND CC.CLASS_ID=?CLASS_ID AND CC.IS_DELETED!=1 
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=NME.COURSE_ID AND NME.SHIFT_ID=?SHIFT_ID
		                                    INNER JOIN
	                                    ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=CCR.SEMESTER_ID
		                                    AND SEM.ACADEMIC_YEAR=NME.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                    WHERE NME.ACADEMIC_YEAR=?AC_YEAR AND SEM.IS_ACTIVE=1  AND NME.COURSE_ID IN (?COURSE_ID) AND NME.IS_DELETED!=1
                                          GROUP BY NME.ALLOTMENT_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMECourseRegistration:
                    {
                        query = @"SELECT 
                                        NR.REGISTRATION_ID,
                                        CT.COURSE_TYPE,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        SS.SEMESTER_NAME
                                    FROM
                                        NME_COURSE_REGISTRATION AS NR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NR.COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = NR.COURSE_TYPE
                                            INNER JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CR.SEMESTER_ID 
                                    WHERE CR.IS_DELETED!=1 
                                       AND NR.IS_DELETED!=1 
                                       AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                       AND NR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.IsNMECourseRegistrationExisting:
                    {

                        query = @"SELECT 
                                        REGISTRATION_ID
                                    FROM
                                        NME_COURSE_REGISTRATION
                                    WHERE
                                        COURSE_ID = ?COURSE_ID AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case NMESQLCommands.FetchNMECourseRegistrationExisting:
                    {

                        query = @"SELECT 
                                        REGISTRATION_ID,COURSE_ID
                                    FROM
                                        NME_COURSE_REGISTRATION
                                    WHERE
                                         ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case NMESQLCommands.SaveNMECourseRegistration:
                    {

                        query = @"INSERT INTO NME_COURSE_REGISTRATION
                                                (
                                                COURSE_TYPE,
                                                COURSE_ID,
                                                ACADEMIC_YEAR)
                                                VALUES
                                                (
                                                ?COURSE_TYPE,
                                                ?COURSE_ID,
                                                ?AC_YEAR);";
                        break;
                    }
                case NMESQLCommands.GetNMECourseTypeByCourseId:
                    {

                        query = @"SELECT 
                                        CT.COURSE_TYPE_ID, CT.COURSE_TYPE
                                    FROM
                                        CP_COURSE_ROOT_?AC_YEAR AS CR
                                            INNER JOIN
                                        CP_COURSE_TYPE_?AC_YEAR AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    WHERE
                                        COURSE_ROOT_ID = ?COURSE_ROOT_ID;";
                        break;
                    }
                case NMESQLCommands.DeleteNMECourseRegistration:
                    {

                        query = @"UPDATE NME_COURSE_REGISTRATION
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE REGISTRATION_ID=?REGISTRATION_ID;";
                        break;
                    }
                case NMESQLCommands.DeleteNMECourseRegistrationByCourseId:
                    {

                        query = @"UPDATE NME_COURSE_REGISTRATION
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE COURSE_ID NOT IN (?COURSE_ID) AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.FetchNMECourseRegistrationById:
                    {
                        query = @"SELECT 
                                        NR.REGISTRATION_ID,
                                        CT.COURSE_TYPE_ID,
                                        CT.COURSE_TYPE,
                                        NR.COURSE_ID
                                    FROM
                                        NME_COURSE_REGISTRATION_?AC_YEAR AS NR
                                            INNER JOIN
                                        CP_COURSE_TYPE_?AC_YEAR AS CT ON CT.COURSE_TYPE_ID = NR.COURSE_TYPE
                                    WHERE
                                        REGISTRATION_ID = ?REGISTRATION_ID;";


                        break;
                    }
                case NMESQLCommands.UpdateNMECourseRegistration:
                    {
                        query = @"UPDATE NME_COURSE_REGISTRATION_?AC_YEAR
                                            SET
                                            COURSE_TYPE = ?COURSE_TYPE,
                                            COURSE_ID = ?COURSE_ID
                                            WHERE REGISTRATION_ID =?REGISTRATION_ID;";


                        break;
                    }
                case NMESQLCommands.GetRegisteredCoursesFromCourseAllotment:
                    {
                        //query = @"SELECT 
                        //                CR.COURSE_ROOT_ID,
                        //                CONCAT(IFNULL(COURSE_TITLE, ''),
                        //                        '(',
                        //                        IFNULL(COURSE_CODE, ''),
                        //                        ')') AS COURSE_TITLE
                        //            FROM
                        //                NME_SETTINGS AS NS
                        //                    INNER JOIN
                        //                CP_CLASSES AS CL ON CL.CLASS_ID = NS.CLASS_ID
                        //                    INNER JOIN
                        //                SUP_SHIFT AS SH ON SH.SHIFT_ID = CL.SHIFT
                        //                    INNER JOIN
                        //                NME_COURSE_ALLOTMENT AS NC ON NC.SHIFT_ID = SH.SHIFT_ID
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NC.COURSE_ID
                        //            WHERE
                        //                NS.SETTINGS_ID = ?SETTINGS_ID AND NC.IS_DELETED!=1 AND NS.ACADEMIC_YEAR=?AC_YEAR AND NC.ACADEMIC_YEAR=?AC_YEAR;";

                        query = @"SELECT 
										
                                        CR.COURSE_ROOT_ID,
                                        CONCAT(IFNULL(COURSE_TITLE, ''),
                                                '(',
                                                IFNULL(COURSE_CODE, ''),
                                                ')') AS COURSE_TITLE
                                    FROM
                                        NME_SETTINGS AS NS
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = NS.CLASS_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CL.SHIFT
                                            INNER JOIN
                                        NME_COURSE_ALLOTMENT AS NC ON NC.SHIFT_ID = SH.SHIFT_ID
											AND NC.ACADEMIC_YEAR=NS.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NC.COURSE_ID
											INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=CR.SEMESTER_ID
											AND SEM.ACADEMIC_YEAR=NS.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                    WHERE
                                        NS.SETTINGS_ID = ?SETTINGS_ID 
                                        AND SEM.IS_ACTIVE=1 
                                        AND NC.IS_DELETED!=1 
                                        AND NS.ACADEMIC_YEAR=?AC_YEAR 
                                        GROUP BY COURSE_ROOT_ID;";
                        break;
                    }
                case NMESQLCommands.UpdateNMEClassCourseQuota:
                    {

                        query = @"UPDATE NME_CLASS_COURSE_QUOTA
                                            SET
                                            COURSE_ID=?COURSE_ID,
                                            SETTINGS_ID=?SETTINGS_ID,
                                            CLASS_ID=?CLASS_ID,
                                            QUOTA=?QUOTA
                                            WHERE NME_CLASS_COURSE_QUOTA_ID = ?NME_CLASS_COURSE_QUOTA_ID;";
                        break;
                    }
                case NMESQLCommands.IsNMEClassCourseQuotaExisting:
                    {
                        query = @"SELECT 
                                    NME_CLASS_COURSE_QUOTA_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    QUOTA,
                                    SETTINGS_ID,
                                    ACADEMIC_YEAR,
                                    IS_DELETED
                                FROM
                                    NME_CLASS_COURSE_QUOTA
                                WHERE
                                    COURSE_ID = ?COURSE_ID AND SETTINGS_ID = ?SETTINGS_ID
                                        AND CLASS_ID = ?CLASS_ID
                                        AND ACADEMIC_YEAR=?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case NMESQLCommands.DeleteBulkNMEClassCourse:
                    {
                        query = @"UPDATE NME_CLASS_COURSE
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE COURSE_ID NOT IN (?COURSE_ID) AND CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case NMESQLCommands.DeleteBulkNMEClassCourseQuota:
                    {
                        query = @"UPDATE NME_CLASS_COURSE_QUOTA
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE COURSE_ID NOT IN (?COURSE_ID) AND CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case NMESQLCommands.GetSelectedClassIDBySettingsId:
                    {
                        query = @"SELECT 
                                        CL.CLASS_ID, CL.CLASS_CODE
                                    FROM
                                        NME_SETTINGS AS NS
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = NS.CLASS_ID
                                    WHERE
                                        NS.SETTINGS_ID =?SETTINGS_ID;";
                        break;
                    }
                case NMESQLCommands.GetRegisteredCoursesFromCourseRegistration:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        CONCAT(IFNULL(COURSE_TITLE, ''),
                                                '(',
                                                IFNULL(COURSE_CODE, ''),
                                                ')') AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_ROOT AS CR
                                            INNER JOIN
                                        NME_COURSE_REGISTRATION AS NR ON NR.COURSE_ID = CR.COURSE_ROOT_ID WHERE NR.IS_DELETED!=1 AND CR.IS_DELETED!=1 AND NR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.FetchCourseTypeByCourseId:
                    {
                        query = @"SELECT 
                                        COURSE_TYPE
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        COURSE_ROOT_ID =?COURSE_ROOT_ID ";
                        break;
                    }
                case NMESQLCommands.UpdateNMECourseRegistrationByCourseId:
                    {
                        query = @"UPDATE NME_COURSE_REGISTRATION
                                        SET 
                                            IS_DELETED = 0
                                        WHERE
                                            COURSE_ID = COURSE_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMEClassByCourseId:
                    {
                        query = @"SELECT 
                                        COURSE_ID
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        COURSE_ID = ?COURSE_ID AND IS_DELETED!=1 GROUP BY COURSE_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMERegisteredStudentByClassId:
                    {
                        query = @"SELECT 
                                        NR.REGISTRATION_ID,
                                        NR.STUDENT_ID,
                                        SP.ROLL_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        NS.SETTINGS_NAME,
                                        CL.CLASS_CODE,
                                        CLS.CLASS_CODE AS SELECTED_CLASS,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                '- ',
                                                IFNULL(CR.COURSE_CODE, '')) AS COURSE_CODE,
                                        IF(NR.REGISTRATION_ID IS NULL,
                                            'NOT REGISTERD',
                                            'REGISTERD') AS STATUS
                                    FROM
                                        NME_SETTINGS AS NS
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.CLASS_ID = NS.CLASS_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                            LEFT JOIN
                                        NME_STUDENT_REGISTRATION AS NR ON NR.STUDENT_ID = SC.STUDENT_ID
                                            AND NR.ACADEMIC_YEAR = ?AC_YEAR
                                            AND NR.SEMESTER IN (?SEMESTER_ID)
                                            AND NR.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NR.COURSE_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = NR.S_CLASS_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR AND NS.IS_DELETED !=1
                                            AND NS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND NS.SEMESTER IN (?SEMESTER_ID)
                                    ORDER BY CL.CLASS_ID , SP.ROLL_NO;";
                        break;
                    }
                case NMESQLCommands.FetchNMERegisteredStudentById:
                    {
                        query = @"SELECT 
                                        ST.REGISTRATION_ID,
                                        SI.ROLL_NO,
                                        ST.STUDENT_ID,
                                        SI.FIRST_NAME,
                                        CLS.SHIFT AS SHIFT_ID,
                                        ST.COURSE_ID,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                '(',
                                                IFNULL(CR.COURSE_CODE, ''),
                                                ')') AS COURSE_CODE,
                                        (SELECT 
                                                SUM(QUOTA)
                                            FROM
                                                NME_CLASS_COURSE_QUOTA AS NQ
                                            WHERE
                                                NQ.COURSE_ID = CR.COURSE_ROOT_ID
                                                    AND NQ.CLASS_ID = CLS.CLASS_ID AND NQ.ACADEMIC_YEAR=?AC_YEAR AND NQ.IS_DELETED!=1) AS SUMOFQUTOA,
                                        CLS.CLASS_ID,
                                        CLS.CLASS_CODE
                                    FROM
                                        NME_STUDENT_REGISTRATION AS ST
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = ST.COURSE_ID AND ST.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ST.CLASS_ID
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID=ST.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SI ON SI.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        ST.REGISTRATION_ID = ?REGISTRATION_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMECourseByClassId:
                    {
                        query = @"SELECT 
                                        NQ.COURSE_ID,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                '(',
                                                IFNULL(CR.COURSE_CODE, ''),
                                                ')') AS COURSE_CODE
                                    FROM
                                        NME_CLASS_COURSE_QUOTA AS NQ
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NQ.COURSE_ID
                                    WHERE
                                        NQ.CLASS_ID = ?CLASS_ID AND NQ.ACADEMIC_YEAR=?AC_YEAR AND NQ.IS_DELETED!=1;";
                        break;
                    }
                case NMESQLCommands.FetchSumOfNMEClassQuotaByClassIdAndCourseId:
                    {
                        //query = @"SELECT 
                        //        IFNULL(NQ.QUOTA, '0') AS ALLOTED_SEATS,
                        //        COUNT(NR.COURSE_ID) AS SUMOFQUTOA
                        //    FROM
                        //        NME_CLASS_COURSE_QUOTA AS NQ
                        //            LEFT JOIN
                        //        NME_STUDENT_REGISTRATION AS NR ON NR.COURSE_ID = NQ.COURSE_ID
                        //            AND NQ.ACADEMIC_YEAR = ?AC_YEAR
                        //            AND NR.ACADEMIC_YEAR = ?AC_YEAR
                        //            AND NR.CLASS_ID = NQ.CLASS_ID
                        //            AND NR.IS_DELETED != 1        
                        //    WHERE
                        //        NQ.COURSE_ID = ?COURSE_ID
                        //            AND NQ.CLASS_ID = ?CLASS_ID
                        //            AND NQ.ACADEMIC_YEAR = ?AC_YEAR
                        //            AND NQ.IS_DELETED != 1;";
                        query = @"SELECT 
                                    IFNULL(NCA.ALLOTED_SEATS, '0') AS ALLOTED_SEATS,
                                    COUNT(NR.COURSE_ID) AS SUMOFQUTOA
                                FROM
                                    NME_COURSE_ALLOTMENT AS NCA
                                         LEFT JOIN
                                    CP_CLASSES AS CC ON CC.SHIFT = NCA.SHIFT_ID
                                        LEFT JOIN
                                    NME_STUDENT_REGISTRATION AS NR ON NR.COURSE_ID = NCA.COURSE_ID
                                        AND NR.CLASS_ID = CC.CLASS_ID
                                        AND NCA.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NR.IS_DELETED != 1
                                WHERE
                                    NCA.COURSE_ID = ?COURSE_ID
                                        AND NCA.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SHIFT_ID = ?SHIFT_ID
                                        AND NCA.IS_DELETED != 1;";
                        break;
                    }
                case NMESQLCommands.UpdateNMERegisteredStudent:
                    {
                        query = @"UPDATE NME_STUDENT_REGISTRATION
                                            SET
                                            STUDENT_ID= ?STUDENT_ID,
                                            CLASS_ID= ?CLASS_ID,
                                            COURSE_ID= ?COURSE_ID,
                                            S_CLASS_ID= ?S_CLASS_ID
                                            WHERE REGISTRATION_ID= ?REGISTRATION_ID;";
                        break;
                    }
                case NMESQLCommands.FetchSelectedClassIdByStudentIdAndCourseId:
                    {
                        query = @"SELECT 
                                        CLS.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS ST
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.SHIFT = ST.SHIFT_ID
                                    WHERE
                                        ST.STUDENT_ID = ?STUDENT_ID AND CLS.COURSE_ID = ?COURSE_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMERegisteredStudentToPush:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                        SEMESTER,
                                        S_CLASS_ID
                                    FROM
                                        NME_STUDENT_REGISTRATION
                                    WHERE
                                        IS_DELETED != 1 AND SEMESTER IN (?SEMESTER) AND ACADEMIC_YEAR=?AC_YEAR ;";
                        break;
                    }
                case NMESQLCommands.DeleteNMECourseRegisteredStudent:
                    {

                        query = @"UPDATE NME_STUDENT_REGISTRATION
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE REGISTRATION_ID=?REGISTRATION_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMESettingsAcademicYear:
                    {
                        query = @"SELECT 
                                        ACADEMIC_YEAR
                                    FROM
                                        NME_SETTINGS
                                    WHERE
                                        IS_ALLOWED = 1
                                    GROUP BY ACADEMIC_YEAR;";
                        break;
                    }
                case NMESQLCommands.PushNMERegisteredStudentToStuCourseInfo:
                    {
                        query = @"INSERT INTO STU_COURSE_INFO
                                            (
                                            COURSE_ID,
                                            CLASS_ID,
                                            STUDENT_ID,
                                            S_CLASS_ID,
                                            ACADEMIC_YEAR,
                                            SEMESTER)
                                            VALUES
                                            (
                                            ?COURSE_ID,
                                            ?CLASS_ID,
                                            ?STUDENT_ID,
                                            ?S_CLASS_ID,
                                            ?AC_YEAR,
                                            ?SEMESTER);";
                        break;
                    }
                case NMESQLCommands.FetchStuCourseInfo:
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
                                    WHERE IS_DELETED!=1 
                                       AND ACADEMIC_YEAR=?AC_YEAR 
                                       AND STUDENT_ID=?STUDENT_ID 
                                       AND CLASS_ID=?CLASS_ID
                                       AND COURSE_ID=?COURSE_ID
                                       AND S_CLASS_ID=?S_CLASS_ID;";
                        break;
                    }
                case NMESQLCommands.SaveNMEStudentRegistration:
                    {
                        query = @"INSERT INTO NME_STUDENT_REGISTRATION
                                   (
                                       STUDENT_ID,
                                       CLASS_ID,
                                       COURSE_ID,
                                       TIME_REGISTERED,
                                       SETTINGS_ID,
                                       SEMESTER,
                                       ACADEMIC_YEAR,
                                       S_CLASS_ID
                                    )
                                   VALUES
                                    (
                                        ?STUDENT_ID,
                                        ?CLASS_ID,
                                        ?COURSE_ID,
                                        CURDATE(),
                                        ?SETTINGS_ID,
                                        ?SEMESTER,
                                        ?AC_YEAR,
                                        ?S_CLASS_ID
                                    );";
                        break;
                    }
                case NMESQLCommands.IsNMEStudentRegistrationExist:
                    {
                        query = @"SELECT 
                                        REGISTRATION_ID
                                    FROM
                                        NME_STUDENT_REGISTRATION
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID AND SEMESTER IN (?SEMESTER)
                                            AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case NMESQLCommands.LoadNMECourseList:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CONCAT(IFNULL(CCR.COURSE_CODE, ''),
                                                ' - ',
                                                IFNULL(CCR.COURSE_TITLE, '')) AS COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        C.CLASS_ID,
                                        CC.CLASS_ID AS NME_CLASS_ID,
                                        NQ.SETTINGS_ID,
                                        IF(NQ.QUOTA = IFNULL(COUNT(NSR.STUDENT_ID), '0'),
                                            'DISABLED',
                                            '') AS QUOTA
                                    FROM
                                        NME_CLASS_COURSE_QUOTA AS NQ
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = NQ.CLASS_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.COURSE_ID = NQ.COURSE_ID
                                            AND CC.SHIFT = C.SHIFT
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = NQ.COURSE_ID
                                            LEFT JOIN
                                        NME_STUDENT_REGISTRATION AS NSR ON NSR.COURSE_ID = NQ.COURSE_ID
                                            AND NSR.CLASS_ID = NQ.CLASS_ID
                                            AND NSR.SETTINGS_ID=NQ.SETTINGS_ID
                                            AND NSR.SEMESTER=CCR.SEMESTER_ID
                                            AND NSR.IS_DELETED != 1
                                            AND NSR.ACADEMIC_YEAR = NQ.ACADEMIC_YEAR
                                    WHERE
                                        NQ.ACADEMIC_YEAR = ?AC_YEAR
		                                    AND CCR.SEMESTER_ID IN (?SEMESTER_ID)
                                            AND NQ.IS_DELETED != 1
                                            AND NQ.CLASS_ID = ?CLASS_ID GROUP BY NQ.NME_CLASS_COURSE_QUOTA_ID;";
                        break;
                    }
                case NMESQLCommands.FetchNMENotRegisteredStudentForDDL:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),' - ',SP.ROLL_NO) AS FIRST_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
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
                                            AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED != 1
                                            AND SEM.BATCH = AB.BATCH_ID
                                            LEFT JOIN
                                        nme_student_registration AS NSR ON NSR.STUDENT_ID = SC.STUDENT_ID
                                            AND NSR.IS_DELETED != 1
                                            AND NSR.CLASS_ID = SC.CLASS_ID
                                            AND NSR.SEMESTER = SEM.SEMESTER
                                            AND NSR.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.CLASS_ID = ?CLASS_ID
                                            AND SEM.IS_ACTIVE = 1
                                            AND NSR.REGISTRATION_ID IS NULL
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case NMESQLCommands.FetchNMERegisteredListBySemestertype:
                    {
                        query = @" SELECT 
                                    NR.REGISTRATION_ID,
                                    NR.STUDENT_ID,
                                    SP.ROLL_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    NS.SETTINGS_NAME,
                                    CL.CLASS_CODE,
                                    CLS.CLASS_CODE AS SELECTED_CLASS,
                                    CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                            '- ',
                                            IFNULL(CR.COURSE_CODE, '')) AS COURSE_CODE,
                                    IF(NR.REGISTRATION_ID IS NULL,
                                        'NOT REGISTERD',
                                        'REGISTERD') AS STATUS
                                FROM
                                    NME_SETTINGS AS NS
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.CLASS_ID = NS.CLASS_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN
                                    CP_CLASSES AS CL ON Cl.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN
                                    NME_STUDENT_REGISTRATION AS NR ON NR.STUDENT_ID = SC.STUDENT_ID
                                        AND NR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NR.SEMESTER IN (?SEMESTER_ID)
                                        AND NR.IS_DELETED != 1
                                        LEFT JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = NR.COURSE_ID
                                        LEFT JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = NR.S_CLASS_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND NS.SEMESTER IN (?SEMESTER_ID)
                                        -- R AND NR.REGISTRATION_ID IS NOT NULL
                                       -- N AND NR.REGISTRATION_ID IS NULL
                                ORDER BY CL.CLASS_ID , SP.ROLL_NO;";
                        break;
                    }
            }
            return query;
        }
    }
}
