using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class ResponseFromSolutionInfi
    {
        public string status { get; set; }
        public List<Datum> data { get; set; }
        public string message { get; set; }
    }
    public class Datum
    {
        public string id { get; set; }
        public string customid { get; set; }
        public string customid1 { get; set; }
        public string customid2 { get; set; }
        public string mobile { get; set; }
        public string status { get; set; }
    }

    public class sms
    {
        public string to { get; set; }
        public string msgid { get; set; }
        public string message { get; set; }
        public string custom { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string CreditCount { get; set; }
        public string sender { get; set; }
    }
    public class ComposeMessageList
    {
        public string message { get; set; }
        public string sender { get; set; }
        public List<sms> sms { get; set; }
        public int unicode { get; set; }
        public int flash { get; set; }
        public string dlrurl { get; set; }

    }
    public class ResponseStatus
    {
        public string id { get; set; }
        public string mobile { get; set; }
        public string senttime { get; set; }
        public string dlrtime { get; set; }
        public string customid { get; set; }
        public string customid1 { get; set; }
        public string customid2 { get; set; }
        public string status { get; set; }
        public string sender { get; set; }
        public string provider { get; set; }
        public string location { get; set; }
    }

    public class ComposeResponcesms
    {
        public string status { get; set; }
        public List<ResponseStatus> data { get; set; }
        public string message { get; set; }
    }
    public class TestSMS
    {
        public string CONTENT { get; set; }
        public string RECEIPIENT_NOS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SMS_MODE { get; set; }
    }
    public class SMS_SETTING
    {
        public string SMS_SETTING_ID { get; set; }
        public string BASE_URL { get; set; }
        public string API_KEY { get; set; }
        public string METHOD { get; set; }
        public string FORMAT { get; set; }
        public string UNICODE { get; set; }
        public string FLASH { get; set; }
        public string DLRURL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string API_MODE { get; set; }
        public string SENDER { get; set; }
        public string REQUEST_METHOD { get; set; }
        public string API_METHOD { get; set; }
        public string USER { get; set; }
        public string ACCUSAGE { get; set; }
        public string ENTITY_ID { get; set; }



    }
    public class SMSBalence
    {
        public string SMS_BALANCE_ID { get; set; }
        public string SMS_DATE { get; set; }
        public string RECEIVED_CREDITS { get; set; }
        public string USED_CREDITS { get; set; }
        public string BALANCE_CREDITS { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string SMS_LIMIT { get; set; }
    }
    public class UpdateSMSBalance
    {
        public string USED_CREDITS { get; set; }
        public string BALANCE_CREDITS { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class SENT_SMS_2017
    {
        public string SENT_SMS_ID { get; set; }
        public string CLASS_IDS { get; set; }
        [DisplayName("Date")]
        public string DATE { get; set; }
        [DisplayName("Sent Time")]
        public string START_TIME { get; set; }
        [DisplayName("End Time")]
        public string END_TIME { get; set; }
        [DisplayName("Message")]
        public string CONTENT { get; set; }
        public string RECEIPIENT_NOS { get; set; }
        [DisplayName("No.of.SMS")]
        public string SMS_COUNT { get; set; }
        [DisplayName("Credit")]
        public string CREDIT_COUNT { get; set; }
        [DisplayName("Type")]
        public string SMS_MODE { get; set; }
        public string JOB_ID { get; set; }
        [DisplayName("Status")]
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string TRANSACTION_ID { get; set; }

    }
    public class FetchClassWiseMobileNumber
    {
        public string MOBILE { get; set; }
        public string RECEIPIENT_ID { get; set; }
        public string CLASS_ID { get; set; }
        public string IS_STAFF { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
    }
    public class FetchClassStaffNumber
    {
        public string MOBILE { get; set; }
        public string CLASS_ID { get; set; }
        public string RECEIPIENT_ID { get; set; }
        public string IS_STAFF { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
    }
    public class FetchAllMobileNumber
    {
        public string MOBILE { get; set; }
        public string RECEIPIENT_ID { get; set; }
        public string IS_STAFF { get; set; }
        public string CLASS_ID { get; set; }
    }

    public class SENT_SMS_LIST_2017
    {
        public string SENT_SMS_LIST_ID { get; set; }
        [DisplayName("Code")]
        public string CODE { get; set; }
        [DisplayName("Name")]
        public string NAME { get; set; }
        public string DATE { get; set; }
        public string COMPOSED_TIME { get; set; }
        public string SENT_TIME { get; set; }
        [DisplayName("Content")]
        public string CONTENT { get; set; }
        public string RECEIPIENT_ID { get; set; }
        [DisplayName("Class")]
        public string CLASS { get; set; }
        public string CREDIT_COUNT { get; set; }
        [DisplayName("Mobile No")]
        public string MOBILE_NO { get; set; }
        [DisplayName("Sent Date")]
        public string SENT_DATE { get; set; }
        [DisplayName("Delivered Date")]
        public string DELIVERED_DATE { get; set; }
        [DisplayName("Status")]
        public string STATUS { get; set; }
        public string SENT_ITEM_ID { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string IS_STAFF { get; set; }
        public string FIRST_NAME { get; set; }
        public string ROLL_NO { get; set; }
        public string CLASS_NAME { get; set; }
        public string MESSAGE_TYPE_NAME { get; set; }
    }
    public class FetchSentItemIds
    {
        public string SENT_ITEM_ID { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string CONTENT { get; set; }
        public string SENT_SMS_LIST_ID { get; set; }

    }
    public class SMS_USERDEFINED_TEMPLATE
    {
        public string USERDEFINED_TEMPLATE_ID { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string TEMPLATE_TEXT { get; set; }
        public string FIELDS_IDS { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string SMS_TEMPLATE_MODULE_ID { get; set; }
        public string TEMPLATE_NAME { get; set; }
    }


    public class SMS_TEMPLATE_MODULE
    {
        public string SMS_TEMPLATE_ID { get; set; }
        public string SMS_TEMPLATE { get; set; }
        public string SMS_MODULE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class EMAIL_SETTING
    {
        public string EMAIL_SETTING_ID { get; set; }
        public string SMTP_HOST { get; set; }
        public string SMTP_PORT { get; set; }
        public string SMTP_ENABLE { get; set; }
        public string FROM_EMAIL_ID { get; set; }
        public string FROM_EMAIL_PASSWORD { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }

    }

    public class SMS_VENDOR_DETAILS
    {

        public string SMS_VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public string VENDOR_PHONE { get; set; }
        public string URL { get; set; }
        public string ACCOUNT_MANAGER_NO { get; set; }
        public string SUPPORT_EMAIL_ID { get; set; }
        public string SALES_EMAIL_ID { get; set; }
        public string INTEGREATION_EMAIL_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }

    public class SentMessage
    {
        public string SENT_SMS_ID { get; set; }
        public string CLASS_IDS { get; set; }
        [DisplayName("Date")]
        public string DATE { get; set; }
        [DisplayName("Sent Time")]
        public string START_TIME { get; set; }
        [DisplayName("End Time")]
        public string END_TIME { get; set; }
        [DisplayName("Message")]
        public string CONTENT { get; set; }
        public string RECEIPIENT_NOS { get; set; }
        [DisplayName("No.of.SMS")]
        public string SMS_COUNT { get; set; }
        [DisplayName("Credit")]
        public string CREDIT_COUNT { get; set; }
        [DisplayName("Type")]
        public string SMS_MODE { get; set; }
        public string JOB_ID { get; set; }
        [DisplayName("Status")]
        public string STATUS { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string SHEDULE_TIME { get; set; }

    }
}
