using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.PaymentGateway
{
    public class EdvironSQL
    {
        public static string GetEdvironSQL(EdvironSQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                // =====================================================
                // SETTINGS
                // =====================================================
                case EdvironSQLCommands.FetchEdvironSettings:
                    {
                        query = @"SELECT 
                                    EDVIRON_SETTING_ID,
                                    BASE_URL,
                                    CLIENT_ID,
                                    SECRET_ID,
                                    SCHOOL_ID,
                                    TRANSACTION_TYPE,
                                    API_VERSION,
                                    RETURN_URL_SUCCESS,
                                    WEBHOOK_URL,
                                    PG_KEY,
                                    IS_ACTIVE,
                                    IS_DELETED,
                                    IS_LIVE
                                FROM
                                    FEE_EDVIRON_SETTING
                                WHERE
                                    IS_ACTIVE = 1
                                    AND IS_DELETED !=1
                                    AND IS_LIVE = ?IS_LIVE;";
                        break;
                    }

                // =====================================================
                // STUDENT INFO (reuse existing student table)
                // =====================================================
                case EdvironSQLCommands.FetchStudentInfo:
                    {
                        query = @"SELECT 
                                    STUDENT_ID ,
                                    S.FIRST_NAME AS STUDENT_NAME,
                                    S.GSUITE_MAIL_ID AS STUDENT_EMAIL,
                                    S.STU_MOBILENO AS STUDENT_PHONE
                                FROM
                                    STU_PERSONAL_INFO AS S
                                WHERE
                                    S.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }

                // =====================================================
                // ORDER OPERATIONS
                // =====================================================
                case EdvironSQLCommands.SavePaymentOrderInfo:
                    {
                        query = @"INSERT INTO FEE_EDVIRON_ORDER_INFO 
                                    (SCHOOL_ID, AMOUNT, ACTUAL_AMOUNT, CALLBACK_URL, STUDENT_ID, 
                                     ACADEMIC_YEAR, STUDENT_NAME, STUDENT_EMAIL, 
                                     FREQUENCY_ID, FREQUENCY_TYPE_ID, CUSTOM_ORDER_ID, 
                                     STATUS, SPLIT_PAYMENTS, INSTITUTE_CODE, PAYMENT_MODE_ID, 
                                     REQ_WEBHOOK_URLS) 
                                  VALUES 
                                    (?SCHOOL_ID, ?AMOUNT, ?ACTUAL_AMOUNT, ?CALLBACK_URL, ?STUDENT_ID, 
                                     ?ACADEMIC_YEAR, ?STUDENT_NAME, ?STUDENT_EMAIL, 
                                     ?FREQUENCY_ID, ?FREQUENCY_TYPE_ID, ?CUSTOM_ORDER_ID, 
                                     ?STATUS, ?SPLIT_PAYMENTS, ?INSTITUTE_CODE, ?PAYMENT_MODE_ID, 
                                     ?REQ_WEBHOOK_URLS);";
                        break;
                    }

                case EdvironSQLCommands.UpdatePaymentOrderUrl:
                    {
                        query = @"UPDATE FEE_EDVIRON_ORDER_INFO 
                                  SET 
                                    COLLECT_REQUEST_ID = ?COLLECT_REQUEST_ID,
                                    COLLECT_REQUEST_URL = ?COLLECT_REQUEST_URL,
                                    SIGN = ?SIGN,
                                    STATUS = ?STATUS,
                                    RESPONSE_JSON = ?RESPONSE_JSON
                                  WHERE 
                                    CUSTOM_ORDER_ID = ?CUSTOM_ORDER_ID;";
                        break;
                    }

                case EdvironSQLCommands.IsOrderExisting:
                    {
                        query = @"SELECT 
                                    EDVIRON_ORDER_ID,
                                    CUSTOM_ORDER_ID,
                                    COLLECT_REQUEST_ID,
                                    COLLECT_REQUEST_URL,
                                    AMOUNT,
                                    STATUS,
                                    PAYMENT_MODE_ID,
                                    FREQUENCY_ID,
                                    FREQUENCY_TYPE_ID,
                                    ACADEMIC_YEAR
                                FROM 
                                    FEE_EDVIRON_ORDER_INFO 
                                WHERE 
                                    STUDENT_ID = ?STUDENT_ID 
                                    AND FREQUENCY_ID = ?FREQUENCY_ID 
                                    AND ACADEMIC_YEAR = ?ACADEMIC_YEAR
                                    AND IS_DELETED !=1
                                    AND UPPER(STATUS) NOT IN ('PAID','SUCCESS')
                                ORDER BY EDVIRON_ORDER_ID DESC
                                LIMIT 1;";
                        break;
                    }

                case EdvironSQLCommands.FetchOrderByCollectId:
                    {
                        query = @"SELECT 
                                    EDVIRON_ORDER_ID,
                                    SCHOOL_ID,
                                    AMOUNT,
                                    ACTUAL_AMOUNT,
                                    STUDENT_ID,
                                    ACADEMIC_YEAR,
                                    STUDENT_NAME,
                                    STUDENT_EMAIL,
                                    FREQUENCY_ID,
                                    FREQUENCY_TYPE_ID,
                                    CUSTOM_ORDER_ID,
                                    COLLECT_REQUEST_ID,
                                    COLLECT_REQUEST_URL,
                                    STATUS,
                                    IS_PAID,
                                    PAYMENT_MODE_ID,
                                    INSTITUTE_CODE
                                FROM 
                                    FEE_EDVIRON_ORDER_INFO 
                                WHERE 
                                    COLLECT_REQUEST_ID = ?COLLECT_REQUEST_ID
                                    AND IS_DELETED !=1
                                LIMIT 1;";
                        break;
                    }

                case EdvironSQLCommands.FetchOrderByCustomOrderId:
                    {
                        query = @"SELECT 
                                    EDVIRON_ORDER_ID,
                                    SCHOOL_ID,
                                    AMOUNT,
                                    ACTUAL_AMOUNT,
                                    STUDENT_ID,
                                    ACADEMIC_YEAR,
                                    STUDENT_NAME,
                                    STUDENT_EMAIL,
                                    FREQUENCY_ID,
                                    FREQUENCY_TYPE_ID,
                                    CUSTOM_ORDER_ID,
                                    COLLECT_REQUEST_ID,
                                    COLLECT_REQUEST_URL,
                                    STATUS,
                                    IS_PAID,
                                    PAYMENT_MODE_ID,
                                    INSTITUTE_CODE
                                FROM 
                                    FEE_EDVIRON_ORDER_INFO 
                                WHERE 
                                    CUSTOM_ORDER_ID = ?CUSTOM_ORDER_ID
                                    AND IS_DELETED !=1
                                LIMIT 1;";
                        break;
                    }

                case EdvironSQLCommands.UpdateOrderStatus:
                    {
                        query = @"UPDATE FEE_EDVIRON_ORDER_INFO 
                                  SET 
                                    STATUS = ?STATUS
                                  WHERE 
                                    CUSTOM_ORDER_ID = ?CUSTOM_ORDER_ID 
                                    AND IS_DELETED = 0;";
                        break;
                    }

                case EdvironSQLCommands.DeleteOrderInfo:
                    {
                        query = @"
                                    SET SQL_SAFE_UPDATES = 0;
                                    UPDATE FEE_EDVIRON_ORDER_INFO 
                                  SET IS_DELETED = 1 
                                  WHERE CUSTOM_ORDER_ID = ?CUSTOM_ORDER_ID; 
                                    SET SQL_SAFE_UPDATES = 1;";
                        break;
                    }

                case EdvironSQLCommands.DeleteOrderInfoByCollectId:
                    {
                        query = @"UPDATE FEE_EDVIRON_ORDER_INFO 
                                  SET IS_DELETED = 1 
                                  WHERE COLLECT_REQUEST_ID = ?COLLECT_REQUEST_ID;";
                        break;
                    }

                // =====================================================
                // PROCESSING LOCK (prevent duplicate fee entries)
                // =====================================================
                case EdvironSQLCommands.AcquireProcessingLock:
                    {
                        query = @"UPDATE FEE_EDVIRON_ORDER_INFO 
                                  SET IS_PROCESSING = 1 
                                  WHERE CUSTOM_ORDER_ID = ?CUSTOM_ORDER_ID 
                                    AND IS_PROCESSING = 0 
                                    AND IS_DELETED = 0;";
                        break;
                    }

                // =====================================================
                // TRANSFER / TRANSACTION INFO
                // =====================================================
                case EdvironSQLCommands.SaveTransferInfo:
                    {
                        query = @"INSERT INTO FEE_EDVIRON_TRANSFER_INFO 
                                    (COLLECT_REQUEST_ID, CUSTOM_ORDER_ID, AMOUNT, 
                                     TRANSACTION_AMOUNT, STATUS, CURRENCY, 
                                     CREATED_AT, UPDATED_AT, BANK_REFERENCE, 
                                     PAYMENT_METHOD, PAYMENT_DETAILS, SCHOOL_ID, 
                                     STUDENT_ID, STUDENT_NAME, STUDENT_EMAIL, 
                                     ACADEMIC_YEAR_ID)
                                  VALUES 
                                    (?COLLECT_REQUEST_ID, ?CUSTOM_ORDER_ID, ?AMOUNT, 
                                     ?TRANSACTION_AMOUNT, ?STATUS, ?CURRENCY, 
                                     ?CREATED_AT, ?UPDATED_AT, ?BANK_REFERENCE, 
                                     ?PAYMENT_METHOD, ?PAYMENT_DETAILS, ?SCHOOL_ID, 
                                     ?STUDENT_ID, ?STUDENT_NAME, ?STUDENT_EMAIL, 
                                     ?ACADEMIC_YEAR_ID);";
                        break;
                    }

                // =====================================================
                // WEBHOOK HISTORY
                // =====================================================
                case EdvironSQLCommands.SaveWebhookHistory:
                    {
                        query = @"INSERT INTO FEE_EDVIRON_WEBHOOK_HISTORY 
                                    (WEBHOOK_TYPE, RAW_PAYLOAD, COLLECT_REQUEST_ID, 
                                     CUSTOM_ORDER_ID, STATUS)
                                  VALUES 
                                    (?WEBHOOK_TYPE, ?RAW_PAYLOAD, ?COLLECT_REQUEST_ID, 
                                     ?CUSTOM_ORDER_ID, ?STATUS);";
                        break;
                    }

                // =====================================================
                // VENDOR ACCOUNTS (split payment)
                // =====================================================
                case EdvironSQLCommands.FetchVendorAccounts:
                    {
                        query = @"SELECT 
                                    EDVIRON_ACCOUNT_ID,
                                    ACCOUNT_NO,
                                    BANK_ACCOUNT_ID,
                                    VENDOR_ID,
                                    ACCOUNT_PURPOSE
                                FROM 
                                    FEE_EDVIRON_VENDOR_ACCOUNT 
                                WHERE 
                                     IS_DELETED !=1;";
                        break;
                    }

                // =====================================================
                // FETCH PAID ORDER FOR FEE ENTRY CHECK
                // =====================================================
                case EdvironSQLCommands.FetchPaidOrderForFeeEntry:
                    {
                        query = @"SELECT 
                                    o.EDVIRON_ORDER_ID,
                                    o.STUDENT_ID,
                                    o.STATUS,
                                    o.FREQUENCY_ID,
                                    o.FREQUENCY_TYPE_ID,
                                    o.ACADEMIC_YEAR_ID,
                                    o.CUSTOM_ORDER_ID,
                                    o.COLLECT_REQUEST_ID,
                                    o.AMOUNT
                                FROM 
                                    FEE_EDVIRON_ORDER_INFO AS o
                                LEFT JOIN 
                                    fee_transaction AS T ON T.CF_ORDER_ID = o.CUSTOM_ORDER_ID
                                WHERE 
                                    o.STATUS IN ('PAID','SUCCESS')
                                    AND o.IS_DELETED = 0
                                    AND o.ENTRY_DATETIME >= NOW() - INTERVAL 3 DAY
                                    AND T.TRANSACTION_ID IS NULL
                                ORDER BY o.EDVIRON_ORDER_ID DESC;";
                        break;
                    }
            }
            return query;
        }
    }
}
