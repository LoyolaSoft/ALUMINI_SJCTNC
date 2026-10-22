using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{

    public class fee_payu_settlement_list
    {
        public string payuid { get; set; }
        public string txnid { get; set; }
        public string txndate { get; set; }
        public string mode { get; set; }
        public string amount { get; set; }
        public string requestid { get; set; }
        public string requestdate { get; set; }
        public string requestaction { get; set; }
        public string requestamount { get; set; }
        public string mer_utr { get; set; }
        public string mer_service_fee { get; set; }
        public string mer_service_tax { get; set; }
        public string mer_net_amount { get; set; }
        public string bank_name { get; set; }
        public string issuing_bank { get; set; }
        public string merchant_subvention_amount { get; set; }
        public string cgst { get; set; }
        public string igst { get; set; }
        public string sgst { get; set; }
        public string transaction_date { get; set; }
        public string settlement_date { get; set; }
        public string actual_settlement_date { get; set; }

    }

    public class SettlementInfo
    {
        public string status { get; set; }
        public string msg { get; set; }
        public List<fee_payu_settlement_list> Txn_details { get; set; }
    }

    public class PROGRAMME_HEADS_AMOUNT
    {
        public string HEAD { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD_ID { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string BATCH_ID { get; set; }
        public string AMOUNT { get; set; }
        public string PROGRAMME_HEAD_ID { get; set; }
        public string SEMESTER { get; set; }
        public string SUBJECT_TYPE { get; set; }

    }
    public class EXAM_COURSE_WISE_HEADS
    {
        public string COURSE_HEAD_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string PROGRAMME_HEAD_ID { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string BATCH_ID { get; set; }
        public string BATCH { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string AMOUNT { get; set; }
        public string ACCOUNT { get; set; }
        public string HEAD { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string HEAD_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string EXAM_NAME { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string LAST_DATE_OF_PAYMENT { get; set; }
    }
    public class JsonProgrammeHeads
    {
        public List<EXAM_PROGRAMME_WISE_HEADS> ProgrammeHeads { get; set; }

    }
    public class EXAM_PROGRAMME_WISE_HEADS
    {
        public string PROGRAMME_HEAD_ID { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string BATCH_ID { get; set; }
        public string AMOUNT { get; set; }
        public string HEAD_ID { get; set; }
        public string IS_USED { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string HEAD { get; set; }
        public string STATUS { get; set; }
        public string PROGRAMME { get; set; }
        public string HEAD_CODE { get; set; }
        public string ACCOUNT { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string BATCH { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }

    }

    public class CourseList
    {
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string COURSE_ID { get; set; }
        public string STATUS { get; set; }
        public string CLASS_ID { get; set; }
        public string SHIFT { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string STAFF_ID { get; set; }
        public string CLSCRSSTF_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
    }

    public class CourseInfo
    {
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TYPE { get; set; }
        public string TOTAL_STUDENT { get; set; }
        public string COURSE_TYPE_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string DATE { get; set; }
        public string STUDENT_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string MOBILE_NO { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string BATCH { get; set; }
        public string CLASSLEVEL { get; set; }

    }
    public class CourseComponents
    {
        public string CIA_GROUP_COMPONENT_ID { get; set; }
        public string MAX_MARK { get; set; }
        public string COMPONENT { get; set; }
        public string CIA_GROUP_ID { get; set; }
        public string COURSE_TYPE_ID { get; set; }
        public string GROUP_MARK { get; set; }
    }
    public class CIAMarks
    {
        public string COURSE_ROOT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string CIA_C1 { get; set; }
        public string CIA_C2 { get; set; }
        public string CIA_C3 { get; set; }
        public string CIA_C4 { get; set; }
        public string CIA_C5 { get; set; }
        public string CIA_C6 { get; set; }
        public string CIA_C7 { get; set; }
        public string CIA_C8 { get; set; }
        public string CIA_TOTAL { get; set; }
        public string MARK_ID { get; set; }
        public string CLASS_ID { get; set; }

    }
    public class ClassList
    {
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER { get; set; }
        public string CLASS_CODE { get; set; }
    }
    public class CIA_COMPONENT_MARK
    {
        public string CIA_GORUP_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CIA_GROUP_MARKS { get; set; }
        public string COMPONENT_MARK_ID { get; set; }
    }
    public class JSON_CIA_COMPONENT_MARKS
    {
        public List<CIA_COMPONENT_MARK> CIA_COMPONENT_MARKS { get; set; }
    }
    public class CIA_MARKS
    {
        public string CLASS_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string CIA_C1 { get; set; }
        public string CIA_C2 { get; set; }
        public string CIA_C3 { get; set; }
        public string CIA_C4 { get; set; }
        public string CIA_C5 { get; set; }
        public string CIA_C6 { get; set; }
        public string CIA_C7 { get; set; }
        public string CIA_C8 { get; set; }
        public string SEMESTER_ID { get; set; }
        public string CIA_TOTAL { get; set; }
        public string COURSE_ID { get; set; }
        public string MARK_ID { get; set; }
    }
    public class JSON_CIA_MARKS
    {
        public List<CIA_MARKS> CIA_MARKS { get; set; }
        public List<SAVE_CIA_MARKS> SAVE_CIA_MARKS { get; set; }
        public List<EXAM_TIMETABLE> liTimeTable { get; set; }
    }
    public class SAVE_CIA_MARKS
    {
        public string CIA_GROUP_MARK_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string COURSE_GROUP_MARK { get; set; }
        public string SEMESTER_ID { get; set; }
        public string INTERNAL_MARK_ID { get; set; }
        public string INTERNAL_MARK { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_DELETED { get; set; }
        public string AC_YEAR { get; set; }
        public string CLASS_ID { get; set; }

    }
    public class CIA_TOTAL_MARKS
    {
        public string ADMISSION_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string NAME { get; set; }
        public string CIA_TOTAL { get; set; }
    }
    public class CIA_COMPONENTS_MARK
    {

        public string ADMISSION_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string NAME { get; set; }
        public string CIA_C1 { get; set; }
        public string CIA_C2 { get; set; }
        public string CIA_C3 { get; set; }
        public string CIA_C4 { get; set; }
        public string CIA_C5 { get; set; }
        public string CIA_C6 { get; set; }
        public string CIA_C7 { get; set; }
        public string CIA_C8 { get; set; }
        public string CIA_TOTAL { get; set; }
    }
    public class CIA_COURSE_AND_CLASS_INFO
    {
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string COURSE_TYPE { get; set; }
        public string INTERNAL { get; set; }
        public string COMPONENT_ID { get; set; }
        public string COURSE_ID { get; set; }
    }

    // CIA Entry By Jeevan ... 
    public class CIA_FETCH_COURSE_INFO
    {
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string TOTAL_STUDENT { get; set; }
        public string COURSE_TYPE_ID { get; set; }
        public string COURSE_TYPE { get; set; }
        public string CLASS_NAME { get; set; }
        public string SEMESTER_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_ID { get; set; }
        public string SEMESTER { get; set; }
        public string STAFF_ID { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string CLASS_TYPE { get; set; }

    }
    public class CIA_MARKS_INFO
    {
        public string CIA_GROUP_MARK_ID { get; set; }
        public string NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string ADMISSION_NO { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string MAX_MARK { get; set; }
        public string ORDER_ID { get; set; }
        public string CIA_GROUP_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public string COMPONENT { get; set; }
        public string COURSE_GROUP_MARK { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string REMARK { get; set; }
        public string PART { get; set; }
        public string PART_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_ORDER { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string DEPARTMENT { get; set; }
        public string STAFF_ID { get; set; }
        public string CLSCRSSTF_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string STATUS { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string OBE_COMPONENT_ID { get; set; }
        public string IS_ABSENT { get; set; }
        public string OBE_ABSENT_ID { get; set; }
        public string DUMMY_NO { get; set; }
        public string PACKET_NO { get; set; }
        public string COURSE_TYPE_ID { get; set; }

    }
    public class CIA_COMPONENT
    {
        public string CIA_GROUP_COMPONENT_ID { get; set; }
        public string MAX_MARK { get; set; }
        public string COMPONENT { get; set; }
        public string COMPONENT_ID { get; set; }
        public string COURSE_TYPE_ID { get; set; }
        public string GROUP_MARK { get; set; }
        public string CIA_GROUP_ID { get; set; }
        public string ORDER_ID { get; set; }
        public string INTERNAL { get; set; }
        public string DISABLED { get; set; }
        public string LAST_DATE { get; set; }
        public string VALIDATE { get; set; }


    }
    public class CIA_TOTAL
    {
        public string CLASS_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string TOTAL { get; set; }
        public string COURSE_ID { get; set; }
        public string ADMISSION_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string CLASS_YEAR { get; set; }
        public string ROLL_NO { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string COURSE_GROUP_MARK { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string CIA_GROUP_MARK_CHECKING_ID { get; set; }
        public string STATUS { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TYPE { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string BATCH { get; set; }
        public string CLASSLEVEL { get; set; }
    }

    public class CIA_GROUP_MARK_CHECKING
    {
        public string CIA_GROUP_MARK_CHECKING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string COURSE_GROUP_MARK { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STATUS { get; set; }

        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }

        public string CLASS_NAME { get; set; }
        public string COURSE_TITLE { get; set; }

        public string SEMESTER { get; set; }
        public string BATCH { get; set; }
    }

    // CIA Marks List ..
    public class CIA_STAFF_INFO
    {
        public string DEPARTMENT { get; set; }
        public string ORDER_DATE { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string STAFF_ALLOTED_ID { get; set; }
        public string DUE_DATE { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string INTERNAL { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string COURSE_TYPE { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_ID { get; set; }
    }
    // Exam Registration ...
    public class EXAM_REGISTRATION
    {
        public string EXAM_SETTING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_TYPE { get; set; }
        public string IS_STUDENT_VIEW_CIA_MARK { get; set; }
        public string RESULT { get; set; }
        public string PACKET_NO { get; set; }
        public string PACKET_REFERENCE_NO { get; set; }
        public string IS_COMPULSORY { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string MOBILE_NO { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string PROGRAMME { get; set; }
        public string BATCH { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string DURATION_UNIT { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER { get; set; }
        public string AC_YEAR { get; set; }
        public string YEAR { get; set; }
        public string DATE { get; set; }
        public string MONTH { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string SUBJECT_TYPE_ID { get; set; }
        public string STATUS { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string TOTAL { get; set; }
        public string PAID { get; set; }
        public string BALANCE { get; set; }
        public string COURSE_STRUCTURE_ID { get; set; }
        public string CREDITS { get; set; }
        public string PART { get; set; }
        public string STAFF_CODE { get; set; }
        public string STAFF_ID { get; set; }
        public string CS_SIGN_PATH { get; set; }
        public string HOD_SIGN_PATH { get; set; }
        public string SIGN_PATH { get; set; }
        public string MAPPING_ID { get; set; }

        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string CUR_DATE { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string COST { get; set; }
        public string REGISTRATION_ID { get; set; }
        public string S_CLASS_ID { get; set; }
        public string CIA_GROUP_MARK_CHECKING_ID { get; set; }
        public string INTERNAL { get; set; }
        public string COURSE_ORDER { get; set; }
        public string NAME_IN_NATIVE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string AMOUNT { get; set; }
        public string IS_VIRIFIED { get; set; }
        public string UNREADCOUNT { get; set; }
        public string IS_ATTENDED { get; set; }


    }
    public class EXAM_REGISTRATION_LIST
    {
        public string STUDENT_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string IS_COMPULSORY { get; set; }
    }
    public class GET_ACTIVE_SEMESTER_BATCH
    {
        public string BATCH { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string SEMESTER { get; set; }
        public string IS_ACTIVE { get; set; }
        public string STUDENT_ID { get; set; }
        public string PROGRAMME { get; set; }
    }
    public class EXAM_INVIGILATION_STAFF_WISE
    {
        public string EXAM_INVIGILATION_ID { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_INVIGILATOR_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string HALL_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string SESSION { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string SESSION_NAME { get; set; }
        public string EXAM_NAME { get; set; }
        public string M_EXAM { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string IS_DELETED { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
    }
    public class DateSessionWiseInvigilationGroup
    {
        public string ExamDate { get; set; }
        public string SessionId { get; set; }
        public string SessionName { get; set; }
        public string SessionDisplayName { get; set; }
        public string ExamTitle { get; set; }
        public List<List<EXAM_INVIGILATION_STAFF_WISE>> Pages { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (Pages != null) ? Pages.Count : 0;
    }
    public class GET_PROGRAMME_AND_BATCH
    {
        public string STUDENT_ID { get; set; }
        public string PROGRAMME { get; set; }
        public string BATCH { get; set; }
        public string SEMESTER_ID { get; set; }
        public string CLASS_YEAR { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string APPTYPE_ID { get; set; }
    }
    public class SEMESTER_RESULT
    {
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string RESULT { get; set; }
        public string STATUS { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER { get; set; }
        public string AC_YEAR { get; set; }
        public string PASSED { get; set; }
        public string FAILED { get; set; }
        public string WRITTEN_SEMESTER { get; set; }
        public string SUBJECT_TYPE_ID { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string AMOUNT { get; set; }
        public string INTERNAL_MARK { get; set; }
        public string EXTERNAL_MARK { get; set; }
        public string TOTAL { get; set; }
        public string CREDIT { get; set; }
        public string PART { get; set; }
        public string M_PASS { get; set; }
        public string GRADE { get; set; }
        public string GRADE_POINT { get; set; }
        public string CLASS_YEAR { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string APPLIED { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_TYPE { get; set; }
        public string NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string CIA_RESULT { get; set; }

    }

    // Save Subject Details ..
    public class SaveExamDetails
    {
        public List<SEMESTER_RESULT> SubjectDetails { get; set; }
        public List<EXAM_QUESTION_PAPER> ExamQuestionpaper { get; set; }
        public List<EXAM_INVIGILATION> liExamInvigilation { get; set; }
        public List<EXAM_EVALUATION> liJsonExamEvaluation { get; set; }
        public List<EXAM_REGISTRATION> liJsonExamRegistration { get; set; }
        public List<ListStudents> listudentfolionumber { get; set; }


    }
    public class EXAM_REGISTRATION_2017
    {
        public string REGISTRATION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string ADMISSION_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string EXAM_SEMESTER { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string TOTAL_COUNT { get; set; }
        public string EXAM_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string CREDIT { get; set; }
        public string SEMESTER { get; set; }
        public string M_PASS { get; set; }
        public string IS_ARREAR { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string CLASS_YEAR { get; set; }
        public string TOTAL { get; set; }

    }
    public class ExamRegistrationModel
    {
        public string REGISTRATION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class FetchHallTicket
    {
        public string REGISTRATION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STATUS { get; set; }
        public string AC_YEAR { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_YEAR { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_TIME { get; set; }
        public string SEAT_CODE { get; set; }
        public string SEAT_POSITION { get; set; }
        public string BLOCK_NAME { get; set; }
        public string BLOCK_CODE { get; set; }
        public string PHOTO { get; set; }
        public string SESSION_NAME { get; set; }
        public string IS_VIRIFIED { get; set; }
        public string AMOUNT { get; set; }
    }
    public class STU_HALL_TICKET
    {
        public string ROLL_NO { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STATUS { get; set; }
        public string AC_YEAR { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MONTH_YEAR { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_TIME { get; set; }
        public string SESSION_NAME { get; set; }
        public string HALL_NAME { get; set; }
        public string DATE_OF_EXAM { get; set; }
        public string ELIGIBLE_TYPE { get; set; }
        public string PHOTO { get; set; }
        public string CLASS_NAME { get; set; }
        public string UMIS_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string EXAM_NAME { get; set; }
        public string HALL_TICKET_PUBLICATION_DATE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string HALL_TICKET_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
    }
    public class HallTicketDetails
    {
        public string DEPARTMENT { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEGREE { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string SEMESTER { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string YEAR { get; set; }
        public string CLASS_NAME { get; set; }
        public string PHOTO { get; set; }

    }

    public class EXAM_SETTING
    {
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string BATCH  {get; set; }
        public string M_EXAM { get; set; }
        public string M_PASS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string LAST_DATE_FOR_EXAM_FEE { get; set; }
        public string HALL_TICKET_PUBLICATION_DATE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string EXAM_TYPE { get; set; }
        public string PROGRAMME_TYPE_IDS { get; set; }
        public string SCRIPT_COUNT { get; set; }
        public string DUMMY_NO { get; set; }
        public string ONLINE_EXAM_SETTING_ID { get; set; }
    }
    public class COUNT_STATUS_BY_COURSE
    {
        public string COURSE_ID { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string STATUS { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string GENDER { get; set; }

    }
    [Serializable()]
    public class CollegeDetails
    {
        public string COLLEGE_NAME { get; set; }
        public string UNIVERSITY { get; set; }
        public string GRADE { get; set; }
        public string COLLEGE_TYPE { get; set; }
        public string PLACE { get; set; }
        public string COLLEGE_LOGO { get; set; }
        public string COLLEGE_CODE { get; set; }
        public string LOGIN_IMG_PATH { get; set; }
        public string COLLEGE_ADDRESS { get; set; }
        public string DISTRICT { get; set; }

    }
    public class ExamControllerPrint
    {
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_TYPE { get; set; }
        public string STATUS { get; set; }
        public string BATCH { get; set; }
        public string CLASS_ID { get; set; }
    }

    // Online Quiz ..
    public class LIST_QUIZ
    {
        public string QUESTION { get; set; }
        public string QUESTION_TYPE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string QUESTION_ID { get; set; }
        public string ANSWER { get; set; }
        public string CLASS_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string QUIZ_ID { get; set; }
        public string SET_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string MARK { get; set; }
    }
    public class QUIZ_QUESTIONS_2017
    {
        //public string PRIMARY_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string OPTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string OPTIONS { get; set; }
    }
    public class QUIZ_OPTIONS_2017
    {
        public string OPTION_ID { get; set; }
        public string OPTION { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string SETTING_ID { get; set; }
        public string MARK { get; set; }
        public string SET_ID { get; set; }
        public string QUIZ_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class JSON_QUIZ_OPTIONS
    {
        public List<QUIZ_OPTIONS_2017> liQuizOption { get; set; }
    }
    public class QUIZ_2017
    {
        public string QUIZ_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string SELECTED_ANSWER { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class JSON_SAVE_QUIZ
    {
        public List<QUIZ_2017> ObjSaveQuizAnswer { get; set; }
        public List<QUIZ_MATCH_THE_FOLLOWING_OPTIONS> liMF_Option { get; set; }
        public List<QUIZ_SAVE_MATCH_THE_FOLLOWING> liOnlineQuiz { get; set; }
        public List<QUIZ_STUDENT_SHUFFLE> liQuizStudentShuffle { get; set; }
    }
    public class JSON_SAVE_EXAM_ANSWER
    {
        public List<EXAM_OBJECTIVE_QUESTIONS> liOnlineExamanswer { get; set; }
        public List<EXAM_EVALUATION_MARKS> liEvaluationmark { get; set; }
    }
    public class QUIZ_RESULT
    {
        public string SELECTED { get; set; }
        public string COUNT { get; set; }
        public string CHECKED { get; set; }
        public string QUESTION_ID { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string SELECTED_ANSWER { get; set; }
        public string FIRST_NAME { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string STATUS { get; set; }
        public string DATE { get; set; }
        public string Q_TYPE { get; set; }
        public string QUIZ_ID { get; set; }
        public string CORRECT_ANSWER { get; set; }
    
    }

    public class Cia_Status
    {
        public string UPLOAD_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string COMPONENT_STATUS1 { get; set; }
        public string STATUS1 { get; set; }
        public string COMPONENT_STATUS2 { get; set; }
        public string STATUS2 { get; set; }
        public string COMPONENT_STATUS3 { get; set; }
        public string STATUS3 { get; set; }
        public string COMPONENT_STATUS4 { get; set; }
        public string STATUS4 { get; set; }
        public string COMPONENT_STATUS5 { get; set; }
        public string STATUS5 { get; set; }
        public string COMPONENT_STATUS6 { get; set; }
        public string STATUS6 { get; set; }
        public string COMPONENT_STATUS7 { get; set; }
        public string STATUS7 { get; set; }
        public string COMPONENT_STATUS8 { get; set; }
        public string STATUS8 { get; set; }
        public string COMPONENT_STATUS9 { get; set; }
        public string STATUS9 { get; set; }
        public string COMPONENT_STATUS10 { get; set; }
        public string STATUS10 { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE { get; set; }
        public string NAME { get; set; }
        public string SEMESTER_NAME { get; set; }
    }

    public class FETCH_CIA_MARKS
    {
        public string STAFF_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string STUDENT_ID { get; set; }
    }

    public class SEMESTER_MARKS
    {
        public string SEMESTER_MARK_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string INTERNAL_MARK { get; set; }
        public string EXTERNAL_MARK { get; set; }
        public string TOTAL { get; set; }
        public string GRADE { get; set; }
        public string GRADE_POINT { get; set; }
        public string CREDIT { get; set; }
        public string RESULT { get; set; }
        public string APPLIED { get; set; }
        public string SEMESTER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string M_PASS { get; set; }
        public string IS_ARREAR { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_REVALUATION_SETTING_ID { get; set; }
        public string EXAM_PHOTOCOPY_SETTING_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string NAME { get; set; }
        public string PART { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string PART_NAME { get; set; }
        public string MAXIMUM { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string COURSE_ORDER { get; set; }
        public string M_EXAM { get; set; }
        public string START_DATE { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string BATCH_ID { get; set; }
        public string INTERNAL { get; set; }
        public string EXTERNAL { get; set; }
        public string TOTAL_MARK { get; set; }
        public string DEPARTMENT { get; set; }
        public string BATCH { get; set; }
        public string DAY_EVEN { get; set; }
        public string DEGREE { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string ENTRY_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string PHOTO { get; set; }

        public string CERTIFICATE_TYPE { get; set; }

        public string BEGIN_YEAR { get; set; }
        public string EXPIRED_YEAR { get; set; }
        public string BAR_CODE { get; set; }

        public string CLASS_YEAR { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string BRANCH { get; set; }
        public string ACADMICE_SEMESTER { get; set; }
        public string PROVISIONAL { get; set; }
        public string CONVOCATON { get; set; }
        public string LANGUAGE_NAME { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string AC_YEAR { get; set; }

        //Extra
        public string TAMIL_NAME { get; set; }
        public string PROGRAMME_NAME_1 { get; set; }
        public string PROGRAMME_NAME_2 { get; set; }

        public string PART_III_CLASS { get; set; }
        public string UNIVERSITY_CODE { get; set; }
        public string APPTYPE_ID { get; set; }
        public string PERCENTAGE { get; set; }
        public string GENDER_NAME { get; set; }
        public string SHIFT_NAME { get; set; }
        public string CGPA { get; set; }
        public string GP { get; set; }
        public string GROUP { get; set; }
        public string IS_CONVERSION { get; set; }
        public string ENTRY_MARK { get; set; }
        public string MARK_LENGTH { get; set; }
        public string CONVERT_MARK { get; set; }
        public string DUMMY_NO { get; set; }
        public string EXTERNAL_MARK_ID { get; set; }
        public string IS_REVALUATION { get; set; }
        public string TOPPER_ID { get; set; }
        public string MAX_MARK { get; set; }
        public string NAD_FIELD_ID { get; set; }
        public string NAD_FIELD_NAME { get; set; }
        public string FIELD_ORDER { get; set; }
        public string ORG_NAME { get; set; }
        public string SUB1_GRADE_POINTS { get; set; }
        public string SUB1_CREDIT { get; set; }
        public string SUB1_CREDIT_POINTS { get; set; }
        public string SUB1_REMARKS { get; set; }
        public string SUB1_VV_MIN { get; set; }
        public string SUB1_VV_MAX { get; set; }
        public string SUB1_TH_CE_MRKS { get; set; }
        public string SUB1_CREDIT_ELIGIBILITY { get; set; }
        public string AADHAAR_NAME { get; set; }
        public string SUB1MIN { get; set; }
        public string SUB1_TH_MAX { get; set; }
        public string SUB1_VV_MRKS { get; set; }
        public string SUB1_PR_CE_MRKS { get; set; }
        public string SUB1_TH_MIN { get; set; }
        public string SUB1_PR_MAX { get; set; }
        public string SUB1_PR_MIN { get; set; }
        public string SUB1_CE_MAX { get; set; }
        public string SUB1_CE_MIN { get; set; }
        public string SUB1_TH_MRKS { get; set; }
        public string SUB1_PR_MRKS { get; set; }
        public string SUB1_CE_MRKS { get; set; }
        public string SUB1_TOT { get; set; }
        public string SUB1_GRADE { get; set; }
        public string TOT_VV_MIN { get; set; }
        public string TOT_VV_MRKS { get; set; }
        public string TOT_CREDIT { get; set; }
        public string TOT_CREDIT_POINTS { get; set; }
        public string TOT_GRADE_POINTS { get; set; }
        public string PREV_TOT_MRKS { get; set; }
        public string GRAND_TOT_MAX { get; set; }
        public string GRAND_TOT_MIN { get; set; }
        public string GRAND_TOT_MRKS { get; set; }
        public string GRAND_TOT_CREDIT { get; set; }
        public string REMARKS { get; set; }
        public string SGPA { get; set; }
        public string ABC_ACCOUNT_ID { get; set; }
        public string TERM_TYPE { get; set; }
        public string TOT_GRADE { get; set; }
        public string SUB1NM { get; set; }
        public string SUB1 { get; set; }
        public string SUB1MAX { get; set; }
        public string SEM { get; set; }
        public string EXAM_TYPE { get; set; }
        public string TOT { get; set; }
        public string TOT_MIN { get; set; }
        public string TOT_MRKS { get; set; }
        public string TOT_TH_MAX { get; set; }
        public string TOT_TH_MIN { get; set; }
        public string TOT_TH_MRKS { get; set; }
        public string TOT_PR_MAX { get; set; }
        public string TOT_PR_MIN { get; set; }
        public string TOT_PR_MRKS { get; set; }
        public string TOT_CE_MAX { get; set; }
        public string TOT_CE_MIN { get; set; }
        public string TOT_CE_MRKS { get; set; }
        public string TOT_VV_MAX { get; set; }
        public string ACADEMIC_COURSE_ID { get; set; }
        public string COURSE_NAME { get; set; }
        public string STREAM { get; set; }
        public string REGN_NO { get; set; }
        public string RROLL { get; set; }
        public string CNAME { get; set; }
        public string DOB { get; set; }
        public string FNAM { get; set; }
        public string MNAME { get; set; }
        public string MRKS_REC_STATUS { get; set; }
        public string YEAR { get; set; }
        public string PERCENT { get; set; }
        public string DOI { get; set; }
        public string CERT_NO { get; set; }
        public string IS_PRACTICAL { get; set; }

    }

    public class SUP_BATCH_DURATION
    {
        public string DURATION_ID { get; set; }
        public string BATCH_ID { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string DURATION { get; set; }
        public string BEGIN_YEAR { get; set; }
        public string EXPIRED_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string CLASS_ID { get; set; }
    }
    public class JSON_SEMESTER_MARK
    {
        public List<SEMESTER_MARKS> JSON_LISEMESTER_MARKS { get; set; }
    }
    public class SemesterMarks
    {
        public string SHIFT { get; set; }
        public string CLASS { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string HOUR { get; set; }
        public string ROLL_NO { get; set; }
        public string NAME { get; set; }
    }

    public class QUIZ_MATCH_THE_FOLLOWING_QUESTIONS
    {
        public string QUIZ_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ANSWER_ID { get; set; }
        public string ANSWER { get; set; }
        public string QUESTION { get; set; }
        public string MARK { get; set; }
        public string SET_ID { get; set; }
        public string QUESTION_TYPE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string NO_OF_QUESTIONS { get; set; }
    }

    public class QUIZ_MATCH_THE_FOLLOWING_OPTIONS : QUIZ_MATCH_THE_FOLLOWING_QUESTIONS
    {
        public string OPTION_ID { get; set; }
        public string OPTION { get; set; }
        public string OPTIONS { get; set; }
        public string QUESTION_ID { get; set; }
        //public string ANSWER { get; set; }
        // public string MARK { get; set; }
        //public string QUIZ_ID { get; set; }
        //public string SET_ID { get; set; }
        //public string SETTING_ID { get; set; }
        //public string ACADEMIC_YEAR { get; set; }
        //public string SEMESTER_ID { get; set; }
        //public string IS_ACTIVE { get; set; }
        //public string IS_DELETED { get; set; }
    }

    public class QUIZ_SETTING
    {
        public string SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string QUIZ_TYPE_ID { get; set; }
        public string QUIZ_STF_ID { get; set; }
        public string NO_OF_QUESTIONS { get; set; }
        public string EACH_QUESTION_MARKS { get; set; }
        public string TOTAL_MARK { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACTIVE_DATE { get; set; }
        public string TIME { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string STAFF_ID { get; set; }
        public string STATUS { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_CORRECT { get; set; }
        public string IS_SHOW_MARKS { get; set; }
        public string IS_SHOW_RIGHT_ANSWER { get; set; }
    }

    public class QUIZ_STUDENT_SHUFFLE
    {
        public string QUIZ_SHUFFLE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string ENTRY_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string SET_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class EXAM_TIMETABLE
    {
        public string TIMETABLE_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_TIME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ADVANCE { get; set; }
        public string ADVANCE_DATE { get; set; }
        public string ADVANCE_TIME { get; set; }
        public string IS_POSTPONED { get; set; }
        public string POSTPONED_DATE { get; set; }
        public string POSTPONED_TIME { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ARREAR_SETTING_ID { get; set; }
        public string IS_UPDATED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        // Exam Filed 
        public string COURSE_TITLE { get; set; }
        public string CLASS_NAME { get; set; }
        public string BATCH { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string SEMESTER { get; set; }
        public string EXAM_SESSION_ID { get; set; }
        public string EXAM_SESSION { get; set; }
        public string SEMESTER_ID { get; set; }
        public string UGC_COURSE_TYPE_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string EXAM_NAME { get; set; }
        public string EXAM_TYPE { get; set; }

        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string EXAM_SEATING_EXCEPTOIN_COURSE_ID { get; set; }
        public string GENDER { get; set; }
        public string STATUS { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string SHIFT { get; set; }
        public string ONLINE_EXAM_SETTING_ID { get; set; }

    }

    public class ListQuizStudent
    {
        public string QUIZ_SHUFFLE_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string SET_ID { get; set; }
        public string SET_NAME { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string STUDENT_ID { get; set; }
    }

    public class OnlineQuizTest
    {
        public string QUIZ_SAVE_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string QUIZ_QUESTION { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string QUESTION { get; set; }
        public string QUESTION_COUNT { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string ANSWER { get; set; }
        public string SET_ID { get; set; }
        public string SET_NAME { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string SETTING_ID { get; set; }
        public string CLASSS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string QUIZ_ID { get; set; }
        public string OPTION_ID { get; set; }
        public string OPTION { get; set; }
        public string STATUS { get; set; }
        public string SELECTED_ANSWER { get; set; }
        public string MARK { get; set; }
        public string ANSWER_ID { get; set; }
        public string IS_ATTEMPT { get; set; }
    }

    public class QUIZ_SAVE_MATCH_THE_FOLLOWING
    {
        public string QUIZ_SAVE_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string DATE { get; set; }
        public string STUDENT_ID { get; set; }
        public string OPTION_ID { get; set; }
        public string ANSWER_ID { get; set; }
        public string STATUS { get; set; }
        public string SETTING_ID { get; set; }
        public string SET_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string QUIZ_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string MINUTES { get; set; }

    }

    public class QUIZ_CLASS_COURSE_STAFF : QUIZ_SETTING
    {
        public string SEMESTER_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE { get; set; }
        public string FIRST_NAME { get; set; }
        public string SET_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COUNT { get; set; }
        public string MINUTES { get; set; }
        public string QUIZ_TYPE_NAME { get; set; }
        // public string QUIZ_TYPE_ID { get; set; }


    }


    public class ONLINE_EXAM_SETTING
    {
        public string EXAM_SETTING_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string TIME { get; set; }
        public string SESSION_ID { get; set; }
        public string SESSION_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STAFF_ID { get; set; }
        public string NO_OF_QUESTIONS { get; set; }
        public string QUESTIONS_TO_ATTEN { get; set; }
        public string EACH_QUESTION_MARKS { get; set; }
        public string TOTAL_MARK { get; set; }
        public string SEMESTER_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class ONLINE_EXAM_QUESTIONS
    {
        public string EXAM_QUESTIONS_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string QUESTION { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STAFF_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class ONLINE_EXAM_ANSWERS
    {
        public string EXAM_ANSWER_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string QUESTION_ID { get; set; }
        public string ANSWER { get; set; }
        public string MARK { get; set; }
        public string EVALUATOR_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string TIME_DURATION { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class ONLINE_EXAM_SUBMITTED
    {
        public string SUBMIT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string TIME_DURATION { get; set; }
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class ListExamination
    {
        public string EXAM_SETTING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string ADMISSION_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string PHOTO { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string EXAM_QUESTIONS_ID { get; set; }
        public string QUESTIONS { get; set; }
        public string EXAM_DATE { get; set; }
        public string TIME { get; set; }
        public string SESSION_NAME { get; set; }
        public string NO_OF_QUESTIONS { get; set; }
        public string QUESTIONS_TO_ATTEN { get; set; }
        public string EACH_QUESTION_MARKS { get; set; }
        public string TOTAL_MARK { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DEGREE { get; set; }
    }
    public class QUIZ_STUDENT_MARKLIST
    {
        public string STAFF_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string QUESTION_TYPE { get; set; }
        public string QUESTION_COUNT { get; set; }
        public string STATUS { get; set; }
        public string MARK { get; set; }
    }

    public class QUIZ_CERTIFICATE_RESULT
    {
        public string SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string TOTAL_MARK { get; set; }
        public string SCORE_MARK { get; set; }
        public string PERCENTAGE { get; set; }
        public string STUDENT_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string COURSE_ID { get; set; }
    }



    public class JSON_SAVE_ONLINE_EXAMINFO
    {
        public List<ONLINE_EXAM_QUESTIONS> liExamQuestions { get; set; }
        public List<ONLINE_EXAM_ANSWERS> liExamAnswers { get; set; }
        public List<EXAM_COURSE_WISE_COST> liExamCourseWiseCost { get; set; }
        public List<EXAM_HEADS> liExamHeads { get; set; }
    }

    public class EXAM_COURSE_WISE_COST
    {
        public string COURSE_WISE_COST_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string TITLE { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string PROGRAMME_TYPE { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string EXAM_TYPE_ID { get; set; }
        public string EXAM_TYPE { get; set; }
        public string UGC_TYPE_COST_ID { get; set; }
        public string UGC_TYPE_COST { get; set; }
        public string UGC_COURSE_TYPE_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string COST { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SUBJECT_TYPE_ID { get; set; }
        public string SUBJECT_TYPE { get; set; }

        // Extra ...,
        public string INTERNAL_COST_ID { get; set; }

    }

    public class EXAM_UGC_COURSE_TYPE_COST
    {
        public string UGC_TYPE_COST_ID { get; set; }
        public string TITLE { get; set; }
        public string UGC_COURSE_TYPE_ID { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COST { get; set; }
        public string STAFF_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class LIST_UGC_COURSE_TYPE_COST
    {
        public string UGC_TYPE_COST_ID { get; set; }
        public string TITLE { get; set; }
        public string UGC_COURSE_TYPE_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COST { get; set; }
        public string STAFF_ID { get; set; }
        public string EXAM_NAME { get; set; }
        public string LAST_DATE_FOR_EXAM_FEE { get; set; }
        public string ENTRY_DATE { get; set; }
    }

    public class EXAM_HEADS
    {
        public string EXAM_HEAD_ID { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string SHIFT_ID { get; set; }
        public string SHIFT_NAME { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string HEAD { get; set; }
        public string COST { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class SaveExamSubjects
    {
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string FEE_FREQUENCY_FEE_MODE_ID { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string HEAD_ID { get; set; }
        public string EXAM_HEAD_ID { get; set; }
        public string COST { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
    }

    public class ListArrerPapers
    {
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string BATCH { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string SHIFT { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string RESULT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_WISE_COST_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string PROGRAMME_TYPE { get; set; }
        public string UGC_TYPE_COST_ID { get; set; }
        public string FINE { get; set; }
        public string DEPARTMENT { get; set; }
    }
    public class ONLINEEXAMINATION_REPORTFIELDS
    {
        public string STUDENT_ID { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ENTRY_TIME { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_TITLE { get; set; }
        public string SEMESTER_NAME { get; set; }

    }
    public class EXAM_SEATINGS
    {
        public string SEATING_ID { get; set; }
        public string ROOM_ID { get; set; }
        public string COLUMN_NO { get; set; }
        public string ROW_NO { get; set; }
        public string SEAT_CODE { get; set; }
        public string SEAT_POSITION { get; set; }
        public string EXAM_TYPE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string SEATING_ALLOTEMENT_ID { get; set; }
        public SUP_ROOM SUP_ROOM { get; set; }
    }
    public class EXAM_SEATING_ALLOTEMENT
    {
        public string SEATING_ALLOTEMENT_ID { get; set; }
        public string SEATING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_SESSION_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ARREAR_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string REGISTER_NO { get; set; }
        public string CLASS_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string SEAT_POSITION { get; set; }
        public string SEAT_CODE { get; set; }
        public string EXAM_CAPACITY { get; set; }
        public string EXAM_SESSION { get; set; }
        public string ROW { get; set; }
        public string COLUMN { get; set; }
        public string ROOM_NO { get; set; }
        public string BLOCK_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string EXAM_TYPE { get; set; }
        public string ROOM_ID { get; set; }
        public string ROW_NO { get; set; }
        public string COLUMN_NO { get; set; }
        public string BLOCK_CODE { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string CLASS_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string BLOCK_ORDER { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string ROOM_ORDER { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string SPACE_COLUMN { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string GENDER { get; set; }
    }
    public class EXAM_OVERALL_FEE_REPORT
    {
        public string EXAM_SETTING_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD { get; set; }
        public string HEAD { get; set; }
        public string COST { get; set; }
        public string COURSE_ID { get; set; }
        public string TITLE { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string PROGRAMME_TYPE_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME { get; set; }

    }

    public class EXAM_SEATING_INVIGILATOR
    {
        public string SEATING_INVIGILATOR_ID { get; set; }
        public string ROOM_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_SESSION_ID { get; set; }
        public string EXAM_SESSION { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ROOM_NO { get; set; }
        public string TOTAL_STAFF { get; set; }
        public string DEPARTMENT { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string SUPERVISION_COUNT { get; set; }
        public string SUPERVISION_TYPE { get; set; }
        public string SHIFT { get; set; }
        public string EXAM_CAPACITY { get; set; }
        public string BLOCK_NAME { get; set; }

    }
    public class EXAM_INVIGILATOR_FIELDS
    {
        public string SEATING_INVIGILATOR_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string ROOM_NO { get; set; }
        public string ROOM_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_SESSION { get; set; }
        public string EXAM_NAME { get; set; }
        public string EXAM_TYPE_NAME { get; set; }
    }
    public class DUMMY_HALLTICKET_DISTRIBUTED
    {
        public string DUM_HALLTICKET_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string DISTRIBUTED_ID { get; set; }
        public string DATE_TIME { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }

    public class EXAM_REVALUATION_SETTING
    {
        public string EXAM_REVALUATION_SETTING_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_SETTING { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string MAX_PAPER { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
    }

    public class EXAM_REVALUATION_REGISTRATION
    {
        public string EXAM_REVALUATION_REGISTER_ID { get; set; }
        public string EXAM_REVALUATION_SETTING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string PAPERS { get; set; }
        public string AMOUNT { get; set; }
        public string COURSE_ID { get; set; }
        public string REGISTERED_DATE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string EXAM_SETTING { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string FREQUENCY { get; set; }

    }

    public class REVALUATION_FIELDS
    {
        public List<EXAM_REVALUATION_REGISTRATION> liRevaluationRegistration { get; set; }
        public List<SEMESTER_MARKS> liSemesterMarks { get; set; }
    }


    public class EXAM_REVALUATION_REGISTRATION_FIELDS
    {
        public string EXAM_SETTING { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_REVALUATION_SETTING_ID { get; set; }
        public string EXAM_PHOTOCOPY_SETTING_ID { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string MAX_PAPER { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string EXAM_REVALUATION_REGISTER_ID { get; set; }
        public string EXAM_PHOTOCOPY_REGISTER_ID { get; set; }
        public string REGISTERED_DATE { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }

        public string PROGRAMME_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string PAPERS { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string EXAM_PHOTOCOPY_FILE_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string BALANCE { get; set; }

    }



    public class EXAM_PHOTOCOPY_SETTING
    {
        public string EXAM_PHOTOCOPY_SETTING_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_PHOTOCOPY_REGISTER_ID { get; set; }
        public string EXAM_SETTING { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string MAX_PAPER { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }

    }


    public class EXAM_PHOTOCOPY_REGISTRATION
    {
        public string EXAM_PHOTOCOPY_REGISTER_ID { get; set; }
        public string EXAM_PHOTOCOPY_SETTING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string PAPERS { get; set; }
        public string AMOUNT { get; set; }
        public string COURSE_ID { get; set; }
        public string REGISTERED_DATE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string EXAM_SETTING { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
    }


    public class EXAM_PHOTOCOPY_FILES
    {
        public string EXAM_PHOTOCOPY_FILE_ID { get; set; }
        public string EXAM_PHOTOCOPY_REGISTER_ID { get; set; }
        public string PHOTOCOPY { get; set; }
        public string COURSE_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }

    public class JSON_FILEDS
    {
        public List<EXAM_PHOTOCOPY_FILES> liExamPhotoCopyFiles { get; set; }
        public List<CIA_GROUP_MARK_CHECKING> liCiaGroupChecking { get; set; }
    }

    public class EXAM_PHOTOCOPY_FIELDS
    {
        public string STUDENT_ID { get; set; }
        public string EXAM_PHOTOCOPY_REGISTER_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string PHOTOCOPY { get; set; }
        public string EXAM_PHOTOCOPY_FILE_ID { get; set; }
        public string EXAM_PHOTOCOPY_SETTING_ID { get; set; }
    }

    public class MPHILL_STUDENT_PROJECT
    {
        public string M_PROJECT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string PROJECT_TITLE { get; set; }
        public string COURSE_CODE { get; set; }

        public string EXAM_SETTINEG_ID { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class EXAM_EVALUATION_STUDENT_MARK
    {
        public string EXAM_EV_STU_MARK_ID { get; set; }
        public string EVALUATION_COURSE_SETTING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_EVALUATOR_STAFF_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EVALUATION_1 { get; set; }
        public string EVALUATION { get; set; }
        public string EVALUATION_2 { get; set; }
        public string EVALUATION_3 { get; set; }
        public string TOTAL { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACDEMIC_YEAR { get; set; }
        public string REGISTER_NO { get; set; }
        public string FROM_REGISTER_NO { get; set; }
        public string TO_REGISTER_NO { get; set; }
        public string COURSE_CODE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EVALUATION_DATE { get; set; }
        public string ROLE { get; set; }
        public string EXTERNAL { get; set; }
        public string COURSE_TITLE { get; set; }
        public string INTERNAL { get; set; }
        public string MAX_MARK { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string EXAM_MARK { get; set; }
        public string IS_EXTERNAL_INTERNAL { get; set; }
        public string PHONE { get; set; }
        public string NAME { get; set; }
        public string M_EXAM { get; set; }
        public string IS_DATE_VALIDATION { get; set; }
        public string DUMMY_NO { get; set; }
        public string EXAM_BUNDLE_NO { get; set; }
        public string POCKET_NO { get; set; }
        public string POCKET_CATEGORY { get; set; }
        public string IS_ARREAR { get; set; }
        public string EXAM_BUNDLE_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string BUNDLE_ID { get; set; }
        public string EVALUATION_MARK { get; set; }
        public string BUNDLE_STUDENT_ID { get; set; }

    }
    public class EXAM_EVALUATION_COURSE_SETTING
    {
        public string EVALUATION_COURSE_SETTING_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EVALUTION_DATE { get; set; }
        public string IS_EXTERNAL_INTERNAL { get; set; }
        public string EVALUATION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string EVALUATION_DATE { get; set; }
        public string EXAM_NAME { get; set; }
        public string MESSAGE { get; set; }
        public string IS_DATE_VALIDATION { get; set; }
    }
    public class EXAM_EVALUATOR_STAFF_INFO
    {
        public string EXAM_EVALUATOR_STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string NAME { get; set; }
        public string PHONE { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string ROLE { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string MESSAGE { get; set; }
        public string USER_ACCOUNT_ID { get; set; }
        public string EXAM_EVALUATOR_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_EXTERNAL { get; set; }
    }
    public class JSON_EVALUATOR
    {
        public List<EXAM_EVALUATOR_STAFF_INFO> LiEvaluatorInfo { get; set; }
        public List<EXAM_EVALUATION_STUDENT_MARK> JSON_EVSTUDENTMARK { get; set; }
        public List<EXAM_SEATING_INVIGILATOR> JSON_EXAM_SEATING_INVIGILATOR { get; set; }
        public List<EXAM_SEATING_ATTENDANCE> JSON_EXAM_SEATING_ATTENDANCE { get; set; }
        public List<CEO_MARKSHEET_DISTRIBUTION> JSON_MARKSHEET_DISTRIBUTION { get; set; }
        public List<COE_CERTIFICATE_DISTRIBUTION> JSON_CERTIFICATE_DISTRIBUTION { get; set; }
        public List<CERTIFICATE_ISSUE_LEAVE> JSON_CERTIFICATE_ISSUE_LEAVE { get; set; }



    }
    public class CERTIFICATE_ISSUE_LEAVE
    {
        public string LEAVEREQUEST_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string USER_ID { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string REASON { get; set; }
        public string STATUS { get; set; }      // Optional: "Pending" by default
        public string ENTRY_ID { get; set; }
        public string APPROVED_DATE { get; set; }
        public string CREATED_DATE { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }

    public class EXAM_SEATING_ALLOTEMENT_REPORT
    {
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string EXAM_DATE { get; set; }
        public string SEAT_CODE { get; set; }
        public string ROOM_ID { get; set; }
        public string ROOM_NO { get; set; }
        public string BLOCK_NAME { get; set; }
        public string ROW { get; set; }
        public string SEAT_CODE_ID { get; set; }
        public string SESSION_NAME { get; set; }
        public string INVIGILATOR { get; set; }
    }

    public class JSON_OBE
    {
        public List<OBE_QUESTIONS> OBEQuestions { get; set; }
    }
    public class OBE_OBJECT
    {
        public List<OBE_SCORESHEET> liQuestionlevel = new List<OBE_SCORESHEET>();
        public List<OBE_SCORESHEET> liOBECourese = new List<OBE_SCORESHEET>();
        public List<OBE_SCORESHEET> liStudentMark = new List<OBE_SCORESHEET>();
    }

    public class EXAM_SUPPLEMENTARY_SETTING_FOR_PROGRAMME
    {
        public string PROGRAMME_SETTING_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string PROGRAMME_YEAR_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class EXAM_EXTERNAL_USERS
    {
        public string EXTERNAL_USER_ID { get; set; }
        public string EXTERNAL_USER_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string DOB { get; set; }
        public string MOBILE_NO { get; set; }
        public string MAIL_ID { get; set; }
        public string COLLEGE_NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ROLE { get; set; }
        public string USER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class COURSE_SYLLABUS
    {
        public string COURSE_SYLLABUS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_APPROVAL_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string UNIT_ID { get; set; }
        public string TOPIC { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_APPROVAL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class CP_COURSE_APPROVAL
    {
        public string COURSE_APPROVAL_ID { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PART { get; set; }
        public string HRS_PER_WEEK { get; set; }
        public string CREDITS { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string COURSE_ORDER { get; set; }
        public string IS_NME_SUBJECT { get; set; }
        public string IS_ALLIED_SUBJECT { get; set; }
        public string IS_COMPULSORY { get; set; }
        public string IS_APPROVED { get; set; }
        public string INTERNAL { get; set; }
        public string EXTERNAL { get; set; }
        public string TOTAL { get; set; }
        public string EXAM_MARK { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string COURSE_TYPE { get; set; }
        public string PAPER_TYPE { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class EXAM_TT_GROUPING
    {
        public string TT_GROUP_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string EXAM_SESSION_ID { get; set; }
        public string SET_ID { get; set; }
        public string GROUP_ORDER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string EXAM_NAME { get; set; }
        public string SET_NAME { get; set; }
        public string EXAM_SESSION { get; set; }
    }

    public class EXAM_TT_LEAVE
    {
        public string EXAM_LEAVE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string LEAVE_DATE { get; set; }
        public string LEAVE_REASON { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string EXAM_ID { get; set; }
        public string EXAM_NAME { get; set; }
    }
    public class EXAM_TT_STRUCTURE
    {
        public string EXAM_TT_STRUCTURE_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_TYPE { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_SESSION_ID { get; set; }
        public string EXAM_TIME { get; set; }
        public string UGC_COURSE_TYPE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string UGC_COURSE_TYPE_ID { get; set; }
        public string COUNT { get; set; }
        public string FACULTY_ID { get; set; }


    }

    public class EXAM_TT_SETTING
    {
        public string EXAM_TT_SETTING_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ALLOW_ARREAR_LAST { get; set; }
        public string ALLOW_STATIC_SESSION { get; set; }
        public string TT_INTERVAL_DAYS { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string SESSION { get; set; }
        public string BATCH { get; set; }
        public string ARREAR_POSITION { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
    }

    public class EXAM_DEPT_QUESTION_PAPER_SETTING
    {
        public string QUESTION_PAPER_SETTING_ID { get; set; }
        public string SECTION_ID { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
        public string TOPIC { get; set; }
        public string IS_EITHEROR { get; set; }
        public string TOTAL_QUESTIONS { get; set; }
        public string MAX_QUESTION { get; set; }
        public string MARK_PER_QUESTION { get; set; }
        public string TOTAL_MARK { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        // Extra ...,
        public string SECTION { get; set; }
    }

    public class EXAM_QUESTIONS
    {
        public string QUESTION_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string SECTION_ID { get; set; }
        public string QUESTION_LEVEL { get; set; }
        public string QUESTION { get; set; }
        public string EITHER_OR_QUESTION { get; set; }
        public string UNIT_ID { get; set; }
        public string IS_REPEAT { get; set; }
        public string REPEAT_YEAR { get; set; }
        public string NOT_REPEAT_YEAR { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ENTRY_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        // Extra ..,
        public string DEPARTMENT { get; set; }
        public string SEMESTER { get; set; }
        public string EXAM_SETTING_ID { get; set; }



    }

    public class EXAM_QUESTION_PAPER : EXAM_QUESTIONS
    {
        public string QUESTION_PAPER_ID { get; set; }
        //  public string COURSE_ID { get; set; }
        public string QUESTION_LEVEL_ID { get; set; }
        // public string QUESTION_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        //   public string IS_ACTIVE { get; set; }
        //  public string IS_DELETED { get; set; }

        //Extra

    }


    public class EXAM_SEATING_ATTENDANCE
    {
        public string EXAM_ATTENDANCE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string EXAM_TYPE { get; set; }
        public string EXAM_SESSION_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string CREATED_AT { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_CODE { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string ROOM_ID { get; set; }
        public string ROW_NO { get; set; }
        public string COLUMN_NO { get; set; }
        public string IS_CHECKED { get; set; }

    }

    public class EXAM_MALPRACTICE_STUDENTS
    {
        public string EXM_MALPRACTICE_STU_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_TYPE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string MALPRACTICE_DATE { get; set; }
        public string CLASS_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string CURRENT_SEMESTER { get; set; }
        public string INTERVAL_SEMESTER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_NAME { get; set; }
        public string EXAM_TYPE_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string BATCH { get; set; }
    }


    public class CP_EQUIVALENT_COURSE
    {
        public string EQUIVALENT_COURSE_ID { get; set; }
        public string EQUIVALENT_COURSE { get; set; }
        public string EQUIVALENT_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
    }

    public class CEO_MARKSHEET_DISTRIBUTION
    {
        public string MARKSHEET_DISTRIBUTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string REASON { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public string APPROVER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string REQUEST_DATE { get; set; }
        public string APPROVED_DATE { get; set; }
        public string CERTIFICATE_NAME { get; set; }

        //Extra.

        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string MONTH_YEAR_OF_SEMESTER { get; set; }
        public string CERTIFICATE_STATUS { get; set; }
        public string ISSUED_DATE { get; set; }
        public string CERTIFICATE_DISTRIBUTION_ID { get; set; }


    }

    public class COE_CERTIFICATE_DISTRIBUTION
    {
        public string EXTRA_INFORMATION { get; set; }
        public string AUTHORIZER_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CERTIFICATE_DISTRIBUTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string YEAR_OF_SEMESTER { get; set; }
        public string AUTHORIZE_REASON { get; set; }
        public string MONTH_OF_SEMESTER { get; set; }
        public string IS_CLEARED_ALL_SUBJECT { get; set; }
        public string REASON { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public string APPROVER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string REQUEST_DATE { get; set; }
        public string APPROVED_DATE { get; set; }
        public string CERTIFICATE_ID { get; set; }
        public string CERTIFICATE_STATUS { get; set; }
        public string ISSUED_DATE { get; set; }
        public string AUTHORIZE_PERSON { get; set; }
        public string AUTHORIZE_PERSON_NAME { get; set; }
        public string CONTACT_NO { get; set; }
        public string TO_BE_COLLECTED_DATE { get; set; }
        public string CERTIFICATE_TYPE { get; set; }
        //Extra.

        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string CERTIFICATE_NAME { get; set; }

        public string MARKSHEET_DISTRIBUTION_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string MODIFIED_DATE { get; set; }

    }
    public class ONLINE_OBJECTIVE_QUESTIONS
    {
        public string QUESTION_ID { get; set; }
        public string PARTWISE_STAFF_ID { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER_ID { get; set; }
        public string ANSWER { get; set; }
        public string ENTRY_DATE { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }

        public string OPTION_ID { get; set; }
        public string OPTIONS { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_ID { get; set; }

        public string COURSE_TITLE { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string COURSE_TYPE { get; set; }
        public string UGC_COURSE_TYPE { get; set; }

        public string PART { get; set; }
        public string PART_ID { get; set; }
        public string UNIT_ID { get; set; }

        public string UNIT_NAME { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STAFF_ID { get; set; }


        public string STAFF_CODE { get; set; }

        public string FIRST_NAME { get; set; }

        public string DEPARTMENT { get; set; }





    }
    public class ONLINE_OBJECTIVE_OPTIONS
    {
        public string OPTION_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string OPTIONS { get; set; }
        public string ENTRY_DATE { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class ONLINE_EXAM_PARTWISE_STAFF
    {
        public string PARTWISE_STAFF_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string UNIT_ID { get; set; }
        public string PART_ID { get; set; }
        public string TOTAL_QUESTIONS { get; set; }
        public string COUNT { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string QUESTION_ID { get; set; }

    }

    public class EXAM_QUESTION_PATTERN
    {
        public string QUESTION_PATTERN_ID { get; set; }
        public string PATTERN_NAME { get; set; }
        public string TOTAL_HOURS { get; set; }
        public string NO_OF_QUESTIONS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }


    }

    public class EXAM_SECTION
    {
        public string SECTION_ID { get; set; }
        public string SECTION_NAME { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }


    }

    public class EXAM_QUESTION_PATTERN_SECTION
    {
        public string PATTERN_SECTION_ID { get; set; }
        public string QUESTION_PATTERN_ID { get; set; }

        public string QUESTION_TYPE { get; set; }
        public string NO_OF_QUESTIONS { get; set; }
        public string IS_QUES_TYPE_MIX { get; set; }
        public string SECTION_ID { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COUNT { get; set; }
        public string MARK_FOR_EACH_QUESTION { get; set; }
        public string TOTAL_MARK_FOR_EACH_SECTION { get; set; }
        public string SECTION_DECRIPTION { get; set; }
        public string TO_BE_ATTEND { get; set; }



    }
    public class EXAM_OBJECTIVE_QUESTIONS
    {
        public string QUIZ_ID { get; set; }
        public string ENTRY_ID { get; set; }

        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }

        public string QUESTION_TYPE { get; set; }
        public string QUESTION_TYPE_ID { get; set; }

        public string QUESTION { get; set; }
        public string ANSWER_ID { get; set; }
        public string SET_ID { get; set; }
        public string PATTERN_SECTION_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        //extra
        public string QUESTION_PATTERN_ID { get; set; }

        public string SECTION_ID { get; set; }
        public string EITHER_QUESTION { get; set; }
        public string COUNT { get; set; }
        public string OPTION_ID { get; set; }
        public string OPTION { get; set; }
        public string ANSWER { get; set; }

        public string SECTION_NAME { get; set; }
        public string STUDENT_ID { get; set; }

        public string EXAM_DATE { get; set; }

        public string EXAM_NAME { get; set; }
        public string EXAM_INVIGILATION_ID { get; set; }
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string CLASS_NAME { get; set; }
        public string INVIGILATOR { get; set; }
        public string FIRST_NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public string STATUS { get; set; }
        public string MINUTES { get; set; }
        public string SECONDS { get; set; }
        public string IMAGE_PATH { set; get; }
        public string QUIZ_SAVE_ID { get; set; }
        public string TO_BE_ATTEND { get; set; }
        public string TOTAL_HOURS { get; set; }
        public string MINUTES_SECONDS { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_PREVIEW { get; set; }
        public string MARK_FOR_EACH_QUESTION { get; set; }
        public string SECURED_MARK { get; set; }
        public string TOTAL_MARK { get; set; }
        public string TOTAL_MARK_FOR_EACH_SECTION { get; set; }
        public string ANSWER_KEY_PATH { get; set; }
        public string SET { get; set; }



    }
    public class EXAM_OBJECTIVE_OPTIONS
    {
        public string OPTION_ID { get; set; }
        public string OPTION { get; set; }
        public string QUIZ_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        //Extra
        public string ANSWER { get; set; }

    }
    public class EXAM_QUESTION_TYPE
    {
        public string QUESTION_TYPE_ID { get; set; }
        public string QUESTION_TYPE { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class EXAM_INVIGILATION : EXAM_STUDENT_LOG_INFO
    {
        public string EXAM_INVIGILATION_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }


        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }


        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }

        public string EXAM_INVIGILATOR_ID { get; set; }
        public string EXAM_DATE { get; set; }
        public string START_TIME { get; set; }

        public string END_TIME { get; set; }
        public string SUSPECTIONS { get; set; }
        public string DUMMY_NO { get; set; }
        public string SET { get; set; }
        public string STATUS { get; set; }

        public string EVALUATOR_ID { get; set; }
        public string TOTAL_HOURS { get; set; }

        public string COMPLETE { get; set; }
        public string EXAM_NAME { get; set; }
        public string VIOLATION_COUNT { get; set; }

        public string DATE_OF_BIRTH { get; set; }
        public string TIMETABLE_ID { get; set; }
        public string MSGCOUNT { get; set; }
        public string MOBILE_NO { get; set; }
        public string STAFF_CODE { get; set; }
        public string STUDENT_COUNT { get; set; }
        public string CLASS_NAME { get; set; }
        public string EXAM_TIME { get; set; }
        public string SET_NAME { get; set; }
        public string CLASS_ID { get; set; }

    }
    public class EXAM_STUDENT_LOG_INFO
    {
        public string EXAM_LOG_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string INVIGILATION_ID { get; set; }
        public string EXAM_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string PUBLIC_IP { get; set; }
        public string MINUTES_SECONDS { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string IS_VIOLATION { get; set; }
        public string DESCRIPTION { get; set; }
        public string IMAGE_PATH { get; set; }
        public string DEVICE { get; set; }
        public string BROWSER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string DATETIME { get; set; }
    }

    public class EXAM_CHAT_MESSAGE
    {
        public string EXAM_CHAT_MESSAGE_ID { get; set; }
        public string EXAM_INVIGILATION_ID { get; set; }
        public string MESSAGE { get; set; }
        public string MESSAGE_TYPE { get; set; }
        public string FROM_ID { get; set; }
        public string TO_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string IS_READ { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string TIME { get; set; }
        public string DATE { get; set; }

    }
    public class EXAM_SQUAD
    {
        public string EXAM_SQUAD_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_EXTERNAL { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }


    }
    public class EXAM_EVALUATION
    {
        public string EXAM_EVALUATION_ID { get; set; }
        public string INVIGILATION_ID { get; set; }
        public string EVALUATOR_ID { get; set; }
        public string EVALUATION_TYPE { get; set; }
        public string EVALUATION_DATE { get; set; }
        public string DUMMY_NO { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        //Extra
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string EXAM_NAME { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string SET { get; set; }
        public string SET_ID { get; set; }
        public string SECTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string EXAM_EVALUATION_TYPE { get; set; }
        public string IS_COMPLETED { get; set; }
        public string TIMETABLE_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string EXAM_DATE { get; set; }
        public string TOTAL_HOURS { get; set; }
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }

    }
    public class EXAM_EVALUATOR
    {
        public string EXAM_EVALUATOR_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_EXTERNAL { get; set; }
        public string ACADEMIC_YEAR { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class EXAM_EVALUATION_MARKS
    {
        public string EXAM_EVALUATION_MARK_ID { get; set; }
        public string INVIGILATION_ID { get; set; }
        public string EVALUATION_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string OPTION_ID { get; set; }

        public string QUESTION_TYPE { get; set; }

        public string TOTAL_MARK { get; set; }
        public string SECURED_MARK { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string EVALUATION_TYPE { get; set; }
        public string EVALUATION_DATE { get; set; }
        public string EVALUATOR_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DUMMY_NO { get; set; }
        public string STUDENT_ID { get; set; }
        public string SECTION_NAME { get; set; }
        public string QUESTION { get; set; }
        public string MARK_FOR_EACH_QUESTION { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }



    }

    public class EXAM_SUP_EVALUATION_TYPE
    {
        public string EXAM_EVALUATION_TYPE_ID { get; set; }
        public string EXAM_EVALUATION { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class EXAM_ONLINE_EXAM_SETTING
    {
        public string ONLINE_EXAM_SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }

        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_NAME { get; set; }

        public string IS_NEED_CAMERA { get; set; }
        public string IS_NEED_CHAT { get; set; }
        public string IS_NEED_LOG { get; set; }
        public string IS_AUTO_EVALUATION { get; set; }
        public string IS_AUTO_START { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string EXAM_TYPE { get; set; }
        public string QUIZ_TYPE_NAME { get; set; }



    }
    public class QUIZ_QUESTIONS
    {
        //public string PRIMARY_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public object QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string OPTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string OPTIONS { get; set; }
        public object A { get; set; }
        public object B { get; set; }
        public object C { get; set; }
        public object D { get; set; }
        public string E { get; set; }
        public string F { get; set; }
        public string CODE { get; set; }
        public string SET { get; set; }
        public string MARK { get; set; }
    }

    public class SUP_EXAM_DEFAULTERS_CATEGORY_LIST
    {
        public string DEFAULT_CATEGORY_ID { get; set; }
        public string DEFAULT_CATEGORY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }


    }

    public class EXAMINARS_INFO
    {
        public string EXAMINAR_ID { get; set; }
        public string EXAMINAR_ENTRY_ID { get; set; }
        public string EXAMINAR_NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public string EMAIL_ID { get; set; }
        public string COLLEGE_NAME { get; set; }
        public string PLACE { get; set; }
        public string EXPERIENCE_UG { get; set; }
        public string EXPERIENCE_PG { get; set; }
        public string EXPERIENCE_MPHIL { get; set; }
        public string SUBJECT1 { get; set; }
        public string SUBJECT2 { get; set; }
        public string SUBJECT3 { get; set; }
        public string SUBJECT4 { get; set; }
        public string EXAMINAR_CODE { get; set; }
        public string IS_DELETED { get; set; }
        public string DESIGNATION { get; set; }
        public string QUALIFICATION { get; set; }
        public string IS_EXAMINAR { get; set; }
        public string IS_QUESTIONSETTER { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_PRACTICAL { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string DATE { get; set; }
        public string EXAMINAR { get; set; }
        public string PURPOSE { get; set; }
        public string EXAM_SETTING { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_NAME { get; set; }
        public string M_PASS { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string TABLE_CODE { get; set; }
        public string IS_INTERVAL { get; set; }
        public string INTERVAL_FROM_DATE { get; set; }
        public string INTERVAL_TO_DATE { get; set; }
    }

    public class INTERNAL_MARKS
    {


        public string INTERNAL_MARK_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }

        public string INTERNAL_MARK { get; set; }
        public string SEMESTER { get; set; }
        public string EXAM_SETTING_ID { get; set; }

        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string APPTYPE_ID { get; set; }
        public string EXTERNAL_MARK { get; set; }
        public string TOTAL { get; set; }
        public string RESULT { get; set; }
        public string SEMESTER_MARK_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string IS_ARREAR { get; set; }
    }

    public class EXAM_MARK_PACKET_SETTINTG
    {
        public string EXAM_MARK_SUBJECT_ID { get; set; }
        public string EXAM_MARK_SUBJECT { get; set; }
        public string SUBJECT_TYPE_ID { get; set; }
        public string INTERNAL_MARK { get; set; }
        public string EXTERNAL_MARK { get; set; }
        public string TOTAL_PASS_MARK { get; set; }
        public string PACKET_COUNT { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string EXAM_MARK_SUBJECT_TYPE_ID { get; set; }
        public string EXAM_MARK_SUBJECT_TYPE { get; set; }
        public string EXAM_NAME { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string IS_CONVERSION { get; set; }
    }


    public class VA_COURSE_ALLOTMENT
    {
        public string ALLOTED_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string MINIMUM_SEAT { get; set; }
        public string MAXIMUM_SEAT { get; set; }
        public string AMOUNT { get; set; }
        public string MINIMUM_HOUR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string IS_DELETED { get; set; }
        public string DEPARTMENT { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_ALLOTED_ID { get; set; }
        public string SEMESTER { get; set; }
        public string FREQUENCY { get; set; }
        public string SETTINGS_NAME { get; set; }
        public string STUDENT_ID { get; set; }
        public string REGISTERED { get; set; }
        public string CHECKING { get; set; }
        public string REGISTRATION_ID { get; set; }
        public string COUNT { get; set; }
        public string VERIFY { get; set; }
        public string TIME_REGISTERED { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STATEMENT_OF_MARKS { get; set; }

    }
    public class VA_MARKS
    {
        public string MARK_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string INTERNAL_MARK { get; set; }
        public string EXTERNAL_MARK { get; set; }
        public string TOTAL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ENTRY_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string M_PASS { get; set; }
        public string SEMESTER { get; set; }
        public string COUNT { get; set; }
        public string MAXIMUM_SEAT { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string REGISTRATION_ID { get; set; }
        public string SETTINGS_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string FIRST_NAMES { get; set; }
        public string FOLIO_NUMBER { get; set; }
        public string STATUS { get; set; }
        public string TOTAL_CREDIT { get; set; }
        public string TOTAL_DEBIT { get; set; }
        public string BALANCE { get; set; }
        public string F_STUDENT_AC_ID { get; set; }
    }

    public class VA_ATTENDANCE
    {
        public string ATTENDANCE_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string ATTENDANCE_DATE { get; set; }
        public string TIMING { get; set; }
        public string ATTENDANCE_TYPE_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string TIME_REGISTERED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string M_PASS { get; set; }
        public string SEMESTER { get; set; }
        public string MINIMUM_HOUR { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string SEMESTER_NAME { get; set; }
    }

    public class EXAM_EXAMINER_ORDER
    {
        public string ERROR { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_ALLOTED_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ORDER_DATE { get; set; }
        public string DUE_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STAFF_CODE { get; set; }
        public string EXAM_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string FIRST_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string cms_semester_type { get; set; }
    }

    public class SIGNATORIES
    {
        public string SIGNATORIES_ID { get; set; }
        public string ROLE_ID { get; set; }
        public string SIGNATORIES_NAME { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class EXAM_CHIEFEXAMINER_ORDER
    {
        public string STAFF_ALLOTED_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ORDER_DATE { get; set; }
        public string DUE_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STAFF_CODE { get; set; }
        public string EXAM_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string FIRST_NAME { get; set; }
    }

    public class EXAM_FEE_PERMISSION
    {
        public int exam_fee_id { get; set; }
        public string student_id { get; set; }
        public string exam_setting { get; set; }
        public string academic_year { get; set; }
        public string remarks { get; set; }
        public int is_active { get; set; }
        public int is_deleted { get; set; }
        public string created_by { get; set; }
        public DateTime? created_date { get; set; }

        // Helper display properties
        public string roll_no { get; set; }
        public string register_no { get; set; }
        public string student_name { get; set; }
        public string class_name { get; set; }
        public string exam_name { get; set; }
        public decimal total_outstanding_due { get; set; }
    }

    public class ExamFeePaymentViewModel
    {
        public List<FEE_STUDENT_ACCOUNT> ExamFeeAccounts { get; set; }
        public StudentFeeDueSummary StudentDues { get; set; }
        public bool HasOutstandingDues { get; set; }
        public bool HasActivePermission { get; set; }
        public EXAM_FEE_PERMISSION ActivePermission { get; set; }
        public string StudentId { get; set; }
        public string RollNo { get; set; }
        public string RegisterNo { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string AcademicYear { get; set; }
        public string ExamSettingId { get; set; }
        public string ExamSettingName { get; set; }
        public CollegeDetails CollegeInfo { get; set; }
        public decimal TotalExamFee { get; set; }
        public bool CanProceedToPay => !HasOutstandingDues || HasActivePermission;
    }

    public class StudentFeeDueSummary
    {
        public string StudentId { get; set; }
        public string RollNo { get; set; }
        public string StudentName { get; set; }
        public string AcademicYear { get; set; }

        public decimal Term1Due { get; set; }
        public decimal Term2Due { get; set; }
        public decimal CollegeFeeDue => Term1Due + Term2Due;
        public decimal HostelFeeDue { get; set; }
        public decimal BusFeeDue { get; set; }
        public decimal OldFeeDue { get; set; }
        public decimal TotalOutstandingDue => CollegeFeeDue + HostelFeeDue + BusFeeDue + OldFeeDue;
        public bool HasDues => TotalOutstandingDue > 0;

        public List<ItemizedDueDetail> DueItems { get; set; } = new List<ItemizedDueDetail>();
    }

    public class ItemizedDueDetail
    {
        public string FeeCategory { get; set; }
        public string Particulars { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string Status { get; set; }
    }

    public class STUDENT_SEARCH_RESULT
    {
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FULL_NAME { get; set; }
        public string CLASS_NAME { get; set; }
    }

    public class ATT_ATTENDANCE_MARK_UPLOAD_ITEM
    {
        public int ATT_MARK_UPLOAD_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public decimal ATTENDANCE_MARK { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string CREATED_DATE_DISPLAY { get; set; }
        public string STUDENT_NAME { get; set; }
        public string CLASS_NAME { get; set; }
    }

    public class EXAM_SETTING_DROPDOWN_ITEM
    {
        public int EXAM_SETTING_ID { get; set; }
        public string EXAM_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public int IS_ACTIVE { get; set; }
    }
}


