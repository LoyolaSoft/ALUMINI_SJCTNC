using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
using CMS.Utility;
namespace CMS.SQL.Academics
{
    public class AcademicsSQL
    {  
        public static string GetAcademicSQL(AcademicSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                #region Academic Year
                case AcademicSQLCommands.FetchAcademicYearInfo:
                    {
                        query = @"SELECT 
                                        ACADEMIC_YEAR_ID,
                                        AC_YEAR,
                                        DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                        ACTIVE_YEAR,
                                        ACADEMIC_NAME
                                    FROM
                                        ACADEMIC_YEAR
                                    WHERE
                                        IS_DELETED !=1
                                    ORDER BY AC_YEAR desc;";
                        break;
                    }
                case AcademicSQLCommands.FetchActiveYear:
                    {
                        query = @"SELECT 
                                        AC_YEAR,
                                        ACADEMIC_NAME
                                    FROM
                                        ACADEMIC_YEAR
                                    WHERE
                                        ACTIVE_YEAR=1
                                    ORDER BY ACADEMIC_YEAR_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertAcademicYearInfo:
                    {
                        query = @"INSERT INTO ACADEMIC_YEAR(
                                                   AC_YEAR,
                                                    DATE_FROM,
                                                    DATE_TO,
                                                    ACADEMIC_NAME)
                                                     VALUES
                                                     (
                                                        ?AC_YEAR,
                                                        ?DATE_FROM,
                                                        ?DATE_TO,
                                                        ?ACADEMIC_NAME
                                                        );";
                        break;
                    }
                case AcademicSQLCommands.FetchAcademicYearById:
                    {
                        query = @"SELECT 
                                                     AC_YEAR , DATE_FORMAT(DATE_FROM,'%d/%m/%Y') AS DATE_FROM, DATE_FORMAT(DATE_TO,'%d/%m/%Y') AS DATE_TO, ACTIVE_YEAR, ACADEMIC_NAME
                                                FROM
                                                    ACADEMIC_YEAR
                                                WHERE
                                                    ACADEMIC_YEAR_ID=?ACADEMIC_YEAR_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateAcademicInfo:
                    {
                        query = @"UPDATE ACADEMIC_YEAR
                                                SET
                                                AC_YEAR = ?AC_YEAR,
                                                DATE_FROM = ?DATE_FROM,
                                                DATE_TO = ?DATE_TO,
                                                ACADEMIC_NAME = ?ACADEMIC_NAME
                                                WHERE ACADEMIC_YEAR_ID =?ACADEMIC_YEAR_ID ;";
                        break;
                    }
                case AcademicSQLCommands.DeleteAcademicInfo:
                    {
                        query = (@"UPDATE ACADEMIC_YEAR
                                                SET
                                                IS_DELETED =1
                                                WHERE ACADEMIC_YEAR_ID =?ACADEMIC_YEAR_ID ;");
                        break;
                    }
                case AcademicSQLCommands.UpdateActiveAcademic:
                    {
                        query = @"UPDATE ACADEMIC_YEAR
                                                SET
                                                ACTIVE_YEAR =0
                                                WHERE ACADEMIC_YEAR_ID !=?ACADEMIC_YEAR_ID ;";
                        break;
                    }
                case AcademicSQLCommands.UpdateActiveYear:
                    {
                        query = @"UPDATE ACADEMIC_YEAR
                                                SET
                                                ACTIVE_YEAR =1
                                                WHERE ACADEMIC_YEAR_ID =?ACADEMIC_YEAR_ID ;";
                        break;
                    }
                case AcademicSQLCommands.FetchExistingAcademicYear:
                    {
                        query = @"SELECT 
                                        AC_YEAR,
                                        ACADEMIC_NAME
                                    FROM
                                        ACADEMIC_YEAR
                                    WHERE
                                        AC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.FetchCurrentAcademicYearInfo:
                    {
                        query = @"SELECT 
                            ACADEMIC_YEAR_ID,
                            AC_YEAR,
                            DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                            DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                            ACTIVE_YEAR,
                            ACADEMIC_NAME
                        FROM
                            ACADEMIC_YEAR
                        WHERE
                            IS_DELETED != 1 AND AC_YEAR = ?AC_YEAR";
                        break;
                    }
                #endregion

                #region Course Root
                case AcademicSQLCommands.FetchCourseRootInfo:
                    {
                        query = @"SELECT 
                                        RT.COURSE_ROOT_ID,
                                        DT.DEPARTMENT,
                                        RT.YEAR,
                                        SP.PART,
                                        CT.COURSE_TYPE,
                                        RT.COURSE_TITLE,
                                        RT.COURSE_CODE,
                                        SS.SEMESTER_NAME,
                                        RT.IS_NME_SUBJECT,
                                        IS_ALLIED_SUBJECT,
                                        UGC.UGC_COURSE_TYPE
                                    FROM
                                        CP_COURSE_ROOT AS RT
                                            LEFT JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = RT.SEMESTER_ID
                                            LEFT JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = RT.DEPARTMENT
                                            LEFT JOIN
                                        SUP_PART AS SP ON SP.PART_ID = RT.PART
                                            LEFT JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = RT.COURSE_TYPE
		                                    LEFT JOIN
	                                    CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID=RT.UGC_COURSE_TYPE
                                    WHERE
                                        RT.IS_DELETED != 1
                                    ORDER BY RT.COURSE_ROOT_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseRootInfoByAcYearAndDepartmentAndSemesterType:
                    {
                        query = @"SELECT 
                                        RT.COURSE_ROOT_ID,
                                        DT.DEPARTMENT,
                                        RT.YEAR,
                                        SP.PART,
                                        CT.COURSE_TYPE,
                                        RT.COURSE_TITLE,
                                        RT.COURSE_CODE,
                                        SS.SEMESTER_NAME,
                                        RT.IS_NME_SUBJECT,
                                        IS_ALLIED_SUBJECT,
                                        UGC.UGC_COURSE_TYPE
                                    FROM
                                        CP_COURSE_ROOT AS RT
                                            LEFT JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = RT.SEMESTER_ID
                                            LEFT JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = RT.DEPARTMENT
                                            LEFT JOIN
                                        SUP_PART AS SP ON SP.PART_ID = RT.PART
                                            LEFT JOIN
                                        CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = RT.COURSE_TYPE
		                                    LEFT JOIN
	                                    CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID=RT.UGC_COURSE_TYPE
                                        
                                        
                                    WHERE
                                        RT.IS_DELETED != 1
                                        AND RT.ACADEMIC_YEAR=?AC_YEAR
                                        AND RT.DEPARTMENT=?DEPARTMENT
                                        AND RT.SEMESTER_ID=?SEMESTER
                                    ORDER BY RT.COURSE_ROOT_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseInfoByIdForCoe:
                    {
                        query = @"SELECT 
                                        RT.COURSE_ROOT_ID,
                                        RT.DEPARTMENT,
                                        A.ACADEMIC_YEAR_ID,
                                        RT.SEMESTER_ID AS SEMESTER
                                       
                                    FROM
                                        CP_COURSE_ROOT AS RT
                                            INNER JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = RT.SEMESTER_ID
										
                                         INNER JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = RT.DEPARTMENT
                                        
                                        INNER JOIN 
                                        
                                        ACADEMIC_YEAR AS A ON A.AC_YEAR=RT.ACADEMIC_YEAR
                                        AND A.IS_DELETED!=1
                                       
                                    WHERE
                                        RT.IS_DELETED != 1
                                        AND RT.COURSE_ROOT_ID=?COURSE_ROOT_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertCourseRootInfo:
                    {
                        query = @"INSERT INTO CP_COURSE_ROOT
                                                                (
                                                                YEAR_FROM,
                                                                YEAR_TO,
                                                                DEPARTMENT,
                                                                YEAR,
                                                                PART,
                                                                COURSE_TYPE,
                                                                HRS_PER_WEEK,
                                                                CREDITS,
                                                                PAPER_CODE,
                                                                COURSE_TITLE,
                                                                COURSE_CODE,
                                                                SEMESTER_ID,
                                                                IS_NME_SUBJECT,
                                                                IS_ALLIED_SUBJECT,
                                                                COURSE_ORDER,
                                                                SUBJECTS,
                                                                IS_COMPULSORY,
                                                                UGC_COURSE_TYPE,
                                                                PAPER_TYPE,
                                                                SUBJECT_TYPE,ACADEMIC_YEAR,CONS_COURSE_TITLE,INTERNAL,EXTERNAL,TOTAL)
                                                                VALUES
                                                                (?YEAR_FROM,
                                                                ?YEAR_TO,
                                                                ?DEPARTMENT,
                                                                ?AC_YEAR,
                                                                ?PART,
                                                                ?COURSE_TYPE,
                                                                ?HRS_PER_WEEK,
                                                                ?CREDITS,
                                                                ?PAPER_CODE,
                                                                ?COURSE_TITLE,
                                                                ?COURSE_CODE,
                                                                ?SEMESTER_ID,
                                                                ?IS_NME_SUBJECT,
                                                                ?IS_ALLIED_SUBJECT,
                                                                ?COURSE_ORDER,
                                                                ?SUBJECTS,
                                                                ?IS_COMPULSORY,
                                                                ?UGC_COURSE_TYPE,
                                                                ?PAPER_TYPE,
                                                                ?SUBJECT_TYPE,?AC_YEAR,?CONS_COURSE_TITLE,?INTERNAL,?EXTERNAL,?TOTAL);";
                        break;
                    }
                case AcademicSQLCommands.DeleteCourseRootInfo:
                    {
                        query = @"UPDATE CP_COURSE_ROOT
                                                            SET
                                                            IS_DELETED = 1
                                                            WHERE COURSE_ROOT_ID =?COURSE_ROOT_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseRootInfoById:
                    {
                        query = @"SELECT 
	                                  COURSE_ROOT_ID,
                                      YEAR_FROM,
                                      YEAR_TO,
                                      DEPARTMENT,
                                      YEAR,
                                      SEMESTER_FROM,
                                      SEMESTER_TO,
                                      PART,
                                      COURSE_TYPE,
                                      HRS_PER_WEEK,
                                      CREDITS,
                                      OPTION_NAME,
                                      PAPER_CODE,
                                      COURSE_TITLE,
                                      CONS_COURSE_TITLE,
                                      COURSE_CODE,
                                      SEMESTER_ID,
                                      IS_NME_SUBJECT,
                                      IS_ALLIED_SUBJECT,
                                      COURSE_ORDER,
                                      SUBJECTS,
                                      IS_COMPULSORY,
                                      UGC_COURSE_TYPE,
                                      PAPER_TYPE,
                                      SUBJECT_TYPE  ,INTERNAL,EXTERNAL,TOTAL
                                  FROM CP_COURSE_ROOT WHERE COURSE_ROOT_ID=?COURSE_ROOT_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateCourseRootInfo:
                    {
                        query = @"UPDATE CP_COURSE_ROOT
                                                            SET
                                                            YEAR_FROM= ?YEAR_FROM,
                                                            YEAR_TO= ?YEAR_TO,
                                                            DEPARTMENT= ?DEPARTMENT,                                                            
                                                            PART= ?PART,
                                                            COURSE_TYPE= ?COURSE_TYPE,
                                                            HRS_PER_WEEK= ?HRS_PER_WEEK,
                                                            CREDITS= ?CREDITS,
                                                            PAPER_CODE= ?PAPER_CODE,
                                                            COURSE_TITLE= ?COURSE_TITLE,
                                                            COURSE_CODE= ?COURSE_CODE,
                                                            SEMESTER_ID= ?SEMESTER_ID,
                                                            IS_NME_SUBJECT= ?IS_NME_SUBJECT,
                                                            IS_ALLIED_SUBJECT= ?IS_ALLIED_SUBJECT,
                                                            COURSE_ORDER= ?COURSE_ORDER,
                                                            SUBJECTS= ?SUBJECTS,
                                                            IS_COMPULSORY= ?IS_COMPULSORY,
                                                            UGC_COURSE_TYPE= ?UGC_COURSE_TYPE,
                                                            PAPER_TYPE=?PAPER_TYPE,
                                                            SUBJECT_TYPE=?SUBJECT_TYPE,
                                                            CONS_COURSE_TITLE=?CONS_COURSE_TITLE,
                                                            INTERNAL=?INTERNAL,
                                                            EXTERNAL=?EXTERNAL,
                                                            TOTAL=?TOTAL
                                                            WHERE COURSE_ROOT_ID=?COURSE_ROOT_ID;";
                        break;
                    }
                case AcademicSQLCommands.CheckCourseRootInfo:
                    {
                        query = @"SELECT 
	                                  COURSE_ROOT_ID,
                                      YEAR_FROM,
                                      YEAR_TO,
                                      DEPARTMENT,
                                      YEAR,
                                      SEMESTER_FROM,
                                      SEMESTER_TO,
                                      PART,
                                      COURSE_TYPE,
                                      HRS_PER_WEEK,
                                      CREDITS,
                                      OPTION_NAME,
                                      PAPER_CODE,
                                      COURSE_TITLE,
                                      COURSE_CODE,
                                      SEMESTER_ID,
                                      IS_NME_SUBJECT,
                                      IS_ALLIED_SUBJECT,
                                      COURSE_ORDER,
                                      SUBJECTS,
                                      IS_COMPULSORY,
                                      UGC_COURSE_TYPE,CONS_COURSE_TITLE,INTERNAL,EXTERNAL,TOTAL
                                  FROM CP_COURSE_ROOT WHERE COURSE_CODE=?COURSE_CODE;";
                        break;
                    }
                #endregion

                #region Course Wise Staff
                case AcademicSQLCommands.FetchCourseWiseStaffInfo:
                    {
                        query = @"SELECT 
                                        CCS.CLSCRSSTF_ID,
                                        CCS.CLASS_ID,
                                        CCS.COURSE_ID,
                                        CLS.CLASS_NAME,
                                        CLS.CLASS_CODE,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        GROUP_CONCAT(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                    '(',
                                                    IFNULL(SP.STAFF_CODE, ''),
                                                    ')')) AS STAFF_NAME,
                                        COUNT(CCS.STAFF_ID) AS STAFF_COUNT,
                                        CCS.SHIFT,
                                        CCS.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CCS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CCS.CLASS_ID
                                    WHERE CCS.IS_DELETED!=1 
                                        AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                        AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                        GROUP BY CCS.CLASS_ID , CCS.COURSE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseWiseStaffInfoByDepartmentId:
                    {
                        query = @"SELECT 
                                        CCS.CLSCRSSTF_ID,
                                        CCS.CLASS_ID,
                                        CLS.CLASS_NAME,
                                        CCS.COURSE_ID,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        GROUP_CONCAT(CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                    '(',
                                                    IFNULL(SP.STAFF_CODE, ''),
                                                    ')')) AS STAFF_ID,
                                        COUNT(CCS.STAFF_ID) AS STAFF_COUNT,
                                        CCS.SEMESTER_ID,
                                        CCS.SHIFT
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CCS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CCS.CLASS_ID
                                            AND CLS.DEPARTMENT=?DEPARTMENT
                                            AND CLS.SHIFT IN (?SHIFT)
                                    WHERE CCS.IS_DELETED!=1 
                                            AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                            AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                    GROUP BY CCS.CLASS_ID , CCS.COURSE_ID;";
                        break;
                    }
                case AcademicSQLCommands.IsCourseWiseStaffExisting:
                    {
                        query = @"SELECT 
                                        CLSCRSSTF_ID,IFNULL(IS_PRIMARY_STAFF,'0') AS IS_PRIMARY_STAFF
                                    FROM
                                        CP_CLASS_COURSE_STAFF
                                    WHERE
                                        CLASS_ID =?CLASS_ID
                                        AND COURSE_ID =?COURSE_ID AND STAFF_ID=?STAFF_ID AND IS_DELETED!=1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.DeleteCourseWiseStaffInfo:
                    {
                        query = @"UPDATE CP_CLASS_COURSE_STAFF SET IS_DELETED=1 WHERE CLSCRSSTF_ID=?CLSCRSSTF_ID";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseWiseStaffInfoById:
                    {
                        query = @"SELECT 
                                    CLSCRSSTF_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    STAFF_ID,
                                    HRS_WEEK,
                                    HRS_TERM,
                                    SHIFT,
                                    SEMESTER_ID,
                                    IS_PRIMARY_STAFF
                                FROM
                                    CP_CLASS_COURSE_STAFF
                                WHERE
                                    CLSCRSSTF_ID = ?CLSCRSSTF_ID AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.GetClassListByClassTypeId:
                    {
                        query = @" SELECT 
                                        CLASS_ID,CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        DEPARTMENT =?DEPARTMENT 
		                                    AND 
                                        SHIFT =?SHIFT
		                                    AND 
                                        CLASS_TYPE = ?CLASS_TYPE;";
                        break;
                    }
                case AcademicSQLCommands.GetClassTypeByShiftId:
                    {
                        query = @"SELECT 
                                        CLASS_TYPE_ID, CLASS_TYPE
                                    FROM
                                        SUP_CLASS_TYPE;";
                        break;
                    }
                case AcademicSQLCommands.FetchEthicsPaperByClassId:                    
                    {
                        query = @"SELECT 
                                         CLS.CLASS_ID,
                                        CLS.CLASS_CODE AS CLASS_NAME,
                                        CCP.COURSE_ID,
                                        CR.COURSE_TITLE,
                                        CS.HRS_TERM,
                                        CS.HRS_WEEK,
                                        CR.COURSE_CODE,
                                        GROUP_CONCAT(CS.STAFF_ID) AS STAFF_ID,
                                        SEM.SEMESTER_NAME AS SEMESTER,
                                        SEM.SUP_SEMESTER_ID AS SEMESTER_ID
                                    FROM
                                        CP_COURSE_PATTERN AS CCP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CR.IS_DELETED!=1
                                            AND CR.IS_NME_SUBJECT != 1
                                            LEFT JOIN
                                        CP_CLASS_COURSE_STAFF AS CS ON CS.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.CLASS_ID = ?CLASS_ID
                                            AND CS.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = ?CLASS_ID
                                            AND CLS.CLASS_TYPE = ?CLASS_TYPE
                                            INNER JOIN
                                        SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                    WHERE
                                        CCP.PROGRAMME_GROUP_ID = (SELECT 
                                                    PROGRAMME_GROUP_ID
                                                FROM
                                                    CP_CLASSES 
                                                WHERE
                                                    CLASS_ID = ?CLASS_ID)
                                            AND CCP.IS_DELETED != 1
                                            AND CCP.ACADEMIC_YEAR= ?AC_YEAR
                                            AND CR.PAPER_TYPE=6
                                    GROUP BY CCP.COURSE_ID ;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseWiseStaffInfoByClassId:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CLASS_CODE,
                                        COURSE_ID,
                                        COURSE_TITLE,
                                        HRS_TERM,
                                        HRS_WEEK,
                                        COURSE_CODE,
                                        STAFF_NAME,
                                        SEMESTER_NAME,
                                        SUP_SEMESTER_ID
                                    FROM
                                        (SELECT 
                                            CLS.CLASS_ID,
                                                CLS.CLASS_CODE,
                                                CCP.COURSE_ID,
                                                CR.COURSE_TITLE,
                                                '' AS 'HRS_TERM',
                                                CR.HRS_PER_WEEK AS 'HRS_WEEK',
                                                CR.COURSE_CODE,
                                                GROUP_CONCAT(CS.STAFF_ID) AS STAFF_NAME,
                                                SS.SEMESTER_NAME,
                                                SS.SUP_SEMESTER_ID,
                                                CR.PART,
                                                CR.COURSE_ORDER
                                        FROM
                                            CP_COURSE_PATTERN AS CCP
                                        INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CR.IS_NME_SUBJECT != 1
                                        INNER JOIN CP_CLASSES AS CLS ON CLS.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                        LEFT JOIN CP_CLASS_COURSE_STAFF AS CS ON CS.COURSE_ID = CR.COURSE_ROOT_ID
                                            AND CS.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                            AND CS.CLASS_ID = CLS.CLASS_ID
                                            AND CS.IS_DELETED != 1
                                        INNER JOIN ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = CLS.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                                ACADEMIC_YEAR_ID
                                            FROM
                                                ACADEMIC_YEAR
                                            WHERE
                                                ACTIVE_YEAR = 1)
                                        INNER JOIN ACADEMIC_SEMESTER AS SEM ON SEM.ACADEMIC_YEAR = CCP.ACADEMIC_YEAR
                                            AND SEM.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                            AND SEM.SEMESTER = CR.SEMESTER_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.IS_DELETED != 1
                                        INNER JOIN SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SEM.SEMESTER
                                        WHERE
                                            CCP.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SEM.IS_ACTIVE = 1
                                                AND CLS.CLASS_ID = ?CLASS_ID
                                                AND CLS.CLASS_TYPE = ?CLASS_TYPE
                                                AND CCP.IS_DELETED != 1
                                        GROUP BY CCP.COURSE_ID UNION SELECT 
                                            CLS.CLASS_ID,
                                                CLS.CLASS_CODE,
                                                CLS.COURSE_ID,
                                                CR.COURSE_TITLE,
                                                CS.HRS_TERM,
                                                CS.HRS_WEEK,
                                                CR.COURSE_CODE,
                                                GROUP_CONCAT(CS.STAFF_ID) AS STAFF_NAME,
                                                SS.SEMESTER_NAME,
                                                SS.SUP_SEMESTER_ID,
                                                CR.PART,
                                                CR.COURSE_ORDER
                                        FROM
                                            CP_CLASSES AS CLS
                                        INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CLS.COURSE_ID
                                        LEFT JOIN CP_CLASS_COURSE_STAFF AS CS ON CS.COURSE_ID = CLS.COURSE_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.CLASS_ID = CLS.CLASS_ID
                                            AND CS.IS_DELETED != 1
                                        INNER JOIN ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = CLS.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                                ACADEMIC_YEAR_ID
                                            FROM
                                                ACADEMIC_YEAR
                                            WHERE
                                                ACTIVE_YEAR = 1)
                                        INNER JOIN ACADEMIC_SEMESTER AS SEM ON SEM.BATCH = AB.BATCH_ID
                                            AND SEM.SEMESTER = CR.SEMESTER_ID
                                            AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                        INNER JOIN SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                        WHERE
                                            CLS.CLASS_ID = ?CLASS_ID AND SEM.IS_ACTIVE = 1
                                                AND CLS.CLASS_TYPE = ?CLASS_TYPE
                                        GROUP BY CS.CLASS_ID , CS.COURSE_ID) AS T
                                    ORDER BY PART , COURSE_ORDER;";
                        //query = @"SELECT 
                        //                CLS.CLASS_ID,
                        //                CLS.CLASS_CODE,
                        //                CCP.COURSE_ID,
                        //                CR.COURSE_TITLE,
                        //                CS.HRS_TERM,
                        //                CS.HRS_WEEK,
                        //                CR.COURSE_CODE,
                        //                GROUP_CONCAT(CS.STAFF_ID) AS STAFF_NAME,
                        //                SEM.SEMESTER_NAME,
                        //                SEM.SUP_SEMESTER_ID
                        //            FROM
                        //                CP_COURSE_PATTERN AS CCP
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCP.COURSE_ID
                        //                    AND CR.COURSE_ROOT_ID NOT IN (SELECT 
                        //                        CLS.COURSE_ID
                        //                    FROM
                        //                        CP_CLASSES AS CLS
                        //                    WHERE
                        //                        CLS.COURSE_ID != '')
                        //                    AND CR.IS_NME_SUBJECT != 1
                        //                    LEFT JOIN
                        //                CP_CLASS_COURSE_STAFF AS CS ON CS.COURSE_ID = CR.COURSE_ROOT_ID
                        //                    AND CS.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND CS.CLASS_ID = ?CLASS_ID
                        //                    AND CS.IS_DELETED != 1
                        //                    LEFT JOIN
                        //                CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                        //                    AND CLS.CLASS_TYPE = ?CLASS_TYPE
                        //                    INNER JOIN
                        //                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CR.SEMESTER_ID
                        //            WHERE
                        //                CCP.PROGRAMME_GROUP_ID = (SELECT 
                        //                            PROGRAMME_GROUP_ID
                        //                        FROM
                        //                            CP_CLASSES 
                        //                        WHERE
                        //                            CLASS_ID = ?CLASS_ID)
                        //                    AND CR.SEMESTER_ID IN (SELECT 
                        //                        ASE.SEMESTER
                        //                    FROM
                        //                        ACADEMIC_SEMESTER AS ASE
                        //                            INNER JOIN
                        //                        CP_CLASSES AS CLS ON CLS.PROGRAMME_GROUP_ID = ASE.PROGRAMME_GROUP_ID
                        //                            AND CLS.CLASS_ID = ?CLASS_ID
                        //                            INNER JOIN
                        //                        STU_CLASS AS SC ON SC.CLASS_ID = CLS.CLASS_ID
                        //                            AND SC.BATCH = ASE.BATCH
                        //                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    WHERE
                        //                        SC.CLASS_ID = ?CLASS_ID
                        //                            AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                        //                            AND ASE.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                        //                            AND ASE.IS_ACTIVE = 1
                        //                    GROUP BY ASE.SEMESTER)
                        //                    AND CCP.IS_DELETED != 1
                        //                    AND CCP.ACADEMIC_YEAR= ?AC_YEAR
                        //            GROUP BY CCP.COURSE_ID 
                        //            UNION SELECT 
                        //                CLS.CLASS_ID,
                        //                CLS.CLASS_CODE,
                        //                CLS.COURSE_ID,
                        //                CR.COURSE_TITLE,
                        //                CS.HRS_TERM,
                        //                CS.HRS_WEEK,
                        //                CR.COURSE_CODE,
                        //                GROUP_CONCAT(CS.STAFF_ID) AS STAFF_NAME,
                        //                SEM.SEMESTER_NAME,
                        //                SEM.SUP_SEMESTER_ID
                        //            FROM
                        //                CP_CLASSES AS CLS
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CLS.COURSE_ID
                        //                    LEFT JOIN
                        //                CP_CLASS_COURSE_STAFF AS CS ON CS.COURSE_ID = CLS.COURSE_ID
                        //                    AND CS.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND CS.CLASS_ID = CLS.CLASS_ID
                        //                    AND CS.IS_DELETED != 1
                        //                    INNER JOIN
                        //                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CR.SEMESTER_ID
                        //            WHERE
                        //                CLS.CLASS_ID = ?CLASS_ID
                        //                    AND CLS.CLASS_TYPE = ?CLASS_TYPE
                        //            GROUP BY CS.CLASS_ID , CS.COURSE_ID;";
                        break;
                    }
                case AcademicSQLCommands.SaveCourseWiseStaff:
                    {
                        query = @"INSERT INTO CP_CLASS_COURSE_STAFF
                                    (
                                    CLASS_ID,
                                    COURSE_ID,
                                    STAFF_ID,
                                    SHIFT,
                                    SEMESTER_ID,
                                    SEMESTER_TYPE_ID,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (?CLASS_ID,?COURSE_ID,?STAFF_ID,?SHIFT,?SEMESTER_ID,?SEMESTER_TYPE_ID,?AC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.UpdateCourseWiseStaff:
                    {
                        query = @"UPDATE CP_CLASS_COURSE_STAFF SET                                     
                                    CLASS_ID = ?CLASS_ID,
                                    COURSE_ID = ?COURSE_ID,
                                    STAFF_ID = ?STAFF_ID,
                                    SHIFT = ?SHIFT,
                                    IS_PRIMARY_STAFF=?IS_PRIMARY_STAFF,
                                    SEMESTER_ID = ?SEMESTER_ID,
                                    IS_DELETED=?IS_DELETED
                                 
                                WHERE CLSCRSSTF_ID = ?CLSCRSSTF_ID;";
                        break;
                    }
                #endregion

                #region Academic Curriculum
                case AcademicSQLCommands.UpdateAcademicCurriculumInfo:
                    {
                        query = @"UPDATE  ACADEMIC_CURRICULUM_?AC_YEAR SET                                     
                                    PROGRAMME=?PROGRAMME,
                                    BATCH=?BATCH,
                                    COURSE_ID=?COURSE_ID,
                                    IS_ACTIVE=?IS_ACTIVE WHERE CURRICULUM_ID=?CURRICULUM_ID";
                        break;
                    }
                case AcademicSQLCommands.SaveAcademicCurriculum:
                    {
                        query = @"INSERT INTO ACADEMIC_CURRICULUM_?AC_YEAR
                                    (
                                    PROGRAMME,
                                    BATCH,
                                    COURSE_ID,
                                    IS_ACTIVE,
                                    SEMESTER)
                                    VALUES
                                    (?PROGRAMME,?BATCH,?COURSE_ID,?IS_ACTIVE,(SELECT SEMESTER_ID FROM CP_COURSE_ROOT_?AC_YEAR AS CR  WHERE CR.COURSE_ROOT_ID=?COURSE_ID));";
                        break;
                    }
                case AcademicSQLCommands.IsAcademicCurriculumExisting:
                    {
                        query = @"SELECT 
                                        CURRICULUM_ID
                                    FROM
                                        ACADEMIC_CURRICULUM_?AC_YEAR AS AC
                                    WHERE
                                        PROGRAMME =?PROGRAMME AND BATCH =?BATCH
                                            AND COURSE_ID =?COURSE_ID AND AC.IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.CheckActiveCurriculum:
                    {
                        query = @"UPDATE ACADEMIC_CURRICULUM_?AC_YEAR  SET IS_ACTIVE='0' WHERE PROGRAMME='?PROGRAMME' AND BATCH='?BATCH';";
                        break;
                    }
                case AcademicSQLCommands.FetchAcademicCurriculumInfo:
                    {
                        query = @"SELECT 
                                CUR.CURRICULUM_ID,
                                PR.PROGRAMME_CODE,
                                SBT.BATCH,
                                CONCAT(IFNULL(COURSE_TITLE, ''),
                                        '(',
                                        IFNULL(COURSE_CODE, ''),
                                        ')') AS COURSE_CODE,
                                RT.COURSE_TITLE,
                                SEM.SEMESTER_NAME
                            FROM
                                ACADEMIC_CURRICULUM_2018 AS CUR
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = CUR.PROGRAMME
                                    INNER JOIN
                                CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CUR.COURSE_ID
                                    AND RT.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CUR.SEMESTER
                                    INNER JOIN
                                SUP_BATCHES AS SBT ON SBT.BATCH_ID = CUR.BATCH
                            WHERE
                                CUR.IS_DELETED != 1
                                    AND CUR.SEMESTER IN (SELECT 
                                        SEMESTER
                                    FROM
                                        ACADEMIC_SEMESTER
                                    WHERE
                                        IS_ACTIVE = 1 AND ACADEMIC_YEAR =?AC_YEAR
                                    GROUP BY SEMESTER);";
                        break;
                    }
                case AcademicSQLCommands.FetchCurriculumCourseInfoById:
                    {
                        query = @"SELECT 
                                    CUR.CURRICULUM_ID,
                                    PR.PROGRAMME_ID,
                                    PR.PROGRAMME_CODE,
                                    SBT.BATCH_ID,
                                    SBT.BATCH,
                                    RT.COURSE_ROOT_ID,
                                    RT.COURSE_CODE,
                                    RT.COURSE_TITLE,
                                    SEM.SUP_SEMESTER_ID,
                                    SEM.SEMESTER_NAME
                                FROM
                                    ACADEMIC_CURRICULUM_?AC_YEAR AS CUR
                                        INNER JOIN
                                    CP_PROGRAMME_?AC_YEAR AS PR ON PR.PROGRAMME_ID = CUR.PROGRAMME
                                        INNER JOIN
                                    CP_COURSE_ROOT_?AC_YEAR AS RT ON RT.COURSE_ROOT_ID = CUR.COURSE_ID
		                                INNER JOIN
	                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID=CUR.SEMESTER
										INNER JOIN 
									SUP_BATCHES AS SBT ON SBT.BATCH_ID=CUR.BATCH WHERE CUR.IS_DELETED=0 AND CUR.PROGRAMME=?PROGRAMME AND CUR.BATCH=?BATCH;";
                        break;
                    }
                case AcademicSQLCommands.DeleteCurriculumInfo:
                    {
                        query = @"UPDATE ACADEMIC_CURRICULUM_?AC_YEAR SET IS_DELETED=1 WHERE CURRICULUM_ID=?CURRICULUM_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseBySemester:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE,
                                    IF(AC.CURRICULUM_ID IS NULL OR  AC.IS_ACTIVE=0,
                                        '',
                                        'SELECTED') AS SELECTED
                                FROM
                                    CP_COURSE_ROOT_?AC_YEAR AS CR
                                        LEFT JOIN
                                    ACADEMIC_CURRICULUM_?AC_YEAR AS AC ON AC.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND AC.BATCH =?BATCH
                                        AND AC.PROGRAMME =?PROGRAMME AND AC.IS_DELETED!=1
                                WHERE
                                    SEMESTER_ID = (SELECT 
                                            SEMESTER
                                        FROM
                                            ACADEMIC_SEMESTER_?AC_YEAR as AC
                                        WHERE
                                            BATCH =?BATCH AND PROGRAMME =?PROGRAMME
                                                AND IS_ACTIVE = 1 AND AC.IS_DELETED!=1);";
                        break;

                    }
                case AcademicSQLCommands.FetchSelectedCourseWiseStaff:
                    {
                        query = @"SELECT 
                                        CCS.CLSCRSSTF_ID,
                                        CCS.CLASS_ID,
                                        CCS.COURSE_ID,
                                        CLS.CLASS_NAME,
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE,
                                        CCS.SHIFT,
                                        CR.SEMESTER_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '(',
                                                IFNULL(SP.STAFF_CODE, ''),
                                                ')') AS STAFF_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CCS
                                            LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CCS.CLASS_ID
                                            LEFT JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                    WHERE
                                        CCS.CLASS_ID =?CLASS_ID
                                        AND CCS.COURSE_ID =?COURSE_ID AND CCS.ACADEMIC_YEAR=?AC_YEAR AND CCS.IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region Class Course
                case AcademicSQLCommands.IsClassCourseExisting:
                    {

                        query = @"SELECT 
                                    COUNT(CLASS_COURSE_ID) > 0 AS IS_EXIST
                                FROM
                                    CP_CLASS_COURSE
                                WHERE
                                    CLASS_ID =?CLASS_ID AND COURSE_ID =?COURSE_ID
                                        AND IS_DELETED != 1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.DeleteClassCourseInfo:
                    {

                        query = @"UPDATE CP_CLASS_COURSE
                                    SET
                                    IS_DELETED =1
                                    WHERE CLASS_COURSE_ID = ?CLASS_COURSE_ID;";
                        break;
                    }
                case AcademicSQLCommands.SaveClassCourseInfo:
                    {

                        query = @"INSERT INTO CP_CLASS_COURSE
                                    (CLASS_ID,
                                    COURSE_ID,
                                    ACADEMIC_YEAR)
                                    VALUES
                                    (?CLASS_ID,?COURSE_ID,?ACADEMIC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.UpdateClassCourseInfo:
                    {

                        query = @"UPDATE CP_CLASS_COURSE
                                    SET
                                    CLASS_ID = ?CLASS_ID,
                                    COURSE_ID = ?COURSE_ID
                                    WHERE CLASS_COURSE_ID = ?CLASS_COURSE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseListByClassId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(IFNULL(CR.COURSE_CODE, ''),
                                                        '  ( ',
                                                        IFNULL(CR.COURSE_TITLE, ''),
                                                        ' )')) AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_PATTERN AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                            -- AND CR.IS_COMPULSORY = 1
                                            AND CR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                            AND CLS.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_YEAR AS AY ON AY.AC_YEAR = CP.ACADEMIC_YEAR
                                            AND AY.ACTIVE_YEAR = 1
                                            AND AY.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                            AND AB.BATCH_YEAR = CLS.CLASS_YEAR
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                            AND SEM.SEMESTER = CR.SEMESTER_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.IS_DELETED != 1
                                            AND SEM.IS_ACTIVE = 1
                                             AND SEM.ACADEMIC_YEAR=CP.ACADEMIC_YEAR
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CLS.CLASS_ID = ?CLASS_ID
                                            AND CP.IS_DELETED != 1;";

                        break;
                    }
                case AcademicSQLCommands.FetchClassesByClassTypeId:
                    {
                        query = @"SELECT CLASS_ID,CLASS_NAME FROM CP_CLASSES_?AC_YEAR AS CLS WHERE CLS.CLASS_TYPE=?CLASS_TYPE and CLS.IS_DELETED!=1;";
                        break;
                    }

                case AcademicSQLCommands.FetchClassCourseByClassCourseId:
                    {
                        query = @"SELECT 
                                    CLASS_ID, COURSE_ID
                                FROM
                                    CP_CLASS_COURSE
                                WHERE
                                    CLASS_COURSE_ID =?CLASS_COURSE_ID  AND IS_DELETED != 1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.FetchClassCourseList:
                    {
                        //query = @"SELECT 
                        //                CLASS_COURSE_ID,
                        //                CC.CLASS_ID,
                        //                CC.COURSE_ID,
                        //                CT.COURSE_TYPE,
                        //                CR.COURSE_TITLE,
                        //                CR.COURSE_CODE,
                        //                IF(CR.IS_NME_SUBJECT = 1, 'YES', 'NO') AS IS_NME_SUBJECT,
                        //                IF(CR.IS_ALLIED_SUBJECT = 1,
                        //                    'YES',
                        //                    'NO') AS IS_ALLIED_SUBJECT,
                        //                CLS.CLASS_NAME,
                        //                SH.SHIFT_NAME,
                        //                SEM.SEMESTER_NAME
                        //            FROM
                        //                CP_CLASS_COURSE_?AC_YEAR AS CC
                        //                    INNER JOIN
                        //                CP_COURSE_ROOT_?AC_YEAR AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                        //                    INNER JOIN
                        //                CP_CLASSES_?AC_YEAR AS CLS ON CLS.CLASS_ID = CC.CLASS_ID
                        //                    INNER JOIN
                        //                CP_COURSE_TYPE_?AC_YEAR AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                        //                    LEFT JOIN
                        //                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CR.SEMESTER_ID
                        //                    LEFT JOIN
                        //                SUP_SHIFT AS SH ON SH.SHIFT_ID = CLS.SHIFT
                        //                    LEFT JOIN
                        //                CP_PROGRAMME_?AC_YEAR AS PRO ON PRO.PROGRAMME_ID = CLS.PROGRAMME
                        //                    INNER JOIN
                        //                ACADEMIC_SEMESTER_?AC_YEAR AS ASE ON ASE.PROGRAMME = CLS.PROGRAMME
                        //                    AND ASE.IS_ACTIVE = 1
                        //                    AND ASE.BATCH = (SELECT 
                        //                        SP.BATCH
                        //                    FROM
                        //                        STU_CLASS AS SC
                        //                            INNER JOIN
                        //                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                    WHERE
                        //                        SC.CLASS_ID = ?CLASS_ID
                        //                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    GROUP BY SP.BATCH)
                        //            WHERE
                        //                CLS.CLASS_ID = ?CLASS_ID AND CC.IS_DELETED != 1
                        //                    AND CR.SEMESTER_ID = ASE.SEMESTER;";
                        query = @"SELECT 
                                    CLASS_COURSE_ID,
                                    CC.CLASS_ID,
                                    CC.COURSE_ID,
                                    CT.COURSE_TYPE,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    IF(CR.IS_NME_SUBJECT = 1, 'YES', 'NO') AS IS_NME_SUBJECT,
                                    IF(CR.IS_ALLIED_SUBJECT = 1,
                                        'YES',
                                        'NO') AS IS_ALLIED_SUBJECT,
                                    CLS.CLASS_NAME,
                                    SH.SHIFT_NAME,
                                    SEM.SEMESTER_NAME
                                FROM
                                    CP_CLASS_COURSE AS CC
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                        INNER JOIN
                                    CP_CLASSES AS CLS ON CLS.CLASS_ID = CC.CLASS_ID
                                        INNER JOIN
                                    CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                        LEFT JOIN
                                    SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CR.SEMESTER_ID
                                        LEFT JOIN
                                    SUP_SHIFT AS SH ON SH.SHIFT_ID = CLS.SHIFT
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS ASE ON ASE.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                        AND CR.SEMESTER_ID = ASE.SEMESTER
                                        AND ASE.IS_DELETED!=1
                                        AND ASE.IS_ACTIVE = 1
                                        AND ASE.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    CLS.CLASS_ID = ?CLASS_ID
                                        AND CC.IS_DELETED != 1
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                GROUP BY CC.COURSE_ID;";
                        break;
                    }
                #endregion

                #region Semester Root
                case AcademicSQLCommands.FetchSemesterRootInfo:
                    {
                        query = @"SELECT 
                                        SR.ACC_SEMESTER_ID,
                                        SB.BATCH,
                                        P.PROGRAMME_NAME,
                                        SS.SEMESTER_NAME,
                                        DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                        DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                        SR.IS_ACTIVE
                                    FROM
                                        ACADEMIC_SEMESTER AS SR
                                            INNER JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = SR.BATCH
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CP ON CP.PROGRAMME_GROUP_ID = SR.PROGRAMME_GROUP_ID
		                                    INNER JOIN
	                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID=CP.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SR.SEMESTER
                                    WHERE
                                        SR.IS_DELETED != 1 AND SR.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                //case AcademicSQLCommands.UpdateActiveSemester:
                //    {
                //        query = @"UPDATE CP_SEMESTER_ROOT_ 
                //                                SET 
                //                                    IS_ACTIVE = IF(IS_ACTIVE = 0,
                //                                        1,
                //                                        IF(IS_ACTIVE = 1, 0, IS_ACTIVE))
                //                                WHERE
                //                                    SEMESTER_ID =?SEMESTER_ID;";
                //        break;
                //    }
                case AcademicSQLCommands.GetSemesterId:
                    {
                        query = @"SELECT 
                                        SEM.SUP_SEMESTER_ID,
                                        SEM.SEMESTER_NAME,
                                        IF(SR.SEMESTER IS NULL OR  SR.IS_ACTIVE=0,
                                        '',
                                        'SELECTED') AS SELECTED
                                    FROM
                                        ACADEMIC_SEMESTER AS SR
                                            INNER JOIN
                                        SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SR.SEMESTER
                                    WHERE
                                        SR.BATCH = ?BATCH AND ACADEMIC_YEAR=?AC_YEAR AND SR.PROGRAMME_GROUP_ID =?PROGRAMME AND SR.IS_ACTIVE=1;;";
                        break;
                    }
                case AcademicSQLCommands.CheckActiveSemesterRoot:
                    {
                        query = @"UPDATE ACADEMIC_SEMESTER SET IS_ACTIVE='0' WHERE PROGRAMME_GROUP_ID=?PROGRAMME AND BATCH=?BATCH AND ACADEMIC_YEAR= ?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.IsSemesterRootExisting:
                    {
                        query = @"SELECT 
                                        ACC_SEMESTER_ID
                                    FROM
                                        ACADEMIC_SEMESTER AS AC
                                    WHERE
                                        PROGRAMME_GROUP_ID =?PROGRAMME AND BATCH =?BATCH
                                          AND ACADEMIC_YEAR=?AC_YEAR  AND SEMESTER =?SEMESTER;";
                        break;
                    }
                case AcademicSQLCommands.SaveSemesterRoot:
                    {
                        query = @"INSERT INTO ACADEMIC_SEMESTER
                                    (
                                    BATCH,
                                    PROGRAMME_GROUP_ID,
                                    SEMESTER,
                                    DATE_FROM,
                                    DATE_TO,
                                    IS_ACTIVE,ACADEMIC_YEAR)
                                    VALUES
                                    (?BATCH,?PROGRAMME,?SEMESTER,?DATE_FROM,?DATE_TO,?IS_ACTIVE,?AC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.UpdateSemesterRoot:
                    {
                        query = @"UPDATE  ACADEMIC_SEMESTER SET                                     
                                    BATCH=?BATCH,
                                    PROGRAMME_GROUP_ID=?PROGRAMME,
                                    SEMESTER=?SEMESTER,
                                    DATE_FROM=?DATE_FROM,
                                    DATE_TO=?DATE_TO,
                                    IS_ACTIVE=?IS_ACTIVE WHERE ACC_SEMESTER_ID=?ACC_SEMESTER_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteSemesterRootInfo:
                    {
                        query = @"UPDATE ACADEMIC_SEMESTER SET IS_DELETED=1 WHERE ACC_SEMESTER_ID=?ACC_SEMESTER_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchProgrammeByProgrammeLevel:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_GROUP_ID, 
                                        P.PROGRAMME_ID, 
                                        UPPER(CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                          ' ',
                                          IFNULL(CONCAT('(', SPM.PROGRAMME_MODE, ') - '),
	                                      ''),
                                          IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''))) AS PROGRAMME_NAME
                                    FROM
                                        CP_CLASSES AS C
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
		                                    INNER JOIN
	                                    SUP_SHIFT AS SH ON SH.SHIFT_ID=PG.SHIFT
		                                    INNER JOIN
	                                    SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID=PG.PROGRAMME_MODE
                                    WHERE
                                        C.CLASS_LEVEL = ?PROGRAMME_LEVEL
                                    GROUP BY PG.PROGRAMME_GROUP_ID
                                    ORDER BY PG.PROGRAMME_GROUP_ID;";
                        break;
                    }

                case AcademicSQLCommands.UpdateAcademicSemesterInMultiple:
                    {
                        query = @"UPDATE ACADEMIC_SEMESTER 
                                    SET 
                                        IS_ACTIVE = ?IS_ACTIVE
                                    WHERE
                                        PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                            AND SEMESTER = ?SEMESTER
                                            AND BATCH = ?BATCH
                                            AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                #endregion

                #region Department
                case AcademicSQLCommands.FetchDepartmentInfo:
                    {
                        query = @"SELECT 
                                        DT.DEPARTMENT_ID,
                                        DT.DEPARTMENT_CODE,
                                        DT.DEPARTMENT,
                                        DT.DEPARTMENT_ORDER,
                                        FY.FACULTY,
                                        DT.YEAR_OF_PUBLISHMENT,
                                        DT.IS_ACTIVE
                                    FROM
                                        CP_DEPARTMENT AS DT LEFT JOIN CP_FACULTY AS FY ON FY.FACULTY_ID=DT.FACULTY
                                    WHERE
                                        DT.IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.InsertDepartmentInfo:
                    {
                        query = @"INSERT INTO CP_DEPARTMENT
                                            (
                                            DEPARTMENT_CODE,
                                            DEPARTMENT,
                                            DEPARTMENT_ORDER,
                                            FACULTY,
                                            YEAR_OF_PUBLISHMENT,
                                            IS_ACTIVE)
                                            VALUES
                                            (
                                            ?DEPARTMENT_CODE,
                                            ?DEPARTMENT,
                                            ?DEPARTMENT_ORDER,
                                            ?FACULTY,
                                            ?YEAR_OF_PUBLISHMENT,
                                            ?IS_ACTIVE);";
                        break;
                    }
                case AcademicSQLCommands.CheckDepartment:
                    {
                        query = @"SELECT 
                                    DEPARTMENT_ID,
                                    DEPARTMENT_CODE,
                                    DEPARTMENT,
                                    DEPARTMENT_ORDER,
                                    FACULTY,
                                    YEAR_OF_PUBLISHMENT,
                                    IS_ACTIVE
                                FROM
                                    CP_DEPARTMENT where DEPARTMENT_CODE=?DEPARTMENT_CODE;";
                        break;
                    }
                case AcademicSQLCommands.UpdateDepartmentInfo:
                    {
                        query = @"UPDATE CP_DEPARTMENT
                                                            SET
                                                            DEPARTMENT= ?DEPARTMENT,
                                                            DEPARTMENT_CODE=?DEPARTMENT_CODE,
                                                            DEPARTMENT=?DEPARTMENT,
                                                            DEPARTMENT_ORDER=?DEPARTMENT_ORDER,
                                                            FACULTY=?FACULTY,
                                                            YEAR_OF_PUBLISHMENT=?YEAR_OF_PUBLISHMENT,
                                                            IS_ACTIVE=?IS_ACTIVE
                                                            WHERE DEPARTMENT_ID=?DEPARTMENT_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchDepartmentInfoById:
                    {
                        query = @"SELECT 
                                    DEPARTMENT_ID,
                                    DEPARTMENT_CODE,
                                    DEPARTMENT,
                                    DEPARTMENT_ORDER,
                                    FACULTY,
                                    YEAR_OF_PUBLISHMENT,
                                    IS_ACTIVE
                                FROM
                                    CP_DEPARTMENT WHERE DEPARTMENT_ID=?DEPARTMENT_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteDepartmentInfo:
                    {
                        query = @"UPDATE CP_DEPARTMENT
                                                            SET
                                                            IS_DELETED = 1
                                                            WHERE DEPARTMENT_ID =?DEPARTMENT_ID;";
                        break;
                    }
                #endregion

                #region Facutly
                case AcademicSQLCommands.FetchFAcutlyInfo:
                    {
                        query = @"SELECT 
                                        FACULTY_ID,
                                        FACULTY,
                                        FACULTY_CODE,
                                        FACULTY_ORDER,
                                        FAC_SERIES,
                                        IS_ACTIVE
                                    FROM
                                        CP_FACULTY
                                    WHERE
                                        IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.CheckFaculty:
                    {
                        query = @"SELECT 
                                        FACULTY_ID,
                                        FACULTY,
                                        FACULTY_CODE,
                                        FACULTY_ORDER,
                                        FAC_SERIES,
                                        IS_ACTIVE
                                    FROM
                                        CP_FACULTY
                                    WHERE
                                        FACULTY_CODE = ?FACULTY_CODE";
                        break;
                    }
                case AcademicSQLCommands.InsertFacultyInfo:
                    {
                        query = @"INSERT INTO CP_FACULTY
                                            (
                                            FACULTY_CODE,
                                            FACULTY,
                                            FACULTY_ORDER,
                                            FAC_SERIES)
                                            VALUES
                                            (
                                            ?FACULTY_CODE,
                                            ?FACULTY,
                                            ?FACULTY_ORDER,
                                            ?FAC_SERIES);";
                        break;
                    }
                case AcademicSQLCommands.FetchFacultyInfoById:
                    {
                        query = @"SELECT 
                                        FACULTY_ID,
                                        FACULTY,
                                        FACULTY_CODE,
                                        FACULTY_ORDER,
                                        FAC_SERIES
                                FROM
                                    CP_FACULTY WHERE FACULTY_ID=?FACULTY_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateFacultyInfo:
                    {
                        query = @"UPDATE CP_FACULTY 
                                                    SET 
                                                        FACULTY_ID = ?FACULTY_ID,
                                                        FACULTY_CODE = ?FACULTY_CODE,
                                                        FACULTY = ?FACULTY,
                                                        FACULTY_ORDER = ?FACULTY_ORDER,
                                                        FAC_SERIES = ?FAC_SERIES
                                                    WHERE
                                                        FACULTY_ID = ?FACULTY_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteFacultyInfo:
                    {
                        query = @"UPDATE CP_FACULTY 
                                                            SET
                                                            IS_DELETED = 1
                                                            WHERE FACULTY_ID =?FACULTY_ID;";
                        break;
                    }
                #endregion

                #region Class
                case AcademicSQLCommands.FetchClassInfo:
                    {
                        query = @"SELECT 
                                        CLS.CLASS_ID,
                                        CLS.CLASS_CODE,
                                        CLS.CLASS_NAME,
                                        CLS.CLASS_DESCRIPTION,
                                        CT.CLASS_TYPE,
                                        CL.CLASSLEVEL,
                                        CY.CLASS_YEAR,
                                        CP.PROGRAMME_NAME,
                                        DT.DEPARTMENT,
                                        LN.LANGUAGE_NAME,
                                        ST.SHIFT_NAME
                                    FROM
                                        CP_CLASSES AS CLS
                                            LEFT JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = CLS.DEPARTMENT
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
											LEFT JOIN
										cp_programme AS CP ON CP.PROGRAMME_ID=PG.PROGRAMME_ID
                                            LEFT JOIN
                                        SUP_CLASS_LEVEL AS CL ON CL.CLASSLEVELID = CLS.CLASS_LEVEL
                                            LEFT JOIN
                                        SUP_CLASS_TYPE AS CT ON CT.CLASS_TYPE = CLS.CLASS_TYPE
                                            LEFT JOIN
                                        SUP_CLASS_YEAR AS CY ON CY.CLASS_YEAR_ID = CLS.CLASS_YEAR
                                            LEFT JOIN
                                        SUP_LANGUAGE AS LN ON LN.LANGUAGE_ID = CLS.LANGUAGE
                                            LEFT JOIN
                                        SUP_SHIFT AS ST ON ST.SHIFT_ID = CLS.SHIFT WHERE CLS. IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.CheckClass:
                    {
                        query = @"SELECT 
	                                    CLASS_ID,
                                        CLASS_CODE,
                                        CLASS_DESCRIPTION,
                                        CLASS_YEAR 
                                    FROM CP_CLASSES
                                    WHERE CLASS_CODE=?CLASS_CODE;";
                        break;
                    }
                case AcademicSQLCommands.InsertClassInfo:
                    {
                        query = @"INSERT INTO CP_CLASSES
                                                            (
                                                            CLASS_CODE,
                                                            CLASS_NAME,
                                                            CLASS_DESCRIPTION,
                                                            CLASS_TYPE,
                                                            CLASS_LEVEL,
                                                            CLASS_YEAR,
                                                            PROGRAMME_GROUP_ID,
                                                            DEPARTMENT,
                                                            LANGUAGE,
                                                            IS_CHOICE,
                                                            CLASS_ORDER,
                                                            SHIFT,
                                                            COURSE_ID,
                                                            PROGRAMME_MODE)
                                                            VALUES
                                                            (
                                                            ?CLASS_CODE,
                                                            ?CLASS_NAME,
                                                            ?CLASS_DESCRIPTION,
                                                            ?CLASS_TYPE,
                                                            ?CLASS_LEVEL,
                                                            ?CLASS_YEAR,
                                                            ?PROGRAMME_GROUP_ID,
                                                            ?DEPARTMENT,
                                                            ?LANGUAGE,
                                                            ?IS_CHOICE,
                                                            ?CLASS_ORDER,
                                                            ?SHIFT,
                                                            ?COURSE_ID,
                                                            ?PROGRAMME_MODE);";
                        break;
                    }
                case AcademicSQLCommands.FetchClassInfoById:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CLASS_NAME,
                                        CLASS_CODE,
                                        CLASS_DESCRIPTION,
                                        CLASS_TYPE,
                                        CLASS_LEVEL,
                                        CLASS_YEAR,
                                        PROGRAMME_GROUP_ID,
                                        DEPARTMENT,
                                        LANGUAGE,
                                        IS_CHOICE,
                                        CLASS_ORDER,
                                        SHIFT,
                                        COURSE_ID
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateClassInfo:
                    {
                        query = @"UPDATE CP_CLASSES
                                                        SET
                                                        CLASS_ID=?CLASS_ID,
                                                        CLASS_CODE=?CLASS_CODE,
                                                        CLASS_NAME=?CLASS_NAME,
                                                        CLASS_DESCRIPTION=?CLASS_DESCRIPTION,
                                                        CLASS_TYPE=?CLASS_TYPE,
                                                        CLASS_LEVEL=?CLASS_LEVEL,
                                                        CLASS_YEAR=?CLASS_YEAR,
                                                        PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID,
                                                        DEPARTMENT=?DEPARTMENT,
                                                        LANGUAGE=?LANGUAGE,
                                                        IS_CHOICE=?IS_CHOICE,
                                                        CLASS_ORDER=?CLASS_ORDER,
                                                        SHIFT=?SHIFT,
                                                        COURSE_ID=?COURSE_ID,
                                                        PROGRAMME_MODE=?PROGRAMME_MODE
                                                        WHERE CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteClassInfo:
                    {
                        query = @"UPDATE CP_CLASSES
                                                            SET
                                                            IS_DELETED = 1
                                                            WHERE CLASS_ID =?CLASS_ID;";
                        break;
                    }
                #endregion

                #region Batch
                case AcademicSQLCommands.FetcAcademichBatchInfo:
                    {
                        query = @"SELECT 
                                        AB.AC_BATCH_ID,
                                        SB.BATCH,
                                        AY.AC_YEAR
                                    FROM
                                        ACADEMIC_BATCHES AS AB
                                            LEFT JOIN
                                        SUP_BATCHES AS SB ON SB.BATCH_ID = AB.BATCH_ID
                                            LEFT JOIN
                                        ACADEMIC_YEAR AS AY ON AY.ACADEMIC_YEAR_ID = AB.ACADEMIC_YEAR_ID;";
                        break;
                    }
                case AcademicSQLCommands.CheckAcademicBatch:
                    {
                        query = @"SELECT 
                                        AC_BATCH_ID
                                    FROM
                                        ACADEMIC_BATCHES
                                    WHERE
                                        BATCH_ID = ?BATCH_ID AND ACADEMIC_YEAR_ID=?ACADEMIC_YEAR_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertAcademicBatchInfo:
                    {
                        query = @"INSERT INTO ACADEMIC_BATCHES
                                            (
                                            BATCH_ID,
                                            ACADEMIC_YEAR_ID,
                                            BATCH_YEAR)
                                            VALUES
                                            (
                                            ?BATCH_ID,
                                            ?ACADEMIC_YEAR_ID,
                                            ?BATCH_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.UpdateAcademicBatchInfo:
                    {
                        query = @"UPDATE ACADEMIC_BATCHES
                                                        SET
                                                        BATCH_ID = ?BATCH_ID,
                                                        BATCH_YEAR=?BATCH_YEAR,
                                                        ACADEMIC_YEAR_ID = ?ACADEMIC_YEAR_ID
                                                        WHERE AC_BATCH_ID = ?AC_BATCH_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchAcademicBatchesForDDL:
                    {
                        query = @"SELECT
	                                    BATCH_ID,
                                        BATCH,
                                        SUM((SELECT 
                                                MAX(SUBSTRING(BATCH,4)) AS MAX_BATCH_ID
                                            FROM
                                                SUP_BATCHES) - BATCH_ID + 1) AS BATCH_YEAR
                                    FROM
                                        SUP_BATCHES
                                    GROUP BY BATCH_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteAcademicBatches:
                    {
                        query = @"";
                        break;
                    }
                #endregion

                #region CourseType
                case AcademicSQLCommands.FetchCourseTypeInfo:
                    {
                        query = @"SELECT 
                                        CT.COURSE_TYPE_ID,
                                        CT.COURSE_TYPE,
                                        CT.NO_OF_COMPONENTS,
                                        CT.COURSE_TYPE_ORDER,
                                        CT.IS_STU_BASED_SELECTION,
                                        CT.INTERNAL,
                                        CT.EXTERNAL,
                                        CT.TOTAL,
                                        CT.HOURS,
                                        PT.PART,
                                        PL.PROGRAMME_LEVEL,
                                        CT.CREDITS,
                                        CT.IS_STUDENT_VIEW_CIA_MARK
                                    FROM
                                        CP_COURSE_TYPE AS CT
		                                    LEFT JOIN
	                                    SUP_PROGRAMME_LEVEL AS PL ON PL.PROGRAMME_LEVEL_ID=CT.PROGRAMME_LEVEL
		                                    LEFT JOIN
	                                    SUP_PART AS PT ON PT.PART_ID=CT.PART_ID WHERE CT.IS_DELETED!=1 ORDER BY COURSE_TYPE_ID";
                        break;
                    }
                case AcademicSQLCommands.CheckCourseType:
                    {
                        query = @"SELECT 
                                        COURSE_TYPE_ID,COURSE_TYPE,INTERNAL,EXTERNAL,TOTAL
                                    FROM
                                        CP_COURSE_TYPE
                                    WHERE
                                        COURSE_TYPE = ?COURSE_TYPE AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.InsertCourseTypeInfo:
                    {
                        query = @"INSERT INTO CP_COURSE_TYPE
                                                                    (
                                                                    COURSE_TYPE,
                                                                    NO_OF_COMPONENTS,
                                                                    COURSE_TYPE_ORDER,
                                                                    IS_STU_BASED_SELECTION,
                                                                    INTERNAL,
                                                                    EXTERNAL,
                                                                    TOTAL,
                                                                    HOURS,
                                                                    PART_ID,
                                                                    PROGRAMME_LEVEL,
                                                                    CREDITS,
                                                                    IS_STUDENT_VIEW_CIA_MARK,
                                                                    ACADEMIC_YEAR)
                                                                    VALUES
                                                                    (
                                                                    ?COURSE_TYPE,
                                                                    ?NO_OF_COMPONENTS,
                                                                    ?COURSE_TYPE_ORDER,
                                                                    ?IS_STU_BASED_SELECTION,
                                                                    ?INTERNAL,
                                                                    ?EXTERNAL,
                                                                    ?TOTAL,
                                                                    ?HOURS,
                                                                    ?PART_ID,
                                                                    ?PROGRAMME_LEVEL,
                                                                    ?CREDITS,
                                                                    ?IS_STUDENT_VIEW_CIA_MARK,
                                                                    ?AC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseTypeInfoById:
                    {
                        query = @"SELECT 
                                        COURSE_TYPE_ID,
                                        COURSE_TYPE,
                                        NO_OF_COMPONENTS,
                                        COURSE_TYPE_ORDER,
                                        IS_STU_BASED_SELECTION,
                                        INTERNAL,
                                        EXTERNAL,
                                        TOTAL,
                                        HOURS,
                                        PART_ID,
                                        PROGRAMME_LEVEL,
                                        CREDITS,
                                        IS_STUDENT_VIEW_CIA_MARK
                                    FROM
                                        CP_COURSE_TYPE WHERE COURSE_TYPE_ID=?COURSE_TYPE_ID";
                        break;
                    }
                case AcademicSQLCommands.UpdateCourseTypeInfo:
                    {
                        query = @"UPDATE CP_COURSE_TYPE
                                                                SET
                                                                COURSE_TYPE_ID= ?COURSE_TYPE_ID,
                                                                COURSE_TYPE= ?COURSE_TYPE,
                                                                NO_OF_COMPONENTS= ?NO_OF_COMPONENTS,
                                                                COURSE_TYPE_ORDER= ?COURSE_TYPE_ORDER,
                                                                IS_STU_BASED_SELECTION= ?IS_STU_BASED_SELECTION,
                                                                INTERNAL= ?INTERNAL,
                                                                EXTERNAL= ?EXTERNAL,
                                                                TOTAL= ?TOTAL,
                                                                HOURS= ?HOURS,
                                                                PART_ID= ?PART_ID,
                                                                PROGRAMME_LEVEL= ?PROGRAMME_LEVEL,
                                                                CREDITS= ?CREDITS,
                                                                IS_STUDENT_VIEW_CIA_MARK= ?IS_STUDENT_VIEW_CIA_MARK
                                                                WHERE COURSE_TYPE_ID =?COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteCourseTypeInfo:
                    {
                        query = @"UPDATE CP_COURSE_TYPE
                                                                SET
                                                                IS_DELETED = 1
                                                                WHERE COURSE_TYPE_ID =?COURSE_TYPE_ID;";
                        break;
                    }
                #endregion

                #region Programme
                case AcademicSQLCommands.FetchProgrammeInfo:
                    {
                        query = @"SELECT 
                                        PR.PROGRAMME_ID,
                                        PR.PROGRAMME_CODE,
                                        PR.PROGRAMME_NAME,
                                        PR.PROGRAMME_DESCRIPTION,
                                        DT.DEPARTMENT,
                                        DE.DEGREE,
                                        PR.PROGRAMME_ORDER,
                                        PR.DURATION,
                                        DU.DURATION_UNIT,
                                        PR.NO_OF_SEMESTER,
                                        CH.CHANNEL,
                                        PR.PRO_SERIES_ROLLNO,
                                        SUB.SUBJECT,
                                        PR.PRO_SERIES_REGNO,
                                        PR.PRO_SERIES_ADMNO,
                                        ME.MEDIUM_OF_INSTRUCTION,
                                        IA.IS_AIDED_NAME,
                                        NA.NON_AIDED_NAME,
                                        IR.IS_REGULAR_NAME
                                    FROM
                                        CP_PROGRAMME AS PR
                                            LEFT JOIN
                                        CP_DEPARTMENT AS DT ON DT.DEPARTMENT_ID = PR.DEPARTMENT
                                            LEFT JOIN
                                        CP_DEGREE AS DE ON DE.DEGREE_ID = PR.DEGREE
                                            LEFT JOIN
                                        SUP_DURATION_UNIT AS DU ON DU.DURATION_UNIT_ID = PR.DURATION_UNIT
		                                    LEFT JOIN
	                                    SUP_CHANNEL AS CH ON CH.CHANNEL_ID=PR.CHANNEL
		                                    LEFT JOIN
	                                    SUP_SUBJECT AS SUB ON SUB.SUBJECT_ID=PR.SUBJECTS
		                                    LEFT JOIN
	                                    SUP_MEDIUM_OF_INSTRUCTION AS ME ON ME.MEDIUM_ID=PR.MEDIUM_OF_INSTRACTION
		                                    LEFT JOIN
	                                    SUP_IS_AIDED AS IA ON IA.IS_AIDED_ID=PR.IS_AIDED
		                                    LEFT JOIN
	                                    SUP_NON_AIDED AS NA ON NA.NON_AIDED_ID=PR.NON_AIDED
		                                    LEFT JOIN
	                                    SUP_IS_REGULAR AS IR ON IR.IS_REGULAR_ID=PR.IS_REGULAR WHERE PR.IS_DELETED=0";
                        break;
                    }
                case AcademicSQLCommands.CheckProgramme:
                    {
                        query = @"SELECT 
                                        PROGRAMME_ID, PROGRAMME_CODE, PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME
                                    WHERE
                                        PROGRAMME_CODE =?PROGRAMME_CODE;";
                        break;
                    }
                case AcademicSQLCommands.InsertProgrammeInfo:
                    {
                        query = @"INSERT INTO CP_PROGRAMME
                                                                (
                                                                PROGRAMME_CODE,
                                                                PROGRAMME_NAME,
                                                                PROGRAMME_DESCRIPTION,
                                                                DEPARTMENT,
                                                                DEGREE,
                                                                PROGRAMME_ORDER,
                                                                DURATION_UNIT,
                                                                NO_OF_SEMESTER,
                                                                CHANNEL,
                                                                IS_AIDED,
                                                                PRO_SERIES_ROLLNO,
                                                                NON_AIDED,
                                                                IS_REGULAR,
                                                                SUBJECTS,
                                                                PRO_SERIES_REGNO,
                                                                PRO_SERIES_ADMNO,
                                                                MEDIUM_OF_INSTRACTION)
                                                                VALUES
                                                                (
                                                                ?PROGRAMME_CODE,
                                                                ?PROGRAMME_NAME,
                                                                ?PROGRAMME_DESCRIPTION,
                                                                ?DEPARTMENT,
                                                                ?DEGREE,
                                                                ?PROGRAMME_ORDER,
                                                                ?DURATION_UNIT,
                                                                ?NO_OF_SEMESTER,
                                                                ?CHANNEL,
                                                                ?IS_AIDED,
                                                                ?PRO_SERIES_ROLLNO,
                                                                ?NON_AIDED,
                                                                ?IS_REGULAR,
                                                                ?SUBJECTS,
                                                                ?PRO_SERIES_REGNO,
                                                                ?PRO_SERIES_ADMNO,
                                                                ?MEDIUM_OF_INSTRACTION);";
                        break;
                    }
                case AcademicSQLCommands.FetchProgrammeInfoById:
                    {
                        query = @"SELECT 
                                        PROGRAMME_ID,
                                        PROGRAMME_CODE,
                                        PROGRAMME_NAME,
                                        PROGRAMME_DESCRIPTION,
                                        DEPARTMENT,
                                        DEGREE,
                                        PROGRAMME_ORDER,
                                        DURATION_UNIT,
                                        NO_OF_SEMESTER,
                                        CHANNEL,
                                        IS_AIDED,
                                        PRO_SERIES_ROLLNO,
                                        NON_AIDED,
                                        IS_REGULAR,
                                        SUBJECTS,
                                        PRO_SERIES_REGNO,
                                        PRO_SERIES_ADMNO,
                                        MEDIUM_OF_INSTRACTION
                                    FROM
                                        CP_PROGRAMME
                                    WHERE PROGRAMME_ID=?PROGRAMME_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateProgrammeInfo:
                    {
                        query = @"UPDATE CP_PROGRAMME
                                                                SET
                                                                PROGRAMME_ID= ?PROGRAMME_ID,
                                                                PROGRAMME_CODE= ?PROGRAMME_CODE,
                                                                PROGRAMME_NAME= ?PROGRAMME_NAME,
                                                                PROGRAMME_DESCRIPTION= ?PROGRAMME_DESCRIPTION,
                                                                DEPARTMENT= ?DEPARTMENT,
                                                                DEGREE= ?DEGREE,
                                                                PROGRAMME_ORDER= ?PROGRAMME_ORDER,
                                                                DURATION_UNIT= ?DURATION_UNIT,
                                                                NO_OF_SEMESTER= ?NO_OF_SEMESTER,
                                                                CHANNEL= ?CHANNEL,
                                                                IS_AIDED= ?IS_AIDED,
                                                                PRO_SERIES_ROLLNO= ?PRO_SERIES_ROLLNO,
                                                                NON_AIDED= ?NON_AIDED,
                                                                IS_REGULAR= ?IS_REGULAR,
                                                                SUBJECTS= ?SUBJECTS,
                                                                PRO_SERIES_REGNO= ?PRO_SERIES_REGNO,
                                                                PRO_SERIES_ADMNO= ?PRO_SERIES_ADMNO,
                                                                MEDIUM_OF_INSTRACTION= ?MEDIUM_OF_INSTRACTION
                                                                WHERE PROGRAMME_ID= ?PROGRAMME_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteProgrammeInfo:
                    {
                        query = @"UPDATE CP_PROGRAMME
                                                            SET
                                                            IS_DELETED = 1
                                                            WHERE PROGRAMME_ID =?PROGRAMME_ID;";
                        break;
                    }
                #endregion

                #region Degree
                case AcademicSQLCommands.FetchDegreeInfo:
                    {
                        query = @"SELECT 
                                        DE.DEGREE_ID,
                                        DE.DEGREE,
                                        PT.PROGRAMME_TYPE,
                                        PL.PROGRAMME_LEVEL,
                                        DE.DEGREE_ORDER,
                                        FY.FACULTY,
                                        DE.BOS_NAME,
                                        DE.PREFIX,
                                        DE.BOS_SERIES_ROLLNO,
                                        DE.BOS_SERIES_REGNO,
                                        DE.BOS_SERIES_ADMNO
                                    FROM
                                        CP_DEGREE AS DE
                                            INNER JOIN
                                        SUP_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID = DE.PROGRAMME_TYPE
                                            INNER JOIN
                                        SUP_PROGRAMME_LEVEL AS PL ON PL.PROGRAMME_LEVEL_ID = DE.PROGRAMME_LEVEL
                                            INNER JOIN
                                        CP_FACULTY AS FY ON FY.FACULTY_ID = DE.FACULTY
                                    WHERE DE.IS_DELETED=0;";
                        break;
                    }
                case AcademicSQLCommands.CheckDegree:
                    {
                        query = @"SELECT 
                                        DEGREE_ID,
                                        DEGREE,
                                        PROGRAMME_TYPE,
                                        PROGRAMME_LEVEL,
                                        DEGREE_ORDER,
                                        FACULTY,
                                        BOS_NAME,
                                        PREFIX,
                                        BOS_SERIES_ROLLNO,
                                        BOS_SERIES_REGNO,
                                        BOS_SERIES_ADMNO
                                    FROM
                                        CP_DEGREE WHERE PREFIX=?PREFIX;";
                        break;
                    }
                case AcademicSQLCommands.InsertDegreeInfo:
                    {
                        query = @"INSERT INTO CP_DEGREE
                                                                (
                                                                DEGREE,
                                                                PROGRAMME_TYPE,
                                                                PROGRAMME_LEVEL,
                                                                DEGREE_ORDER,
                                                                FACULTY,
                                                                BOS_NAME,
                                                                PREFIX,
                                                                BOS_SERIES_ROLLNO,
                                                                BOS_SERIES_REGNO,
                                                                BOS_SERIES_ADMNO)
                                                                VALUES
                                                                (
                                                                ?DEGREE,
                                                                ?PROGRAMME_TYPE,
                                                                ?PROGRAMME_LEVEL,
                                                                ?DEGREE_ORDER,
                                                                ?FACULTY,
                                                                ?BOS_NAME,
                                                                ?PREFIX,
                                                                ?BOS_SERIES_ROLLNO,
                                                                ?BOS_SERIES_REGNO,
                                                                ?BOS_SERIES_ADMNO);";
                        break;
                    }
                case AcademicSQLCommands.FetchDegreeInfoById:
                    {
                        query = @"SELECT 
                                        DEGREE_ID,
                                        DEGREE,
                                        PROGRAMME_TYPE,
                                        PROGRAMME_LEVEL,
                                        DEGREE_ORDER,
                                        FACULTY,
                                        BOS_NAME,
                                        PREFIX,
                                        BOS_SERIES_ROLLNO,
                                        BOS_SERIES_REGNO,
                                        BOS_SERIES_ADMNO
                                    FROM
                                        CP_DEGREE
                                    WHERE DEGREE_ID=?DEGREE_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateDegreeInfo:
                    {
                        query = @"UPDATE CP_DEGREE
                                                                SET
                                                                DEGREE_ID= ?DEGREE_ID,
                                                                DEGREE= ?DEGREE,
                                                                PROGRAMME_TYPE= ?PROGRAMME_TYPE,
                                                                PROGRAMME_LEVEL= ?PROGRAMME_LEVEL,
                                                                DEGREE_ORDER= ?DEGREE_ORDER,
                                                                FACULTY= ?FACULTY,
                                                                BOS_NAME= ?BOS_NAME,
                                                                PREFIX= ?PREFIX,
                                                                BOS_SERIES_ROLLNO= ?BOS_SERIES_ROLLNO,
                                                                BOS_SERIES_REGNO= ?BOS_SERIES_REGNO,
                                                                BOS_SERIES_ADMNO= ?BOS_SERIES_ADMNO
                                                                WHERE DEGREE_ID= ?DEGREE_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteDegreeInfo:
                    {
                        query = @"UPDATE CP_DEGREE
                                                            SET
                                                            IS_DELETED = 1
                                                            WHERE DEGREE_ID =?DEGREE_ID;";
                        break;
                    }
                #endregion

                #region Coruse Structure
                case AcademicSQLCommands.FetchCourseStructureByAcademicYearAndFaculty:
                    {
                        query = @"SELECT 
                                        COURSE_STRUCTURE_ID,
                                        CREDIT,
                                        HRS_PER_WEEK,
                                        ACADEMIC_YEAR,
                                        IS_OPTIONAL,
                                        IS_NME,
                                        IS_ALLIED,
                                        UGC_COURSE_TYPE UGC_TYPE_ID,
                                        FACULTY AS FACULTY_ID,
                                        PART AS PART_ID,
                                        SEMESTER_NAME AS SEMESTER_ID,
                                        SC.CLASS_YEAR AS PROGRAMME_YEAR,
                                        SPL.PROGRAMME_LEVEL
                                    FROM
                                        CP_COURSE_STRUCTURE AS CS
                                            INNER JOIN
                                        CP_UGC_COURSE_TYPE AS UGC ON UGC.UGC_COURSE_TYPE_ID = CS.UGC_TYPE_ID
                                            INNER JOIN
                                        CP_FACULTY AS CF ON CF.FACULTY_ID = CS.FACULTY_ID
                                            LEFT JOIN
                                        SUP_PART AS SP ON SP.PART_ID = CS.PART_ID
                                            INNER JOIN
                                        SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CS.SEMESTER_ID
                                            INNER JOIN
                                        SUP_CLASS_YEAR AS SC ON SC.CLASS_YEAR_ID = CS.PROGRAMME_YEAR
                                            INNER JOIN
                                        SUP_PROGRAMME_LEVEL AS SPL ON SPL.PROGRAMME_LEVEL_ID = CS.PROGRAMME_LEVEL
                                    WHERE
                                        CS.IS_DELETED != 1
                                            AND CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND CS.FACULTY_ID = ?FACULTY_ID
                                        ORDER BY CS.SEMESTER_ID,CS.PROGRAMME_LEVEL,CS.PROGRAMME_YEAR,CS.PART_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertCoursestructure:
                    {
                        query = @"INSERT INTO CP_COURSE_STRUCTURE
                                        (
                                        CREDIT,
                                        PART_ID,
                                        UGC_TYPE_ID,
                                        HRS_PER_WEEK,
                                        MARKS,
                                        SEMESTER_ID,
                                        FACULTY_ID,
                                        PROGRAMME_LEVEL,
                                        PROGRAMME_YEAR,
                                        ACADEMIC_YEAR,
                                        IS_OPTIONAL,
                                        IS_NME,
                                        IS_ALLIED
                                        )
                                        VALUES
                                        (
                                        ?CREDIT,
                                        ?PART_ID,
                                        ?UGC_TYPE_ID,
                                        ?HRS_PER_WEEK,
                                        ?MARKS,
                                        ?SEMESTER_ID,
                                        ?FACULTY_ID,
                                        ?PROGRAMME_LEVEL,
                                        ?PROGRAMME_YEAR,
                                        ?ACADEMIC_YEAR,
                                        ?IS_OPTIONAL,
                                        ?IS_NME,
                                        ?IS_ALLIED
                                        );";
                        break;
                    }
                case AcademicSQLCommands.UpdateCourseStructure:
                    {
                        query = @"UPDATE CP_COURSE_STRUCTURE
                                        SET
                                        CREDIT = ?CREDIT,
                                        PART_ID = ?PART_ID,
                                        UGC_TYPE_ID = ?UGC_TYPE_ID,
                                        HRS_PER_WEEK = ?HRS_PER_WEEK,
                                        MARKS = ?MARKS,
                                        SEMESTER_ID = ?SEMESTER_ID,
                                        FACULTY_ID = ?FACULTY_ID,
                                        PROGRAMME_LEVEL = ?PROGRAMME_LEVEL,
                                        PROGRAMME_YEAR = ?PROGRAMME_YEAR,
                                        ACADEMIC_YEAR = ?ACADEMIC_YEAR,
                                        IS_OPTIONAL = ?IS_OPTIONAL,
                                        IS_NME = ?IS_NME,
                                        IS_ALLIED = ?IS_ALLIED
                                        WHERE COURSE_STRUCTURE_ID = ?COURSE_STRUCTURE_ID;";
                        break;
                    }
                case AcademicSQLCommands.CoursestructureIsExist:
                    {
                        query = @"";
                        break;
                    }
                case AcademicSQLCommands.FetchCoursestructureByCoursestructureId:
                    {
                        query = @"SELECT COURSE_STRUCTURE_ID,
                                    CREDIT,
                                    PART_ID,
                                    UGC_TYPE_ID,
                                    HRS_PER_WEEK,
                                    MARKS,
                                    SEMESTER_ID,
                                    FACULTY_ID,
                                    PROGRAMME_LEVEL,
                                    PROGRAMME_YEAR,
                                    ACADEMIC_YEAR,
                                    IS_OPTIONAL,
                                    IS_NME,
                                    IS_ALLIED
                                FROM CP_COURSE_STRUCTURE WHERE 
                                 IS_DELETED!=1 
                                 AND COURSE_STRUCTURE_ID=?COURSE_STRUCTURE_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteCoursestructure:
                    {
                        query = @"UPDATE CP_COURSE_STRUCTURE SET IS_DELETED=1 WHERE COURSE_STRUCTURE_ID=?COURSE_STRUCTURE_ID;";
                        break;
                    }
                #endregion

                #region Cousre Pattern
                case AcademicSQLCommands.FetchCoursePatternByAcademicYearandProgramme:
                    {
                        query = @"SELECT 
                                CS.COURSE_STRUCTURE_ID,
                                CP.COURSE_PATTERN_ID,
                                CR.CREDITS AS 'CREDIT',
                                SEM.SEMESTER_NAME,
                                P.PART,
                                UG.UGC_COURSE_TYPE,
                                FA.FACULTY,
                                PL.PROGRAMME_LEVEL,
                                Y.CLASS_YEAR,
                                CP.ACADEMIC_YEAR,
                                CS.MARKS,
                                CR.COURSE_TITLE,
                                CR.COURSE_CODE,
                                UG.UGC_COURSE_TYPE_ID,
                                CR.HRS_PER_WEEK
                            FROM
                                CP_COURSE_PATTERN AS CP
                                    INNER JOIN
                                CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                    AND CS.IS_DELETED != 1
                                    AND CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CS.PART_ID
                                    INNER JOIN
                                CP_UGC_COURSE_TYPE AS UG ON UG.UGC_COURSE_TYPE_ID = CS.UGC_TYPE_ID
                                    AND UG.IS_DELETED != 1
                                    INNER JOIN
                                CP_FACULTY AS FA ON FA.FACULTY_ID = CS.FACULTY_ID
                                    AND FA.IS_DELETED != 1
                                    INNER JOIN
                                SUP_PROGRAMME_LEVEL AS PL ON PL.PROGRAMME_LEVEL_ID = CS.PROGRAMME_LEVEL
                                    INNER JOIN
                                SUP_CLASS_YEAR AS Y ON Y.CLASS_YEAR_ID = CS.PROGRAMME_YEAR
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN
                                CP_COURSE_ROOT CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                    AND CR.IS_DELETED != 1 
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CR.SEMESTER_ID
                            WHERE
                                CP.ACADEMIC_YEAR = ?ACADEMIC_YEAR 
                                  --  AND CR.SEMESTER_ID IN (?SEMESTER_ID)
                                    AND CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID 
                                    AND CP.IS_DELETED!=1 ORDER BY CR.SEMESTER_ID,CS.PART_ID;";
                        break;

                    }
                case AcademicSQLCommands.FetchCoursePatternForAddEdit:
                    {
                        query = @"SELECT 
                                    COURSE_PATTERN_ID,
                                    CS.COURSE_STRUCTURE_ID,
                                    GROUP_CONCAT(COURSE_PATTERN_ID,
                                        '|',
                                        COURSE_ID,
                                        '|',
                                        CS.COURSE_STRUCTURE_ID) AS 'OLD_ATTRIBUTE',
                                    GROUP_CONCAT(COURSE_ID) AS COURSE_ID,
                                    CS.CREDIT,
                                    SEM.SEMESTER_NAME,
                                    P.PART,
                                    UG.UGC_COURSE_TYPE,
                                    FA.FACULTY,
                                    CS.IS_OPTIONAL,
                                    PL.PROGRAMME_LEVEL,
                                    Y.CLASS_YEAR,
                                    CP.ACADEMIC_YEAR,
                                    CS.MARKS,
                                    CR.COURSE_TITLE,
                                    CR.COURSE_CODE,
                                    UG.UGC_COURSE_TYPE_ID
                                FROM
                                    CP_COURSE_PATTERN AS CP
                                        INNER JOIN
                                    CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                        AND CS.IS_DELETED != 1
                                        AND CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        INNER JOIN
                                    SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CS.SEMESTER_ID
                                        LEFT JOIN
                                    SUP_PART AS P ON P.PART_ID = CS.PART_ID
                                        INNER JOIN
                                    CP_UGC_COURSE_TYPE AS UG ON UG.UGC_COURSE_TYPE_ID = CS.UGC_TYPE_ID
                                        AND UG.IS_DELETED != 1
                                        INNER JOIN
                                    CP_FACULTY AS FA ON FA.FACULTY_ID = CS.FACULTY_ID
                                        AND FA.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_PROGRAMME_LEVEL AS PL ON PL.PROGRAMME_LEVEL_ID = CS.PROGRAMME_LEVEL
                                        INNER JOIN
                                    SUP_CLASS_YEAR AS Y ON Y.CLASS_YEAR_ID = CS.PROGRAMME_YEAR
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS PR ON PR.PROGRAMME_ID = PG.PROGRAMME_ID      
                                        INNER JOIN
                                    CP_COURSE_ROOT CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                WHERE
                                    CP.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        AND CS.FACULTY_ID = ?FACULTY_ID
                                        AND CS.SEMESTER_ID IN (?SEMESTER_ID)
                                        AND CS.PROGRAMME_LEVEL = ?PROGRAMME_LEVEL
                                        AND CP.IS_DELETED != 1
                                GROUP BY CP.PROGRAMME_GROUP_ID , CS.COURSE_STRUCTURE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchAllCourseListForCoursePattern:
                    {
                        query = @"SELECT 
                                COURSE_ROOT_ID,
                                DEPARTMENT,
                                CONCAT(IFNULL(COURSE_TITLE, ''),
                                        '(',
                                        IFNULL(COURSE_CODE, ''),
                                        ')') AS COURSE_TITLE,
                                SEMESTER_ID,
                                COURSE_TYPE,
                                CREDITS,
                                UGC_COURSE_TYPE,
                                ACADEMIC_YEAR
                            FROM
                                CP_COURSE_ROOT
                            WHERE
                                IS_DELETED != 1 AND SEMESTER_ID IN (?SEMESTER_ID);";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseStructureforCoursePattern:
                    {
                        query = @"SELECT 
                                COURSE_STRUCTURE_ID,
                                CREDIT,
                                P.PART AS PART_ID,
                                UGC_TYPE_ID,
                                UG.UGC_COURSE_TYPE,
                                MARKS,
                                SEMESTER_ID,
                                CS.FACULTY_ID,
                                CS.PROGRAMME_LEVEL,
                                CS.PROGRAMME_YEAR,
                                CS.IS_OPTIONAL,
                                IS_NME,
                                IS_ALLIED
                            FROM
                                CP_COURSE_STRUCTURE AS CS
                                    INNER JOIN
                                SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = CS.SEMESTER_ID
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = CS.PART_ID
                                    INNER JOIN
                                CP_UGC_COURSE_TYPE AS UG ON UG.UGC_COURSE_TYPE_ID = CS.UGC_TYPE_ID
                                    AND UG.IS_DELETED != 1
                                    INNER JOIN
                                CP_FACULTY AS FA ON FA.FACULTY_ID = CS.FACULTY_ID
                                    AND FA.IS_DELETED != 1
                                    INNER JOIN
                                SUP_PROGRAMME_LEVEL AS PL ON PL.PROGRAMME_LEVEL_ID = CS.PROGRAMME_LEVEL
                                    INNER JOIN
                                SUP_CLASS_YEAR AS Y ON Y.CLASS_YEAR_ID = CS.PROGRAMME_YEAR
                            WHERE
                                CS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND CS.FACULTY_ID = ?FACULTY_ID
		                            AND CS.PROGRAMME_LEVEL = ?PROGRAMME_LEVEL
                                    AND CS.SEMESTER_ID IN (?SEMESTER_ID) AND CS.IS_DELETED!=1        
                                    ORDER BY CS.PROGRAMME_LEVEL , CS.SEMESTER_ID , CS.PART_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertCoursePattern:
                    {
                        query = @"INSERT INTO CP_COURSE_PATTERN
                            (
                            COURSE_STRUCTURE_ID,
                            PROGRAMME_GROUP_ID,
                            COURSE_ID,
                            IS_OPTIONAL,
                            ACADEMIC_YEAR
                            )
                            VALUES(?COURSE_STRUCTURE_ID,?PROGRAMME_GROUP_ID,?COURSE_ID,?IS_OPTIONAL,?ACADEMIC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.UpdateCoursePattern:
                    {
                        query = @"UPDATE CP_COURSE_PATTERN
                                SET
                                COURSE_STRUCTURE_ID = ?COURSE_STRUCTURE_ID,
                                PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID,
                                COURSE_ID = ?COURSE_ID,
                                IS_OPTIONAL = ?IS_OPTIONAL,
                                ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                WHERE COURSE_PATTERN_ID = ?COURSE_PATTERN_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteCoursePattern:
                    {
                        query = @"UPDATE CP_COURSE_PATTERN SET IS_DELETED=1 WHERE COURSE_PATTERN_ID = ?COURSE_PATTERN_ID;";
                        break;
                    }
                #endregion

                #region UgcCourseType
                case AcademicSQLCommands.UgcCourseTypeList:
                    {
                        query = @"SELECT 
                                UGC_COURSE_TYPE_ID,
                                UGC_COURSE_TYPE,
                                UCP.IS_ACTIVE,
                                P.PART AS PART_ID,
                                PL.PROGRAMME_LEVEL
                            FROM
                                CP_UGC_COURSE_TYPE AS UCP
                                    LEFT JOIN
                                SUP_PART AS P ON P.PART_ID = UCP.PART_ID AND P.IS_DELETED!=1
                                    LEFT JOIN
                                SUP_PROGRAMME_LEVEL AS PL ON PL.PROGRAMME_LEVEL_ID = UCP.PROGRAMME_LEVEL AND PL.IS_DELETED!=1 WHERE UCP.IS_DELETED!=1 ORDER BY UGC_COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertUgcCourseType:
                    {
                        query = @"INSERT INTO cp_ugc_course_type
                                (
                                `UGC_COURSE_TYPE`,
                                `PART_ID`,
                                `PROGRAMME_LEVEL`)
                                VALUES
                                (?UGC_COURSE_TYPE,
                                ?PART_ID,
                                ?PROGRAMME_LEVEL);";
                        break;
                    }
                case AcademicSQLCommands.UpdateUgcCourseType:
                    {
                        query = @"UPDATE `CP_UGC_COURSE_TYPE`
                                SET
                                `UGC_COURSE_TYPE` =?UGC_COURSE_TYPE,
                                `PART_ID` = ?PART_ID,
                                `PROGRAMME_LEVEL` = ?PROGRAMME_LEVEL
                                WHERE `UGC_COURSE_TYPE_ID` = ?UGC_COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteUgcCourseType:
                    {
                        query = @"UPDATE CP_UGC_COURSE_TYPE SET IS_DELETED=1 WHERE UGC_COURSE_TYPE_ID=?UGC_COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.CheckExistUgcCourseType:
                    {
                        query = @"SELECT UGC_COURSE_TYPE_ID FROM CP_UGC_COURSE_TYPE WHERE UGC_COURSE_TYPE=?UGC_COURSE_TYPE AND PART_ID=?PART_ID AND PROGRAMME_LEVEL=?PROGRAMME_LEVEL;";
                        break;
                    }
                case AcademicSQLCommands.FetchUgcCourseTypeById:
                    {
                        query = @"SELECT UGC_COURSE_TYPE_ID, UGC_COURSE_TYPE, IS_ACTIVE, PART_ID, PROGRAMME_LEVEL FROM CP_UGC_COURSE_TYPE WHERE IS_DELETED!=1 AND UGC_COURSE_TYPE_ID=?UGC_COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseTypeGroupList:
                    {
                        query = @"SELECT 
                                GROUP_ID,
                                CTG.COURSE_TYPE_ID,
                                CT.COURSE_TYPE,
                                CT.INTERNAL,
                                CT.EXTERNAL,
                                TOTAL,
                                CTG.CA_GROUP,
                                GROUP_MARK
                            FROM
                                CP_COURSE_TYPE_GROUP AS CTG
                                    INNER JOIN
                                CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CTG.COURSE_TYPE_ID
                                    AND CT.IS_DELETED != 1
                            WHERE
                                CTG.IS_DELETED != 1
                            ORDER BY CTG.COURSE_TYPE_ID;";
                        break;
                    }
                #endregion

                #region Course Type Group
                case AcademicSQLCommands.FetchCourseTypeGrupByCourseTypeId:
                    {
                        //query = @"SELECT 
                        //            GROUP_ID,
                        //            CTG.COURSE_TYPE_ID,
                        //            CT.COURSE_TYPE,
                        //            CT.INTERNAL,
                        //            CT.EXTERNAL,
                        //            TOTAL,
                        //            CTG.CA_GROUP,
                        //            GROUP_MARK,
                        //            CT.NO_OF_COMPONENTS
                        //        FROM
                        //            CP_COURSE_TYPE_GROUP AS CTG
                        //                INNER JOIN
                        //            CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CTG.COURSE_TYPE_ID
                        //                AND CT.IS_DELETED != 1
                        //        WHERE
                        //            CTG.IS_DELETED != 1 AND CTG.COURSE_TYPE_ID=?COURSE_TYPE_ID;";
                        query = @"
                                    SELECT 
                                    GROUP_ID,
                                    CTG.COURSE_TYPE_ID,
                                    CT.COURSE_TYPE,
                                    CT.INTERNAL,
                                    CT.EXTERNAL,
                                    TOTAL,
                                    CTG.CA_GROUP,
                                    GROUP_MARK,
                                    CT.NO_OF_COMPONENTS,
                                    IF(CT.COURSE_TYPE_ID =?COURSE_TYPE_ID,
                                        'SELECTED',
                                        '') AS SELECTED
                                      
                                     
                                FROM
                                    CP_COURSE_TYPE AS CT
                                        LEFT JOIN
                                    CP_COURSE_TYPE_GROUP AS CTG ON CTG.COURSE_TYPE_ID = CT.COURSE_TYPE_ID
                                        AND CTG.IS_DELETED != 1 
                                       
                                        WHERE CT.COURSE_TYPE_ID=?COURSE_TYPE_ID
                                        ORDER BY CT.COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.SaveCourseTypeGroup:
                    {
                        query = @"INSERT INTO CP_COURSE_TYPE_GROUP(COURSE_TYPE_ID, CA_GROUP, GROUP_MARK, ACADEMIC_YEAR)
                                VALUES(?COURSE_TYPE_ID, ?CA_GROUP, ?GROUP_MARK, ?AC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.UpdateCourseTypeGroup:
                    {
                        query = @"UPDATE CP_COURSE_TYPE_GROUP 
                                    SET 
                                        CA_GROUP = ?CA_GROUP,
                                        GROUP_MARK = ?GROUP_MARK,
                                        ACADEMIC_YEAR =?AC_YEAR
                                    WHERE
                                        GROUP_ID =?GROUP_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region ProgrammeGroup
                case AcademicSQLCommands.FetchProgrammeGroupList:
                    {
                        query = @"SELECT 
                                PG.PROGRAMME_GROUP_ID,
                                P.PROGRAMME_ID,
                                AP.APPLICATION_TYPE,
                                P.PROGRAMME_NAME,
                                S.SHIFT_NAME,
                                PM.PROGRAMME_MODE
                            FROM
                                CP_PROGRAMME_GROUP AS PG
                                    INNER JOIN
                                CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                    AND P.IS_DELETED != 1
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                    AND PM.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SHIFT AS S ON S.SHIFT_ID = PG.SHIFT
                                    AND S.IS_DELETED != 1
                                    INNER JOIN
                                SUP_APPLICATION_TYPE AS AP ON AP.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                    AND AP.IS_DELETED != 1
                            WHERE
                                PG.IS_DELETED != 1
                            ORDER BY APPTYPE_ID , PG.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case AcademicSQLCommands.InsertProgrammeGroup:
                    {
                        query = @"INSERT INTO `CP_PROGRAMME_GROUP`
                                (
                                `APPTYPE_ID`,
                                `PROGRAMME_ID`,
                                `ACADEMIC_YEAR`,
                                `SHIFT`,
                                `PROGRAMME_MODE`)
                                VALUES
                                (
                                 ?APPTYPE_ID,
                                 ?PROGRAMME_ID,
                                 ?ACADEMIC_YEAR,
                                 ?SHIFT,
                                 ?PROGRAMME_MODE
                                );";
                        break;
                    }
                case AcademicSQLCommands.UpdateProgrammeGroup:
                    {
                        query = @"UPDATE `CP_PROGRAMME_GROUP`
                                SET
                                `APPTYPE_ID` = ?APPTYPE_ID,
                                `PROGRAMME_ID` =?PROGRAMME_ID,
                                `ACADEMIC_YEAR` = ?ACADEMIC_YEAR,
                                `SHIFT` = ?SHIFT,
                                `PROGRAMME_MODE` =?PROGRAMME_MODE
                                WHERE `PROGRAMME_GROUP_ID` = ?PROGRAMME_GROUP_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteProgrammeGroup:
                    {
                        query = @"UPDATE CP_PROGRAMME_GROUP SET IS_DELETED=1 WHERE PROGRAMME_GROUP_ID=?PROGRAMME_GROUP_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchSelectedProgrammeForProgrammeGroup:
                    {
                        query = @"SELECT 
                                PG.PROGRAMME_GROUP_ID,
                                PG.APPTYPE_ID,
                                PG.SHIFT,
                                PG.PROGRAMME_MODE,
                                CP.PROGRAMME_ID,
                                CP.PROGRAMME_NAME,
                                IF(PROGRAMME_GROUP_ID IS NOT NULL,
                                    'SELECTED',
                                    '') AS `STATUS`
                            FROM
                                CP_PROGRAMME AS CP
                                    LEFT JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_ID = CP.PROGRAMME_ID
                                    AND PG.IS_DELETED != 1
                                    AND PG.SHIFT = ?SHIFT
                                    AND PG.APPTYPE_ID = ?APPTYPE_ID
                                    AND PG.PROGRAMME_MODE = ?PROGRAMME_MODE;";
                        break;
                    }
                case AcademicSQLCommands.FetchProgrammeForRemoveDublicates:
                    {
                        query = @"SELECT 
                                PG.PROGRAMME_GROUP_ID,
                                PG.APPTYPE_ID,
                                PG.SHIFT,
                                PG.PROGRAMME_MODE,
                                P.PROGRAMME_ID,
                                P.PROGRAMME_NAME
                            FROM
                                CP_PROGRAMME_GROUP AS PG
                                    INNER JOIN
                                CP_PROGRAMME AS P ON PG.PROGRAMME_ID = P.PROGRAMME_ID
                                    AND P.IS_DELETED != 1
                            WHERE
                                PG.IS_DELETED != 1 AND PG.SHIFT = ?SHIFT
                                    AND PG.APPTYPE_ID = ?APPTYPE_ID
                                    AND PG.PROGRAMME_MODE = ?PROGRAMME_MODE;";
                        break;
                    }
                #endregion
                #region CIAGroupComponent
                case AcademicSQLCommands.ListCIAGroupComponent:
                    {
                        query = @"SELECT 
                                    CT.COURSE_TYPE_ID,
                                    CT.COURSE_TYPE,
                                    CT.INTERNAL,
                                    CG.CA_GROUP,
                                    CG.GROUP_MARK,
                                    SC.COMPONENT,
                                    CAC.CIA_GROUP_COMPONENT_ID
                                FROM
                                    CIA_GROUP_COMPONENT CAC
                                        INNER JOIN
                                    CP_COURSE_TYPE_GROUP AS CG ON CG.GROUP_ID = CAC.CIA_GROUP_ID
                                        AND CG.IS_DELETED != 1
                                        INNER JOIN
                                    CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CG.COURSE_TYPE_ID
                                        AND CT.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_CIA_COMPONENTS AS SC ON SC.COMPONENT_ID = SUP_CIA_COMPONENT_ID
                                        AND SC.IS_DELETED != 1
                                WHERE
                                    CAC.IS_DELETED != 1
                                ORDER BY CG.COURSE_TYPE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseTypeandCourseGroupandComponentByCourseTypeId:
                    {
                        query = @"SELECT
                                    CTG.COURSE_TYPE_ID,
                                    CTG.CA_GROUP,
                                    IF(CT.COURSE_TYPE_ID =?COURSE_TYPE_ID,
                                        'SELECTED',
                                        '') AS SELECTED,
                                        CGC.CIA_GROUP_COMPONENT_ID,
                                          GROUP_ID,
                                        CGC.SUP_CIA_COMPONENT_ID,
                                        CGC.MAX_MARK,
                                        CGC.MIN_MARK,
                                        CGC.ORDER_ID
                                FROM
                                    CP_COURSE_TYPE AS CT
                                        INNER JOIN
                                    CP_COURSE_TYPE_GROUP AS CTG ON CTG.COURSE_TYPE_ID = CT.COURSE_TYPE_ID
                                        AND CTG.IS_DELETED != 1
                                        LEFT JOIN
                                        CIA_GROUP_COMPONENT AS CGC
                                        ON CGC.CIA_GROUP_ID = CTG.GROUP_ID
                                        AND CGC.IS_DELETED != 1

                                        WHERE CT.COURSE_TYPE_ID =?COURSE_TYPE_ID
                                        ORDER BY CT.COURSE_TYPE_ID";
                        break;
                    }

                case AcademicSQLCommands.SaveCiaGroupComponent:
                    {
                        query = @"INSERT INTO 
                                CIA_GROUP_COMPONENT(CIA_GROUP_ID, SUP_CIA_COMPONENT_ID,MAX_MARK,MIN_MARK,ORDER_ID)
                                VALUES(?CIA_GROUP_ID,?SUP_CIA_COMPONENT_ID,?MAX_MARK,?MIN_MARK,?ORDER_ID);";
                        break;
                    }
                case AcademicSQLCommands.UpdateCiaGroupComponent:
                    {
                        query = @"UPDATE CIA_GROUP_COMPONENT
                                SET CIA_GROUP_ID=?CIA_GROUP_ID,SUP_CIA_COMPONENT_ID=?SUP_CIA_COMPONENT_ID,
                                MAX_MARK=?MAX_MARK,MIN_MARK=?MIN_MARK ,ORDER_ID=?ORDER_ID
                                WHERE CIA_GROUP_COMPONENT_ID=?CIA_GROUP_COMPONENT_ID
                                AND IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.DeletedCiaGroupComponentById:
                    {
                        query = @"UPDATE CIA_GROUP_COMPONENT
                                SET IS_DELETED=?IS_DELETED
                                WHERE CIA_GROUP_COMPONENT_ID=?CIA_GROUP_COMPONENT_ID
                                AND IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region CoursePatternReport
                case AcademicSQLCommands.FetchCoursePatternReportByAcademicWise:
                    {
                        query = @"
                                SELECT 
                                    CUCT.UGC_COURSE_TYPE,
                                    CF.FACULTY,
                                    SPL.PROGRAMME_LEVEL,
                                    CCS.PROGRAMME_YEAR,
                                   CONCAT(CP.PROGRAMME_NAME,
                                    ' ',
                                    SPM.PROGRAMME_MODE,
                                    ' ',
                                    '(SHIFT',
                                    ' ',
                                    SH.SHIFT_NAME,
                                    ')') AS PROGRAMME_NAME,
                                    CONCAT(IFNULL(CCR.COURSE_TITLE, ''),
                                    '(',
                                    IFNULL(CCR.COURSE_CODE, ''),
                                    ')') AS COURSE_TITLE,
                                    CCR.COURSE_CODE
                                FROM
                                    CP_COURSE_STRUCTURE AS CCS
                                        INNER JOIN
                                    CP_COURSE_PATTERN AS CCP ON CCP.COURSE_STRUCTURE_ID = CCS.COURSE_STRUCTURE_ID
                                        AND CCP.ACADEMIC_YEAR =?AC_YEAR
                                        AND CCP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                        INNER JOIN
                                    SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS ACS ON ACS.PROGRAMME_GROUP_ID = CPG.PROGRAMME_GROUP_ID
                                        AND ACS.SEMESTER = CCR.SEMESTER_ID
                                        AND ACS.ACADEMIC_YEAR =?AC_YEAR
                                        AND ACS.IS_DELETED != 1
                                        INNER JOIN
                                    CP_UGC_COURSE_TYPE AS CUCT ON CUCT.UGC_COURSE_TYPE_ID = CCS.UGC_TYPE_ID
                                        INNER JOIN
                                    CP_FACULTY AS CF ON CF.FACULTY_ID = CCS.FACULTY_ID
                                        INNER JOIN
                                    SUP_PROGRAMME_LEVEL AS SPL ON SPL.PROGRAMME_LEVEL_ID = CCS.PROGRAMME_LEVEL
                                WHERE
                                        CCS.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        AND CCS.IS_DELETED != 1
                                        AND CCS.FACULTY_ID =?FACULTY_ID
                                        AND ACS.IS_ACTIVE = 1
                                        AND CCS.PROGRAMME_LEVEL =?PROGRAMME_LEVEL
                                        AND CPG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                        AND CCS.SEMESTER_ID IN (?SEMESTER_ID)
                                ORDER BY CPG.PROGRAMME_GROUP_ID,SPL.PROGRAMME_LEVEL,CCS.PROGRAMME_YEAR,CCR.PART;";
                        break;
                    }
                    #endregion

                
                #region ClassIncharges
                case AcademicSQLCommands.FetchClassessByDepartmentshift:
                    {
                        query = @"SELECT 
                                                CI.CLASS_INCHARGE_ID,
                                                CC.CLASS_ID,
                                                CC.CLASS_NAME,
                                                CI.STAFF_ID,
                                                SP.FIRST_NAME,
                                               D.DEPARTMENT
                                            FROM
                                                CP_CLASSES AS CC
                                                LEFT JOIN
                                                CLASS_INCHARGE AS CI ON CC.CLASS_ID = CI.CLASS_ID AND CI.IS_DELETED != 1 
	                                            LEFT JOIN
                                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CI.STAFF_ID
	                                            LEFT JOIN
                                                CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CC.DEPARTMENT 
                                            WHERE
                                              CC.DEPARTMENT = ?DEPARTMENT AND CC.SHIFT = ?SHIFT
                                              AND CC.CLASS_TYPE=2
                                            ORDER BY CC.CLASS_ID ,CC.CLASS_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.SaveClassIncharges:
                    {
                        query = @"INSERT INTO CLASS_INCHARGE(STAFF_ID,CLASS_ID,ACADEMIC_YEAR) VALUES(?STAFF_ID,?CLASS_ID,?ACADEMIC_YEAR);";
                        break;
                    }
                case AcademicSQLCommands.FetchClassInchargeByClassandStaff:
                    {
                        query = @"SELECT 
                                        STAFF_ID, CLASS_ID
                                    FROM
                                        CLASS_INCHARGE
                                    WHERE
                                        CLASS_ID = ?CLASS_ID AND STAFF_ID = ?STAFF_ID
                                            AND IS_ACTIVE = 0
                                           ;";
                        break;
                    }
                case AcademicSQLCommands.UpdateIsDeletedinClassIncharge:
                    {
                        query = @"UPDATE CLASS_INCHARGE SET IS_DELETED=1 WHERE CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case AcademicSQLCommands.UpdateClassIncharge:
                    {
                        query = @"UPDATE CLASS_INCHARGE SET IS_DELETED=0 WHERE CLASS_ID=?CLASS_ID AND STAFF_ID=?STAFF_ID;";
                        break;
                    }
                #endregion
                #region
                case AcademicSQLCommands.FetchEventReisterForm:
                    {
                        query = @"SELECT 
                                EVENT_ID,
                                CC.CLASS_NAME,
                                EVENT_NAME,
                                EVENT_TIME,
                                EVR.ACADEMIC_YEAR,
                                SST.SEMESTER_TYPE,
                                EVENT_VENUE,
                                TITLE,
                                DATE_FORMAT(DATE, '%d/%m/%Y') DATE,
                                SVM.MODE_NAME,
                                SR.ROLE_NAME,
                                NO_OF_REGISTER,
                                EVR.IS_ACTIVE,
                                EVR.IS_DELETED
                            FROM
                                EVENT_REGISTER_FORM AS EVR
                                    INNER JOIN
                                SUP_ROLE AS SR ON SR.ROLE_ID = EVR.ROLE
                                    INNER JOIN
                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SEMESTER_ID
                                    INNER JOIN
                                SUP_EVENT_MODE AS SVM ON SVM.MODE_ID = EVR.MODE
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = EVR.SEMESTER_ID
                                    left JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID=EVR.CLASS_ID
                            WHERE
                                EVR.IS_DELETED != 1;";

                        break;
                    }
                #endregion

                #region Semester Duration
                case AcademicSQLCommands.FetchSemesterDuration:
                    {
                        query = @"   SELECT 
                                                DURATION_ID,
                                                SEMESTER_NAME,
                                                DATE_FORMAT(FROM_DATE, '%d/%m/%Y') FROM_DATE,
                                                DATE_FORMAT(TO_DATE, '%d/%m/%Y') TO_DATE,
                                                ACADEMIC_YEAR,
                                                sst.SEMESTER_TYPE,
                                                sd.IS_ACTIVE,
                                                sd.IS_DELETED
                                            FROM
                                                SEMESTER_DURATION AS sd
                                                    INNER JOIN
                                                sup_semester_type AS sst ON sst.SEMESTER_TYPE_ID = sd.SEMESTER_TYPE
                                            WHERE
                                                sd.IS_DELETED != 1;";
                                          
                        break;
                    }

                case AcademicSQLCommands.AddSemesterDuration:
                    {
                        query = @"INSERT INTO SEMESTER_DURATION
                                                (
                                                 SEMESTER_NAME,
                                                 FROM_DATE,
                                                 TO_DATE,
                                                 ACADEMIC_YEAR,
                                                 SEMESTER_TYPE,
                                                 IS_ACTIVE)

                                                VALUES
                                                (
                                               ?SEMESTER_NAME,
                                               ?FROM_DATE,
                                               ?TO_DATE,
                                               ?ACADEMIC_YEAR,
                                               ?SEMESTER_TYPE,
                                               ?IS_ACTIVE);";
                        break;
                    }


                case AcademicSQLCommands.AddEventRegister:
                    {
                        query = @" INSERT INTO EVENT_REGISTER_FORM 
                                                         (
                                                         CLASS_ID,
                                                         EVENT_NAME,
                                                         EVENT_TIME,
                                                         EVENT_VENUE,
                                                         ACADEMIC_YEAR,
                                                         SEMESTER_ID,
                                                         TITLE,
                                                         DATE,
                                                         MODE,
                                                         ROLE,
                                                         NO_OF_REGISTER,
                                                         IS_ACTIVE)

                                                         VALUES
                                                         (
                                                         ?CLASS_ID,
                                                         ?EVENT_NAME,
                                                         ?EVENT_TIME,
                                                         ?EVENT_VENUE,
                                                         ?ACADEMIC_YEAR,
                                                         ?SEMESTER_ID,
                                                         ?TITLE,
                                                         ?DATE,
                                                         ?MODE,
                                                         ?ROLE,
                                                         ?NO_OF_REGISTER,
                                                         ?IS_ACTIVE);";


                        break;
                    }


                case AcademicSQLCommands.UpdateSemesterDuration:
                    {
                        query = @"UPDATE 
                                                SEMESTER_DURATION
                                                SET 
                                                SEMESTER_NAME=?SEMESTER_NAME,
                                                FROM_DATE=?FROM_DATE,
                                                TO_DATE=?TO_DATE,
                                                ACADEMIC_YEAR=?ACADEMIC_YEAR,
                                                SEMESTER_TYPE=?SEMESTER_TYPE,
                                                IS_ACTIVE=?IS_ACTIVE
                                                WHERE
                                                DURATION_ID=?DURATION_ID
                                                AND IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.UpdateEventRegisteration:
                    {
                        query = @"      UPDATE
                                              EVENT_REGISTER_FORM
                                              SET
                                              CLASS_ID=?CLASS_ID,
                                              EVENT_NAME=?EVENT_NAME,
                                              EVENT_TIME=?EVENT_TIME,
                                              EVENT_VENUE=?EVENT_VENUE,
                                              TITLE=?TITLE,
                                              DATE=?DATE,
                                              MODE=?MODE,
                                              ROLE=?ROLE,
                                              NO_OF_REGISTER=?NO_OF_REGISTER,
                                              IS_ACTIVE=?IS_ACTIVE
                                              WHERE
                                              EVENT_ID=?EVENT_ID;";
                        break;
                    }

                case AcademicSQLCommands.GetStudentDetailsForEventResgister:
                    {
                        query = @"SELECT 
                                        STU.CLASS_ID AS CLASS_ID ,
                                        TY.SEMESTER_TYPE_ID AS SEMESTER_ID,
                                        STU.ACADEMIC_YEAR AS ACADEMIC_YEAR
                                      FROM
                                        STU_CLASS  AS STU
                                           INNER JOIN 
										ACADEMIC_SEMESTER AS ACS ON STU.BATCH=ACS.BATCH
                                           AND STU.PROGRAMME_GROUP_ID=ACS.PROGRAMME_GROUP_ID AND ACS.IS_ACTIVE=1
                                           INNER JOIN
										SUP_SEMESTER AS SEM ON ACS.SEMESTER=SEM.SUP_SEMESTER_ID  AND SEM.IS_ACTIVE=1
                                         INNER JOIN
                                         SUP_SEMESTER_TYPE AS TY ON SEM.SEMESTER_TYPE=TY.SEMESTER_TYPE_ID AND TY.IS_ACTIVE=1
                                        WHERE 
                                         STU.STUDENT_ID=?STUDENT_ID
                                         AND STU.ACADEMIC_YEAR=?ACADEMIC_YEAR;";
                        break;
                    }

                case AcademicSQLCommands.GetEventsForStudent:
                    {
                        query = @"SELECT 
                                        e.EVENT_ID,
                                        e.EVENT_NAME,
                                        e.EVENT_TIME,
                                        e.EVENT_VENUE,
                                        e.TITLE,
                                        e.DATE,
                                        e.MODE,
                                        m.MODE_NAME AS MODE_NAME,         
                                        e.ROLE,

                                        e.NO_OF_REGISTER AS TOTAL_SEATS,

                                        (SELECT COUNT(*) 
                                         FROM STUDENT_EVENT_REGISTER 
                                         WHERE EVENT_ID = e.EVENT_ID AND IS_DELETED != 1) AS REGISTERED_COUNT,

                                        (e.NO_OF_REGISTER -
                                            (SELECT COUNT(*) 
                                             FROM STUDENT_EVENT_REGISTER 
                                             WHERE EVENT_ID = e.EVENT_ID AND IS_DELETED != 1)
                                        ) AS AVAILABLE_SEATS,

                                        (SELECT COUNT(*) 
                                         FROM STUDENT_EVENT_REGISTER 
                                         WHERE EVENT_ID = e.EVENT_ID 
                                           AND STUDENT_ID = ?STUDENT_ID AND IS_DELETED != 1) AS IS_REGISTERED,

                                        e.CLASS_ID,
                                        e.ACADEMIC_YEAR,
                                        e.SEMESTER_ID,
                                        e.IS_ACTIVE,
                                        e.IS_DELETED

                                    FROM EVENT_REGISTER_FORM AS e
                                    INNER JOIN SUP_EVENT_MODE AS m 
                                            ON e.MODE = m.MODE_ID
                                    WHERE e.IS_ACTIVE = 1 
                                      AND e.IS_DELETED != 1
                                      AND e.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                      AND e.SEMESTER_ID = ?SEMESTER_ID
                                      AND e.CLASS_ID = ?CLASS_ID
                                      AND e.ROLE = ?SROLES
                                    ORDER BY e.DATE ASC;";
                        break;
                    }
                case AcademicSQLCommands.GetQuestionType:
                    {
                        query = @"SELECT 
                                        QUESTION_TYPE_ID,
                                        QUESTION_TYPE 
                                       FROM 
                                         SUP_QUIZ_QUESTION_TYPE;";
                        break;
                    }

                case AcademicSQLCommands.EventInsertQuestionSQL:
                    {
                        query = @"INSERT INTO EVENT_FBQUESTIONS
                                    (
                                        EVENT_ID,
                                        ROLE_ID,
                                        QUESTION,
                                        QUESTION_TYPE,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?EVENT_ID,
                                        ?ROLE_ID,
                                        ?QUESTION,
                                        ?QUESTION_TYPE,
                                        ?ACADEMIC_YEAR
                                    );";
                        break;
                    }
                case AcademicSQLCommands.EventUpdateQuestionSQL:
                    {
                        query = @"UPDATE EVENT_FBQUESTIONS
                                        SET
                                            EVENT_ID = ?EVENT_ID,
                                            ROLE_ID = ?ROLE_ID,
                                            QUESTION = ?QUESTION,
                                            QUESTION_TYPE = ?QUESTION_TYPE
                                        WHERE
                                            QUESTION_ID = ?QUESTION_ID";
                        break;
                    }

                case AcademicSQLCommands.AddStudentEventRegister:
                    {
                        query = @"INSERT INTO STUDENT_EVENT_REGISTER
                                    (
                                        STUDENT_ID,
                                        EVENT_ID
                                    )
                                    VALUES
                                    (
                                        ?STUDENT_ID,
                                        ?EVENT_ID
                                    );";
                        break;
                    }

                case AcademicSQLCommands.GetAllEventsBySemester:
                    {
                        query = @"SELECT 
                                    e.EVENT_ID,
                                    e.EVENT_NAME,
                                    e.EVENT_TIME,
                                    e.TITLE
                                FROM EVENT_REGISTER_FORM e
                                WHERE e.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                  AND e.SEMESTER_ID IN (
                                        SELECT SEMESTER_TYPE_ID
                                        FROM SUP_SEMESTER_TYPE
                                        WHERE IS_ACTIVE = 1
                                    )
                                  AND e.IS_DELETED != 1
                                  AND e.IS_ACTIVE = 1
                                ORDER BY e.DATE DESC;";
                        break;
                    }
                case AcademicSQLCommands.GetRoleByEvent:
                    {
                        query = @"SELECT 
                                    r.ROLE_ID,
                                    r.ROLE_NAME
                                    FROM SUP_ROLE AS r
                                    INNER JOIN 
                                    EVENT_REGISTER_FORM AS er ON er.ROLE=r.ROLE_ID
                                    WHERE er.EVENT_ID=?EVENT_ID;";
                        break;
                    }
                case AcademicSQLCommands.GetQuestionByEventRoleID:
                    {
                        query = @"SELECT 
                                q.QUESTION_ID,
                                q.EVENT_ID,
                                q.QUESTION,
                                q.TAMIL_QUESTION,
                                q.QUESTION_TYPE AS QUESTION_TYPE_ID,
                                qt.QUESTION_TYPE AS QUESTION_TYPE_NAME,
                                q.QUESTION_GROUP,
                                q.ORDERED_BY,
                                q.ACADEMIC_YEAR,
                                q.ROLE_ID
                            FROM event_fbquestions q
                            LEFT JOIN SUP_QUIZ_QUESTION_TYPE qt
                                   ON q.QUESTION_TYPE = qt.QUESTION_TYPE_ID
                            WHERE q.EVENT_ID = ?EVENT_ID
                              AND q.ROLE_ID = ?ROLE_ID
                              AND q.IS_DELETED = 0
                            ORDER BY q.ORDERED_BY ASC;
";
                        break;
                    }

                case AcademicSQLCommands.GETQUESTIONBYID:
                    {
                        query = @"SELECT 
                                    q.QUESTION_ID,
                                    q.EVENT_ID,
                                    q.ROLE_ID,
                                    q.QUESTION,
                                    q.TAMIL_QUESTION,
                                    q.QUESTION_TYPE,
                                    t.QUESTION_TYPE_ID,
                                    t.QUESTION_TYPE AS QUESTION_TYPE_NAME,
                                    q.QUESTION_GROUP,
                                    q.ORDERED_BY,
                                    q.ACADEMIC_YEAR,
                                    q.IS_DELETED
                                FROM EVENT_FBQUESTIONS q
                                LEFT JOIN SUP_QUIZ_QUESTION_TYPE t
                                       ON q.QUESTION_TYPE = t.QUESTION_TYPE_ID
                                WHERE q.QUESTION_ID = ?QUESTION_ID
                                  AND q.IS_DELETED = 0;";
                        break;
                    }

                case AcademicSQLCommands.DeleteEventQuetions:
                    {
                        query = @"UPDATE EVENT_FBQUESTIONS
                                        SET
                                            IS_DELETED =1
                                        WHERE
                                            QUESTION_ID = ?QUESTION_ID";
                        break;
                    }


                case AcademicSQLCommands.FecthEventQuestionandObjective:
                    {
                        query = @"SELECT 
                                    m.QUESTION_OBJECT_ID,
                                    q.QUESTION_ID,
                                    q.EVENT_ID,
                                    q.QUESTION,
                                    qt.QUESTION_TYPE AS QUESTION_TYPE,
                                    q.ROLE_ID,
                                    o.OBJECTIVE_ID,
                                    o.OBJECTIVES
                                FROM event_fbquestions q
                                LEFT JOIN SUP_QUIZ_QUESTION_TYPE qt
                                       ON q.QUESTION_TYPE = qt.QUESTION_TYPE_ID
                                LEFT JOIN event_fb_question_objective_mapping m 
                                       ON q.QUESTION_ID = m.QUESTION_ID
                                       AND m.IS_DELETED!=1
                                LEFT JOIN EVENT_FBOBJECTIVES o
                                       ON m.OBJECTIVE_ID = o.OBJECTIVE_ID
                                WHERE q.EVENT_ID = ?EVENT_ID
                                  AND q.ROLE_ID = ?ROLE_ID
                                  AND q.IS_DELETED!=1
                                ORDER BY q.QUESTION_ID, o.OBJECTIVE_ID;";
                        break;
                    }
                case AcademicSQLCommands.GetAllObjectives:
                    {
                          query = @"SELECT OBJECTIVE_ID, OBJECTIVES
                                    FROM EVENT_FBOBJECTIVES
                                    WHERE IS_DELETED != 1
                                    ORDER BY OBJECTIVE_ID ASC;";
                        break;
                    }
                case AcademicSQLCommands.FecthEventQuestion:
                    {
                        query = @"SELECT 
                                q.QUESTION_ID,
                                q.QUESTION,
                                qt.QUESTION_TYPE AS QUESTION_TYPE,
                                q.EVENT_ID,
                                q.ROLE_ID
                            FROM event_fbquestions q
                            LEFT JOIN SUP_QUIZ_QUESTION_TYPE qt
                                   ON q.QUESTION_TYPE = qt.QUESTION_TYPE_ID
                            WHERE q.EVENT_ID = ?EVENT_ID
                              AND q.ROLE_ID = ?ROLE_ID
                              AND q.IS_DELETED !=1
                            ORDER BY q.ORDERED_BY ASC;";
                        break;
                    }
                case AcademicSQLCommands.InsertEventFBQuestionObjectiveMapping:
                    {
                        query = @"INSERT IGNORE  INTO EVENT_FB_QUESTION_OBJECTIVE_MAPPING 
                                (
                                    EVENT_ID,
                                    QUESTION_ID,
                                    OBJECTIVE_ID,
                                    ACADEMIC_YEAR
                                )
                                VALUES (
                                    ?EVENT_ID,
                                    ?QUESTION_ID,
                                    ?OBJECTIVE_ID,
                                    ?ACADEMIC_YEAR
                                );";
                        break;
                    }
                case AcademicSQLCommands.UpdateEventQuestionObjective:
                    {
                        query = @"UPDATE EVENT_FB_QUESTION_OBJECTIVE_MAPPING
                                    SET 
                                        QUESTION_ID = ?QUESTION_ID,
                                        OBJECTIVE_ID = ?OBJECTIVE_ID
                                    WHERE QUESTION_OBJECT_ID = ?QUESTION_OBJECT_ID;";
                        break;
                    }
                case AcademicSQLCommands.FecthEditobjectiveQuestion:
                    {
                        query = @"SELECT 
                                    m.QUESTION_OBJECT_ID,
                                    q.QUESTION_ID,
                                    q.EVENT_ID,
                                    q.QUESTION,
                                    qt.QUESTION_TYPE AS QUESTION_TYPE,
                                    q.ROLE_ID,
                                    o.OBJECTIVE_ID,
                                    o.OBJECTIVES
                                FROM event_fbquestions q
                                LEFT JOIN SUP_QUIZ_QUESTION_TYPE qt
                                       ON q.QUESTION_TYPE = qt.QUESTION_TYPE_ID
                                LEFT JOIN event_fb_question_objective_mapping m 
                                       ON q.QUESTION_ID = m.QUESTION_ID
                                LEFT JOIN EVENT_FBOBJECTIVES o
                                       ON m.OBJECTIVE_ID = o.OBJECTIVE_ID
                                WHERE m.EVENT_ID = ?EVENT_ID
                                  AND m.QUESTION_OBJECT_ID = ?QUESTION_OBJECT_ID
                                ORDER BY q.QUESTION_ID, o.OBJECTIVE_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteEventFBObjective:
                    {
                        query = @"UPDATE EVENT_FB_QUESTION_OBJECTIVE_MAPPING
                                      SET IS_DELETED = 1
                                      WHERE QUESTION_OBJECT_ID = ?QUESTION_OBJECT_ID;";
                        break;
                    }

                case AcademicSQLCommands.FetchSemesterDurationbyId:
                    {
                        query = @"  SELECT
                                            DURATION_ID,
                                            SEMESTER_NAME,
                                           date_format(FROM_DATE,'%d/%m/%Y') FROM_DATE,
                                            date_format( TO_DATE,'%d/%m/%Y') TO_DATE,
                                            ACADEMIC_YEAR,
                                            SEMESTER_TYPE,
                                            IS_ACTIVE
                                           FROM 
                                            SEMESTER_DURATION

                                            WHERE DURATION_ID=?DURATION_ID
                                            AND IS_DELETED !=1;";
                        break;

                    }
              
                case AcademicSQLCommands.FetchEventRegisterById:
                    {
                        query = @"      SELECT
                                            EVENT_ID,
                                            cc.CLASS_ID,
                                            EVENT_NAME,
                                            EVENT_TIME,
                                            EVENT_VENUE,
                                            CC.CLASS_LEVEL AS PROGRAM_YEAR,
                                            CC.CLASS_YEAR,
                                            CC.SHIFT,
                                            ERF.ACADEMIC_YEAR,
                                            SEMESTER_ID,
                                            TITLE,
                                            DATE_FORMAT(DATE,'%d/%m/%Y') DATE,
                                            MODE,
                                            ROLE,
                                            NO_OF_REGISTER,
                                            IS_ACTIVE
                                           FROM 
                                            EVENT_REGISTER_FORM AS ERF
                                            INNER JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID=ERF.CLASS_ID
                                            WHERE EVENT_ID=?EVENT_ID
                                            AND ERF.IS_DELETED !=1;";
                        break;

                    }
              

                case AcademicSQLCommands.DeleteSemesterDurationById:
                    {
                        query = @"UPDATE SEMESTER_DURATION 
                                            SET 
                                            IS_DELETED=?IS_DELETED
                                            WHERE 
                                           DURATION_ID=?DURATION_ID";
                        break;
                    }
                case AcademicSQLCommands.DeleteEventRegisterById:
                    {
                        query = @"UPDATE EVENT_REGISTER_FORM 
                                    SET IS_DELETED = 1
                                    WHERE EVENT_ID = ?EVENT_ID;";
                                    
                        break;
                    }

                case AcademicSQLCommands.FetchAcademicYear:
                    {
                        query = @"SELECT 
                                    DURATION_ID, ACADEMIC_YEAR
                                FROM
                                    semester_duration
                                WHERE
                                    IS_DELETED != 1 ORDER BY DURATION_ID  DESC;";
                        break;

                    }

                #endregion


                case AcademicSQLCommands.CheckIsExistCoursepatternByAcademicyear:
                    {
                        query = @"SELECT COURSE_PATTERN_ID, COURSE_STRUCTURE_ID, PROGRAMME_GROUP_ID, COURSE_ID, IS_OPTIONAL, ACADEMIC_YEAR, IS_ACTIVE FROM CP_COURSE_PATTERN WHERE ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseStructureByAcademicYear:
                    {
                        query = @"SELECT 
                                COURSE_STRUCTURE_ID,
                                CREDIT,
                                PART_ID,
                                UGC_TYPE_ID,
                                HRS_PER_WEEK,
                                MARKS,
                                SEMESTER_ID,
                                FACULTY_ID,
                                PROGRAMME_LEVEL,
                                PROGRAMME_YEAR,
                                ACADEMIC_YEAR,
                                IS_OPTIONAL,
                                IS_NME,
                                IS_ALLIED,
                                IS_ACTIVE
                            FROM
                                CP_COURSE_STRUCTURE
                            WHERE
                                ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED != 1;";
                        break;
                    }
                case AcademicSQLCommands.SelectInsertCoursepatternByCoursestructureId:
                    {
                        query = @"INSERT INTO CP_COURSE_PATTERN (COURSE_STRUCTURE_ID, PROGRAMME_GROUP_ID, COURSE_ID, IS_OPTIONAL, ACADEMIC_YEAR, IS_ACTIVE)
                                SELECT 
                                '@COURSE_STRUCTURE_ID' AS COURSE_STRUCTURE_ID,
                                PROGRAMME_GROUP_ID,
                                COURSE_ID,
                                IS_OPTIONAL,
                                '?AC_YEAR' AS ACADEMIC_YEAR,
                                IS_ACTIVE
                            FROM
                                CP_COURSE_PATTERN
                            WHERE
                                ACADEMIC_YEAR = ?ACADEMIC_YEAR AND IS_DELETED != 1
                                    AND COURSE_STRUCTURE_ID = ?COURSE_STRUCTURE_ID;";
                        break;
                    }

                case AcademicSQLCommands.FetchCoursesFromCoursePatternByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                        CP.PROGRAMME_GROUP_ID,
                                        CP.COURSE_STRUCTURE_ID,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID
                                    FROM
                                        CP_COURSE_PATTERN AS CP
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                            AND C.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                                ACADEMIC_YEAR_ID
                                            FROM
                                                ACADEMIC_YEAR
                                            WHERE
                                                ACTIVE_YEAR = 1 AND IS_DELETED != 1)
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                            AND SEM.SEMESTER = CCR.SEMESTER_ID
                                            AND SEM.IS_DELETED != 1
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SEM.IS_ACTIVE = 1 AND CP.IS_ACTIVE=1
                                            AND CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                            AND CP.IS_DELETED != 1
                                    ORDER BY COURSE_STRUCTURE_ID;";
                        break;
                    }

                #region Rescapes Class Wise Staff
                case AcademicSQLCommands.ListClassCourseStaffForRescapes:
                    {
                        query = @"SELECT 
	                                    CLS_CRSE_STF_ID,
                                        CCS.ACADEMIC_YEAR,
                                        CCR.COURSE_ROOT_ID AS COURSE_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        CS.CLASS_ID,
                                        CS.CLASS_NAME,
                                        CS.SHIFT,
                                        SP.STAFF_ID,
                                        SP.STAFF_CODE,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,''))) AS FIRST_NAME,
                                        D.DEPARTMENT_ID,
                                        D.DEPARTMENT
                                    FROM
                                        CLASSCOURSESTAFFFORRESCAPES AS CCS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CS ON CS.CLASS_ID = CCS.CLASS_ID
                                            INNER JOIN
                                        stf_personal_info AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            LEFT JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=SP.DEPARTMENT
                                    WHERE
                                        CCS.IS_DELETED != 1 AND CCS.ACADEMIC_YEAR=?AC_YEAR
                                    ORDER BY  CCS.CLASS_ID , CCS.STAFF_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchClassCourseStaffForRescapesById:
                    {
                        query = @"SELECT 
                                        CLS_CRSE_STF_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        CLASS_ID,
                                        ACADEMIC_YEAR
                                    FROM
                                        CLASSCOURSESTAFFFORRESCAPES
                                    WHERE
                                       CLS_CRSE_STF_ID = ?CLS_CRSE_STF_ID;";
                        break;
                    }
                case AcademicSQLCommands.IsExistClassCourseStaffForRescapes:
                    {
                        query = @"SELECT 
                                        CLS_CRSE_STF_ID
                                    FROM
                                        CLASSCOURSESTAFFFORRESCAPES
                                    WHERE
                                        CLASS_ID =?CLASS_ID AND COURSE_ID =?COURSE_ID
                                            AND STAFF_ID =?STAFF_ID
                                            AND ACADEMIC_YEAR =?ACADEMIC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case AcademicSQLCommands.SaveClassCourseStaffForRescapes:
                    {
                        query = @"INSERT INTO `classcoursestaffforrescapes`
                                  (
	                                  COURSE_ID,
	                                  STAFF_ID,
	                                  CLASS_ID,
	                                  ACADEMIC_YEAR
                                  )
                                  VALUES
                                  (
                                      ?COURSE_ID,
	                                  ?STAFF_ID,
	                                  ?CLASS_ID,
	                                  ?ACADEMIC_YEAR
                                  );";
                        break;
                    }
                case AcademicSQLCommands.UpdateClassCourseStaffForRescapes:
                    {
                        query= @"UPDATE CLASSCOURSESTAFFFORRESCAPES 
                                 SET 
                                     CLASS_ID = ?CLASS_ID,
                                     COURSE_ID = ?COURSE_ID,
                                     STAFF_ID = ?STAFF_ID,
                                     ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                 WHERE
                                     CLS_CRSE_STF_ID = ?CLS_CRSE_STF_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteClassCourseStaffForRescapes:
                    {
                        query = @"UPDATE CLASSCOURSESTAFFFORRESCAPES SET IS_DELETED=1 WHERE CLS_CRSE_STF_ID=?CLS_CRSE_STF_ID;";
                        break;
                    }
                #endregion

                #region Syllabus Details
                // For Student 
                case AcademicSQLCommands.FetchAllStudiedClassAndYearByUserId:
                    {
                        query = @"SELECT 
                                        SC.STU_CLASS_ID,
                                        SC.STUDENT_ID,
                                        SC.CLASS_ID,
                                        SC.ACADEMIC_YEAR
                                    FROM
                                        STU_CLASS AS SC
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                    ORDER BY ACADEMIC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.FetchRegularSyllabusForStudentByUserId:
                    {
                        query = @"SELECT 
                                        CS.COURSE_STRUCTURE_ID,
                                        D.DEPARTMENT,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        UPPER(CCR.COURSE_TITLE) AS 'COURSE_TITLE',
                                        CCR.SEMESTER_ID,
                                        CCR.PART
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
                                        ACADEMIC_YEAR AS AY ON AY.AC_YEAR = CP.ACADEMIC_YEAR
                                            AND AY.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS S ON S.SEMESTER = CCR.SEMESTER_ID
                                            AND S.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                            AND S.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND S.BATCH = AB.BATCH_ID
                                            AND S.IS_DELETED != 1
                                            LEFT JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=CCR.DEPARTMENT
                                    WHERE
                                        CP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.IS_DELETED != 1
                                    ORDER BY CCR.SEMESTER_ID , CCR.PART;";
                        break;
                    }
                case AcademicSQLCommands.FetchOptionalSyllabusForStudentByUserId:
                    {
                        query = @"SELECT 
                                        D.DEPARTMENT,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        UPPER(CCR.COURSE_TITLE) AS 'COURSE_TITLE',
                                        CCR.SEMESTER_ID,
                                        CCR.PART
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        STU_CLASS AS S ON S.STUDENT_ID = SC.STUDENT_ID
                                            AND S.IS_DELETED != 1
                                            AND S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            AND C.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                            AND AY.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER = CCR.SEMESTER_ID
                                            AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND SEM.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.IS_DELETED != 1
                                            LEFT JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = CCR.DEPARTMENT
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCR.IS_DELETED != 1
                                            AND SC.IS_DELETED != 1
                                    ORDER BY CCR.SEMESTER_ID , CCR.PART;";
                        break;
                    }
                // Course Staff
                case AcademicSQLCommands.FetchSyllabusForCourseStaffByUserId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        ST.APPLICATION_TYPE
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CC.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = CC.CLASS_ID
                                            INNER JOIN
                                        SUP_APPLICATION_TYPE AS ST ON ST.APPLICATION_TYPE_ID = C.CLASS_LEVEL
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS T ON T.SEMESTER_TYPE_ID = CC.SEMESTER_TYPE_ID
                                            AND T.IS_ACTIVE = 1
                                    WHERE
                                        CC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CC.IS_DELETED != 1
                                            AND CC.STAFF_ID = ?STAFF_ID
                                    GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PROGRAMME_LEVEL_ID , CCR.SEMESTER_ID , CCR.PART;";
                        break;
                    }

                // For HOD
                case AcademicSQLCommands.FetchSyllabusForHODByUserId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        ST.APPLICATION_TYPE
                                    FROM
                                        CP_CLASSES AS C
                                            INNER JOIN
                                        CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                            AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_APPLICATION_TYPE AS ST ON ST.APPLICATION_TYPE_ID = C.CLASS_LEVEL
                                    WHERE
                                        C.IS_DELETED != 1
                                            AND C.DEPARTMENT = (SELECT 
                                                DEPARTMENT
                                            FROM
                                                STF_PERSONAL_INFO
                                            WHERE
                                                IS_DELETED != 1 AND STAFF_ID = ?STAFF_ID)
                                    GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY C.CLASS_LEVEL , CCR.SEMESTER_ID , CCR.PART;";
                        break;
                    }
                #endregion

                case AcademicSQLCommands.FetchRegularClassByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                CLASS_ID,
                                CLASS_CODE,
                                CLASS_NAME,
                                CLASS_DESCRIPTION,
                                CLASS_TYPE,
                                CLASS_LEVEL,
                                CLASS_YEAR,
                                DEPARTMENT,
                                SHIFT,
                                COURSE_ID,
                                PROGRAMME_MODE
                            FROM
                                CP_CLASSES
                            WHERE
                                PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                    AND CLASS_TYPE = ?CLASS_TYPE;";
                        break;
                    }
                case AcademicSQLCommands.FetchRegularCourseByProgrammeGroupIdandClassYearId:
                    {
                        query = @"SELECT 
                                    CP.COURSE_ID, CR.COURSE_CODE, CR.COURSE_TITLE,CR.SEMESTER_ID
                                FROM
                                    CP_COURSE_PATTERN AS CP
                                        INNER JOIN
                                    CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                        AND CR.IS_COMPULSORY = 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = ?CLASS_YEAR
                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                            AY.ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR AS AY
                                        WHERE
                                            AY.ACTIVE_YEAR = 1
                                                AND AY.IS_DELETED != 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                        AND SEM.SEMESTER = CR.SEMESTER_ID
                                        AND SEM.BATCH = AB.BATCH_ID
                                        AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SEM.IS_DELETED != 1
                                        AND SEM.IS_ACTIVE = 1
                                WHERE
                                    CP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        AND CP.IS_DELETED != 1;";
                        break;
                    }
                case AcademicSQLCommands.FetchClassCourseByClassId:
                    {
                        query = @"";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseByProgrammeGroupIdandClassYearId:
                    {
                        query = @"SELECT 
                                CP.COURSE_ID, CR.COURSE_CODE, CR.COURSE_TITLE,CR.SEMESTER_ID,CR.IS_COMPULSORY
                            FROM
                                CP_COURSE_PATTERN AS CP
                                    INNER JOIN
                                CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                    INNER JOIN
                                ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = ?CLASS_YEAR
                                    AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                        AY.ACADEMIC_YEAR_ID
                                    FROM
                                        ACADEMIC_YEAR AS AY
                                    WHERE
                                        AY.ACTIVE_YEAR = 1
                                            AND AY.IS_DELETED != 1)
                                    INNER JOIN
                                ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                    AND SEM.SEMESTER = CR.SEMESTER_ID
                                    AND SEM.BATCH = AB.BATCH_ID
                                    AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SEM.IS_DELETED != 1
                                    AND SEM.IS_ACTIVE = 1
                            WHERE
                                CP.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                    AND CP.IS_DELETED != 1;";
                        break;
                    }
                case AcademicSQLCommands.FetchClassCourseStaffByGroupCourseIdClassId:
                    {
                        query = @"SELECT 
                                CLASS_ID,
                                COURSE_ID,
                                COURSE_CODE,
	                            COURSE_TITLE,
                                CT.SHIFT,
                                CT.STAFF_ID,
                                GROUP_CONCAT(ST.STAFF_CODE) AS STAFF_CODE,
                                CR.SEMESTER_ID,
                                SEMESTER_TYPE_ID
                            FROM
                                CP_CLASS_COURSE_STAFF AS CT
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CT.COURSE_ID
		                            INNER JOIN
	                            STF_PERSONAL_INFO AS ST ON ST.STAFF_ID=CT.STAFF_ID
                            WHERE
                                CLASS_ID = ?CLASS_ID AND CT.IS_DELETED != 1
                                    AND SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                    AND CT.ACADEMIC_YEAR = ?AC_YEAR
                            GROUP BY COURSE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchClassCourseStaffByOptionClass:
                    {
                        query = @"SELECT 
                                CL.CLASS_ID,
                                CL.CLASS_NAME,
                                CR.COURSE_CODE,
                                CR.COURSE_TITLE,
                                ST.STAFF_ID
                            FROM
                                CP_CLASSES AS CL
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CL.COURSE_ID
                                    LEFT JOIN
                                CP_CLASS_COURSE_STAFF AS CT ON CT.CLASS_ID = CL.CLASS_ID
                                    AND CT.ACADEMIC_YEAR = ?AC_YEAR
                                    LEFT JOIN
                                STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = CT.STAFF_ID
                            WHERE
                                CL.COURSE_ID = ?COURSE_ROOT_ID AND CL.SHIFT = ?SHIFT
                                    AND CL.IS_DELETED != 1 GROUP BY CL.CLASS_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchOptionalCourseStructureByProgrammeGroupIdandClassYearId:
                    {
                        query = @"SELECT 
                                     CP.COURSE_STRUCTURE_ID,UGC.UGC_COURSE_TYPE
                                FROM
                                    CP_COURSE_PATTERN AS CP
                                        INNER JOIN
                                    CP_COURSE_STRUCTURE AS CS ON CS.COURSE_STRUCTURE_ID = CP.COURSE_STRUCTURE_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID AND CR.IS_COMPULSORY!=1
										INNER JOIN
									cp_ugc_course_type AS UGC ON UGC.UGC_COURSE_TYPE_ID= CR.UGC_COURSE_TYPE
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = ?CLASS_YEAR
                                        AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                            AY.ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR AS AY
                                        WHERE
                                            AY.ACTIVE_YEAR = 1
                                                AND AY.IS_DELETED != 1)
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                        AND SEM.SEMESTER = CR.SEMESTER_ID
                                        AND SEM.BATCH = AB.BATCH_ID
                                        AND SEM.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SEM.IS_DELETED != 1
                                        AND SEM.IS_ACTIVE = 1
                                WHERE
                                    CP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                        AND CP.IS_DELETED != 1 group by COURSE_STRUCTURE_ID;";
                        break;
                    }
                case AcademicSQLCommands.FetchStudentListDoesntAssingnOptionalCourseByCourseStructureByClass:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                SP.FIRST_NAME,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CL.CLASS_NAME
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                            WHERE
                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.CLASS_ID = ?CLASS_ID
                                    AND SC.STUDENT_ID NOT IN (SELECT 
                                        SP.STUDENT_ID
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_PATTERN AS CP ON CP.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                            AND CP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CP.COURSE_ID = SC.COURSE_ID
                                            AND CP.COURSE_STRUCTURE_ID = ?COURSE_STRUCTURE_ID
                                            AND CP.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                                AY.ACADEMIC_YEAR_ID
                                            FROM
                                                ACADEMIC_YEAR AS AY
                                            WHERE
                                                AY.ACTIVE_YEAR = 1
                                                    AND AY.IS_DELETED != 1)
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                            AND SEM.SEMESTER = CCR.SEMESTER_ID
                                            AND SEM.BATCH = AB.BATCH_ID
                                            AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED != 1
                                            AND SEM.IS_ACTIVE = 1
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SC.CLASS_ID = ?CLASS_ID);";
                        break;
                    }
                case AcademicSQLCommands.FetchDublicateStudentOptionalCourseByCourseStructureByClass:
                    {
                        query = @"SELECT 
                                COUNT(SP.STUDENT_ID) AS STUDENT_COUNT,
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                SP.FIRST_NAME,
                                C.CLASS_NAME,
                                group_concat(CCR.COURSE_CODE) AS COURSE_CODE
                            FROM
                                STU_COURSE_INFO AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SC.COURSE_ID
                                    AND CCR.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_PATTERN AS CP ON CP.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                    AND CP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    AND CP.COURSE_ID = SC.COURSE_ID
                                    AND CP.COURSE_STRUCTURE_ID = ?COURSE_STRUCTURE_ID
                                    AND CP.IS_DELETED != 1
                                    INNER JOIN
                                ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                    AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                        AY.ACADEMIC_YEAR_ID
                                    FROM
                                        ACADEMIC_YEAR AS AY
                                    WHERE
                                        AY.ACTIVE_YEAR = 1
                                            AND AY.IS_DELETED != 1)
                                    INNER JOIN
                                ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
                                    AND SEM.SEMESTER = CCR.SEMESTER_ID
                                    AND SEM.BATCH = AB.BATCH_ID
                                    AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                    AND SEM.IS_DELETED != 1
                                    AND SEM.IS_ACTIVE = 1
                                    INNER JOIN
                                CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                            WHERE
                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED != 1
                                    AND SC.CLASS_ID = ?CLASS_ID
                            GROUP BY SC.STUDENT_ID;";
                        break;
                    }
                #region Course Pattern By Incharge
                case AcademicSQLCommands.FetchProgrammeGroupByApplicationTypeAndDepartmentId:
                    {
                        query = @"SELECT 
                                        PG.PROGRAMME_GROUP_ID,
                                        CONCAT(P.PROGRAMME_NAME, ' SHIFT - ', PG.SHIFT) AS PROGRAMME_NAME,
                                        D.FACULTY
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = P.DEPARTMENT
                                    WHERE
                                        PG.IS_DELETED != 1 AND PG.APPTYPE_ID = ?APPTYPE_ID
                                            AND D.DEPARTMENT_ID = ?DEPARTMENT_ID;";
                        break;
                    }
                #endregion

                case AcademicSQLCommands.FetchCalendarDateWiseReport:
                    {
                        query = @"SELECT 
                                    DATE_FORMAT(c.DAY_ORDER_DATE, '%d/%m/%Y') AS DAY_ORDER_DATE,
                                    sdo.DAY AS DAY_ORDER,
                                    C.IS_HOLIDAY,
                                    C.ACADEMIC_YEAR,
                                    c.EVENT
                                FROM
                                    CALENDER AS C
                                        LEFT JOIN
                                    SUP_DAY_ORDER AS SDO ON SDO.DAY_ORDER_ID = C.DAY_ORDER
                                        INNER JOIN
                                    SEMESTER_DURATION AS sd ON sd.academic_year = c.academic_year
                                WHERE
                                    C.ACADEMIC_YEAR = ?AC_YEAR and sd.SEMESTER_TYPE=?SEMESTER_TYPE
                                      --  AND C.IS_HOLIDAY != 1
                                        AND C.IS_DELETED != 1
                                        AND C.DAY_ORDER_DATE BETWEEN sd.FROM_DATE AND sd.TO_DATE
                                ORDER BY C.DAY_ORDER_DATE;";
                        break;
                    }

                case AcademicSQLCommands.FetchStudentTagList:
                    {
                            query = @"     SELECT 
                                            TAG_ID, 
                                            TAG_NAME, 
                                            COLOR_CODE, 
                                            IS_ACTIVE, 
                                            IS_DELETED
                                        FROM
                                            SUP_STUDENT_TAG_LIST
                                        WHERE
                                            IS_DELETED != 1;";
                        break;

                    }


                case AcademicSQLCommands.UpdateStudentTagList:
                    {
                        query = @"                                      
                                            UPDATE 
                                                SUP_STUDENT_TAG_LIST
                                                SET 
                                                TAG_NAME=?TAG_NAME,
                                                COLOR_CODE=?COLOR_CODE,
                                                IS_ACTIVE=?IS_ACTIVE
                                                WHERE
                                                TAG_ID=?TAG_ID;";
                        break;
                    }


                case AcademicSQLCommands.AddStudentTagList:
                    {
                        query = @"INSERT INTO SUP_STUDENT_TAG_LIST
                                                (
                                                 TAG_NAME,
                                                 COLOR_CODE,
                                                 IS_ACTIVE)

                                                VALUES
                                                (
                                               ?TAG_NAME,
                                               ?COLOR_CODE,
                                               ?IS_ACTIVE );";

                        break;
                    }


                case AcademicSQLCommands.FetchStudentTagbyId:
                    {
                        query = @"  SELECT
                                            TAG_ID,
                                            TAG_NAME,
                                            COLOR_CODE,
                                            IS_DELETED,
                                            IS_ACTIVE
                                           FROM 
                                            SUP_STUDENT_TAG_LIST

                                            WHERE TAG_ID=?TAG_ID
                                            AND IS_DELETED !=1;";
                    break;

                    }


                case AcademicSQLCommands.DeleteStudentTagById:
                    {
                        query = @"UPDATE SUP_STUDENT_TAG_LIST 
                                            SET 
                                            IS_DELETED=?IS_DELETED
                                            WHERE 
                                           TAG_ID=?TAG_ID";
                        break;
                    }

                case AcademicSQLCommands.FetchAcademicSyllabusByAcYear:
                    {
                        query = @"SELECT 
                                        ASY.SYLLABUS_ID,
                                        ASY.COURSE_ID,
                                        ASY.FILE_PATH,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE
                                    FROM
                                        ACADEMIC_SYLLABUS ASY
                                            INNER JOIN
                                        CP_COURSE_ROOT CCR ON CCR.COURSE_ROOT_ID = ASY.COURSE_ID
                                    WHERE
                                        ASY.IS_DELETED !=1 AND CCR.IS_DELETED !=1
                                        AND ASY.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case AcademicSQLCommands.InsertAcademicSyllabus:
                    {
                        query = @"INSERT INTO academic_syllabus (COURSE_ID, FILE_PATH,ACADEMIC_YEAR, CREATED_BY) 
                                    VALUES (?COURSE_ID, ?FILE_PATH,?ACADEMIC_YEAR, ?CREATED_BY);";
                        break;
                    }
                case AcademicSQLCommands.UpdateAcademicSyllabus:
                    {
                        query = @"UPDATE academic_syllabus SET FILE_PATH = ?FILE_PATH, MODIFIED_BY = ?MODIFIED_BY 
                                    WHERE SYLLABUS_ID = ?SYLLABUS_ID;";
                        break;
                    }
                case AcademicSQLCommands.DeleteAcademicSyllabus:
                    {
                        query = @"UPDATE academic_syllabus SET IS_DELETED = 1 WHERE SYLLABUS_ID = ?SYLLABUS_ID;";
                        break;
                    }
                case AcademicSQLCommands.CheckSyllabusExist:
                    {
                        query = @"SELECT SYLLABUS_ID FROM academic_syllabus WHERE COURSE_ID = ?COURSE_ID AND IS_DELETED = 0;";
                        break;
                    }
                case AcademicSQLCommands.FetchCourseByCourseCode:
                    {
                        query = @"SELECT COURSE_ROOT_ID, COURSE_CODE, COURSE_TITLE 
                                    FROM CP_COURSE_ROOT 
                                    WHERE COURSE_CODE LIKE ?COURSE_CODE  AND IS_DELETED != 1;";
                        break;
                    }

            }
            return query;
        }
    }
}
