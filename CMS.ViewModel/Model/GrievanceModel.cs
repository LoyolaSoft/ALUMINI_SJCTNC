using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    class GrievanceModel
    {
    }
    public class GRIEVANCEINFO
    {
        public string GRIEVANCE_ID { get; set; }
        public string USER_ID { get; set; }
        public string GRIEVANCE_CODE { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GRIEVANCE_TITLE { get; set; }
        public string GRIEVANCE_SUBJECT { get; set; }
        public string GRIEVANCE_CONTENT { get; set; }
        public string SUB_DATE_TIME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }        
}
    public class GRIEVANCE_TYPE
    {
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GRIEVANCE_TYPE_NAME { get; set; }
        public string PREFIX_CODE { get; set; }
        public string GRIEVANCE_TYPE_CODE { get; set; }
        public string HAS_SUB { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class GRIEVANCE_ATTACHMENTINFO: SUB_ATTACHMENT
    {
        public string ATTACHMENT_ID { get; set; }
        public string GRIEVANCE_ID { get; set; }
        public string ATTACHMENT_PATH { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_TYPE { get; set; }
    }

    public  class SUB_ATTACHMENT
    {
        public string FILE_TYPE { get; set; }
        public string FILE_NAME { get; set; }
    }
    public class GRIEVANCE_MEMBERINFO
    {
        public string G_MEMBER_ID { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class GRIEVANCE_REDRESSALINFO
    {
        public string REDRESSAL_ID { get; set; }
        public string GRIEVANCE_ID { get; set; }
        public string REDRESSAL_CONTENT { get; set; }
        public string REP_DATE_TIME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class GRIEVANCE_REAMINDERINFO
    {
        public string REMINDER_ID { get; set; }
        public string GRIEVANCE_ID { get; set; }
        public string DATE { get; set; }
        public string TIME { get; set; }
        public string IS_READ { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_TYPE { get; set; }
    }
    public class GRIEVANCE_REMAIND
    {
        public string REMINDER_ID { get; set; }
        public string GRIEVANCE_ID { get; set; }
        public string DATE { get; set; }
        public string TIME { get; set; }
        public string IS_VIEWED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class GRIEVANCE_USERINFO
    {
        public string GRIEVANCE_ID { get; set; }
        public string USER_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string PHOTO { get; set; }
        public string FIRST_NAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string CLASS_NAME { get; set; }
        public string GRIEVANCE_CODE { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GRIEVANCE_TYPE_NAME { get; set; }
        public string GRIEVANCE_SUBJECT { get; set; }
        public string GRIEVANCE_CONTENT { get; set; }
        public string ATTACHMENT_PATH { get; set; }
        public string SUB_DATE_TIME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STATUS { get; set; }
        public string IS_READ { get; set; }
        public string IS_VIEWED { get; set; }
    }
    public class GRIEVANCE_VIEWINFO : GRIEVANCE_USERINFO
    {

        public string STAFF_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string REDRESSAL_CONTENT { get; set; }
        public string REP_DATE_TIME { get; set; }
       
    }

    public class REOPEN_GREIVANCE
    {
        public string SUB_DATE_TIME { get; set; }
        public string STAFF_ID { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GREIVANCE_TYPE_NAME { get; set; }
        public string STATUS { get; set; }
        public string CLASS_ID { get; set; }
    }
    public class GRIEVANCE_REPORT: REOPEN_GREIVANCE
    {
        public string START_DATE { get; set; }
        public string MONTH { get; set; }
        public string DAY_ID { get; set; }
        public string END_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }
    public class GREIVANCE_TOTAL
    {
        public string TOTAL_COMPLAINT { get; set; }
        public string REPLIED_COMPLAINT { get; set; }
        public string PENDING_COMPLAINT { get; set; }
        public string IS_DELETED { get; set; }
    }
   
    public class GRIEVANCE_MEMEBERINFO
    {
        public string G_MEMBER_ID { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ROLE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }    
    }
    public class GRIEVANCE_GRIEVANCEINFO
    {
        public string GRIEVANCE_ID { get; set; }
       
        public string USER_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string SENDER_NAME { get; set; }
        public string GRIEVANCE_CODE { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GRIEVANCE_TYPE_NAME { get; set; }
        public string GRIEVANCE_TITLE { get; set; }
        public string GRIEVANCE_SUBJECT { get; set; }
        public string GRIEVANCE_CONTENT { get; set; }
        public string SUB_DATE_TIME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ASSIGN_TO { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ROLE_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class GRIEVANCEINFOANDREDRASSALINFO
    {
        public string GRIEVANCE_ID { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GRIEVANCE_TYPE_NAME { get; set; }
        public string SENDER_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string STU_PHOTO { get; set; }
        public string GRIEVANCE_SUBJECT { get; set; }
        public string GRIEVANCE_CONTENT { get; set; }
        public string SUB_DATE_TIME { get; set; }
        public string GRIEVANCE_TITLE { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_PHOTO { get; set; }
        public string REDRESSAL_CONTENT { get; set; }
        public string REPLY_DATE_TIME { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string ATTACHMENT{ get; set; }
        public string REPLY_ATTACHMENT { get; set; }
        public string GRIEVANCE_COUNT { get; set; }
        public string STATUS { get; set; }
        public string TOTAL_MEMBERS { get; set; }
        public string UN_ASSIGNED { get; set; }
    }
    public class GRIEVANCEMEMBERSINFO
    {
        public string G_MEMBER_ID { get; set; }
        public string GRIEVANCE_TYPE_ID { get; set; }
        public string GRIEVANCE_TYPE_NAME { get; set; }
        public string STAFF_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLE { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLES { get; set; }
        
       
    }
    
}
