using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CMS.ViewModel.Model
{
    public class MentoringModel
    {
        public string MENTORS_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
    }
    public class MentoringVisit
    {
        public string MENTORS_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string NATURE_OF_VISIT { get; set; }
        public string INITIAL_CONTACT_VISIT { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string CAMPUS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATEDAT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class MENTORING_VISITED_STUDENT
    {
        public string FIRST_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string ROLL_NO { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string MENTORS_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string NATURE_OF_VISIT { get; set; }
        public string INITIAL_CONTACT_VISIT { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCLUSION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_YEAR { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string PHOTO { get; set; }
        public string DEPT_ID { get; set; }
    }
    public class Stu_list
    {
        public string STUDENT_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string ADMISSION_NO { get; set; }
        public string COMMUNITY { get; set; }
        public string FIRST_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string BATCH { get; set; }
        public string MENTORINT_ID { get; set; }

    }
    public class MENTORING_VISITED_DETAILS
    {
        public string MENTORS_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string NATURE_OF_VISIT { get; set; }
        public string INITIAL_CONTACT_VISIT { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string USER_ID { get; set; }
        public string CONCLUSION_ARRIVED_AT { get; set; }
    }
    public class VisitingDetails
    {
        public string MENTORS_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string NATURE_OF_VISIT { get; set; }
        public string INITIAL_CONTACT_VISIT { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SHORT_REVIEW { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public string CONCLUSION_ARRIVED_AT { get; set; }


    }

    public class RegularVisitDetails
    {
        public string MENTORS_REGULAR_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string REGULAR_VISIT { get; set; }
        public string SHORT_REVIEW { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string CONCLUSION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string CAMPUS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_PENDING { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class MENTORS_REGULAR_VISIT
    {
        public string MENTORS_REGULAR_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string NATURE_OF_VISIT { get; set; }
        public string INITIAL_CONTACT_VISIT { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string ROLL_NO { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string REGULAR_VISIT { get; set; }
        public string SHORT_REVIEW { get; set; }
        public string IS_PENDING { get; set; }
        public string CONCLUSION_ARRIVED_AT { get; set; }
    }
    public class MENTOR_GROUP_SESSION_RECORDS
    {
        public string MENTOR_GROUP_VISIT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        public string VISIT_DURATION { get; set; }
        public string SHORT_REVIEW { get; set; }
        public string MATTER_DISCUSSED { get; set; }
        public string CONCULATION_ARRIVED_AT { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string GENERAL_IMPRESSION { get; set; }
        public string SELF_ASSESSMENT { get; set; }
        public string DATE_OF_NEXT_VISIT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string NATURE_OF_VISIT { get; set; }
        public string INITIAL_CONTACT_VISIT { get; set; }
        public string CONCLUSION_ARRIVED_AT { get; set; }


    }

    public class STU_personal_info
    {
        public string ADMISSION_NO { get; set; }
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_DESCRIPTION { get; set; }
        public string PERCENTAGE { get; set; }
        public double ACTUAL_PERCENTAGE { get; set; }
        public string TOTAL_HOURS { get; set; }
        public string ABSENT_HOURS { get; set; }
        public string SHIFT_NAME { get; set; }
        public string SHIFT_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string RECORDSCOUNT { get; set; }
        public string CLASS { get; set; }
        public string USER_ID { get; set; }
        public string MOBILE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string STATUS { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string STAFF_CODE { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string RFID { get; set; }
        public string BIOD_NUMBER { get; set; }
        public string CLASS_NAME { get; set; }
        public string RECEIPT_NO { get; set; }
        public string RECEIPT_NO1 { get; set; }
        public string RECEIPT_NO2 { get; set; }
        public string FR_MOBILE { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string RELIGION { get; set; }
        public string COMMUNITY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PROGRAM_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string GENDER_NAME { get; set; }
        public string DEPT_ID { get; set; }
        public string GENDER { get; set; }
        public string STU_EMAILID { get; set; }
        public string ADDRESS { get; set; }
        public string STU_MOBILENO { get; set; }
        public string FATHER_NAME { get; set; }
        public string FR_OCCUPATION { get; set; }
        public string FATHER_EDUCATION { get; set; }
        public string MOTHER_NAME { get; set; }
        public string MO_OCCUPATION { get; set; }
        public string MOTHER_EDUCATION { get; set; }
        public string ANNUAL_INCOME { get; set; }
        public string FR_INCOME { get; set; }
        public string MO_INCOME { get; set; }
        public string GUARDIAN_NAME { get; set; }
        public string GUARDIAN_INCOME { get; set; }
        public string GUARDIAN_OCCUPATION { get; set; }
        public string GUARDIAN_EDUCATION { get; set; }
        public string MENTORS_FAMILY_ID { get; set; }
        public string RELIGIONID { get; set; }
        public string AC_YEAR { get; set; }
        public string MOTHER_INCOME { get; set; }
        public string STAFF_ID { get; set; }
        public string ADDRESS_LINE1 { get; set; }
        public string CASTE { get; set; }
        public string SIBLINGS { get; set; }
        public string SIBLING { get; set; }
        public string AADHAR_NUMBER { get; set; }
        public string EMIS_NO { get; set; }
        public string ABC_ID { get; set; }
        public string UMIS_NO { get; set; }
        public string MODE_OF_TRAVELLING { get; set; }
        
        public string UMIS { get; set; }
        public string EMIS { get; set; }
        public string MENTOR_STUDENT_ID { get; set; }
        public string TRAVELLING { get; set; }



    }
    public class Student_Health_Details
    {
        public string STUDENT_HEALTH_ID { get; set; }
        public string ANY_DISABILITY { get; set; }
        public string ASTHMA { get; set; }
        public string ALLERGY { get; set; }
        public string DIABETES { get; set; }
        public string HEART_DISEASE { get; set; }
        public string WHEEZING { get; set; }
        public string SKIN_DISEASE { get; set; }
        public string HYSTERIA { get; set; }
        public string DISABILITY_TYPE { get; set; }
        public string HOSPITALIZED_FOR_ANY { get; set; }
        public string HOSPITALIZED_REASON { get; set; }
        public string ANYONE_SERIOUSLY_ILL { get; set; }
        public string ANY_ALLERGY { get; set; }
        public string ALLERGY_REASON { get; set; }
        public string COUNT { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PROGRAM_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class StudentDetails
    {
        public string MENTOR_STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }
        public string STU_MOBILENO { get; set; }
        public string STU_EMAILID { get; set; }
        public string RELIGION { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string STUDENT_ID { get; set; }
        public string DEPT_ID { get; set; }
        public string GENDER { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string GENDER_NAME { get; set; }
        public string RELIGIONID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SHIFT_ID { get; set; }
        public string FATHER_NAME { get; set; }
        public string FR_OCCUPATION { get; set; }
        public string MOTHER_NAME { get; set; }
        public string MO_OCCUPATION { get; set; }
        public string GUARDIAN_NAME { get; set; }
        public string GUARDIAN_OCCUPATION { get; set; }
        public string ANNUAL_INCOME { get; set; }
        public string CASTE { get; set; }
        public string COMMUNITY { get; set; }
        public string SIBLINGS { get; set; }
        public string UMIS { get; set; }
        public string EMIS { get; set; }
        public string TRAVELLING { get; set; }
        public string FR_MOBILE { get; set; }
        public string AADHAR_NUMBER { get; set; }

    }
    public class STUDENT_FAMILY_DETAILS
    {
        public string STUDENT_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string FATHER_NAME { get; set; }
        public string FR_OCCUPATION { get; set; }
        public string FATHER_EDUCATION { get; set; }
        public string MOTHER_NAME { get; set; }
        public string MO_OCCUPATION { get; set; }
        public string MOTHER_EDUCATION { get; set; }
        public string FR_INCOME { get; set; }
        public string MO_INCOME { get; set; }
        public string CLASS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MOTHER_INCOME { get; set; }
        public string GUARDIAN_NAME { get; set; }
        public string GUARDIAN_EDUCATION { get; set; }
        public string GUARDIAN_OCCUPATION { get; set; }
        public string GUARDIAN_INCOME { get; set; }
        public string FIRST_NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string GENDER { get; set; }
        public string DEPT_ID { get; set; }
        public string STU_MOBILENO { get; set; }
        public string STU_EMAILID { get; set; }
        public string RELIGION { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_NAME { get; set; }
        public string GENDER_NAME { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string MENTORS_FAMILY_ID { get; set; }
        public string FR_NAME { get; set; }
        public string FR_EDUCATION { get; set; }
        public string MOTHER_OCCUPATION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FR_MOBILE { get; set; }
        public string ANNUAL_INCOME { get; set; }
        public string FAMILY_TYPE { get; set; }
        public string NO_OFBROTHERS { get; set; }
        public string NO_OFSISTERS { get; set; }
        public string CREATED_AT { get; set; }
    }
    public class MENTORING_ACTIVITY_RESPONSE
    {
        public string STUDENT_GUIDANCE_ID { get; set; }
        public string INTERVENTION_AND_GUIDANCE { get; set; }
        public string VISIT_DATE { get; set; }
        //public string GOAL_IN_LIFE { get; set; }
        public string STRENGTH { get; set; }
        public string WEAKNESS { get; set; }
        public string ROLE_MODEL { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PROGRAM_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string STUDENT_ACTIVITY_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string AADHAR { get; set; }
    }
  
    public class MENTORING_HEALTH_DETAILS
    {
        public string STUDENT_HEALTH_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ANY_DISABILITY { get; set; }
        public string ASTHMA { get; set; }
        public string ALLERGY { get; set; }
        public string DIABETES { get; set; }
        public string HEART_DISEASE { get; set; }
        public string WHEEZING { get; set; }
        public string SKIN_DISEASE { get; set; }
        public string HYSTERIA { get; set; }
        public string DISABILITY_TYPE { get; set; }
        public string HOSPITALIZED_FOR_ANY { get; set; }
        public string HOSPITALIZED_REASON { get; set; }
        public string ANYONE_SERIOUSLY_ILL { get; set; }
        public string ANY_ALLERGY { get; set; }
        public string ALLERGY_REASON { get; set; }
        public string DISABILITY_PERCENTAGE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class MENTORING_STUDENT_ACADEMIC_DETAILS
    {
        public string STU_ACADEMIC_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SSLC_PERCENTAGE { get; set; }
        public string SSLC_GROUP { get; set; }
        public string SSLC_SCHOOL { get; set; }
        public string HSC_PERCENTAGE { get; set; }
        public string HSC_GROUP { get; set; }
        public string HSC_SCHOOL { get; set; }
        public string UG_YEAR1_PERCENTAGE { get; set; }
        public string UG_YEAR2_PERCENTAGE { get; set; }
        public string UG_YEAR3_PERCENTAGE { get; set; }
        public string UG_PROGRAMME { get; set; }
        public string EXTRA_QUALIFICATION { get; set; }
        public string EXTRA_QUALIFICATION_PERCENTAGE { get; set; }
        public string ACHIEVEMENTS { get; set; }
        public string HOBBIES { get; set; }
        public string ADDITIONAL_SKILLS { get; set; }
        public string STUDENT_ACTIVITY_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ANALYSIS_ID { get; set; }
        public string SEMESTER { get; set; }
        public string TOTAL_SUBJECTS { get; set; }
        public string SUBJECTS_PASSED { get; set; }
        public string ARREARS { get; set; }
        public string PERCENTAGE { get; set; }
    }
    
    public class MENTORING_STUDENT_ACADEMIC_ANALYSIS
    {
        public string ANALYSIS_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER { get; set; }
        public string TOTAL_SUBJECTS { get; set; }
        public string SUBJECTS_PASSED { get; set; }
        public string ARREARS { get; set; }
        public string PERCENTAGE { get; set; }
    }

    public class LanguageDetailsViewModel
    {
        public string LANGUAGE_ID { get; set; }
        public string READ_ID { get; set; }
        public string WRITE_ID { get; set; }
        public string SPEAK_ID { get; set; }
        public string LANGUAGE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string Languages { get; set; }
    }


    public class MentroingFamilyDetails
    {
        public string NO_OFSISTERS { get; set; }
        public string NO_OFBROTHERS { get; set; }
        public string GUARDIAN_INCOME { get; set; }
        public string GUARDIAN_OCCUPATION { get; set; }
        public string GUARDIAN_EDUCATION { get; set; }
        public string GUARDIAN_NAME { get; set; }
        public string MOTHER_INCOME { get; set; }
        public string MO_OCCUPATION { get; set; }
        public string MOTHER_EDUCATION { get; set; }
        public string MOTHER_NAME { get; set; }
        public string FR_INCOME { get; set; }
        public string FR_OCCUPATION { get; set; }
        public string FATHER_EDUCATION { get; set; }
        public string FATHER_NAME { get; set; }
        public string FAMILY_TYPE { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FR_NAME { get; set; }
        public string FR_EDUCATION { get; set; }
        public string MOTHER_OCCUPATION { get; set; }
        public string MENTORS_FAMILY_ID { get; set; }
    }

    public class INOVATION
    {
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string INTERVENTION_AND_GUIDANCE { get; set; }
        public string VISIT_DATE { get; set; }
        public string STUDENT_GUIDANCE_ID { get; set; }
    }

    public class STF_INFO
    {
        public string STAFF_ID { get; set; }
        public string DEPT_ID { get; set; }
        public string APPROVER_ROLE_ID { get; set; }
        public string STF_LEAVE_SETTING_ID { get; set; }
        public string LEAVE_SETTING_ID { get; set; }
        public string APPROVALER_ID2 { get; set; }
        public string APPROVALER_ID1 { get; set; }
        public string APPROVAL_LEVEL { get; set; }
        public string USER_CODE { get; set; }
        public string STAFF_CODE { get; set; }
        public string STAFF_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string GENDER { get; set; }
        public string KNOWN_AS { get; set; }
        public string PLACE_OF_BIRTH { get; set; }
        public string IDENTIFICATION_MARK1 { get; set; }
        public string IDENTIFICATION_MARK2 { get; set; }
        public string HEIGHT { get; set; }
        public string COMMUNITY { get; set; }
        public string NATIONALITY { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string CATEGORY { get; set; }
        public string MAIN_SUBJECT { get; set; }
        public string ANNUATION_DATE { get; set; }
        public string LEAVING_DATE { get; set; }
        public string LEAVING_REMARKS { get; set; }
        public string USER_FIELD1 { get; set; }
        public string USER_FIELD2 { get; set; }
        public string SESSIONS { get; set; }
        public string DATE_OF_JOIN { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string RELIGION { get; set; }
        public string MOTHER_TONGUE { get; set; }
        public string PHYSICAL_STATUS { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string RETIREMENT_DATE { get; set; }
        public string LEAVING_REASON { get; set; }
        public string PHOTO { get; set; }
        public string SERVICE_REGNO { get; set; }
        public string IS_DELETED { get; set; }
        public string KNOWLEDGE_IN_COMPUTER { get; set; }
        public string CERTIFICATE1 { get; set; }
        public string CERTIFICATE2 { get; set; }
        public string CERTIFICATE3 { get; set; }
        public string QUALIFICATION_DOC1 { get; set; }
        public string QUALIFICATION_DOC2 { get; set; }
        public string QUALIFICATION_DOC3 { get; set; }
        public string QUALIFICATION { get; set; }
        public string NON_TEACHING_CATEGORY { get; set; }
        public string VISA_NUMBER { get; set; }
        public string STAFF_REGISTERED_DATE { get; set; }
        public string STAFF_ORDER { get; set; }
        public string ROLES { get; set; }
        public string PROLE_ID { get; set; }
        public string PREFIX { get; set; }
        public string SHIFT { get; set; }
        public string ATT_SHIFT { get; set; }
        public string SUB_CATEGORY { get; set; }
        public string BOARD_MEMBER { get; set; }
        public string IS_LEFT { get; set; }
        public string IS_ONLINE { get; set; }
        public string BOID_NUMBER { get; set; }
        public string RFID { get; set; }
        public string USER_ACCOUNT_ID { get; set; }
        public string PHONE { get; set; }
        public string SUPERVISION_TYPE { get; set; }
        public string EXP_YEAR_FROM { get; set; }
        public string EXP_YEAR_TO { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string LEAVE_TYPE { get; set; }
        public string LEAVE_APPLY_ID { get; set; }
        public string FROM_SESSION { get; set; }
        public string TO_SESSION { get; set; }
        public string DESCRIPTION { get; set; }
        public string NO_OF_DAYS { get; set; }
        public string STAFF_MODE { get; set; }
        public string ROLE { get; set; }
        public string MONTH_NAME { get; set; }
        public string MENTORINT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string BATCH { get; set; }

    }

    public class STAFF_QUOTA_DETAILS
    {
        public string STAFF_ID { get; set; }
        public string UG1 { get; set; }
        public string UG2 { get; set; }
        public string UG3 { get; set; }
        public string PG1 { get; set; }
        public string PG2 { get; set; }
        public string TOTAL { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string QUOTA_ID { get; set; }
    }
    public class SUP_CLASS_YEAR
    {

        public string CLASS_YEAR_ID { get; set; }
        public string CLASS_YEAR { get; set; }

    }
    public class ResultID
    {
        public string MENTOR_LANGUAGE_ID { get; set; }
    }
    public class PERSONAL_DETAILS
    {
        public string CLASS_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SHIFT_LIST { get; set; }
        public string STUDENT_ID { get; set; }
        public string PROGRAM_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string VISIT_DATE { get; set; }
        public string INTERVENTION_AND_GUIDANCE { get; set; }


    }
    public class SKILLS_AND_TALENTS
    {

        public string SKILLS_AND_TALENTS_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string MENTOR_ID { get; set; }
        public string SKILLS { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        


    }


    public class MENTOR_SPORTS
    {
        public string SPORTS_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string CATEGORY { get; set; }
        public string ACTIVITY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_NAME { get; set; }
        
    }




    public class PARTICIPATION_ACHIEVEMENTS
    {
        public string PARTICIPATION_ID { get; set; }
        public string PARTICIPATION { get; set; }
        public string INSTITUTION { get; set; }
        public string DATE { get; set; }
        public string PLACE { get; set; }
        public string EVENT { get; set; }
        public string LEVEL { get; set; }
        public string PRIZE_ACHIEVEMENT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STUDENT_ID { get; set; }
    }

    public class MENTOR_VAC_PROFESSIONAL
    {
        public string VAC_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER { get; set; }
        public string COURSE_NAME { get; set; }
        public string DURATION { get; set; }
        public string INSTITUTION_PLATFORM { get; set; }
        public string OUTCOME_CERTIFICATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class MENTOR_LEADERSHIP_CONTRIBUTION
    {
        public string CONTRIBUTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CONTRIBUTION_TYPE_ID { get; set; }     
        public string CONTRIBUTION_TYPE_NAME { get; set; }   
        public string EVENT_DATE { get; set; }
        public string EVENT_NAME { get; set; }
        public string PLACE { get; set; }
        public string NATURE_OF_CONTRIBUTION { get; set; }
        public string REMARKS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_CONTRIBUTION_TYPE
{
    public string CONTRIBUTION_TYPE_ID { get; set; }
    public string TYPE_NAME { get; set; }
    public string IS_ACTIVE { get; set; }
    public string IS_DELETED { get; set; }
}

    public class BEHAVIOURAL_CONDUCT_RECORD
    {
        public string RECORD_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string LEVEL_ID { get; set; }
        public string LEVEL_NAME { get; set; }
        public string INCIDENT_DATE { get; set; }
        public string INCIDENT_TIME { get; set; }
        public string INCIDENT_DETAILS { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_ON { get; set; }
        public string UPDATED_ON { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
     
    }
    public class SUP_ACADEMIC_ASSESSMENT_OBJECTIVE
    {
        public string ACADEMIC_ASSESSMENT_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class SUP_BEHAVIOUR_LEVEL
    {
        public string LEVEL_ID { get; set; }
        public string LEVEL_CODE { get; set; }
        public string LEVEL_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STUDENT_ID { get; set; }

    }
    public class PARENT_MEETING
    {
        public string PARENT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEM { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string FEEDBACK { get; set; }

        public string MEETING_DATE { get; set; }
        public string MEETING_TIME { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class MENTOR_LATE_ENTRY
    {
        public string LATE_ENTRY_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string REASON { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
       

        // Display-only fields (from JOIN)
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string MOBILE { get; set; }
        public string LATE_FEE_ID { get; set; }
        public string STU_MOBILENO { get; set; }
        public string LAST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string STAFF_NAME { get; set; }
        public string FINE_AMOUNT { get; set; }
        public string FR_MOBILE { get; set; }
        public string SHIFT_NAME { get; set; }
     

    }

    public class MENTORING_COUNSELLING_SESSION
    {
        public string SESSION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SESSION_DATE { get; set; }
        public string SESSION_TIME { get; set; }
        public string MATTERS_DISCUSSED { get; set; }
        public string GUIDANCE_GIVEN { get; set; }
        public string FOLLOW_UP_REQUIRED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string SUP_SEMESTER_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
    }


    public class MENTORING_ATTENDANCE_REPORT
    {
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string MOBILE { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_TITLE { get; set; }
        public string HOUR_FROM { get; set; }
        public string HOUR_TO { get; set; }
        public string ATTENDENCE_DATE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string ABSENT_COUNT { get; set; }
        public string DAYS_COUNT { get; set; }
        public string DATE_TO { get; set; }
        public string DATE_FROM { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_YEAR { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string AC_YEAR { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string SUP_SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
    }



    public class OverallMentorAssessmentRow
    {
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string OVERALL_ASSESSMENT_ID { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_ON { get; set; }
        public string AREA { get; set; }
        public string SEM1 { get; set; }
        public string SEM2 { get; set; }
        public string SEM3 { get; set; }
        public string SEM4 { get; set; }
        public string SEM5 { get; set; }
        public string SEM6 { get; set; }
        public string ACADEMIC_ASSESSMENT_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SUP_OVERALL_MENTOR_ASSESSMENT_ID { get; set; }
        public string ASSESSMENT_AREA { get; set; }
        public string NO_OF_SEMESTER { get; set; }
        public string ASSESSMENT_OBJECTIVE_ID { get; set; }
        public string OBJECTIVE_NAME { get; set; }
    }


    public class MENTOR_GOAL_IN_LIFE
    {
        public string GOAL_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string GOAL_IN_LIFE { get; set; }
        public string STRENGTH { get; set; }
        public string WEAKNESS { get; set; }
        public string ROLE_MODEL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }



}

