using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.PaymentGateway
{
    public class PaymentGatewaySQL
    {
        public static string GetPaymentGatewaySQL(PaymentGatewaySQLCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                case PaymentGatewaySQLCommands.FetchPaymentModes:
                    {
                        query = @"SELECT 
                                            PM.PAYMENT_MODE_ID,
                                            PM.PAYMENT_MODE,
                                            PM.PAYMENT_MODE_NAME,
                                            PMC.FROM_CHARGES_AMOUNT,
                                            PMC.PAYMENT_GATEWAY_TYPE,
                                            PMC.TO_CHARGES_AMOUNT,
                                            PMC.IS_PERCENTAGE_OR_FLAT,
                                            PMC.PERCENTAGE_OR_FLAT,
                                            PMC.GST_ON_TRANSACTION_FEE
                                        FROM
                                            FEE_PAYMENT_MODE AS PM
                                                INNER JOIN
                                            FEE_PAYMENT_MODE_CHARGES PMC ON PMC.PAYMENT_MODE_ID = PM.PAYMENT_MODE_ID
                                        WHERE
                                            PM.IS_ACTIVE = 1 AND PM.IS_DELETED != 1
                                                AND PMC.IS_DELETED != 1;
";
                        break;
                    }
                case PaymentGatewaySQLCommands.FetchStudentInfo:
                    {
                        query = @"SELECT 
                                    S.STUDENT_ID AS customer_id,
                                    S.FIRST_NAME AS customer_name,
                                    S.STU_EMAILID AS customer_email,
                                    S.STU_MOBILENO AS customer_phone
                                FROM
                                    stu_personal_info AS S
                                WHERE
                                    S.STUDENT_ID = ?STUDENT_ID;";
                        break;
                    }
                case PaymentGatewaySQLCommands.IsOrderExistingByPaymentMode:
                    {
                        query = @"SELECT fee_cf_order_id,
					                 cf_order_request->>'$.order_id'  as order_id,  
					                 cf_order_response->>'$.order_expiry_time'  as order_expiry_time, 
					                 cf_order_response->>'$.order_status'  as order_status,
					                 cf_order_response->>'$.payment_session_id'  as payment_session_id,actual_amount
                                        FROM
                                        FEE_CF_ORDER_INFO_?ACADEMIC_YEAR where 
                                             cf_order_request->'$.order_tags.FREQUENCY_ID'=?FREQUENCY_ID
                                             and cf_order_request->'$.order_tags.USER_ID'= ?USER_ID
                                             and cf_order_request->'$.order_meta.payment_methods'=?PAYMENT_METHODS and UPPER(JSON_UNQUOTE(JSON_EXTRACT(cf_order_response, '$.order_status'))) != 'PAID' and IS_DELETED!=1;";
                        break;
                    }
                case PaymentGatewaySQLCommands.FetchAPIKeyInfoByTransactionType:
                    {
                        query = @"SELECT 
                                    BASE_URL,
                                    CLIENT_ID,
                                    SECRET_ID,
                                    TRANSACTION_TYPE,
                                    API_VERSION,
                                    RETURN_URL,
                                    NOTIFY_URL,
                                    SIGNATURE
                                FROM
                                    FEE_CF_SETTING
                                WHERE
                                    TRANSACTION_TYPE = ?TRANSACTION_TYPE
                                        AND IS_ACTIVE = 1
                                        AND IS_DELETED != 1
                                        AND IS_LIVE = ?IS_LIVE;";
                        break;
                    }
                case PaymentGatewaySQLCommands.SaveCFOrderInfoRequest:
                    {
                        query = @"INSERT INTO FEE_CF_ORDER_INFO_?AC_YEAR(ORDER_ID,CF_ORDER_REQUEST,UDF1,UDF2,UDF3,UDF4,UDF5,UDF6,UDF7,order_amount,actual_amount) VALUES (?ORDER_ID,?CF_ORDER_REQUEST,?UDF1,?UDF2,?UDF3,?UDF4,?UDF5,?UDF6,?UDF7,?order_amount,?actual_amount);";
                        break;
                    }
                case PaymentGatewaySQLCommands.FetchOrderInfoByOrderId:
                    {
                        query = @"
                                 CREATE TABLE IF NOT EXISTS `FEE_CF_ORDER_INFO_?AC_YEAR` (
                                   `fee_cf_order_id` int(11) NOT NULL AUTO_INCREMENT,
                                   `cf_order_id` varchar(50) DEFAULT NULL,
                                   `created_at` varchar(50) DEFAULT NULL,
                                   `customer_details_customer_id` varchar(20) DEFAULT NULL,
                                   `customer_details_customer_name` varchar(150) DEFAULT NULL,
                                   `customer_details_customer_email` varchar(150) DEFAULT NULL,
                                   `customer_details_customer_phone` varchar(30) DEFAULT NULL,
                                   `entity` varchar(20) DEFAULT NULL,
                                   `order_amount` double(11,2) DEFAULT NULL,
                                   `order_currency` varchar(5) DEFAULT NULL,
                                   `order_expiry_time` varchar(50) DEFAULT NULL,
                                   `order_id` varchar(60) DEFAULT NULL,
                                   `actual_amount` int(11) DEFAULT '0',
                                   `order_meta_return_url` varchar(1024) DEFAULT NULL,
                                   `order_meta_notify_url` varchar(1024) DEFAULT NULL,
                                   `order_meta_payment_methods` varchar(1024) DEFAULT NULL,
                                   `order_note` varchar(500) DEFAULT NULL,
                                   `order_splits` varchar(500) DEFAULT NULL,
                                   `order_status` varchar(10) DEFAULT NULL,
                                   `order_tags_frequency_id` varchar(30) DEFAULT NULL,
                                   `order_tags_frequency_type` varchar(30) DEFAULT NULL,
                                   `order_tags_institute_code` varchar(30) DEFAULT NULL,
                                   `order_tags_programe_or_class_id` varchar(1024) DEFAULT NULL,
                                   `order_tags_user_id` varchar(1024) DEFAULT NULL,
                                   `order_tags_user_type` varchar(1024) DEFAULT NULL,
                                   `order_token` varchar(1024) DEFAULT NULL,
                                   `payment_link` varchar(1024) DEFAULT NULL,
                                   `payments_url` varchar(1024) DEFAULT NULL,
                                   `refunds_url` varchar(1024) DEFAULT NULL,
                                   `settlements_url` varchar(1024) DEFAULT NULL,
                                   `cf_order_response` json DEFAULT NULL,
                                   `cf_order_request` json DEFAULT NULL,
                                   `UDF1` varchar(150) DEFAULT NULL,
                                   `UDF2` varchar(150) DEFAULT NULL,
                                   `UDF3` varchar(150) DEFAULT NULL,
                                   `UDF4` varchar(150) DEFAULT NULL,
                                   `UDF5` varchar(150) DEFAULT NULL,
                                   `UDF6` varchar(150) DEFAULT NULL,
                                   `UDF7` varchar(100) DEFAULT NULL,
                                   `entry_datetime` datetime DEFAULT CURRENT_TIMESTAMP,
                                   `IS_ACTIVE` int(11) DEFAULT '0',
                                   `IS_DELETED` int(11) DEFAULT '0',
                                   PRIMARY KEY (`fee_cf_order_id`)
                                 ) ;
                                 
                                 SELECT FEE_CF_ORDER_ID,order_id,actual_amount,CF_ORDER_REQUEST,CF_ORDER_RESPONSE,UDF1,UDF2,UDF3,UDF4,UDF5,UDF6,UDF7
                                             FROM FEE_CF_ORDER_INFO_?AC_YEAR where 
                                                  cf_order_request->'$.order_id'=?ORDER_ID AND IS_DELETED!=1;";
                        break;
                    }
                case PaymentGatewaySQLCommands.UpdateCFOrderResponseInfoAsJson:
                    {
                        query = @"update FEE_CF_ORDER_INFO_?AC_YEAR set cf_order_response=?CF_ORDER_RESPONSE where fee_cf_order_id=?fee_cf_order_id;";
                        break;
                    }
                case PaymentGatewaySQLCommands.SaveSplitInfo:
                    {
                        query = @"INSERT INTO `fee_cf_split_info_by_order`
                                        (
                                        `CF_ORDER_ID`,
                                        `CF_SPLIT_INFO`)
                                        VALUES
                                        (
                                        ?CF_ORDER_ID,
                                        ?CF_SPLIT_INFO
                                        );";
                        break;
                    }
                case PaymentGatewaySQLCommands.DeleteCFOderInfo:
                    {
                        query = @"update fee_cf_order_info_?AC_YEAR set IS_DELETED=1 where fee_cf_order_id=?fee_cf_order_id;";
                        break;
                    }
                case PaymentGatewaySQLCommands.FetchCFPaidOrdersForSettlement:
                    {
                        query = @"select o.cf_order_response->>'$.order_tags.USER_ID' AS STUDENT_ID ,
                                    o.cf_order_response->>'$.order_status' as STATUS, o.cf_order_response->>'$.order_tags.FREQUENCY_ID' AS FREQUENCY_ID,
                                    o.cf_order_response->>'$.order_tags.FREQUENCY_TYPE' AS FREQUENCY_TYPE_ID,
                                    o.cf_order_response->>'$.order_tags.ACADEMIC_YEAR_ID' AS ACADEMIC_YEAR,
                                    O.cf_order_request->>'$.order_id' as CUSTOM_ORDER_ID
                                    from fee_cf_order_info_?AC_YEAR as o 
                                    INNER JOIN fee_transaction AS T ON T.CF_ORDER_ID=O.ORDER_ID
                                    INNER JOIN FEE_COLLECTION AS FC on T.TRANSACTION_ID =FC.TRANSACTION_ID
								    AND FC.SETTLEMENT_DATE is null
                                    where o.cf_order_response->>'$.order_status'='PAID' GROUP BY T.TRANSACTION_ID;";
                        break;
                    }
                case PaymentGatewaySQLCommands.UpdateSettlementInfoByOrderid:
                    {

                        query = @"UPDATE FEE_COLLECTION AS FC
                                   INNER JOIN
                                   FEE_TRANSACTION AS T on T.TRANSACTION_ID =FC.TRANSACTION_ID
                                   AND T.ACADEMIC_YEAR =?AC_YEAR
                                   INNER JOIN
                                   FEE_CF_ORDER_INFO_?AC_YEAR AS O ON T.CF_ORDER_ID=O.ORDER_ID
                                   SET 
                                   FC.SETTLEMENT_DATE =unix_timestamp(?SETTLEMENT_DATE)
                                    WHERE 
                                    FC.SETTLEMENT_DATE is null AND COLLECTION_ID!=0 AND  cf_order_request->'$.order_id'=?CF_ORDER_ID;";
                        break;
                    }
                case PaymentGatewaySQLCommands.FetchCFOrderByForFeeEntry:
                    {
                        query = @"select o.cf_order_response->>'$.order_tags.USER_ID' AS STUDENT_ID ,
                                    o.cf_order_response->>'$.order_status' as STATUS, o.cf_order_response->>'$.order_tags.FREQUENCY_ID' AS FREQUENCY_ID,
                                    o.cf_order_response->>'$.order_tags.FREQUENCY_TYPE' AS FREQUENCY_TYPE_ID,
                                    o.cf_order_response->>'$.order_tags.ACADEMIC_YEAR_ID' AS ACADEMIC_YEAR,
                                    O.cf_order_request->>'$.order_id' as CUSTOM_ORDER_ID
                                    from fee_cf_order_info_2025 as o 
                                    LEFT JOIN fee_transaction AS T ON  T.STUDENT_ID=O.udf1 and t.FREQUENCY=o.udf3
                                    where entry_datetime > CURDATE() - INTERVAL ?NO_OF_BACK_DAYS DAY ;";
                        break;
                    }
            }
            return query;
        }
    }
}
