using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.CertificateCourses
{
    public static class CertificateCourseSQL
    {
        public static string GetCertificateCourseSQL(CertificateCourse sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                #region Certificate Course Root
                case CertificateCourse.ListCertificateCourseBySetting:
                    {
                            query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        COURSE_TITLE,
                                        COURSE_CODE,
                                        COURSE_ORDER,
                                        CR.QUOTA,
                                        SUM(IF(C.REGISTRATION_ID!=0,1,0)) AS USED_QUOTA,
                                        CR.ACADEMIC_YEAR,
                                        CR.IS_ACTIVE,
                                        CR.IS_DELETED
                                    FROM
                                        CERTIFICATE_COURSE_ROOT AS CR
                                            INNER JOIN
                                            CERTIFICATE_COURSE_SETTING AS CCS ON CCS.SETTING_ID=CR.SETTING_ID
                                            AND CCS.IS_ACTIVE=1
                                            AND CCS.IS_DELETED!=1
										
                                            LEFT JOIN 
                                            CERTIFICATE_STUDENT_REGISTRATION AS C ON C.COURSE_ID=CR.COURSE_ROOT_ID
                                            AND C.SETTING_ID=CR.SETTING_ID
                                            AND C.IS_DELETED!=1
										    
                                    WHERE
                                        CR.IS_DELETED != 1
                                        AND CR.SETTING_ID=?SETTING_ID GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }

                case CertificateCourse.FetchCertificateCourseById:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    COURSE_TITLE,
                                    COURSE_CODE,
                                    SETTING_ID,
                                    COURSE_ORDER,
                                    QUOTA,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    CERTIFICATE_COURSE_ROOT 
                                WHERE
                                    IS_DELETED != 1 and COURSE_ROOT_ID=?COURSE_ROOT_ID;";
                        break;
                    }

                case CertificateCourse.SaveCertificateCourses:
                    {
                        query = @"INSERT INTO CERTIFICATE_COURSE_ROOT(
                                    COURSE_TITLE,
                                    COURSE_CODE,
                                    SETTING_ID,
                                    COURSE_ORDER,
                                    QUOTA,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE)
                                  VALUES(
                                    ?COURSE_TITLE,
                                    ?COURSE_CODE,
                                    ?SETTING_ID,
                                    ?COURSE_ORDER,
                                    ?QUOTA,
                                    ?AC_YEAR,
                                    ?IS_ACTIVE);";
                        break;
                    }

                case CertificateCourse.ActivateDeactivateCertificateCourses:
                    {
                        query = @"UPDATE CERTIFICATE_COURSE_ROOT SET IS_ACTIVE=?IS_ACTIVE WHERE COURSE_ROOT_ID=?COURSE_ROOT_ID AND IS_DELETED!=1";
                        break;
                    }
                case CertificateCourse.UpdateCertificateCourses:
                    {
                        query = @"UPDATE CERTIFICATE_COURSE_ROOT
                                    SET
                                    COURSE_TITLE = ?COURSE_TITLE,
                                    COURSE_CODE = ?COURSE_CODE,
                                    SETTING_ID = ?SETTING_ID,
                                    COURSE_ORDER = ?COURSE_ORDER,
                                    QUOTA = ?QUOTA,
                                    IS_ACTIVE = ?IS_ACTIVE
                                    WHERE COURSE_ROOT_ID = ?COURSE_ROOT_ID";
                        break;
                    }

                case CertificateCourse.DeleteCertificateCourses:
                    {
                        query = @"UPDATE CERTIFICATE_COURSE_ROOT
                                    SET
                                    IS_DELETED = 1
                                    WHERE COURSE_ROOT_ID = ?COURSE_ROOT_ID;";
                        break;
                    }
                case CertificateCourse.IsCertificateCourseExist:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID
                                    FROM
                                        CERTIFICATE_COURSE_ROOT
                                    WHERE
                                        COURSE_CODE = ?COURSE_CODE
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case CertificateCourse.FetchCourseDDL:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            ' - ( ',
                                            IFNULL(COURSE_CODE, ''),
                                            ' )') AS COURSE_TITLE
                                FROM
                                    CERTIFICATE_COURSE_ROOT
                                 
                                WHERE
                                    IS_DELETED != 1
                                    AND IS_ACTIVE=1;";
                        break;
                    }
                #endregion

                #region Certificate Class Course
                case CertificateCourse.ListCertificateClassCourses:
                    {
                        query = @"SELECT 
                                    CLASS_COURSE_ID,
                                    CC.CLASS_ID,
                                    C.CLASS_NAME,
                                    CC.COURSE_ID,
                                    CR.COURSE_TITLE,
                                    CC.ACADEMIC_YEAR,
                                    CC.QUOTA,
                                    CC.IS_ACTIVE
                                FROM
                                    CERTIFICATE_CLASS_COURSE_QUOTA AS CC
                                        INNER JOIN
                                    CERTIFICATE_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID AND CR.IS_DELETED!=1
                                    AND CR.IS_ACTIVE=1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = CC.CLASS_ID AND C.IS_DELETED!=1
                                    
                                WHERE
                                    CC.IS_DELETED != 1 AND CC.ACADEMIC_YEAR =?AC_YEAR AND CC.SETTING_ID=?SETTING_ID;";
                        break;
                    }

                case CertificateCourse.FetchCertificateCourseBySettingId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        SUM(IF(CCR.QUOTA!=0,CCR.QUOTA,0))-SUM(IF(CCC.QUOTA!=0,CCC.QUOTA,0)) AS BALANCE_QUOTA,
                                        CONCAT(IFNULL(CCR.COURSE_TITLE,''),' - ',IFNULL(CCR.COURSE_CODE,'')) AS COURSE_TITLE
                                    FROM
                                        CERTIFICATE_COURSE_ROOT AS CCR
                                        
                                        INNER JOIN
                                        CERTIFICATE_COURSE_SETTING AS CCS ON CCS.SETTING_ID=CCR.SETTING_ID
                                        AND CCS.IS_ACTIVE=1
                                        AND CCS.IS_DELETED!=1
                                        
                                        LEFT JOIN
                                        CERTIFICATE_CLASS_COURSE_QUOTA AS CCC ON CCC.COURSE_ID=CCR.COURSE_ROOT_ID
                                        AND CCC.IS_DELETED!=1
                                        AND CCC.SETTING_ID=CCR.SETTING_ID
                                        
                                    WHERE
                                        CCR.IS_DELETED != 1
                                        AND CCR.IS_ACTIVE=1
                                        AND CCR.SETTING_ID=?SETTING_ID GROUP BY CCR.COURSE_ROOT_ID;";
                        break;
                    }

                case CertificateCourse.ActivateCertificateClassCourse:
                    {
                        query = @"UPDATE CERTIFICATE_CLASS_COURSE_QUOTA SET IS_ACTIVE=?IS_ACTIVE WHERE CLASS_COURSE_ID=?CLASS_COURSE_ID;";
                        break;
                    }

                case CertificateCourse.SaveCertificateClassCourses:
                    {
                        query = @"INSERT INTO CERTIFICATE_CLASS_COURSE_QUOTA(
                                    SETTING_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    QUOTA,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE)
                                    VALUES
                                    (
                                    ?SETTING_ID,
                                    ?CLASS_ID,
                                    ?COURSE_ID,
                                    ?QUOTA,
                                    ?AC_YEAR,
                                    ?IS_ACTIVE);";
                        break;
                    }
                case CertificateCourse.UpdateCertificateClassCoursess:
                    {
                        query = @"UPDATE CERTIFICATE_CLASS_COURSE_QUOTA
                                    SET                                   
                                    SETTING_ID=?SETTING_ID,
                                    CLASS_ID=?CLASS_ID,
                                    COURSE_ID=?COURSE_ID,
                                    QUOTA=?QUOTA,
                                    ACADEMIC_YEAR=?AC_YEAR
                                    WHERE CLASS_COURSE_ID = ?CLASS_COURSE_ID;";
                        break;
                    }
                case CertificateCourse.UpdateCertificateClassCourses:
                    {
                        query = @"UPDATE CERTIFICATE_CLASS_COURSE
                                    SET                                   
                                    IS_DELETED=0
                                    WHERE CLASS_COURSE_ID = ?CLASS_COURSE_ID;";
                        break;
                    }

                case CertificateCourse.DeleteCertificateClassCourses:
                    {
                        query = @"UPDATE CERTIFICATE_CLASS_COURSE_QUOTA
                                    SET
                                    IS_DELETED=1
                                    WHERE CLASS_COURSE_ID = ?CLASS_COURSE_ID;";
                        break;
                    }
                case CertificateCourse.IsCertificateClassCourseExist:
                    {
                        query = @"SELECT 
                                        CLASS_COURSE_ID
                                    FROM
                                        CERTIFICATE_CLASS_COURSE_QUOTA
                                    WHERE
                                            SETTING_ID =?SETTING_ID
                                            AND COURSE_ID =?COURSE_ID
                                            AND CLASS_ID=?CLASS_ID
                                            AND ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case CertificateCourse.FetchCertificateClassCourseById:
                    {
                        query = @"SELECT 
                                    CLASS_COURSE_ID,
                                    CLASS_ID,
                                    SETTING_ID,
                                    COURSE_ID,
                                    QUOTA,
                                    IS_ACTIVE
                                FROM
                                    CERTIFICATE_CLASS_COURSE_QUOTA
                                WHERE
                                    CLASS_COURSE_ID = ?CLASS_COURSE_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }

                #endregion

                #region Certificate Student Registration
                case CertificateCourse.ListCertificateRegisteredStudent:
                    {
                        query = @"
                                SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS 'FIRST_NAME',
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    SP.ADMISSION_NO,
                                    C.CLASS_NAME,
                                    CR.COURSE_TITLE,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS 'DATE_OF_BIRTH',
                                    SP.STU_MOBILENO,
                                    SR.REGISTRATION_ID
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                        AND C.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                       AND SP.IS_DELETED != 1
                                       AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    CERTIFICATE_STUDENT_REGISTRATION AS SR ON SR.STUDENT_ID = SP.STUDENT_ID
                                         AND SR.CLASS_ID = C.CLASS_ID
                                        AND SR.IS_DELETED != 1
                                        AND SR.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND SR.SETTING_ID =?SETTING_ID
                                        INNER JOIN
                                    CERTIFICATE_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SR.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                WHERE
                                    SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1
                                ORDER BY C.CLASS_ID;";
                        break;
                    }
                case CertificateCourse.SaveStudentRegistration:
                    {
                        query = @"INSERT INTO CERTIFICATE_STUDENT_REGISTRATION
                                    (
                                    STUDENT_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    TIME_REGISTERED,
                                    ACADEMIC_YEAR,SETTING_ID)
                                    VALUES
                                    (
                                    ?STUDENT_ID,
                                    ?CLASS_ID,
                                    ?COURSE_ID,
                                    current_timestamp(),
                                    ?AC_YEAR,?SETTING_ID
                                    );";
                        break;
                    }
                case CertificateCourse.UpdateRegistration:
                    {
                        query = @"UPDATE CERTIFICATE_STUDENT_REGISTRATION
                                    SET
                                    STUDENT_ID = ?STUDENT_ID,
                                    CLASS_ID = ?CLASS_ID,
                                    COURSE_ID = ?COURSE_ID,
                                    TIME_REGISTERED = current_timestamp(),
                                    SETTING_ID = ?SETTING_ID
                                    WHERE REGISTRATION_ID = ?REGISTRATION_ID;";
                        break;
                    }
                case CertificateCourse.LoadCertificateCourses:
                    {
                        query = @"SELECT 
                            CCR.COURSE_ROOT_ID,
                            CCR.COURSE_TITLE,
                            CCR.COURSE_CODE,
                            CCR.SETTING_ID,
                            C.QUOTA - SUM(IF(CSR.REGISTRATION_ID != 0, 1, 0)) AS BALANCE_QUOTA
                        FROM
                            CERTIFICATE_CLASS_COURSE_QUOTA AS C
                                INNER JOIN
                            CP_CLASSES AS CC ON CC.CLASS_ID = C.CLASS_ID
                                AND CC.IS_DELETED != 1
                                INNER JOIN
                            CERTIFICATE_COURSE_SETTING AS CCS ON CCS.SETTING_ID = C.SETTING_ID
                                AND CCS.IS_ACTIVE = 1
                                AND CCS.IS_DELETED != 1
                                INNER JOIN
                            CERTIFICATE_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = C.COURSE_ID
                                AND CCR.SETTING_ID = C.SETTING_ID
                                AND CCR.IS_DELETED != 1
                                AND CCR.IS_ACTIVE = 1
                                LEFT JOIN
                            CERTIFICATE_STUDENT_REGISTRATION AS CSR ON CSR.CLASS_ID = CC.CLASS_ID
                                AND CSR.IS_DELETED != 1
                                AND CSR.SETTING_ID = C.SETTING_ID
                        WHERE
                            C.CLASS_ID = ?CLASS_ID
                                AND C.ACADEMIC_YEAR = ?AC_YEAR
                                AND C.IS_ACTIVE = 1
                                AND C.SETTING_ID = ?SETTING_ID
                                AND C.IS_DELETED!=1
                        GROUP BY C.COURSE_ID;";          
                        break;
                    }
                case CertificateCourse.FetchRegistrationById:
                    {
                        query = @"SELECT 
                                    REGISTRATION_ID,
                                    STUDENT_ID,
                                    CLASS_ID,
                                    COURSE_ID                                    
                                FROM
                                    CERTIFICATE_STUDENT_REGISTRATION
                                WHERE
                                    REGISTRATION_ID = ?REGISTRATION_ID;";
                        break;
                    }
                case CertificateCourse.IsRegistrationExist:
                    {
                        query = @"SELECT 
                                        REGISTRATION_ID,
                                        STUDENT_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                        TIME_REGISTERED,
                                        ACADEMIC_YEAR
                                    FROM
                                        CERTIFICATE_STUDENT_REGISTRATION
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID 
                                        AND SETTING_ID=?SETTING_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR
                                        AND IS_DELETED!=1;";
                        break;
                    }
                case CertificateCourse.IsRegistrationExistinSameCourse:
                    {
                        query = @"SELECT 
                                        CS.REGISTRATION_ID,
                                        CS.STUDENT_ID,
                                        CS.CLASS_ID,
                                        CS.COURSE_ID,
                                        CS.TIME_REGISTERED,
                                        CS.ACADEMIC_YEAR
                                    FROM
                                        CERTIFICATE_STUDENT_REGISTRATION AS CS
                                        
                                        INNER JOIN
                                        certificate_course_root AS CCR ON CCR.COURSE_ROOT_ID=CS.COURSE_ID
                                        AND CCR.IS_DELETED!=1
                                        
                                    WHERE
                                        CS.STUDENT_ID =?STUDENT_ID
                                        AND CCR.COURSE_CODE=?COURSE_CODE
                                        AND CS.IS_DELETED!=1;";
                        break;
                    }
                case CertificateCourse.DeleteRegistration:
                    {
                        query = @"UPDATE CERTIFICATE_STUDENT_REGISTRATION SET IS_DELETED=1 WHERE REGISTRATION_ID=?REGISTRATION_ID;";
                        break;
                    }
                case CertificateCourse.FetchRegisteredCourse:
                    {
                        query = @"SELECT 
                                        CR.COURSE_CODE,
                                        CR.COURSE_TITLE
                                    FROM
                                        CERTIFICATE_STUDENT_REGISTRATION AS SR
                                            INNER JOIN
                                        CERTIFICATE_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SR.COURSE_ID
                                    WHERE
                                        SR.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SR.STUDENT_ID = ?STUDENT_ID
                                            AND SR.SETTING_ID = ?SETTING_ID
                                            AND SR.IS_DELETED != 1;";
                        break;
                    }
                #endregion
                case CertificateCourse.IsCertificateCourseSettingExist:
                    {
                        query = @"SELECT 
                                        SS.SETTINGS_ID
                                    FROM
                                        CERTIFICATE_COURSE_SETTINGS AS SS
                                            LEFT JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SS.CLASS_ID
                                            LEFT JOIN
                                        SUP_SEMESTER AS SEM ON SEM.SUP_SEMESTER_ID = SS.SEMESTER WHERE SS.SEMESTER=?SEMESTER AND SS.CLASS_ID=?CLASS_ID AND SS.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case CertificateCourse.FetchCertificateCourseSettingList:
                    {
                        query = @"SELECT 
                                    CCS.SETTING_ID,
                                    CCS.SETTING_NAME,
                                    DATE_FORMAT(CCS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(CCS.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    SS.SEMESTER_TYPE,
                                    CCS.ACADEMIC_YEAR,
                                    CCS.IS_ACTIVE
                                FROM
                                    CERTIFICATE_COURSE_SETTING AS CCS
                                        LEFT JOIN
                                    SUP_SEMESTER_TYPE AS SS ON SS.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                      AND SS.IS_DELETED!=1
                                WHERE
                                    CCS.ACADEMIC_YEAR =?AC_YEAR
                                        AND CCS.IS_DELETED != 1;";
                        break;
                    }
                case CertificateCourse.SaveCertificateCourseSetting:
                    {

                        query = @"INSERT INTO CERTIFICATE_COURSE_SETTING
                                                (
                                                SETTING_NAME,
                                                DATE_FROM,
                                                DATE_TO,
                                                SEMESTER_TYPE_ID,
                                                ACADEMIC_YEAR)
                                                VALUES
                                                (
                                                ?SETTING_NAME,
                                                ?DATE_FROM,
                                                ?DATE_TO,
                                                ?SEMESTER_TYPE_ID,
                                                ?AC_YEAR);";
                        break;
                    }
                case CertificateCourse.UpdateCertificateCourseSetting:
                    {
                        query = @"UPDATE 
                               CERTIFICATE_COURSE_SETTING 
                               SET 
                                SETTING_NAME=?SETTING_NAME,
                                DATE_FROM=?DATE_FROM,
                                DATE_TO=?DATE_TO,
                                SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID,
                                ACADEMIC_YEAR=?AC_YEAR
    
                            WHERE SETTING_ID=?SETTING_ID;";
                        break;
                    }
                case CertificateCourse.FetchCertificateCourseSettingById:
                    {
                        query = @"SELECT 
                                    CCS.SETTING_ID,
                                    CCS.SETTING_NAME,
                                    DATE_FORMAT(CCS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(CCS.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    CCS.SEMESTER_TYPE_ID,
                                    CCS.IS_ACTIVE
                                FROM
                                    CERTIFICATE_COURSE_SETTING AS CCS
                                        
                                WHERE
                                     CCS.SETTING_ID=?SETTING_ID
                                        AND CCS.IS_DELETED != 1;";


                        break;
                    }
                case CertificateCourse.UpdateCertificateCourseSettings:
                    {
                        query = @"UPDATE CERTIFICATE_COURSE_SETTINGS
                                                    SET
                                                    SETTINGS_NAME=?SETTINGS_NAME,
                                                    CLASS_ID=?CLASS_ID,
                                                    DATE_FROM=?DATE_FROM,
                                                    DATE_TO=?DATE_TO,
                                                    ACADEMIC_YEAR=?AC_YEAR,
                                                    SEMESTER=?SEMESTER
                                                    WHERE SETTINGS_ID=?SETTINGS_ID;";


                        break;
                    }
                case CertificateCourse.DeleteCertificateCourseSetting:
                    {

                        query = @"UPDATE CERTIFICATE_COURSE_SETTING
                                                    SET
                                                    IS_DELETED=1
                                                    WHERE SETTING_ID=?SETTING_ID;";
                        break;
                    }
                case CertificateCourse.CertificateCourseSettingRegistrationClose:
                    {

                        query = @"UPDATE CERTIFICATE_COURSE_SETTING
                                                    SET
                                                    IS_ACTIVE=?IS_ACTIVE
                                                    WHERE SETTING_ID=?SETTING_ID;";
                        break;
                    }
                case CertificateCourse.ValidateStudent:
                    {
                        query = @"SELECT 
                                    COUNT(*) > 0 AS 'REGISTRATION_ID'
                                FROM
                                    CERTIFICATE_STUDENT_REGISTRATION
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID 
                                    AND SETTING_ID = ?SETTING_ID 
                                    AND IS_DELETED!=1
                                    AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case CertificateCourse.FetchCertificateCourseSettingddl:
                    {
                        query = @"SELECT 
                                    SETTINGS_ID, 
                                    SETTINGS_NAME, 
                                    CLASS_ID
                                FROM
                                    CERTIFICATE_COURSE_SETTINGS
                                WHERE
                                    IS_DELETED != 1 AND IS_ALLOWED != 0";
                        break;
                    }
                case CertificateCourse.FetchCertificateCourseBySetting:
                    {
                        query = @"SELECT 
                                    CCR.COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE,
                                    CL.CLASS_NAME,
                                    IF(CT.SETTINGS_ID IS NULL,
                                        '',
                                        'SELECTED') AS 'SELECTED'
                                FROM
                                    CERTIFICATE_COURSE_SETTINGS AS CT
                                        INNER JOIN
                                    CERTIFICATE_COURSE_ROOT AS CCR ON CCR.IS_DELETED=CT.IS_DELETED
                                        LEFT JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = CT.CLASS_ID
                                        LEFT JOIN
                                    SUP_SHIFT AS SH ON SH.SHIFT_ID = CL.SHIFT
                                WHERE
                                    CT.SETTINGS_ID = ?SETTINGS_ID
                                        AND CT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CT.IS_DELETED != 1;";
                                     // query = @"SELECT 
                                     //         CR.COURSE_ROOT_ID,
                                     //         CONCAT(IFNULL(COURSE_TITLE, ''),
                                     //                 '(',
                                     //                 IFNULL(COURSE_CODE, ''),
                                     //                 ')') AS COURSE_TITLE,
                                     //         CL.CLASS_NAME,
                                     //         IF(CT.SETTINGS_ID IS NULL,
                                     //             '',
                                     //             'SELECTED') AS 'SELECTED'
                                     //     FROM
                                     //         CERTIFICATE_COURSE_ROOT AS CR
                                     //             LEFT JOIN
                                     //         CERTIFICATE_CLASS_COURSE AS CC ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                     //             AND CC.ACADEMIC_YEAR = ?AC_YEAR
					                 //               AND CC.IS_DELETED!=1 
                                     //             AND CC.SETTINGS_ID = ?SETTINGS_ID                                   
                                     //             LEFT JOIN
                                     //         CERTIFICATE_COURSE_SETTINGS AS CT ON CT.CLASS_ID = CC.CLASS_ID
                                     //             AND CT.SETTINGS_ID = ?SETTINGS_ID
                                     //             AND CT.ACADEMIC_YEAR = ?AC_YEAR
                                     //             AND CT.IS_DELETED!=1 
                                     //             LEFT JOIN
                                     //         CP_CLASSES AS CL ON CL.CLASS_ID = CT.CLASS_ID
                                     //             LEFT JOIN
                                     //         SUP_SHIFT AS SH ON SH.SHIFT_ID = CL.SHIFT ORDER BY COURSE_ROOT_ID;";
                        break;
                    }
                case CertificateCourse.UpdateIsDeletedBySettingId:
                    {
                        query = @"UPDATE CERTIFICATE_CLASS_COURSE SET IS_DELETED=1 WHERE SETTINGS_ID=?SETTINGS_ID AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case CertificateCourse.CheckAvailablwQuota:
                    {
                        query = @"SELECT 
                                    QUOTA >(SELECT 
                                            COUNT(STUDENT_ID)
                                        FROM
                                            CERTIFICATE_STUDENT_REGISTRATION
                                        WHERE
                                            COURSE_ID = ?COURSE_ID AND SETTING_ID = ?SETTING_ID
                                                AND ACADEMIC_YEAR = ?AC_YEAR) AS QUOTA
                                FROM
                                    CERTIFICATE_COURSE_REGISTRATION
                                WHERE
                                    COURSE_ID = ?COURSE_ID AND SETTING_ID = ?SETTING_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                #region Certificate root setting
                case CertificateCourse.FetchCertificateCourseSetting:
                    {
                        query = @"SELECT 
                                        SETTING_ID, SETTING_NAME
                                    FROM
                                        CERTIFICATE_COURSE_SETTING
                                    WHERE
                                        ACADEMIC_YEAR =?AC_YEAR AND IS_DELETED != 1 AND IS_ACTIVE=1;";
                        break;
                    }
                #endregion
                #region clup Registeration
                case CertificateCourse.FetchAssociationSetting:
                    {
                        query = @"SELECT 
                                       ASSOCIATION_GROUP_SETTING_ID  SETTING_ID, SETTING_NAME
                                    FROM
                                        association_group_setting
                                    WHERE
                                        ACADEMIC_YEAR =?AC_YEAR AND IS_DELETED != 1 AND IS_ACTIVE=1;";
                        break;
                    }


                case CertificateCourse.ListAssociationBySetting:
                    {
                        query = @"SELECT 
                                        ASSOCICATION_GROUP_ID,
                                        ASSOCIATION_GROUP,
                                        ASSOCIATION_GROUP_CODE,
                                        ASSOCIATION_GROUP_ORDER,
                                        CR.QUOTA,
                                        SUM(IF(C.REGISTRATION_ID!=0,1,0)) AS USED_QUOTA,
                                        CR.ACADEMIC_YEAR,
                                        CR.IS_ACTIVE,
                                        CR.IS_DELETED
                                    FROM
                                        ASSOCIATION_GROUPS AS CR
                                            INNER JOIN
                                            ASSOCIATION_GROUP_SETTING AS CCS ON CCS.ASSOCIATION_GROUP_SETTING_ID=CR.SETTING_ID
                                            AND CCS.IS_ACTIVE=1
                                            AND CCS.IS_DELETED!=1
										
                                            LEFT JOIN 
                                            ASSOCIATION_STUDENT_REGISTRATION AS C ON C.COURSE_ID=CR.ASSOCICATION_GROUP_ID
                                            AND C.SETTING_ID=CR.SETTING_ID
                                            AND C.IS_DELETED!=1
										    
                                    WHERE
                                        CR.IS_DELETED != 1
                                        AND CR.SETTING_ID=?SETTING_ID GROUP BY CR.ASSOCICATION_GROUP_ID;
";
                        break;
                    }

                case CertificateCourse.FetchAssociationGroupById:
                    {
                        query = @"SELECT 
                                    ASSOCICATION_GROUP_ID,
                                    ASSOCIATION_GROUP,
                                    ASSOCIATION_GROUP_CODE,
                                    SETTING_ID,
                                    ASSOCIATION_GROUP_ORDER,
                                    QUOTA,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    ASSOCIATION_GROUPS 
                                WHERE
                                    IS_DELETED != 1 and ASSOCICATION_GROUP_ID=?ASSOCICATION_GROUP_ID;";
                        break;
                    }

                case CertificateCourse.UpdateAssociationGroups:
                    {
                        query = @"UPDATE association_groups
                                    SET
                                    ASSOCIATION_GROUP = ?ASSOCIATION_GROUP,
                                    ASSOCIATION_GROUP_CODE = ?ASSOCIATION_GROUP_CODE,
                                    SETTING_ID = ?SETTING_ID,
                                    ASSOCIATION_GROUP_ORDER = ?ASSOCIATION_GROUP_ORDER,
                                    QUOTA = ?QUOTA,
                                    IS_ACTIVE = ?IS_ACTIVE
                                    WHERE ASSOCICATION_GROUP_ID = ?ASSOCICATION_GROUP_ID";
                        break;
                    }
                case CertificateCourse.IsAssociationGroupsExist:
                    {
                        query = @"SELECT 
                                        ASSOCIATION_GROUP
                                    FROM
                                        association_groups
                                    WHERE
                                        ASSOCIATION_GROUP_CODE = ?ASSOCIATION_GROUP_CODE
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case CertificateCourse.SaveAssociationGroups:
                    {
                        query = @"INSERT INTO association_groups(
                                    ASSOCIATION_GROUP,
                                    ASSOCIATION_GROUP_CODE,
                                    SETTING_ID,
                                    ASSOCIATION_GROUP_ORDER,
                                    QUOTA,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE)
                                  VALUES(
                                    ?ASSOCIATION_GROUP,
                                    ?ASSOCIATION_GROUP_CODE,
                                    ?SETTING_ID,
                                    ?ASSOCIATION_GROUP_ORDER,
                                    ?QUOTA,
                                    ?AC_YEAR,
                                    ?IS_ACTIVE);";
                        break;
                    }
                case CertificateCourse.DeleteAssociationGroups:
                    {
                        query = @"UPDATE association_groups
                                    SET
                                    IS_DELETED = 1
                                    WHERE ASSOCICATION_GROUP_ID = ?ASSOCICATION_GROUP_ID;";
                        break;
                    }

                case CertificateCourse.FetchAssociationSettingById:
                    {
                        query = @"SELECT 
                                    CCS.ASSOCIATION_GROUP_SETTING_ID AS SETTING_ID,
                                    CCS.SETTING_NAME,
                                    DATE_FORMAT(CCS.DATE_FROM, '%d/%m/%Y') AS DATE_FROM,
                                    DATE_FORMAT(CCS.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    CCS.SEMESTER_ID,
                                    CCS.IS_ACTIVE
                                FROM
                                    association_group_setting AS CCS
                                        
                                WHERE
                                     CCS.ASSOCIATION_GROUP_SETTING_ID=?SETTING_ID
                                        AND CCS.IS_DELETED != 1;";


                        break;
                    }
                case CertificateCourse.AssociationSettingRegistrationClose:
                    {

                        query = @"UPDATE association_group_setting
                                                    SET
                                                    IS_ACTIVE=?IS_ACTIVE
                                                    WHERE ASSOCIATION_GROUP_SETTING_ID=?SETTING_ID;";
                        break;
                    }

                case CertificateCourse.ListAssociationClassCourses:
                    {
                        query = @"SELECT 
                                    ASSOCIATION_GROUP_CLASS_QUOTA_ID,
                                    CC.CLASS_ID,
                                    C.CLASS_NAME,
                                    CC.COURSE_ID,
                                    ASSOCIATION_GROUP ,
                                    CC.ACADEMIC_YEAR,
                                    CC.QUOTA,
                                    CC.IS_ACTIVE
                                FROM
                                    association_group_class_quota AS CC
                                        INNER JOIN
                                    association_groups AS CR ON CR.ASSOCICATION_GROUP_ID = CC.COURSE_ID AND CR.IS_DELETED!=1
                                    AND CR.IS_ACTIVE=1
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = CC.CLASS_ID AND C.IS_DELETED!=1
                                WHERE
                                    CC.IS_DELETED != 1 AND CC.ACADEMIC_YEAR =?AC_YEAR AND CC.SETTING_ID=?SETTING_ID;";
                        break;
                    }
                case CertificateCourse.FetchAssociationCourseBySettingId:
                    {
                        query = @"SELECT 
                                      CCR.ASSOCICATION_GROUP_ID AS COURSE_ROOT_ID,
                                      (CCR.QUOTA - COUNT(CCC.REGISTRATION_ID)) AS BALANCE_QUOTA,
                                      CONCAT(IFNULL(CCR.ASSOCIATION_GROUP, ''), ' - ', IFNULL(CCR.ASSOCIATION_GROUP_CODE, '')) AS COURSE_TITLE
                                  FROM
                                      association_groups AS CCR
                                      INNER JOIN association_group_setting AS CCS 
                                          ON CCS.ASSOCIATION_GROUP_SETTING_ID = CCR.SETTING_ID
                                          AND CCS.IS_ACTIVE = 1
                                          AND CCS.IS_DELETED != 1
                                      LEFT JOIN association_student_registration AS CCC 
                                          ON CCC.COURSE_ID = CCR.ASSOCICATION_GROUP_ID
                                          AND CCC.IS_DELETED != 1
                                          AND CCC.SETTING_ID = CCR.SETTING_ID
                                  WHERE
                                      CCR.IS_DELETED != 1
                                      AND CCR.IS_ACTIVE = 1
                                      AND CCR.SETTING_ID = ?SETTING_ID
                                  GROUP BY 
                                      CCR.ASSOCICATION_GROUP_ID;";
                        break;
                    }
                case CertificateCourse.FetchAssociationClassCourseById:
                    {
                        query = @"SELECT 
                                    ASSOCIATION_GROUP_CLASS_QUOTA_ID,
                                    CLASS_ID,
                                    SETTING_ID,
                                    COURSE_ID,
                                    QUOTA,
                                    IS_ACTIVE
                                FROM
                                    association_group_class_quota
                                WHERE
                                    ASSOCIATION_GROUP_CLASS_QUOTA_ID = ?ASSOCIATION_GROUP_CLASS_QUOTA_ID
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case CertificateCourse.ActivateAssociationClassCourse:
                    {
                        query = @"UPDATE association_group_class_quota SET IS_ACTIVE=?IS_ACTIVE WHERE ASSOCIATION_GROUP_CLASS_QUOTA_ID=?ASSOCIATION_GROUP_CLASS_QUOTA_ID;";
                        break;
                    }
                case CertificateCourse.UpdateAssociationClassCoursess:
                    {
                        query = @"UPDATE association_group_class_quota
                                    SET                                   
                                    SETTING_ID=?SETTING_ID,
                                    CLASS_ID=?CLASS_ID,
                                    COURSE_ID=?COURSE_ID,
                                    QUOTA=?QUOTA,
                                    ACADEMIC_YEAR=?AC_YEAR
                                    WHERE ASSOCIATION_GROUP_CLASS_QUOTA_ID = ?ASSOCIATION_GROUP_CLASS_QUOTA_ID;";
                        break;
                    }

                case CertificateCourse.IsAssociationClassCourseExist:
                    {
                        query = @"SELECT 
                                        ASSOCIATION_GROUP_CLASS_QUOTA_ID
                                    FROM
                                        association_group_class_quota
                                    WHERE
                                            SETTING_ID =?SETTING_ID
                                            AND COURSE_ID =?COURSE_ID
                                            AND IS_DELETED !=1
                                            AND CLASS_ID=?CLASS_ID
                                            AND ACADEMIC_YEAR =?AC_YEAR;";
                        break;
                    }
                case CertificateCourse.SaveAssociationClassCourses:
                    {
                        query = @"INSERT INTO association_group_class_quota(
                                    SETTING_ID,
                                    CLASS_ID,
                                    COURSE_ID,
                                    QUOTA,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE)
                                    VALUES
                                    (
                                    ?SETTING_ID,
                                    ?CLASS_ID,
                                    ?COURSE_ID,
                                    ?QUOTA,
                                    ?AC_YEAR,
                                    ?IS_ACTIVE);";
                        break;
                    }
                case CertificateCourse.DeleteAssociationClassCourses:
                    {
                        query = @"UPDATE association_group_class_quota
                                    SET
                                    IS_DELETED=1
                                    WHERE ASSOCIATION_GROUP_CLASS_QUOTA_ID = ?ASSOCIATION_GROUP_CLASS_QUOTA_ID;";
                        break;
                    }
                #endregion

                default:
                    break;
            }
            return query;
        }
    }
}
