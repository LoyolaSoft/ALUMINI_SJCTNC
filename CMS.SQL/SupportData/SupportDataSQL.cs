using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.SupportData
{
    public static class SupportDataSQL
    {
        public static string GetSupportDataSQL(SupportDataSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case SupportDataSQLCommands.FetchApplicationType:
                    {
                        query = @"SELECT 
                                APPLICATION_TYPE_ID, APPLICATION_TYPE, PREFIX, SUFFIX
                            FROM
                                SUP_APPLICATION_TYPE
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTransactionAmountForBarChart:
                    {
                        //query = @"select sum(t.collected) as y , date_format(date_trunc('week',payment_date),'%M-%d') as label from fee_transaction as t group by label order by PAYMENT_DATE";
                        query = @"select sum(t.collected) as y , date_format(payment_date,'%M') as label from fee_transaction as t group by label order by PAYMENT_DATE";
                        break;
                    }
                case SupportDataSQLCommands.GetActiveSemesterType:
                    {
                        query = @"SELECT 
                                    SEMESTER_TYPE_ID, SEMESTER_TYPE
                                FROM
                                    SUP_SEMESTER_TYPE
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupMonths:
                    {
                        query = @"SELECT MONTH_ID,MONTH_NAME FROM SUP_MONTHS where IS_DELETED !=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchFrequencyIdForExam:
                    {
                        query = @"SELECT 
                                    FREQUENCY_ID,
                                    ACADEMIC_YEAR,
                                    FREQUENCY_NAME,
                                    DATE_FROM,
                                    DATE_TO,
                                    STATUS,                               
                                    IS_USED,
                                    date_format(LAST_DATE_OF_PAYMENT,'%Y/%m/%d') as LAST_DATE_OF_PAYMENT,
                                    IS_DOWNLOADED,
                                    IS_UPDATED,
                                    DOWNLOAD_TIME,
                                    TYPE
                                FROM
                                    SUP_FEE_FREQUENCY AS ff
                                      INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = FF.SEMESTER_TYPE
                                            AND ST.IS_ACTIVE = 1
                                WHERE
                                    ff.IS_DELETED != 1 AND ff.TYPE = 1 and ff.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }

                case SupportDataSQLCommands.FecthSubjectType:
                    {
                        query = @"SELECT 
                                    SUBJECT_TYPE_ID, SUBJECT_TYPE
                                FROM
                                    SUP_SUBJECT_TYPE AS ST
                                WHERE
                                    ST.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.IsSupHeadExist:
                    {
                        query = @"SELECT 
                                    HEAD_ID, 
                                    HEAD, 
                                    FEE_CATEGORY
                                FROM
                                    SUP_HEAD WHERE HEAD=?HEAD AND FEE_CATEGORY=?FEE_CATEGORY AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupHead:
                    {
                        query = @"UPDATE SUP_HEAD SET IS_DELETED=1 WHERE HEAD_ID=?HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSupHead:
                    {
                        query = @"INSERT INTO SUP_HEAD (HEAD,FEE_CATEGORY)VALUES(?HEAD,?FEE_CATEGORY);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupHead:
                    {
                        query = @"UPDATE SUP_HEAD SET HEAD=?HEAD,FEE_CATEGORY=?FEE_CATEGORY WHERE HEAD_ID=?HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupHeadForEdit:
                    {
                        query = @"SELECT 
                                    HEAD_ID, 
                                    HEAD, 
                                    FEE_CATEGORY
                                FROM
                                    SUP_HEAD WHERE HEAD_ID=?HEAD_ID";

                        break;
                    }
                case SupportDataSQLCommands.FetchddlCategory:
                    {
                        query = @"SELECT 
                                    FEE_CATEGORY_ID,
                                    FEE_CATEGORY
                                FROM
                                    SUP_FEE_CATEGORY ";

                        break;
                    }
                case SupportDataSQLCommands.FetchSupExtraCuriculam:
                    {
                        query = @"SELECT 
                                        CURUCULAR_ID, CURUCULAR_NAME
                                    FROM
                                        sup_extra_curicular_activities
                                    WHERE
                                        IS_DELETED != 1; ";

                        break;
                    }
                case SupportDataSQLCommands.FetchSupHeadList:
                    {
                        query = @"SELECT 
                                    SH.HEAD_ID, 
                                    SH.HEAD, 
                                    SC.FEE_CATEGORY
                                FROM
                                    SUP_HEAD AS SH 
                                    INNER JOIN
	                                SUP_FEE_CATEGORY AS SC ON SC.FEE_CATEGORY_ID=SH.FEE_CATEGORY WHERE SH.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseListByProgrammeAndBatch:
                    {

                        query = @"SELECT 
                                    CCR.COURSE_CODE,
	                                CCR.COURSE_ROOT_ID,
	                                CONCAT('(',
			                                IFNULL(CCR.COURSE_CODE, ''),
			                                ') ',
			                                IFNULL(CCR.COURSE_TITLE, '')) AS COURSE_TITLE
                                FROM
                                    CP_COURSE_PATTERN AS CP
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=CP.PROGRAMME_GROUP_ID
		                                AND SEM.ACADEMIC_YEAR=CP.ACADEMIC_YEAR
                                        AND SEM.IS_DELETED!=1
		                                INNER JOIN
	                                CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=CP.COURSE_ID
		                                AND CCR.SEMESTER_ID=SEM.SEMESTER
										INNER JOIN
									CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=CP.PROGRAMME_GROUP_ID
                                WHERE
                                    CP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND PG.PROGRAMME_GROUP_ID = ?PROGRAMME
                                        AND SEM.BATCH = ?BATCH AND SEM.IS_ACTIVE=1 AND CP.IS_DELETED!=1  GROUP BY COURSE_CODE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHead:
                    {
                        query = @"SELECT 
                                    HEAD_ID,
                                    HEAD,
                                    HEAD_CODE,
                                    ACCOUNT,
                                    HEAD_TYPE,
                                    IS_USED,
                                    IS_DOWNLOADED,
                                    DOWNLOAD_TIME,
                                    IS_UPDATED,
                                    PROGRAMME_MODE,
                                    FEE_CATEGORY,
                                    TEMP_ID,
                                    HEAD_ORDER,
                                    IS_REFUNDABLE,
                                    SHIFT
                                FROM
                                    SUP_HEAD AS H
                                WHERE
                                    H.IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchActiveSemesterByProgrammeAndBatch:
                    {

                        query = @"SELECT 
                                        ACC_SEMESTER_ID,
                                        BATCH,
                                        PG.PROGRAMME_GROUP_ID,
                                        SEMESTER,
                                        DATE_FROM,
                                        DATE_TO
                                    FROM
                                        ACADEMIC_SEMESTER AS ACS
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = ACS.PROGRAMME_GROUP_ID
                                    WHERE
                                        PG.PROGRAMME_GROUP_ID = ?PROGRAMME AND BATCH = ?BATCH
                                            AND IS_ACTIVE = 1
                                            AND ACS.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY SEMESTER;";
                        break;
                    }

                case SupportDataSQLCommands.FetchAcademicBatches:
                    {
                        query = @"SELECT 
                                AB.BATCH_ID, SB.BATCH
                            FROM
                                ACADEMIC_BATCHES AS AB
                                    INNER JOIN
                                ACADEMIC_YEAR AS AY ON AB.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                    INNER JOIN
                                SUP_BATCHES AS SB ON SB.BATCH_ID = AB.BATCH_ID
                            WHERE
                                AY.ACTIVE_YEAR = 1
                                    AND AY.IS_DELETED != 1
                                    AND SB.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlyRegularClasses:
                    {
                        query = @"SELECT 
                                    CLS.CLASS_ID,
                                    CONCAT(IFNULL(CLS.CLASS_DESCRIPTION, ''),
                                            ' (SHIFT ',
                                            S.SHIFT_NAME,
                                            ')') AS CLASS_NAME
                                FROM
                                    CP_CLASSES AS CLS
                                        INNER JOIN
                                    SUP_SHIFT AS S ON S.SHIFT_ID = CLS.SHIFT
                                WHERE
                                    CLS.IS_DELETED != 1
                                        AND CLS.CLASS_TYPE = 2;";
                        break;
                    }

                case SupportDataSQLCommands.FetchHours:
                    {
                        query = @"SELECT 
                                        HOUR_ID, HOUR, TIME_FROM, TIME_TO, DESCRIPTION
                                    FROM
                                        SUP_HOURS
                                    WHERE
                                        IS_DELETED != 1 AND IS_ACTIVE = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassesByStaffIdAndActiveSemester:
                    {
                        query = @"SELECT 
                                        CS.CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                ' (SHIFT ',
                                                IFNULL(ST.SHIFT_NAME, ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CS.COURSE_ID
                                            LEFT JOIN
                                        SUP_SHIFT AS ST ON ST.SHIFT_ID = CLS.SHIFT
                                            INNER JOIN
                                        SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID=CS.SEMESTER_TYPE_ID
                                            AND S.IS_ACTIVE=1
                                    WHERE
                                        STAFF_ID = ?STAFF_ID
                                            AND CS.IS_DELETED!=1 AND CS.ACADEMIC_YEAR=?AC_YEAR
                                    GROUP BY CS.CLASS_ID;";
                        break;
                    }

                case SupportDataSQLCommands.FetchActiveSemesterForStudentByAcademicYear:
                    {
                        query = @"SELECT 
                                        SEM.SEMESTER
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
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SEM.IS_ACTIVE = 1
                                            AND SC.IS_DELETED != 1
                                            AND SC.STUDENT_ID = ?STUDENT_ID;";

                        break;
                    }
                case SupportDataSQLCommands.FetchActiveSemesterForStaffByAcademicYear:
                    {

                        query = @"SELECT 
                                        GROUP_CONCAT(SEMESTER) AS SEMESTER
                                    FROM
                                        (SELECT 
                                            SEMESTER
                                        FROM
                                            ACADEMIC_SEMESTER
                                        WHERE
                                            IS_ACTIVE = 1 AND IS_DELETED!=1
                                            AND ACADEMIC_YEAR=?AC_YEAR
                                        GROUP BY SEMESTER) T";
                        break;
                    }
                case SupportDataSQLCommands.FetchReasonForAttendance:
                    {

                        query = @"SELECT 
                                REASON_ID, REASON, IS_ACTIVE, IS_DELETED
                            FROM
                                SUP_REASON
                            WHERE
                                IS_DELETED != 1;
                             ";
                        break;
                    }
                case SupportDataSQLCommands.FetchReasonForTagStudentMapping:
                    {

                        query = @"SELECT 
                                TAG_ID, TAG_NAME,COLOR_CODE, IS_ACTIVE, IS_DELETED
                            FROM
                                sup_student_tag_list
                            WHERE
                                IS_DELETED != 1;";

                        break;
                    }
                case SupportDataSQLCommands.FetchAttendanceTypes:
                    {

                        query = @"SELECT 
                                    ATTENDANCE_TYPE_ID, ATTENDANCE_TYPE, IS_DELETED, IS_ACTIVE
                                FROM
                                    SUP_ATTENDANCE_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDayOrderInfoByCurrentDate:
                    {

                        query = @"SELECT 
                                            DAY_ID,
                                            DAY_ORDER_DATE,
                                            DAY_TYPE,
                                            IFNULL(DAY_ORDER,'N/A')AS DAY_ORDER,
                                            -- IF(DAY_ORDER='','N/A',DAY_ORDER) AS DAY_ORDER,
                                            IFNULL(DAY_ORDER_MONTH,'N/A') AS DAY_ORDER_MONTH,
                                            IFNULL(IS_HOLIDAY,'0') AS IS_HOLIDAY,
                                            LOGIN_IMG_PATH
                                        FROM
                                            CALENDER AS CAL
                                        WHERE
                                            CAL.DAY_ORDER_DATE = CURDATE() AND IS_DELETED!=1 ;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassesByStaffId:
                    {
                        query = @"SELECT 
                                        CS.CLASS_ID, CLS.CLASS_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            AND CLS.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS RT ON RT.COURSE_ROOT_ID = CS.COURSE_ID
                                            AND RT.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CS.SEMESTER_TYPE_ID
											AND ST.IS_ACTIVE=1
                                    WHERE
                                        CS.STAFF_ID = ?STAFF_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.IS_DELETED != 1
                                    GROUP BY CS.CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseInfoByCourseCode:
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
                                    PAPER,
                                    COURSE_TYPE,
                                    HRS_PER_WEEK,
                                    CREDITS,
                                    OPTION_NAME,
                                    PAPER_CODE,
                                    COURSE_TITLE,
                                    COURSE_CODE,
                                    SEMESTER_ID,
                                    IS_ACTIVE,
                                    IS_NME_SUBJECT,
                                    IS_ALLIED_SUBJECT,
                                    COURSE_ORDER,
                                    IS_DELETED,
                                    SUBJECTS,
                                    IS_COMPULSORY,
                                    UGC_COURSE_TYPE
                                FROM
                                    CP_COURSE_ROOT
                                WHERE
                                    COURSE_CODE = ?COURSE_CODE;";
                        break;

                    }
                case SupportDataSQLCommands.FetchAcademicYearList:
                    {
                        query = @"SELECT 
                                    ACADEMIC_YEAR_ID, AC_YEAR, ACTIVE_YEAR, ACADEMIC_NAME
                                FROM
                                    ACADEMIC_YEAR
                                WHERE
                                    IS_DELETED != 1
                                ORDER BY AC_YEAR DESC;";
                        break;

                    }
                case SupportDataSQLCommands.FetchFeeSupheadList:
                    {
                        query = @"  SELECT 
                                        HEAD_code as HEAD, HEAD_ID
                                    FROM
                                        SUP_HEAD
                                    WHERE
                                        IS_DELETED != 1
                                            AND HEAD_code IS NOT NULL
                                            AND HEAD_code != ''
                                    ORDER BY HEAD_ID DESC;";
                        break;

                    }
                case SupportDataSQLCommands.FetchActiveAcademicYearList:
                    {
                        query = @"SELECT 
                                    ACADEMIC_YEAR_ID, AC_YEAR, ACTIVE_YEAR
                                FROM
                                    ACADEMIC_YEAR
                                WHERE
                                    IS_DELETED != 1 and ACTIVE_YEAR=1 ORDER BY ACADEMIC_YEAR_ID  DESC;";
                        break;

                    }
                case SupportDataSQLCommands.FetchDegree:
                    {
                        query = @"SELECT 
                                                DEGREE_ID,
                                                DEGREE
                                            FROM
                                                SUP_DEGREE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseType:
                    {
                        query = @"SELECT 
                                        COURSE_TYPE_ID, COURSE_TYPE,NO_OF_COMPONENTS
                                    FROM
                                        CP_COURSE_TYPE WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchBloodGroup:
                    {
                        query = @"SELECT 
                                                BLOOD_GROUP_ID,
                                                BLOOD_GROUP_NAME
                                            FROM
                                                SUP_BLOOD_GROUP;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCommunity:
                    {
                        query = @"SELECT 
                                                COMMUNITYID,
                                                COMMUNITY
                                            FROM
                                                SUP_COMMUNITY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchGender:
                    {
                        query = @"SELECT 
                                                GENDER_ID,
                                                GENDER_NAME
                                            FROM
                                                SUP_GENDER where IS_DELETED !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMarritalStatus:
                    {
                        query = @"SELECT 
                                                MARITAL_STAUS_ID,
                                                MARITAL_STATUS_NAME
                                            FROM
                                                SUP_MARRITAL_STATUS;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMotherTongue:
                    {
                        query = @"SELECT 
                                                MOTHER_TONGUE_ID,
                                                MOTHER_TONGUE_NAME
                                            FROM
                                                SUP_MOTHER_TONGUE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchNationality:
                    {
                        query = @"SELECT 
                                                NATIONALITY_ID,
                                                NATIONALITY_NAME
                                            FROM
                                                SUP_NATIONALITY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartment:
                    {
                        query = @"SELECT 
                                                DEPARTMENT_ID,
                                                DEPARTMENT
                                            FROM
                                                CP_DEPARTMENT WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentbyClasses:
                    {
                        query = @"SELECT 
                                        cd.DEPARTMENT_ID, cd.DEPARTMENT
                                    FROM
                                        cp_classes AS cc
                                            INNER JOIN
                                        cp_department AS cd ON cd.DEPARTMENT_ID = cc.DEPARTMENT
                                    WHERE
                                        cd.IS_DELETED != 1
                                            AND cc.IS_DELETED != 1
                                    GROUP BY cd.DEPARTMENT_ID
                                    ORDER BY cd.DEPARTMENT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchReligion:
                    {
                        query = @"SELECT 
                                                RELIGIONID,
                                                RELIGION
                                            FROM
                                                SUP_RELIGION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupState:
                    {
                        query = @"SELECT 
                                                STATE_ID,
                                                STATE_NAME
                                            FROM
                                                SUP_STATE WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchmediyumofStudy:
                    {
                        query = @"  SELECT 
                                        MEDIUM_ID, MEDIUM_NAME
                                    FROM
                                        sup_mediumStudy
                                    WHERE
                                        IS_DELETED != 1";
                        break;
                    }
                case SupportDataSQLCommands.FetchLevel:
                    {
                        query = @"SELECT 
                                                LEVEL_ID,
                                                LEVEL
                                            FROM
                                                SUP_LEVEL;";
                        break;
                    }
                case SupportDataSQLCommands.FetchPublishCategory:
                    {
                        query = @"SELECT 
                                                    PUBLISH_CATEGORY_ID,
                                                    PUBLISH_CATEGORY
                                                FROM
                                                    SUP_PUBLISH_CATEGORY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchVolume:
                    {
                        query = @"SELECT 
                                                VOLUME_ID,
                                                VOLUME
                                            FROM
                                                SUP_VOLUME;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSubCategory:
                    {
                        query = @"SELECT 
                                                STF_CATEGORY_ID,
                                                STF_CATEGORY
                                            FROM
                                                SUP_STAFF_CATEGORY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchShift:
                    {
                        query = @"SELECT 
                                                SHIFT_ID,
                                                SHIFT_NAME
                                            FROM
                                                SUP_SHIFT where IS_DELETED !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchQualification:
                    {
                        query = @"SELECT 
                                                QUALIFICATION_ID,
                                                QUALIFICATION
                                            FROM
                                                SUP_QUALIFICATION;";
                        break;
                    }


                #region Designation
                case SupportDataSQLCommands.FetchDesignation:
                    {
                        query = @"    SELECT 
                                         DESIGNATIONID,
                                         DESIGNATION,
                                         ISUSED,
                                         ISDELETED,
                                         DESIGNATIONORDER
                                      FROM
                                         SUP_DESIGNATION
                                      WHERE
                                          ISDELETED != 1
                                          ORDER BY DESIGNATIONORDER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchsupStaffRolesResponsibile:
                    {
                        query = @"SELECT 
                                    STF_ROLE_RES_ID,
                                    ROLES__RESPONSIBILITIES,
                                    IS_ACTIVE,
                                    IS_DELETED,
                                    ORDER_BY
                                FROM
                                    SUP_STAFF_ROLES_RESPONSIBILE
                                WHERE
                                    IS_DELETED != 1 AND IS_ACTIVE = 1
                                ORDER BY ORDER_BY;";
                        break;
                    }

                case SupportDataSQLCommands.AddDesignation:
                    {
                        query = @"INSERT INTO SUP_DESIGNATION
                                                (
                                                 DESIGNATION,
                                               
                                                 DESIGNATIONORDER)

                                                VALUES
                                                (
                                               ?DESIGNATION,
                                              
                                               ?DESIGNATIONORDER);";


                        break;
                    }


                case SupportDataSQLCommands.UpdateDesignation:
                    {
                        query = @"UPDATE 
                                                SUP_DESIGNATION
                                                SET 
                                                DESIGNATION=?DESIGNATION,
                                               
                                                DESIGNATIONORDER=?DESIGNATIONORDER

                                                WHERE
                                                DESIGNATIONID=?DESIGNATIONID
                                                AND ISDELETED!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchDesignationbyId:
                    {
                        query = @"SELECT
                                            DESIGNATIONID,
                                            DESIGNATION,
                                           
                                            DESIGNATIONORDER


                                           FROM 
                                            SUP_DESIGNATION

                                            WHERE DESIGNATIONID=?DESIGNATIONID
                                            AND ISDELETED !=1;";
                        break;

                    }


                case SupportDataSQLCommands.DeleteDesignationById:
                    {
                        query = @"UPDATE SUP_DESIGNATION 
                                            SET 
                                            ISDELETED=?ISDELETED
                                            WHERE 
                                           DESIGNATIONID=?DESIGNATIONID";
                        break;
                    }


                #endregion



                #region Methodology

                case SupportDataSQLCommands.FetchMethodology:
                    {
                        query = @"    SELECT 
                                         METHODOLOGY_ID,
                                         METHODOLOGY_NAME,
                                         IS_ACTIVE
                                        
                                      FROM
                                         sup_methodology
                                      WHERE
                                          IS_DELETED != 1";

                        break;
                    }

                case SupportDataSQLCommands.AddMethodology:
                    {
                        query = @"INSERT INTO sup_methodology
                                                (
                                                 METHODOLOGY_NAME,
                                               
                                                 IS_ACTIVE)

                                                VALUES
                                                (
                                               ?METHODOLOGY_NAME,
                                              
                                               ?IS_ACTIVE);";


                        break;
                    }


                case SupportDataSQLCommands.UpdateMethodology:
                    {
                        query = @"UPDATE 
                                                sup_methodology
                                                SET 
                                                METHODOLOGY_NAME=?METHODOLOGY_NAME,
                                               
                                                IS_ACTIVE=?IS_ACTIVE

                                                WHERE
                                                METHODOLOGY_ID=?METHODOLOGY_ID
                                                AND IS_DELETED!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchMethodologybyId:
                    {
                        query = @"SELECT
                                            METHODOLOGY_ID,
                                            METHODOLOGY_NAME,
                                           
                                            IS_ACTIVE


                                           FROM 
                                            sup_methodology

                                            WHERE METHODOLOGY_ID=?METHODOLOGY_ID
                                            AND IS_DELETED !=1;";
                        break;

                    }


                case SupportDataSQLCommands.DeleteMethodologyById:
                    {
                        query = @"UPDATE sup_methodology 
                                            SET 
                                            IS_DELETED=?IS_DELETED
                                            WHERE 
                                           METHODOLOGY_ID=?METHODOLOGY_ID";
                        break;
                    }

                #endregion

                #region Lms

                case SupportDataSQLCommands.FetchLmsUnit:
                    {
                        query = @"  SELECT 
                                         LMS_UNIT_ID,
                                         UNIT,
                                         IS_ACTIVE
                                        
                                      FROM
                                         sup_lms_unit
                                      WHERE
                                          IS_DELETED != 1;";

                        break;
                    }

                case SupportDataSQLCommands.AddLms:
                    {
                        query = @"INSERT INTO sup_lms_unit
                                                (
                                                UNIT,
                                                IS_ACTIVE)
                                                VALUES
                                                (
                                               ?UNIT,
                                               ?IS_ACTIVE);";


                        break;
                    }


                case SupportDataSQLCommands.UpdateLms:
                    {
                        query = @"UPDATE 
                                                sup_lms_unit
                                                SET 
                                                UNIT=?UNIT,
                                                IS_ACTIVE=?IS_ACTIVE
                                                WHERE
                                                LMS_UNIT_ID=?LMS_UNIT_ID
                                                AND IS_DELETED!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchLmsbyId:
                    {
                        query = @"SELECT
                                            LMS_UNIT_ID,
                                            UNIT,
                                            IS_ACTIVE
                                           FROM 
                                            sup_lms_unit
                                            WHERE LMS_UNIT_ID=?LMS_UNIT_ID
                                            AND IS_DELETED !=1;";
                        break;

                    }


                case SupportDataSQLCommands.DeleteLmsById:
                    {
                        query = @"UPDATE sup_lms_unit 
                                            SET 
                                            IS_DELETED=?IS_DELETED
                                            WHERE 
                                           LMS_UNIT_ID=?LMS_UNIT_ID";
                        break;
                    }
                #endregion

                #region Lms

                case SupportDataSQLCommands.FetchSupNonTeachingStaff:
                    {
                        query = @"  SELECT 
                                         NON_TEACHING_CATEGORY_ID,
                                         NON_TEACHING_CATEGORY_NAME,
                                         IS_ACTIVE
                                        
                                      FROM
                                         sup_non_teaching_category
                                      WHERE
                                          IS_DELETE != 1;";

                        break;
                    }

                case SupportDataSQLCommands.AddNonTeachingStaff:
                    {
                        query = @"INSERT INTO sup_non_teaching_category
                                                (
                                                NON_TEACHING_CATEGORY_NAME,
                                                IS_ACTIVE)
                                                VALUES
                                                (
                                               ?NON_TEACHING_CATEGORY_NAME,
                                               ?IS_ACTIVE);";


                        break;
                    }


                case SupportDataSQLCommands.UpdateNonTeachingStaff:
                    {
                        query = @"UPDATE 
                                                sup_non_teaching_category
                                                SET 
                                                NON_TEACHING_CATEGORY_NAME=?NON_TEACHING_CATEGORY_NAME,
                                                IS_ACTIVE=?IS_ACTIVE
                                                WHERE
                                                NON_TEACHING_CATEGORY_ID=?NON_TEACHING_CATEGORY_ID
                                                AND IS_DELETE!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchNonTeachingStaffbyId:
                    {
                        query = @"SELECT
                                            NON_TEACHING_CATEGORY_ID,
                                            NON_TEACHING_CATEGORY_NAME,
                                            IS_ACTIVE
                                           FROM 
                                            sup_non_teaching_category
                                            WHERE NON_TEACHING_CATEGORY_ID=?NON_TEACHING_CATEGORY_ID
                                            AND IS_DELETE !=1;";
                        break;

                    }


                case SupportDataSQLCommands.DeleteNonTeachingStaffById:
                    {
                        query = @"UPDATE sup_non_teaching_category 
                                            SET 
                                            IS_DELETE=?IS_DELETE
                                            WHERE 
                                           NON_TEACHING_CATEGORY_ID=?NON_TEACHING_CATEGORY_ID";
                        break;
                    }
                #endregion


                #region Sup_shift
                case SupportDataSQLCommands.FetchSupShift:
                    {
                        query = @"    SELECT 
                                         SHIFT_ID,
                                         SHIFT_NAME,
                                         TIME,
                                         IS_ACTIVE,
                                         DESCRIPTION
                                        
                                      FROM
                                         sup_shift
                                      WHERE
                                          IS_DELETED != 1";

                        break;
                    }

                case SupportDataSQLCommands.AddShift:
                    {
                        query = @"INSERT INTO sup_shift
                                                (
                                                 SHIFT_NAME,
                                                 TIME,
                                                 DESCRIPTION,
                                                 IS_ACTIVE)

                                                VALUES
                                                (
                                               ?SHIFT_NAME,
                                               ?TIME,
                                               ?DESCRIPTION,
                                               ?IS_ACTIVE);";


                        break;
                    }


                case SupportDataSQLCommands.UpdateShift:
                    {
                        query = @"UPDATE 
                                                sup_shift
                                                SET 
                                                SHIFT_NAME=?SHIFT_NAME,
                                                TIME=?TIME,
                                                DESCRIPTION=?DESCRIPTION,
                                                IS_ACTIVE=?IS_ACTIVE

                                                WHERE
                                                SHIFT_ID=?SHIFT_ID
                                                AND IS_DELETED!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchShiftbyId:
                    {
                        query = @"SELECT
                                            SHIFT_ID,
                                            SHIFT_NAME,
                                            TIME,
                                            DESCRIPTION,
                                            IS_ACTIVE

                                           FROM 
                                            sup_shift

                                            WHERE SHIFT_ID=?SHIFT_ID
                                            AND IS_DELETED !=1;";
                        break;

                    }


                case SupportDataSQLCommands.DeleteShiftById:
                    {
                        query = @"UPDATE sup_shift 
                                            SET 
                                            IS_DELETED=?IS_DELETED
                                            WHERE 
                                           SHIFT_ID=?SHIFT_ID";
                        break;
                    }



                #endregion

                case SupportDataSQLCommands.FetchRelaion:
                    {
                        query = @"SELECT 
                                                RELATION_ID, RELATION
                                            FROM
                                                SUP_RELATION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAchievement:
                    {
                        query = @"SELECT 
                                                ACHIEVEMENT_ID,ACHIEVEMENT_NAME
                                            FROM
                                                SUP_ACHIEVEMENT;";
                        break;
                    }
                case SupportDataSQLCommands.Fetchclass:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchBatch:
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
                                    GROUP BY BATCH_ID ORDER BY BATCH_ID DESC;";
                        break;
                    }
                case SupportDataSQLCommands.FetchBatchByProgramId:
                    {
                        query = @"SELECT 
                                    B.BATCH_ID,
                                    B.BATCH
                                FROM
                                    ACADEMIC_CURRICULUM_?AC_YEAR AC 
                                    INNER JOIN SUP_BATCHES AS B ON B.BATCH_ID=AC.BATCH
                                    WHERE AC.PROGRAMME=?PROGRAMME GROUP BY BATCH;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgram:
                    {
                        query = @"SELECT 
                                                PROGRAMME_ID, PROGRAMME_NAME
                                            FROM
                                               CP_PROGRAMME WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupProgramme:
                    {
                        query = @"SELECT 
                                        AP.PROGRAMME_GROUP_ID,
                                        P.PROGRAMME_ID,
                                        CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                ' (SHIFT - ',
                                                IFNULL(SH.SHIFT_NAME, ''),
                                                ') ',
                                                IFNULL(SPM.PROGRAMME_MODE, '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS AP
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = AP.PROGRAMME_ID
                                            AND P.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT SH ON SH.SHIFT_ID = AP.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = AP.PROGRAMME_MODE
                                    WHERE
                                        AP.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSpokenLanguage:
                    {
                        query = @"SELECT 
                                                    SPOKEN_LANGUAGE_ID, SPOKEN_LANGUAGE_NAME
                                                FROM
                                                    SUP_SPOKEN_LANGUAGE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLanguage:
                    {
                        query = @"SELECT 
                                                    LANGUAGE_ID,LANGUAGE_NAME
                                                FROM
                                                    SUP_LANGUAGE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSpeciallyAbled:
                    {
                        query = @"SELECT 
                                                    SPECIALLY_ABLED_ID, SPECIALLY_ABLED_NAME
                                                FROM
                                                   SUP_SPECIALLYABLED;";
                        break;
                    }
                case SupportDataSQLCommands.FetchResidencyType:
                    {
                        query = @"SELECT 
                                                RESIDENCY_TYPE_ID, RESIDENCY_TYPE_NAME
                                            FROM
                                                SUP_RESIDENCY_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLeftReason:
                    {
                        query = @"SELECT 
                                        REASON_ID, REASON
                                    FROM
                                        SUP_LEFT_REASON
                                    WHERE
                                        IS_DELETED != 1 AND IS_ACTIVE = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOccupation:
                    {
                        query = @"SELECT 
                                                    OCCUPATION_ID, OCCUPATION_NAME
                                                FROM
                                                    SUP_OCCUPATION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchEducation:
                    {
                        query = @"SELECT 
                                                EDUCATION_ID,EDUCATION_NAME
                                            FROM
                                                SUP_EDUCATION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchActivity:
                    {
                        query = @"SELECT 
                                                    ACTIVITY_ID,ACTIVITY_NAME
                                                FROM
                                                    sup_activity group by ACTIVITY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchWeekNo:
                    {
                        query = @"SELECT 
                                                    WEEK_NO_ID, WEEK_NAME
                                                FROM
                                                    SUP_WEEK_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchPart:
                    {
                        query = @"SELECT 
                                                    PART_ID,
                                                    PART
                                                FROM
                                                    SUP_PART;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTalentActivityType:
                    {
                        query = @"SELECT 
                                                    ACTIVITY_TYPE_ID,
                                                    ACTIVITY_TYPE
                                                FROM
                                                    SUP_ACTIVITY_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchActivityArea:
                    {
                        query = @"SELECT 
                                                    ACTIVITY_LEVEL_ID,
                                                    ACTIVITY_LEVEL
                                                FROM
                                                    SUP_ACTIVITY_LEVEL;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamPassed:
                    {
                        query = @"SELECT 
                                                    EXAM_PASSED_ID,
                                                    EXAM_PASSED
                                                FROM
                                                    SUP_EXAMPASSED;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOption:
                    {
                        query = @"SELECT 
                                                    OPTION_ID,
                                                    OPTION_NAME
                                                FROM
                                                    SUP_OPTION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchGrade:
                    {
                        query = @"SELECT 
                                                    OVERALL_GRADE_ID,
                                                    OVERALL_GRADE
                                                FROM
                                                    SUP_OVERALL_GRADE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCountry:
                    {
                        query = @"SELECT 
                                                    COUNTRY_ID,
                                                    COUNTRY_NAME
                                                FROM
                                                    SUP_COUNTRY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemester:
                    {
                        query = @"SELECT 
                                                    SUP_SEMESTER_ID, SEMESTER_NAME
                                                FROM
                                                    SUP_SEMESTER where IS_DELETED !=1;";
                        break;
                    }
                //case SupportDataSQLCommands.FetchOptionName:
                //    {
                //        query = @"SELECT 
                //                                    OPTION_ID, OPTION_NAME
                //                                FROM
                //                                    SUP_OPTION_NAME;";
                //        break;
                //    }
                //case SupportDataSQLCommands.FetchIsNME:
                //    {
                //        query = @"SELECT 
                //                                    IS_NME_ID, IS_NME_NAME
                //                                FROM
                //                                    SUP_IS_NME;";
                //        break;
                //    }
                //case SupportDataSQLCommands.FetchIsAllied:
                //    {
                //        query = @"SELECT 
                //                                    IS_ALLIED_ID, IS_ALLIED_NAME
                //                                FROM
                //                                    SUP_IS_ALLIED;";
                //        break;
                //    }
                //case SupportDataSQLCommands.FetchIsCompulsory:
                //    {
                //        query = @"SELECT 
                //                                    IS_COMPULSORY_ID, IS_COMPULSORY_NAME
                //                                FROM
                //                                    SUP_IS_COMPULSORY;";
                //        break;
                //    }
                case SupportDataSQLCommands.FetchUGCCoursetype:
                    {
                        query = @"SELECT 
                                                    UGC_COURSE_TYPE_ID, UGC_COURSE_TYPE
                                                FROM
                                                    CP_UGC_COURSE_TYPE WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchPaperCode:
                    {
                        query = @"SELECT 
                                                    PAPER_CODE_ID, PAPER_CODE
                                                FROM
                                                    SUP_PAPER_CODE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourse:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        CONCAT(IFNULL(COURSE_TITLE, ''),
                                                '  -  ',
                                                IFNULL(COURSE_CODE, '')) AS COURSE_CODE,
                                        TRIM(COURSE_CODE) AS COURSE_TITLE,
                                        SEMESTER_ID
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaff:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        STAFF_CODE,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                '(',
                                                STAFF_CODE,
                                                ')') AS FIRST_NAME,
                                      IFNULL(D.DEPARTMENT, 'OFFICE') AS DEPARTMENT,DEPARTMENT_ID
                                    FROM
                                        STF_PERSONAL_INFO AS SP
                                            LEFT JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SP.DEPARTMENT
                                            LEFT JOIN
                                         USER_ACCOUNT AS UA ON UA.USERNAME=SP.STAFF_CODE
                                    WHERE
                                        SP.IS_DELETED != 1 AND IS_LEFT !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchActiveBatch:
                    {
                        query = @"SELECT 
                                        SBT.BATCH_ID,SBT.BATCH
                                    FROM
                                        ACADEMIC_YEAR AS ACY
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS ACB ON ACB.ACADEMIC_YEAR_ID = ACY.ACADEMIC_YEAR_ID
		                                    INNER JOIN
	                                    SUP_BATCHES AS SBT ON SBT.BATCH_ID=ACB.BATCH_ID WHERE ACY.ACTIVE_YEAR=1";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassType:
                    {
                        query = @"SELECT 
                                        CLASS_TYPE_ID, CLASS_TYPE
                                    FROM
                                        SUP_CLASS_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffConcat:
                    {
                        query = @"SELECT 
                                        STAFF_ID, CONCAT(IFNULL(FIRST_NAME, ''),
                                                '(',
                                                IFNULL(STAFF_CODE, ''),
                                                ')') AS STAFF_NAME
                                    FROM
                                        STF_PERSONAL_INFO WHERE IS_DELETED !=1 AND IS_LEFT !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTeachingStaffOnlyForTT:
                    {
                        query = @"SELECT 
                                    STAFF_ID,
                                    CONCAT(IFNULL(FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(LAST_NAME, ''),
                                            '(',
                                            IFNULL(STAFF_CODE, ''),
                                            ')') AS STAFF_NAME
                                FROM
                                    STF_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = SP.DEPARTMENT
                                WHERE
                                    NON_TEACHING_CATEGORY != 1
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                ORDER BY SP.DEPARTMENT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchNonTeachingCategory:
                    {
                        query = @"SELECT 
                                    NON_TEACHING_CATEGORY_ID,
                                    NON_TEACHING_CATEGORY_NAME
                                FROM
                                    SUP_NON_TEACHING_CATEGORY WHERE IS_DELETE!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTeachingCategory:
                    {
                        query = @"SELECT 
                                    TEACHING_CATEGORY_ID,
                                    TEACHING_CATEGORY,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    SUP_TEACHING_CATEGORY WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFaculty:
                    {
                        query = @"SELECT 
                                        FACULTY_ID, FACULTY
                                    FROM
                                        CP_FACULTY WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassLevel:
                    {
                        query = @"SELECT 
                                        CLASSLEVELID, CLASSLEVEL
                                    FROM
                                        SUP_CLASS_LEVEL WHERE ISDELETED!=1 and ISUSED =1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassYear:
                    {
                        query = @"SELECT 
                                        CLASS_YEAR_ID, CLASS_YEAR
                                    FROM
                                        SUP_CLASS_YEAR
                                    WHERE
                                        IS_DELETED !=1 and IS_ACTIVE =1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchNMECourses:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID, COURSE_CODE
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        IS_NME_SUBJECT = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeLevel:
                    {
                        query = @"SELECT 
                                        PROGRAMME_LEVEL_ID, PROGRAMME_LEVEL
                                    FROM
                                        SUP_PROGRAMME_LEVEL WHERE IS_DELETED!=1 AND IS_ACTIVE=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeLevelWithoutActive:
                    {
                        query = @"SELECT 
                                        PROGRAMME_LEVEL_ID, PROGRAMME_LEVEL
                                    FROM
                                        SUP_PROGRAMME_LEVEL WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDurationUnit:
                    {
                        query = @"SELECT 
                                        DURATION_UNIT_ID, DURATION_UNIT
                                    FROM
                                        SUP_DURATION_UNIT WHERE IS_DELETED !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchChannel:
                    {
                        query = @"SELECT 
                                        CHANNEL_ID, CHANNEL
                                    FROM
                                        SUP_CHANNEL
                                    WHERE
                                        IS_DELETED = 0";
                        break;
                    }
                case SupportDataSQLCommands.FetchSubjects:
                    {
                        query = @"SELECT 
                                        SUBJECT_ID, SUBJECT
                                    FROM
                                        SUP_SUBJECT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMediumOfInstruction:
                    {
                        query = @"SELECT 
                                        MEDIUM_ID, MEDIUM_OF_INSTRUCTION
                                    FROM
                                        SUP_MEDIUM_OF_INSTRUCTION WHERE IS_DELETED=0;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeType:
                    {
                        query = @"SELECT 
                                        PROGRAMME_TYPE_ID, PROGRAMME_TYPE
                                    FROM
                                        SUP_PROGRAMME_TYPE WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllCourseList:
                    {
                        query = @"SELECT COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE,''),'(',IFNULL(COURSE_CODE,''),')') AS COURSE_TITLE 
                                    FROM CP_COURSE_ROOT WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlyAlliedOptionalCoursesList:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE
                                FROM
                                    CP_COURSE_ROOT_?AC_YEAR
                                WHERE
                                    IS_NME_SUBJECT != 1
                                        AND IS_ALLIED_SUBJECT = 1
                                        AND IS_COMPULSORY = 0
                                        AND IS_DELETED != 1";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlyNMECoursesList:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE,
                                    IF(COURSE_ID IS NOT NULL,'SELECTED', '') AS SELECTED
                                FROM
                                    CP_COURSE_ROOT AS CR
                                        LEFT JOIN
                                    NME_COURSE_REGISTRATION AS NCR ON NCR.COURSE_ID = CR.COURSE_ROOT_ID
                                        AND NCR.IS_DELETED != 1
                                        AND NCR.ACADEMIC_YEAR = ?AC_YEAR
                                WHERE
                                    IS_NME_SUBJECT = 1
                                        AND CR.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlyRegularCoursesList:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(IFNULL(COURSE_TITLE, ''),
                                            '(',
                                            IFNULL(COURSE_CODE, ''),
                                            ')') AS COURSE_TITLE
                                FROM
                                    CP_COURSE_ROOT
                                WHERE
                                    IS_COMPULSORY = 1 AND IS_DELETED != 1";
                        break;
                    }
                case SupportDataSQLCommands.FetchBoardMmber:
                    {
                        query = @"SELECT 
                                        BOARD_MEMBER_ID, BOARD_MEMBER_NAME
                                    FROM
                                        SUP_BOARD_MEMBER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchNMESetting:
                    {
                        query = @"SELECT 
                                        SETTINGS_ID, SETTINGS_NAME, CLASS_ID
                                    FROM
                                        NME_SETTINGS WHERE IS_DELETED!=1 AND IS_ALLOWED!=0 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByShiftId:
                    {
                        query = @"SELECT 
                                    CLASS_ID,
                                    CLASS_NAME
                                FROM
                                    CP_CLASSES
                                WHERE
                                    SHIFT = ?SHIFT  
                                    AND IS_DELETED!=1 
                                ORDER BY CLASS_TYPE,PROGRAMME_GROUP_ID,CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlyOrdinaryClass:
                    {
                        query = @"SELECT 
                                    CLASS_ID,
                                    CLASS_CODE
                                FROM
                                    CP_CLASSES
                                WHERE
                                     COURSE_ID=0 AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAcademicYearById:
                    {
                        query = @"SELECT 
                                    ACADEMIC_YEAR_ID, AC_YEAR, ACTIVE_YEAR
                                FROM
                                    ACADEMIC_YEAR
                                WHERE
                                    IS_DELETED != 1 AND ACADEMIC_YEAR_ID=?ACADEMIC_YEAR_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgramByShiftId:
                    {
                        query = @"SELECT 
                                        PROGRAMME_ID, PROGRAMME_DESCRIPTION
                                    FROM
                                        CP_PROGRAMME_?AC_YEAR AS PR
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.PROGRAMME = PR.PROGRAMME_ID
                                    WHERE
                                        CLS.SHIFT = ?SHIFT
                                    GROUP BY CLS.PROGRAMME;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByAcademicYear:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE                                      
                                        CLS.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentByClassId:
                    {
                        query = @"SELECT 
                                                CLASS_ID,
                                                DEPARTMENT
                                            FROM
                                                CP_CLASSES_?AC_YEAR WHERE CLASS_ID=?CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeAndClassYearByClassId:
                    {
                        query = @"SELECT 
                                        CLASS_YEAR, PROGRAMME
                                    FROM
                                        CP_CLASSES_?AC_YEAR
                                    WHERE
                                        CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByProgrammeAndClassYear:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES_?AC_YEAR CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.CLASS_YEAR = ?CLASS_YEAR AND CLS.PROGRAMME = ?PROGRAMME;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByClassYear:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.CLASS_YEAR = ?CLASS_YEAR  AND CLS.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassLevelByClassId:
                    {
                        query = @"SELECT 
                                        CLASS_LEVEL
                                    FROM
                                        CP_CLASSES_?AC_YEAR
                                    WHERE
                                        CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByClassLevelAndClassYear:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES_?AC_YEAR CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.CLASS_YEAR = ?CLASS_YEAR AND CLS.CLASS_LEVEL=?CLASS_LEVEL AND CLS.COURSE_ID='' AND CLS.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentPersonalInfo:
                    {
                        query = @"SELECT 
                                        STUDENT_ID, CONCAT(IFNULL(FIRST_NAME, ''),
                                                '(',
                                                IFNULL(ROLL_NO, ''),
                                                ')') AS FIRST_NAME
                                    FROM
                                        STU_PERSONAL_INFO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterByCourseId:
                    {
                        query = @"SELECT 
                                        SEMESTER_ID
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        COURSE_ROOT_ID = ?COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDayOrders:
                    {
                        query = @"SELECT 
                                        DAY_ORDER_ID,DAY
                                    FROM
                                        SUP_DAY_ORDER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHourType:
                    {
                        query = @"SELECT 
                                        hour_type_id, hourType
                                    FROM
                                        SUP_HOUR_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.CheckIsOptionalPaper:
                    {
                        query = @"SELECT 
                                        COURSE_CODE
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        COURSE_ROOT_ID = ?COURSE_ID
                                            AND IS_NME_SUBJECT = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchPaperType:
                    {
                        query = @"SELECT 
                                        PAPER_TYPE
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        COURSE_ROOT_ID = ?COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTimeTableSetting:
                    {
                        query = @"SELECT 
                                        SETTING_ID, SETTING_NAME
                                    FROM
                                        TT_TEMPLATE_SETTING WHERE IS_DELETED!=1 AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFeeMainHead:
                    {
                        query = @"SELECT 
                                        MAIN_HEAD_ID, MAIN_HEAD
                                    FROM
                                        SUP_FEE_MAIN_HEAD
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSupFeeMainHead:
                    {
                        query = @"INSERT INTO SUP_FEE_MAIN_HEAD(MAIN_HEAD)VALUES(?MAIN_HEAD);";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFeeMainHeadById:
                    {
                        query = @"SELECT 
                                        MAIN_HEAD_ID, MAIN_HEAD
                                    FROM
                                        SUP_FEE_MAIN_HEAD
                                    WHERE
                                        MAIN_HEAD_ID=?MAIN_HEAD_ID AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupFeeMainHead:
                    {
                        query = @"UPDATE SUP_FEE_MAIN_HEAD
                                        SET
                                        MAIN_HEAD = ?MAIN_HEAD
                                        WHERE MAIN_HEAD_ID = ?MAIN_HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupFeeMainHead:
                    {
                        query = @"UPDATE SUP_FEE_MAIN_HEAD
                                        SET
                                        IS_DELETED = 1
                                        WHERE MAIN_HEAD_ID = ?MAIN_HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.IsSupFeeMainHeadExisting:
                    {
                        query = @"SELECT 
                                        MAIN_HEAD_ID
                                    FROM
                                        SUP_FEE_MAIN_HEAD
                                    WHERE
                                        MAIN_HEAD=?MAIN_HEAD AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllPaperType:
                    {
                        query = @"SELECT 
                                        PAPER_TYPE_ID,PAPER_TYPE
                                    FROM
                                        SUP_PAPER_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllFrequency:
                    {
                        query = @"SELECT 
                                        FREQUENCY_ID, FREQUENCY_NAME
                                    FROM
                                        SUP_FEE_FREQUENCY
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllStudent:
                    {
                        query = @"SELECT 
                                        STUDENT_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                ' (',
                                                REGISTER_NO,
                                                ')') AS FIRST_NAME
                                    FROM
                                        STU_PERSONAL_INFO
                                    WHERE
                                        IS_DELETED != 1 AND IS_LEFT != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoByClassID:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(IFNULL(FIRST_NAME, ''), ifnull(LAST_NAME,''),
                                    ' (',
                                    IFNULL(REGISTER_NO, ''),
                                    ')') AS FIRST_NAME
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_CLASS AS SCLS ON SCLS.STUDENT_ID = SP.STUDENT_ID
                                WHERE
                                    SP.IS_DELETED != 1 AND IS_LEFT != 1
                                        AND SCLS.CLASS_ID = ?CLASS_ID
                                        AND SCLS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoByStudentId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.RFID,
                                        SP.BIOD_NUMBER,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CLS.CLASS_ID,
                                        CLS.CLASS_CODE,
                                        SP.REGISTER_NO,
                                        SS.SHIFT_NAME,
                                        CLS.SHIFT AS SHIFT_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CLS.SHIFT
                                    WHERE
                                        SP.IS_DELETED != 1 AND SP.IS_LEFT != 1 AND SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR AND SP.STUDENT_ID =?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentRollNoByStudentId:
                    {
                        query = @"SELECT 
                                      STUDENT_ID,
                                      FIRST_NAME,
                                      ROLL_NO,
                                      DATE_FORMAT(DATE_OF_BIRTH, '%d-%m-%Y') AS DATE_OF_BIRTH
                                  FROM
                                      STU_PERSONAL_INFO
                                  WHERE
                                      STUDENT_ID IN (?STUDENT_ID)
                                          AND IS_DELETED != 1
                                          AND IS_LEFT != 1 ORDER BY ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupProgrammeMode:
                    {
                        query = @"SELECT 
                                        PROGRAMME_MODE_ID, PROGRAMME_MODE
                                    FROM
                                        SUP_PROGRAMME_MODE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupBank:
                    {
                        query = @"SELECT 
                                        BANK_ID,
                                        BANK_NAME,
                                        ADDRESS,
                                        PHONE
                                    FROM
                                        SUP_BANK WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupBank:
                    {
                        query = @"UPDATE SUP_BANK SET IS_DELETED=1 WHERE BANK_ID=?BANK_ID;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSupBank:
                    {
                        query = @"INSERT INTO SUP_BANK
                                        (
                                        BANK_NAME,
                                        ADDRESS,
                                        PHONE)
                                        VALUES
                                        (
                                        ?BANK_NAME,
                                        ?ADDRESS,
                                        ?PHONE);";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupBankById:
                    {
                        query = @"SELECT 
                                        BANK_ID,
                                        BANK_NAME,
                                        ADDRESS,
                                        PHONE
                                    FROM
                                        SUP_BANK WHERE IS_DELETED!=1 AND BANK_ID=?BANK_ID;";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupBank:
                    {
                        query = @"UPDATE SUP_BANK
                                            SET
                                            BANK_NAME = ?BANK_NAME,
                                            ADDRESS = ?ADDRESS,
                                            PHONE = ?PHONE
                                            WHERE BANK_ID = ?BANK_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupBankAccount:
                    {
                        query = @"SELECT 
                                        SBA.BANK_ACCOUNT_ID,
                                        SBA.ACCOUNT_PURPOSE,
                                        SBA.PASSBOOK_NO,
                                        SB.BANK_NAME,
                                        DATE_FORMAT(SBA.STARTED_DATE, '%d/%m/%y') AS STARTED_DATE,
                                        DATE_FORMAT(SBA.CLOSED_DATE, '%d/%m/%y') AS CLOSED_DATE
                                    FROM
                                        SUP_BANK_ACCOUNT AS SBA
                                            LEFT JOIN
                                        SUP_BANK AS SB ON SB.BANK_ID = SBA.BANK
                                    WHERE
                                        SBA.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchBankList:
                    {
                        query = @"SELECT 
                                        BANK_ID, BANK_NAME
                                    FROM
                                        SUP_BANK
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupBankAccount:
                    {
                        query = @"UPDATE SUP_BANK_ACCOUNT SET IS_DELETED=1 WHERE BANK_ACCOUNT_ID=?BANK_ACCOUNT_ID; ";
                        break;
                    }
                case SupportDataSQLCommands.SaveSupBankAccount:
                    {
                        query = @"INSERT INTO SUP_BANK_ACCOUNT
                                        (
                                        ACCOUNT_PURPOSE,
                                        BANK,
                                        PASSBOOK_NO,
                                        STARTED_DATE,
                                        CLOSED_DATE)
                                        VALUES
                                        (
                                        ?ACCOUNT_PURPOSE,
                                        ?BANK,
                                        ?PASSBOOK_NO,
                                        ?STARTED_DATE,
                                        ?CLOSED_DATE);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupBankAccount:
                    {
                        query = @"UPDATE SUP_BANK_ACCOUNT
                                            SET
                                            BANK_ACCOUNT_ID= ?BANK_ACCOUNT_ID,
                                            ACCOUNT_PURPOSE= ?ACCOUNT_PURPOSE,
                                            BANK= ?BANK,
                                            PASSBOOK_NO= ?PASSBOOK_NO,
                                            STARTED_DATE= ?STARTED_DATE,
                                            CLOSED_DATE= ?CLOSED_DATE
                                            WHERE BANK_ACCOUNT_ID= ?BANK_ACCOUNT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupBankAcountByAccountId:
                    {
                        query = @"SELECT 
                                        BANK_ID
                                    FROM
                                        SUP_BANK
                                    WHERE
                                        IS_DELETED != 1 AND PASSBOOK_NO=?PASSBOOK_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupBankAcountById:
                    {
                        query = @"SELECT 
                                        ACCOUNT_PURPOSE,
                                        BANK,
                                        PASSBOOK_NO,
                                        DATE_FORMAT(STARTED_DATE, '%d/%m/%Y') AS STARTED_DATE,
                                        DATE_FORMAT(CLOSED_DATE, '%d/%m/%Y') AS CLOSED_DATE
                                    FROM
                                        SUP_BANK_ACCOUNT
                                    WHERE
                                        BANK_ACCOUNT_ID = ?BANK_ACCOUNT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSetting:
                    {
                        query = @"SELECT 
                                        EXAM_SETTING_ID, concat(EXAM_NAME,'-',ACADEMIC_YEAR) as EXAM_NAME
                                    FROM
                                        EXAM_SETTING
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllExamSetting:
                    {
                        query = @"SELECT 
                                        EXAM_SETTING_ID, concat(EXAM_NAME,'-',ACADEMIC_YEAR) as EXAM_NAME
                                    FROM
                                        EXAM_SETTING
                                    WHERE
                                        IS_DELETED != 1 ORDER BY EXAM_SETTING_ID DESC;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCIAExamSetting:
                    {
                        query = @"SELECT 
                                EXAM_SETTING_ID,
                                CONCAT(EXAM_NAME, '- ', SEM.SEMESTER_TYPE) AS EXAM_NAME
                            FROM
                                CIA_EXAM_SETTING AS ES
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS SEM ON SEM.SEMESTER_TYPE_ID = ES.SEMESTER
                            WHERE
                                ES.IS_DELETED != 1
                                    AND ES.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamTypeByExamSetting:
                    {
                        query = @"SELECT 
                                        EXAM_SETTING_ID, concat(EXAM_NAME,'-',ACADEMIC_YEAR) as EXAM_NAME,EXAM_TYPE
                                    FROM
                                        EXAM_SETTING
                                    WHERE
                                        IS_DELETED != 1 AND EXAM_SETTING_ID=?EXAM_SETTING_ID AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupPaperType:
                    {
                        query = @"SELECT 
                                        PAPER_TYPE_ID, PAPER_TYPE
                                    FROM
                                        SUP_PAPER_TYPE WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSubjectType:
                    {
                        query = @"SELECT 
                                        SUBJECT_TYPE_ID, SUBJECT_TYPE
                                    FROM
                                        SUP_SUBJECT_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSplAttendanceType:
                    {
                        query = @"SELECT 
                                    SPL_ATTENDANCE_TYPE_ID, SPL_ATTENDANCE_TYPE
                                FROM
                                    SUP_SPL_ATTENDANCE_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOptionalPaperByCourseId:
                    {
                        query = @"SELECT 
                                        COURSE_CODE,COURSE_ROOT_ID AS COURSE_ID
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        COURSE_ROOT_ID = ?COURSE_ID
                                            AND IS_COMPULSORY!= 1;";
                        break;
                    }
                case SupportDataSQLCommands.CheckIsSelectedClass:
                    {
                        query = @"SELECT 
                                        COURSE_ID
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        CLASS_ID = ?CLASS_ID AND IS_CHOICE = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterStartDateByClassId:
                    {

                        query = @"SELECT 
                                    DATE_FORMAT(SEM.DATE_FROM,'%Y-%m-%d') AS DATE_FROM,
                                    DATE_FORMAT(SEM.DATE_TO,'%Y-%m-%d') AS DATE_TO
                                FROM
                                    ACADEMIC_SEMESTER AS SEM
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = ?CLASS_ID
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ?COURSE_ID
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS B ON B.BATCH_YEAR = C.CLASS_YEAR
                                        AND ACADEMIC_YEAR_ID = (SELECT 
                                            ACADEMIC_YEAR_ID
                                        FROM
                                            ACADEMIC_YEAR
                                        WHERE
                                            AC_YEAR = SEM.ACADEMIC_YEAR LIMIT 1)
                                WHERE
                                    SEM.SEMESTER = CCR.SEMESTER_ID
                                        AND SEM.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                        AND SEM.BATCH=B.BATCH_ID
                                        AND SEM.ACADEMIC_YEAR=?AC_YEAR
		                                AND SEM.IS_ACTIVE=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterEndDateByClassId:
                    {
                        query = @"SELECT 
                                        DATE_FORMAT(ASE.DATE_TO,'%Y-%m-%d') AS DATE_TO
                                    FROM
                                        ACADEMIC_SEMESTER_?AC_YEAR AS ASE
                                            INNER JOIN
                                        CP_CLASSES_?AC_YEAR AS CLS ON CLS.PROGRAMME = ASE.PROGRAMME
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.CLASS_ID = CLS.CLASS_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        ASE.PROGRAMME = CLS.PROGRAMME
                                            AND ASE.BATCH = SP.BATCH
                                            AND IS_ACTIVE = 1
                                            AND CLS.CLASS_ID = 1
                                    GROUP BY SP.BATCH
                                    LIMIT 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSubFeeFrequency:
                    {
                        query = @"SELECT 
                                        SF.FREQUENCY_ID,
                                        SF.FREQUENCY_NAME,
                                        SF.ACADEMIC_YEAR,
                                        date_format(SF.DATE_FROM,'%Y/%m/%d') as DATE_FROM,
                                        date_format(SF.DATE_TO,'%Y/%m/%d') as DATE_TO,
                                        date_format(SF.LAST_DATE_OF_PAYMENT,'%Y/%m/%d') as LAST_DATE_OF_PAYMENT,
                                        SFT.FREQUENCY_TYPE
                                    FROM
                                        SUP_FEE_FREQUENCY AS SF
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = SF.TYPE
                                    WHERE
                                        SF.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFrequencytype:
                    {
                        query = @"SELECT 
                                        FREQUENCY_TYPE_ID, FREQUENCY_TYPE
                                    FROM
                                        SUP_FREQUENCY_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSubFeeFrequency:
                    {
                        query = @"INSERT INTO SUP_FEE_FREQUENCY
                                            (
                                            ACADEMIC_YEAR,
                                            FREQUENCY_NAME,
                                            DATE_FROM,
                                            DATE_TO,
                                            LAST_DATE_OF_PAYMENT,
                                            TYPE,
                                            SEMESTER_TYPE)
                                            VALUES
                                            (
                                            ?ACADEMIC_YEAR,
                                            ?FREQUENCY_NAME,
                                            ?DATE_FROM,
                                            ?DATE_TO,
                                            ?LAST_DATE_OF_PAYMENT,
                                            ?TYPE,
                                            ?SEMESTER_TYPE);";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterType:
                    {
                        query = @"SELECT 
                                        SEMESTER_TYPE_ID, SEMESTER_TYPE,IS_ACTIVE
                                    FROM
                                        SUP_SEMESTER_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchSupMode:
                    {
                        query = @"SELECT 
                                        MODE_ID, MODE_NAME,IS_ACTIVE
                                    FROM
                                        SUP_EVENT_MODE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupMessageType:
                    {
                        query = @"SELECT 
                                    MSG_TYPE_ID, MESSAGE_TYPE_NAME
                                FROM
                                    sup_message_type
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiRacks:
                    {
                        query = @"SELECT 
                                        RACK_ID, RACK_NAME
                                    FROM
                                        SUP_LI_RACKS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiCells:
                    {
                        query = @"SELECT 
                                        CELL_ID, CELL_NAME
                                    FROM
                                        SUP_LI_CELLS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.CheckIsFrequencyExisting:
                    {
                        query = @"SELECT 
                                        FREQUENCY_ID
                                    FROM
                                        SUP_FEE_FREQUENCY
                                    WHERE
                                        TYPE = ?TYPE AND SEMESTER_TYPE = ?SEMESTER_TYPE
                                            AND FREQUENCY_NAME = ?FREQUENCY_NAME AND ACADEMIC_YEAR=?ACADEMIC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFeeFrequencyById:
                    {
                        query = @"SELECT 
                                        FREQUENCY_ID,
                                        ACADEMIC_YEAR,
                                        FREQUENCY_NAME,
                                        date_format(DATE_FROM,'%d/%m/%Y') as DATE_FROM,
	                                    date_format(DATE_TO,'%d/%m/%Y') as DATE_TO,
	                                    date_format(LAST_DATE_OF_PAYMENT,'%d/%m/%Y') as LAST_DATE_OF_PAYMENT,
                                        FREQUENCY_TYPE_ID AS FREQUENCY_TYPE,
                                        SEMESTER_TYPE
                                    FROM
                                        SUP_FEE_FREQUENCY
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = TYPE
                                    WHERE
                                        FREQUENCY_ID = ?FREQUENCY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupFeeFrequency:
                    {
                        query = @"UPDATE SUP_FEE_FREQUENCY
                                            SET
                                            FREQUENCY_NAME= ?FREQUENCY_NAME,
                                            DATE_FROM= ?DATE_FROM,
                                            DATE_TO= ?DATE_TO,
                                            LAST_DATE_OF_PAYMENT= ?LAST_DATE_OF_PAYMENT,
                                            TYPE= ?TYPE,
                                            SEMESTER_TYPE= ?SEMESTER_TYPE
                                            WHERE FREQUENCY_ID=?FREQUENCY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupFeeFrequency:
                    {
                        query = @"UPDATE SUP_FEE_FREQUENCY
                                            SET
                                            IS_DELETED=1 WHERE FREQUENCY_ID=?FREQUENCY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFrequency:
                    {
                        query = @"SELECT 
                                        FREQUENCY_ID, FREQUENCY_NAME
                                    FROM
                                        SUP_FEE_FREQUENCY
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDiscount:
                    {
                        query = @"SELECT 
                                        DISCOUNT_ID, DISCOUNT_NAME
                                    FROM
                                        FEE_DISCOUNT
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeeCategory:
                    {
                        query = @"SELECT 
                                        FEE_CATEGORY_ID, FEE_CATEGORY
                                    FROM
                                        SUP_FEE_CATEGORY
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                         CLS.PROGRAMME_GROUP_ID IN(?PROGRAMME_GROUP_ID);";
                        break;
                    }
                case SupportDataSQLCommands.FetchHeadByFeeCategory:
                    {
                        query = @"SELECT 
                                        HEAD_ID, HEAD
                                    FROM
                                        SUP_HEAD
                                    WHERE
                                        IS_DELETED != 1 AND FEE_CATEGORY = ?FEE_CATEGORY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFrequencyByType:
                    {
                        query = @"SELECT 
                                        FREQUENCY_ID, FREQUENCY_NAME
                                    FROM
                                        SUP_FEE_FREQUENCY 
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR AND TYPE=?TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoByClassIdAndDiscountId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    CONCAT(IFNULL(FIRST_NAME, ''),
                                            ' (',
                                            REGISTER_NO,
                                            ')') AS FIRST_NAME,IF(FDA.DISCOUNT_ALLOTMENT_ID IS NOT NULL,'SELECTED','' ) AS STATUS
                                FROM
                                    STU_PERSONAL_INFO AS SP
                                        INNER JOIN
                                    STU_CLASS AS SCLS ON SCLS.STUDENT_ID = SP.STUDENT_ID
									LEFT JOIN FEE_DISCOUNT_ALLOTMENT AS FDA ON FDA.STUDENT_ID=SCLS.STUDENT_ID AND FDA.IS_DELETED!=1 AND FDA.DISCOUNT_ID=?DISCOUNT_ID
                                WHERE
                                    SP.IS_DELETED != 1 AND IS_LEFT != 1
                                        AND SCLS.CLASS_ID = ?CLASS_ID
                                        AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFrequencyByTypeAndDiscountId:
                    {
                        query = @"SELECT 
                                        SFF.FREQUENCY_ID,
                                        SFF.FREQUENCY_NAME,
                                        IF(FDF.DISCOUNT_ID IS NOT NULL,
                                            'SELECTED',
                                            '') AS STATUS
                                    FROM
                                        SUP_FEE_FREQUENCY AS SFF
                                            LEFT JOIN
                                        FEE_DISCOUNT_FREQUENCY AS FDF ON FDF.FREQUENCY_ID = SFF.FREQUENCY_ID
                                            AND FDF.DISCOUNT_ID = ?DISCOUNT_ID AND FDF.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFrequencyByDiscountId:
                    {
                        query = @"SELECT 
                                        SFF.FREQUENCY_ID, SFF.FREQUENCY_NAME
                                    FROM
                                        SUP_FEE_FREQUENCY AS SFF
                                            INNER JOIN
                                        FEE_DISCOUNT_FREQUENCY AS FDF ON FDF.FREQUENCY_ID = SFF.FREQUENCY_ID
                                    WHERE
                                        FDF.DISCOUNT_ID = ?DISCOUNT_ID
                                            AND FDF.IS_DELETED != 1
                                            AND SFF.IS_DELETED != 1
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMainCommunity:
                    {
                        query = @"SELECT 
                                        MAIN_COMMUNITY_ID, MAIN_COMMUNITY
                                    FROM
                                        SUP_MAIN_COMMUNITY
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchState:
                    {
                        query = @"SELECT 
                                        STATE_ID, STATE
                                    FROM
                                        SUP_STATE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchUniversity:
                    {
                        query = @"SELECT 
                                        UNIVERSITY_ID, UNIVERSITY
                                    FROM
                                        SUP_UNIVERSITY
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHSSGroups:
                    {
                        query = @"SELECT 
                                        HSS_GROUP_ID, HSS_GROUP, HSS_GROUP_CODE
                                    FROM
                                        SUP_HSS_GROUPS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeByApprogramme:
                    {
                        query = @"SELECT 
                                    AP.PROGRAMME_GROUP_ID,
                                    P.PROGRAMME_ID,
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
                case SupportDataSQLCommands.FetchProgrammesByApplicationType:
                    {
                        query = @"SELECT 
                                    AP.PROGRAMME_GROUP_ID,
                                    P.PROGRAMME_ID,
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
                                        INNER JOIN
                                    SUP_APPLICATION_TYPE AS APT ON APT.APPLICATION_TYPE_ID = AP.APPTYPE_ID
                                        AND APT.IS_DELETED != 1
                                WHERE
                                    AP.IS_DELETED != 1
                                        AND APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID
                                ORDER BY AP.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchADMClass:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        ADM_CLASSES_?AC_YEAR CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassAndProgramByStudentId:
                    {
                        query = @"SELECT 
                                    CP.PROGRAMME_ID,
                                    CP.PROGRAMME_CODE,
                                    CC.CLASS_CODE,
                                    SC.CLASS_ID,
                                    SC.STUDENT_ID
                                FROM
									STU_CLASS AS SC
										INNER JOIN
									STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
                                        INNER JOIN
									CP_PROGRAMME_GROUP AS P ON P.PROGRAMME_GROUP_ID=SP.PROGRAM_ID
										INNER JOIN
									CP_PROGRAMME AS CP ON CP.PROGRAMME_ID=P.PROGRAMME_ID 
                                        INNER JOIN
									CP_CLASSES AS CC ON CC.CLASS_ID=SC.CLASS_ID
                                WHERE
                                    SC.STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClasses:
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
                                        CLS.IS_DELETED != 1
                                    ORDER BY CLS.CLASS_TYPE,CLS.CLASS_LEVEL,CLS.PROGRAMME_GROUP_ID,CLS.CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOrdinaryClassesByShiftId:
                    {
                        query = @"SELECT 
                                        CLS.CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_CODE, ''),
                                             ' (',
                                             IFNULL(CONCAT(SPM.PROGRAMME_MODE), ''),
                                             ')',
                                             '-',
                                             IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS CLASS_NAME
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
                                        WHERE CLS.COURSE_ID=0 AND CLS.SHIFT=?SHIFT ORDER BY CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlyAllotedNMECoursesList:
                    {
                        query = @"";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoByUserId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CLS.CLASS_ID,
                                        CLS.CLASS_CODE,
                                        CLS.CLASS_NAME AS 'CLASS',
                                        SP.REGISTER_NO,
                                        SS.SHIFT_NAME,
                                        CP.PROGRAMME_NAME
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
											INNER JOIN 
										CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID=CLS.PROGRAMME_GROUP_ID
											INNER JOIN
										CP_PROGRAMME AS CP ON CP.PROGRAMME_ID=PG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CLS.SHIFT
                                    WHERE
                                        SP.IS_DELETED != 1 AND SP.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR AND SP.STUDENT_ID=?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupById:
                    {
                        query = @"SELECT 
                                        PROGRAMME_GROUP_ID,
                                        APPTYPE_ID,
                                        PROGRAMME_ID,
                                        ACADEMIC_YEAR,
                                        IS_NEW,
                                        SHIFT,
                                        PROGRAMME_MODE
                                    FROM
                                        CP_PROGRAMME_GROUP
                                    WHERE
                                        PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupByShiftAndProgrammeId:
                    {
                        query = @"SELECT 
                                    AP.PROGRAMME_GROUP_ID,
                                    P.PROGRAMME_ID,
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
                                    AP.IS_DELETED != 1 AND AP.PROGRAMME_MODE=?PROGRAMME_MODE AND AP.PROGRAMME_ID=?PROGRAMME_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHoursByStaffIdAndActiveSemester:
                    {
                        query = @"SELECT 
                                        SH.HOUR_ID,
                                        CONCAT(IFNULL(SH.HOUR, ''),
                                                ' (',
                                                IFNULL(ST.HOURTYPE, ''),
                                                ')') AS HOUR,
                                         TT.COURSE_ID,
                                         CR.SEMESTER_ID
                                    FROM
                                        TT_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TT.HOUR_ID
                                            INNER JOIN
                                        SUP_HOUR_TYPE AS ST ON ST.HOUR_TYPE_ID = TT.HOUR_TYPE
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=TT.COURSE_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND S.IS_ACTIVE = 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID=CCS.CLASS_ID
                                            AND CC.SHIFT=?SHIFT
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()
                                                    AND ACADEMIC_YEAR = ?AC_YEAR)
                                    GROUP BY SH.HOUR_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeMode:
                    {
                        query = @"SELECT 
                                        FFF.FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID,
                                        CONCAT(IFNULL(SFF.FREQUENCY_NAME, ''),
                                        '( ',
                                        IFNULL(SFT.FREQUENCY_TYPE, ''),
                                        ')') AS FREQUENCY_NAME
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                            INNER JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE
                                            AND SST.IS_ACTIVE = 1
                                    WHERE
                                        FFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFT.IS_DELETED != 1
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFeeFrequencyByFeeFrequencyFeeModeAndFeeRootId:
                    {
                        query = @"SELECT 
                                        FFF.FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID,
                                        CONCAT(IFNULL(SFF.FREQUENCY_NAME, ''),
                                        '( ',
                                        IFNULL(SFT.FREQUENCY_TYPE, ''),'-',IFNULL(FFF.ACADEMIC_YEAR,''),
                                        ')') AS FREQUENCY_NAME,
                                        FFF.FEE_MODE
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE AND SFT.FEE_ROOT_ID in(?FEE_ROOT_ID)
                                            LEFT JOIN
                                        SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE
                                            AND SST.IS_ACTIVE = 1
                                    WHERE
                                        FFF.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SFT.IS_DELETED != 1
                                            AND SFF.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMainHeadsByFrequencyId:
                    {
                        query = @"SELECT 
                                    MH.FEE_MAIN_HEAD_ID, SMH.MAIN_HEAD
                                FROM
                                    FEE_MAIN_HEADS AS MH
                                        INNER JOIN
                                    SUP_FEE_MAIN_HEAD AS SMH ON SMH.MAIN_HEAD_ID = MH.MAIN_HEAD_ID
                                WHERE
                                    MH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FREQUENCY_ID = ?FREQUENCY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMainHeadsByFrequencyIdForMessAndHostel:
                    {
                        query = @"SELECT 
                                    MH.FEE_MAIN_HEAD_ID,MH.MAIN_HEAD_ID,SMH.MAIN_HEAD
                                FROM
                                    FEE_MAIN_HEADS AS MH
                                        INNER JOIN
                                    SUP_FEE_MAIN_HEAD AS SMH ON SMH.MAIN_HEAD_ID = MH.MAIN_HEAD_ID
                                WHERE
                                    MH.ACADEMIC_YEAR = ?AC_YEAR
                                        AND FREQUENCY_ID = ?FREQUENCY_ID GROUP BY MH.MAIN_HEAD_ID";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupBankAccounts:
                    {
                        query = @"SELECT 
                                        BANK_ACCOUNT_ID,
                                        CONCAT(IFNULL(SBA.PASSBOOK_NO, ''),
                                                '(',
                                                IFNULL(SB.BANK_NAME, ''),
                                                ')') AS PASSBOOK_NO
                                    FROM
                                        sup_bank_account AS SBA
                                            INNER JOIN
                                        sup_bank AS SB ON SB.BANK_ID = SBA.BANK
                                    WHERE
                                        SB.IS_DELETED != 1 AND SBA.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterByClassId:
                    {
                        query = @"SELECT                                                      
                                          S.SUP_SEMESTER_ID,
                                          S.SEMESTER_NAME
                                      FROM
                                          ACADEMIC_SEMESTER AS ASE
                                              INNER JOIN
                                          CP_CLASSES AS CLS ON CLS.CLASS_ID = ?CLASS_ID
												INNER JOIN
											SUP_SEMESTER AS S ON S.SUP_SEMESTER_ID=ASE.SEMESTER
                                      WHERE
                                          ASE.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID AND ASE.ACADEMIC_YEAR=?AC_YEAR
                                              AND ASE.BATCH in (SELECT 
                                                  SC.BATCH
                                              FROM
                                                  STU_CLASS AS SC
                                              WHERE
                                                  SC.CLASS_ID = ?CLASS_ID
                                                      AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                              GROUP BY SC.BATCH)
                                              AND ASE.IS_ACTIVE = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByPaperType:
                    {

                        query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        CONCAT(IFNULL(COURSE_TITLE, ''),
                                                ' ( ',
                                                IFNULL(COURSE_CODE, ''),
                                                ' )') AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_PATTERN AS CCP
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
		                                    INNER JOIN
	                                    ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR=C.CLASS_YEAR
		                                    AND AB.ACADEMIC_YEAR_ID=(SELECT ACADEMIC_YEAR_ID FROM ACADEMIC_YEAR WHERE ACTIVE_YEAR=1 AND IS_DELETED!=1)
		                                    INNER JOIN
	                                    ACADEMIC_SEMESTER AS SEM ON SEM.SEMESTER=CCR.SEMESTER_ID
		                                    AND SEM.PROGRAMME_GROUP_ID=CCP.PROGRAMME_GROUP_ID
                                            AND SEM.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                            AND SEM.BATCH=AB.BATCH_ID
                                            AND SEM.IS_DELETED!=1
                                    WHERE
                                        PAPER_TYPE = ?PAPER_TYPE AND C.CLASS_ID = ?CLASS_ID  AND SEM.IS_ACTIVE=1
                                            AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                    GROUP BY COURSE_ROOT_ID;";

                        break;
                    }
                case SupportDataSQLCommands.FetchStaffByClassId:
                    {
                        query = @"SELECT 
                                    SP.STAFF_ID,
                                    UPPER(CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',IFNULL(SP.LAST_NAME,''),
                                            ' ( ',
                                            IFNULL(SP.STAFF_CODE, ''),' )')) AS 'FIRST_NAME'
                                FROM
                                    TT_TIMETABLE AS TT
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = TT.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                WHERE
                                    TT.CLASS_ID = ?CLASS_ID
                                        AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                        AND TT.IS_DELETED!=1
                                GROUP BY SP.STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCiaComponent:
                    {
                        query = @"SELECT 
                                        COMPONENT_ID, 
                                        COMPONENT
                                    FROM
                                        SUP_CIA_COMPONENTS WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByStudentId:
                    {
                        query = @"SELECT 
                                        CLASS_ID
                                    FROM
                                        STU_CLASS
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeeFrequency:
                    {
                        query = @"SELECT 
                                        FEE_FREQUENCY_FEE_MODE_ID AS FREQUENCY_ID,
                                        CONCAT(FREQUENCY_NAME,' (',SFT.FREQUENCY_TYPE,')') AS FREQUENCY_NAME,
                                        SFT.FREQUENCY_TYPE
                                    FROM
                                        FEE_FREQUENCY_FEE_MODE AS FFF
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                            INNER JOIN
                                        SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    WHERE
                                         SFF.IS_DELETED != 1 AND FFF.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupHeadByFrequencyAndProgrammeMode:
                    {
                        query = @"SELECT 
                                        FMH.MAIN_HEAD_ID, SH.HEAD AS MAIN_HEAD,SH.HEAD,SH.HEAD_ID
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                    WHERE
                                        FMH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FMH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SH.IS_DELETED!=1
                                    GROUP BY FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupHeadByFrequencyAndFeeRoot:
                    {
                        query = @"SELECT 
                                FMH.FEE_MAIN_HEAD_ID,
                                FMH.MAIN_HEAD_ID,
                                MAIN_HEAD,
                                SH.HEAD,
                                SH.HEAD_ID
                            FROM
                                FEE_MAIN_HEADS AS FMH
                                    INNER JOIN
                                SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                    AND MH.IS_DELETED != 1
                                    INNER JOIN
                                SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                    AND SH.IS_DELETED != 1
                            WHERE
                                FMH.FREQUENCY_ID = ?FREQUENCY_ID AND FEE_ROOT_ID = ?FEE_ROOT_ID
                                    AND FMH.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND FMH.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHeadByFrequencyAndProgrammeMode:
                    {
                        query = @"SELECT 
                                        FMH.MAIN_HEAD_ID, SH.HEAD AS MAIN_HEAD,SH.HEAD,SH.HEAD_ID
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            INNER JOIN
                                        SUP_HEAD AS SH ON SH.HEAD_ID = FMH.HEAD_ID
                                    WHERE
                                        FMH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FMH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SH.IS_DELETED!=1
                                    GROUP BY SH.HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupHeadsById:
                    {
                        query = @"SELECT 
                                        HEAD_ID, HEAD
                                    FROM
                                        SUP_HEAD
                                    WHERE
                                        HEAD_ID IN (?HEAD_ID) AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupMainHeadById:
                    {
                        query = @"SELECT 
                                        MAIN_HEAD_ID, MAIN_HEAD
                                    FROM
                                        SUP_FEE_MAIN_HEAD
                                    WHERE
                                        MAIN_HEAD_ID IN (?MAIN_HEAD_ID) AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDatesByDates:
                    {
                        query = @"SELECT 
                                        date_format(DAY_ORDER_DATE,'%Y/%m/%d') as DAY_ORDER_DATE
                                    FROM
                                        CALENDER
                                    WHERE
                                        DAY_ORDER_DATE BETWEEN ?DATE_FROM AND ?DATE_TO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentIds:
                    {
                        query = @"SELECT 
                                        FT.STUDENT_ID,
                                        DATE_FORMAT(PAYMENT_DATE, '%Y/%m/%d') AS TRANSACTION_DATE,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                IFNULL(LAST_NAME, '')) AS FIRST_NAME,
                                        CONCAT(CP.PROGRAMME_NAME,
                                                ' ',
                                                SH.DESCRIPTION,
                                                '-',
                                                SPM.PROGRAMME_MODE) AS PROGRAMME_NAME
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                    WHERE
	                                    PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
		                                    AND FREQUENCY = ?FREQUENCY AND CLS.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND CLS.CLASS_LEVEL=?CLASS_LEVEL
                                            AND CLS.CLASS_YEAR=?CLASS_YEAR
                                    GROUP BY FT.PAYMENT_DATE , FT.STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupMainHeadByFrequencyAndProgrammeMode:
                    {
                        query = @"SELECT 
                                        FMH.MAIN_HEAD_ID, SH.MAIN_HEAD
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS SH ON SH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                    WHERE
                                        FMH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND FMH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND FMH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FMH.SHIFT = ?SHIFT
                                            AND SH.IS_DELETED != 1
                                    GROUP BY FMH.MAIN_HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupIds:
                    {
                        query = @"SELECT 
										SC.PROGRAMME_GROUP_ID,
                                        DATE_FORMAT(PAYMENT_DATE, '%Y/%m/%d') AS TRANSACTION_DATE,
                                        CONCAT(CP.PROGRAMME_NAME,
                                                ' ',
                                                SH.DESCRIPTION,
                                                '-',
                                                SPM.PROGRAMME_MODE) AS PROGRAMME_NAME,
                                        COUNT(DISTINCT FT.STUDENT_ID) AS NO_OF_STUDENTS
                                    FROM
                                        FEE_TRANSACTION AS FT
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID AND SC.ACADEMIC_YEAR=?AC_YEAR
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                            INNER JOIN
                                        SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                    WHERE
	                                    PAYMENT_DATE BETWEEN ?DATE_FROM AND ?DATE_TO
		                                    AND FREQUENCY = ?FREQUENCY AND CLS.PROGRAMME_MODE = ?PROGRAMME_MODE AND CLS.SHIFT=?SHIFT
                                            AND CLS.CLASS_LEVEL=?CLASS_LEVEL AND CLS.CLASS_YEAR=?CLASS_YEAR
                                    GROUP BY FT.PAYMENT_DATE,SC.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterResultMonth:
                    {
                        query = @"SELECT EXAM_SETTING_ID,M_EXAM FROM EXAM_SETTING  WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterResultMonthByAcademicyear:
                    {
                        query = @"SELECT EXAM_SETTING_ID,M_EXAM FROM EXAM_SETTING  WHERE ACADEMIC_YEAR=?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.CheckIsNMEClass:
                    {
                        query = @"SELECT 
                                        COURSE_ID,
                                        CONCAT(IFNULL(CR.COURSE_TITLE, ''),
                                                ' - ',
                                                IFNULL(COURSE_CODE, '')) AS 'COURSE'
                                    FROM
                                        CP_CLASSES AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                    WHERE
                                        CC.CLASS_ID = ?CLASS_ID AND CC.IS_CHOICE = 1
                                            AND CC.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByClassId:
                    {
                        query = @"SELECT 
                                    CR.COURSE_ROOT_ID,
                                    CONCAT(IFNULL(UPPER(CR.COURSE_TITLE), ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, ''),
                                            ' - ',
                                            CR.SEMESTER_ID) AS 'COURSE_TITLE'
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CC
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                        INNER JOIN
									SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CC.SEMESTER_TYPE_ID
										AND ST.IS_ACTIVE=1
                                WHERE
                                    CC.CLASS_ID = ?CLASS_ID
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CC.IS_DELETED != 1
                                GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByClassIdandStaffId:
                    {
                        query = @"SELECT 
                                    CR.COURSE_ROOT_ID,
                                    CONCAT(IFNULL(UPPER(CR.COURSE_TITLE), ''),
                                            ' ',
                                            IFNULL(CR.COURSE_CODE, ''),
                                            ' - ',
                                            CR.SEMESTER_ID) AS 'COURSE_TITLE'
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CC
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                        INNER JOIN
									SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CC.SEMESTER_TYPE_ID
										AND ST.IS_ACTIVE=1
                                WHERE
                                    CC.CLASS_ID = ?CLASS_ID
                                    AND CC.STAFF_ID = ?STAFF_ID
                                        AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CC.IS_DELETED != 1
                                GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchValueEducation:
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
                                WHERE
                                    CCP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CCP.IS_DELETED != 1
                                        AND CCR.UGC_COURSE_TYPE=48
                                        AND C.CLASS_ID = ?CLASS_ID GROUP BY CCP.COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupRole:
                    {
                        query = @"SELECT ROLE_ID,ROLE_NAME FROM SUP_ROLE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTitle:
                    {
                        query = @"SELECT NOTICEBOARD_ID, NOTICEBOARD
                                    FROM DB_NOTICEBOARD
                                    WHERE (FIND_IN_SET(?ROLE_ID, ROLE_ID) > 0 OR ?ROLE_ID IS NULL);";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupUserType:
                    {
                        query = @"SELECT USER_TYPE_ID, USER_TYPE_NAME FROM SUP_USER_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchUsers:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                '(',
                                                ROLL_NO,
                                                ')') AS FIRST_NAME
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE
                                        SC.IS_DELETED != 1
                                            AND SP.IS_DELETED != 1 
                                    UNION SELECT 
                                        STAFF_ID AS STUDENT_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '',
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                '(',
                                                STAFF_CODE,
                                                ')') AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentIdsForQueryConstruction:
                    {
                        query = @"SELECT STUDENT_ID,FREQUENCY_ID AS TEMP_ID FROM FEE_STUDENT_ACCOUNT WHERE FREQUENCY_ID=6 AND DEBIT IS NULL;";
                        break;
                    }
                case SupportDataSQLCommands.FetchShiftByClassId:
                    {
                        query = @"SELECT SHIFT FROM CP_CLASSES WHERE CLASS_ID=?CLASS_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByClassIncharge:
                    {
                        query = @"SELECT 
                                    CL.CLASS_ID,
                                    CONCAT(IFNULL(C.CLASS_NAME, ''),
                                            ' (SHIFT ',
                                            IFNULL(SS.SHIFT_NAME, ''),
                                            ')') AS CLASS_NAME
                                FROM
                                    CLASS_INCHARGE AS CL
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = CL.STAFF_ID
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.CLASS_ID = CL.CLASS_ID
                                        INNER JOIN
                                    SUP_SHIFT AS SS ON SS.SHIFT_ID = C.SHIFT
                                WHERE
                                    CL.STAFF_ID = ?STAFF_ID
                                        AND CL.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }

                case SupportDataSQLCommands.FetchShiftAndProgrameGroupByClassId:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        PROGRAMME_GROUP_ID, 
                                        DEPARTMENT, 
                                        SHIFT,
                                        PROGRAMME_MODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        CLASS_ID = ?CLASS_ID AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFirstYearStudentToInsertImage:
                    {
                        query = @"SELECT 
	                                    SP.STUDENT_ID,
                                        C.CLASS_NAME,
                                        D.DEPARTMENT,
                                        SP.ADMISSION_NO,
                                        SP.ROLL_NO
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = C.DEPARTMENT
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SP.RECEIVE_ID != ''
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1;";
                        break;
                    }
                case SupportDataSQLCommands.UnSafeUpdateMode:
                    {
                        query = @"SET SQL_SAFE_UPDATES=0;";
                        break;
                    }
                case SupportDataSQLCommands.SafeUpdateMode:
                    {
                        query = @"SET SQL_SAFE_UPDATES=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentIdByClassIdAndCourseId:
                    {
                        query = @"SELECT 
                                            SP.STUDENT_ID,
                                            CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                            SP.ROLL_NO
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                                INNER JOIN
                                            CP_COURSE_PATTERN AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
		                                        AND PG.ACADEMIC_YEAR=SC.ACADEMIC_YEAR
                                                AND PG.COURSE_ID=?COURSE_ID
                                                AND PG.IS_DELETED!=1
		                                        INNER JOIN
	                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=PG.COURSE_ID
		                                        AND CCR.IS_COMPULSORY=1
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                AND SP.IS_DELETED != 1
                                                AND SP.IS_LEFT != 1
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR AND SC.CLASS_ID=?CLASS_ID
                                                AND SC.IS_DELETED != 1
                                        UNION
                                        SELECT 
                                            SP.STUDENT_ID,
                                            CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                            SP.ROLL_NO
                                        FROM
                                            STU_COURSE_INFO AS SC
                                                INNER JOIN
                                            STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                                AND SP.IS_DELETED != 1
                                                AND SP.IS_LEFT != 1
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.S_CLASS_ID = ?CLASS_ID
                                                AND SC.COURSE_ID = ?COURSE_ID
                                                AND SC.IS_DELETED != 1;";
                        //query = @"SELECT 
                        //                SC.STUDENT_ID,
                        //                SP.ROLL_NO,
                        //                SP.REGISTER_NO,
                        //                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                        ' ',
                        //                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                        //            FROM
                        //                STU_COURSE_INFO AS SC
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                    AND SP.IS_DELETED != 1
                        //                    AND SP.IS_LEFT != 1
                        //            WHERE
                        //                SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.CLASS_ID = ?CLASS_ID
                        //                    AND SC.COURSE_ID = ?COURSE_ID
                        //                    AND SC.IS_DELETED != 1 
                        //            UNION SELECT 
                        //                SC.STUDENT_ID,
                        //                SP.ROLL_NO,
                        //                SP.REGISTER_NO,
                        //                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                        ' ',
                        //                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                        //            FROM
                        //                STU_CLASS AS SC
                        //                    INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                        //                    AND SP.IS_DELETED != 1
                        //                    AND SP.IS_LEFT != 1
                        //                    INNER JOIN
                        //                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                        //                    INNER JOIN
                        //                CP_COURSE_PATTERN AS CP ON CP.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                        //                    AND CP.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                    AND CP.COURSE_ID = ?COURSE_ID
                        //                    AND CP.IS_DELETED != 1
                        //            WHERE
                        //                SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.CLASS_ID = ?CLASS_ID
                        //                    AND SC.IS_DELETED != 1;";
                        //query = @"SELECT 
                        //                SC.STUDENT_ID
                        //            FROM
                        //                STU_CLASS AS SC
                        //                    INNER JOIN
                        //                CP_COURSE_PATTERN AS CCP ON CCP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                        //                    AND CCP.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND CCP.IS_DELETED != 1
                        //                    AND CCP.IS_OPTIONAL != 1
                        //            WHERE
                        //                SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.CLASS_ID = ?CLASS_ID
                        //                    AND CCP.COURSE_ID = ?COURSE_ID
                        //                    AND SC.IS_DELETED != 1
                        //            GROUP BY SC.STUDENT_ID 
                        //            UNION SELECT 
                        //                SC.STUDENT_ID
                        //            FROM
                        //                STU_COURSE_INFO AS SC
                        //            WHERE
                        //                SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.CLASS_ID = ?CLASS_ID
                        //                    AND SC.COURSE_ID = ?COURSE_ID
                        //                    AND SC.IS_DELETED != 1
                        //            GROUP BY SC.STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchS_ClassIdByClassId:
                    {
                        query = @"SELECT 
                                            S_CLASS_ID AS CLASS_ID
                                        FROM
                                            STU_CLASS AS SC
                                                INNER JOIN
                                            STU_COURSE_INFO AS SCI ON SCI.STUDENT_ID = SC.STUDENT_ID
                                                AND SCI.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                INNER JOIN
	                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SP.STUDENT_ID
		                                        AND SP.IS_DELETED!=1 AND SP.IS_LEFT!=1
                                                INNER JOIN
                                            ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                                AND SEM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                                AND SEM.BATCH = SC.BATCH
                                                AND SEM.SEMESTER = SCI.SEMESTER
                                        WHERE
                                            SC.ACADEMIC_YEAR = ?AC_YEAR
                                                AND SC.CLASS_ID = ?CLASS_ID
                                                AND SEM.IS_ACTIVE = 1
                                                AND SC.IS_DELETED != 1
                                                AND SCI.IS_DELETED != 1
                                        GROUP BY SCI.S_CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByClassId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.ROLL_NO, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',IFNULL(SP.LAST_NAME, '')) AS LAST_NAME,
                                    PHOTO,
                                    D.DEPARTMENT,
                                    C.CLASS_YEAR,
                                    C.CLASS_LEVEL AS PROGRAMME_LEVEL
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
										INNER JOIN
									CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=C.DEPARTMENT
                                WHERE
                                        SC.CLASS_ID =?CLASS_ID
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1 ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCIABlockList:
                    {
                        query = @"SELECT 
                                        CIA_BLOCK_ID,
                                        STAFF_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                        PROGRAMME_GROUP_ID,
                                        PROGRAMME_LEVEL,
                                        SHIFT_ID,
                                        S_STAFF_ID,
                                        S_CLASS_ID,
                                        S_COURSE_ID,
                                        DATE_FROM,
                                        DATE_TO,
                                        IS_ACTIVE
                                    FROM
                                        CIA_BLOCK_LIST
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR AND IS_ACTIVE = 1
                                            AND IS_DELETED != 1 AND DATE_FROM<=CURDATE() AND CURDATE()<= DATE_TO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByDepartmentIds:
                    {
                        query = @"SELECT 
                                    CLASS_ID,
                                    CLASS_NAME
                                FROM
                                    CP_CLASSES
                                WHERE
                                    DEPARTMENT IN (?DEPARTMENT) AND COURSE_ID=0 AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentForHOD:
                    {
                        query = @"SELECT 
                                   DEPARTMENT_ID,
                                   DP.DEPARTMENT
                                FROM
                                    STF_DESIGNATION AS STFD
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS ST ON ST.STAFF_ID = STFD.STAFF_ID
                                        INNER JOIN
                                    SUP_DESIGNATION AS SD ON SD.DESIGNATIONID=STFD.DESIGNATION
										LEFT JOIN
                                    CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = ST.DEPARTMENT                                    									
                                WHERE
                                    STFD.STAFF_ID = ?STAFF_ID 
                                        AND STFD.ACADEMIC_YEAR =?AC_YEAR ;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByDepartmenIdShifId:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        DEPARTMENT = ?DEPARTMENT 
		                                    AND SHIFT IN (?SHIFT)
                                            AND IS_DELETED != 1  and CLASS_TYPE=2
                                    ORDER BY CLASS_LEVEL ,SHIFT, CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByDepartmenIdShifIdMuti:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        DEPARTMENT in(?DEPARTMENT) 
		                                    AND SHIFT IN (?SHIFT)
		                                    AND CLASS_YEAR IN (?CLASS_YEAR)
		                                    AND CLASS_LEVEL IN (?CLASS_LEVEL)
                                            AND IS_DELETED != 1 and CLASS_TYPE=2
                                    ORDER BY CLASS_LEVEL ,SHIFT, CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByDepartmenIdShifIdMutiAttEntry:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        DEPARTMENT in(?DEPARTMENT) 
		                                    AND SHIFT IN (?SHIFT)
                                            AND IS_DELETED != 1 and CLASS_TYPE=2
                                    ORDER BY CLASS_LEVEL ,SHIFT, CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffByClassIdAndCourseId:
                    {
                        query = @"SELECT 
                                    SP.STAFF_ID,
                                    CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,''), 
                                        ' - ',IFNULL(CONCAT('( ',SP.STAFF_CODE,' )'),'')) AS FIRST_NAME
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CC
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CC.STAFF_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CC.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE=1
                                WHERE CC.CLASS_ID=?CLASS_ID 
                                    AND CC.COURSE_ID=?COURSE_ID 
                                    AND CC.ACADEMIC_YEAR=?AC_YEAR 
                                    AND CC.IS_DELETED!=1
                                ORDER BY SP.DEPARTMENT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffByGroupCourse:
                    {
                        query = @"SELECT 
                                SP.STAFF_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, ''),
                                        ' - ',
                                        IFNULL(CONCAT('( ', SP.STAFF_CODE, ' )'), '')) AS FIRST_NAME
                            FROM
                                CP_CLASS_COURSE_STAFF AS CC
                                    INNER JOIN
                                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CC.STAFF_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CC.SEMESTER_TYPE_ID
                                    AND ST.IS_ACTIVE = 1
                            WHERE
                                CC.COURSE_ID = ?COURSE_ID
                                    AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND CC.IS_DELETED != 1
                            GROUP BY CC.COURSE_ID , CC.STAFF_ID
                            ORDER BY SP.DEPARTMENT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupActivityLevel:
                    {
                        query = @"SELECT 
                                        ACTIVITY_LEVEL_ID, ACTIVITY_LEVEL
                                    FROM
                                        SUP_ACTIVITY_LEVEL
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupCourseWiseHead:
                    {
                        query = @"SELECT 
                                    COURSE_HEAD_ID,
                                    CONCAT(IFNULL(HEAD,''),' - ',IFNULL(AMOUNT,''))AS HEAD, 
                                    AMOUNT, 
                                    IS_USED, 
                                    IS_DELETED
                                FROM
                                    SUP_COURSE_WISE_HEAD where IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByAcademicYear:
                    {
                        //query = @"SELECT 
                        //                SP.STUDENT_ID,
                        //                CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                        //               -- CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,''),' - ',SP.ROLL_NO) AS LAST_NAME,
                        //                IFNULL(SP.ROLL_NO,'0') AS ROLL_NO,
                        //                IFNULL(SP.REGISTER_NO,'0')AS REGISTER_NO,
                        //                IFNULL(SP.ADMISSION_NO,'0') AS ADMISSION_NO,
                        //                D.DEPARTMENT,
                        //                C.CLASS_NAME
                        //            FROM 
                        //                STU_CLASS AS SC
                        //                   INNER JOIN
                        //                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
                        //                   AND SP.IS_DELETED!=1
                        //                   AND SP.IS_LEFT!=1
                        //                   INNER JOIN
                        //                CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                        //                  INNER JOIN
                        //                CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=C.DEPARTMENT
                        //                WHERE
                        //                    SC.ACADEMIC_YEAR = ?AC_YEAR AND SC.IS_DELETED!=1;";

                        query = @"SELECT 
                                        TEXT1 AS REGISTER_NO,
                                        TEXT2 AS ROLL_NO
                                    FROM
                                        DEMO_TEST_TABLE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSet:
                    {
                        query = @"SELECT 
                                    SET_ID, 
                                    SET_NAME
                                FROM
                                    SUP_SETS
                                WHERE
                                    IS_ACTIVE = 1 
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDayOrderInfoByDate:
                    {
                        query = @"SELECT 
                                        DAY_ORDER
                                    FROM
                                        CALENDER AS CAL
                                    WHERE
                                        CAL.ACADEMIC_YEAR = ?AC_YEAR
                                            AND DAY_ORDER_DATE = ?DAY_ORDER_DATE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByStaffId:
                    {
                        query = @"SELECT 
	                                COURSE_ROOT_ID,
                                    UPPER(CONCAT(IFNULL(COURSE_TITLE,''),' - ',IFNULL(COURSE_CODE,''))) AS COURSE_CODE
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CCS
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE=1
                                WHERE
                                    CCS.STAFF_ID = ?STAFF_ID
                                        AND CCS.IS_DELETED != 1
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeByClassYearId:
                    {
                        query = @"SELECT 
                                    PG.PROGRAMME_GROUP_ID,
                                    P.PROGRAMME_ID,
                                    CONCAT(IFNULL(P.PROGRAMME_NAME,''),' ( ',IFNULL(PM.PROGRAMME_MODE,''),' - SHIFT ',IFNULL(S.SHIFT_NAME,''),')') AS PROGRAMME_NAME
                                FROM
                                    CP_PROGRAMME_GROUP AS PG
                                        INNER JOIN
                                    CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                        INNER JOIN
                                    CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = PG.PROGRAMME_GROUP_ID
		                                INNER JOIN
	                                SUP_SHIFT AS S ON S.SHIFT_ID=PG.SHIFT
		                                INNER JOIN
	                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID=PG.PROGRAMME_MODE 
                                WHERE
                                    C.CLASS_YEAR = ?CLASS_YEAR AND PG.IS_DELETED!=1 AND P.IS_DELETED!=1  GROUP BY PG.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMonthFromCalender:
                    {
                        query = @"SELECT 
                                        DAY_ID,
                                        DAY_ORDER_MONTH
                                    FROM
                                        CALENDER
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR GROUP BY DAY_ORDER_MONTH ORDER BY DAY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMonthFromCalenderByCurrentYear:
                    {
                        query = @"SELECT 
                                        DAY_ID,
                                        DAY_ORDER_MONTH
                                    FROM
                                        CALENDER
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR
                                            AND DATE_FORMAT(DAY_ORDER_DATE, '%Y') = DATE_FORMAT(CURDATE(), '%Y')
                                    GROUP BY DAY_ORDER_MONTH
                                    ORDER BY DAY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoForReport:
                    {
                        query = @"SELECT 
                                        C.CLASS_NAME,
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SP.ADMISSION_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SC.IS_DELETED != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SP.STUDENT_ID = ?STUDENT_ID ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByClassIdAndAcYear:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,''))) AS FIRST_NAME,
                                        C.CLASS_NAME
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
                                        AND SC.CLASS_ID = ?CLASS_ID
                                            AND SC.IS_DELETED != 1 
                                    UNION SELECT 
                                        SP.STUDENT_ID,
                                        SP.REGISTER_NO,
                                        SP.ROLL_NO,
                                        UPPER(CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,''))) AS FIRST_NAME,
                                        C.CLASS_NAME
                                    FROM
                                        STU_COURSE_INFO AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.S_CLASS_ID 
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SC.S_CLASS_ID = ?CLASS_ID
                                        AND SC.COURSE_ID = ?COURSE_ID
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByStaffIdAndClassId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(CR.COURSE_CODE,' - ',CR.COURSE_TITLE)) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CC.CLASS_ID
                                            INNER JOIN
                                        SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CC.SEMESTER_TYPE_ID
                                            AND ST.IS_ACTIVE = 1
                                    WHERE
                                        CC.CLASS_ID = ?CLASS_ID AND CC.STAFF_ID = ?STAFF_ID
                                            AND CC.IS_DELETED != 1
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeByProgrammelevelAndAcYear:
                    {
                        query = @"SELECT DISTINCT
                                            CPG.PROGRAMME_GROUP_ID, CP.PROGRAMME_NAME, CP.PROGRAMME_DESCRIPTION
                                        FROM
                                            CP_PROGRAMME AS CP
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                                AND CPG.IS_DELETED != 1
                                        WHERE
                                            CPG.APPTYPE_ID = ?PROGRAMME_LEVEL_ID AND CPG.SHIFT = ?SHIFT_ID
                                                AND CP.IS_DELETED != 1
                                        ORDER BY CP.PROGRAMME_NAME;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeByProgrammelevelAndAcYearMulti:
                    {
                        query = @"SELECT DISTINCT
                                            CPG.PROGRAMME_GROUP_ID, CP.PROGRAMME_NAME, CP.PROGRAMME_DESCRIPTION
                                        FROM
                                            CP_PROGRAMME AS CP
                                                INNER JOIN
                                            CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_ID = CP.PROGRAMME_ID
                                                AND CPG.IS_DELETED != 1
                                        WHERE
                                            CPG.APPTYPE_ID IN (?PROGRAMME_LEVEL_ID) AND CPG.SHIFT in (?SHIFT_ID)
                                                AND CP.IS_DELETED != 1
                                        ORDER BY CP.PROGRAMME_NAME;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeByProgrammelevelAndAcYearandStaffId:
                    {
                        query = @"SELECT 
                                        CPG.PROGRAMME_GROUP_ID,
                                        CONCAT(CP.PROGRAMME_DESCRIPTION,
                                                ' - ',
                                                SPP.PROGRAMME_MODE,
                                                ' - ',
                                                SS.DESCRIPTION) AS PROGRAMME_DESCRIPTION
                                    FROM
                                        CP_PROGRAMME_GROUP AS CPG
                                            INNER JOIN
                                        CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPP ON SPP.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.DEPARTMENT = CP.DEPARTMENT
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                            AND CPG.IS_DELETED != 1
                                            AND SS.SHIFT_ID in (?SHIFT_ID)
                                            AND CPG.APPTYPE_ID in (?PROGRAMME_LEVEL_ID)
                                            AND SP.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchActiveCertificateSettingIdByActiveYear:
                    {
                        query = @"SELECT 
                                    SETTING_ID, 
                                    SETTING_NAME, 
                                    ACADEMIC_YEAR, 
                                    IS_ACTIVE, 
                                    IS_DELETED
                                FROM
                                    CERTIFICATE_COURSE_SETTING
                                WHERE
                                    IS_ACTIVE = 1 AND ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                C.CLASS_ID,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                PG.PROGRAMME_GROUP_ID,
                                P.PROGRAMME_NAME,
                                UPPER(C.CLASS_NAME) AS CLASS,
                                DATE_FORMAT(T.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%d/%m/%Y') AS SETTLEMENT_DATE
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = C.PROGRAMME_GROUP_ID
                                    AND PG.IS_DELETED != 1
                                    INNER JOIN
                                CP_PROGRAMME AS P ON P.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN
                                FEE_TRANSACTION AS T ON T.STUDENT_ID = SC.STUDENT_ID
                                    AND T.FREQUENCY = ?FREQUENCY
                                    INNER JOIN
                                FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = T.TRANSACTION_ID
                                    AND FC.SETTLEMENT_DATE BETWEEN UNIX_TIMESTAMP(?DATE_FROM) AND UNIX_TIMESTAMP(ADDDATE(?DATE_TO, 1))
                            WHERE
                                SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                    AND C.CLASS_YEAR = ?CLASS_YEAR
                                    AND SC.IS_DELETED != 1
                            GROUP BY T.STUDENT_ID
                            ORDER BY T.PAYMENT_DATE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByProgrammeGroupIdForDateReport:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                CL.CLASS_ID,
                                UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                PG.PROGRAMME_GROUP_ID,
                               UPPER(CONCAT((CP.PROGRAMME_NAME),
                                        ' (SHIFT - ',
                                        (SS.SHIFT_NAME),
                                        ') ',
                                        (PM.PROGRAMME_MODE))) AS PROGRAMME_NAME,
                                UPPER(CL.CLASS_NAME) AS CLASS_NAME,
                                DATE_FORMAT(FT.PAYMENT_DATE, '%d/%m/%Y') AS PAYMENT_DATE,
                                FROM_UNIXTIME(FC.SETTLEMENT_DATE, '%d/%m/%Y') AS SETTLEMENT_DATE
                            FROM
                                FEE_TRANSACTION AS FT
                                    INNER JOIN
                                FEE_COLLECTION AS FC ON FC.TRANSACTION_ID = FT.TRANSACTION_ID
                                    INNER JOIN
                                FEE_MAIN_HEADS AS FM ON FM.FEE_MAIN_HEAD_ID = FC.FEE_MAIN_HEAD_ID
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = FT.STUDENT_ID
                                    AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = PG.PROGRAMME_ID
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = PG.PROGRAMME_MODE
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = PG.SHIFT
                            WHERE
                                FT.FREQUENCY = ?FREQUENCY
                                    AND FC.SETTLEMENT_DATE BETWEEN UNIX_TIMESTAMP(?DATE_FROM) AND UNIX_TIMESTAMP(ADDDATE(?DATE_TO, 1))
                                    AND FT.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                            GROUP BY FT.STUDENT_ID
                            ORDER BY FC.SETTLEMENT_DATE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupId:
                    {
                        query = @"SELECT 
	                                    MH.MAIN_HEAD_ID,
                                        MH.MAIN_HEAD,
                                        H.HEAD_ID,
                                        H.HEAD
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            INNER JOIN
                                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
		                                    INNER JOIN
	                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_MODE=FMH.PROGRAMME_MODE
                                            AND PG.SHIFT=FMH.SHIFT
                                            AND PG.APPTYPE_ID=FMH.APPLICATION_TYPE_ID
                                            AND PG.IS_DELETED!=1
                                    WHERE
                                        FEE_ROOT_ID = ?FEE_ROOT_ID
                                        AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                                        AND FREQUENCY_ID = ?FREQUENCY AND FMH.IS_DELETED!=1 
                                        GROUP BY MH.MAIN_HEAD_ID,H.HEAD_ID ORDER BY MH.MAIN_HEAD_ID";
                        break;
                    }
                case SupportDataSQLCommands.FetchHeadsByFeeRootIdAndProgrammeGroupIdAndFeeCategory:
                    {
                        query = @"SELECT 
	                                    MH.MAIN_HEAD_ID,
                                        MH.MAIN_HEAD,
                                        H.HEAD_ID,
                                        H.HEAD
                                    FROM
                                        FEE_MAIN_HEADS AS FMH
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS MH ON MH.MAIN_HEAD_ID = FMH.MAIN_HEAD_ID
                                            INNER JOIN
                                        SUP_HEAD AS H ON H.HEAD_ID = FMH.HEAD_ID
		                                    INNER JOIN
	                                    CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_MODE=FMH.PROGRAMME_MODE
                                            AND PG.SHIFT=FMH.SHIFT
                                            AND PG.APPTYPE_ID=FMH.APPLICATION_TYPE_ID
                                            AND PG.IS_DELETED!=1
                                    WHERE
                                        FEE_ROOT_ID = ?FEE_ROOT_ID
                                        AND PG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID) AND FEE_CATEGORY_ID IN (?FEE_CATEGORY_ID)                                       
                                        AND FREQUENCY_ID = ?FREQUENCY AND FMH.IS_DELETED!=1 
                                        GROUP BY MH.MAIN_HEAD_ID,H.HEAD_ID ORDER BY MH.MAIN_HEAD_ID";
                        break;
                    }
                case SupportDataSQLCommands.FectchFeeRoot:
                    {
                        query = @"SELECT FEE_ROOT_ID, FEE_ROOT FROM FEE_ROOT WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassCourseStaffByCourseId:
                    {
                        query = @"SELECT 
                                        CCS.CLSCRSSTF_ID,
                                        CCS.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                ' - ',
                                                IFNULL(SP.STAFF_CODE, '')) AS STAFF_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                                            AND CCS.COURSE_ID = ?COURSE_ID
                                            AND CCS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.IS_DELETED != 1
                                    UNION 
	                                    SELECT 
		                                    CCS.CLSCRSSTF_ID,
		                                    CCS.STAFF_ID,
		                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
				                                    ' ',
				                                    IFNULL(SP.LAST_NAME, ''),
				                                    ' - ',
				                                    IFNULL(SP.STAFF_CODE, '')) AS STAFF_NAME
		                                    FROM STU_COURSE_INFO AS SC
		                                    INNER JOIN
	                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.S_CLASS_ID
                                            AND CCS.COURSE_ID = SC.COURSE_ID
                                            AND CCS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                    WHERE 
		                                    SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.COURSE_ID = ?COURSE_ID
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.IS_DELETED != 1";
                        //query = @"SELECT 
                        //                CCS.CLSCRSSTF_ID,
                        //                CCS.STAFF_ID,
                        //                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                        //                        ' ',
                        //                        IFNULL(SP.LAST_NAME, ''),
                        //                        ' - ',
                        //                        IFNULL(SP.STAFF_CODE, '')) AS STAFF_NAME
                        //            FROM
                        //                STU_CLASS AS SC
                        //                    INNER JOIN
                        //                CP_CLASS_COURSE_STAFF AS CCS ON CCS.CLASS_ID = SC.CLASS_ID
                        //                    AND CCS.COURSE_ID = ?COURSE_ID
                        //                    AND CCS.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                        //                    AND CCS.IS_DELETED != 1
                        //                    INNER JOIN
                        //                STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CCS.STAFF_ID
                        //                    AND SP.IS_DELETED != 1
                        //                    AND SP.IS_LEFT != 1
                        //            WHERE
                        //                SC.ACADEMIC_YEAR = ?AC_YEAR
                        //                    AND SC.STUDENT_ID = ?STUDENT_ID
                        //                    AND SC.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStuReportFields:
                    {
                        query = @"SELECT FIELD_ID, FIELD FROM STU_REPORT_FIELDS WHERE IS_DELETED!=1 ORDER BY FEILD_ORDER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupByShiftAndProgrammeMode:
                    {
                        query = @"SELECT 
                                        AP.PROGRAMME_GROUP_ID,
                                        P.PROGRAMME_ID,                                       
                                        CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                ' (SHIFT - ',
                                                IFNULL(SH.SHIFT_NAME, ''),
                                                ') ',
                                                IFNULL(SPM.PROGRAMME_MODE, '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS AP
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = AP.PROGRAMME_ID
                                            AND P.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT SH ON SH.SHIFT_ID = AP.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = AP.PROGRAMME_MODE
                                    WHERE
                                        AP.IS_DELETED != 1 AND AP.PROGRAMME_MODE=?PROGRAMME_MODE AND SHIFT=?SHIFT ORDER BY APPTYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupByShiftAndProgrammeModeAndApplicationType:
                    {
                        query = @"SELECT 
                                        AP.PROGRAMME_GROUP_ID,
                                        P.PROGRAMME_ID,                                       
                                        CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                ' (SHIFT - ',
                                                IFNULL(SH.SHIFT_NAME, ''),
                                                ') ',
                                                IFNULL(SPM.PROGRAMME_MODE, '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS AP
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = AP.PROGRAMME_ID
                                            AND P.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT SH ON SH.SHIFT_ID = AP.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = AP.PROGRAMME_MODE
                                    WHERE
                                        AP.IS_DELETED != 1 AND AP.PROGRAMME_MODE=?PROGRAMME_MODE AND SHIFT=?SHIFT AND AP.APPTYPE_ID=?APPTYPE_ID ORDER BY AP.PROGRAMME_GROUP_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammesByShiftAndProgrammeMode:
                    {
                        query = @"SELECT 
                                        AP.PROGRAMME_GROUP_ID,
                                        P.PROGRAMME_ID,                                       
                                        CONCAT(IFNULL(P.PROGRAMME_NAME, ''),
                                                ' (SHIFT - ',
                                                IFNULL(SH.SHIFT_NAME, ''),
                                                ') ',
                                                IFNULL(SPM.PROGRAMME_MODE, '')) AS PROGRAMME_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS AP
                                            INNER JOIN
                                        CP_PROGRAMME AS P ON P.PROGRAMME_ID = AP.PROGRAMME_ID
                                            AND P.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_SHIFT SH ON SH.SHIFT_ID = AP.SHIFT
                                            INNER JOIN
                                        SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = AP.PROGRAMME_MODE
                                    WHERE
                                        AP.IS_DELETED != 1 AND AP.PROGRAMME_MODE IN (?PROGRAMME_MODE) AND SHIFT IN (?SHIFT)  AND AP.APPTYPE_ID=?APPTYPE_ID ORDER BY APPTYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHostelDDL:
                    {
                        query = @"SELECT 
                                        HOSTEL_ID, HOSTEL_NAME
                                    FROM
                                        SUP_HOSTEL WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchQuizQuestionTypeDDL:
                    {
                        query = @"SELECT 
                                        QUESTION_TYPE_ID,
                                        QUESTION_TYPE
                                    FROM
                                        SUP_QUIZ_QUESTION_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupRoomList:
                    {
                        query = @"SELECT 
                                ROOM_ID,BLOCK_NAME,BLOCK_CODE,R.ROOM_NO,`COLUMN`,`ROW`,EXAM_CAPACITY,ROOM_CAPACITY,R.EXAM_TYPE,ET.EXAM_TYPE_NAME,R.IS_ACTIVE
                            FROM
                                SUP_ROOM AS R
                                    INNER JOIN
                                SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = R.COLLEGE_BLOCK_ID
                                    AND CB.IS_DELETED != 1
                                    INNER JOIN
                                SUP_EXAM_TYPE AS ET ON ET.EXAM_TYPE = R.EXAM_TYPE
                            WHERE
                                R.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupRoomListByExamType:
                    {
                        query = @"SELECT 
                                    ROOM_ID,
                                    BLOCK_NAME,
                                    BLOCK_CODE,
                                    ROOM_NO,
                                    `COLUMN`,
                                    `ROW`,
                                    EXAM_CAPACITY,
                                    ROOM_ORDER,
                                    BLOCK_ORDER,
                                    SPACE_COLUMN,
                                    ROOM_CAPACITY,
                                    R.EXAM_TYPE,
                                    R.IS_ACTIVE
                                 FROM
                                     SUP_ROOM AS R
                                         INNER JOIN
                                     SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = R.COLLEGE_BLOCK_ID
                                         AND CB.IS_DELETED != 1
                                 WHERE
                                     R.IS_DELETED != 1 AND R.EXAM_TYPE =?EXAM_TYPE ORDER BY ROOM_ORDER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupCollgeBlock:
                    {
                        query = @"SELECT COLLEGE_BLOCK_ID, BLOCK_NAME, BLOCK_CODE FROM SUP_COLLEGE_BLOCK WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupCollgeBlockById:
                    {
                        query = @"SELECT COLLEGE_BLOCK_ID, BLOCK_NAME, BLOCK_CODE FROM SUP_COLLEGE_BLOCK WHERE COLLEGE_BLOCK_ID=?COLLEGE_BLOCK_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupRoom:
                    {
                        query = @"UPDATE SUP_ROOM SET IS_DELETED=1 WHERE ROOM_ID=?ROOM_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchRoomByRoomId:
                    {
                        query = @"SELECT 
                                ROOM_ID,
                                ROOM_NO,
                                R.COLLEGE_BLOCK_ID,
                                R.EXAM_TYPE,
                                `COLUMN`,
                                `ROW`,
                                ROOM_CAPACITY,
                                ROOM_ORDER,
                                SPACE_COLUMN,
                                EXAM_CAPACITY,
                                CB.BLOCK_CODE,
                                CB.BLOCK_NAME,
                                ET.EXAM_TYPE
                            FROM
                                SUP_ROOM AS R
                                    INNER JOIN
                                SUP_COLLEGE_BLOCK AS CB ON CB.COLLEGE_BLOCK_ID = R.COLLEGE_BLOCK_ID
                                    INNER JOIN
                                SUP_EXAM_TYPE AS ET ON ET.EXAM_TYPE = R.EXAM_TYPE
                            WHERE
                                R.ROOM_ID = ?ROOM_ID AND R.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.InsertSupRoom:
                    {
                        query = @"INSERT INTO `SUP_ROOM`
                                (
                                `ROOM_ID`,
                                `ROOM_NO`,
                                `COLLEGE_BLOCK_ID`,
                                 EXAM_TYPE,
                                `COLUMN`,
                                `ROW`,
                                `ROOM_ORDER`,
                                `ROOM_CAPACITY`,
                                `SPACE_COLUMN`,
                                `EXAM_CAPACITY`)
                                VALUES
                                (
                                ?ROOM_ID,
                                ?ROOM_NO,
                                ?COLLEGE_BLOCK_ID,
                                ?EXAM_TYPE,
                                ?COLUMN,
                                ?ROW,
                                ?ROOM_ORDER,
                                ?ROOM_CAPACITY,
                                ?SPACE_COLUMN,
                                ?EXAM_CAPACITY
                                );";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupRoom:
                    {
                        query = @"UPDATE `SUP_ROOM`
                                SET                              
                                `ROOM_NO` = ?ROOM_NO,
                                `COLLEGE_BLOCK_ID` = ?COLLEGE_BLOCK_ID,
                                 EXAM_TYPE = ?EXAM_TYPE,
                                `COLUMN` = ?COLUMN,
                                `ROW` = ?ROW,  
                                `ROOM_ORDER` = ?ROOM_ORDER,  
                                `SPACE_COLUMN` = ?SPACE_COLUMN,                              
                                `EXAM_CAPACITY` = ?EXAM_CAPACITY
                                WHERE `ROOM_ID` = ?ROOM_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByClassIdFromCoursePattern:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID, 
                                        CCR.COURSE_CODE, 
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID
                                    FROM
                                        CP_COURSE_PATTERN AS CP
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CP.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
                                        ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR = C.CLASS_YEAR
                                            AND AB.ACADEMIC_YEAR_ID = (SELECT 
                                                ACADEMIC_YEAR_ID
                                            FROM
                                                ACADEMIC_YEAR
                                            WHERE
                                                ACTIVE_YEAR = 1)
                                            INNER JOIN
                                        ACADEMIC_SEMESTER AS SEM ON SEM.BATCH = AB.BATCH_ID
                                            AND SEM.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                            AND SEM.SEMESTER = CCR.SEMESTER_ID
                                            AND SEM.IS_DELETED != 1
                                            AND SEM.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                    WHERE
                                        C.CLASS_ID = ?CLASS_ID AND CP.IS_DELETED != 1
                                            AND SEM.IS_ACTIVE = 1
                                            AND CP.ACADEMIC_YEAR = ?AC_YEAR 
                                            UNION SELECT 
                                                    CCR.COURSE_ROOT_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    CCR.SEMESTER_ID
                                                FROM
                                                    STU_COURSE_INFO AS SCI
                                                        INNER JOIN
                                                    CP_CLASSES AS CC ON CC.CLASS_ID = SCI.S_CLASS_ID
                                                        INNER JOIN
                                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = SCI.COURSE_ID
                                                        AND sci.SEMESTER = CCR.SEMESTER_ID
                                                WHERE
                                                    sci.S_CLASS_ID = ?CLASS_ID
                                                        AND sci.IS_DELETED != 1
                                                        AND sci.ACADEMIC_YEAR = ?AC_YEAR
                                                GROUP BY ccr.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSession:
                    {
                        query = @"SELECT 
                                        SESSION_ID, 
                                        SESSION_NAME, 
                                        DESCRIPTION
                                    FROM
                                        SUP_SESSION
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMainHeadsByFrequencyAndApplicationType:
                    {
                        query = @"SELECT 
                                        MH.FEE_MAIN_HEAD_ID,    
                                        CONCAT(IFNULL(SMH.MAIN_HEAD,''),' ( ',IFNULL(H.HEAD,''),' )') MAIN_HEAD
                                    FROM
                                        FEE_MAIN_HEADS AS MH
                                            INNER JOIN
                                        SUP_FEE_MAIN_HEAD AS SMH ON SMH.MAIN_HEAD_ID = MH.MAIN_HEAD_ID
                                            INNER JOIN
                                        SUP_HEAD AS H ON H.HEAD_ID = MH.HEAD_ID
                                    WHERE
                                        MH.ACADEMIC_YEAR = ?AC_YEAR
                                            AND MH.FREQUENCY_ID = ?FREQUENCY_ID
                                            AND MH.APPLICATION_TYPE_ID = ?APPLICATION_TYPE_ID
                                            AND MH.PROGRAMME_MODE = ?PROGRAMME_MODE
                                            AND MH.SHIFT = ?SHIFT
                                            AND MH.FEE_ROOT_ID = ?FEE_ROOT_ID
                                            AND MH.IS_DELETED!=1 ORDER BY FEE_MAIN_HEAD_ID;";
                        break;
                    }
                case SupportDataSQLCommands.IsTableExist:
                    {
                        query = @"SELECT 
                                     EXISTS( SELECT 
                                            *
                                        FROM
                                            INFORMATION_SCHEMA.TABLES
                                        WHERE
                                             TABLE_NAME = 'ACADEMIC_SEMESTER_?AC_YEAR') AS IS_EXIST;";
                        break;
                    }
                case SupportDataSQLCommands.IsTableExistForSemesterMarks:
                    {
                        query = @"SELECT IF(COUNT(ACADEMIC_YEAR)>1,'1','0') AS IS_EXIST FROM academic_semester WHERE ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchRegularClassesByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                CLS.CLASS_ID, CLS.CLASS_CODE, CLS.CLASS_NAME
                            FROM
                                CP_CLASSES AS CLS
                                    LEFT JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CLS.PROGRAMME_GROUP_ID
                                    AND CPG.PROGRAMME_MODE = CLS.PROGRAMME_MODE
                                    AND CPG.SHIFT = CLS.SHIFT
                                    LEFT JOIN
                                SUP_SHIFT AS SH ON SH.SHIFT_ID = CPG.SHIFT
                                    LEFT JOIN
                                SUP_PROGRAMME_MODE AS SPM ON SPM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                            WHERE
                                CLS.IS_DELETED != 1
                                    AND CLS.CLASS_TYPE = 2
                                    AND CPG.PROGRAMME_GROUP_ID IN (?PROGRAMME_GROUP_ID)
                            ORDER BY CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupStaffType:
                    {
                        query = @"SELECT 
                                    STF_CATEGORY_ID, STF_CATEGORY
                                FROM
                                    sup_staff_category
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupExamType:
                    {
                        query = @"SELECT EXAM_TYPE, EXAM_TYPE_NAME FROM SUP_EXAM_TYPE WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.CheckRoomAlreadyExist:
                    {
                        query = @"SELECT 
                                ROOM_ID,
                                ROOM_NO,
                                COLLEGE_BLOCK_ID,
                                EXAM_TYPE,
                                `COLUMN`,
                                `ROW`,
                                ROOM_CAPACITY,
                                EXAM_CAPACITY
                            FROM
                                SUP_ROOM
                            WHERE
                                ROOM_NO = ?ROOM_NO AND COLLEGE_BLOCK_ID = ?COLLEGE_BLOCK_ID
                                    AND EXAM_TYPE = ?EXAM_TYPE
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOddCourseStudent:
                    {
                        query = @"SELECT 
                                        CLS.STUDENT_ID,   
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CC.COURSE_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                                    FROM
                                        STU_CLASS AS CLS
                                            INNER JOIN
                                        CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                            AND CC.COURSE_ID = ?COURSE_ID
                                            AND CC.IS_DELETED != 1
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                    WHERE
                                        CLS.CLASS_ID = ?CLASS_ID
                                            AND CLS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CLS.IS_DELETED != 1 
                                    UNION SELECT 
                                        CLS.STUDENT_ID,    
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CLS.COURSE_ID
                                    FROM
                                        STU_COURSE_INFO AS CLS
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                    WHERE
                                        CLS.S_CLASS_ID = ?CLASS_ID
                                            AND CLS.COURSE_ID = ?COURSE_ID
                                            AND CLS.IS_DELETED != 1
                                            AND CLS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchEvenCourseStudent:
                    {
                        query = @"SELECT 
                                    CLS.STUDENT_ID,   
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CC.COURSE_ID,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME
                                FROM
                                    STU_CLASS AS CLS
                                        INNER JOIN
                                    CP_CLASS_COURSE AS CC ON CC.CLASS_ID = CLS.CLASS_ID
                                        AND CC.COURSE_ID = ?COURSE_ID
                                        AND CC.IS_DELETED != 1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    CLS.CLASS_ID = ?CLASS_ID
                                        AND CLS.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CLS.IS_DELETED != 1 
                                UNION SELECT 
                                    CLS.STUDENT_ID,    
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                    CLS.COURSE_ID
                                FROM
                                    STU_COURSE_INFO AS CLS
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = CLS.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                WHERE
                                    CLS.S_CLASS_ID = ?CLASS_ID
                                        AND CLS.COURSE_ID = ?COURSE_ID
                                        AND CLS.IS_DELETED != 1
                                        AND CLS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSeatingsList:
                    {
                        query = @"SELECT SEATING_ID, ROOM_ID, COLUMN_NO, ROW_NO, SEAT_CODE, SEAT_POSITION, EXAM_TYPE, IS_DELETED, IS_ACTIVE FROM EXAM_SEATINGS WHERE EXAM_TYPE=?EXAM_TYPE AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupCIAComponentbyCourseId:
                    {
                        query = @"SELECT 
                                COMPONENT_ID, COMPONENT
                            FROM
                                CP_COURSE_ROOT AS CR
                                    INNER JOIN
                                CP_COURSE_TYPE AS CT ON CT.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    INNER JOIN
                                CP_COURSE_TYPE_GROUP CG ON CG.COURSE_TYPE_ID = CR.COURSE_TYPE
                                    INNER JOIN
                                CIA_GROUP_COMPONENT AS CIA ON CIA_GROUP_ID = CG.GROUP_ID
                                    INNER JOIN
                                SUP_CIA_COMPONENTS AS SCIA ON SCIA.COMPONENT_ID = CIA.SUP_CIA_COMPONENT_ID
                                    AND COURSE_ROOT_ID = ?COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStuClassesByStudentId:
                    {
                        query = @"SELECT 
                                    STUDENT_ID, ACADEMIC_YEAR, CLASS_ID
                                FROM
                                    STU_CLASS
                                WHERE
                                    STUDENT_ID =?STUDENT_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDemoTestTable:
                    {
                        query = @"SELECT 
                                        TEST_TABLE_ID, 
                                        TEXT1, 
                                        TEXT2
                                    FROM
                                        DEMO_TEST_TABLE 
                                    ORDER BY TEST_TABLE_ID;";
                        break;
                    }

                //For chat

                case SupportDataSQLCommands.FetchSemMarkForChat:
                    {
                        query = @"  SELECT 
  
                                    CONCAT(IFNULL(ES.EXAM_NAME,''),
                                    ' ',
                                    IFNULL(ES.m_exam,''),'') AS LABEL,
                                    SUM(SM.TOTAL) / COUNT(SM.TOTAL) AS Y
  
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
		                                STU_PERSONAL_INFO AS PI ON PI.STUDENT_ID = SC.STUDENT_ID
                                        AND PI.IS_LEFT != 1
                                        AND PI.IS_DELETED != 1
                                        INNER JOIN
		                                ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                       --  AND AY.ACTIVE_YEAR = 1
                                        AND AY.IS_DELETED!=1
                                        INNER JOIN
		                                ACADEMIC_BATCHES AS B ON B.BATCH_ID = SC.BATCH
                                        AND B.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
		                                INNER JOIN
                                        SEMESTER_MARKS_?AC_YEAR AS SM ON SM.STUDENT_ID = SC.STUDENT_ID
                                        AND SM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = SM.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                        INNER JOIN
                                        ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR = AY.AC_YEAR
                                        AND S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND S.BATCH = B.BATCH_ID
                                        AND S.SEMESTER=CR.SEMESTER_ID
                                        AND S.IS_DELETED != 1
                                        INNER JOIN 
                                        EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID=SM.EXAM_SETTING_ID
                                        AND ES.IS_DELETED!=1
                                WHERE
                                    PI.STUDENT_ID =?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                GROUP BY PI.STUDENT_ID,SM.SEMESTER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTotalAbsentHoursForStudent:
                    {
                        query = @"SELECT 
                                   CONCAT(IFNULL(SS.SEMESTER_NAME,''),' ','SEMESTER') AS LABEL,
                                    SUM(IF(AL.HOUR_FROM!=0,1,0)) AS ABSENT_HOURS,
                                    SS.SUP_SEMESTER_ID
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS PI ON PI.STUDENT_ID = SC.STUDENT_ID
                                        AND PI.IS_LEFT != 1
                                        AND PI.IS_DELETED != 1
                                        INNER JOIN 
									CP_CLASSES AS CC ON CC.CLASS_ID=SC.CLASS_ID
                                        AND CC.IS_DELETED!=1
                                        INNER JOIN
                                    ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                    --   AND AY.ACTIVE_YEAR = 1
                                        INNER JOIN
                                    ACADEMIC_BATCHES AS B ON B.BATCH_YEAR =CC.CLASS_YEAR
                                        AND B.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID
                                        INNER JOIN
                                    ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR = AY.AC_YEAR
                                        AND S.PROGRAMME_GROUP_ID = CC.PROGRAMME_GROUP_ID
                                        AND S.BATCH = B.BATCH_ID
                                     --   AND S.IS_ACTIVE=1
                                        AND S.IS_DELETED!=1
                                        
                                        AND S.IS_DELETED != 1
                                        INNER JOIN
                                    ATT_ABSENTEE_LIST AS AL ON AL.STUDENT_ID = SC.STUDENT_ID
                                        AND AL.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND AL.ATTENDANCE_TYPE_ID=2
                                        AND AL.IS_DELETED != 1
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = AL.COURSE_ID
                                   AND CR.SEMESTER_ID=S.SEMESTER
                                        AND CR.IS_DELETED != 1
                                         LEFT JOIN 
                                         SUP_SEMESTER AS SS ON  SS.SUP_SEMESTER_ID=S.SEMESTER
                                         AND SS.IS_DELETED!=1
                                WHERE
                                    PI.STUDENT_ID =?STUDENT_ID
                                   -- AND SC.ACADEMIC_YEAR=?AC_YEAR
                                    GROUP BY S.SEMESTER ORDER BY S.SEMESTER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTotalWorkingHoursForStudentByRegularCourseWise:
                    {
                        query = @"SELECT 
                                   CONCAT(IFNULL(SS.SEMESTER_NAME,''),' ','SEMESTER') AS LABEL,
								   count(TT.Hour_Id) AS TOTAL_WORKING_HOURS,
                                    SS.SUP_SEMESTER_ID
							
    
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS PI ON PI.STUDENT_ID = SC.STUDENT_ID
                                        AND PI.IS_LEFT != 1
                                        AND PI.IS_DELETED != 1
                                   --     INNER JOIN 
                                  --      CP_CLASSES AS CC ON CC.CLASS_ID=SC.CLASS_ID
                                  --      AND CC.IS_DELETED!=1
                                        INNER JOIN
                                       ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SC.ACADEMIC_YEAR
                                      --  AND AY.ACTIVE_YEAR = 1
                                        AND AY.IS_DELETED!=1
                                        INNER JOIN
                                       ACADEMIC_BATCHES AS B ON B.ACADEMIC_YEAR_ID = AY.ACADEMIC_YEAR_ID 
                                        AND B.BATCH_ID =SC.BATCH
                                        INNER JOIN
                                       ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR = AY.AC_YEAR
                                        AND S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND S.BATCH = B.BATCH_ID
                                    --     AND S.IS_ACTIVE=1
                                        AND S.IS_DELETED != 1
		                              
                                          INNER JOIN 
		                                CALENDER AS C ON C.ACADEMIC_YEAR=AY.AC_YEAR
                                       AND C.DAY_ORDER_DATE BETWEEN S.DATE_FROM AND
                                        S.DATE_TO
                                        AND C.IS_DELETED!=1
                                        AND C.IS_HOLIDAY!=1
		                                INNER JOIN 
                                         TT_TIMETABLE AS TT ON  TT.ACADEMIC_YEAR=SC.ACADEMIC_YEAR 
                                         AND TT.DAY_ORDER_ID=C.DAY_ORDER
                                         AND TT.SEMESTER_ID=S.SEMESTER
                                         AND TT.CLASS_ID=SC.CLASS_ID 
                                          AND TT.IS_DELETED!=1
                                         INNER JOIN
										CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                     --   AND CR.IS_COMPULSORY=1
                                        AND CR.SEMESTER_ID=S.SEMESTER
                                     --   INNER JOIN STU_COURSE_INFO AS SI ON SI.COURSE_ID=TT.COURSE_ID
										-- AND SI.STUDENT_ID=PI.STUDENT_ID
										-- AND SI.ACADEMIC_YEAR=AY.AC_YEAR
                                         LEFT JOIN 
                                         SUP_SEMESTER AS SS ON  SS.SUP_SEMESTER_ID=S.SEMESTER
                                         AND SS.IS_DELETED!=1
                                WHERE
                                    SC.STUDENT_ID =?STUDENT_ID
                                    AND SC.IS_DELETED!=1
                               --     AND SC.ACADEMIC_YEAR=?AC_YEAR
                                    GROUP BY S.SEMESTER ORDER BY S.SEMESTER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTotalWorkingHoursForStudentByOptionalCourseWise:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(SS.SEMESTER_NAME, ''),
                                            ' ',
                                            'SEMESTER') AS LABEL,
                                    COUNT(TT.Hour_Id) AS TOTAL_OPTIONAL_WORKING_HOURS,
                                     SS.SUP_SEMESTER_ID
                                FROM
                                    STU_COURSE_INFO AS CI
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID=CI.STUDENT_ID
                                        AND SC.ACADEMIC_YEAR=CI.ACADEMIC_YEAR
                                        AND SC.IS_DELETED!=1
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS PI ON PI.STUDENT_ID = SC.STUDENT_ID
                                        AND PI.IS_LEFT != 1
                                        AND PI.IS_DELETED != 1
                                         INNER JOIN
                                     CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CI.COURSE_ID
                                        AND CR.IS_DELETED != 1
                                          INNER JOIN
                                     ACADEMIC_SEMESTER AS S ON S.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND S.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND S.BATCH = SC.BATCH
                                     --   AND S.IS_ACTIVE=1
                                        AND S.IS_DELETED != 1
                                        
                                        INNER JOIN
                                    CALENDER AS C ON C.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND C.DAY_ORDER_DATE BETWEEN S.DATE_FROM AND S.DATE_TO
                                        AND C.IS_DELETED != 1
                                        AND C.IS_HOLIDAY!=1
                                        INNER JOIN
   
                                    TT_TIMETABLE AS TT ON TT.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                        AND TT.SEMESTER_ID = S.SEMESTER
                                        AND TT.CLASS_ID = CI.S_CLASS_ID
                                        AND TT.COURSE_ID=CI.COURSE_ID
                                        AND  TT.DAY_ORDER_ID = C.DAY_ORDER
                                        AND TT.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = S.SEMESTER
                                        AND SS.IS_DELETED != 1
                                WHERE
                                    CI.STUDENT_ID =?STUDENT_ID
                                     -- AND CI.ACADEMIC_YEAR =2018
                                        AND CI.IS_DELETED!=1
                                      
                                GROUP BY S.SEMESTER
                                ORDER BY S.SEMESTER;";
                        break;
                    }


                case SupportDataSQLCommands.FetchActiveCoursesFromCoursePattern:
                    {
                        query = @"SELECT 
                                    CR.COURSE_ROOT_ID,
                                    CR.COURSE_CODE,
                                    UPPER(CONCAT(IFNULL(CR.COURSE_CODE, ''),
                                                    '  ( ',
                                                    IFNULL(CR.COURSE_TITLE, ''),
                                                    ' )')) AS COURSE_TITLE
                                FROM
                                    CP_COURSE_PATTERN AS CP
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CP.COURSE_ID
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
                                        AND SEM.ACADEMIC_YEAR = CP.ACADEMIC_YEAR
                                WHERE
                                    CP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND CP.IS_DELETED != 1
                                GROUP BY CP.COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSessionForDDL:
                    {
                        query = @"SELECT 
                                        EXAM_SESSION_ID AS SESSION_ID, 
                                        EXAM_SESSION AS SESSION_NAME
                                    FROM
                                        SUP_EXAM_SESSION
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchExamRevaluationSetting:
                    {
                        query = @"SELECT 
                                    ERS.EXAM_REVALUATION_SETTING_ID,
                                    CONCAT(ES.EXAM_NAME,
                                            '-',
                                            ES.ACADEMIC_YEAR,
                                            '-',
                                            ' ',
                                            SP.APPLICATION_TYPE,
                                            '-',
                                            ' ',
                                            'Maximum Paper',
                                            ' ',
                                            ERS.MAX_PAPER) AS EXAM_SETTING,
                                            ERS.MAX_PAPER

                                FROM
                                    EXAM_REVALUATION_SETTING ERS
                                        INNER JOIN
                                    EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = ERS.EXAM_SETTING_ID
                                        AND ES.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_APPLICATION_TYPE AS SP ON SP.APPLICATION_TYPE_ID = ERS.APPLICATION_TYPE_ID
                                        AND SP.IS_DELETED != 1
                                WHERE
                                    ERS.IS_DELETED != 1
                                     AND DATE_FORMAT(ERS.START_DATE, '%Y-%m-%d') <= DATE_FORMAT(NOW(), '%Y-%m-%d')
                                        AND DATE_FORMAT(ERS.END_DATE, '%Y-%m-%d') >= DATE_FORMAT(NOW(), '%Y-%m-%d')
                                        AND ERS.ACADEMIC_YEAR =?AC_YEAR
                                        AND ERS.IS_ACTIVE=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchExamPhotoCopySetting:
                    {
                        query = @"SELECT 
                                    ERS.EXAM_PHOTOCOPY_SETTING_ID,
                                    CONCAT(ES.EXAM_NAME,
                                            '-',
                                            ES.ACADEMIC_YEAR,
                                            '-',
                                            ' ',
                                            SP.APPLICATION_TYPE,
                                            '-',
                                            ' ',
                                            'Maximum Paper',
                                            ' ',
                                            ERS.MAX_PAPER) AS EXAM_SETTING,
                                            ERS.MAX_PAPER

                                FROM
                                    EXAM_PHOTOCOPY_SETTING ERS
                                        INNER JOIN
                                    EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = ERS.EXAM_SETTING_ID
                                        AND ES.IS_DELETED != 1
                                        INNER JOIN
                                    SUP_APPLICATION_TYPE AS SP ON SP.APPLICATION_TYPE_ID = ERS.APPLICATION_TYPE_ID
                                        AND SP.IS_DELETED != 1
                                WHERE
                                    ERS.IS_DELETED != 1
                                         AND DATE_FORMAT(ERS.START_DATE, '%Y-%m-%d') <= DATE_FORMAT(NOW(), '%Y-%m-%d')
                                        AND DATE_FORMAT(ERS.END_DATE, '%Y-%m-%d') >= DATE_FORMAT(NOW(), '%Y-%m-%d')
                                        AND ERS.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchApplicationTypeByStudentId:
                    {
                        query = @"SELECT 
                                    CPP.APPTYPE_ID
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                        AND SPI.IS_DELETED != 1
                                        AND SPI.IS_LEFT != 1
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP AS CPP ON CPP.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                        AND CPP.IS_DELETED != 1
                                WHERE
                                    SC.IS_DELETED != 1
                                    AND SC.STUDENT_ID=?STUDENT_ID
                                    AND SC.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                #region Menu Items
                case SupportDataSQLCommands.FetchMenuItems:
                    {
                        query = @"SELECT 
                                    MI.MENU_ID,
                                    MI.MENU_NAME,
                                    M.MODULE_NAME,
                                    MI.CONTROLLER,
                                    MI.ACTION
                                FROM
                                    MENU_ITEMS AS MI
                                        INNER JOIN
                                    MODULES AS M ON M.MODULES_ID = MI.PARENT_ID
                                        AND M.IS_DELETED != 1
                                WHERE
                                    MI.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMenuItemById:
                    {
                        query = @"SELECT 
                                MI.MENU_ID,
                                MI.MENU_NAME,
                                MI.PARENT_ID,
                                MI.CONTROLLER,
                                MI.ACTION
                            FROM
                                MENU_ITEMS AS MI
       
                            WHERE
                                MI.IS_DELETED != 1
                                AND MI.MENU_ID=?MENU_ID;";
                        break;
                    }
                case SupportDataSQLCommands.SaveMenuItem:
                    {
                        query = @"INSERT INTO MENU_ITEMS(MENU_NAME,PARENT_ID,CONTROLLER,ACTION)
                                  VALUES(?MENU_NAME,?PARENT_ID,?CONTROLLER,?ACTION);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateMenuItemById:
                    {
                        query = @"UPDATE MENU_ITEMS SET MENU_NAME=?MENU_NAME,PARENT_ID=?PARENT_ID,CONTROLLER=?CONTROLLER,ACTION=?ACTION WHERE MENU_ID=?MENU_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteMenuItemById:
                    {
                        query = @"UPDATE MENU_ITEMS SET IS_DELETED=?IS_DELETED WHERE MENU_ID=?MENU_ID;";
                        break;
                    }

                #endregion

                case SupportDataSQLCommands.FetchUserRolesRights:
                    {
                        query = @"SELECT 
                                    UR.USER_RIGHTS_ID,
                                    MI.MENU_NAME,
                                    SR.ROLE_NAME,
                                    UR.IS_ACTIVE
                                    FROM USER_ROLES_RIGHTS AS UR 

                                    INNER JOIN
                                    MENU_ITEMS AS MI ON MI.MENU_ID=UR.MENU_ITEM_ID
                                    AND MI.IS_DELETED!=1

                                    INNER JOIN
                                    SUP_ROLE AS SR ON SR.ROLE_ID=UR.ROLE_ID

                                    WHERE 
                                    UR.IS_DELETED!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchUserRolesRightsById:
                    {
                        query = @"SELECT 
                                    UR.USER_RIGHTS_ID,
                                    MENU_ITEM_ID,
                                    ROLE_ID,
                                    UR.IS_ACTIVE
                                    FROM USER_ROLES_RIGHTS AS UR 

                                    WHERE 
                                    UR.IS_DELETED!=1 AND UR.USER_RIGHTS_ID=?USER_RIGHTS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.SaveUserRolesRights:
                    {
                        query = @"
                                    INSERT INTO USER_ROLES_RIGHTS(MENU_ITEM_ID,ROLE_ID,IS_ACTIVE)
                                    VALUES(?MENU_ITEM_ID,?ROLE_ID,?IS_ACTIVE);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateUserRolesRightsById:
                    {
                        query = @"UPDATE USER_ROLES_RIGHTS SET MENU_ITEM_ID=?MENU_ITEM_ID,ROLE_ID=?ROLE_ID,IS_ACTIVE=?IS_ACTIVE WHERE USER_RIGHTS_ID=?USER_RIGHTS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.ActivateorDeactivateUserRolesRightsById:
                    {
                        query = @"UPDATE USER_ROLES_RIGHTS SET IS_ACTIVE=?IS_ACTIVE WHERE USER_RIGHTS_ID=?USER_RIGHTS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteUserRolesRightsById:
                    {
                        query = @"UPDATE USER_ROLES_RIGHTS SET IS_DELETED=?IS_DELETED WHERE USER_RIGHTS_ID=?USER_RIGHTS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchUserRolesRightsByMenuItemIdandRoleId:
                    {
                        query = @"SELECT USER_RIGHTS_ID FROM user_roles_rights WHERE MENU_ITEM_ID=?MENU_ITEM_ID AND ROLE_ID=?ROLE_ID AND IS_DELETED!=1;";
                        break;
                    }
                #region  Modules
                case SupportDataSQLCommands.FetchModules:
                    {
                        query = @"SELECT MODULES_ID, MODULE_NAME, MODULES_ORDER, STYLE, has_sub FROM MODULES WHERE IS_DELETED!=1 ORDER BY MODULES_ORDER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchModuleById:
                    {
                        query = @"SELECT MODULES_ID, MODULE_NAME, MODULES_ORDER, STYLE, has_sub FROM MODULES WHERE MODULES_ID=?MODULES_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.SaveModule:
                    {
                        query = @"INSERT INTO MODULES(MODULE_NAME, MODULES_ORDER, STYLE, has_sub) VALUES(?MODULE_NAME, ?MODULES_ORDER, ?STYLE, ?has_sub);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateModuleById:
                    {
                        query = @"UPDATE MODULES SET MODULE_NAME=?MODULE_NAME, MODULES_ORDER=?MODULES_ORDER, STYLE=?STYLE, has_sub=?has_sub WHERE MODULES_ID=?MODULES_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteModuleById:
                    {
                        query = @"UPDATE MODULES SET IS_DELETED=?IS_DELETED WHERE MODULES_ID=?MODULES_ID;";
                        break;
                    }
                #endregion

                #region Semester Configuration
                // Semester Type
                case SupportDataSQLCommands.FetchSemesterTypeList:
                    {
                        query = @"SELECT SEMESTER_TYPE_ID, SEMESTER_TYPE AS SEMESTER_TYPE_NAME, IS_ACTIVE, IS_DELETED FROM semester_type WHERE IS_DELETED!=1 ORDER BY SEMESTER_TYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSemesterTypeById:
                    {
                        query = @"SELECT SEMESTER_TYPE_ID, SEMESTER_TYPE AS SEMESTER_TYPE_NAME, IS_ACTIVE, IS_DELETED FROM semester_type WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSemesterType:
                    {
                        query = @"INSERT INTO semester_type (SEMESTER_TYPE, IS_ACTIVE, IS_DELETED) VALUES (?SEMESTER_TYPE_NAME, ?IS_ACTIVE, 0);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSemesterTypeById:
                    {
                        query = @"UPDATE semester_type SET SEMESTER_TYPE=?SEMESTER_TYPE_NAME, IS_ACTIVE=?IS_ACTIVE WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSemesterTypeById:
                    {
                        query = @"UPDATE semester_type SET IS_DELETED=?IS_DELETED WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.ToggleSemesterTypeStatusById:
                    {
                        query = @"UPDATE semester_type SET IS_ACTIVE=?IS_ACTIVE WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID;";
                        break;
                    }

                // Support Semester Type
                case SupportDataSQLCommands.FetchSupSemesterTypeList:
                    {
                        query = @"SELECT SEMESTER_TYPE_ID, SEMESTER_TYPE, IS_ACTIVE, IS_DELETED FROM sup_semester_type WHERE IS_DELETED!=1 ORDER BY SEMESTER_TYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSemesterTypeById:
                    {
                        query = @"SELECT SEMESTER_TYPE_ID, SEMESTER_TYPE, IS_ACTIVE, IS_DELETED FROM sup_semester_type WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSupSemesterType:
                    {
                        query = @"INSERT INTO sup_semester_type (SEMESTER_TYPE, IS_ACTIVE, IS_DELETED) VALUES (?SEMESTER_TYPE, ?IS_ACTIVE, 0);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupSemesterTypeById:
                    {
                        query = @"UPDATE sup_semester_type SET SEMESTER_TYPE=?SEMESTER_TYPE, IS_ACTIVE=?IS_ACTIVE WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupSemesterTypeById:
                    {
                        query = @"UPDATE sup_semester_type SET IS_DELETED=?IS_DELETED WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.ToggleSupSemesterTypeStatusById:
                    {
                        query = @"UPDATE sup_semester_type SET IS_ACTIVE=?IS_ACTIVE WHERE SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID;";
                        break;
                    }

                // Support Semester
                case SupportDataSQLCommands.FetchSupSemesterList:
                    {
                        query = @"SELECT s.SUP_SEMESTER_ID, s.SEMESTER_NAME, s.SEMESTER_TYPE, IFNULL(st.SEMESTER_TYPE, '') AS SEMESTER_TYPE_NAME, s.IS_ACTIVE, s.IS_DELETED FROM sup_semester s LEFT JOIN sup_semester_type st ON st.SEMESTER_TYPE_ID = s.SEMESTER_TYPE AND st.IS_DELETED!=1 WHERE s.IS_DELETED!=1 ORDER BY s.SUP_SEMESTER_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSemesterById:
                    {
                        query = @"SELECT s.SUP_SEMESTER_ID, s.SEMESTER_NAME, s.SEMESTER_TYPE, IFNULL(st.SEMESTER_TYPE, '') AS SEMESTER_TYPE_NAME, s.IS_ACTIVE, s.IS_DELETED FROM sup_semester s LEFT JOIN sup_semester_type st ON st.SEMESTER_TYPE_ID = s.SEMESTER_TYPE WHERE s.SUP_SEMESTER_ID=?SUP_SEMESTER_ID AND s.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.SaveSupSemester:
                    {
                        query = @"INSERT INTO sup_semester (SEMESTER_NAME, SEMESTER_TYPE, IS_ACTIVE, IS_DELETED) VALUES (?SEMESTER_NAME, ?SEMESTER_TYPE, ?IS_ACTIVE, 0);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupSemesterById:
                    {
                        query = @"UPDATE sup_semester SET SEMESTER_NAME=?SEMESTER_NAME, SEMESTER_TYPE=?SEMESTER_TYPE, IS_ACTIVE=?IS_ACTIVE WHERE SUP_SEMESTER_ID=?SUP_SEMESTER_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.DeleteSupSemesterById:
                    {
                        query = @"UPDATE sup_semester SET IS_DELETED=?IS_DELETED WHERE SUP_SEMESTER_ID=?SUP_SEMESTER_ID;";
                        break;
                    }
                case SupportDataSQLCommands.ToggleSupSemesterStatusById:
                    {
                        query = @"UPDATE sup_semester SET IS_ACTIVE=?IS_ACTIVE WHERE SUP_SEMESTER_ID=?SUP_SEMESTER_ID;";
                        break;
                    }
                #endregion

                #region Library Support Data
                case SupportDataSQLCommands.FetchIllustrationDDL:
                    {
                        query = @"SELECT 
                                        ILLUSTRATE_ID,
                                        CONCAT(IFNULL(ILLUSTRATE_CODE, ''),
                                                ' -  ',
                                                IFNULL(ILLUSTRATE, '')) AS ILLUSTRATE
                                    FROM
                                        SUP_LI_ILLUSTRATION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAccountDDL:
                    {
                        query = @"SELECT 
                                        ACCOUNT_ID,
                                        CONCAT(IFNULL(PREFIX, ''),
                                                ' -  ',
                                                IFNULL(ACCOUNT_NAME, '')) AS ACCOUNT_NAME
                                    FROM
                                        SUP_LI_ACCOUNT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTapDDL:
                    {
                        query = @"SELECT 
                                        TAP_ID,
                                         CONCAT(IFNULL(TAP_CODE, ''),
                                                ' -  ',
                                                IFNULL(TAP_NAME, '')) AS TAP_NAME
                                    FROM
                                        SUP_LI_TAPS;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiMemberCategory:
                    {
                        query = @"SELECT 
                                CATEGORY_ID,
                                CATEGORY_CODE,
                                CONCAT(IFNULL(CATEGORY_CODE,''),' - ',IFNULL(CATEGORY_NAME,'')) AS CATEGORY_NAME,
                                CATEGORY_ALIAS,
                                IS_ACTIVE,
                                IS_DELETED
                            FROM
                                SUP_LI_MEMBER_CATEGORY
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDisciplineDDL:
                    {
                        query = @"SELECT 
                                        DISCIPLINE_ID,
                                        CONCAT(IFNULL(DISCIPLINE_CODE,''),' - ',IFNULL(DISCIPLINE_NAME,'')) AS DISCIPLINE_NAME
                                    FROM
                                        SUP_LI_DISCIPLINE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAutoClassDDL:
                    {
                        query = @"SELECT 
                                      CLASS_ID,
                                      CONCAT(IFNULL(CLASS_CODE,''),' - ',IFNULL(CLASS_NAME,'')) AS CLASS_NAME
                                    FROM
                                        SUP_LI_CLASS;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDocumentationTypeDDL:
                    {
                        query = @"SELECT 
                                       DOCUMENT_TYPE_ID,
                                      CONCAT(IFNULL(DOCUMENT_CODE,''),' - ',IFNULL(DOCUMENT_TYPE_NAME,'')) AS DOCUMENT_TYPE_NAME
                                    FROM
                                        SUP_LI_DOCUMENT_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSeriesDDL:
                    {
                        query = @"SELECT 
                                      SERIES_ID,
                                      CONCAT(IFNULL(SERIES_CODE,''),' - ',IFNULL(SERIES_NAME,'')) AS SERIES_NAME
                                    FROM
                                        SUP_LI_SERIES;";
                        break;
                    }
                case SupportDataSQLCommands.FetchArticleType:
                    {
                        query = @"SELECT 
                                    ARTICLE_TYPE_ID,
                                    ARTICLE_TYPE_NAME,
                                    IS_DELETED,
                                    IS_ACTIVE
                                FROM
                                    SUP_LI_ARTICLE_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStatusDDL:
                    {
                        query = @"SELECT 
                                    STATUS_ID, STATUS_NAME, IS_ACTIVE, IS_DELETED
                                FROM
                                    SUP_LI_BOOK_STATUS
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.UpdateAccessNoInAutoGenerate:
                    {
                        query = @"UPDATE AUTO_GENERATE_NUMBERS SET ACCESS_NO=ACCESS_NO+1 WHERE AUTO_GENERATE_NUMBERS_ID=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLiSubjectType:
                    {
                        query = @"SELECT 
                                    LI_SUBJECT_ID,
                                    CONCAT(IFNULL(SUBJECT_CODE,''),' - ',IFNULL(SUBJECT_NAME,'')) AS SUBJECT_CODE,
                                    SUBJECT_NAME,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    sup_li_subjects
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCurrentDateandTime:
                    {
                        query = @"SELECT 
                                    DATE_FORMAT(NOW(), '%Y-%m-%d') AS C_DATE,
                                    IF(DATE_FORMAT(NOW(), '%r') = '04:00:00 PM',
                                        '1',
                                        TIME_TO_SEC(SUBTIME(DATE_FORMAT(NOW(), '%r'), '04:00:00 PM')) * 1000) AS ELASPED_TIME,
                                    DATE_FORMAT(NOW(), '%r') AS C_TIME;
                                SELECT DATE_FORMAT(NOW(),'%r');";
                        break;
                    }
                case SupportDataSQLCommands.FetchLibraryBookApprovedUserByStudentId:
                    {
                        query = @"SELECT 
                                    LBR.RESERVE_ID,
                                    lower(CONCAT('Dear',
                                    ' ',
                                    IFNULL(LD.FIRST_NAME, ''),
                                    ' ',
                                    IFNULL(LD.LAST_NAME, ''),
                                    ' ',
                                    'Your Reserved Book',
                                    ' ',
                                    IFNULL(LB.BOOK_TITLE, ''),
                                    ' ',
                                    'Has been Approved By The Library Admin on ',
                                    IFNULL(DATE_FORMAT(LBR.APPROVED_DATE, '%Y-%m-%d'),
                                            ''),
                                    '.',
                                    'please go and collect your book within one working day or else it will be cancelled.')) AS STATUS
                                FROM
                                    LI_BOOK_RESERVE AS LBR
                                        INNER JOIN
                                    LI_BOOK_DETAILS AS LB ON LB.BOOK_ID = LBR.BOOK_ID
                                        AND LB.STATUS = 5
                                        AND LB.IS_DELETED != 1
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.USER_ID = LBR.USER_ID
                                        AND LD.IS_DELETED != 1
		                                INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.BIOD_NUMBER=LD.BOID
                                        AND SPI.STUDENT_ID=?STUDENT_ID
                                        AND SPI.IS_LEFT!=1
                                        AND SPI.IS_DELETED!=1
                                WHERE
                                    LBR.IS_APPROVED = 1
                                    AND LBR.ACADEMIC_YEAR=?AC_YEAR
                                        AND LBR.IS_COLLECTED != 1
                                        AND LBR.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLiAuthors:
                    {
                        query = @"SELECT 
                                    AUTHOR_ID,
                                    CONCAT(IFNULL(FIRST_NAME,''),' ',IFNULL(LAST_NAME,''),' ',IFNULL(PUBLISHER,'')) AS FIRST_NAME,
                                    LAST_NAME,
                                    PUBLISHER,
                                    PLACE,
                                    ENTRY_ID,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM
                                    LI_AUTHOR_INFO
    
                                    WHERE 
                                    IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLiRacks:
                    {
                        query = @"SELECT 
                                    RACK_ID,
                                    LR.BLOCK_ID,
                                    BOW_NO,
                                    SECTION_NO,
                                    CONCAT(IFNULL(LB.BLOCK_NAME, ''),' ','BOW'
                                            ':',
                                            IFNULL(BOW_NO, ''),' ',
                                            'RACK:',
                                            IFNULL(RACK_NO, ''),' ',
                                            'SECTION:',
                                            IFNULL(SECTION_NO, ''),' ',
                                            'POSITION:',
                                            IFNULL(POSITION, '')) AS RACK_NO,
                                    POSITION
                                FROM
                                    LI_RACK_INFO AS LR
                                        INNER JOIN
                                    SUP_LI_BLOCK AS LB ON LB.BLOCK_ID = LR.RACK_ID
                                        AND LB.IS_DELETED != 1
                                WHERE
                                    LR.IS_DELETED != 1;";
                        break; ;
                    }
                case SupportDataSQLCommands.FetchBlockDDL:
                    {
                        query = @"SELECT BLOCK_ID, BLOCK_NAME FROM SUP_LI_BLOCK WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffInfoByStaffId:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(FIRST_NAME,''),'',IFNULL(LAST_NAME,'')) AS FIRST_NAME,
                                    PHOTO
                                    FROM
                                        STF_PERSONAL_INFO
                                        WHERE
                                         STAFF_ID=?STAFF_ID
                                        AND IS_DELETED!=1
                                        AND IS_LEFT!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiAccount:
                    {
                        query = @"SELECT 
                                    ACCOUNT_ID,
                                    PREFIX,
                                    ACCOUNT_NAME
                                FROM
                                    SUP_LI_ACCOUNT
                                WHERE
                                    IS_DELETED != 1;";
                        break;

                    }
                case SupportDataSQLCommands.FetchSupLiStatus:
                    {
                        query = @"SELECT 
                                        STATUS_ID, 
                                        STATUS_NAME
                                    FROM
                                        SUP_LI_STATUS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiIllustration:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(ILLUSTRATE, ''),
                                            ' - ',
                                            IFNULL(PREFIX, '')) AS ILLUSTRATE,
                                    ILLUSTRATE_ID
                                FROM
                                    SUP_LI_ILLUSTRATION
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiDoctype:
                    {
                        query = @"SELECT 
                                        DOCUMENT_TYPE_ID, 
                                        CONCAT(DOCUMENT_TYPE,' - ',DOCUMENT_TYPE_CODE) AS DOCUMENT_TYPE
                                    FROM
                                        SUP_LI_DOCUMENT_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiAutoclass:
                    {
                        query = @"SELECT 
                                        CLASS_NO_ID, 
                                        CLASS_NO
                                    FROM
                                        SUP_LI_CLASS_NO
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiDiscipline:
                    {
                        query = @"SELECT 
                                        DISCIPLINE_ID, 
                                        CONCAT(DISCIPLINE_NAME,' - ',DISCIPLINE_CODE) AS DISCIPLINE_NAME
                                    FROM
                                        SUP_LI_DISCIPLINE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLikeywords:
                    {
                        query = @"SELECT 
                                        KEYWORD_ID, 
                                        CONCAT(KEYWORD,' - ',KEY_CODE) AS KEYWORD
                                    FROM
                                        SUP_LI_KEYWORDS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiSeries:
                    {
                        query = @"SELECT 
                                        SERIES_ID, 
                                        CONCAT(SERIES_NAME,' - ',SERIES_CODE) AS SERIES_NAME
                                    FROM
                                        SUP_LI_SERIES
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiImprint:
                    {
                        query = @"SELECT 
                                        IMPRINT_ID, 
                                        IMPRINT
                                    FROM
                                        SUP_LI_IMPRINT
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiClassNo:
                    {
                        query = @"SELECT 
                                        CLASS_NO_ID, 
                                        CLASS_NO
                                    FROM
                                        SUP_LI_CLASS_NO
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiLibCode:
                    {
                        query = @"SELECT 
                                        LIB_CODE_ID, 
                                        CONCAT(LIB_DESCRIPTION,' - ',LIB_CODE) AS LIB_DESCRIPTION
                                    FROM
                                        SUP_LI_LIB_CODE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiLibCode:
                    {
                        query = @"INSERT INTO SUP_LI_DOC_CATEGORY(DOC_CATEGORY_CODE,DOC_CATEGORY)VALUES(?DOC_CATEGORY_CODE,?DOC_CATEGORY);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupLiLibCode:
                    {
                        query = @"UPDATE SUP_LI_DOC_CATEGORY 
                                  SET 
                                    DOC_CATEGORY_CODE=?DOC_CATEGORY_CODE ,
                                    DOC_CATEGORY=?DOC_CATEGORY
                                  WHERE
                                    DOC_CATEGORY_ID=?DOC_CATEGORY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiClassNo:
                    {
                        query = @"INSERT 
                                  INTO
                                  SUP_LI_CLASS_NO(CLASS_NO)
                                  VALUES(?CLASS_NO);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupLiClassNo:
                    {
                        query = @"UPDATE
                                    SUP_LI_CLASS_NO 
                                    SET CLASS_NO=?CLASS_NO 
                                    WHERE 
                                    CLASS_NO_ID=?CLASS_NO_ID;";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiStatus:
                    {
                        query = @"INSERT INTO SUP_LI_STATUS(STATUS_NAME)VALUES(?STATUS_NAME);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupLiStatus:
                    {
                        query = @"UPDATE SUP_LI_STATUS SET STATUS_NAME=?STATUS_NAME WHERE STATUS_ID=?STATUS_ID;";
                        break;
                    }

                case SupportDataSQLCommands.AddSupLiAccount:
                    {
                        query = @"INSERT
                                 INTO 
                                 SUP_LI_ACCOUNT
                                 (PREFIX,
                                 ACCOUNT_NAME)
                                 VALUES
                                 (?PREFIX,
                                 ?ACCOUNT_NAME);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateSupLiAccount:
                    {
                        query = @"UPDATE
                                 SUP_LI_ACCOUNT
                                 SET
                                 PREFIX=?PREFIX,
                                ACCOUNT_NAME=?ACCOUNT_NAME
                                WHERE 
                                ACCOUNT_ID=?ACCOUNT_ID;";
                        break;
                    }

                case SupportDataSQLCommands.AddSupLiIllustrate:
                    {
                        query = @"INSERT INTO sup_li_illustration(PREFIX,ILLUSTRATE)VALUES(?PREFIX,?ILLUSTRATE);";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiDocType:
                    {
                        query = @"INSERT INTO  SUP_LI_DOCUMENT_TYPE (DOCUMENT_TYPE_CODE, DOCUMENT_TYPE)VALUES(?DOCUMENT_TYPE_CODE,?DOCUMENT_TYPE);";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiDescipline:
                    {
                        query = @"INSERT
                                 INTO 
                                 sup_li_discipline
                                 (DISCIPLINE_NAME,
                                 DISCIPLINE_CODE)
                                 VALUES
                                 (?DISCIPLINE_NAME,
                                 ?DISCIPLINE_CODE);";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiKeywords:
                    {
                        query = @"INSERT INTO  SUP_LI_KEYWORDS (KEY_CODE,KEYWORD) VALUES (?KEY_CODE,?KEYWORD);";
                        break;
                    }
                case SupportDataSQLCommands.AddSupLiSeries:
                    {
                        query = @"INSERT
                                 INTO 
                                 sup_li_series
                                 (SERIES_NAME,SERIES_CODE)
                                 VALUES
                                 (?SERIES_NAME,?SERIES_CODE);";
                        break;

                    }
                case SupportDataSQLCommands.FetchSupLiRenewDays:
                    {
                        query = @"
                                SELECT 
                                    MEMBER_TYPE,
                                    DAYS
                                FROM
                                    SUP_LI_RENEW_DAYS
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLiUserIdByStaffId:
                    {
                        query = @"SELECT 
                                    LD.USER_ID
                                FROM
                                    STF_PERSONAL_INFO AS STF
                                        INNER JOIN
                                    LI_USER_DETAILS AS LD ON LD.BOID = STF.BOID_NUMBER
                                        AND LD.IS_DELETED != 1
                                WHERE
                                    STF.STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLiUserIdByStudentId:
                    {
                        query = @"SELECT 
                                    LU.USER_ID
                                FROM
                                    STU_PERSONAL_INFO AS STU
                                        INNER JOIN
                                    LI_USER_DETAILS AS LU ON LU.BOID = STU.BOID_NUMBER
                                        AND LU.IS_DELETED != 1
                                WHERE
                                    STU.STUDENT_ID =?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiServiceTime:
                    {
                        query = @"SELECT 
                                    SERVICE_ID, SERVICE_NAME, S_HOUR, S_MINUTE
                                FROM
                                    SUP_LI_SERVICE_TIME
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region feedback
                case SupportDataSQLCommands.FetchFeedBackQuestionType:
                    {
                        query = @"SELECT 
                                QUESTION_TYPE_ID,
                                QUESTION_TYPE, 
                                IS_ACTIVE,
                                IS_DELETED
                            FROM
                                SUP_FBFEEDBACK_QUESTION_TYPE
                            WHERE
                                IS_DELETED != 1; ";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedBackQuestionGroup:
                    {
                        query = @"SELECT 
                                    QUESTION_GROUP_ID, GROUP_TITLE, IS_DELETED
                                FROM
                                    FBQUESTIONS_GROUP
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchActiveFeedBackSettings:
                    {
                        query = @" SELECT 
                                    FEEDBACK_NAME, FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_SETTINGS
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED != 1
                                   AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }

                case SupportDataSQLCommands.FetchExternalActiveFeedBackSettings:
                    {
                        query = @" SELECT 
                                    FEEDBACK_NAME, FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_SETTINGS
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED != 1
                                   AND ACADEMIC_YEAR=?AC_YEAR AND FEEDBACK_TYPE=3;";
                        break;
                    }

                case SupportDataSQLCommands.FetchFeedBackSettingsByYear:
                    {
                        query = @" SELECT 
                                    FEEDBACK_NAME, FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_SETTINGS
                                WHERE
                                   IS_DELETED != 1
                                   AND ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffFeedBackSettingsByYear:
                    {
                        query = @" SELECT 
                                    FEEDBACK_NAME, FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_SETTINGS
                                WHERE
                                   IS_DELETED != 1
                                   AND ACADEMIC_YEAR=?AC_YEAR AND FEEDBACK_TYPE=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSyllabusFeedBackSettingsByYear:
                    {
                        query = @" SELECT 
                                    FEEDBACK_NAME, FEEDBACK_ID
                                FROM
                                    FBFEEDBACK_SETTINGS
                                WHERE
                                   IS_DELETED != 1
                                   AND ACADEMIC_YEAR=?AC_YEAR AND FEEDBACK_TYPE=2;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedBackRole:
                    {
                        query = @"SELECT 
                                    SUP_FEEDBACK_ROLE_ID,
                                    FEEDBACK_ROLE_NAME
                                FROM
                                    SUP_FBFEEDBACK_ROLE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedBackRoleSettings:
                    {
                        query = @"
                                SELECT 
                                FRS.FEEDBACK_ROLE_ID,
                                FR.FEEDBACK_ROLE_NAME,
                                FRS.IS_ATTENDED
    
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
                                AND FRS.IS_ATTENDED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedBackRoleSettingsByFeedBackid:
                    {
                        query = @"SELECT 
                                FRS.FEEDBACK_ROLE_ID,
                                FR.FEEDBACK_ROLE_NAME,
                                FRS.IS_ATTENDED
    
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
                                AND FRS.FEEDBACK_ID=?FEEDBACK_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedBackSettingByFeedBackId:
                    {
                        query = @"
                                    SELECT 
                                    FS.FEEDBACK_ID,
                                    FS.FEEDBACK_NAME,
                                    FS.NO_OF_QUESTION,
                                    FS.FEEDBACK_TYPE,
                                    FG.GROUP_TITLE AS FEEDBACK_TYPE_NAME
                                FROM
                                    FBFEEDBACK_SETTINGS AS FS
                                    INNER JOIN
                                    fbquestions_group AS FG ON FG.QUESTION_GROUP_ID=FS.FEEDBACK_TYPE
                                    AND FG.IS_DELETED!=1
                                    
                                WHERE
                                    FS.FEEDBACK_ID =?FEEDBACK_ID
									AND FS.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchVicePrincipal:
                    {
                        query = @" SELECT 
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(STF.LAST_NAME, ' '),
                                            ' - ',
                                            IFNULL(STF.STAFF_CODE, '')) AS FIRST_NAME,
                                    STF.STAFF_ID
                                    
                                   
                                    
                                FROM
                                    USER_ACCOUNT AS UA
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                        AND STF.IS_LEFT != 1
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
									user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                  AND AY.ACADEMIC_YEAR=?AC_YEAR
								  AND AY.ROLE_ID=UA.ROLE
                                   AND AY.USER_TYPE='4'
                                  AND AY.IS_DELETED != 1

                                WHERE
                                    UA.ROLE = 8 AND UA.IS_DELETED != 1
                                    AND UA.USER_TYPE = AY.USER_TYPE; ";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedBackSettingByFeedBackIdAndFeedBackRoleId:
                    {
                        query = @" SELECT 
                                    FS.FEEDBACK_ID,
                                    FS.FEEDBACK_NAME,
                                     FS.NO_OF_QUESTION - SUM(IF(FQ.QUESTION_ID IS NULL OR FQ.QUESTION_ID=0,0,1))  AS NO_OF_QUESTION,
                                    FS.FEEDBACK_TYPE,
                                    FG.GROUP_TITLE AS FEEDBACK_TYPE_NAME
                                FROM
                                    FBFEEDBACK_SETTINGS AS FS
                                        INNER JOIN
                                    fbquestions_group AS FG ON FG.QUESTION_GROUP_ID = FS.FEEDBACK_TYPE
                                        AND FG.IS_DELETED != 1
                                        LEFT JOIN
                                    FBFEEDBACK_QUESTIONS AS FQ ON FQ.FEEDBACK_ID = FS.FEEDBACK_ID
                                        AND FQ.IS_DELETED != 1
                                        LEFT JOIN
                                    FBFEEDBACK_ROLE_SETTINGS AS FRS ON FRS.FEEDBACK_ROLE_ID = FQ.FEEDBACK_ROLE_ID
                                        AND FRS.IS_DELETED != 1
                                WHERE
                                    FS.FEEDBACK_ID =?FEEDBACK_ID
                                     AND FQ.FEEDBACK_ROLE_ID = ?FEEDBACK_ROLE_ID;";
                        break;
                    }

                case SupportDataSQLCommands.FetchPrincipal:
                    {
                        query = @"SELECT 
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(STF.LAST_NAME, ' '),
                                            ' - ',
                                            IFNULL(STF.STAFF_CODE, '')) AS FIRST_NAME,
                                    STF.STAFF_ID
                                    
                                FROM
                                    USER_ACCOUNT AS UA
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                        AND STF.IS_LEFT != 1
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
									user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                    AND AY.ACADEMIC_YEAR=?AC_YEAR
									AND AY.USER_TYPE='4'
								    AND AY.ROLE_ID=3
                                   AND AY.IS_DELETED != 1

                                WHERE
                                     UA.IS_DELETED != 1 GROUP BY AY.USER_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchUserRolesByUserIdandAcYear:
                    {
                        query = @"SELECT 
                                   UA.USER_ID,
                                    GROUP_CONCAT(AY.ROLE_ID) AS ROLE
                                     FROM
                                     USER_ACCOUNT AS UA
                                  INNER JOIN
                                  user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                  AND AY.ACADEMIC_YEAR=?AC_YEAR
								--  AND find_in_set(AY.ROLE_ID,UA.ROLE)
                                   AND AY.USER_TYPE=?USER_TYPE
							      AND AY.IS_DELETED!=1
                                    
								WHERE
                                UA.USER_ID=?USER_ID
                                AND UA.IS_DELETED!=1
                                AND UA.USER_TYPE=AY.USER_TYPE;";
                        break;
                    }

                #endregion

                #region Alumnae

                case SupportDataSQLCommands.FetchSupCompetitiveExam:
                    {
                        query = @"	SELECT 
                                        COMPETITIVE_EXAM_ID, COMPETITIVE_EXAM
                                    FROM
                                        SUP_COMPETITIVE_EXAM
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchSupAluContributionType:
                    {
                        query = @"SELECT 
                                    CONTRIBUTION_TYPE_ID,
                                    CONTRIBUTION_TYPE_NAME
                                FROM
                                    SUP_ALU_CONTRIBUTION_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }


                #endregion
                #region Alumnae passedout
                case SupportDataSQLCommands.FetchSupAluScholorshipType:
                    {
                        query = @"SELECT 
                                    ALU_SCHOLORSHIP_TYPE_ID,
                                    SCHOLORSHIP_NAME
                                FROM
                                    SUP_ALU_SCHOLORSHIP_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupAluScholorshipScheme:
                    {
                        query = @"SELECT 
                                    ALU_SCHOLORSHIP_SCHEME_ID,
                                    SCHEME
                                FROM
                                    SUP_ALU_SCHOLORSHIP_SCHEME
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }


                #endregion

                case SupportDataSQLCommands.FetchsmsTemplateByTemplateId:
                    {
                        query = @"SELECT 
                                        TEMPLATE_ID, TEMPLATE_NAME, TEMPLATE_TEXT, UT.FIELDS_IDS
                                    FROM
                                        SUP_SMS_TEMPLATE_MODULE AS T
                                            INNER JOIN
                                        SMS_USERDEFINED_TEMPLATE AS UT ON T.SMS_TEMPLATE_MODULE_ID = UT.TEMPLATE_ID
                                    WHERE
                                        T.ACADEMIC_YEAR=?AC_YEAR
                                        AND UT.TEMPLATE_ID=2
                                        AND T.IS_ACTIVE!=0
                                        AND T.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSMSTemplateText:
                    {
                        query = @"SELECT 
                                        TEMPLATE_ID, TEMPLATE_NAME, TEMPLATE_TEXT, UT.FIELDS_IDS
                                    FROM
                                        SUP_SMS_TEMPLATE_MODULE AS T
                                            INNER JOIN
                                         SMS_USERDEFINED_TEMPLATE AS UT ON T.SMS_TEMPLATE_MODULE_ID = UT.TEMPLATE_ID
                                    WHERE
                                         UT.TEMPLATE_ID=?TEMPLATE_ID
                                        AND T.IS_ACTIVE =1
                                        AND T.IS_DELETED != 1 AND UT.IS_ACTIVE=1
                                        AND UT.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentBySudentId:
                    {
                        query = @"SELECT 
                                DEP.DEPARTMENT
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                    INNER JOIN
                                CP_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = CC.DEPARTMENT
                            WHERE
                                STUDENT_ID = ?STUDENTID;";
                        break;
                    }
                case SupportDataSQLCommands.UpdateImagePath:
                    {
                        query = @"UPDATE STU_PERSONAL_INFO SET PHOTO=?PHOTO WHERE STUDENT_ID=?STUDENTID;";
                        break;
                    }
                case SupportDataSQLCommands.CheckIsConvacationRegisterExist:
                    {
                        query = @"SELECT 
                            CONVOC_APP_REGISTER_ID
                        FROM
                            CONVOCATION_APPLICATION_REGISTER
                        WHERE
                            STUDENT_ID =?STUDENTID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeandClassyear:
                    {
                        query = @"SELECT 
                            SC.PROGRAMME_GROUP_ID AS PROGRAMME,CC.CLASS_YEAR AS CLASS_YEAR_ID
                        FROM
                            STU_CLASS AS SC
                                INNER JOIN
                            CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                        WHERE
                            SC.STUDENT_ID =?STUDENTID
                                AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDeen:
                    {
                        query = @"                        SELECT 
                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(STF.LAST_NAME, ' '),
                                            ' - ',
                                            IFNULL(STF.STAFF_CODE, '')) AS FIRST_NAME,
                                    STF.STAFF_ID
                                    
                                FROM
                                    USER_ACCOUNT AS UA
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = UA.USER_ID
                                        AND STF.IS_LEFT != 1
                                        AND STF.IS_DELETED != 1
                                        INNER JOIN
									user_roles_by_academic_year AS AY ON AY.USER_ID=UA.USER_ID
                                  AND AY.ACADEMIC_YEAR=?AC_YEAR
								    AND AY.ROLE_ID=6
                                   AND AY.USER_TYPE='4'
                                  AND AY.IS_DELETED != 1

                                WHERE
                                      UA.IS_DELETED != 1 GROUP BY AY.USER_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHodByProgrammeGroupId:
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
								  AND AY.ROLE_ID=7
                                   AND AY.USER_TYPE='4'
                                  AND AY.IS_DELETED != 1
                                    WHERE
                                        CPG.PROGRAMME_GROUP_ID =?PROGRAMME_GROUP_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        GROUP BY STF.STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFbFacultyAppraisalType:
                    {
                        query = @"SELECT 
                                    APPRAISAL_TYPE,
                                    FACULTY_SELF_APPRAISAL_TYPE_ID
                                FROM
                                    SUP_FBFACULTY_SELF_APPRAISAL_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMonthWithYearAndAutoNumber:
                    {
                        //query = @"SELECT 
                        //                DAY_ID, 
                        //                DAY_ORDER_MONTH, 
                        //                IS_ACTIVE
                        //            FROM
                        //                (SELECT 
                        //                    CONCAT(DAY_ORDER_MONTH, ' - ', DATE_FORMAT(DAY_ORDER_DATE, '%Y')) AS DAY_ORDER_MONTH,
                        //                        IS_ACTIVE,DAY_ID
                        //                FROM
                        //                    CALENDER
                        //                WHERE
                        //                    ACADEMIC_YEAR = ?AC_YEAR
                        //                        AND DAY_ORDER_MONTH IN(DATE_FORMAT(CURRENT_DATE() - INTERVAL 5 DAY,'%M'),DATE_FORMAT(CURDATE(), '%M'))
                        //                GROUP BY DAY_ORDER_MONTH
                        //                ORDER BY DAY_ID) AS T
                        //            WHERE
                        //                T.IS_ACTIVE = 1 ORDER BY T.DAY_ID;";
                        query = @"SELECT 
                                    IF(MONTH_NAME = 'June',
                                        1,
                                        IF(MONTH_NAME = 'July',
                                            2,
                                            IF(MONTH_NAME = 'August',
                                                3,
                                                IF(MONTH_NAME = 'September ',
                                                    4,
                                                    IF(MONTH_NAME = 'October',
                                                        5,
                                                        IF(MONTH_NAME = 'November',
                                                            6,
                                                            IF(MONTH_NAME = 'December',
                                                                7,
                                                                IF(MONTH_NAME = 'January',
                                                                    8,
                                                                    IF(MONTH_NAME = 'February',
                                                                        9,
                                                                        IF(MONTH_NAME = 'March',
                                                                            10,
                                                                            IF(MONTH_NAME = 'April',
                                                                                11,
                                                                                IF(MONTH_NAME = 'May',
                                                                                    12,'')))))))))))) DAY_ID,
                                                                            DAY_ORDER_MONTH,
                                                                            IS_ACTIVE
                                                                        FROM
                                                                            (SELECT 
                                                                                CONCAT(DAY_ORDER_MONTH) AS DAY_ORDER_MONTH,
                                                                               -- CONCAT(DAY_ORDER_MONTH, ' - ', DATE_FORMAT(DAY_ORDER_DATE, '%Y')) AS DAY_ORDER_MONTH,
                                                                                    IS_ACTIVE,DAY_ID,
                                                                                    DAY_ORDER_MONTH AS MONTH_NAME
                                                                            FROM
                                                                                CALENDER
                                                                            WHERE
                                                                                ACADEMIC_YEAR = ?AC_YEAR
                                                                               -- AND DAY_ID<=(SELECT DAY_ID FROM CALENDER WHERE DAY_ORDER_DATE=CURDATE())
                                                                            GROUP BY DAY_ORDER_MONTH
                                                                            ORDER BY DAY_ID) AS T
                                                                        WHERE
                                                                            T.IS_ACTIVE = 1
                                                                        ORDER BY T.DAY_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchMonthWithYearAndAutoNumberForReport:
                    {
                        query = @"SELECT 
                                       -- (SELECT (@n:=@n + 1)) AS DAY_ID, DAY_ORDER_MONTH
                                         '0' AS DAY_ID, DAY_ORDER_MONTH 
                                    FROM
                                        (SELECT 
                                            CONCAT(DAY_ORDER_MONTH, ' - ', DATE_FORMAT(DAY_ORDER_DATE, '%Y')) AS DAY_ORDER_MONTH
                                        FROM
                                            CALENDER
                                        WHERE
                                            ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY DAY_ORDER_MONTH
                                        ORDER BY DAY_ID) AS T;";
                        break;
                    }
                case SupportDataSQLCommands.DisableMonthForIQACEntry:
                    {
                        query = @"UPDATE CALENDER SET IS_ACTIVE=0 WHERE DAY_ORDER_MONTH=?DAY_ORDER_MONTH AND ACADEMIC_YEAR=?AC_YEAR AND DAY_ID!=0;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupProgrammeType:
                    {
                        query = @"SELECT 
                                        PROGRAMME_TYPE_ID, 
                                        PROGRAMME_TYPE
                                    FROM
                                        SUP_IQAC_PROGRAMME_TYPE
                                    WHERE
                                        IS_ACTIVE = 1 
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByAcyear:
                    {
                        query = @"SELECT 
                                                 SP.STUDENT_ID,
                                                 CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                                -- CONCAT(IFNULL(SP.FIRST_NAME,''),' ',IFNULL(SP.LAST_NAME,''),' - ',SP.ROLL_NO) AS LAST_NAME,
                                                 IFNULL(SP.ROLL_NO,'0') AS ROLL_NO,
                                                 IFNULL(SP.REGISTER_NO,'0')AS REGISTER_NO,
                                                 IFNULL(SP.ADMISSION_NO,'0') AS ADMISSION_NO,
                                                 D.DEPARTMENT,
                                                 C.CLASS_NAME
                                             FROM 
                                                 STU_CLASS AS SC
                                                    INNER JOIN
                                                 STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=SC.STUDENT_ID
                                                    AND SP.IS_DELETED!=1
                                                    AND SP.IS_LEFT!=1
                                                    INNER JOIN
                                                 CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
                                                   INNER JOIN
                                                 CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=C.DEPARTMENT
                                                 WHERE
                                                     SC.ACADEMIC_YEAR = ?AC_YEAR AND SC.IS_DELETED!=1;";
                        break;
                    }

                #region IQAC
                case SupportDataSQLCommands.FetchIQACQuestions:
                    {
                        query = @"SELECT 
                                    IQAC_QUESTIONS, IQAC_QUESTION_ID
                                FROM
                                    IQAC_QUESTIONS
                                WHERE
                                    IS_DELETED != 1
                                    AND IS_ACTIVE=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupIQACProjectType:
                    {
                        query = @"SELECT 
                                    PROJECT_TYPE_ID, PROJECT_TYPE
                                FROM
                                    sup_iqac_project_type
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupIQACTypeOfFunding:
                    {
                        query = @"SELECT 
                                    TYPE_FUNDING_ID, FUNDING_TYPE
                                FROM
                                    sup_iqac_type_of_funding
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupIQACResearchGuidenceCategory:
                    {
                        query = @"SELECT 
                                    IQAC_CATEGORY_ID, CATEGORY_NAME
                                FROM
                                    sup_iqac_research_guidence_category
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupIQACPhdGuidenceCategory:
                    {
                        query = @"SELECT 
                                    IQAC_CATEGORY_ID, CATEGORY_NAME
                                FROM
                                    sup_iqac_phd_guidence_category
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchIQACStatus:
                    {
                        query = @"SELECT 
                                        IQAC_STATUS_ID, 
                                        STATUS_NAME
                                    FROM
                                        SUP_IQAC_STATUS;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupIQACActivity:
                    {
                        query = @"SELECT 
                                        ACTIVITY_ID,
                                        ACTIVITY_NAME
                                    FROM
                                        SUP_IQAC_ACTIVITY;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupStaffAssociation:
                    {
                        query = @"SELECT 
                                        STAFF_ASSOCIATION_ID,
                                        STAFF_ID,
                                        DEPARTMENT_ID,
                                        SS.SHIFT_ID,
                                        SS.SHIFT_NAME
                                    FROM
                                        SUP_STAFF_ASSOCIATION AS SA
                                            INNER JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = SA.SHIFT_ID
                                    WHERE
                                        SA.STAFF_ID = ?STAFF_ID AND SA.IS_DELETED!=1;";
                        break;
                    }
                #endregion

                #region Calender Events


                case SupportDataSQLCommands.FetchLastSevenCalenderEventsByCurDate:
                    {
                        query = @"SELECT 
                                        C.DAY_ID,
                                        DATE_FORMAT(C.DAY_ORDER_DATE, '%d/%m/%Y') AS EVENT_START_DATE,
                                        C.EVENT AS EVENT_NAME
                                    FROM
                                        CALENDER AS C
                                    WHERE
                                        DATE_FORMAT(C.DAY_ORDER_DATE, '%Y-%m-%d') >= DATE_FORMAT(CURDATE(), '%Y-%m-%d')
                                    LIMIT 7;";
                        break;
                    }
                #endregion

                #region Notification
                case SupportDataSQLCommands.FetchSupNotificationLevel:
                    {
                        query = @"SELECT 
                            NOTIFICATION_LEVEL_ID, NOTIFICATION_LEVEL
                        FROM
                            SUP_NOTIFICATION_LEVEL
                        WHERE
                            IS_DELETED != 1;";
                        break;
                    }
                #endregion
                case SupportDataSQLCommands.FetchSupIQACGuest:
                    {
                        query = @"SELECT GUEST_ID,GUEST_NAME FROM  SUP_IQAC_GUEST WHERE IS_ACTIVE=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTeachingStaffOnly:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        -- D.DEPARTMENT,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LAST_NAME, ''),
                                                ' ( ',
                                                IFNULL(STAFF_CODE, ''),
                                                ' )  -  ',D.DEPARTMENT) AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO AS SP
		                                    INNER JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=SP.DEPARTMENT
                                    WHERE
                                        NON_TEACHING_CATEGORY != 1 AND SP.IS_DELETED !=1 AND SP.IS_LEFT !=1
                                    ORDER BY SP.DEPARTMENT;";
                        break;
                    }

                case SupportDataSQLCommands.FetchSupOrganizer:
                    {
                        query = @"SELECT 
                                        ORGANIZER_ID, 
                                        ORGANIZER_NAME
                                    FROM
                                        SUP_ORGANIZER;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffByDepartmentId:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                ' ( ',
                                                IFNULL(STAFF_CODE, ''),
                                                ' ) ') AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        DEPARTMENT = ?DEPARTMENT AND IS_DELETED!=1 AND IS_LEFT !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTeachingStaffOnlyByDepartment:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(LAST_NAME, ''),
                                                ' ( ',
                                                IFNULL(STAFF_CODE, ''),
                                                ' ) ') AS FIRST_NAME
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        DEPARTMENT IN (?DEPARTMENT_ID)
                                        AND NON_TEACHING_CATEGORY !=1
                                       --  AND IS_DELETED!=1
                                       ;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffUserDepartmentId:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        CONCAT(IFNULL(FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(LAST_NAME, ''),
                                                ' ( ',
                                                IFNULL(STAFF_CODE, ''),
                                                ' ) ') AS FIRST_NAME,
                                         D.DEPARTMENT
                                    FROM
                                        STF_PERSONAL_INFO AS SP
                                            INNER JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=SP.DEPARTMENT
                                    WHERE
                                        SP.DEPARTMENT IN (SELECT 
                                                DEPARTMENT
                                            FROM
                                                STF_PERSONAL_INFO
                                            WHERE
                                                STAFF_ID = ?STAFF_ID)
                                    AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseAndShiftByStaffId:
                    {
                        query = @"SELECT 
	                                COURSE_ROOT_ID,
                                    UPPER(CONCAT(IFNULL(COURSE_TITLE,''),' - ',IFNULL(COURSE_CODE,''),' ( SHIFT - ',CCS.SHIFT,' )')) AS COURSE_CODE,
                                    CCS.SHIFT AS SHIFT_ID
                                FROM
                                    CP_CLASS_COURSE_STAFF AS CCS
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                        INNER JOIN
                                    SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID=CCS.SEMESTER_TYPE_ID
                                        AND ST.IS_ACTIVE=1
                                WHERE
                                    CCS.STAFF_ID = ?STAFF_ID
                                        AND CCS.IS_DELETED != 1
                                        AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY COURSE_ROOT_ID,CCS.SHIFT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSettingByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                        ES.EXAM_SETTING_ID,
                                        CONCAT(ES.EXAM_NAME, ' - ', ES.ACADEMIC_YEAR) AS EXAM_NAME
                                    FROM
                                        CP_PROGRAMME_GROUP AS PG
                                            INNER JOIN
                                        EXAM_PROGRAMME_TYPE_SETTING AS PT ON PT.PROGRAMME_TYPE_ID = PG.APPTYPE_ID
                                            INNER JOIN
                                        EXAM_SETTING AS ES ON ES.EXAM_SETTING_ID = PT.EXAM_SETTING_ID
		                                    AND ES.ACADEMIC_YEAR=?AC_YEAR
                                    WHERE
                                        ES.IS_ACTIVE = 1 AND PG.IS_DELETED != 1
                                            AND PG.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                            AND ES.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchuserAcInfo:
                    {
                        query = @"SELECT USER_ACCOUNT_ID,USERNAME,PASSWORD FROM USER_ACCOUNT WHERE USERNAME=?USERNAME;";
                        break;
                    }

                #region dashboard
                case SupportDataSQLCommands.FetchTotalStudentCountByGenderWise:
                    {
                        query = @"SELECT 
                                         SUM(IF(SP.GENDER = 2,1,0)) AS TOTAL_FEMALE_STUDENT,
                                        SUM(IF(SP.GENDER = 1,1,0)) AS TOTAL_MALE_STUDENT
                                         
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED!=1
		                                    AND SP.IS_LEFT!=1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                #endregion
                case SupportDataSQLCommands.FetchStudentByExamSettingId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO, 
                                        DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') as DATE_OF_BIRTH,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'',IFNULL(SP.LAST_NAME,'')) AS LAST_NAME,
                                        PHOTO
                                    FROM
                                        EXAM_REGISTRATION_?AC_YEAR AS ER
		                                    INNER JOIN
	                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID=ER.STUDENT_ID
		                                    AND SP.IS_DELETED!=1
                                    WHERE
                                        ER.EXAM_SETTING_ID = ?EXAM_SETTING_ID AND ER.IS_DELETED!=1
                                        GROUP BY SP.STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffInfoById:
                    {
                        query = @"SELECT 
                                        STAFF_ID,
                                        STAFF_CODE,
                                        FIRST_NAME,
                                        DATE_OF_BIRTH,
                                        DEPARTMENT,
                                        PHOTO,
                                        SHIFT
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentById:
                    {
                        query = @"SELECT 
                                        D.DEPARTMENT_ID,
	                                    D.DEPARTMENT
                                    FROM
                                        CP_DEPARTMENT AS D
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.DEPARTMENT = D.DEPARTMENT_ID
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        SP.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffClassByFeedBackSettingId:
                    {
                        query = @"SELECT 
                                        CS.CLASS_ID, CLS.CLASS_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CS
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CS.CLASS_ID
                                            AND CLS.IS_DELETED != 1
                                            INNER JOIN
										FBFEEDBACK_SETTINGS AS FS ON FS.SEMESTER_TYPE_ID=CS.SEMESTER_TYPE_ID
											AND FS.IS_DELETED!=1
                                            AND FS.FEEDBACK_ID=?FEEDBACK_ID
                                    WHERE
                                        CS.STAFF_ID = ?STAFF_ID
                                            AND CS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CS.IS_DELETED != 1
                                    GROUP BY CS.CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffCourseByFeedBackSettingId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(CR.COURSE_CODE,' - ',CR.COURSE_TITLE)) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = CC.CLASS_ID
                                            INNER JOIN
										FBFEEDBACK_SETTINGS AS FS ON FS.SEMESTER_TYPE_ID=CC.SEMESTER_TYPE_ID
											AND FS.IS_DELETED!=1
                                            AND FS.FEEDBACK_ID=?FEEDBACK_ID
                                    WHERE
                                        CC.CLASS_ID = ?CLASS_ID AND CC.STAFF_ID = ?STAFF_ID
                                            AND CC.IS_DELETED != 1
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }

                #region SJC CIA Cumulative For COE
                case SupportDataSQLCommands.FetchCourseByClassIdAndSemesterId:
                    {
                        query = @"SELECT 
                                        CCS.CLASS_ID,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        CCR.UGC_COURSE_TYPE,
                                        CCR.PROGRAMME_LEVEL_ID,
                                        CCS.CLASS_NAME
                                    FROM
                                        CP_COURSE_PATTERN AS CCP
                                            INNER JOIN
                                        CP_CLASSES AS CCS ON CCS.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                            AND CCS.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CCR.SEMESTER_ID IN (?SEMESTER_ID) 
                                            INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=CCS.PROGRAMME_GROUP_ID
											AND SEM.SEMESTER=CCR.SEMESTER_ID
                                            AND SEM.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                            INNER JOIN
										ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR=CCS.CLASS_YEAR
											AND AB.ACADEMIC_YEAR_ID=(SELECT ACADEMIC_YEAR_ID FROM ACADEMIC_YEAR WHERE AC_YEAR=?AC_YEAR)
                                            AND AB.BATCH_ID=SEM.BATCH
                                    WHERE
                                        CCP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                    GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PART;";
                        break;
                    }

                case SupportDataSQLCommands.FetchCourseByYearAndClassAndSemesterId:
                    {
                        query = @"SELECT 
                                        CCS.CLASS_ID,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        CCR.UGC_COURSE_TYPE,
                                        CCR.PROGRAMME_LEVEL_ID
                                    FROM
                                        CP_COURSE_PATTERN AS CCP
                                            INNER JOIN
                                        CP_CLASSES AS CCS ON CCS.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                           -- AND CCS.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CCR.SEMESTER_ID IN (?SEMESTER_ID) 
                                            INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=CCS.PROGRAMME_GROUP_ID
											AND SEM.SEMESTER=CCR.SEMESTER_ID
                                            AND SEM.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                            INNER JOIN
										ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR=CCS.CLASS_YEAR
											AND AB.ACADEMIC_YEAR_ID=(SELECT ACADEMIC_YEAR_ID FROM ACADEMIC_YEAR WHERE AC_YEAR=?AC_YEAR)
                                            AND AB.BATCH_ID=SEM.BATCH
                                    WHERE
                                        CCP.ACADEMIC_YEAR = ?AC_YEAR
									    AND CCS.CLASS_LEVEL= ?APPLICATION_TYPE_ID
                                        AND CCS.CLASS_YEAR= ?CLASS_YEAR
                                            AND CCP.IS_DELETED != 1
                                    GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PART;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByYearAndClassIdAndSemesterId:
                    {
                        query = @"SELECT 
                                        CCS.CLASS_ID,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        CCR.UGC_COURSE_TYPE,
                                        CCR.PROGRAMME_LEVEL_ID
                                    FROM
                                        CP_COURSE_PATTERN AS CCP
                                            INNER JOIN
                                        CP_CLASSES AS CCS ON CCS.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                           -- AND CCS.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CCR.SEMESTER_ID IN (?SEMESTER_ID) 
                                            INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=CCS.PROGRAMME_GROUP_ID
											AND SEM.SEMESTER=CCR.SEMESTER_ID
                                            AND SEM.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                            INNER JOIN
										ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR=CCS.CLASS_YEAR
											AND AB.ACADEMIC_YEAR_ID=(SELECT ACADEMIC_YEAR_ID FROM ACADEMIC_YEAR WHERE AC_YEAR=?AC_YEAR)
                                            AND AB.BATCH_ID=SEM.BATCH
                                    WHERE
                                        CCP.ACADEMIC_YEAR = ?AC_YEAR
									    AND CCS.CLASS_LEVEL= ?APPLICATION_TYPE_ID
                                        AND CCS.CLASS_YEAR= ?CLASS_YEAR
                                            AND CCP.IS_DELETED != 1
                                    GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PART;";
                        break;
                    }
                #endregion




                #region SJC CIA fail list For COE
                case SupportDataSQLCommands.FetchCourseByandClassIdAndSemesterId:
                    {
                        query = @"SELECT 
                                        CCS.CLASS_ID,
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID,
                                        CCR.UGC_COURSE_TYPE,
                                        CCR.PROGRAMME_LEVEL_ID
                                    FROM
                                        CP_COURSE_PATTERN AS CCP
                                            INNER JOIN
                                        CP_CLASSES AS CCS ON CCS.PROGRAMME_GROUP_ID = CCP.PROGRAMME_GROUP_ID
                                            AND CCS.CLASS_ID = ?CLASS_ID
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCP.COURSE_ID
                                            AND CCR.SEMESTER_ID IN (?SEMESTER_ID) 
                                            INNER JOIN
										ACADEMIC_SEMESTER AS SEM ON SEM.PROGRAMME_GROUP_ID=CCS.PROGRAMME_GROUP_ID
											AND SEM.SEMESTER=CCR.SEMESTER_ID
                                            AND SEM.ACADEMIC_YEAR=CCP.ACADEMIC_YEAR
                                            AND SEM.IS_DELETED!=1
                                            INNER JOIN
										ACADEMIC_BATCHES AS AB ON AB.BATCH_YEAR=CCS.CLASS_YEAR
											AND AB.ACADEMIC_YEAR_ID=(SELECT ACADEMIC_YEAR_ID FROM ACADEMIC_YEAR WHERE AC_YEAR=?AC_YEAR)
                                            AND AB.BATCH_ID=SEM.BATCH
                                    WHERE
                                        CCP.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCP.IS_DELETED != 1
                                    GROUP BY CCR.COURSE_ROOT_ID
                                    ORDER BY CCR.PART;";
                        break;
                    }

                #endregion

                #region Gate Fee Entry
                case SupportDataSQLCommands.FetchStudentByRollNoForGateFee:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SC.CLASS_ID AS CLASS,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        FM.FREQUENCY_ID,
                                        FM.HEAD_ID,
                                        FM.FEE_MAIN_HEAD_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS PG ON PG.PROGRAMME_GROUP_ID = SP.PROGRAM_ID
                                            INNER JOIN
                                        SUP_FEE_FREQUENCY AS FF ON FF.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND FF.SEMESTER_TYPE IN (SELECT 
                                                SEMESTER_TYPE_ID
                                            FROM
                                                SEMESTER_TYPE
                                            WHERE
                                                IS_ACTIVE = 1)
                                            INNER JOIN
                                        FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FF.FREQUENCY_ID
                                            AND FFF.FEE_MODE = ?FEE_MODE
                                            AND FFF.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            INNER JOIN
                                        FEE_MAIN_HEADS AS FM ON FM.APPLICATION_TYPE_ID = PG.APPTYPE_ID
                                            AND FM.FREQUENCY_ID = FFF.FEE_FREQUENCY_FEE_MODE_ID
                                            AND FM.SHIFT = PG.SHIFT
                                            AND FM.ACADEMIC_YEAR = SC.ACADEMIC_YEAR
                                            AND FM.IS_DELETED != 1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SP.ROLL_NO = ?ROLL_NO
                                            AND SC.IS_DELETED != 1;";
                        break;
                    }
                #endregion



                case SupportDataSQLCommands.FetchSupExternalCalculation:
                    {
                        query = @"SELECT 
                                    EXTERNAL_CALCULATION_ID,EXTERNAL_CALCULATION,PROGRAMME_LEVEL_ID,COURSE_TYPE
                                FROM
                                    SUP_EXTERNAL_CALCULATION
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByStaffIdAndSemesterTypeId:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        UPPER(CONCAT(IFNULL(COURSE_TITLE, ''),
                                                        ' - ',
                                                        IFNULL(COURSE_CODE, ''))) AS COURSE_TITLE
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CCS
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                    WHERE
                                        CCS.STAFF_ID = ?STAFF_ID
                                            AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            AND CCS.IS_DELETED != 1
                                            AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchRackInfo:
                    {
                        query = @"SELECT 
                                        RACK_INFO_ID,
                                        CONCAT(IFNULL(RACK_INFO, ''),
                                                ' ( ',
                                                IFNULL(RACK_NAME, ''),
                                                ' - ',
                                                IFNULL(C.CELL_NAME, ''),
                                                ' )') AS RACK_INFO
                                    FROM
                                        LI_RACK_INFO AS RI
                                            INNER JOIN
                                        SUP_LI_CELLS AS C ON C.CELL_ID = RI.CELL_ID
                                            INNER JOIN
                                        SUP_LI_RACKS AS R ON R.RACK_ID = RI.RACK_ID
                                    WHERE
                                        RI.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiBookStatus:
                    {
                        query = @"SELECT 
                                        STATUS_ID,
                                        STATUS_NAME
                                    FROM
                                        SUP_LI_BOOK_STATUS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiBookInvoice:
                    {
                        query = @"SELECT 
	                                INVOICE_ID,
                                    CONCAT('Invoice No : ',IFNULL(INVOICE_NO,''),' || Vendor : ',
                                            IFNULL(VENDOR,''),' || No Of Book : ',
                                            IFNULL(NO_OF_BOOK,''),' || Place : ',
                                            IFNULL(PLACE,'')) AS VENDOR
                                FROM
                                    SUP_LI_BOOK_INVOICE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiTaps:
                    {
                        query = @"SELECT 
                                        TAPS_ID,
                                        CONCAT(PREFIX,' - ',TAPS_NAME)AS TAPS_NAME
                                    FROM
                                        SUP_LI_TAPS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiDocCategory:
                    {
                        query = @"SELECT 
                                        DOC_CATEGORY_ID,
                                        CONCAT(DOC_CATEGORY_CODE,' - ',DOC_CATEGORY)AS DOC_CATEGORY
                                    FROM
                                        SUP_LI_DOC_CATEGORY
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiBookType:
                    {
                        query = @"SELECT 
                                        TYPE_ID,
                                        CONCAT(PREFIX,' - ',BOOK_TYPE)AS BOOK_TYPE
                                    FROM
                                        SUP_LI_BOOK_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByApplicationtypeId:
                    {
                        query = @"SELECT 
                                COURSE_ROOT_ID AS COURSE_ID,
                                CONCAT('(', COURSE_CODE, ')-', COURSE_TITLE) AS COURSE_TITLE
                            FROM
                                CP_COURSE_ROOT
                            WHERE
                                PROGRAMME_LEVEL_ID =?PROGRAMME_LEVEL_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchQuestionLevel:
                    {
                        query = @"SELECT 
                                QUESTION_LEVEL_ID, QUESTION_LEVEL, IS_DELETED, IS_ACTIVE
                            FROM
                                OBE_QUESTION_LEVEL
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByExamId:
                    {
                        query = @"SELECT 
                                    CR.COURSE_ROOT_ID AS COURSE_ID,
                                    CONCAT('(',
                                            CR.COURSE_CODE,
                                            ')-',
                                            CR.COURSE_TITLE) AS COURSE_TITLE
                                FROM
                                    OBE_QUESTIONS AS Q
                                        INNER JOIN
                                    CP_COURSE_ROOT AS CR ON Q.COURSE_ID = CR.COURSE_ROOT_ID
                                WHERE
                                    Q.EXAM_ID =?EXAM_ID
                                GROUP BY Q.COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamdetails:
                    {
                        query = @"SELECT 
                                EXAM_SETTING_ID, EXAM_NAME, ACADEMIC_YEAR
                            FROM
                                EXAM_SETTING
                            WHERE
                                EXAM_SETTING_ID =?EXAM_SETTING_ID AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.AddBookType:
                    {
                        query = @"INSERT INTO SUP_LI_BOOK_TYPE(PREFIX,BOOK_TYPE)VALUES(?PREFIX,?BOOK_TYPE);";
                        break;
                    }
                case SupportDataSQLCommands.UpdateBookType:
                    {
                        query = @"UPDATE SUP_LI_BOOK_TYPE SET PREFIX=?PREFIX,BOOK_TYPE=?BOOK_TYPE WHERE TYPE_ID=?TYPE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSearch:
                    {
                        query = @"SELECT 
                                        SEARCH_TEXT_ID, 
                                        FIELD_NAME, 
                                        SEARCH_TEXT
                                    FROM
                                        SUP_LI_SEARCH_TEXT
                                    WHERE
                                        IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByUgcTypeId:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        CONCAT(IFNULL(COURSE_TITLE, ''),
                                                '  -  ',
                                                IFNULL(COURSE_CODE, '')) AS COURSE_CODE,
                                        COURSE_CODE AS COURSE_TITLE,
                                        SEMESTER_ID
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        IS_DELETED != 1  AND UGC_COURSE_TYPE=?UGC_COURSE_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammmodeandShift:
                    {
                        query = @"SELECT 
                                CPG.PROGRAMME_MODE, CPG.SHIFT
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                            WHERE
                                SC.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SC.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoForProfile:
                    {
                        query = @"SELECT 
                                C.CLASS_NAME,
                                SP.STUDENT_ID,
                                SP.STU_MOBILENO,
                                SP.ROLL_NO,
                                SP.REGISTER_NO,
                                SP.ADMISSION_NO,
                                CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                        ' ',
                                        IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                SP.FATHER_NAME,
                                SA.ADDRESS,
                                SP.PHOTO
                            FROM
                                STU_CLASS AS SC
                                    INNER JOIN
                                STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                    AND SP.IS_DELETED != 1
                                    INNER JOIN
                                CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    LEFT JOIN
                                STU_ADDRESS AS SA ON SA.STUDENT_ID = SC.STUDENT_ID
                            WHERE
                                SC.IS_DELETED != 1
                                    AND SC.ACADEMIC_YEAR =?AC_YEAR
                                    AND SP.STUDENT_ID =?STUDENT_ID
                            ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentByStaffId:
                    {
                        query = @"SELECT 
                                DEPARTMENT
                            FROM
                                STF_PERSONAL_INFO
                            WHERE
                                STAFF_ID = ?STAFF_ID AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupHalls:
                    {
                        query = @"SELECT 
                                    SH.HALL_ID, SH.HALL_NAME
                                FROM
                                    SUP_HALLS AS SH
                                WHERE
                                    SH.IS_DELETED != 1";
                        break;
                    }
                case SupportDataSQLCommands.FetchHallreservationstatus:
                    {
                        query = @"SELECT 
                                STATUS_ID, STATUS_NAME
                            FROM
                                SUP_HALL_RESERVATION_STATUS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffInfo:
                    {
                        query = @"SELECT 
                                IFNULL(SA.CCELL_NO, IFNULL(SA.PCELL_NO, '')) AS MOBILE
                            FROM
                                STF_PERSONAL_INFO AS SPI
                                    LEFT JOIN
                                STF_ADDRESS AS SA ON SPI.STAFF_ID = SA.STAFFNO
                            WHERE
                                STAFF_ID =?STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchHallreservationstatusById:
                    {
                        query = @"SELECT 
                                    STATUS_NAME
                                FROM
                                    SUP_HALL_RESERVATION_STATUS
                                WHERE
                                    STATUS_ID = ?STATUS_ID AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchVisitorCategory:
                    {
                        query = @"SELECT 
                                VISITORS_CATEGORY_ID, VISITORS_CATEGORY_NAME
                            FROM
                                SUP_VISITORS_CATEGORY
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchVisitorsstatus:
                    {
                        query = @"SELECT 
                                STATUS_ID, STATUS_NAME
                            FROM
                                SUP_VISITORS_STATUS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupRoleByLikeKeyword:
                    {
                        query = @"SELECT 
                                        ROLE_ID,ROLE_NAME 
                                 FROM 
                                    SUP_ROLE 
                                 WHERE ROLE_NAME LIKE '%?ROLE_NAME%';";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupUserTypeByLikeKeyword:
                    {
                        query = @"SELECT 
                                        USER_TYPE_ID, USER_TYPE_NAME
                                    FROM
                                        SUP_USER_TYPE
                                    WHERE
                                        USER_TYPE_NAME LIKE '%?USER_TYPE_NAME%';";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentByUserId:
                    {
                        query = @"SELECT 
                                        DEPARTMENT_ID, DP.DEPARTMENT
                                    FROM
                                        STF_PERSONAL_INFO AS ST
                                            LEFT JOIN
                                        CP_DEPARTMENT AS DP ON DP.DEPARTMENT_ID = ST.DEPARTMENT
                                    WHERE
                                        ST.STAFF_ID = ?STAFF_ID AND ST.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentCourseListbyStaffId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CONCAT('( ',CCR.COURSE_CODE,' )  -  ( SEM - ',SEMESTER_ID,' )  ',CCR.COURSE_TITLE) AS COURSE_TITLE
                                    FROM
                                        CP_COURSE_ROOT AS CCR
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.DEPARTMENT = CCR.DEPARTMENT
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        CCR.IS_DELETED != 1 AND SP.STAFF_ID = ?STAFF_ID
                                    ORDER BY CCR.SEMESTER_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllCoursesByDepartmnetId:
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
                                    WHERE
                                         CCR.IS_DELETED != 1
                                            AND CCR.DEPARTMENT=?DEPARTMENT ";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllCoursesByDepartIdandAcyear:
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
                                      cp_department AS cd ON cd.DEPARTMENT_ID = ccr.DEPARTMENT
                                          INNER JOIN
                                      cp_programme AS cp ON cp.DEPARTMENT = CCR.DEPARTMENT
                                          INNER JOIN
                                      cp_programme_group AS cpg ON cpg.PROGRAMME_ID = cp.PROGRAMME_ID
                                          INNER JOIN
                                      cp_course_pattern AS ccp ON ccp.course_id = ccr.course_root_id
                                          AND ccp.IS_DELETED != 1
                                          AND ccp.ACADEMIC_YEAR = ?AC_YEAR
                                          AND ccp.programme_group_id = cpg.programme_group_id
                                  WHERE
                                      CCR.IS_DELETED != 1
                                          AND CCR.DEPARTMENT = ?DEPARTMENT;";
                        break;
                    }



                case SupportDataSQLCommands.FetchValueeducationHoursByStaffIdAndActiveSemester:
                    {
                        query = @"SELECT 
                                SH.HOUR_ID,
                                CONCAT(IFNULL(SH.HOUR, ''),
                                        ' (',
                                        IFNULL(ST.HOURTYPE, ''),
                                        ')') AS HOUR,
                                TT.COURSE_ID,
                                CR.SEMESTER_ID
                            FROM
                                TT_TIMETABLE AS TT
                                    INNER JOIN
                                SUP_HOURS AS SH ON SH.HOUR_ID = TT.HOUR_ID
                                    INNER JOIN
                                SUP_HOUR_TYPE AS ST ON ST.HOUR_TYPE_ID = TT.HOUR_TYPE
                                    INNER JOIN
                                CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = TT.COURSE_ID 
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                    AND CCS.COURSE_ID = TT.COURSE_ID
                                    AND CCS.CLASS_ID = TT.CLASS_ID
                                    AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                    AND CCS.IS_DELETED != 1
                                    INNER JOIN
                                SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                    AND S.IS_ACTIVE = 1
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                    AND CC.SHIFT = ?SHIFT
                            WHERE
		                            TT.STAFF_ID = ?STAFF_ID
                                    AND TT.ACADEMIC_YEAR = ?AC_YEAR
		                            AND CR.PAPER_TYPE=?PAPER_TYPE
                                    AND TT.IS_DELETED != 1
                                    AND TT.DAY_ORDER_ID = (SELECT 
                                        DAY_ORDER
                                    FROM
                                        CALENDER
                                    WHERE
                                        DAY_ORDER_DATE = CURDATE()
                                            AND ACADEMIC_YEAR = ?AC_YEAR)
                            GROUP BY SH.HOUR_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedbackSetting:
                    {
                        query = @"SELECT 
                                        FEEDBACK_NAME, 
                                        FEEDBACK_ID
                                    FROM
                                        FBFEEDBACK_SETTINGS
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedbackSettingList:
                    {
                        query = @"SELECT 
                                        FEEDBACK_NAME, 
                                        FEEDBACK_ID
                                    FROM
                                        FBFEEDBACK_SETTINGS
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentFeedbackSettingList:
                    {
                        query = @"SELECT 
                                        FEEDBACK_NAME, 
                                        FEEDBACK_ID
                                    FROM
                                        FBFEEDBACK_SETTINGS
                                    WHERE
                                        IS_DELETED != 1 AND FEEDBACK_TYPE= 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSettingByExamTypeId:
                    {
                        query = @"SELECT 
                                        EXAM_SETTING_ID,
                                        EXAM_NAME
                                    FROM
                                        EXAM_SETTING
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR 
                                        AND EXAM_TYPE = ?EXAM_TYPE;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupervisionStafftype:
                    {
                        query = @"SELECT 
                                    SUP_SUPERVISION_TYPE_ID,
                                    SUPERVISION_STAFF_TYPE
                                FROM
                                    SUP_SUPERVISON_STAFF_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchRegularclass:
                    {
                        query = @"SELECT 
                                CLASS_ID,
                                CLASS_NAME
                            FROM
                                CP_CLASSES CLS
                                    INNER JOIN
                                SUP_SHIFT AS SH ON SH.SHIFT_ID = CLS.SHIFT
                            WHERE
                                CLS.IS_DELETED != 1 AND CLS.CLASS_TYPE=2;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffCategory:
                    {
                        query = @"SELECT 
                                STF_CATEGORY_ID, STF_CATEGORY
                            FROM
                                SUP_STAFF_CATEGORY
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSalaryCompoenttype:
                    {
                        query = @"SELECT 
                                SALARY_COMPONENT_TYPE_ID, SALARY_COMPONENT_NAME
                            FROM
                                SUP_SALARY_COMPONENT_TYPE
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.Fetchpayrollcomponents:
                    {
                        query = @"SELECT 
                                COMPONENT_ID, COMPONENT_NAME
                            FROM
                                PR_SALARY_COMPONENT_DETAILS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchTemplates:
                    {
                        query = @"SELECT 
                                TEMPLATE_ID, TEMPLATE_NAME
                            FROM
                                PR_SALARY_TEMPLATE_DETAILS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLeaveType:
                    {
                        query = @"SELECT 
                                    LEAVE_TYPE_ID, LEAVE_TYPE
                                FROM
                                    SUP_LEAVE_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllEmployee:
                    {
                        query = @"SELECT 
                                STAFF_ID AS EMPLOYEE_ID, CONCAT(FIRST_NAME, '-', STAFF_CODE) AS FIRST_NAME
                            FROM
                                STF_PERSONAL_INFO
                            WHERE
                                IS_DELETED != 1 AND IS_LEFT != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchIncrementTypes:
                    {
                        query = @"SELECT 
                                    SALARY_INCREMENT_TYPE_ID, INCREMENT_TYPE_NAME
                                FROM
                                    PR_SUP_SALARY_INCREMENT_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllPFComponents:
                    {
                        query = @"SELECT 
                                PF_COMPONENT_ID, PF_COMPONENT_NAME
                            FROM
                                PR_PF_COMPONENTS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentMandatoryFields:
                    {
                        query = @"SELECT 
                                    STU_MANDATORY_ID, MANDATORY_FIELDS, VISIBILE_NAME
                                FROM
                                    STU_MANDATORY_FIELDS
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchSupUnit:
                    {
                        query = @"SELECT 
                                        UNIT_ID, 
                                        UNIT_NAME
                                    FROM
                                        SUP_UNIT
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupSection:
                    {
                        query = @"SELECT 
                                        SECTION_ID, 
                                        SECTION
                                    FROM
                                        SUP_SECTION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchIncrementStatus:
                    {
                        query = @"SELECT 
                                STATUS_ID, STATUS_NAME
                            FROM
                                PR_SUP_INCREMENT_STATUS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchsupApprovallevel:
                    {
                        query = @"SELECT 
                            LEVEL_ID, LEVEL_NAME
                        FROM
                            LM_SUP_APPROVAL_LEVEL
                        WHERE
                            IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupQuestionLevel:
                    {
                        query = @"SELECT 
                                        QUESTION_LEVEL_ID, 
                                        QUESTION_LEVEL
                                    FROM
                                        SUP_QUESTION_LEVEL;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSettingByAcYear:
                    {
                        query = @"SELECT 
                                        EXAM_SETTING_ID,
                                        UPPER(EXAM_NAME) AS EXAM_NAME
                                    FROM
                                        EXAM_SETTING AS ES
                                    WHERE
                                        ES.ACADEMIC_YEAR = ?AC_YEAR AND ES.IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupEvaluation:
                    {
                        query = @"SELECT EVALUATION_ID, EVALUATION_NAME FROM SUP_EVALUATION WHERE IS_DELETE!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupStatus:
                    {
                        query = @"SELECT 
                                    STATUS_ID, STATUS as STATUS_NAME
                                FROM
                                    SUP_STATUS
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByOptionalClassId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        CCR.SEMESTER_ID
                                    FROM
                                        CP_CLASSES AS CC
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID=CC.COURSE_ID
                                    WHERE
                                        CC.CLASS_ID = ?CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchQuestionpattern:
                    {
                        query = @"SELECT 
                                QUESTION_PATTERN_ID, PATTERN_NAME
                            FROM
                                EXAM_QUESTION_PATTERN WHERE IS_DELETED!=1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchSupEvaluationType:
                    {
                        query = @"SELECT 
                                        EXAM_EVALUATION_TYPE_ID, EXAM_EVALUATION
                                    FROM
                                        EXAM_SUP_EVALUATION_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupQuizType:
                    {
                        query = @"SELECT 
                                        QUIZ_TYPE_ID,
                                        QUIZ_TYPE_NAME
                                    FROM
                                        SUP_QUIZ_TYPE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupEducationBoard:
                    {
                        query = @"SELECT 
                                        EDUCATION_BOARD_ID, EDUCATION_BOARD
                                    FROM
                                       sup_education_board
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOnlineExamSettingByAcYear:
                    {
                        query = @"SELECT 
                                OES.EXAM_SETTING_ID,
                                ONLINE_EXAM_SETTING_ID AS ONLINE_EXAM_SETTING_ID,
                                CONCAT(SETTING_NAME, '-', SQT.QUIZ_TYPE_NAME) AS EXAM_NAME,
                                OES.EXAM_TYPE
                            FROM
                                EXAM_ONLINE_EXAM_SETTING AS OES
                                    INNER JOIN
                                SUP_QUIZ_TYPE AS SQT ON SQT.QUIZ_TYPE_ID = OES.EXAM_TYPE
                            WHERE
                                OES.IS_DELETED != 1
                                    AND OES.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCertificatetype:
                    {
                        query = @"SELECT 
                                    CERTIFICATE_ID, CERTIFICATE_NAME
                                FROM
                                    SUP_CERTIFICATE_TYPE
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllthestudent:
                    {
                        query = @"SELECT 
                                REGISTER_NO
                            FROM
                                STU_PERSONAL_INFO
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchClassByStaffAcyearAndSemestertypeId:
                    {
                        query = @"SELECT 
                                        C.CLASS_ID,
                                        C.CLASS_NAME
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CCS
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = CCS.CLASS_ID
                                    WHERE
                                        CCS.IS_DELETED != 1
                                            AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            AND CCS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND CCS.STAFF_ID = ?STAFF_ID
	                                    GROUP BY CCS.CLASS_ID
                                    ORDER BY C.CLASS_LEVEL,C.CLASS_YEAR;";
                        break;
                    }

                case SupportDataSQLCommands.FetchCourseByClassIdStaffAcyearAndSemestertypeId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(CR.COURSE_CODE,' - ',CR.COURSE_TITLE)) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                    WHERE
                                       CC.CLASS_ID = ?CLASS_ID AND  CC.STAFF_ID = ?STAFF_ID
                                            AND CC.IS_DELETED != 1
											AND CC.SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByClassIdAcyearAndSemestertypeId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(CR.COURSE_CODE,' - ',CR.COURSE_TITLE)) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                    WHERE
                                       CC.CLASS_ID = ?CLASS_ID
                                            AND CC.IS_DELETED != 1
											AND CC.SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByClassIdAcyearAndSemestertypeId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(CR.COURSE_CODE,' - ',CR.COURSE_TITLE)) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                    WHERE
                                       CC.CLASS_ID = ?CLASS_ID
                                            AND CC.IS_DELETED != 1
											AND CC.SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentListbyShiftId:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        DATE_FORMAT(SP.DATE_OF_BIRTH, '%d/%m/%Y') AS DATE_OF_BIRTH,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.LAST_NAME, ''),
                                                ' ',
                                                IFNULL(SP.ROLL_NO, '')) AS FIRST_NAME,
                                        D.DEPARTMENT,
                                        C.CLASS_LEVEL AS PROGRAMME_LEVEL
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS D ON D.DEPARTMENT_ID = C.DEPARTMENT
                                    WHERE
                                        c.SHIFT in (?SHIFT)
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                    ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByDepartment:
                    {
                        query = @"SELECT 
                                    CLASS_ID,
                                    CLASS_NAME
                                FROM
                                    CP_CLASSES
                                WHERE
                                    DEPARTMENT IN (?DEPARTMENT) AND IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchObjectiveOptionIdByOptionForQuiz:
                    {
                        query = @"SELECT 
                                OPTION_ID, `OPTION`, QUIZ_ID
                            FROM
                                QUIZ_OBJECTIVE_OPTIONS
                            WHERE
                                `OPTION` = ?ANSWER AND QUIZ_ID = ?QUIZ_ID
                                    AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseInfoByCode:
                    {
                        query = @"SELECT 
                                        COURSE_ROOT_ID,
                                        CONCAT(IFNULL(COURSE_TITLE, ''),
                                                '  -  ',
                                                IFNULL(COURSE_CODE, '')) AS COURSE_CODE,
                                        COURSE_CODE AS COURSE_TITLE,
                                        SEMESTER_ID
                                    FROM
                                        CP_COURSE_ROOT
                                    WHERE
                                        IS_DELETED != 1 AND  COURSE_CODE=?COURSE_CODE;";
                        break;

                    }
                case SupportDataSQLCommands.FetchSupDesignation:
                    {
                        query = @"  SELECT 
                                        DESIGNATIONID, DESIGNATION
                                    FROM
                                        sup_designation
                                    WHERE
                                        ISDELETED != 1";
                        break;

                    }
                case SupportDataSQLCommands.FetchSupPhysicalStatus:
                    {
                        query = @"SELECT
                                PHYSICAL_STATUS_ID, PHYSICAL_STATUS_NAME
                            FROM
                                sup_physical_status
                            WHERE
                                IS_DELETED != 1;";
                        break;

                    }
                case SupportDataSQLCommands.FetchSupStaffStatus:
                    {
                        query = @"SELECT 
                                        STATUS_ID, STATUS
                                    FROM
                                        sup_staff_status
                                    WHERE
                                        IS_DELETED != 1;";
                        break;

                    }
                case SupportDataSQLCommands.FetchCourseListByFeedbackId:
                    {
                        query = @"SELECT 
                                        CCR.COURSE_ROOT_ID,
                                        CONCAT('( ',
                                                CCR.COURSE_CODE,
                                                ' )  -  ( SEM - ',
                                                SEMESTER_ID,
                                                ' )  ',
                                                CCR.COURSE_TITLE) AS COURSE_TITLE,
                                        cd.DEPARTMENT
                                    FROM
                                        FBEVALUATIONSYLLABUS AS fs
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.COURSE_ROOT_ID = fs.course_id
                                            AND ccr.DEPARTMENT = ?DEPARTMENT
                                            AND ccr.IS_DELETED != 1
                                            INNER JOIN
                                        cp_department AS cd ON cd.DEPARTMENT_ID = ccr.DEPARTMENT
                                        WHERE
                                            fs.FEEDBACK_ID = ?FEEDBACK_ID
                                                AND fs.ACADEMIC_YEAR = ?AC_YEAR
                                                AND fs.IS_DELETED != 1
                                        GROUP BY fs.COURSE_ID;";
                        break;

                    }
                case SupportDataSQLCommands.Fetchmalpratices:
                    {
                        query = @"SELECT 
                                                SPI.ADMISSION_NO,
                                                SPI.ROLL_NO,
                                                SPI.FIRST_NAME,
                                                CPC.CLASS_NAME,
                                                CR.COURSE_TITLE,
                                                CR.COURSE_CODE,
                                                count(sc.student_id)-1 as CHECKING

                                            FROM
                                                stu_class sc
                                                    INNER JOIN
                                                stu_personal_info spi ON sc.STUDENT_ID = spi.STUDENT_ID
                                                    INNER JOIN
                                                cp_classes CPC ON CPC.CLASS_ID = SC.CLASS_ID
                                                    INNER JOIN
                                                quiz_malpractice QI ON QI.STUDENT_ID = SC.STUDENT_ID and sc.ACADEMIC_YEAR=QI.ACADEMIC_YEAR
                                                    INNER JOIN
                                                cp_course_root CR ON CR.COURSE_ROOT_ID = QI.COURSE_ID
                                                where sc.IS_DELETED !=1
                                                and spi.IS_DELETED !=1
	                                            and sc.ACADEMIC_YEAR=?AC_YEAR
                                                and QI.IS_DELETED !=1
                                            GROUP BY SC.STUDENT_ID
                                            having checking!=0
                                            order by cpc.class_id,ROLL_NO ;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassListByDeptShitClevelCYear:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        DEPARTMENT in(?DEPARTMENT) 
		                                    AND SHIFT IN (?SHIFT)
		                                    AND CLASS_LEVEL IN (?CLASS_LEVEL)
		                                    AND CLASS_YEAR IN (?CLASS_YEAR)
                                            AND IS_DELETED != 1 and CLASS_TYPE=2
                                    ORDER BY  DEPARTMENT ,CLASS_LEVEL,SHIFT;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoForCertificate:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID,
                                        SP.GENDER,
                                        FIRST_NAME,
                                        CLASS_NAME,
                                        ROLL_NO,
                                        CP.PROGRAMME_NAME,
                                        SC.ACADEMIC_YEAR,
                                        CONCAT(SB.BATCH, '-', SB.BATCH + CC.CLASS_YEAR) AS BATCH
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
                                        sup_batches AS sb ON sb.BATCH_ID = sc.BATCH
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
	                                    AND SC.STUDENT_ID IN (?STUDENT_ID);";
                        break;
                    }
                #region Online Attendance
                case SupportDataSQLCommands.FetchHourByShiftIdForOnlineAttendance:
                    {
                        query = @"SELECT 
                                        SH.HOUR_ID,
                                        CONCAT(IFNULL(SH.HOUR, ''),
                                                ' (',
                                                IFNULL(ST.HOURTYPE, ''),
                                                ')') AS HOUR,
                                         TT.COURSE_ID,
                                         CR.SEMESTER_ID
                                    FROM
                                        TT_ONLINE_TIMETABLE AS TT
                                            INNER JOIN
                                        SUP_HOURS AS SH ON SH.HOUR_ID = TT.HOUR_ID
                                            INNER JOIN
                                        SUP_HOUR_TYPE AS ST ON ST.HOUR_TYPE_ID = TT.HOUR_TYPE
		                                    INNER JOIN
	                                    CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=TT.COURSE_ID
                                            INNER JOIN
                                        CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = TT.STAFF_ID
                                            AND CCS.COURSE_ID = TT.COURSE_ID
                                            AND CCS.CLASS_ID = TT.CLASS_ID
                                            AND CCS.ACADEMIC_YEAR = TT.ACADEMIC_YEAR
                                            AND CCS.IS_DELETED != 1
                                            INNER JOIN
                                        SEMESTER_TYPE AS S ON S.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                            AND S.IS_ACTIVE = 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID=CCS.CLASS_ID
                                            AND CC.SHIFT=?SHIFT
                                    WHERE
                                        TT.STAFF_ID = ?STAFF_ID
                                            AND TT.ACADEMIC_YEAR = ?AC_YEAR
                                            AND TT.IS_DELETED != 1
                                            AND TT.DAY_ORDER_ID = (SELECT 
                                                DAY_ORDER
                                            FROM
                                                CALENDER
                                            WHERE
                                                DAY_ORDER_DATE = CURDATE()
                                                    AND ACADEMIC_YEAR = ?AC_YEAR)
                                    GROUP BY SH.HOUR_ID;";
                        break;
                    }
                #endregion
                case SupportDataSQLCommands.FetchphdDepartment:
                    {
                        query = @"SELECT 
                                    DEPARTMENT_ID, DEPARTMENT
                                FROM
                                    SUP_PHD_DEPARTMENT
                                WHERE
                                    IS_DELETED != 1";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoForNoDUeForm:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID,
                                        FIRST_NAME,
                                        REGISTER_NO,
                                        CD.DEPARTMENT,
                                        ROLL_NO,
                                        SC.ACADEMIC_YEAR,
                                        SUM(IFNULL(TERMFEE1, 0)) - SUM(IFNULL(TERMFEE1PAID, 0)) + SUM(IFNULL(TERMFEE2, 0)) - SUM(IFNULL(TERMFEE2PAID, 0)) AS COLLEGE_FEE,
                                        SUM(IFNULL(hfs.CREDIT, 0)) - SUM(IFNULL(hfs.DEBIT, 0)) AS HOSTELFEEDUE,
                                        SUM(IFNULL(bfs.B_CREDIT, 0)) - SUM(IFNULL(bfs.B_DEBIT, 0)) AS BUSFEEDUE,
                                        SUM(IFNULL(EXAMFEE1, 0)) - SUM(IFNULL(EXAMFEE1PAID, 0)) + SUM(IFNULL(EXAMFEE2, 0)) - SUM(IFNULL(EXAMFEE2PAID, 0)) AS EXAMFEEDUE,
                                        HFS.HOSTEL_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                            LEFT JOIN
                                        stu_fee_due_info AS sfd ON sfd.STUDENT_ID = SP.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = sfd.ACADEMIC_YEAR and sfd.IS_DELETED !=1
                                            LEFT JOIN
                                        (SELECT 
                                            student_id,
                                                SUM(IFNULL(CREDIT, '0')) AS CREDIT,
                                                SUM(IFNULL(DEBIT, '0')) AS DEBIT,
                                                SUM(IFNULL(HOSTEL_ID, '0')) AS HOSTEL_ID
                                        FROM
                                            hostel_fee_student_account AS hs
                                        WHERE
                                            hs.IS_DELETED != 1
                                                AND ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY STUDENT_ID) AS hfs ON hfs.STUDENT_ID = sc.student_id
                                        LEFT JOIN
                                        (SELECT 
                                            student_id,
                                                SUM(IFNULL(CREDIT, '0')) AS B_CREDIT,
                                                SUM(IFNULL(DEBIT, '0')) AS B_DEBIT
                                        FROM
                                            bus_fee_student_account AS bs
                                        WHERE
                                            bs.IS_DELETED != 1
                                                AND bs.ACADEMIC_YEAR = ?AC_YEAR
                                        GROUP BY STUDENT_ID) AS bfs ON bfs.STUDENT_ID = sc.student_id
                                    WHERE
                                        SP.IS_DELETED != 1
                                            AND sc.IS_DELETED != 1
                                            -- AND SP.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoForNoDUeFormAdmin:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SP.FIRST_NAME,
                                    SP.REGISTER_NO,
                                    CD.DEPARTMENT,
                                    SP.ROLL_NO,
                                    SC.ACADEMIC_YEAR,
                                    (IFNULL(GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                                                    0) + GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                                                    0),
                                            0) + IFNULL(ARR.ARREAR_DUE, 0)) AS COLLEGE_FEE,
                                    IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEEDUE,
                                    IFNULL(BF.BUS_FEE, 0) AS BUSFEEDUE,
                                    IFNULL(EF.EXAM_FEE, 0) AS EXAMFEEDUE
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            TERMFEE1,
                                            TERMFEE1PAID,
                                            TERMFEE2,
                                            TERMFEE2PAID,
                                            EXAMFEE1,
                                            EXAMFEE1PAID,
                                            EXAMFEE2,
                                            EXAMFEE2PAID
                                    FROM
                                        STU_FEE_DUE_INFO
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED <> 1) CUR ON CUR.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                                    FROM
                                        STU_FEE_DUE_INFO
                                    WHERE
                                        ACADEMIC_YEAR < ?AC_YEAR AND IS_DELETED <> 1
                                    GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        FSA.STUDENT_ID,
                                            GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS HOSTEL_FEE
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                    INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    WHERE
                                        SFT.FREQUENCY_TYPE_ID = 4
                                            AND FSA.IS_DELETED <> 1
                                    GROUP BY FSA.STUDENT_ID) HF ON HF.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        FSA.STUDENT_ID,
                                            GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS BUS_FEE
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                    INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    WHERE
                                        SFT.FREQUENCY_TYPE_ID = 8
                                            AND FSA.IS_DELETED <> 1
                                    GROUP BY FSA.STUDENT_ID) BF ON BF.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        FSA.STUDENT_ID,
                                            CAST(GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS SIGNED) AS EXAM_FEE
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                    INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    INNER JOIN ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SFF.ACADEMIC_YEAR AND AY.ACTIVE_YEAR = 1 AND AY.IS_DELETED <> 1
                                    INNER JOIN SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE AND SST.IS_ACTIVE = 1 AND SST.IS_DELETED <> 1
                                    WHERE
                                        SFT.FREQUENCY_TYPE_ID = 1
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                    GROUP BY FSA.STUDENT_ID) EF ON EF.STUDENT_ID = SC.STUDENT_ID
                                WHERE
                                    SP.IS_DELETED <> 1
                                        AND SC.IS_DELETED <> 1
                                        AND SC.STUDENT_ID = ?STUDENT_ID
                                GROUP BY SP.STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentInfoForNoDUeFormNew:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SP.FIRST_NAME,
                                    SP.REGISTER_NO,
                                    CD.DEPARTMENT,
                                    SP.ROLL_NO,
                                    SC.ACADEMIC_YEAR,
                                    (IFNULL(GREATEST(IFNULL(CUR.TERMFEE1, 0) - IFNULL(CUR.TERMFEE1PAID, 0),
                                                    0) + GREATEST(IFNULL(CUR.TERMFEE2, 0) - IFNULL(CUR.TERMFEE2PAID, 0),
                                                    0),
                                            0) + IFNULL(ARR.ARREAR_DUE, 0)) AS COLLEGE_FEE,
                                    IFNULL(HF.HOSTEL_FEE, 0) AS HOSTELFEEDUE,
                                    IFNULL(BF.BUS_FEE, 0) AS BUSFEEDUE,
                                    IFNULL(EF.EXAM_FEE, 0) AS EXAMFEEDUE
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            TERMFEE1,
                                            TERMFEE1PAID,
                                            TERMFEE2,
                                            TERMFEE2PAID,
                                            EXAMFEE1,
                                            EXAMFEE1PAID,
                                            EXAMFEE2,
                                            EXAMFEE2PAID
                                    FROM
                                        STU_FEE_DUE_INFO
                                    WHERE
                                        ACADEMIC_YEAR = ?AC_YEAR AND IS_DELETED <> 1) CUR ON CUR.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        STUDENT_ID,
                                            SUM(GREATEST(IFNULL(TERMFEE1, 0) - IFNULL(TERMFEE1PAID, 0), 0) + GREATEST(IFNULL(TERMFEE2, 0) - IFNULL(TERMFEE2PAID, 0), 0)) AS ARREAR_DUE
                                    FROM
                                        STU_FEE_DUE_INFO
                                    WHERE
                                        ACADEMIC_YEAR < ?AC_YEAR AND IS_DELETED <> 1
                                    GROUP BY STUDENT_ID) ARR ON ARR.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        FSA.STUDENT_ID,
                                            GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS HOSTEL_FEE
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                    INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    WHERE
                                        SFT.FREQUENCY_TYPE_ID = 4
                                            AND FSA.IS_DELETED <> 1
                                    GROUP BY FSA.STUDENT_ID) HF ON HF.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        FSA.STUDENT_ID,
                                            GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS BUS_FEE
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                    INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    WHERE
                                        SFT.FREQUENCY_TYPE_ID = 8
                                            AND FSA.IS_DELETED <> 1
                                    GROUP BY FSA.STUDENT_ID) BF ON BF.STUDENT_ID = SC.STUDENT_ID
                                        LEFT JOIN
                                    (SELECT 
                                        FSA.STUDENT_ID,
                                            CAST(GREATEST(SUM(IFNULL(FSA.CREDIT, 0)) - SUM(IFNULL(FSA.DEBIT, 0)), 0) AS SIGNED) AS EXAM_FEE
                                    FROM
                                        FEE_STUDENT_ACCOUNT AS FSA
                                    INNER JOIN FEE_FREQUENCY_FEE_MODE AS FFF ON FFF.FREQUENCY_ID = FSA.FREQUENCY_ID
                                    INNER JOIN SUP_FEE_FREQUENCY AS SFF ON SFF.FREQUENCY_ID = FFF.FREQUENCY_ID
                                    INNER JOIN SUP_FREQUENCY_TYPE AS SFT ON SFT.FREQUENCY_TYPE_ID = FFF.FEE_MODE
                                    INNER JOIN ACADEMIC_YEAR AS AY ON AY.AC_YEAR = SFF.ACADEMIC_YEAR AND AY.ACTIVE_YEAR = 1 AND AY.IS_DELETED <> 1
                                    INNER JOIN SUP_SEMESTER_TYPE AS SST ON SST.SEMESTER_TYPE_ID = SFF.SEMESTER_TYPE AND SST.IS_ACTIVE = 1 AND SST.IS_DELETED <> 1
                                    WHERE
                                        SFT.FREQUENCY_TYPE_ID = 1
                                            AND FSA.IS_DELETED <> 1
                                            AND SFF.IS_DELETED <> 1
                                    GROUP BY FSA.STUDENT_ID) EF ON EF.STUDENT_ID = SC.STUDENT_ID
                                WHERE
                                    SP.IS_DELETED <> 1
                                        AND SC.IS_DELETED <> 1
                                        AND SC.STUDENT_ID = ?STUDENT_ID
                                GROUP BY SP.STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassByShiftAndProgrammeGroup:
                    {
                        query = @"SELECT 
                                        CLASS_ID, 
                                        CLASS_NAME, 
                                        CLASS_CODE
                                    FROM
                                        CP_CLASSES
                                    WHERE
                                        PROGRAMME_GROUP_ID in (?PROGRAMME_GROUP_ID)
                                            AND SHIFT = ?SHIFT
                                            AND CLASS_YEAR in (?CLASS_YEAR)
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeGroupByShiftIdAndProgrammeLevel:
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
                case SupportDataSQLCommands.FetchStudentListForAbsentSMS:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                UPPER(ROLL_NO) AS ROLL_NO,
                                IFNULL(SP.FR_MOBILE,SP.STU_MOBILENO) AS FR_MOBILE,
                                CLS.CLASS_NAME
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                            WHERE
                                SC.CLASS_ID in (?CLASS_ID) AND SP.IS_DELETED !=1 AND SP.IS_LEFT !=1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                            ORDER BY ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentListForCertificateSMS:
                    {
                        query = @"SELECT 
                                SP.STUDENT_ID,
                                UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                UPPER(ROLL_NO) AS ROLL_NO,
                                IFNULL(SP.STU_MOBILENO,SP.FR_MOBILE) AS STU_MOBILENO,
                                CLS.CLASS_NAME
                            FROM
                                STU_PERSONAL_INFO AS SP
                                    INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                    INNER JOIN
                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                            WHERE
                                SC.CLASS_ID in (?CLASS_ID) AND SP.IS_DELETED !=1 AND SP.IS_LEFT !=1
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                            ORDER BY ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentListForBulkchickin:
                    {
                        query = @"    SELECT 
                                                SP.STUDENT_ID,
                                                UPPER(SP.FIRST_NAME) AS FIRST_NAME,
                                                UPPER(ROLL_NO) AS ROLL_NO,
                                                IFNULL(SP.FR_MOBILE, SP.STU_MOBILENO) AS FR_MOBILE,
                                                CLS.CLASS_NAME
                                            FROM
                                                STU_PERSONAL_INFO AS SP
                                                    INNER JOIN
                                                STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                                    INNER JOIN
                                                CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                                    LEFT JOIN
                                                li_checkin_info_?AC_YEAR AS li ON li.USER_ID = sc.student_id
                                                    AND DATE_FORMAT(CHECKIN_DATE, '%d/%m/%Y') = ?CHECKIN_DATE
                                                    AND IS_CHECKOUT = 0
                                            WHERE
                                                SC.CLASS_ID IN (?CLASS_ID)
                                                    AND SP.IS_DELETED != 1
                                                    AND SP.IS_LEFT != 1
                                                    AND li.CHECKIN_ID IS NULL
                                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            ORDER BY ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentMobileNoForAbsentSMS:
                    {
                        query = @" SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                                ' (',
                                                sp.ROLL_NO,
                                                ' )') AS FIRST_NAME,
                                        UPPER(ROLL_NO) AS ROLL_NO,
                                        IFNULL(FR_MOBILE,STU_MOBILENO) AS MOBILE_NO,
                                        IF(CLS.SHIFT = 1,
                                            REPLACE(CLS.CLASS_NAME, ' DAY', ''),
                                            REPLACE(CLS.CLASS_NAME, ' EVE', '')) AS CLASS_NAME,
                                        SC.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SP.STUDENT_ID IN (?STUDENT_ID)
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    ORDER BY ROLL_NO; ";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentMobileNoForCertificateSMS:
                    {
                        query = @" SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(IFNULL(UPPER(SP.FIRST_NAME), ''),
                                                ' (',
                                                sp.ROLL_NO,
                                                ' )') AS FIRST_NAME,
                                        UPPER(ROLL_NO) AS ROLL_NO,
                                        IFNULL(STU_MOBILENO,FR_MOBILE) AS MOBILE_NO,
                                        IF(CLS.SHIFT = 1,
                                            REPLACE(CLS.CLASS_NAME, ' DAY', ''),
                                            REPLACE(CLS.CLASS_NAME, ' EVE', '')) AS CLASS_NAME,
                                        SC.CLASS_ID
                                    FROM
                                        STU_PERSONAL_INFO AS SP
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.STUDENT_ID = SP.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CLS ON CLS.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SP.STUDENT_ID IN (?STUDENT_ID)
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    ORDER BY ROLL_NO; ";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentbyAcYear:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        CONCAT(sp.ROLL_NO,
                                                ' - ',
                                                IFNULL(UPPER(SP.FIRST_NAME), '')) AS FIRST_NAME
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SP.IS_DELETED != 1
                                            AND SP.IS_LEFT != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupOBEUnit:
                    {
                        query = @"SELECT OBE_UNIT_ID,OBE_UNIT FROM SUP_OBE_UNIT WHERE IS_DELETED!=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClasswithCode:
                    {
                        query = @"SELECT 
                                        CLS.CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_CODE, ''),
                                                ' ',
                                                IFNULL(CONCAT('(', SPM.PROGRAMME_MODE, ') - '),
                                                        ''),
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                                IFNULL(CONCAT(' - ',COURSE_CODE),'')
                                                ) AS CLASS_NAME,
                                         CONCAT(IFNULL(CLS.CLASS_NAME, ''),' ',
                                                IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), ''),
                                                IFNULL(CONCAT(' - ', COURSE_CODE), '')) AS CLASS_CODE
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
                                        LEFT JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=CLS.COURSE_ID
                                    WHERE
                                        CLS.IS_DELETED != 1
                                    ORDER BY CLS.CLASS_TYPE DESC,CLS.CLASS_LEVEL,CLS.PROGRAMME_GROUP_ID,CLS.CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchclassForRegulr:
                    {
                        query = @"SELECT 
                                        CLASS_ID,
                                        CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                                '(',
                                                IFNULL(CONCAT(' SHIFT - ',SHIFT_NAME), ''),
                                                ')') AS CLASS_NAME
                                    FROM
                                        CP_CLASSES CLS INNER JOIN SUP_SHIFT AS SH ON SH.SHIFT_ID=CLS.SHIFT
                                    WHERE
                                        CLS.IS_DELETED != 1 AND CLS.CLASS_TYPE=2;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammeByProgrammeGroupId:
                    {
                        query = @"SELECT 
                                      PROGRAMME_GROUP_ID,
                                      CONCAT(PROGRAMME_NAME,
                                              ' - ',
                                              IF(SHIFT = 1, 'DAY', 'EVE')) AS PROGRAMME_NAME
                                  FROM
                                      CP_PROGRAMME_GROUP AS CP
                                          INNER JOIN
                                      CP_PROGRAMME AS P ON P.PROGRAMME_ID = CP.PROGRAMME_ID
                                  WHERE
                                      CP.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamLastUpdate:
                    {
                        query = @" SELECT 
                                        STUDENT_ID,
                                        DATE_FORMAT(
                                            MAX(
                                                GREATEST(
                                                    IFNULL(EXAMFEE1PAID_DATE, '0000-00-00'),
                                                    IFNULL(EXAMFEE2PAID_DATE, '0000-00-00')
                                                )
                                            ),
                                            '%d/%m/%Y'
                                        ) AS EXAMFEEPAID_DATE
                                    FROM 
                                        stu_fee_due_info
                                    WHERE 
                                        ACADEMIC_YEAR = ?AC_YEAR 
                                        AND IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchActiveFeedBackSettingsForStudent:

                    {
                        query = @" SELECT 
                                        FEEDBACK_NAME, FEEDBACK_ID
                                    FROM
                                        FBFEEDBACK_SETTINGS AS FS
                                            INNER JOIN
                                        FB_CLASS_SETTING AS FBS ON FBS.SETTING_ID = FEEDBACK_ID
                                            AND FBS.IS_ACTIVE = 1
                                            AND FBS.IS_DELETED != 1
                                            INNER JOIN
                                        STU_CLASS AS SC ON SC.CLASS_ID = FBS.CLASS_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE
                                        FS.IS_ACTIVE = 1 AND FS.IS_HOSTEL !=1
                                            AND SC.STUDENT_ID = ?STUDENT_ID
                                            AND FS.IS_DELETED != 1 
                                    UNION SELECT 
                                        FEEDBACK_NAME, FEEDBACK_ID
                                    FROM
                                        FBFEEDBACK_SETTINGS AS FS
                                            INNER JOIN
                                        HOSTEL_STUDENT_INFO AS HSI ON HSI.ACADEMIC_YEAR = FS.ACADEMIC_YEAR
                                            AND HSI.IS_DELETED != 1
                                    WHERE
                                        FS.IS_ACTIVE = 1 AND FS.IS_HOSTEL =1
                                            AND HSI.STUDENT_ID = ?STUDENT_ID
                                            AND FS.ACADEMIC_YEAR = ?AC_YEAR
                                            AND FS.IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchFeedBackSettings:
                    {
                        query = @"SELECT 
                     FEEDBACK_NAME, FEEDBACK_ID
                 FROM
                     FBFEEDBACK_SETTINGS;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedbackByacyear:
                    {
                        query = @"SELECT
                     FEEDBACK_ID,
                     FEEDBACK_NAME
                     FROM FBFEEDBACK_SETTINGS
                      WHERE  ACADEMIC_YEAR = ?ACADEMIC_YEAR AND
                      IS_ACTIVE =1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchFeedbackClassByFeedbck:
                    {
                        query = @"SELECT 
                         CS.CLASS_ID,
                         CS.CLASS_NAME
                     FROM
                         CP_CLASSES AS CS 
                     WHERE
                        CS.IS_DELETED != 1 AND CS.CLASS_TYPE=2 AND CLASS_ID not in
                        (select class_id from Fb_class_setting where  SETTING_ID =?FEEDBACK_ID and ACADEMIC_YEAR =?ACADEMIC_YEAR and is_deleted!=1)
                     GROUP BY CS.CLASS_ID;";

                        break;
                    }
                case SupportDataSQLCommands.FetchObjectiveList:
                    {
                        query = @"SELECT 
                    OBJECTIVE_ID,
                    QUESTION_TYPE,
                    OBJECTIVES,
                    IS_DELETED,
                    RATING,
                    OBJECTIVESHORTTERM,
                    OBJECTIVE_ORDER
                FROM
                    FBOBJECTIVES
                WHERE
                    IS_DELETED != 1;";

                        break;
                    }


                case SupportDataSQLCommands.FetchFeedbackQuestionList:
                    {
                        query = @"SELECT 
                                 QUESTION_ID,
                                 FEEDBACK_ID,
                                 FEEDBACK_ROLE_ID,
                                 QUESTION,
                                 QUESTION_TYPE,
                                 IS_DELETED,
                                 QUESTION_GROUP,
                                 ACADEMIC_YEAR
                                FROM
                                    FBFEEDBACK_QUESTIONS
                                WHERE
                                    IS_DELETED != 1 and FEEDBACK_ID=?FEEDBACK_ID;";

                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByClassIdAndStudentId:
                    {
                        query = @"SELECT 
                                    SP.STUDENT_ID,
                                    SP.ROLL_NO,
                                    SP.REGISTER_NO,
                                    DATE_FORMAT(SP.DATE_OF_BIRTH,'%d/%m/%Y') AS DATE_OF_BIRTH,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.LAST_NAME, ''),
                                            ' ',
                                            IFNULL(SP.ROLL_NO, '')) AS FIRST_NAME,
                                    CONCAT(IFNULL(SP.FIRST_NAME, ''),' ',IFNULL(SP.LAST_NAME, '')) AS LAST_NAME,
                                    PHOTO,
                                    D.DEPARTMENT,
                                    D.CLASS_YEAR,
                                    C.CLASS_LEVEL AS PROGRAMME_LEVEL
                                FROM
                                    STU_CLASS AS SC
                                        INNER JOIN
                                    STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                        AND SP.IS_DELETED != 1
                                        AND SP.IS_LEFT != 1
                                        INNER JOIN
									CP_CLASSES AS C ON C.CLASS_ID=SC.CLASS_ID
										INNER JOIN
									CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=C.DEPARTMENT
                                WHERE
                                        SC.STUDENT_ID in(?STUDENT_ID)
                                        AND SC.CLASS_ID =?CLASS_ID
                                        AND SC.ACADEMIC_YEAR =?AC_YEAR
                                        AND SC.IS_DELETED != 1 ORDER BY SP.ROLL_NO;";
                        break;
                    }
                case SupportDataSQLCommands.FetchAllBatch:
                    {
                        query = @"SELECT BATCH_ID,BATCH FROM SUP_BATCHES WHERE IS_DELETED !=1 order by BATCH desc;";
                        break;
                    }
                case SupportDataSQLCommands.FetchRegularClasses:
                    {
                        query = @"SELECT 
                                    CLS.CLASS_ID,
                                    CONCAT(IFNULL(CLS.CLASS_NAME, ''),
                                            ' ',
                                            IFNULL(CONCAT(' SHIFT - ', SH.SHIFT_NAME), '')) AS CLASS_NAME
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
                                    CLS.IS_DELETED != 1
                                        AND CLS.CLASS_TYPE = ?CLASS_TYPE
                                ORDER BY CLS.CLASS_TYPE , CLS.CLASS_LEVEL , CLS.PROGRAMME_GROUP_ID , CLS.CLASS_YEAR;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClasscoursestaffbyClassid:
                    {
                        query = @"SELECT
                              CCS.COURSE_ID as COURSE_ROOT_ID, CONCAT(CR.COURSE_CODE, '-', CR.COURSE_TITLE) AS COURSE_TITLE
                          FROM
                              CP_CLASS_COURSE_STAFF AS CCS
                                  LEFT JOIN
                              CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CCS.COURSE_ID
                                  INNER JOIN
                              STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = CCS.STAFF_ID
                                  AND SPI.IS_DELETED != 1
                                  AND SPI.IS_LEFT != 1
                                  INNER JOIN
                            SUP_SEMESTER_TYPE AS ST ON ST.SEMESTER_TYPE_ID = CCS.SEMESTER_TYPE_ID
                                  AND ST.IS_ACTIVE = 1
                          WHERE
                                CCS.IS_DELETED != 1
                                  AND CCS.ACADEMIC_YEAR =?AC_YEAR
                                  AND CCS.STAFF_ID =?STAFF_ID
                                  AND CCS.CLASS_ID =?CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchLibookCollection:
                    {
                        query = @"SELECT 
                                    COLLECTION_ID, COLLECTION_NAME
                                FROM
                                    SUP_LI_BOOK_COLLECTION
                                WHERE
                                    IS_DELETED != 1;";

                        break;
                    }
                case SupportDataSQLCommands.FetchCpDepartmentFaculty:
                    {
                        query = @"SELECT 
                                      DEPARTMENT_ID,
                                      DEPARTMENT
                                  FROM
                                      CP_DEPARTMENT 
                                WHERE 
                                        IS_DELETED!=1 
                                        and FACULTY !=0";

                        break;
                    }
                case SupportDataSQLCommands.FetchNationalitybyName:
                    {
                        query = @"SELECT 
                                        NATIONALITY_ID,
                                        NATIONALITY_NAME
                                    FROM
                                        SUP_NATIONALITY
                                    WHERE
                                        NATIONALITY_NAME = ?NATIONALITY_NAME;";

                        break;
                    }
                case SupportDataSQLCommands.FetchGenderbyGenderName:
                    {
                        query = @"SELECT 
                                        GENDER_ID,
                                        GENDER_NAME
                                    FROM
                                        SUP_GENDER
                                    WHERE
                                        GENDER_NAME=?GENDER_NAME;";

                        break;
                    }
                case SupportDataSQLCommands.FetchReligionbyReligion:
                    {
                        query = @"SELECT 
                                                RELIGIONID,
                                                RELIGION
                                            FROM
                                                SUP_RELIGION 
                                            WHERE
                                                RELIGION=?RELIGION;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFieldsforstats:
                    {
                        query = @"SELECT 
                                FIELD_ID, FIELD_NAME, ALICE_NAME,PROPERTY_NAME
                            FROM
                                SUP_FIELDS
                            WHERE
                                IS_DELETED != 1 AND IS_USER_STATS = 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgrammesByAppType:
                    {
                        query = @"SELECT 
                                CPG.PROGRAMME_GROUP_ID,
                                CONCAT(CP.PROGRAMME_NAME,
                                        '(',
                                        SS.SHIFT_NAME,
                                        '-',
                                        PM.PROGRAMME_MODE,
                                        ')') AS PROGRAMME_NAME
                            FROM
                                CP_PROGRAMME_GROUP AS CPG
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID=CPG.SHIFT
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                            WHERE
                                CPG.APPTYPE_ID IN (?APPTYPE_ID);";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupFields:
                    {
                        query = @"SELECT 
                                FIELD_ID, FIELD_NAME, ALICE_NAME,PROPERTY_NAME
                            FROM
                                SUP_FIELDS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSubjectTypeDDL:
                    {
                        query = @"SELECT 
                                SUBJECT_ID,
                                SUBJECT
                            FROM
                                SUP_LI_SUBJECT_TYPE
                            WHERE
                                IS_DELETED !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupLiAcademicyear:
                    {
                        query = @"SELECT 
                                    ACADEMIC_YEAR_ID, AC_YEAR
                                FROM
                                    ACADEMIC_YEAR    
                                WHERE
                                    IS_DELETED != 1
                                    ORDER BY AC_YEAR DESC;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamSettingList:
                    {
                        query = @"SELECT 
                                        EXAM_SETTING_ID,
                                        concat(EXAM_NAME,'-',ACADEMIC_YEAR) as EXAM_NAME
                                    FROM
                                        EXAM_SETTING
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseByAcyearAndSemestertypeId:
                    {
                        query = @"SELECT 
                                        CR.COURSE_ROOT_ID,
                                        UPPER(CONCAT(CR.COURSE_CODE,' - ',CR.COURSE_TITLE)) AS 'COURSE_TITLE',
                                        CR.SEMESTER_ID
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                    WHERE
                                             CC.IS_DELETED != 1
											AND CC.SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID
                                            AND CC.ACADEMIC_YEAR = ?AC_YEAR
                                    GROUP BY CR.COURSE_ROOT_ID;";
                        break;
                    }


                case SupportDataSQLCommands.FetchClassByAcyearAndSemestertypeId:
                    {
                        query = @"  SELECT 
										
                                        CR.CLASS_ID,
                                        UPPER(CR.CLASS_NAME) AS 'CLASS_NAME'
                                       
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_CLASSES AS CR ON CR.CLASS_ID = CC.CLASS_ID
                                    WHERE
                                             CC.IS_DELETED != 1
                                          AND   CC.COURSE_ID = ?COURSE_ID
											AND CC.SEMESTER_TYPE_ID=?SEMESTER_TYPE_ID
                                            AND CC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    GROUP BY CR.CLASS_ID;";
                        break;
                    }

                case SupportDataSQLCommands.FetchEventFeedbackEntry:

                    {
                        query = @"    
                                            SELECT 
                                                ERF.EVENT_ID, ERF.EVENT_NAME
                                            FROM
                                                STUDENT_EVENT_REGISTER AS SER
                                                    INNER JOIN
                                                EVENT_REGISTER_FORM AS ERF ON ERF.EVENT_ID = SER.EVENT_ID
                                                    INNER JOIN
                                                EVENT_FB_QUESTION_OBJECTIVE_MAPPING AS FBQ ON FBQ.EVENT_ID = ERF.EVENT_ID
                                                    AND FBQ.IS_ACTIVE = 1
                                                    AND FBQ.IS_DELETED != 1
                                            WHERE
                                                ERF.IS_ACTIVE = 1
                                                    AND SER.STUDENT_ID = ?STUDENT_ID
                                                    AND ERF.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                                    AND ERF.IS_DELETED != 1
                                                    AND SER.IS_DELETED != 1;";
                        break;
                    }



                case SupportDataSQLCommands.FetchActiveEventFeedback:
                    {
                        query = @"  SELECT 
                                    EVENT_NAME, EVENT_ID
                                FROM
                                    EVENT_REGISTER_FORM
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED != 1
                                   AND ACADEMIC_YEAR=?ACADEMIC_YEAR;";
                        break;
                    }

                #region Clup Register
                case SupportDataSQLCommands.FetchAcademicYearByAcYear:
                    {
                        query = @"SELECT 
                                        ACADEMIC_YEAR_ID, AC_YEAR
                                    FROM
                                        ACADEMIC_YEAR
                                    WHERE
                                        IS_DELETED != 1
                                    ORDER BY AC_YEAR DESC";

                        break;

                    }
                #endregion

                case SupportDataSQLCommands.FetchProgrammesByShiftId:
                    {
                        query = @"SELECT
                                CPG.PROGRAMME_GROUP_ID,
                                CONCAT(CP.PROGRAMME_NAME) AS PROGRAMME_NAME
                            FROM
                                CP_PROGRAMME_GROUP AS CPG
                                    INNER JOIN
                                CP_PROGRAMME AS CP ON CP.PROGRAMME_ID = CPG.PROGRAMME_ID
                                    INNER JOIN
                                SUP_SHIFT AS SS ON SS.SHIFT_ID = CPG.SHIFT
                                    INNER JOIN
                                SUP_PROGRAMME_MODE AS PM ON PM.PROGRAMME_MODE_ID = CPG.PROGRAMME_MODE
                            WHERE
                                SS.SHIFT_ID =?SHIFT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStudentByProgrammeid:
                    {
                        query = @"SELECT 
                                  SC.STUDENT_ID,
                                  SPI.ROLL_NO, CP.CLASS_ID,sc.BATCH,
                                  SPI.REGISTER_NO,
                                  SPI.FIRST_NAME,
                                  CP.CLASS_NAME,MENTORINT_ID
                              FROM
                                  STU_CLASS AS SC
                                      INNER JOIN
                                  STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                      INNER JOIN
                                  CP_CLASSES AS CP ON CP.CLASS_ID = SC.CLASS_ID
                                      INNER JOIN
                                  CP_PROGRAMME_GROUP AS CPG ON CPG.PROGRAMME_GROUP_ID = CP.PROGRAMME_GROUP_ID
                                      INNER JOIN
                                  CP_PROGRAMME AS CC ON CC.PROGRAMME_ID = CPG.PROGRAMME_ID
                                  LEFT JOIN MENTORING_STUDENT_LIST AS MSL ON MSL.STUDENT_ID=SC.STUDENT_ID
                              WHERE
                                  CC.PROGRAMME_ID =?PROGRAMME_ID AND SC.BATCH=?BATCH
                              ORDER BY CP.CLASS_ID;";
                        break;
                    }

                case SupportDataSQLCommands.FetchSemesterTypeID:
                    {
                        query = @"SELECT 
                                        SUP_SEMESTER_ID, SEMESTER_NAME,SEMESTER_TYPE,IS_ACTIVE
                                    FROM
                                        SUP_SEMESTER
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchCourseForstaff:
                    {
                        query = @"SELECT 
                                    SPI.FIRST_NAME,
                                    SPI.GSUITE_MAIL_ID,
                                    SPI.MOBILE_NO,
                                    SPI.PHOTO,
                                    CD.DEPARTMENT,
                                    LCS.COURSE_ID,
                                    CCR.COURSE_CODE,
                                    CCR.COURSE_TITLE,
                                    LCS.CLASS_ID,
                                    CC.CLASS_NAME
                                FROM
                                    LMS_CLASS_COURSE_STAFF AS LCS
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS SPI ON SPI.STAFF_ID = LCS.STAFF_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SPI.DEPARTMENT
                                        INNER JOIN
                                    CP_CLASSES AS CC ON CC.CLASS_ID = LCS.CLASS_ID
                                      INNER JOIN 
                                    CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = LCS.COURSE_ID  
                                      WHERE LCS.ACADEMIC_YEAR = ?AC_YEAR AND LCS.SEMESTER_ID IN (SELECT SUP_SEMESTER_ID AS SEMESTER_ID  FROM SUP_SEMESTER WHERE IS_ACTIVE = 1)
                                      AND LCS.STAFF_ID = ?STAFF_ID AND LCS.IS_DELETED !=1 AND SPI.IS_DELETED !=1;";

                                     
                        break;
                    }
                case SupportDataSQLCommands.FetchDepartmentForstaff:
                    {
                        query = @"  SELECT 
                                    SPI.FIRST_NAME,
                                    SPI.GSUITE_MAIL_ID,
                                    SPI.MOBILE_NO,
                                    SPI.PHOTO,
                                    CD.DEPARTMENT
                                FROM
                                  STF_PERSONAL_INFO AS SPI 
                                  INNER JOIN
                                  CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID=SPI.DEPARTMENT
                                  WHERE SPI.IS_LEFT!=1
                                    AND SPI.IS_DELETED!=1 AND CD.IS_DELETED!=1
                                    AND SPI.STAFF_ID=?STAFF_ID;";

                                     
                        break;
                    }

                case SupportDataSQLCommands.SaveLmsannouncement:
                    {
                        query = @"INSERT INTO 
                                LMS_COURSEWISE_ANNOUNCEMENT
                                (
                                CLASS_ID,
                                COURSE_ID,
                                STAFF_ID,
                                MESSAGE)
                                VALUES(
                                ?CLASS_ID,
                                ?COURSE_ID,
                                ?STAFF_ID,
                                ?MESSAGE);";
                        break;
                    }


                case SupportDataSQLCommands.FetchApplicationTypeNew:
                    {
                        query = @"SELECT 
                                APPLICATION_TYPE_ID, APPLICATION_TYPE, PREFIX, SUFFIX
                            FROM
                                SUP_APPLICATION_TYPE
                            WHERE
                                IS_DELETED != 1
                                AND IS_ACTIVE = 1;";
                        break;
                    }

                case SupportDataSQLCommands.FetchClassYearNew:
                    {
                        query = @"SELECT 
                                        CLASS_YEAR_ID, CLASS_YEAR
                                    FROM
                                        SUP_CLASS_YEAR
                                    WHERE
                                        IS_ACTIVE = 1 AND IS_DELETED !=1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchSupCertificateLists:
                    {
                        query = @" 
                                     SELECT 
                                     CERTIFICATE_ID, SCCL.CATEGORY_NAME
                                 FROM
                                     SUP_CERTIFICATE_LIST AS SCL
                                     INNER JOIN
                                     sup_certificate_category_list AS SCCL ON SCCL.CATEGORY_ID=SCL.CATEGORY_ID
                                 WHERE
                                     SCL.IS_DELETED != 1 AND
                                     SCCL.IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchProgramme:
                    {
                        query = @" SELECT 
                                    PROGRAMME_ID, PROGRAMME_NAME
                                FROM
                                    cp_programme
                                WHERE
                                    IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamRegisteredCoursesById:
                    {
                        query = @"SELECT 
                                ER.COURSE_ID, CCR.COURSE_CODE
                            FROM
                                EXAM_REGISTRATION_?AC_YEAR AS ER
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON ER.COURSE_ID = CCR.COURSE_ROOT_ID
                            WHERE
                                ER.EXAM_SETTING_ID =?EXAM_SETTING_ID
                                    AND ER.IS_DELETED != 1
                            GROUP BY ER.COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExamRegisteredCoursesByIdbyexamdate:
                    {
                        query = @"SELECT 
                                     ER.COURSE_ID, CR.COURSE_CODE
                                 FROM
                                     EXAM_REGISTRATION_?AC_YEAR AS ER
                                         INNER JOIN
                                     EXAM_TIMETABLE AS EE ON EE.COURSE_ID = ER.COURSE_ID
                                         AND EE.EXAM_SETTING_ID = ER.EXAM_SETTING_ID
                                         AND EE.EXAM_SESSION_ID IN (?EXAM_SESSION_ID)
                                         AND EE.EXAM_DATE IN (?EXAM_DATE)
                                         INNER JOIN
                                     CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = EE.COURSE_ID
                                         AND PROGRAMME_LEVEL_ID =?PROGRAMME_LEVEL_ID 
                                 WHERE
                                     ER.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                 GROUP BY ER.COURSE_ID
                                 ORDER BY CR.COURSE_CODE";
                        break;
                    }
                case SupportDataSQLCommands.FetchTimetableByExamId:
                    {
                        query = @"SELECT 
                                    ET.TIMETABLE_ID,
                                    CONCAT('\'',DATE_FORMAT(ET.EXAM_DATE, '%d-%m-%Y'),'\'') AS EXAM_TIME,
                                    CONCAT('\'',DATE_FORMAT(ET.EXAM_DATE, '%Y-%m-%d'),'\'') AS EXAM_DATE
                                FROM
                                    EXAM_TIMETABLE AS ET
                                WHERE
                                    ET.EXAM_SETTING_ID = ?EXAM_SETTING_ID
                                        AND ET.IS_DELETED != 1
                                    ORDER BY DATE_FORMAT(ET.EXAM_DATE, '%Y-%m-%d') ASC;";
                        break;
                    }

                case SupportDataSQLCommands.FetchExamRegisteredPracticalCoursesById:
                    {
                        query = @"SELECT 
                                ER.COURSE_ID,
                                CONCAT(CCR.COURSE_CODE, '-', CCR.COURSE_TITLE) AS COURSE_CODE
                            FROM
                                EXAM_REGISTRATION_?AC_YEAR AS ER
                                    INNER JOIN
                                CP_COURSE_ROOT AS CCR ON ER.COURSE_ID = CCR.COURSE_ROOT_ID
                            WHERE
                                ER.EXAM_SETTING_ID =?EXAM_SETTING_ID
                                    -- AND CCR.SUBJECT_TYPE =?SUBJECT_TYPE
                                    AND ER.IS_DELETED != 1
                            GROUP BY ER.COURSE_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchCourseListforExternalCopy:
                    {
                        query = @"SELECT COURSE_ID,CONCAT(COURSE_TITLE ,' (', COURSE_CODE,')') AS COURSE_TITLE FROM CP_CLASS_COURSE_STAFF AS CP
                                  INNER JOIN CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID=CP.COURSE_ID
                                   WHERE CP.ACADEMIC_YEAR=?ACADEMIC_YEAR AND CP.SEMESTER_TYPE_ID=?SEMESTER_ID
                                   AND CP.IS_DELETED!=1 GROUP BY COURSE_ID;"
;
                        break;
                    }
                case SupportDataSQLCommands.FetchClassIDByStudentId:
                    {
                        query = @"SELECT 
                                        SC.STUDENT_ID, SC.CLASS_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SC.STUDENT_ID
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchStaffListById:
                    {
                        query = @"SELECT 
                                        STAFF_ID, FIRST_NAME AS STAFF_NAME, EMAIL, GSUITE_MAIL_ID
                                    FROM
                                        STF_PERSONAL_INFO
                                    WHERE
                                        STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchExternalStaffListById:
                    {
                        query = @" SELECT 
                                        EXAMINAR_ID AS STAFF_ID,
                                        EXAMINAR_NAME AS STAFF_NAME,
                                        EMAIL_ID AS EMAIL,
                                        '' AS GSUITE_MAIL_ID
                                    FROM
                                        EXAM_EXAMINARS_INFO
                                    WHERE
                                        EXAMINAR_ID = ?STAFF_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOptionalCouses:
                    {
                        query = @"SELECT 
                                    COURSE_ROOT_ID,
                                    CONCAT(COURSE_CODE, ' - ', COURSE_TITLE) AS COURSE_TITLE
                                FROM
                                    CP_COURSE_ROOT AS CR 
                                    inner join
                                    cp_course_pattern as cp on cp.COURSE_ID =COURSE_ROOT_ID
                                WHERE
                                     SEMESTER_ID=?SEMESTER_ID AND CP.ACADEMIC_YEAR =?AC_YEAR
                                     AND PROGRAMME_LEVEL_ID=?PROGRAMME_LEVEL_ID group by COURSE_ROOT_ID;";

                        break;
                    }
                case SupportDataSQLCommands.FetchClassinfoByClassId:
                    {
                        query = @"SELECT 
                                CLASS_ID,
                                CONCAT(IFNULL(CLS.CLASS_NAME, '')) AS CLASS_NAME,
                                CLS.CLASS_TYPE
                            FROM
                                CP_CLASSES AS CLS
                            WHERE
                                CLS.IS_DELETED != 1
                                    AND CLS.CLASS_ID =?CLASS_ID;";
                        break;
                    }
                case SupportDataSQLCommands.FetchOptionalClassesByCourseId:
                    {
                        query = @"SELECT 
                                CLASS_ID,CLASS_NAME,CLASS_YEAR
                            FROM
                                CP_CLASSES
                            WHERE
                               
                               CLASS_TYPE != 2 AND  COURSE_ID=?COURSE_ID AND 
                                IS_DELETED != 1;";
                        break;
                    }
                case SupportDataSQLCommands.FetchClassDetails:
                    {
                        query = @"SELECT 
                                CLASS_ID, CLASS_NAME, CLASS_LEVEL, CLASS_YEAR
                            FROM
                                CP_CLASSES
                            WHERE
                                CLASS_ID = ?CLASS_ID AND CLASS_TYPE = 2
                                    AND IS_DELETED != 1;";
                        break;
                    }
                default:
                    break;
            }
            return query;
        }

    }
}
