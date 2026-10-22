using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;
namespace CMS.SQL.IQAC
{
    public static class IQACSQL
    {
        public static string GetIQACSQL(IQACEnumCommands sEnumCommand)
        {
            string sQuery = string.Empty;
            switch (sEnumCommand)
            {
                #region 1. Teaching Staff Workload
                case IQACEnumCommands.SaveTeachingSubjectWorkload:
                    {
                        sQuery = @"INSERT INTO IQAC_TEACHING_SUBJECTS_WORKLOAD
		                            (
			                            STAFF_ID,
			                            CLASS_ID,
			                            COURSE_ID,
			                            LECTURE_ALLOTTED,
			                            LECTURE_TAKEN,
			                            PRACTICAL_ALLOTTED,
			                            PRACTICAL_TAKEN,
			                            CASUAL_LEAVE,
			                            ON_DUTY,
			                            MEDICAL_LEAVE,
			                            ENTRY_DTATE,
			                            MONTH_ID,
			                            ACADEMIC_YEAR
		                            ) VALUES
                                    (
			                            ?STAFF_ID,
			                            ?CLASS_ID,
			                            ?COURSE_ID,
			                            ?LECTURE_ALLOTTED,
			                            ?LECTURE_TAKEN,
			                            ?PRACTICAL_ALLOTTED,
			                            ?PRACTICAL_TAKEN,
			                            ?CASUAL_LEAVE,
			                            ?ON_DUTY,
			                            ?MEDICAL_LEAVE,
			                            CURDATE(),
			                            ?MONTH_ID,
			                            ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateTeachingSubjectWorkload:
                    {
                        sQuery = @"UPDATE IQAC_TEACHING_SUBJECTS_WORKLOAD
                                          SET
                                          STAFF_ID = ?STAFF_ID,
                                          CLASS_ID = ?CLASS_ID,
                                          COURSE_ID = ?COURSE_ID,
                                          LECTURE_ALLOTTED = ?LECTURE_ALLOTTED,
                                          LECTURE_TAKEN = ?LECTURE_TAKEN,
                                          PRACTICAL_ALLOTTED = ?PRACTICAL_ALLOTTED,
                                          PRACTICAL_TAKEN = ?PRACTICAL_TAKEN,
                                          CASUAL_LEAVE = ?CASUAL_LEAVE,
                                          ON_DUTY = ?ON_DUTY,
                                          MEDICAL_LEAVE = ?MEDICAL_LEAVE,
                                          ENTRY_DTATE = CURDATE(),
                                          MONTH_ID = ?MONTH_ID,
                                          ACADEMIC_YEAR = ?AC_YEAR
                                          WHERE WORKLOAD_ID = ?WORKLOAD_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteTeachingSubjectWorkload:
                    {
                        sQuery = @"UPDATE IQAC_TEACHING_SUBJECTS_WORKLOAD SET IS_DELETED=1 WHERE WORKLOAD_ID = ?WORKLOAD_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchTeachingSubjectWorkloadByStaffAndCourseAndMonthId:
                    {
                        sQuery = @"SELECT 
                                        TSW.WORKLOAD_ID,
                                        TSW.STAFF_ID,
                                        TSW.CLASS_ID,
                                        TSW.COURSE_ID,
                                        TSW.LECTURE_ALLOTTED,
                                        TSW.LECTURE_TAKEN,
                                        TSW.PRACTICAL_ALLOTTED,
                                        TSW.PRACTICAL_TAKEN,
                                        TSW.CASUAL_LEAVE,
                                        TSW.ON_DUTY,
                                        TSW.MEDICAL_LEAVE,
                                        TSW.MONTH_ID
                                    FROM
                                        IQAC_TEACHING_SUBJECTS_WORKLOAD AS TSW
                                    WHERE
                                        TSW.STAFF_ID = ?STAFF_ID
                                            AND TSW.COURSE_ID = ?COURSE_ID
                                            AND TSW.MONTH_ID = ?MONTH_ID AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND TSW.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchClassCourseStaffByStaffAndCourseId:
                    {
                        sQuery = @"SELECT 
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CC.STAFF_ID,
                                        CC.CLASS_ID,
                                        C.CLASS_CODE,
                                        CC.COURSE_ID,
                                        CR.SEMESTER_ID,
                                        CR.COURSE_TITLE,
                                        CR.COURSE_CODE
                                    FROM
                                        CP_CLASS_COURSE_STAFF AS CC
                                            INNER JOIN
                                        CP_CLASSES AS C ON C.CLASS_ID = CC.CLASS_ID
                                            AND C.IS_DELETED != 1 
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CR ON CR.COURSE_ROOT_ID = CC.COURSE_ID
                                            AND CR.IS_DELETED != 1 
                                            INNER JOIN 
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = CC.STAFF_ID
                                            AND SP.IS_DELETED != 1 
                                    WHERE
                                        CC.STAFF_ID = ?STAFF_ID AND CC.COURSE_ID = ?COURSE_ID
                                            AND CC.IS_DELETED != 1 AND CC.ACADEMIC_YEAR=?AC_YEAR;";
                        break;
                    }
                #endregion

                #region 2. Praparation & Updation Of Notes
                case IQACEnumCommands.SavePreparationAndUpdationOfNotes:
                    {
                        sQuery = @"INSERT INTO IQAC_PREPARATION_AND_UPDATION_OF_NOTES
                                    (
	                                    STAFF_ID,
	                                    CLASS_ID,
	                                    COURSE_ID,
	                                  --  TOPIC,
	                                    TITLE,
	                                    AUTHOR,
	                                    PUBLICATION,
	                                    YEAR_OF_PUBLICATION,
	                                    PLACE_OF_LIBRARY,
	                                 --   TOPIC_REFERRED,
	                                    ENTRY_DTATE,
	                                    MONTH_ID,
	                                    ACADEMIC_YEAR,
                                        WEBSITE_REFFERED,
                                        E_RESOURSE_DEVELOPED
                                      --  FILE_PATH
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?CLASS_ID,
	                                    ?COURSE_ID,
	                                 --   ?TOPIC,
	                                    ?TITLE,
	                                    ?AUTHOR,
	                                    ?PUBLICATION,
	                                    ?YEAR_OF_PUBLICATION,
	                                    ?PLACE_OF_LIBRARY,
	                                  --  ?TOPIC_REFERRED,
	                                    CURDATE(),
	                                    ?MONTH_ID,
	                                    ?AC_YEAR,
                                        ?WEBSITE_REFFERED,
                                        ?E_RESOURSE_DEVELOPED
                                       -- ?FILE_PATH
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdatePreparationAndUpdationOfNotes:
                    {
                        sQuery = @"UPDATE IQAC_PREPARATION_AND_UPDATION_OF_NOTES
		                             SET
		                                STAFF_ID = ?STAFF_ID,
		                                CLASS_ID = ?CLASS_ID,
		                                COURSE_ID = ?COURSE_ID,
		                              --  TOPIC = ?TOPIC,
		                                TITLE = ?TITLE,
		                                AUTHOR = ?AUTHOR,
		                                PUBLICATION = ?PUBLICATION,
		                                YEAR_OF_PUBLICATION = ?YEAR_OF_PUBLICATION,
		                                PLACE_OF_LIBRARY = ?PLACE_OF_LIBRARY,
		                              --  TOPIC_REFERRED = ?TOPIC_REFERRED,
		                                ENTRY_DTATE = CURDATE(),
		                                MONTH_ID = ?MONTH_ID,
		                                ACADEMIC_YEAR = ?AC_YEAR,
                                        WEBSITE_REFFERED=?WEBSITE_REFFERED,
                                        E_RESOURSE_DEVELOPED=?E_RESOURSE_DEVELOPED,
                                        FILE_PATH=?FILE_PATH
                                  WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeletePreparationAndUpdationOfNotes:
                    {
                        sQuery = @"UPDATE IQAC_PREPARATION_AND_UPDATION_OF_NOTES SET IS_DELETED=1 WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchPreparationAndUpdationOfNotesByStaffAndCourseAndMonthId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                     --   TOPIC,
                                        TITLE,
                                        AUTHOR,
                                        PUBLICATION,
                                        YEAR_OF_PUBLICATION,
                                        PLACE_OF_LIBRARY,
                                      --  TOPIC_REFERRED
                                        WEBSITE_REFFERED,
                                        E_RESOURSE_DEVELOPED,
                                        FILE_PATH
                                    FROM
                                        IQAC_PREPARATION_AND_UPDATION_OF_NOTES 
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND COURSE_ID = ?COURSE_ID
                                            AND MONTH_ID = ?MONTH_ID AND ACADEMIC_YEAR = ?AC_YEAR 
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchPreparationAndUpdationOfNotesByStaffAndClassAndMonthId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                     --   TOPIC,
                                        TITLE,
                                        AUTHOR,
                                        PUBLICATION,
                                        YEAR_OF_PUBLICATION,
                                        PLACE_OF_LIBRARY,
                                      --  TOPIC_REFERRED
                                        WEBSITE_REFFERED,
                                        E_RESOURSE_DEVELOPED,
                                        FILE_PATH
                                    FROM
                                        IQAC_PREPARATION_AND_UPDATION_OF_NOTES 
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND CLASS_ID=?CLASS_ID 
                                            AND COURSE_ID = ?COURSE_ID
                                            AND MONTH_ID = ?MONTH_ID 
                                            AND ACADEMIC_YEAR = ?AC_YEAR 
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.UpdatePreparationAndUpdationOfNotesByIQAcId:
                    {
                        sQuery = @"UPDATE 
                                    iqac_preparation_and_updation_of_notes 
                                    SET
                                    FILE_PATH=?FILE_PATH
                                    WHERE
                                    IQAC_ID=?IQAC_ID
                                    AND IS_DELETED!=1;";
                        break;
                    }

                #endregion

                #region 3. Co - Curricular Aspects
                case IQACEnumCommands.SaveCurricularAspects:
                    {
                        sQuery = @"INSERT INTO IQAC_CO_CURRICULAR_ASPECTS
                                    (
	                                    STAFF_ID,
	                                    DEPT_CODE,
	                                    NAME_OF_COMMITEE,
	                                    ACTIVITY_DATE,
	                                    ACTIVITY_COMPLETED,
	                                    ENTRY_DTATE,
	                                    MONTH_ID,
	                                    ACADEMIC_YEAR,
                                        FILE_PATH
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?DEPT_CODE,
	                                    ?NAME_OF_COMMITEE,
	                                    ?ACTIVITY_DATE,
	                                    ?ACTIVITY_COMPLETED,
	                                    CURDATE(),
	                                    ?MONTH_ID,
	                                    ?AC_YEAR,
                                        ?FILE_PATH
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateCurricularAspects:
                    {
                        sQuery = @"UPDATE IQAC_CO_CURRICULAR_ASPECTS
                                    SET
                                    STAFF_ID = ?STAFF_ID,
                                    DEPT_CODE = ?DEPT_CODE,
                                    NAME_OF_COMMITEE = ?NAME_OF_COMMITEE,
                                    ACTIVITY_DATE = ?ACTIVITY_DATE,
                                    ACTIVITY_COMPLETED = ?ACTIVITY_COMPLETED,
                                    ENTRY_DTATE = CURDATE(),
                                    MONTH_ID = ?MONTH_ID,
                                    ACADEMIC_YEAR = ?AC_YEAR,
                                    FILE_PATH=?FILE_PATH
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchCoCurricularAspectsByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        NAME_OF_COMMITEE,
                                        DATE_FORMAT(ACTIVITY_DATE, '%d/%m/%Y') AS ACTIVITY_DATE,
                                        ACTIVITY_COMPLETED,
                                        FILE_PATH
                                    FROM
                                        IQAC_CO_CURRICULAR_ASPECTS
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteCurricularAspects:
                    {
                        sQuery = @"UPDATE IQAC_CO_CURRICULAR_ASPECTS SET IS_DELETED=1 WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.ValidateCoOrdinator:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        COMMITTEE_NAME,
                                        SP.STAFF_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS STAFF_NAME
                                    FROM
                                        IQAC_CURRICULAR_COORDINATOR AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                    WHERE
                                        I.IS_DELETED != 1
                                            AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            AND I.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                #endregion

                #region 4. Extension Works
                case IQACEnumCommands.SaveExtensionWorks:
                    {
                        sQuery = @"INSERT INTO IQAC_EXTENSION_WORKS 
                                    (
	                                    STAFF_ID,
	                                    DEPT_CODE,
	                                    ACTIVITY_DATE,
	                                    NAME_OF_THE_PROGRAMME,
	                                    ACTIVITY_COMPLETED,
	                                    NO_OF_PARTICIPANTS,
	                                    PLACE,
	                                    ENTRY_DTATE,
	                                    MONTH_ID,
	                                    ACADEMIC_YEAR,
                                        FILE_PATH
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?DEPT_CODE,
	                                    ?ACTIVITY_DATE,
	                                    ?NAME_OF_THE_PROGRAMME,
	                                    ?ACTIVITY_COMPLETED,
	                                    ?NO_OF_PARTICIPANTS,
	                                    ?PLACE,
	                                    CURDATE(),
	                                    ?MONTH_ID,
	                                    ?AC_YEAR,
                                        ?FILE_PATH
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateExtensionWorks:
                    {
                        sQuery = @"UPDATE IQAC_EXTENSION_WORKS
                                   SET
	                                   STAFF_ID = ?STAFF_ID,
	                                   DEPT_CODE = ?DEPT_CODE,
	                                   ACTIVITY_DATE = ?ACTIVITY_DATE,
	                                   NAME_OF_THE_PROGRAMME = ?NAME_OF_THE_PROGRAMME,
	                                   ACTIVITY_COMPLETED = ?ACTIVITY_COMPLETED,
	                                   NO_OF_PARTICIPANTS = ?NO_OF_PARTICIPANTS,
	                                   PLACE = ?PLACE,
	                                   ENTRY_DTATE = CURDATE(),
	                                   MONTH_ID = ?MONTH_ID,
	                                   ACADEMIC_YEAR = ?AC_YEAR,
                                       FILE_PATH=?FILE_PATH
                                   WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchExtensionWorksByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        NAME_OF_THE_PROGRAMME,
                                        ACTIVITY_COMPLETED,
                                        NO_OF_PARTICIPANTS,
                                        PLACE,
                                        FILE_PATH
                                    FROM
                                        IQAC_EXTENSION_WORKS
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteExtensionWorks:
                    {
                        sQuery = @"UPDATE IQAC_EXTENSION_WORKS SET IS_DELETED=1 WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                #endregion

                #region 5. Personal Academic Activity
                case IQACEnumCommands.SavePersonalAcademicActivity:
                    {
                        sQuery = @"INSERT INTO IQAC_PERSONAL_ACADEMIC_ACTIVITY
                                    (
                                        STAFF_ID,
                                        CLASS_CODE,
                                        -- SEMESTER_ID,
                                        DEPT_CODE,
                                        ACTIVITY_DATE,
                                        ACTIVITY,
                                        TOPIC,
                                        IF_OTHERS_SPECIFY,
                                        NO_OF_PARTICIPANTS,
                                        ENTRY_DTATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?CLASS_CODE,
                                        -- ?SEMESTER_ID,
                                        ?DEPT_CODE,
                                        ?ACTIVITY_DATE,
                                        ?ACTIVITY,
                                        ?TOPIC,
                                        ?IF_OTHERS_SPECIFY,
                                        ?NO_OF_PARTICIPANTS,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdatePersonalAcademicActivity:
                    {
                        sQuery = @"UPDATE IQAC_PERSONAL_ACADEMIC_ACTIVITY
                                    SET
                                        STAFF_ID = ?STAFF_ID,
                                        CLASS_CODE = ?CLASS_CODE,
                                      --  SEMESTER_ID = ?SEMESTER_ID,
                                        DEPT_CODE = ?DEPT_CODE,
                                        ACTIVITY_DATE=?ACTIVITY_DATE,
                                        ACTIVITY = ?ACTIVITY,
                                        TOPIC = ?TOPIC,
                                        IF_OTHERS_SPECIFY = ?IF_OTHERS_SPECIFY,
                                        NO_OF_PARTICIPANTS = ?NO_OF_PARTICIPANTS,
                                        ENTRY_DTATE = CURDATE(),
                                        MONTH_ID = ?MONTH_ID,
                                        ACADEMIC_YEAR = ?AC_YEAR
                                        WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchPersonalAcademicActivityByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        CLASS_CODE,
                                        SEMESTER_ID,
                                        DEPT_CODE,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        ACTIVITY,
                                        TOPIC,
                                        IF_OTHERS_SPECIFY,
                                        NO_OF_PARTICIPANTS
                                    FROM
                                        IQAC_PERSONAL_ACADEMIC_ACTIVITY
                                    WHERE
                                         MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND STAFF_ID = ?STAFF_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeletePersonalAcademicActivity:
                    {
                        sQuery = @"UPDATE IQAC_PERSONAL_ACADEMIC_ACTIVITY SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }

                #endregion

                #region 6. Work Done Data
                case IQACEnumCommands.SaveWorkDoneData:
                    {
                        sQuery = @"INSERT INTO IQAC_WORK_DONE_DATA
                                    (
                                        CLASS_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        UNIT,
                                        ALLOTTED,
                                        COVERED,
                                        ENTRY_DTATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?CLASS_ID,
                                        ?COURSE_ID,
                                        ?STAFF_ID,
                                        ?UNIT,
                                        ?ALLOTTED,
                                        ?COVERED,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateWorkDoneData:
                    {
                        sQuery = @"UPDATE IQAC_WORK_DONE_DATA
                                    SET
	                                    CLASS_ID = ?CLASS_ID,
	                                    COURSE_ID = ?COURSE_ID,
	                                    STAFF_ID = ?STAFF_ID,
	                                    UNIT = ?UNIT,
	                                    ALLOTTED = ?ALLOTTED,
	                                    COVERED = ?COVERED,
	                                    ENTRY_DTATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteWorkDoneData:
                    {
                        sQuery = @"UPDATE IQAC_WORK_DONE_DATA SET IS_DELETED=1 WHERE IQAC_ID=1;";
                        break;
                    }
                case IQACEnumCommands.FetchWorkDoneDataByStaffAndCourseAndMonthId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        UNIT,
                                        ALLOTTED,
                                        COVERED
                                    FROM
                                        IQAC_WORK_DONE_DATA
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID
                                            AND COURSE_ID = ?COURSE_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region 7. Examination Duties Internal
                case IQACEnumCommands.SaveExaminationDutiesInternal:
                    {
                        sQuery = @"INSERT INTO IQAC_EXAMINATION_DUTIES_INTERNAL
                                    (
	                                    CLASS_ID,
	                                    COURSE_ID,
	                                    STAFF_ID,
	                                    EXAMINATION_DTATE,
	                                    PAPER_SETTING,
	                                    INVIGILATORS,
	                                    PAPER,
	                                    PAPERS,
	                                    ASSIGNMENTS,
	                                    ENTRY_DTATE,
	                                    MONTH_ID,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?CLASS_ID,
	                                    ?COURSE_ID,
	                                    ?STAFF_ID,
	                                    ?EXAMINATION_DTATE,
	                                    ?PAPER_SETTING,
	                                    ?INVIGILATORS,
	                                    ?PAPER,
	                                    ?PAPERS,
	                                    ?ASSIGNMENTS,
	                                    CURDATE(),
	                                    ?MONTH_ID,
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateExaminationDutiesInternal:
                    {
                        sQuery = @"UPDATE IQAC_EXAMINATION_DUTIES_INTERNAL
                                    SET
	                                    CLASS_ID = ?CLASS_ID,
	                                    COURSE_ID = ?COURSE_ID,
	                                    STAFF_ID = ?STAFF_ID,
	                                    EXAMINATION_DTATE = ?EXAMINATION_DTATE,
	                                    PAPER_SETTING = ?PAPER_SETTING,
	                                    INVIGILATORS = ?INVIGILATORS,
	                                    PAPER = ?PAPER,
	                                    PAPERS = ?PAPERS,
	                                    ASSIGNMENTS = ?ASSIGNMENTS,
	                                    ENTRY_DTATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchExaminationDutiesInternalByStaffAndCourseAndMonthId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        CLASS_ID,
                                        COURSE_ID,
                                        STAFF_ID,
                                        DATE_FORMAT(EXAMINATION_DTATE,'%d/%m/%Y')AS EXAMINATION_DTATE,
                                        PAPER_SETTING,
                                        INVIGILATORS,
                                        PAPER,
                                        PAPERS,
                                        ASSIGNMENTS
                                    FROM
                                        IQAC_EXAMINATION_DUTIES_INTERNAL
                                    WHERE
                                        COURSE_ID = ?COURSE_ID AND STAFF_ID = ?STAFF_ID
                                            AND MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteExaminationDutiesInternal:
                    {
                        sQuery = @"UPDATE IQAC_EXAMINATION_DUTIES_INTERNAL SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                #endregion

                #region 8. Professional Academic Activity
                case IQACEnumCommands.SaveProfessionalAcademicActivity:
                    {
                        sQuery = @"INSERT INTO IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES
                                    (
                                        STAFF_ID,
                                        DEPT_CODE,
                                        ACTIVITY,
                                        ACTIVITY_DATE,
                                        IF_OTHER_SPECIFY,
                                        INSTITUTION_PLACE,
                                        ENTRY_DTATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?DEPT_CODE,
                                        ?ACTIVITY,
                                        ?ACTIVITY_DATE,
                                        ?IF_OTHER_SPECIFY,
                                        ?INSTITUTION_PLACE,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateProfessionalAcademicActivity:
                    {
                        sQuery = @"UPDATE IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES
                                    SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    DEPT_CODE = ?DEPT_CODE,
	                                    ACTIVITY = ?ACTIVITY,
	                                    ACTIVITY_DATE = ?ACTIVITY_DATE,
	                                    IF_OTHER_SPECIFY = ?IF_OTHER_SPECIFY,
	                                    INSTITUTION_PLACE = ?INSTITUTION_PLACE,
	                                    ENTRY_DTATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteProfessionalAcademicActivity:
                    {
                        sQuery = @"UPDATE IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES SET IS_DELETED=1 WHERE IQAC_ID=1;";
                        break;
                    }
                case IQACEnumCommands.FetchProfessionalAcademicActivityByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        ACTIVITY,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        IF_OTHER_SPECIFY,
                                        INSTITUTION_PLACE
                                    FROM
                                        IQAC_PROFESSIONAL_ACADEMIC_ACTIVITIES
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND STAFF_ID = ?STAFF_ID
                                            AND MONTH_ID = ?MONTH_ID;";
                        break;
                    }
                #endregion

                #region 9. Research Project
                case IQACEnumCommands.SaveResearchProject:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_PROJECTS
                                    (
                                        STAFF_ID,
                                        DEPT_CODE,
                                        PROJECT_TYPE,
                                        PROJECT_TITLE,
                                        DURATION,
                                        FUNDING_AGENCY,
                                        ENTRY_DTATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?DEPT_CODE,
                                        ?PROJECT_TYPE,
                                        ?PROJECT_TITLE,
                                        ?DURATION,
                                        ?FUNDING_AGENCY,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResearchProject:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_PROJECTS
                                    SET
	                                    STAFF_ID =?STAFF_ID,
	                                    DEPT_CODE =?DEPT_CODE,
	                                    PROJECT_TYPE =?PROJECT_TYPE,
	                                    PROJECT_TITLE =?PROJECT_TITLE,
	                                    DURATION =?DURATION,
	                                    FUNDING_AGENCY =?FUNDING_AGENCY,
	                                    ENTRY_DTATE =CURDATE(),
	                                    MONTH_ID =?MONTH_ID,
	                                    ACADEMIC_YEAR =?AC_YEAR
                                    WHERE IQAC_ID =?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteResearchProject:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_PROJECTS SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchProjectByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        PROJECT_TYPE,
                                        PROJECT_TITLE,
                                        DURATION,
                                        FUNDING_AGENCY
                                    FROM
                                        IQAC_RESEARCH_PROJECTS
                                    WHERE
                                        MONTH_ID = ?MONTH_ID AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region 10. Research Academic Contribution A
                case IQACEnumCommands.SaveResearchAcademicContributionA:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A
                                    (
	                                    STAFF_ID,
	                                    DEPT_CODE,
	                                    CO_AUTHORS,
	                                    PAPER_TITLE,
	                                    SEMINAR,
	                                    RESEARCH_DATE,
	                                    PLACE,
	                                    ENTRY_DATE,
	                                    MONTH_ID,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?DEPT_CODE,
	                                    ?CO_AUTHORS,
	                                    ?PAPER_TITLE,
	                                    ?SEMINAR,
	                                    ?RESEARCH_DATE,
	                                    ?PLACE,
	                                    CURDATE(),
	                                    ?MONTH_ID,
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResearchAcademicContributionA:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A
                                    SET
	                                    STAFF_ID =?STAFF_ID,
	                                    DEPT_CODE =?DEPT_CODE,
	                                    CO_AUTHORS =?CO_AUTHORS,
	                                    PAPER_TITLE =?PAPER_TITLE,
	                                    SEMINAR =?SEMINAR,
	                                    RESEARCH_DATE =?RESEARCH_DATE,
	                                    PLACE =?PLACE,
	                                    ENTRY_DATE = CURDATE(),
	                                    MONTH_ID =?MONTH_ID,
	                                    ACADEMIC_YEAR =?AC_YEAR
                                    WHERE IQAC_ID =?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchAcademicContributionAByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        CO_AUTHORS,
                                        PAPER_TITLE,
                                        SEMINAR,
                                        DATE_FORMAT(RESEARCH_DATE, '%d/%m/%Y') AS RESEARCH_DATE,
                                        PLACE
                                    FROM
                                        IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteResearchAcademicContributionA:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_A SET IS_DELETED=1 WHERE IQAC_ID =?IQAC_ID;";
                        break;
                    }
                #endregion

                #region 10. Research Academic Contribution B
                case IQACEnumCommands.SaveResearchAcademicContributionB:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B
                                    (
                                        STAFF_ID,
                                        DEPT_CODE,
                                        CO_AUTHORS,
                                        PAPER_TITLE,
                                        JOURNAL_TITLE,
                                        VOLUME_NUMBER_PAGES,
                                        UGC_LISTED,
                                        ENTRY_DATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?DEPT_CODE,
                                        ?CO_AUTHORS,
                                        ?PAPER_TITLE,
                                        ?JOURNAL_TITLE,
                                        ?VOLUME_NUMBER_PAGES,
                                        ?UGC_LISTED,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResearchAcademicContributionB:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B
                                    SET
	                                    IQAC_ID = ?IQAC_ID,
	                                    STAFF_ID = ?STAFF_ID,
	                                    DEPT_CODE = ?DEPT_CODE,
	                                    CO_AUTHORS = ?CO_AUTHORS,
	                                    PAPER_TITLE = ?PAPER_TITLE,
	                                    JOURNAL_TITLE = ?JOURNAL_TITLE,
	                                    VOLUME_NUMBER_PAGES = ?VOLUME_NUMBER_PAGES,
	                                    UGC_LISTED = ?UGC_LISTED,
	                                    ENTRY_DATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchAcademicContributionBByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        CO_AUTHORS,
                                        PAPER_TITLE,
                                        JOURNAL_TITLE,
                                        VOLUME_NUMBER_PAGES,
                                        UGC_LISTED,
                                        ACADEMIC_YEAR
                                    FROM
                                        IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteResearchAcademicContributionB:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_B SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                #endregion

                #region 10. Research Academic Contribution C
                case IQACEnumCommands.SaveResearchAcademicContributionC:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C
                                    (
                                        STAFF_ID,
                                        DEPT_CODE,
                                        SEMINAR_TITLE,
                                        SEMINAR_DATE,
                                        ORGANIZER_AND_VENUE,
                                        ENTRY_DATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?DEPT_CODE,
                                        ?SEMINAR_TITLE,
                                        ?SEMINAR_DATE,
                                        ?ORGANIZER_AND_VENUE,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResearchAcademicContributionC:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C
                                    SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    DEPT_CODE = ?DEPT_CODE,
	                                    SEMINAR_TITLE = ?SEMINAR_TITLE,
	                                    SEMINAR_DATE = ?SEMINAR_DATE,
	                                    ORGANIZER_AND_VENUE = ?ORGANIZER_AND_VENUE,
	                                    ENTRY_DATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchAcademicContributionCByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                            IQAC_ID,
                                            STAFF_ID,
                                            DEPT_CODE,
                                            SEMINAR_TITLE,
                                            DATE_FORMAT(SEMINAR_DATE,'%d/%m/%Y')AS SEMINAR_DATE,
                                            ORGANIZER_AND_VENUE
                                        FROM
                                            IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C
                                        WHERE
                                            STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID
                                                AND ACADEMIC_YEAR = ?AC_YEAR
                                                AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteResearchAcademicContributionC:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_C SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID";
                        break;
                    }
                #endregion

                #region 10. Research Academic Contribution D
                case IQACEnumCommands.SaveResearchAcademicContributionD:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D
                                    (
                                        STAFF_ID,
                                        DEPT_CODE,
                                        WORKSHOP_TITLE,
                                        WORKSHOP_DATE,
                                        ORGANIZER_AND_VENUE,
                                        ENTRY_DATE,
                                        MONTH_ID,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?DEPT_CODE,
                                        ?WORKSHOP_TITLE,
                                        ?WORKSHOP_DATE,
                                        ?ORGANIZER_AND_VENUE,
                                        CURDATE(),
                                        ?MONTH_ID,
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResearchAcademicContributionD:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D
                                   SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    DEPT_CODE = ?DEPT_CODE,
	                                    WORKSHOP_TITLE = ?WORKSHOP_TITLE,
	                                    WORKSHOP_DATE = ?WORKSHOP_DATE,
	                                    ORGANIZER_AND_VENUE = ?ORGANIZER_AND_VENUE,
	                                    ENTRY_DATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                   WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchAcademicContributionDByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        WORKSHOP_TITLE,
                                        DATE_FORMAT(WORKSHOP_DATE,'%d/%m/%Y')AS WORKSHOP_DATE,
                                        ORGANIZER_AND_VENUE
                                    FROM
                                        IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.DeleteResearchAcademicContributionD:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_D SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                #endregion

                #region 10. Research Academic Contribution E
                case IQACEnumCommands.SaveResearchAcademicContributionE:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E
                                    (
	                                    STAFF_ID,
	                                    DEPT_CODE,
	                                    BOOK_TITLE,
	                                    PUBLISHER_PLACE,
	                                    VOLUME_PAGES,
	                                    ISBN_NO,
	                                    ENTRY_DATE,
	                                    MONTH_ID,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?DEPT_CODE,
	                                    ?BOOK_TITLE,
	                                    ?PUBLISHER_PLACE,
	                                    ?VOLUME_PAGES,
	                                    ?ISBN_NO,
	                                    CURDATE(),
	                                    ?MONTH_ID,
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResearchAcademicContributionE:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E
                                    SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    DEPT_CODE = ?DEPT_CODE,
	                                    BOOK_TITLE = ?BOOK_TITLE,
	                                    PUBLISHER_PLACE = ?PUBLISHER_PLACE,
	                                    VOLUME_PAGES = ?VOLUME_PAGES,
	                                    ISBN_NO = ?ISBN_NO,
	                                    ENTRY_DATE = CURDATE(),
	                                    MONTH_ID = ?MONTH_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchAcademicContributionEByMonthAndStaffId:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        DEPT_CODE,
                                        BOOK_TITLE,
                                        PUBLISHER_PLACE,
                                        VOLUME_PAGES,
                                        ISBN_NO
                                    FROM
                                        IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E
                                    WHERE
                                        IS_DELETED != 1 AND STAFF_ID = ?STAFF_ID
                                            AND MONTH_ID = ?MONTH_ID
                                            AND ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case IQACEnumCommands.DeleteResearchAcademicContributionE:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_ACADEMIC_CONTRIBUTION_E SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                #endregion

                #region Month Wise Report
                case IQACEnumCommands.FetchTeachingsubjectsworkloadForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
		                                    IPN.WORKLOAD_ID,
		                                    CONCAT(IFNULL(STF.FIRST_NAME, ''),
				                                    '',
				                                    IFNULL(STF.LAST_NAME, '')) AS FIRST_NAME,
		                                    STF.STAFF_CODE,
		                                    CS.COURSE_TITLE,
		                                    CS.COURSE_CODE,
		                                    CC.CLASS_CODE,
		                                    CC.CLASS_NAME,
		                                    IPN.LECTURE_ALLOTTED,
		                                    IPN.LECTURE_TAKEN,
		                                    IPN.PRACTICAL_ALLOTTED,
		                                    IPN.PRACTICAL_TAKEN,
		                                    IPN.CASUAL_LEAVE,
		                                    IPN.ON_DUTY,
		                                    IPN.MEDICAL_LEAVE,
		                                    IPN.ENTRY_DTATE,
		                                    IPN.ACADEMIC_YEAR,
		                                    CD.DEPARTMENT_CODE,
		                                    CD.DEPARTMENT,
		                                    SS.SUP_SEMESTER_ID AS SEMESTER_ID,
                                            IPN.ACADEMIC_YEAR
	                                    FROM
		                                    IQAC_TEACHING_SUBJECTS_WORKLOAD AS IPN
			                                    INNER JOIN
		                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
			                                    AND STF.IS_DELETED != 1
			                                    AND STF.IS_LEFT != 1
			                                    INNER JOIN
		                                    CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = STF.STAFF_ID
			                                    AND CCS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
			                                    AND CCS.CLASS_ID = IPN.CLASS_ID
			                                    AND CCS.COURSE_ID = IPN.COURSE_ID
			                                    AND CCS.IS_DELETED != 1
			                                    INNER JOIN
		                                    ACADEMIC_YEAR AS AY ON AY.AC_YEAR = CCS.ACADEMIC_YEAR
			                                    AND AY.IS_DELETED != 1
			                                    INNER JOIN
		                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CCS.SEMESTER_ID
			                                    AND SS.IS_DELETED != 1
			                                    INNER JOIN
		                                    CP_CLASSES AS CC ON CC.CLASS_ID = IPN.CLASS_ID
			                                    AND CC.IS_DELETED != 1
			                                    INNER JOIN
		                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
			                                    AND CD.IS_DELETED != 1
			                                    INNER JOIN
		                                    CP_COURSE_ROOT AS CS ON CS.COURSE_ROOT_ID = IPN.COURSE_ID
			                                    AND CS.IS_DELETED != 1
	                                    WHERE
		                                    IPN.ACADEMIC_YEAR = ?ACADEMIC_YEAR
			                                  --  AND IPN.MONTH_ID = ?MONTH_ID
			                                    AND IPN.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchPreparationandUpdationNotesForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                IPN.IQAC_ID,
                                CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                STF.STAFF_CODE,
                                CS.COURSE_TITLE,
                                CS.COURSE_CODE,
                                CC.CLASS_CODE,
                                CC.CLASS_NAME,
                                -- CAL.DAY_ORDER_MONTH,
                                IPN.TITLE,
                                IPN.AUTHOR,
                                IPN.PUBLICATION,
                                IPN.YEAR_OF_PUBLICATION,
                                IPN.PLACE_OF_LIBRARY,
                                IPN.WEBSITE_REFFERED,
                                IPN.E_RESOURSE_DEVELOPED,
                                IPN.ENTRY_DTATE,
                                CD.DEPARTMENT,
                                CD.DEPARTMENT_CODE,
                                SS.SUP_SEMESTER_ID AS SEMESTER_ID,
                                IPN.ACADEMIC_YEAR
    
                            FROM
                                IQAC_PREPARATION_AND_UPDATION_OF_NOTES AS IPN
                                    INNER JOIN
                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                    AND STF.IS_DELETED != 1
                                    AND STF.IS_LEFT != 1
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = STF.STAFF_ID
                                    AND CCS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND CCS.CLASS_ID = IPN.CLASS_ID
                                    AND CCS.COURSE_ID = IPN.COURSE_ID
                                    AND CCS.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CCS.SEMESTER_ID
                                    AND SS.IS_DELETED != 1
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = IPN.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                    INNER JOIN
                                CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                    AND CD.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CS ON CS.COURSE_ROOT_ID = IPN.COURSE_ID
                                    AND CS.IS_DELETED != 1
                            WHERE
                                IPN.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                    -- AND IPN.MONTH_ID =?MONTH_ID
                                    AND IPN.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchCoCurrricularAspectsForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                    IPN.IQAC_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    -- CAL.DAY_ORDER_MONTH,
                                    CD.DEPARTMENT_CODE AS DEPT_CODE,
                                    COMMITTEE_NAME AS NAME_OF_COMMITEE,
                                    DATE_FORMAT(IPN.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                    IPN.ACTIVITY_COMPLETED,
                                    IPN.ENTRY_DTATE,
                                    IPN.FILE_PATH
                                FROM
                                    IQAC_CO_CURRICULAR_ASPECTS AS IPN
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        INNER JOIN
                                    IQAC_CURRICULAR_COORDINATOR AS QA ON QA.IQAC_ID=IPN.NAME_OF_COMMITEE
                                        AND QA.IS_DELETED!=1
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                        AND CD.IS_DELETED != 1
                                WHERE
                                    IPN.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        -- AND IPN.MONTH_ID in (?MONTH_ID)
                                        AND IPN.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchExtensionWorksForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                    IPN.IQAC_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    -- CAL.DAY_ORDER_MONTH,
                                    CD.DEPARTMENT_CODE AS DEPT_CODE,
                                    IPN.NAME_OF_THE_PROGRAMME,
                                    DATE_FORMAT(IPN.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                    IPN.ACTIVITY_COMPLETED,
                                    IPN.ENTRY_DTATE,
                                    IPN.NO_OF_PARTICIPANTS,
                                    IPN.PLACE,
                                    IPN.FILE_PATH
                                FROM
                                    IQAC_EXTENSION_WORKS AS IPN
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                        AND CD.IS_DELETED != 1
                                WHERE
                                    IPN.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                       -- AND IPN.MONTH_ID in(?MONTH_ID)
                                        AND IPN.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchPersonalAcademicActivityForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        IPN.IQAC_ID,
                                        CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CLASS_CODE,
                                        CD.DEPARTMENT_CODE AS DEPT_CODE,
                                        DATE_FORMAT(IPN.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        IPN.ACTIVITY,
                                        IPN.TOPIC,
                                        IPN.IF_OTHERS_SPECIFY,
                                        IPN.NO_OF_PARTICIPANTS,
                                        IPN.ENTRY_DTATE
                                    FROM
                                        IQAC_PERSONAL_ACADEMIC_ACTIVITY AS IPN
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IPN.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                            -- AND IPN.MONTH_ID =?MONTH_ID
                                            AND IPN.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchWorkDoneDataActivityForReportByAcademicYearandMonth: // Not In Use
                    {
                        sQuery = @"SELECT 
                                IPN.IQAC_ID,
                               CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                 STF.STAFF_CODE,
                                CS.COURSE_TITLE,
                                CC.CLASS_NAME,
                                CD.DEPARTMENT,
                                CAL.DAY_ORDER_MONTH,
                                IPN.UNIT,
                                IPN.ALLOTTED,
                                IPN.COVERED,
                                IPN.ENTRY_DTATE
    
                            FROM
                                iqac_work_done_data AS IPN
                                    INNER JOIN
                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                    AND STF.IS_DELETED != 1
                                    AND STF.IS_LEFT != 1
                                    INNER JOIN
                                CALENDER AS CAL ON CAL.DAY_ID = IPN.MONTH_ID
                                    AND CAL.IS_DELETED != 1
                                    AND CAL.ACADEMIC_YEAR = IPN.ACADEMIC_YEAR
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = STF.STAFF_ID
                                    AND CCS.ACADEMIC_YEAR = CAL.ACADEMIC_YEAR
                                    AND CCS.CLASS_ID = IPN.CLASS_ID
                                    AND CCS.COURSE_ID = IPN.COURSE_ID
                                    AND CCS.IS_DELETED != 1
                                    INNER JOIN
                                ACADEMIC_YEAR AS AY ON AY.AC_YEAR = CCS.ACADEMIC_YEAR
                                    AND AY.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CCS.SEMESTER_ID
                                    AND SS.IS_DELETED != 1
                                   
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                    INNER JOIN
                                CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                    AND CD.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CS ON CS.COURSE_ROOT_ID = CCS.COURSE_ID
                                    AND CS.IS_DELETED != 1
                            WHERE
                                IPN.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                    AND IPN.MONTH_ID =?MONTH_ID
                                    AND IPN.IS_DELETED != 1
                            ORDER BY IPN.IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchExamainationDutiesInternalForReportByAcademicYearandMonth: // Not In Use
                    {
                        sQuery = @"SELECT 
                                IPN.IQAC_ID,
                               CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                 STF.STAFF_CODE,
                                CS.COURSE_TITLE,
                                CC.CLASS_NAME,
                                CAL.DAY_ORDER_MONTH,
                                IPN.EXAMINATION_DTATE,
                                IPN.PAPER_SETTING,
                                IPN.INVIGILATORS,
                                IPN.PAPER,
                                IPN.PAPERS,
                                IPN.ASSIGNMENTS,
                                CD.DEPARTMENT,
                                IPN.ENTRY_DTATE
    
                            FROM
                                iqac_examination_duties_internal AS IPN
                                    INNER JOIN
                                STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                    AND STF.IS_DELETED != 1
                                    AND STF.IS_LEFT != 1
                                    INNER JOIN
                                CALENDER AS CAL ON CAL.DAY_ID = IPN.MONTH_ID
                                    AND CAL.IS_DELETED != 1
                                    AND CAL.ACADEMIC_YEAR = IPN.ACADEMIC_YEAR
                                    INNER JOIN
                                CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = STF.STAFF_ID
                                    AND CCS.ACADEMIC_YEAR = CAL.ACADEMIC_YEAR
                                    AND CCS.CLASS_ID = IPN.CLASS_ID
                                    AND CCS.COURSE_ID = IPN.COURSE_ID
                                    AND CCS.IS_DELETED != 1
                                    INNER JOIN
                                ACADEMIC_YEAR AS AY ON AY.AC_YEAR = CCS.ACADEMIC_YEAR
                                    AND AY.IS_DELETED != 1
                                    INNER JOIN
                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = CCS.SEMESTER_ID
                                    AND SS.IS_DELETED != 1
                                   
                                    INNER JOIN
                                CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                    AND CC.IS_DELETED != 1
                                    INNER JOIN
                                CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CC.DEPARTMENT
                                    AND CD.IS_DELETED != 1
                                    INNER JOIN
                                CP_COURSE_ROOT AS CS ON CS.COURSE_ROOT_ID = CCS.COURSE_ID
                                    AND CS.IS_DELETED != 1
                            WHERE
                                IPN.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                    AND IPN.MONTH_ID =?MONTH_ID
                                    AND IPN.IS_DELETED != 1
                            ORDER BY IPN.IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchAwardsForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        L.LEVEL,
                                        IA.AWARD_TITLE,
                                        IA.YEAR,
                                        IA.AWARDED_BY,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_AWARDS AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_LEVEL AS L ON L.LEVEL_ID = IA.LEVEL_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                          --  AND IA.MONTH_ID = ?MONTH_ID
                                            AND IA.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchAwardsForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                            SP.STAFF_ID,
                                            CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                    '  ',
                                                    IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                            CD.DEPARTMENT,
                                            SP.STAFF_CODE,
                                            L.LEVEL,
                                            IA.AWARD_TITLE,
                                            IA.YEAR,
                                            IA.AWARDED_BY,
                                            IA.FILE_PATH
                                        FROM
                                            IQAC_AWARDS AS IA
                                                INNER JOIN
                                            STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                                AND SP.IS_DELETED != 1
                                                INNER JOIN
                                            SUP_LEVEL AS L ON L.LEVEL_ID = IA.LEVEL_ID
                                                INNER JOIN
                                            CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                                AND CD.IS_DELETED != 1
                                        WHERE
                                            IA.YEAR = ?AC_YEAR AND IA.IS_DELETED != 1
                                        ORDER BY SP.DEPARTMENT , SP.STAFF_ID; ";
                        break;
                    }
                case IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.ASSOCIATION_NAME,
                                        IA.POSISION_HELD,
                                        IA.PERIOD,
                                        IA.PERIOD_TO,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_SUBJECT_ASSOCIATION AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1   
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                           -- AND IA.MONTH_ID = ?MONTH_ID
                                            AND IA.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchSubjectAssociationForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.ASSOCIATION_NAME,
                                        IA.POSISION_HELD,
                                        IA.PERIOD,
                                        IA.PERIOD_TO,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_SUBJECT_ASSOCIATION AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.PERIOD = ?AC_YEAR AND IA.IS_DELETED != 1
                                    ORDER BY SP.DEPARTMENT , SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchHonorForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.TITLE_OF_TALK,
                                        IA.PROGRAMME_TITLE,
                                        IA.INSTITION_NAME,
                                        IA.PLACE,
                                        DATE_FORMAT(IA.FROM_DATE,'%d/%m/%Y') AS FROM_DATE,
                                        DATE_FORMAT(IA.TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        IF(HONOR_TYPE_ID='1','Resource Person',IF(HONOR_TYPE_ID='2','Chair Person',IF(HONOR_TYPE_ID=3,'Convenor/Orgnanzing Secrerty',''))) AS HONOR_TYPE_ID ,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_HONOURS AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1 
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                          --  AND IA.MONTH_ID = ?MONTH_ID
                                            AND IA.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchMemberShipForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        cd.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.BOARD_NAME,
                                        IA.COLLEGE_UNIVERSITY,
                                        IA.PERIOD,
                                        IA.PERIOD_TO,
                                        IF(MEMBERSHIP_TYPE_ID='1','Board Of Study Member',IF(MEMBERSHIP_TYPE_ID='2','Academic counsil Member',IF(MEMBERSHIP_TYPE_ID='3','Syndicate Member',''))) AS MEMBERSHIP_TYPE_ID,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_MEMBERSHIP AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1 
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                          --  AND IA.MONTH_ID = ?MONTH_ID
                                            AND IA.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchMemberShipForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        cd.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.BOARD_NAME,
                                        IA.COLLEGE_UNIVERSITY,
                                        IA.PERIOD,
                                        IA.PERIOD_TO,
                                        IF(MEMBERSHIP_TYPE_ID = '1',
                                            'Board Of Study Member',
                                            IF(MEMBERSHIP_TYPE_ID = '2',
                                                'Academic counsil Member',
                                                IF(MEMBERSHIP_TYPE_ID = '3',
                                                    'Syndicate Member',
                                                    ''))) AS MEMBERSHIP_TYPE_ID,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_MEMBERSHIP AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.PERIOD =?AC_YEAR AND IA.IS_DELETED != 1
                                    ORDER BY SP.DEPARTMENT , SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchProjectForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        PY.PROJECT_TYPE,
                                        TF.FUNDING_TYPE AS TYPE_OF_FUNDING, 
                                        IP.PROJECT_TITLE,
                                        IP.DURATION,
                                        IP.PERIOD,
                                        IP.PERIOD_TO,
                                        IP.FUNDING_AGENCY,
                                        IP.AMOUNT,
                                        IP.FILE_PATH
                                    FROM
                                        IQAC_PROJECT AS IP
                                            INNER JOIN
	                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=IP.STAFF_ID
		                                    AND SP.IS_DELETED!=1
		                                    INNER JOIN
                                        SUP_IQAC_PROJECT_TYPE AS PY ON PY.PROJECT_TYPE_ID = IP.PROJECT_TYPE
                                            INNER JOIN
                                        SUP_IQAC_TYPE_OF_FUNDING AS TF ON TF.TYPE_FUNDING_ID = IP.TYPE_OF_FUNDING 
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IP.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                           -- AND IP.MONTH_ID = ?MONTH_ID
                                            AND IP.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchProjectForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        PY.PROJECT_TYPE,
                                        TF.FUNDING_TYPE AS TYPE_OF_FUNDING,
                                        IP.PROJECT_TITLE,
                                        IP.DURATION,
                                        IP.PERIOD,
                                        IP.PERIOD_TO,
                                        IP.FUNDING_AGENCY,
                                        IP.AMOUNT,
                                        IP.FILE_PATH
                                    FROM
                                        IQAC_PROJECT AS IP
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IP.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_IQAC_PROJECT_TYPE AS PY ON PY.PROJECT_TYPE_ID = IP.PROJECT_TYPE
                                            INNER JOIN
                                        SUP_IQAC_TYPE_OF_FUNDING AS TF ON TF.TYPE_FUNDING_ID = IP.TYPE_OF_FUNDING
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IP.PERIOD =?AC_YEAR AND IP.IS_DELETED != 1
                                    ORDER BY SP.DEPARTMENT , SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IR.CATEGORY_NAME AS CATEGORY,
                                        I.STUDENT_NAME,
                                        I.REGISTER_NUMBER,
                                        I.PROJECT_TITLE,
                                        I.REGISTRATION_YEAR,
                                        I.COMPLITION_YEAR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_RESEARCH_GUDIENCE AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_IQAC_RESEARCH_GUIDENCE_CATEGORY AS IR ON IR.IQAC_CATEGORY_ID = I.CATEGORY_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchResearchGuidenceForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IR.CATEGORY_NAME AS CATEGORY,
                                        I.STUDENT_NAME,
                                        I.REGISTER_NUMBER,
                                        I.PROJECT_TITLE,
                                        I.REGISTRATION_YEAR,
                                        I.COMPLITION_YEAR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_RESEARCH_GUDIENCE AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_IQAC_RESEARCH_GUIDENCE_CATEGORY AS IR ON IR.IQAC_CATEGORY_ID = I.CATEGORY_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.COMPLITION_YEAR = ?AC_YEAR
                                            AND I.IS_DELETED != 1
                                    ORDER BY SP.DEPARTMENT , SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchPHDGuidenceForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IR.CATEGORY_NAME,
                                        I.STUDENT_NAME,
                                        I.REGISTER_NUMBER,
                                        I.PROJECT_TITLE,
                                        I.REGISTRATION_YEAR,
                                        DATE_FORMAT(I.GUDIENCE_DATE,'%d/%m/%Y') AS GUDIENCE_DATE,
                                        S.STATUS_NAME AS STATUS,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_PHD_GUDIENCE AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_IQAC_PHD_GUIDENCE_CATEGORY AS IR ON IR.IQAC_CATEGORY_ID = I.CATEGORY_ID
		                                    INNER JOIN
	                                    SUP_IQAC_STATUS AS S ON S.IQAC_STATUS_ID=I.STATUS
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchPublicationAndPrasentationForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        PT.PROGRAMME_TYPE AS PROGRAMME_TYPE_ID,
                                        L.LEVEL AS LEVEL_ID,
                                        DATE_FORMAT(I.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                       -- DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        I.ACTIVITY_TITLE,
                                        I.ORGANIZER,
                                        I.PLACE,
                                        I.PAPER_TITLE,
                                        IF(I.PRESENTING_AUTHOR='1','YES',IF(I.PRESENTING_AUTHOR='2','NO','')) AS PRESENTING_AUTHOR,
                                        IF(I.ACTIVITY_ID='1','Presented',IF(I.ACTIVITY_ID='2','Participated','')) AS ACTIVITY_ID,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_STAFF_ACTIVITY AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
		                                    INNER JOIN
	                                    SUP_IQAC_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID=I.PROGRAMME_TYPE_ID
		                                    INNER JOIN
	                                    SUP_LEVEL AS L ON L.LEVEL_ID=I.LEVEL_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchPaperPublicationForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        I.PAPER_TITLE,
                                        I.JOURNEL_NAME,
                                        I.PUBLICATION_YEAR,
                                        I.PUBLICATION_VOLUME,
                                        I.ISSUE_NUMBER,
                                        I.UGC_NUMBER,
                                        I.PAGE_NUMBER,
                                        I.ISSN_NUMBER,
                                        I.ISBN_NUMBER,
                                        IF(I.SCOPUS_ID='1','YES',IF(I.SCOPUS_ID='2','NO','')) AS SCOPUS_ID,
                                        IF(I.WEB_OF_SCIENCE='1','YES',IF(I.WEB_OF_SCIENCE='2','NO','')) AS WEB_OF_SCIENCE,
                                        IF(I.UGC_LISTED='1','YES',IF(I.UGC_LISTED='2','NO','')) AS UGC_LISTED,
                                        I.SERIAL_NUMBER,
                                        I.IMPACT_FACT,
                                        I.CITATION_INDEX,
                                        IF(I.CORRESPONDING_AUTHOR='1','YES',IF(I.CORRESPONDING_AUTHOR='2','NO','')) AS CORRESPONDING_AUTHOR,
                                        IF(I.FIRST_AUTHOR='1','YES',IF(I.FIRST_AUTHOR='2','NO','')) AS FIRST_AUTHOR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_PAPER_PUBLICATION AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchPaperPublicationForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        I.PAPER_TITLE,
                                        I.JOURNEL_NAME,
                                        I.PUBLICATION_YEAR,
                                        I.PUBLICATION_VOLUME,
                                        I.ISSUE_NUMBER,
                                        I.UGC_NUMBER,
                                        I.PAGE_NUMBER,
                                        I.ISSN_NUMBER,
                                        I.ISBN_NUMBER,
                                        IF(I.SCOPUS_ID='1','YES',IF(I.SCOPUS_ID='2','NO','')) AS SCOPUS_ID,
                                        IF(I.WEB_OF_SCIENCE='1','YES',IF(I.WEB_OF_SCIENCE='2','NO','')) AS WEB_OF_SCIENCE,
                                        IF(I.UGC_LISTED='1','YES',IF(I.UGC_LISTED='2','NO','')) AS UGC_LISTED,
                                        I.SERIAL_NUMBER,
                                        I.IMPACT_FACT,
                                        I.CITATION_INDEX,
                                        IF(I.CORRESPONDING_AUTHOR='1','YES',IF(I.CORRESPONDING_AUTHOR='2','NO','')) AS CORRESPONDING_AUTHOR,
                                        IF(I.FIRST_AUTHOR='1','YES',IF(I.FIRST_AUTHOR='2','NO','')) AS FIRST_AUTHOR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_PAPER_PUBLICATION AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.PUBLICATION_YEAR = ?AC_YEAR
                                            AND I.IS_DELETED != 1 
                                            ORDER BY SP.DEPARTMENT , SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchBookPublicationForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IF(I.CHAPTER='1','Chapter',IF(I.CHAPTER='2','Book','')) AS CHAPTER,
                                        I.BOOK_TITLE,
                                        I.CHAPTER_TITLE,
                                        I.PUBLISHER,
                                        I.PUBLICATION_YEAR,
                                        I.ISBN_NUMBER,
                                        IF(I.SINGLE_AUTHOR='1','YES',IF(I.SINGLE_AUTHOR='2','NO','')) AS SINGLE_AUTHOR,
                                        IF(I.EDITOR='1','YES',IF(I.EDITOR='2','NO','')) AS EDITOR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_BOOK_PUBLICATION AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchBookPublicationForReportByAcademicYear:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IF(I.CHAPTER='1','Chapter',IF(I.CHAPTER='2','Book','')) AS CHAPTER,
                                        I.BOOK_TITLE,
                                        I.CHAPTER_TITLE,
                                        I.PUBLISHER,
                                        I.PUBLICATION_YEAR,
                                        I.ISBN_NUMBER,
                                        IF(I.SINGLE_AUTHOR='1','YES',IF(I.SINGLE_AUTHOR='2','NO','')) AS SINGLE_AUTHOR,
                                        IF(I.EDITOR='1','YES',IF(I.EDITOR='2','NO','')) AS EDITOR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_BOOK_PUBLICATION AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.PUBLICATION_YEAR = ?AC_YEAR
                                            AND I.IS_DELETED != 1 
										ORDER BY SP.DEPARTMENT , SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchAssociationActivityForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        I.IF_OTHERS_SPECIFY,
                                        DATE_FORMAT(I.ACTIVITY_DATE,'%d/%m/%Y') as ACTIVITY_DATE,
                                        DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') as TO_DATE,
                                        I.DETAILS,
                                        I.FILE_PATH,
                                        SS.SHIFT_NAME,
                                        IA.ACTIVITY_NAME
                                    
                                    FROM
                                        IQAC_ASSOCIATION_ACTIVITIES AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
										SUP_SHIFT AS SS ON SS.SHIFT_ID=I.SHIFT_ID
											INNER JOIN
										SUP_IQAC_ACTIVITY AS IA ON IA.ACTIVITY_ID=I.ACTIVITY_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchSeminarWorkshopOrganizedForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        SP.STAFF_CODE,
                                        CD.DEPARTMENT,
                                        I.IF_OTHER_SPECIFY,
                                        DATE_FORMAT(I.ORGANIZED_DATE,'%d/%m/%Y') AS ORGANIZED_DATE,
                                        DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        I.FILE_PATH,
                                        PT.PROGRAMME_TYPE,
                                        L.LEVEL
                                    FROM
                                        IQAC_SEMINOR_WORKSOP_ORGANIZED AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
										SUP_IQAC_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID=I.PROGRAMME_TYPE_ID
											INNER JOIN
										SUP_LEVEL AS L ON L.LEVEL_ID=I.LEVEL_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                case IQACEnumCommands.FetchDepartmentDailyActivityForReportByAcademicYearandMonth:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        SP.STAFF_CODE,
                                        CD.DEPARTMENT,
                                        I.ACTIVITY,
                                        DATE_FORMAT(I.ACTIVITY_DATE,'%d/%m/%Y') as ACTIVITY_DATE,
                                       -- DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') as TO_DATE,
                                        I.FILE_PATH
                                    FROM
                                        DEPT_DAILY_ACTIVITY AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND I.IS_DELETED != 1 ";
                        break;
                    }
                #endregion

                #region Lesson Plan
                case IQACEnumCommands.FetchLessonPlan:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID, 
                                        STAFF_ID,
                                        COURSE_ID, 
                                        TEACHING_METHOD, 
                                        CYCLE, 
                                        UNIT, 
                                        TOPIC, 
                                        STATUS
                                    FROM
                                        STF_LESSON_PLAN
                                    WHERE
                                        IS_DELETED != 1 AND ACADEMIC_YEAR = ?AC_YEAR
                                            AND STAFF_ID = ?STAFF_ID
                                            AND COURSE_ID = ?COURSE_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchListLessonPlan:
                    {
                        sQuery = @"SELECT 
                                        ST.IQAC_ID,
                                        ST.STAFF_ID,
                                        ST.COURSE_ID,
                                        ST.CYCLE,
                                        ST.UNIT,
                                        ST.TOPIC,
                                        ST.TEACHING_METHOD,
                                        ST.STATUS,
                                        ST.ACADEMIC_YEAR,
                                        SP.STAFF_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME, ''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CCR.SEMESTER_ID,
                                        CCR.COURSE_CODE,
                                        CCR.COURSE_TITLE,
                                        D.DEPARTMENT
                                    FROM
                                        STF_LESSON_PLAN AS ST
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = ST.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = ST.COURSE_ID
                                            AND CCR.IS_DELETED != 1
                                            INNER JOIN
	                                    CP_DEPARTMENT AS D ON D.DEPARTMENT_ID=SP.DEPARTMENT
		                                    AND D.IS_DELETED!=1
                                    WHERE
                                        ST.IS_DELETED != 1
                                            AND ST.ACADEMIC_YEAR = ?AC_YEAR
                                            AND ST.STAFF_ID = ?STAFF_ID
                                            AND ST.COURSE_ID = ?COURSE_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveLessonPlan:
                    {
                        sQuery = @"INSERT INTO STF_LESSON_PLAN
                                    (
	                                    STAFF_ID,
	                                    COURSE_ID,
	                                    CYCLE,
	                                    UNIT,
	                                    TOPIC,
	                                    STATUS,
	                                    TEACHING_METHOD,
	                                    ENTRY_DTATE,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?COURSE_ID,
	                                    ?CYCLE,
	                                    ?UNIT,
	                                    ?TOPIC,
	                                    ?STATUS,
	                                    ?TEACHING_METHOD,
	                                    CURDATE(),
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateLessonPlan:
                    {
                        sQuery = @"UPDATE STF_LESSON_PLAN
                                    SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    COURSE_ID = ?COURSE_ID,
	                                    CYCLE = ?CYCLE,
	                                    UNIT = ?UNIT,
	                                    TOPIC = ?TOPIC,
	                                    STATUS = ?STATUS,
	                                    TEACHING_METHOD = ?TEACHING_METHOD,
	                                    ENTRY_DTATE = CURDATE(),
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteLessonPlan:
                    {
                        sQuery = @"UPDATE STF_LESSON_PLAN SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                #endregion

                #region IQAC
                case IQACEnumCommands.FetchIQACQuestion:
                    {
                        sQuery = @"SELECT 
                                        IQAC_QUESTION_ID, 
                                        IQAC_QUESTIONS, 
                                        ACADEMIC_YEAR
                                    FROM
                                        IQAC_QUESTIONS
                                    WHERE
                                        IS_ACTIVE = 1 AND IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region 8. Achivements

                // Awards ,,,,,.....

                case IQACEnumCommands.FetchAwardsList:
                    {
                        sQuery = @"SELECT 
                                        AWARD_ID, 
                                        AWARD_TITLE, 
                                        STAFF_ID, 
                                        YEAR, 
                                        AWARDED_BY, 
                                        L.LEVEL AS LEVEL_ID
                                    FROM
                                        IQAC_AWARDS AS AW
                                            INNER JOIN
                                        SUP_LEVEL AS L ON L.LEVEL_ID=AW.LEVEL_ID
                                    WHERE
                                        AW.IS_DELETED != 1 -- AND MONTH_ID = ?MONTH_ID                                        
                                       --  AND AW.ACADEMIC_YEAR IN (?AC_YEAR)
                                          AND AW.STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchAwardsById:
                    {
                        sQuery = @"SELECT 
                                        AWARD_ID, 
                                        AWARD_TITLE, 
                                        STAFF_ID, 
                                        YEAR, 
                                        AWARDED_BY, 
                                        LEVEL_ID,
                                        FILE_PATH
                                    FROM
                                        IQAC_AWARDS
                                    WHERE
                                        AWARD_ID=?AWARD_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveAwards:
                    {
                        sQuery = @"INSERT INTO IQAC_AWARDS
                                    (
	                                    AWARD_TITLE,
	                                    STAFF_ID,
	                                    YEAR,
	                                    AWARDED_BY,
	                                    LEVEL_ID,
	                                    ENTRY_DATE,
	                                    ACADEMIC_YEAR,
	                                    MONTH_ID
                                    )
                                    VALUES
                                    (
                                        ?AWARD_TITLE,
	                                    ?STAFF_ID,
	                                    ?YEAR,
	                                    ?AWARDED_BY,
	                                    ?LEVEL_ID,
	                                    CURDATE(),
	                                    ?AC_YEAR,
	                                    ?MONTH_ID
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateAwards:
                    {
                        sQuery = @"UPDATE IQAC_AWARDS
                                    SET
	                                    AWARD_TITLE = ?AWARD_TITLE,
	                                    STAFF_ID = ?STAFF_ID,
	                                    YEAR = ?YEAR,
	                                    AWARDED_BY = ?AWARDED_BY,
	                                    LEVEL_ID = ?LEVEL_ID,
	                                    ENTRY_DATE = CURDATE(),
	                                    ACADEMIC_YEAR = ?AC_YEAR,
	                                    MONTH_ID = ?MONTH_ID,
                                        FILE_PATH=?FILE_PATH
                                    WHERE AWARD_ID = ?AWARD_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteAwards:
                    {
                        sQuery = @"UPDATE IQAC_AWARDS SET IS_DELETED=1 WHERE AWARD_ID = ?AWARD_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateAwardFile:
                    {
                        sQuery = @"UPDATE IQAC_AWARDS SET FILE_PATH=?FILE_PATH WHERE AWARD_ID = ?AWARD_ID;";
                        break;
                    }

                // Subject Association ,...
                case IQACEnumCommands.FetchSubjectAssociationList:
                    {
                        sQuery = @"SELECT 
                                        SUBJECT_ASSOCIATION_ID,
                                        STAFF_ID,
                                        ASSOCIATION_NAME,
                                        POSISION_HELD,
                                        PERIOD,PERIOD_TO,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        MONTH_ID
                                    FROM
                                        IQAC_SUBJECT_ASSOCIATION 
                                    WHERE
                                        IS_DELETED != 1 --  AND MONTH_ID = 1 
                                           --  AND ACADEMIC_YEAR IN (?AC_YEAR)
		                                    AND STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchSubjectAssociationById:
                    {
                        sQuery = @"SELECT 
                                        SUBJECT_ASSOCIATION_ID,
                                        STAFF_ID,
                                        ASSOCIATION_NAME,
                                        POSISION_HELD,
                                        PERIOD,PERIOD_TO,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        MONTH_ID,
                                        FILE_PATH
                                    FROM
                                        IQAC_SUBJECT_ASSOCIATION 
                                    WHERE
                                        SUBJECT_ASSOCIATION_ID=?SUBJECT_ASSOCIATION_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveSubjectAssociation:
                    {
                        sQuery = @"INSERT INTO IQAC_SUBJECT_ASSOCIATION
                                    (
                                        STAFF_ID,
                                        ASSOCIATION_NAME,
                                        POSISION_HELD,
                                        PERIOD,
                                        PERIOD_TO,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        MONTH_ID
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?ASSOCIATION_NAME,
                                        ?POSISION_HELD,
                                        ?PERIOD,
                                        ?PERIOD_TO,
                                        CURDATE(),
                                        ?AC_YEAR,
                                        ?MONTH_ID
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateSubjectAssociation:
                    {
                        sQuery = @"UPDATE IQAC_SUBJECT_ASSOCIATION
                                    SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    ASSOCIATION_NAME = ?ASSOCIATION_NAME,
	                                    POSISION_HELD = ?POSISION_HELD,
	                                    PERIOD = ?PERIOD,PERIOD_TO=?PERIOD_TO,
	                                    ENTRY_DATE = CURDATE(),
	                                    ACADEMIC_YEAR = ?AC_YEAR,
	                                    MONTH_ID = ?MONTH_ID,
                                        FILE_PATH=?FILE_PATH
                                    WHERE SUBJECT_ASSOCIATION_ID = ?SUBJECT_ASSOCIATION_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateSubjectAssociationFile:
                    {
                        sQuery = @"UPDATE IQAC_SUBJECT_ASSOCIATION SET FILE_PATH=?FILE_PATH WHERE SUBJECT_ASSOCIATION_ID = ?SUBJECT_ASSOCIATION_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteSubjectAssociation:
                    {
                        sQuery = @"UPDATE IQAC_SUBJECT_ASSOCIATION SET IS_DELETED=1 WHERE SUBJECT_ASSOCIATION_ID = ?SUBJECT_ASSOCIATION_ID;";
                        break;
                    }



                // Honor 
                case IQACEnumCommands.FetchHonorList:
                    {
                        sQuery = @"SELECT 
                                        HONOR_ID,
                                        HONOR_TYPE_ID,STAFF_ID,
                                        IF(HONOR_TYPE_ID='1','Resource Person',if(HONOR_TYPE_ID='2','Chair Person',IF(HONOR_TYPE_ID='3','Convenor/Orgnanzing Secrerty',''))) AS HONOR_TYPE_ID,
                                        TITLE_OF_TALK,
                                        PROGRAMME_TITLE,
                                        INSTITION_NAME,
                                        PLACE,
                                        DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                        DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE
                                    FROM
                                        IQAC_HONOURS
                                    WHERE
                                        IS_DELETED != 1 --  AND MONTH_ID = ?MONTH_ID
                                          --  AND ACADEMIC_YEAR IN (?AC_YEAR)
		                                    AND STAFF_ID=?STAFF_ID;";
                        break;
                    }

                case IQACEnumCommands.FetchHonorById:
                    {
                        sQuery = @"SELECT 
                                        HONOR_ID,
                                        HONOR_TYPE_ID,
                                        STAFF_ID,
                                       -- IF(HONOR_TYPE_ID='1','Resource Person',if(HONOR_TYPE_ID='2','Chair Person',IF(HONOR_TYPE_ID='3','Convenor/Orgnanzing Secrerty',''))) AS HONOR_TYPE_ID,
                                        TITLE_OF_TALK,
                                        PROGRAMME_TITLE,
                                        INSTITION_NAME,
                                        PLACE,
                                        DATE_FORMAT(FROM_DATE, '%d/%m/%Y') AS FROM_DATE,
                                        DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                       FILE_PATH
                                    FROM
                                        IQAC_HONOURS
                                    WHERE
                                        HONOR_ID = ?HONOR_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveHonor:
                    {
                        sQuery = @"INSERT INTO IQAC_HONOURS
                                    (
                                        HONOR_TYPE_ID,STAFF_ID,
                                        TITLE_OF_TALK,
                                        PROGRAMME_TITLE,
                                        INSTITION_NAME,
                                        PLACE,
                                        FROM_DATE,
                                        TO_DATE,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        MONTH_ID
                                    )
                                    VALUES
                                    (
                                        ?HONOR_TYPE_ID,?STAFF_ID,
                                        ?TITLE_OF_TALK,
                                        ?PROGRAMME_TITLE,
                                        ?INSTITION_NAME,
                                        ?PLACE,
                                        ?FROM_DATE,
                                        ?TO_DATE,
                                        CURDATE(),
                                        ?AC_YEAR,
                                        ?MONTH_ID
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateHonorFile:
                    {
                        sQuery = @"UPDATE IQAC_HONOURS SET FILE_PATH=?FILE_PATH WHERE HONOR_ID = ?HONOR_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateHonor:
                    {
                        sQuery = @"UPDATE IQAC_HONOURS
                                    SET
	                                    HONOR_TYPE_ID = ?HONOR_TYPE_ID,
	                                    TITLE_OF_TALK = ?TITLE_OF_TALK,
	                                    PROGRAMME_TITLE = ?PROGRAMME_TITLE,
	                                    INSTITION_NAME = ?INSTITION_NAME,
	                                    PLACE = ?PLACE,
	                                    FROM_DATE = ?FROM_DATE,
                                        TO_DATE = ?TO_DATE,
	                                    ENTRY_DATE = CURDATE(),
	                                    ACADEMIC_YEAR = ?AC_YEAR,
	                                    MONTH_ID = ?MONTH_ID,
                                        FILE_PATH=?FILE_PATH
                                    WHERE HONOR_ID = ?HONOR_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteHonor:
                    {
                        sQuery = @"UPDATE IQAC_HONOURS SET IS_DELETED=1 WHERE HONOR_ID = ?HONOR_ID;";
                        break;
                    }


                // MemberShip 

                case IQACEnumCommands.FetchMembershipList:
                    {
                        sQuery = @"SELECT 
                                        MEMBERSHIP_ID,
                                        STAFF_ID,
                                        IF(MEMBERSHIP_TYPE_ID='1','Board Of Study Member',IF(MEMBERSHIP_TYPE_ID='2','Academic counsil Member',IF(MEMBERSHIP_TYPE_ID='3','Syndicate Member',''))) AS MEMBERSHIP_TYPE,
                                        MEMBERSHIP_TYPE_ID,
                                        BOARD_NAME,
                                        COLLEGE_UNIVERSITY,
                                        PERIOD,PERIOD_TO,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        MONTH_ID
                                    FROM
                                        IQAC_MEMBERSHIP
                                    WHERE
                                        IS_DELETED != 1 --  AND MONTH_ID = 1
                                           -- AND ACADEMIC_YEAR IN (?AC_YEAR)
                                           AND STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchMembershipById:
                    {
                        sQuery = @"SELECT 
                                        MEMBERSHIP_ID,
                                        STAFF_ID,
                                        MEMBERSHIP_TYPE_ID,
                                        BOARD_NAME,
                                        COLLEGE_UNIVERSITY,
                                        PERIOD,PERIOD_TO,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR,
                                        MONTH_ID,
                                        FILE_PATH
                                    FROM
                                        IQAC_MEMBERSHIP
                                    WHERE
                                        MEMBERSHIP_ID=?MEMBERSHIP_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveMembership:
                    {
                        sQuery = @"INSERT INTO IQAC_MEMBERSHIP
                                    (
                                        MEMBERSHIP_TYPE_ID,
	                                    STAFF_ID,
	                                    BOARD_NAME,
	                                    COLLEGE_UNIVERSITY,
	                                    PERIOD,PERIOD_TO,
	                                    ENTRY_DATE,
	                                    ACADEMIC_YEAR,
	                                    MONTH_ID
                                    )
                                    VALUES
                                    (
                                        ?MEMBERSHIP_TYPE_ID,
                                        ?STAFF_ID,
	                                    ?BOARD_NAME,
	                                    ?COLLEGE_UNIVERSITY,
	                                    ?PERIOD,?PERIOD_TO,
	                                    CURDATE(),
	                                    ?AC_YEAR,
	                                    ?MONTH_ID
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateMembershipFilePath:
                    {
                        sQuery = @"UPDATE IQAC_MEMBERSHIP SET FILE_PATH=?FILE_PATH WHERE MEMBERSHIP_ID = ?MEMBERSHIP_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateMembership:
                    {
                        sQuery = @"UPDATE IQAC_MEMBERSHIP
                                    SET
                                        MEMBERSHIP_TYPE_ID=?MEMBERSHIP_TYPE_ID,
	                                    STAFF_ID =?STAFF_ID,
	                                    BOARD_NAME =?BOARD_NAME,
	                                    COLLEGE_UNIVERSITY =?COLLEGE_UNIVERSITY,
	                                    PERIOD =?PERIOD,
	                                    PERIOD_TO =?PERIOD_TO,
	                                    ENTRY_DATE =CURDATE(),
	                                    ACADEMIC_YEAR =?AC_YEAR,
	                                    MONTH_ID =?MONTH_ID,
                                        FILE_PATH=?FILE_PATH
                                    WHERE MEMBERSHIP_ID =?MEMBERSHIP_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteMembership:
                    {
                        sQuery = @"UPDATE IQAC_MEMBERSHIP SET IS_DELETED=1 WHERE MEMBERSHIP_ID=?MEMBERSHIP_ID;";
                        break;
                    }
                #endregion

                #region 10 A. Prasented / Participated (Staff Activity)
                case IQACEnumCommands.FetchPublicationAndPrasentationList:
                    {
                        sQuery = @"SELECT 
                                        IQAC_STAFF_ACTIVITY_ID,
                                        IF(ACTIVITY_ID='1','PRESENTED','PARTICIPATED') AS ACTIVITY_ID,
                                        PT.PROGRAMME_TYPE AS PROGRAMME_TYPE_ID,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') as ACTIVITY_DATE,
                                        DATE_FORMAT(ACTIVITY_DATE_TO,'%d/%m/%Y') as ACTIVITY_DATE_TO,
                                        SA.ACTIVITY_TITLE,
                                        SA.ORGANIZER,
                                        SA.PLACE,
                                        L.LEVEL AS LEVEL_ID,
                                        SA.PAPER_TITLE,
                                        SA.ISBN_NUMBER,
                                        IF(PUBLISHED_IN_PROCEEDING=1,'YES','NO') AS PUBLISHED_IN_PROCEEDING,
                                        IF(PRESENTING_AUTHOR=1,'YES','NO') AS PRESENTING_AUTHOR
                                    FROM
                                        IQAC_STAFF_ACTIVITY AS SA
                                            INNER JOIN
                                        SUP_IQAC_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID = SA.PROGRAMME_TYPE_ID
                                            INNER JOIN
                                        SUP_LEVEL AS L ON L.LEVEL_ID = SA.LEVEL_ID
                                    WHERE
                                       --  SA.ACADEMIC_YEAR IN (?AC_YEAR)
                                             SA.IS_DELETED != 1
                                            AND SA.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchPublicationAndPrasentationById:
                    {
                        sQuery = @"SELECT 
                                        IQAC_STAFF_ACTIVITY_ID,
                                        STAFF_ID,
                                        ACTIVITY_ID,
                                        PROGRAMME_TYPE_ID,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        DATE_FORMAT(ACTIVITY_DATE_TO,'%d/%m/%Y') AS ACTIVITY_DATE_TO,
                                        ACTIVITY_TITLE,
                                        ORGANIZER,
                                        PLACE,
                                        LEVEL_ID,
                                        PAPER_TITLE,
                                        ISBN_NUMBER,
                                        PUBLISHED_IN_PROCEEDING,
                                        PRESENTING_AUTHOR,
                                        FILE_PATH
                                    FROM
                                        IQAC_STAFF_ACTIVITY 
                                    WHERE IQAC_STAFF_ACTIVITY_ID=?IQAC_STAFF_ACTIVITY_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdatePublicationAndPrasentation:
                    {
                        sQuery = @"UPDATE IQAC_STAFF_ACTIVITY
                                    SET
	                                    STAFF_ID =?STAFF_ID,
	                                    ACTIVITY_ID =?ACTIVITY_ID,
	                                    PROGRAMME_TYPE_ID =?PROGRAMME_TYPE_ID,
	                                    ACTIVITY_DATE =?ACTIVITY_DATE,
	                                    ACTIVITY_DATE_TO =?ACTIVITY_DATE_TO,
	                                    ACTIVITY_TITLE =?ACTIVITY_TITLE,
	                                    ORGANIZER =?ORGANIZER,
	                                    PLACE =?PLACE,
	                                    LEVEL_ID =?LEVEL_ID,
	                                    PAPER_TITLE =?PAPER_TITLE,
                                        ISBN_NUMBER=?ISBN_NUMBER,
                                        PUBLISHED_IN_PROCEEDING=?PUBLISHED_IN_PROCEEDING,
	                                    PRESENTING_AUTHOR =?PRESENTING_AUTHOR,
	                                    ENTRY_DATE =CURDATE(),
	                                    ACADEMIC_YEAR =?AC_YEAR
                                    WHERE IQAC_STAFF_ACTIVITY_ID =?IQAC_STAFF_ACTIVITY_ID;";
                        break;
                    }
                case IQACEnumCommands.SavePublicationAndPrasentation:
                    {
                        sQuery = @"INSERT INTO IQAC_STAFF_ACTIVITY
                                    (
                                        STAFF_ID,
                                        ACTIVITY_ID,
                                        PROGRAMME_TYPE_ID,
                                        ACTIVITY_DATE,
                                        ACTIVITY_DATE_TO,
                                        ACTIVITY_TITLE,
                                        ORGANIZER,
                                        PLACE,
                                        LEVEL_ID,
                                        PAPER_TITLE,
                                        ISBN_NUMBER,
                                        PUBLISHED_IN_PROCEEDING,
                                        PRESENTING_AUTHOR,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?ACTIVITY_ID,
                                        ?PROGRAMME_TYPE_ID,
                                        ?ACTIVITY_DATE,
                                        ?ACTIVITY_DATE_TO,
                                        ?ACTIVITY_TITLE,
                                        ?ORGANIZER,
                                        ?PLACE,
                                        ?LEVEL_ID,
                                        ?PAPER_TITLE,
                                        ?ISBN_NUMBER,
                                        ?PUBLISHED_IN_PROCEEDING,
                                        ?PRESENTING_AUTHOR,
                                        CURDATE(),
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.DeletePublicationAndPrasentation:
                    {
                        sQuery = @"UPDATE IQAC_STAFF_ACTIVITY SET IS_DELETED=1 WHERE IQAC_STAFF_ACTIVITY_ID=?IQAC_STAFF_ACTIVITY_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdatePublicationAndPrasentationFilePath:
                    {
                        sQuery = @"UPDATE IQAC_STAFF_ACTIVITY SET FILE_PATH=?FILE_PATH WHERE IQAC_STAFF_ACTIVITY_ID=?IQAC_STAFF_ACTIVITY_ID;";
                        break;
                    }
                #endregion

                #region 10 B. Paper Publication
                case IQACEnumCommands.FetchPaperPublicationList:
                    {
                        sQuery = @"SELECT 
                                        IQAC_PAPER_PUBLICATION_ID,
                                        STAFF_ID,
                                        PAPER_TITLE,
                                        JOURNEL_NAME,
                                        PUBLICATION_YEAR,
                                        PUBLICATION_VOLUME,
                                        ISSUE_NUMBER,
                                        UGC_NUMBER,
                                        IF(OTHERS=1,'SCI','CCSI/SCIE') AS OTHERS,
                                        PAGE_NUMBER,
                                        ISSN_NUMBER,
                                        ISBN_NUMBER,
                                        IF(SCOPUS_ID = 1, 'YES', 'NO') AS SCOPUS_ID,
                                        IF(WEB_OF_SCIENCE = 1, 'YES', 'NO') AS WEB_OF_SCIENCE,
                                        IF(UGC_LISTED = 1, 'YES', 'NO') AS UGC_LISTED,
                                        SERIAL_NUMBER,
                                        IMPACT_FACT,
                                        CITATIONS,
                                        CITATION_INDEX,
                                        IF(CORRESPONDING_AUTHOR = 1,
                                            'YES',
                                            'NO') AS CORRESPONDING_AUTHOR,
                                        IF(FIRST_AUTHOR = 1, 'YES', 'NO') AS FIRST_AUTHOR
                                    FROM
                                        IQAC_PAPER_PUBLICATION
                                    WHERE
                                        STAFF_ID = ?STAFF_ID 
                                           -- AND ACADEMIC_YEAR IN (?AC_YEAR)
                                            AND IS_DELETED != 1;";
                        break;
                    }

                case IQACEnumCommands.FetchSupMonths:
                    {
                        sQuery = @"SELECT MONTH_ID,MONTH_NAME FROM SUP_MONTHS;";
                        break;
                    }

                case IQACEnumCommands.FetchPaperPublicationById:
                    {
                        sQuery = @"SELECT 
                                        IQAC_PAPER_PUBLICATION_ID,
                                        STAFF_ID,
                                        PAPER_TITLE,
                                        JOURNEL_NAME,
                                        PUBLICATION_YEAR,
                                        PUBLICATION_VOLUME,
                                        ISSUE_NUMBER,
                                        UGC_NUMBER,
                                        OTHERS,
                                        PAGE_NUMBER,
                                        ISSN_NUMBER,
                                        ISBN_NUMBER,
                                        SCOPUS_ID,
                                        WEB_OF_SCIENCE,
                                        UGC_LISTED,
                                        SERIAL_NUMBER,
                                        IMPACT_FACT,
                                        CITATIONS,
                                        CITATION_INDEX,
                                        CORRESPONDING_AUTHOR,
                                        FIRST_AUTHOR,
                                        FILE_PATH,
                                        MONTH_ID
                                    FROM
                                        IQAC_PAPER_PUBLICATION
                                    WHERE
                                        IQAC_PAPER_PUBLICATION_ID =  ?IQAC_PAPER_PUBLICATION_ID;";
                        break;
                    }
                case IQACEnumCommands.SavePaperPublication:
                    {
                        sQuery = @"INSERT INTO IQAC_PAPER_PUBLICATION
                                    (
	                                    STAFF_ID,
	                                    PAPER_TITLE,
	                                    JOURNEL_NAME,
	                                    PUBLICATION_YEAR,
	                                    PUBLICATION_VOLUME,
	                                    ISSUE_NUMBER,
                                        UGC_NUMBER,
                                        OTHERS,
	                                    PAGE_NUMBER,
	                                    ISSN_NUMBER,
	                                    ISBN_NUMBER,
	                                    SCOPUS_ID,
	                                    WEB_OF_SCIENCE,
	                                    UGC_LISTED,
	                                    SERIAL_NUMBER,
	                                    IMPACT_FACT,
	                                    CITATIONS,
                                        CITATION_INDEX,
	                                    CORRESPONDING_AUTHOR,
	                                    FIRST_AUTHOR,
	                                    ENTRY_DATE,
	                                    ACADEMIC_YEAR,
                                        MONTH_ID
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?PAPER_TITLE,
	                                    ?JOURNEL_NAME,
	                                    ?PUBLICATION_YEAR,
	                                    ?PUBLICATION_VOLUME,
	                                    ?ISSUE_NUMBER,
                                        ?UGC_NUMBER,
                                        ?OTHERS,
	                                    ?PAGE_NUMBER,
	                                    ?ISSN_NUMBER,
	                                    ?ISBN_NUMBER,
	                                    ?SCOPUS_ID,
	                                    ?WEB_OF_SCIENCE,
	                                    ?UGC_LISTED,
	                                    ?SERIAL_NUMBER,
	                                    ?IMPACT_FACT,
	                                    ?CITATIONS,
                                        ?CITATION_INDEX,
	                                    ?CORRESPONDING_AUTHOR,
	                                    ?FIRST_AUTHOR,
	                                    CURDATE(),
	                                    ?AC_YEAR,
                                        ?MONTH_ID
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdatePaperPublication:
                    {
                        sQuery = @"UPDATE IQAC_PAPER_PUBLICATION
                                    SET
	                                    STAFF_ID =?STAFF_ID,
	                                    PAPER_TITLE =?PAPER_TITLE,
	                                    JOURNEL_NAME =?JOURNEL_NAME,
	                                    PUBLICATION_YEAR =?PUBLICATION_YEAR,
	                                    PUBLICATION_VOLUME =?PUBLICATION_VOLUME,
	                                    ISSUE_NUMBER =?ISSUE_NUMBER,
                                        UGC_NUMBER=?UGC_NUMBER,
                                        OTHERS=?OTHERS,
	                                    PAGE_NUMBER =?PAGE_NUMBER,
	                                    ISSN_NUMBER =?ISSN_NUMBER,
	                                    ISBN_NUMBER =?ISBN_NUMBER,
	                                    SCOPUS_ID =?SCOPUS_ID,
	                                    WEB_OF_SCIENCE =?WEB_OF_SCIENCE,
	                                    UGC_LISTED =?UGC_LISTED,
	                                    SERIAL_NUMBER =?SERIAL_NUMBER,
	                                    IMPACT_FACT =?IMPACT_FACT,
	                                    CITATIONS =?CITATIONS,
                                        CITATION_INDEX=?CITATION_INDEX,
	                                    CORRESPONDING_AUTHOR =?CORRESPONDING_AUTHOR,
	                                    FIRST_AUTHOR =?FIRST_AUTHOR,
	                                    ENTRY_DATE =CURDATE(),
	                                    ACADEMIC_YEAR =?AC_YEAR,
                                        MONTH_ID=?MONTH_ID
                                    WHERE IQAC_PAPER_PUBLICATION_ID = ?IQAC_PAPER_PUBLICATION_ID;";
                        break;
                    }
                case IQACEnumCommands.DeletePaperPublication:
                    {
                        sQuery = @"UPDATE IQAC_PAPER_PUBLICATION SET IS_DELETED=1 WHERE IQAC_PAPER_PUBLICATION_ID = ?IQAC_PAPER_PUBLICATION_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdatePaperPublicationFilePath:
                    {
                        sQuery = @"UPDATE IQAC_PAPER_PUBLICATION SET FILE_PATH=?FILE_PATH WHERE IQAC_PAPER_PUBLICATION_ID = ?IQAC_PAPER_PUBLICATION_ID;";
                        break;
                    }
                #endregion

                #region 10 C. Book Publication
                case IQACEnumCommands.FetchBookPublicationList:
                    {
                        sQuery = @"SELECT 
                                        IQAC_BOOK_PUBLICATION_ID,
                                        STAFF_ID,
                                        CHAPTER,
                                        BOOK_TITLE,
                                        CHAPTER_TITLE,
                                        PUBLISHER,
                                        PUBLICATION_YEAR,
                                        ISBN_NUMBER,
                                        IF(SINGLE_AUTHOR=1,'YES','NO') AS SINGLE_AUTHOR,
                                        IF(EDITOR=1,'YES','NO') AS EDITOR
                                    FROM
                                        IQAC_BOOK_PUBLICATION
                                    WHERE
                                        STAFF_ID = ?STAFF_ID 
                                          --  AND ACADEMIC_YEAR IN (?AC_YEAR)
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchBookPublicationById:
                    {
                        sQuery = @"SELECT 
                                        IQAC_BOOK_PUBLICATION_ID,
                                        STAFF_ID,
                                        CHAPTER,
                                        BOOK_TITLE,
                                        CHAPTER_TITLE,
                                        PUBLISHER,
                                        PUBLICATION_YEAR,
                                        ISBN_NUMBER,
                                        SINGLE_AUTHOR,
                                        EDITOR,
                                        FILE_PATH
                                    FROM
                                        IQAC_BOOK_PUBLICATION
                                    WHERE
                                        IQAC_BOOK_PUBLICATION_ID = ?IQAC_BOOK_PUBLICATION_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveBookPublication:
                    {
                        sQuery = @"INSERT INTO IQAC_BOOK_PUBLICATION
                                    (
	                                    STAFF_ID,
	                                    CHAPTER,
	                                    BOOK_TITLE,
	                                    CHAPTER_TITLE,
	                                    PUBLISHER,
	                                    PUBLICATION_YEAR,
	                                    ISBN_NUMBER,
	                                    SINGLE_AUTHOR,
	                                    EDITOR,
	                                    ENTRY_DATE,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?CHAPTER,
	                                    ?BOOK_TITLE,
	                                    ?CHAPTER_TITLE,
	                                    ?PUBLISHER,
	                                    ?PUBLICATION_YEAR,
	                                    ?ISBN_NUMBER,
	                                    ?SINGLE_AUTHOR,
	                                    ?EDITOR,
	                                    CURDATE(),
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateBookPublication:
                    {
                        sQuery = @"UPDATE IQAC_BOOK_PUBLICATION
                                    SET
	                                    STAFF_ID =?STAFF_ID,
	                                    CHAPTER =?CHAPTER,
	                                    BOOK_TITLE =?BOOK_TITLE,
	                                    CHAPTER_TITLE =?CHAPTER_TITLE,
	                                    PUBLISHER =?PUBLISHER,
	                                    PUBLICATION_YEAR =?PUBLICATION_YEAR,
	                                    ISBN_NUMBER =?ISBN_NUMBER,
	                                    SINGLE_AUTHOR =?SINGLE_AUTHOR,
	                                    EDITOR =?EDITOR,
	                                    ENTRY_DATE =CURDATE(),
	                                    ACADEMIC_YEAR =?AC_YEAR
                                    WHERE IQAC_BOOK_PUBLICATION_ID = ?IQAC_BOOK_PUBLICATION_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteBookPublication:
                    {
                        sQuery = @"UPDATE IQAC_BOOK_PUBLICATION SET IS_DELETED=1 WHERE IQAC_BOOK_PUBLICATION_ID = ?IQAC_BOOK_PUBLICATION_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateBookPublicationFilePath:
                    {
                        sQuery = @"UPDATE IQAC_BOOK_PUBLICATION SET FILE_PATH=?FILE_PATH WHERE IQAC_BOOK_PUBLICATION_ID=?IQAC_BOOK_PUBLICATION_ID;";
                        break;
                    }
                #endregion

                #region 9.  Projects
                case IQACEnumCommands.FetchIQACProjects:
                    {
                        sQuery = @"SELECT 
                                        IQAC_PROJECT_ID,
                                        STF.STAFF_ID,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '',
                                                IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        SPT.PROJECT_TYPE ,
                                        PROJECT_TITLE,
                                        DURATION,
                                        PERIOD,
                                        PERIOD_TO,
                                         SIF.FUNDING_TYPE AS TYPE_OF_FUNDING,
                                        FUNDING_AGENCY,
                                        AMOUNT,
                                        DATE_FORMAT(ENTRY_DATE, '%d/%m/%Y') AS ENTRY_DATE
                                    FROM
                                        IQAC_PROJECT AS IP
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IP.STAFF_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        SUP_IQAC_PROJECT_TYPE AS SPT ON SPT.PROJECT_TYPE_ID=IP.PROJECT_TYPE
                                            AND SPT.IS_DELETED!=1
                                            INNER JOIN
                                        SUP_IQAC_TYPE_OF_FUNDING AS SIF ON SIF.TYPE_FUNDING_ID=IP.TYPE_OF_FUNDING
                                            AND SIF.IS_DELETED!=1
                                    WHERE 
                                        IP.IS_DELETED != 1
                                           -- AND IP.ACADEMIC_YEAR IN (?AC_YEAR)
                                             AND IP.STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchIQACProjectsById:
                    {
                        sQuery = @"SELECT 
                                         IQAC_PROJECT_ID, 
                                         STAFF_ID,
                                         PROJECT_TYPE,
                                         PROJECT_TITLE, 
                                         DURATION, PERIOD, PERIOD_TO,
                                         TYPE_OF_FUNDING, 
                                         FUNDING_AGENCY,
                                         AMOUNT,
                                         FILE_PATH
                                    FROM 
                                        IQAC_PROJECT
                                    WHERE 
                                        IQAC_PROJECT_ID=?IQAC_PROJECT_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveIQACProjects:
                    {
                        sQuery = @"INSERT INTO IQAC_PROJECT
                                   (
                                       STAFF_ID,
                                       PROJECT_TYPE,
                                       PROJECT_TITLE,
                                       DURATION,
                                       PERIOD,
                                       PERIOD_TO,
                                       TYPE_OF_FUNDING,
                                       FUNDING_AGENCY,
                                       AMOUNT,
                                       ENTRY_DATE,
                                       ACADEMIC_YEAR
                                   )
                                   VALUES
                                   (
                                        ?STAFF_ID,
                                        ?PROJECT_TYPE,
                                        ?PROJECT_TITLE,
                                        ?DURATION,
                                        ?PERIOD,
                                        ?PERIOD_TO,
                                        ?TYPE_OF_FUNDING,
                                        ?FUNDING_AGENCY,
                                        ?AMOUNT,
                                        now(),
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateIQACProjectsById:
                    {
                        sQuery = @"UPDATE IQAC_PROJECT
                                    SET 
                                        STAFF_ID=?STAFF_ID,
                                        PROJECT_TYPE=?PROJECT_TYPE,
                                        PROJECT_TITLE=?PROJECT_TITLE,
                                        DURATION=?DURATION,
                                        PERIOD=?PERIOD,
                                        PERIOD_TO=?PERIOD_TO,
                                        TYPE_OF_FUNDING=?TYPE_OF_FUNDING,
                                        FUNDING_AGENCY=?FUNDING_AGENCY,
                                        AMOUNT=?AMOUNT,
                                        ENTRY_DATE=NOW(),
                                        ACADEMIC_YEAR=?AC_YEAR
                                    WHERE 
                                        IQAC_PROJECT_ID=?IQAC_PROJECT_ID;";
                        break;
                    }
                case IQACEnumCommands.DeletedIQACProjectsById:
                    {
                        sQuery = @"UPDATE  IQAC_PROJECT SET IS_DELETED=?IS_DELETED WHERE IQAC_PROJECT_ID=?IQAC_PROJECT_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateIQACProjectsFilePathById:
                    {
                        sQuery = @"UPDATE IQAC_PROJECT SET FILE_PATH=?FILE_PATH WHERE IQAC_PROJECT_ID=?IQAC_PROJECT_ID;";
                        break;
                    }
                #endregion

                #region 9. Research Guidence
                case IQACEnumCommands.FetchIQACResearchGuidence:
                    {
                        sQuery = @"SELECT 
                                        IQAC_RESEARCH_GUDIENCE_ID,
                                        STF.STAFF_ID,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '',
                                                IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        SPT.CATEGORY_NAME AS CATEGORY,
                                        STUDENT_NAME,
                                        REGISTER_NUMBER,
                                        PROJECT_TITLE,
                                        REGISTRATION_YEAR,
                                        COMPLITION_YEAR,
                                        DATE_FORMAT(ENTRY_DATE, '%Y/%m/%d') AS ENTRY_DATE,
                                        FILE_PATH
                                    FROM
                                        IQAC_RESEARCH_GUDIENCE AS IP
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IP.STAFF_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        SUP_IQAC_RESEARCH_GUIDENCE_CATEGORY AS SPT ON SPT.IQAC_CATEGORY_ID=IP.CATEGORY_ID
                                            AND SPT.IS_DELETED!=1
                                    WHERE
                                        IP.IS_DELETED != 1
                                           -- AND IP.ACADEMIC_YEAR IN (?AC_YEAR)
                                            AND IP.STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchIQACResearchGuidenceById:
                    {
                        sQuery = @"SELECT 
                                         IQAC_RESEARCH_GUDIENCE_ID, 
                                         STAFF_ID,
                                         CATEGORY_ID,
                                         STUDENT_NAME, 
                                         REGISTER_NUMBER, 
                                         PROJECT_TITLE,
                                         REGISTRATION_YEAR, 
                                         COMPLITION_YEAR,
                                         FILE_PATH
                                    FROM 
                                         IQAC_RESEARCH_GUDIENCE
                                    WHERE 
                                        IQAC_RESEARCH_GUDIENCE_ID=?IQAC_RESEARCH_GUDIENCE_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveIQACResearchGuidence:
                    {
                        sQuery = @"INSERT INTO IQAC_RESEARCH_GUDIENCE
                                    (
                                        STAFF_ID,
                                        CATEGORY_ID,
                                        STUDENT_NAME,
                                        REGISTER_NUMBER,
                                        PROJECT_TITLE,
                                        REGISTRATION_YEAR,
                                        COMPLITION_YEAR,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?CATEGORY_ID,
                                        ?STUDENT_NAME,
                                        ?REGISTER_NUMBER,
                                        ?PROJECT_TITLE,
                                        ?REGISTRATION_YEAR,
                                        ?COMPLITION_YEAR,
                                        CURDATE(),
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateIQACResearchGuidenceById:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_GUDIENCE
                                    SET 
                                        STAFF_ID=?STAFF_ID,
                                        CATEGORY_ID=?CATEGORY_ID,
                                        STUDENT_NAME=?STUDENT_NAME,
                                        REGISTER_NUMBER=?REGISTER_NUMBER,
                                        PROJECT_TITLE=?PROJECT_TITLE,
                                        REGISTRATION_YEAR=?REGISTRATION_YEAR,
                                        COMPLITION_YEAR=?COMPLITION_YEAR,
                                        ENTRY_DATE=CURDATE(),
                                        ACADEMIC_YEAR=?AC_YEAR
                                    WHERE 
                                        IQAC_RESEARCH_GUDIENCE_ID=?IQAC_RESEARCH_GUDIENCE_ID;";
                        break;
                    }
                case IQACEnumCommands.DeletedIQACResearchGuidenceById:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_GUDIENCE SET IS_DELETED=?IS_DELETED WHERE IQAC_RESEARCH_GUDIENCE_ID=?IQAC_RESEARCH_GUDIENCE_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateIQACResearchGuidenceFilePathById:
                    {
                        sQuery = @"UPDATE IQAC_RESEARCH_GUDIENCE SET FILE_PATH=?FILE_PATH WHERE IQAC_RESEARCH_GUDIENCE_ID=?IQAC_RESEARCH_GUDIENCE_ID;";
                        break;
                    }
                #endregion

                #region PHD Guidence
                case IQACEnumCommands.FetchIQACPhdGuidence:
                    {
                        sQuery = @"SELECT 
                                        IQAC_PHD_GUDIENCE_ID,
                                        STF.STAFF_ID,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '',
                                                IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        SPT.CATEGORY_NAME,
                                        STUDENT_NAME,
                                        REGISTER_NUMBER,
                                        PROJECT_TITLE,
                                        REGISTRATION_YEAR,
                                        STATUS,
                                        DATE_FORMAT(GUDIENCE_DATE, '%d/%m/%Y') AS GUDIENCE_DATE,
                                        DATE_FORMAT(MINI_GUDIENCE_DATE, '%d/%m/%Y') AS MINI_GUDIENCE_DATE,
                                        DATE_FORMAT(MAXI_GUDIENCE_DATE, '%d/%m/%Y') AS MAXI_GUDIENCE_DATE,
                                        DATE_FORMAT(ENTRY_DATE, '%Y/%m/%d') AS ENTRY_DATE,
                                        FILE_PATH
                                   
                                    FROM
                                        IQAC_PHD_GUDIENCE AS IP
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IP.STAFF_ID
                                            AND STF.IS_DELETED != 1
                                            AND STF.IS_LEFT != 1
                                            INNER JOIN
                                        SUP_IQAC_PHD_GUIDENCE_CATEGORY AS SPT ON SPT.IQAC_CATEGORY_ID=IP.CATEGORY_ID
                                            AND SPT.IS_DELETED!=1
                                    WHERE
                                        IP.IS_DELETED != 1 
                                           -- AND IP.ACADEMIC_YEAR IN (?AC_YEAR)
                                            AND IP.STAFF_ID=?STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchIQACPhdGuidenceById:
                    {
                        sQuery = @"SELECT 
                                         IQAC_PHD_GUDIENCE_ID, 
                                         STAFF_ID,
                                         CATEGORY_ID,
                                         STUDENT_NAME, 
                                         REGISTER_NUMBER, 
                                         PROJECT_TITLE,
                                         REGISTRATION_YEAR, 
                                         STATUS,
                                         DATE_FORMAT(GUDIENCE_DATE, '%d/%m/%Y') AS GUDIENCE_DATE,
                                         DATE_FORMAT(MINI_GUDIENCE_DATE, '%d/%m/%Y') AS MINI_GUDIENCE_DATE,
                                         DATE_FORMAT(MAXI_GUDIENCE_DATE, '%d/%m/%Y') AS MAXI_GUDIENCE_DATE,
                                         FILE_PATH
                                    FROM 
                                        iqac_phd_gudience
                                    WHERE 
                                        IQAC_PHD_GUDIENCE_ID=?IQAC_PHD_GUDIENCE_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveIQACPhdGuidence:
                    {
                        sQuery = @"INSERT INTO IQAC_PHD_GUDIENCE
                                    (
                                        STAFF_ID,
                                        CATEGORY_ID,
                                        STUDENT_NAME,
                                        REGISTER_NUMBER,
                                        PROJECT_TITLE,
                                        REGISTRATION_YEAR,
                                        STATUS,
                                        GUDIENCE_DATE,
                                        MINI_GUDIENCE_DATE,
                                        MAXI_GUDIENCE_DATE,
                                        ENTRY_DATE,
                                        ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
                                        ?CATEGORY_ID,
                                        ?STUDENT_NAME,
                                        ?REGISTER_NUMBER,
                                        ?PROJECT_TITLE,
                                        ?REGISTRATION_YEAR,
                                        ?STATUS,
                                        ?GUDIENCE_DATE,
                                        ?MINI_GUDIENCE_DATE,
                                        ?MAXI_GUDIENCE_DATE,
                                        CURDATE(),
                                        ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateIQACPhdGuidenceById:
                    {
                        sQuery = @"UPDATE IQAC_PHD_GUDIENCE
                                   SET 
                                        STAFF_ID=?STAFF_ID,
                                        CATEGORY_ID=?CATEGORY_ID,
                                        STUDENT_NAME=?STUDENT_NAME,
                                        REGISTER_NUMBER=?REGISTER_NUMBER,
                                        PROJECT_TITLE=?PROJECT_TITLE,
                                        REGISTRATION_YEAR=?REGISTRATION_YEAR,
                                        STATUS=?STATUS,
                                        GUDIENCE_DATE=?GUDIENCE_DATE,
                                        MINI_GUDIENCE_DATE=?MINI_GUDIENCE_DATE,
                                        MAXI_GUDIENCE_DATE=?MAXI_GUDIENCE_DATE,
                                        ENTRY_DATE=NOW(),
                                        ACADEMIC_YEAR=?AC_YEAR
                                    WHERE 
                                        IQAC_PHD_GUDIENCE_ID=?IQAC_PHD_GUDIENCE_ID;";
                        break;
                    }
                case IQACEnumCommands.DeletedIQACPhdGuidenceById:
                    {
                        sQuery = @"UPDATE IQAC_PHD_GUDIENCE SET IS_DELETED=?IS_DELETED WHERE IQAC_PHD_GUDIENCE_ID=?IQAC_PHD_GUDIENCE_ID;";

                        break;
                    }
                case IQACEnumCommands.UpdateIQACPhdGuidenceFilePathById:
                    {
                        sQuery = @"UPDATE IQAC_PHD_GUDIENCE SET FILE_PATH=?FILE_PATH WHERE IQAC_PHD_GUDIENCE_ID=?IQAC_PHD_GUDIENCE_ID;";
                        break;
                    }
                #endregion

                #region Daily Activity (Department)
                case IQACEnumCommands.FetchDeptDailyActivityList:
                    {
                        sQuery = @"SELECT 
                                        ACTIVITY_ID,
                                        ACTIVITY,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        STAFF_ID,
                                        FILE_PATH,
                                        ACADEMIC_YEAR
                                    FROM
                                        DEPT_DAILY_ACTIVITY WHERE STAFF_ID=?STAFF_ID AND  IS_DELETED!=1;";
                        break;
                    }
                case IQACEnumCommands.FetchDeptDailyActivityById:
                    {
                        sQuery = @"SELECT 
                                        ACTIVITY_ID,
                                        ACTIVITY,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        STAFF_ID,
                                        FILE_PATH,
                                        ACADEMIC_YEAR
                                    FROM
                                        DEPT_DAILY_ACTIVITY 
                                    WHERE ACTIVITY_ID=?ACTIVITY_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveDeptDailyActivity:
                    {
                        sQuery = @"INSERT INTO DEPT_DAILY_ACTIVITY
                                    (
	                                    ACTIVITY,
	                                    ACTIVITY_DATE,
	                                    STAFF_ID,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
	                                    ?ACTIVITY,
	                                    ?ACTIVITY_DATE,
	                                    ?STAFF_ID,
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateDeptDailyActivity:
                    {
                        sQuery = @"UPDATE DEPT_DAILY_ACTIVITY
                                    SET
	                                    ACTIVITY = ?ACTIVITY,
	                                    ACTIVITY_DATE = ?ACTIVITY_DATE,
	                                    STAFF_ID = ?STAFF_ID,
	                                    FILE_PATH = ?FILE_PATH,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE ACTIVITY_ID = ?ACTIVITY_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteDeptDailyActivity:
                    {
                        sQuery = @"UPDATE DEPT_DAILY_ACTIVITY SET IS_DELETED=1  WHERE ACTIVITY_ID = ?ACTIVITY_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateFileDeptDailyActivity:
                    {
                        sQuery = @"UPDATE DEPT_DAILY_ACTIVITY SET FILE_PATH=?FILE_PATH  WHERE ACTIVITY_ID = ?ACTIVITY_ID;";
                        break;
                    }
                #endregion

                #region Seminor / Workshop Organized
                case IQACEnumCommands.FetchSeminorWorkshopOrganizedList:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        PT.PROGRAMME_TYPE AS PROGRAMME_TYPE_ID,
                                        IF_OTHER_SPECIFY,
                                        DATE_FORMAT(ORGANIZED_DATE, '%d/%m/%Y') AS ORGANIZED_DATE,
                                        DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                        TITLE,
                                        L.LEVEL AS LEVEL_ID,
                                        ACADEMIC_YEAR,
                                        FILE_PATH,
                                        ENTRY_DATE
                                    FROM
                                        IQAC_SEMINOR_WORKSOP_ORGANIZED AS S
                                            INNER JOIN
                                        SUP_IQAC_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID = S.PROGRAMME_TYPE_ID
                                            INNER JOIN
                                        SUP_LEVEL AS L ON L.LEVEL_ID = S.LEVEL_ID
                                    WHERE
                                        S.STAFF_ID = ?STAFF_ID
                                            AND S.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchSeminorWorkshopOrganizedById:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        PROGRAMME_TYPE_ID,IF_OTHER_SPECIFY,
                                        DATE_FORMAT(ORGANIZED_DATE,'%d/%m/%Y') AS ORGANIZED_DATE,
                                        DATE_FORMAT(TO_DATE, '%d/%m/%Y') AS TO_DATE,
                                        TITLE,
                                        LEVEL_ID,
                                        ACADEMIC_YEAR,
                                        FILE_PATH,
                                        ENTRY_DATE
                                    FROM
                                        IQAC_SEMINOR_WORKSOP_ORGANIZED 
                                    WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveSeminorWorkshopOrganized:
                    {
                        sQuery = @"INSERT INTO IQAC_SEMINOR_WORKSOP_ORGANIZED
                                    (
	                                    STAFF_ID,
	                                    PROGRAMME_TYPE_ID,
                                        IF_OTHER_SPECIFY,
	                                    ORGANIZED_DATE,
                                        TO_DATE,
	                                    TITLE,
	                                    LEVEL_ID,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?PROGRAMME_TYPE_ID,
                                        ?IF_OTHER_SPECIFY,
	                                    ?ORGANIZED_DATE,
                                        ?TO_DATE,
	                                    ?TITLE,
	                                    ?LEVEL_ID,
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateSeminorWorkshopOrganized:
                    {
                        sQuery = @"UPDATE IQAC_SEMINOR_WORKSOP_ORGANIZED
                                    SET
	                                    STAFF_ID = ?STAFF_ID,
	                                    PROGRAMME_TYPE_ID = ?PROGRAMME_TYPE_ID,
                                        IF_OTHER_SPECIFY=?IF_OTHER_SPECIFY,
	                                    ORGANIZED_DATE = ?ORGANIZED_DATE,
                                        TO_DATE=?TO_DATE,   
	                                    TITLE = ?TITLE,
	                                    LEVEL_ID = ?LEVEL_ID,
	                                    ACADEMIC_YEAR = ?AC_YEAR
                                    WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteSeminorWorkshopOrganized:
                    {
                        sQuery = @"UPDATE IQAC_SEMINOR_WORKSOP_ORGANIZED SET IS_DELETED=1 WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveOrganizer:
                    {
                        sQuery = @"INSERT INTO SUP_IQAC_ORGANIZER
                                    (
	                                    ORGANIZER_ID,
	                                    PRIMARY_ID,
	                                    ORGANIZER_NAME
                                    )
                                    VALUES
                                    (
                                        ?ORGANIZER_ID,
	                                    ?PRIMARY_ID,
	                                    ?ORGANIZER_NAME
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateOrganizer:
                    {
                        sQuery = @"UPDATE SUP_IQAC_ORGANIZER
                                    SET
	                                    ORGANIZER_ID = ?ORGANIZER_ID,
	                                    PRIMARY_ID =?PRIMARY_ID,
	                                    ORGANIZER_NAME = ?ORGANIZER_NAME
                                    WHERE SUP_ORGANIZER_ID = ?SUP_ORGANIZER_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateFileSeminorWorkshopOrganized:
                    {
                        sQuery = @"UPDATE IQAC_SEMINOR_WORKSOP_ORGANIZED SET FILE_PATH=?FILE_PATH WHERE IQAC_ID = ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchSupOrganizerById:
                    {
                        sQuery = @"SELECT 
                                        SUP_ORGANIZER_ID, 
                                        ORGANIZER_ID, 
                                        PRIMARY_ID, 
                                        ORGANIZER_NAME
                                    FROM
                                        SUP_IQAC_ORGANIZER
                                    WHERE
                                        PRIMARY_ID = ?PRIMARY_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchSupOrganizerListById:
                    {
                        sQuery = @"SELECT 
                                        O.ORGANIZER_NAME AS ORGANIZER, 
                                        I.ORGANIZER_NAME
                                    FROM
                                        SUP_IQAC_ORGANIZER AS I
                                            INNER JOIN
                                        SUP_ORGANIZER AS O ON O.ORGANIZER_ID = I.ORGANIZER_ID
                                    WHERE
                                        PRIMARY_ID = ?PRIMARY_ID;";
                        break;
                    }
                #endregion


                #region Association Activities
                case IQACEnumCommands.FetchAssociationActivitiesList:
                    {
                        sQuery = @"SELECT 
                                            IQAC_ID,
                                            STAFF_ID,
                                            SS.SHIFT_NAME AS SHIFT_ID,
                                            IA.ACTIVITY_NAME AS ACTIVITY_ID,
                                            IF_OTHERS_SPECIFY,
                                            DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                            DATE_FORMAT(TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                            DETAILS,
                                            FILE_PATH
                                        FROM
                                            IQAC_ASSOCIATION_ACTIVITIES AS AA
		                                        INNER JOIN
	                                        SUP_IQAC_ACTIVITY AS IA ON IA.ACTIVITY_ID=AA.ACTIVITY_ID
		                                        INNER JOIN
	                                        SUP_SHIFT AS SS ON SS.SHIFT_ID=AA.SHIFT_ID
                                        WHERE
                                            STAFF_ID = ?STAFF_ID
                                               -- AND ACADEMIC_YEAR IN (?AC_YEAR)
                                                AND AA.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchAssociationActivitiesById:
                    {
                        sQuery = @"SELECT 
                                        IQAC_ID,
                                        STAFF_ID,
                                        SHIFT_ID,
                                        ACTIVITY_ID,
                                        IF_OTHERS_SPECIFY,
                                        DATE_FORMAT(ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                        DATE_FORMAT(TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        DETAILS,
                                        FILE_PATH
                                    FROM
                                        IQAC_ASSOCIATION_ACTIVITIES
                                    WHERE
                                        IQAC_ID= ?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.SaveAssociationActivities:
                    {
                        sQuery = @"INSERT INTO IQAC_ASSOCIATION_ACTIVITIES
                                    (
	                                    STAFF_ID,
	                                    SHIFT_ID,
	                                    ACTIVITY_ID,
	                                    IF_OTHERS_SPECIFY,
	                                    ACTIVITY_DATE,
                                        TO_DATE,
	                                    DETAILS,
	                                    ACADEMIC_YEAR
                                    )
                                    VALUES
                                    (
                                        ?STAFF_ID,
	                                    ?SHIFT_ID,
	                                    ?ACTIVITY_ID,
	                                    ?IF_OTHERS_SPECIFY,
	                                    ?ACTIVITY_DATE,
                                        ?TO_DATE,
	                                    ?DETAILS,
	                                    ?AC_YEAR
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateAssociationActivities:
                    {
                        sQuery = @"UPDATE IQAC_ASSOCIATION_ACTIVITIES
                                    SET
	                                    STAFF_ID =?STAFF_ID,
	                                    SHIFT_ID =?SHIFT_ID,
	                                    ACTIVITY_ID =?ACTIVITY_ID,
	                                    IF_OTHERS_SPECIFY =?IF_OTHERS_SPECIFY,
	                                    ACTIVITY_DATE =?ACTIVITY_DATE,
                                        TO_DATE=?TO_DATE,
	                                    DETAILS =?DETAILS
                                    WHERE IQAC_ID =?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.DeleteAssociationActivities:
                    {
                        sQuery = @"UPDATE IQAC_ASSOCIATION_ACTIVITIES SET IS_DELETED=1 WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                case IQACEnumCommands.UpdateFileAssociationActivities:
                    {
                        sQuery = @"UPDATE IQAC_ASSOCIATION_ACTIVITIES SET FILE_PATH=?FILE_PATH WHERE IQAC_ID=?IQAC_ID;";
                        break;
                    }
                #endregion

                #region Resource Person
                case IQACEnumCommands.FetchResourcePersonById:
                    {
                        sQuery = @"SELECT 
                                        RESOURCE_PERSON_ID,
                                        QUESTION_ID,
                                        G.GUEST_NAME AS GUEST_ID,
                                        PRIMARY_KEY_ID,
                                        PERSON_NAME,
                                        DESIGNATION,
                                        INSTITUTION,
                                        PLACE,
                                        TOPIC
                                    FROM
                                        SUP_IQAC_RESOURCE_PERSON AS RP
                                            LEFT JOIN
                                        SUP_IQAC_GUEST AS G ON G.GUEST_ID=RP.GUEST_ID
                                    WHERE
                                        QUESTION_ID = ?QUESTION_ID 
                                            AND PRIMARY_KEY_ID = ?PRIMARY_KEY_ID
                                            AND rp.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.SaveResourcePerson:
                    {
                        sQuery = @"INSERT INTO SUP_IQAC_RESOURCE_PERSON
                                    (  
	                                    QUESTION_ID,
	                                    GUEST_ID,
	                                    PRIMARY_KEY_ID,
	                                    PERSON_NAME,
	                                    DESIGNATION,
	                                    INSTITUTION,
	                                    PLACE,
	                                    TOPIC
                                    )
                                    VALUES
                                    (
                                        ?QUESTION_ID,
	                                    ?GUEST_ID,
	                                    ?PRIMARY_KEY_ID,
	                                    ?PERSON_NAME,
	                                    ?DESIGNATION,
	                                    ?INSTITUTION,
	                                    ?PLACE,
	                                    ?TOPIC
                                    );";
                        break;
                    }
                case IQACEnumCommands.UpdateResourcePerson:
                    {
                        sQuery = @"UPDATE SUP_IQAC_RESOURCE_PERSON
                                    SET
	                                    QUESTION_ID = ?QUESTION_ID,
	                                    GUEST_ID = ?GUEST_ID,
	                                    PRIMARY_KEY_ID = ?PRIMARY_KEY_ID,
	                                    PERSON_NAME = ?PERSON_NAME,
	                                    DESIGNATION = ?DESIGNATION,
	                                    INSTITUTION = ?INSTITUTION,
	                                    PLACE = ?PLACE,
	                                    TOPIC = ?TOPIC
                                    WHERE RESOURCE_PERSON_ID = ?RESOURCE_PERSON_ID;";
                        break;
                    }
                #endregion

                #region IQAC Report For Status 
                case IQACEnumCommands.FetchWorkloadStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                            FIRST_NAME,
                                            CLASS_ID,
                                            CLASS_NAME,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            STATUS
                                        FROM
                                            (SELECT 
                                                SP.STAFF_ID,
                                                    SP.STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    CC.CLASS_ID,
                                                    CC.CLASS_NAME,
                                                    CCS.COURSE_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = SP.STAFF_ID
                                                AND CCS.IS_DELETED != 1
                                                AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                                AND CCS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                                AND CC.IS_DELETED != 1
                                            INNER JOIN CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                                AND CCR.IS_DELETED != 1
                                            LEFT JOIN IQAC_TEACHING_SUBJECTS_WORKLOAD AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.CLASS_ID = CCS.CLASS_ID
                                                AND I.COURSE_ID = CCS.COURSE_ID
                                                AND I.IS_DELETED != 1
                                                -- AND I.MONTH_ID = ?MONTH_ID
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID , CCS.CLASS_ID , CCS.COURSE_ID) AS T 
                                            WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID,T.COURSE_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchTeachingLearningStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                            FIRST_NAME,
                                            CLASS_ID,
                                            CLASS_NAME,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            STATUS
                                        FROM
                                            (SELECT 
                                                SP.STAFF_ID,
                                                    SP.STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    CC.CLASS_ID,
                                                    CC.CLASS_NAME,
                                                    CCS.COURSE_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = SP.STAFF_ID
                                                AND CCS.IS_DELETED != 1
                                                AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                                AND CCS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                                AND CC.IS_DELETED != 1
                                            INNER JOIN CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                                AND CCR.IS_DELETED != 1
                                            LEFT JOIN IQAC_PREPARATION_AND_UPDATION_OF_NOTES AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.CLASS_ID = CCS.CLASS_ID
                                                AND I.COURSE_ID = CCS.COURSE_ID
                                                AND I.IS_DELETED != 1
                                              --  AND I.MONTH_ID = ?MONTH_ID
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID , CCS.CLASS_ID , CCS.COURSE_ID) AS T 
                                            WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID,T.COURSE_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchCoCurriculerAspectsStatus:
                    {
                        sQuery = @"SELECT STAFF_ID,STAFF_CODE,FIRST_NAME,STATUS FROM (SELECT SP.STAFF_ID,
                                        STAFF_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        IF(I.STAFF_ID IS NULL,'NO','YES') AS STATUS
                                    FROM
                                        STF_PERSONAL_INFO AS SP
		                                    INNER JOIN
	                                    IQAC_CURRICULAR_COORDINATOR AS C ON C.STAFF_ID=SP.STAFF_ID
		                                    AND C.IS_DELETED!=1
                                            LEFT JOIN
                                        IQAC_CO_CURRICULAR_ASPECTS AS I ON I.STAFF_ID = SP.STAFF_ID
                                            AND I.IS_DELETED != 1
                                            -- AND I.MONTH_ID=?MONTH_ID
                                            AND I.ACADEMIC_YEAR=?ACADEMIC_YEAR
                                    WHERE
                                        SP.IS_DELETED != 1 
                                        AND SP.DEPARTMENT = (SELECT 
                                                                    DEPARTMENT
                                                                FROM
                                                                    stF_personal_info
                                                                WHERE
                                                                    STAFF_ID =?STAFF_ID)
                                    GROUP BY SP.STAFF_ID) AS T WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchCurriculerAspectsStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PERSONAL_ACADEMIC_ACTIVITY AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchLessonPlanStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                            FIRST_NAME,
                                            CLASS_ID,
                                            CLASS_NAME,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            STATUS
                                        FROM
                                            (SELECT 
                                                SP.STAFF_ID,
                                                    SP.STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    CC.CLASS_ID,
                                                    CC.CLASS_NAME,
                                                    CCS.COURSE_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = SP.STAFF_ID
                                                AND CCS.IS_DELETED != 1
                                                AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                                AND CCS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                                AND CC.IS_DELETED != 1
                                            INNER JOIN CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                                AND CCR.IS_DELETED != 1
                                            LEFT JOIN STF_LESSON_PLAN AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.COURSE_ID = CCS.COURSE_ID
                                                AND I.IS_DELETED != 1
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID , CCS.CLASS_ID , CCS.COURSE_ID) AS T 
                                            WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID,T.COURSE_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchExtentionWorkStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            LEFT JOIN IQAC_EXTENSION_WORKS AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.IS_DELETED != 1
                                               --  AND I.MONTH_ID = ?MONTH_ID
                                                AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }

                case IQACEnumCommands.FetchAwardsStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_AWARDS AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchSubjectAssociationStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_SUBJECT_ASSOCIATION AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchMembershipStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_MEMBERSHIP AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchHonourStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_HONOURS AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchProjectStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PROJECT AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             stF_personal_info
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchPHDGuidenceStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PHD_GUDIENCE AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             STF_PERSONAL_INFO
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchResearchGuidenceStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_RESEARCH_GUDIENCE AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             STF_PERSONAL_INFO
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchPublicationAndPrasentationStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_STAFF_ACTIVITY AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             STF_PERSONAL_INFO
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchPaperPublicationStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PAPER_PUBLICATION AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             STF_PERSONAL_INFO
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchBookPublicationStatus:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_BOOK_PUBLICATION AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?AC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.DEPARTMENT = (SELECT 
                                                                             DEPARTMENT
                                                                         FROM
                                                                             STF_PERSONAL_INFO
                                                                         WHERE
                                                                             STAFF_ID =?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                           T.STATUS IN('NO','YES');";
                        break;
                    }
                #endregion

                #region IQAC Status For Incharge 
                case IQACEnumCommands.FetchWorkloadStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                            FIRST_NAME,
                                            CLASS_ID,
                                            CLASS_NAME,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            STATUS
                                        FROM
                                            (SELECT 
                                                SP.STAFF_ID,
                                                    SP.STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    CC.CLASS_ID,
                                                    CC.CLASS_NAME,
                                                    CCS.COURSE_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = SP.STAFF_ID
                                                AND CCS.IS_DELETED != 1
                                                AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                                AND CC.IS_DELETED != 1
                                            INNER JOIN CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                                AND CCR.IS_DELETED != 1
                                            LEFT JOIN IQAC_TEACHING_SUBJECTS_WORKLOAD AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.CLASS_ID = CCS.CLASS_ID
                                                AND I.COURSE_ID = CCS.COURSE_ID
                                                AND I.IS_DELETED != 1
                                               -- AND I.MONTH_ID = ?MONTH_ID
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID , CCS.CLASS_ID , CCS.COURSE_ID) AS T 
                                            WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID,T.COURSE_ID;";
                        break;
                    }

                case IQACEnumCommands.FetchTeachingLearningStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                            FIRST_NAME,
                                            CLASS_ID,
                                            CLASS_NAME,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            STATUS
                                        FROM
                                            (SELECT 
                                                SP.STAFF_ID,
                                                    SP.STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    CC.CLASS_ID,
                                                    CC.CLASS_NAME,
                                                    CCS.COURSE_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = SP.STAFF_ID
                                                AND CCS.IS_DELETED != 1
                                                AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                                AND CC.IS_DELETED != 1
                                            INNER JOIN CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                                AND CCR.IS_DELETED != 1
                                            LEFT JOIN IQAC_PREPARATION_AND_UPDATION_OF_NOTES AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.CLASS_ID = CCS.CLASS_ID
                                                AND I.COURSE_ID = CCS.COURSE_ID
                                                AND I.IS_DELETED != 1
                                               -- AND I.MONTH_ID = ?MONTH_ID
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                   
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID , CCS.CLASS_ID , CCS.COURSE_ID) AS T 
                                            WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID,T.COURSE_ID;";
                        break;
                    }

                case IQACEnumCommands.FetchCoCurriculerAspectsStatus1:
                    {
                        sQuery = @"SELECT STAFF_ID,STAFF_CODE,FIRST_NAME,STATUS FROM (SELECT SP.STAFF_ID,
                                        STAFF_CODE,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        IF(I.STAFF_ID IS NULL,'NO','YES') AS STATUS
                                    FROM
                                        STF_PERSONAL_INFO AS SP
		                                    INNER JOIN
	                                    IQAC_CURRICULAR_COORDINATOR AS C ON C.STAFF_ID=SP.STAFF_ID
		                                    AND C.IS_DELETED!=1
                                            LEFT JOIN
                                        IQAC_CO_CURRICULAR_ASPECTS AS I ON I.STAFF_ID = SP.STAFF_ID
                                            AND I.IS_DELETED != 1
                                           -- AND I.MONTH_ID=?MONTH_ID
                                            AND I.ACADEMIC_YEAR=?ACADEMIC_YEAR
                                    WHERE
                                        SP.IS_DELETED != 1 
                                         AND SP.STAFF_ID IN (?STAFF_ID)
                                                   
                                    GROUP BY SP.STAFF_ID) AS T WHERE T.STATUS IN('NO','YES') ORDER BY T.STAFF_ID;";
                        break;
                    }

                case IQACEnumCommands.FetchCurriculerAspectsStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                            -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PERSONAL_ACADEMIC_ACTIVITY AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchLessonPlanStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                            FIRST_NAME,
                                            CLASS_ID,
                                            CLASS_NAME,
                                            COURSE_ID,
                                            COURSE_CODE,
                                            COURSE_TITLE,
                                            STATUS
                                        FROM
                                            (SELECT 
                                                SP.STAFF_ID,
                                                    SP.STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    CC.CLASS_ID,
                                                    CC.CLASS_NAME,
                                                    CCS.COURSE_ID,
                                                    CCR.COURSE_CODE,
                                                    CCR.COURSE_TITLE,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            INNER JOIN CP_CLASS_COURSE_STAFF AS CCS ON CCS.STAFF_ID = SP.STAFF_ID
                                                AND CCS.IS_DELETED != 1
                                                AND CCS.SEMESTER_TYPE_ID = ?SEMESTER_TYPE_ID
                                            INNER JOIN CP_CLASSES AS CC ON CC.CLASS_ID = CCS.CLASS_ID
                                                AND CC.IS_DELETED != 1
                                            INNER JOIN CP_COURSE_ROOT AS CCR ON CCR.COURSE_ROOT_ID = CCS.COURSE_ID
                                                AND CCR.IS_DELETED != 1
                                            LEFT JOIN STF_LESSON_PLAN AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.COURSE_ID = CCS.COURSE_ID
                                                AND I.IS_DELETED != 1
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                                    AND CCS.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            GROUP BY SP.STAFF_ID , CCS.CLASS_ID , CCS.COURSE_ID) AS T 
                                            WHERE T.STATUS IN( 'NO','yes') ORDER BY T.STAFF_ID,T.COURSE_ID;";

                        break;
                    }

                case IQACEnumCommands.FetchExtentionWorkStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                            -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                            LEFT JOIN IQAC_EXTENSION_WORKS AS I ON I.STAFF_ID = SP.STAFF_ID
                                                AND I.IS_DELETED != 1
                                               -- AND I.MONTH_ID = ?MONTH_ID
                                                AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','yes');";
                        break;
                    }

                case IQACEnumCommands.FetchAwardsStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                           -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_AWARDS AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchSubjectAssociationStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                            -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_SUBJECT_ASSOCIATION AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchMembershipStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE,
                                             FIRST_NAME, 
                                           -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_MEMBERSHIP AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchHonourStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                          --  CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_HONOURS AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES')";
                        break;
                    }
                case IQACEnumCommands.FetchProjectStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                            -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PROJECT AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchPHDGuidenceStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                           --  CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PHD_GUDIENCE AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchResearchGuidenceStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                           --  CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_RESEARCH_GUDIENCE AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchPublicationAndPrasentationStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                           --  CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_STAFF_ACTIVITY AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchPaperPublicationStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                            -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_PAPER_PUBLICATION AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                case IQACEnumCommands.FetchBookPublicationStatus1:
                    {
                        sQuery = @"SELECT 
                                            STAFF_ID,
                                            STAFF_CODE, 
                                            FIRST_NAME,
                                            -- CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME, 
                                            STATUS
                                        FROM
                                            (SELECT 
		                                        SP.STAFF_ID,
                                                STAFF_CODE,
                                                    CONCAT(IFNULL(SP.FIRST_NAME, ''), '  ', IFNULL(SP.LAST_NAME, '')) AS FIRST_NAME,
                                                    IF(I.STAFF_ID IS NULL, 'NO', 'YES') AS STATUS
                                            FROM
                                                STF_PERSONAL_INFO AS SP
                                                    LEFT JOIN 
                                                IQAC_BOOK_PUBLICATION AS I ON I.STAFF_ID = SP.STAFF_ID
                                                    AND I.IS_DELETED != 1
                                                    AND I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            WHERE
                                                SP.IS_DELETED != 1
                                                    AND SP.STAFF_ID IN (?STAFF_ID)
                                                    AND NON_TEACHING_CATEGORY != 1
                                            GROUP BY SP.STAFF_ID) AS T
                                        WHERE
                                            T.STATUS IN('NO','YES');";
                        break;
                    }
                #endregion

                #region Fetch Teching Staff
                case IQACEnumCommands.FetchTechingStaffOnly:
                    {
                        sQuery = @"SELECT 
                                      STAFF_ID, CONCAT(IFNULL(FIRST_NAME, ''), '  ', IFNULL(LAST_NAME, '')) AS FIRST_NAME
                                  FROM
                                      STF_PERSONAL_INFO
                                  WHERE
                                      NON_TEACHING_CATEGORY != 1
                                          AND IS_DELETED != 1;";
                        break;
                    }
                #endregion

                #region IQAC Date Wise Report
                case IQACEnumCommands.FetchDateWiseReportForCoCurricularAspects:
                    {
                        sQuery = @"SELECT 
                                    IPN.IQAC_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CD.DEPARTMENT AS DEPT_CODE,
                                    COMMITTEE_NAME AS NAME_OF_COMMITEE,
                                    DATE_FORMAT(IPN.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                    IPN.ACTIVITY_COMPLETED,
                                    IPN.ENTRY_DTATE,
                                    IPN.FILE_PATH
                                FROM
                                    IQAC_CO_CURRICULAR_ASPECTS AS IPN
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        INNER JOIN
                                    IQAC_CURRICULAR_COORDINATOR AS QA ON QA.IQAC_ID=IPN.NAME_OF_COMMITEE
                                        AND QA.IS_DELETED!=1
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                        AND CD.IS_DELETED != 1
                                WHERE
                                    IPN.IS_DELETED != 1 
                                    AND IPN.ACTIVITY_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                    AND IPN.STAFF_ID IN (?STAFF_ID) 
                                    -- AND IPN.ACADEMIC_YEAR=?AC_YEAR
                                ORDER BY IPN.ACTIVITY_DATE,STF.DEPARTMENT,STF.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForExtensionWorks:
                    {
                        sQuery = @"SELECT 
                                    IPN.IQAC_ID,
                                    CONCAT(IFNULL(STF.FIRST_NAME,''),'',IFNULL(STF.LAST_NAME,'')) AS FIRST_NAME,
                                    STF.STAFF_CODE,
                                    CD.DEPARTMENT AS DEPT_CODE,
                                    IPN.NAME_OF_THE_PROGRAMME,
                                    DATE_FORMAT(IPN.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                    IPN.ACTIVITY_COMPLETED,
                                    IPN.ENTRY_DTATE,
                                    IPN.NO_OF_PARTICIPANTS,
                                    IPN.PLACE,
                                    IPN.FILE_PATH
                                FROM
                                    IQAC_EXTENSION_WORKS AS IPN
                                        INNER JOIN
                                    STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = IPN.STAFF_ID
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STF.DEPARTMENT
                                        AND CD.IS_DELETED != 1
                                WHERE
                                     IPN.IS_DELETED != 1  AND IPN.STAFF_ID IN (?STAFF_ID) 
                                    -- AND IPN.ACADEMIC_YEAR=?AC_YEAR 
                                     AND IPN.ACTIVITY_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                ORDER BY IPN.ACTIVITY_DATE,STF.DEPARTMENT,STF.STAFF_ID;";
                        break;
                    }

                case IQACEnumCommands.FetchDateWiseReportforAwards:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        L.LEVEL,
                                        IA.AWARD_TITLE,
                                        IA.YEAR,
                                        IA.AWARDED_BY,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_AWARDS AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_LEVEL AS L ON L.LEVEL_ID = IA.LEVEL_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR =?ACADEMIC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
                                          --  AND IA.MONTH_ID = ?MONTH_ID
                                            AND IA.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportSubjectAssociation:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.ASSOCIATION_NAME,
                                        IA.POSISION_HELD,
                                        IA.PERIOD,
                                        IA.PERIOD_TO,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_SUBJECT_ASSOCIATION AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1   
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR =?AC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
                                           -- AND IA.MONTH_ID = ?MONTH_ID
                                            AND IA.IS_DELETED != 1;   ";
                        break;
                    }

                case IQACEnumCommands.FetchDateWiseReportForHonour:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.TITLE_OF_TALK,
                                        IA.PROGRAMME_TITLE,
                                        IA.INSTITION_NAME,
                                        IA.PLACE,
                                        DATE_FORMAT(IA.FROM_DATE,'%d/%m/%Y') AS FROM_DATE,
                                        DATE_FORMAT(IA.TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        IF(HONOR_TYPE_ID='1','Resource Person',IF(HONOR_TYPE_ID='2','Chair Person',IF(HONOR_TYPE_ID=3,'Convenor/Orgnanzing Secrerty',''))) AS HONOR_TYPE_ID ,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_HONOURS AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1 
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                         IA.IS_DELETED != 1  AND IA.STAFF_ID IN (?STAFF_ID)  
                                        -- AND IA.ACADEMIC_YEAR=?AC_YEAR
                                         AND IA.FROM_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                ORDER BY IA.FROM_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForMembership:
                    {
                        sQuery = @"SELECT 
	                                    SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        cd.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IA.BOARD_NAME,
                                        IA.COLLEGE_UNIVERSITY,
                                        IA.PERIOD,
                                        IA.PERIOD_TO,
                                        IF(MEMBERSHIP_TYPE_ID='1','Board Of Study Member',IF(MEMBERSHIP_TYPE_ID='2','Academic counsil Member',IF(MEMBERSHIP_TYPE_ID='3','Syndicate Member',''))) AS MEMBERSHIP_TYPE_ID,
                                        IA.FILE_PATH
                                    FROM
                                        IQAC_MEMBERSHIP AS IA
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = IA.STAFF_ID
                                            AND SP.IS_DELETED != 1 
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IA.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
                                          --  AND IA.MONTH_ID = ?MONTH_ID
									    AND IA.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForProject:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        PY.PROJECT_TYPE,
                                        TF.FUNDING_TYPE AS TYPE_OF_FUNDING, 
                                        IP.PROJECT_TITLE,
                                        IP.DURATION,
                                        IP.PERIOD,
                                        IP.PERIOD_TO,
                                        IP.FUNDING_AGENCY,
                                        IP.AMOUNT,
                                        IP.FILE_PATH
                                    FROM
                                        IQAC_PROJECT AS IP
                                            INNER JOIN
	                                    STF_PERSONAL_INFO AS SP ON SP.STAFF_ID=IP.STAFF_ID
		                                    AND SP.IS_DELETED!=1
		                                    INNER JOIN
                                        SUP_IQAC_PROJECT_TYPE AS PY ON PY.PROJECT_TYPE_ID = IP.PROJECT_TYPE
                                            INNER JOIN
                                        SUP_IQAC_TYPE_OF_FUNDING AS TF ON TF.TYPE_FUNDING_ID = IP.TYPE_OF_FUNDING 
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        IP.ACADEMIC_YEAR = ?AC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
										-- AND IP.MONTH_ID = ?MONTH_ID
										AND IP.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForResearchGuidence:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IR.CATEGORY_NAME AS CATEGORY,
                                        I.STUDENT_NAME,
                                        I.REGISTER_NUMBER,
                                        I.PROJECT_TITLE,
                                        I.REGISTRATION_YEAR,
                                        I.COMPLITION_YEAR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_RESEARCH_GUDIENCE AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_IQAC_RESEARCH_GUIDENCE_CATEGORY AS IR ON IR.IQAC_CATEGORY_ID = I.CATEGORY_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
										AND I.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForPHDGuidence:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IR.CATEGORY_NAME,
                                        I.STUDENT_NAME,
                                        I.REGISTER_NUMBER,
                                        I.PROJECT_TITLE,
                                        I.REGISTRATION_YEAR,
                                        DATE_FORMAT(I.GUDIENCE_DATE,'%d/%m/%Y') AS GUDIENCE_DATE,
                                        S.STATUS_NAME AS STATUS,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_PHD_GUDIENCE AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        SUP_IQAC_PHD_GUIDENCE_CATEGORY AS IR ON IR.IQAC_CATEGORY_ID = I.CATEGORY_ID
		                                    INNER JOIN
	                                    SUP_IQAC_STATUS AS S ON S.IQAC_STATUS_ID=I.STATUS
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.IS_DELETED != 1  AND I.STAFF_ID IN (?STAFF_ID)
                                       -- AND I.ACADEMIC_YEAR=?AC_YEAR  
                                       AND I.GUDIENCE_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                    ORDER BY I.GUDIENCE_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForPrasentedAndParticipated:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        PT.PROGRAMME_TYPE AS PROGRAMME_TYPE_ID,
                                        L.LEVEL AS LEVEL_ID,
                                        DATE_FORMAT(I.ACTIVITY_DATE,'%d/%m/%Y') AS ACTIVITY_DATE,
                                       -- DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        I.ACTIVITY_TITLE,
                                        I.ORGANIZER,
                                        I.PLACE,
                                        I.PAPER_TITLE,
                                        IF(I.PRESENTING_AUTHOR='1','YES',IF(I.PRESENTING_AUTHOR='2','NO','')) AS PRESENTING_AUTHOR,
                                        IF(I.ACTIVITY_ID='1','Presented',IF(I.ACTIVITY_ID='2','Participated','')) AS ACTIVITY_ID,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_STAFF_ACTIVITY AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
		                                    INNER JOIN
	                                    SUP_IQAC_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID=I.PROGRAMME_TYPE_ID
		                                    INNER JOIN
	                                    SUP_LEVEL AS L ON L.LEVEL_ID=I.LEVEL_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                         I.IS_DELETED != 1  AND I.STAFF_ID IN (?STAFF_ID)  
                                          --  AND I.ACADEMIC_YEAR=?AC_YEAR
                                          AND I.ACTIVITY_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                     ORDER BY I.ACTIVITY_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForPaperPublication:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        I.PAPER_TITLE,
                                        I.JOURNEL_NAME,
                                        I.PUBLICATION_YEAR,
                                        I.PUBLICATION_VOLUME,
                                        I.ISSUE_NUMBER,
                                        I.UGC_NUMBER,
                                        I.PAGE_NUMBER,
                                        I.ISSN_NUMBER,
                                        I.ISBN_NUMBER,
                                        IF(I.SCOPUS_ID='1','YES',IF(I.SCOPUS_ID='2','NO','')) AS SCOPUS_ID,
                                        IF(I.WEB_OF_SCIENCE='1','YES',IF(I.WEB_OF_SCIENCE='2','NO','')) AS WEB_OF_SCIENCE,
                                        IF(I.UGC_LISTED='1','YES',IF(I.UGC_LISTED='2','NO','')) AS UGC_LISTED,
                                        I.SERIAL_NUMBER,
                                        I.IMPACT_FACT,
                                        I.CITATION_INDEX,
                                        IF(I.CORRESPONDING_AUTHOR='1','YES',IF(I.CORRESPONDING_AUTHOR='2','NO','')) AS CORRESPONDING_AUTHOR,
                                        IF(I.FIRST_AUTHOR='1','YES',IF(I.FIRST_AUTHOR='2','NO','')) AS FIRST_AUTHOR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_PAPER_PUBLICATION AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
                                            AND I.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForBookPublication:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        IF(I.CHAPTER='1','Chapter',IF(I.CHAPTER='2','Book','')) AS CHAPTER,
                                        I.BOOK_TITLE,
                                        I.CHAPTER_TITLE,
                                        I.PUBLISHER,
                                        I.PUBLICATION_YEAR,
                                        I.ISBN_NUMBER,
                                        IF(I.SINGLE_AUTHOR='1','YES',IF(I.SINGLE_AUTHOR='2','NO','')) AS SINGLE_AUTHOR,
                                        IF(I.EDITOR='1','YES',IF(I.EDITOR='2','NO','')) AS EDITOR,
                                        I.FILE_PATH
                                    FROM
                                        IQAC_BOOK_PUBLICATION AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                        I.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        AND SP.STAFF_ID IN(?STAFF_ID)
                                            AND I.IS_DELETED != 1;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForDepartmentDailyActivity:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        SP.STAFF_CODE,
                                        CD.DEPARTMENT,
                                        I.ACTIVITY,
                                        DATE_FORMAT(I.ACTIVITY_DATE,'%d/%m/%Y') as ACTIVITY_DATE,
                                        I.FILE_PATH
                                    FROM
                                        DEPT_DAILY_ACTIVITY AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                       I.IS_DELETED != 1 AND I.STAFF_ID IN (?STAFF_ID)  
                                      --  AND I.ACADEMIC_YEAR=?AC_YEAR
                                     AND I.ACTIVITY_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                ORDER BY I.ACTIVITY_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForAssociationActivity:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        CD.DEPARTMENT,
                                        SP.STAFF_CODE,
                                        I.IF_OTHERS_SPECIFY,
                                        DATE_FORMAT(I.ACTIVITY_DATE,'%d/%m/%Y') as ACTIVITY_DATE,
                                        DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') as TO_DATE,
                                        I.DETAILS,
                                        I.FILE_PATH,
                                        SS.SHIFT_NAME,
                                        IA.ACTIVITY_NAME
                                    FROM
                                        IQAC_ASSOCIATION_ACTIVITIES AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
										SUP_SHIFT AS SS ON SS.SHIFT_ID=I.SHIFT_ID
											INNER JOIN
										SUP_IQAC_ACTIVITY AS IA ON IA.ACTIVITY_ID=I.ACTIVITY_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                         I.IS_DELETED != 1 AND I.STAFF_ID IN(?STAFF_ID)  
                                       -- AND I.ACADEMIC_YEAR=?AC_YEAR
                                        AND I.ACTIVITY_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                    ORDER BY I.ACTIVITY_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                case IQACEnumCommands.FetchDateWiseReportForSeminarWorkshopOrganized:
                    {
                        sQuery = @"SELECT 
                                        SP.STAFF_ID,
                                        CONCAT(IFNULL(SP.FIRST_NAME,''),'  ',IFNULL(SP.LAST_NAME,'')) AS FIRST_NAME,
                                        SP.STAFF_CODE,
                                        CD.DEPARTMENT,
                                        I.IF_OTHER_SPECIFY,
                                        DATE_FORMAT(I.ORGANIZED_DATE,'%d/%m/%Y') AS ORGANIZED_DATE,
                                        DATE_FORMAT(I.TO_DATE,'%d/%m/%Y') AS TO_DATE,
                                        I.FILE_PATH,
                                        PT.PROGRAMME_TYPE,
                                        L.LEVEL
                                    FROM
                                        IQAC_SEMINOR_WORKSOP_ORGANIZED AS I
                                            INNER JOIN
                                        STF_PERSONAL_INFO AS SP ON SP.STAFF_ID = I.STAFF_ID
                                            AND SP.IS_DELETED != 1
                                            INNER JOIN
										SUP_IQAC_PROGRAMME_TYPE AS PT ON PT.PROGRAMME_TYPE_ID=I.PROGRAMME_TYPE_ID
											INNER JOIN
										SUP_LEVEL AS L ON L.LEVEL_ID=I.LEVEL_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                            AND CD.IS_DELETED != 1
                                    WHERE
                                         I.IS_DELETED != 1 AND I.STAFF_ID IN (?STAFF_ID)  
                                          -- AND I.ACADEMIC_YEAR=?AC_YEAR
                                        AND I.ORGANIZED_DATE BETWEEN ?DATE_FROM AND ?DATE_TO 
                                    ORDER BY I.ORGANIZED_DATE,SP.DEPARTMENT,SP.STAFF_ID;";
                        break;
                    }
                #endregion
                #region lesson plan Report 
                case IQACEnumCommands.FetchLessonplanYearWiseReport:
                    {
                        sQuery = @"SELECT 
                                        slp.IQAC_ID,
                                        slp.COURSE_ID,
                                        spi.FIRST_NAME,
                                        slp.STAFF_ID,
                                        spi.STAFF_CODE,
                                        ccr.COURSE_TITLE,
                                        COURSE_CODE,
                                        slp.CYCLE,
                                        slp.UNIT,
                                        slp.TOPIC,
                                        cd.DEPARTMENT,
                                        CCR.SEMESTER_ID,
                                        slp.ACADEMIC_YEAR,
                                        cc.CLASS_ID,
                                        cc.CLASS_NAME
                                    FROM
                                        cp_class_course_staff AS ccs
                                            INNER JOIN
                                        stf_lesson_plan AS slp ON slp.STAFF_ID = ccs.STAFF_ID
                                            AND slp.COURSE_ID = ccs.COURSE_ID
                                            AND slp.ACADEMIC_YEAR = ccs.ACADEMIC_YEAR
                                            INNER JOIN
                                        cp_course_root AS ccr ON ccr.COURSE_ROOT_ID = slp.COURSE_ID
                                            INNER JOIN
                                        stf_personal_info AS spi ON spi.STAFF_ID = slp.staff_id
                                            INNER JOIN
                                        cp_classes AS cc ON cc.class_id = ccs.class_id
                                            INNER JOIN
                                        cp_department AS cd ON cd.DEPARTMENT_ID = cc.DEPARTMENT
                                    WHERE
                                        ccs.IS_DELETED != 1
                                            AND slp.IS_DELETED != 1
                                            AND slp.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND CCS.SEMESTER_TYPE_ID IN (?SEMESTER_TYPE_ID)
                                    ORDER BY ccs.CLASS_ID , ccs.STAFF_ID , ccs.COURSE_ID;";
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
