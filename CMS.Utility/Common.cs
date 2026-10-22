using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Reflection;

namespace CMS.Utility
{
    public class Common
    {
        public const string UserID = "UserID ";
        public const string CategoryID = "CategoryID ";
        public const string SubCategory1ID = "SubCategory1ID ";
        public const string SubCategory2ID = "SubCategory2ID ";
        public const string MerchantID = "MerchantID ";
        public const int OffsetItems = 200;
        public const int LinkPageSize = 20;
        public const int LinkMaxPageSize = 10;
        public const string SEMESTER = "SEMESTER";
        public static class ACDEMIC_YEAR
        {
            public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";
            public const string AC_YEAR = "AC_YEAR";
        }
        public static class PageTitles
        {

        }

        public enum PageIndex
        {

        }

        public enum DropDownList
        {

        }

        // Status ....
        public static class STATUS
        {
            public static string Registered = "1";
            public static string Submitted = "2";
            public static string Selected = "3";
            public static string Verified = "4";
            public static string Admitted = "5";
            public static string Transfer = "6";
            public static string Pending = "7";
        }
        public static class DATABASE_NAMES
        {
            public static string ERP_DB = "CMS_SJCTNC";
            public static string WEBSITE_DB = "SJCTNC_WEBSITE_LIVE";
            public static string RESULT_DB = "SJC_EXAM_RESULT";
        }
        
        public static class PaymentMode
        {
            //this ids should not change 
            public static string Cash = "1";
            public static string DD = "2";
            public static string Cheque = "3";
            public static string CreditCard = "4";
            public static string PaidInBank = "5";
            public static string Online = "6";
        }
        public static class FeeRoot
        {
            //this ids should not change 
            public static string CollegeFee = "1";
            public static string HostelFee = "2";
            public static string MessFee = "3";
            public static string HostelAndMess = "4";
            public static string BusFee = "5";
            public static string ConvocationFee = "6";
        }
        public static class RazorpayKeyWords
        {
            //this ids should not change 
            public static string AMOUNT = "amount";
            public static string account = "account";
            public static string CURRENCY = "currency";
            public static string NOTES = "notes";
            public static string TRANSFERS = "transfers";
            public static string INR = "INR";
        }
        public static class FrequencyType
        {
            //this ids should not change 
            public static string ExamFee = "1";
            public static string AdmissionFee = "2";
            public static string AdmissionApplicationFee = "3";
            public static string HostelFee = "4";
            public static string LibraryFee = "5";
            public static string MessFee = "6";
            public static string SemesterFee = "7";
            public static string BusFee = "8";
            public static string GateFee = "9";
            public static string RevaluationFee = "10";
            public static string ConvocationFee = "11";
            public static string Arrerfee = "12";
            public static string PhotoCopyFee = "13";
            public static string Supplementary = "14";

            public static string Term_II = "13";
            public static string Certificate = "14";
        }
        public static class RazorpayAccountType
        {
            //this ids should not change 
            public static string MarchantAccount = "1";
            public static string LinkedAccount = "2";

        }

        public static class status
        {
            //this ids should not change 
            public static string Approved = "1";
            public static string Rejected = "2";

        }
        public static class Quiz_Question_Type
        {
            //this ids should not change 
            public static string ObjectiveType = "1";
            public static string FillInTheBlank = "2";
            public static string MatchTheFollowing = "3";

        }
        public static class Question_Type
        {
            //this ids should not change 
            public static string ObjectiveType = "1";
            public static string FillInTheBlank = "2";
            public static string MatchTheFollowing = "3";
            public static string EitherQuestions = "4";
            public static string ChooseBasedQuestions = "5";



        }
        public static class EXAM_TYPE
        {
            public static string INTERNAL = "1";
            public static string EXTERNAL = "2";
        }

        public static class PayUMoneyAPIsType
        {
            public static string BaseUrl = "1";
        }
        public static class FilePath
        {
            public static string SERVER_PATH = @"AttachMents\Assignments\";
            public static string TASK_FILE = @"Task\Submission\";
            public static string IMPORT_DATA_PATH = @"\ImportData\";
            public static string STUDENT_IMAGE_PATH = @"\StudentImages\";
            public static string STAFF_IMAGE_PATH = @"\StaffImages\";
            public static string IMPORT_DATA = "ImportData.xls";
            public static string BARCODE_IMAGE_PATH = @"Image\HCC\Barcode\";
        }

        public static class Messages
        {
            public static string InvalidUserCredentials = @"Invalid User Credentials !!!";
            public static string RecordsSavedSuccessfully = @"Record(s) Saved Successfully !!!";
            public static string RecordsUpdatedSuccessfully = @"Record(s) Updated Successfully !!!";
            public static string NoRecordsfound = @"No Record(s) Found !!!";
            public static string FailedToSaveRecords = @"Failed To Save Record(s) !!!";
            public static string FailedToUpdatedRecords = @"Failed To Update Record(s) !!!";
            public static string SessionIsExpiredPleaseLoginAgain = @"Session Is Expired Please Login Again !!!";
            public static string RecordDeletedSuccessfully = @"Record deleted successfully ...!";
            public static string FailedToDeletedTryAgain = @"Failed To Deleted Try Again ...!";
            public static string FailedToLoadTryAgain = @"Failed To Load Try Again ...!";
            public static string Success = @"Success ...!";
            public static string QueryIsEmpty = @"Query Is Empty ...!";
            public static string ModelIsNull = @"Model Is Null ...!";
            public static string MissingCoursePleaseMeetAdmin = @"Few Courses are missing for this Academic Year Please Meet Admin with Your Course Code";
            public static string LedgersAreNotMapped = @"Ledgers Are Not Mapped , Please meet exam registration committee";
            public static string CoursesAreNotAlloted = @"Courses Are Not Allotted";
            public const string PleaseConfigureMessageSetting = "Please Configure Message Setting !!!";
            public const string NoMobileNumbersAvailable = "No Mobile Numbers Available !!!";
            public const string LowMessageLimit = "Low Message Limit !!!";
            public static string RequestTimeOut = @"Sorry for the inconvenience, Time out error , Please Close your browser and register again.";
            public static string AccountNotAvailable = @"Sorry for the inconvenience,Account is not available.";
            public static string LedgersAreMapped = @"Ledgers Are Mapped Already !!!.";
            public static string PleaseConfigureExamHeads = @"PleaseConfigureExamHeads !.\n Please meet admin for more information ";
            public static string UserAccountNotCreated = @"UserAccount is not created!";
            public static string RecordsExistAlready = @"Record(s) Are Exist Already!.";
            public static string InvalidInput = @"Invalid Input!";
        }
        public static class LI_STATUS
        {
            public const string STACK_ROOM_SELF = "1";
            public const string REFERHALL_REFERENCE = "2";
            public const string DEPTLIB_STACK = "3";
            public const string TBLIB_SELF = "4";
            public const string THESIS_REFERENCE = "5";
            public const string STACKROOM_CIRCULATION = "6";
            public const string RAREBOOK_ARCHIVE = "7";
            public const string COURSERESERVE_REFERENCE = "8";
            public const string STACKROOM_BINDING = "9";
            public const string WEEDOUT = "10";
            public const string LOANBOOK_SECTION = "11";
            public const string BBANK_STACK = "12";
            public const string BBANK_CIRCULATION = "13";
            public const string VIRGINIA_HALL_REFERENCE = "14";
            public const string STACK_REPLACED = "15";

        }
        public static class CIRCULATION_MODE
        {
            public const string IS_CIRCULATION = "1";
        }
        public static class COLOR
        {
            public const string GREEN = "GREEN";
            public const string RED = "RED";
            public const string YELLOW = "YELLOW";
            public const string AQUA = "AQUA";
            public const string NORMAL = "NORMAL";
        }
        public static class LI_BOOK_STATUS
        {
            public const string CIRCULATION = "1";
            public const string RESERVE_REQUESTED = "2";
            public const string ISSUED = "3";
            public const string RETURNED = "4";
            public const string RESERVE_APPROVED = "5";
            public const string RESERVE_CANCELLED = "6";
            public const string RENEWED = "7";
            public const string RESERVED = "8";
            public const string DAMAGED = "9";
            public const string MISSED = "10";


        }
        public static class ErrorMessage
        {
            public static string Unauthorized = "Unathorized Access !!!";
            #region Informational Error Messages
            public static string Continue = "Continue";
            public static string SwitchingProtocols = "Switching Protocols";
            public static string Processing = "Processing";
            #endregion
            #region Success Messages
            public static string Ok = "OK";
            public static string Created = "Created";
            public static string Accepted = "Accepted";
            public static string NonAuthoritativeInformation = "Non-authoritative Information";
            public static string NoContent = "No Content";
            public static string ResetContent = "Reset Content";
            public static string PartialContent = "Partial Content";
            public static string MultiStatus = "Multi-Status";
            public static string AlreadyReported = "Already Reported";
            public static string IMUsed = "IM Used";
            #endregion
            #region Redirectional Error Messages
            public const string MultipleChoices = "Multiple Choices";
            public const string MovedPermanently = "Moved Permanently";
            public const string Found = "Found";
            public const string SeeOther = "See Other";
            public const string NotModified = "Not Modified";
            public const string UseProxy = "Use Proxy";
            public const string TemporaryRedirect = "Temporary Redirect";
            public const string PermanentRedirect = "Permanent Redirect";
            #endregion
            #region Client Error Messages
            public static string BadRequest = "Bad Request";
            //public static string Unauthorized = "Unauthorized";
            public static string PaymentRequired = "Payment Required";
            public static string Forbidden = "Forbidden";
            public static string NotFound = "Not Found";
            public static string MethodNotAllowed = "Method Not Allowed";
            public static string NotAcceptable = "Not Acceptable";
            public static string ProxyAuthenticationRequired = "Proxy Authentication Required";
            public static string RequestTimeout = "Request Timeout";
            public static string Conflict = "Conflict";
            public static string Gone = "Gone";
            public static string LengthRequired = "Length Required";
            public static string PreconditionFailed = "Precondition Failed";
            public static string PayloadTooLarge = "PayloadTooLarge";
            public static string RequestURITooLong = "Request-URI Too Long";
            public static string UnsupportedMediaType = "Unsupported Media Type";
            public static string RequestedRangeNotSatisfiable = "Requested Range Not Satisfiable";
            public static string ExpectationFailed = "Expectation Failed";
            public static string ImATeapot = "I'm a teapot";
            public static string MisdirectedRequest = "Misdirected Request";
            public static string UnprocessableEntity = "Unprocessable Entity";
            public static string Locked = "Locked";
            public static string FailedDependency = "Failed Dependency";
            public static string UpgradeRequired = "Upgrade Required";
            public static string PreconditionRequired = "Precondition Required";
            public static string TooManyRequests = "Too Many Requests";
            public static string RequestHeaderFieldsTooLarge = "Request Header Fields Too Large";
            public static string ConnectionClosedWithoutResponse = "Connection Closed Without Response";
            public static string UnavailableForLegalReasons = "Unavailable For Legal Reasons";
            public static string ClientClosedRequest = "Client Closed Request";
            #endregion
            #region Server Error Messages
            public static string InternalServerError = "Internal Server Error";
            public static string NotImplemented = "Not Implemented";
            public static string BadGateway = " Bad Gateway";
            public static string ServiceUnavailable = "Service Unavailable";
            public static string GatewayTimeout = "Gateway Timeout";
            public static string HTTPVersionNotSupported = "HTTP Version Not Supported";
            public static string VariantAlsoNegotiates = "Variant Also Negotiates";
            public static string InsufficientStorage = "Insufficient Storage";
            public static string LoopDetected = "Loop Detected";
            public static string NotExtended = "Not Extended";
            public static string NetworkAuthenticationRequired = "Network Authentication Required";
            public static string NetworkConnectTimeoutError = "Network Connect Timeout Error";
            #endregion
        }
        public static class ErrorCode
        {
            #region Informational Error Codes
            public static string Continue = "100";
            public static string SwitchingProtocols = "101";
            public static string Processing = "102";
            public static string SessionOut = "440";
            #endregion
            #region Success Codes
            public static string Ok = "200";
            public static string Created = "201";
            public static string Accepted = "202";
            public static string NonAuthoritativeInformation = "203";
            public static string NoContent = "204";
            public static string ResetContent = "205";
            public static string PartialContent = "206";
            public static string MultiStatus = "207";
            public static string AlreadyReported = "208";
            public static string IMUsed = "226";
            #endregion
            #region Redirectional Error Codes
            public const string MultipleChoices = "300";
            public const string MovedPermanently = "301";
            public const string Found = "302";
            public const string SeeOther = "303";
            public const string NotModified = "304";
            public const string UseProxy = "305";
            public const string TemporaryRedirect = "307";
            public const string PermanentRedirect = "308";
            #endregion
            #region Client Error Codes
            public static string BadRequest = "400";
            public static string Unauthorized = "401";
            public static string PaymentRequired = "402";
            public static string Forbidden = "403";
            public static string NotFound = "404";
            public static string MethodNotAllowed = "405";
            public static string NotAcceptable = "406";
            public static string ProxyAuthenticationRequired = "407";
            public static string RequestTimeout = "408";
            public static string Conflict = "409";
            public static string Gone = "410";
            public static string LengthRequired = "411";
            public static string PreconditionFailed = "412";
            public static string PayloadTooLarge = "413";
            public static string RequestURITooLong = "414";
            public static string UnsupportedMediaType = "415";
            public static string RequestedRangeNotSatisfiable = "416";
            public static string ExpectationFailed = "417";
            public static string ImATeapot = "418";
            public static string MisdirectedRequest = "421";
            public static string UnprocessableEntity = "422";
            public static string Locked = "423";
            public static string FailedDependency = "424";
            public static string UpgradeRequired = "426";
            public static string PreconditionRequired = "428";
            public static string TooManyRequests = "429";
            public static string RequestHeaderFieldsTooLarge = "431";
            public static string ConnectionClosedWithoutResponse = "444";
            public static string UnavailableForLegalReasons = "451";
            public static string ClientClosedRequest = "499";
            #endregion
            #region Server Error Codes
            public static string InternalServerError = "500";
            public static string NotImplemented = "501";
            public static string BadGateway = "502";
            public static string ServiceUnavailable = "503";
            public static string GatewayTimeout = "504";
            public static string HTTPVersionNotSupported = "505";
            public static string VariantAlsoNegotiates = "506";
            public static string InsufficientStorage = "507";
            public static string LoopDetected = "508";
            public static string NotExtended = "510";
            public static string NetworkAuthenticationRequired = "511";
            public static string NetworkConnectTimeoutError = "599";
            #endregion
        }
        public static class AppSettingKeys
        {
            public const string SolutionInfi = "SolutionInfi";
            public const string Mvaayoo = "Mvaayoo";
            public const string SMTPPORT = "smtpport";
            public const string Domain = "Host";
            public const string SSL = "SSL";
            public const string SMSMailID = "SMSMailID";
            public const string SMSMailPassword = "SMSMailPassword";
            public const string SMSNumber = "SMSNumber";
        }

        public static class Formats
        {
            public static string TimeFormat = "HH:mm:ss";
            public static string dd_MM_yyyy = "dd-MM-yyyy";
            public static string MM_dd_yyyy = "MM-dd-yyyy";
            public static string yyyy_MM_dd = "yyyy-MM-dd";
            //public static string TimeFormat = "HH:mm:ss";
            //public static string TimeFormat = "HH:mm:ss";
            //public static string TimeFormat = "HH:mm:ss";


        }
        public static class FBQUESTION_GROUP_IDS
        {
            public static string ForParacticals = "1,3,4,5,6";
            public static string NotForParacticals = "1,2,3,4,6";

        }
        public static class style
        {
            public static string alert_danger = "<div class='alert alert-danger alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>{0}</div> ";
            public static string alert_sucess = "<div class='alert alert-success alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>{0}</div> ";
            public static string alert_info = "<div class='alert alert-info alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>{0}</div> ";
            public static string alert_warning = "<div class='alert alert-warning alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>{0}</div> ";
            public static string display_none = "none ";
            public static string display_block = "block ";
        }
        public abstract class SessionMemeber
        {
            public const string SelectedRole = "SelectedRole ";

        }
        public abstract class URLPages
        {
            public static string ApplicationVirtualPath
            {
                get { return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath; }
            }

            //Route config key Name


            public static string SERVER_PATH = AppDomain.CurrentDomain.BaseDirectory + "\\ ";
            public static string STORE_PATH = @"AttachMents\Store\{0}\ ";
            public static string STORE_PATHMobile = @"AttachMents\Store\ ";
            public static string PROFILE_PATH = @"AttachMents\Profile\ ";
            public static string PROMOTION_PATH = @"AttachMents\PromotionImages\ ";
            public static string ADVERTISEMENT_PATH = @"AttachMents\AdvertisementImages\ ";
            public static string PRODUCTIMAGES_PATH = @"AttachMents\ProductImages\ ";
            public static string CLEARANCE_PATH = @"AttachMents\ClearanceImages\ ";
            public static string BANNER_PATH = @"AttachMents\BannerImages\ ";
            public static string CATALOG_PATH = @"AttachMents\CatalogImages\ ";
            public static string STUDENT_IMGAE_PATH = @"Image\SJC\StudentImages\";
            public static string STAFF_PROFILE_IMAGE = @"Image\SJC\Staff\profileImages\";
            public static string STAFF_PROFILE_FILE = @"\Image\SJC\Staff\ProfileFile\";
            public static string STAFF_IMGAE_PATH = @"Image\HCC\StaffImages\";
            public static string HALL_IMAGE = @"Image\HCC\HallImages\";
            public static string STUDENT_HOSTEL_PATH = @"Image\HCC\Hostel\";
            public static string ACTIVITY_IMAGE_PATH = @"Reports\ActivityImages\";
            public static string ACTIVITY_FILE_PATH = @"Reports\ActivityFiles\";
            public static string GRIEVANCEATTACHMENTPOST_FILE_PATH = @"Grievance\GrievanceAttachmentFiles\POST\";
            public static string GRIEVANCEATTACHMENTREPLY_FILE_PATH = @"Grievance\GrievanceAttachmentFiles\REPLY\";
            public static string LIBRARY_MEMBER_IMAGE_PATH = @"Image\HCC\LibraryMemberImages\";
            public static string ALUMNAE_MEMBER_IMAGE_PATH = @"Image\HCC\AlumnaeMemberImages\";
            public static string EXAM_PHOTOCOPY_IMAGE_PATH = @"Image\HCC\PhotoCopyImagesImages\";
            public static string ALUMNAE_PASSEDOUT_IMAGE_PATH = @"Image\HCC\AlumnaePassedoutImages\";
            public static string IQAC_IMAGE_PATH = @"\Image\SJC\IQAC_Uploads\";
            public static string STAFF_SIGN_PATH = @"Image\HCC\StaffSignImages\";
            public static string EXAM_ANSWER_PATH = @"Image\HCC\ExamAnswers\";
            public static string EXAM_STUDENT_LOG = @"Image\HCC\ExamStudentLog\";
            public static string WEB_ACHIEVEMENT = @"\Image\HCC\WebAchievement\";
            public static string ACHIEVEMENT_FILE_PATH = @"\Image\HCC\WebFilePath\";
            public static string PlacementNews_FILE_PATH = @"\Image\HCC\PlacementFile\";
            public static string PlacementNews_IMG_PATH = @"\Image\HCC\PlacementImg\"; 
            public static string InterViewPlaced_FILE_PATH = @"\Image\HCC\StuInterViewPlacedFile\";
            public static string InterViewPlaced_IMG_PATH = @"\Image\HCC\StuInterViewImg\";
            public static string PHD_supervisors_img = @"\Image\HCC\PhdSuperVisors\";
            public static string LMS_FILE_PATH = @"\Image\SJC\LMS\Coursewise_attach\";
            public static string LMS_ASSIGNMENT_PATH = @"\Image\SJC\LMS\Stu_Assignment\";
            public static string ALUMNAE_PROGRESSION = @"Image\HCC\ALUMNAE_PROGRESSION\";
            public static string ALUMNE_STUDENT_IMGAEPATH = @"Image\SJC\ALUMNAE_PHOTO\";
        }
        public static class MainMenu
        {
            public static string[] Dashboard = { "DSB" };
            public static string[] VirtualStore = { "VST_CAT", "VST_PRD", "VST_CAL", "VST_BNR", "VST_OFF", "VST_ADV", "VST_PRT", "VST_CLS" };
            public static string[] Administration = { "ADM_UD", "ADM_UR", "ADM_SD", "ADM_SS" };
            public static string[] Transaction = { "TRN_TUP", "TRN_WAL", "TRN_BH", "TRN_EAR" };
            public static string[] Notifications = { "NTF_ND", "NTF_MD", "NTF_IF", "NTF_MF" };
            public static string[] Service = { "RSE_RS" };
            public static string[] Reports = { "RPT_R1", "RPT_R2", "RPT_R3" };
        }
        /// <summary>
        /// Class to have all the delimiters 
        /// </summary>
        public class Delimiter
        {
            public static string PLUS = "+";
            public static string QUS = "?";
            public static string COMMA = ",";
            public static string COLON = ":";
            public static string DOLLAR = "$";
            public const string DOT = ".";
            public static string EQUAL = "=";
            public static string EMPTY = " ";
            public static string SINGLEQUOTE = "'";
            public static string OPENBRACKET = "(";
            public static string CLOSEBRACKET = ")";
            public static string OPENSQUAREBRAKET = "[";
            public static string CLOSESQUAREBRAKET = "]";
            public static string GREATERTHAN = ">";
            public static string LESSTHAN = "<";
            public static string ZERO = "0";
            public static string NOTEQUAL = "<>";
            public static string STAR = "*";
            public static string HASH = "#";
            public static string AT = "@";
            public static string ONE = "1";
            public const string SINGLEQUOTEWITHCOMMA = "',";
            public const string SINGLE_SPACE = " ";
            public const string SINGLE_BAR_WITH_SPACE = " | ";
            public const string IPHEN = "-";
            public static string Hide = "Hide";
            public static string Search = "Search";
            public static string Nil = "-Nil-";

            public const string DATE_SEPARATOR = "/";
            public const string AMPRESAND = "&";
            public const string UNDERSCORE = "_";

            public const string ARROW_MARK = " -->";
            public const string SplitDelimiter = "ᴥ";
        }
        /// <summary>
        /// This class contain error log table fields
        /// </summary>
        public class ErrorLog
        {
            public const string TBL_NAME = "TBL_NAME ";
            public const string LOG_ID = "LOG_ID ";
            public const string ERR_METHOD = "ERR_METHOD ";
            public const string CLASS_NAME = "CLASS_NAME ";
            public const string LINE_NO = "LINE_NO ";
            public const string USER_NAME = "USER_NAME ";
            public const string USER_ROLE = "USER_ROLE ";
            public const string DATE_TIME = "DATE_TIME ";
            public const string ERR_SOURCE = "ERR_SOURCE ";
            public const string TARGET_SITE = "TARGET_SITE ";
            public const string MESSAGE = "MESSAGE ";
            public const string STACK_TRACE = "STACK_TRACE ";

        }

        public static class Optimization
        {
            //Product optimization
            public static Int64 PRODUCTMAXSIZE = 500;// maaximum kb for optimization
            public static Int64 PRODUCTMAXWIDTH = 800;
            public static Int64 PRODUCTMAXHEIGHT = 600;

            //Banner optimization
            public static Int64 BANNERMAXSIZE = 512;
            public static Int64 BANNERMAXWIDTH = 1200;
            public static Int64 BANNERMAXHEIGHT = 480;
            //Catelog optimization
            public static Int64 CATELOGMAXSIZE = 512;
            public static Int64 CATELOGMAXWIDTH = 800;
            public static Int64 CATELOGMAXHEIGHT = 200;
            //Advertisment optimization
            public static Int64 ADVERTISMENTMAXSIZE = 512;
            public static Int64 ADVERTISMENTMAXWIDTH = 1200;
            public static Int64 ADVERTISMENTMAXHEIGHT = 480;

        }

        public static class Sup_li_Status
        {
            public static string Circulation = "0";
            public static string Issue = "1";
            public static string Retrun = "2";
            public static string Missing = "3";
            public static string Damaged = "4";
            public static string Reserved = "5";
            public static string Reading = "6";

        }
        public static class SMSFlag
        {
            public static string Sent = "1";
            public static string NotSent = "2";
        }
        public static class DeleteFlag
        {
            public static string Deleted = "1";
            public static string NotDeleted = "0";
        }
        /// <summary>
        /// 
        /// </summary>
        public static class PageFlag
        {
            public static int Select = 1;
            public static int Approve = 2;
            public static int Edit = 3;
            public static int Add = 4;
        }
        public static class BaseClass
        {
            public static string USER_ID = "";
        }
        public static class ROLE
        {
            public static string EVALUATORS = "19";
        }
        public static class SESSION_VARIABLES
        {
            // public static string sConnectionString = "CONNECTIONSTRING";

            public const string USER_ID = "USER_ID";
            public const string USERNAME = "USERNAME";
            public const string USER_TYPE = "USER_TYPE";
            public const string USER_NAME = "USER_NAME";
            public const string USER_ROLE_NAME = "USER_ROLE_NAME";
            public const string USER_ROLE_ID = "USER_ROLE_ID";
            public const string MENU_ITEMS = "MENU_ITEMS";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string CUR_ACADEMIC_YEAR = "CUR_ACADEMIC_YEAR";
            public const string ACADEMIC_YEAR_NAME = "ACADEMIC_YEAR_NAME";
            public const string CLASS_ID = "CLASS_ID";
            public const string FBCLASS_ID = "FBCLASS_ID";
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string USER_INFO = "USER_INFO";
            public const string SHIFT_ID = "SHIFT_ID";
            public const string CLASS_NAME = "CLASS_NAME";
            public const string PHOTO_PATH = "PHOTO_PATH";
            public const string USER_CODE = "USER_CODE";
            public const string COLLEGE_INFO = "COLLEGE_INFO";
            public const string COURSE_ID = "COURSE_ID";
            public const string PHOTO = "PHOTO";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string LAST_INSERT_ID = "LAST_INSERT_ID";
            public const string TABLE_PRIMARY_ID = "TABLE_PRIMARY_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";
            public const string COURSE_ROOT_ID = "COURSE_ROOT_ID";
            public const string CLSCRSSTF_ID = "CLSCRSSTF_ID";
            public const string FACULTY_ID = "FACULTY_ID";
            public const string COURSE_TYPE_ID = "COURSE_TYPE_ID";
            public const string PROGRAMME_ID = "PROGRAMME_ID";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
            public const string DEGREE_ID = "DEGREE_ID";
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string ALLOTMENT_ID = "ALLOTMENT_ID";
            public const string REGISTRATION_ID = "REGISTRATION_ID";
            public const string DAY_ORDER = "DAY_ORDER";
            public const string DAY_ORDER_DATE = "DAY_ORDER_DATE";
            public const string ACTIVE_SEMESTER_FOR_STAFF = "ACTIVE_SEMESTER_FOR_STAFF";
            public const string ACTIVE_SEMESTER_FOR_STUDENT = "ACTIVE_SEMESTER_FOR_STUDENT";
            public const string SETTING_ID = "SETTING_ID";
            public const string DEPARTMENT_TEMPLATE_ID = "DEPARTMENT_TEMPLATE_ID";
            public const string TT_TEMP_ID = "TT_TEMP_ID";
            public const string ACTIVE_SEMESTER_TYPE = "ACTIVE_SEMESTER_TYPE";
            public const string PAYMENT_RESPONSE = "PAYMENT_RESPONSE";
            public const string IS_HOLIDAY = "IS_HOLIDAY";
            public const string COLLEGE_DETAILS = "COLLEGE_DETAILS";
            public const string OTP = "OTP";
            public const string ADMITTED_BATCH = "ADMITTED_BATCH";
            public const string CLASS_LEVEL = "CLASS_LEVEL";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string ACTION_NAME = "ACTION_NAME";
            public const string CONTROLLER_NAME = "CONTROLLER_NAME";
            public const string CLASS_YEAR = "CLASS_YEAR";
            public const string ROLE_NAME = "ROLE_NAME";

            //public const string PERSON_NAME = "USERNAME";
            //public const string CONNECTION_STRING = "CONNECTION_STRING";
            //public const string HEADING1 = "HEADING1";
            //public const string HEADING2 = "HEADING2";
            //public const string SCHOOL_ID = "SCHOOL_ID";
            //public const string SCHOOL_CODE = "SCHOOL_CODE";
            //public const string LOGO_PATH = "LOGO_PATH";
            //public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            //public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";

        }

        public static class L_USER_TYPE
        {
            public const string TEACHING_STAFF = "1";
            public const string NON_TEACHING_STAFF = "2";
            public const string STUDENT = "3";
            public const string EXTERNAL = "4";
        }



        public static class NME_CLASS_COURSE_2017
        {
            public const string NME_CLASS_COURSE_ID = "NME_CLASS_COURSE_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string IS_DELETED = "IS_DELETED";

        }

        public static class NME_CLASS_COURSE_QUOTA_2017
        {

            public const string NME_CLASS_COURSE_QUOTA_ID = "NME_CLASS_COURSE_QUOTA_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string QUOTA = "QUOTA";
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string IS_DELETED = "IS_DELETED";

        }

        public static class NME_CLASSES_2017
        {
            public const string NME_CLASS_ID = "NME_CLASS_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class NME_COURSE_ALLOTMENT_2017
        {
            public const string ALLOTMENT_ID = "ALLOTMENT_ID";
            public const string SHIFT_ID = "SHIFT_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string IS_DELETED = "IS_DELETED";
            public const string ALLOTED_SEATS = "ALLOTED_SEATS";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }

        public static class NME_COURSE_REGISTRATION_2017
        {
            public const string REGISTRATION_ID = "REGISTRATION_ID";
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class NME_SETTINGS
        {
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string SETTINGS_NAME = "SETTINGS_NAME";
            public const string CLASS_ID = "CLASS_ID";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ALLOWED = "IS_ALLOWED";
            public const string IS_DELETED = "IS_DELETED";
            public const string SEMESTER = "SEMESTER";
            public const string P_LEVEL = "P_LEVEL";
            public const string TIME_FROM = "TIME_TO";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_CLASS_WISE_QUOTA = "IS_CLASS_WISE_QUOTA";

        }

        public static class NME_STUDENT_REGISTRATION_2017
        {
            public const string REGISTRATION_ID = "REGISTRATION_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string TIME_REGISTERED = "TIME_REGISTERED";
            public const string SEMESTER = "SEMESTER";
            public const string IS_DELETED = "IS_DELETED";
            public const string S_CLASS_ID = "S_CLASS_ID";
        }



        public static class COLLEGE_INFO_FOR_REPORT
        {
            public const string COLLEGE_ID = "COLLEGE_ID";
            public const string MANAGEMENT_NAME = "MANAGEMENT_NAME";
            public const string COLLEGE_NAME = "COLLEGE_NAME";
            public const string PLACE = "PLACE";
            public const string EMAIL = "EMAIL";
            public const string COLLEGE_CODE = "COLLEGE_CODE";
            public const string COLLEGE_LOGO = "COLLEGE_LOGO";

        }

        public static class USER_INFO
        {
            public const string ADMISSION_NO = "ADMISSION_NO";
            public const string ADMITTED_CLASS = "ADMITTED_CLASS";
            public const string ADMITTED_BATCH = "ADMITTED_BATCH";
            public const string DEPT_ID = "DEPT_ID";
            public const string SHIFT_ID = "SHIFT_ID";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string CLASS_NAME = "CLASS_NAME";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string PHOTO_PATH = "PHOTO_PATH";
            public const string CLASS_ID = "CLASS_ID";
            public const string PHOTO = "PHOTO";
            public const string CLASS_LEVEL = "CLASS_LEVEL";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";

        }
        public static class STUDENT_INFO
        {

            public const string STUDENT_ID = "STUDENT_ID";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string LAST_NAME = "LAST_NAME";
            public const string GENDER_ID = "GENDER_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string DOB = "DOB";
            public const string EMAIL = "EMAIL";
            public const string BLOOD_GROUP_ID = "BLOOD_GROUP_ID";
            public const string BIRTH_PLACE = "BIRTH_PLACE";
            public const string RELIGION_ID = "RELIGION_ID";
            public const string STUDENT_MOBILE = "STUDENT_MOBILE";
            public const string ADMISSION_DATE = "ADMISSION_DATE";
            public const string STATUS = "STATUS";
            public const string APPLICATION_NO = "APPLICATION_NO";
            public const string REGISTER_NO = "REGISTER_NO";
            public const string BATCH_ID = "BATCH_ID";
            public const string GRADUCATE_YEAR = "GRADUCATE_YEAR";
            public const string PRESENT_ADDRESS = "PRESENT_ADDRESS";
            public const string PERMANENT_ADDRESS = "PERMANENT_ADDRESS";
            public const string COUNTRY = "COUNTRY";
            public const string STATE = "STATE";
            public const string CITY = "CITY";
            public const string PINCODE = "PINCODE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string FATHER_NAME = "FATHER_NAME";
            public const string MOTHER_NAME = "MOTHER_NAME";
            public const string GUARDIAN_NAME = "GUARDIAN_NAME";
            public const string IS_GUARDIAN_AVAILABLE = "IS_GUARDIAN_AVAILABLE";
            public const string FATHER_MOBILE = "FATHER_MOBILE";
            public const string MOTHER_MOBILE = "MOTHER_MOBILE";
            public const string GUARDIAN_MOBILE = "GUARDIAN_MOBILE";
            public const string FARTHER_EMAIL = "FARTHER_EMAIL";
            public const string MOTHER_EMAIL = "MOTHER_EMAIL";
            public const string GUARDIAN_EMAIL = "GUARDIAN_EMAIL";
            public const string IS_FIRST_LEARNER = "IS_FIRST_LEARNER";
            public const string IS_PHYSICALLY_CHALLENGED = "IS_PHYSICALLY_CHALLENGED";
            public const string ANNUAL_INCOME = "ANNUAL_INCOME";
            public const string LAST_STUDIED_SCHOOL = "LAST_STUDIED_SCHOOL";
            public const string ADMITTED_COURSE = "ADMITTED_COURSE";
            public const string PERSONAL_MARK1 = "PERSONAL_MARK1";
            public const string PERSONAL_MARK2 = "PERSONAL_MARK2";
            public const string LEAVING_DATE = "LEAVING_DATE";
            public const string LEAVING_REASON = "LEAVING_REASON";
            public const string TC_ISSUED_DATE = "TC_ISSUED_DATE";
            public const string CONDUCT_INFO = "CONDUCT_INFO";
            public const string IMG_PATH = "IMG_PATH";
            public const string FINGER_PRINT = "FINGER_PRINT";
            public const string IS_DELETED = "IS_DELETED";
            public const string ADMINSSION_NO = "ADMINSSION_NO";
            public const string ROLL_NO = "ROLL_NO";
            public const string GENDER = "GENDER";
        }
        public static class STU_ADDRESS
        {
            public const string ADDRESS_ID = "ADDRESS_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string C_DOOR_DETAIL = "C_DOOR_DETAIL";
            public const string C_STREET = "C_STREET";
            public const string C_VILLAGE_AREA = "C_VILLAGE_AREA";
            public const string C_POST_PLACE_TOWN = "C_POST_PLACE_TOWN";
            public const string C_TALUK = "C_TALUK";
            public const string C_CITY = "C_CITY";
            public const string C_DISTRICT = "C_DISTRICT";
            public const string C_PINCODE = "C_PINCODE";
            public const string C_COUNTRY = "C_COUNTRY";
            public const string P_DOOR_DETAIL = "P_DOOR_DETAIL";
            public const string P_STREET = "P_STREET";
            public const string P_VILLAGE_AREA = "P_VILLAGE_AREA";
            public const string P_POST_PLACE_TOWN = "P_POST_PLACE_TOWN";
            public const string P_TALUK = "P_TALUK";
            public const string P_CITY = "P_CITY";
            public const string P_DISTRICT = "P_DISTRICT";
            public const string P_PINCODE = "P_PINCODE";
            public const string P_COUNTRY = "P_COUNTRY";
            public const string C_PHONENO = "C_PHONENO";
            public const string P_PHONENO = "P_PHONENO";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class ACC_YEAR
        {
            public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";
            public const string AC_YEAR = "AC_YEAR";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string ACTIVE_YEAR = "ACTIVE_YEAR";
            public const string IS_DELETED = "IS_DELETED";
            public const string ACADEMIC_NAME = "ACADEMIC_NAME";
        }
        public static class CP_ACADEMIC_CURRICULUM_2017
        {
            public const string CURRICULUM_ID = "CURRICULUM_ID";
            public const string PROGRAMME = "PROGRAMME";
            public const string BATCH = "BATCH";
            public const string COURSE_ID = "COURSE_ID";
            public const string S_NO = "S_NO";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_UPDATE = "IS_UPDATE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string SEMESTER = "SEMESTER";
        }
        public static class SUP_COURSE_TYPE
        {
            public const string COURSE_TYPE_ID = "COURSE_TYPE_ID";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string COURSE_TYPE_CODE = "COURSE_TYPE_CODE";

        }

        public static class USER_ROLES
        {
            public const string ADMIN = "ADMIN";
            public const string SUPERADMIN = "SUPER ADMIN";
            public const string STAFF = "STAFF";
            public const string STUDENT = "STUDENT";
            public const string PRINCIPAL = "PRINCIPAL";
            public const string PARENT = "PARENT";
        }
        public static class COLLEGE_DETAILS
        {
            public const string COLLEGE_ID = "COLLEGE_ID";
            public const string MANAGEMENT_NAME = "MANAGEMENT_NAME";
            public const string COLLEGE_NAME = "COLLEGE_NAME";
            public const string COLLEGE_TYPE = "COLLEGE_TYPE";
            public const string ADDRESS_ONE = "ADDRESS_ONE";
            public const string ADDRESS_TWO = "ADDRESS_TWO";
            public const string PLACE = "PLACE";
            public const string CITY = "CITY";
            public const string DISTRICT = "DISTRICT";
            public const string PINCODE = "PINCODE";
            public const string PHONE = "PHONE";
            public const string EMAIL = "EMAIL";
            public const string COLLEGE_CODE = "COLLEGE_CODE";
            public const string CUSTOMER_CODE = "CUSTOMER_CODE";
            public const string DB_NAME = "DB_NAME";
            public const string USERNAME = "USERNAME";
            public const string LICENSE_NO = "LICENSE_NO";
            public const string PASSWORD = "PASSWORD";
            public const string APPLICATION_URL = "APPLICATION_URL";
            public const string DB_SERVER = "DB_SERVER";
            public const string PORTAL_URL = "PORTAL_URL";
            public const string WEBSITE_URL = "WEBSITE_URL";
            public const string COLLEGE_EMAIL = "COLLEGE_EMAIL";
            public const string COLLEGE_EMAIL_PASSWORD = "COLLEGE_EMAIL_PASSWORD";

        }

        public static class FBCLASS_WISE_STAFF
        {
            public const string FB_ID = "FB_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
        }

        public static class FBCLASSES
        {
            public const string FBCLASSID = "FBCLASSID";
            public const string CLASS_ID = "CLASS_ID";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
        }
        public static class MENU_ITEMS
        {
            public const string MENU_ID = "MENU_ID";
            public const string MENU_NAME = "MENU_NAME";
            public const string PARENT_ID = "PARENT_ID";
            public const string CONTROLLER = "CONTROLLER";
            public const string ACTION = "ACTION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class USER_ACCOUNT
        {
            public const string USER_ACCOUNT_ID = "USER_ACCOUNT_ID";
            public const string USERNAME = "USERNAME";
            public const string PASSWORD = "PASSWORD";
            public const string USER_TYPE = "USER_TYPE";
            public const string USER_TYPE_ID = "USER_TYPE_ID";
            public const string NAME = "NAME";
            public const string EMAIL = "EMAIL";
            public const string MOBILE = "MOBILE";
            public const string LAST_LOGIN = "LAST_LOGIN";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string USER_ID = "USER_ID";
            public const string ROLE = "ROLE";
            public const string PHOTO = "PHOTO";
            public const string CATEGORY = "CATEGORY";
            public const string CONTROLLER_NAME = "CONTROLLER_NAME";
            public const string ACTION_NAME = "ACTION_NAME";
            public const string IS_OTP = "IS_OTP";


        }
        public static class USER_ROLES_RIGHTS
        {
            public const string USER_RIGHTS_ID = "USER_RIGHTS_ID";
            public const string MENU_ITEM_ID = "MENU_ITEM_ID";
            public const string MODIFY = "MODIFY";
            public const string ROLE_ID = "ROLE_ID";
            public const string ROLE_NAME = "ROLE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";

            // Extra 
            public const string USER_ID = "USER_ID";
            public const string USER_TYPE = "USER_TYPE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }

        public static class ACADEMIC_YEAR
        {
            public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";
            public const string ACADEMIC_NAME = "ACADEMIC_NAME";
            public const string YEAR = "YEAR";
            public const string AC_YEAR = "AC_YEAR";
            public const string START_DATE = "START_DATE";
            public const string END_DATE = "END_DATE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string ACTIVE_YEAR = "ACTIVE_YEAR";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_SUBJECT_TYPE
        {
            public const string SUBJECT_ID = "SUBJECT_ID";
            public const string SUBJECT = "SUBJECT";

            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_MONTHS
        {
            public const string MONTH_ID = "MONTH_ID";
            public const string MONTH_NAME = "MONTH_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class FBEVALUATION
        {
            public const string FBSTUDENT_ID = "FBSTUDENT_ID";
            public const string ASSESSOR = "ASSESSOR";
            public const string CLASS_ID = "CLASS_ID";
            public const string ASSESSEE = "ASSESSEE";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string QUESTION = "QUESTION";
            public const string ANSWER = "ANSWER";
            public const string ISDELETED = "ISDELETED";
        }

        public static class FBFEEDBACK_QUESTIONS
        {
            public const string QUESTION_ID = "QUESTION_ID";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string QUESTION = "QUESTION";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string FEEDBACK_NAME = "FEEDBACK_NAME";
            public const string FEEDBACK_ROLE_NAME = "FEEDBACK_ROLE_NAME";
            public const string GROUP_TITLE = "GROUP_TITLE";
            public const string FEEDBACK_ROLE_ID = "FEEDBACK_ROLE_ID";
            public const string QUESTION_TYPE_NAME = "QUESTION_TYPE_NAME";
            public const string QUESTION_GROUP = "QUESTION_GROUP";
            public const string ASSESSOR_ROLE = "ASSESSOR_ROLE";
            public const string ASSESSEE_ROLE = "ASSESSEE_ROLE";
        }
        public static class FBFEEDBACK_SETTINGS
        {
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string FEEDBACK_NAME = "FEEDBACK_NAME";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string QUESTION_TYPE_ID = "QUESTION_TYPE_ID";
            public const string NO_OF_QUESTION = "NO_OF_QUESTION";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_DELETED = "IS_DELETED";
            public const string CLASS_ID = "CLASS_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string FEEDBACK_TYPE = "FEEDBACK_TYPE";
            public const string ASSESSOR = "ASSESSOR";
            public const string ASSESSEE = "ASSESSEE";
            public const string MANUAL_STAFF_SELECTION = "MANUAL_STAFF_SELECTION";
            public const string WITH_IN_DEPARTMENT = "WITH_IN_DEPARTMENT";
            public const string ASSESSOR_CATEGORY = "ASSESSOR_CATEGORY";
            public const string ASSESSEE_CATEGORY = "ASSESSEE_CATEGORY";
        }

        public static class FBOBJECTIVES
        {
            public const string OBJECTIVE_ID = "OBJECTIVE_ID";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
            public const string OBJECTIVES = "OBJECTIVES";
            public const string IS_DELETED = "IS_DELETED";
            public const string RATING = "RATING";
            public const string OBJECTIVESHORTTERM = "OBJECTIVESHORTTERM";
            public const string OBJECTIVE_ORDER = "OBJECTIVE_ORDER";
        }

        public static class FBQUESTIONS_GROUP
        {
            public const string QUESTION_GROUP_ID = "QUESTION_GROUP_ID";
            public const string GROUP_TITLE = "GROUP_TITLE";
            public const string IS_DELETED = "IS_DELETED";

        }

        public static class FBSTUDENT_ENTRY
        {
            public const string STUDENT_ENTRY_ID = "STUDENT_ENTRY_ID";
            public const string ENTRY_STATUS = "ENTRY_STATUS";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string IS_DELETED = "IS_DELETED";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string AC_YEAR = "AC_YEAR";
        }

        public static class FBRATING_BY_STAFF
        {
            public const string OBJECTIVES_COUNT = "OBJECTIVES_COUNT";
            public const string STUDENT_COUNT = "STUDENT_COUNT";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string CLASS_CODE = "CLASS_CODE";
            public const string OBJECTIVES = "OBJECTIVES";
            public const string QUESTION = "QUESTION";
            public const string ASSESSOR = "ASSESSOR";

        }

        public static class MODULES
        {
            public const string MODULES_ID = "MODULES_ID";
            public const string MODULE_NAME = "MODULE_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string STYLE = "STYLE";
            public const string HAS_SUB = "HAS_SUB";
        }
        public static class SQLTables
        {
            public const string tblTest = "";
        }
        public static class CIA_GROUP_COMPONENT
        {
            public const string CIA_GROUP_COMPONENT_ID = "CIA_GROUP_COMPONENT_ID";
            public const string CIA_GROUP_ID = "CIA_GROUP_ID";
            public const string SUP_CIA_COMPONENT_ID = "SUP_CIA_COMPONENT_ID";
            public const string MAX_MARK = "MAX_MARK";
            public const string MIN_MARK = "MIN_MARK";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CIA_MARK_UPLOAD_STATUS
        {
            public const string UPLOAD_ID = "UPLOAD_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string FILE_NAME = "FILE_NAME";
            public const string SHEET_NAME = "SHEET_NAME";
            public const string STATUS = "STATUS";
            public const string STATUS_MESSAGE = "STATUS_MESSAGE";
        }
        public static class CIA_MARKS_2017
        {
            public const string MARK_ID = "MARK_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string COURSE_ROOT_ID = "COURSE_ROOT_ID";
            public const string CIA_C1 = "CIA_C1";
            public const string CIA_C2 = "CIA_C2";
            public const string CIA_C3 = "CIA_C3";
            public const string CIA_C4 = "CIA_C4";
            public const string CIA_C5 = "CIA_C5";
            public const string CIA_C6 = "CIA_C6";
            public const string CIA_C7 = "CIA_C7";
            public const string CIA_C8 = "CIA_C8";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string CIA_TOTAL = "CIA_TOTAL";
        }
        public static class CP_CLASS_COURSE_2017
        {
            public const string CLASS_COURSE_ID = "CLASS_COURSE_ID";
            public const string COURSE_TITLE = "COURSE_TITLE";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string IS_DELETED = "IS_DELETED";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class CP_CLASS_COURSE_STAFF_2017
        {
            public const string CLSCRSSTF_ID = "CLSCRSSTF_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string STF_ALLOTMENT_TYPE = "STF_ALLOTMENT_TYPE";
            public const string HRS_WEEK = "HRS_WEEK";
            public const string HRS_TERM = "HRS_TERM";
            public const string SHIFT = "SHIFT";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_PRIMARY_STAFF = "IS_PRIMARY_STAFF";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string STAFF_NAME = "STAFF_NAME";
            public const string STAFF_COUNT = "STAFF_COUNT";
            public const string SEMESTER_TYPE_ID = "SEMESTER_TYPE_ID";

        }
        public static class CP_CLASSES_2017
        {
            public const string CLASS_ID = "CLASS_ID";
            public const string CLASS_CODE = "CLASS_CODE";
            public const string CLASS_NAME = "CLASS_NAME";
            public const string CLASS_DESCRIPTION = "CLASS_DESCRIPTION";
            public const string CLASS_TYPE = "CLASS_TYPE";
            public const string CLASS_LEVEL = "CLASS_LEVEL";
            public const string CLASS_YEAR = "CLASS_YEAR";
            public const string PROGRAMME = "PROGRAMME";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string LANGUAGE = "LANGUAGE";
            public const string IS_CHOICE = "IS_CHOICE";
            public const string CLASS_ORDER = "CLASS_ORDER";
            public const string CLS_PROMOTION_GROUP = "CLS_PROMOTION_GROUP";
            public const string CLS_PROMOTION_LEVEL = "CLS_PROMOTION_LEVEL";
            public const string SHIFT = "SHIFT";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_UPDATE = "IS_UPDATE";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string COURSE_ID = "COURSE_ID";
            public const string COURSE_NAME = "COURSE_NAME";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string PROGRAMME_MODE = "PROGRAMME_MODE";
            public const string SELECTED_FIELD = "SELECTED_FIELD";
            public const string NOTES_ID = "NOTES_ID";
            public const string QUESTION_PAPER_ID = "QUESTION_PAPER_ID";
            public const string PAPER_TITLE = "PAPER_TITLE";
            public const string NOTES_TITLE = "NOTES_TITLE";
            public const string TYPE = "TYPE";
            public const string QUESTION_PAPER_TYPE_ID = "QUESTION_PAPER_TYPE_ID";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string COURSE_ROOT_ID = "COURSE_ROOT_ID";
        }

        public static class SUP_APPTYPE_PROG_GROUPS
        {
            public const string PRO_GROUPS_ID = "PRO_GROUPS_ID";
            public const string PROGRAMME_ID = "PROGRAMME_ID";
            public const string APPTYPE_ID = "APPTYPE_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_NEW = "IS_NEW";
            public const string PROGRAMME_NAME = "PROGRAMME_NAME";
            public const string PROGRAMME_MODE = "PROGRAMME_MODE";
            public const string PROGRAMME = "PROGRAMME";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
            public const string SHIFT = "SHIFT";
        }
        public static class SUP_APPLICATION_TYPE
        {
            public const string APPLICATION_TYPE_ID = "APPLICATION_TYPE_ID";
            public const string APPLICATION_TYPE = "APPLICATION_TYPE";
            public const string PREFIX = "PREFIX";
            public const string SUFFIX = "SUFFIX";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_EVENT_MODE
        {
            public const string MODE_ID = "MODE_ID";
            public const string MODE_NAME = "MODE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_CERTIFICATE_TYPE
        {
            public const string CERTIFICATE_ID = "CERTIFICATE_ID";
            public const string CERTIFICATE_NAME = "CERTIFICATE_NAME";
           
        }
        public static class CP_COURSE_ROOT_2017
        {
            public const string COURSE_ROOT_ID = "COURSE_ROOT_ID";
            public const string YEAR_FROM = "YEAR_FROM";
            public const string YEAR_TO = "YEAR_TO";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string YEAR = "YEAR";
            public const string SEMESTER_FROM = "SEMESTER_FROM";
            public const string SEMESTER_TO = "SEMESTER_TO";
            public const string PART = "PART";
            public const string PAPER = "PAPER";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string HRS_PER_WEEK = "HRS_PER_WEEK";
            public const string CREDITS = "CREDITS";
            public const string OPTION_NAME = "OPTION_NAME";
            public const string PAPER_CODE = "PAPER_CODE";
            public const string COURSE_TITLE = "COURSE_TITLE";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_NME_SUBJECT = "IS_NME_SUBJECT";
            public const string IS_ALLIED_SUBJECT = "IS_ALLIED_SUBJECT";
            public const string COURSE_ORDER = "COURSE_ORDER";
            public const string IS_DELETED = "IS_DELETED";
            public const string SUBJECTS = "SUBJECTS";
            public const string IS_COMPULSORY = "IS_COMPULSORY";
            public const string UGC_COURSE_TYPE = "UGC_COURSE_TYPE";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string PAPER_TYPE = "PAPER_TYPE";
            public const string SUBJECT_TYPE = "SUBJECT_TYPE";
            public const string CONS_COURSE_TITLE = "CONS_COURSE_TITLE";
            public const string INTERNAL = "INTERNAL";
            public const string EXTERNAL = "EXTERNAL";
            public const string TOTAL = "TOTAL";
            public const string COURSE_ID = "COURSE_ID";

        }
        public static class CP_COURSE_TYPE_2017
        {
            public const string COURSE_TYPE_ID = "COURSE_TYPE_ID";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string NO_OF_COMPONENTS = "NO_OF_COMPONENTS";
            public const string COURSE_TYPE_ORDER = "COURSE_TYPE_ORDER";
            public const string IS_STU_BASED_SELECTION = "IS_STU_BASED_SELECTION";
            public const string IS_DELETED = "IS_DELETED";
            public const string INTERNAL = "INTERNAL";
            public const string EXTERNAL = "EXTERNAL";
            public const string TOTAL = "TOTAL";
            public const string HOURS = "HOURS";
            public const string PART_ID = "PART_ID";
            public const string PROGRAMME_LEVEL = "PROGRAMME_LEVEL";
            public const string CREDITS = "CREDITS";
            public const string IS_STUDENT_VIEW_CIA_MARK = "IS_STUDENT_VIEW_CIA_MARK";
        }
        public static class CP_COURSE_TYPE_CATEGORY_2017
        {
            public const string TYPE_CATEGORY_ID = "TYPE_CATEGORY_ID";
            public const string COURSE_TYPE_TITLE = "COURSE_TYPE_TITLE";
            public const string PROGRAMME_LEVEL_ID = "PROGRAMME_LEVEL_ID";
            public const string COURSE_TYPE_ID = "COURSE_TYPE_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_STU_BASED_SELECTION = "IS_STU_BASED_SELECTION";
            public const string NO_OF_COMPONENTS = "NO_OF_COMPONENTS";
            public const string COURSE_TYPE_ORDER = "COURSE_TYPE_ORDER";
        }
        public static class CP_COURSE_TYPE_GROUP_2017
        {
            public const string GROUP_ID = "GROUP_ID";
            public const string COURSE_TYPE_ID = "COURSE_TYPE_ID";
            public const string CA_GROUP = "CA_GROUP";
            public const string TYPE_ID = "TYPE_ID";
            public const string GROUP_MARK = "GROUP_MARK";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CP_COURSE_TYPE_MARKS_2017
        {
            public const string COM_MARK_ID = "COM_MARK_ID";
            public const string COURSE_TYPE_ID = "COURSE_TYPE_ID";
            public const string MAX_C1 = "MAX_C1";
            public const string MAX_C2 = "MAX_C2";
            public const string MAX_C3 = "MAX_C3";
            public const string MAX_C4 = "MAX_C4";
            public const string MAX_C5 = "MAX_C5";
            public const string MAX_C6 = "MAX_C6";
            public const string MAX_C7 = "MAX_C7";
            public const string MAX_C8 = "MAX_C8";
            public const string MIN_C1 = "MIN_C1";
            public const string MIN_C2 = "MIN_C2";
            public const string MIN_C3 = "MIN_C3";
            public const string MIN_C4 = "MIN_C4";
            public const string MIN_C5 = "MIN_C5";
            public const string MIN_C6 = "MIN_C6";
            public const string MIN_C7 = "MIN_C7";
            public const string MIN_C8 = "MIN_C8";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CP_DEGREE_2017
        {
            public const string DEGREE_ID = "DEGREE_ID";
            public const string DEGREE = "DEGREE";
            public const string PROGRAMME_TYPE = "PROGRAMME_TYPE";
            public const string PROGRAMME_LEVEL = "PROGRAMME_LEVEL";
            public const string IS_DELETED = "IS_DELETED";
            public const string DEGREE_ORDER = "DEGREE_ORDER";
            public const string FACULTY = "FACULTY";
            public const string BOS_NAME = "BOS_NAME";
            public const string PREFIX = "PREFIX";
            public const string BOS_SERIES_ROLLNO = "BOS_SERIES_ROLLNO";
            public const string BOS_SERIES_REGNO = "BOS_SERIES_REGNO";
            public const string BOS_SERIES_ADMNO = "BOS_SERIES_ADMNO";
        }
        public static class CP_DEPARTMENT_2017
        {
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string DEPARTMENT_CODE = "DEPARTMENT_CODE";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string IS_DELETED = "IS_DELETED";
            public const string DEPARTMENT_ORDER = "DEPARTMENT_ORDER";
            public const string FACULTY = "FACULTY";
            public const string YEAR_OF_PUBLISHMENT = "YEAR_OF_PUBLISHMENT";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class CP_FACULTY_2017
        {
            public const string FACULTY_ID = "FACULTY_ID";
            public const string FACULTY_CODE = "FACULTY_CODE";
            public const string FACULTY = "FACULTY";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string FACULTY_ORDER = "FACULTY_ORDER";
            public const string FAC_SERIES = "FAC_SERIES";

            public static string STAFF_ID = "STAFF_ID";
            public static string FIRST_NAME = "FIRST_NAME";
        }
        public static class CP_PROGRAMME_2017
        {
            public const string PROGRAMME_ID = "PROGRAMME_ID";
            public const string PROGRAMME_CODE = "PROGRAMME_CODE";
            public const string PROGRAMME_NAME = "PROGRAMME_NAME";
            public const string PROGRAMME_DESCRIPTION = "PROGRAMME_DESCRIPTION";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string DEGREE = "DEGREE";
            public const string PROGRAMME_ORDER = "PROGRAMME_ORDER";
            public const string DURATION = "DURATION";
            public const string DURATION_UNIT = "DURATION_UNIT";
            public const string NO_OF_SEMESTER = "NO_OF_SEMESTER";
            public const string CHANNEL = "CHANNEL";
            public const string IS_AIDED = "IS_AIDED";
            public const string IS_DELETED = "IS_DELETED";
            public const string PRO_SERIES_ROLLNO = "PRO_SERIES_ROLLNO";
            public const string NON_AIDED = "NON_AIDED";
            public const string IS_REGULAR = "IS_REGULAR";
            public const string SUBJECTS = "SUBJECTS";
            public const string PRO_SERIES_REGNO = "PRO_SERIES_REGNO";
            public const string PRO_SERIES_ADMNO = "PRO_SERIES_ADMNO";
            public const string MEDIUM_OF_INSTRACTION = "MEDIUM_OF_INSTRACTION";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
            public const string PROGRAMME = "PROGRAMME";
        }
        public static class CP_SEMESTER_ROOT_2017
        {
            public const string ACC_SEMESTER_ID = "ACC_SEMESTER_ID";
            public const string BATCH = "BATCH";
            public const string PROGRAMME = "PROGRAMME";
            public const string SEMESTER = "SEMESTER";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SEMESTER_MARKS_2017
        {
            public const string SEMESTER_MARK_ID = "SEMESTER_MARK_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string INTERNAL_MARK = "INTERNAL_MARK";
            public const string EXTERNAL_MARK = "EXTERNAL_MARK";
            public const string TOTAL = "TOTAL";
            public const string RESULT = "RESULT";
            public const string APPLIED = "APPLIED";
            public const string SEMESTER = "SEMESTER";
            public const string M_PASS = "M_PASS";
            public const string IS_ARREAR = "IS_ARREAR";
            public const string GRADE = "GRADE";
            public const string GRADE_POINT = "GRADE_POINT";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string EXAM_SETTING_ID = "EXAM_SETTING_ID";
        }
        public static class ACADEMIC_BATCHES
        {
            public const string AC_BATCH_ID = "AC_BATCH_ID";
            public const string BATCH_ID = "BATCH_ID";
            public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";
            public const string BATCH_YEAR = "BATCH_YEAR";
            public const string BATCH = "BATCH";
        }
        public static class FBCLASS_WISE_STAFF2017
        {
            public const string FB_ID = "FB_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string COURSE_ID = "COURSE_ID";
        }
        public static class FBEVALUATION2017
        {
            public const string FBSTUDENT_ID = "FBSTUDENT_ID";
            public const string ASSESSOR = "ASSESSOR";
            public const string CLASS_ID = "CLASS_ID";
            public const string ASSESSEE = "ASSESSEE";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string QUESTION = "QUESTION";
            public const string ANSWER = "ANSWER";
            public const string COURSE_ID = "COURSE_ID";
            public const string ISDELETED = "ISDELETED";
        }
        public static class FBFEEDBACK_QUSTIONS
        {
            public const string QUESTION_ID = "QUESTION_ID";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string QUESTION = "QUESTION";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string QUESTION_GROUP = "QUESTION_GROUP";
        }
        public static class STF_PERSONAL_INFO
        {
            public const string STAFF_ID = "STAFF_ID";
            public const string USER_CODE = "USER_CODE";
            public const string STAFF_CODE = "STAFF_CODE";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string LAST_NAME = "LAST_NAME";
            public const string GENDER = "GENDER";
            public const string KNOWN_AS = "KNOWN_AS";
            public const string PLACE_OF_BIRTH = "PLACE_OF_BIRTH";
            public const string IDENTIFICATION_MARK1 = "IDENTIFICATION_MARK1";
            public const string IDENTIFICATION_MARK2 = "IDENTIFICATION_MARK2";
            public const string HEIGHT = "HEIGHT";
            public const string COMMUNITY = "COMMUNITY";
            public const string NATIONALITY = "NATIONALITY";
            public const string MARITAL_STATUS = "MARITAL_STATUS";
            public const string CATEGORY = "CATEGORY";
            public const string MAIN_SUBJECT = "MAIN_SUBJECT";
            public const string ANNUATION_DATE = "ANNUATION_DATE";
            public const string LEAVING_DATE = "LEAVING_DATE";
            public const string LEAVING_REMARKS = "LEAVING_REMARKS";
            public const string USER_FIELD1 = "USER_FIELD1";
            public const string USER_FIELD2 = "USER_FIELD2";
            public const string SESSIONS = "SESSIONS";
            public const string DATE_OF_JOIN = "DATE_OF_JOIN";
            public const string DATE_OF_BIRTH = "DATE_OF_BIRTH";
            public const string BLOOD_GROUP = "BLOOD_GROUP";
            public const string RELIGION = "RELIGION";
            public const string MOTHER_TONGUE = "MOTHER_TONGUE";
            public const string PHYSICAL_STATUS = "PHYSICAL_STATUS";
            public const string DESIGNATION = "DESIGNATION";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string RETIREMENT_DATE = "RETIREMENT_DATE";
            public const string LEAVING_REASON = "LEAVING_REASON";
            public const string PHOTO = "PHOTO";
            public const string SERVICE_REGNO = "SERVICE_REGNO";
            public const string IS_DELETED = "IS_DELETED";
            public const string KNOWLEDGE_IN_COMPUTER = "KNOWLEDGE_IN_COMPUTER";
            public const string CERTIFICATE1 = "CERTIFICATE1";
            public const string CERTIFICATE2 = "CERTIFICATE2";
            public const string CERTIFICATE3 = "CERTIFICATE3";
            public const string QUALIFICATION_DOC1 = "QUALIFICATION_DOC1";
            public const string QUALIFICATION_DOC2 = "QUALIFICATION_DOC2";
            public const string QUALIFICATION_DOC3 = "QUALIFICATION_DOC3";
            public const string QUALIFICATION = "QUALIFICATION";
            public const string NON_TEACHING_CATEGORY = "NON_TEACHING_CATEGORY";
            public const string VISA_NUMBER = "VISA_NUMBER";
            public const string STAFF_REGISTERED_DATE = "STAFF_REGISTERED_DATE";
            public const string STAFF_ORDER = "STAFF_ORDER";
            public const string ROLES = "ROLES";
            public const string PROLE_ID = "PROLE_ID";
            public const string PREFIX = "PREFIX";
            public const string SHIFT = "SHIFT";
            public const string ATT_SHIFT = "ATT_SHIFT";
            public const string SUB_CATEGORY = "SUB_CATEGORY";
            public const string BOARD_MEMBER = "BOARD_MEMBER";
            public const string IS_LEFT = "IS_LEFT";
            public const string STAFF_NAME = "STAFF_NAME";
            public const string EMPLOYEE_ID = "EMPLOYEE_ID";
            public const string EMAIL = "EMAIL";
            public const string MOBILE_NO = "MOBILE_NO";
            public const string AADTHAR_NO = "AADTHAR_NO";
            public const string CASTE = "CASTE";
            public const string DATE_OF_ENTRY_TEACHING = "DATE_OF_ENTRY_TEACHING";
            public const string PAN_NO = "PAN_NO";
            public const string STAFF_APPROVAL_NO = "STAFF_APPROVAL_NO";
            public const string MPHIL_APPROVAL_NO = "MPHIL_APPROVAL_NO";
            public const string PHD_APPROVAL_NO = "PHD_APPROVAL_NO";
            public const string FATHER_NAME = "FATHER_NAME";
            public const string MOTHER_NAME = "MOTHER_NAME";
            public const string SPOUSE_NAME = "SPOUSE_NAME";
            public const string ADDRESS_PRESENT = "ADDRESS_PRESENT";
            public const string ADDRESS_PERMANENT = "ADDRESS_PERMANENT";
            public const string EPF_NO = "EPF_NO";
            public const string UAN_NO = "UAN_NO";
            public const string PHD_COMPLETION = "PHD_COMPLETION";
            public const string MPHIL_COMPLETION = "MPHIL_COMPLETION";
            public const string SET_COMPLETION = "SET_COMPLETION";
            public const string NET_COMPLETION = "NET_COMPLETION";
            public const string PG_COMPLETION = "PG_COMPLETION";
            public const string UG_COMPLETION = "UG_COMPLETION";
            public const string HSC_COMPLETION = "HSC_COMPLETION";
            public const string SSLC_COMPLETION = "SSLC_COMPLETION";
            public const string PG_EXPERIENCE = "PG_EXPERIENCE";
            public const string UG_EXPERIENCE = "UG_EXPERIENCE";
            public const string MPHIL_EXPERIENCE = "MPHIL_EXPERIENCE";
            public const string PHD_EXPERIENCE = "PHD_EXPERIENCE";
            public const string BANK_AC_NO = "BANK_AC_NO";
            public const string BANK_NAME = "BANK_NAME";
            public const string BRANCH_NAME = "BRANCH_NAME";
            public const string IFSC_CODE = "IFSC_CODE";
            public const string SALARY_BASIC = "SALARY_BASIC";
            public const string SALARY_DA = "SALARY_DA";
            public const string PACKAGE = "PACKAGE";
            public const string SPECIAL_ALLOWANCE = "SPECIAL_ALLOWANCE";
            public const string OTHERS_SALARY = "OTHERS_SALARY";
            public const string PP_SALARY = "PP_SALARY";
            public const string GROSS_SALARY = "GROSS_SALARY";
            public const string GSUITE_MAIL_ID = "GSUITE_MAIL_ID";
            public const string DATE_OF_EPF = "DATE_OF_EPF";
            public const string STAFF_STATUS = "STAFF_STATUS";

        }

        public static class STU_PARENT_INFO
        {
            public const string PARENT_ID = "PARENT_ID";
            public const string FATHER_NAME = "FATHER_NAME";
            public const string MOTHER_NAME = "MOTHER_NAME";
            public const string FATHER_NUMBER = "FATHER_NUMBER";
            public const string MOTHRE_NUMBER = "MOTHRE_NUMBER";
            public const string MOBILE_FOR_SMS = "MOBILE_FOR_SMS";
            public const string ROLE_ID = "ROLE_ID";
            public const string USER_TYPE = "USER_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_LEFT = "IS_LEFT";
        }
        public static class STF_ADDRESS
        {
            public const string ADDRESS_NO = "ADDRESS_NO";
            public const string STAFFNO = "STAFFNO";
            public const string CDOOR_DETAILS = "CDOOR_DETAILS";
            public const string CSTREET = "CSTREET";
            public const string CPLACE = "CPLACE";
            public const string CCITY = "CCITY";
            public const string CPIN_CODE = "CPIN_CODE";
            public const string CDISTRICT = "CDISTRICT";
            public const string CVILLAGE = "CVILLAGE";
            public const string CCOUNTRY = "CCOUNTRY";
            public const string CPHONE_NO = "CPHONE_NO";
            public const string CCELL_NO = "CCELL_NO";
            public const string CEMAIL = "CEMAIL";
            public const string PSTREET = "PSTREET";
            public const string PVILLAGE = "PVILLAGE";
            public const string PPLACE = "PPLACE";
            public const string PCITY = "PCITY";
            public const string PPIN_CODE = "PPIN_CODE";
            public const string PDISTRICT = "PDISTRICT";
            public const string PCOUNTRY = "PCOUNTRY";
            public const string PPHONE_NO = "PPHONE_NO";
            public const string PCELL_NO = "PCELL_NO";
            public const string ISDELETED = "ISDELETED";
            public const string PDOOR_DETAILS = "PDOOR_DETAILS";
            public const string PEMAIL = "PEMAIL";
        }
        public static class STF_SERVICES
        {
            public const string SERVICE_NO = "SERVICE_NO";
            public const string STAFF_NO = "STAFF_NO";
            public const string DATE_OF_APPOINT = "DATE_OF_APPOINT";
            public const string APPOINTMENT_NAME = "APPOINTMENT_NAME";
            public const string APPOINTMENT_NATURE = "APPOINTMENT_NATURE";
            public const string DATE_OF_TERMINATION = "DATE_OF_TERMINATION";
            public const string INSTITUTE = "INSTITUTE";
            public const string REMARKS = "REMARKS";
            public const string PLACE = "PLACE";
            public const string ISDELETED = "ISDELETED";
        }
        public static class STF_COUNSELING
        {
            public const string STF_COUN_ID = "STF_COUN_ID";
            public const string DATE_OF_COUN = "DATE_OF_COUN";
            public const string DURATION = "DURATION";
            public const string STAFF = "STAFF";
            public const string REASON = "REASON";
            public const string GIVEN = "GIVEN";
            public const string ACTION_TAKEN = "ACTION_TAKEN";
            public const string REMARK = "REMARK";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class STF_PAPER
        {
            public const string PAPER_ID = "PAPER_ID";
            public const string STAFF_NO = "STAFF_NO";
            public const string PAPER_NAME = "PAPER_NAME";
            public const string LEVEL = "LEVEL";
            public const string PUBLISHING_CATEGORY = "PUBLISHING_CATEGORY";
            public const string JOURNAL_NAME = "JOURNAL_NAME";
            public const string NO_OF_PAGES = "NO_OF_PAGES";
            public const string PAGES_FROM = "PAGES_FROM";
            public const string PAGES_TO = "PAGES_TO";
            public const string VOLUME = "VOLUME";
            public const string YEAR_PUBLISHED = "YEAR_PUBLISHED";
            public const string IS_DELETED = "IS_DELETED";

        }
        public static class STF_PUBLISH
        {
            public const string PUBLISH_ID = "PUBLISH_ID";
            public const string STAFF_NO = "STAFF_NO";
            public const string BOOK_NAME = "BOOK_NAME";
            public const string LEVEL = "LEVEL";
            public const string PUBLISHING_CATEGORY = "PUBLISHING_CATEGORY";
            public const string JOURNAL_NAME = "JOURNAL_NAME";
            public const string VOLUME = "VOLUME";
            public const string PUBLISHER = "PUBLISHER";
            public const string PYEAR = "PYEAR";
            public const string EDITION = "EDITION";
            public const string ISDELETED = "ISDELETED";
        }
        public static class STF_QUALIFICATION
        {
            public const string QUALIFICATION_NO = "QUALIFICATION_NO";
            public const string STAFF_NO = "STAFF_NO";
            public const string DEGREE = "DEGREE";
            public const string DEGREE_TYPE = "DEGREE_TYPE";
            public const string MAIN_SUBJECT = "MAIN_SUBJECT";
            public const string ALLIED_SUBJECT = "ALLIED_SUBJECT";
            public const string COMPLETION_MONTH = "COMPLETION_MONTH";
            public const string INSTITUTE_OF_STUDY = "INSTITUTE_OF_STUDY";
            public const string UNIVERSITY = "UNIVERSITY";
            public const string PERCENTAGE = "PERCENTAGE";
            public const string ISDELETED = "ISDELETED";
            public const string COMPLETION_YEAR = "COMPLETION_YEAR";
        }
        public static class STF_TRAINING
        {
            public const string TRAINING_NO = "TRAINING_NO";
            public const string STAFF_NO = "STAFF_NO";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string PROGRAMME = "PROGRAMME";
            public const string PLACE = "PLACE";
            public const string TLEVEL = "TLEVEL";
            public const string COMMENTS = "COMMENTS";
            public const string ISDELETED = "ISDELETED";
        }
        public static class STF_FAMILY
        {
            public const string FAMILY_NO = "FAMILY_NO";
            public const string STAFF_NO = "STAFF_NO";
            public const string NAME = "NAME";
            public const string RELATION = "RELATION";
            public const string DATE_OF_BIRTH = "DATE_OF_BIRTH";
            public const string ISDELETED = "ISDELETED";
        }
        public static class STU_CLASS
        {
            public const string STU_CLASS_ID = "STU_CLASS_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string CLASS_ID = "CLASS_ID";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class STU_COURSE_INFO
        {
            public const string COURSE_INFO_ID = "COURSE_INFO_ID";
            public const string PAPER_CODE_ID = "PAPER_CODE_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string IS_ELECTIVE = "IS_ELECTIVE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string S_CLASS_ID = "S_CLASS_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string TEMP_ID = "TEMP_ID";
            public const string SEMESTER = "SEMESTER";
        }
        public static class STU_PERSONAL_INFO
        {
            public const string STUDENT_ID = "STUDENT_ID";
            public const string ADMISSION_NO = "ADMISSION_NO";
            public const string ADMISSION_DATE = "ADMISSION_DATE";
            public const string ADMITTED_CLASS = "ADMITTED_CLASS";
            public const string DEPT_ID = "DEPT_ID";
            public const string PROGRAM_ID = "PROGRAM_ID";
            public const string GENDER = "GENDER";
            public const string SHIFT_ID = "SHIFT_ID";
            public const string DATE_OF_BIRTH = "DATE_OF_BIRTH";
            public const string MOTHER_TONGUE = "MOTHER_TONGUE";
            public const string SECOND_LANGUAGE = "SECOND_LANGUAGE";
            public const string BLOOD_GROUP = "BLOOD_GROUP";
            public const string RELIGION = "RELIGION";
            public const string PLACE_OF_BIRTH = "PLACE_OF_BIRTH";
            public const string NATIONALITY = "NATIONALITY";
            public const string MODE_OF_TRANSPORT = "MODE_OF_TRANSPORT";
            public const string LEAVIN_GDATE = "LEAVIN_GDATE";
            public const string LEAVING_YEAR = "LEAVING_YEAR";
            public const string LEAVING_CLASS = "LEAVING_CLASS";
            public const string REASON_FOR_LEAVING = "REASON_FOR_LEAVING";
            public const string FATHER_NAME = "FATHER_NAME";
            public const string FR_OCCUPATION = "FR_OCCUPATION";
            public const string FATHER_EDUCATION = "FATHER_EDUCATION";
            public const string MOTHER_NAME = "MOTHER_NAME";
            public const string MO_OCCUPATION = "MO_OCCUPATION";
            public const string MOTHER_EDUCATION = "MOTHER_EDUCATION";
            public const string ANNUAL_INCOME = "ANNUAL_INCOME";
            public const string IS_DELETED = "IS_DELETED";
            public const string RESIDENCE_NO = "RESIDENCE_NO";
            public const string PASSPORT_Number = "PASSPORT_Number";
            public const string DATE_OF_EXPIRY = "DATE_OF_EXPIRY";
            public const string COUNTRY_OF_ISSUE = "COUNTRY_OF_ISSUE";
            public const string PHOTO_PATH = "PHOTO_PATH";
            public const string FR_NATIONALITY = "FR_NATIONALITY";
            public const string FR_EMPLOYER = "FR_EMPLOYER";
            public const string FR_BUSINESS_PHONE = "FR_BUSINESS_PHONE";
            public const string FR_MOBILE = "FR_MOBILE";
            public const string FR_PASSPORT_number = "FR_PASSPORT_number";
            public const string FR_COUNTR_YOF_ISSUE = "FR_COUNTR_YOF_ISSUE";
            public const string MO_NATIONALITY = "MO_NATIONALITY";
            public const string MO_EMPLOYER = "MO_EMPLOYER";
            public const string MO_BUSINESS_PHONE = "MO_BUSINESS_PHONE";
            public const string MO_MOBILE = "MO_MOBILE";
            public const string MO_PASSPORT_number = "MO_PASSPORT_number";
            public const string MO_COUNTRY_OF_ISSUE = "MO_COUNTRY_OF_ISSUE";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string LAST_NAME = "LAST_NAME";
            public const string STU_PASSPORT_IMAGE1 = "STU_PASSPORT_IMAGE1";
            public const string SPOKEN_LANGUAGE = "SPOKEN_LANGUAGE";
            public const string STU_PASSPORT_IMAGE2 = "STU_PASSPORT_IMAGE2";
            public const string STU_PASSPORT_IMAGE3 = "STU_PASSPORT_IMAGE3";
            public const string FATHER_PHOTO = "FATHER_PHOTO";
            public const string MOTHER_PHOTO = "MOTHER_PHOTO";
            public const string UPLOAD_FLAG = "UPLOAD_FLAG";
            public const string FIRST_LANGUAGE = "FIRST_LANGUAGE";
            public const string ISSUED_DATE = "ISSUED_DATE";
            public const string VISA_ISSUED_DATE = "VISA_ISSUED_DATE";
            public const string VISA_EXPIRY_DATE = "VISA_EXPIRY_DATE";
            public const string REG_STATUS = "REG_STATUS";
            public const string CER_ISSUED_DATE = "CER_ISSUED_DATE";
            public const string COUNTRY_OF_BIRTH = "COUNTRY_OF_BIRTH";
            public const string MINISTRY_CHECK = "MINISTRY_CHECK";
            public const string VISA_SPONSOR = "VISA_SPONSOR";
            public const string VISA_Number = "VISA_Number";
            public const string SIBLING = "SIBLING";
            public const string FR_BUSINESS_PO_BOX = "FR_BUSINESS_PO_BOX";
            public const string MO_BUSINESS_PO_BOX = "MO_BUSINESS_PO_BOX";
            public const string APPROVED_DATE = "APPROVED_DATE";
            public const string COMMUNITY = "COMMUNITY";
            public const string CASTE = "CASTE";
            public const string F_DATE_OF_BIRTH = "F_DATE_OF_BIRTH";
            public const string M_DATE_OF_BIRTH = "M_DATE_OF_BIRTH";
            public const string GUARDIAN_NAME = "GUARDIAN_NAME";
            public const string G_CONTACT_NO = "G_CONTACT_NO";
            public const string IS_DOWNLOADED = "IS_DOWNLOADED";
            public const string DOWNLOAD_TIME = "DOWNLOAD_TIME";
            public const string SMS_TYPE = "SMS_TYPE";
            public const string STUDENT_REGISTERDED_DATE = "STUDENT_REGISTERDED_DATE";
            public const string SIBLING_ID = "SIBLING_ID";
            public const string USER_TYPE = "USER_TYPE";
            public const string FR_INCOME = "FR_INCOME";
            public const string MO_INCOME = "MO_INCOME";
            public const string SPECIALLY_ABLED = "SPECIALLY_ABLED";
            public const string DISADVANTAGED_GROUP = "DISADVANTAGED_GROUP";
            public const string CERTIFICATE1 = "CERTIFICATE1";
            public const string CERTIFICATE2 = "CERTIFICATE2";
            public const string CERTIFICATE3 = "CERTIFICATE3";
            public const string IS_UPDATED = "IS_UPDATED";
            public const string ANNIVERSARY_DATE = "ANNIVERSARY_DATE";
            public const string REGISTER_NO = "REGISTER_NO";
            public const string ROLL_NO = "ROLL_NO";
            public const string CLASS_ID = "CLASS_ID";
            public const string MO_DATE_OF_EXPIRY = "MO_DATE_OF_EXPIRY";
            public const string FR_DATE_OF_EXPIRY = "FR_DATE_OF_EXPIRY";
            public const string IS_FRETA_STAFF = "IS_FRETA_STAFF";
            public const string IS_MOETA_STAFF = "IS_MOETA_STAFF";
            public const string FR_EMAIL = "FR_EMAIL";
            public const string MO_EMAIL = "MO_EMAIL";
            public const string UNIVERSITY_ROLLNO = "UNIVERSITY_ROLLNO";
            public const string STU_MOBILENO = "STU_MOBILENO";
            public const string STU_EMAILID = "STU_EMAILID";
            public const string BATCH = "BATCH";
            public const string CURRENT_BATCH = "CURRENT_BATCH";
            public const string DEFICIENCY_LEVEL = "DEFICIENCY_LEVEL";
            public const string DEFICIENCY_TYPE = "DEFICIENCY_TYPE";
            public const string RESIDENCY_TYPE = "RESIDENCY_TYPE";
            public const string EXTENSION_ACTIVITY = "EXTENSION_ACTIVITY";
            public const string ACADEMIC_MENTOR = "ACADEMIC_MENTOR";
            public const string ACCOUNT_NO = "ACCOUNT_NO";
            public const string IFSC_CODE = "IFSC_CODE";
            public const string MICR_CODE = "MICR_CODE";
            public const string IDENTIFICATION_MARK1 = "IDENTIFICATION_MARK1";
            public const string IDENTIFICATION_MARK2 = "IDENTIFICATION_MARK2";
            public const string HOME_PHONE = "HOME_PHONE";
            public const string UPDATE_CR_FLAG = "UPDATE_CR_FLAG";
            public const string TC_APPLIED_DATE = "TC_APPLIED_DATE";
            public const string TC_GIVEN_DATE = "TC_GIVEN_DATE";
            public const string IS_DISCONTINUED = "IS_DISCONTINUED";
            public const string IS_COMPLETED = "IS_COMPLETED";
            public const string ID_ISSUE_DATE = "ID_ISSUE_DATE";
            public const string ID_VALID_FROM = "ID_VALID_FROM";
            public const string ID_VALID_TO = "ID_VALID_TO";
            public const string LAST_SCHOOL_OR_COLLEGE = "LAST_SCHOOL_OR_COLLEGE";
            public const string LAST_STUDIED_PLACE = "LAST_STUDIED_PLACE";
            public const string SUB_GROUP = "SUB_GROUP";
            public const string LAST_UPDATE_TIME = "LAST_UPDATE_TIME";
            public const string TEMP_ID = "TEMP_ID";
            public const string EXAM_PASSED = "EXAM_PASSED";
            public const string MARITAL_STATUS = "MARITAL_STATUS";
            public const string HAILING_FROM = "HAILING_FROM";
            public const string REMARKS = "REMARKS";
            public const string LAST_STUDIED_CLASS = "LAST_STUDIED_CLASS";
            public const string SCHOLARSHIP = "SCHOLARSHIP";
            public const string REPORT_DATE = "REPORT_DATE";
            public const string TC_SERIAL_NO = "TC_SERIAL_NO";
            public const string AADHAR_NUMBER = "AADHAR_NUMBER";
            public const string IS_LEFT = "IS_LEFT";
            public const string CONDUCT = "CONDUCT";
            public const string PHOTO = "PHOTO";
            public const string NAME_IN_NATIVE = "NAME_IN_NATIVE";
            public const string FIRSTGRADUATE = "FIRSTGRADUATE";
            public const string SPORTS = "SPORTS";
            public const string EXTRA_CURRRICULAR = "EXTRA_CURRRICULAR";
            public const string MEDIUM_STUDY = "MEDIUM_STUDY";
            public const string HSS_TOTAL = "HSS_TOTAL";
            public const string STUDY_BOARD = "STUDY_BOARD";
            public const string EMIS_NO = "EMIS_NO";
            public const string HSS_GROUP_CODE = "HSS_GROUP_CODE";
            public const string EX_SERVICE_MAN = "EX_SERVICE_MAN";
            public const string HSS_NO = "HSS_NO";
            public const string ADDRESS_LINE1 = "ADDRESS_LINE1";
            public const string PINCODE = "PINCODE";
            public const string CITY = "CITY";
            public const string STATE_ID = "STATE_ID";
            public const string TAMIL_NAME = "TAMIL_NAME";
            public const string APPTYPE_ID = "APPTYPE_ID";
            public const string LEFT_REASON = "LEFT_REASON";
            public const string PART_1_LANGUAGE = "PART_1_LANGUAGE";
            public const string LAST_STUDIED_UNIVERSITY = "LAST_STUDIED_UNIVERSITY";
            public const string YEAR_OF_PASSING = "YEAR_OF_PASSING";
            public const string PERCENTAGE = "PERCENTAGE";
            public const string ALLIED_2 = "ALLIED_2";
            public const string ALLIED_1 = "ALLIED_1";
            public const string MAIN_SUBJECT = "MAIN_SUBJECT";
            public const string BANK_NAME = "BANK_NAME";
            public const string UMIS_NO = "UMIS_NO";
            public const string BRANCH_NAME = "BRANCH_NAME";
            public const string ENROLLMENT_NO = "ENROLLMENT_NO";
            public const string QUALIFIED_PROMOTION = "QUALIFIED_PROMOTION";
            public const string SERIAL_NO = "SERIAL_NO";
        }
        public static class STU_ACTIVITY
        {
            public const string ACTIVITY_ID = "ACTIVITY_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string POST_HELD = "POST_HELD";
            public const string INITIATIVE_SHOWN = "INITIATIVE_SHOWN";
            public const string PARTICIPATION = "PARTICIPATION";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string EXTRA_ACTIVITY = "EXTRA_ACTIVITY";
            public const string POSITION_OBTAINED = "POSITION_OBTAINED";
            public const string IS_DELETED = "IS_DELETED";
            public const string ACTIVITY = "ACTIVITY";
            public const string ACTIVITY_IMAGE1 = "ACTIVITY_IMAGE1";
            public const string ACTIVITY_IMAGE2 = "ACTIVITY_IMAGE2";
            public const string ACTIVITY_IMAGE3 = "ACTIVITY_IMAGE3";
        }
        public static class STU_CERTIFICATE
        {
            public const string CERTIFICATE_ID = "CERTIFICATE_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string CERTIFICATE_NO = "CERTIFICATE_NO";
            public const string RECEIVED_DATE = "RECEIVED_DATE";
            public const string GIVEN_DATE = "GIVEN_DATE";
            public const string ARCHIVE = "ARCHIVE";
            public const string PURPOSE = "PURPOSE";
            public const string REGISTER_NUMBER = "REGISTER_NUMBER";
            public const string CERTIFICATE_NAME = "CERTIFICATE_NAME";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class STU_COUNSELING
        {
            public const string SC_ID = "SC_ID";
            public const string DOC = "DOC";
            public const string DURATION = "DURATION";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string REMARKS = "REMARKS";
            public const string COMMENTS = "COMMENTS";
            public const string IS_DELETED = "IS_DELETED";
            public const string BATCH = "BATCH";
        }
        public static class STU_COURSES
        {
            public const string COURSE_REGISTRATION_ID = "COURSE_REGISTRATION_ID";
            public const string PROGRAM_ID = "PROGRAM_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string PART = "PART";
            public const string MAIN_SUBJECT = "MAIN_SUBJECT";
            public const string ALLIED1 = "ALLIED1";
            public const string ALLIED2 = "ALLIED2";
            public const string ALLIED3 = "ALLIED3";
            public const string ALLIED4 = "ALLIED4";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ELECTIVE1 = "ELECTIVE1";
            public const string ELECTIVE2 = "ELECTIVE2";
            public const string ELECTIVE3 = "ELECTIVE3";
            public const string ELECTIVE4 = "ELECTIVE4";
            public const string TEMP_ID = "TEMP_ID";
            public const string SPECIAL_SUBJECT = "SPECIAL_SUBJECT";
        }
        public static class STU_INCIDENT
        {
            public const string INCIDENT_ID = "INCIDENT_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string DATE_OF_INCIDENT = "DATE_OF_INCIDENT";
            public const string TIME_OF_INCIDENT = "TIME_OF_INCIDENT";
            public const string PLACE_OF_INCIDENT = "PLACE_OF_INCIDENT";
            public const string FIRST_AID_DONE = "FIRST_AID_DONE";
            public const string INFORMED_TO_PARENTS = "INFORMED_TO_PARENTS";
            public const string DATE_INFORMED = "DATE_INFORMED";
            public const string INCIDENT_DETAILS = "INCIDENT_DETAILS";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class STU_ISSUE_ETC
        {
            public const string TC_ID = "TC_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string TC_PRODUCED_NUMBER = "TC_PRODUCED_NUMBER";
            public const string TC_PRODUCE_DATE = "TC_PRODUCE_DATE";
            public const string TC_ISSUED_NUMBER = "TC_ISSUED_NUMBER";
            public const string TC_ISSUED_DATE = "TC_ISSUED_DATE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ISSUED_TC = "IS_ISSUED_TC";
            public const string IS_DUPLICATED = "IS_DUPLICATED";
            public const string SERIAL_NO = "SERIAL_NO";
            public const string FLAG = "FLAG";
        }
        public static class STU_SIBLING
        {
            public const string SIBLING_ID = "SIBLING_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string NAME = "NAME";
            public const string AGE = "AGE";
            public const string INSTITUTE_NAME = "INSTITUTE_NAME";
            public const string PROGRAM = "PROGRAM";
            public const string IS_DELETED = "IS_DELETED";
            public const string QUALIFICATION = "QUALIFICATION";
            public const string EMPLOYED = "EMPLOYED";
            public const string DATE_OF_BIRTH = "DATE_OF_BIRTH";
            public const string GENDER = "GENDER";
            public const string OCCUPATION = "OCCUPATION";
            public const string MONTHLY_INCOME = "MONTHLY_INCOME";
            public const string PROGNAME = "PROGNAME";
            public const string IS_SAME_COLLEGE = "IS_SAME_COLLEGE";
        }
        public static class STU_TALENTS
        {
            public const string TALENT_ID = "TALENT_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string DATE = "DATE";
            public const string TALENT_AREA = "TALENT_AREA";
            public const string TALENT_ACTIVITY_TYPE = "TALENT_ACTIVITY_TYPE";
            public const string TALENT_DESCRIPTION = "TALENT_DESCRIPTION";
            public const string STATUS = "STATUS";
            public const string GRADE = "GRADE";
            public const string REMARKS = "REMARKS";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class STU_COLLEGE_HISTORY
        {
            public const string SCHOOL_ID = "SCHOOL_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string AGE = "AGE";
            public const string SCHOOL_NAME = "SCHOOL_NAME";
            public const string GRADE = "GRADE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ENTRY_DATE = "ENTRY_DATE";
            public const string EXIT_DATE = "EXIT_DATE";
            public const string ACADEMIC_LEVELS = "ACADEMIC_LEVELS";
            public const string CITY = "CITY";
            public const string COUNTRY = "COUNTRY";
            public const string OFFICIAL_WEBSITE = "OFFICIAL_WEBSITE";
            public const string CURRICULUM = "CURRICULUM";
            public const string REASON_FOR_WITHDRAWAL = "REASON_FOR_WITHDRAWAL";
            public const string LAST_ATTENDANCE_DATE = "LAST_ATTENDANCE_DATE";
        }
        public static class STU_ACHIEVEMENTS
        {
            public const string ACHIEVE_ID = "ACHIEVE_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string ACHIEVEMENTS = "ACHIEVEMENTS";
            public const string IS_DELETED = "IS_DELETED";
            public const string CLASS_ID = "CLASS_ID";
            public const string DATE = "DATE";
            public const string IMAGE_PATH = "IMAGE_PATH";
            public const string ACTIVITY = "ACTIVITY";
        }
        public static class STU_MEDICAL
        {
            public const string MEDICAL_ID = "MEDICAL_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string ALLERGIES = "ALLERGIES";
            public const string DOCTOR_NOTE = "DOCTOR_NOTE";
            public const string MEDICAL_DATE = "MEDICAL_DATE";
            public const string MEDICAL_CONDITION = "MEDICAL_CONDITION";
            public const string IS_DELETED = "IS_DELETED";
            public const string VACCINATION = "VACCINATION";
        }
        public static class STU_REPORT_TEMPLATE
        {
            public const string REPORT_ID = "REPORT_ID";
            public const string MODULE = "MODULE";
            public const string CREATED_BY = "CREATED_BY";
            public const string CREATED_ON = "CREATED_ON";
            public const string REPORT_NAME = "REPORT_NAME";
            public const string FIELD_LIST = "FIELD_LIST";
            public const string CONDITION = "CONDITION";
            public const string ORDER_BY_FIELD = "ORDER_BY_FIELD";
            public const string GROUP_BY_FIELD = "GROUP_BY_FIELD";
            public const string STATISSTIC_FIELD = "STATISSTIC_FIELD";
            public const string COUNT_FIELD = "COUNT_FIELD";
            public const string COUNT_CAPTION = "COUNT_CAPTION";
        }
        public static class STU_LEAVE_LETTER
        {
            public const string LEAVE_LETTER_ID = "LEAVE_LETTER_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string LEAVE_LETTER_TITLE = "LEAVE_LETTER_TITLE";
            public const string LEAVE_LETTER_FORMAT = "LEAVE_LETTER_FORMAT";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string LETTER_FOR = "LETTER_FOR";
        }
        public static class STU_TRANSFER_CERTIFICATE_2017
        {
            public const string CERTIFICATE_ID = "CERTIFICATE_ID";
            public const string SERIAL_NO = "SERIAL_NO";
            public const string ADMISSION_NO = "ADMISSION_NO";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string LAST_NAME = "LAST_NAME";
            public const string GENDER = "GENDER";
            public const string RELIGION = "RELIGION";
            public const string NATIONALITY = "NATIONALITY";
            public const string CASTE = "CASTE";
            public const string COMMUNITY = "COMMUNITY";
            public const string DATE_OF_BIRTH = "DATE_OF_BIRTH";
            public const string ADMISSION_DATE = "ADMISSION_DATE";
            public const string ADMITTED_CLASS = "ADMITTED_CLASS";
            public const string IDENTIFICATION_MARK1 = "IDENTIFICATION_MARK1";
            public const string IDENTIFICATION_MARK2 = "IDENTIFICATION_MARK2";
            public const string MAIN_COURSE = "MAIN_COURSE";
            public const string ALLIED = "ALLIED";
            public const string FEE_PAID = "FEE_PAID";
            public const string SCHOLAR_SHIP = "SCHOLAR_SHIP";
            public const string LEAVING_DATE = "LEAVING_DATE";
            public const string REASON_FOR_LEAVING = "REASON_FOR_LEAVING";
            public const string LEAVING_CLASS = "LEAVING_CLASS";
            public const string CONDUCT = "CONDUCT";
            public const string TC_APPLIED_DATE = "TC_APPLIED_DATE";
            public const string TC_GIVEN_DATE = "TC_GIVEN_DATE";
            public const string FATHER_NAME = "FATHER_NAME";
            public const string GUARDIAN_NAME = "GUARDIAN_NAME";
            public const string COURSE_OF_STUDY = "COURSE_OF_STUDY";
            public const string ACADEMIC_YEARS = "ACADEMIC_YEARS";
            public const string CLASSES_STUDEIED = "CLASSES_STUDEIED";
            public const string FIRST_LANGUAGE = "FIRST_LANGUAGE";
            public const string DATETOWORDS = "DATETOWORDS";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string REGISTER_ID = "REGISTER_ID";
            public const string MEDIUM_OF_INSTRUCTION = "MEDIUM_OF_INSTRUCTION";
            public const string STUDIED_FROM = "STUDIED_FROM";
            public const string CONTACT_NO = "CONTACT_NO";
            public const string ADDRESS = "ADDRESS";
            public const string BEGINYEAR = "BEGINYEAR";
            public const string ENDYEAR = "ENDYEAR";
            public const string CLASS_YEAR = "CLASS_YEAR";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string QUALIFIED_PROMOTION = "QUALIFIED_PROMOTION";
            public const string UMIS_NO = "UMIS_NO";


        }

        public static class QUIZ_SETTING
        {
            public const string SETTING_ID = "SETTING_ID";
            public const string SETTING_NAME = "SETTING_NAME";
            public const string NO_OF_QUESTIONS = "NO_OF_QUESTIONS";
            public const string EACH_QUESTION_MARKS = "EACH_QUESTION_MARKS";
            public const string TOTAL_MARK = "TOTAL_MARK";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string START_DATE = "START_DATE";
            public const string END_DATE = "END_DATE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class QUIZ_STUDENT_SHUFFLE
        {
            public const string QUIZ_SHUFFLE_ID = "QUIZ_SHUFFLE_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string ENTRY_ID = "ENTRY_ID";
            public const string SETTING_ID = "SETTING_ID";
            public const string SET_ID = "SET_ID";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class QUIZ_MATCH_THE_FOLLOWING_QUESTIONS
        {
            public const string QUIZ_ID = "QUIZ_ID";
            public const string ENTRY_ID = "ENTRY_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string QUESTION = "QUESTION";
            public const string SET_ID = "SET_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class QUIZ_MATCH_THE_FOLLOWING_OPTIONS
        {
            public const string OPTION_ID = "OPTION_ID";
            public const string QUESTION = "QUESTION";
            public const string ANSWER = "ANSWER";
            public const string QUIZ_ID = "QUIZ_ID";
            public const string SET_ID = "SET_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_HOURS
        {

            public const string HOUR_ID = "HOUR_ID";
            public const string HOUR = "HOUR";
            public const string TIME_FROM = "TIME_FROM";
            public const string TIME_TO = "TIME_TO";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_FEE_MAIN_HEAD
        {
            public const string MAIN_HEAD_ID = "MAIN_HEAD_ID";
            public const string MAIN_HEAD = "MAIN_HEAD";
            public const string IS_USED = "IS_USED";
            public const string IS_DELETED = "IS_DELETED";

        }

        public static class SUP_FEE_CATEGORY
        {
            public const string FEE_CATEGORY_ID = "FEE_CATEGORY_ID";
            public const string FEE_CATEGORY = "FEE_CATEGORY";
            public const string IS_USED = "IS_USED";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_FEE_FREQUENCY
        {
            public const string FREQUENCY_ID = "FREQUENCY_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string FREQUENCY_NAME = "FREQUENCY_NAME";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string STATUS = "STATUS";
            public const string IS_USED = "IS_USED";
            public const string LAST_DATE_OF_PAYMENT = "LAST_DATE_OF_PAYMENT";
            public const string IS_DOWNLOADED = "IS_DOWNLOADED";
            public const string IS_UPDATED = "IS_UPDATED";
            public const string DOWNLOAD_TIME = "DOWNLOAD_TIME";
            public const string TYPE = "TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string FREQUENCY_TYPE = "FREQUENCY_TYPE";
            public const string FREQUENCY_TYPE_ID = "FREQUENCY_TYPE_ID";
            public const string FEE_MODE = "FEE_MODE";


        }
        public static class SUP_HEAD
        {
            public const string HEAD_ID = "HEAD_ID";
            public const string HEAD = "HEAD";
            public const string HEAD_CODE = "HEAD_CODE";
            public const string ACCOUNT = "ACCOUNT";
            public const string HEAD_TYPE = "HEAD_TYPE";
            public const string IS_USED = "IS_USED";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_DOWNLOADED = "IS_DOWNLOADED";
            public const string DOWNLOAD_TIME = "DOWNLOAD_TIME";
            public const string IS_UPDATED = "IS_UPDATED";
            public const string PROGRAMME_MODE = "PROGRAMME_MODE";
            public const string FEE_CATEGORY = "FEE_CATEGORY";
            public const string TEMP_ID = "TEMP_ID";
            public const string HEAD_ORDER = "HEAD_ORDER";
            public const string IS_REFUNDABLE = "IS_REFUNDABLE";
            public const string SHIFT = "SHIFT";

        }
        public static class SUP_BATCHES
        {
            public const string BATCH_ID = "BATCH_ID";
            public const string BATCH = "BATCH";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string BT_SERIES_ROLLNO = "BT_SERIES_ROLLNO";
            public const string BT_SERIES_REGNO = "BT_SERIES_REGNO";
            public const string BT_SERIES_ADMNO = "BT_SERIES_ADMNO";
        }
        public static class SUP_BLOOD_GROUP
        {
            public const string BLOOD_GROUP_ID = "BLOOD_GROUP_ID";
            public const string BLOOD_GROUP_NAME = "BLOOD_GROUP_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_CATEGORY
        {
            public const string CATEGORY_ID = "CATEGORY_ID";
            public const string CATEGORY_NAME = "CATEGORY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }
        public static class SUP_CIA_COMPONENTS
        {
            public const string COMPONENT_ID = "COMPONENT_ID";
            public const string COMPONENT = "COMPONENT";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_CLASS_LEVEL
        {
            public const string CLASSLEVELID = "CLASSLEVELID";
            public const string CLASSLEVEL = "CLASSLEVEL";
            public const string ISDELETED = "ISDELETED";
            public const string ISUSED = "ISUSED";
        }
        public static class SupClassLevel
        {
            public const string UG = "1";
            public const string PG = "2";
            public const string DIPLOMA = "3";
            public const string MPHIL = "4";
        }
        public static class SUP_CLASS_YEAR
        {
            public const string CLASS_YEAR_ID = "CLASS_YEAR_ID";
            public const string CLASS_YEAR = "CLASS_YEAR";
            public const string IS_DELETED = "ISDELETED";
            public const string IS_ACTIVE = "ISUSED";
        }
        public static class SUP_FIELDS
        {
            public const string FIELD_ID = "FIELD_ID";
            public const string FIELD_NAME = "FIELD_NAME";
            public const string ALICE_NAME = "ALICE_NAME";

        }
        public static class SUP_CLASS_TYPE
        {
            public const string CLASS_TYPE_ID = "CLASS_TYPE_ID";
            public const string CLASS_TYPE = "CLASS_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SupClassType
        {
            public const string Combined = "1";
            public const string Regular = "2";
            public const string Selected = "3";
        }
        public static class SUP_COMMUNITY
        {
            public const string COMMUNITYID = "COMMUNITYID";
            public const string COMMUNITY = "COMMUNITY";
            public const string COMMUNITYORDER = "COMMUNITYORDER";
            public const string ISUSED = "ISUSED";
            public const string ISDELETED = "ISDELETED";
        }
        public static class SUP_CONTACT_TYPE
        {
            public const string CONDUCT_ID = "CONDUCT_ID";
            public const string CONDUCT_TYPE = "CONDUCT_TYPE";
        }
        public static class SUP_STATE
        {
            public const string STATE_ID = "STATE_ID";
            public const string STATE_NAME = "STATE_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_DEPARTMENT
        {
            public const string DEPARTMENT_ID = "DEPARTMENT_ID";
            public const string CODE = "CODE";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string ISUSED = "ISUSED";
            public const string ISDELETED = "ISDELETED";
        }
        public static class SUP_DESIGNATION
        {
            public const string DESIGNATIONID = "DESIGNATIONID";
            public const string DESIGNATION = "DESIGNATION";
            public const string ISUSED = "ISUSED";
            public const string ISDELETED = "ISDELETED";
            public const string DESIGNATIONORDER = "DESIGNATIONORDER";
        }
        public static class SUP_STAFF_ROLES_RESPONSIBILE
        {
            public const string STF_ROLE_RES_ID = "STF_ROLE_RES_ID";
            public const string ROLES__RESPONSIBILITIES = "ROLES__RESPONSIBILITIES";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ORDER_BY = "ORDER_BY";
        }
        public static class SUP_GENDER
        {
            public const string GENDER_ID = "GENDER_ID";
            public const string GENDER_NAME = "GENDER_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_MARRITAL_STATUS
        {
            public const string MARITAL_STAUS_ID = "MARITAL_STAUS_ID";
            public const string MARITAL_STATUS_NAME = "MARITAL_STATUS_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }
        public static class SUP_MOTHER_TONGUE
        {
            public const string MOTHER_TONGUE_ID = "MOTHER_TONGUE_ID";
            public const string MOTHER_TONGUE_NAME = "MOTHER_TONGUE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_NATIONALITY
        {
            public const string NATIONALITY_ID = "NATIONALITY_ID";
            public const string NATIONALITY_NAME = "NATIONALITY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_PAPER_CODE
        {
            public const string PAPER_CODE_ID = "PAPER_CODE_ID";
            public const string PAPER_CODE = "PAPER_CODE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_USED = "IS_USED";
            public const string TEMP_ID = "TEMP_ID";
        }
        public static class SUP_PAPER_TYPE
        {
            public const string PAPER_TYPE_ID = "PAPER_TYPE_ID";
            public const string PAPER_TYPE = "PAPER_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_PART
        {
            public const string PART_ID = "PART_ID";
            public const string PART = "PART";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_SEMESTER
        {
            public const string SUP_SEMESTER_ID = "SUP_SEMESTER_ID";
            public const string SEMESTER_NAME = "SEMESTER_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        } 
        public static class SUP_CERTIFICATE_LIST
        {
            public const string CERTIFICATE_ID = "CERTIFICATE_ID";
            public const string CERTIFICATE_NAME = "CERTIFICATE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_CERTIFICATE_CATEGORY_LIST
        {
            public const string CATEGORY_ID = "CATEGORY_ID";
            public const string CATEGORY_NAME = "CATEGORY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_CERTIFICATE_AUTHORIZE_PERSON
        {
            public const string AUTHORIZER_ID = "AUTHORIZER_ID";
            public const string AUTHORIZER_NAME = "AUTHORIZER_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CERTIFICATE_LIST
        {
            public const string CERTIFICATE_ID = "CERTIFICATE_ID";
            public const string CERTIFICATE_NAME = "CERTIFICATE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SEMESTER_TYPE
        {
            public const string SEMESTER_TYPE_ID = "SEMESTER_TYPE_ID";
            public const string SEMESTER_TYPE = "SEMESTER_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ODD = "1,3,5,7";
            public const string EVEN = "2,4,6,8";
        }

        public static class SUP_PHYSICAL_STATUS
        {
            public const string PHYSICAL_STATUS_ID = "PHYSICAL_STATUS_ID";
            public const string PHYSICAL_STATUS_NAME = "PHYSICAL_STATUS_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_PROGRAMME_LEVEL
        {
            public const string PROGRAMME_LEVEL_ID = "PROGRAMME_LEVEL_ID";
            public const string PROGRAMME_LEVEL = "PROGRAMME_LEVEL";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string PL_SERIES_ROLLNO = "PL_SERIES_ROLLNO";
            public const string PL_SERIES_REGNO = "PL_SERIES_REGNO";
            public const string PL_SERIES_ADMNO = "PL_SERIES_ADMNO";
        }
        public static class SUP_PROGRAMME_MODE
        {
            public const string PROGRAMME_MODE_ID = "PROGRAMME_MODE_ID";
            public const string PROGRAMME_MODE = "PROGRAMME_MODE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string TEMP_ID = "IS_DELETED";
        }
        public static class SUP_PROGRAMME_TYPE
        {
            public const string PROGRAMME_TYPE_ID = "PROGRAMME_TYPE_ID";
            public const string PROGRAMME_TYPE = "PROGRAMME_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_RELIGION
        {
            public const string RELIGIONID = "RELIGIONID";
            public const string RELIGION = "RELIGION";
            public const string RELIGIONORDER = "RELIGIONORDER";
            public const string ISUSED = "ISUSED";
            public const string ISDELETED = "ISDELETED";
            public const string ARABICDATA = "ARABICDATA";
        }
        public static class SUP_ACHIEVEMENT
        {
            public const string ACHIEVEMENT_ID = "ACHIEVEMENT_ID";
            public const string ACHIEVEMENT_NAME = "ACHIEVEMENT_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SDGLIST
        {
            public const string SDG_ID = "SDG_ID";
            public const string SDG_NAME_LIST = "SDG_NAME_LIST";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_ROLE
        {
            public const string ROLE_ID = "ROLE_ID";
            public const string ROLE_NAME = "ROLE_NAME";
            public const string DASHBOARD_ID = "DASHBOARD_ID";
            public const string SHORT_NAME = "SHORT_NAME";
        }
        public static class NoticeBoard
        {
            public const string NOTICEBOARD_ID = "NOTICEBOARD_ID";
            public const string NOTICEBOARD = "NOTICEBOARD";
        }
        public static class SUP_SHIFT
        {
            public const string SHIFT_ID = "SHIFT_ID";
            public const string SHIFT_NAME = "SHIFT_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_ALU_PRESENT_STATUS
        {
            public const string PRESENT_STATUS_ID = "PRESENT_STATUS_ID";
            public const string PRESENT_STATUS_NAME = "PRESENT_STATUS_NAME";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_STAFF_APPLICATION_TYPE
        {
            public const string ALLOCATION_TYPE_ID = "ALLOCATION_TYPE_ID";
            public const string ALLOCATION_TYPE = "ALLOCATION_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_UNIVERSITY_TYPE
        {
            public const string UNIVERSITY_ID = "UNIVERSITY_ID";
            public const string UNIVERSITY = "UNIVERSITY";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_USER_TYPE
        {
            public const string USER_TYPE_ID = "USER_TYPE_ID";
            public const string USER_TYPE_NAME = "USER_TYPE_NAME";
        }

        public static class COURSE_AND_COURSE_INFO_BY_STAFF_ID
        {
            public const string TOTAL_STUDENTS = "TOTAL_STUDENTS";
            public const string COURSE_TITLE = "COURSE_TITLE";
            public const string CLASS_NAME = "CLASS_NAME";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string DEPARTMENT = "DEPARTMENT";
        }

        public static class FBRATING_BY_STAFF_BY_COURSE_ID
        {
            public const string ASSESSOR = "ASSESSOR";
            public const string QUESTION = "QUESTION";
            public const string GROUP_TITLE = "GROUP_TITLE";
            public const string ANSWER = "ANSWER";
            public const string VERY_GOOD = "VERY_GOOD";
            public const string GOOD = "GOOD";
            public const string SATISFACTORY = "SATISFACTORY";
            public const string POOR = "POOR";
            public const string VERY_POOR = "VERY_POOR";
            public const string RATING = "RATING";
            public const string OBJECTIVE_ID = "OBJECTIVE_ID";
            public const string QUESTION_ID = "QUESTION_ID";
            public const string OBJECTIVES = "OBJECTIVES";

        }
        public static class PRINCIPAL_VIEW_FEEDBACK_RESULT
        {

            public const string STAFF_NAME = "STAFF_NAME";
            public const string STAFF_CODE = "STAFF_CODE";
            public const string COURSE_ID = "COURSE_ID";
            public const string COURSE_TITLE = "COURSE_TITLE";
            public const string COURSE_TYPE = "COURSE_TYPE";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string QUESTION = "QUESTION";
            public const string GROUP_TITLE = "GROUP_TITLE";
            public const string VERY_GOOD = "VERY_GOOD";
            public const string GOOD = "GOOD";
            public const string SATISFACTORY = "SATISFACTORY";
            public const string POOR = "POOR";
            public const string VERY_POOR = "VERY_POOR";
            public const string ASSESSEE = "ASSESSEE";
            public const string DEPARTMENT = "DEPARTMENT";
            public const string CLASS_NAME = "CLASS_NAME";
            public const string ASSESSOR = "ASSESSOR";
            public const string TOTAL_STUDENTS = "TOTAL_STUDENTS";

        }
        public static class SUP_DEGREE
        {
            public const string DEGREE_ID = "DEGREE_ID";
            public const string DEGREE = "DEGREE";
            public const string ISACTIVE = "ISACTIVE";
            public const string ISDELETED = "ISDELETED";
        }
        public static class SUP_DURATION_UNIT
        {
            public const string DURATION_UNIT_ID = "DURATION_UNIT_ID";
            public const string DURATION_UNIT = "DURATION_UNIT";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_CHANNEL
        {
            public const string CHANNEL_ID = "CHANNEL_ID";
            public const string CHANNEL = "CHANNEL";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SUBJECT
        {
            public const string SUBJECT_ID = "SUBJECT_ID";
            public const string SUBJECT = "SUBJECT";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_MEDIUM_OF_INSTRUCTION
        {
            public const string MEDIUM_ID = "MEDIUM_ID";
            public const string MEDIUM_OF_INSTRUCTION = "MEDIUM_OF_INSTRUCTION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_IS_AIDED
        {
            public const string IS_AIDED_ID = "IS_AIDED_ID";
            public const string IS_AIDED_NAME = "IS_AIDED_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_NON_AIDED
        {
            public const string NON_AIDED_ID = "NON_AIDED_ID";
            public const string NON_AIDED_NAME = "NON_AIDED_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_IS_REGULAR
        {
            public const string IS_REGULAR_ID = "IS_REGULAR_ID";
            public const string IS_REGULAR_NAME = "IS_REGULAR_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class Sup_PUBLISH_CATEGORY
        {
            public const string PUBLISH_CATEGORY_ID = "PUBLISH_CATEGORY_ID";
            public const string PUBLISH_CATEGORY = "PUBLISH_CATEGORY";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class sup_LEVEL
        {
            public const string LEVEL_ID = "LEVEL_ID";
            public const string LEVEL = "LEVEL";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class Sup_VOLUME
        {
            public const string VOLUME_ID = "VOLUME_ID";
            public const string VOLUME = "VOLUME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class Sup_Qualification
        {
            public const string QUALIFICATION_ID = "QUALIFICATION_ID";
            public const string QUALIFICATION = "QUALIFICATION";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class sup_staff_subcategory
        {
            public const string STF_CATEGORY_ID = "STF_CATEGORY_ID";
            public const string STF_CATEGORY = "STF_CATEGORY";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class sup_designation
        {
            public const string DESIGNATIONID = "DESIGNATIONID";
            public const string DESIGNATION = "DESIGNATION";
            public const string ISDELETED = "ISDELETED";
            public const string ISUSED = "ISUSED";
            public const string DESIGNATIONORDER = "DESIGNATIONORDER";
        }
        public static class sup_physical_status
        {
            public const string PHYSICAL_STATUS_ID = "PHYSICAL_STATUS_ID";
            public const string PHYSICAL_STATUS_NAME = "PHYSICAL_STATUS_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class sup_Staff_Status
        {
            public const string STATUS_ID = "STATUS_ID";
            public const string STATUS = "STATUS";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class SUP_FAMILY
        {
            public const string RELATION_ID = "RELATION_ID";
            public const string RELATION = "RELATION";
        }
        public static class SUP_OCCUPATION
        {
            public const string OCCUPATION_ID = "OCCUPATION_ID";
            public const string OCCUPATION_NAME = "OCCUPATION_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ARABICDATA = "ARABICDATA";
        }
        public static class SUP_EDUCATION
        {
            public const string EDUCATION_ID = "EDUCATION_ID";
            public const string EDUCATION_NAME = "EDUCATION_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_EDUCATION_BOARD
        {
            public const string EDUCATION_BOARD_ID = "EDUCATION_BOARD_ID";
            public const string EDUCATION_BOARD = "EDUCATION_BOARD";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_EXTRA_CURRICULAM
        {
            public const string CURUCULAR_ID = "CURUCULAR_ID";
            public const string CURUCULAR_NAME = "CURUCULAR_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_MEDIUMSTUDY
        {
            public const string MEDIUM_ID = "MEDIUM_ID";
            public const string MEDIUM_NAME = "MEDIUM_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_ACTIVITY
        {
            public const string ACTIVITY_ID = "ACTIVITY_ID";
            public const string ACTIVITY_NAME = "ACTIVITY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_WEEK_NO
        {
            public const string WEEK_NO_ID = "WEEK_NO_ID";
            public const string WEEK_NAME = "WEEK_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SECOND_LANGUAGE
        {
            public const string SECOND_LANGUAGE_ID = "SECOND_LANGUAGE_ID";
            public const string SECOND_LANGUAGE_NAME = "SECOND_LANGUAGE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SPOKEN_LANGUAGE
        {
            public const string SPOKEN_LANGUAGE_ID = "SPOKEN_LANGUAGE_ID";
            public const string SPOKEN_LANGUAGE_NAME = "SPOKEN_LANGUAGE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LANGUAGE
        {
            public const string LANGUAGE_ID = "LANGUAGE_ID";
            public const string LANGUAGE_NAME = "LANGUAGE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_IS_CHOICE
        {
            public const string IS_CHOICE_ID = "IS_CHOICE_ID";
            public const string IS_CHOICE_NAME = "IS_CHOICE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SPECIALLY_ABLED
        {
            public const string SPECIALLY_ABLED_ID = "SPECIALLY_ABLED_ID";
            public const string SPECIALLY_ABLED_NAME = "SPECIALLY_ABLED_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_RESIDENCY_TYPE
        {
            public const string RESIDENCY_TYPE_ID = "RESIDENCY_TYPE_ID";
            public const string RESIDENCY_TYPE_NAME = "RESIDENCY_TYPE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LEFT_REASON
        {
            public const string REASON_ID = "REASON_ID";
            public const string REASON = "REASON";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_ACTIVITY_TYPE
        {
            public const string ACTIVITY_TYPE_ID = "ACTIVITY_TYPE_ID";
            public const string ACTIVITY_TYPE = "ACTIVITY_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_ACTIVITY_LEVEL
        {
            public const string ACTIVITY_LEVEL_ID = "ACTIVITY_LEVEL_ID";
            public const string ACTIVITY_LEVEL = "ACTIVITY_LEVEL";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class EXAM_PROGRAMME_WISE_HEADS
        {
            public const string PROGRAMME_HEAD_ID = "PROGRAMME_HEAD_ID";
            public const string PROGRAMME_ID = "PROGRAMME_ID";
            public const string BATCH_ID = "BATCH_ID";
            public const string AMOUNT = "AMOUNT";
            public const string HEAD_ID = "HEAD_ID";
            public const string IS_USED = "IS_USED";
            public const string IS_DELETED = "IS_DELETED";
            public const string SEMESTER = "SEMESTER";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR ";
        }
        public static class SUP_EXAMPASSED
        {
            public const string EXAM_PASSED_ID = "EXAM_PASSED_ID";
            public const string EXAM_PASSED = "EXAM_PASSED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_OPTION
        {
            public const string OPTION_ID = "OPTION_ID";
            public const string OPTION_NAME = "OPTION_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_OVERALL_GRADE
        {
            public const string OVERALL_GRADE_ID = "OVERALL_GRADE_ID";
            public const string OVERALL_GRADE = "OVERALL_GRADE";
            public const string PERCENTAGE_FROM = "PERCENTAGE_FROM";
            public const string PERCENTAGE_TO = "PERCENTAGE_TO";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_COUNTRY
        {
            public const string COUNTRY_ID = "COUNTRY_ID";
            public const string COUNTRY_NAME = "COUNTRY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_OPTION_NAME
        {
            public const string OPTION_ID = "OPTION_ID";
            public const string OPTION_NAME = "OPTION_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_IS_NME_SUBJECT
        {
            public const string IS_NME_ID = "IS_NME_ID";
            public const string IS_NME_NAME = "IS_NME_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_IS_ALLIED_SUBJECT
        {
            public const string IS_ALLIED_ID = "IS_ALLIED_ID";
            public const string IS_ALLIED_NAME = "IS_ALLIED_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_IS_COMPULSORY
        {
            public const string IS_COMPULSORY_ID = "IS_COMPULSORY_ID";
            public const string IS_COMPULSORY_NAME = "IS_COMPULSORY_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class CP_UGC_COURSE_TYPE
        {
            public const string UGC_COURSE_TYPE_ID = "UGC_COURSE_TYPE_ID";
            public const string UGC_COURSE_TYPE = "UGC_COURSE_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string PART_ID = "PART_ID";
            public const string PROGRAMME_LEVEL = "PROGRAMME_LEVEL";
        }
        public class SUP_NON_TEACHING_CATEGORY
        {
            public const string NON_TEACHING_CATEGORY_ID = "NON_TEACHING_CATEGORY_ID";
            public const string NON_TEACHING_CATEGORY_NAME = "NON_TEACHING_CATEGORY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }
        public static class SUP_TALENT_AREA
        {
            public const string ACTIVITY_LEVEL_ID = "ACTIVITY_LEVEL_ID";
            public const string ACTIVITY_LEVEL = "ACTIVITY_LEVEL";
        }
        public static class SUP_BOARD_MEMBER
        {
            public const string BOARD_MEMBER_ID = "BOARD_MEMBER_ID";
            public const string BOARD_MEMBER_NAME = "BOARD_MEMBER_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_CONDUCT
        {
            public const string CONDUCT_ID = "CONDUCT_ID";
            public const string CONDUCT_NAME = "CONDUCT_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class CALENDER
        {
            public const string CALENDER_ID = "CALENDER_ID";
            public const string EVENT_TITLE = "EVENT_TITLE";
            public const string START_DATE = "START_DATE";
            public const string END_DATE = "END_DATE";
            public const string COLOR = "COLOR";
            public const string YEAR = "YEAR";
            public const string MONTH = "MONTH";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CALENDER_2017
        {
            public const string DAY_ID = "DAY_ID";
            public const string DAY_ORDER_DATE = "DAY_ORDER_DATE";
            public const string DAY_TYPE = "DAY_TYPE";
            public const string DAY_ORDER = "DAY_ORDER";
            public const string DAY_ORDER_MONTH = "DAY_ORDER_MONTH";
            public const string IS_HOLIDAY = "IS_HOLIDAY";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class DayOrderCalender
        {
            public const string DAY_ID = "DAY_ID";
            public const string DAY_ORDER_ID = "DAY_ORDER_ID";
            public const string DAY_TYPE = "DAY_TYPE";
            public const string DAY_ORDER = "DAY_ORDER";
            public const string DAY_ORDER_MONTH = "DAY_ORDER_MONTH";
            public const string IS_HOLIDAY = "IS_HOLIDAY";
            public const string IS_DELETED = "IS_DELETED";
            public const string COLOR = "COLOR";
            public const string DAY_ORDER_DATE = "DAY_ORDER_DATE";
            public const string DAY_ORDER_END_DATE = "DAY_ORDER_END_DATE";

        }
        public static class CALENDER_EVENTS_2017
        {
            public const string EVENT_ID = "EVENT_ID";
            public const string DAY_ID = "DAY_ID";
            public const string EVENT_START_DATE = "EVENT_START_DATE";
            public const string EVENT_TYPE = "EVENT_TYPE";
            public const string EVENT_NAME = "EVENT_NAME";
            public const string EVENT_DESCRIPTION = "EVENT_DESCRIPTION";
            public const string EVENT_DEPARTMENT = "EVENT_DEPARTMENT";
            public const string EVENT_END_DATE = "EVENT_END_DATE";
            public const string STAFF_CATEGORY = "STAFF_CATEGORY";
            public const string ACCESS_VISIBILITY = "ACCESS_VISIBILITY";
            public const string VIEW_MANAGE = "VIEW_MANAGE";
            public const string USER_ID = "USER_ID";
            public const string RESPONSIBLE_STAFF = "RESPONSIBLE_STAFF";
            public const string RESPONSIBLE_STUDENT = "RESPONSIBLE_STUDENT";
            public const string EVENT_REPORT = "EVENT_REPORT";
            public const string EVENT_PARTICIPANTS = "EVENT_PARTICIPANTS";
            public const string EVENT_STATUS = "EVENT_STATUS";
            public const string EVENT_BUDGET = "EVENT_BUDGET";
            public const string AMOUNT_SPENT = "AMOUNT_SPENT";
            public const string HOLIDAY_TYPE = "HOLIDAY_TYPE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class STU_BONAFIDE_CERTIFICATE_2017
        {
            public const string BONAFIDE_ID = "BONAFIDE_ID";
            public const string ROLL_NO = "ROLL_NO";
            public const string NAME = "NAME";
            public const string FATHER_NAME = "FATHER_NAME";
            public const string CLASS = "CLASS";
            public const string DURING_YEAR = "DURING_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_HOUR_TYPE
        {

            public const string HOUR_TYPE_ID = "HOUR_TYPE_ID";
            public const string HOURTYPE = "HOURTYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class HourType
        {
            public const string Regular_Hour = "1";
            public const string Extension_Activities_Hour = "2";
            public const string Instruction_Hour = "3";
            public const string Library_Hour = "4";
            public const string NME = "5";
            public const string Language = "6";
            public const string Value_Education = "7";
        }
        public static class SUP_DAY_ORDERS
        {

            public const string DAY_ORDER_ID = "DAY_ORDER_ID";
            public const string DAY = "DAY";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class TT_TEMPLATE_SETTING
        {

            public const string SETTING_ID = "SETTING_ID";
            public const string SETTING_NAME = "SETTING_NAME";
            public const string ALLOW_STATIC_HOUR = "ALLOW_STATIC_HOUR";
            public const string NO_OF_HOURS = "NO_OF_HOURS";
            public const string NO_OF_DAYS = "NO_OF_DAYS";
            public const string BASIC_HOURS_FOR_HOD = "BASIC_HOURS_FOR_HOD";
            public const string BASIC_HOURS_FOR_STAFF = "BASIC_HOURS_FOR_STAFF";
            public const string SEMESTER_TYPE = "SEMESTER_TYPE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string TEMPLATE_ID = "TEMPLATE_ID";
            public const string GRADUATION_TYPE = "GRADUATION_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_BANK_ACCOUNT
        {
            public const string BANK_ACCOUNT_ID = "BANK_ACCOUNT_ID";
            public const string ACCOUNT_PURPOSE = "ACCOUNT_PURPOSE";
            public const string BANK = "BANK";
            public const string PASSBOOK_NO = "PASSBOOK_NO";
            public const string STARTED_DATE = "STARTED_DATE";
            public const string CLOSED_DATE = "CLOSED_DATE";
            public const string IS_USED = "IS_USED";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_BANK
        {
            public const string BANK_ID = "BANK_ID";
            public const string BANK_NAME = "BANK_NAME";
            public const string ADDRESS = "ADDRESS";
            public const string PHONE = "PHONE";
            public const string IS_USED = "IS_USED";
            public const string IS_DOWNLOADED = "IS_DOWNLOADED";
            public const string DOWNLOAD_TIME = "DOWNLOAD_TIME";
            public const string IS_UPDATED = "IS_UPDATED";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class EXAM_SETTING
        {
            public const string EXAM_SETTING_ID = "EXAM_SETTING_ID";
            public const string EXAM_NAME = "EXAM_NAME";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string M_EXAM = "M_EXAM";
            public const string M_PASS = "M_PASS";
            public const string SEMESTER = "SEMESTER";
            public const string LAST_DATE_FOR_EXAM_FEE = "LAST_DATE_FOR_EXAM_FEE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SUBJECT_TYPE
        {
            public const string SUBJECT_TYPE_ID = "SUBJECT_TYPE_ID";
            public const string SUBJECT_TYPE = "SUBJECT_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class ASSIGNMENT
        {
            public const string ASSIGNMENT_ID = "ASSIGNMENT_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string ASSIGNMENT_TITLE = "ASSIGNMENT_TITLE";
            public const string DATE_FROM = "DATE_FROM";
            public const string SUBMISSION_DATE = "SUBMISSION_DATE";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_SPL_ATTENDANCE_TYPE
        {
            public const string SPL_ATTENDANCE_TYPE_ID = "SPL_ATTENDANCE_TYPE_ID";
            public const string SPL_ATTENDANCE_TYPE = "SPL_ATTENDANCE_TYPE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string HOUR_FROM = "HOUR_FROM";
        }
        public static class SolutionInfiSMSSettingFields
        {
            public const string SMS_SETTING_ID = "SMS_SETTING_ID";
            public const string BASE_URL = "BASE_URL";
            public const string API_KEY = "API_KEY";
            public const string METHOD = "METHOD";
            public const string FORMAT = "FORMAT";
            public const string UNICODE = "UNICODE";
            public const string FLASH = "FLASH";
            public const string JSON = "JSON";
            public const string DLRURL = "DLRURL";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ACADEMIC_YEAR_ID = "ACADEMIC_YEAR_ID";
            public const string API_MODE = "API_MODE";
            public const string SENDER = "SENDER";
            public const string ID = "ID";
            public const string MESSAGE = "MESSAGE";
            public const string NUMBERS = "NUMBERS";
        }
        //Sub Message Type ...
        public static class SUP_MESSAGE_TYPE_ID
        {
            public const string COMMON_ANNOUNCEMENT = "1";
            public const string CLASS_WISE_ANNOUNCEMENT = "2";
            public const string STAFF_ANNOUNCEMENT = "3";
            public const string TEST = "4";
            public const string INDIVIDUA_ANNOUNCEMENT = "5";
            public const string ATTENDANCE = "6";
            public const string HOMEWORK = "7";

            public const string RESENT = "8";
            public const string EXAM_TIME_TABLE = "9";
            public const string GROUP_SMS = "10";
            public const string VOICE_SMS = "11";
            public const string SHEDULE_SMS = "12";
            public const string FEE_DUE_MESSAGE = "13";
            public const string HOSTEL = "14";
            public const string LATE_ENTRY = "13";

        }
        public static class IsActiveFalg
        {
            public static string IsActive = "1";
            public static string IsNotActive = "0";
        }
        public static class SENT_SMS_LIST
        {
            public const string SENT_SMS_LIST_ID = "SENT_SMS_LIST_ID";
            public const string DATE = "DATE";
            public const string COMPOSED_TIME = "COMPOSED_TIME";
            public const string SENT_TIME = "SENT_TIME";
            public const string SENT_DATE = "SENT_DATE";
            public const string DELIVERED_DATE = "DELIVERED_DATE";
            public const string CLASS_ID = "CLASS_ID";
            public const string CONTENT = "CONTENT";
            public const string RECEIPIENT_ID = "RECEIPIENT_ID";
            public const string IS_STAFF = "IS_STAFF";
            public const string TEMPLATE_ID = "TEMPLATE_ID";
            public const string STATUS = "STATUS";
            public const string CREDIT_COUNT = "CREDIT_COUNT";
            public const string TRANSACTION_ID = "TRANSACTION_ID";
            public const string SENT_ITEM_ID = "SENT_ITEM_ID";
            public const string MOBILE_NO = "MOBILE_NO";
            public const string MOBILE = "MOBILE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }
        public static class MessageType
        {
            //this ids should not change 
            public static string COMMON_ANNOUNCEMENT = "1";
            public static string CLASS_WISE_ANNOUNCEMENT = "2";
            public static string STAFF_ANNOUNCEMENT = "3";
            public static string TEST = "4";
            public static string INDIVIDUAL_ANNOUNCEMENT = "5";
            public static string ATTENDANCE = "6";
            public static string HOMEWORK = "7";
            public static string RESENT = "8";
            public static string EXAM_TIME_TABLE = "9";
            public static string GROUP_MESSAGE = "10";
            public static string HOSTEL = "12";
            public static string FEE = "13";
            public static string HALL_RESERVATION = "13";
            public static string OTP = "9";
            public static string LATE_ENTRY = "13";
        }
        public static class HALL_TICKET_VALIDATION
        {
            public const string HALL_TICKET_VALIDATION_ID = "HALL_TICKET_VALIDATION_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string HEAD = "HEAD";
            public const string BALANCE_AMOUNT = "BALANCE_AMOUNT";
            public const string IS_PAID = "IS_PAID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_FREQUENCY_TYPE
        {
            public const string FREQUENCY_TYPE_ID = "FREQUENCY_TYPE_ID";
            public const string FREQUENCY_TYPE = "FREQUENCY_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class CP_COURSE_PATTERN
        {
            public const string COURSE_PATTERN_ID = "COURSE_PATTERN_ID";
            public const string COURSE_STRUCTURE_ID = "COURSE_STRUCTURE_ID";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string IS_OPTIONAL = "IS_OPTIONAL";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CP_COURSE_STRUCTURE
        {
            public const string COURSE_STRUCTURE_ID = "COURSE_STRUCTURE_ID";
            public const string CREDIT = "CREDIT";
            public const string PART_ID = "PART_ID";
            public const string UGC_TYPE_ID = "UGC_TYPE_ID";
            public const string HRS_PER_WEEK = "HRS_PER_WEEK";
            public const string MARKS = "MARKS";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string FACULTY_ID = "FACULTY_ID";
            public const string PROGRAMME_LEVEL = "PROGRAMME_LEVEL";
            public const string PROGRAMME_YEAR = "PROGRAMME_YEAR";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_OPTIONAL = "IS_OPTIONAL";
            public const string IS_NME = "IS_NME";
            public const string IS_ALLIED = "IS_ALLIED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class CERTIFICATE_COURSE_ROOT
        {
            public const string COURSE_ROOT_ID = "COURSE_ROOT_ID";
            public const string COURSE_TITLE = "COURSE_TITLE";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string COURSE_ORDER = "COURSE_ORDER";
            public const string QUOTA = "QUOTA";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class CERTIFICATE_CLASS_COURSE
        {
            public const string CLASS_COURSE_ID = "CLASS_COURSE_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class CERTIFICATE_STUDENT_REGISTRATION
        {
            public const string REGISTRATION_ID = "REGISTRATION_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string TIME_REGISTERED = "TIME_REGISTERED";
            public const string SEMESTER = "SEMESTER";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CERTIFICATE_COURSE_SETTINGS
        {
            public const string SETTINGS_ID = "SETTINGS_ID";
            public const string SETTINGS_NAME = "SETTINGS_NAME";
            public const string CLASS_ID = "CLASS_ID";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ALLOWED = "IS_ALLOWED";
            public const string IS_DELETED = "IS_DELETED";
            public const string TEMP_ID = "TEMP_ID";
            public const string SEMESTER = "SEMESTER";
            public const string P_LEVEL = "P_LEVEL";
            public const string TIME_FROM = "TIME_FROM";
            public const string TIME_TO = "TIME_TO";
        }

        public static class TT_TIMETABLE
        {
            public const string Time_Table_Id = "Time_Table_Id";
            public const string Course_Id = "Course_Id";
            public const string Day_Order_Id = "Day_Order_Id";
            public const string Staff_Id = "Staff_Id";
            public const string Class_Id = "Class_Id";
            public const string Hour_Id = "Hour_Id";
            public const string Is_Active = "Is_Active";
            public const string Is_Deleted = "Is_Deleted";
            public const string Semester_Id = "Semester_Id";
            public const string Hour_Type = "Hour_Type";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }

        public static class STU_COURSE_REMARK
        {
            public const string REMARK_ID = "REMARK_ID";
            public const string CLASS_ID = "CLASS_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string REMARK = "REMARK";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SupRole
        {
            public const string SUPER_ADMIN = "1";
            public const string ADMIN = "2";
            public const string PRINCIPAL = "3";
            public const string STAFF = "4";
            public const string STUDENT = "5";
            public const string DEAN = "6";
            public const string HEAD_OF_THE_DEPARTMENT = "7";
            public const string VICE_PRINCIPAL = "8";
            public const string OFFICE_ASSISTANT = "9";
            public const string EXAM_INCHARGE = "10";
            public const string ASSISTANT_EXAM_INCHARGE = "11";
            public const string COE = "12";
            public const string SECRETARY = "18";
            public const string LIBRARYADMIN = "19";
            public const string ALUMINI = "17";
            public const string EXAM_EXAMINAR = "39";
            public const string MBA_STUDENT = "42";
        }

        public static class SUP_COURSE_WISE_HEAD
        {
            public const string COURSE_HEAD_ID = "COURSE_HEAD_ID";
            public const string HEAD = "HEAD";
            public const string AMOUNT = "AMOUNT";
            public const string IS_USED = "IS_USED";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_SETS
        {
            public const string SET_ID = "SET_ID";
            public const string SET_NAME = "SET_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_FEE_ROOT
        {
            public const string FEE_ROOT_ID = "FEE_ROOT_ID";
            public const string FEE_ROOT = "FEE_ROOT";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class STU_REPORT_FIELDS
        {
            public const string FIELD_ID = "FIELD_ID";
            public const string FIELD = "FIELD";
            public const string FIELD_ALIAS = "FIELD_ALIAS";
            public const string FIELD_TABLE = "FIELD_TABLE";
            public const string FEILD_ORDER = "FEILD_ORDER";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_HOSTEL
        {
            public const string HOSTEL_ID = "HOSTEL_ID";
            public const string HOSTEL_NAME = "HOSTEL_NAME";
            public const string HOSTEL_CODE = "HOSTEL_CODE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_QUIZ_QUESTION_TYPE
        {
            public const string QUESTION_TYPE_ID = "QUESTION_TYPE_ID";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }


        public static class FEE_MAIN_HEADS
        {
            public static string FEE_MAIN_HEAD_ID = "FEE_MAIN_HEAD_ID";
            public static string FREQUENCY_ID = "FREQUENCY_ID";
            public static string APPLICATION_TYPE_ID = "APPLICATION_TYPE_ID";
            public static string MAIN_HEAD_ID = "MAIN_HEAD_ID";
            public static string HEAD_ID = "HEAD_ID";
            public static string PROGRAMME_MODE = "PROGRAMME_MODE";
            public static string SHIFT = "SHIFT";
            public static string FEE_CATEGORY_ID = "FEE_CATEGORY_ID";
            public static string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public static string IS_DELETED = "IS_DELETED";
            public static string FEE_ROOT_ID = "FEE_ROOT_ID";
            public static string BANK_ACCOUNT_ID = "BANK_ACCOUNT_ID";
        }
        public static class GRIEVANCE_TYPE
        {
            public const string GRIEVANCE_TYPE_ID = "GRIEVANCE_TYPE_ID";
            public const string GRIEVANCE_TYPE_NAME = "GRIEVANCE_TYPE_NAME";
            public const string PREFIX_CODE = "PREFIX_CODE";
            public const string GRIEVANCE_TYPE_CODE = "GRIEVANCE_TYPE_CODE";
            public const string HAS_SUB = "HAS_SUB";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }

        public static class GRIEVANCEINFO
        {
            public const string GRIEVANCE_ID = "GRIEVANCE_ID";
            public const string USER_ID = "USER_ID";
            public const string GRIEVANCE_CODE = "GRIEVANCE_CODE";
            public const string GRIEVANCE_TYPE_ID = "GRIEVANCE_TYPE_ID";
            public const string GRIEVANCE_TITLE = "GRIEVANCE_TITLE";
            public const string GRIEVANCE_SUBJECT = "GRIEVANCE_SUBJECT";
            public const string GRIEVANCE_CONTENT = "GRIEVANCE_CONTENT";
            public const string SUB_DATE_TIME = "SUB_DATE_TIME";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string STATUS = "STATUS";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }

        public static class GRIEVANCE_MEMBERINFO
        {
            public const string G_MEMBER_ID = "G_MEMBER_ID";
            public const string GRIEVANCE_TYPE_ID = "GRIEVANCE_TYPE_ID";
            public const string STAFF_ID = "STAFF_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }
        public static class GRIEVANCE_REPORT
        {
            public const string START_DATE = "START_DATE";
            public const string MONTH = "MONTH";
            public const string DAY_ID = "DAY_ID";
            public const string END_DATE = "END_DATE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string SUB_DATE_TIME = "SUB_DATE_TIME";
            public const string STAFF_ID = "STAFF_ID";
            public const string GRIEVANCE_TYPE_ID = "GRIEVANCE_TYPE_ID";
            public const string GREIVANCE_TYPE_NAME = "GREIVANCE_TYPE_NAME";
            public const string STATUS = "STATUS";
            public const string CLASS_ID = "CLASS_ID";
        }


        public static class REPORT_FIELDS_FORCASTEWISE
        {
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string PROGRAMME_MODE = "PROGRAMME_MODE";
            public const string SHIFT = "SHIFT";
            public const string PROGRAMME = "PROGRAMME";
            public const string PROGRAMME_ID = "PROGRAMME_ID";
            public const string COMMUNITY_NAME = "COMMUNITY_NAME";
            public const string COMMUNITY = "COMMUNITY";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
        }

        public static class COURSEPATTERN_REPORT_FIELDS
        {
            public const string FACULTY_ID = "FACULTY_ID";
            public const string PROGRAMME_LEVEL = "PROGRAMME_LEVEL";
            public const string PROGRAMME_YEAR = "PROGRAMME_YEAR";
            public const string PROGRAMME_GROUP_ID = "PROGRAMME_GROUP_ID";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }


        public static class SUP_SESSION
        {
            public const string SESSION_ID = "SESSION_ID";
            public const string SESSION_NAME = "SESSION_NAME";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SESSION
        {
            public const string FN = "1";
            public const string AF = "2";

        }

        public static class SUP_COLLEGE_BLOCK
        {
            public const string COLLEGE_BLOCK_ID = "COLLEGE_BLOCK_ID";
            public const string BLOCK_NAME = "BLOCK_NAME";
            public const string BLOCK_CODE = "BLOCK_CODE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_STAFF_TYPE
        {
            public const string STAFF_TYPE = "STAFF_TYPE";
            public const string STAFF_TYPE_NAME = "STAFF_TYPE_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }


        public static class SUP_EXAM_TYPE
        {
            public const string EXAM_TYPE = "EXAM_TYPE";
            public const string EXAM_TYPE_NAME = "EXAM_TYPE_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SupExamType
        {
            public const string Internal = "1";
            public const string External = "2";
        }
        public static class SUP_EXAM_SESSION
        {
            public const string EXAM_SESSION_ID = "EXAM_SESSION_ID";
            public const string EXAM_SESSION = "EXAM_SESSION";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class GRIEVANCEMEMBERSINFO
        {

            public const string G_MEMBER_ID = "G_MEMBER_ID";
            public const string GRIEVANCE_TYPE_ID = "GRIEVANCE_TYPE_ID";
            public const string GRIEVANCE_TYPE_NAME = "GRIEVANCE_TYPE_NAME";
            public const string STAFF_ID = "STAFF_ID";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string ROLE = "ROLE";
            public const string ROLE_NAME = "ROLE_NAME";
            public const string GRIEVANCE_ID = "GRIEVANCE_ID";
            public const string OLDROLE = "OLDROLE";
        }


        public static class LI_BOOK_DETAILS
        {
            public const string BOOK_ID = "BOOK_ID";
            public const string SUBJECT_ID = "SUBJECT_ID";
            public const string RACK_ID = "RACK_ID";
            public const string ACCESS_NO = "ACCESS_NO";
            public const string CLASS_NO = "CLASS_NO";
            public const string BOOK_NO = "BOOK_NO";
            public const string LANGUAGE = "LANGUAGE";
            public const string PERSONAL_AUTHOR1 = "PERSONAL_AUTHOR1";
            public const string PERSONAL_AUTHOR2 = "PERSONAL_AUTHOR2";
            public const string CORPORATE_AUTHOR1 = "CORPORATE_AUTHOR1";
            public const string CORPORATE_AUTHOR2 = "CORPORATE_AUTHOR2";
            public const string BOOK_TITLE = "BOOK_TITLE";
            public const string PARALLEL_TITLE = "PARALLEL_TITLE";
            public const string NOTES = "NOTES";
            public const string ILLUSTRATE = "ILLUSTRATE";
            public const string STATUS = "STATUS";
            public const string PRICE = "PRICE";
            public const string ISBN_NO = "ISBN_NO";
            public const string DISCODE = "DISCODE";
            public const string TAPS = "TAPS";
            public const string DETAILS = "DETAILS";
            public const string VC = "VC";
            public const string GAP_ID = "GAP_ID";
            public const string PUBLISHER = "PUBLISHER";
            public const string PLACE = "PLACE";
            public const string SERIESCODE = "SERIESCODE";
            public const string SERIES = "SERIES";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string RFID = "RFID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string ABSTRACT = "ABSTRACT";
            public const string DOC_TITLE = "DOC_TITLE";
            public const string NAME_OF_PUBLISHER = "NAME_OF_PUBLISHER";
            public const string NAME_IN_NATIVE = "NAME_IN_NATIVE";
        }

        public static class LI_USER_DETAILS
        {
            public const string USER_ACCOUNT_ID = "USER_ACCOUNT_ID";
            public const string USERNAME = "USERNAME";
            public const string PASSWORD = "PASSWORD";
            public const string ROLE = "ROLE";
            public const string BOID = "BOID";
            public const string NAME = "NAME";
            public const string EMAIL = "EMAIL";
            public const string MOBILE = "MOBILE";
            public const string LAST_LOGIN = "LAST_LOGIN";
            public const string DATE_OF_BIRTH = "DATE_OF_BIRTH";
            public const string MEMBER_TYPE = "MEMBER_TYPE";
            public const string DEGREE = "DEGREE";
            public const string ADDRESS = "ADDRESS";
            public const string BLOOD_GROUP_ID = "BLOOD_GROUP_ID";
            public const string REGISTERED_DATE = "REGISTERED_DATE";
            public const string EXPIRED_DATE = "EXPIRED_DATE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string PHOTO = "PHOTO";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string RFID = "RFID";
        }

        public static class LI_LOGIN_DETAILS
        {
            public const string LOGIN_ID = "LOGIN_ID";
            public const string USER_ID = "USER_ID";
            public const string LOGIN_DATE = "LOGIN_DATE";
            public const string EXIT_DATE = "EXIT_DATE";
            public const string NO_OF_ATTEMPT = "NO_OF_ATTEMPT";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class LI_RULES
        {
            public const string RULE_ID = "RULE_ID";
            public const string MEM_CATEGORY = "MEM_CATEGORY";
            public const string DESCRIPTON_CATEGORY = "DESCRIPTON_CATEGORY";
            public const string MAX_BOOKS = "MAX_BOOKS";
            public const string LOAN_PERIOD = "LOAN_PERIOD";
            public const string FINE_AMOUNT = "FINE_AMOUNT";
            public const string DEPOSIT = "DEPOSIT";
            public const string AUTORENEVAL = "AUTORENEVAL";
            public const string EXTRA_BOOKS = "EXTRA_BOOKS";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class LI_DUES
        {
            public const string DUES_ID = "DUES_ID";
            public const string USER_ID = "USER_ID";
            public const string BOOK_ID = "BOOK_ID";
            public const string REASON = "REASON";
            public const string FINE_AMOUNT = "FINE_AMOUNT";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_PAID = "IS_PAID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class LI_BOOK_ISSUE
        {
            public const string ISSUE_ID = "ISSUE_ID";
            public const string USER_ID = "USER_ID";
            public const string BOOK_ID = "BOOK_ID";
            public const string ISSUE_DATE = "ISSUE_DATE";
            public const string ISSUE_BY = "ISSUE_BY";
            public const string RETURN_DATE = "RETURN_DATE";
            public const string RECEIVED_BY = "RECEIVED_BY";
            public const string STATUS = "STATUS";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class LI_BOOK_RETURN
        {
            public const string RETURN_ID = "RETURN_ID";
            public const string ISSUE_ID = "ISSUE_ID";
            public const string RETURN_DATE = "RETURN_DATE";
            public const string RECEIVED_BY = "RECEIVED_BY";
            public const string STATUS = "STATUS";
            public const string FINE_AMOUNT = "FINE_AMOUNT";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class LI_BOOK_RESERVE
        {
            public const string RESERVE_ID = "RESERVE_ID";
            public const string BOOK_ID = "BOOK_ID";
            public const string RESERVE_DATE = "RESERVE_DATE";
            public const string USER_ID = "USER_ID";
            public const string REASON = "REASON";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class LI_MEMBER_RECALL
        {
            public const string RECALL_ID = "RECALL_ID";
            public const string USER_ID = "USER_ID";
            public const string REASON = "REASON";
            public const string MESSAGE_CONTENT = "MESSAGE_CONTENT";
            public const string SENT_DATE = "SENT_DATE";
            public const string MESSAGE_TYPE = "MESSAGE_TYPE";
            public const string STATUS = "STATUS";
            public const string BOOK_ID = "BOOK_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_MEMBER_CATEGORY
        {
            public const string CATEGORY_ID = "CATEGORY_ID";
            public const string CATEGORY_CODE = "CATEGORY_CODE";
            public const string CATEGORY_NAME = "CATEGORY_NAME";
            public const string CATEGORY_ALIAS = "CATEGORY_ALIAS";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }





        public static class SUP_LI_ILLUSTRATION
        {
            public const string ILLUSTRATE_ID = "ILLUSTRATE_ID";
            public const string ILLUSTRATE_CODE = "ILLUSTRATE_CODE";
            public const string ILLUSTRATE = "ILLUSTRATE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_AUTOCLASS
        {
            public const string AUTO_CLASS_ID = "AUTO_CLASS_ID";
            public const string AUTO_CLASS = "AUTO_CLASS";

        }
        public static class SUP_LI_ACCOUNT
        {
            public const string ACCOUNT_ID = "ACCOUNT_ID";
            public const string ACCOUNT_CODE = "ACCOUNT_CODE";
            public const string ACCOUNT_NAME = "ACCOUNT_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_CLASS
        {
            public const string CLASS_ID = "CLASS_ID";
            public const string CLASS_CODE = "CLASS_CODE";
            public const string CLASS_NAME = "CLASS_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_COURSE
        {
            public const string COURSE_ID = "COURSE_ID";
            public const string COURSE_CODE = "COURSE_CODE";
            public const string COURSE_NAME = "COURSE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_DISCIPLINE
        {
            public const string DISCIPLINE_ID = "DISCIPLINE_ID";
            public const string DISCIPLINE_CODE = "DISCIPLINE_CODE";
            public const string DISCIPLINE_NAME = "DISCIPLINE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_KEYWORDS
        {
            public const string KEYWORD_ID = "KEYWORD_ID";
            public const string KEYWORD = "KEYWORD";
            public const string KEY_CODE = "KEY_CODE";

        }
        public static class SUP_LI_CLASS_NO
        {
            public const string CLASS_NO_ID = "CLASS_NO_ID";
            public const string CLASS_NO = "CLASS_NO";
        }


        public static class SUP_LI_LIB_CODE
        {
            public const string LIB_CODE_ID = "LIB_CODE_ID";
            public const string LIB_DESCRIPTION = "LIB_DESCRIPTION";
            public const string LIB_CODE = "LIB_CODE";

        }



        public static class SUP_LI_DOCUMENT_TYPE
        {
            public const string DOCUMENT_TYPE_ID = "DOCUMENT_TYPE_ID";
            public const string DOCUMENT_TYPE = "DOCUMENT_TYPE";
            public const string DOCUMENT_TYPE_CODE = "DOCUMENT_TYPE_CODE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_HEAD
        {
            public const string HEAD_ID = "HEAD_ID";
            public const string HEAD_CODE = "HEAD_CODE";
            public const string HEAD = "HEAD";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_INTERLIBRARY
        {
            public const string LIBRARY_ID = "LIBRARY_ID";
            public const string LIBRARY_CODE = "LIBRARY_CODE";
            public const string LIBRARY_NAME = "LIBRARY_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_TAPS
        {
            public const string TAPS_ID = "TAPS_ID";
            public const string PREFIX = "PREFIX";
            public const string TAPS_NAME = "TAPS_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_ARTICLE_TYPE
        {
            public const string ARTICLE_TYPE_ID = "ARTICLE_TYPE_ID";
            public const string ARTICLE_TYPE_NAME = "ARTICLE_TYPE_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class SUP_LI_SERIES
        {
            public const string SERIES_ID = "SERIES_ID";
            public const string SERIES_CODE = "SERIES_CODE";
            public const string SERIES_NAME = "SERIES_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class LI_MEMBER_CATEGORY
        {
            public const string TEACHING_STAFF = "1";
            public const string NON_TEACHING_STAFF = "2";
            public const string STUDENT = "3";
            public const string EXTERNAL_USER = "4";
            public const string OTHERS = "5";
        }
        //public static class USER_TYPE
        //{
        //    public const string STUDENT = "5";
        //    public const string STAFF = "4";

        //}
        public static class USER_TYPE
        {
            public const string SUPER_ADMIN = "1";
            public const string ADMIN = "2";
            public const string PRINCIPAL = "3";
            public const string TEACHING_STAFF = "4";
            public const string STUDENT = "5";
            public const string DEAN = "6";
            public const string HEAD_OF_THE_DEPARTMENT = "7";
            public const string VICE_PRINCIPAL = "8";
            public const string OFFICE_ASSISTANT = "9";
            public const string EXAM_INCHARGE = "10";
            public const string ASSISTANT_EXAM_INCHARGE = "11";
            public const string COE = "12";
            public const string MESS_ADMIN = "13";
            public const string EXTERNAL_USER = "14";
            public const string EXTERNAL_USER_FOR_LIBRARY = "15";
            public const string NON_TEACHING_STAFF = "16";
        }
        public static class LI_SERVICE_TIME
        {
            public const string DUES = "1";
            public const string RESERVE_CANCEL = "2";
            public const string FINE = "3";
            public const string SYSTEM_CHECKOUT = "4";
        }


        public static class SUP_LI_STATUS
        {
            public static string STATUS_ID = "STATUS_ID";
            public static string STATUS_NAME = "STATUS_NAME";
            public static string STATUS_CODE = "STATUS_CODE";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_BOOK_STATUS
        {
            public static string STATUS_ID = "STATUS_ID";
            public static string STATUS_NAME = "STATUS_NAME";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }


        public static class LI_AUTHOR_INFO
        {
            public const string AUTHOR_ID = "AUTHOR_ID ";
            public const string FIRST_NAME = "FIRST_NAME";
            public const string LAST_NAME = "LAST_NAME ";
            public const string PUBLISHER = "PUBLISHER ";
            public const string PLACE = "PLACE";
            public const string ENTRY_ID = "ENTRY_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE ";
            public const string IS_DELETED = "IS_DELETED";
        }


        public static class SUP_FBFEEDBACK_QUESTION_TYPE
        {
            public const string QUESTION_TYPE_ID = "QUESTION_TYPE_ID";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";

        }

        public static class SUP_FBFEEDBACK_ROLE
        {
            public const string SUP_FEEDBACK_ROLE_ID = "SUP_FEEDBACK_ROLE_ID";
            public const string FEEDBACK_ROLE_NAME = "FEEDBACK_ROLE_NAME";
            public const string ASSESSOR_ROLE = "ASSESSOR_ROLE";
            public const string ASSESSEE_ROLE = "ASSESSEE_ROLE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }


        public static class FBFEEDBACK_ROLE_SETTINGS
        {
            public const string FEEDBACK_ROLE_ID = "FEEDBACK_ROLE_ID";
            public const string FEEDBACK_NAME = "FEEDBACK_NAME";
            public const string FEEDBACK_ROLE_NAME = "FEEDBACK_ROLE_NAME";
            public const string FEEDBACK_ID = "FEEDBACK_ID";
            public const string SUP_FEEDBACK_ROLE_ID = "SUP_FEEDBACK_ROLE_ID";
            public const string IS_ATTENDED = "IS_ATTENDED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class FBFEEDBACK_ROLE
        {
            public const string VICEPRINCIPAL_BY_PRINCIPAL = "1";
            public const string HOD_BY_PRINCIPAL = "2";
            public const string STAFF_BY_PRINCIPAL = "3";
            public const string STAFF_BY_STUDENT = "4";
            public const string STAFF_INCHARGE_BY_STUDENT = "19";
            public const string HOD_BY_STAFF = "5";
            public const string PRINCIPAL_BY_STAFF = "6";
            public const string VICEPRINCIPAL_BY_STAFF = "7";
            public const string STAFF_BY_DEEN = "8";
            public const string STAFF_BY_HOD = "9";
            public const string STAFF_BY_PEER = "10";
            public const string DEEN_BY_STAFF = "12";
            public const string PRINCIPAL_BY_PEER = "13";
            public const string HOD_BY_PEER = "14";
            public const string FACULTY_BY_PEER = "15";
            public const string DEEN_BY_PEER = "16";
            public const string STAFF_BY_COE = "21";
            public const string STAFF_BY_MANAGER = "20";
            public const string STAFF_SELF_ENTRY = "17";
            public const string STUDENT_SELF_ENTRY = "18";
            public const string CLASS_WISE_FEEDBACK_FOR_STUDENTS = "22";


        }
        public static class IQAC_QUSTIONS
        {
            public const string TEACHING_SUBJECTS = "1";
            public const string TEACHING_LEARNING = "2";
            public const string CO_CURRICULAR_ASPECTS = "3";
            public const string EXTENSION_WORKS = "4";
            public const string CURRICULAR_ASPECTS = "5";
            public const string LESSON_PLAN = "6";
            public const string AWARDS = "7";
            public const string SUBJECT_ASSOCIATION = "8";
            public const string HONOURS = "9";
            public const string MEMBERSHIP = "10";
            public const string PROJECT = "11";
            public const string RESEARCH_GUIDENCE = "12";
            public const string PHD_GUIDENCE = "13";
            public const string PUBLICATION_AND_PRASENTATION = "14";
            public const string PAPER_PUBLICATION = "15";
            public const string BOOK_PUBLICATION = "16";
            public const string DEPARTMENT_DAILY_ACTIVITY = "17";
            public const string ASSOCIATION_ACTIVITIES = "18";
            public const string SEMINAR_WORKSHOP_ORGANIZED = "19";
        }

        public static class SUP_COMPETITIVE_EXAM
        {
            public const string COMPETITIVE_EXAM_ID = "COMPETITIVE_EXAM_ID";
            public const string COMPETITIVE_EXAM = "COMPETITIVE_EXAM";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class ALU_CONTRIBUTION_DETAILS
        {
            public const string ALU_CONTIBUTION_ID = "ALU_CONTIBUTION_ID";
            public const string ALU_REGISTER_ID = "ALU_REGISTER_ID";
            public const string VISIT_OUR_CAMPUS = "VISIT_OUR_CAMPUS";
            public const string SUGGESTION = "SUGGESTION";
            public const string EXPERIENCE = "EXPERIENCE";
            public const string CHERISHED_MEMORY = "CHERISHED_MEMORY";
            public const string CONTRIBUTION_TYPE_ID = "CONTRIBUTION_TYPE_ID";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SubClassYear
        {
            public const string CLASS_YEAR_ID = "CLASS_YEAR_ID";
            public const string CLASS_YEAR = "CLASS_YEAR";
        }

        public static class EXAM_REVALUATION_SETTING
        {
            public const string EXAM_REVALUATION_SETTING_ID = "EXAM_REVALUATION_SETTING_ID";
            public const string EXAM_SETTING_ID = "EXAM_SETTING_ID";
            public const string EXAM_SETTING = "EXAM_SETTING";
            public const string APPLICATION_TYPE_ID = "APPLICATION_TYPE_ID";
            public const string APPLICATION_TYPE = "APPLICATION_TYPE";
            public const string MAX_PAPER = "MAX_PAPER";
            public const string START_DATE = "START_DATE";
            public const string END_DATE = "END_DATE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }
        public static class SMS_TEMPLATE
        {
            public const string HostelSelection = "1";
            public const string DailyAbsenteesSMS = "2";
            public const string LateComers = "3";
            public const string FeePaidSMS = "4";
            public const string HallReservationSMS = "3";
            public const string VisitorAppointmentSMS = "4";
            public const string FeeStructureSMS = "4";
            public const string OTP = "10";
            public const string MARK_SUBMISSION_OTP = "26";
        }

        public static class EXAM_PHOTOCOPY_SETTING
        {
            public const string EXAM_PHOTOCOPY_SETTING_ID = "EXAM_PHOTOCOPY_SETTING_ID";
            public const string EXAM_SETTING_ID = "EXAM_SETTING_ID";
            public const string EXAM_SETTING = "EXAM_SETTING";
            public const string APPLICATION_TYPE_ID = "APPLICATION_TYPE_ID";
            public const string APPLICATION_TYPE = "APPLICATION_TYPE";
            public const string MAX_PAPER = "MAX_PAPER";
            public const string START_DATE = "START_DATE";
            public const string END_DATE = "END_DATE";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
        }

        public static class SUP_ALU_SCHOLORSHIP_TYPE
        {
            public const string ALU_SCHOLORSHIP_TYPE_ID = "ALU_SCHOLORSHIP_TYPE_ID";
            public const string SCHOLORSHIP_NAME = "SCHOLORSHIP_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class SUP_ALU_SCHOLORSHIP_SCHEME
        {
            public const string ALU_SCHOLORSHIP_SCHEME_ID = "ALU_SCHOLORSHIP_SCHEME_ID";
            public const string SCHEME = "SCHEME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_FBFACULTY_SELF_APPRAISAL_TYPE
        {
            public const string FACULTY_SELF_APPRAISAL_TYPE_ID = "FACULTY_SELF_APPRAISAL_TYPE_ID";
            public const string APPRAISAL_TYPE = "APPRAISAL_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_IQAC_PROJECT_TYPE
        {
            public const string PROJECT_TYPE_ID = "PROJECT_TYPE_ID";
            public const string PROJECT_TYPE = "PROJECT_TYPE";

        }
        public static class SUP_IQAC_TYPE_OF_FUNDING
        {
            public const string TYPE_FUNDING_ID = "TYPE_FUNDING_ID";
            public const string FUNDING_TYPE = "FUNDING_TYPE";

        }
        public static class SUP_IQAC_PHD_RESEARCH_CATEGORY
        {
            public const string IQAC_CATEGORY_ID = "IQAC_CATEGORY_ID";
            public const string CATEGORY_NAME = "CATEGORY_NAME";

        }
        public static class SUP_IQAC_PHD_GUIDENCE_CATEGORY
        {
            public const string IQAC_CATEGORY_ID = "IQAC_CATEGORY_ID";
            public const string CATEGORY_NAME = "CATEGORY_NAME";

        }
        public static class SUP_IQAC_STATUS
        {
            public const string IQAC_STATUS_ID = "IQAC_STATUS_ID";
            public const string STATUS_NAME = "STATUS_NAME";
        }

        public static class ASSOCIATION_GROUPS
        {
            public const string ASSOCICATION_GROUP_ID = "ASSOCICATION_GROUP_ID";
            public const string ASSOCIATION_GROUP = "ASSOCIATION_GROUP";
            public const string ASSOCIATION_GROUP_CODE = "ASSOCIATION_GROUP_CODE";
            public const string ASSOCIATION_GROUP_ORDER = "ASSOCIATION_GROUP_ORDER";
            public const string QUOTA = "QUOTA";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class FB_RESULT
        {
            public const string SATISFACTORY = "SATISFACTORY";
            public const string GOOD = "GOOD";
            public const string VERY_GOOD = "VERY GOOD";
            public const string EXCELLENT = "EXCELLENT";
            public const string OUTSTANDING = "OUTSTANDING";
        }

        public static class SUP_NOTIFICATION_LEVEL
        {
            public const string NOTIFICATION_LEVEL_ID = "NOTIFICATION_LEVEL_ID";
            public const string NOTIFICATION_LEVEL = "NOTIFICATION_LEVEL";

        }

        public static class NOTITFICATION_LEVEL
        {
            public const string USER_LEVEL = "1";
            public const string CLASS_LEVEL = "2";
            public const string PROGRAMME_LEVEL = "3";
            public const string COLLEGE_LEVEL = "4";
            public const string APPLICATION_LEVEL = "5";
        }

        public static class CERTIFICATE_COURSE_SETTING
        {
            public const string SETTING_ID = "SETTING_ID";
            public const string SETTING_NAME = "SETTING_NAME";
            public const string DATE_FROM = "DATE_FROM";
            public const string DATE_TO = "DATE_TO";
            public const string SEMESTER_ID = "SEMESTER_ID";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
            public const string SEMESTER_NAME = "SEMESTER_NAME";
        }

        public static class SUP_IQAC_GUEST
        {
            public const string GUEST_ID = "GUEST_ID";
            public const string GUEST_NAME = "GUEST_NAME";
        }

        public static class SUP_ORGANIZER
        {
            public const string ORGANIZER_ID = "ORGANIZER_ID";
            public const string ORGANIZER_NAME = "ORGANIZER_NAME";
        }
        public static class EXAM_EVALUATION_COURSE_SETTING
        {
            public const string EVALUATION_COURSE_SETTING_ID = "EVALUATION_COURSE_SETTING_ID";
            public const string EXAM_SETTING_ID = "EXAM_SETTING_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string EVALUATION_DATE = "EVALUATION_DATE";
            public const string IS_EXTERNAL_INTERNAL = "IS_EXTERNAL_INTERNAL";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class EXAM_EVALUATION_STUDENT_MARK
        {
            public const string EXAM_EV_STU_MARK_ID = "EXAM_EV_STU_MARK_ID";
            public const string EVALUATION_COURSE_SETTING_ID = "EVALUATION_COURSE_SETTING_ID";
            public const string STUDENT_ID = "STUDENT_ID";
            public const string EXAM_EVALUATOR_STAFF_ID = "EXAM_EVALUATOR_STAFF_ID";
            public const string COURSE_ID = "COURSE_ID";
            public const string EVALUATION_1 = "EVALUATION_1";
            public const string EVALUATION_2 = "EVALUATION_2";
            public const string EVALUATION_3 = "EVALUATION_3";
            public const string TOTAL = "TOTAL";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string ACDEMIC_YEAR = "ACDEMIC_YEAR";
            public const string EVALUATION = "EVALUATION";
        }
        public static class EXAM_EVALUATOR_STAFF_INFO
        {
            public const string EXAM_EVALUATOR_STAFF_ID = "EXAM_EVALUATOR_STAFF_ID";
            public const string STAFF_CODE = "STAFF_CODE";
            public const string NAME = "NAME";
            public const string PHONE = "PHONE";
            public const string USERNAME = "USERNAME";
            public const string PASSWORD = "PASSWORD";
            public const string ROLE = "ROLE";
            public const string STAFF_ID = "STAFF_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_RACKS
        {
            public const string RACK_ID = "RACK_ID";
            public const string RACK_NAME = "RACK_NAME";

        }
        public static class SUP_LI_CELLS
        {
            public const string CELL_ID = "CELL_ID";
            public const string CELL_NAME = "CELL_NAME";

        }
        public static class PAPER_TYPE
        {
            public const string Language = "1";
            public const string English = "2";
            public const string Main = "3";
            public const string Allied = "4";
            public const string SBE = "5";
            public const string Value_Education = "6";
            public const string NME = "7";
            public const string Practical = "8";
            public const string EVS = "9";
            public const string Extra_Credit = "10";
            public const string Allied_5 = "11";
            public const string Allied_6 = "12";
            public const string Skill_Based_Elective_5 = "13";
            public const string Skill_Based_Elective_6 = "14";
            public const string Allied_5_Optional = "15";
            public const string Allied_5_Compulsory = "16";
            public const string Project = "17";
            public const string Self_Study = "19";
            public const string RESCAPES = "20";
            public const string Extension_Activities = "21";
            public const string Major_Elective_3 = "22";
        }
        public static class SUBJECT_TYPE
        {
            public const string THEORY = "1";
            public const string PRACTICAL = "2";
            public const string PROJECT = "3";
            public const string FIELD_WORK = "4";
            public const string INTERNSHIP = "5";
            public const string PORTFOLIO = "6";
            public const string RESCAPES = "7";
            public const string DISSERTATION = "8";
            public const string EXTRA_CREDIT = "9";
            public const string SPECIAL_THEORY = "10";
        }
        public static class CLASS_TYPE
        {
            public const string C_Combined = "1";
            public const string R_Regular = "2";
            public const string S_Selected = "3";
        }

        public static class SUP_EXTERNAL_CALCULATION
        {
            public const string EXTERNAL_CALCULATION_ID = "EXTERNAL_CALCULATION_ID";
            public const string EXTERNAL_CALCULATION = "EXTERNAL_CALCULATION";
            public const string IS_DELETED = "IS_DELETED";
            public const string PROGRAMME_LEVEL_ID = "PROGRAMME_LEVEL_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";

        }

        public static class LI_RACK_INFO
        {
            public const string RACK_INFO_ID = "RACK_INFO_ID";
            public const string RACK_INFO = "RACK_INFO";
            public const string RACK_ID = "RACK_ID";
            public const string CELL_ID = "CELL_ID";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";

        }

        public static class SUP_LI_BOOK_INVOICE
        {
            public static string INVOICE_ID = "INVOICE_ID";
            public static string INVOICE_NO = "INVOICE_NO";
            public static string VENDOR = "VENDOR";
            public static string NO_OF_BOOK = "NO_OF_BOOK";
            public static string PLACE = "PLACE";
            public static string DATE_OF_PURCHASE = "DATE_OF_PURCHASE";
            public static string DATE_OF_ENTRY = "DATE_OF_ENTRY";
            public static string IS_DELETED = "IS_DELETED";
            public static string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class SUP_LI_DOC_CATEGORY
        {
            public static string DOC_CATEGORY_ID = "DOC_CATEGORY_ID";
            public static string DOC_CATEGORY_CODE = "DOC_CATEGORY_CODE";
            public static string DOC_CATEGORY = "DOC_CATEGORY";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_LI_BOOK_TYPE
        {
            public static string TYPE_ID = "TYPE_ID";
            public static string PREFIX = "PREFIX";
            public static string BOOK_TYPE = "BOOK_TYPE";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }
        public static class OBE_QUESTION_LEVEL
        {
            public static string QUESTION_LEVEL_ID = "QUESTION_LEVEL_ID";
            public static string QUESTION_LEVEL = "QUESTION_LEVEL";
            public static string OBE_QUESTIONLEVEL_ID = "OBE_QUESTIONLEVEL_ID";
            public static string OBE_QUESTIONLEVEL = "OBE_QUESTIONLEVEL";
            public static string OBE_COMPONENT_ID = "OBE_COMPONENT_ID";
            public static string OBE_COMPONENT = "OBE_COMPONENT";
            public static string OBE_RBT_LEVEL_ID = "OBE_RBT_LEVEL_ID";
            public static string OBE_RBT_LEVEL = "OBE_RBT_LEVEL";

        }
      
        public static class SUP_LI_SEARCH_TEXT
        {
            public static string SEARCH_TEXT_ID = "SEARCH_TEXT_ID";
            public static string FIELD_NAME = "FIELD_NAME";
            public static string SEARCH_TEXT = "SEARCH_TEXT";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LI_BOOK_COLLECTION
        {
            public static string COLLECTION_ID = "SEARCH_TEXT_ID";
            public static string COLLECTION_NAME = "FIELD_NAME";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LEAVE_TYPE
        {
            public static string LEAVE_TYPE_ID = "LEAVE_TYPE_ID";
            public static string LEAVE_TYPE = "LEAVE_TYPE";

            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_LEAVE_STATUS
        {
            public static string LEAVE_STATUS_ID = "LEAVE_STATUS_ID";
            public static string LEAVE_STATUS = "LEAVE_STATUS";

            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }
        public static class LEAVE_STATUS
        {
            public static string Approved = "1";
            public static string Rejected = "2";

        }
        public static class INCREMENT_STATUS
        {
            public static string Approved = "1";
            public static string Rejected = "2";
        }

        public static class EVENT_REGISTRATION_SETTINGS
        {
            public static string EVENT_SETTING_ID = "EVENT_SETTING_ID";
            public static string SUB_EVENT_ID = "SUB_EVENT_ID";
            public static string MAIN_EVENT_ID = "MAIN_EVENT_ID";
            public static string SUB_EVENT_NAME = "SUB_EVENT_NAME";
            public static string MAIN_EVENT_NAME = "MAIN_EVENT_NAME";
        }
        public static class SUP_HALLS
        {
            public static string HALL_ID = "HALL_ID";
            public static string HALL_NAME = "HALL_NAME";

        }
        public static class SUP_HALL_RESERVATION_STATUS
        {
            public static string STATUS_ID = "STATUS_ID";
            public static string STATUS_NAME = "STATUS_NAME";

        }
        public static class HALL_RESERVATION_STATUS
        {
            public static string PENNDING = "PENNDING";
            public static string APPROVED = "APPROVED";
            public static string REJECTED = "REJECTED";

        }
        public static class HALL_RESERVATION
        {

            public static string HALL_NAME = "HALL_NAME";
            public static string FROM_DATE = "FROM_DATE";
            public static string TO_DATE = "TO_DATE";
            public static string FROM_TIME = "FROM_TIME";
            public static string TO_TIME = "TO_TIME";
            public static string APPLY_DATE = "APPLY_DATE";
            public static string APPROVAL_DATE = "APPROVAL_DATE";

            public static string USER_NAME = "USER_NAME";

            public static string STATUS_NAME = "STATUS_NAME";
        }
        public static class VISITORS_MEETING_APPOINTMENT
        {
            public static string APPOINTMENT_DATE = "APPOINTMENT_DATE";
            public static string APPOINTMENT_TIME = "APPOINTMENT_TIME";
            public static string STATUS_NAME = "STATUS_NAME";

        }

        public static class SUP_VISITORS_CATEGORY
        {
            public static string VISITORS_CATEGORY_ID = "VISITORS_CATEGORY_ID";
            public static string VISITORS_CATEGORY_NAME = "VISITORS_CATEGORY_NAME";
        }
        public static class INVENT_ITEMS
        {
            public static string ITEM_ID = "ITEM_ID";
            public static string ITEM_NAME = "ITEM_NAME";
        }
        public static class INVENT_VENDOR_DETAILS
        {
            public static string VENDOR_ID = "VENDOR_ID";
            public static string VENDOR_NAME = "VENDOR_NAME";

        }
        public static class INVENT_DEPARTMENT
        {
            public static string DEPARTMENT_ID = "DEPARTMENT_ID";
            public static string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        }



        public static class SUP_SUPERVISON_STAFF_TYPE
        {
            public static string SUP_SUPERVISION_TYPE_ID = "SUP_SUPERVISION_TYPE_ID";
            public static string SUPERVISION_STAFF_TYPE = "SUPERVISION_STAFF_TYPE";
            public static string IS_DELETED = "IS_DELETED";
            public static string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class EXAM_SEATING_INVIGILATOR
        {
            public static string SEATING_INVIGILATOR_ID = "SEATING_INVIGILATOR_ID";
            public static string STAFF_ID = "STAFF_ID";
            public static string EXAM_DATE = "EXAM_DATE";
            public static string EXAM_SESSION_ID = "EXAM_SESSION_ID";
            public static string EXAM_SETTING_ID = "EXAM_SETTING_ID";
            public static string EXAM_TYPE = "EXAM_TYPE";
            public static string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public static string IS_DELETED = "IS_DELETED";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string INDIVIDUAL_HOUR = "INDIVIDUAL_HOUR";

        }
        public static class PR_SALARY_COMPONENT_DETAILS
        {
            public static string COMPONENT_ID = "COMPONENT_ID";

            public static string COMPONENT_NAME = "COMPONENT_NAME";
            public static string EMPLOYEE_ID = "EMPLOYEE_ID";
            public static string FIRST_NAME = "FIRST_NAME";
            public static string PF_COMPONENT_NAME = "PF_COMPONENT_NAME";
            public static string PF_COMPONENT_ID = "PF_COMPONENT_ID";


        }
        public static class SUP_SALARY_COMPONENT_TYPE
        {
            public static string SALARY_COMPONENT_TYPE_ID = "SALARY_COMPONENT_TYPE_ID";
            public static string SALARY_COMPONENT_NAME = "SALARY_COMPONENT_NAME";
        }
        public static class PR_SALARY_TEMPLATE_DETAILS
        {
            public static string TEMPLATE_ID = "TEMPLATE_ID";
            public static string TEMPLATE_NAME = "TEMPLATE_NAME";
        }
        public static class PR_SUP_SALARY_INCREMENT_TYPE
        {
            public static string SALARY_INCREMENT_TYPE_ID = "SALARY_INCREMENT_TYPE_ID";
            public static string INCREMENT_TYPE_NAME = "INCREMENT_TYPE_NAME";
        }

        public static class STU_MANDATORY_FIELDS
        {
            public static string STU_MANDATORY_ID = "STU_MANDATORY_ID";
            public static string MANDATORY_FIELDS = "MANDATORY_FIELDS";
            public static string VISIBILE_NAME = "VISIBILE_NAME";
            public static string IS_DELETED = "IS_DELETED";
            public static string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SALARY_COMPONENT_TYPE
        {
            public static string Earnings = "1";
            public static string Deductions = "2";
        }
        public static class PR_LEAVE_SETTING
        {
            public static string STF_LEAVE_SETTING_ID = "STF_LEAVE_SETTING_ID";
            public static string LEAVE_TYPE = "LEAVE_TYPE";
        }
        public static class LM_ROLE_BASED_APPROVAL
        {
            public static string APPROVER_ROLE_ID = "APPROVER_ROLE_ID";
            public static string APPROVAL_ID = "APPROVAL_ID";

            public static string ROLE_ID = "ROLE_ID";

        }
        public static class PR_SUP_INCREMENT_STATUS
        {
            public static string STATUS_ID = "STATUS_ID";
            public static string STATUS_NAME = "STATUS_NAME";


        }
        public static class LM_SUP_APPROVAL_LEVEL
        {
            public static string LEVEL_ID = "LEVEL_ID";
            public static string LEVEL_NAME = "LEVEL_NAME";
        }

        public static class SUP_UNIT
        {
            public static string UNIT_ID = "UNIT_ID";
            public static string UNIT_NAME = "UNIT_NAME";
            public static string IS_DELETED = "IS_DELETED";
            public static string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class AutomaticTTMethods
        {
            public const string RegularandRegularArrears = "1";
            public const string RegularandRegularArrearsWithSeparatecode = "2";
            public const string RegularExtraSession = "3";
            public const string RegularExtraSessionandArrearsWithSeparatecode = "4";
            public const string AllRegularOneSession = "5";
            public const string RegularandRegularArrearsAndRegularSemesterArrear = "6";
        }
        public static class SUP_EVALUATION
        {
            public const string EVALUATION_ID = "EVALUATION_ID";
            public const string EVALUATION_NAME = "EVALUATION_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETE = "IS_DELETE";
        }

        public static class SUP_SECTION
        {
            public static string SECTION_ID = "SECTION_ID";
            public static string SECTION = "SECTION";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
            public static string SECTION_NAME = "SECTION_NAME";
       
        }
        public static class SUP_QUESTION_LEVEL
        {
            public static string QUESTION_LEVEL_ID = "QUESTION_LEVEL_ID";
            public static string QUESTION_LEVEL = "QUESTION_LEVEL";
            public static string IS_ACTIVE = "IS_ACTIVE";
            public static string IS_DELETED = "IS_DELETED";
        }
        public static class CP_EQUIVALENT_COURSE
        {
            public const string EQUIVALENT_COURSE_ID = "EQUIVALENT_COURSE_ID";
            public const string EQUIVALENT_COURSE = "EQUIVALENT_COURSE";
            public const string COURSE_ID = "COURSE_ID";
            public const string EQUIVALENT_TYPE = "EQUIVALENT_TYPE";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class SUP_EQUIVALENT_TYPE
        {
            public const string EquivalentCourse = "1";
            public const string ArrearCourse = "2";
        }
        public static class EXAM_QUESTIONS
        {
            public const string QUESTION_ID = "QUESTION_ID";
            public const string QUESTION = "QUESTION";
            public const string QUIZ_ID = "QUIZ_ID";

        }
        public static class SUP_STATUS
        {
            public const string STATUS_ID = "STATUS_ID";
            public const string STATUS_NAME = "STATUS_NAME";
            public const string DESCRIPTION = "DESCRIPTION";
        }


        public static class SUP_CERTIFICATE
        {
            public const string Bonafide = "131";
            public const string Attendance = "132";
            public const string Course = "133";
            public const string Conduct = "134";


        }

        public static class API_METHOD
        {
            public const string SINGLE_SMS = "1";
            public const string BULK_SMS = "2";
        }

        public static class sms_vendor
        {
            public static string Solutioninfini = "1";
            public static string DoveSoft = "2";

        }
        public static class CERTIFICATE_TYPE
            {
            public static string SEMESTER_MARK_SHEET = "1";
            public static string CONSOLIDATE_MARK_SHEET = "3";
        }
        public static class EXAM_QUESTION_PATTERN
        {
            public const string QUESTION_PATTERN_ID = "QUESTION_PATTERN_ID";
            public const string PATTERN_NAME = "PATTERN_NAME";
            public const string PATTERN_SECTION_ID = "PATTERN_SECTION_ID";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
        }

        public static class EXAM_EVALUATION
        {
            public const string EXAM_EVALUATION_ID = "EXAM_EVALUATION_ID";
            public const string INVIGILATION_ID = "INVIGILATION_ID";
            public const string EVALUATOR_ID = "EVALUATOR_ID";
            public const string EVALUATION_TYPE = "EVALUATION_TYPE";
            public const string EVALUATION_DATE = "EVALUATION_DATE";
            public const string DUMMY_NO = "DUMMY_NO";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class EXAM_SUP_EVALUATION_TYPE
        {
            public const string EXAM_EVALUATION_TYPE_ID = "EXAM_EVALUATION_TYPE_ID";
            public const string EXAM_EVALUATION = "EXAM_EVALUATION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
       

        public static class SUP_QUIZ_TYPE
        {
            public const string QUIZ_TYPE_ID = "QUIZ_TYPE_ID";
            public const string QUIZ_TYPE_NAME = "QUIZ_TYPE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }

        public static class EXAM_ONLINE_EXAM_SETTING
        {
            public const string IS_NEED_CAMERA = "IS_NEED_CAMERA";
            public const string IS_NEED_CHAT = "IS_NEED_CHAT";
            public const string IS_NEED_LOG = "IS_NEED_LOG";
            public const string IS_AUTO_EVALUATION = "IS_AUTO_EVALUATION";
            public const string IS_AUTO_START = "IS_AUTO_START";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }


        public static class MAJOR_WISE_ALLIED_COURSE
        {
            public const string MAJOR_ID = "MAJOR_ID";
            public const string MAJOR = "MAJOR";
            public const string ALLIED1 = "ALLIED1";
            public const string ALLIED2 = "ALLIED2";
        }

        public static class SUP_UNITS
        {

            public const string UNIT_ID = "UNIT_ID";
            public const string UNIT_NAME = "UNIT_NAME";

            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_METHODOLOGY
        {

            public const string METHODOLOGY_ID = "METHODOLOGY_ID";
            public const string METHODOLOGY_NAME = "METHODOLOGY_NAME";

            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_MESSAGE_TYPE
        {

            public const string MSG_TYPE_ID = "MSG_TYPE_ID";
            public const string MESSAGE_TYPE_NAME = "MESSAGE_TYPE_NAME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class OBE_CO_ATTAINMENT_COMPONENT
        {
            public const string OBE_COMPONENT_ID = "OBE_COMPONENT_ID";
            public const string OBE_COMPONENT = "OBE_COMPONENT";
            public const string COMPONENT_ID = "COMPONENT_ID";
        }

        public static class LMS_ASSIGNMENT_SETTING
        {
            public const string ASSIGNMENT_SETTING_ID = "ASSIGNMENT_SETTING_ID";
            public const string ASSIGNMENT_NAME = "ASSIGNMENT_NAME";
            public const string CLS_GROUP_ID = "CLS_GROUP_ID";
            public const string GROUP_NAME = "GROUP_NAME";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class SUP_LMS_UNIT
        {
            public const string LMS_UNIT_ID = "LMS_UNIT_ID";
            public const string UNIT = "UNIT";
            public const string IS_DELETED = "IS_DELETED";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }

        public static class EVENT_FBQUESTIONS
        {
            public const string QUESTION_ID = "QUESTION_ID";
            public const string EVENT_ID = "EVENT_ID";
            public const string EVENT_NAME = "EVENT_NAME";
            public const string QUESTION = "QUESTION";
            public const string TAMIL_QUESTION = "TAMIL_QUESTION";
            public const string QUESTION_TYPE = "QUESTION_TYPE";
            public const string QUESTION_GROUP = "QUESTION_GROUP";
            public const string ORDERED_BY = "ORDERED_BY";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string ROLE_ID = "ROLE_ID";
            public const string IS_DELETED = "IS_DELETED";
            
        }

        public static class AP_IMCOME
        {
            public const string INCOME_ID = "INCOME_ID";
            public const string INCOME = "INCOME";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_AP_STATUS
        {
            public const string AP_STATUS = "AP_STATUS";
            public const string STATUS = "STATUS";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class SUP_AP_COLLOBORATION
        {
            public const string COLLOBORATION_ID = "COLLOBORATION_ID";
            public const string COLLOBORATION = "COLLOBORATION";
            public const string IS_ACTIVE = "IS_ACTIVE";
            public const string IS_DELETED = "IS_DELETED";
        }
        public static class CON_SETTING
        {
            public const string SETTING_ID = "SETTING_ID";
            public const string SETTING_NAME = "SETTING_NAME";
            public const string ACADEMIC_YEAR = "ACADEMIC_YEAR";
            public const string DATE_FROM = "DATE_FROM";
            public const string BATCH = "BATCH";
            public const string TO_DATE = "TO_DATE";
            public const string IS_ACTIVE = "IS_ACTIVE";
        }
        public static class CF_TRANSACTION_TYPE
        {
            public static string PAYMENT_GATEWAY = "PAYMENT_GATEWAY";
            public static string PAYOUT = "PAYOUT";
            public static string TRANSFER = "TRANSFER";
            public static string SETTLEMENT = "SETTLEMENTS";
        }

        public static class CF_PAYMENT_GATEWAY_KEY
        {
            public static string ORDER = "ORDER";
            public static string PAYMENT = "PAYMENT";
            public static string PAYOUT = "PAYOUT";
            public static string TRANSFER = "TRANSFER";
            public static string SETTLEMENT = "SET";
        }
        public static class CF_API_URL
        {
            public static string SETTLEMENT_BY_ORDER = "settlements";
            public static string ORDER = "orders";
            public static string TRANSFER = "TRANSFER";
            public static string SETTLEMENT = "SET";
            public static string SECRETKEY = "LST@AUX";
        }
        #region Excel Export
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static HttpResponse ExportToExcel(DataTable dt, string fileName)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xls");
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            string tab = "";
            foreach (DataColumn dc in dt.Columns)
            {
                HttpContext.Current.Response.Write(tab + dc.ColumnName);
                tab = "\t";
            }
            HttpContext.Current.Response.Write("\n");
            int i;
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    HttpContext.Current.Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
                HttpContext.Current.Response.Write("\n");
            }
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            return HttpContext.Current.Response;
        }
        #endregion
    }
}
