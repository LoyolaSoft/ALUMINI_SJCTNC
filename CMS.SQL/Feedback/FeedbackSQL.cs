using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Feedback
{
    public static class FeedbackSQL
    {
        public static string GetFeedbackSQL(FeedbackSQLCommands sEnumCommand)
        {

            string query = "";


            switch (sEnumCommand)
            {


                case FeedbackSQLCommands.FetchFeedbackYearBySettingId:
                    {
                        query = @"SELECT ACADEMIC_YEAR FROM FBFEEDBACK_SETTINGS WHERE  FEEDBACK_ID=?FEEDBACK_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFBObjectives:
                    {
                        query = @"SELECT 
                                        OBJECTIVE_ID, OBJECTIVES, RATING, OBJECTIVESHORTTERM
                                    FROM
                                        FBOBJECTIVES
                                    WHERE
                                        QUESTION_TYPE = ?QUESTION_TYPE
                                    ORDER BY OBJECTIVE_ORDER;";
                        break;
                    }

                case FeedbackSQLCommands.FectchFBQuestions:
                    {
                        query = @"SELECT 
                                    QUESTION, QUESTION_ID, GROUP_TITLE AS QUESTION_TYPE,
                                     FG.QUESTION_GROUP_ID AS QUESTION_GROUP,FQ.FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_QUESTIONS AS FQ
                                        INNER JOIN
                                    FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                       -- AND FG.QUESTION_GROUP_ID IN (?QUESTION_GROUP_ID) 
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID=FQ.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1 AND FS.ACADEMIC_YEAR =FQ.ACADEMIC_YEAR
                                WHERE
                                    FQ.ACADEMIC_YEAR=?AC_YEAR;";

                        break;
                    }

                case FeedbackSQLCommands.FectchFBQuestionsByFeedbackId:
                    {
                        query = @"SELECT 
                                    QUESTION, QUESTION_ID, GROUP_TITLE AS QUESTION_TYPE,
                                     FG.QUESTION_GROUP_ID AS QUESTION_GROUP,FQ.FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_QUESTIONS AS FQ
                                        INNER JOIN
                                    FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                       -- AND FG.QUESTION_GROUP_ID IN (?QUESTION_GROUP_ID) 
                                WHERE
                                    FQ.ACADEMIC_YEAR=?AC_YEAR AND FQ.FEEDBACK_ID=?FEEDBACK_ID;";

                        break;
                    }

                case FeedbackSQLCommands.FetchFBClasswiseStaff:
                    {
                        //query = @"SELECT 
                        //                FB_ID,
                        //                FS.CLASS_ID,
                        //                ST.DEPARTMENT AS 'DEPARTMENT_ID',
                        //                FS.STAFF_ID,
                        //                CONCAT(IF(ST.FIRST_NAME IS NULL,
                        //                            '',
                        //                            ST.FIRST_NAME),
                        //                        ' ',
                        //                        IF(ST.LAST_NAME IS NULL,
                        //                            '',
                        //                            ST.LAST_NAME)) AS STAFF_NAME
                        //            FROM
                        //                FBCLASS_WISE_STAFF?AC_YEAR AS FS
                        //                    INNER JOIN
                        //                stf_personal_info AS ST ON FS.STAFF_ID = ST.STAFF_ID
                        //                    INNER JOIN
                        //                FBFEEDBACK_SETTINGS AS FSTG ON FSTG.FEEDBACK_ID = FS.FEEDBACK_ID
                        //            WHERE
                        //                FSTG.IS_ACTIVE = 1 AND FS.CLASS_ID=?CLASS_ID;";
                        query = @"SELECT 
                                        FS.CLASS_ID,
                                        ST.DEPARTMENT AS 'DEPARTMENT_ID',
                                        FS.STAFF_ID,
                                        CONCAT(IF(ST.FIRST_NAME IS NULL,
                                                    '',
                                                    ST.FIRST_NAME),
                                                ' ',
                                                IF(ST.LAST_NAME IS NULL,
                                                    '',
                                                    ST.LAST_NAME)) AS STAFF_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS FS
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS ST ON FS.STAFF_ID = ST.STAFF_ID
                                            AND ST.IS_DELETED != 1
                                            AND ST.IS_LEFT != 1
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = FS.SEMESTER_TYPE_ID
                                            AND SST.IS_ACTIVE = 1
                                    WHERE
                                        FS.CLASS_ID = ?CLASS_ID
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.IS_DELETED != 1
                                    GROUP BY STAFF_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFBClassesByStaffId:
                    {

                        query = @"SELECT 
                                        CLS.CLASS_ID,
                                        CLS.CLASS_DESCRIPTION,
                                        CLS.PROGRAMME_GROUP_ID AS PROGRAMME,
                                        FCS.COURSE_ID,
                                        RT.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF  AS FCS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = FCS.CLASS_ID
                                            INNER JOIN
										SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=FCS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = FCS.COURSE_ID
											AND RT.IS_DELETED!=1
                                    WHERE
                                        STAFF_ID = ?STAFF_ID
                                    GROUP BY CLASS_ID;";

                        break;
                    }
                case FeedbackSQLCommands.FetchStudentInfo:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.FIRST_NAME,
                                        CLS.CLASS_CODE,
                                        PRO.PROGRAMME_CODE,
                                        DEP.DEPARTMENT,
                                        BT.BATCH,
                                        CLS.CLASS_ID,
                                        SP.PHOTO
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SCLS ON SCLS.STUDENT_ID = SP.STUDENT_ID
                                            AND SCLS.IS_DELETED != 1
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            AND SCLS.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SCLS.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS P ON P.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS PRO ON PRO.PROGRAMME_ID = P.PROGRAMME_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = PRO.DEPARTMENT
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SEM.BATCH = SCLS.BATCH
                                            AND SEM.IS_ACTIVE = 1
                                            INNER JOIN
                                        SUP_BATCHES AS BT ON BT.BATCH_ID = SCLS.BATCH
                                    WHERE
                                        SP.STUDENT_ID = ?STUDENT_ID
                                    GROUP BY SEM.BATCH;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentListFeedBack:
                    {
                        // Query For Fetch All The Courses From Course Pattern..
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    C.CLASS_NAME,
                                    CP.COURSE_ID,
                                    CCR.COURSE_CODE,
                                    UPPER(CCR.COURSE_TITLE) AS 'COURSE_TITLE',
                                    SC.CLASS_ID
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
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.CLASS_ID = C.CLASS_ID
                                        AND SC.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                        AND SC.IS_DELETED != 1
                                        INNER JOIN
									STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID AND SPI.IS_DELETED !=1 AND spi.IS_LEFT !=1
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                            ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR
                                        WHERE
                                            AC_YEAR = ?AC_YEAR AND IS_DELETED != 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS S ON S.SEMESTER = CCR.SEMESTER_ID
                                        AND S.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                        AND S.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND S.BATCH = AB.BATCH_ID
                                        AND S.IS_DELETED != 1
                                        LEFT JOIN
                                    SUP_SUBJECT_TYPE AS SS ON SS.SUBJECT_TYPE_ID = CCR.SUBJECT_TYPE
                                        AND SS.IS_DELETED != 1
                                WHERE
                                    CP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND S.IS_ACTIVE = 1
                                        AND SC.STUDENT_ID = ?STUDENT_ID
                                        AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CS.IS_DELETED != 1
                                GROUP BY CCR.COURSE_CODE ORDER BY CCR.PART,CCR.COURSE_ORDER;";
                        break;
                    }

                case FeedbackSQLCommands.FetchOverallRatingForStaff:
                    {

                        query = @"SELECT 
                                        COUNT(FE.ANSWER) AS OBJECTIVES_COUNT,
                                        COUNT(FE.ASSESSOR)AS STUDENT_COUNT,
                                        STP.FIRST_NAME,
                                        CLS.CLASS_CODE,
                                        FO.OBJECTIVES,
                                        FE.QUESTION
                                    FROM
                                        fbevaluation?AC_YEAR AS FE
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = FE.CLASS_ID
                                            INNER JOIN
                                        staff_personal_info AS STP ON FE.ASSESSEE = STP.STAFF_NO
                                            INNER JOIN
                                        fbobjectives AS FO ON FO.OBJECTIVE_ID = FE.ANSWER
                                    GROUP BY FE.ASSESSEE, FE.ANSWER
                                    ORDER BY STP.FIRST_NAME, FO.OBJECTIVE_ORDER";
                        break;
                    }

                case FeedbackSQLCommands.FetchOverallRatingByStaffId:
                    {
                        query = @"";
                        break;
                    }

                case FeedbackSQLCommands.FetchQuestionWiseRatingByStaffId:
                    {
                        query = @"SELECT 
                                    COUNT(FE.ASSESSOR) AS ASSESSOR,
                                    FQ.QUESTION,
                                    FG.GROUP_TITLE,	                                   
                                    COUNT(IF(FE.ANSWER = 1, 1, NULL)) AS VERY_GOOD,
									COUNT(IF(FE.ANSWER = 2, 1, NULL)) AS GOOD,
									COUNT(IF(FE.ANSWER = 3, 1, NULL)) AS SATISFACTORY,
									COUNT(IF(FE.ANSWER = 4, 1, NULL)) AS POOR,
									COUNT(IF(FE.ANSWER = 5, 1, NULL)) AS VERY_POOR
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                        INNER JOIN
                                    FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FE.QUESTION
                                        INNER JOIN
                                    FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                WHERE
                                    FE.COURSE_ID =?COURSE_ID
                                        AND ASSESSEE =?ASSESSEE
                                            AND CLASS_ID=?CLASS_ID       
                                    AND FE.FEEDBACK_ID=(SELECT FEEDBACK_ID FROM FBFEEDBACK_SETTINGS AS FS WHERE FS.IS_ACTIVE=1 AND FS.ACADEMIC_YEAR=?AC_YEAR AND FS.IS_DELETED!=1) 
                                GROUP BY FE.QUESTION ORDER BY FG.QUESTION_GROUP_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchQuestionWiseRatingByStaffIdusingRate:
                    {
                        query = @" SELECT 
                                        COUNT(FE.ASSESSOR) AS ASSESSOR,
                                        FQ.QUESTION,
                                        OBJECTIVES,
                                        GROUP_TITLE,
                                        SUM(RATING) as RATING,
                                        OBJECTIVE_ID,
                                        FQ.QUESTION_ID
                                    FROM
                                        FBEVALUATION?AC_YEAR AS FE
                                            INNER JOIN
                                        FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FE.QUESTION
                                            INNER JOIN
                                        FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                            INNER JOIN
                                        fbobjectives AS fo ON fo.OBJECTIVE_ID = FE.ANSWER
                                    WHERE
                                        FE.COURSE_ID = ?COURSE_ID
                                            AND ASSESSEE =?ASSESSEE
                                            AND CLASS_ID =?CLASS_ID  
                                            AND FE.FEEDBACK_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS AS FS
                                            WHERE
                                                FS.IS_ACTIVE = 1
                                                    AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND FS.IS_DELETED != 1)
                                    GROUP BY FE.QUESTION,OBJECTIVE_ID
                                    ORDER BY FG.QUESTION_GROUP_ID;";
                        break;
                    }

                case FeedbackSQLCommands.GetAssessorCountByClassID:
                    {
                        break;
                    }
                case FeedbackSQLCommands.FetchQuestionwiseReportByDepartmentsIds:
                    {
                        query = @"SELECT 
                                    CONCAT(IF(STF.FIRST_NAME IS NULL,
                                                '',
                                                STF.FIRST_NAME),
                                            ' ',
                                            IF(STF.LAST_NAME IS NULL,
                                                '',
                                                STF.LAST_NAME)) AS 'STAFF_NAME',
                                    STF.STAFF_CODE,
                                    FE.COURSE_ID,
                                    CR.COURSE_TITLE,
                                    CT.COURSE_TYPE,
                                    CR.COURSE_CODE,
                                    FQ.QUESTION,
                                    FG.GROUP_TITLE,
                                    COUNT(IF(FE.ANSWER = 1, 1, NULL)) AS VERY_GOOD,
                                    COUNT(IF(FE.ANSWER = 2, 1, NULL)) AS GOOD,
                                    COUNT(IF(FE.ANSWER = 3, 1, NULL)) AS SATISFACTORY,
                                    COUNT(IF(FE.ANSWER = 4, 1, NULL)) AS POOR,
                                    COUNT(IF(FE.ANSWER = 5, 1, NULL)) AS VERY_POOR,
                                    FE.ASSESSEE,
                                    DP.DEPARTMENT,
                                    CLS.CLASS_NAME,
                                    COUNT(FE.ASSESSOR) AS ASSESSOR,
                                   
                                                        (SELECT 
                                        COUNT(*)
                                    FROM
                                        CP_CLASS_COURSE AS SCLS
                                    WHERE
                                        SCLS.COURSE_ID = CR.COURSE_ROOT_ID 
                                        AND SCLS.ACADEMIC_YEAR=?AC_YEAR 
                                        AND SCLS.IS_DELETED!=1) AS TOTAL_STUDENTS
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                        INNER JOIN
                                    FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FE.QUESTION
                                        INNER JOIN
                                    FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = FE.COURSE_ID
                                        INNER JOIN
                                    CP_COURSE_TYPE AS CT ON CR.COURSE_TYPE = CT.COURSE_TYPE_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = CR.DEPARTMENT
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON FE.CLASS_ID = CLS.CLASS_ID
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.ASSESSEE
										INNER JOIN
									ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=CLS.PROGRAMME_GROUP_ID
                                WHERE
                                     STF.DEPARTMENT IN (?DEPARTMENT) 
                                     AND CLS.SHIFT IN (?SHIFT)
                                     AND CR.SEMESTER_ID=SEM.SEMESTER 
                                     AND SEM.ACADEMIC_YEAR=?AC_YEAR
                                     AND SEM.IS_ACTIVE=1
                                GROUP BY FE.QUESTION , FE.ASSESSEE , FE.COURSE_ID,FE.CLASS_ID,STF.STAFF_CODE
                                ORDER BY FE.ASSESSEE , FG.QUESTION_GROUP_ID;";

                        break;
                    }

                case FeedbackSQLCommands.FetchQuestionWiseReportByStaffIds:
                    {
                        query = @"SELECT 
                                        CONCAT(IF(STF.FIRST_NAME IS NULL,
                                                    '',
                                                    STF.FIRST_NAME),
                                                ' ',
                                                IF(STF.LAST_NAME IS NULL,
                                                    '',
                                                    STF.LAST_NAME)) AS 'STAFF_NAME',
                                        STF.STAFF_CODE,
                                        FE.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        CT.COURSE_TYPE,
                                        CR.COURSE_CODE,
                                        FQ.QUESTION,
                                        FG.GROUP_TITLE,
                                        COUNT(IF(FE.ANSWER = 1, 1, NULL)) AS VERY_GOOD,
                                        COUNT(IF(FE.ANSWER = 2, 1, NULL)) AS GOOD,
                                        COUNT(IF(FE.ANSWER = 3, 1, NULL)) AS SATISFACTORY,
                                        COUNT(IF(FE.ANSWER = 4, 1, NULL)) AS POOR,
                                        COUNT(IF(FE.ANSWER = 5, 1, NULL)) AS VERY_POOR,
                                        FE.ASSESSEE,
                                        DP.DEPARTMENT,
                                        CLS.CLASS_NAME,
                                        COUNT(FE.ASSESSOR) AS ASSESSOR,
                                        (SELECT 
                                                COUNT(*)
                                            FROM
                                                STU_COURSE_INFO AS SCLS
                                            WHERE
                                                SCLS.S_CLASS_ID = CLS.CLASS_ID
                                                    AND SCLS.COURSE_ID = CR.COURSE_ROOT_ID
                                                    AND SCLS.ACADEMIC_YEAR=?AC_YEAR 
                                                    AND SCLS.IS_DELETED!=1) AS TOTAL_STUDENTS
                                    FROM
                                        FBEVALUATION?AC_YEAR AS FE
                                            INNER JOIN
                                        FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FE.QUESTION
                                            INNER JOIN
                                        FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = FE.COURSE_ID
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CR.COURSE_TYPE = CT.COURSE_TYPE_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = CR.DEPARTMENT
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON FE.CLASS_ID = CLS.CLASS_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.ASSESSEE
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    WHERE
                                        ASSESSEE IN (?ASSESSEE)
                                            AND SEM.ACADEMIC_YEAR=?AC_YEAR
                                            AND SEM.SEMESTER = CR.SEMESTER_ID
                                            AND SEM.IS_ACTIVE = 1
                                    GROUP BY FE.QUESTION , FE.ASSESSEE , FE.COURSE_ID , FE.CLASS_ID
                                    ORDER BY FE.ASSESSEE , FG.QUESTION_GROUP_ID;";

                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCourseListByClassId:
                    {
                        query = @"SELECT 
                                        CC.COURSE_ID, RT.COURSE_TITLE
                                    FROM
                                        STU_PERSONAL_INFO AS ST
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ST.CLASS_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE AS CC ON CC.CLASS_ID = ST.CLASS_ID
                                            AND CC.IS_DELETED = 1
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CC.COURSE_ID
                                            AND RT.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    WHERE
                                        ST.STUDENT_ID = ?STUDENT_ID
                                            AND ST.IS_DELETED != 1
                                            AND ST.IS_LEFT != 1
                                            AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND RT.SEMESTER_ID = SEM.SEMESTER
                                            AND SEM.IS_ACTIVE = 1 
                                    UNION SELECT 
                                        RT.COURSE_ROOT_ID, RT.COURSE_TITLE
                                    FROM
                                        STU_PERSONAL_INFO AS ST
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ST.CLASS_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE AS CC ON CC.CLASS_ID = ST.CLASS_ID
                                            AND CC.IS_DELETED != 1
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_COURSE_INFO AS SC ON SC.STUDENT_ID = ST.STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = SC.COURSE_ID
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND ST.IS_DELETED != 1
                                            AND ST.IS_LEFT != 1
                                            AND SC.CLASS_ID != SC.S_CLASS_ID
                                            AND RT.SEMESTER_ID = SEM.SEMESTER
                                            AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SEM.IS_ACTIVE = 1
                                    GROUP BY SC.COURSE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFBCoursewiseStaff:
                    {
                        //query = @"SELECT 
                        //            CONCAT(IF(SP.FIRST_NAME IS NULL,
                        //                        '',
                        //                        SP.FIRST_NAME),
                        //                    ' ',
                        //                    IF(SP.LAST_NAME IS NULL,
                        //                        '',
                        //                        SP.LAST_NAME)) AS STAFF_NAME,
                        //            S.STAFF_ID,
                        //            S.CLASS_ID
                        //        FROM
                        //            FBCLASS_WISE_STAFF S
                        //                INNER JOIN
                        //            STF_PERSONAL_INFO SP ON SP.STAFF_ID = S.STAFF_ID
                        //        WHERE
                        //            S.CLASS_ID = (SELECT 
                        //                    CLASS_ID
                        //                FROM
                        //                    STU_CLASS AS SC
                        //                WHERE
                        //                    SC.STUDENT_ID = ?STUDENT_ID
                        //                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                LIMIT 1)
                        //                AND S.COURSE_ID = ?COURSE_ID
                        //                AND S.ACADEMIC_YEAR = ?AC_YEAR 
                        //        UNION SELECT 
                        //            CONCAT(IF(SP.FIRST_NAME IS NULL,
                        //                        '',
                        //                        SP.FIRST_NAME),
                        //                    ' ',
                        //                    IF(SP.LAST_NAME IS NULL,
                        //                        '',
                        //                        SP.LAST_NAME)) AS STAFF_NAME,
                        //            S.STAFF_ID,
                        //            S.CLASS_ID
                        //        FROM
                        //            FBCLASS_WISE_STAFF S
                        //                INNER JOIN
                        //            STF_PERSONAL_INFO SP ON SP.STAFF_ID = S.STAFF_ID
                        //        WHERE
                        //            S.CLASS_ID = (SELECT 
                        //                    S_CLASS_ID
                        //                FROM
                        //                    STU_COURSE_INFO AS SC
                        //                WHERE
                        //                    SC.STUDENT_ID = ?STUDENT_ID AND COURSE_ID = ?COURSE_ID
                        //                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                LIMIT 1)
                        //                AND S.COURSE_ID = ?COURSE_ID
                        //                AND S.ACADEMIC_YEAR = ?AC_YEAR;";
                        query = @"SELECT 
                                    CONCAT(IF(SP.FIRST_NAME IS NULL,
                                                '',
                                                SP.FIRST_NAME),
                                            ' ',
                                            IF(SP.LAST_NAME IS NULL,
                                                '',
                                                SP.LAST_NAME)) AS STAFF_NAME,
                                    S.STAFF_ID,
                                    S.CLASS_ID
                                FROM
                                    CP_CLASS_COURSE_STAFF S
                                        INNER JOIN
                                    STF_PERSONAL_INFO SP ON SP.STAFF_ID = S.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    S.CLASS_ID = (SELECT 
                                            CLASS_ID
                                        FROM
                                            STU_CLASS AS SC
                                        WHERE
                                            SC.STUDENT_ID = ?STUDENT_ID
                                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        LIMIT 1)
                                        AND S.COURSE_ID = ?COURSE_ID
                                        AND S.ACADEMIC_YEAR = ?AC_YEAR
                                        AND S.IS_DELETED != 1 
                                UNION SELECT 
                                    CONCAT(IF(SP.FIRST_NAME IS NULL,
                                                '',
                                                SP.FIRST_NAME),
                                            ' ',
                                            IF(SP.LAST_NAME IS NULL,
                                                '',
                                                SP.LAST_NAME)) AS STAFF_NAME,
                                    S.STAFF_ID,
                                    S.CLASS_ID
                                FROM
                                    CP_CLASS_COURSE_STAFF S
                                        INNER JOIN
                                    STF_PERSONAL_INFO SP ON SP.STAFF_ID = S.STAFF_ID
		                                AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    S.CLASS_ID = (SELECT 
                                            S_CLASS_ID
                                        FROM
                                            STU_COURSE_INFO AS SC
                                        WHERE
                                            SC.STUDENT_ID = ?STUDENT_ID AND COURSE_ID = ?COURSE_ID
                                                AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        LIMIT 1)
                                        AND S.COURSE_ID = ?COURSE_ID
                                        AND S.ACADEMIC_YEAR = ?AC_YEAR
                                        AND S.IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFBClassewiseStaffIncharge:
                    {
                        query = @"SELECT 
                                    CONCAT(IF(SP.FIRST_NAME IS NULL,
                                                '',
                                                SP.FIRST_NAME),
                                            ' ',
                                            IF(SP.LAST_NAME IS NULL,
                                                '',
                                                SP.LAST_NAME)) AS STAFF_NAME,
                                    S.STAFF_ID,
                                    S.CLASS_ID
                                FROM
                                    CLASS_INCHARGE AS S
                                        INNER JOIN
                                    STF_PERSONAL_INFO SP ON SP.STAFF_ID = S.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        AND S.ACADEMIC_YEAR = ?AC_YEAR
                                        AND S.CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.ValidateCourseByStudentId:
                    {
                        query = @" SELECT 
                                       FE.FBSTUDENT_ID,
                                        FE.ASSESSOR,
                                        FE.CLASS_ID,
                                        FE.ASSESSEE, 
                                        FE.FEEDBACK_ID,
                                        FE.FEEDBACK_ROLE_ID, 
                                        FE.ROLE,
                                        FE.QUESTION,
                                        FE.ANSWER,
                                        FE.ISDELETED,
                                        FE.COURSE_ID
                                        FROM 
                                        FBEVALUATION?AC_YEAR as FE
                                        
                                         INNER JOIN
                                    FBFEEDBACK_ROLE_SETTINGS AS FR ON FR.FEEDBACK_ROLE_ID=FE.FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED!=1
                                     
                                    WHERE 
                                        FE.COURSE_ID =?COURSE_ID AND FE.ASSESSEE =?ASSESSEE  
                                            AND FE.FEEDBACK_ID=?FEEDBACK_ID
                                            AND FR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID
                                            AND ASSESSOR =?ASSESSOR;";
                        break;
                    }
                case FeedbackSQLCommands.ValidateCommonFeedbackyStudentId:
                    {
                        query = @" SELECT 
                                       FE.FBSTUDENT_ID,
                                        FE.ASSESSOR,
                                        FE.CLASS_ID,
                                        FE.ASSESSEE, 
                                        FE.FEEDBACK_ID,
                                        FE.FEEDBACK_ROLE_ID, 
                                        FE.ROLE,
                                        FE.QUESTION,
                                        FE.ANSWER,
                                        FE.ISDELETED,
                                        FE.COURSE_ID
                                        FROM 
                                        FBEVALUATION?AC_YEAR as FE
                                        
                                         INNER JOIN
                                    FBFEEDBACK_ROLE_SETTINGS AS FR ON FR.FEEDBACK_ROLE_ID=FE.FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED!=1
                                     
                                    WHERE 
                                         FE.FEEDBACK_ID=?FEEDBACK_ID
                                            AND FE.FEEDBACK_ROLE_ID=?FEEDBACK_ROLE_ID
                                            AND ASSESSOR =?ASSESSOR;";
                        break;
                    }
                case FeedbackSQLCommands.ValidateCommonFeedbackStudentId:
                    {
                        query = @" SELECT 
                                       FE.FBSTUDENT_ID,
                                        FE.ASSESSOR,
                                        FE.CLASS_ID,
                                        FE.ASSESSEE, 
                                        FE.FEEDBACK_ID,
                                        FE.FEEDBACK_ROLE_ID, 
                                        FE.ROLE,
                                        FE.QUESTION,
                                        FE.ANSWER,
                                        FE.ISDELETED,
                                        FE.COURSE_ID
                                        FROM 
                                        FBEVALUATION?AC_YEAR as FE
                                        
                                         INNER JOIN
                                    FBFEEDBACK_ROLE_SETTINGS AS FR ON FR.FEEDBACK_ROLE_ID=FE.FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED!=1
                                     
                                    WHERE 
                                         FE.FEEDBACK_ID=?FEEDBACK_ID
                                            AND FR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID
                                            AND ASSESSOR =?ASSESSOR;";
                        break;
                    }

                case FeedbackSQLCommands.FetchclassIdbyStudentAndAcademicYear:
                    {
                        query = @"SELECT 
                                    STUDENT_ID, ACADEMIC_YEAR, CLASS_ID
                                FROM
                                    STU_CLASS
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case FeedbackSQLCommands.CheckCourseTypeByCourseId:
                    {
                        query = @"SELECT 
                                        COUNT(*) > 0 AS STATUS
                                    FROM
                                        CP_COURSE_ROOT AS CR
                                            INNER JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    WHERE
                                        CR.COURSE_ROOT_ID = ?COURSE_ROOT_ID
                                            AND CT.COURSE_TYPE LIKE '%PRACTICAL%'; ";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackInfo:
                    {

                        query = @"SELECT 
                                        FEEDBACK_NAME, DATE_FROM, DATE_TO, ACADEMIC_YEAR,FEEDBACK_ID
                                    FROM
                                        FBFEEDBACK_SETTINGS
                                    WHERE
                                        IS_ACTIVE=1;";

                        break;
                    }
                case FeedbackSQLCommands.FetchCourseAndClassInfoByCourse:
                    {
                        query = @"SELECT 
                                        (SELECT 
                                                COUNT(CLS1.STUDENT_ID)
                                            FROM
                                                STU_CLASS AS CLS1
                                            WHERE
                                                CLS1.CLASS_ID = CLS.CLASS_ID
                                                    AND CLS1.ACADEMIC_YEAR = ?AC_YEAR
                                                    AND CLS1.IS_DELETED != 1) AS TOTAL_STUDENTS,
                                        CR.COURSE_TITLE,
                                        CLS.CLASS_NAME,
                                        CT.COURSE_TYPE,
                                        CR.COURSE_CODE,
                                        DP.DEPARTMENT,
                                        CONCAT(IF(SP.FIRST_NAME IS NULL,
                                                    '',
                                                    SP.FIRST_NAME),
                                                ' ',
                                                IF(SP.LAST_NAME IS NULL,
                                                    '',
                                                    SP.LAST_NAME)) AS STAFF_NAME,
                                        SP.STAFF_CODE
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS FS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = FS.COURSE_ID
                                            LEFT JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = FS.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = CR.DEPARTMENT
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = FS.STAFF_ID
                                    WHERE
                                        FS.COURSE_ID = ?COURSE_ID AND FS.STAFF_ID = ?STAFF_ID
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.CLASS_ID = ?CLASS_ID
                                            AND FS.IS_DELETED!=1
                                    GROUP BY CR.COURSE_CODE;";
                        break;
                    }
                case FeedbackSQLCommands.FetchCourseListByStaffId:
                    {
                        query = @"SELECT 
                                        cs.COURSE_ID, cr.COURSE_TITLE
                                    FROM
                                       CP_CLASS_COURSE_STAFF AS cs
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CS.COURSE_ID
                                            AND CR.IS_DELETED!=1
                                            INNER JOIN
                                       SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CS.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE=1 
                                    WHERE
                                        STAFF_ID =?STAFF_ID 
                                        AND CS.ACADEMIC_YEAR=?AC_YEAR 
                                        AND CS.IS_DELETED!=1 ;";
                        break;
                    }
                case FeedbackSQLCommands.FetchDepartmentList:
                    {

                        query = @"SELECT 
                                    DEPARTMENT, DEPARTMENT_ID
                                FROM
                                    CP_DEPARTMENT;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStaffListByDepartment:
                    {

                        query = @"SELECT 
                                    STF.STAFF_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME,''),' ',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME
                                FROM
                                    STF_PERSONAL_INFO AS STF
                                        INNER JOIN 
                                        CP_DEPARTMENT AS  CD ON CD.DEPARTMENT_ID=STF.DEPARTMENT
                                        AND CD.IS_DELETED!=1
                                        INNER JOIN
                                    USER_ROLES_BY_ACADEMIC_YEAR AS URA ON URA.USER_ID = STF.STAFF_ID
                                        AND URA.IS_DELETED != 1
                                        AND URA.ACADEMIC_YEAR =?AC_YEAR
                                        AND URA.USER_TYPE = 4
                                WHERE
                                    STF.IS_DELETED != 1 AND STF.IS_LEFT != 1
                                    AND STF.DEPARTMENT=?DEPARTMENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchShiftList:
                    {

                        query = @"SELECT SHIFT_ID, SHIFT_NAME FROM SUP_SHIFT;";
                        break;
                    }
                case FeedbackSQLCommands.FetchDepartmentByShiftId:
                    {

                        query = @"SELECT 
                                        CLS.DEPARTMENT AS DEPARTMENT_ID , DP.DEPARTMENT
                                    FROM
                                        CP_CLASSES AS CLS
                                            INNER JOIN
                                        CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = CLS.DEPARTMENT
                                            AND DP.IS_DELETED!=1
                                    WHERE
                                        CLS.SHIFT =?SHIFT
                                        AND CLS.IS_DELETED!=1
                                    GROUP BY CLS.DEPARTMENT;";
                        break;
                    }

                #region Examiners FeedBack Querys
                case FeedbackSQLCommands.FetchExaminersActiveFeedBack:
                    {
                        query = @"SELECT 
                                        FEEDBACK_ID
                                    FROM
                                        FB_EXAMINERS_FEEDBACK_SETTINGS
                                    WHERE
                                        IS_ACTIVE = 1 AND ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED != 1;";
                        break;
                    }

                case FeedbackSQLCommands.FetchExaminersQuestionList:
                    {
                        query = @"SELECT 
	                                    QUESTION_ID,
                                        QUESTION, 
                                        GROUP_TITLE AS QUESTION_TYPE
                                    FROM
                                        FB_EXAMINERS_FEEDBACK_QUESTIONS AS FQ
                                            INNER JOIN
                                        FB_EXAMINERS_QUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                    WHERE
                                        FQ.FEEDBACK_ID = ?FEEDBACK_ID;";
                        break;
                    }

                case FeedbackSQLCommands.ValidateExaminersIsExist:
                    {
                        query = @"SELECT 
                                    FBEVALUATION_ID
                                FROM
                                    FB_EXAMINERS_EVALUATION_?AC_YEAR
                                WHERE
                                    EXAMINERS_ID = ?EXAMINERS_ID AND FEEDBACK_ID = ?FEEDBACK_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }

                case FeedbackSQLCommands.SaveExaminersFeedBack:
                    {
                        query = @"INSERT INTO FB_EXAMINERS_EVALUATION_?AC_YEAR
                                  (
                                     EXAMINERS_ID,
                                     FEEDBACK_ID,
                                     QUESTION,
                                     ANSWER
                                  )
                                  VALUES
                                  (
                                     ?EXAMINERS_ID,
                                     ?FEEDBACK_ID,
                                     ?QUESTION,
                                     ?ANSWER
                                  );";
                        break;
                    }

                case FeedbackSQLCommands.ResultForExaminersFeedack:
                    {
                        query = @"SELECT 
                                    COUNT(FE.EXAMINERS_ID) AS ASSESSOR,
                                    FQ.QUESTION,
                                    FG.GROUP_TITLE,	                                   
                                    COUNT(IF(FE.ANSWER = 1, 1, NULL)) AS VERY_GOOD,
									COUNT(IF(FE.ANSWER = 2, 1, NULL)) AS GOOD,
									COUNT(IF(FE.ANSWER = 3, 1, NULL)) AS SATISFACTORY,
									COUNT(IF(FE.ANSWER = 4, 1, NULL)) AS POOR,
									COUNT(IF(FE.ANSWER = 5, 1, NULL)) AS VERY_POOR
                                FROM
                                    FB_EXAMINERS_EVALUATION_?AC_YEAR AS FE
                                        INNER JOIN
                                    FB_EXAMINERS_FEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FE.QUESTION
                                        INNER JOIN
                                    FB_EXAMINERS_QUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                WHERE
									FE.FEEDBACK_ID=?FEEDBACK_ID
                                GROUP BY FE.QUESTION ORDER BY FG.QUESTION_GROUP_ID;";
                        break;
                    }
                #endregion

                case FeedbackSQLCommands.FetchFeedBackSettings:
                    {
                        query = @"SELECT 
                                    FS.FEEDBACK_ID,
                                    FS.FEEDBACK_NAME,
                                    DATE_FORMAT(FS.DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(FS.DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                    FS.NO_OF_QUESTION,
                                    FS.ACADEMIC_YEAR,
                                    FS.IS_DELETED,
                                    FS.CLASS_ID,
                                    FS.IS_ACTIVE,
                                    FS.ASSESSOR,
                                    FS.ASSESSEE,
                                    FS.MANUAL_STAFF_SELECTION,
                                    FS.WITH_IN_DEPARTMENT,
                                    FS.ASSESSOR_CATEGORY,
                                    FS.ASSESSEE_CATEGORY
                                FROM
                                    FBFEEDBACK_SETTINGS AS FS
                                        
                                WHERE
                                    FS.IS_DELETED != 1
                                        AND FS.ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackSettingsById:
                    {
                        query = @"SELECT 
                                    FS.FEEDBACK_ID,
                                    FS.FEEDBACK_NAME,
                                     DATE_FORMAT(FS.DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                     DATE_FORMAT(FS.DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                    FS.NO_OF_QUESTION,
                                    FS.ACADEMIC_YEAR,
                                    FS.IS_DELETED,
                                    FS.CLASS_ID,
                                    FS.IS_ACTIVE,
                                    FS.FEEDBACK_TYPE,
                                    FS.ASSESSOR,
                                    FS.ASSESSEE,
                                    FS.MANUAL_STAFF_SELECTION,
                                    FS.WITH_IN_DEPARTMENT,
                                    FS.ASSESSOR_CATEGORY,
                                    FS.ASSESSEE_CATEGORY
                                FROM
                                    FBFEEDBACK_SETTINGS AS FS
                                WHERE
                                    FS.IS_DELETED != 1
                                        AND FS.ACADEMIC_YEAR =?AC_YEAR
                                         AND FS.FEEDBACK_ID=?FEEDBACK_ID";
                        break;
                    }
                case FeedbackSQLCommands.SaveFeedBackSetting:
                    {
                        query = @"INSERT INTO FBFEEDBACK_SETTINGS
                                (FEEDBACK_NAME,
                                 DATE_FROM, 
                                 DATE_TO, 
                                 ACADEMIC_YEAR)
 
                                 VALUES(
                                 ?FEEDBACK_NAME,
                                 ?DATE_FROM,
                                 ?DATE_TO,
                                 ?AC_YEAR);";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackSetting:
                    {
                        query = @"UPDATE 
                                    FBFEEDBACK_SETTINGS SET 
                                    FEEDBACK_NAME=?FEEDBACK_NAME, 
                                    DATE_FROM=?DATE_FROM,
                                    DATE_TO=?DATE_TO,
                                     ACADEMIC_YEAR=?AC_YEAR
                                --    NO_OF_QUESTION=?NO_OF_QUESTION,
                                --    FEEDBACK_CATEGORY=?FEEDBACK_CATEGORY

                                    WHERE 
                                    FEEDBACK_ID=?FEEDBACK_ID;";
                        break;
                    }
                case FeedbackSQLCommands.ActivateorDeactivateFeedBackSettingById:
                    {
                        query = @"UPDATE FBFEEDBACK_SETTINGS SET IS_ACTIVE=?IS_ACTIVE
                                    WHERE FEEDBACK_ID=?FEEDBACK_ID;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedBackSettingById:
                    {
                        query = @"UPDATE 
                                    FBFEEDBACK_SETTINGS 
                                        SET 
                                        IS_DELETED=?IS_DELETED 
                                        WHERE
                                    FEEDBACK_ID=?FEEDBACK_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackRoleSettings:
                    {
                        query = @"SELECT 
                                FRS.FEEDBACK_ROLE_ID,
                                FS.FEEDBACK_NAME,
                                FR.FEEDBACK_ROLE_NAME,
                                FRS.IS_ATTENDED
    
                            FROM
                                FBFEEDBACK_ROLE_SETTINGS AS FRS
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FRS.FEEDBACK_ID
                                AND FS.IS_ACTIVE=1
                                AND FS.ACADEMIC_YEAR=?AC_YEAR
                                    INNER JOIN
                                SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED != 1
                            WHERE
                                FRS.IS_DELETED != 1
                                AND FRS.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackRoleSettingsById:
                    {
                        query = @"SELECT 
                                    FRS.FEEDBACK_ROLE_ID,
                                    FR.SUP_FEEDBACK_ROLE_ID,
                                    FRS.FEEDBACK_ID,
                                    FRS.IS_ATTENDED
    
                                FROM
                                    FBFEEDBACK_ROLE_SETTINGS AS FRS
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FRS.FEEDBACK_ID
                                        INNER JOIN
                                    SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                        AND FR.IS_DELETED != 1
                                WHERE
                                    FRS.IS_DELETED != 1
                                    AND FRS.FEEDBACK_ROLE_ID=?FEEDBACK_ROLE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackRoleSetting:
                    {
                        query = @"UPDATE 
                                FBFEEDBACK_ROLE_SETTINGS
                                SET 
                                FEEDBACK_ID=?FEEDBACK_ID,
                                SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID,
                               ACADEMIC_YEAR=?AC_YEAR,
                                IS_ATTENDED=?IS_ATTENDED
                                WHERE 
                                FEEDBACK_ROLE_ID=?FEEDBACK_ROLE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.SaveFeedBackRoleSetting:
                    {
                        query = @"INSERT 
                                    INTO 
                                    FBFEEDBACK_ROLE_SETTINGS
                                    (FEEDBACK_ID, 
                                     SUP_FEEDBACK_ROLE_ID,
                                    ACADEMIC_YEAR,
                                     IS_ATTENDED)
                                   VALUES
                                  (?FEEDBACK_ID,
                                   ?SUP_FEEDBACK_ROLE_ID,
                                   ?AC_YEAR,
                                   ?IS_ATTENDED);";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedBackRoleSettingById:
                    {
                        query = @"UPDATE
                                  FBFEEDBACK_ROLE_SETTINGS
                                  SET 
                                  IS_DELETED=?IS_DELETED
                                  WHERE
                                  FEEDBACK_ROLE_ID=?FEEDBACK_ROLE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionBySettingAndFeedBackRoleAndQuestionGroup:
                    {
                        query = @"
                                SELECT 
                                    FQ.QUESTION_ID,
                                    FS.FEEDBACK_NAME,
                                    SFR.FEEDBACK_ROLE_NAME,
                                    FQ.QUESTION,
                                    FG.GROUP_TITLE 
                                FROM
                                    FBFEEDBACK_QUESTIONS AS FQ
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                        AND FRS.IS_DELETED != 1
                                        AND FRS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                          INNER JOIN
                                    SUP_FBFEEDBACK_ROLE AS SFR ON SFR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                        AND SFR.IS_DELETED != 1
                                        INNER JOIN
                                    FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                        AND FG.IS_DELETED != 1
                                WHERE
                                    FQ.FEEDBACK_ID = ?FEEDBACK_ID
                                        AND FQ.FEEDBACK_ROLE_ID =?FEEDBACK_ROLE_ID
                                        AND FQ.QUESTION_GROUP=?QUESTION_GROUP
                                        AND FQ.IS_DELETED != 1
                                        ORDER BY FQ.QUESTION_ID;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackQuestionsByQuestionId:
                    {
                        query = @"UPDATE 
                                    FBFEEDBACK_QUESTIONS 
                                    SET
                                    FEEDBACK_ID=?FEEDBACK_ID,
                                    FEEDBACK_ROLE_ID=?FEEDBACK_ROLE_ID,
                                    QUESTION=?QUESTION,
                                    QUESTION_GROUP=?QUESTION_GROUP,
                                    ACADEMIC_YEAR=?AC_YEAR

                                    WHERE
                                    QUESTION_ID=?QUESTION_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionsByQuestionId:
                    {
                        query = @"SELECT 
                                    FQ.QUESTION_ID,
                                    FQ.FEEDBACK_ID,
                                    FQ.FEEDBACK_ROLE_ID,
                                    FQ.QUESTION,
                                    FQ.QUESTION_TYPE,
                                    FQ.QUESTION_GROUP
    
                                FROM
                                    FBFEEDBACK_QUESTIONS AS FQ

                                WHERE
                                    FQ.QUESTION_ID = ?QUESTION_ID;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedBackQuestionByQuestionId:
                    {
                        query = @"UPDATE FBFEEDBACK_QUESTIONS 
                                SET 
                                    IS_DELETED = ?IS_DELETED
                                WHERE
                                    QUESTION_ID = ?QUESTION_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackRoleSettingsByFeedBackidAndRole:
                    {
                        query = @"
                                SELECT 
                                FRS.FEEDBACK_ROLE_ID,
                                FR.FEEDBACK_ROLE_NAME,
                                FRS.SUP_FEEDBACK_ROLE_ID,
                                FRS.IS_ATTENDED,
                                FR.ASSESSOR_ROLE,
                                FR.ASSESSEE_ROLE
    
                            FROM
                                FBFEEDBACK_ROLE_SETTINGS AS FRS
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FRS.FEEDBACK_ID
                                AND FS.IS_ACTIVE=1
                                    INNER JOIN
                                SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED != 1
                            WHERE
                                FRS.IS_DELETED != 1
                                AND FRS.FEEDBACK_ID=?FEEDBACK_ID
                                AND FR.ASSESSOR_ROLE IN(?ASSESSOR_ROLE)
                                ORDER BY FRS.FEEDBACK_ROLE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionViewByFeedbackAndAssessorAndAssessee:
                    {
                        query = @"        
                                    SELECT 
                                FQ.FEEDBACK_ID,
                                FQ.QUESTION_ID,
                                FS.FEEDBACK_NAME,
                                FR.FEEDBACK_ROLE_NAME,
                                FQ.QUESTION,
                              --  SQ.QUESTION_TYPE,
                                FG.GROUP_TITLE
                            FROM
                                FBFEEDBACK_QUESTIONS AS FQ
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                    AND FS.IS_ACTIVE = 1
                                    AND FS.IS_DELETED != 1
                                    INNER JOIN
                                FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                    AND FG.IS_DELETED != 1
                                   -- INNER JOIN
                              --  SUP_FBFEEDBACK_QUESTION_TYPE AS SQ ON SQ.QUESTION_TYPE_ID = FQ.QUESTION_TYPE
                               --     AND SQ.IS_DELETED != 1
                                    INNER JOIN
                                FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                    AND FRS.IS_DELETED != 1
                                    INNER JOIN
                                SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED != 1
                            WHERE
                                FR.ASSESSOR_ROLE=?ASSESSOR_ROLE
                                    AND FR.ASSESSEE_ROLE =?ASSESSEE_ROLE
                                    AND FQ.FEEDBACK_ID=?FEEDBACK_ID
                                    AND FR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID
                                    AND FQ.IS_DELETED!=1
                                    ORDER BY FQ.QUESTION_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionViewByFeedbackAndAssessorAndAssesseeNew:
                    {
                        query = @"        
                                    SELECT 
                                FQ.FEEDBACK_ID,
                                FQ.QUESTION_ID,
                                FS.FEEDBACK_NAME,
                                FR.FEEDBACK_ROLE_NAME,
                                FRS.FEEDBACK_ROLE_ID,
                                FQ.QUESTION,
                              --  SQ.QUESTION_TYPE,
                                FG.GROUP_TITLE,
                                FQ.TAMIL_QUESTION
                            FROM
                                FBFEEDBACK_QUESTIONS AS FQ
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                    AND FS.IS_ACTIVE = 1
                                    AND FS.IS_DELETED != 1
                                    INNER JOIN
                                FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                    AND FG.IS_DELETED != 1
                                   -- INNER JOIN
                              --  SUP_FBFEEDBACK_QUESTION_TYPE AS SQ ON SQ.QUESTION_TYPE_ID = FQ.QUESTION_TYPE
                               --     AND SQ.IS_DELETED != 1
                                    INNER JOIN
                                FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                    AND FRS.IS_DELETED != 1
                                    INNER JOIN
                                SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED != 1
                            WHERE
                                FR.ASSESSOR_ROLE=?ASSESSOR_ROLE
                                    AND FR.ASSESSEE_ROLE =?ASSESSEE_ROLE
                                    AND FQ.FEEDBACK_ID=?FEEDBACK_ID
                                    AND FR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID
                                    AND FQ.IS_DELETED!=1
                                    ORDER BY FQ.QUESTION_ID;";
                        break;
                    }

                case FeedbackSQLCommands.FetchFeedBackQuestionObjectiveMappingByFeedbackAndAssessorAndAssessee:
                    {
                        query = @"SELECT 
                                        FQ.FEEDBACK_ID, FQ.QUESTION_ID, FM.OBJECTIVE_ID,O.OBJECTIVES,O.RATING
                                    FROM
                                        FBFEEDBACK_QUESTIONS AS FQ
                                            INNER JOIN
                                        FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                            AND FS.IS_ACTIVE = 1
                                            AND FS.IS_DELETED != 1
                                            INNER JOIN
                                        FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                            AND FG.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                            AND FRS.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                            AND FR.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS FM ON FM.FEEDBACK_ID = FQ.FEEDBACK_ID
                                            AND FM.QUESTION_ID = FQ.QUESTION_ID
                                            AND FM.IS_DELETED != 1
                                            LEFT JOIN
                                        FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FM.OBJECTIVE_ID
                                            AND O.IS_DELETED != 1
                                    WHERE
                                        FR.ASSESSOR_ROLE =?ASSESSOR_ROLE
                                            AND FR.ASSESSEE_ROLE =?ASSESSEE_ROLE
                                            AND FQ.FEEDBACK_ID =?FEEDBACK_ID
                                            AND FR.SUP_FEEDBACK_ROLE_ID =?SUP_FEEDBACK_ROLE_ID
                                            AND FQ.IS_DELETED != 1
                                    ORDER BY FQ.QUESTION_ID,O.OBJECTIVE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionObjectiveMappingByFeedbackAndAssessorAndAssesseeNew:
                    {
                        query = @"SELECT 
                                        FQ.FEEDBACK_ID, FQ.QUESTION_ID, FM.OBJECTIVE_ID,O.OBJECTIVES,O.RATING,O.TAMIL_OBJECTIVES
                                    FROM
                                        FBFEEDBACK_QUESTIONS AS FQ
                                            INNER JOIN
                                        FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                            AND FS.IS_ACTIVE = 1
                                            AND FS.IS_DELETED != 1
                                            INNER JOIN
                                        FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                            AND FG.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                            AND FRS.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                            AND FR.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS FM ON FM.FEEDBACK_ID = FQ.FEEDBACK_ID
                                            AND FM.QUESTION_ID = FQ.QUESTION_ID
                                            AND FM.IS_DELETED != 1
                                            LEFT JOIN
                                        FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FM.OBJECTIVE_ID
                                            AND O.IS_DELETED != 1
                                    WHERE
                                        FR.ASSESSOR_ROLE =?ASSESSOR_ROLE
                                            AND FR.ASSESSEE_ROLE =?ASSESSEE_ROLE
                                            AND FQ.FEEDBACK_ID =?FEEDBACK_ID
                                            AND FR.SUP_FEEDBACK_ROLE_ID =?SUP_FEEDBACK_ROLE_ID
                                            AND FQ.IS_DELETED != 1
                                    ORDER BY FQ.QUESTION_ID,O.OBJECTIVE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionObjectiveMappingByFeedbackAndAssessorNew:
                    {
                        query = @"SELECT 
                                        FQ.FEEDBACK_ID, FQ.QUESTION_ID, FM.OBJECTIVE_ID,O.OBJECTIVES,O.RATING,O.TAMIL_OBJECTIVES
                                    FROM
                                        FBFEEDBACK_QUESTIONS AS FQ
                                            INNER JOIN
                                        FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                            AND FS.IS_ACTIVE = 1
                                            AND FS.IS_DELETED != 1
                                            INNER JOIN
                                        FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                            AND FG.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                            AND FRS.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                            AND FR.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS FM ON FM.FEEDBACK_ID = FQ.FEEDBACK_ID
                                            AND FM.QUESTION_ID = FQ.QUESTION_ID
                                            AND FM.IS_DELETED != 1
                                            LEFT JOIN
                                        FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FM.OBJECTIVE_ID
                                            AND O.IS_DELETED != 1
                                    WHERE
                                        FR.ASSESSOR_ROLE =?ASSESSOR_ROLE
                                            AND FQ.FEEDBACK_ID =?FEEDBACK_ID
                                            AND FR.SUP_FEEDBACK_ROLE_ID =?SUP_FEEDBACK_ROLE_ID
                                            AND FQ.IS_DELETED != 1
                                    ORDER BY FQ.QUESTION_ID,O.OBJECTIVE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackEvaluationByFeedBackAndAssessorAndAssessee:
                    {
                        query = @"SELECT 
                                    FE.FBSTUDENT_ID,
                                    FE.ASSESSOR,
                                    FE.CLASS_ID,
                                    FE.ASSESSEE,
                                    FE.FEEDBACK_ID,
                                    FE.FEEDBACK_ROLE_ID,
                                    FE.ROLE,
                                    FE.QUESTION,
                                    FE.ANSWER,
                                    FE.ISDELETED,
                                    FE.COURSE_ID
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                    INNER JOIN
                                    FBFEEDBACK_ROLE_SETTINGS AS FR ON FR.FEEDBACK_ROLE_ID=FE.FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED!=1
                                WHERE
                                    FE.FEEDBACK_ID =?FEEDBACK_ID AND FE.ASSESSOR =?ASSESSOR
                                        AND FE.ASSESSEE =?ASSESSEE
                                        AND FR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID
                                        AND FE.ISDELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStaffByProgrammeGroup:
                    {
                        query = @"SELECT 
                                         CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,STF.STAFF_ID
                                    FROM
                                        STF_PERSONAL_INFO AS STF
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.DEPARTMENT = STF.DEPARTMENT
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                            AND CPG.IS_DELETED != 1
                                            
                                             INNER JOIN
                                    user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                  AND AY.ACADEMIC_YEAR=?AC_YEAR
                                   AND AY.USER_TYPE='4'
                                   AND AY.ROLE_ID=4
                                  AND AY.IS_DELETED != 1
                                    WHERE
                                        CPG.PROGRAMME_GROUP_ID =?PROGRAMME_GROUP_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        GROUP BY AY.USER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackRoleSettingIsAttendedByFeedbackRoleId:
                    {
                        query = @"
                                UPDATE FBFEEDBACK_ROLE_SETTINGS 
                                SET 
                                    IS_ATTENDED =?IS_ATTENDED
                                WHERE
                                    FEEDBACK_ROLE_ID =?FEEDBACK_ROLE_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackRoleSettingByFeedBackandSupFeedBackRoleId:
                    {
                        query = @"
                                SELECT 
                                    FEEDBACK_ROLE_ID,
                                    FEEDBACK_ID,
                                    SUP_FEEDBACK_ROLE_ID,
                                    IS_ATTENDED,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    FBFEEDBACK_ROLE_SETTINGS
                                WHERE
                                    FEEDBACK_ID =?FEEDBACK_ID
                                        AND SUP_FEEDBACK_ROLE_ID =?SUP_FEEDBACK_ROLE_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbQuestionsGroupByFeedbackId:
                    {
                        query = @"SELECT 
                                    FS.FEEDBACK_NAME,
                                   DATE_FORMAT(FS.DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                     DATE_FORMAT(FS.DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                    FG.GROUP_TITLE,
                                    FG.QUESTION_GROUP_ID
                                FROM
                                    FBQUESTIONS_GROUP AS FG
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FG.FEEDBACK_ID
                                        AND FS.IS_DELETED != 1
                                WHERE
                                    FG.ACADEMIC_YEAR =?AC_YEAR
                                        AND FG.IS_DELETED != 1
                                        AND FG.FEEDBACK_ID =?FEEDBACK_ID;";
                        break;
                    }
                case FeedbackSQLCommands.DeletedFbQuestionGroupByFBQuestionGroupId:
                    {
                        query = @"UPDATE FBQUESTIONS_GROUP 
                                SET 
                                    IS_DELETED =?IS_DELETED WHERE QUESTION_GROUP_ID=?QUESTION_GROUP_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchQuestionGroupByQuestionGroupId:
                    {
                        query = @"SELECT 
                                    GROUP_TITLE, QUESTION_GROUP_ID
                                FROM
                                    FBQUESTIONS_GROUP
                                WHERE
                                    QUESTION_GROUP_ID = ?QUESTION_GROUP_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFbQuestionGroupByGroupId:
                    {
                        query = @"UPDATE 
                                FBQUESTIONS_GROUP
                                SET 
                                GROUP_TITLE=?GROUP_TITLE,
                                FEEDBACK_ID=?FEEDBACK_ID,
                                ACADEMIC_YEAR=?AC_YEAR
                                WHERE 
                                QUESTION_GROUP_ID=?QUESTION_GROUP_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbQuestionGroupByFeedbackId:
                    {
                        query = @"SELECT 
                                    QUESTION_GROUP_ID, GROUP_TITLE
                                FROM
                                    FBQUESTIONS_GROUP
                                WHERE
                                    FEEDBACK_ID =?FEEDBACK_ID AND IS_DELETED != 1
                                        AND ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFacultyPerformanceIndex:
                    {
                        query = @"   
                                SELECT 
                                FBF.FBFACULTY_PERFORMANCE_INDEX_ID,
                                FS.FEEDBACK_NAME AS FEEDBACK_SETTING,
                                FBF.ITEM,
                                FBF.MAX_MARK,
                                FBF.MARK
                            FROM
                                FBFACULTY_PERFORMANCE_INDEX AS FBF
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FBF.FEEDBACK_ID
                                    AND FS.IS_DELETED != 1
                                    AND FS.IS_ACTIVE = 1
                                    AND FS.ACADEMIC_YEAR=?AC_YEAR
                            WHERE
                                FBF.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.SaveFeedbackPerformanceIndex:
                    {
                        query = @" INSERT INTO
                                    fbfaculty_performance_index
                                    (
                                    ITEM,
                                    MARK,
                                    MAX_MARK,
                                    FEEDBACK_ID,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (
                                    ?ITEM,
                                    ?MARK,
                                    ?MAX_MARK,
                                    ?FEEDBACK_ID,
                                    ?AC_YEAR);";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedbackPerformanceIndex:
                    {
                        query = @" UPDATE
                                     FBFACULTY_PERFORMANCE_INDEX
                                     SET 
                                     ITEM=?ITEM,
                                       MARK=?MARK,
                                     MAX_MARK=?MAX_MARK,
                                     FEEDBACK_ID=?FEEDBACK_ID,
                                    ACADEMIC_YEAR=?AC_YEAR
                                     WHERE
                                    FBFACULTY_PERFORMANCE_INDEX_ID=?FBFACULTY_PERFORMANCE_INDEX_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedbackPerformanceIndex:
                    {
                        query = @"UPDATE 
                                fbfaculty_performance_index
                                SET 
                                IS_DELETED=?IS_DELETED
                                WHERE 
                                FBFACULTY_PERFORMANCE_INDEX_ID=?FBFACULTY_PERFORMANCE_INDEX_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackFacultyPerformanceIndexByFeedbackandItem:
                    {
                        query = @"SELECT
                                FBFACULTY_PERFORMANCE_INDEX_ID
                                FROM 
                                FBFACULTY_PERFORMANCE_INDEX
                                WHERE 
                                ITEM=?ITEM
                                AND FEEDBACK_ID=?FEEDBACK_ID
                                AND ACADEMIC_YEAR=?AC_YEAR
                                AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackFacultyPerformanceIndexByid:
                    {
                        query = @"SELECT 
                                    FBFACULTY_PERFORMANCE_INDEX_ID, ITEM,MARK, MAX_MARK, FEEDBACK_ID
                                FROM
                                    fbfaculty_performance_index
                                WHERE
                                    FBFACULTY_PERFORMANCE_INDEX_ID =?FBFACULTY_PERFORMANCE_INDEX_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackFacultyPerformanceIndexForEvaluationByStaffId:
                    {
                        query = @"
                                SELECT 
                                    FPI.ITEM,
                                    FPI.MARK,
                                    FPI.MAX_MARK,
                                    FPI.FBFACULTY_PERFORMANCE_INDEX_ID,
                                    FPI.FEEDBACK_ID,
                                    IF(FPE.FBFACULTY_PERFORMANCE_INDEX_REGISTER_ID IS NULL OR FBFACULTY_PERFORMANCE_INDEX_REGISTER_ID='0','0','1') AS STATUS
                                FROM
                                    FBFACULTY_PERFORMANCE_INDEX AS FPI
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FPI.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = ?STAFF_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        LEFT JOIN
                                    fbfaculty_performance_index_evaluation AS FPE ON FPE.FBFACULTY_PERFORMANCE_INDEX_ID = FPI.FBFACULTY_PERFORMANCE_INDEX_ID
                                        AND FPE.STAFF_ID = SPI.STAFF_ID
                                        AND FPE.ACADEMIC_YEAR =?AC_YEAR
                                        AND FPE.IS_DELETED != 1
                                WHERE
                                    FPI.IS_DELETED != 1
                                        AND FPI.ACADEMIC_YEAR =?AC_YEAR
                                        AND FPI.FEEDBACK_ID=?FEEDBACK_ID
                                ORDER BY FPI.FBFACULTY_PERFORMANCE_INDEX_ID; ";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackAppraisalTypeById:
                    {
                        query = @"SELECT 
                                APPRAISAL_TYPE,
                                FACULTY_SELF_APPRAISAL_TYPE_ID
                                FROM 
                                SUP_FBFACULTY_SELF_APPRAISAL_TYPE
                                WHERE
                                FACULTY_SELF_APPRAISAL_TYPE_ID=?FACULTY_SELF_APPRAISAL_TYPE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackAppraisalType:
                    {
                        query = @"UPDATE sup_fbfaculty_self_appraisal_type 
                                SET APPRAISAL_TYPE=?APPRAISAL_TYPE
                                WHERE 
                                FACULTY_SELF_APPRAISAL_TYPE_ID=?FACULTY_SELF_APPRAISAL_TYPE_ID;";
                        break;
                    }


                case FeedbackSQLCommands.SaveFeedBackAppraisalType:
                    {
                        query = @"INSERT INTO sup_fbfaculty_self_appraisal_type
                        (APPRAISAL_TYPE)
                        VALUES
                        (?APPRAISAL_TYPE);";
                        break;
                    }
                case FeedbackSQLCommands.DeletedBackAppraisalType:
                    {
                        query = @"UPDATE 
                                SUP_FBFACULTY_SELF_APPRAISAL_TYPE
                                SET 
                                IS_DELETED=?IS_DELETED
                                WHERE
                                FACULTY_SELF_APPRAISAL_TYPE_ID=?FACULTY_SELF_APPRAISAL_TYPE_ID
                                AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.ValidateFacultyAppraisalType:
                    {
                        query = @"SELECT 
                                FACULTY_SELF_APPRAISAL_TYPE_ID
                                FROM sup_fbfeedback_question_type
                                WHERE 
                                APPRAISAL_TYPE=?APPRAISAL_TYPE
                                AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFacultySelfAppraisal:
                    {
                        query = @"   
                                SELECT 
                                FBF.FBFACULTY_APPRAISAL_ID,
                                FSP.APPRAISAL_TYPE,
                                FS.FEEDBACK_NAME AS FEEDBACK_SETTING,
                                FBF.ITEM,
                                FBF.MAX_MARK,
                                 FBF.MARK
                            FROM
                                FBFACULTY_SELF_APPRAISAL AS FBF
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FBF.FEEDBACK_ID
                                    AND FS.IS_DELETED != 1
                                    AND FS.IS_ACTIVE = 1
                                    AND FS.ACADEMIC_YEAR=?AC_YEAR
                                    INNER JOIN
                                    SUP_FBFACULTY_SELF_APPRAISAL_TYPE AS FSP ON FSP.FACULTY_SELF_APPRAISAL_TYPE_ID=FBF.APPRAISAL_TYPE_ID
                                    AND FSP.IS_DELETED!=1
                                    
                            WHERE
                                FBF.IS_DELETED != 1;";
                        break;
                    }

                case FeedbackSQLCommands.SaveFeedbackFacultyAppraisal:
                    {
                        query = @" INSERT INTO
                                    FBFACULTY_SELF_APPRAISAL
                                    (
                                    APPRAISAL_TYPE_ID,
                                    ITEM,
                                    MARK,
                                    MAX_MARK,
                                    FEEDBACK_ID,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (
                                    ?APPRAISAL_TYPE_ID,
                                    ?ITEM,
                                    ?MARK,
                                    ?MAX_MARK,
                                    ?FEEDBACK_ID,
                                    ?AC_YEAR);";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedbackFacultyAppraisal:
                    {
                        query = @" UPDATE
                                     FBFACULTY_SELF_APPRAISAL
                                     SET 
                                     APPRAISAL_TYPE_ID=?APPRAISAL_TYPE_ID,
                                     ITEM=?ITEM,
                                     MARK=?MARK,
                                     MAX_MARK=?MAX_MARK,
                                     FEEDBACK_ID=?FEEDBACK_ID,
                                    ACADEMIC_YEAR=?AC_YEAR
                                     WHERE
                                    FBFACULTY_APPRAISAL_ID=?FBFACULTY_APPRAISAL_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackFacultySelfAppraisalByFeedbackandItemandAppraisalType:
                    {
                        query = @"SELECT
                                FBFACULTY_APPRAISAL_ID
                                FROM 
                                FBFACULTY_SELF_APPRAISAL
                                WHERE 
                                ITEM=?ITEM
                                AND FEEDBACK_ID=?FEEDBACK_ID
                                AND APPRAISAL_TYPE_ID=?APPRAISAL_TYPE_ID
                                AND ACADEMIC_YEAR=?AC_YEAR
                                AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackFacultySelfAppraisalByid:
                    {
                        query = @"SELECT 
                                    FBFACULTY_APPRAISAL_ID, ITEM, MARK,APPRAISAL_TYPE_ID, MAX_MARK, FEEDBACK_ID
                                FROM
                                    FBFACULTY_SELF_APPRAISAL
                                WHERE
                                    FBFACULTY_APPRAISAL_ID =?FBFACULTY_APPRAISAL_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedbackFacultyAppraisalbyId:
                    {
                        query = @"UPDATE 
                                FBFACULTY_SELF_APPRAISAL
                                SET 
                                IS_DELETED=?IS_DELETED
                                WHERE 
                                FBFACULTY_APPRAISAL_ID=?FBFACULTY_APPRAISAL_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackFacultySelfAppraisalForEvaluationByStaffId:
                    {
                        query = @"  SELECT 
                                    FPI.ITEM,
                                    FPI.MAX_MARK,
                                    FPI.MARK,
                                    FPI.FBFACULTY_APPRAISAL_ID,
                                    FPI.FEEDBACK_ID,
                                    FPI.APPRAISAL_TYPE_ID,
                                    SFS.APPRAISAL_TYPE,
                                    IF(FPE.FBFACULTY_APPRAISAL_REGISTER_ID IS NULL OR FBFACULTY_APPRAISAL_REGISTER_ID='0','0','1') AS STATUS
                                FROM
                                    fbfaculty_self_appraisal AS FPI
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FPI.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                        sup_fbfaculty_self_appraisal_type AS SFS ON SFS.FACULTY_SELF_APPRAISAL_TYPE_ID=FPI.APPRAISAL_TYPE_ID
                                        AND SFS.IS_DELETED!=1
                                        
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID =?STAFF_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        LEFT JOIN
                                    fbfaculty_self_appraisal_evaluation AS FPE ON FPE.FBFACULTY_APPRAISAL_ID = FPI.FBFACULTY_APPRAISAL_ID
                                        AND FPE.STAFF_ID = SPI.STAFF_ID
                                        AND FPE.ACADEMIC_YEAR =?AC_YEAR
                                        AND FPE.IS_DELETED != 1
                                WHERE
                                    FPI.IS_DELETED != 1
                                        AND FPI.ACADEMIC_YEAR =?AC_YEAR
                                        AND FPI.FEEDBACK_ID=?FEEDBACK_ID
                                ORDER BY FPI.FBFACULTY_APPRAISAL_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFacultySelfAppraisalEvaluationByAppraisalIdandStaffId:
                    {
                        query = @"SELECT 
                                    FBFACULTY_APPRAISAL_REGISTER_ID
                                    FROM 
                                    fbfaculty_self_appraisal_evaluation
                                    WHERE
                                    FBFACULTY_APPRAISAL_ID=?FBFACULTY_APPRAISAL_ID
                                    AND STAFF_ID=?STAFF_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFacultyPerformanceIndexEvaluationByPerformanceIndexandStaff:
                    {
                        query = @"SELECT 
                                    FBFACULTY_PERFORMANCE_INDEX_REGISTER_ID
                                    FROM 
                                    fbfaculty_performance_index_evaluation
                                    WHERE
                                    FBFACULTY_PERFORMANCE_INDEX_ID=?FBFACULTY_PERFORMANCE_INDEX_ID
                                    AND STAFF_ID=?STAFF_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFacultyAssessment:
                    {
                        query = @" SELECT 
                                FBF.FBFACULTY_ASSESSMENT_ID,
                                FS.FEEDBACK_NAME AS FEEDBACK_SETTING,
                                FBF.ITEM
                            FROM
                                fbfaculty_assessment AS FBF
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FBF.FEEDBACK_ID
                                    AND FS.IS_DELETED != 1
                                    AND FS.IS_ACTIVE = 1
                                    AND FS.ACADEMIC_YEAR=?AC_YEAR
                            WHERE
                                FBF.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedbackFacultyAssessment:
                    {
                        query = @"UPDATE
                                     fbfaculty_assessment
                                     SET 
                                     ITEM=?ITEM,
                                     FEEDBACK_ID=?FEEDBACK_ID,
                                    ACADEMIC_YEAR=?AC_YEAR
                                     WHERE
                                    FBFACULTY_ASSESSMENT_ID=?FBFACULTY_ASSESSMENT_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackFacultyAssessmentByFeedbackandItem:
                    {
                        query = @"SELECT
                                FBFACULTY_ASSESSMENT_ID
                                FROM 
                                fbfaculty_assessment
                                WHERE 
                                ITEM=?ITEM
                                AND FEEDBACK_ID=?FEEDBACK_ID
                                AND ACADEMIC_YEAR=?AC_YEAR
                                AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.SaveFeedbackFacultyAssessment:
                    {
                        query = @"INSERT INTO
                                    fbfaculty_assessment
                                    (
                                    ITEM,
                                    FEEDBACK_ID,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (
                                    ?ITEM,
                                    ?FEEDBACK_ID,
                                    ?AC_YEAR);";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedbackFacultyAssessment:
                    {
                        query = @"UPDATE 
                                fbfaculty_assessment
                                SET 
                                IS_DELETED=?IS_DELETED
                                WHERE 
                                FBFACULTY_ASSESSMENT_ID=?FBFACULTY_ASSESSMENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackFacultyAssessmentByid:
                    {
                        query = @"SELECT 
                                    FBFACULTY_ASSESSMENT_ID, ITEM, FEEDBACK_ID
                                FROM
                                    fbfaculty_assessment
                                WHERE
                                    FBFACULTY_ASSESSMENT_ID =?FBFACULTY_ASSESSMENT_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackFacultyAssessmentForEvaluationByStaffId:
                    {
                        query = @"
                                    SELECT 
                                    FPI.ITEM,
                                    FPI.FBFACULTY_ASSESSMENT_ID,
                                    FPI.FEEDBACK_ID,
                                    IF(FPE.FBFACULTY_ASSESSMENT_REGISTER_ID IS NULL OR FBFACULTY_ASSESSMENT_REGISTER_ID='0','0','1') AS STATUS
                                FROM
                                    fbfaculty_assessment AS FPI
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FPI.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = ?ASSESSE
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        LEFT JOIN
                                    fbfaculty_assessment_evaluation AS FPE ON FPE.FBFACULTY_ASSESSMENT_ID = FPI.FBFACULTY_ASSESSMENT_ID
                                        AND FPE.ASSESSE = SPI.STAFF_ID
                                        AND FPE.ACADEMIC_YEAR =?AC_YEAR
                                        AND FPE.ASSESSOR=?ASSESSOR
                                        AND FPE.IS_DELETED != 1
                                WHERE
                                    FPI.IS_DELETED != 1
                                        AND FPI.ACADEMIC_YEAR =?AC_YEAR
                                        AND FPI.FEEDBACK_ID=?FEEDBACK_ID
                                ORDER BY FPI.FBFACULTY_ASSESSMENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFacultyAssessmentEvaluationByAssessmentIdandStaffId:
                    {
                        query = @"SELECT 
                                    FBFACULTY_ASSESSMENT_REGISTER_ID
                                    FROM 
                                    fbfaculty_assessment_evaluation
                                    WHERE
                                    FBFACULTY_ASSESSMENT_ID=?FBFACULTY_ASSESSMENT_ID
                                    AND ASSESSE=?ASSESSE
                                    AND ASSESSOR=?ASSESSOR
                                    AND IS_DELETED!=1;";
                        break;
                    }


                case FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandStaff:
                    {
                        query = @"SELECT 
 
                                    STF.STAFF_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            '',
                                            IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(FS.FEEDBACK_NAME, ' -', FS.ACADEMIC_YEAR) AS FEEDBACK_SETTING,
                                    SUM(IF(FBO.RATING!=0 
                                            AND FRS.SUP_FEEDBACK_ROLE_ID = 4,
                                        FBO.RATING,
                                        0)) AS STUDENT_APPRAISAL,
                                    SUM(IF(FBO.RATING!=0
                                            AND FRS.SUP_FEEDBACK_ROLE_ID = 9,
                                        FBO.RATING,
                                        0)) AS BY_HOD,
                                    SUM(IF(FBO.RATING!=0
                                            AND FRS.SUP_FEEDBACK_ROLE_ID = 3,
                                        FBO.RATING,
                                        0)) AS BY_PRINCIPAL,
                                    SUM(IF(FBO.RATING!=0
                                            AND FRS.SUP_FEEDBACK_ROLE_ID = 8,
                                        FBO.RATING,
                                        0)) AS BY_DEAN,
                                        SUM(IF(FBO.RATING!=0
                                            AND FRS.SUP_FEEDBACK_ROLE_ID = 10,
                                        FBO.RATING,
                                        0)) AS BY_PEER
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.ASSESSEE
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
                                    FBOBJECTIVES AS FBO ON FBO.OBJECTIVE_ID = FE.ANSWER
                                        AND FBO.IS_DELETED != 1
                                        INNER JOIN
                                        FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID=FE.FEEDBACK_ROLE_ID
                                        AND FRS.IS_DELETED!=1
                                        
                                        INNER JOIN
                                        SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID=FRS.SUP_FEEDBACK_ROLE_ID
                                        AND FR.IS_DELETED!=1
                                WHERE
                                    FE.ASSESSEE =?ASSESSEE AND FE.FEEDBACK_ID=?FEEDBACK_ID
                                        AND FE.ISDELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackPerformanceIndexEvaluationForRepoprtByFeedbackandStaff:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            '',
                                            IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(FS.FEEDBACK_NAME, '', FS.ACADEMIC_YEAR) AS FEEDBACK_SETTING,
                                    SUM(IF(FE.SCORE!=0,
                                        FE.SCORE,0)) AS PERFORMANCE_INDEX
                                FROM
    
                                    fbfaculty_performance_index_evaluation AS FE
                                       INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                     STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.STAFF_ID
                                     AND STF.IS_DELETED!=1
                                    AND STF.IS_LEFT!=1
     
                                   
                                WHERE
                                    FE.STAFF_ID =?ASSESSEE
                                        AND FE.FEEDBACK_ID=?FEEDBACK_ID
										AND FE.ACADEMIC_YEAR=?AC_YEAR
                                        AND FE.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackSelfAppraisalEvaluationForRepoprtByFeedbackandStaff:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            '',
                                            IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(FS.FEEDBACK_NAME, '', FS.ACADEMIC_YEAR) AS FEEDBACK_SETTING,
                                    SUM(IF(FE.SCORE IS NOT NULL AND NOT 0,
                                        FE.SCORE,0)) AS SELF_APRAISAL
                                FROM
    
                                    fbfaculty_self_appraisal_evaluation AS FE
                                       INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                     STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.STAFF_ID
                                      AND STF.IS_DELETED!=1
                                      AND STF.IS_LEFT!=1

                                WHERE
                                    FE.STAFF_ID =?ASSESSEE
                                        AND FE.FEEDBACK_ID =?FEEDBACK_ID
                                        AND FE.ACADEMIC_YEAR=?AC_YEAR
                                        AND FE.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandPrincipal:
                    {
                        query = @"SELECT 
 
                                    STF.STAFF_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            '',
                                            IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(FS.FEEDBACK_NAME, '', FS.ACADEMIC_YEAR) AS FEEDBACK_SETTING,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 5,
                                        FBO.RATING,
                                        0)) AS STUDENT_APPRAISAL,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 7,
                                        FBO.RATING,
                                        0)) AS BY_HOD,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 4,
                                        FBO.RATING,
                                        0)) AS BY_FACULTY,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 6,
                                        FBO.RATING,
                                        0)) AS BY_DEAN
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.ASSESSEE
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
                                    FBOBJECTIVES AS FBO ON FBO.OBJECTIVE_ID = FE.ANSWER
                                        AND FBO.IS_DELETED != 1
                                WHERE
                                    FE.ASSESSEE =?ASSESSEE AND FE.FEEDBACK_ID =?FEEDBACK_ID
                                        AND FE.ISDELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackEntryStudents:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID, 
                                        SPI.ROLL_NO,
                                        SPI.FIRST_NAME,
                                        CONCAT(
                                            IFNULL(CLS.CLASS_CODE, ''),
                                            ' ',
                                            IFNULL(CONCAT('(', SPM.PROGRAMME_MODE, ') - '), ''),
                                            IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')
                                        ) AS CLASS_NAME 
                                    FROM 
                                        STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                            AND CPG.PROGRAMME_MODE = CLS.PROGRAMME_MODE
                                            AND CPG.SHIFT = CLS.SHIFT
                                        LEFT JOIN CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        LEFT JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                        LEFT JOIN SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID  AND SPI.IS_DELETED != 1 and SPI.IS_LEFT !=1
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CLS.IS_DELETED != 1
                                        AND SC.STUDENT_ID IN (
                                            SELECT ASSESSOR 
                                            FROM fbevaluation?AC_YEAR
                                            WHERE FEEDBACK_ID = ?FEEDBACK_ID
                                            AND ASSESSOR = ?STUDENT_ID
                                            AND COURSE_ID = ?COURSE_ID
                                        )
                                    GROUP BY 
                                        SPI.STUDENT_ID
                                    ORDER BY 
                                        SPI.STUDENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackNotEntryStudents:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID, 
                                        SPI.ROLL_NO,
                                        SPI.FIRST_NAME,
                                        CONCAT(
                                            IFNULL(CLS.CLASS_CODE, ''),
                                            ' ',
                                            IFNULL(CONCAT('(', SPM.PROGRAMME_MODE, ') - '), ''),
                                            IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')
                                        ) AS CLASS_NAME 
                                    FROM 
                                        STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID and SPI.IS_DELETED != 1 AND SPI.IS_LEFT != 1
                                        INNER JOIN CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                            AND CPG.PROGRAMME_MODE = CLS.PROGRAMME_MODE
                                            AND CPG.SHIFT = CLS.SHIFT
                                        LEFT JOIN CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        LEFT JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                        LEFT JOIN SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CLS.IS_DELETED != 1
                                        AND SC.STUDENT_ID NOT IN (
                                            SELECT ASSESSOR 
                                            FROM fbevaluation?AC_YEAR
                                            WHERE FEEDBACK_ID = ?FEEDBACK_ID
                                        )
                                    GROUP BY 
                                        SPI.STUDENT_ID
                                    ORDER BY 
                                        SPI.STUDENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackNotEntryStudents1:
                    {
                        query = @"SELECT 
                                        SPI.STUDENT_ID, 
                                        SPI.ROLL_NO,
                                        SPI.FIRST_NAME,
                                        CONCAT(
                                            IFNULL(CLS.CLASS_CODE, ''),
                                            ' ',
                                            IFNULL(CONCAT('(', SPM.PROGRAMME_MODE, ') - '), ''),
                                            IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')
                                        ) AS CLASS_NAME 
                                    FROM 
                                        STU_CLASS AS SC
                                        INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                        LEFT JOIN CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                            AND CPG.PROGRAMME_MODE = CLS.PROGRAMME_MODE
                                            AND CPG.SHIFT = CLS.SHIFT
                                        LEFT JOIN CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        LEFT JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                        LEFT JOIN SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID AND SPI.IS_DELETED != 1 and SPI.IS_LEFT !=1
                                        AND SC.IS_DELETED != 1
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CLS.IS_DELETED != 1
                                        AND SC.STUDENT_ID NOT IN (
                                            SELECT ASSESSOR 
                                            FROM fbevaluation?AC_YEAR
                                            WHERE FEEDBACK_ID = ?FEEDBACK_ID
                                            AND COURSE_ID = ?COURSE_ID
                                        )
                                    GROUP BY 
                                        SPI.STUDENT_ID
                                    ORDER BY 
                                        SPI.STUDENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackClassWiseStudentsList:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                ' - ',
                                                IFNULL(SP.ROLL_NO, '')) AS FIRST_NAME
   
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = 2023
                                            AND SC.CLASS_ID = ?CLASS_ID
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandDean:
                    {
                        query = @"SELECT 
 
                                    STF.STAFF_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            '',
                                            IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(FS.FEEDBACK_NAME, '', FS.ACADEMIC_YEAR) AS FEEDBACK_SETTING,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 5,
                                        FBO.RATING,
                                        0)) AS STUDENT_APPRAISAL,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 7,
                                        FBO.RATING,
                                        0)) AS BY_HOD,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 3,
                                        FBO.RATING,
                                        0)) AS BY_PRINCIPAL,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 4,
                                        FBO.RATING,
                                        0)) AS BY_FACULTY
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.ASSESSEE
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
                                    FBOBJECTIVES AS FBO ON FBO.OBJECTIVE_ID = FE.ANSWER
                                        AND FBO.IS_DELETED != 1
                                WHERE
                                    FE.ASSESSEE =?ASSESSEE AND FE.FEEDBACK_ID =?FEEDBACK_ID
                                        AND FE.ISDELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackEvaluationForRepoprtByFeedbackandHod:
                    {
                        query = @"SELECT 
 
                                    STF.STAFF_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            '',
                                            IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CONCAT(FS.FEEDBACK_NAME, '', FS.ACADEMIC_YEAR) AS FEEDBACK_SETTING,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 5,
                                        FBO.RATING,
                                        0)) AS STUDENT_APPRAISAL,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 4,
                                        FBO.RATING,
                                        0)) AS BY_FACULTY,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 3,
                                        FBO.RATING,
                                        0)) AS BY_PRINCIPAL,
                                    SUM(IF(FBO.RATING IS NOT NULL AND NOT 0
                                            AND FE.ROLE = 6,
                                        FBO.RATING,
                                        0)) AS BY_DEAN
                                FROM
                                    FBEVALUATION?AC_YEAR AS FE
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = FE.ASSESSEE
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
                                    FBOBJECTIVES AS FBO ON FBO.OBJECTIVE_ID = FE.ANSWER
                                        AND FBO.IS_DELETED != 1
                                WHERE
                                    FE.ASSESSEE =?ASSESSEE AND FE.FEEDBACK_ID =?FEEDBACK_ID
                                        AND FE.ISDELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStaffByPrincipalRole:
                    {
                        query = @"SELECT 
                                        STF.STAFF_ID,
                                        CONCAT('( ',
                                                IFNULL(STF.STAFF_CODE, ''),
                                                ' )  ',
                                                IFNULL(STF.FIRST_NAME, ''),
                                                '',
                                                IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        USER_ROLES_BY_ACADEMIC_YEAR AS UR ON UR.USER_ID = SP.STAFF_ID
                                            AND UR.ACADEMIC_YEAR = ?AC_YEAR
                                            AND UR.ROLE_ID = 3
                                            AND UR.IS_ACTIVE = 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.DEPARTMENT = SP.DEPARTMENT
                                            AND STF.NON_TEACHING_CATEGORY != 1
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                    WHERE
                                         SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                    ORDER BY STF.STAFF_CODE;";
                        break;
                    }

                case FeedbackSQLCommands.FetchStaffByHodRole:
                    {
                        query = @"        SELECT 
                                         CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,STF.STAFF_ID
                                    FROM
                                        STF_PERSONAL_INFO AS STF
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                          INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.DEPARTMENT = (
                                        SELECT DEPARTMENT FROM STF_PERSONAL_INFO WHERE STAFF_ID=?STAFF_ID AND IS_DELETED!=1
                                        )
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                            AND CPG.IS_DELETED != 1
                                    WHERE
                                         STF.IS_DELETED!=1
                                        AND STF.IS_LEFT!=1
                                        AND UA.ROLE=7
                                        AND STF.DEPARTMENT=CP.DEPARTMENT
                                        GROUP BY STF.STAFF_ID;";
                        break;
                    }

                case FeedbackSQLCommands.FetchStaffByStaffRole:
                    {
                        query = @"SELECT 
                                         CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,STF.STAFF_ID
                                    FROM
                                        STF_PERSONAL_INFO AS STF
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                          INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.DEPARTMENT = (
                                        SELECT DEPARTMENT FROM STF_PERSONAL_INFO WHERE STAFF_ID=?STAFF_ID AND IS_DELETED!=1
                                        )
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                            AND CPG.IS_DELETED != 1
                                            
                                             INNER JOIN
                                    user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                  AND AY.ACADEMIC_YEAR=?AC_YEAR
                                   AND AY.USER_TYPE='4'
                                   AND AY.ROLE_ID=4
                                  AND AY.IS_DELETED != 1
                                    WHERE
                                          STF.STAFF_ID !=?STAFF_ID

                                        AND  STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        AND STF.DEPARTMENT = CP.DEPARTMENT
                                        GROUP BY AY.USER_ID;";
                        break;
                    }

                case FeedbackSQLCommands.FetchStaffByDeanRole:
                    {
                        query = @"      SELECT 
                                         CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,STF.STAFF_ID
                                    FROM
                                        STF_PERSONAL_INFO AS STF
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                            AND UA.IS_DELETED != 1
                                          INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.DEPARTMENT = (
                                        SELECT DEPARTMENT FROM STF_PERSONAL_INFO WHERE STAFF_ID=?STAFF_ID AND IS_DELETED!=1
                                        )
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                            AND CPG.IS_DELETED != 1
                                    WHERE
                                         STF.IS_DELETED!=1
                                        AND STF.IS_LEFT!=1
                                        AND UA.ROLE=6
                                        AND STF.DEPARTMENT=CP.DEPARTMENT
                                        GROUP BY STF.STAFF_ID;";
                        break;
                    }

                case FeedbackSQLCommands.FetchStaffByProgrammeandStaffId:
                    {
                        query = @"SELECT 
                                         CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,STF.STAFF_ID
                                    FROM
                                        STF_PERSONAL_INFO AS STF
                                            INNER JOIN
                                        USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                           -- AND UA.USER_ID=1
                                            AND UA.IS_DELETED != 1
                                          INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.DEPARTMENT = (SELECT DEPARTMENT FROM STF_PERSONAL_INFO WHERE
                                        STAFF_ID=?STAFF_ID AND IS_DELETED!=1)
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                            AND CPG.IS_DELETED != 1
                                    WHERE
                                         STF.STAFF_ID!=?STAFF_ID
                                        AND STF.IS_DELETED!=1
                                        AND STF.IS_LEFT!=1
                                        AND UA.ROLE=4
                                        GROUP BY STF.STAFF_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchQuestionCountByFeedbackAndSupFeedbackRole:
                    {
                        query = @"SELECT 
                                    SUM(IF(FQ.QUESTION_ID IS NOT NULL
                                            AND FQ.QUESTION_ID != 0,
                                        1,
                                        0)) AS QUESTION_COUNT
                                FROM
                                    fbfeedback_questions AS fq
                                        INNER JOIN
                                    fbfeedback_role_settings AS FRS ON FRS.FEEDBACK_ID =fq.FEEDBACK_ID
                                        AND FRS.FEEDBACK_ROLE_ID=fq.FEEDBACK_ROLE_ID
                                        INNER JOIN
                                        sup_fbfeedback_role AS SR ON SR.SUP_FEEDBACK_ROLE_ID=FRS.SUP_FEEDBACK_ROLE_ID
                                        
                                        
                                WHERE
                                   FQ.FEEDBACK_ID=?FEEDBACK_ID
                                        AND SR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID;";
                        break;
                    }

                case FeedbackSQLCommands.FetchTotalUserCountOfEvaluationBySupFeedbackRoleId:
                    {
                        query = @"SELECT 
                                    COUNT(T.ASSESSOR) AS TOTAL
                                FROM
                                    (SELECT 
                                        FE.ASSESSOR
                                    FROM
                                        FBEVALUATION?AC_YEAR AS FE
                                    INNER JOIN FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FE.FEEDBACK_ID
                                        AND FS.IS_ACTIVE = 1
                                        AND FS.IS_DELETED != 1
                                    INNER JOIN FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FE.FEEDBACK_ROLE_ID
                                        AND FRS.IS_DELETED != 1
                                    INNER JOIN SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                        AND FR.IS_DELETED != 1
                                    WHERE
                                        FR.SUP_FEEDBACK_ROLE_ID =?SUP_FEEDBACK_ROLE_ID
                                            AND FE.ASSESSEE =?ASSESSE 
                                            AND FE.FEEDBACK_ID =?FEEDBACK_ID
                                            AND FE.ISDELETED != 1
                                    GROUP BY FE.ASSESSOR,FE.COURSE_ID) AS T;";
                        break;
                    }


                case FeedbackSQLCommands.FetchStaffByStaffId:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,STF.STAFF_ID
                                FROM
                                    STF_PERSONAL_INFO AS STF
                                        INNER JOIN
                                    USER_ACCOUNT AS UA ON UA.USER_ID = STF.STAFF_ID
                                        AND UA.IS_DELETED != 1
									  INNER JOIN
                                    user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                  AND AY.ACADEMIC_YEAR=?AC_YEAR
                                   AND AY.USER_TYPE='4'
                                     AND AY.ROLE_ID = 4
                                  AND AY.IS_DELETED != 1
                                WHERE
                                    STF.STAFF_ID =?STAFF_ID
                                        AND STF.IS_DELETED != 1 GROUP BY AY.USER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchQuestionsByFeedBackId:
                    {
                        query = @"SELECT 
                                        QUESTION_ID,
                                        QUESTION
                                    FROM
                                        FBFEEDBACK_QUESTIONS
                                    WHERE
                                        FEEDBACK_ID = ?FEEDBACK_ID AND IS_DELETED != 1
                                            AND ACADEMIC_YEAR = ?AC_YEAR  order by cast(ORDERED_BY as unsigned);";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCountByFBQuestion:
                    {
                        query = @"SELECT 
                                        QUESTION_ID, 
                                        SUM(ANSWER) AS TOTAL, 
                                        SUM(NO_OF_STUDENTS) AS NO_OF_STUDENT
                                    FROM
                                        (SELECT 
                                                QUESTION AS QUESTION_ID,
                                                ANSWER AS ANSWER_ID,
                                                SUM(
										IF(ANSWER = '1', '5',
										IF(ANSWER = '2', '4', 
										IF(ANSWER = '3', '3',
										IF(ANSWER = '4', '2',
										IF(ANSWER = '5', '1',
										IF(ANSWER = '6', '5',
										IF(ANSWER = '7', '4',
                                        IF(ANSWER = '8', '3',
                                        IF(ANSWER = '9', '2',
                                        IF(ANSWER = '10', '1',
                                        IF(ANSWER = '11', '4',
                                          IF(ANSWER = '12', '5',
                                            IF(ANSWER = '13', '4',
                                              IF(ANSWER = '14', '3',
                                                IF(ANSWER = '15', '2',
                                                  IF(ANSWER = '16', '1',
                                                    IF(ANSWER = '17', '5',
                                                      IF(ANSWER = '18', '4',
                                                        IF(ANSWER = '19', '3',
                                                          IF(ANSWER = '20', '2',
                                                            IF(ANSWER = '21', '1',											
										'0')))))))))))))))))))))) AS ANSWER,
                                                   COUNT(ASSESSOR) AS NO_OF_STUDENTS
                                            FROM
                                                FBEVALUATION?AC_YEAR AS FB
                                    WHERE
                                        FB.CLASS_ID = ?CLASS_ID AND FB.COURSE_ID = ?COURSE_ID
                                            AND FB.FEEDBACK_ID = ?FEEDBACK_ID
                                            AND FB.ASSESSEE=?ASSESSEE
                                            AND FB.ISDELETED != 1
                                    GROUP BY QUESTION , ANSWER) AS T
                                    GROUP BY T.QUESTION_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCountByFBQuestion1:
                    {
                        query = @"SELECT 
                                        QUESTION_ID, 
                                        SUM(ANSWER) AS TOTAL, 
                                        SUM(NO_OF_STUDENTS) AS NO_OF_STUDENT,
                                        COURSE_ID,
                                        CLASS_ID
                                    FROM
                                        (SELECT 
                                                QUESTION AS QUESTION_ID,
                                                ANSWER AS ANSWER_ID,
                                                SUM(
										IF(ANSWER = '1', '5',
										IF(ANSWER = '2', '4', 
										IF(ANSWER = '3', '3',
										IF(ANSWER = '4', '2',
										IF(ANSWER = '5', '1',
										IF(ANSWER = '6', '5',
										IF(ANSWER = '7', '4',
                                        IF(ANSWER = '8', '3',
                                        IF(ANSWER = '9', '2',
                                        IF(ANSWER = '10', '1',
                                        IF(ANSWER = '11', '4',
                                          IF(ANSWER = '12', '5',
                                            IF(ANSWER = '13', '4',
                                              IF(ANSWER = '14', '3',
                                                IF(ANSWER = '15', '2',
                                                  IF(ANSWER = '16', '1',
                                                    IF(ANSWER = '17', '5',
                                                      IF(ANSWER = '18', '4',
                                                        IF(ANSWER = '19', '3',
                                                          IF(ANSWER = '20', '2',
                                                            IF(ANSWER = '21', '1',											
										'0')))))))))))))))))))))) AS ANSWER,
                                                   COUNT(ASSESSOR) AS NO_OF_STUDENTS,
                                                    COURSE_ID,
                                        CLASS_ID
                                            FROM
                                                FBEVALUATION?AC_YEAR AS FB
                                    WHERE
                                        -- FB.CLASS_ID = ?CLASS_ID AND FB.COURSE_ID = ?COURSE_ID
                                            FB.FEEDBACK_ID = ?FEEDBACK_ID
                                            AND FB.ASSESSEE=?ASSESSEE
                                            AND FB.ISDELETED != 1
                                    GROUP BY QUESTION , ANSWER,COURSE_ID,CLASS_ID) AS T
                                    GROUP BY T.QUESTION_ID,T.COURSE_ID,t.CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCountByFBQuestion1RatingBased:
                    {
                        query = @"SELECT 
                                        QUESTION_ID,
                                        SUM(ANSWER) AS TOTAL,
                                        SUM(NO_OF_STUDENTS) AS NO_OF_STUDENT,
                                        COURSE_ID,
                                        CLASS_ID
                                    FROM
                                        (SELECT 
                                            QUESTION AS QUESTION_ID,
                                                ANSWER AS ANSWER_ID,
                                                SUM(FO.RATING) AS ANSWER,
                                                COUNT(ASSESSOR) AS NO_OF_STUDENTS,
                                                COURSE_ID,
                                                CLASS_ID
                                        FROM
                                            FBEVALUATION?AC_YEAR AS FB
			                                    INNER JOIN 
		                                    fbobjectives AS FO ON FB.ANSWER = FO.OBJECTIVE_ID
                                        WHERE
                                         FB.CLASS_ID = ?CLASS_ID AND
                                            FB.COURSE_ID = ?COURSE_ID
                                        AND    FB.FEEDBACK_ID = ?FEEDBACK_ID
                                            AND FB.ASSESSEE=?ASSESSEE
                                            AND FB.ISDELETED != 1
                                    GROUP BY QUESTION , ANSWER,COURSE_ID,CLASS_ID) AS T
                                    GROUP BY T.QUESTION_ID,T.COURSE_ID,t.CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCountByFBQuestion1RatingBasedOverAll:
                    {
                        query = @"SELECT 
                                        QUESTION_ID,
                                        SUM(ANSWER) AS TOTAL,
                                        SUM(NO_OF_STUDENTS) AS NO_OF_STUDENT,
                                        COURSE_ID,
                                        CLASS_ID
                                    FROM
                                        (SELECT 
                                            QUESTION AS QUESTION_ID,
                                                ANSWER AS ANSWER_ID,
                                                SUM(FO.RATING) AS ANSWER,
                                                COUNT(ASSESSOR) AS NO_OF_STUDENTS,
                                                COURSE_ID,
                                                CLASS_ID
                                        FROM
                                            FBEVALUATION?AC_YEAR AS FB
			                                    INNER JOIN 
		                                    fbobjectives AS FO ON FB.ANSWER = FO.OBJECTIVE_ID
                                        WHERE
                                        --  FB.CLASS_ID = ?CLASS_ID AND
                                            -- FB.COURSE_ID = ?COURSE_ID   AND
                                            FB.FEEDBACK_ID = ?FEEDBACK_ID  
                                            AND FB.ASSESSEE=?ASSESSEE
                                            AND FB.ISDELETED != 1
                                    GROUP BY QUESTION , ANSWER,COURSE_ID,CLASS_ID) AS T
                                    GROUP BY T.QUESTION_ID,T.COURSE_ID,t.CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCountByClassAndCourseId:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT
                                    FROM
                                        STU_CLASS AS SC
                                    WHERE
                                        SC.CLASS_ID = ?CLASS_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1 
                                    UNION SELECT 
                                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT
                                    FROM
                                        STU_COURSE_INFO AS SC
                                    WHERE
                                        SC.S_CLASS_ID = ?CLASS_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.COURSE_ID = ?COURSE_ID
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchfeedbackroleId:
                    {
                        query = @"SELECT 
                                FEEDBACK_ROLE_ID
                            FROM
                                FBFEEDBACK_ROLE_SETTINGS
                            WHERE
                                FEEDBACK_ID = ?FEEDBACK_ID
                                    AND SUP_FEEDBACK_ROLE_ID = ?SUP_FEEDBACK_ROLE_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchQuestionsByFeedBackIdandRoleId:
                    {
                        query = @"SELECT 
                                QUESTION_ID, QUESTION
                            FROM
                                FBFEEDBACK_QUESTIONS
                            WHERE
                                FEEDBACK_ID = ?FEEDBACK_ID
                                    AND FEEDBACK_ROLE_ID = ?FEEDBACK_ROLE_ID
                                    AND IS_DELETED != 1
                                    AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbClassesByClassIdAndFeedbackId:
                    {
                        query = @"SELECT * FROM FBCLASSES WHERE FEEDBACK_ID=?FEEDBACK_ID AND CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbEvaluationInfoForReportByClasswise:
                    {
                        query = @"SELECT 
                                        FQ.QUESTION_ID,
                                        FQ.QUESTION,
                                        FB.ANSWER,
                                        SC.STUDENT_ID
                                FROM
                                    STU_CLASS AS SC
                                INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                INNER JOIN FBEVALUATION?AC_YEAR AS FB ON FB.ASSESSOR = SPI.STUDENT_ID
                                    AND FB.ISDELETED != 1
                                    AND FB.FEEDBACK_ID =?FEEDBACK_ID
                                INNER JOIN FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FB.QUESTION
                                    AND FQ.IS_DELETED != 1
                                
                                INNER JOIN FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FB.ANSWER
                                    AND O.IS_DELETED != 1
                                WHERE
                                    SC.CLASS_ID IN (?CLASS_ID)
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1 GROUP BY FQ.QUESTION_ID,FB.ASSESSOR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbEvaluationInfoForReportByClasswiseandAcYear:
                    {
                        query = @"SELECT 
                                        FQ.QUESTION_ID,
                                        FQ.QUESTION,
                                        FB.ANSWER,
                                        SC.STUDENT_ID
                                FROM
                                    STU_CLASS AS SC
                                INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                INNER JOIN FBEVALUATION?AC_YEAR AS FB ON FB.ASSESSOR = SPI.STUDENT_ID
                                    AND FB.ISDELETED != 1
                                    AND FB.FEEDBACK_ID =?FEEDBACK_ID
                                INNER JOIN FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FB.QUESTION
                                    AND FQ.IS_DELETED != 1
                                
                                INNER JOIN FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FB.ANSWER
                                    AND O.IS_DELETED != 1
                                WHERE
                                    SC.CLASS_ID IN (?CLASS_ID)
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1 GROUP BY FQ.QUESTION_ID,FB.ASSESSOR;";
                        break;
                    }

                case FeedbackSQLCommands.FetchFbEvaluationInfoForReportByStaffwiseandAcYear:
                    {
                        query = @"    SELECT 
                                        FQ.QUESTION_ID,
                                        FQ.QUESTION,
                                        FB.ANSWER,
                                        SC.STUDENT_ID,
                                        SP.STAFF_CODE,
                                        SP.FIRST_NAME,
                                        CC.CLASS_NAME,
                                        CR.COURSE_TITLE,
                                        FB.COURSE_ID,
                                        FB.CLASS_ID
                                FROM
                                    STU_CLASS AS SC
                                INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                INNER JOIN FBEVALUATION?AC_YEAR AS FB ON FB.ASSESSOR = SPI.STUDENT_ID
                                    AND FB.ISDELETED != 1
                                    AND FB.FEEDBACK_ID =?FEEDBACK_ID
                                    INNER JOIN
                                    stf_personal_info AS SP ON SP.STAFF_ID=FB.ASSESSEE
                                INNER JOIN FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FB.QUESTION
                                    AND FQ.IS_DELETED != 1						
                                INNER JOIN FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FB.ANSWER
                                    AND O.IS_DELETED != 1
                                   INNER JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = FB.COURSE_ID
                                WHERE
                                       FB.ASSESSEE =?ASSESSEE
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1 GROUP BY FQ.QUESTION_ID,FB.ASSESSOR,FB.COURSE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbFeedbackQuestionsMappingByFeedbackId:
                    {
                        query = @"SELECT 
                                     M.FEEDBACK_ID, M.QUESTION_ID, M.OBJECTIVE_ID,O.OBJECTIVES,O.RATING,O.OBJECTIVE_ORDER
                                FROM
                                    FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS M
                                        INNER JOIN
                                    FBOBJECTIVES AS O ON O.OBJECTIVE_ID = M.OBJECTIVE_ID
                                        AND O.IS_DELETED != 1
                                WHERE
                                    M.FEEDBACK_ID =?FEEDBACK_ID AND M.IS_DELETED != 1;";
                        break;
                    }

                #region External User Add / Edit
                case FeedbackSQLCommands.FetchExternalUserInfo:
                    {
                        query = @"SELECT 
                                        EXTERNAL_USER_ID,
                                        EXTERNAL_USER_CODE,
                                        FIRST_NAME,
                                        MAIL_ID,
                                        MOBILE_NO,
                                        COLLEGE_NAME,
                                        SD.DESIGNATION,
                                        D.DEPARTMENT,
                                        R.ROLE_NAME AS ROLE,
                                        UT.USER_TYPE_NAME AS  USER_TYPE
                                    FROM
                                        EXTERNAL_USERS AS EU
		                                    INNER JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=EU.DEPARTMENT
		                                    INNER JOIN
	                                    SUP_DESIGNATION AS SD ON SD.DESIGNATIONID=EU.DESIGNATION
		                                    INNER JOIN
	                                    SUP_ROLE AS R ON R.ROLE_ID=EU.ROLE
		                                    INNER JOIN
	                                    SUP_USER_TYPE AS UT ON UT.USER_TYPE_ID=EU.USER_TYPE
                                    WHERE
                                        EU.IS_DELETED!=1 AND EU.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchExternalUserById:
                    {
                        query = @"SELECT 
                                        EXTERNAL_USER_ID,
                                        EXTERNAL_USER_CODE,
                                        FIRST_NAME,
                                        MAIL_ID,
                                        MOBILE_NO,
                                        COLLEGE_NAME,
                                        DESIGNATION,
                                        DEPARTMENT,
                                        ROLE,
                                        USER_TYPE
                                    FROM
                                        EXTERNAL_USERS
                                    WHERE
                                        EXTERNAL_USER_ID = ?EXTERNAL_USER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.InsertExternalUserInfo:
                    {
                        query = @"INSERT INTO EXTERNAL_USERS
                                 (
	                                 EXTERNAL_USER_CODE,
	                                 FIRST_NAME,
	                                 MAIL_ID,
	                                 MOBILE_NO,
	                                 COLLEGE_NAME,
	                                 DESIGNATION,
	                                 DEPARTMENT,
	                                 ACADEMIC_YEAR,
	                                 ROLE,
	                                 USER_TYPE
                                 )
                                 VALUES
                                 (
                                     ?EXTERNAL_USER_CODE,
	                                 ?FIRST_NAME,
	                                 ?MAIL_ID,
	                                 ?MOBILE_NO,
	                                 ?COLLEGE_NAME,
	                                 ?DESIGNATION,
	                                 ?DEPARTMENT,
	                                 ?AC_YEAR,
	                                 ?ROLE,
	                                 ?USER_TYPE
                                 );";
                        break;
                    }
                case FeedbackSQLCommands.UpdateExternalUserInfo:
                    {
                        query = @"UPDATE EXTERNAL_USERS
                                  SET
	                                  EXTERNAL_USER_CODE = ?EXTERNAL_USER_CODE,
	                                  FIRST_NAME = ?FIRST_NAME,
	                                  MAIL_ID = ?MAIL_ID,
	                                  MOBILE_NO = ?MOBILE_NO,
	                                  COLLEGE_NAME = ?COLLEGE_NAME,
	                                  DESIGNATION = ?DESIGNATION,
	                                  DEPARTMENT = ?DEPARTMENT,
	                                  ROLE = ?ROLE,
	                                  USER_TYPE = ?USER_TYPE
                                  WHERE EXTERNAL_USER_ID = ?EXTERNAL_USER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteExternalUserInfo:
                    {
                        query = @"UPDATE EXTERNAL_USERS SET IS_DELETED=1 WHERE EXTERNAL_USER_ID=?EXTERNAL_USER_ID;";
                        break;
                    }
                #endregion



                #region External User Course Mapping
                case FeedbackSQLCommands.FetchCourseMappingInfo:
                    {
                        query = @"SELECT 
                                        COURSE_MAPPING_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        DATE_FORMAT(EU.DATE_FROM,'%d/%m/%Y') as DATE_FROM,
                                        DATE_FORMAT(EU.DATE_TO,'%d/%m/%Y')as DATE_TO,
                                        ENTRY_ID,
                                        IS_FINISHED,
                                        U.EXTERNAL_USER_CODE,
                                        U.FIRST_NAME,
                                        DATE_FORMAT(U.DOB,'%d/%m/%Y') AS DOB,
                                        U.MOBILE_NO,
                                        U.COLLEGE_NAME,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS EU
		                                    INNER JOIN
	                                    EXTERNAL_USERS AS U ON U.EXTERNAL_USER_ID=EU.STAFF_ID
		                                    AND U.IS_DELETED!=1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=EU.COURSE_ID
                                            AND CCR.IS_DELETED!=1
                                            INNER JOIN
                                        FBFEEDBACK_SETTINGS AS FB ON FB.FEEDBACK_ID=EU.FEEDBACK_SETTING_ID
                                            AND FB.ACADEMIC_YEAR=EU.ACADEMIC_YEAR
                                            AND FB.IS_ACTIVE=1
                                            AND FB.IS_DELETED!=1
                                    WHERE
                                        EU.IS_DELETED!=1 
                                        AND EU.ENTRY_ID=?ENTRY_ID 
                                        AND EU.ACADEMIC_YEAR=?AC_YEAR ORDER BY U.EXTERNAL_USER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchCourseMappingInfoById:
                    {
                        query = @"SELECT 
                                        COURSE_MAPPING_ID,
                                        FEEDBACK_SETTING_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        DATE_FORMAT(DATE_FROM,'%d/%m/%Y') as DATE_FROM,
                                        DATE_FORMAT(DATE_TO,'%d/%m/%Y')as DATE_TO,
                                        ENTRY_ID,
                                        IS_FINISHED
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING
                                    WHERE
                                        COURSE_MAPPING_ID=?COURSE_MAPPING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.IsExistCourseMappingInfo:
                    {
                        query = @"SELECT 
                                        COURSE_MAPPING_ID,
                                        FEEDBACK_SETTING_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                        ENTRY_ID,
                                        IS_FINISHED
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND COURSE_ID = ?COURSE_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.SaveCourseMappingInfo:
                    {
                        query = @"INSERT INTO FB_EXTERNAL_USER_COURSE_MAPPING
                                  (
                                      FEEDBACK_SETTING_ID,
	                                  COURSE_ID,
	                                  STAFF_ID,
	                                  DATE_FROM,
	                                  DATE_TO,
	                                  ENTRY_ID,
	                                  ACADEMIC_YEAR
                                  )
                                  VALUES
                                  (
                                      ?FEEDBACK_SETTING_ID,
                                      ?COURSE_ID,
	                                  ?STAFF_ID,
	                                  ?DATE_FROM,
	                                  ?DATE_TO,
	                                  ?ENTRY_ID,
	                                  ?AC_YEAR
                                  );";
                        break;
                    }
                case FeedbackSQLCommands.UpdateCourseMapping:
                    {
                        query = @"UPDATE FB_EXTERNAL_USER_COURSE_MAPPING
                                 SET
                                     FEEDBACK_SETTING_ID=?FEEDBACK_SETTING_ID,
	                                 COURSE_ID = ?COURSE_ID,
	                                 STAFF_ID = ?STAFF_ID,
	                                 DATE_FROM = ?DATE_FROM,
	                                 DATE_TO = ?DATE_TO,
	                                 ENTRY_ID = ?ENTRY_ID,
	                                 ACADEMIC_YEAR = ?AC_YEAR
                                 WHERE COURSE_MAPPING_ID = ?COURSE_MAPPING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteCourseMapping:
                    {
                        query = @"UPDATE FB_EXTERNAL_USER_COURSE_MAPPING SET IS_DELETED=1 WHERE COURSE_MAPPING_ID=?COURSE_MAPPING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchExternalUserList:
                    {
                        query = @"SELECT 
                                        EXTERNAL_USER_ID AS STAFF_ID,
                                        CONCAT(FIRST_NAME,' ( ',EXTERNAL_USER_CODE,' )') AS FIRST_NAME
                                    FROM
                                        EXTERNAL_USERS AS EU
                                    WHERE
                                        EU.IS_DELETED != 1  
                                            AND EU.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }

                case FeedbackSQLCommands.CheckIsExternalUserExist:
                    {
                        query = @"SELECT EXTERNAL_USER_ID,EXTERNAL_USER_CODE FROM EXTERNAL_USERS WHERE MOBILE_NO=?MOBILE_NO AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.CheckIsUserAccountExist:
                    {
                        query = @"SELECT USER_ACCOUNT_ID FROM USER_ACCOUNT WHERE USERNAME=?USERNAME AND USER_ID=?USER_ID AND IS_DELETED!=1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchMappedCourseListByExternalStaffId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        D.DEPARTMENT,
                                        CCR.SEMESTER_ID,
                                        FEEDBACK_SETTING_ID AS FEEDBACK_ID,
                                        CM.IS_FINISHED AS IS_ACTIVE
                                    FROM
                                        EXTERNAL_USERS AS EU
                                            INNER JOIN
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS CM ON CM.STAFF_ID = EU.EXTERNAL_USER_ID
                                            AND CM.ACADEMIC_YEAR=?AC_YEAR
                                            AND CM.IS_DELETED != 1
                                            INNER JOIN
                                        FBFEEDBACK_SETTINGS AS FB ON FB.FEEDBACK_ID = CM.FEEDBACK_SETTING_ID
                                            AND FB.IS_ACTIVE = 1
                                            AND FB.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CM.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CCR.DEPARTMENT
                                    WHERE
                                        EU.IS_DELETED != 1 AND EU.EXTERNAL_USER_ID=?STAFF_ID ORDER BY CCR.DEPARTMENT;";
                        break;
                    }
                case FeedbackSQLCommands.FetchExternalCourseMappingInfoByCourseAndStaffId:
                    {
                        query = @"SELECT 
                                    COURSE_MAPPING_ID, 
                                    FEEDBACK_SETTING_ID, 
                                    COURSE_ID, 
                                    STAFF_ID, 
                                    IS_FINISHED
                                FROM
                                    FB_EXTERNAL_USER_COURSE_MAPPING AS cp
                                WHERE
	                                CP.ACADEMIC_YEAR=?AC_YEAR
                                    AND CP.COURSE_ID=?COURSE_ID AND CP.STAFF_ID=?STAFF_ID
                                    AND CP.IS_DELETED!=1
                                    AND CP.FEEDBACK_SETTING_ID = (SELECT 
                                            FEEDBACK_ID
                                        FROM
                                            FBFEEDBACK_SETTINGS
                                        WHERE
                                            ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                                AND IS_ACTIVE = 1
                                                AND IS_DELETED != 1);";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFBFinishedStatus:
                    {
                        query = @"UPDATE FB_EXTERNAL_USER_COURSE_MAPPING SET IS_FINISHED=1 WHERE COURSE_MAPPING_ID=?COURSE_MAPPING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.SaveExternalFeedbackEntry:
                    {
                        query = @"INSERT INTO FB_EXTERNAL_USER_ENTRY
                                  (
                                      FEEDBACK_SETTING_ID,
                                      QUESTION_ID,
                                      COURSE_ID,
                                      STAFF_ID,
                                      REMARK,
                                      ENTRY_DATE,
                                      ACADEMIC_YEAR
                                  )
                                  VALUES
                                  (
                                      ?FEEDBACK_SETTING_ID,
                                      ?QUESTION_ID,
                                      ?COURSE_ID,
                                      ?STAFF_ID,
                                      ?REMARK,
                                      CURDATE(),
                                      ?AC_YEAR
                                  );";
                        break;
                    }
                case FeedbackSQLCommands.UpdateExternalFeedbackEntry:
                    {
                        query = @"UPDATE FB_EXTERNAL_USER_ENTRY
                                  SET
                                    REMARK = ?REMARK
                                  WHERE FB_ENTRY_ID = ?FB_ENTRY_ID;";
                        break;
                    }

                case FeedbackSQLCommands.FetchExternalFeedbackEntryByCourseAndStaffId:
                    {
                        query = @"SELECT 
                                        FB_ENTRY_ID,
                                        QUESTION_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        REMARK
                                    FROM
                                        FB_EXTERNAL_USER_ENTRY
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND COURSE_ID=?COURSE_ID
                                            AND FEEDBACK_SETTING_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS
                                            WHERE
                                                IS_ACTIVE = 1 AND ACADEMIC_YEAR = ?AC_YEAR);";
                        break;
                    }
                case FeedbackSQLCommands.FetchHODsNameAndMobileNoForExternalUser:
                    {
                        query = @"SELECT 
                                        D.DEPARTMENT, 
                                        U.NAME AS FIRST_NAME, 
                                        U.MOBILE
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS CM
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CM.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.DEPARTMENT = CCR.DEPARTMENT
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        USER_ACCOUNT AS U ON U.USERNAME = SP.STAFF_CODE
                                            AND U.USER_ID = SP.STAFF_ID
                                            INNER JOIN
                                        USER_ROLES_BY_ACADEMIC_YEAR AS UR ON UR.USER_ID = U.USER_ID
                                            AND UR.ACADEMIC_YEAR = ?AC_YEAR
                                            AND UR.USER_TYPE = U.USER_TYPE
                                            AND UR.ROLE_ID = 7
                                            AND UR.IS_DELETED!=1
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CCR.DEPARTMENT
                                    WHERE
                                        CM.IS_DELETED != 1 AND CM.STAFF_ID = ?STAFF_ID
                                            AND CM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CM.FEEDBACK_SETTING_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS
                                            WHERE
                                                ACADEMIC_YEAR = CM.ACADEMIC_YEAR
                                                    AND IS_ACTIVE = 1
                                                    AND IS_DELETED != 1)
                                    GROUP BY U.USERNAME ORDER BY CCR.DEPARTMENT;";
                        break;
                    }
                case FeedbackSQLCommands.FetchExternalUserCourseMappingListForAdmin:
                    {
                        query = @"SELECT 
                                        COURSE_MAPPING_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        DATE_FORMAT(EU.DATE_FROM,'%d/%m/%Y') as DATE_FROM,
                                        DATE_FORMAT(EU.DATE_TO,'%d/%m/%Y')as DATE_TO,
                                        ENTRY_ID,
                                        IS_FINISHED,
                                        U.EXTERNAL_USER_CODE,
                                        U.FIRST_NAME,
                                        DATE_FORMAT(U.DOB,'%d/%m/%Y') AS DOB,
                                        U.MOBILE_NO,
                                        U.COLLEGE_NAME,
                                        D.DEPARTMENT,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS EU
		                                    INNER JOIN
	                                    EXTERNAL_USERS AS U ON U.EXTERNAL_USER_ID=EU.STAFF_ID
		                                    AND U.IS_DELETED!=1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=EU.COURSE_ID
                                            AND CCR.IS_DELETED!=1
                                            INNER JOIN
                                        FBFEEDBACK_SETTINGS AS FB ON FB.FEEDBACK_ID=EU.FEEDBACK_SETTING_ID
                                            AND FB.ACADEMIC_YEAR=EU.ACADEMIC_YEAR
                                            AND FB.IS_ACTIVE=1
                                            AND FB.IS_DELETED!=1
                                            INNER JOIN  
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=CCR.DEPARTMENT
                                    WHERE
                                        EU.IS_DELETED!=1
                                        AND EU.ACADEMIC_YEAR=?AC_YEAR ORDER BY U.EXTERNAL_USER_ID;";
                        break;
                    }
                #endregion

                #region Send SMS
                case FeedbackSQLCommands.FetchMessageContentForExternalUserCourseMapping:
                    {
                        query = @"SELECT 
                                    UI.MOBILE_NO AS `to`,
                                    CONCAT('Respected Professor ',UI.FIRST_NAME,' Greetings from St. Josephs College of Arts and Science (Autonomous), Cuddalore. We are happy to include you as a reviewer for our syllabus. Kindly spare your valuable time and review the syllabus allotted to you. The review can be done in online using the link http://erp.sjctnc.in/ ( User Name : ',ui.EXTERNAL_USER_CODE,';  Password : ',ui.MOBILE_NO,')') AS message,
                                    EP.STAFF_ID AS custom,
                                    EP.FEEDBACK_SETTING_ID AS custom1,
                                    MAIL_ID AS custom2,
                                    '2' AS CreditCount
                                FROM
                                    FB_EXTERNAL_USER_COURSE_MAPPING AS EP
                                        INNER JOIN
                                    EXTERNAL_USERS AS UI ON UI.EXTERNAL_USER_ID = EP.STAFF_ID
                                        AND UI.IS_DELETED != 1
                                WHERE
                                    EP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND EP.IS_DELETED != 1 AND EP.ENTRY_ID=?ENTRY_ID
                                        AND EP.FEEDBACK_SETTING_ID = (SELECT 
                                            FEEDBACK_ID
                                        FROM
                                            FBFEEDBACK_SETTINGS
                                        WHERE
                                            ACADEMIC_YEAR = EP.ACADEMIC_YEAR
                                                AND IS_ACTIVE = 1
                                                AND IS_DELETED != 1)
                                GROUP BY EP.STAFF_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFBSentSMSList:
                    {
                        query = @"SELECT 
                                        SENT_SMS_ID,
                                        STAFF_ID,
                                        IS_SMS_SENT,
                                        FEEDBACK_SETTING_ID,
                                        ACADEMIC_YEAR
                                    FROM
                                        FB_SENT_SMS
                                    WHERE
                                        FEEDBACK_SETTING_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS
                                            WHERE
                                                ACADEMIC_YEAR = ?AC_YEAR
                                                    AND IS_ACTIVE = 1
                                                    AND IS_DELETED != 1);";
                        break;
                    }
                #endregion

                #region E Certificate
                case FeedbackSQLCommands.FetchAllotedDepartmentForExternalUser:
                    {
                        query = @"SELECT 
                                        D.DEPARTMENT_ID, 
                                        D.DEPARTMENT
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CCR.DEPARTMENT
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR AND CP.STAFF_ID=?STAFF_ID
                                            AND CP.IS_DELETED != 1
                                            AND CP.FEEDBACK_SETTING_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS
                                            WHERE
                                                ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                                    AND IS_ACTIVE = 1
                                                    AND IS_DELETED != 1)
                                    GROUP BY D.DEPARTMENT_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchAllotedCoursesByDepartmnetIdAndExternalUserId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CONCAT('( ',
                                                CCR.COURSE_CODE,
                                                ' )  -  ( SEM - ',
                                                SEMESTER_ID,
                                                ' )  ',
                                                CCR.COURSE_TITLE) AS COURSE_TITLE
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            AND CCR.DEPARTMENT=?DEPARTMENT
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CP.STAFF_ID = ?STAFF_ID
                                            AND CP.IS_DELETED != 1
                                            AND CP.FEEDBACK_SETTING_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS
                                            WHERE
                                                ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                                    AND IS_ACTIVE = 1
                                                    AND IS_DELETED != 1);";
                        break;
                    }
                case FeedbackSQLCommands.FetchExternalUserCertificateInfo:
                    {
                        query = @"SELECT 
	                                    ES.FIRST_NAME,
	                                    CCR.COURSE_ROOT_ID AS COURSE_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        SD.DESIGNATION,
                                        D.DEPARTMENT,
                                        ES.COLLEGE_NAME,
                                        DATE_FORMAT(CP.ENTRY_DATE,'%d/%m/%Y') AS ENTRY_DATE
                                    FROM
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
	                                    EXTERNAL_USERS AS ES ON ES.EXTERNAL_USER_ID=CP.STAFF_ID
		                                    AND ES.IS_DELETED!=1
                                            INNER JOIN
	                                    SUP_DESIGNATION AS SD ON SD.DESIGNATIONID=ES.DESIGNATION
		                                    INNER JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=CCR.DEPARTMENT
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR AND CP.COURSE_ID=?COURSE_ID
                                            AND CP.STAFF_ID = ?STAFF_ID
                                            AND CP.IS_DELETED != 1 AND CP.IS_FINISHED=1
                                            AND CP.FEEDBACK_SETTING_ID = (SELECT 
                                                FEEDBACK_ID
                                            FROM
                                                FBFEEDBACK_SETTINGS
                                            WHERE
                                                ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                                    AND IS_ACTIVE = 1
                                                    AND IS_DELETED != 1);";
                        break;
                    }
                #endregion

                #region External Feedback Report 
                case FeedbackSQLCommands.FetchExternalFeedBackReportByCourseAndSettingId:
                    {
                        query = @"SELECT 
                                        E.FB_ENTRY_ID,
                                        E.FEEDBACK_SETTING_ID,
                                        E.QUESTION_ID,
                                        E.REMARK,
                                        Q.QUESTION,
                                        QG.QUESTION_GROUP_ID,
                                        QG.GROUP_TITLE,
                                        E.STAFF_ID,
                                        EU.FIRST_NAME,
                                        EU.COLLEGE_NAME,
                                        EU.MOBILE_NO,
                                        E.COURSE_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        D.DEPARTMENT
                                    FROM
                                        FB_EXTERNAL_USER_ENTRY AS E
                                            INNER JOIN
                                        EXTERNAL_USERS AS EU ON EU.EXTERNAL_USER_ID = E.STAFF_ID
                                            AND EU.IS_DELETED != 1
                                            INNER JOIN
                                        FB_EXTERNAL_USER_COURSE_MAPPING AS CM ON CM.STAFF_ID = E.STAFF_ID
                                            AND CM.COURSE_ID=E.COURSE_ID
                                            AND CM.IS_DELETED != 1
                                            AND CM.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CM.FEEDBACK_SETTING_ID = E.FEEDBACK_SETTING_ID
                                            INNER JOIN
                                        FBFEEDBACK_QUESTIONS AS Q ON Q.QUESTION_ID = E.QUESTION_ID
                                            AND Q.IS_DELETED != 1
                                            AND Q.FEEDBACK_ID = E.FEEDBACK_SETTING_ID
                                            INNER JOIN
                                        FBQUESTIONS_GROUP AS QG ON QG.QUESTION_GROUP_ID = Q.QUESTION_GROUP
                                            AND QG.FEEDBACK_ID = E.FEEDBACK_SETTING_ID
                                            AND QG.IS_DELETED != 1
                                            INNER JOIN
                                        cp_course_root AS CCR ON CCR.COURSE_ROOT_ID = E.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CCR.DEPARTMENT
                                    WHERE
                                        E.FEEDBACK_SETTING_ID = ?FEEDBACK_SETTING_ID
                                            AND E.COURSE_ID = ?COURSE_ID
                                    GROUP BY E.COURSE_ID,E.QUESTION_ID,E.STAFF_ID;";
                        break;
                    }
                #endregion

                #region Fetch Feedback syllabus
                case FeedbackSQLCommands.FetchFeedbackOptionsByFeedbackId:
                    {
                        query = @"SELECT 
                                        FQ.QUESTION_ID, 
                                        OP.OPTION_ID, 
                                        OP.OPTION_NAME
                                    FROM
                                        FBFEEDBACK_QUESTIONS AS FQ
                                            INNER JOIN
                                        FBOPTIONS AS OP ON OP.IS_DELETED = FQ.IS_DELETED
                                            AND FIND_IN_SET(OP.OPTION_ID, FQ.QUESTION_TYPE)
                                    WHERE
                                        FQ.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FQ.FEEDBACK_ID = ?FEEDBACK_ID
                                    ORDER BY FQ.QUESTION_ID,OP.ORDER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.SyllabusFeedbackIsExistBySettingAndCourseId:
                    {
                        query = @"SELECT 
                                    EVALUATION_ID
                                  FROM
                                      FBEVALUATIONSYLLABUS
                                  WHERE
                                      FEEDBACK_ID = ?FEEDBACK_ID 
                                          AND COURSE_ID = ?COURSE_ID
                                          AND STUDENT_ID = ?STUDENT_ID 
                                          AND ACADEMIC_YEAR=?AC_YEAR
                                          AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbSyllabusStudentCount:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS STU_COUNT,CP.COURSE_ID,
                                        (SELECT 
                                                COUNT(STUDENT_ID)
                                            FROM
                                                (SELECT 
                                                    STUDENT_ID
                                                FROM
                                                    FBEVALUATIONSYLLABUS
                                                WHERE
                                                    COURSE_ID = ?COURSE_ID AND FEEDBACK_ID = ?FEEDBACK_ID
                                                GROUP BY STUDENT_ID) AS T) AS 'ATT_COUNT'
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_PATTERN AS CP ON CP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CP.COURSE_ID = ?COURSE_ID
                                            AND CP.IS_DELETED != 1
                                             INNER JOIN
                                        CP_COURSE_STRUCTURE AS CCS ON CCS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                            AND CCS.PROGRAMME_YEAR = C.CLASS_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CCR.IS_COMPULSORY = 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1 
                                    UNION SELECT 
                                        COUNT(SC.STUDENT_ID) AS STU_COUNT,
                                        SC.COURSE_ID,
                                        (SELECT 
                                                COUNT(STUDENT_ID)
                                            FROM
                                                (SELECT 
                                                    STUDENT_ID
                                                FROM
                                                    FBEVALUATIONSYLLABUS
                                                WHERE
                                                    COURSE_ID = ?COURSE_ID AND FEEDBACK_ID = ?FEEDBACK_ID
                                                GROUP BY STUDENT_ID) AS T) AS 'ATT_COUNT'
                                    FROM
                                        STU_COURSE_INFO AS SC
                                    WHERE
                                        SC.COURSE_ID = ?COURSE_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFbOptionEvaluationCount:
                    {
                        query = @"SELECT 
	                                    QUESTION_ID,
                                        ANSWER_ID,
                                        COUNT(ANSWER_ID) AS 'ANS_COUNT',COURSE_ID
                                    FROM
                                        FBEVALUATIONSYLLABUS 
                                    WHERE
                                        COURSE_ID = ?COURSE_ID AND FEEDBACK_ID = ?FEEDBACK_ID
                                    GROUP BY ANSWER_ID,QUESTION_ID 
                                    ORDER BY QUESTION_ID;";
                        break;
                    }
                #endregion

                case FeedbackSQLCommands.FetchFeedBackSettingsByAcademicYear:
                    {
                        query = @"SELECT 
                                    FS.FEEDBACK_ID,
                                    FS.FEEDBACK_NAME,
                                    DATE_FORMAT(FS.DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(FS.DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                    FS.NO_OF_QUESTION,
                                    FS.ACADEMIC_YEAR,
                                    FS.IS_DELETED,
                                    FS.CLASS_ID,
                                    FS.IS_ACTIVE,
                                    FS.ASSESSOR,
                                    FS.ASSESSEE,
                                    FS.MANUAL_STAFF_SELECTION,
                                    FS.WITH_IN_DEPARTMENT,
                                    FS.ASSESSOR_CATEGORY,
                                    FS.ASSESSEE_CATEGORY
                                FROM
                                    FBFEEDBACK_SETTINGS AS FS
                                    
                                    INNER JOIN
                                    ACADEMIC_YEAR AS A ON A.AC_YEAR=FS.ACADEMIC_YEAR
                                    AND A.IS_DELETED!=1
                                    
                                        
                                WHERE
                                    FS.IS_DELETED != 1
                                        AND A.ACADEMIC_YEAR_ID=?ACADEMIC_YEAR_ID;";
                        break;
                    }


                case FeedbackSQLCommands.FetchStudentFeedBackSettingsByAcademicYear:
                    {
                        query = @"SELECT 
                                    FS.FEEDBACK_ID,
                                    FS.FEEDBACK_NAME,
                                    DATE_FORMAT(FS.DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(FS.DATE_TO,'%d/%m/%Y') AS DATE_TO,
                                    FS.NO_OF_QUESTION,
                                    FS.ACADEMIC_YEAR,
                                    FS.IS_DELETED,
                                    FS.CLASS_ID,
                                    FS.IS_ACTIVE,
                                    FS.ASSESSOR,
                                    FS.ASSESSEE,
                                    FS.MANUAL_STAFF_SELECTION,
                                    FS.WITH_IN_DEPARTMENT,
                                    FS.ASSESSOR_CATEGORY,
                                    FS.ASSESSEE_CATEGORY
                                FROM
                                    FBFEEDBACK_SETTINGS AS FS
                                    
                                    INNER JOIN
                                    ACADEMIC_YEAR AS A ON A.AC_YEAR=FS.ACADEMIC_YEAR
                                    AND A.IS_DELETED!=1
                                    
                                        
                                WHERE
                                    FS.IS_DELETED != 1
                                        AND A.ACADEMIC_YEAR_ID=?ACADEMIC_YEAR_ID AND FEEDBACK_TYPE= 1;";
                        break;
                    }

                case FeedbackSQLCommands.FetchStudentFeedbackReportByDept:
                    {
                        query = @"SELECT 
                                        ST.DEPARTMENT,
                                        ST.CLASS_NAME,
                                        ST.SHIFT,
                                        ST.CLASS_YEAR,
                                        ST.TOTAL_STUDENT,
                                        ST.CLASS_ID,
                                        TT.SCOUNT AS ATTEND_STUDENT,
                                        ST.APPLICATION_TYPE
                                    FROM
                                        (SELECT 
                                            COUNT(SC.STUDENT_ID) AS TOTAL_STUDENT,
                                                SC.CLASS_ID,
                                                DP.DEPARTMENT,
                                                CL.CLASS_NAME,
                                                CL.CLASS_YEAR,
                                                CL.SHIFT,
                                                AY.APPLICATION_TYPE
                                        FROM
                                            STU_CLASS AS SC
    
                                        INNER JOIN cp_classes AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                            AND CL.CLASS_YEAR=?CLASS_YEAR
        
                                            INNER JOIN CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED != 1
                                            AND CPP.APPTYPE_ID =?APPTYPE_ID
                                            
                                           INNER JOIN
                                            SUP_APPLICATION_TYPE AS AY ON AY.APPLICATION_TYPE_ID=CPP.APPTYPE_ID
                                            AND AY.IS_DELETED!=1
        
                                        INNER JOIN cp_department AS DP ON DP.DEPARTMENT_ID = CL.DEPARTMENT
                                        WHERE
                                            SC.ACADEMIC_YEAR =?AC_YEAR
                                                AND SC.IS_DELETED != 1
                                        GROUP BY SC.CLASS_ID) AS ST
                                            LEFT JOIN
                                        (SELECT 
                                            T1.SCOUNT,
                                                T1.CLASS_NAME,
                                                T1.CLASS_ID,
                                                T1.SHIFT,
                                                T1.DEPARTMENT,
                                                T1.FEEDBACK_ID
                                        FROM
                                            (SELECT 
                                            COUNT(T.STUDENT_ID) AS SCOUNT,
                                                T.CLASS_NAME,
                                                T.CLASS_ID,
                                                T.SHIFT,
                                                T.DEPARTMENT,
                                                T.FEEDBACK_ID
                                        FROM
                                            (SELECT 
                                            SC.STUDENT_ID,
                                                CC.CLASS_NAME,
                                                CC.CLASS_ID,
                                                CC.SHIFT,
                                                D.DEPARTMENT,
                                                FB.FEEDBACK_ID
                                        FROM
                                            FBEVALUATION?AC_YEAR AS FB
                                        INNER JOIN STU_CLASS AS SC ON SC.STUDENT_ID = FB.ASSESSOR
                                            AND SC.ACADEMIC_YEAR =?AC_YEAR
                                            AND SC.IS_DELETED != 1
    
                                        INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            AND CC.CLASS_YEAR=?CLASS_YEAR
                                            AND CC.IS_DELETED != 1
        
                                            INNER JOIN CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                            AND CPP.IS_DELETED != 1
                                            AND CPP.APPTYPE_ID =?APPTYPE_ID
        
                                        INNER JOIN CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CC.DEPARTMENT
                                        WHERE
                                            FB.FEEDBACK_ID =?FEEDBACK_ID
                                                AND FB.ISDELETED != 1
                                        GROUP BY FB.ASSESSOR) AS T
                                        GROUP BY T.CLASS_ID) AS T1) AS TT ON TT.CLASS_ID = ST.CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchCourseByDepartmentForExternalFeedback:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CONCAT('( ',
                                                CCR.COURSE_CODE,
                                                ' )  -  ( SEM - ',
                                                SEMESTER_ID,
                                                ' )  ',
                                                CCR.COURSE_TITLE) AS COURSE_TITLE,
                                        E.FB_ENTRY_ID AS PAPER
                                    FROM
                                        CP_COURSE_ROOT AS CCR
		                                    INNER JOIN
	                                    FB_EXTERNAL_USER_COURSE_MAPPING AS CM ON CM.COURSE_ID=CCR.COURSE_ROOT_ID
		                                    AND CM.ACADEMIC_YEAR=?AC_YEAR 
                                            AND CM.FEEDBACK_SETTING_ID=?FEEDBACK_SETTING_ID
                                            AND CM.IS_DELETED!=1
                                            LEFT JOIN
										FB_EXTERNAL_USER_ENTRY AS E ON E.COURSE_ID=CM.COURSE_ID
											AND E.FEEDBACK_SETTING_ID=CM.FEEDBACK_SETTING_ID
                                            AND E.STAFF_ID=CM.STAFF_ID
                                            AND E.IS_DELETED!=1
                                    WHERE
                                        CCR.IS_DELETED != 1
                                            AND CCR.DEPARTMENT = ?DEPARTMENT_ID
                                        GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PART,CCR.SEMESTER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchCourseByDepartmentForExternalFeedbackStatus:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CONCAT('( ',
                                                CCR.COURSE_CODE,
                                                ' )  -  ( SEM - ',
                                                SEMESTER_ID,
                                                ' )  ',
                                                CCR.COURSE_TITLE) AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_ROOT AS CCR
		                                    INNER JOIN
	                                    FB_EXTERNAL_USER_COURSE_MAPPING AS CM ON CM.COURSE_ID=CCR.COURSE_ROOT_ID
		                                    AND CM.ACADEMIC_YEAR=?AC_YEAR 
                                            AND CM.FEEDBACK_SETTING_ID=?FEEDBACK_SETTING_ID
                                            AND CM.IS_DELETED!=1
                                    WHERE
                                        CCR.IS_DELETED != 1
                                            AND CCR.DEPARTMENT = ?DEPARTMENT_ID
                                        GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PART,CCR.SEMESTER_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchClassWiseStudentStrengthForFeedbackReport:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT, SC.CLASS_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                           AND SPI.IS_DELETED != 1
                                           AND SPI.IS_LEFT != 1
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.CLASS_ID 
                                    UNION SELECT 
                                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT, S_CLASS_ID
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.S_CLASS_ID;";
                        break;
                    }

                #region Feedback pie chart 
                case FeedbackSQLCommands.FetchFbEvaluationInfoForReportByClassandAcYear:
                    {
                        query = @"    SELECT 
                                        FQ.QUESTION_ID,
                                        FQ.QUESTION,
                                        FB.ANSWER,
                                        O.OBJECTIVES,
                                        SC.STUDENT_ID,
                                        SP.STAFF_CODE,
                                        SP.FIRST_NAME,
                                        CC.CLASS_NAME,
                                        CR.COURSE_TITLE,
                                        FB.COURSE_ID,
                                        SC.CLASS_ID
                                FROM
                                    STU_CLASS AS SC
                                INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                INNER JOIN STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    AND SPI.IS_DELETED != 1
                                    AND SPI.IS_LEFT != 1
                                INNER JOIN FBEVALUATION?AC_YEAR AS FB ON FB.ASSESSOR = SPI.STUDENT_ID
                                    AND FB.ISDELETED != 1
                                    AND FB.FEEDBACK_ID =?FEEDBACK_ID
                                    LEFT JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=FB.ASSESSEE
                                INNER JOIN FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FB.QUESTION
                                    AND FQ.IS_DELETED != 1						
                                INNER JOIN FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FB.ANSWER
                                    AND O.IS_DELETED != 1
                                   LEFT JOIN
                                      CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = FB.COURSE_ID
                                WHERE
                                       SC.CLASS_ID in(?CLASS_ID)
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1 GROUP BY FQ.QUESTION_ID,FB.ASSESSOR,FB.COURSE_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentCountByFBQuestionRatingbyClassWise:
                    {
                        query = @"SELECT 
                                    QUESTION_ID,
                                    SUM(ANSWER) AS TOTAL,
                                    SUM(NO_OF_STUDENTS) AS NO_OF_STUDENT,
                                    COURSE_ID,
                                    CLASS_ID
                                FROM
                                    (SELECT 
                                        QUESTION AS QUESTION_ID,
                                            ANSWER AS ANSWER_ID,
                                            SUM(FO.RATING) AS ANSWER,
                                            COUNT(ASSESSOR) AS NO_OF_STUDENTS,
                                            COURSE_ID,
                                            SC.CLASS_ID
                                    FROM
                                        FBEVALUATION?AC_YEAR AS FB
                                    INNER JOIN fbobjectives AS FO ON FB.ANSWER = FO.OBJECTIVE_ID
                                    INNER JOIN stu_class AS sc ON sc.STUDENT_ID = fb.ASSESSOR
                                    WHERE
                                        SC.CLASS_ID IN (?CLASS_ID)
                                            AND FB.FEEDBACK_ID = ?FEEDBACK_ID
                                            AND FB.ISDELETED != 1
                                    GROUP BY QUESTION , ANSWER , COURSE_ID , CLASS_ID) AS T
                                GROUP BY T.QUESTION_ID , T.COURSE_ID , t.CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentFeedbackReportByClass_id:
                    {
                        query = @"SELECT 
                                    ST.DEPARTMENT,
                                    ST.CLASS_NAME,
                                    ST.SHIFT,
                                    ST.CLASS_YEAR,
                                    ST.TOTAL_STUDENT,
                                    ST.CLASS_ID,
                                    TT.SCOUNT AS ATTEND_STUDENT,
                                    ST.APPLICATION_TYPE
                                FROM
                                    (SELECT 
                                        COUNT(SC.STUDENT_ID) AS TOTAL_STUDENT,
                                            SC.CLASS_ID,
                                            DP.DEPARTMENT,
                                            CL.CLASS_NAME,
                                            CL.CLASS_YEAR,
                                            CL.SHIFT,
                                            AY.APPLICATION_TYPE
                                    FROM
                                        STU_CLASS AS SC
                                    INNER JOIN CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                        AND CL.CLASS_ID IN (?CLASS_ID)
                                    INNER JOIN SUP_APPLICATION_TYPE AS AY ON AY.APPLICATION_TYPE_ID = CPP.APPTYPE_ID
                                        AND AY.IS_DELETED != 1
                                    INNER JOIN cp_department AS DP ON DP.DEPARTMENT_ID = CL.DEPARTMENT
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    GROUP BY SC.CLASS_ID) AS ST
                                        LEFT JOIN
                                    (SELECT 
                                        T1.SCOUNT,
                                            T1.CLASS_NAME,
                                            T1.CLASS_ID,
                                            T1.SHIFT,
                                            T1.DEPARTMENT,
                                            T1.FEEDBACK_ID
                                    FROM
                                        (SELECT 
                                        COUNT(T.STUDENT_ID) AS SCOUNT,
                                            T.CLASS_NAME,
                                            T.CLASS_ID,
                                            T.SHIFT,
                                            T.DEPARTMENT,
                                            T.FEEDBACK_ID
                                    FROM
                                        (SELECT 
                                        SC.STUDENT_ID,
                                            CC.CLASS_NAME,
                                            CC.CLASS_ID,
                                            CC.SHIFT,
                                            D.DEPARTMENT,
                                            FB.FEEDBACK_ID
                                    FROM
                                        FBEVALUATION?AC_YEAR AS FB
                                    INNER JOIN STU_CLASS AS SC ON SC.STUDENT_ID = FB.ASSESSOR
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                    INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        AND CC.IS_DELETED != 1
                                    INNER JOIN CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                        AND CC.CLASS_ID IN (?CLASS_ID)
                                    INNER JOIN CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CC.DEPARTMENT
                                    WHERE
                                        FB.FEEDBACK_ID = ?FEEDBACK_ID
                                            AND FB.ISDELETED != 1
                                    GROUP BY FB.ASSESSOR) AS T
                                    GROUP BY T.CLASS_ID) AS T1) AS TT ON TT.CLASS_ID = ST.CLASS_ID;";
                        break;
                    }
                #endregion

                case FeedbackSQLCommands.FetchFeedBackClassSettings:
                    {
                        query = @"SELECT 
                                    FBS.CLASS_SETTING_ID,
                                    FS.FEEDBACK_NAME,
                                    DATE_FORMAT(FS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(FS.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    FS.NO_OF_QUESTION,
                                    FS.ACADEMIC_YEAR,
                                    FS.IS_DELETED,
                                    CC.CLASS_NAME,
                                    FS.CLASS_ID,
                                    FBS.IS_ACTIVE,
                                    FS.ASSESSOR,
                                    FS.ASSESSEE,
                                    FS.MANUAL_STAFF_SELECTION,
                                    FS.WITH_IN_DEPARTMENT,
                                    FS.ASSESSOR_CATEGORY,
                                    FS.ASSESSEE_CATEGORY
                                FROM
                                    FB_CLASS_SETTING AS FBS
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FBS.SETTING_ID
                                        AND FS.IS_DELETED != 1
                                        AND FS.IS_ACTIVE = 1
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = FBS.CLASS_ID
                                WHERE
                                    FBS.IS_DELETED != 1
                                        AND FS.FEEDBACK_ID = ?FEEDBACK_ID
                                        AND FS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackClassSettingsById:
                    {
                        query = @"SELECT 
                    FBS.CLASS_SETTING_ID,
                    FS.FEEDBACK_NAME,
                    DATE_FORMAT(FS.DATE_FROM,'%d/%m/%Y') AS DATE_FROM,
                    DATE_FORMAT(FS.DATE_TO,'%d/%m/%Y') AS DATE_TO,
                    FS.NO_OF_QUESTION,
                    FS.ACADEMIC_YEAR,
                    FS.IS_DELETED,
                    CC.CLASS_NAME,
                    FS.CLASS_ID,
                    FS.IS_ACTIVE,
                    FS.ASSESSOR,
                    FS.ASSESSEE,
                    FS.MANUAL_STAFF_SELECTION,
                    FS.WITH_IN_DEPARTMENT,
                    FS.ASSESSOR_CATEGORY,
                    FS.ASSESSEE_CATEGORY
                FROM
                    Fb_class_setting AS FBS 
                    inner join 
                    fbfeedback_settings as FS on FS.FEEDBACK_ID  =FBS.SETTING_ID AND FS.IS_DELETED != 1 AND FS.IS_ACTIVE =1
                    INNER JOIN
                    cp_classes AS CC on CC.CLASS_ID = FBS.CLASS_ID
                        
                WHERE
                    FBS.IS_DELETED != 1
                        AND FS.ACADEMIC_YEAR =?AC_YEAR
                         AND FS.CLASS_SETTING_ID=?CLASS_SETTING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.isalreadyUpdatedfeedbckclass:
                    {
                        query = @" SELECT 
                                        CLASS_SETTING_ID, CLASS_ID, ACADEMIC_YEAR
                                    FROM
                                        FB_CLASS_SETTING
                                    WHERE
                                        ACADEMIC_YEAR = ?ACADEMIC_YEAR AND SETTING_ID = ?FEEDBACK_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case FeedbackSQLCommands.SaveFeedBackSClassetting:
                    {
                        query = @"INSERT INTO FB_CLASS_SETTING(CLASS_ID,SETTING_ID,FEEDBACK_TYPE,ACADEMIC_YEAR,IS_ACTIVE)
                      SELECT 
                         CC.CLASS_ID ,
                         FEEDBACK_ID AS SETTING_ID,
                         FEEDBACK_TYPE,
                         FS.ACADEMIC_YEAR,
                         0 AS IS_ACTIVE
                      FROM 
                         FBFEEDBACK_SETTINGS AS FS
                            INNER JOIN
                         CP_CLASSES AS CC 
                           WHERE
                     FS.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND FS.FEEDBACK_ID = ?FEEDBACK_ID AND FS.IS_DELETED !=1
                         AND 
                     CC.CLASS_ID in (?CLASS_ID) AND CC.IS_DELETED !=1 AND CC.CLASS_ID NOT IN
                         (SELECT CLASS_ID FROM FB_CLASS_SETTING WHERE CLASS_ID in (?CLASS_ID) AND SETTING_ID =?FEEDBACK_ID AND ACADEMIC_YEAR =?ACADEMIC_YEAR AND IS_DELETED!=1);";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackClassSettingToAllCLass:
                    {
                        query = @"INSERT INTO FB_CLASS_SETTING(CLASS_ID,SETTING_ID,FEEDBACK_TYPE,ACADEMIC_YEAR,IS_ACTIVE)
                   SELECT 
                      CC.CLASS_ID ,
                      FEEDBACK_ID AS SETTING_ID,
                      FEEDBACK_TYPE,
                      FS.ACADEMIC_YEAR,
                      1 AS IS_ACTIVE
                      FROM 
                      FBFEEDBACK_SETTINGS AS FS
                      INNER JOIN
                      CP_CLASSES AS CC 
                      WHERE
                      FS.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND FS.FEEDBACK_ID = ?FEEDBACK_ID AND FS.IS_DELETED !=1
                      AND 
                      CC.CLASS_TYPE =2 AND CLASS_YEAR IN(1,2,3) AND CC.IS_DELETED !=1;";
                        break;
                    }
                case FeedbackSQLCommands.UpdateFeedBackClassSetting:
                    {
                        query = @" UPDATE  Fb_class_setting SET  IS_ACTIVE =?IS_ACTIVE where  CLASS_SETTING_ID =?CLASS_SETTING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.ActivateorDeactivateFeedBackClassSettingById:
                    {
                        query = @"UPDATE Fb_class_setting SET IS_ACTIVE=?IS_ACTIVE
                    WHERE CLASS_SETTING_ID=?CLASS_SETTING_ID;";
                        break;
                    }
                case FeedbackSQLCommands.DeleteFeedBackClassSettingById:
                    {
                        query = @"UPDATE 
                    Fb_class_setting 
                        SET 
                        IS_DELETED=?IS_DELETED 
                        WHERE
                    CLASS_SETTING_ID=?CLASS_SETTING_ID;";
                        break;
                    }

                #region   question objective mapping 

                case FeedbackSQLCommands.ActivateorDeactivateFeedbackQuestionObjectiveMappingById:
                    {
                        query = @"UPDATE FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING SET IS_ACTIVE=?IS_ACTIVE
                  WHERE QUESTION_OBJECT_ID=?QUESTION_OBJECT_ID;";

                        break;
                    }


                case FeedbackSQLCommands.DeleteFeedbackQuestionObjectiveMappingById:
                    {
                        query = @"UPDATE FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING SET IS_DELETED=?IS_DELETED
                  WHERE
                QUESTION_OBJECT_ID=?QUESTION_OBJECT_ID;";

                        break;
                    }


                case FeedbackSQLCommands.FetchQuestionObjectiveMappingById:
                    {
                        query = @"SELECT 
                         FQBM.QUESTION_OBJECT_ID,
                         FS.FEEDBACK_NAME,
                         FQ.QUESTION,
                        FQBM.QUESTION_ID,
                        FQBM.OBJECTIVE_ID,
                        FQBM.FEEDBACK_ID,
                        FO.OBJECTIVES,
                        FQBM.ACADEMIC_YEAR,
                        FQBM.IS_ACTIVE
                    FROM
                        FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS FQBM
                            INNER JOIN
                        FBFEEDBACK_SETTINGS AS FS ON FQBM.FEEDBACK_ID = FS.FEEDBACK_ID AND FS.IS_DELETED !=1
                            INNER JOIN
                        FBFEEDBACK_QUESTIONS AS FQ ON FQBM.QUESTION_ID = FQ.QUESTION_ID AND FQ.IS_DELETED !=1
                            INNER JOIN
                        FBOBJECTIVES AS FO ON FQBM.OBJECTIVE_ID = FO.OBJECTIVE_ID AND FO.IS_DELETED !=1
                    WHERE
                        FQBM.IS_DELETED != 1 AND QUESTION_OBJECT_ID =?QUESTION_OBJECT_ID;";

                        break;
                    }





                case FeedbackSQLCommands.SaveFeedbackQuestionObjectiveMapping:
                    {
                        query = @"INSERT INTO FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING(
                                                                            FEEDBACK_ID,
                                                                            QUESTION_ID,
                                                                            OBJECTIVE_ID,
                                                                            ACADEMIC_YEAR,
                                                                            IS_ACTIVE)
                                                                       VALUES(
                                                                            ?FEEDBACK_ID,
                                                                            ?QUESTION_ID,
                                                                            ?OBJECTIVE_ID,
                                                                            ?ACADEMIC_YEAR,
                                                                            ?IS_ACTIVE);";

                        break;
                    }

                case FeedbackSQLCommands.UpdateFeedbackQuestionObjectiveMapping:
                    {
                        query = @"UPDATE FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING SET 
                       FEEDBACK_ID=?FEEDBACK_ID,
                       QUESTION_ID=?QUESTION_ID,
                       OBJECTIVE_ID=?OBJECTIVE_ID,
                       ACADEMIC_YEAR=?ACADEMIC_YEAR,
                       IS_ACTIVE=?IS_ACTIVE
                       WHERE 
                        QUESTION_OBJECT_ID=?QUESTION_OBJECT_ID;";

                        break;
                    }



                case FeedbackSQLCommands.FetchFeedbackQuestionMapping:
                    {
                        query = @"SELECT 
                                    FQBM.QUESTION_OBJECT_ID,
                                    FQBM.FEEDBACK_ID,
                                    FQBM.QUESTION_ID,
                                    FQBM.OBJECTIVE_ID,
                                    FS.FEEDBACK_NAME,
                                    FQ.QUESTION,
                                    FO.OBJECTIVES,
                                    FQBM.ACADEMIC_YEAR,
                                    FQBM.IS_ACTIVE
                                FROM
                                    FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS FQBM
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FQBM.FEEDBACK_ID = FS.FEEDBACK_ID
                                        AND FS.IS_DELETED != 1
                                        INNER JOIN
                                    FBFEEDBACK_QUESTIONS AS FQ ON FQBM.QUESTION_ID = FQ.QUESTION_ID
                                        AND fq.FEEDBACK_ID = FS.FEEDBACK_ID
                                        AND FQ.IS_DELETED != 1
                                        INNER JOIN
                                    FBOBJECTIVES AS FO ON FQBM.OBJECTIVE_ID = FO.OBJECTIVE_ID
                                        AND FO.IS_DELETED != 1
                                WHERE
                                    FQBM.IS_DELETED != 1
                                        AND FS.FEEDBACK_ID = ?FEEDBACK_ID;";

                        break;
                    }

                #endregion

                case FeedbackSQLCommands.FetchFeedEvalutaionByFeedbackID:
                    {
                        query = @"SELECT 
                                    FQ.QUESTION_ID,
                                    FQ.QUESTION,
                                    FB.ANSWER,
                                    SC.STUDENT_ID,
                                    CC.CLASS_NAME,
                                    FB.CLASS_ID,
                                    FB.FEEDBACK_ID,
                                    GROUP_TITLE,
                                    FQ.QUESTION_GROUP,
                                    O.OBJECTIVES

                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        AND CC.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    FBEVALUATION?AC_YEAR AS FB ON FB.ASSESSOR = SPI.STUDENT_ID
                                        AND FB.ISDELETED != 1
                                        AND FB.FEEDBACK_ID = ?FEEDBACK_ID
                                        INNER JOIN
                                    FBFEEDBACK_QUESTIONS AS FQ ON FQ.QUESTION_ID = FB.QUESTION
                                        AND FQ.IS_DELETED != 1
                                        INNER JOIN
                                    FBOBJECTIVES AS O ON O.OBJECTIVE_ID = FB.ANSWER
                                        AND O.IS_DELETED != 1
                                        LEFT JOIN
                                    FBQUESTIONS_GROUP AS FQG ON FQG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                        AND FQG.IS_DELETED != 1
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                GROUP BY FQ.QUESTION_ID , FB.ASSESSOR;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackSettingBySettingId:
                    {
                        query = @"SELECT 
                                        IS_HOSTEL,
                                        ACADEMIC_YEAR,
                                        FEEDBACK_TYPE,
                                        FEEDBACK_NAME
                                    FROM
                                        FBFEEDBACK_SETTINGS
                                    WHERE
                                        FEEDBACK_ID = ?FEEDBACK_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchAssorCountByFBQuestion1RatingBasedOverAll:
                    {
                        query = @"SELECT 
                                        QUESTION_ID,
                                        SUM(ANSWER) AS TOTAL,
                                        SUM(NO_OF_STUDENTS) AS NO_OF_STUDENT
                                    FROM
                                        (SELECT 
                                            QUESTION AS QUESTION_ID,
                                                ANSWER AS ANSWER_ID,
                                                SUM(FO.RATING) AS ANSWER,
                                                COUNT(ASSESSOR) AS NO_OF_STUDENTS
                                        FROM
                                            FBEVALUATION?AC_YEAR AS FB
			                                    INNER JOIN 
		                                    fbobjectives AS FO ON FB.ANSWER = FO.OBJECTIVE_ID
                                        WHERE
                                       
                                            FB.FEEDBACK_ID = ?FEEDBACK_ID  
                                            AND FB.ISDELETED != 1
                                    GROUP BY QUESTION , ANSWER) AS T
                                    GROUP BY T.QUESTION_ID;";
                        break;
                    }
                //case FeedbackSQLCommands.FetchClassWiseStudentStrengthForFeedbackReport:
                //    {
                //        query = @"SELECT 
                //                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT, SC.CLASS_ID
                //                    FROM
                //                        STU_CLASS AS SC
                //                            INNER JOIN
                //                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                //                    WHERE
                //                        SC.ACADEMIC_YEAR = ?AC_YEAR
                //                           AND SPI.IS_DELETED != 1
                //                           AND SPI.IS_LEFT != 1
                //                            AND SC.IS_DELETED != 1
                //                    GROUP BY SC.CLASS_ID 
                //                    UNION SELECT 
                //                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT, S_CLASS_ID
                //                    FROM
                //                        STU_COURSE_INFO AS SC
                //                            INNER JOIN
                //                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                //                    WHERE
                //                        SC.ACADEMIC_YEAR = ?AC_YEAR
                //                            AND SPI.IS_DELETED != 1
                //                            AND SPI.IS_LEFT != 1
                //                            AND SC.IS_DELETED != 1
                //                    GROUP BY SC.S_CLASS_ID;";
                //        break;
                //    }
                case FeedbackSQLCommands.FetchHostelStudentStrengthForFeedbackReport:
                    {
                        query = @"SELECT 
                                        COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT, SC.CLASS_ID
                                    FROM
                                        HOSTEL_STUDENT_INFO AS HSI
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = HSI.STUDENT_ID
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND HSI.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1;";
                        break;
                    }
                case FeedbackSQLCommands.FetchStudentStrengthFromFbClassSettingForFeedbackReport:
                    {
                        query = @"SELECT 
                                    COUNT(SC.STUDENT_ID) AS NO_OF_STUDENT, SC.CLASS_ID
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    FB_CLASS_SETTING AS FCS ON FCS.CLASS_ID = SC.CLASS_ID
                                        AND SC.ACADEMIC_YEAR = FCS.ACADEMIC_YEAR 
                                        AND FCS.IS_DELETED !=1
                                        INNER JOIN
                                    FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FCS.SETTING_ID
                                WHERE
                                    SC.ACADEMIC_YEAR = ?AC_YEAR AND FS.FEEDBACK_ID=?FEEDBACK_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        AND SC.IS_DELETED != 1
                                GROUP BY SC.CLASS_ID;";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedbackQuestionsMappingByFeedbackId:
                    {
                        query = @"SELECT 
                                     M.FEEDBACK_ID, M.QUESTION_ID, M.OBJECTIVE_ID,O.OBJECTIVES,O.RATING,O.OBJECTIVE_ORDER
                                FROM
                                    FBFEEDBACK_QUESTION_OBJECTIVE_MAPPING AS M
                                        INNER JOIN
                                    FBOBJECTIVES AS O ON O.OBJECTIVE_ID = M.OBJECTIVE_ID
                                        AND O.IS_DELETED != 1
                                WHERE
                                    M.FEEDBACK_ID =?FEEDBACK_ID AND M.IS_DELETED != 1;";
                        break;
                    }


                case FeedbackSQLCommands.ValidateEventByStudentId:
                    {
                        query = @"    SELECT 
                                                FE.FBSTUDENT_ID,
                                                FE.STUDENT_ID,
                                                FE.CLASS_ID,
                                                FE.STAFF_ID,
                                                SER.EVENT_ID,
                                                FE.QUESTION,
                                                FE.ANSWER,
                                                FE.ISDELETED,
                                                FE.ROLE_ID
                                            FROM
                                                EVENT_FBEVALUATION AS FE
                                                    INNER JOIN
                                                STUDENT_EVENT_REGISTER AS SER ON SER.EVENT_ID = FE.EVENT_ID
                                            WHERE
                                                SER.IS_DELETED != 1
                                                    AND SER.STUDENT_ID = ?STUDENT_ID
                                                    AND SER.EVENT_ID = ?EVENT_ID;";
                        break;
                    }


                case FeedbackSQLCommands.FetchEventObjectiveList:
                    {
                        query = @"                           SELECT 
														efom.OBJECTIVE_ID,
                                                        EFOM.EVENT_ID,
                                                        EFOM.ACADEMIC_YEAR,
                                                        EF.OBJECTIVES,
                                                        EFOM.QUESTION_ID,
                                                        EF.RATING,
                                                        EF.IS_DELETED
                                                    FROM
                                                        EVENT_FB_QUESTION_OBJECTIVE_MAPPING AS EFOM
                                                            INNER JOIN
                                                        EVENT_FBOBJECTIVES AS EF ON EF.OBJECTIVE_ID = EFOM.OBJECTIVE_ID
                                                        inner join
                                                        EVENT_FBQUESTIONS as efq on efq.EVENT_ID=efom.EVENT_ID
                                                    WHERE
                                                        EFOM.IS_DELETED != 1
                                                            AND EF.IS_DELETED != 1
                                                        AND EFOM.EVENT_ID = ?EVENT_ID; ";
                        break;
                    }


                case FeedbackSQLCommands.FetchEventQuestionList:
                    {
                        query = @"        
                                                  SELECT 
                                        EFQM.QUESTION_OBJECT_ID,
                                        EQ.QUESTION,
                                        EQ.ACADEMIC_YEAR,
                                        EFQM.IS_ACTIVE,
                                        EQ.IS_DELETED
                                    FROM
                                        EVENT_FB_QUESTION_OBJECTIVE_MAPPING AS EFQM
                                            INNER JOIN
                                        EVENT_FBQUESTIONS AS EQ ON EQ.QUESTION_ID = EFQM.QUESTION_OBJECT_ID
                                       WHERE 
                                    EQ.IS_DELETED!=1 AND EFQM.EVENT_ID =?EVENT_ID
                                   and  EFQM.IS_DELETED!=1;";
                        break;
                    }



                case FeedbackSQLCommands.FetchEventMappingList:
                    {
                        query = @"              SELECT 
                                            SER.EVENT_ID, EFQ.QUESTION_ID, EFO.OBJECTIVE_ID,EQOM.IS_DELETED
                                        FROM
                                            EVENT_FB_QUESTION_OBJECTIVE_MAPPING AS EQOM
                                                INNER JOIN
                                            EVENT_FBOBJECTIVES AS EFO ON EFO.OBJECTIVE_ID = EQOM.OBJECTIVE_ID
                                                INNER JOIN
                                            EVENT_FBQUESTIONS AS EFQ ON EFQ.QUESTION_ID = EQOM.QUESTION_ID
                                                INNER JOIN
                                            STUDENT_EVENT_REGISTER AS SER ON SER.EVENT_ID = EQOM.EVENT_ID
                                            whERE EQOM.IS_DELETED!=1;  ";
                        break;
                    }
                case FeedbackSQLCommands.FetchFeedBackQuestionViewByFeedbackAndAssessor:
                    {
                        query = @"        
                                    SELECT 
                                FQ.FEEDBACK_ID,
                                FQ.QUESTION_ID,
                                FS.FEEDBACK_NAME,
                                FR.FEEDBACK_ROLE_NAME,
                                FRS.FEEDBACK_ROLE_ID,
                                FQ.QUESTION,
                              --  SQ.QUESTION_TYPE,
                                FG.GROUP_TITLE,
                                FQ.TAMIL_QUESTION
                            FROM
                                FBFEEDBACK_QUESTIONS AS FQ
                                    INNER JOIN
                                FBFEEDBACK_SETTINGS AS FS ON FS.FEEDBACK_ID = FQ.FEEDBACK_ID
                                    AND FS.IS_ACTIVE = 1
                                    AND FS.IS_DELETED != 1
                                    INNER JOIN
                                FBQUESTIONS_GROUP AS FG ON FG.QUESTION_GROUP_ID = FQ.QUESTION_GROUP
                                    AND FG.IS_DELETED != 1
                                   -- INNER JOIN
                              --  SUP_FBFEEDBACK_QUESTION_TYPE AS SQ ON SQ.QUESTION_TYPE_ID = FQ.QUESTION_TYPE
                               --     AND SQ.IS_DELETED != 1
                                    INNER JOIN
                                FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                    AND FRS.IS_DELETED != 1
                                    INNER JOIN
                                SUP_FBFEEDBACK_ROLE AS FR ON FR.SUP_FEEDBACK_ROLE_ID = FRS.SUP_FEEDBACK_ROLE_ID
                                    AND FR.IS_DELETED != 1
                            WHERE
                                FR.ASSESSOR_ROLE=?ASSESSOR_ROLE
                                    AND FQ.FEEDBACK_ID=?FEEDBACK_ID
                                    AND FR.SUP_FEEDBACK_ROLE_ID=?SUP_FEEDBACK_ROLE_ID
                                    AND FQ.IS_DELETED!=1
                                    ORDER BY FQ.QUESTION_ID;";
                        break;
                    }

            }
            return query;
        }
    }
}
