using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class HostelModel
    {
    }

    public class HOSTEL_JSON_MODEL
    {
        public List<HOSTEL_REGISTRATION> liHostel = new List<HOSTEL_REGISTRATION>();
        public List<STU_RELATION> liRelation = new List<STU_RELATION>();
    }
    public class HOSTEL_REGISTRATION : STU_ADDRESS
    {
        public string RENEWAL_REGISTRATION_ID { get; set; }
        // public string STUDENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string HOSTEL_ID { get; set; }
        public string ROOM_NO { get; set; }
        public string PARISH_NAME { get; set; }
        public string SELECTION_DATE { get; set; }
        public string SELECTED_BY { get; set; }
        public string COLLEGE_RELATION { get; set; }
        public string STUDENT_INFO { get; set; }
        public string FAMILY_PHOTO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_SUBMITTED { get; set; }
        public string IS_ACTIVE { get; set; }
        //  public string IS_DELETED { get; set; }
        public string STATUS { get; set; }
        public string RELATION_ID { get; set; }
    }

    public class STU_ADDRESS
    {
        public string ADDRESS_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string C_DOOR_DETAIL { get; set; }
        public string C_STREET { get; set; }
        public string C_VILLAGE_AREA { get; set; }
        public string C_POST_PLACE_TOWN { get; set; }
        public string C_TALUK { get; set; }
        public string C_CITY { get; set; }
        public string C_DISTRICT { get; set; }
        public string C_PINCODE { get; set; }
        public string C_COUNTRY { get; set; }
        public string P_DOOR_DETAIL { get; set; }
        public string P_STREET { get; set; }
        public string P_VILLAGE_AREA { get; set; }
        public string P_POST_PLACE_TOWN { get; set; }
        public string P_TALUK { get; set; }
        public string P_CITY { get; set; }
        public string P_DISTRICT { get; set; }
        public string P_PINCODE { get; set; }
        public string P_COUNTRY { get; set; }
        public string IS_DELETED { get; set; }
        public string P_PHONENO { get; set; }
        public string C_PHONENO { get; set; }
    }

    public class FetchClassAndProgram
    {
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_CODE { get; set; }
        public string CLASS_CODE { get; set; }
        public string CLASS_ID { get; set; }
        public string STUDENT_ID { get; set; }
    }

    public class STU_RELATION
    {
        public string RELATION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string RELATION_NAME { get; set; }
        public string RELATION_SHIP { get; set; }
        public string OCCUPATION { get; set; }
        public string INCOME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class HOSTEL_STUDENT_INFO
    {
        public string STU_HOSTEL_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string HOSTEL_NAME { get; set; }
        public string ROOM_NO { get; set; }
        public string COT_NO { get; set; }
        public string BLOCK_NAME { get; set; }
        public string BLOCK_ID { get; set; }
        public string HOSTEL_ID { get; set; }
        public string BLOCK_INCHARGE { get; set; }
        public string ADMITTED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ADMITTED_YEAR { get; set; }
        public string IS_LEFT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string FIRST_NAME { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string SHIFT_NAME { get; set; }      
        public string CLASS_YEAR { get; set; }
        public string STU_MOBILENO { get; set; }
        public string HEAD { get; set; }
        public string BALANCE_AMOUNT { get; set; }

    }

    public class SUP_BLOCK
    {
        public string BLOCK_ID   {get;set;}
        public string BLOCK_NAME {get;set;}
        public string HOSTEL_ID  {get;set;}
        public string IS_ACTIVE  {get;set;}
        public string IS_DELETED { get; set; }
    }
    public class JsonStudentPromotion
    {
        public List<HOSTEL_STUDENT_INFO> StudentPromotionList { get;set;}
    }
    public class STU_REJOIN
    {
        public string REJOIN_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_LEFT { get; set; }
        public string IS_DELETED { get; set; }
        public string SEMESTER { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string OLD_ROLL_NO { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string ENTRY_ID { get; set; }
        public string REMARKS { get; set; }
        public string ENTRY_DATE { get; set; }
        public string RFID { get; set; }
        public string BIOD_NUMBER { get; set; }
        public string BAR_CODE { get; set; }
        public string CLASS_ID { get; set; }
        public string BATCH { get; set; }
        public string STU_CLASS_ID { get; set; }


    }

}
