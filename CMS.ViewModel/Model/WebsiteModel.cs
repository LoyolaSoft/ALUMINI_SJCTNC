using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace CMS.ViewModel.Model
{
    public class WebsiteModel
    {
        public string EVENT_ID { get; set; }
        public string EVENT_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string EVENT_TITLE { get; set; }

    }
    public class WEB_HOME_EVENTS
    {
        public string EVENT_ID { get; set; }
        public string ACHIEVEMENT_ID { get; set; }
        public string ACHIEVEMNT_CATEGORY { get; set; }
        public string EVENT_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string EVENT_TITLE { get; set; }
        public string DATE { get; set; }
        public string VENUE { get; set; }
        public string TIME { get; set; }
        public string FILE_PATH { get; set; }
        public string DESCRIPTION { get; set; }
        public string IMG_PATH { get; set; }
        public string APPROVE_LEVEL1 { get; set; }
        public string APPROVER_LEVEL2 { get; set; }
        public string APPROVER_LEVEL3 { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACHIEVEMENT_NAME { get; set; }
        public string ENTRY_DATE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string APPROVERID1 { get; set; }
        public string FIRST_NAME { get; set; }
        public string APPROVERID2 { get; set; }
        public string APPROVERID3 { get; set; }
        public string TITLE { get; set; }
        public string SDG_ID { get; set; }
        public string LINKTEXT { get; set; }
        public string LINK { get; set; }
        public string STAFF_ID { get; set; }

    }
    public class WEB_SUP_ACHIEVEMENT
    {
        public string ACHIEVEMENT_ID { get; set; }
        public string ACHIEVEMENT_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class WEB_SDGLIST
    {
        public string SDG_ID { get; set; }
        public string SDG_NAME_LIST { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class STU_PLACEMENT_NEWS
    {
        public string PLACEMENT_ID { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string VENUE { get; set; }
        public string TIME { get; set; }
        public string FILE_PATH { get; set; }
        public string DESCRIPTION { get; set; }
        public string IMG_PATH { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACHIEVEMENT_NAME { get; set; }
        public string ENTRY_DATE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string TITLE { get; set; }
        public string LINKTEXT { get; set; }
        public string LINK { get; set; }
        public string IS_UG { get; set; }
        public string IS_PG { get; set; }
        public string IS_INTERVIEW { get; set; }
        public string OTHERS { get; set; }

    }
    public class PLACED_STUDENT_LIST
    {
        public string ID { get; set; }
        public string PLACED_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string YEAR_OF_GRADUATION { get; set; }
        public string YEAR_OF_PLACEMENT { get; set; }
        public string COMPANY_NAME { get; set; }
        public string POSITION_OFFERED { get; set; }
        public string REMARKS { get; set; }
        public string CREATE_DATE { get; set; }
        public string SALARY_PACKAGE { get; set; }
        public string FILE_PATH { get; set; }
        public string IMG_PATH { get; set; }
        public string IS_DELETED { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string PHOTO { get; set; }
        public string IS_WEB_PLACEMENT { get; set; }
    }
    public class WEB_MENU_CLICKS_REPORT
    {
        public string CLICK_DATE { get; set; }
        public string MAIN_MENU { get; set; }
        public string SUB_MENU { get; set; }
        public string INSIDE_MENU { get; set; }
        public string CLICK_COUNT { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
    }

    public class ROLE_RESPONSIBILITY
    {
        public string STF_ROLE_RES_ID { get; set; }
        public string ROLES__RESPONSIBILITIES { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ORDER_BY { get; set; }
    }
}
