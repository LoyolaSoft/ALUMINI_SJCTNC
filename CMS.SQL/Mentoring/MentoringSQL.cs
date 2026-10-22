using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAO;

namespace CMS.SQL.Mentoring
{
    public class MentoringSQL
    {
        public static string GetMentorSQL(MentorSQLCommend EnumSQLCommend)
        {
            string query = string.Empty;
            switch (EnumSQLCommend)
            {
                case MentorSQLCommend.Fetchmentorvisiteddetailsforadmin:
                    {
                        query = @"SELECT 
                                        MV.MENTORS_VISIT_ID,
                                        MV.STUDENT_ID,
                                        MV.CLASS_ID,
                                        MV.MENTOR_ID,
                                        MV.NATURE_OF_VISIT,
                                        MV.INITIAL_CONTACT_VISIT,
                                        DATE_FORMAT(MV.VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                        MV.VISIT_TIME,
                                        MV.VISIT_DURATION,
                                        MV.MATTER_DISCUSSED,
                                        MV.ACTION_TAKEN,
                                        MV.GENERAL_IMPRESSION,
                                        DATE_FORMAT(MV.DATE_OF_NEXT_VISIT, '%d-%m-%Y') AS DATE_OF_NEXT_VISIT,  
                                        MV.ACADEMIC_YEAR,
                                        MV.CREATED_AT,
                                        MV.CONCLUSION_ARRIVED_AT,
                                        MV.IS_ACTIVE,
                                        MV.IS_DELETED,
                                        STU.FIRST_NAME,
                                        CL.CLASS_ID,
                                        CL.CLASS_CODE,
                                        STU.ROLL_NO,
                                        AC.ACADEMIC_YEAR_ID,
                                        AC.AC_YEAR
                                    FROM
                                        mentor_visit_details MV
                                        INNER JOIN stu_personal_info STU ON MV.STUDENT_ID = STU.STUDENT_ID
                                        INNER JOIN cp_classes CL ON MV.CLASS_ID = CL.CLASS_ID
                                        INNER JOIN academic_year AC ON MV.ACADEMIC_YEAR = AC.AC_YEAR
										WHERE 
                                        MV.IS_DELETED !=1;";
                        break;
                    }
                case MentorSQLCommend.FetchVisitRecoredByAcyear:
                    {
                        query = @"SELECT 
                                        MV.MENTORS_VISIT_ID,
                                        MV.STUDENT_ID,
                                        MV.CLASS_ID,
                                        MV.MENTOR_ID,
                                        MV.NATURE_OF_VISIT,
                                        MV.INITIAL_CONTACT_VISIT,
                                        DATE_FORMAT(MV.VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                        MV.VISIT_TIME,
                                        MV.VISIT_DURATION,
                                        MV.MATTER_DISCUSSED,
                                        MV.ACTION_TAKEN,
                                        MV.GENERAL_IMPRESSION,
                                        DATE_FORMAT(MV.DATE_OF_NEXT_VISIT, '%d-%m-%Y') AS DATE_OF_NEXT_VISIT,  
                                        MV.ACADEMIC_YEAR,
                                        MV.CREATED_AT,
                                        MV.CONCLUSION_ARRIVED_AT,
                                        MV.IS_ACTIVE,
                                        MV.IS_DELETED,
                                        STU.FIRST_NAME,
                                        CL.CLASS_ID,
                                        CL.CLASS_CODE,
                                        STU.ROLL_NO,
                                        AC.ACADEMIC_YEAR_ID,
                                        AC.AC_YEAR
                                    FROM
                                        mentor_visit_details MV
                                        INNER JOIN stu_personal_info STU ON MV.STUDENT_ID = STU.STUDENT_ID
                                        INNER JOIN cp_classes CL ON MV.CLASS_ID = CL.CLASS_ID
                                        INNER JOIN academic_year AC ON MV.ACADEMIC_YEAR = AC.AC_YEAR
										WHERE 
                                        MV.IS_DELETED !=1 AND
                                        MV.ACADEMIC_YEAR = 2024;";
                        break;
                    }

                case MentorSQLCommend.SaveMentoringVisitedStudentDet:
                    {
                        query = @"INSERT INTO mentor_visit_details(MENTORS_VISIT_ID,
										 STUDENT_ID,
										 CLASS_ID,
										 MENTOR_ID,
										 NATURE_OF_VISIT,
										 INITIAL_CONTACT_VISIT,
										 VISIT_DATE,
                                         VISIT_TIME,
										 VISIT_DURATION,
										 MATTER_DISCUSSED,
										 CONCLUSION_ARRIVED_AT,
										 ACTION_TAKEN,
										 GENERAL_IMPRESSION,
										 DATE_OF_NEXT_VISIT, 
										 ACADEMIC_YEAR,
										 CREATED_AT,
										 IS_ACTIVE,
										 IS_DELETED) VALUES (?MENTORS_VISIT_ID,
                                         ?STUDENT_ID,
                                         ?CLASS_ID,
                                         ?MENTOR_ID, 
                                         ?NATURE_OF_VISIT,
                                         ?INITIAL_CONTACT_VISIT,
                                         ?VISIT_DATE,
                                         ?VISIT_TIME,
                                         ?VISIT_DURATION,
                                         ?MATTER_DISCUSSED,
                                         ?CONCLUSION_ARRIVED_AT,
                                         ?ACTION_TAKEN,
                                         ?GENERAL_IMPRESSION,
                                         ?DATE_OF_NEXT_VISIT,
                                         ?ACADEMIC_YEAR,
                                         ?CREATED_AT,
                                         1, 
                                         0);";


                        break;
                    }
                case MentorSQLCommend.SaveRegularMentoringVisitedStudentDet:
                    {
                        query = @"INSERT INTO mentor_regular_visit_details 
                                                                (STUDENT_ID,
                                                                CLASS_ID,
                                                                 MENTOR_ID,
                                                                 REGULAR_VISIT,
                                                                 SHORT_REVIEW,
                                                                 VISIT_DATE,
                                                                 VISIT_TIME,
                                                                 VISIT_DURATION,
                                                                 MATTER_DISCUSSED,
                                                                 CONCLUSION_ARRIVED_AT,
                                                                 ACTION_TAKEN,
                                                                 GENERAL_IMPRESSION,
                                                                 DATE_OF_NEXT_VISIT,
                                                                 ACADEMIC_YEAR,
                                                                 CREATED_AT,
                                                                 IS_PENDING,
                                                                 IS_ACTIVE,
                                                                 IS_DELETED)
                                                                VALUES 
                                                                (?STUDENT_ID,
                                                                ?CLASS_ID,
                                                                ?MENTOR_ID,
                                                                ?REGULAR_VISIT,
                                                                ?SHORT_REVIEW,
                                                                ?VISIT_DATE,
                                                                ?VISIT_TIME,
                                                                ?VISIT_DURATION,
                                                                ?MATTER_DISCUSSED,
                                                                ?CONCLUSION_ARRIVED_AT,
                                                                ?ACTION_TAKEN,
                                                                ?GENERAL_IMPRESSION,
                                                                ?DATE_OF_NEXT_VISIT,
                                                                ?ACADEMIC_YEAR,
                                                                ?CREATED_AT,
                                                                ?IS_PENDING,
                                                                1,
                                                                0);";
                        break;
                    }



                case MentorSQLCommend.FetchmentorRegularvisiteddetails:
                    {
                        query = @"SELECT 
		                            STU.ROLL_NO,
		                            STU.FIRST_NAME,
                                    CL.CLASS_CODE,
		                            MR.MENTORS_REGULAR_VISIT_ID,
		                            MR.STUDENT_ID,
		                            MR.CLASS_ID, 
                                    MR.MENTOR_ID,
                                    MR.REGULAR_VISIT,
                                    MR.SHORT_REVIEW,
                                    DATE_FORMAT(MR.VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                    MR.VISIT_TIME,
                                    MR.VISIT_DURATION,
                                    MR.MATTER_DISCUSSED,
                                    MR.CONCLUSION_ARRIVED_AT,
                                    MR.ACTION_TAKEN,
                                    MR.GENERAL_IMPRESSION,
                                    DATE_FORMAT(MR.DATE_OF_NEXT_VISIT, '%d-%m-%Y') AS DATE_OF_NEXT_VISIT,
                                    MR.ACADEMIC_YEAR,
                                    MR.CREATED_AT,
                                    MR.IS_PENDING,
                                    MR.IS_ACTIVE,
                                    MR.IS_DELETED
                                    FROM 
                                    mentor_regular_visit_details MR
                                    INNER JOIN 
                                    stu_personal_info AS STU ON STU.STUDENT_ID = MR.STUDENT_ID
                                    INNER JOIN 
                                    cp_classes AS CL ON CL.CLASS_ID = MR.CLASS_ID
                                    WHERE 
                                    MR.IS_DELETED != 1;";
                        break;
                    }



                case MentorSQLCommend.FetchMentorGroupSessionRecords:
                    {
                        query = @"SELECT 
		                                STU.ROLL_NO,
		                                STU.FIRST_NAME,
                                        CL.CLASS_CODE,
		                                GD.MENTOR_GROUP_VISIT_ID,
                                        GD.STUDENT_ID,
                                        GD.CLASS_ID,
                                        GD.MENTOR_ID,
                                        DATE_FORMAT(GD.VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                        GD.VISIT_TIME,
                                        GD.VISIT_DURATION,
                                        GD.SHORT_REVIEW,
                                        GD.MATTER_DISCUSSED,
                                        GD.CONCULATION_ARRIVED_AT,
                                        GD.ACTION_TAKEN,
                                        GD.GENERAL_IMPRESSION,
                                        GD.SELF_ASSESSMENT,
                                        DATE_FORMAT(GD.DATE_OF_NEXT_VISIT,'%d-%m-%Y') AS DATE_OF_NEXT_VISIT,
                                        GD.ACADEMIC_YEAR,
                                        GD.CREATED_AT,
                                        GD.IS_ACTIVE,
                                        GD.IS_DELETED
                                        FROM 
                                        mentor_group_visit_details GD
                                        INNER JOIN 
                                        stu_personal_info AS STU ON STU.STUDENT_ID = GD.STUDENT_ID
                                        INNER JOIN 
                                        cp_classes AS CL ON CL.CLASS_ID = GD.CLASS_ID
                                        WHERE
                                        GD.IS_DELETED != 1;";
                        break;
                    }


                case MentorSQLCommend.SaveMentoringGroupSessionDet:
                    {
                        query = @"
                                              INSERT INTO mentor_group_visit_details(
                                     STUDENT_ID, CLASS_ID, MENTOR_ID, VISIT_DATE, VISIT_TIME,
                                     VISIT_DURATION, SHORT_REVIEW, MATTER_DISCUSSED, 
                                     CONCULATION_ARRIVED_AT, ACTION_TAKEN, GENERAL_IMPRESSION, 
                                     DATE_OF_NEXT_VISIT, ACADEMIC_YEAR,IS_ACTIVE,IS_DELETED
                                       ) VALUES (
                                     ?STUDENT_ID, ?CLASS_ID, ?MENTOR_ID, ?VISIT_DATE, ?VISIT_TIME, 
                                     ?VISIT_DURATION, ?SHORT_REVIEW, ?MATTER_DISCUSSED, 
                                     ?CONCULATION_ARRIVED_AT, ?ACTION_TAKEN, ?GENERAL_IMPRESSION, 
                                     ?DATE_OF_NEXT_VISIT, ?ACADEMIC_YEAR,1,0
                                      );";
                        break;
                    }


                case MentorSQLCommend.SaveMentoringFamilyDetails:
                    {
                        query = @"INSERT INTO MENTOR_FAMILY_DETAILS
                                              ( STUDENT_ID,
                                              FR_NAME,
                                              FR_EDUCATION,
                                              FR_OCCUPATION, 
                                              FR_INCOME, 
                                              MOTHER_NAME, 
                                              MOTHER_EDUCATION, 
                                              MOTHER_OCCUPATION, 
                                              MOTHER_INCOME, 
                                              GUARDIAN_NAME,
                                              GUARDIAN_EDUCATION, 
                                              GUARDIAN_OCCUPATION, 
                                              GUARDIAN_INCOME,
                                              ACADEMIC_YEAR,
                                              NO_OFSISTERS,
                                              NO_OFBROTHERS,
                                              FAMILY_TYPE,
                                              PROGRAM_ID,
                                              CLASS_ID,
                                              IS_DELETED,
                                              IS_ACTIVE)
                                              VALUES
                                              (
					                          ?STUDENT_ID,
                                              ?FR_NAME,
                                              ?FR_EDUCATION,
                                              ?FR_OCCUPATION, 
                                              ?FR_INCOME, 
                                              ?MOTHER_NAME, 
                                              ?MOTHER_EDUCATION, 
                                              ?MOTHER_OCCUPATION, 
                                              ?MOTHER_INCOME, 
                                              ?GUARDIAN_NAME,
                                              ?GUARDIAN_EDUCATION, 
                                              ?GUARDIAN_OCCUPATION, 
                                              ?GUARDIAN_INCOME,
                                              ?ACADEMIC_YEAR,
                                              ?NO_OFSISTERS,
                                              ?NO_OFBROTHERS,
                                              ?FAMILY_TYPE,
                                              ?PROGRAM_ID,
                                              ?CLASS_ID,
                                              0,
                                              1
					                          );";
                        break;
                    }


                case MentorSQLCommend.UpdateMentoringFamilyDetails:
                    {
                        query = @"UPDATE MENTOR_FAMILY_DETAILS SET 
                                FR_NAME=?FR_NAME,
                                 FR_EDUCATION=?FR_EDUCATION,
                                 FR_OCCUPATION=?FR_OCCUPATION,
                                 FR_INCOME=?FR_INCOME,
                                 MOTHER_NAME=?MOTHER_NAME,
                                 MOTHER_EDUCATION=?MOTHER_EDUCATION,
                                 MOTHER_OCCUPATION=?MOTHER_OCCUPATION,
                                 MOTHER_INCOME=?MOTHER_INCOME,
                                 GUARDIAN_NAME=?GUARDIAN_NAME,
                                 GUARDIAN_EDUCATION=?GUARDIAN_EDUCATION,
                                 GUARDIAN_OCCUPATION=?GUARDIAN_OCCUPATION,
                                 GUARDIAN_INCOME=?GUARDIAN_INCOME,
								 ACADEMIC_YEAR = ?ACADEMIC_YEAR,
								 IS_ACTIVE = 1,
								 IS_DELETED = 0,
								 CLASS_ID = ?CLASS_ID,
								 PROGRAM_ID = ?PROGRAM_ID,
								 FAMILY_TYPE = ?FAMILY_TYPE
                                 WHERE 
                                 STUDENT_ID=?STUDENT_ID;";
                        break;
                    }


                case MentorSQLCommend.FetchRegDetailsbyAcYear:
                    {
                        query = @"SELECT 
		                            STU.ROLL_NO,
		                            STU.FIRST_NAME,
                                    CL.CLASS_CODE,
		                            MR.MENTORS_REGULAR_VISIT_ID,
		                            MR.STUDENT_ID,
		                            MR.CLASS_ID, 
                                    MR.MENTOR_ID,
                                    MR.REGULAR_VISIT,
                                    MR.SHORT_REVIEW,
                                    DATE_FORMAT(MR.VISIT_DATE , '%d-%m-%Y') AS VISIT_DATE, 
                                    MR.VISIT_TIME,
                                    MR.VISIT_DURATION,
                                    MR.MATTER_DISCUSSED,
                                    MR.CONCLUSION_ARRIVED_AT,
                                    MR.ACTION_TAKEN,
                                    MR.GENERAL_IMPRESSION,
                                    DATE_FORMAT(MR.DATE_OF_NEXT_VISIT, '%d-%m-%Y') AS DATE_OF_NEXT_VISIT,
                                    MR.ACADEMIC_YEAR,
                                    MR.CREATED_AT,
                                    MR.IS_PENDING,
                                    MR.IS_ACTIVE,
                                    MR.IS_DELETED
                                    FROM 
                                    mentor_regular_visit_details MR
                                    INNER JOIN 
                                    stu_personal_info AS STU ON STU.STUDENT_ID = MR.STUDENT_ID
                                    INNER JOIN 
                                    cp_classes AS CL ON CL.CLASS_ID = MR.CLASS_ID
                                    WHERE 
                                    MR.ACADEMIC_YEAR = ?ACADEMIC_YEAR;";
                        break;
                    }

                case MentorSQLCommend.FetchStudentPersonaldet:
                    {
                        query = @"SELECT 
                                        STU.STUDENT_ID,
                                        STU.ACADEMIC_YEAR,
                                        STU.PROGRAM_ID,
                                        STU.CLASS_ID,
                                        STU.REGISTER_NO,
                                        STU.FIRST_NAME,
                                        STU.ROLL_NO,
                                        STU.DATE_OF_BIRTH,
                                        STU.GENDER,
                                        STU.DEPT_ID,
                                        CD.DEPARTMENT,
                                        STU.STU_MOBILENO,
                                        MSTU.STU_EMAILID,
                                        mstu.RELIGION,
                                        CL.CLASS_CODE,
                                        CL.CLASS_NAME,
                                        GN.GENDER_NAME,
                                        STU.BLOOD_GROUP,
                                        BL.BLOOD_GROUP_NAME,
                                        mstu.ADDRESS as ADDRESS_LINE1,
                                        mstu.SHIFT_ID,
                                        SS.SHIFT_NAME,
                                        mstu.FATHER_NAME,
                                        mstu.MOTHER_NAME,
                                        mstu.FR_OCCUPATION,
                                        mstu.MO_OCCUPATION,
                                        mstu.GUARDIAN_NAME,
                                        mstu.GUARDIAN_OCCUPATION,
                                        mstu.ANNUAL_INCOME,
                                        mstu.CASTE,
                                        mstu.COMMUNITY,
                                        mstu.SIBLINGS,
                                        MSTU.AADHAR_NUMBER,
                                        mstu.UMIS AS UMIS_NO,
                                        ABC_ID,
                                        mstu.TRAVELLING,
                                        MENTOR_STUDENT_ID,
                                        mstu.EMIS AS EMIS_NO,
                                        mstu.RELIGIONID AS RELIGIONID,
                                        mstu.FR_MOBILE
                                    FROM
                                        mentor_stu_personal_details AS mstu
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS STU ON stu.STUDENT_ID = mstu.STUDENT_ID
                                            INNER JOIN
                                        stu_class AS stc ON stc.STUDENT_ID = mstu.STUDENT_ID
                                            INNER JOIN
                                        CP_CLASSES AS CL ON CL.CLASS_ID = STC.CLASS_ID
                                            INNER JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CL.DEPARTMENT
                                            LEFT JOIN
                                        SUP_GENDER AS GN ON GN.GENDER_ID = mstu.GENDER
                                            LEFT JOIN
                                        SUP_BLOOD_GROUP AS BL ON BL.BLOOD_GROUP_ID = mstu.BLOOD_GROUP
                                            LEFT JOIN
                                        SUP_RELIGION AS REG ON REG.RELIGIONID = mstu.RELIGIONID
                                            LEFT JOIN
                                        SUP_SHIFT AS SS ON SS.SHIFT_ID = CL.SHIFT
                                    WHERE
                                        STU.STUDENT_ID = ?STUDENT_ID AND (MSTU.IS_DELETED != 1 OR MSTU.IS_DELETED IS NULL)
                                            AND stc.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }

                case MentorSQLCommend.FetchStudentFamilydetails:
                    {
                        query = @"SELECT 
		                                FAM.MENTORS_FAMILY_ID,
		                                FAM.STUDENT_ID,
		                                FAM.FR_NAME,
		                                FAM.FR_EDUCATION,
		                                FAM.FR_OCCUPATION,
		                                FAM.FR_INCOME,
		                                FAM.MOTHER_NAME,
		                                FAM.MOTHER_EDUCATION,
		                                FAM.MOTHER_OCCUPATION,
		                                FAM.MOTHER_INCOME,
		                                FAM.GUARDIAN_NAME,
		                                FAM.GUARDIAN_EDUCATION,
		                                FAM.GUARDIAN_OCCUPATION,
		                                FAM.GUARDIAN_INCOME,
		                                FAM.ACADEMIC_YEAR,
		                                FAM.CREATED_AT,
		                                FAM.IS_ACTIVE, 
		                                FAM.IS_DELETED,
		                                FAM.CLASS_ID,
		                                FAM.PROGRAM_ID, 
		                                FAM.FAMILY_TYPE,
		                                FAM.NO_OFSISTERS,
		                                FAM.NO_OFBROTHERS
		                                FROM 
                                        MENTOR_FAMILY_DETAILS FAM
                                        WHERE 
                                        FAM.ACADEMIC_YEAR = ?ACADEMIC_YEAR AND
                                        FAM.STUDENT_ID = ?STUDENT_ID AND
                                        FAM.CLASS_ID = ?CLASS_ID AND
		                                FAM.PROGRAM_ID = ?PROGRAM_ID;";
                        break;
                    }

                case MentorSQLCommend.CheckStudentFamilydetailsbyStudentId:
                    {
                        query = @"SELECT 
	                                    STUDENT_ID,
                                        FIRST_NAME,
                                        ACADEMIC_YEAR,
                                        FATHER_NAME,
                                        FR_OCCUPATION,
                                        FATHER_EDUCATION,
                                        MOTHER_NAME,
                                        MO_OCCUPATION,
                                        MOTHER_EDUCATION,
                                        ANNUAL_INCOME,
                                        FR_MOBILE,
                                        PROGRAM_ID,
                                        CLASS_ID
                                    FROM
                                        stu_personal_info
                                        WHERE
                                        ACADEMIC_YEAR = ?ACADEMIC_YEAR AND
                                        STUDENT_ID = ?STUDENT_ID AND
                                        CLASS_ID = ?CLASS_ID AND
                                        PROGRAM_ID = ?PROGRAM_ID;";
                        break;
                    }


                case MentorSQLCommend.FetchVisistStudetForEdit:
                    {
                        query = @"SELECT 
                                            MV.STUDENT_ID,
                                            MV.MENTORS_VISIT_ID,
                                            MV.CLASS_ID,
                                            MV.MENTOR_ID,
                                            MV.NATURE_OF_VISIT,
                                            MV.INITIAL_CONTACT_VISIT,
                                            date_format( MV.VISIT_DATE,'%d-%m-%Y') as VISIT_DATE,
                                            MV.VISIT_TIME,
                                            MV.VISIT_DURATION,
                                            MV.MATTER_DISCUSSED,
                                            MV.ACTION_TAKEN,
                                            MV.GENERAL_IMPRESSION,
                                            date_format( MV.DATE_OF_NEXT_VISIT,'%d-%m-%Y') as DATE_OF_NEXT_VISIT,
                                            MV.ACADEMIC_YEAR,
                                            MV.CREATED_AT,
                                            MV.CONCLUSION_ARRIVED_AT,
                                            MV.IS_ACTIVE,
                                            MV.IS_DELETED,
                                            STU.ROLL_NO,
                                            STU.FIRST_NAME,
                                            CL.CLASS_CODE,
                                            AC.ACADEMIC_YEAR_ID,
                                            AC.AC_YEAR
                                        FROM
                                            mentor_visit_details MV
                                                INNER JOIN
                                            stu_personal_info AS STU ON STU.STUDENT_ID = MV.STUDENT_ID
                                                INNER JOIN
                                            cp_classes AS CL ON CL.CLASS_ID = MV.CLASS_ID
                                                INNER JOIN
                                            ACADEMIC_YEAR AS AC ON AC.AC_YEAR = MV.ACADEMIC_YEAR
                                        WHERE
                                            MV.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }


                case MentorSQLCommend.UpdateMentorVisitDetailsByStudentID:
                    {
                        query = @"UPDATE mentor_visit_details
                                                SET 
                                                    CLASS_ID = ?CLASS_ID,
                                                    NATURE_OF_VISIT = ?NATURE_OF_VISIT,
                                                    INITIAL_CONTACT_VISIT = ?INITIAL_CONTACT_VISIT,
                                                    VISIT_DATE = ?VISIT_DATE,
                                                    VISIT_TIME = ?VISIT_TIME,
                                                    VISIT_DURATION = ?VISIT_DURATION,
                                                    MATTER_DISCUSSED = ?MATTER_DISCUSSED,
                                                    CONCLUSION_ARRIVED_AT = ?CONCLUSION_ARRIVED_AT,
                                                    ACTION_TAKEN = ?ACTION_TAKEN,
                                                    GENERAL_IMPRESSION = ?GENERAL_IMPRESSION,
                                                    DATE_OF_NEXT_VISIT = ?DATE_OF_NEXT_VISIT
                                                WHERE STUDENT_ID = ?STUDENT_ID;
                                                ";
                        break;
                    }
                case MentorSQLCommend.FetchStudentpersonalinfo:
                    {
                        query = @"SELECT 
                                    STU.STUDENT_ID,
                                    STU.ACADEMIC_YEAR,
                                    STU.PROGRAM_ID,
                                    STU.CLASS_ID,
                                    STU.REGISTER_NO,
                                    STU.FIRST_NAME,
                                    STU.ROLL_NO,
                                    STU.DATE_OF_BIRTH,
                                    STU.GENDER,
                                    STU.DEPT_ID,
                                    CD.DEPARTMENT,
                                    STU.STU_MOBILENO,
                                    STU.STU_EMAILID,
                                    STU.RELIGION,
                                    CL.CLASS_CODE,
                                    CL.CLASS_NAME,
                                    GN.GENDER_NAME,
                                    STU.BLOOD_GROUP,
                                    BL.BLOOD_GROUP_NAME,
                                    REG.RELIGION,
                                    STU.ADDRESS_LINE1,
                                    SS.SHIFT_NAME,
                                    STU.FR_MOBILE
                                FROM
                                    STU_PERSONAL_INFO AS STU
                                        INNER JOIN
                                    STU_CLASS AS SC ON SC.STUDENT_ID = STU.STUDENT_ID
                                        INNER JOIN
                                    CP_CLASSES AS CL ON CL.CLASS_ID = SC.CLASS_ID
                                        INNER JOIN
                                    CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = CL.DEPARTMENT
                                        LEFT JOIN
                                    SUP_GENDER AS GN ON GN.GENDER_ID = STU.GENDER
                                        LEFT JOIN
                                    SUP_BLOOD_GROUP AS BL ON BL.BLOOD_GROUP_ID = STU.BLOOD_GROUP
                                        LEFT JOIN
                                    SUP_RELIGION AS REG ON REG.RELIGIONID = STU.RELIGION
                                        LEFT JOIN
                                    SUP_SHIFT AS SS ON SS.SHIFT_ID = CL.SHIFT
                                WHERE
                                    STU.STUDENT_ID = ?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case MentorSQLCommend.InsertStudentDetails:
                    {
                        query = @"INSERT INTO mentor_stu_personal_details 
                                    (STUDENT_ID, FIRST_NAME, DEPT_ID, GENDER, DATE_OF_BIRTH, BLOOD_GROUP, RELIGIONID, REGISTER_NO, 
                                     ROLL_NO, CLASS_ID, PROGRAM_ID, STU_EMAILID, STU_MOBILENO, ADDRESS)
                                    VALUES (?STUDENT_ID, ?FIRST_NAME, ?DEPT_ID, ?GENDER, ?DATE_OF_BIRTH, ?BLOOD_GROUP, ?RELIGIONID, 
                                            ?REGISTER_NO, ?ROLL_NO, ?CLASS_ID, ?PROGRAM_ID, ?STU_EMAILID, ?STU_MOBILENO, ?ADDRESS);
                                    ";
                        break;
                    }
                case MentorSQLCommend.InsertStudentPersonal:
                    {
                        query = @"INSERT INTO mentor_stu_personal_details
                                                                (STUDENT_ID,
                                                                 FIRST_NAME,
                                                                 ROLL_NO,
                                                                 REGISTER_NO,
                                                                 SHIFT_ID,
                                                                 DEPT_ID,
                                                                 CLASS_ID,
                                                                 PROGRAM_ID,
                                                                 DATE_OF_BIRTH,
                                                                 GENDER,
                                                                 FATHER_NAME,
                                                                 FR_OCCUPATION,
                                                                 MOTHER_NAME,
                                                                 MO_OCCUPATION,
                                                                 GUARDIAN_NAME,
                                                                 GUARDIAN_OCCUPATION,
                                                                 ANNUAL_INCOME,
                                                                 CASTE,
                                                                 COMMUNITY,
                                                                 RELIGION,
                                                                 SIBLINGS,
                                                                 BLOOD_GROUP,
                                                                 RELIGIONID,
                                                                 STU_MOBILENO,
                                                                 STU_EMAILID,
                                                                 ADDRESS,
                                                                 AADHAR_NUMBER,
                                                                 UMIS,
                                                                 EMIS,
                                                                 TRAVELLING,
                                                                 FR_MOBILE) values(
                                                                 ?STUDENT_ID,
                                                                 ?FIRST_NAME,
                                                                 ?ROLL_NO,
                                                                 ?REGISTER_NO,
                                                                 ?SHIFT_ID,
                                                                 ?DEPT_ID,
                                                                 ?CLASS_ID,
                                                                 ?PROGRAM_ID,
                                                                 ?DATE_OF_BIRTH,
                                                                 ?GENDER,
                                                                 ?FATHER_NAME,
                                                                 ?FR_OCCUPATION,
                                                                 ?MOTHER_NAME,
                                                                 ?MO_OCCUPATION,
                                                                 ?GUARDIAN_NAME,
                                                                 ?GUARDIAN_OCCUPATION,
                                                                 ?ANNUAL_INCOME,
                                                                 ?CASTE,
                                                                 ?COMMUNITY,
                                                                 ?RELIGION,
                                                                 ?SIBLINGS,
                                                                 ?BLOOD_GROUP,
                                                                 ?RELIGIONID,
                                                                 ?STU_MOBILENO,
                                                                 ?STU_EMAILID,
                                                                 ?ADDRESS,
                                                                 ?AADHAR_NUMBER,
                                                                 ?UMIS,
                                                                 ?EMIS,
                                                                 ?TRAVELLING,
                                                                 ?FR_MOBILE);;
                                    ";
                        break;
                    }
                case MentorSQLCommend.CheckStudentPersonalExists:
                    {
                        query = @"SELECT COUNT(1)
                      FROM mentor_stu_personal_details
                      WHERE STUDENT_ID = ?STUDENT_ID;
                                    ";
                        break;
                    }
                case MentorSQLCommend.UpdateStudentPersonal:
                    {
                        query = @"UPDATE mentor_stu_personal_details
                      SET   
                              FIRST_NAME  = ?FIRST_NAME,
                              ROLL_NO = ?ROLL_NO,
                              REGISTER_NO = ?REGISTER_NO,
                              SHIFT_ID = ?SHIFT_ID,
                              DEPT_ID = ?DEPT_ID,
                              CLASS_ID = ?CLASS_ID,
                              PROGRAM_ID  = ?PROGRAM_ID,
                              DATE_OF_BIRTH = ?DATE_OF_BIRTH,
                              GENDER = ?GENDER,
                              FATHER_NAME = ?FATHER_NAME,
                              FR_OCCUPATION = ?FR_OCCUPATION,
                              MOTHER_NAME = ?MOTHER_NAME,
                              MO_OCCUPATION = ?MO_OCCUPATION,
                              GUARDIAN_NAME = ?GUARDIAN_NAME,
                              GUARDIAN_OCCUPATION = ?GUARDIAN_OCCUPATION,
                              ANNUAL_INCOME = ?ANNUAL_INCOME,
                              CASTE = ?CASTE,
                              COMMUNITY = ?COMMUNITY,
                              RELIGION = ?RELIGION,
                              SIBLINGS = ?SIBLINGS,
                              BLOOD_GROUP = ?BLOOD_GROUP,
                              RELIGIONID  = ?RELIGIONID,
                              STU_MOBILENO = ?STU_MOBILENO,
                              STU_EMAILID = ?STU_EMAILID,
                              ADDRESS = ?ADDRESS,
                              AADHAR_NUMBER = ?AADHAR_NUMBER,
                              UMIS = ?UMIS,
                              EMIS = ?EMIS,
                              TRAVELLING = ?TRAVELLING,
                              FR_MOBILE = ?FR_MOBILE
                      WHERE MENTOR_STUDENT_ID = ?MENTOR_STUDENT_ID;
                                    ";
                        break;
                    }
                case MentorSQLCommend.FetchStudentGuidance:
                    {
                        query = @"SELECT 	
		                            STUDENT_GUIDANCE_ID,
                                    INTERVENTION_AND_GUIDANCE,
                                    VISIT_DATE,
                                    IS_DELETED,
                                    IS_ACTIVE,
                                    STUDENT_ID,
                                    ACADEMIC_YEAR,
                                    PROGRAM_ID,
                                    CLASS_ID
                                    FROM 
                                    mentoring_Intervention_Guidance_Details
                                    WHERE 
		                            ACADEMIC_YEAR = ?ACADEMIC_YEAR AND
                                    PROGRAM_ID = ?PROGRAM_ID AND
                                    CLASS_ID = ?CLASS_ID AND
                                    STUDENT_ID = ?STUDENT_ID
                                     AND IS_DELETED!=1
                                     ORDER BY VISIT_DATE ASC;";
                        break;
                    }
                case MentorSQLCommend.FetchInterventionById:
                    query = @"SELECT 
                                STUDENT_GUIDANCE_ID,
                                INTERVENTION_AND_GUIDANCE,
                                DATE_FORMAT(VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                STUDENT_ID,
                                ACADEMIC_YEAR,
                                PROGRAM_ID,
                                CLASS_ID
                              FROM mentoring_Intervention_Guidance_Details 
                              WHERE STUDENT_GUIDANCE_ID = ?STUDENT_GUIDANCE_ID;";
                    break;
                case MentorSQLCommend.InsertGuidenceDetais:
                    {
                        query = @"INSERT 
                                  INTO mentoring_Intervention_Guidance_Details(
			                                INTERVENTION_AND_GUIDANCE,
                                      STUDENT_ID,ACADEMIC_YEAR,PROGRAM_ID,CLASS_ID,
                                            VISIT_DATE, IS_ACTIVE, IS_DELETED) VALUES (?INTERVENTION_AND_GUIDANCE,?STUDENT_ID,?ACADEMIC_YEAR,?PROGRAM_ID,?CLASS_ID,?VISIT_DATE, 1, 0);";
                        break;
                    }
                case MentorSQLCommend.FetchValuesandHabits:
                    {
                             query = @" SELECT 
                                    STUDENT_ACTIVITY_ID,
                                    GOAL_IN_LIFE,
                                    STRENGTH,
                                    WEAKNESS,
                                    ROLE_MODEL,
                                    STUDENT_ID
                                FROM mentoring_activity_response
                                WHERE STUDENT_ID = ?STUDENT_ID
                                AND IS_DELETED = 0;";

                                  
                        break;
                    }
                case MentorSQLCommend.FetchParticipation:
                    {
                        query = @"SELECT 
                                        PARTICIPATION_ID, 
                                        PARTICIPATION, 
                                        STUDENT_ID,
                                        INSTITUTION,
                                        DATE_FORMAT(DATE, '%d/%m/%Y') AS DATE, 
                                        PLACE, 
                                        EVENT,
                                        LEVEL, 
                                        PRIZE_ACHIEVEMENT,
                                        IS_ACTIVE, 
                                        IS_DELETED
                                        FROM
                                        MENTOR_PARTICIPATION_ACHIEVEMENTS
                                        WHERE 
		                                STUDENT_ID = ?STUDENT_ID
                                        AND IFNULL(IS_DELETED, 0) = 0;";
                        break;
                    }

                case MentorSQLCommend.FetchParticipationById:
                    {
                        query = @"SELECT 
                                        PARTICIPATION_ID, 
                                        PARTICIPATION, 
                                        STUDENT_ID,
                                        INSTITUTION,
                                        DATE_FORMAT(DATE, '%Y-%m-%d') AS DATE, 
                                        PLACE, 
                                        EVENT,
                                        LEVEL, 
                                        PRIZE_ACHIEVEMENT,
                                        IS_ACTIVE, 
                                        IS_DELETED
                                        FROM
                                        MENTOR_PARTICIPATION_ACHIEVEMENTS
                                        WHERE 
                                        PARTICIPATION_ID = ?PARTICIPATION_ID;";
                        break;
                    }

                case MentorSQLCommend.DeleteParticipation:
                    {
                        query = @"UPDATE MENTOR_PARTICIPATION_ACHIEVEMENTS
                                    SET IS_DELETED = 1
                                    WHERE PARTICIPATION_ID = ?PARTICIPATION_ID;";
                        break;
                    }

                case MentorSQLCommend.InsertValueDetails:
                    {
                        query = @"INSERT INTO mentoring_activity_response
                                        (
                                            GOAL_IN_LIFE,
                                            STRENGTH,
                                            WEAKNESS,
                                            ROLE_MODEL,
                                            STUDENT_ID,
                                            IS_ACTIVE,
                                            IS_DELETED
                                        )
                                        VALUES
                                        (
                                            ?GOAL_IN_LIFE,
                                            ?STRENGTH,
                                            ?WEAKNESS,
                                            ?ROLE_MODEL,
                                            ?STUDENT_ID,
                                            1,
                                            0);";
                                        

                                               
                                            
                        break;
                    }
                case MentorSQLCommend.UpdateValueDetails:
                    {
                        query = @"UPDATE mentoring_activity_response
                                        SET
                                            GOAL_IN_LIFE = ?GOAL_IN_LIFE,
                                            STRENGTH = ?STRENGTH,
                                            WEAKNESS = ?WEAKNESS,
                                            ROLE_MODEL = ?ROLE_MODEL
                                        WHERE STUDENT_ACTIVITY_ID = ?STUDENT_ACTIVITY_ID
                                        AND STUDENT_ID = ?STUDENT_ID;";

                                    
                                            
                        break;
                    }
                case MentorSQLCommend.InsertStudentHealthDet:
                    {
                        query = @"INSERT INTO mentoring_student_health_details (
                                    ANY_DISABILITY,
                                    ASTHMA,
                                    ALLERGY,
                                    DIABETES,
                                    HEART_DISEASE,
                                    WHEEZING,
                                    SKIN_DISEASE,
                                    HYSTERIA,
                                    DISABILITY_TYPE,
                                    HOSPITALIZED_FOR_ANY,
                                    HOSPITALIZED_REASON,
                                    ANYONE_SERIOUSLY_ILL,
                                    ANY_ALLERGY,
                                    ALLERGY_REASON,
                                    DISABILITY_PERCENTAGE,
                                    STUDENT_ID,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED
                                ) VALUES (
                                    ?ANY_DISABILITY,
                                    ?ASTHMA,
                                    ?ALLERGY,
                                    ?DIABETES,
                                    ?HEART_DISEASE,
                                    ?WHEEZING,
                                    ?SKIN_DISEASE,
                                    ?HYSTERIA,
                                    ?DISABILITY_TYPE,
                                    ?HOSPITALIZED_FOR_ANY,
                                    ?HOSPITALIZED_REASON,
                                    ?ANYONE_SERIOUSLY_ILL,
                                    ?ANY_ALLERGY,
                                    ?ALLERGY_REASON,
                                    ?DISABILITY_PERCENTAGE,
                                    ?STUDENT_ID,
                                    ?ACADEMIC_YEAR,
                                    ?IS_ACTIVE,
                                    ?IS_DELETED);";
                        break;
                    }
                case MentorSQLCommend.FetchealthDetails:
                    {
                        query = @"SELECT
                                    STUDENT_HEALTH_ID,
                                    ANY_DISABILITY,
                                    ASTHMA,
                                    ALLERGY,
                                    DIABETES,
                                    HEART_DISEASE,
                                    WHEEZING,
                                    SKIN_DISEASE,
                                    HYSTERIA,
                                    DISABILITY_TYPE,
                                    HOSPITALIZED_FOR_ANY,
                                    HOSPITALIZED_REASON,
                                    ANYONE_SERIOUSLY_ILL,
                                    ANY_ALLERGY,
                                    ALLERGY_REASON,
                                    DISABILITY_PERCENTAGE,
                                    STUDENT_ID,
                                    ACADEMIC_YEAR,
                                    IS_ACTIVE,
                                    IS_DELETED
                                FROM mentoring_student_health_details
                                WHERE STUDENT_ID = ?STUDENT_ID
                                  AND IS_ACTIVE = '1'
                                  AND IS_DELETED = '0';";
                        break;
                    }
                case MentorSQLCommend.FetchStudentAcademicdetails:
                    {
                        query = @"SELECT
                STU_ACADEMIC_ID,
                STUDENT_ID,
                SSLC_PERCENTAGE,
                SSLC_SCHOOL,
                HSC_PERCENTAGE,
                HSC_SCHOOL,
                IS_ACTIVE,
                IS_DELETED
              FROM mentor_student_academic_details
              WHERE STUDENT_ID = ?STUDENT_ID
                AND IS_ACTIVE = '1'
                AND IS_DELETED = '0';";
                        break;
                    }
                case MentorSQLCommend.InsertStudentAcademicDetails:
                    {
                        query = @"INSERT INTO mentor_student_academic_details (
                                                    STUDENT_ID,
                                                    SSLC_PERCENTAGE,
                                                    SSLC_SCHOOL,
                                                    HSC_PERCENTAGE,
                                                    HSC_SCHOOL,
                                                    IS_ACTIVE
                                                  ) VALUES (
                                                    ?STUDENT_ID,
                                                    ?SSLC_PERCENTAGE,
                                                    ?SSLC_SCHOOL,
                                                    ?HSC_PERCENTAGE,
                                                    ?HSC_SCHOOL,
                                                    ?IS_ACTIVE
                                                  );";
                        break;
                    }
                case MentorSQLCommend.UpdateStudentAcademicDetails:
                    {
                        query = @"UPDATE mentor_student_academic_details
              SET
                SSLC_PERCENTAGE = ?SSLC_PERCENTAGE,
                SSLC_SCHOOL = ?SSLC_SCHOOL,
                HSC_PERCENTAGE = ?HSC_PERCENTAGE,
                HSC_SCHOOL = ?HSC_SCHOOL,
                IS_ACTIVE = ?IS_ACTIVE,
                IS_DELETED = ?IS_DELETED
              WHERE STU_ACADEMIC_ID = ?STU_ACADEMIC_ID;
                                            ";
                        break;
                    }
                case MentorSQLCommend.FetchStudentAcademicAnalysis:
                    {
                        query = @"SELECT 
    ANALYSIS_ID,
    STUDENT_ID,
    SEMESTER,
    TOTAL_SUBJECTS,
    SUBJECTS_PASSED,
    ARREARS,
    PERCENTAGE
FROM mentor_student_academic_analysis
WHERE STUDENT_ID = ?STUDENT_ID
  AND IS_ACTIVE = '1'
  AND IS_DELETED = '0'
ORDER BY FIELD(SEMESTER,'I','II','III','IV','V','VI');";
                        break;
                    }
                case MentorSQLCommend.DeleteMentoringDetails:
                    {
                        query = @"UPDATE mentor_visit_details
	                                        SET 
                                            IS_DELETED = 1
                                            WHERE 
                                            STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchRegstudentbystudentId:
                    {
                        query = @"SELECT 
		                            STU.ROLL_NO,
		                            STU.FIRST_NAME,
                                    CL.CLASS_CODE,
		                            MR.MENTORS_REGULAR_VISIT_ID,
		                            MR.STUDENT_ID,
		                            MR.CLASS_ID, 
                                    MR.MENTOR_ID,
                                    MR.REGULAR_VISIT,
                                    MR.SHORT_REVIEW,
                                    DATE_FORMAT(MR.VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                    MR.VISIT_TIME,
                                    MR.VISIT_DURATION,
                                    MR.MATTER_DISCUSSED,
                                    MR.CONCLUSION_ARRIVED_AT,
                                    MR.ACTION_TAKEN,
                                    MR.GENERAL_IMPRESSION,
                                    DATE_FORMAT(MR.DATE_OF_NEXT_VISIT, '%d-%m-%Y') AS DATE_OF_NEXT_VISIT,
                                    MR.ACADEMIC_YEAR,
                                    MR.CREATED_AT,
                                    MR.IS_PENDING,
                                    MR.IS_ACTIVE,
                                    MR.IS_DELETED
                                    FROM 
                                    mentor_regular_visit_details MR
                                    INNER JOIN 
                                    stu_personal_info AS STU ON STU.STUDENT_ID = MR.STUDENT_ID
                                    INNER JOIN 
                                    cp_classes AS CL ON CL.CLASS_ID = MR.CLASS_ID
									WHERE
                                    MR.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case MentorSQLCommend.UpdateMentorRegulerDetails:
                    {
                        query = @"UPDATE mentor_regular_visit_details
                                                        SET
                                                            REGULAR_VISIT = ?REGULAR_VISIT,
                                                            SHORT_REVIEW = ?SHORT_REVIEW,
                                                            VISIT_DATE = ?VISIT_DATE,
                                                            VISIT_TIME = ?VISIT_TIME,
                                                            VISIT_DURATION = ?VISIT_DURATION,
                                                            MATTER_DISCUSSED = ?MATTER_DISCUSSED,
                                                            CONCLUSION_ARRIVED_AT = ?CONCLUSION_ARRIVED_AT,
                                                            ACTION_TAKEN = ?ACTION_TAKEN,
                                                            GENERAL_IMPRESSION = ?GENERAL_IMPRESSION,
                                                            DATE_OF_NEXT_VISIT = ?DATE_OF_NEXT_VISIT,
                                                            IS_DELETED = 0,
                                                            IS_ACTIVE = 1
                                                        WHERE
                                                            STUDENT_ID = ?STUDENT_ID;
                                                        ";
                        break;
                    }
                case MentorSQLCommend.UpdateMentoringGroupDetails:
                    {
                        query = @"SELECT 
		                                STU.ROLL_NO,
		                                STU.FIRST_NAME,
                                        CL.CLASS_CODE,
		                                GD.MENTOR_GROUP_VISIT_ID,
                                        GD.STUDENT_ID,
                                        GD.CLASS_ID,
                                        GD.MENTOR_ID,
                                        DATE_FORMAT(GD.VISIT_DATE, '%d-%m-%Y') AS VISIT_DATE,
                                        GD.VISIT_TIME,
                                        GD.VISIT_DURATION,
                                        GD.SHORT_REVIEW,
                                        GD.MATTER_DISCUSSED,
                                        GD.CONCULATION_ARRIVED_AT,
                                        GD.ACTION_TAKEN,
                                        GD.GENERAL_IMPRESSION,
                                        GD.SELF_ASSESSMENT,
                                        DATE_FORMAT(GD.DATE_OF_NEXT_VISIT,'%d-%m-%Y') AS DATE_OF_NEXT_VISIT,
                                        GD.ACADEMIC_YEAR,
                                        GD.CREATED_AT,
                                        GD.IS_ACTIVE,
                                        GD.IS_DELETED
                                        FROM 
                                        mentor_group_visit_details GD
                                        INNER JOIN 
                                        stu_personal_info AS STU ON STU.STUDENT_ID = GD.STUDENT_ID
                                        INNER JOIN 
                                        cp_classes AS CL ON CL.CLASS_ID = GD.CLASS_ID
                                        WHERE
                                        GD.STUDENT_ID = ?STUDENT_ID; ";
                        break;
                    }

                case MentorSQLCommend.UpdateMentoringvisitDet:
                    {
                        query = @"UPDATE mentor_visit_details
                                                SET 
                                                    CLASS_ID = ?CLASS_ID,
                                                    MENTOR_ID = ?MENTOR_ID, 
                                                    NATURE_OF_VISIT = ?NATURE_OF_VISIT, 
                                                    INITIAL_CONTACT_VISIT = ?INITIAL_CONTACT_VISIT, 
                                                    VISIT_DATE = ?VISIT_DATE, 
                                                    VISIT_TIME = ?VISIT_TIME, 
                                                    VISIT_DURATION = ?VISIT_DURATION, 
                                                    MATTER_DISCUSSED = ?MATTER_DISCUSSED, 
                                                    CONCLUSION_ARRIVED_AT = ?CONCLUSION_ARRIVED_AT, 
                                                    ACTION_TAKEN = ?ACTION_TAKEN, 
                                                    GENERAL_IMPRESSION = ?GENERAL_IMPRESSION, 
                                                    DATE_OF_NEXT_VISIT = ?DATE_OF_NEXT_VISIT, 
                                                    CAMPUS_ID = ?CAMPUS_ID, 
                                                    ACADEMIC_YEAR = ?ACADEMIC_YEAR, 
                                                    CREATED_AT = CREATED_AT, 
                                                    IS_ACTIVE = 1, 
                                                    IS_DELETED = 0
                                                WHERE 
                                                     STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case MentorSQLCommend.UpdateMentoringGPDetails:
                    {
                        query = @"UPDATE mentor_group_visit_details
                                            SET 
                                                CLASS_ID = ?CLASS_ID, 
                                                MENTOR_ID = ?MENTOR_ID, 
                                                VISIT_DATE = VISIT_DATE 
                                                VISIT_TIME = ?VISIT_TIME, 
                                                VISIT_DURATION = ?VISIT_DURATION, 
                                                SHORT_REVIEW = ?SHORT_REVIEW, 
                                                MATTER_DISCUSSED = ?MATTER_DISCUSSED, 
                                                CONCULATION_ARRIVED_AT = ?CONCULATION_ARRIVED_AT, 
                                                ACTION_TAKEN = ?ACTION_TAKEN, 
                                                GENERAL_IMPRESSION = ?GENERAL_IMPRESSION, 
                                                SELF_ASSESSMENT = ?SELF_ASSESSMENT, 
                                                DATE_OF_NEXT_VISIT = ?DATE_OF_NEXT_VISIT, 
                                                ACADEMIC_YEAR = ?ACADEMIC_YEAR, 
                                                CREATED_AT = ?CREATED_AT, 
                                                IS_ACTIVE = 1, 
                                                IS_DELETED = 0
                                            WHERE 
                                                STUDENT_ID = ?STUDENT_ID;
                                            ";
                        break;
                    }
                case MentorSQLCommend.DeleteMentorReularDetails:
                    {
                        query = @"UPDATE mentor_regular_visit_details
	                                        SET 
                                            IS_DELETED = 1
                                            WHERE 
                                            STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case MentorSQLCommend.DeleteMentorGroupDetails:
                    {
                        query = @"UPDATE mentor_group_visit_details
	                                        SET 
                                            IS_DELETED = 1
                                            WHERE 
                                            STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case MentorSQLCommend.InsertLanguageDetails:
                    {
                        query = @"INSERT INTO mentor_language_details(
			                                        LANGUAGE_ID,
                                                    READ_ID,
                                                    WRITE_ID,
                                                    SPEAK_ID,
                                                    IS_DELETED,
                                                    IS_ACTIVATE)
                                                    VALUES
                                                    (
                                                    ?LANGUAGE_ID,
                                                    ?READ_ID,
                                                    ?WRITE_ID,
                                                    ?SPEAK_ID,
                                                    0,
                                                    1
                                                    );
                                                     SELECT LAST_INSERT_ID();";
                        break;
                    }
                case MentorSQLCommend.UpdateFamilyDetails:
                    {
                        query = @"UPDATE MENTOR_FAMILY_DETAILS
                                                    SET 
                                                        FR_NAME = ?FR_NAME,
                                                        FR_EDUCATION = ?FR_EDUCATION,
                                                        FR_OCCUPATION = ?FR_OCCUPATION,
                                                        FR_INCOME = ?FR_INCOME,
                                                        MOTHER_NAME = ?MOTHER_NAME,
                                                        MOTHER_EDUCATION = ?MOTHER_EDUCATION,
                                                        MOTHER_OCCUPATION = ?MOTHER_OCCUPATION,
                                                        MOTHER_INCOME = ?MOTHER_INCOME,
                                                        GUARDIAN_NAME = ?GUARDIAN_NAME,
                                                        GUARDIAN_EDUCATION = ?GUARDIAN_EDUCATION,
                                                        GUARDIAN_OCCUPATION = ?GUARDIAN_OCCUPATION,
                                                        GUARDIAN_INCOME = ?GUARDIAN_INCOME,
                                                        ACADEMIC_YEAR = ?ACADEMIC_YEAR,
                                                        NO_OFSISTERS = ?NO_OFSISTERS,
                                                        NO_OFBROTHERS = ?NO_OFBROTHERS,
                                                        FAMILY_TYPE = ?FAMILY_TYPE,
                                                        PROGRAM_ID = ?PROGRAM_ID,
                                                        CLASS_ID = ?CLASS_ID,
                                                        IS_DELETED = 0,
                                                        IS_ACTIVE = 1
                                                    WHERE STUDENT_ID = ?STUDENT_ID;
                                                    ";
                        break;
                    }
                case MentorSQLCommend.FetchMaxLanValues:
                    {
                        query = @"SELECT 
                                    MAX(MENTOR_LANGUAGE_ID),
                                    STUDENT_ID,
                                    CLASS_ID,
                                    PROGRAM_ID,
                                    ACADEMIC_YEAR
                                FROM
                                    mentor_language_details;
                                ";
                        break;
                    }
                case MentorSQLCommend.UpdateInovationDetils:
                    {
                        query = @"UPDATE mentoring_Intervention_Guidance_Details SET
                                    INTERVENTION_AND_GUIDANCE = ?INTERVENTION_AND_GUIDANCE ,
                                    VISIT_DATE = ?VISIT_DATE
                                    WHERE 
                                    STUDENT_GUIDANCE_ID = ?STUDENT_GUIDANCE_ID;";
                        break;
                    }
                case MentorSQLCommend.DeleteGuidenceDetais:
                    {
                        query = @"UPDATE mentoring_Intervention_Guidance_Details
                                SET
                                    IS_DELETED = 1
                                WHERE
                                    STUDENT_GUIDANCE_ID = ?STUDENT_GUIDANCE_ID;";
                        break;
                    }

                case MentorSQLCommend.saveMentoringstudentlist:
                    {
                        query = @"INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID, CLASS_ID, STAFF_ID, BATCH)
                                  VALUE (?STUDENT_ID, ?CLASS_ID, ?STAFF_ID, ?BATCH);";
                        break;
                    }
                case MentorSQLCommend.updateMentoringstudentlist:
                    {
                        query = @"UPDATE MENTORING_STUDENT_LIST SET 
                                  STUDENT_ID=?STUDENT_ID,
                                  CLASS_ID=?CLASS_ID,
                                  STAFF_ID=?STAFF_ID,
                                  BATCH=?BATCH
                                  WHERE MENTORINT_ID=?MENTORINT_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchStudentMentoring:
                    {
                        query = @"   SELECT 
                                            SPI.ROLL_NO,
                                            SPI.FIRST_NAME,
                                            CC.CLASS_NAME,
                                            STF.FIRST_NAME,
                                            MSL.BATCH
                                        FROM
                                            MENTORING_STUDENT_LIST AS MSL
                                                INNER JOIN
                                            stu_class AS SC ON SC.STUDENT_ID = MSL.STUDENT_ID
                                                INNER JOIN
                                            stu_personal_info AS SPI ON SPI.STUDENT_ID = MSL.STUDENT_ID
                                                INNER JOIN
                                            CP_CLASSES AS CC ON CC.CLASS_ID = MSL.CLASS_ID
                                                INNER JOIN
                                            stf_personal_info AS STF ON STF.STAFF_ID = MSL.STAFF_ID
                                            WHERE
                                            MSL.STAFF_ID=?STAFF_ID
                                            AND SPI.IS_DELETED!=1 
                                            AND SPI.IS_LEFT!=1
                                            AND SC.ACADEMIC_YEAR=?ACADEMIC_YEAR
                                            ORDER BY CC.CLASS_ID,SPI.STUDENT_ID,MSL.BATCH;";
                        break;
                    }
                case MentorSQLCommend.FetchStaffListForMentoring:
                    {
                        query = @"SELECT 
                                       SP.STAFF_ID, SP.FIRST_NAME, SP.STAFF_CODE, CD.DEPARTMENT, MENTORINT_ID, STUDENT_ID, BATCH,UA.PHOTO,CD.DEPARTMENT_ID AS DEPT_ID
                                   FROM
                                       STF_PERSONAL_INFO AS SP
                                           INNER JOIN
                                       USER_ACCOUNT AS UA ON UA.USER_ID = SP.STAFF_ID
                                           INNER JOIN
                                       CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                           AND CD.IS_DELETED != 1
				                  	 LEFT JOIN 
                                       MENTORING_STUDENT_LIST AS MT ON MT.STAFF_ID = SP.STAFF_ID
                                   WHERE
                                       SP.CATEGORY = ?CATEGORY AND SP.IS_DELETED != 1 AND SP.IS_LEFT !=1 GROUP BY SP.STAFF_ID
                                   ORDER BY CD.DEPARTMENT,SP.STAFF_CODE,MENTORINT_ID ;";
                        break;
                    }
                case MentorSQLCommend.FetchMentoring:
                    {
                        query = @"SELECT 
                                       UPPER(SP.FIRST_NAME) AS FIRST_NAME, SP.DATE_OF_BIRTH, CLASS_NAME, CLASS_YEAR,CONCAT(STP.FIRST_NAME,' [ ',CD.DEPARTMENT,' ]')  AS STAFF_NAME,MT.STAFF_ID,SP.DEPT_ID AS DEPT_ID,STP.PHOTO  
                                   FROM
                                       STU_CLASS AS SC
                                           INNER JOIN
                                       CP_CLASSES AS C ON C.CLASS_ID = SC.CLASS_ID
                                           INNER JOIN
                                       STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID        
                                           LEFT JOIN
                                       MENTORING_STUDENT_LIST AS MT ON MT.STUDENT_ID = SP.STUDENT_ID AND MT.IS_DELETED !=1
                                          LEFT JOIN 
                                   	STF_PERSONAL_INFO AS STP ON STP.STAFF_ID = MT.STAFF_ID AND STP.IS_LEFT !=1 AND STP.IS_DELETED !=1
                                          LEFT JOIN
                                        CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = STP.DEPARTMENT AND CD.IS_DELETED !=1 AND CD.IS_DELETED != 1
                                  WHERE
                                      SC.ACADEMIC_YEAR = ?AC_YEAR
                                          AND SC.IS_DELETED != 1
                                          AND SC.STUDENT_ID = ?STUDENT_ID LIMIT 1;";
                        break;
                    }
                case MentorSQLCommend.SaveMentorforstudent:
                    {
                        query = @" INSERT INTO MENTORING_STUDENT_LIST (STUDENT_ID,STAFF_ID,IS_ACTIVE)VALUE(?STUDENT_ID,?STAFF_ID,?IS_ACTIVE);";
                        break;
                    }
                case MentorSQLCommend.UpdateMentorforstudent:
                    {
                        query = @" UPDATE MENTORING_STUDENT_LIST SET STAFF_ID = ?STAFF_ID, IS_ACTIVE = ?IS_ACTIVE WHERE MENTORINT_ID = ?MENTORINT_ID;";
                        break;
                    }
                case MentorSQLCommend.fetchstudentformentoring:
                    {
                        query = @" SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SP.APPLICATION_NO,
                                        SP.ADMISSION_NO,
                                        SP.FIRST_NAME,
                                        CS.CLASS_NAME,
                                        CS.CLASS_YEAR,
                                        SM.PROGRAMME_MODE,
                                       CONCAT(SP.ADMISSION_DATE) AS ADMISSION_DATE,CD.DEPARTMENT,MT.STAFF_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CC ON CC.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
										CP_PROGRAMME AS CD ON CD.PROGRAMME_ID = CC.PROGRAMME_ID
                                            INNER JOIN
                                        CP_CLASSES AS CS ON CS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN 
                                        SUP_PROGRAMME_MODE AS SM ON SM.PROGRAMME_MODE_ID=CS.PROGRAMME_MODE
                                           LEFT JOIN
                                        MENTORING_STUDENT_LIST AS MT ON MT.STUDENT_ID = SP.STUDENT_ID AND MT.IS_DELETED !=1
                                    WHERE
                                        SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.PROGRAMME_GROUP_ID = ?PROGRAMME_GROUP_ID
                                            AND CS.CLASS_YEAR = ?PROGRAMME_YEAR
                                            AND SP.IS_DELETED != 1
                                    AND SP.IS_LEFT!=1;";
                        break;
                    }
                case MentorSQLCommend.fetchstudentformentoringdropdown:
                    {
                        query = @"SELECT 
                                    MS.STUDENT_ID,
                                    MS.STAFF_ID,
                                    SP.FIRST_NAME,
                                    SP.ROLL_NO
                                FROM
                                    mentoring_student_list AS MS
                                        INNER JOIN
                                    stu_personal_info AS SP ON MS.STUDENT_ID = SP.STUDENT_ID
                                WHERE
                                    SP.IS_LEFT != 1
                                    AND MS.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchStaffListForMentoringadmin:
                    {
                        query = @"SELECT 
                                       SP.STAFF_ID, SP.FIRST_NAME, SP.STAFF_CODE, CD.DEPARTMENT,UA.PHOTO,CD.DEPARTMENT_ID AS DEPT_ID
                                   FROM
                                       STF_PERSONAL_INFO AS SP
                                           INNER JOIN
                                       USER_ACCOUNT AS UA ON UA.USER_ID = SP.STAFF_ID
                                           INNER JOIN
                                       CP_DEPARTMENT AS CD ON CD.DEPARTMENT_ID = SP.DEPARTMENT
                                           AND CD.IS_DELETED != 1
                                   WHERE
                                       SP.CATEGORY = ?CATEGORY AND SP.IS_DELETED != 1 AND SP.IS_LEFT !=1 AND SP.DEPARTMENT !=?DEPARTMENT GROUP BY SP.STAFF_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchStaffforMetoringQuota:
                    {
                        query = @"SELECT 
                                    STF.FIRST_NAME,
                                    STF.STAFF_CODE,
                                    STF.STAFF_ID,
                                    STF.DEPARTMENT,
                                    CPD.DEPARTMENT
                                FROM
                                    STF_PERSONAL_INFO AS STF
                                        INNER JOIN
                                    CP_DEPARTMENT AS CPD ON CPD.DEPARTMENT_ID = STF.DEPARTMENT
                                        AND CPD.IS_DELETED != 1
                                WHERE
                                    STF.STAFF_ID IN(?STAFF_ID) AND STF.DEPARTMENT IN(?DEPARTMENT)
                                        AND STF.IS_DELETED != 1
                                        AND STF.IS_LEFT != 1;";
                        break;
                    }
                case MentorSQLCommend.SaveMentoringquotadetails:
                    {
                        query = @"INSERT INTO MENTORING_QUOTA_ALLOCATION(
                                                    STAFF_ID,
                                                     UG1,
                                                     UG2,
                                                     UG3,
                                                     PG1,
                                                     PG2,
                                                     TOTAL)
                                                     VALUES (
                                                     ?STAFF_ID,
                                                     ?UG1,
                                                     ?UG2,
                                                     ?UG3,
                                                     ?PG1,
                                                     ?PG2,
                                                     ?TOTAL);";
                        break;
                    }
                case MentorSQLCommend.StaffMentoringQuotaaldreadyexist:
                    {
                        query = @"SELECT 
                                        QUOTA_ID,
                                        STAFF_ID,
                                        UG1,
                                        UG2,
                                        UG3,
                                        PG1,
                                        PG2,
                                        TOTAL,
                                        IS_DELETED,
                                        IS_ACTIVE
                                    FROM
                                        MENTORING_QUOTA_ALLOCATION
                                    WHERE
                                        STAFF_ID = ?STAFF_ID AND IS_DELETED != 1;";
                        break;
                    }
                case MentorSQLCommend.UpdateMentoringQuotaDetails:
                    {
                        query = @"UPDATE MENTORING_QUOTA_ALLOCATION SET 
                                                            UG1 = ?UG1,
                                                            UG2 = ?UG2,
                                                            UG3 = ?UG3,
                                                            PG1 = ?PG1,
                                                            PG2 = ?PG2,
                                                            TOTAL = ?TOTAL
                                                            WHERE 
                                                                STAFF_ID = ?STAFF_ID;";
                        break;
                    }


                case MentorSQLCommend.PARTICIPATION_ACHIEVEMENTS:
                    {
                        query = @"SELECT 
                                                STUDENT_HEALTH_ID,
                                                ANY_DISABILITY,
                                                ASTHMA,
                                                ALLERGY,
                                                DIABETES,
                                                HEART_DISEASE,
                                                WHEEZING,
                                                SKIN_DISEASE,
                                                HYSTERIA,
                                                DISABILITY_TYPE,
                                                HOSPITALIZED_FOR_ANY,
                                                HOSPITALIZED_REASON,
                                                ANYONE_SERIOUSLY_ILL,
                                                ANY_ALLERGY,
                                                ALLERGY_REASON,
                                                COUNT,
                                                STUDENT_ID,
                                                ACADEMIC_YEAR,
                                                PROGRAM_ID,
                                                CLASS_ID,
                                                IS_ACTIVE,
                                                IS_DELETED
                                            FROM
                                                MENTORING_STUDENT_HEALTH_DETAILS
                                                  WHERE 
                                        
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }



                case MentorSQLCommend.UpdateHealthStudentDetails:
                    {
                        query = @"UPDATE mentoring_student_health_details
                                    SET
                                        ANY_DISABILITY = ?ANY_DISABILITY,
                                        ASTHMA = ?ASTHMA,
                                        ALLERGY = ?ALLERGY,
                                        DIABETES = ?DIABETES,
                                        HEART_DISEASE = ?HEART_DISEASE,
                                        WHEEZING = ?WHEEZING,
                                        SKIN_DISEASE = ?SKIN_DISEASE,
                                        HYSTERIA = ?HYSTERIA,
                                        DISABILITY_TYPE = ?DISABILITY_TYPE,
                                        HOSPITALIZED_FOR_ANY = ?HOSPITALIZED_FOR_ANY,
                                        HOSPITALIZED_REASON = ?HOSPITALIZED_REASON,
                                        ANYONE_SERIOUSLY_ILL = ?ANYONE_SERIOUSLY_ILL,
                                        ANY_ALLERGY = ?ANY_ALLERGY,
                                        ALLERGY_REASON = ?ALLERGY_REASON,
                                        DISABILITY_PERCENTAGE = ?DISABILITY_PERCENTAGE,
                                        IS_ACTIVE = ?IS_ACTIVE,
                                        IS_DELETED = ?IS_DELETED
                                    WHERE STUDENT_HEALTH_ID = ?STUDENT_HEALTH_ID;";
                        break;
                    }


                case MentorSQLCommend.FetchSkillandTalent:
                    {
                        query = @"SELECT 
                                    SKILLS_AND_TALENTS_ID,
                                    STUDENT_ID,
                                    SS.SEMESTER_NAME,
                                    SKILLS,
                                    ACADEMIC_YEAR,
                                    CREATED_AT,
                                    SAT.IS_ACTIVE,
                                    SAT.IS_DELETED
                                FROM
                                    SKILLS_AND_TALENTS AS SAT
                                        INNER JOIN
                                    SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID = SAT.SEMESTER_ID
                                WHERE
                                    SAT.STUDENT_ID = ?STUDENT_ID
                                        AND SAT.IS_DELETED != 1
                                ORDER BY SS.SUP_SEMESTER_ID ASC;";
                        break;
                    }


                case MentorSQLCommend.InsertSkillandTalents:
                    {
                        query = @"     INSERT INTO skills_and_talents
                                            (
                                            STUDENT_ID,
                                            SEMESTER_ID,
                                            SEMESTER_NAME,
                                            SKILLS,
                                            ACADEMIC_YEAR,
                                            CREATED_AT,
                                            IS_ACTIVE)
                                            VALUES
                                            (
                                            ?STUDENT_ID,
                                            ?SEMESTER_ID,
                                            ?SEMESTER_NAME,
                                            ?SKILLS,
                                            ?ACADEMIC_YEAR,
                                            NOW(),
                                            1);";
                                                
                        break;
                    }
                case MentorSQLCommend.FetchSkillById:
                    {
                        query = @"SELECT 
                                        SKILLS_AND_TALENTS_ID,
                                        STUDENT_ID,
                                        SEMESTER_ID,
                                        SKILLS,
                                        ACADEMIC_YEAR,
                                        CREATED_AT,
                                        IS_ACTIVE,
                                        IS_DELETED
                                    FROM SKILLS_AND_TALENTS
                                    WHERE SKILLS_AND_TALENTS_ID = ?SKILLS_AND_TALENTS_ID
                                    AND IS_DELETED = 0;";
                        break;
                    }
                case MentorSQLCommend.UpdateSkillandTalents:
                    {
                        query = @"UPDATE SKILLS_AND_TALENTS
                                        SET
                                            STUDENT_ID=?STUDENT_ID,
                                            SEMESTER_ID = ?SEMESTER_ID,
                                            SKILLS = ?SKILLS,
                                            ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                        WHERE
                                            SKILLS_AND_TALENTS_ID = ?SKILLS_AND_TALENTS_ID
                                            AND IS_DELETED != 1;";
                        break;
                    }
                case MentorSQLCommend.DeleteSkillandTalents:
                    {
                        query = @"UPDATE SKILLS_AND_TALENTS
                                SET
                                    IS_DELETED = 1
                                WHERE SKILLS_AND_TALENTS_ID = ?SKILLS_AND_TALENTS_ID;";
                        break;
                    }
         


              

                case MentorSQLCommend.FetchValuesandclass:
                    {
                        query = @"SELECT 
                                        SP.STUDENT_ID,
                                        SP.ROLL_NO,
                                        SP.REGISTER_NO,
                                        SP.FIRST_NAME,
                                        CS.CLASS_NAME,
                                        CS.CLASS_ID,
                                        SC.ACADEMIC_YEAR,
                                        SM.PROGRAMME_MODE,
                                        CD.PROGRAMME_ID
                                    FROM
                                        STU_CLASS AS SC
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SP ON SP.STUDENT_ID = SC.STUDENT_ID
                                            INNER JOIN
                                        CP_PROGRAMME_GROUP AS CC ON CC.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                            INNER JOIN
                                        CP_PROGRAMME AS CD ON CD.PROGRAMME_ID = CC.PROGRAMME_ID
                                            INNER JOIN
                                        CP_CLASSES AS CS ON CS.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN 
                                        SUP_PROGRAMME_MODE AS SM ON SM.PROGRAMME_MODE_ID=CS.PROGRAMME_MODE
                                    WHERE
                                        SC.STUDENT_ID = ?STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                            AND SP.IS_DELETED != 1;";
                        break;
                    }

                case MentorSQLCommend.FetchVACCourses:
                    {
                        query = @"SELECT
                                    VAC_ID,
                                    STUDENT_ID,
                                    SEMESTER,
                                    COURSE_NAME,
                                    DURATION,
                                    INSTITUTION_PLATFORM,
                                    OUTCOME_CERTIFICATE
                                FROM Mentor_VAC_Professional_Courses
                                WHERE STUDENT_ID = ?STUDENT_ID
                                  AND IS_DELETED = 0
                                ORDER BY
                                    FIELD(SEMESTER, 'I','II','III','IV','V','VI');
";
                        break;
                    }
                case MentorSQLCommend.InsertVACCourse:
                    {
                        query = @"INSERT INTO Mentor_VAC_Professional_Courses
                                (
                                    STUDENT_ID,
                                    SEMESTER,
                                    COURSE_NAME,
                                    DURATION,
                                    INSTITUTION_PLATFORM,
                                    OUTCOME_CERTIFICATE,
                                    IS_ACTIVE,
                                    IS_DELETED,
                                    CREATED_DATE
                                )
                                VALUES
                                (
                                    ?STUDENT_ID,
                                    ?SEMESTER,
                                    ?COURSE_NAME,
                                    ?DURATION,
                                    ?INSTITUTION_PLATFORM,
                                    ?OUTCOME_CERTIFICATE,
                                    1,
                                    0,
                                    NOW()
                                );";
                        break;
                    }
                case MentorSQLCommend.UpdateVACCourse:
                    {
                        query = @"UPDATE Mentor_VAC_Professional_Courses
                                SET
                                    SEMESTER = ?SEMESTER,
                                    COURSE_NAME = ?COURSE_NAME,
                                    DURATION = ?DURATION,
                                    INSTITUTION_PLATFORM = ?INSTITUTION_PLATFORM,
                                    OUTCOME_CERTIFICATE = ?OUTCOME_CERTIFICATE,
                                    UPDATED_DATE = NOW()
                                WHERE VAC_ID = ?VAC_ID;";
                        break;
                    }
                case MentorSQLCommend.DeleteVACCourse:
                    {
                        query = @"UPDATE Mentor_VAC_Professional_Courses
                                SET
                                    IS_DELETED = 1,
                                    UPDATED_DATE = NOW()
                                WHERE VAC_ID = ?VAC_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchContributionTypes:
                    {
                        query = @"SELECT
                                CONTRIBUTION_TYPE_ID,
                                TYPE_NAME
                            FROM SUP_CONTRIBUTION_TYPE
                            WHERE IS_DELETED = 0
                              AND IS_ACTIVE = 1
                            ORDER BY CONTRIBUTION_TYPE_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchContributions:
                    {
                        query = @"SELECT
                                C.CONTRIBUTION_ID,
                                C.STUDENT_ID,
                                C.CONTRIBUTION_TYPE_ID,
                                S.TYPE_NAME AS CONTRIBUTION_TYPE_NAME,
                                DATE_FORMAT(C.EVENT_DATE,'%d/%m/%Y') as EVENT_DATE,
                                C.EVENT_NAME,
                                C.PLACE,
                                C.NATURE_OF_CONTRIBUTION,
                                C.REMARKS
                            FROM Mentor_Leadership_Contributions C
                            INNER JOIN SUP_CONTRIBUTION_TYPE S
                                ON S.CONTRIBUTION_TYPE_ID = C.CONTRIBUTION_TYPE_ID
                            WHERE C.STUDENT_ID = ?STUDENT_ID
                              AND C.IS_DELETED = 0;";
                        break;
                    }
                case MentorSQLCommend.InsertContribution:
                    {
                        query = @"INSERT INTO Mentor_Leadership_Contributions
                                        (
                                            STUDENT_ID,
                                            CONTRIBUTION_TYPE_ID,
                                            EVENT_DATE,
                                            EVENT_NAME,
                                            PLACE,
                                            NATURE_OF_CONTRIBUTION,
                                            REMARKS,
                                            IS_ACTIVE,
                                            IS_DELETED,
                                            CREATED_DATE
                                        )
                                        VALUES
                                        (
                                            ?STUDENT_ID,
                                            ?CONTRIBUTION_TYPE_ID,
                                            ?EVENT_DATE,
                                            ?EVENT_NAME,
                                            ?PLACE,
                                            ?NATURE_OF_CONTRIBUTION,
                                            ?REMARKS,
                                            1,
                                            0,
                                            NOW()
                                        );";
                        break;
                    }
                case MentorSQLCommend.UpdateContribution:
                    {
                        query = @"UPDATE Mentor_Leadership_Contributions
                                    SET
                                        CONTRIBUTION_TYPE_ID = ?CONTRIBUTION_TYPE_ID,
                                        EVENT_DATE = ?EVENT_DATE,
                                        EVENT_NAME = ?EVENT_NAME,
                                        PLACE = ?PLACE,
                                        NATURE_OF_CONTRIBUTION = ?NATURE_OF_CONTRIBUTION,
                                        REMARKS = ?REMARKS,
                                        UPDATED_DATE = NOW()
                                    WHERE CONTRIBUTION_ID = ?CONTRIBUTION_ID;";
                        break;
                    }
                case MentorSQLCommend.DeleteContribution:
                    {
                        query = @"UPDATE Mentor_Leadership_Contributions
                                    SET
                                        IS_DELETED = 1,
                                        UPDATED_DATE = NOW()
                                    WHERE CONTRIBUTION_ID = ?CONTRIBUTION_ID;";
                        break;
                    }
                case MentorSQLCommend.DeleteBehaviourRecord:
                    {
                        query = @" UPDATE MENTOR_BEHAVIOURAL_CONDUCT_RECORD
                                    SET
                                        IS_DELETED = 1
                                    WHERE RECORD_ID = ?RECORD_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchContributionById:
                    {
                        query = @"SELECT
                                    CONTRIBUTION_ID,
                                    STUDENT_ID,
                                    CONTRIBUTION_TYPE_ID,
                                    EVENT_DATE,
                                    EVENT_NAME,
                                    PLACE,
                                    NATURE_OF_CONTRIBUTION,
                                    REMARKS
                                FROM Mentor_Leadership_Contributions
                                WHERE CONTRIBUTION_ID = ?CONTRIBUTION_ID
                                AND IS_DELETED = 0;";
                        break;
                    }
                case MentorSQLCommend.FetchBehaviouralRecords:
                    {
                        query = @" SELECT
                                        RECORD_ID,
                                        STUDENT_ID,
                                        sl.LEVEL_NAME,
                                        INCIDENT_DATE,
                                        INCIDENT_TIME,
                                        INCIDENT_DETAILS,
                                        ACTION_TAKEN,
                                        REMARKS
                                    FROM
                                        MENTOR_BEHAVIOURAL_CONDUCT_RECORD AS MB
                                        INNER JOIN 
                                        SUP_BEHAVIOUR_LEVEL AS SL ON SL.LEVEL_ID=MB.LEVEL_ID
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID
                                        AND MB.IS_DELETED != 1
                                    ORDER BY
                                        INCIDENT_DATE DESC,
                                        INCIDENT_TIME DESC;
                                    ";
                        break;
                    }
                case MentorSQLCommend.FetchBehaviourById:
                    {
                        query = @"SELECT
                                            RECORD_ID,
                                            STUDENT_ID,
                                            LEVEL_ID,
                                            INCIDENT_DATE,
                                            INCIDENT_TIME,
                                            INCIDENT_DETAILS,
                                            ACTION_TAKEN,
                                            REMARKS
                                        FROM
                                            MENTOR_BEHAVIOURAL_CONDUCT_RECORD
                                        WHERE
                                            RECORD_ID = ?RECORD_ID
                                            AND IS_DELETED != 1;

                                    ";
                        break;
                    }
                case MentorSQLCommend.InsertBehaviourRecord:
                    {
                        query = @"INSERT INTO MENTOR_BEHAVIOURAL_CONDUCT_RECORD
                                            (
                                                STUDENT_ID,
                                                LEVEL_ID,
                                                INCIDENT_DATE,
                                                INCIDENT_TIME,
                                                INCIDENT_DETAILS,
                                                ACTION_TAKEN,
                                                REMARKS,
                                                IS_DELETED,
                                                IS_ACTIVE,
                                                CREATED_ON
                                            )
                                            VALUES
                                            (
                                                ?STUDENT_ID,
                                                ?LEVEL_ID,
                                                ?INCIDENT_DATE,
                                                ?INCIDENT_TIME,
                                                ?INCIDENT_DETAILS,
                                                ?ACTION_TAKEN,
                                                ?REMARKS,
                                                0,
                                                1,
                                                NOW() );  ";
                                  
                        break;
                    }
                case MentorSQLCommend.UpdateBehaviourRecord:
                    {
                        query = @"UPDATE MENTOR_BEHAVIOURAL_CONDUCT_RECORD
                                SET
                                  LEVEL_ID          = ?LEVEL_ID,
                                  INCIDENT_DATE     = ?INCIDENT_DATE,
                                  INCIDENT_TIME     = ?INCIDENT_TIME,
                                  INCIDENT_DETAILS  = ?INCIDENT_DETAILS,
                                  ACTION_TAKEN      = ?ACTION_TAKEN,
                                  REMARKS           = ?REMARKS,
                                  UPDATED_ON        = NOW()
                              WHERE
                                  RECORD_ID = ?RECORD_ID";
                        break;
                    }



                case MentorSQLCommend.FetchSportsDetails:
                    {
                        query = @"SELECT 
                                                SPORTS_ID,
                                                STUDENT_ID,
                                                SS.SEMESTER_NAME,
                                                CATEGORY,
                                                ACTIVITY,
                                                MS.IS_ACTIVE,
                                                MS.IS_DELETED
                                            FROM
                                                MENTOR_SPORTS AS MS
                                                INNER JOIN
                                                SUP_SEMESTER AS SS ON SS.SUP_SEMESTER_ID=MS.SEMESTER_ID
                                                  WHERE 
                                                MS.IS_DELETED <> 1 and 
                                        STUDENT_ID = ?STUDENT_ID; ";
                                       
                        break;
                    }
                case MentorSQLCommend.Participation_Achievements:
                    {
                        query = @"SELECT 
                                                PARTICIPATION_ID,
                                                PARTICIPATION,
                                                INSTITUTION,
                                                DATE,
                                                PLACE,
                                                EVENT,
                                                LEVEL,
                                                PRIZE_ACHIEVEMENT,
                                                IS_ACTIVE,
                                                IS_DELETED
                                            FROM
                                                MENTOR_PARTICIPATION_ACHIEVEMENTS
                                                  WHERE 
                                        STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }

                case MentorSQLCommend.InsertParticipationAchements:
                    {
                        query = @"  INSERT INTO MENTOR_PARTICIPATION_ACHIEVEMENTS(
                                                PARTICIPATION_ID,
                                                PARTICIPATION,
                                                STUDENT_ID,
                                                INSTITUTION,
                                                DATE,
                                                PLACE,
                                                EVENT,
                                                LEVEL,
                                                PRIZE_ACHIEVEMENT,
                                                IS_ACTIVE,
                                                IS_DELETED
                                                )
                                                VALUES
                                                (
                                                ?PARTICIPATION_ID,
                                                ?PARTICIPATION,
                                                ?STUDENT_ID,
                                                ?INSTITUTION,
                                                ?DATE,
                                                ?PLACE,
                                                ?EVENT,
                                                ?LEVEL,
                                                ?PRIZE_ACHIEVEMENT,
                                                ?IS_ACTIVE,
                                                ?IS_DELETED);";

                        break;
                    }

                case MentorSQLCommend.UpdateParticipationAchements:
                    {
                        query = @"       UPDATE MENTOR_PARTICIPATION_ACHIEVEMENTS
                                            SET 
                                            
                                                PARTICIPATION = ?PARTICIPATION,
                                                STUDENT_ID = ?STUDENT_ID,
                                                INSTITUTION = ?INSTITUTION,
                                                DATE = ?DATE, 
                                                PLACE = ?PLACE
                                                EVENT = ?EVENT
                                                LEVEL = ?LEVEL
                                                PRIZE_ACHIEVEMENT = ?PRIZE_ACHIEVEMENT
                                                IS_ACTIVE = ?IS_ACTIVE
                                                IS_DELETED = ?IS_DELETED
                                            WHERE
                                                PARTICIPATION_ID = ?PARTICIPATION_ID;";

                        break;
                    }

                case MentorSQLCommend.FetchParentMeeting:
                    {
                        query = @" SELECT
                                        PM.PARENT_ID,
                                        PM.STUDENT_ID,
                                        PM.SEM,
                                        SS.SEMESTER_NAME,
                                        PM.FEEDBACK,
                                        DATE_FORMAT(PM.MEETING_DATE,'%d/%m/%Y') as MEETING_DATE,
                                        PM.MEETING_TIME,
                                        PM.IS_ACTIVE,
                                        PM.IS_DELETED
                                    FROM
                                        MENTOR_PARENT_MEETING PM
                                    LEFT JOIN 
                                        SUP_SEMESTER SS ON SS.SUP_SEMESTER_ID = PM.SEM
                                    WHERE
                                        PM.STUDENT_ID = ?STUDENT_ID
                                        AND PM.IS_DELETED != 1
                                    ORDER BY
                                        PM.SEM;";
                                    
                        break;
                    }

                case MentorSQLCommend.InsertParentMeeting:
                    {
                        query = @"  INSERT INTO MENTOR_PARENT_MEETING
                                        (
                                            STUDENT_ID,
                                            SEM,
                                            FEEDBACK,
                                            MEETING_DATE,
                                            MEETING_TIME,
                                            IS_ACTIVE,
                                            IS_DELETED
                                        )
                                        VALUES
                                        (
                                            ?STUDENT_ID,
                                            ?SEM,
                                            ?FEEDBACK,
                                            ?MEETING_DATE,
                                            ?MEETING_TIME,
                                            1,
                                            0
                                        );";

                        break;
                    }

                case MentorSQLCommend.UpdateParentMeeting:
                    {
                        query = @"     UPDATE MENTOR_PARENT_MEETING
                                            SET 
                                                SEM = ?SEM,
                                                STUDENT_ID=?STUDENT_ID,
                                                FEEDBACK = ?FEEDBACK,
                                                MEETING_DATE = ?MEETING_DATE,
                                                MEETING_TIME = ?MEETING_TIME,
                                                IS_ACTIVE = ?IS_ACTIVE
                                            WHERE
                                                PARENT_ID = ?PARENT_ID;";

                        break;
                    }

                case MentorSQLCommend.FetchParentMeetingById:
                    {
                        query = @" SELECT
                                        PARENT_ID,
                                        STUDENT_ID,
                                        SEM,
                                        FEEDBACK,
                                        MEETING_DATE,
                                        MEETING_TIME,
                                        IS_ACTIVE,
                                        IS_DELETED
                                    FROM
                                        MENTOR_PARENT_MEETING
                                    WHERE
                                        PARENT_ID = ?PARENT_ID
                                        AND IS_DELETED = 0;";
                        break;
                    }

                case MentorSQLCommend.DeleteParentMeeting:
                    {
                        query = @"UPDATE MENTOR_PARENT_MEETING
                                    SET
                                        IS_DELETED = 1
                                    WHERE
                                        PARENT_ID = ?PARENT_ID;";
                        break;
                    }

                case MentorSQLCommend.InsertAchievement:
                    {
                        query = @" INSERT INTO MENTOR_PARTICIPATION_ACHIEVEMENTS( 
                                                STUDENT_ID, 
                                                INSTITUTION,
                                                DATE, 
                                                PLACE, 
                                                EVENT, 
                                                LEVEL, 
                                                PRIZE_ACHIEVEMENT,
                                                IS_ACTIVE, 
                                                IS_DELETED)
                                                values
                                                ( 
                                                ?STUDENT_ID,
                                                ?INSTITUTION, 
                                                ?DATE, 
                                                ?PLACE,
                                                ?EVENT,
                                                ?LEVEL,
                                                ?PRIZE_ACHIEVEMENT,
                                                1, 
                                                0);";
                                                
                        break;
                    }

                case MentorSQLCommend.UpdateParticipation:
                    {
                        query = @"UPDATE MENTOR_PARTICIPATION_ACHIEVEMENTS 
                                            SET
                                                STUDENT_ID = ?STUDENT_ID,
                                                INSTITUTION = ?INSTITUTION,
                                                DATE = ?DATE,
                                                PLACE = ?PLACE,
                                                EVENT = ?EVENT,
                                                LEVEL = ?LEVEL,
                                                PRIZE_ACHIEVEMENT = ?PRIZE_ACHIEVEMENT,
                                                IS_ACTIVE = ?IS_ACTIVE
                                            WHERE PARTICIPATION_ID = ?PARTICIPATION_ID;";
                                            
                        break;
                    }
              



                case MentorSQLCommend.FetchLateEntry:
                    {
                        query = @"
                             SELECT 
                                    LE.LATE_FEE_ID,
                                    LE.STUDENT_ID,
                                    LE.ENTRY_DATE,
                                    LE.IS_ACTIVE,
                                    LE.IS_DELETED,
                                    SPI.ROLL_NO,
                                    SPI.FIRST_NAME,
                                    CC.CLASS_NAME ,
                                    CC.CLASS_CODE,
                                    SPI.STU_MOBILENO
                                FROM STU_LATE_ENTRY LE
                                INNER JOIN
                                STU_CLASS AS SC ON SC.STUDENT_ID=LE.STUDENT_ID
                                INNER JOIN
                                STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID=LE.STUDENT_ID
                                INNER JOIN 
                                CP_CLASSES AS CC ON CC.CLASS_ID=SC.CLASS_ID
                                WHERE LE.IS_DELETED != 1
                                AND LE.STUDENT_ID = ?STUDENT_ID
                                ORDER BY LE.ENTRY_DATE DESC;";
                        break;
                    }
                case MentorSQLCommend.InsertLateEntry:
                    {
                        query = @"INSERT INTO STU_LATE_ENTRY
                            (
                                STUDENT_ID,
                                ENTRY_DATE,
                                REASON,
                                IS_ACTIVE,
                                IS_DELETED
                            )
                            VALUES
                            (
                                ?STUDENT_ID,
                                ?ENTRY_DATE,
                                ?REASON,
                                1,
                                0
                            );";
                        break;
                    }

                case MentorSQLCommend.UpdateLateEntry:
                    {
                        query = @"
                        UPDATE STU_LATE_ENTRY
                        SET
                        ENTRY_DATE = ?ENTRY_DATE,
                        REASON = ?REASON,
                        IS_ACTIVE = ?IS_ACTIVE
                        WHERE LATE_ENTRY_ID = ?LATE_ENTRY_ID;";
                        break;
                    }

                
                case MentorSQLCommend.FetchGoalInLife:
                    {
                        query = @"SELECT 
                                    GOAL_ID,
                                    STUDENT_ID,
                                    GOAL_IN_LIFE,
                                    STRENGTH,
                                    WEAKNESS,
                                    ROLE_MODEL,
                                    IS_ACTIVE,
                                    IS_DELETED
                                  FROM MENTOR_GOAL_IN_LIFE
                                  WHERE STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }


                case MentorSQLCommend.InsertGoalInLife:
                    {
                        query = @"INSERT INTO mentor_goal_in_life
                                            (
                                                STUDENT_ID,
                                                GOAL_IN_LIFE,
                                                STRENGTH,
                                                WEAKNESS,
                                                ROLE_MODEL
                                            )
                                            VALUES
                                            (
                                                ?STUDENT_ID,
                                                ?GOAL_IN_LIFE,
                                                ?STRENGTH,
                                                ?WEAKNESS,
                                                ?ROLE_MODEL
                                            );

                                );";
                        break;
                    }


                case MentorSQLCommend.UpdateGoalInLife:
                    {
                        query = @"UPDATE MENTOR_GOAL_IN_LIFE
                                  SET
                                    GOAL_IN_LIFE = ?GOAL_IN_LIFE,
                                    STRENGTH = ?STRENGTH,
                                    WEAKNESS = ?WEAKNESS,
                                    ROLE_MODEL = ?ROLE_MODEL,
                                    IS_ACTIVE = ?IS_ACTIVE
                                  WHERE GOAL_ID = ?GOAL_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchOverallMentorAssessment:
                    {
                        query = @"SELECT 
                                    OVERALL_ASSESSMENT_ID,
                                    AREA,
                                    SEM1,
                                    SEM2,
                                    SEM3,
                                    SEM4,
                                    SEM5,
                                    SEM6
                                FROM
                                    mentor_overall_assessment
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND ACADEMIC_YEAR = ?ACADEMIC_YEAR;";
                        break;
                    }
                case MentorSQLCommend.FetchOverallNewMentorAssessment:
                    {
                        query = @"SELECT 
                                    OVERALL_ASSESSMENT_ID,
                                    SUP_OVERALL_MENTOR_ASSESSMENT_ID,
                                    SEM1,
                                    SEM2,
                                    SEM3,
                                    SEM4,
                                    SEM5,
                                    SEM6
                                FROM
                                    mentor_newoverall_assessment
                                WHERE
                                    STUDENT_ID = ?STUDENT_ID
                                        AND ACADEMIC_YEAR = ?ACADEMIC_YEAR;";
                        break;
                    }
                case MentorSQLCommend.FetchSupOverallMentorAssessmentCategory:
                    {
                        query = @"SELECT 
                                        SUP_OVERALL_MENTOR_ASSESSMENT_ID,
                                        ASSESSMENT_AREA,
                                        NO_OF_SEMESTER
                                    FROM
                                        SUP_OVERALL_MENTOR_ASSESSMENT_CATEGORY WHERE IS_DELETED !=1;";
                        break;
                    }
                case MentorSQLCommend.SaveOverallMentorAssessment:
                    {
                        query = @"INSERT INTO mentor_overall_assessment
                                (STUDENT_ID, AREA, SEM1, SEM2, SEM3, SEM4, SEM5, SEM6, ACADEMIC_YEAR, CREATED_BY)
                                VALUES
                                (?STUDENT_ID, ?AREA, ?SEM1, ?SEM2, ?SEM3, ?SEM4, ?SEM5, ?SEM6, ?ACADEMIC_YEAR, ?CREATED_BY);";
                        break;
                    }
                case MentorSQLCommend.SaveNewOverallMentorAssessment:
                    {
                        query = @"INSERT INTO mentor_newoverall_assessment
                                (STUDENT_ID, SUP_OVERALL_MENTOR_ASSESSMENT_ID, SEM1, SEM2, SEM3, SEM4, SEM5, SEM6, ACADEMIC_YEAR, CREATED_BY)
                                VALUES
                                (?STUDENT_ID, ?SUP_OVERALL_MENTOR_ASSESSMENT_ID, ?SEM1, ?SEM2, ?SEM3, ?SEM4, ?SEM5, ?SEM6, ?ACADEMIC_YEAR, ?CREATED_BY);";
                        break;
                    }
                case MentorSQLCommend.UpdateOverallMentorAssessment:
                    {
                        query = @"UPDATE mentor_overall_assessment
                                    SET
                                        SEM1 = ?SEM1,
                                        SEM2 = ?SEM2,
                                        SEM3 = ?SEM3,
                                        SEM4 = ?SEM4,
                                        SEM5 = ?SEM5,
                                        SEM6 = ?SEM6,
                                        CREATED_BY = ?CREATED_BY
                                    WHERE OVERALL_ASSESSMENT_ID = ?OVERALL_ASSESSMENT_ID;";
                        break;
                    }
                case MentorSQLCommend.UpdatenewOverallMentorAssessment:
                    {
                        query = @"UPDATE mentor_newoverall_assessment
                                    SET
                                        SEM1 = ?SEM1,
                                        SEM2 = ?SEM2,
                                        SEM3 = ?SEM3,
                                        SEM4 = ?SEM4,
                                        SEM5 = ?SEM5,
                                        SEM6 = ?SEM6,
                                        CREATED_BY = ?CREATED_BY
                                    WHERE OVERALL_ASSESSMENT_ID = ?OVERALL_ASSESSMENT_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchBehaviourLevels:
                    {
                        query = @"SELECT
                                    LEVEL_ID,
                                    LEVEL_CODE,
                                    LEVEL_NAME
                                FROM
                                    SUP_BEHAVIOUR_LEVEL
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED !=1
                                ORDER BY
                                    LEVEL_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchOverallMentorAssessmentObjective:
                    {
                        query = @"  SELECT 
                                        ASSESSMENT_OBJECTIVE_ID,
                                        SUP_OVERALL_MENTOR_ASSESSMENT_ID,
                                        OBJECTIVE_NAME
                                    FROM
                                        SUP_OVERALL_MENTOR_ASSESSMENT_OBJECTIVE
                                    WHERE
                                        IS_DELETED != 1;";
                        break;
                    }
                case MentorSQLCommend.FetchCounsellingSessions:
                    {
                        query = @"SELECT 
                                CS.SESSION_ID,
                                CS.STUDENT_ID,
                                CS.SEMESTER_ID,
                                SM.SEMESTER_NAME,
                                CS.SESSION_DATE,
                                CS.SESSION_TIME,
                                CS.MATTERS_DISCUSSED,
                                CS.GUIDANCE_GIVEN,
                                CS.FOLLOW_UP_REQUIRED,
                                CS.IS_ACTIVE,
                                CS.IS_DELETED
                            FROM MENTORING_COUNSELLING_SESSION CS
                            INNER JOIN SUP_SEMESTER SM 
                                ON SM.SUP_SEMESTER_ID = CS.SEMESTER_ID
                            WHERE CS.STUDENT_ID = ?STUDENT_ID
                              AND CS.IS_ACTIVE=1
                              AND CS.IS_DELETED = 0
                            ORDER BY SM.SUP_SEMESTER_ID ASC;";
                        break;
                    }
                case MentorSQLCommend.FetchCounsellingSessionById:
                    {
                        query = @"SELECT 
                                        SESSION_ID,
                                        STUDENT_ID,
                                        SEMESTER_ID,
                                        SESSION_DATE,
                                        SESSION_TIME,
                                        MATTERS_DISCUSSED,
                                        GUIDANCE_GIVEN,
                                        FOLLOW_UP_REQUIRED,
                                        IS_ACTIVE,
                                        IS_DELETED
                                        FROM MENTORING_COUNSELLING_SESSION
                                        WHERE SESSION_ID = ?SESSION_ID
                                          AND IS_ACTIVE=1
                                          AND IS_DELETED = 0
                                        ORDER BY SESSION_DATE DESC;";
                        break;
                    }
                case MentorSQLCommend.InsertCounsellingSession:
                    {
                        query = @"INSERT INTO MENTORING_COUNSELLING_SESSION
                                (
                                 STUDENT_ID,
                                 SEMESTER_ID, 
                                 SESSION_DATE, 
                                 SESSION_TIME,
                                 MATTERS_DISCUSSED,
                                 GUIDANCE_GIVEN, 
                                 FOLLOW_UP_REQUIRED,
                                 IS_ACTIVE
                                 )
                                VALUES
                                (
                                 ?STUDENT_ID,
                                 ?SEMESTER_ID, 
                                 ?SESSION_DATE,
                                 ?SESSION_TIME,
                                 ?MATTERS_DISCUSSED, 
                                 ?GUIDANCE_GIVEN, 
                                 ?FOLLOW_UP_REQUIRED,
                                 1
                                );";
                        break;
                    }
                case MentorSQLCommend.UpdateCounsellingSession:
                    {
                        query = @"UPDATE MENTORING_COUNSELLING_SESSION
                                SET
                                    SEMESTER_ID = ?SEMESTER_ID,
                                    SESSION_DATE = ?SESSION_DATE,
                                    SESSION_TIME = ?SESSION_TIME,
                                    MATTERS_DISCUSSED = ?MATTERS_DISCUSSED,
                                    GUIDANCE_GIVEN = ?GUIDANCE_GIVEN,
                                    FOLLOW_UP_REQUIRED = ?FOLLOW_UP_REQUIRED
                                WHERE SESSION_ID = ?SESSION_ID;";
                        break;
                    }
                case MentorSQLCommend.DeleteCounsellingSession:
                    {
                        query = @"UPDATE MENTORING_COUNSELLING_SESSION
                                                SET IS_DELETED = 1
                                                WHERE SESSION_ID = ?SESSION_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchSemesters:
                    {
                        query = @"SELECT 
                                 SEMESTER_TYPE,
                                 SUP_SEMESTER_ID,
                                 SEMESTER_NAME 
                                 FROM 
                                 SUP_SEMESTER 
                                 WHERE IS_DELETED != 1;";
                        break;
                    }
                case MentorSQLCommend.FetchValueandclass:
                    {
                        query = @"SELECT 
                                    SC.STUDENT_ID,
                                    SC.ACADEMIC_YEAR,
                                    SC.CLASS_ID,
                                    PG.PROGRAMME_ID AS PROGRAM_ID
                                FROM
                                    STU_CLASS SC
                                        INNER JOIN
                                    CP_PROGRAMME_GROUP PG ON PG.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                WHERE
                                    SC.STUDENT_ID = ?STUDENT_ID
                                        AND SC.ACADEMIC_YEAR = ?ACADEMIC_YEAR;";
                        break;
                    }

                case MentorSQLCommend.FetchMentorstudnetabsentReport:
                    {
                        query = @"SELECT
                                    ATT.COURSE_ID,
                                    ATT.STUDENT_ID,
                                    ATT.CLASS_ID,
                                    STUP.ROLL_NO,
                                    STUP.FIRST_NAME,
                                    STUP.STU_MOBILENO AS MOBILE,
                                    cpc.CLASS_NAME,
                                    cpr.COURSE_TITLE,
                                    ATT.HOUR_FROM,
                                    ATT.HOUR_TO,
                                    ATT.ATTENDENCE_DATE
                                FROM 
                                    att_absentee_list AS ATT
                                    INNER JOIN cp_classes as cpc on cpc.CLASS_ID=ATT.CLASS_ID AND cpc.IS_DELETED!=1
                                    INNER JOIN cp_course_root as cpr on cpr.COURSE_ROOT_ID=ATT.COURSE_ID AND cpc.IS_DELETED!=1
                                    INNER JOIN stu_personal_info AS STUP ON STUP.STUDENT_ID=ATT.STUDENT_ID
                                    WHERE 
                                    ATT.STUDENT_ID=?STUDENT_ID
                                    AND cpr.SEMESTER_ID=?SEMESTER_ID
                                    AND ATT.IS_DELETED!=1 order by cpr.COURSE_ROOT_ID;";
                        break;
                    }
                case MentorSQLCommend.FetchSemesterDatesById:
                    {
                        query = @"SELECT 
                                    ASM.DATE_FROM, 
                                    DATE_FORMAT(ASM.DATE_TO, '%d/%m/%Y') AS DATE_TO,
                                    (SELECT COUNT(CAL.DAY_ORDER_DATE) 
                                     FROM CALENDER AS CAL 
                                     WHERE CAL.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR 
                                       AND CAL.DAY_ORDER_DATE BETWEEN ASM.DATE_FROM AND LEAST(ASM.DATE_TO, CURDATE()) 
                                       AND CAL.IS_HOLIDAY != 1) AS DAYS_COUNT
                                  FROM 
                                    ACADEMIC_SEMESTER AS ASM
                                    INNER JOIN STU_CLASS AS SC ON SC.PROGRAMME_GROUP_ID = ASM.PROGRAMME_GROUP_ID 
                                       AND SC.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR 
                                       AND SC.BATCH = ASM.BATCH
                                  WHERE 
                                    SC.STUDENT_ID = ?STUDENT_ID 
                                    AND ASM.SEMESTER = ?SEMESTER";
                        break;
                    }
                case MentorSQLCommend.FetchActiveSemester:
                    {
                        query = @"  SELECT ASM.SEMESTER FROM ACADEMIC_SEMESTER ASM 
                                    INNER JOIN STU_CLASS SC ON SC.PROGRAMME_GROUP_ID = ASM.PROGRAMME_GROUP_ID 
                                    AND SC.ACADEMIC_YEAR = ASM.ACADEMIC_YEAR 
                                    AND SC.BATCH = ASM.BATCH
                                    WHERE SC.STUDENT_ID = ?STUDENT_ID 
                                    AND SC.ACADEMIC_YEAR = ?AC_YEAR 
                                    AND ASM.IS_ACTIVE = 1;";
                        break;
                    }

                case MentorSQLCommend.FetchSports:
                    {
                        query = @"SELECT  *
   
                            FROM
                                mentor_sports
                            WHERE
                                STUDENT_ID = ?STUDENT_ID
                                    AND IFNULL(IS_DELETED, '0') = '0'";
                        break;
                    }

                case MentorSQLCommend.FetchSportById:
                    {
                        query = @"SELECT * 
                                FROM mentor_sports 
                                WHERE SPORTS_ID = ?SPORTS_ID";

                        break;
                    }

                case MentorSQLCommend.InsertSports:
                    {
                        query = @"INSERT INTO mentor_sports
                            (
                            STUDENT_ID, 
                            SEMESTER_ID, 
                            CATEGORY,
                            ACTIVITY,
                            IS_ACTIVE)
                            VALUES
                            (
                            ?STUDENT_ID, 
                            ?SEMESTER_ID,
                            ?CATEGORY,
                            ?ACTIVITY,
                            '1')";
                        break;
                    }

                case MentorSQLCommend.UpdateSports:
                    {

                        query = @"UPDATE 
                        mentor_sports SET
                        SEMESTER_ID=?SEMESTER_ID,
                        CATEGORY=?CATEGORY,
                         ACTIVITY=?ACTIVITY
                        WHERE SPORTS_ID=?SPORTS_ID";
                        break;
                    }

                case MentorSQLCommend.DeleteSports:
                    {
                        query = @"UPDATE
                            mentor_sports SET IS_DELETED=1
                                WHERE SPORTS_ID=?SPORTS_ID";
                        break;
                    }

                case MentorSQLCommend.FetchLateEntryStudentDateWise:
                    {
                        query = @"SELECT 
                                        SPI.ROLL_NO,
                                        CONCAT(IFNULL(SPI.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(SPI.LAST_NAME, '')) AS FIRST_NAME,
                                        STF.STAFF_CODE,
                                        CONCAT(IFNULL(STF.FIRST_NAME, ''),
                                                '  ',
                                                IFNULL(STF.LAST_NAME, '')) AS 'STAFF_NAME',
                                        DATE_FORMAT(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s') ENTRY_DATE,
                                        FINE_AMOUNT,
                                        CC.CLASS_NAME,
                                        CC.CLASS_CODE,
                                        SS.SHIFT_NAME,
                                        SPI.FR_MOBILE,
                                        SPI.STU_MOBILENO
                                    FROM
                                        STU_LATE_ENTRY AS SL
                                            INNER JOIN
                                        STU_PERSONAL_INFO AS SPI ON SPI.STUDENT_ID = SL.STUDENT_ID
                                            AND SPI.IS_DELETED != 1
                                            AND SPI.IS_LEFT != 1
                                            left JOIN
                                        STF_PERSONAL_INFO AS STF ON STF.STAFF_ID = SL.ENTRY_ID
                                            AND STF.IS_DELETED != 1
                                            INNER JOIN
                                        stu_class AS SC ON SC.STUDENT_ID = SPI.STUDENT_ID
                                            AND SC.ACADEMIC_YEAR = ?AC_YEAR
                                            AND SC.IS_DELETED != 1
                                            INNER JOIN
                                        CP_CLASSES AS CC ON CC.CLASS_ID = SC.CLASS_ID
                                            INNER JOIN
                                        sup_shift AS SS ON SS.SHIFT_ID = CC.SHIFT
                                    WHERE
                                    
                                        SL.IS_DELETED != 1 AND STUDENT_ID = ?STUDENT_ID
                                            AND DATE_FORMAT(SL.ENTRY_DATE, '%Y-%m-%d') BETWEEN '?FROM_DATE' AND '?TODATE'
                                            AND SL.ACADEMIC_YEAR = ?AC_YEAR
                                      
                                    ORDER BY CC.CLASS_ID , SPI.ROLL_NO ,STR_TO_DATE(SL.ENTRY_DATE, '%d/%m/%Y %H:%i:%s');";
                        break;
                    }


                case MentorSQLCommend.FetchMentorstudentabsentReport:
                    {
                                     query = @" SELECT 
                                                                    ATT.STUDENT_ID,
                                                                    ATT.CLASS_ID,
                                                                    SPI.ROLL_NO,
                                                                    SPI.FIRST_NAME,
                                                                    SPI.STU_MOBILENO AS MOBILE,
                                                                    DATE_FORMAT(ATT.ATTENDENCE_DATE, '%d-%m-%Y') AS ATTENDENCE_DATE
                                                                FROM
                                                                    att_common_absentee_list AS ATT
                                                                        INNER JOIN
                                                                    stu_personal_info AS SPI ON SPI.STUDENT_ID = ATT.STUDENT_ID
                                                                        INNER JOIN
                                                                    stu_class AS SC ON SC.STUDENT_ID = ATT.STUDENT_ID
                                                                        INNER JOIN
                                                                    academic_semester AS SE ON SE.PROGRAMME_GROUP_ID = SC.PROGRAMME_GROUP_ID
                                                                        AND sc.ACADEMIC_YEAR = se.ACADEMIC_YEAR
                                                                        AND ATT.ACADEMIC_YEAR = se.ACADEMIC_YEAR
                                                                        INNER JOIN
                                                                    sup_semester AS SS ON ss.SUP_SEMESTER_ID = se.SEMESTER
                                                                        AND SS.SEMESTER_TYPE = ATT.SEMESTER_TYPE
                                                                WHERE
                                                                    ATT.STUDENT_ID = ?STUDENT_ID
                                                                        AND SS.SUP_SEMESTER_ID = ?SUP_SEMESTER_ID
                                                                        AND att.IS_DELETED != 1
                                                                        AND SC.IS_DELETED != 1
                                                                        AND SPI.IS_LEFT != 1
                                                                GROUP BY COMMON_ATTENDANCE_ID;";
                        break;
                    }



                case MentorSQLCommend.FetchOverallRecords:
                    {
                        query = @" SELECT
                                        OVERALL_ASSESSMENT_ID,
                                        STUDENT_ID,
                                        AREA,
                                        SEM1,
                                        SEM2,
                                        SEM3,
                                        SEM4,
                                        SEM5,
                                        SEM6,
                                        ACADEMIC_YEAR,
                                        CREATED_BY,
                                        CREATED_ON
                                    FROM
                                        mentor_overall_assessment AS MB
                                        INNER JOIN 
                                        sup_Academic_Assessment_objective AS SL ON SL.ACADEMIC_ASSESSMENT_ID=MB.OVERALL_ASSESSMENT_ID
                                    WHERE
                                        STUDENT_ID = ?STUDENT_ID
                                        AND MB.IS_DELETED != 1;
                                    ";
                        break;
                    }

                case MentorSQLCommend.FetchOverallAssessment:
                    {
                        query = @"   SELECT 
                                    ACADEMIC_ASSESSMENT_ID, TYPE_NAME
                                FROM
                                    SUP_ACADEMIC_ASSESSMENT_OBJECTIVE
                                WHERE
                                    IS_ACTIVE = 1 AND IS_DELETED != 1
                                ORDER BY ACADEMIC_ASSESSMENT_ID;";
                        break;
                    }

                case MentorSQLCommend.FetchCommunity:
                    {
                        query = @"SELECT 
                                                COMMUNITYID,
                                                COMMUNITY
                                            FROM
                                                SUP_COMMUNITY;";
                        break;
                    }


            }
            return query;
        }
    }
}



