using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{

    public class CP_CLASSES
    {
        public string CLASS_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_DESCRIPTION { get; set; }
        public string CLASS_TYPE { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string CLASS_YEAR { get; set; }
        public string PROGRAMME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string LANGUAGE { get; set; }
        public string IS_CHOICE { get; set; }
        public string CLASS_ORDER { get; set; }
        public string CLS_PROMOTION_GROUP { get; set; }
        public string CLS_PROMOTION_LEVEL { get; set; }
        public string SHIFT { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_UPDATE { get; set; }
        public string COURSE_TYPE { get; set; }
        public string COURSE_ID { get; set; }
        public string COURSE { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string CIA_DISABLE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string EXAM_TIME { get; set; }
        public string ONLINE_CLASS_LINK { get; set; }
        public string STAFF_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string STUDENT_ID { get; set; }
       
    }
    
    public class TimeTableTemplate
    {
              
        public string CLASS_ID { get; set; }                     
        public string CLSCRSSTF_ID { get; set; }
        public string Time_Table_Id { get; set; }
        public string TT_Temp_Id { get; set; }
        public string Day_Order_Id { get; set; }
        public string DAY { get; set; }
        public string Hour_Id { get; set; }
        public string HOUR { get; set; }
        public string Paper_Type_Id { get; set; }
        public string PAPER_TYPE { get; set; }
        public string Is_Static { get; set; }
        public string Is_Active { get; set; }
        public string Is_Deleted { get; set; }
        public string Faculty_Id { get; set; }
        public string FACULTY { get; set; }
        public string Setting_Id { get; set; }
        public string SETTING_NAME { get; set; }
        public string Year { get; set; }
        public string GraduationType { get; set; }
        public string CLASSLEVEL { get; set; }
        public string CLASS_YEAR { get; set; }
        public string SHIFT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string SEMESTER_ID { get; set; }
    }

    public class TimeTableSetting
    {
        public string SETTING_NAME { get; set; }
        public string SETTING_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string SEMESTER_NAME { get; set; }
        public string NO_OF_HOURS { get; set; }
        public string NO_OF_DAYS { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string BASIC_HOURS_FOR_STAFF { get; set; }
        public string BASIC_HOURS_FOR_HOD { get; set; }
        public string ALLOW_STATIC_HOUR { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string GRADUATION_TYPE { get; set; }
        public string CLASSLEVEL { get; set; }

    }
    public class TT_DEPARTMENT_WISE_TEMPLATE
    {
        public string DEPARTMENT_TEMPLATE_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string SETTING_NAME { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string Academic_Year { get; set; }
        public string STATUS { get; set; }

    }

    public class ACADEMIC_CURRICULUM_2017
    {
        public string CURRICULUM_ID { get; set; }
        public string PROGRAMME { get; set; }
        public string BATCH { get; set; }
        public string COURSE_ID { get; set; }
        public string S_NO { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_UPDATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string SEMESTER { get; set; }
        public string PAPER_TYPE { get; set; }
        public string SUBJECT_TYPE_ID { get; set; }
        public string SUBJECT_TYPE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string AMOUNT { get; set; }        
        public string HEAD { get; set; }
        public string COURSE_TYPE { get; set; }
        public string COURSE_HEAD_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        
    }

    public class CP_PROGRAMME_2017
    {
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_CODE { get; set; }
        public string DATE { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_DESCRIPTION { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEGREE { get; set; }
        public string PROGRAMME_ORDER { get; set; }
        public string DURATION { get; set; }
        public string DURATION_UNIT { get; set; }
        public string NO_OF_SEMESTER { get; set; }
        public string CHANNEL { get; set; }
        public string IS_AIDED { get; set; }
        public string IS_DELETED { get; set; }
        public string PRO_SERIES_ROLLNO { get; set; }
        public string NON_AIDED { get; set; }
        public string IS_REGULAR { get; set; }
        public string SUBJECTS { get; set; }
        public string PRO_SERIES_REGNO { get; set; }
        public string PRO_SERIES_ADMNO { get; set; }
        public string MEDIUM_OF_INSTRACTION { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SHIFT { get; set; }
        public string SHIFT_ID { get; set; }
        public string SETTING_ID { get; set; }
        public string FEE_MODE { get; set; }

    }
    public class PendagogicMethod
    {

        public string S_CLASS_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_CODE { get; set; }
        public string HOUR_ID { get; set; }
        public string HOUR { get; set; }
        public string PAPER_TYPE { get; set; }
        public string IS_STATIC { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER_ID { get; set; }
        public string PROGRAMME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string Count { get; set; }
        public string SHIFT { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        //public string STAFF_ID { get; set; }
        public string NAME { get; set; }
        public string COURSE_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string DAY_ORDER_DATE { get; set; }
        public string DATE { get; set; }
        public string STAFF_ID { get; set; }
        public string METHODOLOGY_ID { get; set; }
        public string UNIT_ID { get; set; }
        public string TOPIC_TITLE { get; set; }
        public string SUP_TOPIC { get; set; }
        public string IS_ACTIVE { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string PENDAGOGIC_ID { get; set; }
        public string METHODOLOGY_NAME { get; set; }
        public string UNIT_NAME { get; set; }
        public string ENTRY_ID { get; set; }
        public string SUP_STAFF { get; set; }
        public string TOTAL_HOUR { get; set; }
        public string ENTRY_HOUR { get; set; }
        public string DAY_ORDER_ID { get; set; }
        public string IS_GLOBAL { get; set; }
        public string METHODOLOGY_COUNT { get; set; }
        public string STAFF_NAME { get; set; }
        
    }
    public class TT_AcademicCurriculum
    {
        public string AC_BATCH_ID { get; set; }
        public string BATCH_ID { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string BATCH_YEAR { get; set; }
    }
    public class TimeTable
    {
        public string Time_Table_Id { get; set; }
        //   public string TIME_TABLE_ID { get; set; }
        public string Course_Id { get; set; }
        public string Day_Order_Id { get; set; }
        public string Staff_Id { get; set; }
        public string Class_Id { get; set; }
        public string S_CLASS_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_CODE { get; set; }
        public string Hour_Id { get; set; }
       
        public string HOUR { get; set; }
        public string hourType { get; set; }
        public string Hour_Type { get; set; }
        // public string HOURTYPE { get; set; }
        public string PAPER_TYPE { get; set; }
        public string hour_type_id { get; set; }
        public string Paper_Type_Id { get; set; }
        public string Is_Active { get; set; }
        public string IS_STATIC { get; set; }
        public string Is_Deleted { get; set; }
        public string Semester_Id { get; set; }
        public string PROGRAMME { get; set; }
        public string GraduationType { get; set; }
        public string Faculty_Id { get; set; }
        public string Setting_Id { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COURSE_ROOT_ID { get; set; }
        public string sCourseId { get; set; }
        public string sStaffId { get; set; }
        public string Count { get; set; }
        public string CLSCRSSTF_ID { get; set; }
        public string TT_Temp_Id { get; set; }
        public string SHIFT { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        //public string STAFF_ID { get; set; }
        public string NAME { get; set; }
        public string SEMESTER_TYPE_ID { get; set; }
       
        
    }

}
