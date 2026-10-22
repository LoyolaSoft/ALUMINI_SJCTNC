using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class TaskManagementModel
    {

    }
    public class AssignmentModel
    {
        public string ASSIGNMENT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_ID { get; set; }
        public string ASSIGNMENT_TITLE { get; set; }
        public string DATE_FROM { get; set; }
        public string SUBMISSION_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_FILE_UPLOAD { get; set; }
        public string STUDENT_ID { get; set; }
        public string PHOTO { get; set; }
        public string STAFF_NAME { get; set; }
        public string COURSE_CODE { get; set; }
        public string NO_OF_ASSIGNMENTS { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string FILE_PATH { get; set; }
        public string SUBMITTED_DATE { get; set; }
    }

    public class SUP_TASK_SETTING
    {
        public string TASK_ID { get; set; }
        public string TASK_NAME { get; set; }
        public string LAST_DATE { get; set; }
        public string SEMESTER { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COMPONENT_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class CIA_BLOCK_LIST
    {
       public string CIA_BLOCK_ID { get; set; }
       public string STAFF_ID { get; set; }
       public string CLASS_ID { get; set; }
       public string COURSE_ID { get; set; }
       public string PROGRAMME_GROUP_ID { get; set; }
       public string PROGRAMME_LEVEL { get; set; }
       public string SHIFT_ID { get; set; }
       public string S_STAFF_ID { get; set; }
       public string S_CLASS_ID { get; set; }
       public string S_COURSE_ID { get; set; }
       public string DATE_FROM { get; set; }
       public string DATE_TO { get; set; }
       public string ORDER_ID { get; set; }
       public string ACADEMIC_YEAR { get; set; }
       public string IS_ACTIVE { get; set; }
       public string IS_DELETED { get; set; }
    }
    public class AcademicActivityModel
    {
        public string ACTIVITY_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ACTIVITY_LEVEL { get; set; }
        public string ACTIVITY_LEVEL_ID { get; set; }
        public string ACTIVITY_TITLE { get; set; }
        public string DATE_FROM { get; set; }
        public string SUBMISSION_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_SUBMITTED { get; set; }
        public string IS_FILE_UPLOAD { get; set; }
        public string PHOTO_PATH { get; set; }
        public string FILE_PATH { get; set; }
        public string STATUS { get; set; }
        public string SUBMITTED_DATE { get; set; }
        public string FIRST_NAME { get; set; }
        public string ACTIVITY_STATUS_NAME { get; set; }
        public string FILE_CORRECTION { get; set; }
        public string FILE_CORRECTION_STATUS { get; set; }
        public string FILE_CORRECTION_RETURN { get; set; }
        public string FILE_CR_STATUS { get; set; }

    }

    public class NOTITFICATIONS
    {
        public string NOTIFICATION_ID { get; set; }
        public string NOTIFICATION_ROLE_TYPE_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string NOTIFICATION { get; set; }
        public string USER_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_YEAR { get; set; }
        public string APP_TYPE_ID { get; set; }
        public string NOTIFICATION_LEVEL_ID { get; set; }
        public string IS_VIEWED { get; set; }
        public string NOTIFIED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string DEPARTMENT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class EVENT_REGISTRATION_SETTINGS
    {
        public string EVENT_SETTING_ID { get; set; }
        public string MAIN_EVENT_NAME { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
       
    }
    public class SUB_EVENTS
    {
        public string SUB_EVENT_ID { get; set; }
        public string SUB_EVENT_NAME { get; set; }
        public string MAIN_EVENT_ID { get; set; }
        public string MAIN_EVENT_NAME { get; set; }
        public string ALLOTED_NO { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
              
    }
    public class STU_EVENT_REGISTRATION
    {
        public string EVENT_REGISTRATION_ID { get; set; }
        public string MAIN_EVENT_NAME { get; set; }
        public string SUB_EVENT_NAME { get; set; }
        public string MAIN_EVENT_ID { get; set; }
        public string SUB_EVENT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ALLOTED_NO { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string ALLOTED_SEATS { get; set; }
        public string SUMOFQUTOA { get; set; }

    }
    public class MEETING_ASSIGNED
    {
        public string MEETING_ASSIGNED_ID { get; set; }
        public string MEETING_PARTICIPANT_ID { get; set; }
        public string MEETING_NAME { get; set; }
        public string DATE { get; set; }
        public string VENUE { get; set; }
        public string TIME { get; set; }
        public string ASSIGNED_BY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string PARTICIPANT_ID { get; set; }
        public string MEETING_ID { get; set; }
    }
    public class MEETING_PARTICIPANTS
    {
        public string MEETING_PARTICIPANT_ID { get; set; }
        public string MEETING_ID { get; set; }
        public string PARTICIPANT_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class TASK_LIST
    {
        public string TASK_ID { get; set; }
        public string TASK_ASSIGNED_ID { get; set; }
        public string TASK_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string TASK_DATE { get; set; }
        public string STAFF_ID { get; set; }
        public string SUB_DATE { get; set; }
        public string ASSIGNED_BY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FILE_PATH { get; set; }
        public string TASK_SUB_ID { get; set; }
    }
    public class TASK_ASSIGNED_LIST
    {
        public string TASK_ASSIGNED_ID { get; set; }
        public string TASK_ID { get; set; }
        public string STAFF_ID { get; set; }
        
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class TASK_SUBMISSION_LIST
    {
        public string TASK_SUB_ID { get; set; }
        public string TASK_ID { get; set; }
        public string TASK_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string SUB_DATE { get; set; }

        public string FILE_PATH { get; set; }

        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_HALLS
    {
        public string HALL_ID { get; set; }
        public string HALL_NO { get; set; }
        public string HALL_NAME { get; set; }
        public string LOCATION { get; set; }
        public string CAPACITY { get; set; }
        public string IMAGE { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string BLOCK_ID { get; set; }
        public string BLOCK { get; set; }
        public string BLOCK_NAME { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class HALL_RESERVATION
    {
        public string HALL_BOOK_ID { get; set; }
        public string HALL_ID { get; set; }
        public string HALL_NAME { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string FROM_TIME { get; set; }
        public string TO_TIME { get; set; }
        public string APPLY_DATE { get; set; }
        public string APPROVAL_DATE { get; set; }
        public string PURPOSE { get; set; }
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_AUTO_SCHEDULE { get; set; }

        public string IS_AUTO_SMS_SEND { get; set; }
        public string BLOCK_NAME { get; set; }


    }
    public class SUP_HALL_RESERVATION_STATUS
    {
        public string STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class HALL_RESERVATION_SETTING
    {
        public string SETTING_ID { get; set; }
        public string IS_AUTO_SCHEDULE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_AUTO_SMS_SEND { get; set; }
    }

    public class SUP_VISITORS_CATEGORY
    {
        public string VISITORS_CATEGORY_ID { get; set; }
        public string VISITORS_CATEGORY_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
       
    }
    public class VISITORS_MEETING_ASSIGNMENT
    {
        public string VISITORS_MEET_ID { get; set; }
        public string FIRST_NAME { get; set; }

        public string VISITORS_NAME { get; set; }
        public string MOBILE { get; set; }
        public string VISIT_DATE { get; set; }
        public string VISITORS_CATEGORY { get; set; }
        public string VISITORS_CATEGORY_NAME { get; set; }
        public string PURPOSE { get; set; }
        public string STAFF_ID { get; set; }
        public string STATUS { get; set; }
        public string IS_REQUIRED_APPOINMENT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class VISITORS_MEETING_APPOINTMENT
    {
        public string VISITORS_APPOINTMENT_ID { get; set; }
        public string VISITORS_NAME { get; set; }
        public string MOBILE { get; set; }
        
        public string VISITORS_CATEGORY { get; set; }
        public string VISITORS_CATEGORY_NAME { get; set; }
        public string VISITORS_MEET_ID { get; set; }
        public string APPOINTMENT_DATE { get; set; }
        public string APPOINTMENT_TIME { get; set; }
        public string REPLY { get; set; }
        public string STATUS { get; set; }
        public string VISIT_DATE { get; set; }
        public string PURPOSE { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_MET { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        
    }
    


}
