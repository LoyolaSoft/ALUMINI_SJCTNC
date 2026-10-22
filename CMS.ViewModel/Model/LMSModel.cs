using CMS.ViewModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.Model
{
    public class LMS_COURSEWISE_ATTACHMENTS
    {
        public string COURSEWISE_ATTACHMENT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string UNIT { get; set; }    
        public string TITLE { get; set; }
        public string SUBJECT { get; set; }
        [AllowHtml]
        public string DESCRIPTION { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string FILE_PATH { get; set; }

        public string COURSEWISE_FILE_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string NOTES_ID { get; set; }
        public string QUESTION_PAPER_TYPE_ID { get; set; }
        public string QUESTION_PAPER_ID { get; set; }
        public string PAPER_TITLE { get; set; }
        public string TYPE { get; set; }
        public string NOTES_TITLE { get; set; }
        public string EXCEL_UNIT { get; set; }
        public string EXCEL_TOPIC { get; set; }
        public string ATT_DATE { get; set; }
        public string ANY_OTHER { get; set; }
        public string RUBRIC { get; set; }
        public string ASSESSMENT { get; set; }
        public string RESOURCE { get; set; }
        public string METHOD { get; set; }
        public string HOUR { get; set; }
        public string TOPIC { get; set; }
        public string STUDENT_ID { get; set; }
        public string DATE_FROM { get; set; }
        public string TOTAL_COUNT { get; set; }
        public string DAY_ORDER_DATE { get; set; }
        public string DAY_ORDER_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SUP_SEMESTER_ID { get; set; }
        public string HOURID { get; set; }
        public string ATTACHMENT { get; set; }
        public string LMS_UNIT_ID { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string ATTACHMENT_PATH { get; set; }



    }
    public class LMS_COURSEWISE_FILES
    {
        public string COURSEWISE_FILE_ID { get; set; }
        public string COURSEWISE_ATTACHMENT_ID { get; set; }
        public string FILE_PATH { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class SUP_LMS_UNIT
    {
        public string LMS_UNIT_ID { get; set; }
        public string UNIT { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class LMS_COURSEWISE_ANNOUNCEMENT
    {
        public string ANNOUNCEMENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE_CODE { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_TITLE { get; set; }

        public string STAFF_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string FIRST_NAME { get; set; }

        public string DATE { get; set; }
        public string MESSAGE { get; set; }
        public string ANNOUNCEMENT_TYPE { get; set; }
        public string STUDENT_ID { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string CATEGORY { get; set; }
        public string MESSAGE_TYPE { get; set; }
        public string FILE_PATH { get; set; }
        public string PHOTO { get; set; }
        public string DATES { get; set; }
        public string MOBILE_NO { get; set; }
        public string DEPARTMENT { get; set; }
        public string GSUITE_MAIL_ID { get; set; }
        public string EXISTING_FILE_NAME { get; set; }
    }
    public class LMS_STUDENT_ASSIGNMENT_SUBMISSION
    {
        public string STU_ASSIGNMENT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string NAME { get; set; }

        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string COURSE_CODE { get; set; }

        public string COURSE_ID { get; set; }
        public string TOTAL_MARK { get; set; }
        public string SEC_MARK { get; set; }
        public string ASSIGNMENT_SETTING_ID { get; set; }
        public string ASSIGNMENT_NAME { get; set; }
        public string CLASS_NAME { get; set; }

        public string COURSE_TITLE { get; set; }
        public string FILE_PATH { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        public string TOTAL { get; set; }
        public string SUBMITED { get; set; }
        public string S_MARK { get; set; }
        public string FIRST_NAME { get; set; }
        public string END_TIME { get; set; }
        public string START_TIME { get; set; }
        public string END_DATE { get; set; }
        public string START_DATE { get; set; }
        public string CLASS_ID { get; set; }
        public string STAFF_ID { get; set; }
    }
    public class LMS_ASSIGNMENT_SETTING
    {

        public string ASSIGNMENT_SETTING_ID { get; set; }
        public string ASSIGNMENT_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }

        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string DISCRIPTION { get; set; }
        public string DESCRIPTION { get; set; }

        public string CLASS_ID { get; set; }
        //public string ROLL_NO { get; set; }
        
        public string TOTAL_MARK { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string TIME { get; set; }

        public string ACADEMIC_YEAR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        public string SEC_MARK { get; set; }
        public string FILE_PATH { get; set; }
        public string STAFF_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string STATUS { get; set; }
        public string TOTAL { get; set; }
        public string LMS_GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string STUDENT_ID { get; set; }
        public string CURRENT_STUDENT_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string SUBMITED { get; set; }    
        public string ASSIGNMENT_INFO { get; set; }
        public string STUDENT_NAME { get; set; }
        public string STU_ASSIGNMENT_ID { get; set; }
        public string LINK_URL { get; set; }
        public string EXISTING_FILE_NAME { get; set; }
        public string ATTACHMENTS { get; set; }
        public string ROLL_NO { get; set; }
       
    }
    public class LMS_CLASS_GROUP
    {
        public string CLS_GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class LMS_EXCEL_UPLOAD_DETAILS
    {
        public string ATTACHMENT_ID { get; set; }
        public string COURSEWISE_ATTACHMENT_ID { get; set; }
        public string TOPIC { get; set; }
        public string UNIT { get; set; }
        public string ATT_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string HOUR { get; set; }
        public string METHOD { get; set; }
        public string RESOURCE { get; set; }
        public string ASSESSMENT { get; set; }
        public string RUBRIC { get; set; }
        public string ANY_OTHER { get; set; }
    }
    
    public class LMS_EXCEL_UPLOAD_ASSIGNMENT
    {
        public string CLASS_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string ASSIGNMENT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string TOTAL_MARK { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ENTRY_DATE { get; set; }
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
    }
    public class LMS_ZOOM_MEETINGS
    {

        public string FIRST_NAME { get; set; }
        public string COURSE_TITLE { get; set; }
        public string CREATED_DATE { get; set; }
        public string JOIN_URL { get; set; }
        public string MEETING_PASSWORD { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string MEETING_UID { get; set; }
        public string START_URL { get; set; }
        public string MEETING_ID { get; set; }
        public string COURSE_ID { get; set; }


    }
    public class CHAT_UNREAD_COUNT
    {
        public string CHATROOMID { get; set; }
        public string COURSE_ID { get; set; }
        public string UNREADCOUNT { get; set; }
    }
    public class CHAT_MESSAGE
    {
        public string MESSAGEID { get; set; }
        public string CHATROOMID { get; set; }
        public string SENDERID { get; set; }
        public string SENDERNAME { get; set; }
        public string MESSAGETEXT { get; set; }
        public string SENTAT { get; set; }
    }
    public class STUDENT_CHAT_CONTEXT
    {
        public string CLASS_ID { get; set; }
        public string DEPARTMENTID { get; set; }
        public string YEAR { get; set; }
        public string SEMESTERID { get; set; }
    }
    public class CHAT_ROOM
    {
        public string CHATROOMID { get; set; }
        public string COURSEID { get; set; }
        public string DEPARTMENTID { get; set; }
        public string YEAR { get; set; }
        public string SEMESTERID { get; set; }
        public string ACADEMICYEAR { get; set; }

    }
    public class STAFF_COURSE_CHATINFO
    {

        public string COURSE_ID { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COURSE_CODE { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string YEAR { get; set; }
        public string DEPARTMENT { get; set; }
        public string SEMESTER_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STAFF_ID { get; set; }
        public string UNREADCOUNT { get; set; }


    }


    public class JSON_LMS_ASSIGNMENT
    {
        public List<LMS_STUDENT_ASSIGNMENT_SUBMISSION> listuassignment { get; set; }

    }
}
