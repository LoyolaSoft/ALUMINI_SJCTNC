using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.Model
{
    public class SUP_SUBJECT_TYPE
    {
        public string SUBJECT_TYPE_ID { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class Sup_Fee_Category
    {
        public string FEE_CATEGORY_ID { get; set; }
        public string FEE_CATEGORY { get; set; }
    }
    public class SUP_HEAD
    {
        public string MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD { get; set; }
        public string HEAD_CODE { get; set; }
        public string ACCOUNT { get; set; }
        public string HEAD_TYPE { get; set; }
        public string IS_USED { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string IS_UPDATED { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string STATUS { get; set; }
        public string FEE_CATEGORY { get; set; }
        public string TEMP_ID { get; set; }
        public string HEAD_ORDER { get; set; }
        public string IS_REFUNDABLE { get; set; }
        public string SHIFT { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string FEE_MODE { get; set; }
        public string FREQUENCY_ID { get; set; }
    }
    public class SUP_FEE_MAIN_HEAD
    {
        public string MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD { get; set; }
        public string HEAD { get; set; }
        public string HEAD_ID { get; set; }
        public string IS_USED { get; set; }
        public string IS_DELETED { get; set; }
        public string HEAD_COUNT { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string FEE_CATEGORY_ID { get; set; }

    }
    public class SUP_BANK
    {
        public string BANK_ID { get; set; }
        public string BANK_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string IS_UPDATED { get; set; }
        public string IS_USED { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_BANK_ACCOUNT
    {
        public string BANK_ACCOUNT_ID { get; set; }
        public string ACCOUNT_PURPOSE { get; set; }
        public string BANK { get; set; }
        public string BANK_NAME { get; set; }
        public string PASSBOOK_NO { get; set; }
        public string STARTED_DATE { get; set; }
        public string CLOSED_DATE { get; set; }
        public string IS_USED { get; set; }
        public string IS_DELETED { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string ACCOUNT_TYPE { get; set; }
    }
    public class SUP_FEE_FREQUENCY
    {
        public string FREQUENCY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string STATUS { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_USED { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string LAST_DATE_OF_PAYMENT { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string TYPE { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string FEE_MODE { get; set; }
        public string MAIN_HEAD { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }


    }

    public class SUP_FEE_ROOT
    {
        public string FEE_ROOT_ID { get; set; }
        public string FEE_ROOT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class SUP_HOURS
    {
        public string HOUR_ID { get; set; }
        public string HOUR { get; set; }
        public string TIME_FROM { get; set; }
        public string COURSE_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string TIME_TO { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        // Extra
        public string STAFF_ID { get; set; }
        public string NO_OF_HOURS { get; set; }

    }
    public class JsonResultData
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string sResult { get; set; }
        public string sResultS { get; set; }
        public List<string> sResultStringArray = new List<string>();
        public object oResult { get; set; }

    }
    public class SUP_RELATION
    {
        public string RELATION_ID { get; set; }
        public string RELATION { get; set; }
    }
    public class SupCourseInfo
    {
        public string COURSE_ROOT_ID { get; set; }
        public string YEAR_FROM { get; set; }
        public string YEAR_TO { get; set; }
        public string DEPARTMENT { get; set; }
        public string YEAR { get; set; }
        public string SEMESTER_FROM { get; set; }
        public string SEMESTER_TO { get; set; }
        public string PART { get; set; }
        public string PAPER { get; set; }
        public string COURSE_TYPE { get; set; }
        public string HRS_PER_WEEK { get; set; }
        public string CREDITS { get; set; }
        public string OPTION_NAME { get; set; }
        public string PAPER_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_NME_SUBJECT { get; set; }
        public string IS_ALLIED_SUBJECT { get; set; }
        public string COURSE_ORDER { get; set; }
        public string IS_DELETED { get; set; }
        public string SUBJECTS { get; set; }
        public string IS_COMPULSORY { get; set; }
        public string UGC_COURSE_TYPE { get; set; }

    }
    public class STAFF_INFO
    {
        public string STAFF_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_LEFT { get; set; }
    }
    public class SupCourseList
    {

    }
    public class SupPaperType
    {
        public string PAPER_TYPE_ID { get; set; }
        public string PAPER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class SupGender
    {
        public string GENDER_ID { get; set; }
        public string GENDER_NAME { get; set; }
    }
    public class SupBloodGroup
    {
        public string BLOOD_GROUP_ID { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }
    }
    public class SupMotherTongue
    {
        public string MOTHER_TONGUE_ID { get; set; }
        public string MOTHER_TONGUE_NAME { get; set; }
    }
    public class SupNationality
    {
        public string NATIONALITY_ID { get; set; }
        public string NATIONALITY_NAME { get; set; }
    }
    public class SupDepartment
    {
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT { get; set; }
    }
    public class SupCommunity
    {
        public string COMMUNITYID { get; set; }
        public string COMMUNITY { get; set; }
    }
    public class SupMaritalStatus
    {
        public string MARITAL_STAUS_ID { get; set; }
        public string MARITAL_STATUS_NAME { get; set; }
    }
    public class SUP_RELIGION
    {
        public string RELIGIONID { get; set; }
        public string RELIGION { get; set; }
    }
    public class SUP_STATE
    {
        public string STATE_ID { get; set; }
        public string STATE_NAME { get; set; }
        public string STATE { get; set; }
    }
    public class SUP_DEGREE
    {
        public string DEGREE_ID { get; set; }
        public string DEGREE { get; set; }
    }
    public class SUP_DAY_ORDER
    {
        public string DAY_ORDER_ID { get; set; }
        public string DAY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class Sup_Duration_Unit
    {
        public string DURATION_UNIT_ID { get; set; }
        public string DURATION_UNIT { get; set; }
    }
    public class Sup_Channel
    {
        public string CHANNEL_ID { get; set; }
        public string CHANNEL { get; set; }
    }
    public class Sup_Subject
    {
        public string SUBJECT_ID { get; set; }
        public string SUBJECT { get; set; }
    }
    public class SUP_MEDIUM_OF_INSTRUCTION
    {
        public string MEDIUM_ID { get; set; }
        public string MEDIUM_OF_INSTRUCTION { get; set; }
    }
    public class Sup_Volume
    {
        public string VOLUME_ID { get; set; }
        public string VOLUME { get; set; }
    }
    public class sup_Level
    {
        public string LEVEL_ID { get; set; }
        public string LEVEL { get; set; }
    }
    public class sup_PublishCategory
    {
        public string PUBLISH_CATEGORY_ID { get; set; }
        public string PUBLISH_CATEGORY { get; set; }
    }
    public class Sup_Shift
    {
        public string SHIFT_ID { get; set; }
        public string SHIFT_NAME { get; set; }
        public string TIME { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class Sup_Alu_Present_Status
    {
        public string PRESENT_STATUS_ID { get; set; }
        public string PRESENT_STATUS_NAME { get; set; }
    }
    public class Sup_Qualification
    {
        public string QUALIFICATION_ID { get; set; }
        public string QUALIFICATION { get; set; }
    }
    public class Sup_SubCategory
    {
        public string STF_CATEGORY_ID { get; set; }
        public string STF_CATEGORY { get; set; }
    }
    public class Sup_Designation
    {
        public string DESIGNATIONID { get; set; }
        public string DESIGNATION { get; set; }
        public string DESIGNATIONORDER { get; set; }
        public string ISUSED { get; set; }
        public string ISDELETED { get; set; }
    }
    public class SUP_STAFF_ROLES_RESPONSIBILE
    {
        public string STF_ROLE_RES_ID { get; set; }
        public string ROLES__RESPONSIBILITIES { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ORDER_BY { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class Sup_Physical_Status
    {
        public string PHYSICAL_STATUS_ID { get; set; }
        public string PHYSICAL_STATUS_NAME { get; set; }
    }
    public class Sup_Staff_Status
    {
        public string STATUS_ID { get; set; }
        public string STATUS { get; set; }
    }
    public class SUP_ACHIEVEMENT
    {
        public string ACHIEVEMENT_ID { get; set; }
        public string ACHIEVEMENT_NAME { get; set; }
    }
    public class SUP_PART
    {
        public string PART_ID { get; set; }
        public string PART { get; set; }
    }
    public class SupProgrammeLevel
    {
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
    }
    public class SupProgrammeType
    {
        public string PROGRAMME_TYPE_ID { get; set; }
        public string PROGRAMME_TYPE { get; set; }
    }
    public class SupProgrammeMode
    {
        public string PROGRAMME_MODE_ID { get; set; }
        public string PROGRAMME_MODE { get; set; }
    }
    public class SUP_FAMILY
    {
        public string RELATION_ID { get; set; }
        public string RELATION { get; set; }
    }
    public class SUP_APPLICATION_TYPE
    {
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string PREFIX { get; set; }
        public string SUFFIX { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class cp_Classes_2017
    {
        public string STAFF_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string SHIFT { get; set; }
        public string CLASS_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_YEAR { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string PROGRAMME { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string BATCH { get; set; }
        public string CHECKIN_DATE { get; set; }
        public string DATE { get; set; }
        public string COURSE_ID { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        //Extra
        public string CERTIFICATE_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string CHECKIN_ID { get; set; }

    }
    public class StuCourseInfo
    {
        public string CLASS_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string SHIFT { get; set; }
        public string CLASS_NAME { get; set; }
        public string DEPARTMENT { get; set; }
    }

    public class SEMESTER_TYPE
    {
        public string SEMESTER_TYPE_ID { get; set; }
        public string SEMESTER_TYPE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_SEMESTER_TYPE
    {
        public string SEMESTER_TYPE_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class sup_message_type
    {
        public string MSG_TYPE_ID { get; set; }
        public string MESSAGE_TYPE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class sup_Batch
    {
        public string BATCH_ID { get; set; }
        public string BATCH { get; set; }
        public string BATCH_YEAR { get; set; }
        public string AC_BATCH_ID { get; set; }
    }
    public class CERTIFICATE_STU_INFO
    {
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string BATCH { get; set; }
        public string CLASS_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string BEHAVE { get; set; }
        public string GENDER { get; set; }
        public string DEPARTMENT { get; set; }
        public string COLLEGE_FEE { get; set; }
        public string EXAMFEEDUE { get; set; }
        public string HOSTELFEEDUE { get; set; }
        public string BUSFEEDUE { get; set; }
        public string REGISTER_NO { get; set; }
        public string PHOTO { get; set; }
        public string SUBJECT_CODE { get; set; }
        public string ELIGIBLE_TYPE { get; set; }
        public string DATE_OF_EXAM { get; set; }
        public string HALL_NAME { get; set; }
        public string SESSION { get; set; }
        public string ATTPERCENTAGE { get; set; }
        public string HOSTEL_ID { get; set; }
        public string ADMISSION_DATE { get; set; }
        public string LEAVING_DATE { get; set; }
    }
    public class sup_frequency_type
    {
        public string FREQUENCY_TYPE_ID { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string FEE_FREQUENCY_FEE_MODE_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
    }

    public class SUP_REASON
    {
        public string REASON_ID { get; set; }
        public string REASON { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_STUDENT_TAG_LIST
    {
        public string TAG_ID { get; set; }
        public string TAG_NAME { get; set; }
        public string COLOR_CODE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class SUP_ATTENDANCE_TYPE
    {
        public string ATTENDANCE_TYPE_ID { get; set; }
        public string ATTENDANCE_TYPE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }

    }
    public class SUP_SPL_ATTENDANCE_TYPE
    {
        public string SPL_ATTENDANCE_TYPE_ID { get; set; }
        public string SPL_ATTENDANCE_TYPE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }

    }
    public class cp_Program_2017
    {
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_DESCRIPTION { get; set; }
    }
    public class cp_Department_2017
    {
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT { get; set; }
    }
    public class sup_Second_language
    {
        public string SECOND_LANGUAGE_ID { get; set; }
        public string SECOND_LANGUAGE_NAME { get; set; }
    }
    public class Sup_Spoken_Language
    {
        public string SPOKEN_LANGUAGE_ID { get; set; }
        public string SPOKEN_LANGUAGE_NAME { get; set; }
    }
    public class Sup_Language
    {
        public string LANGUAGE_ID { get; set; }
        public string LANGUAGE_NAME { get; set; }
    }
    public class SUP_ACTIVITY_TYPE
    {
        public string ACTIVITY_TYPE_ID { get; set; }
        public string ACTIVITY_TYPE { get; set; }
    }
    public class SUP_ACADEMIC_YEAR_LIST
    {
        public string ACADEMIC_YEAR_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string ACTIVE_YEAR { get; set; }
    }
    public class Sup_Specially_Abled
    {
        public string SPECIALLY_ABLED_ID { get; set; }
        public string SPECIALLY_ABLED_NAME { get; set; }
    }
    public class Sup_ResidencyType
    {
        public string RESIDENCY_TYPE_ID { get; set; }
        public string RESIDENCY_TYPE_NAME { get; set; }
    }
    public class SUP_LEFT_REASON
    {
        public string REASON_ID { get; set; }
        public string REASON { get; set; }
    }
    public class Sup_Occupation
    {
        public string OCCUPATION_ID { get; set; }
        public string OCCUPATION_NAME { get; set; }
    }
    public class Sup_Education
    {
        public string EDUCATION_ID { get; set; }
        public string EDUCATION_NAME { get; set; }
    }
    public class Sup_Education_board
    {
        public string EDUCATION_BOARD_ID { get; set; }
        public string EDUCATION_BOARD { get; set; }
    }
    public class Sup_Extra_curriculam
    {
        public string CURUCULAR_ID { get; set; }
        public string CURUCULAR_NAME { get; set; }
    }
    public class Sup_MediyumofStudy
    {
        public string MEDIUM_ID { get; set; }
        public string MEDIUM_NAME { get; set; }
    }
    public class Sup_Mother_Occupation
    {
        public string OCCUPATION_ID { get; set; }
        public string OCCUPATION_NAME { get; set; }
    }
    public class Sup_Mother_Education
    {
        public string EDUCATION_ID { get; set; }
        public string EDUCATION_NAME { get; set; }
    }
    public class Sup_Father_Nationality
    {
        public string NATIONALITY_ID { get; set; }
        public string NATIONALITY_NAME { get; set; }
    }
    public class Sup_Father_Employer
    {
        public string EMPLOYER_ID { get; set; }
        public string EMPLOYER_NAME { get; set; }
    }
    public class Sup_Father_country_Of_Issue
    {
        public string COUNTRY_OF_ISSUE_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
    }
    public class Sup_Mother_Nationality
    {
        public string NATIONALITY_ID { get; set; }
        public string NATIONALITY_NAME { get; set; }
    }
    public class Sup_Mother_Employer
    {
        public string EMPLOYER_ID { set; get; }
        public string EMPLOYER_NAME { set; get; }
    }
    public class Sup_Mother_country_Of_Issue
    {
        public string COUNTRY_OF_ISSUE_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
    }
    public class Sup_Activity
    {
        public string ACTIVITY_ID { get; set; }
        public string ACTIVITY_NAME { get; set; }
    }
    public class SUP_ACTIVITY_LEVEL
    {
        public string ACTIVITY_LEVEL_ID { get; set; }
        public string ACTIVITY_LEVEL { get; set; }
    }
    public class Sup_Week_No
    {
        public string WEEK_NO_ID { get; set; }
        public string WEEK_NAME { get; set; }
    }
    public class SUP_TALENT_AREA
    {
        public string ACTIVITY_LEVEL_ID { get; set; }
        public string ACTIVITY_LEVEL { get; set; }
    }
    public class SUP_EXAMPASSED
    {
        public string EXAM_PASSED_ID { get; set; }
        public string EXAM_PASSED { get; set; }
    }
    public class SUP_OPTION
    {
        public string OPTION_ID { get; set; }
        public string OPTION_NAME { get; set; }
    }
    public class SUP_OVERALL_GRADE
    {
        public string OVERALL_GRADE_ID { get; set; }
        public string OVERALL_GRADE { get; set; }
    }
    public class SUP_COUNTRY
    {
        public string COUNTRY_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
    }
    public class SUP_COURSEBYID
    {
        public string COURSE_ROOT_ID = "COURSE_ROOT_ID";
        public string COURSE_TITLE = "COURSE_TITLE";
    }
    public class SUP_CLASS
    {
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string SETTING_ID { get; set; }
    }
    public class SupSemester
    {
        public string SUP_SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
        public bool SELECTED { get; set; }
    }
    public class SupClassType
    {
        public string CLASS_TYPE_ID { get; set; }
        public string CLASS_TYPE { get; set; }

    }
    public class SupClassLevel
    {
        public string CLASSLEVELID { get; set; }
        public string CLASSLEVEL { get; set; }

    }
    public class SupFaculty
    {
        public string FACULTY_ID { get; set; }
        public string FACULTY { get; set; }

    }
    public class SubClassYear
    {
        public string CLASS_YEAR_ID { get; set; }
        public string CLASS_YEAR { get; set; }

    }
    public class SUP_FIELDS
    {
        public string FIELD_ID { get; set; }
        public string FIELD_NAME { get; set; }
        public string ALICE_NAME { get; set; }
        public string PROPERTY_NAME { get; set; }
    }
    public class STATS_MODEL
    {
        public string CLASS_NAME { get; set; }
        public string CLASS_YEAR { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string RELIGIONID { get; set; }
        public string RELIGION { get; set; }
        public string IS_LEFT { get; set; }
        public int STUDENT_COUNT { get; set; }
        public int EMPTY_FIELD { get; set; }
        public string SELECTED_FIELD { get; set; }
        public string SELECTED_NAME { get; set; }
    }
    public class SupOptionName
    {
        public string OPTION_ID { get; set; }
        public string OPTION_NAME { get; set; }
    }

    public class NoticeBoard
    {
        public string NOTICEBOARD_ID { get; set; }
        public string NOTICEBOARD { get; set; }
        public string IS_ACCEPTED { get; set; }
        public string NOTICEBOARD_LOG_ID { get; set; }
        public string USER_ID { get; set; }
        public string IS_CANCELED { get; set; }
        public string IS_VIEWED { get; set; }
        public string IS_RESPONSE_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string ROLE_ID { get; set; }
        public string APPTYPE { get; set; }
        public string CLASS_YEAR { get; set; }
        public string LINK_UPLOAD { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string RoleId { get; set; }
        // New Fields
        [AllowHtml]
        public string DESCRIPTION { get; set; }
        public string FILE_TYPE { get; set; }
        public string DUE_DATE { get; set; }
        public string SHIFT { get; set; }
        public string ROLE_NAME { get; set; }
        public string APPTYPE_NAME { get; set; }
        public string CLASS_YEAR_NAME { get; set; }
        public string SHIFT_NAME { get; set; }
        public string IS_COMMENT_ENABLE { get; set; }
        public string IS_FILE_UPLOAD_ENABLE { get; set; }
        public string VIEW_COUNT { get; set; }
        public string RESPONSE_COUNT { get; set; }
        public string COMMENTS { get; set; }
        public string FILE_PATH { get; set; }
        public string FIRST_NAME { get; set; }
        public string CREATED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
    }

    public class SupPaperCode
    {
        public string PAPER_CODE_ID { get; set; }
        public string PAPER_CODE { get; set; }
    }
    public class SupCourseType
    {
        public string COURSE_TYPE_ID { get; set; }
        public string COURSE_TYPE { get; set; }
    }
    public class SupIsNMESubject
    {
        public string IS_NME_ID { get; set; }
        public string IS_NME_NAME { get; set; }
    }
    public class SupIsAlliedSubject
    {
        public string IS_ALLIED_ID { get; set; }
        public string IS_ALLIED_NAME { get; set; }
    }
    public class SupIsCompulsory
    {
        public string IS_COMPULSORY_ID { get; set; }
        public string IS_COMPULSORY_NAME { get; set; }
    }
    public class SupUGCCourseType
    {
        public string UGC_COURSE_TYPE_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
    }
    public class cp_Course_Root_2017
    {
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string IS_NME_SUBJECT { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER { get; set; }
        public string PAPER_TYPE { get; set; }
        public string SELECTED { get; set; }
        public string DEPARTMENT { get; set; }
        public string COURSE_TYPE { get; set; }
        public string CREDITS { get; set; }
        public string CREDIT { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
        public string NME_CLASS_ID { get; set; }
        public string SETTINGS_ID { get; set; }
        public string QUOTA { get; set; }
        public string IS_ALLIED_SUBJECT { get; set; }
        public string CLSCRSSTF_ID { get; set; }

    }
    public class stf_Personal_Info
    {
        public string STAFF_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_CODE { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_ID { get; set; }
        public string PROGRAMME { get; set; }
        public string PHOTO { get; set; }
        public string FIRST_NAME { get; set; }
        public bool SELECTED { get; set; }
        public string RECORDSCOUNT { get; set; }
        public string CLASS_ID { get; set; }
        public string CATEGORY { get; set; }
        public string MOBILE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string ROLES { get; set; }
        public string SHIFT { get; set; }
        public string PAPER_TYPE { get; set; }
        public string DATE_OF_BIRTH { get; set; }


    }
    public class SUP_NON_TEACHING_CATEGORY
    {
        public string NON_TEACHING_CATEGORY_ID { get; set; }
        public string NON_TEACHING_CATEGORY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETE { get; set; }
    }

    public class SUP_BOARD_MEMBER
    {
        public string BOARD_MEMBER_ID { get; set; }
        public string BOARD_MEMBER_NAME { get; set; }
    }
    public class SUP_CONDUCT
    {
        public string CONDUCT_ID { get; set; }
        public string CONDUCT_NAME { get; set; }
    }
    public class NME_Setting
    {
        public string SETTINGS_ID { get; set; }
        public string SETTINGS_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_ROLE
    {
        public string ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
    }
    public class SUP_USER_TYPE
    {
        public string USER_TYPE_ID { get; set; }
        public string USER_TYPE_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }

    }
    public class DataPoint1
    {

        //Explicitly setting the name to be used while serializing to JSON.        
        public string label { get; set; }

        //Explicitly setting the name to be used while serializing to JSON.

        public string y { get; set; }
    }
    public class DataPoint
    {

        //Explicitly setting the name to be used while serializing to JSON.        
        public string label { get; set; }

        //Explicitly setting the name to be used while serializing to JSON.

        public double y { get; set; }
    }
    public class DataLabel
    {

        //Explicitly setting the name to be used while serializing to JSON.        
        public string label { get; set; }


    }
    public class DataY
    {

        //Explicitly setting the name to be used while serializing to JSON.        
        public double y { get; set; }

    }

    public class SUP_TEACHING_CATEGORY
    {
        public string TEACHING_CATEGORY_ID { get; set; }
        public string TEACHING_CATEGORY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class CATEGORY
    {
        public List<SUP_TEACHING_CATEGORY> liTeachingCategory { get; set; }
        public List<SUP_NON_TEACHING_CATEGORY> liNonTeachingCategory { get; set; }
    }
    public class Student_Personal_Info
    {
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_ID { get; set; }
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
        public string FR_MOBILE { get; set; }
        public string MOBILE_NO { get; set; }
        public string CHECKIN_I { get; set; }
        public string STU_MOBILENO { get; set; }

    }
    public class SUP_HOUR_TYPE
    {
        public string hour_type_id { get; set; }
        public string hourType { get; set; }
        public string is_active { get; set; }
        public string is_deleted { get; set; }
    }
    public class AUTO_GENERATE_NUMBERS
    {
        public string auto_generate_numbers_id { get; set; }
        public string exam_receipt_no { get; set; }
        public string exam_receipt_prefix { get; set; }
        public string advance_receipt_no { get; set; }
        public string advance_receipt_prefix { get; set; }
        public string Access_No { get; set; }
    }
    public class SUP_HOSTEL
    {
        public string HOSTEL_ID { get; set; }
        public string HOSTEL_NAME { get; set; }
        public string HOSTEL_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_UNIVERSITY
    {
        public string UNIVERSITY_ID { get; set; }
        public string UNIVERSITY { get; set; }
    }
    public class MainCommunity
    {
        public string MAIN_COMMUNITY_ID { get; set; }
        public string MAIN_COMMUNITY { get; set; }
    }
    public class SUP_HSS_GROUPS
    {
        public string HSS_GROUP_ID { get; set; }
        public string HSS_GROUP { get; set; }
        public string HSS_GROUP_CODE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_USED { get; set; }
    }
    public class SUP_APPTYPE_PROG_GROUPS
    {
        public string PRO_GROUPS_ID { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string APPTYPE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_NEW { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string PROGRAMME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string SHIFT { get; set; }
    }
    public class DropDownListModel
    {
        public string MOTHER_TONGUE { get; set; }
        public string RELIGION_ID { get; set; }
        public string CASTE { get; set; }
        public string CCOUNTRY { get; set; }
        public string GENDER { get; set; }
        public string HSS_GROUP_ID { get; set; }
        public string OCCUPATION { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string UNIVERSITY { get; set; }
        public string CSTATE { get; set; }
        public string MAIN_COMMUNITY { get; set; }
        public string ADM_CLASS { get; set; }
    }

    public class SUP_APPLICTION_TYPE
    {
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string PREFIX { get; set; }
        public string SUFFIX { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_SEMESTER
    {
        public string SUP_SEMESTER_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string SEMESTER_TYPE_NAME { get; set; }
        public string COMMUNITYID { get; set; }
        public string COMMUNITY { get; set; }


    }
    public class SUP_CERTIFICATE_LIST
    {
        public string CERTIFICATE_ID { get; set; }
        public string CERTIFICATE_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string CATEGORY_ID { get; set; }
    }
    public class SUP_CERTIFICATE_CATEGORY_LIST
    {
        public string CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string ORDER_BY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_CERTIFICATE_AUTHORIZE_PERSON
    {
        public string AUTHORIZER_ID { get; set; }
        public string AUTHORIZER_NAME { get; set; }
        public string ORDER_BY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_PROGRAMME_LEVEL
    {
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string PL_SERIES_ROLLNO { get; set; }
        public string PL_SERIES_REGNO { get; set; }
        public string PL_SERIES_ADMNO { get; set; }
    }

    public class ACADEMIC_SEMESTER
    {
        public string ACC_SEMESTER_ID { get; set; }
        public string BATCH { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string SEMESTER { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string BEGIN_YEAR { get; set; }
        public string EXPIRED_YEAR { get; set; }
        public string STUDENT_ID { get; set; }
        public string BATCH_YEAR { get; set; }
        public string CLASS_YEAR { get; set; }
    }

    public class CP_COURSE_ROOT
    {
        public string COURSE_ROOT_ID { get; set; }
        public string YEAR_FROM { get; set; }
        public string YEAR_TO { get; set; }
        public string DEPARTMENT { get; set; }
        public string YEAR { get; set; }
        public string SEMESTER_FROM { get; set; }
        public string SEMESTER_TO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PART { get; set; }
        public string PAPER { get; set; }
        public string COURSE_TYPE { get; set; }
        public string HRS_PER_WEEK { get; set; }
        public string CREDITS { get; set; }
        public string OPTION_NAME { get; set; }
        public string PAPER_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_NME_SUBJECT { get; set; }
        public string IS_ALLIED_SUBJECT { get; set; }
        public string COURSE_ORDER { get; set; }
        public string IS_DELETED { get; set; }
        public string SUBJECTS { get; set; }
        public string IS_COMPULSORY { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string PAPER_TYPE { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_NAME { get; set; }
        public string CIA_DISABLE { get; set; }
        public string CLSCRSSTF_ID { get; set; }
        public string CONS_COURSE_TITLE { get; set; }
        public string SHIFT_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string COURSE_STRUCTURE_ID { get; set; }
        public string INTERNAL { get; set; }
        public string EXTERNAL { get; set; }
        public string TOTAL { get; set; }
        public string FEEDBACK_ID { get; set; }
        public string TIMETABLE_ID { get; set; }
        public string EXAM_COURSE_TYPE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_MARK { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_ID { get; set; }
    }

    public class Morris_Chart
    {
        public string sMonthColor { get; set; }
        public string sFeeColor { get; set; }
        public string sMonthData { get; set; }
        public string sFeeData { get; set; }
        public string sMonthLabel { get; set; }
        public string Message { get; set; }

    }
    public class CLASS_INCHARGE
    {
        public string CLASS_INCHARGE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_COURSE_WISE_HEAD
    {
        public string COURSE_HEAD_ID { get; set; }
        public string HEAD { get; set; }
        public string AMOUNT { get; set; }
        public string IS_USED { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_SETS
    {
        public string SET_ID { get; set; }
        public string SET_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_QUIZ_QUESTION_TYPE
    {
        public string QUESTION_TYPE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    #region Sup CIA Components
    public class SUP_CIA_COMPONENTS
    {
        public string COMPONENT_ID { get; set; }
        public string COMPONENT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    #endregion


    public class SUP_ROOM
    {
        public string ROOM_ID { get; set; }
        public string ROOM_NO { get; set; }
        public string COLLEGE_BLOCK_ID { get; set; }
        public string COLUMN { get; set; }
        public string ROW { get; set; }
        public string ROOM_CAPACITY { get; set; }
        public string EXAM_CAPACITY { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string BLOCK_NAME { get; set; }
        public string EXAM_TYPE { get; set; }
        public string BLOCK_CODE { get; set; }
        public string EXAM_TYPE_NAME { get; set; }
        public string BLOCK_ORDER { get; set; }
        public string ROOM_ORDER { get; set; }
        public string SPACE_COLUMN { get; set; }
    }

    public class SUP_EXAM_TYPE
    {
        public string EXAM_TYPE { get; set; }
        public string EXAM_TYPE_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class SUP_EXAM_SESSION
    {
        public string EXAM_SESSION_ID { get; set; }
        public string EXAM_SESSION { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string EXAM_TYPE { get; set; }
    }


    public class SUP_COLLEGE_BLOCK
    {
        public string COLLEGE_BLOCK_ID { get; set; }
        public string BLOCK_NAME { get; set; }
        public string BLOCK_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_SESSION
    {
        public string SESSION_ID { get; set; }
        public string SESSION_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class SUP_STAFF_TYPE
    {
        public string STAFF_TYPE { get; set; }
        public string STAFF_TYPE_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class BIRTHDAY_CELEBRITIES
    {
        public string STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string PHOTO { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string TODAY_DATE { get; set; }
        public string STUDENT_ID { get; set; }
        public string MOBILE { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string CATEGORY { get; set; }

    }

    public class DashBoardModel
    {
        public string STUDENT_COUNT { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string TOTAL_MALE_STUDENT { get; set; }
        public string TOTAL_FEMALE_STUDENT { get; set; }
        public string TOTAL_ABSENT { get; set; }
        public string HOUR_ID { get; set; }
        public string ROLE_ID { get; set; }
        public string IS_GROUP { get; set; }
        public string ACTION_NAME { get; set; }
        public string ROLE_NAME { get; set; }
        public string CONTROLLER_NAME { get; set; }
        public string DASHBOARD_ID { get; set; }
        public string SHORT_NAME { get; set; }
        public string IS_ORDER { get; set; }
        public string USER_ROLE_ID { get; set; }
        public string IS_ACTIVE { get; set; }

    }

    public class SUP_GRADE_POINTS
    {
        public string SUP_GRADE_POINT_ID { get; set; }
        public string GRADE { get; set; }
        public string GRADE_POINT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string MIN_MARK { get; set; }
        public string MAX_MARK { get; set; }
        public string PROGRAME_LEVEL { get; set; }
        public string PROGRAME_GROUP_ID { get; set; }
    }


    public class SEM_DETAILS_FOR_CHAT
    {
        public string label { get; set; }
        public string LABEL { get; set; }
        public double y { get; set; }
        public string Y { get; set; }
        public string TOTAL_WORKING_HOURS { get; set; }
        public string ABSENT_HOURS { get; set; }
        public string SUP_SEMESTER_ID { get; set; }
        public string TOTAL_OPTIONAL_WORKING_HOURS { get; set; }
        public int TOTAL_HOURS { get; set; }
        public int ATTENDANCE_PERCENTAGE { get; set; }
    }

    #region Library
    public class SUP_LI_ACCOUNT
    {
        public string ACCOUNT_ID { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string PREFIX { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_COURSE
    {
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_DISCIPLINE
    {
        public string DISCIPLINE_ID { get; set; }
        public string DISCIPLINE_CODE { get; set; }
        public string DISCIPLINE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_DOCTYPE
    {
        public string DOCUMENT_TYPE_ID { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string DOCUMENT_TYPE_CODE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_HEAD
    {
        public string HEAD_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_LI_ARTICLE_TYPE
    {
        public string ARTICLE_TYPE_ID { get; set; }
        public string ARTICLE_TYPE_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class SUP_LI_SERIES
    {
        public string SERIES_ID { get; set; }
        public string SERIES_CODE { get; set; }
        public string SERIES_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class LIBRARY_BOOK_APPROVED_ANNOUNCE
    {
        public string RESERVE_ID { get; set; }
        public string STATUS { get; set; }
    }

    public class SUP_LI_BLOCK
    {
        public string BLOCK_ID { get; set; }
        public string BLOCK_NAME { get; set; }
        public string BLOCK_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_STATUS
    {
        public string STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string STATUS_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_BOOK_STATUS
    {
        public string STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class SUP_LI_ILLUSTRATION
    {
        public string ILLUSTRATE_ID { get; set; }
        public string ILLUSTRATE_CODE { get; set; }
        public string PREFIX { get; set; }
        public string ILLUSTRATE { get; set; }
    }

    public class SUP_LI_AUTOCLASS
    {
        public string AUTO_CLASS_ID { get; set; }
        public string AUTO_CLASS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_KEYWORDS
    {
        public string KEYWORD_ID { get; set; }
        public string KEYWORD { get; set; }
        public string KEY_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_IMPRINTS
    {
        public string IMPRINT_ID { get; set; }
        public string IMPRINT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_CLASS_NO
    {
        public string CLASS_NO_ID { get; set; }
        public string CLASS_NO { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_LIB_CODE
    {
        public string LIB_CODE_ID { get; set; }
        public string LIB_DESCRIPTION { get; set; }
        public string LIB_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_LI_RENEW_DAYS
    {
        public string RENEW_DAY_ID { get; set; }
        public string MEMBER_TYPE { get; set; }
        public string DAYS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_SERVICE_TIME
    {
        public string SERVICE_ID { get; set; }
        public string SERVICE_NAME { get; set; }
        public string S_HOUR { get; set; }
        public string S_MINUTE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    #endregion

    public class SUP_PROGRAMME_TYPE
    {
        public string PROGRAMME_TYPE_ID { get; set; }
        public string PROGRAMME_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class SUP_STAFF_ASSOCIATION
    {
        public string STAFF_ASSOCIATION_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string SHIFT_ID { get; set; }
        public string SHIFT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_NOTIFICATION_LEVEL
    {
        public string NOTIFICATION_LEVEL_ID { get; set; }
        public string NOTIFICATION_LEVEL { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class MENU_ITEMS
    {
        public string MENU_ID { get; set; }
        public string MENU_NAME { get; set; }
        public string MODULE_NAME { get; set; }
        public string PARENT_ID { get; set; }
        public string CONTROLLER { get; set; }
        public string ACTION { get; set; }
        public string MODULE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_ORGANIZER
    {
        public string ORGANIZER_ID { get; set; }
        public string ORGANIZER_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class MODULES
    {
        public string MODULES_ID { get; set; }
        public string MODULE_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string MODULES_ORDER { get; set; }
        public string STYLE { get; set; }
        public string has_sub { get; set; }
    }

    public class USER_ROLES_RIGHTS
    {
        public string USER_RIGHTS_ID { get; set; }
        public string MENU_ITEM_ID { get; set; }
        public string MODIFY { get; set; }
        public string ROLE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string MENU_NAME { get; set; }
        public string ROLE_NAME { get; set; }
        public string IS_GROUP { get; set; }
        public string USER_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string GROUP_CONCAT { get; set; }

    }
    public class SUP_EXTERNAL_CALCULATION
    {
        public string EXTERNAL_CALCULATION_ID { get; set; }
        public string EXTERNAL_CALCULATION { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string COURSE_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_BOOK_INVOICE
    {
        public string INVOICE_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public string VENDOR { get; set; }
        public string NO_OF_BOOK { get; set; }
        public string PLACE { get; set; }
        public string DATE_OF_PURCHASE { get; set; }
        public string DATE_OF_ENTRY { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class SUP_LI_TAPS
    {
        public string TAPS_ID { get; set; }
        public string PREFIX { get; set; }
        public string TAPS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_DOC_CATEGORY
    {
        public string DOC_CATEGORY_ID { get; set; }
        public string DOC_CATEGORY_CODE { get; set; }
        public string DOC_CATEGORY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LI_BOOK_TYPE
    {
        public string TYPE_ID { get; set; }
        public string PREFIX { get; set; }
        public string BOOK_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_SUPERVISON_STAFF_TYPE
    {
        public string SUP_SUPERVISION_TYPE_ID { get; set; }
        public string SUPERVISION_STAFF_TYPE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class FBOPTIONS
    {
        public string QUESTION_ID { get; set; }
        public string OPTION_ID { get; set; }
        public string OPTION_NAME { get; set; }
    }
    public class SUP_STAFF_CATEGORY
    {
        public string STF_CATEGORY_ID { get; set; }
        public string STF_CATEGORY { get; set; }
    }

    public class SUP_UNIT
    {
        public string UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_SECTION
    {
        public string SECTION_ID { get; set; }
        public string SECTION { get; set; }
        public string SECTION_NAME { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_QUESTION_LEVEL
    {
        public string QUESTION_LEVEL_ID { get; set; }
        public string QUESTION_LEVEL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_EVALUATION
    {
        public string EVALUATION_ID { get; set; }
        public string EVALUATION_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETE { get; set; }
    }
    public class SUP_STATUS
    {
        public string STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string STATUS { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETE { get; set; }
    }

    public class SUP_QUIZ_TYPE
    {
        public string QUIZ_TYPE_ID { get; set; }
        public string QUIZ_TYPE_NAME { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_CERTIFICATE_TYPE
    {
        public string CERTIFICATE_ID { get; set; }
        public string CERTIFICATE_NAME { get; set; }
        public string SIGNATURE_1 { get; set; }
        public string SIGNATURE_2 { get; set; }
        public string APPROVER_1 { get; set; }
        public string APPROVER_2 { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }

    public class ARREAR_STUDENT_REGISTRATION
    {
        public string ARREAR_STUDENT_REGISTRATION_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_UNITS
    {
        public string UNIT_ID { get; set; }
        public string UNIT_NAME { get; set; }
    }
    public class SUP_METHODOLOGY
    {
        public string METHODOLOGY_ID { get; set; }
        public string METHODOLOGY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SUP_LMS_CLASS_NOTES
    {
        public string NOTES_ID { get; set; }
        public string NOTES_TITLE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string QUESTION_PAPER_ID { get; set; }
        public string PAPER_TITLE { get; set; }
        public string QUESTION_PAPER_TYPE_ID { get; set; }
        public string TYPE { get; set; }

        }

    public class SUP_PROGRAMME
    {
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

}   
    
    public class CONVOCATION_SETTING
    {
        public string SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DATE_FROM { get; set; }
        public string BATCH { get; set; }
        public string TO_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class CONVOCATION_REMARKS
    {
        public string REMARK_ID { get; set; }
        public string REMARK { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class CONVOCATION_SETTINGS
    {
        public string SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DATE_FROM { get; set; }
        public string BATCH { get; set; }
        public string TO_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
    }


