using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class LibraryModel
    {

    }
    public class CIRCULATION_FIELDS
    {
        public List<LI_BOOK_CIRCULATION> liBookCirculation { get; set; }
        public LI_USER_ACCOUNT objUserAccount { get; set; }
        public string sMessage { get; set; }
        public string sErrorCode { get; set; }
        public List<GRID_CLASS> liGridClass = new List<GRID_CLASS>();
        public List<LI_BOOK_RESERVE> liBookReservation { get; set; }

    }
    public class CirculationArgs
    {
        public bool sCheckinStatus = false;
        public List<LI_CHECKIN_INFO> liCheckInfo = new List<LI_CHECKIN_INFO>();
        public LI_CHECKIN_INFO objCheckInfo;
        public List<LI_USER_ACCOUNT> liUserAccount;

    }
    public class IssueAppend
    {
        public static String[] str = new string[4];
        public static int rCount = 0;

    }

    public class LI_USER_ACCOUNT
    {
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string RFID { get; set; }
        public string BOID { get; set; }
        public string ROLE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string PHOTO { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MOBILENO { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string EMAILID { get; set; }
        public string BAR_CODE { get; set; }
        public string USER_TYPE { get; set; }
        public string USER_TYPE_NAME { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string SCAN_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string CHECKIN_ID { get; set; }
    }

    public class LI_BOOK_INFO
    {
        public string BOOK_ID { get; set; }
        public string ACCESS_NO { get; set; }
        public string CLASS_NO { get; set; }
        public string BOOK_NO { get; set; }
        public string LANGUAGE { get; set; }
        public string AUTHOR { get; set; }
        public string PERSONAL_AUTHOR1 { get; set; }
        public string PERSONAL_AUTHOR2 { get; set; }
        public string JOINED_AUTHOR1 { get; set; }
        public string JOINED_AUTHOR2 { get; set; }
        public string CORPORATE_AUTHOR1 { get; set; }
        public string CORPORATE_AUTHOR2 { get; set; }
        public string DOC_TITLE { get; set; }
        public string PARALLEL_TITLE { get; set; }
        public string NOTES { get; set; }
        public string PHYDESC { get; set; }
        public string ILLUSTRATE { get; set; }
        public string STATUS { get; set; }
        public string PRICE { get; set; }
        public string DOP { get; set; }
        public string ISBN_NO { get; set; }
        public string DISIPLINE { get; set; }
        public string TAPS { get; set; }
        public string DETAILS { get; set; }
        public string VC { get; set; }
        public string NAME_OF_PUBLISHER { get; set; }
        public string PLACE_OF_PUBLISHER { get; set; }
        public string SERIES { get; set; }
        public string RFID { get; set; }
        public string LIB_CATEGORY { get; set; }
        public string DOC_CATEGORY { get; set; }
        public string ENTRY_DATE { get; set; }
        public string BOOK_PUBLISH_YEAR { get; set; }
        public string BARCODE { get; set; }
        public string RACK_ID { get; set; }
        public string SUB_ACCOUNT { get; set; }
        public string KEYWORD { get; set; }
        public string INVOICE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string LI_BOOK_STATUS { get; set; }
        public string C_STATUS { get; set; }
        public string STATUS_MESSAGE { get; set; }
        public string IS_CIRCULATION { get; set; }
        public string STATUS_NAME { get; set; }
    }
    public class LI_BOOK_DETAILS
    {
        public string BOOK_ID { get; set; }
        public string RACK_ID { get; set; }
        public string ACCESS_NO { get; set; }
        public string CLASS_NO { get; set; }
        public string BOOK_NO { get; set; }
        public string LANGUAGE { get; set; }
        public string PERSONAL_AUTHOR1 { get; set; }
        public string PERSONAL_AUTHOR2 { get; set; }
        public string JOINED_AUTHOR1 { get; set; }
        public string JOINED_AUTHOR2 { get; set; }
        public string CORPORATE_AUTHOR1 { get; set; }
        public string CORPORATE_AUTHOR2 { get; set; }
        public string DOC_TITLE { get; set; }
        public string PARALLEL_TITLE { get; set; }
        public string NOTES { get; set; }
        public string PHYDESC { get; set; }
        public string ILLUSTRATE { get; set; }
        public string STATUS { get; set; }
        public string PRICE { get; set; }
        public string DOP { get; set; }
        public string ISBN_NO { get; set; }
        public string DISIPLINE { get; set; }
        public string TAPS { get; set; }
        public string DETAILS { get; set; }
        public string VC { get; set; }
        public string NAME_OF_PUBLISHER { get; set; }
        public string PLACE_OF_PUBLISHER { get; set; }
        public string SERIES { get; set; }
        public string LIB_CATEGORY { get; set; }
        public string BOOK_PUBLISH_YEAR { get; set; }
        public string SUB_ACCOUNT { get; set; }
        public string KEYWORD { get; set; }
        public string INVOICE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string LI_BOOK_STATUS { get; set; }
        public string ENTRY_DATE { get; set; }
        public string RFID { get; set; }
        public string BARCODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        // Extra
        public string LC { get; set; }
        public string CLASS_ID { get; set; }
        public string SUBJECT_ID { get; set; }
        public string AUTHOR_ID { get; set; }
        public string IS_CIRCULATION { get; set; }
        public string BOOK_TITLE { get; set; }
        public string BOOK_STATUS { get; set; }
        public string ISSUE_DATE { get; set; }
        public string RETURN_DATE { get; set; }
        public string BOID { get; set; }
        public string SERIESCODE { get; set; }
        public string IS_ARTICLE { get; set; }
        public string ARTICLE_TYPE { get; set; }
        public string PAGES { get; set; }
        public string ABSTRACT { get; set; }
        public string LIB_CODE { get; set; }
        public string AUTHOR { get; set; }
        public string DOC_CATEGORY { get; set; }
        public string BAR_CODE { get; set; }
        public string RESERVE_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string COLLECTION_NAME { get; set; }
    }

    public class LI_USER_DETAILS
    {
        public string USER_ACCOUNT_ID { get; set; }
        public string USER_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string ROLE { get; set; }
        public string BOID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string LAST_LOGIN { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string MEMBER_TYPE { get; set; }
        public string DEGREE { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string PINCODE { get; set; }
        public string BLOOD_GROUP_ID { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }
        public string REGISTERED_DATE { get; set; }
        public string EXPIRED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PHOTO { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string RFID { get; set; }
        public string MAX_BOOKS { get; set; }
        public string BAR_CODE { get; set; }
    }

    public class LI_USER_PROVISIONS : LI_USER_DETAILS
    {
        public string PROVISION_ID { get; set; }

        public string EXTRA_BOOKS { get; set; }

    }
    public class GRID_CLASS
    {
        public string BOOK_ID { get; set; }
        public string ISSUE_ID { get; set; }
        public string RESERVE_ID { get; set; }
        public string STATUS { get; set; }
        public string INDEX { get; set; }
        public string COLOR { get; set; }
        public string IS_TRANSACTION_COMPLETED { get; set; }

    }
    public class LI_LOGIN_DETAILS
    {
        public string LOGIN_ID { get; set; }
        public string USER_ID { get; set; }
        public string LOGIN_DATE { get; set; }
        public string EXIT_DATE { get; set; }
        public string NO_OF_ATTEMPT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string SPENT_TIME { get; set; }
        public string IS_DELETED { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
    }


    public class LI_RULES
    {
        public string RULE_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string ROLE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string NO_OF_BOOKS { get; set; }
        public string NO_OF_DAYS { get; set; }
        public string NO_OF_RENEW_DAYS { get; set; }
        public string PER_DAY_DUE_COST { get; set; }
        public string REVISED_DUE_DATES { get; set; }
        public string REVISED_DUE_COST { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

        // Extra ...
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
    }
    public class LI_PERIODS
    {
        public string PERIOD_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string ROLE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string SEM_BEGIN_DATE { get; set; }
        public string SEM_END_DATE { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_LI_RACKS
    {
        public string RACK_ID { get; set; }
        public string RACK_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class SUP_LI_CELLS
    {
        public string CELL_ID { get; set; }
        public string CELL_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class LI_RACK_INFO
    {
        public string RACK_INFO_ID { get; set; }
        public string RACK_INFO { get; set; }
        public string RACK_ID { get; set; }
        public string RACK { get; set; }
        public string CELL { get; set; }

        public string CELL_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }

    public class LI_DUES
    {
        public string DUES_ID { get; set; }
        public string USER_ID { get; set; }
        public string BOOK_ID { get; set; }
        public string ISSUE_ID { get; set; }
        public string REASON { get; set; }
        public string FINE_AMOUNT { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_PAID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class LI_BOOK_ISSUE
    {
        public string ACADEMIC_YEAR { get; set; }
        public string ISSUE_ID { get; set; }
        public string LI_BOOK_RETURN { get; set; }
        public string USER_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string USER_TYPE_NAME { get; set; }
        public string ROLE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string BOOK_ID { get; set; }
        public string BOOK_TITLE { get; set; }
        public string AUTHOR { get; set; }
        public string ISSUE_DATE { get; set; }
        public string ISSUE_BY { get; set; }
        public string STATUS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_SMS_SENT { get; set; }
        public string IS_RENEWED { get; set; }
        public string ACTUAL_RETURNED_DATE { get; set; }
        public string IS_RETURNED { get; set; }
        public string RECEIVED_BY { get; set; }
        public string RETURN_DATE { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string BOOK_STATUS { get; set; }
        public string ACCESS_NO { get; set; }
        public string ISBN_NO { get; set; }
        public string FINE { get; set; }
        public string BALANCE { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string FIRST_NAME { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string IS_DAMAGED { get; set; }
        public string IS_MISSED { get; set; }
        public string IS_RETURN_CONFIRMED { get; set; }
        public string IS_DSMS_SENT { get; set; }
        public string DSMS_SENT_DATE { get; set; }
        public string IS_MSMS_SENT { get; set; }
        public string MSMS_SENT_DATE { get; set; }
        public string IS_RCSMS_SENT { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string DUES_ID { get; set; }
        public string IS_RESERVE_APPROVED { get; set; }
        public string TOTAL_COUNT { get; set; }
        public string RUN_NUMBER { get; set; }
    }

    public class LI_ISSUE_DUES
    {
        public string DUES_ID { get; set; }
        public string ISSUE_ID { get; set; }
        public string BALANCE { get; set; }
        public string REASON { get; set; }
        public string DUE_FINE_AMOUNT { get; set; }
        public string REVISED_DUE_FINE_AMOUNT { get; set; }
        public string DAMAGE_FINE_AMOUNT { get; set; }
        public string MISSING_FINE_AMOUNT { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_PAID { get; set; }
        public string RECEIPT_NO { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string ENTRY_DATE { get; set; }
    }
    public class LI_BOOK_READING
    {
        public string READING_ID { get; set; }
        public string USER_ID { get; set; }
        public string BOOK_ID { get; set; }
        public string READING_DATE { get; set; }
        public string ISSUE_BY { get; set; }
        public string RETURN_DATE { get; set; }
        public string RECEIVED_BY { get; set; }
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string BOOK_STATUS { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
    }


    public class LI_BOOK_RESERVE
    {
        public string RESERVE_ID { get; set; }
        public string USER_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string ROLE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string BOOK_ID { get; set; }
        public string REASON { get; set; }
        public string ENTRY_DATE { get; set; }
        public string APPROVED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public string IS_APPROVED { get; set; }
        public string IS_COLLECTED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string COLLECTED_DATE { get; set; }
        public string IS_CANCELLED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string CANCELLED_DATE { get; set; }
        public string CANCELLED_BY { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string CANCEL_REASON { get; set; }
        public string IS_USER_CANCELLED { get; set; }
        public string IS_SYSTEM_CANCELLED { get; set; }
        public string FIRST_NAME { get; set; }
        public string USER_TYPE_NAME { get; set; }
        public string BOOK_TITLE { get; set; }
        public string BOOK_NO { get; set; }
        public string ACCESS_NO { get; set; }
        public string ISBN_NO { get; set; }
        public string AUTHOR { get; set; }
        public string U_BARCODE { get; set; }
        public string U_RFID { get; set; }
        public string IS_CSMS_SENT { get; set; }
        public string IS_ASMS_SENT { get; set; }
        public string STATUS { get; set; }
        public string COLLECTED_BY { get; set; }
        public string DAY_ID { get; set; }
        public string DAYS { get; set; }
        public string STUDENT_ID { get; set; }
        public string LI_BOOK_COLLECTION_ID { get; set; }

    }

    public class LI_LOGIN_FIELDS
    {
        public string USER_ID { get; set; }
        public string RFID { get; set; }
        public string BOID { get; set; }
        public string FIRST_NAME { get; set; }
        public string CURRENT_DATE { get; set; }
        public string CURRENT_TIME { get; set; }
        public string VISITORS { get; set; }
        public string LIB_HRS { get; set; }
        public string STATUS { get; set; }
        public string MESSAGE { get; set; }
        public string PHOTO { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string MAX_BOOKS { get; set; }
        public string DEGREE { get; set; }
    }

    public class LI_BOOK_CIRCULATION : LI_BOOK_ISSUE
    {




    }

    public class ListBookIssue : LI_BOOK_DETAILS
    {
        public string USER_ID { get; set; }
        public string ISSUE_ID { get; set; }
        public string PER_DAY_DUE_COST { get; set; }
        public string REVISED_DUE_DATES { get; set; }
        public string REVISED_DUE_COST { get; set; }
        public string DAY_ID { get; set; }
        public string DUES { get; set; }
        public string DUE_FINE_AMOUNT { get; set; }
        public string REVISED_DUE_FINE_AMOUNT { get; set; }
        public string NO_OF_DAYS { get; set; }
        public string IS_RENEWED { get; set; }
        public string NO_OF_RENEW_DAYS { get; set; }
        public string FINE_AMOUNT { get; set; }
        public string REASON { get; set; }
        public string DUES_ID { get; set; }
        public string DAY_ORDER_DATE { get; set; }

    }
    public class LI_BOOK_TRANSACTION
    {
        public string ISSUE_ID { get; set; }
        public string RESERVE_ID { get; set; }
        public string RESERVE_CANCEL_REASON { get; set; }
        public string BOOK_STATUS { get; set; }
        public string FINE { get; set; }
    }

    public class Save_Json_List
    {
        public List<LI_BOOK_ISSUE> liBookIssue { get; set; }
        public List<LI_BOOK_TRANSACTION> liBookTransaction { get; set; }

        public List<LI_BOOK_RESERVE> liBookReserve { get; set; }
        public List<LI_BOOK_DETAILS> liBookDetails { get; set; }
        public List<LI_BOOK_READING> liBookReading { get; set; }
        public List<LI_USER_PROVISIONS> liUserProvision { get; set; }
    }

    public class LI_REPORT_FIELDS : LI_BOOK_DETAILS
    {
        public bool CHECKIN_ID { get; set; }
        public bool CALL_NO { get; set; }
        public bool BILL_NO { get; set; }
        public string TOTAL_COUNT { get; set; }
        public bool BILL_AMOUNT { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string AC_YEAR { get; set; }
        public string SUBJECT { get; set; }
        public string CHECKIN { get; set; }
        public string ISSUE_ID { get; set; }
        public string USER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string MEMBER_TYPE { get; set; }
        public string PHOTO { get; set; }
        public string CLASS_NAME { get; set; }
        public string COURSE_CODE { get; set; }

        public string ISSUE_BY { get; set; }
        public string RECEIVED_BY { get; set; }
        public string ISSUE_STATUS { get; set; }
        //public string RESERVE_ID { get; set; }
        public string RESERVE_DATE { get; set; }
        public string REASON { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string MESSAGE_CONTENT { get; set; }
        public string SENT_DATE { get; set; }
        public string FINE_AMOUNT { get; set; }
        public string IS_PAID { get; set; }
        public string LOGIN_ID { get; set; }
        public string LOGIN_DATE { get; set; }
        public string EXIT_DATE { get; set; }
        public string NO_OF_ATTEMPT { get; set; }
        public string SPENT_TIME { get; set; }
        public string DEGREE { get; set; }
        public string REGISTERED_DATE { get; set; }
        public string EXPIRED_DATE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string LAST_LOGIN { get; set; }
        public string DUES_ID { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string PUBLISHER { get; set; }

        public string RACK_NO { get; set; }
        public string SUBJECT_NAME { get; set; }
        public string LANGUAGE_NAME { get; set; }
        public string USERNAME { get; set; }
        public string IS_USER_CANCELLED { get; set; }
        public string IS_SYSTEM_CANCELLED { get; set; }
        public string CANCELLED_SMS { get; set; }
        public string APPROVED_SMS { get; set; }
        public string SYSTEM_CANCELLED_SMS { get; set; }
        public string APPROVED_DATE { get; set; }
        public string CANCELLED_DATE { get; set; }
        public string IS_CANCELLED { get; set; }
        public string LI_BOOK_COLLECTION_ID { get; set; }

        // Extra
        public string ACTUAL_RETURNED_DATE { get; set; }
        public string CHECKIN_DATE { get; set; }
        public string CHECKIN_TIME { get; set; }
        public string CHECKOUT_DATE { get; set; }
        public string CHECKOUT_TIME { get; set; }
        public string USER_TYPE_NAME { get; set; }
        public string COUNT { get; set; }
        public string REVIEW_ID { get; set; }
        public string REVIEW { get; set; }
        public string BOOK_ID { get; set; }
        public string ACCESS_NO { get; set; }
        public string BOOK_TITLE { get; set; }
        public string AUTHOR { get; set; }
        public string ISSUE_DATE { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string CLASS_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string COURSE_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string COURSE_TITLE { get; set; }
        public string STAFF_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string STUDENT_NAME { get; set; }
        public string COMPONENT { get; set; }
        public string GROUP_MARK { get; set; }
        public string COMPONENT_ID { get; set; }
        public string REVIEWED { get; set; }
        public string STUDENT_ID { get; set; }
        public string SEC_MARK { get; set; }
        public string SUBMITED { get; set; }
        public string GROUP_ID { get; set; }
        public string PER_BOOK_MARK { get; set; }
        public string ERA_SETTING_ID { get; set; }
        public string PERSONAL_AUTHOR1 { get; set; }
        public string BOOK_NO { get; set; }
        public string CIA_GROUP_COMPONENT_ID { get; set; }
        public string COURSE_GROUP_ID { get; set; }
        public string ERA_COMPONENT_ID { get; set; }
        public string COURSE_GROUP_MARK { get; set; }
        public string TOTAL_MARK { get; set; }
        public string MAX_MARK { get; set; }
        public string ENTRY_ID { get; set; }
        public string CIA_GROUP_MARK_ID { get; set; }
        public string SEMESTER_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_READING { get; set; }
        public string IS_WRITING { get; set; }


    }
    public class USER_DETAILS_FIELDS
    {
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string PHOTO { get; set; }
        public string ROLES { get; set; }
        public string MOBILENO { get; set; }
        public string EMAILID { get; set; }
        public string BOID_NUMBER { get; set; }
        public string BOID { get; set; }
        public string RFID { get; set; }
        public string USER_ID { get; set; }
        public string STATUS { get; set; }
        public string STATUS1 { get; set; }
        public string MEMBER_TYPE { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string PINCODE { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }

    }

    public class LI_AUTHOR_INFO
    {
        public string AUTHOR_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PUBLISHER { get; set; }
        public string PLACE { get; set; }
        public string ENTRY_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }


    public class LI_SMS_FIELDS
    {
        public string FIRST_NAME { get; set; }
        public string USER_ID { get; set; }
        public string BOOK_TITLE { get; set; }
        public string MESSAGE_CONTENT { get; set; }
        public string MOBILE { get; set; }
        public string C_DATE { get; set; }
        public string ELASPED_TIME { get; set; }
        public string C_TIME { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }

    }


    public class LI_EXTERNAL_USER_INFO
    {
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string USER_TYPE { get; set; }
        public string BOID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string DEGREE { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string BLOOD_GROUP_ID { get; set; }
        public string REGISTERED_DATE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PHOTO { get; set; }
        public string RFID { get; set; }
        public string STATE { get; set; }
        public string PINCODE { get; set; }
        public string CITY { get; set; }
        public string BAR_CODE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string COUNT_FIELD { get; set; }
        public string RUN_NUMBER { get; set; }
    }

    public class LI_CHECKIN_INFO
    {
        public string CHECKIN_ID { get; set; }
        public string USER_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASSLEVEL { get; set; }
        public string USER_TYPE { get; set; }
        public string ROLE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string CHECKIN_DATE { get; set; }
        public string IS_CHECKOUT { get; set; }
        public string CHECKOUT_DATE { get; set; }
        public string IS_SYSTEM_CHECKOUT { get; set; }
        public string SEMESTER_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }


    public class SUP_LI_SEARCH_TEXT
    {
        public string SEARCH_TEXT_ID { get; set; }
        public string FIELD_NAME { get; set; }
        public string SEARCH_TEXT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class SUP_LI_BOOK_COLLECTION
    {
        public string COLLECTION_NAME { get; set; }
        public string COLLECTION_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class USER_ACCOUNT
    {
        public string USER_ACCOUNT_ID { get; set; }
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string RFID { get; set; }
        public string BOID { get; set; }
        public string ROLE { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string PHOTO { get; set; }
        public string FIRST_NAME { get; set; }
        public string STUDENT_ID { get; set; }
        public string LAST_NAME { get; set; }
        public string MOBILENO { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string EMAILID { get; set; }
        public string BAR_CODE { get; set; }
        public string USER_TYPE { get; set; }
        public string PROGRAMME_LEVEL { get; set; }
        public string SCAN_ID { get; set; }
        public string AC_YEAR { get; set; }
        public string SEMESTER { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string CHECKIN_ID { get; set; }
        public string CHECKIN_DATE { get; set; }
        public string IS_CHECKOUT { get; set; }

    }

    public class EXTERNAL_USERS
    {
        public string EXTERNAL_USER_ID { get; set; }
        public string EXTERNAL_USER_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string DOB { get; set; }
        public string MAIL_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string COLLEGE_NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ROLE { get; set; }
        public string USER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class LISTENING_AUDIO_MODEL
    {
        public string LISTENING_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string AUDIO_URL { get; set; }
        public string AVAILABLE_FROM_DATE { get; set; }
        public string AVAILABLE_TO_DATE { get; set; }
        public string DURATION_SECONDS { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_AT { get; set; }
        public string UPDATED_AT { get; set; }
        public string CLASS_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string QUESTION_ID { get; set; }
        public string HAS_ATTEMPTED { get; set; }
        public string ANSWER_ID { get; set; }
        public string STUDENT_ID { get; set; }

    }
    public class LISTENING_QUIZ_QESTION
    {
        public string LISTENING_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string QUESTION_TEXT { get; set; }
        public string OPTION_A { get; set; }
        public string OPTION_B { get; set; }
        public string OPTION_C { get; set; }
        public string OPTION_D { get; set; }
        public string CORRECT_ANSWER { get; set; }
        public string CLASS_YEAR { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string SELECTED_ANSWER { get; set; }
        public string STUDENT_ID { get; set; }
        public string ANSWER_ID { get; set; }
        public string SUBMITTED_AT { get; set; }
        public string RESULT { get; set; }
        public string DESCRIPTION { get; set; }
        //public string CORRECT_ANSWER { get; set; }
        //public string QUESTION_TEXT { get; set; }
        //public string DESCRIPTION { get; set; }
        //public string DESCRIPTION { get; set; }
    }
    public class LISTENING_QUIZ_ANSWER
    {
        public string LISTENING_ID { get; set; }
        public string QUESTION_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string QUESTION_TEXT { get; set; }

        public string CORRECT_ANSWER { get; set; }
        public string SELECTED_ANSWER { get; set; }
        public string USER_ID { get; set; }
        public string ANSWER_ID { get; set; }
        public string SUBMITTED_AT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string STUDENT_ID { get; set; }

    }
    public class JSONLibraryModel
    {
        public List<LI_REPORT_FIELDS> LiJsonIssueBookRemark = new List<LI_REPORT_FIELDS>();
    }
}
