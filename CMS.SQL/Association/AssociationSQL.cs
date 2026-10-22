using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;

namespace CMS.SQL.Association
{
    public static class AssociationSQL
    {
        public static string GetAssociationSQL(AssociationSQLCommands sEnumCommand) 
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case AssociationSQLCommands.ListAssociationGroupSetting:
                    {
                        query = @"SELECT 
                                    ASSOCIATION_GROUP_SETTING_ID,
                                    SETTING_NAME,
                                    DATE_FORMAT(DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    ST.SEMESTER_TYPE,
                                    AGT.IS_ACTIVE
                                FROM
                                    ASSOCIATION_GROUP_SETTING AS AGT
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = AGT.SEMESTER_ID
                                WHERE
                                    AGT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND AGT.IS_DELETED != 1;";
                        break;
                    }
                case AssociationSQLCommands.CheckStudentIsRegisterd:
                    {
                        query = @"SELECT 
                                    AG.ASSOCIATION_GROUP
                                FROM
                                    ASSOCIATION_STUDENT_REGISTRATION AS ASR
                                        INNER JOIN
                                    ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = ASR.COURSE_ID
                                        INNER JOIN
                                    ASSOCIATION_GROUP_SETTING AS ST ON ST.ASSOCIATION_GROUP_SETTING_ID = ASR.SETTING_ID
                                        AND ST.IS_ACTIVE = 1
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS SM ON SM.SEMESTER_TYPE_ID AND SM.IS_ACTIVE = 1
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND ASR.ACADEMIC_YEAR = ?AC_YEAR AND ASR.IS_DELETED!=1;";
                        break;
                    }
                case AssociationSQLCommands.LoadAssociationGroups:
                    {
                        query = @"SELECT 
                                AQ.CLASS_ID,
                                AG.ASSOCICATION_GROUP_ID AS COURSE_ID,
                                ST.ASSOCIATION_GROUP_SETTING_ID AS SETTING_ID,
                                ST.SEMESTER_ID,
                                AG.ASSOCIATION_GROUP,
                                AG.ASSOCICATION_GROUP_ID,
								AQ.QUOTA - SUM(IF(CSR.REGISTRATION_ID!=0,1,0)) AS BALANCE_QUOTA
                       
                            FROM
                                ASSOCIATION_GROUP_CLASS_QUOTA AS AQ
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = AQ.CLASS_ID
                                    INNER JOIN
                                ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = AQ.COURSE_ID
                                    INNER JOIN
                                ASSOCIATION_GROUP_SETTING AS ST ON ST.ASSOCIATION_GROUP_SETTING_ID = AQ.SETTING_ID
                                    AND ST.IS_ACTIVE = 1
                                    AND ST.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SM ON SM.SEMESTER_TYPE_ID = ST.SEMESTER_ID
                                    AND SM.IS_ACTIVE = 1
                                    
                                    LEFT JOIN 
									ASSOCIATION_STUDENT_REGISTRATION AS CSR ON CSR.CLASS_ID=CL.CLASS_ID
                                    AND CSR.IS_DELETED!=1
                                    AND CSR.COURSE_ID=AG.ASSOCICATION_GROUP_ID
                                    AND CSR.SETTING_ID=ST.ASSOCIATION_GROUP_SETTING_ID

                                    
                            WHERE
                                AQ.CLASS_ID =?CLASS_ID AND AQ.IS_ACTIVE = 1
                                    AND AQ.ACADEMIC_YEAR =?AC_YEAR
                                    AND AQ.IS_DELETED != 1 GROUP BY AQ.ASSOCIATION_GROUP_CLASS_QUOTA_ID ORDER BY AG.ASSOCIATION_GROUP_ORDER;";
                        break;
                    }
                case AssociationSQLCommands.FetchClassQuotaForCourseByCourseIdandClassId:
                    {
                        query = @"SELECT 
                                    IFNULL(AQ.QUOTA, '0') AS ALLOTED_SEATS
                                FROM
                                    ASSOCIATION_GROUP_CLASS_QUOTA AS AQ
                                        INNER JOIN
                                    ASSOCIATION_GROUP_SETTING AS ST ON AQ.SETTING_ID = ST.ASSOCIATION_GROUP_SETTING_ID
                                        AND ST.IS_ACTIVE = 1
                                        AND ST.ACADEMIC_YEAR=?AC_YEAR
                                WHERE
                                    AQ.CLASS_ID = ?CLASS_ID AND AQ.COURSE_ID = ?COURSE_ID AND AQ.ACADEMIC_YEAR=?AC_YEAR
                                        AND AQ.IS_DELETED != 1;";
                        break;
                    }
                case AssociationSQLCommands.FetchSumofCourseRegisteredByCourseIdandClassId:
                    {
                        query = @"SELECT 
                                    COUNT(AR.COURSE_ID) AS SUMOFQUTOA
                                FROM
                                    ASSOCIATION_STUDENT_REGISTRATION AS AR
                                        INNER JOIN
                                    ASSOCIATION_GROUP_SETTING AS ST ON AR.SETTING_ID = ST.ASSOCIATION_GROUP_SETTING_ID
                                        AND ST.IS_ACTIVE = 1
                                        AND ST.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    AR.CLASS_ID = ?CLASS_ID AND AR.COURSE_ID = ?COURSE_ID
                                        AND AR.ACADEMIC_YEAR = ?AC_YEAR
                                        AND AR.IS_DELETED != 1;";
                        break;
                    }
                case AssociationSQLCommands.CheckAssociationAvailablity:
                    {
                        query = @"SELECT 
                                (AQ.QUOTA > (SELECT 
                                        COUNT(*) AS CC
                                    FROM
                                        ASSOCIATION_STUDENT_REGISTRATION AS AR
                                    WHERE
                                        AR.CLASS_ID = ?CLASS_ID AND COURSE_ID = ?COURSE_ID
                                            AND AR.ACADEMIC_YEAR = ?AC_YEAR
                                            AND AR.SETTING_ID = 1
                                            AND AR.IS_DELETED != 1)) AS COURSEAVAILABILITY
                            FROM
                                ASSOCIATION_GROUP_CLASS_QUOTA AS AQ
                            WHERE
                                AQ.CLASS_ID = ?CLASS_ID AND AQ.COURSE_ID = ?COURSE_ID
                                    AND AQ.SETTING_ID = ?SETTING_ID
                                    AND AQ.ACADEMIC_YEAR = ?AC_YEAR
                                    AND AQ.IS_DELETED != 1;";
                        break;
                    }
                case AssociationSQLCommands.InsertAssociationStudentRegistration:
                    {
                        query= @"INSERT INTO `ASSOCIATION_STUDENT_REGISTRATION`
                                (`STUDENT_ID`,`CLASS_ID`,`COURSE_ID`,`SETTING_ID`,`ACADEMIC_YEAR`)VALUES
                                (?STUDENT_ID,?CLASS_ID,?COURSE_ID,?SETTING_ID,?ACADEMIC_YEAR);";
                        break;
                    }
                case AssociationSQLCommands.ListAssociationRegistration:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                        ' ',
                                        IFNULL(UPPER(SP.LAST_NAME), '')) AS FIRST_NAME,
                                SP.ROLL_NO,
                                CL.CLASS_NAME,
                                IF(AR.COURSE_ID,
                                    AG.ASSOCIATION_GROUP,
                                    'NOT REGISTERED') AS ASSOCIATION_GROUP,
                                CP.PROGRAMME_NAME,
                                AR.REGISTRATION_ID,
                                SC.CLASS_ID,
                                AR.COURSE_ID,
                                AR.SETTING_ID
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CL.CLASS_LEVEL = 1
                                    AND CL.CLASS_YEAR = 1
                                    AND CL.IS_DELETED != 1
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN
                                ASSOCIATION_GROUP_SETTING ST ON ST.ACADEMIC_YEAR =?AC_YEAR
                                    AND ST.IS_ACTIVE = 1
                                    AND ST.IS_DELETED != 1
                                    INNER JOIN
                                ASSOCIATION_STUDENT_REGISTRATION AS AR ON AR.STUDENT_ID = SC.STUDENT_ID
                                    AND AR.IS_DELETED != 1
                                    AND AR.SETTING_ID = ST.ASSOCIATION_GROUP_SETTING_ID
                                    AND AR.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN
                                ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = AR.COURSE_ID
                                    AND AG.IS_DELETED != 1
                            WHERE
                                SC.ACADEMIC_YEAR =?AC_YEAR
                                    AND SC.IS_DELETED != 1
                            ORDER BY PG.PROGRAMME_MODE , PG.SHIFT , PG.PROGRAMME_GROUP_ID , SP.ROLL_NO;";
                        break;
                    }
                case AssociationSQLCommands.FetchAssociationGroup:
                    {
                        query = @"SELECT 
                                ASSOCICATION_GROUP_ID,
                                ASSOCIATION_GROUP,
                                ASSOCIATION_GROUP_CODE,
                                ASSOCIATION_GROUP_ORDER,
                                QUOTA,
                                SEMESTER_ID
                            FROM
                                ASSOCIATION_GROUPS
                            WHERE
                                IS_ACTIVE = 1 AND IS_DELETED != 1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case AssociationSQLCommands.FetchAssociationGroupByRegisteredCountAndClass:
                    {
                        query = @"SELECT 
                                AQ.COURSE_ID,
                                AQ.QUOTA AS ALLOTED_SEATS,
                                AG.ASSOCIATION_GROUP,
                                AQ.CLASS_ID,
                                ST.ASSOCIATION_GROUP_SETTING_ID AS SETTING_ID,
                                ST.SEMESTER_ID,
                                (SELECT 
                                        COUNT(*)
                                    FROM
                                        ASSOCIATION_STUDENT_REGISTRATION AS AR
                                    WHERE
                                        AR.CLASS_ID = CL.CLASS_ID
                                            AND AR.COURSE_ID = AG.ASSOCICATION_GROUP_ID
                                            AND AR.IS_DELETED != 1
                                            AND AR.SETTING_ID = AQ.SETTING_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR) AS SUMOFQUTOA
                            FROM
                                ASSOCIATION_GROUP_CLASS_QUOTA AS AQ
                                    INNER JOIN
                                ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = AQ.COURSE_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = AQ.CLASS_ID
                                    INNER JOIN
                                ASSOCIATION_GROUP_SETTING AS ST ON ST.ASSOCIATION_GROUP_SETTING_ID = AQ.SETTING_ID
                                    AND ST.IS_ACTIVE = 1
                                    AND ST.ACADEMIC_YEAR = ?AC_YEAR
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SM ON SM.SEMESTER_TYPE_ID = ST.SEMESTER_ID
                                    AND SM.IS_ACTIVE = 1
                            WHERE
                                AQ.CLASS_ID = ?CLASS_ID AND AQ.IS_DELETED != 1
                                    AND AQ.IS_ACTIVE = 1
                            ORDER BY AG.ASSOCIATION_GROUP_ORDER;";
                        break;
                    }

                case AssociationSQLCommands.FetchNotRegisteredStudentsForddl:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                        ' ',
                                        IFNULL(UPPER(SP.LAST_NAME), '')) AS FIRST_NAME,
                                SP.ROLL_NO,
                                CL.CLASS_NAME
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CL.IS_DELETED != 1    
                            WHERE
                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.IS_DELETED != 1
                                    AND SC.CLASS_ID = ?CLASS_ID
                                    AND SC.STUDENT_ID NOT IN (SELECT 
                                        AR.STUDENT_ID
                                    FROM
                                        ASSOCIATION_STUDENT_REGISTRATION AS AR
                                    WHERE
                                        AR.CLASS_ID = SC.CLASS_ID
                                            AND AR.STUDENT_ID = SC.STUDENT_ID
                                            AND AR.ACADEMIC_YEAR = ?AC_YEAR
                                            AND AR.SETTING_ID = ?SETTING_ID
                                            AND AR.IS_DELETED != 1)
                            ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case AssociationSQLCommands.UpdateAssociationStudentRegistration:
                    {
                        query = @"UPDATE `ASSOCIATION_STUDENT_REGISTRATION`
                                SET
                                COURSE_ID = ?COURSE_ID,
                                CLASS_ID=?CLASS_ID,
                                SETTING_ID=?SETTING_ID,
                                STUDENT_ID=?STUDENT_ID,
                                ACADEMIC_YEAR=?AC_YEAR,
                                `TIME_REGISTERED` = CURRENT_TIMESTAMP()
                                WHERE REGISTRATION_ID = ?REGISTRATION_ID;";
                        break;
                    }
                case AssociationSQLCommands.FetchRegistrationById:
                    {
                        query = @"SELECT 
                                    REGISTRATION_ID,
                                    STUDENT_ID,
                                    CLASS_ID,
                                    COURSE_ID
                                    
                                FROM
                                    ASSOCIATION_STUDENT_REGISTRATION
                                WHERE
                                    REGISTRATION_ID = ?REGISTRATION_ID;";
                        break;
                    }
                case AssociationSQLCommands.DeleteRegistration:
                    {
                        query = @"UPDATE ASSOCIATION_STUDENT_REGISTRATION SET IS_DELETED=1 WHERE REGISTRATION_ID=?REGISTRATION_ID;";
                        break;
                    }


                #region Clup Registeration
                case AssociationSQLCommands.UpdateAssociationGroupSetting:
                    {
                        query = @"UPDATE 
                               association_group_setting 
                               SET 
                                SETTING_NAME=?SETTING_NAME,
                                DATE_FROM=?DATE_FROM,
                                DATE_TO=?DATE_TO,
                                SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID,
                                ACADEMIC_YEAR=?AC_YEAR
    
                            WHERE ASSOCIATION_GROUP_SETTING_ID=?ASSOCIATION_GROUP_SETTING_ID;";
                        break;
                    }
                case AssociationSQLCommands.SaveAssociationSetting:
                    {

                        query = @"INSERT INTO association_group_setting
                                                (
                                                SETTING_NAME,
                                                DATE_FROM,
                                                DATE_TO,
                                                SEMESTER_ID,
                                                ACADEMIC_YEAR)
                                                VALUES
                                                (
                                                ?SETTING_NAME,
                                                ?DATE_FROM,
                                                ?DATE_TO,
                                                ?SEMESTER_ID,
                                                ?AC_YEAR);";
                        break;
                    }

                case AssociationSQLCommands.FetchAssociationGroupsByAcyear:
                    {
                        query = @"SELECT 
                                    AG.ASSOCICATION_GROUP_ID AS COURSE_ID,
                                    AG.ASSOCIATION_GROUP,
                                    AG.ASSOCICATION_GROUP_ID
                                FROM
                                    ASSOCIATION_GROUPS AS AG
                                WHERE
                                    AG.ACADEMIC_YEAR =?AC_YEAR
                                        AND AG.IS_DELETED != 1
                                GROUP BY AG.ASSOCICATION_GROUP_ID
                                ORDER BY AG.ASSOCIATION_GROUP_ORDER;";
                        break;
                    }

                case AssociationSQLCommands.AssociationRegistrationList:
                    {
                        query = @"SELECT 
                                SC.STUDENT_ID,
                                CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                        ' ',
                                        IFNULL(UPPER(SP.LAST_NAME), '')) AS FIRST_NAME,
                                ifnull(SP.ROLL_NO,SP.ADMISSION_NO) as ROLL_NO,
                                CL.CLASS_NAME,
                                IF(AR.COURSE_ID,
                                    AG.ASSOCIATION_GROUP,
                                    'NOT REGISTERED') AS ASSOCIATION_GROUP,
                                CP.PROGRAMME_NAME,
                                AR.REGISTRATION_ID,
                                SC.CLASS_ID,
                                AR.COURSE_ID,
                                AR.SETTING_ID
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    AND CL.CLASS_LEVEL = 1
                                    
                                    AND CL.IS_DELETED != 1
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN
                                ASSOCIATION_GROUP_SETTING ST ON ST.ACADEMIC_YEAR =?AC_YEAR                                    
                                    AND ST.IS_DELETED != 1
                                    INNER JOIN
                                ASSOCIATION_STUDENT_REGISTRATION AS AR ON AR.STUDENT_ID = SC.STUDENT_ID
                                    AND AR.IS_DELETED != 1
                                    AND AR.SETTING_ID = ST.ASSOCIATION_GROUP_SETTING_ID
                                    AND AR.ACADEMIC_YEAR =?AC_YEAR
                                    INNER JOIN
                                ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = AR.COURSE_ID
                                    AND AG.IS_DELETED != 1
                            WHERE
                                SC.ACADEMIC_YEAR =?AC_YEAR AND AR.COURSE_ID in (?COURSE_ID)
                                    AND SC.IS_DELETED != 1
                            ORDER BY PG.PROGRAMME_MODE , PG.SHIFT , PG.PROGRAMME_GROUP_ID , SP.ROLL_NO;";
                        break;
                    }

                case AssociationSQLCommands.LoadAllAssociationGroups:
                    {
                        query = @"SELECT 
                                    AG.ASSOCICATION_GROUP_ID AS COURSE_ID,
                                    ST.ASSOCIATION_GROUP_SETTING_ID AS SETTING_ID,
                                    ST.SEMESTER_ID,
                                    AG.ASSOCIATION_GROUP,
                                    AG.ASSOCICATION_GROUP_ID
                                FROM
                                    ASSOCIATION_GROUPS AS AG
                                        INNER JOIN
                                    ASSOCIATION_GROUP_SETTING AS ST ON ST.ACADEMIC_YEAR = AG.ACADEMIC_YEAR
                                        AND ST.IS_ACTIVE = 1
                                        AND ST.ACADEMIC_YEAR =?AC_YEAR
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS SM ON SM.SEMESTER_TYPE_ID = ST.SEMESTER_ID
                                WHERE
                                    AG.IS_ACTIVE = 1
                                        AND AG.ACADEMIC_YEAR =?AC_YEAR
                                        AND AG.IS_DELETED != 1
                                GROUP BY AG.ASSOCICATION_GROUP_ID
                                ORDER BY AG.ASSOCIATION_GROUP_ORDER;";
                        break;
                    }
                case AssociationSQLCommands.ListAssociationRegistrationByStaff:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                            ' ',
                                            IFNULL(UPPER(SP.LAST_NAME), '')) AS FIRST_NAME,
                                    SP.ROLL_NO,
                                    CL.CLASS_NAME,
                                    IF(AR.COURSE_ID,
                                        AG.ASSOCIATION_GROUP,
                                        'NOT REGISTERED') AS ASSOCIATION_GROUP,
                                    CP.PROGRAMME_NAME,
                                    AR.REGISTRATION_ID,
                                    SC.CLASS_ID,
                                    AR.COURSE_ID,
                                    AR.SETTING_ID
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                        AND CL.CLASS_LEVEL = 1
                                        AND CL.CLASS_YEAR = 1
                                        AND CL.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = CL.PROGRAMME_GROUP_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                        INNER JOIN
                                    ASSOCIATION_GROUP_SETTING ST ON ST.ACADEMIC_YEAR =?AC_YEAR
                                        AND ST.IS_ACTIVE = 1
                                        AND ST.IS_DELETED != 1
                                        INNER JOIN
                                    ASSOCIATION_STUDENT_REGISTRATION AS AR ON AR.STUDENT_ID = SC.STUDENT_ID
                                        AND AR.IS_DELETED != 1
                                        AND AR.SETTING_ID = ST.ASSOCIATION_GROUP_SETTING_ID
                                        AND AR.ACADEMIC_YEAR =?AC_YEAR
                                        INNER JOIN
                                    ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = AR.COURSE_ID
                                        AND AG.IS_DELETED != 1
                                WHERE
                                    SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                        AND AG.ASSOCICATION_GROUP_ID IN (SELECT 
                                            COURSE_ID
                                        FROM
                                            association_groups_incharge
                                        WHERE
                                            SETTING_ID =ST.ASSOCIATION_GROUP_SETTING_ID AND INCHARGE_ID =?STAFF_ID
                                                AND IS_DELETED != 1)
                                ORDER BY PG.PROGRAMME_MODE , PG.SHIFT , PG.PROGRAMME_GROUP_ID , SP.ROLL_NO;";
                        break;
                    }
                case AssociationSQLCommands.FetchRegularClasses:
                    {
                        query = @"SELECT 
                                        CLS.CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_CODE, ''),
                                                ' ',
                                                IFNULL(CONCAT('(', SPM.PROGRAMME_MODE, ') - '),
                                                        ''),
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS CLASS_NAME,
                                         CONCAT(IFNULL(CLS.CLASS_NAME, ''),' ',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS CLASS_CODE
                                    FROM
                                        CP_CLASSES AS CLS
                                            LEFT JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                            AND CPG.PROGRAMME_MODE = CLS.PROGRAMME_MODE
                                            AND CPG.SHIFT = CLS.SHIFT
                                            LEFT JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            LEFT JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                            LEFT JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                    WHERE
                                        CLS.IS_DELETED != 1 and CLS.CLASS_TYPE=2
                                    ORDER BY CLS.CLASS_TYPE,CLS.CLASS_LEVEL,CLS.PROGRAMME_GROUP_ID,CLS.CLASS_YEAR;";
                        break;
                    }


                case AssociationSQLCommands.ListAssociationRegistrationByCourseId:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                            ' ',
                                            IFNULL(UPPER(SP.LAST_NAME), '')) AS FIRST_NAME,
                                    SP.ROLL_NO,
                                    IF(AR.COURSE_ID,
                                        AG.ASSOCIATION_GROUP,
                                        'NOT REGISTERED') AS ASSOCIATION_GROUP,
                                    CP.PROGRAMME_NAME,
                                    AR.REGISTRATION_ID,
                                    AR.COURSE_ID,
                                    AR.SETTING_ID
                                FROM
                                    ASSOCIATION_STUDENT_REGISTRATION AS AR
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = AR.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                        INNER JOIN
                                    CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                        INNER JOIN
                                    ASSOCIATION_GROUPS AS AG ON AG.ASSOCICATION_GROUP_ID = AR.COURSE_ID
                                        AND AG.IS_DELETED != 1
                                WHERE
                                    AR.ACADEMIC_YEAR =?AC_YEAR
                                        AND AR.COURSE_ID =?COURSE_ID
                                        AND AR.IS_DELETED != 1
                                ORDER BY PG.PROGRAMME_MODE , PG.SHIFT , PG.PROGRAMME_GROUP_ID , SP.ROLL_NO;";
                        break;
                    }
                    #endregion
            }
            return query;
        }
    }
}
