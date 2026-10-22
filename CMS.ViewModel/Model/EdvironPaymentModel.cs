using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CMS.ViewModel.Model
{
    // =====================================================
    // EDVIRON PAYMENT GATEWAY MODELS
    // Adapted from School project for College (INT PKs)
    // =====================================================

    /// <summary>
    /// Edviron API settings stored in FEE_EDVIRON_SETTING table
    /// </summary>
    public class FEE_EDVIRON_SETTING_INFO
    {
        public string EDVIRON_SETTING_ID { get; set; }
        public string BASE_URL { get; set; }
        public string CLIENT_ID { get; set; }
        public string SECRET_ID { get; set; }
        public string SCHOOL_ID { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string API_VERSION { get; set; }
        public string RETURN_URL_SUCCESS { get; set; }
        public string WEBHOOK_URL { get; set; }
        public string PG_KEY { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_LIVE { get; set; }
    }

    /// <summary>
    /// Edviron order info stored in FEE_EDVIRON_ORDER_INFO table
    /// </summary>
    public class EDVIRON_ORDER_INFO
    {
        public string EDVIRON_ORDER_ID { get; set; }
        public string SCHOOL_ID { get; set; }
        public string AMOUNT { get; set; }
        public string ACTUAL_AMOUNT { get; set; }
        public string CALLBACK_URL { get; set; }
        public string STUDENT_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ACADEMIC_YEAR_ID { get; set; }
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
    }

    /// <summary>
    /// Edviron transaction/transfer info from webhook
    /// </summary>
    public class EDVIRON_TRANSFER_INFO_MODEL
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
    }

    /// <summary>
    /// Vendor account mapping for split payments
    /// </summary>
    public class EdvironVendorAccountModel
    {
        public string EDVIRON_ACCOUNT_ID { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string VENDOR_ID { get; set; }
        public string ACCOUNT_PURPOSE { get; set; }
    }

    /// <summary>
    /// Vendor info for split payment in API request
    /// </summary>
    public class EdvironVendorInfoModel
    {
        public string vendor_id { get; set; }
        public double? amount { get; set; }
        public double? percentage { get; set; }
    }

    /// <summary>
    /// Edviron Create Collect Request - API request body
    /// </summary>
    public class EdvironOrderRequestModel
    {
        [JsonProperty("school_id")]
        public string school_id { get; set; }

        [JsonProperty("amount")]
        public string amount { get; set; }

        [JsonProperty("callback_url")]
        public string callback_url { get; set; }

        [JsonProperty("student_id")]
        public string student_id { get; set; }

        [JsonProperty("student_name")]
        public string student_name { get; set; }

        [JsonProperty("student_email")]
        public string student_email { get; set; }

        [JsonProperty("additional_data")]
        public object additional_data { get; set; }

        [JsonProperty("req_webhook_urls")]
        public string[] req_webhook_urls { get; set; }

        [JsonProperty("receipt")]
        public string receipt { get; set; }

        [JsonProperty("custom_order_id")]
        public string custom_order_id { get; set; }

        [JsonProperty("payment_session_id")]
        public string payment_session_id { get; set; }

        [JsonProperty("sign")]
        public string sign { get; set; }

        [JsonProperty("split_payments")]
        public bool split_payments { get; set; }

        [JsonProperty("vendors_info")]
        public List<EdvironVendorInfoModel> vendors_info { get; set; }

        [JsonProperty("enabled_modes")]
        public string[] enabled_modes { get; set; }

        [JsonProperty("disabled_modes")]
        public string[] disabled_modes { get; set; }
    }

    /// <summary>
    /// Edviron API response for collect request creation and status check
    /// </summary>
    public class EdvironCollectResponseModel
    {
        public string Collect_Request_Id { get; set; }
        public string Collect_Request_Url { get; set; }
        public string Sign { get; set; }
        public string status { get; set; }
        public string amount { get; set; }
        public string transaction_amount { get; set; }
    }

    /// <summary>
    /// Edviron transaction info from webhook or API
    /// </summary>
    public class EdvironTransactionInfoModel
    {
        public string status { get; set; }
        public string amount { get; set; }
        public string transaction_amount { get; set; }
        public string payment_method { get; set; }
        public string bank_reference { get; set; }
        public string collect_id { get; set; }
        public string custom_order_id { get; set; }
        public string school_id { get; set; }
        public string student_id { get; set; }
        public string student_name { get; set; }
        public string student_email { get; set; }
        public string createdAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        public string transaction_time { get; set; }

        [JsonProperty("details")]
        public string payment_details { get; set; }

        public EdvironAdditionalDataModel additional_data { get; set; }
    }

    /// <summary>
    /// Additional data embedded in Edviron transactions
    /// </summary>
    public class EdvironAdditionalDataModel
    {
        public string uid { get; set; }
        public string acyear { get; set; }
        public EdvironStudentDetailsModel student_details { get; set; }
    }

    /// <summary>
    /// Student details within additional data
    /// </summary>
    public class EdvironStudentDetailsModel
    {
        public string student_id { get; set; }
        public string student_name { get; set; }
        public string student_email { get; set; }
        public string receipt { get; set; }
    }

    /// <summary>
    /// Edviron settlement webhook payload
    /// </summary>
    public class EdvironSettlementWebhookModel
    {
        public string adjustment { get; set; }
        public decimal amount_settled { get; set; }
        public decimal payment_amount { get; set; }
        public string payment_from { get; set; }
        public string payment_till { get; set; }
        public decimal service_charge { get; set; }
        public decimal service_tax { get; set; }
        public string settled_on { get; set; }
        public decimal settlement_amount { get; set; }
        public decimal settlement_charge { get; set; }
        public string settlement_id { get; set; }
        public string settlement_initiated_on { get; set; }
        public string status { get; set; }
        public string utr { get; set; }
    }

    /// <summary>
    /// Edviron refund webhook payload
    /// </summary>
    public class EdvironRefundWebhookModel
    {
        public string refund_id { get; set; }
        public decimal refund_amount { get; set; }
        public string status_description { get; set; }
        public string school_id { get; set; }
        public string order_id { get; set; }
    }

    /// <summary>
    /// Webhook history audit record
    /// </summary>
    public class EDVIRON_WEBHOOK_HISTORY
    {
        public string WEBHOOK_ID { get; set; }
        public string WEBHOOK_TYPE { get; set; }
        public string RAW_PAYLOAD { get; set; }
        public string COLLECT_REQUEST_ID { get; set; }
        public string CUSTOM_ORDER_ID { get; set; }
        public string STATUS { get; set; }
        public string IS_PROCESSED { get; set; }
    }

    /// <summary>
    /// View model for payment response page
    /// </summary>
    public class EdvironPaymentResponseViewModel
    {
        public string Status { get; set; }
        public string CollectRequestId { get; set; }
        public string CustomOrderId { get; set; }
        public string Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string Message { get; set; }
        public string StudentName { get; set; }
        public string FrequencyName { get; set; }
        public string PaymentDate { get; set; }
    }
}
