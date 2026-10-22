using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace CMS.ViewModel.Model
{


    #region RazorPayModel
    public class Notes
    {
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }

    }


    public class OrderIdResponse
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string amount { get; set; }
        public string amount_paid { get; set; }
        public string amount_due { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public string offer_id { get; set; }
        public string status { get; set; }
        public string attempts { get; set; }
        public Notes notes = new Notes();
        public string created_at { get; set; }
        public string key { get; set; }
        public string secret { get; set; }
    }

    public class PaymentResponse
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public string invoice_id { get; set; }
        public string international { get; set; }
        public string method { get; set; }
        public string amount_refunded { get; set; }
        public string refund_status { get; set; }
        public string captured { get; set; }
        public string description { get; set; }
        public string card_id { get; set; }
        public string bank { get; set; }
        public string wallet { get; set; }
        public string vpa { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        public Notes notes { get; set; }
        public string fee { get; set; }
        public string tax { get; set; }
        public string error_code { get; set; }
        public string error_description { get; set; }
        public string created_at { get; set; }
    }


    public class Item
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string source { get; set; }
        public string recipient { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public int amount_reversed { get; set; }
        public Notes notes { get; set; }
        public int fees { get; set; }
        public int? tax { get; set; }
        public bool on_hold { get; set; }
        public object on_hold_until { get; set; }
        public object recipient_settlement_id { get; set; }
        public int created_at { get; set; }
        public string utr { get; set; }
        public List<object> linked_account_notes { get; set; }
        public RecipientSettlement recipient_settlement { get; set; }
        // public string id { get; set; }
        // public string entity { get; set; }
        // public int amount { get; set; }
        // public string currency { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public object invoice_id { get; set; }
        public bool international { get; set; }
        public string method { get; set; }
        public int amount_refunded { get; set; }
        public object refund_status { get; set; }
        public bool captured { get; set; }
        public string description { get; set; }
        public string card_id { get; set; }
        public object bank { get; set; }
        public object wallet { get; set; }
        public string vpa { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        // public Notes notes { get; set; }
        public int fee { get; set; }
        // public int? tax { get; set; }
        public string error_code { get; set; }
        public string error_description { get; set; }
        //public int created_at { get; set; }
    }

    public class TransferResponse
    {
        public string entity { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }
    }
    public class TransferItemResponse
    {

        public string id { get; set; }
        public string entity { get; set; }
        public string source { get; set; }
        public string recipient { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string amount_reversed { get; set; }
        public Notes notes { get; set; }
        public string fees { get; set; }
        public string tax { get; set; }
        public string on_hold { get; set; }
        public string on_hold_until { get; set; }
        public string recipient_settlement_id { get; set; }
        public RecipientSettlement recipient_settlement { get; set; }
        public int created_at { get; set; }
        public List<object> linked_account_notes { get; set; }
        //  public string id { get; set; }
        //  public string entity { get; set; }
        //  public int amount { get; set; }
        //  public string currency { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public object invoice_id { get; set; }
        public bool international { get; set; }
        public string method { get; set; }
        public int amount_refunded { get; set; }
        public object refund_status { get; set; }
        public bool captured { get; set; }
        public string description { get; set; }
        public string card_id { get; set; }
        public object bank { get; set; }
        public object wallet { get; set; }
        public object vpa { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        //  public Notes notes { get; set; }
        public int fee { get; set; }
        // public int? tax { get; set; }
        public string error_code { get; set; }
        public string error_description { get; set; }
        //  public int created_at { get; set; }
    }
    public class RecipientSettlement
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string fees { get; set; }
        public string tax { get; set; }
        public string utr { get; set; }
        public string created_at { get; set; }
    }
    #endregion

    public class FEE_PAYU_REQUEST
    {
        public string request_id { get; set; }
        public string key { get; set; }
        public string txnid { get; set; }
        public string amount { get; set; }
        public string productinfo { get; set; }
        public string firstname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string lastname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zipcode { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string pg { get; set; }
        public string hash { get; set; }

    }
    public class FEE_MERCHANT_ACCOUNT_INFO
    {
        public string MERCHANT_ACCOUNT_INFO { get; set; }
        public string ACCOUNT_ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string KEY { get; set; }
        public string SALT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string BASE_URL { get; set; }
        public string AUTHORIZATION { get; set; }
        public string API_TYPE { get; set; }
        public string HASH_SEQUENCE { get; set; }
        public string ACCOUNT_TYPE { get; set; }
        public string STUDENT_ID { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string STU_EMAILID { get; set; }
        public string STU_MOBILENO { get; set; }
        public string curl { get; set; }
        public string furl { get; set; }
        public string surl { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
    }
    public class fee_payu_response
    {
        public string PAYU_RESPONSE_ID { get; set; }
        public string key { get; set; }
        [Display(Name = "Transaction Id")]
        public string txnid { get; set; }
        [Display(Name = "Amount")]
        public string amount { get; set; }
        [Display(Name = "Description")]
        public string productinfo { get; set; }
        [Display(Name = "Name")]

        public string firstname { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Phone")]
        public string phone { get; set; }
        public string lastname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zipcode { get; set; }
        public string udf1 { get; set; }
        [Display(Name = "Frequency")]
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }
        public string udf7 { get; set; }
        public string udf8 { get; set; }
        public string udf9 { get; set; }
        public string udf10 { get; set; }
        public string hash { get; set; }
        public string payment_source { get; set; }
        public string PG_TYPE { get; set; }
        [Display(Name = "Bank ref_no")]
        public string bank_ref_num { get; set; }
        public string bankcode { get; set; }
        public string error { get; set; }
        public string error_Message { get; set; }
        public string name_on_card { get; set; }
        public string cardnum { get; set; }
        public string mode { get; set; }
        public string mihpayid { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Date")]
        public string addedon { get; set; }
        public string unmappedstatus { get; set; }
        public string additionalCharges { get; set; }
        public string RECEIPT_NO { get; set; }


    }
    public class PayUPost
    {
        public string key { get; set; }
        public string txnid { get; set; }
        [Display(Name = "Amount")]
        public string amount { get; set; }
        public string productinfo { get; set; }
        [Display(Name = "Name")]
        public string firstname { get; set; }
        [Display(Name = "E-Mail")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string email { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }
        public string udf7 { get; set; }
        public string udf8 { get; set; }
        public string udf9 { get; set; }
        public string udf10 { get; set; }
        public string salt { get; set; }
        public string hash { get; set; }
        public string BASE_URL { get; set; }
        public string sMessage { get; set; }
        [Required]
        [Display(Name = "Mobile")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        public string phone { get; set; }
        public string surl { get; set; }
        public string furl { get; set; }
        public string curl { get; set; }
        public string city { get; set; }
        public string address2 { get; set; }
        public string address1 { get; set; }
        public string lastname { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pg { get; set; }
        public string zipcode { get; set; }
    }
    public class FeeMainHead
    {
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string HEAD_ID { get; set; }
        public string FEE_ROOT { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string MAIN_HEAD { get; set; }
        public string HEAD { get; set; }
        public string STATUS { get; set; }
        public string SHIFT_NAME { get; set; }
        public string SHIFT { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string PASSBOOK_NO { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string FEE_CATEGORY_ID { get; set; }
        public string FEE_CATEGORY { get; set; }
        public string EXAM_HEAD_ID { get; set; }
        public string COST { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string PROGRAMME_LEVEL_ID { get; set; }
    }
    public class JsonFeeStudentAccountList
    {
        public List<FEE_STUDENT_ACCOUNT> StudentAccountList { get; set; }
    }
    public class JsonFeeTransactionList
    {
        public List<FEE_TRANSACTION> FeeTransactionList { get; set; }
    }
    public class FEE_STUDENT_ACCOUNT
    {
        public string RAZORPAY_ID { get; set; }
        public string STUDENT_AC_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string HEAD { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD_NAME { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string BRS_ID { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string SPECIAL_FEES_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string BANK { get; set; }
        public string CLASS_ID { get; set; }
        public string TEMP_ID { get; set; }
        public string IS_REFUND { get; set; }
        public string FEE_MODE { get; set; }
        public string SPONSOR_ID { get; set; }
        public string TO_BE_UPLOADED { get; set; }
        public string F_STUDENT_AC_ID { get; set; }
        public string WAIVE_ID { get; set; }
        public string FINE_DATE { get; set; }
        public string IS_CANCELLED_HEAD { get; set; }
        public string BALANCE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAID_AMOUNT { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string STU_EMAILID { get; set; }
        public string STU_MOBILENO { get; set; }
        public string STATUS { get; set; }
        public string MAIN_HEAD { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string NO_OF_STUDENTS { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string PAYU_RESPONSE_ID { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string COLLECTED { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string ACCOUNT_ID { get; set; }
        public string ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string ACCOUNT_LABEL { get; set; }
        public string HOSTEL_ID { get; set; }
        public string HOSTEL_NAME { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string FEE_TRANSACTION_BANK { get; set; }
        public string IS_LEFT { get; set; }
        public string TERMFEE1 { get; set; }
        public string TERMFEE1DUE { get; set; }
        public string AMOUNT { get; set; }
        public string COST { get; set; }
        public string PAYMENTTYPEID { get; set; }
        public string INSTALLMENT_ID { get; set; }
    }
    public class FEE_STAFF_ACCOUNT
    {
        public string STAFF_AC_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string HEAD { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string BRS_ID { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string SPECIAL_FEES_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string BANK { get; set; }
        public string TEMP_ID { get; set; }
        public string IS_REFUND { get; set; }
        public string SPONSOR_ID { get; set; }
        public string TO_BE_UPLOADED { get; set; }
        public string F_STAFF_AC_ID { get; set; }
        public string WAIVE_ID { get; set; }
        public string FINE_DATE { get; set; }
        public string IS_CANCELLED_HEAD { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string STAFF_NAME { get; set; }
        public string STAFF_CODE { get; set; }
    }
    public class FEE_STAFF_TRANSACTION
    {
        public string TRANSACTION_ID { get; set; }
        public string STAFF_ID { get; set; }
        public string FREQUENCY { get; set; }
        public string CLASS { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DD_CHEQUE_NO { get; set; }
        public string COLLECTED { get; set; }
        public string DISCOUNT { get; set; }
        public string DEDUCT_STUDENT_ACCOUNT { get; set; }
        public string RECEIPT_NARRATION { get; set; }
        public string USERNAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_AMOUNT_COLLECTED { get; set; }
        public string IS_ACCOUNTANT_COLLECTED { get; set; }
        public string IS_ADVANCE { get; set; }
        public string UPLOAD_FLAG { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string FEE_TRANSACTION_COUNTER { get; set; }
        public string FEE_TRANSACTION_BANK { get; set; }
        public string TEMP_ID { get; set; }
        public string EXCESS_AMT { get; set; }
        public string F_TRANSACTION_ID { get; set; }
        public string FREQUENCY_TO { get; set; }
        public string CHALLAN_NO { get; set; }
        public string PayUResponse_Id { get; set; }
        public string RAZORPAY_ID { get; set; }
        public string EASEPAYID { get; set; }
        public string CF_ORDER_ID { get; set; }
    }
    public class FEE_STAFF_COLLECTION
    {
        public string COLLECTION_ID { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string HEAD { get; set; }
        public string PAID_AMOUNT { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string FREQUENCY { get; set; }
        public string RECEIPT_NO { get; set; }
        public string SORT_ORDER { get; set; }
        public string IS_DELETED { get; set; }
        public string TEMP_ID { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string FEE_RAZORPAY_TRANSFER_ID { get; set; }
        public string FEE_RAZORPAY_SETTLEMENT_ID { get; set; }
    }
    public class STAFF_FEE_EDVIRON_ORDER_INFO
    {
        public string EDVIRON_ORDER_ID { get; set; }
        public string SCHOOL_ID { get; set; }
        public string AMOUNT { get; set; }
        public string ACTUAL_AMOUNT { get; set; }
        public string CALLBACK_URL { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STUDENT_NAME { get; set; }
        public string STUDENT_EMAIL { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string CUSTOM_ORDER_ID { get; set; }
        public string COLLECT_REQUEST_ID { get; set; }
        public string COLLECT_REQUEST_URL { get; set; }
        public string SIGN { get; set; }
        public string STATUS { get; set; }
        public string IS_PAID { get; set; }
        public string SPLIT_PAYMENTS { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_PROCESSING { get; set; }
        public string REQ_WEBHOOK_URLS { get; set; }
        public string INSTITUTE_CODE { get; set; }
        public string RESPONSE_JSON { get; set; }
        public string PAYMENT_MODE_ID { get; set; }
        public string ENTRY_DATETIME { get; set; }
        public string UPDATED_AT { get; set; }
    }
    public class STAFF_FEE_EDVIRON_TRANSFER_INFO
    {
        public string TRANSFER_ID { get; set; }
        public string COLLECT_REQUEST_ID { get; set; }
        public string CUSTOM_ORDER_ID { get; set; }
        public string AMOUNT { get; set; }
        public string TRANSACTION_AMOUNT { get; set; }
        public string STATUS { get; set; }
        public string CURRENCY { get; set; }
        public string CREATED_AT { get; set; }
        public string UPDATED_AT { get; set; }
        public string BANK_REFERENCE { get; set; }
        public string PAYMENT_METHOD { get; set; }
        public string PAYMENT_DETAILS { get; set; }
        public string SCHOOL_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string STUDENT_NAME { get; set; }
        public string STUDENT_EMAIL { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string ENTRY_DATETIME { get; set; }
    }
    public class FEE_STUDENT_DUE
    {
        public string IS_APPROVED { get; set; }
        public string PREVIOUS_YEAR_TERM_DUE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string CLASS_YEAR { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string HEAD { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD_NAME { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string CLASS_ID { get; set; }
        public string BALANCE { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string STU_EMAILID { get; set; }
        public string STU_MOBILENO { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string IS_LEFT { get; set; }
        public string TERMFEE1 { get; set; }
        public string TERMFEE1DUE { get; set; }
        public string TERMFEE2 { get; set; }
        public string TERMFEE2DUE { get; set; }
        public string BUSFEE { get; set; }
        public string BUSFEEDUE { get; set; }
        public string EXAMFEE { get; set; }
        public string EXAMFEEDUE { get; set; }
        public string HOSTELFEE { get; set; }
        public string HOSTELFEEDUE { get; set; }
        public string IS_DELETED { get; set; }
        public string STU_DUE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ENTRY_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string CLASS_NAME { get; set; }
        public string TOTAL_DUE_AMOUNT { get; set; }
        public string TERMFEE1PAID { get; set; }
        public string TERMFEE2PAID { get; set; }
        public string BUSFEEPAID { get; set; }
        public string HOSTELFEEPAID { get; set; }
        public string TERMFEEPAID_DATE { get; set; }
        public string BUSFEEPAID_DATE { get; set; }
        public string HOSTELFEEPAID_DATE { get; set; }
        public string EXAMFEEPAID { get; set; }
        public string EXAMFEEPAID_DATE { get; set; }
        public string EXAMFEE1 { get; set; }
        public string EXAMFEE1PAID { get; set; }
        public string EXAMFEE1DUE { get; set; }
        public string EXAMFEE1PAID_DATE { get; set; }
        public string EXAMFEE2 { get; set; }
        public string EXAMFEE2PAID { get; set; }
        public string EXAMFEE2DUE { get; set; }
        public string EXAMFEE2PAID_DATE { get; set; }
        public string MONTH { get; set; }
        public string ESTABLISHMENT { get; set; }
        public string FeeAmount { get; set; }
        public string F_STUDENT_AC_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string TERM { get; set; }
        public string BUS_ROUTE { get; set; }
        public string STUDENT_AC_ID { get; set; }
        public string ROLE { get; set; }
        public string COLLEGE_FEE { get; set; }
        public string DEPARTMENT { get; set; }
        public string SHIFT { get; set; }
        public string SHIFT_NAME { get; set; }
        public string BUS_NO { get; set; }
        public string RELIGION { get; set; }
        public string UMIS_NO { get; set; }
        public string COMMUNITY { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string FATHER_NAME { get; set; }
        public string REASON { get; set; }
        public string TOPAY { get; set; }
        public string PAID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string CLASSLIST { get; set; }
        public string STAFF_CODE { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
       
    }
    public class EXAM_HALLTICKET_SPECIAL_APPROVALS
    {

        public string APPROVAL_ID { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string STUDENT_ID { get; set; }
        public string APPROVAL_REASON { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVAL_DATE { get; set; }
        public string VALID_FROM { get; set; }
        public string VALID_TO { get; set; }
        public string APPROVAL_DAYS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_APPROVED { get; set; }

    }
    public class FEE_TRANSACTION
    {
        public string TRANSACTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string FREQUENCY { get; set; }
        public string CLASS { get; set; }
        public string CLASS_LEVEL { get; set; }
        public string CLASS_YEAR { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DD_CHEQUE_NO { get; set; }
        public string COLLECTED { get; set; }
        public string DISCOUNT { get; set; }
        public string DEDUCT_STUDENT_ACCOUNT { get; set; }
        public string RECEIPT_NARRATION { get; set; }
        public string USERNAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ACC_YEAR { get; set; }
        public string PCOUNT { get; set; }
        public string FCOUNT { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_AMOUNT_COLLECTED { get; set; }
        public string IS_ACCOUNTANT_COLLECTED { get; set; }
        public string IS_ADVANCE { get; set; }
        public string UPLOAD_FLAG { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string FEE_TRANSACTION_COUNTER { get; set; }
        public string FEE_TRANSACTION_BANK { get; set; }
        public string TEMP_ID { get; set; }
        public string EXCESS_AMT { get; set; }
        public string F_TRANSACTION_ID { get; set; }
        public string FREQUENCY_TO { get; set; }
        public string CHALLAN_NO { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string BALANCE { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string TOTAL_AMOUNT { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string MONTH { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string PayUResponse_Id { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string DAY_ORDER_DATE { get; set; }
        public string DAY_ORDER_MONTH { get; set; }
        public string SHIFT { get; set; }
        public string HEAD { get; set; }
        public string MAIN_HEAD { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string HEAD_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PAID_AMOUNT { get; set; }
        public string RAZORPAY_ID { get; set; }
        public string HOSTEL_ID { get; set; }
        public string HOSTEL_NAME { get; set; }
        public string STATUS { get; set; }
        public string ORDER_ID { get; set; }
        public string RAZOR_PAY_ID { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string FEE_CATEGORY_ID { get; set; }
        public string PASSBOOK_NO { get; set; }
        public string FEE_MODE { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string CF_ORDER_ID { get; set; }

    }
    public class FEE_DISCOUNT
    {
        public string DISCOUNT_ID { get; set; }
        public string DISCOUNT_NAME { get; set; }
        public string IS_DELETED { get; set; }
        public string DISCOUNT_TYPE { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string DISCOUNT_VALUE { get; set; }
        public string IS_USED { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
    }
    public class JSON_FEE_DISCOUNT_ALLOTMENT
    {
        public List<FEE_DISCOUNT_ALLOTMENT> DiscountAllotmentList { get; set; }
    }
    public class FEE_DISCOUNT_ALLOTMENT
    {
        public string DISCOUNT_ALLOTMENT_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACCADEMIC_YEAR { get; set; }
        public string CLASS_ID { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string DISCOUNT_NAME { get; set; }
        public string DISCOUNT_VALUE { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string IS_DELETED { get; set; }
        public string UPLOAD_FLAG { get; set; }
        public string IS_UPDATED { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string FREQUENCY { get; set; }
        public string FEE_CATEGORY { get; set; }
        public string HEAD { get; set; }
        public string PROGRAMME { get; set; }
    }
    public class FEE_STRUCTURE_LIST
    {
        public List<FEE_STRUCTURE> FEE_STRUCTURE { get; set; }
        public List<FeeMainHead> JSON_FEE_MAIN_HEAD { get; set; }
    }
    public class FEE_STRUCTURE
    {
        public string FEE_STRUCTURE_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string CLASS { get; set; }
        public string CLASS_YEAR_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string HEAD { get; set; }
        public string HEAD_NAME { get; set; }
        public string MAIN_HEAD { get; set; }
        public string HEAD_ID { get; set; }
        public string AMOUNT { get; set; }
        public string IS_DELETED { get; set; }
        public string UPLOAD_FLAG { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string FEE_MODE { get; set; }
        public string FEE_CATEGORY { get; set; }
        public string FEE_CATEGORY_NAME { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string FEE_CATEGORY_ID { get; set; }
        public string PROGRAMME { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string TEMP_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CREDIT { get; set; }
        public string SHIFT { get; set; }
        public string APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE { get; set; }
        public string PROGRAMME_MODE_ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string SHIFT_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string PASSBOOK_NO { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string DEPARTMENT { get; set; }


    }
    public class FEE_DISCOUNT_FREQUENCY
    {
        public string DISCOUNT_FRE_ID { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
    }
    public class FEE_DISCOUNT_HEAD
    {
        public string DISCOUNT_HEAD_ID { get; set; }
        public string DISCOUNT_ID { get; set; }
        public string DISCOUNT_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
    }
    public class FEE_ADVANCE_PAYMENT
    {
        public string ADVANCE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string CLASS { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DD_CHEQUE_NO { get; set; }
        public string COLLECTED { get; set; }
        public string DEBIT { get; set; }
        public string RECEIPT_NARRATION { get; set; }
        public string USERNAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string IS_DELETED { get; set; }
        public string BRS_ID { get; set; }
        public string IS_AMOUNT_COLLECTED { get; set; }
        public string IS_ACCOUNTANT_COLLECTED { get; set; }
        public string UPLOAD_FLAG { get; set; }
        public string IS_DOWNLOADED { get; set; }
        public string IS_UPDATED { get; set; }
        public string DOWNLOAD_TIME { get; set; }
        public string FEE_TRANSACTION_COUNTER { get; set; }
        public string FEE_TRANSACTION_BANK { get; set; }
        public string TEMP_ID { get; set; }
        public string EXCESS_AMT { get; set; }
        public string CHALLAN_NO { get; set; }
        public string PayUResponse_Id { get; set; }
    }
    public class HALL_TICKET_VALIDATION
    {
        public string HALL_TICKET_VALIDATION_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string REASON { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }
    public class FEE_FREQUENCY_FEE_MODE
    {
        public string FEE_FREQUENCY_FEE_MODE_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FEE_MODE { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string CLASS_YEAR { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string PROGRAMME_MODE { get; set; }
        public string SHIFT { get; set; }
    }


    public class FEE_RAZORPAY_SETTLEMENT_INFO
    {
        public string SETTLEMENT_ID { get; set; }
        public string ID { get; set; }
        public string ENTITY { get; set; }
        public string AMOUNT { get; set; }
        public string STATUS { get; set; }
        public string FEES { get; set; }
        public string TAX { get; set; }
        public string UTR { get; set; }
        public string CREATED_AT { get; set; }
        public string RECIPIENT { get; set; }
    }

    public class FEE_RAZORPAY_ACCOUNTS
    {
        public string RAZORPAY_ACCOUNT_ID { get; set; }
        public string ACCOUNT_ID { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string ACCOUNT_EMAIL_ID { get; set; }
        public string ACTIVATED_AT { get; set; }
        public string CREATED_AT { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string RAZORPAY_ACCOUNT_TYPE_ID { get; set; }
        public string KEY { get; set; }
        public string SECRET_KEY { get; set; }
    }
    public class FEE_RAZORPAY_PAYMENT_INFO
    {
        public string RAZORPAY_PAMENT_ID { get; set; }
        public string ID { get; set; }
        public string ENTITY { get; set; }
        public string AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public string STATUS { get; set; }
        public string ORDER_ID { get; set; }
        public string INVOICE_ID { get; set; }
        public string INTERNATIONAL { get; set; }
        public string METHOD { get; set; }
        public string AMOUNT_REFUNDED { get; set; }
        public string REFUND_STATUS { get; set; }
        public string CAPTURED { get; set; }
        public string DESCRIPTION { get; set; }
        public string CARD_ID { get; set; }
        public string BANK { get; set; }
        public string WALLET { get; set; }
        public string VPA { get; set; }
        public string EMAIL { get; set; }
        public string CONTACT { get; set; }
        public string FEE { get; set; }
        public string TAX { get; set; }
        public string ERROR_CODE { get; set; }
        public string ERROR_DESCRIPTION { get; set; }
        public string CREATED_AT { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string RECEIPT_NO { get; set; }

        public string IS_DELETED { get; set; }
    }

    public class FEE_RAZORPAY_TRANSFER_INFO
    {
        public string RAZORPAY_TRANSFER_ID { get; set; }
        public string ID { get; set; }
        public string ENTITY { get; set; }
        public string SOURCE { get; set; }
        public string RECIPIENT { get; set; }
        public string AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public string AMOUNT_REVERSED { get; set; }
        public string FEES { get; set; }
        public string TAX { get; set; }
        public string ON_HOLD { get; set; }
        public string ON_HOLD_UNTIL { get; set; }
        public string RECIPIENT_SETTLEMENT_ID { get; set; }
        public string CREATED_AT { get; set; }
        public string LINKED_ACCOUNT_NOTES { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class FEE_RAZORPAY_ORDER_INFO
    {
        public string RAZORPAY_ORDER_ID { get; set; }
        public string ID { get; set; }
        public string ENTITY { get; set; }
        public string AMOUNT { get; set; }
        public string AMOUNT_PAID { get; set; }
        public string AMOUNT_DUE { get; set; }
        public string CURRENCY { get; set; }
        public string RECEIPT { get; set; }
        public string OFFER_ID { get; set; }
        public string STATUS { get; set; }
        public string ATTEMPTS { get; set; }
        public string CREATED_AT { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }

    public class FEE_REPORT : FEE_TRANSACTION
    {
        public string ADMISSION_NO { get; set; }
        public string PAYURESPONSE_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string NAME { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string ACCOUNT_ID { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string ID { get; set; }
        //  public string ORDER_ID { get; set; }
        public string CREATED_AT { get; set; }
        public string RECIPIENT_SETTLEMENT_ID { get; set; }
        public string AMOUNT { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string F_STUDENT_AC_ID { get; set; }
        public string IS_LEFT { get; set; }
    }

    public class Fee_Account
    {
        public string STUDENT_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string REGISTER_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string HEAD { get; set; }
        public string DEBIT { get; set; }
        public string CREDIT { get; set; }
        public string FEE_MAIN_HEAD_ID { get; set; }
        public string COUNT { get; set; }
        public string PROGRAMME_GROUP_CODE { get; set; }
    }

    public class STU_LATE_ENTRY
    {
        public string LATE_FEE_ID { get; set; }
        public string ENTRY_TIME { get; set; }
        public string STUDENT_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FINE_AMOUNT { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ENTRY_ID { get; set; }
        public string SEMESTER_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_FINE_ADDED { get; set; }
        public string IS_LEAVE_ADDED { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
       

        // Extra 
        public string COUNT { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAFF_CODE { get; set; }
        public string STAFF_NAME { get; set; }
        public string IS_CURDATE { get; set; }
        public string CLASS_NAME { get; set; }
        public string CLASS_CODE { get; set; }
        public string SHIFT_NAME { get; set; }
        public string FR_MOBILE { get; set; }
        public string LAST_NAME { get; set; }
        public string STU_MOBILENO { get; set; }
    }
    public class FEE_PAYMENT_MODE
    {

        private string PAYMENT_MODE_ID { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string PAYMENT_MODE_NAME { get; set; }
        private string IS_ACTIVE { get; set; }
        private string IS_DELETED { get; set; }
    }
    public class FEE_PAYMENT_MODE_CHARGES : FEE_PAYMENT_MODE
    {
        public string PAYMENT_MODE_CHARGE_ID { get; set; }
        public string PAYMENT_MODE_ID { get; set; }
        public string FROM_CHARGES_AMOUNT { get; set; }
        public string TO_CHARGES_AMOUNT { get; set; }
        public string IS_PERCENTAGE_OR_FLAT { get; set; }
        public string PERCENTAGE_OR_FLAT { get; set; }
        public string GST_ON_TRANSACTION_FEE { get; set; }
        public string TRANSACTION_FEE { get; set; }
        public string GST_AMOUNT_ON_TRANSACTION_FEE { get; set; }
        public string TOTAL_AMOUNT { get; set; }
        public string FEE_AMOUNT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string PAYMENT_GATEWAY_TYPE { get; set; }
    }
    public class FEE_CF_PAYMENT_INFO_BY_ORDER_ID
    {

        public string cf_payment_id { get; set; }
        public string order_id { get; set; }
        public string entity { get; set; }
        public string payment_currency { get; set; }
        public string order_amount { get; set; }
        public string is_captured { get; set; }
        public string payment_group { get; set; }
        public string authorization { get; set; }
        public string payment_method { get; set; }
        public string payment_amount { get; set; }
        public string payment_time { get; set; }
        public string payment_completion_time { get; set; }
        public string payment_status { get; set; }
        public string payment_message { get; set; }
        public string bank_reference { get; set; }
        public string auth_id { get; set; }
        public string PAYMENT_ID { get; set; }
        public string CF_PAYMENT_RESPONSE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class Order_tag
    {

        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public string INSTITUTE_CODE { get; set; }
        public string PROGRAME_OR_CLASS_ID { get; set; }
        public string PROGRAME_ORDER_ID { get; set; }
        public string USER_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string PAYMENT_METHODS { get; set; }
        public string actual_amount { get; set; }

    }
    public class CF_ORDER_INFO
    {
        public string fee_cf_order_id { get; set; }
        public string cf_order_id { get; set; }
        public string created_at { get; set; }
        public CustomerDetails customer_details { get; set; }
        public string entity { get; set; }
        public string order_amount { get; set; }
        public string order_currency { get; set; }
        public string order_expiry_time { get; set; }
        public string order_id { get; set; }
        public OrderMeta order_meta { get; set; }
        public object order_note { get; set; }
        public List<OrderSplit> order_splits { get; set; }
        public string order_status { get; set; }
        public Order_tag order_tags { get; set; }
        public string order_token { get; set; }
        public string payment_link { get; set; }
        public string CF_ORDER_RESPONSE { get; set; }
        public string CF_ORDER_REQUEST { get; set; }
        public Payments payments { get; set; }
        public Refunds refunds { get; set; }
        public Settlements settlements { get; set; }
        public string RECEIPT_NO { get; set; }
        public string ERROR_CODE { get; set; }
        public string ERROR_DESCRIPTION { get; set; }
        public string payment_session_id { get; set; }
        public string COLLECT_REQUEST_ID { get; set; }
        public string CUSTOM_ORDER_ID { get; set; }
        public string AMOUNT { get; set; }
        public string STATUS { get; set; }
        public string COLLECT_REQUEST_URL { get; set; }
        public string SIGN { get; set; }
        public string SCHOOL_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string CLIENT_KEY { get; set; }
        public string IS_PAID { get; set; }
        public string FEE_CF_ORDER_ID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string actual_amount { get; set; }

    }
    public class Settlements
    {
        public string url { get; set; }
    }
    public class CF_OrderRequest
    {
        public CustomerDetails customer_details { get; set; }
        public OrderMeta order_meta { get; set; }
        public Order_tag order_tags { get; set; }
        public List<OrderSplitep> order_splits { get; set; }

        public string order_id { get; set; }
        public string order_amount { get; set; }
        public string order_currency { get; set; }
        public string SCHOOL_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string AMOUNT { get; set; }
        public string CALLBACK_URL { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string STUDENT_EMAIL { get; set; }
        public string STUDENT_NAME { get; set; }
        public string ADDITIONAL_DATA { get; set; }
        public string CUSTOM_ORDER_ID { get; set; }
        public string SPLIT_PAYMENTS { get; set; }
        public string[] REQ_WEBHOOK_URLS { get; set; }
        public string VENDOR_INFO { get; set; }
        public string SIGN { get; set; }
        public string INSTITUTE_CODE { get; set; }
        public string STATUS { get; set; }
    }
    public class OrderSplitep
    {
        public string vendor_id { get; set; }
        public decimal amount { get; set; }
    }
    public class CustomerDetails
    {
        public string customer_id { get; set; }
        public object customer_name { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
    }

    public class OrderMeta
    {
        public object return_url { get; set; }
        public object notify_url { get; set; }
        public object payment_methods { get; set; }
    }

    public class Payments
    {
        public string url { get; set; }
    }

    public class Refunds
    {
        public string url { get; set; }
    }

    public class FEE_CF_VENDOR_WEBHOOK_SETTLEMENT_INFO
    {

        public string CF_VENDOR_SETTLEMENT_INFO_ID { get; set; }
        public string VENDOR_SETTLEMENT_RESPONSE { get; set; }

    }

    public class FEE_CF_SPLIT_INFO_BY_ORDER
    {

        public string SPLIT_INFO_ID { get; set; }
        public string CF_ORDER_ID { get; set; }
        public string CF_SPLIT_INFO { get; set; }
        public string CF_SPLIT_SETTLEMENT_INFO { get; set; }

    }

    public class FEE_CF_WEBHOOK_NOTIFY
    {

        public string CF_NOTIFY_URL_ID { get; set; }
        public string CF_NOTIFY_URL_RESPONSE { get; set; }


    }
    public class CF_SPLIT_RESPONSE_BY_ORDER
    {
        public string message { get; set; }
        public string status { get; set; }
        public string orderAmount { get; set; }
        public List<OrderSplit> orderSplit { get; set; }
        public string serviceCharge { get; set; }
        public string serviceTax { get; set; }
        public string splitServiceCharge { get; set; }
        public string splitServiceTax { get; set; }
        public string settlementAmount { get; set; }
        public string settlementEligiblityDate { get; set; }
        public List<Vendor> vendors { get; set; }
    }
    public class Vendor
    {
        public List<Split> split { get; set; }
    }
    public class Vendors
    {
        public string id { get; set; }
        public string settlementId { get; set; }
        public string settlementAmount { get; set; }
        public string settlementEligibilityDate { get; set; }
    }
    public class SettlemtnData
    {
        public Settlement settlement { get; set; }
        public DateTime event_time { get; set; }
        public string type { get; set; }
    }

    public class CF_VENDOR_WEBHOOK_FOR_SETTLEMENT
    {
        public SettlemtnData data { get; set; }
    }
    public class Settlement
    {
        public string _id { get; set; }
        public string settlementAmount { get; set; }
        public string adjustment { get; set; }
        public string netSettlementAmount { get; set; }
        public string fromDate { get; set; }
        public string tillDate { get; set; }
        public string status { get; set; }
        public string utrNumber { get; set; }
        public string settlementDate { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public int __v { get; set; }
        public string schoolId { get; set; }
    }

    public class CFSettlementResponse
    {
        public string page { get; set; }
        public string limit { get; set; }
        public List<Settlement> settlements { get; set; }
    }

    public class CF_SettlementRecon
    {
        public string amount { get; set; }
        public string settlement_eligibility_time { get; set; }
        public string merchant_order_id { get; set; }
        public string tx_time { get; set; }
        public string settlement_id { get; set; }
        public string settled { get; set; }
        public string fee { get; set; }
        public string tax { get; set; }
        public string entity_id { get; set; }
        public string type { get; set; }
        public string merchant_vendor_id { get; set; }
        public string added_on_time { get; set; }
        public string settlement_time { get; set; }
        public string settlement_utr { get; set; }
        public string currency { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string refund_arn { get; set; }
    }

    public class CFSettlementReconResponse
    {
        public object cursor { get; set; }
        public List<CF_SettlementRecon> data { get; set; }
        public int limit { get; set; }
    }

    public class FEE_CF_WEBHOOK_VENDOR_SETTLEMENT_FAILURE
    {
        public string CF_WEBHOOK_VENDOR_SETTLEMENT_FAILURE_ID { get; set; }
        public string CF_WEBHOOK_VENDOR_SETTLEMENT_FAILURE_RESPONSE { get; set; }
    }
    public class FEE_CF_WEBHOOK_VENDOR_SETTLEMENT_SUCCESS
    {
        public string CF_WEBHOOK_VENDOR_SETTLEMENT_SUCCESS_ID { get; set; }
        public string CF_WEBHOOK_VENDOR_SETTLEMENT_RESPONSE { get; set; }
    }
    public class CF_SETTLEMENT_RESPONSE_BY_ORDER
    {
        public string cf_payment_id { get; set; }
        public string order_id { get; set; }
        public string entity { get; set; }
        public string order_amount { get; set; }
        public string payment_time { get; set; }
        public string payment_amount { get; set; }
        public string service_charge { get; set; }
        public string service_tax { get; set; }
        public string settlement_amount { get; set; }
        public string cf_settlement_id { get; set; }
        public string transfer_id { get; set; }
        public string transfer_time { get; set; }
        public string transfer_utr { get; set; }
        public string order_currency { get; set; }
        public string settlement_currency { get; set; }
        public string settlement_time { get; set; }
        public string merchant_order_id { get; set; }
        public string forex_conversion_handling_charge { get; set; }
        public string forex_conversion_handling_tax { get; set; }
        public string forex_conversion_rate { get; set; }
        public string charges_currency { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string CF_ORDER_ID { get; set; }
        public string surcharge { get; set; }
        public string tax_on_surcharge { get; set; }
    }

    public class SaveCFOrderInfo
    {
        public string order_id { get; set; }
        public string order_amount { get; set; }
        public string actual_amount { get; set; }
        public string order_currency { get; set; }
        public string order_status { get; set; }
        public string SCHOOL_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_TYPE_ID { get; set; }
        public string AMOUNT { get; set; }
        public string CALLBACK_URL { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string STUDENT_EMAIL { get; set; }
        public string STUDENT_NAME { get; set; }
        public string ADDITIONAL_DATA { get; set; }
        public string CUSTOM_ORDER_ID { get; set; }
        public string SPLIT_PAYMENTS { get; set; }
        public string[] REQ_WEBHOOK_URLS { get; set; }
        public string VENDOR_INFO { get; set; }
        public string SIGN { get; set; }
        public string INSTITUTE_CODE { get; set; }
        public string STATUS { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string CF_ORDER_RESPONSE { get; set; }
        public string CF_ORDER_REQUEST { get; set; }
    }
    public class FEE_ORDER_AND_CLIENT_KEY
    {
        public string ORDER_AND_CLIENT_KEY { get; set; }
        public string ORDER_ID { get; set; }
        public string CLIENT_KEY { get; set; }
        public string CONNECTION_STRING { get; set; }
    }

    public class OrderSplit
    {
        public string vendor_id { get; set; }
        public decimal amount { get; set; }

    }
    public class FEE_CF_SETTING
    {

        public string FEE_CF_SETTING_ID { get; set; }
        public string BASE_URL { get; set; }
        public string CLIENT_ID { get; set; }
        public string SECRET_ID { get; set; }
        public string SCHOOL_ID { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string API_VERSION { get; set; }
        public string NOTIFY_URL { get; set; }
        public string RETURN_URL { get; set; }
        public string PG_KEY { get; set; }
        public string SIGNATURE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_LIVE { get; set; }
        public string RETURN_URL_SUCCESS { get; set; }
        public string RETURN_URL_FAILED { get; set; }
        public string WEBHOOK_URL { get; set; }
    }
    public class CF_SETTLEMENT_LIST_RESPONSE
    {
        public List<CF_SETTLEMENT_RESPONSE_BY_ORDER> data { get; set; }
        public Pagination pagination { get; set; }
    }
    public class Pagination
    {
        public string next_cursor { get; set; }
    }
    public class CF_RECON_VENDOR_RESPONSE
    {
        public List<CF_RECON_VENDOR_DATA> data { get; set; }

        public CF_RECON_VENDOR_PAGINATION pagination { get; set; }
    }
    public class CF_RECON_VENDOR_DATA
    {
        public decimal amount { get; set; }
        public string settlement_eligibility_time { get; set; }
        public string merchant_order_id { get; set; }
        public string tx_time { get; set; }
        public long settlement_id { get; set; }
        public bool settled { get; set; }
        public string fee { get; set; }
        public string tax { get; set; }
        public string entity_id { get; set; }
        public string type { get; set; }
        public string merchant_vendor_id { get; set; }
        public string added_on_time { get; set; }
        public string settlement_time { get; set; }
        public string settlement_utr { get; set; }
        public string currency { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string refund_arn { get; set; }
    }
    public class CF_RECON_VENDOR_PAGINATION
    {
        public int limit { get; set; }

        public string cursor { get; set; }
    }

    public class CF_SETTLEMENT_SEARCH_RESPONSE
    {
        public List<CF_SETTLEMENT_SEARCH_ITEM> data { get; set; }
    }

    public class CF_SETTLEMENT_SEARCH_ITEM
    {
        public long cf_settlement_id { get; set; }
        public string status { get; set; }
        public decimal settlement_amount { get; set; }
        public string settlement_time { get; set; }
        public string settlement_utr { get; set; }
    }

    // CashFree Order Settlement Response (for use with /orders/{order_id}/settlements)
    public class CF_ORDER_SETTLEMENT
    {
        public long cf_settlement_id { get; set; }
        public string order_id { get; set; }
        public string payment_time { get; set; }
        public string settlement_time { get; set; }
        public decimal settlement_amount { get; set; }
        public string settlement_utr { get; set; }
        public string status { get; set; }
    }
    public class Split
    {
        public string label { get; set; }
        public double amount { get; set; }
    }
    public class CIA_MALPRACTICE
    {
        public string CIA_MALPRACTICE_ID { get; set; }
        public string COURSE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string FR_MOBILE { get; set; }
        public string EXAM_SETTING_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public string ENTRY_ID { get; set; }
        public string ENTRY_DATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_TITLE { get; set; }
        public string COMPONENT { get; set; }
    }

    public class FEE_RECEIPT_GENERATION
    {

        public string PAID_AMOUNT { get; set; }
        public string ISSUE_ID { get; set; }
        public string RECEIVE_ID { get; set; }
        public string PROGRAMME_ID { get; set; }
        public string PROGRAMME_NAME { get; set; }
        public string ISSUE_DATE { get; set; }
        public string APPLICATION_NO { get; set; }
        public string OFFLINE_APPLICATION_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string CONTACT_NO { get; set; }
        public string SHIFT { get; set; }
        public string DOB { get; set; }
        public string HSC_NO { get; set; }
        public string CLASS_NAME { get; set; }
        public string PROGRAMME_GROUP_ID { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DD_CHEQUE_NO { get; set; }
        public string COLLECTED { get; set; }
        public string DISCOUNT { get; set; }
        public string DEDUCT_STUDENT_ACCOUNT { get; set; }
        public string PayUResponse_Id { get; set; }
        public string FREQUENCY { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string SMS_MOBILE_NO { get; set; }
        public string FEE_MODE { get; set; }
        public string FEE_ROOT_ID { get; set; }
        public string MAIN_HEAD { get; set; }
        public string PROGRAMME_GROUP_CODE { get; set; }
        public string DEBIT { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string IFSCODE { get; set; }
        public string BANK_ACCOUNT_NO { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string HEAD { get; set; }


    }

    public class SUP_BUS
    {
        public string BUS_ID { get; set; }
        public string BUS_NAME { get; set; }
        public string DRIVER_NAME { get; set; }
        public string ROUTE { get; set; }
        public string IS_ACIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ALLOTED_SEATS { get; set; }
        public string ACTIVE_STATUS { get; set; }
    }

    public class TRANSPORT_BUS_STAGE
    {
        public string BUS_STAGE_ID { get; set; }
        public string STAGE_NAME { get; set; }
        public string KM { get; set; }
        public string BUS_ID { get; set; }
        public string ALLOTED_SEATS { get; set; }
        public string AMOUNT { get; set; }
        public string AMOUNT_II { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string BUS_NAME { get; set; }
        public string ACTIVE_STATUS { get; set; }
    }

    public class TRANSPORT_BUS_REGISTRATION
    {
        public string BUS_REGISTERED_ID { get; set; }
        public string STAGE_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ROLE_ID { get; set; }
        public string ENTRY_TIME { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string STAGE_NAME { get; set; }
        public string BUS_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string ADMISSION_NO { get; set; }
    }

    public class HOSTEL_FEE_REPORT
    {
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string HOSTEL_NAME { get; set; }
        public string ROOM_NO { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string BALANCE { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string HOSTEL_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }

    public class BUS_FEE_REPORT
    {
        public string USER_TYPE { get; set; }
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public string BUS_ID { get; set; }
        public string BUS_NAME { get; set; }
        public string BUS_NO { get; set; }
        public string STAGE_ID { get; set; }
        public string STAGE_NAME { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string BALANCE { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }

    public class BUS_FEE_ACCOUNT_ITEM
    {
        public string USER_TYPE { get; set; }
        public long ACCOUNT_ID { get; set; }
        public long USER_ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string DETAILS { get; set; }
        public decimal ALLOCATED_AMOUNT { get; set; }
        public decimal PAID_AMOUNT { get; set; }
        public decimal BALANCE_AMOUNT { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string FREQUENCY_ID { get; set; }
        public int CAN_EDIT_DELETE { get; set; }
    }

    public class HOSTEL_HEAD_CODE_ITEM
    {
        public long FEE_MAIN_HEAD_ID { get; set; }
        public string MAIN_HEAD_ID { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD_NAME { get; set; }
    }

    public class EXAM_FEE_REPORT
    {
        public string STUDENT_ID { get; set; }
        public string ROLL_NO { get; set; }
        public string REGISTER_NO { get; set; }
        public string FIRST_NAME { get; set; }
        public string CLASS_ID { get; set; }
        public string CLASS_NAME { get; set; }
        public string MOBILE_NO { get; set; }
        public string HEAD_ID { get; set; }
        public string HEAD_CODE { get; set; }
        public string HEAD_NAME { get; set; }
        public string FREQUENCY_ID { get; set; }
        public string FREQUENCY_NAME { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string BALANCE { get; set; }
        public string PAYMENT_DATE { get; set; }
        public string RECEIPT_NO { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string ACADEMIC_YEAR { get; set; }
    }
}

